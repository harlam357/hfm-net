using HFM.Core.Client.Mocks;
using HFM.Core.Logging;
using HFM.Preferences;

using Moq;

using NUnit.Framework;

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
            var mockClient = new Mock<IClient>();
            mockClient.SetupAdd(x => x.ClientDataCollectionChanged += It.IsAny<EventHandler>());
            mockClient.SetupAdd(x => x.RetrieveFinished += It.IsAny<EventHandler>());
            // Act
            configuration.Add("test", mockClient.Object);
            // Assert
            mockClient.VerifyAdd(x => x.ClientDataCollectionChanged += It.IsAny<EventHandler>());
            mockClient.VerifyAdd(x => x.RetrieveFinished += It.IsAny<EventHandler>());
        }

        [Test]
        public void ClientConfiguration_ClientDataCollectionChangedRaisesConfigurationChanged()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var mockClient = new Mock<IClient>();
            bool clientInvalidateFired = false;
            configuration.ClientConfigurationChanged += (sender, args) =>
            {
                if (args.Action == ClientConfigurationChangedAction.Invalidate) clientInvalidateFired = true;
            };
            configuration.Add("test", mockClient.Object);
            // Act
            mockClient.Raise(x => x.ClientDataCollectionChanged += null, this, EventArgs.Empty);
            // Assert
            Assert.IsTrue(clientInvalidateFired);
        }

        [Test]
        public void ClientConfiguration_ClientRetrieveFinishedRaisesConfigurationChanged()
        {
            // Arrange
            var configuration = CreateConfiguration();
            var mockClient = new Mock<IClient>();
            bool clientDataInvalidatedFired = false;
            configuration.ClientConfigurationChanged += (sender, args) =>
            {
                if (args.Action == ClientConfigurationChangedAction.Invalidate) clientDataInvalidatedFired = true;
            };
            configuration.Add("test", mockClient.Object);
            // Act
            mockClient.Raise(x => x.RetrieveFinished += null, this, EventArgs.Empty);
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
            // Act
            configuration.Edit("test", new ClientSettings { Name = "test2", Server = "server1", Port = 36331 });
            // Assert
            Assert.AreEqual(1, configuration.Count);
            Assert.AreEqual("test2", configuration.GetClients().First().Settings.Name);
            Assert.AreEqual(ClientConfigurationChangedAction.Edit, changedEventArgs.Action);
            Assert.AreEqual("test2", changedEventArgs.Client.Settings.Name);
            Assert.AreEqual("server1", changedEventArgs.Client.Settings.Server);
            Assert.AreEqual(36331, changedEventArgs.Client.Settings.Port);
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
        public void ClientConfiguration_Remove_CallsClientClose()
        {
            // Arrange
            var mockClient = new Mock<IFahClient>();
            var configuration = CreateConfiguration();
            configuration.Add("test", mockClient.Object);
            // Act
            configuration.Remove("test");
            // Assert
            mockClient.Verify(x => x.Close());
        }

        [Test]
        public void ClientConfiguration_Remove_CallsClientDispose()
        {
            // Arrange
            var mockClient = new Mock<IClientDisposable>();
            var configuration = CreateConfiguration();
            configuration.Add("test", mockClient.Object);
            // Act
            configuration.Remove("test");
            // Assert
            mockClient.Verify(x => x.Dispose());
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
        public void ClientConfiguration_Clear_CallsClientClose()
        {
            // Arrange
            var mockClient = new Mock<IFahClient>();
            var configuration = CreateConfiguration();
            configuration.Add("test", mockClient.Object);
            // Act
            configuration.Clear();
            // Assert
            mockClient.Verify(x => x.Close());
        }

        [Test]
        public void ClientConfiguration_Clear_CallsClientDispose()
        {
            // Arrange
            var mockClient = new Mock<IClientDisposable>();
            var configuration = CreateConfiguration();
            configuration.Add("test", mockClient.Object);
            // Act
            configuration.Clear();
            // Assert
            mockClient.Verify(x => x.Dispose());
        }

        [Test]
        public async Task ClientConfiguration_GetClientDataCollection_IsThreadSafe()
        {
            // Arrange
            var test1 = new MockFahClientRefreshesSlots();
            await test1.Connect();
            var test2 = new MockFahClientRefreshesSlots();
            await test2.Connect();
            var test3 = new MockFahClientRefreshesSlots();
            await test3.Connect();

            var configuration = CreateConfiguration();
            configuration.Add("test1", test1);
            configuration.Add("test2", test2);
            configuration.Add("test3", test3);
            var clients = configuration.GetClients();

            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            var token = cts.Token;

            _ = Task.Run(() =>
            {
                while (true)
                {
                    token.ThrowIfCancellationRequested();

                    Parallel.ForEach(clients, x =>
                    {
                        var client = (FahClient)x;
                        client.RefreshSlots();
                    });
                }
            }, token);

            const int count = 2000;

            var tasks = Enumerable.Range(0, count)
                .Select(_ => Task.Run(() =>
                {
                    token.ThrowIfCancellationRequested();

                    Thread.Sleep(10);
                    foreach (var x in configuration.GetClientDataCollection())
                    {
                        // enumeration of client data
                    }
                }, token))
                .ToArray();

            try
            {
                Task.WaitAll(tasks);
            }
            catch (AggregateException aggEx)
            {
                if (!aggEx.InnerExceptions.Any(x => x is OperationCanceledException))
                {
                    Assert.Fail("Enumeration failed");
                }
            }
            catch (Exception)
            {
                Assert.Fail("Enumeration failed");
            }
            finally
            {
                cts.Cancel();
            }
        }

        private static ClientConfiguration CreateConfiguration() =>
            new(null,
                new InMemoryPreferencesProvider(),
                new ClientFactory(),
                (l, p, c) => new ClientScheduledTasksWithoutEvents(l, p, c));

        // a ClientScheduledTasks that does not respond to preference or configuration changed events
        private class ClientScheduledTasksWithoutEvents : ClientScheduledTasks
        {
            public ClientScheduledTasksWithoutEvents(ILogger logger, IPreferences preferences, ClientConfiguration clientConfiguration)
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

        private class MockFahClientRefreshesSlots : MockFahClient
        {
            private static readonly Random _Random = new();

            protected override void OnRefreshSlots(ICollection<IClientData> collection)
            {
                collection.Clear();

                int count = _Random.Next(1, 5);
                for (int i = 0; i < count; i++)
                {
                    collection.Add(new ClientData());
                }
            }
        }
    }

    public interface IClientDisposable : IClient, IDisposable
    {

    }
}
