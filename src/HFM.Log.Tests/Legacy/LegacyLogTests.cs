
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using NUnit.Framework;

using HFM.Log.Internal;

namespace HFM.Log.Legacy
{
   [TestFixture]
   public class LegacyLogTests
   {
      [Test]
      public async Task LegacyLog_ReadAsync_FromFahLogReader_Test()
      {
         // Arrange
         var log = new LegacyLog();
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt"))
         using (var reader = new LegacyLogTextReader(textReader))
         {
            // Act
            await log.ReadAsync(reader);
         }
         // Assert
         Assert.IsTrue(log.ClientRuns.Count > 0);
      }

      [Test]
      public async Task LegacyLog_ReadAsync_FromPath_Test()
      {
         // Arrange
         var log = await LegacyLog.ReadAsync("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt");
         Assert.IsTrue(log.ClientRuns.Count > 0);
      }

      [Test]
      public void LegacyLog_Clear_Test()
      {
         // Arrange
         var log = new LegacyLog();
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt"))
         using (var reader = new LegacyLogTextReader(textReader))
         {
            log.Read(reader);
         }
         Assert.IsTrue(log.ClientRuns.Count > 0);
         // Act
         log.Clear();
         // Assert
         Assert.AreEqual(0, log.ClientRuns.Count);
      }

      // ReSharper disable InconsistentNaming

      [Test]
      public void LegacyLog_Read_SMP_1_Test() // verbosity 9
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 30, 149));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 150, 273));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 7, 30, 0, 40, 27, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp -verbosity 9";
         expectedRunData.ClientVersion = "6.24beta";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5131EA752EB60547";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 261;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 274);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 302, 401));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 402, 752));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 7, 31, 0, 7, 43, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp -verbosity 9";
         expectedRunData.ClientVersion = "6.24beta";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5131EA752EB60547";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 2;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 263;
         expectedSlotRunData.Status = LegacySlotStatus.GettingWorkPacket;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.First().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(5, unitRun.LogLines[3].Data);
         Assert.AreEqual("2.08", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[21].ToString().Contains("Project: 2677 (Run 10, Clone 29, Gen 28)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[79].Data);
      }

      [Test]
      public void LegacyLog_Read_SMP_2_Test() // verbosity 9
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_2\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 30, 220));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 221, 382));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 18, 2, 40, 5, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp -verbosity 9";
         expectedRunData.ClientVersion = "6.24beta";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "3A49EBB303C19834";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 2;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 292;
         expectedSlotRunData.Status = LegacySlotStatus.SendingWorkPacket;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual("2.08", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[17].ToString().Contains("Project: 2677 (Run 10, Clone 49, Gen 38)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[150].Data);
      }

      [Test]
      public void LegacyLog_Read_SMP_3_Test() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_3\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 231, 384));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 385, 408));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 25, 18, 11, 37, DateTimeKind.Utc);
         expectedRunData.Arguments = "-local -forceasm -smp 4";
         expectedRunData.ClientVersion = "6.02";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         // verbosity (normal) does not output User ID after requested from server
         // see ClientLogLines indexes 29 & 30
         expectedRunData.UserID = null;
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         // NOTE: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.TotalCompletedUnits = null; 
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual("2.08", unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[15].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[137].Data);
      }

      [Test]
      public void LegacyLog_Read_SMP_10_Test() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt");

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 2);

         // Setup SlotRun 0
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 6, 26, 82);
         var expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(11, 40, 51);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 16;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         var frameData = new WorkUnitFrameData();
         frameData.ID = 1;
         frameData.RawFramesComplete = 2500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(437960000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 2;
         frameData.RawFramesComplete = 5000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(467420000000);
         frameData.Duration = TimeSpan.FromTicks(29460000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 3;
         frameData.RawFramesComplete = 7500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(495910000000);
         frameData.Duration = TimeSpan.FromTicks(28490000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 4;
         frameData.RawFramesComplete = 10000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(524760000000);
         frameData.Duration = TimeSpan.FromTicks(28850000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 5;
         frameData.RawFramesComplete = 12500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(545810000000);
         frameData.Duration = TimeSpan.FromTicks(21050000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 6;
         frameData.RawFramesComplete = 15000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(565090000000);
         frameData.Duration = TimeSpan.FromTicks(19280000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 7;
         frameData.RawFramesComplete = 17500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(584280000000);
         frameData.Duration = TimeSpan.FromTicks(19190000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 8;
         frameData.RawFramesComplete = 20000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(603520000000);
         frameData.Duration = TimeSpan.FromTicks(19240000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 9;
         frameData.RawFramesComplete = 22500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(622710000000);
         frameData.Duration = TimeSpan.FromTicks(19190000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 10;
         frameData.RawFramesComplete = 25000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(642200000000);
         frameData.Duration = TimeSpan.FromTicks(19490000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 11;
         frameData.RawFramesComplete = 27500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(661990000000);
         frameData.Duration = TimeSpan.FromTicks(19790000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 12;
         frameData.RawFramesComplete = 30000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(681760000000);
         frameData.Duration = TimeSpan.FromTicks(19770000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 13;
         frameData.RawFramesComplete = 32500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(703600000000);
         frameData.Duration = TimeSpan.FromTicks(21840000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 14;
         frameData.RawFramesComplete = 35000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(723520000000);
         frameData.Duration = TimeSpan.FromTicks(19920000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 15;
         frameData.RawFramesComplete = 37500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(743220000000);
         frameData.Duration = TimeSpan.FromTicks(19700000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 16;
         frameData.RawFramesComplete = 40000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(762810000000);
         frameData.Duration = TimeSpan.FromTicks(19590000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 0
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 9, 11, 40, 51, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 1
         expectedRun = new ClientRun(null, 83);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 110, 180);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(21, 28, 23);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 30;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 16;
         frameData.RawFramesComplete = 40034;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(773750000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 17;
         frameData.RawFramesComplete = 42500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(802160000000);
         frameData.Duration = TimeSpan.FromTicks(28410000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 18;
         frameData.RawFramesComplete = 45000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(831110000000);
         frameData.Duration = TimeSpan.FromTicks(28950000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 19;
         frameData.RawFramesComplete = 47500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(861260000000);
         frameData.Duration = TimeSpan.FromTicks(30150000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 20;
         frameData.RawFramesComplete = 50000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(21420000000);
         frameData.Duration = TimeSpan.FromTicks(24160000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 21;
         frameData.RawFramesComplete = 52500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(41510000000);
         frameData.Duration = TimeSpan.FromTicks(20090000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 22;
         frameData.RawFramesComplete = 55000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(61390000000);
         frameData.Duration = TimeSpan.FromTicks(19880000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 23;
         frameData.RawFramesComplete = 57500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(81360000000);
         frameData.Duration = TimeSpan.FromTicks(19970000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 24;
         frameData.RawFramesComplete = 60000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(101360000000);
         frameData.Duration = TimeSpan.FromTicks(20000000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 25;
         frameData.RawFramesComplete = 62500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(121220000000);
         frameData.Duration = TimeSpan.FromTicks(19860000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 26;
         frameData.RawFramesComplete = 65000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(141230000000);
         frameData.Duration = TimeSpan.FromTicks(20010000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 27;
         frameData.RawFramesComplete = 67500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(161300000000);
         frameData.Duration = TimeSpan.FromTicks(20070000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 28;
         frameData.RawFramesComplete = 70000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(181330000000);
         frameData.Duration = TimeSpan.FromTicks(20030000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 29;
         frameData.RawFramesComplete = 72500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(200990000000);
         frameData.Duration = TimeSpan.FromTicks(19660000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 30;
         frameData.RawFramesComplete = 75000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(220520000000);
         frameData.Duration = TimeSpan.FromTicks(19530000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 31;
         frameData.RawFramesComplete = 77500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(240670000000);
         frameData.Duration = TimeSpan.FromTicks(20150000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 32;
         frameData.RawFramesComplete = 80000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(260170000000);
         frameData.Duration = TimeSpan.FromTicks(19500000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 33;
         frameData.RawFramesComplete = 82500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(279690000000);
         frameData.Duration = TimeSpan.FromTicks(19520000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 34;
         frameData.RawFramesComplete = 85000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(300080000000);
         frameData.Duration = TimeSpan.FromTicks(20390000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 35;
         frameData.RawFramesComplete = 87500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(320380000000);
         frameData.Duration = TimeSpan.FromTicks(20300000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 36;
         frameData.RawFramesComplete = 90000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(340730000000);
         frameData.Duration = TimeSpan.FromTicks(20350000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 37;
         frameData.RawFramesComplete = 92500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(361110000000);
         frameData.Duration = TimeSpan.FromTicks(20380000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 38;
         frameData.RawFramesComplete = 95000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(381020000000);
         frameData.Duration = TimeSpan.FromTicks(19910000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 39;
         frameData.RawFramesComplete = 97500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(401070000000);
         frameData.Duration = TimeSpan.FromTicks(20050000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 40;
         frameData.RawFramesComplete = 100000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(424980000000);
         frameData.Duration = TimeSpan.FromTicks(23910000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 41;
         frameData.RawFramesComplete = 102500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(448890000000);
         frameData.Duration = TimeSpan.FromTicks(23910000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 42;
         frameData.RawFramesComplete = 105000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(469250000000);
         frameData.Duration = TimeSpan.FromTicks(20360000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 43;
         frameData.RawFramesComplete = 107500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(489090000000);
         frameData.Duration = TimeSpan.FromTicks(19840000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 44;
         frameData.RawFramesComplete = 110000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(509150000000);
         frameData.Duration = TimeSpan.FromTicks(20060000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 45;
         frameData.RawFramesComplete = 112500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(529590000000);
         frameData.Duration = TimeSpan.FromTicks(20440000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 1
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 9, 21, 28, 23, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(1);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 2
         expectedRun = new ClientRun(null, 181);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 206, 291);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(15, 4, 22);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 38;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 46;
         frameData.RawFramesComplete = 115000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(554950000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 47;
         frameData.RawFramesComplete = 117500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(574690000000);
         frameData.Duration = TimeSpan.FromTicks(19740000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 48;
         frameData.RawFramesComplete = 120000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(594490000000);
         frameData.Duration = TimeSpan.FromTicks(19800000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 49;
         frameData.RawFramesComplete = 122500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(621600000000);
         frameData.Duration = TimeSpan.FromTicks(27110000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 50;
         frameData.RawFramesComplete = 125000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(650790000000);
         frameData.Duration = TimeSpan.FromTicks(29190000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 51;
         frameData.RawFramesComplete = 127500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(672820000000);
         frameData.Duration = TimeSpan.FromTicks(22030000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 52;
         frameData.RawFramesComplete = 130000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(692530000000);
         frameData.Duration = TimeSpan.FromTicks(19710000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 53;
         frameData.RawFramesComplete = 132500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(712250000000);
         frameData.Duration = TimeSpan.FromTicks(19720000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 54;
         frameData.RawFramesComplete = 135000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(731930000000);
         frameData.Duration = TimeSpan.FromTicks(19680000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 55;
         frameData.RawFramesComplete = 137500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(751660000000);
         frameData.Duration = TimeSpan.FromTicks(19730000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 56;
         frameData.RawFramesComplete = 140000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(771410000000);
         frameData.Duration = TimeSpan.FromTicks(19750000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 57;
         frameData.RawFramesComplete = 142500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(791120000000);
         frameData.Duration = TimeSpan.FromTicks(19710000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 58;
         frameData.RawFramesComplete = 145000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(810990000000);
         frameData.Duration = TimeSpan.FromTicks(19870000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 59;
         frameData.RawFramesComplete = 147500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(837440000000);
         frameData.Duration = TimeSpan.FromTicks(26450000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 60;
         frameData.RawFramesComplete = 150000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(860690000000);
         frameData.Duration = TimeSpan.FromTicks(23250000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 61;
         frameData.RawFramesComplete = 152500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(16570000000);
         frameData.Duration = TimeSpan.FromTicks(19880000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 62;
         frameData.RawFramesComplete = 155000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(36220000000);
         frameData.Duration = TimeSpan.FromTicks(19650000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 63;
         frameData.RawFramesComplete = 157500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(56020000000);
         frameData.Duration = TimeSpan.FromTicks(19800000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 64;
         frameData.RawFramesComplete = 160000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(75500000000);
         frameData.Duration = TimeSpan.FromTicks(19480000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 65;
         frameData.RawFramesComplete = 162500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(94860000000);
         frameData.Duration = TimeSpan.FromTicks(19360000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 66;
         frameData.RawFramesComplete = 165000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(114150000000);
         frameData.Duration = TimeSpan.FromTicks(19290000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 67;
         frameData.RawFramesComplete = 167500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(133490000000);
         frameData.Duration = TimeSpan.FromTicks(19340000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 68;
         frameData.RawFramesComplete = 170000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(152930000000);
         frameData.Duration = TimeSpan.FromTicks(19440000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 69;
         frameData.RawFramesComplete = 172500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(172370000000);
         frameData.Duration = TimeSpan.FromTicks(19440000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 70;
         frameData.RawFramesComplete = 175000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(191800000000);
         frameData.Duration = TimeSpan.FromTicks(19430000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 71;
         frameData.RawFramesComplete = 177500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(210990000000);
         frameData.Duration = TimeSpan.FromTicks(19190000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 72;
         frameData.RawFramesComplete = 180000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(230140000000);
         frameData.Duration = TimeSpan.FromTicks(19150000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 73;
         frameData.RawFramesComplete = 182500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(249320000000);
         frameData.Duration = TimeSpan.FromTicks(19180000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 74;
         frameData.RawFramesComplete = 185000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(268830000000);
         frameData.Duration = TimeSpan.FromTicks(19510000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 75;
         frameData.RawFramesComplete = 187500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(287990000000);
         frameData.Duration = TimeSpan.FromTicks(19160000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 76;
         frameData.RawFramesComplete = 190000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(307160000000);
         frameData.Duration = TimeSpan.FromTicks(19170000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 77;
         frameData.RawFramesComplete = 192500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(326400000000);
         frameData.Duration = TimeSpan.FromTicks(19240000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 78;
         frameData.RawFramesComplete = 195000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(345850000000);
         frameData.Duration = TimeSpan.FromTicks(19450000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 79;
         frameData.RawFramesComplete = 197500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(365620000000);
         frameData.Duration = TimeSpan.FromTicks(19770000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 80;
         frameData.RawFramesComplete = 200000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(385280000000);
         frameData.Duration = TimeSpan.FromTicks(19660000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 81;
         frameData.RawFramesComplete = 202500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(405020000000);
         frameData.Duration = TimeSpan.FromTicks(19740000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 82;
         frameData.RawFramesComplete = 205000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(424650000000);
         frameData.Duration = TimeSpan.FromTicks(19630000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 83;
         frameData.RawFramesComplete = 207500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(444310000000);
         frameData.Duration = TimeSpan.FromTicks(19660000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 2
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 10, 15, 4, 22, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(2);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 3
         expectedRun = new ClientRun(null, 292);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 316, 353);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 1, 31);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 2;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 83;
         frameData.RawFramesComplete = 207474;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(469390000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 3
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 13, 1, 31, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(3);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 4
         expectedRun = new ClientRun(null, 354);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 382, 400);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 20, 2);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 0;
         expectedUnitRunData.ProjectID = 0;
         expectedUnitRunData.ProjectRun = 0;
         expectedUnitRunData.ProjectClone = 0;
         expectedUnitRunData.ProjectGen = 0;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 4
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 13, 20, 2, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(4);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(0, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 5
         expectedRun = new ClientRun(null, 401);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, -1, 426, 449);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(13, 21, 35);
         expectedUnitRunData.CoreVersion = null;
         expectedUnitRunData.FramesObserved = 2;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 84;
         frameData.RawFramesComplete = 210000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(492790000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 85;
         frameData.RawFramesComplete = 212500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(513170000000);
         frameData.Duration = TimeSpan.FromTicks(20380000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 5
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 13, 20, 57, DateTimeKind.Utc);
         expectedRunData.Arguments = "-configonly";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(5);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 6
         expectedRun = new ClientRun(null, 450);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 475, 513);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(14, 25, 41);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 3;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 86;
         frameData.RawFramesComplete = 215000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(536220000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 87;
         frameData.RawFramesComplete = 217500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(556700000000);
         frameData.Duration = TimeSpan.FromTicks(20480000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 88;
         frameData.RawFramesComplete = 220000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(584830000000);
         frameData.Duration = TimeSpan.FromTicks(28130000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 6
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 14, 25, 41, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(6);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 7
         expectedRun = new ClientRun(null, 514);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 538, 578);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(16, 51, 8);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 4;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 89;
         frameData.RawFramesComplete = 222500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(611920000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 90;
         frameData.RawFramesComplete = 225000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(632370000000);
         frameData.Duration = TimeSpan.FromTicks(20450000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 91;
         frameData.RawFramesComplete = 227500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(652920000000);
         frameData.Duration = TimeSpan.FromTicks(20550000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 92;
         frameData.RawFramesComplete = 230000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(673590000000);
         frameData.Duration = TimeSpan.FromTicks(20670000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 7
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 16, 51, 7, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(7);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 8
         expectedRun = new ClientRun(null, 579);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 6, 607, 700);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(19, 21, 4);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 8;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 4;
         expectedUnitRunData.ProjectClone = 11;
         expectedUnitRunData.ProjectGen = 18;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.FINISHED_UNIT;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 93;
         frameData.RawFramesComplete = 232500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(706920000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 94;
         frameData.RawFramesComplete = 235000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(728520000000);
         frameData.Duration = TimeSpan.FromTicks(21600000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 95;
         frameData.RawFramesComplete = 237500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(750110000000);
         frameData.Duration = TimeSpan.FromTicks(21590000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 96;
         frameData.RawFramesComplete = 240000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(771700000000);
         frameData.Duration = TimeSpan.FromTicks(21590000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 97;
         frameData.RawFramesComplete = 242500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(793280000000);
         frameData.Duration = TimeSpan.FromTicks(21580000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 98;
         frameData.RawFramesComplete = 245000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(814860000000);
         frameData.Duration = TimeSpan.FromTicks(21580000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 99;
         frameData.RawFramesComplete = 247500;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(836330000000);
         frameData.Duration = TimeSpan.FromTicks(21470000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 100;
         frameData.RawFramesComplete = 250000;
         frameData.RawFramesTotal = 250000;
         frameData.TimeStamp = TimeSpan.FromTicks(857550000000);
         frameData.Duration = TimeSpan.FromTicks(21220000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 7, 701, 731);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(0, 9, 22);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 0;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 6;
         expectedUnitRunData.ProjectClone = 12;
         expectedUnitRunData.ProjectGen = 21;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 8
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 11, 19, 21, 4, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(8);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));
         // Setup ClientRun 9
         expectedRun = new ClientRun(null, 732);

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 7, 760, 809);
         expectedUnitRunData = new LegacyUnitRunData();
         expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(1, 12, 10);
         expectedUnitRunData.CoreVersion = "2.10";
         expectedUnitRunData.FramesObserved = 13;
         expectedUnitRunData.ProjectID = 2683;
         expectedUnitRunData.ProjectRun = 6;
         expectedUnitRunData.ProjectClone = 12;
         expectedUnitRunData.ProjectGen = 21;
         expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;
         frameDataDictionary = new Dictionary<int, WorkUnitFrameData>();
         frameData = new WorkUnitFrameData();
         frameData.ID = 2;
         frameData.RawFramesComplete = 5000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(57660000000);
         frameData.Duration = TimeSpan.FromTicks(0);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 3;
         frameData.RawFramesComplete = 7500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(77480000000);
         frameData.Duration = TimeSpan.FromTicks(19820000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 4;
         frameData.RawFramesComplete = 10000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(97360000000);
         frameData.Duration = TimeSpan.FromTicks(19880000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 5;
         frameData.RawFramesComplete = 12500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(117320000000);
         frameData.Duration = TimeSpan.FromTicks(19960000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 6;
         frameData.RawFramesComplete = 15000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(137540000000);
         frameData.Duration = TimeSpan.FromTicks(20220000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 7;
         frameData.RawFramesComplete = 17500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(157650000000);
         frameData.Duration = TimeSpan.FromTicks(20110000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 8;
         frameData.RawFramesComplete = 20000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(177830000000);
         frameData.Duration = TimeSpan.FromTicks(20180000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 9;
         frameData.RawFramesComplete = 22500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(197960000000);
         frameData.Duration = TimeSpan.FromTicks(20130000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 10;
         frameData.RawFramesComplete = 25000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(217930000000);
         frameData.Duration = TimeSpan.FromTicks(19970000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 11;
         frameData.RawFramesComplete = 27500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(237640000000);
         frameData.Duration = TimeSpan.FromTicks(19710000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 12;
         frameData.RawFramesComplete = 30000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(257270000000);
         frameData.Duration = TimeSpan.FromTicks(19630000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 13;
         frameData.RawFramesComplete = 32500;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(276950000000);
         frameData.Duration = TimeSpan.FromTicks(19680000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         frameData = new WorkUnitFrameData();
         frameData.ID = 14;
         frameData.RawFramesComplete = 35000;
         frameData.RawFramesTotal = 249999;
         frameData.TimeStamp = TimeSpan.FromTicks(296610000000);
         frameData.Duration = TimeSpan.FromTicks(19660000000);
         frameDataDictionary.Add(frameData.ID, frameData);
         expectedUnitRunData.FrameDataDictionary = frameDataDictionary;
         expectedUnitRun.Data = expectedUnitRunData;
         expectedSlotRun.UnitRuns.Add(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         // Setup ClientRunData 9
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 12, 12, 1, 12, 10, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp 8 -bigadv -verbosity 9";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;

         actualRun = fahLog.ClientRuns.ElementAt(9);
         AssertClientRun.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, LogLineEnumerable.Create(actualRun).Count(x => x.Data is LogLineDataParserError));

         // Verify LogLine Properties
         actualRun = fahLog.ClientRuns.Last();
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 8 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(8).SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(6, unitRun.LogLines[3].Data);
         Assert.AreEqual("2.10", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[21].ToString().Contains("Project: 2683 (Run 4, Clone 11, Gen 18)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[53].Data);
      }

      [Test]
      public void LegacyLog_Read_SMP_15_Test() // lots of Client-core communications error
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_15\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 36, 233));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 234, 283));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 284, 333));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 334, 657));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 658, 707));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 708, 757));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 758, 1081));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 1082, 1146));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 1147, 1218));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 1219, 1268));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 1269, 1340));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 1341, 1435));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 1436, 1537));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 1538, 1587));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 1588, 1637));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 1638, 1709));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 1710, 1759));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 1760, 1824));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 1825, 2148));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 2149, 2198));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 2199, 2417));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 2418, 2489));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 2490, 2539));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 2540, 2589));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 2590, 2913));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 2914, 2963));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 2964, 3013));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 3014, 3352));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 3353, 3447));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 3448, 3497));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 3498, 3644));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 3645, 3709));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 3710, 3759));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 3760, 3792));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 9, 14, 2, 48, 27, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp -verbosity 9";
         expectedRunData.ClientVersion = "6.30";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "DC1DAF57D91DF79";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 33;
         expectedSlotRunData.TotalCompletedUnits = 617;
         expectedSlotRunData.Status = LegacySlotStatus.EuePause;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.First().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(0, unitRun.LogLines[3].Data);
         Assert.AreEqual("2.22", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[20].ToString().Contains("Project: 6071 (Run 0, Clone 39, Gen 70)"));
         Assert.AreEqual(LogLineType.ClientCoreCommunicationsError, (int)unitRun.LogLines[26].LineType);
      }

      [Test]
      public void LegacyLog_Read_SMP_17_Test() // v6.23 A4 SMP
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_17\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 24, 174));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 175, 207));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 3, 20, 7, 52, 34, DateTimeKind.Utc);
         expectedRunData.Arguments = "-smp -bigadv -betateam -verbosity 9";
         expectedRunData.ClientVersion = "6.34";
         expectedRunData.FoldingID = "GreyWhiskers";
         expectedRunData.Team = 0;
         expectedRunData.UserID = "51EA5C9A7EF9D58E";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 885;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(0, unitRun.LogLines[8].Data);
         Assert.AreEqual("2.27", unitRun.LogLines[15].Data);
         Assert.That(unitRun.LogLines[27].ToString().Contains("Project: 8022 (Run 11, Clone 318, Gen 24)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[106].Data);
      }

      [Test]
      public void LegacyLog_Read_GPU2_1_Test() // verbosity 9
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\GPU2_1\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 130, 325));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 326, 386));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 387, 448));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 449, 509));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 510, 570));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 571, 617));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 8, 5, 7, 18, DateTimeKind.Utc);
         expectedRunData.Arguments = "-verbosity 9 -local";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "CF185086C102A47";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 5;
         // NOTE: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.TotalCompletedUnits = null; 
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 618);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 663, 736));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 737, 934));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 935, 1131));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 1132, 1328));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 1329, 1525));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 1526, 1723));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 1724, 1925));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 1926, 2122));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 2123, 2320));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 2321, 2517));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 2518, 2714));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 2715, 2916));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 2917, 2995));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 8, 6, 18, 28, DateTimeKind.Utc);
         expectedRunData.Arguments = "-verbosity 9 -local";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "CF185086C102A47";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 11;
         expectedSlotRunData.FailedUnits = 1;
         expectedSlotRunData.TotalCompletedUnits = 12;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.First().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual("1.19", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[24].ToString().Contains("Project: 5771 (Run 12, Clone 109, Gen 805)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[156].Data);
      }

      [Test]
      public void LegacyLog_Read_GPU2_2_Test() // verbosity (normal)
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\GPU2_2\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 34, 207));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 208, 381));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 382, 446));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 14, 4, 40, 2, DateTimeKind.Utc);
         expectedRunData.Arguments = null;
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "51108B97183EA3DF";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 2;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 4221;
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.First().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(8, unitRun.LogLines[3].Data);
         Assert.AreEqual("1.19", unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[22].ToString().Contains("Project: 5751 (Run 8, Clone 205, Gen 527)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[154].Data);
      }

      [Test]
      public void LegacyLog_Read_GPU2_3_Test() // verbosity (normal) / EUE Pause Test
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\GPU2_3\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 24, 55));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 18, 3, 26, 33, DateTimeKind.Utc);
         expectedRunData.Arguments = null;
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "JollySwagman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "1D1493BB0A79C9AE";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 56);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 80, 220));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 221, 270));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 8, 271, 319));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 320, 372));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 373, 420));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 421, 463));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 18, 3, 54, 16, DateTimeKind.Utc);
         expectedRunData.Arguments = null;
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "JollySwagman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "1D1493BB0A79C9AE";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 5;
         expectedSlotRunData.TotalCompletedUnits = 224;
         expectedSlotRunData.Status = LegacySlotStatus.EuePause;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - UnitRun 3)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.ElementAt(3);
         Assert.AreEqual(9, unitRun.LogLines[3].Data);
         Assert.AreEqual("1.19", unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[22].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(WorkUnitResult.UNSTABLE_MACHINE, unitRun.LogLines[39].Data);

         // Special Check to be sure the reader is catching the EUE Pause line (Current ClientRun - Current UnitRun)
         unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(LogLineType.ClientEuePauseState, (int)unitRun.LogLines[42].LineType);
      }

      [Test]
      public void LegacyLog_Read_GPU2_7_Test() // verbosity (normal) / Project String After "+ Processing work unit"
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\GPU2_7\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 24, 82));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 1, 31, 1, 57, 21, DateTimeKind.Utc);
         expectedRunData.Arguments = null;
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "Zagen30";
         expectedRunData.Team = 46301;
         expectedRunData.UserID = "xxxxxxxxxxxxxxxxxxx";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 1994;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(0, unitRun.LogLines[4].Data);
         Assert.AreEqual("1.31", unitRun.LogLines[13].Data);
         Assert.That(unitRun.LogLines[26].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual("1.31", unitRun.Data.CoreVersion);
         Assert.AreEqual(5781, unitRun.Data.ProjectID);
         Assert.AreEqual(2, unitRun.Data.ProjectRun);
         Assert.AreEqual(700, unitRun.Data.ProjectClone);
         Assert.AreEqual(2, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.None, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void LegacyLog_Read_GPU3_2_Test() // verbosity 9 / OPENMMGPU v2.19
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\GPU3_2\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 27, 169));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 170, 218));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 2, 17, 17, 19, 31, DateTimeKind.Utc);
         expectedRunData.Arguments = "-gpu 0 -verbosity 9 -local -verbosity 9";
         expectedRunData.ClientVersion = "6.41r2";
         expectedRunData.FoldingID = "HayesK";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "37114EB5198643C1";
         expectedRunData.MachineID = 2;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 847;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(3, unitRun.LogLines[7].Data);
         Assert.AreEqual("2.19", unitRun.LogLines[14].Data);
         Assert.That(unitRun.LogLines[29].ToString().Contains("Project: 10634 (Run 11, Clone 24, Gen 14)"));

         unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(new TimeSpan(17, 31, 22), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(12, unitRun.Data.FramesObserved);
         Assert.AreEqual("2.19", unitRun.Data.CoreVersion);
         Assert.AreEqual(10634, unitRun.Data.ProjectID);
         Assert.AreEqual(8, unitRun.Data.ProjectRun);
         Assert.AreEqual(24, unitRun.Data.ProjectClone);
         Assert.AreEqual(24, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.None, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void LegacyLog_Read_Standard_1_Test() // verbosity 9
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\Standard_1\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 18, 2, 15, 30, DateTimeKind.Utc);
         expectedRunData.Arguments = "-configonly";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "4E34332601E26450";
         expectedRunData.MachineID = 5;
         expectedRun.Data = expectedRunData;
         expectedSlotRun.Data = new LegacySlotRunData();

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 30);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 179, 592));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 593, 838));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 18, 2, 17, 46, DateTimeKind.Utc);
         expectedRunData.Arguments = "-verbosity 9 -forceasm";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "4E34332601E26450";
         expectedRunData.MachineID = 5;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         // NOTE: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.TotalCompletedUnits = null; 
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.ElementAt(1);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 2 Positions
         expectedRun = new ClientRun(null, 839);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 874, 951));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 8, 20, 4, 17, 29, DateTimeKind.Utc);
         expectedRunData.Arguments = "-verbosity 9 -forceasm -oneunit";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "4E34332601E26450";
         expectedRunData.MachineID = 5;
         expectedRun.Data = expectedRunData;
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 2;
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 1 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(1).SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual("1.90", unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[18].ToString().Contains("Project: 4456 (Run 173, Clone 0, Gen 31)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[254].Data);
      }

      [Test]
      public void LegacyLog_Read_Standard_5_Test() // verbosity 9
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\Standard_5\\FAHlog.txt");

         // Check Run 3 Positions
         var expectedRun = new ClientRun(null, 788);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 820, 926));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 3, 24, 0, 28, 52, DateTimeKind.Utc);
         expectedRunData.Arguments = "-oneunit -forceasm -verbosity 9";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "borden.b";
         expectedRunData.Team = 131;
         expectedRunData.UserID = "722723950C6887C2";
         expectedRunData.MachineID = 3;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(3);
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Check Run 4 Positions
         expectedRun = new ClientRun(null, 927);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 961, 1014));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 3, 24, 0, 41, 07, DateTimeKind.Utc);
         expectedRunData.Arguments = "-forceasm -verbosity 9 -oneunit";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "borden.b";
         expectedRunData.Team = 131;
         expectedRunData.UserID = "722723950C6887C2";
         expectedRunData.MachineID = 3;
         expectedRun.Data = expectedRunData;
         expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LegacySlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(4, unitRun.LogLines[6].Data);
         Assert.AreEqual("23", unitRun.LogLines[17].Data);
         Assert.That(unitRun.LogLines[2].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[10].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[45].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));

         unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(new TimeSpan(0, 41, 7), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual("23", unitRun.Data.CoreVersion);
         Assert.AreEqual(10002, unitRun.Data.ProjectID);
         Assert.AreEqual(19, unitRun.Data.ProjectRun);
         Assert.AreEqual(0, unitRun.Data.ProjectClone);
         Assert.AreEqual(51, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.None, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void LegacyLog_Read_Standard_6_Test() // verbosity normal / Gromacs 3.3
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\Standard_6\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 9, 27, 293));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 0, 294, 553));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 1, 554, 813));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 2, 814, 1073));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 3, 1074, 1337));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 4, 1338, 1601));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 1602, 1869));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 6, 1870, 2129));
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 7, 2130, 2323));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 3, 10, 15, 48, 32, DateTimeKind.Utc);
         expectedRunData.Arguments = null;
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "DrSpalding";
         expectedRunData.Team = 48083;
         expectedRunData.UserID = "1E19BD450434A6ED";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 8;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 229;
         expectedSlotRunData.Status = LegacySlotStatus.Paused;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(7, unitRun.LogLines[3].Data);
         Assert.AreEqual("1.90", unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[17].ToString().Contains("Project: 4461 (Run 886, Clone 3, Gen 56)"));

         // Special Check to be sure the reader is catching the Pause For Battery line
         Assert.AreEqual(LogLineType.WorkUnitPausedForBattery, (int)unitRun.LogLines[193].LineType);
      }

      [Test]
      public void LegacyLog_Read_Standard_9_Test() // v6.23 A4 Uniprocessor
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\Standard_9\\FAHlog.txt");

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Add(new UnitRun(expectedSlotRun, 5, 24, 276));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.Now.Year, 3, 16, 18, 46, 15, DateTimeKind.Utc);
         expectedRunData.Arguments = "-oneunit -verbosity 9";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "Amaruk";
         expectedRunData.Team = 50625;
         expectedRunData.UserID = "1E53CB2Axxxxxxxx";
         expectedRunData.MachineID = 14;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 1;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = 173;
         expectedSlotRunData.Status = LegacySlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Last();
         AssertClientRun.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.First().SlotRuns[0].UnitRuns.First();
         Assert.AreEqual(5, unitRun.LogLines[7].Data);
         Assert.AreEqual("2.27", unitRun.LogLines[14].Data);
         Assert.That(unitRun.LogLines[23].ToString().Contains("Project: 10741 (Run 0, Clone 1996, Gen 3)"));
      }

      // ReSharper restore InconsistentNaming
   }
}
