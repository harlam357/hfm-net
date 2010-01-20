/*
 * HFM.NET - Benchmark Collection Class
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances
{
   [Serializable]
   public class ProteinBenchmarkCollection : IProteinBenchmarkCollection
   {
      #region Constants
      /// <summary>
      /// Benchmark Data File Name
      /// </summary>
      private const string DataStoreFilename = "BenchmarkCache.dat";
      #endregion

      #region Members
      /// <summary>
      /// Benchmark List
      /// </summary>
      private readonly List<InstanceProteinBenchmark> _benchmarkList = new List<InstanceProteinBenchmark>(); 
      #endregion

      #region Implementation
      /// <summary>
      /// Update Benchmark Data from given UnitInfo and frame indexes
      /// </summary>
      /// <param name="unit">The UnitInfo containing the UnitFrame data</param>
      /// <param name="startingFrame">Starting Frame Index</param>
      /// <param name="endingFrame">Ending Frame Index</param>
      public void UpdateBenchmarkData(IUnitInfo unit, int startingFrame, int endingFrame)
      {
         // project is not known, don't add to benchmark data
         if (unit.ProjectIsUnknown) return;
      
         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;
         
         InstanceProteinBenchmark findBenchmark = FindBenchmark(unit);
         if (findBenchmark == null)
         {
            InstanceProteinBenchmark newBenchmark =
               new InstanceProteinBenchmark(unit.OwningInstanceName, unit.OwningInstancePath, unit.ProjectID);

            if (UpdateBenchmarkFrames(unit, startingFrame, endingFrame, newBenchmark))
            {
               _benchmarkList.Add(newBenchmark);
            }
         }
         else
         {
            UpdateBenchmarkFrames(unit, startingFrame, endingFrame, findBenchmark);
         }
      }

      /// <summary>
      /// Update Benchmark Data from given UnitInfo and frame indexes
      /// </summary>
      /// <param name="unit">The UnitInfo containing the UnitFrame data</param>
      /// <param name="startingFrame">Starting Frame Index</param>
      /// <param name="endingFrame">Ending Frame Index</param>
      /// <param name="benchmark">The InstanceProteinBenchmark to Update</param>
      private static bool UpdateBenchmarkFrames(IUnitInfo unit, int startingFrame, int endingFrame, InstanceProteinBenchmark benchmark)
      {
         bool result = false;
      
         for (int i = startingFrame; i <= endingFrame; i++)
         {
            if (unit.UnitFrames[i] != null)
            {
               if (benchmark.SetFrameTime(unit.UnitFrames[i].FrameDuration))
               {
                  result = true;
               }
               if (unit.UnitFrames[i].FrameID == unit.Frames)
               {
                  UnitInfoCollection.WriteCompletedUnitInfo(unit);
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose,
                                          String.Format(CultureInfo.CurrentCulture, "{0} ({1}) FrameID '{2}' Not Found ({3})",
                                                        HfmTrace.FunctionName, unit.OwningInstanceName, i, unit.ProjectRunCloneGen));
            }
         }
         
         return result;
      }

      /// <summary>
      /// Get the Benchmark Average frame time based on the Owner and Project info from the given UnitInfo
      /// </summary>
      /// <param name="unit">The UnitInfo containing the Owner and Project data</param>
      public TimeSpan GetBenchmarkAverageFrameTime(IUnitInfo unit)
      {
         InstanceProteinBenchmark findBenchmark = FindBenchmark(unit);
         if (findBenchmark != null)
         {
            return findBenchmark.AverageFrameTime;
         }

         return TimeSpan.Zero;
      }

      /// <summary>
      /// Find the Benchmark based on the Owner and Project info from the given UnitInfo
      /// </summary>
      /// <param name="unit">The UnitInfo containing the Owner and Project data</param>
      private InstanceProteinBenchmark FindBenchmark(IUnitInfo unit)
      {
         return _benchmarkList.Find(delegate(InstanceProteinBenchmark proteinBenchmark)
         {
            return proteinBenchmark.OwningInstanceName == unit.OwningInstanceName &&
                   proteinBenchmark.OwningInstancePath == unit.OwningInstancePath &&
                   proteinBenchmark.ProjectID == unit.ProjectID;
         });
      }
      
      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      public void Delete(IBenchmarkClient Client)
      {
         if (Client.AllClients) throw new InvalidOperationException("Cannot delete All Clients.");
      
         _benchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
         {
            BenchmarkClient benchmarkClient = new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
            return benchmarkClient.Equals(Client);
         });
         Serialize();
      }

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      public void Delete(IBenchmarkClient Client, int ProjectID)
      {
         _benchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (Client.AllClients)
            {
               return benchmark.ProjectID.Equals(ProjectID);
            }
            else
            {
               BenchmarkClient benchmarkClient =
                  new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
               if (benchmarkClient.Equals(Client))
               {
                  return benchmark.ProjectID.Equals(ProjectID);
               }
               
               return false;
            }
         });
         Serialize();
      }
      
      /// <summary>
      /// Determine if the given BenchmarkClient exists in the Benchmarks data
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      public bool ContainsClient(IBenchmarkClient Client)
      {
         return _benchmarkList.Find(delegate(InstanceProteinBenchmark benchmark)
         {
            if (Client.AllClients)
            {
               return true;
            }
            else
            {
               BenchmarkClient benchmarkClient =
                  new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
               if (benchmarkClient.Equals(Client))
               {
                  return true;
               }
               
               return false;
            }
         }) != null;
      }
      
      /// <summary>
      /// Refresh the Minimum Frame Time for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      public void RefreshMinimumFrameTime(IBenchmarkClient Client, int ProjectID)
      {
         List<InstanceProteinBenchmark> benchmarks = GetBenchmarks(Client, ProjectID);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.RefreshBenchmarkMinimumFrameTime();
         }
         Serialize();
      }

      /// <summary>
      /// Get List of BenchmarkClients
      /// </summary>
      public IBenchmarkClient[] GetBenchmarkClients()
      {
         List<IBenchmarkClient> BenchmarkClients = new List<IBenchmarkClient>();
         BenchmarkClients.Add(new BenchmarkClient());
         
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (BenchmarkClients.Contains(benchmark.Client) == false)
            {
               BenchmarkClients.Add(benchmark.Client);
            }
         }
         
         IBenchmarkClient[] returnArray = BenchmarkClients.ToArray();
         Array.Sort(returnArray);

         return returnArray;
      }

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      public int[] GetBenchmarkProjects()
      {
         return GetBenchmarkProjects(new BenchmarkClient());
      }

      /// <summary>
      /// Get List of Benchmark Project Numbers
      /// </summary>
      public int[] GetBenchmarkProjects(IBenchmarkClient Client)
      {
         List<int> Projects = new List<int>();
         
         foreach (InstanceProteinBenchmark benchmark in _benchmarkList)
         {
            if (Projects.Contains(benchmark.ProjectID) == false)
            {
               if (Client.AllClients)
               {
                  Projects.Add(benchmark.ProjectID);
               }
               else
               {
                  if (benchmark.Client.Equals(Client))
                  {
                     Projects.Add(benchmark.ProjectID);
                  }
               }
            }
         }

         int[] returnArray = Projects.ToArray();
         Array.Sort(returnArray);

         return returnArray;
      }
      
      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      public List<InstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient Client)
      {
         return _benchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (Client.AllClients)
            {
               return true;
            }
            else
            {
               return benchmark.Client.Equals(Client);
            }
         });
      }

      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      public List<InstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient Client, int ProjectID)
      {
         return _benchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (Client.AllClients)
            {
               return benchmark.ProjectID.Equals(ProjectID);
            }
            else
            {
               if (benchmark.Client.Equals(Client))
               {
                  return benchmark.ProjectID.Equals(ProjectID);
               }

               return false;
            }
         });
      }
      
      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Name
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewName">New Benchmark Owner Name</param>
      public void UpdateInstanceName(IBenchmarkClient Client, string NewName)
      {
         List<InstanceProteinBenchmark> benchmarks = GetBenchmarks(Client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstanceName = NewName;
         }
         Serialize();
      }

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Path
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewPath">New Benchmark Owner Path</param>
      public void UpdateInstancePath(IBenchmarkClient Client, string NewPath)
      {
         List<InstanceProteinBenchmark> benchmarks = GetBenchmarks(Client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstancePath = NewPath;
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
            #region Benchmark File Migration Code (v0.3.0 => v0.4.0)
            lock (classLock)
            {
               IPreferenceSet Prefs = InstanceProvider.GetInstance<IPreferenceSet>();
               string oldfile = Path.Combine(Prefs.ApplicationPath, DataStoreFilename);
               string newfile = Path.Combine(Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);

               // Look for file in new location first
               if (_Instance == null)
               {
                  try
                  {
                     _Instance = Deserialize(newfile);
                  }
                  catch (UnauthorizedAccessException ex)
                  {
                      HfmTrace.WriteToHfmConsole(ex);
                  }
               }
               
               // If not found, look for old file location
               if (_Instance == null)
               {
                  // If it exists
                  if (File.Exists(oldfile))
                  {
                     try
                     {
                        try
                        {
                           // Try and deserialize it
                           _Instance = Deserialize(oldfile);
                           // If that succeeds, delete the old file
                           File.Delete(oldfile);
                        }
                        catch (UnauthorizedAccessException)
                        {
                           // If file permissions stop us from deserializing
                           // Copy the old file to the new location
                           File.Copy(oldfile, newfile, false);

                           // Try and deserialize from the new location
                           _Instance = Deserialize(newfile);
                        }
                     }
                     catch (Exception ex)
                     {
                        HfmTrace.WriteToHfmConsole(ex);
                     }
                  }
               }
               
               // If all else fails or no benchmark file exists, create a new collection
               if (_Instance == null)
               {
                  _Instance = new ProteinBenchmarkCollection();
               }
            }
            #endregion

            #region Standard Code (commented)
            //lock (classLock)
            //{
            //   if (_Instance == null)
            //   {
            //      _Instance = Deserialize(Path.Combine(PreferenceSet.Instance.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
            //   }
            //   if (_Instance == null)
            //   {
            //      _Instance = new ProteinBenchmarkCollection();
            //   }
            //}
            #endregion
            
            return _Instance;
         }
      }
      #endregion

      #region Serialization Support
      public static void Serialize()
      {
         Serialize(Instance, Path.Combine(InstanceProvider.GetInstance<IPreferenceSet>().GetPreference<string>(
                                             Preference.ApplicationDataFolderPath), DataStoreFilename));
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
         /*** Throw for Benchmark File Migration (Remove With Migration Code) ***/
         catch (UnauthorizedAccessException)
         {
            throw;
         }
         /*** Throw for Benchmark File Migration (Remove With Migration Code) ***/
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
