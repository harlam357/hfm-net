using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core.Logging;
using HFM.Forms.Controls;
using HFM.Preferences;

namespace HFM.Forms.Views
{
    public interface IMessagesView
    {
        void ScrollToEnd();

        void SetManualStartPosition();

        void SetLocation(int x, int y);

        void SetSize(int width, int height);

        void Show();

        void Close();

        bool Visible { get; set; }
    }

    public partial class MessagesForm : FormWrapper, IMessagesView
    {
        private const int MaxLines = 512;

        private readonly IPreferenceSet _prefs;
        private readonly ILoggerEvents _loggerEvents;
        private readonly List<string> _lines = new List<string>(MaxLines);

        public MessagesForm(IPreferenceSet prefs, ILoggerEvents loggerEvents)
        {
            _prefs = prefs;
            _loggerEvents = loggerEvents;
            _loggerEvents.Logged += (s, e) => AddMessage(e.Messages);

            InitializeComponent();
        }

        private void AddMessage(ICollection<string> messages)
        {
            if ((_lines.Count + messages.Count) > MaxLines)
            {
                _lines.RemoveRange(0, MaxLines / 4);
            }
            _lines.AddRange(messages);

            UpdateMessages(_lines.ToArray());
        }

        public void ScrollToEnd()
        {
            txtMessages.SelectionStart = txtMessages.Text.Length;
            txtMessages.ScrollToCaret();
        }

        public void SetManualStartPosition()
        {
            StartPosition = FormStartPosition.Manual;
        }

        public void SetLocation(int x, int y)
        {
            Location = new Point(x, y);
        }

        public void SetSize(int width, int height)
        {
            Size = new Size(width, height);
        }

        private delegate void UpdateMessagesDelegate(string[] lines);

        private void UpdateMessages(string[] lines)
        {
            if (InvokeRequired)
            {
                txtMessages.BeginInvoke(new UpdateMessagesDelegate(UpdateMessages), new object[] { lines });
                return;
            }

            txtMessages.Lines = lines;
            ScrollToEnd();
        }

        private void txtMessages_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F7)
            {
                // Close on F7 - Issue 74
                Close();
            }
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            // Save state data
            if (WindowState == FormWindowState.Normal)
            {
                _prefs.Set(Preference.MessagesFormLocation, Location);
                _prefs.Set(Preference.MessagesFormSize, Size);
                _prefs.Save();
            }

            Hide();
            e.Cancel = true;
            base.OnClosing(e);
        }
    }
}
