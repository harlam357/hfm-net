
using System;

namespace HFM.Framework.DataTypes
{
   public static class Default
   {
      // Folding ID and Team Defaults
      public const string FoldingIDDefault = "Unknown";
      public const int TeamDefault = 0;
      public const string CoreIDDefault = "Unknown";

      public const int MaxDecimalPlaces = 5;

      private static bool IsRunningOnMono()
      {
         return Type.GetType("Mono.Runtime") != null;
      }

      /// <summary>
      /// Get the DateTimeStyle for the given Client Instance.
      /// </summary>
      public static System.Globalization.DateTimeStyles DateTimeStyle
      {
         get
         {
            System.Globalization.DateTimeStyles style;

            if (IsRunningOnMono())
            {
               style = System.Globalization.DateTimeStyles.AssumeUniversal |
                       System.Globalization.DateTimeStyles.AdjustToUniversal;
            }
            else
            {
               // set parse style to parse local
               style = System.Globalization.DateTimeStyles.NoCurrentDateDefault |
                       System.Globalization.DateTimeStyles.AssumeUniversal |
                       System.Globalization.DateTimeStyles.AdjustToUniversal;
            }

            return style;
         }
      }
   }
}
