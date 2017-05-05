/*
 * HFM.NET - Plugin Loader Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

using System.Collections.Generic;
using System.IO;

using Castle.Core.Logging;

using HFM.Core.Serializers;
using HFM.Proteins;

namespace HFM.Core.Plugins
{
   public class PluginLoader
   {
      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      private readonly IFileSerializerPluginManager<List<DataTypes.Protein>> _proteinPluginManager;
      private readonly IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>> _benchmarkPluginManager;
      private readonly IFileSerializerPluginManager<List<DataTypes.ClientSettings>> _clientSettingsPluginManager;
      private readonly IFileSerializerPluginManager<List<DataTypes.HistoryEntry>> _historyEntryPluginManager;

      public PluginLoader(IPreferenceSet prefs, IFileSerializerPluginManager<List<DataTypes.Protein>> proteinPluginManager,
                                                IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>> benchmarkPluginManager,
                                                IFileSerializerPluginManager<List<DataTypes.ClientSettings>> clientSettingsPluginManager,
                                                IFileSerializerPluginManager<List<DataTypes.HistoryEntry>> historyEntryPluginManager)
      {
         _prefs = prefs;
         _proteinPluginManager = proteinPluginManager;
         _benchmarkPluginManager = benchmarkPluginManager;
         _clientSettingsPluginManager = clientSettingsPluginManager;
         _historyEntryPluginManager = historyEntryPluginManager;
      }

      private string PluginsFolder
      {
         get { return Path.Combine(_prefs.ApplicationDataFolderPath, Constants.PluginsFolderName); }
      }

      public void Load()
      {
         #region Protein Serializer Plugins

         // register built in types
         _proteinPluginManager.RegisterPlugin(typeof(TabSerializer).Name, new TabSerializer());
         _proteinPluginManager.RegisterPlugin(typeof(HtmlSerializer).Name, new HtmlSerializer());
         _proteinPluginManager.RegisterPlugin(typeof(JsonSerializer).Name, new JsonSerializer());
         // load from plugin folder
         string path = Path.Combine(PluginsFolder, Constants.PluginsProteinsFolderName);
         if (Directory.Exists(path))
         {
            LogResults(_proteinPluginManager.LoadAllPlugins(path));
         }

         #endregion

         #region Benchmark Serializer Plugins
         
         // register built in types
         _benchmarkPluginManager.RegisterPlugin(typeof(ProtoBufFileSerializer<>).Name, new ProtoBufFileSerializer<List<DataTypes.ProteinBenchmark>>());
         _benchmarkPluginManager.RegisterPlugin(typeof(XmlFileSerializer<>).Name, new XmlFileSerializer<List<DataTypes.ProteinBenchmark>>());
         // load from plugin folder
         path = Path.Combine(PluginsFolder, Constants.PluginsBenchmarksFolderName);
         if (Directory.Exists(path))
         {
            LogResults(_benchmarkPluginManager.LoadAllPlugins(path));
         }

         #endregion

         #region ClientSettings Serializer Plugins

         // register built in types
         _clientSettingsPluginManager.RegisterPlugin(typeof(HfmFileSerializer).Name, new HfmFileSerializer { Logger = Logger });
         _clientSettingsPluginManager.RegisterPlugin(typeof(HfmLegacyFileSerializer).Name, new HfmLegacyFileSerializer { Logger = Logger });
         // load from plugin folder
         path = Path.Combine(PluginsFolder, Constants.PluginsClientSettingsFolderName);
         if (Directory.Exists(path))
         {
            LogResults(_clientSettingsPluginManager.LoadAllPlugins(path));
         }

         #endregion

         #region HistoryEntry Serializer Plugins

         // register built in types
         _historyEntryPluginManager.RegisterPlugin(typeof(HistoryEntryCsvSerializer).Name, new HistoryEntryCsvSerializer());
         // load from plugin folder
         path = Path.Combine(PluginsFolder, Constants.PluginsClientSettingsFolderName);
         if (Directory.Exists(path))
         {
            LogResults(_clientSettingsPluginManager.LoadAllPlugins(path));
         }

         #endregion
      }

      private void LogResults(IEnumerable<PluginLoadInfo> pluginLoadResults)
      {
         foreach (var loadInfo in pluginLoadResults)
         {
            if (loadInfo.Result.Equals(PluginLoadResult.Success))
            {
               Logger.InfoFormat("Loaded Plugin: {0}", loadInfo.FilePath);
            }
            else if (loadInfo.Result.Equals(PluginLoadResult.Failure))
            {
               if (loadInfo.Exception != null)
               {
                  Logger.WarnFormat(loadInfo.Exception, "Plugin Load Failed: {0}", loadInfo.Message);
               }
               else
               {
                  Logger.WarnFormat("Plugin Load Failed: {0}", loadInfo.Message);
               }
            }
         }
      }
   }
}
