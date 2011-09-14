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
using System.Diagnostics.CodeAnalysis;
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

      #region Service Interfaces

      private readonly IPreferenceSet _prefs;

      private readonly IUnitInfoContainer _unitInfoContainer;

      /// <summary>
      /// Display instance collection (this is bound to the DataGridView)
      /// </summary>
      private readonly IDisplayInstanceCollection _displayCollection;

      public IDisplayInstanceCollection DisplayCollection
      {
         get { return _displayCollection; }
      }

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

      public int Count
      {
         get { return _instanceCollection.Count; }
      }

      public ICollection<string> Keys
      {
         get { return _instanceCollection.Keys; }
      }

      #region Constructor / Initialize
      
      /// <summary>
      /// Primary Constructor
      /// </summary>
      public InstanceCollection(IPreferenceSet prefs, 
                                IUnitInfoContainer unitInfoContainer,
                                IDisplayInstanceCollection displayCollection)
      {
         _prefs = prefs;
         _unitInfoContainer = unitInfoContainer;
         _displayCollection = displayCollection;

         _instanceCollection = new Dictionary<string, ClientInstance>();
      }

      public void Initialize()
      {
         // Set Offline Clients Sort Flag
         OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast);

         // Hook-up PreferenceSet Event Handlers
         _prefs.OfflineLastChanged += delegate { OfflineClientsLast = _prefs.GetPreference<bool>(Preference.OfflineLast); };
      }
      
      #endregion
      
      #region Events

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

      #endregion

      public void LoadInstances(IEnumerable<ClientInstance> instances)
      {
         // clear all instance data before deserialize
         Clear();

         // add each instance to the collection
         foreach (var instance in instances)
         {
            Add(instance.Settings.InstanceName, instance);
         }
      }

      #region IDictionary<string, ClientInstance>

      public void Add(string key, ClientInstance instance)
      {
         _instanceCollection.Add(key, instance);
      }
      
      public void Remove(string key)
      {
         _instanceCollection.Remove(key);
      }
      
      public void Clear()
      {
         _instanceCollection.Clear();
      }

      public bool ContainsKey(string key)
      {
         //var findInstance = _instanceCollection.Values.FirstOrDefault(
         //   instance => instance.Settings.InstanceName.ToUpperInvariant() == instanceName.ToUpperInvariant());
         //return findInstance != null;
         return _instanceCollection.ContainsKey(key);
      }

      public bool TryGetValue(string key, out ClientInstance value)
      {
         return _instanceCollection.TryGetValue(key, out value);
      }

      public ClientInstance GetClientInstance(string key)
      {
         return _instanceCollection[key];
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
            RefreshDisplayInstances();
            _displayCollection.RaiseListChangedEvents = true;
         }
         //_displayCollection.ResetBindings();
      }

      /// <summary>
      /// Call only from RefreshDisplayCollection()
      /// </summary>
      private void RefreshDisplayInstances()
      {
         _displayCollection.Clear();

         foreach (var instance in _instanceCollection.Values)
         {
            if (instance.DisplayInstances.Count == 0)
            {
               _displayCollection.Add(instance.CreateDisplayInstance());
            }
            else
            {
               foreach (var displayInstance in instance.DisplayInstances.Values)
               {
                  _displayCollection.Add(displayInstance);
               }
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
      public IDisplayInstance[] GetCurrentDisplayInstanceArray()
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
      /// <param name="key">Instance Name</param>
      public IDisplayInstance FindDisplayInstance(string key)
      {
         return _displayCollection.FirstOrDefault(displayInstance => displayInstance.Name == key);
      }

      public void SaveCurrentUnitInfo()
      {
         // If no clients loaded, stub out
         if (HasInstances == false) return;

         _unitInfoContainer.Clear();

         lock (_instanceCollection)
         {
            foreach (ClientInstance instance in _instanceCollection.Values)
            {
               foreach (var displayInstance in instance.DisplayInstances.Values)
               {
                  // Don't save the UnitInfo object if the contained Project is Unknown
                  if (displayInstance.CurrentUnitInfo.UnitInfoData.ProjectIsUnknown() == false)
                  {
                     _unitInfoContainer.Add((UnitInfo)displayInstance.CurrentUnitInfo.UnitInfoData);
                  }
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
