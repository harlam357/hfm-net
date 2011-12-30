/*
 * HFM.NET - User Stats Data Model
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

namespace HFM.Forms.Models
{
   public sealed class UserStatsDataModel : INotifyPropertyChanged
   {
      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      private readonly System.Timers.Timer _updateTimer;

      private readonly IPreferenceSet _prefs;
      private readonly IXmlStatsDataContainer _dataContainer;

      public UserStatsDataModel(IPreferenceSet prefs, IXmlStatsDataContainer dataContainer)
      {
         _updateTimer = new System.Timers.Timer();
         _updateTimer.Elapsed += UpdateTimerElapsed;

         _prefs = prefs;
         _prefs.ShowUserStatsChanged += delegate
                                        {
                                           ControlsVisible = _prefs.Get<bool>(Preference.ShowXmlStats);
                                           if (ControlsVisible)
                                           {
                                              _dataContainer.GetEocXmlData(false);
                                              StartTimer();
                                           }
                                           else
                                           {
                                              StopTimer();
                                           }
                                        };
         _dataContainer = dataContainer;
         _dataContainer.XmlStatsDataChanged += delegate
                                               {
                                                  AutoMapper.Mapper.Map(_dataContainer.XmlStatsData, this);
                                                  OnPropertyChanged(null);
                                               };

         // apply data container to the model
         ControlsVisible = _prefs.Get<bool>(Preference.ShowXmlStats);
         if (ControlsVisible)
         {
            DateTime nextUpdateTime = _dataContainer.GetNextUpdateTime();
            if (nextUpdateTime < DateTime.UtcNow)
            {
               _dataContainer.GetEocXmlData(false);
            }
            StartTimer();
         }
         AutoMapper.Mapper.Map(_dataContainer.XmlStatsData, this);
      }

      private void UpdateTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
      {
         StopTimer();
         _dataContainer.GetEocXmlData(false);
         StartTimer();
      }

      private void StartTimer()
      {
         DateTime nextUpdateTime = _dataContainer.GetNextUpdateTime();
         if (nextUpdateTime < DateTime.UtcNow)
         {
            // if a recent update interval cannot be determined 
            // then default to 3 hours from now.
            _updateTimer.Interval = TimeSpan.FromHours(3).TotalMilliseconds;
         }
         else
         {
            // get the length of time from now until the next update STARTS
            TimeSpan nextUpdateInterval = nextUpdateTime.Subtract(DateTime.UtcNow);
            // now ADD a random number of minutes (15 to 30) to the interval to
            // allow time for the update to finish and also some staggering
            // between all HFM instances so the EOC servers aren't bombarded
            // all at the same time.
            nextUpdateInterval = nextUpdateInterval.Add(TimeSpan.FromMinutes(GetRandomMinutes()));
            // set it
            _updateTimer.Interval = nextUpdateInterval.TotalMilliseconds;
         }

         _logger.Info("Starting EOC Stats Update Timer Loop: {0} Minutes", Convert.ToInt32(TimeSpan.FromMilliseconds(_updateTimer.Interval).TotalMinutes));
         _updateTimer.Start();
      }

      private static double GetRandomMinutes()
      {
         var r = new Random(DateTime.Now.Second);
         return r.Next(15, 30);
      }

      private void StopTimer()
      {
         _logger.Info("Stopping EOC Stats Timer Loop");
         _updateTimer.Stop();
      }

      public void SetViewStyle(bool showTeamStats)
      {
         _prefs.Set(Preference.ShowTeamStats, showTeamStats);
         // all properties
         OnPropertyChanged(null);
      }

      public void Refresh()
      {
         _dataContainer.GetEocXmlData(true);
      }

      private static string BuildLabel(string labelName, object value)
      {
         return String.Format(CultureInfo.CurrentCulture, String.Concat(labelName, ": ", Constants.EocStatsFormat), value);
      }

      #region Data Properties

      public string Rank
      {
         get { return BuildLabel("Team", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamRank : UserTeamRank); }
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
         get { return BuildLabel("Project", _prefs.Get<bool>(Preference.ShowTeamStats) ? 0 : UserOverallRank); }
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

      public string TwentyFourHourAvgerage
      {
         get { return BuildLabel("24hr", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamTwentyFourHourAvgerage : UserTwentyFourHourAvgerage); }
      }

      private long _teamTwentyFourHourAvgerage;
      /// <summary>
      /// Team 24 Hour Points Average
      /// </summary>
      public long TeamTwentyFourHourAvgerage
      {
         get { return _teamTwentyFourHourAvgerage; }
         set
         {
            if (_teamTwentyFourHourAvgerage != value)
            {
               _teamTwentyFourHourAvgerage = value;
               OnPropertyChanged("TwentyFourHourAvgerage");
            }
         }
      }

      private long _userTwentyFourHourAvgerage;
      /// <summary>
      /// User 24 Hour Points Average
      /// </summary>
      public long UserTwentyFourHourAvgerage
      {
         get { return _userTwentyFourHourAvgerage; }
         set
         {
            if (_userTwentyFourHourAvgerage != value)
            {
               _userTwentyFourHourAvgerage = value;
               OnPropertyChanged("TwentyFourHourAvgerage");
            }
         }
      }

      public string PointsToday
      {
         get { return BuildLabel("Today", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamPointsToday : UserPointsToday); }
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
         get { return BuildLabel("Week", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamPointsWeek : UserPointsWeek); }
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
         get { return BuildLabel("Total", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamPointsTotal : UserPointsTotal); }
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
         get { return BuildLabel("WUs", _prefs.Get<bool>(Preference.ShowTeamStats) ? TeamWorkUnitsTotal : UserWorkUnitsTotal); }
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

      public bool OverallRankVisible
      {
         get { return !_prefs.Get<bool>(Preference.ShowTeamStats) && ControlsVisible; }
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
