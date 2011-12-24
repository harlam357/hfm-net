/*
 * HFM.NET - Core Extension Methods
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
         return settings == null ? String.Empty : String.Format(CultureInfo.InvariantCulture, "{0}-{1}", settings.Name, Default.FahLogFileName);
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

         foreach (SlotModel instance in slots)
         {
            instance.UserIdIsDuplicate = duplicates.Contains(instance.UserAndMachineId);
         }
      }

      private static void FindDuplicateProjects(IEnumerable<SlotModel> slots)
      {
         var duplicates = (from x in slots
                           group x by x.UnitInfoLogic.UnitInfoData.ProjectRunCloneGen() into g
                           let count = g.Count()
                           where count > 1 && g.First().UnitInfoLogic.UnitInfoData.ProjectIsUnknown() == false
                           select g.Key);

         foreach (SlotModel instance in slots)
         {
            instance.ProjectIsDuplicate = duplicates.Contains(instance.UnitInfoLogic.UnitInfoData.ProjectRunCloneGen());
         }
      }
   }
}
