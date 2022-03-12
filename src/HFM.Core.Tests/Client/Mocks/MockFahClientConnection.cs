using HFM.Client;

namespace HFM.Core.Client.Mocks;

public class MockFahClientConnection : FahClientConnection
{
    private bool _connected;

    public override bool Connected => _connected;

    public MockFahClientConnection() : base("foo", 2000)
    {

    }

    public override void Open() => _connected = true;

    public override Task OpenAsync()
    {
        _connected = true;
        return Task.CompletedTask;
    }

    public override void Close() => _connected = false;

    public IList<MockFahClientCommand> Commands { get; } = new List<MockFahClientCommand>();

    protected override FahClientCommand OnCreateCommand()
    {
        var command = new MockFahClientCommand(this);
        Commands.Add(command);
        return command;
    }

    protected override FahClientReader OnCreateReader() => new MockFahClientReader(this);
}

public class MockFahClientCommand : FahClientCommand
{
    public MockFahClientCommand(FahClientConnection connection) : base(connection)
    {

    }

    public bool Executed { get; private set; }

    public override int Execute()
    {
        Executed = true;
        return 0;
    }

    public override Task<int> ExecuteAsync()
    {
        Executed = true;
        return Task.FromResult(0);
    }
}

public class MockFahClientReader : FahClientReader
{
    public MockFahClientReader(FahClientConnection connection) : base(connection)
    {

    }

    public override bool Read() => false;

    public override Task<bool> ReadAsync() => Task.FromResult(false);
}
