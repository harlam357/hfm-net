using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;

using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.Data.Sqlite;

namespace HFM.Core.Data
{
    internal enum WorkUnitRepositoryTable
    {
        WuHistory,
        DbVersion
    }

    /// <summary>
    /// Represents a general interface to the work unit history database capable of selecting and executing via ad hoc sql statements.
    /// </summary>
    public interface IWorkUnitDatabase
    {
        DataTable Select(string sql, params object[] args);

        DataTable Select(SqliteConnection connection, string sql, params object[] args);

        int Execute(SqliteConnection connection, string sql, params object[] args);
    }

    public class WorkUnitRepository : IWorkUnitDatabase, IDisposable
    {
        private string ConnectionString => String.Concat("Data Source=", FilePath);

        private string FilePath { get; set; }

        public bool Connected { get; private set; }

        public ILogger Logger { get; }

        public IProteinService ProteinService { get; }

        private static readonly Dictionary<WorkUnitRepositoryTable, SqlTableCommands> _SqlTableCommandDictionary =
            new()
            {
                { WorkUnitRepositoryTable.WuHistory, new WuHistorySqlTableCommands() },
                { WorkUnitRepositoryTable.DbVersion, new VersionSqlTableCommands() }
            };

        public const string DefaultFileName = "WuHistory.db3";

        public WorkUnitRepository(ILogger logger, IProteinService proteinService)
        {
            Logger = logger ?? NullLogger.Instance;
            ProteinService = proteinService ?? NullProteinService.Instance;
        }

        public void Initialize(string filePath)
        {
            FilePath = filePath;
            try
            {
                bool exists = TableExists(WorkUnitRepositoryTable.WuHistory);
                if (!exists)
                {
                    CreateTable(WorkUnitRepositoryTable.WuHistory);
                    CreateTable(WorkUnitRepositoryTable.DbVersion);
                    SetDatabaseVersion(Application.Version);
                }
                // verify the connection by performing a db operation
                using (var table = Select("SELECT * FROM [WuHistory] LIMIT 1"))
                {
                    Debug.Assert(table != null);
                }
                Connected = true;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                Connected = false;

                throw;
            }
        }

        private SqliteConnection CreateConnection() => new(ConnectionString);

        public bool RequiresUpgrade()
        {
            if (!Connected) return false;

            var version = GetDatabaseVersion();
            Logger.Info($"WU History database v{version}");
            return RequiresUpgrade(Version.Parse(version), VersionString092);
        }

        public void Upgrade(IProgress<ProgressInfo> progress = null)
        {
            if (!Connected) return;

            var versionNumber = Version.Parse(GetDatabaseVersion());
            try
            {
                UpgradeToVersion092(versionNumber, progress);
            }
            catch (Exception ex)
            {
                Connected = false;
                throw new DataException("Database upgrade failed.", ex);
            }
        }

        private const string VersionString092 = "0.9.2";

        private void UpgradeToVersion092(Version versionNumber, IProgress<ProgressInfo> progress)
        {
            if (!RequiresUpgrade(versionNumber, VersionString092)) return;

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        Logger.Info($"Performing WU History database upgrade to v{VersionString092}...");
                        // delete duplicates
                        var duplicateDeleter = new DuplicateDeleter(Logger, this, connection);
                        duplicateDeleter.Execute(progress);
                        // add columns to WuHistory table
                        AddProteinColumns(connection);
                        // update the WuHistory table with protein info
                        var proteinDataUpdater = new ProteinDataUpdater(this, ProteinService, connection);
                        proteinDataUpdater.Execute(progress, CancellationToken.None);
                        // set database version
                        SetDatabaseVersion(connection, VersionString092);

                        transaction.Commit();
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

        private static bool RequiresUpgrade(Version versionNumber, string upgradeVersionString)
        {
            return versionNumber < Version.Parse(upgradeVersionString);
        }

        private static void AddProteinColumns(SqliteConnection connection)
        {
            using (var adder = new SQLiteAddColumnCommand(_SqlTableCommandDictionary[WorkUnitRepositoryTable.WuHistory].TableName, connection))
            {
                adder.AddColumn("WorkUnitName", "TEXT");
                adder.AddColumn("KFactor", "REAL");
                adder.AddColumn("Core", "TEXT");
                adder.AddColumn("Frames", "INTEGER");
                adder.AddColumn("Atoms", "INTEGER");
                adder.AddColumn("Credit", "REAL");
                adder.AddColumn("PreferredDays", "REAL");
                adder.AddColumn("MaximumDays", "REAL");
                adder.Execute();
            }
        }

        private void SetDatabaseVersion(string version)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                SetDatabaseVersion(connection, version);
            }
        }

        private void SetDatabaseVersion(SqliteConnection connection, string version)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(version));

            if (!TableExists(connection, WorkUnitRepositoryTable.DbVersion))
            {
                CreateTable(connection, WorkUnitRepositoryTable.DbVersion);
            }

            Logger.Info($"Setting database version to: v{version}");
            using (var cmd = connection.CreateCommand())
            {
                cmd.CommandText = "INSERT INTO [DbVersion] (Version) VALUES (@0);";
                var param = new SqliteParameter("@0", version);
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
        }

        public IList<PetaPocoWorkUnitRow> Fetch()
        {
            Debug.Assert(TableExists(WorkUnitRepositoryTable.WuHistory));

            var result = new List<PetaPocoWorkUnitRow>();

            string select = _SqlTableCommandDictionary[WorkUnitRepositoryTable.WuHistory].SelectSql;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = select;
                    using var reader = command.ExecuteReader();
                    while (reader.Read())
                    {
                        var row = new PetaPocoWorkUnitRow();
                        row.ID = reader.GetFieldValue<long>("ID");
                        row.ProjectID = reader.GetFieldValue<int>("ProjectID");
                        row.ProjectRun = reader.GetFieldValue<int>("ProjectRun");
                        row.ProjectClone = reader.GetFieldValue<int>("ProjectClone");
                        row.ProjectGen = reader.GetFieldValue<int>("ProjectGen");
                        row.Username = reader.GetFieldValue<string>("Username");
                        row.Team = reader.GetFieldValue<int>("Team");
                        row.FramesCompleted = reader.GetFieldValue<int>("FramesCompleted");
                        row.WorkUnitName = reader.GetFieldValue<string>("WorkUnitName");
                        row.KFactor = reader.GetFieldValue<double>("KFactor");
                        row.Core = reader.GetFieldValue<string>("Core");
                        row.Frames = reader.GetFieldValue<int>("Frames");
                        row.Atoms = reader.GetFieldValue<int>("Atoms");
                        row.PreferredDays = reader.GetFieldValue<double>("PreferredDays");
                        row.MaximumDays = reader.GetFieldValue<double>("MaximumDays");
                        row.Name = reader.GetFieldValue<string>("InstanceName");
                        row.Path = reader.GetFieldValue<string>("InstancePath");
                        row.CoreVersion = reader.GetFieldValue<float>("CoreVersion");
                        row.FrameTimeValue = reader.GetFieldValue<int>("FrameTime");
                        row.ResultValue = reader.GetFieldValue<int>("Result");
                        row.Assigned = reader.GetFieldValue<DateTime>("DownloadDateTime");
                        row.Finished = reader.GetFieldValue<DateTime>("CompletionDateTime");
                        row.BaseCredit = reader.GetFieldValue<double>("Credit");
                        result.Add(row);
                    }
                }
            }

            return result;
        }

        public DataTable Select(string sql, params object[] args)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return Select(connection, sql, args);
            }
        }

        public DataTable Select(SqliteConnection connection, string sql, params object[] args)
        {
            var operatingConnection = connection;
            try
            {
                if (operatingConnection is null)
                {
                    operatingConnection = CreateConnection();
                    operatingConnection.Open();
                }

                using (var cmd = operatingConnection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    using (var reader = cmd.ExecuteReader())
                    {
                        var table = new DataTable();
                        table.Load(reader);
                        return table;
                    }
                }
            }
            finally
            {
                if (connection is null)
                {
                    operatingConnection?.Dispose();
                }
            }
        }

        public int Execute(SqliteConnection connection, string sql, params object[] args)
        {
            var operatingConnection = connection;
            try
            {
                if (operatingConnection is null)
                {
                    operatingConnection = CreateConnection();
                    operatingConnection.Open();
                }

                using (var cmd = operatingConnection.CreateCommand())
                {
                    cmd.CommandText = sql;
                    int count = 0;
                    foreach (var p in args)
                    {
                        cmd.Parameters.Add(new SqliteParameter($"@{count++}", p));
                    }
                    return cmd.ExecuteNonQuery();
                }
            }
            finally
            {
                if (connection is null)
                {
                    operatingConnection?.Dispose();
                }
            }
        }

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                }
            }

            _disposed = true;
        }

        private bool TableExists(WorkUnitRepositoryTable databaseTable)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return TableExists(connection, databaseTable);
            }
        }

        private static bool TableExists(SqliteConnection connection, WorkUnitRepositoryTable databaseTable)
        {
            using var command = connection.CreateCommand();
            using var table = command.GetSchema(databaseTable.ToString());
            return table.Rows.Count != 0;
        }

        private void CreateTable(WorkUnitRepositoryTable databaseTable)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                CreateTable(connection, databaseTable);
            }
        }

        private static void CreateTable(SqliteConnection connection, WorkUnitRepositoryTable databaseTable)
        {
            using (var command = _SqlTableCommandDictionary[databaseTable].GetCreateTableCommand(connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private const string VersionStringDefault = "0.0.0";

        internal string GetDatabaseVersion()
        {
            if (!TableExists(WorkUnitRepositoryTable.DbVersion))
            {
                return VersionStringDefault;
            }

            using (var table = Select("SELECT * FROM [DbVersion] ORDER BY ID DESC LIMIT 1;"))
            {
                if (table.Rows.Count != 0)
                {
                    return table.Rows[0]["Version"].ToString();
                }
            }

            return VersionStringDefault;
        }

        #region Private Helper Classes

        private abstract class SqlTableCommands
        {
            public abstract string TableName { get; }

            public virtual string SelectSql => String.Empty;

            public abstract DbCommand GetCreateTableCommand(SqliteConnection connection);
        }

        private class WuHistorySqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string WuHistoryTableName = "WuHistory";

            private static readonly string _WuHistoryTableCreateSql = "CREATE TABLE [{0}] (" +
                                                           "[ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                                           "[ProjectID] INTEGER NOT NULL," +
                                                           "[ProjectRun] INTEGER NOT NULL," +
                                                           "[ProjectClone] INTEGER NOT NULL," +
                                                           "[ProjectGen] INTEGER NOT NULL," +
                                                           "[InstanceName] TEXT NOT NULL," +
                                                           "[InstancePath] TEXT NOT NULL," +
                                                           "[Username] TEXT NOT NULL," +
                                                           "[Team] INTEGER NOT NULL," +
                                                           "[CoreVersion] REAL NOT NULL," +
                                                           "[FramesCompleted] INTEGER NOT NULL," +
                                                           "[FrameTime] INTEGER NOT NULL," +
                                                           "[Result] INTEGER NOT NULL," +
                                                           "[DownloadDateTime] DATETIME NOT NULL," +
                                                           "[CompletionDateTime] DATETIME NOT NULL," +
                                                           SetDefaultValue("[WorkUnitName] TEXT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[KFactor] REAL DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Core] TEXT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Frames] INTEGER DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Atoms] INTEGER DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Credit] REAL DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[PreferredDays] REAL DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[MaximumDays] REAL DEFAULT {0} NOT NULL") +
                                                           ");";

            private static string SetDefaultValue(string columnDef)
            {
                return String.Format(CultureInfo.InvariantCulture, columnDef, SQLiteAddColumnCommand.GetDefaultValue(columnDef));
            }

            private const string WuHistoryTableSelect = "SELECT [ID], " +
                                                               "[ProjectID], " +
                                                               "[ProjectRun], " +
                                                               "[ProjectClone], " +
                                                               "[ProjectGen], " +
                                                               "[InstanceName], " +
                                                               "[InstancePath], " +
                                                               "[Username], " +
                                                               "[Team], " +
                                                               "[CoreVersion], " +
                                                               "[FramesCompleted], " +
                                                               "[FrameTime], " +
                                                               "[Result], " +
                                                               "[DownloadDateTime], " +
                                                               "[CompletionDateTime], " +
                                                               "[WorkUnitName], " +
                                                               "[KFactor], " +
                                                               "[Core], " +
                                                               "[Frames], " +
                                                               "[Atoms], " +
                                                               "[Credit], " +
                                                               "[PreferredDays], " +
                                                               "[MaximumDays] " +
                                                               "FROM [WuHistory]";

            #endregion

            public override string TableName => WuHistoryTableName;

            public override string SelectSql => WuHistoryTableSelect;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input.")]
            public override DbCommand GetCreateTableCommand(SqliteConnection connection)
            {
                var command = connection.CreateCommand();
                command.CommandText = String.Format(CultureInfo.InvariantCulture,
                    _WuHistoryTableCreateSql, WuHistoryTableName);
                return command;
            }
        }

        private class VersionSqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string VersionTableName = "DbVersion";

            private const string VersionTableCreateSql = "CREATE TABLE [{0}] (" +
                                                         "[ID] INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                                         "[Version] TEXT NOT NULL);";

            #endregion

            public override string TableName => VersionTableName;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input.")]
            public override DbCommand GetCreateTableCommand(SqliteConnection connection)
            {
                var command = connection.CreateCommand();
                command.CommandText = String.Format(CultureInfo.InvariantCulture,
                    VersionTableCreateSql, VersionTableName);
                return command;
            }
        }

        #endregion
    }
}
