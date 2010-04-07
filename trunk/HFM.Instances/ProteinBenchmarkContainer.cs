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

      #region Members
      /// <summary>
      /// Benchmark Collection
      /// </summary>
      private ProteinBenchmarkCollection _collection;
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs; 
      #endregion

      #region Constructor
      public ProteinBenchmarkContainer(IPreferenceSet Prefs)
      {
         _Prefs = Prefs;
      } 
      #endregion

      #region Implementation
      /// <summary>
      /// Update Benchmark Data from given UnitInfo and frame indexes
      /// </summary>
      /// <param name="unit">The UnitInfo containing the UnitFrame data</param>
      /// <param name="startingFrame">Starting Frame Index</param>
      /// <param name="endingFrame">Ending Frame Index</param>
      public void UpdateBenchmarkData(IUnitInfoLogic unit, int startingFrame, int endingFrame)
      {
         if (unit == null) throw new ArgumentNullException("unit", "Argument 'unit' cannot be null.");

         // project is not known, don't add to benchmark data
         if (unit.ProjectIsUnknown) return;

         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;

         IInstanceProteinBenchmark findBenchmark = FindBenchmark(unit);
         if (findBenchmark == null)
         {
            InstanceProteinBenchmark newBenchmark =
               new InstanceProteinBenchmark(unit.OwningInstanceName, unit.OwningInstancePath, unit.ProjectID);

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
      private static bool UpdateBenchmarkFrames(IUnitInfoLogic unit, int startingFrame, int endingFrame, IInstanceProteinBenchmark benchmark)
      {
         bool result = false;

         for (int i = startingFrame; i <= endingFrame; i++)
         {
            IUnitFrame frame = unit.UnitInfoData.GetUnitFrame(i);
            if (frame != null)
            {
               if (benchmark.SetFrameTime(frame.FrameDuration))
               {
                  result = true;
               }

               /*** Logic Moved to ClientInstance.UpdateBenchmarkData() ***/
               //if (frame.FrameID == unit.Frames)
               //{
               //   UnitInfoContainer.WriteCompletedUnitInfo(unit);
               //}
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
      public TimeSpan GetBenchmarkAverageFrameTime(IUnitInfoLogic unit)
      {
         IInstanceProteinBenchmark findBenchmark = FindBenchmark(unit);
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
      private IInstanceProteinBenchmark FindBenchmark(IUnitInfoLogic unit)
      {
         return _collection.BenchmarkList.Find(delegate(InstanceProteinBenchmark proteinBenchmark)
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

         _collection.BenchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
         {
            BenchmarkClient benchmarkClient = new BenchmarkClient(benchmark.OwningInstanceName, benchmark.OwningInstancePath);
            return benchmarkClient.Equals(Client);
         });
         Write();
      }

      /// <summary>
      /// Delete all Benchmarks for the given BenchmarkClient and ProjectID
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      public void Delete(IBenchmarkClient Client, int ProjectID)
      {
         _collection.BenchmarkList.RemoveAll(delegate(InstanceProteinBenchmark benchmark)
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
         Write();
      }

      /// <summary>
      /// Determine if the given BenchmarkClient exists in the Benchmarks data
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      public bool ContainsClient(IBenchmarkClient Client)
      {
         return _collection.BenchmarkList.Find(delegate(InstanceProteinBenchmark benchmark)
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
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(Client, ProjectID);
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
         List<IBenchmarkClient> BenchmarkClients = new List<IBenchmarkClient>();
         BenchmarkClients.Add(new BenchmarkClient());

         foreach (InstanceProteinBenchmark benchmark in _collection.BenchmarkList)
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

         foreach (InstanceProteinBenchmark benchmark in _collection.BenchmarkList)
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
      public List<IInstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient Client)
      {
         List<InstanceProteinBenchmark> list = _collection.BenchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
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
         
         List<IInstanceProteinBenchmark> interfaceList = 
            list.ConvertAll(new Converter<InstanceProteinBenchmark, IInstanceProteinBenchmark>(
               delegate(InstanceProteinBenchmark benchmark)
               {
                  return (IInstanceProteinBenchmark)benchmark;
               }));
               
         return interfaceList;
      }

      /// <summary>
      /// Get List of Benchmarks for the given BenchmarkClient
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="ProjectID">Project Number</param>
      public List<IInstanceProteinBenchmark> GetBenchmarks(IBenchmarkClient Client, int ProjectID)
      {
         List<InstanceProteinBenchmark> list = _collection.BenchmarkList.FindAll(delegate(InstanceProteinBenchmark benchmark)
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

         List<IInstanceProteinBenchmark> interfaceList =
            list.ConvertAll(new Converter<InstanceProteinBenchmark, IInstanceProteinBenchmark>(
               delegate(InstanceProteinBenchmark benchmark)
               {
                  return (IInstanceProteinBenchmark)benchmark;
               }));
               
         return interfaceList;
      }

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Name
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewName">New Benchmark Owner Name</param>
      public void UpdateInstanceName(IBenchmarkClient Client, string NewName)
      {
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(Client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstanceName = NewName;
         }
         Write();
      }

      /// <summary>
      /// Update the given BenchmarkClient benchmarks with the new Owning Instance Path
      /// </summary>
      /// <param name="Client">BenchmarkClient containing Client Name and Path data</param>
      /// <param name="NewPath">New Benchmark Owner Path</param>
      public void UpdateInstancePath(IBenchmarkClient Client, string NewPath)
      {
         IList<IInstanceProteinBenchmark> benchmarks = GetBenchmarks(Client);
         foreach (InstanceProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstancePath = NewPath;
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
      public void Read(bool Merge)
      {
         string FilePath = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename);

         ProteinBenchmarkCollection collection = DeserializeLegacy(FilePath);
         if (collection == null)
         {
            collection = Deserialize(FilePath);
         }
         else
         {
            BackupNetSerializedDataFile(FilePath);
         }

         if (Merge)
         {
            _collection = MergeCollections(_collection, collection);
         }
         else
         {
            _collection = collection;
         }

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
         Serialize(_collection, Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), DataStoreFilename));
      }

      /// <summary>
      /// Read Xml File
      /// </summary>
      public void ReadXml(string FilePath)
      {
         ReadXml(FilePath, false);
      }

      /// <summary>
      /// Read Xml File
      /// </summary>
      public void ReadXml(string FilePath, bool Merge)
      {
         ProteinBenchmarkCollection collection = DeserializeFromXml(Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), FilePath));
         
         if (Merge)
         {
            _collection = MergeCollections(_collection, collection);
         }
         else
         {
            _collection = collection;
         }

         if (_collection == null)
         {
            _collection = new ProteinBenchmarkCollection();
         }
      }

      /// <summary>
      /// Write Xml File
      /// </summary>
      public void WriteXml(string FilePath)
      {
         SerializeToXml(_collection, Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), FilePath));
      }
      
      private void BackupNetSerializedDataFile(string FilePath)
      {
         string LegacyFilePath = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), LegacyDataStoreFilename);

         try
         {
            // Backward Compatibility - Backup .NET Serialized Data File
            File.Copy(FilePath, LegacyFilePath);
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

      private static readonly object _serializeLock = typeof(ProteinBenchmarkCollection);

      public static void Serialize(ProteinBenchmarkCollection collection, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         lock (_serializeLock)
         {
            //FileStream fileStream = null;
            //BinaryFormatter formatter = new BinaryFormatter();
            //try
            //{
            //   fileStream = new FileStream(filePath, FileMode.Create);
            //   formatter.Serialize(fileStream, collection);
            //}
            //catch (Exception ex)
            //{
            //   HfmTrace.WriteToHfmConsole(ex);
            //}
            //finally
            //{
            //   if (fileStream != null)
            //   {
            //      fileStream.Close();
            //   }
            //}

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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }

      public static void SerializeToXml(ProteinBenchmarkCollection collection, string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         lock (_serializeLock)
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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }

      public static ProteinBenchmarkCollection Deserialize(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);

         return collection;
      }

      public static ProteinBenchmarkCollection DeserializeLegacy(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

         ProteinBenchmarkCollection collection = null;

         FileStream fileStream = null;
         BinaryFormatter formatter = new BinaryFormatter();
         try
         {
            fileStream = new FileStream(filePath, FileMode.Open);
            collection = (ProteinBenchmarkCollection)formatter.Deserialize(fileStream);
            collection.BenchmarkList.ForEach(delegate(InstanceProteinBenchmark benchmark)
                                             {
                                                benchmark.ConvertQueueToList();
                                             });
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

      public static ProteinBenchmarkCollection DeserializeFromXml(string filePath)
      {
         DateTime Start = HfmTrace.ExecStart;

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

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
         
         return collection;
      }
      #endregion
   }
}
