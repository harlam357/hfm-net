
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using NUnit.Framework;
using Rhino.Mocks;

using Castle.Core.Logging;

using HFM.Core.DataTypes;
using HFM.Core.DataTypes.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ProteinBenchmarkContainerTests
   {
      [Test]
      public void UpdateBenchmarkDataTest()
      {
         // setup
         var database = MockRepository.GenerateMock<IUnitInfoDatabase>();
         var container = new ProteinBenchmarkContainer(database);

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
         var container = new ProteinBenchmarkContainer(MockRepository.GenerateStub<IUnitInfoDatabase>())
                         {
                            FileName = Path.Combine("..\\..\\TestFiles", Constants.BenchmarkCacheFileName),
                            Serializer = new ProtoBufFileSerializer<List<ProteinBenchmark>>()
                         };

         container.Read();
         Assert.AreEqual(1246, container.Count);
      }

      [Test]
      public void ReadTest2()
      {
         var container = new ProteinBenchmarkContainer(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = Path.Combine("..\\..\\DirectoryNotFound", Constants.BenchmarkCacheFileName),
            Serializer = new ProtoBufFileSerializer<List<ProteinBenchmark>>()
         };

         container.Read();
         Assert.AreEqual(0, container.Count);
      }

      [Test]
      public void WriteTest1()
      {
         var container = new ProteinBenchmarkContainer(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = Path.Combine("TestProteinBenchmarkBinary.dat", Constants.BenchmarkCacheFileName),
            Serializer = new ProtoBufFileSerializer<List<ProteinBenchmark>>()
         };

         var list = CreateTestList();
         foreach (ProteinBenchmark benchmark in list)
         {
            container.Add(benchmark);
         }
         Assert.AreEqual(list.Count, container.Count);
         container.Write();
         container.Clear();
         Assert.AreEqual(0, container.Count);
         container.Read();
         Assert.AreEqual(list.Count, container.Count);
         ValidateTestList(container.ToList());
      }

      [Test]
      public void WriteTest2()
      {
         var container = new ProteinBenchmarkContainer(MockRepository.GenerateStub<IUnitInfoDatabase>())
         {
            FileName = Path.Combine("TestProteinBenchmark.xml", Constants.BenchmarkCacheFileName),
            Serializer = new XmlFileSerializer<List<ProteinBenchmark>>()
         };

         var list = CreateTestList();
         foreach (ProteinBenchmark item in list)
         {
            container.Add(item);
         }
         Assert.AreEqual(list.Count, container.Count);
         container.Write();
         container.Clear();
         Assert.AreEqual(0, container.Count);
         container.Read();
         Assert.AreEqual(list.Count, container.Count);
         ValidateTestList(container.ToList());
      }
      
      private static IList<ProteinBenchmark> CreateTestList()
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
         
         return list.AsReadOnly();
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
