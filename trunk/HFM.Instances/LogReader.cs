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
using System.Text.RegularExpressions;

using HFM.Instrumentation;

namespace HFM.Instances
{
   /// <summary>
   /// Log Line Types
   /// </summary>
   public enum LogLineType
   {
      Unknown = -1,
      LogOpen = 0,
      LogHeader,
      ClientSendWorkToServer,
      ClientAutosendStart,
      ClientAutosendComplete,
      ClientSendStart,
      ClientSendConnectFailed,
      ClientSendFailed,
      ClientSendComplete,
      ClientArguments,
      ClientUserNameTeam,
      ClientRequestingUserID,
      ClientReceivedUserID,
      ClientUserID,
      ClientMachineID,
      ClientAttemptGetWorkPacket,
      ClientIndicateMemory,
      ClientDetectCpu,
      WorkUnitProcessing,
      WorkUnitCoreDownload,
      WorkUnitIndex,
      WorkUnitQueueIndex,
      WorkUnitWorking,
      WorkUnitStart,
      WorkUnitCoreVersion,
      WorkUnitRunning,
      WorkUnitProject,
      WorkUnitFrame,
      WorkUnitPaused,
      WorkUnitShuttingDownCore,
      WorkUnitCoreShutdown,
      ClientNumberOfUnitsCompleted,
      ClientCoreCommunicationsErrorShutdown,
      ClientEuePauseState,
      ClientShutdown
   }

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
      public IList<LogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _ClientRunList.LastClientRun;
            if (lastClientRun.UnitStartIndex.Count > 1)
            {
               int start = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 2];
               int end = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 1];

               int length = end - start;

               LogLine[] logLines = new LogLine[length];

               _ClientLogLines.CopyTo(start, logLines, 0, length);

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
            ClientRun lastClientRun = _ClientRunList.LastClientRun;
            if (lastClientRun.UnitStartIndex.Count > 0)
            {
               int start = lastClientRun.UnitStartIndex[lastClientRun.UnitStartIndex.Count - 1];
               int end = _ClientLogLines.Count;

               int length = end - start;

               LogLine[] logLines = new LogLine[length];

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
      public IList<LogLine> GetLogLinesFromQueueIndex(int QueueIndex)
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

                  LogLine[] logLines = new LogLine[length];

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
      public static string GetProjectFromLogLines(ICollection<LogLine> logLines)
      {
         foreach (LogLine line in logLines)
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
      public static ClientStatus GetStatusFromLogLines(ICollection<LogLine> logLines)
      {
         ClientStatus returnStatus = ClientStatus.Unknown;
      
         foreach (LogLine line in logLines)
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
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if LogFileName is Null or Empty.</exception>
      public void ScanFAHLog(ClientInstance Instance, string LogFilePath)
      {
         if (Instance == null) throw new ArgumentNullException("Instance", "Argument 'Instance' cannot be null.");
         if (String.IsNullOrEmpty(LogFilePath)) throw new ArgumentException("Argument 'LogFileName' cannot be a null or empty string.");

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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Instance.InstanceName, Start);
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

      /// <summary>
      /// Populate Client Startup Arguments from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      public void PopulateClientStartupArgumentData(ClientInstance Instance)
      {
         PopulateClientStartupArgumentData(Instance, ClientRunList.Count - 1);
      }
      
      /// <summary>
      /// Populate Client Startup Arguments from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      /// <param name="RunIndex">Client Run Index</param>
      public void PopulateClientStartupArgumentData(ClientInstance Instance, int RunIndex)
      {
         Instance.Arguments = ClientRunList[RunIndex].Arguments;
      }

      /// <summary>
      /// Populate FoldingID, Team, UserID, and MachineID from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      public void PopulateUserAndMachineData(ClientInstance Instance)
      {
         PopulateUserAndMachineData(Instance, ClientRunList.Count - 1);
      }
      
      /// <summary>
      /// Populate FoldingID, Team, UserID, and MachineID from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      /// <param name="RunIndex">Client Run Index</param>
      public void PopulateUserAndMachineData(ClientInstance Instance, int RunIndex)
      {
         Instance.FoldingID = ClientRunList[RunIndex].FoldingID;
         Instance.Team = ClientRunList[RunIndex].Team;

         Instance.UserID = ClientRunList[RunIndex].UserID;
         Instance.MachineID = ClientRunList[RunIndex].MachineID;
      }

      /// <summary>
      /// Populate Completed, Failed, and Total Completed Work Units from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      public void PopulateWorkUnitCountData(ClientInstance Instance)
      {
         PopulateWorkUnitCountData(Instance, ClientRunList.Count - 1);
      }

      /// <summary>
      /// Populate Completed, Failed, and Total Completed Work Units from Log Data
      /// </summary>
      /// <param name="Instance">ClientInstance to populate</param>
      /// <param name="RunIndex">Client Run Index</param>
      public void PopulateWorkUnitCountData(ClientInstance Instance, int RunIndex)
      {
         Instance.NumberOfCompletedUnitsSinceLastStart = ClientRunList[RunIndex].NumberOfCompletedUnits;
         Instance.NumberOfFailedUnitsSinceLastStart = ClientRunList[RunIndex].NumberOfFailedUnits;

         Instance.TotalUnits = ClientRunList[RunIndex].NumberOfTotalUnitsCompleted;
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
      private void HandleWorkUnitCoreShutdown(LogLine logLine)
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
                        logLine.LineData.Equals(WorkUnitResult.Interrupted))
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
      private void HandleClientNumberOfUnitsCompleted(LogLine logLine)
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
   
   public class ClientLogLineList : List<LogLine>
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
         else if (logLine.Contains("] ******************* Folding@Home Core *******************"))
         {
            return LogLineType.WorkUnitStart;
         }
         /*******************/
         else if (logLine.Contains("] Version"))
         {
            return LogLineType.WorkUnitCoreVersion;
         }
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
   
   public class LogLine
   {
      #region Regex (Static)
      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex rUserTeam =
         new Regex("\\[(?<Timestamp>.{8})\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rReceivedUserID =
         new Regex("\\[(?<Timestamp>.{8})\\].*- Received User ID = (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rUserID =
         new Regex("\\[(?<Timestamp>.{8})\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rMachineID =
         new Regex("\\[(?<Timestamp>.{8})\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rUnitIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on Unit 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rQueueIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on queue slot 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Core Version string.
      /// </summary>
      private static readonly Regex rCoreVersion =
         new Regex("\\[(?<Timestamp>.{8})\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Work Unit Project string.
      /// </summary>
      private static readonly Regex rProjectID =
         new Regex("\\[(?<Timestamp>.{8})\\] Project: (?<ProjectNumber>.*) \\(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Standard and SMP Clients Frame Completion Lines (Gromacs Style).
      /// </summary>
      private static readonly Regex rFramesCompleted =
         new Regex("\\[(?<Timestamp>.{8})\\] Completed (?<Completed>.*) out of (?<Total>.*) steps  \\((?<Percent>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 1
      /// </summary>
      private static readonly Regex rPercent1 =
         new Regex("(?<Percent>.*) percent", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 2
      /// </summary>
      private static readonly Regex rPercent2 =
            new Regex("(?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      ///// <summary>
      ///// Regular Expression to match Standard Client Frame Completion Lines (ProtoMol Style - Experimental)
      ///// </summary>
      //private static readonly Regex rFramesCompletedProtomol =
      //      new Regex("\\[(?<Timestamp>.{8})\\] Completed (?<Completed>.*) out of (?<Total>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match GPU2 Client Frame Completion Lines
      /// </summary>
      private static readonly Regex rFramesCompletedGpu =
            new Regex("\\[(?<Timestamp>.{8})\\] Completed (?<Percent>[0-9]{1,3})%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rCoreShutdown =
         new Regex("\\[(?<Timestamp>.{8})\\] Folding@home Core Shutdown: (?<UnitResult>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Completed Work Units string.
      /// </summary>
      private static readonly Regex rCompletedWUs =
         new Regex("\\[(?<Timestamp>.{8})\\] \\+ Number of Units Completed: (?<Completed>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);  
      #endregion

      #region Members
      private LogLineType _LineType;
      public LogLineType LineType
      {
         get { return _LineType; }
         set { _LineType = value; }
      }

      private readonly int _LineIndex;
      public int LineIndex
      {
         get { return _LineIndex; }
      }

      private readonly string _LineRaw;
      public string LineRaw
      {
         get { return _LineRaw; }
      }

      private readonly object _LineData;
      public object LineData
      {
         get { return _LineData; }
      } 
      #endregion

      #region CTOR
      public LogLine(LogLineType type, int index, string logLine)
      {
         try
         {
            _LineType = type;
            _LineIndex = index;
            _LineRaw = logLine;
            _LineData = GetLineData(this);
         }
         catch (Exception ex)
         {
            _LineType = LogLineType.Unknown;

            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
         }
      } 
      #endregion

      #region Methods
      private static object GetLineData(LogLine logLine)
      {
         switch (logLine.LineType)
         {
            case LogLineType.ClientArguments:
               return logLine.LineRaw.Substring(10).Trim();
            case LogLineType.ClientUserNameTeam:
               Match mUserTeam;
               if ((mUserTeam = rUserTeam.Match(logLine.LineRaw)).Success)
               {
                  ArrayList list = new ArrayList(2);
                  list.Add(mUserTeam.Result("${Username}"));
                  list.Add(Int32.Parse(mUserTeam.Result("${TeamNumber}")));
                  return list;
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse User Name and Team values from '{0}'", logLine.LineRaw));
               }
            case LogLineType.ClientReceivedUserID:
               Match mReceivedUserID;
               if ((mReceivedUserID = rReceivedUserID.Match(logLine.LineRaw)).Success)
               {
                  return mReceivedUserID.Result("${UserID}");
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
               }
            case LogLineType.ClientUserID:
               Match mUserID;
               if ((mUserID = rUserID.Match(logLine.LineRaw)).Success)
               {
                  return mUserID.Result("${UserID}");
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
               }
            case LogLineType.ClientMachineID:
               Match mMachineID;
               if ((mMachineID = rMachineID.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mMachineID.Result("${MachineID}"));
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Machine ID value from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitIndex:
               Match mUnitIndex;
               if ((mUnitIndex = rUnitIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mUnitIndex.Result("${QueueIndex}"));
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitQueueIndex:
               Match mQueueIndex;
               if ((mQueueIndex = rQueueIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mQueueIndex.Result("${QueueIndex}"));
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitCoreVersion:
               Match mCoreVer;
               if ((mCoreVer = rCoreVersion.Match(logLine.LineRaw)).Success)
               {
                  string sCoreVer = mCoreVer.Result("${CoreVer}");
                  if (sCoreVer.IndexOf(" ") > 1)
                  {
                     return sCoreVer.Substring(0, sCoreVer.IndexOf(" "));
                  }
                  else
                  {
                     return sCoreVer;
                  }
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Core Version value from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitProject:
               Match mProjectID;
               if ((mProjectID = rProjectID.Match(logLine.LineRaw)).Success)
               {
                  return mProjectID;
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Project (R/C/G) values from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitFrame:
               FrameData frame = new FrameData();
               if (CheckForCompletedFrame(logLine, frame))
               {
                  return frame;
               }
               else if (CheckForCompletedGpuFrame(logLine, frame))
               {
                  return frame;
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Frame Data from '{0}'", logLine.LineRaw));
               }
            case LogLineType.WorkUnitCoreShutdown:
               Match mCoreShutdown;
               if ((mCoreShutdown = rCoreShutdown.Match(logLine.LineRaw)).Success)
               {
                  return UnitInfo.WorkUnitResultFromString(mCoreShutdown.Result("${UnitResult}"));
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Work Unit Result value from '{0}'", logLine.LineRaw));
               }
            case LogLineType.ClientNumberOfUnitsCompleted:
               Match mCompletedWUs;
               if ((mCompletedWUs = rCompletedWUs.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mCompletedWUs.Result("${Completed}"));
               }
               else
               {
                  throw new FormatException(String.Format("Failed to parse Units Completed value from '{0}'", logLine.LineRaw));
               }
         }

         return null;
      }

      public static string GetProjectString(LogLine line)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            int ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
            int ProjectRun = Int32.Parse(match.Result("${Run}"));
            int ProjectClone = Int32.Parse(match.Result("${Clone}"));
            int ProjectGen = Int32.Parse(match.Result("${Gen}"));

            return String.Format("P{0} (R{1}, C{2}, G{3})", ProjectID,
                                                            ProjectRun,
                                                            ProjectClone,
                                                            ProjectGen);
         }

         throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
      }

      public static string GetLongProjectString(LogLine line)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            int ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
            int ProjectRun = Int32.Parse(match.Result("${Run}"));
            int ProjectClone = Int32.Parse(match.Result("${Clone}"));
            int ProjectGen = Int32.Parse(match.Result("${Gen}"));

            return String.Format("{0} (Run {1}, Clone {2}, Gen {3})", ProjectID,
                                                                      ProjectRun,
                                                                      ProjectClone,
                                                                      ProjectGen);
         }

         throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      /// <param name="frame">Frame Data</param>
      private static bool CheckForCompletedFrame(LogLine logLine, FrameData frame)
      {
         Match mFramesCompleted = rFramesCompleted.Match(logLine.LineRaw);
         if (mFramesCompleted.Success)
         {
            try
            {
               frame.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               frame.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
            }
            catch (FormatException ex)
            {
               throw new FormatException(String.Format("{0} Failed to parse raw frame values from '{1}'.", HfmTrace.FunctionName, logLine), ex);
            }

            string percentString = mFramesCompleted.Result("${Percent}");

            Match mPercent1 = rPercent1.Match(percentString);
            Match mPercent2 = rPercent2.Match(percentString);

            int framePercent;
            if (mPercent1.Success)
            {
               framePercent = Int32.Parse(mPercent1.Result("${Percent}"));
            }
            else if (mPercent2.Success)
            {
               framePercent = Int32.Parse(mPercent2.Result("${Percent}"));
            }
            // Try to parse a percentage from in between the parentheses (for older single core clients like v5.02) - Issue 36
            else if (Int32.TryParse(percentString, out framePercent) == false)
            {
               throw new FormatException(String.Format("Failed to parse frame percent from '{0}'.", logLine));
            }

            // Validate the steps are in tolerance with the detected frame percent - Issue 98
            double calculatedPercent = ((double)frame.RawFramesComplete / frame.RawFramesTotal) * 100;
            // ex. [00:19:40] Completed 82499 out of 250000 steps  (33%) - Would Validate
            //     [00:19:40] Completed 82750 out of 250000 steps  (33%) - Would Validate
            // 10% frame step tolerance. In the example the completed must be within 250 steps.
            if (Math.Abs(calculatedPercent - framePercent) <= 0.1)
            {
               frame.TimeStampString = mFramesCompleted.Result("${Timestamp}");
               frame.FrameID = framePercent;
               
               return true;
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Not on percent boundry '{0}' (this is not a problem).", logLine), true);
               return false;
            }
         }
         /*** ProtoMol Only */
         //else
         //{
         //   Match mFramesCompletedProtomol = rFramesCompletedProtomol.Match(logLine.LineRaw);
         //   if (mFramesCompletedProtomol.Success)
         //   {
         //      try
         //      {
         //         frame.RawFramesComplete = Int32.Parse(mFramesCompletedProtomol.Result("${Completed}"));
         //         frame.RawFramesTotal = Int32.Parse(mFramesCompletedProtomol.Result("${Total}"));
         //      }
         //      catch (FormatException ex)
         //      {
         //         throw new FormatException(String.Format("Failed to parse raw frame values from '{0}'.", logLine), ex);
         //      }

         //      double calculatedPercent = ((double)frame.RawFramesComplete / frame.RawFramesTotal) * 100;
         //      int framePercent = (int)Math.Floor(calculatedPercent);
               
         //      frame.TimeStampString = mFramesCompletedProtomol.Result("${Timestamp}");
         //      frame.FrameID = framePercent;
               
         //      return true;
         //   }
            
         //   return false;
         //}
         /*******************/
         
         return false;
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      /// <param name="frame">Frame Data</param>
      private static bool CheckForCompletedGpuFrame(LogLine logLine, FrameData frame)
      {
         Match mFramesCompletedGpu = rFramesCompletedGpu.Match(logLine.LineRaw);
         if (mFramesCompletedGpu.Success)
         {
            logLine.LineType = LogLineType.WorkUnitFrame;

            frame.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            frame.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            //TODO: Hard code here, 100 GPU Frames. Could I get this from the Project Data?
            //I could but what's the point, 100% is 100%.

            frame.TimeStampString = mFramesCompletedGpu.Result("${Timestamp}");
            frame.FrameID = frame.RawFramesComplete;
            
            return true;
         }
         
         return false;
      }

      public override string ToString()
      {
         return _LineRaw;
      } 
      #endregion
   }
   
   internal class FrameData
   {
      private string _TimeStampString;
      public string TimeStampString
      {
         get { return _TimeStampString; }
         set { _TimeStampString = value; }
      }
   
      private int _RawFramesComplete;
      public int RawFramesComplete
      {
         get { return _RawFramesComplete; }
         set { _RawFramesComplete = value; }
      }

      private int _RawFramesTotal;
      public int RawFramesTotal
      {
         get { return _RawFramesTotal; }
         set { _RawFramesTotal = value; }
      }

      private int _FrameID;
      public int FrameID
      {
         get { return _FrameID; }
         set { _FrameID = value; }
      }
   }
}
