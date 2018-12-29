
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   internal static class UnitInfoExtensions
   {
      internal static void AddLogLineWithFrameData(this UnitInfo unitInfo, UnitRunFrameData frameData)
      {
         if (unitInfo.LogLines == null)
         {
            unitInfo.LogLines = new List<LogLine>();
         }
         unitInfo.LogLines.Add(new LogLine { LineType = LogLineType.WorkUnitFrame, Data = frameData });
      }
   }
}
