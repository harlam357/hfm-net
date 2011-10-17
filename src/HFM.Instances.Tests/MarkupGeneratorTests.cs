/*
 * HFM.NET - Markup Generator Class Tests
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
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class MarkupGeneratorTests
   {
      [Test]
      public void GenerateTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyXml)).Return(true);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(true);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(true);
         
         IEnumerable<IDisplayInstance> displayinstances = SetupDisplayInstanceCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);
         
         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNotNull(markupGenerator.HtmlFilePaths);
         Assert.IsNotNull(markupGenerator.ClientDataFilePath);
      }

      [Test]
      public void GenerateXmlOnlyTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyXml)).Return(true);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(false);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<IDisplayInstance> displayinstances = SetupDisplayInstanceCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);

         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
         Assert.IsNull(markupGenerator.ClientDataFilePath);
      }

      [Test]
      public void DontGenerateTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyXml)).Return(false);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(false);
         prefs.Stub(x => x.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<IDisplayInstance> displayinstances = SetupDisplayInstanceCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);

         Assert.IsNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
         Assert.IsNull(markupGenerator.ClientDataFilePath);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GenerateArgumentNullTest1()
      {
         var prefs = SetupStubPreferenceSet();
         
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(null);
      }

      [Test]
      public void GenerateXmlTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         IEnumerable<IDisplayInstance> instances = SetupDisplayInstanceCollection();
         
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateXml(instances);
         
         Assert.AreEqual(2, markupGenerator.XmlFilePaths.Count);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Overview.xml"), markupGenerator.XmlFilePaths[0]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Instances.xml"), markupGenerator.XmlFilePaths[1]);
      }

      [Test]
      public void GenerateHtmlTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         IEnumerable<IDisplayInstance> instances = SetupDisplayInstanceCollection();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateHtml(instances);

         Assert.AreEqual(6, markupGenerator.HtmlFilePaths.Count);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "index.html"), markupGenerator.HtmlFilePaths[0]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobile.html"), markupGenerator.HtmlFilePaths[1]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "summary.html"), markupGenerator.HtmlFilePaths[2]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobilesummary.html"), markupGenerator.HtmlFilePaths[3]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test2.html"), markupGenerator.HtmlFilePaths[4]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test1.html"), markupGenerator.HtmlFilePaths[5]);
      }

      [Test]
      public void GenerateClientDataTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         IEnumerable<IDisplayInstance> instances = SetupDisplayInstanceCollection();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateClientData(instances);

         Assert.IsNotNull(markupGenerator.ClientDataFilePath);
      }

      private static IPreferenceSet SetupStubPreferenceSet()
      {
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         prefs.Stub(x => x.ApplicationPath).Return(@"..\..\..\HFM");
         prefs.Stub(x => x.GetPreference<string>(Preference.WebOverview)).Return("WebOverview.xslt");
         prefs.Stub(x => x.GetPreference<string>(Preference.WebMobileOverview)).Return("WebMobileOverview.xslt");
         prefs.Stub(x => x.GetPreference<string>(Preference.WebSummary)).Return("WebSummary.xslt");
         prefs.Stub(x => x.GetPreference<string>(Preference.WebMobileSummary)).Return("WebMobileSummary.xslt");
         prefs.Stub(x => x.GetPreference<string>(Preference.WebInstance)).Return("WebInstance.xslt");
         return prefs;
      }

      private static IEnumerable<IDisplayInstance> SetupDisplayInstanceCollection()
      {
         // setup stubs
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         var proteinCollection = MockRepository.GenerateStub<IProteinCollection>();
         proteinCollection.Stub(x => x.GetProtein(0, false)).IgnoreArguments().Return(new Protein());
         var benchmarkContainer = MockRepository.GenerateStub<IProteinBenchmarkContainer>();

         var instances = new List<IDisplayInstance>();

         // setup concrete instance with stubs
         var instance = new DisplayInstance();
         instance.Prefs = prefs;
         instance.BenchmarkContainer = benchmarkContainer;
         // set concrete values
         instance.UnitInfo = new UnitInfo();
         instance.Settings = new ClientInstanceSettings { InstanceName = "Test2" };
         instance.BuildUnitInfoLogic(new Protein());
         instance.CurrentLogLines = new List<LogLine>();
         instances.Add(instance);

         // setup concrete instance with stubs
         instance = new DisplayInstance();
         instance.Prefs = prefs;
         instance.BenchmarkContainer = benchmarkContainer;
         // Test For - Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
         // Make sure we return null for CurrentLogLines in the second DataAggregator mock.
         instance.UnitInfo = new UnitInfo();
         instance.Settings = new ClientInstanceSettings { InstanceName = "Test1" };
         instance.BuildUnitInfoLogic(new Protein());
         instance.CurrentLogLines = null;
         instances.Add(instance);

         return instances;
      }
   }
}
