/*
 * HFM.NET - Instance Collection Class
 * Copyright (C) 2006-2007 David Rawling
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
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using HFM.Framework;
using HFM.Helpers;
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
      public Int32 TotalRunCompletedUnits;
      public Int32 TotalRunFailedUnits;
      public Int32 TotalClientCompletedUnits;
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
      public event EventHandler FindDuplicatesComplete;
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
      /// Web Generation Timer Object (init 15 minutes)
      /// </summary>
      private readonly System.Timers.Timer webTimer = new System.Timers.Timer(900000);

      /// <summary>
      /// Local time that denotes when a full retrieve started (only accessed by the RetrieveInProgress property)
      /// </summary>
      private DateTime _RetrieveExecStart;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress (only accessed by the RetrieveInProgress property)
      /// </summary>
      private volatile bool _RetrievalInProgress;
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
               _RetrievalInProgress = true;
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Total Retrieval Execution Time: {0}", HfmTrace.GetExecTime(_RetrieveExecStart)));
               _RetrievalInProgress = false;
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
      private ClientInstance _SelectedInstance;
      
      /// <summary>
      /// List of Duplicate Project (R/C/G)
      /// </summary>
      private readonly List<string> _duplicateProjects;

      /// <summary>
      /// List of Duplicate Client User and Machine ID combinations
      /// </summary>
      private readonly List<string> _duplicateUserId;

      /// <summary>
      /// Internal filename
      /// </summary>
      private string _ConfigFilename = String.Empty;

      /// <summary>
      /// Internal variable storing whether New, Open, Quit should prompt for saving the config first
      /// </summary>
      private bool _ChangedAfterSave;

      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private NetworkOps _networkOps;
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _Prefs;

      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinCollection _proteinCollection;

      /// <summary>
      /// Protein Collection Interface
      /// </summary>
      private readonly IProteinBenchmarkContainer _benchmarkContainer;
      
      /// <summary>
      /// UnitInfo Container Interface
      /// </summary>
      private readonly IUnitInfoContainer _unitInfoContainer;
      #endregion

      #region CTOR
      /// <summary>
      /// Default Constructor
      /// </summary>
      public InstanceCollection(IPreferenceSet prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer)
      {
         _Prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         
         _unitInfoContainer = InstanceProvider.GetInstance<IUnitInfoContainer>();
         _unitInfoContainer.Read();
      
         _instanceCollection = new Dictionary<string, ClientInstance>();
         _displayCollection = new SortableBindingList<DisplayInstance>();
         _duplicateProjects = new List<string>();
         _duplicateUserId = new List<string>();

         // Hook up Retrieval Timer Event Handler
         workTimer.Elapsed += bgWorkTimer_Tick;
         // Hook up Web Generation Timer Event Handler
         webTimer.Elapsed += webGenTimer_Tick;

         // Hook up Protein Collection Updated Event Handler
         InstanceProvider.GetInstance<IProteinCollection>().Downloader.ProjectInfoUpdated += ProteinCollection_ProjectInfoUpdated;

         // Set Offline Clients Sort Flag
         OfflineClientsLast = _Prefs.GetPreference<bool>(Preference.OfflineLast);

         // Clear the Log File Cache Folder
         ClearCacheFolder();

         // Hook-up PreferenceSet Event Handlers
         _Prefs.OfflineLastChanged += PreferenceSet_OfflineLastChanged;
         _Prefs.TimerSettingsChanged += Prefs_TimerSettingsChanged;
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

      private void OnFindDuplicatesComplete(EventArgs e)
      {
         if (FindDuplicatesComplete != null)
         {
            FindDuplicatesComplete(this, e);
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
      
         if (Path.IsPathRooted(xmlDocName) == false)
         {
            xmlDocName = Path.Combine(_Prefs.ApplicationPath, xmlDocName);
         }

         var serializer = new ClientInstanceXmlSerializer();
         var collectionDataInterface = new InstanceCollectionDataInterface(GetCurrentInstanceArray());
         serializer.DataInterface = collectionDataInterface;
         serializer.Deserialize(xmlDocName);
         
         var builder = new ClientInstanceFactory(_Prefs, _proteinCollection, _benchmarkContainer);
         ICollection<ClientInstance> instances = builder.HandleImportResults(collectionDataInterface.Settings);

         foreach (ClientInstance instance in instances)
         {
            var restoreUnitInfo = _unitInfoContainer.RetrieveUnitInfo(instance);
            if (restoreUnitInfo != null)
            {
               instance.RestoreUnitInfo(restoreUnitInfo);
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, instance.InstanceName, "Restored UnitInfo.");
            }
            Add(instance, false);
         }

         if (HasInstances)
         {
            _ConfigFilename = xmlDocName;

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and Web Generation Timers
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
         var serializer = new ClientInstanceXmlSerializer();
         lock (_instanceCollection)
         {
            serializer.DataInterface = new InstanceCollectionDataInterface(GetCurrentInstanceArray());
         }
         
         serializer.Serialize(xmlDocName);
         
         _ConfigFilename = xmlDocName;
         _ChangedAfterSave = false;
         OnCollectionSaved(EventArgs.Empty);
      }
      #endregion

      /// <summary>
      /// Read FahMon ClientsTab.txt file and import new instance collection
      /// </summary>
      /// <param name="filename">Path of ClientsTab.txt to import</param>
      public void FromFahMonClientsTab(string filename)
      {
         _ChangedAfterSave = false;

         var serializer = new ClientInstanceFahMonSerializer();
         var collectionDataInterface = new InstanceCollectionDataInterface(GetCurrentInstanceArray());
         serializer.DataInterface = collectionDataInterface;
         serializer.Deserialize(filename);
         
         var builder = new ClientInstanceFactory(_Prefs, _proteinCollection, _benchmarkContainer);
         ICollection<ClientInstance> instances = builder.HandleImportResults(collectionDataInterface.Settings);

         foreach (ClientInstance instance in instances)
         {
            Add(instance, false);
         }

         if (HasInstances)
         {
            _ChangedAfterSave = true;

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and Web Generation Timers
            SetTimerState();

            OnCollectionLoaded(EventArgs.Empty);
         }
      }

      #region List Like Implementation (eventually implement IList or ICollection)
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      public void Add(IClientInstanceSettings settings)
      {
         var builder = new ClientInstanceFactory(_Prefs, _proteinCollection, _benchmarkContainer);
         var instance = builder.Create((ClientInstanceSettings)settings);
         Add(instance, true);
      }
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      /// <param name="fireAddedEvent">Specifies whether this call fires the InstanceAdded Event</param>
      private void Add(ClientInstance instance, bool fireAddedEvent)
      {
         if (ContainsName(instance.InstanceName))
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Client Name '{0}' already exists.", instance.InstanceName));
         }

         _instanceCollection.Add(instance.InstanceName, instance);
         OnCollectionChanged(EventArgs.Empty);
         
         if (fireAddedEvent)
         {
            RetrieveSingleClient(instance);

            _ChangedAfterSave = true;
            OnInstanceAdded(EventArgs.Empty);
         }
      }
      
      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      public void Edit(IClientInstanceSettings settings)
      {
         // if the host key changed
         if (SelectedInstance.InstanceName != settings.InstanceName)
         {
            // check for a duplicate name
            if (ContainsName(settings.InstanceName))
            {
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Client Name '{0}' already exists.", settings.InstanceName));
            }
         }

         // now get a handle on the Selected Instance and its Name and Path
         var instance = SelectedInstance;
         var previousName = SelectedInstance.Settings.InstanceName;
         var previousPath = SelectedInstance.Settings.Path;
         // load the new settings
         instance.Settings.LoadSettings(settings);
         // Instance Name changed but isn't an already existing key
         if (previousName != instance.Settings.InstanceName)
         {
            // update InstanceCollection
            UpdateDisplayInstanceName(SelectedInstance.InstanceName, instance.InstanceName);
            Remove(previousName, false);
            Add(instance, false);

            // update the Names in the BenchmarkContainer
            _benchmarkContainer.UpdateInstanceName(new BenchmarkClient(previousName, instance.Path), instance.InstanceName);
         }
         // the path changed
         if (StringOps.PathsEqual(previousPath, instance.Path) == false)
         {
            // update the Paths in the BenchmarkContainer
            _benchmarkContainer.UpdateInstancePath(new BenchmarkClient(instance.InstanceName, previousPath), instance.Path);
         }
         
         RetrieveSingleClient(instance);

         _ChangedAfterSave = true;
         OnInstanceEdited(EventArgs.Empty);
      }
      
      /// <summary>
      /// Remove an Instance by Key
      /// </summary>
      /// <param name="key">Instance Key</param>
      public void Remove(string key)
      {
         Remove(key, true);
      }

      /// <summary>
      /// Remove an Instance by Key
      /// </summary>
      /// <param name="key">Instance Key</param>
      /// <param name="fireRemovedEvent">Specifies whether this call fires the InstanceRemoved Event</param>
      private void Remove(string key, bool fireRemovedEvent)
      {
         _instanceCollection.Remove(key);
         DisplayInstance findInstance = FindDisplayInstance(_displayCollection, key);
         _displayCollection.Remove(findInstance);
         OnCollectionChanged(EventArgs.Empty);
         
         if (fireRemovedEvent)
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
         _duplicateUserId.Clear();

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
      /// <param name="instanceName">Instance Name to search for</param>
      public bool ContainsName(string instanceName)
      {
         var findInstance = new List<ClientInstance>(_instanceCollection.Values).Find(
            instance => instance.InstanceName.ToUpperInvariant() == instanceName.ToUpperInvariant());
         return findInstance != null;
      }
      #endregion

      #region Retrieval Logic
      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      private void bgWorkTimer_Tick(object sender, EventArgs e)
      {
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Running Retrieval Process...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      private void webGenTimer_Tick(object sender, EventArgs e)
      {
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
      
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer");
         webTimer.Stop();
         
         DoWebGeneration();

         StartWebGenTimer();
      }
      
      private void DoWebGeneration()
      {
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.GenerateWeb));

         // lazy initialize
         var markupGenerator = InstanceProvider.GetInstance<IMarkupGenerator>();

         try
         {
            if (markupGenerator.GenerationInProgress)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Web Generation already in progress...");
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Starting Web Generation...");

               var uploadHtml = _Prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
               var uploadXml = _Prefs.GetPreference<bool>(Preference.WebGenCopyXml);
               DateTime start = HfmTrace.ExecStart;
               ICollection<IClientInstance> instances = GetCurrentInstanceArray();
               instances = GetDisplaySortedInstanceCollection(instances);
               if (uploadHtml)
               {
                  markupGenerator.GenerateHtml(instances);
               }
               else if (uploadXml)
               {
                  markupGenerator.GenerateXml(instances);  
               }

               DeployWebsite(markupGenerator.HtmlFilePaths, markupGenerator.XmlFilePaths, instances);

               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture, 
                  "Total Web Generation Execution Time: {0}", HfmTrace.GetExecTime(start)));
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      
      private ICollection<IClientInstance> GetDisplaySortedInstanceCollection(ICollection<IClientInstance> instances)
      {
         // Issue 166 - Make the Web Summary Page respect the current Sort Column and Direction.
         var sortedCollection = new List<IClientInstance>(instances.Count);
         var displayCollection = GetCurrentDisplayInstanceArray();
         foreach (var d in displayCollection)
         {
            DisplayInstance displayInstance = d;
            var instance = instances.Where(c => c.Settings.InstanceName == displayInstance.Name);
            if (instance.Count() == 1)
            {
               sortedCollection.Add(instance.First());
            }
         }

         return sortedCollection.AsReadOnly();
      }
      
      private void DeployWebsite(ICollection<string> htmlFilePaths, ICollection<string> xmlFilePaths,
                                 ICollection<IClientInstance> instances)
      {
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.GenerateWeb));
      
         var copyHtml = _Prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
         var copyXml = _Prefs.GetPreference<bool>(Preference.WebGenCopyXml);
      
         Match match = StringOps.MatchFtpWithUserPassUrl(_Prefs.GetPreference<string>(Preference.WebRoot));
         if (match.Success)
         {
            string server = match.Result("${domain}");
            string ftpPath = match.Result("${file}");
            string username = match.Result("${username}");
            string password = match.Result("${password}");

            if (_networkOps == null) _networkOps = new NetworkOps();
            
            if (copyHtml)
            {
               _networkOps.FtpWebUpload(server, ftpPath, username, password, htmlFilePaths, instances, _Prefs);
            }
            if (copyXml)
            {
               _networkOps.FtpXmlUpload(server, ftpPath, username, password, xmlFilePaths, _Prefs);
            }
         }
         else
         {
            var webRoot = _Prefs.GetPreference<string>(Preference.WebRoot);
            var cssFile = _Prefs.GetPreference<string>(Preference.CssFile);

            // Create the web folder (just in case)
            if (Directory.Exists(webRoot) == false)
            {
               Directory.CreateDirectory(webRoot);
            }

            if (copyHtml)
            {
               // Copy the CSS file to the output directory
               string cssFilePath = Path.Combine(Path.Combine(_Prefs.ApplicationPath, Constants.CssFolderName), cssFile);
               if (File.Exists(cssFilePath))
               {
                  File.Copy(cssFilePath, Path.Combine(webRoot, cssFile), true);
               }

               foreach (string filePath in htmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }

               if (_Prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
               {
                  foreach (ClientInstance instance in instances)
                  {
                     string cachedFahlogPath = Path.Combine(_Prefs.CacheDirectory, instance.CachedFAHLogName);
                     if (File.Exists(cachedFahlogPath))
                     {
                        File.Copy(cachedFahlogPath, Path.Combine(webRoot, instance.CachedFAHLogName), true);
                     }
                  }
               }
            }
            if (copyXml)
            {
               foreach (string filePath in xmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }
            }
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
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
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

            // run post retrieval processes
            if (_Prefs.GetPreference<bool>(Preference.GenerateWeb) && 
                _Prefs.GetPreference<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               new MethodInvoker(DoWebGeneration).BeginInvoke(null, null);
            }

            if (_Prefs.GetPreference<bool>(Preference.ShowUserStats))
            {
               OnRefreshUserStatsData(EventArgs.Empty);
            }

            // Enable the data retrieval timer
            if (_Prefs.GetPreference<bool>(Preference.SyncOnSchedule))
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
      private void DoRetrieval()
      {
         // get flag synchronous or asynchronous - we don't want this flag to change on us
         // in the middle of a retrieve, so grab it now and use the local copy
         bool synchronous = _Prefs.GetPreference<bool>(Preference.SyncOnLoad);

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
               ClientInstance instance;
               if (_instanceCollection.TryGetValue(instanceKeys[i], out instance))
               {
                  if (synchronous) // do the individual retrieves on a single thread
                  {
                     RetrieveInstance(instance);
                  }
                  else // fire individual threads to do the their own retrieve simultaneously
                  {
                     IAsyncResult async = QueueNewRetrieval(instance);

                     // get the wait handle for each invoked delegate
                     waitHandleList.Add(async.AsyncWaitHandle);
                  }
               }

               i++; // increment the outer loop counter
            }

            if (synchronous == false)
            {
               WaitHandle[] waitHandles = waitHandleList.ToArray();
               // wait for all invoked threads to complete
               WaitHandle.WaitAll(waitHandles);
            }
         }

         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         FindDuplicates();

         // Save the benchmark collection
         InstanceProvider.GetInstance<IProteinBenchmarkContainer>().Write();
      }

      /// <summary>
      /// Delegate used for asynchronous instance retrieval
      /// </summary>
      /// <param name="instance"></param>
      private delegate void RetrieveInstanceDelegate(ClientInstance instance);

      /// <summary>
      /// Stick this Instance in the background thread queue to retrieve the info for the given Instance
      /// </summary>
      private IAsyncResult QueueNewRetrieval(ClientInstance instance)
      {
         return new RetrieveInstanceDelegate(RetrieveInstance).BeginInvoke(instance, null, null);
      }

      /// <summary>
      /// Stub to execute retrieve and refresh display
      /// </summary>
      /// <param name="instance"></param>
      private void RetrieveInstance(ClientInstance instance)
      {
         if (instance.RetrievalInProgress == false)
         {
            instance.Retrieve();
            // This event should signal the UI to update
            OnInstanceRetrieved(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="instanceName">Client Instance Name</param>
      public void RetrieveSingleClient(string instanceName)
      {
         RetrieveSingleClient(_instanceCollection[instanceName]);
      }

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      public void RetrieveSingleClient(ClientInstance instance)
      {
         // fire the actual retrieval thread
         new RetrieveInstanceDelegate(DoSingleClientRetieval).BeginInvoke(instance, null, null);
      }

      /// <summary>
      /// Do a single retrieval operation on the given Client Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      private void DoSingleClientRetieval(ClientInstance instance)
      {
         if (instance.RetrievalInProgress == false)
         {
            IAsyncResult async = QueueNewRetrieval(instance);
            async.AsyncWaitHandle.WaitOne();

            // check for clients with duplicate Project (Run, Clone, Gen) or UserID
            FindDuplicates();

            // Save the benchmark collection
            InstanceProvider.GetInstance<IProteinBenchmarkContainer>().Write();
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
            HfmTrace.WriteToHfmConsole(TraceLevel.Info, "No Hosts - Stopping All Background Timer Loops");
            workTimer.Stop();
            webTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (_Prefs.GetPreference<bool>(Preference.SyncOnSchedule))
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
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
               workTimer.Stop();
            }
         }

         // Enable the web generation timer
         if (_Prefs.GetPreference<bool>(Preference.GenerateWeb) &&
             _Prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false)
         {
            StartWebGenTimer();
         }
         else
         {
            if (webTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer Loop");
               webTimer.Stop();
            }
         }
      }

      /// <summary>
      /// Starts Retrieval Timer Loop
      /// </summary>
      private void StartBackgroundTimer()
      {
         int syncTimeMinutes = _Prefs.GetPreference<int>(Preference.SyncTimeMinutes);
         
         workTimer.Interval = syncTimeMinutes * MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Retrieval Timer Loop: {0} Minutes", syncTimeMinutes));
         workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_Prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
         
         int generateInterval = _Prefs.GetPreference<int>(Preference.GenerateInterval);
      
         webTimer.Interval = generateInterval * MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture,
            "Starting Web Generation Timer Loop: {0} Minutes", generateInterval));
         webTimer.Start();
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
         
         DateTime start = HfmTrace.ExecStart;

         _unitInfoContainer.Clear();
         
         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               // Don't save the UnitInfo object if the contained Project is Unknown
               if (instance.CurrentUnitInfo.ProjectIsUnknown == false)
               {
                  _unitInfoContainer.Add(instance.CurrentUnitInfoConcrete.UnitInfoData);
               }
            }
         }

         _unitInfoContainer.Write();

         HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
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
               var decimalPlaces = _Prefs.GetPreference<int>(Preference.DecimalPlaces);
               if (findInstance != null)
               {
                  findInstance.Load(instance, decimalPlaces);
               }
               else
               {
                  var newInstance = new DisplayInstance(_Prefs);
                  newInstance.Load(instance, decimalPlaces);
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
      private void UpdateDisplayInstanceName(string oldName, string newName)
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
         var instances = GetCurrentInstanceArray();
         FindDuplicateUserId(instances);
         FindDuplicateProjects(instances);

         OnFindDuplicatesComplete(EventArgs.Empty);
      }

      public void FindDuplicateUserId(IEnumerable<ClientInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.UserAndMachineId into g
                           let count = g.Count()
                           where count > 1 && g.First().UserIdUnknown == false
                           select g.Key);

         foreach (ClientInstance instance in instances)
         {
            instance.UserIdIsDuplicate = duplicates.Contains(instance.UserAndMachineId);
         }
      }

      public void FindDuplicateProjects(IEnumerable<ClientInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.CurrentUnitInfo.ProjectRunCloneGen into g
                           let count = g.Count()
                           where count > 1 && g.First().CurrentUnitInfo.ProjectIsUnknown == false
                           select g.Key);

         foreach (ClientInstance instance in instances)
         {
            instance.ProjectIsDuplicate = duplicates.Contains(instance.CurrentUnitInfo.ProjectRunCloneGen);
         }
      }
      #endregion

      #region Helper Functions
      public void SetCurrentInstance(IList selectedClients)
      {
         if (selectedClients.Count > 0)
         {
            Debug.Assert(selectedClients.Count == 1);
            
            if (selectedClients[0] is DataGridViewRow)
            {
               object nameColumnValue = ((DataGridViewRow) selectedClients[0]).Cells["Name"].Value;
               if (nameColumnValue != null)
               {
                  string instanceName = nameColumnValue.ToString();
                  ClientInstance instance;
                  SelectedInstance = Instances.TryGetValue(instanceName, out instance) ? instance : null;
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
      private ClientInstance[] GetCurrentInstanceArray()
      {
         // Changing this to return an empty array instead of null
         // Less hassle not having to possibly deal with a null reference - 4/17/10
         //if (Count > 0)
         //{
            return _instanceCollection.Values.ToArray();
         //}

         //return null;
      }
      
      private DisplayInstance[] GetCurrentDisplayInstanceArray()
      {
         DisplayInstance[] displayInstances = new DisplayInstance[_displayCollection.Count];
         _displayCollection.CopyTo(displayInstances, 0);

         return displayInstances;
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
      /// <param name="key">Instance Name</param>
      /// <returns></returns>
      private static DisplayInstance FindDisplayInstance(IEnumerable<DisplayInstance> collection, string key)
      {
         return new List<DisplayInstance>(collection).Find(displayInstance => displayInstance.Name == key);
      }

      /// <summary>
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private static void ClearCacheFolder()
      {
         DateTime start = HfmTrace.ExecStart;

         IPreferenceSet prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         string cacheFolder = Path.Combine(prefs.GetPreference<string>(Preference.ApplicationDataFolderPath),
                                           prefs.GetPreference<string>(Preference.CacheFolder));

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

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, start);
      }

      /// <summary>
      /// Sets OfflineLast Property on ClientInstances Collection
      /// </summary>
      private void PreferenceSet_OfflineLastChanged(object sender, EventArgs e)
      {
         OfflineClientsLast = _Prefs.GetPreference<bool>(Preference.OfflineLast);
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
