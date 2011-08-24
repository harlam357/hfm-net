/*
 * HFM.NET - Client Data Type Enumerations
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

namespace HFM.Client.DataTypes
{
   // ReSharper disable InconsistentNaming

   public enum OperatingSystemType
   {
      Unknown,
      Windows,
      WindowsXP,
      WindowsXPx64,
      Vista32,
      Vista64,
      Windows7,
      //Windows7x32,
      //Windows7x64,
      Linux,
      OSX

      // Expand Linux and OSX members if necessary
   }

   // ReSharper restore InconsistentNaming

   public enum ClientType
   {
      Unknown,
      Normal,
      Advanced,
      BigAdv,
      Beta
   }

   // ReSharper disable InconsistentNaming

   public enum ClientSubType
   {
      Unknown = 0,
      Normal = 1,    // ???
      StdCli = 1,    // appears to be uniprocessor (Normal) on Windows
      Linux = 1,     // appears to be uniprocessor (Normal) on Linux
      SMP = 2,
      GPU = 3,
   }

   // ReSharper restore InconsistentNaming

   public enum MaxPacketSize
   {
      Unknown,
      Small,
      Normal,
      Big
   }

   public enum CorePriority
   {
      Unknown,
      Idle,
      Low
   }

   public enum SlotStatus
   {
      Unknown,
      Paused,
      Running,
      Finishing,
      Send
   }
}
