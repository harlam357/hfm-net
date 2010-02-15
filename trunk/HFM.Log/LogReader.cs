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
      private readonly LogLineList _ClientLogLines = new LogLineList();
      /// <summary>
      /// List of client log lines.
      /// </summary>
      public LogLineList ClientLogLines
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
      public IFahLogUnitData GetEmptyFahLogUnitData()
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

      private static void PopulateProjectData(ILogLine line, IFahLogUnitData data)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            ProjectInfo info = new ProjectInfo();

            info.ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
            info.ProjectRun = Int32.Parse(match.Result("${Run}"));
            info.ProjectClone = Int32.Parse(match.Result("${Clone}"));
            info.ProjectGen = Int32.Parse(match.Result("${Gen}"));
            
            data.ProjectInfoList.Add(info);
         }
         else
         {
            throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
         }
      }

      /// <summary>
      /// Get an Empty unitinfo Log Data
      /// </summary>
      public IUnitInfoLogData GetEmptyUnitInfoLogData()
      {
         return new UnitInfoLogData();
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
                  data.DueTime = DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss",
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
}
