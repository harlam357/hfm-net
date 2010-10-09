/*
 * HFM.NET - Benchmark Container Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class ProteinBenchmarkContainerTests
   {
      private MockRepository _mocks;
      private IPreferenceSet _prefs;
      private IUnitInfoDatabase _database;
      private ProteinBenchmarkContainer _container;
   
      [SetUp]
      public void Init()
      {
         _mocks = new MockRepository();
         _prefs = _mocks.DynamicMock<IPreferenceSet>();
         _database = _mocks.DynamicMock<IUnitInfoDatabase>();
         _container = new ProteinBenchmarkContainer(_prefs, _database);
      }

      [Test]
      public void UpdateBenchmarkDataTest()
      {
         var unitInfo1 = new UnitInfo();
         unitInfo1.OwningInstanceName = "Owner";
         unitInfo1.OwningInstancePath = "Path";
         unitInfo1.ProjectID = 2669;
         unitInfo1.ProjectRun = 1;
         unitInfo1.ProjectClone = 2;
         unitInfo1.ProjectGen = 3;
         unitInfo1.UnitFrames.Add(0, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(0), FrameID = 0 });
         unitInfo1.UnitFrames.Add(1, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 1 });
         unitInfo1.UnitFrames.Add(2, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 2 });
         unitInfo1.UnitFrames.Add(3, new UnitFrame { FrameDuration = TimeSpan.FromMinutes(5), FrameID = 3 });
         unitInfo1.FinishedTime = new DateTime(2010, 1, 1);

         var currentUnitInfo = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(currentUnitInfo.UnitInfoData).Return(unitInfo1);
         SetupResult.For(currentUnitInfo.LastUnitFrameID).Return(0);
         
         var unitInfoLogic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic1.UnitInfoData).Return(unitInfo1);
         SetupResult.For(unitInfoLogic1.LastUnitFrameID).Return(3);

         var unitInfo2 = new UnitInfo();
         unitInfo2.OwningInstanceName = "Owner";
         unitInfo2.OwningInstancePath = "Path";
         unitInfo2.ProjectID = 2669;
         unitInfo2.ProjectRun = 2;
         unitInfo2.ProjectClone = 3;
         unitInfo2.ProjectGen = 4;
         unitInfo2.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic2 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic2.UnitInfoData).Return(unitInfo2);

         var unitInfo3 = new UnitInfo();
         unitInfo3.OwningInstanceName = "Owner";
         unitInfo3.OwningInstancePath = "Path";
         unitInfo3.ProjectID = 2669;
         unitInfo3.ProjectRun = 3;
         unitInfo3.ProjectClone = 4;
         unitInfo3.ProjectGen = 5;
         unitInfo3.FinishedTime = new DateTime(2010, 1, 1);
         var unitInfoLogic3 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic3.UnitInfoData).Return(unitInfo3);

         var parsedUnits = new List<IUnitInfoLogic> { unitInfoLogic1, unitInfoLogic2, unitInfoLogic3 };

         Expect.Call(() => _database.WriteUnitInfo(null)).IgnoreArguments().Repeat.Times(3);
         
         _mocks.ReplayAll();

         Assert.AreEqual(false, _container.ContainsClient(new BenchmarkClient("Owner", "Path")));
         Assert.AreEqual(false, new List<int>(_container.GetBenchmarkProjects()).Contains(2669));
         Assert.AreEqual(TimeSpan.Zero, _container.GetBenchmarkAverageFrameTime(currentUnitInfo));
         
         _container.UpdateBenchmarkData(currentUnitInfo, parsedUnits.ToArray(), 2);
         
         Assert.AreEqual(true, _container.ContainsClient(new BenchmarkClient("Owner", "Path")));
         Assert.AreEqual(true, new List<int>(_container.GetBenchmarkProjects()).Contains(2669));
         Assert.AreEqual(TimeSpan.FromMinutes(5), _container.GetBenchmarkAverageFrameTime(currentUnitInfo));
         
         _mocks.VerifyAll();
      }

      [Test]
      public void ProtoBufSerializationTest()
      {
         var collection = LoadTestCollection();
         ValidateTestCollection(collection);
         ProteinBenchmarkContainer.Serialize(collection, "ProtoBufTest.dat");
         
         ProteinBenchmarkCollection collection2 = ProteinBenchmarkContainer.Deserialize("ProtoBufTest.dat");
         ValidateTestCollection(collection2);
      }

      [Test]
      public void ProtoBufDeserializeFileNotFoundTest()
      {
         ProteinBenchmarkCollection testCollection = ProteinBenchmarkContainer.Deserialize("FileNotFound.dat");
         Assert.IsNull(testCollection);
      }

      [Test]
      public void XmlSerializationTest()
      {
         var collection = LoadTestCollection();
         ValidateTestCollection(collection);
         ProteinBenchmarkContainer.SerializeToXml(collection, "XmlTest.xml");

         ProteinBenchmarkCollection collection2 = ProteinBenchmarkContainer.DeserializeFromXml("XmlTest.xml");
         ValidateTestCollection(collection2);
      }
      
      private static ProteinBenchmarkCollection LoadTestCollection()
      {
         var collection = new ProteinBenchmarkCollection();
         
         for (int i = 0; i < 10; i++)
         {
            var benchmark = new InstanceProteinBenchmark("TestOwner", "TestPath", 100 + i);
            for (int j = 1; j < 6; j++)
            {
               benchmark.SetFrameTime(TimeSpan.FromMinutes(j));
            }
            collection.BenchmarkList.Add(benchmark);
         }
         
         return collection;
      }
      
      private static void ValidateTestCollection(ProteinBenchmarkCollection collection)
      {
         for (int i = 0; i < 10; i++)
         {
            InstanceProteinBenchmark benchmark = collection.BenchmarkList[i];
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

      //[Test]
      //public void MergeCollectionsTest()
      //{
      //   var collection = LoadTestCollection();
      //   int count = collection.BenchmarkList.Count;

      //   ProteinBenchmarkCollection mergedCollection = ProteinBenchmarkContainer.MergeCollections(collection, collection);
      //   Assert.AreEqual(count * 2, mergedCollection.BenchmarkList.Count);

      //   mergedCollection = ProteinBenchmarkContainer.MergeCollections(collection, null);
      //   Assert.AreEqual(count, mergedCollection.BenchmarkList.Count);

      //   mergedCollection = ProteinBenchmarkContainer.MergeCollections(null, collection);
      //   Assert.AreEqual(count, mergedCollection.BenchmarkList.Count);

      //   mergedCollection = ProteinBenchmarkContainer.MergeCollections(null, null);
      //   Assert.IsNull(mergedCollection);
      //}
   }
}
