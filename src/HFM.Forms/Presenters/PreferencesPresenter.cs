using System.Globalization;

using HFM.Core;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Forms.Models;
using HFM.Forms.Services;
using HFM.Forms.Views;
using HFM.Preferences;

namespace HFM.Forms.Presenters
{
    public class PreferencesPresenter : DialogPresenter<PreferencesModel>
    {
        public ILogger Logger { get; }
        public PreferencesModel Model { get; }
        public MessageBoxPresenter MessageBox { get; }
        public ExceptionPresenterFactory ExceptionPresenter { get; }

        public PreferencesPresenter(PreferencesModel model, ILogger logger, MessageBoxPresenter messageBox, ExceptionPresenterFactory exceptionPresenter)
            : base(model)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ExceptionPresenter = exceptionPresenter ?? NullExceptionPresenterFactory.Instance;
        }

        protected override IWin32Dialog OnCreateDialog() => new PreferencesDialog(this);

        public void OKClicked()
        {
            if (Model.ValidateAcceptance())
            {
                Dialog.DialogResult = DialogResult.OK;
                try
                {
                    Dialog.Close();
                }
                catch (Exception ex)
                {
                    Dialog.DialogResult = ExceptionPresenter.ShowDialog(Dialog, ex, false);
                }
            }
        }

        public void CancelClicked()
        {
            Dialog.DialogResult = DialogResult.Cancel;
            Dialog.Close();
        }

        public void BrowseForWebGenerationPath(FolderDialogPresenter dialog)
        {
            if (!String.IsNullOrWhiteSpace(Model.WebGenerationModel.Path))
            {
                dialog.SelectedPath = Model.WebGenerationModel.Path;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Model.WebGenerationModel.Path = dialog.SelectedPath;
            }
        }

        public void TestReportingEmail(SendMailService sendMailService)
        {
            if (!Model.ReportingModel.ValidateAcceptance())
            {
                MessageBox.ShowError(Dialog, "Please correct error conditions before sending a test email.", Core.Application.NameAndVersion);
            }
            else
            {
                try
                {
                    var m = Model.ReportingModel;
                    sendMailService.SendEmail(m.FromAddress, m.ToAddress, $"{Core.Application.Name} - Test Email",
                        $"{Core.Application.Name} - Test Email", m.Server, m.Port, m.Username, m.Password, m.IsSecure);
                    MessageBox.ShowInformation(Dialog, "Test email sent successfully.", Core.Application.NameAndVersion);
                }
                catch (Exception ex)
                {
                    Logger.Warn(ex.Message, ex);
                    var text = String.Format("Test email failed to send.  Please check your email settings.{0}{0}Error: {1}", Environment.NewLine, ex.Message);
                    MessageBox.ShowError(Dialog, text, Core.Application.NameAndVersion);
                }
            }
        }

        public void TestExtremeOverclockingUser(LocalProcessService localProcess)
        {
            string url = String.Concat(EocStatsService.UserBaseUrl, Model.OptionsModel.EocUserID);
            string caption = "EOC User Stats page";
            TestUrl(localProcess, url, caption);
        }

        public async Task TestFoldingAtHomeUser(LocalProcessService localProcess, FahUserService userService)
        {
            var fahUser = await userService.FindUserAndLogError(Model.OptionsModel.FahUserID, Logger).ConfigureAwait(true);

            string fileName = new Uri(String.Concat(FahUrl.UserBaseUrl, fahUser.ID)).AbsoluteUri;
            const string errorMessage = "An error occurred while attempting to open the FAH user stats page.";
            localProcess.StartAndNotifyError(fileName, errorMessage, Logger, MessageBox);
        }

        public void TestExtremeOverclockingTeam(LocalProcessService localProcess)
        {
            string url = String.Concat(EocStatsService.TeamBaseUrl, Model.OptionsModel.TeamID);
            string caption = "EOC Team Stats page";
            TestUrl(localProcess, url, caption);
        }

        private void TestUrl(LocalProcessService localProcess, string url, string caption)
        {
            try
            {
                localProcess.Start(url);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                string text = String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, caption);
                MessageBox.ShowError(text, Core.Application.NameAndVersion);
            }
        }

        public async Task TestWebGenerationConnection(FtpService ftpService)
        {
            try
            {
                string path = Model.WebGenerationModel.Path;
                if (Model.WebGenerationModel.FtpModeEnabled)
                {
                    string host = Model.WebGenerationModel.Server;
                    int port = Model.WebGenerationModel.Port;
                    string username = Model.WebGenerationModel.Username;
                    string password = Model.WebGenerationModel.Password;
                    var ftpMode = Model.WebGenerationModel.FtpMode;

                    await Task.Run(() => ftpService.CheckConnection(host, port, path, username, password, ftpMode)).ConfigureAwait(false);
                }
                else
                {
                    if (!Directory.Exists(path))
                    {
                        throw new DirectoryNotFoundException($"{path} does not exist.");
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                throw;
            }
        }

        public void ShowTestWebGenerationConnectionSucceededMessage()
        {
            MessageBox.ShowInformation(Dialog, "Test Connection Succeeded", Core.Application.NameAndVersion);
        }

        public void ShowTestWebGenerationConnectionFailedMessage(Exception exception)
        {
            string text = String.Format(CultureInfo.CurrentCulture, "Test Connection Failed{0}{0}{1}", Environment.NewLine, exception.Message);
            MessageBox.ShowError(Dialog, text, Core.Application.NameAndVersion);
        }

        public void BrowseForConfigurationFile(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileName(Model.ClientsModel.DefaultConfigFile);
            string extension = Core.Client.ClientSettingsFileSerializer.DefaultFileExtension;
            string filter = Core.Client.ClientSettingsFileSerializer.DefaultFileTypeFilter;
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, extension, filter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.ClientsModel.DefaultConfigFile = result;
            }
        }

        private const string ExeExtension = "exe";
        private const string ExeFilter = "Program Files|*.exe";

        public void BrowseForLogFileViewer(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileName(Model.OptionsModel.LogFileViewer);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, ExeExtension, ExeFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.OptionsModel.LogFileViewer = result;
            }
        }

        public void BrowseForFileExplorer(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileName(Model.OptionsModel.FileExplorer);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, ExeExtension, ExeFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.OptionsModel.FileExplorer = result;
            }
        }

        private static (string InitialDirectory, string FileName) GetInitialDirectoryAndFileName(string path)
        {
            string initialDirectory = null;
            string fileName = null;

            if (!String.IsNullOrEmpty(path))
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    initialDirectory = fileInfo.DirectoryName;
                    fileName = fileInfo.Name;
                }
                else
                {
                    var dirInfo = new DirectoryInfo(path);
                    if (dirInfo.Exists)
                    {
                        initialDirectory = dirInfo.FullName;
                    }
                }
            }

            return (initialDirectory, fileName);
        }

        private const string XsltExtension = "xslt";
        private const string XsltFilter = "XML Transform (*.xslt;*.xsl)|*.xslt;*.xsl";

        public void BrowseForOverviewTransform(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.OverviewXsltPath);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.OverviewXsltPath = GetPathOrFileNameIfInDefaultXsltPath(result);
            }
        }

        public void BrowseForSummaryTransform(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.SummaryXsltPath);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.SummaryXsltPath = GetPathOrFileNameIfInDefaultXsltPath(result);
            }
        }

        public void BrowseForSlotTransform(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.SlotXsltPath);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.SlotXsltPath = GetPathOrFileNameIfInDefaultXsltPath(result);
            }
        }

        private string DefaultXsltPath =>
            Path.Combine(Model.Preferences.Get<string>(Preference.ApplicationPath), Core.Application.XsltFolderName);

        private (string InitialDirectory, string FileName) GetInitialDirectoryAndFileNameForTransform(string path)
        {
            string initialDirectory = null;
            string fileName = null;

            if (!String.IsNullOrEmpty(path))
            {
                string defaultXsltPath = DefaultXsltPath;

                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    initialDirectory = fileInfo.DirectoryName;
                    fileName = fileInfo.Name;
                }
                else if (File.Exists(Path.Combine(defaultXsltPath, path)))
                {
                    initialDirectory = defaultXsltPath;
                    fileName = path;
                }
                else
                {
                    var dirInfo = new DirectoryInfo(path);
                    if (dirInfo.Exists)
                    {
                        initialDirectory = dirInfo.FullName;
                    }
                }
            }

            return (initialDirectory, fileName);
        }

        private string GetPathOrFileNameIfInDefaultXsltPath(string path)
        {
            // Check to see if the directory the file is in is \HFM\XSL.  If so, return only the file name.
            return DefaultXsltPath == Path.GetDirectoryName(path) ? Path.GetFileName(path) : path;
        }

        private static string ShowFileDialog(FileDialogPresenter dialog, string initialDirectory, string fileName, string extension, string filter)
        {
            if (!String.IsNullOrEmpty(initialDirectory))
            {
                dialog.InitialDirectory = initialDirectory;
            }
            if (!String.IsNullOrEmpty(fileName))
            {
                dialog.FileName = fileName;
            }
            dialog.DefaultExt = extension;
            dialog.Filter = filter;
            return dialog.ShowDialog() == DialogResult.OK ? dialog.FileName : null;
        }
    }
}
