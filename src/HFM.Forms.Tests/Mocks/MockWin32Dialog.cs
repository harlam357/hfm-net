using System;
using System.Windows.Forms;

using HFM.Forms.Views;

namespace HFM.Forms.Mocks
{
    public class MockWin32Dialog : IWin32Dialog
    {
        protected Func<IWin32Window, DialogResult> DialogResultProvider { get; }

        public MockWin32Dialog()
        {

        }

        public MockWin32Dialog(Func<IWin32Window, DialogResult> dialogResultProvider)
        {
            DialogResultProvider = dialogResultProvider;
        }

        public bool Shown { get; set; }

        public DialogResult DialogResult { get; set; }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            Shown = true;
            DialogResult = OnProvideDialogResult(owner);
            return DialogResult;
        }

        protected virtual DialogResult OnProvideDialogResult(IWin32Window owner)
        {
            return DialogResultProvider?.Invoke(owner) ?? default;
        }

        public void Close()
        {
            Shown = false;
            OnClosed(this, EventArgs.Empty);
        }

        public event EventHandler Closed;

        protected virtual void OnClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        public IntPtr Handle { get; }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }
    }

    public class MockWin32Dialog<TPresenter> : MockWin32Dialog
    {
        protected TPresenter Presenter { get; }
        protected Action<TPresenter> PresenterAction { get; }

        public MockWin32Dialog(TPresenter presenter, Action<TPresenter> presenterAction)
        {
            Presenter = presenter;
            PresenterAction = presenterAction;
        }

        public MockWin32Dialog(TPresenter presenter, Action<TPresenter> presenterAction, Func<IWin32Window, DialogResult> dialogResultProvider) : base(dialogResultProvider)
        {
            Presenter = presenter;
            PresenterAction = presenterAction;
        }

        protected override DialogResult OnProvideDialogResult(IWin32Window owner)
        {
            PresenterAction?.Invoke(Presenter);
            return DialogResultProvider?.Invoke(owner) ?? default;
        }
    }
}
