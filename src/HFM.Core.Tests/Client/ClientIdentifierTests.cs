
using System;

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
            var x = new ClientIdentifier("Foo", "Bar", 36330, guid);
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
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.NewGuid());
            var y = new ClientIdentifier("Foo", "Bar", 36330, Guid.NewGuid());
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenOnlyOneHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.Empty);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_AreEqualWhenNameServerPortAreEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
            var y = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_AreNotEqualWhenNameServerPortAreNotEqualWithNoGuid()
        {
            // Arrange
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
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
            var x = new ClientIdentifier("Foo", "Bar", 36330, guid);
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
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.NewGuid());
            var y = new ClientIdentifier("Foo", "Bar", 36330, Guid.NewGuid());
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) & comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ClientIdentifier_ProteinBenchmarkEqualityComparer_AreEqualWhenOnlyOneHasGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.Empty);
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
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
            var y = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
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
            var x = new ClientIdentifier("Foo", "Bar", 36330, Guid.Empty);
            var y = new ClientIdentifier("Fizz", "Bizz", 46330, Guid.Empty);
            var comparer = ClientIdentifier.ProteinBenchmarkEqualityComparer;
            // Act
            var result = comparer.Equals(x, y) | comparer.GetHashCode(x) == comparer.GetHashCode(y);
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ClientIdentifier_WithGuidIsLessThanWithoutGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.Empty);
            // Act
            var result = x.CompareTo(y);
            // Assert
            Assert.AreEqual(-1, result);
        }

        [Test]
        public void ClientIdentifier_WithoutGuidIsGreaterThanWithGuid()
        {
            // Arrange
            var x = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.NewGuid());
            var y = new ClientIdentifier(null, null, ClientIdentifier.NoPort, Guid.Empty);
            // Act
            var result = y.CompareTo(x);
            // Assert
            Assert.AreEqual(1, result);
        }
    }
}
