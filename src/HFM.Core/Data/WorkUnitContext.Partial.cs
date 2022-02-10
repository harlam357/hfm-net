using System.Globalization;

using HFM.Core.WorkUnits;
using HFM.Proteins;

using Microsoft.EntityFrameworkCore;

namespace HFM.Core.Data;

public partial class WorkUnitContext
{
    private const string VersionStringDefault = "0.0.0";

    public async Task<string> GetDatabaseVersion()
    {
        if (!await Database.CanConnectAsync().ConfigureAwait(false))
        {
            return null;
        }

        var versionRow = await Versions.OrderByDescending(x => x.ID).FirstOrDefaultAsync().ConfigureAwait(false);
        return versionRow?.Version ?? VersionStringDefault;
    }

    [DbFunction]
    public static double CalculatePPD(int frameTime,
                                      int frames,
                                      double credit,
                                      double kFactor,
                                      double timeoutDays,
                                      double expirationDays,
                                      string assigned,
                                      string finished,
                                      int bonus)
    {
        TimeSpan unitTime = GetUnitTime(frameTime, frames, expirationDays, assigned, finished, bonus);

        return ProductionCalculator.GetBonusPPD(
            TimeSpan.FromSeconds(frameTime),
            frames,
            credit,
            kFactor,
            timeoutDays,
            expirationDays,
            unitTime);
    }

    [DbFunction]
    public static double CalculateCredit(int frameTime,
                                         int frames,
                                         double credit,
                                         double kFactor,
                                         double timeoutDays,
                                         double expirationDays,
                                         string assigned,
                                         string finished,
                                         int bonus)
    {
        TimeSpan unitTime = GetUnitTime(frameTime, frames, expirationDays, assigned, finished, bonus);

        return ProductionCalculator.GetBonusCredit(
            credit,
            kFactor,
            timeoutDays,
            expirationDays,
            unitTime);
    }

    private static TimeSpan GetUnitTime(int frameTime,
                                        int frames,
                                        double expirationDays,
                                        string assigned,
                                        string finished,
                                        int bonus)
    {
        const string format = "yyyy-MM-dd HH:mm:ss";
        const DateTimeStyles style = DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal;

        DateTime.TryParseExact(assigned, format, CultureInfo.InvariantCulture, style, out var assignedDateTime);
        DateTime.TryParseExact(finished, format, CultureInfo.InvariantCulture, style, out var finishedDateTime);

        const double oneDayInSeconds = 86400.0;
        var bonusCalculation = (BonusCalculation)bonus;

        return bonusCalculation switch
        {
            BonusCalculation.DownloadTime => finishedDateTime.Subtract(assignedDateTime),
            BonusCalculation.FrameTime => TimeSpan.FromDays(frameTime * frames / oneDayInSeconds),
            BonusCalculation.None => TimeSpan.FromDays(expirationDays)
        };
    }
}
