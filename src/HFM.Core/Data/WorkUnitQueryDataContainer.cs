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
                FilePath = Path.Combine(path, DefaultFileName);
            }
        }

        public override void Read()
        {
            base.Read();
            // remove queries containing removed fields
            Data = Data.Where(x => !HasRemovedField(x)).ToList();
        }

        private static bool HasRemovedField(WorkUnitQuery query)
        {
            var parameters = query?.Parameters;
            // 14 is WorkUnitName
            return parameters != null && parameters.Any(x => 14 == (int)x.Column);
        }
    }
}
