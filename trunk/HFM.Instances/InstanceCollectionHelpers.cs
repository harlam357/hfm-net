/*
 * HFM.NET - Instance Collection Helper Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

using HFM.Preferences;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public static class InstanceCollectionHelpers
   {
      /// <summary>
      /// Get Totals for all Client Instances in given Collection
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      public static InstanceTotals GetInstanceTotals(ICollection<ClientInstance> Instances)
      {
         InstanceTotals totals = new InstanceTotals();
         
         // If no Instance Collection, return initialized totals.
         // Added this check because this function is now being passed a copy of the client 
         // instances references using GetCurrentInstanceArray() and not the collection 
         // directly, since the "live" collection can change at any time.
         if (Instances == null)
         {
            return totals;
         }

         totals.TotalClients = Instances.Count;

         foreach (ClientInstance Instance in Instances)
         {
            totals.PPD += Instance.PPD;
            totals.UPD += Instance.UPD;
            totals.TotalCompletedUnits += Instance.NumberOfCompletedUnitsSinceLastStart;
            totals.TotalFailedUnits += Instance.NumberOfFailedUnitsSinceLastStart;

            if (Instance.ProductionValuesOk)
            {
               totals.WorkingClients++;
            }
         }

         totals.NonWorkingClients = totals.TotalClients - totals.WorkingClients;

         return totals;
      }

      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public static void FindDuplicates(List<string> DuplicateUserID, List<string> DuplicateProjects, ICollection<ClientInstance> Instances)
      {
         // If no instances stub out
         if (Instances == null) return;

         DateTime Start = HfmTrace.ExecStart;

         DuplicateUserID.Clear();
         DuplicateProjects.Clear();

         PreferenceSet Prefs = PreferenceSet.Instance;

         // If neither check is selected, just get out
         if (Prefs.DuplicateProjectCheck == false &&
             Prefs.DuplicateUserIDCheck == false) return;

         Hashtable userHash = new Hashtable(Instances.Count);
         Hashtable projectHash = new Hashtable(Instances.Count);

         foreach (ClientInstance Instance in Instances)
         {
            if (Prefs.DuplicateProjectCheck)
            {
               string PRCG = Instance.CurrentUnitInfo.ProjectRunCloneGen;
               if (projectHash.Contains(PRCG))
               {
                  DuplicateProjects.Add(PRCG);
               }
               else
               {
                  // don't add an unknown project
                  if (Instance.CurrentUnitInfo.ProjectIsUnknown == false)
                  {
                     projectHash.Add(Instance.CurrentUnitInfo.ProjectRunCloneGen, null);
                  }
               }
            }

            if (Prefs.DuplicateUserIDCheck)
            {
               string UserAndMachineID = Instance.UserAndMachineID;
               if (userHash.Contains(UserAndMachineID))
               {
                  DuplicateUserID.Add(UserAndMachineID);
               }
               else
               {
                  // don't add an unknown User ID
                  if (Instance.UserIDUnknown == false)
                  {
                     userHash.Add(UserAndMachineID, null);
                  }
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
   }
}
