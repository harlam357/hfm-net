
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

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
            var workUnitRepository = MockRepository.GenerateMock<IWorkUnitRepository>();
            var fahClient = new FahClient(null, new InMemoryPreferenceSet(), benchmarkService, null, workUnitRepository);

            var workUnit = new WorkUnit();
            workUnit.ProjectID = 2669;
            workUnit.ProjectRun = 1;
            workUnit.ProjectClone = 2;
            workUnit.ProjectGen = 3;
            workUnit.Finished = new DateTime(2010, 1, 1);
            workUnit.QueueIndex = 0;
            var settings = new ClientSettings { Name = "Owner", Server = "Path", Port = ClientSettings.NoPort };
            var currentWorkUnit = new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnit);
            var slotIdentifier = currentWorkUnit.SlotModel.SlotIdentifier;

            var workUnitCopy = workUnit.Copy();
            workUnitCopy.FramesObserved = 4;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(0), ID = 0 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 1 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 2 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 3 });
            workUnitCopy.FrameData = frameDataDictionary;
            workUnitCopy.UnitResult = WorkUnitResult.FinishedUnit;

            var parsedUnits = new[] { new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnitCopy) };

            workUnitRepository.Stub(x => x.Connected).Return(true);
            workUnitRepository.Expect(x => x.Insert(null)).IgnoreArguments().Repeat.Times(1);

            var benchmarkIdentifier = new ProteinBenchmarkIdentifier(2669);

            // Assert (pre-condition)
            Assert.IsFalse(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsFalse(benchmarkService.GetBenchmarkProjects(slotIdentifier).Contains(2669));
            Assert.IsNull(benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier));

            // Act
            fahClient.UpdateWorkUnitBenchmarkAndRepository(currentWorkUnit, parsedUnits);

            // Assert
            Assert.IsTrue(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsTrue(benchmarkService.GetBenchmarkProjects(slotIdentifier).Contains(2669));
            Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkService.GetBenchmark(slotIdentifier, benchmarkIdentifier).AverageFrameTime);

            workUnitRepository.VerifyAllExpectations();
        }

        [Test]
        public void FahClient_UpdateBenchmarkFrameTimes_DoesNotUpdateBenchmarksWhenNoFramesAreComplete()
        {
            // Arrange
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var fahClient = new FahClient(null, new InMemoryPreferenceSet(), benchmarkService, null, null);

            var workUnit = new WorkUnit();
            workUnit.ProjectID = 12345;
            workUnit.ProjectRun = 6;
            workUnit.ProjectClone = 7;
            workUnit.ProjectGen = 8;
            workUnit.Assigned = DateTime.UtcNow;
            var settings = new ClientSettings { Name = "Foo", Server = "Bar", Port = ClientSettings.DefaultPort };
            var workUnitModel = new WorkUnitModel(new SlotModel(new NullClient { Settings = settings }), workUnit);
            var newWorkUnitModel = new WorkUnitModel(workUnitModel.SlotModel, workUnit.Copy());

            // Act
            fahClient.UpdateBenchmarkFrameTimes(workUnitModel, newWorkUnitModel);

            // Assert
            Assert.IsNull(benchmarkService.GetBenchmark(newWorkUnitModel.SlotModel.SlotIdentifier, newWorkUnitModel.BenchmarkIdentifier));
        }

        [Test]
        public async Task FahClient_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_10()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_10" };
            var fahClient = CreateClient(settings);
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_10\\slots.txt"))));
            // Act
            fahClient.RefreshSlots();
            // Assert
            var slots = fahClient.Slots.ToList();
            Assert.AreEqual(2, slots.Count);
            Assert.AreEqual(SlotType.CPU, slots[0].SlotType);
            Assert.AreEqual(4, slots[0].SlotThreads);
            Assert.AreEqual(null, slots[0].SlotProcessor);
            Assert.AreEqual(SlotType.GPU, slots[1].SlotType);
            Assert.AreEqual(null, slots[1].SlotThreads);
            Assert.AreEqual("GeForce GTX 285", slots[1].SlotProcessor);
        }

        [Test]
        public async Task FahClient_RefreshSlots_ParsesSlotDescriptionForSlotTypeAndSlotThreads_Client_v7_12()
        {
            // Arrange
            var settings = new ClientSettings { Name = "Client_v7_12" };
            var fahClient = CreateClient(settings);
            var extractor = new FahClientJsonMessageExtractor();
            await fahClient.Messages.UpdateMessageAsync(
                extractor.Extract(new StringBuilder(
                    File.ReadAllText("..\\..\\..\\TestFiles\\Client_v7_12\\slots.txt"))));
            // Act
            fahClient.RefreshSlots();
            // Assert
            var slots = fahClient.Slots.ToList();
            Assert.AreEqual(1, slots.Count);
            Assert.AreEqual(SlotType.CPU, slots[0].SlotType);
            Assert.AreEqual(4, slots[0].SlotThreads);
            Assert.AreEqual(null, slots[0].SlotProcessor);
        }

        private static FahClient CreateClient(ClientSettings settings)
        {
            var client = new FahClient(null, null, null, null, null);
            client.Settings = settings;
            return client;
        }
    }
}
