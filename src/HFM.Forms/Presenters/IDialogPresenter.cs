using System;
using System.Windows.Forms;

using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    /// <summary>
    /// Defines a presenter that shows a modal dialog.
    /// </summary>
    public interface IDialogPresenter : IDisposable
    {
        IWin32Dialog Dialog { get; }

        DialogResult ShowDialog(IWin32Window owner);
    }
}
