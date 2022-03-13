using HFM.Core.Logging;

using Microsoft.Data.Sqlite;
using Microsoft.Extensions.Logging;

namespace HFM.Core.Data;

public class TestableWorkUnitContextRepository : WorkUnitContextRepository
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

    private static readonly object _CreateLock = new();

    protected override WorkUnitContext CreateWorkUnitContext()
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
