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

using Castle.Core.Logging;

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
        private ILogger _logger;

        public ILogger Logger => _logger ?? (_logger = NullLogger.Instance);

        private readonly IPreferenceSet _prefs;
        private readonly EocStatsScheduledTask _scheduledTask;

        public UserStatsDataModel(ILogger logger, IPreferenceSet prefs, EocStatsScheduledTask scheduledTask)
        {
            _logger = logger;
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

        #region Data Properties

        public string Rank
        {
            get { return BuildLabel("Team", ShowTeamStats ? TeamRank : UserTeamRank); }
        }

        private int _teamRank;
        /// <summary>
        /// Team Rank
        /// </summary>
        public int TeamRank
        {
            get { return _teamRank; }
            set
            {
                if (_teamRank != value)
                {
                    _teamRank = value;
                    OnPropertyChanged("Rank");
                }
            }
        }

        private int _userTeamRank;
        /// <summary>
        /// User Team Rank
        /// </summary>
        public int UserTeamRank
        {
            get { return _userTeamRank; }
            set
            {
                if (_userTeamRank != value)
                {
                    _userTeamRank = value;
                    OnPropertyChanged("Rank");
                }
            }
        }

        public string OverallRank
        {
            get { return BuildLabel("Project", ShowTeamStats ? 0 : UserOverallRank); }
        }

        private int _userOverallRank;
        /// <summary>
        /// User Overall Rank
        /// </summary>
        public int UserOverallRank
        {
            get { return _userOverallRank; }
            set
            {
                if (_userOverallRank != value)
                {
                    _userOverallRank = value;
                    OnPropertyChanged("OverallRank");
                }
            }
        }

        public string TwentyFourHourAverage
        {
            get { return BuildLabel("24hr", ShowTeamStats ? TeamTwentyFourHourAverage : UserTwentyFourHourAverage); }
        }

        private long _teamTwentyFourHourAverage;
        /// <summary>
        /// Team 24 Hour Points Average
        /// </summary>
        public long TeamTwentyFourHourAverage
        {
            get { return _teamTwentyFourHourAverage; }
            set
            {
                if (_teamTwentyFourHourAverage != value)
                {
                    _teamTwentyFourHourAverage = value;
                    OnPropertyChanged("TwentyFourHourAverage");
                }
            }
        }

        private long _userTwentyFourHourAverage;
        /// <summary>
        /// User 24 Hour Points Average
        /// </summary>
        public long UserTwentyFourHourAverage
        {
            get { return _userTwentyFourHourAverage; }
            set
            {
                if (_userTwentyFourHourAverage != value)
                {
                    _userTwentyFourHourAverage = value;
                    OnPropertyChanged("TwentyFourHourAverage");
                }
            }
        }

        public string PointsToday
        {
            get { return BuildLabel("Today", ShowTeamStats ? TeamPointsToday : UserPointsToday); }
        }

        private long _teamPointsToday;
        /// <summary>
        /// Team Points Today
        /// </summary>
        public long TeamPointsToday
        {
            get { return _teamPointsToday; }
            set
            {
                if (_teamPointsToday != value)
                {
                    _teamPointsToday = value;
                    OnPropertyChanged("PointsToday");
                }
            }
        }

        private long _userPointsToday;
        /// <summary>
        /// User Points Today
        /// </summary>
        public long UserPointsToday
        {
            get { return _userPointsToday; }
            set
            {
                if (_userPointsToday != value)
                {
                    _userPointsToday = value;
                    OnPropertyChanged("PointsToday");
                }
            }
        }

        public string PointsWeek
        {
            get { return BuildLabel("Week", ShowTeamStats ? TeamPointsWeek : UserPointsWeek); }
        }

        private long _teamPointsWeek;
        /// <summary>
        /// Team Points Week
        /// </summary>
        public long TeamPointsWeek
        {
            get { return _teamPointsWeek; }
            set
            {
                if (_teamPointsWeek != value)
                {
                    _teamPointsWeek = value;
                    OnPropertyChanged("PointsWeek");
                }
            }
        }

        private long _userPointsWeek;
        /// <summary>
        /// User Points Week
        /// </summary>
        public long UserPointsWeek
        {
            get { return _userPointsWeek; }
            set
            {
                if (_userPointsWeek != value)
                {
                    _userPointsWeek = value;
                    OnPropertyChanged("PointsWeek");
                }
            }
        }

        public string PointsTotal
        {
            get { return BuildLabel("Total", ShowTeamStats ? TeamPointsTotal : UserPointsTotal); }
        }

        private long _teamPointsTotal;
        /// <summary>
        /// Team Points Total
        /// </summary>
        public long TeamPointsTotal
        {
            get { return _teamPointsTotal; }
            set
            {
                if (_teamPointsTotal != value)
                {
                    _teamPointsTotal = value;
                    OnPropertyChanged("PointsTotal");
                }
            }
        }

        private long _userPointsTotal;
        /// <summary>
        /// User Points Total
        /// </summary>
        public long UserPointsTotal
        {
            get { return _userPointsTotal; }
            set
            {
                if (_userPointsTotal != value)
                {
                    _userPointsTotal = value;
                    OnPropertyChanged("PointsTotal");
                }
            }
        }

        public string WorkUnitsTotal
        {
            get { return BuildLabel("WUs", ShowTeamStats ? TeamWorkUnitsTotal : UserWorkUnitsTotal); }
        }

        private long _teamWorkUnitsTotal;
        /// <summary>
        /// Team Work Units Total
        /// </summary>
        public long TeamWorkUnitsTotal
        {
            get { return _teamWorkUnitsTotal; }
            set
            {
                if (_teamWorkUnitsTotal != value)
                {
                    _teamWorkUnitsTotal = value;
                    OnPropertyChanged("WorkUnitsTotal");
                }
            }
        }

        private long _userWorkUnitsTotal;
        /// <summary>
        /// User Work Units Total
        /// </summary>
        public long UserWorkUnitsTotal
        {
            get { return _userWorkUnitsTotal; }
            set
            {
                if (_userWorkUnitsTotal != value)
                {
                    _userWorkUnitsTotal = value;
                    OnPropertyChanged("WorkUnitsTotal");
                }
            }
        }

        #endregion

        #region Visible Properties

        private bool _controlsVisible;

        public bool ControlsVisible
        {
            get { return _controlsVisible; }
            set
            {
                if (_controlsVisible != value)
                {
                    _controlsVisible = value;
                    OnPropertyChanged("ControlsVisible");
                    OnPropertyChanged("OverallRankVisible");
                }
            }
        }

        private bool ShowTeamStats
        {
            get { return _prefs.Get<StatsType>(Preference.UserStatsType) == StatsType.Team; }
        }

        public bool OverallRankVisible
        {
            get { return !ShowTeamStats && ControlsVisible; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
