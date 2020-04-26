
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using HFM.Client.ObjectModel;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Log;

namespace HFM.Core.Client
{
    internal class FahClientMessageAggregator
    {
        public IFahClient FahClient { get; }
        public SlotModel SlotModel { get; }

        public FahClientMessageAggregator(IFahClient fahClient, SlotModel slotModel)
        {
            FahClient = fahClient;
            SlotModel = slotModel;
        }

        public ClientMessageAggregatorResult AggregateData()
        {
            var result = new ClientMessageAggregatorResult();
            result.CurrentUnitIndex = -1;

            ClientRun clientRun = FahClient.Messages.Log.ClientRuns.Last();
            SlotRun slotRun = null;
            if (clientRun.SlotRuns.ContainsKey(SlotModel.SlotID))
            {
                slotRun = clientRun.SlotRuns[SlotModel.SlotID];
            }
            result.StartTime = clientRun.Data.StartTime;

            if (FahClient.Logger.IsDebugEnabled)
            {
                foreach (var s in LogLineEnumerable.Create(clientRun).Where(x => x.Data is LogLineDataParserError))
                {
                    FahClient.Logger.Debug(String.Format(Logger.NameFormat, FahClient.Settings.Name, $"Failed to parse log line: {s}"));
                }
            }

            var unitCollection = FahClient.Messages.UnitCollection;
            var options = FahClient.Messages.Options;
            var slotOptions = FahClient.Messages.SlotCollection.FirstOrDefault(x => x.ID == SlotModel.SlotID)?.SlotOptions;
            var currentWorkUnit = SlotModel.WorkUnitModel.Data;
            var info = FahClient.Messages.Info;

            BuildWorkUnits(result, slotRun, unitCollection, options, slotOptions, currentWorkUnit, SlotModel.SlotID);
            result.WorkUnitInfos = BuildSlotWorkUnitInfos(unitCollection, info, slotOptions, SlotModel.SlotID);

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
                wui.State = unit.State;
                wui.WaitingOn = unit.WaitingOn;
                wui.Attempts = unit.Attempts.GetValueOrDefault();
                wui.NextAttempt = unit.NextAttemptTimeSpan.GetValueOrDefault();
                wui.NumberOfSmpCores = info.System.CPUs.GetValueOrDefault();
                wui.AssignedDateTimeUtc = unit.AssignedDateTime.GetValueOrDefault();
                wui.ProjectID = unit.Project.GetValueOrDefault();
                wui.ProjectRun = unit.Run.GetValueOrDefault();
                wui.ProjectClone = unit.Clone.GetValueOrDefault();
                wui.ProjectGen = unit.Gen.GetValueOrDefault();
                wui.SlotID = slotId;
                wui.WorkServer = unit.WorkServer;
                wui.CPU = GetCPUString(info, slotOptions);
                wui.OperatingSystem = info.System.OS;
                // Memory Value is in Gigabytes - turn into Megabytes and truncate
                wui.Memory = (int)(info.System.MemoryValue.GetValueOrDefault() * 1024);
                d.Add(unit.ID.GetValueOrDefault(), wui);

                if (unit.State.Equals("RUNNING", StringComparison.OrdinalIgnoreCase))
                {
                    d.CurrentWorkUnitKey = unit.ID.GetValueOrDefault();
                }
            }

            if (d != null)
            {
                // if no RUNNING index and at least something in the queue
                if (d.CurrentWorkUnitKey == -1 && d.Count != 0)
                {
                    // take the minimum queue id
                    d.CurrentWorkUnitKey = d.Keys.First();
                }
            }

            return d;
        }

        private static string GetCPUString(Info info, SlotOptions slotOptions)
        {
            var gpuIndex = slotOptions[Options.GPUIndex];
            return gpuIndex != null 
                ? Int32.TryParse(gpuIndex, out var i) ? info.System.GPUInfos[i].FriendlyName : String.Empty 
                : info.System.CPU;
        }

        private void BuildWorkUnits(ClientMessageAggregatorResult result,
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
                var unitRun = GetUnitRun(slotRun, unit.ID.GetValueOrDefault(), projectInfo);
                if (unitRun == null)
                {
                    string message = $"Could not find log section for Slot {slotId} {projectInfo}.";
                    FahClient.Logger.Debug(String.Format(Logger.NameFormat, FahClient.Settings.Name, message));
                }

                WorkUnit workUnit = BuildWorkUnit(unit, options, slotOptions, unitRun);
                if (workUnit != null)
                {
                    result.WorkUnits.Add(unit.ID.GetValueOrDefault(), workUnit);
                    if (unit.State.Equals("RUNNING", StringComparison.OrdinalIgnoreCase))
                    {
                        result.CurrentUnitIndex = unit.ID.GetValueOrDefault();
                    }
                }
            }

            // if no RUNNING WU found
            if (result.CurrentUnitIndex == -1)
            {
                // look for a WU with READY state
                var unit = unitCollection.FirstOrDefault(x => x.Slot == slotId && x.State.Equals("READY", StringComparison.OrdinalIgnoreCase));
                if (unit != null)
                {
                    result.CurrentUnitIndex = unit.ID.GetValueOrDefault();
                }
            }

            // if the current unit has already left the UnitCollection then find the log section and update here
            if (!foundCurrentUnitInfo)
            {
                // Get the Log Lines for this queue position from the reader
                var unitRun = GetUnitRun(slotRun, currentWorkUnit.QueueIndex, currentWorkUnit);
                if (unitRun != null)
                {
                    // create a copy of the current WorkUnit object so we're not working with an
                    // instance that is referenced by a SlotModel that is bound to the grid - Issue 277
                    var workUnitCopy = currentWorkUnit.Copy();

                    PopulateWorkUnitFromLogData(workUnitCopy, unitRun);
                    result.WorkUnits.Add(workUnitCopy.QueueIndex, workUnitCopy);
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

            workUnit.QueueIndex = queueEntry.ID.GetValueOrDefault();

            /* DownloadTime (AssignedDateTime from HFM.Client API) */
            workUnit.DownloadTime = queueEntry.AssignedDateTime.GetValueOrDefault();

            /* DueTime (TimeoutDateTime from HFM.Client API) */
            workUnit.DueTime = queueEntry.TimeoutDateTime.GetValueOrDefault();

            /* Project (R/C/G) */
            workUnit.ProjectID = queueEntry.Project.GetValueOrDefault();
            workUnit.ProjectRun = queueEntry.Run.GetValueOrDefault();
            workUnit.ProjectClone = queueEntry.Clone.GetValueOrDefault();
            workUnit.ProjectGen = queueEntry.Gen.GetValueOrDefault();

            /* FoldingID and Team from Queue Entry */
            workUnit.FoldingID = options[Options.User] ?? Unknown.Value;
            workUnit.Team = ToNullableInt32(options[Options.Team]).GetValueOrDefault();
            workUnit.SlotType = SlotTypeConvert.FromSlotOptions(slotOptions);

            /* Core ID */
            workUnit.CoreID = queueEntry.Core.Replace("0x", String.Empty).ToUpperInvariant();
        }

        private static int? ToNullableInt32(string value)
        {
            return Int32.TryParse(value, out var result) ? (int?)result : null;
        }
    }
}
