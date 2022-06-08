using System.Globalization;
using System.Text;

using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.Client;

// TODO: TimeFormatting should be a function of SlotModel, not view grid painting
public enum TimeFormatting
{
    None,
    Format1
}

public enum UnitTotalsType
{
    All,
    ClientStart
}

public interface IClientData
{
    SlotStatus Status { get; }
    int PercentComplete { get; }
    string Name { get; }
    string SlotTypeString { get; }
    string Processor { get; }
    TimeSpan TPF { get; }
    double PPD { get; }
    TimeSpan ETA { get; }
    DateTime ETADate { get; }
    string Core { get; }
    string CoreID { get; }
    string ProjectRunCloneGen { get; }
    double Credit { get; }
    int Completed { get; }
    int Failed { get; }
    string Username { get; }
    DateTime Assigned { get; }
    DateTime PreferredDeadline { get; }
    IProjectInfo ProjectInfo { get; }
    ValidationRuleErrors Errors { get; }
}

public class SlotModel : IClientData, IProteinBenchmarkDetailSource
{
    public SlotIdentifier SlotIdentifier => new(Client.Settings.ClientIdentifier, SlotID);

    public IClient Client { get; }

    public WorkUnitModel WorkUnitModel { get; set; }

    public IProjectInfo ProjectInfo => WorkUnitModel.WorkUnit;

    public SlotModel(IClient client)
    {
        Client = client ?? throw new ArgumentNullException(nameof(client));
        WorkUnitModel = WorkUnitModel.Empty(this);
    }

    public int SlotID { get; set; } = SlotIdentifier.NoSlotID;

    public virtual SlotStatus Status { get; set; }

    public virtual int PercentComplete { get; set; }

    public virtual string Name
    {
        get => SlotIdentifier.Name;
        set => _ = value;
    }

    public SlotType SlotType => Description?.SlotType ?? default;

    public virtual string SlotTypeString { get; set; }

    string IProteinBenchmarkDetailSource.Processor => Description?.Processor;

    public virtual string Processor => Description?.Processor;

    public int? Threads => Description is CPUSlotDescription cpu ? cpu.CPUThreads : null;

    public SlotDescription Description { get; set; }

    public virtual TimeSpan TPF { get; set; }

    public virtual double PPD { get; set; }

    public virtual double UPD { get; set; }

    public virtual TimeSpan ETA { get; set; }

    public virtual DateTime ETADate { get; set; }

    public virtual string Core { get; set; }

    public virtual string CoreID { get; set; }

    public virtual string ProjectRunCloneGen { get; set; }

    public virtual double Credit { get; set; }

    public virtual int Completed { get; set; }

    public virtual int Failed { get; set; }

    public virtual string Username { get; set; }

    public virtual DateTime Assigned { get; set; }

    public virtual DateTime PreferredDeadline { get; set; }

    public IReadOnlyCollection<LogLine> CurrentLogLines { get; set; } = new List<LogLine>();

    public WorkUnitQueueItemCollection WorkUnitQueue { get; set; }

    public ValidationRuleErrors Errors { get; } = new();

    public static SlotModel CreateOfflineSlotModel(IClient client) =>
        new(client) { Status = SlotStatus.Offline };

    public static void ValidateRules(ICollection<SlotModel> slots, IPreferences preferences)
    {
        var rules = new IClientDataValidationRule[]
        {
            new ClientUsernameValidationRule(preferences),
            new ClientProjectIsDuplicateValidationRule(ClientProjectIsDuplicateValidationRule.FindDuplicateProjects(slots))
        };

        foreach (var slot in slots)
        {
            foreach (var rule in rules)
            {
                rule.Validate(slot);
            }
        }
    }
}

public class FahClientSlotModel : SlotModel, ICompletedFailedUnitsSource
{
    private PPDCalculation PPDCalculation => Preferences.Get<PPDCalculation>(Preference.PPDCalculation);

    private BonusCalculation BonusCalculation => Preferences.Get<BonusCalculation>(Preference.BonusCalculation);

    private bool ShowVersions => Preferences.Get<bool>(Preference.DisplayVersions);

    private int DecimalPlaces => Preferences.Get<int>(Preference.DecimalPlaces);

    private IPreferences Preferences { get; }

    private readonly SlotStatus _status;

    public FahClientSlotModel(IPreferences preferences, IClient client, SlotStatus status, int slotID) : base(client)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        _status = status;
        SlotID = slotID;
    }

    public override SlotStatus Status =>
        _status == SlotStatus.Running
            ? IsUsingBenchmarkFrameTime
                ? SlotStatus.RunningNoFrameTimes
                : SlotStatus.Running
            : _status;

    /// <summary>
    /// Current progress (percentage) of the unit
    /// </summary>
    public override int PercentComplete =>
        Status.IsRunning() || Status == SlotStatus.Paused
            ? WorkUnitModel.PercentComplete
            : 0;

    public override string SlotTypeString
    {
        get
        {
            if (SlotType == SlotType.Unknown)
            {
                return String.Empty;
            }

            var sb = new StringBuilder(SlotType.ToString());
            if (Threads.HasValue)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, ":{0}", Threads);
            }
            if (ShowVersions && !String.IsNullOrEmpty(Client.Platform?.ClientVersion))
            {
                sb.Append($" ({Client.Platform.ClientVersion})");
            }
            return sb.ToString();
        }
    }

    public override string Processor
    {
        get
        {
            if (ShowVersions && WorkUnitModel.WorkUnit.Platform?.Implementation
                    is WorkUnitPlatformImplementation.CUDA
                    or WorkUnitPlatformImplementation.OpenCL)
            {
                var platform = WorkUnitModel.WorkUnit.Platform;
                var sb = new StringBuilder(base.Processor);
                sb.Append($" ({platform.Implementation} {platform.DriverVersion})");
                return sb.ToString();
            }
            return base.Processor;
        }
    }

    private bool IsUsingBenchmarkFrameTime => WorkUnitModel.WorkUnit.HasProject() && WorkUnitModel.IsUsingBenchmarkFrameTime(PPDCalculation);

    /// <summary>
    /// Time per frame (TPF) of the unit
    /// </summary>
    public override TimeSpan TPF => Status.IsRunning() ? WorkUnitModel.GetFrameTime(PPDCalculation) : TimeSpan.Zero;

    /// <summary>
    /// Points per day (PPD) rating for this instance
    /// </summary>
    public override double PPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetPPD(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : 0;

    /// <summary>
    /// Units per day (UPD) rating for this instance
    /// </summary>
    public override double UPD => Status.IsRunning() ? Math.Round(WorkUnitModel.GetUPD(PPDCalculation), 3) : 0;

    /// <summary>
    /// Estimated time of arrival (ETA) for this protein
    /// </summary>
    public override TimeSpan ETA => Status.IsRunning() ? WorkUnitModel.GetEta(PPDCalculation) : TimeSpan.Zero;

    /// <summary>
    /// Estimated time of arrival (ETA) for this protein
    /// </summary>
    public override DateTime ETADate => Status.IsRunning() ? WorkUnitModel.GetEtaDate(PPDCalculation) : DateTime.MinValue;

    public override string Core
    {
        get
        {
            if (ShowVersions && WorkUnitModel.WorkUnit.CoreVersion != null)
            {
                return String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnitModel.CurrentProtein.Core, WorkUnitModel.WorkUnit.CoreVersion);
            }
            return WorkUnitModel.CurrentProtein.Core;
        }
    }

    public override string CoreID => WorkUnitModel.WorkUnit.CoreID;

    public override string ProjectRunCloneGen => WorkUnitModel.WorkUnit.ToShortProjectString();

    public override double Credit => Status.IsRunning() ? Math.Round(WorkUnitModel.GetCredit(Status, PPDCalculation, BonusCalculation), DecimalPlaces) : WorkUnitModel.CurrentProtein.Credit;

    public override int Completed =>
        Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
            ? TotalCompletedUnits
            : TotalRunCompletedUnits;

    public override int Failed =>
        Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
            ? TotalFailedUnits
            : TotalRunFailedUnits;

    /// <summary>
    /// Gets or sets the number of completed units since the last client start.
    /// </summary>
    public int TotalRunCompletedUnits { get; set; }

    /// <summary>
    /// Gets or sets the total number of completed units.
    /// </summary>
    public int TotalCompletedUnits { get; set; }

    /// <summary>
    /// Gets or sets the number of failed units since the last client start.
    /// </summary>
    public int TotalRunFailedUnits { get; set; }

    /// <summary>
    /// Gets or sets the total number of failed units.
    /// </summary>
    public int TotalFailedUnits { get; set; }

    /// <summary>
    /// Combined Folding ID and Team String
    /// </summary>
    public override string Username =>
        String.IsNullOrWhiteSpace(WorkUnitModel.WorkUnit.FoldingID)
            ? String.Empty
            : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnitModel.WorkUnit.FoldingID, WorkUnitModel.WorkUnit.Team);

    public override DateTime Assigned => WorkUnitModel.Assigned;

    public override DateTime PreferredDeadline => WorkUnitModel.PreferredDeadline;
}
