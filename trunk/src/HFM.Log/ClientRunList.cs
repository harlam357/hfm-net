/*
 * HFM.NET - Client Run List Class
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

using System.Collections.Generic;
using System.Diagnostics;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   internal sealed class ClientRunList : ClientRunListBase
   {
      #region Fields

      private readonly Dictionary<int, UnitIndexData> _unitIndexData;

      #endregion

      #region Constructor

      public ClientRunList()
      {
         _unitIndexData = new Dictionary<int, UnitIndexData>();
      }

      #endregion

      #region Methods

      internal override void Build(IList<LogLine> logLines)
      {
         base.Build(logLines);

         // anything left is an active work unit
         foreach (var unitIndex in _unitIndexData.Values)
         {
            if (CurrentClientRun == null)
            {
               Add(new ClientRun(unitIndex.WorkingIndex));
            }

            Debug.Assert(CurrentClientRun != null);

            CurrentClientRun.UnitIndexes.Add(new UnitIndex(unitIndex.QueueIndex, unitIndex.WorkingIndex));
         }

         // remove all
         _unitIndexData.Clear();
      }

      protected override void HandleWorkUnitWorking(LogLine logLine)
      {
         var queueIndex = (int)logLine.LineData;
         _unitIndexData[queueIndex] = new UnitIndexData { QueueIndex = queueIndex, WorkingIndex = logLine.LineIndex };
      }

      protected override void HandleWorkUnitCoreReturn(LogLine logLine)
      {
         AddWorkUnitResult(((UnitResult)logLine.LineData).Value);
      }

      protected override void HandleWorkUnitCleaningUp(LogLine logLine)
      {
         base.HandleWorkUnitCleaningUp(logLine);

         var queueIndex = (int)logLine.LineData;
         if (_unitIndexData.ContainsKey(queueIndex))
         {
            if (CurrentClientRun == null)
            {
               Add(new ClientRun(logLine.LineIndex));
            }

            Debug.Assert(CurrentClientRun != null);

            CurrentClientRun.UnitIndexes.Add(new UnitIndex(queueIndex, _unitIndexData[queueIndex].WorkingIndex, logLine.LineIndex));

            // remove from dictionary
            _unitIndexData.Remove(queueIndex);
         }
      }

      #endregion

      private class UnitIndexData
      {
         public int QueueIndex { get; set; }
         public int WorkingIndex { get; set; }
      }
   }
}
