using System.Text;

using HFM.Core.Client.Mocks;
using HFM.Core.WorkUnits;
using HFM.Preferences;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientTests
    {
        [TestFixture]
        public class GivenConnectedClientSettingsChanged : FahClientTests
        {
            private MockFahClient _fahClient;

            [SetUp]
            public async Task BeforeEach()
            {
                _fahClient = MockFahClient.Create("test");
                await _fahClient.Connect();
            }

            [Test]
            public void Then_ClosesConnectionWhenDisabledChanges()
            {
                Assert.IsTrue(_fahClient.Connected);
                _fahClient.Settings = new ClientSettings { Name = "test", Disabled = true };
                Assert.IsFalse(_fahClient.Connected);
            }
        }

        [TestFixture]
        public class GivenConnectedClientReceivesSlotsMessage : FahClientTests
        {
            private MockFahClient _fahClient;

            [SetUp]
            public async Task BeforeEach()
            {
                _fahClient = MockFahClient.Create("test");
                await _fahClient.Connect();
            }

            [Test]
            public async Task Then_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_10()
            {
                await _fahClient.LoadMessage(@"..\..\..\..\TestFiles\Client_v7_10\slots.txt");

                _fahClient.RefreshSlots();

                var collection = _fahClient.ClientDataCollection.Cast<FahClientData>().ToList();
                Assert.AreEqual(2, collection.Count);
                Assert.AreEqual(SlotType.CPU, collection[0].SlotType);
                Assert.AreEqual(4, collection[0].Threads);
                Assert.AreEqual(null, collection[0].Processor);
                Assert.AreEqual(SlotType.GPU, collection[1].SlotType);
                Assert.AreEqual(null, collection[1].Threads);
                Assert.AreEqual("GeForce GTX 285", collection[1].Processor);
            }

            [Test]
            public async Task Then_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_12()
            {
                await _fahClient.LoadMessage(@"..\..\..\..\TestFiles\Client_v7_12\slots.txt");

                _fahClient.RefreshSlots();

                var collection = _fahClient.ClientDataCollection.Cast<FahClientData>().ToList();
                Assert.AreEqual(1, collection.Count);
                Assert.AreEqual(SlotType.CPU, collection[0].SlotType);
                Assert.AreEqual(4, collection[0].Threads);
                Assert.AreEqual(null, collection[0].Processor);
            }

            [Test]
            public async Task Then_RefreshSlots_ParsesDisabledSlotStatus()
            {
                // Arrange
                var buffer = new StringBuilder();
                buffer.AppendLine("PyON 1 slots");
                buffer.AppendLine(JsonConvert.SerializeObject(
                    new[]
                    {
                        new
                        {
                            id = "00",
                            status = "DISABLED"
                        }
                    }));
                buffer.AppendLine("---");
                await _fahClient.LoadMessage(buffer);

                _fahClient.RefreshSlots();

                var slots = _fahClient.ClientDataCollection.Cast<FahClientData>().ToList();
                Assert.AreEqual(1, slots.Count);
                Assert.AreEqual(SlotType.Unknown, slots[0].SlotType);
                Assert.AreEqual(SlotStatus.Disabled, slots[0].Status);
            }
        }

        [TestFixture]
        public class GivenConnectedClientRetrievesData : FahClientTests
        {
            // Version 7.6.21

            private MockFahClient _fahClient;

            [SetUp]
            public async Task BeforeEach()
            {
                _fahClient = MockFahClient.Create("test");
                await _fahClient.Connect();
                await _fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_19");
                _fahClient.RefreshSlots();
                await _fahClient.Retrieve();
            }

            [Test]
            public void ThenSlotsAreUpdated()
            {
                var slot00 = (FahClientData)_fahClient.ClientDataCollection.ElementAt(0);
                Assert.AreEqual("AMD Ryzen 7 3700X 8-Core Processor", slot00.Processor);
                Assert.IsNull(slot00.WorkUnitQueue);
                Assert.AreNotEqual(0, slot00.CurrentLogLines.Count);
                Assert.AreEqual(-1, slot00.WorkUnitModel.ID);

                var slot01 = (FahClientData)_fahClient.ClientDataCollection.ElementAt(1);
                Assert.AreEqual("Geforce RTX 2060", slot01.Processor);
                Assert.AreEqual(1, slot01.WorkUnitQueue.Count);
                Assert.AreNotEqual(0, slot01.CurrentLogLines.Count);
                Assert.AreEqual(0, slot01.WorkUnitModel.ID);
            }

            [Test]
            public void ThenGpuSlotProcessorShowsPlatformImplementationAndDriverVersion()
            {
                _fahClient.Preferences.Set(Preference.DisplayVersions, true);
                var slot01 = _fahClient.ClientDataCollection.ElementAt(1);
                Assert.AreEqual("Geforce RTX 2060 (CUDA 456.71)", slot01.Processor);
            }

            [Test]
            public void ThenTheWorkUnitRepositoryIsUpdated()
            {
                var mockWorkUnitRepository = Mock.Get(_fahClient.WorkUnitRepository);
                mockWorkUnitRepository.Verify(x => x.UpdateAsync(It.IsAny<WorkUnitModel>()), Times.Once);
            }
        }

        [TestFixture]
        public class GivenConnectedClientRetrievesDataFromOlderClient : FahClientTests
        {
            // Version 7.6.13

            private MockFahClient _fahClient;

            [SetUp]
            public async Task BeforeEach()
            {
                _fahClient = MockFahClient.Create("test");
                await _fahClient.Connect();
                await _fahClient.LoadMessagesFrom(@"..\..\..\..\TestFiles\Client_v7_20");
                _fahClient.RefreshSlots();
                await _fahClient.Retrieve();
            }

            [Test]
            public void ThenSlotsAreUpdated()
            {
                var slot00 = (FahClientData)_fahClient.ClientDataCollection.ElementAt(0);
                Assert.AreEqual("Tesla T4", slot00.Processor);
                Assert.AreEqual(1, slot00.WorkUnitQueue.Count);
                Assert.AreNotEqual(0, slot00.CurrentLogLines.Count);
                Assert.AreEqual(1, slot00.WorkUnitModel.ID);

                var slot01 = (FahClientData)_fahClient.ClientDataCollection.ElementAt(1);
                Assert.AreEqual("Tesla T4", slot01.Processor);
                Assert.AreEqual(1, slot01.WorkUnitQueue.Count);
                Assert.AreNotEqual(0, slot01.CurrentLogLines.Count);
                Assert.AreEqual(2, slot01.WorkUnitModel.ID);
            }

            [Test]
            public void ThenGpuSlotProcessorShowsPlatformImplementationAndDriverVersion()
            {
                _fahClient.Preferences.Set(Preference.DisplayVersions, true);
                var slot00 = _fahClient.ClientDataCollection.ElementAt(0);
                Assert.AreEqual("Tesla T4 (CUDA 470.82)", slot00.Processor);

                _fahClient.Preferences.Set(Preference.DisplayVersions, true);
                var slot01 = _fahClient.ClientDataCollection.ElementAt(1);
                Assert.AreEqual("Tesla T4 (CUDA 470.82)", slot01.Processor);
            }

            [Test]
            public void ThenTheWorkUnitRepositoryIsUpdated()
            {
                var mockWorkUnitRepository = Mock.Get(_fahClient.WorkUnitRepository);
                mockWorkUnitRepository.Verify(x => x.UpdateAsync(It.IsAny<WorkUnitModel>()), Times.Exactly(2));
            }
        }

        [TestFixture]
        public class GivenConnectedClientSendsCommandsToSendMessageUpdates : FahClientTests
        {
            private MockFahClient _fahClient;

            [SetUp]
            public async Task BeforeEach()
            {
                _fahClient = MockFahClient.Create("test");
                await _fahClient.Connect();
                await _fahClient.SetupClientToSendMessageUpdatesAsync();
            }

            [Test]
            public void Then_CommandsAreExecuted()
            {
                var connection = (MockFahClientConnection)_fahClient.Connection;
                Assert.AreEqual(7, connection.Commands.Count);
                Assert.IsTrue(connection.Commands.All(x => x.Executed));
            }
        }
    }
}
