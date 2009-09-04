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

using HFM.Proteins;
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
      /// Local log text array.
      /// </summary>
      private string[] _FAHLogText = null;
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

      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<LogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _ClientRunList.LastClientRun;
            if (lastClientRun.UnitStartPositions.Count > 1)
            {
               int start = lastClientRun.UnitStartPositions[lastClientRun.UnitStartPositions.Count - 2];
               int end = lastClientRun.UnitStartPositions[lastClientRun.UnitStartPositions.Count - 1];

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
            if (lastClientRun.UnitStartPositions.Count > 0)
            {
               int start = lastClientRun.UnitStartPositions[lastClientRun.UnitStartPositions.Count - 1];
               int end = _FAHLogText.Length;

               int length = end - start;
               
               LogLine[] logLines = new LogLine[length];
               
               _ClientLogLines.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }

      /// <summary>
      /// Read log file into local string array.
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if LogFileName is Null or Empty.</exception>
      public void ReadLogText(ClientInstance Instance, string LogFilePath)
      {
         if (Instance == null) throw new ArgumentNullException("Instance", "Argument 'Instance' cannot be null.");
         if (String.IsNullOrEmpty(LogFilePath)) throw new ArgumentException("Argument 'LogFileName' cannot be a null or empty string.");

         DateTime Start = HfmTrace.ExecStart;

         _FAHLogText = File.ReadAllLines(LogFilePath);

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Instance.InstanceName, Start);
      }

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      public void ScanFAHLog(ClientInstance Instance)
      {
         if (Instance == null) throw new ArgumentNullException("Instance", "Argument 'Instance' cannot be null.");

         DateTime Start = HfmTrace.ExecStart;

         for (int i = 0; i < _FAHLogText.Length; i++)
         {
            _ClientLogLines.HandleLogLine(i, _FAHLogText[i]);
         }

         foreach (LogLine line in _ClientLogLines)
         {
            _ClientRunList.HandleLogLine(line);
         }

         DoLastRunLevelDetection(Instance);

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Instance.InstanceName, Start);
      }

      /// <summary>
      /// Inspect the log of the last client run.
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      private void DoLastRunLevelDetection(ClientInstance Instance)
      {
         ClientRun clientRun = _ClientRunList.LastClientRun;
         
         if (clientRun != null)
         {
            ArrayList list = (ArrayList)_ClientLogLines.GetRunUserNameAndTeam(clientRun);
            if (list != null)
            {
               Instance.FoldingID = list[0].ToString();
               Instance.Team = (int)list[1];
            }
            
            Instance.UserID = _ClientLogLines.GetRunUserID(clientRun);
            Instance.MachineID = _ClientLogLines.GetRunMachineID(clientRun);
            
            Instance.NumberOfCompletedUnitsSinceLastStart = clientRun.NumberOfCompletedUnits;
            Instance.NumberOfFailedUnitsSinceLastStart = clientRun.NumberOfFailedUnits;
            
            Instance.TotalUnits = clientRun.NumberOfTotalUnitsCompleted;
         }
      }
   }
   
   /// <summary>
   /// List of Client Runs.
   /// </summary>
   public class ClientRunList : List<ClientRun>
   {
      /// <summary>
      /// Local variable containing the current LogLineType.
      /// </summary>
      private LogLineType _CurrentLineType = LogLineType.Unknown;

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
               HandleWorkUnitCoreDownload();
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitWorking:
               HandleWorkUnitWorking(line.LineIndex);
               break;
            case LogLineType.WorkUnitStart:
               HandleWorkUnitStart(line.LineIndex);
               _CurrentLineType = line.LineType;
               break;
            case LogLineType.WorkUnitRunning:
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
         Add(new ClientRun(LineIndex));
      }

      /// <summary>
      /// Handles LogHeader LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleLogHeader(int LineIndex)
      {
         if (_CurrentLineType.Equals(LogLineType.LogOpen) ||
             _CurrentLineType.Equals(LogLineType.LogHeader)) return;

         Add(new ClientRun(LineIndex));
      }

      /// <summary>
      /// Handles WorkUnitProcessing LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitProcessing(int LineIndex)
      {
         if (LastClientRun != null)
         {
            LastClientRun.UnitStartPositions.Add(LineIndex);
         }
      }

      /// <summary>
      /// Handles WorkUnitCoreDownload LogLineType.
      /// </summary>
      private void HandleWorkUnitCoreDownload()
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitProcessing))
         {
            if (LastClientRun != null)
            {
               LastClientRun.UnitStartPositions.RemoveAt(LastClientRun.UnitStartPositions.Count - 1);
            }
         }
      }

      /// <summary>
      /// Handles WorkUnitWorking LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitWorking(int LineIndex)
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitPaused))
         {
            _CurrentLineType = LogLineType.WorkUnitRunning;
         }
         else 
         {
            if (_CurrentLineType.Equals(LogLineType.WorkUnitProcessing) == false)
            {
               if (LastClientRun != null)
               {
                  LastClientRun.UnitStartPositions.Add(LineIndex);
               }
            }
            
            _CurrentLineType = LogLineType.WorkUnitWorking;
         }
      }

      /// <summary>
      /// Handles WorkUnitStart LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitStart(int LineIndex)
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitWorking) == false)
         {
            if (LastClientRun != null)
            {
               LastClientRun.UnitStartPositions.Add(LineIndex);
            }
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
   }
   
   /// <summary>
   /// Holds Positions (Index) for a Single Client Run.
   /// </summary>
   public class ClientRun
   {
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
      public int ClientStartPosition
      {
         get { return _ClientStartPosition; }
      }

      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      private readonly List<int> _UnitStartPositions = new List<int>();
      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      public List<int> UnitStartPositions
      {
         get { return _UnitStartPositions; }
      }

      /// <summary>
      /// Primary Constructor.
      /// </summary>
      /// <param name="ClientStartLineIndex">Line index of client start position.</param>
      public ClientRun(int ClientStartLineIndex)
      {
         _ClientStartPosition = ClientStartLineIndex;
      }
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
         else if (logLine.Contains("] + Working ..."))
         {
            return LogLineType.WorkUnitWorking;
         }
         else if (logLine.Contains("] *------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
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
         else if (logLine.Contains("] Entering M.D."))
         {
            return true;
         }

         return false;
      }

      public string GetRunArguments(ClientRun ClientRun)
      {
         foreach (LogLine line in this)
         {
            if (line.LineIndex >= ClientRun.ClientStartPosition)
            {
               if (line.LineType.Equals(LogLineType.ClientArguments))
               {
                  return line.LineData.ToString();
               }
            }
         }
         
         return String.Empty;
      }

      public object GetRunUserNameAndTeam(ClientRun ClientRun)
      {
         // Make sure to get the last occurance of User name / Team.
         // When one configures the client the original User name and Team
         // will be logged.  When the config is done, the User name and Team
         // input during the config will be logged.  We want this configed
         // User name and Team.
         object UserNameAndTeam = null;
      
         foreach (LogLine line in this)
         {
            if (line.LineIndex >= ClientRun.ClientStartPosition)
            {
               if (line.LineType.Equals(LogLineType.ClientUserNameTeam))
               {
                  UserNameAndTeam = line.LineData;
               }
            }
         }

         return UserNameAndTeam;
      }

      public string GetRunUserID(ClientRun ClientRun)
      {
         foreach (LogLine line in this)
         {
            if (line.LineIndex >= ClientRun.ClientStartPosition)
            {
               if (line.LineType.Equals(LogLineType.ClientUserID) ||
                   line.LineType.Equals(LogLineType.ClientReceivedUserID))
               {
                  return line.LineData.ToString();
               }
            }
         }

         return ClientInstance.DefaultUserID;
      }

      public int GetRunMachineID(ClientRun ClientRun)
      {
         foreach (LogLine line in this)
         {
            if (line.LineIndex >= ClientRun.ClientStartPosition)
            {
               if (line.LineType.Equals(LogLineType.ClientMachineID))
               {
                  return (int)line.LineData;
               }
            }
         }

         return ClientInstance.DefaultMachineID;
      }
   }
   
   public class LogLine
   {
      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex rUserTeam =
         new Regex("\\[(?<Timestamp>.*)\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rReceivedUserID =
         new Regex("\\[(?<Timestamp>.*)\\].*- Received User ID = (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rUserID =
         new Regex("\\[(?<Timestamp>.*)\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rMachineID =
         new Regex("\\[(?<Timestamp>.*)\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex rCoreVersion =
         new Regex("\\[(?<Timestamp>.*)\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex rProjectNumber =
         new Regex("\\[(?<Timestamp>.*)\\] Project: (?<ProjectNumber>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rCoreShutdown =
         new Regex("\\[(?<Timestamp>.*)\\] Folding@home Core Shutdown: (?<UnitResult>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex rCompletedWUs =
         new Regex("\\[(?<Timestamp>.*)\\] \\+ Number of Units Completed: (?<Completed>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline); 
   
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
      
      public LogLine(LogLineType type, int index, string logLine)
      {
         _LineType = type;
         _LineIndex = index;
         _LineRaw = logLine;
         _LineData = GetLineData(type, logLine);
      }

      private static object GetLineData(LogLineType type, string logLine)
      {
         switch (type)
         {
            case LogLineType.ClientArguments:
               return logLine.Substring(10).Trim();
            case LogLineType.ClientUserNameTeam:
               Match mUserTeam;
               if ((mUserTeam = rUserTeam.Match(logLine)).Success)
               {
                  ArrayList list = new ArrayList(2);
                  list.Add(mUserTeam.Result("${Username}"));
                  list.Add(Int32.Parse(mUserTeam.Result("${TeamNumber}")));
                  return list;
               }
               break;
            case LogLineType.ClientReceivedUserID:
               Match mReceivedUserID;
               if ((mReceivedUserID = rReceivedUserID.Match(logLine)).Success)
               {
                  return mReceivedUserID.Result("${UserID}");
               }
               break;
            case LogLineType.ClientUserID:
               Match mUserID;
               if ((mUserID = rUserID.Match(logLine)).Success)
               {
                  return mUserID.Result("${UserID}");
               }
               break;
            case LogLineType.ClientMachineID:
               Match mMachineID;
               if ((mMachineID = rMachineID.Match(logLine)).Success)
               {
                  return Int32.Parse(mMachineID.Result("${MachineID}"));
               }
               break;
            case LogLineType.WorkUnitCoreVersion:
               Match mCoreVer;
               if ((mCoreVer = rCoreVersion.Match(logLine)).Success)
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
               break;
            case LogLineType.WorkUnitProject:
               Match mProjectNumber;
               if ((mProjectNumber = rProjectNumber.Match(logLine)).Success)
               {
                  return mProjectNumber.Result("${ProjectNumber}");
               }
               break;
            case LogLineType.WorkUnitCoreShutdown:
               Match mCoreShutdown;
               if ((mCoreShutdown = rCoreShutdown.Match(logLine)).Success)
               {
                  return UnitInfo.WorkUnitResultFromString(mCoreShutdown.Result("${UnitResult}"));
               }
               break;
            case LogLineType.ClientNumberOfUnitsCompleted:
               Match mCompletedWUs;
               if ((mCompletedWUs = rCompletedWUs.Match(logLine)).Success)
               {
                  return Int32.Parse(mCompletedWUs.Result("${Completed}"));
               }
               break;
         }
         
         return null;
      }

      public override string ToString()
      {
         return _LineRaw;
      }
   }
}
