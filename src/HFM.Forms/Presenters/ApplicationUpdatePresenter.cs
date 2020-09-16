using System;
using System.Globalization;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Logging;
using HFM.Core.Net;
using HFM.Forms.Models;
using HFM.Forms.Views;
using HFM.Preferences;

namespace HFM.Forms.Presenters
{
    public class ApplicationUpdatePresenter : DialogPresenter<ApplicationUpdateModel>
    {
        public ApplicationUpdateModel Model { get; }
        public ILogger Logger { get; }
        public IPreferences Preferences { get; }
        public MessageBoxPresenter MessageBox { get; }

        public ApplicationUpdatePresenter(ApplicationUpdateModel model, ILogger logger, IPreferences preferences, MessageBoxPresenter messageBox)
            : base(model)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        protected override IWin32Dialog OnCreateDialog() => new ApplicationUpdateDialog(this);

        public async Task DownloadClick(FileDialogPresenter saveFile)
        {
            if (!ShowSaveFileView(saveFile)) return;

            Model.DownloadInProgress = true;
            try
            {
                bool downloadResult = await Task.Run(PerformDownload).ConfigureAwait(true);
                if (downloadResult)
                {
                    Model.SelectedUpdateFileIsReadyToBeExecuted = Model.SelectedUpdateFile.UpdateType == (int)ApplicationUpdateFileType.Executable;

                    Dialog.DialogResult = DialogResult.OK;
                    Dialog.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                string message = String.Format(CultureInfo.CurrentCulture,
                    "Download failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message);
                MessageBox.ShowError(message, Core.Application.NameAndVersion);
            }
            finally
            {
                Model.DownloadInProgress = false;
            }
        }

        private bool ShowSaveFileView(FileDialogPresenter saveFile)
        {
            if (Model.SelectedUpdateFile is null) return false;

            saveFile.FileName = Model.SelectedUpdateFile.Name;
            if (saveFile.ShowDialog(Dialog) == DialogResult.OK)
            {
                Model.SelectedUpdateFileLocalFilePath = saveFile.FileName;
                return true;
            }

            return false;
        }

        private WebOperation _webOperation;

        private bool PerformDownload()
        {
            var selectedUpdateFile = Model.SelectedUpdateFile;
            var uri = new Uri(selectedUpdateFile.HttpAddress);
            var path = Model.SelectedUpdateFileLocalFilePath;

            _webOperation = WebOperation.Create(uri);
            _webOperation.WebRequest.Proxy = WebProxyFactory.Create(Preferences);

            // execute the download
            _webOperation.Download(path);

            if (_webOperation.Result != WebOperationResult.Completed)
            {
                return false;
            }

            // verify, throws exception on error
            selectedUpdateFile.Verify(path);
            return true;
        }

        public void CancelClick()
        {
            if (_webOperation != null && _webOperation.State == WebOperationState.InProgress)
            {
                _webOperation.Cancel();
            }
            else
            {
                Dialog.DialogResult = DialogResult.Cancel;
                Dialog.Close();
            }
        }
    }

    public class ApplicationUpdatePresenterFactory
    {
        public ILogger Logger { get; }
        public IPreferences Preferences { get; }
        public MessageBoxPresenter MessageBox { get; }

        public ApplicationUpdatePresenterFactory(ILogger logger, IPreferences preferences, MessageBoxPresenter messageBox)
        {
            Logger = logger ?? NullLogger.Instance;
            Preferences = preferences ?? new InMemoryPreferencesProvider();
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        public virtual ApplicationUpdatePresenter Create(ApplicationUpdateModel model) => new ApplicationUpdatePresenter(model, Logger, Preferences, MessageBox);
    }
}
