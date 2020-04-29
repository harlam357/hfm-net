
using NUnit.Framework;

namespace HFM.Core.WorkUnits
{
    [TestFixture]
    public class ProteinBenchmarkIdentifierTests
    {
        [Test]
        public void ProteinBenchmarkIdentifier_AreEqualWhenProjectIDAndProcessorAndThreadsAreEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, "i9", 8);
            var y = new ProteinBenchmarkIdentifier(1234, "i9", 8);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProteinBenchmarkIdentifier_AreNotEqualWhenProjectIDAndProcessorAndThreadsAreNotEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, "i9", 8);
            var y = new ProteinBenchmarkIdentifier(5678, "nvidia", 2);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ProteinBenchmarkIdentifier_AreEqualWhenProjectIDAndProcessorAreEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, "i9", ProteinBenchmarkIdentifier.NoThreads);
            var y = new ProteinBenchmarkIdentifier(1234, "i9", ProteinBenchmarkIdentifier.NoThreads);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProteinBenchmarkIdentifier_AreNotEqualWhenProjectIDAndProcessorAreNotEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, "i9", ProteinBenchmarkIdentifier.NoThreads);
            var y = new ProteinBenchmarkIdentifier(5678, "nvidia", ProteinBenchmarkIdentifier.NoThreads);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void ProteinBenchmarkIdentifier_AreEqualWhenProjectIDAreEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, ProteinBenchmarkIdentifier.NoProcessor, ProteinBenchmarkIdentifier.NoThreads);
            var y = new ProteinBenchmarkIdentifier(1234, ProteinBenchmarkIdentifier.NoProcessor, ProteinBenchmarkIdentifier.NoThreads);
            // Act
            var result = x.Equals(y) & x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProteinBenchmarkIdentifier_AreNotEqualWhenProjectIDAreNotEqual()
        {
            // Arrange
            var x = new ProteinBenchmarkIdentifier(1234, ProteinBenchmarkIdentifier.NoProcessor, ProteinBenchmarkIdentifier.NoThreads);
            var y = new ProteinBenchmarkIdentifier(5678, ProteinBenchmarkIdentifier.NoProcessor, ProteinBenchmarkIdentifier.NoThreads);
            // Act
            var result = x.Equals(y) | x.GetHashCode() == y.GetHashCode();
            // Assert
            Assert.IsFalse(result);
        }
    }
}
