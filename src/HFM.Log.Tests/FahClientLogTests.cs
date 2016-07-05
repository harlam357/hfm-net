
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
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2316, 2408);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 14, 35);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 36;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 17;
         expectedProjectInfo.ProjectGen = 67;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2409, 2436);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 37, 49);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 17;
         expectedProjectInfo.ProjectGen = 67;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2437, 2569);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 37, 54);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 53;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 10
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2570, 2597);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 23, 26);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 24;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 11
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

         // Setup SlotRun 1 - UnitRun 12
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

         // Setup SlotRun 1 - UnitRun 13
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

         // Setup SlotRun 1 - UnitRun 14
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3292, 3356);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 21, 31);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 38;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 15
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 3357, 3384);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 57, 19);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 38;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 16
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

         // Setup SlotRun 1 - UnitRun 17
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

         // Setup SlotRun 1 - UnitRun 18
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3997, 4089);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 1, 6);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11410;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4090, 4117);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 22, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11410;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 15;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 20
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

         // Setup SlotRun 1 - UnitRun 21
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

         // Setup SlotRun 1 - UnitRun 22
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

         // Setup SlotRun 1 - UnitRun 23
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

         // Setup SlotRun 1 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5415, 5536);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 22, 30);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 41;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 8;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 25
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5537, 5565);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 51, 20);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 8;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 26
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5562, 5743);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 51, 33);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 53;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 27
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5744, 5771);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 3, 23);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 28
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

         // Setup SlotRun 1 - UnitRun 29
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

         // Setup SlotRun 1 - UnitRun 30
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

         // Setup SlotRun 1 - UnitRun 31
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6897, 6958);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 2, 43);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 11;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 22;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 65;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6959, 6986);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 26, 40);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10495;
         expectedProjectInfo.ProjectRun = 22;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 65;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 33
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

         // Setup SlotRun 1 - UnitRun 34
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

         // Setup SlotRun 1 - UnitRun 35
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

         // Setup SlotRun 1 - UnitRun 36
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

         // Setup SlotRun 1 - UnitRun 37
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

         // Setup SlotRun 1 - UnitRun 38
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

         // Setup SlotRun 1 - UnitRun 39
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

         // Setup SlotRun 1 - UnitRun 40
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

         // Setup SlotRun 1 - UnitRun 41
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

         // Setup SlotRun 1 - UnitRun 42
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

         // Setup SlotRun 1 - UnitRun 43
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

         // Setup SlotRun 1 - UnitRun 44
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

         // Setup SlotRun 1 - UnitRun 45
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10346, 10385);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 55, 17);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 12;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 75;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 46
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10386, 10767);
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

         // Setup SlotRun 1 - UnitRun 47
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

         // Setup SlotRun 1 - UnitRun 48
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

         // Setup SlotRun 1 - UnitRun 49
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

         // Setup SlotRun 1 - UnitRun 50
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11603, 11681);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 15, 11);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 33;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 79;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 51
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11682, 11712);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 9, 36);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9201;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 79;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 52
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11705, 11732);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 9, 41);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 31;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 53
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 11733, 12288);
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

         // Setup SlotRun 1 - UnitRun 54
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

         // Setup SlotRun 1 - UnitRun 55
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12473, 12697);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 7, 0);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 47;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 44;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 56
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12752, 12866);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 2, 18);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 8;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 44;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 57
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 12867, 12894);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 28, 57);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11403;
         expectedProjectInfo.ProjectRun = 7;
         expectedProjectInfo.ProjectClone = 44;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 58
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12895, 13177);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 29, 4);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 59
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

         // Setup SlotRun 1 - UnitRun 60
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13178, 13205);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 18, 36);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11402;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 61
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

         // Setup SlotRun 1 - UnitRun 62
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13688, 13722);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 28, 18);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 30;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 63
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13723, 14009);
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

         // Setup SlotRun 1 - UnitRun 64
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

         // Setup SlotRun 1 - UnitRun 65
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

         // Setup SlotRun 1 - UnitRun 66
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

         // Setup SlotRun 1 - UnitRun 67
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

         // Setup SlotRun 1 - UnitRun 68
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

         // Setup SlotRun 1 - UnitRun 69
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

         // Setup SlotRun 1 - UnitRun 70
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

         // Setup SlotRun 1 - UnitRun 71
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

         // Setup SlotRun 1 - UnitRun 72
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

         // Setup SlotRun 1 - UnitRun 73
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

         // Setup SlotRun 1 - UnitRun 74
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

         // Setup SlotRun 1 - UnitRun 75
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

         // Setup SlotRun 1 - UnitRun 76
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

         // Setup SlotRun 1 - UnitRun 77
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

         // Setup SlotRun 1 - UnitRun 78
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

         // Setup SlotRun 1 - UnitRun 79
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

         // Setup SlotRun 1 - UnitRun 80
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

         // Setup SlotRun 1 - UnitRun 81
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

         // Setup SlotRun 1 - UnitRun 82
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

         // Setup SlotRun 1 - UnitRun 83
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

         // Setup SlotRun 1 - UnitRun 84
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

         // Setup SlotRun 1 - UnitRun 85
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

         // Setup SlotRun 1 - UnitRun 86
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17569, 17603);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 30, 25);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11501;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 10;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 1 - UnitRun 87
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17604, 17764);
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

         // Setup SlotRun 1 - UnitRun 88
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

         // Setup SlotRun 1 - UnitRun 89
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

         // Setup SlotRun 1 - UnitRun 90
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

         // Setup SlotRun 1 - UnitRun 91
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

         // Setup SlotRun 1 - UnitRun 92
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

         // Setup SlotRun 1 - UnitRun 93
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

         // Setup SlotRun 1 - UnitRun 94
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

         // Setup SlotRun 1 - UnitRun 95
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

         // Setup SlotRun 1 - UnitRun 96
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

         // Setup SlotRun 1 - UnitRun 97
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

         // Setup SlotRun 1 - UnitRun 98
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

         // Setup SlotRun 1 - UnitRun 99
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

         // Setup SlotRun 1 - UnitRun 100
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

         // Setup SlotRun 1 - UnitRun 101
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

         // Setup SlotRun 1 - UnitRun 102
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

         // Setup SlotRun 1 - UnitRun 103
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

         // Setup SlotRun 1 - UnitRun 104
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

         // Setup SlotRun 1 - UnitRun 105
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

         // Setup SlotRun 1 - UnitRun 106
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

         // Setup SlotRun 1 - UnitRun 107
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

         // Setup SlotRun 1 - UnitRun 108
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

         // Setup SlotRun 1 - UnitRun 109
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

         // Setup SlotRun 1 - UnitRun 110
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

         // Setup SlotRun 1 - UnitRun 111
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

         // Setup SlotRun 1 - UnitRun 112
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

         // Setup SlotRun 1 - UnitRun 113
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

         // Setup SlotRun 1 - UnitRun 114
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

         // Setup SlotRun 1 - UnitRun 115
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

         // Setup SlotRun 1 - UnitRun 116
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

         // Setup SlotRun 1 - UnitRun 117
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

         // Setup SlotRun 1 - UnitRun 118
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

         // Setup SlotRun 1 - UnitRun 119
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

         // Setup SlotRun 1 - UnitRun 120
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

         // Setup SlotRun 1 - UnitRun 121
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
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 113, 128);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 47, 2);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 83;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 7;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 129, 146);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 47, 11);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 83;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 7;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 197, 212);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 48, 11);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 83;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 7;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 213, 228);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 49, 11);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10496;
         expectedProjectInfo.ProjectRun = 83;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 7;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 283, 296);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 50, 34);
         expectedUnitRun.Data.CoreVersion = 0f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 0;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 5
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

         // Setup SlotRun 2 - UnitRun 6
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

         // Setup SlotRun 2 - UnitRun 7
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

         // Setup SlotRun 2 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1060, 1098);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 47, 7);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9704;
         expectedProjectInfo.ProjectRun = 8;
         expectedProjectInfo.ProjectClone = 7;
         expectedProjectInfo.ProjectGen = 247;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1099, 1405);
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

         // Setup SlotRun 2 - UnitRun 10
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

         // Setup SlotRun 2 - UnitRun 11
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

         // Setup SlotRun 2 - UnitRun 12
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

         // Setup SlotRun 2 - UnitRun 13
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

         // Setup SlotRun 2 - UnitRun 14
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

         // Setup SlotRun 2 - UnitRun 15
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

         // Setup SlotRun 2 - UnitRun 16
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

         // Setup SlotRun 2 - UnitRun 17
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

         // Setup SlotRun 2 - UnitRun 18
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

         // Setup SlotRun 2 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4241, 4364);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 41, 11);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 46;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 20
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4365, 4392);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 54, 52);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11400;
         expectedProjectInfo.ProjectRun = 11;
         expectedProjectInfo.ProjectClone = 5;
         expectedProjectInfo.ProjectGen = 0;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 21
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

         // Setup SlotRun 2 - UnitRun 22
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

         // Setup SlotRun 2 - UnitRun 23
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 4672, 4741);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 52, 2);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 16;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 3, 4742, 4769);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 12, 29);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10493;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 19;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 25
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

         // Setup SlotRun 2 - UnitRun 26
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

         // Setup SlotRun 2 - UnitRun 27
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

         // Setup SlotRun 2 - UnitRun 28
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

         // Setup SlotRun 2 - UnitRun 29
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

         // Setup SlotRun 2 - UnitRun 30
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

         // Setup SlotRun 2 - UnitRun 31
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

         // Setup SlotRun 2 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6621, 6708);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 14, 26);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 31;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 16;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 33
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6709, 6736);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 4, 32);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 16;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 34
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

         // Setup SlotRun 2 - UnitRun 35
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

         // Setup SlotRun 2 - UnitRun 36
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

         // Setup SlotRun 2 - UnitRun 37
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

         // Setup SlotRun 2 - UnitRun 38
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

         // Setup SlotRun 2 - UnitRun 39
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

         // Setup SlotRun 2 - UnitRun 40
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

         // Setup SlotRun 2 - UnitRun 41
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

         // Setup SlotRun 2 - UnitRun 42
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

         // Setup SlotRun 2 - UnitRun 43
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

         // Setup SlotRun 2 - UnitRun 44
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

         // Setup SlotRun 2 - UnitRun 45
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

         // Setup SlotRun 2 - UnitRun 46
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

         // Setup SlotRun 2 - UnitRun 47
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

         // Setup SlotRun 2 - UnitRun 48
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

         // Setup SlotRun 2 - UnitRun 49
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

         // Setup SlotRun 2 - UnitRun 50
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

         // Setup SlotRun 2 - UnitRun 51
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

         // Setup SlotRun 2 - UnitRun 52
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

         // Setup SlotRun 2 - UnitRun 53
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

         // Setup SlotRun 2 - UnitRun 54
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

         // Setup SlotRun 2 - UnitRun 55
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12546, 12698);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 6, 17);
         expectedUnitRun.Data.CoreVersion = 0.14f;
         expectedUnitRun.Data.FramesObserved = 70;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 56
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12775, 12986);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 2, 35);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 36;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9852;
         expectedProjectInfo.ProjectRun = 3;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 11;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 57
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

         // Setup SlotRun 2 - UnitRun 58
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

         // Setup SlotRun 2 - UnitRun 59
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

         // Setup SlotRun 2 - UnitRun 60
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 13895, 14141);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 22, 16);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 71;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 38;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 61
         expectedUnitRun = new UnitRun(expectedSlotRun, 2, 14142, 14169);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 37, 12);
         expectedUnitRun.Data.CoreVersion = 0.16f;
         expectedUnitRun.Data.FramesObserved = 0;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10494;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 38;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.BadWorkUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 2 - UnitRun 62
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

         // Setup SlotRun 2 - UnitRun 63
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

         // Setup SlotRun 2 - UnitRun 64
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21504, 21578);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 0, 44);
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

         // Setup SlotRun 2 - UnitRun 65
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21579, 21615);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 0, 59);
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

         // Setup SlotRun 2 - UnitRun 66
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21716, 21736);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 1, 59);
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

         // Setup SlotRun 2 - UnitRun 67
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21742, 21759);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 2, 59);
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

         // Setup SlotRun 2 - UnitRun 68
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21760, 21777);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 3, 59);
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

         // Setup SlotRun 2 - UnitRun 69
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21783, 21800);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 4, 59);
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

         // Setup SlotRun 2 - UnitRun 70
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21801, 21818);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 5, 59);
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

         // Setup SlotRun 2 - UnitRun 71
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21819, 21863);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 6, 59);
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

         // Setup SlotRun 2 - UnitRun 72
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21970, 22016);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 7, 59);
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

         // Setup SlotRun 2 - UnitRun 73
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22098, 22155);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 8, 59);
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

         // Setup SlotRun 2 - UnitRun 74
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22163, 22185);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 9, 59);
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

         // Setup SlotRun 2 - UnitRun 75
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22186, 22203);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 10, 59);
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

         // Setup SlotRun 2 - UnitRun 76
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22209, 22226);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 11, 59);
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

         // Setup SlotRun 2 - UnitRun 77
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22227, 22244);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 12, 59);
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

         // Setup SlotRun 2 - UnitRun 78
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22245, 22262);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 13, 59);
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

         // Setup SlotRun 2 - UnitRun 79
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22310, 22361);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 14, 59);
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

         // Setup SlotRun 2 - UnitRun 80
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22430, 22447);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 15, 59);
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

         // Setup SlotRun 2 - UnitRun 81
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22453, 22470);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 16, 59);
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

         // Setup SlotRun 2 - UnitRun 82
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22471, 22488);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 17, 59);
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

         // Setup SlotRun 2 - UnitRun 83
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22494, 22511);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 18, 59);
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

         // Setup SlotRun 2 - UnitRun 84
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22512, 22529);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 19, 59);
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

         // Setup SlotRun 2 - UnitRun 85
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22569, 22615);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 21, 19);
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

         // Setup SlotRun 2 - UnitRun 86
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22716, 22766);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 22, 19);
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

         // Setup SlotRun 2 - UnitRun 87
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22848, 22865);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 23, 19);
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

         // Setup SlotRun 2 - UnitRun 88
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22866, 22883);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 24, 19);
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

         // Setup SlotRun 2 - UnitRun 89
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22957, 22974);
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
         expectedSlotRun.Data.FailedUnits = 8;
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

      [Test]
      public void FahClientLog_Read_Client_v7_15_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_15\\log.txt"), FahLogType.FahClient);

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 0);

         // Setup SlotRun 0
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 72, 132);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 1, 31);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 30;
         var expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11633;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.UnknownEnum;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 133, 312);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 17, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 69;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11633;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 33;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 232, 467);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 35, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 867;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 113;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 411, 647);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 34, 47);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11636;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 69;
         expectedProjectInfo.ProjectGen = 31;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 568, 807);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 18, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10197;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 16;
         expectedProjectInfo.ProjectGen = 39;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 746, 971);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 54, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 779;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 981;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 908, 1150);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 13, 20);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11626;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 168;
         expectedProjectInfo.ProjectGen = 20;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1072, 1330);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 26, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11636;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 92;
         expectedProjectInfo.ProjectGen = 15;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1251, 1493);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 54, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9762;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 25;
         expectedProjectInfo.ProjectGen = 166;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1430, 1650);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 48, 23);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 23;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 73;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 10
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1594, 1803);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 38, 21);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6396;
         expectedProjectInfo.ProjectRun = 18;
         expectedProjectInfo.ProjectClone = 23;
         expectedProjectInfo.ProjectGen = 248;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 11
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1750, 1985);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 4, 20);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11626;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 42;
         expectedProjectInfo.ProjectGen = 25;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 12
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1906, 2137);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 18, 39);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6392;
         expectedProjectInfo.ProjectRun = 30;
         expectedProjectInfo.ProjectClone = 31;
         expectedProjectInfo.ProjectGen = 288;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 13
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2084, 2319);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 8, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11634;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 204;
         expectedProjectInfo.ProjectGen = 24;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 14
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2240, 2472);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 21, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6393;
         expectedProjectInfo.ProjectRun = 18;
         expectedProjectInfo.ProjectClone = 38;
         expectedProjectInfo.ProjectGen = 194;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 15
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2419, 2630);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 17, 23);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 78;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 96;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 16
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2574, 2787);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 3, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 592;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 125;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 17
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2731, 2968);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 46, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11641;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 317;
         expectedProjectInfo.ProjectGen = 8;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 18
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2888, 3120);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 13, 7);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6398;
         expectedProjectInfo.ProjectRun = 65;
         expectedProjectInfo.ProjectClone = 27;
         expectedProjectInfo.ProjectGen = 285;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3067, 3283);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 39, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10197;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 32;
         expectedProjectInfo.ProjectGen = 30;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 20
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3223, 3439);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 16, 40);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 60;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 34;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 21
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3383, 3592);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 5, 18);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6393;
         expectedProjectInfo.ProjectRun = 25;
         expectedProjectInfo.ProjectClone = 28;
         expectedProjectInfo.ProjectGen = 150;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 22
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3539, 3751);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 32, 2);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 26;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 98;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 23
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3695, 3907);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 17, 10);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 79;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 55;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3851, 4070);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 36, 51);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9761;
         expectedProjectInfo.ProjectRun = 86;
         expectedProjectInfo.ProjectClone = 26;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 25
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4007, 4224);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 6, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6391;
         expectedProjectInfo.ProjectRun = 111;
         expectedProjectInfo.ProjectClone = 34;
         expectedProjectInfo.ProjectGen = 82;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 26
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4171, 4383);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 49, 57);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 205;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 79;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 27
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4326, 4538);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 36, 5);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6395;
         expectedProjectInfo.ProjectRun = 28;
         expectedProjectInfo.ProjectClone = 10;
         expectedProjectInfo.ProjectGen = 135;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 28
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4484, 4703);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 1, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 414;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 1025;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 29
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4640, 4860);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 18, 59);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 102;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 30
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4803, 5024);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 3, 51);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 514;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 998;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 31
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4961, 5180);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 20, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 683;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 69;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5124, 5336);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 7, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 538;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 71;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 33
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5280, 5490);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 53, 50);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 8600;
         expectedProjectInfo.ProjectRun = 121;
         expectedProjectInfo.ProjectClone = 13;
         expectedProjectInfo.ProjectGen = 312;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 34
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5437, 5648);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 19, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 805;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 91;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 35
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5592, 5829);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 30, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11635;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 664;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 36
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5749, 5993);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 23, 22);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 2534;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 701;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 37
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5929, 6156);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 40, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 1080;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 934;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 38
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6093, 6312);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 57, 31);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 246;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 39
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6256, 6493);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 40, 34);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11641;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 588;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 40
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6413, 6648);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 6, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 384;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 1;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 41
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6592, 6827);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 54, 44);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11625;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 896;
         expectedProjectInfo.ProjectGen = 2;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 42
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6749, 7008);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 40, 34);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11641;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 608;
         expectedProjectInfo.ProjectGen = 5;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 43
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6928, 7171);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 54, 52);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9752;
         expectedProjectInfo.ProjectRun = 424;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 975;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 44
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7107, 7324);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 10, 54);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6393;
         expectedProjectInfo.ProjectRun = 124;
         expectedProjectInfo.ProjectClone = 20;
         expectedProjectInfo.ProjectGen = 115;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 45
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7271, 7483);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 36, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 184;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 131;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 46
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7427, 7636);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 35, 3);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6392;
         expectedProjectInfo.ProjectRun = 20;
         expectedProjectInfo.ProjectClone = 26;
         expectedProjectInfo.ProjectGen = 172;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 47
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7583, 7795);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 2, 22);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 241;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 80;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 48
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7739, 7976);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 49, 25);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11636;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 89;
         expectedProjectInfo.ProjectGen = 27;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 49
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7896, 8128);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 30, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6394;
         expectedProjectInfo.ProjectRun = 58;
         expectedProjectInfo.ProjectClone = 26;
         expectedProjectInfo.ProjectGen = 222;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 50
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8075, 8283);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 56, 54);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 6398;
         expectedProjectInfo.ProjectRun = 2;
         expectedProjectInfo.ProjectClone = 20;
         expectedProjectInfo.ProjectGen = 319;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 51
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8230, 8444);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 22, 57);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11643;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 9;
         expectedProjectInfo.ProjectGen = 21;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 52
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8386, 8601);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 12, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 112;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 80;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 53
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8545, 8665);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 59, 10);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 62;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11626;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 886;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.UnknownEnum;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 54
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8666, 8816);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 25, 11);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 40;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11626;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 886;
         expectedProjectInfo.ProjectGen = 6;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 55
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8736, 8997);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 57, 0);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 11642;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 95;
         expectedProjectInfo.ProjectGen = 26;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 56
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8917, 9016);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 30, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 21;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 10197;
         expectedProjectInfo.ProjectRun = 0;
         expectedProjectInfo.ProjectClone = 11;
         expectedProjectInfo.ProjectGen = 32;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 54;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup ClientRunData 0
         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2016, 3, 5, 5, 0, 47, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         FahLogAssert.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(2, actualRun.Count(x => x.LineType == LogLineType.Error));
      }

      [Test]
      public void FahClientLog_Read_Client_v7_16_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_16\\log.txt"), FahLogType.FahClient);

         // Setup ClientRun 0
         var expectedRun = new ClientRun(null, 0);

         // Setup SlotRun 0
         var expectedSlotRun = new SlotRun(expectedRun, 0);
         expectedRun.SlotRuns.Add(0, expectedSlotRun);

         // Setup SlotRun 0 - UnitRun 0
         var expectedUnitRun = new UnitRun(expectedSlotRun, 1, 73, 192);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 51, 3);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 39;
         var expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 587;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 266;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 1
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 142, 347);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 35, 32);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 17;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 245;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 2
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 294, 502);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 14, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 233;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 160;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 3
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 449, 660);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 52, 41);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 61;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 258;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 4
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 604, 814);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 35, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 144;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 226;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 5
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 761, 969);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 11, 44);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 723;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 240;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 6
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 916, 1121);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 55, 6);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 693;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 232;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 7
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1071, 1277);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 34, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 875;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 209;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 8
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1224, 1432);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 13, 4);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 479;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 233;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 9
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1379, 1584);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 54, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 802;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 155;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 10
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1534, 1739);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 33, 13);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 346;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 78;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 11
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1686, 1895);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 12, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 424;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 150;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 12
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 1842, 2052);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 49, 25);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 172;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 205;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 13
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 1997, 2205);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 28, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 59;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 130;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 14
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2152, 2360);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 8, 17);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 60;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 220;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 15
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2307, 2516);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 50, 44);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 606;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 253;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 16
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2463, 2671);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 31, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 559;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 225;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 17
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2618, 2826);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 13, 7);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 633;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 184;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 18
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 2773, 2982);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 51, 23);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 159;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 360;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 19
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 2929, 3137);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 34, 5);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 796;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 89;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 20
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3084, 3292);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 16, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 519;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 254;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 21
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3239, 3447);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 59, 13);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 615;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 227;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 22
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3394, 3603);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 39, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 457;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 305;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 23
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3550, 3758);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 18, 26);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 229;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 222;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 24
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 3705, 3913);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 58, 2);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 476;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 124;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 25
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 3860, 4069);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 38, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 671;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 226;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 26
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4016, 4224);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 19, 20);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 166;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 109;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 27
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4171, 4379);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 59, 52);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 716;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 64;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 28
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4326, 4534);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 45, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 800;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 160;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 29
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4481, 4690);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 24, 41);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 720;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 281;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 30
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4637, 4849);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 6, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 844;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 239;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 31
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 4792, 5004);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 47, 55);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 401;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 135;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 32
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 4949, 5158);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 31, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 250;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 65;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 33
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5105, 5313);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 12, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 904;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 71;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 34
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5260, 5466);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 54, 5);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 900;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 226;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 35
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5415, 5623);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 37, 31);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 204;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 74;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 36
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5568, 5777);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 20, 8);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 219;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 58;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 37
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 5724, 5929);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 4, 21);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 445;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 252;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 38
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 5879, 6084);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 48, 12);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 701;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 120;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 39
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6031, 6238);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 31, 15);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 608;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 40
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6187, 6392);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 10, 51);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 286;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 159;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 41
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6339, 6547);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 53, 57);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 710;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 243;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 42
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6494, 6699);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 35, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 533;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 218;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 43
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6649, 6855);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 15, 41);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 264;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 58;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 44
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 6802, 7010);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 58, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 339;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 87;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 45
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 6957, 7167);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 38, 25);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 259;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 89;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 46
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7112, 7321);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 22, 12);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 786;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 138;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 47
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7268, 7476);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 0, 59);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 835;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 183;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 48
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7423, 7631);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 43, 11);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 516;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 231;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 49
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7578, 7787);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 24, 53);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 802;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 93;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 50
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 7733, 7941);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 7, 0);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 758;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 285;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 51
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 7889, 8094);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 48, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 664;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 181;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 52
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8041, 8249);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 31, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 69;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 98;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 53
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8196, 8405);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 13, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 54;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 157;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 54
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8351, 8560);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 56, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 629;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 60;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 55
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8507, 8716);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 35, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 808;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 75;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 56
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8662, 8870);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 15, 53);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 28;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 42;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 57
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 8817, 9026);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 58, 5);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 875;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 63;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 58
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 8973, 9178);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 40, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 558;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 56;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 59
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9128, 9333);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 23, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 74;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 145;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 60
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9280, 9488);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 6, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 603;
         expectedProjectInfo.ProjectClone = 4;
         expectedProjectInfo.ProjectGen = 55;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 61
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9435, 9644);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 48, 59);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 742;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 147;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 62
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9591, 9801);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 28, 4);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 96;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 267;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 63
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 9746, 9956);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 9, 27);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 376;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 175;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 64
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 9901, 10110);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 48, 44);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 747;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 246;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 65
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10057, 10267);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 29, 15);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 613;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 187;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 66
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10212, 10417);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 12, 27);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 641;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 155;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 67
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10367, 10572);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 56, 55);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 862;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 295;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 68
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10519, 10728);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 39, 22);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 447;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 136;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 69
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10675, 10883);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 18, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 129;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 284;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 70
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 10830, 11039);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 58, 50);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 673;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 178;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 71
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 10985, 11194);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 37, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 178;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 194;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 72
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11141, 11346);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 19, 39);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 340;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 184;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 73
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11296, 11501);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 1, 20);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 774;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 266;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 74
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11448, 11656);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 43, 8);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 663;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 296;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 75
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11603, 11814);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 23, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 108;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 176;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 76
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 11759, 11969);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 7, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 678;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 204;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 77
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 11914, 12119);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 48, 52);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 552;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 216;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 78
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12069, 12275);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 30, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 710;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 275;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 79
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12222, 12430);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 9, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 350;
         expectedProjectInfo.ProjectClone = 6;
         expectedProjectInfo.ProjectGen = 110;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 80
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12377, 12585);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 52, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 287;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 200;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 81
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12532, 12742);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 32, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 95;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 239;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 82
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12687, 12896);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 18, 51);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 895;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 148;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 83
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 12843, 13051);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 1, 3);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 556;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 196;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 84
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 12998, 13206);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 42, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 881;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 171;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 85
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13153, 13362);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 23, 42);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 851;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 119;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 86
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13309, 13516);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 5, 44);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 324;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 173;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 87
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13464, 13671);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 44, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 70;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 75;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 88
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13616, 13825);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 22, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 687;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 88;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 89
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 13772, 13981);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 6, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 395;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 227;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 90
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 13928, 14134);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 47, 27);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 316;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 141;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 91
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14083, 14286);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 28, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 70;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 178;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 92
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14236, 14441);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 7, 50);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 582;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 275;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 93
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14388, 14597);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 52, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 909;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 113;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 94
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14544, 14749);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 31, 20);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 303;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 91;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 95
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 14699, 14904);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 12, 12);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 921;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 274;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 96
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 14851, 15060);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 54, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 863;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 317;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 97
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15007, 15212);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 35, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 150;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 238;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 98
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15162, 15368);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 20, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 82;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 256;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 99
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15314, 15519);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 59, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 590;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 204;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 100
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15469, 15675);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 44, 26);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 820;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 251;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 101
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15622, 15832);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 28, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 194;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 306;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 102
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 15777, 15987);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 12, 51);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 112;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 199;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 103
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 15932, 16138);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 51, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 1;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 211;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 104
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16088, 16290);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 30, 59);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 792;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 149;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 105
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16240, 16445);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 12, 25);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 79;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 295;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 106
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16392, 16600);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 56, 52);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 169;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 172;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 107
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16547, 16756);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 40, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 551;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 244;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 108
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 16703, 16913);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 23, 6);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 804;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 327;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 109
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 16858, 17066);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 3, 12);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 111;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 178;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 110
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17013, 17222);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 46, 23);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 406;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 221;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 111
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17169, 17374);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 27, 15);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 453;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 207;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 112
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17324, 17529);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 10, 7);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 533;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 208;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 113
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17476, 17686);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 56, 15);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 784;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 279;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 114
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17631, 17842);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 36, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 193;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 273;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 115
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 17787, 17995);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 17, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 637;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 296;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 116
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 17942, 18145);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 2, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 185;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 312;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 117
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18097, 18305);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 47, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 877;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 262;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 118
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18248, 18463);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 27, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 761;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 173;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 119
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18407, 18623);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 9, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 144;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 239;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 120
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18563, 18780);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 48, 3);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 529;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 167;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 121
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 18723, 18939);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 30, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 84;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 331;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 122
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 18883, 19097);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 9, 42);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 494;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 188;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 123
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19041, 19258);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 51, 8);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 199;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 364;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 124
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19199, 19416);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 31, 30);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 256;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 292;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 125
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19359, 19575);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 13, 27);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 150;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 279;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 126
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19518, 19736);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 55, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 479;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 291;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 127
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19677, 19895);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 40, 32);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 56;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 306;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 128
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 19836, 20056);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 20, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 189;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 233;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 129
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 19996, 20215);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 1, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 161;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 130
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20156, 20374);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 42, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 869;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 312;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 131
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20315, 20536);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 26, 55);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 154;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 293;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 132
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20474, 20690);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 6, 56);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 231;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 280;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 133
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20637, 20825);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 48, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 413;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 274;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 134
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 20792, 21006);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 29, 40);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 181;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 310;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 135
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 20953, 21162);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 15, 13);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 528;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 274;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 136
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21109, 21317);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 54, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 595;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 262;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 137
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21264, 21472);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 34, 40);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 86;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 246;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 138
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21419, 21627);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 17, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 81;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 217;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 139
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21574, 21780);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 57, 29);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 430;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 263;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 140
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 21730, 21935);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 36, 11);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 869;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 319;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 141
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 21882, 22090);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 17, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 159;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 237;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 142
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22037, 22246);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 59, 30);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 883;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 176;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 143
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22193, 22401);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 43, 23);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 481;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 216;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 144
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22348, 22557);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 25, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 31;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 307;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 145
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22503, 22712);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 7, 56);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 158;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 266;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 146
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22659, 22870);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 48, 2);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 563;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 314;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 147
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 22815, 23022);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 29, 14);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 333;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 299;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 148
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 22970, 23177);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 9, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 189;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 242;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 149
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23122, 23328);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 50, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 86;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 274;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 150
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23278, 23483);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 34, 10);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 49;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 298;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 151
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23430, 23638);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 17, 22);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 38;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 313;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 152
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23585, 23793);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 57, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 733;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 151;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 153
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 23740, 23949);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 42, 6);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 179;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 197;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 154
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 23896, 24104);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 25, 33);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 66;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 328;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 155
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24051, 24259);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 9, 26);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 45;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 262;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 156
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24206, 24415);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 51, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 793;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 298;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 157
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24362, 24572);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 33, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 72;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 267;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 158
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24517, 24725);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 11, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 497;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 274;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 159
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24672, 24882);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 54, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 234;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 218;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 160
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 24827, 25033);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 37, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 512;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 249;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 161
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 24983, 25185);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 22, 17);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 865;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 338;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 162
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25135, 25340);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 6, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 321;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 241;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 163
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25287, 25496);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 54, 38);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 738;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 281;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 164
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25443, 25651);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 40, 36);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 803;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 188;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 165
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25598, 25803);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 22, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 553;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 300;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 166
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 25753, 25958);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 3, 15);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 135;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 306;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 167
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 25905, 26111);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 48, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 149;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 322;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 168
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26061, 26268);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 30, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 605;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 238;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 169
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26213, 26421);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 15, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 38;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 178;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 170
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26368, 26577);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 55, 27);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 521;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 285;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 171
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26524, 26732);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 36, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 86;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 154;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 172
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26679, 26884);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 18, 55);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 274;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 292;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 173
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 26834, 27039);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 1, 57);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 36;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 248;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 174
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 26986, 27199);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 44, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 867;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 250;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 175
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27142, 27349);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 29, 6);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 698;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 322;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 176
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27299, 27506);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 8, 22);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 665;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 278;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 177
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27451, 27660);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 48, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 875;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 176;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 178
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27607, 27812);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 30, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 514;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 250;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 179
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 27762, 27964);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(6, 14, 32);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 329;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 331;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 180
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 27914, 28122);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 54, 39);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 382;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 312;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 181
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28066, 28276);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 35, 41);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 4;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 328;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 182
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28223, 28431);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(11, 16, 7);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 321;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 192;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 183
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28378, 28588);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 57, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 601;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 283;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 184
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28533, 28741);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(14, 35, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 19;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 245;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 185
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28688, 28897);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(16, 15, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 790;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 283;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 186
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 28844, 29052);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 54, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 296;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 250;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 187
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 28999, 29209);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 33, 8);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 342;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 289;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 188
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29154, 29363);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(21, 13, 14);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 65;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 242;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 189
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29310, 29517);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 54, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 910;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 255;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 190
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29465, 29670);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 36, 18);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 514;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 256;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 191
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29617, 29823);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(2, 16, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 105;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 325;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 192
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 29772, 29979);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 58, 35);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 109;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 281;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 193
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 29926, 30134);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 38, 12);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 115;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 135;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 194
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30081, 30286);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 20, 4);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 86;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 273;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 195
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30236, 30442);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(9, 2, 6);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 296;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 343;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 196
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30389, 30597);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 40, 28);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 632;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 208;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 197
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30544, 30752);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 19, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 759;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 272;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 198
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 30699, 30907);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 58, 16);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 79;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 232;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 199
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 30854, 31065);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 38, 52);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 599;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 305;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 200
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31010, 31222);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 19, 58);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 189;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 273;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 201
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31167, 31372);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(19, 3, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 116;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 301;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 202
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31322, 31528);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 41, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 656;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 342;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 203
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31475, 31680);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 24, 55);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 381;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 156;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 204
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31630, 31838);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(0, 7, 1);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9036;
         expectedProjectInfo.ProjectRun = 228;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 275;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 205
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 31782, 31991);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 52, 14);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9033;
         expectedProjectInfo.ProjectRun = 118;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 310;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 206
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 31938, 32147);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 37, 26);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 129;
         expectedProjectInfo.ProjectClone = 3;
         expectedProjectInfo.ProjectGen = 292;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 207
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32094, 32302);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(5, 18, 42);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 783;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 297;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 208
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32249, 32457);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(7, 2, 9);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 771;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 279;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 209
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32404, 32613);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(8, 41, 46);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 767;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 363;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 210
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32560, 32768);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(10, 24, 57);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9032;
         expectedProjectInfo.ProjectRun = 555;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 235;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 211
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 32715, 32920);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(12, 3, 43);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9040;
         expectedProjectInfo.ProjectRun = 13;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 240;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 212
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 32870, 33076);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(13, 44, 45);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 508;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 314;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 213
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33022, 33233);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(15, 27, 37);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 564;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 305;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 214
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33178, 33386);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(17, 8, 19);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 68;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 273;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 215
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33333, 33541);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(18, 49, 21);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9031;
         expectedProjectInfo.ProjectRun = 459;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 250;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 216
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33488, 33696);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(20, 29, 48);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9039;
         expectedProjectInfo.ProjectRun = 864;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 289;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 217
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33643, 33852);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(22, 8, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9037;
         expectedProjectInfo.ProjectRun = 28;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 277;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 218
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 33799, 34007);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(23, 49, 26);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9035;
         expectedProjectInfo.ProjectRun = 6;
         expectedProjectInfo.ProjectClone = 2;
         expectedProjectInfo.ProjectGen = 238;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 219
         expectedUnitRun = new UnitRun(expectedSlotRun, 0, 33954, 34159);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(1, 31, 3);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 101;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9034;
         expectedProjectInfo.ProjectRun = 739;
         expectedProjectInfo.ProjectClone = 0;
         expectedProjectInfo.ProjectGen = 272;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.FinishedUnit;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 220
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 34109, 34249);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(3, 10, 49);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 82;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 353;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 238;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Interrupted;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRun 0 - UnitRun 221
         expectedUnitRun = new UnitRun(expectedSlotRun, 1, 34282, 34345);
         expectedUnitRun.Data = new UnitRunData();
         expectedUnitRun.Data.UnitStartTimeStamp = new TimeSpan(4, 41, 24);
         expectedUnitRun.Data.CoreVersion = 2.27f;
         expectedUnitRun.Data.FramesObserved = 5;
         expectedProjectInfo = new ProjectInfo();
         expectedProjectInfo.ProjectID = 9038;
         expectedProjectInfo.ProjectRun = 353;
         expectedProjectInfo.ProjectClone = 1;
         expectedProjectInfo.ProjectGen = 238;
         expectedUnitRun.Data.ProjectInfoList.Add(expectedProjectInfo);
         expectedUnitRun.Data.WorkUnitResult = WorkUnitResult.Unknown;
         expectedSlotRun.UnitRuns.Push(expectedUnitRun);

         // Setup SlotRunData 0
         expectedSlotRun.Data = new SlotRunData();
         expectedSlotRun.Data.CompletedUnits = 220;
         expectedSlotRun.Data.FailedUnits = 0;
         expectedSlotRun.Data.TotalCompletedUnits = null;
         expectedSlotRun.Data.Status = SlotStatus.Unknown;

         // Setup ClientRunData 0
         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2016, 6, 18, 15, 50, 21, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.ElementAt(0);
         FahLogAssert.AreEqual(expectedRun, actualRun, true);

         Assert.AreEqual(2, actualRun.Count(x => x.LineType == LogLineType.Error));
      }

      // ReSharper restore InconsistentNaming
   }
}
