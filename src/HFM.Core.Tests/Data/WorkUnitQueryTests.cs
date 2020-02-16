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
    public class WorkUnitQueryTests
    {
        [Test]
        public void WorkUnitQuery_Create_Test()
        {
            var query = new WorkUnitQuery();
            Assert.AreEqual(WorkUnitQuery.SelectAll.Name, query.Name);
            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void WorkUnitQuery_DeepClone_Test()
        {
            var query = new WorkUnitQuery("Test")
                .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, "Test Instance")
                .AddParameter(WorkUnitRowColumn.DownloadDateTime, WorkUnitQueryOperator.GreaterThan, new DateTime(2000, 1, 1));

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
        public void WorkUnitQuery_Compare_Test1()
        {
            var query1 = new WorkUnitQuery();
            var query2 = new WorkUnitQuery();
            Assert.AreEqual(0, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test2()
        {
            var query1 = new WorkUnitQuery { Name = "Name" };
            var query2 = new WorkUnitQuery();
            Assert.AreEqual(1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test3()
        {
            var query1 = new WorkUnitQuery();
            var query2 = new WorkUnitQuery { Name = "Name" };
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test4()
        {
            var query1 = new WorkUnitQuery { Name = null };
            var query2 = new WorkUnitQuery();
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test5()
        {
            var query1 = new WorkUnitQuery { Name = null };
            var query2 = new WorkUnitQuery { Name = null };
            Assert.AreEqual(0, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test6()
        {
            var query1 = new WorkUnitQuery { Name = "A" };
            var query2 = new WorkUnitQuery { Name = "B" };
            Assert.AreEqual(-1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test7()
        {
            var query1 = new WorkUnitQuery { Name = "B" };
            var query2 = new WorkUnitQuery { Name = "A" };
            Assert.AreEqual(1, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_Test8()
        {
            var query1 = new WorkUnitQuery();
            Assert.AreEqual(1, query1.CompareTo(null));
        }

        [Test]
        public void WorkUnitQueryParameter_Create_Test()
        {
            var parameter = new WorkUnitQueryParameter();
            Assert.AreEqual(WorkUnitRowColumn.ProjectID, parameter.Column);
            Assert.AreEqual(WorkUnitQueryOperator.Equal, parameter.Operator);
        }

        [Test]
        public void WorkUnitQueryParameter_Value_Test1()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = "Value";
            Assert.AreEqual("Value", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Value_Test2()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = new DateTime(2000, 1, 1);
            Assert.AreEqual(new DateTime(2000, 1, 1), parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Value_Test3()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = 6900;
            Assert.AreEqual("6900", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_Default_Test()
        {
            var parameter = new WorkUnitQueryParameter();
            Assert.AreEqual("=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_Equal_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.Equal };
            Assert.AreEqual("=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_GreaterThan_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.GreaterThan };
            Assert.AreEqual(">", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_GreaterThanOrEqual_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.GreaterThanOrEqual };
            Assert.AreEqual(">=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_LessThan_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.LessThan };
            Assert.AreEqual("<", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_LessThanOrEqual_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.LessThanOrEqual };
            Assert.AreEqual("<=", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_Like_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.Like };
            Assert.AreEqual("LIKE", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_NotLike_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.NotLike };
            Assert.AreEqual("NOT LIKE", parameter.OperatorString);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_NotEqual_Test()
        {
            var parameter = new WorkUnitQueryParameter { Operator = WorkUnitQueryOperator.NotEqual };
            Assert.AreEqual("!=", parameter.OperatorString);
        }
    }
}
