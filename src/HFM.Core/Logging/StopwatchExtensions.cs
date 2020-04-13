
using System.Diagnostics;

namespace HFM.Core.Logging
{
    internal static class StopwatchExtensions
    {
        internal static string GetExecTime(this Stopwatch sw)
        {
            return $"{sw.ElapsedMilliseconds:#,##0} ms";
        }
    }
}
