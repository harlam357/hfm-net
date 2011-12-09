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

using System;
using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ClientDictionaryTests
   {
      private IClientFactory _factory;
      private ClientDictionary _clientDictionary;

      [SetUp]
      public void Init()
      {
         _factory = MockRepository.GenerateMock<IClientFactory>();
         _clientDictionary = new ClientDictionary(_factory);
      }

      [Test]
      public void LoadTest1()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientDictionary.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest2()
      {
         // Arrage
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientDictionary.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest3()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Remove("test");
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientDictionary.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest4()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         ClientDataDirtyEventArgs dataDirtyEventArgs = null;
         _clientDictionary.ClientDataDirty += (sender, e) => dataDirtyEventArgs = e;
         // Assert
         _clientDictionary.Load(settingsCollection);
         // Assert
         Assert.IsTrue(dictionaryChangedFired);
         Assert.IsNull(dataDirtyEventArgs.Name);
         _factory.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void LoadTestArgumentNullException1()
      {
         _clientDictionary.Load(null);
      }

      [Test]
      public void AddTest1()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.IsDirty);
      }

      [Test]
      public void AddTest2()
      {
         // Arrange
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         ClientDataDirtyEventArgs dataDirtyEventArgs = null;
         _clientDictionary.ClientDataDirty += (sender, e) => dataDirtyEventArgs = e;
         // Act
         _clientDictionary.Add("test", new LegacyClient());
         // Assert
         Assert.IsTrue(dictionaryChangedFired);
         Assert.AreEqual("test", dataDirtyEventArgs.Name);
      }

      [Test]
      public void AddTest3()
      {
         // Arrange
         var settings = new ClientSettings { Name = "test" };
         _factory.Expect(x => x.Create(settings)).Return(
            new FahClient(MockRepository.GenerateStub<IFahClientInterface>()));
         // Act
         _clientDictionary.Add(settings);
         // Assert
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void AddVerifySubscriptionTest1()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         // Act
         _clientDictionary.Add("test", client);
         // Assert
         client.AssertWasCalled(x => x.SlotsChanged += Arg<EventHandler>.Is.Anything);
         client.AssertWasCalled(x => x.RetrievalFinished += Arg<EventHandler>.Is.Anything);
      }

      [Test]
      public void AddVerifySubscriptionTest2()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         bool clientDataInvalidatedFired = false;
         _clientDictionary.ClientDataInvalidated += delegate { clientDataInvalidatedFired = true; };
         _clientDictionary.Add("test", client);
         // Act
         client.Raise(x => x.SlotsChanged += null, this, EventArgs.Empty);
         // Assert
         Assert.IsTrue(clientDataInvalidatedFired);
      }

      [Test]
      public void AddVerifySubscriptionTest3()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         bool clientDataInvalidatedFired = false;
         _clientDictionary.ClientDataInvalidated += delegate { clientDataInvalidatedFired = true; };
         _clientDictionary.Add("test", client);
         // Act
         client.Raise(x => x.RetrievalFinished += null, this, EventArgs.Empty);
         // Assert
         Assert.IsTrue(clientDataInvalidatedFired);
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void AddTestArgumentNullException1()
      {
         // ReSharper disable AssignNullToNotNullAttribute
         _clientDictionary.Add(null, new LegacyClient());
         // ReSharper restore AssignNullToNotNullAttribute
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void AddTestArgumentNullException2()
      {
         _clientDictionary.Add("test", null);
      }

      [Test]
      [ExpectedException]
      public void AddTestArgumentNullException3()
      {
         _clientDictionary.Add(null);
      }

      [Test]
      public void EditTest1()
      {
         // Arrange
         _clientDictionary.Add("test", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "test", Path = "/home/harlam357/" } });
         Assert.AreEqual(1, _clientDictionary.Count);
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         ClientEditedEventArgs editedEventArgs = null;
         _clientDictionary.ClientEdited += (sender, e) => editedEventArgs = e;
         ClientDataDirtyEventArgs dataDirtyEventArgs = null;
         _clientDictionary.ClientDataDirty += (sender, e) => dataDirtyEventArgs = e;
         // Act
         _clientDictionary.Edit("test", new ClientSettings(ClientType.Legacy) { Name = "test2", Path = "/home/harlam357/FAH/" });
         // Assert
         Assert.AreEqual(1, _clientDictionary.Count);
         Assert.IsTrue(_clientDictionary.ContainsKey("test2"));
         Assert.IsTrue(dictionaryChangedFired);
         Assert.AreEqual("test", editedEventArgs.PreviousName);
         Assert.AreEqual("/home/harlam357/", editedEventArgs.PreviousPath);
         Assert.AreEqual("test2", editedEventArgs.NewName);
         Assert.AreEqual("/home/harlam357/FAH/", editedEventArgs.NewPath);
         Assert.AreEqual("test2", dataDirtyEventArgs.Name);
      }

      [Test]
      public void EditTest2()
      {
         // Arrange
         _clientDictionary.Add("test", new FahClient(MockRepository.GenerateStub<IFahClientInterface>()) { Settings = new ClientSettings { Name = "test", Server = "server", Port = 36330 } });
         Assert.AreEqual(1, _clientDictionary.Count);
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         ClientEditedEventArgs editedEventArgs = null;
         _clientDictionary.ClientEdited += (sender, e) => editedEventArgs = e;
         ClientDataDirtyEventArgs dataDirtyEventArgs = null;
         _clientDictionary.ClientDataDirty += (sender, e) => dataDirtyEventArgs = e;
         // Act
         _clientDictionary.Edit("test", new ClientSettings { Name = "test2", Server = "server1", Port = 36331 });
         // Assert
         Assert.AreEqual(1, _clientDictionary.Count);
         Assert.IsTrue(_clientDictionary.ContainsKey("test2"));
         Assert.IsTrue(dictionaryChangedFired);
         Assert.AreEqual("test", editedEventArgs.PreviousName);
         Assert.AreEqual("server:36330", editedEventArgs.PreviousPath);
         Assert.AreEqual("test2", editedEventArgs.NewName);
         Assert.AreEqual("server1:36331", editedEventArgs.NewPath);
         Assert.AreEqual("test2", dataDirtyEventArgs.Name);
      }

      [Test]
      [ExpectedException(typeof(ArgumentException))]
      public void EditTestArgumentException1()
      {
         // Arrange
         _clientDictionary.Add("test", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "test", Path = "/home/harlam357/" } });
         _clientDictionary.Add("other", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "other", Path = "/home/other/" } });
         Assert.AreEqual(2, _clientDictionary.Count);
         // Act
         _clientDictionary.Edit("test", new ClientSettings(ClientType.Legacy) { Name = "other", Path = "/home/harlam357/FAH/" });
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void EditTestArgumentNullException1()
      {
         _clientDictionary.Edit(null, new ClientSettings());
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void EditTestArgumentNullException2()
      {
         _clientDictionary.Edit("test", null);
      }

      [Test]
      public void RemoveTest1()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         Assert.IsFalse(_clientDictionary.Remove("test"));
         Assert.IsFalse(_clientDictionary.IsDirty);
      }

      [Test]
      public void RemoveTest2()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.Remove("test"));
         Assert.IsTrue(_clientDictionary.IsDirty);
      }

      [Test]
      public void RemoveTest3()
      {
         // Arrange
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         bool clientRemovedFired = false;
         _clientDictionary.ClientRemoved += delegate { clientRemovedFired = true; };
         // Act
         Assert.IsFalse(_clientDictionary.Remove("test"));
         // Assert
         Assert.IsFalse(dictionaryChangedFired);
         Assert.IsFalse(clientRemovedFired);
      }

      [Test]
      public void RemoveTest4()
      {
         // Arrange
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         bool clientRemovedFired = false;
         _clientDictionary.ClientRemoved += delegate { clientRemovedFired = true; };
         _clientDictionary.Add("test", new LegacyClient());
         // Act
         Assert.IsTrue(_clientDictionary.Remove("test"));
         // Assert
         Assert.IsTrue(dictionaryChangedFired);
         Assert.IsTrue(clientRemovedFired);
      }

      [Test]
      public void RemoveTest5()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         _clientDictionary.Add("test", client);
         client.Expect(x => x.ClearEventSubscriptions());
         client.Expect(x => x.Abort());
         // Act
         _clientDictionary.Remove("test");
         // Assert
         client.VerifyAllExpectations();
      }

      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void RemoveArgumentNullExceptionTest1()
      {
         // ReSharper disable AssignNullToNotNullAttribute
         Assert.IsFalse(_clientDictionary.Remove(null));
         // ReSharper restore AssignNullToNotNullAttribute
      }

      [Test]
      public void ClearTest1()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.IsDirty);
      }

      [Test]
      public void ClearTest2()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.IsDirty);
      }

      [Test]
      public void ClearTest3()
      {
         Assert.IsFalse(_clientDictionary.IsDirty);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Remove("test");
         Assert.IsTrue(_clientDictionary.IsDirty);
         _clientDictionary.Clear();
         Assert.IsFalse(_clientDictionary.IsDirty);
      }

      [Test]
      public void ClearTest4()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsFalse(dictionaryChangedFired);
      }

      [Test]
      public void ClearTest5()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientDictionary.Count);
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsTrue(dictionaryChangedFired);
      }

      [Test]
      public void ClearTest6()
      {
         Assert.AreEqual(0, _clientDictionary.Count);
         _clientDictionary.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientDictionary.Count);
         _clientDictionary.Remove("test");
         Assert.AreEqual(0, _clientDictionary.Count);
         bool dictionaryChangedFired = false;
         _clientDictionary.DictionaryChanged += delegate { dictionaryChangedFired = true; };
         _clientDictionary.Clear();
         Assert.IsFalse(dictionaryChangedFired);
      }

      [Test]
      public void ClearTest7()
      {
         // Arrange
         var client1 = MockRepository.GenerateMock<IClient>();
         _clientDictionary.Add("test", client1);
         client1.Expect(x => x.ClearEventSubscriptions());
         var client2 = MockRepository.GenerateMock<IClient>();
         _clientDictionary.Add("test2", client2);
         client2.Expect(x => x.ClearEventSubscriptions());
         // Act
         _clientDictionary.Clear();
         // Assert
         client1.VerifyAllExpectations();
         client2.VerifyAllExpectations();
      }
   }
}
