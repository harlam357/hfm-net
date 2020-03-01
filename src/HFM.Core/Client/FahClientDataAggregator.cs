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
using HFM.Core.WorkUnits;
using HFM.Log;

namespace HFM.Core.Client
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
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        /// <summary>
        /// Aggregate Data and return WorkUnit Dictionary.
        /// </summary>
        public DataAggregatorResult AggregateData(ClientRun clientRun, UnitCollection unitCollection, Info info, Options options,
                                                  SlotOptions slotOptions, WorkUnit currentWorkUnit, int slotId)
        {
            if (clientRun == null) throw new ArgumentNullException(nameof(clientRun));
            if (unitCollection == null) throw new ArgumentNullException(nameof(unitCollection));
            if (options == null) throw new ArgumentNullException(nameof(options));
            if (slotOptions == null) throw new ArgumentNullException(nameof(slotOptions));
            if (currentWorkUnit == null) throw new ArgumentNullException(nameof(currentWorkUnit));

            var result = new DataAggregatorResult();
            result.CurrentUnitIndex = -1;

            SlotRun slotRun = null;
            if (clientRun.SlotRuns.ContainsKey(slotId))
            {
                slotRun = clientRun.SlotRuns[slotId];
            }
            result.StartTime = clientRun.Data.StartTime;

            if (Logger.IsDebugEnabled)
            {
                foreach (var s in LogLineEnumerable.Create(clientRun).Where(x => x.Data is LogLineDataParserError))
                {
                    Logger.DebugFormat(Logging.Logger.NameFormat, ClientName, $"Failed to parse log line: {s}");
                }
            }

            BuildWorkUnits(result, slotRun, unitCollection, options, slotOptions, currentWorkUnit, slotId);
            result.WorkUnitInfos = BuildSlotWorkUnitInfos(unitCollection, info, slotOptions, slotId);

            if (result.WorkUnits.ContainsKey(result.CurrentUnitIndex) && result.WorkUnits[result.CurrentUnitIndex].LogLines != null)
            {
                result.CurrentLogLines = result.WorkUnits[result.CurrentUnitIndex].LogLines;
            }
            else if (slotRun != null)
            {
                result.CurrentLogLines = LogLineEnumerable.Create(slotRun).ToList();
            }
            else
            {
                result.CurrentLogLines = LogLineEnumerable.Create(clientRun).ToList();
            }

            return result;
        }

        private static SlotWorkUnitDictionary BuildSlotWorkUnitInfos(IEnumerable<Unit> unitCollection, Info info, SlotOptions slotOptions, int slotId)
        {
            SlotWorkUnitDictionary d = null;
            foreach (var unit in unitCollection.Where(unit => unit.Slot == slotId))
            {
                if (d == null)
                {
                    d = new SlotWorkUnitDictionary { CurrentWorkUnitKey = -1 };
                }

                var wui = new SlotWorkUnitInfo();
                wui.State = unit.StateEnum.ToString();
                wui.WaitingOn = unit.WaitingOn;
                wui.Attempts = unit.Attempts;
                wui.NextAttempt = unit.NextAttemptTimeSpan.GetValueOrDefault();
                wui.NumberOfSmpCores = info.System.CpuCount;
                wui.AssignedDateTimeUtc = unit.AssignedDateTime.GetValueOrDefault();
                wui.ProjectID = unit.Project;
                wui.ProjectRun = unit.Run;
                wui.ProjectClone = unit.Clone;
                wui.ProjectGen = unit.Gen;
                wui.SlotID = slotId;
                wui.WorkServer = unit.WorkServer;
                wui.CPU = GetCPU(info, slotOptions);
                wui.OperatingSystem = GetOperatingSystem(info.System);
                // Memory Value is in Gigabytes - turn into Megabytes and truncate
                wui.Memory = (int)(info.System.MemoryValue.GetValueOrDefault() * 1024);
                d.Add(unit.Id, wui);

                if (unit.StateEnum == UnitState.Running)
                {
                    d.CurrentWorkUnitKey = unit.Id;
                }
            }

            if (d != null)
            {
                // if no running index and at least something in the queue
                if (d.CurrentWorkUnitKey == -1 && d.Count != 0)
                {
                    // take the minimum queue id
                    d.CurrentWorkUnitKey = d.Keys.First();
                }
            }

            return d;
        }

        private static string GetOperatingSystem(SystemInfo systemInfo)
        {
            return !String.IsNullOrWhiteSpace(systemInfo.OperatingSystemArchitecture)
               ? String.Format(CultureInfo.InvariantCulture, "{0} {1}", systemInfo.OperatingSystem, systemInfo.OperatingSystemArchitecture)
               : systemInfo.OperatingSystem;
        }

        private static string GetCPU(Info info, SlotOptions slotOptions)
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
                return info.System.Cpu;
            }

            return String.Empty;
        }

        private void BuildWorkUnits(DataAggregatorResult result,
                                    SlotRun slotRun,
                                    ICollection<Unit> unitCollection,
                                    Options options,
                                    SlotOptions slotOptions,
                                    WorkUnit currentWorkUnit,
                                    int slotId)
        {
            Debug.Assert(unitCollection != null);
            Debug.Assert(options != null);
            Debug.Assert(slotOptions != null);
            Debug.Assert(currentWorkUnit != null);

            result.WorkUnits = new Dictionary<int, WorkUnit>();

            bool foundCurrentUnitInfo = false;

            foreach (var unit in unitCollection.Where(x => x.Slot == slotId))
            {
                var projectInfo = unit.ToProjectInfo();
                if (projectInfo.EqualsProject(currentWorkUnit) &&
                    unit.AssignedDateTime.GetValueOrDefault().Equals(currentWorkUnit.DownloadTime))
                {
                    foundCurrentUnitInfo = true;
                }

                // Get the Log Lines for this queue position from the reader
                var unitRun = GetUnitRun(slotRun, unit.Id, projectInfo);
                if (unitRun == null)
                {
                    string message = $"Could not find log section for Slot {slotId} {projectInfo}.";
                    Logger.DebugFormat(Logging.Logger.NameFormat, ClientName, message);
                }

                WorkUnit workUnit = BuildWorkUnit(unit, options, slotOptions, unitRun);
                if (workUnit != null)
                {
                    result.WorkUnits.Add(unit.Id, workUnit);
                    if (unit.StateEnum == UnitState.Running)
                    {
                        result.CurrentUnitIndex = unit.Id;
                    }
                }
            }

            // if no running WU found
            if (result.CurrentUnitIndex == -1)
            {
                // look for a WU with Ready state
                var unit = unitCollection.FirstOrDefault(x => x.Slot == slotId && x.StateEnum == UnitState.Ready);
                if (unit != null)
                {
                    result.CurrentUnitIndex = unit.Id;
                }
            }

            // if the current unit has already left the UnitCollection then find the log section and update here
            if (!foundCurrentUnitInfo)
            {
                // Get the Log Lines for this queue position from the reader
                var unitRun = GetUnitRun(slotRun, currentWorkUnit.QueueIndex, currentWorkUnit);
                if (unitRun != null)
                {
                    // create a clone of the current WorkUnit object so we're not working with an
                    // instance that is referenced by a SlotModel that is bound to the grid - Issue 277
                    WorkUnit currentClone = currentWorkUnit.DeepClone();

                    PopulateWorkUnitFromLogData(currentClone, unitRun);
                    result.WorkUnits.Add(currentClone.QueueIndex, currentClone);
                }
            }
        }

        private static UnitRun GetUnitRun(SlotRun slotRun, int queueIndex, IProjectInfo projectInfo)
        {
            return slotRun?.UnitRuns.LastOrDefault(x => x.QueueIndex == queueIndex && projectInfo.EqualsProject(x.Data.ToProjectInfo()));
        }

        private static WorkUnit BuildWorkUnit(Unit queueEntry, Options options, SlotOptions slotOptions, UnitRun unitRun)
        {
            Debug.Assert(queueEntry != null);
            Debug.Assert(options != null);
            Debug.Assert(slotOptions != null);

            var workUnit = new WorkUnit();
            PopulateWorkUnitFromFahClientData(workUnit, queueEntry, options, slotOptions);
            if (unitRun != null)
            {
                PopulateWorkUnitFromLogData(workUnit, unitRun);
            }
            return workUnit;
        }

        private static void PopulateWorkUnitFromLogData(WorkUnit workUnit, UnitRun unitRun)
        {
            Debug.Assert(workUnit != null);
            Debug.Assert(unitRun != null);

            workUnit.LogLines = LogLineEnumerable.Create(unitRun).ToList();
            workUnit.FrameData = unitRun.Data.FrameDataDictionary;
            workUnit.UnitStartTimeStamp = unitRun.Data.UnitStartTimeStamp ?? TimeSpan.MinValue;
            workUnit.FramesObserved = unitRun.Data.FramesObserved;
            workUnit.CoreVersion = ParseCoreVersion(unitRun.Data.CoreVersion);
            workUnit.UnitResult = WorkUnitResultString.ToWorkUnitResult(unitRun.Data.WorkUnitResult);

            // there is no finished time available from the client API
            // since the unit history database won't write the same
            // result twice, the first time this hits use the local UTC
            // value for the finished time... not as good as what was
            // available with v6.
            if (IsTerminating(workUnit))
            {
               workUnit.FinishedTime = DateTime.UtcNow;
            }
        }

        private static bool IsTerminating(WorkUnit workUnit)
        {
            return workUnit.UnitResult.IsTerminating() ||
                   IsUnknownEnumTerminating(workUnit);
        }

        private static bool IsUnknownEnumTerminating(WorkUnit workUnit)
        {
            return workUnit.UnitResult == WorkUnitResult.UnknownEnum &&
                   workUnit.LogLines.Any(x => x.LineType == LogLineType.WorkUnitTooManyErrors);
        }

        private static float ParseCoreVersion(string coreVer)
        {
            if (coreVer is null) return 0.0f;

            if (Single.TryParse(coreVer, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            {
                return value;
            }
            // Try to parse Core Versions in the 0.#.## format
            if (coreVer.StartsWith("0."))
            {
                if (Single.TryParse(coreVer.Substring(2), NumberStyles.Number, CultureInfo.InvariantCulture, out value))
                {
                    return value;
                }
            }
            return 0.0f;
        }

        private static void PopulateWorkUnitFromFahClientData(WorkUnit workUnit, Unit queueEntry, Options options, SlotOptions slotOptions)
        {
            Debug.Assert(workUnit != null);
            Debug.Assert(queueEntry != null);
            Debug.Assert(options != null);
            Debug.Assert(slotOptions != null);

            workUnit.QueueIndex = queueEntry.Id;

            /* DownloadTime (AssignedDateTime from HFM.Client API) */
            workUnit.DownloadTime = queueEntry.AssignedDateTime.GetValueOrDefault();

            /* DueTime (TimeoutDateTime from HFM.Client API) */
            workUnit.DueTime = queueEntry.TimeoutDateTime.GetValueOrDefault();

            /* Project (R/C/G) */
            workUnit.ProjectID = queueEntry.Project;
            workUnit.ProjectRun = queueEntry.Run;
            workUnit.ProjectClone = queueEntry.Clone;
            workUnit.ProjectGen = queueEntry.Gen;

            /* FoldingID and Team from Queue Entry */
            workUnit.FoldingID = options.User ?? WorkUnit.DefaultFoldingID;
            workUnit.Team = options.Team ?? WorkUnit.DefaultTeam;
            workUnit.SlotType = slotOptions.ToSlotType();

            /* Core ID */
            workUnit.CoreID = queueEntry.Core.Replace("0x", String.Empty).ToUpperInvariant();
        }
    }
}
