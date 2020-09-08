using System;

using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.Data
{
    public class EocStatsDataContainer : DataContainer<EocStatsData>
    {
        public const string DefaultFileName = "UserStatsCache.dat";

        public override Serializers.IFileSerializer<EocStatsData> DefaultSerializer => new Serializers.ProtoBufFileSerializer<EocStatsData>();

        public EocStatsDataContainer() : this(null, null)
        {

        }

        public EocStatsDataContainer(ILogger logger, IPreferences preferences) : base(logger)
        {
            var path = preferences?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }
    }
}
