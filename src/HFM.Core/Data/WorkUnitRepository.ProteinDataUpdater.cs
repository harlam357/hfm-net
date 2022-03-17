using System.Data;
using System.Globalization;

using HFM.Core.WorkUnits;

using Microsoft.Data.Sqlite;

namespace HFM.Core.Data
{
    public class ProteinDataUpdater
    {
        private readonly IWorkUnitDatabase _database;
        private readonly IProteinService _proteinService;
        private readonly SqliteConnection _connection;

        public ProteinDataUpdater(IWorkUnitDatabase database, IProteinService proteinService) : this(database, proteinService, null)
        {

        }

        public ProteinDataUpdater(IWorkUnitDatabase database, IProteinService proteinService, SqliteConnection connection)
        {
            _database = database ?? throw new ArgumentNullException(nameof(database));
            _proteinService = proteinService ?? NullProteinService.Instance;
            _connection = connection;
        }

        public void Execute(IProgress<ProgressInfo> progress, CancellationToken cancellationToken)
        {
            const string selectSql = "SELECT ProjectID FROM WuHistory GROUP BY ProjectID";

            using (var table = _database.Select(_connection, selectSql))
            {
                int count = 0;
                int lastProgress = 0;
                foreach (DataRow row in table.Rows)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    var projectId = row.Field<long>("ProjectID");
                    var updateSql = GetUpdateSql(projectId, "ProjectID", projectId);
                    if (updateSql != null)
                    {
                        _database.Execute(_connection, updateSql.SQL, updateSql.Arguments);
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
        }

        private PetaPoco.Sql GetUpdateSql(long projectId, string column, long arg)
        {
            // get the correct protein
            var protein = _proteinService.Get((int)projectId);
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
