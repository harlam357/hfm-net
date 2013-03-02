/*
 * HFM.NET - Application Boot Strapper
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading.Tasks;
using System.Windows.Forms;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Forms;

namespace HFM
{
   internal sealed class BootStrapper
   {
      private ILogger _logger;
      private readonly IPreferenceSet _prefs;

      public BootStrapper(IPreferenceSet prefs)
      {
         _prefs = prefs;
      }

      public void Strap(string[] args)
      {
         #region Parse Arguments

         IEnumerable<Argument> arguments = Arguments.Parse(args);
         IEnumerable<Argument> errorArguments = arguments.Where(x => x.Type == ArgumentType.Unknown || x.Type == ArgumentType.Error);
         if (errorArguments.Count() != 0)
         {
            MessageBox.Show(Arguments.GetUsageMessage(errorArguments), Core.Application.NameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         #endregion

         #region Process Arguments
         
         _prefs.Process(arguments);

         #endregion

         try
         {
            _prefs.Initialize();
         }
         catch (Exception ex)
         {
            ShowStartupError(ex, Properties.Resources.UserPreferencesFailed);
            return;
         }

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
            catch (RemotingException ex)
            {
               if (MessageBox.Show(Properties.Resources.RemotingFailedQuestion, Core.Application.NameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning).Equals(DialogResult.No))
               {
                  ShowStartupError(ex, Properties.Resources.RemotingCallFailed);
                  return;
               }
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, Properties.Resources.RemotingCallFailed);
               return;
            }

            #endregion

            #region Setup Logging

            // create messages view (hooks into logging messages)
            ServiceLocator.Resolve<IMessagesView>();
            // write log header
            _logger = ServiceLocator.Resolve<ILogger>();
            _logger.Info(String.Empty);
            _logger.Info(String.Format(CultureInfo.InvariantCulture, "Starting - HFM.NET v{0}", Core.Application.VersionWithRevision));
            _logger.Info(String.Empty);
            // check for Mono runtime
            if (Core.Application.IsRunningOnMono)
            {
               Version monoVersion = null;
               try
               {
                  monoVersion = Core.Application.GetMonoVersionNumer();
               }
               catch (Exception ex)
               {
                  _logger.Warn(Properties.Resources.MonoDetectFailed, ex);
               }

               if (monoVersion != null)
               {
                  try
                  {
                     ValidateMonoVersion(monoVersion);
                  }
                  catch (InvalidOperationException ex)
                  {
                     ShowStartupError(ex, ex.Message);
                     return;
                  }
                  _logger.Info("Running on Mono v{0}...", monoVersion);
               }
               else
               {
                  _logger.Info("Running on Mono...");
               }
            }

            #endregion

            #region Setup Cache Folder

            try
            {
               ClearCacheFolder();
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, Properties.Resources.CacheSetupFailed);
               return;
            }

            #endregion

            #region Load Plugins

            var pluginLoader = ServiceLocator.Resolve<Core.Plugins.PluginLoader>();
            pluginLoader.Load();

            #endregion

            #region Register IPC Channel

            try
            {
               SingleInstanceHelper.RegisterIpcChannel(NewInstanceDetected);
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, Properties.Resources.IpcRegisterFailed);
               return;
            }

            #endregion

            #region Initialize Main View

            var mainView = ServiceLocator.Resolve<IMainView>();
            var mainPresenter = ServiceLocator.Resolve<MainPresenter>();
            mainPresenter.Arguments = arguments;
            try
            {
               mainView.Initialize(mainPresenter);
            }
            catch (Exception ex)
            {
               ShowStartupError(ex, Properties.Resources.FailedToInitUI);
               return;
            }

            mainView.WorkUnitHistoryMenuEnabled = false;
            var database = ServiceLocator.Resolve<IUnitInfoDatabase>();
            if (database.Connected)
            {
               database.UpgradeExecuting += DatabaseUpgradeExecuting;
               database.Upgrade();
               database.UpgradeExecuting -= DatabaseUpgradeExecuting;
               mainView.WorkUnitHistoryMenuEnabled = true;
            }

            #endregion

            #region Register the Unhandled Exception Dialog

            ExceptionDialog.RegisterForUnhandledExceptions(Core.Application.NameAndVersionWithRevision,
               Environment.OSVersion.VersionString, ExceptionLogger);

            #endregion

            System.Windows.Forms.Application.Run((Form)mainView);
         }
      }

      private static void DatabaseUpgradeExecuting(object sender, UpgradeExecutingEventArgs e)
      {
         // Execute Asynchronous Operation
         var view = ServiceLocator.Resolve<IProgressDialogView>("ProgressDialog");
         view.ProcessRunner = e.Process;
         view.Icon = Properties.Resources.hfm_48_48;
         view.Text = "Upgrading Database";
         view.StartPosition = FormStartPosition.CenterScreen;
         view.Process();
      }

      private static void NewInstanceDetected(object sender, NewInstanceDetectedEventArgs e)
      {
         var mainView = ServiceLocator.Resolve<IMainView>();
         mainView.SecondInstanceStarted(e.Args);
      }

      private static void ExceptionLogger(Exception ex)
      {
         var logger = ServiceLocator.Resolve<ILogger>();
         logger.ErrorFormat(ex, "{0}", ex.Message);
      }

      internal static void ShowStartupError(Exception ex, string message)
      {
         ExceptionDialog.ShowErrorDialog(ex, Core.Application.NameAndVersionWithRevision, Environment.OSVersion.VersionString,
            message, Constants.GoogleGroupUrl, true);
      }
      
      private static void ValidateMonoVersion(Version monoVersion)
      {
         Debug.Assert(monoVersion != null);
         if (monoVersion.Major < 2 || monoVersion.Minor < 8)
         {
            throw new InvalidOperationException(Properties.Resources.MonoTooOld);
         }
      }

      /// <summary>
      /// Clears the log cache folder specified by the CacheFolder setting
      /// </summary>
      private void ClearCacheFolder()
      {
         var di = new DirectoryInfo(_prefs.CacheDirectory);
         if (!di.Exists)
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
                  _logger.WarnFormat(ex, Properties.Resources.CacheFileDeleteFailed, fi.Name);
               }
            }
         }
      }
   }
}
