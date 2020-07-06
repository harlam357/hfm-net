
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting;
using System.Threading;
using System.Windows.Forms;

using Castle.Windsor;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Forms;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM
{
    internal class BootStrapper
    {
        public string[] Args { get; }
        public IWindsorContainer Container { get; }
        public Logger Logger { get; private set; }
        public IPreferenceSet Preferences { get; private set; }
        public Form MainForm { get; private set; }

        public BootStrapper(string[] args, IWindsorContainer container)
        {
            Args = args;
            Container = container;
        }

        internal void Execute()
        {
            var arguments = Arguments.Parse(Args);
            var errorArguments = arguments.Where(x => x.Type == ArgumentType.Unknown || x.Type == ArgumentType.Error).ToList();
            if (errorArguments.Count != 0)
            {
                MessageBox.Show(Arguments.GetUsageMessage(errorArguments), Core.Application.NameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            AppDomain.CurrentDomain.AssemblyResolve += AssemblyResolve;

            // Issue 180 - Restore the already running instance to the screen.
            using (var singleInstance = new SingleInstanceHelper())
            {
                if (!CheckSingleInstance(singleInstance))
                {
                    return;
                }

                Logger = InitializeLogging();
                if (!CheckMonoVersion())
                {
                    return;
                }

                Preferences = Container.Resolve<IPreferenceSet>();
                if (!InitializePreferences(arguments.Any(x => x.Type == ArgumentType.ResetPrefs)))
                {
                    return;
                }

                if (!ClearCacheFolder(Preferences.Get<string>(Preference.CacheDirectory)))
                {
                    return;
                }

                if (!RegisterIpcChannel())
                {
                    return;
                }

                var appDataPath = Preferences.Get<string>(Preference.ApplicationDataFolderPath);
                var mainView = Container.Resolve<IMainView>();
                if (!InitializeMainView(appDataPath, arguments, mainView))
                {
                    return;
                }

                RegisterForUnhandledExceptions();

                Application.ApplicationExit += (s, e) => Preferences.Save();
                MainForm = (Form)mainView;
            }
        }

        private bool CheckSingleInstance(SingleInstanceHelper singleInstance)
        {
            try
            {
                if (!singleInstance.Start())
                {
                    SingleInstanceHelper.SignalFirstInstance(Args);
                    return false;
                }
            }
            catch (RemotingException ex)
            {
                DialogResult result = MessageBox.Show(Properties.Resources.RemotingFailedQuestion,
                   Core.Application.NameAndVersion, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                if (result == DialogResult.No)
                {
                    ShowStartupException(ex, Properties.Resources.RemotingCallFailed);
                    return false;
                }
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.RemotingCallFailed);
                return false;
            }
            return true;
        }

        private Logger InitializeLogging()
        {
            // create messages view (hooks into logging messages)
            Container.Resolve<IMessagesView>();
            var logger = (Logger)Container.Resolve<ILogger>();
            // write log header
            logger.Info(String.Empty);
            logger.Info(String.Format(CultureInfo.InvariantCulture, "Starting - HFM.NET v{0}", Core.Application.FullVersion));
            logger.Info(String.Empty);

            Application.ApplicationExit += (s, e) =>
            {
                logger.Info("----------");
                logger.Info("Exiting...");
                logger.Info(String.Empty);
            };
            return logger;
        }

        private bool CheckMonoVersion()
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
                Logger.Warn(Properties.Resources.MonoDetectFailed, ex);
            }

            if (monoVersion != null)
            {
                if (monoVersion.Major < 2 || (monoVersion.Major == 2 && monoVersion.Minor < 8))
                {
                    var ex = new InvalidOperationException(Properties.Resources.MonoTooOld);
                    ShowStartupException(ex);
                    return false;
                }
                Logger.Info($"Running on Mono v{monoVersion}...");
            }
            else
            {
                Logger.Info("Running on Mono...");
            }
            return true;
        }

        private bool InitializePreferences(bool reset)
        {
            try
            {
                if (reset)
                {
                    Preferences.Reset();
                }
                else
                {
                    Preferences.Load();
                    ValidatePreferences();
                }
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.UserPreferencesFailed);
                return false;
            }

            // set logging level from preferences
            Logger.Level = Preferences.Get<LoggerLevel>(Preference.MessageLevel);

            // process logging level changes
            Preferences.PreferenceChanged += (s, e) =>
            {
                if (e.Preference == Preference.MessageLevel)
                {
                    var newLevel = Preferences.Get<LoggerLevel>(Preference.MessageLevel);
                    if (newLevel != Logger.Level)
                    {
                        Logger.Level = newLevel;
                        Logger.Info($"Logging Level Changed: {newLevel}");
                    }
                }
            };

            return true;
        }

        private void ValidatePreferences()
        {
            // MessageLevel
            var level = Preferences.Get<LoggerLevel>(Preference.MessageLevel);
            if (level < LoggerLevel.Info)
            {
                level = LoggerLevel.Info;
            }
            else if (level > LoggerLevel.Debug)
            {
                level = LoggerLevel.Debug;
            }
            Preferences.Set(Preference.MessageLevel, level);

            const int defaultInterval = 15;
            var clientRetrievalTask = Preferences.Get<Preferences.Data.ClientRetrievalTask>(Preference.ClientRetrievalTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(clientRetrievalTask.Interval))
            {
                clientRetrievalTask.Interval = defaultInterval;
                Preferences.Set(Preference.ClientRetrievalTask, clientRetrievalTask);
            }
            var webGenerationTask = Preferences.Get<Preferences.Data.WebGenerationTask>(Preference.WebGenerationTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(webGenerationTask.Interval))
            {
                webGenerationTask.Interval = defaultInterval;
                Preferences.Set(Preference.WebGenerationTask, webGenerationTask);
            }
        }

        private bool ClearCacheFolder(string cacheDirectory)
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
                            Logger.Warn(String.Format(Properties.Resources.CacheFileDeleteFailed, fi.Name), ex);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.CacheSetupFailed);
                return false;
            }
            return true;
        }

        private bool RegisterIpcChannel()
        {
            try
            {
                SingleInstanceHelper.RegisterIpcChannel((s, e) =>
                {
                    var mainView = Container.Resolve<IMainView>();
                    mainView.SecondInstanceStarted(e.Args);
                });
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.IpcRegisterFailed);
                return false;
            }
            return true;
        }

        private bool InitializeMainView(string appDataPath, ICollection<Argument> arguments, IMainView mainView)
        {
            var mainPresenter = Container.Resolve<MainPresenter>();
            string openFile = arguments.FirstOrDefault(x => x.Type == ArgumentType.OpenFile)?.Data;
            try
            {
                mainView.Initialize(mainPresenter, Container.Resolve<IProteinService>(), Container.Resolve<UserStatsDataModel>(), openFile);
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.FailedToInitUI);
                return false;
            }

            mainView.WorkUnitHistoryMenuEnabled = false;
            var repository = (WorkUnitRepository)Container.Resolve<IWorkUnitRepository>();
            try
            {
                repository.Initialize(Path.Combine(appDataPath, WorkUnitRepository.DefaultFileName));
                if (repository.RequiresUpgrade())
                {
                    using (var dialog = new ProgressDialogAsync((progress, token) => repository.Upgrade(progress), false))
                    {
                        dialog.Icon = Properties.Resources.hfm_48_48;
                        dialog.Text = Core.Application.NameAndVersion;
                        dialog.StartPosition = FormStartPosition.CenterScreen;
                        dialog.ShowDialog();
                        if (dialog.Exception != null)
                        {
                            ShowStartupException(dialog.Exception, Properties.Resources.WuHistoryUpgradeFailed, false);
                        }
                    }
                }
                
                mainView.WorkUnitHistoryMenuEnabled = repository.Connected;
            }
            catch (Exception ex)
            {
                ShowStartupException(ex, Properties.Resources.WuHistoryUpgradeFailed, false);
            }
            return true;
        }

        [SuppressMessage("Microsoft.Reliability", "CA2001:AvoidCallingProblematicMethods", MessageId = "System.Reflection.Assembly.LoadFile")]
        private System.Reflection.Assembly AssemblyResolve(object sender, ResolveEventArgs args)
        {
            const string sqliteDll = "System.Data.SQLite";
            if (args.Name.StartsWith(sqliteDll, StringComparison.Ordinal))
            {
                string platform = Core.Application.IsRunningOnMono ? "Mono" : Environment.GetEnvironmentVariable("PROCESSOR_ARCHITECTURE");
                if (platform != null)
                {
                    string filePath = Path.GetFullPath(Path.Combine(Application.StartupPath, "SQLite", platform, String.Concat(sqliteDll, ".dll")));
                    var logger = Container.Resolve<ILogger>();
                    logger.Info($"SQLite DLL Path: {filePath}");
                    if (File.Exists(filePath))
                    {
                        return System.Reflection.Assembly.LoadFile(filePath);
                    }
                }
            }
            return null;
        }

        public string ApplicationName { get; } = Core.Application.NameAndFullVersion;
        public string OSVersion { get; } = Environment.OSVersion.VersionString;
        public string ReportUrl { get; } = Core.Application.SupportForumUrl;

        public void RegisterForUnhandledExceptions()
        {
            var properties = new Dictionary<string, string>
            {
                { "Application", ApplicationName }, 
                { "OS Version", OSVersion }
            };
            
            Application.ThreadException += (s, e) => ShowThreadExceptionDialog(e, properties);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ShowUnhandledExceptionDialog(e, properties);
        }

        private void ShowThreadExceptionDialog(ThreadExceptionEventArgs e, IDictionary<string, string> properties)
        {
            using (var presenter = new ExceptionPresenter(Logger, MessageBoxPresenter.Default, properties, ReportUrl))
            {
                presenter.ShowDialog(null, e.Exception, false);
            }
        }

        private void ShowUnhandledExceptionDialog(UnhandledExceptionEventArgs e, IDictionary<string, string> properties)
        {
            using (var presenter = new ExceptionPresenter(Logger, MessageBoxPresenter.Default, properties, ReportUrl))
            {
                presenter.ShowDialog(null, (Exception)e.ExceptionObject, e.IsTerminating);
            }
        }

        internal void ShowStartupException(Exception exception, string message = null, bool mustTerminate = true)
        {
            var properties = new Dictionary<string, string>
            {
                { "Application", ApplicationName }, 
                { "OS Version", OSVersion },
                { "Startup Error", message }
            };

            using (var presenter = new ExceptionPresenter(Logger, MessageBoxPresenter.Default, properties, ReportUrl))
            {
                presenter.ShowDialog(null, exception, mustTerminate);
            }
        }
    }
}
