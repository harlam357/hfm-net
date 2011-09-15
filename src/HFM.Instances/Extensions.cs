/*
 * HFM.NET - Instance Extensions Class
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

using System.Collections.Generic;
using System.Linq;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public static class Extensions
   {
      /// <summary>
      /// Get Totals for all Client Instances in given Collection
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      public static InstanceTotals GetInstanceTotals(this IEnumerable<IDisplayInstance> instances)
      {
         var totals = new InstanceTotals();
         
         // If no Instance Collection, return initialized totals.
         // Added this check because this function is now being passed a copy of the client 
         // instances references using GetCurrentInstanceArray() and not the collection 
         // directly, since the "live" collection can change at any time.
         // 4/17/10 - GetCurrentInstanceArray() no longer returns null when there are no clients
         //           it returns an empty collection.  However, leaving this check for now.
         if (instances == null)
         {
            return totals;
         }

         totals.TotalClients = instances.Count();

         foreach (IDisplayInstance instance in instances)
         {
            totals.PPD += instance.PPD;
            totals.UPD += instance.UPD;
            totals.TotalRunCompletedUnits += instance.TotalRunCompletedUnits;
            totals.TotalRunFailedUnits += instance.TotalRunFailedUnits;
            totals.TotalClientCompletedUnits += instance.TotalClientCompletedUnits;

            if (instance.ProductionValuesOk)
            {
               totals.WorkingClients++;
            }
         }

         return totals;
      }

      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public static void FindDuplicates(this IEnumerable<IDisplayInstance> displayCollection) // Issue 19
      {
         FindDuplicateUserId(displayCollection);
         FindDuplicateProjects(displayCollection);
      }

      private static void FindDuplicateUserId(IEnumerable<IDisplayInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.UserAndMachineId into g
                           let count = g.Count()
                           where count > 1 && g.First().UserIdUnknown == false
                           select g.Key);

         foreach (IDisplayInstance instance in instances)
         {
            instance.UserIdIsDuplicate = duplicates.Contains(instance.UserAndMachineId);
         }
      }

      private static void FindDuplicateProjects(IEnumerable<IDisplayInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.CurrentUnitInfo.UnitInfoData.ProjectRunCloneGen() into g
                           let count = g.Count()
                           where count > 1 && g.First().CurrentUnitInfo.UnitInfoData.ProjectIsUnknown() == false
                           select g.Key);

         foreach (IDisplayInstance instance in instances)
         {
            instance.ProjectIsDuplicate = duplicates.Contains(instance.CurrentUnitInfo.UnitInfoData.ProjectRunCloneGen());
         }
      }

      public static bool HasInstances(this IDictionary<string, ClientInstance> instances)
      {
         return instances.Count != 0;
      }
   }
}
