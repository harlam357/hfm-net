﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

using Moq;

using Newtonsoft.Json;

using NUnit.Framework;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientTests
    {
        [Test]
        public void FahClient_UpdateWorkUnitBenchmarkAndRepository()
        {
            // Arrange
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var mockWorkUnitRepository = new Mock<IWorkUnitRepository>();
            var fahClient = new FahClient(null, new InMemoryPreferencesProvider(), benchmarkService, null, mockWorkUnitRepository.Object);

            var workUnit = new WorkUnit();
            workUnit.ProjectID = 2669;
            workUnit.ProjectRun = 1;
            workUnit.ProjectClone = 2;
            workUnit.ProjectGen = 3;
            workUnit.Finished = new DateTime(2010, 1, 1);
            workUnit.ID = 0;
            var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
            var previousWorkUnitModel = new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnit);
            var slotIdentifier = previousWorkUnitModel.SlotModel.SlotIdentifier;

            var workUnitCopy = workUnit.Copy();
            workUnitCopy.FramesObserved = 4;
            var frames = new Dictionary<int, LogLineFrameData>()
               .With(new LogLineFrameData { Duration = TimeSpan.FromMinutes(0), ID = 0 },
                     new LogLineFrameData { Duration = TimeSpan.FromMinutes(5), ID = 1 },
                     new LogLineFrameData { Duration = TimeSpan.FromMinutes(5), ID = 2 },
                     new LogLineFrameData { Duration = TimeSpan.FromMinutes(5), ID = 3 });
            workUnitCopy.Frames = frames;
            workUnitCopy.UnitResult = WorkUnitResult.FinishedUnit;

            var workUnitModels = new[] { new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnitCopy) };

            mockWorkUnitRepository.SetupGet(x => x.Connected).Returns(true);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(2669);

            // Assert (pre-condition)
            Assert.IsFalse(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsFalse(benchmarkService.GetBenchmarkProjects(slotIdentifier).Contains(2669));
            Assert.IsNull(benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier));

            // Act
            fahClient.UpdateWorkUnitBenchmarkAndRepository(workUnitModels, previousWorkUnitModel);

            // Assert
            Assert.IsTrue(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsTrue(benchmarkService.GetBenchmarkProjects(slotIdentifier).Contains(2669));
            Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier).AverageFrameTime);

            mockWorkUnitRepository.Verify(x => x.Insert(It.IsAny<WorkUnitModel>()), Times.Once);
        }

        [Test]
        public void FahClient_UpdateBenchmarkFrameTimes_DoesNotUpdateBenchmarksWhenNoFramesAreComplete()
        {
            // Arrange
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var fahClient = new FahClient(null, new InMemoryPreferencesProvider(), benchmarkService, null, null);

            var workUnit = new WorkUnit();
            workUnit.ProjectID = 12345;
            workUnit.ProjectRun = 6;
            workUnit.ProjectClone = 7;
            workUnit.ProjectGen = 8;
            workUnit.Assigned = DateTime.UtcNow;
            var settings = new ClientSettings { Name = "Foo", Server = "Bar", Port = ClientSettings.DefaultPort };
            var previousWorkUnitModel = new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnit);
            var workUnitModel = new WorkUnitModel(previousWorkUnitModel.SlotModel, workUnit.Copy());

            // Act
            fahClient.UpdateBenchmarkFrameTimes(previousWorkUnitModel, workUnitModel);

            // Assert
            Assert.IsNull(benchmarkService.GetBenchmark(workUnitModel.SlotModel.SlotIdentifier, workUnitModel.BenchmarkIdentifier));
        }

        [Test]
        public async Task FahClient_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_10()
        {
            // Arrange
            var fahClient = CreateClient("Client_v7_10");
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText(@"..\..\..\..\TestFiles\Client_v7_10\slots.txt"))));
            // Act
            fahClient.RefreshSlots();
            // Assert
            var slots = fahClient.Slots.Cast<FahClientSlotModel>().ToList();
            Assert.AreEqual(2, slots.Count);
            Assert.AreEqual(SlotType.CPU, slots[0].SlotType);
            Assert.AreEqual(4, slots[0].Threads);
            Assert.AreEqual(null, slots[0].Processor);
            Assert.AreEqual(SlotType.GPU, slots[1].SlotType);
            Assert.AreEqual(null, slots[1].Threads);
            Assert.AreEqual("GeForce GTX 285", slots[1].Processor);
        }

        [Test]
        public async Task FahClient_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_12()
        {
            // Arrange
            var fahClient = CreateClient("Client_v7_12");
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText(@"..\..\..\..\TestFiles\Client_v7_12\slots.txt"))));
            // Act
            fahClient.RefreshSlots();
            // Assert
            var slots = fahClient.Slots.Cast<FahClientSlotModel>().ToList();
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(SlotType.CPU, slots[0].SlotType);
            Assert.AreEqual(4, slots[0].Threads);
            Assert.AreEqual(null, slots[0].Processor);
        }

        [Test]
        public async Task FahClient_RefreshSlots_ParsesDisabledSlotStatus()
        {
            // Arrange
            var fahClient = CreateClient("ParsesDisabledSlotStatus");
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

            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(buffer));
            // Act
            fahClient.RefreshSlots();
            // Assert
            var slots = fahClient.Slots.Cast<FahClientSlotModel>().ToList();
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(SlotType.Unknown, slots[0].SlotType);
            Assert.AreEqual(SlotStatus.Disabled, slots[0].Status);
        }

        private static FahClient CreateClient(string clientName)
        {
            var client = new FahClient(null, null, null, null, null);
            client.Settings = new ClientSettings { Name = clientName };
            return client;
        }
    }
}
