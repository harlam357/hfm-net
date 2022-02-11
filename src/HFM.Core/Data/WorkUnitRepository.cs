using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;

using AutoMapper;

using HFM.Core.Logging;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    internal enum WorkUnitRepositoryTable
    {
        WuHistory,
        Version
    }

    public class Page<T>
    {
        public Page()
        {

        }

        public Page(PetaPoco.Page<T> page)
        {
            CurrentPage = page.CurrentPage;
            TotalPages = page.TotalPages;
            TotalItems = page.TotalItems;
            ItemsPerPage = page.ItemsPerPage;
            Items = page.Items;
        }

        public long CurrentPage { get; set; }

        public long TotalPages { get; set; }

        public long TotalItems { get; set; }

        public long ItemsPerPage { get; set; }

        public IList<T> Items { get; set; }
    }

    public interface IWorkUnitRepository
    {
        /// <summary>
        /// Flag that notes if the Database is safe to call.
        /// </summary>
        bool Connected { get; }

        // TODO: Idea rename to Upsert and also capture frame data (i.e. benchmark data)
        long Insert(WorkUnitModel workUnitModel);

        int Delete(WorkUnitRow row);

        IList<WorkUnitRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation);

        Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation);

        long CountCompleted(string clientName, DateTime? clientStartTime);

        long CountFailed(string clientName, DateTime? clientStartTime);
    }

    /// <summary>
    /// Represents a general interface to the work unit history database capable of selecting and executing via ad hoc sql statements.
    /// </summary>
    public interface IWorkUnitDatabase
    {
        DataTable Select(string sql, params object[] args);

        DataTable Select(SQLiteConnection connection, string sql, params object[] args);

        int Execute(string sql, params object[] args);

        int Execute(SQLiteConnection connection, string sql, params object[] args);
    }

    public partial class WorkUnitRepository : IWorkUnitDatabase, IDisposable
    {
        private string ConnectionString => String.Concat("Data Source=", FilePath, ";DateTimeKind=Utc");

        public string FilePath { get; private set; }

        /// <summary>
        /// Flag that notes if the Database is safe to call
        /// </summary>
        public bool Connected { get; private set; }

        public ILogger Logger { get; }

        public IProteinService ProteinService { get; }

        private readonly IMapper _mapper;

        private static readonly Dictionary<WorkUnitRepositoryTable, SqlTableCommands> _SqlTableCommandDictionary =
            new Dictionary<WorkUnitRepositoryTable, SqlTableCommands>
            {
                { WorkUnitRepositoryTable.WuHistory, new WuHistorySqlTableCommands() },
                { WorkUnitRepositoryTable.Version, new VersionSqlTableCommands() }
            };

        public const string DefaultFileName = "WuHistory.db3";

        #region Constructor

        public WorkUnitRepository(ILogger logger, IProteinService proteinService)
        {
            Logger = logger ?? NullLogger.Instance;
            ProteinService = proteinService ?? NullProteinService.Instance;
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<WorkUnitRowProfile>()).CreateMapper();

            SQLiteFunction.RegisterFunction(typeof(ToSlotType));
            SQLiteFunction.RegisterFunction(typeof(GetProduction));
        }

        #endregion

        #region Methods

        public void Initialize(string filePath)
        {
            FilePath = filePath;
            try
            {
                bool exists = TableExists(WorkUnitRepositoryTable.WuHistory);
                if (!exists)
                {
                    CreateTable(WorkUnitRepositoryTable.WuHistory);
                    CreateTable(WorkUnitRepositoryTable.Version);
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

        public SQLiteConnection CreateConnection()
        {
            return new SQLiteConnection(ConnectionString);
        }

        #region Upgrade

        public bool RequiresUpgrade()
        {
            if (!Connected) return false;

            var version = GetDatabaseVersion();
            Logger.Info($"WU History database v{version}");
            return RequiresUpgrade(Version.Parse(version), VersionString092);
        }

        public void Upgrade()
        {
            Upgrade(null);
        }

        public void Upgrade(IProgress<ProgressInfo> progress)
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
                        proteinDataUpdater.Execute(progress, default, default, CancellationToken.None);
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

        private static void AddProteinColumns(SQLiteConnection connection)
        {
            using (var adder = new SQLiteAddColumnCommand(_SqlTableCommandDictionary[WorkUnitRepositoryTable.WuHistory].TableName, connection))
            {
                adder.AddColumn("WorkUnitName", "VARCHAR(30)");
                adder.AddColumn("KFactor", "FLOAT");
                adder.AddColumn("Core", "VARCHAR(20)");
                adder.AddColumn("Frames", "INT");
                adder.AddColumn("Atoms", "INT");
                adder.AddColumn("Credit", "FLOAT");
                adder.AddColumn("PreferredDays", "FLOAT");
                adder.AddColumn("MaximumDays", "FLOAT");
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

        private void SetDatabaseVersion(SQLiteConnection connection, string version)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(version));

            if (!TableExists(connection, WorkUnitRepositoryTable.Version))
            {
                CreateTable(connection, WorkUnitRepositoryTable.Version);
            }

            Logger.Info($"Setting database version to: v{version}");
            using (var cmd = new SQLiteCommand("INSERT INTO [DbVersion] (Version) VALUES (?);", connection))
            {
                var param = new SQLiteParameter("Version", DbType.String) { Value = version };
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Insert

        public long Insert(WorkUnitModel workUnitModel)
        {
            if (!ValidateWorkUnit(workUnitModel.WorkUnit))
            {
                return -1;
            }

            // ensure the given work unit is not written more than once
            if (WorkUnitExists(workUnitModel.WorkUnit))
            {
                return -1;
            }

            var entry = _mapper.Map<PetaPocoWorkUnitRow>(workUnitModel);
            // cannot map these two properties from a WorkUnit instance
            // they only live at the WorkUnitModel level
            entry.FramesCompleted = workUnitModel.FramesComplete;
            entry.FrameTimeValue = workUnitModel.GetRawTime(PPDCalculation.AllFrames);
            // copy protein values for insert
            entry.WorkUnitName = workUnitModel.CurrentProtein.WorkUnitName;
            entry.KFactor = workUnitModel.CurrentProtein.KFactor;
            entry.Core = workUnitModel.CurrentProtein.Core;
            entry.Frames = workUnitModel.CurrentProtein.Frames;
            entry.Atoms = workUnitModel.CurrentProtein.NumberOfAtoms;
            entry.BaseCredit = workUnitModel.CurrentProtein.Credit;
            entry.PreferredDays = workUnitModel.CurrentProtein.PreferredDays;
            entry.MaximumDays = workUnitModel.CurrentProtein.MaximumDays;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    return (long)database.Insert(entry);
                }
            }
        }

        private static bool ValidateWorkUnit(WorkUnit workUnit)
        {
            return workUnit.HasProject() &&
                   !workUnit.Assigned.IsMinValue() &&
                   !workUnit.Finished.IsMinValue();
        }

        private bool WorkUnitExists(WorkUnit workUnit)
        {
            var rows = Fetch(CreateWorkUnitQuery(workUnit), BonusCalculation.None);
            return rows.Count != 0;
        }

        private static WorkUnitQuery CreateWorkUnitQuery(WorkUnit workUnit)
        {
            return new WorkUnitQuery($"Query for existing {workUnit.ToProjectString()}")
                .AddParameter(WorkUnitRowColumn.ProjectID, WorkUnitQueryOperator.Equal, workUnit.ProjectID)
                .AddParameter(WorkUnitRowColumn.ProjectRun, WorkUnitQueryOperator.Equal, workUnit.ProjectRun)
                .AddParameter(WorkUnitRowColumn.ProjectClone, WorkUnitQueryOperator.Equal, workUnit.ProjectClone)
                .AddParameter(WorkUnitRowColumn.ProjectGen, WorkUnitQueryOperator.Equal, workUnit.ProjectGen)
                .AddParameter(WorkUnitRowColumn.Assigned, WorkUnitQueryOperator.Equal, workUnit.Assigned);
        }

        #endregion

        #region Delete

        public int Delete(WorkUnitRow row)
        {
            Debug.Assert(TableExists(WorkUnitRepositoryTable.WuHistory));
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    return database.Delete(row);
                }
            }
        }

        #endregion

        #region Fetch

        public IList<WorkUnitRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return FetchInternal(query, bonusCalculation);
            }
            finally
            {
                Logger.Debug($"Database Fetch ({query}) completed in {sw.GetExecTime()}");
            }
        }

        private IList<WorkUnitRow> FetchInternal(WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            Debug.Assert(TableExists(WorkUnitRepositoryTable.WuHistory));

            var select = new PetaPoco.Sql(_SqlTableCommandDictionary[WorkUnitRepositoryTable.WuHistory].SelectSql);
            select.Append(query);
            GetProduction.BonusCalculation = bonusCalculation;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    return database.Fetch<PetaPocoWorkUnitRow>(select).Cast<WorkUnitRow>().ToList();
                }
            }
        }

        public Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return new Page<WorkUnitRow>(PageInternal(page, itemsPerPage, query, bonusCalculation));
            }
            finally
            {
                Logger.Debug($"Database Page Fetch ({query}) completed in {sw.GetExecTime()}");
            }
        }

        private PetaPoco.Page<WorkUnitRow> PageInternal(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            Debug.Assert(TableExists(WorkUnitRepositoryTable.WuHistory));

            var select = new PetaPoco.Sql(_SqlTableCommandDictionary[WorkUnitRepositoryTable.WuHistory].SelectSql);
            select.Append(query);
            GetProduction.BonusCalculation = bonusCalculation;
            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    var result = database.Page<PetaPocoWorkUnitRow>(page, itemsPerPage, select);
                    return new PetaPoco.Page<WorkUnitRow>
                    {
                        Context = result.Context,
                        CurrentPage = result.CurrentPage,
                        Items = result.Items.Cast<WorkUnitRow>().ToList(),
                        ItemsPerPage = result.ItemsPerPage,
                        TotalItems = result.TotalItems,
                        TotalPages = result.TotalPages
                    };
                }
            }
        }

        #endregion

        #region Select

        public DataTable Select(string sql, params object[] args)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return Select(connection, sql, args);
            }
        }

        public DataTable Select(SQLiteConnection connection, string sql, params object[] args)
        {
            var operatingConnection = connection;
            try
            {
                if (operatingConnection is null)
                {
                    operatingConnection = CreateConnection();
                    operatingConnection.Open();
                }

                using (var database = new PetaPoco.Database(operatingConnection))
                using (var cmd = database.CreateCommand(database.Connection, sql, args))
                {
                    using (var adapter = new SQLiteDataAdapter((SQLiteCommand)cmd))
                    {
                        var table = new DataTable();
                        adapter.Fill(table);
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

        public int Execute(string sql, params object[] args)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return Execute(connection, sql, args);
            }
        }

        public int Execute(SQLiteConnection connection, string sql, params object[] args)
        {
            var operatingConnection = connection;
            try
            {
                if (operatingConnection is null)
                {
                    operatingConnection = CreateConnection();
                    operatingConnection.Open();
                }

                using (var database = new PetaPoco.Database(operatingConnection))
                {
                    return database.Execute(sql, args);
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

        #endregion

        #region Count

        public long CountCompleted(string clientName, DateTime? clientStartTime)
        {
            return Count(clientName, true, clientStartTime);
        }

        public long CountFailed(string clientName, DateTime? clientStartTime)
        {
            return Count(clientName, false, clientStartTime);
        }

        private long Count(string clientName, bool finished, DateTime? clientStartTime)
        {
            var query = new WorkUnitQuery()
                .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, clientName)
                .AddParameter(WorkUnitRowColumn.Result, finished ? WorkUnitQueryOperator.Equal : WorkUnitQueryOperator.NotEqual, (int)WorkUnitResult.FinishedUnit);

            if (clientStartTime.HasValue)
            {
                query.AddParameter(finished ? WorkUnitRowColumn.Finished : WorkUnitRowColumn.Assigned,
                    WorkUnitQueryOperator.GreaterThan, clientStartTime.Value);
            }

            var countSql = PetaPoco.Sql.Builder.Select("COUNT(*)")
               .From("WuHistory")
               .Append(query);

            using (var connection = CreateConnection())
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    return database.ExecuteScalar<long>(countSql);
                }
            }
        }

        #endregion

        #region IDisposable Members

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

        #endregion

        #region Table Helpers

        internal bool TableExists(WorkUnitRepositoryTable databaseTable)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return TableExists(connection, databaseTable);
            }
        }

        private static bool TableExists(SQLiteConnection connection, WorkUnitRepositoryTable databaseTable)
        {
            using (DataTable table = connection.GetSchema("Tables", new[] { null, null, _SqlTableCommandDictionary[databaseTable].TableName, null }))
            {
                return table.Rows.Count != 0;
            }
        }

        internal void CreateTable(WorkUnitRepositoryTable databaseTable)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                CreateTable(connection, databaseTable);
            }
        }

        private static void CreateTable(SQLiteConnection connection, WorkUnitRepositoryTable databaseTable)
        {
            using (var command = _SqlTableCommandDictionary[databaseTable].GetCreateTableCommand(connection))
            {
                command.ExecuteNonQuery();
            }
        }

        private const string VersionStringDefault = "0.0.0";

        internal string GetDatabaseVersion()
        {
            if (!TableExists(WorkUnitRepositoryTable.Version))
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

        #endregion

        #endregion

        #region Private Helper Classes

        private abstract class SqlTableCommands
        {
            public abstract string TableName { get; }

            public virtual string SelectSql => String.Empty;

            public abstract DbCommand GetCreateTableCommand(SQLiteConnection connection);
        }

        private class WuHistorySqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string WuHistoryTableName = "WuHistory";

            private static readonly string _WuHistoryTableCreateSql = "CREATE TABLE [{0}] (" +
                                                           "[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                                           "[ProjectID] INT  NOT NULL," +
                                                           "[ProjectRun] INT  NOT NULL," +
                                                           "[ProjectClone] INT  NOT NULL," +
                                                           "[ProjectGen] INT  NOT NULL," +
                                                           "[InstanceName] VARCHAR(60)  NOT NULL," +
                                                           "[InstancePath] VARCHAR(260)  NOT NULL," +
                                                           "[Username] VARCHAR(60)  NOT NULL," +
                                                           "[Team] INT  NOT NULL," +
                                                           "[CoreVersion] FLOAT  NOT NULL," +
                                                           "[FramesCompleted] INT  NOT NULL," +
                                                           "[FrameTime] INT  NOT NULL," +
                                                           "[Result] INT  NOT NULL," +
                                                           "[DownloadDateTime] DATETIME  NOT NULL," +
                                                           "[CompletionDateTime] DATETIME  NOT NULL," +
                                                           SetDefaultValue("[WorkUnitName] VARCHAR(30) DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[KFactor] FLOAT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Core] VARCHAR(20) DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Frames] INT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Atoms] INT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[Credit] FLOAT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[PreferredDays] FLOAT DEFAULT {0} NOT NULL,") +
                                                           SetDefaultValue("[MaximumDays] FLOAT DEFAULT {0} NOT NULL") +
                                                           ");";

            private static string SetDefaultValue(string columnDef)
            {
                return String.Format(CultureInfo.InvariantCulture, columnDef, SQLiteAddColumnCommand.GetDefaultValue(columnDef));
            }

            private const string WuHistoryTableSelect = "SELECT * FROM (SELECT [ID], " +
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
                                                                              "[MaximumDays], " +
                                                                              "ToSlotType(Core) As [SlotType], " +
                                                                              "CAST(GetProduction(FrameTime, Frames, Credit, KFactor, PreferredDays, MaximumDays, datetime(DownloadDateTime), datetime(CompletionDateTime), 0) As FLOAT) As [PPD], " +
                                                                              "CAST(GetProduction(FrameTime, Frames, Credit, KFactor, PreferredDays, MaximumDays, datetime(DownloadDateTime), datetime(CompletionDateTime), 1) As FLOAT) As [CalcCredit] " +
                                                                              "FROM [WuHistory])";

            #endregion

            public override string TableName => WuHistoryTableName;

            public override string SelectSql => WuHistoryTableSelect;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input.")]
            public override DbCommand GetCreateTableCommand(SQLiteConnection connection)
            {
                return new SQLiteCommand(connection)
                {
                    CommandText = String.Format(CultureInfo.InvariantCulture,
                                               _WuHistoryTableCreateSql, WuHistoryTableName)
                };
            }
        }

        private class VersionSqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string VersionTableName = "DbVersion";

            private const string VersionTableCreateSql = "CREATE TABLE [{0}] (" +
                                                         "[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                                         "[Version] VARCHAR(20)  NOT NULL);";

            #endregion

            public override string TableName => VersionTableName;

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Security", "CA2100:Review SQL queries for security vulnerabilities", Justification = "No user input.")]
            public override DbCommand GetCreateTableCommand(SQLiteConnection connection)
            {
                return new SQLiteCommand(connection)
                {
                    CommandText = String.Format(CultureInfo.InvariantCulture,
                                               VersionTableCreateSql, VersionTableName)
                };
            }
        }

        #endregion
    }
}
