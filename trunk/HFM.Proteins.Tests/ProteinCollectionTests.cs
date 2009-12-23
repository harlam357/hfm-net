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
using NUnit.Framework;

using Majestic12;

using HFM.Proteins;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinCollectionTests
   {
      [Test]
      public void DownloadFromStanfordTest()
      {
         Uri baseUri = new Uri(Environment.CurrentDirectory);
         Uri fileUri = new Uri(baseUri, "..\\TestFiles\\psummaryC.html");
      
         ProteinCollection.ProjectLoadLocation = fileUri;
         ProteinCollection Proteins = ProteinCollection.Instance;
         
         Assert.AreEqual(345, Proteins.Count);
         
         Protein p = Proteins.GetProtein(2483);
         Assert.AreEqual(false, p.IsUnknown);
         p = Proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
         // Do it twice to exercise the Projects Not Found List
         p = Proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
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
         Uri baseUri = new Uri(Environment.CurrentDirectory);
         Uri fileUri = new Uri(baseUri, FilePath);

         HTMLparser pSummary = ProteinCollection.InitHTMLparser(fileUri);
         HTMLchunk oChunk;

         // Parse until returned oChunk is null indicating we reached end of parsing
         while ((oChunk = pSummary.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "tr")
            {
               Assert.AreEqual("Project Number", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Server IP", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Work Unit Name", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Number of Atoms", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Preferred (days)", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Final deadline (days)", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Credit", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Frames", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Code", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Description", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Contact", ProteinCollection.GetNextThValue(pSummary));
               Assert.AreEqual("Kfactor", ProteinCollection.GetNextThValue(pSummary));
               
               return;
            }
         }
         
         Assert.Fail("Never found a table row to validate.");
      }
   }
}
