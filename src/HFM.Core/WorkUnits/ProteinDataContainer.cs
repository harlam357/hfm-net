using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    public class ProteinDataContainer : DataContainer<List<Protein>>
    {
        public const string DefaultFileName = "ProjectInfo.tab";

        public override Serializers.IFileSerializer<List<Protein>> DefaultSerializer => new TabSerializer();

        public ProteinDataContainer() : this(null, null)
        {

        }

        public ProteinDataContainer(ILogger logger, IPreferences preferences) : base(logger)
        {
            var path = preferences?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = Path.Combine(path, DefaultFileName);
            }
        }

        private class TabSerializer : Serializers.IFileSerializer<List<Protein>>
        {
            private readonly TabDelimitedTextSerializer _serializer = new TabDelimitedTextSerializer();

            public string FileExtension => "tab";

            public string FileTypeFilter => "Project Info Tab Delimited Files|*.tab";

            public List<Protein> Deserialize(string path)
            {
                var result = _serializer.ReadFile(path);
                return result as List<Protein> ?? result.ToList();
            }

            public void Serialize(string path, List<Protein> value)
            {
                _serializer.WriteFile(path, value);
            }
        }
    }
}
