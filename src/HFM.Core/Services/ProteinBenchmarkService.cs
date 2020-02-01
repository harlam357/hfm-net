/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.Linq;
using System.Threading;

using HFM.Core.Client;
using HFM.Core.Data;
using HFM.Core.DataTypes;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core
{
   public interface IProteinBenchmarkService
   {
      /// <summary>
      /// List of slot identifier objects.
      /// </summary>
      ICollection<ProteinBenchmarkSlotIdentifier> SlotIdentifiers { get; }

      void UpdateData(UnitInfo unit, int startingFrame, int endingFrame);

      /// <summary>
      /// Gets the ProteinBenchmark based on the UnitInfo owner and project data.
      /// </summary>
      /// <param name="unitInfo">The UnitInfo containing owner and project data.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="unitInfo"/> is null.</exception>
      ProteinBenchmark GetBenchmark(UnitInfo unitInfo);

      /// <summary>
      /// Removes all the elements from the ProteinBenchmarkCollection that match the slot identifier.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to remove from the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="slotIdentifier"/> represents all clients.</exception>
      void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier);

      /// <summary>
      /// Removes all the elements from the ProteinBenchmarkCollection that match the slot identifier and projectId.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to remove from the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId);

      /// <summary>
      /// Determines whether the ProteinBenchmarkCollection contains a specific value.
      /// </summary>
      /// <returns>
      /// true if <paramref name="slotIdentifier"/> is found in the ProteinBenchmarkCollection; otherwise, false.
      /// </returns>
      /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      bool Contains(ProteinBenchmarkSlotIdentifier slotIdentifier);

      /// <summary>
      /// Gets a list of benchmark project numbers.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      ICollection<Int32> GetBenchmarkProjects(ProteinBenchmarkSlotIdentifier slotIdentifier);

      /// <summary>
      /// Gets a list of ProteinBenchmark objects.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier);

      /// <summary>
      /// Gets a list of ProteinBenchmark objects.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier, Int32 projectId);

      /// <summary>
      /// Updates the owner name of all the elements in ProteinBenchmarkCollection that match the given client name and path.
      /// </summary>
      void UpdateOwnerName(string clientName, string clientPath, string newName);

      /// <summary>
      /// Updates the owner path of all the elements in ProteinBenchmarkCollection that match the given client name and path.
      /// </summary>
      void UpdateOwnerPath(string clientName, string clientPath, string newPath);

      /// <summary>
      /// Updates the minimum frame time of all the elements in ProteinBenchmarkCollection that match the slot identifier and projectId.
      /// </summary>
      /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
      /// <param name="projectId">The Folding@Home project number.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
      void UpdateMinimumFrameTime(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId);

      /// <summary>
      /// Adds a ProteinBenchmark to the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <param name="item">The ProteinBenchmark to add to the <see cref="T:System.Collections.Generic.ICollection`1"/>.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="item"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">The <paramref name="item"/> already exists in the <see cref="T:System.Collections.Generic.ICollection`1"/>.</exception>
      void Add(ProteinBenchmark item);

      ICollection<ProteinBenchmark> GetAll();
   }

   public sealed class ProteinBenchmarkService : DataContainer<List<ProteinBenchmark>>, IProteinBenchmarkService
   {
      #region Properties

      public override Serializers.IFileSerializer<List<ProteinBenchmark>> DefaultSerializer
      {
         get { return new Serializers.ProtoBufFileSerializer<List<ProteinBenchmark>>(); }
      }

      public ICollection<ProteinBenchmarkSlotIdentifier> SlotIdentifiers
      {
         get
         {
            var slotIdentifiers = Data.Select(x => x.ToSlotIdentifier()).Distinct().ToList();
            slotIdentifiers.Add(new ProteinBenchmarkSlotIdentifier());
            slotIdentifiers.Sort();
            return slotIdentifiers.AsReadOnly();
         }
      }

      #endregion

      private readonly ReaderWriterLockSlim _cacheLock;

      #region Constructor

      public ProteinBenchmarkService()
         : this(null)
      {
         
      } 

      public ProteinBenchmarkService(IPreferenceSet prefs)
      {
         var path = prefs != null ? prefs.Get<string>(Preference.ApplicationDataFolderPath) : null;
         if (!String.IsNullOrEmpty(path))
         {
            FileName = System.IO.Path.Combine(path, Constants.BenchmarkCacheFileName);
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
            WorkUnitFrameData frameData = unit.GetFrameData(i);
            if (frameData != null)
            {
               if (benchmark.SetFrameDuration(frameData.Duration))
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
            return Data.Find(benchmark => Equals(benchmark, unitInfo));
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      private static bool Equals(ProteinBenchmark benchmark, UnitInfo unifInfo)
      {
         return benchmark.OwningSlotName == unifInfo.OwningSlotName &&
                FileSystemPath.Equals(benchmark.OwningClientPath, unifInfo.OwningClientPath) &&
                benchmark.ProjectID == unifInfo.ProjectID;
      }

      public void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");
         if (slotIdentifier.AllSlots) throw new ArgumentException("Cannot remove all client slots.");

         _cacheLock.EnterWriteLock();
         try
         {
            Data.RemoveAll(benchmark => benchmark.ToSlotIdentifier().Equals(slotIdentifier));
            Write();
         }
         finally
         {
            _cacheLock.ExitWriteLock();
         }
      }

      public void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

         _cacheLock.EnterWriteLock();
         try
         {
            Data.RemoveAll(benchmark =>
                           {
                              if (slotIdentifier.AllSlots)
                              {
                                 return benchmark.ProjectID.Equals(projectId);
                              }
                              if (benchmark.ToSlotIdentifier().Equals(slotIdentifier))
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

      public bool Contains(ProteinBenchmarkSlotIdentifier slotIdentifier)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

         _cacheLock.EnterReadLock();
         try
         {
            return Data.Find(benchmark =>
                             {
                                if (slotIdentifier.AllSlots)
                                {
                                   return true;
                                }
                                if (benchmark.ToSlotIdentifier().Equals(slotIdentifier))
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

      public ICollection<Int32> GetBenchmarkProjects(ProteinBenchmarkSlotIdentifier slotIdentifier)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

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

               if (slotIdentifier.AllSlots)
               {
                  projects.Add(benchmark.ProjectID);
               }
               else
               {
                  if (benchmark.ToSlotIdentifier().Equals(slotIdentifier))
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

      public ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

         _cacheLock.EnterReadLock();
         try
         {
            var list = Data.FindAll(benchmark =>
                                    {
                                       if (slotIdentifier.AllSlots)
                                       {
                                          return true;
                                       }
                                       return benchmark.ToSlotIdentifier().Equals(slotIdentifier);
                                    });

            return list.AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier, Int32 projectId)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

         _cacheLock.EnterReadLock();
         try
         {
            var list = Data.FindAll(benchmark =>
                                    {
                                       if (slotIdentifier.AllSlots)
                                       {
                                          return benchmark.ProjectID.Equals(projectId);
                                       }
                                       if (benchmark.ToSlotIdentifier().Equals(slotIdentifier))
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
         Debug.Assert(ClientSettings.ValidateName(newName));

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
                                             FileSystemPath.Equals(benchmark.OwningClientPath, clientPath)).AsReadOnly();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }

      public void UpdateMinimumFrameTime(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId)
      {
         if (slotIdentifier == null) throw new ArgumentNullException("slotIdentifier");

         // GetBenchmarks() BEFORE entering write lock 
         // because it uses a read lock
         IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(slotIdentifier, projectId);
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

      public ICollection<ProteinBenchmark> GetAll()
      {
         _cacheLock.EnterReadLock();
         try
         {
            return Data.ToList();
         }
         finally
         {
            _cacheLock.ExitReadLock();
         }
      }
   }
}
