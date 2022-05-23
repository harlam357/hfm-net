using LightInject;

namespace HFM.Core.Client;

internal class CompositionRoot : ICompositionRoot
{
    public void Compose(IServiceRegistry serviceRegistry)
    {
        serviceRegistry.Register<ClientConfiguration>(new PerContainerLifetime());

        serviceRegistry.Register(factory =>
        {
            var clientFactories = new Dictionary<ClientType, IClientTypeFactory>
            {
                {
                    ClientType.FahClient,
                    new FahClientTypeFactory(factory.GetInstance<IFahClient>)
                }
            };
            return new ClientFactory(clientFactories);
        }, new PerContainerLifetime());

        serviceRegistry.Register<IFahClient, FahClient>(new PerRequestLifeTime());
    }
}
