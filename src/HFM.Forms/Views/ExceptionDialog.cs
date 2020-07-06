
using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HFM.Forms.Internal;

namespace HFM.Forms
{
    public partial class ExceptionDialog : Form
    {
        public delegate void LogException(Exception ex);

        private static bool _registered;
        private static string _applicationIdStatic;
        private static string _osVersionStatic;
        private static LogException _exceptionLogger;

        public static void RegisterForUnhandledExceptions(string applicationId)
        {
            RegisterForUnhandledExceptions(applicationId, null);
        }

        public static void RegisterForUnhandledExceptions(string applicationId, string osVersion)
        {
            RegisterForUnhandledExceptions(applicationId, osVersion, null);
        }

        public static void RegisterForUnhandledExceptions(string applicationId, string osVersion, LogException exceptionLogger)
        {
            if (_registered)
            {
                throw new InvalidOperationException("Exception Dialog is already registered.");
            }
            _applicationIdStatic = applicationId;
            _osVersionStatic = osVersion;
            _exceptionLogger = exceptionLogger;
            Application.ThreadException += ShowErrorDialog;
            _registered = true;
        }

        private static void ShowErrorDialog(object sender, ThreadExceptionEventArgs e)
        {
            ShowErrorDialog(e.Exception, _applicationIdStatic, _osVersionStatic, null);
        }

        public static void ShowErrorDialog(Exception exception, string applicationId, string osVersion, string message)
        {
            ShowErrorDialog(exception, applicationId, osVersion, message, false);
        }

        public static void ShowErrorDialog(Exception exception, string applicationId, string osVersion, string message, bool mustTerminate)
        {
            ShowErrorDialog(exception, applicationId, osVersion, message, null, mustTerminate);
        }

        public static void ShowErrorDialog(Exception exception, string applicationId, string osVersion, string message, string reportUrl)
        {
            ShowErrorDialog(exception, applicationId, osVersion, message, reportUrl, false);
        }

        public static void ShowErrorDialog(Exception exception, string applicationId, string osVersion, string message, string reportUrl, bool mustTerminate)
        {
            if (_exceptionLogger != null) _exceptionLogger(exception);
            try
            {
                using (ExceptionDialog box = new ExceptionDialog(exception, applicationId, osVersion, message, reportUrl, mustTerminate))
                {
                    box.ShowDialog();
                }
            }
            catch (Exception ex)
            {
                if (_exceptionLogger != null) _exceptionLogger(exception);
                MessageBox.Show(ex.ToString(), message, MessageBoxButtons.OK, MessageBoxIcon.Error,
                   MessageBoxDefaultButton.Button1, MessageBoxOptions.DefaultDesktopOnly);
            }
        }

        private readonly Exception _exceptionThrown;
        private readonly string _applicationId;
        private readonly string _osVersion;
        private readonly string _message;
        private readonly string _reportUrl;

        public ExceptionDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Creates a new ExceptionDialog instance.
        /// </summary>
        /// <param name="exception">The exception to display</param>
        /// <param name="applicationId">The application name and version to display</param>
        /// <param name="osVersion">The operating system version to display</param>
        /// <param name="message">An additional message to display</param>
        /// <param name="reportUrl">Override configured target URL for report button</param>
        /// <param name="mustTerminate">If <paramref name="mustTerminate"/> is true, the continue button is not available.</param>
        public ExceptionDialog(Exception exception, string applicationId, string osVersion, string message, string reportUrl, bool mustTerminate)
        {
            _exceptionThrown = exception;
            _applicationId = applicationId;
            _osVersion = osVersion;
            _message = message;
            _reportUrl = reportUrl;

            InitializeComponent();

            if (mustTerminate)
            {
                btnExit.Visible = false;
                btnContinue.Text = btnExit.Text;
                btnContinue.Left -= btnExit.Width - btnContinue.Width;
                btnContinue.Width = btnExit.Width;
            }

            exceptionTextBox.Text = GetClipboardString();
        }

        string GetClipboardString()
        {
            StringBuilder sb = new StringBuilder();
            if (_applicationId != null)
            {
                sb.AppendLine(_applicationId);
                sb.AppendLine();
            }
            if (_osVersion != null)
            {
                sb.AppendLine(_osVersion);
                sb.AppendLine();
            }
            if (_message != null)
            {
                sb.AppendLine(_message);
                sb.AppendLine();
            }
            sb.AppendLine("Exception Thrown:");
            sb.AppendLine(_exceptionThrown.ToString());
            return sb.ToString();
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            CopyInfoToClipboard();
            string reportUrl = String.IsNullOrEmpty(_reportUrl) ? Properties.Settings.Default.ReportUrl : _reportUrl;
            if (String.IsNullOrEmpty(reportUrl) == false)
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
                    Thread th = new Thread((ThreadStart)delegate
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
                if (_exceptionLogger != null) _exceptionLogger(ex);
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
