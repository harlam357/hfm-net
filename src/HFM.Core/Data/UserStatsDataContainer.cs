using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.Data;

public class UserStatsDataContainer : DataContainer<UserStatsData>
{
    public const string DefaultFileName = "UserStatsCache.dat";

    public override Serializers.IFileSerializer<UserStatsData> DefaultSerializer => new Serializers.ProtoBufFileSerializer<UserStatsData>();

    public UserStatsDataContainer() : this(null, null)
    {

    }

    public UserStatsDataContainer(ILogger logger, IPreferences preferences) : base(logger)
    {
        var path = preferences?.Get<string>(Preference.ApplicationDataFolderPath);
        if (!String.IsNullOrEmpty(path))
        {
            FilePath = Path.Combine(path, DefaultFileName);
        }
    }
}
