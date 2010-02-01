/*
 * HFM.NET - Protein Collection Class
 * Copyright (C) 2006 David Rawling
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

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Proteins
{
   /// <summary>
   /// Protein Collection
   /// </summary>
   //[Serializable]
   public sealed class ProteinCollection : SortedDictionary<Int32, IProtein>, IProteinCollection
   {
      #region Members
      private readonly string _ProjectInfoLocation;
      /// <summary>
      /// ProjectInfo.tab File Location
      /// </summary>
      public string ProjectInfoLocation
      {
         get { return _ProjectInfoLocation; }
      }
      
      private readonly IProjectSummaryDownloader _Downloader;
      /// <summary>
      /// Project Summary Downloader Interface
      /// </summary>
      public IProjectSummaryDownloader Downloader
      {
         get { return _Downloader; }
      }
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs;

      /// <summary>
      /// List of Projects that were not found after a download attempt.
      /// </summary>
      /// <remarks>See GetProtein() and BeginDownloadFromStanford().</remarks>
      private readonly Dictionary<Int32, DateTime> _ProjectsNotFound = new Dictionary<Int32, DateTime>(); 
      #endregion
      
      public ProteinCollection(IProjectSummaryDownloader Downloader, IPreferenceSet Prefs)
      {
         _Prefs = Prefs;
         _ProjectInfoLocation = Path.Combine(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), "ProjectInfo.tab");
         
         _Downloader = Downloader;
         _Downloader.Dictionary = this;
         _Downloader.ProjectInfoLocation = ProjectInfoLocation;
         _Prefs = Prefs;
      }

      #region Methods
      /// <summary>
      /// Execute Primary Collection Load Sequence
      /// </summary>
      public void Load()
      {
         if (LoadFromTabDelimitedFile() == false)
         {
            _Downloader.DownloadFromStanford();
         }
      }

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      public bool LoadFromTabDelimitedFile()
      {
         return LoadFromTabDelimitedFile(_ProjectInfoLocation);
      }

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      /// <param name="ProjectInfoFilePath">Path to File</param>
      public bool LoadFromTabDelimitedFile(string ProjectInfoFilePath)
      {
         DateTime Start = HfmTrace.ExecStart;
         try
         {
            // If this is set to true, then it signals that the Project load
            // was successful but all expected data was not present.  We now
            // need to Download the psummary and update currently running
            // Projects with current data. Old Projects not found on the 
            // psummary will be left in the collection for historical purposes
            // and the missing values defaulted.
            bool BackwardCompatibleDownload = false;

            String[] CSVData = File.ReadAllLines(ProjectInfoFilePath);
            foreach (String sLine in CSVData)
            {
               try
               {
                  // Parse the current line from the CSV file
                  Protein p = new Protein();
                  String[] lineData = sLine.Split(new char[] {'\t'}, StringSplitOptions.None);
                  p.ProjectNumber = Int32.Parse(lineData[0], CultureInfo.InvariantCulture);
                  p.ServerIP = lineData[1].Trim();
                  p.WorkUnitName = lineData[2].Trim();
                  p.NumAtoms = Int32.Parse(lineData[3], CultureInfo.InvariantCulture);
                  p.PreferredDays = Double.Parse(lineData[4], CultureInfo.InvariantCulture);
                  p.MaxDays = Double.Parse(lineData[5], CultureInfo.InvariantCulture);
                  p.Credit = Double.Parse(lineData[6], CultureInfo.InvariantCulture);
                  p.Frames = Int32.Parse(lineData[7], CultureInfo.InvariantCulture);
                  p.Core = lineData[8];
                  p.Description = lineData[9];
                  p.Contact = lineData[10];

                  // Backward Compatibility with v0.4.0 or prior
                  // Newer versions download and parse the KFactor
                  if (lineData.Length > 11)
                  {
                     p.KFactor = Double.Parse(lineData[11], CultureInfo.InvariantCulture);
                  }
                  else
                  {
                     BackwardCompatibleDownload = true;
                     p.KFactor = 0;
                  }

                  if (ContainsKey(p.ProjectNumber))
                  {
                     this[p.ProjectNumber] = p;
                  }
                  else
                  {
                     Add(p.ProjectNumber, p);
                  }
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
               }
            }

            if (BackwardCompatibleDownload)
            {
               // Signal the caller to Download and update from the psummary
               return false;
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            return false;
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
         }

         return true;
      }

      /// <summary>
      /// Clear the Projects not found cache
      /// </summary>
      public void ClearProjectsNotFoundCache()
      {
         // See GetProtein() implementation
         _ProjectsNotFound.Clear();
      }

      private delegate void DownloadFromStanfordDelegate();

      /// <summary>
      /// Download project information from Stanford University (THREAD SAFE)
      /// </summary>
      public IAsyncResult BeginDownloadFromStanford()
      {
         return new DownloadFromStanfordDelegate(_Downloader.DownloadFromStanford).BeginInvoke(null, null);
      }
      
      /// <summary>
      /// Get Protein from Collection (should be called from worker thread)
      /// </summary>
      /// <param name="ProjectID">Project ID</param>
      public IProtein GetProtein(int ProjectID)
      {
         // If Project Requested is 0, just return a "blank" Protein.
         if (ProjectID == 0) return new Protein();
      
         // If Project is Found, return it
         if (ContainsKey(ProjectID)) return this[ProjectID];
         
         HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                    String.Format("Project ID '{0}' not found in Protein Collection.", ProjectID), true);
         
         // If Project has already been looked for previously
         if (_ProjectsNotFound.ContainsKey(ProjectID))
         {
            // If it has been less than one day since this Project triggered an
            // automatic download attempt, just return a "blank" Protein.
            if (DateTime.Now.Subtract(_ProjectsNotFound[ProjectID]).TotalDays < 1)
            {
               return new Protein();
            }
         }
         
         // Execute a Download
         _Downloader.DownloadFromStanford();

         // If the Project is now Found
         if (ContainsKey(ProjectID))
         {
            // Remove it from the Not Found List and return it
            _ProjectsNotFound.Remove(ProjectID);
            return this[ProjectID];
         }
         else
         {
            // If already on the Not Found List
            if (_ProjectsNotFound.ContainsKey(ProjectID))
            {
               // Update the Last Download Attempt Date
               _ProjectsNotFound[ProjectID] = DateTime.Now;
            }
            else // Add it
            {
               _ProjectsNotFound.Add(ProjectID, DateTime.Now);
            }
            
            HfmTrace.WriteToHfmConsole(TraceLevel.Error,
                                       String.Format("Project ID '{0}' not found on Stanford Web Project Summary.", ProjectID), true);
         }
         
         // Return a "blank" Protein
         return new Protein();
      }
      
      /// <summary>
      /// Get a New Protein from the Collection
      /// </summary>
      public IProtein GetNewProtein()
      {
         return new Protein();
      }
      #endregion
   }
}
