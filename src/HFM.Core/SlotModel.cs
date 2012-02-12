/*
 * HFM.NET - Slot Model Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using HFM.Client.DataTypes;
using HFM.Core.DataTypes;

namespace HFM.Core
{
   public class SlotModel
   {
      #region IPreferenceSet

      public IPreferenceSet Prefs { get; set; }

      private PpdCalculationType CalculationType
      {
         get { return Prefs.Get<PpdCalculationType>(Preference.PpdCalculation); }
      }

      private BonusCalculationType CalculateBonus
      {
         get { return Prefs.Get<BonusCalculationType>(Preference.CalculateBonus); }
      }

      public bool ShowVersions
      {
         get { return Prefs.Get<bool>(Preference.ShowVersions); }
      }

      private int DecimalPlaces
      {
         get { return Prefs.Get<int>(Preference.DecimalPlaces); }
      }

      private CompletedCountDisplayType CompletedCountDisplay
      {
         get { return Prefs.Get<CompletedCountDisplayType>(Preference.CompletedCountDisplay); }
      }

      public bool ShowETADate
      {
         get { return Prefs.Get<bool>(Preference.EtaDate); }
      }

      #endregion

      #region Root Data Types

      private UnitInfoLogic _unitInfoLogic;
      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public UnitInfoLogic UnitInfoLogic
      {
         get { return _unitInfoLogic; }
         set
         {
            if (_unitInfoLogic != null)
            {
               UpdateTimeOfLastProgress(value);
            }
            _unitInfoLogic = value;
            _unitInfo = null;
         }
      }

      // ReSharper disable UnaccessedField.Local
      private UnitInfo _unitInfo;
      // ReSharper restore UnaccessedField.Local
      public UnitInfo UnitInfo
      {
         get { return _unitInfo ?? UnitInfoLogic.UnitInfoData; }
         set { _unitInfo = value; }
      }

      public bool Owns(IOwnedByClient value)
      {
         //if (Settings.IsFahClient())
         //{
         //   if (value.OwningSlotName.Equals(Name) &&
         //       value.OwningClientPath.Equals(Settings.DataPath()))
         //   {
         //      return true;
         //   }
         //}
         //else if (Settings.IsLegacy())
         //{
            if (value.OwningSlotName.Equals(Name) &&
                Paths.Equal(value.OwningClientPath, Settings.DataPath()))
            {
               return true;
            }   
         //}

         return false;
      }

      public ClientSettings Settings { get; set; }

      // temporary and only used with FahClient type
      public SlotOptions SlotOptions { get; set; }

      #endregion

      #region Constructor

      public SlotModel()
      {
         _unitInfoLogic = new UnitInfoLogic();

         Initialize();
         TimeOfLastUnitStart = DateTime.MinValue;
         TimeOfLastFrameProgress = DateTime.MinValue;
      }

      public void Initialize()
      {
         Arguments = String.Empty;
         UserId = Constants.DefaultUserID;
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

      /// <summary>
      /// Client Path and Arguments (If Arguments Exist)
      /// </summary>
      public string ClientPathAndArguments
      {
         get { return Arguments.Length == 0 ? Settings.Path : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Settings.Path, Arguments); }
      }

      /// <summary>
      /// User ID associated with this client
      /// </summary>
      public string UserId { get; set; }

      /// <summary>
      /// True if User ID is Unknown
      /// </summary>
      public bool UserIdUnknown
      {
         get { return UserId.Length == 0; }
      }

      private int _machineId;
      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      public int MachineId
      {
         // if SlotId is populated by a v7 client then also use it for the MachineId value
         get { return SlotId > -1 ? SlotId : _machineId; }
         set { _machineId = value; }
      }

      /// <summary>
      /// Combined User ID and Machine ID String
      /// </summary>
      public string UserAndMachineId
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", UserId, MachineId); }
      }

      #endregion

      #region Grid Properties

      /// <summary>
      /// Status of this client
      /// </summary>
      public SlotStatus Status { get; set; }

      public float Progress
      {
         get { return ((float)PercentComplete) / 100; }
      }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public int PercentComplete
      {
         get { return ProductionValuesOk || Status.Equals(SlotStatus.Paused) ? UnitInfoLogic.PercentComplete : 0; }
      }

      public string Name
      {
         get { return Settings.IsFahClient() ? Settings.Name.AppendSlotId(SlotId) : Settings.Name; }
      }
      
      private int _slotId = -1;

      public int SlotId
      {
         get { return _slotId; }
         set { _slotId = value; }
      }

      public string SlotType
      {
         get
         {
            if (ShowVersions && !String.IsNullOrEmpty(ClientVersion))
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", UnitInfo.SlotType, ClientVersion);
            }
            return UnitInfo.SlotType.ToString();
         }
      }

      /// <summary>
      /// Client Version
      /// </summary>
      public string ClientVersion { get; set; }

      public bool IsUsingBenchmarkFrameTime
      {
         get { return ProductionValuesOk && UnitInfoLogic.IsUsingBenchmarkFrameTime(CalculationType); }
      }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TPF
      {
         get { return ProductionValuesOk ? UnitInfoLogic.GetFrameTime(CalculationType) : TimeSpan.Zero; }
      }
      
      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public double PPD
      {
         get { return ProductionValuesOk ? Math.Round(UnitInfoLogic.GetPPD(Status, CalculationType, CalculateBonus), DecimalPlaces) : 0; }
      }
      
      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public double UPD
      {
         get { return ProductionValuesOk ? Math.Round(UnitInfoLogic.GetUPD(CalculationType), 3) : 0; }
      }

      public int MHz
      {
         get { return Settings.ClientProcessorMegahertz; }
      }

      public double PPDMHz
      {
         get { return Math.Round(PPD / MHz, 3); }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public TimeSpan ETA
      {
         get { return ProductionValuesOk ? UnitInfoLogic.GetEta(CalculationType) : TimeSpan.Zero; }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public DateTime ETADate
      {
         get { return ProductionValuesOk ? UnitInfoLogic.GetEtaDate(CalculationType) : DateTime.MinValue; }
      }

      public string Core
      {
         get
         {
            if (ShowVersions && !String.IsNullOrEmpty(UnitInfo.CoreVersion))
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", UnitInfoLogic.CurrentProtein.Core, UnitInfo.CoreVersion);
            }
            return UnitInfoLogic.CurrentProtein.Core;
         }
      }

      public string CoreId
      {
         get { return UnitInfo.CoreID; }
      }

      public string ProjectRunCloneGen
      {
         get { return UnitInfo.ProjectRunCloneGen(); }
      }

      public double Credit
      {
         get { return ProductionValuesOk ? Math.Round(UnitInfoLogic.GetCredit(Status, CalculationType, CalculateBonus), DecimalPlaces) : UnitInfoLogic.CurrentProtein.Credit; }
      }

      public int Completed
      {
         get { return CompletedCountDisplay.Equals(CompletedCountDisplayType.ClientTotal) ? TotalCompletedUnits : TotalRunCompletedUnits; }
      }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      public int TotalRunCompletedUnits { get; set; }

      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      public int TotalCompletedUnits { get; set; }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      public int TotalRunFailedUnits { get; set; }

      /// <summary>
      /// Combined Folding ID and Team String
      /// </summary>
      public string Username
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", UnitInfo.FoldingID, UnitInfo.Team); }
      }

      public DateTime DownloadTime
      {
         get { return UnitInfoLogic.DownloadTime; }
      }

      public DateTime PreferredDeadline
      {
         get { return UnitInfoLogic.PreferredDeadline; }
      }

      /// <summary>
      /// Flag denoting if Progress, Production, and Time based values are OK to Display
      /// </summary>
      public bool ProductionValuesOk
      {
         get
         {
            return Status.Equals(SlotStatus.Running) ||
                   Status.Equals(SlotStatus.RunningAsync) ||
                   Status.Equals(SlotStatus.RunningNoFrameTimes) ||
                   Status.Equals(SlotStatus.Finishing);
         }
      }

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
            throw new ArgumentOutOfRangeException("QueueIndex", String.Format(CultureInfo.CurrentCulture,
               "Index is out of range.  Requested Index: {0}.  Array Length: {1}", queueIndex, UnitLogLines.Length));
         }

         if (UnitLogLines[queueIndex] != null)
         {
            return UnitLogLines[queueIndex];
         }

         return null;
      }

      public IList<LogLine>[] UnitLogLines { get; set; }

      /// <summary>
      /// Client Queue
      /// </summary>
      public ClientQueue Queue { get; set; }
      
      #endregion

      #region Grid Data Warnings

      /// <summary>
      /// User ID is a Duplicate of another Client's User ID
      /// </summary>
      public bool UserIdIsDuplicate { get; set; }

      /// <summary>
      /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
      /// </summary>
      public bool ProjectIsDuplicate { get; set; }

      public bool UsernameOk
      {
         get
         {
            // if these are the default assigned values, don't check otherwise and just return true
            if (UnitInfo.FoldingID == Constants.DefaultFoldingID && UnitInfo.Team == Constants.DefaultTeam)
            {
               return true;
            }

            if ((UnitInfo.FoldingID != Prefs.Get<string>(Preference.StanfordId) ||
                      UnitInfo.Team != Prefs.Get<int>(Preference.TeamId)) &&
                (Status.Equals(SlotStatus.Unknown) == false && Status.Equals(SlotStatus.Offline) == false))
            {
               return false;
            }

            return true;
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
      /// Update Time of Last Frame Progress based on Current and Parsed UnitInfo
      /// </summary>
      private void UpdateTimeOfLastProgress(UnitInfoLogic parsedUnitInfo)
      {
         // Matches the Current Project and Raw Download Time
         if (UnitInfoLogic.UnitInfoData.Equals(parsedUnitInfo.UnitInfoData))
         {
            // If the Unit Start Time Stamp is no longer the same as the UnitInfoLogic
            if (parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                UnitInfoLogic.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(UnitInfoLogic.UnitInfoData.UnitStartTimeStamp) == false)
            {
               TimeOfLastUnitStart = DateTime.Now;
            }

            // If the Frames Complete is greater than the UnitInfoLogic Frames Complete
            if (parsedUnitInfo.FramesComplete > UnitInfoLogic.FramesComplete)
            {
               // Update the Time Of Last Frame Progress
               TimeOfLastFrameProgress = DateTime.Now;
            }
         }
         else // Different UnitInfo - Update the Time Of Last 
         // Unit Start and Clear Frame Progress Value
         {
            TimeOfLastUnitStart = DateTime.Now;
            TimeOfLastFrameProgress = DateTime.MinValue;
         }
      }

      #endregion
   }
}
