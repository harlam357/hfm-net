
using System;

namespace HFM.Core.Net
{
    public static class HttpUrl
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1054:URI-like parameters should not be strings", Justification = "Validating string input can be converted to Uri.")]
        public static bool Validate(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
