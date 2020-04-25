
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public class NullClient : Client
    {
        public NullClient() : base(null, null, null)
        {
            
        }

        public NullClient(ILogger logger, IPreferenceSet preferences, IProteinBenchmarkService benchmarkService) 
            : base(logger, preferences, benchmarkService)
        {

        }

        protected override void OnRetrieve()
        {
            // do nothing
        }
    }
}
