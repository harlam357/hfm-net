
using NUnit.Framework;

namespace HFM.Core.UserStats;

[TestFixture]
public class UserStatsDataContainerTests
{
    [Test]
    public void UserStatsDataContainer_Read_FromDisk()
    {
        // Arrange
        var container = new UserStatsDataContainer
        {
            FilePath = Path.Combine("TestFiles", UserStatsDataContainer.DefaultFileName),
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
    public void UserStatsDataContainer_Write_ToDisk()
    {
        // Arrange
        using (var artifacts = new ArtifactFolder())
        {
            var container = new UserStatsDataContainer
            {
                FilePath = Path.Combine(artifacts.Path, "TestUserStatsBinary.dat"),
                Data = CreateTestStatsData()
            };
            // Act
            container.Write();
            // Assert
            // clear the data and read it
            container.Data = null;
            container.Read();
            ValidateTestStatsData(container.Data);
        }
    }

    private static UserStatsData CreateTestStatsData()
    {
        var data = new UserStatsData
        {
            LastUpdated = new DateTime(2020, 1, 1),
            UserTwentyFourHourAverage = 1,
            UserPointsToday = 2,
            UserPointsWeek = 3,
            UserPointsTotal = 4,
            UserWorkUnitsTotal = 5,
            UserPointsUpdate = 6,
            UserTeamRank = 7,
            UserOverallRank = 8,
            UserChangeRankTwentyFourHours = 9,
            UserChangeRankSevenDays = 10,
            TeamTwentyFourHourAverage = 11,
            TeamPointsToday = 12,
            TeamPointsWeek = 13,
            TeamPointsTotal = 14,
            TeamWorkUnitsTotal = 15,
            TeamPointsUpdate = 16,
            TeamRank = 17,
            TeamChangeRankTwentyFourHours = 18,
            TeamChangeRankSevenDays = 19,
            Status = "foo"
        };
        return data;
    }

    private static void ValidateTestStatsData(UserStatsData data)
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
