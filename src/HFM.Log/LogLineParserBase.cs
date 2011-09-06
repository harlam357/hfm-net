/*
 * HFM.NET - Log Line Parser Base Class
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
using System.Diagnostics;
using System.Text.RegularExpressions;

using HFM.Framework.DataTypes;

namespace HFM.Log
{
   internal abstract class LogLineParserBase
   {
      #region Regex (Static)

      /// <summary>
      /// Regular Expression to match Work Unit Project string.
      /// </summary>
      private static readonly Regex ProjectIDRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:].*Project: (?<ProjectNumber>.*) \\(Run (?<Run>.*), Clone (?<Clone>.*), Gen (?<Gen>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Core Version string.
      /// </summary>
      private static readonly Regex CoreVersionRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:].*Version (?<CoreVer>.*) \\(.*\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Standard and SMP Clients Frame Completion Lines (Gromacs Style).
      /// </summary>
      private static readonly Regex FramesCompletedRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:].*Completed (?<Completed>.*) out of (?<Total>.*) steps {1,2}\\((?<Percent>.*)\\)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 1
      /// </summary>
      private static readonly Regex Percent1Regex =
         new Regex("(?<Percent>.*) percent", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Percent Style 2
      /// </summary>
      private static readonly Regex Percent2Regex =
         new Regex("(?<Percent>.*)%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match GPU2 Client Frame Completion Lines
      /// </summary>
      private static readonly Regex FramesCompletedGpuRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:].*Completed (?<Percent>[0-9]{1,3})%", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Regular Expression to match Machine ID string.
      /// </summary>
      private static readonly Regex CoreShutdownRegex =
         new Regex("\\[?(?<Timestamp>.{8})[\\]|:].*Folding@home Core Shutdown: (?<UnitResult>.*)", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      #endregion

      #region Methods
      
      internal virtual object GetLineData(ILogLine logLine)
      {
         switch (logLine.LineType)
         {
            case LogLineType.WorkUnitProject:
               Match projectId;
               if ((projectId = ProjectIDRegex.Match(logLine.LineRaw)).Success)
               {
                  return projectId;
               }
               return new LogLineError(String.Format("Failed to parse Project (R/C/G) values from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreVersion:
               Match mCoreVer;
               if ((mCoreVer = CoreVersionRegex.Match(logLine.LineRaw)).Success)
               {
                  string sCoreVer = mCoreVer.Result("${CoreVer}").Trim();
                  float coreVersion;
                  if (Single.TryParse(sCoreVer, out coreVersion))
                  {
                     return sCoreVer;
                  }
                  return new LogLineError(String.Format("Failed to parse Core Version from '{0}'", logLine.LineRaw));
               }
               return null;
            case LogLineType.WorkUnitFrame:
               UnitFrame frame = GetUnitFrame(logLine);
               if (frame != null)
               {
                  return frame;
               }
               frame = GetGpuUnitFrame(logLine);
               if (frame != null)
               {
                  return frame;
               }
               return new LogLineError(String.Format("Failed to parse Frame Data from '{0}'", logLine.LineRaw));
            case LogLineType.WorkUnitCoreShutdown:
               Match mCoreShutdown;
               if ((mCoreShutdown = CoreShutdownRegex.Match(logLine.LineRaw)).Success)
               {
                  return mCoreShutdown.Result("${UnitResult}").ToWorkUnitResult();
               }
               return new LogLineError(String.Format("Failed to parse Work Unit Result value from '{0}'", logLine.LineRaw));
         }

         return null;
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (All other clients).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      protected static UnitFrame GetUnitFrame(ILogLine logLine)
      {
         Debug.Assert(logLine != null);

         Match mFramesCompleted = FramesCompletedRegex.Match(logLine.LineRaw);
         if (mFramesCompleted.Success)
         {
            var frame = new UnitFrame();

            int result;
            if (Int32.TryParse(mFramesCompleted.Result("${Completed}"), out result))
            {
               frame.RawFramesComplete = result;
            }
            else
            {
               return null;
            }

            if (Int32.TryParse(mFramesCompleted.Result("${Total}"), out result))
            {
               frame.RawFramesTotal = result;
            }
            else
            {
               return null;
            }

            string percentString = mFramesCompleted.Result("${Percent}");

            Match mPercent1 = Percent1Regex.Match(percentString);
            Match mPercent2 = Percent2Regex.Match(percentString);

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
            else if (!Int32.TryParse(percentString, out framePercent))
            {
               return null;
            }

            // Validate the steps are in tolerance with the detected frame percent - Issue 98
            double calculatedPercent = ((double)frame.RawFramesComplete / frame.RawFramesTotal) * 100;
            // ex. [00:19:40] Completed 82499 out of 250000 steps  (33%) - Would Validate
            //     [00:19:40] Completed 82750 out of 250000 steps  (33%) - Would Validate
            // 10% frame step tolerance. In the example the completed must be within 250 steps.
            if (Math.Abs(calculatedPercent - framePercent) <= 0.1)
            {
               frame.TimeOfFrame = logLine.ParseTimeStamp();
               frame.FrameID = framePercent;

               return frame;
            }
            /*** ProtoMol Only */
            // Issue 191 - New ProtoMol Projects don't report frame progress on the precent boundry.
            if (Math.Abs(calculatedPercent - (framePercent + 1)) <= 0.1)
            {
               frame.TimeOfFrame = logLine.ParseTimeStamp();
               frame.FrameID = framePercent + 1;

               return frame;
            }
            /*******************/

            return null;
         }

         return null;
      }

      /// <summary>
      /// Check the given log line for Completed Frame information (GPU Only).
      /// </summary>
      /// <param name="logLine">Log Line</param>
      protected static UnitFrame GetGpuUnitFrame(ILogLine logLine)
      {
         Debug.Assert(logLine != null);

         Match mFramesCompletedGpu = FramesCompletedGpuRegex.Match(logLine.LineRaw);
         if (mFramesCompletedGpu.Success)
         {
            var frame = new UnitFrame();

            frame.RawFramesComplete = Int32.Parse(mFramesCompletedGpu.Result("${Percent}"));
            frame.RawFramesTotal = 100; //Instance.CurrentProtein.Frames
            // I get this from the project data but what's the point. 100% is 100%.

            frame.TimeOfFrame = logLine.ParseTimeStamp();
            frame.FrameID = frame.RawFramesComplete;

            return frame;
         }

         return null;
      }

      #endregion
   }

   internal struct LogLineError
   {
      private readonly string _message;
      internal string Message
      {
         get { return _message; }
      }

      internal LogLineError(string message)
      {
         _message = message;
      }
   }
}
