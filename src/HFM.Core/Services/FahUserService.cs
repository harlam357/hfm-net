using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json.Serialization;

namespace HFM.Core.Services;

public class FahUser
{
    [JsonPropertyName("id")]
    public int ID { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }
}

public abstract class FahUserService
{
    public static FahUserService Default { get; } = new DefaultFahUserService();

    public abstract Task<FahUser> FindUser(string name);
}

public class DefaultFahUserService : FahUserService
{
    private static readonly HttpClient _Client = new();

    public override async Task<FahUser> FindUser(string name)
    {
        string uri = String.Format(CultureInfo.InvariantCulture, FahUrl.UserFindApiUrlTemplate, name);
        return await _Client.GetFromJsonAsync<FahUser>(uri).ConfigureAwait(false);
    }
}

public class NullFahUserService : FahUserService
{
    public static NullFahUserService Instance { get; } = new();

    public override Task<FahUser> FindUser(string name) => Task.FromResult(new FahUser());
}
