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
using HFM.Framework.DataTypes;
using ProtoBuf;

using HFM.Framework;

namespace HFM.Instances
{
   [Serializable]
   [ProtoContract]
   public sealed class InstanceProteinBenchmark : IInstanceProteinBenchmark
   {
      private const Int32 DefaultMaxFrames = 300;

      private static readonly object FrameTimesListLock = new object();
   
      #region Members

      /// <summary>
      /// Old .NET Based Frame Time Queue
      /// </summary>
      /// <remarks>Leave until ready to remove support for old Benchmarks Format</remarks>
      private readonly Queue<TimeSpan> _FrameTimes = null;
      
      #region Owner Data Properties
      private string _OwningInstanceName;
      /// <summary>
      /// Name of the Client Instance that owns this Benchmark
      /// </summary>
      [ProtoMember(1)]
      public string OwningInstanceName
      {
         get { return _OwningInstanceName; }
         set { _OwningInstanceName = value; }
      }

      private string _OwningInstancePath;
      /// <summary>
      /// Path of the Client Instance that owns this Benchmark
      /// </summary>
      [ProtoMember(2)]
      public string OwningInstancePath
      {
         get { return _OwningInstancePath; }
         set { _OwningInstancePath = value; }
      }
      #endregion

      private Int32 _ProjectID;
      /// <summary>
      /// Project ID
      /// </summary>
      [ProtoMember(3)]
      public Int32 ProjectID
      {
         get { return _ProjectID; }
         set { _ProjectID = value; }
      }

      private TimeSpan _MinimumFrameTime;
      /// <summary>
      /// Minimum Frame Time
      /// </summary>
      [ProtoMember(4)]
      [XmlIgnore]
      public TimeSpan MinimumFrameTime
      {
         get
         {
            if (_MinimumFrameTime == TimeSpan.MaxValue)
            {
               return TimeSpan.Zero;
            }
            return _MinimumFrameTime;
         }
         set { _MinimumFrameTime = value; }
      }

      [XmlElement("MinimumFrameTime")]
      [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
      /// <summary>
      /// Minimum Frame Time (Ticks)
      /// </summary>
      public long MinimumFrameTimeTicks
      {
         get { return _MinimumFrameTime.Ticks; }
         set { _MinimumFrameTime = new TimeSpan(value); }
      }
      
      [NonSerialized]
      private List<FrameTime> _frameTimesList = new List<FrameTime>();
      /// <summary>
      /// Frame Times List
      /// </summary>
      [ProtoMember(5)]
      public List<FrameTime> FrameTimes
      {
         get { return _frameTimesList; }
         set { _frameTimesList = value; }
      }

      /// <summary>
      /// Backward Compatibility - Convert .NET Serialized Queue to protobuf-net Serialized List
      /// </summary>
      public void ConvertQueueToList()
      {
         _frameTimesList = new List<FrameTime>(_FrameTimes.Count);
         foreach (TimeSpan span in _FrameTimes)
         {
            _frameTimesList.Add(new FrameTime(span));
         }
      }
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
                  foreach (FrameTime time in FrameTimes)
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
      public InstanceProteinBenchmark()
      {
      
      }

      public InstanceProteinBenchmark(string ownerName, string ownerPath, Int32 proteinID)
      {
         _OwningInstanceName = ownerName;
         _OwningInstancePath = ownerPath;
         _ProjectID = proteinID;
         _MinimumFrameTime = TimeSpan.Zero;
         _frameTimesList = new List<FrameTime>(DefaultMaxFrames);
      } 
      #endregion

      /// <summary>
      /// Benchmark Client Descriptor
      /// </summary>
      public BenchmarkClient Client
      {
         get 
         { 
            return new BenchmarkClient(OwningInstanceName, OwningInstancePath);
         }
      }

      /// <summary>
      /// Benchmark Protein
      /// </summary>
      public IProtein Protein
      {
         get
         {
            IProtein protein;
            InstanceProvider.GetInstance<IProteinCollection>().TryGetValue(_ProjectID, out protein);

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
            if (frameTime < _MinimumFrameTime || _MinimumFrameTime.Equals(TimeSpan.Zero))
            {
               _MinimumFrameTime = frameTime;
            }

            lock (FrameTimesListLock)
            {
               Debug.WriteLine("In SetFrameTime() method");
               // Dequeue once we have the Maximum number of frame times
               if (FrameTimes.Count == DefaultMaxFrames)
               {
                  _frameTimesList.RemoveAt(DefaultMaxFrames - 1);
               }
               _frameTimesList.Insert(0, new FrameTime(frameTime));
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
            foreach (FrameTime frameTime in FrameTimes)
            {
               if (frameTime.Duration < minimumFrameTime || minimumFrameTime.Equals(TimeSpan.Zero))
               {
                  minimumFrameTime = frameTime.Duration;
               }
            }
         }

         if (minimumFrameTime.Equals(TimeSpan.Zero) == false)
         {
            _MinimumFrameTime = minimumFrameTime;
         }
      }

      /// <summary>
      /// Return Multi-Line String (Array)
      /// </summary>
      /// <param name="UnitInfo">Client Instance UnitInfo (null for unavailable)</param>
      /// <param name="PpdFormatString">PPD Format String</param>
      /// <param name="ProductionValuesOk">Client Instance Production Values Flag</param>
      public string[] ToMultiLineString(IUnitInfoLogic UnitInfo, string PpdFormatString, bool ProductionValuesOk)
      {
         List<string> output = new List<string>(12);

         IProtein theProtein = Protein;
         if (theProtein != null)
         {
            output.Add(String.Empty);
            output.Add(String.Format(" Name: {0}", OwningInstanceName));
            output.Add(String.Format(" Path: {0}", OwningInstancePath));
            output.Add(String.Format(" Number of Frames Observed: {0}", FrameTimes.Count));
            output.Add(String.Empty);
            output.Add(String.Format(" Min. Time / Frame : {0} - {1:" + PpdFormatString + "} PPD", 
               MinimumFrameTime, MinimumFrameTimePPD));
            output.Add(String.Format(" Avg. Time / Frame : {0} - {1:" + PpdFormatString + "} PPD", 
               AverageFrameTime, AverageFrameTimePPD));

            if (UnitInfo != null && UnitInfo.UnitInfoData.ProjectID.Equals(theProtein.ProjectNumber) &&
                                    ProductionValuesOk)
            {
               output.Add(String.Format(" Cur. Time / Frame : {0} - {1:" + PpdFormatString + "} PPD",
                  UnitInfo.TimePerLastSection, UnitInfo.PPDPerLastSection));
               output.Add(String.Format(" R3F. Time / Frame : {0} - {1:" + PpdFormatString + "} PPD",
                  UnitInfo.TimePerThreeSections, UnitInfo.PPDPerThreeSections));
               output.Add(String.Format(" All  Time / Frame : {0} - {1:" + PpdFormatString + "} PPD",
                  UnitInfo.TimePerAllSections, UnitInfo.PPDPerAllSections));
               output.Add(String.Format(" Eff. Time / Frame : {0} - {1:" + PpdFormatString + "} PPD",
                  UnitInfo.TimePerUnitDownload, UnitInfo.PPDPerUnitDownload));
            }
            
            output.Add(String.Empty);
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} could not find Project ID '{1}'.", HfmTrace.FunctionName, _ProjectID));
         }

         return output.ToArray();
      } 
      #endregion
   }
   
   [ProtoContract]
   public sealed class FrameTime
   {
      public FrameTime()
      {
         
      }

      public FrameTime(TimeSpan duration)
      {
         _Duration = duration;
      }

      private TimeSpan _Duration;
      [XmlIgnore]
      public TimeSpan Duration
      {
         get { return _Duration; }
      }
      
      [ProtoMember(1)]
      [XmlAttribute("Duration")]
      [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
      public long DurationTicks
      {
         get { return _Duration.Ticks; }
         set { _Duration = new TimeSpan(value); }
      }
   }
}
