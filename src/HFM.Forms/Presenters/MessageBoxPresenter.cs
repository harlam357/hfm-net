
using System.Windows.Forms;

namespace HFM.Forms.Presenters
{
    public abstract class MessageBoxPresenter
    {
        public static MessageBoxPresenter Default { get; } = DefaultMessageBoxPresenter.Instance;

        // ReSharper disable once EmptyConstructor
        protected MessageBoxPresenter()
        {

        }

        public abstract void ShowError(string text, string caption);

        public abstract void ShowError(IWin32Window owner, string text, string caption);

        public abstract void ShowInformation(string text, string caption);

        public abstract void ShowInformation(IWin32Window owner, string text, string caption);

        public abstract DialogResult AskYesNoQuestion(string text, string caption);

        public abstract DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption);

        public abstract DialogResult AskYesNoCancelQuestion(string text, string caption);

        public abstract DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption);
    }

    public class DefaultMessageBoxPresenter : MessageBoxPresenter
    {
        public static DefaultMessageBoxPresenter Instance { get; } = new DefaultMessageBoxPresenter();

        protected DefaultMessageBoxPresenter()
        {

        }

        public override void ShowError(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override void ShowError(IWin32Window owner, string text, string caption)
        {
            MessageBox.Show(GetOwnerOrNull(owner), text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override void ShowInformation(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override void ShowInformation(IWin32Window owner, string text, string caption)
        {
            MessageBox.Show(GetOwnerOrNull(owner), text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override DialogResult AskYesNoQuestion(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption)
        {
            return MessageBox.Show(GetOwnerOrNull(owner), text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoCancelQuestion(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption)
        {
            return MessageBox.Show(GetOwnerOrNull(owner), text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        private static IWin32Window GetOwnerOrNull(IWin32Window owner)
        {
            return owner is Control control && control.IsDisposed ? null : owner;
        }
    }

    public class NullMessageBoxPresenter : MessageBoxPresenter
    {
        public static NullMessageBoxPresenter Instance { get; } = new NullMessageBoxPresenter();

        protected NullMessageBoxPresenter()
        {

        }

        public override void ShowError(string text, string caption)
        {
        }

        public override void ShowError(IWin32Window owner, string text, string caption)
        {
        }

        public override void ShowInformation(string text, string caption)
        {
        }

        public override void ShowInformation(IWin32Window owner, string text, string caption)
        {
        }

        public override DialogResult AskYesNoQuestion(string text, string caption)
        {
            return default;
        }

        public override DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption)
        {
            return default;
        }

        public override DialogResult AskYesNoCancelQuestion(string text, string caption)
        {
            return default;
        }

        public override DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption)
        {
            return default;
        }
    }
}
