
using System.Text;

namespace HFM.Core.Client
{
    public static class NumberFormat
    {
        /// <summary>
        /// Gets the number format string using the given number of decimal places.
        /// </summary>
        public static string Get(int decimalPlaces)
        {
            const string baseFormat = "###,###,##0";
            if (decimalPlaces <= 0) return baseFormat;

            var sb = new StringBuilder(baseFormat);
            sb.Append(".");
            for (int i = 0; i < decimalPlaces; i++)
            {
                sb.Append("0");
            }
            return sb.ToString();
        }
    }
}
