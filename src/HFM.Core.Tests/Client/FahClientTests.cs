/*
 * HFM.NET - Fah Client Class Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Client;
using HFM.Core.Data;
using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Proteins;

namespace HFM.Core.Client
{
    [TestFixture]
    public class FahClientTests
    {
        [Test]
        public void FahClient_ArgumentNullException_Test()
        {
            Assert.Throws<ArgumentNullException>(() => new FahClient(null));
        }

        [Test]
        public void FahClient_UpdateBenchmarkData_Test()
        {
            // setup
            var benchmarks = new ProteinBenchmarkService(new ProteinBenchmarkDataContainer());
            var repository = MockRepository.GenerateMock<IWorkUnitRepository>();
            var fahClient = new FahClient(MockRepository.GenerateStub<IMessageConnection>()) { BenchmarkService = benchmarks, WorkUnitRepository = repository };

            var workUnit = new WorkUnit();
            workUnit.OwningClientName = "Owner";
            workUnit.OwningClientPath = "Path";
            workUnit.OwningSlotId = 0;
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
            repository.Stub(x => x.Connected).Return(true);
            repository.Expect(x => x.Insert(null)).IgnoreArguments().Repeat.Times(1);

            var benchmarkClient = new ProteinBenchmarkSlotIdentifier("Owner Slot 00", "Path");

            // Assert (pre-condition)
            Assert.IsFalse(benchmarks.DataContainer.Data.Any(x => x.ToSlotIdentifier().Equals(benchmarkClient)));
            Assert.IsFalse(new List<int>(benchmarks.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
            Assert.IsNull(benchmarks.GetBenchmark(currentWorkUnit.Data));

            // Act
            fahClient.UpdateBenchmarkData(currentWorkUnit, parsedUnits);

            // Assert
            Assert.IsTrue(benchmarks.DataContainer.Data.Any(x => x.ToSlotIdentifier().Equals(benchmarkClient)));
            Assert.IsTrue(new List<int>(benchmarks.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
            Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarks.GetBenchmark(currentWorkUnit.Data).AverageFrameTime);

            repository.VerifyAllExpectations();
        }
    }
}
