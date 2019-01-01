/*
 * HFM.NET - Unit Frame Class
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Globalization;

namespace HFM.Log
{
   /// <summary>
   /// Represents work unit frame data gathered from a <see cref="LogLine"/> object.
   /// </summary>
   public class WorkUnitFrameData
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="WorkUnitFrameData"/> class.
      /// </summary>
      public WorkUnitFrameData()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="WorkUnitFrameData"/> class.
      /// </summary>
      /// <param name="other">The other instance from which data will be copied.</param>
      public WorkUnitFrameData(WorkUnitFrameData other)
      {
         if (other == null) return;

         ID = other.ID;
         RawFramesComplete = other.RawFramesComplete;
         RawFramesTotal = other.RawFramesTotal;
         TimeStamp = other.TimeStamp;
         Duration = other.Duration;
      }

      /// <summary>
      /// Gets or sets the frame ID.
      /// </summary>
      public int ID { get; set; }

      /// <summary>
      /// Gets or sets the raw (expanded) number of frames complete.
      /// </summary>
      public int RawFramesComplete { get; set; }

      /// <summary>
      /// Gets or sets the raw (expanded) number of frames complete.
      /// </summary>
      public int RawFramesTotal { get; set; }

      /// <summary>
      /// Gets or sets the time stamp of the frame.
      /// </summary>
      public TimeSpan TimeStamp { get; set; }

      /// <summary>
      /// Gets or sets the duration of the frame.
      /// </summary>
      public TimeSpan Duration { get; set; }

      /// <summary>
      /// Returns a string that represents the current <see cref="WorkUnitFrameData"/> object.
      /// </summary>
      /// <returns>A string that represents the current <see cref="WorkUnitFrameData"/> object.</returns>
      public override string ToString()
      {
         return String.Format(CultureInfo.CurrentCulture, 
            "ID: {0}, Raw Complete: {1}, Raw Total: {2}, Time Stamp: {3}, Duration: {4}",
            ID, RawFramesComplete, RawFramesTotal, TimeStamp, Duration);
      }
   }
}
