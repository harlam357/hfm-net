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

using HFM.Core.DataTypes;

namespace HFM.Core
{
    [TestFixture]
    public class ClientConfigurationTests
    {
        private ClientFactory _factory;
        private ClientConfiguration _clientConfiguration;

        [SetUp]
        public void TestSetUp()
        {
            _factory = new ClientFactory();
            _clientConfiguration = new ClientConfiguration(_factory);
        }

        [Test]
        public void ClientDictionary_ArgumentNullException_Test()
        {
            Assert.Throws(typeof(ArgumentNullException), () => new ClientConfiguration(null));
        }

        [Test]
        public void ClientConfiguration_Load_CreatesAndAddsClientsToTheConfiguration()
        {
            // Arrange
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            // Act
            _clientConfiguration.Load(settings);
            // Assert
            Assert.AreEqual(1, _clientConfiguration.Count);
        }

        [Test]
        public void ClientConfiguration_Load_SetsIsDirtyPropertyToFalse()
        {
            // Arrange
            _clientConfiguration.Add("test", new FahClient());
            Assert.IsTrue(_clientConfiguration.IsDirty);
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            // Act
            _clientConfiguration.Load(settings);
            // Assert
            Assert.AreEqual(1, _clientConfiguration.Count);
            Assert.IsFalse(_clientConfiguration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Add_SetsIsDirtyPropertyToTrue()
        {
            // Act
            _clientConfiguration.Add("test", new FahClient());
            // Assert
            Assert.IsTrue(_clientConfiguration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Remove_SetsIsDirtyPropertyToTrue()
        {
            // Arrange
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            _clientConfiguration.Load(settings);
            // Act
            bool result = _clientConfiguration.Remove("test");
            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(_clientConfiguration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Load_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            ConfigurationChangedEventArgs eventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            _clientConfiguration.Load(settings);
            // Assert
            Assert.AreEqual(ConfigurationChangedType.Add, eventArgs.ChangedType);
            Assert.IsNull(eventArgs.Client);
        }

        // TODO: Load() method - test client SlotsChanged and RetrievalFinished subscriptions

        [Test]
        public void ClientConfiguration_Load_ThrowsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Load(null));
        }

        [Test]
        public void ClientConfiguration_Add_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var settings = new ClientSettings { Name = "test", Server = "foo" };
            ConfigurationChangedEventArgs eventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            _clientConfiguration.Add(settings);
            // Assert
            Assert.AreEqual(ConfigurationChangedType.Add, eventArgs.ChangedType);
            Assert.IsNotNull(eventArgs.Client);
        }
        
        [Test]
        public void ClientConfiguration_Add_SubscribesToClientEvents()
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
        public void ClientConfiguration_ClientSlotsChangedRaisesConfigurationChanged()
        {
            // Arrange
            var client = MockRepository.GenerateMock<IClient>();
            bool clientInvalidateFired = false;
            _clientConfiguration.ConfigurationChanged += (sender, args) =>
            {
                if (args.ChangedType == ConfigurationChangedType.Invalidate) clientInvalidateFired = true;
            };
            _clientConfiguration.Add("test", client);
            // Act
            client.Raise(x => x.SlotsChanged += null, this, EventArgs.Empty);
            // Assert
            Assert.IsTrue(clientInvalidateFired);
        }

        [Test]
        public void ClientConfiguration_ClientRetrievalFinishedRaisesConfigurationChanged()
        {
            // Arrange
            var client = MockRepository.GenerateMock<IClient>();
            bool clientDataInvalidatedFired = false;
            _clientConfiguration.ConfigurationChanged += (sender, args) =>
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
        public void ClientConfiguration_Add_ThrowsWhenKeyIsNullAndClientIsNotNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Add(null, new FahClient()));
        }

        [Test]
        public void ClientConfiguration_Add_ThrowsWhenKeyIsNotNullAndClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Add("test", null));
        }

        [Test]
        public void ClientConfiguration_Add_ThrowsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Add(null));
        }

        [Test]
        public void ClientConfiguration_Edit_RaisesEvents()
        {
            // Arrange
            _clientConfiguration.Add("test", new FahClient { Settings = new ClientSettings { Name = "test", Server = "server", Port = ClientSettings.DefaultPort } });
            ConfigurationChangedEventArgs changedEventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { changedEventArgs = e; };
            ClientEditedEventArgs editedEventArgs = null;
            _clientConfiguration.ClientEdited += (sender, e) => editedEventArgs = e;
            // Act
            _clientConfiguration.Edit("test", new ClientSettings { Name = "test2", Server = "server1", Port = 36331 });
            // Assert
            Assert.AreEqual(1, _clientConfiguration.Count);
            Assert.IsTrue(_clientConfiguration.ContainsKey("test2"));
            Assert.AreEqual(ConfigurationChangedType.Edit, changedEventArgs.ChangedType);
            Assert.AreEqual("test2", changedEventArgs.Client.Settings.Name);
            Assert.AreEqual("server1-36331", changedEventArgs.Client.Settings.DataPath());
            Assert.AreEqual("test", editedEventArgs.PreviousName);
            Assert.AreEqual("server-36330", editedEventArgs.PreviousPath);
            Assert.AreEqual("test2", editedEventArgs.NewName);
            Assert.AreEqual("server1-36331", editedEventArgs.NewPath);
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenClientNameAlreadyExistsInConfiguration()
        {
            // Arrange
            _clientConfiguration.Add("test", new FahClient { Settings = new ClientSettings { Name = "test", Server = "foo" } });
            _clientConfiguration.Add("other", new FahClient { Settings = new ClientSettings { Name = "other", Server = "bar" } });
            Assert.AreEqual(2, _clientConfiguration.Count);
            // Act & Assert
            Assert.Throws(typeof(ArgumentException), () => _clientConfiguration.Edit("test", new ClientSettings { Name = "other", Server = "bar" }));
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenKeyIsNullAndClientSettingsIsNotNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Edit(null, new ClientSettings()));
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenKeyIsNotNullAndClientSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Edit("test", null));
        }

        [Test]
        public void ClientConfiguration_Remove_ReturnsFalseWhenKeyDoesNotExist()
        {
            Assert.IsFalse(_clientConfiguration.Remove("test"));
            Assert.IsFalse(_clientConfiguration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Remove_RaisesConfigurationChangedEvent()
        {
            // Arrange
            ConfigurationChangedEventArgs eventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { eventArgs = e; };
            _clientConfiguration.Add("test", new FahClient());
            // Act
            bool result = _clientConfiguration.Remove("test");
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ConfigurationChangedType.Remove, eventArgs.ChangedType);
            Assert.IsNotNull(eventArgs.Client);
        }

        [Test]
        public void ClientConfiguration_Remove_CallsClientAbortAndFactoryRelease()
        {
            // Arrange
            var client = MockRepository.GeneratePartialMock<FahClient>();
            var fahClientFactory = MockRepository.GenerateMock<IFahClientFactory>();
            var configuration = new ClientConfiguration(new ClientFactory { FahClientFactory = fahClientFactory });
            configuration.Add("test", client);
            client.Expect(x => x.Abort());
            fahClientFactory.Expect(x => x.Release(client));
            // Act
            configuration.Remove("test");
            // Assert
            client.VerifyAllExpectations();
            fahClientFactory.VerifyAllExpectations();
        }

        [Test]
        public void ClientConfiguration_Remove_ThrowsWhenKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => _clientConfiguration.Remove(null));
        }

        [Test]
        public void ClientConfiguration_Clear_SetsIsDirtyPropertyToFalse()
        {
            // Arrange
            _clientConfiguration.Add("test", new FahClient());
            // Act
            _clientConfiguration.Clear();
            // Assert
            Assert.IsFalse(_clientConfiguration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Clear_DoesNotRaiseConfigurationChangedWhenConfigurationIsEmpty()
        {
            // Arrange
            ConfigurationChangedEventArgs eventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            _clientConfiguration.Clear();
            // Assert
            Assert.IsNull(eventArgs);
        }

        [Test]
        public void ClientConfiguration_Clear_RaisesConfigurationChangedEvent()
        {
            // Arrange
            _clientConfiguration.Add("test", new FahClient());
            ConfigurationChangedEventArgs eventArgs = null;
            _clientConfiguration.ConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            _clientConfiguration.Clear();
            // Assert
            Assert.AreEqual(ConfigurationChangedType.Clear, eventArgs.ChangedType);
            Assert.IsNull(eventArgs.Client);
        }

        [Test]
        public void ClientConfiguration_Clear_CallsClientAbortAndFactoryRelease()
        {
            // Arrange
            var client = MockRepository.GeneratePartialMock<FahClient>();
            var fahClientFactory = MockRepository.GenerateMock<IFahClientFactory>();
            var configuration = new ClientConfiguration(new ClientFactory { FahClientFactory = fahClientFactory });
            configuration.Add("test", client);
            client.Expect(x => x.Abort());
            fahClientFactory.Expect(x => x.Release(client));
            // Act
            configuration.Clear();
            // Assert
            client.VerifyAllExpectations();
            fahClientFactory.VerifyAllExpectations();
        }
    }
}
