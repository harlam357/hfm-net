
using System;
using System.Collections.Generic;
using System.Linq;

namespace HFM.Log
{
   internal static class FahLogInterpreter
   {
      internal static ClientRunData GetClientRunData(ClientRun clientRun)
      {
         if (clientRun.Parent is Legacy.LegacyLog)
         {
            return GetClientRunDataLegacy(clientRun);
         }
         if (clientRun.Parent is FahClient.FahClientLog)
         {
            return GetClientRunDataFahClient(clientRun);
         }
         throw new ArgumentException("ClientRun FahLogType unknown", "clientRun");
      }

      private static ClientRunData GetClientRunDataLegacy(ClientRun clientRun)
      {
         var clientRunData = new ClientRunData();

         foreach (var line in clientRun.LogLines)
         {
            if (line.LineType == LogLineType.LogOpen)
            {
               clientRunData.StartTime = (DateTime)line.LineData;
            }
            else if (line.LineType == LogLineType.ClientVersion)
            {
               clientRunData.ClientVersion = line.LineData.ToString();
            }
            else if (line.LineType == LogLineType.ClientArguments)
            {
               clientRunData.Arguments = line.LineData.ToString();
            }
            else if (line.LineType == LogLineType.ClientUserNameTeam)
            {
               var userAndTeam = (Tuple<string, int>)line.LineData;
               clientRunData.FoldingID = userAndTeam.Item1;
               clientRunData.Team = userAndTeam.Item2;
            }
            else if (line.LineType == LogLineType.ClientUserID ||
                     line.LineType == LogLineType.ClientReceivedUserID)
            {
               clientRunData.UserID = line.LineData.ToString();
            }
            else if (line.LineType == LogLineType.ClientMachineID)
            {
               clientRunData.MachineID = (int)line.LineData;
            }
         }

         return clientRunData;
      }

      private static ClientRunData GetClientRunDataFahClient(ClientRun clientRun)
      {
         var clientRunData = new ClientRunData();

         foreach (var line in clientRun.LogLines)
         {
            switch (line.LineType)
            {
               case LogLineType.LogOpen:
                  clientRunData.StartTime = (DateTime)line.LineData;
                  break;
            }
         }

         return clientRunData;
      }

      internal static SlotRunData GetSlotRunData(SlotRun slotRun)
      {
         if (slotRun.Parent.Parent is Legacy.LegacyLog)
         {
            return GetSlotRunDataLegacy(slotRun);
         }
         if (slotRun.Parent.Parent is FahClient.FahClientLog)
         {
            return GetSlotRunDataFahClient(slotRun);
         }
         throw new ArgumentException("SlotRun FahLogType unknown", "slotRun");
      }

      private static SlotRunData GetSlotRunDataLegacy(SlotRun slotRun)
      {
         var slotRunData = new SlotRunData();

         foreach (var unitRun in slotRun.UnitRuns)
         {
            AddWorkUnitResult(slotRunData, unitRun.Data.WorkUnitResult);
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
                line.LineType == LogLineType.WorkUnitStart ||
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

      private static SlotRunData GetSlotRunDataFahClient(SlotRun slotRun)
      {
         var slotRunData = new SlotRunData();

         foreach (var unitRun in slotRun.UnitRuns)
         {
            AddWorkUnitResult(slotRunData, unitRun.Data.WorkUnitResult);
         }

         return slotRunData;
      }

      private static void AddWorkUnitResult(SlotRunData slotRunData, WorkUnitResult workUnitResult)
      {
         if (workUnitResult == WorkUnitResult.FinishedUnit)
         {
            slotRunData.CompletedUnits++;
         }
         else if (IsFailedWorkUnit(workUnitResult))
         {
            slotRunData.FailedUnits++;
         }
      }

      private static bool IsFailedWorkUnit(WorkUnitResult result)
      {
         switch (result)
         {
            case WorkUnitResult.EarlyUnitEnd:
            case WorkUnitResult.UnstableMachine:
            case WorkUnitResult.BadWorkUnit:
            case WorkUnitResult.ClientCoreError:
               return true;
            default:
               return false;
         }
      }

      internal static UnitRunData GetUnitRunData(UnitRun unitRun)
      {
         if (unitRun.Parent.Parent.Parent is Legacy.LegacyLog)
         {
            return GetUnitRunDataLegacy(unitRun);
         }
         if (unitRun.Parent.Parent.Parent is FahClient.FahClientLog)
         {
            return GetUnitRunDataFahClient(unitRun);
         }
         throw new ArgumentException("UnitRun FahLogType unknown", "unitRun");
      }

      private static UnitRunData GetUnitRunDataLegacy(UnitRun unitRun)
      {
         bool clientWasPaused = false;
         bool lookForProject = true;

         var unitRunData = new UnitRunData();
         foreach (var line in unitRun.LogLines)
         {
            #region Unit Start

            if ((line.LineType == LogLineType.WorkUnitProcessing ||
                 line.LineType == LogLineType.WorkUnitWorking ||
                 line.LineType == LogLineType.WorkUnitStart ||
                 line.LineType == LogLineType.WorkUnitFrame) &&
                 unitRunData.UnitStartTimeStamp == null)
            {
               unitRunData.UnitStartTimeStamp = CommonLogLineParser.GetTimeStamp(line);
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
               unitRunData.UnitStartTimeStamp = CommonLogLineParser.GetTimeStamp(line);
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
                  if (line.LineData != null)
                  {
                     var projectInfo = (Tuple<int, int, int, int>)line.LineData;
                     unitRunData.ProjectID = projectInfo.Item1;
                     unitRunData.ProjectRun = projectInfo.Item2;
                     unitRunData.ProjectClone = projectInfo.Item3;
                     unitRunData.ProjectGen = projectInfo.Item4;
                  }
               }
            }

            #endregion

            switch (line.LineType)
            {
               case LogLineType.WorkUnitFrame:
                  if (line.LineData != null)
                  {
                     unitRunData.FramesObserved++;
                  }
                  break;
               case LogLineType.WorkUnitCoreVersion:
                  if (Math.Abs(unitRunData.CoreVersion) < Single.Epsilon && line.LineData != null)
                  {
                     unitRunData.CoreVersion = (float)line.LineData;
                  }
                  break;
               case LogLineType.WorkUnitCoreShutdown:
               case LogLineType.ClientCoreCommunicationsError:
                  unitRunData.WorkUnitResult = (WorkUnitResult)line.LineData;
                  break;
               case LogLineType.WorkUnitCallingCore:
                  unitRunData.Threads = (int)line.LineData;
                  break;
               case LogLineType.ClientNumberOfUnitsCompleted:
                  unitRunData.TotalCompletedUnits = (int)line.LineData;
                  break;
            }
         }
         return unitRunData;
      }

      private static UnitRunData GetUnitRunDataFahClient(UnitRun unitRun)
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
                  if (line.LineData != null)
                  {
                     unitRunData.FramesObserved++;
                  }
                  break;
               case LogLineType.WorkUnitCoreVersion:
                  if (Math.Abs(unitRunData.CoreVersion) < Single.Epsilon && line.LineData != null)
                  {
                     unitRunData.CoreVersion = (float)line.LineData;
                  }
                  break;
               case LogLineType.WorkUnitProject:
                  var projectInfo = (Tuple<int, int, int, int>)line.LineData;
                  unitRunData.ProjectID = projectInfo.Item1;
                  unitRunData.ProjectRun = projectInfo.Item2;
                  unitRunData.ProjectClone = projectInfo.Item3;
                  unitRunData.ProjectGen = projectInfo.Item4;
                  break;
               case LogLineType.WorkUnitCoreReturn:
                  unitRunData.WorkUnitResult = (WorkUnitResult)line.LineData;
                  break;
            }
         }
         return unitRunData;
      }
   }
}
