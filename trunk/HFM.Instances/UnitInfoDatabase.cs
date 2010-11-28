   
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Dynamic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

using ProtoBuf;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public interface IUnitInfoDatabase
   {
      string DatabaseFilePath { get; set; }

      bool TableExists(string tableName);

      void CreateTable(string tableName);

      void DeleteAllUnitInfoData();
      
      void WriteUnitInfo(IUnitInfoLogic unitInfoLogic);

      int DeleteUnitInfo(long id);
      
      void ImportCompletedUnits(ICollection<HistoryEntry> entries);
      
      IList<HistoryEntry> QueryUnitData(QueryParameters parameters);

      IList<HistoryEntry> QueryUnitData(QueryParameters parameters, HistoryProductionView productionView);
   }

   public class UnitInfoDatabase : IUnitInfoDatabase
   {
      public const string WuHistoryTableName = "WuHistory";

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

      private const string WuHistoryTableDropSql = "DROP TABLE [{0}];";                                                     

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
                                                                        "[CompletionDateTime]) VALUES({1});";

      private const string WuHistoryTableInsertValuesFormat =
         "{0}, {1}, {2}, {3}, '{4}', '{5}', '{6}', {7}, {8}, {9}, {10}, {11}, @DownloadDateTime, @CompletionDateTime";

      public string DatabaseFilePath { get; set; }
      private readonly IProteinCollection _proteinCollection;
   
      public UnitInfoDatabase(IProteinCollection proteinCollection)
      {
         _proteinCollection = proteinCollection;
      }
      
      public bool TableExists(string tableName)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            return TableExists(con, WuHistoryTableName);
         }
      }
      
      public void CreateTable(string tableName)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            CreateTable(con, WuHistoryTableName);
         }
      }
      
      public void DeleteAllUnitInfoData()
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            if (TableExists(con, WuHistoryTableName))
            {
               DropTable(con, WuHistoryTableName);
            }
         }
      }
   
      public void WriteUnitInfo(IUnitInfoLogic unitInfoLogic)
      {
         // validate the UnitInfoLogic before opening the connection
         if (ValidateUnitInfo(unitInfoLogic.UnitInfoData) == false) return;

         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            if (TableExists(con, WuHistoryTableName))
            {
               // do upgrade
            }
            else
            {
               CreateTable(con, WuHistoryTableName);
               Debug.Assert(TableExists(con, WuHistoryTableName));
            }

            // ensure this unit is not written twice
            if (UnitInfoExists(con, unitInfoLogic) == false)
            {
               WriteUnitInfoToDatabase(con, unitInfoLogic);
            }
         }
      }
      
      public int DeleteUnitInfo(long id)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            if (TableExists(con, WuHistoryTableName))
            {
               var parameters = new QueryParameters();
               parameters.Fields.Add(new QueryField { Name = QueryFieldName.ID, Type = QueryFieldType.Equal, Value = id });
               return DeleteRows(con, parameters);
            }

            return 0;
         }
      }
      
      public void ImportCompletedUnits(ICollection<HistoryEntry> entries)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            if (TableExists(con, WuHistoryTableName))
            {
               // do upgrade
            }
            else
            {
               CreateTable(con, WuHistoryTableName);
               Debug.Assert(TableExists(con, WuHistoryTableName));
            }

            using (var trans = con.BeginTransaction())
            {
               foreach (var historyEntry in entries)
               {
                  WriteUnitInfoToDatabase(con, historyEntry);
               }
               trans.Commit();
            }
         }
      }
      
      private static bool TableExists(DbConnection con, string tableName)
      {
         DataTable table = con.GetSchema("Tables", new[] { null, null, tableName, null });
         return table.Rows.Count != 0;
      }

      private static void CreateTable(SQLiteConnection con, string tableName)
      {
         using (var command = new SQLiteCommand(con))
         {
            command.CommandText = String.Format(CultureInfo.InvariantCulture,
                                                WuHistoryTableCreateSql, tableName);
            command.ExecuteNonQuery();
         }
      }
      
      private static void DropTable(SQLiteConnection con, string tableName)
      {
         using (var command = new SQLiteCommand(con))
         {
            command.CommandText = String.Format(CultureInfo.InvariantCulture,
                                                WuHistoryTableDropSql, tableName);
            command.ExecuteNonQuery();
         }
      }
      
      private static bool ValidateUnitInfo(IUnitInfo unitInfo)
      {
         bool result = ValidateFinishedUnitInfo(unitInfo);
         if (!(result))
         {
            // Issue 233
            result = ValidateIncompleteUnitInfo(unitInfo);
         }
         return result;
      }
      
      private static bool ValidateFinishedUnitInfo(IUnitInfo unitInfo)
      {
         return unitInfo.ProjectIsUnknown == false &&
                unitInfo.UnitResult.Equals(WorkUnitResult.FinishedUnit) &&
                unitInfo.DownloadTime.Equals(DateTime.MinValue) == false &&
                unitInfo.FinishedTime.Equals(DateTime.MinValue) == false;
      }
      
      private static bool ValidateIncompleteUnitInfo(IUnitInfo unitInfo)
      {
         // Finished Time will not be populated if any of these error
         // results are detected.  Only check for valid Project and 
         // download time - Issue 233
      
         return unitInfo.ProjectIsUnknown == false &&
               (unitInfo.UnitResult.Equals(WorkUnitResult.BadWorkUnit) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.CoreOutdated) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.EarlyUnitEnd) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.Interrupted) ||
                unitInfo.UnitResult.Equals(WorkUnitResult.UnstableMachine)) &&
                unitInfo.DownloadTime.Equals(DateTime.MinValue) == false;
      }

      private static bool UnitInfoExists(SQLiteConnection con, IUnitInfoLogic unitInfoLogic)
      {
         var rows = ExecuteQueryUnitData(con, BuildUnitKeyQueryParameters(unitInfoLogic));
         return rows.Count != 0;
      }

      //private static bool UnitInfoExists(SQLiteConnection con, HistoryEntry entry)
      //{
      //   var rows = ExecuteQueryUnitData(con, BuildUnitKeyQueryParameters(entry));
      //   return rows.Length != 0;
      //}
      
      private static QueryParameters BuildUnitKeyQueryParameters(IUnitInfoLogic unitInfoLogic)
      {
         var parameters = new QueryParameters();
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectID, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectID });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectRun, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectRun });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectClone, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectClone });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.ProjectGen, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.ProjectGen });
         parameters.Fields.Add(new QueryField { Name = QueryFieldName.DownloadDateTime, Type = QueryFieldType.Equal, Value = unitInfoLogic.UnitInfoData.DownloadTime });
         return parameters;
      }

      private static void WriteUnitInfoToDatabase(SQLiteConnection con, IUnitInfoLogic unitInfoLogic)
      {
         using (var command = new SQLiteCommand(con))
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, unitInfoLogic.UnitInfoData.OwningInstanceName,
               String.Format(CultureInfo.CurrentCulture, "Writing unit {0} to database.", unitInfoLogic.ProjectRunCloneGen));
         
            var downloadDateTime = new SQLiteParameter("DownloadDateTime", DbType.DateTime) { Value = unitInfoLogic.UnitInfoData.DownloadTime };
            command.Parameters.Add(downloadDateTime);
            var completionDateTime = new SQLiteParameter("CompletionDateTime", DbType.DateTime) { Value = unitInfoLogic.UnitInfoData.FinishedTime };
            command.Parameters.Add(completionDateTime);
            
            command.CommandText = String.Format(CultureInfo.InvariantCulture, WuHistoryTableInsertSql, WuHistoryTableName, GetValuesString(unitInfoLogic));
            command.ExecuteNonQuery();
         }
      }
      
      private static string GetValuesString(IUnitInfoLogic unitInfoLogic)
      {
         return String.Format(CultureInfo.InvariantCulture,
                              WuHistoryTableInsertValuesFormat, 
                              unitInfoLogic.UnitInfoData.ProjectID,
                              unitInfoLogic.UnitInfoData.ProjectRun,
                              unitInfoLogic.UnitInfoData.ProjectClone,
                              unitInfoLogic.UnitInfoData.ProjectGen,
                              unitInfoLogic.UnitInfoData.OwningInstanceName,
                              unitInfoLogic.UnitInfoData.OwningInstancePath,
                              unitInfoLogic.UnitInfoData.FoldingID,
                              unitInfoLogic.UnitInfoData.Team,
                              unitInfoLogic.UnitInfoData.CoreVersion,
                              unitInfoLogic.FramesComplete,
                              unitInfoLogic.RawTimePerAllSections,
                              (int)unitInfoLogic.UnitInfoData.UnitResult);
      }

      private static void WriteUnitInfoToDatabase(SQLiteConnection con, HistoryEntry entry)
      {
         using (var command = new SQLiteCommand(con))
         {
            var downloadDateTime = new SQLiteParameter("DownloadDateTime", DbType.DateTime) { Value = entry.DownloadDateTime };
            command.Parameters.Add(downloadDateTime);
            var completionDateTime = new SQLiteParameter("CompletionDateTime", DbType.DateTime) { Value = entry.CompletionDateTime };
            command.Parameters.Add(completionDateTime);

            command.CommandText = String.Format(CultureInfo.InvariantCulture, WuHistoryTableInsertSql, WuHistoryTableName, GetValuesString(entry));
            command.ExecuteNonQuery();
         }
      }

      private static string GetValuesString(HistoryEntry entry)
      {
         return String.Format(CultureInfo.InvariantCulture,
                              WuHistoryTableInsertValuesFormat,
                              entry.ProjectID,
                              entry.ProjectRun,
                              entry.ProjectClone,
                              entry.ProjectGen,
                              entry.InstanceName,
                              entry.InstancePath,
                              entry.Username,
                              entry.Team,
                              entry.CoreVersion,
                              entry.FramesCompleted,
                              entry.FrameTime.TotalSeconds,
                              (int)entry.Result);
      }

      public IList<HistoryEntry> QueryUnitData(QueryParameters parameters)
      {
         return QueryUnitData(parameters, HistoryProductionView.BonusDownloadTime);
      }
      
      public IList<HistoryEntry> QueryUnitData(QueryParameters parameters, HistoryProductionView productionView)
      {
         using (var con = new SQLiteConnection(@"Data Source=" + DatabaseFilePath))
         {
            con.Open();
            return ExecuteQueryUnitData(con, parameters, productionView, _proteinCollection);
         }
      }
      
      private static IList<HistoryEntry> ExecuteQueryUnitData(SQLiteConnection con, QueryParameters parameters)
      {
         return ExecuteQueryUnitData(con, parameters, HistoryProductionView.BonusDownloadTime, null);
      }

      private static IList<HistoryEntry> ExecuteQueryUnitData(SQLiteConnection con, QueryParameters parameters, 
                                                              HistoryProductionView productionView, IProteinCollection proteinCollection)
      {
         if (TableExists(con, WuHistoryTableName))
         {
            var table = GetDataTable(con, parameters);

            var query =
               from wu in table.AsEnumerable()
               select new HistoryEntry
               {
                  ID = wu.Field<long>("ID"),
                  ProjectID = wu.Field<int>("ProjectID"),
                  ProjectRun = wu.Field<int>("ProjectRun"),
                  ProjectClone = wu.Field<int>("ProjectClone"),
                  ProjectGen = wu.Field<int>("ProjectGen"),
                  InstanceName = wu.Field<string>("InstanceName"),
                  InstancePath = wu.Field<string>("InstancePath"),
                  Username = wu.Field<string>("Username"),
                  Team = wu.Field<int>("Team"),
                  CoreVersion = (float)wu.Field<double>("CoreVersion"),
                  FramesCompleted = wu.Field<int>("FramesCompleted"),
                  FrameTime = TimeSpan.FromSeconds(wu.Field<int>("FrameTime")),
                  Result = (WorkUnitResult)wu.Field<int>("Result"),
                  DownloadDateTime = DateTime.SpecifyKind(wu.Field<DateTime>("DownloadDateTime"), DateTimeKind.Utc),
                  CompletionDateTime = DateTime.SpecifyKind(wu.Field<DateTime>("CompletionDateTime"), DateTimeKind.Utc),
                  ProductionView = productionView
               };

            if (proteinCollection == null) return query.ToList();
            
            var proteins = proteinCollection.Proteins;
            var joinQuery = from entry in query
                            join protein in proteins on entry.ProjectID equals protein.ProjectNumber into groupJoin
                            from entryProtein in groupJoin.DefaultIfEmpty()
                            select entry.SetProtein(entryProtein);

            return FilterProteinParameters(parameters, joinQuery);
         }

         return new List<HistoryEntry>();
      }
      
      private static DataTable GetDataTable(SQLiteConnection con, QueryParameters parameters)
      {
         var selectBuilder = new SelectStatementBuilder();
         var command = new SQLiteCommand(selectBuilder.BuildSelectStatement(parameters), con);
         var adapter = new SQLiteDataAdapter(command);

         var table = new DataTable();
         adapter.Fill(table);
         return table;
      }

      private static int DeleteRows(SQLiteConnection con, QueryParameters parameters)
      {
         var selectBuilder = new SelectStatementBuilder();
         var command = new SQLiteCommand(selectBuilder.BuildSelectStatement(parameters), con);
         var adapter = new SQLiteDataAdapter(command);
         var table = new DataTable();
         adapter.Fill(table);

         if (table.Rows.Count != 0)
         {
            foreach (DataRow row in table.Rows)
            {
               row.Delete();
            }
            var builder = new SQLiteCommandBuilder(adapter);
            adapter.UpdateCommand = builder.GetUpdateCommand();
            return adapter.Update(table);
         }

         return 0;
      }
      
      private static IList<HistoryEntry> FilterProteinParameters(QueryParameters parameters, IEnumerable<HistoryEntry> entries)
      {
         var query = entries.AsQueryable();

         foreach (var field in parameters.Fields)
         {
            if (field.Name.Equals(QueryFieldName.WorkUnitName) ||
                field.Name.Equals(QueryFieldName.KFactor) ||
                field.Name.Equals(QueryFieldName.Core) ||
                field.Name.Equals(QueryFieldName.Frames) ||
                field.Name.Equals(QueryFieldName.Atoms) ||
                field.Name.Equals(QueryFieldName.ClientType) ||
                field.Name.Equals(QueryFieldName.PPD) ||
                field.Name.Equals(QueryFieldName.Credit))
            {
               try
               {
                  query = query.Where(BuildWhereCondition(field));
               }
               catch (ParseException ex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
               }
            }
         }

         return query.ToList();
      }

      private static string BuildWhereCondition(QueryField queryField)
      {
         string valueFormat = "{2}";
         if (queryField.Name.Equals(QueryFieldName.WorkUnitName) ||
             queryField.Name.Equals(QueryFieldName.Core))
         {
            valueFormat = "\"{2}\"";
         }
      
         var sbWhere = new StringBuilder();
         //if (queryField.Type.Equals(QueryFieldType.All) == false)
         //{
            sbWhere.AppendFormat(CultureInfo.InvariantCulture, "{0} {1} " + valueFormat, queryField.Name, queryField.Operator, queryField.Value);
         //}

         return sbWhere.ToString();
      }
   }

   internal class SelectStatementBuilder
   {
      private const string WuHistoryTableSelectAllSql = "SELECT * FROM [{0}] ";
      private const string AndSpace = "AND ";
   
      private bool _appendAnd;

      public string BuildSelectStatement(QueryParameters parameters)
      {
         if (parameters.Fields.Count == 0)
         {
            return String.Format(CultureInfo.InvariantCulture, WuHistoryTableSelectAllSql, UnitInfoDatabase.WuHistoryTableName);
         }
         
         // reset
         _appendAnd = false;

         var sbWhere = new StringBuilder("WHERE ");

         foreach (var field in parameters.Fields)
         {
            if (field.Name.Equals(QueryFieldName.ID) ||
                field.Name.Equals(QueryFieldName.ProjectID) ||
                field.Name.Equals(QueryFieldName.ProjectRun) ||
                field.Name.Equals(QueryFieldName.ProjectClone) ||
                field.Name.Equals(QueryFieldName.ProjectGen) ||
                field.Name.Equals(QueryFieldName.InstanceName) ||
                field.Name.Equals(QueryFieldName.InstancePath) ||
                field.Name.Equals(QueryFieldName.Username) ||
                field.Name.Equals(QueryFieldName.Team) ||
                field.Name.Equals(QueryFieldName.CoreVersion) ||
                field.Name.Equals(QueryFieldName.FramesCompleted) ||
                field.Name.Equals(QueryFieldName.FrameTime) ||
                field.Name.Equals(QueryFieldName.Result) ||
                field.Name.Equals(QueryFieldName.DownloadDateTime) ||
                field.Name.Equals(QueryFieldName.CompletionDateTime))
            {
               sbWhere.Append(BuildWhereCondition(field));
               sbWhere.Append(_appendAnd ? AndSpace : String.Empty);
            }
         }

         string selectCommand = String.Format(CultureInfo.InvariantCulture,
            WuHistoryTableSelectAllSql, UnitInfoDatabase.WuHistoryTableName);
         if (_appendAnd)
         {
            selectCommand += sbWhere.ToString();
            selectCommand = selectCommand.Remove(selectCommand.LastIndexOf(AndSpace)).Trim();
         }
         selectCommand += " ORDER BY [ID] ASC";

         return selectCommand;
      }

      private string BuildWhereCondition(QueryField queryField)
      {
         _appendAnd = true;

         var sbWhere = new StringBuilder();
         sbWhere.AppendFormat(CultureInfo.InvariantCulture, "[{0}] ", queryField.Name);
         sbWhere.Append(BuildValueCondition(queryField.Operator, queryField.Value));

         return sbWhere.ToString();
      }

      private static string BuildValueCondition(string oper, object value)
      {
         return String.Format(CultureInfo.InvariantCulture, "{0} {1} ", oper, GetFormattedValue(value));
      }

      private static string GetFormattedValue(object value)
      {
         if (value is DateTime)
         {
            var dateTime = (DateTime)value;
            return String.Format(CultureInfo.InvariantCulture, "'{0}'", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
         }

         return String.Format(CultureInfo.InvariantCulture, "'{0}'", value);
      }
   }

   public class CompletedUnitsReadResult
   {
      public CompletedUnitsReadResult()
      {
         Entries = new List<HistoryEntry>();
         ErrorLines = new List<string>();
      }

      public int Duplicates { get; set; }
      public List<HistoryEntry> Entries { get; private set; }
      public List<string> ErrorLines { get; private set; }
   }
   
   public interface ICompletedUnitsFileReader : IProgressProcessRunner
   {
      string CompletedUnitsFilePath { get; set; }
      CompletedUnitsReadResult Result { get; }
   
      void WriteCompletedUnitErrorLines(string filePath, IEnumerable<string> lines);
   }

   public class CompletedUnitsFileReader : ICompletedUnitsFileReader
   {
      private const string Comma = ",";

      public bool Processing { get; private set; }
      public Exception Exception { get; private set; }

      public string CompletedUnitsFilePath { get; set; }
      public CompletedUnitsReadResult Result { get; private set; }
   
      #region Import CompletedUnits.csv

      private static string GetUnitCsvHeader()
      {
         var sbldr = new StringBuilder();
         sbldr.Append("ProjectID");
         sbldr.Append(Comma);
         sbldr.Append("Work Unit Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Path");
         sbldr.Append(Comma);
         sbldr.Append("Username");
         sbldr.Append(Comma);
         sbldr.Append("Team");
         sbldr.Append(Comma);
         sbldr.Append("Client Type");
         sbldr.Append(Comma);
         sbldr.Append("Core Name");
         sbldr.Append(Comma);
         sbldr.Append("Core Version");
         sbldr.Append(Comma);
         sbldr.Append("Frame Time (Average)");
         sbldr.Append(Comma);
         sbldr.Append("PPD");
         sbldr.Append(Comma);
         sbldr.Append("Download Date");
         sbldr.Append(Comma);
         sbldr.Append("Download Time");
         sbldr.Append(Comma);
         sbldr.Append("Completion Date (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Completion Time (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Credit");
         sbldr.Append(Comma);
         sbldr.Append("Frames");
         sbldr.Append(Comma);
         sbldr.Append("Atoms");
         sbldr.Append(Comma);
         sbldr.Append("Run/Clone/Gen");

         return sbldr.ToString();
      }

      public event EventHandler<ProgressEventArgs> ProgressChanged;
      private void OnProgressChanged(ProgressEventArgs e)
      {
         if (ProgressChanged != null)
         {
            ProgressChanged(this, e);
         }
      }

      public event EventHandler ProcessFinished;
      private void OnProcessFinished(EventArgs e)
      {
         if (ProcessFinished != null)
         {
            ProcessFinished(this, e);
         }
      }

      public void Process()
      {
         Processing = true;
         Exception = null;
         Result = null;
         
         try
         {
            string[] lines = File.ReadAllLines(CompletedUnitsFilePath);
            var result = new CompletedUnitsReadResult();

            for (int i = 0; i < lines.Length; i++)
            {
               var progress = (int)((i / (double)lines.Length) * 100);
               
               try
               {
                  if (lines[i].Equals(GetUnitCsvHeader()))
                  {
                     continue;
                  }
                  HistoryEntry entry = ParseHistoryEntry(lines[i]);
                  if (result.Entries.Contains(entry))
                  {
                     result.Duplicates++;
                  }
                  else
                  {
                     result.Entries.Add(entry);
                  }
                  
                  OnProgressChanged(new ProgressEventArgs(progress, String.Format(CultureInfo.CurrentCulture,
                     "P{0} (R{1}, C{2}, G{3})", entry.ProjectID, entry.ProjectRun, entry.ProjectClone, entry.ProjectGen)));
               }
               catch (FormatException)
               {
                  result.ErrorLines.Add(lines[i]);
                  OnProgressChanged(new ProgressEventArgs(progress, String.Empty));
               }
            }

            Result = result;
         }
         catch (Exception ex)
         {
            Exception = ex;
         }
         finally
         {
            Processing = false;
            OnProcessFinished(EventArgs.Empty);
         }
      }
      
      public bool SupportsCancellation
      {
         get { return false; }
      }

      public void Cancel()
      {
         throw new NotImplementedException();
      }

      private static HistoryEntry ParseHistoryEntry(string line)
      {
         string[] tokens = line.Split(',');
         if (tokens.Length != 19)
         {
            throw new FormatException("Too many commas.");
         }

         var entry = new HistoryEntry();
         entry.ProjectID = Int32.Parse(tokens[0]);
         GetRunCloneGen(tokens[18], entry);
         entry.InstanceName = tokens[2];
         entry.InstancePath = tokens[3];
         entry.Username = tokens[4];
         entry.Team = Int32.Parse(tokens[5]);
         entry.CoreVersion = Single.Parse(tokens[8]);
         entry.FramesCompleted = 100; // assumed
         entry.FrameTime = TimeSpan.Parse(tokens[9]);
         entry.Result = WorkUnitResult.FinishedUnit; // assumed
         entry.DownloadDateTime = DateTime.ParseExact(tokens[11] + " " + tokens[12], "M/d/yyyy h:mm tt",
                                                      DateTimeFormatInfo.CurrentInfo,
                                                      DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal);
         entry.CompletionDateTime = DateTime.ParseExact(tokens[13] + " " + tokens[14], "M/d/yyyy h:mm tt",
                                                      DateTimeFormatInfo.CurrentInfo,
                                                      DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal);
         return entry;
      }

      private static void GetRunCloneGen(string token, HistoryEntry entry)
      {
         var regEx = new Regex("\\((?<Run>.*)/(?<Clone>.*)/(?<Gen>.*)\\)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
         var rcgMatch = regEx.Match(token);
         if (rcgMatch.Success == false) throw new FormatException("Cannot match R/C/G.");

         entry.ProjectRun = Int32.Parse(rcgMatch.Result("${Run}"));
         entry.ProjectClone = Int32.Parse(rcgMatch.Result("${Clone}"));
         entry.ProjectGen = Int32.Parse(rcgMatch.Result("${Gen}"));
      }

      public void WriteCompletedUnitErrorLines(string filePath, IEnumerable<string> lines)
      {
         using (var stream = File.CreateText(filePath))
         {
            stream.WriteLine(GetUnitCsvHeader());
            foreach (var line in lines)
            {
               stream.WriteLine(line);
            }
         }
      }

      #endregion
   }
}
