
using System;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HFM.Core.WorkUnits
{
    [DataContract]
    [DebuggerDisplay("{Duration}")]
    public sealed class ProteinBenchmarkFrameTime
    {
        public TimeSpan Duration { get; set; }

        [DataMember(Order = 1)]
        public long DurationTicks
        {
            get => Duration.Ticks;
            set => Duration = new TimeSpan(value);
        }
    }
}
