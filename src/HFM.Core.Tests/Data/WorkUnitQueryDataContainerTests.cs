using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitQueryDataContainerTests
    {
        [Test]
        public void WorkUnitQueryDataContainer_Read_FromDisk()
        {
            // Arrange
            var container = new WorkUnitQueryDataContainer
            {
                FilePath = Path.Combine("TestFiles", WorkUnitQueryDataContainer.DefaultFileName),
            };
            // Act
            container.Read();
            // Assert
            Assert.AreEqual(10, container.Data.Count);
        }

        [Test]
        public void WorkUnitQueryDataContainer_Write_ToDisk()
        {
            // Arrange
            // TODO: Implement ArtifactFolder
            var container = new WorkUnitQueryDataContainer
            {
                FilePath = "TestQueryParametersBinary.dat",
                Data = CreateTestQueryParameters(),
            };
            // Act
            container.Write();
            // Assert
            // clear the data and read it
            container.Data = null;
            container.Read();
            ValidateTestQueryParameters(container.Data);
        }

        private static List<WorkUnitQuery> CreateTestQueryParameters()
        {
            var list = new List<WorkUnitQuery>();
            for (int i = 0; i < 5; i++)
            {
                list.Add(new WorkUnitQuery("Test" + i)
                    .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, "Test" + i));
            }

            return list;
        }

        private static void ValidateTestQueryParameters(IList<WorkUnitQuery> list)
        {
            for (int i = 0; i < 5; i++)
            {
                WorkUnitQuery workUnitQuery = list[i];
                Assert.AreEqual("Test" + i, workUnitQuery.Name);
                Assert.AreEqual(WorkUnitRowColumn.Name, workUnitQuery.Parameters[0].Column);
                Assert.AreEqual(WorkUnitQueryOperator.Equal, workUnitQuery.Parameters[0].Operator);
                Assert.AreEqual("Test" + i, workUnitQuery.Parameters[0].Value);
            }
        }
    }
}
