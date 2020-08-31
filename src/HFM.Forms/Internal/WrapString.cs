using System;
using System.Globalization;

namespace HFM.Forms.Internal
{
    internal static class WrapString
    {
        internal static string InQuotes(string value) => String.Format(CultureInfo.InvariantCulture, "\"{0}\"", value);
    }
}
