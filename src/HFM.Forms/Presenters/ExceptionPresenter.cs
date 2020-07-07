
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Windows.Forms;

using HFM.Core.Logging;
using HFM.Forms.Internal;

namespace HFM.Forms
{
    public class ExceptionPresenter : IDisposable
    {
        public ILogger Logger { get; }
        public MessageBoxPresenter MessageBox { get; }
        public IDictionary<string, string> Properties { get; }
        public string ReportUrl { get; }

        public ExceptionPresenter(ILogger logger, MessageBoxPresenter messageBox, IDictionary<string, string> properties, string reportUrl)
        {
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            Properties = properties;
            ReportUrl = reportUrl;
        }

        public Exception Exception { get; protected set; }
        public bool MustTerminate { get; protected set; }
        public IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner, Exception exception, bool mustTerminate)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            MustTerminate = mustTerminate;

            Dialog = new ExceptionDialog(this);
            return Dialog.ShowDialog(owner);
        }

        public void Dispose()
        {
            Dialog?.Dispose();
        }

        public string BuildExceptionText()
        {
            var sb = new StringBuilder();
            if (Properties != null)
            {
                foreach (var property in Properties)
                {
                    sb.AppendLine($"{property.Key}: {property.Value}");
                    sb.AppendLine();
                }
            }
            sb.AppendLine("Exception Thrown:");
            sb.AppendLine(Exception.ToString());
            return sb.ToString();
        }

        public void ReportClicked(bool copyToClipboard)
        {
            CopyInfoToClipboard(copyToClipboard);
            if (!String.IsNullOrEmpty(ReportUrl))
            {
                StartUrl(ReportUrl);
            }
        }

        private void CopyInfoToClipboard(bool copyToClipboard)
        {
            if (!copyToClipboard) return;

            // TODO: Replace ClipboardWrapper.SetText() with abstraction
            
            string exceptionText = BuildExceptionText();
            if (Application.OleRequired() == ApartmentState.STA)
            {
                ClipboardWrapper.SetText(exceptionText);
            }
            else
            {
                var thread = new Thread((ThreadStart)delegate
                {
                    ClipboardWrapper.SetText(exceptionText);
                });
                thread.Name = nameof(CopyInfoToClipboard);
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
            }
        }

        private void StartUrl(string url)
        {
            try
            {
                // TODO: Replace Process.Start() with abstraction
                Process.Start(url);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Dialog, ex.Message, Core.Application.NameAndVersion);
            }
        }

        public void ContinueClicked()
        {
            Dialog.DialogResult = DialogResult.Ignore;
            Dialog.Close();
        }

        public void ExitClicked()
        {
            var result = MessageBox.AskYesNoQuestion(Dialog, "Are you sure you want to exit the application?", Core.Application.NameAndVersion);
            if (result == DialogResult.Yes)
            {
                // TODO: Replace Application.Exit() with abstraction
                Application.Exit();
            }
        }
    }

    public class NullExceptionPresenter : ExceptionPresenter
    {
        public static NullExceptionPresenter Instance { get; } = new NullExceptionPresenter();

        public NullExceptionPresenter() : base(null, null, null, null)
        {
        }

        public override DialogResult ShowDialog(IWin32Window owner, Exception exception, bool mustTerminate)
        {
            Exception = exception;
            MustTerminate = mustTerminate;

            return DialogResult.Ignore;
        }
    }
}
