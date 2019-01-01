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
         Assert.IsNull(unitInfo.FrameData);
         Assert.AreEqual(Constants.DefaultCoreID, unitInfo.CoreID);
      }

      [Test]
      public void UnitInfo_CurrentFrame_DefaultValueIsNull_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         // Act & Assert
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_IsSourcedFromFrameDataDictionary_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = 0 });
         unitInfo.FrameData = frameDataDictionary;
         // Act & Assert
         Assert.AreSame(frameDataDictionary[0], unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_IsSourcedFromFrameDataWithGreatestID_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>()
            .With(new WorkUnitFrameData { ID = 0 }, new WorkUnitFrameData { ID = 1 }, new WorkUnitFrameData { ID = 5 });
         unitInfo.FrameData = frameDataDictionary;
         // Act & Assert
         Assert.AreSame(frameDataDictionary[5], unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_CurrentFrame_ReturnsNullIfMaximumFrameDataIdIsNotZeroOrGreater_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = -1 });
         unitInfo.FrameData = frameDataDictionary;
         // Act & Assert
         Assert.IsNull(unitInfo.CurrentFrame);
      }

      [Test]
      public void UnitInfo_GetFrameData_ReturnsNullIfRequestedIdDoesNotExist_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         // Act & Assert
         Assert.IsNull(unitInfo.GetFrameData(0));
      }

      [Test]
      public void UnitInfo_GetFrameData_ReturnsObjectIfRequestedIdExists_Test()
      {
         // Arrange
         var unitInfo = new UnitInfo();
         var frameDataDictionary = new Dictionary<int, WorkUnitFrameData>().With(new WorkUnitFrameData { ID = 0 });
         unitInfo.FrameData = frameDataDictionary;
         // Act & Assert
         Assert.IsNotNull(unitInfo.GetFrameData(0));
      }
   }
}
