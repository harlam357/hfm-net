using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data;

public record ProteinBenchmark
{
    public SlotIdentifier SlotIdentifier { get; init; }

    public ProteinBenchmarkIdentifier BenchmarkIdentifier { get; init; }

    public TimeSpan MinimumFrameTime { get; init; }

    public TimeSpan AverageFrameTime
    {
        get
        {
            if (FrameTimes.Count <= 0) return TimeSpan.Zero;

            TimeSpan totalTime = TimeSpan.Zero;
            totalTime = FrameTimes.Aggregate(totalTime, (current, frameTime) => current.Add(frameTime.Duration));
            return TimeSpan.FromSeconds(Convert.ToInt32(totalTime.TotalSeconds) / FrameTimes.Count);
        }
    }

    public IReadOnlyList<ProteinBenchmarkFrameTime> FrameTimes { get; init; }
}
