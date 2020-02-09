
using System.Diagnostics;

// ReSharper disable once CheckNamespace
namespace HFM.Core
{
    internal static class StopwatchExtensions
    {
        internal static string GetExecTime(this Stopwatch sw)
        {
            return $"{sw.ElapsedMilliseconds:#,##0} ms";
        }
    }
}
