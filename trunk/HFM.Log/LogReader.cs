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
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Log
{
   public interface ILogReader
   {
      /// <summary>
      /// Returns the last client run data.
      /// </summary>
      ClientRun CurrentClientRun { get; }

      /// <summary>
      /// Returns log text of the current client run.
      /// </summary>
      IList<LogLine> CurrentClientRunLogLines { get; }

      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      IList<LogLine> PreviousWorkUnitLogLines { get; }

      /// <summary>
      /// Returns log text of the current work unit.
      /// </summary>
      IList<LogLine> CurrentWorkUnitLogLines { get; }

      /// <summary>
      /// Get a list of Log Lines that correspond to the given Queue Index.
      /// </summary>
      /// <param name="queueIndex">The Queue Index (0-9)</param>
      IList<LogLine> GetLogLinesFromQueueIndex(int queueIndex);

      /// <summary>
      /// Get FAHlog Unit Data from the given Log Lines
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      FahLogUnitData GetFahLogDataFromLogLines(ICollection<LogLine> logLines);

      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="System.ArgumentException">Throws if logFilePath is Null or Empty.</exception>
      /// <exception cref="System.IO.IOException">Throws if file specified by logFilePath cannot be read.</exception>
      /// <exception cref="System.FormatException">Throws if log data fails parsing.</exception>
      UnitInfoLogData GetUnitInfoLogData(string logFilePath);

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if logFilePath is Null or Empty.</exception>
      void ScanFahLog(string logFilePath);
   }

   /// <summary>
   /// Reads FAHlog.txt files, determines log positions, and does run level detection.
   /// </summary>
   public class LogReader : ILogReader
   {
      #region Fields
      
      private readonly Regex _rTimeStamp =
            new Regex("\\[(?<Timestamp>.{8})\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex _rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
      /// <summary>
      /// List of client run positions.
      /// </summary>
      private readonly ClientRunList _clientRunList = new ClientRunList(); 
      /// <summary>
      /// List of client run positions.
      /// </summary>
      public List<ClientRun> ClientRunList
      {
         get { return _clientRunList; }
      }

      /// <summary>
      /// List of client log lines.
      /// </summary>
      private readonly LogLineList _logLineList = new LogLineList();
      /// <summary>
      /// List of client log lines.
      /// </summary>
      public List<LogLine> LogLineList
      {
         get { return _logLineList; }
      }
      
      #endregion

      #region Properties
      
      /// <summary>
      /// Returns the last client run data.
      /// </summary>
      public ClientRun CurrentClientRun
      {
         get { return _clientRunList.CurrentClientRun; }
      }

      /// <summary>
      /// Returns log text of the current client run.
      /// </summary>
      public IList<LogLine> CurrentClientRunLogLines
      {
         get
         {
            ClientRun lastClientRun = _clientRunList.CurrentClientRun;
            if (lastClientRun != null)
            {
               int start = lastClientRun.ClientStartIndex;
               int end = _logLineList.Count;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

               return logLines;
            }

            return null;
         }
      }
      
      /// <summary>
      /// Returns log text of the previous work unit.
      /// </summary>
      public IList<LogLine> PreviousWorkUnitLogLines
      {
         get
         {
            ClientRun lastClientRun = _clientRunList.CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitIndexes.Count > 1)
            {
               int start = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 2].StartIndex;
               int end = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 1].StartIndex;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

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
            ClientRun lastClientRun = _clientRunList.CurrentClientRun;
            if (lastClientRun != null && lastClientRun.UnitIndexes.Count > 0)
            {
               int start = lastClientRun.UnitIndexes[lastClientRun.UnitIndexes.Count - 1].StartIndex;
               int end = _logLineList.Count;

               int length = end - start;

               var logLines = new LogLine[length];

               _logLineList.CopyTo(start, logLines, 0, length);

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
      /// <param name="queueIndex">The Queue Index (0-9)</param>
      public IList<LogLine> GetLogLinesFromQueueIndex(int queueIndex)
      {
         // walk backwards through the ClientRunList and then backward
         // through the UnitQueueIndex list.  Find the first (really last
         // because we're itterating in reverse) UnitQueueIndex that matches
         // the given queueIndex.
         for (int i = ClientRunList.Count - 1; i >= 0; i--)
         {
            for (int j = ClientRunList[i].UnitIndexes.Count - 1; j >= 0; j--)
            {
               // if a match is found
               if (ClientRunList[i].UnitIndexes[j].QueueIndex == queueIndex)
               {
                  // set the unit start position
                  int start = ClientRunList[i].UnitIndexes[j].StartIndex;
                  int end = DetermineEndPosition(i, j);

                  int length = end - start;

                  var logLines = new LogLine[length];

                  _logLineList.CopyTo(start, logLines, 0, length);

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
            if (j == ClientRunList[i].UnitIndexes.Count - 1)
            {
               // use the last line index as the end position
               return _logLineList.Count;
            }
            else // we're working on a unit prior to the last
            {
               // use the unit start position for the next unit
               return ClientRunList[i].UnitIndexes[j + 1].StartIndex;
            }
         }
         else // we're working on a client run prior to the last
         {
            // we're workin on the last unit in the run
            if (j == ClientRunList[i].UnitIndexes.Count - 1)
            {
               // use the client start position for the next client run
               return ClientRunList[i + 1].ClientStartIndex;
            }
            else
            {
               // use the unit start position for the next unit
               return ClientRunList[i].UnitIndexes[j + 1].StartIndex;
            }
         }
      }

      /// <summary>
      /// Get FAHlog Unit Data from the given Log Lines
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      public FahLogUnitData GetFahLogDataFromLogLines(ICollection<LogLine> logLines)
      {
         var data = new FahLogUnitData();

         if (logLines == null) return data;
         
         bool clientWasPaused = false;
         bool lookForProject = true;
      
         foreach (var line in logLines)
         {
            #region Unit Start
            if ((line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                 line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                 line.LineType.Equals(LogLineType.WorkUnitStart) ||
                 line.LineType.Equals(LogLineType.WorkUnitFrame)) &&
                 data.UnitStartTimeStamp.Equals(TimeSpan.MinValue))
            {
               data.UnitStartTimeStamp = GetLogLineTimeStamp(line);
            }

            if (line.LineType.Equals(LogLineType.WorkUnitPaused) || 
                line.LineType.Equals(LogLineType.WorkUnitPausedForBattery))
            {
               clientWasPaused = true;
            }
            else if ((line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                      line.LineType.Equals(LogLineType.WorkUnitResumeFromBattery) ||
                      line.LineType.Equals(LogLineType.WorkUnitFrame)) &&
                      clientWasPaused)
            {
               clientWasPaused = false;

               // Reset the Frames Observed Count
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
               var frame = line.LineData as UnitFrame;
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
            if (lookForProject)
            {
               // If we encounter a work unit frame, we should have
               // already seen the Project Information, stop looking
               if (line.LineType.Equals(LogLineType.WorkUnitFrame))
               {
                  lookForProject = false;
               }
               if (line.LineType.Equals(LogLineType.WorkUnitProject))
               {
                  PopulateProjectData(line, data);
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
            if (line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                line.LineType.Equals(LogLineType.WorkUnitStart) ||
                line.LineType.Equals(LogLineType.WorkUnitFrame) ||
                line.LineType.Equals(LogLineType.WorkUnitResumeFromBattery))
            {
               data.Status = ClientStatus.RunningNoFrameTimes;
            }
            else if (line.LineType.Equals(LogLineType.WorkUnitPaused) || 
                     line.LineType.Equals(LogLineType.WorkUnitPausedForBattery))
            {
               data.Status = ClientStatus.Paused;
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
         
         return data;
      }

      /// <summary>
      /// Get the time stamp from this log line and set as the unit's start time
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private TimeSpan GetLogLineTimeStamp(ILogLine logLine)
      {
         Match mTimeStamp;
         if ((mTimeStamp = _rTimeStamp.Match(logLine.LineRaw)).Success)
         {
            try
            {
               DateTime timeStamp = DateTime.ParseExact(mTimeStamp.Result("${Timestamp}"), "HH:mm:ss",
                                                        DateTimeFormatInfo.InvariantInfo,
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
         Debug.Assert(line != null);
         Debug.Assert(data != null);
      
         if (line.LineType.Equals(LogLineType.WorkUnitProject) == false)
         {
            throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
         }
         
         Match match = (Match)line.LineData;

         ProjectInfo info = new ProjectInfo
                            {
                               ProjectID = Int32.Parse(match.Result("${ProjectNumber}")),
                               ProjectRun = Int32.Parse(match.Result("${Run}")),
                               ProjectClone = Int32.Parse(match.Result("${Clone}")),
                               ProjectGen = Int32.Parse(match.Result("${Gen}"))
                            };

         data.ProjectInfoList.Add(info);
      }

      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="System.ArgumentException">Throws if logFilePath is Null or Empty.</exception>
      /// <exception cref="System.IO.IOException">Throws if file specified by logFilePath cannot be read.</exception>
      /// <exception cref="System.FormatException">Throws if log data fails parsing.</exception>
      public UnitInfoLogData GetUnitInfoLogData(string logFilePath)
      {
         if (String.IsNullOrEmpty(logFilePath))
         {
            throw new ArgumentException("Argument 'logFilePath' cannot be a null or empty string.");
         }

         string[] logLines;
         try
         {
            logLines = File.ReadAllLines(logFilePath);
         }
         catch (Exception ex)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture, "Failed to read file '{0}'", logFilePath), ex);
         }

         var data = new UnitInfoLogData();

         string line = null;
         try
         {
            foreach (string s in logLines)
            {
               line = s;
            
               /* Name (Only Read Here) */
               if (line.StartsWith("Name: "))
               {
                  data.ProteinName = line.Substring(6);
               }
               /* Tag (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Tag: "))
               {
                  data.ProteinTag = line.Substring(5);

                  Match mProjectNumberFromTag;
                  if ((mProjectNumberFromTag = _rProjectNumberFromTag.Match(data.ProteinTag)).Success)
                  {
                     data.ProjectID = Int32.Parse(mProjectNumberFromTag.Result("${ProjectNumber}"));
                     data.ProjectRun = Int32.Parse(mProjectNumberFromTag.Result("${Run}"));
                     data.ProjectClone = Int32.Parse(mProjectNumberFromTag.Result("${Clone}"));
                     data.ProjectGen = Int32.Parse(mProjectNumberFromTag.Result("${Gen}"));
                  }
               }
               /* DownloadTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Download time: "))
               {
                  data.DownloadTime = DateTime.ParseExact(line.Substring(15), "MMMM d H:mm:ss",
                                                          DateTimeFormatInfo.InvariantInfo,
                                                          PlatformOps.GetDateTimeStyle());
               }
               /* DueTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Due time: "))
               {
                  data.DueTime = DateTime.ParseExact(line.Substring(10), "MMMM d H:mm:ss",
                                                     DateTimeFormatInfo.InvariantInfo,
                                                     PlatformOps.GetDateTimeStyle());
               }
               /* Progress (Supplemental Read - if progress percentage cannot be determined through FAHlog.txt) */
               else if (line.StartsWith("Progress: "))
               {
                  data.Progress = Int32.Parse(line.Substring(10, line.IndexOf("%") - 10));
               }
            }
         }
         catch (Exception ex)
         {
            throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Failed to parse line '{0}'", line), ex);
         }

         return data;
      }

      /// <summary>
      /// Scan the FAHLog text lines to determine work unit boundries.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentException">Throws if logFilePath is Null or Empty.</exception>
      public void ScanFahLog(string logFilePath)
      {
         if (String.IsNullOrEmpty(logFilePath))
         {
            throw new ArgumentException("Argument 'logFilePath' cannot be a null or empty string.");
         }

         string[] fahLogText = File.ReadAllLines(logFilePath);

         // Need to clear any previous data before scanning.  
         // This Scan could be called multiple times on the same instance.
         _logLineList.Clear();
         _clientRunList.Clear();
         
         _logLineList.AddRange(fahLogText);

         // Now that we know the LineType for each LogLine, hand off the List
         // of LogLine to the ClientRun List so it can determine the Client 
         // and Unit Start Indexes.

         _clientRunList.HandleLogLines(_logLineList);

         DoRunLevelDetection();
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
               end = LogLineList.Count;
            }
            else // we're working on a client run prior to the last
            {
               // use the client start position for the next client run
               end = ClientRunList[i + 1].ClientStartIndex;
            }
         
            for (int j = ClientRunList[i].ClientStartIndex; j < end; j++)
            {
               if (LogLineList[j].LineType.Equals(LogLineType.ClientVersion))
               {
                  ClientRunList[i].ClientVersion = LogLineList[j].LineData.ToString();
               }
               else if (LogLineList[j].LineType.Equals(LogLineType.ClientArguments))
               {
                  ClientRunList[i].Arguments = LogLineList[j].LineData.ToString();
               }
               else if (LogLineList[j].LineType.Equals(LogLineType.ClientUserNameTeam))
               {
                  ArrayList userAndTeam = (ArrayList)LogLineList[j].LineData;
                  ClientRunList[i].FoldingID = userAndTeam[0].ToString();
                  ClientRunList[i].Team = (int)userAndTeam[1];
               }
               else if (LogLineList[j].LineType.Equals(LogLineType.ClientUserID) ||
                        LogLineList[j].LineType.Equals(LogLineType.ClientReceivedUserID))
               {
                  ClientRunList[i].UserID = LogLineList[j].LineData.ToString();
               }
               else if (LogLineList[j].LineType.Equals(LogLineType.ClientMachineID))
               {
                  ClientRunList[i].MachineID = (int)LogLineList[j].LineData;
               }
            }
         }
      }
      #endregion
   }
}
