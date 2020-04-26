
using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

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
            workUnit.FinishedTime = new DateTime(2010, 1, 1);
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

            // Assert (pre-condition)
            Assert.IsFalse(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsFalse(new List<int>(benchmarkService.GetBenchmarkProjects(slotIdentifier)).Contains(2669));
            Assert.IsNull(benchmarkService.GetBenchmark(slotIdentifier, 2669));

            // Act
            fahClient.UpdateWorkUnitBenchmarkAndRepository(currentWorkUnit, parsedUnits);

            // Assert
            Assert.IsTrue(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsTrue(new List<int>(benchmarkService.GetBenchmarkProjects(slotIdentifier)).Contains(2669));
            Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkService.GetBenchmark(slotIdentifier, 2669).AverageFrameTime);

            workUnitRepository.VerifyAllExpectations();
        }
    }
}
