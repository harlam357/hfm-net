using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientIdentifierTests
    {
        [Test]
        public void ClientIdentifier_AreEqualWhenGuidsAreEqual()
        {
            // Arrange
            var guid = Guid.NewGuid(); 
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, guid);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, guid);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenGuidsAreNotEqual()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid());
            var y = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid());
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenFirstHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenSecondHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_AreEqualWhenNameServerPortAreEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenNameServerPortAreNotEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreNotEqualWhenGuidsAreEqual()
        {
            // Arrange
            var guid = Guid.NewGuid(); 
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, guid);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, guid);
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) | comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenGuidsAreNotEqual()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid());
            var y = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid());
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenFirstHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenSecondHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenNameServerPortAreEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreNotEqualWhenNameServerPortAreNotEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty);
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) | comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_CompareTo_WithGuidIsLessThanWithoutGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ClientIdentifier_CompareTo_WithoutGuidIsGreaterThanWithGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty);
            // Act
            var result = y.CompareTo(x);
            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void ClientIdentifier_CompareTo_ReturnsZeroWhenGuidsAreEqual()
        {
            // Arrange
            var guid = Guid.NewGuid();
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, guid);
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, guid);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(x == y);
            Assert.IsFalse(x != y);
            Assert.IsTrue(x <= y);
            Assert.IsTrue(x >= y);
            Assert.IsFalse(x < y);
            Assert.IsFalse(x > y);
        }

        [Test]
        public void ClientIdentifier_CompareTo_ReturnsNonZeroWhenGuidsAreNotEqual()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid());
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreNotEqual(0, result);
            Assert.IsFalse(x == y);
            Assert.IsTrue(x != y);
        }

        [Test]
        public void ClientIdentifier_CompareTo_ReturnsZeroWhenNameServerAndPortAreEqual()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreEqual(0, result);
            Assert.IsTrue(x == y);
            Assert.IsFalse(x != y);
            Assert.IsTrue(x <= y);
            Assert.IsTrue(x >= y);
            Assert.IsFalse(x < y);
            Assert.IsFalse(x > y);
        }

        [Test]
        public void ClientIdentifier_CompareTo_ReturnsNonZeroWhenNameServerAndPortAreNotEqual()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreNotEqual(0, result);
            Assert.IsFalse(x == y);
            Assert.IsTrue(x != y);
        }

        [Test]
        public void ClientIdentifier_ToString_WithServerAndPort()
        {
            var identifier = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            Assert.AreEqual($"Foo (Bar:{ClientSettings.DefaultPort})", identifier.ToString());
        }

        [Test]
        public void ClientIdentifier_ToString_WithServer()
        {
            var identifier = new ClientIdentifier("Foo", "Bar", ClientSettings.NoPort, Guid.Empty);
            Assert.AreEqual("Foo (Bar)", identifier.ToString());
        }

        [Test]
        public void ClientIdentifier_ToString_WithOnlyName()
        {
            var identifier = new ClientIdentifier("Foo", null, ClientSettings.NoPort, Guid.Empty);
            Assert.AreEqual("Foo", identifier.ToString());
        }

        [Test]
        public void ClientIdentifier_ToConnectionString_WithServerAndPort()
        {
            var identifier = new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty);
            Assert.AreEqual($"Bar:{ClientSettings.DefaultPort}", identifier.ToConnectionString());
        }

        [Test]
        public void ClientIdentifier_ToConnectionString_WithServer()
        {
            var identifier = new ClientIdentifier("Foo", "Bar", ClientSettings.NoPort, Guid.Empty);
            Assert.AreEqual("Bar", identifier.ToConnectionString());
        }

        [Test]
        public void ClientIdentifier_FromConnectionString_ServerDashPort()
        {
            const string path = "Server-12345";
            // Act
            var identifier = ClientIdentifier.FromConnectionString("Foo", path, Guid.Empty);
            // Assert
            Assert.AreEqual("Foo", identifier.Name);
            Assert.AreEqual("Server", identifier.Server);
            Assert.AreEqual(12345, identifier.Port);
            Assert.AreEqual(Guid.Empty, identifier.Guid);
        }

        [Test]
        public void ClientIdentifier_FromConnectionString_ServerColonPort()
        {
            const string path = "Server:12345";
            // Act
            var identifier = ClientIdentifier.FromConnectionString("Foo", path, Guid.Empty);
            // Assert
            Assert.AreEqual("Foo", identifier.Name);
            Assert.AreEqual("Server", identifier.Server);
            Assert.AreEqual(12345, identifier.Port);
            Assert.AreEqual(Guid.Empty, identifier.Guid);
        }

        [Test]
        public void ClientIdentifier_FromConnectionString_FileSystemPath()
        {
            const string path = @"\\server\share";
            var guid = Guid.NewGuid();
            // Act
            var identifier = ClientIdentifier.FromConnectionString("Bar", path, guid);
            // Assert
            Assert.AreEqual("Bar", identifier.Name);
            Assert.AreEqual(path, identifier.Server);
            Assert.AreEqual(ClientSettings.NoPort, identifier.Port);
            Assert.AreEqual(guid, identifier.Guid);
        }
    }
}
