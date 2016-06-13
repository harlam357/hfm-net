
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

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
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

         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2012, 1, 11, 3, 24, 22, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         //Assert.AreEqual(1, logInterpreter.LogLineParsingErrors.Count());

         var projectInfo = new ProjectInfo { ProjectID = 7610, ProjectRun = 630, ProjectClone = 0, ProjectGen = 59 };
         var unitRun = GetUnitRun(actualRun.SlotRuns[0], 1, projectInfo);
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
         unitRun = GetUnitRun(actualRun.SlotRuns[1], 2, projectInfo);
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
      public void FahClientLog_Read_Client_v7_13_Test()
      {
         // Scan
         var fahLog = FahLog.Read(File.ReadAllLines("..\\..\\..\\TestFiles\\Client_v7_13\\log.txt"), FahLogType.FahClient);

         // Check Run 0 Positions
         var expectedRun = new ClientRun(null, 0);
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

         expectedRun.Data = new ClientRunData();
         expectedRun.Data.StartTime = new DateTime(2014, 7, 25, 13, 57, 36, DateTimeKind.Utc);
         expectedRun.Data.Arguments = null;
         expectedRun.Data.ClientVersion = null;
         expectedRun.Data.FoldingID = null;
         expectedRun.Data.Team = 0;
         expectedRun.Data.UserID = null;
         expectedRun.Data.MachineID = 0;

         var actualRun = fahLog.ClientRuns.Peek();
         FahLogAssert.AreEqual(expectedRun, actualRun);

         //Assert.AreEqual(1, logInterpreter.LogLineParsingErrors.Count());

         var projectInfo = new ProjectInfo { ProjectID = 13001, ProjectRun = 430, ProjectClone = 2, ProjectGen = 48 };
         var unitRun = GetUnitRun(actualRun.SlotRuns[0], 2, projectInfo);
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
         unitRun = GetUnitRun(actualRun.SlotRuns[0], 2, projectInfo);
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

      // ReSharper restore InconsistentNaming

      private static UnitRun GetUnitRun(SlotRun slotRun, int queueIndex, IProjectInfo projectInfo)
      {
         if (slotRun != null)
         {
            var unitRun = slotRun.UnitRuns.FirstOrDefault(x => x.QueueIndex == queueIndex && projectInfo.EqualsProject(x.Data));
            if (unitRun != null)
            {
               return unitRun;
            }
         }
         return null;
      }
   }
}
