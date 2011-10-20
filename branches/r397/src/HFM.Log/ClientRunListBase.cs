/*
 * HFM.NET - Client Run List Base Class
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

using System.Collections;
using System.Collections.Generic;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   internal abstract class ClientRunListBase : List<ClientRun>
   {
      #region Properties

      /// <summary>
      /// Returns the most recent Client Run if available, otherwise null.
      /// </summary>
      protected ClientRun CurrentClientRun
      {
         get { return Count > 0 ? this[Count - 1] : null; }
      }

      #endregion

      #region Methods

      internal virtual void Build(IList<LogLine> logLines)
      {
         // clear before building
         Clear();

         // LogLine contains the LineType, so we'll scan the List of LogLine
         // and set Client and Unit Start Indexes.
         foreach (var line in logLines)
         {
            HandleLogLine(line);
         }
      }

      /// <summary>
      /// Handles the given LogLineType and sets the correct position value.
      /// </summary>
      private void HandleLogLine(ILogLine logLine)
      {
         switch (logLine.LineType)
         {
            case LogLineType.LogOpen:
               HandleLogOpen(logLine);
               break;
            case LogLineType.LogHeader:
               HandleLogHeader(logLine);
               break;
            case LogLineType.WorkUnitProcessing:
               HandleWorkUnitProcessing(logLine);
               break;
            case LogLineType.WorkUnitCoreDownload:
               HandleWorkUnitCoreDownload(logLine);
               break;
            case LogLineType.WorkUnitIndex:
            case LogLineType.WorkUnitQueueIndex:
               HandleWorkUnitQueueIndex(logLine);
               break;
            case LogLineType.WorkUnitWorking:
               HandleWorkUnitWorking(logLine);
               break;
            case LogLineType.WorkUnitStart:
               HandleWorkUnitStart(logLine);
               break;
            case LogLineType.WorkUnitRunning:
               HandleWorkUnitRunning(logLine);
               break;
            case LogLineType.WorkUnitPaused:
               HandleWorkUnitPaused(logLine);
               break;
            case LogLineType.WorkUnitCoreShutdown:
            case LogLineType.ClientCoreCommunicationsError:
               HandleWorkUnitCoreShutdown(logLine);
               break;
            case LogLineType.ClientNumberOfUnitsCompleted:
               HandleClientNumberOfUnitsCompleted(logLine);
               break;
            case LogLineType.WorkUnitCleaningUp:
               HandleWorkUnitCleaningUp(logLine);
               break;
         }
      }

      protected virtual void HandleLogOpen(ILogLine logLine)
      {
         // Add a new ClientRun on LogOpen
         Add(new ClientRun(logLine.LineIndex));
      }

      protected virtual void HandleLogHeader(ILogLine logLine)
      {
         
      }
      
      protected virtual void HandleWorkUnitProcessing(ILogLine logLine)
      {
         
      }

      protected virtual void HandleWorkUnitCoreDownload(ILogLine logLine)
      {
         
      }

      protected virtual void HandleWorkUnitQueueIndex(ILogLine logLine)
      {
         
      }

      protected abstract void HandleWorkUnitWorking(ILogLine logLine);

      protected virtual void HandleWorkUnitStart(ILogLine logLine)
      {
         
      }

      protected virtual void HandleWorkUnitRunning(ILogLine logLine)
      {
         
      }

      protected virtual void HandleWorkUnitPaused(ILogLine logLine)
      {
         
      }

      protected virtual void HandleWorkUnitCoreShutdown(ILogLine logLine)
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

      private void HandleClientNumberOfUnitsCompleted(ILogLine logLine)
      {
         if (CurrentClientRun != null)
         {
            // really legacy specific but can live here
            CurrentClientRun.TotalCompletedUnits = (int)logLine.LineData;
         }
      }

      protected virtual void HandleWorkUnitCleaningUp(ILogLine logLine)
      {
         
      }
      
      #endregion
   }
}
