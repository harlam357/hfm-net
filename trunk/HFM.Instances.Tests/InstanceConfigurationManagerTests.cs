/*
 * HFM.NET - Instance Configuration Manager Tests
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
using System.Collections.Generic;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class InstanceConfigurationManagerTests
   {
      private MockRepository _mocks;
   
      private IPreferenceSet _prefs;
      private IClientInstanceFactory _instanceFactory;
      private IUnitInfoContainer _unitInfoContainer;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
      
         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty);
         _instanceFactory = _mocks.DynamicMock<IClientInstanceFactory>();
         _unitInfoContainer = _mocks.DynamicMock<IUnitInfoContainer>();
      }
   
      [Test]
      public void DefaultsTest()
      {
         _mocks.ReplayAll();
      
         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         Assert.AreEqual(String.Empty, configurationManager.ConfigFilename);
         Assert.AreEqual(0, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(false, configurationManager.HasConfigFilename);
         Assert.AreEqual("hfm", configurationManager.ConfigFileExtension); //TODO: default is different than after Read
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
         
         _mocks.VerifyAll();
      }
      
      [Test]
      public void ReadConfigFileTest()
      {
         var instance1 = new ClientInstance();
         var proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0, false)).Return(new Protein()).Repeat.Twice();
         instance1.ProteinCollection = proteinCollection;
         
         Expect.Call(_instanceFactory.HandleImportResults(null)).Return(
            (new List<ClientInstance> { instance1 }).AsReadOnly()).IgnoreArguments();
         Expect.Call(_unitInfoContainer.RetrieveUnitInfo(instance1)).Return(new UnitInfo());
         
         _mocks.ReplayAll();
         
         // initialize AFTER mocks are in replay mode
         instance1.Initialize();

         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         configurationManager.ReadConfigFile("..\\..\\TestFiles\\test.hfm", 1);
         Assert.AreEqual("..\\..\\TestFiles\\test.hfm", configurationManager.ConfigFilename);
         Assert.AreEqual(1, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(true, configurationManager.HasConfigFilename);
         Assert.AreEqual(".hfm", configurationManager.ConfigFileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
         
         _mocks.VerifyAll();
      }
      
      [Test]
      public void WriteConfigFileTest()
      {
         var instance1 = new ClientInstance();
         var proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetProtein(0, false)).Return(new Protein());
         instance1.ProteinCollection = proteinCollection;
      
         _mocks.ReplayAll();
         
         instance1.Initialize();

         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         configurationManager.WriteConfigFile(new[] { instance1 }, "..\\..\\TestFiles\\new.ext", 1);
         Assert.AreEqual("..\\..\\TestFiles\\new.ext", configurationManager.ConfigFilename);
         Assert.AreEqual(1, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(true, configurationManager.HasConfigFilename);
         Assert.AreEqual(".ext", configurationManager.ConfigFileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
         
         _mocks.VerifyAll();
      }
   }
}
