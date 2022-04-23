using System.Diagnostics;

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
        private readonly ClientRun _clientRun;
        private readonly DateTime _unitRetrievalTime;

        public WorkUnitCollectionBuilder(ILogger logger, ClientSettings settings, UnitCollection units, Options options, ClientRun clientRun, DateTime unitRetrievalTime)
        {
            Logger = logger ?? NullLogger.Instance;
            Settings = settings ?? throw new ArgumentNullException(nameof(settings));
            _units = units;
            _options = options;
            _clientRun = clientRun;
            _unitRetrievalTime = unitRetrievalTime;
        }

        public WorkUnitCollection BuildForSlot(int slotID, WorkUnit previousWorkUnit)
        {
            if (_units is null) return new WorkUnitCollection();
            if (previousWorkUnit is null) throw new ArgumentNullException(nameof(previousWorkUnit));

            var slotRun = GetSlotRun(slotID);
            if (Logger.IsDebugEnabled && slotRun != null)
            {
                foreach (var s in LogLineEnumerable.Create(slotRun).Where(x => x.Data is LogLineDataParserError))
                {
                    Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, $"Failed to parse log line: {s}"));
                }
            }

            return BuildWorkUnitCollection(slotID, previousWorkUnit);
        }

        private WorkUnitCollection BuildWorkUnitCollection(int slotID, WorkUnit previousWorkUnit)
        {
            Debug.Assert(previousWorkUnit != null);

            var workUnits = new WorkUnitCollection(_units.Where(x => x.Slot == slotID).Select(x => BuildWorkUnit(slotID, x)));
            var currentID = GetCurrentWorkUnitID(slotID);
            if (currentID.HasValue)
            {
                workUnits.CurrentID = currentID.Value;
            }

            // if the previous unit has already left the UnitCollection then find the log section and update here
            if (PreviousWorkUnitShouldBeCompleted(workUnits, previousWorkUnit))
            {
                var unitRun = GetUnitRun(slotID, previousWorkUnit.ID, previousWorkUnit);
                if (unitRun != null)
                {
                    workUnits.Add(CompleteWorkUnitWithLogData(previousWorkUnit, unitRun));
                }
            }

            return workUnits;
        }

        private static bool PreviousWorkUnitShouldBeCompleted(WorkUnitCollection workUnits, WorkUnit previousWorkUnit) =>
            !workUnits.HasWorkUnit(previousWorkUnit) &&
            !workUnits.ContainsID(previousWorkUnit.ID);

        private WorkUnit CompleteWorkUnitWithLogData(WorkUnit previousWorkUnit, UnitRun unitRun)
        {
            // create a copy of the previous WorkUnit so we're not mutating a given instance
            var workUnitCopy = previousWorkUnit.Copy();
            workUnitCopy.UnitRetrievalTime = _unitRetrievalTime;

            PopulateWorkUnitFromLogData(workUnitCopy, unitRun);
            return workUnitCopy;
        }

        private SlotRun GetSlotRun(int slotID) =>
            _clientRun != null && _clientRun.SlotRuns.TryGetValue(slotID, out var slotRun)
                ? slotRun
                : null;

        private UnitRun GetUnitRun(int slotID, int queueIndex, IProjectInfo projectInfo) =>
            GetSlotRun(slotID)?.UnitRuns.LastOrDefault(x => x.QueueIndex == queueIndex && projectInfo.EqualsProject(ToProjectInfo(x.Data)));

        private WorkUnit BuildWorkUnit(int slotID, Unit unit)
        {
            Debug.Assert(unit != null);

            var workUnit = new WorkUnit { UnitRetrievalTime = _unitRetrievalTime };
            PopulateWorkUnitFromClientData(workUnit, unit, _options);

            var projectInfo = ToProjectInfo(unit);
            var unitRun = GetUnitRun(slotID, unit.ID.GetValueOrDefault(), projectInfo);
            if (unitRun == null)
            {
                string message = $"Could not find log section for Slot {slotID} {projectInfo}.";
                Logger.Debug(String.Format(Logging.Logger.NameFormat, Settings.Name, message));
            }
            else
            {
                PopulateWorkUnitFromLogData(workUnit, unitRun);
            }

            return workUnit;
        }

        private int? GetCurrentWorkUnitID(int slotID)
        {
            int? currentID = _units
                .FirstOrDefault(x => x.Slot == slotID && x.State.Equals("RUNNING", StringComparison.OrdinalIgnoreCase))
                ?.ID;

            if (!currentID.HasValue)
            {
                currentID = _units
                    .FirstOrDefault(x => x.Slot == slotID && x.State.Equals("READY", StringComparison.OrdinalIgnoreCase))
                    ?.ID;
            }

            return currentID;
        }

        private static void PopulateWorkUnitFromClientData(WorkUnit workUnit, Unit unit, Options options)
        {
            Debug.Assert(workUnit != null);
            Debug.Assert(unit != null);

            workUnit.ID = unit.ID.GetValueOrDefault();
            workUnit.Assigned = unit.AssignedDateTime.GetValueOrDefault();
            workUnit.Timeout = unit.TimeoutDateTime.GetValueOrDefault();

            workUnit.ProjectID = unit.Project.GetValueOrDefault();
            workUnit.ProjectRun = unit.Run.GetValueOrDefault();
            workUnit.ProjectClone = unit.Clone.GetValueOrDefault();
            workUnit.ProjectGen = unit.Gen.GetValueOrDefault();

            workUnit.FoldingID = options?[Options.User] ?? Unknown.Value;
            workUnit.Team = ToNullableInt32(options?[Options.Team]).GetValueOrDefault();

            workUnit.CoreID = unit.Core?.Replace("0x", String.Empty).ToUpperInvariant();
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
            workUnit.Platform = unitRun.Data.Platform;
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

        private static bool IsTerminating(WorkUnit workUnit) =>
            workUnit.UnitResult.IsTerminating() || IsUnknownEnumTerminating(workUnit);

        private static bool IsUnknownEnumTerminating(WorkUnit workUnit) =>
            workUnit.UnitResult == WorkUnitResult.UnknownEnum &&
            workUnit.LogLines.Any(x => x.LineType == LogLineType.WorkUnitTooManyErrors);

        private static Version ParseCoreVersion(string value) =>
            value is null
                ? null
                : Version.TryParse(value, out var version)
                    ? version
                    : null;

        private static int? ToNullableInt32(string value) =>
            Int32.TryParse(value, out var result)
                ? (int?)result
                : null;

        private static ProjectInfo ToProjectInfo(Unit unit) =>
            new ProjectInfo
            {
                ProjectID = unit.Project.GetValueOrDefault(),
                ProjectRun = unit.Run.GetValueOrDefault(),
                ProjectClone = unit.Clone.GetValueOrDefault(),
                ProjectGen = unit.Gen.GetValueOrDefault()
            };

        private static ProjectInfo ToProjectInfo(UnitRunData data) =>
            new ProjectInfo
            {
                ProjectID = data.ProjectID,
                ProjectRun = data.ProjectRun,
                ProjectClone = data.ProjectClone,
                ProjectGen = data.ProjectGen
            };
    }
}
