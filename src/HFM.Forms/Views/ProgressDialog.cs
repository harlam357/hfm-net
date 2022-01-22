using System;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
using HFM.Forms.Controls;

namespace HFM.Forms.Views
{
    public delegate void ProgressFunction(IProgress<ProgressInfo> progress, CancellationToken cancellationToken);

    public sealed partial class ProgressDialog : FormBase
    {
        private readonly ProgressFunction _progressFunction;

        public bool SupportsCancellation { get; }

        public bool IsCanceled { get; private set; }

        public Exception Exception { get; private set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProgressDialog"/> class.
        /// </summary>
        public ProgressDialog(ProgressFunction progressFunction, bool supportsCancellation)
        {
            SupportsCancellation = supportsCancellation;
            _progressFunction = progressFunction;

            InitializeComponent();
            _baseSize = Size;
            SetCancellationControls();
        }

        private void ProcessCancelButtonClick(object sender, EventArgs e)
        {
            Debug.Assert(SupportsCancellation);

            if (_taskInProgress)
            {
                Debug.Assert(_cancellationTokenSource != null);
                _cancellationTokenSource.Cancel();
            }
        }

        private CancellationTokenSource _cancellationTokenSource;
        private bool _taskInProgress;

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            var progress = new Progress<ProgressInfo>();
            progress.ProgressChanged += (s, progressInfo) =>
            {
                int value = SanitizeProgressValue(progressInfo.ProgressPercentage);
                // Sets the progress bar value, without using Windows animation
                // https://stackoverflow.com/questions/5332616/disabling-net-progressbar-animation-when-changing-value
                if (value == progressBar.Maximum)
                {
                    progressBar.Maximum = value + 1;
                    progressBar.Value = value + 1;
                    progressBar.Maximum = value;
                }
                else
                {
                    progressBar.Value = value + 1;
                }

                progressBar.Value = value;
                if (progressInfo.Message is not null)
                {
                    messageLabel.Text = progressInfo.Message;
                }
            };

            await RunProgressFunction(progress);
            Close();
        }

        private int SanitizeProgressValue(int value)
        {
            if (value < 0)
            {
                return 0;
            }
            if (value > progressBar.Maximum)
            {
                return progressBar.Maximum;
            }
            return value;
        }

        private async Task RunProgressFunction(IProgress<ProgressInfo> progress)
        {
            var token = CancellationToken.None;
            if (SupportsCancellation)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                token = _cancellationTokenSource.Token;
            }
            _taskInProgress = true;
            try
            {
                await Task.Run(() => _progressFunction(progress, token), token).ConfigureAwait(false);
            }
            catch (OperationCanceledException)
            {
                IsCanceled = true;
            }
            catch (Exception ex)
            {
                Exception = ex;
            }
            finally
            {
                _taskInProgress = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (_taskInProgress)
            {
                if (SupportsCancellation)
                {
                    Debug.Assert(_cancellationTokenSource != null);
                    _cancellationTokenSource.Cancel();
                }
                e.Cancel = true;
                return;
            }
            base.OnFormClosing(e);
        }

        private readonly Size _baseSize;

        private void SetCancellationControls()
        {
            ProcessCancelButton.Visible = SupportsCancellation;
            Size = SupportsCancellation ? _baseSize : new Size(_baseSize.Width, _baseSize.Height - 30);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _cancellationTokenSource?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
