
using System.Collections.Generic;

using HFM.Core.DataTypes;
using HFM.Log;

namespace HFM.Core
{
   internal static class UnitInfoExtensions
   {
      internal static void AddLogLineWithFrameData(this UnitInfo unitInfo, WorkUnitFrameData frameData)
      {
         // TODO: This is a weird requirement, but the FrameData dictionary is only updated when the LogLines property is set.
         var logLines = unitInfo.LogLines ?? new List<LogLine>();
         logLines.Add(new LogLine { LineType = LogLineType.WorkUnitFrame, Data = frameData });
         unitInfo.LogLines = logLines;
      }
   }
}
