/*
 * HFM.NET - Log Line List Class
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

using System.Text.RegularExpressions;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Log Lines.
   /// </summary>
   internal sealed class LogLineList : LogLineListBase
   {
      public LogLineList(LogFileType logFileType)
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

         if (logLine.Contains("************************* Folding@home Client"))
         {
            return LogLineType.LogOpen;
         }
         if (logLine.Contains(":Sending unit results:"))
         {
            return LogLineType.ClientSendWorkToServer;
         }
         if (logLine.Contains(": Uploading"))
         {
            return LogLineType.ClientSendStart;
         }
         if (logLine.Contains(": Upload complete"))
         {
            return LogLineType.ClientSendComplete;
         }
         if (logLine.Contains(":Requesting new work unit for slot"))
         {
            return LogLineType.ClientAttemptGetWorkPacket;
         }
         if (logLine.Contains(":Starting Unit"))
         {
            return LogLineType.WorkUnitWorking;
         }
         if (logLine.Contains(":*------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
         if (logLine.Contains(":Version"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         if (logLine.Contains(":Project:"))
         {
            return LogLineType.WorkUnitProject;
         }
         if (logLine.Contains(":Completed "))
         {
            return LogLineType.WorkUnitFrame;
         }
         if (logLine.Contains(":- Shutting down core"))
         {
            return LogLineType.WorkUnitShuttingDownCore;
         }
         if (logLine.Contains(":Folding@home Core Shutdown:"))
         {
            return LogLineType.WorkUnitCoreShutdown;
         }
         if (Regex.IsMatch(logLine, "FahCore, running Unit \\d{2}, returned: "))
         {
            return LogLineType.WorkUnitCoreReturn;
         }
         if (logLine.Contains(":Cleaning up Unit"))
         {
            return LogLineType.WorkUnitCleaningUp;
         }

         return LogLineType.Unknown;
      }
   }
}
