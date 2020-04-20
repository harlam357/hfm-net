
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client;
using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientMessagesTests
    {
        [Test]
        public async Task FahClientMessages_Clear_RemovesAllMessageData()
        {
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Heartbeat, String.Empty));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Info, "[ ]"));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Options, "{ }"));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" } ]"));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }"));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0 } ]"));
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.LogRestart, "\"Log\""));
                // Assert (pre-condition)
                Assert.IsNotNull(messages.Heartbeat);
                Assert.IsNotNull(messages.Info);
                Assert.IsNotNull(messages.Options);
                Assert.IsNotNull(messages.SlotCollection);
                Assert.AreNotEqual(0, messages.SlotOptionsCollection.Count);
                Assert.IsNotNull(messages.UnitCollection);
                Assert.AreNotEqual(0, messages.Log.ClientRuns.Count);
                Assert.IsTrue(messages.LogIsRetrieved);
                // Act
                messages.Clear();
                // Assert
                Assert.IsNull(messages.Heartbeat);
                Assert.IsNull(messages.Info);
                Assert.IsNull(messages.Options);
                Assert.IsNull(messages.SlotCollection);
                Assert.AreEqual(0, messages.SlotOptionsCollection.Count);
                Assert.IsNull(messages.UnitCollection);
                Assert.AreEqual(0, messages.Log.ClientRuns.Count);
                Assert.IsFalse(messages.LogIsRetrieved);
            }
        }

        [Test]
        public async Task FahClientMessages_SetupClientToSendMessageUpdatesAsync_ExecutesUpdateCommands()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var connection = (MockFahClientConnection)fahClient.Connection;
            var messages = new FahClientMessages(fahClient);
            // Act
            await messages.SetupClientToSendMessageUpdatesAsync();
            // Assert
            Assert.AreEqual(7, connection.Commands.Count);
            Assert.IsTrue(connection.Commands.All(x => x.Executed));
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestHeartbeat()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var heartbeat = CreateMessage(FahClientMessageType.Heartbeat, String.Empty);
            // Act
            var result = await messages.UpdateMessageAsync(heartbeat);
            // Assert
            Assert.AreEqual(heartbeat, messages.Heartbeat);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public void FahClientMessages_IsHeartbeatOverdue_ReturnsFalseWhenThereIsNoHeartbeatMessage()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            // Act
            var overdue = messages.IsHeartbeatOverdue();
            // Assert
            Assert.IsFalse(overdue);
        }

        [Test]
        public async Task FahClientMessages_IsHeartbeatOverdue_ReturnsTrueWhenHeartbeatHasNotBeenReceivedAfterLongPeriodOfTime()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var heartbeat = new FahClientMessage(new FahClientMessageIdentifier(FahClientMessageType.Heartbeat, DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5))), null);
            await messages.UpdateMessageAsync(heartbeat);
            // Act
            var overdue = messages.IsHeartbeatOverdue();
            // Assert
            Assert.IsTrue(overdue);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestInfo()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var info = CreateMessage(FahClientMessageType.Info, "[ ]");
            // Act
            var result = await messages.UpdateMessageAsync(info);
            // Assert
            Assert.IsNotNull(messages.Info);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestOptions()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var options = CreateMessage(FahClientMessageType.Options, "{ }");
            // Act
            var result = await messages.UpdateMessageAsync(options);
            // Assert
            Assert.IsNotNull(messages.Options);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestSlotCollection()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var messages = new FahClientMessages(fahClient);
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // Act
            var result = await messages.UpdateMessageAsync(slotInfo);
            // Assert
            Assert.IsNotNull(messages.SlotCollection);
            Assert.AreEqual(2, messages.SlotCollection.Count);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ExecutesSlotOptionsCommandForEachSlotInSlotCollection()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var connection = (MockFahClientConnection)fahClient.Connection;
            var messages = new FahClientMessages(fahClient);
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // Act
            var result = await messages.UpdateMessageAsync(slotInfo);
            // Assert
            Assert.AreEqual(2, connection.Commands.Count);
            Assert.IsTrue(connection.Commands.All(x => x.Executed));
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresEachSlotOptionsMessage()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var slotOptions0 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }");
            var slotOptions1 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"1\" }");
            // Act
            var result = await messages.UpdateMessageAsync(slotOptions0);
            // Assert
            Assert.AreEqual(1, messages.SlotOptionsCollection.Count);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
            // Act
            result = await messages.UpdateMessageAsync(slotOptions1);
            // Assert
            Assert.AreEqual(2, messages.SlotOptionsCollection.Count);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_PopulatesSlotWithSlotOptionsAfterReceivingAllSlotOptions()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var messages = new FahClientMessages(fahClient);
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // slot info must have already populated the SlotCollection
            await messages.UpdateMessageAsync(slotInfo);
            var slotOptions0 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }");
            var slotOptions1 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"1\" }");
            // Assert
            Assert.IsNull(messages.SlotCollection[0].SlotOptions);
            Assert.IsNull(messages.SlotCollection[1].SlotOptions);
            // Act
            await messages.UpdateMessageAsync(slotOptions0);
            // Assert
            Assert.IsNull(messages.SlotCollection[0].SlotOptions);
            Assert.IsNull(messages.SlotCollection[1].SlotOptions);
            // Act
            await messages.UpdateMessageAsync(slotOptions1);
            // Assert
            Assert.AreSame(messages.SlotOptionsCollection[0], messages.SlotCollection[0].SlotOptions);
            Assert.AreSame(messages.SlotOptionsCollection[1], messages.SlotCollection[1].SlotOptions);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ReturnsNotificationThatSlotInformationHasChangedAfterReceivingAllSlotOptions()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var messages = new FahClientMessages(fahClient);
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // slot info must have already populated the SlotCollection
            await messages.UpdateMessageAsync(slotInfo);
            var slotOptions0 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }");
            var slotOptions1 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"1\" }");
            // Act
            var result = await messages.UpdateMessageAsync(slotOptions0);
            // Assert
            Assert.AreEqual(1, messages.SlotOptionsCollection.Count);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
            // Act
            result = await messages.UpdateMessageAsync(slotOptions1);
            // Assert
            Assert.AreEqual(2, messages.SlotOptionsCollection.Count);
            Assert.IsTrue(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ReturnsNotificationThatClientDataShouldBeUpdatedWhenSlotInformationHasChangedAfterLogIsRetrieved()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" } ]");
                await messages.UpdateMessageAsync(slotInfo);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                await messages.UpdateMessageAsync(logRestart);
                slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\", \"description\": \"cpu:15\" } ]");
                var slotOptions = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }");
                // Act
                var result = await messages.UpdateMessageAsync(slotInfo);
                // Assert
                Assert.IsTrue(messages.LogIsRetrieved);
                Assert.IsFalse(result.SlotsUpdated);
                Assert.IsFalse(result.ExecuteRetrieval);
                // Act
                result = await messages.UpdateMessageAsync(slotOptions);
                // Assert
                Assert.IsTrue(messages.LogIsRetrieved);
                Assert.IsTrue(result.SlotsUpdated);
                Assert.IsTrue(result.ExecuteRetrieval);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestUnitCollection()
        {
            // Arrange
            var messages = new FahClientMessages(null);
            var queueInfo = CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0 }, { \"id\": \"00\", \"slot\": 1 } ]");
            // Act
            var result = await messages.UpdateMessageAsync(queueInfo);
            // Assert
            Assert.IsNotNull(messages.UnitCollection);
            Assert.AreEqual(2, messages.UnitCollection.Count);
            Assert.IsFalse(result.SlotsUpdated);
            Assert.IsFalse(result.ExecuteRetrieval);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ReturnsNotificationThatClientDataShouldBeUpdatedWhenUnitInformationHasChangedAfterLogIsRetrieved()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                var queueInfo = CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0, \"percentdone\": \"8%\" } ]");
                await messages.UpdateMessageAsync(queueInfo);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                await messages.UpdateMessageAsync(logRestart);
                queueInfo = CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0, \"percentdone\": \"9%\" } ]");
                // Act
                var result = await messages.UpdateMessageAsync(queueInfo);
                // Assert
                Assert.IsTrue(messages.LogIsRetrieved);
                Assert.IsFalse(result.SlotsUpdated);
                Assert.IsTrue(result.ExecuteRetrieval);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLogMessagesInFahClientLog()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                var result = await messages.UpdateMessageAsync(logRestart);
                // Assert
                Assert.AreEqual(1, messages.Log.ClientRuns[0].LogLines.Count);
                Assert.IsFalse(result.SlotsUpdated);
                Assert.IsFalse(result.ExecuteRetrieval);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_WritesLogMessagesToCachedClientLogFile()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                await messages.UpdateMessageAsync(logRestart);
                // Assert
                string path = Path.Combine(fahClient.Preferences.Get<string>(Preference.CacheDirectory), fahClient.Settings.ClientLogFileName);
                Assert.IsTrue(File.Exists(path));
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_LogIsNotRetrievedUntilMessageLengthIsNotLarge()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(fahClient);
                var logText = new String(Enumerable.Repeat('a', 66000).ToArray());
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, $"\"{logText}\"");
                // Act
                await messages.UpdateMessageAsync(logRestart);
                // Assert
                Assert.IsFalse(messages.LogIsRetrieved);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ExecutesQueueInfoCommandWhenLogIsRetrieved()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages(artifacts.Path);
                var connection = (MockFahClientConnection)fahClient.Connection;
                var messages = new FahClientMessages(fahClient);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                await messages.UpdateMessageAsync(logRestart);
                // Assert
                Assert.IsTrue(messages.LogIsRetrieved);
                Assert.AreEqual(1, connection.Commands.Count);
                var command = connection.Commands[0];
                Assert.AreEqual("queue-info", command.CommandText);
                Assert.IsTrue(command.Executed);
            }
        }

        private static FahClientMessage CreateMessage(string type, string text)
        {
            return new FahClientMessage(new FahClientMessageIdentifier(type, DateTime.UtcNow), new StringBuilder(text));
        }

        private static IFahClient SetupFahClientForSendingMockCommands()
        {
            var fahClient = MockRepository.GenerateStub<IFahClient>();
            var connection = new MockFahClientConnection();
            fahClient.Stub(x => x.Connection).Return(connection);
            return fahClient;
        }

        private static IFahClient SetupFahClientForHandlingLogMessages(string path)
        {
            var fahClient = MockRepository.GenerateStub<IFahClient>();
            fahClient.Stub(x => x.Logger).Return(NullLogger.Instance);
            var preferences = new InMemoryPreferenceSet(null, path, null);
            fahClient.Stub(x => x.Preferences).Return(preferences);
            var settings = new ClientSettings { Name = "test" };
            fahClient.Settings = settings;
            var connection = new MockFahClientConnection();
            fahClient.Stub(x => x.Connection).Return(connection);
            return fahClient;
        }

        private class MockFahClientConnection : FahClientConnection
        {
            public MockFahClientConnection()
                : base("foo", 2000)
            {

            }
            
            public IList<MockFahClientCommand> Commands { get; } = new List<MockFahClientCommand>();

            protected override FahClientCommand OnCreateCommand()
            {
                var command = new MockFahClientCommand(this);
                Commands.Add(command);
                return command;
            }
        }

        private class MockFahClientCommand : FahClientCommand
        {
            public MockFahClientCommand(FahClientConnection connection) : base(connection)
            {

            }

            public bool Executed { get; private set; }

            public override int Execute()
            {
                Executed = true;
                return 0;
            }

            public override Task<int> ExecuteAsync()
            {
                Executed = true;
                return Task.FromResult(0);
            }
        }
    }
}
