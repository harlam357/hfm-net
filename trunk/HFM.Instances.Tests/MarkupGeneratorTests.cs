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

      private const string TestFolder = "MarkupGeneratorTestOutput";
      
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         
         if (Directory.Exists(TestFolder))
         {
            Directory.Delete(TestFolder, true);
         }

         Directory.CreateDirectory(TestFolder);
      }

      [Test]
      public void GenerateXmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         ICollection<IDisplayInstance> instances = SetupMockClientInstanceCollection();
         
         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.DoXmlGeneration(TestFolder, instances);
         
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "Overview.xml")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "Instances.xml")));
         
         _mocks.VerifyAll();
      }

      [Test]
      public void GenerateHtmlTest()
      {
         IPreferenceSet prefs = SetupMockPreferenceSet();
         ICollection<IDisplayInstance> instances = SetupMockClientInstanceCollection();

         _mocks.ReplayAll();

         var markupGenerator = new MarkupGenerator(prefs);
         markupGenerator.DoHtmlGeneration(TestFolder, instances);

         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "index.html")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "mobile.html")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "summary.html")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "mobilesummary.html")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "Test1.html")));
         Assert.IsTrue(File.Exists(Path.Combine(TestFolder, "Test2.html")));
         
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

      private ICollection<IDisplayInstance> SetupMockClientInstanceCollection()
      {
         var newProtein = _mocks.DynamicMock<IProtein>();
      
         var instances = new List<IDisplayInstance>();
         var instance = _mocks.DynamicMock<IDisplayInstance>();
         SetupResult.For(instance.CurrentProtein).Return(newProtein);
         SetupResult.For(instance.CurrentLogLines).Return(new List<ILogLine>());
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
