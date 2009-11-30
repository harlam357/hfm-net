/*
 * HFM.NET - Protein Collection Class
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using HFM.Helpers;
using HFM.Preferences;
using HFM.Instrumentation;

namespace HFM.Proteins
{
   /// <summary>
   /// Protein Collection is a Generic Dictionary based on a string key and a Protein value.
   /// </summary>
   //[Serializable]
   public sealed class ProteinCollection : SortedDictionary<Int32, Protein>
   {
      #region Members
      // Would like to make this a const
      private readonly string _LocalProjectInfoFile = Path.Combine(PreferenceSet.Instance.AppDataPath, "ProjectInfo.tab");
      
      public delegate Protein GetProteinDelegate();
      private static GetProteinDelegate _GetProteinHandler = null;
      public static GetProteinDelegate GetProteinHandler
      {
         get { return _GetProteinHandler; }
         set { _GetProteinHandler = value; }
      }

      // Date and Time of Last psummary download attempt. See BeginDownloadFromStanford() and DownloadFromStanford().
      private DateTime _LastDownloadTime = DateTime.MinValue;

      // List of Projects that were not found after a download attempt.  See GetProtein().
      // If a Project is added to this list, it will not trigger another download attempt for 1 Day, unless
      // the user manually initiates a Project Download.  In which case, all Project on this list will be 
      // cleared. See BeginDownloadFromStanford().
      private readonly Dictionary<Int32, DateTime> _ProjectsNotFound = new Dictionary<Int32, DateTime>(); 
      #endregion

      #region Properties
      private static Uri _ProjectLoadLocation;
      
      public static Uri ProjectLoadLocation
      {
         get { return _ProjectLoadLocation; }
         set { _ProjectLoadLocation = value; }
      } 
      #endregion

      #region Events and Event Wrappers
      public event EventHandler ProjectInfoUpdated;

      private void OnProjectInfoUpdated(EventArgs e)
      {
         if (ProjectInfoUpdated != null)
         {
            ProjectInfoUpdated(this, e);
         }
      } 
      #endregion

      #region Constructor
      /// <summary>
      /// Private constructor - only called via the Instance property
      /// </summary>
      private ProteinCollection()
      {
         if (ProjectLoadLocation == null)
         {
            if (LoadFromTabDelimitedFile() == false)
            {
               DownloadFromStanford();
            }
         }
         else
         {
            DownloadFromStanford(ProjectLoadLocation);
         }
      } 
      #endregion

      #region Singleton Support
      // Instance and Class Lock
      private static ProteinCollection _Instance;
      private readonly static object _classLock = typeof(ProteinCollection);
      
      /// <summary>
      /// Thread-safe, static, Singleton initialize/return
      /// </summary>
      public static ProteinCollection Instance
      {
         get
         {
            lock (_classLock)
            {
               if (_Instance == null)
               {
                  _Instance = new ProteinCollection();
               }
            }
            return _Instance;
         }
      }
      #endregion

      #region Methods
      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      private bool LoadFromTabDelimitedFile()
      {
         return LoadFromTabDelimitedFile(_LocalProjectInfoFile);
      }

      /// <summary>
      /// Load the Protein Collection from Tab Delimited File
      /// </summary>
      /// <param name="ProjectInfoFilePath">Path to File</param>
      private bool LoadFromTabDelimitedFile(string ProjectInfoFilePath)
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
                  String[] lineData = sLine.Split(new char[] { '\t' }, StringSplitOptions.None);
                  p.ProjectNumber = Int32.Parse(lineData[0]);
                  p.ServerIP = lineData[1].Trim();
                  p.WorkUnitName = lineData[2].Trim();
                  p.NumAtoms = Int32.Parse(lineData[3]);
                  p.PreferredDays = Double.Parse(lineData[4]);
                  p.MaxDays = Double.Parse(lineData[5]);
                  p.Credit = Double.Parse(lineData[6]);
                  p.Frames = Int32.Parse(lineData[7]);
                  p.Core = lineData[8];
                  p.Description = lineData[9];
                  p.Contact = lineData[10];
                  
                  // Backward Compatibility with v0.4.0 or prior
                  // Newer versions download and parse the KFactor
                  if (lineData.Length > 11)
                  {
                     p.KFactor = Double.Parse(lineData[11]);
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
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      public IAsyncResult BeginDownloadFromStanford()
      {
         // Clear - see notes above list declaration
         _ProjectsNotFound.Clear();
         // Clear the Last Download Time
         _LastDownloadTime = DateTime.MinValue;
         return new System.Windows.Forms.MethodInvoker(DownloadFromStanford).BeginInvoke(null, null);
      }
      
      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      private void DownloadFromStanford()
      {
         // if an auto download was attempted in the last hour, don't execute again
         TimeSpan LastDownloadDifference = DateTime.Now.Subtract(_LastDownloadTime);
         if (LastDownloadDifference.TotalHours > 1)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                       "Attempting to Download new Project data...", true);
         
            _LastDownloadTime = DateTime.Now;
            DownloadFromStanford(new Uri(PreferenceSet.Instance.ProjectDownloadUrl));
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                       String.Format(CultureInfo.CurrentCulture, "Download executed {0:0} minutes ago.", 
                                       LastDownloadDifference.TotalMinutes), true);
         }
      }

      /// <summary>
      /// Download project information from Stanford University (psummary.html)
      /// </summary>
      private void DownloadFromStanford(Uri ProjectDownloadUri)
      {
         DateTime Start = HfmTrace.ExecStart;
         lock (_classLock)
         {
            try
            {
               HTMLparser pSummary = InitHTMLparser(ProjectDownloadUri);
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
                        if (ContainsKey(p.ProjectNumber))
                        {
                           this[p.ProjectNumber] = p;
                        }
                        else
                        {
                           Add(p.ProjectNumber, p);
                        }
                     }
                  }
               }
               if (Count > 0)
               {
                  SaveToTabDelimitedFile();

                  HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Loaded {0} Proteins from Stanford.", Count), true);
                  OnProjectInfoUpdated(EventArgs.Empty);
               }
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
            }
            finally
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
            }
         }
      }

      #region HTML Parsing Methods
      /// <summary>
      /// Initialize and Return HTMLparser Instance.
      /// </summary>
      /// <param name="ProjectDownloadUri">Uri Pointing to psummary</param>
      [CLSCompliant(false)]
      public static HTMLparser InitHTMLparser(Uri ProjectDownloadUri)
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
            if (Int32.TryParse(GetNextTdValue(pSummary), out ProjectNumber))
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
               p.NumAtoms = Int32.Parse(GetNextTdValue(pSummary));
            }
            catch (FormatException)
            {
               p.NumAtoms = 0;
            }
            p.PreferredDays = Double.Parse(GetNextTdValue(pSummary));
            p.MaxDays = Double.Parse(GetNextTdValue(pSummary));
            p.Credit = Double.Parse(GetNextTdValue(pSummary));
            p.Frames = Int32.Parse(GetNextTdValue(pSummary));
            p.Core = GetNextTdValue(pSummary);
            p.Description = GetNextTdValue(pSummary, "href");
            p.Contact = GetNextTdValue(pSummary);
            p.KFactor = Double.Parse(GetNextTdValue(pSummary));
            
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
         HTMLchunk oChunk;
         while ((oChunk = pSummary.ParseNext()) != null)
         {
            // Look for an Open "td" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "td")
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
                     // If it's a Close "td" Tag, return Empty String
                     else if (oChunk.oType.Equals(HTMLchunkType.CloseTag) &&
                              oChunk.sTag.ToLower() == "td")
                     {
                        return String.Empty;
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
                      oChunk.oType.Equals(HTMLchunkType.OpenTag))
                  {
                     // Return the specified Parameter Name
                     return oChunk.oParams[ParamName].ToString();
                  }
               }
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

         String[] CSVData = new String[Count];
         Int32 i = 0;
         
         foreach (KeyValuePair<Int32, Protein> kvp in this)
         {
            // Project Number, Server IP, Work Unit Name, Number of Atoms, Preferred (days),
            // Final Deadline (days), Credit, Frames, Code, Description, Contact, KFactor

            CSVData[i++] = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}\t{11}",
               /*  0 */ kvp.Value.ProjectNumber,    /*  1 */ kvp.Value.ServerIP,
               /*  2 */ kvp.Value.WorkUnitName,     /*  3 */ kvp.Value.NumAtoms,
               /*  4 */ kvp.Value.PreferredDays,    /*  5 */ kvp.Value.MaxDays,
               /*  6 */ kvp.Value.Credit,           /*  7 */ kvp.Value.Frames,
               /*  8 */ kvp.Value.Core,             /*  9 */ kvp.Value.Description,
               /* 10 */ kvp.Value.Contact,          /* 11 */ kvp.Value.KFactor);
         }

         File.WriteAllLines(_LocalProjectInfoFile, CSVData, Encoding.ASCII);
         
         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
      
      /// <summary>
      /// Get Protein (should be called from worker thread)
      /// </summary>
      /// <param name="ProjectID">Project ID</param>
      public Protein GetProtein(int ProjectID)
      {
         // Substitute an external delegate
         if (_GetProteinHandler != null)
         {
            return _GetProteinHandler();
         }

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
         DownloadFromStanford();

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
      #endregion
   }
}
