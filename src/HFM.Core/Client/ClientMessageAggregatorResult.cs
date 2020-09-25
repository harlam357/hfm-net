using System.Collections.Generic;

using HFM.Core.WorkUnits;

namespace HFM.Core.Client
{
    internal sealed class ClientMessageAggregatorResult
    {
        // Queue Values
        public int CurrentUnitIndex { get; set; }

        public WorkUnitQueue WorkUnitQueue { get; set; }

        public IDictionary<int, WorkUnit> WorkUnits { get; set; }
    }
}
