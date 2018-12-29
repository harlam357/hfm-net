
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Log.Legacy;
using HFM.Log.Internal;

namespace HFM.Log
{
   [TestFixture]
   public class LegacyLogTests
   {
      [Test]
      public void LegacyLog_Clear_Test()
      {
         // Arrange
         var log = new LegacyLog();
         using (var textReader = new StreamReader("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"))
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 30, 149));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 150, 273));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 7, 30, 0, 40, 27, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 274);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 302, 401));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 402, 752));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 7, 31, 0, 7, 43, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.GettingWorkPacket;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(5, unitRun.LogLines[3].Data);
         Assert.AreEqual(2.08f, unitRun.LogLines[10].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 30, 220));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 221, 382));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 40, 5, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.SendingWorkPacket;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual(2.08f, unitRun.LogLines[10].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 231, 384));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 385, 408));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 25, 18, 11, 37, DateTimeKind.Utc);
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
         expectedSlotRunData.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual(2.08f, unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[15].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(WorkUnitResult.FINISHED_UNIT, unitRun.LogLines[137].Data);
      }

      [Test]
      public void LegacyLog_Read_SMP_10_Test() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         var fahLog = LegacyLog.Read("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt");

         // Check Run 5 Positions
         var expectedRun = new ClientRun(null, 401);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, -1, 426, 449));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 12, 11, 13, 20, 57, DateTimeKind.Utc);
         expectedRunData.Arguments = "-configonly";
         expectedRunData.ClientVersion = "6.24R3";
         expectedRunData.FoldingID = "sneakysnowman";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "5D2DCEF06CE524B3";
         expectedRunData.MachineID = 1;
         expectedRun.Data = expectedRunData;
         var expectedSlotRunData = new LegacySlotRunData();
         expectedSlotRunData.CompletedUnits = 0;
         expectedSlotRunData.FailedUnits = 0;
         expectedSlotRunData.TotalCompletedUnits = null;
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(4);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         actualRun = fahLog.ClientRuns.Peek();
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 8 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(1).SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(6, unitRun.LogLines[3].Data);
         Assert.AreEqual(2.10f, unitRun.LogLines[10].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 36, 233));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 234, 283));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 284, 333));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 334, 657));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 658, 707));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 708, 757));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 758, 1081));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 1082, 1146));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 1147, 1218));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 1219, 1268));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 1269, 1340));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 1341, 1435));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 1436, 1537));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 1538, 1587));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 1588, 1637));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 1638, 1709));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 1710, 1759));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 1760, 1824));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 1825, 2148));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 2149, 2198));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 2199, 2417));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 2418, 2489));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 2490, 2539));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 2540, 2589));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 2590, 2913));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 2914, 2963));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 2964, 3013));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 3014, 3352));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 3353, 3447));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 3448, 3497));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 3498, 3644));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 3645, 3709));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 3710, 3759));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 3760, 3792));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 9, 14, 2, 48, 27, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.EuePause;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(0, unitRun.LogLines[3].Data);
         Assert.AreEqual(2.22f, unitRun.LogLines[10].Data);
         Assert.That(unitRun.LogLines[20].ToString().Contains("Project: 6071 (Run 0, Clone 39, Gen 70)"));
         Assert.AreEqual(LogLineType.ClientCoreCommunicationsError, unitRun.LogLines[26].LineType);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 24, 174));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 175, 207));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 20, 7, 52, 34, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(0, unitRun.LogLines[8].Data);
         Assert.AreEqual(2.27f, unitRun.LogLines[15].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 130, 325));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 326, 386));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 387, 448));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 449, 509));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 510, 570));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 571, 617));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 8, 5, 7, 18, DateTimeKind.Utc);
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
         expectedSlotRunData.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 618);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 663, 736));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 737, 934));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 935, 1131));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 1132, 1328));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 1329, 1525));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 1526, 1723));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 1724, 1925));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 1926, 2122));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 2123, 2320));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 2321, 2517));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 2518, 2714));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 2715, 2916));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 2917, 2995));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 8, 6, 18, 28, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual(1.19f, unitRun.LogLines[10].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 34, 207));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 208, 381));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 382, 446));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 14, 4, 40, 2, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(8, unitRun.LogLines[3].Data);
         Assert.AreEqual(1.19f, unitRun.LogLines[8].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 24, 55));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 3, 26, 33, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 56);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 80, 220));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 221, 270));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 271, 319));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 320, 372));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 373, 420));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 421, 463));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 3, 54, 16, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.EuePause;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - UnitRun 3)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.ElementAt(2);
         Assert.AreEqual(9, unitRun.LogLines[3].Data);
         Assert.AreEqual(1.19f, unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[22].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(WorkUnitResult.UNSTABLE_MACHINE, unitRun.LogLines[39].Data);

         // Special Check to be sure the reader is catching the EUE Pause line (Current ClientRun - Current UnitRun)
         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(LogLineType.ClientEuePauseState, unitRun.LogLines[42].LineType);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 24, 82));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 1, 31, 1, 57, 21, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(0, unitRun.LogLines[4].Data);
         Assert.AreEqual(1.31f, unitRun.LogLines[13].Data);
         Assert.That(unitRun.LogLines[26].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual(1.31f, unitRun.Data.CoreVersion);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 27, 169));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 170, 218));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 2, 17, 17, 19, 31, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(3, unitRun.LogLines[7].Data);
         Assert.AreEqual(2.19f, unitRun.LogLines[14].Data);
         Assert.That(unitRun.LogLines[29].ToString().Contains("Project: 10634 (Run 11, Clone 24, Gen 14)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(17, 31, 22), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(12, unitRun.Data.FramesObserved);
         Assert.AreEqual(2.19f, unitRun.Data.CoreVersion);
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
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 15, 30, DateTimeKind.Utc);
         expectedRunData.Arguments = "-configonly";
         expectedRunData.ClientVersion = "6.23";
         expectedRunData.FoldingID = "harlam357";
         expectedRunData.Team = 32;
         expectedRunData.UserID = "4E34332601E26450";
         expectedRunData.MachineID = 5;
         expectedRun.Data = expectedRunData;
         expectedSlotRun.Data = new LegacySlotRunData();

         var actualRun = fahLog.ClientRuns.ElementAt(2);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun(null, 30);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 179, 592));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 593, 838));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 17, 46, DateTimeKind.Utc);
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
         expectedSlotRunData.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.ElementAt(1);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 2 Positions
         expectedRun = new ClientRun(null, 839);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 874, 951));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 20, 4, 17, 29, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 1 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(1).SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].Data);
         Assert.AreEqual(1.90f, unitRun.LogLines[10].Data);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 820, 926));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 24, 0, 28, 52, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Check Run 4 Positions
         expectedRun = new ClientRun(null, 927);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 961, 1014));
         expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 24, 0, 41, 07, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.RunningNoFrameTimes;
         expectedSlotRun.Data = expectedSlotRunData;

         actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(4, unitRun.LogLines[6].Data);
         Assert.AreEqual(23f, unitRun.LogLines[17].Data);
         Assert.That(unitRun.LogLines[2].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[10].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[45].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(0, 41, 7), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual(23f, unitRun.Data.CoreVersion);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 27, 293));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 294, 553));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 554, 813));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 814, 1073));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 1074, 1337));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 1338, 1601));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 1602, 1869));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 1870, 2129));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 2130, 2323));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 10, 15, 48, 32, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.Paused;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(7, unitRun.LogLines[3].Data);
         Assert.AreEqual(1.90f, unitRun.LogLines[8].Data);
         Assert.That(unitRun.LogLines[17].ToString().Contains("Project: 4461 (Run 886, Clone 3, Gen 56)"));

         // Special Check to be sure the reader is catching the Pause For Battery line
         Assert.AreEqual(LogLineType.WorkUnitPausedForBattery, unitRun.LogLines[193].LineType);
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
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 24, 276));
         var expectedRunData = new LegacyClientRunData();
         expectedRunData.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 16, 18, 46, 15, DateTimeKind.Utc);
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
         expectedSlotRunData.Status = LogSlotStatus.Stopped;
         expectedSlotRun.Data = expectedSlotRunData;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(5, unitRun.LogLines[7].Data);
         Assert.AreEqual(2.27f, unitRun.LogLines[14].Data);
         Assert.That(unitRun.LogLines[23].ToString().Contains("Project: 10741 (Run 0, Clone 1996, Gen 3)"));
      }

      // ReSharper restore InconsistentNaming
   }
}
