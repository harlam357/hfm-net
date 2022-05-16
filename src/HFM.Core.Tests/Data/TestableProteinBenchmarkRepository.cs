using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HFM.Core.Data;

public class TestableProteinBenchmarkRepository : IProteinBenchmarkRepository
{
    private readonly SqliteConnection _connection;
    private readonly string _connectionString;

    public TestableProteinBenchmarkRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public TestableProteinBenchmarkRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<ICollection<SlotIdentifier>> GetSlotIdentifiersAsync()
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(null, context);
            return await repository.GetSlotIdentifiersAsync().ConfigureAwait(false);
        }
    }

    public async Task<ICollection<int>> GetBenchmarkProjectsAsync(SlotIdentifier slotIdentifier)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(null, context);
            return await repository.GetBenchmarkProjectsAsync(slotIdentifier).ConfigureAwait(false);
        }
    }

    public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
    {
        var context = CreateWorkUnitContext();
        using (context)
        {
            var repository = new ProteinBenchmarkRepository(null, context);
            return repository.GetBenchmark(slotIdentifier, benchmarkIdentifier);
        }
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, int projectID)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(null, context);
            return await repository.GetBenchmarksAsync(slotIdentifier, projectID).ConfigureAwait(false);
        }
    }

    public async Task<ICollection<ProteinBenchmark>> GetBenchmarksAsync(SlotIdentifier slotIdentifier, IEnumerable<int> projects)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new ProteinBenchmarkRepository(null, context);
            return await repository.GetBenchmarksAsync(slotIdentifier, projects).ConfigureAwait(false);
        }
    }

    private static readonly object _CreateLock = new();

    private WorkUnitContext CreateWorkUnitContext()
    {
        var context = new WorkUnitContext(builder =>
        {
            if (_connection is null)
            {
                builder.UseConnectionString(_connectionString);
            }
            else
            {
                builder.UseConnection(_connection);
            }
#if DEBUG
            builder.LogTo(TestLogger.Instance.Debug, LogLevel.Information);
#endif
        });
        lock (_CreateLock)
        {
            context.Database.EnsureCreated();
        }
        return context;
    }
}
