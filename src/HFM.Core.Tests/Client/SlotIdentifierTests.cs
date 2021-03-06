﻿
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
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreNotEqualWhenGuidsAreEqual()
        {
            // Arrange
            var guid = Guid.NewGuid(); 
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, guid), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Fizz", "Bizz", 46330, guid), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) | comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenGuidsAreNotEqual()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenFirstHasGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenSecondHasGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier(null, null, ClientSettings.NoPort, Guid.NewGuid()), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenNameServerPortAreEqualWithNoGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void SlotIdentifier_ProteinBenchmarkEqualityComparer_AreNotEqualWhenNameServerPortAreNotEqualWithNoGuid()
        {
            // Arrange
            var x = new SlotIdentifier(new ClientIdentifier("Foo", "Bar", ClientSettings.DefaultPort, Guid.Empty), SlotIdentifier.NoSlotID);
            var y = new SlotIdentifier(new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty), SlotIdentifier.NoSlotID);
            var comparer = SlotIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) | comparer.GetHashCode(x) == comparer.GetHashCode(y);
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
    }
}
