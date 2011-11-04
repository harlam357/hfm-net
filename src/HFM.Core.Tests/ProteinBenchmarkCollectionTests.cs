/*
 * HFM.NET - Benchmark Collection Class Tests
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ProteinBenchmarkCollectionTests
   {
      [Test]
      public void UpdateBenchmarkDataTest()
      {
         // setup
         var database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         var container = new ProteinBenchmarkCollection(database);

         var unitInfo1 = new UnitInfo();
         unitInfo1.OwningInstanceName = "Owner";
         unitInfo1.OwningInstancePath = "Path";
         unitInfo1.ProjectID = 2669;
         unitInfo1.ProjectRun = 1;
         unitInfo1.ProjectClone = 2;
         unitInfo1.ProjectGen = 3;
         unitInfo1.FinishedTime = new DateTime(2010, 1, 1);
         var currentUnitInfo = new UnitInfoLogic(new Protein(), container, unitInfo1);

         var unitInfo1Clone = unitInfo1.DeepClone();
         unitInfo1Clone.UnitFrames.Add(0, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(0), FrameID = 0 });
         unitInfo1Clone.UnitFrames.Add(1, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 1 });
         unitInfo1Clone.UnitFrames.Add(2, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 2 });
         unitInfo1Clone.UnitFrames.Add(3, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 3 });
         var unitInfoLogic1 = new UnitInfoLogic(new Protein(), container, unitInfo1Clone);

         var unitInfo2 = new UnitInfo();
         unitInfo2.OwningInstanceName = "Owner";
         unitInfo2.OwningInstancePath = "Path";
         unitInfo2.ProjectID = 2669;
         unitInfo2.ProjectRun = 2;
         unitInfo2.ProjectClone = 3;
         unitInfo2.ProjectGen = 4;
         unitInfo2.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic2 = new UnitInfoLogic(new Protein(), container, unitInfo2);

         var unitInfo3 = new UnitInfo();
         unitInfo3.OwningInstanceName = "Owner";
         unitInfo3.OwningInstancePath = "Path";
         unitInfo3.ProjectID = 2669;
         unitInfo3.ProjectRun = 3;
         unitInfo3.ProjectClone = 4;
         unitInfo3.ProjectGen = 5;
         unitInfo3.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic3 = new UnitInfoLogic(new Protein(), container, unitInfo3);

         var parsedUnits = new[] { unitInfoLogic1, unitInfoLogic2, unitInfoLogic3 };

         // arrange
         database.Stub(x => x.ConnectionOk).Return(true);
         database.Expect(x => x.WriteUnitInfo(null)).IgnoreArguments().Repeat.Times(3);

         var benchmarkClient = new BenchmarkClient("Owner", "Path");

         // assert before act
         Assert.AreEqual(false, container.Contains(benchmarkClient));
         Assert.AreEqual(false, new List<int>(container.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.IsNull(container.GetBenchmark(currentUnitInfo.UnitInfoData));
         
         // act
         container.UpdateData(currentUnitInfo, parsedUnits, 2);

         // assert after act
         Assert.AreEqual(true, container.Contains(benchmarkClient));
         Assert.AreEqual(true, new List<int>(container.GetBenchmarkProjects(benchmarkClient)).Contains(2669));
         Assert.AreEqual(TimeSpan.FromMinutes(5), container.GetBenchmark(currentUnitInfo.UnitInfoData).AverageFrameTime);
         
         database.VerifyAllExpectations();
      }

      [Test]
      public void ReadTest1()
      {
         var container = new ProteinBenchmarkCollection(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = Path.Combine("..\\..\\TestFiles", Constants.BenchmarkCacheFileName),
            Serializer = new ProtoBufFileSerializer<List<ProteinBenchmark>>()
         };

         container.Read();
         Assert.AreEqual(1246, container.Count);
      }

      [Test]
      public void WriteTest1()
      {
         var collection = new ProteinBenchmarkCollection(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = "TestProteinBenchmarkBinary.dat",
            Serializer = new ProtoBufFileSerializer<List<ProteinBenchmark>>()
         };

         collection.Data = CreateTestList();
         collection.Write();
         collection.Data = null;
         collection.Read();
         ValidateTestList(collection.Data);
      }

      [Test]
      public void WriteTest2()
      {
         var collection = new ProteinBenchmarkCollection(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = "TestProteinBenchmarkBinary.dat",
            Serializer = new XmlFileSerializer<List<ProteinBenchmark>>()
         };

         collection.Data = CreateTestList();
         collection.Write();
         collection.Data = null;
         collection.Read();
         ValidateTestList(collection.Data);
      }
      
      private static List<ProteinBenchmark> CreateTestList()
      {
         var list = new List<ProteinBenchmark>();
         for (int i = 0; i < 10; i++)
         {
            var benchmark = new ProteinBenchmark
                            {
                               OwningInstanceName = "TestOwner",
                               OwningInstancePath = "TestPath",
                               ProjectID = 100 + i
                            };
            
            for (int j = 1; j < 6; j++)
            {
               benchmark.SetFrameTime(TimeSpan.FromMinutes(j));
            }
            list.Add(benchmark);
         }

         return list;
      }

      private static void ValidateTestList(IList<ProteinBenchmark> list)
      {
         for (int i = 0; i < 10; i++)
         {
            ProteinBenchmark benchmark = list[i];
            Assert.AreEqual("TestOwner", benchmark.OwningInstanceName);
            Assert.AreEqual("TestPath", benchmark.OwningInstancePath);
            Assert.AreEqual(100 + i, benchmark.ProjectID);
            
            int index = 0;
            for (int j = 5; j > 0; j--)
            {
               Assert.AreEqual(TimeSpan.FromMinutes(j), benchmark.FrameTimes[index].Duration);
               index++;
            }
         }
      }
   }
}
