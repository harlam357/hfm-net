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
using System.Collections.Generic;
using System.Diagnostics;
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
      private readonly IPreferenceSet _prefs;

      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinBenchmarkContainer _benchmarkContainer;

      private readonly UnitInfo _unitInfo;
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      public IUnitInfo UnitInfoData
      {
         get { return _unitInfo; }
      }

      /// <summary>
      /// Owning Client Instance Settings Interface
      /// </summary>
      private readonly IClientInstanceSettings _clientInstanceSettings;

      private readonly IDisplayInstance _displayInstance;
      #endregion

      #region Constructors

      [CLSCompliant(false)]
      public UnitInfoLogic(IPreferenceSet prefs, IProtein protein, IProteinBenchmarkContainer benchmarkContainer, 
                           IUnitInfo unitInfo, IClientInstanceSettings clientInstanceSettings, IDisplayInstance displayInstance)
      {
         _prefs = prefs;
         _benchmarkContainer = benchmarkContainer;
         _unitInfo = (UnitInfo)unitInfo;
         _clientInstanceSettings = clientInstanceSettings;
         _displayInstance = displayInstance;

         _unitInfo.OwningInstanceName = _clientInstanceSettings.InstanceName;
         _unitInfo.OwningInstancePath = _clientInstanceSettings.Path;
         _unitInfo.UnitRetrievalTime = _displayInstance.LastRetrievalTime;

         CurrentProtein = protein;
         _unitInfo.TypeOfClient = PlatformOps.GetClientTypeFromCore(CurrentProtein.Core);
      }

      #endregion

      #region Unit Level Members
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get 
         {
            if (_unitInfo.DownloadTime.Equals(DateTime.MinValue) == false)
            {
               if (_clientInstanceSettings.ClientIsOnVirtualMachine == false)
               {
                  return _unitInfo.DownloadTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset));
               }

               return _unitInfo.DownloadTime.Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset)); 
            }
            
            return _unitInfo.DownloadTime; 
         }
      }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime PreferredDeadline
      {
         get
         {
            if (_unitInfo.DownloadTimeUnknown == false)
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
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime FinalDeadline
      {
         get
         {
            if (_unitInfo.DownloadTimeUnknown == false && CurrentProtein.IsUnknown == false)
            {
               return DownloadTime.AddDays(CurrentProtein.MaxDays);
            }

            return DateTime.MinValue;
         }
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
               if (_clientInstanceSettings.ClientIsOnVirtualMachine == false)
               {
                  return _unitInfo.DueTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset));
               }

               return _unitInfo.DueTime.Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset));
            }

            return _unitInfo.DueTime; 
         }
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
               if (_clientInstanceSettings.ClientIsOnVirtualMachine == false)
               {
                  return _unitInfo.FinishedTime.ToLocalTime().Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset));
               }

               return _unitInfo.FinishedTime.Subtract(TimeSpan.FromMinutes(_clientInstanceSettings.ClientTimeOffset));
            }

            return _unitInfo.FinishedTime; 
         }
      }

      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      public string ProjectRunCloneGen
      {
         get
         {
            return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})",
               _unitInfo.ProjectID, _unitInfo.ProjectRun, _unitInfo.ProjectClone, _unitInfo.ProjectGen);
         }
      }

      private IProtein _currentProtein;
      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public IProtein CurrentProtein
      {
         get { return _currentProtein; }
         protected set
         {
            if (value == null)
            {
               throw new ArgumentException("The given value cannot be null.");
            }

            // Set the New Protein
            _currentProtein = value;
         }
      }
      #endregion

      #region Frames/Percent Completed Unit Level Members
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public int FramesComplete
      {
         get
         {
            // Report Frame Progress even if CurrentProtein.IsUnknown - 11/22/09
            int rawScaleFactor = _unitInfo.RawFramesTotal / CurrentProtein.Frames;
            if (rawScaleFactor > 0)
            {
               int computedFramesComplete = _unitInfo.RawFramesComplete / rawScaleFactor;
               
               // Make sure FramesComplete is 0 or greater but
               // not greater than the CurrentProtein.Frames
               if (computedFramesComplete >= 0 && 
                   computedFramesComplete <= CurrentProtein.Frames)
               {
                  return computedFramesComplete;
               }
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
            // Report Percent Progress even if CurrentProtein.IsUnknown - 11/22/09
            return FramesComplete * 100 / CurrentProtein.Frames;
         }
      }
      #endregion

      #region Production Value Properties
      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      public int RawTimePerSection
      {
         get
         {
            switch (_prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation))
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

            return _benchmarkContainer.GetBenchmarkAverageFrameTime(this);
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
      public double PPD
      {
         get { return GetPPD(TimePerFrame); }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public TimeSpan ETA
      {
         get { return new TimeSpan((CurrentProtein.Frames - FramesComplete) * TimePerFrame.Ticks); }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public DateTime EtaDate
      {
         get { return _unitInfo.UnitRetrievalTime.Add(ETA); }
      }

      /// <summary>
      /// Flag specifying if EtaDate is Unknown
      /// </summary>
      public bool EtaDateUnknown
      {
         get { return EtaDate.Equals(DateTime.MinValue); }
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

               return _unitInfo.UnitRetrievalTime.Add(ETA).Subtract(DownloadTime);
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
               return TimeSpan.FromSeconds(TimePerFrame.TotalSeconds * CurrentProtein.Frames);
            }

            return TimeSpan.Zero;
         }
      }
      #endregion

      #region Frame (UnitFrame) Data Variables
      
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      public int FramesObserved
      {
         get { return _unitInfo.FramesObserved; }
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
      public int LastUnitFrameID
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
      public int RawTimePerUnitDownload
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
                  TimeSpan timeSinceUnitDownload = _unitInfo.UnitRetrievalTime.Subtract(DownloadTime);
                  return (Convert.ToInt32(timeSinceUnitDownload.TotalSeconds) / CurrentFrame.FrameID);
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
         get { return TimeSpan.FromSeconds(RawTimePerUnitDownload); }
      }

      /// <summary>
      /// PPD based on average frame time since unit download
      /// </summary>
      public double PPDPerUnitDownload
      {
         get { return GetPPD(TimePerUnitDownload); }
      }

      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      public int RawTimePerAllSections
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
         get { return TimeSpan.FromSeconds(RawTimePerAllSections); }
      }

      /// <summary>
      /// PPD based on average frame time over all sections
      /// </summary>
      public double PPDPerAllSections
      {
         get { return GetPPD(TimePerAllSections); }
      }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      public int RawTimePerThreeSections
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
         get { return TimeSpan.FromSeconds(RawTimePerThreeSections); }
      }

      /// <summary>
      /// PPD based on average frame time over the last three sections
      /// </summary>
      public double PPDPerThreeSections
      {
         get { return GetPPD(TimePerThreeSections); }
      }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      public int RawTimePerLastSection
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
         get { return GetPPD(TimePerLastSection); }
      }
      
      private double GetPPD(TimeSpan frameTime)
      {
         if (CurrentProtein.IsUnknown)
         {
            return 0;
         }
         
         // Issue 125
         if (_prefs.GetPreference<bool>(Preference.CalculateBonus))
         {
            // Issue 183
            if (_displayInstance.Status.Equals(ClientStatus.RunningAsync) ||
                _displayInstance.Status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return CurrentProtein.GetPPD(frameTime, EftByFrameTime);
            }

            return CurrentProtein.GetPPD(frameTime, EftByDownloadTime);
         }

         return CurrentProtein.GetPPD(frameTime);
      }

      public void ShowPPDTrace()
      {
         if (CurrentProtein.IsUnknown)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _unitInfo.OwningInstanceName, "Current Protein is Unknown... 0 PPD.");
         }
         else
         {
            // Issue 125
            if (_prefs.GetPreference<bool>(Preference.CalculateBonus))
            {
               // Issue 183
               if (_displayInstance.Status.Equals(ClientStatus.RunningAsync) ||
                   _displayInstance.Status.Equals(ClientStatus.RunningNoFrameTimes))
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _unitInfo.OwningInstanceName, "Calculate Bonus PPD - EFT by Frame Time.");
                  CurrentProtein.GetPPD(TimePerFrame, EftByFrameTime, _unitInfo.OwningInstanceName);
               }
               else
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _unitInfo.OwningInstanceName, "Calculate Bonus PPD - EFT by Download Time.");
                  CurrentProtein.GetPPD(TimePerFrame, EftByDownloadTime, _unitInfo.OwningInstanceName);
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _unitInfo.OwningInstanceName, "Calculate Standard PPD.");
               CurrentProtein.GetPPD(TimePerFrame, _unitInfo.OwningInstanceName);
            }
         }
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

         int averageSeconds = 0;

         try
         {
            int frameNumber = CurrentFrame.FrameID;

            // Make sure we only add frame durations greater than a Zero TimeSpan
            // The first frame will always have a Zero TimeSpan for frame duration
            // we don't want to take this frame into account when calculating 'AllFrames' - Issue 23
            TimeSpan totalTime = TimeSpan.Zero;
            int countFrames = 0;

            for (int i = 0; i < numberOfFrames; i++)
            {
               // Issue 199
               if (_unitInfo.UnitFrames.ContainsKey(frameNumber) &&
                   _unitInfo.UnitFrames[frameNumber].FrameDuration > TimeSpan.Zero)
               {
                  totalTime = totalTime.Add(_unitInfo.UnitFrames[frameNumber].FrameDuration);
                  countFrames++;
               }
               frameNumber--;
            }

            if (countFrames > 0)
            {
               averageSeconds = Convert.ToInt32(totalTime.TotalSeconds) / countFrames;
            }
         }
         // Issue 199
         catch (KeyNotFoundException ex)
         {
            averageSeconds = 0;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         }

         return averageSeconds;
      }
      
      #endregion
   }
}
