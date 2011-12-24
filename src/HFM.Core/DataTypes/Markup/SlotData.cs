/*
 * HFM.NET - Markup Data Class
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

using System.Runtime.Serialization;

namespace HFM.Core.DataTypes.Markup
{
   [DataContract(Namespace = "")]
   public class SlotData
   {
      [DataMember(Order = 1)]
      public string Arguments { get; set; }

      [DataMember(Order = 2)]
      public string ClientVersion { get; set; }

      [DataMember(Order = 3)]
      public string UserId { get; set; }

      [DataMember(Order = 4)]
      public int MachineId { get; set; }

      [DataMember(Order = 5)]
      public int TotalRunCompletedUnits { get; set; }

      [DataMember(Order = 6)]
      public int TotalCompletedUnits { get; set; }

      [DataMember(Order = 7)]
      public int TotalRunFailedUnits { get; set; }

      [DataMember(Order = 8)]
      public GridData GridData { get; set; }

      [DataMember(Order = 9)]
      public UnitInfo UnitInfo { get; set; }
   }
}