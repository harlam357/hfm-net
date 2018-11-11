
using System;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Log
{
   /// <summary>
   /// Creates run data objects by aggregating information owned by <see cref="ClientRun"/>, <see cref="SlotRun"/>, and <see cref="UnitRun"/> objects.
   /// </summary>
   public abstract class RunDataAggregator
   {
      /// <summary>
      /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
      /// </summary>
      public ClientRunData GetClientRunData(ClientRun clientRun)
      {
         return OnGetClientRunData(clientRun);
      }

      /// <summary>
      /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
      /// </summary>
      protected abstract ClientRunData OnGetClientRunData(ClientRun clientRun);

      /// <summary>
      /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
      /// </summary>
      public SlotRunData GetSlotRunData(SlotRun slotRun)
      {
         return OnGetSlotRunData(slotRun);
      }

      /// <summary>
      /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
      /// </summary>
      protected abstract SlotRunData OnGetSlotRunData(SlotRun slotRun);

      /// <summary>
      /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
      /// </summary>
      public UnitRunData GetUnitRunData(UnitRun unitRun)
      {
         return OnGetUnitRunData(unitRun);
      }

      /// <summary>
      /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
      /// </summary>
      protected abstract UnitRunData OnGetUnitRunData(UnitRun unitRun);
   }

   namespace FahClient
   {
      /// <summary>
      /// Creates run data objects for a v7 or newer client.
      /// </summary>
      public class FahClientRunDataAggregator : RunDataAggregator
      {
         /// <summary>
         /// Gets a singleton instance of the <see cref="FahClientRunDataAggregator"/> class.
         /// </summary>
         public static FahClientRunDataAggregator Instance { get; } = new FahClientRunDataAggregator();

         /// <summary>
         /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
         /// </summary>
         protected override ClientRunData OnGetClientRunData(ClientRun clientRun)
         {
            var clientRunData = new ClientRunData();

            foreach (var line in clientRun.LogLines)
            {
               switch (line.LineType)
               {
                  case LogLineType.LogOpen:
                     clientRunData.StartTime = (DateTime)line.Data;
                     break;
               }
            }

            return clientRunData;
         }

         /// <summary>
         /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
         /// </summary>
         protected override SlotRunData OnGetSlotRunData(SlotRun slotRun)
         {
            var slotRunData = new SlotRunData();

            foreach (var unitRun in slotRun.UnitRuns)
            {
               slotRunData.AddWorkUnitResult(unitRun.Data.WorkUnitResult);
            }

            return slotRunData;
         }

         /// <summary>
         /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
         /// </summary>
         protected override UnitRunData OnGetUnitRunData(UnitRun unitRun)
         {
            var unitRunData = new UnitRunData();
            foreach (var line in unitRun.LogLines)
            {
               switch (line.LineType)
               {
                  case LogLineType.WorkUnitWorking:
                     unitRunData.UnitStartTimeStamp = line.TimeStamp;
                     break;
                  case LogLineType.WorkUnitFrame:
                     if (unitRunData.UnitStartTimeStamp == null)
                     {
                        unitRunData.UnitStartTimeStamp = line.TimeStamp;
                     }
                     if (line.Data != null)
                     {
                        unitRunData.FramesObserved++;
                     }
                     break;
                  case LogLineType.WorkUnitCoreVersion:
                     if (Math.Abs(unitRunData.CoreVersion) < Single.Epsilon && line.Data != null)
                     {
                        unitRunData.CoreVersion = (float)line.Data;
                     }
                     break;
                  case LogLineType.WorkUnitProject:
                     var data = (WorkUnitProjectData)line.Data;
                     unitRunData.ProjectID = data.ProjectID;
                     unitRunData.ProjectRun = data.ProjectRun;
                     unitRunData.ProjectClone = data.ProjectClone;
                     unitRunData.ProjectGen = data.ProjectGen;
                     break;
                  case LogLineType.WorkUnitCoreReturn:
                     unitRunData.WorkUnitResult = (WorkUnitResult)line.Data;
                     break;
               }
            }
            return unitRunData;
         }
      }
   }

   namespace Legacy
   {
      /// <summary>
      /// Creates run data objects for a v6 or prior client.
      /// </summary>
      public class LegacyRunDataAggregator : RunDataAggregator
      {
         /// <summary>
         /// Gets a singleton instance of the <see cref="LegacyRunDataAggregator"/> class.
         /// </summary>
         public static LegacyRunDataAggregator Instance { get; } = new LegacyRunDataAggregator();

         /// <summary>
         /// Creates a new <see cref="ClientRunData"/> object from the information contained in the <see cref="ClientRun"/> object.
         /// </summary>
         protected override ClientRunData OnGetClientRunData(ClientRun clientRun)
         {
            var clientRunData = new ClientRunData();

            foreach (var line in clientRun.LogLines)
            {
               if (line.LineType == LogLineType.LogOpen)
               {
                  clientRunData.StartTime = (DateTime)line.Data;
               }
               else if (line.LineType == LogLineType.ClientVersion)
               {
                  clientRunData.ClientVersion = line.Data.ToString();
               }
               else if (line.LineType == LogLineType.ClientArguments)
               {
                  clientRunData.Arguments = line.Data.ToString();
               }
               else if (line.LineType == LogLineType.ClientUserNameAndTeam)
               {
                  var data = (ClientUserNameAndTeamData)line.Data;
                  clientRunData.FoldingID = data.FoldingID;
                  clientRunData.Team = data.Team;
               }
               else if (line.LineType == LogLineType.ClientUserID ||
                        line.LineType == LogLineType.ClientReceivedUserID)
               {
                  clientRunData.UserID = line.Data.ToString();
               }
               else if (line.LineType == LogLineType.ClientMachineID)
               {
                  clientRunData.MachineID = (int)line.Data;
               }
            }

            return clientRunData;
         }

         /// <summary>
         /// Creates a new <see cref="SlotRunData"/> object from the information contained in the <see cref="SlotRun"/> object.
         /// </summary>
         protected override SlotRunData OnGetSlotRunData(SlotRun slotRun)
         {
            var slotRunData = new SlotRunData();

            foreach (var unitRun in slotRun.UnitRuns)
            {
               slotRunData.AddWorkUnitResult(unitRun.Data.WorkUnitResult);
               if (slotRunData.TotalCompletedUnits == null)
               {
                  slotRunData.TotalCompletedUnits = unitRun.Data.TotalCompletedUnits;
               }
            }

            // try to get the status from the most recent unit run
            var lastUnitRun = slotRun.UnitRuns.FirstOrDefault();
            if (lastUnitRun != null)
            {
               slotRunData.Status = GetSlotRunDataStatusLegacy(lastUnitRun.LogLines);
            }
            else
            {
               // otherwise, get the status from the parent ClientRun
               slotRunData.Status = GetSlotRunDataStatusLegacy(slotRun.Parent.LogLines);
            }

            return slotRunData;
         }

         private static LogSlotStatus GetSlotRunDataStatusLegacy(IEnumerable<LogLine> logLines)
         {
            var status = LogSlotStatus.Unknown;

            foreach (var line in logLines)
            {
               if (line.LineType == LogLineType.WorkUnitProcessing ||
                   line.LineType == LogLineType.WorkUnitWorking ||
                   line.LineType == LogLineType.WorkUnitCoreStart ||
                   line.LineType == LogLineType.WorkUnitFrame ||
                   line.LineType == LogLineType.WorkUnitResumeFromBattery)
               {
                  status = LogSlotStatus.RunningNoFrameTimes;
               }
               else if (line.LineType == LogLineType.WorkUnitPaused ||
                        line.LineType == LogLineType.WorkUnitPausedForBattery)
               {
                  status = LogSlotStatus.Paused;
               }
               else if (line.LineType == LogLineType.ClientSendWorkToServer)
               {
                  status = LogSlotStatus.SendingWorkPacket;
               }
               else if (line.LineType == LogLineType.ClientAttemptGetWorkPacket)
               {
                  status = LogSlotStatus.GettingWorkPacket;
               }
               else if (line.LineType == LogLineType.ClientEuePauseState)
               {
                  status = LogSlotStatus.EuePause;
               }
               else if (line.LineType == LogLineType.ClientShutdown ||
                        line.LineType == LogLineType.ClientCoreCommunicationsErrorShutdown)
               {
                  status = LogSlotStatus.Stopped;
               }
            }

            return status;
         }

         /// <summary>
         /// Creates a new <see cref="UnitRunData"/> object from the information contained in the <see cref="UnitRun"/> object.
         /// </summary>
         protected override UnitRunData OnGetUnitRunData(UnitRun unitRun)
         {
            bool clientWasPaused = false;
            bool lookForProject = true;

            var unitRunData = new UnitRunData();
            foreach (var line in unitRun.LogLines)
            {
               #region Unit Start

               if ((line.LineType == LogLineType.WorkUnitProcessing ||
                    line.LineType == LogLineType.WorkUnitWorking ||
                    line.LineType == LogLineType.WorkUnitCoreStart ||
                    line.LineType == LogLineType.WorkUnitFrame) &&
                    unitRunData.UnitStartTimeStamp == null)
               {
                  unitRunData.UnitStartTimeStamp = line.TimeStamp;
               }

               if (line.LineType == LogLineType.WorkUnitPaused ||
                   line.LineType == LogLineType.WorkUnitPausedForBattery)
               {
                  clientWasPaused = true;
               }
               else if ((line.LineType == LogLineType.WorkUnitWorking ||
                         line.LineType == LogLineType.WorkUnitResumeFromBattery ||
                         line.LineType == LogLineType.WorkUnitFrame) &&
                         clientWasPaused)
               {
                  clientWasPaused = false;

                  // Reset the Frames Observed Count
                  // This will cause the Instance to only use frames beyond this point to
                  // set frame times and determine status - Issue 13 (Revised)
                  unitRunData.FramesObserved = 0;
                  // Reset the Unit Start Time
                  unitRunData.UnitStartTimeStamp = line.TimeStamp;
               }

               #endregion

               #region Project

               if (lookForProject)
               {
                  // If we encounter a work unit frame, we should have
                  // already seen the Project Information, stop looking
                  if (line.LineType == LogLineType.WorkUnitFrame)
                  {
                     lookForProject = false;
                  }
                  if (line.LineType == LogLineType.WorkUnitProject)
                  {
                     if (line.Data != null)
                     {
                        var data = (WorkUnitProjectData)line.Data;
                        unitRunData.ProjectID = data.ProjectID;
                        unitRunData.ProjectRun = data.ProjectRun;
                        unitRunData.ProjectClone = data.ProjectClone;
                        unitRunData.ProjectGen = data.ProjectGen;
                     }
                  }
               }

               #endregion

               switch (line.LineType)
               {
                  case LogLineType.WorkUnitFrame:
                     if (line.Data != null)
                     {
                        unitRunData.FramesObserved++;
                     }
                     break;
                  case LogLineType.WorkUnitCoreVersion:
                     if (Math.Abs(unitRunData.CoreVersion) < Single.Epsilon && line.Data != null)
                     {
                        unitRunData.CoreVersion = (float)line.Data;
                     }
                     break;
                  case LogLineType.WorkUnitCoreShutdown:
                  case LogLineType.ClientCoreCommunicationsError:
                     unitRunData.WorkUnitResult = (WorkUnitResult)line.Data;
                     break;
                  case LogLineType.WorkUnitCallingCore:
                     unitRunData.Threads = (int)line.Data;
                     break;
                  case LogLineType.ClientNumberOfUnitsCompleted:
                     unitRunData.TotalCompletedUnits = (int)line.Data;
                     break;
               }
            }
            return unitRunData;
         }
      }
   }
}
