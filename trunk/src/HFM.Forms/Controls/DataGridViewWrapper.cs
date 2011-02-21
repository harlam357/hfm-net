/*
 * HFM.NET - DataGridView Wrapper Class
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

using System;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
   public partial class DataGridViewWrapper : DataGridView
   {
      /// <summary>
      /// Local Flag That Halts the SelectionChanged Event
      /// </summary>
      public bool FreezeSelectionChanged { get; set; }

      /// <summary>
      /// Local Flag That Halts the Sorted Event
      /// </summary>
      public bool FreezeSorted { get; set; }

      /// <summary>
      /// Key for the Currently Selected Row
      /// </summary>
      public string CurrentRowKey { get; private set; }

      /// <summary>
      /// Constructor
      /// </summary>
      public DataGridViewWrapper()
      {
         InitializeComponent();
      }

      /// <summary>
      /// Raises the <see cref="E:System.Windows.Forms.DataGridView.SelectionChanged"/> event.
      /// </summary>
      /// <param name="e">An <see cref="T:System.EventArgs"/> that contains information about the event.
      ///                 </param>
      protected override void OnSelectionChanged(EventArgs e)
      {
         if (FreezeSelectionChanged) return;

         // Code moved here from OnCellMouseDown
         if (SelectedRows.Count != 0)
         {
            CurrentRowKey = SelectedRows[0].Cells["Name"].Value.ToString();
         }
         else
         {
            CurrentRowKey = String.Empty;
         }
      
         base.OnSelectionChanged(e);
      }

      /// <summary>
      /// Raises the <see cref="E:System.Windows.Forms.DataGridView.Sorted"/> event. 
      /// </summary>
      /// <param name="e">An <see cref="T:System.EventArgs"/> that contains the event data.
      ///                 </param>
      protected override void OnSorted(EventArgs e)
      {
         if (FreezeSorted) return;
      
         base.OnSorted(e);
      }

      //protected override void OnCellMouseDown(DataGridViewCellMouseEventArgs e)
      //{
      //   if (e.RowIndex == -1)
      //   {
      //      if (SelectedRows.Count != 0)
      //      {
      //         CurrentRowKey = SelectedRows[0].Cells["Name"].Value.ToString();
      //      }
      //      else
      //      {
      //         CurrentRowKey = String.Empty;
      //      }
      //   }
      
      //   base.OnCellMouseDown(e);
      //}

      public const int NumberOfDisplayFields = 19;

      public static void SetupDataGridViewColumns(DataGridView dgv)
      {
         // ReSharper disable PossibleNullReferenceException
         dgv.Columns.Add("Status", "Status");
         dgv.Columns["Status"].DataPropertyName = "Status";
         dgv.Columns.Add("Progress", "Progress");
         dgv.Columns["Progress"].DataPropertyName = "Progress";
         var progressStyle = new DataGridViewCellStyle { Format = "00%" };
         dgv.Columns["Progress"].DefaultCellStyle = progressStyle;
         dgv.Columns.Add("Name", "Name");
         dgv.Columns["Name"].DataPropertyName = "Name";
         dgv.Columns.Add("ClientType", "Client Type");
         dgv.Columns["ClientType"].DataPropertyName = "ClientType";
         dgv.Columns.Add("TPF", "TPF");
         dgv.Columns["TPF"].DataPropertyName = "TPF";
         dgv.Columns.Add("PPD", "PPD");
         dgv.Columns["PPD"].DataPropertyName = "PPD";
         dgv.Columns.Add("MHz", "MHz");
         dgv.Columns["MHz"].DataPropertyName = "MHz";
         dgv.Columns.Add("PpdMHz", "PPD/MHz");
         dgv.Columns["PpdMHz"].DataPropertyName = "PpdMHz";
         dgv.Columns.Add("ETA", "ETA");
         dgv.Columns["ETA"].DataPropertyName = "ETA";
         dgv.Columns.Add("Core", "Core");
         dgv.Columns["Core"].DataPropertyName = "Core";
         dgv.Columns.Add("CoreID", "Core ID");
         dgv.Columns["CoreID"].DataPropertyName = "CoreID";
         dgv.Columns.Add("ProjectRunCloneGen", "Project (Run, Clone, Gen)");
         dgv.Columns["ProjectRunCloneGen"].DataPropertyName = "ProjectRunCloneGen";
         dgv.Columns.Add("Credit", "Credit");
         dgv.Columns["Credit"].DataPropertyName = "Credit";
         dgv.Columns.Add("Complete", "Complete");
         dgv.Columns["Complete"].DataPropertyName = "Complete";
         dgv.Columns.Add("Failed", "Failed");
         dgv.Columns["Failed"].DataPropertyName = "TotalRunFailedUnits";
         dgv.Columns.Add("Username", "User Name");
         dgv.Columns["Username"].DataPropertyName = "Username";
         dgv.Columns.Add("DownloadTime", "Download Time");
         dgv.Columns["DownloadTime"].DataPropertyName = "DownloadTime";
         dgv.Columns.Add("Deadline", "Deadline");
         dgv.Columns["Deadline"].DataPropertyName = "PreferredDeadline";
         dgv.Columns.Add("Dummy", String.Empty);
         //dataGridView1.Columns["Dummy"].DataPropertyName = "Dummy";
         // ReSharper restore PossibleNullReferenceException
      }
   }
}
