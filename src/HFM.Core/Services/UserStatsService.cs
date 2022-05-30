using System.Xml;

using HFM.Preferences;

namespace HFM.Core.Services;

public interface IUserStatsService
{
    UserStatsData GetStatsData();
}

public class UserStatsService : IUserStatsService
{
    public const string UserBaseUrl = "https://folding.extremeoverclocking.com/user_summary.php?s=&u=";
    public const string TeamBaseUrl = "https://folding.extremeoverclocking.com/team_summary.php?s=&t=";
    public const string UserXmlBaseUrl = "https://folding.extremeoverclocking.com/xml/user_summary.php?u=";

    private readonly IPreferences _preferences;

    public UserStatsService(IPreferences preferences)
    {
        _preferences = preferences;
    }

    public UserStatsData GetStatsData()
    {
        string eocXmlDataUrl = String.Concat(UserXmlBaseUrl, _preferences.Get<int>(Preference.EocUserId));

        var xmlData = new XmlDocument();
        using (var reader = XmlReader.Create(eocXmlDataUrl, new XmlReaderSettings { XmlResolver = null }))
        {
            xmlData.Load(reader);
        }
        xmlData.RemoveChild(xmlData.ChildNodes[0]);

        XmlNode eocNode = GetXmlNode(xmlData, "EOC_Folding_Stats");
        return CreateStatsDataFromXmlNode(eocNode);
    }

    /// <summary>
    /// Updates the data container
    /// </summary>
    /// <param name="eocNode">EOC Stats XmlNode</param>
    private static UserStatsData CreateStatsDataFromXmlNode(XmlNode eocNode)
    {
        XmlNode teamNode = eocNode.SelectSingleNode("team");
        XmlNode userNode = eocNode.SelectSingleNode("user");

        var data = new UserStatsData { LastUpdated = DateTime.UtcNow };
        data.UserTwentyFourHourAverage = Convert.ToInt64(GetXmlNode(userNode, "Points_24hr_Avg").InnerText);
        data.UserPointsToday = Convert.ToInt64(GetXmlNode(userNode, "Points_Today").InnerText);
        data.UserPointsWeek = Convert.ToInt64(GetXmlNode(userNode, "Points_Week").InnerText);
        data.UserPointsTotal = Convert.ToInt64(GetXmlNode(userNode, "Points").InnerText);
        data.UserWorkUnitsTotal = Convert.ToInt64(GetXmlNode(userNode, "WUs").InnerText);
        data.UserPointsUpdate = Convert.ToInt64(GetXmlNode(userNode, "Points_Update").InnerText);
        data.UserTeamRank = Convert.ToInt32(GetXmlNode(userNode, "Team_Rank").InnerText);
        data.UserOverallRank = Convert.ToInt32(GetXmlNode(userNode, "Overall_Rank").InnerText);
        data.UserChangeRankTwentyFourHours = Convert.ToInt32(GetXmlNode(userNode, "Change_Rank_24hr").InnerText);
        data.UserChangeRankSevenDays = Convert.ToInt32(GetXmlNode(userNode, "Change_Rank_7days").InnerText);
        data.TeamTwentyFourHourAverage = Convert.ToInt64(GetXmlNode(teamNode, "Points_24hr_Avg").InnerText);
        data.TeamPointsToday = Convert.ToInt64(GetXmlNode(teamNode, "Points_Today").InnerText);
        data.TeamPointsWeek = Convert.ToInt64(GetXmlNode(teamNode, "Points_Week").InnerText);
        data.TeamPointsTotal = Convert.ToInt64(GetXmlNode(teamNode, "Points").InnerText);
        data.TeamWorkUnitsTotal = Convert.ToInt64(GetXmlNode(teamNode, "WUs").InnerText);
        data.TeamPointsUpdate = Convert.ToInt64(GetXmlNode(teamNode, "Points_Update").InnerText);
        data.TeamRank = Convert.ToInt32(GetXmlNode(teamNode, "Rank").InnerText);
        data.TeamChangeRankTwentyFourHours = Convert.ToInt32(GetXmlNode(teamNode, "Change_Rank_24hr").InnerText);
        data.TeamChangeRankSevenDays = Convert.ToInt32(GetXmlNode(teamNode, "Change_Rank_7days").InnerText);

        XmlNode statusNode = GetXmlNode(eocNode, "status");
        data.Status = GetXmlNode(statusNode, "Update_Status").InnerText;

        return data;
    }

    private static XmlNode GetXmlNode(XmlNode xmlNode, string xpath)
    {
        XmlNode node = xmlNode.SelectSingleNode(xpath);
        if (node == null)
        {
            throw new XmlException($"Node '{xpath}' does not exist.");
        }

        return node;
    }
}
