/*
 * HFM.NET - History Entry Class
 * Copyright (C) 2009-2013 Ryan Harlamert (harlam357)
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
using System.Diagnostics;

namespace HFM.Core.DataTypes
{
   [PetaPoco.TableName("WuHistory")]
   [PetaPoco.PrimaryKey("ID")]
   public class HistoryEntry : IEquatable<HistoryEntry>
   {
      private Protein _protein;

      public HistoryEntry()
      {
         _protein = new Protein();

         ProductionView = HistoryProductionView.BonusDownloadTime;
      }

      public long ID { get; set; }

      public int ProjectID { get; set; }

      public int ProjectRun { get; set; }

      public int ProjectClone { get; set; }

      public int ProjectGen { get; set; }

      [PetaPoco.Column("InstanceName")]
      public string Name { get; set; }

      [PetaPoco.Column("InstancePath")]
      public string Path { get; set; }

      public string Username { get; set; }

      public int Team { get; set; }

      public float CoreVersion { get; set; }

      public int FramesCompleted { get; set; }

      [PetaPoco.Ignore]
      public TimeSpan FrameTime
      {
         get { return TimeSpan.FromSeconds(FrameTimeValue); }
      }
      [PetaPoco.Column("FrameTime")]
      public int FrameTimeValue { get; set; }

      [PetaPoco.Ignore]
      public string Result
      {
         get { return ResultValue.ToWorkUnitResultString(); }
      }
      [PetaPoco.Column("Result")]
      public int ResultValue { get; set; }

      public DateTime DownloadDateTime { get; set; }

      public DateTime CompletionDateTime { get; set; }

      private string _workUnitName = String.Empty;

      public string WorkUnitName
      {
         get { return _protein == null ? _workUnitName : _protein.WorkUnitName; }
         set
         {
            if (_protein == null)
            {
               _workUnitName = value;
            }
            else
            {
               _protein.WorkUnitName = value;
            }
         }
      }

      private double _kfactor;

      public double KFactor
      {
         get { return _protein == null ? _kfactor : _protein.KFactor; }
         set
         {
            if (_protein == null)
            {
               _kfactor = value;
            }
            else
            {
               _protein.KFactor = value;
            }
         }
      }

      private string _core = String.Empty;

      public string Core
      {
         get { return _protein == null ? _core : _protein.Core; }
         set
         {
            if (_protein == null)
            {
               _core = value;
            }
            else
            {
               _protein.Core = value;
            }
         }
      }

      private int _frames;

      public int Frames
      {
         get { return _protein == null ? _frames : _protein.Frames; }
         set
         {
            if (_protein == null)
            {
               _frames = value;
            }
            else
            {
               _protein.Frames = value;
            }
         }
      }

      private int _atoms;

      public int Atoms
      {
         get { return _protein == null ? _atoms : _protein.NumberOfAtoms; }
         set
         {
            if (_protein == null)
            {
               _atoms = value;
            }
            else
            {
               _protein.NumberOfAtoms = value;
            }
         }
      }

      public string SlotType { get; set; }
      
      [PetaPoco.Ignore]
      public HistoryProductionView ProductionView { get; set; }

      [PetaPoco.Ignore]
      public double PPD
      {
         get
         {
            if (_protein == null) return 0;
         
            switch (ProductionView)
            {
               case HistoryProductionView.Standard:
                  return _protein.GetPPD(FrameTime);
               case HistoryProductionView.BonusFrameTime:
                  return _protein.GetPPD(FrameTime, TimeSpan.FromSeconds(FrameTime.TotalSeconds * Frames), true);
               case HistoryProductionView.BonusDownloadTime:
                  return _protein.GetPPD(FrameTime, CompletionDateTime.Subtract(DownloadDateTime), true);
               default:
                  // ReSharper disable HeuristicUnreachableCode
                  Debug.Assert(false);
                  return 0;
                  // ReSharper restore HeuristicUnreachableCode
            }
         }
      }

      private double _credit;

      public double Credit
      {
         get
         {
            if (_protein == null) return _credit;

            switch (ProductionView)
            {
               case HistoryProductionView.Standard:
                  return _protein.Credit;
               case HistoryProductionView.BonusFrameTime:
                  return _protein.GetCredit(TimeSpan.FromSeconds(FrameTime.TotalSeconds * Frames), true);
               case HistoryProductionView.BonusDownloadTime:
                  return _protein.GetCredit(CompletionDateTime.Subtract(DownloadDateTime), true);
               default:
                  // ReSharper disable HeuristicUnreachableCode
                  Debug.Assert(false);
                  return 0;
                  // ReSharper restore HeuristicUnreachableCode
            }
         }
         set
         {
            if (_protein == null)
            {
               _credit = value;
            }
            else
            {
               _protein.Credit = value;
            }
         }
      }

      private double _preferredDays;

      public double PreferredDays
      {
         get { return _protein == null ? _preferredDays : _protein.PreferredDays; }
         set
         {
            if (_protein == null)
            {
               _preferredDays = value;
            }
            else
            {
               _protein.PreferredDays = value;
            }
         }
      }

      private double _maximumDays;

      public double MaximumDays
      {
         get { return _protein == null ? _maximumDays : _protein.MaximumDays; }
         set
         {
            if (_protein == null)
            {
               _maximumDays = value;
            }
            else
            {
               _protein.MaximumDays = value;
            }
         }
      }
      
      public HistoryEntry SetProtein(Protein protein)
      {
         _protein = protein;
         if (protein != null)
         {
            SlotType = protein.Core.ToSlotType().ToString();
         }

         return this;
      }

      #region IEquatable<HistoryEntry> Implementation

      public bool Equals(HistoryEntry other)
      {
         if (other == null) return false;

         return (ProjectID == other.ProjectID &&
                 ProjectRun == other.ProjectRun &&
                 ProjectClone == other.ProjectClone &&
                 ProjectGen == other.ProjectGen &&
                 DownloadDateTime == other.DownloadDateTime);
      }

      #endregion
   }
}
