
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

using harlam357.Core;
using harlam357.Core.ComponentModel;

namespace HFM.Core.Data.SQLite
{
   public sealed partial class UnitInfoDatabase
   {
      private sealed class ProteinDataUpdater : ProteinDataUpdaterBase, IAsyncProcessor
      {
         public ProteinDataUpdater(SQLiteConnection connection, IProteinService proteinService)
            : base(connection, proteinService)
         {

         }

         public Exception Exception { get; private set; }

         public bool IsCompleted { get; private set; }

         public bool IsFaulted { get; private set; }

         public async Task ExecuteAsync(IProgress<ProgressInfo> progress)
         {
            try
            {
               var ct = CancellationToken.None;
               await Task.Run(() => ExecuteInternal(ct, progress), ct).ConfigureAwait(false);
               IsCompleted = true;
            }
            catch (Exception ex)
            {
               Exception = ex;
               IsFaulted = true;
            }
         }
      }

      private sealed class ProteinDataUpdaterWithCancellation : ProteinDataUpdaterBase, IAsyncProcessorWithCancellation
      {
         public ProteinDataUpdaterWithCancellation(SQLiteConnection connection, IProteinService proteinService)
            : base(connection, proteinService)
         {

         }

         public Exception Exception { get; private set; }

         public bool IsCanceled { get; private set; }

         public bool IsCompleted { get; private set; }

         public bool IsFaulted { get; private set; }

         public async Task ExecuteAsync(IProgress<ProgressInfo> progress)
         {
            await ExecuteAsync(CancellationToken.None, progress).ConfigureAwait(false);
         }

         public async Task ExecuteAsync(CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
         {
            try
            {
               await Task.Run(() => ExecuteInternal(cancellationToken, progress), cancellationToken).ConfigureAwait(false);
               IsCompleted = true;
            }
            catch (OperationCanceledException)
            {
               IsCanceled = true;
            }
            catch (Exception ex)
            {
               Exception = ex;
               IsFaulted = true;
            }
         }
      }

      private abstract class ProteinDataUpdaterBase
      {
         private readonly SQLiteConnection _connection;
         private readonly IProteinService _proteinService;

         protected ProteinDataUpdaterBase(SQLiteConnection connection, IProteinService proteinService)
         {
            if (connection == null) throw new ArgumentNullException("connection");
            if (proteinService == null) throw new ArgumentNullException("proteinService");

            _connection = connection;
            _proteinService = proteinService;
         }

         public ProteinUpdateType UpdateType { get; set; }

         public long UpdateArg { get; set; }

         protected void ExecuteInternal(CancellationToken cancellationToken, IProgress<ProgressInfo> progress)
         {
            const string workUnitNameUnknown = "WorkUnitName = '' OR WorkUnitName = 'Unknown'";

            if (UpdateType == ProteinUpdateType.All ||
                UpdateType == ProteinUpdateType.Unknown)
            {
               var selectSql = PetaPoco.Sql.Builder.Select("ProjectID").From("WuHistory");
               if (UpdateType == ProteinUpdateType.Unknown)
               {
                  selectSql = selectSql.Where(workUnitNameUnknown);
               }
               selectSql = selectSql.GroupBy("ProjectID");

               using (var table = Select(_connection, selectSql.SQL))
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
                        if (UpdateType == ProteinUpdateType.Unknown)
                        {
                           updateSql = updateSql.Where(workUnitNameUnknown);
                        }
                        Execute(_connection, updateSql.SQL, updateSql.Arguments);
                     }
                     count++;

                     int progressPercentage = Convert.ToInt32((count / (double)table.Rows.Count) * 100);
                     if (progressPercentage != lastProgress)
                     {
                        string message = String.Format(CultureInfo.CurrentCulture, "Updating project {0} of {1}.", count, table.Rows.Count);
                        if (progress != null)
                        {
                           progress.Report(new ProgressInfo(progressPercentage, message));
                        }
                        lastProgress = progressPercentage;
                     }
                  }
               }
            }
            else if (UpdateType == ProteinUpdateType.Project)
            {
               int projectId = (int)UpdateArg;
               var updateSql = GetUpdateSql(projectId, "ProjectID", projectId);
               if (updateSql != null)
               {
                  Execute(_connection, updateSql.SQL, updateSql.Arguments);
               }
            }
            else if (UpdateType == ProteinUpdateType.Id)
            {
               long id = UpdateArg;
               var selectSql = PetaPoco.Sql.Builder.Select("ProjectID").From("WuHistory").Where("ID = @0", id);

               using (var table = Select(_connection, selectSql.SQL, selectSql.Arguments))
               {
                  if (table.Rows.Count != 0)
                  {
                     var projectId = table.Rows[0].Field<int>("ProjectID");
                     var updateSql = GetUpdateSql(projectId, "ID", id);
                     if (updateSql != null)
                     {
                        Execute(_connection, updateSql.SQL, updateSql.Arguments);
                     }
                  }
               }
            }
         }

         private PetaPoco.Sql GetUpdateSql(int projectId, string column, long arg)
         {
            // get the correct protein
            var protein = _proteinService.Get(projectId);
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
}
