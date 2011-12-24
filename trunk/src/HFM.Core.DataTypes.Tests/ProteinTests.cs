/*
 * HFM.NET - Protein Class Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;

using NUnit.Framework;

namespace HFM.Core.DataTypes.Tests
{
   [TestFixture]
   public class ProteinTests
   {
      [Test]
      public void DefaultValueTest()
      {
         var protein = new Protein();
         Assert.AreEqual(0, protein.ProjectNumber);
         Assert.AreEqual("0.0.0.0", protein.ServerIP);
         Assert.AreEqual("Unknown", protein.WorkUnitName);
         Assert.AreEqual(0, protein.NumberOfAtoms);
         Assert.AreEqual(0, protein.PreferredDays);
         Assert.AreEqual(0, protein.MaximumDays);
         Assert.AreEqual(0, protein.Credit);
         Assert.AreEqual(100, protein.Frames);
         Assert.AreEqual("Unknown", protein.Core);
         Assert.AreEqual("Unassigned Description", protein.Description);
         Assert.AreEqual("Unknown", protein.Contact);
         Assert.AreEqual(0, protein.KFactor);
      }

      [Test]
      public void GetPPDTest1()
      {
         var protein = new Protein { Credit = 500 };
         Assert.AreEqual(1440.0, protein.GetPPD(TimeSpan.FromMinutes(5)));
      }

      [Test]
      public void GetPPDTest2()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(39307.35, protein.GetPPD(TimeSpan.FromMinutes(5), true), 0.01);
      }

      [Test]
      public void GetPPDTest3()
      {
         var protein = new Protein();
         Assert.AreEqual(0.0, protein.GetPPD(TimeSpan.Zero));
      }

      [Test]
      public void GetUPDTest1()
      {
         var protein = new Protein { Credit = 500 };
         Assert.AreEqual(2.88, protein.GetUPD(TimeSpan.FromMinutes(5)));
      }

      [Test]
      public void GetCreditTest1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(700, protein.GetCredit(TimeSpan.FromMinutes(5 * 100), false));
      }

      [Test]
      public void GetCreditTest2()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(13648.0, protein.GetCredit(TimeSpan.FromMinutes(5 * 100), true));
      }

      [Test]
      public void GetMultiplierTest1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         Assert.AreEqual(19.5, protein.GetMultiplier(TimeSpan.FromMinutes(5 * 100), true), 0.01);
      }

      [Test]
      public void GetProductionValuesTest1()
      {
         var protein = new Protein { Credit = 700, PreferredDays = 3, MaximumDays = 5, KFactor = 26.4 };
         var values = protein.GetProductionValues(TimeSpan.FromMinutes(5), 
                                                  TimeSpan.FromMinutes(5 * 100).Add(TimeSpan.FromMinutes(10)), 
                                                  TimeSpan.FromMinutes(5 * 100), true);
         Assert.AreEqual(TimeSpan.FromMinutes(5), values.TimePerFrame);
         Assert.AreEqual(700, values.BaseCredit);
         Assert.AreEqual(2016.0, values.BasePPD);
         Assert.AreEqual(TimeSpan.FromDays(3), values.PreferredTime);
         Assert.AreEqual(TimeSpan.FromDays(5), values.MaximumTime);
         Assert.AreEqual(26.4, values.KFactor);
         Assert.AreEqual(TimeSpan.FromMinutes(5 * 100).Add(TimeSpan.FromMinutes(10)), values.EftByDownloadTime);
         Assert.AreEqual(19.31, values.DownloadTimeBonusMulti, 0.01);
         Assert.AreEqual(13514.0, values.DownloadTimeBonusCredit);
         Assert.AreEqual(38920.07, values.DownloadTimeBonusPPD, 0.01);
         Assert.AreEqual(TimeSpan.FromMinutes(5 * 100), values.EftByFrameTime);
         Assert.AreEqual(19.5, values.FrameTimeBonusMulti, 0.01);
         Assert.AreEqual(13648.0, values.FrameTimeBonusCredit);
         Assert.AreEqual(39307.35, values.FrameTimeBonusPPD, 0.01);
      }
   }
}
