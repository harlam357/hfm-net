/*
 * HFM.NET - Protein Dictionary Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ProteinDictionaryTests
   {
      [Test]
      public void Get_Test1()
      {
         // Arrange
         var prefs = MockRepository.GenerateStub<IPreferenceSet>();
         var downloader = MockRepository.GenerateMock<IProjectSummaryDownloader>();
         downloader.Expect(x => x.DownloadFromStanford());
         downloader.Stub(x => x.DownloadFilePath).Return("..\\..\\..\\HFM.Proteins.Tests\\TestFiles\\psummary.html");
         
         var proteins = new ProteinDictionary(prefs, downloader);
         var protein = CreateValidProtein(2483);
         proteins.Add(protein);
         // Act
         Protein p = proteins.Get(2483, true);
         Assert.IsNotNull(p);
         p = proteins.Get(2482, true);
         Assert.IsNull(p);
         // Do it twice to exercise the projects not found list
         p = proteins.Get(2482, true);
         Assert.IsNull(p);
         // Assert
         downloader.VerifyAllExpectations();
      }

      private static Protein CreateValidProtein(int projectNumber)
      {
         return new Protein { ProjectNumber = projectNumber, PreferredDays = 1, MaximumDays = 1, Credit = 1, Frames = 100 };
      }
   }
}
