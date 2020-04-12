/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Windows.Forms;

using Castle.Windsor;

using harlam357.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Forms;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM
{
    internal static class BootStrapper
    {
        internal static void Execute(string[] args, IWindsorContainer container)
        {
            var arguments = Arguments.Parse(args);
            var errorArguments = arguments.Where(x => x.Type == ArgumentType.Unknown || x.Type == ArgumentType.Error).ToList();
            if (errorArguments.Count != 0)
            {
                MessageBox.Show(Arguments.GetUsageMessage(errorArguments), Core.Application.NameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += (s, e) => CustomResolve(e, container);
            Core.Internal.AsyncProcessorExtensions.ExecuteAsyncWithProgressAction = AsyncProcessorExtensions.ExecuteAsyncWithProgress;

            // Issue 180 - Restore the already running instance to the screen.
            using (var singleInstance = new SingleInstanceHelper())
            {
                if (!CheckSingleInstance(args, singleInstance))
                {
                    return;
                }

                var logger = InitializeLogging(container);
                if (!CheckMonoVersion(logger))
                {
                    return;
                }

                var prefs = container.Resolve<IPreferenceSet>();
                if (!InitializePreferences(prefs, logger, arguments.Any(x => x.Type == ArgumentType.ResetPrefs)))
                {
                    return;
                }

                if (!ClearCacheFolder(prefs.Get<string>(Preference.CacheDirectory), logger))
                {
                    return;
                }

                if (!RegisterIpcChannel(container))
                {
                    return;
                }

                var appDataPath = prefs.Get<string>(Preference.ApplicationDataFolderPath);
                var mainView = container.Resolve<IMainView>();
                if (!InitializeMainView(appDataPath, container, arguments, mainView))
                {
                    return;
                }

                ExceptionDialog.RegisterForUnhandledExceptions(
                   Core.Application.NameAndFullVersion,
                   Environment.OSVersion.VersionString,
                   ex => logger.Error(ex.Message, ex));

                System.Windows.Forms.Application.ApplicationExit += (s, e) =>
                {
                // Save preferences
                prefs.Save();
                };
                System.Windows.Forms.Application.Run((Form)mainView);
            }
        }

        private static bool CheckSingleInstance(string[] args, SingleInstanceHelper singleInstance)
        {
            try
            {
                if (!singleInstance.Start())
                {
                    SingleInstanceHelper.SignalFirstInstance(args);
                    return false;
                }
            }
            catch (RemotingException ex)
            {
                DialogResult result = MessageBox.Show(Properties.Resources.RemotingFailedQuestion,
                   Core.Application.NameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    ShowStartupError(ex, Properties.Resources.RemotingCallFailed);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.RemotingCallFailed);
                return false;
            }
            return true;
        }

        private static ILogger InitializeLogging(IWindsorContainer container)
        {
            // create messages view (hooks into logging messages)
            container.Resolve<IMessagesView>();
            var logger = container.Resolve<ILogger>();
            // write log header
            logger.Info(String.Empty);
            logger.Info(String.Format(CultureInfo.InvariantCulture, "Starting - HFM.NET v{0}", Core.Application.FullVersion));
            logger.Info(String.Empty);

            System.Windows.Forms.Application.ApplicationExit += (s, e) =>
            {
                logger.Info("----------");
                logger.Info("Exiting...");
                logger.Info(String.Empty);
            };
            return logger;
        }

        private static bool CheckMonoVersion(ILogger logger)
        {
            // check for Mono runtime
            if (!Core.Application.IsRunningOnMono)
            {
                return true;
            }

            Version monoVersion = null;
            try
            {
                monoVersion = Core.Application.GetMonoVersion();
            }
            catch (Exception ex)
            {
                logger.Warn(Properties.Resources.MonoDetectFailed, ex);
            }

            if (monoVersion != null)
            {
                if (monoVersion.Major < 2 || (monoVersion.Major == 2 && monoVersion.Minor < 8))
                {
                    var ex = new InvalidOperationException(Properties.Resources.MonoTooOld);
                    ShowStartupError(ex);
                    return false;
                }
                logger.Info($"Running on Mono v{monoVersion}...");
            }
            else
            {
                logger.Info("Running on Mono...");
            }
            return true;
        }

        private static bool InitializePreferences(IPreferenceSet prefs, ILogger logger, bool reset)
        {
            try
            {
                if (reset)
                {
                    prefs.Reset();
                }
                else
                {
                    prefs.Load();
                    ValidatePreferences(prefs);
                }
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.UserPreferencesFailed);
                return false;
            }
            // set logging level from prefs
            ((Core.Logging.Logger)logger).Level = (LoggerLevel)prefs.Get<int>(Preference.MessageLevel);
            return true;
        }

        private static void ValidatePreferences(IPreferenceSet prefs)
        {
            // MessageLevel
            int level = prefs.Get<int>(Preference.MessageLevel);
            if (level < 4)
            {
                level = 4;
            }
            else if (level > 5)
            {
                level = 5;
            }
            prefs.Set(Preference.MessageLevel, level);

            const int defaultInterval = 15;
            var clientRetrievalTask = prefs.Get<Preferences.Data.ClientRetrievalTask>(Preference.ClientRetrievalTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(clientRetrievalTask.Interval))
            {
                clientRetrievalTask.Interval = defaultInterval;
                prefs.Set(Preference.ClientRetrievalTask, clientRetrievalTask);
            }
            var webGenerationTask = prefs.Get<Preferences.Data.WebGenerationTask>(Preference.WebGenerationTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(webGenerationTask.Interval))
            {
                webGenerationTask.Interval = defaultInterval;
                prefs.Set(Preference.WebGenerationTask, webGenerationTask);
            }
        }

        private static bool ClearCacheFolder(string cacheDirectory, ILogger logger)
        {
            try
            {
                var di = new DirectoryInfo(cacheDirectory);
                if (!di.Exists)
                {
                    di.Create();
                }
                else
                {
                    foreach (var fi in di.EnumerateFiles())
                    {
                        try
                        {
                            fi.Delete();
                        }
                        catch (Exception ex)
                        {
                            logger.Warn(String.Format(Properties.Resources.CacheFileDeleteFailed, fi.Name), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.CacheSetupFailed);
                return false;
            }
            return true;
        }

        private static bool RegisterIpcChannel(IWindsorContainer container)
        {
            try
            {
                SingleInstanceHelper.RegisterIpcChannel((s, e) =>
                {
                    var mainView = container.Resolve<IMainView>();
                    mainView.SecondInstanceStarted(e.Args);
                });
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.IpcRegisterFailed);
                return false;
            }
            return true;
        }

        private static bool InitializeMainView(string appDataPath, IWindsorContainer container, ICollection<Argument> arguments, IMainView mainView)
        {
            var mainPresenter = container.Resolve<MainPresenter>();
            string openFile = arguments.FirstOrDefault(x => x.Type == ArgumentType.OpenFile)?.Data;
            try
            {
                mainView.Initialize(mainPresenter, container.Resolve<IProteinService>(), container.Resolve<UserStatsDataModel>(), openFile);
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.FailedToInitUI);
                return false;
            }

            mainView.WorkUnitHistoryMenuEnabled = false;
            var repository = container.Resolve<IWorkUnitRepository>();
            try
            {
                repository.Initialize(Path.Combine(appDataPath, WorkUnitRepository.DefaultFileName));
                mainView.WorkUnitHistoryMenuEnabled = repository.Connected;
            }
            catch (Exception ex)
            {
                ShowStartupError(ex, Properties.Resources.WuHistoryUpgradeFailed, false);
            }
            return true;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        private static System.Reflection.Assembly CustomResolve(ResolveEventArgs args, IWindsorContainer container)
        {
            const string sqliteDll = "System.Data.SQLite";
            if (args.Name.StartsWith(sqliteDll, StringComparison.Ordinal))
            {
                string platform = Core.Application.IsRunningOnMono ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                if (platform != null)
                {
                    string filePath = Path.GetFullPath(Path.Combine(System.Windows.Forms.Application.StartupPath, Path.Combine(Path.Combine("SQLite", platform), String.Concat(sqliteDll, ".dll"))));
                    var logger = container.Resolve<ILogger>();
                    logger.Info($"SQLite DLL Path: {filePath}");
                    if (File.Exists(filePath))
                    {
                        return System.Reflection.Assembly.LoadFile(filePath);
                    }
                }
            }
            return null;
        }

        internal static void ShowStartupError(Exception ex, string message = null, bool mustTerminate = true)
        {
            ExceptionDialog.ShowErrorDialog(
               ex,
               Core.Application.NameAndFullVersion,
               Environment.OSVersion.VersionString,
               message,
               Core.Application.SupportForumUrl,
               mustTerminate);
        }
    }
}
