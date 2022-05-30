using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.ScheduledTasks;

public class UserStatsScheduledTask : ScheduledTask
{
    private readonly IPreferences _preferences;
    private readonly ILogger _logger;
    private readonly IUserStatsService _statsService;

    public UserStatsDataContainer DataContainer { get; }

    public UserStatsScheduledTask(IPreferences preferences, ILogger logger, IUserStatsService userStatsService, UserStatsDataContainer dataContainer)
        : base("EOC stats")
    {
        _preferences = preferences ?? new InMemoryPreferencesProvider();
        _logger = logger ?? NullLogger.Instance;
        _statsService = userStatsService ?? throw new ArgumentNullException(nameof(userStatsService));
        DataContainer = dataContainer ?? throw new ArgumentNullException(nameof(dataContainer));
        Interval = CalculateInterval(DataContainer.Data.LastUpdated);
        Changed += TaskChanged;

        SubscribeToPreferenceChangedEvent();
    }

    private void SubscribeToPreferenceChangedEvent() =>
        _preferences.PreferenceChanged += (s, e) =>
        {
            if (e.Preference == Preference.EnableUserStats)
            {
                bool enableUserStats = _preferences.Get<bool>(Preference.EnableUserStats);
                if (enableUserStats)
                {
                    Start();
                }
                else
                {
                    Stop();
                }
            }
        };

    public void RunOrStartIfEnabled()
    {
        if (_preferences.Get<bool>(Preference.EnableUserStats))
        {
            if (TimeForNextUpdate(DataContainer.Data.LastUpdated))
            {
                Run();
            }
            else
            {
                Start();
            }
        }
    }

    private void TaskChanged(object sender, ScheduledTaskChangedEventArgs e)
    {
        switch (e.Action)
        {
            case ScheduledTaskChangedAction.Started:
                _logger.Info(e.ToString(i => $"{(int)TimeSpan.FromMilliseconds(i.GetValueOrDefault()).TotalMinutes} minutes"));
                break;
            case ScheduledTaskChangedAction.Faulted:
                _logger.Error(e.ToString());
                break;
            case ScheduledTaskChangedAction.AlreadyInProgress:
                _logger.Warn(e.ToString());
                break;
            default:
                _logger.Info(e.ToString());
                break;
        }
    }

    protected override void OnRun(CancellationToken ct)
    {
        try
        {
            var newStatsData = _statsService.GetStatsData();

            // if the new data is not equal to the previous data, we updated... otherwise, if the update
            // status is current we should assume the data is current but did not change
            if (!DataContainer.Data.Equals(newStatsData) || newStatsData.Status == "Current")
            {
                DataContainer.Data = newStatsData;
                DataContainer.Write();
            }
        }
        catch (Exception)
        {
            DataContainer.Data.LastUpdated = DateTime.UtcNow;
            DataContainer.Write();
            throw;
        }
        finally
        {
            Interval = CalculateInterval(DataContainer.Data.LastUpdated);
        }
    }

    private static bool TimeForNextUpdate(DateTime lastUpdated)
    {
        var utcNow = DateTime.UtcNow;
        var isDaylightSavingsTime = DateTime.Now.IsDaylightSavingTime();

        DateTime nextUpdateTime = GetNextUpdateTime(lastUpdated, utcNow, isDaylightSavingsTime);

        return utcNow >= nextUpdateTime;
    }

    private static double CalculateInterval(DateTime lastUpdated)
    {
        var utcNow = DateTime.UtcNow;
        var isDaylightSavingsTime = DateTime.Now.IsDaylightSavingTime();

        DateTime nextUpdateTime = GetNextUpdateTime(lastUpdated, utcNow, isDaylightSavingsTime);

        TimeSpan timeUntilNextUpdate = nextUpdateTime.Subtract(utcNow);
        // add a random number of minutes to the interval to allow time for the update to finish on the EOC servers.
        // this also provides for some staggering between all HFM instances so the EOC servers aren't bombarded all at the same time.
        timeUntilNextUpdate = timeUntilNextUpdate.Add(TimeSpan.FromMinutes(GetRandomMinutes()));

        double interval = timeUntilNextUpdate.TotalMilliseconds;
        // sanity check - Timer.Interval must be positive and less than Int32.MaxValue.
        // Otherwise an exception is thrown when calling Start() on the timer.
        if (interval > TimeSpan.FromMinutes(1).TotalMilliseconds && interval < Int32.MaxValue)
        {
            return interval;
        }
        // GetNextUpdateTime should provide reasonable value but if that fails for some reason fall back to a three hour interval
        return TimeSpan.FromHours(3).TotalMilliseconds;
    }

    private static double GetRandomMinutes()
    {
        var r = new Random(DateTime.Now.Second);
#pragma warning disable CA5394 // Do not use insecure randomness
        return r.Next(15, 30);
#pragma warning restore CA5394 // Do not use insecure randomness
    }

    /// <summary>
    /// Gets the time for the next stats update time in UTC.
    /// </summary>
    public static DateTime GetNextUpdateTime(DateTime lastUpdated, DateTime utcNow, bool isDaylightSavingsTime)
    {
        // if last updated is either MinValue (no value) or is in the future,
        // update to either set a value or correct a bad (future) value
        if (lastUpdated == DateTime.MinValue || lastUpdated > utcNow)
        {
            return utcNow;
        }

        // What I really need to know is if it is Daylight Savings Time
        // in the Central Time Zone, not the local machines Time Zone.
        int offset = 0;
        if (isDaylightSavingsTime)
        {
            offset = 1;
        }

        DateTime nextUpdateTime = lastUpdated.Date;

        int hours = 24;
        for (int i = 0; i < 9; i++)
        {
            if (lastUpdated.TimeOfDay >= TimeSpan.FromHours(hours - offset))
            {
                nextUpdateTime = nextUpdateTime.Add(TimeSpan.FromHours(hours + 3 - offset));
                break;
            }

            hours -= 3;
        }

        return nextUpdateTime;
    }
}
