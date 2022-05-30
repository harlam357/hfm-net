using LightInject;

namespace HFM.Core;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        // ApplicationUpdateService - Scoped
        serviceRegistry.Register<ApplicationUpdateService>(new PerScopeLifetime());
    }
}
