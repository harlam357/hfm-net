
using System;
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   internal sealed class DataAggregatorResult
   {
      // ClientRun Values
      public DateTime StartTime { get; set; }

      public string Arguments { get; set; }

      public string ClientVersion { get; set; }

      public string UserID { get; set; }

      public int MachineID { get; set; }

      // SlotRun Values
      public SlotStatus Status { get; set; }

      public IList<LogLine> CurrentLogLines { get; set; }

      // Queue Values
      public int CurrentUnitIndex { get; set; }

      public QueueDictionary Queue { get; set; }

      public IDictionary<int, UnitInfo> UnitInfos { get; set; }
   }
}
