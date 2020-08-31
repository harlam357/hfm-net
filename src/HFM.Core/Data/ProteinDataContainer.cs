
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Data
{
    public class ProteinDataContainer : DataContainer<List<Protein>>
    {
        public const string DefaultFileName = "ProjectInfo.tab";

        public override Serializers.IFileSerializer<List<Protein>> DefaultSerializer => new TabSerializer();

        public ProteinDataContainer() : this(null)
        {
            
        }

        public ProteinDataContainer(IPreferences prefs)
        {
            var path = prefs?.Get<string>(Preference.ApplicationDataFolderPath);
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
                return _serializer.ReadFile(path).ToList();
            }

            public void Serialize(string path, List<Protein> value)
            {
                _serializer.WriteFile(path, value);
            }
        }
    }
}
