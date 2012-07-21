/*
 * HFM.NET - Core Extension Methods
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
using System.Drawing;
using System.Globalization;
using System.Linq;

using HFM.Core.DataTypes;

namespace HFM.Core
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

      public static string ToDateString(this DateTime date)
      {
         return ToDateString(date, String.Format(CultureInfo.CurrentCulture,
                  "{0} {1}", date.ToShortDateString(), date.ToShortTimeString()));
      }

      public static string ToDateString(this IEquatable<DateTime> date, string formattedValue)
      {
         return date.Equals(DateTime.MinValue) ? "Unknown" : formattedValue;
      }

      /// <summary>
      /// Get time delta between the TimeSpan arguments.
      /// </summary>
      /// <param name="time1">First TimeSpan value.</param>
      /// <param name="time2">Second TimeSpan value.</param>
      internal static TimeSpan GetDelta(this TimeSpan time1, TimeSpan time2)
      {
         TimeSpan delta;

         // check for rollover back to 00:00:00 time1 will be less than previous time2 reading
         if (time1 < time2)
         {
            // get time before rollover
            delta = TimeSpan.FromDays(1).Subtract(time2);
            // add time from latest reading
            delta = delta.Add(time1);
         }
         else
         {
            delta = time1.Subtract(time2);
         }

         return delta;
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
            case SlotStatus.RunningNoFrameTimes:
            case SlotStatus.Paused:
            case SlotStatus.Finishing:
            case SlotStatus.Offline:
               return ColorTranslator.ToHtml(Color.Black);
            default:
               return ColorTranslator.ToHtml(Color.White);
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
            case SlotStatus.Finishing: // v7 specific
               return Color.Khaki;
            case SlotStatus.Ready:     // v7 specific
               return Color.DarkCyan;
            case SlotStatus.Stopping:  // v7 specific
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

      #region SlotName

      internal static string AppendSlotId(this string name, int slotId)
      {
         return slotId >= 0 ? String.Format(CultureInfo.InvariantCulture, "{0} Slot {1:00}", name, slotId) : name;
      }

      #endregion

      #region ClientSettings

      public static bool IsFahClient(this ClientSettings settings)
      {
         return settings == null ? false : settings.ClientType.Equals(ClientType.FahClient);
      }

      public static bool IsLegacy(this ClientSettings settings)
      {
         return settings == null ? false : settings.ClientType.Equals(ClientType.Legacy);
      }

      /// <summary>
      /// Used to supply a "path" value to the benchmarks and unit info database.
      /// </summary>
      public static string DataPath(this ClientSettings settings)
      {
         if (settings == null) return String.Empty;

         if (settings.IsFahClient())
         {
            return String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Server, settings.Port);
         }
         return settings.Path;
      }

      public static string CachedFahLogFileName(this ClientSettings settings)
      {
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, settings.IsLegacy() ? Constants.FahLogFileName : Constants.FahClientLogFileName);
      }

      public static string CachedUnitInfoFileName(this ClientSettings settings)
      {
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Constants.UnitInfoFileName);
      }

      public static string CachedQueueFileName(this ClientSettings settings)
      {
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Constants.QueueFileName);
      }

      #endregion

      #region SlotTotals

      /// <summary>
      /// Get the totals for all slots.
      /// </summary>
      /// <returns>The totals for all slots.</returns>
      public static SlotTotals GetSlotTotals(this IEnumerable<SlotModel> slots)
      {
         var totals = new SlotTotals();

         // If no slots return initialized totals.
         if (slots == null)
         {
            return totals;
         }

         totals.TotalSlots = slots.Count();

         foreach (SlotModel slot in slots)
         {
            totals.PPD += slot.PPD;
            totals.UPD += slot.UPD;
            totals.TotalRunCompletedUnits += slot.TotalRunCompletedUnits;
            totals.TotalRunFailedUnits += slot.TotalRunFailedUnits;
            totals.TotalCompletedUnits += slot.TotalCompletedUnits;

            if (slot.ProductionValuesOk)
            {
               totals.WorkingSlots++;
            }
         }

         return totals;
      }

      #endregion

      #region FindDuplicates

      /// <summary>
      /// Find Clients with Duplicate User IDs or Project (R/C/G).
      /// </summary>
      public static void FindDuplicates(this IEnumerable<SlotModel> slots)
      {
         // Issue 19
         FindDuplicateUserId(slots);
         FindDuplicateProjects(slots);
      }

      private static void FindDuplicateUserId(IEnumerable<SlotModel> slots)
      {
         var duplicates = (from x in slots
                           group x by x.UserAndMachineId into g
                           let count = g.Count()
                           where count > 1 && g.First().UserIdUnknown == false
                           select g.Key);

         foreach (SlotModel slot in slots)
         {
            slot.UserIdIsDuplicate = duplicates.Contains(slot.UserAndMachineId);
         }
      }

      private static void FindDuplicateProjects(IEnumerable<SlotModel> slots)
      {
         var duplicates = (from x in slots
                           group x by x.UnitInfoLogic.UnitInfoData.ProjectRunCloneGen() into g
                           let count = g.Count()
                           where count > 1 && g.First().UnitInfoLogic.UnitInfoData.ProjectIsUnknown() == false
                           select g.Key);

         foreach (SlotModel slot in slots)
         {
            slot.ProjectIsDuplicate = duplicates.Contains(slot.UnitInfoLogic.UnitInfoData.ProjectRunCloneGen());
         }
      }

      #endregion

      #region SlotType

      public static SlotType ToSlotType(this string value)
      {
         SlotType type = ToSlotTypeFromCoreName(value);
         if (type.Equals(SlotType.Unknown))
         {
            type = ToSlotTypeFromCoreId(value);
         }
         return type;
      }

      /// <summary>
      /// Determine the Client Type based on the FAH Core Name
      /// </summary>
      /// <param name="coreName">FAH Core Name (from psummary)</param>
      private static SlotType ToSlotTypeFromCoreName(string coreName)
      {
         // make this method more forgiving - rwh 9/6/10
         if (String.IsNullOrEmpty(coreName))
         {
            return SlotType.Unknown;
         }

         switch (coreName.ToUpperInvariant())
         {
            case "GROMACS":
            case "DGROMACS":
            case "GBGROMACS":
            case "AMBER":
            case "GROMACS33":
            case "GROST":
            case "GROSIMT":
            case "DGROMACSB":
            case "DGROMACSC":
            case "GRO-A4":
            case "PROTOMOL":
               return SlotType.Uniprocessor;
            case "GRO-SMP":
            case "GROCVS":
            case "GRO-A3":
            case "GRO-A5":
            case "GRO-A6":
               return SlotType.SMP;
            case "GROGPU2":
            case "GROGPU2-MT":
            case "OPENMMGPU":
            case "OPENMM_OPENCL":
            case "ATI-DEV":
            case "NVIDIA-DEV":
               return SlotType.GPU;
            default:
               return SlotType.Unknown;
         }
      }

      /// <summary>
      /// Determine the Client Type based on the FAH Core ID
      /// </summary>
      /// <param name="coreId">FAH Core ID</param>
      private static SlotType ToSlotTypeFromCoreId(string coreId)
      {
         // make this method more forgiving - rwh 9/6/10
         if (String.IsNullOrEmpty(coreId))
         {
            return SlotType.Unknown;
         }

         switch (coreId.ToUpperInvariant())
         {
            case "78": // Gromacs
            case "79": // Double Gromacs
            case "7A": // GB Gromacs
            case "7B": // Double Gromacs B
            case "7C": // Double Gromacs C
            case "80": // Gromacs SREM
            case "81": // Gromacs SIMT
            case "82": // Amber
            case "A0": // Gromacs 33
            case "B4": // ProtoMol
               return SlotType.Uniprocessor;
            case "A1": // Gromacs SMP
            case "A2": // Gromacs SMP
            case "A3": // Gromacs SMP2
            case "A5": // Gromacs SMP2
            case "A6": // Gromacs SMP2
               return SlotType.SMP;
            case "11": // GPU2 - GROGPU2
            case "12": // GPU2 - ATI-DEV
            case "13": // GPU2 - NVIDIA-DEV
            case "14": // GPU2 - GROGPU2-MT
            case "15": // GPU3 - OPENMMGPU - NVIDIA
            case "16": // GPU3 - OPENMMGPU - ATI
               return SlotType.GPU;
            default:
               return SlotType.Unknown;
         }
      }

      #endregion

      #region BonusCalculationType

      public static bool IsEnabled(this BonusCalculationType type)
      {
         return type.Equals(BonusCalculationType.DownloadTime) ||
                type.Equals(BonusCalculationType.FrameTime);
      }

      #endregion

      #region UnitInfo

      internal static bool IsSameUnitAs(this UnitInfo unit1, UnitInfo unit2)
      {
         if (unit1 == null) throw new ArgumentNullException("unit1");

         if (unit2 == null)
         {
            return false;
         }

         // if the Projects are known
         if (!unit1.ProjectIsUnknown() && !unit2.ProjectIsUnknown())
         {
            // ReSharper disable RedundantThisQualifier

            // equals the Project and Download Time
            if (unit1.EqualsProject(unit2) &&
                unit1.DownloadTime.Equals(unit2.DownloadTime))
            {
               return true;
            }

            // ReSharper restore RedundantThisQualifier
         }

         return false;
      }

      #endregion

      #region DeepClone

      public static Protein DeepClone(this Protein value)
      {
         return ProtoBuf.Serializer.DeepClone(value);
      }

      public static QueryParameters DeepClone(this QueryParameters value)
      {
         return ProtoBuf.Serializer.DeepClone(value);
      }

      internal static UnitInfo DeepClone(this UnitInfo value)
      {
         return ProtoBuf.Serializer.DeepClone(value);
      }

      internal static ClientSettings DeepClone(this ClientSettings value)
      {
         return ProtoBuf.Serializer.DeepClone(value);
      }

      #endregion
   }
}
