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

namespace HFM.Core.WorkUnits
{
    public interface IProteinBenchmarkService
    {
        ICollection<SlotIdentifier> GetSlotIdentifiers();

        void Update(SlotIdentifier slotIdentifier, int projectID, IEnumerable<TimeSpan> frameTimes);

        ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, int projectID);

        void RemoveAll(SlotIdentifier slotIdentifier);

        void RemoveAll(SlotIdentifier slotIdentifier, int projectID);

        ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier);

        ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID);

        void UpdateOwnerName(string clientName, string clientPath, string newName);

        void UpdateOwnerPath(string clientName, string clientPath, string newPath);

        void UpdateMinimumFrameTime(SlotIdentifier slotIdentifier, int projectID);
    }

    public class ProteinBenchmarkService : IProteinBenchmarkService
    {
        public ProteinBenchmarkDataContainer DataContainer { get; }
        private readonly ReaderWriterLockSlim _cacheLock;

        public ProteinBenchmarkService(ProteinBenchmarkDataContainer dataContainer)
        {
            DataContainer = dataContainer;
            _cacheLock = new ReaderWriterLockSlim();
        }

        public ICollection<SlotIdentifier> GetSlotIdentifiers()
        {
            return Enumerable.Repeat(SlotIdentifier.AllSlots, 1)
                .Concat(DataContainer.Data.Select(x => x.SlotIdentifier).Distinct())
                .OrderBy(s => s)
                .ToList();
        }

        public void Update(SlotIdentifier slotIdentifier, int projectID, IEnumerable<TimeSpan> frameTimes)
        {
            // GetBenchmark() BEFORE entering write lock because it uses a read lock
            ProteinBenchmark benchmark = GetBenchmark(slotIdentifier, projectID);
            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                if (benchmark == null)
                {
                    benchmark = ProteinBenchmark.FromSlotIdentifier(slotIdentifier);
                    benchmark.ProjectID = projectID;
                    DataContainer.Data.Add(benchmark);
                }
                foreach (var f in frameTimes)
                {
                    benchmark.SetFrameDuration(f);
                }
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        // TODO: GetBenchmark or GetBenchmarks, one of the other, doesn't make sense to have both
        public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, int projectID)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return DataContainer.Data.Find(b => MatchSlotAndProject(b, slotIdentifier, projectID));
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public void RemoveAll(SlotIdentifier slotIdentifier)
        {
            if (slotIdentifier == SlotIdentifier.AllSlots) throw new ArgumentException("Cannot remove all client slots.");

            _cacheLock.EnterWriteLock();
            try
            {
                DataContainer.Data.RemoveAll(b => b.SlotIdentifier.Equals(slotIdentifier));
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public void RemoveAll(SlotIdentifier slotIdentifier, int projectID)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                DataContainer.Data.RemoveAll(b => MatchSlotAndProject(b, slotIdentifier, projectID));
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier)
        {
            _cacheLock.EnterReadLock();
            try
            {
                IEnumerable<ProteinBenchmark> benchmarkQuery = DataContainer.Data;
                if (SlotIdentifier.AllSlots != slotIdentifier)
                {
                    benchmarkQuery = benchmarkQuery.Where(b => b.SlotIdentifier.Equals(slotIdentifier));
                }
                return benchmarkQuery.Select(b => b.ProjectID).Distinct().OrderBy(p => p).ToList();
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        // TODO: GetBenchmark or GetBenchmarks, one of the other, doesn't make sense to have both
        public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return DataContainer.Data.FindAll(b => MatchSlotAndProject(b, slotIdentifier, projectID));
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        private static bool MatchSlotAndProject(ProteinBenchmark b, SlotIdentifier slotIdentifier, int projectID)
        {
            return b.ProjectID.Equals(projectID) && (SlotIdentifier.AllSlots == slotIdentifier || b.SlotIdentifier.Equals(slotIdentifier));
        }

        public void UpdateOwnerName(string clientName, string clientPath, string newName)
        {
            if (clientName == null) throw new ArgumentNullException(nameof(clientName));
            if (clientPath == null) throw new ArgumentNullException(nameof(clientPath));
            if (newName == null) throw new ArgumentNullException(nameof(newName));

            // Core library - should have a valid client name 
            Debug.Assert(DataTypes.ClientSettings.ValidateName(newName));

            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                var benchmarks = EnumerateBenchmarksForOwnerUpdate(clientName, clientPath);
                foreach (var b in benchmarks)
                {
                    b.OwningClientName = newName;
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

            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                var benchmarks = EnumerateBenchmarksForOwnerUpdate(clientName, clientPath);
                foreach (var b in benchmarks)
                {
                    b.OwningClientPath = newPath;
                }
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        private IEnumerable<ProteinBenchmark> EnumerateBenchmarksForOwnerUpdate(string clientName, string clientPath)
        {
            return DataContainer.Data.Where(b => b.OwningClientName.Equals(clientName) && FileSystemPath.Equals(b.OwningClientPath, clientPath));
        }

        public void UpdateMinimumFrameTime(SlotIdentifier slotIdentifier, int projectID)
        {
            // GetBenchmarks() BEFORE entering write lock because it uses a read lock
            IEnumerable<ProteinBenchmark> benchmarks = GetBenchmarks(slotIdentifier, projectID);
            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                foreach (var b in benchmarks)
                {
                    b.UpdateMinimumFrameTime();
                }
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }
    }
}
