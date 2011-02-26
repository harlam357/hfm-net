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
using System.Linq;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using Castle.Windsor;
using Castle.Windsor.Configuration.Interpreters;
using Castle.Core.Resource;

using harlam357.Windows.Forms;

using HFM.Classes;
using HFM.Forms;
using HFM.Framework;
using HFM.Instances;

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
         
         ICollection<Argument> arguments;
         try
         {
            arguments = Arguments.Parse(args);
         }
         catch (FormatException ex)
         {
            //TODO: show usage
            ShowStartupError(ex, null);
            return;
         }

         // Issue 180 - Restore the already running instance to the screen.
         using (var singleInstance = new SingleInstanceHelper())
         {
            try
            {
               if (!singleInstance.Start())
               {
                  SingleInstanceHelper.SignalFirstInstance(args);
                  return;
               }
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, "Failed to signal first instance of HFM.NET.  Please try starting HFM.NET again before reporting this issue.");
               return;
            }

            Initialize(arguments);
         }
      }
      
      private static void Initialize(IEnumerable<Argument> arguments)
      {
         #region Primary Initialization

         try
         {
            var container = new WindsorContainer(new XmlInterpreter(new ConfigResource("castle")));
            InstanceProvider.SetContainer(container);
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "Windsor Container failed to initialize.  Either components are missing or the HFM.exe.config file is corrupt.");
            return;
         }

         IPreferenceSet prefs;
         try
         {
            prefs = InstanceProvider.GetInstance<IPreferenceSet>();
            SetupTraceListeners(prefs, InstanceProvider.GetInstance<IMessagesView>());
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "Logging failed to initialize.");
            return;
         }

         try
         {
            if (!SetupUserPreferences(arguments, prefs))
            {
               return;
            }
            // Get the actual TraceLevel from the preferences
            TraceLevelSwitch.Instance.Level = (TraceLevel)prefs.GetPreference<int>(Preference.MessageLevel);
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "User preferences failed to initialize.  The user.config file is likely corrupt.  Start with the '/r' switch to reset the user preferences.");
            return;
         }

         if (PlatformOps.IsRunningOnMono())
         {
            HfmTrace.WriteToHfmConsole("Running on Mono...");
         }

         bool connectionOk = false;
         try
         {
            var database = InstanceProvider.GetInstance<IUnitInfoDatabase>();
            database.DatabaseFilePath = Path.Combine(prefs.ApplicationDataFolderPath, Constants.SqLiteFilename);
            connectionOk = database.CheckConnection();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }
         
         try
         {
            ClearCacheFolder(prefs);
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "Failed to create or clear the data cache folder.");
            return;
         }

         // Read Containers
         var statsData = InstanceProvider.GetInstance<IXmlStatsDataContainer>();
         statsData.Read();
         var proteinCollection = InstanceProvider.GetInstance<IProteinCollection>();
         proteinCollection.Read();
         var benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Read();
         var unitInfoContainer = InstanceProvider.GetInstance<IUnitInfoContainer>();
         unitInfoContainer.Read();

         // Initialize the Instance Collection
         var instanceCollection = InstanceProvider.GetInstance<IInstanceCollection>();
         instanceCollection.Initialize();

         frmMain frm;
         try
         {
            frm = (frmMain)InstanceProvider.GetInstance<IMainView>();
            frm.Initialize(InstanceProvider.GetInstance<MainPresenter>(), proteinCollection);
            frm.WorkUnitHistoryMenuEnabled = connectionOk;
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "Primary UI failed to initialize.");
            return;
         }

         try
         {
            SingleInstanceHelper.RegisterIpcChannel(frm.SecondInstanceStarted);
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, "Single Instance IPC channel failed to register.");
            return;
         }

         // Register the Unhandled Exception Dialog
         ExceptionDialog.RegisterForUnhandledExceptions(PlatformOps.ApplicationNameAndVersionWithRevision, 
            Environment.OSVersion.VersionString, HfmTrace.WriteToHfmConsole);

         Application.Run(frm);

         #endregion
      }
      
      private static bool SetupUserPreferences(IEnumerable<Argument> arguments, IPreferenceSet prefs)
      {
         var argument = arguments.FirstOrDefault(x => x.Type.Equals(ArgumentType.ResetPrefs));
         if (argument != null)
         {
            var userConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (userConfig.HasFile)
            {
               File.Delete(userConfig.FilePath);
            }
            // Reset
            prefs.Reset();

            return false;
         }
         
         // Upgrade
         prefs.Upgrade();
         // Init
         prefs.Initialize();

         return true;
      }

      private static void ShowStartupError(Exception ex, string message)
      {
         ExceptionDialog.ShowErrorDialog(ex, PlatformOps.ApplicationNameAndVersionWithRevision, Environment.OSVersion.VersionString,
            message, Constants.GoogleGroupUrl, true);
      }

      private static void ApplicationExit(object sender, EventArgs e)
      {
         // Save preferences
         var prefs = InstanceProvider.GetInstance<IPreferenceSet>();
         prefs.Save();
         // Save the benchmark collection
         var benchmarkContainer = InstanceProvider.GetInstance<IProteinBenchmarkContainer>();
         benchmarkContainer.Write();

         HfmTrace.WriteToHfmConsole("----------");
         HfmTrace.WriteToHfmConsole("Exiting...");
         HfmTrace.WriteToHfmConsole(String.Empty);
      }

      private static System.Reflection.Assembly CustomResolve(object sender, ResolveEventArgs args)
      {
         const string sqliteDll = "System.Data.SQLite";
         if (args.Name.StartsWith(sqliteDll, StringComparison.Ordinal))
         {
            string platform = PlatformOps.IsRunningOnMono() ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
            if (platform != null)
            {
               string filePath = Path.GetFullPath(Path.Combine(Application.StartupPath, Path.Combine(Path.Combine("SQLite", platform), String.Concat(sqliteDll, ".dll"))));
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
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private static void ClearCacheFolder(IPreferenceSet prefs)
      {
         DateTime start = HfmTrace.ExecStart;

         string cacheFolder = Path.Combine(prefs.ApplicationDataFolderPath,
                                           prefs.GetPreference<string>(Preference.CacheFolder));

         var di = new DirectoryInfo(cacheFolder);
         if (di.Exists == false)
         {
            di.Create();
         }
         else
         {
            foreach (var fi in di.GetFiles())
            {
               try
               {
                  fi.Delete();
               }
               catch (Exception ex)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, String.Format(
                     CultureInfo.InvariantCulture, "Failed to clear cache file '{0}'.", fi.Name), ex);
               }
            }
         }

         HfmTrace.WriteToHfmConsole(TraceLevel.Info, start);
      }

      /// <summary>
      /// Creates Trace Listener for Log File writing and Message Window output
      /// </summary>
      private static void SetupTraceListeners(IPreferenceSet prefs, IMessagesView messagesView)
      {
         // Ensure the HFM User Application Data Folder Exists
         var applicationDataFolderPath = prefs.ApplicationDataFolderPath;
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

         HfmTrace.Instance.TextMessage += ((sender, e) => messagesView.AddMessage(e.Message));
         HfmTrace.WriteToHfmConsole(String.Empty);
         HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.InvariantCulture, "Starting - HFM.NET v{0}", PlatformOps.ApplicationVersionWithRevision));
         HfmTrace.WriteToHfmConsole(String.Empty);
      }
   }
}
