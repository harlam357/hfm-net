/*
 * HFM.NET - Work Unit History Database
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using System.Linq.Dynamic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public enum SqlTable
   {
      WuHistory,
      Version
   }

   public interface IUnitInfoDatabase
   {
      /// <summary>
      /// Flag that notes if the Database is safe to call.
      /// </summary>
      bool Connected { get; }

      void Insert(UnitInfoLogic unitInfoLogic);

      int Delete(HistoryEntry entry);

      IList<HistoryEntry> Fetch(QueryParameters parameters);

      IList<HistoryEntry> Fetch(QueryParameters parameters, HistoryProductionView productionView);
   }

   //public class MyMapper : PetaPoco.IMapper
   //{
   //   public void GetTableInfo(Type t, PetaPoco.TableInfo ti)
   //   {
   //      // no implementation
   //   }
   //
   //   public bool MapPropertyToColumn(System.Reflection.PropertyInfo pi, ref string columnName, ref bool resultColumn)
   //   {
   //      // no implementation
   //      return true;
   //   }
   //
   //   public Func<object, object> GetFromDbConverter(System.Reflection.PropertyInfo pi, Type sourceType)
   //   {
   //      // no implementation
   //      return null;
   //   }
   //
   //   public Func<object, object> GetToDbConverter(Type sourceType)
   //   {
   //      if (sourceType == typeof(DateTime))
   //      {
   //         return value =>
   //                {
   //                   var dateTime = (DateTime)value;
   //                   //return String.Format(CultureInfo.InvariantCulture, "'{0}'", 
   //                   return dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture); // + "Z";
   //                };
   //      }
   //      return null;
   //   }
   //}

   public sealed class UnitInfoDatabase : IUnitInfoDatabase
   {
      #region Fields

      public bool ForceDateTimesToUtc { get; set; }

      private string ConnectionString
      {
         get { return @"Data Source=" + DatabaseFilePath + (ForceDateTimesToUtc ? ";DateTimeKind=Utc" : String.Empty); }
      }

      private const string DbProvider = "System.Data.SQLite";

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

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IProteinDictionary _proteinDictionary;
      private static readonly Dictionary<SqlTable, SqlTableCommands> SqlTableCommands = new Dictionary<SqlTable, SqlTableCommands>
                                                                                        {
                                                                                           { SqlTable.WuHistory, new WuHistorySqlTableCommands() },
                                                                                           { SqlTable.Version, new VersionSqlTableCommands() }
                                                                                        };  

      #endregion

      #region Constructor

      public UnitInfoDatabase(IPreferenceSet prefs, IProteinDictionary proteinDictionary)
      {
         if (proteinDictionary == null) throw new ArgumentNullException("proteinDictionary");

         _proteinDictionary = proteinDictionary;

         ForceDateTimesToUtc = true;
         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            DatabaseFilePath = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.SqLiteFilename);
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
            if (!TableExists(SqlTable.WuHistory))
            {
               //PerformUpgrade();
               CreateTable(SqlTable.WuHistory);
            }
            var parameters = new QueryParameters();
            parameters.Fields.Add(new QueryField
                                    {
                                       Name = QueryFieldName.ID,
                                       Type = QueryFieldType.Equal,
                                       Value = 0
                                    });
            Fetch(parameters);
            Connected = true;
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
            Connected = false;
         }
      }

      //private void PerformUpgrade()
      //{
      //   
      //}

      #region Insert
   
      public void Insert(UnitInfoLogic unitInfoLogic)
      {
         // validate the UnitInfoLogic before opening the connection
         if (ValidateUnitInfo(unitInfoLogic.UnitInfoData) == false) return;

         Debug.Assert(TableExists(SqlTable.WuHistory));

         // ensure this unit is not written twice
         if (!UnitInfoExists(unitInfoLogic))
         {
            var entry = AutoMapper.Mapper.Map<HistoryEntry>(unitInfoLogic.UnitInfoData);
            entry.FramesCompleted = unitInfoLogic.FramesComplete;
            entry.FrameTimeValue = unitInfoLogic.GetRawTime(PpdCalculationType.AllFrames);
            using (var database = new PetaPoco.Database(ConnectionString, DbProvider))
            {
               database.Insert(entry);
            }
         }
      }

      private static bool ValidateUnitInfo(UnitInfo unitInfo)
      {
         bool result = ValidateFinishedUnitInfo(unitInfo);
         if (!result)
         {
            // Issue 233
            result = ValidateIncompleteUnitInfo(unitInfo);
         }
         return result;
      }

      private static bool ValidateFinishedUnitInfo(UnitInfo unitInfo)
      {
         return unitInfo.ProjectIsUnknown() == false &&
                unitInfo.UnitResult.Equals(WorkUnitResult.FinishedUnit) &&
                unitInfo.DownloadTime.Equals(DateTime.MinValue) == false &&
                unitInfo.FinishedTime.Equals(DateTime.MinValue) == false;
      }

      private static bool ValidateIncompleteUnitInfo(UnitInfo unitInfo)
      {
         // Finished Time will not be populated if any of these error
         // results are detected.  Only check for valid Project and 
         // download time - Issue 233

         return unitInfo.ProjectIsUnknown() == false &&
               (unitInfo.UnitResult.Equals(WorkUnitResult.BadWorkUnit) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.CoreOutdated) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.EarlyUnitEnd) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.Interrupted) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.UnstableMachine)) &&
                unitInfo.DownloadTime.Equals(DateTime.MinValue) == false;
      }

      private bool UnitInfoExists(UnitInfoLogic unitInfoLogic)
      {
         var rows = Fetch(BuildUnitKeyQueryParameters(unitInfoLogic));
         return rows.Count != 0;
      }

      private static QueryParameters BuildUnitKeyQueryParameters(UnitInfoLogic unitInfoLogic)
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectID, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectID });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectRun, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectRun });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectClone, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectClone });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectGen, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectGen });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.DownloadTime });
         return parameters;
      }

      #endregion

      #region Delete
      
      public int Delete(HistoryEntry entry)
      {
         Debug.Assert(TableExists(SqlTable.WuHistory));

         using (var database = new PetaPoco.Database(ConnectionString, DbProvider))
         {
            return database.Delete(entry);
         }
      }

      #endregion

      #region Fetch
      
      public IList<HistoryEntry> Fetch(QueryParameters parameters)
      {
         return Fetch(parameters, HistoryProductionView.BonusDownloadTime);
      }
      
      public IList<HistoryEntry> Fetch(QueryParameters parameters, HistoryProductionView productionView)
      {
         DateTime start = Instrumentation.ExecStart;
         try
         {
            return FetchInternal(parameters, productionView);
         }
         finally
         {
            _logger.Debug("Database Fetch ({0}) completed in {1}", parameters, Instrumentation.GetExecTime(start));
         }
      }

      public IList<HistoryEntry> FetchInternal(QueryParameters parameters, HistoryProductionView productionView)
      {
         Debug.Assert(TableExists(SqlTable.WuHistory));
          
         List<HistoryEntry> query;
         using (var database = new PetaPoco.Database(ConnectionString, DbProvider))
         {
            PetaPoco.Sql where = WhereBuilder.Execute(parameters);
            query = where != null ? database.Fetch<HistoryEntry>(where) : database.Fetch<HistoryEntry>(String.Empty);
         }
         Debug.Assert(query != null);
         query.ForEach(x => x.ProductionView = productionView);

         if (_proteinDictionary == null) return query;
            
         var joinQuery = from entry in query
                           join protein in _proteinDictionary.Values on entry.ProjectID equals protein.ProjectNumber into groupJoin
                           from entryProtein in groupJoin.DefaultIfEmpty()
                           select entry.SetProtein(entryProtein);

         return FilterProteinParameters(parameters, joinQuery);
      }

      private IList<HistoryEntry> FilterProteinParameters(QueryParameters parameters, IEnumerable<HistoryEntry> entries)
      {
         var query = entries.AsQueryable();

         foreach (var field in parameters.Fields)
         {
            if (field.Name.Equals(QueryFieldName.WorkUnitName) ||
                field.Name.Equals(QueryFieldName.KFactor) ||
                field.Name.Equals(QueryFieldName.Core) ||
                field.Name.Equals(QueryFieldName.Frames) ||
                field.Name.Equals(QueryFieldName.Atoms) ||
                field.Name.Equals(QueryFieldName.SlotType) ||
                field.Name.Equals(QueryFieldName.PPD) ||
                field.Name.Equals(QueryFieldName.Credit))
            {
               if (field.Type.Equals(QueryFieldType.Like))
               {
                  query = Like(query, field);
               }
               else
               {
                  try
                  {
                     query = query.Where(BuildWhereCondition(field));
                  }
                  catch (ParseException ex)
                  {
                     Logger.WarnFormat(ex, "{0}", ex.Message);
                  }
               }
            }
         }

         return query.ToList();
      }

      private static IQueryable<HistoryEntry> Like(IQueryable<HistoryEntry> query, QueryField field)
      {
         Debug.Assert(field.Type.Equals(QueryFieldType.Like));

         if (field.Name.Equals(QueryFieldName.WorkUnitName))
         {
            return query.Where(x => IsSqlLikeMatch(x.WorkUnitName, field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.KFactor))
         {
            return query.Where(x => IsSqlLikeMatch(x.KFactor.ToString(), field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.Core))
         {
            return query.Where(x => IsSqlLikeMatch(x.Core, field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.Frames))
         {
            return query.Where(x => IsSqlLikeMatch(x.Frames.ToString(), field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.Atoms))
         {
            return query.Where(x => IsSqlLikeMatch(x.Atoms.ToString(), field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.SlotType))
         {
            return query.Where(x => IsSqlLikeMatch(x.SlotType, field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.PPD))
         {
            return query.Where(x => IsSqlLikeMatch(x.PPD.ToString(), field.Value.ToString()));
         }
         if (field.Name.Equals(QueryFieldName.Credit))
         {
            return query.Where(x => IsSqlLikeMatch(x.Credit.ToString(), field.Value.ToString()));
         }

         // ReSharper disable HeuristicUnreachableCode
         Debug.Assert(false);
         return query;
         // ReSharper restore HeuristicUnreachableCode
      }

      private static bool IsSqlLikeMatch(string input, string pattern)
      {
         // Method from here: http://bytes.com/topic/c-sharp/answers/253519-using-regex-create-sqls-like-like-function

         /* Turn "off" all regular expression related syntax in
         * the pattern string. */
         pattern = Regex.Escape(pattern);

         /* Replace the SQL LIKE wildcard metacharacters with the
         * equivalent regular expression metacharacters. */
         pattern = pattern.Replace("%", ".*?").Replace("_", ".");

         /* The previous call to Regex.Escape actually turned off
         * too many metacharacters, i.e. those which are recognized by
         * both the regular expression engine and the SQL LIKE
         * statement ([...] and [^...]). Those metacharacters have
         * to be manually unescaped here. */
         pattern = pattern.Replace(@"\[", "[").Replace(@"\]", "]").Replace(@"\^", "^");

         // anchor the pattern - rwh 12/1/12
         if (!pattern.StartsWith("^"))
         {
            pattern = "^" + pattern;
         }
         if (!pattern.EndsWith("$"))
         {
            pattern = pattern + "$";
         }
         
         return input != null && Regex.IsMatch(input, pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
      }

      private static string BuildWhereCondition(QueryField queryField)
      {
         string valueFormat = "{2}";
         if (queryField.Name.Equals(QueryFieldName.WorkUnitName) ||
             queryField.Name.Equals(QueryFieldName.Core) ||
             queryField.Name.Equals(QueryFieldName.SlotType))
         {
            valueFormat = "\"{2}\"";
         }

         return String.Format(CultureInfo.InvariantCulture, "{0} {1} " + valueFormat, queryField.Name, queryField.Operator, queryField.Value);
      }

      #endregion

      #region Table Helpers

      internal bool TableExists(SqlTable sqlTable)
      {
         using (var con = new SQLiteConnection(ConnectionString))
         {
            con.Open();
            DataTable table = con.GetSchema("Tables", new[] { null, null, SqlTableCommands[sqlTable].TableName, null });
            return table.Rows.Count != 0;
         }
      }

      internal void CreateTable(SqlTable sqlTable)
      {
         using (var con = new SQLiteConnection(ConnectionString))
         {
            con.Open();
            using (var command = SqlTableCommands[sqlTable].GetCreateTableCommand(con))
            {
               command.ExecuteNonQuery();
            }
         }
      }

      internal void DropTable(SqlTable sqlTable)
      {
         using (var con = new SQLiteConnection(ConnectionString))
         {
            con.Open();
            using (var command = SqlTableCommands[sqlTable].GetDropTableCommand(con))
            {
               command.ExecuteNonQuery();
            }
         }
      }

      #endregion
      
      #endregion

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
               if (field.Name.Equals(QueryFieldName.ID) ||
                   field.Name.Equals(QueryFieldName.ProjectID) ||
                   field.Name.Equals(QueryFieldName.ProjectRun) ||
                   field.Name.Equals(QueryFieldName.ProjectClone) ||
                   field.Name.Equals(QueryFieldName.ProjectGen) ||
                   field.Name.Equals(QueryFieldName.Name) ||
                   field.Name.Equals(QueryFieldName.Path) ||
                   field.Name.Equals(QueryFieldName.Username) ||
                   field.Name.Equals(QueryFieldName.Team) ||
                   field.Name.Equals(QueryFieldName.CoreVersion) ||
                   field.Name.Equals(QueryFieldName.FramesCompleted) ||
                   field.Name.Equals(QueryFieldName.FrameTime) ||
                   field.Name.Equals(QueryFieldName.Result) ||
                   field.Name.Equals(QueryFieldName.DownloadDateTime) ||
                   field.Name.Equals(QueryFieldName.CompletionDateTime))
               {
                  sql = sql.Append(appendAnd ? "AND " : String.Empty);
                  sql = BuildWhereCondition(sql, field);
                  appendAnd = true;
               }
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
         };
      }
   }

   public abstract class SqlTableCommands
   {
      #region SQL Strings

      private const string DropTableSql = "DROP TABLE [{0}];";
      private const string SelectSql = "SELECT * FROM [{0}];";

      #endregion

      public abstract string TableName { get; }

      public abstract DbCommand GetCreateTableCommand(SQLiteConnection connection);

      public abstract DbCommand GetInsertCommand(SQLiteConnection connection);

      public virtual DbCommand GetDropTableCommand(SQLiteConnection connection)
      {
         return new SQLiteCommand(connection)
                {
                   CommandText = String.Format(CultureInfo.InvariantCulture, DropTableSql, TableName)
                };
      }
      
      public virtual SQLiteCommand GetSelectCommand(SQLiteConnection connection)
      {
         return new SQLiteCommand(connection)
                {
                   CommandText = String.Format(CultureInfo.InvariantCulture, SelectSql, TableName)
                };
      }
   }

   public sealed class WuHistorySqlTableCommands : SqlTableCommands
   {
      #region SQL Constants
      
      private const string WuHistoryTableName = "WuHistory";

      private const string WuHistoryTableCreateSql = "CREATE TABLE [{0}] (" +
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
                                                     "[CompletionDateTime] DATETIME  NOT NULL);";

      private const string WuHistoryTableInsertSql = "INSERT INTO [{0}] ([ProjectID]," +
                                                                        "[ProjectRun]," +
                                                                        "[ProjectClone]," +
                                                                        "[ProjectGen]," +
                                                                        "[InstanceName]," +
                                                                        "[InstancePath]," +
                                                                        "[Username]," +
                                                                        "[Team]," +
                                                                        "[CoreVersion]," +
                                                                        "[FramesCompleted]," +
                                                                        "[FrameTime]," +
                                                                        "[Result]," +
                                                                        "[DownloadDateTime]," +
                                                                        "[CompletionDateTime]) VALUES(?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";

      #endregion

      public override string TableName
      {
         get { return WuHistoryTableName; }
      }

      public override DbCommand GetCreateTableCommand(SQLiteConnection connection)
      {
         return new SQLiteCommand(connection)
                {
                   CommandText = String.Format(CultureInfo.InvariantCulture, 
                                               WuHistoryTableCreateSql, WuHistoryTableName)
                };
      }

      public override DbCommand GetInsertCommand(SQLiteConnection connection)
      {
         return new SQLiteCommand(connection)
                {
                   CommandText = String.Format(CultureInfo.InvariantCulture,
                                               WuHistoryTableInsertSql, WuHistoryTableName)
                };
      }
   }

   public sealed class VersionSqlTableCommands : SqlTableCommands
   {
      #region SQL Constants

      private const string VersionTableName = "DbVersion";

      private const string VersionTableCreateSql = "CREATE TABLE [{0}] (" +
                                                   "[Version] FLOAT  NOT NULL);";
      
      #endregion

      public override string TableName
      {
         get { return VersionTableName; }
      }

      public override DbCommand GetCreateTableCommand(SQLiteConnection connection)
      {
         return new SQLiteCommand(connection)
                {
                   CommandText = String.Format(CultureInfo.InvariantCulture,
                                               VersionTableCreateSql, VersionTableName)
                };
      }

      public override DbCommand GetInsertCommand(SQLiteConnection connection)
      {
         //return new SQLiteCommand(connection)
         //       {
         //          CommandText = String.Format(CultureInfo.InvariantCulture,
         //                                      WuHistoryTableInsertSql, WuHistoryTableName)
         //       };
         return null;
      }
   }
}
