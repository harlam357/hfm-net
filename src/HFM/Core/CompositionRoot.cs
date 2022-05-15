using LightInject;

namespace HFM.Core
{
    public class CompositionRoot : ICompositionRoot
    {
        public void Compose(IServiceRegistry serviceRegistry)
        {
            // ILogger - Singleton
            serviceRegistry.Register(_ => new Logging.Logger(Application.DataFolderPath), new PerContainerLifetime());
            serviceRegistry.Register<Logging.ILogger>(factory => factory.GetInstance<Logging.Logger>(), new PerContainerLifetime());
            serviceRegistry.Register<Logging.ILoggerEvents>(factory => factory.GetInstance<Logging.Logger>(), new PerContainerLifetime());

            // IPreferenceSet - Singleton
            serviceRegistry.Register<Preferences.IPreferences>(
                _ => new Preferences.XmlPreferencesProvider(Application.Path, Application.DataFolderPath, Application.Version), new PerContainerLifetime());

            // IWorkUnitRepository - Singleton
            serviceRegistry.Register<Data.IWorkUnitRepository, Data.ScopedWorkUnitContextRepositoryProxy>(new PerContainerLifetime());

            // WorkUnitRepository - Transient
            serviceRegistry.Register<Data.WorkUnitRepository>(new PerRequestLifeTime());

            // WorkUnitContext - Scoped
            serviceRegistry.AddDbContext(CreateWorkUnitContext);

            // ClientConfiguration - Singleton
            serviceRegistry.Register<Client.ClientConfiguration>(new PerContainerLifetime());

            // ClientFactory - Singleton
            serviceRegistry.Register(
                factory => new Client.ClientFactory(new Dictionary<Client.ClientType, Client.IClientTypeFactory>
                    { { Client.ClientType.FahClient, new Client.FahClientTypeFactory(factory.GetInstance<Client.IFahClient>) } }), new PerContainerLifetime());

            // HFM.Core.IFahClient - Transient
            serviceRegistry.Register<Client.IFahClient, Client.FahClient>(new PerRequestLifeTime());

            // WorkUnitQueryDataContainer - Singleton
            serviceRegistry.Register<Data.WorkUnitQueryDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.WorkUnitQueryDataContainer>((_, instance) => instance.Read());

            // IProteinBenchmarkRepository - Singleton
            serviceRegistry.Register<Data.IProteinBenchmarkRepository, Data.ScopedProteinBenchmarkRepository>(new PerContainerLifetime());

            // IEocStatsService - Singleton
            serviceRegistry.Register<Services.IEocStatsService, Services.EocStatsService>(new PerContainerLifetime());

            // EocStatsDataContainer - Singleton
            serviceRegistry.Register<Data.EocStatsDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.EocStatsDataContainer>((_, instance) => instance.Read());

            // EocStatsScheduledTask - Singleton
            serviceRegistry.Register<ScheduledTasks.EocStatsScheduledTask>(new PerContainerLifetime());

            // ProteinDataContainer - Singleton
            serviceRegistry.Register<Data.ProteinDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.ProteinDataContainer>((_, instance) => instance.Read());

            // IProteinService - Singleton
            serviceRegistry.Register<WorkUnits.IProteinService, WorkUnits.ProteinService>(new PerContainerLifetime());

            // IProjectSummaryService - Singleton
            serviceRegistry.Register<Services.IProjectSummaryService, Services.ProjectSummaryService>(new PerContainerLifetime());

            // LocalProcessService - Singleton
            serviceRegistry.RegisterInstance(Services.LocalProcessService.Default);

            // ApplicationUpdateService - Scoped
            serviceRegistry.Register<ApplicationUpdateService>(new PerScopeLifetime());
        }

        private static Data.WorkUnitContext CreateWorkUnitContext(IServiceFactory factory)
        {
            var preferences = factory.GetInstance<Preferences.IPreferences>();
            string appDataPath = preferences.Get<string>(Preferences.Preference.ApplicationDataFolderPath);
            string connectionString = $"Data Source={Path.Combine(appDataPath, "WorkUnits.db")}";
            return new Data.WorkUnitContext(connectionString);
        }
    }

    internal static class ServiceRegistryExtensions
    {
        internal static IServiceRegistry AddDbContext<TContext>(this IServiceRegistry serviceRegistry, Func<IServiceFactory, TContext> factory)
            where TContext : Microsoft.EntityFrameworkCore.DbContext =>
            serviceRegistry.Register(factory, new PerScopeLifetime());
    }
}
