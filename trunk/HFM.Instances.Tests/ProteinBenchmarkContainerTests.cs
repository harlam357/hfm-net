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
         var unitInfoLogic1 = _mocks.DynamicMock<IUnitInfoLogic>();
         SetupResult.For(unitInfoLogic1.UnitInfoData).Return(unitInfo1);
         
         
         //_container.UpdateBenchmarkData();
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

      [Test]
      public void MergeCollectionsTest()
      {
         var collection = LoadTestCollection();
         int count = collection.BenchmarkList.Count;

         ProteinBenchmarkCollection mergedCollection = ProteinBenchmarkContainer.MergeCollections(collection, collection);
         Assert.AreEqual(count * 2, mergedCollection.BenchmarkList.Count);

         mergedCollection = ProteinBenchmarkContainer.MergeCollections(collection, null);
         Assert.AreEqual(count, mergedCollection.BenchmarkList.Count);

         mergedCollection = ProteinBenchmarkContainer.MergeCollections(null, collection);
         Assert.AreEqual(count, mergedCollection.BenchmarkList.Count);

         mergedCollection = ProteinBenchmarkContainer.MergeCollections(null, null);
         Assert.IsNull(mergedCollection);
      }
   }
}
