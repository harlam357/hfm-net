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
using HFM.Core.WorkUnits;

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
            Server,
            CpuType,
            OS,
            Memory,
            SmpCores,
            MachineId
        }
        // ReSharper restore UnusedMember.Local

        public event EventHandler<QueueIndexChangedEventArgs> QueueIndexChanged;

        private QueueDictionary _queue;
        private IProteinService _proteinService;

        private SlotType _slotType = SlotType.Unknown;

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
            SetQueue(queue, SlotType.Unknown);
        }

        public void SetQueue(QueueDictionary queue, SlotType type)
        {
            if (queue != null)
            {
                _queue = queue;
                _slotType = type;

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
                    txtBeginDate.Text = String.Format("{0} {1}", item.BeginTimeLocal.ToShortDateString(), item.BeginTimeLocal.ToShortTimeString());
                }
                txtServer.Text = item.ServerIP;
                txtCpuType.Text = item.CpuString;
                txtOsType.Text = item.OsString;
                txtMemory.Text = item.Memory.ToString(CultureInfo.CurrentCulture);
                txtSmpCores.Text = item.NumberOfSmpCores.ToString(CultureInfo.CurrentCulture);
                txtMachineID.Text = item.MachineID.ToString(CultureInfo.CurrentCulture);

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
            txtServer.Visible = visible;
            txtCpuType.Visible = visible;
            txtOsType.Visible = visible;
            txtMemory.Visible = visible;
            txtSmpCores.Visible = visible;
            txtMachineID.Visible = visible;

            if (visible == false)
            {
                tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
            }
            else
            {
                switch (_slotType)
                {
                    case SlotType.Unknown:
                        lblCpuType.Text = "CPU Type:";
                        txtSmpCores.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                        break;
                    case SlotType.GPU:
                        lblCpuType.Text = "GPU Type:";
                        txtSmpCores.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = 0;
                        break;
                    case SlotType.CPU:
                        lblCpuType.Text = "CPU Type:";
                        txtSmpCores.Visible = true;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.SmpCores].Height = DefaultRowHeight;
                        break;
                }
            }
        }
    }

    [ExcludeFromCodeCoverage]
    public class QueueIndexChangedEventArgs : EventArgs
    {
        public int Index { get; }

        public QueueIndexChangedEventArgs(int index)
        {
            Index = index;
        }
    }
}
