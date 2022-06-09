using System.Globalization;
using System.Text;

using HFM.Core.WorkUnits;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Core.Client;

// TODO: TimeFormatting should be a function of ClientData, not view grid painting
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

public class FahClientData : ClientData, IProteinBenchmarkDetailSource, ICompletedFailedUnitsSource, IFahClientCommand
{
    private PPDCalculation PPDCalculation => Preferences.Get<PPDCalculation>(Preference.PPDCalculation);

    private BonusCalculation BonusCalculation => Preferences.Get<BonusCalculation>(Preference.BonusCalculation);

    private bool ShowVersions => Preferences.Get<bool>(Preference.DisplayVersions);

    private int DecimalPlaces => Preferences.Get<int>(Preference.DecimalPlaces);

    public IPreferences Preferences { get; }

    public IClient Client { get; }

    public int SlotID { get; }

    public WorkUnitModel WorkUnitModel { get; set; }

    public FahClientData(IPreferences preferences, IClient client, SlotStatus status, int slotID)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
        Client = client ?? throw new ArgumentNullException(nameof(client));
        base.Status = status;
        SlotID = slotID;
        WorkUnitModel = WorkUnitModel.Empty(this);
    }

    public SlotDescription Description { get; set; }

    public WorkUnitQueueItemCollection WorkUnitQueue { get; set; }

    public SlotType SlotType => Description?.SlotType ?? default;

    string IProteinBenchmarkDetailSource.Processor => Description?.Processor;

    public int? Threads => Description is CPUSlotDescription cpu ? cpu.CPUThreads : null;

    public override SlotStatus Status =>
        base.Status == SlotStatus.Running
            ? IsUsingBenchmarkFrameTime
                ? SlotStatus.RunningNoFrameTimes
                : SlotStatus.Running
            : base.Status;

    private bool IsUsingBenchmarkFrameTime =>
        WorkUnitModel.WorkUnit.HasProject() && WorkUnitModel.IsUsingBenchmarkFrameTime(PPDCalculation);

    public override int PercentComplete =>
        Status.IsRunning() || Status == SlotStatus.Paused
            ? WorkUnitModel.PercentComplete
            : 0;

    public override string Name => SlotIdentifier.Name;

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
            string processor = Description?.Processor;
            var platform = WorkUnitModel.WorkUnit.Platform;
            bool showPlatform = platform?.Implementation
                is WorkUnitPlatformImplementation.CUDA
                or WorkUnitPlatformImplementation.OpenCL;

            return ShowVersions && showPlatform
                ? $"{processor} ({platform.Implementation} {platform.DriverVersion})"
                : processor;
        }
    }

    public override TimeSpan TPF =>
        Status.IsRunning()
            ? WorkUnitModel.GetFrameTime(PPDCalculation)
            : TimeSpan.Zero;

    public override double PPD =>
        Status.IsRunning()
            ? Math.Round(WorkUnitModel.GetPPD(Status, PPDCalculation, BonusCalculation), DecimalPlaces)
            : 0;

    public override double UPD =>
        Status.IsRunning()
            ? Math.Round(WorkUnitModel.GetUPD(PPDCalculation), 3)
            : 0;

    public override TimeSpan ETA =>
        Status.IsRunning()
            ? WorkUnitModel.GetEta(PPDCalculation)
            : TimeSpan.Zero;

    public override DateTime ETADate =>
        Status.IsRunning()
            ? WorkUnitModel.GetEtaDate(PPDCalculation)
            : DateTime.MinValue;

    public override string Core
    {
        get
        {
            string core = WorkUnitModel.CurrentProtein.Core;
            Version coreVersion = WorkUnitModel.WorkUnit.CoreVersion;

            return ShowVersions && coreVersion is not null
                ? String.Format(CultureInfo.InvariantCulture, "{0} ({1})", core, coreVersion)
                : core;
        }
    }

    public override string CoreID => WorkUnitModel.WorkUnit.CoreID;

    public override string ProjectRunCloneGen => WorkUnitModel.WorkUnit.ToShortProjectString();

    public override double Credit =>
        Status.IsRunning()
            ? Math.Round(WorkUnitModel.GetCredit(Status, PPDCalculation, BonusCalculation), DecimalPlaces)
            : WorkUnitModel.CurrentProtein.Credit;

    public override int Completed =>
        Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
            ? TotalCompletedUnits
            : TotalRunCompletedUnits;

    public override int Failed =>
        Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
            ? TotalFailedUnits
            : TotalRunFailedUnits;

    public override string Username =>
        String.IsNullOrWhiteSpace(WorkUnitModel.WorkUnit.FoldingID)
            ? String.Empty
            : String.Format(CultureInfo.InvariantCulture, "{0} ({1})", WorkUnitModel.WorkUnit.FoldingID, WorkUnitModel.WorkUnit.Team);

    public override DateTime Assigned => WorkUnitModel.Assigned;

    public override DateTime PreferredDeadline => WorkUnitModel.PreferredDeadline;

    // ICompletedFailedUnitsSource
    public int TotalRunCompletedUnits { get; set; }

    public int TotalCompletedUnits { get; set; }

    public int TotalRunFailedUnits { get; set; }

    public int TotalFailedUnits { get; set; }

    public override IProjectInfo ProjectInfo => WorkUnitModel.WorkUnit;

    public override IProductionProvider ProductionProvider => WorkUnitModel;

    public override string FoldingID => WorkUnitModel.WorkUnit.FoldingID;

    public override int Team => WorkUnitModel.WorkUnit.Team;

    public override ClientSettings Settings => Client.Settings;

    public override ClientPlatform Platform => Client.Platform;

    public override Protein CurrentProtein => WorkUnitModel.CurrentProtein;

    public override SlotIdentifier SlotIdentifier => new(Client.Settings.ClientIdentifier, SlotID);

    public override ProteinBenchmarkIdentifier BenchmarkIdentifier => WorkUnitModel.BenchmarkIdentifier;

    // IFahClientCommand
    public void Fold(int? slotID) => (Client as IFahClientCommand)?.Fold(slotID);

    public void Pause(int? slotID) => (Client as IFahClientCommand)?.Pause(slotID);

    public void Finish(int? slotID) => (Client as IFahClientCommand)?.Finish(slotID);
}
