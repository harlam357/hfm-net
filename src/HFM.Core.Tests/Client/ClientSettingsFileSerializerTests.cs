
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientSettingsFileSerializerTests
    {
        [Test]
        public void ClientSettingsFileSerializer_Deserialize_FromFile()
        {
            // Arrange
            var serializer = new ClientSettingsFileSerializer();
            // Act
            var settings = serializer.Deserialize("..\\..\\TestFiles\\TestClientSettings.hfmx");
            // Assert
            Assert.IsNotNull(settings);
            Assert.AreEqual(1, settings.Count);
            var s = settings.First();
            Assert.AreEqual(ClientType.FahClient, s.ClientType);
            Assert.AreEqual("Client1", s.Name);
            Assert.AreEqual("192.168.100.250", s.Server);
            Assert.AreEqual(36330, s.Port);
            Assert.AreEqual("foobar", s.Password);
        }
    }
}
