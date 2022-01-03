using System;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Log;

using Moq;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientMessageActionTests
    {
        [Test]
        public void DelegateFahClientMessageAction_Execute_DoesNotRunActionWhenMessageTypeDoesNotMatch()
        {
            // Arrange
            bool executed = false;
            var action = new DelegateFahClientMessageAction(FahClientMessageType.SlotInfo, () => executed = true);
            // Act
            action.Execute(null);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public void DelegateFahClientMessageAction_Execute_RunsActionWhenMessageTypeMatches()
        {
            // Arrange
            bool executed = false;
            var action = new DelegateFahClientMessageAction(FahClientMessageType.SlotInfo, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.SlotInfo);
            // Assert
            Assert.IsTrue(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_DoesNotRunActionWhenMessageTypeIsSlotInfoAndLogIsNotRetrieved()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.SlotInfo);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_RunsActionWhenMessageTypeIsSlotInfoAndLogIsRetrieved()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessagesWithLogRetrieved();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.SlotInfo);
            // Assert
            Assert.IsTrue(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_DoesNotRunActionWhenMessageTypeIsQueueInfoAndUnitCollectionHasNoItems()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessagesWithLogRetrieved();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.QueueInfo);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_DoesNotRunActionWhenMessageTypeIsQueueInfoAndLogIsNotRetrieved()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.QueueInfo);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public async Task ExecuteRetrieveMessageAction_Execute_RunsActionWhenMessageTypeIsQueueInfoAndUnitCollectionHasItemsAndLogIsRetrieved()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessagesWithLogRetrieved();
            var queueInfo = CreateMessage(FahClientMessageType.QueueInfo, "[ { \"id\": \"01\", \"slot\": 0 }, { \"id\": \"00\", \"slot\": 1 } ]");
            await messages.UpdateMessageAsync(queueInfo);
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.QueueInfo);
            // Assert
            Assert.IsTrue(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_DoesNotRunActionWhenMessageTypeIsLogRestartAndSlotCollectionIsNull()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.LogRestart);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public async Task ExecuteRetrieveMessageAction_Execute_RunsActionWhenMessageTypeIsLogRestartAndSlotCollectionIsNotNull()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            await messages.UpdateMessageAsync(slotInfo);
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.LogRestart);
            // Assert
            Assert.IsTrue(executed);
        }

        [Test]
        public void ExecuteRetrieveMessageAction_Execute_DoesNotRunActionWhenMessageTypeIsLogUpdateAndSlotCollectionIsNull()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.LogUpdate);
            // Assert
            Assert.IsFalse(executed);
        }

        [Test]
        public async Task ExecuteRetrieveMessageAction_Execute_RunsActionWhenMessageTypeIsLogUpdateAndSlotCollectionIsNotNull()
        {
            // Arrange
            bool executed = false;
            var messages = CreateFahClientMessages();
            var slotInfo = CreateMessage(FahClientMessageType.SlotInfo, "[ { \"id\": \"00\" }, { \"id\": \"01\" } ]");
            await messages.UpdateMessageAsync(slotInfo);
            var action = new ExecuteRetrieveMessageAction(messages, () => executed = true);
            // Act
            action.Execute(FahClientMessageType.LogUpdate);
            // Assert
            Assert.IsTrue(executed);
        }

        private static FahClientMessages CreateFahClientMessages() => new(Mock.Of<IFahClient>());

        private static FahClientMessages CreateFahClientMessagesWithLogRetrieved()
        {
            var messages = CreateFahClientMessages();
            messages.Log.ClientRuns.Add(new ClientRun(messages.Log, 0));
            return messages;
        }

        private static FahClientMessage CreateMessage(string type, string text) =>
            new(new FahClientMessageIdentifier(type, DateTime.UtcNow), new StringBuilder(text));
    }
}
