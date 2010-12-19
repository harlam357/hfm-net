/*
 * HFM.NET - Duplicate Finder Class
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

using System.Linq;
using System.Collections.Generic;

using HFM.Framework;

namespace HFM.Instances
{
   internal static class DuplicateFinder
   {
      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public static void FindDuplicates(IEnumerable<IDisplayInstance> displayCollection) // Issue 19
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
                           group x by x.CurrentUnitInfo.ProjectRunCloneGen into g
                           let count = g.Count()
                           where count > 1 && g.First().CurrentUnitInfo.UnitInfoData.ProjectIsUnknown == false
                           select g.Key);

         foreach (IDisplayInstance instance in instances)
         {
            instance.ProjectIsDuplicate = duplicates.Contains(instance.CurrentUnitInfo.ProjectRunCloneGen);
         }
      }
   }
}
