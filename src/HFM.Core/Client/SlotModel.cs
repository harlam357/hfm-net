/*
 * HFM.NET
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
using System.Globalization;
using System.Linq;

using HFM.Client.DataTypes;
using HFM.Core.DataTypes;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public class SlotModel
    {
        #region IPreferenceSet

        public IPreferenceSet Prefs { get; set; }

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

        public ClientSettings Settings { get; set; }

        // HFM.Client data type
        public SlotOptions SlotOptions { get; set; }

        #endregion

        #region Constructor

        public SlotModel()
        {
            _workUnitModel = new WorkUnitModel();

            Initialize();
            TimeOfLastUnitStart = DateTime.MinValue;
            TimeOfLastFrameProgress = DateTime.MinValue;
        }

        public void Initialize()
        {
            Arguments = String.Empty;
            MachineId = Constants.DefaultMachineID;
            // Status = 
            ClientVersion = String.Empty;
            TotalRunCompletedUnits = 0;
            TotalCompletedUnits = 0;
            TotalRunFailedUnits = 0;
        }

        #endregion

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
        public int MachineId
        {
            // if SlotId is populated by a v7 client then also use it for the MachineId value
            get => SlotId > -1 ? SlotId : _machineId;
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
        public int PercentComplete => ProductionValuesOk || Status == SlotStatus.Paused ? WorkUnitModel.PercentComplete : 0;

        public string Name => Settings.Name.AppendSlotId(SlotId);

        private int _slotId = -1;

        public int SlotId
        {
            get => _slotId;
            set => _slotId = value;
        }

        public string SlotType
        {
            get
            {
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

        public bool IsUsingBenchmarkFrameTime => ProductionValuesOk && WorkUnitModel.IsUsingBenchmarkFrameTime(PPDCalculation);

        /// <summary>
        /// Time per frame (TPF) of the unit
        /// </summary>
        public TimeSpan TPF => ProductionValuesOk ? WorkUnitModel.GetFrameTime(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Points per day (PPD) rating for this instance
        /// </summary>
        public double PPD => ProductionValuesOk ? Math.Round(WorkUnitModel.GetPPD(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : 0;

        /// <summary>
        /// Units per day (UPD) rating for this instance
        /// </summary>
        public double UPD => ProductionValuesOk ? Math.Round(WorkUnitModel.GetUPD(PPDCalculation), 3) : 0;

        /// <summary>
        /// Estimated time of arrival (ETA) for this protein
        /// </summary>
        public TimeSpan ETA => ProductionValuesOk ? WorkUnitModel.GetEta(PPDCalculation) : TimeSpan.Zero;

        /// <summary>
        /// Esimated time of arrival (ETA) for this protein
        /// </summary>
        public DateTime ETADate => ProductionValuesOk ? WorkUnitModel.GetEtaDate(PPDCalculation) : DateTime.MinValue;

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

        public string CoreId => WorkUnit.CoreID;

        public string ProjectRunCloneGen => WorkUnit.ToShortProjectString();

        public double Credit => ProductionValuesOk ? Math.Round(WorkUnitModel.GetCredit(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : WorkUnitModel.CurrentProtein.Credit;

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
        public string Username => String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnit.FoldingID, WorkUnit.Team);

        public DateTime DownloadTime => WorkUnitModel.DownloadTime;

        public DateTime PreferredDeadline => WorkUnitModel.PreferredDeadline;

        /// <summary>
        /// Flag denoting if Progress, Production, and Time based values are OK to Display
        /// </summary>
        public bool ProductionValuesOk =>
            Status == SlotStatus.Running ||
            Status == SlotStatus.RunningNoFrameTimes ||
            Status == SlotStatus.Finishing;

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
                if (WorkUnit.FoldingID == WorkUnit.DefaultFoldingID && WorkUnit.Team == WorkUnit.DefaultTeam)
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
