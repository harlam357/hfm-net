
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
    }
}
