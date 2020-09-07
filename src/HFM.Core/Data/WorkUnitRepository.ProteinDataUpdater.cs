
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Threading;

namespace HFM.Core.Data
{
    public enum WorkUnitProteinUpdateScope
    {
        All,
        Unknown,
        Project,
        Id
    }

    public class ProteinDataUpdater
    {
        private readonly IWorkUnitRepository _repository;
        private readonly SQLiteConnection _connection;

        public ProteinDataUpdater(IWorkUnitRepository repository) : this(repository, null)
        {

        }

        public ProteinDataUpdater(IWorkUnitRepository repository, SQLiteConnection connection)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _connection = connection;
        }

        private (bool NewConnection, SQLiteConnection Connection, SQLiteTransaction Transaction) GetOpenConnection()
        {
            var newConnection = false;
            var connection = _connection;
            SQLiteTransaction transaction = null;

            if (connection is null)
            {
                connection = _repository.CreateConnection();
                connection.Open();
                transaction = connection.BeginTransaction();
                newConnection = true;
            }

            return (newConnection, connection, transaction);
        }

        public void Execute(IProgress<ProgressInfo> progress, CancellationToken cancellationToken, WorkUnitProteinUpdateScope scope, long id)
        {
            const string workUnitNameUnknown = "WorkUnitName = '' OR WorkUnitName = 'Unknown'";

            bool newConnection = false;
            SQLiteConnection connection = null;
            SQLiteTransaction transaction = null;
            try
            {
                (newConnection, connection, transaction) = GetOpenConnection();
                switch (scope)
                {
                    case WorkUnitProteinUpdateScope.All:
                    case WorkUnitProteinUpdateScope.Unknown:
                        {
                            var selectSql = PetaPoco.Sql.Builder.Select("ProjectID").From("WuHistory");
                            if (scope == WorkUnitProteinUpdateScope.Unknown)
                            {
                                selectSql = selectSql.Where(workUnitNameUnknown);
                            }
                            selectSql = selectSql.GroupBy("ProjectID");

                            using (var table = _repository.Select(connection, selectSql.SQL))
                            {
                                int count = 0;
                                int lastProgress = 0;
                                foreach (DataRow row in table.Rows)
                                {
                                    cancellationToken.ThrowIfCancellationRequested();

                                    var projectId = row.Field<int>("ProjectID");
                                    var updateSql = GetUpdateSql(projectId, "ProjectID", projectId);
                                    if (updateSql != null)
                                    {
                                        if (scope == WorkUnitProteinUpdateScope.Unknown)
                                        {
                                            updateSql = updateSql.Where(workUnitNameUnknown);
                                        }
                                        using (var database = new PetaPoco.Database(connection))
                                        {
                                            database.Execute(updateSql);
                                        }
                                    }
                                    count++;

                                    int progressPercentage = Convert.ToInt32((count / (double)table.Rows.Count) * 100);
                                    if (progressPercentage != lastProgress)
                                    {
                                        string message = String.Format(CultureInfo.CurrentCulture, "Updating project {0} of {1}.", count, table.Rows.Count);
                                        progress?.Report(new ProgressInfo(progressPercentage, message));
                                        lastProgress = progressPercentage;
                                    }
                                }
                            }
                            break;
                        }
                    case WorkUnitProteinUpdateScope.Project:
                        {
                            int projectId = (int)id;
                            var updateSql = GetUpdateSql(projectId, "ProjectID", projectId);
                            if (updateSql != null)
                            {
                                using (var database = new PetaPoco.Database(connection))
                                {
                                    database.Execute(updateSql);
                                }
                            }
                            break;
                        }
                    case WorkUnitProteinUpdateScope.Id:
                        {
                            var selectSql = PetaPoco.Sql.Builder.Select("ProjectID").From("WuHistory").Where("ID = @0", id);

                            using (var table = _repository.Select(connection, selectSql.SQL, selectSql.Arguments))
                            {
                                if (table.Rows.Count != 0)
                                {
                                    var projectId = table.Rows[0].Field<int>("ProjectID");
                                    var updateSql = GetUpdateSql(projectId, "ID", id);
                                    if (updateSql != null)
                                    {
                                        using (var database = new PetaPoco.Database(connection))
                                        {
                                            database.Execute(updateSql);
                                        }
                                    }
                                }
                            }
                            break;
                        }
                }

                transaction?.Commit();
            }
            catch (Exception)
            {
                transaction?.Rollback();
                throw;
            }
            finally
            {
                if (newConnection)
                {
                    transaction?.Dispose();
                    connection?.Dispose();
                }
            }
        }

        private PetaPoco.Sql GetUpdateSql(int projectId, string column, long arg)
        {
            // get the correct protein
            var protein = _repository.ProteinService?.Get(projectId);
            if (protein != null)
            {
                var updateSql = PetaPoco.Sql.Builder.Append("UPDATE [WuHistory]")
                   .Append("SET [WorkUnitName] = @0,", protein.WorkUnitName)
                   .Append("[KFactor] = @0,", protein.KFactor)
                   .Append("[Core] = @0,", protein.Core)
                   .Append("[Frames] = @0,", protein.Frames)
                   .Append("[Atoms] = @0,", protein.NumberOfAtoms)
                   .Append("[Credit] = @0,", protein.Credit)
                   .Append("[PreferredDays] = @0,", protein.PreferredDays)
                   .Append("[MaximumDays] = @0", protein.MaximumDays)
                   .Where(column + " = @0", arg);
                return updateSql;
            }

            return null;
        }
    }
}
