using LightInject;

namespace HFM.Core.Logging;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        var logger = new Logger(Application.DataFolderPath);
        serviceRegistry.Register<ILogger>(_ => logger, new PerContainerLifetime());
        serviceRegistry.Register<ILoggerEvents>(_ => logger, new PerContainerLifetime());
    }
}
