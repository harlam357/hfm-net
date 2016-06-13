
using System.Linq;

using NUnit.Framework;

namespace HFM.Log
{
   internal static class FahLogAssert
   {
      internal static void AreEqual(ClientRun expectedRun, ClientRun actualRun)
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
   }
}
