using System;
using System.Collections.Generic;
using System.Windows.Forms;

using HFM.Forms.Mocks;

namespace HFM.Forms.Presenters.Mocks
{
    public class MockFileDialogPresenter : FileDialogPresenter
    {
        public ICollection<MethodInvocation> Invocations { get; } = new List<MethodInvocation>();

        private readonly Func<IWin32Window, DialogResult> _dialogResultProvider;

        public MockFileDialogPresenter(Func<IWin32Window, DialogResult> dialogResultProvider)
        {
            _dialogResultProvider = dialogResultProvider;
        }

        public override DialogResult ShowDialog()
        {
            Invocations.Add(new MethodInvocation(nameof(ShowDialog)));
            return OnProvideDialogResult(null);
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            Invocations.Add(new MethodInvocation(nameof(ShowDialog), owner));
            return OnProvideDialogResult(owner);
        }

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner)
        {
            return _dialogResultProvider?.Invoke(owner) ?? default;
        }
    }
}
