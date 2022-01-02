using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            using (var client = new TestClientSettingsChanged())
            {
                // Act
                client.Settings = new ClientSettings { Name = "Foo" };
                // Assert
                Assert.IsNull(client.OldSettings);
                Assert.AreSame(client.Settings, client.NewSettings);
            }
        }

        [Test]
        public void Client_Settings_CallsOnSettingsChanged_OldIsSameAndCurrentIsSame()
        {
            // Arrange
            var oldSettings = new ClientSettings { Name = "Bar" };
            using (var client = new TestClientSettingsChanged())
            {
                client.Settings = oldSettings;
                // Act
                client.Settings = new ClientSettings { Name = "Foo" };
                // Assert
                Assert.AreSame(oldSettings, client.OldSettings);
                Assert.AreSame(client.Settings, client.NewSettings);
            }
        }

        [Test]
        public void Client_Settings_CallsOnSettingsChanged_OldIsSameAndCurrentIsNull()
        {
            // Arrange
            var oldSettings = new ClientSettings { Name = "Bar" };
            using (var client = new TestClientSettingsChanged())
            {
                client.Settings = oldSettings;
                // Act
                client.Settings = null;
                // Assert
                Assert.AreSame(oldSettings, client.OldSettings);
                Assert.IsNull(client.NewSettings);
            }
        }

        [Test]
        public void Client_Slots_ContainsOneOfflineSlot()
        {
            using (var client = new TestClient())
            {
                Assert.AreEqual(1, client.Slots.Count());
                Assert.AreEqual(SlotStatus.Offline, client.Slots.First().Status);
            }
        }

        [Test]
        public void Client_RefreshSlots_RaisesSlotsChangedEvent()
        {
            // Arrange
            using (var client = new TestClient())
            {
                bool raised = false;
                client.SlotsChanged += (s, e) => raised = true;
                // Act
                client.RefreshSlots();
                // Assert
                Assert.IsTrue(raised);
            }
        }

        [Test]
        public async Task Client_Connect_ConnectsTheClient()
        {
            // Arrange
            using (var client = new TestClient())
            {
                // Act
                await client.Connect();
                // Assert
                Assert.IsTrue(client.Connected);
            }
        }

        [Test]
        public async Task Client_Connect_DoesNotConnectTheClientIfTheClientIsDisabled()
        {
            // Arrange
            using (var client = new TestClient())
            {
                client.Settings = new ClientSettings { Disabled = true };
                // Act
                await client.Connect();
                // Assert
                Assert.IsFalse(client.Connected);
            }
        }

        [Test]
        public void Client_Connect_Throws()
        {
            // Arrange
            using (var client = new TestClientConnectThrows())
            {
                // Act & Assert
                Assert.ThrowsAsync<Exception>(() => client.Connect());
            }
        }

        [Test]
        public async Task Client_Retrieve_ClearsIsCancellationRequested()
        {
            // Arrange
            using (var client = new TestClient())
            {
                client.Cancel();
                Assert.IsTrue(client.IsCancellationRequested);
                // Act
                await client.Retrieve();
                // Assert
                Assert.IsFalse(client.IsCancellationRequested);
            }
        }

        [Test]
        public async Task Client_Retrieve_CancelIsCalledDuringRetrieve()
        {
            // Arrange
            using (var client = new TestClientCancelsOnRetrieve())
            {
                Assert.IsFalse(client.IsCancellationRequested);
                // Act
                await client.Retrieve();
                // Assert
                Assert.IsTrue(client.IsCancellationRequested);
            }
        }

        [Test]
        public async Task Client_Retrieve_SetsLastRetrieveTime()
        {
            // Arrange
            using (var client = new TestClient())
            {
                var expected = client.LastRetrieveTime;
                // Act
                await client.Retrieve();
                // Assert
                Assert.AreNotEqual(expected, client.LastRetrieveTime);
            }
        }

        [Test]
        public async Task Client_Retrieve_ConnectsIfNotConnected()
        {
            // Arrange
            using (var client = new TestClient())
            {
                // Act
                await client.Retrieve();
                // Assert
                Assert.IsTrue(client.Connected);
            }
        }

        [Test]
        public async Task Client_Retrieve_DoesNotConnectTheClientIfTheClientIsDisabled()
        {
            // Arrange
            using (var client = new TestClient())
            {
                client.Settings = new ClientSettings { Disabled = true };
                // Act
                await client.Retrieve();
                // Assert
                Assert.IsFalse(client.Connected);
            }
        }

        [Test]
        public async Task Client_Retrieve_ConnectThrowsAndLogsException()
        {
            // Arrange
            using (var client = new TestClientConnectThrows())
            {
                // Act
                await client.Retrieve();
                // Assert
                var mockLogger = Mock.Get(client.Logger);
                mockLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));
            }
        }

        [Test]
        public async Task Client_Retrieve_ThrowsAndLogsException()
        {
            // Arrange
            using (var client = new TestClientRetrieveThrows())
            {
                // Act
                await client.Retrieve();
                // Assert
                var mockLogger = Mock.Get(client.Logger);
                mockLogger.Verify(x => x.Error(It.IsAny<string>(), It.IsAny<Exception>()));
            }
        }

        [Test]
        public async Task Client_Retrieve_OnMultipleRetrieveCallsConnectIsCalledOnce()
        {
            // Arrange
            using (var client = new TestClient())
            {
                const int count = 10;
                // Act
                for (int i = 0; i < count; i++)
                {
                    await client.Retrieve();
                }
                // Assert
                Console.WriteLine($"Retrieve Count: {client.RetrieveCount}");
                Assert.IsTrue(client.Connected);
                Assert.AreEqual(count - 1, client.RetrieveCount);
            }
        }

        [Test]
        public async Task Client_Retrieve_RaisesRetrieveFinishedEvent()
        {
            // Arrange
            using (var client = new TestClient())
            {
                bool raised = false;
                client.RetrieveFinished += (s, e) => raised = true;
                // Act
                await client.Retrieve();
                // Assert
                Assert.IsTrue(raised);
            }
        }

        [Test]
        public void Client_Retrieve_OnMultipleThreadsThrowsAwayConcurrentCallsToRetrieve()
        {
            // Arrange
            using (var client = new TestClientRetrieveOnMultipleThreads())
            {
                const int count = 100;
                // Act
                var tasks = Enumerable.Range(0, count)
                    // ReSharper disable once AccessToDisposedClosure
                    .Select(_ => Task.Run(() => client.Retrieve()))
                    .ToArray();
                Task.WaitAll(tasks);
                // Assert
                Console.WriteLine($"Retrieve In Progress Count: {client.RetrieveInProgressCount}");
                Console.WriteLine($"Retrieve Count: {client.RetrieveCount}");
                Assert.IsTrue(client.RetrieveInProgressCount > 0);
                Assert.IsTrue(client.RetrieveCount > 0);
                Assert.AreEqual(count, client.RetrieveInProgressCount + client.RetrieveCount);
            }
        }

        [Test]
        public void Client_Slots_IsThreadSafe()
        {
            // Arrange
            using (var client = new TestClientRefreshesSlots())
            {
                Task.Run(() =>
                {
                    while (true)
                    {
                        // ReSharper disable once AccessToDisposedClosure
                        client.RefreshSlots();
                    }
                });

                const int count = 10;

                var tasks = Enumerable.Range(0, count)
                    .Select(_ => Task.Run(() =>
                    {
                        Thread.Sleep(10);
                        // ReSharper disable once AccessToDisposedClosure
                        foreach (var x in client.Slots)
                        {
                            // enumeration of client slots
                        }
                    }))
                    .ToArray();

                try
                {
                    Task.WaitAll(tasks);
                }
                catch (Exception)
                {
                    Assert.Fail("Enumeration failed");
                }
            }
        }

        private class TestClient : Client
        {
            public TestClient() : base(Mock.Of<ILogger>(), null, null)
            {

            }

            protected TestClient(bool connected) : this()
            {
                Connected = connected;
            }

            protected override Task OnConnect()
            {
                Connected = true;
                return Task.CompletedTask;
            }

            public int RetrieveCount { get; private set; }

            protected override Task OnRetrieve()
            {
                RetrieveCount++;
                return Task.CompletedTask;
            }
        }

        private class TestClientSettingsChanged : TestClient
        {
            public ClientSettings OldSettings { get; private set; }
            public ClientSettings NewSettings { get; private set; }

            protected override void OnSettingsChanged(ClientSettings oldSettings, ClientSettings newSettings)
            {
                OldSettings = oldSettings;
                NewSettings = newSettings;
            }
        }

        private class TestClientConnectThrows : TestClient
        {
            protected override Task OnConnect() => throw new Exception(nameof(OnConnect));
        }

        private class TestClientRetrieveThrows : TestClient
        {
            public TestClientRetrieveThrows() : base(true)
            {

            }

            protected override Task OnRetrieve() => throw new Exception(nameof(OnRetrieve));
        }

        private class TestClientCancelsOnRetrieve : TestClient
        {
            public TestClientCancelsOnRetrieve() : base(true)
            {

            }

            protected override Task OnRetrieve()
            {
                Cancel();
                return Task.CompletedTask;
            }
        }

        private class TestClientRetrieveOnMultipleThreads : TestClient
        {
            public TestClientRetrieveOnMultipleThreads() : base(true)
            {

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

        private class TestClientRefreshesSlots : TestClient
        {
            public TestClientRefreshesSlots() : base(true)
            {

            }

            private static readonly Random _Random = new();

            protected override void OnRefreshSlots(ICollection<SlotModel> slots)
            {
                slots.Clear();

                int count = _Random.Next(1, 5);
                for (int i = 0; i < count; i++)
                {
                    slots.Add(new SlotModel(this));
                }
            }
        }
    }
}
