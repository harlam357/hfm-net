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

namespace HFM.Core.DataTypes
{
   [PetaPoco.TableName("WuHistory")]
   [PetaPoco.PrimaryKey("ID")]
   public class HistoryEntry
   {
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

      public string WorkUnitName { get; set; }

      public double KFactor { get; set; }

      public string Core { get; set; }

      public int Frames { get; set; }

      public int Atoms { get; set; }

      [PetaPoco.Column("Credit")]
      public double BaseCredit { get; set; }

      public double PreferredDays { get; set; }

      public double MaximumDays { get; set; }

      [PetaPoco.ResultColumn]
      public string SlotType { get; set; }

      [PetaPoco.ResultColumn]
      public int ProductionView { get; set; }

      [PetaPoco.ResultColumn]
      public double PPD { get; set; }

      [PetaPoco.ResultColumn("CalcCredit")]
      public double Credit { get; set; }
   }
}
