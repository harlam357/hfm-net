using LightInject;

namespace HFM.Core.UserStats;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<UserStatsDataContainer>(new PerContainerLifetime());
        serviceRegistry.Initialize<UserStatsDataContainer>((_, instance) => instance.Read());
        serviceRegistry.Register<IUserStatsService, UserStatsService>(new PerContainerLifetime());
        serviceRegistry.Register<UserStatsScheduledTask>(new PerContainerLifetime());
        serviceRegistry.Initialize<UserStatsScheduledTask>((_, instance) => instance.RunOrStartIfEnabled());
    }
}
