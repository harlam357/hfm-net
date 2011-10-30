/*
 * HFM.NET - Unit Info Class Tests
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

namespace HFM.Core.DataTypes.Tests
{
   [TestFixture]
   public class UnitInfoTests
   {
      [Test]
      public void InitTest()
      {
         var unitInfo = new UnitInfo();
         Assert.AreEqual(SlotType.Unknown, unitInfo.SlotType);
         Assert.IsTrue(unitInfo.DownloadTime.IsUnknown());
         Assert.IsTrue(unitInfo.DueTime.IsUnknown());
         Assert.IsTrue(unitInfo.UnitStartTimeStamp.IsZero());
         Assert.IsTrue(unitInfo.FinishedTime.IsUnknown());
         Assert.IsTrue(unitInfo.CoreVersion.Length == 0);
         Assert.IsTrue(unitInfo.ProteinName.Length == 0);
         Assert.IsTrue(unitInfo.ProteinTag.Length == 0);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfo.UnitResult);
         Assert.IsTrue(unitInfo.UnitFrames.Count == 0);
         Assert.AreEqual(Default.CoreID, unitInfo.CoreID);
      }
      
      [Test]
      public void CurrentFrameTest1()
      {
         var unitInfo = new UnitInfo();
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void CurrentFrameTest2()
      {
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame { FrameID = 0 };
         unitInfo.UnitFrames.Add(unitFrame.FrameID, unitFrame);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
      }

      [Test]
      public void CurrentFrameTest3()
      {
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame { FrameID = 0 };
         unitInfo.UnitFrames.Add(unitFrame.FrameID, unitFrame);
         unitFrame = new UnitFrame { FrameID = 1 };
         unitInfo.UnitFrames.Add(unitFrame.FrameID, unitFrame);
         unitFrame = new UnitFrame { FrameID = 5 };
         unitInfo.UnitFrames.Add(unitFrame.FrameID, unitFrame);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
      }

      [Test]
      public void CurrentFrameTest4()
      {
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame { FrameID = -1 };
         unitInfo.UnitFrames.Add(unitFrame.FrameID, unitFrame);
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void SetCurrentFrameTest1()
      {
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame
                         {
                            RawFramesComplete = 0,
                            RawFramesTotal = 100000,
                            FrameID = 0,
                            TimeOfFrame = new TimeSpan(0, 0, 0),
                            FrameDuration = new TimeSpan(0, 0, 0)
                         };
         unitInfo.SetCurrentFrame(unitFrame);
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(1, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);

         unitInfo.SetCurrentFrame(unitFrame);
         // still only 1 frame
         Assert.AreEqual(1, unitInfo.UnitFrames.Count);
      }

      [Test]
      public void SetCurrentFrameTest2()
      {
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame
         {
            RawFramesComplete = 0,
            RawFramesTotal = 100000,
            FrameID = 0,
            TimeOfFrame = new TimeSpan(0, 0, 0),
            FrameDuration = new TimeSpan(0, 0, 0)
         };
         unitInfo.SetCurrentFrame(unitFrame);
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(1, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
         // no duration - first frame
         Assert.AreEqual(new TimeSpan(0, 0, 0), unitInfo.CurrentFrame.FrameDuration);

         unitFrame = new UnitFrame
         {
            RawFramesComplete = 1000,
            RawFramesTotal = 100000,
            FrameID = 1,
            TimeOfFrame = new TimeSpan(0, 5, 0),
            FrameDuration = new TimeSpan(0, 0, 0)
         };
         unitInfo.SetCurrentFrame(unitFrame);
         Assert.AreEqual(1000, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(2, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
         // still no duration - unitInfo.FramesObserved must be > 1
         Assert.AreEqual(new TimeSpan(0, 0, 0), unitInfo.CurrentFrame.FrameDuration);

         unitFrame = new UnitFrame
         {
            RawFramesComplete = 2000,
            RawFramesTotal = 100000,
            FrameID = 2,
            TimeOfFrame = new TimeSpan(0, 10, 0),
            FrameDuration = new TimeSpan(0, 0, 0)
         };
         // set observed count
         unitInfo.FramesObserved = 2;
         unitInfo.SetCurrentFrame(unitFrame);
         Assert.AreEqual(2000, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(3, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
         // now we get a frame duration
         Assert.AreEqual(new TimeSpan(0, 5, 0), unitInfo.CurrentFrame.FrameDuration);
      }

      [Test]
      public void SetCurrentFrameTest3()
      {
         // this tests GetDelta() rollover functionality
      
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame
         {
            RawFramesComplete = 0,
            RawFramesTotal = 100000,
            FrameID = 0,
            TimeOfFrame = new TimeSpan(23, 55, 0),
            FrameDuration = new TimeSpan(0, 0, 0)
         };
         unitInfo.SetCurrentFrame(unitFrame);
         // no duration - first frame
         Assert.AreEqual(new TimeSpan(0, 0, 0), unitInfo.CurrentFrame.FrameDuration);

         unitFrame = new UnitFrame
         {
            RawFramesComplete = 1000,
            RawFramesTotal = 100000,
            FrameID = 1,
            TimeOfFrame = new TimeSpan(0, 5, 0),
            FrameDuration = new TimeSpan(0, 0, 0)
         };
         // set observed count
         unitInfo.FramesObserved = 2;
         unitInfo.SetCurrentFrame(unitFrame);
         // now we get a frame duration
         Assert.AreEqual(new TimeSpan(0, 10, 0), unitInfo.CurrentFrame.FrameDuration);
      }
      
      [Test]
      [ExpectedException(typeof(ArgumentNullException))]
      public void SetCurrentFrameTest4()
      {
         var unitInfo = new UnitInfo();
         unitInfo.SetCurrentFrame(null);
      }
      
      [Test]
      public void GetUnitFrameTest1()
      {
         var unitInfo = new UnitInfo();
         Assert.IsNull(unitInfo.GetUnitFrame(0));
      }

      [Test]
      public void GetUnitFrameTest2()
      {
         var unitInfo = new UnitInfo();
         unitInfo.UnitFrames.Add(0, new UnitFrame());
         Assert.IsNotNull(unitInfo.GetUnitFrame(0));
         Assert.IsNull(unitInfo.GetUnitFrame(1));
      }
   }
}
