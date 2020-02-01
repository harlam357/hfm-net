
using System.Linq;

using NUnit.Framework;

namespace HFM.Core.Serializers
{
    [TestFixture]
    public class HfmFileSerializerTests
    {
        [Test]
        public void HfmFileSerializer_Deserialize_FromFile()
        {
            // Arrange
            var serializer = new HfmFileSerializer();
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
