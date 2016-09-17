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

using System;
using System.Diagnostics.CodeAnalysis;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")]
   internal struct LegacyClientStatusData
   {
      internal string ClientName { get; set; }

      internal SlotType SlotType { get; set; }

      internal DateTime UnitRetrievalTime { get; set; }

      internal bool UtcOffsetIsZero { get; set; }

      internal TimeSpan UtcOffset { get; set; }

      internal int ClientTimeOffset { get; set; }

      internal DateTime TimeOfLastUnitStart { get; set; }

      internal DateTime TimeOfLastFrameProgress { get; set; }

      internal SlotStatus CurrentStatus { get; set; }

      internal SlotStatus ReturnedStatus { get; set; }

      internal int FrameTime { get; set; }

      internal TimeSpan BenchmarkAverageFrameTime { get; set; }

      internal TimeSpan TimeOfLastFrame { get; set; }

      internal TimeSpan UnitStartTimeStamp { get; set; }

      internal bool AllowRunningAsync { get; set; }
   }
}
