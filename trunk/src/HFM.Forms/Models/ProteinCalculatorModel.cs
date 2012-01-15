/*
 * HFM.NET - Protein Calculator Model
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.ComponentModel;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Models
{
   public sealed class ProteinCalculatorModel : INotifyPropertyChanged 
   {
      private readonly IProteinDictionary _proteinDictionary;

      public ProteinCalculatorModel(IProteinDictionary proteinDictionary)
      {
         _proteinDictionary = proteinDictionary;
      }

      public void Calculate()
      {
         var protein = _proteinDictionary[SelectedProject].DeepClone();
         if (PreferredDeadlineChecked) protein.PreferredDays = PreferredDeadline;
         if (FinalDeadlineChecked) protein.MaximumDays = FinalDeadline;
         if (KFactorChecked) protein.KFactor = KFactor;

         TimeSpan frameTime = TimeSpan.FromMinutes(TpfMinutes).Add(TimeSpan.FromSeconds(TpfSeconds));
         TimeSpan totalTimeByFrame = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
         TimeSpan totalTimeByUser = totalTimeByFrame;
         if (TotalWuTimeEnabled)
         {
            totalTimeByUser = TimeSpan.FromMinutes(TotalWuTimeMinutes).Add(TimeSpan.FromSeconds(TotalWuTimeSeconds));
            // user time is less than total time by frame, not permitted
            if (totalTimeByUser < totalTimeByFrame)
            {
               totalTimeByUser = totalTimeByFrame;
            }
         }
         var values = protein.GetProductionValues(frameTime, totalTimeByUser, totalTimeByFrame, true);
         CoreName = protein.Core;
         SlotType = protein.Core.ToSlotType().ToString();
         NumberOfAtoms = protein.NumberOfAtoms;
         CompletionTime = Math.Round((TotalWuTimeEnabled ? totalTimeByUser.TotalDays : totalTimeByFrame.TotalDays), Default.MaxDecimalPlaces);
         PreferredDeadline = protein.PreferredDays;
         FinalDeadline = protein.MaximumDays;
         KFactor = protein.KFactor;
         BonusMultiplier = TotalWuTimeEnabled ? values.DownloadTimeBonusMulti : values.FrameTimeBonusMulti;
         BaseCredit = values.BaseCredit;
         TotalCredit = TotalWuTimeEnabled ? values.DownloadTimeBonusCredit : values.FrameTimeBonusCredit;
         BasePpd = values.BasePPD;
         TotalPpd = TotalWuTimeEnabled ? values.DownloadTimeBonusPPD : values.FrameTimeBonusPPD;
      }

      #region Properties

      public IEnumerable<int> Projects
      {
         get { return _proteinDictionary.Keys; }   
      }

      private int _selectedProject;
      public int SelectedProject
      {
         get { return _selectedProject; }
         set
         {
            if (_selectedProject != value)
            {
               _selectedProject = value;
               OnPropertyChanged("SelectedProject");
            }
         }
      }

      private int _tpfMinutes;
      public int TpfMinutes
      {
         get { return _tpfMinutes; }
         set
         {  
            if (_tpfMinutes != value)
            {
               _tpfMinutes = value;
               OnPropertyChanged("TpfMinutes");
            }
         }
      }

      private int _tpfSeconds;
      public int TpfSeconds
      {
         get { return _tpfSeconds; }
         set
         {
            if (_tpfSeconds != value)
            {
               _tpfSeconds = value;
               OnPropertyChanged("TpfSeconds");
            }
         }
      }

      private bool _totalWuTimeEnabled;
      public bool TotalWuTimeEnabled
      {
         get { return _totalWuTimeEnabled; }
         set
         {
            if (_totalWuTimeEnabled != value)
            {
               _totalWuTimeEnabled = value;
               OnPropertyChanged("TotalWuTimeEnabled");
            }
         }
      }

      private int _totalWuTimeMinutes;
      public int TotalWuTimeMinutes
      {
         get { return _totalWuTimeMinutes; }
         set
         {
            if (_totalWuTimeMinutes != value)
            {
               _totalWuTimeMinutes = value;
               OnPropertyChanged("TotalWuTimeMinutes");
            }
         }
      }

      private int _totalWuTimeSeconds;
      public int TotalWuTimeSeconds
      {
         get { return _totalWuTimeSeconds; }
         set
         {
            if (_totalWuTimeSeconds != value)
            {
               _totalWuTimeSeconds = value;
               OnPropertyChanged("TotalWuTimeSeconds");
            }
         }
      }

      private string _coreName;
      public string CoreName
      {
         get { return _coreName; }
         set
         {
            if (_coreName != value)
            {
               _coreName = value;
               OnPropertyChanged("CoreName");
            }
         }
      }

      private string _slotType;
      public string SlotType
      {
         get { return _slotType; }
         set
         {
            if (_slotType != value)
            {
               _slotType = value;
               OnPropertyChanged("SlotType");
            }
         }
      }

      private int _numberOfAtoms;
      public int NumberOfAtoms
      {
         get { return _numberOfAtoms; }
         set
         {
            if (_numberOfAtoms != value)
            {
               _numberOfAtoms = value;
               OnPropertyChanged("NumberOfAtoms");
            }
         }
      }

      private double _completionTime;
      public double CompletionTime
      {
         get { return _completionTime; }
         set
         {
            if (_completionTime != value)
            {
               _completionTime = value;
               OnPropertyChanged("CompletionTime");
            }
         }
      }

      private double _preferredDeadline;
      public double PreferredDeadline
      {
         get { return _preferredDeadline; }
         set
         {
            if (_preferredDeadline != value)
            {
               _preferredDeadline = value;
               OnPropertyChanged("PreferredDeadline");
            }
         }
      }

      private bool _preferredDeadlineChecked;
      public bool PreferredDeadlineChecked
      {
         get { return _preferredDeadlineChecked; }
         set
         {
            if (_preferredDeadlineChecked != value)
            {
               _preferredDeadlineChecked = value;
               OnPropertyChanged("PreferredDeadlineChecked");
               OnPropertyChanged("PreferredDeadlineIsReadOnly");
            }
         }
      }

      public bool PreferredDeadlineIsReadOnly
      {
         get { return !PreferredDeadlineChecked; }
      }

      private double _finalDeadline;
      public double FinalDeadline
      {
         get { return _finalDeadline; }
         set
         {
            if (_finalDeadline != value)
            {
               _finalDeadline = value;
               OnPropertyChanged("FinalDeadline");
            }
         }
      }

      private bool _finalDeadlineChecked;
      public bool FinalDeadlineChecked
      {
         get { return _finalDeadlineChecked; }
         set
         {
            if (_finalDeadlineChecked != value)
            {
               _finalDeadlineChecked = value;
               OnPropertyChanged("FinalDeadlineChecked");
               OnPropertyChanged("FinalDeadlineIsReadOnly");
            }
         }
      }

      public bool FinalDeadlineIsReadOnly
      {
         get { return !FinalDeadlineChecked; }
      }

      private double _kFactor;
      public double KFactor
      {
         get { return _kFactor; }
         set
         {
            if (_kFactor != value)
            {
               _kFactor = value;
               OnPropertyChanged("KFactor");
            }
         }
      }

      private bool _kFactorChecked;
      public bool KFactorChecked
      {
         get { return _kFactorChecked; }
         set
         {
            if (_kFactorChecked != value)
            {
               _kFactorChecked = value;
               OnPropertyChanged("KFactorChecked");
               OnPropertyChanged("KFactorIsReadOnly");
            }
         }
      }

      public bool KFactorIsReadOnly
      {
         get { return !KFactorChecked; }
      }

      private double _bonusMultiplier;
      public double BonusMultiplier
      {
         get { return _bonusMultiplier; }
         set
         {
            if (_bonusMultiplier != value)
            {
               _bonusMultiplier = value;
               OnPropertyChanged("BonusMultiplier");
            }
         }
      }

      private double _baseCredit;
      public double BaseCredit
      {
         get { return _baseCredit; }
         set
         {
            if (_baseCredit != value)
            {
               _baseCredit = value;
               OnPropertyChanged("BaseCredit");
            }
         }
      }

      public double BonusCredit
      {
         get { return TotalCredit - BaseCredit; }
      }

      private double _totalCredit;
      public double TotalCredit
      {
         get { return _totalCredit; }
         set
         {
            if (_totalCredit != value)
            {
               _totalCredit = value;
               OnPropertyChanged("BonusCredit");
               OnPropertyChanged("TotalCredit");
            }
         }
      }

      private double _basePpd;
      public double BasePpd
      {
         get { return _basePpd; }
         set
         {
            if (_basePpd != value)
            {
               _basePpd = value;
               OnPropertyChanged("BasePpd");
            }
         }
      }

      public double BonusPpd
      {
         get { return TotalPpd - BasePpd; }
      }

      private double _totalPpd;
      public double TotalPpd
      {
         get { return _totalPpd; }
         set
         {
            if (_totalPpd != value)
            {
               _totalPpd = value;
               OnPropertyChanged("BonusPpd");
               OnPropertyChanged("TotalPpd");
            }
         }
      }

      #endregion

      #region INotifyPropertyChanged Members

      public event PropertyChangedEventHandler PropertyChanged;

      private void OnPropertyChanged(string propertyName)
      {
         if (PropertyChanged != null) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
      }

      #endregion
   }
}
