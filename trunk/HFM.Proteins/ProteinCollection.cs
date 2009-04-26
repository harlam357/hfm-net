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
using System.IO;
using System.Net;
using System.Text;

using Majestic12;

using HFM.Preferences;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Proteins
{
    /// <summary>
    /// Event Arguments for the ProjectInfoUpdated event
    /// </summary>
    public class ProjectInfoUpdatedEventArgs: EventArgs
    {
    }

    public delegate void ProjectInfoUpdatedEventHandler(object sender, ProjectInfoUpdatedEventArgs e);

    /// <summary>
    /// Protein Collection is a Generic Dictionary based on a string key and a Protein value.
    /// The protein collection is Singleton pattern for efficiency (imagine 60 instances downloading
    /// protein information simultaneously)
    /// </summary>
    public class ProteinCollection : Dictionary<Int32, Protein>
    {
        private static ProteinCollection _Instance;
        private readonly static Object classLock = typeof(ProteinCollection);
        //private readonly Timer _DownloadTimer;
        
        // Erk ... this required local admin before!?
        private readonly string _LocalProjectInfoFile = Path.Combine(PreferenceSet.Instance.AppDataPath, "ProjectInfo.tab");

        public event ProjectInfoUpdatedEventHandler ProjectInfoUpdated;

        /// <summary>
        /// Event thrower.
        /// </summary>
        /// <param name="e"></param>
        protected virtual void OnProjectInfoUpdated(ProjectInfoUpdatedEventArgs e)
        {
            if (ProjectInfoUpdated != null)
            {
                ProjectInfoUpdated(this, e);
            }
        }

        /// <summary>
        /// Private constructor - only called via the Instance property
        /// </summary>
        private ProteinCollection()
        {
            DateTime Start = Debug.ExecStart;
            if (LoadFromTabDelimitedFile() == false)
            {
               DownloadFromStanford();
            }

            //_DownloadTimer = new Timer();
            //_DownloadTimer.Interval = 172800;     // 1000ms/sec * 60sec/min * 60min/hr * 24hr/d * 2d;
            //_DownloadTimer.Tick += new System.EventHandler(this._DownloadTimer_Tick);
            //_DownloadTimer.Start();
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
        }



        /// <summary>
        /// Thread-safe, static, Singleton initialize/return
        /// </summary>
        public static ProteinCollection Instance
        {
            get
            {
                lock (classLock)
                {
                    if (_Instance == null)
                    {
                        _Instance = new ProteinCollection();
                    }
                }
                return _Instance;
            }
        }

        ///// <summary>
        ///// Handle scheduled (timer) download
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void _DownloadTimer_Tick(object sender, EventArgs e)
        //{
        //    DateTime Start = Debug.ExecStart;
        //    _DownloadTimer.Stop();

        //    DownloadFromStanford(null);
        //    LoadFromTabDelimitedFile(_LocalProjectInfoFile);

        //    _DownloadTimer.Start();
        //    Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
        //}

        public bool LoadFromTabDelimitedFile()
        {
            return LoadFromTabDelimitedFile(_LocalProjectInfoFile);
        }

       /// <summary>
        /// Load the previously saved collection of protein information from Tab Delimited File
        /// </summary>
        /// <param name="ProjectInfoFile">Filename to load</param>
        public bool LoadFromTabDelimitedFile(string ProjectInfoFile)
        {
            DateTime Start = Debug.ExecStart;
            //String[] CSVData = new String[this.Count];

            try
            {
                String[] CSVData = File.ReadAllLines(ProjectInfoFile);
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
                        p.PreferredDays = Int32.Parse(lineData[4]);
                        p.MaxDays = Int32.Parse(lineData[5]);
                        p.Credit = Int32.Parse(lineData[6]);
                        p.Frames = Int32.Parse(lineData[7]);
                        p.Core = lineData[8];
                        p.Description = lineData[9];
                        p.Contact = lineData[10];

                        if (ContainsKey(p.ProjectNumber))
                        {
                            this[p.ProjectNumber] = p;
                        }
                        else
                        {
                            Add(p.ProjectNumber, p);
                        }
                    }
                    catch (Exception ExInner)
                    {
                        Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ExInner.Message));
                    }
                }
                if (Count > 0)
                {
                   OnProjectInfoUpdated(new ProjectInfoUpdatedEventArgs());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
                return false;
            }
            
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
            return true;
        }

        public void DownloadFromStanford()
        {
            System.Threading.ThreadPool.QueueUserWorkItem(DownloadFromStanford, null);
        }

        /// <summary>
        /// Download project information from Stanford University (psummaryC.html)
        /// </summary>
        /// <param name="State">Null in this implementation</param>
        public void DownloadFromStanford(Object State /* null */)
        {
            DateTime Start = Debug.ExecStart;
            lock (this)
            {
                PreferenceSet Prefs = PreferenceSet.Instance;

                WebRequest wrq = WebRequest.Create("http://vspx27.stanford.edu/psummaryC.html");
                wrq.Method = WebRequestMethods.Http.Get;
                WebResponse wrs;
                StreamReader sr1;
                if (Prefs.UseProxy)
                {
                    wrq.Proxy = new WebProxy(Prefs.ProxyServer, Prefs.ProxyPort);
                    if (Prefs.UseProxyAuth)
                    {
                        wrq.Proxy.Credentials = new NetworkCredential(Prefs.ProxyUser, Prefs.ProxyPass);
                    }
                }
                else
                {
                    wrq.Proxy = null;
                }

                // TODO: Handle timeouts and errors
                try
                {
                    wrs = wrq.GetResponse();
                    sr1 = new StreamReader(wrs.GetResponseStream(), Encoding.ASCII);

                    if ((wrs == null) || (sr1 == null))
                    {
                        throw new IOException("The web response or stream was null");
                    }
                }
                catch (WebException ExWeb)
                {
                    Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw WebException {1}.", Debug.FunctionName, ExWeb.Message));
                    Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start))); 
                    return;
                }
                catch (IOException ExIO)
                {
                    Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw IOException {1}.", Debug.FunctionName, ExIO.Message));
                    Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
                    return;
                }
                catch (Exception ex)
                {
                    Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw WebException {1}.", Debug.FunctionName, ex.Message));
                    Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
                    return;
                }

                HTMLparser pSummary = new HTMLparser();
                String sSummaryPage = sr1.ReadToEnd();
                pSummary.Init(sSummaryPage);

                // Locate the table
                HTMLchunk oChunk = null;

                // Parse until returned oChunk is null indicating we reached end of parsing
                while ((oChunk = pSummary.ParseNext()) != null)
                {
                    if (oChunk.sTag.ToLower() == "tr")
                    {
                        Protein p = new Protein();
                        while (((oChunk = pSummary.ParseNext()) != null) && (oChunk.sTag.ToLower() != "td"))
                            ; // Do nothing!
                        // Skip the empty attributes
                        oChunk = pSummary.ParseNext();
                        try
                        {
                            #region Parse Code for HTML Table
                            // Suck out the project number
                            p.ProjectNumber = Int32.Parse(oChunk.oHTML.ToString());

                            // Skip the closing tag, opening tags and attributes
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.ServerIP = oChunk.oHTML.ToString().Trim();

                            // Skip the closing tag, opening tags and attributes
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.WorkUnitName = oChunk.oHTML.ToString().Trim();

                            // Skip the closing tag, opening tags and attributes
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.NumAtoms = Int32.Parse(oChunk.oHTML.ToString());

                            // Skip the closing tag, opening tags and attributes
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.PreferredDays = Int32.Parse(oChunk.oHTML.ToString().Substring(0, oChunk.oHTML.IndexOf('.')).Trim());

                            // Skip the closing tag, opening tags and attributes
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            try
                            {
                                p.MaxDays = Int32.Parse(oChunk.oHTML.ToString().Substring(0, oChunk.oHTML.IndexOf('.')).Trim());
                            }
                            catch
                            {
                                p.MaxDays = 0;
                            }

                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.Credit = Int32.Parse(oChunk.oHTML.ToString().Substring(0, oChunk.oHTML.IndexOf('.')).Trim());

                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.Frames = Int32.Parse(oChunk.oHTML.ToString().Trim());

                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.Core = oChunk.oHTML.ToString();

                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.Description = oChunk.oParams["href"].ToString();

                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            oChunk = pSummary.ParseNext();
                            p.Contact = oChunk.oHTML.ToString();
                            #endregion

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
                            // Ignore this row of the table - unparseable
                            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} threw exception while parsing HTML: {1}", Debug.FunctionName, ex.Message));
                        }
                    }
                }
                if (Count > 0)
                {
                   OnProjectInfoUpdated(new ProjectInfoUpdatedEventArgs());
                }
            }

            SaveToTabDelimitedFile(_LocalProjectInfoFile);

            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} loaded {1} proteins from Stanford", Debug.FunctionName, Instance.Count));
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
            return;
        }

        /// <summary>
        /// Save the current protein collection to CSV file (for reload next execution)
        /// </summary>
        /// <param name="ExtraNFOFile">Fully qualified filename to write</param>
        public void SaveToTabDelimitedFile(String ExtraNFOFile)
        {
            DateTime Start = Debug.ExecStart;

            String[] CSVData = new String[this.Count];
            Int32 i = 0;

            foreach (KeyValuePair<Int32, Protein> kvp in this)
            {
            // Project Number, Server IP, Work Unit Name, Number of Atoms, Preferred (days),
            // Final Deadline (days), Credit, Frames, Code, Description, Contact

                CSVData[i++] = String.Format("{0}\t{1}\t{2}\t{3}\t{4}\t{5}\t{6}\t{7}\t{8}\t{9}\t{10}",
                                /*  0 */ kvp.Value.ProjectNumber,    /*  1 */ kvp.Value.ServerIP,
                                /*  2 */ kvp.Value.WorkUnitName,     /*  3 */ kvp.Value.NumAtoms,
                                /*  4 */ kvp.Value.PreferredDays,    /*  5 */ kvp.Value.MaxDays,
                                /*  6 */ kvp.Value.Credit,           /*  7 */ kvp.Value.Frames,
                                /*  8 */ kvp.Value.Core,             /*  9 */ kvp.Value.Description,
                                /* 10 */ kvp.Value.Contact);
           }

            File.WriteAllLines(_LocalProjectInfoFile, CSVData, Encoding.ASCII);
            Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
        }
    }
}
