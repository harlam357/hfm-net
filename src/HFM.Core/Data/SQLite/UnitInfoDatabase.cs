/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Threading.Tasks;

using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Data.SQLite
{
    public enum SqlTable
    {
        WuHistory,
        Version
    }

    public enum ProteinUpdateType
    {
        All,
        Unknown,
        Project,
        Id
    }

    public interface IUnitInfoDatabase : IDisposable
    {
        /// <summary>
        /// Flag that notes if the Database is safe to call.
        /// </summary>
        bool Connected { get; }

        void Upgrade();

        bool Insert(UnitInfoModel unitInfoModel);

        int Delete(HistoryEntry entry);

        IList<HistoryEntry> Fetch(QueryParameters parameters);

        IList<HistoryEntry> Fetch(QueryParameters parameters, BonusCalculationType bonusCalculation);

        PetaPoco.Page<HistoryEntry> Page(long page, long itemsPerPage, QueryParameters parameters);

        PetaPoco.Page<HistoryEntry> Page(long page, long itemsPerPage, QueryParameters parameters, BonusCalculationType bonusCalculation);

        DataTable Select(string sql, params object[] args);

        int Execute(string sql, params object[] args);

        long CountCompleted(string clientName, DateTime? clientStartTime);

        long CountFailed(string clientName, DateTime? clientStartTime);

        Task<bool> UpdateProteinDataAsync(ProteinUpdateType type, long updateArg);
    }

    public sealed partial class UnitInfoDatabase : IUnitInfoDatabase
    {
        #region Fields

        public bool ForceDateTimesToUtc { get; set; }

        private string ConnectionString
        {
            get { return @"Data Source=" + DatabaseFilePath + (ForceDateTimesToUtc ? ";DateTimeKind=Utc" : String.Empty); }
        }

        //private const string DbProvider = "System.Data.SQLite";

        private string _databaseFilePath;
        /// <summary>
        /// Get or Set the Database File Path
        /// </summary>
        public string DatabaseFilePath
        {
            get { return _databaseFilePath; }
            set
            {
                _databaseFilePath = value;
                CheckConnection();
            }
        }

        /// <summary>
        /// Flag that notes if the Database is safe to call
        /// </summary>
        public bool Connected { get; private set; }

        private readonly ILogger _logger = NullLogger.Instance;

        private readonly IProteinService _proteinService;
        private static readonly Dictionary<SqlTable, SqlTableCommands> SqlTableCommandDictionary = new Dictionary<SqlTable, SqlTableCommands>
                                                                                                 {
                                                                                                    { SqlTable.WuHistory, new WuHistorySqlTableCommands() },
                                                                                                    { SqlTable.Version, new VersionSqlTableCommands() }
                                                                                                 };

        #endregion

        public const string DefaultFileName = "WuHistory.db3";
        
        #region Constructor

        public UnitInfoDatabase(IPreferenceSet prefs, IProteinService proteinService)
           : this(prefs, proteinService, null)
        {

        }

        public UnitInfoDatabase(IPreferenceSet prefs, IProteinService proteinService, ILogger logger)
        {
            _proteinService = proteinService ?? throw new ArgumentNullException(nameof(proteinService));

            if (logger != null)
            {
                _logger = logger;
            }

            SQLiteFunction.RegisterFunction(typeof(ToSlotType));
            SQLiteFunction.RegisterFunction(typeof(GetProduction));

            ForceDateTimesToUtc = true;
            var path = prefs?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                DatabaseFilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check the Database Connection
        /// </summary>
        private void CheckConnection()
        {
            try
            {
                bool exists = TableExists(SqlTable.WuHistory);
                if (!exists)
                {
                    CreateTable(SqlTable.WuHistory);
                    CreateTable(SqlTable.Version);
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
                _logger.ErrorFormat(ex, "{0}", ex.Message);

                Connected = false;
            }
        }

        #region Upgrade

        public void Upgrade()
        {
            string dbVersionString = "0.0.0.0";
            if (TableExists(SqlTable.Version))
            {
                string versionFromTable = GetDatabaseVersion();
                if (!String.IsNullOrEmpty(versionFromTable))
                {
                    dbVersionString = versionFromTable;
                }
            }
            else
            {
                CreateTable(SqlTable.Version);
            }
            int dbVersion = Application.ParseVersion(dbVersionString);
            _logger.InfoFormat("WU History database v{0}", dbVersionString);

            UpgradeToVersion092(dbVersion);
        }

        private void UpgradeToVersion092(int dbVersion)
        {
            const string upgradeVersionString = "0.9.2";
            if (dbVersion < Application.ParseVersion(upgradeVersionString))
            {
                using (var connection = new SQLiteConnection(ConnectionString))
                {
                    connection.Open();
                    using (var transaction = connection.BeginTransaction())
                    {
                        try
                        {
                            _logger.InfoFormat("Performing WU History database upgrade to v{0}...", upgradeVersionString);
                            // delete duplicates
                            var duplicateDeleter = new DuplicateDeleter(connection, _logger);
                            duplicateDeleter.ExecuteAsyncWithProgress(true).Wait();
                            // add columns to WuHistory table
                            AddProteinColumns(connection);
                            // update the WuHistory table with protein info
                            var proteinDataUpdater = new ProteinDataUpdater(connection, _proteinService);
                            proteinDataUpdater.ExecuteAsyncWithProgress(true).Wait();
                            // set database version
                            SetDatabaseVersion(connection, upgradeVersionString);

                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            throw new DataException("Database upgrade failed.", ex);
                        }
                    }
                }
            }
        }

        private static void AddProteinColumns(SQLiteConnection connection)
        {
            using (var adder = new SQLiteColumnAdder(SqlTableCommandDictionary[SqlTable.WuHistory].TableName, connection))
            {
                adder.AddColumn("WorkUnitName", "VARCHAR(30)");
                adder.AddColumn("KFactor", "FLOAT");
                adder.AddColumn("Core", "VARCHAR(20)");
                adder.AddColumn("Frames", "INT");
                adder.AddColumn("Atoms", "INT");
                adder.AddColumn("Credit", "FLOAT");
                adder.AddColumn("PreferredDays", "FLOAT");
                adder.AddColumn("MaximumDays", "FLOAT");
                adder.Execute(false);
            }
        }

        private void SetDatabaseVersion(string version)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                SetDatabaseVersion(connection, version);
            }
        }

        private void SetDatabaseVersion(SQLiteConnection connection, string version)
        {
            Debug.Assert(!String.IsNullOrWhiteSpace(version));

            _logger.InfoFormat("Setting database version to: v{0}", version);
            using (var cmd = new SQLiteCommand("INSERT INTO [DbVersion] (Version) VALUES (?);", connection))
            {
                var param = new SQLiteParameter("Version", DbType.String) { Value = version };
                cmd.Parameters.Add(param);
                cmd.ExecuteNonQuery();
            }
        }

        #endregion

        #region Insert

        public bool Insert(UnitInfoModel unitInfoModel)
        {
            // if the work unit is not valid simply return
            if (!ValidateUnitInfo(unitInfoModel.UnitInfoData))
            {
                return false;
            }

            // The Insert operation does not setup a WuHistory table if
            // it does not exist.  This was already handled when the
            // the DatabaseFilePath was set.
            Debug.Assert(TableExists(SqlTable.WuHistory));

            // ensure this unit is not written twice
            if (UnitInfoExists(unitInfoModel.UnitInfoData))
            {
                return false;
            }

            var entry = AutoMapper.Mapper.Map<HistoryEntry>(unitInfoModel.UnitInfoData);
            // cannot map these two properties from a UnitInfo instance
            // they only live at the UnitInfoLogic level
            entry.FramesCompleted = unitInfoModel.FramesComplete;
            entry.FrameTimeValue = unitInfoModel.GetRawTime(PpdCalculationType.AllFrames);
            // copy protein values for insert
            entry.WorkUnitName = unitInfoModel.CurrentProtein.WorkUnitName;
            entry.KFactor = unitInfoModel.CurrentProtein.KFactor;
            entry.Core = unitInfoModel.CurrentProtein.Core;
            entry.Frames = unitInfoModel.CurrentProtein.Frames;
            entry.Atoms = unitInfoModel.CurrentProtein.NumberOfAtoms;
            entry.BaseCredit = unitInfoModel.CurrentProtein.Credit;
            entry.PreferredDays = unitInfoModel.CurrentProtein.PreferredDays;
            entry.MaximumDays = unitInfoModel.CurrentProtein.MaximumDays;
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    database.Insert(entry);
                }
            }

            return true;
        }

        private static bool ValidateUnitInfo(UnitInfo unitInfo)
        {
            // if Project and Download Time are valid
            if (!unitInfo.ProjectIsUnknown() && unitInfo.DownloadTime != DateTime.MinValue)
            {
                // if UnitResult is FinishedUnit
                if (unitInfo.UnitResult == WorkUnitResult.FinishedUnit)
                {
                    // the Finished Time must be valid
                    return unitInfo.FinishedTime != DateTime.MinValue;
                }
                // otherwise, the UnitResult must be a Terminating error result
                return unitInfo.UnitResult.IsTerminating();
            }
            return false;
        }

        private bool UnitInfoExists(UnitInfo unitInfo)
        {
            var rows = Fetch(BuildUnitKeyQueryParameters(unitInfo));
            return rows.Count != 0;
        }

        private static QueryParameters BuildUnitKeyQueryParameters(UnitInfo unitInfo)
        {
            var parameters = new QueryParameters { Name = String.Format(CultureInfo.InvariantCulture, "Query for existing {0}", unitInfo.ToProjectString()) };
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectID, Type = QueryFieldType.Equal, Value = unitInfo.ProjectID });
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectRun, Type = QueryFieldType.Equal, Value = unitInfo.ProjectRun });
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectClone, Type = QueryFieldType.Equal, Value = unitInfo.ProjectClone });
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectGen, Type = QueryFieldType.Equal, Value = unitInfo.ProjectGen });
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.Equal, Value = unitInfo.DownloadTime });
            return parameters;
        }

        #endregion

        #region Delete

        public int Delete(HistoryEntry entry)
        {
            Debug.Assert(TableExists(SqlTable.WuHistory));
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    return database.Delete(entry);
                }
            }
        }

        #endregion

        #region Fetch

        public IList<HistoryEntry> Fetch(QueryParameters parameters)
        {
            return Fetch(parameters, BonusCalculationType.DownloadTime);
        }

        public IList<HistoryEntry> Fetch(QueryParameters parameters, BonusCalculationType bonusCalculation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return FetchInternal(parameters, bonusCalculation);
            }
            finally
            {
                _logger.DebugFormat("Database Fetch ({0}) completed in {1}", parameters, sw.GetExecTime());
            }
        }

        private IList<HistoryEntry> FetchInternal(QueryParameters parameters, BonusCalculationType bonusCalculation)
        {
            Debug.Assert(TableExists(SqlTable.WuHistory));

            var select = new PetaPoco.Sql(SqlTableCommandDictionary[SqlTable.WuHistory].SelectSql);
            select.Append(WhereBuilder.Execute(parameters));
            GetProduction.BonusCalculation = bonusCalculation;
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    List<HistoryEntry> query = database.Fetch<HistoryEntry>(select);
                    return query;
                }
            }
        }

        public PetaPoco.Page<HistoryEntry> Page(long page, long itemsPerPage, QueryParameters parameters)
        {
            return Page(page, itemsPerPage, parameters, BonusCalculationType.DownloadTime);
        }

        public PetaPoco.Page<HistoryEntry> Page(long page, long itemsPerPage, QueryParameters parameters, BonusCalculationType bonusCalculation)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                return PageInternal(page, itemsPerPage, parameters, bonusCalculation);
            }
            finally
            {
                _logger.DebugFormat("Database Page Fetch ({0}) completed in {1}", parameters, sw.GetExecTime());
            }
        }

        private PetaPoco.Page<HistoryEntry> PageInternal(long page, long itemsPerPage, QueryParameters parameters, BonusCalculationType bonusCalculation)
        {
            Debug.Assert(TableExists(SqlTable.WuHistory));

            var select = new PetaPoco.Sql(SqlTableCommandDictionary[SqlTable.WuHistory].SelectSql);
            select.Append(WhereBuilder.Execute(parameters));
            GetProduction.BonusCalculation = bonusCalculation;
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var database = new PetaPoco.Database(connection))
                {
                    PetaPoco.Page<HistoryEntry> query = database.Page<HistoryEntry>(page, itemsPerPage, select);
                    Debug.Assert(query != null);
                    return query;
                }
            }
        }

        #endregion

        #region Select

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
        public DataTable Select(string sql, params object[] args)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return Select(connection, sql, args);
            }
        }

        private static DataTable Select(SQLiteConnection connection, string sql, params object[] args)
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

        #region Execute

        public int Execute(string sql, params object[] args)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                return Execute(connection, sql, args);
            }
        }

        private static int Execute(SQLiteConnection connection, string sql, params object[] args)
        {
            using (var database = new PetaPoco.Database(connection))
            {
                return database.Execute(sql, args);
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

        private long Count(string clientName, bool completed, DateTime? clientStartTime)
        {
            var parameters = new QueryParameters();
            parameters.Fields.Add(new QueryField { Name = QueryFieldName.Name, Type = QueryFieldType.Equal, Value = clientName });
            parameters.Fields.Add(new QueryField
            {
                Name = QueryFieldName.Result,
                Type = completed ? QueryFieldType.Equal : QueryFieldType.NotEqual,
                Value = (int)WorkUnitResult.FinishedUnit
            });
            if (clientStartTime.HasValue)
            {
                parameters.Fields.Add(new QueryField
                {
                    Name = completed ? QueryFieldName.CompletionDateTime : QueryFieldName.DownloadDateTime,
                    Type = QueryFieldType.GreaterThan,
                    Value = clientStartTime.Value
                });
            }
            PetaPoco.Sql where = WhereBuilder.Execute(parameters);

            var countSql = PetaPoco.Sql.Builder.Select("ID", "InstanceName", "DownloadDateTime", "CompletionDateTime", "Result", "COUNT(*)")
               .From("WuHistory")
               .Append(where);

            using (var table = Select(countSql.SQL, countSql.Arguments))
            {
                return (long)table.Rows[0][5];
            }
        }

        #endregion

        public async Task<bool> UpdateProteinDataAsync(ProteinUpdateType type, long updateArg)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // update the WuHistory table with protein info
                        var proteinDataUpdater = new ProteinDataUpdaterWithCancellation(connection, _proteinService);
                        proteinDataUpdater.UpdateType = type;
                        proteinDataUpdater.UpdateArg = updateArg;
                        await proteinDataUpdater.ExecuteAsyncWithProgress(true, this).ConfigureAwait(false);
                        if (proteinDataUpdater.IsCanceled)
                        {
                            transaction.Rollback();
                            return false;
                        }
                        transaction.Commit();
                        return true;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }

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

        internal bool TableExists(SqlTable sqlTable)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (DataTable table = connection.GetSchema("Tables", new[] { null, null, SqlTableCommandDictionary[sqlTable].TableName, null }))
                {
                    return table.Rows.Count != 0;
                }
            }
        }

        internal void CreateTable(SqlTable sqlTable)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = SqlTableCommandDictionary[sqlTable].GetCreateTableCommand(connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        internal void DropTable(SqlTable sqlTable)
        {
            using (var connection = new SQLiteConnection(ConnectionString))
            {
                connection.Open();
                using (var command = SqlTableCommandDictionary[sqlTable].GetDropTableCommand(connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }

        internal string GetDatabaseVersion()
        {
            if (!TableExists(SqlTable.Version))
            {
                return null;
            }

            using (var table = Select("SELECT * FROM [DbVersion] ORDER BY ID DESC LIMIT 1;"))
            {
                if (table.Rows.Count != 0)
                {
                    return table.Rows[0]["Version"].ToString();
                }
            }

            return null;
        }

        #endregion

        #endregion

        #region Private Helper Classes

        private static class WhereBuilder
        {
            public static PetaPoco.Sql Execute(QueryParameters parameters)
            {
                if (parameters.Fields.Count == 0)
                {
                    return null;
                }

                // reset
                bool appendAnd = false;

                PetaPoco.Sql sql = PetaPoco.Sql.Builder.Append("WHERE ");
                foreach (var field in parameters.Fields)
                {
                    sql = sql.Append(appendAnd ? "AND " : String.Empty);
                    sql = BuildWhereCondition(sql, field);
                    appendAnd = true;
                }

                return appendAnd ? sql.Append(" ORDER BY [ID] ASC") : null;
            }

            private static PetaPoco.Sql BuildWhereCondition(PetaPoco.Sql sql, QueryField queryField)
            {
                string format = "[{0}] {1} @0";
                if (queryField.Name.Equals(QueryFieldName.DownloadDateTime) ||
                    queryField.Name.Equals(QueryFieldName.CompletionDateTime))
                {
                    format = "datetime([{0}]) {1} datetime(@0)";
                }
                sql = sql.Append(String.Format(CultureInfo.InvariantCulture, format,
                         ColumnNameOverides.ContainsKey(queryField.Name) ? ColumnNameOverides[queryField.Name] : queryField.Name.ToString(),
                         queryField.Operator), queryField.Value);
                return sql;
            }

            private static readonly Dictionary<QueryFieldName, string> ColumnNameOverides = new Dictionary<QueryFieldName, string>
         {
            { QueryFieldName.Name, "InstanceName" },
            { QueryFieldName.Path, "InstancePath" },
            { QueryFieldName.Credit, "CalcCredit" },
         };
        }

        private abstract class SqlTableCommands
        {
            #region SQL Strings

            private const string DropTableSql = "DROP TABLE [{0}];";

            #endregion

            public abstract string TableName { get; }

            public virtual string SelectSql
            {
                get { return String.Empty; }
            }

            public abstract DbCommand GetCreateTableCommand(SQLiteConnection connection);

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            public DbCommand GetDropTableCommand(SQLiteConnection connection)
            {
                return new SQLiteCommand(connection)
                {
                    CommandText = String.Format(CultureInfo.InvariantCulture, DropTableSql, TableName)
                };
            }
        }

        private sealed class WuHistorySqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string WuHistoryTableName = "WuHistory";

            private static readonly string WuHistoryTableCreateSql = "CREATE TABLE [{0}] (" +
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
                return String.Format(CultureInfo.InvariantCulture, columnDef, SQLiteColumnAdder.GetDefaultValue(columnDef));
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

            public override string TableName
            {
                get { return WuHistoryTableName; }
            }

            public override string SelectSql
            {
                get { return WuHistoryTableSelect; }
            }

            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope")]
            public override DbCommand GetCreateTableCommand(SQLiteConnection connection)
            {
                return new SQLiteCommand(connection)
                {
                    CommandText = String.Format(CultureInfo.InvariantCulture,
                                               WuHistoryTableCreateSql, WuHistoryTableName)
                };
            }
        }

        private sealed class VersionSqlTableCommands : SqlTableCommands
        {
            #region SQL Constants

            private const string VersionTableName = "DbVersion";

            private const string VersionTableCreateSql = "CREATE TABLE [{0}] (" +
                                                         "[ID] INTEGER  NOT NULL PRIMARY KEY AUTOINCREMENT," +
                                                         "[Version] VARCHAR(20)  NOT NULL);";

            #endregion

            public override string TableName
            {
                get { return VersionTableName; }
            }

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

        // ReSharper disable InconsistentNaming
        private sealed class SQLiteColumnAdder : IDisposable
        // ReSharper restore InconsistentNaming
        {
            private readonly string _tableName;
            private readonly SQLiteConnection _connection;

            public SQLiteColumnAdder(string tableName, SQLiteConnection connection)
            {
                _tableName = tableName;
                _connection = connection;

                Debug.Assert(_connection.State == ConnectionState.Open);
            }

            private readonly List<DbCommand> _commands = new List<DbCommand>();
            private EnumerableRowCollection<DataRow> _rows;

            public void AddColumn(string name, string dataType)
            {
                if (name == null) throw new ArgumentNullException("name");
                if (dataType == null) throw new ArgumentNullException("dataType");

                if (_rows == null)
                {
                    using (var adapter = new SQLiteDataAdapter("PRAGMA table_info(WuHistory);", _connection))
                    using (var table = new DataTable())
                    {
                        adapter.Fill(table);
                        _rows = table.AsEnumerable();
                    }
                }

                bool columnExists = _rows.Any(row => row.Field<string>(1) == name);
                if (!columnExists)
                {
                    string commandText = String.Format(CultureInfo.InvariantCulture,
                       "ALTER TABLE [{0}] ADD COLUMN [{1}] {2} DEFAULT {3} NOT NULL", _tableName, name, dataType, GetDefaultValue(dataType));
                    _commands.Add(new SQLiteCommand(_connection) { CommandText = commandText });
                }
            }

            public static object GetDefaultValue(string dataType)
            {
                if (dataType.Contains("VARCHAR"))
                {
                    return "''";
                }
                if (dataType.Contains("INT"))
                {
                    return 0;
                }
                if (dataType.Contains("FLOAT"))
                {
                    return 0.0f;
                }

                string message = String.Format(CultureInfo.CurrentCulture, "Data type {0} is not valid.", dataType);
                throw new ArgumentException(message, "dataType");
            }

            //public void Execute()
            //{
            //   Execute(true);
            //}

            public void Execute(bool useTransaction)
            {
                if (useTransaction)
                {
                    using (var transaction = _connection.BeginTransaction())
                    {
                        ExecuteInternal();
                        transaction.Commit();
                    }
                }
                else
                {
                    ExecuteInternal();
                }
            }

            private void ExecuteInternal()
            {
                foreach (var command in _commands)
                {
                    command.ExecuteNonQuery();
                }
            }

            public void Dispose()
            {
                foreach (var command in _commands)
                {
                    command.Dispose();
                }
            }
        }

        #endregion
    }
}
