/*
 * HFM.NET - Log Parser Class
 * Copyright (C) 2006 David Rawling
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

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Log
{
   public sealed class LogParser
   {
      #region Members
      private readonly string _InstanceName;
      private readonly bool _ClientIsOnVirtualMachine;
      private readonly IUnitInfo _parsedUnitInfo;
      
      private readonly Regex rTimeStamp =
            new Regex("\\[(?<Timestamp>.{8})\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
      private readonly Regex rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private bool _bCoreFound;
      private bool _bProjectFound;
      #endregion

      /// <summary>
      /// Class inspects LogLines and unitinfo.txt file to populate the given UnitInfo object.
      /// </summary>
      /// <param name="InstanceName">Client Instance Name that owns the log file we're parsing.</param>
      /// <param name="ClientIsOnVirtualMachine">Client on VM (Times as UTC) Flag.</param>
      /// <param name="parsedUnitInfo">Container for parsed information.</param>
      public LogParser(string InstanceName, bool ClientIsOnVirtualMachine, IUnitInfo parsedUnitInfo)
      {
         _InstanceName = InstanceName;
         _ClientIsOnVirtualMachine = ClientIsOnVirtualMachine;
         _parsedUnitInfo = parsedUnitInfo;
      }

      #region Parsing Methods
      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="LogFileName">Full path to local copy of unitinfo.txt.</param>
      public bool ParseUnitInfoFile(string LogFileName)
      {
         if (File.Exists(LogFileName) == false) return false;

         DateTime Start = HfmTrace.ExecStart;

         TextReader tr = null;
         try
         {
            tr = File.OpenText(LogFileName);

            while (tr.Peek() != -1)
            {
               String sData = tr.ReadLine();
               /* Name (Only Read Here) */
               if (sData.StartsWith("Name: "))
               {
                  _parsedUnitInfo.ProteinName = sData.Substring(6);
               }
               /* Tag (Could be read here or Previously by through the queue.dat) */
               else if (sData.StartsWith("Tag: ") && _parsedUnitInfo.ProteinTagUnknown)
               {
                  _parsedUnitInfo.ProteinTag = sData.Substring(5);
                  
                  // If we don't know the ProjectID yet
                  if (_parsedUnitInfo.ProjectIsUnknown)
                  {
                     try
                     {
                        _parsedUnitInfo.DoProjectIDMatch(rProjectNumberFromTag.Match(_parsedUnitInfo.ProteinTag));
                     }
                     catch (FormatException ex)
                     {
                        HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _InstanceName, ex);
                     }
                  }
               }
               /* DownloadTime (Could be read here or Previously by through the queue.dat) */
               else if (sData.StartsWith("Download time: ") && _parsedUnitInfo.DownloadTimeUnknown)
               {
                  if (_ClientIsOnVirtualMachine)
                  {
                     _parsedUnitInfo.DownloadTime =
                        DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo);
                  }
                  else
                  {
                     _parsedUnitInfo.DownloadTime =
                        DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                            System.Globalization.DateTimeStyles.AssumeUniversal);
                  }
               }
               /* DueTime (Could be read here or Previously by through the queue.dat) */
               else if (sData.StartsWith("Due time: ") && _parsedUnitInfo.DueTimeUnknown)
               {
                  if (_ClientIsOnVirtualMachine)
                  {
                     _parsedUnitInfo.DueTime =
                        DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo);
                  }
                  else
                  {
                     _parsedUnitInfo.DueTime =
                        DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                            System.Globalization.DateTimeStyles.AssumeUniversal);
                  }
               }
               /* Progress (Supplemental Read - if progress percentage cannot be determined through FAHlog.txt) */
               //else if (sData.StartsWith("Progress: "))
               //{
               //   _parsedUnitInfo.PercentComplete = Int32.Parse(sData.Substring(10, sData.IndexOf("%") - 10));
               //}
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(_InstanceName, ex);
            return false;
         }
         finally
         {
            if (tr != null)
            {
               tr.Dispose();
            }

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _InstanceName, Start);
         }

         return true;
      }

      /// <summary>
      /// Itterates through the given list of LogLines and parses the desired information.
      /// </summary>
      /// <param name="FAHLog">List of LogLines to Parse.</param>
      public void ParseFAHLog(IList<ILogLine> FAHLog)
      {
         if (FAHLog == null) throw new ArgumentNullException("FAHLog", "Argument 'FAHLog' cannot be null.");
         if (FAHLog.Count == 0) throw new ArgumentException("Argument 'FAHLog' cannot have zero elements.");
      
         DateTime Start = HfmTrace.ExecStart;

         _bCoreFound = false;
         // Set Flag based on if Project is already Known
         _bProjectFound = !(_parsedUnitInfo.ProjectIsUnknown);
         
         bool ClientWasPaused = false;

         // start the parse loop where the client started last
         foreach (LogLine logLine in FAHLog)
         {
            if (_parsedUnitInfo.UnitStartTimeStamp.Equals(TimeSpan.Zero))
            {
               SetUnitStartTimeStamp(logLine.LineRaw);
            }
            CheckForCoreVersion(logLine);
            CheckForProjectID(logLine);
            CheckForCompletedFrame(logLine);

            if (logLine.LineType.Equals(LogLineType.WorkUnitPaused)) // || logLine.LineRaw.Contains("+ Running on battery power"))
            {
               ClientWasPaused = true;
            }
            else if (logLine.LineType.Equals(LogLineType.WorkUnitWorking) && ClientWasPaused)
            {
               ClientWasPaused = false;

               // Clear the Current Frame (Also Resets Frames Observed Count)
               // This will cause the Instance to only use frames beyond this point to 
               // set frame times and determine status - Issue 13 (Revised)
               _parsedUnitInfo.ClearCurrentFrame();
               // Reset the Unit Start Time
               SetUnitStartTimeStamp(logLine.LineRaw); 
            }
            else if (logLine.LineType.Equals(LogLineType.WorkUnitCoreShutdown) && logLine.LineData != null)
            {
               _parsedUnitInfo.UnitResult = (WorkUnitResult)logLine.LineData;
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _InstanceName, Start);
      }
      #endregion

      #region Parsing Helpers
      /// <summary>
      /// Get the time stamp from this log line and set as the unit's start time
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void SetUnitStartTimeStamp(string logLine)
      {
         Match mTimeStamp;
         if ((mTimeStamp = rTimeStamp.Match(logLine)).Success)
         {
            try
            {
               DateTime timeStamp = DateTime.ParseExact(mTimeStamp.Result("${Timestamp}"), "HH:mm:ss",
                                                        System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                        GetDateTimeStyle(_ClientIsOnVirtualMachine));

               _parsedUnitInfo.UnitStartTimeStamp = timeStamp.TimeOfDay;
            }
            catch (FormatException)
            {
               _parsedUnitInfo.UnitStartTimeStamp = TimeSpan.Zero;
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) Failed to get 'UnitStartTime' from {2}.",
                  HfmTrace.FunctionName, _InstanceName, logLine));
            }
         }
      }
      
      /// <summary>
      /// Check the given log line for Core version information
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForCoreVersion(ILogLine logLine)
      {
         if (_bCoreFound == false && logLine.LineType.Equals(LogLineType.WorkUnitCoreVersion) && logLine.LineData != null)
         {
            _parsedUnitInfo.CoreVersion = logLine.LineData.ToString();
            _bCoreFound = true;
         }
      }
      
      /// <summary>
      /// Check the given log line for Project information
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForProjectID(ILogLine logLine)
      {
         if (_bProjectFound == false && logLine.LineType.Equals(LogLineType.WorkUnitProject) && logLine.LineData != null)
         {
            try
            {
               _parsedUnitInfo.DoProjectIDMatch((Match)logLine.LineData);
               _bProjectFound = true;
            }
            catch (FormatException ex)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _parsedUnitInfo.OwningInstanceName, ex);
            }
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForCompletedFrame(ILogLine logLine)
      {
         if (logLine.LineType.Equals(LogLineType.WorkUnitFrame))
         {
            _parsedUnitInfo.SetCurrentFrame(logLine, GetDateTimeStyle(_ClientIsOnVirtualMachine));
         }
      }
      
      /// <summary>
      /// Get the DateTimeStyle for the given Client Instance.
      /// </summary>
      /// <param name="ClientIsOnVirtualMachine">Client on VM (Times as UTC) Flag.</param>
      private static System.Globalization.DateTimeStyles GetDateTimeStyle(bool ClientIsOnVirtualMachine)
      {
         System.Globalization.DateTimeStyles style;
      
         if (ClientIsOnVirtualMachine)
         {
            if (PlatformOps.IsRunningOnMono())
            {
               style = System.Globalization.DateTimeStyles.None;
            }
            else
            {
               // set parse style to maintain universal
               style = System.Globalization.DateTimeStyles.NoCurrentDateDefault;
            }
         }
         else
         {
            if (PlatformOps.IsRunningOnMono())
            {
               style = System.Globalization.DateTimeStyles.AssumeUniversal;
            }
            else
            {
               // set parse style to parse local
               style = System.Globalization.DateTimeStyles.NoCurrentDateDefault |
                       System.Globalization.DateTimeStyles.AssumeUniversal;
            }
         }
         
         return style;
      }
      #endregion
   }
}
