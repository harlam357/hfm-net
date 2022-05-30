using System.Runtime.Serialization;

namespace HFM.Core.UserStats;

[DataContract]
public record UserStatsData
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
}
