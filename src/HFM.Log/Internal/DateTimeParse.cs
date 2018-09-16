
using System.Globalization;

namespace HFM.Log.Internal
{
   internal static class DateTimeParse
   {
      /// <summary>
      /// Gets the DateTimeStyles value for DateTime parsing operations based on the current runtime (Mono or other).
      /// </summary>
      internal static DateTimeStyles Styles
      {
         get
         {
            DateTimeStyles style;

            if (Environment.IsMonoRuntime)
            {
               style = DateTimeStyles.AssumeUniversal |
                       DateTimeStyles.AdjustToUniversal;
            }
            else
            {
               // set parse style to parse local
               style = DateTimeStyles.NoCurrentDateDefault |
                       DateTimeStyles.AssumeUniversal |
                       DateTimeStyles.AdjustToUniversal;
            }

            return style;
         }
      }
   }
}
