using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinProductionTests
    {
        [Test]
        public void ProteinProduction_EqualsProteinProduction_ReturnsTrue()
        {
            // Arrange
            var production = new ProteinProduction(1.0, 2.0, 3.0, 4.0);
            var other = production;
            // Act
            var result = production.Equals(other);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(production.GetHashCode(), other.GetHashCode());
            Assert.IsTrue(production == other);
            Assert.IsFalse(production != other);
        }

        [Test]
        public void ProteinProduction_EqualsProteinProduction_ReturnsFalse()
        {
            // Arrange
            var production = new ProteinProduction(1.0, 2.0, 3.0, 4.0);
            var other = new ProteinProduction(2.0, 3.0, 4.0, 5.0);
            // Act
            var result = production.Equals(other);
            // Assert
            Assert.IsFalse(result);
            Assert.AreNotEqual(production.GetHashCode(), other.GetHashCode());
            Assert.IsFalse(production == other);
            Assert.IsTrue(production != other);
        }

        [Test]
        public void ProteinProduction_EqualsObject_ReturnsTrue()
        {
            // Arrange
            var production = new ProteinProduction(1.0, 2.0, 3.0, 4.0);
            object other = production;
            // Act
            var result = production.Equals(other);
            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void ProteinProduction_EqualsObject_ReturnsFalse()
        {
            // Arrange
            var production = new ProteinProduction(1.0, 2.0, 3.0, 4.0);
            object other = new ProteinProduction(2.0, 3.0, 4.0, 5.0);
            // Act
            var result = production.Equals(other);
            // Assert
            Assert.IsFalse(result);
        }
    }
}
