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
      private MockRepository _mocks;

      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
      }
      
      [Test]
      public void GenerateTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyXml)).Return(true);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(true);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(true);
         
         IEnumerable<IDisplayInstance> displayinstances = SetupMockDisplayInstanceCollection();
         IEnumerable<IClientInstance> clientInstances = new List<IClientInstance>();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances, clientInstances);
         
         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNotNull(markupGenerator.HtmlFilePaths);
         Assert.IsNotNull(markupGenerator.ClientDataFilePath);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void GenerateXmlOnlyTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyXml)).Return(true);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(false);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<IDisplayInstance> displayinstances = SetupMockDisplayInstanceCollection();
         IEnumerable<IClientInstance> clientInstances = new List<IClientInstance>();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances, clientInstances);

         Assert.IsNotNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
         Assert.IsNull(markupGenerator.ClientDataFilePath);

         _mocks.VerifyAll();
      }

      [Test]
      public void DontGenerateTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyXml)).Return(false);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyHtml)).Return(false);
         Expect.Call(prefs.GetPreference<bool>(Preference.WebGenCopyClientData)).Return(false);

         IEnumerable<IDisplayInstance> displayinstances = SetupMockDisplayInstanceCollection();
         IEnumerable<IClientInstance> clientInstances = new List<IClientInstance>();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(displayinstances, clientInstances);

         Assert.IsNull(markupGenerator.XmlFilePaths);
         Assert.IsNull(markupGenerator.HtmlFilePaths);
         Assert.IsNull(markupGenerator.ClientDataFilePath);

         _mocks.VerifyAll();
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GenerateArgumentNullTest1()
      {
         var prefs = SetupMockPreferenceSet();
         
         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(null, new List<IClientInstance>());
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void GenerateArgumentNullTest2()
      {
         var prefs = SetupMockPreferenceSet();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.Generate(SetupMockDisplayInstanceCollection(), null);
      }

      [Test]
      public void GenerateXmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IEnumerable<IDisplayInstance> instances = SetupMockDisplayInstanceCollection();
         
         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateXml(instances);
         
         Assert.AreEqual(2, markupGenerator.XmlFilePaths.Count);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Overview.xml"), markupGenerator.XmlFilePaths[0]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Instances.xml"), markupGenerator.XmlFilePaths[1]);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void GenerateHtmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IEnumerable<IDisplayInstance> instances = SetupMockDisplayInstanceCollection();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateHtml(instances);

         Assert.AreEqual(6, markupGenerator.HtmlFilePaths.Count);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "index.html"), markupGenerator.HtmlFilePaths[0]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobile.html"), markupGenerator.HtmlFilePaths[1]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "summary.html"), markupGenerator.HtmlFilePaths[2]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "mobilesummary.html"), markupGenerator.HtmlFilePaths[3]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test2.html"), markupGenerator.HtmlFilePaths[4]);
         Assert.AreEqual(Path.Combine(Path.GetTempPath(), "Test1.html"), markupGenerator.HtmlFilePaths[5]);
         
         _mocks.VerifyAll();
      }

      [Test]
      public void GenerateClientDataTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         IEnumerable<IClientInstance> instances = new List<IClientInstance>();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.GenerateClientData(instances);

         Assert.IsNotNull(markupGenerator.ClientDataFilePath);

         _mocks.VerifyAll();
      }

      private IPreferenceSet SetupMockPreferenceSet()
      {
         var prefs = _mocks.DynamicMock<IPreferenceSet>();
         SetupResult.For(prefs.ApplicationPath).Return(@"..\..\..\HFM");
         Expect.Call(prefs.GetPreference<string>(Preference.WebOverview)).Return("WebOverview.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebMobileOverview)).Return("WebMobileOverview.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebSummary)).Return("WebSummary.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebMobileSummary)).Return("WebMobileSummary.xslt").Repeat.Any();
         Expect.Call(prefs.GetPreference<string>(Preference.WebInstance)).Return("WebInstance.xslt").Repeat.Any();
         return prefs;
      }

      private IEnumerable<IDisplayInstance> SetupMockDisplayInstanceCollection()
      {
         var newProtein = _mocks.DynamicMock<IProtein>();
      
         var instances = new List<IDisplayInstance>();
         var instance = _mocks.DynamicMock<IDisplayInstance>();
         SetupResult.For(instance.CurrentProtein).Return(newProtein);
         SetupResult.For(instance.CurrentLogLines).Return(new List<LogLine>());
         SetupResult.For(instance.Name).Return("Test2");
         instances.Add(instance);

         instance = _mocks.DynamicMock<IDisplayInstance>();
         // Test For - Issue 201 - Web Generation Fails when a Client with no CurrentLogLines is encountered.
         // Make sure we return null for CurrentLogLines in the second DataAggregator mock.
         SetupResult.For(instance.CurrentProtein).Return(newProtein);
         SetupResult.For(instance.CurrentLogLines).Return(null);
         SetupResult.For(instance.Name).Return("Test1");
         instances.Add(instance);

         return instances;
      }
   }
}
