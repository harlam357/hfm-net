using System.Runtime.Serialization;

namespace HFM.Core.SlotXml
{
    [DataContract(Namespace = "")]
    public class Protein
    {
        [DataMember(Order = 1)]
        public int ProjectNumber { get; set; }

        [DataMember(Order = 2)]
        public string ServerIP { get; set; }

        [DataMember(Order = 3)]
        public string WorkUnitName { get; set; }

        [DataMember(Order = 4)]
        public int NumberOfAtoms { get; set; }

        [DataMember(Order = 5)]
        public double PreferredDays { get; set; }

        [DataMember(Order = 6)]
        public double MaximumDays { get; set; }

        [DataMember(Order = 7)]
        public double Credit { get; set; }

        [DataMember(Order = 8)]
        public int Frames { get; set; }

        [DataMember(Order = 9)]
        public string Core { get; set; }

        [DataMember(Order = 10)]
        public string Description { get; set; }

        [DataMember(Order = 11)]
        public string Contact { get; set; }

        [DataMember(Order = 12)]
        public double KFactor { get; set; }
    }
}
