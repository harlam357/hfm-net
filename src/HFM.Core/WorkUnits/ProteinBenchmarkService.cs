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

using HFM.Core.Data;
using HFM.Log;

namespace HFM.Core.WorkUnits
{
    public interface IProteinBenchmarkService
    {
        /// <summary>
        /// List of slot identifier objects.
        /// </summary>
        ICollection<ProteinBenchmarkSlotIdentifier> SlotIdentifiers { get; }

        void UpdateData(WorkUnit workUnit, int startingFrame, int endingFrame);

        /// <summary>
        /// Gets the ProteinBenchmark based on the WorkUnit owner and project data.
        /// </summary>
        /// <param name="workUnit">The WorkUnit containing owner and project data.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="workUnit"/> is null.</exception>
        ProteinBenchmark GetBenchmark(WorkUnit workUnit);

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
        /// Gets a list of benchmark project numbers.
        /// </summary>
        /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
        ICollection<int> GetBenchmarkProjects(ProteinBenchmarkSlotIdentifier slotIdentifier);

        /// <summary>
        /// Gets a list of ProteinBenchmark objects.
        /// </summary>
        /// <param name="slotIdentifier">The slot identifier to locate in the ProteinBenchmarkCollection.</param>
        /// <param name="projectId">The Folding@Home project number.</param>
        /// <exception cref="T:System.ArgumentNullException"><paramref name="slotIdentifier"/> is null.</exception>
        ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId);

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
    }

    public class ProteinBenchmarkService : IProteinBenchmarkService
    {
        public ICollection<ProteinBenchmarkSlotIdentifier> SlotIdentifiers
        {
            get
            {
                var slotIdentifiers = DataContainer.Data.Select(x => x.ToSlotIdentifier()).Distinct().ToList();
                slotIdentifiers.Add(new ProteinBenchmarkSlotIdentifier());
                slotIdentifiers.Sort();
                return slotIdentifiers.AsReadOnly();
            }
        }

        public ProteinBenchmarkDataContainer DataContainer { get; }
        private readonly ReaderWriterLockSlim _cacheLock;

        public ProteinBenchmarkService(ProteinBenchmarkDataContainer dataContainer)
        {
            DataContainer = dataContainer;
            _cacheLock = new ReaderWriterLockSlim();
        }

        #region Implementation

        public void UpdateData(WorkUnit workUnit, int startingFrame, int endingFrame)
        {
            if (workUnit == null) throw new ArgumentNullException(nameof(workUnit));

            // project is not known, don't add to benchmark data
            if (workUnit.ProjectIsUnknown()) return;

            // no progress has been made so stub out
            if (startingFrame > endingFrame) return;

            // GetBenchmark() BEFORE entering write lock because it uses a read lock
            ProteinBenchmark findBenchmark = GetBenchmark(workUnit);
            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                if (findBenchmark == null)
                {
                    var newBenchmark = new ProteinBenchmark
                    {
                        OwningClientName = workUnit.OwningClientName,
                        OwningClientPath = workUnit.OwningClientPath,
                        OwningSlotId = workUnit.OwningSlotId,
                        ProjectID = workUnit.ProjectID
                    };

                    if (UpdateFrames(workUnit, startingFrame, endingFrame, newBenchmark))
                    {
                        DataContainer.Data.Add(newBenchmark);
                    }
                }
                else
                {
                    UpdateFrames(workUnit, startingFrame, endingFrame, findBenchmark);
                }
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        private bool UpdateFrames(WorkUnit workUnit, int startingFrame, int endingFrame, ProteinBenchmark benchmark)
        {
            bool result = false;

            for (int i = startingFrame; i <= endingFrame; i++)
            {
                WorkUnitFrameData frameData = workUnit.GetFrameData(i);
                if (frameData != null)
                {
                    if (benchmark.SetFrameDuration(frameData.Duration))
                    {
                        result = true;
                    }
                }
            }

            return result;
        }

        public ProteinBenchmark GetBenchmark(WorkUnit workUnit)
        {
            if (workUnit == null) throw new ArgumentNullException(nameof(workUnit));

            _cacheLock.EnterReadLock();
            try
            {
                return DataContainer.Data.Find(benchmark => Equals(benchmark, workUnit));
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        private static bool Equals(ProteinBenchmark benchmark, WorkUnit workUnit)
        {
            return benchmark.OwningSlotName == workUnit.OwningSlotName &&
                   FileSystemPath.Equals(benchmark.OwningClientPath, workUnit.OwningClientPath) &&
                   benchmark.ProjectID == workUnit.ProjectID;
        }

        public void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier)
        {
            if (slotIdentifier == null) throw new ArgumentNullException(nameof(slotIdentifier));
            if (slotIdentifier.AllSlots) throw new ArgumentException("Cannot remove all client slots.");

            _cacheLock.EnterWriteLock();
            try
            {
                DataContainer.Data.RemoveAll(benchmark => benchmark.ToSlotIdentifier().Equals(slotIdentifier));
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public void RemoveAll(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId)
        {
            if (slotIdentifier == null) throw new ArgumentNullException(nameof(slotIdentifier));

            _cacheLock.EnterWriteLock();
            try
            {
                DataContainer.Data.RemoveAll(benchmark =>
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
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public ICollection<int> GetBenchmarkProjects(ProteinBenchmarkSlotIdentifier slotIdentifier)
        {
            if (slotIdentifier == null) throw new ArgumentNullException(nameof(slotIdentifier));

            _cacheLock.EnterReadLock();
            try
            {
                var projects = new List<int>();
                foreach (var benchmark in DataContainer.Data)
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

        public ICollection<ProteinBenchmark> GetBenchmarks(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId)
        {
            if (slotIdentifier == null) throw new ArgumentNullException(nameof(slotIdentifier));

            _cacheLock.EnterReadLock();
            try
            {
                var list = DataContainer.Data.FindAll(benchmark =>
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
            if (clientName == null) throw new ArgumentNullException(nameof(clientName));
            if (clientPath == null) throw new ArgumentNullException(nameof(clientPath));
            if (newName == null) throw new ArgumentNullException(nameof(newName));

            // Core library - should have a valid client name 
            Debug.Assert(DataTypes.ClientSettings.ValidateName(newName));

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
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public void UpdateOwnerPath(string clientName, string clientPath, string newPath)
        {
            if (clientName == null) throw new ArgumentNullException(nameof(clientName));
            if (clientPath == null) throw new ArgumentNullException(nameof(clientPath));
            if (newPath == null) throw new ArgumentNullException(nameof(newPath));

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
                DataContainer.Write();
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
                return DataContainer.Data.FindAll(benchmark => benchmark.OwningClientName.Equals(clientName) &&
                                                                FileSystemPath.Equals(benchmark.OwningClientPath, clientPath)).AsReadOnly();
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public void UpdateMinimumFrameTime(ProteinBenchmarkSlotIdentifier slotIdentifier, int projectId)
        {
            if (slotIdentifier == null) throw new ArgumentNullException(nameof(slotIdentifier));

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
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        #endregion
    }
}
