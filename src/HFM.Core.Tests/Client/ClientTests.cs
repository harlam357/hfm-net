using HFM.Core.Client.Mocks;
using HFM.Core.Logging;

using Moq;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class ClientTests
    {
        [Test]
        public void Client_Settings_CallsOnSettingsChanged_OldIsNullAndCurrentIsSame()
        {
            // Arrange
            var client = new TestClientSettingsChanged();
            // Act
            client.Settings = new ClientSettings { Name = "Foo" };
            // Assert
            Assert.IsNull(client.OldSettings);
            Assert.AreSame(client.Settings, client.NewSettings);
        }

        [Test]
        public void Client_Settings_CallsOnSettingsChanged_OldIsSameAndCurrentIsSame()
        {
            // Arrange
            var oldSettings = new ClientSettings { Name = "Bar" };
            var client = new TestClientSettingsChanged();
            client.Settings = oldSettings;
            // Act
            client.Settings = new ClientSettings { Name = "Foo" };
            // Assert
            Assert.AreSame(oldSettings, client.OldSettings);
            Assert.AreSame(client.Settings, client.NewSettings);
        }

        [Test]
        public void Client_Settings_CallsOnSettingsChanged_OldIsSameAndCurrentIsNull()
        {
            // Arrange
            var oldSettings = new ClientSettings { Name = "Bar" };
            var client = new TestClientSettingsChanged();
            client.Settings = oldSettings;
            // Act
            client.Settings = null;
            // Assert
            Assert.AreSame(oldSettings, client.OldSettings);
            Assert.IsNull(client.NewSettings);
        }

        [Test]
        public void Client_ClientDataCollection_ContainsOneOfflineSlot()
        {
            var client = new MockClient();
            Assert.AreEqual(1, client.ClientDataCollection.Count);
            Assert.AreEqual(SlotStatus.Offline, client.ClientDataCollection.First().Status);
        }

        [Test]
        public async Task Client_Connect_ConnectsTheClient()
        {
            // Arrange
            var client = new MockClient();
            // Act
            await client.Connect();
            // Assert
            Assert.IsTrue(client.Connected);
        }

        [Test]
        public async Task Client_Connect_DoesNotConnectTheClientIfTheClientIsDisabled()
        {
            // Arrange
            var client = new MockClient();
            client.Settings = new ClientSettings { Disabled = true };
            // Act
            await client.Connect();
            // Assert
            Assert.IsFalse(client.Connected);
        }

        [Test]
        public void Client_Connect_Throws()
        {
            // Arrange
            var client = new TestClientConnectThrows();
            // Act & Assert
            Assert.ThrowsAsync<Exception>(() => client.Connect());
        }

        [Test]
        public async Task Client_Retrieve_ClearsIsCancellationRequested()
        {
            // Arrange
            var client = new MockClient();
            client.Close();
            Assert.IsTrue(client.IsCancellationRequested);
            // Act
            await client.Retrieve();
            // Assert
            Assert.IsFalse(client.IsCancellationRequested);
        }

        [Test]
        public async Task Client_Retrieve_CloseIsCalledDuringRetrieve()
        {
            // Arrange
            var client = new TestClientClosesOnRetrieve();
            Assert.IsFalse(client.IsCancellationRequested);
            // Act
            await client.Retrieve();
            // Assert
            Assert.IsTrue(client.IsCancellationRequested);
        }

        [Test]
        public async Task Client_Retrieve_SetsLastRetrieveTime()
        {
            // Arrange
            var client = new MockClient();
            var expected = client.LastRetrieveTime;
            // Act
            await client.Retrieve();
            // Assert
            Assert.AreNotEqual(expected, client.LastRetrieveTime);
        }

        [Test]
        public async Task Client_Retrieve_ConnectsIfNotConnected()
        {
            // Arrange
            var client = new MockClient();
            // Act
            await client.Retrieve();
            // Assert
            Assert.IsTrue(client.Connected);
        }

        [Test]
        public async Task Client_Retrieve_DoesNotConnectTheClientIfTheClientIsDisabled()
        {
            // Arrange
            var client = new MockClient();
            client.Settings = new ClientSettings { Disabled = true };
            // Act
            await client.Retrieve();
            // Assert
            Assert.IsFalse(client.Connected);
        }

        [Test]
        public async Task Client_Retrieve_ConnectThrowsAndLogsException()
        {
            // Arrange
            var client = new TestClientConnectThrows();
            // Act
            await client.Retrieve();
            // Assert
            var mockLogger = Mock.Get(client.Logger);
            mockLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));
        }

        [Test]
        public async Task Client_Retrieve_ThrowsAndLogsException()
        {
            // Arrange
            var client = new TestClientRetrieveThrows();
            // Act
            await client.Retrieve();
            // Assert
            var mockLogger = Mock.Get(client.Logger);
            mockLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));
        }

        [Test]
        public async Task Client_Retrieve_OnMultipleRetrieveCallsConnectIsCalledOnce()
        {
            // Arrange
            var client = new MockClient();
            const int count = 10;
            // Act
            for (int i = 0; i < count; i++)
            {
                await client.Retrieve();
            }
            // Assert
            TestLogger.Instance.Debug($"Retrieve Count: {client.RetrieveCount}");
            Assert.IsTrue(client.Connected);
            Assert.AreEqual(count - 1, client.RetrieveCount);
        }

        [Test]
        public async Task Client_Retrieve_RaisesClientDataChangedEvent()
        {
            // Arrange
            var client = new MockClient();
            bool raised = false;
            client.ClientDataChanged += (s, e) => raised = true;
            // Act
            await client.Retrieve();
            // Assert
            Assert.IsTrue(raised);
        }

        [Test]
        public void Client_Retrieve_OnMultipleThreadsThrowsAwayConcurrentCallsToRetrieve()
        {
            // Arrange
            var client = new TestClientRetrieveOnMultipleThreads();
            const int count = 100;
            // Act
            var tasks = Enumerable.Range(0, count)
                // ReSharper disable once AccessToDisposedClosure
                .Select(_ => Task.Run(() => client.Retrieve()))
                .ToArray();
            Task.WaitAll(tasks);
            // Assert
            TestLogger.Instance.Debug($"Retrieve In Progress Count: {client.RetrieveInProgressCount}");
            TestLogger.Instance.Debug($"Retrieve Count: {client.RetrieveCount}");
            Assert.IsTrue(client.RetrieveInProgressCount > 0);
            Assert.IsTrue(client.RetrieveCount > 0);
            Assert.AreEqual(count, client.RetrieveInProgressCount + client.RetrieveCount);
        }

        private class TestClientSettingsChanged : MockClient
        {
            public ClientSettings OldSettings { get; private set; }
            public ClientSettings NewSettings { get; private set; }

            protected override void OnSettingsChanged(ClientSettings oldSettings, ClientSettings newSettings)
            {
                OldSettings = oldSettings;
                NewSettings = newSettings;
            }
        }

        private class TestClientConnectThrows : MockClient
        {
            public TestClientConnectThrows() : base(Mock.Of<ILogger>())
            {

            }

            protected override Task OnConnect() => throw new Exception(nameof(OnConnect));
        }

        private class TestClientRetrieveThrows : MockClient
        {
            public TestClientRetrieveThrows() : base(Mock.Of<ILogger>())
            {
                Connected = true;
            }

            protected override Task OnRetrieve() => throw new Exception(nameof(OnRetrieve));
        }

        private class TestClientClosesOnRetrieve : MockClient
        {
            public TestClientClosesOnRetrieve()
            {
                Connected = true;
            }

            protected override Task OnRetrieve()
            {
                Close();
                return Task.CompletedTask;
            }
        }

        private class TestClientRetrieveOnMultipleThreads : MockClient
        {
            public TestClientRetrieveOnMultipleThreads()
            {
                Connected = true;
            }

            private int _retrieveInProgressCount;

            public int RetrieveInProgressCount => _retrieveInProgressCount;

            protected override void OnRetrieveInProgress()
            {
                Thread.Sleep(100);
                Interlocked.Increment(ref _retrieveInProgressCount);
            }

            private int _retrieveCount;

            public new int RetrieveCount => _retrieveCount;

            protected override async Task OnRetrieve()
            {
                await Task.Delay(10);
                Interlocked.Increment(ref _retrieveCount);
            }
        }
    }
}
