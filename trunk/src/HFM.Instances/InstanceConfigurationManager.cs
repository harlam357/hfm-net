/*
 * HFM.NET - Instance Configuration Manager Class
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
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

using HFM.Framework;
using HFM.Plugins;

namespace HFM.Instances
{
   public class InstanceConfigurationManager
   {
      private readonly IPreferenceSet _prefs;
      private readonly IClientInstanceFactory _instanceFactory;
      private readonly IUnitInfoContainer _unitInfoContainer;
      private readonly IList<IClientInstanceSettingsSerializer> _settingsPlugins;
      
      /// <summary>
      /// Client Configuration Filename
      /// </summary>
      public string ConfigFilename { get; private set; }

      /// <summary>
      /// Current Plugin Index
      /// </summary>
      public int SettingsPluginIndex { get; private set; }

      /// <summary>
      /// Number of Settings Plugins
      /// </summary>
      public int SettingsPluginsCount
      {
         get { return _settingsPlugins.Count; }
      }

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
      
      public InstanceConfigurationManager(IPreferenceSet prefs, IClientInstanceFactory instanceFactory, IUnitInfoContainer unitInfoContainer)
      {
         _prefs = prefs;
         _instanceFactory = instanceFactory;
         _unitInfoContainer = unitInfoContainer;
         _settingsPlugins = GetClientInstanceSerializers();

         ClearConfigFilename();
      }

      /// <summary>
      /// Clear the Configuration Filename
      /// </summary>
      public void ClearConfigFilename()
      {
         ConfigFilename = String.Empty;
      }
   
      /// <summary>
      /// Reads a collection of Client Instance Settings from file
      /// </summary>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      public ICollection<ClientInstance> ReadConfigFile(string filePath, int filterIndex)
      {
         Debug.Assert(String.IsNullOrEmpty(filePath) == false);
         Debug.Assert(filterIndex <= _settingsPlugins.Count);

         var serializer = _settingsPlugins[filterIndex - 1];
         var collectionDataInterface = new InstanceCollectionDataInterface();
         serializer.DataInterface = collectionDataInterface;
         serializer.Deserialize(filePath);

         var instances = _instanceFactory.HandleImportResults(collectionDataInterface.Settings);
         foreach (var instance in instances)
         {
            foreach (var displayInstance in instance.DisplayInstances.Values)
            {
               var restoreUnitInfo = _unitInfoContainer.RetrieveUnitInfo(displayInstance);
               if (restoreUnitInfo != null)
               {
                  displayInstance.RestoreUnitInfo(restoreUnitInfo);
                  HfmTrace.WriteToHfmConsole(TraceLevel.Verbose, instance.Settings.InstanceName, "Restored UnitInfo.");
               }
            }
         }

         if (instances.Count != 0)
         {
            // update the settings plugin index only if something was loaded
            SettingsPluginIndex = filterIndex;
            ConfigFilename = filePath;
         }

         return instances;
      }

      /// <summary>
      /// Saves the current collection of Client Instances to file
      /// </summary>
      /// <param name="instances">Client Instance Collection</param>
      /// <param name="filePath">Path to Config File</param>
      /// <param name="filterIndex">Dialog file type filter index (1 based)</param>
      public void WriteConfigFile(ICollection<ClientInstance> instances, string filePath, int filterIndex)
      {
         Debug.Assert(instances != null);
         Debug.Assert(String.IsNullOrEmpty(filePath) == false);
         Debug.Assert(filterIndex > 0 && filterIndex <= _settingsPlugins.Count);

         var serializer = _settingsPlugins[filterIndex - 1];
         serializer.DataInterface = new InstanceCollectionDataInterface(instances);
         serializer.Serialize(filePath);

         SettingsPluginIndex = filterIndex;
         ConfigFilename = filePath;
      }

      #region Client Instance Serializer Plugins

      private ReadOnlyCollection<IClientInstanceSettingsSerializer> GetClientInstanceSerializers()
      {
         var serializers = new List<IClientInstanceSettingsSerializer>();
         serializers.Add(new ClientInstanceXmlSerializer());

         var di = new DirectoryInfo(Path.Combine(_prefs.ApplicationDataFolderPath, Constants.PluginsFolderName));
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
            if (t.IsPublic && (t.Attributes & TypeAttributes.Abstract) != TypeAttributes.Abstract)
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
   }
}
