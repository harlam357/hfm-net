/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;

namespace HFM.Forms.Controls
{
   [ExcludeFromCodeCoverage]
   public sealed partial class QueueControl : UserControl
   {
      // ReSharper disable UnusedMember.Local
      private enum QueueControlRows
      {
         IndexCombo = 0,
         Blank1,
         Status,
         WaitingOn,
         Attempts,
         NextAttempt,
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
   
      private QueueDictionary _queue;
      private IProteinService _proteinService;

      private SlotType _slotType = SlotType.Unknown;
      private bool _utcOffsetIsZero;
      
      private const int DefaultRowHeight = 23;
   
      public QueueControl()
      {
         InitializeComponent();
      }
      
      public void SetProteinService(IProteinService proteinService)
      {
         _proteinService = proteinService;
      }

      private void OnQueueIndexChanged(QueueIndexChangedEventArgs e)
      {
         if (QueueIndexChanged != null)
         {
            QueueIndexChanged(this, e);
         }
      }
      
      public void SetQueue(QueueDictionary queue)
      {
         SetQueue(queue, SlotType.Unknown, false);
      }

      public void SetQueue(QueueDictionary queue, SlotType type, bool utcOffsetIsZero)
      {
         if (queue != null)
         {
            _queue = queue;
            _slotType = type;
            _utcOffsetIsZero = utcOffsetIsZero;
            
            cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
            cboQueueIndex.DataSource = CreateEntryNameCollection(_queue);
            cboQueueIndex.DisplayMember = "DisplayMember";
            cboQueueIndex.ValueMember = "ValueMember";
            cboQueueIndex.SelectedIndex = -1;
            cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;

            cboQueueIndex.SelectedValue = _queue.CurrentIndex;
         }
         else
         {
            _queue = null;
            _slotType = SlotType.Unknown;
            _utcOffsetIsZero = false;
            SetControlsVisible(false);
         }
      }

      private static ICollection<ListItem> CreateEntryNameCollection(QueueDictionary queue)
      {
         return queue.Select(kvp => new ListItem
         {
            DisplayMember = String.Format(CultureInfo.InvariantCulture, "{0} - {1}", kvp.Key, kvp.Value.ToShortProjectString()),
            ValueMember = kvp.Key
         }).ToList().AsReadOnly();
      }

      private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (_queue == null) return;
      
         if (cboQueueIndex.SelectedIndex > -1)
         {
            SetControlsVisible(true);

            QueueUnitItem item = _queue[(int)cboQueueIndex.SelectedValue];
            txtStatus.Text = item.EntryStatusLiteral;
            WaitingOnTextBox.Text = String.IsNullOrEmpty(item.WaitingOn) ? "(No Action)" : item.WaitingOn;
            AttemptsTextBox.Text = item.Attempts.ToString();
            NextAttemptTextBox.Text = item.NextAttempt.ToString();
            var protein = _proteinService.Get(item.ProjectID);
            txtCredit.Text = protein != null ? protein.Credit.ToString(CultureInfo.CurrentCulture) : "0";
            if (item.BeginTimeUtc.IsUnknown())
            {
               txtBeginDate.Text = "(Unknown)";
            }
            else
            {
               if (_utcOffsetIsZero)
               {
                  txtBeginDate.Text = String.Format("{0} {1}", item.BeginTimeUtc.ToShortDateString(), item.BeginTimeUtc.ToShortTimeString());
               }
               else
               {
                  txtBeginDate.Text = String.Format("{0} {1}", item.BeginTimeLocal.ToShortDateString(), item.BeginTimeLocal.ToShortTimeString());
               }
            }
            if (item.EndTimeUtc.Equals(new DateTime(2000, 1, 1, 0, 0, 0, DateTimeKind.Utc)))
            {
               txtEndDate.Text = "(Not Completed)";
            }
            else
            {
               if (_utcOffsetIsZero)
               {
                  txtEndDate.Text = String.Format("{0} {1}", item.EndTimeUtc.ToShortDateString(), item.EndTimeUtc.ToShortTimeString());
               }
               else
               {
                  txtEndDate.Text = String.Format("{0} {1}", item.EndTimeLocal.ToShortDateString(), item.EndTimeLocal.ToShortTimeString());
               }
               
            }
            txtSpeedFactor.Text = String.Format(CultureInfo.CurrentCulture, "{0} x min speed", item.SpeedFactor);
            txtPerformanceFraction.Text = String.Format(CultureInfo.CurrentCulture, "{0} (u={1})", _queue.PerformanceFraction, _queue.PerformanceFractionUnitWeight);
            txtMegaFlops.Text = String.Format(CultureInfo.CurrentCulture, "{0:f}", item.MegaFlops);
            txtServer.Text = item.ServerIP;
            txtAverageDownloadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _queue.DownloadRateAverage, _queue.DownloadRateUnitWeight);
            txtAverageUploadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _queue.UploadRateAverage, _queue.UploadRateUnitWeight);
            txtCpuType.Text = item.CpuString;
            txtOsType.Text = item.OsString;
            txtMemory.Text = item.Memory.ToString(CultureInfo.CurrentCulture);
            txtBenchmark.Text = item.Benchmark.ToString(CultureInfo.CurrentCulture);
            txtSmpCores.Text = item.NumberOfSmpCores.ToString(CultureInfo.CurrentCulture);
            txtCoresToUse.Text = item.UseCores.ToString(CultureInfo.CurrentCulture);
            txtUserID.Text = item.UserID;
            txtMachineID.Text = item.MachineID.ToString(CultureInfo.CurrentCulture);
            
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
            
            OnQueueIndexChanged(new QueueIndexChangedEventArgs((int)cboQueueIndex.SelectedValue));
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
         WaitingOnTextBox.Visible = visible;
         AttemptsTextBox.Visible = visible;
         NextAttemptTextBox.Visible = visible;
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
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = 0;
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
            tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
         }
         else
         {
            SetControlsForClientType(_queue.ClientType);

            switch (_slotType)
            {
               case SlotType.Unknown:
               //case SlotType.Uniprocessor:
                  lblCpuType.Text = "CPU Type:";
                  txtBenchmark.Visible = _queue.ClientType.Equals(ClientType.Legacy);
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = _queue.ClientType.Equals(ClientType.Legacy) ? DefaultRowHeight : 0;
                  txtSmpCores.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                  txtCoresToUse.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
                  break;
               case SlotType.GPU:
                  lblCpuType.Text = _queue.ClientType.Equals(ClientType.Legacy) ? "CPU Type:" : "GPU Type:";
                  txtBenchmark.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = 0;
                  txtSmpCores.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                  txtCoresToUse.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = 0;
                  break;
               case SlotType.CPU:
                  lblCpuType.Text = "CPU Type:";
                  txtBenchmark.Visible = false;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.Benchmark].Height = 0;
                  txtSmpCores.Visible = true;
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = DefaultRowHeight;
                  txtCoresToUse.Visible = _queue.ClientType.Equals(ClientType.Legacy);
                  tableLayoutPanel1.RowStyles[(int)QueueControlRows.CoresToUse].Height = _queue.ClientType.Equals(ClientType.Legacy) ? DefaultRowHeight : 0;
                  break;
            }
         }
      }

      private void SetControlsForClientType(ClientType type)
      {
         bool legacyVisible = type.Equals(ClientType.Legacy);
         int legacyHeight = type.Equals(ClientType.Legacy) ? DefaultRowHeight : 0;

         txtEndDate.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.EndDate].Height = legacyHeight;
         txtSpeedFactor.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.SpeedFactor].Height = legacyHeight;
         txtPerformanceFraction.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.PerfFraction].Height = legacyHeight;
         txtMegaFlops.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.MegaFlops].Height = legacyHeight;
         txtAverageDownloadRate.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.AvgDownload].Height = legacyHeight;
         txtAverageUploadRate.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.AvgUpload].Height = legacyHeight;
         txtUserID.Visible = legacyVisible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.UserId].Height = legacyHeight;

         bool visible = type.Equals(ClientType.FahClient);
         int height = type.Equals(ClientType.FahClient) ? DefaultRowHeight : 0;

         WaitingOnTextBox.Visible = visible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.WaitingOn].Height = height;
         AttemptsTextBox.Visible = visible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.Attempts].Height = height;
         NextAttemptTextBox.Visible = visible;
         tableLayoutPanel1.RowStyles[(int)QueueControlRows.NextAttempt].Height = height;
      }
   }
   
   [ExcludeFromCodeCoverage]
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
