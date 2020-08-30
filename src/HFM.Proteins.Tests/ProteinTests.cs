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
