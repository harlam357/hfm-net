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
using HFM.Core.Client;
using HFM.Core.WorkUnits;

namespace HFM.Forms.Controls
{
    // TODO: Rename to WorkUnitInfoControl
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

        private SlotWorkUnitDictionary _slotWorkUnitInfos;
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
            QueueIndexChanged?.Invoke(this, e);
        }

        public void SetWorkUnitInfos(SlotWorkUnitDictionary workUnitInfos)
        {
            SetWorkUnitInfos(workUnitInfos, SlotType.Unknown);
        }

        public void SetWorkUnitInfos(SlotWorkUnitDictionary workUnitInfos, SlotType type)
        {
            if (workUnitInfos != null)
            {
                _slotWorkUnitInfos = workUnitInfos;
                _slotType = type;

                cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
                cboQueueIndex.DataSource = CreateEntryNameCollection(_slotWorkUnitInfos);
                cboQueueIndex.DisplayMember = "DisplayMember";
                cboQueueIndex.ValueMember = "ValueMember";
                cboQueueIndex.SelectedIndex = -1;
                cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;

                cboQueueIndex.SelectedValue = _slotWorkUnitInfos.CurrentWorkUnitKey;
            }
            else
            {
                _slotWorkUnitInfos = null;
                _slotType = SlotType.Unknown;
                SetControlsVisible(false);
            }
        }

        private static ICollection<ListItem> CreateEntryNameCollection(SlotWorkUnitDictionary slotWorkUnit)
        {
            return slotWorkUnit.Select(kvp => new ListItem
            {
                DisplayMember = String.Format(CultureInfo.InvariantCulture, "{0} - {1}", kvp.Key, kvp.Value.ToShortProjectString()),
                ValueMember = kvp.Key
            }).ToList().AsReadOnly();
        }

        private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_slotWorkUnitInfos == null) return;

            if (cboQueueIndex.SelectedIndex > -1)
            {
                SetControlsVisible(true);

                SlotWorkUnitInfo info = _slotWorkUnitInfos[(int)cboQueueIndex.SelectedValue];
                txtStatus.Text = info.State;
                WaitingOnTextBox.Text = String.IsNullOrEmpty(info.WaitingOn) ? "(No Action)" : info.WaitingOn;
                AttemptsTextBox.Text = info.Attempts.ToString();
                NextAttemptTextBox.Text = info.NextAttempt.ToString();
                var protein = _proteinService.Get(info.ProjectID);
                txtCredit.Text = protein != null ? protein.Credit.ToString(CultureInfo.CurrentCulture) : "0";
                if (info.AssignedDateTimeUtc.IsUnknown())
                {
                    txtBeginDate.Text = "(Unknown)";
                }
                else
                {
                    var localTime = info.AssignedDateTimeUtc.ToLocalTime();
                    txtBeginDate.Text = $"{localTime.ToShortDateString()} {localTime.ToShortTimeString()}";
                }
                txtServer.Text = info.WorkServer;
                txtCpuType.Text = info.CPU;
                txtOsType.Text = info.OperatingSystem;
                txtMemory.Text = info.Memory.ToString(CultureInfo.CurrentCulture);
                txtSmpCores.Text = info.NumberOfSmpCores.ToString(CultureInfo.CurrentCulture);
                txtMachineID.Text = info.SlotID.ToString(CultureInfo.CurrentCulture);

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
