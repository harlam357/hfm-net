
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.Threading.Tasks;

using Castle.Core.Logging;

using harlam357.Core;
using harlam357.Core.ComponentModel;

namespace HFM.Core.Data
{
    public partial class WorkUnitRepository
    {
        private class DuplicateDeleter : AsyncProcessorBase
        {
            private readonly SQLiteConnection _connection;
            private readonly ILogger _logger;

            public DuplicateDeleter(SQLiteConnection connection, ILogger logger)
            {
                _connection = connection ?? throw new ArgumentNullException(nameof(connection));
                _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            }

            protected override async Task OnExecuteAsync(IProgress<ProgressInfo> progress)
            {
                await Task.Run(() => ExecuteInternal(progress)).ConfigureAwait(false);
            }

            private void ExecuteInternal(IProgress<ProgressInfo> progress)
            {
                var selectSql = PetaPoco.Sql.Builder.Select("ID", "ProjectID", "ProjectRun", "ProjectClone", "ProjectGen", "DownloadDateTime", "COUNT(*)")
                   .From("WuHistory")
                   .GroupBy("ProjectID", "ProjectRun", "ProjectClone", "ProjectGen", "DownloadDateTime")
                   .Append("HAVING COUNT(*) > 1");

                int count = 0;
                int totalCount = 0;
                _logger.Info("Checking for duplicate WU History entries...");

                using (var table = Select(_connection, selectSql.SQL))
                {
                    int lastProgress = 0;
                    foreach (DataRow row in table.Rows)
                    {
                        var deleteSql = PetaPoco.Sql.Builder.Append("DELETE FROM WuHistory")
                           .Where("ID < @0 AND ProjectID = @1 AND ProjectRun = @2 AND ProjectClone = @3 AND ProjectGen = @4 AND datetime(DownloadDateTime) = datetime(@5)",
                                  row.ItemArray[0], row.ItemArray[1], row.ItemArray[2], row.ItemArray[3], row.ItemArray[4], row.ItemArray[5]);

                        int result;
                        using (var database = new PetaPoco.Database(_connection))
                        {
                            result = database.Execute(deleteSql);
                        }
                        if (result != 0)
                        {
                            _logger.DebugFormat("Deleted rows: {0}", result);
                            totalCount += result;
                        }
                        count++;

                        int progressPercentage = Convert.ToInt32((count / (double)table.Rows.Count) * 100);
                        if (progressPercentage != lastProgress)
                        {
                            string message = String.Format(CultureInfo.CurrentCulture, "Deleting duplicate {0} of {1}.", count, table.Rows.Count);
                            progress?.Report(new ProgressInfo(progressPercentage, message));
                            lastProgress = progressPercentage;
                        }
                    }
                }

                if (totalCount != 0)
                {
                    _logger.InfoFormat("Total number of duplicate WU History entries deleted: {0}", totalCount);
                }
            }
        }
    }
}