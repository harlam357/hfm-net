using HFM.Core.Client;
using HFM.Core.WorkUnits;

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
