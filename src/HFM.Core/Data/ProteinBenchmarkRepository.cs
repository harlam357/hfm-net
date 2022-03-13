using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public interface IProteinBenchmarkRepository
{
    ICollection<SlotIdentifier> GetSlotIdentifiers();

    ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier);

    ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier);

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

    public ICollection<SlotIdentifier> GetSlotIdentifiers() => GetSlotIdentifiersInternal(false);

    public ICollection<SlotIdentifier> GetAllSlotIdentifiers() => GetSlotIdentifiersInternal(true);

    private ICollection<SlotIdentifier> GetSlotIdentifiersInternal(bool all)
    {
        using var context = CreateWorkUnitContext();

        var clients = context.Clients
            .Where(x => all || x.Guid != null)
            .AsEnumerable()
            .Select(x => new
            {
                x.ID,
                ClientIdentifier = ClientIdentifier.FromConnectionString(x.Name, x.ConnectionString,
                    x.Guid is null ? Guid.Empty : Guid.Parse(x.Guid))
            })
            .ToList();

        var clientSlots = context.WorkUnits
            .Where(x => all || x.Frames.Count != 0)
            .Select(x => new { x.ClientID, x.ClientSlot })
            .Distinct()
            .AsEnumerable()
            .GroupBy(x => x.ClientID)
            .ToDictionary(g => g.Key, g => g.Select(x => x.ClientSlot));

        var slotIdentifiers = new List<SlotIdentifier>();
        foreach (var c in clients)
        {
            if (clientSlots.TryGetValue(c.ID, out var slots))
            {
                foreach (var s in slots)
                {
                    slotIdentifiers.Add(new SlotIdentifier(c.ClientIdentifier, s ?? SlotIdentifier.NoSlotID));
                }
            }
        }
        return slotIdentifiers;
    }

    public ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier) => GetBenchmarkProjectsInternal(slotIdentifier, false);

    public ICollection<int> GetAllBenchmarkProjects(SlotIdentifier slotIdentifier) => GetBenchmarkProjectsInternal(slotIdentifier, true);

    public ICollection<int> GetBenchmarkProjectsInternal(SlotIdentifier slotIdentifier, bool all)
    {
        using var context = CreateWorkUnitContext();

        var clients = context.Clients
            .Where(x => slotIdentifier.IsAllSlots() || x.Guid == slotIdentifier.ClientIdentifier.Guid.ToString());

        return context.WorkUnits
            .Where(x => all || x.Frames.Count != 0)
            .Where(x => clients.Contains(x.Client) || x.ClientSlot == (slotIdentifier.SlotID == SlotIdentifier.NoSlotID ? null : slotIdentifier.SlotID))
            .Select(x => x.Protein.ProjectID)
            .Distinct()
            .OrderBy(x => x)
            .ToList();
    }

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier) => throw new NotImplementedException();

    public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID) => throw new NotImplementedException();

    public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, IEnumerable<int> projects) => throw new NotImplementedException();

    protected abstract WorkUnitContext CreateWorkUnitContext();
}
