
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Threading;

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

    public interface IWorkUnitRepository : IDisposable
    {
        /// <summary>
        /// Flag that notes if the Database is safe to call.
        /// </summary>
        bool Connected { get; }

        IProteinService ProteinService { get; }

        ILogger Logger { get; }

        // TODO: Idea rename to Upsert and also capture frame data (i.e. benchmark data)
        bool Insert(WorkUnitModel workUnitModel);

        int Delete(WorkUnitRow row);

        IList<WorkUnitRow> Fetch(WorkUnitQuery query, BonusCalculation bonusCalculation);

        PetaPoco.Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation);

        long CountCompleted(string clientName, DateTime? clientStartTime);

        long CountFailed(string clientName, DateTime? clientStartTime);

        SQLiteConnection CreateConnection();

        DataTable Select(SQLiteConnection connection, string sql, params object[] args);
    }

    public partial class WorkUnitRepository : IWorkUnitRepository
    {
        private string ConnectionString => String.Concat("Data Source=", FilePath, ";DateTimeKind=Utc");

        public string FilePath { get; private set; }

        /// <summary>
        /// Flag that notes if the Database is safe to call
        /// </summary>
        public bool Connected { get; private set; }

        public IProteinService ProteinService { get; }

        private ILogger _logger;
        public ILogger Logger => _logger ?? (_logger = NullLogger.Instance);

        private readonly IMapper _mapper;

        private static readonly Dictionary<WorkUnitRepositoryTable, SqlTableCommands> _SqlTableCommandDictionary =
            new Dictionary<WorkUnitRepositoryTable, SqlTableCommands>
            {
                { WorkUnitRepositoryTable.WuHistory, new WuHistorySqlTableCommands() },
                { WorkUnitRepositoryTable.Version, new VersionSqlTableCommands() }
            };

        public const string DefaultFileName = "WuHistory.db3";

        #region Constructor

        public WorkUnitRepository(IProteinService proteinService, ILogger logger)
        {
            ProteinService = proteinService ?? throw new ArgumentNullException(nameof(proteinService));
            _logger = logger;
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
            return RequiresUpgrade(Application.ParseVersionNumber(version), VersionString092);
        }

        public void Upgrade()
        {
            Upgrade(null);
        }

        public void Upgrade(IProgress<ProgressInfo> progress)
        {
            if (!Connected) return;

            int versionNumber = Application.ParseVersionNumber(GetDatabaseVersion());
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

        private void UpgradeToVersion092(int versionNumber, IProgress<ProgressInfo> progress)
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
                        var duplicateDeleter = new DuplicateDeleter(this, connection);
                        duplicateDeleter.Execute(progress);
                        // add columns to WuHistory table
                        AddProteinColumns(connection);
                        // update the WuHistory table with protein info
                        var proteinDataUpdater = new ProteinDataUpdater(this, connection);
                        proteinDataUpdater.Execute(progress, CancellationToken.None, default, default);
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

        private static bool RequiresUpgrade(int versionNumber, string upgradeVersionString)
        {
            return versionNumber < Application.ParseVersionNumber(upgradeVersionString);
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

        public bool Insert(WorkUnitModel workUnitModel)
        {
            if (!ValidateWorkUnit(workUnitModel.WorkUnit))
            {
                return false;
            }

            // ensure the given work unit is not written more than once
            if (WorkUnitExists(workUnitModel.WorkUnit))
            {
                return false;
            }

            var entry = _mapper.Map<WorkUnitRow>(workUnitModel);
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
                    database.Insert(entry);
                }
            }

            return true;
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
                    return database.Fetch<WorkUnitRow>(select);
                }
            }
        }

        public PetaPoco.Page<WorkUnitRow> Page(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return PageInternal(page, itemsPerPage, query, bonusCalculation);
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
                    return database.Page<WorkUnitRow>(page, itemsPerPage, select);
                }
            }
        }

        #endregion

        #region Select

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        private DataTable Select(string sql, params object[] args)
        {
            using (var connection = CreateConnection())
            {
                connection.Open();
                return Select(connection, sql, args);
            }
        }

        public DataTable Select(SQLiteConnection connection, string sql, params object[] args)
        {
            using (var database = new PetaPoco.Database(connection))
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
            //GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
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

        internal bool TableExists(SQLiteConnection connection, WorkUnitRepositoryTable databaseTable)
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

        internal void CreateTable(SQLiteConnection connection, WorkUnitRepositoryTable databaseTable)
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
