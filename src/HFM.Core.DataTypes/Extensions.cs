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
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace HFM.Core.DataTypes
{
   public static class Extensions
   {
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

      #region Protein

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

      private static readonly Regex FoldingSlotRegex =
         new Regex("(?<Timestamp>.{8}):FS(?<FoldingSlot>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      // a copy of this regex exists in HFM.Log
      private static readonly Regex WorkUnitRunningRegex =
         new Regex("(?<Timestamp>.{8}):WU(?<UnitIndex>\\d{2}):FS(?<FoldingSlot>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

      // a copy of this regex exists in HFM.Log
      private static readonly Regex WorkUnitRunningRegex38 =
         new Regex("(?<Timestamp>.{8}):Unit (?<UnitIndex>\\d{2}):.*", RegexOptions.Compiled | RegexOptions.ExplicitCapture | RegexOptions.Singleline);

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

      #endregion
   }
}
