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

using HFM.Preferences;
using HFM.Proteins;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class LogParser
   {
      #region Members
      private readonly ClientInstance _Instance;
      private readonly UnitInfo _parsedUnitInfo;
      
      private ClientStatus _returnStatus;
      
      private readonly Regex rTimeStamp =
            new Regex("\\[(?<Timestamp>.*)\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
      private readonly Regex rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rProteinID =
            new Regex(@"(?<ProjectNumber>.*) \(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompleted =
            new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Completed>.*) out of (?<Total>.*) steps  \\((?<Percent>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompletedGpu =
            new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rPercent1 =
            new Regex("(?<Percent>.*) percent", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rPercent2 =
            new Regex("(?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private bool _bUnitStart = false;
      private bool _bCoreFound = false;
      private bool _bProjectFound = false;
      #endregion

      /// <summary>
      /// Class inspects LogLines and unitinfo.txt file to populate the given UnitInfo object.
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing.</param>
      /// <param name="parsedUnitInfo">Container for parsed information.</param>
      public LogParser(ClientInstance Instance, UnitInfo parsedUnitInfo)
      {
         _Instance = Instance;
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
               if (sData.StartsWith("Name: "))
               {
                  _parsedUnitInfo.ProteinName = sData.Substring(6);
               }
               else if (sData.StartsWith("Tag:"))
               {
                  _parsedUnitInfo.ProteinTag = sData.Substring(5);
                  try 
                  {
                     DoProjectIDMatch(_parsedUnitInfo, rProjectNumberFromTag.Match(_parsedUnitInfo.ProteinTag), _parsedUnitInfo.ProteinTag);
                  }
                  catch (FormatException ex)
                  {
                     HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _Instance.InstanceName, ex);
                  }
               }
               else if (sData.StartsWith("Download time: "))
               {
                  if (_Instance.ClientIsOnVirtualMachine)
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
               else if (sData.StartsWith("Due time: "))
               {
                  _parsedUnitInfo.DueTime = DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AssumeUniversal);
               }
               else if (sData.StartsWith("Progress: "))
               {
                  _parsedUnitInfo.PercentComplete = Int32.Parse(sData.Substring(10, sData.IndexOf("%") - 10));
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(_Instance.InstanceName, ex);
            return false;
         }
         finally
         {
            if (tr != null)
            {
               tr.Dispose();
            }

            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _Instance.InstanceName, Start);
         }

         return true;
      }

      /// <summary>
      /// Itterates through the given list of LogLines and parses the desired information.
      /// </summary>
      /// <param name="FAHLog">List of LogLines to Parse.</param>
      public ClientStatus ParseFAHLog(IList<LogLine> FAHLog)
      {
         DateTime Start = HfmTrace.ExecStart;

         _bUnitStart = false;
         _bCoreFound = false;
         _bProjectFound = false;
         
         _returnStatus = ClientStatus.Unknown;

         // start the parse loop where the client started last
         foreach (LogLine logLine in FAHLog)
         {
            SetUnitStartTimeStamp(logLine.LineRaw);
            CheckForCoreVersion(logLine);
            CheckForProjectID(logLine);

            // don't start parsing frames until we know the client type
            // we have to know the project number before we know the client type (see SetProjectID)
            if (_parsedUnitInfo.TypeOfClient.Equals(ClientType.Unknown) == false)
            {
               if (_parsedUnitInfo.TypeOfClient.Equals(ClientType.GPU))
               {
                  CheckForCompletedGpuFrame(logLine);
               }
               else //SMP or Standard
               {
                  try
                  {
                     CheckForCompletedFrame(logLine);
                  }
                  catch (FormatException ex)
                  {
                     HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _Instance.InstanceName, ex);
                  }
               }
            }

            if (logLine.LineType.Equals(LogLineType.WorkUnitStart))
            {
               _returnStatus = ClientStatus.RunningNoFrameTimes;
            }
            else if (logLine.LineType.Equals(LogLineType.WorkUnitPaused)) // || logLine.LineRaw.Contains("+ Running on battery power"))
            {
               _returnStatus = ClientStatus.Paused;
            }
            else if (logLine.LineType.Equals(LogLineType.WorkUnitWorking) && _returnStatus.Equals(ClientStatus.Paused))
            {
               _returnStatus = ClientStatus.RunningNoFrameTimes;
               
               // Reset Frames Observed, Current Frame, and Unit Start Time after Pause. 
               // This will cause the Instance to only use frames beyond this point to 
               // set frame times and determine status - Issue 13 (Revised)
               _parsedUnitInfo.CurrentFrame = null;
               _parsedUnitInfo.FramesObserved = 0;

               // Clear UnitStart Flag so we can read a new value with SetUnitStartTimeStamp()
               _bUnitStart = false;
               SetUnitStartTimeStamp(logLine.LineRaw); 
            }
            else if (logLine.LineType.Equals(LogLineType.WorkUnitCoreShutdown))
            {
               _parsedUnitInfo.UnitResult = (WorkUnitResult)logLine.LineData;
            }
            else if (logLine.LineType.Equals(LogLineType.ClientEuePauseState))
            {
               _returnStatus = ClientStatus.EuePause;
            }
            else if (logLine.LineType.Equals(LogLineType.ClientAttemptGetWorkPacket))
            {
               _returnStatus = ClientStatus.GettingWorkPacket;
            }
            else if (logLine.LineType.Equals(LogLineType.ClientShutdown) ||
                     logLine.LineType.Equals(LogLineType.ClientCoreCommunicationsErrorShutdown))
            {
               _returnStatus = ClientStatus.Stopped;
               break; //we found a Shutdown message, quit parsing
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, _Instance.InstanceName, Start);
         
         return _returnStatus;
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
         if (_bUnitStart == false && _returnStatus.Equals(ClientStatus.Unknown) == false)
         {
            if ((mTimeStamp = rTimeStamp.Match(logLine)).Success)
            {
               try
               {
                  DateTime timeStamp = DateTime.ParseExact(mTimeStamp.Result("${Timestamp}"), "HH:mm:ss",
                                                           System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                           GetDateTimeStyle(_Instance.ClientIsOnVirtualMachine));

                  _parsedUnitInfo.UnitStartTime = timeStamp.TimeOfDay;
                  _bUnitStart = true;
               }
               catch (FormatException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) Failed to get 'UnitStartTime' from {2}.",
                     HfmTrace.FunctionName, _Instance.InstanceName, logLine));
               }
            }
         }
      }
      
      /// <summary>
      /// Check the given log line for Core version information
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForCoreVersion(LogLine logLine)
      {
         if (_bCoreFound == false && logLine.LineType.Equals(LogLineType.WorkUnitCoreVersion))
         {
            _parsedUnitInfo.CoreVersion = logLine.LineData.ToString();
            _bCoreFound = true;
         }
      }
      
      /// <summary>
      /// Check the given log line for Project information
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForProjectID(LogLine logLine)
      {
         if (_bProjectFound == false && logLine.LineType.Equals(LogLineType.WorkUnitProject))
         {
            try
            {
               DoProjectIDMatch(_parsedUnitInfo, rProteinID.Match(logLine.LineData.ToString()), logLine.LineData.ToString());
               _bProjectFound = true;
            }
            catch (FormatException ex)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, _parsedUnitInfo.OwningInstanceName, ex);
            }
         }
      }

      /// <summary>
      /// Attempts to Set Project ID with given Match.  If Project cannot be found in local cache, download again.
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="match">Regex Match containing Project data</param>
      /// <param name="matchValue">String value being matched (for logging purposes)</param>
      private static void DoProjectIDMatch(UnitInfo parsedUnitInfo, Match match, string matchValue)
      {
         try
         {
            SetProjectID(parsedUnitInfo, match, matchValue);
         }
         catch (KeyNotFoundException)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} Project ID '{1}' not found in Protein Collection.",
                                                     HfmTrace.FunctionName, parsedUnitInfo.ProjectID));

            // If a Project cannot be identified using the local Project data, update Project data from Stanford. - Issue 4
            HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                       String.Format("{0} Attempting to download new Project data...", HfmTrace.FunctionName));
            ProteinCollection.Instance.DownloadFromStanford();
            
            try
            {
               SetProjectID(parsedUnitInfo, match, matchValue);
            }
            catch (KeyNotFoundException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Error,
                                          String.Format("{0} Project ID '{1}' not found on Stanford Web Project Summary.",
                                                        HfmTrace.FunctionName, parsedUnitInfo.ProjectID));
            }
         }
      }

      /// <summary>
      /// Sets the ProjectID and gets the Protein info from the Protein Collection (from Stanford)
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="match">Project string match</param>
      /// <param name="matchValue">String value being matched (for logging purposes)</param>
      /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when Project ID cannot be found in Protein Collection.</exception>
      /// <exception cref="FormatException">Thrown when Project ID string fails to parse.</exception>
      private static void SetProjectID(UnitInfo parsedUnitInfo, Match match, string matchValue)
      {
         if (match.Success)
         {
            parsedUnitInfo.ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
            parsedUnitInfo.ProjectRun = Int32.Parse(match.Result("${Run}"));
            parsedUnitInfo.ProjectClone = Int32.Parse(match.Result("${Clone}"));
            parsedUnitInfo.ProjectGen = Int32.Parse(match.Result("${Gen}"));

            parsedUnitInfo.CurrentProtein = ProteinCollection.Instance[parsedUnitInfo.ProjectID];
            parsedUnitInfo.TypeOfClient = GetClientTypeFromProtein(parsedUnitInfo.CurrentProtein);
         }
         else
         {
            throw new FormatException(String.Format("Failed to parse the Project (R/C/G) values from '{0}'", matchValue));
         }
      }

      /// <summary>
      /// Determine the client type based on the current protein core
      /// </summary>
      /// <param name="CurrentProtein">Current Instance Protein</param>
      /// <returns>Client Type</returns>
      private static ClientType GetClientTypeFromProtein(Protein CurrentProtein)
      {
         switch (CurrentProtein.Core)
         {
            case "GROMACS":
            case "DGROMACS":
            case "GBGROMACS":
            case "AMBER":
            case "QMD":
            case "GROMACS33":
            case "GROST":
            case "GROSIMT":
            case "DGROMACSB":
            case "DGROMACSC":
            case "GRO-A4":
            case "TINKER":
               return ClientType.Standard;
            case "GRO-SMP":
            case "GROCVS":
               return ClientType.SMP;
            case "GROGPU2":
            case "GROGPU2-MT":
            case "ATI-DEV":
            case "NVIDIA-DEV":
               return ClientType.GPU;
            default:
               return ClientType.Unknown;
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForCompletedGpuFrame(LogLine logLine)
      {
         Match mFramesCompletedGpu = rFramesCompletedGpu.Match(logLine.LineRaw);
         if (mFramesCompletedGpu.Success)
         {
            logLine.LineType = LogLineType.WorkUnitFrame;
         
            _parsedUnitInfo.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            _parsedUnitInfo.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            //TODO: Hard code here, 100 GPU Frames. Could I get this from the Project Data?
            //I could but what's the point, 100% is 100%.

            SetTimeStamp(mFramesCompletedGpu.Result("${Timestamp}"), _parsedUnitInfo.RawFramesComplete);
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      private void CheckForCompletedFrame(LogLine logLine)
      {
         Match mFramesCompleted = rFramesCompleted.Match(logLine.LineRaw);
         if (mFramesCompleted.Success)
         {
            logLine.LineType = LogLineType.WorkUnitFrame;
         
            try
            {
               _parsedUnitInfo.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               _parsedUnitInfo.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
            }
            catch (FormatException ex)
            {
               throw new FormatException(String.Format("{0} Failed to parse raw frame values from '{1}'.", HfmTrace.FunctionName, logLine), ex);
            }

            string percentString = mFramesCompleted.Result("${Percent}");
            
            Match mPercent1 = rPercent1.Match(percentString);
            Match mPercent2 = rPercent2.Match(percentString);

            int percent;
            if (mPercent1.Success)
            {
               percent = Int32.Parse(mPercent1.Result("${Percent}"));
            }
            else if (mPercent2.Success)
            {
               percent = Int32.Parse(mPercent2.Result("${Percent}"));
            }
            // Try to parse a percentage from in between the parentheses (for older single core clients like v5.02) - Issue 36
            else if (Int32.TryParse(percentString, out percent) == false)
            {
               throw new FormatException(String.Format("{0} Failed to parse frame percent from '{1}'.", HfmTrace.FunctionName, logLine));
            }

            SetTimeStamp(mFramesCompleted.Result("${Timestamp}"), percent);
         }
      }

      /// <summary>
      /// Set the Raw Time per Section based on the given and previously read.
      /// </summary>
      /// <param name="timeStampString">String containing the current frame time stamp.</param>
      /// <param name="percent">Frame Percentage</param>
      private void SetTimeStamp(string timeStampString, int percent)
      {
         SetTimeStamp(_Instance.ClientIsOnVirtualMachine, _parsedUnitInfo, timeStampString, percent);   
      }

      /// <summary>
      /// Set the Raw Time per Section based on the given and previously read.
      /// </summary>
      /// <param name="ClientIsOnVirtualMachine">Boolean representing client on machine running local time as UTC.</param>
      /// <param name="parsedUnitInfo">Container for parsed information.</param>
      /// <param name="timeStampString">String containing the current frame time stamp.</param>
      /// <param name="percent">Frame Percentage</param>
      private static void SetTimeStamp(bool ClientIsOnVirtualMachine, UnitInfo parsedUnitInfo, string timeStampString, int percent)
      {
         DateTime timeStamp = DateTime.ParseExact(timeStampString, "HH:mm:ss",
                              System.Globalization.DateTimeFormatInfo.InvariantInfo,
                              GetDateTimeStyle(ClientIsOnVirtualMachine));

         parsedUnitInfo.SetCurrentFrame(new UnitFrame(percent, timeStamp.TimeOfDay));
      }
      
      /// <summary>
      /// Get the DateTimeStyle for the given Client Instance.
      /// </summary>
      /// <param name="ClientIsOnVirtualMachine">Boolean representing client on machine running local time as UTC.</param>
      private static System.Globalization.DateTimeStyles GetDateTimeStyle(bool ClientIsOnVirtualMachine)
      {
         System.Globalization.DateTimeStyles style;
      
         if (ClientIsOnVirtualMachine)
         {
            // set parse style to maintain universal
            style = System.Globalization.DateTimeStyles.NoCurrentDateDefault;
         }
         else
         {
            // set parse style to parse local
            style = System.Globalization.DateTimeStyles.NoCurrentDateDefault |
                    System.Globalization.DateTimeStyles.AssumeUniversal;
         }
         
         return style;
      }
      #endregion
   }
}
