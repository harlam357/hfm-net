/*
 * HFM.NET - Unit Info Logic Class Tests
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
using System.Globalization;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class UnitInfoLogicTests
   {
      private IProteinBenchmarkCollection _benchmarkCollection;

      [SetUp]
      public void Init()
      {
         _benchmarkCollection = MockRepository.GenerateStub<IProteinBenchmarkCollection>();
      }
      
      #region DownloadTime
      
      [Test]
      public void DownloadTimeTest1()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;
         
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime(), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 60;
         
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = 0;
         
         Assert.AreEqual(unitInfo.DownloadTime,
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest4()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = -60;

         Assert.AreEqual(unitInfo.DownloadTime
                         .Add(TimeSpan.FromMinutes(60)), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest5()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime, 
                         unitInfoLogic.DownloadTime);
      }
      
      #endregion
      
      #region DueTime

      [Test]
      public void DueTimeTest1()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DueTime.ToLocalTime(),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest2()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 60;

         Assert.AreEqual(unitInfo.DueTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest3()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DueTime,
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest4()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = -60;

         Assert.AreEqual(unitInfo.DueTime
                         .Add(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest5()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.MinValue };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DueTime,
                         unitInfoLogic.DueTime);
      }
      
      #endregion

      #region FinishedTime

      [Test]
      public void FinishedTimeTest1()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime(),
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest2()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 60;

         Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest3()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.FinishedTime, unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest4()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = -60;

         Assert.AreEqual(unitInfo.FinishedTime.Add(TimeSpan.FromMinutes(60)), 
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest5()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.MinValue };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.FinishedTime, unitInfoLogic.FinishedTime);
      }

      #endregion

      #region PreferredDeadline

      [Test]
      public void PreferredDeadlineTest1()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };         
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;
         
         Assert.AreEqual(unitInfo.DownloadTime.AddDays(3).ToLocalTime(), 
                         unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, DueTime = DateTime.UtcNow.Add(TimeSpan.FromDays(5)) };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         // PreferredDeadline comes from DueTime when Protein.IsUnknown
         Assert.AreEqual(unitInfo.DownloadTime.AddDays(5).ToLocalTime(),
                         unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime, unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest4()
      {
         // daylight savings time test (in DST => Standard Time)
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 7 };
         var unitInfo = new UnitInfo { DownloadTime = new DateTime(2011, 11, 1) };
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(),
                         unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest5()
      {
         // daylight savings time test (in Standard Time => DST)
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 7 };
         var unitInfo = new UnitInfo { DownloadTime = new DateTime(2011, 3, 9) };
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(),
                         unitInfoLogic.PreferredDeadline);
      }
      
      #endregion

      #region FinalDeadline

      [Test]
      public void FinalDeadlineTest1()
      {
         var protein = new Protein { ProjectNumber = 1, MaximumDays = 6 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.AddDays(6).ToLocalTime(),
                         unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(DateTime.MinValue, unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime, unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest4()
      {
         // daylight savings time test (in DST => Standard Time)
         var protein = new Protein { ProjectNumber = 1, MaximumDays = 7 };
         var unitInfo = new UnitInfo { DownloadTime = new DateTime(2011, 11, 1) };
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(),
                         unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest5()
      {
         // daylight savings time test (in Standard Time => DST)
         var protein = new Protein { ProjectNumber = 1, MaximumDays = 7 };
         var unitInfo = new UnitInfo { DownloadTime = new DateTime(2011, 3, 9) };
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.AddDays(7).ToLocalTime(),
                         unitInfoLogic.FinalDeadline);
      }
      
      #endregion
      
      #region CurrentProtein
      
      [Test]
      public void CurrentProteinTest1()
      {
         var protein = new Protein { ProjectNumber = 2669 };
         var unitInfoLogic = CreateUnitInfoLogic(protein, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreSame(protein, unitInfoLogic.CurrentProtein);
      }

      [Test]
      public void CurrentProteinTest2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.IsTrue(unitInfoLogic.CurrentProtein.IsUnknown());
      }
      
      #endregion

      #region Frame and Percent Complete

      [Test]
      public void FramesCompleteTest1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(1, new UnitFrame { FrameID = 1 });
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(1, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest2()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(-1, new UnitFrame { FrameID = -1 });
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest3()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(101, new UnitFrame { FrameID = 101 });
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(100, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest4()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void PercentCompleteTest1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(5, new UnitFrame { FrameID = 5 });
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(5, unitInfoLogic.PercentComplete);
      }

      [Test]
      public void PercentCompleteTest2()
      {
         var protein = new Protein { Frames = 200 };
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(5, new UnitFrame { FrameID = 5 });
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(2, unitInfoLogic.PercentComplete);
      }
      
      #endregion

      #region PerSectionTests

      [Test]
      public void PerUnitDownloadTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(600, unitInfoLogic.GetRawTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(TimeSpan.FromSeconds(600), unitInfoLogic.GetFrameTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(144, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.EffectiveRate, false));
      }

      [Test]
      public void PerUnitDownloadTest2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.EffectiveRate, false));
      }

      [Test]
      public void PerUnitDownloadTest3()
      {
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.EffectiveRate, false));
      }

      [Test]
      public void PerUnitDownloadTest4()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", -1));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.EffectiveRate, false));
      }
      
      [Test]
      public void PerAllSectionsTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(360, unitInfoLogic.GetRawTime(PpdCalculationType.AllFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(360), unitInfoLogic.GetFrameTime(PpdCalculationType.AllFrames));
         Assert.AreEqual(240, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.AllFrames, false));
      }

      [Test]
      public void PerAllSectionsTest2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.AllFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.AllFrames));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.AllFrames, false));
      }

      [Test]
      public void PerThreeSectionsTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(376, unitInfoLogic.GetRawTime(PpdCalculationType.LastThreeFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(376), unitInfoLogic.GetFrameTime(PpdCalculationType.LastThreeFrames));
         Assert.AreEqual(229.78723, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastThreeFrames, false));
      }

      [Test]
      public void PerThreeSectionsTest2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.LastThreeFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.LastThreeFrames));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastThreeFrames, false));
      }

      [Test]
      public void PerLastSectionTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(380, unitInfoLogic.GetRawTime(PpdCalculationType.LastFrame));
         Assert.AreEqual(TimeSpan.FromSeconds(380), unitInfoLogic.GetFrameTime(PpdCalculationType.LastFrame));
         Assert.AreEqual(227.36842, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
      }

      [Test]
      public void PerLastSectionTest2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetRawTime(PpdCalculationType.LastFrame));
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.GetFrameTime(PpdCalculationType.LastFrame));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
      }
      
      [Test]
      public void TimePerSectionTest1()
      {
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;
         
         Assert.AreEqual(380, unitInfoLogic.GetRawTime(PpdCalculationType.LastFrame));
         Assert.AreEqual(TimeSpan.FromSeconds(380), unitInfoLogic.GetFrameTime(PpdCalculationType.LastFrame));
      }

      [Test]
      public void TimePerSectionTest2()
      {
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(376, unitInfoLogic.GetRawTime(PpdCalculationType.LastThreeFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(376), unitInfoLogic.GetFrameTime(PpdCalculationType.LastThreeFrames));
      }

      [Test]
      public void TimePerSectionTest3()
      {
         var unitInfo = new UnitInfo { FramesObserved = 5 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:05:10", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:11:30", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:17:40", 3));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:24:00", 4));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(360, unitInfoLogic.GetRawTime(PpdCalculationType.AllFrames));
         Assert.AreEqual(TimeSpan.FromSeconds(360), unitInfoLogic.GetFrameTime(PpdCalculationType.AllFrames));
      }

      [Test]
      public void TimePerSectionTest4()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = true;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(600, unitInfoLogic.GetRawTime(PpdCalculationType.EffectiveRate));
         Assert.AreEqual(TimeSpan.FromSeconds(600), unitInfoLogic.GetFrameTime(PpdCalculationType.EffectiveRate));
      }
      
      [Test]
      public void TimePerSectionTest5()
      {
         _benchmarkCollection.Stub(x => x.GetBenchmark(null)).IgnoreArguments().Return(new ProteinBenchmark { FrameTimes = { new ProteinFrameTime { Duration = TimeSpan.FromMinutes(10) } } });
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;
         Assert.AreEqual(TimeSpan.FromMinutes(10), unitInfoLogic.GetFrameTime(PpdCalculationType.LastFrame));
      }

      [Test]
      public void TimePerSectionTest6()
      {
         _benchmarkCollection = null;
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.GetFrameTime(PpdCalculationType.LastFrame));
      }

      #endregion

      #region Credit, UPD, PPD

      [Test]
      public void CreditUPDAndPPDTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6 };
         var utcNow = DateTime.UtcNow;
         var unitInfo = new UnitInfo { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(849, unitInfoLogic.GetCredit(ClientStatus.RunningNoFrameTimes, PpdCalculationType.LastFrame, true));
         Assert.AreEqual(2.4, unitInfoLogic.GetUPD(PpdCalculationType.LastFrame));
         Assert.AreEqual(2036.46753, unitInfoLogic.GetPPD(ClientStatus.RunningNoFrameTimes, PpdCalculationType.LastFrame, true));
      }

      [Test]
      public void CreditUPDAndPPDTest2()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaximumDays = 6};
         var utcNow = DateTime.UtcNow;
         var unitInfo = new UnitInfo { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(1897, unitInfoLogic.GetCredit(ClientStatus.Running, PpdCalculationType.LastFrame, true));
         Assert.AreEqual(2.4, unitInfoLogic.GetUPD(PpdCalculationType.LastFrame));
         Assert.AreEqual(4553.67983, unitInfoLogic.GetPPD(ClientStatus.Running, PpdCalculationType.LastFrame, true));
      }

      [Test]
      public void CreditUPDAndPPDTest3()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100 };
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(100, unitInfoLogic.GetCredit(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
         Assert.AreEqual(2.4, unitInfoLogic.GetUPD(PpdCalculationType.LastFrame));
         Assert.AreEqual(240, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
      }

      [Test]
      public void CreditUPDAndPPDTest4()
      {
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(0, unitInfoLogic.GetCredit(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
         Assert.AreEqual(2.4, unitInfoLogic.GetUPD(PpdCalculationType.LastFrame));
         Assert.AreEqual(0, unitInfoLogic.GetPPD(ClientStatus.Unknown, PpdCalculationType.LastFrame, false));
      }
      
      #endregion
      
      #region ETA
      
      [Test]
      public void EtaTest1()
      {
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(TimeSpan.FromMinutes(582), unitInfoLogic.GetEta(PpdCalculationType.LastFrame));
      }

      [Test]
      public void EtaTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         unitInfo.UnitRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.AreEqual(unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(612)), unitInfoLogic.GetEtaDate(PpdCalculationType.LastFrame));
      }
      
      #endregion

      #region AllFramesCompleted

      [Test]
      public void AllFramesCompleted1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 100));
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo);
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.IsTrue(unitInfoLogic.AllFramesCompleted);
      }

      [Test]
      public void AllFramesCompleted2()
      {
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo());
         unitInfoLogic.UtcOffsetIsZero = false;
         unitInfoLogic.ClientTimeOffset = 0;

         Assert.IsFalse(unitInfoLogic.AllFramesCompleted);
      }
      
      #endregion

      #region Helpers
      
      private UnitInfoLogic CreateUnitInfoLogic(Protein protein, UnitInfo unitInfo)
      {
         return new UnitInfoLogic(protein, _benchmarkCollection, unitInfo);
      }
      
      private static UnitFrame MakeUnitFrame(string timeStamp, int frameId)
      {
         return new UnitFrame
                {
                   FrameID = frameId,
                   TimeOfFrame = ParseTimeStamp(timeStamp),
                };
      }

      private static TimeSpan ParseTimeStamp(string timeStamp)
      {
         return DateTime.ParseExact(timeStamp, "HH:mm:ss",
                                    DateTimeFormatInfo.InvariantInfo,
                                    Default.DateTimeStyle).TimeOfDay;
      }
      
      #endregion
   }
}
