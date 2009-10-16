/*
 * HFM.NET - Queue Control
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using HFM.Instances;
using HFM.Proteins;

namespace HFM.Classes
{
   public sealed partial class QueueControl : UserControl
   {
      public event EventHandler<QueueIndexChangedEventArgs> QueueIndexChanged;
   
      private QueueReader _qr = null;
      private ClientType _ClientType = ClientType.Unknown;
      private bool _ClientIsOnVirtualMachine;
      
      private readonly Point _UserIdLabelLocation;
      private readonly Point _UserIdTextboxLocation;
      private readonly Point _MachineIdLabelLocation;
      private readonly Point _MachineTextboxLocation;
   
      public QueueControl()
      {
         InitializeComponent();
         
         _UserIdLabelLocation = lblUserID.Location;
         _UserIdTextboxLocation = txtUserID.Location;
         _MachineIdLabelLocation = lblMachineID.Location;
         _MachineTextboxLocation = txtMachineID.Location;
      }
      
      private void OnQueueIndexChanged(QueueIndexChangedEventArgs e)
      {
         if (QueueIndexChanged != null)
         {
            QueueIndexChanged(this, e);
         }
      }
      
      [CLSCompliant(false)]
      public void SetQueue(QueueReader qr)
      {
         SetQueue(qr, ClientType.Unknown, false);
      }

      [CLSCompliant(false)]
      public void SetQueue(QueueReader qr, ClientType type, bool vm)
      {
         if (qr != null && qr.QueueReadOk)
         {
            _qr = qr;
            _ClientType = type;
            _ClientIsOnVirtualMachine = vm;
            
            cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
            cboQueueIndex.DataSource = _qr.EntryNameCollection;
            cboQueueIndex.SelectedIndex = -1;
            cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;
            
            cboQueueIndex.SelectedIndex = (int)_qr.CurrentIndex;
         }
         else
         {
            _qr = null;
            _ClientType = ClientType.Unknown;
            _ClientIsOnVirtualMachine = false;
            SetControlsVisible(false);
         }
      }

      private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
      {
         if ((_qr != null && _qr.QueueReadOk) == false) return;
      
         if (cboQueueIndex.SelectedIndex > -1)
         {
            SetControlsVisible(true);
         
            QueueEntry entry = _qr.GetQueueEntry((uint)cboQueueIndex.SelectedIndex);
            txtStatus.Text = entry.EntryStatus.ToString();
            txtCredit.Text = ProteinCollection.Instance.ContainsKey(entry.ProjectID) ? ProteinCollection.Instance[entry.ProjectID].Credit.ToString(CultureInfo.CurrentCulture) : "0";
            if (_ClientIsOnVirtualMachine)
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
               if (_ClientIsOnVirtualMachine)
               {
                  txtEndDate.Text = String.Format("{0} {1}", entry.EndTimeUtc.ToShortDateString(), entry.EndTimeUtc.ToShortTimeString());
               }
               else
               {
                  txtEndDate.Text = String.Format("{0} {1}", entry.EndTimeLocal.ToShortDateString(), entry.EndTimeLocal.ToShortTimeString());
               }
               
            }
            txtSpeedFactor.Text = String.Format(CultureInfo.CurrentCulture, "{0} x min speed", entry.SpeedFactor);
            txtPerformanceFraction.Text = String.Format(CultureInfo.CurrentCulture, "{0} (u={1})", _qr.PerformanceFraction, _qr.PerformanceFractionUnitWeight);
            txtMegaFlops.Text = String.Format(CultureInfo.CurrentCulture, "{0:f}", entry.MegaFlops);
            txtServer.Text = entry.ServerIP;
            txtAverageDownloadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _qr.DownloadRateAverage, _qr.DownloadRateUnitWeight);
            txtAverageUploadRate.Text = String.Format(CultureInfo.CurrentCulture, "{0} KB/s (u={1})", _qr.UploadRateAverage, _qr.UploadRateUnitWeight);
            txtCpuType.Text = entry.CpuString;
            txtOsType.Text = entry.OsString;
            txtMemory.Text = entry.Memory.ToString(CultureInfo.CurrentCulture);
            txtGpuMemory.Text = entry.GpuMemory.ToString(CultureInfo.CurrentCulture);
            txtBenchmark.Text = entry.Benchmark.ToString(CultureInfo.CurrentCulture);
            txtSmpCores.Text = entry.NumberOfSmpCores.ToString(CultureInfo.CurrentCulture);
            txtCoresToUse.Text = entry.UseCores.ToString(CultureInfo.CurrentCulture);
            txtUserID.Text = entry.UserID;
            txtMachineID.Text = entry.MachineID.ToString(CultureInfo.CurrentCulture);
            
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
         
         if (visible == false)
         {
            lblGpuMemory.Visible = false;
            txtGpuMemory.Visible = false;
            lblBenchmark.Visible = true;
            txtBenchmark.Visible = false;
            lblSmpCores.Visible = false;
            txtSmpCores.Visible = false;
            lblCoresToUse.Visible = false;
            txtCoresToUse.Visible = false;
            lblUserID.Location = new Point(_UserIdLabelLocation.X, _UserIdLabelLocation.Y - 23);
            txtUserID.Location = new Point(_UserIdTextboxLocation.X, _UserIdTextboxLocation.Y - 23);
            lblMachineID.Location = new Point(_MachineIdLabelLocation.X, _MachineIdLabelLocation.Y - 23);
            txtMachineID.Location = new Point(_MachineTextboxLocation.X, _MachineTextboxLocation.Y - 23);
         }
         else
         {
            switch (_ClientType)
            {
               case ClientType.Unknown:
               case ClientType.Standard:
                  lblGpuMemory.Visible = false;
                  txtGpuMemory.Visible = false;
                  lblBenchmark.Visible = true;
                  txtBenchmark.Visible = true;
                  lblSmpCores.Visible = false;
                  txtSmpCores.Visible = false;
                  lblCoresToUse.Visible = false;
                  txtCoresToUse.Visible = false;
                  lblUserID.Location = new Point(_UserIdLabelLocation.X, _UserIdLabelLocation.Y - 23);
                  txtUserID.Location = new Point(_UserIdTextboxLocation.X, _UserIdTextboxLocation.Y - 23);
                  lblMachineID.Location = new Point(_MachineIdLabelLocation.X, _MachineIdLabelLocation.Y - 23);
                  txtMachineID.Location = new Point(_MachineTextboxLocation.X, _MachineTextboxLocation.Y - 23);
                  break;
               case ClientType.GPU:
                  lblGpuMemory.Visible = true;
                  txtGpuMemory.Visible = true;
                  lblBenchmark.Visible = false;
                  txtBenchmark.Visible = false;
                  lblSmpCores.Visible = false;
                  txtSmpCores.Visible = false;
                  lblCoresToUse.Visible = false;
                  txtCoresToUse.Visible = false;
                  lblUserID.Location = new Point(_UserIdLabelLocation.X, _UserIdLabelLocation.Y - 23);
                  txtUserID.Location = new Point(_UserIdTextboxLocation.X, _UserIdTextboxLocation.Y - 23);
                  lblMachineID.Location = new Point(_MachineIdLabelLocation.X, _MachineIdLabelLocation.Y - 23);
                  txtMachineID.Location = new Point(_MachineTextboxLocation.X, _MachineTextboxLocation.Y - 23);
                  break;
               case ClientType.SMP:
                  lblGpuMemory.Visible = false;
                  txtGpuMemory.Visible = false;
                  lblBenchmark.Visible = false;
                  txtBenchmark.Visible = false;
                  lblSmpCores.Visible = true;
                  txtSmpCores.Visible = true;
                  lblCoresToUse.Visible = true;
                  txtCoresToUse.Visible = true;
                  lblUserID.Location = _UserIdLabelLocation;
                  txtUserID.Location = _UserIdTextboxLocation;
                  lblMachineID.Location = _MachineIdLabelLocation;
                  txtMachineID.Location = _MachineTextboxLocation;
                  break;
            }
         }

         txtUserID.Visible = visible;
         txtMachineID.Visible = visible;
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
