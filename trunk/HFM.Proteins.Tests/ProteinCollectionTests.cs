/*
 * HFM.NET - Protein Collection Class Tests
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using Majestic12;

using HFM.Framework;
using HFM.Proteins;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinCollectionTests
   {
      private MockRepository mocks;
      private IPreferenceSet _Prefs;
   
      [SetUp]
      public void Init()
      {
         mocks = new MockRepository();
         _Prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(_Prefs.GetPreference<bool>(Preference.UseProxy)).Return(false).Repeat.Any();
         Expect.Call(_Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty).Repeat.Any();
      }

      [Test]
      public void DownloadFromStanfordTest()
      {
         mocks.ReplayAll();
      
         Uri baseUri = new Uri(Environment.CurrentDirectory);
         Uri fileUri = new Uri(baseUri, "..\\TestFiles\\psummaryC.html");

         ProjectSummaryDownloader Downloader = new ProjectSummaryDownloader();
         Downloader.Dictionary = new SortedDictionary<int, IProtein>();
         Downloader.Prefs = _Prefs;
         Downloader.ReadFromProjectSummaryHtml(fileUri);

         Assert.AreEqual(345, Downloader.Dictionary.Count);

         mocks.VerifyAll();
      }

      [Test]
      public void GetProteinTest()
      {
         IProjectSummaryDownloader Downloader = mocks.Stub<IProjectSummaryDownloader>();
         mocks.ReplayAll();
         
         ProteinCollection Proteins = new ProteinCollection(Downloader, _Prefs);
         Proteins.Add(2483, new Protein(2483));
      
         IProtein p = Proteins.GetProtein(2483);
         Assert.AreEqual(false, p.IsUnknown);
         p = Proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
         // Do it twice to exercise the Projects Not Found List
         p = Proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
         
         mocks.VerifyAll();
      }

      [Test]
      public void ValidatePsummaryTableLayout()
      {
         ValidatePsummaryTableLayout("..\\TestFiles\\psummary.html");
      }

      [Test]
      public void ValidatePsummaryBTableLayout()
      {
         ValidatePsummaryTableLayout("..\\TestFiles\\psummaryB.html");
      }

      [Test]
      public void ValidatePsummaryCTableLayout()
      {
         ValidatePsummaryTableLayout("..\\TestFiles\\psummaryC.html");
      }

      public void ValidatePsummaryTableLayout(string FilePath)
      {
         ProjectSummaryDownloader Downloader = new ProjectSummaryDownloader();
         Downloader.Prefs = _Prefs;
         
         Uri baseUri = new Uri(Environment.CurrentDirectory);
         Uri fileUri = new Uri(baseUri, FilePath);

         HTMLparser pSummary = Downloader.InitHTMLparser(fileUri);
         HTMLchunk oChunk;

         // Parse until returned oChunk is null indicating we reached end of parsing
         while ((oChunk = pSummary.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "tr")
            {
               Assert.AreEqual("Project Number", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Server IP", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Work Unit Name", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Number of Atoms", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Preferred (days)", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Final deadline (days)", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Credit", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Frames", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Code", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Description", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Contact", ProjectSummaryDownloader.GetNextThValue(pSummary));
               Assert.AreEqual("Kfactor", ProjectSummaryDownloader.GetNextThValue(pSummary));
               
               return;
            }
         }
         
         Assert.Fail("Never found a table row to validate.");
      }
   }
}
