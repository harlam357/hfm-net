using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public interface IProteinBenchmarkRepository
{
    ICollection<SlotIdentifier> GetSlotIdentifiers();

    ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier);

    WorkUnitContextProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier);

    ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID);

    ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, IEnumerable<int> projects);
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

        var query = QueryWorkUnitsByClientSlot(slotIdentifier, context);
        return query
            .Where(x => x.Frames.Count != 0)
            .Select(x => x.Protein.ProjectID)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public WorkUnitContextProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        using var context = CreateWorkUnitContext();

        var query = QueryWorkUnitsByClientSlot(slotIdentifier, context);
        var frames = query
            .Where(x => x.Protein.ProjectID == benchmarkIdentifier.ProjectID &&
                        x.Processor == benchmarkIdentifier.Processor &&
                        x.Threads == (benchmarkIdentifier.HasThreads ? benchmarkIdentifier.Threads : null))
            .SelectMany(x => x.Frames)
            .Where(x => x.Duration.ToString() != TimeSpan.Zero.ToString())
            .OrderByDescending(x => x.WorkUnitID).ThenByDescending(x => x.FrameID)
            .Take(300)
            .ToList();

        return new WorkUnitContextProteinBenchmark
        {
            ProjectID = benchmarkIdentifier.ProjectID,
            Processor = benchmarkIdentifier.Processor,
            Threads = benchmarkIdentifier.HasThreads ? benchmarkIdentifier.Threads : ProteinBenchmarkIdentifier.NoThreads,
            FrameTimes = frames.Select(x => new ProteinBenchmarkFrameTime(x.Duration)).ToList(),
            MinimumFrameTime = frames.Min(x => x.Duration)
        };
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

    public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID) => throw new NotImplementedException();

    public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, IEnumerable<int> projects) => throw new NotImplementedException();

    protected abstract WorkUnitContext CreateWorkUnitContext();
}
