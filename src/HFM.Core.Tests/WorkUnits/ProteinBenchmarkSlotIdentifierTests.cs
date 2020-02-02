/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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

namespace HFM.Core.WorkUnits
{
   [TestFixture]
   public class ProteinBenchmarkSlotIdentifierTests
   {
      [Test]
      public void ProteinBenchmarkSlotIdentifier_Constructor_ArgumentException_Test1()
      {
         Assert.Throws<ArgumentException>(() => new ProteinBenchmarkSlotIdentifier(null, "path"));
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Constructor_ArgumentException_Test2()
      {
         Assert.Throws<ArgumentException>(() => new ProteinBenchmarkSlotIdentifier(String.Empty, "path"));
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Constructor_ArgumentException_Test3()
      {
         Assert.Throws<ArgumentException>(() => new ProteinBenchmarkSlotIdentifier("name", null));
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Constructor_ArgumentException_Test4()
      {
         Assert.Throws<ArgumentException>(() => new ProteinBenchmarkSlotIdentifier("name", String.Empty));
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Value_Test1()
      {
         var client = new ProteinBenchmarkSlotIdentifier();
         Assert.AreEqual("All Slots", client.Value);
         Assert.IsTrue(client.AllSlots);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Value_Test2()
      {
         var client = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.AreEqual("name (path)", client.Value);
         Assert.IsFalse(client.AllSlots);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_GetHashCode_Test1()
      {
         var client = new ProteinBenchmarkSlotIdentifier();
         Assert.AreEqual(0, client.GetHashCode());
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_GetHashCode_Test2()
      {
         var client = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.AreEqual(-331019282, client.GetHashCode());
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test1()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new ProteinBenchmarkSlotIdentifier();
         // calls Object.Equals() override
         Assert.AreEqual(client1, client2);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test2()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new object();
         // calls Object.Equals() override
         Assert.AreNotEqual(client1, client2);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test3()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test4()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path");
         var client2 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test5()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) < 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test6()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test7()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path\\");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test8()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path\\");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test9()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path/");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test10()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path/");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test11()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path\\to\\folder\\");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path/to/folder/");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test12()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name1", "path\\");
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path\\");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test13()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path\\");
         var client2 = new ProteinBenchmarkSlotIdentifier("name2", "path\\");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) < 0);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test14()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsFalse(client1.Equals(null));
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test15()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsTrue(client1 == client2);
         Assert.IsFalse(client1 != client2);
         Assert.IsFalse(client1 < client2);
         Assert.IsFalse(client1 > client2);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test16()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         var client2 = new ProteinBenchmarkSlotIdentifier("name", "path");
         Assert.IsFalse(client1 == client2);
         Assert.IsTrue(client1 != client2);
         Assert.IsTrue(client1 < client2);
         Assert.IsFalse(client1 > client2);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test17()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier("name", "path");
         var client2 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsFalse(client1 == client2);
         Assert.IsTrue(client1 != client2);
         Assert.IsFalse(client1 < client2);
         Assert.IsTrue(client1 > client2);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test18()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsFalse(client1 == null);
         Assert.IsTrue(client1 != null);
         //Assert.IsFalse(client1 < null);
         //Assert.IsFalse(client1 > null);
      }

      [Test]
      public void ProteinBenchmarkSlotIdentifier_Comparison_Test19()
      {
         var client1 = new ProteinBenchmarkSlotIdentifier();
         Assert.IsFalse(null == client1);
         Assert.IsTrue(null != client1);
         //Assert.IsFalse(null < client1);
         //Assert.IsFalse(null > client1);
      }
   }
}
