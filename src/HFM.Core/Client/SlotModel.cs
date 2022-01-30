using System.Globalization;
using System.Text;

using HFM.Core.WorkUnits;
using HFM.Log;
using HFM.Preferences;

namespace HFM.Core.Client
{
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

    public class SlotModel
    {
        public SlotIdentifier SlotIdentifier => new(Client.Settings.ClientIdentifier, SlotID);

        public IClient Client { get; }

        public WorkUnitModel WorkUnitModel { get; set; }

        public SlotModel(IClient client)
        {
            Client = client ?? throw new ArgumentNullException(nameof(client));
            WorkUnitModel = new WorkUnitModel(this);
        }

        public int SlotID { get; set; } = SlotIdentifier.NoSlotID;

        public virtual SlotStatus Status { get; set; }

        public virtual int PercentComplete { get; set; }

        public virtual string Name
        {
            get => SlotIdentifier.Name;
            set => _ = value;
        }

        public SlotType SlotType { get; set; }

        public virtual string SlotTypeString { get; set; }

        public virtual string Processor { get; set; }

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

        public bool ProjectIsDuplicate { get; set; }

        public bool UsernameOk { get; set; }

        public static SlotModel CreateOfflineSlotModel(IClient client) =>
            new SlotModel(client) { Status = SlotStatus.Offline };

        public static void ValidateRules(ICollection<SlotModel> slots)
        {
            var rules = new ISlotModelRule[]
            {
                new SlotModelUsernameRule(),
                new SlotModelProjectIsDuplicateRule(SlotModelProjectIsDuplicateRule.FindDuplicateProjects(slots))
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

    public class FahClientSlotModel : SlotModel, IProteinBenchmarkDetailSource, ICompletedFailedUnitsSource
    {
        private PPDCalculation PPDCalculation => Client.Preferences.Get<PPDCalculation>(Preference.PPDCalculation);

        private BonusCalculation BonusCalculation => Client.Preferences.Get<BonusCalculation>(Preference.BonusCalculation);

        private bool ShowVersions => Client.Preferences.Get<bool>(Preference.DisplayVersions);

        private int DecimalPlaces => Client.Preferences.Get<int>(Preference.DecimalPlaces);

        private readonly SlotStatus _status;

        public FahClientSlotModel(IClient client, SlotStatus status, int slotID, SlotType slotType) : base(client)
        {
            _status = status;
            SlotID = slotID;
            SlotType = slotType;
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
                if (ShowVersions && !String.IsNullOrEmpty(Client.ClientVersion))
                {
                    sb.Append($" ({Client.ClientVersion})");
                }
                return sb.ToString();
            }
        }

        public int? Threads { get; set; }

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
            Client.Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
              ? TotalCompletedUnits
              : TotalRunCompletedUnits;

        public override int Failed =>
            Client.Preferences.Get<UnitTotalsType>(Preference.UnitTotals) == UnitTotalsType.All
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

    public interface ISlotModelRule
    {
        void Validate(SlotModel slotModel);
    }

    public class SlotModelUsernameRule : ISlotModelRule
    {
        public void Validate(SlotModel slotModel)
        {
            if (FoldingIdentityIsDefault(slotModel.WorkUnitModel.WorkUnit))
            {
                slotModel.UsernameOk = true;
            }
            else if (!slotModel.Status.IsOnline())
            {
                slotModel.UsernameOk = true;
            }
            else
            {
                slotModel.UsernameOk = slotModel.WorkUnitModel.WorkUnit.FoldingID == slotModel.Client.Preferences.Get<string>(Preference.StanfordId) &&
                                       slotModel.WorkUnitModel.WorkUnit.Team == slotModel.Client.Preferences.Get<int>(Preference.TeamId);
            }
        }

        private static bool FoldingIdentityIsDefault(WorkUnit workUnit) =>
            (String.IsNullOrWhiteSpace(workUnit.FoldingID) || workUnit.FoldingID == Unknown.Value) && workUnit.Team == default;
    }

    public class SlotModelProjectIsDuplicateRule : ISlotModelRule
    {
        private readonly ICollection<string> _duplicateProjects;

        public SlotModelProjectIsDuplicateRule(ICollection<string> duplicateProjects)
        {
            _duplicateProjects = duplicateProjects ?? throw new ArgumentNullException(nameof(duplicateProjects));
        }

        public static ICollection<string> FindDuplicateProjects(ICollection<SlotModel> slots)
        {
            return slots.GroupBy(x => x.WorkUnitModel.WorkUnit.ToShortProjectString())
                .Where(g => g.Count() > 1 && g.First().WorkUnitModel.WorkUnit.HasProject())
                .Select(g => g.Key)
                .ToList();
        }

        public void Validate(SlotModel slotModel)
        {
            slotModel.ProjectIsDuplicate = _duplicateProjects.Contains(slotModel.WorkUnitModel.WorkUnit.ToShortProjectString());
        }
    }
}
