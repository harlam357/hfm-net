/*
 * HFM.NET - Data Type Extension Methods
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HFM.Framework.DataTypes
{
   public static class Extensions
   {
      #region DateTime/TimeSpan

      public static bool IsKnown(this DateTime dateTime)
      {
         return !IsUnknown(dateTime);
      }
   
      public static bool IsUnknown(this DateTime dateTime)
      {
         return dateTime.Equals(DateTime.MinValue);
      }
      
      public static bool IsZero(this TimeSpan timeSpan)
      {
         return timeSpan.Equals(TimeSpan.Zero);
      }

      #endregion

      #region Color

      public static Color FindNearestKnown(this Color c)
      {
         var best = new ColorName { Name = null };

         foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
         {
            Color known = Color.FromName(colorName);
            int dist = Math.Abs(c.R - known.R) + Math.Abs(c.G - known.G) + Math.Abs(c.B - known.B);

            if (best.Name == null || dist < best.Distance)
            {
               best.Color = known;
               best.Name = colorName;
               best.Distance = dist;
            }
         }

         return best.Color;
      }

      struct ColorName
      {
         public Color Color;
         public string Name;
         public int Distance;
      }

      #endregion

      #region IProjectInfo

      /// <summary>
      /// Is Project Unknown?
      /// </summary>
      /// <returns>true if Project (R/C/G) has not been identified.</returns>
      public static bool ProjectIsUnknown(this IProjectInfo projectInfo)
      {
         if (projectInfo == null) return true;

         return projectInfo.ProjectID == 0 &&
                projectInfo.ProjectRun == 0 &&
                projectInfo.ProjectClone == 0 &&
                projectInfo.ProjectGen == 0;
      }

      /// <summary>
      /// Formatted Project (R/C/G) information.
      /// </summary>
      public static string ProjectRunCloneGen(this IProjectInfo projectInfo)
      {
         if (projectInfo == null) return String.Empty;

         return String.Format(CultureInfo.InvariantCulture, "P{0} (R{1}, C{2}, G{3})", 
            projectInfo.ProjectID, projectInfo.ProjectRun, projectInfo.ProjectClone, projectInfo.ProjectGen);
      }

      /// <summary>
      /// Equals Project (R/C/G)
      /// </summary>
      /// <returns>true if Project (R/C/G) are equal.</returns>
      public static bool EqualsProject(this IProjectInfo projectInfo1, IProjectInfo projectInfo2)
      {
         if (projectInfo1 == null || projectInfo2 == null) return false;

         return (projectInfo1.ProjectID == projectInfo2.ProjectID &&
                 projectInfo1.ProjectRun == projectInfo2.ProjectRun &&
                 projectInfo1.ProjectClone == projectInfo2.ProjectClone &&
                 projectInfo1.ProjectGen == projectInfo2.ProjectGen);
      }

      #endregion

      #region ClientStatus

      /// <summary>
      /// Gets Status Color Pen Object
      /// </summary>
      public static Pen GetDrawingPen(this ClientStatus status)
      {
         return new Pen(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Color Brush Object
      /// </summary>
      public static SolidBrush GetDrawingBrush(this ClientStatus status)
      {
         return new SolidBrush(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      public static string GetHtmlColor(this ClientStatus status)
      {
         return ColorTranslator.ToHtml(GetStatusColor(status));
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      public static string GetHtmlFontColor(this ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.RunningAsync:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.RunningNoFrameTimes:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Paused:
               return ColorTranslator.ToHtml(Color.Black);
            case ClientStatus.SendingWorkPacket:
            case ClientStatus.GettingWorkPacket:
               return ColorTranslator.ToHtml(Color.White);
            case ClientStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.Black);
         }
      }

      /// <summary>
      /// Gets Status Color Object
      /// </summary>
      private static Color GetStatusColor(ClientStatus status)
      {
         switch (status)
         {
            case ClientStatus.Running:
               return Color.Green;
            case ClientStatus.RunningAsync:
               return Color.Blue;
            case ClientStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case ClientStatus.Stopped:
            case ClientStatus.EuePause:
            case ClientStatus.Hung:
               return Color.DarkRed;
            case ClientStatus.Paused:
               return Color.Orange;
            case ClientStatus.SendingWorkPacket:
            case ClientStatus.GettingWorkPacket:
               return Color.Purple;
            case ClientStatus.Offline:
               return Color.Gray;
            default:
               return Color.Gray;
         }
      }

      #endregion
      
      #region WorkUnitResult

      private const string FinishedUnit = "FINISHED_UNIT";
      private const string EarlyUnitEnd = "EARLY_UNIT_END";
      private const string UnstableMachine = "UNSTABLE_MACHINE";
      private const string Interrupted = "INTERRUPTED";
      private const string BadWorkUnit = "BAD_WORK_UNIT";
      private const string CoreOutdated = "CORE_OUTDATED";

      /// <summary>
      /// Get the WorkUnitResult Enum representation of the given result string.
      /// </summary>
      /// <param name="result">Work Unit Result as String.</param>
      public static WorkUnitResult ToWorkUnitResult(this string result)
      {
         switch (result)
         {
            case FinishedUnit:
               return WorkUnitResult.FinishedUnit;
            case EarlyUnitEnd:
               return WorkUnitResult.EarlyUnitEnd;
            case UnstableMachine:
               return WorkUnitResult.UnstableMachine;
            case Interrupted:
               return WorkUnitResult.Interrupted;
            case BadWorkUnit:
               return WorkUnitResult.BadWorkUnit;
            case CoreOutdated:
               return WorkUnitResult.CoreOutdated;
            default:
               return WorkUnitResult.Unknown;
         }
      }

      #endregion

      #region IEnumerable<LogLine>

      public static readonly Regex WorkUnitRunningRegex =
         new Regex("(?<Timestamp>.{8}):Unit (?<UnitIndex>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Return the log lines from start index to end index.
      /// </summary>
      public static IEnumerable<LogLine> WhereLineIndex(this IEnumerable<LogLine> logLines, int startIndex, int endIndex)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         return logLines.Where(x => x.LineIndex >= startIndex && x.LineIndex <= endIndex);
      }

      /// <summary>
      /// Filter v7 log lines by queue index (:Unit xx:).  Has no effect on legacy log lines.
      /// </summary>
      public static IEnumerable<LogLine> Filter(this IEnumerable<LogLine> logLines, LogFilterType filterType)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         ILogLine logLine = logLines.FirstOrDefault();
         if (logLine == null)
         {
            return logLines;
         }
         if (!logLine.LineType.Equals(LogLineType.WorkUnitWorking))
         {
            return logLines;
         }

         var queueIndex = (int)logLine.LineData;
         if (filterType.Equals(LogFilterType.IndexOnly))
         {
            return FilterIndexOnly(logLines, queueIndex);
         }
         return FilterIndexAndNonIndexed(logLines, queueIndex);
      }

      private static IEnumerable<LogLine> FilterIndexOnly(this IEnumerable<LogLine> logLines, int queueIndex)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match = WorkUnitRunningRegex.Match(line.LineRaw);
            if (match.Success && Int32.Parse(match.Result("${UnitIndex}")) == queueIndex)
            {
               list.Add(line);
            }
         }

         return list.AsReadOnly();
      }

      private static IEnumerable<LogLine> FilterIndexAndNonIndexed(this IEnumerable<LogLine> logLines, int queueIndex)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match = WorkUnitRunningRegex.Match(line.LineRaw);
            if (match.Success)
            {
               if (Int32.Parse(match.Result("${UnitIndex}")) == queueIndex)
               {
                  list.Add(line);
               }
            }
            else
            {
               list.Add(line);
            }
         }

         return list.AsReadOnly();
      }

      /// <summary>
      /// Return the first project info found in the log lines or null.
      /// </summary>
      public static IProjectInfo FirstProjectInfoOrDefault(this IEnumerable<LogLine> logLines)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         var projectLine = logLines.FirstOrDefault(x => x.LineType.Equals(LogLineType.WorkUnitProject));
         if (projectLine == null)
         {
            return null;
         }

         var match = (Match)projectLine.LineData;
         return new ProjectInfo
         {
            ProjectID = Int32.Parse(match.Result("${ProjectNumber}")),
            ProjectRun = Int32.Parse(match.Result("${Run}")),
            ProjectClone = Int32.Parse(match.Result("${Clone}")),
            ProjectGen = Int32.Parse(match.Result("${Gen}"))
         };
      }

      #endregion
   }
}
