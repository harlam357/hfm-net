/*
 * HFM.NET - Benchmark Client Class Tests
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
using System.Globalization;

using NUnit.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class BenchmarkClientTests
   {
      [Test]
      public void BenchmarkClientComparisonTests()
      {
         BenchmarkClient bcAll = new BenchmarkClient();
         BenchmarkClient bcAllClone = new BenchmarkClient();

         Assert.AreEqual(bcAll, bcAll.Client);
         Assert.AreEqual("All Clients", bcAll.NameAndPath);

         BenchmarkClient bc1 = new BenchmarkClient("C1", @"\\server\share");
         BenchmarkClient bc1Clone = new BenchmarkClient("C1", @"\\server\share");

         BenchmarkClient bc2 = new BenchmarkClient("C2", @"\\server\share");
         Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "{0} ({1})", bc2.Name, bc2.Path), bc2.NameAndPath);

         Assert.IsFalse(bcAll.Equals(null));
         Assert.IsTrue(bcAll.Equals(bcAllClone));
         Assert.IsFalse(bcAll.Equals(bc1));
         Assert.IsFalse(bcAll.Equals(bc1Clone));
         Assert.IsFalse(bcAll.Equals(bc2));

         Assert.IsTrue(bc1.Equals(bc1Clone));
         Assert.IsFalse(bc1.Equals(bc2));

         Assert.IsFalse(bc1 < bc1Clone);
         Assert.IsFalse(bc1 > bc1Clone);

         Assert.IsFalse(bc1 > bc2);
         Assert.IsTrue(bc1 < bc2);
         Assert.AreEqual(-1, bc1.CompareTo(bc2));

         Assert.IsFalse(bc2 < bc1);
         Assert.IsTrue(bc2 > bc1);
         Assert.AreEqual(1, bc2.CompareTo(bc1));

         Assert.IsFalse(bcAll > bc1);
         Assert.IsTrue(bcAll < bc1);
         Assert.AreEqual(-1, bcAll.CompareTo(bc1));

         Assert.IsFalse(bc1 < bcAll);
         Assert.IsTrue(bc1 > bcAll);
         Assert.AreEqual(1, bc1.CompareTo(bcAll));

         Assert.AreEqual(bc1.GetHashCode(), bc1Clone.GetHashCode());
      }
   }
}
