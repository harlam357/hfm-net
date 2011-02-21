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

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class UnitInfoLogicTests
   {
      private MockRepository _mocks;
      private IProteinBenchmarkContainer _benchmarkContainer;
      private IDisplayInstance _displayInstance;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _benchmarkContainer = _mocks.DynamicMock<IProteinBenchmarkContainer>();
         _displayInstance = _mocks.Stub<IDisplayInstance>();
      }
      
      #region DownloadTime
      
      [Test]
      public void DownloadTimeTest1()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);
         
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime(), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);
         
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);
         
         Assert.AreEqual(unitInfo.DownloadTime,
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest4()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, -60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DownloadTime
                         .Add(TimeSpan.FromMinutes(60)), 
                         unitInfoLogic.DownloadTime);
      }

      [Test]
      public void DownloadTimeTest5()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DownloadTime, 
                         unitInfoLogic.DownloadTime);
      }
      
      #endregion
      
      #region DueTime

      [Test]
      public void DueTimeTest1()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DueTime.ToLocalTime(),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest2()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DueTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest3()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DueTime,
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest4()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, -60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DueTime
                         .Add(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.DueTime);
      }

      [Test]
      public void DueTimeTest5()
      {
         var unitInfo = new UnitInfo { DueTime = DateTime.MinValue };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DueTime,
                         unitInfoLogic.DueTime);
      }
      
      #endregion

      #region FinishedTime

      [Test]
      public void FinishedTimeTest1()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime(),
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest2()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.FinishedTime.ToLocalTime()
                         .Subtract(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest3()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.FinishedTime,
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest4()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(true, -60);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.FinishedTime
                         .Add(TimeSpan.FromMinutes(60)),
                         unitInfoLogic.FinishedTime);
      }

      [Test]
      public void FinishedTimeTest5()
      {
         var unitInfo = new UnitInfo { FinishedTime = DateTime.MinValue };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.FinishedTime,
                         unitInfoLogic.FinishedTime);
      }

      #endregion

      #region PreferredDeadline

      [Test]
      public void PreferredDeadlineTest1()
      {
         var protein = new Protein { ProjectNumber = 1, PreferredDays = 3 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };         
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);
         
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime().AddDays(3), 
                         unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, DueTime = DateTime.UtcNow.Add(TimeSpan.FromDays(5)) };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         // PreferredDeadline comes from DueTime when Protein.IsUnknown
         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime().AddDays(5),
                         unitInfoLogic.PreferredDeadline);
      }

      [Test]
      public void PreferredDeadlineTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DownloadTime,
                         unitInfoLogic.PreferredDeadline);
      }
      
      #endregion

      #region FinalDeadline

      [Test]
      public void FinalDeadlineTest1()
      {
         var protein = new Protein { ProjectNumber = 1, MaxDays = 6 };
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);

         Assert.AreEqual(unitInfo.DownloadTime.ToLocalTime().AddDays(6),
                         unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(DateTime.MinValue,
                         unitInfoLogic.FinalDeadline);
      }

      [Test]
      public void FinalDeadlineTest3()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.MinValue };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(new Protein(), unitInfo, settings);

         Assert.AreEqual(unitInfo.DownloadTime,
                         unitInfoLogic.FinalDeadline);
      }
      
      #endregion
      
      #region CurrentProtein
      
      [Test]
      public void CurrentProteinTest1()
      {
         var protein = new Protein { ProjectNumber = 2669 };
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, new UnitInfo(), settings);

         Assert.AreSame(protein, unitInfoLogic.CurrentProtein);
      }

      [Test]
      public void CurrentProteinTest2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.IsTrue(unitInfoLogic.CurrentProtein.IsUnknown);
      }
      
      #endregion

      #region Frame and Percent Complete

      [Test]
      public void FramesCompleteTest1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(1, new UnitFrame { FrameID = 1 });
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(1, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest2()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(-1, new UnitFrame { FrameID = -1 });
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(0, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest3()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(101, new UnitFrame { FrameID = 101 });
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(100, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void FramesCompleteTest4()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.AreEqual(0, unitInfoLogic.FramesComplete);
      }

      [Test]
      public void PercentCompleteTest1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(5, new UnitFrame { FrameID = 5 });
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(5, unitInfoLogic.PercentComplete);
      }

      [Test]
      public void PercentCompleteTest2()
      {
         var protein = new Protein { Frames = 200 };
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(5, new UnitFrame { FrameID = 5 });
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);

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
         var settings = CreateClientInstanceSettings(false, 0);
         
         _displayInstance.LastRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         _mocks.ReplayAll();
         
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);
         
         Assert.AreEqual(7800, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(7800), unitInfoLogic.TimePerUnitDownload);
         Assert.AreEqual(11.07692, unitInfoLogic.PPDPerUnitDownload);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void PerUnitDownloadTest2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerUnitDownload);
         Assert.AreEqual(0, unitInfoLogic.PPDPerUnitDownload);
      }

      [Test]
      public void PerUnitDownloadTest3()
      {
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerUnitDownload);
         Assert.AreEqual(0, unitInfoLogic.PPDPerUnitDownload);
      }

      [Test]
      public void PerUnitDownloadTest4()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", -1));
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerUnitDownload);
         Assert.AreEqual(0, unitInfoLogic.PPDPerUnitDownload);
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
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);

         Assert.AreEqual(360, unitInfoLogic.RawTimePerAllSections);
         Assert.AreEqual(TimeSpan.FromSeconds(360), unitInfoLogic.TimePerAllSections);
         Assert.AreEqual(240, unitInfoLogic.PPDPerAllSections);
      }

      [Test]
      public void PerAllSectionsTest2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerAllSections);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerAllSections);
         Assert.AreEqual(0, unitInfoLogic.PPDPerAllSections);
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
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);

         Assert.AreEqual(376, unitInfoLogic.RawTimePerThreeSections);
         Assert.AreEqual(TimeSpan.FromSeconds(376), unitInfoLogic.TimePerThreeSections);
         Assert.AreEqual(229.78723, unitInfoLogic.PPDPerThreeSections);
      }

      [Test]
      public void PerThreeSectionsTest2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerThreeSections);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerThreeSections);
         Assert.AreEqual(0, unitInfoLogic.PPDPerThreeSections);
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
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);

         Assert.AreEqual(380, unitInfoLogic.RawTimePerLastSection);
         Assert.AreEqual(TimeSpan.FromSeconds(380), unitInfoLogic.TimePerLastSection);
         Assert.AreEqual(227.36842, unitInfoLogic.PPDPerLastSection);
      }

      [Test]
      public void PerLastSectionTest2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);

         Assert.AreEqual(0, unitInfoLogic.RawTimePerLastSection);
         Assert.AreEqual(TimeSpan.FromSeconds(0), unitInfoLogic.TimePerLastSection);
         Assert.AreEqual(0, unitInfoLogic.PPDPerLastSection);
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
         var settings = CreateClientInstanceSettings(false, 0);

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         /*****/
         Expect.Call(prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
            .Return(PpdCalculationType.LastFrame).Repeat.AtLeastOnce();
         /*****/
         _mocks.ReplayAll();
         
         var unitInfoLogic = CreateUnitInfoLogic(prefs, null, unitInfo, settings);

         Assert.AreEqual(380, unitInfoLogic.RawTimePerSection);
         Assert.AreEqual(TimeSpan.FromSeconds(380), unitInfoLogic.TimePerFrame);
         
         _mocks.VerifyAll();
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
         var settings = CreateClientInstanceSettings(false, 0);

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         /*****/
         Expect.Call(prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
            .Return(PpdCalculationType.LastThreeFrames).Repeat.AtLeastOnce();
         /*****/
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(prefs, null, unitInfo, settings);

         Assert.AreEqual(376, unitInfoLogic.RawTimePerSection);
         Assert.AreEqual(TimeSpan.FromSeconds(376), unitInfoLogic.TimePerFrame);

         _mocks.VerifyAll();
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
         var settings = CreateClientInstanceSettings(false, 0);

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         /*****/
         Expect.Call(prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
            .Return(PpdCalculationType.AllFrames).Repeat.AtLeastOnce();
         /*****/
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(prefs, null, unitInfo, settings);

         Assert.AreEqual(360, unitInfoLogic.RawTimePerSection);
         Assert.AreEqual(TimeSpan.FromSeconds(360), unitInfoLogic.TimePerFrame);

         _mocks.VerifyAll();
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
         var settings = CreateClientInstanceSettings(false, 0);

         _displayInstance.LastRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         /*****/
         Expect.Call(prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
            .Return(PpdCalculationType.EffectiveRate).Repeat.AtLeastOnce();
         /*****/
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(prefs, protein, unitInfo, settings);

         Assert.AreEqual(7800, unitInfoLogic.RawTimePerSection);
         Assert.AreEqual(TimeSpan.FromSeconds(7800), unitInfoLogic.TimePerFrame);

         _mocks.VerifyAll();
      }
      
      [Test]
      public void TimePerSectionTest5()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         Expect.Call(_benchmarkContainer.GetBenchmarkAverageFrameTime(null)).IgnoreArguments()
            .Return(TimeSpan.FromMinutes(10));
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);
         Assert.AreEqual(TimeSpan.FromMinutes(10), unitInfoLogic.TimePerFrame);

         _mocks.VerifyAll();
      }

      [Test]
      public void TimePerSectionTest6()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         _benchmarkContainer = null;
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimePerFrame);
      }

      #endregion

      #region Credit, UPD, PPD

      [Test]
      public void CreditUPDAndPPDTest1()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaxDays = 6 };
         var utcNow = DateTime.UtcNow;
         var unitInfo = new UnitInfo { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var settings = CreateClientInstanceSettings(false, 0);

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.GetPreference<bool>(Preference.CalculateBonus))
            .Return(true).Repeat.AtLeastOnce();
         /*****/
         _displayInstance.Status = ClientStatus.RunningNoFrameTimes;
         /*****/
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(prefs, protein, unitInfo, settings);
         Assert.AreEqual(849, unitInfoLogic.Credit);
         Assert.AreEqual(2.4, unitInfoLogic.UPD);
         Assert.AreEqual(2036.46753, unitInfoLogic.PPD);

         _mocks.VerifyAll();
      }

      [Test]
      public void CreditUPDAndPPDTest2()
      {
         var protein = new Protein { ProjectNumber = 1, Credit = 100, KFactor = 5, PreferredDays = 3, MaxDays = 6};
         var utcNow = DateTime.UtcNow;
         var unitInfo = new UnitInfo { FinishedTime = utcNow, DownloadTime = utcNow.Subtract(TimeSpan.FromHours(2)), FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var settings = CreateClientInstanceSettings(false, 0);

         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.GetPreference<bool>(Preference.CalculateBonus))
            .Return(true).Repeat.AtLeastOnce();
         /*****/
         _displayInstance.Status = ClientStatus.Running;
         /*****/
         _mocks.ReplayAll();

         var unitInfoLogic = CreateUnitInfoLogic(prefs, protein, unitInfo, settings);
         Assert.AreEqual(1897, unitInfoLogic.Credit);
         Assert.AreEqual(2.4, unitInfoLogic.UPD);
         Assert.AreEqual(4553.67983, unitInfoLogic.PPD);

         _mocks.VerifyAll();
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
         var settings = CreateClientInstanceSettings(false, 0);

         var unitInfoLogic = CreateUnitInfoLogic(protein, unitInfo, settings);
         Assert.AreEqual(100, unitInfoLogic.Credit);
         Assert.AreEqual(2.4, unitInfoLogic.UPD);
         Assert.AreEqual(240, unitInfoLogic.PPD);
      }

      [Test]
      public void CreditUPDAndPPDTest4()
      {
         var unitInfo = new UnitInfo { FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);
         Assert.AreEqual(0, unitInfoLogic.Credit);
         Assert.AreEqual(2.4, unitInfoLogic.UPD);
         Assert.AreEqual(0, unitInfoLogic.PPD);
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
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);
         Assert.AreEqual(TimeSpan.FromMinutes(582), unitInfoLogic.ETA);
      }

      [Test]
      public void EtaTest2()
      {
         var unitInfo = new UnitInfo { DownloadTime = DateTime.UtcNow, FramesObserved = 4 };
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 0));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:04:00", 1));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:09:00", 2));
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:15:00", 3));
         var settings = CreateClientInstanceSettings(false, 0);
         
         _displayInstance.LastRetrievalTime = unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(30));
         _mocks.ReplayAll();
         
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);
         Assert.AreEqual(unitInfo.DownloadTime.Add(TimeSpan.FromMinutes(612)), unitInfoLogic.EtaDate);
         
         _mocks.VerifyAll();
      }
      
      #endregion

      #region AllFramesCompleted

      [Test]
      public void AllFramesCompleted1()
      {
         var unitInfo = new UnitInfo();
         unitInfo.SetCurrentFrame(MakeUnitFrame("00:00:00", 100));
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, unitInfo, settings);
         Assert.IsTrue(unitInfoLogic.AllFramesCompleted);
      }

      [Test]
      public void AllFramesCompleted2()
      {
         var settings = CreateClientInstanceSettings(false, 0);
         var unitInfoLogic = CreateUnitInfoLogic(null, new UnitInfo(), settings);
         Assert.IsFalse(unitInfoLogic.AllFramesCompleted);
      }
      
      #endregion

      #region Helpers

      private UnitInfoLogic CreateUnitInfoLogic(IProtein protein, IUnitInfo unitInfo, IClientInstanceSettings settings)
      {
         return CreateUnitInfoLogic(MockRepository.GenerateMock<IPreferenceSet>(), protein, unitInfo, settings);
      }
      
      private UnitInfoLogic CreateUnitInfoLogic(IPreferenceSet prefs, IProtein protein, IUnitInfo unitInfo, IClientInstanceSettings settings)
      {
         return new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, settings, _displayInstance);
      }
      
      private static IClientInstanceSettings CreateClientInstanceSettings(bool utcOffsetIsZero, int clientTimeOffset)
      {
         var settings = new ClientInstanceSettings();
         settings.ClientIsOnVirtualMachine = utcOffsetIsZero;
         settings.ClientTimeOffset = clientTimeOffset;
         settings.InstanceName = "Owner";
         settings.Path = "Path";

         return settings;
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
