/*
 * HFM.NET - Legacy Log Interpreter Class
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

using System.Collections.Generic;
using System.Linq;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public sealed class LogInterpreterLegacy : LogInterpreterBase
   {
      #region Constructor

      public LogInterpreterLegacy(IList<LogLine> logLines, IList<ClientRun> clientRuns)
         : base(logLines, clientRuns)
      {
         
      }

      #endregion

      #region Properties

      /// <summary>
      /// Returns log text of the current client run.
      /// </summary>
      public IList<LogLine> CurrentClientRunLogLines
      {
         get
         {
            if (CurrentClientRun != null)
            {
               int start = CurrentClientRun.ClientStartIndex;
               int end = LogLineList.Count - 1;
               return LogLineList.WhereLineIndex(start, end).ToList().AsReadOnly();
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
            if (CurrentClientRun != null && CurrentClientRun.UnitIndexes.Count > 1)
            {
               int start = CurrentClientRun.UnitIndexes[CurrentClientRun.UnitIndexes.Count - 2].StartIndex;
               int end = CurrentClientRun.UnitIndexes[CurrentClientRun.UnitIndexes.Count - 1].StartIndex - 1;
               return LogLineList.WhereLineIndex(start, end).ToList().AsReadOnly();
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
            if (CurrentClientRun != null && CurrentClientRun.UnitIndexes.Count > 0)
            {
               int start = CurrentClientRun.UnitIndexes[CurrentClientRun.UnitIndexes.Count - 1].StartIndex;
               int end = LogLineList.Count - 1;
               return LogLineList.WhereLineIndex(start, end).ToList().AsReadOnly();
            }

            return null;
         }
      }
      
      #endregion

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
         for (int i = ClientRunList.Count - 1; i >= 0; i--)
         {
            for (int j = ClientRunList[i].UnitIndexes.Count - 1; j >= 0; j--)
            {
               // if a match is found
               if (ClientRunList[i].UnitIndexes[j].QueueIndex == queueIndex)
               {
                  int start = ClientRunList[i].UnitIndexes[j].StartIndex;
                  int end = DetermineEndPosition(i, j) - 1;
                  return LogLineList.WhereLineIndex(start, end).ToList().AsReadOnly();
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
         if (i == ClientRunList.Count - 1)
         {
            // we're workin on the last unit in the run
            if (j == ClientRunList[i].UnitIndexes.Count - 1)
            {
               // use the last line index as the end position
               return LogLineList.Count;
            }

            // use the unit start position for the next unit
            return ClientRunList[i].UnitIndexes[j + 1].StartIndex;
         }

         // we're workin on the last unit in the run
         if (j == ClientRunList[i].UnitIndexes.Count - 1)
         {
            // use the client start position for the next client run
            return ClientRunList[i + 1].ClientStartIndex;
         }

         // use the unit start position for the next unit
         return ClientRunList[i].UnitIndexes[j + 1].StartIndex;
      }

      #endregion
   }
}
