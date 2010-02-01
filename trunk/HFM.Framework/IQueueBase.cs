/*
 * HFM.NET - Queue Base Interface
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

namespace HFM.Framework
{
   [CLSCompliant(false)]
   public interface IQueueBase
   {
      /// <summary>
      /// Flag Denoting if Class holds a Populated or Empty Queue Structure
      /// </summary>
      bool DataPopulated { get; }
   
      /// <summary>
      /// Create a new Instance
      /// </summary>
      IQueueBase Create();
   
      /// <summary>
      /// Queue (client) version
      /// </summary>
      UInt32 Version { get; }

      /// <summary>
      /// Current index number
      /// </summary>
      UInt32 CurrentIndex { get; }

      /// <summary>
      /// Performance fraction
      /// </summary>
      float PerformanceFraction { get; }

      /// <summary>
      /// Performance fraction unit weight
      /// </summary>
      UInt32 PerformanceFractionUnitWeight { get; }

      /// <summary>
      /// Download rate sliding average
      /// </summary>
      float DownloadRateAverage { get; }

      /// <summary>
      /// Download rate unit weight
      /// </summary>
      UInt32 DownloadRateUnitWeight { get; }

      /// <summary>
      /// Upload rate sliding average
      /// </summary>
      float UploadRateAverage { get; }

      /// <summary>
      /// Upload rate unit weight
      /// </summary>
      UInt32 UploadRateUnitWeight { get; }

      /// <summary>
      /// Results successfully sent (after upload failures)
      /// </summary>
      UInt32 ResultsSent { get; }
      
      /// <summary>
      /// Get the Current QueueEntry.
      /// </summary>
      IQueueEntry CurrentQueueEntry { get; }

      /// <summary>
      /// Get the QueueEntry at the specified Index.
      /// </summary>
      /// <param name="Index">Queue Entry Index</param>
      IQueueEntry GetQueueEntry(uint Index);

      /// <summary>
      /// Collection used to populate UI Controls
      /// </summary>
      ICollection<string> EntryNameCollection { get; }
   }
}