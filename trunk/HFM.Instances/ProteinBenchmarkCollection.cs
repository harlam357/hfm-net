/*
 * HFM.NET - Benchmark Collection Helper Class
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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HFM.Instrumentation;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Instances
{
   [Serializable]
   public class ProteinBenchmarkCollection
   {
      #region Members
      private const string DataStoreFilename = "BenchmarkCache.dat";

      private readonly List<InstanceProteinBenchmark> _benchmarkList = new List<InstanceProteinBenchmark>(); 
      #endregion

      #region Implementation
      public void UpdateBenchmarkData(UnitInfo unit, int startingFrame, int endingFrame)
      {
         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;

         InstanceProteinBenchmark findBenchmark = FindBenchmark(unit);

         if (findBenchmark == null)
         {
            InstanceProteinBenchmark newBenchmark =
               new InstanceProteinBenchmark(unit.OwningInstanceName, unit.OwningInstancePath, unit.ProjectID);

            SetFrames(unit, newBenchmark, startingFrame, endingFrame);
            _benchmarkList.Add(newBenchmark);
         }
         else
         {
            SetFrames(unit, findBenchmark, startingFrame, endingFrame);
         }
      }

      private static void SetFrames(UnitInfo unit, InstanceProteinBenchmark benchmark, int startingFrame, int endingFrame)
      {
         for (int i = startingFrame; i <= endingFrame; i++)
         {
            if (unit.UnitFrames[i] != null)
            {
               benchmark.SetFrameTime(unit.UnitFrames[i].FrameDuration);
               if (unit.UnitFrames[i].FramePercent == 100)
               {
                  UnitInfoCollection.Instance.WriteCompletedUnitInfo(unit);
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                          String.Format("{0} ({1}) FrameID '{2}' Not Found ({3})",
                                                        HfmTrace.FunctionName, unit.OwningInstanceName, i, unit.ProjectRunCloneGen));
            }
         }
      }

      public TimeSpan GetBenchmarkAverageFrameTime(UnitInfo unit)
      {
         InstanceProteinBenchmark findBenchmark = FindBenchmark(unit);

         if (findBenchmark != null)
         {
            return findBenchmark.AverageFrameTime;
         }

         return TimeSpan.Zero;
      }

      private InstanceProteinBenchmark FindBenchmark(UnitInfo unit)
      {
         return _benchmarkList.Find(delegate(InstanceProteinBenchmark proteinBenchmark)
                                    {
                                       return proteinBenchmark.OwningInstanceName == unit.OwningInstanceName &&
                                              proteinBenchmark.OwningInstancePath == unit.OwningInstancePath &&
                                              proteinBenchmark.ProjectID == unit.ProjectID;
                                    });
      }

      public int[] GetBenchmarkProjects()
      {
         List<int> Projects = new List<int>();
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (Projects.Contains(benchmark.ProjectID) == false)
            {
               Projects.Add(benchmark.ProjectID);
            }
         }

         int[] returnArray = Projects.ToArray();

         Array.Sort(returnArray);

         return returnArray;
      }

      public List<InstanceProteinBenchmark> GetProjectBenchmarks(int ProjectID)
      {
         List<InstanceProteinBenchmark> Benchmarks = new List<InstanceProteinBenchmark>();
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (benchmark.ProjectID == ProjectID)
            {
               Benchmarks.Add(benchmark);
            }
         }

         return Benchmarks;
      }

      public void UpdateInstanceName(string oldName, string newName)
      {
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (benchmark.OwningInstanceName == oldName)
            {
               benchmark.OwningInstanceName = newName;
            }
         }

         Serialize();
      }

      public void UpdateInstancePath(string instanceName, string newPath)
      {
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (instanceName == benchmark.OwningInstanceName)
            {
               benchmark.OwningInstancePath = newPath;
            }
         }

         Serialize();
      } 
      #endregion

      #region Singleton Support
      private static ProteinBenchmarkCollection _Instance;
      private static readonly object classLock = typeof(ProteinBenchmarkCollection);

      public static ProteinBenchmarkCollection Instance
      {
         get
         {
            lock (classLock)
            {
               if (_Instance == null)
               {
                  _Instance = Deserialize(Path.Combine(PreferenceSet.AppPath, DataStoreFilename));
               }
               if (_Instance == null)
               {
                  _Instance = new ProteinBenchmarkCollection();
               }
            }
            return _Instance;
         }
      }
      #endregion

      #region Serialization Support
      public void Serialize()
      {
         Serialize(Instance, Path.Combine(PreferenceSet.AppPath, DataStoreFilename));
      }

      private static ProteinBenchmarkCollection Deserialize(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;
      
         ProteinBenchmarkCollection collection = null;

         FileStream fileStream = null;
         BinaryFormatter formatter = new BinaryFormatter();
         try
         {
            fileStream = new FileStream(filePath, FileMode.Open);
            collection = (ProteinBenchmarkCollection)formatter.Deserialize(fileStream);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            if (fileStream != null)
            {
               fileStream.Close();
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);

         return collection;
      }

      private static readonly object _serializeLock = typeof(ProteinBenchmarkCollection);

      private static void Serialize(ProteinBenchmarkCollection collection, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;
      
         lock (_serializeLock)
         {
            FileStream fileStream = null;
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
               fileStream = new FileStream(filePath, FileMode.Create);
               formatter.Serialize(fileStream, collection);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
            finally
            {
               if (fileStream != null)
               {
                  fileStream.Close();
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
      #endregion
   }
}
