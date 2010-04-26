/*
 * HFM.NET - Single Instance Helper Class
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

/*
 * Class based primarily on code from: http://www.codeproject.com/KB/cs/SingleInstanceAppMutex.aspx
 */

using System;
using System.Threading;
using System.Windows.Forms;

using HFM.Forms;
using HFM.Framework;

namespace HFM.Classes
{
   public static class SingleInstanceHelper
   {
      private static Mutex _mutex;

      public static bool Start()
      {
         string mutexName = String.Format("Global\\{0}", PlatformOps.AssemblyGuid);

         bool onlyInstance;
         _mutex = new Mutex(true, mutexName, out onlyInstance);
         return onlyInstance;
      }

      public static void ShowFirstInstance()
      {
         if (PlatformOps.IsRunningOnMono())
         {
            MessageBox.Show("Another instance of HFM.NET is already running.");
         }
         else
         {
            IntPtr hWnd = NativeMethods.FindWindow(null, frmMain.FormTitle);
            NativeMethods.ShowToFront(hWnd);
         }
      }

      public static void Stop()
      {
         _mutex.ReleaseMutex();
      }
   }
}
