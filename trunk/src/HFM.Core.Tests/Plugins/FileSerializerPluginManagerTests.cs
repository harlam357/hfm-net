
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Core.Tests.Plugins
{
   [TestFixture]
   public class FileSerializerPluginManagerTests
   {
      private FileSerializerPluginManager<List<Protein>> _pluginManager;

      [SetUp]
      public void Init()
      {
         _pluginManager = new FileSerializerPluginManager<List<Protein>>();
      }

      [Test]
      public void LoadPluginTest1()
      {
         const string fileName = "..\\..\\TestFiles\\HFM.Proteins.ExampleSerializer.dll";

         PluginLoadInfo info = _pluginManager.LoadPlugin(fileName);
         Assert.AreEqual(Path.GetFullPath(fileName), info.FilePath);
         Assert.AreEqual(PluginLoadResult.Success, info.Result);
         Assert.AreEqual(String.Empty, info.Message);
         Assert.AreEqual(null, info.Exception);
      }

      [Test]
      public void LoadPluginTest2()
      {
         const string fileName = "..\\..\\TestFiles\\HFM.Proteins.ExampleSerializer.NoFileDescription.dll";

         PluginLoadInfo info = _pluginManager.LoadPlugin(fileName);
         Assert.AreEqual(Path.GetFullPath(fileName), info.FilePath);
         Assert.AreEqual(PluginLoadResult.Failure, info.Result);
         Assert.AreNotEqual(String.Empty, info.Message);
         Assert.AreEqual(null, info.Exception);
      }

      [Test]
      public void LoadPluginTest3()
      {
         const string fileName = "..\\..\\TestFiles\\HFM.Proteins.ExampleSerializer.NoFileExtension.dll";

         PluginLoadInfo info = _pluginManager.LoadPlugin(fileName);
         Assert.AreEqual(Path.GetFullPath(fileName), info.FilePath);
         Assert.AreEqual(PluginLoadResult.Failure, info.Result);
         Assert.AreNotEqual(String.Empty, info.Message);
         Assert.AreEqual(null, info.Exception);
      }

      [Test]
      public void LoadPluginTest4()
      {
         const string fileName = "..\\..\\TestFiles\\HFM.Proteins.ExampleSerializer.FileNotFound.dll";

         PluginLoadInfo info = _pluginManager.LoadPlugin(fileName);
         Assert.AreEqual(Path.GetFullPath(fileName), info.FilePath);
         Assert.AreEqual(PluginLoadResult.Failure, info.Result);
         Assert.AreNotEqual(String.Empty, info.Message);
         Assert.AreEqual(typeof(FileNotFoundException), info.Exception.GetType());
      }

      [Test]
      public void LoadAllPluginsTest1()
      {
         const string path = "..\\..\\TestFiles";

         List<PluginLoadInfo> info = _pluginManager.LoadAllPlugins(path).ToList();
         Assert.AreEqual(3, info.Count);
         Assert.AreEqual(1, _pluginManager.Keys.Count());
         Assert.IsNotNull(_pluginManager.GetPlugin("HFM.Proteins.ExampleSerializer"));
         Assert.IsNull(_pluginManager.GetPlugin("HFM.Proteins.ExampleSerializer.NoFileDescription"));
         Assert.IsNull(_pluginManager.GetPlugin("HFM.Proteins.ExampleSerializer.NoFileExtension"));
      }
   }
}
