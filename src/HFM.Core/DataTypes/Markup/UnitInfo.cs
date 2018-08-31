
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes.Markup
{
   [DataContract(Namespace = "")]
   public class UnitInfo
   {
      [DataMember(Order = 1)]
      public string OwningClientName { get; set; }

      [DataMember(Order = 2)]
      public string OwningClientPath { get; set; }

      [DataMember(Order = 26, IsRequired = true)]
      public int OwningSlotId { get; set; }

      [DataMember(Order = 3)]
      public DateTime UnitRetrievalTime { get; set; }

      [DataMember(Order = 4)]
      public string FoldingID { get; set; }

      [DataMember(Order = 5)]
      public int Team { get; set; }

      [DataMember(Order = 6)]
      public SlotType SlotType { get; set; }

      [DataMember(Order = 7)]
      public DateTime DownloadTime { get; set; }

      [DataMember(Order = 8)]
      public DateTime DueTime { get; set; }

      [DataMember(Order = 9)]
      public TimeSpan UnitStartTimeStamp { get; set; }

      [DataMember(Order = 10)]
      public DateTime FinishedTime { get; set; }

      [DataMember(Order = 11)]
      public float CoreVersion { get; set; }

      [DataMember(Order = 12)]
      public int ProjectID { get; set; }

      [DataMember(Order = 13)]
      public int ProjectRun { get; set; }

      [DataMember(Order = 14)]
      public int ProjectClone { get; set; }

      [DataMember(Order = 15)]
      public int ProjectGen { get; set; }

      [DataMember(Order = 16)]
      public string ProteinName { get; set; }

      [DataMember(Order = 17)]
      public string ProteinTag { get; set; }

      [DataMember(Order = 18)]
      public WorkUnitResult UnitResult { get; set; }

      [DataMember(Order = 19)]
      public int RawFramesComplete { get; set; }

      [DataMember(Order = 20)]
      public int RawFramesTotal { get; set; }

      [DataMember(Order = 21)]
      public int FramesObserved { get; set; }

      // Open DataMember - was of type UnitFrame
      //[DataMember(Order = 22)]

      [DataMember(Order = 23)]
      public IList<LogLine> LogLines { get; set; }

      [DataMember(Order = 24)]
      public string CoreID { get; set; }

      [DataMember(Order = 25)]
      public int QueueIndex { get; set; }
   }
}
