
using System;
using System.Windows.Forms;

using HFM.Core.Logging;
using HFM.Core.Services;
using HFM.Forms.Models;

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
            if (!String.IsNullOrWhiteSpace(Model.ScheduledTasksModel.WebRoot))
            {
                dialog.SelectedPath = Model.ScheduledTasksModel.WebRoot;
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                Model.ScheduledTasksModel.WebRoot = dialog.SelectedPath;
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
    }
}
