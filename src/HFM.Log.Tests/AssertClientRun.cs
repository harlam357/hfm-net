
using System.Linq;

using NUnit.Framework;

namespace HFM.Log
{
   internal static class AssertClientRun
   {
      internal static void AreEqual(ClientRun expectedRun, ClientRun actualRun, bool assertUnitRunData = false)
      {
         Assert.AreEqual(expectedRun.ClientStartIndex, actualRun.ClientStartIndex);
         Assert.AreEqual(expectedRun.Data.StartTime, actualRun.Data.StartTime);
         if (expectedRun.Data is Legacy.LegacyClientRunData legacyExpectedRunData && actualRun.Data is Legacy.LegacyClientRunData legacyActualRunData)
         {
            Assert.AreEqual(legacyExpectedRunData.Arguments, legacyActualRunData.Arguments);
            Assert.AreEqual(legacyExpectedRunData.ClientVersion, legacyActualRunData.ClientVersion);
            Assert.AreEqual(legacyExpectedRunData.FoldingID, legacyActualRunData.FoldingID);
            Assert.AreEqual(legacyExpectedRunData.Team, legacyActualRunData.Team);
            Assert.AreEqual(legacyExpectedRunData.UserID, legacyActualRunData.UserID);
            Assert.AreEqual(legacyExpectedRunData.MachineID, legacyActualRunData.MachineID);
         }

         Assert.AreEqual(expectedRun.SlotRuns.Count, actualRun.SlotRuns.Count);
         foreach (int slotRunKey in expectedRun.SlotRuns.Keys)
         {
            var expectedSlotRun = expectedRun.SlotRuns[slotRunKey];
            var actualSlotRun = actualRun.SlotRuns[slotRunKey];
            Assert.AreEqual(expectedSlotRun.Data.CompletedUnits, actualSlotRun.Data.CompletedUnits);
            Assert.AreEqual(expectedSlotRun.Data.FailedUnits, actualSlotRun.Data.FailedUnits);
            if (expectedSlotRun.Data is Legacy.LegacySlotRunData legacyExpectedSlotRunData &&
                actualSlotRun.Data is Legacy.LegacySlotRunData legacyActualSlotRunData)
            {
               Assert.AreEqual(legacyExpectedSlotRunData.TotalCompletedUnits, legacyActualSlotRunData.TotalCompletedUnits);
               Assert.AreEqual(legacyExpectedSlotRunData.Status, legacyActualSlotRunData.Status);
            }

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
                  if (expectedUnitRun.Data.FrameDataDictionary != null && actualUnitRun.Data.FrameDataDictionary != null)
                  {
                     Assert.AreEqual(expectedUnitRun.Data.FrameDataDictionary.Count, actualUnitRun.Data.FrameDataDictionary.Count);
                     foreach (int frameDataKey in expectedUnitRun.Data.FrameDataDictionary.Keys)
                     {
                        var expectedFrameDataDictionary = expectedUnitRun.Data.FrameDataDictionary[frameDataKey];
                        var actualFrameDataDictionary = actualUnitRun.Data.FrameDataDictionary[frameDataKey];
                        Assert.AreEqual(expectedFrameDataDictionary.ID, actualFrameDataDictionary.ID);
                        Assert.AreEqual(expectedFrameDataDictionary.RawFramesComplete, actualFrameDataDictionary.RawFramesComplete);
                        Assert.AreEqual(expectedFrameDataDictionary.RawFramesTotal, actualFrameDataDictionary.RawFramesTotal);
                        Assert.AreEqual(expectedFrameDataDictionary.TimeStamp, actualFrameDataDictionary.TimeStamp);
                        Assert.AreEqual(expectedFrameDataDictionary.Duration, actualFrameDataDictionary.Duration);
                     }
                  }
               }
            }
         }
      }
   }
}
