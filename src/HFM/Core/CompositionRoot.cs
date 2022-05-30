using LightInject;

namespace HFM.Core;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<Services.IUserStatsService, Services.UserStatsService>(new PerContainerLifetime());
        serviceRegistry.Register<ScheduledTasks.UserStatsScheduledTask>(new PerContainerLifetime());
        serviceRegistry.Initialize<ScheduledTasks.UserStatsScheduledTask>((_, instance) => instance.RunOrStartIfEnabled());

        // ApplicationUpdateService - Scoped
        serviceRegistry.Register<ApplicationUpdateService>(new PerScopeLifetime());
    }
}
