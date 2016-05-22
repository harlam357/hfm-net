
using System;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public static class LogInterpreter2
   {
      public static ClientRun2Data GetClientRunData(ClientRun2 clientRun)
      {
         switch (clientRun.Parent.LogFileType)
         {
            case LogFileType.Legacy:
               return GetClientRunDataLegacy(clientRun);
            case LogFileType.FahClient:
               return GetClientRunDataFahClient(clientRun);
         }
         throw new ArgumentException("ClientRun LogFileType unknown", "clientRun");
      }

      private static ClientRun2Data GetClientRunDataLegacy(ClientRun2 clientRun)
      {
         var clientRunData = new ClientRun2Data();

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

      private static ClientRun2Data GetClientRunDataFahClient(ClientRun2 clientRun)
      {
         var clientRunData = new ClientRun2Data();

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

      public static SlotRunData GetSlotRunData(SlotRun slotRun)
      {
         switch (slotRun.Parent.Parent.LogFileType)
         {
            case LogFileType.Legacy:
               return GetSlotRunDataLegacy(slotRun);
            case LogFileType.FahClient:
               return GetSlotRunDataFahClient(slotRun);
         }
         throw new ArgumentException("SlotRun LogFileType unknown", "slotRun");
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

         var lastUnitRun = slotRun.UnitRuns.Peek();
         if (lastUnitRun != null)
         {
            foreach (var line in lastUnitRun.LogLines)
            {
               if (line.LineType == LogLineType.WorkUnitProcessing ||
                   line.LineType == LogLineType.WorkUnitWorking ||
                   line.LineType == LogLineType.WorkUnitStart ||
                   line.LineType == LogLineType.WorkUnitFrame ||
                   line.LineType == LogLineType.WorkUnitResumeFromBattery)
               {
                  slotRunData.Status = SlotStatus.RunningNoFrameTimes;
               }
               else if (line.LineType == LogLineType.WorkUnitPaused ||
                        line.LineType == LogLineType.WorkUnitPausedForBattery)
               {
                  slotRunData.Status = SlotStatus.Paused;
               }
               else if (line.LineType == LogLineType.ClientSendWorkToServer)
               {
                  slotRunData.Status = SlotStatus.SendingWorkPacket;
               }
               else if (line.LineType == LogLineType.ClientAttemptGetWorkPacket)
               {
                  slotRunData.Status = SlotStatus.GettingWorkPacket;
               }
               else if (line.LineType == LogLineType.ClientEuePauseState)
               {
                  slotRunData.Status = SlotStatus.EuePause;
               }
               else if (line.LineType == LogLineType.ClientShutdown ||
                        line.LineType == LogLineType.ClientCoreCommunicationsErrorShutdown)
               {
                  slotRunData.Status = SlotStatus.Stopped;
               }
            }
         }

         return slotRunData;
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
         else if (workUnitResult == WorkUnitResult.EarlyUnitEnd ||
                  workUnitResult == WorkUnitResult.UnstableMachine ||
                  workUnitResult == WorkUnitResult.Interrupted ||
                  workUnitResult == WorkUnitResult.BadWorkUnit ||
                  workUnitResult == WorkUnitResult.CoreOutdated ||
                  workUnitResult == WorkUnitResult.ClientCoreError)
         {
            slotRunData.FailedUnits++;
         }
      }

      public static UnitRunData GetUnitRunData(UnitRun unitRun)
      {
         switch (unitRun.Parent.Parent.Parent.LogFileType)
         {
            case LogFileType.Legacy:
               return GetUnitRunDataLegacy(unitRun);
            case LogFileType.FahClient:
               return GetUnitRunDataFahClient(unitRun);
         }
         throw new ArgumentException("UnitRun LogFileType unknown", "unitRun");
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
               unitRunData.UnitStartTimeStamp = line.GetTimeStamp();
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
               unitRunData.UnitStartTimeStamp = line.GetTimeStamp();
            }

            #endregion

            #region Project

            if (lookForProject)
            {
               // If we encounter a work unit frame, we should have
               // already seen the Project Information, stop looking
               if (line.LineType.Equals(LogLineType.WorkUnitFrame))
               {
                  lookForProject = false;
               }
               if (line.LineType.Equals(LogLineType.WorkUnitProject))
               {
                  unitRunData.ProjectInfoList.Add((IProjectInfo)line.LineData);
               }
            }

            #endregion

            switch (line.LineType)
            {
               case LogLineType.WorkUnitFrame:
                  if (line.LineData != null)
                  {
                     unitRunData.FramesObserved++;
                     //unitRunData.FrameDataList.Add(line);
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
                  unitRunData.UnitStartTimeStamp = line.GetTimeStamp();
                  break;
               case LogLineType.WorkUnitFrame:
                  if (unitRunData.UnitStartTimeStamp == null)
                  {
                     unitRunData.UnitStartTimeStamp = line.GetTimeStamp();
                  }
                  if (line.LineData != null)
                  {
                     unitRunData.FramesObserved++;
                     //unitRunData.FrameDataList.Add(line);
                  }
                  break;
               case LogLineType.WorkUnitCoreVersion:
                  if (Math.Abs(unitRunData.CoreVersion) < Single.Epsilon && line.LineData != null)
                  {
                     unitRunData.CoreVersion = (float)line.LineData;
                  }
                  break;
               case LogLineType.WorkUnitProject:
                  unitRunData.ProjectInfoList.Add((IProjectInfo)line.LineData);
                  break;
               case LogLineType.WorkUnitCoreReturn:
                  unitRunData.WorkUnitResult = (WorkUnitResult)line.LineData;
                  break;
            }
         }
         return unitRunData;
      }

      public static UnitRun GetUnitRun(SlotRun slotRun, int queueIndex, IProjectInfo projectInfo)
      {
         foreach (var unitRun in slotRun.UnitRuns.Where(x => x.QueueIndex == queueIndex))
         {
            var projectLine = unitRun.LogLines.FirstOrDefault(x => x.LineType == LogLineType.WorkUnitProject);
            if (projectLine == null)
            {
               return null;
            }

            var lineProjectInfo = (IProjectInfo)projectLine.LineData;
            if (projectInfo.EqualsProject(lineProjectInfo))
            {
               return unitRun;
            }
         }
         return null;
      }
   }
}
