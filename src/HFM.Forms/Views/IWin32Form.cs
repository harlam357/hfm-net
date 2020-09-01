using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace HFM.Forms.Views
{
    public interface IWin32Form : IWin32Window, ISynchronizeInvoke, IDisposable
    {
        void Show();

        void Close();

        FormWindowState WindowState { get; set; }

        void BringToFront();

        Point Location { get; set; }

        Size Size { get; set; }

        Size MinimumSize { get; }

        event EventHandler Closed;
    }
}
