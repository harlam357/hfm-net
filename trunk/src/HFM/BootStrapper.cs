/*
 * HFM.NET - Application Boot Strapper
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
using System.Collections.Generic;

using Castle.Core.Logging;
using Castle.Facilities.Logging;
using Castle.Windsor;

using harlam357.Windows.Forms;

using HFM.Core;

namespace HFM
{
   internal sealed class BootStrapper
   {
      public void Strap(string[] args)
      {
         #region Configure Container

         IWindsorContainer container = new WindsorContainer();
         // Facilities
         container.AddFacility<LoggingFacility>(f => f.LogUsing<Core.Logging.LoggerFactory>());
         // Components
         container.Install(new Configuration.ContainerInstaller(),
                           new Preferences.Configuration.ContainerInstaller(),
                           new Core.Configuration.ContainerInstaller());
         
         ServiceLocator.SetContainer(container);

         #endregion

         #region Create Object Maps

         Core.Configuration.ObjectMapper.CreateMaps();

         #endregion

         #region Parse Arguments

         ICollection<Argument> arguments;
         try
         {
            arguments = Arguments.Parse(args);
         }
         catch (FormatException ex)
         {
            // show usage dialog
            ShowStartupError(ex, null);
            return;
         }

         #endregion

         #region Process Arguments
         
         var processor = container.Resolve<ArgumentProcessor>();
         if (!processor.Process(arguments))
         {
            // arguments specified to exit the application
            return;
         }

         #endregion

         // Issue 180 - Restore the already running instance to the screen.
         using (var singleInstance = new SingleInstanceHelper())
         {
            #region Check Single Instance

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

            #endregion

            #region Set Logging Level

            var logger = (TraceLogger)container.Resolve<ILogger>();
            var prefs = container.Resolve<IPreferenceSet>();
            logger.Level = (LoggerLevel)prefs.Get<int>(Preference.MessageLevel);

            #endregion

            if (Application.IsRunningOnMono)
            {
               logger.Info("Running on Mono...");
            }

            // Handled by ContainerInstaller
            //#region Read Data Containers

            //var statsData = container.Resolve<IXmlStatsDataContainer>();
            //statsData.Read();
            //var proteinCollection = container.Resolve<IProteinDictionary>();
            //proteinCollection.Read();
            //var benchmarkContainer = container.Resolve<IProteinBenchmarkCollection>();
            //benchmarkContainer.Read();
            //var unitInfoContainer = container.Resolve<IUnitInfoCollection>();
            //unitInfoContainer.Read();

            //#endregion

            #region Load Plugins

            var pluginLoader = container.Resolve<Core.Plugins.PluginLoader>();
            pluginLoader.Load();

            #endregion

            #region Register IPC Channel

            try
            {
               SingleInstanceHelper.RegisterIpcChannel(NewInstanceDetected);
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, "Single Instance IPC channel failed to register.");
               return;
            }

            #region Initialize Main View

            //frmMain frm;
            //try
            //{
            //   frm = (frmMain)ServiceLocator.Resolve<IMainView>();
            //   frm.Initialize(ServiceLocator.Resolve<MainPresenter>(), proteinCollection);
            //   frm.WorkUnitHistoryMenuEnabled = connectionOk;
            //}
            //catch (Exception ex)
            //{
            //   ShowStartupError(ex, "Primary UI failed to initialize.");
            //   return;
            //}

            #endregion

            // Register the Unhandled Exception Dialog
            ExceptionDialog.RegisterForUnhandledExceptions(Application.NameAndVersionWithRevision,
               Environment.OSVersion.VersionString, ExceptionLogger);

            //System.Windows.Forms.Application.Run(frm);
         }

         #endregion
      }

      private void NewInstanceDetected(object sender, NewInstanceDetectedEventArgs e)
      {
         //var mainView = ServiceLocator.Resolve<IMainView>();
         //mainView.SecondInstanceStarted(e.Args);
      }

      private static void ExceptionLogger(Exception ex)
      {
         var logger = ServiceLocator.Resolve<ILogger>();
         logger.ErrorFormat(ex, "{0}", ex.Message);
      }

      internal static void ShowStartupError(Exception ex, string message)
      {
         ExceptionDialog.ShowErrorDialog(ex, Application.NameAndVersionWithRevision, Environment.OSVersion.VersionString,
            message, Constants.GoogleGroupUrl, true);
      }
      
      //try
      //{
      //   ClearCacheFolder(prefs);
      //}
      //catch (Exception ex)
      //{
      //   ShowStartupError(ex, "Failed to create or clear the data cache folder.");
      //   return;
      //}

      ///// <summary>
      ///// Clears the log cache folder specified by the CacheFolder setting
      ///// </summary>
      //private void ClearCacheFolder(IPreferenceSet prefs)
      //{
      //   string cacheFolder = Path.Combine(prefs.CacheDirectory,
      //                                     prefs.GetPreference<string>(Preference.CacheFolder));

      //   var di = new DirectoryInfo(cacheFolder);
      //   if (di.Exists == false)
      //   {
      //      di.Create();
      //   }
      //   else
      //   {
      //      foreach (var fi in di.GetFiles())
      //      {
      //         try
      //         {
      //            fi.Delete();
      //         }
      //         catch (Exception ex)
      //         {
      //            _logger.WarnFormat(ex, "Failed to clear cache file '{0}'.", fi.Name);
      //         }
      //      }
      //   }
      //}

      ///// <summary>
      ///// Creates Trace Listener for Log File writing and Message Window output
      ///// </summary>
      //private static void SetupTraceListeners(IPreferenceSet prefs, IMessagesView messagesView)
      //{
      //   // Ensure the HFM User Application Data Folder Exists
      //   var applicationDataFolderPath = prefs.ApplicationDataFolderPath;
      //   if (Directory.Exists(applicationDataFolderPath) == false)
      //   {
      //      Directory.CreateDirectory(applicationDataFolderPath);
      //   }

      //   string logFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmLogFileName);
      //   string prevLogFilePath = Path.Combine(applicationDataFolderPath, Constants.HfmPrevLogFileName);

      //   var fi = new FileInfo(logFilePath);
      //   if (fi.Exists && fi.Length > 512000)
      //   {
      //      var fi2 = new FileInfo(prevLogFilePath);
      //      if (fi2.Exists)
      //      {
      //         fi2.Delete();
      //      }
      //      fi.MoveTo(prevLogFilePath);
      //   }

      //   var listener = new TextWriterTraceListener(logFilePath);
      //   Trace.Listeners.Add(listener);
      //   Trace.AutoFlush = true;

      //   HfmTrace.Instance.TextMessage += ((sender, e) => messagesView.AddMessage(e.Message));
      //   HfmTrace.WriteToHfmConsole(String.Empty);
      //   HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.InvariantCulture, "Starting - HFM.NET v{0}", PlatformOps.ApplicationVersionWithRevision));
      //   HfmTrace.WriteToHfmConsole(String.Empty);
      //}
   }
}
