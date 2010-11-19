/*
 * HFM.NET - Benchmark Data Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Xml.Serialization;

using ProtoBuf;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   [ProtoContract]
   public sealed class InstanceProteinBenchmark : IInstanceProteinBenchmark
   {
      private const int DefaultMaxFrames = 300;

      private static readonly object FrameTimesListLock = new object();
   
      #region Fields
      
      #region Owner Data Properties

      /// <summary>
      /// Name of the Client Instance that owns this Benchmark
      /// </summary>
      [ProtoMember(1)]
      public string OwningInstanceName { get; set; }

      /// <summary>
      /// Path of the Client Instance that owns this Benchmark
      /// </summary>
      [ProtoMember(2)]
      public string OwningInstancePath { get; set; }

      #endregion

      /// <summary>
      /// Project ID
      /// </summary>
      [ProtoMember(3)]
      public int ProjectID { get; set; }

      /// <summary>
      /// Minimum Frame Time
      /// </summary>
      [ProtoMember(4)]
      [XmlIgnore]
      public TimeSpan MinimumFrameTime { get; set; }

      /// <summary>
      /// Minimum Frame Time (Ticks)
      /// </summary>
      [XmlElement("MinimumFrameTime")]
      [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
      public long MinimumFrameTimeTicks
      {
         get { return MinimumFrameTime.Ticks; }
         set { MinimumFrameTime = new TimeSpan(value); }
      }

      /// <summary>
      /// Frame Times List
      /// </summary>
      [ProtoMember(5)]
      public List<ProteinFrameTime> FrameTimes { get; set; }

      #endregion

      #region Properties
      
      /// <summary>
      /// PPD based on Minimum Frame Time
      /// </summary>
      public double MinimumFrameTimePPD
      {
         get 
         { 
            if (Protein != null)
            {
               // Issue 125 & 129
               if (InstanceProvider.GetInstance<IPreferenceSet>().GetPreference<bool>(Preference.CalculateBonus))
               {
                  TimeSpan finishTime = TimeSpan.FromMilliseconds(MinimumFrameTime.TotalMilliseconds * Protein.Frames);
                  return Protein.GetPPD(MinimumFrameTime, finishTime);
               }

               return Protein.GetPPD(MinimumFrameTime);
            }
            
            return 0;
         }
      }

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
                  Debug.WriteLine("In AverageFrameTime property");
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
      /// PPD based on Average Frame Time
      /// </summary>
      public double AverageFrameTimePPD
      {
         get
         {
            if (Protein != null)
            {
               // Issue 125 & 129
               if (InstanceProvider.GetInstance<IPreferenceSet>().GetPreference<bool>(Preference.CalculateBonus))
               {
                  TimeSpan finishTime = TimeSpan.FromMilliseconds(AverageFrameTime.TotalMilliseconds * Protein.Frames);
                  return Protein.GetPPD(AverageFrameTime, finishTime);
               }

               return Protein.GetPPD(AverageFrameTime);
            }

            return 0;
         }
      }
      #endregion

      #region Constructor
      // ReSharper disable UnusedMember.Local
      /// <summary>
      /// Used by protobuf-net serializer
      /// </summary>
      private InstanceProteinBenchmark()
      {
         MinimumFrameTime = TimeSpan.Zero;
         FrameTimes = new List<ProteinFrameTime>(DefaultMaxFrames);
      }
      // ReSharper restore UnusedMember.Local

      public InstanceProteinBenchmark(string ownerName, string ownerPath, int projectID)
      {
         OwningInstanceName = ownerName;
         OwningInstancePath = ownerPath;
         ProjectID = projectID;
         MinimumFrameTime = TimeSpan.Zero;
         FrameTimes = new List<ProteinFrameTime>(DefaultMaxFrames);
      } 
      #endregion

      /// <summary>
      /// Benchmark Client Descriptor
      /// </summary>
      public BenchmarkClient Client
      {
         get { return new BenchmarkClient(OwningInstanceName, OwningInstancePath); }
      }

      /// <summary>
      /// Benchmark Protein
      /// </summary>
      public IProtein Protein
      {
         get
         {
            IProtein protein;
            InstanceProvider.GetInstance<IProteinCollection>().TryGetValue(ProjectID, out protein);

            return protein;
         }
      }

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
               Debug.WriteLine("In SetFrameTime() method");
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
      public void RefreshBenchmarkMinimumFrameTime()
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
   }
}
