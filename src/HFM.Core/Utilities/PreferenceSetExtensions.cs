
using System.Net;
using System.Text;

using HFM.Preferences;

namespace HFM.Core
{
   public static class PreferenceSetExtensions
   {
      /// <summary>
      /// Gets a WebProxy object based on preference settings.
      /// </summary>
      public static IWebProxy GetWebProxy(this IPreferenceSet prefs)
      {
         if (prefs == null || !prefs.Get<bool>(Preference.UseProxy))
         {
            return null;
         }

         IWebProxy proxy = new WebProxy(prefs.Get<string>(Preference.ProxyServer),
            prefs.Get<int>(Preference.ProxyPort));

         if (prefs.Get<bool>(Preference.UseProxyAuth))
         {
            proxy.Credentials = NetworkCredentialFactory.Create(prefs.Get<string>(Preference.ProxyUser),
               prefs.Get<string>(Preference.ProxyPass));
         }
         return proxy;
      }

      /// <summary>
      /// Gets the points per day format string based on preference settings.
      /// </summary>
      public static string GetPpdFormatString(this IPreferenceSet prefs)
      {
         var decimalPlaces = prefs.Get<int>(Preference.DecimalPlaces);

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