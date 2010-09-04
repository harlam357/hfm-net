/*
 * HFM.NET - Project Summary Downloader Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Text;

using Majestic12;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instrumentation;

namespace HFM.Proteins
{
   public sealed class ProjectSummaryDownloader : IProjectSummaryDownloader
   {
      #region Fields
      
      /// <summary>
      /// Collection Load Class Lock
      /// </summary>
      private readonly static object DownloadLock = new object();

      /// <summary>
      /// Time of Last Successful Download
      /// </summary>
      public DateTime LastDownloadTime { get; private set; }

      /// <summary>
      /// Project Summary HTML File Location
      /// </summary>
      public Uri ProjectSummaryLocation { get; set; }

      /// <summary>
      /// Local Project Info Tab File Location
      /// </summary>
      public string ProjectInfoLocation { get; set; }

      /// <summary>
      /// Protein Storage Dictionary
      /// </summary>
      public SortedDictionary<int, IProtein> Dictionary { get; set; }

      /// <summary>
      /// Preferences Interface
      /// </summary>
      public IPreferenceSet Prefs { get; set; }

      private readonly HTMLparser _htmlParser;
      
      #endregion

      public ProjectSummaryDownloader()
      {
         LastDownloadTime = DateTime.MinValue;
         _htmlParser = new HTMLparser();
      }

      #region Events and Event Wrappers

      public event EventHandler<DownloadProgressEventArgs> DownloadProgress;
      private void OnDownloadProgress(DownloadProgressEventArgs e)
      {
         if (DownloadProgress != null)
         {
            DownloadProgress(this, e);
         }
      }

      public event EventHandler ProjectDownloadFinished;
      private void OnProjectDownloadFinished(EventArgs e)
      {
         if (ProjectDownloadFinished != null)
         {
            ProjectDownloadFinished(this, e);
         }
      }
      
      /// <summary>
      /// Project (Protein) Data has been Updated
      /// </summary>
      public event EventHandler ProjectInfoUpdated;
      private void OnProjectInfoUpdated(EventArgs e)
      {
         if (ProjectInfoUpdated != null)
         {
            ProjectInfoUpdated(this, e);
         }
      }
      
      #endregion

      /// <summary>
      /// Reset the Last Download Time
      /// </summary>
      public void ResetLastDownloadTime()
      {
         // Reset the Last Download Time - see DownloadFromStanford(bool)
         LastDownloadTime = DateTime.MinValue;
      }

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public void DownloadFromStanford()
      {
         var projectDownloadUrl = Prefs.GetPreference<string>(Preference.ProjectDownloadUrl);
         DownloadFromStanford(new Uri(projectDownloadUrl), true);
      }

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public void DownloadFromStanford(Uri projectDownloadUrl, bool saveToFile)
      {
         lock (DownloadLock)
         {
            // if a download was attempted in the last hour, don't execute again
            TimeSpan lastDownloadDifference = DateTime.Now.Subtract(LastDownloadTime);
            if (lastDownloadDifference.TotalHours > 1)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Attempting to Download new Project data...", true);
               try
               {
                  ReadFromProjectSummaryHtml(projectDownloadUrl);
                  LastDownloadTime = DateTime.Now;

                  string loadedProteins = String.Format(CultureInfo.CurrentCulture, "Loaded {0} Proteins from Stanford.", Dictionary.Count);
                  OnDownloadProgress(new DownloadProgressEventArgs(100, loadedProteins));

                  if (Dictionary.Count > 0)
                  {
                     if (saveToFile) SaveToTabDelimitedFile();

                     HfmTrace.WriteToHfmConsole(TraceLevel.Info, loadedProteins, true);
                     OnProjectInfoUpdated(EventArgs.Empty);
                  }
               }
               catch (Exception ex)
               {
                  OnDownloadProgress(new DownloadProgressEventArgs(0, ex.Message));
                  HfmTrace.WriteToHfmConsole(ex);
               }
               finally
               {
                  OnProjectDownloadFinished(EventArgs.Empty);
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                          String.Format(CultureInfo.CurrentCulture, "Download executed {0:0} minutes ago.",
                                          lastDownloadDifference.TotalMinutes), true);
            }
         }
      }

      /// <summary>
      /// Read Project Information from HTML (psummary.html)
      /// </summary>
      public void ReadFromProjectSummaryHtml(Uri location)
      {
         DateTime start = HfmTrace.ExecStart;

         ProjectSummaryLocation = location;

         try
         {
            string[] psummaryLines = PerformDownload(ProjectSummaryLocation);
            for (int i = 0; i < psummaryLines.Length; i++)
            {
               Protein p = ParseProteinRow(psummaryLines[i]);
               if (p != null && p.Valid) 
               {
                  Dictionary[p.ProjectNumber] = p;
               }

               var progress = (int)((i / (double)psummaryLines.Length) * 100);
               OnDownloadProgress(new DownloadProgressEventArgs(progress, p == null ? String.Empty : p.WorkUnitName));
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, start);
         }
      }

      /// <summary>
      /// 
      /// </summary>
      /// <param name="projectDownloadUri">Uri Pointing to psummary</param>
      private static string[] PerformDownload(Uri projectDownloadUri)
      {
         var net = new NetworkOps();

         string tempPath = Path.Combine(Path.GetTempPath(), "psummary.html");
         net.HttpDownloadHelper(projectDownloadUri, tempPath, String.Empty, String.Empty);

         return File.ReadAllLines(tempPath);
      }

      #region HTML Parsing Methods

      /// <summary>
      /// Parse the HTML Table Row (tr) into a Protein Instance.
      /// </summary>
      private Protein ParseProteinRow(string html)
      {
         _htmlParser.Init(html);
         var p = new Protein();

         HTMLchunk oChunk;
         while ((oChunk = _htmlParser.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "tr")
            {
               try
               {
                  int projectNumber;
                  if (Int32.TryParse(GetNextTdValue(_htmlParser), NumberStyles.Integer, CultureInfo.InvariantCulture,
                                     out projectNumber))
                  {
                     p.ProjectNumber = projectNumber;
                  }
                  else
                  {
                     return null;
                  }
                  p.ServerIP = GetNextTdValue(_htmlParser);
                  p.WorkUnitName = GetNextTdValue(_htmlParser);
                  try
                  {
                     p.NumAtoms = Int32.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
                  }
                  catch (FormatException)
                  {
                     p.NumAtoms = 0;
                  }
                  p.PreferredDays = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
                  p.MaxDays = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
                  p.Credit = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
                  p.Frames = Int32.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);
                  p.Core = GetNextTdValue(_htmlParser);
                  p.Description = GetNextTdValue(_htmlParser, "href");
                  p.Contact = GetNextTdValue(_htmlParser);
                  p.KFactor = Double.Parse(GetNextTdValue(_htmlParser), CultureInfo.InvariantCulture);

                  return p;
               }
               catch (Exception ex)
               {
                  // Ignore this row of the table - unparseable
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, ex);
               }
            }
         }

         return null;
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="pSummary">HTMLparser Instance</param>
      [CLSCompliant(false)]
      public static string GetNextTdValue(HTMLparser pSummary)
      {
         return GetNextTdValue(pSummary, String.Empty);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      /// <param name="paramName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextTdValue(HTMLparser htmlParser, string paramName)
      {
         return GetNextValue(htmlParser, "td", paramName);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Heading Column (th).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      [CLSCompliant(false)]
      public static string GetNextThValue(HTMLparser htmlParser)
      {
         return GetNextValue(htmlParser, "th", String.Empty);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="htmlParser">HTMLparser Instance</param>
      /// <param name="tagName">Name of Tag to Search for</param>
      /// <param name="paramName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextValue(HTMLparser htmlParser, string tagName, string paramName)
      {
         HTMLchunk oChunk;
         while ((oChunk = htmlParser.ParseNext()) != null)
         {
            // Look for an Open Tag matching the given Tag Name
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == tagName)
            {
               // If not looking for a Tag Parameter
               if (paramName.Length == 0)
               {
                  // Look inside the "td" Tag
                  oChunk = htmlParser.ParseNext();
                  if (oChunk != null)
                  {
                     // If it's an Open "font" Tag
                     if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                         oChunk.sTag.ToLower() == "font")
                     {
                        // Look inside the "font" Tag
                        oChunk = htmlParser.ParseNext();

                        // If it's Text, return it
                        if (oChunk != null &&
                            oChunk.oType.Equals(HTMLchunkType.Text))
                        {
                           return oChunk.oHTML.Trim();
                        }
                     }
                     // If it's Text, return it
                     else if (oChunk.oType.Equals(HTMLchunkType.Text))
                     {
                        return oChunk.oHTML.Trim();
                     }
                  }
               }
               // Looking for a Tag Parameter
               else
               {
                  // Look inside the "td" Tag
                  oChunk = htmlParser.ParseNext();

                  // If it's an Open Tag
                  if (oChunk != null &&
                      oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                      oChunk.oParams.Contains(paramName))
                  {
                     // Return the specified Parameter Name
                     return oChunk.oParams[paramName].ToString();
                  }
               }

               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, "Could not find psummary column value.  Returning an empty string.", true);

               return String.Empty;
            }
         }

         throw new InvalidOperationException("Could not complete operation to get td tag value.");
      }
      #endregion

      /// <summary>
      /// Save Protein Collection to File (for reload next execution)
      /// </summary>
      private void SaveToTabDelimitedFile()
      {
         DateTime start = HfmTrace.ExecStart;

         var csvData = new String[Dictionary.Count];
         int i = 0;

         foreach (KeyValuePair<Int32, IProtein> kvp in Dictionary)
         {
            // Project Number, Server IP, Work Unit Name, Number of Atoms, Preferred (days),
            // Final Deadline (days), Credit, Frames, Code, Description, Contact, KFactor

            csvData[i++] = String.Format(CultureInfo.InvariantCulture,
                                         "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
               /*  0 */ kvp.Value.ProjectNumber,    /*  1 */ kvp.Value.ServerIP,
               /*  2 */ kvp.Value.WorkUnitName,     /*  3 */ kvp.Value.NumAtoms,
               /*  4 */ kvp.Value.PreferredDays,    /*  5 */ kvp.Value.MaxDays,
               /*  6 */ kvp.Value.Credit,           /*  7 */ kvp.Value.Frames,
               /*  8 */ kvp.Value.Core,             /*  9 */ kvp.Value.Description,
               /* 10 */ kvp.Value.Contact,          /* 11 */ kvp.Value.KFactor);
         }

         File.WriteAllLines(ProjectInfoLocation, csvData, Encoding.ASCII);

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
      }

      #region IDisposable Members

      private bool _disposed;

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      private void Dispose(bool disposing)
      {
         if (!_disposed)
         {
            if (disposing)
            {
               _htmlParser.Dispose();
            }
         }

         _disposed = true;
      }

      ~ProjectSummaryDownloader()
      {
         Dispose(false);
      }

      #endregion
   }
}
