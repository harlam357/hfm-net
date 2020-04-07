
using System;

namespace HFM.Core.Net
{
    public static class HttpUrl
    {
        public static bool Validate(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out var result) && (result.Scheme == Uri.UriSchemeHttp || result.Scheme == Uri.UriSchemeHttps);
        }
    }
}
