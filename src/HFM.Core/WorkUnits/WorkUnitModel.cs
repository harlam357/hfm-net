
using System;
using System.Globalization;
using System.Text;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    public class WorkUnitModel
    {
        public SlotModel SlotModel { get; }

        public WorkUnit WorkUnit { get; }

        public Protein CurrentProtein { get; set; } = new Protein();

        public WorkUnitModel(SlotModel slotModel) : this(slotModel, new WorkUnit())
        {

        }

        public WorkUnitModel(SlotModel slotModel, WorkUnit workUnit)
        {
            SlotModel = slotModel;
            WorkUnit = workUnit;
        }

        #region Unit Level Members

        /// <summary>
        /// Date/time the unit was downloaded
        /// </summary>
        public DateTime DownloadTime => GetTime(WorkUnit.DownloadTime);

        /// <summary>
        /// Date/time the unit is due (preferred deadline)
        /// </summary>
        public DateTime DueTime => GetTime(WorkUnit.DueTime);

        /// <summary>
        /// Date/time the unit finished
        /// </summary>
        public DateTime FinishedTime => GetTime(WorkUnit.FinishedTime);

        private DateTime GetTime(DateTime dateTime)
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
                if (WorkUnit.DownloadTime.IsMinValue()) return WorkUnit.DownloadTime;

                return ProteinIsUnknown(CurrentProtein)
                          ? DueTime
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
                if (WorkUnit.DownloadTime.IsMinValue()) return WorkUnit.DownloadTime;

                return ProteinIsUnknown(CurrentProtein)
                          ? DateTime.MinValue
                          : AdjustDeadlineForDaylightSavings(CurrentProtein.MaximumDays);
            }
        }

        private DateTime AdjustDeadlineForDaylightSavings(double days)
        {
            DateTime deadline = DownloadTime.AddDays(days);

            // download time is DST
            if (DownloadTime.IsDaylightSavingTime())
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
            if (DownloadTime.IsMinValue() || WorkUnit.CurrentFrame.ID <= 0) { return 0; }

            // Issue 92
            TimeSpan timeSinceUnitDownload = WorkUnit.UnitRetrievalTime.Subtract(DownloadTime);
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

            var benchmark = SlotModel.Client.BenchmarkService.GetBenchmark(SlotModel.SlotIdentifier, WorkUnit.ProjectID);
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
            if (DownloadTime.IsMinValue())
            {
                return TimeSpan.Zero;
            }
            if (!FinishedTime.IsMinValue())
            {
                return FinishedTime.Subtract(DownloadTime);
            }

            // If ETA is Zero and AllFramesAreCompleted == false, the Unit Time
            // will be Zero.  Otherwise, it will be given a value of the
            // (UnitRetrievalTime plus ETA) minus the DownloadTime.
            TimeSpan eta = GetEta(frameTime);
            if (eta == TimeSpan.Zero && AllFramesCompleted == false)
            {
                return TimeSpan.Zero;
            }

            return WorkUnit.UnitRetrievalTime.Add(eta).Subtract(DownloadTime);
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
            var noBonusValues = CurrentProtein.GetProductionValues(frameTime, TimeSpan.Zero);
            TimeSpan unitTimeByDownloadTime = GetUnitTimeByDownloadTime(frameTime);
            var bonusByDownloadValues = CurrentProtein.GetProductionValues(frameTime, unitTimeByDownloadTime);
            TimeSpan unitTimeByFrameTime = GetUnitTimeByFrameTime(frameTime);
            var bonusByFrameValues = CurrentProtein.GetProductionValues(frameTime, unitTimeByFrameTime);
            logger.Debug(CreateProductionDebugOutput(WorkUnit.ToShortProjectString(), frameTime, CurrentProtein, noBonusValues,
                                                           unitTimeByDownloadTime, bonusByDownloadValues,
                                                           unitTimeByFrameTime, bonusByFrameValues));
        }

        private static string CreateProductionDebugOutput(string project, TimeSpan frameTime, Protein protein, ProductionValues noBonusValues,
                                                          TimeSpan unitTimeByDownloadTime, ProductionValues bonusByDownloadValues,
                                                          TimeSpan unitTimeByFrameTime, ProductionValues bonusByFrameValues)
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
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             UPD: {0}", noBonusValues.UPD));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", noBonusValues.PPD));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " **** Production: {0} ****", "Bonus by Download Time"));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "       Unit Time: {0}", unitTimeByDownloadTime));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "           Multi: {0}", bonusByDownloadValues.Multiplier));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Credit: {0}", bonusByDownloadValues.Credit));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", bonusByDownloadValues.PPD));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, " **** Production: {0} ****", "Bonus by Frame Time"));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "       Unit Time: {0}", unitTimeByFrameTime));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "           Multi: {0}", bonusByFrameValues.Multiplier));
            sb.AppendLine(String.Format(CultureInfo.CurrentCulture, "          Credit: {0}", bonusByFrameValues.Credit));
            sb.Append(String.Format(CultureInfo.CurrentCulture, "             PPD: {0}", bonusByFrameValues.PPD));
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
            // we don't want to take this frame into account when calculating 'AllFrames' - Issue 23
            TimeSpan totalTime = TimeSpan.Zero;
            int countFrames = 0;

            int frameId = WorkUnit.CurrentFrame.ID;
            for (int i = 0; i < numberOfFrames; i++)
            {
                // Issue 199
                var frameData = WorkUnit.GetFrameData(frameId);
                if (frameData != null && frameData.Duration > TimeSpan.Zero)
                {
                    totalTime = totalTime.Add(frameData.Duration);
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
}
