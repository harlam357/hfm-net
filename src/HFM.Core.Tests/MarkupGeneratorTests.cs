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
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class MarkupGeneratorTests
   {
      [SetUp]
      public void Init()
      {
         Core.Configuration.ObjectMapper.CreateMaps();
      }

      [Test]
      public void GenerateTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyXml)).Return(true);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyHtml)).Return(true);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyClientData)).Return(true);
         
         IEnumerable<SlotModel> displayinstances = CreateSlotModelCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);
         
         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNotNull(markupGenerator.HtmlFilePaths);
      }

      [Test]
      public void GenerateXmlOnlyTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyXml)).Return(true);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyHtml)).Return(false);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<SlotModel> displayinstances = CreateSlotModelCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);

         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
      }

      [Test]
      public void DontGenerateTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyXml)).Return(false);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyHtml)).Return(false);
         prefs.Stub(x => x.Get<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<SlotModel> displayinstances = CreateSlotModelCollection();
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances);

         Assert.IsNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
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
         IEnumerable<SlotModel> instances = CreateSlotModelCollection();
         
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateXml(instances);
         
         Assert.AreEqual(3, markupGenerator.XmlFilePaths.Count());
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "SlotSummary.xml"), markupGenerator.XmlFilePaths.ElementAt(0));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test2.xml"), markupGenerator.XmlFilePaths.ElementAt(1));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test1.xml"), markupGenerator.XmlFilePaths.ElementAt(2));
      }

      [Test]
      public void GenerateHtmlTest()
      {
         IPreferenceSet prefs = SetupStubPreferenceSet();
         IEnumerable<SlotModel> instances = CreateSlotModelCollection();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateHtml(instances);

         Assert.AreEqual(6, markupGenerator.HtmlFilePaths.Count());
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "index.html"), markupGenerator.HtmlFilePaths.ElementAt(0));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobile.html"), markupGenerator.HtmlFilePaths.ElementAt(1));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "summary.html"), markupGenerator.HtmlFilePaths.ElementAt(2));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobilesummary.html"), markupGenerator.HtmlFilePaths.ElementAt(3));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test2.html"), markupGenerator.HtmlFilePaths.ElementAt(4));
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test1.html"), markupGenerator.HtmlFilePaths.ElementAt(5));
      }

      private static IPreferenceSet SetupStubPreferenceSet()
      {
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         prefs.Stub(x => x.ApplicationPath).Return(@"..\..\..\HFM");
         prefs.Stub(x => x.Get<string>(Preference.WebOverview)).Return("WebOverview.xslt");
         prefs.Stub(x => x.Get<string>(Preference.WebMobileOverview)).Return("WebMobileOverview.xslt");
         prefs.Stub(x => x.Get<string>(Preference.WebSummary)).Return("WebSummary.xslt");
         prefs.Stub(x => x.Get<string>(Preference.WebMobileSummary)).Return("WebMobileSummary.xslt");
         prefs.Stub(x => x.Get<string>(Preference.WebInstance)).Return("WebInstance.xslt");
         return prefs;
      }

      private static IEnumerable<SlotModel> CreateSlotModelCollection()
      {
         // setup stubs
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         var proteinCollection = MockRepository.GenerateStub<IProteinDictionary>();
         proteinCollection.Stub(x => x.GetProteinOrDownload(0)).IgnoreArguments().Return(new Protein());

         var instances = new List<SlotModel>();

         // setup concrete instance with stubs
         var instance = new SlotModel();
         instance.Prefs = prefs;
         // set concrete values
         instance.Settings = new ClientSettings { Name = "Test2" };
         instance.CurrentLogLines = new List<LogLine>();
         instances.Add(instance);

         // setup concrete instance with stubs
         instance = new SlotModel();
         instance.Prefs = prefs;
         // Test For - Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
         // Make sure we return null for CurrentLogLines in the second DataAggregator mock.
         instance.Settings = new ClientSettings { Name = "Test1" };
         instance.CurrentLogLines = null;
         instances.Add(instance);

         return instances;
      }
   }
}
