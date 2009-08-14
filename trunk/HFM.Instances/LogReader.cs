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
      WorkUnitProcessing,
      CoreDownload,
      WorkUnitWorking,
      WorkUnitStart,
      WorkUnitStarted
   }

   /// <summary>
   /// Reads FAHlog.txt files, determines log positions, and does run level detection.
   /// </summary>
   public class LogReader
   {
      #region Members
      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex rUserTeam =
               new Regex("\\[(?<Timestamp>.*)\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

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

      /// <summary>
      /// Local Flag set true once User ID has been found.
      /// </summary>
      private bool _bUserIDFound = false;
      /// <summary>
      /// Local Flag set true once Machine ID has been found.
      /// </summary>
      private bool _bMachineIDFound = false;

      /// <summary>
      /// Local log text array.
      /// </summary>
      private string[] _FAHLogText = null;
      /// <summary>
      /// List of client run positions.
      /// </summary>
      private readonly ClientRunPositionsList _ClientRunPositionsList = new ClientRunPositionsList(); 
      
      public ClientRunPositionsList LogPositions
      {
         get { return _ClientRunPositionsList; }
      }
      #endregion

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
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<string> GetPreviousWorkUnitLog()
      {
         ClientRunPositions lastRun = _ClientRunPositionsList.LastClientRun;
         if (lastRun.UnitStartPositions.Count > 1)
         {
            int start = lastRun.UnitStartPositions[lastRun.UnitStartPositions.Count - 2];
            int end = lastRun.UnitStartPositions[lastRun.UnitStartPositions.Count - 1];
            
            int length = end - start;
            
            string[] logLines = new string[length];
            
            Array.Copy(_FAHLogText, start, logLines, 0, length);
            
            return logLines;
         }
         
         return null;
      }

      /// <summary>
      /// Returns log text of the current work unit.
      /// </summary>
      public IList<string> GetCurrentWorkUnitLog()
      {
         ClientRunPositions lastRun = _ClientRunPositionsList.LastClientRun;
         if (lastRun.UnitStartPositions.Count > 0)
         {
            int start = lastRun.UnitStartPositions[lastRun.UnitStartPositions.Count - 1];
            int end = _FAHLogText.Length;

            int length = end - start;

            string[] logLines = new string[length];

            Array.Copy(_FAHLogText, start, logLines, 0, length);

            return logLines;
         }

         return null;
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
            string logLine = _FAHLogText[i];
            
            LogLineType lineType = DetermineLineType(logLine);
            _ClientRunPositionsList.HandleLineType(lineType, i);
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
         ClientRunPositions clientRun = _ClientRunPositionsList.LastClientRun;
         
         if (clientRun != null)
         {
            int startIndex = clientRun.ClientStartPosition;
            
            for (int i = startIndex; i < _FAHLogText.Length; i++)
            {
               string logLine = _FAHLogText[i];
               
               CheckForUserTeamAndIDs(Instance, logLine);
            
               // Only accumulate Completed and Failed work unit counts on most recent work unit - Issue 35
               if (logLine.Contains("FINISHED_UNIT"))
               {
                  Instance.NumberOfCompletedUnitsSinceLastStart++;
               }
               if ((logLine.Contains("EARLY_UNIT_END") || logLine.Contains("UNSTABLE_MACHINE")))
               {
                  Instance.NumberOfFailedUnitsSinceLastStart++;
               }
            }
         }
      }

      /// <summary>
      /// Inspect the given log line for Folding ID and Team or User ID and Machine ID values.
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      /// <param name="LogLine">The log line being inspected.</param>
      private void CheckForUserTeamAndIDs(ClientInstance Instance, string LogLine)
      {
         Match mUserTeam;
         if ((mUserTeam = rUserTeam.Match(LogLine)).Success)
         {
            Instance.FoldingID = mUserTeam.Result("${Username}");
            Instance.Team = Int32.Parse(mUserTeam.Result("${TeamNumber}"));
         }

         Match mUserID;
         if (_bUserIDFound == false && (mUserID = rUserID.Match(LogLine)).Success)
         {
            Instance.UserID = mUserID.Result("${UserID}");
            _bUserIDFound = true;
         }

         Match mMachineID;
         if (_bMachineIDFound == false && (mMachineID = rMachineID.Match(LogLine)).Success)
         {
            Instance.MachineID = Int32.Parse(mMachineID.Result("${MachineID}"));
            _bMachineIDFound = true;
         }
      }

      /// <summary>
      /// Inspect the given log line and determine the line type.
      /// </summary>
      /// <param name="LogLine">The log line being inspected.</param>
      private static LogLineType DetermineLineType(string LogLine)
      {
         if (LogLine.Contains("--- Opening Log file"))
         {
            return LogLineType.LogOpen;
         }
         else if (LogLine.Contains("###############################################################################"))
         {
            return LogLineType.LogHeader;
         }
         else if (LogLine.Contains("+ Processing work unit"))
         {
            return LogLineType.WorkUnitProcessing;
         }
         else if (LogLine.Contains("+ Downloading new core"))
         {
            return LogLineType.CoreDownload;
         }
         else if (LogLine.Contains("+ Working ..."))
         {
            return LogLineType.WorkUnitWorking;
         }
         else if (LogLine.Contains("*------------------------------*"))
         {
            return LogLineType.WorkUnitStart;
         }
         else if (IsLineTypeWorkUnitStarted(LogLine))
         {
            return LogLineType.WorkUnitStarted;
         }
         else
         {
            return LogLineType.Unknown;
         }
      }

      /// <summary>
      /// Inspect the given log line and determine if the line type is LogLineType.WorkUnitStarted.
      /// </summary>
      /// <param name="LogLine">The log line being inspected.</param>
      private static bool IsLineTypeWorkUnitStarted(string LogLine)
      {
         if (LogLine.Contains("Preparing to commence simulation"))
         {
            return true;
         }
         else if (LogLine.Contains("Called DecompressByteArray"))
         {
            return true;
         }
         else if (LogLine.Contains("- Digital signature verified"))
         {
            return true;
         }
         else if (LogLine.Contains("Entering M.D."))
         {
            return true;
         }
         
         return false;
      }
   }
   
   /// <summary>
   /// List of Client Run Positions.
   /// </summary>
   public class ClientRunPositionsList : List<ClientRunPositions>
   {
      /// <summary>
      /// Local variable containing the current LogLineType.
      /// </summary>
      private LogLineType _CurrentLineType = LogLineType.Unknown;

      /// <summary>
      /// Returns the most recent client run if available, otherwise null.
      /// </summary>
      public ClientRunPositions LastClientRun
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
      /// <param name="LineType">LogLineType to handle.</param>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      public void HandleLineType(LogLineType LineType, int LineIndex)
      {
         switch (LineType)
         {
            case LogLineType.LogOpen:
               HandleLogOpen(LineIndex);
               _CurrentLineType = LineType;
               break;
            case LogLineType.LogHeader:
               HandleLogHeader(LineIndex);
               _CurrentLineType = LineType;
               break;
            case LogLineType.WorkUnitProcessing:
               HandleWorkUnitProcessing(LineIndex);
               _CurrentLineType = LineType;
               break;
            case LogLineType.CoreDownload:
               HandleCoreDownload();
               _CurrentLineType = LineType;
               break;
            case LogLineType.WorkUnitWorking:
               HandleWorkUnitWorking(LineIndex);
               _CurrentLineType = LineType;
               break;
            case LogLineType.WorkUnitStart:
               HandleWorkUnitStart(LineIndex);
               _CurrentLineType = LineType;
               break;
            case LogLineType.WorkUnitStarted:
               //HandleWorkUnitStarted(LineIndex);
               _CurrentLineType = LineType;
               break;
         }
      }

      /// <summary>
      /// Handles LogOpen LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleLogOpen(int LineIndex)
      {
         Add(new ClientRunPositions(LineIndex));
      }

      /// <summary>
      /// Handles LogHeader LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleLogHeader(int LineIndex)
      {
         if (_CurrentLineType.Equals(LogLineType.LogOpen) ||
             _CurrentLineType.Equals(LogLineType.LogHeader)) return;

         Add(new ClientRunPositions(LineIndex));
      }

      /// <summary>
      /// Handles WorkUnitProcessing LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitProcessing(int LineIndex)
      {
         LastClientRun.UnitStartPositions.Add(LineIndex);
      }

      /// <summary>
      /// Handles CoreDownload LogLineType.
      /// </summary>
      private void HandleCoreDownload()
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitProcessing))
         {
            LastClientRun.UnitStartPositions.RemoveAt(LastClientRun.UnitStartPositions.Count - 1);
         }
      }

      /// <summary>
      /// Handles WorkUnitWorking LogLineType.
      /// </summary>
      /// <param name="LineIndex">Index of the given LogLineType.</param>
      private void HandleWorkUnitWorking(int LineIndex)
      {
         if (_CurrentLineType.Equals(LogLineType.WorkUnitProcessing) == false)
         {
            LastClientRun.UnitStartPositions.Add(LineIndex);
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
            LastClientRun.UnitStartPositions.Add(LineIndex);
         }
      }

      //private void HandleWorkUnitStarted(int LineIndex)
      //{
      //   throw new Exception("The method or operation is not implemented.");
      //}
   }
   
   /// <summary>
   /// Holds Positions (Index) for a Single Client Run.
   /// </summary>
   public class ClientRunPositions
   {
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
      public ClientRunPositions(int ClientStartLineIndex)
      {
         _ClientStartPosition = ClientStartLineIndex;
      }
   }
}
