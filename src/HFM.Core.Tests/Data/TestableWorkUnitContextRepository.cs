using HFM.Core.Logging;
using HFM.Core.WorkUnits;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HFM.Core.Data;

public class TestableWorkUnitContextRepository : IWorkUnitRepository
{
    private readonly SqliteConnection _connection;
    private readonly string _connectionString;

    public TestableWorkUnitContextRepository(SqliteConnection connection)
    {
        _connection = connection;
    }

    public TestableWorkUnitContextRepository(string connectionString)
    {
        _connectionString = connectionString;
    }

    public async Task<long> UpdateAsync(WorkUnitModel workUnitModel)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.UpdateAsync(workUnitModel).ConfigureAwait(false);
        }
    }

    public async Task<int> DeleteAsync(WorkUnitRow row)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.DeleteAsync(row).ConfigureAwait(false);
        }
    }

    public async Task<IList<WorkUnitRow>> FetchAsync(WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.FetchAsync(query, bonusCalculation).ConfigureAwait(false);
        }
    }

    public async Task<Page<WorkUnitRow>> PageAsync(long page, long itemsPerPage, WorkUnitQuery query, BonusCalculation bonusCalculation)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.PageAsync(page, itemsPerPage, query, bonusCalculation).ConfigureAwait(false);
        }
    }

    public async Task<long> CountCompletedAsync(string clientName, DateTime? clientStartTime)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.CountCompletedAsync(clientName, clientStartTime).ConfigureAwait(false);
        }
    }

    public async Task<long> CountFailedAsync(string clientName, DateTime? clientStartTime)
    {
        var context = CreateWorkUnitContext();
        await using (context.ConfigureAwait(false))
        {
            var repository = new WorkUnitContextRepository(null, context);
            return await repository.CountFailedAsync(clientName, clientStartTime).ConfigureAwait(false);
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
