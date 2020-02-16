/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
                FilePath = Path.Combine("..\\..\\TestFiles", WorkUnitQueryDataContainer.DefaultFileName),
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
                FilePath = "TestQueryParametersBinary.dat", Data = CreateTestQueryParameters(),
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
