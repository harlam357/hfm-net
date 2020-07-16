
using System;
using System.Windows.Forms;

namespace HFM.Forms
{
    public abstract class FolderDialogPresenter
    {
        // ReSharper disable once EmptyConstructor
        protected FolderDialogPresenter()
        {

        }

        public virtual string SelectedPath { get; set; }

        public abstract DialogResult ShowDialog();

        public abstract DialogResult ShowDialog(IWin32Window owner);
    }

    public class DefaultFolderDialogPresenter : FolderDialogPresenter, IDisposable
    {
        private readonly FolderBrowserDialog _dialog;

        protected DefaultFolderDialogPresenter(FolderBrowserDialog dialog)
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

    public class NullFolderDialogPresenter : FolderDialogPresenter
    {
        public static NullFolderDialogPresenter Instance { get; } = new NullFolderDialogPresenter();

        protected NullFolderDialogPresenter()
        {

        }

        public override DialogResult ShowDialog()
        {
            return default;
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            return default;
        }
    }
}
