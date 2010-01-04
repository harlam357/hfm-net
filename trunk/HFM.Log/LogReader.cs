/*
 * HFM.NET - Log Reader Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Log
{
   /// <summary>
   /// Reads FAHlog.txt files, determines log positions, and does run level detection.
   /// </summary>
   public class LogReader
   {
      #region Members
      /// <summary>
      /// List of client run positions.
      /// </summary>
      private readonly ClientRunList _ClientRunList = new ClientRunList(); 
      /// <summary>
      /// List of client run positions.
      /// </summary>
      public ClientRunList ClientRunList
      {
         get { return _ClientRunList; }
      }

      /// <summary>
      /// List of client log lines.
      /// </summary>
      private readonly ClientLogLineList _ClientLogLines = new ClientLogLineList();
      /// <summary>
      /// List of client log lines.
      /// </summary>
      public ClientLogLineList ClientLogLines
      {
         get { return _ClientLogLines; }
      }
      #endregion

      #region Properties
      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<ILogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _ClientRunList.LastClientRun;
            if (lastClientRun.UnitStartIndex.Count > 1)
            {
               int start = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 2];
               int end = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 1];

               int length = end - start;

               ILogLine[] logLines = new LogLine[length];

               _ClientLogLines.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }

      /// <summary>
      /// Returns log text of the current work unit.
      /// </summary>
      public IList<ILogLine> CurrentWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _ClientRunList.LastClientRun;
            if (lastClientRun.UnitStartIndex.Count > 0)
            {
               int start = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 1];
               int end = _ClientLogLines.Count;

               int length = end - start;

               ILogLine[] logLines = new LogLine[length];

               _ClientLogLines.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      } 
      #endregion

      #region Methods
      /// <summary>
      /// Get a list of Log Lines that correspond to the given Queue Index.
      /// </summary>
      /// <param name="QueueIndex">The Queue Index (0-9)</param>
      public IList<ILogLine> GetLogLinesFromQueueIndex(int QueueIndex)
      {
         // walk backwards through the ClientRunList and then backward
         // through the UnitQueueIndex list.  Find the first (really last
         // because we're itterating in reverse) UnitQueueIndex that matches
         // the given QueueIndex.
         for (int i = ClientRunList.Count - 1; i >= 0; i--)
         {
            for (int j = ClientRunList[i].UnitStartIndex.Count - 1; j >= 0; j--)
            {
               // if a match is found
               if (ClientRunList[i].UnitQueueIndex[j] == QueueIndex)
               {
                  // set the unit start position
                  int start = ClientRunList[i].UnitStartIndex[j];
                  int end = DetermineEndPosition(i, j);

                  int length = end - start;

                  ILogLine[] logLines = new LogLine[length];

                  _ClientLogLines.CopyTo(start, logLines, 0, length);

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
         if (i == ClientRunList.Count - 1)
         {
            // we're workin on the last unit in the run
            if (j == ClientRunList[i].UnitStartIndex.Count - 1)
            {
               // use the last line index as the end position
               return _ClientLogLines.Count;
            }
            else // we're working on a unit prior to the last
            {
               // use the unit start position for the next unit
               return ClientRunList[i].UnitStartIndex[j + 1];
            }
         }
         else // we're working on a client run prior to the last
         {
            // we're workin on the last unit in the run
            if (j == ClientRunList[i].UnitStartIndex.Count - 1)
            {
               // use the client start position for the next client run
               return ClientRunList[i + 1].ClientStartIndex;
            }
            else
            {
               // use the unit start position for the next unit
               return ClientRunList[i].UnitStartIndex[j + 1];
            }
         }
      }

      /// <summary>
      /// Find the WorkUnitProject Line in the given collection and return it's value.
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      public static string GetProjectFromLogLines(ICollection<ILogLine> logLines)
      {
         if (logLines == null) return String.Empty;
      
         foreach (ILogLine line in logLines)
         {
            // If we encounter a work unit frame, we should have
            // already seen the Project Information, stop looking
            if (line.LineType.Equals(LogLineType.WorkUnitFrame))
            {
               break;
            }
            if (line.LineType.Equals(LogLineType.WorkUnitProject))
            {
               return LogLine.GetProjectString(line);
            }
         }

         return String.Empty;
      }

      /// <summary>
      /// Get the ClientStatus based on the given collection of Log Lines.
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      public static ClientStatus GetStatusFromLogLines(ICollection<ILogLine> logLines)
      {
         ClientStatus returnStatus = ClientStatus.Unknown;
      
         foreach (ILogLine line in logLines)
         {
            if (returnStatus.Equals(ClientStatus.Unknown) &&
               (line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                line.LineType.Equals(LogLineType.WorkUnitStart)))
            {
               returnStatus = ClientStatus.RunningNoFrameTimes;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitPaused)) // || line.LineRaw.Contains("+ Running on battery power"))
            {
               returnStatus = ClientStatus.Paused;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitWorking) && returnStatus.Equals(ClientStatus.Paused))
            {
               returnStatus = ClientStatus.RunningNoFrameTimes;
            }
            else if (line.LineType.Equals(LogLineType.ClientSendWorkToServer))
            {
               returnStatus = ClientStatus.SendingWorkPacket;
            }
            else if (line.LineType.Equals(LogLineType.ClientAttemptGetWorkPacket))
            {
               returnStatus = ClientStatus.GettingWorkPacket;
            }
            else if (line.LineType.Equals(LogLineType.ClientEuePauseState))
            {
               returnStatus = ClientStatus.EuePause;
            }
            else if (line.LineType.Equals(LogLineType.ClientShutdown) ||
                     line.LineType.Equals(LogLineType.ClientCoreCommunicationsErrorShutdown))
            {
               returnStatus = ClientStatus.Stopped;
            }
         }
         
         return returnStatus;
      }

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="InstanceName">Client Instance Name that owns the log file we're parsing.</param>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if LogFileName is Null or Empty.</exception>
      public void ScanFAHLog(string InstanceName, string LogFilePath)
      {
         if (String.IsNullOrEmpty(InstanceName) || String.IsNullOrEmpty(LogFilePath))
         {
            throw new ArgumentException("Arguments 'InstanceName' and 'LogFileName' cannot be a null or empty string.");
         }

         DateTime Start = HfmTrace.ExecStart;

         string[] FAHLogText = File.ReadAllLines(LogFilePath);

         // Need to clear any previous data before scanning.  
         // This Scan could be called multiple times on the same instance.
         _ClientLogLines.Clear();
         _ClientRunList.Clear();

         // Scan all raw lines and create a LogLine object for each denoting its
         // LogLineType and any LineData parsed from the raw line.  Store in 
         // sequential order in the Client LogLine List.
         for (int i = 0; i < FAHLogText.Length; i++)
         {
            _ClientLogLines.HandleLogLine(i, FAHLogText[i]);
         }

         // Now that we know the LineType for each log line, scan the List of LogLine
         // and set Client and Unit Start Indexes.
         foreach (LogLine line in _ClientLogLines)
         {
            _ClientRunList.HandleLogLine(line);
         }

         DoRunLevelDetection();

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, Start);
      }
      
      private void DoRunLevelDetection()
      {
         for (int i = 0; i < ClientRunList.Count; i++)
         {
            int end;
            // we're working on the last client run
            if (i == ClientRunList.Count - 1)
            {
               // use the last line index as the end position
               end = ClientLogLines.Count;
            }
            else // we're working on a client run prior to the last
            {
               // use the client start position for the next client run
               end = ClientRunList[i + 1].ClientStartIndex;
            }
         
            for (int j = ClientRunList[i].ClientStartIndex; j < end; j++)
            {
               if (ClientLogLines[j].LineType.Equals(LogLineType.ClientArguments))
               {
                  ClientRunList[i].Arguments = ClientLogLines[j].LineData.ToString();
               }
               else if (ClientLogLines[j].LineType.Equals(LogLineType.ClientUserNameTeam))
               {
                  ArrayList UserAndTeam = (ArrayList)ClientLogLines[j].LineData;
                  ClientRunList[i].FoldingID = UserAndTeam[0].ToString();
                  ClientRunList[i].Team = (int)UserAndTeam[1];
               }
               else if (ClientLogLines[j].LineType.Equals(LogLineType.ClientUserID) ||
                        ClientLogLines[j].LineType.Equals(LogLineType.ClientReceivedUserID))
               {
                  ClientRunList[i].UserID = ClientLogLines[j].LineData.ToString();
               }
               else if (ClientLogLines[j].LineType.Equals(LogLineType.ClientMachineID))
               {
                  ClientRunList[i].MachineID = (int)ClientLogLines[j].LineData;
               }
            }
         }
      }
      #endregion
   }
   
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   public class ClientRunList : List<ClientRun>
   {
      #region Members
      /// <summary>
      /// Local variable containing the current LogLineType.
      /// </summary>
      private LogLineType _CurrentLineType = LogLineType.Unknown;

      /// <summary>
      /// Holds starting information for the current work unit.
      /// </summary>
      private UnitStartContainer _UnitStart = new UnitStartContainer(); 
      #endregion

      #region Properties
      /// <summary>
      /// Returns the most recent client run if available, otherwise null.
      /// </summary>
      public ClientRun LastClientRun
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
      /// <summary>
      /// Handles the given LogLineType and sets the correct position value.
      /// </summary>
      public void HandleLogLine(LogLine line)
      {
         switch (line.LineType)
         {
            case LogLineType.LogOpen:
               HandleLogOpen(line.LineIndex);
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.LogHeader:
               HandleLogHeader(line.LineIndex);
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitProcessing:
               HandleWorkUnitProcessing(line.LineIndex);
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitCoreDownload:
               HandleWorkUnitCoreDownload(line.LineIndex);
               _CurrentLineType = line.LineType;
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
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitRunning:
               HandleWorkUnitRunning();
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitPaused:
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitCoreShutdown:
               HandleWorkUnitCoreShutdown(line);
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.ClientNumberOfUnitsCompleted:
               HandleClientNumberOfUnitsCompleted(line);
               break;
         }
      }

      /// <summary>
      /// Handles LogOpen LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleLogOpen(int LineIndex)
      {
         // Add a new ClientRun on LogOpen
         Add(new ClientRun(LineIndex));
      }

      /// <summary>
      /// Handles LogHeader LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleLogHeader(int LineIndex)
      {
         // If the last line observed was a LogOpen or a LogHeader, return
         // and don't use this as a signal to add a new ClientRun.
         if (_CurrentLineType.Equals(LogLineType.LogOpen) ||
             _CurrentLineType.Equals(LogLineType.LogHeader)) return;

         // Otherwise, if we see a LogHeader and the preceeding line was not
         // a LogOpen or a LogHeader, then we use this as a signal to create
         // a new ClientRun.  This is a backup option and I don't expect this
         // situtation to happen at all if the log file is not corrupt.
         Add(new ClientRun(LineIndex));
      }

      /// <summary>
      /// Handles WorkUnitProcessing LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitProcessing(int LineIndex)
      {
         // If we have not found a ProcessingIndex (== -1) then set it.
         // Othwerwise, if ProcessingIndex (!= -1) and a CoreDownloadIndex
         // has been observerd and is greater than the current ProcessingIndex,
         // then update the ProcessingIndex to bypass the CoreDownload section
         // of the log file.
         if (_UnitStart.WorkUnitProcessingIndex == -1 ||
            (_UnitStart.WorkUnitProcessingIndex != -1 &&
             _UnitStart.WorkUnitCoreDownloadIndex > _UnitStart.WorkUnitProcessingIndex))
         {
            _UnitStart.WorkUnitProcessingIndex = LineIndex;
         }
      }

      /// <summary>
      /// Handles WorkUnitCoreDownload LogLineType.
      /// </summary>
      private void HandleWorkUnitCoreDownload(int LineIndex)
      {
         _UnitStart.WorkUnitCoreDownloadIndex = LineIndex;
      }

      /// <summary>
      /// Handles WorkUnitQueueIndex LogLineType.
      /// </summary>
      private void HandleWorkUnitQueueIndex(int QueueIndex)
      {
         _UnitStart.WorkUnitQueueSlotIndex = QueueIndex;
      }

      /// <summary>
      /// Handles WorkUnitWorking LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitWorking(int LineIndex)
      {
         // This first check allows us to overlook the "+ Working ..." message
         // that gets written after a client is Paused.  We don't want to key
         // work unit positions based on this log entry.
         if (_CurrentLineType.Equals(LogLineType.WorkUnitPaused))
         {
            // Return to a Running state
            _CurrentLineType = LogLineType.WorkUnitRunning;
         }
         else
         {
            _UnitStart.WorkUnitWorkingIndex = LineIndex;
            _CurrentLineType = LogLineType.WorkUnitWorking;
         }
      }

      /// <summary>
      /// Handles WorkUnitStart LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitStart(int LineIndex)
      {
         _UnitStart.WorkUnitStartIndex = LineIndex;
      }

      /// <summary>
      /// Handles WorkUnitRunning LogLineType.
      /// </summary>
      /// <remarks></remarks>
      private void HandleWorkUnitRunning()
      {
         if (LastClientRun != null)
         {
            // If we've already seen a WorkUnitRunning line, ignore this one.
            if (_CurrentLineType.Equals(LogLineType.WorkUnitRunning)) return;

            if (_UnitStart.WorkUnitProcessingIndex > -1)
            {
               LastClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitProcessingIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               LastClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
            }
            else if (_UnitStart.WorkUnitWorkingIndex > -1)
            {
               LastClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitWorkingIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               LastClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
            }
            else if (_UnitStart.WorkUnitStartIndex > -1)
            {
               LastClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitStartIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               LastClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
            }
            else
            {
               // No Unit Start Index.  This log file looks to be corrupted.
               throw new InvalidOperationException("Could not find a Unit Start Index.");
            }

            // Make a new container
            _UnitStart = new UnitStartContainer();
         }
         else
         {
            // no client run to attach this unit start
            throw new InvalidOperationException("Found Work Unit Data before any Log Headers.");
         }
      }

      /// <summary>
      /// Handles WorkUnitCoreShutdown LogLineType.
      /// </summary>
      /// <param name="logLine">The given LogLine object.</param>
      private void HandleWorkUnitCoreShutdown(ILogLine logLine)
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitRunning))
         {
            if (LastClientRun != null)
            {
               if (logLine.LineData.Equals(WorkUnitResult.FinishedUnit))
               {
                  LastClientRun.NumberOfCompletedUnits++;
               }
               else if (logLine.LineData.Equals(WorkUnitResult.EarlyUnitEnd) ||
                        logLine.LineData.Equals(WorkUnitResult.UnstableMachine) ||
                        logLine.LineData.Equals(WorkUnitResult.Interrupted) ||
                        logLine.LineData.Equals(WorkUnitResult.CoreOutdated)) 
               {
                  LastClientRun.NumberOfFailedUnits++;
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
            LastClientRun.NumberOfTotalUnitsCompleted = (int)logLine.LineData;
         }
      } 
      #endregion
   }
   
   /// <summary>
   /// Container for captured Unit Start Indexes
   /// </summary>
   internal class UnitStartContainer
   {
      #region Members
      internal int WorkUnitProcessingIndex = -1;
      internal int WorkUnitCoreDownloadIndex = -1;
      internal int WorkUnitQueueSlotIndex = -1;
      internal int WorkUnitWorkingIndex = -1;
      internal int WorkUnitStartIndex = -1; 
      #endregion
   }
   
   /// <summary>
   /// Holds Positions (Index) for a Single Client Run.
   /// </summary>
   public class ClientRun
   {
      #region Members
      private string _Arguments = String.Empty;
      public string Arguments
      {
         get { return _Arguments; }
         set { _Arguments = value; }
      }

      private string _FoldingID = String.Empty;
      public string FoldingID
      {
         get { return _FoldingID; }
         set { _FoldingID = value; }
      }
      
      private int _Team;
      public int Team
      {
         get { return _Team; }
         set { _Team = value; }
      }

      private string _UserID = String.Empty;
      public string UserID
      {
         get { return _UserID; }
         set { _UserID = value; }
      }
      
      private int _MachineID;
      public int MachineID
      {
         get { return _MachineID; }
         set { _MachineID = value; }
      }
      
      private int _NumberOfCompletedUnits = 0;
      public int NumberOfCompletedUnits
      {
         get { return _NumberOfCompletedUnits; }
         set { _NumberOfCompletedUnits = value; }
      }

      private int _NumberOfFailedUnits = 0;
      public int NumberOfFailedUnits
      {
         get { return _NumberOfFailedUnits; }
         set { _NumberOfFailedUnits = value; }
      }

      private int _NumberOfTotalUnitsCompleted = 0;
      public int NumberOfTotalUnitsCompleted
      {
         get { return _NumberOfTotalUnitsCompleted; }
         set { _NumberOfTotalUnitsCompleted = value; }
      }

      /// <summary>
      /// Line index of client start position.
      /// </summary>
      private readonly int _ClientStartPosition;
      /// <summary>
      /// Line index of client start position.
      /// </summary>
      public int ClientStartIndex
      {
         get { return _ClientStartPosition; }
      }

      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      private readonly List<int> _UnitStartIndex = new List<int>();
      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      public List<int> UnitStartIndex
      {
         get { return _UnitStartIndex; }
      }

      /// <summary>
      /// List of Queue Indexes that correspond to the Unit Start Indexes for this client run.
      /// </summary>
      private readonly List<int> _UnitQueueIndex = new List<int>();
      /// <summary>
      /// List of Queue Indexes that correspond to the Unit Start Indexes for this client run.
      /// </summary>
      public List<int> UnitQueueIndex
      {
         get { return _UnitQueueIndex; }
      } 
      #endregion

      #region CTOR
      /// <summary>
      /// Primary Constructor
      /// </summary>
      /// <param name="ClientStartIndex">Line index of client start.</param>
      public ClientRun(int ClientStartIndex)
      {
         _ClientStartPosition = ClientStartIndex;
      } 
      #endregion
   }
   
   public class ClientLogLineList : List<ILogLine>
   {
      public void HandleLogLine(int index, string logLine)
      {
         LogLineType lineType = DetermineLineType(logLine);
         Add(new LogLine(lineType, index, logLine));
      }

      /// <summary>
      /// Inspect the given log line and determine the line type.
      /// </summary>
      /// <param name="logLine">The log line being inspected.</param>
      private static LogLineType DetermineLineType(string logLine)
      {
         if (logLine.Contains("--- Opening Log file"))
         {
            return LogLineType.LogOpen;
         }
         else if (logLine.Contains("###############################################################################"))
         {
            return LogLineType.LogHeader;
         }
         else if (logLine.Contains("] Sending work to server"))
         {
            return LogLineType.ClientSendWorkToServer;
         }
         else if (logLine.Contains("] - Autosending finished units..."))
         {
            return LogLineType.ClientAutosendStart;
         }
         else if (logLine.Contains("] - Autosend completed"))
         {
            return LogLineType.ClientAutosendComplete;
         }
         else if (logLine.Contains("] + Attempting to send results"))
         {
            return LogLineType.ClientSendStart;
         }
         else if (logLine.Contains("] + Could not connect to Work Server"))
         {
            return LogLineType.ClientSendConnectFailed;
         }
         else if (logLine.Contains("] - Error: Could not transmit unit"))
         {
            return LogLineType.ClientSendFailed;
         }
         else if (logLine.Contains("] + Results successfully sent"))
         {
            return LogLineType.ClientSendComplete;
         }
         else if (logLine.Contains("Arguments:"))
         {
            return LogLineType.ClientArguments;
         }
         else if (logLine.Contains("] - User name:"))
         {
            return LogLineType.ClientUserNameTeam;
         }
         else if (logLine.Contains("] + Requesting User ID from server"))
         {
            return LogLineType.ClientRequestingUserID;
         }
         else if (logLine.Contains("- Received User ID ="))
         {
            return LogLineType.ClientReceivedUserID;
         }
         else if (logLine.Contains("] - User ID:"))
         {
            return LogLineType.ClientUserID;
         }
         else if (logLine.Contains("] - Machine ID:"))
         {
            return LogLineType.ClientMachineID;
         }
         else if (logLine.Contains("] + Attempting to get work packet"))
         {
            return LogLineType.ClientAttemptGetWorkPacket;
         }
         else if (logLine.Contains("] - Will indicate memory of"))
         {
            return LogLineType.ClientIndicateMemory;
         }
         else if (logLine.Contains("] - Detect CPU. Vendor:"))
         {
            return LogLineType.ClientDetectCpu;
         }
         else if (logLine.Contains("] + Processing work unit"))
         {
            return LogLineType.WorkUnitProcessing;
         }
         else if (logLine.Contains("] + Downloading new core"))
         {
            return LogLineType.WorkUnitCoreDownload;
         }
         else if (logLine.Contains("] Working on Unit 0"))
         {
            return LogLineType.WorkUnitIndex;
         }
         else if (logLine.Contains("] Working on queue slot 0"))
         {
            return LogLineType.WorkUnitQueueIndex;
         }
         else if (logLine.Contains("] + Working ..."))
         {
            return LogLineType.WorkUnitWorking;
         }
         else if (logLine.Contains("] *------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("] ************************** ProtoMol Folding@Home Core **************************"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*******************/
         else if (logLine.Contains("] Version"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("]   Version:"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
         /*******************/
         else if (IsLineTypeWorkUnitStarted(logLine))
         {
            return LogLineType.WorkUnitRunning;
         }
         else if (logLine.Contains("] Project:"))
         {
            return LogLineType.WorkUnitProject;
         }
         else if (logLine.Contains("] Completed "))
         {
            return LogLineType.WorkUnitFrame;
         }
         else if (logLine.Contains("] + Paused"))
         {
            return LogLineType.WorkUnitPaused;
         }
         else if (logLine.Contains("] - Shutting down core"))
         {
            return LogLineType.WorkUnitShuttingDownCore;
         }
         else if (logLine.Contains("] Folding@home Core Shutdown:"))
         {
            return LogLineType.WorkUnitCoreShutdown;
         }
         else if (logLine.Contains("] + Number of Units Completed:"))
         {
            return LogLineType.ClientNumberOfUnitsCompleted;
         }
         else if (logLine.Contains("] This is a sign of more serious problems, shutting down."))
         {
            return LogLineType.ClientCoreCommunicationsErrorShutdown;
         }
         else if (logLine.Contains("] EUE limit exceeded. Pausing 24 hours."))
         {
            return LogLineType.ClientEuePauseState;
         }
         else if (logLine.Contains("] Folding@Home will go to sleep for 1 day"))
         {
            return LogLineType.ClientEuePauseState;
         }
         else if (logLine.Contains("Folding@Home Client Shutdown"))
         {
            return LogLineType.ClientShutdown;
         }
         else
         {
            return LogLineType.Unknown;
         }
      }

      /// <summary>
      /// Inspect the given log line and determine if the line type is LogLineType.WorkUnitRunning.
      /// </summary>
      /// <param name="logLine">The log line being inspected.</param>
      private static bool IsLineTypeWorkUnitStarted(string logLine)
      {
         if (logLine.Contains("] Preparing to commence simulation"))
         {
            return true;
         }
         else if (logLine.Contains("] Called DecompressByteArray"))
         {
            return true;
         }
         else if (logLine.Contains("] - Digital signature verified"))
         {
            return true;
         }
         /*** ProtoMol Only */
         else if (logLine.Contains("] Digital signatures verified"))
         {
            return true;
         }
         /*******************/
         else if (logLine.Contains("] Entering M.D."))
         {
            return true;
         }

         return false;
      }
   }
}
