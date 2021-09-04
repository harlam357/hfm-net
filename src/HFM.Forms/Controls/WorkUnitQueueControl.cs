using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.WorkUnits;

using Unknown = HFM.Core.Unknown;

namespace HFM.Forms.Controls
{
    public sealed partial class WorkUnitQueueControl : UserControl
    {
        public IProteinService ProteinService { get; set; }

        public WorkUnitQueueControl()
        {
            InitializeComponent();
        }

        private WorkUnitQueueItemCollection _workUnitQueue;

        public void SetWorkUnitQueue(WorkUnitQueueItemCollection workUnitQueue)
        {
            if (workUnitQueue is null)
            {
                _workUnitQueue = null;
                SetControlsVisible(false);
            }
            else
            {
                _workUnitQueue = workUnitQueue;

                cboQueueIndex.SelectedIndexChanged -= cboQueueIndex_SelectedIndexChanged;
                cboQueueIndex.DataSource = CreateListItemCollection(_workUnitQueue);
                cboQueueIndex.DisplayMember = nameof(ListItem.DisplayMember);
                cboQueueIndex.ValueMember = nameof(ListItem.ValueMember);
                cboQueueIndex.SelectedIndex = -1;
                cboQueueIndex.SelectedIndexChanged += cboQueueIndex_SelectedIndexChanged;

                cboQueueIndex.SelectedValue = _workUnitQueue.CurrentID;
            }
        }

        private static ICollection<ListItem> CreateListItemCollection(WorkUnitQueueItemCollection workUnitQueue)
        {
            return workUnitQueue
                .Select(x => new ListItem(FormatDisplay(x), x.ID))
                .ToList();

            string FormatDisplay(WorkUnitQueueItem workUnit)
            {
                return String.Format(CultureInfo.InvariantCulture, "WU{0:00}:FS{1:00}:{2}", workUnit.ID, workUnit.SlotID, workUnit.ToShortProjectString());
            }
        }

        private void cboQueueIndex_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_workUnitQueue is null) return;

            if (cboQueueIndex.SelectedIndex > -1)
            {
                SetControlsVisible(true);

                WorkUnitQueueItem item = _workUnitQueue[(int)cboQueueIndex.SelectedValue];
                StatusTextBox.Text = item.State;
                WaitingOnTextBox.Text = item.WaitingOn;
                AttemptsTextBox.Text = item.Attempts.ToString(CultureInfo.CurrentCulture);
                NextAttemptTextBox.Text = FormatNextAttempt(item.NextAttempt);
                var protein = ProteinService.Get(item.ProjectID);
                BaseCreditTextBox.Text = protein != null ? protein.Credit.ToString(CultureInfo.CurrentCulture) : "0";
                AssignedTextBox.Text = FormatAsLocalDateTime(item.Assigned);
                TimeoutTextBox.Text = FormatAsLocalDateTime(item.Timeout);
                ExpirationTextBox.Text = FormatAsLocalDateTime(item.Deadline);
                WorkServerTextBox.Text = item.WorkServer;
                CollectServerTextBox.Text = item.CollectionServer;
                OSTextBox.Text = item.OperatingSystem;
                MemoryTextBox.Text = item.Memory.ToString(CultureInfo.CurrentCulture);
                CPUThreadsTextBox.Text = item.CPUThreads.ToString(CultureInfo.CurrentCulture);
            }
            else
            {
                SetControlsVisible(false);
            }
        }

        private static string FormatNextAttempt(TimeSpan value) => value == TimeSpan.Zero ? Unknown.Value : value.ToString();

        private static string FormatAsLocalDateTime(DateTime value)
        {
            if (value == DateTime.MinValue)
            {
                return Unknown.Value;
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

            foreach (var c in tableLayoutPanel1.Controls)
            {
                if (c is TextBox textBox)
                {
                    textBox.Visible = visible;
                }
            }
        }
    }
}
