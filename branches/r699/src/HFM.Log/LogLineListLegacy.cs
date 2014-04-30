/*
 * HFM.NET - Legacy Log Line List Class
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

using HFM.Core.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Log Lines.
   /// </summary>
   internal sealed class LogLineListLegacy : LogLineListBase
   {
      public LogLineListLegacy(LogFileType logFileType)
         : base(logFileType)
      {

      }

      /// <summary>
      /// Inspect the given raw log line and determine the line type.
      /// </summary>
      /// <param name="logLine">The raw log line being inspected.</param>
      protected override LogLineType DetermineLineType(string logLine)
      {
         LogLineType baseLineType = base.DetermineLineType(logLine);
         if (!baseLineType.Equals(LogLineType.Unknown))
         {
            return baseLineType;
         }

         if (logLine.Contains("--- Opening Log file"))
         {
            return LogLineType.LogOpen;
         }
         if (logLine.Contains("###############################################################################"))
         {
            return LogLineType.LogHeader;
         }
         if (logLine.Contains("Folding@Home Client Version"))
         {
            return LogLineType.ClientVersion;
         }
         if (logLine.Contains("] Sending work to server"))
         {
            return LogLineType.ClientSendWorkToServer;
         }
         if (logLine.Contains("] - Autosending finished units..."))
         {
            return LogLineType.ClientAutosendStart;
         }
         if (logLine.Contains("] - Autosend completed"))
         {
            return LogLineType.ClientAutosendComplete;
         }
         if (logLine.Contains("] + Attempting to send results"))
         {
            return LogLineType.ClientSendStart;
         }
         if (logLine.Contains("] + Could not connect to Work Server"))
         {
            return LogLineType.ClientSendConnectFailed;
         }
         if (logLine.Contains("] - Error: Could not transmit unit"))
         {
            return LogLineType.ClientSendFailed;
         }
         if (logLine.Contains("] + Results successfully sent"))
         {
            return LogLineType.ClientSendComplete;
         }
         if (logLine.Contains("Arguments:"))
         {
            return LogLineType.ClientArguments;
         }
         if (logLine.Contains("] - User name:"))
         {
            return LogLineType.ClientUserNameTeam;
         }
         if (logLine.Contains("] + Requesting User ID from server"))
         {
            return LogLineType.ClientRequestingUserID;
         }
         if (logLine.Contains("- Received User ID ="))
         {
            return LogLineType.ClientReceivedUserID;
         }
         if (logLine.Contains("] - User ID:"))
         {
            return LogLineType.ClientUserID;
         }
         if (logLine.Contains("] - Machine ID:"))
         {
            return LogLineType.ClientMachineID;
         }
         if (logLine.Contains("] + Attempting to get work packet"))
         {
            return LogLineType.ClientAttemptGetWorkPacket;
         }
         if (logLine.Contains("] - Will indicate memory of"))
         {
            return LogLineType.ClientIndicateMemory;
         }
         if (logLine.Contains("] - Detect CPU. Vendor:"))
         {
            return LogLineType.ClientDetectCpu;
         }
         if (logLine.Contains("] + Processing work unit"))
         {
            return LogLineType.WorkUnitProcessing;
         }
         if (logLine.Contains("] + Downloading new core"))
         {
            return LogLineType.WorkUnitCoreDownload;
         }
         if (logLine.Contains("] Working on Unit 0"))
         {
            return LogLineType.WorkUnitIndex;
         }
         if (logLine.Contains("] Working on queue slot 0"))
         {
            return LogLineType.WorkUnitQueueIndex;
         }
         if (logLine.Contains("] + Working ..."))
         {
            return LogLineType.WorkUnitWorking;
         }
         if (logLine.Contains("] - Calling"))
         {
            return LogLineType.WorkUnitCallingCore;
         }
         if (logLine.Contains("] *------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*** ProtoMol Only */
         if (logLine.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*******************/
         if (logLine.Contains("] Version"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*** ProtoMol Only */
         if (logLine.Contains("]   Version:"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*******************/
         if (logLine.Contains("] Project:"))
         {
            return LogLineType.WorkUnitProject;
         }
         if (logLine.Contains("] Completed "))
         {
            return LogLineType.WorkUnitFrame;
         }
         if (logLine.Contains("] + Paused"))
         {
            return LogLineType.WorkUnitPaused;
         }
         if (logLine.Contains("] + Running on battery power"))
         {
            return LogLineType.WorkUnitPausedForBattery;
         }
         if (logLine.Contains("] + Off battery, restarting core"))
         {
            return LogLineType.WorkUnitResumeFromBattery;
         }
         if (logLine.Contains("] - Shutting down core"))
         {
            return LogLineType.WorkUnitShuttingDownCore;
         }
         if (logLine.Contains("] Folding@home Core Shutdown:"))
         {
            return LogLineType.WorkUnitCoreShutdown;
         }
         if (logLine.Contains("] + Number of Units Completed:"))
         {
            return LogLineType.ClientNumberOfUnitsCompleted;
         }
         if (logLine.Contains("] Client-core communications error:"))
         {
            return LogLineType.ClientCoreCommunicationsError;
         }
         if (logLine.Contains("] This is a sign of more serious problems, shutting down."))
         {
            //TODO: No unit test coverage - need test log that contains this string
            return LogLineType.ClientCoreCommunicationsErrorShutdown;
         }
         if (logLine.Contains("] EUE limit exceeded. Pausing 24 hours."))
         {
            return LogLineType.ClientEuePauseState;
         }
         if (logLine.Contains("Folding@Home will go to sleep for 1 day"))
         {
            return LogLineType.ClientEuePauseState;
         }
         if (logLine.Contains("Folding@Home Client Shutdown"))
         {
            return LogLineType.ClientShutdown;
         }
         
         return LogLineType.Unknown;
      }
   }
}
