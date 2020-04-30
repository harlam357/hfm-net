
using System;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Forms
{
    internal static class ProteinBenchmarkIdentifierExtensions
    {
        internal static string ToProcessorAndThreadsString(this ProteinBenchmarkIdentifier identifier, SlotType slotType)
        {
            return identifier.HasProcessor
                ? identifier.HasThreads
                    ? $"{slotType}:{identifier.Threads} / {identifier.Processor}"
                    : $"{slotType} / {identifier.Processor}"
                : String.Empty;
        }
    }
}
