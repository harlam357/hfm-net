using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Core.Client;

public interface IClientDataValidationRule
{
    void Validate(SlotModel slotModel);
}

public class ClientUsernameValidationRule : IClientDataValidationRule
{
    private readonly IPreferences _preferences;

    public ClientUsernameValidationRule(IPreferences preferences)
    {
        _preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
    }

    public void Validate(SlotModel slotModel)
    {
        if (FoldingIdentityIsDefault(slotModel.WorkUnitModel.WorkUnit) || !slotModel.Status.IsOnline())
        {
            slotModel.UsernameOk = true;
        }
        else
        {
            slotModel.UsernameOk = slotModel.WorkUnitModel.WorkUnit.FoldingID == _preferences.Get<string>(Preference.StanfordId) &&
                                   slotModel.WorkUnitModel.WorkUnit.Team == _preferences.Get<int>(Preference.TeamId);
        }
    }

    private static bool FoldingIdentityIsDefault(WorkUnit workUnit) =>
        (String.IsNullOrWhiteSpace(workUnit.FoldingID) || workUnit.FoldingID == Unknown.Value) && workUnit.Team == default;
}

public class ClientProjectIsDuplicateValidationRule : IClientDataValidationRule
{
    private readonly ICollection<string> _duplicateProjects;

    public ClientProjectIsDuplicateValidationRule(ICollection<string> duplicateProjects)
    {
        _duplicateProjects = duplicateProjects ?? throw new ArgumentNullException(nameof(duplicateProjects));
    }

    public static ICollection<string> FindDuplicateProjects(ICollection<SlotModel> slots) =>
        slots.GroupBy(x => x.WorkUnitModel.WorkUnit.ToShortProjectString())
            .Where(g => g.Count() > 1 && g.First().WorkUnitModel.WorkUnit.HasProject())
            .Select(g => g.Key)
            .ToList();

    public void Validate(SlotModel slotModel) =>
        slotModel.ProjectIsDuplicate = _duplicateProjects.Contains(slotModel.WorkUnitModel.WorkUnit.ToShortProjectString());
}
