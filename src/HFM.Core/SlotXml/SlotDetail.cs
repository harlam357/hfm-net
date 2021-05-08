using System;
using System.Runtime.Serialization;

namespace HFM.Core.SlotXml
{
    [DataContract(Namespace = "")]
    public class SlotDetail
    {
        [DataMember(Order = 1)]
        public string HfmVersion { get; set; }

        [DataMember(Order = 2)]
        public string NumberFormat { get; set; }

        [DataMember(Order = 3)]
        public DateTime UpdateDateTime { get; set; }

        [DataMember(Order = 4)]
        public bool LogFileAvailable { get; set; }

        [DataMember(Order = 5)]
        public string LogFileName { get; set; }

        [DataMember(Order = 6)]
        public int TotalRunCompletedUnits { get; set; }

        [DataMember(Order = 7)]
        public int TotalCompletedUnits { get; set; }

        [DataMember(Order = 8)]
        public int TotalRunFailedUnits { get; set; }

        [DataMember(Order = 9)]
        public int TotalFailedUnits { get; set; }

        [DataMember(Order = 10)]
        public SlotData SlotData { get; set; }
    }
}
