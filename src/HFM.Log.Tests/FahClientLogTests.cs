
using System;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   [TestFixture]
   public class FahClientLogTests
   {
      // ReSharper disable InconsistentNaming

      [Test]
      public void FahClientLog_Read_Client_v7_10_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_10\\log.txt"), FahLogType.FahClient);

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 0);

         // Setup SlotRun 0
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 85, 402);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 25, 32);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 10;
         var expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 7610;
         expectedProjectInfo.ProjectRun = 630;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 59;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 0;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup SlotRun 1
         expectedSlotRun = new SlotRun(expectedRun, 1);
         expectedRun.SlotRuns.Add(1, expectedSlotRun);

         // Setup SlotRun 1 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 90, 349);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 25, 36);
         expectedUnitRun.Data.CoreVersion = 1.31f;
         expectedUnitRun.Data.FramesObserved = 100;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 5767;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 138;
         expectedProjectInfo.ProjectGen = 144;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 276, 413);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 21, 52);
         expectedUnitRun.Data.CoreVersion = 1.31f;
         expectedUnitRun.Data.FramesObserved = 53;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 5772;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 364;
         expectedProjectInfo.ProjectGen = 252;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 1
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 1;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup ClientRunData 0
         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2012, 1, 11, 3, 24, 22, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         FahLogAssert.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, actualRun.Count(x => x.LineType == LogLineType.Error));

      }

      [Test]
      public void FahClientLog_Read_Client_v7_13_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_13\\log.txt"), FahLogType.FahClient);

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 0);

         // Setup SlotRun 1
         var expectedSlotRun = new SlotRun(expectedRun, 1);
         expectedRun.SlotRuns.Add(1, expectedSlotRun);

         // Setup SlotRun 1 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 2, 74, 212);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 57, 36);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 28;
         var expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 416;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 32;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 161, 522);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 44, 55);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 274;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 54;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 471, 831);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 41, 51);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 681;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 51;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 780, 1141);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 38, 53);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 1573;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 38;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1090, 1451);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 36, 43);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 529;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 41;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1400, 1760);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 33, 23);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 715;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 49;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1709, 2070);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 30, 18);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 248;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 51;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2019, 2301);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 26, 26);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 88;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 1719;
         expectedProjectInfo.ProjectClone = 9;
         expectedProjectInfo.ProjectGen = 68;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 1
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 7;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup SlotRun 0
         expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 79, 271);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 57, 36);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 31;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 340;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 36;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 219, 581);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 59, 51);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 430;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 48;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 529, 890);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 57, 4);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 291;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 54;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 838, 1200);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 54, 4);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 1958;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 48;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1148, 1510);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 51, 12);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 509;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 33;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1458, 1819);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 48, 2);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 507;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 49;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1767, 2129);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 44, 44);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13001;
         expectedProjectInfo.ProjectRun = 228;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 62;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2078, 2302);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 41, 56);
         expectedUnitRun.Data.CoreVersion = 0.52f;
         expectedUnitRun.Data.FramesObserved = 86;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 13000;
         expectedProjectInfo.ProjectRun = 671;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 50;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 7;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup ClientRunData 0
         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2014, 7, 25, 13, 57, 36, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         FahLogAssert.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(1, actualRun.Count(x => x.LineType == LogLineType.Error));
      }

      [Test]
      public void FahClientLog_Read_Client_v7_14_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_14\\log.txt"), FahLogType.FahClient);

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 0);

         // Setup SlotRun 1
         var expectedSlotRun = new SlotRun(expectedRun, 1);
         expectedRun.SlotRuns.Add(1, expectedSlotRun);

         // Setup SlotRun 1 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 98, 643);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 47, 0);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 91;
         var expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 14;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 87;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 605, 987);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 20, 45);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 80;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 3;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 948, 1284);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 28, 59);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 171;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1244, 1550);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 55, 55);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9712;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 167;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1509, 1838);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 47, 52);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 88;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 170;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1801, 2094);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 48, 25);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 504;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2054, 2355);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 15, 47);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2316, 2436);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 37, 49);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 36;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 17;
         expectedProjectInfo.ProjectGen = 67;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2437, 2597);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 23, 26);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 53;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2598, 2860);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 23, 32);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 18;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 86;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 10
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2821, 3125);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 21, 22);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 18;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 11
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3085, 3329);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 20, 13);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 243;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 12
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3292, 3384);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 57, 19);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 38;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 13
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3385, 3752);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 57, 25);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 57;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 14
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3712, 4035);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 59, 8);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 15
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3997, 4117);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 22, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11410;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 16
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4118, 4522);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 22, 29);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 17
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4482, 4910);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 24, 30);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 15;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 18
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4870, 5183);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 26, 12);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 9;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5143, 5455);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 24, 55);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 36;
         expectedProjectInfo.ProjectClone = 7;
         expectedProjectInfo.ProjectGen = 79;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 20
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5415, 5565);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 51, 20);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 41;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 8;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 21
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5562, 5771);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 3, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 53;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 22
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 5772, 6113);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 3, 29);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 22;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 23
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6075, 6551);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 5, 11);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11410;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 25;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6511, 6937);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 1, 34);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11411;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 34;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 25
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6897, 6986);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 26, 40);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 11;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 22;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 65;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 26
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6987, 7417);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 26, 45);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 10;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 27
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7376, 7767);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 28, 31);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 45;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 153;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 28
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 7727, 8058);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 29, 45);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 256;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 29
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8021, 8468);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 31, 15);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11410;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 30
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8430, 8512);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 27, 24);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 12;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 13;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 24;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 31
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8506, 8827);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 50, 30);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 12;
         expectedProjectInfo.ProjectClone = 44;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8786, 9081);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 55, 33);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 31;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 33
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9041, 9365);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 7, 39);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 9;
         expectedProjectInfo.ProjectClone = 28;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 34
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9327, 9690);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 19, 56);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 7;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 35
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9650, 9928);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 18, 18);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 43;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 36
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9893, 10116);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 8, 13);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 40;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 37
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10079, 10384);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 57, 46);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 77;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 38
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10346, 10767);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 55, 31);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 12;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 75;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 39
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 10727, 10947);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 41, 55);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9152;
         expectedProjectInfo.ProjectRun = 15;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 40
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10909, 11465);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 8, 10);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9212;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 50;
         expectedProjectInfo.ProjectGen = 38;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 41
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11424, 11639);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 14, 3);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 119;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 114;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 42
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11603, 11712);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 9, 36);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 33;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 79;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 43
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11705, 12288);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 9, 51);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 31;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 44
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12245, 12511);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 8, 21);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 45
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12473, 12894);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 28, 57);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 55;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 44;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 46
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12895, 13205);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 18, 36);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 47
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13173, 13488);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 18, 45);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 29;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 48
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 13450, 13747);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 53, 8);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 68;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 49
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13688, 14009);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 28, 21);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 30;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 50
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13970, 14320);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 20, 43);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 10;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 51
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 14279, 14810);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 16, 32);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 74;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 52
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14772, 14951);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 8, 36);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 22;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 53
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 14913, 15092);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 9, 45);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 54
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15053, 15248);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 10, 50);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 55
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15193, 15370);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 11, 54);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 56
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15332, 15525);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 12, 47);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 22;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 57
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15472, 15650);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 13, 55);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 58
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15612, 15789);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 14, 45);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 59
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 15761, 15927);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 15, 58);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 60
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15901, 16067);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 17, 14);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 61
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16041, 16209);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 18, 33);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 62
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16171, 16349);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 19, 28);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 22;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 63
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16310, 16502);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 20, 35);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 64
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16449, 16643);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 21, 42);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 65
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16589, 16764);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 22, 34);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 66
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16738, 16906);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 23, 31);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 13;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 67
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 16868, 17044);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 24, 39);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 68
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17018, 17185);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 25, 55);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 69
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17158, 17326);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 27, 7);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 70
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17298, 17465);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 28, 25);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 71
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17439, 17625);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 29, 33);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 72
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17569, 17764);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 30, 30);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 73
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 17737, 17923);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 31, 45);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 74
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17867, 18043);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 32, 58);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 14;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 75
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18017, 18186);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 33, 55);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 76
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18147, 18323);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 34, 47);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 77
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18296, 18465);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 35, 59);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 78
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18426, 18605);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 37, 13);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 79
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18567, 18762);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 38, 23);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 80
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18707, 18885);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 39, 31);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 15;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 81
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 18846, 18995);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 40, 25);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 82
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18996, 19178);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 41, 38);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 15;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 83
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19123, 19318);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 42, 47);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 13;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 84
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19262, 19440);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 43, 38);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 85
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19402, 19581);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 44, 32);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 86
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19542, 19721);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 45, 45);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 87
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19681, 19857);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 46, 47);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 88
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19831, 20000);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 48, 0);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 89
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 19961, 20140);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 49, 5);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 16;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 90
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20101, 20279);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 50, 5);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 91
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20240, 20419);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 51, 10);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 16;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 92
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20380, 20574);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 52, 16);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 93
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20520, 20714);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 53, 23);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 94
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20659, 20837);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 54, 15);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 13;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 95
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 20799, 20977);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 55, 9);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 96
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20939, 21133);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 56, 21);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 16;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 97
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21078, 21257);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 57, 32);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 98
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21218, 21396);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 58, 24);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 99
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21358, 21566);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 59, 26);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 100
         expectedUnitRun = new UnitRun(expectedSlotRun, 4, 21512, 21723);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 0, 40);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 101
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21696, 22021);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 7, 1);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 9;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 102
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 21963, 22162);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 8, 6);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 16;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 103
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22139, 22429);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 14, 28);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 104
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 22417, 22715);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 20, 54);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 12;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 105
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22676, 22847);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 22, 0);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 3;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 106
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22888, 22954);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 0;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 1
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 95;
         expectedSlotRun.Data.FailedUnits = 11;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup SlotRun 2
         expectedSlotRun = new SlotRun(expectedRun, 2);
         expectedRun.SlotRuns.Add(2, expectedSlotRun);

         // Setup SlotRun 2 - UnitRun 0
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 113, 296);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 50, 34);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 83;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 7;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 293, 599);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 50, 39);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 15;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 125;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 558, 827);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 15, 50);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9151;
         expectedProjectInfo.ProjectRun = 10;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 791, 1097);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 47, 24);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 350;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1060, 1405);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 47, 26);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 8;
         expectedProjectInfo.ProjectClone = 7;
         expectedProjectInfo.ProjectGen = 247;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 1367, 1690);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 12, 22);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 12;
         expectedProjectInfo.ProjectGen = 205;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1650, 1976);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 37, 54);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 215;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 3;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1939, 2294);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 37, 41);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 2256, 2744);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 36, 45);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 18;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2703, 3057);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 33, 58);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 61;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 10
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3017, 3478);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 36, 7);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 27;
         expectedProjectInfo.ProjectGen = 15;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 11
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3438, 3700);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 45, 57);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 12
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3662, 3983);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 42, 51);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 26;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 13
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3945, 4281);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 44, 39);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 28;
         expectedProjectInfo.ProjectClone = 18;
         expectedProjectInfo.ProjectGen = 61;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 14
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4241, 4392);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 54, 52);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 46;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 15
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4393, 4700);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 54, 56);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 14;
         expectedProjectInfo.ProjectGen = 80;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 16
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 4626, 4671);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 51, 53);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 68;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 17
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 4672, 4769);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 12, 29);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 18
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4770, 5094);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 12, 35);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 5;
         expectedProjectInfo.ProjectClone = 38;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5053, 5370);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 21, 56);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 158;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 26;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 20
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 5333, 5720);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 21, 54);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 47;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 21
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5680, 5987);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 31, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 16;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 67;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 22
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5947, 6252);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 27, 56);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 45;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 23
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 6212, 6434);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 29, 44);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 256;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 9;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6397, 6659);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 29, 19);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 17;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 18;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 25
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6621, 6736);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 4, 32);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 31;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 16;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 26
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6737, 7077);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 4, 40);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 350;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 17;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 27
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7040, 7266);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 4, 15);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 49;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 28
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7226, 7522);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 50, 30);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 92;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 125;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 29
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 7486, 7673);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 50, 15);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 81;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 18;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 30
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7674, 7949);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 2, 44);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 9;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 60;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 31
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7909, 8223);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 48, 48);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 43;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8183, 8409);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 58, 24);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 80;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 33
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8369, 8700);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 54, 32);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 177;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 17;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 34
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 8663, 9029);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 53, 52);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 35
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8989, 9309);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 48, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 30;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 36
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9269, 9571);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 47, 22);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 10;
         expectedProjectInfo.ProjectClone = 34;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 37
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 9531, 9880);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 57, 14);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 13;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 38
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9835, 10279);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 54, 1);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 29;
         expectedProjectInfo.ProjectClone = 8;
         expectedProjectInfo.ProjectGen = 4;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 39
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 10241, 10570);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 52, 57);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 40
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10530, 11051);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 38, 34);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 13;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 41
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11015, 11215);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 37, 27);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 77;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 42
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11178, 11798);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 26, 51);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9212;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 77;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 43
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11760, 11973);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 32, 4);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 12;
         expectedProjectInfo.ProjectClone = 46;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 44
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11935, 12164);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 22, 8);
         expectedUnitRun.Data.CoreVersion = 0.4f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10490;
         expectedProjectInfo.ProjectRun = 182;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 14;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 45
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12127, 12221);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 21, 46);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 26;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 30;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 46
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12215, 12584);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 7, 3);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 31;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 47
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12546, 12986);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 2, 35);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 106;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 48
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12946, 13288);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 56, 30);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11401;
         expectedProjectInfo.ProjectRun = 26;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 49
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13251, 13580);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 42, 59);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 16;
         expectedProjectInfo.ProjectClone = 46;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 50
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13540, 13935);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 33, 19);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 15;
         expectedProjectInfo.ProjectClone = 21;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 51
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 13895, 14169);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 37, 12);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 71;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 38;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 52
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14170, 14422);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 37, 23);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 19;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 53
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14404, 21577);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 26, 58);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 23;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 54
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21504, 22974);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 27, 17);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 27;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 2
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 46;
         expectedSlotRun.Data.FailedUnits = 9;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup ClientRunData 0
         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2015, 12, 8, 12, 44, 41, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         FahLogAssert.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(0, actualRun.Count(x => x.LineType == LogLineType.Error));
      }

      // ReSharper restore InconsistentNaming
   }
}
