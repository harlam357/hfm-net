/*
 * HFM.NET - Preferences - Options Tab - Binding Model
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Diagnostics;

using HFM.Framework;

namespace HFM.Forms.Models
{
   class OptionsModel : INotifyPropertyChanged
   {
      private readonly IPreferenceSet _prefs;

      public OptionsModel(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      #region Interactive Options

      public bool OfflineLast
      {
         get { return _prefs.GetPreference<bool>(Preference.OfflineLast); }
         set
         {
            if (OfflineLast != value)
            {
               _prefs.SetPreference(Preference.OfflineLast, value);
               OnPropertyChanged("OfflineLast");
            }
         }
      }

      public bool ColorLogFile
      {
         get { return _prefs.GetPreference<bool>(Preference.ColorLogFile); }
         set
         {
            if (ColorLogFile != value)
            {
               _prefs.SetPreference(Preference.ColorLogFile, value);
               OnPropertyChanged("ColorLogFile");
            }
         }
      }

      public bool AutoSaveConfig
      {
         get { return _prefs.GetPreference<bool>(Preference.AutoSaveConfig); }
         set
         {
            if (AutoSaveConfig != value)
            {
               _prefs.SetPreference(Preference.AutoSaveConfig, value);
               OnPropertyChanged("AutoSaveConfig");
            }
         }
      }

      public bool MaintainSelectedClient
      {
         get { return _prefs.GetPreference<bool>(Preference.MaintainSelectedClient); }
         set
         {
            if (MaintainSelectedClient != value)
            {
               _prefs.SetPreference(Preference.MaintainSelectedClient, value);
               OnPropertyChanged("MaintainSelectedClient");
            }
         }
      }
      
      public PpdCalculationType PpdCalculation
      {
         get { return _prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation); }
         set
         {
            if (PpdCalculation != value)
            {
               _prefs.SetPreference(Preference.PpdCalculation, value);
               OnPropertyChanged("PpdCalculation");
            }
         }
      }

      public int DecimalPlaces
      {
         get { return _prefs.GetPreference<int>(Preference.DecimalPlaces); }
         set
         {
            if (DecimalPlaces != value)
            {
               _prefs.SetPreference(Preference.DecimalPlaces, value);
               OnPropertyChanged("DecimalPlaces");
            }
         }
      }

      public bool CalculateBonus
      {
         get { return _prefs.GetPreference<bool>(Preference.CalculateBonus); }
         set
         {
            if (CalculateBonus != value)
            {
               _prefs.SetPreference(Preference.CalculateBonus, value);
               OnPropertyChanged("CalculateBonus");
            }
         }
      }
      
      public bool EtaDate
      {
         get { return _prefs.GetPreference<bool>(Preference.EtaDate); }
         set
         {
            if (EtaDate != value)
            {
               _prefs.SetPreference(Preference.EtaDate, value);
               OnPropertyChanged("EtaDate");
            }
         }
      }
      
      #endregion

      #region Debug Message Level

      public TraceLevel MessageLevel
      {
         get { return (TraceLevel)_prefs.GetPreference<int>(Preference.MessageLevel); }
         set
         {
            if (MessageLevel != value)
            {
               _prefs.SetPreference(Preference.MessageLevel, (int)value);
               OnPropertyChanged("MessageLevel");
            }
         }
      }
      
      #endregion

      #region Form Docking Style

      public FormShowStyleType FormShowStyle
      {
         get { return _prefs.GetPreference<FormShowStyleType>(Preference.FormShowStyle); }
         set
         {
            if (FormShowStyle != value)
            {
               _prefs.SetPreference(Preference.FormShowStyle, value);
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
                          { DisplayMember = TraceLevel.Off.ToString(), ValueMember = TraceLevel.Off },
                          new ListItem
                          { DisplayMember = TraceLevel.Error.ToString(), ValueMember = TraceLevel.Error },
                          new ListItem
                          { DisplayMember = TraceLevel.Warning.ToString(), ValueMember = TraceLevel.Warning },
                          new ListItem
                          { DisplayMember = TraceLevel.Info.ToString(), ValueMember = TraceLevel.Info },
                          new ListItem
                          { DisplayMember = TraceLevel.Verbose.ToString(), ValueMember = TraceLevel.Verbose }
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
