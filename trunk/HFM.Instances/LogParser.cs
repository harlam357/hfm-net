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
using System.Windows.Forms;

using HFM.Proteins;
using Debug=HFM.Instrumentation.Debug;

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
      //     new Regex("Protein: (?<Protein>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rCoreVersion =
           new Regex("\\[(?<Timestamp>.*)\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompleted =
           new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Completed>.*) out of (?<Total>.*) steps  ((?<Percent>.*))", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompletedGpu =
           new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Percent>.*)%");

      //private Regex rCompletedWUs =
      //     new Regex("Number of Units Completed: (?<Completed>.*)$", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline); 

      private readonly Regex rProteinID =
           new Regex(@"(?<ProjectNumber>.*) \(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      bool _bProjectFound = false;
      bool _bCoreFound = false;
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
                  // Note: Removed setting of ProteinID. I will use ProjectID and ProjectRunCloneGen instead.
                  //Instance.UnitInfo.ProteinID = ProteinHelper.ExtractProteinID(sData.Substring(6));
                  Instance.UnitInfo.ProteinName = sData.Substring(6);
               }
               else if (sData.StartsWith("Tag:"))
               {
                  Instance.UnitInfo.ProteinTag = sData.Substring(5);
                  try
                  {
                     SetProjectID(Instance, rProjectNumberFromTag.Match(Instance.UnitInfo.ProteinTag));
                     _bProjectFound = true;
                  }
                  catch (FormatException)
                  {
                     //TODO: Log this failure
                  }
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
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
            return false;
         }
         finally
         {
            if (tr != null)
            {
               tr.Dispose();
            }
         }
         
         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Instance.Name, Debug.GetExecTime(Start)));
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
         //StreamReader FAHlog;
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

         // setup project and core found flags
         _bProjectFound = false;
         _bCoreFound = false;

         //while (FAHlog.Peek() != -1)
         // start the parse loop where the client started last
         for (int i = clientStart; i < FAHLogText.Length; i++)
         {
            //string logLine = FAHlog.ReadLine();
            string logLine = FAHLogText[i];
            // add the line to the instance log holder
            //Instance.CurrentLogText.Add(logLine);

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
               Instance.UnitInfo.Status = eClientStatus.RunningNoFrameTimes;
               
               // add the line to the instance log holder
               Instance.CurrentLogText.Add(logLine);
            
               CheckForProjectID(Instance, logLine);
               CheckForCoreVersion(Instance, logLine);

               // don't start parsing frames until we know the client type
               // we have to know the project number before we know the client type (see SetProjectID)
               if (Instance.UnitInfo.ClientType.Equals(eClientType.Unknown) == false)
               {
                  if (Instance.UnitInfo.ClientType.Equals(eClientType.GPU))
                  {
                     CheckForCompletedGpuFrame(Instance, logLine, ref time1, ref time2, ref time3, ref time4);
                  }
                  else //SMP or Standard
                  {
                     CheckForCompletedFrame(Instance, logLine, ref time1, ref time2, ref time3, ref time4);
                  }
               }

               if (logLine.Contains("+ Paused"))
               {
                  Instance.UnitInfo.Status = eClientStatus.Paused;
               }
               if (logLine.Contains("+ Working ...") && Instance.UnitInfo.Status.Equals(eClientStatus.Paused))
               {
                  Instance.UnitInfo.Status = eClientStatus.RunningNoFrameTimes;
               }
               if (logLine.Contains("Folding@Home Client Shutdown"))
               {
                  Instance.UnitInfo.Status = eClientStatus.Stopped;
                  break; //we found a Shutdown message, quit parsing
               }
            }

            // Process UI message queue
            Application.DoEvents();
         }

         //FAHlog.Close();

         bool bResult = true;
         
         if (Instance.UnitInfo.Status != eClientStatus.Stopped &&
             Instance.UnitInfo.Status != eClientStatus.Paused)
         {
            DetermineStatus(Instance);
            if (Instance.UnitInfo.Status.Equals(eClientStatus.Hung))
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

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Instance.Name, Debug.GetExecTime(Start)));

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
            //FAHlog = File.OpenText(LogFileName);
            FAHLogText = File.ReadAllLines(LogFileName);
            GetParsingStartPositionsFromLog(FAHLogText, out clientStart, out unitStart);
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
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
            //if (s.Contains("###############################################################################"))
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
            try
            {
               SetProjectID(Instance, rProteinID.Match(mProjectNumber.Result("${ProjectNumber}")));
               _bProjectFound = true;
            }
            catch (FormatException)
            {
               //TODO: Log this failure
            }
         }
      }

      /// <summary>
      /// Sets the ProjectID and gets the Protein info from the Protein Collection (from Stanford)
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="match">Project string match</param>
      private static void SetProjectID(ClientInstance Instance, Match match)
      {
         if (match.Success)
         {
            Instance.UnitInfo.ProjectID = int.Parse(match.Result("${ProjectNumber}"));
            Instance.UnitInfo.ProjectRun = int.Parse(match.Result("${Run}"));
            Instance.UnitInfo.ProjectClone = int.Parse(match.Result("${Clone}"));
            Instance.UnitInfo.ProjectGen = int.Parse(match.Result("${Gen}"));

            try
            {
               Instance.CurrentProtein = ProteinCollection.Instance[Instance.UnitInfo.ProjectID];
               Instance.UnitInfo.ClientType = GetClientTypeFromProtein(Instance.CurrentProtein);
            }
            catch (System.Collections.Generic.KeyNotFoundException ex)
            {
               // Disregard - we don't know the protein name!
               Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
            }
         }
         else
         {
            throw new FormatException("Failed to parse the Project (R/C/G) values");
         }
      }

      /// <summary>
      /// Determine the client type based on the current protein core
      /// </summary>
      /// <param name="CurrentProtein">Current Instance Protein</param>
      /// <returns>Client Type</returns>
      private static eClientType GetClientTypeFromProtein(Protein CurrentProtein)
      {
         switch (CurrentProtein.Core)
         {
            case "TINKER":
            case "AMBER":
            case "GROMACS":
            case "DGROMACS":
            case "DGROMACSC":
            case "GROMACS33":
            case "GROSIMT":
            case "GRO-A4":
               return eClientType.Standard;
            case "GRO-SMP":
            case "GROCVS":
               return eClientType.SMP;
            case "GROGPU2":
            case "GROGPU2-MT":
               return eClientType.GPU;
            default:
               return eClientType.Unknown;
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
            // This works on SMP A1 & A2 Core Log Files
            //TODO: Test Standard Client Logs

            try
            {
               Instance.UnitInfo.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               Instance.UnitInfo.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
            }
            catch (FormatException)
            {
               //TODO: Log this failure
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
            tDelta = new TimeSpan(0, 24, 0, 0).Subtract(timeCompareFrame.TimeOfDay);
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
         // Terminal Time - defined as current time minus twice (5 times for GPU) the current Raw Time per Section.
         // if a new frame has not completed in twice the amount of time it should take to complete we should deem 
         // this client Hung.
         DateTime terminalTime;

         if (Instance.UnitInfo.ClientType.Equals(eClientType.GPU))
         {
            terminalTime = DateTime.Now.Subtract(new TimeSpan(0, 0, Instance.UnitInfo.RawTimePerSection * 5));
         }
         else
         {
            terminalTime = DateTime.Now.Subtract(new TimeSpan(0, 0, Instance.UnitInfo.RawTimePerSection * 2));
         }

         // make sure we have calculated a frame time (could be based on 'LastFrame' or 'LastThreeFrames')
         if (Instance.UnitInfo.RawTimePerSection > 0)
         {
            DateTime currentFrameTime;
            DateTime now = DateTime.Now;
         
            if (Instance.ClientIsOnVirtualMachine)
            {
               // get todays date only (in universal), we'll add the current time below
               currentFrameTime = new DateTime(DateTime.Today.Ticks, DateTimeKind.Utc);
            }
            else
            {
               // get todays date only, we'll add the current time below
               currentFrameTime = DateTime.Today;
            }

            Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Current TimeOfDay: {2}", Debug.FunctionName, Instance.Name, now.TimeOfDay));
            Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Current TimeOfLastFrame: {2}", Debug.FunctionName, Instance.Name, Instance.UnitInfo.TimeOfLastFrame));

            // the current time of day is less than the last frame time, then the frame was from the day prior
            // this should only happen after midnight time on the machine running HFM and when the client has 
            // not completed a frame since the local machine time rolled over to the next day
            if (now.TimeOfDay.Hours < Instance.UnitInfo.TimeOfLastFrame.Hours)
            {
               System.Diagnostics.Debug.WriteLine("Doing prior day adjustment...");

               // get today's date and subtract 1 day
               currentFrameTime = currentFrameTime.Subtract(new TimeSpan(1, 0, 0, 0));
            }

            // add Time of Last Frame (TimeSpan) to the DateTime with the correct date
            currentFrameTime = currentFrameTime.Add(Instance.UnitInfo.TimeOfLastFrame);

            if (currentFrameTime > terminalTime)
            {
               Instance.UnitInfo.Status = eClientStatus.Running;
            }
            else // current frame is less than terminal time
            {
               Instance.UnitInfo.Status = eClientStatus.Hung;
            }
         }
      }
      #endregion
   }
}
