
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HFM.Core.WorkUnits
{
    [DataContract]
    [DebuggerDisplay("{Duration}")]
    public class ProteinBenchmarkFrameTime
    {
        public ProteinBenchmarkFrameTime()
        {
            
        }

        public ProteinBenchmarkFrameTime(TimeSpan duration)
        {
            Duration = duration;
        }

        public TimeSpan Duration { get; set; }

        [DataMember(Order = 1)]
        public long DurationTicks
        {
            get => Duration.Ticks;
            set => Duration = new TimeSpan(value);
        }

        internal static ProteinBenchmarkFrameTime FromMinutes(double value)
        {
            return new ProteinBenchmarkFrameTime(TimeSpan.FromMinutes(value));
        }
    }
}
