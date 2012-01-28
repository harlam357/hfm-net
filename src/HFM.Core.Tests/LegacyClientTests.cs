/*
 * HFM.NET - Legacy Client Class Tests
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
   public class LegacyClientTests
   {
      [Test]
      public void UpdateBenchmarkDataTest()
      {
         // setup
         var benchmarkCollection = new ProteinBenchmarkCollection();
         var database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         var legacyClient = new LegacyClient { BenchmarkCollection = benchmarkCollection, UnitInfoDatabase = database };

         var unitInfo1 = new UnitInfo();
         unitInfo1.OwningClientName = "Owner";
         unitInfo1.OwningClientPath = "Path";
         unitInfo1.ProjectID = 2669;
         unitInfo1.ProjectRun = 1;
         unitInfo1.ProjectClone = 2;
         unitInfo1.ProjectGen = 3;
         unitInfo1.FinishedTime = new DateTime(2010, 1, 1);
         var currentUnitInfo = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo1 };

         var unitInfo1Clone = unitInfo1.DeepClone();
         unitInfo1Clone.UnitFrames.Add(0, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(0), FrameID = 0 });
         unitInfo1Clone.UnitFrames.Add(1, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 1 });
         unitInfo1Clone.UnitFrames.Add(2, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 2 });
         unitInfo1Clone.UnitFrames.Add(3, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 3 });
         var unitInfoLogic1 = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo1Clone };

         var unitInfo2 = new UnitInfo();
         unitInfo2.OwningClientName = "Owner";
         unitInfo2.OwningClientPath = "Path";
         unitInfo2.ProjectID = 2669;
         unitInfo2.ProjectRun = 2;
         unitInfo2.ProjectClone = 3;
         unitInfo2.ProjectGen = 4;
         unitInfo2.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic2 = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo2 };

         var unitInfo3 = new UnitInfo();
         unitInfo3.OwningClientName = "Owner";
         unitInfo3.OwningClientPath = "Path";
         unitInfo3.ProjectID = 2669;
         unitInfo3.ProjectRun = 3;
         unitInfo3.ProjectClone = 4;
         unitInfo3.ProjectGen = 5;
         unitInfo3.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic3 = new UnitInfoLogic { CurrentProtein = new Protein(), UnitInfoData = unitInfo3 };

         var parsedUnits = new[] { unitInfoLogic1, unitInfoLogic2, unitInfoLogic3 };

         // arrange
         database.Stub(x => x.Connected).Return(true);
         database.Expect(x => x.WriteUnitInfo(null)).IgnoreArguments().Repeat.Times(3);

         var benchmarkClient = new BenchmarkClient("Owner", "Path");

         // assert before act
         Assert.AreEqual(false, benchmarkCollection.Contains(benchmarkClient));
         Assert.AreEqual(false, new List<int>(benchmarkCollection.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.IsNull(benchmarkCollection.GetBenchmark(currentUnitInfo.UnitInfoData));

         // act
         legacyClient.UpdateBenchmarkData(currentUnitInfo, parsedUnits, 2);

         // assert after act
         Assert.AreEqual(true, benchmarkCollection.Contains(benchmarkClient));
         Assert.AreEqual(true, new List<int>(benchmarkCollection.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.AreEqual(TimeSpan.FromMinutes(5), benchmarkCollection.GetBenchmark(currentUnitInfo.UnitInfoData).AverageFrameTime);

         database.VerifyAllExpectations();
      }
   }
}
