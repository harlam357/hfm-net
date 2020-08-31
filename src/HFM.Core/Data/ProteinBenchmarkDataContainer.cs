
using System;
using System.Collections.Generic;

using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Data
{
    public class ProteinBenchmarkDataContainer : DataContainer<List<ProteinBenchmark>>
    {
        public const string DefaultFileName = "BenchmarkCache.dat";
        
        public override Serializers.IFileSerializer<List<ProteinBenchmark>> DefaultSerializer => new Serializers.ProtoBufFileSerializer<List<ProteinBenchmark>>();

        public ProteinBenchmarkDataContainer() : this(null)
        {

        }

        public ProteinBenchmarkDataContainer(IPreferences prefs)
        {
            var path = prefs?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }
    }
}
