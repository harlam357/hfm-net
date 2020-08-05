
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;

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

        public ICollection<MockMessageBoxInvocation> Invocations { get; } = new List<MockMessageBoxInvocation>();

        public override void ShowError(string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(ShowError), null, text, caption));
        }

        public override void ShowError(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(ShowError), owner, text, caption));
        }
      
        public override void ShowInformation(string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(ShowInformation), null, text, caption));
        }

        public override void ShowInformation(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(ShowInformation), owner, text, caption));
        }

        public override DialogResult AskYesNoQuestion(string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(AskYesNoQuestion), null, text, caption));
            return OnProvideDialogResult(null, text, caption);
        }

        public override DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(AskYesNoQuestion), owner, text, caption));
            return OnProvideDialogResult(owner, text, caption);
        }

        public override DialogResult AskYesNoCancelQuestion(string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(AskYesNoCancelQuestion), null, text, caption));
            return OnProvideDialogResult(null, text, caption);
        }

        public override DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption)
        {
            Invocations.Add(new MockMessageBoxInvocation(nameof(AskYesNoCancelQuestion), owner, text, caption));
            return OnProvideDialogResult(owner, text, caption);
        }

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner, string text, string caption)
        {
            return _dialogResultProvider?.Invoke(owner, text, caption) ?? default;
        }
    }

    [DebuggerDisplay("{Name}, {Text}, {Caption}")]
    public class MockMessageBoxInvocation
    {
        public MockMessageBoxInvocation(string name, IWin32Window owner, string text, string caption)
        {
            Name = name;
            Owner = owner;
            Text = text;
            Caption = caption;
        }

        public string Name { get; }
        public IWin32Window Owner { get; }
        public string Text { get; }
        public string Caption { get; }

    }
}
