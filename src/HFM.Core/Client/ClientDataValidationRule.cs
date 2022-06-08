using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client;

public interface IClientDataValidationRule
{
    void Validate(IClientData clientData);
}

public class ClientUsernameValidationRule : IClientDataValidationRule
{
    public const string Key = "UsernameOk";

    private readonly IPreferences _preferences;

    public ClientUsernameValidationRule(IPreferences preferences)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
    }

    public void Validate(IClientData clientData)
    {
        if (FoldingIdentityIsDefault(clientData) || !clientData.Status.IsOnline())
        {
            clientData.Errors[Key] = true;
        }
        else
        {
            clientData.Errors[Key] = clientData.FoldingID == _preferences.Get<string>(Preference.StanfordId) &&
                                     clientData.Team == _preferences.Get<int>(Preference.TeamId);
        }
    }

    private static bool FoldingIdentityIsDefault(IClientData clientData) =>
        (String.IsNullOrWhiteSpace(clientData.FoldingID) || clientData.FoldingID == Unknown.Value) && clientData.Team == default;
}

public class ClientProjectIsDuplicateValidationRule : IClientDataValidationRule
{
    public const string Key = "ProjectIsDuplicate";

    private readonly ICollection<string> _duplicateProjects;

    public ClientProjectIsDuplicateValidationRule(ICollection<string> duplicateProjects)
    {
        _duplicateProjects = duplicateProjects ?? throw new ArgumentNullException(nameof(duplicateProjects));
    }

    public static ICollection<string> FindDuplicateProjects(ICollection<IClientData> collection) =>
        collection.GroupBy(x => x.ProjectInfo.ToShortProjectString())
            .Where(g => g.Count() > 1 && g.First().ProjectInfo.HasProject())
            .Select(g => g.Key)
            .ToList();

    public void Validate(IClientData clientData) =>
        clientData.Errors[Key] = _duplicateProjects.Contains(clientData.ProjectInfo.ToShortProjectString());
}

public class ValidationRuleErrors : Dictionary<string, object>
{
    public T GetValue<T>(string key) => TryGetValue(key, out object value) ? (T)value : default;
}
