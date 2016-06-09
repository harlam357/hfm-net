
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace HFM.Log
{
   public static class UnitInfoLog
   {
      /// <summary>
      /// Parse the content from the unitinfo.txt file.
      /// </summary>
      /// <param name="logFilePath">Path to the log file.</param>
      /// <exception cref="System.ArgumentException">Throws if logFilePath is null or empty.</exception>
      /// <exception cref="System.IO.IOException">Throws if file specified by logFilePath cannot be read.</exception>
      /// <exception cref="System.FormatException">Throws if log data fails parsing.</exception>
      public static UnitInfoLogData Read(string logFilePath)
      {
         if (String.IsNullOrEmpty(logFilePath))
         {
            throw new ArgumentException("Argument 'logFilePath' cannot be a null or empty string.");
         }

         string[] logLines;
         try
         {
            logLines = File.ReadAllLines(logFilePath);
         }
         catch (Exception ex)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture, "Failed to read file '{0}'", logFilePath), ex);
         }

         var data = new UnitInfoLogData();

         string line = null;
         try
         {
            foreach (string s in logLines)
            {
               line = s;

               /* Name (Only Read Here) */
               if (line.StartsWith("Name: "))
               {
                  data.ProteinName = line.Substring(6);
               }
               /* Tag (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Tag: "))
               {
                  data.ProteinTag = line.Substring(5);

                  Match mProjectNumberFromTag;
                  if ((mProjectNumberFromTag = UnitInfoRegex.RegexProjectNumberFromTag.Match(data.ProteinTag)).Success)
                  {
                     data.ProjectID = Int32.Parse(mProjectNumberFromTag.Result("${ProjectNumber}"));
                     data.ProjectRun = Int32.Parse(mProjectNumberFromTag.Result("${Run}"));
                     data.ProjectClone = Int32.Parse(mProjectNumberFromTag.Result("${Clone}"));
                     data.ProjectGen = Int32.Parse(mProjectNumberFromTag.Result("${Gen}"));
                  }
               }
               /* DownloadTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Download time: "))
               {
                  data.DownloadTime = DateTime.ParseExact(line.Substring(15), "MMMM d H:mm:ss",
                                                          DateTimeFormatInfo.InvariantInfo,
                                                          LogReaderExtensions.DateTimeStyle);
               }
               /* DueTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Due time: "))
               {
                  data.DueTime = DateTime.ParseExact(line.Substring(10), "MMMM d H:mm:ss",
                                                     DateTimeFormatInfo.InvariantInfo,
                                                     LogReaderExtensions.DateTimeStyle);
               }
               /* Progress (Supplemental Read - if progress percentage cannot be determined through FAHlog.txt) */
               else if (line.StartsWith("Progress: "))
               {
                  data.Progress = Int32.Parse(line.Substring(10, line.IndexOf("%") - 10));
               }
            }
         }
         catch (Exception ex)
         {
            throw new FormatException(String.Format(CultureInfo.CurrentCulture, "Failed to parse line '{0}'", line), ex);
         }

         return data;
      }
   }
}
