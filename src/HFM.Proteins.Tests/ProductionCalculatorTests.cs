using System;

using NUnit.Framework;

namespace HFM.Proteins
{
    [TestFixture]
    public class ProductionCalculatorTests
    {
        [Test]
        public void ProductionCalculator_GetUPD_ReturnsZeroWhenFrameTimeIsZero_Test()
        {
            var frameTime = TimeSpan.Zero;
            Assert.AreEqual(0.0, ProductionCalculator.GetUPD(frameTime, 100));
        }

        [Test]
        public void ProductionCalculator_GetUPD_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            Assert.AreEqual(2.88, ProductionCalculator.GetUPD(frameTime, 100));
        }

        [Test]
        public void ProductionCalculator_GetBonusMultiplier_ReturnsOneWhenKFactorIsZero_Test()
        {
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(1.0, ProductionCalculator.GetBonusMultiplier(0, 3.0, 5.0, unitTime), 0.01);
        }

        [Test]
        public void ProductionCalculator_GetBonusMultiplier_ReturnsOneWhenUnitTimeIsZero_Test()
        {
            var unitTime = TimeSpan.Zero;
            Assert.AreEqual(1.0, ProductionCalculator.GetBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
        }

        [Test]
        public void ProductionCalculator_GetBonusMultiplier_ReturnsOneWhenUnitTimeGreaterThanPreferredTime_Test()
        {
            var unitTime = TimeSpan.FromDays(4);
            Assert.AreEqual(1.0, ProductionCalculator.GetBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
        }

        [Test]
        public void ProductionCalculator_GetBonusMultiplier_Test()
        {
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(19.5, ProductionCalculator.GetBonusMultiplier(26.4, 3.0, 5.0, unitTime), 0.01);
        }

        [Test]
        public void ProductionCalculator_GetBonusCredit_ReturnsCreditWithNoBonusWhenUnitTimeIsZero_Test()
        {
            var unitTime = TimeSpan.Zero;
            Assert.AreEqual(700, ProductionCalculator.GetBonusCredit(700, 26.4, 3.0, 5.0, unitTime));
        }

        [Test]
        public void ProductionCalculator_GetBonusCredit_ReturnsCreditWithBonus_Test()
        {
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(13648.383, ProductionCalculator.GetBonusCredit(700, 26.4, 3.0, 5.0, unitTime));
        }

        [Test]
        public void ProductionCalculator_GetPPD_ReturnsZeroWhenFrameTimeIsZero_Test()
        {
            var frameTime = TimeSpan.Zero;
            Assert.AreEqual(0.0, ProductionCalculator.GetPPD(frameTime, 0, 0.0));
        }

        [Test]
        public void ProductionCalculator_GetPPD_ReturnsPPDWithNoBonus_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            Assert.AreEqual(1440.0, ProductionCalculator.GetPPD(frameTime, 100, 500.0));
        }

        [Test]
        public void ProductionCalculator_GetBonusPPD_ReturnsZeroWhenFrameTimeIsZero_Test()
        {
            var frameTime = TimeSpan.Zero;
            var unitTime = TimeSpan.Zero;
            Assert.AreEqual(0.0, ProductionCalculator.GetBonusPPD(frameTime, 0, 0.0, 0.0, 0.0, 0.0, unitTime));
        }

        [Test]
        public void ProductionCalculator_GetBonusPPD_ReturnsPPDWithNoBonusWhenUnitTimeIsZero_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.Zero;
            Assert.AreEqual(1440.0, ProductionCalculator.GetBonusPPD(frameTime, 100, 500.0, 0.0, 0.0, 0.0, unitTime));
        }

        [Test]
        public void ProductionCalculator_GetBonusPPD_ReturnsPPDWithBonus_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            Assert.AreEqual(39307.35, ProductionCalculator.GetBonusPPD(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime), 0.01);
        }

        [Test]
        public void ProductionCalculator_GetProteinProduction_NoBonus_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.Zero;
            var values = ProductionCalculator.GetProteinProduction(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime);
            Assert.AreEqual(2.88, values.UPD);
            Assert.AreEqual(1.0, values.Multiplier);
            Assert.AreEqual(700.0, values.Credit);
            Assert.AreEqual(2016.0, values.PPD);
        }

        [Test]
        public void ProductionCalculator_GetProteinProduction_WithBonus_Test()
        {
            var frameTime = TimeSpan.FromMinutes(5);
            var unitTime = TimeSpan.FromMinutes(5 * 100);
            var values = ProductionCalculator.GetProteinProduction(frameTime, 100, 700.0, 26.4, 3.0, 5.0, unitTime);
            Assert.AreEqual(2.88, values.UPD);
            Assert.AreEqual(19.5, values.Multiplier, 0.01);
            Assert.AreEqual(13648.383, values.Credit);
            Assert.AreEqual(39307.35, values.PPD, 0.01);
        }
    }
}
