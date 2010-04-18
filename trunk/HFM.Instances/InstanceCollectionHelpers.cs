/*
 * HFM.NET - Instance Collection Helper Class
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

using System.Collections.Generic;

using HFM.Framework;

namespace HFM.Instances
{
   public static class InstanceCollectionHelpers
   {
      /// <summary>
      /// Get Totals for all Client Instances in given Collection
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      public static InstanceTotals GetInstanceTotals(ICollection<IClientInstance> instances)
      {
         InstanceTotals totals = new InstanceTotals();
         
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

         totals.TotalClients = instances.Count;

         foreach (IClientInstance instance in instances)
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

         totals.NonWorkingClients = totals.TotalClients - totals.WorkingClients;

         return totals;
      }
   }
}
