
using System.Windows.Forms;

namespace HFM.Forms
{
    public abstract class MessageBoxPresenter
    {
        public static MessageBoxPresenter Default { get; } = new DefaultMessageBoxPresenter();

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
        public override void ShowError(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public override void ShowError(IWin32Window owner, string text, string caption)
        {
            MessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
      
        public override void ShowInformation(string text, string caption)
        {
            MessageBox.Show(text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override void ShowInformation(IWin32Window owner, string text, string caption)
        {
            MessageBox.Show(owner, text, caption, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public override DialogResult AskYesNoQuestion(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoQuestion(IWin32Window owner, string text, string caption)
        {
            return MessageBox.Show(owner, text, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoCancelQuestion(string text, string caption)
        {
            return MessageBox.Show(text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }

        public override DialogResult AskYesNoCancelQuestion(IWin32Window owner, string text, string caption)
        {
            return MessageBox.Show(owner, text, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        }
    }

    public class NullMessageBoxPresenter : MessageBoxPresenter
    {
        public static NullMessageBoxPresenter Instance { get; } = new NullMessageBoxPresenter();

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
