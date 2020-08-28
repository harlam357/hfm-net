using System;
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms.Presenters
{
    public abstract class ColorDialogPresenter
    {
        public virtual Color Color { get; set; }

        public abstract DialogResult ShowDialog();

        public abstract DialogResult ShowDialog(IWin32Window owner);
    }

    public class DefaultColorDialogPresenter : ColorDialogPresenter, IDisposable
    {
        private readonly ColorDialog _dialog;

        public DefaultColorDialogPresenter(ColorDialog dialog)
        {
            _dialog = dialog;
        }

        public override Color Color
        {
            get => _dialog.Color;
            set => _dialog.Color = value;
        }

        public override DialogResult ShowDialog() => _dialog.ShowDialog();

        public override DialogResult ShowDialog(IWin32Window owner) => _dialog.ShowDialog(owner);

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
    }

    public class NullColorDialogPresenter : ColorDialogPresenter
    {
        public override DialogResult ShowDialog() => default;

        public override DialogResult ShowDialog(IWin32Window owner) => default;
    }
}
