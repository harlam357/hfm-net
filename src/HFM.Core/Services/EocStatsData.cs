using System.Runtime.Serialization;

namespace HFM.Core.Services;

[DataContract]
public record EocStatsData
{
    [DataMember(Order = 1)]
    public DateTime LastUpdated { get; set; }

    [DataMember(Order = 2)]
    public long UserTwentyFourHourAverage { get; set; }

    [DataMember(Order = 3)]
    public long UserPointsToday { get; set; }

    [DataMember(Order = 4)]
    public long UserPointsWeek { get; set; }

    [DataMember(Order = 5)]
    public long UserPointsTotal { get; set; }

    [DataMember(Order = 6)]
    public long UserWorkUnitsTotal { get; set; }

    [DataMember(Order = 7)]
    public long UserPointsUpdate { get; set; }

    [DataMember(Order = 8)]
    public int UserTeamRank { get; set; }

    [DataMember(Order = 9)]
    public int UserOverallRank { get; set; }

    [DataMember(Order = 10)]
    public int UserChangeRankTwentyFourHours { get; set; }

    [DataMember(Order = 11)]
    public int UserChangeRankSevenDays { get; set; }

    [DataMember(Order = 12)]
    public long TeamTwentyFourHourAverage { get; set; }

    [DataMember(Order = 13)]
    public long TeamPointsToday { get; set; }

    [DataMember(Order = 14)]
    public long TeamPointsWeek { get; set; }

    [DataMember(Order = 15)]
    public long TeamPointsTotal { get; set; }

    [DataMember(Order = 16)]
    public long TeamWorkUnitsTotal { get; set; }

    [DataMember(Order = 17)]
    public long TeamPointsUpdate { get; set; }

    [DataMember(Order = 18)]
    public int TeamRank { get; set; }

    [DataMember(Order = 19)]
    public int TeamChangeRankTwentyFourHours { get; set; }

    [DataMember(Order = 20)]
    public int TeamChangeRankSevenDays { get; set; }

    // not serialized
    public string Status { get; set; }

    /// <summary>
    /// Gets the time for the next stats update time in UTC.
    /// </summary>
    public static DateTime GetNextUpdateTime(DateTime lastUpdated, DateTime utcNow, bool isDaylightSavingsTime)
    {
        // if last updated is either MinValue (no value) or is in the future,
        // update to either set a value or correct a bad (future) value
        if (lastUpdated == DateTime.MinValue || lastUpdated > utcNow)
        {
            return utcNow;
        }

        // What I really need to know is if it is Daylight Savings Time
        // in the Central Time Zone, not the local machines Time Zone.
        int offset = 0;
        if (isDaylightSavingsTime)
        {
            offset = 1;
        }

        DateTime nextUpdateTime = lastUpdated.Date;

        int hours = 24;
        for (int i = 0; i < 9; i++)
        {
            if (lastUpdated.TimeOfDay >= TimeSpan.FromHours(hours - offset))
            {
                nextUpdateTime = nextUpdateTime.Add(TimeSpan.FromHours(hours + 3 - offset));
                break;
            }

            hours -= 3;
        }

        return nextUpdateTime;
    }
}
