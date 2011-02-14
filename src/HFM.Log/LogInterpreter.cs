/*
 * HFM.NET - Log Interpreter Class
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
using System.Linq;
using System.Collections.Generic;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   public class LogInterpreter
   {
      private readonly List<LogLine> _logLineList;
      private readonly List<ClientRun> _clientRunList;

      public LogInterpreter(List<LogLine> logLines, List<ClientRun> clientRuns)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");
         if (clientRuns == null) throw new ArgumentNullException("clientRuns");

         _logLineList = logLines;
         _clientRunList = clientRuns;
      }
      
      /// <summary>
      /// Returns any data parsing error messages in the log lines.
      /// </summary>
      public IEnumerable<string> LogLineParsingErrors
      {
         get
         {
            return (from x in _logLineList
                    where x.LineType.Equals(LogLineType.Error) && x.LineData is Exception
                    select x.LineData).Cast<Exception>().Select(x => x.Message);

         }
      }

      /// <summary>
      /// Returns the most recent Client Run if available, otherwise null.
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get { return _clientRunList.Count > 0 ? _clientRunList[_clientRunList.Count - 1] : null; }
      }

      /// <summary>
      /// Returns log text of the current client run.
      /// </summary>
      public IList<LogLine> CurrentClientRunLogLines
      {
         get
         {
            ClientRun lastClientRun = CurrentClientRun;
            if (lastClientRun != null)
            {
               int start = lastClientRun.ClientStartIndex;
               int end = _logLineList.Count;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }

      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<LogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitIndexes.Count > 1)
            {
               int start = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 2].StartIndex;
               int end = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 1].StartIndex;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }

      /// <summary>
      /// Returns log text of the current work unit.
      /// </summary>
      public IList<LogLine> CurrentWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitIndexes.Count > 0)
            {
               int start = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 1].StartIndex;
               int end = _logLineList.Count;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }

      #region Methods

      /// <summary>
      /// Get a list of Log Lines that correspond to the given Queue Index.
      /// </summary>
      /// <param name="queueIndex">The Queue Index (0-9)</param>
      public IList<LogLine> GetLogLinesForQueueIndex(int queueIndex)
      {
         // walk backwards through the ClientRunList and then backward
         // through the UnitQueueIndex list.  Find the first (really last
         // because we're itterating in reverse) UnitQueueIndex that matches
         // the given queueIndex.
         for (int i = _clientRunList.Count - 1; i >= 0; i--)
         {
            for (int j = _clientRunList[i].UnitIndexes.Count - 1; j >= 0; j--)
            {
               // if a match is found
               if (_clientRunList[i].UnitIndexes[j].QueueIndex == queueIndex)
               {
                  // set the unit start position
                  int start = _clientRunList[i].UnitIndexes[j].StartIndex;
                  int end = DetermineEndPosition(i, j);

                  int length = end - start;

                  var logLines = new LogLine[length];

                  _logLineList.CopyTo(start, logLines, 0, length);

                  return logLines;
               }
            }
         }

         return null;
      }

      /// <summary>
      /// Determine the ending index of the Work Unit Log Lines.
      /// </summary>
      private int DetermineEndPosition(int i, int j)
      {
         // we're working on the last client run
         if (i == _clientRunList.Count - 1)
         {
            // we're workin on the last unit in the run
            if (j == _clientRunList[i].UnitIndexes.Count - 1)
            {
               // use the last line index as the end position
               return _logLineList.Count;
            }
            else // we're working on a unit prior to the last
            {
               // use the unit start position for the next unit
               return _clientRunList[i].UnitIndexes[j + 1].StartIndex;
            }
         }
         else // we're working on a client run prior to the last
         {
            // we're workin on the last unit in the run
            if (j == _clientRunList[i].UnitIndexes.Count - 1)
            {
               // use the client start position for the next client run
               return _clientRunList[i + 1].ClientStartIndex;
            }
            else
            {
               // use the unit start position for the next unit
               return _clientRunList[i].UnitIndexes[j + 1].StartIndex;
            }
         }
      }

      #endregion
   }
}
