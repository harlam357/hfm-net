/*
 * HFM.NET - Legacy Data Aggregator Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using AutoMapper;
using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Log;
using HFM.Queue;

namespace HFM.Core
{
   internal class LegacyDataAggregator
   {
      /// <summary>
      /// Instance Name
      /// </summary>
      public string ClientName { get; set; }

      private ILogger _logger;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger ?? (_logger = NullLogger.Instance); }
         [CoverageExclude]
         set { _logger = value; }
      }

      /// <summary>
      /// Aggregate Data and return UnitInfo List
      /// </summary>
      public DataAggregatorResult AggregateData(FahLog fahLog, QueueData queueData, UnitInfoLogData unitInfo)
      {
         if (Logger.IsDebugEnabled)
         {
            foreach (var s in fahLog.Where(x => x.LineType == LogLineType.Error))
            {
               Logger.Debug(Constants.ClientNameFormat, ClientName, String.Format("Failed to parse log line: {0}", s));
            }
         }

         var currentClientRun = GetCurrentClientRun(fahLog);
         if (currentClientRun == null)
         {
            return null;
         }

         var result = new DataAggregatorResult();
         result.StartTime = currentClientRun.Data.StartTime;
         result.Arguments = currentClientRun.Data.Arguments;
         result.ClientVersion = currentClientRun.Data.ClientVersion;
         result.UserID = currentClientRun.Data.UserID;
         result.MachineID = currentClientRun.Data.MachineID;
         result.Status = currentClientRun.SlotRuns[0].Data.Status;

         // Decision Time: If Queue Read fails parse from logs only
         if (queueData != null)
         {
            GenerateUnitInfoDataFromQueue(result, queueData, fahLog, unitInfo);
            result.Queue = BuildClientQueue(queueData);
            result.CurrentUnitIndex = result.Queue.CurrentIndex;
         }
         else
         {
            Logger.Warn(Constants.ClientNameFormat, ClientName,
               "Queue unavailable or failed read.  Parsing logs without queue.");

            GenerateUnitInfoDataFromLogs(result, fahLog, unitInfo);
            // default Unit Index if only parsing logs
            result.CurrentUnitIndex = 1;
         }

         if (result.UnitInfos == null || result.UnitInfos[result.CurrentUnitIndex] == null || result.UnitInfos[result.CurrentUnitIndex].LogLines == null)
         {
            result.CurrentLogLines = currentClientRun.ToList();
         }
         else
         {
            result.CurrentLogLines = result.UnitInfos[result.CurrentUnitIndex].LogLines;
         }

         return result;
      }

      private static ClientQueue BuildClientQueue(QueueData q)
      {
         Debug.Assert(q != null);

         var cq = Mapper.Map<QueueData, ClientQueue>(q);
         for (int i = 0; i < 10; i++)
         {
            cq.Add(i, Mapper.Map<QueueEntry, ClientQueueEntry>(q.GetQueueEntry((uint)i)));
         }

         return cq;
      }

      private void GenerateUnitInfoDataFromLogs(DataAggregatorResult result, FahLog fahLog, UnitInfoLogData unitInfo)
      {
         result.UnitInfos = new Dictionary<int, UnitInfo>(2);
         for (int i = 0; i < 2; i++)
         {
            result.UnitInfos[i] = null;
         }

         var currentClientRun = GetCurrentClientRun(fahLog);
         var previousUnitRun = GetPreviousUnitRun(fahLog);
         if (previousUnitRun != null)
         {
            result.UnitInfos[0] = BuildUnitInfo(null, currentClientRun, previousUnitRun, null);
         }

         var currentUnitRun = GetCurrentUnitRun(fahLog);
         result.UnitInfos[1] = BuildUnitInfo(null, currentClientRun, currentUnitRun, unitInfo, currentUnitRun == null);
      }

      private static ClientRun GetCurrentClientRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         return fahLog.ClientRuns.FirstOrDefault();
      }

      private static SlotRun GetCurrentSlotRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var clientRun = GetCurrentClientRun(fahLog);
         return clientRun != null ? clientRun.SlotRuns[0] : null;
      }

      private static UnitRun GetPreviousUnitRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var slotRun = GetCurrentSlotRun(fahLog);
         return slotRun != null && slotRun.UnitRuns.Count > 1 ? slotRun.UnitRuns.ElementAt(1) : null;
      }

      private static UnitRun GetCurrentUnitRun(FahLog fahLog)
      {
         Debug.Assert(fahLog != null);

         var slotRun = GetCurrentSlotRun(fahLog);
         return slotRun != null ? slotRun.UnitRuns.FirstOrDefault() : null;
      }

      private void GenerateUnitInfoDataFromQueue(DataAggregatorResult result, QueueData q, FahLog fahLog, UnitInfoLogData unitInfo)
      {
         Debug.Assert(q != null);

         result.UnitInfos = new Dictionary<int, UnitInfo>(10);
         for (int i = 0; i < 10; i++)
         {
            result.UnitInfos[i] = null;
         }

         var clientRun = GetCurrentClientRun(fahLog);
         for (int queueIndex = 0; queueIndex < result.UnitInfos.Count; queueIndex++)
         {
            var unitRun = GetUnitRunForQueueIndex(fahLog, queueIndex);

            UnitInfoLogData unitInfoLogData = null;
            // On the Current Queue Index
            if (queueIndex == q.CurrentIndex)
            {
               // Get the UnitInfo Log Data
               unitInfoLogData = unitInfo;
            }

            result.UnitInfos[queueIndex] = BuildUnitInfo(q.GetQueueEntry((uint)queueIndex), clientRun, unitRun, unitInfoLogData);
            if (result.UnitInfos[queueIndex] == null)
            {
               if (queueIndex == q.CurrentIndex)
               {
                  string message = String.Format(CultureInfo.CurrentCulture,
                     "Could not verify log section for current queue entry {0}. Trying to parse with most recent log section.", queueIndex);
                  Logger.Warn(Constants.ClientNameFormat, ClientName, message);

                  unitRun = GetCurrentUnitRun(fahLog);

                  var slotRun = GetCurrentSlotRun(fahLog);
                  if (slotRun != null && slotRun.Data.Status == SlotStatus.GettingWorkPacket)
                  {
                     unitRun = null;
                     unitInfoLogData = new UnitInfoLogData();
                  }
                  result.UnitInfos[queueIndex] = BuildUnitInfo(q.GetQueueEntry((uint)queueIndex), clientRun, unitRun, unitInfoLogData, true);
               }
               else
               {
                  // Just skip this unit and continue
                  string message = String.Format(CultureInfo.CurrentCulture,
                     "Could not find or verify log section for queue entry {0} (this is not a problem).", queueIndex);
                  Logger.Debug(Constants.ClientNameFormat, ClientName, message);
               }
            }
         }
      }

      private static UnitRun GetUnitRunForQueueIndex(FahLog fahLog, int queueIndex)
      {
         foreach (var clientRun in fahLog.ClientRuns)
         {
            var slotRun = clientRun.SlotRuns[0];
            var unitRun = slotRun.UnitRuns.FirstOrDefault(x => x.QueueIndex == queueIndex);
            if (unitRun != null)
            {
               return unitRun;
            }
         }
         return null;
      }

      private static UnitInfo BuildUnitInfo(QueueEntry queueEntry, ClientRun clientRun, UnitRun unitRun, UnitInfoLogData unitInfoLogData, bool matchOverride = false)
      {
         // queueEntry can be null
         Debug.Assert(clientRun != null);
         // unitInfoLogData can be null

         var unit = new UnitInfo();

         UnitRunData unitRunData;
         if (unitRun == null)
         {
            if (matchOverride)
            {
               unitRunData = new UnitRunData();
            }
            else
            {
               return null;
            }
         }
         else
         {
            unit.LogLines = unitRun.ToList();
            unitRunData = unitRun.Data;
         }
         unit.UnitStartTimeStamp = unitRunData.UnitStartTimeStamp ?? TimeSpan.MinValue;
         unit.FramesObserved = unitRunData.FramesObserved;
         unit.CoreVersion = unitRunData.CoreVersion;
         unit.UnitResult = unitRunData.WorkUnitResult;

         if (queueEntry != null)
         {
            UpdateUnitInfoFromQueueData(unit, queueEntry);
            SearchFahLogUnitDataProjects(unit, unitRunData);
            UpdateUnitInfoFromLogData(unit, clientRun.Data, unitRunData, unitInfoLogData);

            if (!ProjectsMatch(unit, unitRunData) && !ProjectsMatch(unit, unitInfoLogData) && !matchOverride)
            {
               return null;
            }
         }
         else
         {
            UpdateUnitInfoFromLogData(unit, clientRun.Data, unitRunData, unitInfoLogData);
         }

         return unit;
      }

      private static void SearchFahLogUnitDataProjects(UnitInfo unit, UnitRunData unitRunData)
      {
         Debug.Assert(unit != null);
         Debug.Assert(unitRunData != null);

         for (int i = 0; i < unitRunData.ProjectInfoList.Count; i++)
         {
            if (ProjectsMatch(unit, unitRunData.ProjectInfoList[i]))
            {
               unitRunData.ProjectInfoIndex = i;
            }
         }
      }

      private static bool ProjectsMatch(UnitInfo unit, IProjectInfo projectInfo)
      {
         Debug.Assert(unit != null);

         if (unit.ProjectIsUnknown() || projectInfo == null) return false;

         return (unit.ProjectID == projectInfo.ProjectID &&
                 unit.ProjectRun == projectInfo.ProjectRun &&
                 unit.ProjectClone == projectInfo.ProjectClone &&
                 unit.ProjectGen == projectInfo.ProjectGen);
      }

      private static void UpdateUnitInfoFromQueueData(UnitInfo unitInfo, QueueEntry queueEntry)
      {
         Debug.Assert(unitInfo != null);
         Debug.Assert(queueEntry != null);

         // convert to enum
         var queueEntryStatus = (QueueEntryStatus)queueEntry.EntryStatus;

         if ((queueEntryStatus == QueueEntryStatus.Unknown ||
              queueEntryStatus == QueueEntryStatus.Empty ||
              queueEntryStatus == QueueEntryStatus.Garbage ||
              queueEntryStatus == QueueEntryStatus.Abandonded) == false)
         {
            /* Tag (Could be read here or through the unitinfo.txt file) */
            unitInfo.ProteinTag = queueEntry.WorkUnitTag;

            /* DownloadTime (Could be read here or through the unitinfo.txt file) */
            unitInfo.DownloadTime = queueEntry.BeginTimeUtc;

            /* DueTime (Could be read here or through the unitinfo.txt file) */
            unitInfo.DueTime = queueEntry.DueTimeUtc;

            /* FinishedTime */
            if (queueEntryStatus == QueueEntryStatus.Finished ||
                queueEntryStatus == QueueEntryStatus.ReadyForUpload)
            {
               unitInfo.FinishedTime = queueEntry.EndTimeUtc;
            }

            /* Project (R/C/G) */
            unitInfo.ProjectID = queueEntry.ProjectID;
            unitInfo.ProjectRun = queueEntry.ProjectRun;
            unitInfo.ProjectClone = queueEntry.ProjectClone;
            unitInfo.ProjectGen = queueEntry.ProjectGen;

            /* FoldingID and Team from Queue Entry */
            unitInfo.FoldingID = queueEntry.FoldingID;
            unitInfo.Team = (int)queueEntry.TeamNumber;

            /* Core ID */
            unitInfo.CoreID = queueEntry.CoreNumberHex.ToUpperInvariant();
         }
      }

      private static void UpdateUnitInfoFromLogData(UnitInfo unitInfo, ClientRunData clientRunData, UnitRunData unitRunData, UnitInfoLogData unitInfoLogData)
      {
         Debug.Assert(unitInfo != null);
         Debug.Assert(clientRunData != null);
         Debug.Assert(unitRunData != null);
         // unitInfoLogData can be null

         /* Project (R/C/G) (Could have already been read through Queue) */
         if (unitInfo.ProjectIsUnknown())
         {
            unitInfo.ProjectID = unitRunData.ProjectID;
            unitInfo.ProjectRun = unitRunData.ProjectRun;
            unitInfo.ProjectClone = unitRunData.ProjectClone;
            unitInfo.ProjectGen = unitRunData.ProjectGen;
         }

         if (unitRunData.Threads > 1)
         {
            unitInfo.SlotType = SlotType.CPU;
         }

         if (unitInfoLogData != null)
         {
            unitInfo.ProteinName = unitInfoLogData.ProteinName;

            /* Tag (Could have already been read through Queue) */
            if (unitInfo.ProteinTag.Length == 0)
            {
               unitInfo.ProteinTag = unitInfoLogData.ProteinTag;
            }

            /* DownloadTime (Could have already been read through Queue) */
            if (unitInfo.DownloadTime.IsUnknown())
            {
               unitInfo.DownloadTime = unitInfoLogData.DownloadTime;
            }

            /* DueTime (Could have already been read through Queue) */
            if (unitInfo.DueTime.IsUnknown())
            {
               unitInfo.DueTime = unitInfoLogData.DueTime;
            }

            /* FinishedTime (Not available in unitinfo log) */

            /* Project (R/C/G) (Could have already been read through Queue) */
            if (unitInfo.ProjectIsUnknown())
            {
               unitInfo.ProjectID = unitInfoLogData.ProjectID;
               unitInfo.ProjectRun = unitInfoLogData.ProjectRun;
               unitInfo.ProjectClone = unitInfoLogData.ProjectClone;
               unitInfo.ProjectGen = unitInfoLogData.ProjectGen;
            }
         }

         /* FoldingID and Team from last ClientRun (Could have already been read through Queue) */
         if (unitInfo.FoldingID == Constants.DefaultFoldingID && unitInfo.Team == Constants.DefaultTeam)
         {
            if (!String.IsNullOrEmpty(clientRunData.FoldingID))
            {
               unitInfo.FoldingID = clientRunData.FoldingID;
               unitInfo.Team = clientRunData.Team;
            }
         }

         // The queue will have the FoldingID and Team that was set in the client when the work unit was assigned.
         // If the user subsequently changed their FoldingID and Team before this unit was completed the
         // FoldingID and Team read from the queue will NOT reflect that change.
         //if (unitInfo.FoldingID != clientRunData.FoldingID || unitInfo.Team != clientRunData.Team)
         //{
         //   if (!String.IsNullOrEmpty(clientRunData.FoldingID))
         //   {
         //      unitInfo.FoldingID = clientRunData.FoldingID;
         //      unitInfo.Team = clientRunData.Team;
         //   }
         //}
      }
   }
}
