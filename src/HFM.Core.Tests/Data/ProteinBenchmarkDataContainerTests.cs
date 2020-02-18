
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

using NUnit.Framework;

using HFM.Core.Serializers;
using HFM.Core.WorkUnits;

namespace HFM.Core.Data
{
    [TestFixture]
    public class ProteinBenchmarkDataContainerTests
    {
        [Test]
        public void ProteinBenchmarkDataContainer_Read_FromDisk()
        {
            // Arrange
            var container = new ProteinBenchmarkDataContainer
            {
                FilePath = Path.Combine("..\\..\\TestFiles", ProteinBenchmarkDataContainer.DefaultFileName)
            };
            // Act
            container.Read();
            // Assert
            Assert.AreEqual(692, container.Data.Count);
        }

        [Test]
        public void ProteinBenchmarkDataContainer_Write_ToDisk()
        {
            // Arrange
            // TODO: Implement ArtifactFolder
            var collection = new ProteinBenchmarkDataContainer
            {
                FilePath = "TestProteinBenchmark.dat", Data = CreateTestBenchmarkData(),
            };
            // Act
            collection.Write();
            // Assert
            // clear the data and read it
            collection.Data = null;
            collection.Read();
            ValidateTestBenchmarkData(collection.Data);
        }

        [Test]
        public void ProteinBenchmark_WriteWithDataContractFileSerializer()
        {
            // Arrange
            var data = CreateTestBenchmarkData();
            var serializer = new DataContractFileSerializer<List<ProteinBenchmark>>();
            // Act
            // TODO: Implement ArtifactFolder
            serializer.Serialize("TestProteinBenchmark.xml", data);
            // Assert
            var fromXml = serializer.Deserialize("TestProteinBenchmark.xml");
            ValidateTestBenchmarkData(fromXml);
        }

        private static List<ProteinBenchmark> CreateTestBenchmarkData()
        {
            var list = new List<ProteinBenchmark>();
            for (int i = 0; i < 10; i++)
            {
                var benchmark = new ProteinBenchmark
                {
                    OwningClientName = "TestOwner",
                    OwningClientPath = "TestPath",
                    ProjectID = 100 + i
                };

                for (int j = 1; j < 6; j++)
                {
                    benchmark.AddFrameTime(TimeSpan.FromMinutes(j));
                }
                list.Add(benchmark);
            }

            for (int i = 10; i < 20; i++)
            {
                var benchmark = new ProteinBenchmark
                {
                    OwningClientName = "TestOwner2",
                    OwningClientPath = "TestPath2",
                    OwningSlotId = i - 10,
                    ProjectID = 200 + i
                };

                for (int j = 1; j < 6; j++)
                {
                    benchmark.AddFrameTime(TimeSpan.FromMinutes(j + 10));
                }
                list.Add(benchmark);
            }

            return list;
        }

        private static void ValidateTestBenchmarkData(List<ProteinBenchmark> list)
        {
            for (int i = 0; i < 10; i++)
            {
                ProteinBenchmark benchmark = list[i];
                Assert.AreEqual("TestOwner", benchmark.OwningSlotName);
                Assert.AreEqual("TestOwner", benchmark.OwningClientName);
                Assert.AreEqual("TestPath", benchmark.OwningClientPath);
                Assert.AreEqual(-1, benchmark.OwningSlotId);
                Assert.AreEqual(100 + i, benchmark.ProjectID);

                int index = 0;
                for (int j = 5; j > 0; j--)
                {
                    Assert.AreEqual(TimeSpan.FromMinutes(j), benchmark.FrameTimes[index].Duration);
                    index++;
                }
            }

            for (int i = 10; i < 20; i++)
            {
                ProteinBenchmark benchmark = list[i];
                Assert.AreEqual(String.Format(CultureInfo.InvariantCulture, "TestOwner2 Slot {0:00}", (i - 10)), benchmark.OwningSlotName);
                Assert.AreEqual("TestOwner2", benchmark.OwningClientName);
                Assert.AreEqual("TestPath2", benchmark.OwningClientPath);
                Assert.AreEqual(i - 10, benchmark.OwningSlotId);
                Assert.AreEqual(200 + i, benchmark.ProjectID);

                int index = 0;
                for (int j = 5; j > 0; j--)
                {
                    Assert.AreEqual(TimeSpan.FromMinutes(j + 10), benchmark.FrameTimes[index].Duration);
                    index++;
                }
            }
        }
    }
}
