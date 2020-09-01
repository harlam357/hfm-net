using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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

        public Point Location { get; set; }

        public Size Size { get; set; }

        public Size MinimumSize { get; set; }

        public event EventHandler Closed;

        protected virtual void OnClosed(object sender, EventArgs e)
        {
            Closed?.Invoke(this, e);
        }

        public IntPtr Handle => IntPtr.Zero;

        public void Dispose()
        {
            Close();
        }

        public ICollection<MethodInvocation> Invocations { get; } = new List<MethodInvocation>();

        // ISynchronizeInvoke
        IAsyncResult ISynchronizeInvoke.BeginInvoke(Delegate method, object[] args) => throw new NotImplementedException();

        object ISynchronizeInvoke.EndInvoke(IAsyncResult result) => throw new NotImplementedException();

        object ISynchronizeInvoke.Invoke(Delegate method, object[] args) => throw new NotImplementedException();

        bool ISynchronizeInvoke.InvokeRequired => false;
    }
}
