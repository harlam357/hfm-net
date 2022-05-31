using System.Globalization;
using System.Text;

namespace HFM.Core.ApplicationUpdates.Internal;

internal static class ByteArrayExtensions
{
    /// <summary>
    /// Converts an array of bytes to a Hex string representation.
    /// </summary>
    internal static string ToHex(this byte[] value)
    {
        if (value == null || value.Length == 0)
        {
            return String.Empty;
        }

        const string hexFormat = "{0:X2}";
        var sb = new StringBuilder();
        foreach (byte b in value)
        {
            sb.Append(String.Format(CultureInfo.InvariantCulture, hexFormat, b));
        }
        return sb.ToString();
    }
}
