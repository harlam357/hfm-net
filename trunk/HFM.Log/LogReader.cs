/*
 * HFM.NET - Log Reader Class
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Log
{
   /// <summary>
   /// Reads FAHlog.txt files, determines log positions, and does run level detection.
   /// </summary>
   public class LogReader : ILogReader
   {
      #region Members
      private readonly Regex rTimeStamp =
            new Regex("\\[(?<Timestamp>.{8})\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
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
      /// Returns the last client run data.
      /// </summary>
      public IClientRun CurrentClientRun
      {
         get { return ClientRunList.CurrentClientRun; }
      }
      
      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<ILogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _ClientRunList.CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitStartIndex.Count > 1)
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
            ClientRun lastClientRun = _ClientRunList.CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitStartIndex.Count > 0)
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
      /// Get an Empty FAHlog Unit Data
      /// </summary>
      public IFahLogUnitData GetEmptyFahLogData()
      {
         return new FahLogUnitData();
      }

      /// <summary>
      /// Get FAHlog Unit Data from the given Log Lines
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      public IFahLogUnitData GetFahLogDataFromLogLines(ICollection<ILogLine> logLines)
      {
         FahLogUnitData data = new FahLogUnitData();

         if (logLines == null) return data;
         
         bool ClientWasPaused = false;
         bool LookForProject = true;
      
         foreach (ILogLine line in logLines)
         {
            #region Unit Start
            if (data.UnitStartTimeStamp.Equals(TimeSpan.MinValue))
            {
               data.UnitStartTimeStamp = GetLogLineTimeStamp(line);
            }
            
            if (line.LineType.Equals(LogLineType.WorkUnitPaused)) // || logLine.LineRaw.Contains("+ Running on battery power"))
            {
               ClientWasPaused = true;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitWorking) && ClientWasPaused)
            {
               ClientWasPaused = false;

               // Clear the Current Frame (Also Resets Frames Observed Count)
               // This will cause the Instance to only use frames beyond this point to 
               // set frame times and determine status - Issue 13 (Revised)
               data.FramesObserved = 0;
               // Reset the Unit Start Time
               data.UnitStartTimeStamp = GetLogLineTimeStamp(line);
            }
            #endregion
            
            #region Frame Data
            if (line.LineType.Equals(LogLineType.WorkUnitFrame))
            {
               IFrameData frame = line.LineData as IFrameData;
               if (frame == null)
               {
                  // If not found, clear the LineType and get out
                  line.LineType = LogLineType.Unknown;
               }
               else
               {
                  data.FramesObserved++;
                  data.FrameDataList.Add(line);
               }
            }
            #endregion

            #region Core Version
            if (data.CoreVersion.Length == 0)
            {
               if (line.LineType.Equals(LogLineType.WorkUnitCoreVersion) && line.LineData != null)
               {
                  data.CoreVersion = line.LineData.ToString();
               }
            }
            #endregion

            #region Project
            if (LookForProject)
            {
               // If we encounter a work unit frame, we should have
               // already seen the Project Information, stop looking
               if (line.LineType.Equals(LogLineType.WorkUnitFrame))
               {
                  LookForProject = false;
               }
               if (line.LineType.Equals(LogLineType.WorkUnitProject))
               {
                  PopulateProjectData(line, data);
                  LookForProject = false;
               }
            }
            #endregion
            
            #region Unit Result
            if (line.LineType.Equals(LogLineType.WorkUnitCoreShutdown) && line.LineData != null)
            {
               data.UnitResult = (WorkUnitResult)line.LineData;
            }
            #endregion

            #region Client Status
            if (data.Status.Equals(ClientStatus.Unknown) &&
               (line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                line.LineType.Equals(LogLineType.WorkUnitStart)))
            {
               data.Status = ClientStatus.RunningNoFrameTimes;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitPaused)) // || line.LineRaw.Contains("+ Running on battery power"))
            {
               data.Status = ClientStatus.Paused;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitWorking) && data.Status.Equals(ClientStatus.Paused))
            {
               data.Status = ClientStatus.RunningNoFrameTimes;
            }
            else if (line.LineType.Equals(LogLineType.ClientSendWorkToServer))
            {
               data.Status = ClientStatus.SendingWorkPacket;
            }
            else if (line.LineType.Equals(LogLineType.ClientAttemptGetWorkPacket))
            {
               data.Status = ClientStatus.GettingWorkPacket;
            }
            else if (line.LineType.Equals(LogLineType.ClientEuePauseState))
            {
               data.Status = ClientStatus.EuePause;
            }
            else if (line.LineType.Equals(LogLineType.ClientShutdown) ||
                     line.LineType.Equals(LogLineType.ClientCoreCommunicationsErrorShutdown))
            {
               data.Status = ClientStatus.Stopped;
            }
            #endregion
         }
         
         //TODO: Fix This! Convert back to Zero TimeSpan, calling code still expects this for now.
         if (data.UnitStartTimeStamp.Equals(TimeSpan.MinValue))
         {
            data.UnitStartTimeStamp = TimeSpan.Zero;
         }
         
         return data;
      }

      /// <summary>
      /// Get the time stamp from this log line and set as the unit's start time
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private TimeSpan GetLogLineTimeStamp(ILogLine logLine)
      {
         Match mTimeStamp;
         if ((mTimeStamp = rTimeStamp.Match(logLine.LineRaw)).Success)
         {
            try
            {
               DateTime timeStamp = DateTime.ParseExact(mTimeStamp.Result("${Timestamp}"), "HH:mm:ss",
                                                        System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                        PlatformOps.GetDateTimeStyle());

               return timeStamp.TimeOfDay;
            }
            catch (FormatException)
            { }
         }
         
         return TimeSpan.MinValue;
      }

      private static void PopulateProjectData(ILogLine line, FahLogUnitData data)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            data.ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
            data.ProjectRun = Int32.Parse(match.Result("${Run}"));
            data.ProjectClone = Int32.Parse(match.Result("${Clone}"));
            data.ProjectGen = Int32.Parse(match.Result("${Gen}"));
         }
         else
         {
            throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
         }
      }

      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="LogFilePath">Path to the log file.</param>
      public IUnitInfoLogData GetUnitInfoLogData(string LogFilePath)
      {
         return GetUnitInfoLogData(String.Empty, LogFilePath);
      }

      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="InstanceName">Name of the Client Instance that called this method.</param>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentNullException">Throws if InstanceName is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LogFilePath is Null or Empty.</exception>
      public IUnitInfoLogData GetUnitInfoLogData(string InstanceName, string LogFilePath)
      {
         if (InstanceName == null) throw new ArgumentNullException("InstanceName", "Argument 'InstanceName' cannot be null.");

         if (String.IsNullOrEmpty(LogFilePath))
         {
            throw new ArgumentException("Argument 'LogFileName' cannot be a null or empty string.");
         }
      
         if (File.Exists(LogFilePath) == false) return null;

         DateTime Start = HfmTrace.ExecStart;

         UnitInfoLogData data = new UnitInfoLogData();

         TextReader tr = null;
         try
         {
            tr = File.OpenText(LogFilePath);

            while (tr.Peek() != -1)
            {
               String sData = tr.ReadLine();
               /* Name (Only Read Here) */
               if (sData.StartsWith("Name: "))
               {
                  data.ProteinName = sData.Substring(6);
               }
               /* Tag (Could be read here or through the queue.dat) */
               else if (sData.StartsWith("Tag: "))
               {
                  data.ProteinTag = sData.Substring(5);

                  Match mProjectNumberFromTag;
                  if ((mProjectNumberFromTag = rProjectNumberFromTag.Match(data.ProteinTag)).Success)
                  {
                     try
                     {
                        data.ProjectID = Int32.Parse(mProjectNumberFromTag.Result("${ProjectNumber}"));
                        data.ProjectRun = Int32.Parse(mProjectNumberFromTag.Result("${Run}"));
                        data.ProjectClone = Int32.Parse(mProjectNumberFromTag.Result("${Clone}"));
                        data.ProjectGen = Int32.Parse(mProjectNumberFromTag.Result("${Gen}"));
                     }
                     catch (FormatException ex)
                     {
                        HfmTrace.WriteToHfmConsole(TraceLevel.Warning, InstanceName, ex);

                        data.ProjectID = 0;
                        data.ProjectRun = 0;
                        data.ProjectClone = 0;
                        data.ProjectGen = 0;
                     }
                  }
               }
               /* DownloadTime (Could be read here or through the queue.dat) */
               else if (sData.StartsWith("Download time: "))
               {
                  data.DownloadTime = DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                                          System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                          PlatformOps.GetDateTimeStyle());
               }
               /* DueTime (Could be read here or through the queue.dat) */
               else if (sData.StartsWith("Due time: "))
               {
                  data.DueTime = DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                                     System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                     PlatformOps.GetDateTimeStyle());
               }
               /* Progress (Supplemental Read - if progress percentage cannot be determined through FAHlog.txt) */
               else if (sData.StartsWith("Progress: "))
               {
                  data.Progress = Int32.Parse(sData.Substring(10, sData.IndexOf("%") - 10));
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(InstanceName, ex);
            return null;
         }
         finally
         {
            if (tr != null)
            {
               tr.Dispose();
            }

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, InstanceName, Start);
         }

         return data;
      }

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if LogFileName is Null or Empty.</exception>
      public void ScanFAHLog(string LogFilePath)
      {
         ScanFAHLog(String.Empty, LogFilePath);
      }

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="InstanceName">Name of the Client Instance that called this method.</param>
      /// <param name="LogFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentNullException">Throws if InstanceName is Null.</exception>
      /// <exception cref="ArgumentException">Throws if LogFilePath is Null or Empty.</exception>
      public void ScanFAHLog(string InstanceName, string LogFilePath)
      {
         if (InstanceName == null) throw new ArgumentNullException("InstanceName", "Argument 'InstanceName' cannot be null.");
      
         if (String.IsNullOrEmpty(LogFilePath))
         {
            throw new ArgumentException("Argument 'LogFileName' cannot be a null or empty string.");
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
         if (CurrentClientRun != null)
         {
            // If we've already seen a WorkUnitRunning line, ignore this one.
            if (_CurrentLineType.Equals(LogLineType.WorkUnitRunning)) return;

            if (_UnitStart.WorkUnitProcessingIndex > -1)
            {
               CurrentClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitProcessingIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               CurrentClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
            }
            else if (_UnitStart.WorkUnitWorkingIndex > -1)
            {
               CurrentClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitWorkingIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               CurrentClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
            }
            else if (_UnitStart.WorkUnitStartIndex > -1)
            {
               CurrentClientRun.UnitStartIndex.Add(_UnitStart.WorkUnitStartIndex);
               // Set the Queue Slot - we don't care if we found a valid slot or not
               CurrentClientRun.UnitQueueIndex.Add(_UnitStart.WorkUnitQueueSlotIndex);
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
            if (CurrentClientRun != null)
            {
               if (logLine.LineData.Equals(WorkUnitResult.FinishedUnit))
               {
                  CurrentClientRun.NumberOfCompletedUnits++;
               }
               else if (logLine.LineData.Equals(WorkUnitResult.EarlyUnitEnd) ||
                        logLine.LineData.Equals(WorkUnitResult.UnstableMachine) ||
                        logLine.LineData.Equals(WorkUnitResult.Interrupted) ||
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
   /// List of Client Log Lines.
   /// </summary>
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
   
   public class FahLogUnitData : IFahLogUnitData
   {
      private TimeSpan _UnitStartTimeStamp = TimeSpan.MinValue;
      /// <summary>
      /// Unit Starting Time Stamp
      /// </summary>
      public TimeSpan UnitStartTimeStamp
      {
         get { return _UnitStartTimeStamp; }
         set { _UnitStartTimeStamp = value; }
      }

      private IList<ILogLine> _FrameDataList = new List<ILogLine>(101);
      /// <summary>
      /// List of Log Lines containing Frame Data
      /// </summary>
      public IList<ILogLine> FrameDataList
      {
         get { return _FrameDataList; }
         set { _FrameDataList = value; }
      }

      private Int32 _FramesObserved;
      /// <summary>
      /// Number of Frames Observed since Last Unit Start
      /// </summary>
      public Int32 FramesObserved
      {
         get { return _FramesObserved; }
         set { _FramesObserved = value; }
      }
   
      private string _CoreVersion = String.Empty;
      /// <summary>
      /// Core Version
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
         set { _CoreVersion = value; }
      }
   
      private Int32 _ProjectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; }
      }

      private Int32 _ProjectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get { return _ProjectRun; }
         set { _ProjectRun = value; }
      }

      private Int32 _ProjectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get { return _ProjectClone; }
         set { _ProjectClone = value; }
      }

      private Int32 _ProjectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get { return _ProjectGen; }
         set { _ProjectGen = value; }
      }
      
      private WorkUnitResult _UnitResult = WorkUnitResult.Unknown;
      /// <summary>
      /// Work Unit Result
      /// </summary>
      public WorkUnitResult UnitResult
      {
         get { return _UnitResult; }
         set { _UnitResult = value; }
      }

      private ClientStatus _Status = ClientStatus.Unknown;
      /// <summary>
      /// Client Status
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
         set { _Status = value; }
      }
   }
   
   public class UnitInfoLogData : IUnitInfoLogData
   {
      private string _ProteinName;
      /// <summary>
      /// Protein Name
      /// </summary>
      public string ProteinName
      {
         get { return _ProteinName; }
         set { _ProteinName = value; }
      }
      
      private string _ProteinTag;
      /// <summary>
      /// Protein Tag
      /// </summary>
      public string ProteinTag
      {
         get { return _ProteinTag; }
         set { _ProteinTag = value; }
      }

      private Int32 _ProjectID;
      /// <summary>
      /// Project ID Number
      /// </summary>
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; }
      }

      private Int32 _ProjectRun;
      /// <summary>
      /// Project ID (Run)
      /// </summary>
      public Int32 ProjectRun
      {
         get { return _ProjectRun; }
         set { _ProjectRun = value; }
      }

      private Int32 _ProjectClone;
      /// <summary>
      /// Project ID (Clone)
      /// </summary>
      public Int32 ProjectClone
      {
         get { return _ProjectClone; }
         set { _ProjectClone = value; }
      }

      private Int32 _ProjectGen;
      /// <summary>
      /// Project ID (Gen)
      /// </summary>
      public Int32 ProjectGen
      {
         get { return _ProjectGen; }
         set { _ProjectGen = value; }
      }

      private DateTime _DownloadTime;
      /// <summary>
      /// Download Time
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
         set { _DownloadTime = value; }
      }

      private DateTime _DueTime;
      /// <summary>
      /// Due Time
      /// </summary>
      public DateTime DueTime
      {
         get { return _DueTime; }
         set { _DueTime = value; }
      }

      private int _Progress;
      /// <summary>
      /// Progress Percentage
      /// </summary>
      public int Progress
      {
         get { return _Progress; }
         set { _Progress = value; }
      }
   }
}
