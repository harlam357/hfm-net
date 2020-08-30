using System;

namespace HFM.Proteins
{
    /// <summary>
    /// Provides extensions to the <see cref="Protein"/> class to assist with measuring work unit (protein) production with the <see cref="ProductionCalculator"/>.
    /// </summary>
    public static class ProteinProductionExtensions
    {
        /// <summary>
        /// Gets the units per day measurement based the given frame time and number of frames.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <returns>The units per day for the work unit.</returns>
        public static double GetUPD(this Protein protein, TimeSpan frameTime)
        {
            return ProductionCalculator.GetUPD(frameTime, protein.Frames);
        }

        /// <summary>
        /// Gets the production bonus multiplier.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The production bonus multiplier.</returns>
        public static double GetBonusMultiplier(this Protein protein, TimeSpan unitTime)
        {
            return ProductionCalculator.GetBonusMultiplier(protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);
        }

        /// <summary>
        /// Gets the credit measurement based the given work unit information and the unit completion time.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The credit for the work unit.</returns>
        public static double GetBonusCredit(this Protein protein, TimeSpan unitTime)
        {
            return ProductionCalculator.GetBonusCredit(protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);
        }

        /// <summary>
        /// Gets the points per day measurement based the given frame time and work unit credit.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <returns>The points per day for the work unit.</returns>
        public static double GetPPD(this Protein protein, TimeSpan frameTime)
        {
            return ProductionCalculator.GetPPD(frameTime, protein.Frames, protein.Credit);
        }

        /// <summary>
        /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The points per day for the work unit.</returns>
        public static double GetBonusPPD(this Protein protein, TimeSpan frameTime, TimeSpan unitTime)
        {
            return ProductionCalculator.GetBonusPPD(frameTime, protein.Frames, protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);
        }

        /// <summary>
        /// Gets all production measurements based the given frame time, work unit information, and the unit completion time.
        /// </summary>
        /// <param name="protein"></param>
        /// <param name="frameTime">The work unit frame time.</param>
        /// <param name="unitTime">The overall unit completion time.</param>
        /// <returns>The production measurements for the work unit.</returns> 
        public static ProteinProduction GetProteinProduction(this Protein protein, TimeSpan frameTime, TimeSpan unitTime)
        {
            return ProductionCalculator.GetProteinProduction(frameTime, protein.Frames, protein.Credit, protein.KFactor, protein.PreferredDays, protein.MaximumDays, unitTime);
        }
    }
}
