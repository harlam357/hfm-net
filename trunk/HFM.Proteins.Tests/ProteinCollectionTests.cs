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
         Uri fileUri = new Uri(baseUri, "..\\TestFiles\\psummary.html");
      
         ProteinCollection.ProjectLoadLocation = fileUri;
         ProteinCollection Proteins = ProteinCollection.Instance;
         
         Assert.AreEqual(196, Proteins.Count);
         
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
         Uri baseUri = new Uri(Environment.CurrentDirectory);
         Uri fileUri = new Uri(baseUri, "..\\TestFiles\\psummary.html");

         HTMLparser pSummary = ProteinCollection.InitHTMLparser(fileUri);
         HTMLchunk oChunk;

         // Parse until returned oChunk is null indicating we reached end of parsing
         while ((oChunk = pSummary.ParseNext()) != null)
         {
            // Look for an Open "tr" Tag
            if (oChunk.oType.Equals(HTMLchunkType.OpenTag) &&
                oChunk.sTag.ToLower() == "tr")
            {
               Assert.AreEqual("Project Number", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Server IP", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Work Unit Name", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Number of Atoms", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Preferred (days)", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Final deadline (days)", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Credit", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Frames", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Code", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Description", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Contact", ProteinCollection.GetNextTdValue(pSummary));
               Assert.AreEqual("Kfactor", ProteinCollection.GetNextTdValue(pSummary));
               
               return;
            }
         }
         
         Assert.Fail("Never found a table row to validate.");
      }
   }
}
