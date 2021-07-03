using System;
using System.Threading;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Core.ScheduledTasks
{
    public class EocStatsScheduledTask : ScheduledTask
    {
        public IPreferences Preferences { get; }

        public ILogger Logger { get; }

        public IEocStatsService StatsService { get; }

        public EocStatsDataContainer DataContainer { get; }

        public EocStatsScheduledTask(IPreferences preferences, ILogger logger, IEocStatsService eocStatsService, EocStatsDataContainer dataContainer)
            : base("EOC stats")
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            Logger = logger ?? NullLogger.Instance;
            StatsService = eocStatsService ?? throw new ArgumentNullException(nameof(eocStatsService));
            DataContainer = dataContainer ?? throw new ArgumentNullException(nameof(dataContainer));
            Interval = CalculateInterval(DataContainer.Data.LastUpdated);
            Changed += TaskChanged;

            Preferences.PreferenceChanged += (s, e) =>
            {
                if (e.Preference == Preference.EnableUserStats)
                {
                    bool enableUserStats = Preferences.Get<bool>(Preference.EnableUserStats);
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

            if (Preferences.Get<bool>(Preference.EnableUserStats))
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
                    Logger.Info(e.ToString(i => $"{(int)TimeSpan.FromMilliseconds(i.GetValueOrDefault()).TotalMinutes} minutes"));
                    break;
                case ScheduledTaskChangedAction.Faulted:
                    Logger.Error(e.ToString());
                    break;
                case ScheduledTaskChangedAction.AlreadyInProgress:
                    Logger.Warn(e.ToString());
                    break;
                default:
                    Logger.Info(e.ToString());
                    break;
            }
        }

        protected override void OnRun(CancellationToken ct)
        {
            try
            {
                var newStatsData = StatsService.GetStatsData();

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

            DateTime nextUpdateTime = EocStatsData.GetNextUpdateTime(lastUpdated, utcNow, isDaylightSavingsTime);

            return utcNow >= nextUpdateTime;
        }

        private static double CalculateInterval(DateTime lastUpdated)
        {
            var utcNow = DateTime.UtcNow;
            var isDaylightSavingsTime = DateTime.Now.IsDaylightSavingTime();

            DateTime nextUpdateTime = EocStatsData.GetNextUpdateTime(lastUpdated, utcNow, isDaylightSavingsTime);

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
    }
}
