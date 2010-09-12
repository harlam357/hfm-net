/*
 * HFM.NET - Log Line Class
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
using System.Diagnostics;
using System.Text.RegularExpressions;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Log
{
   internal static class LogLine
   {
      #region Regex (Static)
      // ReSharper disable InconsistentNaming
      /// <summary>
      /// Regular Expression to match User (Folding ID) and Team string.
      /// </summary>
      private static readonly Regex rUserTeam =
         new Regex("\\[(?<Timestamp>.{8})\\] - User name: (?<Username>.*) \\(Team (?<TeamNumber>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rReceivedUserID =
         new Regex("\\[(?<Timestamp>.{8})\\].*- Received User ID = (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match User ID string.
      /// </summary>
      private static readonly Regex rUserID =
         new Regex("\\[(?<Timestamp>.{8})\\] - User ID: (?<UserID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rMachineID =
         new Regex("\\[(?<Timestamp>.{8})\\] - Machine ID: (?<MachineID>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rUnitIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on Unit 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Unit Index string.
      /// </summary>
      private static readonly Regex rQueueIndex =
         new Regex("\\[(?<Timestamp>.{8})\\] Working on queue slot 0(?<QueueIndex>[\\d]) \\[.*\\]", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Core Version string.
      /// </summary>
      private static readonly Regex rCoreVersion =
         new Regex("\\[(?<Timestamp>.{8})\\] Version (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /*** ProtoMol Only */
      /// <summary>
      /// Regular Expression to match ProtoMol Core Version string.
      /// </summary>
      private static readonly Regex rProtoMolCoreVersion =
         new Regex("\\[(?<Timestamp>.{8})\\]   Version: (?<CoreVer>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      /*******************/

      /// <summary>
      /// Regular Expression to match Work Unit Project string.
      /// </summary>
      private static readonly Regex rProjectID =
         new Regex("\\[(?<Timestamp>.{8})\\] Project: (?<ProjectNumber>.*) \\(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Standard and SMP Clients Frame Completion Lines (Gromacs Style).
      /// </summary>
      private static readonly Regex rFramesCompleted =
         new Regex("\\[(?<Timestamp>.{8})\\] Completed (?<Completed>.*) out of (?<Total>.*) steps {1,2}\\((?<Percent>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 1
      /// </summary>
      private static readonly Regex rPercent1 =
         new Regex("(?<Percent>.*) percent", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 2
      /// </summary>
      private static readonly Regex rPercent2 =
            new Regex("(?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match GPU2 Client Frame Completion Lines
      /// </summary>
      private static readonly Regex rFramesCompletedGpu =
            new Regex("\\[(?<Timestamp>.{8})\\] Completed (?<Percent>[0-9]{1,3})%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex rCoreShutdown =
         new Regex("\\[(?<Timestamp>.{8})\\] Folding@home Core Shutdown: (?<UnitResult>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Completed Work Units string.
      /// </summary>
      private static readonly Regex rCompletedWUs =
         new Regex("\\[(?<Timestamp>.{8})\\] \\+ Number of Units Completed: (?<Completed>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);
      // ReSharper restore InconsistentNaming
      #endregion

      #region Methods
      
      public static object GetLineData(ILogLine logLine)
      {
         switch (logLine.LineType)
         {
            case LogLineType.ClientVersion:
               int versionIndex = logLine.LineRaw.IndexOf("Version", StringComparison.Ordinal) + 8;
               return versionIndex < logLine.LineRaw.Length ? logLine.LineRaw.Substring(versionIndex) : String.Empty;
            case LogLineType.ClientArguments:
               return logLine.LineRaw.Substring(10).Trim();
            case LogLineType.ClientUserNameTeam:
               Match mUserTeam;
               if ((mUserTeam = rUserTeam.Match(logLine.LineRaw)).Success)
               {
                  ArrayList list = new ArrayList(2);
                  list.Add(mUserTeam.Result("${Username}"));
                  list.Add(Int32.Parse(mUserTeam.Result("${TeamNumber}")));
                  return list;
               }
               throw new FormatException(String.Format("Failed to parse User Name and Team values from '{0}'", logLine.LineRaw));
            case LogLineType.ClientReceivedUserID:
               Match receivedUserId;
               if ((receivedUserId = rReceivedUserID.Match(logLine.LineRaw)).Success)
               {
                  return receivedUserId.Result("${UserID}");
               }
               throw new FormatException(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientUserID:
               Match userId;
               if ((userId = rUserID.Match(logLine.LineRaw)).Success)
               {
                  return userId.Result("${UserID}");
               }
               throw new FormatException(String.Format("Failed to parse User ID value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientMachineID:
               Match machineId;
               if ((machineId = rMachineID.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(machineId.Result("${MachineID}"));
               }
               throw new FormatException(String.Format("Failed to parse Machine ID value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitIndex:
               Match mUnitIndex;
               if ((mUnitIndex = rUnitIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mUnitIndex.Result("${QueueIndex}"));
               }
               throw new FormatException(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitQueueIndex:
               Match mQueueIndex;
               if ((mQueueIndex = rQueueIndex.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mQueueIndex.Result("${QueueIndex}"));
               }
               throw new FormatException(String.Format("Failed to parse Work Unit Queue Index from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreVersion:
               Match mCoreVer;
               if ((mCoreVer = rCoreVersion.Match(logLine.LineRaw)).Success)
               {
                  string sCoreVer = mCoreVer.Result("${CoreVer}");
                  if (sCoreVer.IndexOf(" ") > 1)
                  {
                     return sCoreVer.Substring(0, sCoreVer.IndexOf(" "));
                  }
                  return sCoreVer;
               }
               /*** ProtoMol Only */
               if ((mCoreVer = rProtoMolCoreVersion.Match(logLine.LineRaw)).Success)
               {
                  return mCoreVer.Result("${CoreVer}");
               }
               /*******************/
               throw new FormatException(String.Format("Failed to parse Core Version value from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitProject:
               Match projectId;
               if ((projectId = rProjectID.Match(logLine.LineRaw)).Success)
               {
                  return projectId;
               }
               throw new FormatException(String.Format("Failed to parse Project (R/C/G) values from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitFrame:
               var frame = new UnitFrame();
               if (CheckForCompletedFrame(logLine, frame))
               {
                  return frame;
               }
               if (CheckForCompletedGpuFrame(logLine, frame))
               {
                  return frame;
               }
               throw new FormatException(String.Format("Failed to parse Frame Data from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreShutdown:
               Match mCoreShutdown;
               if ((mCoreShutdown = rCoreShutdown.Match(logLine.LineRaw)).Success)
               {
                  return StringOps.WorkUnitResultFromString(mCoreShutdown.Result("${UnitResult}"));
               }
               throw new FormatException(String.Format("Failed to parse Work Unit Result value from '{0}'", logLine.LineRaw));
            case LogLineType.ClientNumberOfUnitsCompleted:
               Match mCompletedWUs;
               if ((mCompletedWUs = rCompletedWUs.Match(logLine.LineRaw)).Success)
               {
                  return Int32.Parse(mCompletedWUs.Result("${Completed}"));
               }
               throw new FormatException(String.Format("Failed to parse Units Completed value from '{0}'", logLine.LineRaw));
         }

         return null;
      }

      public static string GetProjectString(ILogLine line)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            int projectId = Int32.Parse(match.Result("${ProjectNumber}"));
            int projectRun = Int32.Parse(match.Result("${Run}"));
            int projectClone = Int32.Parse(match.Result("${Clone}"));
            int projectGen = Int32.Parse(match.Result("${Gen}"));

            return String.Format("P{0} (R{1}, C{2}, G{3})", projectId,
                                                            projectRun,
                                                            projectClone,
                                                            projectGen);
         }

         throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
      }

      public static string GetLongProjectString(ILogLine line)
      {
         if (line.LineType.Equals(LogLineType.WorkUnitProject))
         {
            Match match = (Match)line.LineData;

            int projectId = Int32.Parse(match.Result("${ProjectNumber}"));
            int projectRun = Int32.Parse(match.Result("${Run}"));
            int projectClone = Int32.Parse(match.Result("${Clone}"));
            int projectGen = Int32.Parse(match.Result("${Gen}"));

            return String.Format("{0} (Run {1}, Clone {2}, Gen {3})", projectId,
                                                                      projectRun,
                                                                      projectClone,
                                                                      projectGen);
         }

         throw new ArgumentException(String.Format("Log line is not of type '{0}'", LogLineType.WorkUnitProject), "line");
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      /// <param name="frame">Frame Data</param>
      private static bool CheckForCompletedFrame(ILogLine logLine, UnitFrame frame)
      {
         Debug.Assert(logLine != null);
         Debug.Assert(frame != null);
      
         Match mFramesCompleted = rFramesCompleted.Match(logLine.LineRaw);
         if (mFramesCompleted.Success)
         {
            try
            {
               frame.RawFramesComplete = Int32.Parse(mFramesCompleted.Result("${Completed}"));
               frame.RawFramesTotal = Int32.Parse(mFramesCompleted.Result("${Total}"));
            }
            catch (FormatException ex)
            {
               throw new FormatException(String.Format("{0} Failed to parse raw frame values from '{1}'.", HfmTrace.FunctionName, logLine), ex);
            }

            string percentString = mFramesCompleted.Result("${Percent}");

            Match mPercent1 = rPercent1.Match(percentString);
            Match mPercent2 = rPercent2.Match(percentString);

            int framePercent;
            if (mPercent1.Success)
            {
               framePercent = Int32.Parse(mPercent1.Result("${Percent}"));
            }
            else if (mPercent2.Success)
            {
               framePercent = Int32.Parse(mPercent2.Result("${Percent}"));
            }
            // Try to parse a percentage from in between the parentheses (for older single core clients like v5.02) - Issue 36
            else if (Int32.TryParse(percentString, out framePercent) == false)
            {
               throw new FormatException(String.Format("Failed to parse frame percent from '{0}'.", logLine));
            }

            // Validate the steps are in tolerance with the detected frame percent - Issue 98
            double calculatedPercent = ((double)frame.RawFramesComplete / frame.RawFramesTotal) * 100;
            // ex. [00:19:40] Completed 82499 out of 250000 steps  (33%) - Would Validate
            //     [00:19:40] Completed 82750 out of 250000 steps  (33%) - Would Validate
            // 10% frame step tolerance. In the example the completed must be within 250 steps.
            if (Math.Abs(calculatedPercent - framePercent) <= 0.1)
            {
               frame.TimeStampString = mFramesCompleted.Result("${Timestamp}");
               frame.FrameID = framePercent;

               return true;
            }
            /*** ProtoMol Only */
            // Issue 191 - New ProtoMol Projects don't report frame progress on the precent boundry.
            if (Math.Abs(calculatedPercent - (framePercent + 1)) <= 0.1)
            {
               frame.TimeStampString = mFramesCompleted.Result("${Timestamp}");
               frame.FrameID = framePercent + 1;

               return true;
            }
            /*******************/
            
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("Not on percent boundry '{0}' (this is not a problem).", logLine), true);
            return false;
         }

         return false;
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      /// <param name="frame">Frame Data</param>
      private static bool CheckForCompletedGpuFrame(ILogLine logLine, UnitFrame frame)
      {
         Debug.Assert(logLine != null);
         Debug.Assert(frame != null);
      
         Match mFramesCompletedGpu = rFramesCompletedGpu.Match(logLine.LineRaw);
         if (mFramesCompletedGpu.Success)
         {
            logLine.LineType = LogLineType.WorkUnitFrame;

            frame.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            frame.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            //TODO: Hard code here, 100 GPU Frames. Could I get this from the Project Data?
            //I could but what's the point, 100% is 100%.

            frame.TimeStampString = mFramesCompletedGpu.Result("${Timestamp}");
            frame.FrameID = frame.RawFramesComplete;

            return true;
         }

         return false;
      }
      
      #endregion
   }
}
