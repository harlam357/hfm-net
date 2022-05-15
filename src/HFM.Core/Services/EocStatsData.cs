using System.Runtime.Serialization;

namespace HFM.Core.Services;

[DataContract]
public class EocStatsData : IEquatable<EocStatsData>
{
    public EocStatsData()
    {

    }

    public EocStatsData(DateTime lastUpdated)
    {
        LastUpdated = lastUpdated;
    }

    /// <summary>
    /// Stats Last Updated
    /// </summary>
    [DataMember(Order = 1)]
    public DateTime LastUpdated { get; set; }

    /// <summary>
    /// User 24 Hour Points Average
    /// </summary>
    [DataMember(Order = 2)]
    public long UserTwentyFourHourAverage { get; set; }

    /// <summary>
    /// User Points Today
    /// </summary>
    [DataMember(Order = 3)]
    public long UserPointsToday { get; set; }

    /// <summary>
    /// User Points Week
    /// </summary>
    [DataMember(Order = 4)]
    public long UserPointsWeek { get; set; }

    /// <summary>
    /// User Points Total
    /// </summary>
    [DataMember(Order = 5)]
    public long UserPointsTotal { get; set; }

    /// <summary>
    /// User Work Units Total
    /// </summary>
    [DataMember(Order = 6)]
    public long UserWorkUnitsTotal { get; set; }

    /// <summary>
    /// User Points Update
    /// </summary>
    [DataMember(Order = 7)]
    public long UserPointsUpdate { get; set; }

    /// <summary>
    /// User Team Rank
    /// </summary>
    [DataMember(Order = 8)]
    public int UserTeamRank { get; set; }

    /// <summary>
    /// User Overall Rank
    /// </summary>
    [DataMember(Order = 9)]
    public int UserOverallRank { get; set; }

    /// <summary>
    /// User Change Rank Twenty Four Hours
    /// </summary>
    [DataMember(Order = 10)]
    public int UserChangeRankTwentyFourHours { get; set; }

    /// <summary>
    /// User Change Rank Twenty Four Hours
    /// </summary>
    [DataMember(Order = 11)]
    public int UserChangeRankSevenDays { get; set; }

    /// <summary>
    /// Team 24 Hour Points Average
    /// </summary>
    [DataMember(Order = 12)]
    public long TeamTwentyFourHourAverage { get; set; }

    /// <summary>
    /// Team Points Today
    /// </summary>
    [DataMember(Order = 13)]
    public long TeamPointsToday { get; set; }

    /// <summary>
    /// Team Points Week
    /// </summary>
    [DataMember(Order = 14)]
    public long TeamPointsWeek { get; set; }

    /// <summary>
    /// Team Points Total
    /// </summary>
    [DataMember(Order = 15)]
    public long TeamPointsTotal { get; set; }

    /// <summary>
    /// Team Work Units Total
    /// </summary>
    [DataMember(Order = 16)]
    public long TeamWorkUnitsTotal { get; set; }

    /// <summary>
    /// Team Points Update
    /// </summary>
    [DataMember(Order = 17)]
    public long TeamPointsUpdate { get; set; }

    /// <summary>
    /// Team Rank
    /// </summary>
    [DataMember(Order = 18)]
    public int TeamRank { get; set; }

    /// <summary>
    /// Team Change Rank Twenty Four Hours
    /// </summary>
    [DataMember(Order = 19)]
    public int TeamChangeRankTwentyFourHours { get; set; }

    /// <summary>
    /// Team Change Rank Twenty Four Hours
    /// </summary>
    [DataMember(Order = 20)]
    public int TeamChangeRankSevenDays { get; set; }

    // not serialized
    public string Status { get; set; }

    public bool Equals(EocStatsData other)
    {
        if (other is null) return false;
        if (ReferenceEquals(this, other)) return true;
        return UserTwentyFourHourAverage == other.UserTwentyFourHourAverage &&
               UserPointsToday == other.UserPointsToday &&
               UserPointsWeek == other.UserPointsWeek &&
               UserPointsTotal == other.UserPointsTotal &&
               UserWorkUnitsTotal == other.UserWorkUnitsTotal &&
               UserPointsUpdate == other.UserPointsUpdate &&
               UserTeamRank == other.UserTeamRank &&
               UserOverallRank == other.UserOverallRank &&
               UserChangeRankTwentyFourHours == other.UserChangeRankTwentyFourHours &&
               UserChangeRankSevenDays == other.UserChangeRankSevenDays &&
               TeamTwentyFourHourAverage == other.TeamTwentyFourHourAverage &&
               TeamPointsToday == other.TeamPointsToday &&
               TeamPointsWeek == other.TeamPointsWeek &&
               TeamPointsTotal == other.TeamPointsTotal &&
               TeamWorkUnitsTotal == other.TeamWorkUnitsTotal &&
               TeamPointsUpdate == other.TeamPointsUpdate &&
               TeamRank == other.TeamRank &&
               TeamChangeRankTwentyFourHours == other.TeamChangeRankTwentyFourHours &&
               TeamChangeRankSevenDays == other.TeamChangeRankSevenDays;
    }

    public override bool Equals(object obj)
    {
        if (obj is null) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != GetType()) return false;
        return Equals((EocStatsData)obj);
    }

    public override int GetHashCode()
    {
        // ReSharper disable NonReadonlyMemberInGetHashCode
        unchecked
        {
            var hashCode = UserTwentyFourHourAverage.GetHashCode();
            hashCode = (hashCode * 397) ^ UserPointsToday.GetHashCode();
            hashCode = (hashCode * 397) ^ UserPointsWeek.GetHashCode();
            hashCode = (hashCode * 397) ^ UserPointsTotal.GetHashCode();
            hashCode = (hashCode * 397) ^ UserWorkUnitsTotal.GetHashCode();
            hashCode = (hashCode * 397) ^ UserPointsUpdate.GetHashCode();
            hashCode = (hashCode * 397) ^ UserTeamRank;
            hashCode = (hashCode * 397) ^ UserOverallRank;
            hashCode = (hashCode * 397) ^ UserChangeRankTwentyFourHours;
            hashCode = (hashCode * 397) ^ UserChangeRankSevenDays;
            hashCode = (hashCode * 397) ^ TeamTwentyFourHourAverage.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamPointsToday.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamPointsWeek.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamPointsTotal.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamWorkUnitsTotal.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamPointsUpdate.GetHashCode();
            hashCode = (hashCode * 397) ^ TeamRank;
            hashCode = (hashCode * 397) ^ TeamChangeRankTwentyFourHours;
            hashCode = (hashCode * 397) ^ TeamChangeRankSevenDays;
            return hashCode;
        }
        // ReSharper restore NonReadonlyMemberInGetHashCode
    }

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
