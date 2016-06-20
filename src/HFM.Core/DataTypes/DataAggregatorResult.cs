
using System.Collections.Generic;

using HFM.Log;

namespace HFM.Core.DataTypes
{
   internal sealed class DataAggregatorResult
   {
      public ClientQueue Queue { get; set; }

      public int CurrentUnitIndex { get; set; }

      public ClientRun CurrentClientRun { get; set; }

      public IList<LogLine> CurrentLogLines { get; set; }

      public IDictionary<int, IList<LogLine>> UnitLogLines { get; set; }

      public IDictionary<int, UnitInfo> UnitInfos { get; set; }
   }
}