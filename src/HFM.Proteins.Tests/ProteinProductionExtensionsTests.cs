using System;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProteinProductionExtensionsTests
    {
        [Test]
        public void ProteinProductionExtensions_GetUPD_Test()
        {
            var protein = new Protein();
            var frameTime = TimeSpan.FromMinutes(5);
            Assert.AreEqual(2.88, protein.GetUPD(frameTime));
        }

        [Test]
        public void ProteinProductionExtensions_GetBonusMultiplier_Test()
        {
            var protein = new Protein { KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(19.5, protein.GetBonusMultiplier(unitTime), 0.01);
        }

        [Test]
        public void ProteinProductionExtensions_GetBonusCredit_ReturnsCreditWithBonus_Test()
        {
            var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(13648.383, protein.GetBonusCredit(unitTime));
        }

        [Test]
        public void ProteinProductionExtensions_GetPPD_ReturnsPPDWithNoBonus_Test()
        {
            var protein = new Protein { Credit = 500.0 };
            var frameTime = TimeSpan.FromMinutes(5);
            Assert.AreEqual(1440.0, protein.GetPPD(frameTime));
        }

        [Test]
        public void ProteinProductionExtensions_GetBonusPPD_ReturnsPPDWithBonus_Test()
        {
            var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(39307.35, protein.GetBonusPPD(frameTime, unitTime), 0.01);
        }

        [Test]
        public void ProteinProductionExtensions_GetProteinProduction_WithBonus_Test()
        {
            var protein = new Protein { Credit = 700.0, KFactor = 26.4, PreferredDays = 3.0, MaximumDays = 5.0 };
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            var values = protein.GetProteinProduction(frameTime, unitTime);
            Assert.AreEqual(2.88, values.UPD);
            Assert.AreEqual(19.5, values.Multiplier, 0.01);
            Assert.AreEqual(13648.383, values.Credit);
            Assert.AreEqual(39307.35, values.PPD, 0.01);
        }
    }
}
