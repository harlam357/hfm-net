/*
 * HFM.NET - Instance Collection Helper Class
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
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;

using HFM.Helpers;
using HFM.Preferences;
using HFM.Proteins;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Instances
{
   public struct InstanceTotals
   {
      public double PPD;
      public double UPD;
      public Int32 WorkingClients;
      public Int32 NonWorkingClients;
      public Int32 TotalCompletedUnits;
      public Int32 TotalFailedUnits;
   }

   public sealed class FoldingInstanceCollection
   {
      #region Events
      public event EventHandler CollectionChanged;
      public event EventHandler CollectionLoaded;
      public event EventHandler CollectionSaved;
      public event EventHandler InstanceAdded;
      public event EventHandler InstanceEdited;
      public event EventHandler InstanceRemoved;
      public event EventHandler InstanceRetrieved;
      public event EventHandler DuplicatesFound;
      #endregion
      
      #region Members
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
      #endregion

      #region CTOR
      /// <summary>
      /// Default Constructor
      /// </summary>
      public FoldingInstanceCollection()
      {
         _instanceCollection = new Dictionary<string, ClientInstance>();
         _displayCollection = new SortableBindingList<DisplayInstance>();
         _duplicateProjects = new List<string>();
         _duplicateUserID = new List<string>();
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

      private void OnDuplicatesFound(EventArgs e)
      {
         if (DuplicatesFound != null)
         {
            DuplicatesFound(this, e);
         }
      }
      #endregion

      #region Properties
      /// <summary>
      /// Client Instance Collection
      /// </summary>
      public Dictionary<string, ClientInstance> InstanceCollection
      {
         get { return _instanceCollection; }
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
         get { return _ConfigFilename != String.Empty; }
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
         set { _displayCollection.OfflineClientsLast = value; }
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
         _ChangedAfterSave = false;
         _ConfigFilename = xmlDocName;

         XmlDocument xmlData = new XmlDocument();

         // Load the XML file
         if ((xmlDocName.Substring(1, 1) == ":") || (xmlDocName.StartsWith("\\\\")))
         {
            xmlData.Load(xmlDocName);
         }
         else
         {
            xmlData.Load(Path.Combine(PreferenceSet.Instance.AppDataPath, xmlDocName));
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
                  instance.CurrentUnitInfo = restoreUnitInfo;
                  Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Restored UnitInfo for Instance '{1}'.", Debug.FunctionName, instance.InstanceName));
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
         _ConfigFilename = xmlDocName;
      
         XmlDocument xmlData = new XmlDocument();

         // Create the XML Declaration (well formed)
         XmlDeclaration xmlDeclaration = xmlData.CreateXmlDeclaration("1.0", "utf-8", null);
         xmlData.InsertBefore(xmlDeclaration, xmlData.DocumentElement);

         // Create the root element
         XmlElement xmlRoot = xmlData.CreateElement("Hosts");

         // Loop through the collection and serialize the lot
         foreach (KeyValuePair<String, ClientInstance> de in _instanceCollection)
         {
            ClientInstance fi = de.Value;
            XmlDocument xmlDoc = fi.ToXml();

            foreach (XmlNode xn in xmlDoc.ChildNodes)
            {
               xmlRoot.AppendChild(xmlData.ImportNode(xn, true));
            }
         }
         xmlData.AppendChild(xmlRoot);

         // Save the XML stream to the file
         if ((xmlDocName.Substring(1, 1) == ":") || (xmlDocName.StartsWith("\\\\")))
         {
            xmlData.Save(xmlDocName);
         }
         else
         {
            xmlData.Save(Path.Combine(PreferenceSet.Instance.AppDataPath, xmlDocName));
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
               if (line.Equals(String.Empty) == false && line.StartsWith("#") == false)
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
                        Debug.WriteToHfmConsole(TraceLevel.Info,
                                                String.Format("{0} Added FahMon Instance Name: {1}.", Debug.FunctionName, instance.InstanceName));
                     }
                     else
                     {
                        Debug.WriteToHfmConsole(TraceLevel.Warning,
                                                String.Format("{0} Failed to add FahMon Instance: {1}.", Debug.FunctionName, line));
                     }
                  }
                  else
                  {
                     Debug.WriteToHfmConsole(TraceLevel.Warning,
                                             String.Format("{0} Failed to add FahMon Instance (not tab delimited): {1}.", Debug.FunctionName, line));
                  }
               }
            }
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error,
                                    String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
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
            Debug.WriteToHfmConsole(TraceLevel.Warning,
                                    String.Format("{0} Cleaned FahMon Instance Name: {1}.", Debug.FunctionName, instanceName));
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

      #region Implementation
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
      
      public void Edit(string oldName, string oldPath, ClientInstance Instance)
      {
         // if the host key changed
         if (oldName != Instance.InstanceName)
         {
            UpdateDisplayInstanceName(oldName, Instance.InstanceName);
            Remove(oldName, false);
            Add(Instance, false);
            
            ProteinBenchmarkCollection.Instance.UpdateInstanceName(oldName, Instance.InstanceName);
         }
         // if the path changed, update the paths in the benchmark collection
         if (oldPath != Instance.Path)
         {
            ProteinBenchmarkCollection.Instance.UpdateInstancePath(Instance.InstanceName, Instance.Path);
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
         
         OnCollectionChanged(EventArgs.Empty);
      }

      /// <summary>
      /// Collection Contains Name
      /// </summary>
      /// <param name="instanceName">Instance Name to search for</param>
      public bool ContainsName(string instanceName)
      {
         ClientInstance findInstance = new List<ClientInstance>(_instanceCollection.Values).Find(delegate(ClientInstance instance)
                                                                {
                                                                   return instance.InstanceName.ToUpper() == instanceName.ToUpper();
                                                                });
         return findInstance != null;
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
         for (int i = 0; i < numInstances; )
         {
            waitHandleList.Clear();
            // loop through the instances (can only handle up to 64 wait handles at a time)
            for (int j = 0; j < 64 && i < numInstances; j++)
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
         ProteinBenchmarkCollection.Instance.Serialize();
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
         Instance.Retrieve();
         // This event should signal the UI to update
         OnInstanceRetrieved(EventArgs.Empty);
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
         IAsyncResult async = QueueNewRetrieval(Instance);
         async.AsyncWaitHandle.WaitOne();

         // check for clients with duplicate Project (Run, Clone, Gen) or UserID
         FindDuplicates();

         // Save the benchmark collection
         ProteinBenchmarkCollection.Instance.Serialize();
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
         
         DateTime Start = Debug.ExecStart;

         UnitInfoCollection collection = UnitInfoCollection.Instance;

         collection.Clear();

         foreach (ClientInstance instance in _instanceCollection.Values)
         {
            // Don't save the UnitInfo object if the contained Project is Unknown
            if (instance.CurrentUnitInfo.ProjectIsUnknown == false)
            {
               collection.Add(instance.CurrentUnitInfo);
            }
         }

         collection.Serialize();

         Debug.WriteToHfmConsole(TraceLevel.Verbose, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
      }
      #endregion

      #region Binding Support
      /// <summary>
      /// Get Display Collection For Binding
      /// </summary>
      /// <returns>List of Display Instances</returns>
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
               instance.SetTimeBasedValues();
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
      /// Find Clients with Duplicate UserIDs or Project (R/C/G) - Issue 19
      /// </summary>
      private void FindDuplicates()
      {
         DateTime Start = Debug.ExecStart;

         _duplicateUserID.Clear();
         _duplicateProjects.Clear();

         Hashtable userHash = new Hashtable(Count);
         Hashtable projectHash = new Hashtable(Count);

         foreach (ClientInstance instance in _instanceCollection.Values)
         {
            string PRCG = instance.CurrentUnitInfo.ProjectRunCloneGen;
            if (projectHash.Contains(PRCG))
            {
               _duplicateProjects.Add(PRCG);
            }
            else
            {
               // don't add an unknown project
               if (instance.CurrentUnitInfo.ProjectIsUnknown == false)
               {
                  projectHash.Add(instance.CurrentUnitInfo.ProjectRunCloneGen, null);
               }
            }

            string UserAndMachineID = instance.UserAndMachineID;
            if (userHash.Contains(UserAndMachineID))
            {
               _duplicateUserID.Add(UserAndMachineID);
            }
            else
            {
               // don't add an unknown User ID
               if (instance.UserIDUnknown == false)
               {
                  userHash.Add(UserAndMachineID, null);
               }
            }
         }

         Debug.WriteToHfmConsole(TraceLevel.Info,
                                 String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));

         if (_duplicateUserID.Count > 0 || _duplicateProjects.Count > 0)
         {
            OnDuplicatesFound(EventArgs.Empty);
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
      /// <summary>
      /// Get Totals for all loaded Client Instances
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      public InstanceTotals GetInstanceTotals()
      {
         InstanceTotals totals = new InstanceTotals();
         
         foreach (ClientInstance instance in _instanceCollection.Values)
         {
            totals.PPD += instance.CurrentUnitInfo.PPD;
            totals.UPD += instance.CurrentUnitInfo.UPD;
            totals.TotalCompletedUnits += instance.NumberOfCompletedUnitsSinceLastStart;
            totals.TotalFailedUnits += instance.NumberOfFailedUnitsSinceLastStart;
            
            switch (instance.Status)
            {
               case ClientStatus.Running:
               case ClientStatus.RunningNoFrameTimes:
                  totals.WorkingClients++;
                  break;
               //case ClientStatus.Paused:
               //   newPausedHosts++;
               //   break;
               //case ClientStatus.Hung:
               //   newHungHosts++;
               //   break;
               //case ClientStatus.Stopped:
               //   newStoppedHosts++;
               //   break;
               //case ClientStatus.Offline:
               //case ClientStatus.Unknown:
               //   newOfflineUnknownHosts++;
               //   break;
            }
         }
         
         totals.NonWorkingClients = Count - totals.WorkingClients;

         return totals;
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
      #endregion
   }
}
