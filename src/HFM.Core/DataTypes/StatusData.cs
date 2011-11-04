/*
 * HFM.NET - Status Data Structure
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

using System;
using System.Diagnostics.CodeAnalysis;

namespace HFM.Core.DataTypes
{
   [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
   public struct StatusData
   {
      public string InstanceName { get; set; }

      public SlotType SlotType { get; set; }

      public DateTime LastRetrievalTime { get; set; }

      public bool IgnoreUtcOffset { get; set; }

      public TimeSpan UtcOffset { get; set; }

      public int ClientTimeOffset { get; set; }

      public DateTime TimeOfLastUnitStart { get; set; }

      public DateTime TimeOfLastFrameProgress { get; set; }

      public ClientStatus CurrentStatus { get; set; }

      public ClientStatus ReturnedStatus { get; set; }

      public int FrameTime { get; set; }

      public TimeSpan AverageFrameTime { get; set; }

      public TimeSpan TimeOfLastFrame { get; set; }

      public TimeSpan UnitStartTimeStamp { get; set; }

      public bool AllowRunningAsync { get; set; }
   }
}
