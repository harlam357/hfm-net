
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HFM.Forms.Internal;

namespace HFM.Forms
{
    public partial class ExceptionDialog : Form, IWin32Dialog
    {
        private readonly ExceptionPresenter _presenter;
        
        public ExceptionDialog(ExceptionPresenter presenter)
        {
            _presenter = presenter;

            InitializeComponent();

            if (_presenter.MustTerminate)
            {
                btnExit.Visible = false;
                btnContinue.Text = btnExit.Text;
                btnContinue.Left -= btnExit.Width - btnContinue.Width;
                btnContinue.Width = btnExit.Width;
            }

            exceptionTextBox.Text = GetClipboardString();
        }

        private string GetClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            if (_presenter.Properties != null)
            {
                foreach (var property in _presenter.Properties)
                {
                    sb.AppendLine($"{property.Key}: {property.Value}");
                    sb.AppendLine();
                }
            }
            sb.AppendLine("Exception Thrown:");
            sb.AppendLine(_presenter.Exception.ToString());
            return sb.ToString();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            CopyInfoToClipboard();
            string reportUrl = _presenter.ReportUrl;
            if (!String.IsNullOrEmpty(reportUrl))
            {
                StartUrl(reportUrl);
            }
        }

        private void CopyInfoToClipboard()
        {
            if (copyErrorCheckBox.Checked)
            {
                string exceptionText = exceptionTextBox.Text;
                if (Application.OleRequired() == ApartmentState.STA)
                {
                    ClipboardWrapper.SetText(exceptionText);
                }
                else
                {
                    var th = new Thread((ThreadStart)delegate
                    {
                        ClipboardWrapper.SetText(exceptionText);
                    });
                    th.Name = "CopyInfoToClipboard";
                    th.SetApartmentState(ApartmentState.STA);
                    th.Start();
                }
            }
        }

        private void StartUrl(string url)
        {
            try
            {
                Process.Start(url);
            }
            catch (Exception ex)
            {
                _presenter.Logger.Error(ex.Message, ex);
                MessageBox.Show(ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error,
                   MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private void btnContinue_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Ignore;
            Close();
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("Are you sure you want to exit the application?.", Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2,
                MessageBoxOptions.DefaultDesktopOnly) == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}
