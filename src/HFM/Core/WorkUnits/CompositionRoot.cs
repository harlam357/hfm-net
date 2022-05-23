using LightInject;

namespace HFM.Core.WorkUnits;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<IProteinService, ProteinService>(new PerContainerLifetime());
        serviceRegistry.Register<IProjectSummaryService, ProjectSummaryService>(new PerContainerLifetime());
    }
}
