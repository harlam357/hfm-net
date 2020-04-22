
using HFM.Core.Logging;

namespace HFM.Core.Client
{
    public class NullClient : Client
    {
        public NullClient() : this(null)
        {
            
        }

        public NullClient(ILogger logger) : base(logger)
        {

        }

        protected override void OnRetrieve()
        {
            // do nothing
        }
    }
}
