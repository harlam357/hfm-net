using System.Collections.Generic;
using System.Runtime.Serialization;

using HFM.Core.Client;

namespace HFM.Core.SlotXml
{
    [DataContract(Namespace = "")]
    public class SlotData
    {
        // TODO: Status as string
        [DataMember(Order = 1)]
        public SlotStatus Status { get; set; }

        [DataMember(Order = 2)]
        public string StatusColor { get; set; }

        [DataMember(Order = 3)]
        public string StatusFontColor { get; set; }

        [DataMember(Order = 4)]
        public int PercentComplete { get; set; }

        [DataMember(Order = 5)]
        public string Name { get; set; }

        [DataMember(Order = 6)]
        public string SlotType { get; set; }

        [DataMember(Order = 7)]
        public string ClientVersion { get; set; }

        [DataMember(Order = 8)]
        public string TPF { get; set; }

        [DataMember(Order = 9)]
        public double PPD { get; set; }

        [DataMember(Order = 10)]
        public double UPD { get; set; }

        [DataMember(Order = 11)]
        public string ETA { get; set; }

        [DataMember(Order = 12)]
        public string Core { get; set; }

        [DataMember(Order = 13)]
        public string CoreId { get; set; }

        [DataMember(Order = 14)]
        public bool ProjectIsDuplicate { get; set; }

        [DataMember(Order = 15)]
        public string ProjectRunCloneGen { get; set; }

        [DataMember(Order = 16)]
        public double Credit { get; set; }

        [DataMember(Order = 17)]
        public int Completed { get; set; }

        [DataMember(Order = 18)]
        public int Failed { get; set; }

        [DataMember(Order = 19)]
        public int TotalRunCompletedUnits { get; set; }

        [DataMember(Order = 20)]
        public int TotalCompletedUnits { get; set; }

        [DataMember(Order = 21)]
        public int TotalRunFailedUnits { get; set; }

        [DataMember(Order = 22)]
        public int TotalFailedUnits { get; set; }

        [DataMember(Order = 23)]
        public bool UsernameOk { get; set; }

        [DataMember(Order = 24)]
        public string Username { get; set; }

        [DataMember(Order = 25)]
        public string DownloadTime { get; set; }

        [DataMember(Order = 26)]
        public string PreferredDeadline { get; set; }

        [DataMember(Order = 27)]
        public IList<LogLine> CurrentLogLines { get; set; }

        [DataMember(Order = 28)]
        public Protein Protein { get; set; }
    }
}
