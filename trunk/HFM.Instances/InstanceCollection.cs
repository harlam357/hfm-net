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
using System.ComponentModel;
using System.Windows.Forms;
using System.Xml;
using System.Reflection;
using System.Collections.Generic;

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

        public const int NumberOfDisplayFields = 17;
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
            if ((xmlDocName.Substring(1,1) == ":") || (xmlDocName.StartsWith("\\\\")))
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

                //// Load the type called for by the XML
                //Type t = Type.GetType(_InstanceType);
                //// Create the array of parameters for the fromXml call
                //System.Xml.XmlNode[] paramData = new System.Xml.XmlNode[1];
                //// Set the parameter array element to the XML data from above
                //paramData[0] = xn;
                //// Create an instance of the object
                //object o = Activator.CreateInstance(t);
                //// Locate the FromXml method - must exist for our hierarchy
                //MethodInfo mi = t.GetMethod("FromXml");
                //// Call the FromXml method - deserialize
                //mi.Invoke(o, paramData);
                //// Add the new object to our collection
                //Add((Base)o);
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
            System.Xml.XmlDocument xmlData = new System.Xml.XmlDocument();

            // Create the XML Declaration (well formed)
            XmlDeclaration xmlDeclaration = xmlData.CreateXmlDeclaration("1.0", "utf-8", null);
            xmlData.InsertBefore(xmlDeclaration, xmlData.DocumentElement);

            // Create the root element
            System.Xml.XmlElement xmlRoot = xmlData.CreateElement("Hosts");

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
            if ((xmlDocName.Substring(1,1) == ":") || (xmlDocName.StartsWith("\\\\")))
            {
                xmlData.Save(xmlDocName);
            }
            else
            {
                xmlData.Save(Preferences.PreferenceSet.Instance.AppDataPath + "\\" + xmlDocName);
            }
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
           Add(instance.Name, instance);
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
                 DisplayInstance findInstance = FindDisplayInstance(_displayCollection, instance.Name);
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
