using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace HFM.Core.Internal
{
    internal static class NewtonsoftHttpClientExtensions
    {
        internal static async Task<T> GetFromJsonAsync<T>(this HttpClient httpClient, string uri, JsonSerializerSettings settings = null, CancellationToken cancellationToken = default)
        {
            var response = await httpClient.GetAsync(uri, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonConvert.DeserializeObject<T>(json, settings);
        }
    }
}
