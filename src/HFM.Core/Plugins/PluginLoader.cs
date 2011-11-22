
using System.Collections.Generic;
using System.IO;

using Castle.Core.Logging;

using HFM.Core.Serializers;
using HFM.Proteins;

namespace HFM.Core.Plugins
{
   public class PluginLoader
   {
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly IPreferenceSet _prefs;
      private readonly IFileSerializerPluginManager<List<DataTypes.Protein>> _proteinPluginManager;
      private readonly IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>> _benchmarkPluginManager;

      public PluginLoader(IPreferenceSet prefs, IFileSerializerPluginManager<List<DataTypes.Protein>> proteinPluginManager,
                                                IFileSerializerPluginManager<List<DataTypes.ProteinBenchmark>> benchmarkPluginManager)
      {
         _prefs = prefs;
         _proteinPluginManager = proteinPluginManager;
         _benchmarkPluginManager = benchmarkPluginManager;
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
      }

      private void LogResults(IEnumerable<PluginLoadInfo> pluginLoadResults)
      {
         foreach (var loadInfo in pluginLoadResults)
         {
            if (loadInfo.Result.Equals(PluginLoadResult.Success))
            {
               _logger.Info("Loaded Plugin: {0}", loadInfo.FilePath);
            }
            else if (loadInfo.Result.Equals(PluginLoadResult.Failure))
            {
               if (loadInfo.Exception != null)
               {
                  _logger.WarnFormat(loadInfo.Exception, "Plugin Load Failed: {0}", loadInfo.Message);
               }
               else
               {
                  _logger.Warn("Plugin Load Failed: {0}", loadInfo.Message);
               }
            }
         }
      }
   }
}
