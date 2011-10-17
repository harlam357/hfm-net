/*
 * HFM.NET - Display Instance Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

using ProtoBuf;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   [ProtoContract]
   public class DisplayInstance : IDisplayInstance
   {
      /// <summary>
      /// Name (key) of ExternalInstance that owns this DisplayInstance (otherwise null)
      /// </summary>
      public string ExternalInstanceName { get; set; }
      
      public IPreferenceSet Prefs { get; set; }
      public IProteinBenchmarkContainer BenchmarkContainer { get; set; }

      private IUnitInfoLogic _currentUnitInfo;

      private PpdCalculationType CalculationType
      {
         get { return Prefs.Get<PpdCalculationType>(Preference.PpdCalculation); }
      }

      private bool CalculateBonus
      {
         get { return Prefs.Get<bool>(Preference.CalculateBonus); }
      }

      /// <summary>
      /// Class member containing info specific to the current work unit
      /// </summary>
      public IUnitInfoLogic CurrentUnitInfo
      {
         get { return _currentUnitInfo; }
         set
         {
            UpdateTimeOfLastProgress(value);
            _currentUnitInfo = value;
         }
      }
      
      private UnitInfo _unitInfo;
      [ProtoMember(1)]
      public UnitInfo UnitInfo
      {
         get
         {
            if (CurrentUnitInfo != null)
            {
               return CurrentUnitInfo.UnitInfoData;
            }
            return null;
         }
         set { _unitInfo = value; }
      }
      
      [ProtoMember(2)]
      public ClientInstanceSettings Settings { get; set; }
      
      public void BuildUnitInfoLogic(IProtein protein)
      {
         if (protein == null) throw new ArgumentNullException("protein");

         if (_unitInfo == null) throw new InvalidOperationException();
         CurrentUnitInfo = new UnitInfoLogic(protein, BenchmarkContainer, _unitInfo, Settings);
         _unitInfo = null;
      }

      /// <summary>
      /// Client Startup Arguments
      /// </summary>
      [ProtoMember(3)]
      public string Arguments { get; set; }

      /// <summary>
      /// User ID associated with this client
      /// </summary>
      [ProtoMember(4)]
      public string UserId { get; set; }

      /// <summary>
      /// True if User ID is Unknown
      /// </summary>
      public bool UserIdUnknown
      {
         get { return UserId.Length == 0; }
      }

      /// <summary>
      /// Machine ID associated with this client
      /// </summary>
      [ProtoMember(5)]
      public int MachineId { get; set; }

      /// <summary>
      /// Combined User ID and Machine ID String
      /// </summary>
      public string UserAndMachineId
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", UserId, MachineId); }
      }

      /// <summary>
      /// The Folding ID (Username) attached to this client
      /// </summary>
      public string FoldingID
      {
         get { return CurrentUnitInfo.UnitInfoData.FoldingID; }
      }

      /// <summary>
      /// The Team number attached to this client
      /// </summary>
      public int Team
      {
         get { return CurrentUnitInfo.UnitInfoData.Team; }
      }

      /// <summary>
      /// Combined Folding ID and Team String
      /// </summary>
      public string FoldingIDAndTeam
      {
         get { return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", FoldingID, Team); }
      }

      #region IDisplayInstance Members

      #region Grid Properties

      /// <summary>
      /// Status of this client
      /// </summary>
      [ProtoMember(6)]
      public ClientStatus Status { get; set; }

      public float Progress
      {
         get { return ((float)PercentComplete) / 100; }
      }

      public string Name
      {
         get { return Settings.InstanceName; }
      }

      public string ClientType
      {
         get
         {
            if (Prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(ClientVersion) == false)
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", TypeOfClient, ClientVersion);
            }
            return TypeOfClient.ToString();
         }
      }
      
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TPF
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.GetFrameTime(CalculationType);
            }

            return TimeSpan.Zero;
         }
      }
      
      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public double PPD
      {
         get
         {
            if (ProductionValuesOk)
            {
               return Math.Round(CurrentUnitInfo.GetPPD(Status, CalculationType, CalculateBonus), Prefs.GetPreference<int>(Preference.DecimalPlaces));
            }

            return 0;
         }
      }
      
      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public double UPD
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.GetUPD(CalculationType);
            }

            return 0;
         }
      }

      public int MHz
      {
         get { return Settings.ClientProcessorMegahertz; }
      }

      public double PpdMHz
      {
         get { return Math.Round(PPD / MHz, 3); }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public TimeSpan ETA
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.GetEta(CalculationType);
            }

            return TimeSpan.Zero;
         }
      }
      
      public string Core
      {
         get
         {
            if (Prefs.GetPreference<bool>(Preference.ShowVersions) && String.IsNullOrEmpty(CoreVersion) == false)
            {
               return String.Format(CultureInfo.CurrentCulture, "{0} ({1})", CurrentUnitInfo.CurrentProtein.Core, CoreVersion);
            }
            return CurrentUnitInfo.CurrentProtein.Core;
         }
      }

      public string CoreID
      {
         get { return CurrentUnitInfo.UnitInfoData.CoreID; }
      }

      public string ProjectRunCloneGen
      {
         get { return CurrentUnitInfo.UnitInfoData.ProjectRunCloneGen(); }
      }

      public double Credit
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.GetCredit(Status, CalculationType, CalculateBonus);
            }

            return CurrentUnitInfo.CurrentProtein.Credit;
         }
      }

      public int Complete
      {
         get
         {
            if (Prefs.GetPreference<CompletedCountDisplayType>(Preference.CompletedCountDisplay).Equals(CompletedCountDisplayType.ClientTotal))
            {
               return TotalClientCompletedUnits;
            }
            return TotalRunCompletedUnits;
         }
      }

      /// <summary>
      /// Number of failed units since the last client start
      /// </summary>
      [ProtoMember(7)]
      public int TotalRunFailedUnits { get; set; }

      public string Username
      {
         get { return FoldingIDAndTeam; }
      }

      public DateTime DownloadTime
      {
         get { return CurrentUnitInfo.DownloadTime; }
      }

      public DateTime PreferredDeadline
      {
         get { return CurrentUnitInfo.PreferredDeadline; }
      }

      #endregion

      #region Complex Interfaces

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public IProtein CurrentProtein
      {
         get { return CurrentUnitInfo.CurrentProtein; }
      }

      /// <summary>
      /// Current Log Lines based on UnitLogLines Array and CurrentUnitIndex
      /// </summary>
      [ProtoMember(8)]
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

      private DateTime _lastRetrievalTime = DateTime.MinValue;
      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      [ProtoMember(9)]
      public DateTime LastRetrievalTime
      {
         get { return _lastRetrievalTime; }
         set { _lastRetrievalTime = value; }
      }

      /// <summary>
      /// Flag denoting if Progress, Production, and Time based values are OK to Display
      /// </summary>
      public bool ProductionValuesOk
      {
         get
         {
            if (Status.Equals(ClientStatus.Running) ||
                Status.Equals(ClientStatus.RunningAsync) ||
                Status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return true;
            }

            return false;
         }
      }

      /// <summary>
      /// Client host type (Path, FTP, or HTTP)
      /// </summary>
      public InstanceType InstanceHostType
      {
         get { return Settings.InstanceHostType; }
      }

      /// <summary>
      /// Location of log files for this instance
      /// </summary>
      public string Path
      {
         get { return Settings.Path; }
      }

      /// <summary>
      /// Client Path and Arguments (If Arguments Exist)
      /// </summary>
      public string ClientPathAndArguments
      {
         get
         {
            if (Arguments.Length == 0)
            {
               return Path;
            }

            return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", Path, Arguments);
         }
      }

      /// <summary>
      /// Specifies that this client is on a VM that reports local time as UTC
      /// </summary>
      public bool ClientIsOnVirtualMachine
      {
         get { return Settings.ClientIsOnVirtualMachine; }
      }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public int FramesComplete
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.FramesComplete;
            }

            return 0;
         }
      }
      
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public int PercentComplete
      {
         get
         {
            if (ProductionValuesOk ||
                Status.Equals(ClientStatus.Paused))
            {
               return CurrentUnitInfo.PercentComplete;
            }

            return 0;
         }
      }

      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public ClientType TypeOfClient
      {
         get { return CurrentUnitInfo.UnitInfoData.TypeOfClient; }
      }

      /// <summary>
      /// Client Version
      /// </summary>
      [ProtoMember(10)]
      public string ClientVersion { get; set; }

      public string CoreName
      {
         get { return CurrentUnitInfo.CurrentProtein.Core; }
      }

      public string CoreVersion
      {
         get { return CurrentUnitInfo.UnitInfoData.CoreVersion; }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public int ProjectID
      {
         get { return CurrentUnitInfo.UnitInfoData.ProjectID; }
      }

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
            if (FoldingID == Default.FoldingID && Team == Default.Team)
            {
               return true;
            }

            if ((FoldingID != Prefs.GetPreference<string>(Preference.StanfordId) ||
                      Team != Prefs.GetPreference<int>(Preference.TeamId)) &&
                (Status.Equals(ClientStatus.Unknown) == false && Status.Equals(ClientStatus.Offline) == false))
            {
               return false;
            }

            return true;
         }
      }

      /// <summary>
      /// Number of completed units since the last client start
      /// </summary>
      [ProtoMember(11)]
      public int TotalRunCompletedUnits { get; set; }

      /// <summary>
      /// Total Units Completed for lifetime of the client (read from log file)
      /// </summary>
      [ProtoMember(12)]
      public int TotalClientCompletedUnits { get; set; }

      /// <summary>
      /// Cached FAHlog Filename for this instance
      /// </summary>
      public string CachedFahLogName
      {
         get { return Settings.CachedFahLogName; }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public DateTime EtaDate
      {
         get
         {
            if (ProductionValuesOk)
            {
               return CurrentUnitInfo.GetEtaDate(CalculationType);
            }

            return DateTime.MinValue;
         }
      }

      #endregion

      public bool Owns(IOwnedByClientInstance value)
      {
         // External Instances don't own anything local
         if (ExternalInstanceName == null &&
             value.OwningInstanceName.Equals(Settings.InstanceName) &&
             Paths.Equal(value.OwningInstancePath, Settings.Path))
         {
            return true;
         }

         return false;
      }

      #region Client Level Members

      public void InitClientLevelMembers()
      {
         Arguments = String.Empty;
         UserId = Constants.DefaultUserID;
         MachineId = Constants.DefaultMachineID;
         //FoldingID = Constants.FoldingIDDefault;
         //Team = Constants.TeamDefault;
         TotalRunCompletedUnits = 0;
         TotalRunFailedUnits = 0;
         TotalClientCompletedUnits = 0;
      }

      #endregion

      #region Unit Progress Client Level Members

      private DateTime _timeOfLastUnitStart = DateTime.MinValue;
      /// <summary>
      /// Local Time when this Client last detected Frame Progress
      /// </summary>
      internal DateTime TimeOfLastUnitStart
      {
         get { return _timeOfLastUnitStart; }
         set { _timeOfLastUnitStart = value; }
      }

      private DateTime _timeOfLastFrameProgress = DateTime.MinValue;
      /// <summary>
      /// Local Time when this Client last detected Frame Progress
      /// </summary>
      internal DateTime TimeOfLastFrameProgress
      {
         get { return _timeOfLastFrameProgress; }
         set { _timeOfLastFrameProgress = value; }
      }

      #endregion

      /// <summary>
      /// Update Time of Last Frame Progress based on Current and Parsed UnitInfo
      /// </summary>
      private void UpdateTimeOfLastProgress(IUnitInfoLogic parsedUnitInfo)
      {
         // Matches the Current Project and Raw Download Time
         if (CurrentUnitInfo.EqualsUnitInfoLogic(parsedUnitInfo))
         {
            // If the Unit Start Time Stamp is no longer the same as the CurrentUnitInfo
            if (parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(TimeSpan.MinValue) == false &&
                parsedUnitInfo.UnitInfoData.UnitStartTimeStamp.Equals(CurrentUnitInfo.UnitInfoData.UnitStartTimeStamp) == false)
            {
               TimeOfLastUnitStart = DateTime.Now;
            }

            // If the Frames Complete is greater than the CurrentUnitInfo Frames Complete
            if (parsedUnitInfo.FramesComplete > CurrentUnitInfo.FramesComplete)
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

      /// <summary>
      /// Restore the given UnitInfo into this Client Instance
      /// </summary>
      /// <param name="unitInfo">UnitInfo Object to Restore</param>
      /// <param name="protein">Protein Object that corresponds to the UnitInfo Object</param>
      public void RestoreUnitInfo(UnitInfo unitInfo, IProtein protein)
      {
         UnitInfo = unitInfo;
         BuildUnitInfoLogic(protein);
      }
   }
}
