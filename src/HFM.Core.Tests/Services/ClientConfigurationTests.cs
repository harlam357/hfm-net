/*
 * HFM.NET - Client Configuration Class Tests
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

using System;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client;
using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ClientConfigurationTests
   {
      private IClientFactory _factory;
      private ClientConfiguration _clientConfiguration;

      [SetUp]
      public void Init()
      {
         _factory = MockRepository.GenerateMock<IClientFactory>();
         _clientConfiguration = new ClientConfiguration(_factory);
      }

      [Test]
      public void ClientDictionary_ArgumentNullException_Test()
      {
         Assert.Throws(typeof(ArgumentNullException), () => new ClientConfiguration(null));
      }

      [Test]
      public void LoadTest1()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest2()
      {
         // Arrage
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest3()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         // Act
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Remove("test");
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Load(settingsCollection);
         // Assert
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadTest4()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { new LegacyClient { Settings = settingsCollection[0] } });
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         // Act
         _clientConfiguration.Load(settingsCollection);
         // Assert
         Assert.AreEqual(ConfigurationChangedType.Add, eventArgs.ChangedType);
         Assert.IsNull(eventArgs.Client);
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void LoadVerifySubscriptionTest1()
      {
         // Arrange
         var settingsCollection = new[] { new ClientSettings(ClientType.Legacy) { Name = "test" } };
         var client1 = MockRepository.GenerateMock<IClient>();
         client1.Stub(x => x.Settings).Return(settingsCollection[0]);
         _factory.Expect(x => x.CreateCollection(settingsCollection)).Return(new[] { client1 });
         // Act
         _clientConfiguration.Load(settingsCollection);
         // Assert
         _factory.VerifyAllExpectations();
         client1.AssertWasCalled(x => x.SlotsChanged += Arg<EventHandler>.Is.Anything);
         client1.AssertWasCalled(x => x.RetrievalFinished += Arg<EventHandler>.Is.Anything);
      }

      [Test]
      public void LoadTestArgumentNullException1()
      {
         Assert.Throws(typeof(ArgumentNullException), () => _clientConfiguration.Load(null));
      }

      [Test]
      public void AddTest1()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.IsDirty);
      }

      [Test]
      public void AddTest2()
      {
         // Arrange
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         // Act
         _clientConfiguration.Add("test", new LegacyClient());
         // Assert
         Assert.AreEqual(ConfigurationChangedType.Add, eventArgs.ChangedType);
         Assert.IsNotNull(eventArgs.Client);
      }

      [Test]
      public void AddTest3()
      {
         // Arrange
         var settings = new ClientSettings { Name = "test" };
         _factory.Expect(x => x.Create(settings)).Return(
            new FahClient(MockRepository.GenerateStub<IMessageConnection>()));
         // Act
         _clientConfiguration.Add(settings);
         // Assert
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void AddVerifySubscriptionTest1()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         // Act
         _clientConfiguration.Add("test", client);
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
         _clientConfiguration.DictionaryChanged += (sender, args) =>
         {
            if (args.ChangedType == ConfigurationChangedType.Invalidate) clientDataInvalidatedFired = true;
         };
         _clientConfiguration.Add("test", client);
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
         _clientConfiguration.DictionaryChanged += (sender, args) =>
         {
            if (args.ChangedType == ConfigurationChangedType.Invalidate) clientDataInvalidatedFired = true;
         };
         _clientConfiguration.Add("test", client);
         // Act
         client.Raise(x => x.RetrievalFinished += null, this, EventArgs.Empty);
         // Assert
         Assert.IsTrue(clientDataInvalidatedFired);
      }

      [Test]
      public void AddTestArgumentNullException1()
      {
         Assert.Throws(typeof(ArgumentNullException), () => _clientConfiguration.Add(null, new LegacyClient()));
      }

      [Test]
      public void AddTestArgumentNullException2()
      {
         Assert.Throws(typeof(ArgumentNullException), () =>  _clientConfiguration.Add("test", null));
      }

      [Test]
      public void AddTestArgumentNullException3()
      {
         Assert.Throws(typeof(ArgumentNullException), () => _clientConfiguration.Add(null));
      }

      [Test]
      public void EditTest1()
      {
         // Arrange
         _clientConfiguration.Add("test", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "test", Path = "/home/harlam357/" } });
         Assert.AreEqual(1, _clientConfiguration.Count);
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         ClientEditedEventArgs editedEventArgs = null;
         _clientConfiguration.ClientEdited += (sender, e) => editedEventArgs = e;
         // Act
         _clientConfiguration.Edit("test", new ClientSettings(ClientType.Legacy) { Name = "test2", Path = "/home/harlam357/FAH/" });
         // Assert
         Assert.AreEqual(1, _clientConfiguration.Count);
         Assert.IsTrue(_clientConfiguration.ContainsKey("test2"));
         Assert.AreEqual(ConfigurationChangedType.Edit, eventArgs.ChangedType);
         Assert.AreEqual("test2", eventArgs.Client.Settings.Name);
         Assert.AreEqual("/home/harlam357/FAH/", eventArgs.Client.Settings.Path);
         Assert.AreEqual("test", editedEventArgs.PreviousName);
         Assert.AreEqual("/home/harlam357/", editedEventArgs.PreviousPath);
         Assert.AreEqual("test2", editedEventArgs.NewName);
         Assert.AreEqual("/home/harlam357/FAH/", editedEventArgs.NewPath);
      }

      [Test]
      public void EditTest2()
      {
         // Arrange
         _clientConfiguration.Add("test", new FahClient(MockRepository.GenerateStub<IMessageConnection>()) { Settings = new ClientSettings { Name = "test", Server = "server", Port = 36330 } });
         Assert.AreEqual(1, _clientConfiguration.Count);
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         ClientEditedEventArgs editedEventArgs = null;
         _clientConfiguration.ClientEdited += (sender, e) => editedEventArgs = e;
         // Act
         _clientConfiguration.Edit("test", new ClientSettings { Name = "test2", Server = "server1", Port = 36331 });
         // Assert
         Assert.AreEqual(1, _clientConfiguration.Count);
         Assert.IsTrue(_clientConfiguration.ContainsKey("test2"));
         Assert.AreEqual(ConfigurationChangedType.Edit, eventArgs.ChangedType);
         Assert.AreEqual("test2", eventArgs.Client.Settings.Name);
         Assert.AreEqual("server1-36331", eventArgs.Client.Settings.DataPath());
         Assert.AreEqual("test", editedEventArgs.PreviousName);
         Assert.AreEqual("server-36330", editedEventArgs.PreviousPath);
         Assert.AreEqual("test2", editedEventArgs.NewName);
         Assert.AreEqual("server1-36331", editedEventArgs.NewPath);
      }

      [Test]
      public void EditTestArgumentException1()
      {
         // Arrange
         _clientConfiguration.Add("test", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "test", Path = "/home/harlam357/" } });
         _clientConfiguration.Add("other", new LegacyClient { Settings = new ClientSettings(ClientType.Legacy) { Name = "other", Path = "/home/other/" } });
         Assert.AreEqual(2, _clientConfiguration.Count);
         // Act
         Assert.Throws(typeof(ArgumentException), () => _clientConfiguration.Edit("test", new ClientSettings(ClientType.Legacy) { Name = "other", Path = "/home/harlam357/FAH/" }));
      }

      [Test]
      public void EditTestArgumentNullException1()
      {
         Assert.Throws(typeof(ArgumentNullException), () => _clientConfiguration.Edit(null, new ClientSettings()));
      }

      [Test]
      public void EditTestArgumentNullException2()
      {
         Assert.Throws(typeof(ArgumentNullException), () => _clientConfiguration.Edit("test", null));
      }

      [Test]
      public void RemoveTest1()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         Assert.IsFalse(_clientConfiguration.Remove("test"));
         Assert.IsFalse(_clientConfiguration.IsDirty);
      }

      [Test]
      public void RemoveTest2()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.Remove("test"));
         Assert.IsTrue(_clientConfiguration.IsDirty);
      }

      [Test]
      public void RemoveTest3()
      {
         // Arrange
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         // Act
         Assert.IsFalse(_clientConfiguration.Remove("test"));
         // Assert
         Assert.IsNull(eventArgs);
      }

      [Test]
      public void RemoveTest4()
      {
         // Arrange
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         _clientConfiguration.Add("test", new LegacyClient());
         // Act
         Assert.IsTrue(_clientConfiguration.Remove("test"));
         // Assert
         Assert.AreEqual(ConfigurationChangedType.Remove, eventArgs.ChangedType);
         Assert.IsNotNull(eventArgs.Client);
      }

      [Test]
      public void RemoveTest5()
      {
         // Arrange
         var client = MockRepository.GenerateMock<IClient>();
         _clientConfiguration.Add("test", client);
         client.Expect(x => x.Abort());
         _factory.Expect(x => x.Release(client));
         // Act
         _clientConfiguration.Remove("test");
         // Assert
         client.VerifyAllExpectations();
         _factory.VerifyAllExpectations();
      }

      [Test]
      public void RemoveArgumentNullExceptionTest1()
      {
         Assert.Throws(typeof(ArgumentNullException), () => Assert.IsFalse(_clientConfiguration.Remove(null)));
      }

      [Test]
      public void ClearTest1()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Clear();
         Assert.IsFalse(_clientConfiguration.IsDirty);
      }

      [Test]
      public void ClearTest2()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Clear();
         Assert.IsFalse(_clientConfiguration.IsDirty);
      }

      [Test]
      public void ClearTest3()
      {
         Assert.IsFalse(_clientConfiguration.IsDirty);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Remove("test");
         Assert.IsTrue(_clientConfiguration.IsDirty);
         _clientConfiguration.Clear();
         Assert.IsFalse(_clientConfiguration.IsDirty);
      }

      [Test]
      public void ClearTest4()
      {
         Assert.AreEqual(0, _clientConfiguration.Count);
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         _clientConfiguration.Clear();
         Assert.IsNull(eventArgs);
      }

      [Test]
      public void ClearTest5()
      {
         Assert.AreEqual(0, _clientConfiguration.Count);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientConfiguration.Count);
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         _clientConfiguration.Clear();
         Assert.AreEqual(ConfigurationChangedType.Clear, eventArgs.ChangedType);
         Assert.IsNull(eventArgs.Client);
      }

      [Test]
      public void ClearTest6()
      {
         Assert.AreEqual(0, _clientConfiguration.Count);
         _clientConfiguration.Add("test", new LegacyClient());
         Assert.AreEqual(1, _clientConfiguration.Count);
         _clientConfiguration.Remove("test");
         Assert.AreEqual(0, _clientConfiguration.Count);
         ConfigurationChangedEventArgs eventArgs = null;
         _clientConfiguration.DictionaryChanged += (sender, e) => { eventArgs = e; };
         _clientConfiguration.Clear();
         Assert.IsNull(eventArgs);
      }

      [Test]
      public void ClearTest7()
      {
         // Arrange
         var client1 = MockRepository.GenerateMock<IClient>();
         _clientConfiguration.Add("test", client1);
         client1.Expect(x => x.Abort());
         var client2 = MockRepository.GenerateMock<IClient>();
         _clientConfiguration.Add("test2", client2);
         client2.Expect(x => x.Abort());
         // Act
         _clientConfiguration.Clear();
         // Assert
         client1.VerifyAllExpectations();
         client2.VerifyAllExpectations();
      }
   }
}
