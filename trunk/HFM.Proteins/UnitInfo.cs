/*
 * HFM.NET - Unit Info Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using HFM.Instrumentation;

namespace HFM.Proteins
{
   #region Enum
   public enum ClientType
   {
      Unknown,
      Standard,
      SMP,
      GPU
   } 
   
   public enum WorkUnitResult
   {
      Unknown,
      FinishedUnit,
      EarlyUnitEnd,
      UnstableMachine
   }
   #endregion

   /// <summary>
   /// Contains the state of a protein in progress
   /// </summary>
   [Serializable]
   public class UnitInfo
   {
      #region Const
      public const string UsernameDefault = "Unknown";
      public const int TeamDefault = 0;
      
      private const string FinishedUnit = "FINISHED_UNIT";
      private const string EarlyUnitEnd = "EARLY_UNIT_END";
      private const string UnstableMachine = "UNSTABLE_MACHINE";
      #endregion
   
      #region CTOR
      /// <summary>
      /// Overload Constructor
      /// </summary>
      public UnitInfo(string ownerName, string ownerPath)
         : this(ownerName, ownerPath, UsernameDefault, TeamDefault)
      {

      }

      /// <summary>
      /// Primary Constructor
      /// </summary>
      public UnitInfo(string ownerName, string ownerPath, string foldingID, int team)
      {
         _OwningInstanceName = ownerName;
         _OwningInstancePath = ownerPath;
         _FoldingID = foldingID;
         _Team = team;
         
         Clear();
         ClearFrameData();
      } 
      #endregion

      #region Owner Data Properties
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      private string _OwningInstanceName;
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstanceName
      {
         get { return _OwningInstanceName; }
         set { _OwningInstanceName = value; }
      }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      private string _OwningInstancePath;
      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstancePath
      {
         get { return _OwningInstancePath; }
         set { _OwningInstancePath = value; }
      }
      #endregion

      #region Read Only Properties
      
      /// <summary>
      /// Formatted Project (Run, Clone, Gen) Information
      /// </summary>
      public string ProjectRunCloneGen
      {
         get
         {
            return String.Format("P{0} (R{1}, C{2}, G{3})", ProjectID,
                                                            ProjectRun,
                                                            ProjectClone,
                                                            ProjectGen);
         }
      }

      /// <summary>
      /// Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      public Int32 RawTimePerSection
      {
         get
         {
            switch (Preferences.PreferenceSet.Instance.PpdCalculation)
            {
               case Preferences.ePpdCalculation.LastFrame:
                  return _RawTimePerLastSection;
               case Preferences.ePpdCalculation.LastThreeFrames:
                  return _RawTimePerThreeSections;
               case Preferences.ePpdCalculation.AllFrames:
                  return _RawTimePerAllSections;
               case Preferences.ePpdCalculation.EffectiveRate:
                  return _RawTimePerUnitDownload;
            }

            return 0;
         }
      }
      
      /// <summary>
      /// Work unit deadline
      /// </summary>
      public DateTime Deadline
      {
         get 
         {
            return DownloadTime.AddDays(CurrentProtein.PreferredDays);
         }
      }
      
      /// <summary>
      /// Returns true if Project (R/C/G) has not been identified
      /// </summary>
      public bool ProjectIsUnknown
      {
         get 
         {
            return ProjectID == 0 &&
                   ProjectRun == 0 &&
                   ProjectClone == 0 &&
                   ProjectGen ==0;
         }
      }

      #region Values Based on UnitFrame Data
      /// <summary>
      /// Timestamp from the last completed frame
      /// </summary>
      public TimeSpan TimeOfLastFrame
      {
         get
         {
            if (_CurrentFrame != null)
            {
               return _CurrentFrame.TimeOfFrame;
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// Last frame percent based on UnitFrame Data
      /// </summary>
      public Int32 LastUnitFramePercent
      {
         get
         {
            for (int i = 100; i >= 0; i--)
            {
               if (UnitFrames[i] != null)
               {
                  return UnitFrames[i].FramePercent;
               }
            }

            return 0;
         }
      }
      #endregion
      
      #endregion

      #region Public Properties and Related Private Members
      
      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      private string _FoldingID;
      /// <summary>
      /// The Folding ID (Username) attached to this work unit
      /// </summary>
      public string FoldingID
      {
         get { return _FoldingID; }
         set { _FoldingID = value; }
      }

      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      private Int32 _Team;
      /// <summary>
      /// The Team number attached to this work unit
      /// </summary>
      public Int32 Team
      {
         get { return _Team; }
         set { _Team = value; }
      }
      
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      private ClientType _TypeOfClient;
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public ClientType TypeOfClient
      {
         get { return _TypeOfClient; }
         set { _TypeOfClient = value; }
      }

      /// <summary>
      /// Core Version Number
      /// </summary>
      private string _CoreVersion = String.Empty;
      /// <summary>
      /// Core Version Number
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
         set { _CoreVersion = value; }
      }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      private DateTime _DownloadTime;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
         set { _DownloadTime = value; }
      }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      private DateTime _DueTime;
      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime
      {
         get { return _DueTime; }
         set { _DueTime = value; }
      }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      private Int32 _FramesComplete;
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public Int32 FramesComplete
      {
         get { return _FramesComplete; }
         set { _FramesComplete = value; }
      }

      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      private TimeSpan _UnitStartTime;
      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      public TimeSpan UnitStartTime
      {
         get { return _UnitStartTime; }
         set { _UnitStartTime = value; }
      }

      /// <summary>
      /// Project ID Number
      /// </summary>
      private Int32 _ProjectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; }
      }

      /// <summary>
      /// Project ID (Run)
      /// </summary>
      private Int32 _ProjectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get { return _ProjectRun; }
         set { _ProjectRun = value; }
      }

      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      private Int32 _ProjectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get { return _ProjectClone; }
         set { _ProjectClone = value; }
      }

      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      private Int32 _ProjectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get { return _ProjectGen; }
         set { _ProjectGen = value; }
      }
      
      /// <summary>
      /// Name of the unit
      /// </summary>
      private String _ProteinName = String.Empty;
      /// <summary>
      /// Name of the unit
      /// </summary>
      public String ProteinName
      {
         get { return _ProteinName; }
         set { _ProteinName = value; }
      }

      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      private string _ProteinTag = String.Empty;
      /// <summary>
      /// Tag string as read from the UnitInfo.txt file
      /// </summary>
      public string ProteinTag
      {
         get { return _ProteinTag; }
         set { _ProteinTag = value; }
      }

      /// <summary>
      /// Raw number of frames complete (this is not always a percent value)
      /// </summary>
      private Int32 _RawComplete;
      /// <summary>
      /// Raw number of frames complete (this is not always a percent value)
      /// </summary>
      public Int32 RawFramesComplete
      {
         get { return _RawComplete; }
         set
         {
            _RawComplete = value;
         }
      }

      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      private Int32 _RawTotal;
      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      public Int32 RawFramesTotal
      {
         get { return _RawTotal; }
         set
         {
            _RawTotal = value;
         }
      }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      private WorkUnitResult _UnitResult;
      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      public WorkUnitResult UnitResult
      {
         get { return _UnitResult; }
         set { _UnitResult = value; }
      }

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      private Protein _CurrentProtein;
      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public Protein CurrentProtein
      {
         get { return _CurrentProtein; }
         set 
         {
            _CurrentProtein = value;
            ClearFrameData();
         }
      }

      #region Time Based Values
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      private Int32 _PercentComplete;
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public Int32 PercentComplete
      {
         get { return _PercentComplete; }
         set
         {
            // Add check for valid values instead of just accepting whatever is given - Issue 2
            if (value < 0 || value > 100)
            {
               _PercentComplete = 0;
            }
            else
            {
               _PercentComplete = value;
            }
         }
      }
      
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      private TimeSpan _TimePerFrame;
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan TimePerFrame
      {
         get { return _TimePerFrame; }
         set { _TimePerFrame = value; }
      }

      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      private Double _UPD;
      /// <summary>
      /// Units per day (UPD) rating for this instance
      /// </summary>
      public Double UPD
      {
         get
         {
            return _UPD;
         }
         set { _UPD = value; }
      }

      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      private Double _PPD;
      /// <summary>
      /// Points per day (PPD) rating for this instance
      /// </summary>
      public Double PPD
      {
         get
         {
            return _PPD;
         }
         set { _PPD = value; }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      private TimeSpan _ETA;
      /// <summary>
      /// Esimated time of arrival (ETA) for this protein
      /// </summary>
      public TimeSpan ETA
      {
         get { return _ETA; }
         set { _ETA = value; }
      }

      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      private Int32 _RawTimePerUnitDownload;
      /// <summary>
      /// Average frame time since unit download
      /// </summary>
      public Int32 RawTimePerUnitDownload
      {
         get { return _RawTimePerUnitDownload; }
         set
         {
            _RawTimePerUnitDownload = value;
         }
      }
      
      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      private Int32 _RawTimePerAllSections = 0;
      /// <summary>
      /// Average frame time over all sections
      /// </summary>
      public Int32 RawTimePerAllSections
      {
         get { return _RawTimePerAllSections; }
         set
         {
            _RawTimePerAllSections = value;
         }
      }

      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      private Int32 _RawTimePerThreeSections = 0;
      /// <summary>
      /// Average frame time over the last three sections
      /// </summary>
      public Int32 RawTimePerThreeSections
      {
         get { return _RawTimePerThreeSections; }
         set
         {
            _RawTimePerThreeSections = value;
         }
      }

      /// <summary>
      /// Frame time of the last section
      /// </summary>
      private Int32 _RawTimePerLastSection = 0;
      /// <summary>
      /// Frame time of the last section
      /// </summary>
      public Int32 RawTimePerLastSection
      {
         get { return _RawTimePerLastSection; }
         set
         {
            _RawTimePerLastSection = value;
         }
      }
      #endregion

      #region Frame (UnitFrame) Data Variables

      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      private Int32 _FramesObserved = 0;
      /// <summary>
      /// Number of Frames Observed on this Unit
      /// </summary>
      public Int32 FramesObserved
      {
         get { return _FramesObserved; }
         set { _FramesObserved = value; }
      }
      
      /// <summary>
      /// Last Observed Frame on this Unit
      /// </summary>
      private UnitFrame _CurrentFrame = null;
      public UnitFrame CurrentFrame
      {
         get { return _CurrentFrame; }
         set { _CurrentFrame = value; }
      }
      
      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      private UnitFrame[] _UnitFrames = null;
      /// <summary>
      /// Frame Data for this Unit
      /// </summary>
      public UnitFrame[] UnitFrames
      {
         get { return _UnitFrames; }
         set { _UnitFrames = value; }
      }
      
      #endregion
      
      #endregion

      #region Clear UnitInfo and Clear Time Based Values
      private void Clear()
      {
         TypeOfClient = ClientType.Unknown;
         CoreVersion = String.Empty;
         DownloadTime = DateTime.MinValue;
         DueTime = DateTime.MinValue;
         FramesComplete = 0;
         PercentComplete = 0;
         ProjectID = 0;
         ProjectRun = 0;
         ProjectClone = 0;
         ProjectGen = 0;
         ProteinName = String.Empty;
         ProteinTag = String.Empty;
         RawFramesComplete = 0;
         RawFramesTotal = 0;
         UnitResult = WorkUnitResult.Unknown;
         CurrentProtein = new Protein();

         ClearTimeBasedValues();
      }
      
      /// <summary>
      /// Clear only the time based values for this instance
      /// </summary>
      public void ClearTimeBasedValues()
      {
         // Set in SetTimeBasedValues()
         PercentComplete = 0;
         TimePerFrame = TimeSpan.Zero;
         UPD = 0.0;
         PPD = 0.0;
         ETA = TimeSpan.Zero;

         // Set in SetFrameTimes()
         RawTimePerUnitDownload = 0;
         RawTimePerAllSections = 0;
         RawTimePerThreeSections = 0;
         RawTimePerLastSection = 0;
      }
      #endregion

      #region Set Frame and Clear Frame Data
      /// <summary>
      /// Set the Current Work Unit Frame
      /// </summary>
      /// <param name="frame">Current Work Unit Frame</param>
      public void SetCurrentFrame(UnitFrame frame)
      {
         if (_UnitFrames[frame.FramePercent] == null)
         {
            // increment observed count
            _FramesObserved++;
         
            CurrentFrame = frame;
            UnitFrames[CurrentFrame.FramePercent] = CurrentFrame;
            
            CurrentFrame.FrameDuration = TimeSpan.Zero;
            if (CurrentFrame.FramePercent > 0 && UnitFrames[CurrentFrame.FramePercent - 1] != null && FramesObserved > 1)
            {
               CurrentFrame.FrameDuration = GetDelta(CurrentFrame.TimeOfFrame, UnitFrames[CurrentFrame.FramePercent - 1].TimeOfFrame);
            }
         }
      }

      /// <summary>
      /// Clear the Observed Count, Current Frame Pointer, and the UnitFrames Array
      /// </summary>
      public void ClearFrameData()
      {
         _FramesObserved = 0;
         _CurrentFrame = null;
         _UnitFrames = new UnitFrame[CurrentProtein.Frames + 1]; //[101];
      }
      #endregion

      #region Calculate Frame Time Variations
      /// <summary>
      /// Sets Frame Time Variations based on the observed frames
      /// </summary>
      public void SetFrameTimes()
      {
         RawTimePerLastSection = 0;
         RawTimePerThreeSections = 0;
         RawTimePerAllSections = 0;
         RawTimePerUnitDownload = 0;
         
         if (FramesObserved > 0)
         {
            // time is valid for 1 "set" ago
            if (_FramesObserved > 1)
            {
               RawTimePerLastSection = Convert.ToInt32(_CurrentFrame.FrameDuration.TotalSeconds);
            }
            
            // time is valid for 3 "sets" ago
            if (_FramesObserved > 3)
            {
               RawTimePerThreeSections = GetDuration(3);
            }

            RawTimePerAllSections = GetDuration(FramesObserved);

            // Make sure CurrentFramePercent is greater than 0 to avoid DivideByZeroException - Issue 34
            if (DownloadTime.Equals(DateTime.MinValue) == false && CurrentFrame.FramePercent > 0)
            {
               TimeSpan timeSinceUnitDownload = DateTime.Now.Subtract(DownloadTime);
               RawTimePerUnitDownload = (Convert.ToInt32(timeSinceUnitDownload.TotalSeconds) / CurrentFrame.FramePercent);
            }
         }
      }
      
      /// <summary>
      /// Get the total duration over the specified number of most recent frames
      /// </summary>
      /// <param name="numberOfFrames">Number of most recent frames</param>
      public int GetDuration(int numberOfFrames)
      {
         int TotalSeconds = 0;
         int frameNumber = CurrentFrame.FramePercent;
         
         try
         {
            // Make sure we only add frame durations greater than a Zero TimeSpan
            // The first frame will always have a Zero TimeSpan for frame duration
            // we don't want to take this frame into account when calculating 'AllFrames' - Issue 23
            TimeSpan TotalTime = TimeSpan.Zero;
            int countFrames = 0;
            
            for (int i = 0; i < numberOfFrames; i++)
            {
               if (UnitFrames[frameNumber].FrameDuration > TimeSpan.Zero)
               {
                  TotalTime = TotalTime.Add(UnitFrames[frameNumber].FrameDuration);
                  countFrames++;
               }
               frameNumber--;
            }
            
            if (countFrames > 0)
            {
               TotalSeconds = Convert.ToInt32(TotalTime.TotalSeconds) / countFrames;
            }
         }
         catch (NullReferenceException ex)
         {
            TotalSeconds = 0;
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         }

         return TotalSeconds;
      }

      /// <summary>
      /// Get Time Delta between given frames
      /// </summary>
      /// <param name="timeLastFrame">Time of last frame</param>
      /// <param name="timeCompareFrame">Time of a previous frame to compare</param>
      private static TimeSpan GetDelta(TimeSpan timeLastFrame, TimeSpan timeCompareFrame)
      {
         TimeSpan tDelta;

         // check for rollover back to 00:00:00 timeLastFrame will be less than previous timeCompareFrame reading
         if (timeLastFrame < timeCompareFrame)
         {
            // get time before rollover
            tDelta = TimeSpan.FromDays(1).Subtract(timeCompareFrame);
            // add time from latest reading
            tDelta = tDelta.Add(timeLastFrame);
         }
         else
         {
            tDelta = timeLastFrame.Subtract(timeCompareFrame);
         }

         return tDelta;
      }
      #endregion
      
      /// <summary>
      /// Get the WorkUnitResult Enum representation of the given result string.
      /// </summary>
      /// <param name="result">Work Unit Result as String.</param>
      public static WorkUnitResult WorkUnitResultFromString(string result)
      {
         switch (result)
         {
            case FinishedUnit:
               return WorkUnitResult.FinishedUnit;
            case EarlyUnitEnd:
               return WorkUnitResult.EarlyUnitEnd;
            case UnstableMachine:
               return WorkUnitResult.UnstableMachine;
            default:
               return WorkUnitResult.Unknown;
         }
      }
   }
}
