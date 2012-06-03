/*
 * HFM.NET - Legacy Client Run List Class
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

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   internal sealed class ClientRunListLegacy : ClientRunListBase
   {
      #region Fields

      /// <summary>
      /// Local variable containing the current LogLineType.
      /// </summary>
      private LogLineType _currentLineType;

      /// <summary>
      /// Holds index information for the current work unit.
      /// </summary>
      private UnitIndexData _unitIndexData;

      #endregion

      #region Constructor

      public ClientRunListLegacy()
      {
         _currentLineType = LogLineType.Unknown;
      }

      #endregion

      #region Methods

      internal override void Build(ICollection<LogLine> logLines)
      {
         // init unit index container
         _unitIndexData.Initialize();

         base.Build(logLines);

         DoRunLevelDetection(logLines);
      }

      protected override void HandleLogOpen(LogLine logLine)
      {
         base.HandleLogOpen(logLine);

         _currentLineType = logLine.LineType;
      }

      protected override void HandleLogHeader(LogLine logLine)
      {
         base.HandleLogHeader(logLine);

         // If the last line observed was a LogOpen or a LogHeader, return
         // and don't use this as a signal to add a new ClientRun.
         if (_currentLineType.Equals(LogLineType.LogOpen) ||
             _currentLineType.Equals(LogLineType.LogHeader)) return;

         // Otherwise, if we see a LogHeader and the preceeding line was not
         // a LogOpen or a LogHeader, then we use this as a signal to create
         // a new ClientRun.  This is a backup option and I don't expect this
         // situtation to happen at all if the log file is not corrupt.
         Add(new ClientRun(logLine.LineIndex));

         _currentLineType = logLine.LineType;
      }

      protected override void HandleWorkUnitProcessing(LogLine logLine)
      {
         base.HandleWorkUnitProcessing(logLine);

         // If we have not found a ProcessingIndex (== -1) then set it.
         // Othwerwise, if ProcessingIndex (!= -1) and a CoreDownloadIndex
         // has been observerd and is greater than the current ProcessingIndex,
         // then update the ProcessingIndex to bypass the CoreDownload section
         // of the log file.
         if (_unitIndexData.ProcessingIndex == -1 ||
            (_unitIndexData.ProcessingIndex != -1 &&
             _unitIndexData.CoreDownloadIndex > _unitIndexData.ProcessingIndex))
         {
            _unitIndexData.ProcessingIndex = logLine.LineIndex;
         }

         _currentLineType = logLine.LineType;
      }

      protected override void HandleWorkUnitCoreDownload(LogLine logLine)
      {
         base.HandleWorkUnitCoreDownload(logLine);

         _unitIndexData.CoreDownloadIndex = logLine.LineIndex;
         _currentLineType = logLine.LineType;
      }

      protected override void HandleWorkUnitQueueIndex(LogLine logLine)
      {
         base.HandleWorkUnitQueueIndex(logLine);

         _unitIndexData.QueueSlotIndex = (int)logLine.LineData;
      }

      protected override void HandleWorkUnitWorking(LogLine logLine)
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
            _unitIndexData.WorkingIndex = logLine.LineIndex;
            _currentLineType = logLine.LineType;
         }
      }

      protected override void HandleWorkUnitStart(LogLine logLine)
      {
         _unitIndexData.StartIndex = logLine.LineIndex;
         _currentLineType = logLine.LineType;
      }

      protected override void HandleWorkUnitRunning(LogLine logLine)
      {
         if (CurrentClientRun == null)
         {
            Add(new ClientRun(logLine.LineIndex));
         }

         Debug.Assert(CurrentClientRun != null);

         // If we've already seen a WorkUnitRunning line, ignore this one.);
         if (_currentLineType.Equals(LogLineType.WorkUnitRunning)) return;

         // Not Checking the Queue Slot - we don't care if we found a valid slot or not
         if (_unitIndexData.ProcessingIndex > -1)
         {
            CurrentClientRun.UnitIndexes.Add(new UnitIndex(_unitIndexData.QueueSlotIndex, _unitIndexData.ProcessingIndex));
         }
         else if (_unitIndexData.WorkingIndex > -1)
         {
            CurrentClientRun.UnitIndexes.Add(new UnitIndex(_unitIndexData.QueueSlotIndex, _unitIndexData.WorkingIndex));
         }
         else if (_unitIndexData.StartIndex > -1)
         {
            CurrentClientRun.UnitIndexes.Add(new UnitIndex(_unitIndexData.QueueSlotIndex, _unitIndexData.StartIndex));
         }
         else
         {
            CurrentClientRun.UnitIndexes.Add(new UnitIndex(_unitIndexData.QueueSlotIndex, logLine.LineIndex));
         }

         _currentLineType = logLine.LineType;

         // Re-initialize Values
         _unitIndexData.Initialize();
      }

      protected override void HandleWorkUnitPaused(LogLine logLine)
      {
         base.HandleWorkUnitPaused(logLine);

         _currentLineType = logLine.LineType;
      }

      protected override void HandleWorkUnitCoreShutdown(LogLine logLine)
      {
         if (_currentLineType.Equals(LogLineType.WorkUnitRunning))
         {
            AddWorkUnitResult((WorkUnitResult)logLine.LineData);
         }

         _currentLineType = logLine.LineType;
      }

      private void DoRunLevelDetection(ICollection<LogLine> logLines)
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

            foreach (var line in logLines.WhereLineIndex(this[i].ClientStartIndex, end))
            {
               if (line.LineType.Equals(LogLineType.ClientVersion))
               {
                  this[i].ClientVersion = line.LineData.ToString();
               }
               else if (line.LineType.Equals(LogLineType.ClientArguments))
               {
                  this[i].Arguments = line.LineData.ToString();
               }
               else if (line.LineType.Equals(LogLineType.ClientUserNameTeam))
               {
                  var userAndTeam = (ArrayList)line.LineData;
                  this[i].FoldingID = userAndTeam[0].ToString();
                  this[i].Team = (int)userAndTeam[1];
               }
               else if (line.LineType.Equals(LogLineType.ClientUserID) ||
                        line.LineType.Equals(LogLineType.ClientReceivedUserID))
               {
                  this[i].UserID = line.LineData.ToString();
               }
               else if (line.LineType.Equals(LogLineType.ClientMachineID))
               {
                  this[i].MachineID = (int)line.LineData;
               }

               #region Client Status

               if (line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                   line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                   line.LineType.Equals(LogLineType.WorkUnitStart) ||
                   line.LineType.Equals(LogLineType.WorkUnitFrame) ||
                   line.LineType.Equals(LogLineType.WorkUnitResumeFromBattery))
               {
                  this[i].Status = SlotStatus.RunningNoFrameTimes;
               }
               else if (line.LineType.Equals(LogLineType.WorkUnitPaused) ||
                        line.LineType.Equals(LogLineType.WorkUnitPausedForBattery))
               {
                  this[i].Status = SlotStatus.Paused;
               }
               else if (line.LineType.Equals(LogLineType.ClientSendWorkToServer))
               {
                  this[i].Status = SlotStatus.SendingWorkPacket;
               }
               else if (line.LineType.Equals(LogLineType.ClientAttemptGetWorkPacket))
               {
                  this[i].Status = SlotStatus.GettingWorkPacket;
               }
               else if (line.LineType.Equals(LogLineType.ClientEuePauseState))
               {
                  this[i].Status = SlotStatus.EuePause;
               }
               else if (line.LineType.Equals(LogLineType.ClientShutdown) ||
                        line.LineType.Equals(LogLineType.ClientCoreCommunicationsErrorShutdown))
               {
                  this[i].Status = SlotStatus.Stopped;
               }

               #endregion
            }
         }
      }

      #endregion

      /// <summary>
      /// Data container for captured unit indexes
      /// </summary>
      private struct UnitIndexData
      {
         public int ProcessingIndex;
         public int CoreDownloadIndex;
         public int QueueSlotIndex;
         public int WorkingIndex;
         public int StartIndex;

         public void Initialize()
         {
            ProcessingIndex = -1;
            CoreDownloadIndex = -1;
            QueueSlotIndex = -1;
            WorkingIndex = -1;
            StartIndex = -1;
         }
      }
   }
}
