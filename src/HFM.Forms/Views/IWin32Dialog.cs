
using System;
using System.Windows.Forms;

namespace HFM.Forms
{
    public interface IWin32Dialog : IWin32Window, IDisposable
    {
        DialogResult DialogResult { get; set; }

        DialogResult ShowDialog(IWin32Window owner);

        void Close();
    }
}
