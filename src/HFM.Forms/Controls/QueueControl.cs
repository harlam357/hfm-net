
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

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
            BaseCredit,
            BeginDate,
            WorkServer,
            CPU,
            OS,
            Memory,
            CPUThreads,
            MachineID
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
                DisplayMember = String.Format(CultureInfo.InvariantCulture, "{0:00} - {1}", kvp.Key, kvp.Value.ToShortProjectString()),
                ValueMember = kvp.Key
            }).ToList();
        }

        private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_slotWorkUnitInfos == null) return;

            if (cboQueueIndex.SelectedIndex > -1)
            {
                SetControlsVisible(true);

                SlotWorkUnitInfo info = _slotWorkUnitInfos[(int)cboQueueIndex.SelectedValue];
                StatusTextBox.Text = info.State;
                WaitingOnTextBox.Text = String.IsNullOrEmpty(info.WaitingOn) ? "(No Action)" : info.WaitingOn;
                AttemptsTextBox.Text = info.Attempts.ToString();
                NextAttemptTextBox.Text = info.NextAttempt.ToString();
                var protein = _proteinService.Get(info.ProjectID);
                BaseCreditTextBox.Text = protein != null ? protein.Credit.ToString(CultureInfo.CurrentCulture) : "0";
                AssignedTextBox.Text = FormatAssignedDateTimeUtc(info.AssignedDateTimeUtc);
                WorkServerTextBox.Text = info.WorkServer;
                CPUTypeTextBox.Text = info.CPU;
                OSTextBox.Text = info.OperatingSystem;
                MemoryTextBox.Text = info.Memory.ToString(CultureInfo.CurrentCulture);
                CPUThreadsTextBox.Text = info.CPUThreads.ToString(CultureInfo.CurrentCulture);
                MachineIDTextBox.Text = info.SlotID.ToString(CultureInfo.CurrentCulture);

                OnQueueIndexChanged(new QueueIndexChangedEventArgs((int)cboQueueIndex.SelectedValue));
            }
            else
            {
                // hide controls and display queue not available message
                SetControlsVisible(false);

                OnQueueIndexChanged(new QueueIndexChangedEventArgs(-1));
            }
        }

        private static string FormatAssignedDateTimeUtc(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return "(Unknown)";
            }

            var localTime = value.ToLocalTime();
            return $"{localTime.ToShortDateString()} {localTime.ToShortTimeString()}";
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

            StatusTextBox.Visible = visible;
            WaitingOnTextBox.Visible = visible;
            AttemptsTextBox.Visible = visible;
            NextAttemptTextBox.Visible = visible;
            BaseCreditTextBox.Visible = visible;
            AssignedTextBox.Visible = visible;
            WorkServerTextBox.Visible = visible;
            CPUTypeTextBox.Visible = visible;
            OSTextBox.Visible = visible;
            MemoryTextBox.Visible = visible;
            CPUThreadsTextBox.Visible = visible;
            MachineIDTextBox.Visible = visible;

            if (visible == false)
            {
                tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
            }
            else
            {
                switch (_slotType)
                {
                    case SlotType.Unknown:
                        CPULabel.Text = "CPU:";
                        CPUThreadsTextBox.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
                        break;
                    case SlotType.GPU:
                        CPULabel.Text = "GPU:";
                        CPUThreadsTextBox.Visible = false;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = 0;
                        break;
                    case SlotType.CPU:
                        CPULabel.Text = "CPU:";
                        CPUThreadsTextBox.Visible = true;
                        tableLayoutPanel1.RowStyles[(int)QueueControlRows.CPUThreads].Height = DefaultRowHeight;
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
