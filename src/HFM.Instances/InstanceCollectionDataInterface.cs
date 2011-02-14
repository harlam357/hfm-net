/*
 * HFM.NET - Instance Collection Data Interface Class
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
using System.Globalization;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using HFM.Plugins;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances
{
   public class InstanceCollectionDataInterface : IInstanceCollectionDataInterface
   {
      private readonly List<ClientInstanceSettings> _settingsList;
      
      public ReadOnlyCollection<ClientInstanceSettings> Settings
      {
         get { return _settingsList.AsReadOnly(); }
      }
      
      private readonly ICollection<ClientInstanceSettings> _collection;

      public InstanceCollectionDataInterface()
      {
         _settingsList = new List<ClientInstanceSettings>();
         _collection = new List<ClientInstanceSettings>();
      }

      public InstanceCollectionDataInterface(ICollection<IClientInstance> instances)
      {
         _settingsList = new List<ClientInstanceSettings>();
         _collection = new List<ClientInstanceSettings>(instances.Count);
         foreach (var instance in instances)
         {
            _collection.Add(instance.Settings);
         }
      }
   
      public InstanceCollectionDataInterface(ICollection<ClientInstanceSettings> collection)
      {
         _settingsList = new List<ClientInstanceSettings>();
         _collection = collection;
      }

      #region IInstanceCollectionDataInterface Members

      public ReadOnlyCollection<string> InstanceNames
      {
         get
         {
            var names = new List<string>(_collection.Count);
            names.AddRange(_collection.Select(instance => instance.InstanceName));
            return names.AsReadOnly();
         }
      }

      public IClientInstanceSettingsDataInterface GetNewDataInterface()
      {
         _settingsList.Add(new ClientInstanceSettings());
         return new ClientInstanceSettingsDataInterface(_settingsList[_settingsList.Count - 1]);
      }

      public IClientInstanceSettingsDataInterface GetExistingDataInterface(string instanceName)
      {
         var settings = _collection.First(x => x.InstanceName == instanceName);
         if (settings == null)
         {
            throw new InvalidOperationException(String.Format(CultureInfo.CurrentCulture,
               "Client Instance Name '{0}' does not exist.", instanceName));
         }

         return new ClientInstanceSettingsDataInterface(settings);
      }

      public void ThrowAwayNewDataInterface(string reason)
      {
         if (_settingsList.Count > 0)
         {
            _settingsList[_settingsList.Count - 1].ImportError = reason;
         }
      }

      #endregion
   }
}
