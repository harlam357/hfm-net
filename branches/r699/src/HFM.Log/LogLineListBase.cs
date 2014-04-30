/*
 * HFM.NET - Log Line List Base Class
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

using HFM.Core.DataTypes;

namespace HFM.Log
{
   internal abstract class LogLineListBase : LinkedList<LogLine>
   {
      private readonly LogLineParserBase _logLineParser;

      protected LogLineListBase(LogFileType logFileType)
      {
         _logLineParser = logFileType.GetLogLineParser();
      }

      /// <summary>
      /// Adds the elements of the specified list to the end of the List.
      /// </summary>
      /// <param name="logLines">List of raw log lines.</param>
      internal void AddRange(IEnumerable<string> logLines)
      {
         // Scan all raw lines and create a LogLine object for each denoting its
         // LogLineType and any LineData parsed from the raw line.
         foreach (string line in logLines)
         {
            Add(line);
         }
      }

      /// <summary>
      /// Adds a LogLine to the end of the List.
      /// </summary>
      /// <param name="logLine">The raw log line being added.</param>
      internal void Add(string logLine)
      {
         LogLineType lineType = DetermineLineType(logLine);
         var logLineObject = new LogLine { LineType = lineType, LineIndex = Count, LineRaw = logLine };
         try
         {
            object lineData = _logLineParser.GetLineData(logLineObject);
            if (lineData is LogLineError)
            {
               logLineObject.LineType = LogLineType.Error;
               logLineObject.LineData = ((LogLineError)lineData).Message;
            }
            else
            {
               logLineObject.LineData = lineData;
            }
         }
         catch (Exception ex)
         {
            logLineObject.LineType = LogLineType.Error;
            logLineObject.LineData = ex.Message;
         }
         AddLast(logLineObject);
      }

      protected virtual LogLineType DetermineLineType(string logLine)
      {
         return IsLineTypeWorkUnitRunning(logLine) ? LogLineType.WorkUnitRunning : LogLineType.Unknown;
      }

      /// <summary>
      /// Determine if the line type is LogLineType.WorkUnitRunning.
      /// </summary>
      /// <param name="logLine">The raw log line being inspected.</param>
      protected static bool IsLineTypeWorkUnitRunning(string logLine)
      {
         // Change for v7: Removed the leading "] " portion of the
         // string for all but the ProtoMol specific conditions.

         if (logLine.Contains("Preparing to commence simulation"))
         {
            return true;
         }
         if (logLine.Contains("Called DecompressByteArray"))
         {
            return true;
         }
         if (logLine.Contains("- Digital signature verified"))
         {
            return true;
         }
         /*** ProtoMol Only */
         if (logLine.Contains("] Digital signatures verified"))
         {
            return true;
         }
         /*******************/
         if (logLine.Contains("Entering M.D."))
         {
            return true;
         }

         return false;
      }
   }
}
