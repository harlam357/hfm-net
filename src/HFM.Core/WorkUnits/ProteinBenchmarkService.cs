
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

using HFM.Core.Client;
using HFM.Core.Data;

namespace HFM.Core.WorkUnits
{
    public interface IProteinBenchmarkService
    {
        ICollection<ClientIdentifier> GetClientIdentifiers();

        ICollection<SlotIdentifier> GetSlotIdentifiers();

        ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier);

        ProteinBenchmark Update(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier, IEnumerable<TimeSpan> frameTimes);

        ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier);

        ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID);

        void RemoveAll(SlotIdentifier slotIdentifier);

        void RemoveAll(SlotIdentifier slotIdentifier, int projectID);

        void UpdateMinimumFrameTime(SlotIdentifier slotIdentifier, int projectID);

        void UpdateClientIdentifier(ClientIdentifier clientIdentifier);
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

        public ICollection<ClientIdentifier> GetClientIdentifiers()
        {
            return DataContainer.Data.Select(x => x.SlotIdentifier.ClientIdentifier)
                .OrderBy(c => c)
                .Distinct(ClientIdentifier.ProteinBenchmarkEqualityComparer)
                .ToList();
        }

        public ICollection<SlotIdentifier> GetSlotIdentifiers()
        {
            return DataContainer.Data.Select(x => x.SlotIdentifier)
                .OrderBy(s => s)
                .Distinct(SlotIdentifier.ProteinBenchmarkEqualityComparer)
                .ToList();
        }

        public ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return DataContainer.Data.Where(b => BenchmarkMatchesSlotIdentifier(b, slotIdentifier))
                    .Select(b => b.ProjectID).Distinct().OrderBy(p => p).ToList();
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        public ProteinBenchmark Update(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier, IEnumerable<TimeSpan> frameTimes)
        {
            if (slotIdentifier == SlotIdentifier.AllSlots) throw new ArgumentException("Cannot update all client slots.");
            if (frameTimes == null) throw new ArgumentNullException(nameof(frameTimes));

            // GetBenchmark() BEFORE entering write lock because it uses a read lock
            var benchmark = GetBenchmark(slotIdentifier, benchmarkIdentifier);
            // write lock
            _cacheLock.EnterWriteLock();
            try
            {
                if (benchmark is null)
                {
                    benchmark = new ProteinBenchmark();
                    DataContainer.Data.Add(benchmark);
                }
                benchmark
                    .UpdateFromSlotIdentifier(slotIdentifier)
                    .UpdateFromBenchmarkIdentifier(benchmarkIdentifier);

                foreach (var f in frameTimes)
                {
                    benchmark.AddFrameTime(f);
                }
                DataContainer.Write();

                return benchmark;
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
        {
            _cacheLock.EnterReadLock();
            try
            {
                var benchmarks = DataContainer.Data.FindAll(b => BenchmarkMatchesSlotIdentifier(b, slotIdentifier));
                return GetBenchmark(benchmarks, benchmarkIdentifier);
            }
            finally
            {
                _cacheLock.ExitReadLock();
            }
        }

        private static ProteinBenchmark GetBenchmark(List<ProteinBenchmark> benchmarks, ProteinBenchmarkIdentifier benchmarkIdentifier)
        {
            // most specific, matches ProjectID, Processor, and Threads
            var benchmark = benchmarks.Find(b => b.BenchmarkIdentifier.Equals(benchmarkIdentifier));
            if (benchmark is null)
            {
                // less specific, matches only ProjectID... upgrade any existing benchmark for this slot
                benchmarkIdentifier = new ProteinBenchmarkIdentifier(benchmarkIdentifier.ProjectID);
                benchmark = benchmarks.Find(b => b.BenchmarkIdentifier.Equals(benchmarkIdentifier));
            }
            return benchmark;
        }

        public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID)
        {
            _cacheLock.EnterReadLock();
            try
            {
                return DataContainer.Data.FindAll(b => BenchmarkMatchesSlotIdentifierAndProject(b, slotIdentifier, projectID));
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
                DataContainer.Data.RemoveAll(b => BenchmarkMatchesSlotIdentifier(b, slotIdentifier));
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
                DataContainer.Data.RemoveAll(b => BenchmarkMatchesSlotIdentifierAndProject(b, slotIdentifier, projectID));
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        private static bool BenchmarkMatchesSlotIdentifierAndProject(ProteinBenchmark b, SlotIdentifier slotIdentifier, int projectID)
        {
            return b.ProjectID.Equals(projectID) && BenchmarkMatchesSlotIdentifier(b, slotIdentifier);
        }

        private static bool BenchmarkMatchesSlotIdentifier(ProteinBenchmark b, SlotIdentifier slotIdentifier)
        {
            // when AllSlots is given, then all SlotIdentifiers match
            if (SlotIdentifier.AllSlots.Equals(slotIdentifier)) return true;

            // most specific, matches ClientIdentifier on Guid first... then Name, Server, and Port
            if (b.SlotIdentifier.Equals(slotIdentifier)) return true;

            // less specific, matches ClientIdentifier only on Name, Server, and Port
            if (SlotIdentifier.ProteinBenchmarkEqualityComparer.Equals(b.SlotIdentifier, slotIdentifier)) return true;

            return false;
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

        public void UpdateClientIdentifier(ClientIdentifier clientIdentifier)
        {
            _cacheLock.EnterWriteLock();
            try
            {
                var benchmarks = DataContainer.Data.Where(b => BenchmarkMatchesClientIdentifier(b, clientIdentifier));
                foreach (var b in benchmarks)
                {
                    b.UpdateFromClientIdentifier(clientIdentifier);
                }
                DataContainer.Write();
            }
            finally
            {
                _cacheLock.ExitWriteLock();
            }
        }

        private static bool BenchmarkMatchesClientIdentifier(ProteinBenchmark b, ClientIdentifier clientIdentifier)
        {
            // most specific, matches ClientIdentifier on Guid first... then Name, Server, and Port
            if (b.SlotIdentifier.ClientIdentifier.Equals(clientIdentifier)) return true;

            // less specific, matches ClientIdentifier only on Name, Server, and Port
            if (ClientIdentifier.ProteinBenchmarkEqualityComparer.Equals(b.SlotIdentifier.ClientIdentifier, clientIdentifier)) return true;

            return false;
        }
    }

    public class NullProteinBenchmarkService : IProteinBenchmarkService
    {
        public static NullProteinBenchmarkService Instance { get; } = new NullProteinBenchmarkService();

        public ICollection<ClientIdentifier> GetClientIdentifiers()
        {
            return new List<ClientIdentifier>();
        }

        public ICollection<SlotIdentifier> GetSlotIdentifiers()
        {
            return new List<SlotIdentifier>();
        }

        public ICollection<int> GetBenchmarkProjects(SlotIdentifier slotIdentifier)
        {
            return new List<int>();
        }

        public ProteinBenchmark Update(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier, IEnumerable<TimeSpan> frameTimes)
        {
            return null;
        }

        public ProteinBenchmark GetBenchmark(SlotIdentifier slotIdentifier, ProteinBenchmarkIdentifier benchmarkIdentifier)
        {
            return null;
        }

        public ICollection<ProteinBenchmark> GetBenchmarks(SlotIdentifier slotIdentifier, int projectID)
        {
            return new List<ProteinBenchmark>();
        }

        public void RemoveAll(SlotIdentifier slotIdentifier)
        {

        }

        public void RemoveAll(SlotIdentifier slotIdentifier, int projectID)
        {

        }

        public void UpdateMinimumFrameTime(SlotIdentifier slotIdentifier, int projectID)
        {

        }

        public void UpdateClientIdentifier(ClientIdentifier clientIdentifier)
        {

        }
    }
}
