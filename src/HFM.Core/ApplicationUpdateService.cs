using System.IO;
using System.Net.Cache;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Core
{
    public interface IApplicationUpdateService
    {
        ApplicationUpdate GetApplicationUpdate(string url);
    }

    public class ApplicationUpdateService : IApplicationUpdateService
    {
        public IPreferenceSet Preferences { get; }

        public ApplicationUpdateService(IPreferenceSet preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public ApplicationUpdate GetApplicationUpdate(string url)
        {
            using (var stream = new MemoryStream())
            {
                var webOperation = WebOperation.Create(url);
                webOperation.WebRequest.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);
                webOperation.WebRequest.Proxy = WebProxyFactory.Create(Preferences);
                webOperation.Download(stream);

                stream.Position = 0;
                var serializer = new ApplicationUpdateSerializer();
                return serializer.Deserialize(stream);
            }
        }
    }
}
