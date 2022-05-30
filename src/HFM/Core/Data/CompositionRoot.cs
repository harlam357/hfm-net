using LightInject;

namespace HFM.Core.Data;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<WorkUnitQueryDataContainer>(new PerContainerLifetime());
        serviceRegistry.Initialize<WorkUnitQueryDataContainer>((_, instance) => instance.Read());

        serviceRegistry.Register<UserStatsDataContainer>(new PerContainerLifetime());
        serviceRegistry.Initialize<UserStatsDataContainer>((_, instance) => instance.Read());

        serviceRegistry.Register<ProteinDataContainer>(new PerContainerLifetime());
        serviceRegistry.Initialize<ProteinDataContainer>((_, instance) => instance.Read());
    }
}
