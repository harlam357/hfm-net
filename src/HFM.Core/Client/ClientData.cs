using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client;

public interface IClientData
{
    SlotStatus Status { get; }
    int PercentComplete { get; }
    string Name { get; }
    string SlotTypeString { get; }
    string Processor { get; }
    TimeSpan TPF { get; }
    double PPD { get; }
    double UPD { get; }
    TimeSpan ETA { get; }
    DateTime ETADate { get; }
    string Core { get; }
    string ProjectRunCloneGen { get; }
    double Credit { get; }
    int Completed { get; }
    int Failed { get; }
    string Username { get; }
    DateTime Assigned { get; }
    DateTime PreferredDeadline { get; }

    IProjectInfo ProjectInfo { get; }
    IProductionProvider ProductionProvider { get; }
    IReadOnlyCollection<LogLine> CurrentLogLines { get; }
    ValidationRuleErrors Errors { get; }

    string FoldingID { get; }
    int Team { get; }
    ClientSettings Settings { get; }
    ClientPlatform Platform { get; }
    Protein CurrentProtein { get; }

    SlotIdentifier SlotIdentifier { get; }
    ProteinBenchmarkIdentifier BenchmarkIdentifier { get; }
}

public class ClientData : IClientData
{
    public virtual SlotStatus Status { get; set; }
    public virtual int PercentComplete { get; set; }
    public virtual string Name { get; set; }
    public virtual string SlotTypeString { get; set; }
    public virtual string Processor { get; set; }
    public virtual TimeSpan TPF { get; set; }
    public virtual double PPD { get; set; }
    public virtual double UPD { get; set; }
    public virtual TimeSpan ETA { get; set; }
    public virtual DateTime ETADate { get; set; }
    public virtual string Core { get; set; }
    public virtual string ProjectRunCloneGen { get; set; }
    public virtual double Credit { get; set; }
    public virtual int Completed { get; set; }
    public virtual int Failed { get; set; }
    public virtual string Username { get; set; }
    public virtual DateTime Assigned { get; set; }
    public virtual DateTime PreferredDeadline { get; set; }

    public virtual IProjectInfo ProjectInfo { get; set; }
    public virtual IProductionProvider ProductionProvider { get; set; }
    public virtual IReadOnlyCollection<LogLine> CurrentLogLines { get; set; } = new List<LogLine>();
    public ValidationRuleErrors Errors { get; } = new();

    public virtual string FoldingID { get; set; }
    public virtual int Team { get; set; }
    public virtual ClientSettings Settings { get; set; }
    public virtual ClientPlatform Platform { get; set; }
    public virtual Protein CurrentProtein { get; set; }

    public virtual SlotIdentifier SlotIdentifier { get; set; }
    public virtual ProteinBenchmarkIdentifier BenchmarkIdentifier { get; set; }

    public static ClientData Offline(ClientSettings settings) =>
        new()
        {
            Status = SlotStatus.Offline,
            Name = settings?.Name,
            Settings = settings
        };

    public static void ValidateRules(ICollection<IClientData> collection, IPreferences preferences)
    {
        var rules = new IClientDataValidationRule[]
        {
            new ClientUsernameValidationRule(preferences),
            new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(collection))
        };

        foreach (var c in collection)
        {
            foreach (var rule in rules)
            {
                rule.Validate(c);
            }
        }
    }
}
