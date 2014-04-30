/*
 * HFM.NET - Client Settings Manager Tests
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using System.IO;

using NUnit.Framework;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;
using HFM.Core.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ClientSettingsManagerTests
   {
      [Test]
      public void DefaultsTest()
      {
         var configurationManager = new ClientSettingsManager(GetPluginManager());
         Assert.AreEqual(String.Empty, configurationManager.FileName);
         Assert.AreEqual(1, configurationManager.FilterIndex);
         Assert.AreEqual(2, configurationManager.PluginCount);
         Assert.AreEqual("hfmx", configurationManager.FileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfmx|HFM Legacy Configuration Files|*.hfm", configurationManager.FileTypeFilters);
      }
      
      [Test]
      public void ReadTest()
      {
         var configurationManager = new ClientSettingsManager(GetPluginManager());
         configurationManager.Read("..\\..\\TestFiles\\LegacyTest.hfm", 2);
         Assert.AreEqual("..\\..\\TestFiles\\LegacyTest.hfm", configurationManager.FileName);
         Assert.AreEqual(2, configurationManager.FilterIndex);
         Assert.AreEqual(".hfm", configurationManager.FileExtension);
      }
      
      [Test]
      public void WriteTest()
      {
         var instance1 = new LegacyClient();
         instance1.Settings = new ClientSettings(ClientType.Legacy) { Name = "test" };

         const string testFile = "..\\..\\TestFiles\\new.ext";

         var configurationManager = new ClientSettingsManager(GetPluginManager());
         configurationManager.Write(new[] { instance1.Settings }, testFile, 1);
         Assert.AreEqual("..\\..\\TestFiles\\new.ext", configurationManager.FileName);
         Assert.AreEqual(1, configurationManager.FilterIndex);
         Assert.AreEqual(".ext", configurationManager.FileExtension);

         File.Delete(testFile);
      }

      private static IFileSerializerPluginManager<List<ClientSettings>> GetPluginManager()
      {
         var pluginManager = new FileSerializerPluginManager<List<ClientSettings>>();
         pluginManager.RegisterPlugin(typeof(HfmFileSerializer).Name, new HfmFileSerializer());
         pluginManager.RegisterPlugin(typeof(HfmLegacyFileSerializer).Name, new HfmLegacyFileSerializer());
         return pluginManager;
      }
   }
}
