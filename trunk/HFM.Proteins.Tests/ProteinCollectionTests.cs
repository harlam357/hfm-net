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
using HFM.Framework.DataTypes;

namespace HFM.Proteins.Tests
{
   [TestFixture]
   public class ProteinCollectionTests
   {
      [Test]
      public void GetProteinTest()
      {
         var mocks = new MockRepository();
         var downloader = mocks.Stub<IProjectSummaryDownloader>();
         var prefs = mocks.DynamicMock<IPreferenceSet>();
         Expect.Call(prefs.GetPreference<string>(Preference.ApplicationDataFolderPath)).Return(String.Empty).Repeat.Any();
         
         mocks.ReplayAll();
         
         var proteins = new ProteinCollection(prefs, downloader);
         proteins.Add(2483, new Protein { ProjectNumber = 2483 });
      
         IProtein p = proteins.GetProtein(2483);
         Assert.AreEqual(false, p.IsUnknown);
         p = proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
         // Do it twice to exercise the Projects Not Found List
         p = proteins.GetProtein(2482);
         Assert.AreEqual(true, p.IsUnknown);
         
         mocks.VerifyAll();
      }
   }
}
