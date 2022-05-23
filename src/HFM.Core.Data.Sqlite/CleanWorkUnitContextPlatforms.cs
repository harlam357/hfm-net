using HFM.Core.Logging;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HFM.Core.Data;

public class CleanWorkUnitContextPlatforms
{
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public CleanWorkUnitContextPlatforms(ILogger logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger ?? NullLogger.Instance;
        _serviceScopeFactory = serviceScopeFactory;
    }

    private int _count;
    private int _total;
    private int _lastPercent;

    public async Task ExecuteAsync(IProgress<ProgressInfo> progress)
    {
        ReportProgressMessage(progress, "Cleaning platform data...");

        await RemoveDuplicates(progress).ConfigureAwait(false);
        await AddVersion().ConfigureAwait(false);
    }

    private async Task RemoveDuplicates(IProgress<ProgressInfo> progress)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        var platforms = await context.Platforms.ToListAsync().ConfigureAwait(false);

        _total = platforms.Count;

        foreach (var p in platforms)
        {
            ReportPlatformProgress(progress);

            var duplicates = platforms
                .Where(x =>
                    x.ClientVersion == p.ClientVersion &&
                    x.OperatingSystem == p.OperatingSystem &&
                    x.Implementation == p.Implementation &&
                    x.Processor.StartsWith(p.Processor + " (", StringComparison.Ordinal) &&
                    x.Threads == p.Threads &&
                    x.DriverVersion == p.DriverVersion &&
                    x.ComputeVersion == p.ComputeVersion &&
                    x.CUDAVersion == p.CUDAVersion)
                .ToList();

            if (duplicates.Any())
            {
                var query = context.WorkUnits
                    .Where(x => duplicates
                        .Select(y => y.ID)
                        .Contains(x.PlatformID.GetValueOrDefault()));

                foreach (var w in query)
                {
                    w.PlatformID = p.ID;
                }

                foreach (var d in duplicates)
                {
                    context.Platforms.Remove(d);
                }
            }
        }

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private async Task AddVersion()
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<WorkUnitContext>();
        context.Versions.Add(new VersionEntity { Version = Application.Version });

        await context.SaveChangesAsync().ConfigureAwait(false);
    }

    private static int CalculatePercent(int count, int total) => ((count * 200) + total) / (total * 2);

    private void ReportProgressMessage(IProgress<ProgressInfo> progress, string message)
    {
        _logger.Info(message);
        progress.Report(new ProgressInfo(_lastPercent, message));
    }

    private void ReportPlatformProgress(IProgress<ProgressInfo> progress)
    {
        int percent = CalculatePercent(++_count, _total);
        if (percent != _lastPercent)
        {
            _lastPercent = percent;
            string message = $"Cleaning platform {_count} of {_total}";
            _logger.Info(message);
            progress.Report(new ProgressInfo(_lastPercent, message));
        }
    }
}
