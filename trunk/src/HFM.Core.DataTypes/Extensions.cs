/*
 * HFM.NET - Data Type Extension Methods
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

namespace HFM.Core.DataTypes
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

      #region IProtein

      public static bool IsUnknown(this Protein protein)
      {
         if (protein == null) return true;

         return protein.ProjectNumber == 0;
      }

      public static bool IsValid(this Protein protein)
      {
         if (protein == null) return false;

         return (protein.ProjectNumber > 0 &&
                 protein.PreferredDays > 0 &&
                 protein.MaximumDays > 0 &&
                 protein.Credit > 0 &&
                 protein.Frames > 0 &&
                 protein.KFactor >= 0);
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

      public static string ToWorkUnitResultString(this int result)
      {
         return ToWorkUnitResultString((WorkUnitResult)result);
      }

      public static string ToWorkUnitResultString(this WorkUnitResult result)
      {
         switch (result)
         {
            case WorkUnitResult.FinishedUnit:
               return FinishedUnit;
            case WorkUnitResult.EarlyUnitEnd:
               return EarlyUnitEnd;
            case WorkUnitResult.UnstableMachine:
               return UnstableMachine;
            case WorkUnitResult.Interrupted:
               return Interrupted;
            case WorkUnitResult.BadWorkUnit:
               return BadWorkUnit;
            case WorkUnitResult.CoreOutdated:
               return CoreOutdated;
            default:
               return String.Empty;
         }
      }

      #endregion

      #region IEnumerable<LogLine>

      public static readonly Regex WorkUnitRunningRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      private static readonly Regex FoldingSlotRegex =
         new Regex("(?<Timestamp>.{8}):FS(?<FoldingSlot>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      public static readonly Regex WorkUnitRunningRegex38 =
         new Regex("(?<Timestamp>.{8}):Unit (?<UnitIndex>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      /// <summary>
      /// Return the log lines from start index to end index.
      /// </summary>
      public static IEnumerable<LogLine> WhereLineIndex(this IEnumerable<LogLine> logLines, int startIndex, int endIndex)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         // end index cannot be less than start
         if (endIndex < startIndex)
         {
            // if so, return everything > than start index
            return logLines.Where(x => x.LineIndex >= startIndex);
         }
         return logLines.Where(x => x.LineIndex >= startIndex && x.LineIndex <= endIndex);
      }

      /// <summary>
      /// Filter v7 log lines by index.  Has no effect on legacy log lines.
      /// </summary>
      public static IEnumerable<LogLine> Filter(this IEnumerable<LogLine> logLines, LogFilterType filterType, int index)
      {
         if (logLines == null) throw new ArgumentNullException("logLines");

         switch (filterType)
         {
            case LogFilterType.UnitIndex:
               return FilterUnitIndex(logLines, index);
            case LogFilterType.UnitAndNonIndexed:
               return FilterUnitAndNonIndexed(logLines, index);
            case LogFilterType.SlotIndex:
               return FilterSlotIndex(logLines, index);
            case LogFilterType.SlotAndNonIndexed:
               return FilterSlotAndNonIndexed(logLines, index);
            default:
               return logLines;
         }
      }

      private static IEnumerable<LogLine> FilterUnitIndex(this IEnumerable<LogLine> logLines, int index)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match;
            if ((match = WorkUnitRunningRegex.Match(line.LineRaw)).Success ||
                (match = WorkUnitRunningRegex38.Match(line.LineRaw)).Success)
            {
               if (Int32.Parse(match.Result("${UnitIndex}")) == index)
               {
                  list.Add(line);
               }
            }
         }

         return list.AsReadOnly();
      }

      private static IEnumerable<LogLine> FilterUnitAndNonIndexed(this IEnumerable<LogLine> logLines, int index)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match;
            if ((match = WorkUnitRunningRegex.Match(line.LineRaw)).Success ||
                (match = WorkUnitRunningRegex38.Match(line.LineRaw)).Success)
            {
               if (Int32.Parse(match.Result("${UnitIndex}")) == index)
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

      private static IEnumerable<LogLine> FilterSlotIndex(this IEnumerable<LogLine> logLines, int index)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match;
            if ((match = WorkUnitRunningRegex.Match(line.LineRaw)).Success ||
                (match = FoldingSlotRegex.Match(line.LineRaw)).Success)
            {
               if (Int32.Parse(match.Result("${FoldingSlot}")) == index)
               {
                  list.Add(line);
               }
            }
         }

         return list.AsReadOnly();
      }

      private static IEnumerable<LogLine> FilterSlotAndNonIndexed(this IEnumerable<LogLine> logLines, int index)
      {
         Debug.Assert(logLines != null);

         var list = new List<LogLine>(logLines.Count());
         foreach (var line in logLines)
         {
            Match match;
            if ((match = WorkUnitRunningRegex.Match(line.LineRaw)).Success ||
                (match = FoldingSlotRegex.Match(line.LineRaw)).Success)
            {
               if (Int32.Parse(match.Result("${FoldingSlot}")) == index)
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

      #region SlotStatus

      /// <summary>
      /// Gets Status Html Color String
      /// </summary>
      public static string GetHtmlColor(this SlotStatus status)
      {
         return ColorTranslator.ToHtml(status.GetStatusColor());
      }

      /// <summary>
      /// Gets Status Html Font Color String
      /// </summary>
      public static string GetHtmlFontColor(this SlotStatus status)
      {
         switch (status)
         {
            case SlotStatus.Running:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.RunningAsync:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.RunningNoFrameTimes:
               return ColorTranslator.ToHtml(Color.Black);
            case SlotStatus.Stopped:
            case SlotStatus.EuePause:
            case SlotStatus.Hung:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.Paused:
               return ColorTranslator.ToHtml(Color.Black);
            case SlotStatus.SendingWorkPacket:
            case SlotStatus.GettingWorkPacket:
               return ColorTranslator.ToHtml(Color.White);
            case SlotStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.Black);
         }
      }

      /// <summary>
      /// Gets Status Color Object
      /// </summary>
      public static Color GetStatusColor(this SlotStatus status)
      {
         switch (status)
         {
            case SlotStatus.Running:
               return Color.Green;
            case SlotStatus.RunningAsync:
               return Color.Blue;
            case SlotStatus.RunningNoFrameTimes:
               return Color.Yellow;
            case SlotStatus.Stopped:
            case SlotStatus.EuePause:
            case SlotStatus.Hung:
               return Color.DarkRed;
            case SlotStatus.Paused:
               return Color.Orange;
            case SlotStatus.SendingWorkPacket:
            case SlotStatus.GettingWorkPacket:
               return Color.Purple;
            case SlotStatus.Offline:
               return Color.Gray;
            default:
               return Color.Gray;
         }
      }

      #endregion
   }
}
