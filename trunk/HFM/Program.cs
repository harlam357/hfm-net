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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Classes;
using HFM.Forms;
using HFM.Instrumentation;

namespace HFM
{
   static class Program
   {
      public static string[] Args;

      /// <summary>
      /// The main entry point for the application.
      /// </summary>
      [STAThread]
      static void Main(string[] args)
      {
         AppDomain.CurrentDomain.AssemblyResolve += CustomResolve;

         Args = args;
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         #region Primary Initialization
         // Issue 180 - Restore the already running instance to the screen.
         try
         {
            if (!SingleInstanceHelper.Start())
            {
               SingleInstanceHelper.SignalFirstInstance(args);
               return;
            }
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Single Instance Helper Failed to Start.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }

         try
         {
            var container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
            InstanceProvider.SetContainer(container);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Windsor Container Failed to Initialize.  The HFM.exe.config file is likely corrupt.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }

         IPreferenceSet prefs;
         try
         {
            prefs = InstanceProvider.GetInstance<IPreferenceSet>();
            if (prefs.Initialize() == false) return;
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Preferences Failed to Initialize.  The user.config file is likely corrupt.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }

         try
         {
            SetupTraceListeners(prefs, InstanceProvider.GetInstance<IMessagesView>());
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Logging Failed to Initialize.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }

         frmMain frm;
         try
         {
            frm = InstanceProvider.GetInstance<frmMain>();
            frm.Initialize();
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Primary UI Failed to Initialize.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }
         #endregion

         try
         {
            SingleInstanceHelper.RegisterIpcChannel(frm);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Single Instance IPC Channel Failed to Register.",
               Constants.GoogleGroupUrl, PlatformOps.ApplicationNameAndVersionWithRevision, true);
            return;
         }

         ExceptionDialog.RegisterForUnhandledExceptions(PlatformOps.ApplicationNameAndVersionWithRevision, HfmTrace.WriteToHfmConsole);
         
         Application.Run(frm);
         SingleInstanceHelper.Stop();
      }

      private static System.Reflection.Assembly CustomResolve(object sender, ResolveEventArgs args)
      {
         const string sqliteDll = "System.Data.SQLite";
         if (args.Name.StartsWith(sqliteDll))
         {
            string platform = PlatformOps.IsRunningOnMono() ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (platform != null)
            {
               string filePath = Path.GetFullPath(Path.Combine(Path.Combine("SQLite", platform), String.Concat(sqliteDll, ".dll")));
               HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.CurrentCulture, "SQLite DLL Path: {0}", filePath));
               if (File.Exists(filePath))
               {
                  return System.Reflection.Assembly.LoadFile(filePath);
               }
            }
         }
         return null;
      }

      /// <summary>
      /// Creates Trace Listener for Log File writing and Message Window output
      /// </summary>
      private static void SetupTraceListeners(IPreferenceSet prefs, IMessagesView messagesView)
      {
         // Ensure the HFM User Application Data Folder Exists
         var applicationDataFolderPath = prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);
         if (Directory.Exists(applicationDataFolderPath) == false)
         {
            Directory.CreateDirectory(applicationDataFolderPath);
         }

         string logFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmLogFileName);
         string prevLogFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmPrevLogFileName);

         var fi = new FileInfo(logFilePath);
         if (fi.Exists && fi.Length > 512000)
         {
            var fi2 = new FileInfo(prevLogFilePath);
            if (fi2.Exists)
            {
               fi2.Delete();
            }
            fi.MoveTo(prevLogFilePath);
         }

         var listener = new TextWriterTraceListener(logFilePath);
         Trace.Listeners.Add(listener);
         Trace.AutoFlush = true;

         // Set Level to Warning to catch any errors that come from loading the preferences
         TraceLevelSwitch.Instance.Level = TraceLevel.Warning;

         HfmTrace.Instance.TextMessage += ((sender, e) => messagesView.AddMessage(e.Message));
         HfmTrace.WriteToHfmConsole(String.Empty);
         HfmTrace.WriteToHfmConsole(String.Format("Starting - HFM.NET v{0}", PlatformOps.ApplicationVersionWithRevision));
         HfmTrace.WriteToHfmConsole(String.Empty);

         // Get the actual TraceLevel from the preferences
         TraceLevelSwitch.Instance.Level = (TraceLevel)prefs.GetPreference<int>(Preference.MessageLevel);
      }
   }
}
