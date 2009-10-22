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
using System.Diagnostics.CodeAnalysis;

using HFM.Instrumentation;
using HFM.Preferences;
using HFM.Helpers;
using HFM.Proteins;

namespace HFM.Instances
{
   #region Enum
   public enum WorkUnitResult
   {
      Unknown,
      FinishedUnit,
      EarlyUnitEnd,
      UnstableMachine,
      Interrupted
   }
   #endregion

   /// <summary>
   /// Contains the state of a protein in progress
   /// </summary>
   [Serializable]
   public class UnitInfo : IOwnedByClientInstance
   {
      #region Const
      public const string UsernameDefault = "Unknown";
      public const int TeamDefault = 0;
      
      private const string FinishedUnit = "FINISHED_UNIT";
      private const string EarlyUnitEnd = "EARLY_UNIT_END";
      private const string UnstableMachine = "UNSTABLE_MACHINE";
      private const string Interrupted = "INTERRUPTED";
      #endregion
   
      #region CTOR
      /// <summary>
      /// Overload Constructor
      /// </summary>
      public UnitInfo(string ownerName, string ownerPath, DateTime unitRetrievalTime)
         : this(ownerName, ownerPath, unitRetrievalTime, UsernameDefault, TeamDefault)
      {

      }

      /// <summary>
      /// Primary Constructor
      /// </summary>
      public UnitInfo(string ownerName, string ownerPath, DateTime unitRetrievalTime, string foldingID, int team)
      {
         _OwningInstanceName = ownerName;
         _OwningInstancePath = ownerPath;
         _UnitRetrievalTime = unitRetrievalTime;
         _FoldingID = foldingID;
         _Team = team;
         
         Clear();
         ClearUnitFrameData();
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
            switch (PreferenceSet.Instance.PpdCalculation)
            {
               case ePpdCalculation.LastFrame:
                  return RawTimePerLastSection;
               case ePpdCalculation.LastThreeFrames:
                  return RawTimePerThreeSections;
               case ePpdCalculation.AllFrames:
                  return RawTimePerAllSections;
               case ePpdCalculation.EffectiveRate:
                  return RawTimePerUnitDownload;
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
            if (DownloadTime.Equals(DateTime.MinValue) == false)
            {
               return DownloadTime.AddDays(CurrentProtein.PreferredDays);
            }
            
            return DateTime.MinValue;
         }
      }
      
      /// <summary>
      /// Flag specifying if Download Time value is Unknown
      /// </summary>
      public bool DownloadTimeUnknown
      {
         get { return DownloadTime.Equals(DateTime.MinValue); }
      }

      /// <summary>
      /// Flag specifying if Due Time value is Unknown
      /// </summary>
      public bool DueTimeUnknown
      {
         get { return DueTime.Equals(DateTime.MinValue); }
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

      /// <summary>
      /// Flag specifying if Protein Tag value is Unknown
      /// </summary>
      public bool ProteinTagUnknown
      {
         get { return ProteinTag.Length == 0; }
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
      /// When the the used to generate this UnitInfo was retrieved
      /// </summary>
      private readonly DateTime _UnitRetrievalTime;
      /// <summary>
      /// When the log files were last successfully retrieved
      /// </summary>
      public DateTime UnitRetrievalTime
      {
         get { return _UnitRetrievalTime; }
      }

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
      private ClientType _TypeOfClient = ClientType.Unknown;
      /// <summary>
      /// Client Type for this work unit
      /// </summary>
      public ClientType TypeOfClient
      {
         get { return _TypeOfClient; }
         set { _TypeOfClient = value; }
      }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      private DateTime _DownloadTime = DateTime.MinValue;
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
      private DateTime _DueTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime
      {
         get { return _DueTime; }
         set { _DueTime = value; }
      }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      private DateTime _FinishedTime = DateTime.MinValue;
      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      public DateTime FinishedTime
      {
         get { return _FinishedTime; }
         set { _FinishedTime = value; }
      }

      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      private TimeSpan _UnitStartTime = TimeSpan.Zero;
      /// <summary>
      /// Unit Start Time Stamp
      /// </summary>
      public TimeSpan UnitStartTime
      {
         get { return _UnitStartTime; }
         set { _UnitStartTime = value; }
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
      private Int32 _RawFramesComplete;
      /// <summary>
      /// Raw number of frames complete (this is not always a percent value)
      /// </summary>
      public Int32 RawFramesComplete
      {
         get { return _RawFramesComplete; }
         set
         {
            _RawFramesComplete = value;
         }
      }

      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      private Int32 _RawFramesTotal;
      /// <summary>
      /// Raw total number of frames (this is not always 100)
      /// </summary>
      public Int32 RawFramesTotal
      {
         get { return _RawFramesTotal; }
         set
         {
            _RawFramesTotal = value;
         }
      }

      /// <summary>
      /// The Result of this Work Unit
      /// </summary>
      private WorkUnitResult _UnitResult = WorkUnitResult.Unknown;
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
            ClearUnitFrameData();
         }
      }

      #region Time Based Values
      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      public Int32 FramesComplete
      {
         get
         {
            if (CurrentProtein.IsUnknown == false)
            {
               Int32 RawScaleFactor = RawFramesTotal / CurrentProtein.Frames;
               if (RawScaleFactor > 0)
               {
                  return RawFramesComplete / RawScaleFactor;
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
            if (CurrentProtein.IsUnknown == false)
            {
               return FramesComplete * 100 / CurrentProtein.Frames;
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

            return ProteinBenchmarkCollection.Instance.GetBenchmarkAverageFrameTime(this);
         }
      }

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      public Double UPD
      {
         get
         {
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetUPD(TimePerFrame);
            }
            
            return 0;
         }
      }

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      public Double PPD
      {
         get
         {
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetPPD(TimePerFrame);
            }

            return 0;
         }
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public TimeSpan ETA
      {
         get
         {
            return new TimeSpan((100 - PercentComplete) * TimePerFrame.Ticks);
         }
      }

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
               if (DownloadTime.Equals(DateTime.MinValue) == false && CurrentFrame.FramePercent > 0)
               {
                  // Use UnitRetrievalTime (sourced from ClientInstance.LastRetrievalTime) as basis for
                  // TimeSinceUnitDownload.  This removes the use of the "floating" value DateTime.Now
                  // as a basis for the calculation. - Issue 92
                  TimeSpan TimeSinceUnitDownload = UnitRetrievalTime.Subtract(DownloadTime);
                  return (Convert.ToInt32(TimeSinceUnitDownload.TotalSeconds) / CurrentFrame.FramePercent);
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
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetPPD(TimePerUnitDownload);
            }
            
            return 0;
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
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetPPD(TimePerAllSections);
            }

            return 0;
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
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetPPD(TimePerThreeSections);
            }

            return 0;
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
               return Convert.ToInt32(_CurrentFrame.FrameDuration.TotalSeconds);
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
               return _CurrentFrame.FrameDuration;
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
            if (CurrentProtein.IsUnknown == false)
            {
               return CurrentProtein.GetPPD(TimePerLastSection);
            }

            return 0;
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
      [SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays")]
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
         DownloadTime = DateTime.MinValue;
         DueTime = DateTime.MinValue;
         FinishedTime = DateTime.MinValue;
         UnitStartTime = TimeSpan.Zero;
         CoreVersion = String.Empty;
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
         else
         {
            //TODO: Trace write... saw same frame percent twice.
         }
      }

      /// <summary>
      /// Clear the Observed Count, Current Frame Pointer, and the UnitFrames Array
      /// </summary>
      public void ClearUnitFrameData()
      {
         _FramesObserved = 0;
         _CurrentFrame = null;
         // Frames (Percentage) will always need exactly 101 slots
         // If the log output changes from showing completion as
         // percentage, this logic will need revaluated
         _UnitFrames = new UnitFrame[101];
      }
      #endregion

      #region Calculate Frame Time Variations
      /// <summary>
      /// Get the average duration over the specified number of most recent frames
      /// </summary>
      /// <param name="numberOfFrames">Number of most recent frames</param>
      private int GetDurationInSeconds(int numberOfFrames)
      {
         // No Frames have been captured yet, just return 0.
         if (CurrentFrame == null || UnitFrames == null)
         {
            return 0;
         }

         if (numberOfFrames < 1) // || numberOfFrames > 100) TODO: possibly add an upper bound check here
         {
            throw new ArgumentOutOfRangeException("numberOfFrames", "Argument 'numberOfFrames' must be greater than 0.");
         }

         int AverageSeconds = 0;
         
         try
         {
            int frameNumber = CurrentFrame.FramePercent;
         
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
      
      ///// <summary>
      ///// Get the average duration over the specified number of most recent frames
      ///// </summary>
      ///// <param name="numberOfFrames">Number of most recent frames</param>
      //private TimeSpan GetDurationTimeSpan(int numberOfFrames)
      //{
      //   return TimeSpan.FromSeconds(GetDurationInSeconds(numberOfFrames));
      //}

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
      
      #region Static Helpers
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
            case Interrupted:
               return WorkUnitResult.Interrupted;
            default:
               return WorkUnitResult.Unknown;
         }
      }

      /// <summary>
      /// Determine the client type based on the current protein core
      /// </summary>
      /// <param name="CurrentProtein">Current Instance Protein</param>
      /// <returns>Client Type</returns>
      public static ClientType GetClientTypeFromProtein(Protein CurrentProtein)
      {
         switch (CurrentProtein.Core)
         {
            case "GROMACS":
            case "DGROMACS":
            case "GBGROMACS":
            case "AMBER":
            case "QMD":
            case "GROMACS33":
            case "GROST":
            case "GROSIMT":
            case "DGROMACSB":
            case "DGROMACSC":
            case "GRO-A4":
            case "TINKER":
               return ClientType.Standard;
            case "GRO-SMP":
            case "GROCVS":
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
