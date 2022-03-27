using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client;

public class NullClient : Client
{
    public NullClient() : base(null, null, null)
    {
    }

    public NullClient(ILogger logger, IPreferences preferences, IProteinBenchmarkRepository benchmarks)
        : base(logger, preferences, benchmarks)
    {
    }

    protected override Task OnRetrieve() => Task.CompletedTask;
}
