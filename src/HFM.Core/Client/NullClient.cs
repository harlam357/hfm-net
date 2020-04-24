
using HFM.Core.Logging;
using HFM.Preferences;

namespace HFM.Core.Client
{
    public class NullClient : Client
    {
        public NullClient() : this(null)
        {
            
        }

        public NullClient(ILogger logger) : this(logger, null)
        {

        }

        public NullClient(ILogger logger, IPreferenceSet preferences) : base(logger, preferences)
        {

        }

        protected override void OnRetrieve()
        {
            // do nothing
        }
    }
}
