/*
 * HFM.NET - Query Parameters Collection Class Tests
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using HFM.Core.DataTypes;
using HFM.Core.Serializers;

namespace HFM.Core.Tests
{
   [TestFixture]
   public class QueryParameterCollectionTests
   {
      [Test]
      public void ReadTest1()
      {
         var collection = new QueryParametersCollection
         {
            FileName = Path.Combine("..\\..\\TestFiles", Constants.QueryCacheFileName),
            Serializer = new ProtoBufFileSerializer<List<QueryParameters>>()
         };

         collection.Read();
         Assert.AreEqual(10, collection.Count);
      }

      [Test]
      public void WriteTest1()
      {
         var collection = new QueryParametersCollection
         {
            FileName = "TestQueryParametersBinary.dat",
            Serializer = new ProtoBufFileSerializer<List<QueryParameters>>()
         };

         collection.Data = CreateTestList();
         collection.Write();
         collection.Data = null;
         collection.Read();
         ValidateTestList(collection.Data);
      }

      private static List<QueryParameters> CreateTestList()
      {
         var list = new List<QueryParameters>();
         for (int i = 0; i < 5; i++)
         {
            var queryParameters = new QueryParameters();
            queryParameters.Name = "Test" + i;
            queryParameters.Fields.Add(new QueryField { Name = QueryFieldName.InstanceName, Type = QueryFieldType.Equal, Value = "Test" + i });
            list.Add(queryParameters);
         }

         return list;
      }

      private static void ValidateTestList(IList<QueryParameters> list)
      {
         for (int i = 0; i < 5; i++)
         {
            QueryParameters queryParameters = list[i];
            Assert.AreEqual("Test" + i, queryParameters.Name);
            Assert.AreEqual(QueryFieldName.InstanceName, queryParameters.Fields[0].Name);
            Assert.AreEqual(QueryFieldType.Equal, queryParameters.Fields[0].Type);
            Assert.AreEqual("Test" + i, queryParameters.Fields[0].Value);
         }
      }
   }
}
