/*
 * HFM.NET - Display Instance Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Windows.Forms;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Instances
{
   public class DisplayInstance
   {
      #region Members & Read Only Properties
      /// <summary>
      /// 
      /// </summary>
      private ClientStatus _Status;
      /// <summary>
      /// 
      /// </summary>
      public ClientStatus Status
      {
         get { return _Status; }
      }

      /// <summary>
      /// Private member holding the percentage progress of the unit
      /// </summary>
      private string _Progress;
      /// <summary>
      /// Current progress (percentage) of the unit
      /// </summary>
      public string Progress
      {
         get { return _Progress; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _InstanceName;
      /// <summary>
      /// 
      /// </summary>
      public String Name
      {
         get { return _InstanceName; }
      }

      /// <summary>
      /// 
      /// </summary>
      private ClientType _ClientType;
      /// <summary>
      /// 
      /// </summary>
      public ClientType ClientType
      {
         get { return _ClientType; }
      }

      /// <summary>
      /// Private member holding the time per frame of the unit
      /// </summary>
      private TimeSpan _TimePerFrame;
      /// <summary>
      /// 
      /// </summary>
      public TimeSpan TPF
      {
         get { return _TimePerFrame; }
         set { _TimePerFrame = value; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD
      {
         get { return _PPD; }
      }

      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      private Int32 _MHz;
      /// <summary>
      /// The number of processor megahertz for this client instance
      /// </summary>
      public Int32 MHz
      {
         get { return _MHz; }
      }

      /// <summary>
      /// Private variable holding the PPD rating for this instance
      /// </summary>
      private double _PPD_MHz;
      /// <summary>
      /// PPD rating for this instance
      /// </summary>
      public double PPD_MHz
      {
         get { return _PPD_MHz; }
      }

      /// <summary>
      /// Private variable holding the ETA
      /// </summary>
      private TimeSpan _ETA;
      /// <summary>
      /// ETA for this instance
      /// </summary>
      public TimeSpan ETA
      {
         get { return _ETA; }
      }

      /// <summary>
      /// 
      /// </summary>
      private String _Core;
      /// <summary>
      /// 
      /// </summary>
      public String Core
      {
         get { return _Core; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _CoreVersion;
      /// <summary>
      /// 
      /// </summary>
      public string CoreVersion
      {
         get { return _CoreVersion; }
      }

      /// <summary>
      /// 
      /// </summary>
      private string _ProjectRunCloneGen;

      /// <summary>
      /// 
      /// </summary>
      public string ProjectRunCloneGen
      {
         get { return _ProjectRunCloneGen; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Credit;
      /// <summary>
      /// 
      /// </summary>
      public int Credit
      {
         get { return _Credit; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Complete;
      /// <summary>
      /// 
      /// </summary>
      public int Complete
      {
         get { return _Complete; }
      }

      /// <summary>
      /// 
      /// </summary>
      private int _Failed;
      /// <summary>
      /// 
      /// </summary>
      public int Failed
      {
         get { return _Failed; }
      }
      
      /// <summary>
      /// 
      /// </summary>
      private string _Username;
      /// <summary>
      /// 
      /// </summary>
      public string Username
      {
         get { return _Username; }
      }
      
      /// <summary>
      /// Private member holding the download time of the unit
      /// </summary>
      private DateTime _DownloadTime;
      /// <summary>
      /// Date/time the unit was downloaded
      /// </summary>
      public DateTime DownloadTime
      {
         get { return _DownloadTime; }
      }

      /// <summary>
      /// 
      /// </summary>
      private DateTime _Deadline;
	   /// <summary>
	   /// 
	   /// </summary>
      public DateTime Deadline
      {
         get { return _Deadline; }
      }

      public string Dummy
      {
         get { return String.Empty; }
      }
      #endregion

      #region Implementation
      public void Load(ClientInstance Instance)
      {
         _Status = Instance.Status;
         _Progress = String.Format("{0:00}%", Instance.CurrentUnitInfo.PercentComplete);
         _InstanceName = Instance.InstanceName;
         _ClientType = Instance.CurrentUnitInfo.TypeOfClient;
         _TimePerFrame = Instance.CurrentUnitInfo.TimePerFrame;
         _PPD = Math.Round(Instance.CurrentUnitInfo.PPD, PreferenceSet.Instance.DecimalPlaces);
         _MHz = Instance.ClientProcessorMegahertz;
         _PPD_MHz = Math.Round(Instance.CurrentUnitInfo.PPD / Instance.ClientProcessorMegahertz, 3);
         _ETA = Instance.CurrentUnitInfo.ETA;
         _Core = Instance.CurrentUnitInfo.CurrentProtein.Core;
         _CoreVersion = Instance.CurrentUnitInfo.CoreVersion;
         _ProjectRunCloneGen = Instance.CurrentUnitInfo.ProjectRunCloneGen;
         _Credit = Instance.CurrentUnitInfo.CurrentProtein.Credit;
         _Complete = Instance.NumberOfCompletedUnitsSinceLastStart;
         _Failed = Instance.NumberOfFailedUnitsSinceLastStart;
         _Username = String.Format("{0} ({1})", Instance.CurrentUnitInfo.FoldingID, Instance.CurrentUnitInfo.Team);
         _DownloadTime = Instance.CurrentUnitInfo.DownloadTime;
         _Deadline = Instance.CurrentUnitInfo.Deadline;
      }

      public void UpdateName(string Key)
      {
         _InstanceName = Key;
      } 
      #endregion

      public static void SetupDataGridViewColumns(DataGridView dataGridView1)
      {
         dataGridView1.Columns.Add("Status", "Status");
         dataGridView1.Columns["Status"].DataPropertyName = "Status";
         dataGridView1.Columns.Add("Progress", "Progress");
         dataGridView1.Columns["Progress"].DataPropertyName = "Progress";
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
         dataGridView1.Columns.Add("CoreVersion", "Core Version");
         dataGridView1.Columns["CoreVersion"].DataPropertyName = "CoreVersion";
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
      }
   }
}
