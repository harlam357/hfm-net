/*
 * HFM.NET - Completed Units File Reader Tests
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using NUnit.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class CompletedUnitsFileReaderTests
   {
      [Test]
      public void ReadCompletedUnitsTest()
      {
         var completedUnitsReader = new CompletedUnitsFileReader { CompletedUnitsFilePath = "..\\..\\TestFiles\\CompletedUnits.csv" };
         completedUnitsReader.Process();
         Assert.AreEqual(0, completedUnitsReader.Result.Duplicates);
         Assert.AreEqual(44, completedUnitsReader.Result.Entries.Count);
         Assert.AreEqual(0, completedUnitsReader.Result.ErrorLines.Count);
      }

      [Test]
      public void ReadCompletedUnitsLargeTest()
      {
         var completedUnitsReader = new CompletedUnitsFileReader { CompletedUnitsFilePath = "..\\..\\TestFiles\\CompletedUnitsLarge.csv" };
         completedUnitsReader.Process();
         Assert.AreEqual(8, completedUnitsReader.Result.Duplicates);
         Assert.AreEqual(6994, completedUnitsReader.Result.Entries.Count);
         Assert.AreEqual(153, completedUnitsReader.Result.ErrorLines.Count);
      }
   }
}
