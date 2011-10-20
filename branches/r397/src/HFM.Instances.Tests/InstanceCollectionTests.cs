/*
 * HFM.NET - Instance Collection Class Tests
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

using NUnit.Framework;

using HFM.Framework.DataTypes;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class InstanceCollectionTests
   {
      private InstanceCollection _instanceCollection;

      [SetUp]
      public void Init()
      {
         _instanceCollection = new InstanceCollection();
      }

      [Test]
      public void LoadInstancesTest1()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.LoadInstances(new[] { new ClientInstance { Settings = new ClientInstanceSettings { InstanceName = "test"} } });
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void LoadInstancesTest2()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.LoadInstances(new[] { new ClientInstance { Settings = new ClientInstanceSettings { InstanceName = "test" } } });
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void LoadInstancesTest3()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.Remove("test");
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.LoadInstances(new[] { new ClientInstance { Settings = new ClientInstanceSettings { InstanceName = "test" } } });
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void AddTest1()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Dirty);
      }

      [Test]
      public void AddTest2()
      {
         bool collectionChangedFired = false;
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void RemoveTest1()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         Assert.IsFalse(_instanceCollection.Remove("test"));
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void RemoveTest2()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Remove("test"));
         Assert.IsTrue(_instanceCollection.Dirty);
      }

      [Test]
      public void RemoveTest3()
      {
         bool collectionChangedFired = false;
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         Assert.IsFalse(_instanceCollection.Remove("test"));
         Assert.IsFalse(collectionChangedFired);
      }

      [Test]
      public void RemoveTest4()
      {
         bool collectionChangedFired = false;
         _instanceCollection.Add("test", new ClientInstance());
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         Assert.IsTrue(_instanceCollection.Remove("test"));
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void ClearTest1()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Clear();
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void ClearTest2()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.Clear();
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void ClearTest3()
      {
         Assert.IsFalse(_instanceCollection.Dirty);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.Remove("test");
         Assert.IsTrue(_instanceCollection.Dirty);
         _instanceCollection.Clear();
         Assert.IsFalse(_instanceCollection.Dirty);
      }

      [Test]
      public void ClearTest4()
      {
         Assert.AreEqual(0, _instanceCollection.Count);
         bool collectionChangedFired = false;
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         _instanceCollection.Clear();
         Assert.IsFalse(collectionChangedFired);
      }

      [Test]
      public void ClearTest5()
      {
         Assert.AreEqual(0, _instanceCollection.Count);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.AreEqual(1, _instanceCollection.Count);
         bool collectionChangedFired = false;
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         _instanceCollection.Clear();
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void ClearTest6()
      {
         Assert.AreEqual(0, _instanceCollection.Count);
         _instanceCollection.Add("test", new ClientInstance());
         Assert.AreEqual(1, _instanceCollection.Count);
         _instanceCollection.Remove("test");
         Assert.AreEqual(0, _instanceCollection.Count);
         bool collectionChangedFired = false;
         _instanceCollection.CollectionChanged += delegate { collectionChangedFired = true; };
         _instanceCollection.Clear();
         Assert.IsFalse(collectionChangedFired);
      }
   }
}
