using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using HFM.Client.ObjectModel;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Log;

namespace HFM.Core.Client
{
    internal class WorkUnitCollectionBuilder
    {
        public ILogger Logger { get; }
        public ClientSettings Settings { get; }

        private readonly UnitCollection _units;
        private readonly Options _options;
        private readonly SlotRun _slotRun;

        public WorkUnitCollectionBuilder(ILogger logger, ClientSettings settings, UnitCollection units, Options options, SlotRun slotRun)
        {
            Logger = logger ?? NullLogger.Instance;
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _units = units ?? throw new ArgumentNullException(nameof(units));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _slotRun = slotRun;
        }

        public WorkUnitCollection BuildForSlot(int slotID, WorkUnit currentWorkUnit)
        {
            if (currentWorkUnit == null) throw new ArgumentNullException(nameof(currentWorkUnit));

            if (Logger.IsDebugEnabled && _slotRun != null)
            {
                foreach (var s in LogLineEnumerable.Create(_slotRun).Where(x => x.Data is LogLineDataParserError))
                {
                    Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, $"Failed to parse log line: {s}"));
                }
            }

            return BuildWorkUnits(slotID, currentWorkUnit);
        }

        private WorkUnitCollection BuildWorkUnits(int slotID, WorkUnit currentWorkUnit)
        {
            Debug.Assert(currentWorkUnit != null);

            bool foundCurrentUnitInfo = false;
            var workUnits = new WorkUnitCollection();

            foreach (var unit in _units.Where(x => x.Slot == slotID))
            {
                var projectInfo = unit.ToProjectInfo();
                if (projectInfo.EqualsProject(currentWorkUnit) &&
                    unit.AssignedDateTime.GetValueOrDefault().Equals(currentWorkUnit.Assigned))
                {
                    foundCurrentUnitInfo = true;
                }

                // Get the Log Lines for this queue position from the reader
                var unitRun = GetUnitRun(_slotRun, unit.ID.GetValueOrDefault(), projectInfo);
                if (unitRun == null)
                {
                    string message = $"Could not find log section for Slot {slotID} {projectInfo}.";
                    Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, message));
                }

                WorkUnit workUnit = BuildWorkUnit(unit, _options, unitRun);
                if (workUnit != null)
                {
                    workUnits.Add(workUnit);
                    if (unit.State.Equals("RUNNING", StringComparison.OrdinalIgnoreCase))
                    {
                        workUnits.CurrentID = workUnit.ID;
                    }
                }
            }

            // if no RUNNING WU found
            if (workUnits.CurrentID == WorkUnitCollection.NoID)
            {
                // look for a WU with READY state
                var unit = _units.FirstOrDefault(x => x.Slot == slotID && x.State.Equals("READY", StringComparison.OrdinalIgnoreCase));
                if (unit != null)
                {
                    workUnits.CurrentID = unit.ID.GetValueOrDefault();
                }
            }

            // if the current unit has already left the UnitCollection then find the log section and update here
            if (!foundCurrentUnitInfo)
            {
                // Get the Log Lines for this queue position from the reader
                var unitRun = GetUnitRun(_slotRun, currentWorkUnit.ID, currentWorkUnit);
                if (unitRun != null)
                {
                    // create a copy of the current WorkUnit object so we're not working with an
                    // instance that is referenced by a SlotModel that is bound to the grid - Issue 277
                    var workUnitCopy = currentWorkUnit.Copy();

                    PopulateWorkUnitFromLogData(workUnitCopy, unitRun);
                    workUnits.Add(workUnitCopy);
                }
            }

            return workUnits;
        }

        private static UnitRun GetUnitRun(SlotRun slotRun, int queueIndex, IProjectInfo projectInfo)
        {
            return slotRun?.UnitRuns.LastOrDefault(x => x.QueueIndex == queueIndex && projectInfo.EqualsProject(x.Data.ToProjectInfo()));
        }

        private static WorkUnit BuildWorkUnit(Unit unit, Options options, UnitRun unitRun)
        {
            Debug.Assert(unit != null);
            Debug.Assert(options != null);

            var workUnit = new WorkUnit();
            PopulateWorkUnitFromFahClientData(workUnit, unit, options);
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
            workUnit.Frames = unitRun.Data.Frames;
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
                workUnit.Finished = DateTime.UtcNow;
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

        private static void PopulateWorkUnitFromFahClientData(WorkUnit workUnit, Unit unit, Options options)
        {
            Debug.Assert(workUnit != null);
            Debug.Assert(unit != null);
            Debug.Assert(options != null);

            workUnit.ID = unit.ID.GetValueOrDefault();

            workUnit.Assigned = unit.AssignedDateTime.GetValueOrDefault();

            workUnit.Timeout = unit.TimeoutDateTime.GetValueOrDefault();

            /* Project (R/C/G) */
            workUnit.ProjectID = unit.Project.GetValueOrDefault();
            workUnit.ProjectRun = unit.Run.GetValueOrDefault();
            workUnit.ProjectClone = unit.Clone.GetValueOrDefault();
            workUnit.ProjectGen = unit.Gen.GetValueOrDefault();

            /* FoldingID and Team from Queue Entry */
            workUnit.FoldingID = options[Options.User] ?? Unknown.Value;
            workUnit.Team = ToNullableInt32(options[Options.Team]).GetValueOrDefault();

            /* Core ID */
            workUnit.CoreID = unit.Core.Replace("0x", String.Empty).ToUpperInvariant();
        }

        private static int? ToNullableInt32(string value)
        {
            return Int32.TryParse(value, out var result) ? (int?)result : null;
        }
    }
}
