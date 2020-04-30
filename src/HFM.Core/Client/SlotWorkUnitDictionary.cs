
using System;
using System.Collections.Generic;

using HFM.Core.WorkUnits;

namespace HFM.Core.Client
{
    /// <summary>
    /// Represent a work unit assigned to a slot in the client work unit queue.
    /// </summary>
    public class SlotWorkUnitInfo : IProjectInfo
    {
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
        public DateTime AssignedDateTimeUtc { get; set; }

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

    public class SlotWorkUnitDictionary : Dictionary<int, SlotWorkUnitInfo>
    {
        /// <summary>
        /// Gets the current work unit key value.
        /// </summary>
        public int CurrentWorkUnitKey { get; set; }

        /// <summary>
        /// Gets the work unit at the current index.
        /// </summary>
        public SlotWorkUnitInfo Current => TryGetValue(CurrentWorkUnitKey, out var info) ? info : null;
    }
}
