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
using Rhino.Mocks;

using HFM.Framework;
using HFM.Proteins;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinCollectionTests
   {
      [Test]
      public void GetProteinTest()
      {
         MockRepository mocks = new MockRepository();
         IProjectSummaryDownloader Downloader = mocks.Stub<IProjectSummaryDownloader>();
         IPreferenceSet Prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(Prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty).Repeat.Any();
         
         mocks.ReplayAll();
         
         ProteinCollection Proteins = new ProteinCollection(Downloader, Prefs);
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
   }
}
