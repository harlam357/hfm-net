
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

using HFM.Client.ObjectModel;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.Client
{
    // TODO: TimeFormatting should be a function of SlotModel, not view grid painting
    public enum TimeFormatting
    {
        None,
        Format1
    }

    public enum UnitTotalsType
    {
        All,
        ClientStart
    }

    public class SlotModel
    {
        #region IPreferenceSet

        public IPreferenceSet Prefs => _client.Preferences;

        private PPDCalculation PPDCalculation => Prefs.Get<PPDCalculation>(Preference.PPDCalculation);

        private BonusCalculation BonusCalculation => Prefs.Get<BonusCalculation>(Preference.BonusCalculation);

        private bool ShowVersions => Prefs.Get<bool>(Preference.DisplayVersions);

        private int DecimalPlaces => Prefs.Get<int>(Preference.DecimalPlaces);

        internal bool ShowETADate => Prefs.Get<bool>(Preference.DisplayEtaAsDate);

        #endregion

        #region Root Data Types

        private WorkUnitModel _workUnitModel;
        /// <summary>
        /// Class member containing info specific to the current work unit
        /// </summary>
        public WorkUnitModel WorkUnitModel
        {
            get => _workUnitModel;
            set
            {
                if (_workUnitModel != null)
                {
                    UpdateTimeOfLastProgress(value);
                }
                _workUnitModel = value;
                _workUnit = null;
            }
        }

        private WorkUnit _workUnit;

        public WorkUnit WorkUnit
        {
            get => _workUnit ?? WorkUnitModel.Data;
            set => _workUnit = value;
        }

        public ClientSettings Settings => _client.Settings;

        // HFM.Client data type
        public SlotOptions SlotOptions { get; set; }

        #endregion

        private readonly IClient _client;

        public SlotModel(IClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));

            _workUnitModel = new WorkUnitModel();

            Initialize();
            TimeOfLastUnitStart = DateTime.MinValue;
            TimeOfLastFrameProgress = DateTime.MinValue;
        }

        private const int DefaultMachineID = 0;
        
        public void Initialize()
        {
            Arguments = String.Empty;
            MachineID = DefaultMachineID;
            // Status = 
            ClientVersion = String.Empty;
            TotalRunCompletedUnits = 0;
            TotalCompletedUnits = 0;
            TotalRunFailedUnits = 0;
        }

        #region Display Meta Data

        /// <summary>
        /// Client Startup Arguments
        /// </summary>
        public string Arguments { get; set; }

        // TODO: Do something similar for v7
        ///// <summary>
        ///// Client Path and Arguments (If Arguments Exist)
        ///// </summary>
        //public string ClientPathAndArguments
        //{
        //   get { return Arguments.Length == 0 ? Settings.Path : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Settings.Path, Arguments); }
        //}

        private int _machineId;
        /// <summary>
        /// Machine ID associated with this client
        /// </summary>
        public int MachineID
        {
            // if SlotId is populated by a v7 client then also use it for the MachineId value
            get => SlotID > -1 ? SlotID : _machineId;
            set => _machineId = value;
        }

        #endregion

        #region Grid Properties

        /// <summary>
        /// Status of this client
        /// </summary>
        public SlotStatus Status { get; set; }

        public float Progress => ((float)PercentComplete) / 100;

        /// <summary>
        /// Current progress (percentage) of the unit
        /// </summary>
        public int PercentComplete => Status.IsRunning() || Status == SlotStatus.Paused ? WorkUnitModel.PercentComplete : 0;

        public SlotIdentifier SlotIdentifier => new SlotIdentifier(Settings.ToClientIdentifier(), SlotID);

        public string Name => SlotIdentifier.Name;

        public int SlotID { get; set; } = SlotIdentifier.NoSlotID;

        public string SlotType
        {
            get
            {
                if (WorkUnit.SlotType == HFM.Core.Client.SlotType.Unknown)
                {
                    return String.Empty;
                }
                string slotType = WorkUnit.SlotType.ToString();
                if (ShowVersions && !String.IsNullOrEmpty(ClientVersion))
                {
                    return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", slotType, ClientVersion);
                }
                return slotType;
            }
        }

        /// <summary>
        /// Client Version
        /// </summary>
        public string ClientVersion { get; set; }

        public bool IsUsingBenchmarkFrameTime => Status.IsRunning() && WorkUnitModel.IsUsingBenchmarkFrameTime(PPDCalculation);

        /// <summary>
        /// Time per frame (TPF) of the unit
        /// </summary>
        public TimeSpan TPF => Status.IsRunning() ? WorkUnitModel.GetFrameTime(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Points per day (PPD) rating for this instance
        /// </summary>
        public double PPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetPPD(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : 0;

        /// <summary>
        /// Units per day (UPD) rating for this instance
        /// </summary>
        public double UPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetUPD(PPDCalculation), 3) : 0;

        /// <summary>
        /// Estimated time of arrival (ETA) for this protein
        /// </summary>
        public TimeSpan ETA => Status.IsRunning() ? WorkUnitModel.GetEta(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Estimated time of arrival (ETA) for this protein
        /// </summary>
        public DateTime ETADate => Status.IsRunning() ? WorkUnitModel.GetEtaDate(PPDCalculation) : DateTime.MinValue;

        public string Core
        {
            get
            {
                if (ShowVersions && Math.Abs(WorkUnit.CoreVersion) > Single.Epsilon)
                {
                    return String.Format(CultureInfo.InvariantCulture, "{0} ({1:0.##})", WorkUnitModel.CurrentProtein.Core, WorkUnit.CoreVersion);
                }
                return WorkUnitModel.CurrentProtein.Core;
            }
        }

        public string CoreID => WorkUnit.CoreID;

        public string ProjectRunCloneGen => WorkUnit.ToShortProjectString();

        public double Credit => Status.IsRunning() ? Math.Round(WorkUnitModel.GetCredit(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : WorkUnitModel.CurrentProtein.Credit;

        public int Completed =>
           Prefs.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
              ? TotalCompletedUnits
              : TotalRunCompletedUnits;

        public int Failed =>
           Prefs.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
              ? TotalFailedUnits
              : TotalRunFailedUnits;

        /// <summary>
        /// Gets or sets the number of completed units since the last client start.
        /// </summary>
        public int TotalRunCompletedUnits { get; set; }

        /// <summary>
        /// Gets or sets the total number of completed units.
        /// </summary>
        public int TotalCompletedUnits { get; set; }

        /// <summary>
        /// Gets or sets the number of failed units since the last client start.
        /// </summary>
        public int TotalRunFailedUnits { get; set; }

        /// <summary>
        /// Gets or sets the total number of failed units.
        /// </summary>
        public int TotalFailedUnits { get; set; }

        /// <summary>
        /// Combined Folding ID and Team String
        /// </summary>
        public string Username => 
            String.IsNullOrWhiteSpace(WorkUnit.FoldingID)
                ? String.Empty
                : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnit.FoldingID, WorkUnit.Team);

        public DateTime DownloadTime => WorkUnitModel.DownloadTime;

        public DateTime PreferredDeadline => WorkUnitModel.PreferredDeadline;

        #endregion

        #region Complex Interfaces

        /// <summary>
        /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
        /// </summary>
        public IList<LogLine> CurrentLogLines { get; set; }

        /// <summary>
        /// Return LogLine List for Specified Queue Index
        /// </summary>
        /// <param name="queueIndex">Index in Queue</param>
        /// <exception cref="ArgumentOutOfRangeException">If queueIndex is outside the bounds of the Log Lines Array</exception>
        public IList<LogLine> GetLogLinesForQueueIndex(int queueIndex)
        {
            if (UnitLogLines == null) return null;

            // Check the UnitLogLines array against the requested Queue Index - Issue 171
            if (queueIndex < 0 || queueIndex > UnitLogLines.Length - 1)
            {
                throw new ArgumentOutOfRangeException("queueIndex", String.Format(CultureInfo.CurrentCulture,
                   "Index is out of range.  Requested Index: {0}.  Array Length: {1}", queueIndex, UnitLogLines.Length));
            }

            if (UnitLogLines[queueIndex] != null)
            {
                return UnitLogLines[queueIndex];
            }

            return null;
        }

        public IList<LogLine>[] UnitLogLines { get; set; }

        public SlotWorkUnitDictionary WorkUnitInfos { get; set; }

        #endregion

        #region Grid Data Warnings

        /// <summary>
        /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
        /// </summary>
        public bool ProjectIsDuplicate { get; set; }

        public bool UsernameOk
        {
            get
            {
                // if these are the default assigned values, don't check the prefs and just return true
                if ((String.IsNullOrWhiteSpace(WorkUnit.FoldingID) || WorkUnit.FoldingID == Unknown.Value) && WorkUnit.Team == default)
                {
                    return true;
                }
                // if the slot is unknown or offline, don't check the prefs and just return true
                if (Status == SlotStatus.Unknown || Status == SlotStatus.Offline)
                {
                    return true;
                }
                return WorkUnit.FoldingID == Prefs.Get<string>(Preference.StanfordId) &&
                       WorkUnit.Team == Prefs.Get<int>(Preference.TeamId);
            }
        }

        #endregion

        #region Slot Time Meta Data

        /// <summary>
        /// Local Time when this Client last detected Frame Progress
        /// </summary>
        public DateTime TimeOfLastUnitStart { get; set; } // should be init to DateTime.MinValue

        /// <summary>
        /// Local Time when this Client last detected Frame Progress
        /// </summary>
        public DateTime TimeOfLastFrameProgress { get; set; } // should be init to DateTime.MinValue

        /// <summary>
        /// Update Time of Last Frame Progress based on Current and Parsed WorkUnit
        /// </summary>
        private void UpdateTimeOfLastProgress(WorkUnitModel parsedWorkUnit)
        {
            // Matches the Current Project and Raw Download Time
            if (WorkUnitModel.Data.EqualsProjectAndDownloadTime(parsedWorkUnit.Data))
            {
                // If the Unit Start Time Stamp is no longer the same as the UnitInfoLogic
                if (parsedWorkUnit.Data.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                    WorkUnitModel.Data.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                    parsedWorkUnit.Data.UnitStartTimeStamp.Equals(WorkUnitModel.Data.UnitStartTimeStamp) == false)
                {
                    TimeOfLastUnitStart = DateTime.Now;
                }

                // If the Frames Complete is greater than the UnitInfoLogic Frames Complete
                if (parsedWorkUnit.FramesComplete > WorkUnitModel.FramesComplete)
                {
                    // Update the Time Of Last Frame Progress
                    TimeOfLastFrameProgress = DateTime.Now;
                }
            }
            else // Different WorkUnit - Update the Time Of Last 
                 // Unit Start and Clear Frame Progress Value
            {
                TimeOfLastUnitStart = DateTime.Now;
                TimeOfLastFrameProgress = DateTime.MinValue;
            }
        }

        #endregion

        /// <summary>
        /// Find slots with duplicate projects.
        /// </summary>
        public static void FindDuplicateProjects(ICollection<SlotModel> slots)
        {
            var duplicates = slots.GroupBy(x => x.WorkUnitModel.Data.ToShortProjectString())
                .Where(g => g.Count() > 1 && g.First().WorkUnitModel.Data.ProjectIsKnown())
                .Select(g => g.Key)
                .ToList();

            foreach (var slot in slots)
            {
                slot.ProjectIsDuplicate = duplicates.Contains(slot.WorkUnitModel.Data.ToShortProjectString());
            }
        }
    }
}
