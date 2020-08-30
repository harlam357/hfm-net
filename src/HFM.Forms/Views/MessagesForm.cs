using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class MessagesForm : FormBase, IWin32Form
    {
        private readonly MessagesPresenter _presenter;

        public MessagesForm(MessagesPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();

            LoadData(_presenter.Model);
        }

        private void LoadData(MessagesModel model)
        {
            var (location, size) = WindowPosition.Normalize(this, model.FormLocation, model.FormSize);

            Location = location;
            LocationChanged += (s, e) => model.FormLocation = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Size = size;
            SizeChanged += (s, e) => model.FormSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;

            UpdateMessages(model.Messages.ToArray());
            _presenter.Model.Messages.ListChanged += OnMessagesListChanged;
        }

        private void OnMessagesListChanged(object sender, ListChangedEventArgs e)
        {
            if (e.ListChangedType == ListChangedType.Reset)
            {
                UpdateMessages(_presenter.Model.Messages.ToArray());
            }
        }

        private void MessagesForm_Shown(object sender, EventArgs e)
        {
            ScrollToEnd();
        }

        private void ScrollToEnd()
        {
            txtMessages.SelectionStart = txtMessages.Text.Length;
            txtMessages.ScrollToCaret();
        }

        private void UpdateMessages(string[] lines)
        {
            if (InvokeRequired)
            {
                txtMessages.BeginInvoke(new Action<string[]>(UpdateMessages), new object[] { lines });
                return;
            }

            txtMessages.Lines = lines;
            ScrollToEnd();
        }

        private void txtMessages_KeyDown(object sender, KeyEventArgs e)
        {
            // Close on F7
            if (e.KeyCode == Keys.F7)
            {
                Close();
            }
        }
    }
}
