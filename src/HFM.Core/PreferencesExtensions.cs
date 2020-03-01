
using System.Net;
using System.Text;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Core
{
    public static class PreferencesExtensions
    {
        /// <summary>
        /// Gets a WebProxy object based on preference settings.
        /// </summary>
        public static IWebProxy GetWebProxy(this IPreferenceSet preferences)
        {
            if (preferences == null || !preferences.Get<bool>(Preference.UseProxy))
            {
                return null;
            }

            IWebProxy proxy = new WebProxy(preferences.Get<string>(Preference.ProxyServer),
               preferences.Get<int>(Preference.ProxyPort));

            if (preferences.Get<bool>(Preference.UseProxyAuth))
            {
                proxy.Credentials = NetworkCredentialFactory.Create(preferences.Get<string>(Preference.ProxyUser),
                   preferences.Get<string>(Preference.ProxyPass));
            }
            return proxy;
        }

        /// <summary>
        /// Gets the points per day format string based on preference settings.
        /// </summary>
        public static string GetPpdFormatString(this IPreferenceSet preferences)
        {
            var decimalPlaces = preferences.Get<int>(Preference.DecimalPlaces);

            var sb = new StringBuilder("###,###,##0");
            if (decimalPlaces > 0)
            {
                sb.Append(".");
                for (int i = 0; i < decimalPlaces; i++)
                {
                    sb.Append("0");
                }
            }

            return sb.ToString();
        }
    }
}