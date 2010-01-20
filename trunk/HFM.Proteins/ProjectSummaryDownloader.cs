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
      #region Members
      /// <summary>
      /// Collection Load Class Lock
      /// </summary>
      private readonly static object _downloadLock = new object();

      private DateTime _LastDownloadTime = DateTime.MinValue;
      /// <summary>
      /// Time of Last Successful Download
      /// </summary>
      public DateTime LastDownloadTime
      {
         get { return _LastDownloadTime; }
      }

      private Uri _ProjectSummaryLocation;
      /// <summary>
      /// Project Summary HTML File Location
      /// </summary>
      public Uri ProjectSummaryLocation
      {
         get { return _ProjectSummaryLocation; }
         set { _ProjectSummaryLocation = value; }
      }

      private string _ProjectInfoLocation;
      /// <summary>
      /// Local Project Info Tab File Location
      /// </summary>
      public string ProjectInfoLocation
      {
         get { return _ProjectInfoLocation; }
         set { _ProjectInfoLocation = value; }
      }
      
      private SortedDictionary<Int32, IProtein> _Dictionary;
      /// <summary>
      /// Protein Storage Dictionary
      /// </summary>
      public SortedDictionary<Int32, IProtein> Dictionary
      {
         get { return _Dictionary; }
         set { _Dictionary = value; }
      }
      
      private IPreferenceSet _Prefs;
      /// <summary>
      /// Preferences Interface
      /// </summary>
      public IPreferenceSet Prefs
      {
         get { return _Prefs; }
         set { _Prefs = value; }
      }
      #endregion

      #region Events and Event Wrappers
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
         _LastDownloadTime = DateTime.MinValue;
      }

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public void DownloadFromStanford()
      {
         string ProjectDownloadUrl = _Prefs.GetPreference<string>(Preference.ProjectDownloadUrl);
         DownloadFromStanford(new Uri(ProjectDownloadUrl), true);
      }

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public void DownloadFromStanford(Uri ProjectDownloadUrl, bool SaveToFile)
      {
         lock (_downloadLock)
         {
            // if a download was attempted in the last hour, don't execute again
            TimeSpan LastDownloadDifference = DateTime.Now.Subtract(_LastDownloadTime);
            if (LastDownloadDifference.TotalHours > 1)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Attempting to Download new Project data...", true);
               try
               {
                  ReadFromProjectSummaryHtml(ProjectDownloadUrl);
                  _LastDownloadTime = DateTime.Now;

                  if (_Dictionary.Count > 0)
                  {
                     if (SaveToFile) SaveToTabDelimitedFile();

                     HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Loaded {0} Proteins from Stanford.", _Dictionary.Count), true);
                     OnProjectInfoUpdated(EventArgs.Empty);
                  }
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                          String.Format(CultureInfo.CurrentCulture, "Download executed {0:0} minutes ago.",
                                          LastDownloadDifference.TotalMinutes), true);
            }
         }
      }

      /// <summary>
      /// Read Project Information from HTML (psummary.html)
      /// </summary>
      public void ReadFromProjectSummaryHtml(Uri location)
      {
         DateTime Start = HfmTrace.ExecStart;

         ProjectSummaryLocation = location;

         try
         {
            HTMLparser pSummary = InitHTMLparser(ProjectSummaryLocation);
            HTMLchunk oChunk;

            // Parse until returned oChunk is null indicating we reached end of parsing
            while ((oChunk = pSummary.ParseNext()) != null)
            {
               // Look for an Open "tr" Tag
               if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                   oChunk.sTag.ToLower() == "tr")
               {
                  Protein p = ParseProteinRow(pSummary);
                  if (p != null)
                  {
                     if (_Dictionary.ContainsKey(p.ProjectNumber))
                     {
                        _Dictionary[p.ProjectNumber] = p;
                     }
                     else
                     {
                        _Dictionary.Add(p.ProjectNumber, p);
                     }
                  }
               }
            }
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
         }
      }

      #region HTML Parsing Methods
      /// <summary>
      /// Initialize and Return HTMLparser Instance.
      /// </summary>
      /// <param name="ProjectDownloadUri">Uri Pointing to psummary</param>
      [CLSCompliant(false)]
      public HTMLparser InitHTMLparser(Uri ProjectDownloadUri)
      {
         string sSummaryPage;
         using (StreamReader sr1 = new StreamReader(
                NetworkOps.HttpDownloadHelper(ProjectDownloadUri), Encoding.ASCII))
         {
            sSummaryPage = sr1.ReadToEnd();
         }

         HTMLparser pSummary = new HTMLparser();
         pSummary.Init(sSummaryPage);
         return pSummary;
      }

      /// <summary>
      /// Parse the HTML Table Row (tr) into a Protein Instance.
      /// </summary>
      /// <param name="pSummary">HTMLparser Instance</param>
      private static Protein ParseProteinRow(HTMLparser pSummary)
      {
         Protein p = new Protein();

         try
         {
            int ProjectNumber;
            if (Int32.TryParse(GetNextTdValue(pSummary), NumberStyles.Integer, CultureInfo.InvariantCulture, out ProjectNumber))
            {
               p.ProjectNumber = ProjectNumber;
            }
            else
            {
               return null;
            }
            p.ServerIP = GetNextTdValue(pSummary);
            p.WorkUnitName = GetNextTdValue(pSummary);
            try
            {
               p.NumAtoms = Int32.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);
            }
            catch (FormatException)
            {
               p.NumAtoms = 0;
            }
            p.PreferredDays = Double.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);
            p.MaxDays = Double.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);
            p.Credit = Double.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);
            p.Frames = Int32.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);
            p.Core = GetNextTdValue(pSummary);
            p.Description = GetNextTdValue(pSummary, "href");
            p.Contact = GetNextTdValue(pSummary);
            p.KFactor = Double.Parse(GetNextTdValue(pSummary), CultureInfo.InvariantCulture);

            return p;
         }
         catch (Exception ex)
         {
            // Ignore this row of the table - unparseable
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
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
      /// <param name="pSummary">HTMLparser Instance</param>
      /// <param name="ParamName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextTdValue(HTMLparser pSummary, string ParamName)
      {
         return GetNextValue(pSummary, "td", ParamName);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Heading Column (th).
      /// </summary>
      /// <param name="pSummary">HTMLparser Instance</param>
      [CLSCompliant(false)]
      public static string GetNextThValue(HTMLparser pSummary)
      {
         return GetNextValue(pSummary, "th", String.Empty);
      }

      /// <summary>
      /// Return the value enclosed in the HTML Table Column (td).
      /// </summary>
      /// <param name="pSummary">HTMLparser Instance</param>
      /// <param name="TagName">Name of Tag to Search for</param>
      /// <param name="ParamName">Name of Tag Parameter to Return</param>
      [CLSCompliant(false)]
      public static string GetNextValue(HTMLparser pSummary, string TagName, string ParamName)
      {
         HTMLchunk oChunk;
         while ((oChunk = pSummary.ParseNext()) != null)
         {
            // Look for an Open Tag matching the given Tag Name
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == TagName)
            {
               // If not looking for a Tag Parameter
               if (ParamName.Length == 0)
               {
                  // Look inside the "td" Tag
                  oChunk = pSummary.ParseNext();
                  if (oChunk != null)
                  {
                     // If it's an Open "font" Tag
                     if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                         oChunk.sTag.ToLower() == "font")
                     {
                        // Look inside the "font" Tag
                        oChunk = pSummary.ParseNext();

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
                  oChunk = pSummary.ParseNext();

                  // If it's an Open Tag
                  if (oChunk != null &&
                      oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                      oChunk.oParams.Contains(ParamName))
                  {
                     // Return the specified Parameter Name
                     return oChunk.oParams[ParamName].ToString();
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
         DateTime Start = HfmTrace.ExecStart;

         String[] CSVData = new String[_Dictionary.Count];
         Int32 i = 0;

         foreach (KeyValuePair<Int32, IProtein> kvp in _Dictionary)
         {
            // Project Number, Server IP, Work Unit Name, Number of Atoms, Preferred (days),
            // Final Deadline (days), Credit, Frames, Code, Description, Contact, KFactor

            CSVData[i++] = String.Format(CultureInfo.InvariantCulture,
                                         "{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
               /*  0 */ kvp.Value.ProjectNumber,    /*  1 */ kvp.Value.ServerIP,
               /*  2 */ kvp.Value.WorkUnitName,     /*  3 */ kvp.Value.NumAtoms,
               /*  4 */ kvp.Value.PreferredDays,    /*  5 */ kvp.Value.MaxDays,
               /*  6 */ kvp.Value.Credit,           /*  7 */ kvp.Value.Frames,
               /*  8 */ kvp.Value.Core,             /*  9 */ kvp.Value.Description,
               /* 10 */ kvp.Value.Contact,          /* 11 */ kvp.Value.KFactor);
         }

         File.WriteAllLines(_ProjectInfoLocation, CSVData, Encoding.ASCII);

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
   }
}
