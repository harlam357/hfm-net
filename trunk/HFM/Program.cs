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
using System.IO;
using System.Windows.Forms;

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Forms;
using HFM.Instrumentation;

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

         cmdArgs = argv;
         Application.EnableVisualStyles();
         Application.SetCompatibleTextRenderingDefault(false);

         #region Primary Initialization
         try
         {
            WindsorContainer container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
            InstanceProvider.SetContainer(container);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Windsor Container Failed to Initialize.  The HFM.exe.config file is likely corrupt.",
               "http://groups.google.com/group/hfm-net", PlatformOps.ApplicationNameAndVersion, true);
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
               "http://groups.google.com/group/hfm-net", PlatformOps.ApplicationNameAndVersion, true);
            return;
         }

         IMessagesView messagesView;
         try
         {
            messagesView = InstanceProvider.GetInstance<IMessagesView>();
            SetupTraceListeners(prefs, messagesView);
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Logging Failed to Initialize.",
               "http://groups.google.com/group/hfm-net", PlatformOps.ApplicationNameAndVersion, true);
            return;
         }

         frmMain frm = new frmMain(prefs, messagesView);
         try
         {
            frm.Initialize();
         }
         catch (Exception ex)
         {
            ExceptionDialog.ShowErrorDialog(ex, "Primary UI Failed to Initialize.",
               "http://groups.google.com/group/hfm-net", PlatformOps.ApplicationNameAndVersion, true);
            return;
         }
         #endregion

         ExceptionDialog.RegisterForUnhandledExceptions(PlatformOps.ApplicationNameAndVersion, HfmTrace.WriteToHfmConsole);
         Application.Run(frm);
         GC.KeepAlive(m);
      }

      /// <summary>
      /// Creates Trace Listener for Log File writing and Message Window output
      /// </summary>
      private static void SetupTraceListeners(IPreferenceSet prefs, IMessagesView messagesView)
      {
         // Ensure the HFM User Application Data Folder Exists
         string applicationDataFolderPath = prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);
         if (Directory.Exists(applicationDataFolderPath) == false)
         {
            Directory.CreateDirectory(applicationDataFolderPath);
         }

         string logFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmLogFileName);
         string prevLogFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmPrevLogFileName);

         FileInfo fi = new FileInfo(logFilePath);
         if (fi.Exists && fi.Length > 512000)
         {
            FileInfo fi2 = new FileInfo(prevLogFilePath);
            if (fi2.Exists)
            {
               fi2.Delete();
            }
            fi.MoveTo(prevLogFilePath);
         }

         TextWriterTraceListener listener = new TextWriterTraceListener(logFilePath);
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
