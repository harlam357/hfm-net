using System.Globalization;
using System.Text;

using HFM.Core.Client;
using HFM.Core.Collections;
using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Proteins;

namespace HFM.Core.WorkUnits;

public class WorkUnitModel : IQueueItem
{
    public int ID => WorkUnit.ID;

    public SlotModel SlotModel { get; }

    public WorkUnit WorkUnit { get; }

    public Protein CurrentProtein { get; set; } = new();

    private readonly IProteinBenchmarkRepository _benchmarks;

    public static WorkUnitModel Empty(SlotModel slotModel) =>
        new(slotModel, new WorkUnit(), null);

    public WorkUnitModel(SlotModel slotModel, WorkUnit workUnit, IProteinBenchmarkRepository benchmarks)
    {
        SlotModel = slotModel ?? throw new ArgumentNullException(nameof(slotModel));
        WorkUnit = workUnit ?? throw new ArgumentNullException(nameof(workUnit));
        _benchmarks = benchmarks ?? NullProteinBenchmarkRepository.Instance;
    }

    public ProteinBenchmarkIdentifier BenchmarkIdentifier =>
        SlotModel is IProteinBenchmarkDetailSource detailSource
            ? new ProteinBenchmarkIdentifier(WorkUnit.ProjectID, detailSource.Processor, detailSource.Threads.GetValueOrDefault())
            : new ProteinBenchmarkIdentifier(WorkUnit.ProjectID);

    #region Unit Level Members

    /// <summary>
    /// Gets the work unit assigned date and time (Local).
    /// </summary>
    public DateTime Assigned => ToLocalTime(WorkUnit.Assigned);

    /// <summary>
    /// Gets the work unit timeout date and time (Local).
    /// </summary>
    public DateTime Timeout => ToLocalTime(WorkUnit.Timeout);

    /// <summary>
    /// Gets the work unit finished date and time (Local).
    /// </summary>
    public DateTime Finished => ToLocalTime(WorkUnit.Finished);

    private static DateTime ToLocalTime(DateTime dateTime)
    {
        if (dateTime.IsMinValue()) { return dateTime; }

        return dateTime.ToLocalTime();
    }

    /// <summary>
    /// Work Unit Preferred Deadline
    /// </summary>
    public DateTime PreferredDeadline
    {
        get
        {
            if (WorkUnit.Assigned.IsMinValue()) return WorkUnit.Assigned;

            return ProteinIsUnknown(CurrentProtein)
                ? Timeout
                : AdjustDeadlineForDaylightSavings(CurrentProtein.PreferredDays);
        }
    }

    /// <summary>
    /// Work Unit Preferred Deadline
    /// </summary>
    public DateTime FinalDeadline
    {
        get
        {
            if (WorkUnit.Assigned.IsMinValue()) return WorkUnit.Assigned;

            return ProteinIsUnknown(CurrentProtein)
                ? DateTime.MinValue
                : AdjustDeadlineForDaylightSavings(CurrentProtein.MaximumDays);
        }
    }

    private DateTime AdjustDeadlineForDaylightSavings(double days)
    {
        DateTime deadline = Assigned.AddDays(days);

        // download time is DST
        if (Assigned.IsDaylightSavingTime())
        {
            if (!deadline.IsDaylightSavingTime())
            {
                return deadline.Subtract(TimeSpan.FromHours(1));
            }
        }
        else // not DST
        {
            if (deadline.IsDaylightSavingTime())
            {
                return deadline.AddHours(1);
            }
        }

        return deadline;
    }

    #endregion

    #region Frames/Percent Completed Unit Level Members

    /// <summary>
    /// Frame progress of the unit
    /// </summary>
    public int FramesComplete
    {
        get
        {
            if (WorkUnit.CurrentFrame == null || WorkUnit.CurrentFrame.ID < 0) return 0;
            return WorkUnit.CurrentFrame.ID <= CurrentProtein.Frames ? WorkUnit.CurrentFrame.ID : CurrentProtein.Frames;
        }
    }

    /// <summary>
    /// Current progress (percentage) of the unit
    /// </summary>
    public int PercentComplete => FramesComplete * 100 / CurrentProtein.Frames;

    #endregion

    #region Production Value Properties

    /// <summary>
    /// Frame Time per section based on current PPD calculation setting (readonly)
    /// </summary>
    public int GetRawTime(PPDCalculation ppdCalculation)
    {
        switch (ppdCalculation)
        {
            case PPDCalculation.LastFrame:
                return WorkUnit.FramesObserved > 1 ? Convert.ToInt32(WorkUnit.CurrentFrame.Duration.TotalSeconds) : 0;
            case PPDCalculation.LastThreeFrames:
                return WorkUnit.FramesObserved > 3 ? GetDurationInSeconds(3) : 0;
            case PPDCalculation.AllFrames:
                return WorkUnit.FramesObserved > 0 ? GetDurationInSeconds(WorkUnit.FramesObserved) : 0;
            case PPDCalculation.EffectiveRate:
                return GetRawTimePerUnitDownload();
        }

        return 0;
    }

    private int GetRawTimePerUnitDownload()
    {
        //if (_unitInfo.FramesObserved <= 0) return 0;

        // was making the check on FramesObserved above - I believe this was in an
        // attempt to validate that an object would be assigned to CurrentFrame.
        // if this is truly what we're trying to validate, it would make more sense
        // to simply check CurrentFrame for null - 2/7/11
        if (WorkUnit.CurrentFrame == null) return 0;

        // Make sure FrameID is greater than 0 to avoid DivideByZeroException - Issue 34
        if (Assigned.IsMinValue() || WorkUnit.CurrentFrame.ID <= 0) { return 0; }

        // Issue 92
        TimeSpan timeSinceUnitDownload = WorkUnit.UnitRetrievalTime.Subtract(Assigned);
        return (Convert.ToInt32(timeSinceUnitDownload.TotalSeconds) / WorkUnit.CurrentFrame.ID);
    }

    public bool IsUsingBenchmarkFrameTime(PPDCalculation ppdCalculation)
    {
        return GetRawTime(ppdCalculation) == 0;
    }

    /// <summary>
    /// Time per frame (TPF) of the unit
    /// </summary>
    public TimeSpan GetFrameTime(PPDCalculation ppdCalculation)
    {
        int rawTime = GetRawTime(ppdCalculation);
        if (rawTime != 0)
        {
            return TimeSpan.FromSeconds(rawTime);
        }

        var benchmark = _benchmarks.GetBenchmark(SlotModel.SlotIdentifier, BenchmarkIdentifier);
        return benchmark?.AverageFrameTime ?? TimeSpan.Zero;
    }

    /// <summary>
    /// Work unit credit
    /// </summary>
    public double GetCredit(SlotStatus status, PPDCalculation ppdCalculation, BonusCalculation calculateBonus)
    {
        TimeSpan frameTime = GetFrameTime(ppdCalculation);
        return GetCredit(GetUnitTimeByDownloadTime(frameTime), GetUnitTimeByFrameTime(frameTime), status, calculateBonus);
    }

    /// <summary>
    /// Units per day (UPD) rating for this unit
    /// </summary>
    public double GetUPD(PPDCalculation ppdCalculation)
    {
        return ProductionCalculator.GetUPD(GetFrameTime(ppdCalculation), CurrentProtein.Frames);
    }

    /// <summary>
    /// Points per day (PPD) rating for this unit
    /// </summary>
    public double GetPPD(SlotStatus status, PPDCalculation ppdCalculation, BonusCalculation calculateBonus)
    {
        TimeSpan frameTime = GetFrameTime(ppdCalculation);
        return GetPPD(frameTime, GetUnitTimeByDownloadTime(frameTime), GetUnitTimeByFrameTime(frameTime), status, calculateBonus);
    }

    /// <summary>
    /// Estimated time of arrival (ETA) for this unit
    /// </summary>
    public TimeSpan GetEta(PPDCalculation ppdCalculation)
    {
        return GetEta(GetFrameTime(ppdCalculation));
    }

    /// <summary>
    /// Estimated time of arrival (ETA) for this unit
    /// </summary>
    private TimeSpan GetEta(TimeSpan frameTime)
    {
        return new TimeSpan((CurrentProtein.Frames - FramesComplete) * frameTime.Ticks);
    }

    /// <summary>
    /// Estimated time of arrival (ETA) for this unit
    /// </summary>
    public DateTime GetEtaDate(PPDCalculation ppdCalculation)
    {
        return WorkUnit.UnitRetrievalTime.Add(GetEta(ppdCalculation));
    }

    /// <summary>
    /// Specifies if All Frames have been Completed
    /// </summary>
    public bool AllFramesCompleted => CurrentProtein.Frames == FramesComplete;

    private TimeSpan GetUnitTimeByDownloadTime(TimeSpan frameTime)
    {
        if (Assigned.IsMinValue())
        {
            return TimeSpan.Zero;
        }
        if (!Finished.IsMinValue())
        {
            return Finished.Subtract(Assigned);
        }

        // If ETA is Zero and AllFramesAreCompleted == false, the Unit Time
        // will be Zero.  Otherwise, it will be given a value of the
        // (UnitRetrievalTime plus ETA) minus the DownloadTime.
        TimeSpan eta = GetEta(frameTime);
        if (eta == TimeSpan.Zero && AllFramesCompleted == false)
        {
            return TimeSpan.Zero;
        }

        return WorkUnit.UnitRetrievalTime.Add(eta).Subtract(Assigned);
    }

    private TimeSpan GetUnitTimeByFrameTime(TimeSpan frameTime)
    {
        return TimeSpan.FromSeconds(frameTime.TotalSeconds * CurrentProtein.Frames);
    }

    #endregion

    #region Calculate Credit and PPD

    private double GetCredit(TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime, SlotStatus status, BonusCalculation calculateBonus)
    {
        if (ProteinIsUnknown(CurrentProtein))
        {
            return 0.0;
        }

        switch (calculateBonus)
        {
            case BonusCalculation.DownloadTime when status == SlotStatus.RunningNoFrameTimes:
                return CurrentProtein.GetBonusCredit(unitTimeByFrameTime);
            case BonusCalculation.DownloadTime:
                return CurrentProtein.GetBonusCredit(unitTimeByDownloadTime);
            case BonusCalculation.FrameTime:
                return CurrentProtein.GetBonusCredit(unitTimeByFrameTime);
            default:
                return CurrentProtein.Credit;
        }
    }

    private double GetPPD(TimeSpan frameTime, TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime, SlotStatus status, BonusCalculation calculateBonus)
    {
        if (ProteinIsUnknown(CurrentProtein))
        {
            return 0.0;
        }

        switch (calculateBonus)
        {
            case BonusCalculation.DownloadTime when status == SlotStatus.RunningNoFrameTimes:
                return CurrentProtein.GetBonusPPD(frameTime, unitTimeByFrameTime);
            case BonusCalculation.DownloadTime:
                return CurrentProtein.GetBonusPPD(frameTime, unitTimeByDownloadTime);
            case BonusCalculation.FrameTime:
                return CurrentProtein.GetBonusPPD(frameTime, unitTimeByFrameTime);
            default:
                return CurrentProtein.GetPPD(frameTime);
        }
    }

    public void ShowProductionTrace(ILogger logger, string slotName, SlotStatus status, PPDCalculation ppdCalculation, BonusCalculation bonusCalculation)
    {
        // test the level
        if (!logger.IsDebugEnabled) return;

        if (ProteinIsUnknown(CurrentProtein))
        {
            logger.Debug(String.Format(Logger.NameFormat, slotName, "Protein is unknown... 0 PPD."));
            return;
        }

        switch (bonusCalculation)
        {
            case BonusCalculation.DownloadTime:
                logger.Debug(String.Format(Logger.NameFormat, slotName,
                    status == SlotStatus.RunningNoFrameTimes
                        ? "Calculate Bonus PPD by Frame Time."
                        : "Calculate Bonus PPD by Download Time."));
                break;
            case BonusCalculation.FrameTime:
                logger.Debug(String.Format(Logger.NameFormat, slotName, "Calculate Bonus PPD by Frame Time."));
                break;
            default:
                logger.Debug(String.Format(Logger.NameFormat, slotName, "Calculate Standard PPD."));
                break;
        }

        TimeSpan frameTime = GetFrameTime(ppdCalculation);
        var noBonus = CurrentProtein.GetProteinProduction(frameTime, TimeSpan.Zero);
        TimeSpan unitTimeByDownloadTime = GetUnitTimeByDownloadTime(frameTime);
        var bonusByDownload = CurrentProtein.GetProteinProduction(frameTime, unitTimeByDownloadTime);
        TimeSpan unitTimeByFrameTime = GetUnitTimeByFrameTime(frameTime);
        var bonusByFrame = CurrentProtein.GetProteinProduction(frameTime, unitTimeByFrameTime);
        logger.Debug(CreateProductionDebugOutput(WorkUnit.ToShortProjectString(), frameTime, CurrentProtein, noBonus,
            unitTimeByDownloadTime, bonusByDownload,
            unitTimeByFrameTime, bonusByFrame));
    }

    private static string CreateProductionDebugOutput(string project, TimeSpan frameTime, Protein protein, ProteinProduction noBonus,
        TimeSpan unitTimeByDownloadTime, ProteinProduction bonusByDownload,
        TimeSpan unitTimeByFrameTime, ProteinProduction bonusByFrame)
    {
        var sb = new StringBuilder();
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " ******* Project: {0} *******", project));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Frames: {0}", protein.Frames));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Credit: {0}", protein.Credit));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "         KFactor: {0}", protein.KFactor));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "  Preferred Time: {0}", TimeSpan.FromDays(protein.PreferredDays)));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "    Maximum Time: {0}", TimeSpan.FromDays(protein.MaximumDays)));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " **** Production: {0} ****", "No Bonus"));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "      Frame Time: {0}", frameTime));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             UPD: {0}", noBonus.UPD));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", noBonus.PPD));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " **** Production: {0} ****", "Bonus by Download Time"));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "       Unit Time: {0}", unitTimeByDownloadTime));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "           Multi: {0}", bonusByDownload.Multiplier));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Credit: {0}", bonusByDownload.Credit));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", bonusByDownload.PPD));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " **** Production: {0} ****", "Bonus by Frame Time"));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "       Unit Time: {0}", unitTimeByFrameTime));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "           Multi: {0}", bonusByFrame.Multiplier));
        sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Credit: {0}", bonusByFrame.Credit));
        sb.Append(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", bonusByFrame.PPD));
        return sb.ToString();
    }

    #endregion

    #region Calculate Production Variations

    /// <summary>
    /// Get the average duration over the specified number of most recent frames
    /// </summary>
    /// <param name="numberOfFrames">Number of most recent frames</param>
    private int GetDurationInSeconds(int numberOfFrames)
    {
        // the numberOfFrames must be 1 or greater
        // if CurrentFrame is null then no frames have been captured yet
        if (numberOfFrames < 1 || WorkUnit.CurrentFrame == null)
        {
            return 0;
        }

        // init return value
        int averageSeconds = 0;

        // Make sure we only add frame durations greater than a Zero TimeSpan
        // The first frame will always have a Zero TimeSpan for frame duration
        // we don't want to take this frame into account when calculating 'AllFrames'
        TimeSpan totalTime = TimeSpan.Zero;
        int countFrames = 0;

        int frameId = WorkUnit.CurrentFrame.ID;
        for (int i = 0; i < numberOfFrames; i++)
        {
            var frame = WorkUnit.GetFrame(frameId);
            if (frame != null && frame.Duration > TimeSpan.Zero)
            {
                totalTime = totalTime.Add(frame.Duration);
                countFrames++;
            }
            frameId--;
        }

        if (countFrames > 0)
        {
            averageSeconds = Convert.ToInt32(totalTime.TotalSeconds) / countFrames;
        }

        return averageSeconds;
    }

    #endregion

    internal static bool ProteinIsUnknown(Protein protein)
    {
        return protein.ProjectNumber == 0;
    }
}

public class WorkUnitModelCollection : QueueItemCollection<WorkUnitModel>
{
    public WorkUnitModelCollection()
    {

    }

    public WorkUnitModelCollection(IEnumerable<WorkUnitModel> workUnits)
    {
        foreach (var workUnit in workUnits)
        {
            Add(workUnit);
        }
    }
}
