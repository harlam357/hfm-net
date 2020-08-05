using System;
using System.Runtime.InteropServices;

namespace HFM.Forms
{
    /// <summary>
    /// Contains P/Invoke methods for functions in the Windows API.
    /// </summary>
    internal static class NativeMethods
    {
        // ReSharper disable InconsistentNaming
        internal const int WM_VSCROLL = 277;
        internal const int SB_LINEUP = 0;
        internal const int SB_LINEDOWN = 1;
        internal const int SB_TOP = 6;
        internal const int SB_BOTTOM = 7;
        internal const int EM_LINESCROLL = 0x00B6;
        // ReSharper restore InconsistentNaming

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

        //[DllImport("user32.dll")]
        //internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        //[DllImport("user32.dll", SetLastError = true)]
        //internal static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);

        //internal static void ShowToFront(IntPtr hWnd)
        //{
        //   ShowWindow(hWnd, 1);
        //   SetForegroundWindow(hWnd);
        //}
    }
}
