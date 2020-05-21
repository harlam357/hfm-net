
using System;
using System.Data;
using System.Data.SQLite;
using System.Globalization;

using HFM.Core.Logging;

namespace HFM.Core.Data
{
    public class DuplicateDeleter
    {
        private readonly IWorkUnitRepository _repository;
        private readonly SQLiteConnection _connection;

        public DuplicateDeleter(IWorkUnitRepository repository, SQLiteConnection connection)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public void Execute(IProgress<ProgressInfo> progress)
        {
            var selectSql = PetaPoco.Sql.Builder.Select("ID", "ProjectID", "ProjectRun", "ProjectClone", "ProjectGen", "DownloadDateTime", "COUNT(*)")
               .From("WuHistory")
               .GroupBy("ProjectID", "ProjectRun", "ProjectClone", "ProjectGen", "DownloadDateTime")
               .Append("HAVING COUNT(*) > 1");

            int count = 0;
            int totalCount = 0;
            var logger = _repository.Logger ?? NullLogger.Instance;

            string message = "Checking for duplicate WU History entries...";
            logger.Info(message);
            progress?.Report(new ProgressInfo(0, message));

            using (var table = _repository.Select(_connection, selectSql.SQL))
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
                        logger.Debug($"Deleted rows: {result}");
                        totalCount += result;
                    }
                    count++;

                    int progressPercentage = Convert.ToInt32((count / (double)table.Rows.Count) * 100);
                    if (progressPercentage != lastProgress)
                    {
                        message = String.Format(CultureInfo.CurrentCulture, "Deleting duplicate {0} of {1}.", count, table.Rows.Count);
                        progress?.Report(new ProgressInfo(progressPercentage, message));
                        lastProgress = progressPercentage;
                    }
                }
            }

            if (totalCount != 0)
            {
                logger.Info($"Total number of duplicate WU History entries deleted: {totalCount}");
            }
        }
    }
}
