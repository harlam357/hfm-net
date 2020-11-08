using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

using HFM.Core.WorkUnits;

namespace HFM.Core.Client
{
    /// <summary>
    /// Represent a work unit assigned to a slot in the client work unit queue.
    /// </summary>
    public class WorkUnitQueueItem : IProjectInfo
    {
        public int ID { get; }

        public WorkUnitQueueItem(int id)
        {
            ID = id;
        }

        public int ProjectID { get; set; }

        public int ProjectRun { get; set; }

        public int ProjectClone { get; set; }

        public int ProjectGen { get; set; }

        /// <summary>
        /// Get or sets the state of this work unit.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Get or sets the action this work unit is waiting to perform.
        /// </summary>
        public string WaitingOn { get; set; }

        /// <summary>
        /// Get or sets the the number of attempts the client has made at the WaitingOn action.
        /// </summary>
        public int Attempts { get; set; }

        /// <summary>
        /// Get or sets the length of time till the next attempt at the WaitingOn action.
        /// </summary>
        public TimeSpan NextAttempt { get; set; }

        /// <summary>
        /// Get or sets the date and time this work unit was assigned in UTC.
        /// </summary>
        public DateTime Assigned { get; set; }

        /// <summary>
        /// Get or sets the work server IP address.
        /// </summary>
        public string WorkServer { get; set; }

        /// <summary>
        /// Gets or sets the name of the CPU or GPU that is processing this work unit.
        /// </summary>
        public string CPU { get; set; }

        /// <summary>
        /// Gets or sets the name of the operating system that is processing this work unit.
        /// </summary>
        public string OperatingSystem { get; set; }

        /// <summary>
        /// Gets or sets the installed memory on the system that is processing this work unit.
        /// </summary>
        public int Memory { get; set; }

        /// <summary>
        /// Gets or sets the number of CPU threads.
        /// </summary>
        public int CPUThreads { get; set; }

        /// <summary>
        /// Get or sets the slot ID where this work unit is assigned.
        /// </summary>
        public int SlotID { get; set; }
    }

    public class WorkUnitQueue : IEnumerable<WorkUnitQueueItem>
    {
        private readonly WorkUnitQueueItemKeyedCollection _inner = new WorkUnitQueueItemKeyedCollection();

        public int DefaultQueueID => _inner.Count > 0 ? _inner.First().ID : NoQueueID;

        internal const int NoQueueID = -1;

        /// <summary>
        /// Gets the current work unit queue ID value.
        /// </summary>
        public int CurrentQueueID { get; set; } = NoQueueID;

        /// <summary>
        /// Gets the work unit at the current index.
        /// </summary>
        public WorkUnitQueueItem Current => _inner.Contains(CurrentQueueID) ? _inner[CurrentQueueID] : null;

        public void Add(WorkUnitQueueItem workUnit) => _inner.Add(workUnit);

        public int Count => _inner.Count;

        public WorkUnitQueueItem this[int id] => _inner.Contains(id) ? _inner[id] : null;

        public IEnumerator<WorkUnitQueueItem> GetEnumerator() => _inner.GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        private class WorkUnitQueueItemKeyedCollection : KeyedCollection<int, WorkUnitQueueItem>
        {
            public WorkUnitQueueItemKeyedCollection() : base(EqualityComparer<int>.Default, 1)
            {

            }

            protected override int GetKeyForItem(WorkUnitQueueItem item) => item.ID;
        }
    }
}
