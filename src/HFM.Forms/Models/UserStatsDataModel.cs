/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

using HFM.Core;
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
        private readonly IPreferenceSet _prefs;
        private readonly EocStatsScheduledTask _scheduledTask;

        public UserStatsDataModel(IPreferenceSet prefs, EocStatsScheduledTask scheduledTask)
        {
            _prefs = prefs;
            _scheduledTask = scheduledTask;

            _prefs.PreferenceChanged += (s, e) =>
                                        {
                                            if (e.Preference == Preference.EnableUserStats)
                                            {
                                                ControlsVisible = _prefs.Get<bool>(Preference.EnableUserStats);
                                                if (ControlsVisible)
                                                {
                                                    RefreshFromData();
                                                }
                                            }
                                        };

            ControlsVisible = _prefs.Get<bool>(Preference.EnableUserStats);
            RefreshFromData();
        }

        public void SetViewStyle(bool showTeamStats)
        {
            _prefs.Set(Preference.UserStatsType, showTeamStats ? StatsType.Team : StatsType.User);
            OnPropertyChanged(null);
        }

        public void Refresh()
        {
            _scheduledTask.Run(false);
            RefreshFromData();
        }

        private void RefreshFromData()
        {
            AutoMapper.Mapper.Map(_scheduledTask.DataContainer.Data, this);
            OnPropertyChanged(null);
        }

        private static string BuildLabel(string labelName, object value)
        {
            return String.Format(CultureInfo.CurrentCulture, String.Concat(labelName, ": ", Constants.EocStatsFormat), value);
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

        private bool ShowTeamStats => _prefs.Get<StatsType>(Preference.UserStatsType) == StatsType.Team;

        public bool OverallRankVisible => !ShowTeamStats && ControlsVisible;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
