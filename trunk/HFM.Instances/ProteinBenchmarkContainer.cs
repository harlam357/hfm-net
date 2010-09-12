/*
 * HFM.NET - Benchmark Container Class
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
using System.Text;
using System.Xml.Serialization;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public class ProteinBenchmarkContainer : IProteinBenchmarkContainer
   {
      #region Constants
      
      /// <summary>
      /// Benchmark Data File Name
      /// </summary>
      private const string DataStoreFilename = "BenchmarkCache.dat";
      /// <summary>
      /// Legacy Benchmark Data File Name
      /// </summary>
      private const string LegacyDataStoreFilename = "LegacyBenchmarkCache.dat";
      
      #endregion

      #region Fields
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _prefs;

      private readonly IUnitInfoDatabase _database;
      
      /// <summary>
      /// Benchmark Collection
      /// </summary>
      private ProteinBenchmarkCollection _collection;
      
      #endregion

      #region Constructor
      public ProteinBenchmarkContainer(IPreferenceSet prefs, IUnitInfoDatabase database)
      {
         _prefs = prefs;
         _database = database;
      } 
      #endregion

      #region Implementation

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="currentUnitInfo">Current UnitInfo</param>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="nextUnitIndex">Index of Current UnitInfo</param>
      public void UpdateBenchmarkData(IUnitInfoLogic currentUnitInfo, IUnitInfoLogic[] parsedUnits, int nextUnitIndex)
      {
         bool foundCurrent = false;
         bool processUpdates = false;
         int index = nextUnitIndex;

         #region Set index for the oldest unit in the array
         if (index == parsedUnits.Length - 1)
         {
            index = 0;
         }
         else
         {
            index++;
         }
         #endregion

         while (index != -1)
         {
            // If Current has not been found, check the nextUnitIndex
            // or try to match the Current Project and Raw Download Time
            if (processUpdates == false && (index == nextUnitIndex ||
                                            PlatformOps.IsUnitInfoCurrentUnitInfo(currentUnitInfo, parsedUnits[index])))
            {
               foundCurrent = true;
               processUpdates = true;
            }

            if (processUpdates)
            {
               int previousFrameID = 0;
               if (foundCurrent)
               {
                  // current frame has already been recorded, increment to the next frame
                  previousFrameID = currentUnitInfo.LastUnitFrameID + 1;
                  foundCurrent = false;
               }

               // Even though the CurrentUnitInfo has been found in the parsed UnitInfoLogic array doesn't
               // mean that all entries in the array will be present.  See TestFiles\SMP_12\FAHlog.txt.
               if (parsedUnits[index] != null)
               {
                  // Update benchmarks
                  UpdateBenchmarkData(parsedUnits[index].UnitInfoData, previousFrameID, parsedUnits[index].LastUnitFrameID);

                  // Write Completed Unit Info only for units that are NOT current (i.e. have moved into history)
                  // For some WUs (typically bigadv) all frames could be complete but the FinishedTime read from
                  // the queue.dat is not yet populated.  To write this units production using an accurate bonus
                  // multiplier that FinishedTime needs to be populated.
                  if (index != nextUnitIndex)
                  {
                     // Make sure all Frames have been completed (not necessarily observed, but completed)
                     if (parsedUnits[index].AllFramesAreCompleted)
                     {
                        _database.WriteCompletedUnitInfo(parsedUnits[index]);
                     }
                  }
               }
            }

            #region Increment to the next unit or set terminal value
            if (index == nextUnitIndex)
            {
               index = -1;
            }
            else if (index == parsedUnits.Length - 1)
            {
               index = 0;
            }
            else
            {
               index++;
            }
            #endregion
         }
      }
      
      /// <summary>
      /// Update Benchmark Data from given UnitInfo and frame indexes
      /// </summary>
      /// <param name="unit">The UnitInfo containing the UnitFrame data</param>
      /// <param name="startingFrame">Starting Frame Index</param>
      /// <param name="endingFrame">Ending Frame Index</param>
      private void UpdateBenchmarkData(IUnitInfo unit, int startingFrame, int endingFrame)
      {
         if (unit == null) throw new ArgumentNullException("unit", "Argument 'unit' cannot be null.");

         // project is not known, don't add to benchmark data
         if (unit.ProjectIsUnknown) return;

         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;

         IInstanceProteinBenchmark findBenchmark = FindBenchmark(unit);
         if (findBenchmark == null)
         {
            var newBenchmark = new InstanceProteinBenchmark(unit.OwningInstanceName, unit.OwningInstancePath, unit.ProjectID);

            if (UpdateBenchmarkFrames(unit, startingFrame, endingFrame, newBenchmark))
            {
               _collection.BenchmarkList.Add(newBenchmark);
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
      private static bool UpdateBenchmarkFrames(IUnitInfo unit, int startingFrame, int endingFrame, IInstanceProteinBenchmark benchmark)
      {
         bool result = false;

         for (int i = startingFrame; i <= endingFrame; i++)
         {
            IUnitFrame frame = unit.GetUnitFrame(i);
            if (frame != null)
            {
               if (benchmark.SetFrameTime(frame.FrameDuration))
               {
                  result = true;
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format(CultureInfo.CurrentCulture, 
                  "({0}) FrameID '{1}' not found for Project ({2})", unit.OwningInstanceName, i, unit.ProjectID));
            }
         }

         return result;
      }

      /// <summary>
      /// Get the Benchmark Average frame time based on the Owner and Project info from the given UnitInfo
      /// </summary>
      /// <param name="unit">The UnitInfo containing the Owner and Project data</param>
      public TimeSpan GetBenchmarkAverageFrameTime(IUnitInfoLogic unit)
      {
         IInstanceProteinBenchmark findBenchmark = FindBenchmark(unit.UnitInfoData);
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
      private IInstanceProteinBenchmark FindBenchmark(IUnitInfo unit)
      {
         return _collection.BenchmarkList.Find(benchmark => benchmark.OwningInstanceName == unit.OwningInstanceName &&
                                                            StringOps.PathsEqual(benchmark.OwningInstancePath, unit.OwningInstancePath) &&
                                                            benchmark.ProjectID == unit.ProjectID);
      }

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      public void Delete(IBenchmarkClient client)
      {
         if (client.AllClients) throw new InvalidOperationException("Cannot delete All Clients.");

         _collection.BenchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
         {
            BenchmarkClient benchmarkClient = new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
            return benchmarkClient.Equals(client);
         });
         Write();
      }

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient and Project ID
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectId">Project Number</param>
      public void Delete(IBenchmarkClient client, int projectId)
      {
         _collection.BenchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (client.AllClients)
            {
               return benchmark.ProjectID.Equals(projectId);
            }
            else
            {
               BenchmarkClient benchmarkClient =
                  new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
               if (benchmarkClient.Equals(client))
               {
                  return benchmark.ProjectID.Equals(projectId);
               }

               return false;
            }
         });
         Write();
      }

      /// <summary>
      /// Determine if the given BenchmarkClient exists in the Benchmarks data
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      public bool ContainsClient(IBenchmarkClient client)
      {
         return _collection.BenchmarkList.Find(delegate(InstanceProteinBenchmark benchmark)
         {
            if (client.AllClients)
            {
               return true;
            }
            else
            {
               BenchmarkClient benchmarkClient =
                  new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
               if (benchmarkClient.Equals(client))
               {
                  return true;
               }

               return false;
            }
         }) != null;
      }

      /// <summary>
      /// Refresh the Minimum Frame Time for the given BenchmarkClient and Project ID
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectId">Project Number</param>
      public void RefreshMinimumFrameTime(IBenchmarkClient client, int projectId)
      {
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(client, projectId);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.RefreshBenchmarkMinimumFrameTime();
         }
         Write();
      }

      /// <summary>
      /// Get List of BenchmarkClients
      /// </summary>
      public IBenchmarkClient[] GetBenchmarkClients()
      {
         List<IBenchmarkClient> benchmarkClients = new List<IBenchmarkClient>();
         benchmarkClients.Add(new BenchmarkClient());

         foreach (InstanceProteinBenchmark benchmark in _collection.BenchmarkList)
         {
            if (benchmarkClients.Contains(benchmark.Client) == false)
            {
               benchmarkClients.Add(benchmark.Client);
            }
         }

         IBenchmarkClient[] returnArray = benchmarkClients.ToArray();
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
      public int[] GetBenchmarkProjects(IBenchmarkClient client)
      {
         List<int> projects = new List<int>();

         foreach (InstanceProteinBenchmark benchmark in _collection.BenchmarkList)
         {
            if (projects.Contains(benchmark.ProjectID) == false)
            {
               if (client.AllClients)
               {
                  projects.Add(benchmark.ProjectID);
               }
               else
               {
                  if (benchmark.Client.Equals(client))
                  {
                     projects.Add(benchmark.ProjectID);
                  }
               }
            }
         }

         int[] returnArray = projects.ToArray();
         Array.Sort(returnArray);

         return returnArray;
      }

      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      public List<IInstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient client)
      {
         List<InstanceProteinBenchmark> list = _collection.BenchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (client.AllClients)
            {
               return true;
            }
            else
            {
               return benchmark.Client.Equals(client);
            }
         });
         
         List<IInstanceProteinBenchmark> interfaceList = 
            list.ConvertAll(benchmark => (IInstanceProteinBenchmark)benchmark);
               
         return interfaceList;
      }

      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="projectId">Project Number</param>
      public List<IInstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient client, int projectId)
      {
         List<InstanceProteinBenchmark> list = _collection.BenchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
         {
            if (client.AllClients)
            {
               return benchmark.ProjectID.Equals(projectId);
            }
            else
            {
               if (benchmark.Client.Equals(client))
               {
                  return benchmark.ProjectID.Equals(projectId);
               }

               return false;
            }
         });

         List<IInstanceProteinBenchmark> interfaceList =
            list.ConvertAll(benchmark => (IInstanceProteinBenchmark)benchmark);
               
         return interfaceList;
      }

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Name
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="newName">New Benchmark Owner Name</param>
      public void UpdateInstanceName(IBenchmarkClient client, string newName)
      {
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstanceName = newName;
         }
         Write();
      }

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Path
      /// </summary>
      /// <param name="client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="newPath">New Benchmark Owner Path</param>
      public void UpdateInstancePath(IBenchmarkClient client, string newPath)
      {
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstancePath = newPath;
         }
         Write();
      }
      #endregion

      #region Serialization Support
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read()
      {
         Read(false);
      }
      
      /// <summary>
      /// Read Binary File
      /// </summary>
      public void Read(bool merge)
      {
         string filePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);

         ProteinBenchmarkCollection collection = DeserializeLegacy(filePath);
         if (collection == null)
         {
            collection = Deserialize(filePath);
         }
         else
         {
            BackupNetSerializedDataFile(filePath);
         }

         _collection = merge ? MergeCollections(_collection, collection) : collection;
         if (_collection == null)
         {
            _collection = new ProteinBenchmarkCollection();
         }
      }

      /// <summary>
      /// Write Binary File
      /// </summary>
      public void Write()
      {
         Serialize(_collection, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }

      /// <summary>
      /// Read Xml File
      /// </summary>
      public void ReadXml(string filePath)
      {
         ReadXml(filePath, false);
      }

      /// <summary>
      /// Read Xml File
      /// </summary>
      public void ReadXml(string filePath, bool merge)
      {
         ProteinBenchmarkCollection collection = DeserializeFromXml(Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), filePath));
         
         _collection = merge ? MergeCollections(_collection, collection) : collection;
         if (_collection == null)
         {
            _collection = new ProteinBenchmarkCollection();
         }
      }

      /// <summary>
      /// Write Xml File
      /// </summary>
      public void WriteXml(string filePath)
      {
         SerializeToXml(_collection, Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), filePath));
      }
      
      private void BackupNetSerializedDataFile(string filePath)
      {
         string legacyFilePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), LegacyDataStoreFilename);

         try
         {
            // Backward Compatibility - Backup .NET Serialized Data File
            File.Copy(filePath, legacyFilePath);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      /// <summary>
      /// Merge two Benchmark Collections
      /// </summary>
      /// <param name="collection1">Collection 1</param>
      /// <param name="collection2">Collection 2</param>
      public static ProteinBenchmarkCollection MergeCollections(ProteinBenchmarkCollection collection1, ProteinBenchmarkCollection collection2)
      {
         if (collection1 == null)
         {
            if (collection2 != null)
            {
               // no collection1 - return collection2
               return collection2;
            }
         }
         else // we have a collection1
         {
            if (collection2 != null)
            {
               // we have both - merge
               ProteinBenchmarkCollection mergedCollection = new ProteinBenchmarkCollection();
               mergedCollection.BenchmarkList.AddRange(collection1.BenchmarkList);
               mergedCollection.BenchmarkList.AddRange(collection2.BenchmarkList);
               return mergedCollection;
            }

            // no collection2 - return collection1
            return collection1;
         }

         // have no collections
         return null;
      }

      private static readonly object SerializeLock = typeof(ProteinBenchmarkCollection);

      public static void Serialize(ProteinBenchmarkCollection collection, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               try
               {
                  ProtoBuf.Serializer.Serialize(fileStream, collection);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static void SerializeToXml(ProteinBenchmarkCollection collection, string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         lock (SerializeLock)
         {
            using (TextWriter stream = new StreamWriter(filePath, false, Encoding.UTF8))
            {
               try
               {
                  XmlSerializer s = new XmlSerializer(typeof(ProteinBenchmarkCollection));
                  s.Serialize(stream, collection);
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      public static ProteinBenchmarkCollection Deserialize(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         ProteinBenchmarkCollection collection = null;
         try
         {
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
               collection = ProtoBuf.Serializer.Deserialize<ProteinBenchmarkCollection>(fileStream);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return collection;
      }

      public static ProteinBenchmarkCollection DeserializeLegacy(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         ProteinBenchmarkCollection collection = null;

         FileStream fileStream = null;
         BinaryFormatter formatter = new BinaryFormatter();
         try
         {
            fileStream = new FileStream(filePath, FileMode.Open);
            collection = (ProteinBenchmarkCollection)formatter.Deserialize(fileStream);
            collection.BenchmarkList.ForEach(benchmark => benchmark.ConvertQueueToList());
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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);

         return collection;
      }

      public static ProteinBenchmarkCollection DeserializeFromXml(string filePath)
      {
         DateTime start = HfmTrace.ExecStart;

         ProteinBenchmarkCollection collection = null;
         using (FileStream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
         {
            try
            {
               XmlSerializer s = new XmlSerializer(typeof(ProteinBenchmarkCollection));
               collection = (ProteinBenchmarkCollection)s.Deserialize(stream);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
         
         return collection;
      }
      #endregion
   }
}
