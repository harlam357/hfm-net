using LightInject;

namespace HFM.Core.ApplicationUpdates;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<ApplicationUpdateService>(new PerScopeLifetime());
    }
}
