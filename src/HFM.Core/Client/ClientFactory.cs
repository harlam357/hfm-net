using System;
using System.Collections.Generic;
using System.Linq;

using HFM.Core.Net;

namespace HFM.Core.Client
{
    public interface IClientTypeFactory
    {
        IClient Create();
    }

    public class NullClientTypeFactory : IClientTypeFactory
    {
        public IClient Create()
        {
            return new NullClient();
        }
    }

    public class FahClientTypeFactory : IClientTypeFactory
    {
        private readonly Func<IFahClient> _factory;

        public FahClientTypeFactory(Func<IFahClient> factory)
        {
            _factory = factory;
        }

        public IClient Create()
        {
            return _factory();
        }
    }

    public class ClientFactory
    {
        private readonly IDictionary<ClientType, IClientTypeFactory> _clientFactories;

        public ClientFactory() : this(new Dictionary<ClientType, IClientTypeFactory> { { ClientType.FahClient, new NullClientTypeFactory() } })
        {

        }

        public ClientFactory(IDictionary<ClientType, IClientTypeFactory> clientFactories)
        {
            _clientFactories = clientFactories ?? throw new ArgumentNullException(nameof(clientFactories));
        }

        public ICollection<IClient> CreateCollection(IEnumerable<ClientSettings> settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            return settings.Select(Create).Where(client => client != null).ToList();
        }

        public IClient Create(ClientSettings settings)
        {
            if (settings == null) throw new ArgumentNullException(nameof(settings));

            // special consideration for obsolete ClientType values that may appear in hfmx configuration files
            if (!_clientFactories.TryGetValue(settings.ClientType, out var factory))
            {
                return null;
            }

            if (!ClientSettings.ValidateName(settings.Name)) throw new ArgumentException($"Client name {settings.Name} is not valid.", nameof(settings));
            if (String.IsNullOrWhiteSpace(settings.Server)) throw new ArgumentException("Client server (host) name is empty.", nameof(settings));
            if (!TcpPort.Validate(settings.Port)) throw new ArgumentException($"Client server (host) port {settings.Port} is not valid.", nameof(settings));

            IClient client = factory.Create();
            if (client != null)
            {
                client.Settings = settings;
            }
            return client;
        }
    }
}
