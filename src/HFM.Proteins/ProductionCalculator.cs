using System;

namespace HFM.Proteins
{
    /// <summary>
    /// Measures work unit (protein) production based on frame time, work unit information, and the unit completion time.
    /// </summary>
    public static class ProductionCalculator
    {
        private const int MaxDecimalPlaces = 5;

        /// <summary>
        /// Gets the units per day measurement based the given frame time and number of frames.
        /// </summary>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="frames">The number of frames in the work unit.</param>
        /// <returns>The units per day for the work unit.</returns>
        public static double GetUPD(TimeSpan frameTime, int frames)
        {
            double totalTime = frameTime.TotalSeconds * frames;
            if (totalTime <= 0.0)
            {
                return 0.0;
            }
            return frameTime.Equals(TimeSpan.Zero) ? 0.0 : 86400 / totalTime;
        }

        /// <summary>
        /// Gets the production bonus multiplier.
        /// </summary>
        /// <param name="kFactor">The KFactor assigned to the work unit.</param>
        /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
        /// <param name="maximumDays">The final deadline (in decimal days).</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The production bonus multiplier.</returns>
        public static double GetBonusMultiplier(double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
        {
            if (kFactor > 0 && unitTime > TimeSpan.Zero)
            {
                if (unitTime <= TimeSpan.FromDays(preferredDays))
                {
                    return Math.Round(Math.Sqrt(maximumDays * kFactor / unitTime.TotalDays), MaxDecimalPlaces);
                }
            }
            return 1.0;
        }

        /// <summary>
        /// Gets the credit measurement based the given work unit information and the unit completion time.
        /// </summary>
        /// <param name="credit">The base credit assigned to the work unit.</param>
        /// <param name="kFactor">The KFactor assigned to the work unit.</param>
        /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
        /// <param name="maximumDays">The final deadline (in decimal days).</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The credit for the work unit.</returns>
        public static double GetBonusCredit(double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
        {
            double bonusMulti = GetBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime);
            return Math.Round(credit * bonusMulti, MaxDecimalPlaces);
        }

        /// <summary>
        /// Gets the points per day measurement based the given frame time and work unit credit.
        /// </summary>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="frames">The number of frames in the work unit.</param>
        /// <param name="credit">The base credit assigned to the work unit.</param>
        /// <returns>The points per day for the work unit.</returns>
        public static double GetPPD(TimeSpan frameTime, int frames, double credit)
        {
            if (frameTime.Equals(TimeSpan.Zero)) return 0;

            double basePPD = GetUPD(frameTime, frames) * credit;
            return Math.Round(basePPD, MaxDecimalPlaces);
        }

        /// <summary>
        /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
        /// </summary>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="frames">The number of frames in the work unit.</param>
        /// <param name="credit">The base credit assigned to the work unit.</param>
        /// <param name="kFactor">The KFactor assigned to the work unit.</param>
        /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
        /// <param name="maximumDays">The final deadline (in decimal days).</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The points per day for the work unit.</returns>
        public static double GetBonusPPD(TimeSpan frameTime, int frames, double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
        {
            if (frameTime.Equals(TimeSpan.Zero)) return 0;

            double basePPD = GetUPD(frameTime, frames) * credit;
            double bonusMulti = GetBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime);
            return Math.Round(basePPD * bonusMulti, MaxDecimalPlaces);
        }

        /// <summary>
        /// Gets all protein production measurements based the given frame time, work unit information, and the unit completion time.
        /// </summary>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="frames">The number of frames in the work unit.</param>
        /// <param name="credit">The base credit assigned to the work unit.</param>
        /// <param name="kFactor">The KFactor assigned to the work unit.</param>
        /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
        /// <param name="maximumDays">The final deadline (in decimal days).</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The production measurements for the work unit.</returns> 
        public static ProteinProduction GetProteinProduction(TimeSpan frameTime, int frames, double credit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
        {
            var value = new ProteinProduction
            {
                UPD = GetUPD(frameTime, frames),
                Multiplier = GetBonusMultiplier(kFactor, preferredDays, maximumDays, unitTime),
                Credit = GetBonusCredit(credit, kFactor, preferredDays, maximumDays, unitTime),
                PPD = GetBonusPPD(frameTime, frames, credit, kFactor, preferredDays, maximumDays, unitTime)
            };
            return value;
        }
    }
}
