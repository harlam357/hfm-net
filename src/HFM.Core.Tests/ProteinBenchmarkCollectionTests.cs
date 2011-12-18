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

using HFM.Core.DataTypes;
using HFM.Core.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ProteinBenchmarkCollectionTests
   {
      [Test]
      public void ReadTest1()
      {
         var container = new ProteinBenchmarkCollection
         {
            FileName = Path.Combine("..\\..\\TestFiles", Constants.BenchmarkCacheFileName),
         };

         container.Read();
         Assert.AreEqual(1246, container.Count);
      }

      [Test]
      public void WriteTest1()
      {
         var collection = new ProteinBenchmarkCollection
         {
            FileName = "TestProteinBenchmarkBinary.dat",
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
         var collection = new ProteinBenchmarkCollection();
         var serializer = new XmlFileSerializer<List<ProteinBenchmark>>();

         collection.Data = CreateTestList();
         collection.Write("TestProteinBenchmarkXml.xml", serializer);
         var data = collection.Read("TestProteinBenchmarkXml.xml", serializer);
         ValidateTestList(data);
      }
      
      private static List<ProteinBenchmark> CreateTestList()
      {
         var list = new List<ProteinBenchmark>();
         for (int i = 0; i < 10; i++)
         {
            var benchmark = new ProteinBenchmark
                            {
                               OwningSlotName = "TestOwner",
                               OwningSlotPath = "TestPath",
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
            Assert.AreEqual("TestOwner", benchmark.OwningSlotName);
            Assert.AreEqual("TestPath", benchmark.OwningSlotPath);
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
