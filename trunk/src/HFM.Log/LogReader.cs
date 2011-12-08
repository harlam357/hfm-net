/*
 * HFM.NET - Log Reader Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.IO;
using System.Text.RegularExpressions;

using HFM.Core.DataTypes;

namespace HFM.Log
{
   public enum LogFileType
   {
      Legacy,
      FahClient
   }

   /// <summary>
   /// FAH Client Log File Reader.  Gets data from FAHlog.txt and unitinfo.txt files.
   /// </summary>
   public static class LogReader
   {
      #region Fields
      
      private static readonly Regex RegexProjectNumberFromTag =
         new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      #endregion

      #region Methods
      
      /// <summary>
      /// Get FAHlog Unit Data from the given Log Lines
      /// </summary>
      /// <param name="logLines">Log Lines to search</param>
      public static FahLogUnitData GetFahLogDataFromLogLines(IEnumerable<LogLine> logLines)
      {
         var data = new FahLogUnitData();

         if (logLines == null) return data;
         
         bool clientWasPaused = false;
         bool lookForProject = true;
         int queueIndex = -1;
      
         foreach (var line in logLines)
         {
            #region Unit Start

            if ((line.LineType.Equals(LogLineType.WorkUnitProcessing) ||
                 line.LineType.Equals(LogLineType.WorkUnitWorking) ||
                 line.LineType.Equals(LogLineType.WorkUnitStart) ||
                 line.LineType.Equals(LogLineType.WorkUnitFrame)) &&
                 data.UnitStartTimeStamp.Equals(TimeSpan.MinValue))
            {
               data.UnitStartTimeStamp = line.GetTimeStamp() ?? TimeSpan.MinValue;
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
               data.UnitStartTimeStamp = line.GetTimeStamp() ?? TimeSpan.MinValue;
            }

            #endregion
            
            #region Frame Data

            if (line.LineType.Equals(LogLineType.WorkUnitFrame))
            {
               Debug.Assert(line.LineData is UnitFrame);
               data.FramesObserved++;
               data.FrameDataList.Add(line);
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

            if ((line.LineType.Equals(LogLineType.WorkUnitCoreShutdown) && line.LineData != null) ||
                (line.LineType.Equals(LogLineType.ClientCoreCommunicationsError) && line.LineData != null))
            {
               data.UnitResult = (WorkUnitResult)line.LineData;
            }

            if (line.LineType.Equals(LogLineType.WorkUnitWorking) && line.LineData != null)
            {
               queueIndex = (int)line.LineData;
            }
            if (line.LineType.Equals(LogLineType.WorkUnitCoreReturn) && line.LineData != null)
            {
               // make sure the result being captured 
               // is for the correct queue index.
               var unitResult = (UnitResult)line.LineData;
               if (unitResult.Index == queueIndex)
               {
                  data.UnitResult = unitResult.Value;
               }
            }

            #endregion
         }
         
         return data;
      }

      private static void PopulateProjectData(LogLine line, FahLogUnitData data)
      {
         Debug.Assert(line != null);
         Debug.Assert(data != null);
         Debug.Assert(line.LineType.Equals(LogLineType.WorkUnitProject));
         
         var match = (Match)line.LineData;
         var info = new ProjectInfo
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
      /// <exception cref="System.ArgumentException">Throws if logFilePath is null or empty.</exception>
      /// <exception cref="System.IO.IOException">Throws if file specified by logFilePath cannot be read.</exception>
      /// <exception cref="System.FormatException">Throws if log data fails parsing.</exception>
      public static UnitInfoLogData GetUnitInfoLogData(string logFilePath)
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
                  if ((mProjectNumberFromTag = RegexProjectNumberFromTag.Match(data.ProteinTag)).Success)
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
                                                          Default.DateTimeStyle);
               }
               /* DueTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Due time: "))
               {
                  data.DueTime = DateTime.ParseExact(line.Substring(10), "MMMM d H:mm:ss",
                                                     DateTimeFormatInfo.InvariantInfo,
                                                     Default.DateTimeStyle);
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
      /// Determine log line types and data.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="ArgumentNullException">Throws if logFilePath is null.</exception>
      /// <exception cref="ArgumentException">Throws if logFilePath is empty.</exception>
      public static List<LogLine> GetLogLines(string logFilePath)
      {
         return GetLogLines(logFilePath, LogFileType.Legacy);
      }

      /// <summary>
      /// Determine log line types and data.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <param name="logFileType">File Type - Legacy or FahClient</param>
      /// <exception cref="ArgumentNullException">Throws if logFilePath is null.</exception>
      /// <exception cref="ArgumentException">Throws if logFilePath is empty.</exception>
      public static List<LogLine> GetLogLines(string logFilePath, LogFileType logFileType)
      {
         if (logFilePath == null) throw new ArgumentNullException("logFilePath");

         if (logFilePath.Length == 0)
         {
            throw new ArgumentException("Argument 'logFilePath' cannot be an empty string.");
         }

         return GetLogLines(File.ReadAllLines(logFilePath), logFileType);
      }

      /// <summary>
      /// Determine log line types and data.
      /// </summary>
      /// <param name="logLines">List of log lines.</param>
      /// <exception cref="ArgumentNullException">Throws if logLines is null.</exception>
      public static List<LogLine> GetLogLines(IList<string> logLines)
      {
         return GetLogLines(logLines, LogFileType.Legacy);
      }

      /// <summary>
      /// Determine log line types and data.
      /// </summary>
      /// <param name="logLines">List of log lines.</param>
      /// <param name="logFileType">File Type - Legacy or FahClient</param>
      /// <exception cref="ArgumentNullException">Throws if logLines is null.</exception>
      public static List<LogLine> GetLogLines(IList<string> logLines, LogFileType logFileType)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         // Need to clear any previous data before adding new range.
         var logLineList = logFileType.GetLogLineList();
         logLineList.AddRange(logLines);

         return logLineList;
      }

      /// <summary>
      /// Scan the log lines to find client run data and work unit start positions.
      /// </summary>
      /// <param name="logLines">Log lines to scan.</param>
      /// <exception cref="ArgumentNullException">Throws if logLines is null.</exception>
      public static List<ClientRun> GetClientRuns(IList<LogLine> logLines)
      {
         return GetClientRuns(logLines, LogFileType.Legacy);
      }

      /// <summary>
      /// Scan the log lines to find client run data and work unit start positions.
      /// </summary>
      /// <param name="logLines">Log lines to scan.</param>
      /// <param name="logFileType">File Type - Legacy or FahClient</param>
      /// <exception cref="ArgumentNullException">Throws if logLines is null.</exception>
      public static List<ClientRun> GetClientRuns(IList<LogLine> logLines, LogFileType logFileType)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         // Now that we know the LineType for each LogLine, hand off the List
         // of LogLine to the ClientRun List so it can determine the Client 
         // and Unit Start Indexes.
         var clientRunList = logFileType.GetClientRunList();
         clientRunList.Build(logLines);

         return clientRunList;
      }
      
      #endregion
   }

   public static class LogReaderExtensions
   {
      private static readonly Regex TimeStampRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      #region LogLine

      /// <summary>
      /// Get the time stamp from the log line or return null.
      /// </summary>
      public static TimeSpan? GetTimeStamp(this LogLine logLine)
      {
         Match timeStampMatch;
         if ((timeStampMatch = TimeStampRegex.Match(logLine.LineRaw)).Success)
         {
            try
            {
               return ParseTimeStamp(timeStampMatch.Result("${Timestamp}"));
            }
            catch (FormatException)
            {
               
            }
         }

         return null;
      }

      /// <summary>
      /// Get the time stamp from the log line or throw.
      /// </summary>
      /// <exception cref="FormatException">Throws if time stamp string cannot be parsed.</exception>
      public static TimeSpan ParseTimeStamp(this LogLine logLine)
      {
         Match timeStampMatch;
         if ((timeStampMatch = TimeStampRegex.Match(logLine.LineRaw)).Success)
         {
            return ParseTimeStamp(timeStampMatch.Result("${Timestamp}"));
         }

         throw new FormatException(String.Format("Failed to parse Time Stamp Data from '{0}'", logLine.LineRaw));
      }

      private static TimeSpan ParseTimeStamp(string value)
      {
         return DateTime.ParseExact(value, "HH:mm:ss",
                                    DateTimeFormatInfo.InvariantInfo,
                                    Default.DateTimeStyle).TimeOfDay;
      }

      #endregion

      #region LogFileType

      internal static LogLineListBase GetLogLineList(this LogFileType logFileType)
      {
         if (logFileType.Equals(LogFileType.Legacy))
         {
            return (LogLineListBase)Activator.CreateInstance(typeof(LogLineListLegacy), logFileType);
         }
         return (LogLineListBase)Activator.CreateInstance(typeof(LogLineList), logFileType);
      }

      internal static ClientRunListBase GetClientRunList(this LogFileType logFileType)
      {
         if (logFileType.Equals(LogFileType.Legacy))
         {
            return Activator.CreateInstance<ClientRunListLegacy>();
         }
         return Activator.CreateInstance<ClientRunList>();
      }

      internal static LogLineParserBase GetLogLineParser(this LogFileType logFileType)
      {
         if (logFileType.Equals(LogFileType.Legacy))
         {
            return Activator.CreateInstance<LogLineParserLegacy>();
         }
         return Activator.CreateInstance<LogLineParser>();
      }

      #endregion
   }
}
