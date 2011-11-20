/*
 * HFM.NET - Queue Control
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System;
using System.Globalization;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Controls
{
   public sealed partial class QueueControl : UserControl
   {
      // ReSharper disable UnusedMember.Local
      private enum QueueControlRows
      {
         IndexCombo = 0,
         Blank1,
         Status,
         Credit,
         BeginDate,
         EndDate,
         SpeedFactor,
         PerfFraction,
         MegaFlops,
         Server,
         AvgDownload,
         AvgUpload,
         CpuType,
         OS,
         Memory,
         Benchmark,
         SmpCores,
         CoresToUse,
         UserId,
         MachineId
      }
      // ReSharper restore UnusedMember.Local
   
      public event EventHandler<QueueIndexChangedEventArgs> QueueIndexChanged;
   
      private ClientQueue _qBase;
      private IProteinDictionary _proteinCollection;

      private SlotType _slotType = SlotType.Unknown;
      private bool _utcOffsetIsZero;
      
      private const int DefaultRowHeight = 23;
   
      public QueueControl()
      {
         InitializeComponent();
      }
      
      public void SetProteinCollection(IProteinDictionary proteinCollection)
      {
         _proteinCollection = proteinCollection;
      }

      private void OnQueueIndexChanged(QueueIndexChangedEventArgs e)
      {
         if (QueueIndexChanged != null)
         {
            QueueIndexChanged(this, e);
         }
      }
      
      public void SetQueue(ClientQueue qBase)
      {
         SetQueue(qBase, SlotType.Unknown, false);
      }

      public void SetQueue(ClientQueue qBase, SlotType type, bool utcOffsetIsZero)
      {
         if (qBase != null)
         {
            _qBase = qBase;
            _slotType = type;
            _utcOffsetIsZero = utcOffsetIsZero;
            
            cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
            cboQueueIndex.DataSource = _qBase.EntryNameCollection;
            cboQueueIndex.SelectedIndex = -1;
            cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;

            cboQueueIndex.SelectedIndex = _qBase.CurrentIndex;
         }
         else
         {
            _qBase = null;
            _slotType = SlotType.Unknown;
            _utcOffsetIsZero = false;
            SetControlsVisible(false);
         }
      }

      private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (_qBase == null) return;
      
         if (cboQueueIndex.SelectedIndex > -1)
         {
            SetControlsVisible(true);

            ClientQueueEntry entry = _qBase.GetQueueEntry(cboQueueIndex.SelectedIndex);
            txtStatus.Text = entry.EntryStatusLiteral;
            txtCredit.Text = _proteinCollection.ContainsKey(entry.ProjectID) ? _proteinCollection[entry.ProjectID].Credit.ToString(CultureInfo.CurrentCulture) : "0";
            if (_utcOffsetIsZero)
            {
               txtBeginDate.Text = String.Format("{0} {1}", entry.BeginTimeUtc.ToShortDateString(), entry.BeginTimeUtc.ToShortTimeString());
            }
            else
            {
               txtBeginDate.Text = String.Format("{0} {1}", entry.BeginTimeLocal.ToShortDateString(), entry.BeginTimeLocal.ToShortTimeString());
            }
            if (entry.EndTimeUtc.Equals(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
               txtEndDate.Text = "(Not Completed)";
            }
            else
            {
               if (_utcOffsetIsZero)
               {
                  txtEndDate.Text = String.Format("{0} {1}", entry.EndTimeUtc.ToShortDateString(), entry.EndTimeUtc.ToShortTimeString());
               }
               else
               {
                  txtEndDate.Text = String.Format("{0} {1}", entry.EndTimeLocal.ToShortDateString(), entry.EndTimeLocal.ToShortTimeString());
               }
               
            }
            txtSpeedFactor.Text = String.Format(CultureInfo.CurrentCulture, "{0} x min speed", entry.SpeedFactor);
            txtPerformanceFraction.Text = String.Format(CultureInfo.CurrentCulture, "{0} (u={1})", _qBase.PerformanceFraction, _qBase.PerformanceFractionUnitWeight);
            txtMegaFlops.Text = String.Format(CultureInfo.CurrentCulture, "{0:f}", entry.MegaFlops);
            txtServer.Text = entry.ServerIP;
            txtAverageDownloadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _qBase.DownloadRateAverage, _qBase.DownloadRateUnitWeight);
            txtAverageUploadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _qBase.UploadRateAverage, _qBase.UploadRateUnitWeight);
            txtCpuType.Text = entry.CpuString;
            txtOsType.Text = entry.OsString;
            txtMemory.Text = entry.Memory.ToString(CultureInfo.CurrentCulture);
            txtBenchmark.Text = entry.Benchmark.ToString(CultureInfo.CurrentCulture);
            txtSmpCores.Text = entry.NumberOfSmpCores.ToString(CultureInfo.CurrentCulture);
            txtCoresToUse.Text = entry.UseCores.ToString(CultureInfo.CurrentCulture);
            txtUserID.Text = entry.UserID;
            txtMachineID.Text = entry.MachineID.ToString(CultureInfo.CurrentCulture);
            
            #region Test TextBox Code (commented)
            //txtStatus.Text = "Status";
            //txtCredit.Text = "Credit";
            //txtBeginDate.Text = "BeginDate";
            //txtEndDate.Text = "EndDate";
            //txtSpeedFactor.Text = "SpeedFactor";
            //txtPerformanceFraction.Text = "PerfFraction";
            //txtMegaFlops.Text = "MegaFlops";
            //txtServer.Text = "Server";
            //txtAverageDownloadRate.Text = "AvgDownload";
            //txtAverageUploadRate.Text = "AvgUpload";
            //txtCpuType.Text = "CpuType";
            //txtOsType.Text = "OsType";
            //txtMemory.Text = "Memory";
            //txtGpuMemory.Text = "GpuMemory";
            //txtBenchmark.Text = "Benchmark";
            //txtSmpCores.Text = "SmpCores";
            //txtCoresToUse.Text = "CoresToUse";
            //txtUserID.Text = "UserID";
            //txtMachineID.Text = "MachineID";
            #endregion
            
            OnQueueIndexChanged(new QueueIndexChangedEventArgs(cboQueueIndex.SelectedIndex));
         }
         else
         {
            // hide controls and display queue not available message
            SetControlsVisible(false);

            OnQueueIndexChanged(new QueueIndexChangedEventArgs(-1));
         }
      }
      
      private void SetControlsVisible(bool visible)
      {
         if (visible == false)
         {
            cboQueueIndex.DataSource = null;
            cboQueueIndex.Items.Clear();
            cboQueueIndex.Items.Add("No Queue Data");
            cboQueueIndex.SelectedIndex = 0;
         }

         txtStatus.Visible = visible;
         txtCredit.Visible = visible;
         txtBeginDate.Visible = visible;
         txtEndDate.Visible = visible;
         txtSpeedFactor.Visible = visible;
         txtPerformanceFraction.Visible = visible;
         txtMegaFlops.Visible = visible;
         txtServer.Visible = visible;
         txtAverageDownloadRate.Visible = visible;
         txtAverageUploadRate.Visible = visible;
         txtCpuType.Visible = visible;
         txtOsType.Visible = visible;
         txtMemory.Visible = visible;
         txtBenchmark.Visible = visible;
         txtSmpCores.Visible = visible;
         txtCoresToUse.Visible = visible;
         txtUserID.Visible = visible;
         txtMachineID.Visible = visible;
         
         if (visible == false)
         {
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = DefaultRowHeight;
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
         }
         else
         {
            switch (_slotType)
            {
               case SlotType.Unknown:
               case SlotType.Uniprocessor:
                  // Set Rows to Zero Height and Hide Labels First
                  txtSmpCores.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                  txtCoresToUse.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
                  // Then Show the Client Specific Queue Row(s)
                  txtBenchmark.Visible = true;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = DefaultRowHeight;
                  break;
               case SlotType.GPU:
                  // Set Rows to Zero Height and Hide Labels First
                  txtBenchmark.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = 0;
                  txtSmpCores.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                  txtCoresToUse.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
                  // Then Show the Client Specific Queue Row(s)
                  break;
               case SlotType.SMP:
                  // Set Rows to Zero Height and Hide Labels First
                  txtBenchmark.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = 0;
                  // Then Show the Client Specific Queue Row(s)
                  txtCoresToUse.Visible = true;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = DefaultRowHeight;
                  txtSmpCores.Visible = true;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = DefaultRowHeight;
                  break;
            }
         }
      }
   }
   
   public class QueueIndexChangedEventArgs : EventArgs
   {
      private readonly int _index;
      
      public int Index
      {
         get { return _index; }
      }
      
      public QueueIndexChangedEventArgs(int index)
      {
         _index = index;
      }
   }
}
