
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
   public delegate TimeSpan? LogLineTimeStampParserFunction(LogLine logLine);

   /// <summary>
   /// Parses time stamp information from client log lines.
   /// </summary>
   public class LogLineTimeStampParser
   {
      /// <summary>
      /// Gets a singleton instance of the <see cref="LogLineTimeStampParser"/> class.
      /// </summary>
      public static LogLineTimeStampParser Instance { get; } = new LogLineTimeStampParser();

      /// <summary>
      /// Returns a <see cref="TimeSpan"/> value based on the contents of the raw log line.
      /// </summary>
      /// <param name="logLine">The log line to parse.</param>
      /// <returns>A TimeSpan representing the time stamp data parsed from the log line or null if parsing fails.</returns>
      public TimeSpan? ParseTimeStamp(LogLine logLine)
      {
         return OnParseTimeStamp(logLine);
      }

      /// <summary>
      /// Implement this method in a derived type and return a <see cref="TimeSpan"/> value based on the contents of the raw log line.
      /// </summary>
      /// <param name="logLine">The log line to parse.</param>
      /// <returns>A TimeSpan representing the time stamp data parsed from the log line or null if parsing fails.</returns>
      protected virtual TimeSpan? OnParseTimeStamp(LogLine logLine)
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