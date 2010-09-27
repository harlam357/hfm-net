/*
 * HFM.NET - Client Run List Class
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
using System.Globalization;

using HFM.Framework;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   public class ClientRunList : List<ClientRun>
   {
      #region Members
      /// <summary>
      /// Local variable containing the current LogLineType.
      /// </summary>
      private LogLineType _currentLineType = LogLineType.Unknown;

      /// <summary>
      /// Holds starting information for the current work unit.
      /// </summary>
      private UnitStartContainer _unitStart; 
      #endregion

      #region Properties
      /// <summary>
      /// Returns the most recent client run if available, otherwise null.
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get
         {
            if (Count > 0)
            {
               return this[Count - 1];
            }

            return null;
         }
      } 
      #endregion

      #region Methods
      public void HandleLogLines(IList<Framework.LogLine> logLines)
      {
         _unitStart.Initialize();
      
         // Now that we know the LineType for each log line, scan the List of LogLine
         // and set Client and Unit Start Indexes.
         foreach (ILogLine line in logLines)
         {
            HandleLogLine(line);
         }
      }
      
      /// <summary>
      /// Handles the given LogLineType and sets the correct position value.
      /// </summary>
      private void HandleLogLine(ILogLine line)
      {
         switch (line.LineType)
         {
            case LogLineType.LogOpen:
               HandleLogOpen(line.LineIndex);
               _currentLineType = line.LineType;
               break;
            case LogLineType.LogHeader:
               HandleLogHeader(line.LineIndex);
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitProcessing:
               HandleWorkUnitProcessing(line.LineIndex);
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitCoreDownload:
               HandleWorkUnitCoreDownload(line.LineIndex);
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitIndex:
            case LogLineType.WorkUnitQueueIndex:
               HandleWorkUnitQueueIndex((int)line.LineData);
               break;
            case LogLineType.WorkUnitWorking:
               HandleWorkUnitWorking(line.LineIndex);
               break;
            case LogLineType.WorkUnitStart:
               HandleWorkUnitStart(line.LineIndex);
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitRunning:
               HandleWorkUnitRunning(line);
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitPaused:
               _currentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitCoreShutdown:
               HandleWorkUnitCoreShutdown(line);
               _currentLineType = line.LineType;
               break;
            case LogLineType.ClientNumberOfUnitsCompleted:
               HandleClientNumberOfUnitsCompleted(line);
               break;
         }
      }

      /// <summary>
      /// Handles LogOpen LogLineType.
      /// </summary>
      /// <param name="lineIndex">Index of the given LogLineType.</param>
      private void HandleLogOpen(int lineIndex)
      {
         // Add a new ClientRun on LogOpen
         Add(new ClientRun(lineIndex));
      }

      /// <summary>
      /// Handles LogHeader LogLineType.
      /// </summary>
      /// <param name="lineIndex">Index of the given LogLineType.</param>
      private void HandleLogHeader(int lineIndex)
      {
         // If the last line observed was a LogOpen or a LogHeader, return
         // and don't use this as a signal to add a new ClientRun.
         if (_currentLineType.Equals(LogLineType.LogOpen) ||
             _currentLineType.Equals(LogLineType.LogHeader)) return;

         // Otherwise, if we see a LogHeader and the preceeding line was not
         // a LogOpen or a LogHeader, then we use this as a signal to create
         // a new ClientRun.  This is a backup option and I don't expect this
         // situtation to happen at all if the log file is not corrupt.
         Add(new ClientRun(lineIndex));
      }

      /// <summary>
      /// Handles WorkUnitProcessing LogLineType.
      /// </summary>
      /// <param name="lineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitProcessing(int lineIndex)
      {
         // If we have not found a ProcessingIndex (== -1) then set it.
         // Othwerwise, if ProcessingIndex (!= -1) and a CoreDownloadIndex
         // has been observerd and is greater than the current ProcessingIndex,
         // then update the ProcessingIndex to bypass the CoreDownload section
         // of the log file.
         if (_unitStart.WorkUnitProcessingIndex == -1 ||
             (_unitStart.WorkUnitProcessingIndex != -1 &&
              _unitStart.WorkUnitCoreDownloadIndex > _unitStart.WorkUnitProcessingIndex))
         {
            _unitStart.WorkUnitProcessingIndex = lineIndex;
         }
      }

      /// <summary>
      /// Handles WorkUnitCoreDownload LogLineType.
      /// </summary>
      private void HandleWorkUnitCoreDownload(int lineIndex)
      {
         _unitStart.WorkUnitCoreDownloadIndex = lineIndex;
      }

      /// <summary>
      /// Handles WorkUnitQueueIndex LogLineType.
      /// </summary>
      private void HandleWorkUnitQueueIndex(int queueIndex)
      {
         _unitStart.WorkUnitQueueSlotIndex = queueIndex;
      }

      /// <summary>
      /// Handles WorkUnitWorking LogLineType.
      /// </summary>
      /// <param name="lineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitWorking(int lineIndex)
      {
         // This first check allows us to overlook the "+ Working ..." message
         // that gets written after a client is Paused.  We don't want to key
         // work unit positions based on this log entry.
         if (_currentLineType.Equals(LogLineType.WorkUnitPaused))
         {
            // Return to a Running state
            _currentLineType = LogLineType.WorkUnitRunning;
         }
         else
         {
            _unitStart.WorkUnitWorkingIndex = lineIndex;
            _currentLineType = LogLineType.WorkUnitWorking;
         }
      }

      /// <summary>
      /// Handles WorkUnitStart LogLineType.
      /// </summary>
      /// <param name="lineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitStart(int lineIndex)
      {
         _unitStart.WorkUnitStartIndex = lineIndex;
      }

      /// <summary>
      /// Handles WorkUnitRunning LogLineType.
      /// </summary>
      /// <remarks></remarks>
      private void HandleWorkUnitRunning(ILogLine line)
      {
         if (CurrentClientRun == null)
         {
            // no client run to attach this unit start
            throw new InvalidOperationException("Found Work Unit Data before any Log Headers.");
         }
         
         // If we've already seen a WorkUnitRunning line, ignore this one.
         if (_currentLineType.Equals(LogLineType.WorkUnitRunning)) return;

         if (_unitStart.WorkUnitProcessingIndex > -1)
         {
            CurrentClientRun.UnitStartIndex.Add(_unitStart.WorkUnitProcessingIndex);
            // Set the Queue Slot - we don't care if we found a valid slot or not
            CurrentClientRun.UnitQueueIndex.Add(_unitStart.WorkUnitQueueSlotIndex);
         }
         else if (_unitStart.WorkUnitWorkingIndex > -1)
         {
            CurrentClientRun.UnitStartIndex.Add(_unitStart.WorkUnitWorkingIndex);
            // Set the Queue Slot - we don't care if we found a valid slot or not
            CurrentClientRun.UnitQueueIndex.Add(_unitStart.WorkUnitQueueSlotIndex);
         }
         else if (_unitStart.WorkUnitStartIndex > -1)
         {
            CurrentClientRun.UnitStartIndex.Add(_unitStart.WorkUnitStartIndex);
            // Set the Queue Slot - we don't care if we found a valid slot or not
            CurrentClientRun.UnitQueueIndex.Add(_unitStart.WorkUnitQueueSlotIndex);
         }
         else
         {
            // No Unit Start Index.  This log section looks to be corrupted.
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format(CultureInfo.CurrentCulture,
               "Could not find a Unit Start Index.  The log section around line {0} appears to be corrupted.", line.LineIndex + 1));
         }

         // Re-initialize Values
         _unitStart.Initialize();
      }

      /// <summary>
      /// Handles WorkUnitCoreShutdown LogLineType.
      /// </summary>
      /// <param name="logLine">The given LogLine object.</param>
      private void HandleWorkUnitCoreShutdown(ILogLine logLine)
      {
         if (_currentLineType.Equals(LogLineType.WorkUnitRunning))
         {
            if (CurrentClientRun != null)
            {
               if (logLine.LineData.Equals(WorkUnitResult.FinishedUnit))
               {
                  CurrentClientRun.NumberOfCompletedUnits++;
               }
               else if (logLine.LineData.Equals(WorkUnitResult.EarlyUnitEnd) ||
                        logLine.LineData.Equals(WorkUnitResult.UnstableMachine) ||
                        logLine.LineData.Equals(WorkUnitResult.Interrupted) ||
                        logLine.LineData.Equals(WorkUnitResult.BadWorkUnit) ||
                        logLine.LineData.Equals(WorkUnitResult.CoreOutdated)) 
               {
                  CurrentClientRun.NumberOfFailedUnits++;
               }
            }
         }
      }

      /// <summary>
      /// Handles ClientNumberOfUnitsCompleted LogLineType.
      /// </summary>
      /// <param name="logLine">The given LogLine object.</param>
      private void HandleClientNumberOfUnitsCompleted(ILogLine logLine)
      {
         if (CurrentClientRun != null)
         {
            CurrentClientRun.NumberOfTotalUnitsCompleted = (int)logLine.LineData;
         }
      } 
      #endregion
   }

   /// <summary>
   /// Container for captured Unit Start Indexes
   /// </summary>
   internal struct UnitStartContainer
   {
      #region Members
      internal int WorkUnitProcessingIndex;
      internal int WorkUnitCoreDownloadIndex;
      internal int WorkUnitQueueSlotIndex;
      internal int WorkUnitWorkingIndex;
      internal int WorkUnitStartIndex;
      #endregion
      
      #region Methods
      internal void Initialize()
      {
         WorkUnitProcessingIndex = -1;
         WorkUnitCoreDownloadIndex = -1;
         WorkUnitQueueSlotIndex = -1;
         WorkUnitWorkingIndex = -1;
         WorkUnitStartIndex = -1;
      }
      #endregion
   }
}
