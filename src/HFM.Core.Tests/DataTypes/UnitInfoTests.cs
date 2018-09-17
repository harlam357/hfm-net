/*
 * HFM.NET - Unit Info Class Tests
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
using System.Collections.Generic;

using HFM.Log;

using NUnit.Framework;

namespace HFM.Core.DataTypes
{
   [TestFixture]
   public class UnitInfoTests
   {
      [Test]
      public void UnitInfo_DefaultPropertyValues_Test()
      {
         var unitInfo = new UnitInfo();
         Assert.AreEqual(SlotType.Unknown, unitInfo.SlotType);
         Assert.IsTrue(unitInfo.DownloadTime.IsUnknown());
         Assert.IsTrue(unitInfo.DueTime.IsUnknown());
         Assert.IsTrue(unitInfo.UnitStartTimeStamp == TimeSpan.Zero);
         Assert.IsTrue(unitInfo.FinishedTime.IsUnknown());
         Assert.IsTrue(unitInfo.CoreVersion == 0);
         Assert.IsTrue(unitInfo.ProteinName.Length == 0);
         Assert.IsTrue(unitInfo.ProteinTag.Length == 0);
         Assert.AreEqual(WorkUnitResult.Unknown, unitInfo.UnitResult);
         Assert.IsNull(unitInfo.LogLines);
         Assert.IsNull(unitInfo.UnitFrames);
         Assert.AreEqual(Constants.DefaultCoreID, unitInfo.CoreID);
      }

      [Test]
      public void UnitInfo_CurrentFrame_Test1()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         // Act & Assert
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_Test2()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame { ID = 0 };
         var logLine = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame };
         unitInfo.LogLines = new List<LogLine>(new[] { logLine });
         // Act & Assert
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_Test3()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var logLine0 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = new UnitFrame { ID = 0 } };
         var logLine1 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = new UnitFrame { ID = 1 } };
         var unitFrame5 = new UnitFrame { ID = 5 };
         var logLine5 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame5 };
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0, logLine1, logLine5 });
         // Act & Assert
         Assert.AreSame(unitFrame5, unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_Test4()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame { ID = -1 };
         var logLine = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame };
         unitInfo.LogLines = new List<LogLine>(new[] { logLine });
         // Act & Assert
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_SetUnitFrameDataFromLogLines_Test1()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var unitFrame = new UnitFrame
         {
            RawFramesComplete = 0,
            RawFramesTotal = 100000,
            ID = 0
         };
         var logLine = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine });
         // Assert
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(1, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame, unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_SetUnitFrameDataFromLogLines_Test2()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var unitFrame0 = new UnitFrame
         {
            RawFramesComplete = 0,
            RawFramesTotal = 100000,
            ID = 0
         };
         var logLine0 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame0 };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0 });
         // Assert 0
         Assert.AreEqual(0, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(1, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame0, unitInfo.CurrentFrame);
         // no duration - first frame
         Assert.AreEqual(new TimeSpan(0, 0, 0), unitInfo.CurrentFrame.Duration);

         var unitFrame1 = new UnitFrame
         {
            RawFramesComplete = 1000,
            RawFramesTotal = 100000,
            ID = 1,
            TimeStamp = new TimeSpan(0, 5, 0)
         };
         var logLine1 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame1 };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0, logLine1 });
         // Assert 1
         Assert.AreEqual(1000, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(2, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame1, unitInfo.CurrentFrame);
         Assert.AreEqual(new TimeSpan(0, 5, 0), unitInfo.CurrentFrame.Duration);

         var unitFrame2 = new UnitFrame
         {
            RawFramesComplete = 2000,
            RawFramesTotal = 100000,
            ID = 2,
            TimeStamp = new TimeSpan(0, 10, 0)
         };
         var logLine2 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame2 };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0, logLine1, logLine2 });
         // Assert 2
         Assert.AreEqual(2000, unitInfo.RawFramesComplete);
         Assert.AreEqual(100000, unitInfo.RawFramesTotal);
         Assert.AreEqual(3, unitInfo.UnitFrames.Count);
         Assert.AreSame(unitFrame2, unitInfo.CurrentFrame);
         Assert.AreEqual(new TimeSpan(0, 5, 0), unitInfo.CurrentFrame.Duration);
      }

      [Test]
      public void UnitInfo_SetUnitFrameDataFromLogLines_Test3()
      {
         // this tests GetDelta() rollover functionality

         // Arrange
         var unitInfo = new UnitInfo();
         var unitFrame0 = new UnitFrame
         {
            RawFramesComplete = 0,
            RawFramesTotal = 100000,
            ID = 0,
            TimeStamp = new TimeSpan(23, 55, 0),
            Duration = new TimeSpan(0, 0, 0)
         };
         var logLine0 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame0 };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0 });
         // Assert 0 - no duration - first frame
         Assert.AreEqual(new TimeSpan(0, 0, 0), unitInfo.CurrentFrame.Duration);

         var unitFrame1 = new UnitFrame
         {
            RawFramesComplete = 1000,
            RawFramesTotal = 100000,
            ID = 1,
            TimeStamp = new TimeSpan(0, 5, 0),
            Duration = new TimeSpan(0, 0, 0)
         };
         var logLine1 = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = unitFrame1 };
         // Act
         unitInfo.LogLines = new List<LogLine>(new[] { logLine0, logLine1 });
         // Assert 1 - now we get a frame duration
         Assert.AreEqual(new TimeSpan(0, 10, 0), unitInfo.CurrentFrame.Duration);
      }

      [Test]
      public void UnitInfo_GetUnitFrame_Test1()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         // Act & Assert
         Assert.IsNull(unitInfo.GetUnitFrame(0));
      }

      [Test]
      public void UnitInfo_GetUnitFrame_Test2()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var logLine = new LogLine { LineType = LogLineType.WorkUnitFrame, Data = new UnitFrame { ID = 0 } };
         unitInfo.LogLines = new List<LogLine>(new[] { logLine });
         // Act & Assert
         Assert.IsNotNull(unitInfo.GetUnitFrame(0));
         Assert.IsNull(unitInfo.GetUnitFrame(1));
      }
   }
}
