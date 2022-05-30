using NUnit.Framework;

namespace HFM.Core.ScheduledTasks;

[TestFixture]
public class UserStatsScheduledTaskTests
{
    [Test]
    public void UserStatsScheduledTask_GetNextUpdateTime_WhenDuringStandardTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        // this value has to be in past, a future date will generate a result that equals DateTime.MinValue.
        DateTime lastUpdated = utcNow.Date.Subtract(TimeSpan.FromDays(1));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(0)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(1)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(3)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(2)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(3)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(4)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(6)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(5)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(6)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(7)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(9)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(8)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(9)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(10)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(12)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(11)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(12)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(13)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(15)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(14)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(15)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(16)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(18)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(17)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(18)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(19)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(21)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(20)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(21)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(22)), utcNow, false));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromDays(1)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(23)), utcNow, false));
    }

    [Test]
    public void UserStatsScheduledTask_GetNextUpdateTime_WhenDuringDaylightSavingsTime()
    {
        DateTime utcNow = DateTime.UtcNow;
        // this value has to be in past, a future date will generate a result that equals DateTime.MinValue.
        DateTime lastUpdated = utcNow.Date.Subtract(TimeSpan.FromDays(1));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(2)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(0)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(2)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(1)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(2)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(3)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(5)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(4)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(5)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(6)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(8)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(7)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(8)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(9)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(11)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(10)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(11)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(12)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(14)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(13)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(14)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(15)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(17)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(16)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(17)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(18)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(20)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(19)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(20)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(21)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(TimeSpan.FromHours(23)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(22)), utcNow, true));

        Assert.AreEqual(lastUpdated.Add(new TimeSpan(1, 2, 0, 0)),
            UserStatsScheduledTask.GetNextUpdateTime(lastUpdated.Add(TimeSpan.FromHours(23)), utcNow, true));
    }

    [Test]
    public void UserStatsScheduledTask_GetNextUpdateTime_ReturnsGivenUtcNowValueWhenLastUpdatedValueIsMinValue()
    {
        DateTime utcNow = DateTime.UtcNow;

        Assert.AreEqual(utcNow,
            UserStatsScheduledTask.GetNextUpdateTime(DateTime.MinValue, utcNow, false));
    }

    [Test]
    public void UserStatsScheduledTask_GetNextUpdateTime_ReturnsGivenUtcNowValueWhenLastUpdatedValueIsInTheFuture()
    {
        DateTime utcNow = DateTime.UtcNow;

        Assert.AreEqual(utcNow,
            UserStatsScheduledTask.GetNextUpdateTime(DateTime.MaxValue, utcNow, false));
    }
}
