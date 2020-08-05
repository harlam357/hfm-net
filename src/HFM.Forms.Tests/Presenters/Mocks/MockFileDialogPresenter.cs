
using System;
using System.Windows.Forms;

namespace HFM.Forms.Presenters.Mocks
{
    public class MockFileDialogPresenter : FileDialogPresenter
    {
        private readonly Func<IWin32Window, DialogResult> _dialogResultProvider;

        public MockFileDialogPresenter(Func<IWin32Window, DialogResult> dialogResultProvider)
        {
            _dialogResultProvider = dialogResultProvider;
        }

        public override DialogResult ShowDialog()
        {
            return OnProvideDialogResult(null);
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            return OnProvideDialogResult(owner);
        }

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner)
        {
            return _dialogResultProvider?.Invoke(owner) ?? default;
        }
    }
}
