
using System;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.Linq;

using HFM.Core.WorkUnits;
using HFM.Proteins;

namespace HFM.Core.Data
{
   public partial class WorkUnitRepository
   {
      [SQLiteFunction(Name = "ToSlotType", Arguments = 1, FuncType = FunctionType.Scalar)]
      private sealed class ToSlotType : SQLiteFunction
      {
         public override object Invoke(object[] args)
         {
            Debug.Assert(args.Length == 1);
            if (args[0] == null || Convert.IsDBNull(args[0]))
            {
               return String.Empty;
            }

            var core = (string)args[0];
            return String.IsNullOrEmpty(core) ? String.Empty : core.ToSlotType().ToString();
         }
      }

      [SQLiteFunction(Name = "GetProduction", Arguments = 9, FuncType = FunctionType.Scalar)]
      private sealed class GetProduction : SQLiteFunction
      {
         [ThreadStatic]
         public static BonusCalculation BonusCalculation;

         public override object Invoke(object[] args)
         {
            Debug.Assert(args.Length == 9);
            if (args.Any(x => x == null || Convert.IsDBNull(x)))
            {
               return 0.0;
            }

            var frameTime = TimeSpan.FromSeconds((long)args[0]);
            // unbox then cast to int
            var frames = (int)((long)args[1]);
            var baseCredit = (double)args[2];
            var kFactor = (double)args[3];
            var preferredDays = (double)args[4];
            var maximumDays = (double)args[5];
            DateTime.TryParseExact((string)args[6], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                   DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                   out var downloadDateTime);
            DateTime.TryParseExact((string)args[7], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture,
                                   DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal,
                                   out var completionDateTime);
            var calcOption = (long)args[8];

            TimeSpan unitTime = TimeSpan.Zero;
            switch (BonusCalculation)
            {
               case BonusCalculation.FrameTime:
                  unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * frames);
                  break;
               case BonusCalculation.DownloadTime:
                  unitTime = completionDateTime.Subtract(downloadDateTime);
                  break;
            }

            if (calcOption != 0)
            {
               return ProductionCalculator.GetBonusCredit(baseCredit, kFactor, preferredDays, maximumDays, unitTime);
            }
            return ProductionCalculator.GetBonusPPD(frameTime, frames, baseCredit, kFactor, preferredDays, maximumDays, unitTime);
         }
      }
   }
}
