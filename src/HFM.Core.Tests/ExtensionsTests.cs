/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using Rhino.Mocks;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Core
{
    [TestFixture]
    public class ExtensionsTests
    {
        #region DateTime/TimeSpan

        [Test]
        public void DateTimeIsKnownTest1()
        {
            var dateTime = new DateTime();
            Assert.IsFalse(dateTime.IsKnown());
        }

        [Test]
        public void DateTimeIsKnownTest2()
        {
            var dateTime = DateTime.Now;
            Assert.IsTrue(dateTime.IsKnown());
        }

        [Test]
        public void DateTimeIsUnknownTest1()
        {
            var dateTime = new DateTime();
            Assert.IsTrue(dateTime.IsUnknown());
        }

        [Test]
        public void DateTimeIsUnknownTest2()
        {
            var dateTime = DateTime.Now;
            Assert.IsFalse(dateTime.IsUnknown());
        }

        #endregion
    }
}
