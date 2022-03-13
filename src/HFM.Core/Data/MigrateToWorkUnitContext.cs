using System.Globalization;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public class MigrateToWorkUnitContext
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly WorkUnitRepository _repository;

    public MigrateToWorkUnitContext(ILogger logger, IServiceScopeFactory serviceScopeFactory, WorkUnitRepository repository)
    {
        _logger = logger ?? NullLogger.Instance;
        _serviceScopeFactory = serviceScopeFactory;
        _repository = repository;
    }

    private int _count;
    private int _total;
    private int _workUnitCount;
    private int _workUnitTotal;
    private int _lastPercent;

    public async Task ExecuteAsync(IProgress<ProgressInfo> progress)
    {
        ReportProgressMessage(progress, "Getting existing work unit data...");
        var rows = _repository.Fetch();

        _total = rows.Count * 2;
        _workUnitTotal = rows.Count;

        await AddClientsAndProjects(progress, rows).ConfigureAwait(false);
        await AddWorkUnits(progress, rows).ConfigureAwait(false);
        await AddVersions().ConfigureAwait(false);
    }

    private async Task AddClientsAndProjects(IProgress<ProgressInfo> progress, IEnumerable<WorkUnitRow> rows)
    {
        ReportProgressMessage(progress, "Finding clients and projects...");
        var proteins = new HashSet<ProteinEntity>();
        var clients = new HashSet<ClientEntity>();

        foreach (var r in rows.Cast<PetaPocoWorkUnitRow>())
        {
            ReportClientAndProjectProgress(progress);

            proteins.Add(new ProteinEntity
            {
                ProjectID = r.ProjectID,
                Atoms = r.Atoms,
                Core = r.Core,
                Credit = r.BaseCredit,
                Frames = r.Frames,
                KFactor = r.KFactor,
                TimeoutDays = r.PreferredDays,
                ExpirationDays = r.MaximumDays
            });

            var slotIdentifier = SlotIdentifier.FromName(r.Name, r.Path, Guid.Empty);

            clients.Add(new ClientEntity
            {
                Name = slotIdentifier.ClientIdentifier.Name,
                ConnectionString = slotIdentifier.ClientIdentifier.ToConnectionString()
            });
        }

        await AddRange(proteins).ConfigureAwait(false);
        await AddRange(clients).ConfigureAwait(false);
    }

    private async Task AddWorkUnits(IProgress<ProgressInfo> progress, IEnumerable<WorkUnitRow> rows)
    {
        var workUnits = new HashSet<WorkUnitEntity>();

        using (var scope = _serviceScopeFactory.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
            foreach (var r in rows.Cast<PetaPocoWorkUnitRow>())
            {
                ReportWorkUnitProgress(progress);

                var slotIdentifier = SlotIdentifier.FromName(r.Name, r.Path, Guid.Empty);

                var p = await context.Proteins.FirstAsync(x =>
                        x.ProjectID == r.ProjectID &&
                        Math.Abs(x.Credit - r.BaseCredit) < 0.001 &&
                        Math.Abs(x.KFactor - r.KFactor) < 0.001 &&
                        x.Frames == r.Frames &&
                        x.Core == r.Core &&
                        x.Atoms == r.Atoms &&
                        Math.Abs(x.TimeoutDays - r.PreferredDays) < 0.001 &&
                        Math.Abs(x.ExpirationDays - r.MaximumDays) < 0.001)
                    .ConfigureAwait(false);

                var c = await context.Clients.FirstAsync(x =>
                        x.Name == slotIdentifier.ClientIdentifier.Name &&
                        x.ConnectionString == slotIdentifier.ClientIdentifier.ToConnectionString())
                    .ConfigureAwait(false);

                workUnits.Add(new WorkUnitEntity
                {
                    ProteinID = p.ID,
                    ClientID = c.ID,
                    DonorName = r.Username,
                    DonorTeam = r.Team,
                    CoreVersion = r.CoreVersion.ToString(CultureInfo.InvariantCulture),
                    Result = r.Result,
                    Assigned = r.Assigned.Normalize(),
                    Finished = r.Finished.Normalize(),
                    ProjectRun = r.ProjectRun,
                    ProjectClone = r.ProjectClone,
                    ProjectGen = r.ProjectGen,
                    HexID = null,
                    FramesCompleted = r.FramesCompleted,
                    FrameTimeInSeconds = r.FrameTimeValue,
                    ClientSlot = slotIdentifier.SlotID >= 0 ? slotIdentifier.SlotID : null
                });
            }
        }

        ReportProgressMessage(progress, "Saving work unit data...");
        await AddRange(workUnits).ConfigureAwait(false);
    }

    private async Task AddVersions()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();

        var database = (IWorkUnitDatabase)_repository;
        var table = database.Select("SELECT * FROM [DbVersion]");
        foreach (System.Data.DataRow row in table.Rows)
        {
            context.Versions.Add(new VersionEntity { Version = row["Version"].ToString() });
        }
        context.Versions.Add(new VersionEntity { Version = Application.Version });

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private void ReportProgressMessage(IProgress<ProgressInfo> progress, string message)
    {
        _logger.Info(message);
        progress.Report(new ProgressInfo(_lastPercent, message));
    }

    private void ReportClientAndProjectProgress(IProgress<ProgressInfo> progress)
    {
        int percent = CalculatePercent(++_count, _total);
        if (percent != _lastPercent)
        {
            _lastPercent = percent;
            progress.Report(new ProgressInfo(_lastPercent, null));
        }
    }

    private void ReportWorkUnitProgress(IProgress<ProgressInfo> progress)
    {
        int percent = CalculatePercent(++_count, _total);
        _workUnitCount++;
        if (percent != _lastPercent || _workUnitCount == _workUnitTotal)
        {
            _lastPercent = percent;
            string message = $"Migrating work unit {_workUnitCount} of {_workUnitTotal}";
            _logger.Info(message);
            progress.Report(new ProgressInfo(_lastPercent, message));
        }
    }

    private static int CalculatePercent(int count, int total) => ((count * 200) + total) / (total * 2);

    private async Task AddRange<T>(IEnumerable<T> entities, int count = 100) where T : class
    {
        foreach (var batch in ToInMemoryBatches(entities, count))
        {
            using var scope = _serviceScopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
            await context.Set<T>().AddRangeAsync(batch).ConfigureAwait(false);
            await context.SaveChangesAsync().ConfigureAwait(false);
        }
    }

    private static IEnumerable<IEnumerable<T>> ToInMemoryBatches<T>(IEnumerable<T> source, int count)
    {
        List<T> batch = null;
        foreach (var item in source)
        {
            batch ??= new List<T>();
            batch.Add(item);

            if (batch.Count != count)
            {
                continue;
            }

            yield return batch;
            batch = null;
        }

        if (batch != null)
        {
            yield return batch;
        }
    }
}
