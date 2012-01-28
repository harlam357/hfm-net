/*
 * HFM.NET - Fah Client Class Tests
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class FahClientTests
   {
      [Test]
      public void UpdateBenchmarkDataTest()
      {
         // setup
         var benchmarkCollection = new ProteinBenchmarkCollection();
         var database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         var fahClient = new FahClient(MockRepository.GenerateStub<IFahClientInterface>()) { BenchmarkCollection = benchmarkCollection, UnitInfoDatabase = database };

         var unitInfo1 = new UnitInfo();
         unitInfo1.OwningClientName = "Owner";
         unitInfo1.OwningClientPath = "Path";
         unitInfo1.OwningSlotId = 0;
         unitInfo1.ProjectID = 2669;
         unitInfo1.ProjectRun = 1;
         unitInfo1.ProjectClone = 2;
         unitInfo1.ProjectGen = 3;
         unitInfo1.FinishedTime = new DateTime(2010, 1, 1);
         unitInfo1.QueueIndex = 0;
         var currentUnitInfo = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo1 };

         var unitInfo1Clone = unitInfo1.DeepClone();
         unitInfo1Clone.UnitFrames.Add(0, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(0), FrameID = 0 });
         unitInfo1Clone.UnitFrames.Add(1, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 1 });
         unitInfo1Clone.UnitFrames.Add(2, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 2 });
         unitInfo1Clone.UnitFrames.Add(3, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 3 });
         var unitInfoLogic1 = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo1Clone };

         var parsedUnits = new[] { unitInfoLogic1 };

         // arrange
         database.Stub(x => x.Connected).Return(true);
         database.Expect(x => x.WriteUnitInfo(null)).IgnoreArguments().Repeat.Times(1);

         var benchmarkClient = new BenchmarkClient("Owner Slot 0", "Path");

         // assert before act
         Assert.AreEqual(false, benchmarkCollection.Contains(benchmarkClient));
         Assert.AreEqual(false, new List<int>(benchmarkCollection.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.IsNull(benchmarkCollection.GetBenchmark(currentUnitInfo.UnitInfoData));

         // act
         fahClient.UpdateBenchmarkData(currentUnitInfo, parsedUnits, 0);

         // assert after act
         Assert.AreEqual(true, benchmarkCollection.Contains(benchmarkClient));
         Assert.AreEqual(true, new List<int>(benchmarkCollection.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkCollection.GetBenchmark(currentUnitInfo.UnitInfoData).AverageFrameTime);

         database.VerifyAllExpectations();
      }
   }
}
