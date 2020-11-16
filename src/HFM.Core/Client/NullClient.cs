using System.Threading.Tasks;

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

        public NullClient(ILogger logger, IPreferences preferences, IProteinBenchmarkService benchmarkService)
            : base(logger, preferences, benchmarkService)
        {
        }

        protected override Task OnRetrieve() => Task.CompletedTask;
    }
}
