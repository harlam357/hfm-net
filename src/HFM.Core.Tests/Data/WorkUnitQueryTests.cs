﻿/*
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
        public void WorkUnitQuery_DefaultConstructor_HasPropertyValues()
        {
            var query = new WorkUnitQuery();
            Assert.AreEqual(String.Empty, query.Name);
            Assert.AreEqual(0, query.Parameters.Count);
        }

        [Test]
        public void WorkUnitQuery_DeepClone_ReturnsCopyOfQuery()
        {
            // Arrange
            var query = new WorkUnitQuery("Test")
                .AddParameter(WorkUnitRowColumn.Name, WorkUnitQueryOperator.Equal, "Test Instance")
                .AddParameter(WorkUnitRowColumn.DownloadDateTime, WorkUnitQueryOperator.GreaterThan, new DateTime(2000, 1, 1));
            // Act
            var copy = query.DeepClone();
            // Assert
            Assert.AreNotSame(query, copy);
            Assert.AreEqual(query.Name, copy.Name);
            Assert.AreEqual(query.Parameters.Count, copy.Parameters.Count);
            for (int i = 0; i < query.Parameters.Count; i++)
            {
                Assert.AreEqual(query.Parameters[i].Column, copy.Parameters[i].Column);
                Assert.AreEqual(query.Parameters[i].Operator, copy.Parameters[i].Operator);
                Assert.AreEqual(query.Parameters[i].Value, copy.Parameters[i].Value);
            }
        }

        [Test]
        public void WorkUnitQuery_Compare_AreEqual()
        {
            var query1 = new WorkUnitQuery();
            var query2 = new WorkUnitQuery();
            Assert.IsTrue(query1.CompareTo(query2) == 0);
            Assert.IsTrue(query1 == query2);
        }

        [Test]
        public void WorkUnitQuery_Compare_IsGreaterThan()
        {
            var query1 = new WorkUnitQuery { Name = "Name" };
            var query2 = new WorkUnitQuery();
            Assert.IsTrue(query1.CompareTo(query2) > 0);
            Assert.IsTrue(query1 > query2);
        }

        [Test]
        public void WorkUnitQuery_Compare_IsLessThan()
        {
            var query1 = new WorkUnitQuery();
            var query2 = new WorkUnitQuery { Name = "Name" };
            Assert.IsTrue(query1.CompareTo(query2) < 0);
            Assert.IsTrue(query1 < query2);
        }

        [Test]
        public void WorkUnitQuery_Compare_NullAreEqual()
        {
            var query1 = new WorkUnitQuery { Name = null };
            var query2 = new WorkUnitQuery { Name = null };
            Assert.AreEqual(0, query1.CompareTo(query2));
        }

        [Test]
        public void WorkUnitQuery_Compare_IsGreaterThanNull()
        {
            var query1 = new WorkUnitQuery();
            var query2 = new WorkUnitQuery { Name = null };
            Assert.IsTrue(query1.CompareTo(query2) > 0);
            Assert.IsTrue(query1 > query2);
        }

        [Test]
        public void WorkUnitQuery_Compare_NullIsLessThan()
        {
            var query1 = new WorkUnitQuery { Name = null };
            var query2 = new WorkUnitQuery();
            Assert.IsTrue(query1.CompareTo(query2) < 0);
            Assert.IsTrue(query1 < query2);
        }
        
        [Test]
        public void WorkUnitQuery_Compare_AreNotEqual()
        {
            var query1 = new WorkUnitQuery { Name = "A" };
            var query2 = new WorkUnitQuery { Name = "B" };
            Assert.IsTrue(query1.CompareTo(query2) != 0);
            Assert.IsTrue(query1 != query2);
        }

        [Test]
        public void WorkUnitQuery_Compare_IsGreaterThanNullQuery()
        {
            var query = new WorkUnitQuery();
            Assert.IsTrue(query.CompareTo(null) > 0);
            Assert.IsTrue(query > null);
        }

        [Test]
        public void WorkUnitQueryParameter_DefaultConstructor_HasPropertyValues()
        {
            var parameter = new WorkUnitQueryParameter();
            Assert.AreEqual(WorkUnitRowColumn.ProjectID, parameter.Column);
            Assert.AreEqual(WorkUnitQueryOperator.Equal, parameter.Operator);
            Assert.AreEqual("=", parameter.GetOperatorString());
        }

        [Test]
        public void WorkUnitQueryParameter_Value_StringBehavior()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = "Value";
            Assert.AreEqual("Value", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Value_DateTimeBehavior()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = new DateTime(2000, 1, 1);
            Assert.AreEqual(new DateTime(2000, 1, 1), parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Value_IntegerBehavior()
        {
            var parameter = new WorkUnitQueryParameter();
            parameter.Value = 6900;
            Assert.AreEqual("6900", parameter.Value);
            parameter.Value = null;
            Assert.IsNull(parameter.Value);
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_OperatorsHaveStringValue()
        {
            var parameter = new WorkUnitQueryParameter();
            foreach (WorkUnitQueryOperator op in Enum.GetValues(typeof(WorkUnitQueryOperator)))
            {
                parameter.Operator = op;
                Console.WriteLine(parameter.GetOperatorString());
            }
        }

        [Test]
        public void WorkUnitQueryParameter_Operator_UnknownOperatorsThrowExceptionOnStringValue()
        {
            var parameter = new WorkUnitQueryParameter { Operator = (WorkUnitQueryOperator)Int32.MaxValue };
            Assert.Throws<InvalidOperationException>(() => parameter.GetOperatorString());
        }
    }
}