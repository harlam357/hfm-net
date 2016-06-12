
using System;
using System.IO;
using System.Linq;

using HFM.Core.DataTypes;

using NUnit.Framework;

namespace HFM.Log.Tests
{
   [TestFixture]
   public class LogReader2Tests
   {
      [Test]
      public void SMP_1_FAHlog() // verbosity 9
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_1\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 30, 149));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 150, 273));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 7, 30, 0, 40, 27, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-smp -verbosity 9";
         expectedRun.Data.ClientVersion = "6.24beta";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "5131EA752EB60547";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 261;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun2(null, 274);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 302, 401));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 402, 752));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 7, 31, 0, 7, 43, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-smp -verbosity 9";
         expectedRun.Data.ClientVersion = "6.24beta";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "5131EA752EB60547";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 2;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 263;
         expectedSlotRun.Data.Status = SlotStatus.GettingWorkPacket;

         actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(5, unitRun.LogLines[3].LineData);
         Assert.AreEqual(2.08f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[21].ToString().Contains("Project: 2677 (Run 10, Clone 29, Gen 28)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[79].LineData);
      }

      [Test]
      public void SMP_2_FAHlog() // verbosity 9
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_2\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 30, 220));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 221, 382));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 40, 5, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-smp -verbosity 9";
         expectedRun.Data.ClientVersion = "6.24beta";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "3A49EBB303C19834";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 2;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 292;
         expectedSlotRun.Data.Status = SlotStatus.SendingWorkPacket;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].LineData);
         Assert.AreEqual(2.08f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[17].ToString().Contains("Project: 2677 (Run 10, Clone 49, Gen 38)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[150].LineData);

         // Special Check to be sure the reader is catching the Attempting To Send line (Current ClientRun - Last Unit)
         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(LogLineType.ClientSendStart, unitRun.LogLines[158].LineType);
      }

      [Test]
      public void SMP_3_FAHlog() // verbosity (normal) / Handles Core Download on Startup / notfred's instance
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_3\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 231, 384));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 385, 408));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 25, 18, 11, 37, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-local -forceasm -smp 4";
         expectedRun.Data.ClientVersion = "6.02";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         // verbosity (normal) does not output User ID after requested from server
         // see ClientLogLines indexes 29 & 30
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].LineData);
         Assert.AreEqual(2.08f, unitRun.LogLines[8].LineData);
         Assert.That(unitRun.LogLines[15].ToString().Contains("Project: 2677 (Run 4, Clone 60, Gen 40)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[137].LineData);
      }

      [Test]
      public void SMP_10_FAHlog() // -smp 8 -bigadv verbosity 9 / Corrupted Log Section in Client Run Index 5
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_10\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 5 Positions
         var expectedRun = new ClientRun2(null, 401);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, -1, 426, 449));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 12, 11, 13, 20, 57, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-configonly";
         expectedRun.Data.ClientVersion = "6.24R3";
         expectedRun.Data.FoldingID = "sneakysnowman";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "5D2DCEF06CE524B3";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.ElementAt(4);
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         actualRun = fahLog.ClientRuns.Peek();
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 8 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(1).SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(6, unitRun.LogLines[3].LineData);
         Assert.AreEqual(2.10f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[21].ToString().Contains("Project: 2683 (Run 4, Clone 11, Gen 18)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[53].LineData);
      }

      [Test]
      public void SMP_15_FAHlog() // lots of Client-core communications error
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_15\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
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
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 9, 14, 2, 48, 27, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-smp -verbosity 9";
         expectedRun.Data.ClientVersion = "6.30";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "DC1DAF57D91DF79";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 33;
         expectedSlotRun.Data.TotalCompletedUnits = 617;
         expectedSlotRun.Data.Status = SlotStatus.EuePause;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(0, unitRun.LogLines[3].LineData);
         Assert.AreEqual(2.22f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[20].ToString().Contains("Project: 6071 (Run 0, Clone 39, Gen 70)"));
         Assert.AreEqual(WorkUnitResult.ClientCoreError, unitRun.LogLines[26].LineData);
      }

      [Test]
      public void SMP_17_FAHlog() // v6.23 A4 SMP
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\SMP_17\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 24, 174));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 175, 207));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 20, 7, 52, 34, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-smp -bigadv -betateam -verbosity 9";
         expectedRun.Data.ClientVersion = "6.34";
         expectedRun.Data.FoldingID = "GreyWhiskers";
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = "51EA5C9A7EF9D58E";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 885;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(0, unitRun.LogLines[8].LineData);
         Assert.AreEqual(2.27f, unitRun.LogLines[15].LineData);
         Assert.That(unitRun.LogLines[27].ToString().Contains("Project: 8022 (Run 11, Clone 318, Gen 24)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[106].LineData);
      }

      [Test]
      public void GPU2_1_FAHlog() // verbosity 9
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\GPU2_1\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 130, 325));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 326, 386));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 387, 448));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 449, 509));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 510, 570));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 571, 617));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 8, 5, 7, 18, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-verbosity 9 -local";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "CF185086C102A47";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 5;
         expectedSlotRun.Data.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun2(null, 618);
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
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 8, 6, 18, 28, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-verbosity 9 -local";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "CF185086C102A47";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 11;
         expectedSlotRun.Data.FailedUnits = 1;
         expectedSlotRun.Data.TotalCompletedUnits = 12;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].LineData);
         Assert.AreEqual(1.19f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[24].ToString().Contains("Project: 5771 (Run 12, Clone 109, Gen 805)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[156].LineData);
      }

      [Test]
      public void GPU2_2_FAHlog() // verbosity (normal)
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\GPU2_2\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 34, 207));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 208, 381));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 382, 446));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 14, 4, 40, 2, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "51108B97183EA3DF";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 2;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 4221;
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(8, unitRun.LogLines[3].LineData);
         Assert.AreEqual(1.19f, unitRun.LogLines[8].LineData);
         Assert.That(unitRun.LogLines[22].ToString().Contains("Project: 5751 (Run 8, Clone 205, Gen 527)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[154].LineData);
      }

      [Test]
      public void GPU2_3_FAHlog() // verbosity (normal) / EUE Pause Test
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\GPU2_3\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 24, 55));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 3, 26, 33, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "JollySwagman";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "1D1493BB0A79C9AE";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun2(null, 56);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 6, 80, 220));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 7, 221, 270));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 8, 271, 319));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 9, 320, 372));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 373, 420));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 421, 463));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 3, 54, 16, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "JollySwagman";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "1D1493BB0A79C9AE";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 5;
         expectedSlotRun.Data.TotalCompletedUnits = 224;
         expectedSlotRun.Data.Status = SlotStatus.EuePause;

         actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - UnitRun 3)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.ElementAt(2);
         Assert.AreEqual(9, unitRun.LogLines[3].LineData);
         Assert.AreEqual(1.19f, unitRun.LogLines[8].LineData);
         Assert.That(unitRun.LogLines[22].ToString().Contains("Project: 5756 (Run 6, Clone 32, Gen 480)"));
         Assert.AreEqual(WorkUnitResult.UnstableMachine, unitRun.LogLines[39].LineData);

         // Special Check to be sure the reader is catching the EUE Pause line (Current ClientRun - Current UnitRun)
         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(LogLineType.ClientEuePauseState, unitRun.LogLines[42].LineType);
      }

      [Test]
      public void GPU2_7_FAHlog() // verbosity (normal) / Project String After "+ Processing work unit"
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\GPU2_7\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 24, 82));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 1, 31, 1, 57, 21, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "Zagen30";
         expectedRun.Data.Team = 46301;
         expectedRun.Data.UserID = "xxxxxxxxxxxxxxxxxxx";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 1994;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(0, unitRun.LogLines[4].LineData);
         Assert.AreEqual(1.31f, unitRun.LogLines[13].LineData);
         Assert.That(unitRun.LogLines[26].ToString().Contains("Project: 5781 (Run 2, Clone 700, Gen 2)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(1, 57, 21), unitRun.Data.UnitStartTimeStamp);
         //Assert.AreEqual(5, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual(1.31f, unitRun.Data.CoreVersion);
         Assert.AreEqual(null, unitRun.Data.ProjectInfoIndex);
         Assert.AreEqual(2, unitRun.Data.ProjectInfoList.Count);
         Assert.AreEqual(5781, unitRun.Data.ProjectID);
         Assert.AreEqual(2, unitRun.Data.ProjectRun);
         Assert.AreEqual(700, unitRun.Data.ProjectClone);
         Assert.AreEqual(2, unitRun.Data.ProjectGen);
         Assert.AreEqual(unitRun.Data.ProjectID, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitRun.Data.ProjectRun, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitRun.Data.ProjectClone, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitRun.Data.ProjectGen, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void GPU3_2_FAHlog() // verbosity 9 / OPENMMGPU v2.19
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\GPU3_2\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 3, 27, 169));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 170, 218));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 2, 17, 17, 19, 31, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-gpu 0 -verbosity 9 -local -verbosity 9";
         expectedRun.Data.ClientVersion = "6.41r2";
         expectedRun.Data.FoldingID = "HayesK";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "37114EB5198643C1";
         expectedRun.Data.MachineID = 2;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 847;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(3, unitRun.LogLines[7].LineData);
         Assert.AreEqual(2.19f, unitRun.LogLines[14].LineData);
         Assert.That(unitRun.LogLines[29].ToString().Contains("Project: 10634 (Run 11, Clone 24, Gen 14)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(17, 31, 22), unitRun.Data.UnitStartTimeStamp);
         //Assert.AreEqual(12, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(12, unitRun.Data.FramesObserved);
         Assert.AreEqual(2.19f, unitRun.Data.CoreVersion);
         Assert.AreEqual(null, unitRun.Data.ProjectInfoIndex);
         Assert.AreEqual(1, unitRun.Data.ProjectInfoList.Count);
         Assert.AreEqual(10634, unitRun.Data.ProjectID);
         Assert.AreEqual(8, unitRun.Data.ProjectRun);
         Assert.AreEqual(24, unitRun.Data.ProjectClone);
         Assert.AreEqual(24, unitRun.Data.ProjectGen);
         Assert.AreEqual(unitRun.Data.ProjectID, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitRun.Data.ProjectRun, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitRun.Data.ProjectClone, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitRun.Data.ProjectGen, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void Standard_1_FAHlog() // verbosity 9
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Standard_1\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 15, 30, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-configonly";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "4E34332601E26450";
         expectedRun.Data.MachineID = 5;
         expectedSlotRun.Data = new SlotRunData();

         var actualRun = fahLog.ClientRuns.ElementAt(2);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 1 Positions
         expectedRun = new ClientRun2(null, 30);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 179, 592));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 593, 838));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 18, 2, 17, 46, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-verbosity 9 -forceasm";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "4E34332601E26450";
         expectedRun.Data.MachineID = 5;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null; // TODO: not capturing line "+ Starting local stats count at 1"
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         actualRun = fahLog.ClientRuns.ElementAt(1);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 2 Positions
         expectedRun = new ClientRun2(null, 839);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 874, 951));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 8, 20, 4, 17, 29, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-verbosity 9 -forceasm -oneunit";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "harlam357";
         expectedRun.Data.Team = 32;
         expectedRun.Data.UserID = "4E34332601E26450";
         expectedRun.Data.MachineID = 5;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 2;
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (ClientRun 1 - First UnitRun)
         var unitRun = fahLog.ClientRuns.ElementAt(1).SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(1, unitRun.LogLines[3].LineData);
         Assert.AreEqual(1.90f, unitRun.LogLines[10].LineData);
         Assert.That(unitRun.LogLines[18].ToString().Contains("Project: 4456 (Run 173, Clone 0, Gen 31)"));
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.LogLines[254].LineData);
      }

      [Test]
      public void Standard_5_FAHlog() // verbosity 9
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Standard_5\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 3 Positions
         var expectedRun = new ClientRun2(null, 788);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 820, 926));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 24, 0, 28, 52, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-oneunit -forceasm -verbosity 9";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "borden.b";
         expectedRun.Data.Team = 131;
         expectedRun.Data.UserID = "722723950C6887C2";
         expectedRun.Data.MachineID = 3;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         var actualRun = fahLog.ClientRuns.ElementAt(1);
         DoClientRunCheck(expectedRun, actualRun);

         // Check Run 4 Positions
         expectedRun = new ClientRun2(null, 927);
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 4, 961, 1014));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 24, 0, 41, 07, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-forceasm -verbosity 9 -oneunit";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "borden.b";
         expectedRun.Data.Team = 131;
         expectedRun.Data.UserID = "722723950C6887C2";
         expectedRun.Data.MachineID = 3;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.RunningNoFrameTimes;

         actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(4, unitRun.LogLines[6].LineData);
         Assert.AreEqual(23f, unitRun.LogLines[17].LineData);
         Assert.That(unitRun.LogLines[2].ToString().Contains("Project: 6501 (Run 13, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[10].ToString().Contains("Project: 6501 (Run 15, Clone 0, Gen 0)"));
         Assert.That(unitRun.LogLines[45].ToString().Contains("Project: 10002 (Run 19, Clone 0, Gen 51)"));

         unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(new TimeSpan(0, 41, 7), unitRun.Data.UnitStartTimeStamp);
         //Assert.AreEqual(5, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(5, unitRun.Data.FramesObserved);
         Assert.AreEqual(23f, unitRun.Data.CoreVersion);
         Assert.AreEqual(null, unitRun.Data.ProjectInfoIndex);
         Assert.AreEqual(3, unitRun.Data.ProjectInfoList.Count);
         Assert.AreEqual(10002, unitRun.Data.ProjectID);
         Assert.AreEqual(19, unitRun.Data.ProjectRun);
         Assert.AreEqual(0, unitRun.Data.ProjectClone);
         Assert.AreEqual(51, unitRun.Data.ProjectGen);
         Assert.AreEqual(unitRun.Data.ProjectID, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectID);
         Assert.AreEqual(unitRun.Data.ProjectRun, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectRun);
         Assert.AreEqual(unitRun.Data.ProjectClone, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectClone);
         Assert.AreEqual(unitRun.Data.ProjectGen, unitRun.Data.ProjectInfoList[unitRun.Data.ProjectInfoList.Count - 1].ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void Standard_6_FAHlog() // verbosity normal / Gromacs 3.3
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Standard_6\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 2);
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
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 10, 15, 48, 32, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "DrSpalding";
         expectedRun.Data.Team = 48083;
         expectedRun.Data.UserID = "1E19BD450434A6ED";
         expectedRun.Data.MachineID = 1;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 8;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 229;
         expectedSlotRun.Data.Status = SlotStatus.Paused;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(1).LogLines); // previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (Current ClientRun - Current UnitRun)
         var unitRun = fahLog.ClientRuns.Peek().SlotRuns[0].UnitRuns.Peek();
         Assert.AreEqual(7, unitRun.LogLines[3].LineData);
         Assert.AreEqual(1.90f, unitRun.LogLines[8].LineData);
         Assert.That(unitRun.LogLines[17].ToString().Contains("Project: 4461 (Run 886, Clone 3, Gen 56)"));

         // Special Check to be sure the reader is catching the Pause For Battery line
         Assert.AreEqual(LogLineType.WorkUnitPausedForBattery, unitRun.LogLines[193].LineType);
      }

      [Test]
      public void Standard_9_FAHlog() // v6.23 A4 Uniprocessor
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Standard_9\\FAHlog.txt"), LogFileType.Legacy);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 5, 24, 276));
         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(DateTime.UtcNow.Year, 3, 16, 18, 46, 15, DateTimeKind.Utc);
         expectedRun.Data.Arguments = "-oneunit -verbosity 9";
         expectedRun.Data.ClientVersion = "6.23";
         expectedRun.Data.FoldingID = "Amaruk";
         expectedRun.Data.Team = 50625;
         expectedRun.Data.UserID = "1E53CB2Axxxxxxxx";
         expectedRun.Data.MachineID = 14;
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = 173;
         expectedSlotRun.Data.Status = SlotStatus.Stopped;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         // Verify LogLine Properties
         Assert.AreEqual(1, actualRun.SlotRuns[0].UnitRuns.Count); // no previous
         Assert.IsNotNull(actualRun.SlotRuns[0].UnitRuns.ElementAt(0).LogLines); // current

         // Spot Check Work Unit Data (First ClientRun - First UnitRun)
         var unitRun = fahLog.ClientRuns.Last().SlotRuns[0].UnitRuns.Last();
         Assert.AreEqual(5, unitRun.LogLines[7].LineData);
         Assert.AreEqual(2.27f, unitRun.LogLines[14].LineData);
         Assert.That(unitRun.LogLines[23].ToString().Contains("Project: 10741 (Run 0, Clone 1996, Gen 3)"));
      }

      #region Version 7 Logs

      [Test]
      public void Client_v7_10()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"), LogFileType.FahClient);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 1);
         expectedRun.SlotRuns.Add(1, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 90, 349));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 276, 413));
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 85, 402));
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(2012, 1, 11, 3, 24, 22, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         //Assert.AreEqual(1, logInterpreter.LogLineParsingErrors.Count());

         var projectInfo = new ProjectInfo { ProjectID = 7610, ProjectRun = 630, ProjectClone = 0, ProjectGen = 59 };
         var unitRun = LogInterpreter2.GetUnitRun(actualRun.SlotRuns[0], 1, projectInfo);
         Assert.AreEqual(39, unitRun.LogLines.Count);
         Assert.AreEqual(new TimeSpan(3, 25, 32), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(2.27f, unitRun.Data.CoreVersion);
         //Assert.AreEqual(10, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(10, unitRun.Data.FramesObserved);
         Assert.AreEqual(7610, unitRun.Data.ProjectID);
         Assert.AreEqual(630, unitRun.Data.ProjectRun);
         Assert.AreEqual(0, unitRun.Data.ProjectClone);
         Assert.AreEqual(59, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);

         projectInfo = new ProjectInfo { ProjectID = 5772, ProjectRun = 7, ProjectClone = 364, ProjectGen = 252 };
         unitRun = LogInterpreter2.GetUnitRun(actualRun.SlotRuns[1], 2, projectInfo);
         Assert.AreEqual(98, unitRun.LogLines.Count);
         Assert.AreEqual(new TimeSpan(4, 21, 52), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(1.31f, unitRun.Data.CoreVersion);
         //Assert.AreEqual(53, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(53, unitRun.Data.FramesObserved);
         Assert.AreEqual(5772, unitRun.Data.ProjectID);
         Assert.AreEqual(7, unitRun.Data.ProjectRun);
         Assert.AreEqual(364, unitRun.Data.ProjectClone);
         Assert.AreEqual(252, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);
      }

      [Test]
      public void Client_v7_13()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_13\\log.txt"), LogFileType.FahClient);

         // Check Run 0 Positions
         var expectedRun = new ClientRun2(null, 0);
         var expectedSlotRun = new SlotRun(expectedRun, 1);
         expectedRun.SlotRuns.Add(1, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 74, 212));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 161, 522));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 471, 831));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 780, 1141));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 1090, 1451));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 1400, 1760));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 1709, 2070));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 2019, 2301));
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 7;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 79, 271));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 219, 581));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 529, 890));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 838, 1200));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 1148, 1510));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 1, 1458, 1819));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 0, 1767, 2129));
         expectedSlotRun.UnitRuns.Push(new UnitRun(expectedSlotRun, 2, 2078, 2302));
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 7;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         expectedRun.Data = new ClientRun2Data();
         expectedRun.Data.StartTime = new DateTime(2014, 7, 25, 13, 57, 36, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.Peek();
         DoClientRunCheck(expectedRun, actualRun);

         //Assert.AreEqual(1, logInterpreter.LogLineParsingErrors.Count());

         var projectInfo = new ProjectInfo { ProjectID = 13001, ProjectRun = 430, ProjectClone = 2, ProjectGen = 48 };
         var unitRun = LogInterpreter2.GetUnitRun(actualRun.SlotRuns[0], 2, projectInfo);
         Assert.AreEqual(154, unitRun.LogLines.Count);
         Assert.AreEqual(new TimeSpan(16, 59, 51), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(0.0f, unitRun.Data.CoreVersion);
         //Assert.AreEqual(101, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(101, unitRun.Data.FramesObserved);
         Assert.AreEqual(13001, unitRun.Data.ProjectID);
         Assert.AreEqual(430, unitRun.Data.ProjectRun);
         Assert.AreEqual(2, unitRun.Data.ProjectClone);
         Assert.AreEqual(48, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.FinishedUnit, unitRun.Data.WorkUnitResult);

         projectInfo = new ProjectInfo { ProjectID = 13000, ProjectRun = 671, ProjectClone = 1, ProjectGen = 50 };
         unitRun = LogInterpreter2.GetUnitRun(actualRun.SlotRuns[0], 2, projectInfo);
         Assert.AreEqual(111, unitRun.LogLines.Count);
         Assert.AreEqual(new TimeSpan(4, 41, 56), unitRun.Data.UnitStartTimeStamp);
         Assert.AreEqual(0.0f, unitRun.Data.CoreVersion);
         //Assert.AreEqual(86, unitRun.Data.FrameDataList.Count);
         Assert.AreEqual(86, unitRun.Data.FramesObserved);
         Assert.AreEqual(13000, unitRun.Data.ProjectID);
         Assert.AreEqual(671, unitRun.Data.ProjectRun);
         Assert.AreEqual(1, unitRun.Data.ProjectClone);
         Assert.AreEqual(50, unitRun.Data.ProjectGen);
         Assert.AreEqual(WorkUnitResult.Unknown, unitRun.Data.WorkUnitResult);
      }

      #endregion

      private static void DoClientRunCheck(ClientRun2 expectedRun, ClientRun2 actualRun)
      {
         Assert.AreEqual(expectedRun.ClientStartIndex, actualRun.ClientStartIndex);
         Assert.AreEqual(expectedRun.Data.StartTime, actualRun.Data.StartTime);
         Assert.AreEqual(expectedRun.Data.Arguments, actualRun.Data.Arguments);
         Assert.AreEqual(expectedRun.Data.ClientVersion, actualRun.Data.ClientVersion);
         Assert.AreEqual(expectedRun.Data.FoldingID, actualRun.Data.FoldingID);
         Assert.AreEqual(expectedRun.Data.Team, actualRun.Data.Team);
         Assert.AreEqual(expectedRun.Data.UserID, actualRun.Data.UserID);
         Assert.AreEqual(expectedRun.Data.MachineID, actualRun.Data.MachineID);

         Assert.AreEqual(expectedRun.SlotRuns.Count, actualRun.SlotRuns.Count);
         foreach (int key in expectedRun.SlotRuns.Keys)
         {
            Assert.AreEqual(expectedRun.SlotRuns[key].Data.CompletedUnits, actualRun.SlotRuns[key].Data.CompletedUnits);
            Assert.AreEqual(expectedRun.SlotRuns[key].Data.FailedUnits, actualRun.SlotRuns[key].Data.FailedUnits);
            Assert.AreEqual(expectedRun.SlotRuns[key].Data.TotalCompletedUnits, actualRun.SlotRuns[key].Data.TotalCompletedUnits);
            Assert.AreEqual(expectedRun.SlotRuns[key].Data.Status, actualRun.SlotRuns[key].Data.Status);

            Assert.AreEqual(expectedRun.SlotRuns[key].UnitRuns.Count, actualRun.SlotRuns[key].UnitRuns.Count);
            for (int i = 0; i < expectedRun.SlotRuns[key].UnitRuns.Count; i++)
            {
               Assert.AreEqual(expectedRun.SlotRuns[key].UnitRuns.ElementAt(i).QueueIndex, actualRun.SlotRuns[key].UnitRuns.ElementAt(i).QueueIndex);
               Assert.AreEqual(expectedRun.SlotRuns[key].UnitRuns.ElementAt(i).StartIndex, actualRun.SlotRuns[key].UnitRuns.ElementAt(i).StartIndex);
               Assert.AreEqual(expectedRun.SlotRuns[key].UnitRuns.ElementAt(i).EndIndex, actualRun.SlotRuns[key].UnitRuns.ElementAt(i).EndIndex);
            }
         }
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void FahLog_Read_ArgumentNullException_Test()
      {
         FahLog.Read(null, LogFileType.Legacy);
      }

      [Test]
      public void GPU2_5_UnitInfo()
      {
         var data = UnitInfoLog.Read("..\\..\\..\\TestFiles\\GPU2_5\\unitinfo.txt");
         Assert.AreEqual("p4744_lam5w_300K", data.ProteinName);
         Assert.AreEqual("-", data.ProteinTag);
         Assert.AreEqual(0, data.ProjectID);
         Assert.AreEqual(0, data.ProjectRun);
         Assert.AreEqual(0, data.ProjectClone);
         Assert.AreEqual(0, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 2, 20, 35, 41), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 1, 5, 20, 35, 41), data.DueTime);
         Assert.AreEqual(73, data.Progress);
      }

      [Test]
      public void SMP_10_UnitInfo()
      {
         var data = UnitInfoLog.Read("..\\..\\..\\TestFiles\\SMP_10\\unitinfo.txt");
         Assert.AreEqual("Gromacs", data.ProteinName);
         Assert.AreEqual("P2683R6C12G21", data.ProteinTag);
         Assert.AreEqual(2683, data.ProjectID);
         Assert.AreEqual(6, data.ProjectRun);
         Assert.AreEqual(12, data.ProjectClone);
         Assert.AreEqual(21, data.ProjectGen);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 12, 0, 9, 22), data.DownloadTime);
         Assert.AreEqual(new DateTime(DateTime.Now.Year, 12, 18, 0, 9, 22), data.DueTime);
         Assert.AreEqual(1724900, data.Progress);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void GetUnitInfoLogData_ArgumentNull()
      {
         UnitInfoLog.Read(null);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void GetUnitInfoLogData_ArgumentEmpty()
      {
         UnitInfoLog.Read(String.Empty);
      }

      [Test]
      [ExpectedException(typeof(DirectoryNotFoundException))]
      public void GetUnitInfoLogData_FileDoesNotExist()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\DoesNotExist\\unitinfo.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo1()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo1.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo2()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo2.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo3()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo3.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo4()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo4.txt");
      }

      [Test]
      [ExpectedException(typeof(FormatException))]
      public void Malformed_1_UnitInfo5()
      {
         UnitInfoLog.Read("..\\..\\..\\TestFiles\\Malformed_1\\unitinfo5.txt");
      }
   }
}
