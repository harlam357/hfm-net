/*
 * HFM.NET - Benchmark Collection Class
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
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IProteinBenchmarkCollection : ICollection<ProteinBenchmark>
   {
      /// <summary>
      /// List of BenchmarkClient objects.
      /// </summary>
      IEnumerable<BenchmarkClient> BenchmarkClients { get; }

      void UpdateData(UnitInfo unit, int startingFrame, int endingFrame);

      /// <summary>
      /// Gets the ProteinBenchmark based on the UnitInfo owner and project data.
      /// </summary>
      /// <param name="unitInfo">The UnitInfo containing owner and project data.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="unitInfo"/> is null.</exception>
      ProteinBenchmark GetBenchmark(UnitInfo unitInfo);

      /// <summary>
      /// Removes all the elements from the ProteinBenchmarkCollection that match the benchmarkClient.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to remove from the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="benchmarkClient"/> represents all clients.</exception>
      void RemoveAll(BenchmarkClient benchmarkClient);

      /// <summary>
      /// Removes all the elements from the ProteinBenchmarkCollection that match the benchmarkClient and projectId.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to remove from the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      void RemoveAll(BenchmarkClient benchmarkClient, int projectId);

      /// <summary>
      /// Determines whether the ProteinBenchmarkCollection contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="benchmarkClient"/> is found in the ProteinBenchmarkCollection; otherwise, false.
      /// </returns>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      bool Contains(BenchmarkClient benchmarkClient);

      /// <summary>
      /// Gets a list of benchmark project numbers.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      IEnumerable<int> GetBenchmarkProjects(BenchmarkClient benchmarkClient);

      /// <summary>
      /// Gets a list of ProteinBenchmark objects.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient);

      /// <summary>
      /// Gets a list of ProteinBenchmark objects.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient, int projectId);

      /// <summary>
      /// Updates the owner name of all the elements in ProteinBenchmarkCollection that match the given client name and path.
      /// </summary>
      void UpdateOwnerName(string clientName, string clientPath, string newName);

      /// <summary>
      /// Updates the owner path of all the elements in ProteinBenchmarkCollection that match the given client name and path.
      /// </summary>
      void UpdateOwnerPath(string clientName, string clientPath, string newPath);

      /// <summary>
      /// Updates the minimum frame time of all the elements in ProteinBenchmarkCollection that match the benchmarkClient and projectId.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      void UpdateMinimumFrameTime(BenchmarkClient benchmarkClient, int projectId);

      #region ICollection<ProteinBenchmark> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Adds a ProteinBenchmark to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The ProteinBenchmark to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">The <paramref name="item"/> already exists in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</exception>
      new void Add(ProteinBenchmark item);

      #endregion

      #region DataContainer<T>

      void Read();

      void Write();

      #endregion
   }

   public sealed class ProteinBenchmarkCollection : DataContainer<List<ProteinBenchmark>>, IProteinBenchmarkCollection
   {
      #region Properties

      public override Plugins.IFileSerializer<List<ProteinBenchmark>> DefaultSerializer
      {
         get { return new Serializers.ProtoBufFileSerializer<List<ProteinBenchmark>>(); }
      }

      public IEnumerable<BenchmarkClient> BenchmarkClients
      {
         get
         {
            var benchmarkClients = new List<BenchmarkClient> { new BenchmarkClient() };
            foreach (var benchmark in Data)
            {
               if (!benchmarkClients.Contains(benchmark.Client))
               {
                  benchmarkClients.Add(benchmark.Client);
               }
            }

            benchmarkClients.Sort();
            return benchmarkClients.AsReadOnly();
         }
      }

      #endregion

      private readonly ReaderWriterLockSlim _cacheLock;

      #region Constructor

      public ProteinBenchmarkCollection()
         : this(null)
      {
         
      } 

      public ProteinBenchmarkCollection(IPreferenceSet prefs)
      {
         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.BenchmarkCacheFileName);
         }
         _cacheLock = new ReaderWriterLockSlim();
      }

      #endregion

      #region Implementation

      #region UpdateData

      public void UpdateData(UnitInfo unit, int startingFrame, int endingFrame)
      {
         Debug.Assert(unit != null);

         // project is not known, don't add to benchmark data
         if (unit.ProjectIsUnknown()) return;

         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;

         // GetBenchmark() BEFORE entering write lock 
         // because it uses a read lock
         ProteinBenchmark findBenchmark = GetBenchmark(unit);
         // write lock
         _cacheLock.EnterWriteLock();
         try
         {
            if (findBenchmark == null)
            {
               var newBenchmark = new ProteinBenchmark
                                  {
                                     OwningClientName = unit.OwningClientName,
                                     OwningClientPath = unit.OwningClientPath,
                                     OwningSlotId = unit.OwningSlotId,
                                     ProjectID = unit.ProjectID
                                  };

               if (UpdateFrames(unit, startingFrame, endingFrame, newBenchmark))
               {
                  Data.Add(newBenchmark);
               }
            }
            else
            {
               UpdateFrames(unit, startingFrame, endingFrame, findBenchmark);
            }
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      private bool UpdateFrames(UnitInfo unit, int startingFrame, int endingFrame, ProteinBenchmark benchmark)
      {
         bool result = false;

         for (int i = startingFrame; i <= endingFrame; i++)
         {
            UnitFrame frame = unit.GetUnitFrame(i);
            if (frame != null)
            {
               if (benchmark.SetFrameTime(frame.FrameDuration))
               {
                  result = true;
               }
            }
            else
            {
               Logger.DebugFormat("({0}) FrameID '{1}' not found for Project {2}", unit.OwningSlotName, i, unit.ProjectID);
            }
         }

         return result;
      }

      #endregion

      public ProteinBenchmark GetBenchmark(UnitInfo unitInfo)
      {
         if (unitInfo == null) throw new ArgumentNullException("unitInfo");

         _cacheLock.EnterReadLock();
         try
         {
            return Data.Find(benchmark => benchmark.Equals(unitInfo));
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public void RemoveAll(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");
         if (benchmarkClient.AllClients) throw new ArgumentException("Cannot remove all clients.");

         _cacheLock.EnterWriteLock();
         try
         {
            Data.RemoveAll(benchmark => benchmark.Client.Equals(benchmarkClient));
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      public void RemoveAll(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         _cacheLock.EnterWriteLock();
         try
         {
            Data.RemoveAll(benchmark =>
                           {
                              if (benchmarkClient.AllClients)
                              {
                                 return benchmark.ProjectID.Equals(projectId);
                              }
                              if (benchmark.Client.Equals(benchmarkClient))
                              {
                                 return benchmark.ProjectID.Equals(projectId);
                              }
                              return false;
                           });
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      public bool Contains(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         _cacheLock.EnterReadLock();
         try
         {
            return Data.Find(benchmark =>
                             {
                                if (benchmarkClient.AllClients)
                                {
                                   return true;
                                }
                                if (benchmark.Client.Equals(benchmarkClient))
                                {
                                   return true;
                                }
                                return false;
                             }) != null;
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public IEnumerable<int> GetBenchmarkProjects(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         _cacheLock.EnterReadLock();
         try
         {
            var projects = new List<int>();
            foreach (var benchmark in Data)
            {
               if (projects.Contains(benchmark.ProjectID))
               {
                  continue;
               }

               if (benchmarkClient.AllClients)
               {
                  projects.Add(benchmark.ProjectID);
               }
               else
               {
                  if (benchmark.Client.Equals(benchmarkClient))
                  {
                     projects.Add(benchmark.ProjectID);
                  }
               }
            }

            projects.Sort();
            return projects.AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         _cacheLock.EnterReadLock();
         try
         {
            var list = Data.FindAll(benchmark =>
                                    {
                                       if (benchmarkClient.AllClients)
                                       {
                                          return true;
                                       }
                                       return benchmark.Client.Equals(benchmarkClient);
                                    });

            return list.AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         _cacheLock.EnterReadLock();
         try
         {
            var list = Data.FindAll(benchmark =>
                                    {
                                       if (benchmarkClient.AllClients)
                                       {
                                          return benchmark.ProjectID.Equals(projectId);
                                       }
                                       if (benchmark.Client.Equals(benchmarkClient))
                                       {
                                          return benchmark.ProjectID.Equals(projectId);
                                       }
                                       return false;
                                    });

            return list.AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public void UpdateOwnerName(string clientName, string clientPath, string newName)
      {
         if (clientName == null) throw new ArgumentNullException("clientName");
         if (clientPath == null) throw new ArgumentNullException("clientPath");
         if (newName == null) throw new ArgumentNullException("newName");
         
         // Core library - should have a valid client name 
         Debug.Assert(Validate.ClientName(newName));

         // GetBenchmarks() BEFORE entering write lock 
         // because it uses a read lock
         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarksForOwnerUpdate(clientName, clientPath);
         // write lock
         _cacheLock.EnterWriteLock();
         try
         {
            foreach (ProteinBenchmark benchmark in benchmarks)
            {
               benchmark.OwningClientName = newName;
            }
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      public void UpdateOwnerPath(string clientName, string clientPath, string newPath)
      {
         if (clientName == null) throw new ArgumentNullException("clientName");
         if (clientPath == null) throw new ArgumentNullException("clientPath");
         if (newPath == null) throw new ArgumentNullException("newPath");

         // GetBenchmarks() BEFORE entering write lock 
         // because it uses a read lock
         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarksForOwnerUpdate(clientName, clientPath);
         // write lock
         _cacheLock.EnterWriteLock();
         try
         {
            foreach (ProteinBenchmark benchmark in benchmarks)
            {
               benchmark.OwningClientPath = newPath;
            }
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      private IEnumerable<ProteinBenchmark> GetBenchmarksForOwnerUpdate(string clientName, string clientPath)
      {
         Debug.Assert(clientName != null);
         Debug.Assert(clientPath != null);

         _cacheLock.EnterReadLock();
         try
         {
            return Data.FindAll(benchmark => benchmark.OwningClientName.Equals(clientName) &&
                                             Paths.Equal(benchmark.OwningClientPath, clientPath)).AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public void UpdateMinimumFrameTime(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         // GetBenchmarks() BEFORE entering write lock 
         // because it uses a read lock
         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(benchmarkClient, projectId);
         // write lock
         _cacheLock.EnterWriteLock();
         try
         {
            foreach (ProteinBenchmark benchmark in benchmarks)
            {
               benchmark.UpdateMinimumFrameTime();
            }
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }
      
      #endregion

      #region ICollection<ProteinBenchmark> Members

      public void Add(ProteinBenchmark item)
      {
         if (item == null) throw new ArgumentNullException("item");
         if (Data.Contains(item)) throw new ArgumentException("The benchmark already exists.", "item");

         _cacheLock.EnterWriteLock();
         try
         {
            Data.Add(item);
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      [CoverageExclude]
      public void Clear()
      {
         _cacheLock.EnterWriteLock();
         try
         {
            Data.Clear();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      public bool Contains(ProteinBenchmark item)
      {
         _cacheLock.EnterReadLock();
         try
         {
            return item != null && Data.Contains(item);
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      [CoverageExclude]
      void ICollection<ProteinBenchmark>.CopyTo(ProteinBenchmark[] array, int arrayIndex)
      {
         _cacheLock.EnterReadLock();
         try
         {
            Data.CopyTo(array, arrayIndex);
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public int Count
      {
         [CoverageExclude]
         get
         {
            _cacheLock.EnterReadLock();
            try
            {
               return Data.Count;
            }
            finally
            {
               _cacheLock.ExitReadLock();
            }
         }
      }

      bool ICollection<ProteinBenchmark>.IsReadOnly
      {
         [CoverageExclude]
         get { return false; }
      }

      public bool Remove(ProteinBenchmark item)
      {
         _cacheLock.EnterWriteLock();
         try
         {
            return item != null && Data.Remove(item);
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      #endregion

      #region IEnumerable<ProteinBenchmark> Members

      [CoverageExclude]
      public IEnumerator<ProteinBenchmark> GetEnumerator()
      {
         _cacheLock.EnterReadLock();
         try
         {
            return Data.GetEnumerator();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      #endregion

      #region IEnumerable Members

      [CoverageExclude]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
