
using System;

using HFM.Core;

namespace HFM.Preferences
{
   internal static class Validation
   {
      private const int MinutesDefault = 15;

      internal static int GetValidInternal(string value)
      {
         int result;
         if (!Int32.TryParse(value, out result))
         {
            return MinutesDefault;
         }
         return GetValidInternal(result);
      }

      internal static int GetValidInternal(int value)
      {
         return !Validate.Minutes(value) ? MinutesDefault : value;
      }
   }
}
