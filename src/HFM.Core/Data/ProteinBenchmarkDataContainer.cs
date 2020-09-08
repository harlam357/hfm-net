using System;
using System.Collections.Generic;

using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Data
{
    public class ProteinBenchmarkDataContainer : DataContainer<List<ProteinBenchmark>>
    {
        public const string DefaultFileName = "BenchmarkCache.dat";

        public override Serializers.IFileSerializer<List<ProteinBenchmark>> DefaultSerializer => new Serializers.ProtoBufFileSerializer<List<ProteinBenchmark>>();

        public ProteinBenchmarkDataContainer() : this(null, null)
        {

        }

        public ProteinBenchmarkDataContainer(ILogger logger, IPreferences preferences) : base(logger)
        {
            var path = preferences?.Get<string>(Preference.ApplicationDataFolderPath);
            if (!String.IsNullOrEmpty(path))
            {
                FilePath = System.IO.Path.Combine(path, DefaultFileName);
            }
        }
    }
}
