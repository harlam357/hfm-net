using System;
using System.Windows.Forms;

namespace HFM.Forms.Views
{
    public interface IWin32Form : IWin32Window, IDisposable
    {
        void Show();

        void Close();

        FormWindowState WindowState { get; set; }

        void BringToFront();

        event EventHandler Closed;
    }
}
