using System;
using System.Globalization;
using System.Text.RegularExpressions;

using HFM.Log.Internal;

namespace HFM.Log
{
   /// <summary>
   /// Defines a function used to parse time stamp data from a log line.
   /// </summary>
   /// <param name="logLine">The log line to parse.</param>
   /// <returns>A TimeSpan representing the time stamp data parsed from the log line.</returns>
   public delegate TimeSpan? LogLineTimeStampParserDelegate(LogLine logLine);

   public static class LogLineTimeStampParser
   {
      /// <summary>
      /// Gets a delegate representing the default time stamp parsing function.
      /// </summary>
      public static LogLineTimeStampParserDelegate Default { get; } = TryParseTimeStamp;

      private static TimeSpan? TryParseTimeStamp(LogLine logLine)
      {
         Match timeStampMatch;
         if ((timeStampMatch = FahLogRegex.Common.TimeStampRegex.Match(logLine.Raw)).Success)
         {
            string timeStampString = timeStampMatch.Groups["Timestamp"].Value;
            if (DateTime.TryParseExact(timeStampString, "HH:mm:ss",
                                       DateTimeFormatInfo.InvariantInfo,
                                       DateTimeParse.Styles, out DateTime result))
            {
               return result.TimeOfDay;
            }
         }

         return null;
      }
   }
}