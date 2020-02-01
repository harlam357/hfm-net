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

namespace HFM.Client.DataTypes
{
#pragma warning disable 1591

   /// <summary>
   /// Folding@Home client type.
   /// </summary>
   public enum FahClientType
   {
      Unknown,
      Normal,
      Advanced,
      BigAdv,
      Beta,
      BigBeta
   }

   // ReSharper disable InconsistentNaming

   /// <summary>
   /// Folding@Home sub-client type (CPU or GPU).
   /// </summary>
   public enum FahClientSubType
   {
      Unknown = 0,
      CPU = 1,
      GPU = 2,
   }

   // ReSharper restore InconsistentNaming

   // Matches HFM.Core.SlotStatus

   /// <summary>
   /// Represents the status of a Folding@Home client slot.
   /// </summary>
   public enum FahClientSlotStatus
   {
      /// <summary>
      /// The status of the slot is unknown.
      /// </summary>
      Unknown = 0,
      /// <summary>
      /// The slot is paused.
      /// </summary>
      Paused = 1,
      /// <summary>
      /// The slot is running.
      /// </summary>
      Running = 2,
      /// <summary>
      /// The slot is finishing.
      /// </summary>
      Finishing = 3,
      /// <summary>
      /// The slot is ready for work.
      /// </summary>
      Ready = 4,
      /// <summary>
      /// The slot is stopping.
      /// </summary>
      Stopping = 5,
      /// <summary>
      /// The slot work has failed.
      /// </summary>
      Failed = 6
   }

   /// <summary>
   /// Folding@Home work unit state.
   /// </summary>
   public enum UnitState
   {
      Unknown,
      Running,
      Download,
      Send,
      Ready
   }

   /// <summary>
   /// Folding@Home maximum packet size.
   /// </summary>
   public enum MaxPacketSize
   {
      Unknown,
      Small,
      Normal,
      Big
   }

   /// <summary>
   /// Folding@Home core priority.
   /// </summary>
   public enum CorePriority
   {
      Unknown,
      Idle,
      Low
   }

#pragma warning restore 1591
}
