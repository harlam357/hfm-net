
using System;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
    public class PreferencesPresenter : IDisposable
    {
        public ILogger Logger { get; }
        public PreferencesModel Model { get; }
        public MessageBoxPresenter MessageBox { get; }
        public ExceptionPresenterFactory ExceptionPresenter { get; }

        public PreferencesPresenter(PreferencesModel model, ILogger logger, MessageBoxPresenter messageBox, ExceptionPresenterFactory exceptionPresenter)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ExceptionPresenter = exceptionPresenter ?? NullExceptionPresenterFactory.Instance;
        }

        public IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            Dialog = new PreferencesDialog(this);
            return Dialog.ShowDialog(owner);
        }

        public void OKClicked()
        {
            if (Model.ValidateAcceptance())
            {
                try
                {
                    Model.Save();
                    Dialog.DialogResult = DialogResult.OK;
                }
                catch (Exception ex)
                {
                    Dialog.DialogResult = ExceptionPresenter.ShowDialog(Dialog, ex, false);
                }
                Dialog.Close();
            }
        }

        public void Dispose()
        {
            Dialog?.Dispose();
        }

        public void BrowseWebFolderClicked(FolderDialogPresenter dialog)
        {
            if (!String.IsNullOrWhiteSpace(Model.WebGenerationModel.WebRoot))
            {
                dialog.SelectedPath = Model.WebGenerationModel.WebRoot;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Model.WebGenerationModel.WebRoot = dialog.SelectedPath;
            }
        }

        public void TestEmailClicked(SendMailService sendMailService)
        {
            if (Model.ReportingModel.HasError)
            {
                MessageBox.ShowError(Dialog, "Please correct error conditions before sending a test email.", Core.Application.NameAndVersion);
            }
            else
            {
                try
                {
                    var m = Model.ReportingModel;
                    sendMailService.SendEmail(m.FromAddress, m.ToAddress, "HFM.NET - Test Email",
                        "HFM.NET - Test Email", m.ServerAddress, m.ServerPort, m.ServerUsername, m.ServerPassword, m.ServerSecure);
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

        public void TestExtremeOverclockingUserClicked(LocalProcessService localProcess)
        {
            string url = String.Concat(EocStatsService.UserBaseUrl, Model.OptionsModel.EocUserID);
            string caption = "EOC User Stats page";
            TestUrl(localProcess, url, caption);
        }

        public void TestFoldingAtHomeUserClicked(LocalProcessService localProcess)
        {
            string url = String.Concat(FahUrl.UserBaseUrl, Model.OptionsModel.FahUserID);
            string caption = "FAH User Stats page";
            TestUrl(localProcess, url, caption);
        }

        public void TestExtremeOverclockingTeamClicked(LocalProcessService localProcess)
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
                string path = Model.WebGenerationModel.WebRoot;
                if (Model.WebGenerationModel.FtpModeEnabled)
                {
                    string host = Model.WebGenerationModel.WebGenServer;
                    int port = Model.WebGenerationModel.WebGenPort;
                    string username = Model.WebGenerationModel.WebGenUsername;
                    string password = Model.WebGenerationModel.WebGenPassword;
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

        public void BrowseForConfigurationFileClicked(FileDialogPresenter dialog)
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

        public void BrowseForLogViewerClicked(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileName(Model.OptionsModel.LogFileViewer);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, ExeExtension, ExeFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.OptionsModel.LogFileViewer = result;
            }
        }

        public void BrowseForFileExplorerClicked(FileDialogPresenter dialog)
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
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.WebOverview);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.WebOverview = GetPathOrFileNameIfInDefaultXsltPath(result);
            }
        }

        public void BrowseForSummaryTransform(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.WebSummary);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.WebSummary = GetPathOrFileNameIfInDefaultXsltPath(result);
            }
        }

        public void BrowseForSlotTransform(FileDialogPresenter dialog)
        {
            var x = GetInitialDirectoryAndFileNameForTransform(Model.WebVisualStylesModel.WebSlot);
            string result = ShowFileDialog(dialog, x.InitialDirectory, x.FileName, XsltExtension, XsltFilter);
            if (!String.IsNullOrEmpty(result))
            {
                Model.WebVisualStylesModel.WebSlot = GetPathOrFileNameIfInDefaultXsltPath(result);
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
