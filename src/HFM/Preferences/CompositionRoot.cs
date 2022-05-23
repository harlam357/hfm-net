using LightInject;

using Application = HFM.Core.Application;

namespace HFM.Preferences;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        var provider = new XmlPreferencesProvider(Application.Path, Application.DataFolderPath, Application.Version);
        serviceRegistry.Register<IPreferences>(_ => provider, new PerContainerLifetime());
    }
}
