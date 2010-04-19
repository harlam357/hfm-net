/*
 * HFM.NET - Unit Info Logic Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class UnitInfoLogic : IUnitInfoLogic
   {
      #region Members
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs;

      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinCollection _proteinCollection;

      private readonly UnitInfo _unitInfo;
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      public IUnitInfo UnitInfoData
      {
         get { return _unitInfo; }
      }

      /// <summary>
      /// Owning Client Instance Interface
      /// </summary>
      private readonly IClientInstance _clientInstance;

      /// <summary>
      /// Use UTC Time Data as Local Time
      /// </summary>
      private bool UtcOffsetIsZero
      {
         get { return _clientInstance.ClientIsOnVirtualMachine; }
      }
      
      /// <summary>
      /// Client Time Adjustment (apply to DateTime values)
      /// </summary>
      private int ClientTimeOffset
      {
         get { return _clientInstance.ClientTimeOffset; }
      }
      #endregion

      #region Owner Data Properties
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstanceName
      {
         get { return _unitInfo.OwningInstanceName; }
         set { _unitInfo.OwningInstanceName = value; }
      }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstancePath
      {
         get { return _unitInfo.OwningInstancePath; }
         set { _unitInfo.OwningInstancePath = value; }
      }
      #endregion

      #region Retrieval Time Property
      /// <summary>
      /// Local time the logs used to generate this UnitInfo were retrieved
      /// </summary>
      public DateTime UnitRetrievalTime
      {
         get { return _unitInfo.UnitRetrievalTime; }
         set { _unitInfo.UnitRetrievalTime = value; }
      }
      #endregion

      #region Folding ID and Team Properties
      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      public string FoldingID
      {
         get { return _unitInfo.FoldingID; }
         set { _unitInfo.FoldingID = value; }
      }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      public Int32 Team
      {
         get { return _unitInfo.Team; }
         set { _unitInfo.Team = value; }
      }
      #endregion

      #region Constructors
      [CLSCompliant(false)]
      public UnitInfoLogic(IPreferenceSet prefs, IProteinCollection proteinCollection, IUnitInfo unitInfo, IClientInstance clientInstance)
      {
         _Prefs = prefs;
         _proteinCollection = proteinCollection;
         _unitInfo = (UnitInfo)unitInfo;
         _clientInstance = clientInstance;

         OwningInstanceName = _clientInstance.InstanceName;
         OwningInstancePath = _clientInstance.Path;
         UnitRetrievalTime = _clientInstance.LastRetrievalTime;

         SetCurrentProtein();
         TypeOfClient = GetClientTypeFromProtein(CurrentProtein);
      }
      #endregion

      #region Unit Level Members
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public ClientType TypeOfClient
      {
         get { return _unitInfo.TypeOfClient; }
         set { _unitInfo.TypeOfClient = value; }
      }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get 
         {
            if (_unitInfo.DownloadTime.Equals(DateTime.MinValue) == false)
            {
               if (UtcOffsetIsZero == false)
               {
                  return _unitInfo.DownloadTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(ClientTimeOffset));
               }

               return _unitInfo.DownloadTime.Subtract(TimeSpan.FromMinutes(ClientTimeOffset)); 
            }
            
            return _unitInfo.DownloadTime; 
         }
      }

      /// <summary>
      /// Flag specifying if Download Time is Unknown
      /// </summary>
      public bool DownloadTimeUnknown
      {
         get { return _unitInfo.DownloadTimeUnknown; }
      }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime PreferredDeadline
      {
         get
         {
            if (DownloadTimeUnknown == false)
            {
               if (CurrentProtein.IsUnknown == false)
               {
                  return DownloadTime.AddDays(CurrentProtein.PreferredDays);
               }
               
               return DueTime;
            }

            return DateTime.MinValue;
         }
      }

      /// <summary>
      /// Flag specifying if Preferred Deadline is Unknown
      /// </summary>
      public bool PreferredDeadlineUnknown
      {
         get { return PreferredDeadline.Equals(DateTime.MinValue); }
      }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime FinalDeadline
      {
         get
         {
            if (DownloadTimeUnknown == false && CurrentProtein.IsUnknown == false)
            {
               return DownloadTime.AddDays(CurrentProtein.MaxDays);
            }

            return DateTime.MinValue;
         }
      }

      /// <summary>
      /// Flag specifying if Final Deadline is Unknown
      /// </summary>
      public bool FinalDeadlineUnknown
      {
         get { return FinalDeadline.Equals(DateTime.MinValue); }
      }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime
      {
         get 
         {
            if (_unitInfo.DueTime.Equals(DateTime.MinValue) == false)
            {
               if (UtcOffsetIsZero == false)
               {
                  return _unitInfo.DueTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(ClientTimeOffset));
               }

               return _unitInfo.DueTime.Subtract(TimeSpan.FromMinutes(ClientTimeOffset));
            }

            return _unitInfo.DueTime; 
         }
      }

      /// <summary>
      /// Flag specifying if Due Time is Unknown
      /// </summary>
      public bool DueTimeUnknown
      {
         get { return _unitInfo.DueTimeUnknown; }
      }

      /// <summary>
      /// Unit Start Time Stamp (Time Stamp from First Parsable Line in LogLines)
      /// </summary>
      /// <remarks>Used to Determine Status when a LogLine Time Stamp is not available - See ClientInstance.HandleReturnedStatus</remarks>
      public TimeSpan UnitStartTimeStamp
      {
         get { return _unitInfo.UnitStartTimeStamp; }
      }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      public DateTime FinishedTime
      {
         get 
         {
            if (_unitInfo.FinishedTime.Equals(DateTime.MinValue) == false)
            {
               if (UtcOffsetIsZero == false)
               {
                  return _unitInfo.FinishedTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(ClientTimeOffset));
               }

               return _unitInfo.FinishedTime.Subtract(TimeSpan.FromMinutes(ClientTimeOffset));
            }

            return _unitInfo.FinishedTime; 
         }
      }

      /// <summary>
      /// Core Version Number
      /// </summary>
      public string CoreVersion
      {
         get { return _unitInfo.CoreVersion; }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get { return _unitInfo.ProjectID; }
         set
         {
            _unitInfo.ProjectID = value;
            if (_unitInfo.ProjectID == 0)
            {
               CurrentProtein = _proteinCollection.GetNewProtein();
            }
            else
            {
               CurrentProtein = _proteinCollection.GetProtein(ProjectID);
            }
         }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get { return _unitInfo.ProjectRun; }
         set { _unitInfo.ProjectRun = value; }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get { return _unitInfo.ProjectClone; }
         set { _unitInfo.ProjectClone = value; }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get { return _unitInfo.ProjectGen; }
         set { _unitInfo.ProjectGen = value; }
      }

      /// <summary>
      /// Project (R/C/G) is a Duplicate of another Client's Project (R/C/G)
      /// </summary>
      public bool ProjectIsDuplicate { get; set; }

      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      public bool ProjectIsUnknown
      {
         get { return _unitInfo.ProjectIsUnknown; } 
      }

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      public string ProjectRunCloneGen
      {
         get
         {
            return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})", 
               ProjectID, ProjectRun, ProjectClone, ProjectGen);
         }
      }

      /// <summary>
      /// Name of the unit
      /// </summary>
      public String ProteinName
      {
         get { return _unitInfo.ProteinName; }
         set { _unitInfo.ProteinName = value; }
      }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      public string ProteinTag
      {
         get { return _unitInfo.ProteinTag; }
         set { _unitInfo.ProteinTag = value; }
      }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      public WorkUnitResult UnitResult
      {
         get { return _unitInfo.UnitResult; }
         set { _unitInfo.UnitResult = value; }
      }

      private IProtein _CurrentProtein;
      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public IProtein CurrentProtein
      {
         get { return _CurrentProtein; }
         protected set
         {
            if (value == null)
            {
               throw new ArgumentException("The given value cannot be null.");
            }

            // Set the New Protein
            _CurrentProtein = value;
         }
      }
      #endregion

      #region Frames/Percent Completed Unit Level Members
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public Int32 FramesComplete
      {
         get
         {
            // Report Frame Progress even if CurrentProtein.IsUnknown - 11/22/09
            Int32 RawScaleFactor = _unitInfo.RawFramesTotal / CurrentProtein.Frames;
            if (RawScaleFactor > 0)
            {
               int ComputedFramesComplete = _unitInfo.RawFramesComplete / RawScaleFactor;
               
               // Make sure FramesComplete is 0 or greater but
               // not greater than the CurrentProtein.Frames
               if (ComputedFramesComplete >= 0 && 
                   ComputedFramesComplete <= CurrentProtein.Frames)
               {
                  return ComputedFramesComplete;
               }
            }

            return 0;
         }
      }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public Int32 PercentComplete
      {
         get
         {
            // Report Percent Progress even if CurrentProtein.IsUnknown - 11/22/09
            return FramesComplete * 100 / CurrentProtein.Frames;
         }
      }
      #endregion

      #region Production Value Properties
      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      public Int32 RawTimePerSection
      {
         get
         {
            switch (_Prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
            {
               case PpdCalculationType.LastFrame:
                  return RawTimePerLastSection;
               case PpdCalculationType.LastThreeFrames:
                  return RawTimePerThreeSections;
               case PpdCalculationType.AllFrames:
                  return RawTimePerAllSections;
               case PpdCalculationType.EffectiveRate:
                  return RawTimePerUnitDownload;
            }

            return 0;
         }
      }
      
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TimePerFrame
      {
         get
         {
            if (RawTimePerSection != 0)
            {
               return TimeSpan.FromSeconds(RawTimePerSection);
            }

            return InstanceProvider.GetInstance<IProteinBenchmarkContainer>().GetBenchmarkAverageFrameTime(this);
         }
      }

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      public Double UPD
      {
         get
         {
            // The unknown protein frame count is 100.
            // No need to check if we are working with
            // a known or unknown protein here - 1/27/10
            //if (CurrentProtein.IsUnknown == false)
            //{
               return CurrentProtein.GetUPD(TimePerFrame);
            //}

            //return 0;
         }
      }

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      public Double PPD
      {
         get
         {
            return GetPPD(TimePerFrame);
         }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public TimeSpan ETA
      {
         get
         {
            return new TimeSpan((CurrentProtein.Frames - FramesComplete) * TimePerFrame.Ticks);
         }
      }

      /// <summary>
      /// Specifies if All Frames have been Completed
      /// </summary>
      public bool AllFramesAreCompleted
      {
         get
         {
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.Frames == FramesComplete;
            }

            return false;
         }
      }

      /// <summary>
      /// Esimated Finishing Time for this unit
      /// </summary>
      public TimeSpan EftByDownloadTime
      {
         get
         {
            if (DownloadTime.Equals(DateTime.MinValue) == false)
            {
               if (FinishedTime.Equals(DateTime.MinValue) == false)
               {
                  return FinishedTime.Subtract(DownloadTime);
               }

               // Issue 156 - ETA must be a positive TimeSpan
               // Issue 134 - Since fixing Issue 156 it appears that once most 
               // bigadv units finish their last frame they would be assigned a 
               // Zero EFT since their ETA values would have been zero and they 
               // had not yet written the FinishedTime to the queue.dat file.  
               // In light of this I've added the AllFramesAreCompleted property.  
               // Now, if ETA is Zero and AllFramesAreCompleted == false, the EFT 
               // will be Zero.  Otherwise, it will be given a value of the 
               // (UnitRetrievalTime plus ETA) minus the DownloadTime.
               if (ETA.Equals(TimeSpan.Zero) && AllFramesAreCompleted == false)
               {
                  return TimeSpan.Zero;
               }

               return UnitRetrievalTime.Add(ETA).Subtract(DownloadTime);
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// Esimated Finishing Time for this unit
      /// </summary>
      public TimeSpan EftByFrameTime
      {
         get
         {
            if (DownloadTime.Equals(DateTime.MinValue) == false)
            {
               if (FinishedTime.Equals(DateTime.MinValue) == false)
               {
                  return FinishedTime.Subtract(DownloadTime);
               }
            }

            if (CurrentProtein.IsUnknown == false)
            {
               return TimeSpan.FromSeconds(RawTimePerSection * CurrentProtein.Frames);
            }

            return TimeSpan.Zero;
         }
      }
      #endregion

      #region CurrentProtein Pass-Through Properties/Methods
      public string WorkUnitName
      {
         get
         {
            return CurrentProtein.WorkUnitName;
         }
      }

      public double NumAtoms
      {
         get
         {
            return CurrentProtein.NumAtoms;
         }
      }

      public double Credit
      {
         get
         {
            return CurrentProtein.Credit;
         }
      }

      /// <summary>
      /// Get the Credit of the Unit (including bonus)
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public double GetBonusCredit()
      {
         // Issue 183
         if (_clientInstance.Status.Equals(ClientStatus.RunningAsync) ||
             _clientInstance.Status.Equals(ClientStatus.RunningNoFrameTimes))
         {
            return CurrentProtein.GetBonusCredit(EftByFrameTime);
         }
         
         return CurrentProtein.GetBonusCredit(EftByDownloadTime);
      }

      public double Frames
      {
         get
         {
            return CurrentProtein.Frames;
         }
      }

      public string Core
      {
         get
         {
            return CurrentProtein.Core;
         }
      }
      #endregion

      #region Frame (UnitFrame) Data Variables
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      public Int32 FramesObserved
      {
         get { return _unitInfo.FramesObserved; }
         set { _unitInfo.FramesObserved = value; }
      }

      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      public IUnitFrame CurrentFrame
      {
         get { return _unitInfo.CurrentFrame; }
      }

      /// <summary>
      /// Timestamp from the last completed frame
      /// </summary>
      public TimeSpan TimeOfLastFrame
      {
         get
         {
            if (CurrentFrame != null)
            {
               return CurrentFrame.TimeOfFrame;
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// Last Frame ID based on UnitFrame Data
      /// </summary>
      public Int32 LastUnitFrameID
      {
         get
         {
            // This Property is handled differently than TimeOfLastFrame
            // above because we want the most recent FrameID even if the
            // CurrentFrame Property returns null.
            for (int i = CurrentProtein.Frames; i >= 0; i--)
            {
               if (_unitInfo.UnitFrames.ContainsKey(i))
               {
                  return _unitInfo.UnitFrames[i].FrameID;
               }
            }

            return 0;
         }
      }
      #endregion

      #region Unit Production Variations
      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      public Int32 RawTimePerUnitDownload
      {
         get
         {
            if (FramesObserved > 0)
            {
               // Make sure CurrentFramePercent is greater than 0 to avoid DivideByZeroException - Issue 34
               if (DownloadTime.Equals(DateTime.MinValue) == false && CurrentFrame.FrameID > 0)
               {
                  // Use UnitRetrievalTime (sourced from ClientInstance.LastRetrievalTime) as basis for
                  // TimeSinceUnitDownload.  This removes the use of the "floating" value DateTime.Now
                  // as a basis for the calculation. - Issue 92
                  TimeSpan TimeSinceUnitDownload = UnitRetrievalTime.Subtract(DownloadTime);
                  return (Convert.ToInt32(TimeSinceUnitDownload.TotalSeconds) / CurrentFrame.FrameID);
               }

               return 0;
            }

            return 0;
         }
      }

      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      public TimeSpan TimePerUnitDownload
      {
         get
         {
            return TimeSpan.FromSeconds(RawTimePerUnitDownload);
         }
      }

      /// <summary>
      /// PPD based on average frame time since unit download
      /// </summary>
      public double PPDPerUnitDownload
      {
         get
         {
            return GetPPD(TimePerUnitDownload);
         }
      }

      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      public Int32 RawTimePerAllSections
      {
         get
         {
            if (FramesObserved > 0)
            {
               return GetDurationInSeconds(FramesObserved);
            }

            return 0;
         }
      }

      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      public TimeSpan TimePerAllSections
      {
         get
         {
            return TimeSpan.FromSeconds(RawTimePerAllSections);
         }
      }

      /// <summary>
      /// PPD based on average frame time over all sections
      /// </summary>
      public double PPDPerAllSections
      {
         get
         {
            return GetPPD(TimePerAllSections);
         }
      }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      public Int32 RawTimePerThreeSections
      {
         get
         {
            // time is valid for 3 "sets" ago
            if (FramesObserved > 3)
            {
               return GetDurationInSeconds(3);
            }

            return 0;
         }
      }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      public TimeSpan TimePerThreeSections
      {
         get
         {
            return TimeSpan.FromSeconds(RawTimePerThreeSections);
         }
      }

      /// <summary>
      /// PPD based on average frame time over the last three sections
      /// </summary>
      public double PPDPerThreeSections
      {
         get
         {
            return GetPPD(TimePerThreeSections);
         }
      }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      public Int32 RawTimePerLastSection
      {
         get
         {
            // time is valid for 1 "set" ago
            if (FramesObserved > 1)
            {
               return Convert.ToInt32(CurrentFrame.FrameDuration.TotalSeconds);
            }

            return 0;
         }
      }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      public TimeSpan TimePerLastSection
      {
         get
         {
            // time is valid for 1 "set" ago
            if (FramesObserved > 1)
            {
               return CurrentFrame.FrameDuration;
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// PPD based on frame time of the last section
      /// </summary>
      public double PPDPerLastSection
      {
         get
         {
            return GetPPD(TimePerLastSection);
         }
      }
      
      private double GetPPD(TimeSpan frameTime)
      {
         if (CurrentProtein.IsUnknown == false)
         {
            // Issue 125
            if (_Prefs.GetPreference<bool>(Preference.CalculateBonus))
            {
               // Issue 183
               if (_clientInstance.Status.Equals(ClientStatus.RunningAsync) ||
                   _clientInstance.Status.Equals(ClientStatus.RunningNoFrameTimes))
               {
                  return CurrentProtein.GetPPD(frameTime, EftByFrameTime);
               }
               
               return CurrentProtein.GetPPD(frameTime, EftByDownloadTime);
            }

            return CurrentProtein.GetPPD(frameTime);
         }

         return 0;
      }
      
      #endregion

      #region Calculate Production Variations
      /// <summary>
      /// Get the average duration over the specified number of most recent frames
      /// </summary>
      /// <param name="numberOfFrames">Number of most recent frames</param>
      private int GetDurationInSeconds(int numberOfFrames)
      {
         // No Frames have been captured yet, just return 0.
         if (CurrentFrame == null || _unitInfo.UnitFrames == null)
         {
            return 0;
         }

         if (numberOfFrames < 1) //TODO: possibly add an upper bound check here
         {
            throw new ArgumentOutOfRangeException("numberOfFrames", "Argument 'numberOfFrames' must be greater than 0.");
         }

         int AverageSeconds = 0;

         try
         {
            int frameNumber = CurrentFrame.FrameID;

            // Make sure we only add frame durations greater than a Zero TimeSpan
            // The first frame will always have a Zero TimeSpan for frame duration
            // we don't want to take this frame into account when calculating 'AllFrames' - Issue 23
            TimeSpan TotalTime = TimeSpan.Zero;
            int countFrames = 0;

            for (int i = 0; i < numberOfFrames; i++)
            {
               if (_unitInfo.UnitFrames[frameNumber].FrameDuration > TimeSpan.Zero)
               {
                  TotalTime = TotalTime.Add(_unitInfo.UnitFrames[frameNumber].FrameDuration);
                  countFrames++;
               }
               frameNumber--;
            }

            if (countFrames > 0)
            {
               AverageSeconds = Convert.ToInt32(TotalTime.TotalSeconds) / countFrames;
            }
         }
         catch (NullReferenceException ex)
         {
            AverageSeconds = 0;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         }

         return AverageSeconds;
      }
      #endregion

      #region Helpers
      public void SetCurrentProtein()
      {
         CurrentProtein = _proteinCollection.GetProtein(ProjectID);
      }
  
      /// <summary>
      /// Determine the client type based on the current protein core
      /// </summary>
      /// <param name="currentProtein">Current Instance Protein</param>
      /// <returns>Client Type</returns>
      private static ClientType GetClientTypeFromProtein(IProtein currentProtein)
      {
         switch (currentProtein.Core.ToUpperInvariant())
         {
            case "GROMACS":
            case "DGROMACS":
            case "GBGROMACS":
            case "AMBER":
            //case "QMD":
            case "GROMACS33":
            case "GROST":
            case "GROSIMT":
            case "DGROMACSB":
            case "DGROMACSC":
            case "GRO-A4":
            //case "TINKER":
            /*** ProtoMol Only */
            case "PROTOMOL":
               /*******************/
               return ClientType.Standard;
            case "GRO-SMP":
            case "GROCVS":
            case "GRO-A3":
               return ClientType.SMP;
            case "GROGPU2":
            case "GROGPU2-MT":
            case "ATI-DEV":
            case "NVIDIA-DEV":
               return ClientType.GPU;
            default:
               return ClientType.Unknown;
         }
      }
      #endregion
   }
}
