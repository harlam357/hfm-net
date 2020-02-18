/*
 * HFM.NET
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Globalization;
using System.Text;

using Castle.Core.Logging;

using HFM.Core.Client;
using HFM.Proteins;

namespace HFM.Core.WorkUnits
{
    public sealed class WorkUnitModel
    {
        private readonly IProteinBenchmarkService _benchmarkService;

        private WorkUnit _workUnit = new WorkUnit();
        
        public WorkUnit Data
        {
            get => _workUnit;
            set => _workUnit = value ?? new WorkUnit();
        }

        private Protein _currentProtein = new Protein();
        /// <summary>
        /// Class member containing info on the currently running protein
        /// </summary>
        public Protein CurrentProtein
        {
            get => _currentProtein;
            set => _currentProtein = value ?? new Protein();
        }

        public double ClientTimeOffset { get; set; }

        public bool UtcOffsetIsZero { get; set; }

        internal WorkUnitModel()
        {

        }

        public WorkUnitModel(IProteinBenchmarkService benchmarkService)
        {
            _benchmarkService = benchmarkService;
        }

        #region Unit Level Members

        /// <summary>
        /// Date/time the unit was downloaded
        /// </summary>
        public DateTime DownloadTime => GetTime(_workUnit.DownloadTime);

        /// <summary>
        /// Date/time the unit is due (preferred deadline)
        /// </summary>
        public DateTime DueTime => GetTime(_workUnit.DueTime);

        /// <summary>
        /// Date/time the unit finished
        /// </summary>
        public DateTime FinishedTime => GetTime(_workUnit.FinishedTime);

        private DateTime GetTime(DateTime dateTime)
        {
            if (dateTime.IsUnknown()) { return dateTime; }

            var offset = TimeSpan.FromMinutes(ClientTimeOffset);
            return UtcOffsetIsZero
                      ? dateTime.Subtract(offset)
                      : dateTime.ToLocalTime().Subtract(offset);
        }

        /// <summary>
        /// Work Unit Preferred Deadline
        /// </summary>
        public DateTime PreferredDeadline
        {
            get
            {
                if (_workUnit.DownloadTime.IsUnknown()) return _workUnit.DownloadTime;

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
                if (_workUnit.DownloadTime.IsUnknown()) return _workUnit.DownloadTime;

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
                if (_workUnit.CurrentFrame == null || _workUnit.CurrentFrame.ID < 0) return 0;
                return _workUnit.CurrentFrame.ID <= CurrentProtein.Frames ? _workUnit.CurrentFrame.ID : CurrentProtein.Frames;
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
        public int GetRawTime(PpdCalculationType calculationType)
        {
            switch (calculationType)
            {
                case PpdCalculationType.LastFrame:
                    return _workUnit.FramesObserved > 1 ? Convert.ToInt32(_workUnit.CurrentFrame.Duration.TotalSeconds) : 0;
                case PpdCalculationType.LastThreeFrames:
                    return _workUnit.FramesObserved > 3 ? GetDurationInSeconds(3) : 0;
                case PpdCalculationType.AllFrames:
                    return _workUnit.FramesObserved > 0 ? GetDurationInSeconds(_workUnit.FramesObserved) : 0;
                case PpdCalculationType.EffectiveRate:
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
            if (_workUnit.CurrentFrame == null) return 0;

            // Make sure FrameID is greater than 0 to avoid DivideByZeroException - Issue 34
            if (DownloadTime.IsUnknown() || _workUnit.CurrentFrame.ID <= 0) { return 0; }

            // Issue 92
            TimeSpan timeSinceUnitDownload = _workUnit.UnitRetrievalTime.Subtract(DownloadTime);
            return (Convert.ToInt32(timeSinceUnitDownload.TotalSeconds) / _workUnit.CurrentFrame.ID);
        }

        public bool IsUsingBenchmarkFrameTime(PpdCalculationType calculationType)
        {
            return GetRawTime(calculationType) == 0;
        }

        /// <summary>
        /// Time per frame (TPF) of the unit
        /// </summary>
        public TimeSpan GetFrameTime(PpdCalculationType calculationType)
        {
            int rawTime = GetRawTime(calculationType);
            if (rawTime != 0)
            {
                return TimeSpan.FromSeconds(rawTime);
            }

            var benchmark = _benchmarkService?.GetBenchmark(Data.SlotIdentifier, Data.ProjectID);
            return benchmark?.AverageFrameTime ?? TimeSpan.Zero;
        }

        /// <summary>
        /// Work unit credit
        /// </summary>
        public double GetCredit(SlotStatus status, PpdCalculationType calculationType, BonusCalculationType calculateBonus)
        {
            TimeSpan frameTime = GetFrameTime(calculationType);
            return GetCredit(GetUnitTimeByDownloadTime(frameTime), GetUnitTimeByFrameTime(frameTime), status, calculateBonus);
        }

        /// <summary>
        /// Units per day (UPD) rating for this unit
        /// </summary>
        public double GetUPD(PpdCalculationType calculationType)
        {
            return ProductionCalculator.GetUPD(GetFrameTime(calculationType), CurrentProtein.Frames);
        }

        /// <summary>
        /// Points per day (PPD) rating for this unit
        /// </summary>
        public double GetPPD(SlotStatus status, PpdCalculationType calculationType, BonusCalculationType calculateBonus)
        {
            TimeSpan frameTime = GetFrameTime(calculationType);
            return GetPPD(frameTime, GetUnitTimeByDownloadTime(frameTime), GetUnitTimeByFrameTime(frameTime), status, calculateBonus);
        }

        /// <summary>
        /// Estimated time of arrival (ETA) for this unit
        /// </summary>
        public TimeSpan GetEta(PpdCalculationType calculationType)
        {
            return GetEta(GetFrameTime(calculationType));
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
        public DateTime GetEtaDate(PpdCalculationType calculationType)
        {
            return _workUnit.UnitRetrievalTime.Add(GetEta(calculationType));
        }

        /// <summary>
        /// Specifies if All Frames have been Completed
        /// </summary>
        public bool AllFramesCompleted => CurrentProtein.Frames == FramesComplete;

        private TimeSpan GetUnitTimeByDownloadTime(TimeSpan frameTime)
        {
            if (DownloadTime.IsUnknown())
            {
                return TimeSpan.Zero;
            }
            if (FinishedTime.IsKnown())
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

            return _workUnit.UnitRetrievalTime.Add(eta).Subtract(DownloadTime);
        }

        private TimeSpan GetUnitTimeByFrameTime(TimeSpan frameTime)
        {
            return TimeSpan.FromSeconds(frameTime.TotalSeconds * CurrentProtein.Frames);
        }

        #endregion

        #region Calculate Credit and PPD

        private double GetCredit(TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime, SlotStatus status, BonusCalculationType calculateBonus)
        {
            if (ProteinIsUnknown(CurrentProtein))
            {
                return 0.0;
            }

            switch (calculateBonus)
            {
                case BonusCalculationType.DownloadTime when status == SlotStatus.RunningNoFrameTimes:
                    return CurrentProtein.GetBonusCredit(unitTimeByFrameTime);
                case BonusCalculationType.DownloadTime:
                    return CurrentProtein.GetBonusCredit(unitTimeByDownloadTime);
                case BonusCalculationType.FrameTime:
                    return CurrentProtein.GetBonusCredit(unitTimeByFrameTime);
                default:
                    return CurrentProtein.Credit;
            }
        }

        private double GetPPD(TimeSpan frameTime, TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime, SlotStatus status, BonusCalculationType calculateBonus)
        {
            if (ProteinIsUnknown(CurrentProtein))
            {
                return 0.0;
            }

            switch (calculateBonus)
            {
                case BonusCalculationType.DownloadTime when status == SlotStatus.RunningNoFrameTimes:
                    return CurrentProtein.GetBonusPPD(frameTime, unitTimeByFrameTime);
                case BonusCalculationType.DownloadTime:
                    return CurrentProtein.GetBonusPPD(frameTime, unitTimeByDownloadTime);
                case BonusCalculationType.FrameTime:
                    return CurrentProtein.GetBonusPPD(frameTime, unitTimeByFrameTime);
                default:
                    return CurrentProtein.GetPPD(frameTime);
            }
        }

        public void ShowProductionTrace(ILogger logger, string slotName, SlotStatus status, PpdCalculationType calculationType, BonusCalculationType bonusCalculationType)
        {
            // test the level
            if (!logger.IsDebugEnabled) return;

            if (ProteinIsUnknown(CurrentProtein))
            {
                logger.DebugFormat(Constants.ClientNameFormat, slotName, "Protein is unknown... 0 PPD.");
                return;
            }

            switch (bonusCalculationType)
            {
                case BonusCalculationType.DownloadTime:
                    logger.DebugFormat(Constants.ClientNameFormat, slotName,
                        status == SlotStatus.RunningNoFrameTimes
                            ? "Calculate Bonus PPD by Frame Time."
                            : "Calculate Bonus PPD by Download Time.");
                    break;
                case BonusCalculationType.FrameTime:
                    logger.DebugFormat(Constants.ClientNameFormat, slotName, "Calculate Bonus PPD by Frame Time.");
                    break;
                default:
                    logger.DebugFormat(Constants.ClientNameFormat, slotName, "Calculate Standard PPD.");
                    break;
            }

            TimeSpan frameTime = GetFrameTime(calculationType);
            var noBonusValues = CurrentProtein.GetProductionValues(frameTime, TimeSpan.Zero);
            TimeSpan unitTimeByDownloadTime = GetUnitTimeByDownloadTime(frameTime);
            var bonusByDownloadValues = CurrentProtein.GetProductionValues(frameTime, unitTimeByDownloadTime);
            TimeSpan unitTimeByFrameTime = GetUnitTimeByFrameTime(frameTime);
            var bonusByFrameValues = CurrentProtein.GetProductionValues(frameTime, unitTimeByFrameTime);
            logger.Debug(CreateProductionDebugOutput(Data.ToShortProjectString(), frameTime, CurrentProtein, noBonusValues,
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
            if (numberOfFrames < 1 || _workUnit.CurrentFrame == null)
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

            int frameId = _workUnit.CurrentFrame.ID;
            for (int i = 0; i < numberOfFrames; i++)
            {
                // Issue 199
                var frameData = _workUnit.GetFrameData(frameId);
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
