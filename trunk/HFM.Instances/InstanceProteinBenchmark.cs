/*
 * HFM.NET - Benchmark Data Class
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
using System.Diagnostics;

using HFM.Framework;
using HFM.Preferences;
using HFM.Proteins;
using HFM.Instrumentation;

namespace HFM.Instances
{
   [Serializable]
   public class InstanceProteinBenchmark : IOwnedByClientInstance
   {
      #region Members & Read Only Properties

      private const Int32 DefaultMaxFrames = 300;
      
      #region Owner Data Properties
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      private string _OwningInstanceName;
      /// <summary>
      /// Name of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstanceName
      {
         get { return _OwningInstanceName; }
         set { _OwningInstanceName = value; }
      }

      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      private string _OwningInstancePath;
      /// <summary>
      /// Path of the Client Instance that owns this UnitInfo
      /// </summary>
      public string OwningInstancePath
      {
         get { return _OwningInstancePath; }
         set { _OwningInstancePath = value; }
      }
      #endregion

      private readonly Int32 _ProjectID;
      public Int32 ProjectID
      {
         get { return _ProjectID; }
      }

      private TimeSpan _MinimumFrameTime;
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
      }
      
      public double MinimumFrameTimePPD
      {
         get 
         { 
            if (Protein != null)
            {
               // Issue 125 & 129
               if (PreferenceSet.Instance.CalculateBonus)
               {
                  TimeSpan FinishTime = TimeSpan.FromMilliseconds(MinimumFrameTime.TotalMilliseconds * Protein.Frames);
                  return Protein.GetPPD(MinimumFrameTime, FinishTime);
               }

               return Protein.GetPPD(MinimumFrameTime);
            }
            
            return 0;
         }
      }

      public TimeSpan AverageFrameTime
      {
         get
         {
            if (_FrameTimes.Count > 0)
            {
               TimeSpan totalTime = TimeSpan.Zero;
               foreach (TimeSpan time in _FrameTimes)
               {
                  totalTime = totalTime.Add(time);
               }

               return TimeSpan.FromSeconds((Convert.ToInt32(totalTime.TotalSeconds) / _FrameTimes.Count));
            }

            return TimeSpan.Zero;
         }
      }

      public double AverageFrameTimePPD
      {
         get
         {
            if (Protein != null)
            {
               // Issue 125 & 129
               if (PreferenceSet.Instance.CalculateBonus)
               {
                  TimeSpan FinishTime = TimeSpan.FromMilliseconds(AverageFrameTime.TotalMilliseconds * Protein.Frames);
                  return Protein.GetPPD(AverageFrameTime, FinishTime);
               }

               return Protein.GetPPD(AverageFrameTime);
            }

            return 0;
         }
      }

      private readonly Queue<TimeSpan> _FrameTimes;
      public Queue<TimeSpan> FrameTimes
      {
         get { return _FrameTimes; }
      } 
      
      #endregion

      #region Constructor
      public InstanceProteinBenchmark(string ownerName, string ownerPath, Int32 proteinID)
      {
         _OwningInstanceName = ownerName;
         _OwningInstancePath = ownerPath;
         _ProjectID = proteinID;
         _MinimumFrameTime = TimeSpan.Zero;
         _FrameTimes = new Queue<TimeSpan>(DefaultMaxFrames);
      } 
      #endregion
      
      public BenchmarkClient Client
      {
         get 
         { 
            return new BenchmarkClient(OwningInstanceName, OwningInstancePath);
         }
      }
      
      public Protein Protein
      {
         get
         {
            Protein protein;
            ProteinCollection.Instance.TryGetValue(_ProjectID, out protein);

            return protein;
         }
      }

      #region Implementation
      public bool SetFrameTime(TimeSpan frameTime)
      {
         if (frameTime > TimeSpan.Zero)
         {
            if (frameTime < _MinimumFrameTime || _MinimumFrameTime.Equals(TimeSpan.Zero))
            {
               _MinimumFrameTime = frameTime;
            }

            // Dequeue once we have the Maximum number of frame times
            if (_FrameTimes.Count == DefaultMaxFrames)
            {
               _FrameTimes.Dequeue();
            }
            _FrameTimes.Enqueue(frameTime);
            
            return true;
         }
         
         return false;
      }

      public void RefreshBenchmarkMinimumFrameTime()
      {
         TimeSpan minimumFrameTime = TimeSpan.Zero;
         foreach (TimeSpan frameTime in FrameTimes)
         {
            if (frameTime < minimumFrameTime || minimumFrameTime.Equals(TimeSpan.Zero) )
            {
               minimumFrameTime = frameTime;
            }
         }

         if (minimumFrameTime.Equals(TimeSpan.Zero) == false)
         {
            _MinimumFrameTime = minimumFrameTime;
         }
      }

      public string[] ToMultiLineString(ClientInstance Instance)
      {
         List<string> output = new List<string>(12);

         Protein theProtein = Protein;
         if (theProtein != null)
         {
            output.Add(String.Empty);
            output.Add(String.Format(" Name: {0}", OwningInstanceName));
            output.Add(String.Format(" Path: {0}", OwningInstancePath));
            output.Add(String.Format(" Number of Frames Observed: {0}", FrameTimes.Count));
            output.Add(String.Empty);
            output.Add(String.Format(" Min. Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD", 
               MinimumFrameTime, MinimumFrameTimePPD));
            output.Add(String.Format(" Avg. Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD", 
               AverageFrameTime, AverageFrameTimePPD));

            if (Instance != null && Instance.CurrentUnitInfo.ProjectID.Equals(theProtein.ProjectNumber) &&
                                    Instance.ProductionValuesOk)
            {
               output.Add(String.Format(" Cur. Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD", 
                  Instance.CurrentUnitInfo.TimePerLastSection, Instance.CurrentUnitInfo.PPDPerLastSection));
               output.Add(String.Format(" R3F. Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD",
                  Instance.CurrentUnitInfo.TimePerThreeSections, Instance.CurrentUnitInfo.PPDPerThreeSections));
               output.Add(String.Format(" All  Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD",
                  Instance.CurrentUnitInfo.TimePerAllSections, Instance.CurrentUnitInfo.PPDPerAllSections));
               output.Add(String.Format(" Eff. Time / Frame : {0} - {1:" + PreferenceSet.PpdFormatString + "} PPD",
                  Instance.CurrentUnitInfo.TimePerUnitDownload, Instance.CurrentUnitInfo.PPDPerUnitDownload));
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
}
