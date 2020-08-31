using System.Collections.Generic;

using LightInject;

namespace HFM.Core
{
    public class CompositionRoot : ICompositionRoot
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "LightInject ILifetime instances.")]
        public void Compose(IServiceRegistry serviceRegistry)
        {
            // ILogger - Singleton
            serviceRegistry.Register(_ => new Logging.Logger(Application.DataFolderPath), new PerContainerLifetime());
            serviceRegistry.Register<Logging.ILogger>(factory => factory.GetInstance<Logging.Logger>(), new PerContainerLifetime());
            serviceRegistry.Register<Logging.ILoggerEvents>(factory => factory.GetInstance<Logging.Logger>(), new PerContainerLifetime());

            // IPreferenceSet - Singleton
            serviceRegistry.Register<Preferences.IPreferenceSet>(
                _ => new Preferences.XmlPreferencesProvider(Application.Path, Application.DataFolderPath, Application.FullVersion), new PerContainerLifetime());

            // IWorkUnitRepository - Singleton
            serviceRegistry.Register<Data.IWorkUnitRepository, Data.WorkUnitRepository>(new PerContainerLifetime());

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
            serviceRegistry.Initialize<Data.WorkUnitQueryDataContainer>((factory, instance) => instance.Read());

            // ProteinBenchmarkDataContainer - Singleton
            serviceRegistry.Register<Data.ProteinBenchmarkDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.ProteinBenchmarkDataContainer>((factory, instance) => instance.Read());

            // IProteinBenchmarkService - Singleton
            serviceRegistry.Register<WorkUnits.IProteinBenchmarkService, WorkUnits.ProteinBenchmarkService>(new PerContainerLifetime());

            // IEocStatsService - Singleton
            serviceRegistry.Register<Services.IEocStatsService, Services.EocStatsService>(new PerContainerLifetime());

            // EocStatsDataContainer - Singleton
            serviceRegistry.Register<Data.EocStatsDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.EocStatsDataContainer>((factory, instance) => instance.Read());

            // EocStatsScheduledTask - Singleton
            serviceRegistry.Register<ScheduledTasks.EocStatsScheduledTask>(new PerContainerLifetime());

            // ProteinDataContainer - Singleton
            serviceRegistry.Register<Data.ProteinDataContainer>(new PerContainerLifetime());
            serviceRegistry.Initialize<Data.ProteinDataContainer>((factory, instance) => instance.Read());

            // IProteinService - Singleton
            serviceRegistry.Register<WorkUnits.IProteinService, WorkUnits.ProteinService>(new PerContainerLifetime());

            // IProjectSummaryService - Singleton
            serviceRegistry.Register<Services.IProjectSummaryService, Services.ProjectSummaryService>(new PerContainerLifetime());

            // LocalProcessService - Singleton
            serviceRegistry.RegisterInstance(Services.LocalProcessService.Default);
        }
    }
}
