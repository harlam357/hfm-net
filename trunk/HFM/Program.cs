/*
 * HFM.NET - Application Entry Point
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

using HFM.Framework;

namespace HFM
{
   static class Program
   {
      private static System.Threading.Mutex m;

      public static String[] cmdArgs;

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(String[] argv)
      {
         bool ok;
         m = new System.Threading.Mutex(true, "HFM", out ok);

         if (ok == false)
         {
            MessageBox.Show("Another instance of HFM.NET is already running.");
            return;
         }

         WindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
         InstanceProvider.SetContainer(container);

         cmdArgs = argv;
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);
         Application.Run(new Forms.frmMain(InstanceProvider.GetInstance<IPreferenceSet>()));

         GC.KeepAlive(m);
      }
   }
}
