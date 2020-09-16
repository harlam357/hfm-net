using System;
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms.Presenters.Mocks
{
    public class MockColorDialogPresenter : ColorDialogPresenter
    {
        private readonly Func<IWin32Window, DialogResult> _dialogResultProvider;

        public MockColorDialogPresenter(Func<IWin32Window, DialogResult> dialogResultProvider)
        {
            _dialogResultProvider = dialogResultProvider;
        }

        public override Color Color { get; set; }

        public override DialogResult ShowDialog(IWin32Window owner) => OnProvideDialogResult(owner);

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner) => _dialogResultProvider?.Invoke(owner) ?? default;
    }
}
