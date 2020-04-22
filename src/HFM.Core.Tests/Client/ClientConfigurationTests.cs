
using System;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientConfigurationTests
    {
        [Test]
        public void ClientConfiguration_Load_CreatesAndAddsClientsToTheConfiguration()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            // Act
            configuration.Load(settings);
            // Assert
            Assert.AreEqual(1, configuration.Count);
        }

        [Test]
        public void ClientConfiguration_Load_SetsIsDirtyPropertyToFalse()
        {
            // Arrange
            var configuration = CreateConfiguration();
            configuration.Add("test", new NullClient());
            Assert.IsTrue(configuration.IsDirty);
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            // Act
            configuration.Load(settings);
            // Assert
            Assert.AreEqual(1, configuration.Count);
            Assert.IsFalse(configuration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Add_SetsIsDirtyPropertyToTrue()
        {
            // Arrange
            var configuration = CreateConfiguration();
            // Act
            configuration.Add("test", new NullClient());
            // Assert
            Assert.IsTrue(configuration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Remove_SetsIsDirtyPropertyToTrue()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            configuration.Load(settings);
            // Act
            bool result = configuration.Remove("test");
            // Assert
            Assert.IsTrue(result);
            Assert.IsTrue(configuration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Load_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var settings = new[] { new ClientSettings { Name = "test", Server = "foo" } };
            ClientConfigurationChangedEventArgs eventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            configuration.Load(settings);
            // Assert
            Assert.AreEqual(ClientConfigurationChangedAction.Add, eventArgs.Action);
            Assert.IsNull(eventArgs.Client);
        }

        [Test]
        public void ClientConfiguration_Load_ThrowsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Load(null));
        }

        [Test]
        public void ClientConfiguration_Add_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var settings = new ClientSettings { Name = "test", Server = "foo" };
            ClientConfigurationChangedEventArgs eventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            configuration.Add(settings);
            // Assert
            Assert.AreEqual(ClientConfigurationChangedAction.Add, eventArgs.Action);
            Assert.IsNotNull(eventArgs.Client);
        }
        
        [Test]
        public void ClientConfiguration_Add_SubscribesToClientEvents()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var client = MockRepository.GenerateMock<IClient>();
            // Act
            configuration.Add("test", client);
            // Assert
            client.AssertWasCalled(x => x.SlotsChanged += Arg<EventHandler>.Is.Anything);
            client.AssertWasCalled(x => x.RetrievalFinished += Arg<EventHandler>.Is.Anything);
        }

        [Test]
        public void ClientConfiguration_ClientSlotsChangedRaisesConfigurationChanged()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var client = MockRepository.GenerateMock<IClient>();
            bool clientInvalidateFired = false;
            configuration.ClientConfigurationChanged += (sender, args) =>
            {
                if (args.Action == ClientConfigurationChangedAction.Invalidate) clientInvalidateFired = true;
            };
            configuration.Add("test", client);
            // Act
            client.Raise(x => x.SlotsChanged += null, this, EventArgs.Empty);
            // Assert
            Assert.IsTrue(clientInvalidateFired);
        }

        [Test]
        public void ClientConfiguration_ClientRetrievalFinishedRaisesConfigurationChanged()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var client = MockRepository.GenerateMock<IClient>();
            bool clientDataInvalidatedFired = false;
            configuration.ClientConfigurationChanged += (sender, args) =>
            {
                if (args.Action == ClientConfigurationChangedAction.Invalidate) clientDataInvalidatedFired = true;
            };
            configuration.Add("test", client);
            // Act
            client.Raise(x => x.RetrievalFinished += null, this, EventArgs.Empty);
            // Assert
            Assert.IsTrue(clientDataInvalidatedFired);
        }

        [Test]
        public void ClientConfiguration_Add_ThrowsWhenKeyIsNullAndClientIsNotNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Add(null, new NullClient()));
        }

        [Test]
        public void ClientConfiguration_Add_ThrowsWhenKeyIsNotNullAndClientIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Add("test", null));
        }

        [Test]
        public void ClientConfiguration_Add_ThrowsWhenSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Add(null));
        }

        [Test]
        public void ClientConfiguration_Edit_RaisesEvents()
        {
            // Arrange
            var configuration = CreateConfiguration();
            configuration.Add("test", new NullClient { Settings = new ClientSettings { Name = "test", Server = "server", Port = ClientSettings.DefaultPort } });
            ClientConfigurationChangedEventArgs changedEventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { changedEventArgs = e; };
            ClientEditedEventArgs editedEventArgs = null;
            configuration.ClientEdited += (sender, e) => editedEventArgs = e;
            // Act
            configuration.Edit("test", new ClientSettings { Name = "test2", Server = "server1", Port = 36331 });
            // Assert
            Assert.AreEqual(1, configuration.Count);
            Assert.IsTrue(configuration.ContainsKey("test2"));
            Assert.AreEqual(ClientConfigurationChangedAction.Edit, changedEventArgs.Action);
            Assert.AreEqual("test2", changedEventArgs.Client.Settings.Name);
            Assert.AreEqual("server1-36331", changedEventArgs.Client.Settings.ClientPath);
            Assert.AreEqual("test", editedEventArgs.OldName);
            Assert.AreEqual("server-36330", editedEventArgs.OldPath);
            Assert.AreEqual("test2", editedEventArgs.NewName);
            Assert.AreEqual("server1-36331", editedEventArgs.NewPath);
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenClientNameAlreadyExistsInConfiguration()
        {
            // Arrange
            var configuration = CreateConfiguration();
            configuration.Add("test", new NullClient { Settings = new ClientSettings { Name = "test", Server = "foo" } });
            configuration.Add("other", new NullClient { Settings = new ClientSettings { Name = "other", Server = "bar" } });
            Assert.AreEqual(2, configuration.Count);
            // Act & Assert
            Assert.Throws(typeof(ArgumentException), () => configuration.Edit("test", new ClientSettings { Name = "other", Server = "bar" }));
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenKeyIsNullAndClientSettingsIsNotNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Edit(null, new ClientSettings()));
        }

        [Test]
        public void ClientConfiguration_Edit_ThrowsWhenKeyIsNotNullAndClientSettingsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Edit("test", null));
        }

        [Test]
        public void ClientConfiguration_Remove_ReturnsFalseWhenKeyDoesNotExist()
        {
            var configuration = CreateConfiguration();
            Assert.IsFalse(configuration.Remove("test"));
            Assert.IsFalse(configuration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Remove_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var configuration = CreateConfiguration();
            ClientConfigurationChangedEventArgs eventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { eventArgs = e; };
            configuration.Add("test", new NullClient());
            // Act
            bool result = configuration.Remove("test");
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(ClientConfigurationChangedAction.Remove, eventArgs.Action);
            Assert.IsNotNull(eventArgs.Client);
        }

        [Test]
        public void ClientConfiguration_Remove_CallsClientAbortAndFactoryRelease()
        {
            // Arrange
            var client = MockRepository.GenerateMock<IFahClient>();
            client.Expect(x => x.Abort());
            var configuration = CreateConfiguration();
            configuration.Add("test", client);
            // Act
            configuration.Remove("test");
            // Assert
            client.VerifyAllExpectations();
        }

        [Test]
        public void ClientConfiguration_Remove_ThrowsWhenKeyIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => CreateConfiguration().Remove(null));
        }

        [Test]
        public void ClientConfiguration_Clear_SetsIsDirtyPropertyToFalse()
        {
            // Arrange
            var configuration = CreateConfiguration();
            configuration.Add("test", new NullClient());
            // Act
            configuration.Clear();
            // Assert
            Assert.IsFalse(configuration.IsDirty);
        }

        [Test]
        public void ClientConfiguration_Clear_DoesNotRaiseConfigurationChangedWhenConfigurationIsEmpty()
        {
            // Arrange
            var configuration = CreateConfiguration();
            ClientConfigurationChangedEventArgs eventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            configuration.Clear();
            // Assert
            Assert.IsNull(eventArgs);
        }

        [Test]
        public void ClientConfiguration_Clear_RaisesConfigurationChangedEvent()
        {
            // Arrange
            var configuration = CreateConfiguration();
            configuration.Add("test", new NullClient());
            ClientConfigurationChangedEventArgs eventArgs = null;
            configuration.ClientConfigurationChanged += (sender, e) => { eventArgs = e; };
            // Act
            configuration.Clear();
            // Assert
            Assert.AreEqual(ClientConfigurationChangedAction.Clear, eventArgs.Action);
            Assert.IsNull(eventArgs.Client);
        }

        [Test]
        public void ClientConfiguration_Clear_CallsClientAbortAndFactoryRelease()
        {
            // Arrange
            var client = MockRepository.GenerateMock<IFahClient>();
            client.Expect(x => x.Abort());
            var configuration = CreateConfiguration();
            configuration.Add("test", client);
            // Act
            configuration.Clear();
            // Assert
            client.VerifyAllExpectations();
        }

        private static ClientConfiguration CreateConfiguration()
        {
            return new ClientConfiguration(null,
                new InMemoryPreferenceSet(),
                new ClientFactory(),
                (l, p, c) => new ClientScheduledTasksWithoutEvents(l, p, c));
        }

        // a ClientScheduledTasks that does not respond to preference or configuration changed events
        private class ClientScheduledTasksWithoutEvents : ClientScheduledTasks
        {
            public ClientScheduledTasksWithoutEvents(ILogger logger, IPreferenceSet preferences, ClientConfiguration clientConfiguration)
                : base(logger, preferences, clientConfiguration)
            {
                
            }

            protected override void OnPreferenceChanged(object sender, PreferenceChangedEventArgs e)
            {
                
            }

            protected override void OnClientConfigurationChanged(object sender, ClientConfigurationChangedEventArgs e)
            {
                
            }
        }
    }
}
