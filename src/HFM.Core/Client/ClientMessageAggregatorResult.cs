﻿using HFM.Core.WorkUnits;

namespace HFM.Core.Client
{
    internal sealed class ClientMessageAggregatorResult
    {
        public WorkUnitQueueItemCollection WorkUnitQueue { get; set; }

        public WorkUnitCollection WorkUnits { get; set; }
    }
}
