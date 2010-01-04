/*
 * HFM.NET - Instance Collection Class
 * Copyright (C) 2006-2007 David Rawling
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
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.ComponentModel;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

using HFM.Framework;
using HFM.Helpers;
using HFM.Preferences;
using HFM.Proteins;
using HFM.Instrumentation;

namespace HFM.Instances
{
   public struct InstanceTotals
   {
      public double PPD;
      public double UPD;
      public Int32 TotalClients;
      public Int32 WorkingClients;
      public Int32 NonWorkingClients;
      public Int32 TotalCompletedUnits;
      public Int32 TotalFailedUnits;
   }

   public sealed class InstanceCollection : IDisposable
   {
      #region Events
      public event EventHandler CollectionChanged;
      public event EventHandler CollectionLoaded;
      public event EventHandler CollectionSaved;
      public event EventHandler InstanceAdded;
      public event EventHandler InstanceEdited;
      public event EventHandler InstanceRemoved;
      public event EventHandler InstanceRetrieved;
      public event EventHandler SelectedInstanceChanged;
      public event EventHandler DuplicatesFoundOrChanged;
      public event EventHandler OfflineLastChanged;
      
      public event EventHandler RefreshUserStatsData;
      #endregion
      
      #region Members
      /// <summary>
      /// Conversion factor - minutes to milli-seconds
      /// </summary>
      private const int MinToMillisec = 60000;

      /// <summary>
      /// Retrieval Timer Object (init 10 minutes)
      /// </summary>
      private readonly System.Timers.Timer workTimer = new System.Timers.Timer(600000);

      /// <summary>
      /// WebGen Timer Object (init 15 minutes)
      /// </summary>
      private readonly System.Timers.Timer webTimer = new System.Timers.Timer(900000);

      /// <summary>
      /// Local time that denotes when a full retrieve started (only accessed by the RetrieveInProgress property)
      /// </summary>
      private DateTime _RetrieveExecStart;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress (only accessed by the RetrieveInProgress property)
      /// </summary>
      private volatile bool _RetrievalInProgress = false;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _RetrievalInProgress; }
         private set
         {
            if (value)
            {
               _RetrieveExecStart = HfmTrace.ExecStart;
               _RetrievalInProgress = value;
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Total Retrieval Execution Time: {0}", HfmTrace.GetExecTime(_RetrieveExecStart)));
               _RetrievalInProgress = value;
            }
         }
      }
      
      /// <summary>
      /// Total Number of Fields (Columns) available to the DataGridView
      /// </summary>
      public const int NumberOfDisplayFields = 19;
      
      /// <summary>
      /// Main instance collection
      /// </summary>
      private readonly Dictionary<string, ClientInstance> _instanceCollection;

      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly SortableBindingList<DisplayInstance> _displayCollection;
      
      /// <summary>
      /// Currently Selected Client Instance
      /// </summary>
      private ClientInstance _SelectedInstance = null;
      
      /// <summary>
      /// List of Duplicate Project (R/C/G)
      /// </summary>
      private readonly List<string> _duplicateProjects;

      /// <summary>
      /// List of Duplicate Client User and Machine ID combinations
      /// </summary>
      private readonly List<string> _duplicateUserID;

      /// <summary>
      /// Internal filename
      /// </summary>
      private string _ConfigFilename = String.Empty;

      /// <summary>
      /// Internal variable storing whether New, Open, Quit should prompt for saving the config first
      /// </summary>
      private bool _ChangedAfterSave = false;

      /// <summary>
      /// Passive FTP Web Upload
      /// </summary>
      private bool _PassiveFtpWebUpload = true;
      #endregion

      #region CTOR
      /// <summary>
      /// Default Constructor
      /// </summary>
      public InstanceCollection()
      {
         _instanceCollection = new Dictionary<string, ClientInstance>();
         _displayCollection = new SortableBindingList<DisplayInstance>();
         _duplicateProjects = new List<string>();
         _duplicateUserID = new List<string>();

         // Hook up Background Timer Event Handler
         workTimer.Elapsed += bgWorkTimer_Tick;
         // Hook up WebGen Timer Event Handler
         webTimer.Elapsed += webGenTimer_Tick;

         // Hook up Protein Collection Updated Event Handler
         ProteinCollection.Instance.ProjectInfoUpdated += ProteinCollection_ProjectInfoUpdated;

         // Set Offline Clients Sort Flag
         OfflineClientsLast = PreferenceSet.Instance.OfflineLast;

         // Clear the Log File Cache Folder
         ClearCacheFolder();

         // Hook-up PreferenceSet Event Handlers
         PreferenceSet Prefs = PreferenceSet.Instance;
         Prefs.OfflineLastChanged += PreferenceSet_OfflineLastChanged;
         Prefs.TimerSettingsChanged += Prefs_TimerSettingsChanged;
         Prefs.DuplicateCheckChanged += PreferenceSet_DuplicateCheckChanged;
      }
      #endregion

      #region Event Wrappers
      private void OnCollectionChanged(EventArgs e)
      {
         if (CollectionChanged != null)
         {
            CollectionChanged(this, e);
         }
      }

      private void OnCollectionLoaded(EventArgs e)
      {
         if (CollectionLoaded != null)
         {
            CollectionLoaded(this, e);
         }
      }

      private void OnCollectionSaved(EventArgs e)
      {
         if (CollectionSaved != null)
         {
            CollectionSaved(this, e);
         }
      }

      private void OnInstanceAdded(EventArgs e)
      {
         if (InstanceAdded != null)
         {
            InstanceAdded(this, e);
         }
      }

      private void OnInstanceEdited(EventArgs e)
      {
         if (InstanceEdited != null)
         {
            InstanceEdited(this, e);
         }
      }

      private void OnInstanceRemoved(EventArgs e)
      {
         if (InstanceRemoved != null)
         {
            InstanceRemoved(this, e);
         }
      }

      private void OnInstanceRetrieved(EventArgs e)
      {
         if (InstanceRetrieved != null)
         {
            InstanceRetrieved(this, e);
         }
      }

      private void OnDuplicatesFoundOrChanged(EventArgs e)
      {
         if (DuplicatesFoundOrChanged != null)
         {
            DuplicatesFoundOrChanged(this, e);
         }
      }

      private void OnOfflineLastChanged(EventArgs e)
      {
         if (OfflineLastChanged != null)
         {
            OfflineLastChanged(this, e);
         }
      }

      /// <summary>
      /// Force Raises the SelectedInstanceChanged event.
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
      public void RaiseSelectedInstanceChanged()
      {
         OnSelectedInstanceChanged(EventArgs.Empty);
      }

      /// <summary>
      /// Raises the SelectedInstanceChanged event.
      /// </summary>
      private void OnSelectedInstanceChanged(EventArgs e)
      {
         if (SelectedInstanceChanged != null)
         {
            SelectedInstanceChanged(this, e);
         }
      }

      private void OnRefreshUserStatsData(EventArgs e)
      {
         if (RefreshUserStatsData != null)
         {
            RefreshUserStatsData(this, e);
         }
      }
      #endregion

      #region Properties
      /// <summary>
      /// Client Instance Collection
      /// </summary>
      public Dictionary<string, ClientInstance> Instances
      {
         get { return _instanceCollection; }
      }

      /// <summary>
      /// Currently Selected Client Instance
      /// </summary>
      public ClientInstance SelectedInstance
      {
         get { return _SelectedInstance; }
         private set 
         { 
            if (_SelectedInstance != value)
            {
               _SelectedInstance = value;
               OnSelectedInstanceChanged(EventArgs.Empty);
            }
         }
      }

      /// <summary>
      /// Client Configuration Filename
      /// </summary>
      public string ConfigFilename
      {
         get { return _ConfigFilename; }
      }
      
      /// <summary>
      /// Client Configuration has Filename defined
      /// </summary>
      public bool HasConfigFilename
      {
         get { return _ConfigFilename.Length > 0; }
      }

      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      public bool ChangedAfterSave
      {
         get { return _ChangedAfterSave; }
      }

      /// <summary>
      /// Client Instance Count
      /// </summary>
      public int Count
      {
         get { return _instanceCollection.Count; }
      }

      /// <summary>
      /// Returns True if Client Instance Count is greater than 0
      /// </summary>
      public bool HasInstances
      {
         get { return Count > 0; }
      }

      /// <summary>
      /// Tells the SortableBindingList whether to sort Offline Clients Last
      /// </summary>
      public bool OfflineClientsLast
      {
         get { return _displayCollection.OfflineClientsLast; }
         set 
         { 
            if (_displayCollection.OfflineClientsLast != value)
            {
               _displayCollection.OfflineClientsLast = value;
               OnOfflineLastChanged(EventArgs.Empty);
            }
         }
      }
      #endregion

      #region Read and Write Xml
      /// <summary>
      /// Loads a collection of Host Instances from disk
      /// </summary>
      /// <param name="xmlDocName">Filename (verbatim) to load data from - User AppData Path is prepended
      /// if the path does not start with either ?: or \\</param>
      public void FromXml(string xmlDocName)
      {
         if (String.IsNullOrEmpty(xmlDocName))
         {
            throw new ArgumentException("Argument 'xmlDocName' cannot be a null or empty string.", "xmlDocName");
         }

         _ChangedAfterSave = false;

         XmlDocument xmlData = new XmlDocument();

         // Load the XML file
         if (Path.IsPathRooted(xmlDocName))
         {
            xmlData.Load(xmlDocName);
         }
         else
         {
            xmlData.Load(Path.Combine(PreferenceSet.AppPath, xmlDocName));
         }

         // xmlData now contains the collection of Nodes. Hopefully.
         xmlData.RemoveChild(xmlData.ChildNodes[0]);

         foreach (XmlNode xn in xmlData.ChildNodes[0])
         {
            string InstanceType = xn.SelectSingleNode("HostType").InnerText;

            if (Enum.IsDefined(typeof(InstanceType), InstanceType))
            {
               InstanceType type = (InstanceType)Enum.Parse(typeof(InstanceType), InstanceType, false);
               ClientInstance instance = new ClientInstance(type);
               instance.FromXml(xn);
               UnitInfo restoreUnitInfo = UnitInfoCollection.Instance.RetrieveUnitInfo(instance.InstanceName, instance.Path);
               if (restoreUnitInfo != null)
               {
                  instance.RestoreUnitInfo(restoreUnitInfo);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Restored UnitInfo for Instance '{1}'.", HfmTrace.FunctionName, instance.InstanceName));
               }
               Add(instance, false);
            }
            else
            {
               MessageBox.Show(String.Format("Client {0} failed to load.", xn.Attributes["Name"].ChildNodes[0].Value));
            }
         }

         if (HasInstances)
         {
            _ConfigFilename = xmlDocName;

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and WebGen Timers
            SetTimerState();

            OnCollectionLoaded(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Saves the current collection of Host Instances to disk
      /// </summary>
      public void ToXml()
      {
         ToXml(ConfigFilename);
      }

      /// <summary>
      /// Saves the current collection of Host Instances to disk
      /// </summary>
      /// <param name="xmlDocName">Filename (verbatim) to save data to - User AppData Path is prepended
      /// if the path does not start with either ?: or \\</param>
      public void ToXml(string xmlDocName)
      {
         if (String.IsNullOrEmpty(xmlDocName))
         {
            throw new ArgumentException("Argument 'xmlDocName' cannot be a null or empty string.", "xmlDocName");
         }

         XmlDocument xmlData = new XmlDocument();

         // Create the XML Declaration (well formed)
         XmlDeclaration xmlDeclaration = xmlData.CreateXmlDeclaration("1.0", "utf-8", null);
         xmlData.InsertBefore(xmlDeclaration, xmlData.DocumentElement);

         // Create the root element
         XmlElement xmlRoot = xmlData.CreateElement("Hosts");

         lock (_instanceCollection)
         {
            // Loop through the collection and serialize the lot
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               XmlDocument xmlDoc = instance.ToXml();
               foreach (XmlNode xn in xmlDoc.ChildNodes)
               {
                  xmlRoot.AppendChild(xmlData.ImportNode(xn, true));
               }
            }
         }
         
         xmlData.AppendChild(xmlRoot);

         // Save the XML stream to the file
         if (Path.IsPathRooted(xmlDocName))
         {
            xmlData.Save(xmlDocName);
            _ConfigFilename = xmlDocName;
         }
         else
         {
            throw new ArgumentException(String.Format("Argument 'xmlDocName' must be a rooted path.  Given path '{0}'.", xmlDocName), "xmlDocName");
         }
         
         _ChangedAfterSave = false;
         OnCollectionSaved(EventArgs.Empty);
      }
      #endregion

      #region FahMon Import Support
      /// <summary>
      /// Read FahMon ClientsTab.txt file and import new instance collection
      /// </summary>
      /// <param name="filename">Path of ClientsTab.txt to import</param>
      public void FromFahMonClientsTab(string filename)
      {
         _ChangedAfterSave = false;
         
         StreamReader fileStream = null;
         try
         {
            // Open File
            fileStream = File.OpenText(filename);

            // Reader Loop
            while (fileStream.Peek() != -1)
            {
               // Get the line and remove whitespace
               string line = fileStream.ReadLine();
               line = line.Trim();

               // Check for commented or empty line
               if (String.IsNullOrEmpty(line) == false && line.StartsWith("#") == false)
               {
                  // Tokenize the line
                  string[] tokens = line.Split(new char[] {'\t'});

                  if (tokens.Length > 1) // we should have at least name and path
                  {
                     ClientInstance instance = GetNewInstance(tokens);
                     if (instance != null)
                     {
                        // Check for Client is on Virtual Machine setting
                        if (tokens.Length > 3)
                        {
                           if (tokens[3].Equals("*"))
                           {
                              instance.ClientIsOnVirtualMachine = true;
                           }
                        }
                        Add(instance, false);
                        HfmTrace.WriteToHfmConsole(TraceLevel.Info,
                                                   String.Format("{0} Added FahMon Instance Name: {1}.", HfmTrace.FunctionName, instance.InstanceName));
                     }
                     else
                     {
                        HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                                   String.Format("{0} Failed to add FahMon Instance: {1}.", HfmTrace.FunctionName, line));
                     }
                  }
                  else
                  {
                     HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                                String.Format("{0} Failed to add FahMon Instance (not tab delimited): {1}.", HfmTrace.FunctionName, line));
                  }
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            if (fileStream != null)
            {
               fileStream.Close();
            }
         }

         if (HasInstances)
         {
            _ChangedAfterSave = true;

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and WebGen Timers
            SetTimerState();

            OnCollectionLoaded(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Inspects tokens gathered from FahMon ClientsTab.txt line and attempts to
      /// create an HFM ClientInstance object based on those tokens
      /// </summary>
      /// <param name="tokens">Tokenized String (String Array)</param>
      private static ClientInstance GetNewInstance(string[] tokens)
      {
         // Get the instance name token and validate
         string instanceName = tokens[0].Replace("\"", String.Empty);
         if (StringOps.ValidateInstanceName(instanceName) == false)
         {
            // Remove illegal characters
            instanceName = StringOps.CleanInstanceName(instanceName);
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} Cleaned FahMon Instance Name: {1}.", HfmTrace.FunctionName, instanceName));
         }

         // Get the instance path token
         string instancePath = tokens[1].Replace("\"", String.Empty);
         Match matchURL = StringOps.MatchHttpOrFtpUrl(instancePath);
         Match matchFTPUserPass = StringOps.MatchFtpWithUserPassUrl(instancePath);

         // Declare instance variable
         ClientInstance instance = null;

         if (matchURL.Success) // we have a valid URL
         {
            if (instancePath.StartsWith("http"))
            {
               instance = new ClientInstance(InstanceType.HTTPInstance);
               instance.InstanceName = instanceName;
               instance.Path = instancePath;
            }
            else if (instancePath.StartsWith("ftp"))
            {
               instance = new ClientInstance(InstanceType.FTPInstance);
               instance.InstanceName = instanceName;
               instance.Server = matchURL.Result("${domain}");
               instance.Path = matchURL.Result("${file}");
            }
         }
         else if (matchFTPUserPass.Success) // we have a valid FTP with User Pass
         {
            instance = new ClientInstance(InstanceType.FTPInstance);
            instance.InstanceName = instanceName;
            instance.Server = matchFTPUserPass.Result("${domain}");
            instance.Path = matchFTPUserPass.Result("${file}");
            instance.Username = matchFTPUserPass.Result("${username}");
            instance.Password = matchFTPUserPass.Result("${password}");
         }
         else // try to validate as a path instance
         {
            if (StringOps.ValidatePathInstancePath(instancePath))
            {
               instance = new ClientInstance(InstanceType.PathInstance);
               instance.InstanceName = instanceName;
               instance.Path = instancePath;
            }
            else if (StringOps.ValidatePathInstancePath(instancePath += Path.DirectorySeparatorChar))
            {
               instance = new ClientInstance(InstanceType.PathInstance);
               instance.InstanceName = instanceName;
               instance.Path = instancePath;
            }
         }
         
         return instance;
      }
      #endregion

      #region List Like Implementation (eventually implement IList or ICollection)
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      public void Add(ClientInstance Instance)
      {
         Add(Instance, true);
      }
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      /// <param name="FireAddedEvent">Specifies whether this call fires the InstanceAdded Event</param>
      private void Add(ClientInstance Instance, bool FireAddedEvent)
      {
         Add(Instance.InstanceName, Instance, FireAddedEvent);
      }

      /// <summary>
      /// Add an Instance with Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      /// <param name="Instance">Client Instance</param>
      public void Add(string Key, ClientInstance Instance)
      {
         Add(Key, Instance, true);
      }

      /// <summary>
      /// Add an Instance with Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      /// <param name="Instance">Client Instance</param>
      /// <param name="FireAddedEvent">Specifies whether this call fires the InstanceAdded Event</param>
      private void Add(string Key, ClientInstance Instance, bool FireAddedEvent)
      {
         _instanceCollection.Add(Key, Instance);
         OnCollectionChanged(EventArgs.Empty);
         
         if (FireAddedEvent)
         {
            RetrieveSingleClient(Instance);

            _ChangedAfterSave = true;
            OnInstanceAdded(EventArgs.Empty);
         }
      }
      
      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="PreviousName">Previous Client Instance Name</param>
      /// <param name="PreviousPath">Previous Client Instance Path</param>
      /// <param name="Instance">Client Instance</param>
      public void Edit(string PreviousName, string PreviousPath, ClientInstance Instance)
      {
         // if the host key changed
         if (PreviousName != Instance.InstanceName)
         {
            UpdateDisplayInstanceName(PreviousName, Instance.InstanceName);
            Remove(PreviousName, false);
            Add(Instance, false);
            
            ProteinBenchmarkCollection.Instance.UpdateInstanceName(new BenchmarkClient(PreviousName, Instance.Path), Instance.InstanceName);
         }
         // if the path changed, update the paths in the benchmark collection
         if (PreviousPath != Instance.Path)
         {
            ProteinBenchmarkCollection.Instance.UpdateInstancePath(new BenchmarkClient(Instance.InstanceName, PreviousPath), Instance.Path);
         }
         
         RetrieveSingleClient(Instance);

         _ChangedAfterSave = true;
         OnInstanceEdited(EventArgs.Empty);
      }
      
      /// <summary>
      /// Remove an Instance by Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      public void Remove(string Key)
      {
         Remove(Key, true);
      }

      /// <summary>
      /// Remove an Instance by Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      /// <param name="FireRemovedEvent">Specifies whether this call fires the InstanceRemoved Event</param>
      private void Remove(string Key, bool FireRemovedEvent)
      {
         _instanceCollection.Remove(Key);
         DisplayInstance findInstance = FindDisplayInstance(_displayCollection, Key);
         _displayCollection.Remove(findInstance);
         OnCollectionChanged(EventArgs.Empty);
         
         if (FireRemovedEvent)
         {
            _ChangedAfterSave = true;
            OnInstanceRemoved(EventArgs.Empty);

            FindDuplicates();
         }
      }
      
      /// <summary>
      /// Clear All Instance Data
      /// </summary>
      public void Clear()
      {
         if (HasInstances)
         {
            SaveCurrentUnitInfo();
         }
      
         _instanceCollection.Clear();
         _displayCollection.Clear();
         _duplicateProjects.Clear();
         _duplicateUserID.Clear();

         // new config filename
         _ConfigFilename = String.Empty;
         // collection has not changed
         _ChangedAfterSave = false;
         // This will disable the timers, we have no hosts
         SetTimerState();
         
         OnCollectionChanged(EventArgs.Empty);
      }

      /// <summary>
      /// Collection Contains Name
      /// </summary>
      /// <param name="InstanceName">Instance Name to search for</param>
      public bool ContainsName(string InstanceName)
      {
         ClientInstance findInstance = new List<ClientInstance>(_instanceCollection.Values).Find(delegate(ClientInstance instance)
                                                                {
                                                                   return instance.InstanceName.ToUpper() == InstanceName.ToUpper();
                                                                });
         return findInstance != null;
      }
      #endregion

      #region Retrieval Logic
      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void bgWorkTimer_Tick(object sender, EventArgs e)
      {
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Running Background Timer...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void webGenTimer_Tick(object sender, EventArgs e)
      {
         if (PreferenceSet.Instance.GenerateWeb == false) return;

         DateTime Start = HfmTrace.ExecStart;

         if (webTimer.Enabled)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping WebGen Timer Loop");
         }
         webTimer.Stop();

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Starting WebGen.", HfmTrace.FunctionName));

         PreferenceSet Prefs = PreferenceSet.Instance;
         Match match = StringOps.MatchFtpWithUserPassUrl(Prefs.WebRoot);
         
         try
         {
            ClientInstance[] CurrentInstances = GetCurrentInstanceArray();
            if (match.Success)
            {
               XMLGen.DoHtmlGeneration(Path.GetTempPath(), CurrentInstances);

               string Server = match.Result("${domain}");
               string FtpPath = match.Result("${file}");
               string Username = match.Result("${username}");
               string Password = match.Result("${password}");

               try
               {
                  XMLGen.DoWebFtpUpload(Server, FtpPath, Username, Password, CurrentInstances, _PassiveFtpWebUpload);
               }
               catch (WebException ex)
               {
                  //TODO: Relying on this exception message is bad... if the app is ever localized, this message
                  //      will be in the localized language.  Should probably add a setting to just turn this
                  //      on or off.  Will do so at a later time.
                  if (ex.Message.Contains("The remote server returned an error: 227 Entering Passive Mode"))
                  {
                     HfmTrace.WriteToHfmConsole(String.Format("{0} Passive FTP transfer failed... trying Non-Passive transfer.", HfmTrace.FunctionName));
                     XMLGen.DoWebFtpUpload(Server, FtpPath, Username, Password, CurrentInstances, false);
                     _PassiveFtpWebUpload = false;
                  }
                  else
                  {
                     throw;
                  }
               }
            }
            else
            {
               // Create the web folder (just in case)
               if (Directory.Exists(Prefs.WebRoot) == false)
               {
                  Directory.CreateDirectory(Prefs.WebRoot);
               }

               // Copy the CSS file to the output directory
               string sCSSFileName = Path.Combine(Path.Combine(PreferenceSet.AppPath, "CSS"), Prefs.CSSFileName);
               if (File.Exists(sCSSFileName))
               {
                  File.Copy(sCSSFileName, Path.Combine(Prefs.WebRoot, Prefs.CSSFileName), true);
               }

               XMLGen.DoHtmlGeneration(Prefs.WebRoot, CurrentInstances);
               
               foreach (ClientInstance Instance in CurrentInstances)
               {
                  string CachedFAHlogPath = Path.Combine(PreferenceSet.CacheDirectory, Instance.CachedFAHLogName);
                  if (File.Exists(CachedFAHlogPath))
                  {
                     File.Copy(CachedFAHlogPath, Path.Combine(Prefs.WebRoot, Instance.CachedFAHLogName), true);
                  }
               }
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         finally
         {
            StartWebGenTimer();
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
         }
      }

      /// <summary>
      /// Forces a log and screen refresh when the Stanford info is updated
      /// </summary>
      private void ProteinCollection_ProjectInfoUpdated(object sender, EventArgs e)
      {
         // Do Retrieve on all clients after Project Info is updated (this is confirmed needed)
         QueueNewRetrieval();
      }

      /// <summary>
      /// Stick each Instance in the background thread queue to retrieve the info for a given Instance
      /// </summary>
      public void QueueNewRetrieval()
      {
         // don't fire this process twice
         if (RetrievalInProgress)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Retrieval Already In Progress...", HfmTrace.FunctionName));
            return;
         }
         
         // only fire if there are Hosts
         if (HasInstances)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Background Timer Loop");
            workTimer.Stop();

            // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
            new MethodInvoker(DoRetrievalWrapper).BeginInvoke(null, null);
         }
      }
      
      /// <summary>
      /// Wraps the DoRetrieval function on a seperate thread and fires post retrieval processes
      /// </summary>
      private void DoRetrievalWrapper()
      {
         try
         {
            // set full retrieval flag
            RetrievalInProgress = true;

            // fire the actual retrieval thread
            IAsyncResult async = new MethodInvoker(DoRetrieval).BeginInvoke(null, null);
            // wait for completion
            async.AsyncWaitHandle.WaitOne();

            PreferenceSet Prefs = PreferenceSet.Instance;

            // run post retrieval processes
            if (Prefs.GenerateWeb && Prefs.WebGenAfterRefresh)
            {
               // do a web gen
               webGenTimer_Tick(null, null);
            }

            if (Prefs.ShowUserStats)
            {
               OnRefreshUserStatsData(EventArgs.Empty);
            }

            // Enable the data retrieval timer
            if (Prefs.SyncOnSchedule)
            {
               StartBackgroundTimer();
            }
         }
         finally
         {
            // clear full retrieval flag
            RetrievalInProgress = false;
         }
      }

      /// <summary>
      /// Do a full retrieval operation
      /// </summary>
      public void DoRetrieval()
      {
         // get flag synchronous or asynchronous - we don't want this flag to change on us
         // in the middle of a retrieve, so grab it now and use the local copy
         bool Synchronous = PreferenceSet.Instance.SyncOnLoad;

         // copy the current instance keys into local array
         int numInstances = _instanceCollection.Count;
         string[] instanceKeys = new string[numInstances];
         _instanceCollection.Keys.CopyTo(instanceKeys, 0);

         List<WaitHandle> waitHandleList = new List<WaitHandle>();
         for (int i = 0; i < numInstances;)
         {
            waitHandleList.Clear();
            // loop through the instances (can only handle up to 64 wait handles at a time)
            // limiting to 20 to reduce threadpool starvation
            for (int j = 0; j < 20 && i < numInstances; j++)
            {
               // try to get the key value from the collection, if the value is not found then
               // the user removed a client in the middle of a retrieve process, ignore the key
               ClientInstance Instance;
               if (_instanceCollection.TryGetValue(instanceKeys[i], out Instance))
               {
                  if (Synchronous) // do the individual retrieves on a single thread
                  {
                     RetrieveInstance(Instance);
                  }
                  else // fire individual threads to do the their own retrieve simultaneously
                  {
                     IAsyncResult async = QueueNewRetrieval(Instance);

                     // get the wait handle for each invoked delegate
                     waitHandleList.Add(async.AsyncWaitHandle);
                  }
               }

               i++; // increment the outer loop counter
            }

            if (Synchronous == false)
            {
               WaitHandle[] waitHandles = waitHandleList.ToArray();
               // wait for all invoked threads to complete
               WaitHandle.WaitAll(waitHandles);
            }
         }

         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         FindDuplicates();

         // Save the benchmark collection
         ProteinBenchmarkCollection.Serialize();
      }

      /// <summary>
      /// Delegate used for asynchronous instance retrieval
      /// </summary>
      /// <param name="Instance"></param>
      private delegate void RetrieveInstanceDelegate(ClientInstance Instance);

      /// <summary>
      /// Stick this Instance in the background thread queue to retrieve the info for the given Instance
      /// </summary>
      private IAsyncResult QueueNewRetrieval(ClientInstance Instance)
      {
         return new RetrieveInstanceDelegate(RetrieveInstance).BeginInvoke(Instance, null, null);
      }

      /// <summary>
      /// Stub to execute retrieve and refresh display
      /// </summary>
      /// <param name="Instance"></param>
      private void RetrieveInstance(ClientInstance Instance)
      {
         if (Instance.RetrievalInProgress == false)
         {
            Instance.Retrieve();
            // This event should signal the UI to update
            OnInstanceRetrieved(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="InstanceName">Client Instance Name</param>
      public void RetrieveSingleClient(string InstanceName)
      {
         RetrieveSingleClient(_instanceCollection[InstanceName]);
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      public void RetrieveSingleClient(ClientInstance Instance)
      {
         // fire the actual retrieval thread
         new RetrieveInstanceDelegate(DoSingleClientRetieval).BeginInvoke(Instance, null, null);
      }

      /// <summary>
      /// Do a single retrieval operation on the given Client Instance
      /// </summary>
      /// <param name="Instance">Client Instance</param>
      private void DoSingleClientRetieval(ClientInstance Instance)
      {
         if (Instance.RetrievalInProgress == false)
         {
            IAsyncResult async = QueueNewRetrieval(Instance);
            async.AsyncWaitHandle.WaitOne();

            // check for clients with duplicate Project (Run, Clone, Gen) or UserID
            FindDuplicates();

            // Save the benchmark collection
            ProteinBenchmarkCollection.Serialize();
         }
      }

      /// <summary>
      /// Disable and enable the background work timers
      /// </summary>
      public void SetTimerState()
      {
         // Disable timers if no hosts
         if (HasInstances == false)
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "No Hosts - Stopping Both Background Timer Loops");
            workTimer.Stop();
            webTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (PreferenceSet.Instance.SyncOnSchedule)
         {
            if (RetrievalInProgress == false)
            {
               StartBackgroundTimer();
            }
         }
         else
         {
            if (workTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Background Timer Loop");
               workTimer.Stop();
            }
         }

         // Enable the web generation timer
         if (PreferenceSet.Instance.GenerateWeb && PreferenceSet.Instance.WebGenAfterRefresh == false)
         {
            StartWebGenTimer();
         }
         else
         {
            if (webTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping WebGen Timer Loop");
               webTimer.Stop();
            }
         }
      }

      /// <summary>
      /// Starts Retrieval Timer Loop
      /// </summary>
      private void StartBackgroundTimer()
      {
         workTimer.Interval = Convert.ToInt32(PreferenceSet.Instance.SyncTimeMinutes) * MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Background Timer Loop: {0} Minutes",
                                                                    PreferenceSet.Instance.SyncTimeMinutes));
         workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         if (PreferenceSet.Instance.GenerateWeb && PreferenceSet.Instance.WebGenAfterRefresh == false)
         {
            webTimer.Interval = Convert.ToInt32(PreferenceSet.Instance.GenerateInterval) * MinToMillisec;
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting WebGen Timer Loop: {0} Minutes",
                                                                       PreferenceSet.Instance.GenerateInterval));
            webTimer.Start();
         }
      }
      #endregion

      #region Save UnitInfo Collection
      /// <summary>
      /// Serialize the Current UnitInfo Objects to disk
      /// </summary>
      public void SaveCurrentUnitInfo()
      {
         // If no clients loaded, stub out
         if (HasInstances == false) return;
         
         DateTime Start = HfmTrace.ExecStart;

         UnitInfoCollection collection = UnitInfoCollection.Instance;

         collection.Clear();
         
         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               // Don't save the UnitInfo object if the contained Project is Unknown
               if (instance.CurrentUnitInfo.ProjectIsUnknown == false)
               {
                  collection.Add(instance.CurrentUnitInfo);
               }
            }
         }

         UnitInfoCollection.Serialize();

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, Start);
      }
      #endregion

      #region Binding Support
      /// <summary>
      /// Get Display Collection For Binding
      /// </summary>
      /// <returns>List of Display Instances</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public BindingList<DisplayInstance> GetDisplayCollection()
      {
         RefreshDisplayCollection();
         return _displayCollection;
      }

      /// <summary>
      /// Refresh the Display Collection from the Instance Collection
      /// </summary>
      public void RefreshDisplayCollection()
      {
         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               DisplayInstance findInstance = FindDisplayInstance(_displayCollection, instance.InstanceName);
               if (findInstance != null)
               {
                  findInstance.Load(instance);
               }
               else
               {
                  DisplayInstance newInstance = new DisplayInstance();
                  newInstance.Load(instance);
                  _displayCollection.Add(newInstance);
               }
            }
         }
      }

      /// <summary>
      /// Update an Instance Name in the Display Collection
      /// </summary>
      /// <param name="oldName">Old Instance Name</param>
      /// <param name="newName">New Instance Name</param>
      public void UpdateDisplayInstanceName(string oldName, string newName)
      {
         DisplayInstance findInstance = FindDisplayInstance(_displayCollection, oldName);
         findInstance.UpdateName(newName);
      }
      #endregion

      #region Duplicate UserID and Project Support
      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public void FindDuplicates() // Issue 19
      {
         int previousDuplicateUserIDCount = _duplicateUserID.Count;
         int previousDuplicateProjectsCount = _duplicateProjects.Count;

         InstanceCollectionHelpers.FindDuplicates(_duplicateUserID, _duplicateProjects, GetCurrentInstanceArray());

         if (_duplicateUserID.Count > 0 || _duplicateProjects.Count > 0 ||
             _duplicateUserID.Count != previousDuplicateUserIDCount || _duplicateProjects.Count != previousDuplicateProjectsCount)
         {
            OnDuplicatesFoundOrChanged(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Look for given Project (R/C/G) in list of Duplicates
      /// </summary>
      /// <param name="PRCG">The Project (R/C/G) to look for</param>
      public bool IsDuplicateProject(string PRCG)
      {
         return _duplicateProjects.Contains(PRCG);
      }

      /// <summary>
      /// Look for given User and Machine ID combination in list of Duplicates
      /// </summary>
      /// <param name="UserAndMachineID">The User and Machine ID to look for</param>
      public bool IsDuplicateUserAndMachineID(string UserAndMachineID)
      {
         return _duplicateUserID.Contains(UserAndMachineID);
      }
      #endregion

      #region Helper Functions
      public void SetCurrentInstance(IList SelectedClients)
      {
         if (SelectedClients.Count > 0)
         {
            Debug.Assert(SelectedClients.Count == 1);
            
            if (SelectedClients[0] is DataGridViewRow)
            {
               object NameColumnValue = ((DataGridViewRow) SelectedClients[0]).Cells["Name"].Value;
               if (NameColumnValue != null)
               {
                  string InstanceName = NameColumnValue.ToString();
                  ClientInstance Instance;
                  if (Instances.TryGetValue(InstanceName, out Instance))
                  {
                     SelectedInstance = Instance;
                  }
                  else
                  {
                     SelectedInstance = null;
                  }
               }
               else
               {
                  SelectedInstance = null;
               }
            }
         }
         else
         {
            SelectedInstance = null;
         }
      }

      /// <summary>
      /// Get Array Representation of Current Client Instance objects in Collection
      /// </summary>
      public ClientInstance[] GetCurrentInstanceArray()
      {
         if (Count > 0)
         {
            ClientInstance[] instances = new ClientInstance[Count];
            _instanceCollection.Values.CopyTo(instances, 0);

            return instances;
         }

         return null;
      }
      
      /// <summary>
      /// Get Totals for all loaded Client Instances
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public InstanceTotals GetInstanceTotals()
      {
         return InstanceCollectionHelpers.GetInstanceTotals(GetCurrentInstanceArray());
      }

      /// <summary>
      /// Finds the DisplayInstance by Key (Instance Name)
      /// </summary>
      /// <param name="collection">DisplayIntance Collection</param>
      /// <param name="Key">Instance Name</param>
      /// <returns></returns>
      private static DisplayInstance FindDisplayInstance(IEnumerable<DisplayInstance> collection, string Key)
      {
         return new List<DisplayInstance>(collection).Find(delegate(DisplayInstance displayInstance)
                                                            {
                                                               return displayInstance.Name == Key;
                                                            });
      }

      /// <summary>
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private static void ClearCacheFolder()
      {
         DateTime Start = HfmTrace.ExecStart;

         string cacheFolder = Path.Combine(PreferenceSet.Instance.AppDataPath,
                                           PreferenceSet.Instance.CacheFolder);

         DirectoryInfo di = new DirectoryInfo(cacheFolder);
         if (di.Exists == false)
         {
            di.Create();
         }
         else
         {
            foreach (FileInfo fi in di.GetFiles())
            {
               try
               {
                  fi.Delete();
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format("Failed to Clear Cache File '{0}'.", fi.Name), ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, Start);
      }

      /// <summary>
      /// Sets OfflineLast Property on ClientInstances Collection
      /// </summary>
      private void PreferenceSet_OfflineLastChanged(object sender, EventArgs e)
      {
         OfflineClientsLast = PreferenceSet.Instance.OfflineLast;
      }

      /// <summary>
      /// Checks for Duplicates after Duplicate Check Preferences have changed
      /// </summary>
      private void PreferenceSet_DuplicateCheckChanged(object sender, EventArgs e)
      {
         FindDuplicates(); // Issue 81
      }

      private void Prefs_TimerSettingsChanged(object sender, EventArgs e)
      {
         SetTimerState();
      }
      #endregion
      
      #region IDisposable Members
      /// <summary>
      /// Performs application-defined tasks associated with freeing, releasing, or
      /// resetting unmanaged resources.
      /// </summary>
      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      /// <summary>
      /// Disposes the object.
      /// </summary>
      private void Dispose(bool disposing)
      {
         if (false == _disposed)
         {
            // clean native resources        
            
            if (disposing)
            {
               // clean managed resources
               workTimer.Dispose();
               webTimer.Dispose();
            }

            _disposed = true;
         }
      }

      private bool _disposed;
      #endregion
   }
}
