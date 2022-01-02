using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using HFM.Client;
using HFM.Log;

namespace HFM.Core.Client.Mocks
{
    internal static class FahClientFactoryForTests
    {
        internal static async Task<FahClient> CreateClientWithMessagesLoadedFrom(string clientName, string path)
        {
            var fahClient = CreateClient(clientName);
            await LoadMessagesFrom(fahClient, path);
            return fahClient;
        }

        internal static FahClient CreateClient(string clientName)
        {
            var client = new ConnectedFahClient();
            client.Settings = new ClientSettings { Name = clientName };
            return client;
        }

        private static async Task LoadMessagesFrom(FahClient fahClient, string path)
        {
            var extractor = new FahClientJsonMessageExtractor();

            foreach (var file in Directory.EnumerateFiles(path).OrderBy(x => x))
            {
                if (Path.GetFileName(file) == "log.txt")
                {
                    using (var textReader = new StreamReader(file))
                    using (var reader = new FahClientLogTextReader(textReader))
                    {
                        await fahClient.Messages.Log.ReadAsync(reader);
                    }
                }
                else
                {
                    await fahClient.Messages.UpdateMessageAsync(extractor.Extract(new StringBuilder(File.ReadAllText(file))));
                }
            }
        }

        private class ConnectedFahClient : FahClient
        {
            public ConnectedFahClient() : base(null, null, null, null, null)
            {
                Messages = new FahClientMessages(this, null);
            }

            public override bool Connected => true;
        }
    }
}
