/*
 * HFM.NET - Client Run Interface
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

namespace HFM.Framework
{
   public interface IClientRun
   {
      string Arguments { get; set; }

      string FoldingID { get; set; }

      int Team { get; set; }

      string UserID { get; set; }

      int MachineID { get; set; }

      int NumberOfCompletedUnits { get; set; }

      int NumberOfFailedUnits { get; set; }

      int NumberOfTotalUnitsCompleted { get; set; }

      /// <summary>
      /// Line index of client start position.
      /// </summary>
      int ClientStartIndex { get; }

      /// <summary>
      /// List of work unit start positions for this client run.
      /// </summary>
      List<int> UnitStartIndex { get; }

      /// <summary>
      /// List of Queue Indexes that correspond to the Unit Start Indexes for this client run.
      /// </summary>
      List<int> UnitQueueIndex { get; }
   }
}