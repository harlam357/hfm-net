/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

namespace HFM.Core
{
   // ReSharper disable InconsistentNaming

   /// <summary>
   /// Folding Slot Type
   /// </summary>
   public enum SlotType
   {
      Unknown = 0,
      CPU = 1,
      GPU = 2
   }

   // ReSharper restore InconsistentNaming

   public enum MinimizeToOption
   {
      SystemTray,
      TaskBar,
      Both
   }

   public enum StatsType
   {
      User,
      Team
   }

   public enum TimeFormatting
   {
      None,
      Format1
   }

   public enum UnitTotalsType
   {
      All,
      ClientStart
   }

   public enum GraphLayoutType
   {
      Single,
      ClientsPerGraph
   }
}
