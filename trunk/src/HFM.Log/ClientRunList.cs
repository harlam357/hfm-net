/*
 * HFM.NET - Client Run List Class
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
using System.Diagnostics;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   internal sealed class ClientRunList : ClientRunListBase
   {
      #region Fields

      private readonly Dictionary<int, UnitIndexes> _unitIndexes;

      #endregion

      #region Constructor

      public ClientRunList()
      {
         _unitIndexes = new Dictionary<int, UnitIndexes>();
      }

      #endregion

      #region Methods

      internal override void Build(IList<LogLine> logLines)
      {
         base.Build(logLines);

         // anything left is an active work unit
         foreach (var unitIndex in _unitIndexes.Values)
         {
            if (CurrentClientRun == null)
            {
               Add(new ClientRun(unitIndex.WorkingIndex));
            }

            Debug.Assert(CurrentClientRun != null);

            CurrentClientRun.UnitIndexes.Add(new UnitIndex(unitIndex.QueueIndex, unitIndex.WorkingIndex));
         }

         // remove all
         _unitIndexes.Clear();
      }

      protected override void HandleWorkUnitWorking(ILogLine logLine)
      {
         var queueIndex = (int)logLine.LineData;
         _unitIndexes[queueIndex] = new UnitIndexes { QueueIndex = queueIndex, WorkingIndex = logLine.LineIndex };
      }

      protected override void HandleWorkUnitCoreShutdown(ILogLine logLine)
      {
         if (CurrentClientRun != null)
         {
            if (logLine.LineData.Equals(WorkUnitResult.FinishedUnit))
            {
               CurrentClientRun.CompletedUnits++;
            }
            else if (logLine.LineData.Equals(WorkUnitResult.EarlyUnitEnd) ||
                     logLine.LineData.Equals(WorkUnitResult.UnstableMachine) ||
                     logLine.LineData.Equals(WorkUnitResult.Interrupted) ||
                     logLine.LineData.Equals(WorkUnitResult.BadWorkUnit) ||
                     logLine.LineData.Equals(WorkUnitResult.CoreOutdated) ||
                     logLine.LineData.Equals(WorkUnitResult.ClientCoreError))
            {
               CurrentClientRun.FailedUnits++;
            }
         }
      }

      protected override void HandleWorkUnitCleaningUp(ILogLine logLine)
      {
         base.HandleWorkUnitCleaningUp(logLine);

         var queueIndex = (int)logLine.LineData;
         if (_unitIndexes.ContainsKey(queueIndex))
         {
            if (CurrentClientRun == null)
            {
               Add(new ClientRun(logLine.LineIndex));
            }

            Debug.Assert(CurrentClientRun != null);

            CurrentClientRun.UnitIndexes.Add(new UnitIndex(queueIndex, _unitIndexes[queueIndex].WorkingIndex, logLine.LineIndex));

            // remove from dictionary
            _unitIndexes.Remove(queueIndex);
         }
      }

      #endregion

      private class UnitIndexes
      {
         public int QueueIndex { get; set; }
         public int WorkingIndex { get; set; }
      }
   }
}
