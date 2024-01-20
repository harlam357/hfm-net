using HFM.Client;
using HFM.Core.Client.Mocks;
using HFM.Preferences;

using Moq;

using NUnit.Framework;

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
                var fahClient = SetupFahClientForHandlingLogMessages();
                var preferences = SetupPreferencesProviderForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(null, preferences);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Heartbeat, String.Empty), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Info, "[ ]"), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.Options, "{ }"), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" } ]"), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }"), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0 } ]"), fahClient);
                await messages.UpdateMessageAsync(CreateMessage(FahClientMessageType.LogRestart, "\"Log\""), fahClient);
                // Assert (pre-condition)
                Assert.IsNotNull(messages.Heartbeat);
                Assert.IsNotNull(messages.Info);
                Assert.IsNotNull(messages.Options);
                Assert.IsNotNull(messages.SlotCollection);
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
                Assert.IsNull(messages.UnitCollection);
                Assert.AreEqual(0, messages.Log.ClientRuns.Count);
                Assert.IsFalse(messages.LogIsRetrieved);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestHeartbeat()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            var heartbeat = CreateMessage(FahClientMessageType.Heartbeat, String.Empty);
            // Act
            var result = await messages.UpdateMessageAsync(heartbeat, null);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(heartbeat, messages.Heartbeat);
        }

        [Test]
        public void FahClientMessages_IsHeartbeatOverdue_ReturnsFalseWhenThereIsNoHeartbeatMessage()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            // Act
            var overdue = messages.IsHeartbeatOverdue();
            // Assert
            Assert.IsFalse(overdue);
        }

        [Test]
        public async Task FahClientMessages_IsHeartbeatOverdue_ReturnsTrueWhenHeartbeatHasNotBeenReceivedAfterLongPeriodOfTime()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            var heartbeat = new FahClientMessage(
                new FahClientMessageIdentifier(
                    FahClientMessageType.Heartbeat,
                    DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(5))),
                new(0),
                FahClientMessage.JsonMessageFormat);
            await messages.UpdateMessageAsync(heartbeat, null);
            // Act
            var overdue = messages.IsHeartbeatOverdue();
            // Assert
            Assert.IsTrue(overdue);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestInfo()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            var info = CreateMessage(FahClientMessageType.Info, "[ ]");
            // Act
            var result = await messages.UpdateMessageAsync(info, null);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.Info);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestOptions()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            var options = CreateMessage(FahClientMessageType.Options, "{ }");
            // Act
            var result = await messages.UpdateMessageAsync(options, null);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.Options);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestSlotCollection()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var messages = CreateFahClientMessages();
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // Act
            var result = await messages.UpdateMessageAsync(slotInfo, fahClient);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.SlotCollection);
            Assert.AreEqual(2, messages.SlotCollection.Count);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_ExecutesSlotOptionsCommandForEachSlotInSlotCollection()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var connection = (MockFahClientConnection)fahClient.Connection;
            var messages = CreateFahClientMessages();
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // Act
            var result = await messages.UpdateMessageAsync(slotInfo, fahClient);
            // Assert
            Assert.IsTrue(result);
            Assert.AreEqual(2, connection.Commands.Count);
            Assert.IsTrue(connection.Commands.All(x => x.Executed));
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_PopulatesSlotWithSlotOptions()
        {
            // Arrange
            var fahClient = SetupFahClientForSendingMockCommands();
            var messages = CreateFahClientMessages();
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            // slot info must have already populated the SlotCollection
            await messages.UpdateMessageAsync(slotInfo, fahClient);
            var slotOptions0 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"0\" }");
            var slotOptions1 = CreateMessage(FahClientMessageType.SlotOptions, "{ \"machine-id\": \"1\" }");
            // Assert
            Assert.IsNull(messages.SlotCollection[0].SlotOptions);
            Assert.IsNull(messages.SlotCollection[1].SlotOptions);
            // Act
            var result = await messages.UpdateMessageAsync(slotOptions0, fahClient);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.SlotCollection[0].SlotOptions);
            Assert.IsNull(messages.SlotCollection[1].SlotOptions);
            // Act
            result = await messages.UpdateMessageAsync(slotOptions1, fahClient);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.SlotCollection[0].SlotOptions);
            Assert.IsNotNull(messages.SlotCollection[1].SlotOptions);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLatestUnitCollection()
        {
            // Arrange
            var messages = CreateFahClientMessages();
            var queueInfo = CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0 }, { \"id\": \"00\", \"slot\": 1 } ]");
            // Act
            var result = await messages.UpdateMessageAsync(queueInfo, null);
            // Assert
            Assert.IsTrue(result);
            Assert.IsNotNull(messages.UnitCollection);
            Assert.AreEqual(2, messages.UnitCollection.Count);
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_StoresLogMessagesInFahClientLog()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages();
                var preferences = SetupPreferencesProviderForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(null, preferences);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                var result = await messages.UpdateMessageAsync(logRestart, fahClient);
                // Assert
                Assert.IsTrue(result);
                Assert.AreEqual(1, messages.Log.ClientRuns[0].LogLines.Count);
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_WritesLogMessagesToCachedClientLogFile()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages();
                var preferences = SetupPreferencesProviderForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(null, preferences);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                await messages.UpdateMessageAsync(logRestart, fahClient);
                // Assert
                string path = Path.Combine(preferences.Get<string>(Preference.CacheDirectory), fahClient.Settings.ClientLogFileName);
                Assert.IsTrue(File.Exists(path));
            }
        }

        [Test]
        public async Task FahClientMessages_UpdateMessageAsync_LogIsNotRetrievedUntilMessageLengthIsNotLarge()
        {
            // Arrange
            using (var artifacts = new ArtifactFolder())
            {
                var fahClient = SetupFahClientForHandlingLogMessages();
                var preferences = SetupPreferencesProviderForHandlingLogMessages(artifacts.Path);
                var messages = new FahClientMessages(null, preferences);
                var logText = new string(Enumerable.Repeat('a', UInt16.MaxValue).ToArray());
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, $"\"{logText}\"");
                // Act
                await messages.UpdateMessageAsync(logRestart, fahClient);
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
                var fahClient = SetupFahClientForHandlingLogMessages();
                var preferences = SetupPreferencesProviderForHandlingLogMessages(artifacts.Path);
                var connection = (MockFahClientConnection)fahClient.Connection;
                var messages = new FahClientMessages(null, preferences);
                var logRestart = CreateMessage(FahClientMessageType.LogRestart, "\"Log\"");
                // Act
                await messages.UpdateMessageAsync(logRestart, fahClient);
                // Assert
                Assert.IsTrue(messages.LogIsRetrieved);
                Assert.AreEqual(1, connection.Commands.Count);
                var command = connection.Commands[0];
                Assert.AreEqual("queue-info", command.CommandText);
                Assert.IsTrue(command.Executed);
            }
        }

        private static FahClientMessages CreateFahClientMessages() => new(null, null, null);

        private static FahClientMessage CreateMessage(string type, string text) => new(
            new(type, DateTime.UtcNow),
            new(text),
            FahClientMessage.JsonMessageFormat);

        private static IFahClient SetupFahClientForSendingMockCommands()
        {
            var mockClient = new Mock<IFahClient>();
            var connection = new MockFahClientConnection();
            mockClient.SetupGet(x => x.Connection).Returns(connection);
            return mockClient.Object;
        }

        private static IFahClient SetupFahClientForHandlingLogMessages()
        {
            var mockClient = new Mock<IFahClient>();
            var settings = new ClientSettings { Name = "test" };
            mockClient.SetupProperty(x => x.Settings);
            mockClient.Object.Settings = settings;
            var connection = new MockFahClientConnection();
            mockClient.SetupGet(x => x.Connection).Returns(connection);
            return mockClient.Object;
        }

        private static IPreferences SetupPreferencesProviderForHandlingLogMessages(string path) =>
            new InMemoryPreferencesProvider(null, path, null);
    }
}
