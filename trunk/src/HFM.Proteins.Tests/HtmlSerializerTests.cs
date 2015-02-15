/*
 * HFM.NET - HTML Serializer Tests
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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

using System.Linq;

using NUnit.Framework;

namespace HFM.Proteins.Tests
{
   [TestFixture]   
   public class HtmlSerializerTests
   {
      [Test]
      public void Deserialize_Test1()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummary.html").ToList();
         Assert.AreEqual(696, proteins.Count);
      }

      [Test]
      public void Deserialize_Test2()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryB.html").ToList();
         Assert.AreEqual(196, proteins.Count);
      }

      [Test]
      public void Deserialize_Test3()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryC.html").ToList();
         Assert.AreEqual(712, proteins.Count);
      }

      [Test]
      public void Deserialize_Test4()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryNew.html").ToList();
         Assert.AreEqual(546, proteins.Count);
      }

      [Test]
      public void Deserialize_Test5()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryNewB.html").ToList();
         Assert.AreEqual(315, proteins.Count);
      }

      [Test]
      public void Deserialize_Test6()
      {
         var serializer = new HtmlSerializer();
         var proteins = serializer.Deserialize("..\\..\\TestFiles\\psummaryNewC.html").ToList();
         Assert.AreEqual(600, proteins.Count);
      }
   }
}
