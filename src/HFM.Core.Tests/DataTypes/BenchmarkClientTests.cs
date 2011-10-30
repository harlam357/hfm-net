/*
 * HFM.NET - Benchmark Client Class Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

using HFM.Core.DataTypes;

namespace HFM.Core.Tests.DataTypes
{
   [TestFixture]
   public class BenchmarkClientTests
   {
      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ConstructorArgumentException1()
      {
         new BenchmarkClient(null, "path");
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ConstructorArgumentException2()
      {
         new BenchmarkClient(String.Empty, "path");
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ConstructorArgumentException3()
      {
         new BenchmarkClient("name", null);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void ConstructorArgumentException4()
      {
         new BenchmarkClient("name", String.Empty);
      }

      [Test]
      public void ValueTest1()
      {
         var client = new BenchmarkClient();
         Assert.AreEqual(String.Empty, client.Name);
         Assert.AreEqual(String.Empty, client.Path);
         Assert.AreEqual("All Clients", client.NameAndPath);
         Assert.IsTrue(client.AllClients);
         Assert.AreSame(client, client.Client);
      }

      [Test]
      public void ValueTest2()
      {
         var client = new BenchmarkClient("name", "path");
         Assert.AreEqual("name", client.Name);
         Assert.AreEqual("path", client.Path);
         Assert.AreEqual("name (path)", client.NameAndPath);
         Assert.IsFalse(client.AllClients);
         Assert.AreSame(client, client.Client);
      }

      [Test]
      public void HashCodeTest1()
      {
         var client = new BenchmarkClient();
         Assert.AreEqual(1, client.GetHashCode());
      }

      [Test]
      public void HashCodeTest2()
      {
         var client = new BenchmarkClient("name", "path");
         Assert.AreEqual(1548099437, client.GetHashCode());
      }

      [Test]
      public void ComparisonTest1()
      {
         var client1 = new BenchmarkClient();
         var client2 = new BenchmarkClient();
         // calls Object.Equals() override
         Assert.AreEqual(client1, client2);
      }

      [Test]
      public void ComparisonTest2()
      {
         var client1 = new BenchmarkClient();
         var client2 = new object();
         // calls Object.Equals() override
         Assert.AreNotEqual(client1, client2);
      }

      [Test]
      public void ComparisonTest3()
      {
         var client1 = new BenchmarkClient();
         var client2 = new BenchmarkClient();
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest4()
      {
         var client1 = new BenchmarkClient("name", "path");
         var client2 = new BenchmarkClient();
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ComparisonTest5()
      {
         var client1 = new BenchmarkClient();
         var client2 = new BenchmarkClient("name", "path");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) < 0);
      }

      [Test]
      public void ComparisonTest6()
      {
         var client1 = new BenchmarkClient("name", "path");
         var client2 = new BenchmarkClient("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest7()
      {
         var client1 = new BenchmarkClient("name", "path\\");
         var client2 = new BenchmarkClient("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest8()
      {
         var client1 = new BenchmarkClient("name", "path");
         var client2 = new BenchmarkClient("name", "path\\");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest9()
      {
         var client1 = new BenchmarkClient("name", "path/");
         var client2 = new BenchmarkClient("name", "path");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest10()
      {
         var client1 = new BenchmarkClient("name", "path");
         var client2 = new BenchmarkClient("name", "path/");
         Assert.IsTrue(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) == 0);
      }

      [Test]
      public void ComparisonTest11()
      {
         var client1 = new BenchmarkClient("name", "path\\");
         var client2 = new BenchmarkClient("name", "path/");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ComparisonTest12()
      {
         var client1 = new BenchmarkClient("name1", "path\\");
         var client2 = new BenchmarkClient("name", "path\\");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) > 0);
      }

      [Test]
      public void ComparisonTest13()
      {
         var client1 = new BenchmarkClient("name", "path\\");
         var client2 = new BenchmarkClient("name2", "path\\");
         Assert.IsFalse(client1.Equals(client2));
         Assert.IsTrue(client1.CompareTo(client2) < 0);
      }

      [Test]
      public void ComparisonTest14()
      {
         var client1 = new BenchmarkClient();
         Assert.IsFalse(client1.Equals(null));
         Assert.IsTrue(client1.CompareTo(null) > 0);
      }

      [Test]
      public void ComparisonTest15()
      {
         var client1 = new BenchmarkClient();
         var client2 = new BenchmarkClient();
         Assert.IsTrue(client1 == client2);
         Assert.IsFalse(client1 != client2);
         Assert.IsFalse(client1 < client2);
         Assert.IsFalse(client1 > client2);
      }

      [Test]
      public void ComparisonTest16()
      {
         var client1 = new BenchmarkClient();
         var client2 = new BenchmarkClient("name", "path");
         Assert.IsFalse(client1 == client2);
         Assert.IsTrue(client1 != client2);
         Assert.IsTrue(client1 < client2);
         Assert.IsFalse(client1 > client2);
      }

      [Test]
      public void ComparisonTest17()
      {
         var client1 = new BenchmarkClient("name", "path");
         var client2 = new BenchmarkClient();
         Assert.IsFalse(client1 == client2);
         Assert.IsTrue(client1 != client2);
         Assert.IsFalse(client1 < client2);
         Assert.IsTrue(client1 > client2);
      }

      [Test]
      public void ComparisonTest18()
      {
         var client1 = new BenchmarkClient();
         Assert.IsFalse(client1 == null);
         Assert.IsTrue(client1 != null);
         Assert.IsFalse(client1 < null);
         Assert.IsTrue(client1 > null);
      }

      [Test]
      public void ComparisonTest19()
      {
         var client1 = new BenchmarkClient();
         Assert.IsFalse(null == client1);
         Assert.IsTrue(null != client1);
         Assert.IsTrue(null < client1);
         Assert.IsFalse(null > client1);
      }
   }
}
