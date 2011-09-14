/*
 * HFM.NET - Instance Configuration Manager Tests
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

using Castle.Core;
using Castle.Windsor;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class InstanceConfigurationManagerTests
   {
      private IPreferenceSet _prefs;
      private IProteinCollection _proteinCollection;
      private IUnitInfoContainer _unitInfoContainer;

      private IClientInstanceFactory _instanceFactory;
      private IWindsorContainer _container;


      [SetUp]
      public void Init()
      {
         _prefs = MockRepository.GenerateStub<IPreferenceSet>();
         _prefs.Stub(x => x.ApplicationDataFolderPath).Return(String.Empty);
         _proteinCollection = SetupStubProteinCollection("GRO-A3", 100);
         _unitInfoContainer = MockRepository.GenerateStub<IUnitInfoContainer>();

         _instanceFactory = new ClientInstanceFactory();
         // setup container for real ClientInstanceFactory
         _container = new WindsorContainer();
         _container.Kernel.AddComponent("clientInstance", typeof(ClientInstance), LifestyleType.Transient);
         _container.Kernel.AddComponentInstance("preferenceSet", _prefs);
         _container.Kernel.AddComponentInstance("proteinCollection", typeof(IProteinCollection), _proteinCollection);
         _container.Kernel.AddComponentInstance("benchmarkContainer", typeof(IProteinBenchmarkContainer), MockRepository.GenerateStub<IProteinBenchmarkContainer>());
         _container.Kernel.AddComponentInstance("statusLogic", typeof(IStatusLogic), MockRepository.GenerateStub<IStatusLogic>());
         _container.Kernel.AddComponentInstance("dataRetriever", typeof(IDataRetriever), MockRepository.GenerateStub<IDataRetriever>());
         _container.Kernel.AddComponentInstance("dataAggregator", typeof(IDataAggregator), MockRepository.GenerateStub<IDataAggregator>());
         InstanceProvider.SetContainer(_container);
      }
   
      [Test]
      public void DefaultsTest()
      {
         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         Assert.AreEqual(String.Empty, configurationManager.ConfigFilename);
         Assert.AreEqual(0, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(false, configurationManager.HasConfigFilename);
         Assert.AreEqual("hfm", configurationManager.ConfigFileExtension); //TODO: default is different than after Read
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
      }
      
      [Test]
      public void ReadConfigFileTest()
      {
         _unitInfoContainer.Stub(x => x.RetrieveUnitInfo(null)).IgnoreArguments().Return(new UnitInfo());
         
         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         configurationManager.ReadConfigFile("..\\..\\TestFiles\\test.hfm", 1);
         Assert.AreEqual("..\\..\\TestFiles\\test.hfm", configurationManager.ConfigFilename);
         Assert.AreEqual(1, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(true, configurationManager.HasConfigFilename);
         Assert.AreEqual(".hfm", configurationManager.ConfigFileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
      }
      
      [Test]
      public void WriteConfigFileTest()
      {
         var instance1 = new ClientInstance();
         instance1.ProteinCollection = _proteinCollection;
         instance1.Settings = new ClientInstanceSettings { InstanceName = "test" };
      
         var configurationManager = new InstanceConfigurationManager(_prefs, _instanceFactory, _unitInfoContainer);
         configurationManager.WriteConfigFile(new[] { instance1 }, "..\\..\\TestFiles\\new.ext", 1);
         Assert.AreEqual("..\\..\\TestFiles\\new.ext", configurationManager.ConfigFilename);
         Assert.AreEqual(1, configurationManager.SettingsPluginIndex);
         Assert.AreEqual(1, configurationManager.SettingsPluginsCount);
         Assert.AreEqual(true, configurationManager.HasConfigFilename);
         Assert.AreEqual(".ext", configurationManager.ConfigFileExtension);
         Assert.AreEqual("HFM Configuration Files|*.hfm", configurationManager.FileTypeFilters);
      }

      private static IProteinCollection SetupStubProteinCollection(string core, int frames)
      {
         var currentProtein = new Protein { Core = core, Frames = frames };
         var proteinCollection = MockRepository.GenerateStub<IProteinCollection>();
         proteinCollection.Stub(x => x.GetProtein(0, true)).Return(currentProtein).IgnoreArguments();

         return proteinCollection;
      }
   }
}
