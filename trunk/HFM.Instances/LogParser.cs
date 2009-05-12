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
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
//using System.Windows.Forms;

using HFM.Proteins;
using Debug = HFM.Instrumentation.Debug;

namespace HFM.Instances
{
   class LogParser
   {
      #region Members
      private readonly Regex rProjectNumber =
            new Regex("Project: (?<ProjectNumber>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      //private Regex rProtein =
      //      new Regex("Protein: (?<Protein>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rCoreVersion =
            new Regex("\\[(?<Timestamp>.*)\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompleted =
            new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Completed>.*) out of (?<Total>.*) steps  \\((?<Percent>.*)%\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompletedGpu =
            new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rUserTeam =
            new Regex("\\[(?<Timestamp>.*)\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rUserID =
            new Regex("\\[(?<Timestamp>.*)\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rMachineID =
            new Regex("\\[(?<Timestamp>.*)\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      //private Regex rCompletedWUs =
      //      new Regex("Number of Units Completed: (?<Completed>.*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline); 

      private readonly Regex rProteinID =
            new Regex(@"(?<ProjectNumber>.*) \(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      bool _bProjectFound = false;
      bool _bCoreFound = false;
      bool _bUserTeamFound = false;
      bool _bUserIDFound = false;
      bool _bMachineIDFound = false;
      #endregion

      #region Parsing Methods
      /// <summary>
      /// Extract the content from the UnitInfo.txt file produced by the folding
      /// console
      /// </summary>
      /// <param name="LogFileName">Full path to local copy of UnitInfo.txt</param>
      /// <param name="Instance">Reference back to the instance to which the
      /// UnitInfo file belongs</param>
      public Boolean ParseUnitInfo(String LogFileName, ClientInstance Instance)
      {
         if (!File.Exists(LogFileName)) return false;

         DateTime Start = Debug.ExecStart;

         TextReader tr = null;
         try
         {
            tr = File.OpenText(LogFileName);

            while (tr.Peek() != -1)
            {
               String sData = tr.ReadLine();
               if (sData.StartsWith("Name: "))
               {
                  Instance.UnitInfo.ProteinName = sData.Substring(6);
               }
               else if (sData.StartsWith("Tag:"))
               {
                  Instance.UnitInfo.ProteinTag = sData.Substring(5);
                  DoProjectIDMatch(Instance, rProjectNumberFromTag.Match(Instance.UnitInfo.ProteinTag));
               }
               else if (sData.StartsWith("Download time: "))
               {
                  Instance.UnitInfo.DownloadTime = DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AssumeUniversal);
               }
               else if (sData.StartsWith("Due time: "))
               {
                  Instance.UnitInfo.DueTime = DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AssumeUniversal);
               }
               else if (sData.StartsWith("Progress: "))
               {
                  Instance.UnitInfo.PercentComplete = Int32.Parse(sData.Substring(10, sData.IndexOf("%") - 10));
               }
            }
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
            return false;
         }
         finally
         {
            if (tr != null)
            {
               tr.Dispose();
            }
         }

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Instance.InstanceName, Debug.GetExecTime(Start)));
         return true;
      }

      /// <summary>
      /// Reads through the FAH log file and grabs desired information
      /// </summary>
      /// <param name="LogFileName">Full path to the FAH log file</param>
      /// <param name="Instance">Instance to which the log file data is
      /// attached</param>
      /// <returns></returns>
      public Boolean ParseFAHLog(String LogFileName, ClientInstance Instance)
      {
         // if file does not exist, get out
         if (!File.Exists(LogFileName)) return false;

         DateTime Start = Debug.ExecStart;

         // declare variables for log reading and starting points
         string[] FAHLogText = null;
         int clientStart = 0;
         int unitStart = 0;

         // try to read the log.  if failure, get out
         if (!ReadLogText(LogFileName, ref FAHLogText, ref clientStart, ref unitStart)) return false;

         // setup frame time holders
         DateTime time1 = DateTime.MinValue;         // Time - Three frames ago
         DateTime time2 = DateTime.MinValue;         // Time - Two frames ago
         DateTime time3 = DateTime.MinValue;         // Time - one frame ago
         DateTime time4 = DateTime.MinValue;         // Time - current frame

         // start the parse loop where the client started last
         for (int i = clientStart; i < FAHLogText.Length; i++)
         {
            string logLine = FAHLogText[i];
            // add the line to the instance log holder
            //Instance.CurrentLogText.Add(logLine);
            
            // Read Username and Team Number - Issue 5
            CheckForUserTeamAndIDs(Instance, logLine);

            if (logLine.Contains("FINISHED_UNIT"))
            {
               Instance.NumberOfCompletedUnitsSinceLastStart++;
            }
            if (logLine.Contains("EARLY_UNIT_END") || logLine.Contains("UNSTABLE_MACHINE"))
            {
               Instance.NumberOfFailedUnitsSinceLastStart++;
            }

            //Match mCompletedWUs = rCompletedWUs.Match(logLine);
            //if (mCompletedWUs.Success)
            //{
            //   Instance.TotalUnits = Int32.Parse(mCompletedWUs.Result("${Completed}"));
            //}

            // begin true parsing at the begining of the most recent unit
            if (i > unitStart)
            {
               Instance.Status = ClientStatus.RunningNoFrameTimes;

               // add the line to the instance log holder
               Instance.CurrentLogText.Add(logLine);

               CheckForProjectID(Instance, logLine);
               CheckForCoreVersion(Instance, logLine);

               // don't start parsing frames until we know the client type
               // we have to know the project number before we know the client type (see SetProjectID)
               if (Instance.UnitInfo.TypeOfClient.Equals(ClientType.Unknown) == false)
               {
                  if (Instance.UnitInfo.TypeOfClient.Equals(ClientType.GPU))
                  {
                     CheckForCompletedGpuFrame(Instance, logLine, ref time1, ref time2, ref time3, ref time4);
                  }
                  else //SMP or Standard
                  {
                     CheckForCompletedFrame(Instance, logLine, ref time1, ref time2, ref time3, ref time4);
                  }
               }

               if (logLine.Contains("+ Paused")) // || logLine.Contains("+ Running on battery power"))
               {
                  Instance.Status = ClientStatus.Paused;
               }
               if (logLine.Contains("+ Working ...") && Instance.Status.Equals(ClientStatus.Paused))
               {
                  Instance.Status = ClientStatus.RunningNoFrameTimes;
               }
               if (logLine.Contains("Folding@Home Client Shutdown"))
               {
                  Instance.Status = ClientStatus.Stopped;
                  break; //we found a Shutdown message, quit parsing
               }
            }

            // Process UI message queue
            //Application.DoEvents();
         }

         bool bResult = true;

         if (Instance.Status != ClientStatus.Stopped &&
             Instance.Status != ClientStatus.Paused)
         {
            DetermineStatus(Instance);
            if (Instance.Status.Equals(ClientStatus.Hung))
            {
               // client is hung, clear PPD values
               bResult = false;
            }
         }
         else
         {
            // client is stopped or paused, clear PPD values
            bResult = false;
         }

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Instance.InstanceName, Debug.GetExecTime(Start)));

         return bResult;
      }
      #endregion

      #region Parsing Helpers
      /// <summary>
      /// Reads Log File into string array then determines the parsing start points
      /// </summary>
      /// <param name="LogFileName">Log File Name</param>
      /// <param name="FAHLogText">Log Line array</param>
      /// <param name="clientStart">Client start message index in array</param>
      /// <param name="unitStart">Last unit start message index in array</param>
      /// <returns>Success or Failure</returns>
      private static bool ReadLogText(string LogFileName, ref string[] FAHLogText, ref int clientStart, ref int unitStart)
      {
         try
         {
            FAHLogText = File.ReadAllLines(LogFileName);
            GetParsingStartPositionsFromLog(FAHLogText, out clientStart, out unitStart);
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            return false;
         }
         return true;
      }

      /// <summary>
      /// Finds the starting indexes in the given Log File string array
      /// </summary>
      /// <param name="FAHLogText">Log Line array</param>
      /// <param name="clientStart">Client start message index in array</param>
      /// <param name="unitStart">Last unit start message index in array</param>
      private static void GetParsingStartPositionsFromLog(string[] FAHLogText, out int clientStart, out int unitStart)
      {
         clientStart = 0;
         unitStart = 0;

         //work backwards through the file text to find starting positions
         for (int i = FAHLogText.Length - 1; i >= 0; i--)
         {
            string s = FAHLogText[i];
            if (s.Contains("--- Opening Log file"))
            {
               clientStart = i;
            }

            if (s.Contains("*------------------------------*") && unitStart == 0)
            {
               unitStart = i;
            }

            if (clientStart != 0 && unitStart != 0)
            {
               //found our starting points, get out
               break;
            }
         }
      }

      /// <summary>
      /// Check the given log line for Project information
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForProjectID(ClientInstance Instance, string logLine)
      {
         Match mProjectNumber;
         if (_bProjectFound == false && (mProjectNumber = rProjectNumber.Match(logLine)).Success)
         {
            DoProjectIDMatch(Instance, rProteinID.Match(mProjectNumber.Result("${ProjectNumber}")));
         }
      }

      /// <summary>
      /// Attempts to Set Project ID with given Match.  If Project cannot be found in local cache, download again.
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="match">Regex Match containing Project data</param>
      private void DoProjectIDMatch(ClientInstance Instance, Match match)
      {
         try
         {
            SetProjectID(Instance, match);
            _bProjectFound = true;
         }
         catch (System.Collections.Generic.KeyNotFoundException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning,
                                    String.Format("{0} Project ID '{1}' not found in Protein Collection.",
                                                  Debug.FunctionName, Instance.UnitInfo.ProjectID));

            // If a Project cannot be identified using the local Project data, update Project data from Stanford. - Issue 4
            Debug.WriteToHfmConsole(TraceLevel.Info,
                                    String.Format("{0} Attempting to download new Project data...", Debug.FunctionName));
            ProteinCollection.Instance.DownloadFromStanford(null);
            try
            {
               SetProjectID(Instance, rProjectNumberFromTag.Match(Instance.UnitInfo.ProteinTag));
               _bProjectFound = true;
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} Project ID '{1}' not found on Stanford Web Project Summary.",
                                                     Debug.FunctionName, Instance.UnitInfo.ProjectID));
            }
         }
         catch (FormatException ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} {1}.", Debug.FunctionName, ex.Message));
         }
      }

      /// <summary>
      /// Sets the ProjectID and gets the Protein info from the Protein Collection (from Stanford)
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="match">Project string match</param>
      /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when Project ID cannot be found in Protein Collection.</exception>
      /// <exception cref="FormatException">Thrown when Project ID string fails to parse.</exception>
      private static void SetProjectID(ClientInstance Instance, Match match)
      {
         if (match.Success)
         {
            Instance.UnitInfo.ProjectID = int.Parse(match.Result("${ProjectNumber}"));
            Instance.UnitInfo.ProjectRun = int.Parse(match.Result("${Run}"));
            Instance.UnitInfo.ProjectClone = int.Parse(match.Result("${Clone}"));
            Instance.UnitInfo.ProjectGen = int.Parse(match.Result("${Gen}"));

            Instance.CurrentProtein = ProteinCollection.Instance[Instance.UnitInfo.ProjectID];
            Instance.UnitInfo.TypeOfClient = GetClientTypeFromProtein(Instance.CurrentProtein);
         }
         else
         {
            throw new FormatException(String.Format("Failed to parse the Project (R/C/G) values from '{0}'", match.Value));
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
      /// Check the given log line for Core version information
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForCoreVersion(ClientInstance Instance, string logLine)
      {
         Match mCoreVer;
         if (_bCoreFound == false && (mCoreVer = rCoreVersion.Match(logLine)).Success)
         {
            string sCoreVer = mCoreVer.Result("${CoreVer}");
            Instance.UnitInfo.CoreVersion = sCoreVer.Substring(0, sCoreVer.IndexOf(" "));
            _bCoreFound = true;
         }
      }

      /// <summary>
      /// Check the given log line for User, Team, and Machine related ID values
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForUserTeamAndIDs(ClientInstance Instance, string logLine)
      {
         Match mUserTeam;
         if (_bUserTeamFound == false && (mUserTeam = rUserTeam.Match(logLine)).Success)
         {
            Instance.UnitInfo.Username = mUserTeam.Result("${Username}");
            Instance.UnitInfo.Team = int.Parse(mUserTeam.Result("${TeamNumber}"));
            _bUserTeamFound = true;
         }

         Match mUserID;
         if (_bUserIDFound == false && (mUserID = rUserID.Match(logLine)).Success)
         {
            Instance.UserID = mUserID.Result("${UserID}");
            _bUserIDFound = true;
         }

         Match mMachineID;
         if (_bMachineIDFound == false && (mMachineID = rMachineID.Match(logLine)).Success)
         {
            Instance.MachineID = int.Parse(mMachineID.Result("${MachineID}"));
            _bMachineIDFound = true;
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only)
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="logLine">Log Line</param>
      /// <param name="time1"></param>
      /// <param name="time2"></param>
      /// <param name="time3"></param>
      /// <param name="time4"></param>
      private void CheckForCompletedGpuFrame(ClientInstance Instance, string logLine, ref DateTime time1, ref DateTime time2, ref DateTime time3, ref DateTime time4)
      {
         Match mFramesCompletedGpu = rFramesCompletedGpu.Match(logLine);
         if (mFramesCompletedGpu.Success)
         {
            //This works on GPU2 & GPU2-MT Core Log Files

            Instance.UnitInfo.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            Instance.UnitInfo.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            //TODO: Hard code here, 100 GPU Frames. Could I get this from the Project Data?
            //I could but what's the point, 100% is 100%.

            SetTimeStamp(Instance, mFramesCompletedGpu.Result("${Timestamp}"),
                         ref time1, ref time2, ref time3, ref time4);
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients)
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="logLine">Log Line</param>
      /// <param name="time1"></param>
      /// <param name="time2"></param>
      /// <param name="time3"></param>
      /// <param name="time4"></param>
      private void CheckForCompletedFrame(ClientInstance Instance, string logLine, ref DateTime time1, ref DateTime time2, ref DateTime time3, ref DateTime time4)
      {
         Match mFramesCompleted = rFramesCompleted.Match(logLine);
         if (mFramesCompleted.Success)
         {
            // This works on SMP A1 & A2 Core
            // Confirmed to work with Standard Gromacs Core

            try
            {
               Instance.UnitInfo.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               Instance.UnitInfo.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
               //string temp = mFramesCompleted.Result("${Percent}");
            }
            catch (FormatException)
            {
               Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} Failed to parse raw frame values from '{1}'.", Debug.FunctionName, logLine));
            }

            SetTimeStamp(Instance, mFramesCompleted.Result("${Timestamp}"),
                         ref time1, ref time2, ref time3, ref time4);
         }
      }

      /// <summary>
      /// Set the Raw Time per Section based on the given and previously read 
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="timeStampString">String containing the current frame time stamp</param>
      /// <param name="time1"></param>
      /// <param name="time2"></param>
      /// <param name="time3"></param>
      /// <param name="time4"></param>
      private static void SetTimeStamp(ClientInstance Instance, string timeStampString, ref DateTime time1, ref DateTime time2,
                                                                                        ref DateTime time3, ref DateTime time4)
      {
         System.Globalization.DateTimeStyles style;

         if (Instance.ClientIsOnVirtualMachine)
         {
            // set parse style to maintain universal
            style = System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.NoCurrentDateDefault |
                    System.Globalization.DateTimeStyles.AdjustToUniversal;
         }
         else
         {
            // set parse style to parse local
            style = System.Globalization.DateTimeStyles.AssumeUniversal |
                    System.Globalization.DateTimeStyles.NoCurrentDateDefault;
         }

         DateTime timeStamp = DateTime.ParseExact(timeStampString, "HH:mm:ss",
                               System.Globalization.DateTimeFormatInfo.InvariantInfo,
                               style);

         time1 = time2;
         time2 = time3;
         time3 = time4;
         time4 = timeStamp;

         Instance.UnitInfo.TimeOfLastFrame = time4.TimeOfDay;

         if (time1 != DateTime.MinValue)
         {
            // time1 is valid for 3 "sets" ago
            TimeSpan tDelta = GetDelta(time4, time1);
            Instance.UnitInfo.RawTimePerThreeSections = Convert.ToInt32(tDelta.TotalSeconds / 3);
         }

         //else if (time2 != DateTime.MinValue)
         //{
         //   // time2 is valid for 2 "set" ago
         //   TimeSpan tDelta = GetDelta(time4, time2);
         //   Instance.UnitInfo.RawTimePerSection = Convert.ToInt32(tDelta.TotalSeconds / 2);
         //}

         if (time3 != DateTime.MinValue)
         {
            // time3 is valid for 1 "set" ago
            TimeSpan tDelta = GetDelta(time4, time3);
            Instance.UnitInfo.RawTimePerLastSection = Convert.ToInt32(tDelta.TotalSeconds);
         }
      }

      /// <summary>
      /// Get Time Delta between given frames
      /// </summary>
      /// <param name="timeLastFrame">Time of last frame</param>
      /// <param name="timeCompareFrame">Time of a previous frame to compare</param>
      /// <returns></returns>
      private static TimeSpan GetDelta(DateTime timeLastFrame, DateTime timeCompareFrame)
      {
         TimeSpan tDelta;

         // check for rollover back to 00:00:00 timeLastFrame will be less than previous timeCompareFrame reading
         if (timeLastFrame < timeCompareFrame)
         {
            // get time before rollover
            tDelta = TimeSpan.FromDays(1).Subtract(timeCompareFrame.TimeOfDay);
            // add time from latest reading
            tDelta = tDelta.Add(timeLastFrame.TimeOfDay);
         }
         else
         {
            tDelta = timeLastFrame.Subtract(timeCompareFrame);
         }

         return tDelta;
      }

      /// <summary>
      /// Determine Client Status
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      private static void DetermineStatus(ClientInstance Instance)
      {
         #region Get Terminal Time
         // Terminal Time - defined as last retrieval time minus twice (7 times for GPU) the current Raw Time per Section.
         // if a new frame has not completed in twice the amount of time it should take to complete we should deem this client Hung.
         DateTime terminalDateTime;

         if (Instance.UnitInfo.TypeOfClient.Equals(ClientType.GPU))
         {
            terminalDateTime = Instance.LastRetrievalTime.Subtract(new TimeSpan(0, 0, Instance.UnitInfo.RawTimePerSection * 7));
         }
         else
         {
            terminalDateTime = Instance.LastRetrievalTime.Subtract(new TimeSpan(0, 0, Instance.UnitInfo.RawTimePerSection * 2));
         }
         #endregion

         // make sure we have calculated a frame time (could be based on 'LastFrame' or 'LastThreeFrames')
         if (Instance.UnitInfo.RawTimePerSection > 0)
         {
            #region Get Last Retrieval Time Date
            DateTime currentFrameDateTime;
         
            if (Instance.ClientIsOnVirtualMachine)
            {
               // get only the date from the last retrieval time (in universal), we'll add the current time below
               currentFrameDateTime = new DateTime(Instance.LastRetrievalTime.Date.Ticks, DateTimeKind.Utc);
            }
            else
            {
               // get only the date from the last retrieval time, we'll add the current time below
               currentFrameDateTime = Instance.LastRetrievalTime.Date;
            }
            #endregion
            
            #region Apply Frame Time Offset and Set Current Frame Time Date
            TimeSpan offset = TimeSpan.FromMinutes(Instance.ClientTimeOffset);
            TimeSpan adjustedFrameTime = Instance.UnitInfo.TimeOfLastFrame.Subtract(offset);

            // client time has already rolled over to the next day. the offset correction has 
            // caused the adjusted frame time span to be negetive.  take the that negetive span
            // and add it to a full 24 hours to correct.
            if (adjustedFrameTime < TimeSpan.Zero)
            {
               adjustedFrameTime = TimeSpan.FromDays(1).Add(adjustedFrameTime);
            }
            
            // the offset correction has caused the frame time span to be greater than 24 hours.
            // subtract the extra day from the adjusted frame time span.
            else if (adjustedFrameTime > TimeSpan.FromDays(1))
            {
               adjustedFrameTime = adjustedFrameTime.Subtract(TimeSpan.FromDays(1));
            }

            // add adjusted Time of Last Frame (TimeSpan) to the DateTime with the correct date
            currentFrameDateTime = currentFrameDateTime.Add(adjustedFrameTime);
            #endregion
            
            #region Check For Frame from Prior Day (Midnight Rollover on Local Machine)
            bool priorDayAdjust = false;
            
            // if the current (and adjusted) frame time hours is greater than the last retrieval time hours, 
            // and the time difference is greater than an hour, then frame is from the day prior.
            // this should only happen after midnight time on the machine running HFM when the monitored client has 
            // not completed a frame since the local machine time rolled over to the next day, otherwise the time
            // stamps between HFM and the client are too far off, a positive offset should be set to correct.
            if (currentFrameDateTime.TimeOfDay.Hours > Instance.LastRetrievalTime.TimeOfDay.Hours &&
                currentFrameDateTime.TimeOfDay.Subtract(Instance.LastRetrievalTime.TimeOfDay).Hours > 0)
            {
               priorDayAdjust = true;

               // subtract 1 day from today's date
               currentFrameDateTime = currentFrameDateTime.Subtract(TimeSpan.FromDays(1));
            }
            #endregion
            
            #region Write Verbose Trace
            if (HFM.Instrumentation.TraceLevelSwitch.GetTraceLevelSwitch().TraceVerbose)
            {
               System.Collections.Generic.List<string> messages = new System.Collections.Generic.List<string>(10);

               messages.Add(String.Format("{0} ({1})", Debug.FunctionName, Instance.InstanceName));
               messages.Add(String.Format(" - Retrieval Time (Date) ------- : {0}", Instance.LastRetrievalTime));
               messages.Add(String.Format(" - Time Of Last Frame (TimeSpan) : {0}", Instance.UnitInfo.TimeOfLastFrame));
               messages.Add(String.Format(" - Offset (Minutes) ------------ : {0}", Instance.ClientTimeOffset));
               messages.Add(String.Format(" - Time Of Last Frame (Adjusted) : {0}", adjustedFrameTime));
               messages.Add(String.Format(" - Prior Day Adjustment -------- : {0}", priorDayAdjust));
               messages.Add(String.Format(" - Time Of Last Frame (Date) --- : {0}", currentFrameDateTime));
               messages.Add(String.Format(" - Terminal Time (Date) -------- : {0}", terminalDateTime));
               
               Debug.WriteToHfmConsole(TraceLevel.Verbose, messages.ToArray());
            }
            #endregion

            if (currentFrameDateTime > terminalDateTime)
            {
               Instance.Status = ClientStatus.Running;
            }
            else // current frame is less than terminal time
            {
               Instance.Status = ClientStatus.Hung;
            }
         }
      }
      #endregion
   }
}
