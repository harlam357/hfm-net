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
using System.Diagnostics;
using System.IO;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using System.Xml;
using System.Collections.Generic;
using HFM.Helpers;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Instances
{
   public class FoldingInstanceCollection
   {
      #region Members
      public event EventHandler RefreshCollection;

      /// <summary>
      /// Main instance collection
      /// </summary>
      private readonly Dictionary<string, ClientInstance> _instanceCollection = new Dictionary<string, ClientInstance>();

      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly SortableBindingList<DisplayInstance> _displayCollection = new SortableBindingList<DisplayInstance>();
      #endregion

      #region CTOR
      /// <summary>
      /// Default Constructor
      /// </summary>
      public FoldingInstanceCollection()
      {
         _instanceCollection = new Dictionary<string, ClientInstance>();
         _displayCollection = new SortableBindingList<DisplayInstance>();
         OnRefreshCollection(EventArgs.Empty);
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
      /// Client Instance Count
      /// </summary>
      public int Count
      {
         get { return _instanceCollection.Count; }
      }

      /// <summary>
      /// Tells the SortableBindingList whether to sort Offline Clients Last
      /// </summary>
      public bool OfflineClientsLast
      {
         get { return _displayCollection.OfflineClientsLast; }
         set { _displayCollection.OfflineClientsLast = value; }
      }

      public const int NumberOfDisplayFields = 19;
      #endregion

      #region Read and Write Xml
      /// <summary>
      /// Loads a collection of Host Instances from disk
      /// </summary>
      /// <param name="xmlDocName">Filename (verbatim) to load data from - User AppData Path is prepended
      /// if the path does not start with either ?: or \\</param>
      public virtual void FromXml(String xmlDocName)
      {
         XmlDocument xmlData = new XmlDocument();

         // Save the XML stream to the file
         if ((xmlDocName.Substring(1, 1) == ":") || (xmlDocName.StartsWith("\\\\")))
         {
            xmlData.Load(xmlDocName);
         }
         else
         {
            xmlData.Load(Preferences.PreferenceSet.Instance.AppDataPath + "\\" + xmlDocName);
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
               Add(instance);
            }
            else
            {
               MessageBox.Show(String.Format("Client {0} failed to load.", xn.Attributes["Name"].ChildNodes[0].Value));
            }
         }

         OnRefreshCollection(EventArgs.Empty);
      }

      /// <summary>
      /// Saves the current collection of Host Instances to disk
      /// </summary>
      /// <param name="xmlDocName">Filename (verbatim) to save data to - User AppData Path is prepended
      /// if the path does not start with either ?: or \\</param>
      public virtual void ToXml(String xmlDocName)
      {
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
            xmlData.Save(Preferences.PreferenceSet.Instance.AppDataPath + "\\" + xmlDocName);
         }
      }
      #endregion

      #region FahMon Import Support
      /// <summary>
      /// Read FahMon ClientsTab.txt file and import new instance collection
      /// </summary>
      /// <param name="filename">Path of ClientsTab.txt to import</param>
      public void FromFahMonClientsTab(string filename)
      {
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
                        Add(instance);
                        Debug.WriteToHfmConsole(TraceLevel.Info,
                                                String.Format("{0} Added FahMon Instance Name: {1}.", Debug.FunctionName, instance.InstanceName));
                     }
                     else
                     {
                        Debug.WriteToHfmConsole(TraceLevel.Warning,
                                                String.Format("{0} Failed to add FahMon Instance: {1}.", Debug.FunctionName, line));
                     }
                  }
               }
            }

            OnRefreshCollection(EventArgs.Empty);
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

      #region Event Wrappers
      protected void OnRefreshCollection(EventArgs e)
      {
         if (RefreshCollection != null)
         {
            RefreshCollection(this, e);
         }
      }
      #endregion

      #region Implementation
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="instance">Instance</param>
      public void Add(ClientInstance instance)
      {
         Add(instance.InstanceName, instance);
      }

      /// <summary>
      /// Add an Instance with Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      /// <param name="instance">Instance</param>
      public void Add(string Key, ClientInstance instance)
      {
         _instanceCollection.Add(Key, instance);
         OnRefreshCollection(EventArgs.Empty);
      }

      /// <summary>
      /// Remove an Instance by Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      public void Remove(string Key)
      {
         _instanceCollection.Remove(Key);
         DisplayInstance findInstance = FindDisplayInstance(_displayCollection, Key);
         _displayCollection.Remove(findInstance);
         OnRefreshCollection(EventArgs.Empty);
      }

      /// <summary>
      /// Instance Collection Contains Key
      /// </summary>
      /// <param name="Key">Instance Key</param>
      /// <returns></returns>
      public bool Contains(string Key)
      {
         return _instanceCollection.ContainsKey(Key);
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

      public void UpdateDisplayHostName(string oldName, string newName)
      {
         DisplayInstance findInstance = FindDisplayInstance(_displayCollection, oldName);
         findInstance.UpdateName(newName);
      }
      #endregion

      #region Helper Functions
      /// <summary>
      /// Finds the DisplayInstance by Key (Instance Name)
      /// </summary>
      /// <param name="collection">DisplayIntance Collection</param>
      /// <param name="Key">Instance Name</param>
      /// <returns></returns>
      private static DisplayInstance FindDisplayInstance(IEnumerable<DisplayInstance> collection, string Key)
      {
         foreach (DisplayInstance instance in collection)
         {
            if (instance.Name == Key)
            {
               return instance;
            }
         }

         return null;
      }
      #endregion
   }
}
