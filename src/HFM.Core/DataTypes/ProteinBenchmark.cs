/*
 * HFM.NET - Protein Benchmark Data Class
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [Serializable]
   [DataContract]
   public sealed class ProteinBenchmark : IOwnedByClientSlot, IEquatable<IOwnedByClientSlot>
   {
      private const int DefaultMaxFrames = 300;

      private static readonly object FrameTimesListLock = new object();
   
      #region Properties
      
      #region Owner Data Properties

      /// <summary>
      /// Name of the Client Instance that owns this Benchmark
      /// </summary>
      [DataMember(Order = 1)]
      public string OwningInstanceName { get; set; }

      /// <summary>
      /// Path of the Client Instance that owns this Benchmark
      /// </summary>
      [DataMember(Order = 2)]
      public string OwningInstancePath { get; set; }

      #endregion

      /// <summary>
      /// Project ID
      /// </summary>
      [DataMember(Order = 3)]
      public int ProjectID { get; set; }

      /// <summary>
      /// Minimum Frame Time
      /// </summary>
      [DataMember(Order = 4)]
      public TimeSpan MinimumFrameTime { get; set; }

      /// <summary>
      /// Average Frame Time
      /// </summary>
      public TimeSpan AverageFrameTime
      {
         get
         {
            if (FrameTimes.Count > 0)
            {
               TimeSpan totalTime = TimeSpan.Zero;
               lock (FrameTimesListLock)
               {
                  foreach (ProteinFrameTime time in FrameTimes)
                  {
                     totalTime = totalTime.Add(time.Duration);
                  }
               }

               return TimeSpan.FromSeconds((Convert.ToInt32(totalTime.TotalSeconds) / FrameTimes.Count));
            }

            return TimeSpan.Zero;
         }
      }

      /// <summary>
      /// Frame Times List
      /// </summary>
      [DataMember(Order = 5)]
      public List<ProteinFrameTime> FrameTimes { get; set; }

      /// <summary>
      /// Benchmark Client Descriptor
      /// </summary>
      public BenchmarkClient Client
      {
         get { return new BenchmarkClient(OwningInstanceName, OwningInstancePath); }
      }
      
      #endregion

      #region Constructor
      
      /// <summary>
      /// Default Constructor
      /// </summary>
      public ProteinBenchmark()
      {
         MinimumFrameTime = TimeSpan.Zero;
         FrameTimes = new List<ProteinFrameTime>(DefaultMaxFrames);
      }
      
      #endregion
      
      #region Implementation
      
      /// <summary>
      /// Set Next Frame Time
      /// </summary>
      /// <param name="frameTime">Frame Time</param>
      public bool SetFrameTime(TimeSpan frameTime)
      {
         if (frameTime > TimeSpan.Zero)
         {
            if (frameTime < MinimumFrameTime || MinimumFrameTime.Equals(TimeSpan.Zero))
            {
               MinimumFrameTime = frameTime;
            }

            lock (FrameTimesListLock)
            {
               // Dequeue once we have the Maximum number of frame times
               if (FrameTimes.Count == DefaultMaxFrames)
               {
                  FrameTimes.RemoveAt(DefaultMaxFrames - 1);
               }
               FrameTimes.Insert(0, new ProteinFrameTime { Duration = frameTime });
            }

            return true;
         }
         
         return false;
      }

      /// <summary>
      /// Refresh the Minimum Frame Time for this Benchmark based on current List of Frame Times
      /// </summary>
      public void UpdateMinimumFrameTime()
      {
         TimeSpan minimumFrameTime = TimeSpan.Zero;
         lock (FrameTimesListLock)
         {
            foreach (ProteinFrameTime frameTime in FrameTimes)
            {
               if (frameTime.Duration < minimumFrameTime || minimumFrameTime.Equals(TimeSpan.Zero))
               {
                  minimumFrameTime = frameTime.Duration;
               }
            }
         }

         if (minimumFrameTime.Equals(TimeSpan.Zero) == false)
         {
            MinimumFrameTime = minimumFrameTime;
         }
      }
      
      #endregion

      #region IEquatable<ProteinBenchmark> Members

      public bool Equals(IOwnedByClientSlot other)
      {
         return OwningInstanceName == other.OwningInstanceName &&
                Paths.Equal(OwningInstancePath, other.OwningInstancePath) &&
                ProjectID == other.ProjectID;
      }

      #endregion
   }
}
