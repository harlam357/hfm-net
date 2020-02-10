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

using HFM.Core.DataTypes;

namespace HFM.Core.Data
{
    [TestFixture]
    public class QueryParametersDataContainerTests
    {
        [Test]
        public void QueryParametersDataContainer_Read_FromDisk()
        {
            // Arrange
            var container = new QueryParametersDataContainer
            {
                FilePath = Path.Combine("..\\..\\TestFiles", QueryParametersDataContainer.DefaultFileName),
            };
            // Act
            container.Read();
            // Assert
            Assert.AreEqual(10, container.Data.Count);
        }

        [Test]
        public void QueryParametersDataContainer_Write_ToDisk()
        {
            // Arrange
            // TODO: Implement ArtifactFolder
            var container = new QueryParametersDataContainer
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

        private static List<QueryParameters> CreateTestQueryParameters()
        {
            var list = new List<QueryParameters>();
            for (int i = 0; i < 5; i++)
            {
                var queryParameters = new QueryParameters { Name = "Test" + i };
                queryParameters.Fields.Add(new QueryField { Name = QueryFieldName.Name, Type = QueryFieldType.Equal, Value = "Test" + i });
                list.Add(queryParameters);
            }

            return list;
        }

        private static void ValidateTestQueryParameters(IList<QueryParameters> list)
        {
            for (int i = 0; i < 5; i++)
            {
                QueryParameters queryParameters = list[i];
                Assert.AreEqual("Test" + i, queryParameters.Name);
                Assert.AreEqual(QueryFieldName.Name, queryParameters.Fields[0].Name);
                Assert.AreEqual(QueryFieldType.Equal, queryParameters.Fields[0].Type);
                Assert.AreEqual("Test" + i, queryParameters.Fields[0].Value);
            }
        }
    }
}
