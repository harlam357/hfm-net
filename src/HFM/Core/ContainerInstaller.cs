
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using Castle.MicroKernel.Registration;
using Castle.MicroKernel.Resolvers;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace HFM.Core
{
    [ExcludeFromCodeCoverage]
    public class ContainerInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ILazyComponentLoader>().ImplementedBy<LazyOfTComponentLoader>());

            // ILogger - Singleton
            container.Register(Component.For<Logging.ILogger, Logging.ILoggerEvents>().ImplementedBy<Logging.Logger>()
                .UsingFactoryMethod(() => new Logging.Logger(Application.DataFolderPath)));

            // IPreferenceSet - Singleton
            container.Register(Component.For<Preferences.IPreferenceSet>().ImplementedBy<Preferences.PreferenceSet>()
                .UsingFactoryMethod(() => new Preferences.PreferenceSet(Application.Path, Application.DataFolderPath, Application.FullVersion)));

            // IWorkUnitRepository - Singleton
            container.Register(Component.For<Data.IWorkUnitRepository>().ImplementedBy<Data.WorkUnitRepository>());

            // ClientConfiguration - Singleton
            container.Register(Component.For<Client.ClientConfiguration>());

            // ClientFactory - Singleton
            container.Register(Component.For<Client.ClientFactory>()
                .UsingFactoryMethod(kernel => new Client.ClientFactory(new Dictionary<Client.ClientType, Client.IClientTypeFactory> 
                    { { Client.ClientType.FahClient, kernel.Resolve<Client.FahClientTypeFactory>() } })));

            // HFM.Core.IFahClient - Transient
            container.Register(Component.For<Client.IFahClient>().ImplementedBy<Client.FahClient>().LifeStyle.Transient);

            // HFM.Core.IFahClientFactory - Singleton
            container.Register(Component.For<Client.FahClientTypeFactory>());

            // WorkUnitQueryDataContainer - Singleton
            container.Register(Component.For<Data.WorkUnitQueryDataContainer>()
                .OnCreate(instance => instance.Read()));

            // ProteinBenchmarkDataContainer - Singleton
            container.Register(Component.For<Data.ProteinBenchmarkDataContainer>()
                .OnCreate(instance => instance.Read())
                .OnDestroy(instance => instance.Write()));

            // IProteinBenchmarkService - Singleton
            container.Register(Component.For<WorkUnits.IProteinBenchmarkService>().ImplementedBy<WorkUnits.ProteinBenchmarkService>());

            // IEocStatsService - Singleton
            container.Register(Component.For<Services.IEocStatsService>().ImplementedBy<Services.EocStatsService>());

            // EocStatsDataContainer - Singleton
            container.Register(Component.For<Data.EocStatsDataContainer>()
                .OnCreate(instance => instance.Read()));

            // EocStatsScheduledTask - Singleton
            container.Register(Component.For<ScheduledTasks.EocStatsScheduledTask>());

            // ProteinDataContainer - Singleton
            container.Register(Component.For<Data.ProteinDataContainer>()
                .OnCreate(instance => instance.Read()));

            // IProteinService - Singleton
            container.Register(Component.For<WorkUnits.IProteinService>().ImplementedBy<WorkUnits.ProteinService>());

            // IProjectSummaryService - Singleton
            container.Register(Component.For<Services.IProjectSummaryService>().ImplementedBy<Services.ProjectSummaryService>());
        }
    }
}
