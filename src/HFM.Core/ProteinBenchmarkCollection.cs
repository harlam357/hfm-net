/*
 * HFM.NET - Benchmark Collection Class
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
using System.Diagnostics;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IProteinBenchmarkCollection : ICollection<ProteinBenchmark>
   {
      /// <summary>
      /// List of BenchmarkClient objects.
      /// </summary>
      IEnumerable<BenchmarkClient> BenchmarkClients { get; }

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="currentUnitInfo">Current UnitInfo</param>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="nextUnitIndex">Index of Current UnitInfo</param>
      void UpdateData(IUnitInfoLogic currentUnitInfo, IUnitInfoLogic[] parsedUnits, int nextUnitIndex);

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
      /// Updates the owner name of all the elements in ProteinBenchmarkCollection that match the benchmarkClient and projectId.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="name">The new benchmark owner name.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null or <paramref name="name"/> is null.</exception>
      void UpdateOwnerName(BenchmarkClient benchmarkClient, string name);

      /// <summary>
      /// Updates the owner path of all the elements in ProteinBenchmarkCollection that match the benchmarkClient and projectId.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="path">The new benchmark owner path.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null or <paramref name="path"/> is null.</exception>
      void UpdateOwnerPath(BenchmarkClient benchmarkClient, string path);

      /// <summary>
      /// Updates the minimum frame time of all the elements in ProteinBenchmarkCollection that match the benchmarkClient and projectId.
      /// </summary>
      /// <param name="benchmarkClient">The BenchmarkClient to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="benchmarkClient"/> is null.</exception>
      void UpdateMinimumFrameTime(BenchmarkClient benchmarkClient, int projectId);

      #region DataContainer<T>

      void Read();

      void Write();

      #endregion
   }

   public class ProteinBenchmarkCollection : DataContainer<List<ProteinBenchmark>>, IProteinBenchmarkCollection
   {
      #region Fields
      
      private readonly IUnitInfoDatabase _database;

      #endregion

      #region Properties

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

      #region Constructor

      public ProteinBenchmarkCollection(IUnitInfoDatabase database)
      {
         _database = database;
      } 

      #endregion

      #region Implementation

      #region UpdateData

      /// <summary>
      /// Update Project Benchmarks
      /// </summary>
      /// <param name="currentUnitInfo">Current UnitInfo</param>
      /// <param name="parsedUnits">Parsed UnitInfo Array</param>
      /// <param name="nextUnitIndex">Index of Current UnitInfo</param>
      public void UpdateData(IUnitInfoLogic currentUnitInfo, IUnitInfoLogic[] parsedUnits, int nextUnitIndex)
      {
         var foundCurrent = false;
         var processUpdates = false;
         var index = GetStartingIndex(nextUnitIndex, parsedUnits.Length);

         while (index != -1)
         {
            // If Current has not been found, check the nextUnitIndex
            // or try to match the Current Project and Raw Download Time
            if (processUpdates == false && (index == nextUnitIndex || currentUnitInfo.UnitInfoData.Equals(parsedUnits[index].UnitInfoData)))
            {
               foundCurrent = true;
               processUpdates = true;
            }

            if (processUpdates)
            {
               int previousFramesComplete = 0;
               if (foundCurrent)
               {
                  // current frame has already been recorded, increment to the next frame
                  previousFramesComplete = currentUnitInfo.FramesComplete + 1;
                  foundCurrent = false;
               }

               // Even though the CurrentUnitInfo has been found in the parsed UnitInfoLogic array doesn't
               // mean that all entries in the array will be present.  See TestFiles\SMP_12\FAHlog.txt.
               if (parsedUnits[index] != null)
               {
                  // Update benchmarks
                  UpdateData(parsedUnits[index].UnitInfoData, previousFramesComplete, parsedUnits[index].FramesComplete);
                  // Update history database
                  if (_database.ConnectionOk)
                  {
                     try
                     {
                        _database.WriteUnitInfo(parsedUnits[index]);
                     }
                     catch (Exception ex)
                     {
                        Logger.ErrorFormat(ex, "{0}", ex.Message);
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
      
      private static int GetStartingIndex(int nextUnitIndex, int numberOfUnits)
      {
         if (nextUnitIndex == numberOfUnits - 1)
         {
            return 0;
         }

         return nextUnitIndex + 1;
      }
      
      private void UpdateData(UnitInfo unit, int startingFrame, int endingFrame)
      {
         Debug.Assert(unit != null);

         // project is not known, don't add to benchmark data
         if (unit.ProjectIsUnknown()) return;

         // no progress has been made so stub out
         if (startingFrame > endingFrame) return;

         ProteinBenchmark findBenchmark = GetBenchmark(unit);
         if (findBenchmark == null)
         {
            var newBenchmark = new ProteinBenchmark
                               {
                                  OwningInstanceName = unit.OwningInstanceName,
                                  OwningInstancePath = unit.OwningInstancePath,
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
               Logger.DebugFormat("({0}) FrameID '{1}' not found for Project {2}", unit.OwningInstanceName, i, unit.ProjectID);
            }
         }

         return result;
      }

      #endregion

      public ProteinBenchmark GetBenchmark(UnitInfo unitInfo)
      {
         if (unitInfo == null) throw new ArgumentNullException("unitInfo");

         return Data.Find(benchmark => benchmark.Equals(unitInfo));
      }

      public void RemoveAll(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");
         if (benchmarkClient.AllClients) throw new ArgumentException("Cannot remove all clients.");

         Data.RemoveAll(benchmark => benchmark.Client.Equals(benchmarkClient));
         Write();
      }

      public void RemoveAll(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

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

      public bool Contains(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

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

      public IEnumerable<int> GetBenchmarkProjects(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

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

      public IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

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

      public IEnumerable<ProteinBenchmark> GetBenchmarks(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

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

      public void UpdateOwnerName(BenchmarkClient benchmarkClient, string name)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");
         if (name == null) throw new ArgumentNullException("name");
         
         // Core library - should have a valid instance name 
         Debug.Assert(Validate.InstanceName(name));

         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(benchmarkClient);
         foreach (ProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstanceName = name;
         }
         Write();
      }

      public void UpdateOwnerPath(BenchmarkClient benchmarkClient, string path)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");
         if (path == null) throw new ArgumentNullException("path");

         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(benchmarkClient);
         foreach (ProteinBenchmark benchmark in benchmarks)
         {
            benchmark.OwningInstancePath = path;
         }
         Write();
      }

      public void UpdateMinimumFrameTime(BenchmarkClient benchmarkClient, int projectId)
      {
         if (benchmarkClient == null) throw new ArgumentNullException("benchmarkClient");

         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(benchmarkClient, projectId);
         foreach (ProteinBenchmark benchmark in benchmarks)
         {
            benchmark.UpdateMinimumFrameTime();
         }
         Write();
      }
      
      #endregion

      #region ICollection<ProteinBenchmark> Members

      /// <summary>
      /// Adds a ProteinBenchmark to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The ProteinBenchmark to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">The <paramref name="item"/> already exists in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</exception>
      public void Add(ProteinBenchmark item)
      {
         if (item == null) throw new ArgumentNullException("item");
         if (Contains(item)) throw new ArgumentException("The benchmark already exists.", "item");

         Data.Add(item);
      }

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      public void Clear()
      {
         Data.Clear();
      }

      /// <summary>
      /// Determines whether the <see cref="T:System.Collections.Generic.ICollection`1"/> contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> is found in the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false.
      /// </returns>
      /// <param name="item">The ProteinBenchmark to locate in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Contains(ProteinBenchmark item)
      {
         return item != null && Data.Contains(item);
      }

      /// <summary>
      /// Copies the elements of the <see cref="T:System.Collections.Generic.ICollection`1"/> to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from <see cref="T:System.Collections.Generic.ICollection`1"/>. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
      /// <param name="arrayIndex">The zero-based index in <paramref name="array"/> at which copying begins.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="arrayIndex"/> is less than 0.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
      ///     -or-
      ///     <paramref name="arrayIndex"/> is equal to or greater than the length of <paramref name="array"/>.
      ///     -or-
      ///     The number of elements in the source <see cref="T:System.Collections.Generic.ICollection`1"/> is greater than the available space from <paramref name="arrayIndex"/> to the end of the destination <paramref name="array"/>.
      /// </exception>
      void ICollection<ProteinBenchmark>.CopyTo(ProteinBenchmark[] array, int arrayIndex)
      {
         Data.CopyTo(array, arrayIndex);
      }

      /// <summary>
      /// Gets the number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// The number of elements contained in the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      public int Count
      {
         get { return Data.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only.
      /// </summary>
      /// <returns>
      /// true if the <see cref="T:System.Collections.Generic.ICollection`1"/> is read-only; otherwise, false.
      /// </returns>
      bool ICollection<ProteinBenchmark>.IsReadOnly
      {
         get { return false; }
      }

      /// <summary>
      /// Removes the first occurrence of a specific object from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <returns>
      /// true if <paramref name="item"/> was successfully removed from the <see cref="T:System.Collections.Generic.ICollection`1"/>; otherwise, false. This method also returns false if <paramref name="item"/> is not found in the original <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </returns>
      /// <param name="item">The ProteinBenchmark to remove from the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      public bool Remove(ProteinBenchmark item)
      {
         return item != null && Data.Remove(item);
      }

      #endregion

      #region IEnumerable<ProteinBenchmark> Members

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public IEnumerator<ProteinBenchmark> GetEnumerator()
      {
         return Data.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
