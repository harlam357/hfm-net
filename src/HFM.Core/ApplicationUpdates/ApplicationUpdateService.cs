using System;
using System.IO;
using System.Net.Cache;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Core.ApplicationUpdates
{
    public interface IApplicationUpdateService
    {
        ApplicationUpdate GetApplicationUpdate(Uri requestUri);
    }

    public class ApplicationUpdateService : IApplicationUpdateService
    {
        public IPreferences Preferences { get; }

        public ApplicationUpdateService(IPreferences preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public ApplicationUpdate GetApplicationUpdate(Uri requestUri)
        {
            using (var stream = new MemoryStream())
            {
                var webOperation = WebOperation.Create(requestUri);
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
