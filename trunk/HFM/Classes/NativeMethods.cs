/*
 * HFM.NET - Native P/Invoke Methods Class
 * Copyright (C) 2010 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

using System;
using System.Runtime.InteropServices;

namespace HFM.Classes
{
   /// <summary>
   /// Contains P/Invoke methods for functions in the Windows API.
   /// </summary>
   public static class NativeMethods
   {
      // ReSharper disable InconsistentNaming
      public const int WM_VSCROLL = 277;
      public const int SB_LINEUP = 0;
      public const int SB_LINEDOWN = 1;
      public const int SB_TOP = 6;
      public const int SB_BOTTOM = 7;
      // ReSharper restore InconsistentNaming

      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      public static extern IntPtr SendMessage(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);

      [DllImport("user32.dll")]
      public static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

      [DllImport("user32.dll", SetLastError = true)]
      public static extern IntPtr FindWindow(string lpClassName, string lpWindowName);

      [DllImport("user32.dll")]
      public static extern bool SetForegroundWindow(IntPtr hWnd);

      public static void ShowToFront(IntPtr hWnd)
      {
         ShowWindow(hWnd, 1);
         SetForegroundWindow(hWnd);
      }
   }
}
