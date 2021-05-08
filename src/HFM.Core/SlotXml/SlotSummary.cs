using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

using HFM.Core.Client;

namespace HFM.Core.SlotXml
{
    [DataContract(Namespace = "")]
    public class SlotSummary
    {
        [DataMember(Order = 1)]
        public string HfmVersion { get; set; }

        [DataMember(Order = 2)]
        public string NumberFormat { get; set; }

        [DataMember(Order = 3)]
        public DateTime UpdateDateTime { get; set; }

        [DataMember(Order = 4)]
        public SlotTotals SlotTotals { get; set; }

        [DataMember(Order = 5)]
        public List<SlotData> Slots { get; set; }
    }
}
