/*
 * HFM.NET - Fah Client Data Aggregator Class
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

using Castle.Core.Logging;

using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   internal class FahClientDataAggregator
   {
      /// <summary>
      /// Client name.
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
      /// Aggregate Data and return UnitInfo Dictionary.
      /// </summary>
      public DataAggregatorResult AggregateData(ClientRun clientRun, UnitCollection unitCollection, Info info, Options options,
                                                SlotOptions slotOptions, UnitInfo currentUnitInfo, int slotId)
      {
         if (clientRun == null) throw new ArgumentNullException("clientRun");
         if (unitCollection == null) throw new ArgumentNullException("unitCollection");
         if (options == null) throw new ArgumentNullException("options");
         if (slotOptions == null) throw new ArgumentNullException("slotOptions");
         if (currentUnitInfo == null) throw new ArgumentNullException("currentUnitInfo");

         var result = new DataAggregatorResult();
         result.CurrentUnitIndex = -1;

         SlotRun slotRun = null;
         if (clientRun.SlotRuns.ContainsKey(slotId))
         {
            slotRun = clientRun.SlotRuns[slotId];
         }
         result.StartTime = clientRun.Data.StartTime;
         result.Arguments = clientRun.Data.Arguments;
         result.ClientVersion = clientRun.Data.ClientVersion;
         result.UserID = clientRun.Data.UserID;
         result.MachineID = clientRun.Data.MachineID;
         result.Status = slotRun != null ? slotRun.Data.Status : SlotStatus.Unknown;

         if (Logger.IsDebugEnabled)
         {
            foreach (var s in clientRun.Where(x => x.LineType == LogLineType.Error))
            {
               Logger.Debug(Constants.ClientNameFormat, ClientName, String.Format("Failed to parse log line: {0}", s));
            }
         }

         GenerateUnitInfoDataFromQueue(result, slotRun, unitCollection, options, slotOptions, currentUnitInfo, slotId);
         result.Queue = BuildClientQueue(unitCollection, info, slotOptions, slotId);

         if (result.UnitInfos.ContainsKey(result.CurrentUnitIndex) && result.UnitInfos[result.CurrentUnitIndex].LogLines != null)
         {
            result.CurrentLogLines = result.UnitInfos[result.CurrentUnitIndex].LogLines;
         }
         else if (slotRun != null)
         {
            result.CurrentLogLines = slotRun.ToList();
         }
         else
         {
            result.CurrentLogLines = clientRun.ToList();
         }

         return result;
      }

      private static ClientQueue BuildClientQueue(IEnumerable<Unit> unitCollection, Info info, SlotOptions slotOptions, int slotId)
      {
         ClientQueue cq = null;
         foreach (var unit in unitCollection.Where(unit => unit.Slot == slotId))
         {
            // don't create a queue until we find a unit that matches this slot id
            if (cq == null)
            {
               cq = new ClientQueue { ClientType = ClientType.FahClient, CurrentIndex = -1 };
            }

            var cqe = new ClientQueueEntry();
            cqe.EntryStatusLiteral = unit.StateEnum.ToString();
            cqe.WaitingOn = unit.WaitingOn;
            cqe.Attempts = unit.Attempts;
            cqe.NextAttempt = unit.NextAttemptTimeSpan.GetValueOrDefault();
            cqe.NumberOfSmpCores = info.System.CpuCount;
            cqe.BeginTimeUtc = unit.AssignedDateTime.GetValueOrDefault();
            cqe.BeginTimeLocal = unit.AssignedDateTime.GetValueOrDefault().ToLocalTime();
            cqe.ProjectID = unit.Project;
            cqe.ProjectRun = unit.Run;
            cqe.ProjectClone = unit.Clone;
            cqe.ProjectGen = unit.Gen;
            cqe.MachineID = slotId;
            cqe.ServerIP = unit.WorkServer;
            cqe.CpuString = GetCpuString(info, slotOptions);
            cqe.OsString = info.System.OperatingSystemEnum.ToOperatingSystemString(info.System.OperatingSystemArchitectureEnum);
            // Memory Value is in Gigabytes - turn into Megabytes and truncate
            cqe.Memory = (int)(info.System.MemoryValue.GetValueOrDefault() * 1024);
            cq.Add(unit.Id, cqe);

            if (unit.StateEnum == FahUnitStatus.Running)
            {
               cq.CurrentIndex = unit.Id;
            }
         }

         if (cq != null)
         {
            // if no running index and at least something in the queue
            if (cq.CurrentIndex == -1 && cq.Count != 0)
            {
               // take the minimum queue id
               cq.CurrentIndex = cq.Keys.First();
            }
         }

         return cq;
      }

      private static string GetCpuString(Info info, SlotOptions slotOptions)
      {
         if (slotOptions.GpuIndex.HasValue)
         {
            switch (slotOptions.GpuIndex)
            {
               case 0:
                  return info.System.GpuId0Type;
               case 1:
                  return info.System.GpuId1Type;
               case 2:
                  return info.System.GpuId2Type;
               case 3:
                  return info.System.GpuId3Type;
               case 4:
                  return info.System.GpuId4Type;
               case 5:
                  return info.System.GpuId5Type;
               case 6:
                  return info.System.GpuId6Type;
               case 7:
                  return info.System.GpuId7Type;
            }
         }
         else
         {
            return info.System.CpuType.ToCpuTypeString();
         }

         return String.Empty;
      }

      private void GenerateUnitInfoDataFromQueue(DataAggregatorResult result, SlotRun slotRun, ICollection<Unit> unitCollection, Options options,
                                                 SlotOptions slotOptions, UnitInfo currentUnitInfo, int slotId)
      {
         Debug.Assert(unitCollection != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);
         Debug.Assert(currentUnitInfo != null);

         result.UnitInfos = new Dictionary<int, UnitInfo>();

         bool foundCurrentUnitInfo = false;

         foreach (var unit in unitCollection)
         {
            if (unit.Slot != slotId)
            {
               // does not match requested slot
               continue;
            }

            var projectInfo = unit.ToProjectInfo();
            if (projectInfo.EqualsProject(currentUnitInfo) &&
                unit.AssignedDateTime.GetValueOrDefault().Equals(currentUnitInfo.DownloadTime))
            {
               foundCurrentUnitInfo = true;
            }

            // Get the Log Lines for this queue position from the reader
            var unitRun = GetUnitRun(slotRun, unit.Id, projectInfo);
            if (unitRun == null)
            {
               string message = String.Format(CultureInfo.CurrentCulture,
                  "Could not find log section for Slot {0} {1}. Cannot update log data for this unit.", slotId, projectInfo);
               Logger.Warn(Constants.ClientNameFormat, ClientName, message);
            }

            UnitInfo unitInfo = BuildUnitInfo(unit, options, slotOptions, unitRun);
            if (unitInfo != null)
            {
               result.UnitInfos.Add(unit.Id, unitInfo);
               if (unit.StateEnum == FahUnitStatus.Running)
               {
                  result.CurrentUnitIndex = unit.Id;
               }
            }
         }

         // if no running WU found
         if (result.CurrentUnitIndex == -1)
         {
            // look for a WU with Ready state
            var unit = unitCollection.FirstOrDefault(x => x.Slot == slotId && x.StateEnum == FahUnitStatus.Ready);
            if (unit != null)
            {
               result.CurrentUnitIndex = unit.Id;
            }
         }

         // if the current unit has already left the UnitCollection then find the log section and update here
         if (!foundCurrentUnitInfo)
         {
            // Get the Log Lines for this queue position from the reader
            var unitRun = GetUnitRun(slotRun, currentUnitInfo.QueueIndex, currentUnitInfo);
            if (unitRun != null)
            {
               // create a clone of the current UnitInfo object so we're not working with an
               // instance that is referenced by a SlotModel that is bound to the grid - Issue 277
               UnitInfo currentClone = currentUnitInfo.DeepClone();

               UpdateUnitInfoFromLogData(currentClone, unitRun);
               result.UnitInfos.Add(currentClone.QueueIndex, currentClone);
            }
         }
      }

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

      private UnitInfo BuildUnitInfo(Unit queueEntry, Options options, SlotOptions slotOptions, UnitRun unitRun)
      {
         Debug.Assert(queueEntry != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);

         var unit = new UnitInfo();
         UpdateUnitInfoFromFahClientData(unit, queueEntry, options, slotOptions);
         if (unitRun != null)
         {
            UpdateUnitInfoFromLogData(unit, unitRun);
         }

         return unit;
      }

      private void UpdateUnitInfoFromLogData(UnitInfo unitInfo, UnitRun unitRun)
      {
         Debug.Assert(unitInfo != null);
         Debug.Assert(unitRun != null);

         unitInfo.LogLines = unitRun.ToList();
         unitInfo.UnitStartTimeStamp = unitRun.Data.UnitStartTimeStamp ?? TimeSpan.MinValue;
         unitInfo.FramesObserved = unitRun.Data.FramesObserved;
         unitInfo.CoreVersion = unitRun.Data.CoreVersion;
         unitInfo.UnitResult = unitRun.Data.WorkUnitResult;

         // there is no finished time available from the client API
         // since the unit history database won't write the same
         // result twice, the first time this hits use the local UTC
         // value for the finished time... not as good as what was
         // available with v6.
         if (unitInfo.UnitResult == WorkUnitResult.FinishedUnit)
         {
            var finishedTime = DateTime.UtcNow;
            string message = String.Format(CultureInfo.CurrentCulture,
               "Setting Finished Time {0} for {1}.", finishedTime, unitInfo.ToProjectInfo());
            Logger.Debug(Constants.ClientNameFormat, ClientName, message);
            unitInfo.FinishedTime = DateTime.UtcNow;
         }
      }

      private static void UpdateUnitInfoFromFahClientData(UnitInfo unitInfo, Unit queueEntry, Options options, SlotOptions slotOptions)
      {
         Debug.Assert(unitInfo != null);
         Debug.Assert(queueEntry != null);
         Debug.Assert(options != null);
         Debug.Assert(slotOptions != null);

         unitInfo.QueueIndex = queueEntry.Id;

         /* DownloadTime (AssignedDateTime from HFM.Client API) */
         unitInfo.DownloadTime = queueEntry.AssignedDateTime.GetValueOrDefault();

         /* DueTime (TimeoutDateTime from HFM.Client API) */
         unitInfo.DueTime = queueEntry.TimeoutDateTime.GetValueOrDefault();

         /* FinishedTime */
         //if (queueEntryStatus.Equals(QueueEntryStatus.Finished) ||
         //    queueEntryStatus.Equals(QueueEntryStatus.ReadyForUpload))
         //{
         //   unit.FinishedTime = entry.EndTimeUtc;
         //}

         /* Project (R/C/G) */
         unitInfo.ProjectID = queueEntry.Project;
         unitInfo.ProjectRun = queueEntry.Run;
         unitInfo.ProjectClone = queueEntry.Clone;
         unitInfo.ProjectGen = queueEntry.Gen;

         /* FoldingID and Team from Queue Entry */
         unitInfo.FoldingID = options.User ?? Constants.DefaultFoldingID;
         unitInfo.Team = options.Team ?? Constants.DefaultTeam;
         unitInfo.SlotType = slotOptions.ToSlotType();

         /* Core ID */
         unitInfo.CoreID = queueEntry.Core.Replace("0x", String.Empty).ToUpperInvariant();
      }
   }
}
