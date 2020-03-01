
using System.Text;

using HFM.Preferences;

namespace HFM.Core
{
    public static class PreferencesExtensions
    {
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