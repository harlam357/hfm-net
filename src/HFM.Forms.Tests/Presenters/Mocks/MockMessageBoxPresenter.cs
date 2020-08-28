
using System;
using System.Collections.Generic;
using System.Windows.Forms;

using HFM.Forms.Mocks;

namespace HFM.Forms.Presenters.Mocks
{
    public class MockMessageBoxPresenter : MessageBoxPresenter
    {
        private readonly Func<IWin32Window, string, string, DialogResult> _dialogResultProvider;

        public MockMessageBoxPresenter()
        {

        }

        public MockMessageBoxPresenter(Func<IWin32Window, string, string, DialogResult> dialogResultProvider)
        {
            _dialogResultProvider = dialogResultProvider;
        }

        public ICollection<MethodInvocation> Invocations { get; } = new List<MethodInvocation>();

        public override void ShowError(string text, string caption)
        {
            Invocations.Add(new MethodInvocation(nameof(ShowError), null, text, caption));
        }

        public override void ShowError(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MethodInvocation(nameof(ShowError), owner, text, caption));
        }

        public override void ShowInformation(string text, string caption)
        {
            Invocations.Add(new MethodInvocation(nameof(ShowInformation), null, text, caption));
        }

        public override void ShowInformation(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MethodInvocation(nameof(ShowInformation), owner, text, caption));
        }

        public override DialogResult AskYesNoQuestion(string text, string caption)
        {
            var result = OnProvideDialogResult(null, text, caption);
            Invocations.Add(new MethodInvocationWithReturnValue(result, nameof(AskYesNoQuestion), null, text, caption));
            return result;
        }

        public override DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption)
        {
            var result = OnProvideDialogResult(owner, text, caption);
            Invocations.Add(new MethodInvocationWithReturnValue(result, nameof(AskYesNoQuestion), owner, text, caption));
            return result;
        }

        public override DialogResult AskYesNoCancelQuestion(string text, string caption)
        {
            var result = OnProvideDialogResult(null, text, caption);
            Invocations.Add(new MethodInvocationWithReturnValue(result, nameof(AskYesNoCancelQuestion), null, text, caption));
            return result;
        }

        public override DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption)
        {
            var result = OnProvideDialogResult(owner, text, caption);
            Invocations.Add(new MethodInvocationWithReturnValue(result, nameof(AskYesNoCancelQuestion), owner, text, caption));
            return result;
        }

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner, string text, string caption)
        {
            return _dialogResultProvider?.Invoke(owner, text, caption) ?? default;
        }
    }
}
