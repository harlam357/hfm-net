using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client;

public class NullClient : Client
{
    public NullClient() : base(null, null)
    {
    }

    public NullClient(ILogger logger, IPreferences preferences)
        : base(logger, preferences)
    {
    }

    protected override Task OnRetrieve() => Task.CompletedTask;
}
