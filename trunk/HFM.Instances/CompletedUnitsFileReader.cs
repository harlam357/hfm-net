/*
 * HFM.NET - Completed Units File Reader Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public class CompletedUnitsFileReader : ICompletedUnitsFileReader
   {
      private const string Comma = ",";

      public bool Processing { get; private set; }
      public Exception Exception { get; private set; }

      public string CompletedUnitsFilePath { get; set; }
      public CompletedUnitsReadResult Result { get; private set; }
   
      #region Import CompletedUnits.csv

      private static string GetUnitCsvHeader()
      {
         var sbldr = new StringBuilder();
         sbldr.Append("ProjectID");
         sbldr.Append(Comma);
         sbldr.Append("Work Unit Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Name");
         sbldr.Append(Comma);
         sbldr.Append("Instance Path");
         sbldr.Append(Comma);
         sbldr.Append("Username");
         sbldr.Append(Comma);
         sbldr.Append("Team");
         sbldr.Append(Comma);
         sbldr.Append("Client Type");
         sbldr.Append(Comma);
         sbldr.Append("Core Name");
         sbldr.Append(Comma);
         sbldr.Append("Core Version");
         sbldr.Append(Comma);
         sbldr.Append("Frame Time (Average)");
         sbldr.Append(Comma);
         sbldr.Append("PPD");
         sbldr.Append(Comma);
         sbldr.Append("Download Date");
         sbldr.Append(Comma);
         sbldr.Append("Download Time");
         sbldr.Append(Comma);
         sbldr.Append("Completion Date (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Completion Time (Observed)");
         sbldr.Append(Comma);
         sbldr.Append("Credit");
         sbldr.Append(Comma);
         sbldr.Append("Frames");
         sbldr.Append(Comma);
         sbldr.Append("Atoms");
         sbldr.Append(Comma);
         sbldr.Append("Run/Clone/Gen");

         return sbldr.ToString();
      }

      public event EventHandler<ProgressEventArgs> ProgressChanged;
      private void OnProgressChanged(ProgressEventArgs e)
      {
         if (ProgressChanged != null)
         {
            ProgressChanged(this, e);
         }
      }

      public event EventHandler ProcessFinished;
      private void OnProcessFinished(EventArgs e)
      {
         if (ProcessFinished != null)
         {
            ProcessFinished(this, e);
         }
      }

      public void Process()
      {
         Processing = true;
         Exception = null;
         Result = null;
         
         try
         {
            string[] lines = File.ReadAllLines(CompletedUnitsFilePath);
            var result = new CompletedUnitsReadResult();

            for (int i = 0; i < lines.Length; i++)
            {
               var progress = (int)((i / (double)lines.Length) * 100);
               
               try
               {
                  if (lines[i].Equals(GetUnitCsvHeader()))
                  {
                     continue;
                  }
                  HistoryEntry entry = ParseHistoryEntry(lines[i]);
                  if (result.Entries.Contains(entry))
                  {
                     result.Duplicates++;
                  }
                  else
                  {
                     result.Entries.Add(entry);
                  }
                  
                  OnProgressChanged(new ProgressEventArgs(progress, String.Format(CultureInfo.CurrentCulture,
                                                                                  "P{0} (R{1}, C{2}, G{3})", entry.ProjectID, entry.ProjectRun, entry.ProjectClone, entry.ProjectGen)));
               }
               catch (FormatException)
               {
                  result.ErrorLines.Add(lines[i]);
                  OnProgressChanged(new ProgressEventArgs(progress, String.Empty));
               }
            }

            Result = result;
         }
         catch (Exception ex)
         {
            Exception = ex;
         }
         finally
         {
            Processing = false;
            OnProcessFinished(EventArgs.Empty);
         }
      }
      
      public bool SupportsCancellation
      {
         get { return false; }
      }

      public void Cancel()
      {
         throw new NotImplementedException();
      }

      private static HistoryEntry ParseHistoryEntry(string line)
      {
         string[] tokens = line.Split(',');
         if (tokens.Length != 19)
         {
            throw new FormatException("Too many commas.");
         }

         var entry = new HistoryEntry();
         entry.ProjectID = Int32.Parse(tokens[0]);
         GetRunCloneGen(tokens[18], entry);
         entry.InstanceName = tokens[2];
         entry.InstancePath = tokens[3];
         entry.Username = tokens[4];
         entry.Team = Int32.Parse(tokens[5]);
         entry.CoreVersion = Single.Parse(tokens[8]);
         entry.FramesCompleted = 100; // assumed
         entry.FrameTime = TimeSpan.Parse(tokens[9]);
         entry.Result = WorkUnitResult.FinishedUnit; // assumed
         entry.DownloadDateTime = DateTime.ParseExact(tokens[11] + " " + tokens[12], "M/d/yyyy h:mm tt",
                                                      DateTimeFormatInfo.CurrentInfo,
                                                      DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal);
         entry.CompletionDateTime = DateTime.ParseExact(tokens[13] + " " + tokens[14], "M/d/yyyy h:mm tt",
                                                        DateTimeFormatInfo.CurrentInfo,
                                                        DateTimeStyles.AssumeLocal | DateTimeStyles.AdjustToUniversal);
         return entry;
      }

      private static void GetRunCloneGen(string token, HistoryEntry entry)
      {
         var regEx = new Regex("\\((?<Run>.*)/(?<Clone>.*)/(?<Gen>.*)\\)", RegexOptions.ExplicitCapture | RegexOptions.Singleline);
         var rcgMatch = regEx.Match(token);
         if (rcgMatch.Success == false) throw new FormatException("Cannot match R/C/G.");

         entry.ProjectRun = Int32.Parse(rcgMatch.Result("${Run}"));
         entry.ProjectClone = Int32.Parse(rcgMatch.Result("${Clone}"));
         entry.ProjectGen = Int32.Parse(rcgMatch.Result("${Gen}"));
      }

      public void WriteCompletedUnitErrorLines(string filePath, IEnumerable<string> lines)
      {
         using (var stream = File.CreateText(filePath))
         {
            stream.WriteLine(GetUnitCsvHeader());
            foreach (var line in lines)
            {
               stream.WriteLine(line);
            }
         }
      }

      #endregion
   }
}
