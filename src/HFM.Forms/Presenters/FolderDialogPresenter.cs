
using System;
using System.Windows.Forms;

namespace HFM.Forms
{
    public abstract class FolderDialogPresenter
    {
        public virtual string SelectedPath { get; set; }

        public virtual DialogResult ShowDialog()
        {
            return default;
        }

        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            return default;
        }
    }

    public class DefaultFolderDialogPresenter : FolderDialogPresenter, IDisposable
    {
        private readonly FolderBrowserDialog _dialog;

        public DefaultFolderDialogPresenter(FolderBrowserDialog dialog)
        {
            _dialog = dialog;
        }

        public override string SelectedPath
        {
            get => _dialog.SelectedPath;
            set => _dialog.SelectedPath = value;
        }

        public override DialogResult ShowDialog()
        {
            return _dialog.ShowDialog();
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            return _dialog.ShowDialog(owner);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _dialog?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public static DefaultFolderDialogPresenter BrowseFolder()
        {
            return new DefaultFolderDialogPresenter(new FolderBrowserDialog());
        }
    }
}
