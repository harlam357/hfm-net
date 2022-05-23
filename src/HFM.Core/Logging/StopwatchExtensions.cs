using System.Diagnostics;

namespace HFM.Core.Logging;

public static class StopwatchExtensions
{
    public static string GetExecTime(this Stopwatch sw) => $"{sw.ElapsedMilliseconds:#,##0} ms";
}
