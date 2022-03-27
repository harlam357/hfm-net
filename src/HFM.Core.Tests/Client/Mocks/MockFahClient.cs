using System.Text;

using HFM.Client;
using HFM.Core.Data;
using HFM.Log;

using Moq;

namespace HFM.Core.Client.Mocks;

public class MockFahClient : FahClient
{
    public MockFahClient() : base(null, null, null, null, Mock.Of<IWorkUnitRepository>())
    {
        Messages = new FahClientMessages(this, null);
    }

    protected override async Task OnConnect()
    {
        Connection = new MockFahClientConnection();
        await Connection.OpenAsync();
    }

    protected override void OnClose()
    {
        base.OnClose();
        Connection.Close();
    }

    public static MockFahClient Create(string clientName)
    {
        var client = new MockFahClient
        {
            Settings = new ClientSettings { Name = clientName }
        };
        return client;
    }

    public async Task LoadMessagesFrom(string path)
    {
        var extractor = new FahClientJsonMessageExtractor();

        foreach (var file in Directory.EnumerateFiles(path).OrderBy(x => x))
        {
            if (Path.GetFileName(file) == "log.txt")
            {
                using (var textReader = new StreamReader(file))
                using (var reader = new FahClientLogTextReader(textReader))
                {
                    await Messages.Log.ReadAsync(reader);
                }
            }
            else
            {
                await Messages.UpdateMessageAsync(
                    extractor.Extract(
                        new StringBuilder(
                            await File.ReadAllTextAsync(file))));
            }
        }
    }

    public async Task LoadMessage(string path) =>
        await LoadMessage(new StringBuilder(await File.ReadAllTextAsync(path)));

    public async Task LoadMessage(StringBuilder buffer)
    {
        var extractor = new FahClientJsonMessageExtractor();
        await Messages.UpdateMessageAsync(extractor.Extract(buffer));
    }
}
