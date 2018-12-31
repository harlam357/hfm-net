
using System.Collections.Generic;

using HFM.Log;

namespace HFM.Core
{
   internal static class UnitInfoExtensions
   {
      internal static IDictionary<int, WorkUnitFrameData> With(this IDictionary<int, WorkUnitFrameData> frameDataDictionary, params WorkUnitFrameData[] frameDataCollection)
      {
         foreach (var frameData in frameDataCollection)
         {
            frameDataDictionary.Add(frameData.ID, frameData);
         }
         return frameDataDictionary;
      }
   }
}
