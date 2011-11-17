/*
 * HFM.NET - Unit Info Logic Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Linq;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IUnitInfoLogic
   {
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      UnitInfo UnitInfoData { get; set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      DateTime DownloadTime { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime PreferredDeadline { get; }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      DateTime FinalDeadline { get; }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      DateTime FinishedTime { get; }

      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      Protein CurrentProtein { get; set; }

      /// <summary>
      /// Frame progress of the unit
      /// </summary>
      int FramesComplete { get; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      int PercentComplete { get; }

      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      TimeSpan GetFrameTime(PpdCalculationType calculationType);

      /// <summary>
      /// Work unit credit
      /// </summary>
      double GetCredit(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      double GetUPD(PpdCalculationType calculationType);

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      double GetPPD(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      TimeSpan GetEta(PpdCalculationType calculationType);

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      DateTime GetEtaDate(PpdCalculationType calculationType);

      /// <summary>
      /// Specifies if All Frames have been Completed
      /// </summary>
      bool AllFramesCompleted { get; }

      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      int GetRawTime(PpdCalculationType calculationType);

      void ShowPPDTrace(ILogger logger, ClientStatus status, PpdCalculationType calculationType, bool calculateBonus);
   }

   public sealed class UnitInfoLogic : IUnitInfoLogic
   {
      #region Fields

      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinBenchmarkCollection _benchmarkCollection;

      private UnitInfo _unitInfo = new UnitInfo();
      /// <summary>
      /// Unit Info Data Class
      /// </summary>
      public UnitInfo UnitInfoData
      {
         get { return _unitInfo; }
         set { _unitInfo = value ?? new UnitInfo(); }
      }

      private Protein _currentProtein = new Protein();
      /// <summary>
      /// Class member containing info on the currently running protein
      /// </summary>
      public Protein CurrentProtein
      {
         get { return _currentProtein; }
         set { _currentProtein = value ?? new Protein(); }
      }

      public double ClientTimeOffset { get; set; }

      public bool UtcOffsetIsZero { get; set; }

      #endregion

      #region Constructors

      public UnitInfoLogic(IProteinBenchmarkCollection benchmarkCollection)
      {
         _benchmarkCollection = benchmarkCollection;
      }

      #endregion

      #region Unit Level Members

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return GetTime(_unitInfo.DownloadTime); }
      }

      /// <summary>
      /// Date/time the unit is due (preferred deadline)
      /// </summary>
      public DateTime DueTime
      {
         get { return GetTime(_unitInfo.DueTime); }
      }

      /// <summary>
      /// Date/time the unit finished
      /// </summary>
      public DateTime FinishedTime
      {
         get { return GetTime(_unitInfo.FinishedTime); }
      }
      
      private DateTime GetTime(DateTime dateTime)
      {
         if (dateTime.IsUnknown()) { return dateTime; }

         var offset = TimeSpan.FromMinutes(ClientTimeOffset);
         return UtcOffsetIsZero
                   ? dateTime.Subtract(offset)
                   : dateTime.ToLocalTime().Subtract(offset);
      }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime PreferredDeadline
      {
         get
         {
            if (_unitInfo.DownloadTime.IsUnknown()) return _unitInfo.DownloadTime;

            return CurrentProtein.IsUnknown()
                      ? DueTime
                      : AdjustDeadlineForDaylightSavings(CurrentProtein.PreferredDays);
         }
      }

      /// <summary>
      /// Work Unit Preferred Deadline
      /// </summary>
      public DateTime FinalDeadline
      {
         get
         {
            if (_unitInfo.DownloadTime.IsUnknown()) return _unitInfo.DownloadTime;

            return CurrentProtein.IsUnknown()
                      ? DateTime.MinValue
                      : AdjustDeadlineForDaylightSavings(CurrentProtein.MaximumDays);
         }
      }

      private DateTime AdjustDeadlineForDaylightSavings(double days)
      {
         DateTime deadline = DownloadTime.AddDays(days);

         // download time is DST
         if (DownloadTime.IsDaylightSavingTime())
         {
            if (!deadline.IsDaylightSavingTime())
            {
               return deadline.Subtract(TimeSpan.FromHours(1));
            }
         }
         else // not DST
         {
            if (deadline.IsDaylightSavingTime())
            {
               return deadline.AddHours(1);
            }
         }

         return deadline;
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
            // *** Use the Current Frame ID *** - 2/12/11
            // the log parsing API has already validated the reported
            // frame percentage is within 10% tolerance of the raw
            // completed and total steps.
            //
            // See HFM.Log.LogLineParser.CheckForCompletedFrame()
            //
            // Fix for GPU3 units reporting only 99 Frames Completed
            // to the Work Unit History Database - Issue 253
            if (_unitInfo.CurrentFrame != null)
            {
               // Make sure CurrentFrame.FrameID is 0 or greater 
               if (_unitInfo.CurrentFrame.FrameID >= 0)
               {
                  // but not greater than the CurrentProtein.Frames
                  if (_unitInfo.CurrentFrame.FrameID <= CurrentProtein.Frames)
                  {
                     return _unitInfo.CurrentFrame.FrameID;
                  }

                  // if it is, just return the protein frame count
                  return CurrentProtein.Frames;
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
            // Report even if CurrentProtein.IsUnknown - 11/22/09
            return FramesComplete * 100 / CurrentProtein.Frames;
         }
      }

      #endregion

      #region Production Value Properties

      /// <summary>
      /// Frame Time per section based on current PPD calculation setting (readonly)
      /// </summary>
      public int GetRawTime(PpdCalculationType calculationType)
      {
         switch (calculationType)
         {
            case PpdCalculationType.LastFrame:
               return _unitInfo.FramesObserved > 1 ? Convert.ToInt32(_unitInfo.CurrentFrame.FrameDuration.TotalSeconds) : 0;
            case PpdCalculationType.LastThreeFrames:
               return _unitInfo.FramesObserved > 3 ? GetDurationInSeconds(3) : 0;
            case PpdCalculationType.AllFrames:
               return _unitInfo.FramesObserved > 0 ? GetDurationInSeconds(_unitInfo.FramesObserved) : 0;
            case PpdCalculationType.EffectiveRate:
               return GetRawTimePerUnitDownload();
         }

         return 0;
      }

      private int GetRawTimePerUnitDownload()
      {
         //if (_unitInfo.FramesObserved <= 0) return 0;

         // was making the check on FramesObserved above - I believe this was in an
         // attempt to validate that an object would be assigned to CurrentFrame.
         // if this is truly what we're trying to validate, it would make more sense
         // to simply check CurrentFrame for null - 2/7/11
         if (_unitInfo.CurrentFrame == null) return 0;

         // Make sure FrameID is greater than 0 to avoid DivideByZeroException - Issue 34
         if (DownloadTime.IsUnknown() || _unitInfo.CurrentFrame.FrameID <= 0) { return 0; }

         // Issue 92
         TimeSpan timeSinceUnitDownload = _unitInfo.UnitRetrievalTime.Subtract(DownloadTime);
         return (Convert.ToInt32(timeSinceUnitDownload.TotalSeconds) / _unitInfo.CurrentFrame.FrameID);
      }
      
      /// <summary>
      /// Time per frame (TPF) of the unit
      /// </summary>
      public TimeSpan GetFrameTime(PpdCalculationType calculationType)
      {
         int rawTime = GetRawTime(calculationType);
         if (rawTime != 0)
         {
            return TimeSpan.FromSeconds(rawTime);
         }

         // Issue 79 - no benchmark container is available to merged display instances
         if (_benchmarkCollection != null)
         {
            ProteinBenchmark benchmark = _benchmarkCollection.GetBenchmark(UnitInfoData);
            if (benchmark != null)
            {
               return benchmark.AverageFrameTime;
            }
         }
         return TimeSpan.Zero;
      }

      /// <summary>
      /// Work unit credit
      /// </summary>
      public double GetCredit(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus)
      {
         TimeSpan frameTime = GetFrameTime(calculationType);
         return GetCredit(GetEftByDownloadTime(frameTime), GetEftByFrameTime(frameTime), status, calculateBonus);
      }

      /// <summary>
      /// Units per day (UPD) rating for this unit
      /// </summary>
      public double GetUPD(PpdCalculationType calculationType)
      {
         return CurrentProtein.GetUPD(GetFrameTime(calculationType));
      }

      /// <summary>
      /// Points per day (PPD) rating for this unit
      /// </summary>
      public double GetPPD(ClientStatus status, PpdCalculationType calculationType, bool calculateBonus)
      {
         TimeSpan frameTime = GetFrameTime(calculationType);
         return GetPPD(frameTime, GetEftByDownloadTime(frameTime), GetEftByFrameTime(frameTime), status, calculateBonus);
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public TimeSpan GetEta(PpdCalculationType calculationType)
      {
         return GetEta(GetFrameTime(calculationType));
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      private TimeSpan GetEta(TimeSpan frameTime)
      {
         return new TimeSpan((CurrentProtein.Frames - FramesComplete) * frameTime.Ticks);
      }

      /// <summary>
      /// Esimated time of arrival (ETA) for this unit
      /// </summary>
      public DateTime GetEtaDate(PpdCalculationType calculationType)
      {
         return _unitInfo.UnitRetrievalTime.Add(GetEta(calculationType));
      }

      /// <summary>
      /// Specifies if All Frames have been Completed
      /// </summary>
      public bool AllFramesCompleted
      {
         // Report even if CurrentProtein.IsUnknown - 2/8/11
         get { return CurrentProtein.Frames == FramesComplete; }
      }
      
      /// <summary>
      /// Esimated Finishing Time (by Download Time)
      /// </summary>
      private TimeSpan GetEftByDownloadTime(TimeSpan frameTime)
      {
         if (DownloadTime.IsUnknown()) return TimeSpan.Zero; 

         if (FinishedTime.IsKnown())
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
         TimeSpan eta = GetEta(frameTime);
         if (eta.IsZero() && AllFramesCompleted == false)
         {
            return TimeSpan.Zero;
         }

         return _unitInfo.UnitRetrievalTime.Add(eta).Subtract(DownloadTime);
      }

      /// <summary>
      /// Esimated Finishing Time (by Frame Time)
      /// </summary>
      private TimeSpan GetEftByFrameTime(TimeSpan frameTime)
      {
         // Don't do this anymore... we actually want to know the
         // EftByFrameTime even if DownloadTime and FinishedTime
         // are known values - 2/8/11
         //if (DownloadTime.Equals(DateTime.MinValue) == false)
         //{
         //   if (FinishedTime.Equals(DateTime.MinValue) == false)
         //   {
         //      return FinishedTime.Subtract(DownloadTime);
         //   }
         //}

         // Report even if CurrentProtein.IsUnknown - 2/8/11
         return TimeSpan.FromSeconds(frameTime.TotalSeconds * CurrentProtein.Frames);
      }

      #endregion

      #region Calculate Credit and PPD

      private double GetCredit(TimeSpan eftByDownloadTime, TimeSpan eftByFrameTime, ClientStatus status, bool calculateBonus)
      {
         if (CurrentProtein.IsUnknown())
         {
            return 0;
         }

         // Issue 125
         if (calculateBonus)
         {
            // Issue 183
            if (status.Equals(ClientStatus.RunningAsync) ||
                status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return CurrentProtein.GetCredit(eftByFrameTime, true);
            }

            return CurrentProtein.GetCredit(eftByDownloadTime, true);
         }

         return CurrentProtein.Credit;
      }
      
      private double GetPPD(TimeSpan frameTime, TimeSpan eftByDownloadTime, TimeSpan eftByFrameTime, ClientStatus status, bool calculateBonus)
      {
         if (CurrentProtein.IsUnknown())
         {
            return 0;
         }
         
         // Issue 125
         if (calculateBonus)
         {
            // Issue 183
            if (status.Equals(ClientStatus.RunningAsync) ||
                status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               return CurrentProtein.GetPPD(frameTime, eftByFrameTime, true);
            }

            return CurrentProtein.GetPPD(frameTime, eftByDownloadTime, true);
         }

         return CurrentProtein.GetPPD(frameTime);
      }

      public void ShowPPDTrace(ILogger logger, ClientStatus status, PpdCalculationType calculationType, bool calculateBonus)
      {
         // test the level
         if (!logger.IsDebugEnabled) return;
      
         if (CurrentProtein.IsUnknown())
         {
            logger.DebugFormat(Constants.InstanceNameFormat, _unitInfo.OwningInstanceName, "Protein is unknown... 0 PPD.");
            return;
         }

         // Issue 125
         if (calculateBonus)
         {
            // Issue 183
            if (status.Equals(ClientStatus.RunningAsync) ||
                status.Equals(ClientStatus.RunningNoFrameTimes))
            {
               logger.DebugFormat(Constants.InstanceNameFormat, _unitInfo.OwningInstanceName, "Calculate Bonus PPD by Frame Time.");
            }
            else
            {
               logger.DebugFormat(Constants.InstanceNameFormat, _unitInfo.OwningInstanceName, "Calculate Bonus PPD by Download Time.");
            }
         }
         else
         {
            logger.DebugFormat(Constants.InstanceNameFormat, _unitInfo.OwningInstanceName, "Calculate Standard PPD.");
         }

         TimeSpan frameTime = GetFrameTime(calculationType);
         var values = CurrentProtein.GetProductionValues(frameTime, GetEftByDownloadTime(frameTime), GetEftByFrameTime(frameTime), calculateBonus);
         values.ToMultiLineString().ToList().ForEach(logger.Debug);
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
         // UnitFrames == null should NEVER happen, but I'm leaving the
         // check here anyway.  CurrentFrame could easily be null.
         if (_unitInfo.UnitFrames == null || _unitInfo.CurrentFrame == null)
         {
            return 0;
         }

         if (numberOfFrames < 1) //TODO: possibly add an upper bound check here
         {
            // used to throw ArgumentOutOfRangeException here, didn't make sense
            // just return 0.  Callers aren't equipt to handle the exception.
            return 0;
         }

         int averageSeconds = 0;

         // Commented try/catch... the ContainsKey() check 
         // should be all that is needed here. - 2/21/11
         //try
         //{
            int frameNumber = _unitInfo.CurrentFrame.FrameID;

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
         //}
         //// Issue 199
         //catch (KeyNotFoundException ex)
         //{
         //   averageSeconds = 0;
         //   HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         //}

         return averageSeconds;
      }
      
      #endregion
   }
}
