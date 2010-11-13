/*
 * HFM.NET - Unit Info Logic Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
   
      [Test]
      public void UnitInfoLogicTimePropertyTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 3, 6);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(false, 0);
         _mocks.ReplayAll();

         var unitInfo = new UnitInfo();
         SetProject(unitInfo);
         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         SetDateTimeProperties(unitInfo);
         AssertDateTimeProperties(unitInfoLogic, unitInfo.DownloadTime.ToLocalTime(),
                                                 unitInfo.DownloadTime.ToLocalTime().AddDays(protein.PreferredDays),
                                                 unitInfo.DownloadTime.ToLocalTime().AddDays(protein.MaxDays),
                                                 unitInfo.DueTime.ToLocalTime(),
                                                 unitInfo.FinishedTime.ToLocalTime());
         _mocks.VerifyAll();            
      }

      [Test]
      public void UnitInfoLogicTimePropertyUtcZeroTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 4, 8);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(true, 0);
         _mocks.ReplayAll();

         var unitInfo = new UnitInfo();
         SetProject(unitInfo);
         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         SetDateTimeProperties(unitInfo);
         AssertDateTimeProperties(unitInfoLogic, unitInfo.DownloadTime,
                                                 unitInfo.DownloadTime.AddDays(protein.PreferredDays),
                                                 unitInfo.DownloadTime.AddDays(protein.MaxDays),
                                                 unitInfo.DueTime,
                                                 unitInfo.FinishedTime);
         _mocks.VerifyAll();
      }

      [Test]
      public void UnitInfoLogicTimePropertyWithUnknownProteinTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockNewProtein();
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(false, 0);
         _mocks.ReplayAll();

         var unitInfo = new UnitInfo();
         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         SetDateTimeProperties(unitInfo);
         AssertDateTimeProperties(unitInfoLogic, unitInfo.DownloadTime.ToLocalTime(),
                                                 unitInfo.DueTime.ToLocalTime(), 
                                                 DateTime.MinValue,
                                                 unitInfo.DueTime.ToLocalTime(),
                                                 unitInfo.FinishedTime.ToLocalTime());
         _mocks.VerifyAll();
      }

      private static void SetProject(UnitInfo unitInfo)
      {
         unitInfo.ProjectID = 2669;
         unitInfo.ProjectRun = 1;
         unitInfo.ProjectClone = 2;
         unitInfo.ProjectGen = 3;
      }
      
      private static void SetDateTimeProperties(UnitInfo unitInfo)
      {
         unitInfo.DownloadTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         unitInfo.DueTime = new DateTime(2010, 1, 6, 0, 0, 0, DateTimeKind.Utc);
         unitInfo.FinishedTime = new DateTime(2010, 1, 1, 4, 30, 0, DateTimeKind.Utc);
      }
      
      private static void AssertDateTimeProperties(UnitInfoLogic unitInfoLogic, DateTime downloadTime, 
                                                   DateTime preferredDeadline, DateTime finalDeadline, 
                                                   DateTime dueTime, DateTime finishedTime)
      {
         // Unit Info Logic Values
         Assert.AreEqual(false, unitInfoLogic.UnitInfoData.DownloadTimeUnknown);
         Assert.AreEqual(downloadTime, unitInfoLogic.DownloadTime);
         Assert.AreEqual(preferredDeadline, unitInfoLogic.PreferredDeadline);
         Assert.AreEqual(finalDeadline, unitInfoLogic.FinalDeadline);
         Assert.AreEqual(false, unitInfoLogic.UnitInfoData.DueTimeUnknown);
         Assert.AreEqual(dueTime, unitInfoLogic.DueTime);
         Assert.AreEqual(finishedTime, unitInfoLogic.FinishedTime);

         if (unitInfoLogic.CurrentProtein.IsUnknown)
         {
            // use DueTime for PreferredDeadline when CurrentProtein.IsUnknown
            Assert.AreEqual(unitInfoLogic.DueTime, unitInfoLogic.PreferredDeadline);
         }
      }

      [Test]
      public void UnitInfoLogicFramePropertyTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 3, 6);
         var baseDate = new DateTime(2010, 1, 1);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(false, 0, "Owner", "Path");
         _displayInstance.LastRetrievalTime = baseDate.Add(TimeSpan.FromMinutes(30));

         var unitInfo = new UnitInfo();
         UnitFrame line1 = MakeUnitFrame("00:00:00", 0, 0, 250000);
         UnitFrame line2 = MakeUnitFrame("00:04:00", 1, 2500, 250000);
         UnitFrame line3 = MakeUnitFrame("00:09:00", 2, 5000, 250000);
         UnitFrame line4 = MakeUnitFrame("00:15:00", 3, 7500, 250000);

         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(100);
         _mocks.ReplayAll();

         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 4;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);

         Assert.AreEqual(3, unitInfoLogic.FramesComplete);
         Assert.AreEqual(3, unitInfoLogic.PercentComplete);
         Assert.AreEqual(new TimeSpan(0, 5, 0), unitInfoLogic.TimePerFrame);
         // UPD
         Assert.AreEqual(100, unitInfoLogic.PPD);
         Assert.AreEqual(new TimeSpan(8, 5, 0), unitInfoLogic.ETA);
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.EftByDownloadTime);
         Assert.AreEqual(new TimeSpan(8, 20, 00), unitInfoLogic.EftByFrameTime);
         unitInfo.DownloadTime = baseDate;
         Assert.AreEqual(new TimeSpan(14, 35, 00), unitInfoLogic.EftByDownloadTime);
         Assert.AreEqual(TimeSpan.FromMinutes(15), unitInfoLogic.TimeOfLastFrame);
         Assert.AreEqual(3, unitInfoLogic.LastUnitFrameID);

         AssertTimeVariations(unitInfoLogic, 7800, 300, 300, 360);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void UnitInfoLogicFramePropertyUtcZeroTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 3, 6);
         var baseDate = new DateTime(2010, 1, 1);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(true, 0, "Owner", "Path");
         _displayInstance.LastRetrievalTime = baseDate.Add(TimeSpan.FromMinutes(90));

         var unitInfo = new UnitInfo();
         UnitFrame line1 = MakeUnitFrame("00:00:00", 0, 0, 100);
         UnitFrame line2 = MakeUnitFrame("00:05:10", 1, 1, 100);
         UnitFrame line3 = MakeUnitFrame("00:11:30", 2, 2, 100);
         UnitFrame line4 = MakeUnitFrame("00:17:40", 3, 3, 100);
         UnitFrame line5 = MakeUnitFrame("00:24:00", 4, 4, 100);

         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(100);
         _mocks.ReplayAll();

         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 5;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);
         unitInfo.SetCurrentFrame(line5);

         Assert.AreEqual(4, unitInfoLogic.FramesComplete);
         Assert.AreEqual(4, unitInfoLogic.PercentComplete);
         Assert.AreEqual(new TimeSpan(0, 6, 0), unitInfoLogic.TimePerFrame);
         // UPD
         Assert.AreEqual(100, unitInfoLogic.PPD);
         Assert.AreEqual(new TimeSpan(9, 36, 0), unitInfoLogic.ETA);
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.EftByDownloadTime);
         Assert.AreEqual(new TimeSpan(10, 00, 00), unitInfoLogic.EftByFrameTime);
         unitInfo.DownloadTime = baseDate;
         Assert.AreEqual(new TimeSpan(11, 6, 00), unitInfoLogic.EftByDownloadTime);
         Assert.AreEqual(TimeSpan.FromMinutes(24), unitInfoLogic.TimeOfLastFrame);
         Assert.AreEqual(4, unitInfoLogic.LastUnitFrameID);

         AssertTimeVariations(unitInfoLogic, 1350, 360, 376, 380);

         _mocks.VerifyAll();
      }
      
      [Test]
      public void UnitInfoLogicFramesAndPpdTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 3, 6);
         var baseDate = new DateTime(2010, 1, 1);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(true, 0, "Owner", "Path");
         _displayInstance.LastRetrievalTime = baseDate.Add(TimeSpan.FromMinutes(90));

         var unitInfo = new UnitInfo();
         UnitFrame line1 = MakeUnitFrame("00:00:00", 0, 0, 100);
         UnitFrame line2 = MakeUnitFrame("00:05:10", 1, 1, 100);
         UnitFrame line3 = MakeUnitFrame("00:11:30", 2, 2, 100);
         UnitFrame line4 = MakeUnitFrame("00:17:40", 3, 3, 100);
         UnitFrame line5 = MakeUnitFrame("00:24:00", 4, 4, 100);

         Expect.Call(protein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(100);
         _mocks.ReplayAll();

         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         AssertRawTimesZero(unitInfoLogic);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 5;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);
         unitInfo.SetCurrentFrame(line5);

         Assert.AreEqual(100, unitInfoLogic.PPD);
         unitInfo.DownloadTime = baseDate;
         AssertTimeVariations(unitInfoLogic, 1350, 360, 376, 380);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void UnitInfoLogicFramesAndBonusPpdTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProtein protein = SetupMockProtein("GROCVS", 100, 3, 6);
         var baseDate = new DateTime(2010, 1, 1);
         IClientInstanceSettings clientInstance = SetupMockClientInstanceSettings(true, 0, "Owner", "Path");
         _displayInstance.LastRetrievalTime = baseDate.Add(TimeSpan.FromMinutes(90));

         var unitInfo = new UnitInfo();
         UnitFrame line1 = MakeUnitFrame("00:00:00", 0, 0, 100);
         UnitFrame line2 = MakeUnitFrame("00:05:10", 1, 1, 100);
         UnitFrame line3 = MakeUnitFrame("00:11:30", 2, 2, 100);
         UnitFrame line4 = MakeUnitFrame("00:17:40", 3, 3, 100);
         UnitFrame line5 = MakeUnitFrame("00:24:00", 4, 4, 100);

         Expect.Call(prefs.GetPreference<bool>(Preference.CalculateBonus)).Return(true);
         Expect.Call(protein.GetPPD(TimeSpan.Zero, TimeSpan.Zero)).IgnoreArguments().Return(200);
         _mocks.ReplayAll();

         var unitInfoLogic = new UnitInfoLogic(prefs, protein, _benchmarkContainer, unitInfo, clientInstance, _displayInstance);

         AssertRawTimesZero(unitInfoLogic);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 5;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);
         unitInfo.SetCurrentFrame(line5);

         Assert.AreEqual(200, unitInfoLogic.PPD);
         unitInfo.DownloadTime = baseDate;
         AssertTimeVariations(unitInfoLogic, 1350, 360, 376, 380);

         _mocks.VerifyAll();
      }

      private IProtein SetupMockProtein(string core, int frames, int preferredDays, int maxDays)
      {
         var currentProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(currentProtein.Core).Return(core).Repeat.Any();
         Expect.Call(currentProtein.Frames).Return(frames).Repeat.Any();
         Expect.Call(currentProtein.PreferredDays).Return(preferredDays).Repeat.Any();
         Expect.Call(currentProtein.MaxDays).Return(maxDays).Repeat.Any();

         return currentProtein;
      }
      
      private IProtein SetupMockNewProtein()
      {
         var newProtein = _mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Core).Return(String.Empty).Repeat.Any();
         Expect.Call(newProtein.Frames).Return(100).Repeat.Any();
         Expect.Call(newProtein.IsUnknown).Return(true).Repeat.Any();

         return newProtein;
      }
      
      private static void AssertRawTimesZero(IUnitInfoLogic unitInfoLogic)
      {
         Assert.AreEqual(0, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(0, unitInfoLogic.RawTimePerAllSections);
         Assert.AreEqual(0, unitInfoLogic.RawTimePerThreeSections);
         Assert.AreEqual(0, unitInfoLogic.RawTimePerLastSection);
      }

      private IPreferenceSet SetupMockPreferenceSet()
      {
         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation)).Return(PpdCalculationType.AllFrames).Repeat.Any();
         return prefs;
      }
      
      private IClientInstanceSettings SetupMockClientInstanceSettings(bool utcOffsetIsZero, int clientTimeOffset)
      {
         return SetupMockClientInstanceSettings(utcOffsetIsZero, clientTimeOffset, "Owner", "Path");
      }

      private IClientInstanceSettings SetupMockClientInstanceSettings(bool utcOffsetIsZero, int clientTimeOffset, string instanceName, string path)
      {
         var settings = _mocks.Stub<IClientInstanceSettings>();
         settings.ClientIsOnVirtualMachine = utcOffsetIsZero;
         settings.ClientTimeOffset = clientTimeOffset;
         settings.InstanceName = instanceName;
         settings.Path = path;

         return settings;
      }

      public UnitFrame MakeUnitFrame(string timeStampString, int frameId, int complete, int total)
      {
         return new UnitFrame
                {
                   FrameID = frameId,
                   TimeStampString = timeStampString,
                   RawFramesComplete = complete,
                   RawFramesTotal = total
                };
      }

      private static void AssertTimeVariations(IUnitInfoLogic unitInfoLogic, int unitDownload, int allSections,
                                                                             int threeSections, int lastSection)
      {
         Assert.AreEqual(unitDownload, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerUnitDownload), unitInfoLogic.TimePerUnitDownload);

         Assert.AreEqual(allSections, unitInfoLogic.RawTimePerAllSections);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerAllSections), unitInfoLogic.TimePerAllSections);

         Assert.AreEqual(threeSections, unitInfoLogic.RawTimePerThreeSections);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerThreeSections), unitInfoLogic.TimePerThreeSections);

         Assert.AreEqual(lastSection, unitInfoLogic.RawTimePerLastSection);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerLastSection), unitInfoLogic.TimePerLastSection);
      }
   }
}
