/*
 * HFM.NET - Query Parameters Class Tests
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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

namespace HFM.Core.Data
{
    [TestFixture]
    public class WorkUnitHistoryQueryTests
    {
        [Test]
        public void WorkUnitHistoryQuery_Create_Test()
        {
            var param = new WorkUnitHistoryQuery();
            Assert.AreEqual(WorkUnitHistoryQuery.SelectAll.Name, param.Name);
            Assert.AreEqual(0, param.Parameters.Count);
        }

        [Test]
        public void WorkUnitHistoryQuery_DeepClone_Test()
        {
            var param = new WorkUnitHistoryQuery("Test")
                .AddParameter(WorkUnitHistoryRowColumn.Name, QueryFieldType.Equal, "Test Instance")
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, QueryFieldType.GreaterThan, new DateTime(2000, 1, 1));

            var copy = param.DeepClone();
            Assert.AreNotSame(param, copy);
            Assert.AreEqual(param.Name, copy.Name);
            for (int i = 0; i < param.Parameters.Count; i++)
            {
                Assert.AreEqual(param.Parameters[i].Column, copy.Parameters[i].Column);
                Assert.AreEqual(param.Parameters[i].Type, copy.Parameters[i].Type);
                Assert.AreEqual(param.Parameters[i].Value, copy.Parameters[i].Value);
            }
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test1()
        {
            var param1 = new WorkUnitHistoryQuery();
            var param2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(0, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test2()
        {
            var param1 = new WorkUnitHistoryQuery { Name = "Name" };
            var param2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(1, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test3()
        {
            var param1 = new WorkUnitHistoryQuery();
            var param2 = new WorkUnitHistoryQuery { Name = "Name" };
            Assert.AreEqual(-1, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test4()
        {
            var param1 = new WorkUnitHistoryQuery { Name = null };
            var param2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(-1, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test5()
        {
            var param1 = new WorkUnitHistoryQuery { Name = null };
            var param2 = new WorkUnitHistoryQuery { Name = null };
            Assert.AreEqual(0, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test6()
        {
            var param1 = new WorkUnitHistoryQuery { Name = "A" };
            var param2 = new WorkUnitHistoryQuery { Name = "B" };
            Assert.AreEqual(-1, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test7()
        {
            var param1 = new WorkUnitHistoryQuery { Name = "B" };
            var param2 = new WorkUnitHistoryQuery { Name = "A" };
            Assert.AreEqual(1, param1.CompareTo(param2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test8()
        {
            var param1 = new WorkUnitHistoryQuery();
            Assert.AreEqual(1, param1.CompareTo(null));
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Create_Test()
        {
            var field = new WorkUnitHistoryQueryParameter();
            Assert.AreEqual(WorkUnitHistoryRowColumn.ProjectID, field.Column);
            Assert.AreEqual(QueryFieldType.Equal, field.Type);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test1()
        {
            var field = new WorkUnitHistoryQueryParameter();
            field.Value = "Value";
            Assert.AreEqual("Value", field.Value);
            field.Value = null;
            Assert.IsNull(field.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test2()
        {
            var field = new WorkUnitHistoryQueryParameter();
            field.Value = new DateTime(2000, 1, 1);
            Assert.AreEqual(new DateTime(2000, 1, 1), field.Value);
            field.Value = null;
            Assert.IsNull(field.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test3()
        {
            var field = new WorkUnitHistoryQueryParameter();
            field.Value = 6900;
            Assert.AreEqual("6900", field.Value);
            field.Value = null;
            Assert.IsNull(field.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Default_Test()
        {
            var field = new WorkUnitHistoryQueryParameter();
            Assert.AreEqual("=", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Equal_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.Equal };
            Assert.AreEqual("=", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_GreaterThan_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.GreaterThan };
            Assert.AreEqual(">", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_GreaterThanOrEqual_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.GreaterThanOrEqual };
            Assert.AreEqual(">=", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_LessThan_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.LessThan };
            Assert.AreEqual("<", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_LessThanOrEqual_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.LessThanOrEqual };
            Assert.AreEqual("<=", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Like_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.Like };
            Assert.AreEqual("LIKE", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_NotLike_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.NotLike };
            Assert.AreEqual("NOT LIKE", field.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_NotEqual_Test()
        {
            var field = new WorkUnitHistoryQueryParameter { Type = QueryFieldType.NotEqual };
            Assert.AreEqual("!=", field.Operator);
        }
    }
}
