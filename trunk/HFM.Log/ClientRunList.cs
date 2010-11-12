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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   internal class ClientRunList : List<ClientRun>
   {
      #region Fields
      
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
      /// Returns the most recent Client Run if available, otherwise null.
      /// </summary>
      private ClientRun LastClientRun
      {
         get { return Count > 0 ? this[Count - 1] : null; }
      } 
      
      #endregion

      #region Methods
      
      internal void Build(IList<LogLine> logLines)
      {
         // clear before building
         Clear();
         // init unit start container
         _unitStart.Initialize();
      
         // LogLine contains the LineType, so we'll scan the List of LogLine
         // and set Client and Unit Start Indexes.
         foreach (var line in logLines)
         {
            HandleLogLine(line);
         }

         DoRunLevelDetection(logLines);
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
         if (LastClientRun == null)
         {
            Add(new ClientRun(line.LineIndex));
         }

         Debug.Assert(LastClientRun != null);
         
         // If we've already seen a WorkUnitRunning line, ignore this one.);
         if (_currentLineType.Equals(LogLineType.WorkUnitRunning)) return;

         // Not Checking the Queue Slot - we don't care if we found a valid slot or not
         if (_unitStart.WorkUnitProcessingIndex > -1)
         {
            LastClientRun.UnitIndexes.Add(new UnitIndex(_unitStart.WorkUnitQueueSlotIndex, _unitStart.WorkUnitProcessingIndex));
         }
         else if (_unitStart.WorkUnitWorkingIndex > -1)
         {
            LastClientRun.UnitIndexes.Add(new UnitIndex(_unitStart.WorkUnitQueueSlotIndex, _unitStart.WorkUnitWorkingIndex));
         }
         else if (_unitStart.WorkUnitStartIndex > -1)
         {
            LastClientRun.UnitIndexes.Add(new UnitIndex(_unitStart.WorkUnitQueueSlotIndex, _unitStart.WorkUnitStartIndex));
         }
         else
         {
            LastClientRun.UnitIndexes.Add(new UnitIndex(_unitStart.WorkUnitQueueSlotIndex, line.LineIndex));
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
            if (LastClientRun != null)
            {
               if (logLine.LineData.Equals(WorkUnitResult.FinishedUnit))
               {
                  LastClientRun.CompletedUnits++;
               }
               else if (logLine.LineData.Equals(WorkUnitResult.EarlyUnitEnd) ||
                        logLine.LineData.Equals(WorkUnitResult.UnstableMachine) ||
                        logLine.LineData.Equals(WorkUnitResult.Interrupted) ||
                        logLine.LineData.Equals(WorkUnitResult.BadWorkUnit) ||
                        logLine.LineData.Equals(WorkUnitResult.CoreOutdated)) 
               {
                  LastClientRun.FailedUnits++;
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
         if (LastClientRun != null)
         {
            LastClientRun.TotalCompletedUnits = (int)logLine.LineData;
         }
      }

      private void DoRunLevelDetection(IList<LogLine> logLines)
      {
         for (int i = 0; i < Count; i++)
         {
            int end;
            // we're working on the last client run
            if (i == Count - 1)
            {
               // use the last line index as the end position
               end = logLines.Count;
            }
            else // we're working on a client run prior to the last
            {
               // use the client start position for the next client run
               end = this[i + 1].ClientStartIndex;
            }

            for (int j = this[i].ClientStartIndex; j < end; j++)
            {
               if (logLines[j].LineType.Equals(LogLineType.ClientVersion))
               {
                  this[i].ClientVersion = logLines[j].LineData.ToString();
               }
               else if (logLines[j].LineType.Equals(LogLineType.ClientArguments))
               {
                  this[i].Arguments = logLines[j].LineData.ToString();
               }
               else if (logLines[j].LineType.Equals(LogLineType.ClientUserNameTeam))
               {
                  var userAndTeam = (ArrayList)logLines[j].LineData;
                  this[i].FoldingID = userAndTeam[0].ToString();
                  this[i].Team = (int)userAndTeam[1];
               }
               else if (logLines[j].LineType.Equals(LogLineType.ClientUserID) ||
                        logLines[j].LineType.Equals(LogLineType.ClientReceivedUserID))
               {
                  this[i].UserID = logLines[j].LineData.ToString();
               }
               else if (logLines[j].LineType.Equals(LogLineType.ClientMachineID))
               {
                  this[i].MachineID = (int)logLines[j].LineData;
               }
            }
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
