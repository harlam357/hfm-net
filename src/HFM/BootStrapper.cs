using System.Globalization;

using HFM.Core.Logging;
using HFM.Forms.Services;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Forms.Views;
using HFM.Preferences;

using LightInject;

namespace HFM
{
    internal class BootStrapper
    {
        public string[] Args { get; }
        public IServiceFactory Container { get; }
        public Logger Logger { get; private set; }
        public IPreferences Preferences { get; private set; }
        public Form MainForm { get; private set; }

        public BootStrapper(string[] args, IServiceFactory container)
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

            if (CheckSingleInstance())
            {
                Logger = InitializeLogging();
                Preferences = InitializePreferences(arguments);
                ClearCacheFolder();
                MainForm = InitializeMainForm(arguments);
                RegisterForUnhandledExceptions();

                Application.ApplicationExit += (s, e) => Preferences.Save();
            }
        }

        private static bool CheckSingleInstance()
        {
            bool singleInstance = SingleInstanceHelper.Start();
            if (!singleInstance)
            {
                MessageBox.Show(Properties.Resources.AlreadyRunning,
                    Core.Application.NameAndVersion, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            return singleInstance;
        }

        private Logger InitializeLogging()
        {
            // create messages model (hooks into logging messages)
            _ = Container.GetInstance<MessagesModel>();
            var logger = (Logger)Container.GetInstance<ILogger>();
            // write log header
            logger.Info(String.Empty);
            logger.Info(String.Format(CultureInfo.InvariantCulture, "Starting - {0} v{1}", Core.Application.Name, Core.Application.Version));
            logger.Info(String.Empty);

            Application.ApplicationExit += (s, e) =>
            {
                logger.Info("----------");
                logger.Info("Exiting...");
                logger.Info(String.Empty);
            };
            return logger;
        }

        private IPreferences InitializePreferences(ICollection<Argument> arguments)
        {
            bool reset = arguments.Any(x => x.Type == ArgumentType.ResetPrefs);
            var preferences = Container.GetInstance<IPreferences>();

            try
            {
                if (reset)
                {
                    preferences.Reset();
                }
                else
                {
                    preferences.Load();
                    ValidatePreferences(preferences);
                }
            }
            catch (Exception ex)
            {
                throw new StartupException(Properties.Resources.UserPreferencesFailed, ex);
            }

            // set logging level from preferences
            Logger.Level = preferences.Get<LoggerLevel>(Preference.MessageLevel);

            // process logging level changes
            preferences.PreferenceChanged += (s, e) =>
            {
                if (e.Preference == Preference.MessageLevel)
                {
                    var newLevel = preferences.Get<LoggerLevel>(Preference.MessageLevel);
                    if (newLevel != Logger.Level)
                    {
                        Logger.Level = newLevel;
                        Logger.Info($"Logging Level Changed: {newLevel}");
                    }
                }
            };

            return preferences;
        }

        private static void ValidatePreferences(IPreferences preferences)
        {
            // MessageLevel
            var level = preferences.Get<LoggerLevel>(Preference.MessageLevel);
            if (level < LoggerLevel.Info)
            {
                level = LoggerLevel.Info;
            }
            else if (level > LoggerLevel.Debug)
            {
                level = LoggerLevel.Debug;
            }
            preferences.Set(Preference.MessageLevel, level);

            const int defaultInterval = 15;
            var clientRetrievalTask = preferences.Get<Preferences.Data.ClientRetrievalTask>(Preference.ClientRetrievalTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(clientRetrievalTask.Interval))
            {
                clientRetrievalTask.Interval = defaultInterval;
                preferences.Set(Preference.ClientRetrievalTask, clientRetrievalTask);
            }
            var webGenerationTask = preferences.Get<Preferences.Data.WebGenerationTask>(Preference.WebGenerationTask);
            if (!Core.Client.ClientScheduledTasks.ValidateInterval(webGenerationTask.Interval))
            {
                webGenerationTask.Interval = defaultInterval;
                preferences.Set(Preference.WebGenerationTask, webGenerationTask);
            }
        }

        private void ClearCacheFolder()
        {
            var cacheDirectory = Preferences.Get<string>(Preference.CacheDirectory);

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
                throw new StartupException(Properties.Resources.CacheSetupFailed, ex);
            }
        }

        private MainForm InitializeMainForm(ICollection<Argument> arguments)
        {
            MainPresenter mainPresenter;
            string openFile = arguments.FirstOrDefault(x => x.Type == ArgumentType.OpenFile)?.Data;
            try
            {
                mainPresenter = Container.GetInstance<MainPresenter>();
                mainPresenter.ConfigFilePathFromArguments = openFile;
            }
            catch (Exception ex)
            {
                throw new StartupException(Properties.Resources.FailedToInitUI, ex);
            }

            var mainForm = (MainForm)mainPresenter.Form;
            try
            {
                var migration = new WorkUnitContextMigration(Container, Logger, Preferences);
                migration.Migrate();
            }
            catch (Exception ex)
            {
                mainForm.WorkUnitHistoryMenuItemEnabled = false;
                ShowStartupException(ex, Properties.Resources.WuHistoryUpgradeFailed, false);
            }

            return mainForm;
        }

        public void RegisterForUnhandledExceptions()
        {
            Application.ThreadException += (s, e) => ShowThreadExceptionDialog(e);
            AppDomain.CurrentDomain.UnhandledException += (s, e) => ShowUnhandledExceptionDialog(e);
        }

        private void ShowThreadExceptionDialog(ThreadExceptionEventArgs e)
        {
            var presenter = Container.GetInstance<ExceptionPresenterFactory>();
            presenter.ShowDialog(null, e.Exception, false);
        }

        private void ShowUnhandledExceptionDialog(UnhandledExceptionEventArgs e)
        {
            var presenter = Container.GetInstance<ExceptionPresenterFactory>();
            presenter.ShowDialog(null, (Exception)e.ExceptionObject, e.IsTerminating);
        }

        internal void ShowStartupException(Exception exception, string message = null, bool mustTerminate = true)
        {
            var properties = new Dictionary<string, string>
            {
                { "Application", Core.Application.NameAndVersion },
                { "OS Version", Environment.OSVersion.VersionString }
            };
            if (!String.IsNullOrEmpty(message))
            {
                properties.Add("Startup Error", message);
            }

            var presenter = new DefaultExceptionPresenterFactory(Logger, MessageBoxPresenter.Default, LocalProcessService.Default, properties, Core.Application.SupportForumUrl);
            presenter.ShowDialog(null, exception, mustTerminate);
        }
    }
}
