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

using HFM.Proteins;
using Debug = HFM.Instrumentation.Debug;

namespace HFM.Instances
{
   #region Enum
   enum UnitToRead
   {
      Last,
      Previous1
   } 
   #endregion

   class LogParser
   {
      #region Members
      private readonly Regex rTimeStamp =
            new Regex("\\[(?<Timestamp>.*)\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      
      private readonly Regex rProjectNumber =
            new Regex("Project: (?<ProjectNumber>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rProjectNumberFromTag =
            new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      //private Regex rProtein =
      //      new Regex("Protein: (?<Protein>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rCoreVersion =
            new Regex("\\[(?<Timestamp>.*)\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rFramesCompleted =
            new Regex("\\[(?<Timestamp>.*)\\] Completed (?<Completed>.*) out of (?<Total>.*) steps  \\((?<Percent>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

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
            
      private readonly Regex rPercent1 =
            new Regex("(?<Percent>.*) percent", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private readonly Regex rPercent2 =
            new Regex("(?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private bool _bCoreFound = false;
      private bool _bReadUnitInfoFile = false;
      private bool _bUserIDFound = false;
      private bool _bMachineIDFound = false;

      // declare variables for log reading and starting points
      private string[] FAHLogText = null;
      private int _ClientStartPosition = 0;
      public int ClientStartPosition
      {
         get { return _ClientStartPosition; }
      }
      private int _LastUnitStartPosition = 0;
      public int LastUnitStartPosition
      {
         get { return _LastUnitStartPosition; }
      }
      private int _Previous1UnitStartPosition = 0;
      public int Previous1UnitStartPosition
      {
         get { return _Previous1UnitStartPosition; }
      }
      #endregion

      #region Parsing Methods
      /// <summary>
      /// Extract the content from the unitinfo.txt file produced by the folding
      /// console
      /// </summary>
      /// <param name="LogFileName">Full path to local copy of unitinfo.txt</param>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      public Boolean ParseUnitInfoFile(String LogFileName, ClientInstance Instance, UnitInfo parsedUnitInfo)
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
                  parsedUnitInfo.ProteinName = sData.Substring(6);
               }
               else if (sData.StartsWith("Tag:"))
               {
                  parsedUnitInfo.ProteinTag = sData.Substring(5);
                  DoProjectIDMatch(parsedUnitInfo, rProjectNumberFromTag.Match(parsedUnitInfo.ProteinTag));
               }
               else if (sData.StartsWith("Download time: "))
               {
                  if (Instance.ClientIsOnVirtualMachine)
                  {
                     parsedUnitInfo.DownloadTime =
                        DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo);
                  }
                  else
                  {
                     parsedUnitInfo.DownloadTime =
                        DateTime.ParseExact(sData.Substring(15), "MMMM d H:mm:ss",
                                            System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                            System.Globalization.DateTimeStyles.AssumeUniversal);
                  }
               }
               else if (sData.StartsWith("Due time: "))
               {
                  parsedUnitInfo.DueTime = DateTime.ParseExact(sData.Substring(10), "MMMM d H:mm:ss", System.Globalization.DateTimeFormatInfo.InvariantInfo, System.Globalization.DateTimeStyles.AssumeUniversal);
               }
               else if (sData.StartsWith("Progress: "))
               {
                  parsedUnitInfo.PercentComplete = Int32.Parse(sData.Substring(10, sData.IndexOf("%") - 10));
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
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="ReadUnit">Specify which work unit to parse from the FAHLog</param>
      /// <returns>Parsed Log File Information</returns>
      public UnitInfo ParseFAHLog(String LogFileName, ClientInstance Instance, UnitToRead ReadUnit)
      {
         DateTime Start = Debug.ExecStart;

         _bCoreFound = false;
         
         UnitInfo parsedUnitInfo = new UnitInfo(Instance.InstanceName, Instance.Path);

         int parseStart;
         int parseEnd = Int32.MaxValue;
         switch (ReadUnit)
         {
            case UnitToRead.Last:
               parseStart = _LastUnitStartPosition;
               break;
            case UnitToRead.Previous1:
               parseStart = _Previous1UnitStartPosition;
               parseEnd = _LastUnitStartPosition;
               break;
            default:
               throw new NotImplementedException(String.Format("Reads for type '{0}' are not yet implemented.", ReadUnit));
         }

         // start the parse loop where the client started last
         for (int i = _ClientStartPosition; i < FAHLogText.Length; i++)
         {
            string logLine = FAHLogText[i];
            // add the line to the instance log holder
            //Instance.CurrentLogText.Add(logLine);
            
            // Read Username and Team Number - Issue 5
            CheckForUserTeamAndIDs(Instance, parsedUnitInfo, logLine);

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

            // Get the UnitStart time stamp from the first line for this WU - Issue 10
            if (i == parseStart)
            {
               SetUnitStartTimeStamp(Instance, parsedUnitInfo, logLine);
            }

            // begin parsing the specified unit based on positions set above
            if (i >= parseStart && i < parseEnd)
            {
               // only parse the unitinfo.txt file when parsing the most recent unit log
               if (ReadUnit.Equals(UnitToRead.Last) && _bReadUnitInfoFile == false)
               {
                  if (ParseUnitInfoFile(Path.Combine(Instance.BaseDirectory, Instance.CachedUnitInfoName), Instance, parsedUnitInfo) == false)
                  {
                     Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) UnitInfo parse failed.", Debug.FunctionName, Instance.InstanceName));
                  }
                  _bReadUnitInfoFile = true;
               }

               // add the line to the instance log holder
               parsedUnitInfo.CurrentLogText.Add(logLine);

               CheckForProjectID(parsedUnitInfo, logLine);
               CheckForCoreVersion(parsedUnitInfo, logLine);

               // don't start parsing frames until we know the client type
               // we have to know the project number before we know the client type (see SetProjectID)
               if (parsedUnitInfo.TypeOfClient.Equals(ClientType.Unknown) == false)
               {
                  if (parsedUnitInfo.TypeOfClient.Equals(ClientType.GPU))
                  {
                     CheckForCompletedGpuFrame(Instance, parsedUnitInfo, logLine);
                  }
                  else //SMP or Standard
                  {
                     try
                     {
                        CheckForCompletedFrame(Instance, parsedUnitInfo, logLine);
                     }
                     catch (FormatException ex)
                     {
                        Debug.WriteToHfmConsole(TraceLevel.Warning, ex.Message);
                     }
                  }
               }

               if (logLine.Contains("+ Paused")) // || logLine.Contains("+ Running on battery power"))
               {
                  Instance.Status = ClientStatus.Paused;
               }
               if (logLine.Contains("+ Working ...") && Instance.Status.Equals(ClientStatus.Paused))
               {
                  Instance.Status = ClientStatus.RunningNoFrameTimes;
                  // Reset Frames Observed after Pause, this will cause the Instance to only use frames
                  // beyond this point to set frame times and determine status - Issue 13
                  parsedUnitInfo.FramesObserved = 0; 
               }
               if (logLine.Contains("Folding@Home Client Shutdown"))
               {
                  Instance.Status = ClientStatus.Stopped;
                  break; //we found a Shutdown message, quit parsing
               }
            }
         }

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} ({1}) Execution Time: {2}", Debug.FunctionName, Instance.InstanceName, Debug.GetExecTime(Start)));

         return parsedUnitInfo;
      }
      #endregion

      #region Parsing Helpers
      /// <summary>
      /// Reads Log File into string array then determines the parsing start points
      /// </summary>
      /// <param name="LogFileName">Log File Name</param>
      /// <returns>Success or Failure</returns>
      public bool ReadLogText(string LogFileName)
      {
         // reset FAHLog text and position variables
         FAHLogText = null;
         _ClientStartPosition = 0;
         _LastUnitStartPosition = 0;
         _Previous1UnitStartPosition = 0;

         // if file does not exist, get out
         if (!File.Exists(LogFileName)) return false;

         try
         {
            FAHLogText = File.ReadAllLines(LogFileName);
            GetParsingStartPositionsFromLog(FAHLogText, out _ClientStartPosition, out _LastUnitStartPosition, out _Previous1UnitStartPosition);
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
      /// <param name="ClientStartPosition">Client start message index in array</param>
      /// <param name="LastUnitStartPosition">Last unit start message index in array</param>
      /// <param name="Previous1UnitStartPosition">First previous unit start message index in array</param>
      private static void GetParsingStartPositionsFromLog(string[] FAHLogText, out int ClientStartPosition, out int LastUnitStartPosition, out int Previous1UnitStartPosition)
      {
         ClientStartPosition = 0;
         LastUnitStartPosition = 0;
         Previous1UnitStartPosition = 0;

         //work backwards through the file text to find starting positions
         for (int i = FAHLogText.Length - 1; i >= 0; i--)
         {
            string s = FAHLogText[i];
            if (s.Contains("--- Opening Log file"))
            {
               ClientStartPosition = i;
            }

            if (s.Contains("*------------------------------*"))
            {
               if (LastUnitStartPosition == 0)
               {
                  LastUnitStartPosition = i; // Set start of most current unit
               }
               else if (Previous1UnitStartPosition == 0)
               {
                  Previous1UnitStartPosition = i; // Set start of previous unit
               }
            }

            if (ClientStartPosition != 0 && LastUnitStartPosition != 0 && Previous1UnitStartPosition != 0)
            {
               //found our starting points, get out
               break;
            }
         }
      }

      /// <summary>
      /// Check the given log line for Project information
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForProjectID(UnitInfo parsedUnitInfo, string logLine)
      {
         Match mProjectNumber;
         if ((mProjectNumber = rProjectNumber.Match(logLine)).Success)
         {
            DoProjectIDMatch(parsedUnitInfo, rProteinID.Match(mProjectNumber.Result("${ProjectNumber}")));
         }
      }

      /// <summary>
      /// Attempts to Set Project ID with given Match.  If Project cannot be found in local cache, download again.
      /// </summary>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="match">Regex Match containing Project data</param>
      private void DoProjectIDMatch(UnitInfo parsedUnitInfo, Match match)
      {
         try
         {
            SetProjectID(parsedUnitInfo, match);
         }
         catch (System.Collections.Generic.KeyNotFoundException)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning,
                                    String.Format("{0} Project ID '{1}' not found in Protein Collection.",
                                                  Debug.FunctionName, parsedUnitInfo.ProjectID));

            // If a Project cannot be identified using the local Project data, update Project data from Stanford. - Issue 4
            Debug.WriteToHfmConsole(TraceLevel.Info,
                                    String.Format("{0} Attempting to download new Project data...", Debug.FunctionName));
            ProteinCollection.Instance.DownloadFromStanford(null);
            try
            {
               SetProjectID(parsedUnitInfo, rProjectNumberFromTag.Match(parsedUnitInfo.ProteinTag));
            }
            catch (System.Collections.Generic.KeyNotFoundException)
            {
               Debug.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("{0} Project ID '{1}' not found on Stanford Web Project Summary.",
                                                     Debug.FunctionName, parsedUnitInfo.ProjectID));
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
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="match">Project string match</param>
      /// <exception cref="System.Collections.Generic.KeyNotFoundException">Thrown when Project ID cannot be found in Protein Collection.</exception>
      /// <exception cref="FormatException">Thrown when Project ID string fails to parse.</exception>
      private static void SetProjectID(UnitInfo parsedUnitInfo, Match match)
      {
         if (match.Success)
         {
            // once a current frame exists we already have the ProjectID
            // when a previous unit is returned during this unit's progress the project string
            // is drawn just as it is drawn at the beginning of a unit, we don't want this string
            // to override the ProjectID we already captured
            if (parsedUnitInfo.CurrentFrame == null)
            {
               parsedUnitInfo.ProjectID = Int32.Parse(match.Result("${ProjectNumber}"));
               parsedUnitInfo.ProjectRun = Int32.Parse(match.Result("${Run}"));
               parsedUnitInfo.ProjectClone = Int32.Parse(match.Result("${Clone}"));
               parsedUnitInfo.ProjectGen = Int32.Parse(match.Result("${Gen}"));

               parsedUnitInfo.CurrentProtein = ProteinCollection.Instance[parsedUnitInfo.ProjectID];
               parsedUnitInfo.TypeOfClient = GetClientTypeFromProtein(parsedUnitInfo.CurrentProtein);
            }
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
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForCoreVersion(UnitInfo parsedUnitInfo, string logLine)
      {
         Match mCoreVer;
         if (_bCoreFound == false && (mCoreVer = rCoreVersion.Match(logLine)).Success)
         {
            string sCoreVer = mCoreVer.Result("${CoreVer}");
            parsedUnitInfo.CoreVersion = sCoreVer.Substring(0, sCoreVer.IndexOf(" "));
            _bCoreFound = true;
         }
      }

      /// <summary>
      /// Check the given log line for User, Team, and Machine related ID values
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForUserTeamAndIDs(ClientInstance Instance, UnitInfo parsedUnitInfo, string logLine)
      {
         Match mUserTeam;
         if ((mUserTeam = rUserTeam.Match(logLine)).Success)
         {
            parsedUnitInfo.FoldingID = mUserTeam.Result("${Username}");
            parsedUnitInfo.Team = int.Parse(mUserTeam.Result("${TeamNumber}"));
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
      /// Get the time stamp from this log line and set as the unit's start time
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void SetUnitStartTimeStamp(ClientInstance Instance, UnitInfo parsedUnitInfo, string logLine)
      {
         Match mTimeStamp = rTimeStamp.Match(logLine);
         if (mTimeStamp.Success)
         {
            System.Globalization.DateTimeStyles style;

            if (Instance.ClientIsOnVirtualMachine)
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

            DateTime timeStamp = DateTime.ParseExact(mTimeStamp.Result("${Timestamp}"), "HH:mm:ss",
                                                     System.Globalization.DateTimeFormatInfo.InvariantInfo,
                                                     style);

            parsedUnitInfo.UnitStartTime = timeStamp.TimeOfDay;
         }
         else
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} ({1}) Failed to set 'UnitStartTime'.", Debug.FunctionName, Instance.InstanceName));
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only)
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForCompletedGpuFrame(ClientInstance Instance, UnitInfo parsedUnitInfo, string logLine)
      {
         Match mFramesCompletedGpu = rFramesCompletedGpu.Match(logLine);
         if (mFramesCompletedGpu.Success)
         {
            parsedUnitInfo.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            parsedUnitInfo.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            //TODO: Hard code here, 100 GPU Frames. Could I get this from the Project Data?
            //I could but what's the point, 100% is 100%.

            SetTimeStamp(Instance, parsedUnitInfo, mFramesCompletedGpu.Result("${Timestamp}"), parsedUnitInfo.RawFramesComplete);
         }
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients)
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="logLine">Log Line</param>
      private void CheckForCompletedFrame(ClientInstance Instance, UnitInfo parsedUnitInfo, string logLine)
      {
         Match mFramesCompleted = rFramesCompleted.Match(logLine);
         if (mFramesCompleted.Success)
         {
            try
            {
               parsedUnitInfo.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               parsedUnitInfo.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
            }
            catch (FormatException ex)
            {
               throw new FormatException(String.Format("{0} Failed to parse raw frame values from '{1}'.", Debug.FunctionName, logLine), ex);
            }

            Match mPercent1 = rPercent1.Match(mFramesCompleted.Result("${Percent}"));
            Match mPercent2 = rPercent2.Match(mFramesCompleted.Result("${Percent}"));

            int percent;
            if (mPercent1.Success)
            {
               percent = Int32.Parse(mPercent1.Result("${Percent}"));
            }
            else if (mPercent2.Success)
            {
               percent = Int32.Parse(mPercent2.Result("${Percent}"));
            }
            else
            {
               throw new FormatException(String.Format("{0} Failed to parse frame percent from '{1}'.", Debug.FunctionName, logLine));
            }

            SetTimeStamp(Instance, parsedUnitInfo, mFramesCompleted.Result("${Timestamp}"), percent);
         }
      }

      /// <summary>
      /// Set the Raw Time per Section based on the given and previously read 
      /// </summary>
      /// <param name="Instance">Client Instance that owns the log file we're parsing</param>
      /// <param name="parsedUnitInfo">Container for parsed information</param>
      /// <param name="timeStampString">String containing the current frame time stamp</param>
      /// <param name="percent">Frame Percentage</param>
      private static void SetTimeStamp(ClientInstance Instance, UnitInfo parsedUnitInfo, string timeStampString, int percent)
      {
         System.Globalization.DateTimeStyles style;

         if (Instance.ClientIsOnVirtualMachine)
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

         DateTime timeStamp = DateTime.ParseExact(timeStampString, "HH:mm:ss",
                               System.Globalization.DateTimeFormatInfo.InvariantInfo,
                               style);

         parsedUnitInfo.SetCurrentFrame(new UnitFrame(percent, timeStamp.TimeOfDay));
      }
      #endregion
   }
}