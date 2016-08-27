
using System;

using HFM.Core;

namespace HFM.Preferences
{
   internal static class Validation
   {
      private const int IntervalDefault = 15;

      internal static int GetValidInterval(string value)
      {
         int result;
         if (!Int32.TryParse(value, out result))
         {
            return IntervalDefault;
         }
         return GetValidInterval(result);
      }

      internal static int GetValidInterval(int value)
      {
         return !Validate.Minutes(value) ? IntervalDefault : value;
      }

      internal static int GetValidMessageLevel(int level)
      {
         if (level < 4)
         {
            level = 4;
         }
         else if (level > 5)
         {
            level = 5;
         }
         return level;
      }
   }
}
