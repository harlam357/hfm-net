
using System;
using System.Globalization;
using System.Text;

namespace HFM.Proteins
{
   public static class ProductionCalculator
   {
      private const int MaxDecimalPlaces = 5;

      /// <summary>
      /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="protein">The work unit information.</param>
      /// <returns>The points per day for the work unit.</returns>
      public static double GetPPD(TimeSpan frameTime, Protein protein)
      {
         return GetPPD(frameTime, protein, false);
      }

      /// <summary>
      /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="protein">The work unit information.</param>
      /// <param name="calculateUnitTimeByFrameTime"></param>
      /// <returns>The points per day for the work unit.</returns>
      public static double GetPPD(TimeSpan frameTime, Protein protein, bool calculateUnitTimeByFrameTime)
      {
         return GetPPD(frameTime,
                       protein.Frames,
                       protein.Credit,
                       protein.KFactor,
                       protein.PreferredDays,
                       protein.MaximumDays,
                       calculateUnitTimeByFrameTime ? TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames) : TimeSpan.Zero);
      }

      /// <summary>
      /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="protein">The work unit information.</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The points per day for the work unit.</returns>
      public static double GetPPD(TimeSpan frameTime, Protein protein, TimeSpan unitTime)
      {
         return GetPPD(frameTime,
                       protein.Frames,
                       protein.Credit,
                       protein.KFactor,
                       protein.PreferredDays,
                       protein.MaximumDays,
                       unitTime);
      }

      /// <summary>
      /// Gets the points per day measurement based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="frames">The number of frames in the work unit.</param>
      /// <param name="baseCredit">The base credit assigned to the work unit.</param>
      /// <param name="kFactor">The KFactor assigned to the work unit.</param>
      /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
      /// <param name="maximumDays">The final deadline (in decimal days).</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The points per day for the work unit.</returns>
      public static double GetPPD(TimeSpan frameTime, int frames, double baseCredit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
      {
         if (frameTime.Equals(TimeSpan.Zero)) return 0;

         double basePPD = GetUPD(frameTime, frames) * baseCredit;
         double bonusMulti = GetMultiplier(kFactor, preferredDays, maximumDays, unitTime);
         double bonusPPD = Math.Round((basePPD * bonusMulti), MaxDecimalPlaces);

         return bonusPPD;
      }

      /// <summary>
      /// Gets the units per day measurement based the given frame time and number of frames.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="frames">The number of frames in the work unit.</param>
      /// <returns>The units per day for the work unit.</returns>
      public static double GetUPD(TimeSpan frameTime, int frames)
      {
         double totalTime = (frameTime.TotalSeconds * frames);
         if (totalTime <= 0.0)
         {
            return 0.0;
         }
         return frameTime.Equals(TimeSpan.Zero) ? 0.0 : 86400 / totalTime;
      }

      /// <summary>
      /// Gets the credit measurement based the given work unit information and the unit completion time.
      /// </summary>
      /// <param name="protein">The work unit information.</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The credit for the work unit.</returns>
      public static double GetCredit(Protein protein, TimeSpan unitTime)
      {
         return GetCredit(protein.Credit,
                          protein.KFactor,
                          protein.PreferredDays,
                          protein.MaximumDays,
                          unitTime);
      }

      /// <summary>
      /// Gets the credit measurement based the given work unit information and the unit completion time.
      /// </summary>
      /// <param name="baseCredit">The base credit assigned to the work unit.</param>
      /// <param name="kFactor">The KFactor assigned to the work unit.</param>
      /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
      /// <param name="maximumDays">The final deadline (in decimal days).</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The credit for the work unit.</returns>
      public static double GetCredit(double baseCredit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
      {
         double bonusMulti = GetMultiplier(kFactor, preferredDays, maximumDays, unitTime);
         return Math.Round((baseCredit * bonusMulti), MaxDecimalPlaces);
      }

      /// <summary>
      /// Gets the production bonus multiplier.
      /// </summary>
      /// <param name="protein">The work unit information.</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The production bonus multiplier.</returns>
      public static double GetMultiplier(Protein protein, TimeSpan unitTime)
      {
         return GetMultiplier(protein.KFactor,
                              protein.PreferredDays,
                              protein.MaximumDays,
                              unitTime);
      }

      /// <summary>
      /// Gets the production bonus multiplier.
      /// </summary>
      /// <param name="kFactor">The KFactor assigned to the work unit.</param>
      /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
      /// <param name="maximumDays">The final deadline (in decimal days).</param>
      /// <param name="unitTime">The overall unit completion time.</param>
      /// <returns>The production bonus multiplier.</returns>
      public static double GetMultiplier(double kFactor, double preferredDays, double maximumDays, TimeSpan unitTime)
      {
         // Make sure the given TimeSpan is not negative
         if (kFactor > 0 && unitTime > TimeSpan.Zero)
         {
            if (unitTime <= TimeSpan.FromDays(preferredDays))
            {
               return Math.Round(Math.Sqrt((maximumDays * kFactor) / unitTime.TotalDays), MaxDecimalPlaces);
            }
         }

         return 1;
      }

      /// <summary>
      /// Gets all production measurements based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="protein">The work unit information.</param>
      /// <param name="unitTimeByDownloadTime">The overall unit completion time based on the unit's download time.</param>
      /// <param name="unitTimeByFrameTime">The overall unit completion time based on the unit's current frame time.</param>
      /// <returns>The production measurements for the work unit.</returns> 
      public static ProductionValues GetProductionValues(TimeSpan frameTime, Protein protein, TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime)
      {
         return GetProductionValues(frameTime,
                                    protein.Frames,
                                    protein.Credit,
                                    protein.KFactor,
                                    protein.PreferredDays,
                                    protein.MaximumDays,
                                    unitTimeByDownloadTime,
                                    unitTimeByFrameTime);
      }

      /// <summary>
      /// Gets all production measurements based the given frame time, work unit information, and the unit completion time.
      /// </summary>
      /// <param name="frameTime">The work unit frame time.</param>
      /// <param name="frames">The number of frames in the work unit.</param>
      /// <param name="baseCredit">The base credit assigned to the work unit.</param>
      /// <param name="kFactor">The KFactor assigned to the work unit.</param>
      /// <param name="preferredDays">The preferred deadline (in decimal days).</param>
      /// <param name="maximumDays">The final deadline (in decimal days).</param>
      /// <param name="unitTimeByDownloadTime">The overall unit completion time based on the unit's download time.</param>
      /// <param name="unitTimeByFrameTime">The overall unit completion time based on the unit's current frame time.</param>
      /// <returns>The production measurements for the work unit.</returns> 
      public static ProductionValues GetProductionValues(TimeSpan frameTime, int frames, double baseCredit, double kFactor, double preferredDays, double maximumDays, TimeSpan unitTimeByDownloadTime, TimeSpan unitTimeByFrameTime)
      {
         var value = new ProductionValues
         {
            TimePerFrame = frameTime,
            BaseCredit = baseCredit,
            BasePPD = GetPPD(frameTime, frames, baseCredit, kFactor, preferredDays, maximumDays, TimeSpan.Zero),
            PreferredTime = TimeSpan.FromDays(preferredDays),
            MaximumTime = TimeSpan.FromDays(maximumDays),
            KFactor = kFactor,
            UnitTimeByDownloadTime = unitTimeByDownloadTime,
            DownloadTimeBonusMulti = GetMultiplier(kFactor, preferredDays, maximumDays, unitTimeByDownloadTime),
            DownloadTimeBonusCredit = GetCredit(baseCredit, kFactor, preferredDays, maximumDays, unitTimeByDownloadTime),
            DownloadTimeBonusPPD = GetPPD(frameTime, frames, baseCredit, kFactor, preferredDays, maximumDays, unitTimeByDownloadTime),
            UnitTimeByFrameTime = unitTimeByFrameTime,
            FrameTimeBonusMulti = GetMultiplier(kFactor, preferredDays, maximumDays, unitTimeByFrameTime),
            FrameTimeBonusCredit = GetCredit(baseCredit, kFactor, preferredDays, maximumDays, unitTimeByFrameTime),
            FrameTimeBonusPPD = GetPPD(frameTime, frames, baseCredit, kFactor, preferredDays, maximumDays, unitTimeByFrameTime)
         };
         return value;
      }
   }

   public struct ProductionValues
   {
      public TimeSpan TimePerFrame { get; set; }

      public double BaseCredit { get; set; }

      public double BasePPD { get; set; }

      public TimeSpan PreferredTime { get; set; }

      public TimeSpan MaximumTime { get; set; }

      public double KFactor { get; set; }

      public TimeSpan UnitTimeByDownloadTime { get; set; }

      public double DownloadTimeBonusMulti { get; set; }

      public double DownloadTimeBonusCredit { get; set; }

      public double DownloadTimeBonusPPD { get; set; }

      public TimeSpan UnitTimeByFrameTime { get; set; }

      public double FrameTimeBonusMulti { get; set; }

      public double FrameTimeBonusCredit { get; set; }

      public double FrameTimeBonusPPD { get; set; }
   }
}
