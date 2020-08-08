using System;

using HFM.Forms.Views;

namespace HFM.Forms.Presenters
{
    /// <summary>
    /// Defines a presenter that shows a non-modal window.
    /// </summary>
    public interface IFormPresenter : IDisposable
    {
        IWin32Form Form { get; }

        void Show();

        void Close();

        event EventHandler Closed;
    }
}
