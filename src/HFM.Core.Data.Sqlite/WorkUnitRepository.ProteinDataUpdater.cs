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
                    var protein = _proteinService.Get((int)projectId);
                    if (protein is not null)
                    {
                        _database.Execute(_connection, UpdateSql,
                            protein.WorkUnitName, protein.KFactor, protein.Core, protein.Frames,
                            protein.NumberOfAtoms, protein.Credit, protein.PreferredDays, protein.MaximumDays,
                            projectId);
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

        private const string UpdateSql =
            "UPDATE [WuHistory] " +
               "SET [WorkUnitName] = @0, " +
                   "[KFactor] = @1, " +
                   "[Core] = @2, " +
                   "[Frames] = @3, " +
                   "[Atoms] = @4, " +
                   "[Credit] = @5, " +
                   "[PreferredDays] = @6, " +
                   "[MaximumDays] = @7 " +
               "WHERE ProjectID = @8";
    }
}
