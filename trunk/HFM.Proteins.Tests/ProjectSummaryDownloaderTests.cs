/*
 * HFM.NET - Project Summary Downloader Tests
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
using System.Diagnostics;

using Castle.Windsor;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;
using HFM.Instrumentation;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProjectSummaryDownloaderTests
   {
      private IWindsorContainer _container;
      private MockRepository _mocks;

      private IPreferenceSet _prefs;
   
      [SetUp]
      public void Init()
      {
         TraceLevelSwitch.Instance.Level = TraceLevel.Verbose;
      
         _container = new WindsorContainer();
         _mocks = new MockRepository();

         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(_prefs.GetPreference<bool>(Preference.UseProxy)).Return(false).Repeat.Any();
         Expect.Call(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty).Repeat.Any();
         _container.Kernel.AddComponentInstance<IPreferenceSet>(typeof(IPreferenceSet), _prefs);
         InstanceProvider.SetContainer(_container);
      }

      [Test]
      public void DownloadFromStanfordPsummary_Test()
      {
         _mocks.ReplayAll();

         var baseUri = new Uri(Environment.CurrentDirectory);
         var fileUri = new Uri(baseUri, "..\\TestFiles\\psummary.html");

         var downloader = new ProjectSummaryDownloader();
         downloader.Dictionary = new SortedDictionary<int, IProtein>();
         downloader.Prefs = _prefs;
         downloader.ReadFromProjectSummaryHtml(fileUri);

         Assert.AreEqual(267, downloader.Dictionary.Count);

         _mocks.VerifyAll();
      }

      [Test]
      public void DownloadFromStanfordPsummaryB_Test()
      {
         _mocks.ReplayAll();

         var baseUri = new Uri(Environment.CurrentDirectory);
         var fileUri = new Uri(baseUri, "..\\TestFiles\\psummaryB.html");

         var downloader = new ProjectSummaryDownloader();
         downloader.Dictionary = new SortedDictionary<int, IProtein>();
         downloader.Prefs = _prefs;
         downloader.ReadFromProjectSummaryHtml(fileUri);

         Assert.AreEqual(263, downloader.Dictionary.Count);

         _mocks.VerifyAll();
      }

      [Test]
      public void DownloadFromStanfordPsummaryC_Test()
      {
         _mocks.ReplayAll();

         var baseUri = new Uri(Environment.CurrentDirectory);
         var fileUri = new Uri(baseUri, "..\\TestFiles\\psummaryC.html");

         var downloader = new ProjectSummaryDownloader();
         downloader.Dictionary = new SortedDictionary<int, IProtein>();
         downloader.Prefs = _prefs;
         downloader.ReadFromProjectSummaryHtml(fileUri);

         Assert.AreEqual(345, downloader.Dictionary.Count);

         _mocks.VerifyAll();
      }
   }
}
