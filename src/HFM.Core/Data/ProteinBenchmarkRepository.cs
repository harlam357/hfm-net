using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public interface IProteinBenchmarkRepository
{
    Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync();

    Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier);

    ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier);

    Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID);

    Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID, int count);

    Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects);
}

public class ScopedProteinBenchmarkRepositoryProxy : IProteinBenchmarkRepository
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public ScopedProteinBenchmarkRepositoryProxy(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public async Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return await repository.GetSlotIdentifiersAsync().ConfigureAwait(false);
        }
    }

    public async Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return await repository.GetBenchmarkProjectsAsync(slotIdentifier).ConfigureAwait(false);
        }
    }

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        using (context)
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return repository.GetBenchmark(slotIdentifier, benchmarkIdentifier);
        }
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return await repository.GetBenchmarksAsync(slotIdentifier, projectID).ConfigureAwait(false);
        }
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID, int count)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return await repository.GetBenchmarksAsync(slotIdentifier, projectID, count).ConfigureAwait(false);
        }
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(_logger, context);
            return await repository.GetBenchmarksAsync(slotIdentifier, projects).ConfigureAwait(false);
        }
    }
}

public class ProteinBenchmarkRepository : IProteinBenchmarkRepository
{
    private const int DefaultMaxFrames = 300;

    public ILogger Logger { get; }

    private readonly WorkUnitContext _context;

    public ProteinBenchmarkRepository(ILogger logger, WorkUnitContext context)
    {
        Logger = logger ?? NullLogger.Instance;
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync()
    {
        var clients = (await _context.Clients
            .Where(x => x.Guid != null)
            .ToListAsync().ConfigureAwait(false))
            .Select(x => (
                ClientID: x.ID,
                ClientIdentifier: ClientIdentifier.FromConnectionString(x.Name, x.ConnectionString,
                    x.Guid is null ? Guid.Empty : Guid.Parse(x.Guid))));

        var clientSlots = (await _context.WorkUnits
            .Where(x => x.Frames.Count != 0)
            .Select(x => new { x.ClientID, x.ClientSlot })
            .Distinct()
            .ToListAsync().ConfigureAwait(false))
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

    public async Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier) =>
        await QueryWorkUnitsByClientSlot(slotIdentifier, _context)
            .Where(x => x.Frames.Count != 0)
            .Select(x => x.Protein.ProjectID)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync().ConfigureAwait(false);

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        var frames = QueryWorkUnitsByClientSlot(slotIdentifier, _context)
            .Include(x => x.Platform)
            .Where(x =>
                x.Protein.ProjectID == benchmarkIdentifier.ProjectID &&
                x.Platform.Processor == benchmarkIdentifier.Processor &&
                x.Platform.Threads == (benchmarkIdentifier.HasThreads ? benchmarkIdentifier.Threads : null))
            .SelectMany(x => x.Frames)
            .Where(x => x.Duration.ToString() != TimeSpan.Zero.ToString())
            .OrderByDescending(x => x.WorkUnitID).ThenByDescending(x => x.FrameID)
            .Take(DefaultMaxFrames)
            .ToList();

        return frames.Count == 0
            ? null
            : new ProteinBenchmark
            {
                SlotIdentifier = slotIdentifier,
                BenchmarkIdentifier = benchmarkIdentifier,
                FrameTimes = frames.Select(x => new ProteinBenchmarkFrameTime(x.Duration)).ToList(),
                MinimumFrameTime = frames.Any() ? frames.Min(x => x.Duration) : TimeSpan.Zero
            };
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID) =>
        await GetBenchmarksAsync(slotIdentifier, new[] { projectID }, null).ConfigureAwait(false);

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID, int count) =>
        await GetBenchmarksAsync(slotIdentifier, new[] { projectID }, count).ConfigureAwait(false);

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects) =>
        await GetBenchmarksAsync(slotIdentifier, projects, null).ConfigureAwait(false);

    private async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects, int? count) =>
        (await QueryWorkUnitsByClientSlot(slotIdentifier, _context)
            .Include(x => x.Protein)
            .Include(x => x.Client)
            .Include(x => x.Platform)
            .Include(x => x.Frames
                .Where(y => y.Duration.ToString() != TimeSpan.Zero.ToString())
                .OrderByDescending(y => y.FrameID))
            .Where(x => projects.Contains(x.Protein.ProjectID) && x.Frames.Any())
            .OrderByDescending(x => x.ID)
            .ToListAsync().ConfigureAwait(false))
            .GroupBy(x =>
                (SlotIdentifier: new SlotIdentifier(
                    ClientIdentifier.FromConnectionString(
                        x.Client.Name, x.Client.ConnectionString, Guid.Parse(x.Client.Guid)),
                            x.ClientSlot ?? SlotIdentifier.NoSlotID),
                BenchmarkIdentifier: new ProteinBenchmarkIdentifier(
                    x.Protein.ProjectID, x.Platform?.Processor, x.Platform?.Threads ?? ProteinBenchmarkIdentifier.NoThreads)))
            .Select(x =>
            {
                var frames = x
                    .SelectMany(y => y.Frames)
                    .Take(DefaultMaxFrames)
                    .ToList();

                return new ProteinBenchmark
                {
                    SlotIdentifier = x.Key.SlotIdentifier,
                    BenchmarkIdentifier = x.Key.BenchmarkIdentifier,
                    FrameTimes = frames.Select(y => new ProteinBenchmarkFrameTime(y.Duration)).ToList(),
                    MinimumFrameTime = frames.Min(y => y.Duration)
                };
            })
            .TakeWhile((_, i) => !count.HasValue || i < count)
            .ToList();

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
}

public class NullProteinBenchmarkRepository : IProteinBenchmarkRepository
{
    public static NullProteinBenchmarkRepository Instance { get; } = new();

    public async Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync() =>
        await Task.FromResult(Array.Empty<SlotIdentifier>()).ConfigureAwait(false);

    public async Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier) =>
        await Task.FromResult(Array.Empty<int>()).ConfigureAwait(false);

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier) => null;

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID) =>
        await Task.FromResult(Array.Empty<ProteinBenchmark>()).ConfigureAwait(false);

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID, int count) =>
        await Task.FromResult(Array.Empty<ProteinBenchmark>()).ConfigureAwait(false);

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects) =>
        await Task.FromResult(Array.Empty<ProteinBenchmark>()).ConfigureAwait(false);
}
