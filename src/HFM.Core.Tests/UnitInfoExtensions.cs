using System.Collections.Generic;

using HFM.Log;

namespace HFM.Core
{
    internal static class UnitInfoExtensions
    {
        internal static IDictionary<int, LogLineFrameData> With(this IDictionary<int, LogLineFrameData> frames, params LogLineFrameData[] framesToAdd)
        {
            foreach (var f in framesToAdd)
            {
                frames.Add(f.ID, f);
            }
            return frames;
        }
    }
}
