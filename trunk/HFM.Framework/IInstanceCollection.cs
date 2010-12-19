/*
 * HFM.NET - Instance Collection Interface
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
using System.Diagnostics.CodeAnalysis;

using HFM.Framework.DataTypes;

namespace HFM.Framework
{
   public interface IInstanceAccessor
   {
      /// <summary>
      /// Display Instance Accessor
      /// </summary>
      /// <param name="key">Display Instance Name</param>
      IDisplayInstance this[string key] { get; }
   }

   public interface IInstanceCollection : IInstanceAccessor, IDisposable
   {
      /// <summary>
      /// Local flag that denotes a full retrieve already in progress
      /// </summary>
      bool RetrievalInProgress { get; }

      /// <summary>
      /// Returns true if Client Instance Count is greater than 0
      /// </summary>
      bool HasInstances { get; }

      IClientInstance SelectedClientInstance { get; }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Client Files' is Visbile
      /// </summary>
      bool ClientFilesMenuItemVisible { get; }

      /// <summary>
      /// Specifies if the UI Menu Item for 'View Cached Log File' is Visbile
      /// </summary>
      bool CachedLogMenuItemVisible { get; }

      IDisplayInstance SelectedDisplayInstance { get; }
      
      /// <summary>
      /// Denotes the Saved State of the Current Client Configuration (false == saved, true == unsaved)
      /// </summary>
      bool ChangedAfterSave { get; }

      void Initialize();

      #region Events

      event EventHandler RefreshGrid;

      event EventHandler InvalidateGrid;

      event EventHandler OfflineLastChanged;
      
      event EventHandler InstanceDataChanged;

      /// <summary>
      /// Force Raises the SelectedInstanceChanged event.
      /// </summary>
      [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")]
      void RaiseSelectedInstanceChanged();

      event EventHandler SelectedInstanceChanged;

      event EventHandler RefreshUserStatsData;
      
      #endregion

      #region Read and Write Config File
      
      /// <summary>
      /// Reads a collection of Client Instance Settings from file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      void ReadConfigFile(string filePath, int filterIndex);

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      void WriteConfigFile();

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      void WriteConfigFile(string filePath, int filterIndex);
      
      #endregion

      #region Add/Edit/Remove/Clear/Contains
      
      /// <summary>
      /// Add an Instance
      /// </summary>
      /// <param name="settings">Client Instance Settings</param>
      void Add(ClientInstanceSettings settings);
      
      /// <summary>
      /// Edit the ClientInstance Name and Path
      /// </summary>
      /// <param name="previousName"></param>
      /// <param name="previousPath"></param>
      /// <param name="settings">Client Instance Settings</param>
      void Edit(string previousName, string previousPath, ClientInstanceSettings settings);
      
      /// <summary>
      /// Remove an Instance by Name
      /// </summary>
      /// <param name="instanceName">Instance Name</param>
      void Remove(string instanceName);
      
      /// <summary>
      /// Clear All Instance Data
      /// </summary>
      void Clear();
      
      #endregion

      /// <summary>
      /// Stick each Instance in the background thread queue to retrieve the info for a given Instance
      /// </summary>
      void QueueNewRetrieval();

      /// <summary>
      /// Retrieve the given Client Instance
      /// </summary>
      /// <param name="instanceName">Client Instance Name</param>
      void RetrieveSingleClient(string instanceName);

      /// <summary>
      /// Serialize the Current UnitInfo Objects to disk
      /// </summary>
      void SaveCurrentUnitInfo();

      /// <summary>
      /// Refresh the Display Collection from the Instance Collection
      /// </summary>
      void RefreshDisplayCollection();

      void SetSelectedInstance(string instanceName);
      
      /// <summary>
      /// Get Totals for all loaded Client Instances
      /// </summary>
      /// <returns>Totals for all Instances (InstanceTotals Structure)</returns>
      [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate")]
      InstanceTotals GetInstanceTotals();
   }
}
