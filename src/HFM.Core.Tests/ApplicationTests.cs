/*
 * HFM.NET - Application Class Tests
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

namespace HFM.Core.Tests
{
   [TestFixture]
   public class ApplicationTests
   {
      [Test]
      public void Application_ParseVersion_Test1()
      {
         Assert.AreEqual(1020030004, Application.ParseVersion("1.2.3.4"));
      }

      [Test]
      public void Application_ParseVersion_Test2()
      {
         Assert.Throws<ArgumentNullException>(() => Application.ParseVersion(null));
      }

      [Test]
      public void Application_ParseVersion_Test3()
      {
         Assert.AreEqual(1020030000, Application.ParseVersion("1.2.3"));
      }

      [Test]
      public void Application_ParseVersion_Test4()
      {
         Assert.Throws<FormatException>(() => Application.ParseVersion("1.2.3.b"));
      }
   }
}
