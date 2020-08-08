using System;
using System.Collections.Generic;
using System.Windows.Forms;

using HFM.Forms.Views;

namespace HFM.Forms.Mocks
{
    public class MockWin32Form : IWin32Form
    {
        public bool Shown { get; set; }

        public void Show()
        {
            Shown = true;
        }

        public void Close()
        {
            Shown = false;
            OnClosed(this, EventArgs.Empty);
        }

        public FormWindowState WindowState { get; set; }

        public void BringToFront()
        {
            Invocations.Add(new MethodInvocation(nameof(BringToFront)));
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
        }

        public ICollection<MethodInvocation> Invocations { get; } = new List<MethodInvocation>();
    }
}
