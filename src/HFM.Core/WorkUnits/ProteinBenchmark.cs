using System.Diagnostics;
using System.Runtime.Serialization;

using HFM.Core.Client;

namespace HFM.Core.WorkUnits
{
    [Serializable]
    [DataContract]
    [DebuggerDisplay("{SlotIdentifier} {BenchmarkIdentifier}")]
    public sealed class ProteinBenchmark
    {
        private const int DefaultMaxFrames = 300;

        private static readonly object FrameTimesListLock = new();

        public SlotIdentifier SlotIdentifier => new(ClientIdentifier.FromConnectionString(SourceName, SourcePath, SourceGuid), SourceSlotID);

        /// <summary>
        /// Gets or sets the source client name.
        /// </summary>
        [DataMember(Order = 1)]
        public string SourceName { get; set; }

        /// <summary>
        /// Gets or sets the source client path (physical path or host name and port).
        /// </summary>
        [DataMember(Order = 2)]
        public string SourcePath { get; set; }

        /// <summary>
        /// Gets or sets the source client unique identifier.
        /// </summary>
        [DataMember(Order = 7)]
        public Guid SourceGuid { get; set; }

        /// <summary>
        /// Gets or sets the source client slot ID number.
        /// </summary>
        [DataMember(Order = 6, IsRequired = true)]
        public int SourceSlotID { get; set; }

        public ProteinBenchmarkIdentifier BenchmarkIdentifier => new ProteinBenchmarkIdentifier(ProjectID, Processor, Threads);

        [DataMember(Order = 3)]
        public int ProjectID { get; set; }

        [DataMember(Order = 8)]
        public string Processor { get; set; }

        [DataMember(Order = 9)]
        public int Threads { get; set; }

        [DataMember(Order = 4)]
        public TimeSpan MinimumFrameTime { get; set; }

        public TimeSpan AverageFrameTime
        {
            get
            {
                if (FrameTimes.Count <= 0) return TimeSpan.Zero;

                TimeSpan totalTime = TimeSpan.Zero;
                lock (FrameTimesListLock)
                {
                    totalTime = FrameTimes.Aggregate(totalTime, (current, frameTime) => current.Add(frameTime.Duration));
                }
                // ReSharper disable once PossibleLossOfFraction
                return TimeSpan.FromSeconds(Convert.ToInt32(totalTime.TotalSeconds) / FrameTimes.Count);
            }
        }

        [DataMember(Order = 5)]
        public List<ProteinBenchmarkFrameTime> FrameTimes { get; set; }

        public ProteinBenchmark()
        {
            SourceSlotID = SlotIdentifier.NoSlotID;
            MinimumFrameTime = TimeSpan.Zero;
            FrameTimes = new List<ProteinBenchmarkFrameTime>(DefaultMaxFrames);
        }

        public ProteinBenchmark UpdateFromClientIdentifier(ClientIdentifier clientIdentifier)
        {
            SourceName = clientIdentifier.Name;
            SourcePath = clientIdentifier.ToConnectionString();
            SourceGuid = clientIdentifier.Guid;
            return this;
        }

        public ProteinBenchmark UpdateFromSlotIdentifier(SlotIdentifier slotIdentifier)
        {
            UpdateFromClientIdentifier(slotIdentifier.ClientIdentifier);
            SourceSlotID = slotIdentifier.SlotID;
            return this;
        }

        public ProteinBenchmark UpdateFromBenchmarkIdentifier(ProteinBenchmarkIdentifier benchmarkIdentifier)
        {
            ProjectID = benchmarkIdentifier.ProjectID;
            Processor = benchmarkIdentifier.Processor;
            Threads = benchmarkIdentifier.Threads;
            return this;
        }

        public void AddFrameTime(TimeSpan frameTime)
        {
            if (frameTime <= TimeSpan.Zero) return;

            if (frameTime < MinimumFrameTime || MinimumFrameTime.Equals(TimeSpan.Zero))
            {
                MinimumFrameTime = frameTime;
            }

            lock (FrameTimesListLock)
            {
                // Dequeue once we have the Maximum number of frame times
                if (FrameTimes.Count == DefaultMaxFrames)
                {
                    FrameTimes.RemoveAt(DefaultMaxFrames - 1);
                }
                FrameTimes.Insert(0, new ProteinBenchmarkFrameTime { Duration = frameTime });
            }
        }

        /// <summary>
        /// Updates the <see cref="MinimumFrameTime"/> property value based on the current <see cref="FrameTimes"/> collection.
        /// </summary>
        public void UpdateMinimumFrameTime()
        {
            TimeSpan minimumFrameTime = TimeSpan.Zero;
            lock (FrameTimesListLock)
            {
                foreach (ProteinBenchmarkFrameTime frameTime in FrameTimes)
                {
                    if (frameTime.Duration < minimumFrameTime || minimumFrameTime.Equals(TimeSpan.Zero))
                    {
                        minimumFrameTime = frameTime.Duration;
                    }
                }
            }

            if (minimumFrameTime.Equals(TimeSpan.Zero) == false)
            {
                MinimumFrameTime = minimumFrameTime;
            }
        }
    }
}
