using System;
using System.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    public abstract class DialogPresenter : IDialogPresenter
    {
        public virtual IWin32Dialog Dialog { get; protected set; }

        public virtual DialogResult ShowDialog(IWin32Window owner)
        {
            Dialog = OnCreateDialog();
            Dialog.Closed += OnClosed;
            return Dialog.ShowDialog(owner);
        }

        protected abstract IWin32Dialog OnCreateDialog();

        protected virtual void OnClosed(object sender, EventArgs e)
        {

        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    Dialog?.Dispose();
                }
            }
            _disposed = true;
        }
    }

    public abstract class DialogPresenter<TViewModel> : DialogPresenter where TViewModel : ViewModelBase
    {
        protected ViewModelBase ModelBase { get; }

        protected DialogPresenter(TViewModel model)
        {
            ModelBase = model;
        }

        public override DialogResult ShowDialog(IWin32Window owner)
        {
            ModelBase.Load();
            return base.ShowDialog(owner);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            if (Dialog.DialogResult == DialogResult.OK)
            {
                ModelBase.Save();
            }
            base.OnClosed(sender, e);
        }
    }
}
