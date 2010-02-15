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
using System.Globalization;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class UnitInfoLogicTests
   {
      private MockRepository mocks;
      private IProteinBenchmarkContainer benchmarkCollection;

      [SetUp]
      public void Init()
      {
         mocks = new MockRepository();
         benchmarkCollection = mocks.DynamicMock<IProteinBenchmarkContainer>();
      }
   
      [Test]
      public void UnitInfoLogicTimePropertyTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 3, 6);
         IPreferenceSet Prefs = SetupMockPreferenceSet();
         mocks.ReplayAll();
      
         UnitInfo unitInfo = new UnitInfo();
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection, 
            unitInfo, false, "Owner", "Path", DateTime.Now);

         SetDateTimeProperties(unitInfo);
         AssertDateTimeProperties(unitInfoLogic, unitInfo.DownloadTime.ToLocalTime(),
            unitInfo.DueTime.ToLocalTime(), unitInfo.FinishedTime.ToLocalTime());
         SetAndAssertProject(unitInfoLogic);
         AssertDeadlines(unitInfoLogic, new DateTime(2010, 1, 3, 18, 0, 0), new DateTime(2010, 1, 6, 18, 0, 0));

         mocks.VerifyAll();            
      }

      [Test]
      public void UnitInfoLogicTimePropertyUtcZeroTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 4, 8);
         IPreferenceSet Prefs = SetupMockPreferenceSet();
         mocks.ReplayAll();

         UnitInfo unitInfo = new UnitInfo();
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection,
            unitInfo, true, "Owner", "Path", DateTime.Now);

         SetDateTimeProperties(unitInfo);
         AssertDateTimeProperties(unitInfoLogic, unitInfo.DownloadTime,
            unitInfo.DueTime, unitInfo.FinishedTime);
         SetAndAssertProject(unitInfoLogic);
         AssertDeadlines(unitInfoLogic, new DateTime(2010, 1, 5, 0, 0, 0), new DateTime(2010, 1, 9, 0, 0, 0));

         mocks.VerifyAll();
      }
      
      private static void SetDateTimeProperties(IUnitInfo unitInfo)
      {
         unitInfo.DownloadTime = new DateTime(2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
         unitInfo.DueTime = new DateTime(2010, 1, 6, 0, 0, 0, DateTimeKind.Utc);
         unitInfo.FinishedTime = new DateTime(2010, 1, 1, 4, 30, 0, DateTimeKind.Utc);
      }
      
      private static void AssertDateTimeProperties(UnitInfoLogic unitInfoLogic, DateTime DownloadTime, 
                                                   DateTime DueTime, DateTime FinishedTime)
      {
         // Unit Info Logic Values
         Assert.AreEqual(DownloadTime, unitInfoLogic.DownloadTime);
         // use DueTime for PreferredDeadline when CurrentProtein.IsUnknown
         Assert.AreEqual(unitInfoLogic.DueTime, unitInfoLogic.PreferredDeadline);
         Assert.AreEqual(false, unitInfoLogic.PreferredDeadlineUnknown);
         Assert.AreEqual(DateTime.MinValue, unitInfoLogic.FinalDeadline);
         Assert.AreEqual(true, unitInfoLogic.FinalDeadlineUnknown);
         Assert.AreEqual(DueTime, unitInfoLogic.DueTime);
         Assert.AreEqual(FinishedTime, unitInfoLogic.FinishedTime);
      }
      
      private static void SetAndAssertProject(UnitInfoLogic unitInfoLogic)
      {
         Assert.IsTrue(unitInfoLogic.CurrentProtein.IsUnknown);
         unitInfoLogic.ProjectID = 0;
         Assert.IsTrue(unitInfoLogic.CurrentProtein.IsUnknown);
         unitInfoLogic.ProjectID = 2669;
         unitInfoLogic.ProjectRun = 1;
         unitInfoLogic.ProjectClone = 2;
         unitInfoLogic.ProjectGen = 3;
         Assert.IsFalse(unitInfoLogic.CurrentProtein.IsUnknown);
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})",
            unitInfoLogic.ProjectID, unitInfoLogic.ProjectRun,
            unitInfoLogic.ProjectClone, unitInfoLogic.ProjectGen),
            unitInfoLogic.ProjectRunCloneGen);
      }
      
      private static void AssertDeadlines(IUnitInfoLogic unitInfoLogic, DateTime PreferredDeadline, DateTime FinalDeadline)
      {
         Assert.AreEqual(PreferredDeadline, unitInfoLogic.PreferredDeadline);
         Assert.AreEqual(false, unitInfoLogic.PreferredDeadlineUnknown);
         Assert.AreEqual(FinalDeadline, unitInfoLogic.FinalDeadline);
         Assert.AreEqual(false, unitInfoLogic.FinalDeadlineUnknown);
      }

      [Test]
      public void UnitInfoLogicFramePropertyTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 3, 6);
         IPreferenceSet Prefs = SetupMockPreferenceSet();

         UnitInfo unitInfo = new UnitInfo();
         ILogLine line1 = MakeLogLine("00:00:00", 0, 0, 250000);
         ILogLine line2 = MakeLogLine("00:04:00", 1, 2500, 250000);
         ILogLine line3 = MakeLogLine("00:09:00", 2, 5000, 250000);
         ILogLine line4 = MakeLogLine("00:15:00", 3, 7500, 250000);
         mocks.ReplayAll();

         DateTime baseDate = new DateTime(2010, 1, 1);
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection, 
            unitInfo, false, "Owner", "Path", baseDate.Add(TimeSpan.FromMinutes(30)));

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
         // PPD
         Assert.AreEqual(new TimeSpan(8, 5, 0), unitInfoLogic.ETA);
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.EFT);
         unitInfo.DownloadTime = baseDate;
         Assert.AreEqual(new TimeSpan(14, 35, 00), unitInfoLogic.EFT);
         Assert.AreEqual(TimeSpan.FromMinutes(15), unitInfoLogic.TimeOfLastFrame);
         Assert.AreEqual(3, unitInfoLogic.LastUnitFrameID);

         AssertPpdVariations(unitInfoLogic, 7800, 300, 300, 360, 0);
      }

      [Test]
      public void UnitInfoLogicFramePropertyUtcZeroTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 3, 6);
         IPreferenceSet Prefs = SetupMockPreferenceSet();

         UnitInfo unitInfo = new UnitInfo();
         ILogLine line1 = MakeLogLine("00:00:00", 0, 0, 100);
         ILogLine line2 = MakeLogLine("00:05:10", 1, 1, 100);
         ILogLine line3 = MakeLogLine("00:11:30", 2, 2, 100);
         ILogLine line4 = MakeLogLine("00:17:40", 3, 3, 100);
         ILogLine line5 = MakeLogLine("00:24:00", 4, 4, 100);
         mocks.ReplayAll();

         DateTime baseDate = new DateTime(2010, 1, 1);
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection,
            unitInfo, true, "Owner", "Path", baseDate.Add(TimeSpan.FromMinutes(90)));

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
         // PPD
         Assert.AreEqual(new TimeSpan(9, 36, 0), unitInfoLogic.ETA);
         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.EFT);
         unitInfo.DownloadTime = baseDate;
         Assert.AreEqual(new TimeSpan(11, 6, 00), unitInfoLogic.EFT);
         Assert.AreEqual(TimeSpan.FromMinutes(24), unitInfoLogic.TimeOfLastFrame);
         Assert.AreEqual(4, unitInfoLogic.LastUnitFrameID);

         AssertPpdVariations(unitInfoLogic, 1350, 360, 376, 380, 0);
      }
      
      

      [Test]
      public void UnitInfoLogicFramesAndPpdTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 3, 6);
         IPreferenceSet Prefs = SetupMockPreferenceSet();

         UnitInfo unitInfo = new UnitInfo();
         ILogLine line1 = MakeLogLine("00:00:00", 0, 0, 100);
         ILogLine line2 = MakeLogLine("00:05:10", 1, 1, 100);
         ILogLine line3 = MakeLogLine("00:11:30", 2, 2, 100);
         ILogLine line4 = MakeLogLine("00:17:40", 3, 3, 100);
         ILogLine line5 = MakeLogLine("00:24:00", 4, 4, 100);
         mocks.ReplayAll();

         DateTime baseDate = new DateTime(2010, 1, 1);
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection,
            unitInfo, true, "Owner", "Path", baseDate.Add(TimeSpan.FromMinutes(90)));

         AssertRawTimesZero(unitInfoLogic);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 5;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);
         unitInfo.SetCurrentFrame(line5);

         unitInfoLogic.ProjectID = 2669;
         unitInfoLogic.ProjectRun = 1;
         unitInfoLogic.ProjectClone = 2;
         unitInfoLogic.ProjectGen = 3;
         Assert.IsFalse(unitInfoLogic.CurrentProtein.IsUnknown);

         unitInfo.DownloadTime = baseDate;
         AssertPpdVariations(unitInfoLogic, 1350, 360, 376, 380, 100);
      }

      [Test]
      public void UnitInfoLogicFramesAndBonusPpdTest()
      {
         IProteinCollection proteinCollection = SetupMockProteinCollection("GROCVS", 100, 3, 6);
         IPreferenceSet Prefs = SetupMockPreferenceSet();
         Expect.Call(Prefs.GetPreference<bool>(Preference.CalculateBonus)).Return(true).Repeat.Any();

         UnitInfo unitInfo = new UnitInfo();
         ILogLine line1 = MakeLogLine("00:00:00", 0, 0, 100);
         ILogLine line2 = MakeLogLine("00:05:10", 1, 1, 100);
         ILogLine line3 = MakeLogLine("00:11:30", 2, 2, 100);
         ILogLine line4 = MakeLogLine("00:17:40", 3, 3, 100);
         ILogLine line5 = MakeLogLine("00:24:00", 4, 4, 100);
         mocks.ReplayAll();

         DateTime baseDate = new DateTime(2010, 1, 1);
         UnitInfoLogic unitInfoLogic = new UnitInfoLogic(Prefs, proteinCollection,
            unitInfo, true, "Owner", "Path", baseDate.Add(TimeSpan.FromMinutes(90)));

         AssertRawTimesZero(unitInfoLogic);

         Assert.AreEqual(TimeSpan.Zero, unitInfoLogic.TimeOfLastFrame);
         unitInfo.FramesObserved = 5;
         unitInfo.SetCurrentFrame(line1);
         unitInfo.SetCurrentFrame(line2);
         unitInfo.SetCurrentFrame(line3);
         unitInfo.SetCurrentFrame(line4);
         unitInfo.SetCurrentFrame(line5);

         unitInfoLogic.ProjectID = 2669;
         unitInfoLogic.ProjectRun = 1;
         unitInfoLogic.ProjectClone = 2;
         unitInfoLogic.ProjectGen = 3;
         Assert.IsFalse(unitInfoLogic.CurrentProtein.IsUnknown);

         unitInfo.DownloadTime = baseDate;
         AssertPpdVariations(unitInfoLogic, 1350, 360, 376, 380, 200);
      }

      private IProteinCollection SetupMockProteinCollection(string Core, int Frames, int PreferredDays, int MaxDays)
      {
         IProtein currentProtein = mocks.DynamicMock<IProtein>();
         Expect.Call(currentProtein.Core).Return(Core).Repeat.Any();
         Expect.Call(currentProtein.Frames).Return(Frames).Repeat.Any();
         Expect.Call(currentProtein.PreferredDays).Return(PreferredDays).Repeat.Any();
         Expect.Call(currentProtein.MaxDays).Return(MaxDays).Repeat.Any();
         Expect.Call(currentProtein.GetPPD(TimeSpan.Zero)).IgnoreArguments().Return(100).Repeat.Any();
         Expect.Call(currentProtein.GetPPD(TimeSpan.Zero, TimeSpan.Zero)).IgnoreArguments().Return(200).Repeat.Any();

         IProtein newProtein = mocks.DynamicMock<IProtein>();
         Expect.Call(newProtein.Core).Return(String.Empty).Repeat.Any();
         Expect.Call(newProtein.Frames).Return(Frames).Repeat.Any();
         Expect.Call(newProtein.IsUnknown).Return(true).Repeat.Any();

         IProteinCollection proteinCollection = mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0)).Return(newProtein).Repeat.Any();
         Expect.Call(proteinCollection.GetProtein(2669)).Return(currentProtein).Repeat.Any();
         Expect.Call(proteinCollection.GetNewProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
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
         IPreferenceSet Prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(Prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation)).Return(PpdCalculationType.AllFrames).Repeat.Any();
         return Prefs;
      }

      public ILogLine MakeLogLine(string TimeStampString, int FrameID, int Complete, int Total)
      {
         ILogLine line = mocks.DynamicMock<ILogLine>();
         IFrameData frame = mocks.DynamicMock<IFrameData>();

         Expect.Call(line.LineType).Return(LogLineType.WorkUnitFrame);
         Expect.Call(line.LineData).Return(frame);
         Expect.Call(frame.FrameID).Return(FrameID).Repeat.Any();
         Expect.Call(frame.TimeStampString).Return(TimeStampString);
         Expect.Call(frame.RawFramesComplete).Return(Complete);
         Expect.Call(frame.RawFramesTotal).Return(Total);
         
         return line;
      }

      private static void AssertPpdVariations(IUnitInfoLogic unitInfoLogic, int UnitDownload, int AllSections,
                                              int ThreeSections, int LastSection, double PPD)
      {
         Assert.AreEqual(UnitDownload, unitInfoLogic.RawTimePerUnitDownload);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerUnitDownload), unitInfoLogic.TimePerUnitDownload);
         Assert.AreEqual(PPD, unitInfoLogic.PPDPerUnitDownload);

         Assert.AreEqual(AllSections, unitInfoLogic.RawTimePerAllSections);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerAllSections), unitInfoLogic.TimePerAllSections);
         Assert.AreEqual(PPD, unitInfoLogic.PPDPerAllSections);

         Assert.AreEqual(ThreeSections, unitInfoLogic.RawTimePerThreeSections);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerThreeSections), unitInfoLogic.TimePerThreeSections);
         Assert.AreEqual(PPD, unitInfoLogic.PPDPerThreeSections);

         Assert.AreEqual(LastSection, unitInfoLogic.RawTimePerLastSection);
         Assert.AreEqual(TimeSpan.FromSeconds(unitInfoLogic.RawTimePerLastSection), unitInfoLogic.TimePerLastSection);
         Assert.AreEqual(PPD, unitInfoLogic.PPDPerLastSection);
      }
   }
}
