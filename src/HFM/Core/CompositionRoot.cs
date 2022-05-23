using LightInject;

namespace HFM.Core;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        // IEocStatsService - Singleton
        serviceRegistry.Register<Services.IEocStatsService, Services.EocStatsService>(new PerContainerLifetime());

        // EocStatsScheduledTask - Singleton
        serviceRegistry.Register<ScheduledTasks.EocStatsScheduledTask>(new PerContainerLifetime());

        // ApplicationUpdateService - Scoped
        serviceRegistry.Register<ApplicationUpdateService>(new PerScopeLifetime());
    }
}
