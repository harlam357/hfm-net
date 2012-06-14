/*
 * HFM.NET - Status Logic Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Castle.Core.Logging;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IStatusLogic
   {
      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determine the Status.
      /// </summary>
      /// <param name="statusData">Client Status Data</param>
      SlotStatus HandleStatusData(StatusData statusData);
   }

   public class StatusLogic : IStatusLogic
   {
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      /// <summary>
      /// Handles the Client Status Returned by Log Parsing and then determine the Status.
      /// </summary>
      /// <param name="statusData">Client Status Data</param>
      public SlotStatus HandleStatusData(StatusData statusData)
      {
         switch (statusData.ReturnedStatus)
         {
            case SlotStatus.Running:      // at this point, we should not see Running Status
            case SlotStatus.RunningAsync: // at this point, we should not see RunningAsync Status
            case SlotStatus.RunningNoFrameTimes:
               break;
            case SlotStatus.Unknown:
               _logger.Error("Unable to Determine Status for Client '{0}'", statusData.ClientName);
               // Update Client Status - don't call Determine Status
               return statusData.ReturnedStatus;
            case SlotStatus.Offline:
            case SlotStatus.Stopped:
            case SlotStatus.EuePause:
            case SlotStatus.Hung:
            case SlotStatus.Paused:
            case SlotStatus.SendingWorkPacket:
            case SlotStatus.GettingWorkPacket:
               // Update Client Status - don't call Determine Status
               return statusData.ReturnedStatus;
         }

         // if we have a frame time, use it
         if (statusData.FrameTime > 0)
         {
            SlotStatus status = DetermineStatus(statusData);
            if (status.Equals(SlotStatus.Hung) && statusData.AllowRunningAsync) // Issue 124
            {
               return DetermineAsyncStatus(statusData);
            }

            return status;
         }

         // no frame time based on the current PPD calculation selection ('LastFrame', 'LastThreeFrames', etc)
         // this section attempts to give DetermineStats values to detect Hung clients before they have a valid
         // frame time - Issue 10
         else
         {
            // if we have no time stamp
            if (statusData.TimeOfLastFrame == TimeSpan.Zero)
            {
               // use the unit start time
               statusData.TimeOfLastFrame = statusData.UnitStartTimeStamp;
            }

            statusData.FrameTime = GetBaseFrameTime(statusData.BenchmarkAverageFrameTime, statusData.SlotType);
            if (DetermineStatus(statusData).Equals(SlotStatus.Hung))
            {
               // Issue 124
               if (statusData.AllowRunningAsync)
               {
                  if (DetermineAsyncStatus(statusData).Equals(SlotStatus.Hung))
                  {
                     return SlotStatus.Hung;
                  }
                  else
                  {
                     return statusData.ReturnedStatus;
                  }
               }

               return SlotStatus.Hung;
            }
            else
            {
               return statusData.ReturnedStatus;
            }
         }
      }

      private static int GetBaseFrameTime(TimeSpan averageFrameTime, SlotType slotType)
      {
         // no frame time based on the current PPD calculation selection ('LastFrame', 'LastThreeFrames', etc)
         // this section attempts to give DetermineStats values to detect Hung clients before they have a valid
         // frame time - Issue 10

         // get the average frame time for this client and project id
         if (averageFrameTime > TimeSpan.Zero)
         {
            return Convert.ToInt32(averageFrameTime.TotalSeconds);
         }

         // no benchmarked average frame time, use some arbitrary (and large) values for the frame time
         // we want to give the client plenty of time to show progress but don't want it to sit idle for days
         else
         {
            // CPU: use 1 hour (3600 seconds) as a base frame time
            int baseFrameTime = 3600;
            if (slotType.Equals(SlotType.GPU))
            {
               // GPU: use 10 minutes (600 seconds) as a base frame time
               baseFrameTime = 600;
            }

            return baseFrameTime;
         }
      }
      
      /// <summary>
      /// Determine Client Status
      /// </summary>
      /// <param name="statusData">Client Status Data</param>
      private SlotStatus DetermineStatus(StatusData statusData)
      {
         #region Get Terminal Time
         // Terminal Time - defined as last retrieval time minus twice (7 times for GPU) the current Raw Time per Section.
         // if a new frame has not completed in twice the amount of time it should take to complete we should deem this client Hung.
         DateTime terminalDateTime;

         if (statusData.SlotType.Equals(SlotType.GPU))
         {
            terminalDateTime = statusData.UnitRetrievalTime.Subtract(TimeSpan.FromSeconds(statusData.FrameTime * 7));
         }
         else
         {
            terminalDateTime = statusData.UnitRetrievalTime.Subtract(TimeSpan.FromSeconds(statusData.FrameTime * 2));
         }
         #endregion

         #region Get Last Retrieval Time Date
         DateTime currentFrameDateTime;

         if (statusData.UtcOffsetIsZero)
         {
            // get only the date from the last retrieval time (in universal), we'll add the current time below
            currentFrameDateTime = new DateTime(statusData.UnitRetrievalTime.Date.Ticks, DateTimeKind.Utc);
         }
         else
         {
            // get only the date from the last retrieval time, we'll add the current time below
            currentFrameDateTime = statusData.UnitRetrievalTime.Date;
         }
         #endregion

         #region Apply Frame Time Offset and Set Current Frame Time Date
         TimeSpan offset = TimeSpan.FromMinutes(statusData.ClientTimeOffset);
         TimeSpan adjustedFrameTime = statusData.TimeOfLastFrame;
         if (statusData.UtcOffsetIsZero == false)
         {
            adjustedFrameTime = adjustedFrameTime.Add(statusData.UtcOffset);
         }
         adjustedFrameTime = adjustedFrameTime.Subtract(offset);

         // client time has already rolled over to the next day. the offset correction has 
         // caused the adjusted frame time span to be negetive.  take the that negetive span
         // and add it to a full 24 hours to correct.
         if (adjustedFrameTime < TimeSpan.Zero)
         {
            adjustedFrameTime = TimeSpan.FromDays(1).Add(adjustedFrameTime);
         }

         // the offset correction has caused the frame time span to be greater than 24 hours.
         // subtract the extra day from the adjusted frame time span.
         else if (adjustedFrameTime > TimeSpan.FromDays(1))
         {
            adjustedFrameTime = adjustedFrameTime.Subtract(TimeSpan.FromDays(1));
         }

         // add adjusted Time of Last Frame (TimeSpan) to the DateTime with the correct date
         currentFrameDateTime = currentFrameDateTime.Add(adjustedFrameTime);
         #endregion

         #region Check For Frame from Prior Day (Midnight Rollover on Local Machine)
         bool priorDayAdjust = false;

         // if the current (and adjusted) frame time hours is greater than the last retrieval time hours, 
         // and the time difference is greater than an hour, then frame is from the day prior.
         // this should only happen after midnight time on the machine running HFM when the monitored client has 
         // not completed a frame since the local machine time rolled over to the next day, otherwise the time
         // stamps between HFM and the client are too far off, a positive offset should be set to correct.
         if (currentFrameDateTime.TimeOfDay.Hours > statusData.UnitRetrievalTime.TimeOfDay.Hours &&
             currentFrameDateTime.TimeOfDay.Subtract(statusData.UnitRetrievalTime.TimeOfDay).Hours > 0)
         {
            priorDayAdjust = true;

            // subtract 1 day from today's date
            currentFrameDateTime = currentFrameDateTime.Subtract(TimeSpan.FromDays(1));
         }
         #endregion

         #region Write Verbose Trace
         if (_logger.IsDebugEnabled)
         {
            var messages = new List<string>(10);
            messages.Add(String.Format("{0} ({1})", Instrumentation.FunctionName, statusData.ClientName));
            messages.Add(String.Format(" - Retrieval Time (Date) ------- : {0}", statusData.UnitRetrievalTime));
            messages.Add(String.Format(" - Time Of Last Frame (TimeSpan) : {0}", statusData.TimeOfLastFrame));
            messages.Add(String.Format(" - Offset (Minutes) ------------ : {0}", statusData.ClientTimeOffset));
            messages.Add(String.Format(" - Time Of Last Frame (Adjusted) : {0}", adjustedFrameTime));
            messages.Add(String.Format(" - Prior Day Adjustment -------- : {0}", priorDayAdjust));
            messages.Add(String.Format(" - Time Of Last Frame (Date) --- : {0}", currentFrameDateTime));
            messages.Add(String.Format(" - Terminal Time (Date) -------- : {0}", terminalDateTime));
            messages.ForEach(x => _logger.Debug(x));
         }
         #endregion

         if (currentFrameDateTime > terminalDateTime)
         {
            return SlotStatus.Running;
         }
         else // current frame is less than terminal time
         {
            return SlotStatus.Hung;
         }
      }

      /// <summary>
      /// Determine Client Status
      /// </summary>
      /// <param name="statusData">Client Status Data</param>
      private SlotStatus DetermineAsyncStatus(StatusData statusData)
      {
         #region Get Terminal Time
         // Terminal Time - defined as last retrieval time minus twice (7 times for GPU) the current Raw Time per Section.
         // if a new frame has not completed in twice the amount of time it should take to complete we should deem this client Hung.
         DateTime terminalDateTime;

         if (statusData.SlotType.Equals(SlotType.GPU))
         {
            terminalDateTime = statusData.UnitRetrievalTime.Subtract(TimeSpan.FromSeconds(statusData.FrameTime * 7));
         }
         else
         {
            terminalDateTime = statusData.UnitRetrievalTime.Subtract(TimeSpan.FromSeconds(statusData.FrameTime * 2));
         }
         #endregion

         #region Determine Unit Progress Value to Use
         Debug.Assert(statusData.TimeOfLastUnitStart.Equals(DateTime.MinValue) == false);

         DateTime lastProgress = statusData.TimeOfLastUnitStart;
         if (statusData.TimeOfLastFrameProgress > statusData.TimeOfLastUnitStart)
         {
            lastProgress = statusData.TimeOfLastFrameProgress;
         }
         #endregion

         #region Write Verbose Trace
         if (_logger.IsDebugEnabled)
         {
            var messages = new List<string>(4);
            messages.Add(String.Format("{0} ({1})", Instrumentation.FunctionName, statusData.ClientName));
            messages.Add(String.Format(" - Retrieval Time (Date) ------- : {0}", statusData.UnitRetrievalTime));
            messages.Add(String.Format(" - Time Of Last Unit Start ----- : {0}", statusData.TimeOfLastUnitStart));
            messages.Add(String.Format(" - Time Of Last Frame Progress - : {0}", statusData.TimeOfLastFrameProgress));
            messages.Add(String.Format(" - Terminal Time (Date) -------- : {0}", terminalDateTime));
            messages.ForEach(x => _logger.Debug(x));
         }
         #endregion

         if (lastProgress > terminalDateTime)
         {
            return SlotStatus.RunningAsync;
         }
         else // time of last progress is less than terminal time
         {
            return SlotStatus.Hung;
         }
      }
   }
}
