/*
 * HFM.NET - Display Instance Class
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

using HFM.Framework;

namespace HFM.Instances
{
   public class DisplayInstance
   {
      #region Members & Read Only Properties

      /// <summary>
      /// 
      /// </summary>
      public ClientStatus Status { get; private set; }

      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public float Progress { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public String Name { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public ClientType ClientType { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public TimeSpan TPF { get; private set; }

      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD { get; private set; }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      public Int32 MHz { get; private set; }

      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD_MHz { get; private set; }

      /// <summary>
      /// ETA for this instance
      /// </summary>
      public TimeSpan ETA { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public String Core { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public string CoreId { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public string ProjectRunCloneGen { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public double Credit { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public int Complete { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public int Failed { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public string Username { get; private set; }

      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime { get; private set; }

      /// <summary>
      /// 
      /// </summary>
      public DateTime Deadline { get; private set; }

      [SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic")]
      public string Dummy
      {
         get { return String.Empty; }
      }
      #endregion

      #region Implementation
      public void Load(ClientInstance instance, int decimalPlaces, IPreferenceSet prefs)
      {
         Status = instance.Status;
         Progress = ((float)instance.PercentComplete) / 100;
         Name = instance.InstanceName;
         ClientType = instance.CurrentUnitInfo.TypeOfClient;
         TPF = instance.TimePerFrame;
         PPD = Math.Round(instance.PPD, decimalPlaces);
         MHz = instance.ClientProcessorMegahertz;
         PPD_MHz = Math.Round(instance.PPD / instance.ClientProcessorMegahertz, 3);
         ETA = instance.ETA;
         Core = instance.CurrentUnitInfo.Core;
         CoreId = instance.CurrentUnitInfo.CoreId;
         ProjectRunCloneGen = instance.CurrentUnitInfo.ProjectRunCloneGen;
         Credit = instance.Credit;
         Complete = instance.TotalRunCompletedUnits;
         Failed = instance.TotalRunFailedUnits;
         Username = instance.FoldingIDAndTeam;
         DownloadTime = instance.CurrentUnitInfo.DownloadTime;
         Deadline = instance.CurrentUnitInfo.PreferredDeadline;
      }

      public void UpdateName(string key)
      {
         Name = key;
      } 
      #endregion

      public static void SetupDataGridViewColumns(DataGridView dataGridView1)
      {
         // ReSharper disable PossibleNullReferenceException
         dataGridView1.Columns.Add("Status", "Status");
         dataGridView1.Columns["Status"].DataPropertyName = "Status";
         dataGridView1.Columns.Add("Progress", "Progress");
         dataGridView1.Columns["Progress"].DataPropertyName = "Progress";
         DataGridViewCellStyle progressStyle = new DataGridViewCellStyle { Format = "00%" };
         dataGridView1.Columns["Progress"].DefaultCellStyle = progressStyle;
         dataGridView1.Columns.Add("Name", "Name");
         dataGridView1.Columns["Name"].DataPropertyName = "Name";
         dataGridView1.Columns.Add("ClientType", "Client Type");
         dataGridView1.Columns["ClientType"].DataPropertyName = "ClientType";
         dataGridView1.Columns.Add("TPF", "TPF");
         dataGridView1.Columns["TPF"].DataPropertyName = "TPF";
         dataGridView1.Columns.Add("PPD", "PPD");
         dataGridView1.Columns["PPD"].DataPropertyName = "PPD";
         dataGridView1.Columns.Add("MHz", "MHz");
         dataGridView1.Columns["MHz"].DataPropertyName = "MHz";
         dataGridView1.Columns.Add("PPD_MHz", "PPD/MHz");
         dataGridView1.Columns["PPD_MHz"].DataPropertyName = "PPD_MHz";
         dataGridView1.Columns.Add("ETA", "ETA");
         dataGridView1.Columns["ETA"].DataPropertyName = "ETA";
         dataGridView1.Columns.Add("Core", "Core");
         dataGridView1.Columns["Core"].DataPropertyName = "Core";
         dataGridView1.Columns.Add("CoreId", "Core ID");
         dataGridView1.Columns["CoreId"].DataPropertyName = "CoreId";
         dataGridView1.Columns.Add("ProjectRunCloneGen", "Project (Run, Clone, Gen)");
         dataGridView1.Columns["ProjectRunCloneGen"].DataPropertyName = "ProjectRunCloneGen";
         dataGridView1.Columns.Add("Credit", "Credit");
         dataGridView1.Columns["Credit"].DataPropertyName = "Credit";
         dataGridView1.Columns.Add("Complete", "Complete");
         dataGridView1.Columns["Complete"].DataPropertyName = "Complete";
         dataGridView1.Columns.Add("Failed", "Failed");
         dataGridView1.Columns["Failed"].DataPropertyName = "Failed";
         dataGridView1.Columns.Add("Username", "User Name");
         dataGridView1.Columns["Username"].DataPropertyName = "Username";
         dataGridView1.Columns.Add("DownloadTime", "Download Time");
         dataGridView1.Columns["DownloadTime"].DataPropertyName = "DownloadTime";
         dataGridView1.Columns.Add("Deadline", "Deadline");
         dataGridView1.Columns["Deadline"].DataPropertyName = "Deadline";
         dataGridView1.Columns.Add("Dummy", String.Empty);
         //dataGridView1.Columns["Dummy"].DataPropertyName = "Dummy";
         // ReSharper restore PossibleNullReferenceException
      }
   }
}
