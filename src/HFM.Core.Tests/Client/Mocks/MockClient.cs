using System.Threading.Tasks;

using HFM.Core.Logging;

using Moq;

namespace HFM.Core.Client.Mocks
{
    public class MockClient : Client
    {
        public MockClient() : base(Mock.Of<ILogger>(), null, null)
        {

        }

        protected MockClient(bool connected) : this()
        {
            Connected = connected;
        }

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
    }
}
