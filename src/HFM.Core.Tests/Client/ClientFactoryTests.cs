
using System;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientFactoryTests
    {
        [Test]
        public void ClientFactory_CreateCollection_ThrowsWhenSettingsIsNull()
        {
            var factory = new ClientFactory();
            Assert.Throws<ArgumentNullException>(() => factory.CreateCollection(null));
        }

        [Test]
        public void ClientFactory_CreateCollection_CreatesClientsOnlyForValidClientTypes()
        {
            // Arrange
            var factory = new ClientFactory();
            var client1 = new ClientSettings
            {
                Name = "Client 1",
                Server = "foo"
            };
            var client2 = new ClientSettings
            {
                Name = "Client 2",
                Server = "bar"
            };
            var client3 = new ClientSettings
            {
                ClientType = ClientType.Legacy,
                Name = "Client 3"
            };
            // Act
            var clients = factory.CreateCollection(new[] { client1, client2, client3 });
            // Assert
            Assert.AreEqual(2, clients.Count);
        }

        [Test]
        public void ClientFactory_CreateCollection_ThrowsWhenClientSettingsIsNotValid()
        {
            // Arrange
            var factory = new ClientFactory();
            var client = new ClientSettings
            {
                Name = "Client 1",
                Server = "foo",
                Port = 0
            };
            // Act & Assert
            Assert.Throws<ArgumentException>(() => factory.CreateCollection(new[] { client }));
        }

        [Test]
        public void ClientFactory_Create_ThrowsWhenSettingsIsNull()
        {
            var factory = new ClientFactory();
            Assert.Throws<ArgumentNullException>(() => factory.Create(null));
        }

        [Test]
        public void ClientFactory_Create_ThrowsWhenSettingsNameIsNotValid()
        {
            var factory = new ClientFactory();
            Assert.Throws<ArgumentException>(() => factory.Create(new ClientSettings()));
        }

        [Test]
        public void ClientFactory_Create_ThrowsWhenSettingsNameContainsInvalidChars()
        {
            var factory = new ClientFactory();
            var invalidChars = System.IO.Path.GetInvalidFileNameChars();
            Assert.Throws<ArgumentException>(() => factory.Create(new ClientSettings
            {
                Name = " " + invalidChars[0] + invalidChars[1] + " "
            }));
        }

        [Test]
        public void ClientFactory_Create_ThrowsWhenSettingsServerIsNotValid()
        {
            var factory = new ClientFactory();
            Assert.Throws<ArgumentException>(() => factory.Create(new ClientSettings { Name = "Client 1" }));
        }

        [Test]
        public void ClientFactory_Create_ReturnsNewClientAndSetsSettingsProperty()
        {
            // Arrange
            var factory = new ClientFactory();
            var settings = new ClientSettings
            {
                Name = "FahClient",
                Server = "192.168.100.200"
            };
            // Act
            var client = factory.Create(settings);
            // Assert
            Assert.IsNotNull(client);
            Assert.AreEqual(settings, client.Settings);
        }

        [Test]
        public void ClientFactory_Ctor_ThrowsWhenClientFactoriesIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ClientFactory(null));
        }
    }
}
