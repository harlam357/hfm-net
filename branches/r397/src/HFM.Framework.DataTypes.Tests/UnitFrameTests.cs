/*
 * HFM.NET - Unit Frame Class Tests
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;

using NUnit.Framework;

namespace HFM.Framework.DataTypes.Tests
{
   [TestFixture]
   public class UnitFrameTests
   {
      [Test]
      public void HashCodeTest1()
      {
         var frame = new UnitFrame();
         Assert.AreEqual(0, frame.GetHashCode());
      }

      [Test]
      public void HashCodeTest2()
      {
         var frame = new UnitFrame { FrameID = 89, TimeOfFrame = TimeSpan.FromHours(5), FrameDuration = TimeSpan.FromMinutes(4) };
         Assert.AreEqual(1742409840, frame.GetHashCode());
      }

      [Test]
      public void ComparisonTest1()
      {
         var frame1 = new UnitFrame();
         var frame2 = new UnitFrame();
         // calls Object.Equals() override
         Assert.AreEqual(frame1, frame2);
      }

      [Test]
      public void ComparisonTest2()
      {
         var frame1 = new UnitFrame();
         var frame2 = new object();
         // calls Object.Equals() override
         Assert.AreNotEqual(frame1, frame2);
      }

      [Test]
      public void ComparisonTest3()
      {
         var frame1 = new UnitFrame();
         var frame2 = new UnitFrame();
         Assert.IsTrue(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) == 0);
      }

      [Test]
      public void ComparisonTest4()
      {
         var frame1 = new UnitFrame { FrameID = 1 };
         var frame2 = new UnitFrame();
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) > 0);
      }

      [Test]
      public void ComparisonTest5()
      {
         var frame1 = new UnitFrame();
         var frame2 = new UnitFrame { FrameID = 1 };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) < 0);
      }

      [Test]
      public void ComparisonTest6()
      {
         var frame1 = new UnitFrame { FrameID = 1, TimeOfFrame = TimeSpan.FromMinutes(5), FrameDuration = TimeSpan.FromMinutes(5) };
         var frame2 = new UnitFrame { FrameID = 1, TimeOfFrame = TimeSpan.FromMinutes(5), FrameDuration = TimeSpan.FromMinutes(5) };
         Assert.IsTrue(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) == 0);
      }

      [Test]
      public void ComparisonTest7()
      {
         var frame1 = new UnitFrame { FrameID = 1 };
         var frame2 = new UnitFrame { FrameID = 2 };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) < 0);
      }

      [Test]
      public void ComparisonTest8()
      {
         var frame1 = new UnitFrame { FrameID = 2 };
         var frame2 = new UnitFrame { FrameID = 1 };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) > 0);
      }

      [Test]
      public void ComparisonTest9()
      {
         var frame1 = new UnitFrame { TimeOfFrame = TimeSpan.FromMinutes(1) };
         var frame2 = new UnitFrame { TimeOfFrame = TimeSpan.FromMinutes(2) };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) < 0);
      }

      [Test]
      public void ComparisonTest10()
      {
         var frame1 = new UnitFrame { TimeOfFrame = TimeSpan.FromMinutes(2) };
         var frame2 = new UnitFrame { TimeOfFrame = TimeSpan.FromMinutes(1) };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) > 0);
      }

      [Test]
      public void ComparisonTest11()
      {
         var frame1 = new UnitFrame { FrameDuration = TimeSpan.FromMinutes(1) };
         var frame2 = new UnitFrame { FrameDuration = TimeSpan.FromMinutes(2) };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) < 0);
      }

      [Test]
      public void ComparisonTest12()
      {
         var frame1 = new UnitFrame { FrameDuration = TimeSpan.FromMinutes(2) };
         var frame2 = new UnitFrame { FrameDuration = TimeSpan.FromMinutes(1) };
         Assert.IsFalse(frame1.Equals(frame2));
         Assert.IsTrue(frame1.CompareTo(frame2) > 0);
      }

      [Test]
      public void ComparisonTest13()
      {
         var frame1 = new UnitFrame();
         Assert.IsFalse(frame1.Equals(null));
         Assert.IsTrue(frame1.CompareTo(null) > 0);
      }

      [Test]
      public void ComparisonTest14()
      {
         var frame1 = new UnitFrame();
         var frame2 = new UnitFrame();
         Assert.IsTrue(frame1 == frame2);
         Assert.IsFalse(frame1 != frame2);
         Assert.IsFalse(frame1 < frame2);
         Assert.IsFalse(frame1 > frame2);
      }

      [Test]
      public void ComparisonTest15()
      {
         var frame1 = new UnitFrame();
         var frame2 = new UnitFrame { FrameID = 1 };
         Assert.IsFalse(frame1 == frame2);
         Assert.IsTrue(frame1 != frame2);
         Assert.IsTrue(frame1 < frame2);
         Assert.IsFalse(frame1 > frame2);
      }

      [Test]
      public void ComparisonTest16()
      {
         var frame1 = new UnitFrame { FrameID = 1 };
         var frame2 = new UnitFrame();
         Assert.IsFalse(frame1 == frame2);
         Assert.IsTrue(frame1 != frame2);
         Assert.IsFalse(frame1 < frame2);
         Assert.IsTrue(frame1 > frame2);
      }

      [Test]
      public void ComparisonTest17()
      {
         var frame1 = new UnitFrame();
         Assert.IsFalse(frame1 == null);
         Assert.IsTrue(frame1 != null);
         Assert.IsFalse(frame1 < null);
         Assert.IsTrue(frame1 > null);
      }

      [Test]
      public void ComparisonTest18()
      {
         var frame1 = new UnitFrame();
         Assert.IsFalse(null == frame1);
         Assert.IsTrue(null != frame1);
         Assert.IsTrue(null < frame1);
         Assert.IsFalse(null > frame1);
      }
   }
}
