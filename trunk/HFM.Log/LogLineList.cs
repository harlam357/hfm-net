/*
 * HFM.NET - Log Line List Class
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

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Log Lines.
   /// </summary>
   public class LogLineList : List<LogLine>
   {
      public void HandleLogLine(int index, string logLine)
      {
         LogLineType lineType = DetermineLineType(logLine);
         var logLineObject = new LogLine { LineType = lineType, LineIndex = index, LineRaw = logLine };
         try
         {
            logLineObject.LineData = LogLineParser.GetLineData(logLineObject);
         }
         catch (Exception ex)
         {
            logLineObject.LineType = LogLineType.Unknown;
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, ex);
         }
         Add(logLineObject);
      }

      /// <summary>
      /// Inspect the given log line and determine the line type.
      /// </summary>
      /// <param name="logLine">The log line being inspected.</param>
      private static LogLineType DetermineLineType(string logLine)
      {
         if (logLine.Contains("--- Opening Log file"))
         {
            return LogLineType.LogOpen;
         }
         else if (logLine.Contains("###############################################################################"))
         {
            return LogLineType.LogHeader;
         }
         else if (logLine.Contains("Folding@Home Client Version"))
         {
            return LogLineType.ClientVersion;
         }
         else if (logLine.Contains("] Sending work to server"))
         {
            return LogLineType.ClientSendWorkToServer;
         }
         else if (logLine.Contains("] - Autosending finished units..."))
         {
            return LogLineType.ClientAutosendStart;
         }
         else if (logLine.Contains("] - Autosend completed"))
         {
            return LogLineType.ClientAutosendComplete;
         }
         else if (logLine.Contains("] + Attempting to send results"))
         {
            return LogLineType.ClientSendStart;
         }
         else if (logLine.Contains("] + Could not connect to Work Server"))
         {
            return LogLineType.ClientSendConnectFailed;
         }
         else if (logLine.Contains("] - Error: Could not transmit unit"))
         {
            return LogLineType.ClientSendFailed;
         }
         else if (logLine.Contains("] + Results successfully sent"))
         {
            return LogLineType.ClientSendComplete;
         }
         else if (logLine.Contains("Arguments:"))
         {
            return LogLineType.ClientArguments;
         }
         else if (logLine.Contains("] - User name:"))
         {
            return LogLineType.ClientUserNameTeam;
         }
         else if (logLine.Contains("] + Requesting User ID from server"))
         {
            return LogLineType.ClientRequestingUserID;
         }
         else if (logLine.Contains("- Received User ID ="))
         {
            return LogLineType.ClientReceivedUserID;
         }
         else if (logLine.Contains("] - User ID:"))
         {
            return LogLineType.ClientUserID;
         }
         else if (logLine.Contains("] - Machine ID:"))
         {
            return LogLineType.ClientMachineID;
         }
         else if (logLine.Contains("] + Attempting to get work packet"))
         {
            return LogLineType.ClientAttemptGetWorkPacket;
         }
         else if (logLine.Contains("] - Will indicate memory of"))
         {
            return LogLineType.ClientIndicateMemory;
         }
         else if (logLine.Contains("] - Detect CPU. Vendor:"))
         {
            return LogLineType.ClientDetectCpu;
         }
         else if (logLine.Contains("] + Processing work unit"))
         {
            return LogLineType.WorkUnitProcessing;
         }
         else if (logLine.Contains("] + Downloading new core"))
         {
            return LogLineType.WorkUnitCoreDownload;
         }
         else if (logLine.Contains("] Working on Unit 0"))
         {
            return LogLineType.WorkUnitIndex;
         }
         else if (logLine.Contains("] Working on queue slot 0"))
         {
            return LogLineType.WorkUnitQueueIndex;
         }
         else if (logLine.Contains("] + Working ..."))
         {
            return LogLineType.WorkUnitWorking;
         }
         else if (logLine.Contains("] *------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*******************/
         else if (logLine.Contains("] Version"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("]   Version:"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*******************/
         else if (IsLineTypeWorkUnitStarted(logLine))
         {
            return LogLineType.WorkUnitRunning;
         }
         else if (logLine.Contains("] Project:"))
         {
            return LogLineType.WorkUnitProject;
         }
         else if (logLine.Contains("] Completed "))
         {
            return LogLineType.WorkUnitFrame;
         }
         else if (logLine.Contains("] + Paused"))
         {
            return LogLineType.WorkUnitPaused;
         }
         else if (logLine.Contains("] + Running on battery power"))
         {
            return LogLineType.WorkUnitPausedForBattery;
         }
         else if (logLine.Contains("] + Off battery, restarting core"))
         {
            return LogLineType.WorkUnitResumeFromBattery;
         }
         else if (logLine.Contains("] - Shutting down core"))
         {
            return LogLineType.WorkUnitShuttingDownCore;
         }
         else if (logLine.Contains("] Folding@home Core Shutdown:"))
         {
            return LogLineType.WorkUnitCoreShutdown;
         }
         else if (logLine.Contains("] + Number of Units Completed:"))
         {
            return LogLineType.ClientNumberOfUnitsCompleted;
         }
         else if (logLine.Contains("] This is a sign of more serious problems, shutting down."))
         {
            return LogLineType.ClientCoreCommunicationsErrorShutdown;
         }
         else if (logLine.Contains("] EUE limit exceeded. Pausing 24 hours."))
         {
            return LogLineType.ClientEuePauseState;
         }
         else if (logLine.Contains("] Folding@Home will go to sleep for 1 day"))
         {
            return LogLineType.ClientEuePauseState;
         }
         else if (logLine.Contains("Folding@Home Client Shutdown"))
         {
            return LogLineType.ClientShutdown;
         }
         else
         {
            return LogLineType.Unknown;
         }
      }

      /// <summary>
      /// Inspect the given log line and determine if the line type is LogLineType.WorkUnitRunning.
      /// </summary>
      /// <param name="logLine">The log line being inspected.</param>
      private static bool IsLineTypeWorkUnitStarted(string logLine)
      {
         if (logLine.Contains("] Preparing to commence simulation"))
         {
            return true;
         }
         else if (logLine.Contains("] Called DecompressByteArray"))
         {
            return true;
         }
         else if (logLine.Contains("] - Digital signature verified"))
         {
            return true;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("] Digital signatures verified"))
         {
            return true;
         }
         /*******************/
         else if (logLine.Contains("] Entering M.D."))
         {
            return true;
         }

         return false;
      }
   }
}
