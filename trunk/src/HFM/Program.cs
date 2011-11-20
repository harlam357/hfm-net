/*
 * HFM.NET - Application Entry Point
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;
using System.IO;

using Application = System.Windows.Forms.Application;

namespace HFM
{
   static class Program
   {
      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         Application.ApplicationExit += ApplicationExit;
         AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;

         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         try
         {
            var bootStrapper = new BootStrapper();
            bootStrapper.Strap(args);
         }
         catch (Exception ex)
         {
            BootStrapper.ShowStartupError(ex, null);
         }
      }
      
      private static void ApplicationExit(object sender, EventArgs e)
      {
         // Save preferences
         var prefs = Core.ServiceLocator.Resolve<Core.IPreferenceSet>();
         prefs.Save();
         // Save the benchmark collection
         var benchmarkContainer = Core.ServiceLocator.Resolve<Core.IProteinBenchmarkCollection>();
         benchmarkContainer.Write();

         var logger = Core.ServiceLocator.Resolve<Castle.Core.Logging.ILogger>();
         logger.Info("----------");
         logger.Info("Exiting...");
         logger.Info(String.Empty);
      }

      [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
      private static System.Reflection.Assembly CustomResolve(object sender, ResolveEventArgs args)
      {
         const string sqliteDll = "System.Data.SQLite";
         if (args.Name.StartsWith(sqliteDll, StringComparison.Ordinal))
         {
            string platform = Core.Application.IsRunningOnMono ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (platform != null)
            {
               string filePath = Path.GetFullPath(Path.Combine(Application.StartupPath, Path.Combine(Path.Combine("SQLite", platform), String.Concat(sqliteDll, ".dll"))));
               var logger = Core.ServiceLocator.Resolve<Castle.Core.Logging.ILogger>();
               logger.Info("SQLite DLL Path: {0}", filePath);
               if (File.Exists(filePath))
               {
                  return System.Reflection.Assembly.LoadFile(filePath);
               }
            }
         }
         return null;
      }
   }
}
