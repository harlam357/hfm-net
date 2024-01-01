using System.Globalization;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Proteins;

using Microsoft.EntityFrameworkCore;

namespace HFM.Core.Data;

public partial class WorkUnitContext
{
    private const string VersionStringDefault = "0.0.0";

    public string GetDatabaseVersion()
    {
        if (!Database.CanConnect())
        {
            return null;
        }

        var versionRow = Versions.OrderByDescending(x => x.ID).FirstOrDefault();
        return versionRow?.Version ?? VersionStringDefault;
    }

    [DbFunction]
    public static string ToSlotName(string name, int? slotID)
    {
        if (!slotID.HasValue)
        {
            return name;
        }

        var client = new ClientIdentifier(name, null, ClientSettings.NoPort, Guid.Empty);
        var slot = new SlotIdentifier(client, slotID.Value);
        return slot.Name;
    }

    [DbFunction]
    public static string ToSlotType(string core) =>
        String.IsNullOrEmpty(core) ? String.Empty : ConvertToSlotType.FromCoreName(core).ToString();

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

        return ProductionCalculator.CalculateBonusPointsPerDay(
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

        return ProductionCalculator.CalculateBonusCredit(
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
            BonusCalculation.DownloadTime => finishedDateTime == default
                ? TimeSpan.FromDays(frameTime * frames / oneDayInSeconds)
                : finishedDateTime.Subtract(assignedDateTime),
            BonusCalculation.FrameTime => TimeSpan.FromDays(frameTime * frames / oneDayInSeconds),
            BonusCalculation.None => TimeSpan.FromDays(expirationDays),
            _ => TimeSpan.FromDays(expirationDays)
        };
    }
}
