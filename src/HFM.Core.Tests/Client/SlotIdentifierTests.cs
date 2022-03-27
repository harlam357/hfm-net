
using System;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class SlotIdentifierTests
    {
        [Test]
        public void SlotIdentifier_AreEqualWhenGuidsAreEqual()
        {
            // Arrange
            var guid = Guid.NewGuid(); 
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, guid), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Fizz", "Bizz", 46330, guid), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_AreNotEqualWhenGuidsAreNotEqual()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SlotIdentifier_AreNotEqualWhenFirstHasGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SlotIdentifier_AreNotEqualWhenSecondHasGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SlotIdentifier_AreEqualWhenNameServerPortAreEqualWithNoGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_AreNotEqualWhenNameServerPortAreNotEqualWithNoGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty), SlotIdentifier.NoSlotID);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SlotIdentifier_WithGuidIsLessThanWithoutGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void SlotIdentifier_WithoutGuidIsGreaterThanWithGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            // Act
            var result = y.CompareTo(x);
            // Assert
            Assert.AreEqual(1, result);
        }

        [Test]
        public void SlotIdentifier_ToString_WithServerAndPort()
        {
            var identifier = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), 0);
            Assert.AreEqual($"Foo Slot 00 (Bar:{ClientSettings.DefaultPort})", identifier.ToString());
        }

        [Test]
        public void SlotIdentifier_ToString_WithServer()
        {
            var identifier = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.NoPort, Guid.Empty), 0);
            Assert.AreEqual("Foo Slot 00 (Bar)", identifier.ToString());
        }

        [Test]
        public void SlotIdentifier_ToString_WithOnlyName()
        {
            var identifier = new SlotIdentifier(new ClientIdentifier("Foo", null, ClientSettings.NoPort, Guid.Empty), 0);
            Assert.AreEqual("Foo Slot 00", identifier.ToString());
        }

        [Test]
        public void SlotIdentifier_FromName_ServerDashPort()
        {
            const string name = "Foo";
            const string path = "Server-12345";
            // Act
            var identifier = SlotIdentifier.FromName(name, path, Guid.Empty);
            // Assert
            Assert.AreEqual(name, identifier.Name);
            Assert.AreEqual(-1, identifier.SlotID);
            Assert.AreEqual("Server", identifier.ClientIdentifier.Server);
            Assert.AreEqual(12345, identifier.ClientIdentifier.Port);
            Assert.AreEqual(Guid.Empty, identifier.ClientIdentifier.Guid);
        }

        [Test]
        public void SlotIdentifier_FromName_ServerColonPort()
        {
            const string name = "Foo";
            const string path = "Server:12345";
            // Act
            var identifier = SlotIdentifier.FromName(name, path, Guid.Empty);
            // Assert
            Assert.AreEqual(name, identifier.Name);
            Assert.AreEqual(-1, identifier.SlotID);
            Assert.AreEqual("Server", identifier.ClientIdentifier.Server);
            Assert.AreEqual(12345, identifier.ClientIdentifier.Port);
            Assert.AreEqual(Guid.Empty, identifier.ClientIdentifier.Guid);
        }

        [Test]
        public void SlotIdentifier_FromName_FileSystemPath()
        {
            const string name = "Bar";
            const string path = @"\\server\share";
            var guid = Guid.NewGuid();
            // Act
            var identifier = SlotIdentifier.FromName(name, path, guid);
            // Assert
            Assert.AreEqual(name, identifier.Name);
            Assert.AreEqual(-1, identifier.SlotID);
            Assert.AreEqual(path, identifier.ClientIdentifier.Server);
            Assert.AreEqual(ClientSettings.NoPort, identifier.ClientIdentifier.Port);
            Assert.AreEqual(guid, identifier.ClientIdentifier.Guid);
        }

        [Test]
        public void SlotIdentifier_FromName_WithSlot()
        {
            const string name = "Bar Slot 01";
            const string path = "Server:12345";
            // Act
            var identifier = SlotIdentifier.FromName(name, path, Guid.Empty);
            // Assert
            Assert.AreEqual(name, identifier.Name);
            Assert.AreEqual(1, identifier.SlotID);
            Assert.AreEqual("Server", identifier.ClientIdentifier.Server);
            Assert.AreEqual(12345, identifier.ClientIdentifier.Port);
            Assert.AreEqual(Guid.Empty, identifier.ClientIdentifier.Guid);
        }
    }
}
