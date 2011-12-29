/*
 * HFM.NET - Preferences - Options Tab - Binding Model
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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using Castle.Core.Logging;

using HFM.Core;

namespace HFM.Forms.Models
{
   class OptionsModel : INotifyPropertyChanged
   {
      public OptionsModel(IPreferenceSet prefs)
      {
         Load(prefs);
      }

      public void Load(IPreferenceSet prefs)
      {
         OfflineLast = prefs.Get<bool>(Preference.OfflineLast);
         ColorLogFile = prefs.Get<bool>(Preference.ColorLogFile);
         AutoSaveConfig = prefs.Get<bool>(Preference.AutoSaveConfig);
         PpdCalculation = prefs.Get<PpdCalculationType>(Preference.PpdCalculation);
         DecimalPlaces = prefs.Get<int>(Preference.DecimalPlaces);
         CalculateBonus = prefs.Get<bool>(Preference.CalculateBonus);
         EtaDate = prefs.Get<bool>(Preference.EtaDate);
         DuplicateProjectCheck = prefs.Get<bool>(Preference.DuplicateProjectCheck);
         DuplicateUserIdCheck = prefs.Get<bool>(Preference.DuplicateUserIdCheck);
         ShowXmlStats = prefs.Get<bool>(Preference.ShowXmlStats);
         MessageLevel = (LoggerLevel)prefs.Get<int>(Preference.MessageLevel);
         FormShowStyle = prefs.Get<FormShowStyleType>(Preference.FormShowStyle);
      }

      public void Update(IPreferenceSet prefs)
      {
         prefs.Set(Preference.OfflineLast, OfflineLast);
         prefs.Set(Preference.ColorLogFile, ColorLogFile);
         prefs.Set(Preference.AutoSaveConfig, AutoSaveConfig);
         prefs.Set(Preference.PpdCalculation, PpdCalculation);
         prefs.Set(Preference.DecimalPlaces, DecimalPlaces);
         prefs.Set(Preference.CalculateBonus, CalculateBonus);
         prefs.Set(Preference.EtaDate, EtaDate);
         prefs.Set(Preference.DuplicateProjectCheck, DuplicateProjectCheck);
         prefs.Set(Preference.DuplicateUserIdCheck, DuplicateUserIdCheck);
         prefs.Set(Preference.ShowXmlStats, ShowXmlStats);
         prefs.Set(Preference.MessageLevel, (int)MessageLevel);
         prefs.Set(Preference.FormShowStyle, FormShowStyle);
      }

      #region Interactive Options

      private bool _offlineLast;

      public bool OfflineLast
      {
         get { return _offlineLast; }
         set
         {
            if (OfflineLast != value)
            {
               _offlineLast = value;
               OnPropertyChanged("OfflineLast");
            }
         }
      }

      private bool _colorLogFile;

      public bool ColorLogFile
      {
         get { return _colorLogFile; }
         set
         {
            if (ColorLogFile != value)
            {
               _colorLogFile = value;
               OnPropertyChanged("ColorLogFile");
            }
         }
      }

      private bool _autoSaveConfig;

      public bool AutoSaveConfig
      {
         get { return _autoSaveConfig; }
         set
         {
            if (AutoSaveConfig != value)
            {
               _autoSaveConfig = value;
               OnPropertyChanged("AutoSaveConfig");
            }
         }
      }

      private PpdCalculationType _ppdCalculation;
      
      public PpdCalculationType PpdCalculation
      {
         get { return _ppdCalculation; }
         set
         {
            if (PpdCalculation != value)
            {
               _ppdCalculation = value;
               OnPropertyChanged("PpdCalculation");
            }
         }
      }

      private int _decimalPlaces;

      public int DecimalPlaces
      {
         get { return _decimalPlaces; }
         set
         {
            if (DecimalPlaces != value)
            {
               _decimalPlaces = value;
               OnPropertyChanged("DecimalPlaces");
            }
         }
      }

      private bool _calculateBonus;

      public bool CalculateBonus
      {
         get { return _calculateBonus; }
         set
         {
            if (CalculateBonus != value)
            {
               _calculateBonus = value;
               OnPropertyChanged("CalculateBonus");
            }
         }
      }

      private bool _etaDate;
      
      public bool EtaDate
      {
         get { return _etaDate; }
         set
         {
            if (EtaDate != value)
            {
               _etaDate = value;
               OnPropertyChanged("EtaDate");
            }
         }
      }

      private bool _duplicateProjectCheck;

      public bool DuplicateProjectCheck
      {
         get { return _duplicateProjectCheck; }
         set
         {
            if (DuplicateProjectCheck != value)
            {
               _duplicateProjectCheck = value;
               OnPropertyChanged("DuplicateProjectCheck");
            }
         }
      }

      private bool _duplicateUserIdCheck;

      public bool DuplicateUserIdCheck
      {
         get { return _duplicateUserIdCheck; }
         set
         {
            if (DuplicateUserIdCheck != value)
            {
               _duplicateUserIdCheck = value;
               OnPropertyChanged("DuplicateUserIdCheck");
            }
         }
      }

      private bool _showXmlStats;

      public bool ShowXmlStats
      {
         get { return _showXmlStats; }
         set
         {
            if (ShowXmlStats != value)
            {
               _showXmlStats = value;
               OnPropertyChanged("ShowXmlStats");
            }
         }
      }
      
      #endregion

      #region Debug Message Level

      private LoggerLevel _messageLevel;

      public LoggerLevel MessageLevel
      {
         get { return _messageLevel; }
         set
         {
            if (MessageLevel != value)
            {
               _messageLevel = value;
               OnPropertyChanged("MessageLevel");
            }
         }
      }
      
      #endregion

      #region Form Docking Style

      private FormShowStyleType _formShowStyle;

      public FormShowStyleType FormShowStyle
      {
         get { return _formShowStyle; }
         set
         {
            if (FormShowStyle != value)
            {
               _formShowStyle = value;
               OnPropertyChanged("FormShowStyle");
            }
         }
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

      public static ReadOnlyCollection<ListItem> PpdCalculationList
      {
         get
         {
            var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = "Last Frame", ValueMember = PpdCalculationType.LastFrame },
                          new ListItem
                          { DisplayMember = "Last Three Frames", ValueMember = PpdCalculationType.LastThreeFrames },
                          new ListItem
                          { DisplayMember = "All Frames", ValueMember = PpdCalculationType.AllFrames },
                          new ListItem
                          { DisplayMember = "Effective Rate", ValueMember = PpdCalculationType.EffectiveRate }
                       };
            return list.AsReadOnly();
         }
      }

      public static ReadOnlyCollection<ListItem> DebugList
      {
         get
         {
            var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = LoggerLevel.Off.ToString(), ValueMember = LoggerLevel.Off },
                          new ListItem
                          { DisplayMember = LoggerLevel.Fatal.ToString(), ValueMember = LoggerLevel.Fatal },
                          new ListItem
                          { DisplayMember = LoggerLevel.Error.ToString(), ValueMember = LoggerLevel.Error },
                          new ListItem
                          { DisplayMember = LoggerLevel.Warn.ToString(), ValueMember = LoggerLevel.Warn },
                          new ListItem
                          { DisplayMember = LoggerLevel.Info.ToString(), ValueMember = LoggerLevel.Info },
                          new ListItem
                          { DisplayMember = LoggerLevel.Debug.ToString(), ValueMember = LoggerLevel.Debug }
                       };
            return list.AsReadOnly();
         }
      }

      public static ReadOnlyCollection<ListItem> DockingStyleList
      {
         get
         {
            var list = new List<ListItem>
                       {
                          new ListItem
                          { DisplayMember = "System Tray", ValueMember = FormShowStyleType.SystemTray },
                          new ListItem
                          { DisplayMember = "Task Bar", ValueMember = FormShowStyleType.TaskBar },
                          new ListItem
                          { DisplayMember = "Both", ValueMember = FormShowStyleType.Both }
                       };
            return list.AsReadOnly();
         }
      }
   }
   
   struct ListItem
   {
      public string DisplayMember { get; set; }
      public object ValueMember { get; set; }
   }
}
