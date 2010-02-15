/*
 * HFM.NET - Unit Info Class Tests
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Framework;

namespace HFM.Instances.Tests
{
   [TestFixture]
   public class UnitInfoTests
   {
      [Test]
      public void UnitInfoPropertyTest()
      {
         UnitInfo unitInfo = new UnitInfo();
         
         Assert.AreEqual(DateTime.MinValue, unitInfo.DownloadTime);
         Assert.IsTrue(unitInfo.DownloadTimeUnknown);
         unitInfo.DownloadTime = DateTime.Now;
         Assert.IsFalse(unitInfo.DownloadTimeUnknown);

         Assert.AreEqual(DateTime.MinValue, unitInfo.DueTime);
         Assert.IsTrue(unitInfo.DueTimeUnknown);
         unitInfo.DueTime = DateTime.Now;
         Assert.IsFalse(unitInfo.DueTimeUnknown);

         Assert.AreEqual(0, unitInfo.ProjectID);
         Assert.AreEqual(0, unitInfo.ProjectRun);
         Assert.AreEqual(0, unitInfo.ProjectClone);
         Assert.AreEqual(0, unitInfo.ProjectGen);
         Assert.IsTrue(unitInfo.ProjectIsUnknown);

         unitInfo.ProjectID = 2677;
         unitInfo.ProjectRun = 14;
         unitInfo.ProjectClone = 69;
         unitInfo.ProjectGen = 39;
         Assert.IsFalse(unitInfo.ProjectIsUnknown);
      }

      [Test]
      public void UnitInfoSetCurrentFrameTest_1()
      {
         DoUnitInfoSetCurrentFrameTest("00:05:12", "00:13:34", new TimeSpan(0, 8, 22));
      }

      [Test]
      public void UnitInfoSetCurrentFrameTest_2()
      {
         DoUnitInfoSetCurrentFrameTest("23:45:12", "00:03:54", new TimeSpan(0, 18, 42));
      }

      public void DoUnitInfoSetCurrentFrameTest(string TimeStampString1, string TimeStampString2, TimeSpan expectedDuration)
      {
         UnitInfo unitInfo = new UnitInfo();
         
         MockRepository mocks = new MockRepository();
         
         ILogLine line1 = mocks.DynamicMock<ILogLine>();
         ILogLine line2 = mocks.DynamicMock<ILogLine>();
         IFrameData frame1 = mocks.DynamicMock<IFrameData>();
         IFrameData frame2 = mocks.DynamicMock<IFrameData>();
         
         Expect.Call(line1.LineType).Return(LogLineType.WorkUnitFrame);
         Expect.Call(line2.LineType).Return(LogLineType.WorkUnitFrame);
         Expect.Call(line1.LineData).Return(frame1);
         Expect.Call(line2.LineData).Return(frame2);
         Expect.Call(frame1.FrameID).Return(0).Repeat.Any();
         Expect.Call(frame2.FrameID).Return(1).Repeat.Any();
         Expect.Call(frame1.TimeStampString).Return(TimeStampString1);
         Expect.Call(frame2.TimeStampString).Return(TimeStampString2);
         Expect.Call(frame1.RawFramesComplete).Return(0);
         Expect.Call(frame2.RawFramesComplete).Return(2500);
         Expect.Call(frame1.RawFramesTotal).Return(250000);
         Expect.Call(frame2.RawFramesTotal).Return(250000);
         
         mocks.ReplayAll();

         // This property is set before setting unit frames
         // it will already contain the total number of
         // observed 'WorkUnitFrame' log lines.
         unitInfo.FramesObserved = 2;
         
         Assert.IsNull(unitInfo.CurrentFrame);
         Assert.AreEqual(null, unitInfo.GetUnitFrame(0));
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(0, unitInfo.RawFramesTotal);
         
         unitInfo.SetCurrentFrame(line1);
         Assert.IsNotNull(unitInfo.CurrentFrame);
         Assert.AreEqual(TimeSpan.Zero, unitInfo.GetUnitFrame(0).FrameDuration);
         Assert.AreEqual(null, unitInfo.GetUnitFrame(1));
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(250000, unitInfo.RawFramesTotal);
         
         unitInfo.SetCurrentFrame(line2);
         Assert.AreEqual(TimeSpan.Zero, unitInfo.GetUnitFrame(0).FrameDuration);
         Assert.AreEqual(expectedDuration, unitInfo.GetUnitFrame(1).FrameDuration);
         Assert.AreEqual(null, unitInfo.GetUnitFrame(2));
         Assert.AreEqual(2500, unitInfo.RawFramesComplete);
         Assert.AreEqual(250000, unitInfo.RawFramesTotal);
         
         mocks.VerifyAll();
      }
   }
}
