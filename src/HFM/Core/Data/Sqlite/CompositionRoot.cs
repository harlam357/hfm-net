using LightInject;

namespace HFM.Core.Data.Sqlite;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<WorkUnitRepository>(new PerRequestLifeTime());

        serviceRegistry.Register<IWorkUnitRepository, ScopedWorkUnitContextRepositoryProxy>(new PerContainerLifetime());
        serviceRegistry.Register<IProteinBenchmarkRepository, ScopedProteinBenchmarkRepositoryProxy>(new PerContainerLifetime());
        serviceRegistry.Decorate<IProteinBenchmarkRepository, ProteinBenchmarkRepositoryCachingDecorator>();

        serviceRegistry.AddDbContext(CreateWorkUnitContext);
    }

    private static WorkUnitContext CreateWorkUnitContext(IServiceFactory factory)
    {
        var preferences = factory.GetInstance<Preferences.IPreferences>();
        string appDataPath = preferences.Get<string>(Preferences.Preference.ApplicationDataFolderPath);
        string connectionString = $"Data Source={Path.Combine(appDataPath, "WorkUnits.db")}";
        return new WorkUnitContext(connectionString);
    }
}

internal static class ServiceRegistryExtensions
{
    internal static IServiceRegistry AddDbContext<TContext>(this IServiceRegistry serviceRegistry, Func<IServiceFactory, TContext> factory)
        where TContext : Microsoft.EntityFrameworkCore.DbContext =>
        serviceRegistry.Register(factory, new PerScopeLifetime());
}
