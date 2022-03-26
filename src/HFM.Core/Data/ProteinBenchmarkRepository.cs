using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public interface IProteinBenchmarkRepository
{
    ICollection<SlotIdentifier> GetSlotIdentifiers();

    ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier);

    WorkUnitContextProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier);

    ICollection<WorkUnitContextProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID);

    ICollection<WorkUnitContextProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, IEnumerable<int> projects);
}

public class ScopedProteinBenchmarkRepository : ProteinBenchmarkRepository
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopedProteinBenchmarkRepository(ILogger logger, IServiceScopeFactory serviceScopeFactory) : base(logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override WorkUnitContext CreateWorkUnitContext()
    {
        var scope = _serviceScopeFactory.CreateScope();
        return scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
    }
}

public abstract class ProteinBenchmarkRepository : IProteinBenchmarkRepository
{
    private const int DefaultMaxFrames = 300;

    public ILogger Logger { get; }

    protected ProteinBenchmarkRepository() : this(null)
    {

    }

    protected ProteinBenchmarkRepository(ILogger logger)
    {
        Logger = logger ?? NullLogger.Instance;
    }

    public ICollection<SlotIdentifier> GetSlotIdentifiers()
    {
        using var context = CreateWorkUnitContext();

        var clients = context.Clients
            .Where(x => x.Guid != null)
            .AsEnumerable()
            .Select(x => (
                ClientID: x.ID,
                ClientIdentifier: ClientIdentifier.FromConnectionString(x.Name, x.ConnectionString,
                    x.Guid is null ? Guid.Empty : Guid.Parse(x.Guid))));

        var clientSlots = context.WorkUnits
            .Where(x => x.Frames.Count != 0)
            .Select(x => new { x.ClientID, x.ClientSlot })
            .Distinct()
            .AsEnumerable()
            .ToLookup(x => x.ClientID, x => x.ClientSlot);

        var slotIdentifiers = new List<SlotIdentifier>();
        foreach (var c in clients)
        {
            slotIdentifiers.AddRange(
                clientSlots[c.ClientID].Select(s =>
                    new SlotIdentifier(c.ClientIdentifier, s ?? SlotIdentifier.NoSlotID)));
        }
        return slotIdentifiers;
    }

    public ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier)
    {
        using var context = CreateWorkUnitContext();

        return QueryWorkUnitsByClientSlot(slotIdentifier, context)
            .Where(x => x.Frames.Count != 0)
            .Select(x => x.Protein.ProjectID)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public WorkUnitContextProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        using var context = CreateWorkUnitContext();

        var frames = QueryWorkUnitsByClientSlot(slotIdentifier, context)
            .Where(x =>
                x.Protein.ProjectID == benchmarkIdentifier.ProjectID &&
                x.Processor == benchmarkIdentifier.Processor &&
                x.Threads == (benchmarkIdentifier.HasThreads ? benchmarkIdentifier.Threads : null))
            .SelectMany(x => x.Frames)
            .Where(x => x.Duration.ToString() != TimeSpan.Zero.ToString())
            .OrderByDescending(x => x.WorkUnitID).ThenByDescending(x => x.FrameID)
            .Take(DefaultMaxFrames)
            .ToList();

        return new WorkUnitContextProteinBenchmark
        {
            SlotIdentifier = slotIdentifier,
            BenchmarkIdentifier = benchmarkIdentifier,
            FrameTimes = frames.Select(x => new ProteinBenchmarkFrameTime(x.Duration)).ToList(),
            MinimumFrameTime = frames.Min(x => x.Duration)
        };
    }

    public ICollection<WorkUnitContextProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID) =>
        GetBenchmarks(slotIdentifier, new[] { projectID });

    public ICollection<WorkUnitContextProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, IEnumerable<int> projects)
    {
        using var context = CreateWorkUnitContext();

        return QueryWorkUnitsByClientSlot(slotIdentifier, context)
            .Include(x => x.Protein)
            .Include(x => x.Client)
            .Include(x => x.Frames
                .Where(y => y.Duration.ToString() != TimeSpan.Zero.ToString())
                .OrderByDescending(y => y.FrameID))
            .Where(x => projects.Contains(x.Protein.ProjectID) && x.Frames.Any())
            .OrderByDescending(x => x.ID)
            .AsEnumerable()
            .GroupBy(x =>
                (SlotIdentifier: new SlotIdentifier(
                    ClientIdentifier.FromConnectionString(
                        x.Client.Name, x.Client.ConnectionString, Guid.Parse(x.Client.Guid)),
                            x.ClientSlot ?? SlotIdentifier.NoSlotID),
                BenchmarkIdentifier: new ProteinBenchmarkIdentifier(
                    x.Protein.ProjectID, x.Processor, x.Threads ?? ProteinBenchmarkIdentifier.NoThreads)))
            .Select(x =>
            {
                var frames = x
                    .SelectMany(y => y.Frames)
                    .Take(DefaultMaxFrames)
                    .ToList();

                return new WorkUnitContextProteinBenchmark
                {
                    SlotIdentifier = x.Key.SlotIdentifier,
                    BenchmarkIdentifier = x.Key.BenchmarkIdentifier,
                    FrameTimes = frames.Select(y => new ProteinBenchmarkFrameTime(y.Duration)).ToList(),
                    MinimumFrameTime = frames.Min(y => y.Duration)
                };
            })
            .ToList();
    }

    private static IQueryable<WorkUnitEntity> QueryWorkUnitsByClientSlot(SlotIdentifier slotIdentifier, WorkUnitContext context)
    {
        var query = context.WorkUnits.AsQueryable();

        if (!slotIdentifier.IsAllSlots())
        {
            int? clientSlot = slotIdentifier.HasSlotID
                ? slotIdentifier.SlotID
                : null;

            var clients = context.Clients.Where(x => x.Guid == slotIdentifier.ClientIdentifier.Guid.ToString());
            query = query.Where(x => clients.Contains(x.Client) && x.ClientSlot == clientSlot);
        }

        return query;
    }

    protected abstract WorkUnitContext CreateWorkUnitContext();
}
