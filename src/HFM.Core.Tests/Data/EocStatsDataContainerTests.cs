/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.IO;

using NUnit.Framework;
using Rhino.Mocks;

using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.Data
{
    [TestFixture]
    public class EocStatsDataContainerTests
    {
        [Test]
        public void EocStatsDataContainer_Read_FromDisk()
        {
            // Arrange
            var container = new EocStatsDataContainer
            {
                FilePath = Path.Combine("..\\..\\TestFiles", EocStatsDataContainer.DefaultFileName),
            };
            // Act
            container.Read();
            // Assert
            var data = container.Data;
            Assert.IsNotNull(data);
            Assert.AreEqual(new DateTime(634544894826625391), data.LastUpdated);
            Assert.AreEqual(142307, data.UserTwentyFourHourAverage);
            Assert.AreEqual(216422, data.UserPointsToday);
            Assert.AreEqual(298200, data.UserPointsWeek);
            Assert.AreEqual(106207955, data.UserPointsTotal);
            Assert.AreEqual(84390, data.UserWorkUnitsTotal);
            Assert.AreEqual(3975, data.UserPointsUpdate);
            Assert.AreEqual(9, data.UserTeamRank);
            Assert.AreEqual(109, data.UserOverallRank);
            Assert.AreEqual(0, data.UserChangeRankTwentyFourHours);
            Assert.AreEqual(0, data.UserChangeRankSevenDays);
            Assert.AreEqual(5384879, data.TeamTwentyFourHourAverage);
            Assert.AreEqual(5018383, data.TeamPointsToday);
            Assert.AreEqual(10231667, data.TeamPointsWeek);
            Assert.AreEqual(4596308949, data.TeamPointsTotal);
            Assert.AreEqual(9348380, data.TeamWorkUnitsTotal);
            Assert.AreEqual(1110543, data.TeamPointsUpdate);
            Assert.AreEqual(4, data.TeamRank);
            Assert.AreEqual(0, data.TeamChangeRankTwentyFourHours);
            Assert.AreEqual(0, data.TeamChangeRankSevenDays);
            // not serialized
            Assert.IsNull(data.Status);
        }

        [Test]
        public void EocStatsDataContainer_Write_ToDisk()
        {
            // Arrange
            // TODO: Implement ArtifactFolder
            var container = new EocStatsDataContainer
            {
                FilePath = "TestUserStatsBinary.dat", Data = CreateTestStatsData(),
            };
            // Act
            container.Write();
            // Assert
            // clear the data and read it
            container.Data = null;
            container.Read();
            ValidateTestStatsData(container.Data);
        }

        private static EocStatsData CreateTestStatsData()
        {
            var data = new EocStatsData();
            data.LastUpdated = new DateTime(2020, 1, 1);
            data.UserTwentyFourHourAverage = 1;
            data.UserPointsToday = 2;
            data.UserPointsWeek = 3;
            data.UserPointsTotal = 4;
            data.UserWorkUnitsTotal = 5;
            data.UserPointsUpdate = 6;
            data.UserTeamRank = 7;
            data.UserOverallRank = 8;
            data.UserChangeRankTwentyFourHours = 9;
            data.UserChangeRankSevenDays = 10;
            data.TeamTwentyFourHourAverage = 11;
            data.TeamPointsToday = 12;
            data.TeamPointsWeek = 13;
            data.TeamPointsTotal = 14;
            data.TeamWorkUnitsTotal = 15;
            data.TeamPointsUpdate = 16;
            data.TeamRank = 17;
            data.TeamChangeRankTwentyFourHours = 18;
            data.TeamChangeRankSevenDays = 19;
            data.Status = "foo";
            return data;
        }

        private static void ValidateTestStatsData(EocStatsData data)
        {
            Assert.AreEqual(new DateTime(2020, 1, 1), data.LastUpdated);
            Assert.AreEqual(1, data.UserTwentyFourHourAverage);
            Assert.AreEqual(2, data.UserPointsToday);
            Assert.AreEqual(3, data.UserPointsWeek);
            Assert.AreEqual(4, data.UserPointsTotal);
            Assert.AreEqual(5, data.UserWorkUnitsTotal);
            Assert.AreEqual(6, data.UserPointsUpdate);
            Assert.AreEqual(7, data.UserTeamRank);
            Assert.AreEqual(8, data.UserOverallRank);
            Assert.AreEqual(9, data.UserChangeRankTwentyFourHours);
            Assert.AreEqual(10, data.UserChangeRankSevenDays);
            Assert.AreEqual(11, data.TeamTwentyFourHourAverage);
            Assert.AreEqual(12, data.TeamPointsToday);
            Assert.AreEqual(13, data.TeamPointsWeek);
            Assert.AreEqual(14, data.TeamPointsTotal);
            Assert.AreEqual(15, data.TeamWorkUnitsTotal);
            Assert.AreEqual(16, data.TeamPointsUpdate);
            Assert.AreEqual(17, data.TeamRank);
            Assert.AreEqual(18, data.TeamChangeRankTwentyFourHours);
            Assert.AreEqual(19, data.TeamChangeRankSevenDays);
            // not serialized
            Assert.IsNull(data.Status);
        }
    }
}
