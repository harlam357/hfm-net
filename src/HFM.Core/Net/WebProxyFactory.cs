
using System.Net;

using HFM.Preferences;

namespace HFM.Core.Net
{
    public static class WebProxyFactory
    {
        /// <summary>
        /// Creates a WebProxy object based on preference settings.
        /// </summary>
        public static WebProxy Create(IPreferences preferences)
        {
            if (preferences == null || !preferences.Get<bool>(Preference.UseProxy))
            {
                return null;
            }

            var proxy = new WebProxy(
                preferences.Get<string>(Preference.ProxyServer),
                preferences.Get<int>(Preference.ProxyPort));

            if (preferences.Get<bool>(Preference.UseProxyAuth))
            {
                proxy.Credentials = NetworkCredentialFactory.Create(
                    preferences.Get<string>(Preference.ProxyUser),
                    preferences.Get<string>(Preference.ProxyPass));
            }
            return proxy;
        }
    }
}
