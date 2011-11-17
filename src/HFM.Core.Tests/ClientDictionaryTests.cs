/*
 * HFM.NET - Client Dictionary Class Tests
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

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ClientDictionaryTests
   {
      private ClientDictionary _clientDictionary;

      [SetUp]
      public void Init()
      {
         _clientDictionary = new ClientDictionary();
      }

      [Test]
      public void LoadInstancesTest1()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.LoadClients(new[] { new LegacyClient { Settings = new ClientSettings { Name = "test"} } });
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void LoadInstancesTest2()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.LoadClients(new[] { new LegacyClient { Settings = new ClientSettings { Name = "test" } } });
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void LoadInstancesTest3()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.Remove("test");
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.LoadClients(new[] { new LegacyClient { Settings = new ClientSettings { Name = "test" } } });
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void AddTest1()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Dirty);
      }

      [Test]
      public void AddTest2()
      {
         bool collectionChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void RemoveTest1()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         Assert.IsFalse(_clientDictionary.Remove("test"));
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void RemoveTest2()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Remove("test"));
         Assert.IsTrue(_clientDictionary.Dirty);
      }

      [Test]
      public void RemoveTest3()
      {
         bool collectionChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         Assert.IsFalse(_clientDictionary.Remove("test"));
         Assert.IsFalse(collectionChangedFired);
      }

      [Test]
      public void RemoveTest4()
      {
         bool collectionChangedFired = false;
         _clientDictionary.Add("test", new LegacyClient());
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         Assert.IsTrue(_clientDictionary.Remove("test"));
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void ClearTest1()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void ClearTest2()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void ClearTest3()
      {
         Assert.IsFalse(_clientDictionary.Dirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.Remove("test");
         Assert.IsTrue(_clientDictionary.Dirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.Dirty);
      }

      [Test]
      public void ClearTest4()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         bool collectionChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsFalse(collectionChangedFired);
      }

      [Test]
      public void ClearTest5()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientDictionary.Count);
         bool collectionChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsTrue(collectionChangedFired);
      }

      [Test]
      public void ClearTest6()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientDictionary.Count);
         _clientDictionary.Remove("test");
         Assert.AreEqual(0, _clientDictionary.Count);
         bool collectionChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { collectionChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsFalse(collectionChangedFired);
      }
   }
}
