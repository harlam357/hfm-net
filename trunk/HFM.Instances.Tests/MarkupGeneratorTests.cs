/*
 * HFM.NET - Markup Generator Class Tests
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
 
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class MarkupGeneratorTests
   {
      private MockRepository _mocks;
      private readonly string _testFolderPath;

      private const string TestFolder = "MarkupGeneratorTestOutput";
      
      public MarkupGeneratorTests()
      {
         _testFolderPath = Path.Combine(@"..\..\", TestFolder);
      }
      
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         
         if (Directory.Exists(_testFolderPath))
         {
            Directory.Delete(_testFolderPath, true);
         }

         Directory.CreateDirectory(_testFolderPath);
      }

      [Test]
      public void GenerateXmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProteinCollection proteinCollection = SetupMockProteinCollection();
         ICollection<IClientInstance> instances = SetupMockClientInstanceCollection();
         
         _mocks.ReplayAll();

         MarkupGenerator markupGenerator = new MarkupGenerator(prefs, proteinCollection);
         markupGenerator.DoXmlGeneration(_testFolderPath, instances);
         
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "Overview.xml")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "Instances.xml")));
         
         _mocks.VerifyAll();
      }

      [Test]
      public void GenerateHtmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IProteinCollection proteinCollection = SetupMockProteinCollection();
         ICollection<IClientInstance> instances = SetupMockClientInstanceCollection();

         _mocks.ReplayAll();

         MarkupGenerator markupGenerator = new MarkupGenerator(prefs, proteinCollection);
         markupGenerator.DoHtmlGeneration(_testFolderPath, instances);

         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "index.html")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "mobile.html")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "summary.html")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "mobilesummary.html")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "Test1.html")));
         Assert.IsTrue(File.Exists(Path.Combine(_testFolderPath, "Test2.html")));
         
         _mocks.VerifyAll();
      }

      private IPreferenceSet SetupMockPreferenceSet()
      {
         IPreferenceSet prefs = _mocks.DynamicMock<IPreferenceSet>();
         SetupResult.For(prefs.ApplicationPath).Return(@"..\..\..\HFM");
         Expect.Call(prefs.GetPreference<string>(Preference.WebOverview)).Return("WebOverview.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebMobileOverview)).Return("WebMobileOverview.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebSummary)).Return("WebSummary.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebMobileSummary)).Return("WebMobileSummary.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebInstance)).Return("WebInstance.xslt").Repeat.Any();
         return prefs;
      }

      private IProteinCollection SetupMockProteinCollection()
      {
         IProtein newProtein = _mocks.DynamicMock<IProtein>();
         IProteinCollection proteinCollection = _mocks.DynamicMock<IProteinCollection>();
         Expect.Call(proteinCollection.GetNewProtein()).Return(newProtein).Repeat.Any();

         return proteinCollection;
      }
      
      private ICollection<IClientInstance> SetupMockClientInstanceCollection()
      {
         List<IClientInstance> instances = new List<IClientInstance>();

         IUnitInfoLogic unitInfoLogic = _mocks.Stub<IUnitInfoLogic>();
         IDataAggregator dataAggregator = _mocks.Stub<IDataAggregator>();
         SetupResult.For(dataAggregator.CurrentLogLines).Return(new List<ILogLine>());
         
         IClientInstance instance = _mocks.Stub<IClientInstance>();
         instance.InstanceName = "Test2";
         SetupResult.For(instance.CurrentUnitInfo).Return(unitInfoLogic);
         SetupResult.For(instance.DataAggregator).Return(dataAggregator);         
         
         instances.Add(instance);

         instance = _mocks.Stub<IClientInstance>();
         instance.InstanceName = "Test1";
         SetupResult.For(instance.CurrentUnitInfo).Return(unitInfoLogic);
         SetupResult.For(instance.DataAggregator).Return(dataAggregator);

         instances.Add(instance);

         return instances;
      }
   }
}
