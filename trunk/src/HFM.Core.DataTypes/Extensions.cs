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
using System.Text;
using System.Text.RegularExpressions;

namespace HFM.Core.DataTypes
{
   /// <summary>
   /// Shared data type extension methods class.
   /// </summary>
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
      private const string GpuMemtestError = "GPU_MEMTEST_ERROR";

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
            case GpuMemtestError:
               return WorkUnitResult.GpuMemtestError;
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
            case WorkUnitResult.GpuMemtestError:
               return GpuMemtestError;
            default:
               return String.Empty;
         }
      }

      #endregion

      #region CpuType

      public static string ToCpuTypeString(this CpuType type)
      {
         switch (type)
         {
            case CpuType.Core2:
               return "Core 2";
            case CpuType.Corei7:
               return "Core i7";
            case CpuType.Corei5:
               return "Core i5";
            case CpuType.Corei3:
               return "Core i3";
            case CpuType.PhenomII:
               return "Phenom II";
            case CpuType.Phenom:
               return "Phenom";
            case CpuType.Athlon:
               return "Athlon";
         }

         return "Unknown";
      }

      #endregion

      #region OperatingSystemType

      public static string ToOperatingSystemString(this OperatingSystemType type)
      {
         return ToOperatingSystemString(type, OperatingSystemArchitectureType.Unknown);
      }

      public static string ToOperatingSystemString(this OperatingSystemType type, OperatingSystemArchitectureType arch)
      {
         string osName = "Unknown";

         switch (type)
         {
            case OperatingSystemType.Windows:
               osName = "Windows";
               break;
            case OperatingSystemType.WindowsXP:
               osName = "Windows XP";
               break;
            case OperatingSystemType.WindowsVista:
               osName = "Vista";
               break;
            case OperatingSystemType.Windows7:
               osName = "Windows 7";
               break;
            case OperatingSystemType.Linux:
               osName = "Linux";
               break;
            case OperatingSystemType.OSX:
               osName = "OS X";
               break;
         }

         return arch.Equals(OperatingSystemArchitectureType.Unknown) ? osName : String.Format(CultureInfo.InvariantCulture, "{0} {1}", osName, arch);
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

      #region StringBuilder

      ///// <summary>
      ///// Reports the index of the first occurrence of the specified string in this instance.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder to search.</param>
      ///// <param name="value">The string to seek.</param>
      ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
      ///// <exception cref="System.ArgumentNullException">value is null.</exception>
      //public static int IndexOf(this StringBuilder sb, string value)
      //{
      //   return IndexOf(sb, value, 0);
      //}

      ///// <summary>
      ///// Reports the index of the first occurrence of the specified string in this instance. The search starts at a specified character position.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder to search.</param>
      ///// <param name="value">The string to seek.</param>
      ///// <param name="startIndex">The search starting position.</param>
      ///// <returns>The zero-based index position of value if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
      ///// <exception cref="System.ArgumentNullException">value is null.</exception>
      ///// <exception cref="System.ArgumentOutOfRangeException">startIndex is negative. -or- startIndex specifies a position not within this instance.</exception>
      //public static int IndexOf(this StringBuilder sb, string value, int startIndex)
      //{
      //   return IndexOf(sb, value, startIndex, StringComparison.CurrentCulture);
      //}

      /// <summary>
      /// Reports the index of the first occurrence of the specified string in the current System.String object. A parameter specifies the type of search to use for the specified string.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder to search.</param>
      /// <param name="value">The string to seek.</param>
      /// <param name="ignoreCase">Ignore character case.</param>
      /// <returns>The index position of the value parameter if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is 0.</returns>
      /// <exception cref="System.ArgumentNullException">value is null.</exception>
      public static int IndexOf(this StringBuilder sb, string value, bool ignoreCase)
      {
         return IndexOf(sb, value, 0, ignoreCase);
      }

      /// <summary>
      /// Reports the index of the first occurrence of the specified string in the current System.String object. Parameters specify the starting search position in the current string and the type of search to use for the specified string.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder to search.</param>
      /// <param name="value">The string to seek.</param>
      /// <param name="startIndex">The search starting position.</param>
      /// <param name="ignoreCase">Ignore character case.</param>
      /// <returns>The zero-based index position of the value parameter if that string is found, or -1 if it is not. If value is System.String.Empty, the return value is startIndex.</returns>
      /// <exception cref="System.ArgumentNullException">value is null.</exception>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex is negative. -or- startIndex specifies a position not within this instance.</exception>
      public static int IndexOf(this StringBuilder sb, string value, int startIndex, bool ignoreCase)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (value == null) throw new ArgumentNullException("value");
         if (startIndex < 0 || startIndex > sb.Length) throw new ArgumentOutOfRangeException("startIndex");

         if (value.Length == 0)
         {
            return startIndex;
         }

         int index;
         int length = value.Length;
         int maxSearchLength = (sb.Length - length) + 1;

         if (ignoreCase)
         {
            for (int i = startIndex; i < maxSearchLength; ++i)
            {
               if (Char.ToLower(sb[i]) == Char.ToLower(value[0]))
               {
                  index = 1;
                  while ((index < length) && (Char.ToLower(sb[i + index]) == Char.ToLower(value[index])))
                  {
                     ++index;
                  }

                  if (index == length)
                  {
                     return i;
                  }
               }
            }

            return -1;
         }

         for (int i = startIndex; i < maxSearchLength; ++i)
         {
            if (sb[i] == value[0])
            {
               index = 1;
               while ((index < length) && (sb[i + index] == value[index]))
               {
                  ++index;
               }

               if (index == length)
               {
                  return i;
               }
            }
         }

         return -1;
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <returns>A string that is equivalent to the substring of length length that begins at startIndex in this instance, or System.String.Empty if startIndex is equal to the length of this instance and length is zero.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static string Substring(this StringBuilder sb, int startIndex, int length)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         var temp = new char[length];
         sb.CopyTo(startIndex, temp, 0, length);
         return new string(temp);
      }

      ///// <summary>
      ///// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      ///// </summary>
      ///// <param name="sb">The System.Text.StringBuilder source instance.</param>
      ///// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      ///// <param name="length">The number of characters in the substring.</param>
      ///// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      ///// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      //public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, int length)
      //{
      //   return SubstringBuilder(sb, startIndex, length, null);
      //}

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="dest">The System.Text.StringBuilder destination instance. If null a new StringBuilder will be returned.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, StringBuilder dest, int length)
      {
         return SubstringBuilder(sb, startIndex, dest, length, false);
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <param name="trimOnly">if true trim the existing StringBuilder instance. -or- if false create a new StringBuilder instance and leave the existing instance in tact.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      public static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, int length, bool trimOnly)
      {
         return SubstringBuilder(sb, startIndex, null, length, trimOnly);
      }

      /// <summary>
      /// Retrieves a substring from this instance. The substring starts at a specified character position and has a specified length.
      /// </summary>
      /// <param name="sb">The System.Text.StringBuilder source instance.</param>
      /// <param name="startIndex">The zero-based starting character position of a substring in this instance.</param>
      /// <param name="dest">The System.Text.StringBuilder destination instance. If null a new StringBuilder will be returned.</param>
      /// <param name="length">The number of characters in the substring.</param>
      /// <param name="trimOnly">if true trim the existing StringBuilder instance. -or- if false create a new StringBuilder instance and leave the existing instance in tact.</param>
      /// <returns>A System.Text.StringBuilder that is equivalent to the substring of length length that begins at startIndex in this instance.</returns>
      /// <exception cref="System.ArgumentOutOfRangeException">startIndex plus length indicates a position not within this instance. -or- startIndex or length is less than zero.</exception>
      private static StringBuilder SubstringBuilder(this StringBuilder sb, int startIndex, StringBuilder dest, int length, bool trimOnly)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         if (trimOnly)
         {
            sb.Remove(0, startIndex);
            sb.Remove(length, sb.Length - length);
            return sb;
         }

         var result = dest ?? new StringBuilder();
         sb.CopyTo(startIndex, result, length);
         return result;
      }

      public static void CopyTo(this StringBuilder sb, StringBuilder dest)
      {
         CopyTo(sb, 0, dest, sb.Length);
      }

      private static void CopyTo(this StringBuilder sb, int startIndex, StringBuilder dest, int length)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (startIndex < 0) throw new ArgumentOutOfRangeException("startIndex");
         if (dest == null) throw new ArgumentNullException("dest");
         if (length < 0) throw new ArgumentOutOfRangeException("length");
         if (startIndex + length > sb.Length) throw new ArgumentOutOfRangeException("length");

         dest.Clear();
         dest.EnsureCapacity(length);
         for (int i = startIndex; i < (startIndex + length); i++)
         {
            dest.Append(sb[i]);
         }
      }

      private const int MaxChunkSize = 8000;

      public static IEnumerable<char[]> GetChunks(this StringBuilder sb)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         return GetChunks(sb, MaxChunkSize);
      }

      private static IEnumerable<char[]> GetChunks(this StringBuilder sb, int chunkSize)
      {
         if (sb == null) throw new ArgumentNullException("sb");
         if (chunkSize < 1000) throw new ArgumentOutOfRangeException("chunkSize");

         var list = new List<char[]>();
         for (int i = 0; i < sb.Length; i += chunkSize)
         {
            int length = i + chunkSize < sb.Length ? chunkSize : sb.Length - i;
            var temp = new char[length];
            sb.CopyTo(i, temp, 0, temp.Length);
            list.Add(temp);
         }
         return list.AsReadOnly();
      }

      public static StringBuilder MergeChunks(this IEnumerable<char[]> chunks)
      {
         if (chunks == null) throw new ArgumentNullException("chunks");

         var sb = new StringBuilder(chunks.Count() * MaxChunkSize);
         foreach (var chunk in chunks)
         {
            sb.Append(chunk);
         }
         return sb;
      }

      public static IEnumerable<string> Split(this StringBuilder sb, char splitChar)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         var list = new LinkedList<string>();

         int lastIndex = 0;
         for (int i = 0; i < sb.Length; i++)
         {
            if (sb[i] == splitChar)
            {
               var buffer = new char[i - lastIndex];
               sb.CopyTo(lastIndex, buffer, 0, buffer.Length);
               lastIndex = i + 1;

               list.AddLast(new string(buffer));
            }
         }

         return list;
      }

      public static bool EndsWith(this StringBuilder sb, char value)
      {
         if (sb == null) throw new ArgumentNullException("sb");

         if (sb.Length > 0)
         {
            return sb[sb.Length - 1].Equals(value);
         }

         return false;
      }

      //public static bool EndsWith(this StringBuilder sb, string value)
      //{
      //   if (sb == null) throw new ArgumentNullException("sb");
      //   if (value == null) throw new ArgumentNullException("value");
      //
      //   if (value.Length <= sb.Length)
      //   {
      //      var temp = new char[value.Length];
      //      sb.CopyTo(sb.Length - value.Length, temp, 0, value.Length);
      //      return new string(temp).Equals(value);
      //   }
      //
      //   return false;
      //}

      #endregion
   }
}
