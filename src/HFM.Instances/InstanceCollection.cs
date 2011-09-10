/*
 * HFM.NET - Instance Collection Class
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Threading;
using System.Collections.Generic;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   [SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
   public sealed class InstanceCollection
   {
      #region Fields

      /// <summary>
      /// Local time that denotes when a full retrieve started (only accessed by the RetrieveInProgress property)
      /// </summary>
      private DateTime _retrieveExecStart;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress (only accessed by the RetrieveInProgress property)
      /// </summary>
      private volatile bool _retrievalInProgress;
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress
      /// </summary>
      public bool RetrievalInProgress
      {
         get { return _retrievalInProgress; }
         private set
         {
            if (value)
            {
               _retrieveExecStart = HfmTrace.ExecStart;
               _retrievalInProgress = true;
            }
            else
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Total Retrieval Execution Time: {0}", HfmTrace.GetExecTime(_retrieveExecStart)));
               _retrievalInProgress = false;
            }
         }
      }

      /// <summary>
      /// Display Instance Accessor
      /// </summary>
      /// <param name="key">Display Instance Name</param>
      public IDisplayInstance this[string key]
      {
         get { return _displayCollection.FirstOrDefault(x => x.Name == key); }
      }

      /// <summary>
      /// Returns true if Client Instance Count is greater than 0
      /// </summary>
      public bool HasInstances
      {
         get { return _instanceCollection.Count > 0; }
      }

      /// <summary>
      /// Tells the SortableBindingList whether to sort Offline Clients Last
      /// </summary>
      private bool OfflineClientsLast
      {
         set
         {
            if (_displayCollection.OfflineClientsLast != value)
            {
               _displayCollection.OfflineClientsLast = value;
               OnOfflineLastChanged(EventArgs.Empty);
            }
         }
      }

      public ClientInstance SelectedClientInstance { get; private set; }

      public IDisplayInstance SelectedDisplayInstance { get; private set; }

      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      public bool ChangedAfterSave { get; set; }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Client Files' is Visbile
      /// </summary>
      public bool ClientFilesMenuItemVisible
      {
         get 
         {
            return SelectedClientInstance == null || 
                   SelectedClientInstance.Settings.InstanceHostType.Equals(InstanceType.PathInstance);
         }
      }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Cached Log File' is Visbile
      /// </summary>
      public bool CachedLogMenuItemVisible
      {
         get
         {
            return SelectedClientInstance == null ||
                   !(SelectedClientInstance.Settings.ExternalInstance);
         }
      }

      #region Service Interfaces

      private IMarkupGenerator _markupGenerator;

      private IWebsiteDeployer _websiteDeployer;
      
      private readonly IPreferenceSet _prefs;

      private readonly IProteinCollection _proteinCollection;

      private readonly IProteinBenchmarkContainer _benchmarkContainer;
      
      private readonly IUnitInfoContainer _unitInfoContainer;

      private readonly IXmlStatsDataContainer _statsData;

      private readonly IClientInstanceFactory _instanceFactory;

      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly IDisplayInstanceCollection _displayCollection;

      private readonly InstanceConfigurationManager _configurationManager;

      #endregion

      /// <summary>
      /// Client Instance Collection
      /// </summary>
      private readonly Dictionary<string, ClientInstance> _instanceCollection;

      /// <summary>
      /// Retrieval Timer Object (init 10 minutes)
      /// </summary>
      private readonly System.Timers.Timer _workTimer = new System.Timers.Timer(600000);

      /// <summary>
      /// Web Generation Timer Object (init 15 minutes)
      /// </summary>
      private readonly System.Timers.Timer _webTimer = new System.Timers.Timer(900000);
      
      #endregion

      #region Constructor / Initialize
      
      /// <summary>
      /// Primary Constructor
      /// </summary>
      public InstanceCollection(IPreferenceSet prefs, 
                                IProteinCollection proteinCollection, 
                                IProteinBenchmarkContainer benchmarkContainer, 
                                IUnitInfoContainer unitInfoContainer,
                                IXmlStatsDataContainer statsData,
                                IClientInstanceFactory instanceFactory,
                                IDisplayInstanceCollection displayCollection,
                                InstanceConfigurationManager configurationManager)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _unitInfoContainer = unitInfoContainer;
         _statsData = statsData;
         _instanceFactory = instanceFactory;
         _displayCollection = displayCollection;
         _configurationManager = configurationManager;

         _instanceCollection = new Dictionary<string, ClientInstance>();
      }

      public void Initialize()
      {
         // Hook up Retrieval Timer Event Handler
         _workTimer.Elapsed += WorkTimerTick;
         // Hook up Web Generation Timer Event Handler
         _webTimer.Elapsed += WebGenTimerTick;

         // Hook up Protein Collection Updated Event Handler
         _proteinCollection.Downloader.ProjectInfoUpdated += ProjectInfoUpdated;

         // Set Offline Clients Sort Flag
         OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast);

         // Hook-up PreferenceSet Event Handlers
         _prefs.OfflineLastChanged += delegate { OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast); };
         _prefs.TimerSettingsChanged += delegate { SetTimerState(); };
      }
      
      #endregion
      
      #region Events

      public event EventHandler RefreshGrid;
      private void OnRefreshGrid(EventArgs e)
      {
         if (RefreshGrid != null)
         {
            RefreshGrid(this, e);
         }
      }

      public event EventHandler InvalidateGrid;
      private void OnInvalidateGrid(EventArgs e)
      {
         if (InvalidateGrid != null)
         {
            InvalidateGrid(this, e);
         }
      }

      public event EventHandler OfflineLastChanged;
      private void OnOfflineLastChanged(EventArgs e)
      {
         if (OfflineLastChanged != null)
         {
            OfflineLastChanged(this, e);
         }
      }

      public event EventHandler InstanceDataChanged;
      private void OnInstanceDataChanged(EventArgs e)
      {
         if (InstanceDataChanged != null)
         {
            InstanceDataChanged(this, e);
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

      public event EventHandler SelectedInstanceChanged;
      private void OnSelectedInstanceChanged(EventArgs e)
      {
         if (SelectedInstanceChanged != null)
         {
            SelectedInstanceChanged(this, e);
         }
      }

      public event EventHandler RefreshUserStatsControls;
      private void OnRefreshUserStatsControls(EventArgs e)
      {
         if (RefreshUserStatsControls != null)
         {
            RefreshUserStatsControls(this, e);
         }
      }
      
      #endregion

      public void LoadInstances(IEnumerable<ClientInstance> instances)
      {
         // clear all instance data before deserialize
         Clear();

         if (instances.Count() != 0)
         {
            // add each instance to the collection
            foreach (var instance in instances)
            {
               Add(instance, false);
            }

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and Web Generation Timers
            SetTimerState();
         }
      }

      #region Add/Edit/Remove/Clear/Contains
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      public void Add(ClientInstanceSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");

         Add(_instanceFactory.Create(settings), true);
      }
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      /// <param name="fireAddedEvent">Specifies whether this call fires the InstanceAdded Event</param>
      private void Add(ClientInstance instance, bool fireAddedEvent)
      {
         Debug.Assert(instance != null);
      
         if (ContainsName(instance.Settings.InstanceName))
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Client Name '{0}' already exists.", instance.Settings.InstanceName));
         }

         // Issue 230
         bool hasInstances = HasInstances;
         
         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            _instanceCollection.Add(instance.Settings.InstanceName, instance);
         }
         OnRefreshGrid(EventArgs.Empty);
         
         if (fireAddedEvent)
         {
            RetrieveSingleClient(instance);

            ChangedAfterSave = true;
            OnInstanceDataChanged(EventArgs.Empty);
            
            // Issue 230
            if (hasInstances == false)
            {
               SetTimerState();
            }
         }
      }
      
      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="previousName"></param>
      /// <param name="previousPath"></param>
      /// <param name="settings">Client Instance Settings</param>
      public void Edit(string previousName, string previousPath, ClientInstanceSettings settings)
      {
         if (previousName == null) throw new ArgumentNullException("previousName");
         if (previousPath == null) throw new ArgumentNullException("previousPath");
         if (settings == null) throw new ArgumentNullException("settings");

         Debug.Assert(_instanceCollection.ContainsKey(previousName));

         // if the host key changed
         if (previousName != settings.InstanceName)
         {
            // check for a duplicate name
            if (ContainsName(settings.InstanceName))
            {
               throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
                  "Client Name '{0}' already exists.", settings.InstanceName));
            }
         }

         var instance = _instanceCollection[previousName];
         // load the new settings
         instance.Settings.LoadSettings(settings);

         // Instance Name changed but isn't an already existing key
         if (previousName != instance.Settings.InstanceName)
         {
            // lock added here - 9/28/10
            lock (_instanceCollection)
            {
               // update InstanceCollection
               _instanceCollection.Remove(previousName);
               _instanceCollection.Add(instance.Settings.InstanceName, instance);
            }

            // Issue 79 - 9/28/10
            if (instance.Settings.ExternalInstance == false)
            {
               // update the Names in the BenchmarkContainer
               _benchmarkContainer.UpdateInstanceName(new BenchmarkClient(previousName, instance.Settings.Path), instance.Settings.InstanceName);
            }
         }
         // the path changed
         if (Paths.Equal(previousPath, instance.Settings.Path) == false)
         {
            // Issue 79 - 9/28/10
            if (instance.Settings.ExternalInstance == false)
            {
               // update the Paths in the BenchmarkContainer
               _benchmarkContainer.UpdateInstancePath(new BenchmarkClient(instance.Settings.InstanceName, previousPath), instance.Settings.Path);
            }
         }
         
         RetrieveSingleClient(instance);

         ChangedAfterSave = true;
         OnInstanceDataChanged(EventArgs.Empty);
      }
      
      /// <summary>
      /// Remove an Instance by Name
      /// </summary>
      /// <param name="instanceName">Instance Name</param>
      public void Remove(string instanceName)
      {
         if (String.IsNullOrEmpty(instanceName))
         {
            throw new ArgumentException("Argument 'instanceName' cannot be a null or empty string.");
         }

         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            _instanceCollection.Remove(instanceName);
            var findInstance = FindDisplayInstance(_displayCollection, instanceName);
            if (findInstance != null)
            {
               _displayCollection.Remove(findInstance);
            }
         }
         OnRefreshGrid(EventArgs.Empty);
         
         ChangedAfterSave = true;
         OnInstanceDataChanged(EventArgs.Empty);

         DuplicateFinder.FindDuplicates(_displayCollection);
         OnInvalidateGrid(EventArgs.Empty);
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

         // new config filename
         _configurationManager.ClearConfigFilename();
         // collection has not changed
         ChangedAfterSave = false;
         // This will disable the timers, we have no hosts
         SetTimerState();
         
         OnRefreshGrid(EventArgs.Empty);
      }

      /// <summary>
      /// Collection Contains Name
      /// </summary>
      /// <param name="instanceName">Instance Name to search for</param>
      public bool ContainsName(string instanceName)
      {
         var findInstance = _instanceCollection.Values.FirstOrDefault(
            instance => instance.Settings.InstanceName.ToUpperInvariant() == instanceName.ToUpperInvariant());
         return findInstance != null;
      }
      
      #endregion

      #region Retrieval Logic

      /// <summary>
      /// When the host refresh timer expires, refresh all the hosts
      /// </summary>
      private void WorkTimerTick(object sender, EventArgs e)
      {
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Running Retrieval Process...");
         QueueNewRetrieval();
      }

      /// <summary>
      /// When the web gen timer expires, refresh the website files
      /// </summary>
      private void WebGenTimerTick(object sender, EventArgs e)
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
      
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer");
         _webTimer.Stop();
         
         DoWebGeneration();

         StartWebGenTimer();
      }
      
      private void DoWebGeneration()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));

         // lazy initialize
         if (_markupGenerator == null) _markupGenerator = InstanceProvider.GetInstance<IMarkupGenerator>();
         if (_websiteDeployer == null) _websiteDeployer = InstanceProvider.GetInstance<IWebsiteDeployer>();

         try
         {
            if (_markupGenerator.GenerationInProgress)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Web Generation already in progress...");
            }
            else
            {
               DateTime start = HfmTrace.ExecStart;
            
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Starting Web Generation...");

               ICollection<IDisplayInstance> displayInstances = GetCurrentDisplayInstanceArray();
               ICollection<ClientInstance> clientInstances = GetCurrentInstanceArray();
               
               _markupGenerator.Generate(displayInstances, clientInstances);
               _websiteDeployer.DeployWebsite(_markupGenerator.HtmlFilePaths, _markupGenerator.XmlFilePaths, _markupGenerator.ClientDataFilePath, displayInstances);

               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture, 
                  "Total Web Generation Execution Time: {0}", HfmTrace.GetExecTime(start)));
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }

      /// <summary>
      /// Forces a log and screen refresh when the Stanford info is updated
      /// </summary>
      private void ProjectInfoUpdated(object sender, EventArgs e)
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
            _workTimer.Stop();

            // fire the retrieval wrapper thread (basically a wait thread off the UI thread)
            new Action(DoRetrievalWrapper).BeginInvoke(null, null);
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
            IAsyncResult async = new Action(DoRetrieval).BeginInvoke(null, null);
            // wait for completion
            async.AsyncWaitHandle.WaitOne();

            // run post retrieval processes
            if (_prefs.GetPreference<bool>(Preference.GenerateWeb) && 
                _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               new Action(DoWebGeneration).BeginInvoke(null, null);
            }

            if (_prefs.GetPreference<bool>(Preference.ShowXmlStats))
            {
               RefreshUserStatsData(false);
            }

            // Enable the data retrieval timer
            if (_prefs.GetPreference<bool>(Preference.SyncOnSchedule))
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
         bool synchronous = _prefs.GetPreference<bool>(Preference.SyncOnLoad);

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
         DuplicateFinder.FindDuplicates(_displayCollection);
         OnInvalidateGrid(EventArgs.Empty);

         // Save the benchmark collection
         _benchmarkContainer.Write();
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
            OnRefreshGrid(EventArgs.Empty);
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
            DuplicateFinder.FindDuplicates(_displayCollection);
            OnInvalidateGrid(EventArgs.Empty);

            // Save the benchmark collection
            _benchmarkContainer.Write();
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
            _workTimer.Stop();
            _webTimer.Stop();
            return;
         }

         // Enable the data retrieval timer
         if (_prefs.GetPreference<bool>(Preference.SyncOnSchedule))
         {
            if (RetrievalInProgress == false)
            {
               StartBackgroundTimer();
            }
         }
         else
         {
            if (_workTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
               _workTimer.Stop();
            }
         }

         // Enable the web generation timer
         if (_prefs.GetPreference<bool>(Preference.GenerateWeb) &&
             _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false)
         {
            StartWebGenTimer();
         }
         else
         {
            if (_webTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer Loop");
               _webTimer.Stop();
            }
         }
      }

      /// <summary>
      /// Starts Retrieval Timer Loop
      /// </summary>
      private void StartBackgroundTimer()
      {
         var syncTimeMinutes = _prefs.GetPreference<int>(Preference.SyncTimeMinutes);
         
         _workTimer.Interval = syncTimeMinutes * Constants.MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Retrieval Timer Loop: {0} Minutes", syncTimeMinutes));
         _workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
         
         var generateInterval = _prefs.GetPreference<int>(Preference.GenerateInterval);

         _webTimer.Interval = generateInterval * Constants.MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture,
            "Starting Web Generation Timer Loop: {0} Minutes", generateInterval));
         _webTimer.Start();
      }

      /// <summary>
      /// Refresh Stats Data from EOC
      /// </summary>
      /// <param name="forceRefresh">If true, ignore last refresh time stamps and update.</param>
      public void RefreshUserStatsData(bool forceRefresh)
      {
         _statsData.GetEocXmlData(forceRefresh);
         OnRefreshUserStatsControls(EventArgs.Empty);
      }

      #endregion

      #region Binding Support

      /// <summary>
      /// Refresh the Display Collection from the Instance Collection
      /// </summary>
      public void RefreshDisplayCollection()
      {
         lock (_instanceCollection)
         {
            _displayCollection.RaiseListChangedEvents = false;
            ClearExternalDisplayInstances();
            RefreshLocalInstances();
            RefreshExternalInstances();
            _displayCollection.RaiseListChangedEvents = true;
         }
         _displayCollection.ResetBindings();
      }

      /// <summary>
      /// Call only from RefreshDisplayCollection()
      /// </summary>
      private void ClearExternalDisplayInstances()
      {
         var displayCollectionCopy = _displayCollection.ToList();
         foreach (var instance in displayCollectionCopy)
         {
            if (instance.Settings.ExternalInstance ||
                instance.ExternalInstanceName != null)
            {
               _displayCollection.Remove(instance);
            }     
         }
      }

      /// <summary>
      /// Call only from RefreshDisplayCollection()
      /// </summary>
      private void RefreshLocalInstances()
      {
         var localInstances = from instance in _instanceCollection.Values
                              where instance.Settings.ExternalInstance == false
                              select instance;

         foreach (var instance in localInstances)
         {
            IDisplayInstance findInstance = FindDisplayInstance(_displayCollection, instance.Settings.InstanceName);
            if (findInstance == null)
            {
               _displayCollection.Add(instance.DisplayInstance);
            }
         }
      }

      /// <summary>
      /// Call only from RefreshDisplayCollection()
      /// </summary>
      private void RefreshExternalInstances()
      {
         var externalInstances = from instance in _instanceCollection.Values
                                 where instance.Settings.ExternalInstance
                                 select instance;

         foreach (var instance in externalInstances)
         {
            if (instance.ExternalDisplayInstances != null)
            {
               foreach (var displayInstance in instance.ExternalDisplayInstances)
               {
                  _displayCollection.Add(displayInstance);
               }
            }
            else
            {
               // add the ExternalInstance directly to the display 
               // collection, something must be shown to the user 
               // so they can manipulate it.
               _displayCollection.Add(instance.DisplayInstance);
            }
         }
      }

      #endregion

      #region Helper Functions
      
      public void SetSelectedInstance(string instanceName)
      {
         lock (_instanceCollection)
         {
            var previousClient = SelectedDisplayInstance;
            if (instanceName != null)
            {
               SelectedDisplayInstance = _displayCollection.FirstOrDefault(x => x.Name == instanceName);
               SelectedClientInstance = SelectedDisplayInstance.ExternalInstanceName != null ? 
                  _instanceCollection[SelectedDisplayInstance.ExternalInstanceName] : _instanceCollection[instanceName];
            }
            else
            {
               SelectedDisplayInstance = null;
               SelectedClientInstance = null;
            }

            if (previousClient != SelectedDisplayInstance)
            {
               RaiseSelectedInstanceChanged();
            }
         }
      }

      /// <summary>
      /// Get Array Representation of Current Client Instance objects in Collection
      /// </summary>
      public ClientInstance[] GetCurrentInstanceArray()
      {
         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            return _instanceCollection.Values.ToArray();
         }
      }

      /// <summary>
      /// Get Array Representation of Current Display Instance objects in Collection
      /// </summary>
      private IDisplayInstance[] GetCurrentDisplayInstanceArray()
      {
         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            return _displayCollection.ToArray();
         }
      }
      
      /// <summary>
      /// Get Totals for all loaded Client Instances
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public InstanceTotals GetInstanceTotals()
      {
         return InstanceCollectionHelpers.GetInstanceTotals(GetCurrentDisplayInstanceArray());
      }

      /// <summary>
      /// Finds the DisplayInstance by Key (Instance Name)
      /// </summary>
      /// <param name="collection">DisplayIntance Collection</param>
      /// <param name="key">Instance Name</param>
      /// <returns></returns>
      private static IDisplayInstance FindDisplayInstance(IEnumerable<IDisplayInstance> collection, string key)
      {
         return collection.FirstOrDefault(displayInstance => displayInstance.Name == key);
      }

      private void SaveCurrentUnitInfo()
      {
         // If no clients loaded, stub out
         if (HasInstances == false) return;

         _unitInfoContainer.Clear();

         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               // Don't save the UnitInfo object if the contained Project is Unknown
               if (instance.CurrentUnitInfo.UnitInfoData.ProjectIsUnknown() == false)
               {
                  _unitInfoContainer.Add((UnitInfo)instance.CurrentUnitInfo.UnitInfoData);
               }
            }
         }

         _unitInfoContainer.Write();
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
               // not really the "correct" place
               // to do this... but it keeps the
               // logic out of the UI.

               // Save current work units
               SaveCurrentUnitInfo();
               
               // clean managed resources
               _workTimer.Dispose();
               _webTimer.Dispose();
            }

            _disposed = true;
         }
      }

      private bool _disposed;
      #endregion
   }
}
