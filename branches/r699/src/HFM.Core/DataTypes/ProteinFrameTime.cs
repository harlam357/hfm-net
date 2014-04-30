/*
 * HFM.NET - Protein Frame Time Class
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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract]
   public sealed class ProteinFrameTime
   {
      public TimeSpan Duration { get; set; }

      [DataMember(Order = 1)]
      [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
      public long DurationTicks
      {
         get { return Duration.Ticks; }
         set { Duration = new TimeSpan(value); }
      }
   }
}
