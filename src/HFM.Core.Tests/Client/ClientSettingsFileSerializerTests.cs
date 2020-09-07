using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientSettingsFileSerializerTests
    {
        [Test]
        public void ClientSettingsFileSerializer_Deserialize_FromFile()
        {
            // Arrange
            var serializer = new ClientSettingsFileSerializer(null);
            // Act
            var settings = serializer.Deserialize("..\\..\\TestFiles\\ClientSettings_0_9_11.hfmx");
            // Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.Count);
            var s = settings.First();
            Assert.AreEqual(ClientType.FahClient, s.ClientType);
            Assert.AreEqual("Client1", s.Name);
            Assert.AreEqual("192.168.100.250", s.Server);
            Assert.AreEqual(36330, s.Port);
            Assert.AreEqual("foobar", s.Password);
            Assert.AreEqual(Guid.Empty, s.Guid);
        }

        [Test]
        public void ClientSettingsFileSerializer_RoundTrip_FromFile()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var toFile = new List<ClientSettings> { new ClientSettings
            {
                ClientType = ClientType.FahClient,
                Name = "Foo",
                Server = "Bar",
                Port = 12345,
                Password = "fizzbizz",
                Guid = guid
            } };
            var serializer = new ClientSettingsFileSerializer(null);
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                // Act
                serializer.Serialize(path, toFile);
                var fromFile = serializer.Deserialize(path); 
                // Assert
                Assert.AreEqual(toFile.Count, fromFile.Count);
                var to = toFile.First();
                var from = fromFile.First();
                Assert.AreEqual(to.ClientType, from.ClientType);
                Assert.AreEqual(to.Name, from.Name);
                Assert.AreEqual(to.Server, from.Server);
                Assert.AreEqual(to.Port, from.Port);
                Assert.AreEqual(to.Password, from.Password);
                Assert.AreEqual(to.Guid, from.Guid);
            }
        }

        [Test]
        public void ClientSettingsFileSerializer_Serialize_GeneratesGuidValuesWhenGuidIsEmpty()
        {
            // Arrange
            var toFile = new List<ClientSettings> { new ClientSettings() };
            var serializer = new ClientSettingsFileSerializer(null);
            using (var artifacts = new ArtifactFolder())
            {
                string path = artifacts.GetRandomFilePath();
                // Act
                serializer.Serialize(path, toFile);
                var fromFile = serializer.Deserialize(path); 
                // Assert
                var from = fromFile.First();
                Assert.AreNotEqual(Guid.Empty, from.Guid);
            }
        }
    }
}
