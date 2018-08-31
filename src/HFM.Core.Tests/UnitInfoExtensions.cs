
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   public static class UnitInfoExtensions
   {
      public static void SetUnitFrame(this UnitInfo unitInfo, UnitFrame unitFrame)
      {
         var logLines = unitInfo.LogLines ?? new List<LogLine>();
         logLines.Add(new LogLine { LineType = LogLineType.WorkUnitFrame, LineData = unitFrame });
         unitInfo.LogLines = logLines;
      }
   }
}
