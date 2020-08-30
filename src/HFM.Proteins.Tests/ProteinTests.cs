using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinTests
    {
        [Test]
        public void Protein_DefaultPropertyValues()
        {
            var protein = new Protein();
            Assert.AreEqual(0, protein.ProjectNumber);
            Assert.AreEqual(null, protein.ServerIP);
            Assert.AreEqual(null, protein.WorkUnitName);
            Assert.AreEqual(0, protein.NumberOfAtoms);
            Assert.AreEqual(0, protein.PreferredDays);
            Assert.AreEqual(0, protein.MaximumDays);
            Assert.AreEqual(0, protein.Credit);
            Assert.AreEqual(100, protein.Frames);
            Assert.AreEqual(null, protein.Core);
            Assert.AreEqual(null, protein.Description);
            Assert.AreEqual(null, protein.Contact);
            Assert.AreEqual(0, protein.KFactor);
        }

        [Test]
        public void Protein_Copy_ReturnsNewInstance()
        {
            // Arrange
            var protein = new Protein
            {
                ProjectNumber = 1,
                ServerIP = nameof(Protein.ServerIP),
                WorkUnitName = nameof(Protein.WorkUnitName),
                NumberOfAtoms = 2,
                PreferredDays = 3,
                MaximumDays = 4,
                Credit = 5,
                Frames = 6,
                Core = nameof(Protein.Core),
                Description = nameof(Protein.Description),
                Contact = nameof(Protein.Contact),
                KFactor = 7
            };
            // Act
            var copy = protein.Copy();
            // Assert
            Assert.AreNotSame(protein, copy);
            Assert.AreEqual(1, copy.ProjectNumber);
            Assert.AreEqual(nameof(Protein.ServerIP), copy.ServerIP);
            Assert.AreEqual(nameof(Protein.WorkUnitName), copy.WorkUnitName);
            Assert.AreEqual(2, copy.NumberOfAtoms);
            Assert.AreEqual(3, copy.PreferredDays);
            Assert.AreEqual(4, copy.MaximumDays);
            Assert.AreEqual(5, copy.Credit);
            Assert.AreEqual(6, copy.Frames);
            Assert.AreEqual(nameof(Protein.Core), copy.Core);
            Assert.AreEqual(nameof(Protein.Description), copy.Description);
            Assert.AreEqual(nameof(Protein.Contact), copy.Contact);
            Assert.AreEqual(7, copy.KFactor);
        }

        [Test]
        public void Protein_IsValid_ReturnsTrueWhenAllRequiredPropertiesArePopulated_Test()
        {
            var protein = new Protein { ProjectNumber = 1, PreferredDays = 3, MaximumDays = 5, Credit = 500, Frames = 100, KFactor = 26.4 };
            Assert.IsTrue(Protein.IsValid(protein));
        }

        [Test]
        public void Protein_IsValid_ReturnsFalseWhenAllRequiredPropertiesAreNotPopulated_Test()
        {
            var protein = new Protein();
            Assert.IsFalse(Protein.IsValid(protein));
        }

        [Test]
        public void Protein_IsValid_ReturnsFalseWhenProteinIsNull_Test()
        {
            Assert.IsFalse(Protein.IsValid(null));
        }
    }
}
