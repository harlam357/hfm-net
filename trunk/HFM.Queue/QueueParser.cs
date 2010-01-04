/*
 * HFM.NET - Queue Parser Class
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
using System.Collections.Generic;

using HFM.Framework;

namespace HFM.Queue
{
   [CLSCompliant(false)]
   public static class QueueParser
   {
      /// <summary>
      /// Parse Queue Information Info UnitInfo
      /// </summary>
      /// <param name="entry">QueueEntry to Parse</param>
      /// <param name="parsedUnitInfo">UnitInfo to Populate</param>
      /// <param name="ClientIsOnVirtualMachine">Client on VM (Times as UTC) Flag</param>
      public static void ParseQueueEntry(QueueEntry entry, IUnitInfo parsedUnitInfo, bool ClientIsOnVirtualMachine)
      {
         if ((entry.EntryStatus.Equals(QueueEntryStatus.Unknown) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Empty) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Garbage) ||
              entry.EntryStatus.Equals(QueueEntryStatus.Abandonded)) == false)
         {
            /* Tag (Could be read here or through the unitinfo.txt file) */
            parsedUnitInfo.ProteinTag = entry.WorkUnitTag;

            /* DownloadTime (Could be read here or through the unitinfo.txt file) */
            if (ClientIsOnVirtualMachine)
            {
               parsedUnitInfo.DownloadTime = entry.BeginTimeUtc;
            }
            else
            {
               parsedUnitInfo.DownloadTime = entry.BeginTimeLocal;
            }

            /* DueTime (Could be read here or through the unitinfo.txt file) */
            if (ClientIsOnVirtualMachine)
            {
               parsedUnitInfo.DueTime = entry.DueTimeUtc;
            }
            else
            {
               parsedUnitInfo.DueTime = entry.DueTimeLocal;
            }

            /* FinishedTime */
            if (entry.EntryStatus.Equals(QueueEntryStatus.Finished))
            {
               if (ClientIsOnVirtualMachine)
               {
                  parsedUnitInfo.FinishedTime = entry.EndTimeUtc;
               }
               else
               {
                  parsedUnitInfo.FinishedTime = entry.EndTimeLocal;
               }
            }

            /* FoldingID and Team from Queue Entry */
            parsedUnitInfo.FoldingID = entry.FoldingID;
            parsedUnitInfo.Team = (int)entry.TeamNumber;

            /* Project (R/C/G) Match */
            List<int> ProjectRCG = new List<int>(4);

            ProjectRCG.Add(entry.ProjectID);
            ProjectRCG.Add(entry.ProjectRun);
            ProjectRCG.Add(entry.ProjectClone);
            ProjectRCG.Add(entry.ProjectGen);

            parsedUnitInfo.DoProjectIDMatch(ProjectRCG);
         }
      }
   }
}
