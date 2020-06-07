
using System;
using System.Windows.Forms;

namespace HFM.Forms.Mocks
{
    public class MockWin32Dialog : IWin32Dialog
    {
        public bool Shown { get; set; }

        public DialogResult DialogResult { get; set; }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            Shown = true;
            return default;
        }

        public void Close()
        {
            Shown = false;
        }

        public IntPtr Handle { get; }

        public void Dispose()
        {
            Close();
        }
    }
}
