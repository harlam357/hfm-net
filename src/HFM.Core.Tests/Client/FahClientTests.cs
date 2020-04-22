
using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientTests
    {
        [Test]
        public void FahClient_UpdateBenchmarkData_Test()
        {
            // setup
            var benchmarkService = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var workUnitRepository = MockRepository.GenerateMock<IWorkUnitRepository>();
            var fahClient = new FahClient(null, new InMemoryPreferenceSet(), null, benchmarkService, workUnitRepository);

            var slotIdentifier = new SlotIdentifier(ClientIdentifier.FromPath("Owner", "Path"), SlotIdentifier.NoSlotID);

            var workUnit = new WorkUnit();
            workUnit.SlotIdentifier = slotIdentifier;
            workUnit.ProjectID = 2669;
            workUnit.ProjectRun = 1;
            workUnit.ProjectClone = 2;
            workUnit.ProjectGen = 3;
            workUnit.FinishedTime = new DateTime(2010, 1, 1);
            workUnit.QueueIndex = 0;
            var currentWorkUnit = new WorkUnitModel { CurrentProtein = new Protein(), Data = workUnit };

            var workUnitCopy = workUnit.DeepClone();
            workUnitCopy.FramesObserved = 4;
            var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
               .With(new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(0), ID = 0 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 1 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 2 },
                     new WorkUnitFrameData { Duration = TimeSpan.FromMinutes(5), ID = 3 });
            workUnitCopy.FrameData = frameDataDictionary;
            workUnitCopy.UnitResult = WorkUnitResult.FinishedUnit;

            var parsedUnits = new[] { new WorkUnitModel { CurrentProtein = new Protein(), Data = workUnitCopy } };

            // Arrange
            workUnitRepository.Stub(x => x.Connected).Return(true);
            workUnitRepository.Expect(x => x.Insert(null)).IgnoreArguments().Repeat.Times(1);

            // Assert (pre-condition)
            Assert.IsFalse(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsFalse(new List<int>(benchmarkService.GetBenchmarkProjects(slotIdentifier)).Contains(2669));
            Assert.IsNull(benchmarkService.GetBenchmark(slotIdentifier, 2669));

            // Act
            fahClient.UpdateBenchmarkData(currentWorkUnit, parsedUnits);

            // Assert
            Assert.IsTrue(benchmarkService.DataContainer.Data.Any(x => x.SlotIdentifier.Equals(slotIdentifier)));
            Assert.IsTrue(new List<int>(benchmarkService.GetBenchmarkProjects(slotIdentifier)).Contains(2669));
            Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkService.GetBenchmark(slotIdentifier, 2669).AverageFrameTime);

            workUnitRepository.VerifyAllExpectations();
        }
    }
}
