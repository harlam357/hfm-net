
using System.Linq;

using NUnit.Framework;

namespace HFM.Log
{
   internal static class FahLogAssert
   {
      internal static void AreEqual(ClientRun expectedRun, ClientRun actualRun, bool assertUnitRunData = false)
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
            var expectedSlotRun = expectedRun.SlotRuns[key];
            var actualSlotRun = actualRun.SlotRuns[key];
            Assert.AreEqual(expectedSlotRun.Data.CompletedUnits, actualSlotRun.Data.CompletedUnits);
            Assert.AreEqual(expectedSlotRun.Data.FailedUnits, actualSlotRun.Data.FailedUnits);
            Assert.AreEqual(expectedSlotRun.Data.TotalCompletedUnits, actualSlotRun.Data.TotalCompletedUnits);
            Assert.AreEqual(expectedSlotRun.Data.Status, actualSlotRun.Data.Status);

            Assert.AreEqual(expectedSlotRun.UnitRuns.Count, actualSlotRun.UnitRuns.Count);
            for (int i = 0; i < expectedSlotRun.UnitRuns.Count; i++)
            {
               var expectedUnitRun = expectedSlotRun.UnitRuns.ElementAt(i);
               var actualUnitRun = actualSlotRun.UnitRuns.ElementAt(i);
               Assert.AreEqual(expectedUnitRun.QueueIndex, actualUnitRun.QueueIndex);
               Assert.AreEqual(expectedUnitRun.StartIndex, actualUnitRun.StartIndex);
               Assert.AreEqual(expectedUnitRun.EndIndex, actualUnitRun.EndIndex);
               if (assertUnitRunData)
               {
                  Assert.AreEqual(expectedUnitRun.Data.UnitStartTimeStamp, actualUnitRun.Data.UnitStartTimeStamp);
                  Assert.AreEqual(expectedUnitRun.Data.CoreVersion, actualUnitRun.Data.CoreVersion);
                  Assert.AreEqual(expectedUnitRun.Data.FramesObserved, actualUnitRun.Data.FramesObserved);
                  Assert.AreEqual(expectedUnitRun.Data.ProjectID, actualUnitRun.Data.ProjectID);
                  Assert.AreEqual(expectedUnitRun.Data.ProjectRun, actualUnitRun.Data.ProjectRun);
                  Assert.AreEqual(expectedUnitRun.Data.ProjectClone, actualUnitRun.Data.ProjectClone);
                  Assert.AreEqual(expectedUnitRun.Data.ProjectGen, actualUnitRun.Data.ProjectGen);
                  Assert.AreEqual(expectedUnitRun.Data.WorkUnitResult, actualUnitRun.Data.WorkUnitResult);
               }
            }
         }
      }
   }
}
