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
using System.Diagnostics;
using System.Globalization;

using Castle.Core.Logging;

using HFM.Core;
using HFM.Core.Data;
using HFM.Core.Services;
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

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        private readonly System.Timers.Timer _updateTimer;

        private readonly IPreferenceSet _prefs;
        private readonly IEocStatsService _eocStatsService;
        private readonly EocStatsDataContainer _dataContainer;

        public UserStatsDataModel(IPreferenceSet prefs, IEocStatsService eocStatsService, EocStatsDataContainer dataContainer)
        {
            _updateTimer = new System.Timers.Timer();
            _updateTimer.Elapsed += UpdateTimerElapsed;

            _prefs = prefs;
            _eocStatsService = eocStatsService;
            _dataContainer = dataContainer;

            _prefs.PreferenceChanged += (s, e) =>
                                        {
                                            if (e.Preference == Preference.EnableUserStats)
                                            {
                                                ControlsVisible = _prefs.Get<bool>(Preference.EnableUserStats);
                                                if (ControlsVisible)
                                                {
                                                    RefreshEocStatsData(false);
                                                    StartTimer();
                                                }
                                                else
                                                {
                                                    StopTimer();
                                                }
                                            }
                                        };

            // apply data container to the model
            ControlsVisible = _prefs.Get<bool>(Preference.EnableUserStats);
            if (ControlsVisible)
            {
                if (TimeForNextUpdate())
                {
                    RefreshEocStatsData(false);
                }
                StartTimer();
            }
            AutoMapper.Mapper.Map(_dataContainer.Data, this);
        }

        private void UpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            StopTimer();
            RefreshEocStatsData(false);
            StartTimer();
        }

        private bool TimeForNextUpdate()
        {
            var utcNow = DateTime.UtcNow;
            var isDaylightSavingsTime = DateTime.Now.IsDaylightSavingTime();

            DateTime nextUpdateTime = EocStatsData.GetNextUpdateTime(_dataContainer.Data.LastUpdated, utcNow, isDaylightSavingsTime);

            Logger.DebugFormat("Current Time: {0} (UTC)", utcNow);
            Logger.DebugFormat("Next EOC Stats Update Time: {0} (UTC)", nextUpdateTime);

            return utcNow >= nextUpdateTime;
        }

        private void StartTimer()
        {
            var utcNow = DateTime.UtcNow;
            var isDaylightSavingsTime = DateTime.Now.IsDaylightSavingTime();

            DateTime nextUpdateTime = EocStatsData.GetNextUpdateTime(_dataContainer.Data.LastUpdated, utcNow, isDaylightSavingsTime);

            TimeSpan timeUntilNextUpdate = nextUpdateTime.Subtract(utcNow);
            // add a random number of minutes to the interval to allow time for the update to finish on the EOC servers.
            // this also provides for some staggering between all HFM instances so the EOC servers aren't bombarded all at the same time.
            timeUntilNextUpdate = timeUntilNextUpdate.Add(TimeSpan.FromMinutes(GetRandomMinutes()));

            double interval = timeUntilNextUpdate.TotalMilliseconds;
            // sanity check - Timer.Interval must be positive and less than Int32.MaxValue.
            // Otherwise an exception is thrown when calling Start() on the timer.
            if (interval > TimeSpan.FromMinutes(1).TotalMilliseconds && interval < Int32.MaxValue)
            {
                _updateTimer.Interval = interval;
            }
            else
            {
                // GetNextUpdateTime should provide reasonable value but if that fails for some reason fall back to a three hour interval
                _updateTimer.Interval = TimeSpan.FromHours(3).TotalMilliseconds;
            }

            Logger.InfoFormat("Starting EOC Stats Update Timer Loop: {0} Minutes", Convert.ToInt32(TimeSpan.FromMilliseconds(_updateTimer.Interval).TotalMinutes));
            _updateTimer.Start();
        }

        private static double GetRandomMinutes()
        {
            var r = new Random(DateTime.Now.Second);
            return r.Next(15, 30);
        }

        private void StopTimer()
        {
            Logger.Info("Stopping EOC Stats Timer Loop");
            _updateTimer.Stop();
        }

        public void SetViewStyle(bool showTeamStats)
        {
            _prefs.Set(Preference.UserStatsType, showTeamStats ? StatsType.Team : StatsType.User);
            // all properties
            OnPropertyChanged(null);
        }

        public void Refresh()
        {
            RefreshEocStatsData(true);
        }

        private static string BuildLabel(string labelName, object value)
        {
            return String.Format(CultureInfo.CurrentCulture, String.Concat(labelName, ": ", Constants.EocStatsFormat), value);
        }

        /// <summary>
        /// Get Overall User Data from EOC XML
        /// </summary>
        /// <param name="forceRefresh">Force Refresh or allow to check for next update time</param>
        private void RefreshEocStatsData(bool forceRefresh)
        {
            // if Forced or Time For an Update
            if (forceRefresh || TimeForNextUpdate())
            {
                var sw = Stopwatch.StartNew();

                try
                {
                    var newStatsData = _eocStatsService.GetStatsData();

                    // if Forced, set Last Updated and Serialize or
                    // if the new data is not equal to the previous data, we updated... otherwise, if the update
                    // status is current we should assume the data is current but did not change - Issue 67
                    if (forceRefresh || !_dataContainer.Data.Equals(newStatsData) || newStatsData.Status == "Current")
                    {
                        _dataContainer.Data = newStatsData;
                        _dataContainer.Write();

                        AutoMapper.Mapper.Map(_dataContainer.Data, this);
                        OnPropertyChanged(null);
                    }
                }
                catch (Exception ex)
                {
                    Logger.ErrorFormat(ex, "{0}", ex.Message);
                }
                finally
                {
                    // TODO: Consolidate with StopwatchExtensions.GetExecTime()
                    Logger.InfoFormat("EOC Stats Updated in {0}", $"{sw.ElapsedMilliseconds:#,##0} ms");
                }
            }

            Logger.InfoFormat("Last EOC Stats Update: {0} (UTC)", _dataContainer.Data.LastUpdated);
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
