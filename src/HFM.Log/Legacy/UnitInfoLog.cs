
using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

using HFM.Log.Internal;

namespace HFM.Log.Legacy
{
   public static class UnitInfoLog
   {
      /// <summary>
      /// Reads the content from a unitinfo.txt file.
      /// </summary>
      /// <param name="path">Path to the log file.</param>
      /// <exception cref="System.ArgumentException">if path is null or empty.</exception>
      /// <exception cref="System.FormatException">if log data fails parsing.</exception>
      public static UnitInfoLogData Read(string path)
      {
         if (String.IsNullOrEmpty(path))
         {
            throw new ArgumentException("Argument 'path' cannot be a null or empty string.");
         }

         string[] logLines = File.ReadAllLines(path);

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

                  Match projectNumberFromTagMatch;
                  if ((projectNumberFromTagMatch = RegexProjectNumberFromTag.Match(data.ProteinTag)).Success)
                  {
                     data.ProjectID = Int32.Parse(projectNumberFromTagMatch.Groups["ProjectNumber"].Value);
                     data.ProjectRun = Int32.Parse(projectNumberFromTagMatch.Groups["Run"].Value);
                     data.ProjectClone = Int32.Parse(projectNumberFromTagMatch.Groups["Clone"].Value);
                     data.ProjectGen = Int32.Parse(projectNumberFromTagMatch.Groups["Gen"].Value);
                  }
               }
               /* DownloadTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Download time: "))
               {
                  data.DownloadTime = DateTime.ParseExact(line.Substring(15), "MMMM d H:mm:ss",
                                                          DateTimeFormatInfo.InvariantInfo,
                                                          DateTimeParse.Styles);
               }
               /* DueTime (Could be read here or through the queue.dat) */
               else if (line.StartsWith("Due time: "))
               {
                  data.DueTime = DateTime.ParseExact(line.Substring(10), "MMMM d H:mm:ss",
                                                     DateTimeFormatInfo.InvariantInfo,
                                                     DateTimeParse.Styles);
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

      private const RegexOptions Options = RegexOptions.Compiled | RegexOptions.ExplicitCapture;

      private static readonly Regex RegexProjectNumberFromTag =
         new Regex("P(?<ProjectNumber>.*)R(?<Run>.*)C(?<Clone>.*)G(?<Gen>.*)", Options);
   }
}
