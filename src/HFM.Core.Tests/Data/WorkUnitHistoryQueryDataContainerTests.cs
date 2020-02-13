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
    public class WorkUnitHistoryQueryDataContainerTests
    {
        [Test]
        public void WorkUnitHistoryQueryDataContainer_Read_FromDisk()
        {
            // Arrange
            var container = new WorkUnitHistoryQueryDataContainer
            {
                FilePath = Path.Combine("..\\..\\TestFiles", WorkUnitHistoryQueryDataContainer.DefaultFileName),
            };
            // Act
            container.Read();
            // Assert
            Assert.AreEqual(10, container.Data.Count);
        }

        [Test]
        public void WorkUnitHistoryQueryDataContainer_Write_ToDisk()
        {
            // Arrange
            // TODO: Implement ArtifactFolder
            var container = new WorkUnitHistoryQueryDataContainer
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

        private static List<WorkUnitHistoryQuery> CreateTestQueryParameters()
        {
            var list = new List<WorkUnitHistoryQuery>();
            for (int i = 0; i < 5; i++)
            {
                list.Add(new WorkUnitHistoryQuery("Test" + i)
                    .AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Equal, "Test" + i));
            }

            return list;
        }

        private static void ValidateTestQueryParameters(IList<WorkUnitHistoryQuery> list)
        {
            for (int i = 0; i < 5; i++)
            {
                WorkUnitHistoryQuery workUnitHistoryQuery = list[i];
                Assert.AreEqual("Test" + i, workUnitHistoryQuery.Name);
                Assert.AreEqual(WorkUnitHistoryRowColumn.Name, workUnitHistoryQuery.Parameters[0].Column);
                Assert.AreEqual(WorkUnitHistoryQueryOperator.Equal, workUnitHistoryQuery.Parameters[0].Type);
                Assert.AreEqual("Test" + i, workUnitHistoryQuery.Parameters[0].Value);
            }
        }
    }
}
