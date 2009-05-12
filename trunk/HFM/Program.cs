/*
 * HFM.NET - Application Shell
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Windows.Forms;

namespace HFM
{
    static class Program
    {
        public static String[] cmdArgs;

        /// <summary>
        /// Configure the threadpool size
        /// </summary>
       //private static void SetThreadPool()
       //{
       //   int iMinThreads, iMinIOThreads;
       //   int iMaxThreads, iMaxIOThreads;
       //   int iThreads, iIOThreads;

       //   // Get the current sizes and use to ensure good initial sizes for the pool
       //   System.Threading.ThreadPool.GetMinThreads(out iMinThreads, out iMinIOThreads);
       //   System.Threading.ThreadPool.GetMaxThreads(out iMaxThreads, out iMaxIOThreads);

       //   iThreads = System.Math.Min(iMaxThreads, 10 * System.Environment.ProcessorCount);
       //   iIOThreads = System.Math.Min(iMaxIOThreads, 10 * iMinIOThreads * System.Environment.ProcessorCount);

       //   System.Threading.ThreadPool.SetMinThreads(iThreads, iIOThreads);
       //}

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(String[] argv)
        {
            cmdArgs = argv;
            Application.EnableVisualStyles();
            // Use GDI+ Text Rendering - Issue 9
            Application.SetCompatibleTextRenderingDefault(true);
            //SetThreadPool();
            Application.Run(new Forms.frmMain());
        }
    }
}