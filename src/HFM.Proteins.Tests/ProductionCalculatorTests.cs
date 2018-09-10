
using System;

using NUnit.Framework;

namespace HFM.Proteins
{
   [TestFixture]
   public class ProductionCalculatorTests
   {
      [Test]
      public void GetPPD_Test1()
      {
         var protein = new Protein { Credit = 500 };
         Assert.AreEqual(1440.0, ProductionCalculator.GetPPD(TimeSpan.FromMinutes(5), protein));
      }

      [Test]
      public void GetPPD_Test2()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(39307.35, ProductionCalculator.GetPPD(TimeSpan.FromMinutes(5), protein, true), 0.01);
      }

      [Test]
      public void GetPPD_Test3()
      {
         var protein = new Protein();
         Assert.AreEqual(0.0, ProductionCalculator.GetPPD(TimeSpan.Zero, protein));
      }

      [Test]
      public void GetUPD_Test1()
      {
         var protein = new Protein { Credit = 500 };
         Assert.AreEqual(2.88, ProductionCalculator.GetUPD(TimeSpan.FromMinutes(5), protein.Frames));
      }

      [Test]
      public void GetCredit_Test1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(700, ProductionCalculator.GetCredit(protein, TimeSpan.Zero));
      }

      [Test]
      public void GetCredit_Test2()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(13648.383, ProductionCalculator.GetCredit(protein, TimeSpan.FromMinutes(5 * 100)));
      }

      [Test]
      public void GetMultiplier_Test1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(19.5, ProductionCalculator.GetMultiplier(protein, TimeSpan.FromMinutes(5 * 100)), 0.01);
      }

      [Test]
      public void GetProductionValues_Test1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         var values = ProductionCalculator.GetProductionValues(TimeSpan.FromMinutes(5), protein, 
                                                               TimeSpan.FromMinutes(5 * 100).Add(TimeSpan.FromMinutes(10)),
                                                               TimeSpan.FromMinutes(5 * 100));
         Assert.AreEqual(TimeSpan.FromMinutes(5), values.TimePerFrame);
         Assert.AreEqual(700, values.BaseCredit);
         Assert.AreEqual(2016.0, values.BasePPD);
         Assert.AreEqual(TimeSpan.FromDays(3), values.PreferredTime);
         Assert.AreEqual(TimeSpan.FromDays(5), values.MaximumTime);
         Assert.AreEqual(26.4, values.KFactor);
         Assert.AreEqual(TimeSpan.FromMinutes(5 * 100).Add(TimeSpan.FromMinutes(10)), values.UnitTimeByDownloadTime);
         Assert.AreEqual(19.31, values.DownloadTimeBonusMulti, 0.01);
         Assert.AreEqual(13513.913, values.DownloadTimeBonusCredit);
         Assert.AreEqual(38920.07, values.DownloadTimeBonusPPD, 0.01);
         Assert.AreEqual(TimeSpan.FromMinutes(5 * 100), values.UnitTimeByFrameTime);
         Assert.AreEqual(19.5, values.FrameTimeBonusMulti, 0.01);
         Assert.AreEqual(13648.383, values.FrameTimeBonusCredit);
         Assert.AreEqual(39307.35, values.FrameTimeBonusPPD, 0.01);
      }
   }
}
