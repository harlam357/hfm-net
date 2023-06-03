using HFM.Core.Client;
using HFM.Core.Collections;
using HFM.Core.WorkUnits;

using Microsoft.Extensions.Caching.Memory;

namespace HFM.Core.Data;

public class ProteinBenchmarkRepositoryCachingDecorator : IProteinBenchmarkRepository
{
    private readonly IProteinBenchmarkRepository _repository;
    private readonly IMemoryCache _memoryCache;

    public ProteinBenchmarkRepositoryCachingDecorator(IProteinBenchmarkRepository repository, IMemoryCache memoryCache)
    {
        _repository = repository;
        _memoryCache = memoryCache;
    }

    public Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync() =>
        _repository.GetSlotIdentifiersAsync();

    public Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier) =>
        _repository.GetBenchmarkProjectsAsync(slotIdentifier);

    private const double BenchmarkAverageFrameTimeFactor = 0.56;

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        var key = (slotIdentifier, benchmarkIdentifier);
        if (_memoryCache.TryGetValue(key, out var value))
        {
            return (ProteinBenchmark)value;
        }

        var benchmark = _repository.GetBenchmark(slotIdentifier, benchmarkIdentifier);
        if (benchmark is not null)
        {
            var expiration = benchmark.AverageFrameTime * BenchmarkAverageFrameTimeFactor;
            _memoryCache.Set(key, benchmark, expiration);
        }

        return benchmark;
    }

    private const int BenchmarksExpirationInSeconds = 5;

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID, int count)
    {
        var key = (slotIdentifier, projectID, count);
        if (_memoryCache.TryGetValue(key, out var value))
        {
            return (ICollection<ProteinBenchmark>)value;
        }

        var benchmarks = await _repository.GetBenchmarksAsync(slotIdentifier, projectID, count).ConfigureAwait(false);
        if (benchmarks.Count != 0)
        {
            var expiration = TimeSpan.FromSeconds(BenchmarksExpirationInSeconds);
            _memoryCache.Set(key, benchmarks, expiration);
        }

        return benchmarks;
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects)
    {
        var projectsCollection = projects.CastOrToCollection();
        var key = (slotIdentifier, String.Join(',', projectsCollection));
        if (_memoryCache.TryGetValue(key, out var value))
        {
            return (ICollection<ProteinBenchmark>)value;
        }

        var benchmarks = await _repository.GetBenchmarksAsync(slotIdentifier, projectsCollection).ConfigureAwait(false);
        if (benchmarks.Count != 0)
        {
            var expiration = TimeSpan.FromSeconds(BenchmarksExpirationInSeconds);
            _memoryCache.Set(key, benchmarks, expiration);
        }

        return benchmarks;
    }
}
