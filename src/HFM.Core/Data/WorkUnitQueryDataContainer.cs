using System;
using System.Collections.Generic;

using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Data
{
    public class WorkUnitQueryDataContainer : DataContainer<List<WorkUnitQuery>>
    {
        public const string DefaultFileName = "WuHistoryQuery.dat";

        public override Serializers.IFileSerializer<List<WorkUnitQuery>> DefaultSerializer => new Serializers.ProtoBufFileSerializer<List<WorkUnitQuery>>();

        public WorkUnitQueryDataContainer() : this(null, null)
        {

        }

        public WorkUnitQueryDataContainer(ILogger logger, IPreferences preferences) : base(logger)
        {
            var path = preferences?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }
    }
}
