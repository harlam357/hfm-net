using System;
using System.Drawing;
using System.Windows.Forms;

using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public class ColorDialogPresenter : DialogPresenter
    {
        public virtual Color Color
        {
            get => ColorDialog?.Color ?? Color.Empty;
            set
            {
                if (ColorDialog is null) return;
                ColorDialog.Color = value;
            }
        }

        public Win32ColorDialog ColorDialog => Dialog as Win32ColorDialog;

        protected override IWin32Dialog OnCreateDialog() => new Win32ColorDialog();
    }

    public sealed class Win32ColorDialog : IWin32Dialog
    {
        private readonly ColorDialog _dialog;

        public Win32ColorDialog()
        {
            _dialog = new ColorDialog();
        }

        public Color Color
        {
            get => _dialog.Color;
            set => _dialog.Color = value;
        }

        public IntPtr Handle => IntPtr.Zero;

        public void Dispose() => _dialog.Dispose();

        public DialogResult DialogResult
        {
            get => DialogResult.None;
            set => throw new NotImplementedException();
        }

        public DialogResult ShowDialog(IWin32Window owner) => _dialog.ShowDialog(owner);

        public void Close() => throw new NotImplementedException();

        public event EventHandler Closed;
    }
}
