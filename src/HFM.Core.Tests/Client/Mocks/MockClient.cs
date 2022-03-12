using HFM.Core.Logging;

namespace HFM.Core.Client.Mocks;

public class MockClient : Client
{
    public MockClient() : base(null, null, null)
    {

    }

    public MockClient(ILogger logger) : base(logger, null, null)
    {

    }

    public sealed override bool Connected { get; protected set; }

    protected override Task OnConnect()
    {
        Connected = true;
        return Task.CompletedTask;
    }

    public int RetrieveCount { get; private set; }

    protected override Task OnRetrieve()
    {
        RetrieveCount++;
        return Task.CompletedTask;
    }

    protected override void OnClose()
    {
        base.OnClose();
        Connected = false;
    }
}
