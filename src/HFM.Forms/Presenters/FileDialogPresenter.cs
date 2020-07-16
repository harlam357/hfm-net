
using System;
using System.Windows.Forms;

namespace HFM.Forms
{
    public abstract class FileDialogPresenter
    {
        // ReSharper disable once EmptyConstructor
        protected FileDialogPresenter()
        {

        }

        public virtual string DefaultExt { get; set; }

        public virtual string FileName { get; set; }

        public virtual string InitialDirectory { get; set; }

        public virtual string Filter { get; set; }

        public virtual int FilterIndex { get; set; }

        public virtual bool RestoreDirectory { get; set; }

        public abstract DialogResult ShowDialog();

        public abstract DialogResult ShowDialog(IWin32Window owner);
    }

    public class DefaultFileDialogPresenter : FileDialogPresenter, IDisposable
    {
        private readonly FileDialog _dialog;

        protected DefaultFileDialogPresenter(FileDialog dialog)
        {
            _dialog = dialog;
        }

        public override string DefaultExt
        {
            get => _dialog.DefaultExt;
            set => _dialog.DefaultExt = value;
        }

        public override string FileName
        {
            get => _dialog.FileName;
            set => _dialog.FileName = value;
        }

        public override string InitialDirectory
        {
            get => _dialog.InitialDirectory;
            set => _dialog.InitialDirectory = value;
        }

        public override string Filter
        {
            get => _dialog.Filter;
            set => _dialog.Filter = value;
        }

        public override int FilterIndex
        {
            get => _dialog.FilterIndex;
            set => _dialog.FilterIndex = value;
        }

        public override bool RestoreDirectory
        {
            get => _dialog.RestoreDirectory;
            set => _dialog.RestoreDirectory = value;
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

        public static DefaultFileDialogPresenter OpenFile()
        {
            return new DefaultFileDialogPresenter(new OpenFileDialog());
        }

        public static DefaultFileDialogPresenter SaveFile()
        {
            return new DefaultFileDialogPresenter(new SaveFileDialog());
        }
    }

    public class NullFileDialogPresenter : FileDialogPresenter
    {
        public static NullFileDialogPresenter Instance { get; } = new NullFileDialogPresenter();

        protected NullFileDialogPresenter()
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
