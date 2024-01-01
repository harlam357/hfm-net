using System.Globalization;

using HFM.Core.Client;
using HFM.Proteins;

namespace HFM.Forms.Models;

public interface IBenchmarksReportSource
{
    SlotIdentifier? SlotIdentifier { get; }

    IReadOnlyCollection<int> Projects { get; }

    IReadOnlyList<Color> Colors { get; }
}

public abstract class BenchmarksReport
{
    public string Key { get; }

    public object Result { get; protected set; }

    protected BenchmarksReport(string key)
    {
        Key = key ?? throw new ArgumentNullException(nameof(key));
    }

    public abstract Task Generate(IBenchmarksReportSource source);

    protected static IEnumerable<string> EnumerateProjectInformation(Protein protein)
    {
        yield return String.Format(CultureInfo.InvariantCulture, " Project ID: {0}", protein.ProjectNumber);
        yield return String.Format(CultureInfo.InvariantCulture, " Core: {0}", protein.Core);
        yield return String.Format(CultureInfo.InvariantCulture, " Credit: {0}", protein.Credit);
        yield return String.Format(CultureInfo.InvariantCulture, " Frames: {0}", protein.Frames);
    }

    protected static double GetPPD(Protein protein, TimeSpan frameTime, bool calculateBonus)
    {
        if (calculateBonus)
        {
            var unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
            return protein.CalculateBonusPointsPerDay(frameTime, unitTime);
        }
        return protein.CalculatePointsPerDay(frameTime);
    }
}
