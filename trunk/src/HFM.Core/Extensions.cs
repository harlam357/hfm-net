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
using System.Globalization;
using System.Linq;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public static class Extensions
   {
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
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, settings.IsLegacy() ? Default.FahLogFileName : Default.FahClientLogFileName);
      }

      public static string CachedUnitInfoFileName(this ClientSettings settings)
      {
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Default.UnitInfoFileName);
      }

      public static string CachedQueueFileName(this ClientSettings settings)
      {
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Default.QueueFileName);
      }

      #endregion

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
      /// Get the totals for all slots.
      /// </summary>
      /// <returns>The totals for all slots.</returns>
      public static SlotTotals GetSlotTotals(this IEnumerable<SlotModel> slots)
      {
         var totals = new SlotTotals();

         // If no Instance Collection, return initialized totals.
         // Added this check because this function is now being passed a copy of the client 
         // slots references using GetCurrentInstanceArray() and not the collection 
         // directly, since the "live" collection can change at any time.
         // 4/17/10 - GetCurrentInstanceArray() no longer returns null when there are no clients
         //           it returns an empty collection.  However, leaving this check for now.
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

      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public static void FindDuplicates(this IEnumerable<SlotModel> slots) // Issue 19
      {
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
               return SlotType.SMP;
            case "GROGPU2":
            case "GROGPU2-MT":
            case "OPENMMGPU":
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

      #region Protein

      public static Protein DeepClone(this Protein protein)
      {
         return ProtoBuf.Serializer.DeepClone(protein);
      }

      #endregion
   }
}
