
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using HFM.Core.Logging;

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
            MessageBox = messageBox;
            Properties = properties;
            ReportUrl = reportUrl;
        }

        public Exception Exception { get; protected set; }
        public bool MustTerminate { get; protected set; }
        public IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner, Exception exception, bool mustTerminate)
        {
            Exception = exception;
            MustTerminate = mustTerminate;

            Dialog = new ExceptionDialog(this);
            return Dialog.ShowDialog(owner);
        }

        public void Dispose()
        {
            Dialog?.Dispose();
        }
    }
}
