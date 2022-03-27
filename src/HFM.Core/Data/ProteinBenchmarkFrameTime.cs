using System.Diagnostics;

namespace HFM.Core.Data;

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
}
