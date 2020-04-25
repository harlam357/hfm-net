
using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

using AutoMapper;

using HFM.Core.ScheduledTasks;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public enum StatsType
    {
        User,
        Team
    }

    public sealed class UserStatsDataModel : INotifyPropertyChanged
    {
        private readonly IPreferenceSet _preferences;
        private readonly EocStatsScheduledTask _scheduledTask;
        private readonly IMapper _mapper;

        public UserStatsDataModel(ISynchronizeInvoke synchronizeInvoke, IPreferenceSet preferences, EocStatsScheduledTask scheduledTask)
        {
            _preferences = preferences;
            _scheduledTask = scheduledTask;
            _mapper = new MapperConfiguration(cfg => cfg.AddProfile<UserStatsDataModelProfile>()).CreateMapper();

            _preferences.PreferenceChanged += (s, e) =>
            {
                if (e.Preference == Preference.EnableUserStats)
                {
                    ControlsVisible = _preferences.Get<bool>(Preference.EnableUserStats);
                    if (ControlsVisible)
                    {
                        RefreshFromData();
                    }
                }
            };

            _scheduledTask.Changed += (s, e) =>
            {
                switch (e.Action)
                {
                    case ScheduledTaskChangedAction.Finished:
                        // scheduled task completes on thread pool, but RefreshFromData will trigger UI control updates
                        // provide the ISynchronizeInvoke instance for posting the updates back to the UI thread
                        RefreshFromData(synchronizeInvoke);
                        break;
                }
            };

            ControlsVisible = _preferences.Get<bool>(Preference.EnableUserStats);
            RefreshFromData();
        }

        public void SetViewStyle(bool showTeamStats)
        {
            _preferences.Set(Preference.UserStatsType, showTeamStats ? StatsType.Team : StatsType.User);
            OnPropertyChanged(null);
        }

        public void Refresh()
        {
            _scheduledTask.Run(false);
        }

        private void RefreshFromData(ISynchronizeInvoke synchronizeInvoke = null)
        {
            _mapper.Map(_scheduledTask.DataContainer.Data, this);
            if (synchronizeInvoke is null)
            {
                OnPropertyChanged(null);
            }
            else
            {
                var action = new Action(() => OnPropertyChanged(null));
                synchronizeInvoke.BeginInvoke(action, null);
            }
        }

        private const string NumberFormat = "{0:###,###,##0}";

        private static string BuildLabel(string labelName, object value)
        {
            return String.Format(CultureInfo.CurrentCulture, String.Concat(labelName, ": ", NumberFormat), value);
        }

        public string Rank => BuildLabel("Team", ShowTeamStats ? TeamRank : UserTeamRank);
        public int TeamRank { get; set; }
        public int UserTeamRank { get; set; }

        public string OverallRank => BuildLabel("Project", ShowTeamStats ? 0 : UserOverallRank);
        public int UserOverallRank { get; set; }

        public string TwentyFourHourAverage => BuildLabel("24hr", ShowTeamStats ? TeamTwentyFourHourAverage : UserTwentyFourHourAverage);
        public long TeamTwentyFourHourAverage { get; set; }
        public long UserTwentyFourHourAverage { get; set; }

        public string PointsToday => BuildLabel("Today", ShowTeamStats ? TeamPointsToday : UserPointsToday);
        public long TeamPointsToday { get; set; }
        public long UserPointsToday { get; set; }

        public string PointsWeek => BuildLabel("Week", ShowTeamStats ? TeamPointsWeek : UserPointsWeek);
        public long TeamPointsWeek { get; set; }
        public long UserPointsWeek { get; set; }

        public string PointsTotal => BuildLabel("Total", ShowTeamStats ? TeamPointsTotal : UserPointsTotal);
        public long TeamPointsTotal { get; set; }
        public long UserPointsTotal { get; set; }

        public string WorkUnitsTotal => BuildLabel("WUs", ShowTeamStats ? TeamWorkUnitsTotal : UserWorkUnitsTotal);
        public long TeamWorkUnitsTotal { get; set; }
        public long UserWorkUnitsTotal { get; set; }

        private bool _controlsVisible;

        public bool ControlsVisible
        {
            get => _controlsVisible;
            set
            {
                if (_controlsVisible != value)
                {
                    _controlsVisible = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(OverallRankVisible));
                }
            }
        }

        private bool ShowTeamStats => _preferences.Get<StatsType>(Preference.UserStatsType) == StatsType.Team;

        public bool OverallRankVisible => !ShowTeamStats && ControlsVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
