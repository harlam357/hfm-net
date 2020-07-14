
using System;
using System.Windows.Forms;

using HFM.Core.Logging;
using HFM.Forms.Models;

namespace HFM.Forms
{
    public class PreferencesPresenter : IDisposable
    {
        public ILogger Logger { get; }
        public PreferencesModel Model { get; }
        public MessageBoxPresenter MessageBox { get; }
        public ExceptionPresenter ExceptionPresenter { get; }

        public PreferencesPresenter(PreferencesModel model, ILogger logger, MessageBoxPresenter messageBox, ExceptionPresenter exceptionPresenter)
        {
            Model = model ?? throw new ArgumentNullException(nameof(model));
            Logger = logger ?? NullLogger.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ExceptionPresenter = exceptionPresenter ?? NullExceptionPresenter.Instance;
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
    }
}
