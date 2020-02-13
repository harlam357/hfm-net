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
            var query = new WorkUnitHistoryQuery();
            Assert.AreEqual(WorkUnitHistoryQuery.SelectAll.Name, query.Name);
            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void WorkUnitHistoryQuery_DeepClone_Test()
        {
            var query = new WorkUnitHistoryQuery("Test")
                .AddParameter(WorkUnitHistoryRowColumn.Name, WorkUnitHistoryQueryOperator.Equal, "Test Instance")
                .AddParameter(WorkUnitHistoryRowColumn.DownloadDateTime, WorkUnitHistoryQueryOperator.GreaterThan, new DateTime(2000, 1, 1));

            var copy = query.DeepClone();
            Assert.AreNotSame(query, copy);
            Assert.AreEqual(query.Name, copy.Name);
            for (int i = 0; i < query.Parameters.Count; i++)
            {
                Assert.AreEqual(query.Parameters[i].Column, copy.Parameters[i].Column);
                Assert.AreEqual(query.Parameters[i].Operator, copy.Parameters[i].Operator);
                Assert.AreEqual(query.Parameters[i].Value, copy.Parameters[i].Value);
            }
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test1()
        {
            var query1 = new WorkUnitHistoryQuery();
            var query2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(0, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test2()
        {
            var query1 = new WorkUnitHistoryQuery { Name = "Name" };
            var query2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test3()
        {
            var query1 = new WorkUnitHistoryQuery();
            var query2 = new WorkUnitHistoryQuery { Name = "Name" };
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test4()
        {
            var query1 = new WorkUnitHistoryQuery { Name = null };
            var query2 = new WorkUnitHistoryQuery();
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test5()
        {
            var query1 = new WorkUnitHistoryQuery { Name = null };
            var query2 = new WorkUnitHistoryQuery { Name = null };
            Assert.AreEqual(0, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test6()
        {
            var query1 = new WorkUnitHistoryQuery { Name = "A" };
            var query2 = new WorkUnitHistoryQuery { Name = "B" };
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test7()
        {
            var query1 = new WorkUnitHistoryQuery { Name = "B" };
            var query2 = new WorkUnitHistoryQuery { Name = "A" };
            Assert.AreEqual(1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitHistoryQuery_Compare_Test8()
        {
            var query1 = new WorkUnitHistoryQuery();
            Assert.AreEqual(1, query1.CompareTo(null));
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Create_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter();
            Assert.AreEqual(WorkUnitHistoryRowColumn.ProjectID, parameter.Column);
            Assert.AreEqual(WorkUnitHistoryQueryOperator.Equal, parameter.Operator);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test1()
        {
            var parameter = new WorkUnitHistoryQueryParameter();
            parameter.Value = "Value";
            Assert.AreEqual("Value", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test2()
        {
            var parameter = new WorkUnitHistoryQueryParameter();
            parameter.Value = new DateTime(2000, 1, 1);
            Assert.AreEqual(new DateTime(2000, 1, 1), parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Value_Test3()
        {
            var parameter = new WorkUnitHistoryQueryParameter();
            parameter.Value = 6900;
            Assert.AreEqual("6900", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Default_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter();
            Assert.AreEqual("=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Equal_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.Equal };
            Assert.AreEqual("=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_GreaterThan_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.GreaterThan };
            Assert.AreEqual(">", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_GreaterThanOrEqual_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.GreaterThanOrEqual };
            Assert.AreEqual(">=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_LessThan_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.LessThan };
            Assert.AreEqual("<", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_LessThanOrEqual_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.LessThanOrEqual };
            Assert.AreEqual("<=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_Like_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.Like };
            Assert.AreEqual("LIKE", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_NotLike_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.NotLike };
            Assert.AreEqual("NOT LIKE", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitHistoryQueryParameter_Operator_NotEqual_Test()
        {
            var parameter = new WorkUnitHistoryQueryParameter { Operator = WorkUnitHistoryQueryOperator.NotEqual };
            Assert.AreEqual("!=", parameter.OperatorString);
        }
    }
}
