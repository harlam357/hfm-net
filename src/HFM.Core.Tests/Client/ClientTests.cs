using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

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
            var client = new TestClient();
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
            var client = new TestClient { Settings = oldSettings };
            // Act
            client.Settings = new ClientSettings { Name = "Foo" };
            // Assert
            Assert.AreSame(oldSettings, client.OldSettings);
            Assert.AreSame(client.Settings, client.NewSettings);
        }

        [Test]
        public void Client_Slots_ContainsOneOfflineSlot()
        {
            var client = new TestClient();
            Assert.AreEqual(1, client.Slots.Count());
            Assert.AreEqual(SlotStatus.Offline, client.Slots.First().Status);
        }

        [Test]
        public void Client_RefreshSlots_RaisesSlotsChangedEvent()
        {
            // Arrange
            var client = new TestClient();
            bool raised = false;
            client.SlotsChanged += (s, e) => raised = true;
            // Act
            client.RefreshSlots();
            // Assert
            Assert.IsTrue(raised);
        }

        [Test]
        public void Client_Retrieve_ClearsIsCancellationRequested()
        {
            // Arrange
            var client = new TestClient();
            client.Cancel();
            Assert.IsTrue(client.IsCancellationRequested);
            // Act
            client.Retrieve();
            // Assert
            Assert.IsFalse(client.IsCancellationRequested);
        }

        [Test]
        public void Client_Retrieve_CancelDuringRetrieve()
        {
            // Arrange
            var client = new TestClientCancelsOnRetrieve();
            Assert.IsFalse(client.IsCancellationRequested);
            // Act
            client.Retrieve();
            // Assert
            Assert.IsTrue(client.IsCancellationRequested);
        }

        [Test]
        public void Client_Retrieve_SetsLastRetrieveTime()
        {
            // Arrange
            var client = new TestClient();
            var expected = client.LastRetrieveTime;
            // Act
            client.Retrieve();
            // Assert
            Assert.AreNotEqual(expected, client.LastRetrieveTime);
        }

        [Test]
        public void Client_Retrieve_RaisesRetrieveFinishedEvent()
        {
            // Arrange
            var client = new TestClient();
            bool raised = false;
            client.RetrieveFinished += (s, e) => raised = true;
            // Act
            client.Retrieve();
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
                .Select(_ => Task.Run(() => client.Retrieve()))
                .ToList();
            Task.WaitAll(tasks.ToArray());
            // Assert
            Console.WriteLine($"Retrieve In Progress Count: {client.RetrieveInProgressCount}");
            Console.WriteLine($"Retrieve Count: {client.RetrieveCount}");
            Assert.IsTrue(client.RetrieveInProgressCount > 0);
            Assert.IsTrue(client.RetrieveCount > 0);
            Assert.AreEqual(count, client.RetrieveInProgressCount + client.RetrieveCount);
        }

        private class TestClient : Client
        {
            public TestClient() : base(null, null, null)
            {

            }

            public ClientSettings OldSettings { get; private set; }
            public ClientSettings NewSettings { get; private set; }

            protected override void OnSettingsChanged(ClientSettings oldSettings, ClientSettings newSettings)
            {
                OldSettings = oldSettings;
                NewSettings = newSettings;
            }

            protected override void OnRetrieve()
            {

            }
        }

        private class TestClientCancelsOnRetrieve : Client
        {
            public TestClientCancelsOnRetrieve() : base(null, null, null)
            {

            }

            protected override void OnRetrieve() => Cancel();
        }

        private class TestClientRetrieveOnMultipleThreads : Client
        {
            public TestClientRetrieveOnMultipleThreads() : base(null, null, null)
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

            public int RetrieveCount => _retrieveCount;

            protected override void OnRetrieve()
            {
                Thread.Sleep(10);
                Interlocked.Increment(ref _retrieveCount);
            }
        }
    }
}
