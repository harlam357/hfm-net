
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   public static class UnitInfoExtensions
   {
      public static void AddWorkUnitFrame(this UnitInfo unitInfo, UnitRunFrameData frameData)
      {
         var logLines = unitInfo.LogLines ?? new List<LogLine>();
         logLines.Add(new LogLine { LineType = LogLineType.WorkUnitFrame, Data = frameData });
         unitInfo.LogLines = logLines;
      }
   }
}
