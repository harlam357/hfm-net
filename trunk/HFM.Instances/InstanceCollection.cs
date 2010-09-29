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
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.ComponentModel;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Collections.Generic;

using HFM.Framework;
using HFM.Plugins;

namespace HFM.Instances
{
   public sealed class InstanceCollection : IDisposable
   {
      #region Fields
      
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
      /// Client Instance Collection
      /// </summary>
      private readonly Dictionary<string, ClientInstance> _instanceCollection;

      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly DisplayInstanceSortableBindingList _displayCollection;

      /// <summary>
      /// Display Instance Accessor
      /// </summary>
      /// <param name="key">Display Instance Name</param>
      [CLSCompliant(false)]
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

      [CLSCompliant(false)]
      public IClientInstance SelectedClientInstance { get; private set; }

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

      [CLSCompliant(false)]
      public IDisplayInstance SelectedDisplayInstance { get; private set; }

      private int _settingsPluginIndex;

      private IList<IClientInstanceSettingsSerializer> _settingsPlugins;
      
      /// <summary>
      /// String Representation of the File Type Filters used in an Open File Dialog
      /// </summary>
      public string FileTypeFilters
      {
         get
         {
            var sb = new StringBuilder();
            foreach (var plugin in _settingsPlugins)
            {
               sb.Append(plugin.FileTypeFilter);
               sb.Append("|");
            }

            sb.Length = sb.Length - 1;
            return sb.ToString();
         }
      }

      /// <summary>
      /// Client Configuration Filename
      /// </summary>
      public string ConfigFilename { get; private set; }

      /// <summary>
      /// Client Configuration has Filename defined
      /// </summary>
      public bool HasConfigFilename
      {
         get { return ConfigFilename.Length != 0; }
      }
      
      /// <summary>
      /// Current Config File Extension or the Default File Extension
      /// </summary>
      public string ConfigFileExtension
      {
         get
         {
            if (HasConfigFilename)
            {
               return Path.GetExtension(ConfigFilename);
            }
            
            Debug.Assert(_settingsPlugins.Count != 0);
            return _settingsPlugins[0].FileExtension;
         }
      }

      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      public bool ChangedAfterSave { get; private set; }

      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private NetworkOps _networkOps;
      
      /// <summary>
      /// Preferences Interface
      /// </summary>
      private readonly IPreferenceSet _prefs;

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

      /// <summary>
      /// Client Instance Factory
      /// </summary>
      private readonly IClientInstanceFactory _instanceFactory;
      #endregion

      #region CTOR / Initialize
      
      /// <summary>
      /// Default Constructor
      /// </summary>
      [CLSCompliant(false)]
      public InstanceCollection(IPreferenceSet prefs, 
                                IProteinCollection proteinCollection, 
                                IProteinBenchmarkContainer benchmarkContainer, 
                                IUnitInfoContainer unitInfoContainer,
                                IClientInstanceFactory instanceFactory)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _unitInfoContainer = unitInfoContainer;
         _instanceFactory = instanceFactory;

         _instanceCollection = new Dictionary<string, ClientInstance>();
         _displayCollection = new DisplayInstanceSortableBindingList();

         ConfigFilename = String.Empty;
      }

      public void Initialize()
      {
         _unitInfoContainer.Read();
      
         // Hook up Retrieval Timer Event Handler
         workTimer.Elapsed += bgWorkTimer_Tick;
         // Hook up Web Generation Timer Event Handler
         webTimer.Elapsed += webGenTimer_Tick;

         // Hook up Protein Collection Updated Event Handler
         _proteinCollection.Downloader.ProjectInfoUpdated += ProteinCollection_ProjectInfoUpdated;

         // Set Offline Clients Sort Flag
         OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast);

         // Clear the Log File Cache Folder
         ClearCacheFolder();

         // Hook-up PreferenceSet Event Handlers
         _prefs.OfflineLastChanged += delegate { OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast); };
         _prefs.TimerSettingsChanged += delegate { SetTimerState(); };

         _settingsPlugins = GetClientInstanceSerializers();
      }
      
      #endregion
      
      #region Client Instance Serializer Plugins
      
      private ReadOnlyCollection<IClientInstanceSettingsSerializer> GetClientInstanceSerializers()
      {
         var serializers = new List<IClientInstanceSettingsSerializer>();
         serializers.Add(new ClientInstanceXmlSerializer());
         
         var di = new DirectoryInfo(Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), "ClientSettings"));
         if (di.Exists)
         {
            var files = di.GetFiles("*.dll");
            foreach (var file in files)
            {
               try
               {
                  Assembly asm = Assembly.LoadFrom(file.FullName);
                  var serializer = GetSerializer(asm);
                  if (serializer != null && ValidateSerializer(serializer))
                  {
                     serializers.Add(serializer);
                  }
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(ex);
               }
            }
         }

         return serializers.AsReadOnly();
      }
      
      private static IClientInstanceSettingsSerializer GetSerializer(Assembly asm)
      {
         // Loop through each type in the DLL
         foreach (Type t in asm.GetTypes())
         {
            // Only look at public types
            if (t.IsPublic &&
                !((t.Attributes & TypeAttributes.Abstract) == TypeAttributes.Abstract))
            {
               // See if this type implements our interface
               var interfaceType = t.GetInterface("IClientInstanceSettingsSerializer", false);
               if (interfaceType != null)
               {
                  return (IClientInstanceSettingsSerializer)Activator.CreateInstance(t);
               }
            }
         }

         return null;
      }
      
      private static bool ValidateSerializer(IClientInstanceSettingsSerializer serializer)
      {
         var numOfBarChars = serializer.FileTypeFilter.Count(x => x == '|');
         if (String.IsNullOrEmpty(serializer.FileExtension) || numOfBarChars != 1)
         {
            // extention filter string, too many bar characters
            return false;
         }

         return true;
      }
      
      #endregion

      #region Events

      public event EventHandler CollectionChanged;
      private void OnCollectionChanged(EventArgs e)
      {
         if (CollectionChanged != null)
         {
            CollectionChanged(this, e);
         }
      }

      public event EventHandler CollectionLoaded;
      private void OnCollectionLoaded(EventArgs e)
      {
         if (CollectionLoaded != null)
         {
            CollectionLoaded(this, e);
         }
      }

      public event EventHandler CollectionSaved;
      private void OnCollectionSaved(EventArgs e)
      {
         if (CollectionSaved != null)
         {
            CollectionSaved(this, e);
         }
      }

      public event EventHandler InstanceAdded;
      private void OnInstanceAdded(EventArgs e)
      {
         if (InstanceAdded != null)
         {
            InstanceAdded(this, e);
         }
      }

      public event EventHandler InstanceEdited;
      private void OnInstanceEdited(EventArgs e)
      {
         if (InstanceEdited != null)
         {
            InstanceEdited(this, e);
         }
      }

      public event EventHandler InstanceRemoved;
      private void OnInstanceRemoved(EventArgs e)
      {
         if (InstanceRemoved != null)
         {
            InstanceRemoved(this, e);
         }
      }

      public event EventHandler InstanceRetrieved;
      private void OnInstanceRetrieved(EventArgs e)
      {
         if (InstanceRetrieved != null)
         {
            InstanceRetrieved(this, e);
         }
      }

      public event EventHandler FindDuplicatesComplete;
      private void OnFindDuplicatesComplete(EventArgs e)
      {
         if (FindDuplicatesComplete != null)
         {
            FindDuplicatesComplete(this, e);
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

      public event EventHandler RefreshUserStatsData;
      private void OnRefreshUserStatsData(EventArgs e)
      {
         if (RefreshUserStatsData != null)
         {
            RefreshUserStatsData(this, e);
         }
      }
      
      #endregion

      #region Read and Write Config File
      
      /// <summary>
      /// Reads a collection of Client Instance Settings from file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      public void ReadConfigFile(string filePath, int filterIndex)
      {
         if (String.IsNullOrEmpty(filePath))
         {
            throw new ArgumentException("Argument 'filePath' cannot be a null or empty string.", "filePath");
         }

         if (filterIndex > _settingsPlugins.Count)
         {
            throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture, 
               "Argument 'filterIndex' must be between 1 and {0}.", _settingsPlugins.Count));
         }

         // clear all instance data before deserialize
         Clear();
         
         var serializer = _settingsPlugins[filterIndex - 1];
         var collectionDataInterface = new InstanceCollectionDataInterface(GetCurrentInstanceArray());
         serializer.DataInterface = collectionDataInterface;
         serializer.Deserialize(filePath);
         
         var instances = _instanceFactory.HandleImportResults(collectionDataInterface.Settings);
         foreach (var instance in instances)
         {
            var restoreUnitInfo = _unitInfoContainer.RetrieveUnitInfo(instance);
            if (restoreUnitInfo != null)
            {
               instance.RestoreUnitInfo(restoreUnitInfo);
               HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, instance.Settings.InstanceName, "Restored UnitInfo.");
            }
            Add(instance, false);
         }

         if (HasInstances)
         {
            // update the settings plugin index only if something was loaded
            _settingsPluginIndex = filterIndex;
            ConfigFilename = filePath;

            // Get client logs         
            QueueNewRetrieval();
            // Start Retrieval and Web Generation Timers
            SetTimerState();

            OnCollectionLoaded(EventArgs.Empty);
         }
      }

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      public void WriteConfigFile()
      {
         WriteConfigFile(ConfigFilename, _settingsPluginIndex);
      }

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      public void WriteConfigFile(string filePath, int filterIndex)
      {
         if (String.IsNullOrEmpty(filePath))
         {
            throw new ArgumentException("Argument 'filePath' cannot be a null or empty string.", "filePath");
         }

         if (filterIndex > _settingsPlugins.Count)
         {
            throw new ArgumentOutOfRangeException("filterIndex", String.Format(CultureInfo.CurrentCulture,
               "Argument 'filterIndex' must be between 1 and {0}.", _settingsPlugins.Count));
         }

         var serializer = _settingsPlugins[filterIndex - 1];
         lock (_instanceCollection)
         {
            serializer.DataInterface = new InstanceCollectionDataInterface(GetCurrentInstanceArray());
         }
         
         serializer.Serialize(filePath);
         
         ConfigFilename = filePath;
         ChangedAfterSave = false;
         OnCollectionSaved(EventArgs.Empty);
      }
      
      #endregion

      #region List Like Implementation (eventually implement IList or ICollection)
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      public void Add(IClientInstanceSettings settings)
      {
         if (settings == null) throw new ArgumentNullException("settings");

         Add(_instanceFactory.Create((ClientInstanceSettings)settings), true);
      }
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="instance">Client Instance</param>
      /// <param name="fireAddedEvent">Specifies whether this call fires the InstanceAdded Event</param>
      private void Add(ClientInstance instance, bool fireAddedEvent)
      {
         if (ContainsName(instance.Settings.InstanceName))
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Client Name '{0}' already exists.", instance.Settings.InstanceName));
         }

         // lock added here - 9/28/10
         lock (_instanceCollection)
         {
            _instanceCollection.Add(instance.Settings.InstanceName, instance);
         }
         OnCollectionChanged(EventArgs.Empty);
         
         if (fireAddedEvent)
         {
            RetrieveSingleClient(instance);

            ChangedAfterSave = true;
            OnInstanceAdded(EventArgs.Empty);
         }
      }
      
      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="previousName"></param>
      /// <param name="previousPath"></param>
      /// <param name="settings">Client Instance Settings</param>
      public void Edit(string previousName, string previousPath, IClientInstanceSettings settings)
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
         if (StringOps.PathsEqual(previousPath, instance.Settings.Path) == false)
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
         OnInstanceEdited(EventArgs.Empty);
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
         OnCollectionChanged(EventArgs.Empty);
         
         ChangedAfterSave = true;
         OnInstanceRemoved(EventArgs.Empty);

         FindDuplicates();
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
         ConfigFilename = String.Empty;
         // collection has not changed
         ChangedAfterSave = false;
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
         var findInstance = _instanceCollection.Values.FirstOrDefault(
            instance => instance.Settings.InstanceName.ToUpperInvariant() == instanceName.ToUpperInvariant());
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
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
      
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Web Generation Timer");
         webTimer.Stop();
         
         DoWebGeneration();

         StartWebGenTimer();
      }
      
      private void DoWebGeneration()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));

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

               var uploadHtml = _prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
               var uploadXml = _prefs.GetPreference<bool>(Preference.WebGenCopyXml);
               DateTime start = HfmTrace.ExecStart;
               ICollection<IClientInstance> instances = GetCurrentInstanceArray();
               ICollection<IDisplayInstance> displayInstances = GetCurrentDisplayInstanceArray();
               if (uploadHtml)
               {
                  // GenerateHtml calls GenerateXml - these two
                  // calls are mutually exclusive
                  markupGenerator.GenerateHtml(displayInstances);
               }
               else if (uploadXml)
               {
                  markupGenerator.GenerateXml(displayInstances);  
               }
               // For now if Xml Upload is choosen, generate
               // the External Data File
               string externalFilePath = null;
               if (uploadXml)
               {
                  externalFilePath = BuildExternalDataFile(instances);
               }

               DeployWebsite(markupGenerator.HtmlFilePaths, markupGenerator.XmlFilePaths, externalFilePath, displayInstances);

               HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format(CultureInfo.CurrentCulture, 
                  "Total Web Generation Execution Time: {0}", HfmTrace.GetExecTime(start)));
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
      }
      
      private static string BuildExternalDataFile(IEnumerable<IClientInstance> instances)
      {
         var list = (from instance in instances
                     where instance.Settings.ExternalInstance == false
                     select (DisplayInstance)instance.DisplayInstance).ToList();

         DateTime start = HfmTrace.ExecStart;

         var filePath = Path.Combine(Path.GetTempPath(), Constants.LocalExternal);
         try
         {
            using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
               ProtoBuf.Serializer.Serialize(fileStream, list);
            }
            return filePath;
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            return null;
         }
         finally
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, start);
         }
      }
      
      private void DeployWebsite(ICollection<string> htmlFilePaths, ICollection<string> xmlFilePaths,
                                 string externalFilePath, ICollection<IDisplayInstance> instances)
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
      
         var copyHtml = _prefs.GetPreference<bool>(Preference.WebGenCopyHtml);
         var copyXml = _prefs.GetPreference<bool>(Preference.WebGenCopyXml);
      
         Match match = StringOps.MatchFtpWithUserPassUrl(_prefs.GetPreference<string>(Preference.WebRoot));
         if (match.Success)
         {
            string server = match.Result("${domain}");
            string ftpPath = match.Result("${file}");
            string username = match.Result("${username}");
            string password = match.Result("${password}");

            if (_networkOps == null) _networkOps = new NetworkOps();
            
            if (copyHtml)
            {
               _networkOps.FtpWebUpload(server, ftpPath, username, password, htmlFilePaths, instances, _prefs);
            }
            if (copyXml)
            {
               _networkOps.FtpXmlUpload(server, ftpPath, username, password, xmlFilePaths, _prefs);
               _networkOps.FtpUploadHelper(server, ftpPath, externalFilePath, username, password, _prefs.GetPreference<FtpType>(Preference.WebGenFtpMode));
            }
         }
         else
         {
            var webRoot = _prefs.GetPreference<string>(Preference.WebRoot);
            var cssFile = _prefs.GetPreference<string>(Preference.CssFile);

            // Create the web folder (just in case)
            if (Directory.Exists(webRoot) == false)
            {
               Directory.CreateDirectory(webRoot);
            }

            if (copyHtml)
            {
               // Copy the CSS file to the output directory
               string cssFilePath = Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName), cssFile);
               if (File.Exists(cssFilePath))
               {
                  File.Copy(cssFilePath, Path.Combine(webRoot, cssFile), true);
               }

               foreach (string filePath in htmlFilePaths)
               {
                  File.Copy(filePath, Path.Combine(webRoot, Path.GetFileName(filePath)), true);
               }

               if (_prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog))
               {
                  foreach (var instance in instances)
                  {
                     string cachedFahlogPath = Path.Combine(_prefs.CacheDirectory, instance.Settings.CachedFahLogName);
                     // Issue 79 - External Instances don't have full FAHlog.txt files available
                     if (instance.ExternalInstanceName == null && File.Exists(cachedFahlogPath))
                     {
                        File.Copy(cachedFahlogPath, Path.Combine(webRoot, instance.Settings.CachedFahLogName), true);
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
            if (_prefs.GetPreference<bool>(Preference.GenerateWeb) && 
                _prefs.GetPreference<bool>(Preference.WebGenAfterRefresh))
            {
               // do a web gen (on another thread)
               new MethodInvoker(DoWebGeneration).BeginInvoke(null, null);
            }

            if (_prefs.GetPreference<bool>(Preference.ShowXmlStats))
            {
               OnRefreshUserStatsData(EventArgs.Empty);
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
         FindDuplicates();

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
            workTimer.Stop();
            webTimer.Stop();
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
            if (workTimer.Enabled)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Info, "Stopping Retrieval Timer Loop");
               workTimer.Stop();
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
         int syncTimeMinutes = _prefs.GetPreference<int>(Preference.SyncTimeMinutes);
         
         workTimer.Interval = syncTimeMinutes * Constants.MinToMillisec;
         HfmTrace.WriteToHfmConsole(TraceLevel.Info, String.Format("Starting Retrieval Timer Loop: {0} Minutes", syncTimeMinutes));
         workTimer.Start();
      }

      /// <summary>
      /// Start the Web Generation Timer
      /// </summary>
      private void StartWebGenTimer()
      {
         Debug.Assert(_prefs.GetPreference<bool>(Preference.GenerateWeb));
         Debug.Assert(_prefs.GetPreference<bool>(Preference.WebGenAfterRefresh) == false);
         
         int generateInterval = _prefs.GetPreference<int>(Preference.GenerateInterval);

         webTimer.Interval = generateInterval * Constants.MinToMillisec;
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
               if (instance.CurrentUnitInfo.UnitInfoData.ProjectIsUnknown == false)
               {
                  _unitInfoContainer.Add(instance.CurrentUnitInfo.UnitInfoData);
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
      [CLSCompliant(false)]
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      public BindingList<IDisplayInstance> GetDisplayCollection()
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

      #region Duplicate UserID and Project Support
      /// <summary>
      /// Find Clients with Duplicate UserIDs or Project (R/C/G)
      /// </summary>
      public void FindDuplicates() // Issue 19
      {
         FindDuplicateUserId(_displayCollection);
         FindDuplicateProjects(_displayCollection);

         OnFindDuplicatesComplete(EventArgs.Empty);
      }

      private static void FindDuplicateUserId(IEnumerable<IDisplayInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.UserAndMachineId into g
                           let count = g.Count()
                           where count > 1 && g.First().UserIdUnknown == false
                           select g.Key);

         foreach (IDisplayInstance instance in instances)
         {
            instance.UserIdIsDuplicate = duplicates.Contains(instance.UserAndMachineId);
         }
      }

      private static void FindDuplicateProjects(IEnumerable<IDisplayInstance> instances)
      {
         var duplicates = (from x in instances
                           group x by x.CurrentUnitInfo.ProjectRunCloneGen into g
                           let count = g.Count()
                           where count > 1 && g.First().CurrentUnitInfo.UnitInfoData.ProjectIsUnknown == false
                           select g.Key);

         foreach (IDisplayInstance instance in instances)
         {
            instance.ProjectIsDuplicate = duplicates.Contains(instance.CurrentUnitInfo.ProjectRunCloneGen);
         }
      }
      #endregion

      #region Helper Functions
      
      public void SetSelectedInstance(IList selectedClients)
      {
         lock (_instanceCollection)
         {
            var previousClient = SelectedDisplayInstance;
            if (selectedClients.Count > 0)
            {
               Debug.Assert(selectedClients.Count == 1);

               var selectedClient = selectedClients[0] as DataGridViewRow;
               if (selectedClient != null)
               {
                  var nameColumnValue = selectedClient.Cells["Name"].Value;
                  if (nameColumnValue != null)
                  {
                     var instanceName = nameColumnValue.ToString();
                     SelectedDisplayInstance = _displayCollection.FirstOrDefault(x => x.Name == instanceName);
                     SelectedClientInstance = SelectedDisplayInstance.ExternalInstanceName != null ? 
                        _instanceCollection[SelectedDisplayInstance.ExternalInstanceName] : _instanceCollection[instanceName];
                  }
                  else
                  {
                     SelectedDisplayInstance = null;
                     SelectedClientInstance = null;
                  }
               }
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
      private ClientInstance[] GetCurrentInstanceArray()
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

      /// <summary>
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private void ClearCacheFolder()
      {
         DateTime start = HfmTrace.ExecStart;

         string cacheFolder = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath),
                                           _prefs.GetPreference<string>(Preference.CacheFolder));

         var di = new DirectoryInfo(cacheFolder);
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
