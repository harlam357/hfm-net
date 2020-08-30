
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;

using HFM.Preferences.Data;
using HFM.Preferences.Internal;

namespace HFM.Preferences
{
    public abstract class PreferenceSetBase : IPreferenceSet
    {
        public string ApplicationPath { get; }

        public string ApplicationDataFolderPath { get; }

        public string ApplicationVersion { get; }

        private PreferenceDictionary _prefs;

        protected PreferenceSetBase(string applicationPath, string applicationDataFolderPath, string applicationVersion)
        {
            ApplicationPath = applicationPath;
            ApplicationDataFolderPath = applicationDataFolderPath;
            ApplicationVersion = applicationVersion;

            var data = new PreferenceData { ApplicationVersion = ApplicationVersion };
            _prefs = CreateDictionary(data);
        }

        public void Reset()
        {
            var data = new PreferenceData { ApplicationVersion = ApplicationVersion };
            Write(data);
            _prefs = CreateDictionary(data);
        }

        public void Load()
        {
            var data = Read() ?? Migrate() ?? new PreferenceData();
            Load(data);
        }

        public void Load(PreferenceData data)
        {
            Upgrade(data);
            _prefs = CreateDictionary(data);
        }

        private PreferenceData Migrate()
        {
            return null;
        }

        private void Upgrade(PreferenceData data)
        {
            if (data.ApplicationVersion == ApplicationVersion)
            {
                return;
            }

            if (Version.TryParse(data.ApplicationVersion, out Version settingsVersion))
            {
                ExecuteUpgrades(settingsVersion, data);
            }
            data.ApplicationVersion = ApplicationVersion;
            Write(data);
        }

        private void ExecuteUpgrades(Version settingsVersion, PreferenceData data)
        {
            foreach (var upgrade in EnumerateUpgrades().Where(upgrade => settingsVersion < upgrade.Version))
            {
                upgrade.Action(data);
            }
        }

        private IEnumerable<PreferenceUpgrade> EnumerateUpgrades()
        {
            return OnEnumerateUpgrades();
        }

        protected virtual IEnumerable<PreferenceUpgrade> OnEnumerateUpgrades()
        {
            yield break;
        }

        protected class PreferenceUpgrade
        {
            public Version Version { get; set; }

            public Action<PreferenceData> Action { get; set; }
        }

        private PreferenceDictionary CreateDictionary(PreferenceData data)
        {
            var prefs = new PreferenceDictionary(data);

            prefs.AddReadOnly(Preference.ApplicationPath, p => ApplicationPath);
            prefs.AddReadOnly(Preference.ApplicationDataFolderPath, p => ApplicationDataFolderPath);
            prefs.AddReadOnly(Preference.ApplicationVersion, p => p.ApplicationVersion);
            prefs.AddReadOnly(Preference.CacheDirectory, p => Path.Combine(ApplicationDataFolderPath, p.ApplicationSettings.CacheFolder));

            prefs.Add(Preference.FormLocation, p => p.MainWindow.Location);
            prefs.Add(Preference.FormSize, p => p.MainWindow.Size);
            prefs.Add(Preference.FormColumns, p => p.MainWindowGrid.Columns);
            prefs.Add(Preference.FormSortColumn, p => p.MainWindowGrid.SortColumn);
            prefs.Add(Preference.FormSortOrder, p => p.MainWindowGrid.SortOrder);
            prefs.Add(Preference.FormSplitterLocation, p => p.MainWindowState.SplitterLocation);
            prefs.Add(Preference.FormLogWindowHeight, p => p.MainWindowState.LogWindowHeight);
            prefs.Add(Preference.FormLogWindowVisible, p => p.MainWindowState.LogWindowVisible);
            prefs.Add(Preference.QueueWindowVisible, p => p.MainWindowState.QueueWindowVisible);
            prefs.Add(Preference.TimeFormatting, p => p.MainWindowGridProperties.TimeFormatting);
            prefs.Add(Preference.UnitTotals, p => p.MainWindowGridProperties.UnitTotals);
            prefs.Add(Preference.DisplayVersions, p => p.MainWindowGridProperties.DisplayVersions);

            prefs.Add(Preference.MinimizeTo, p => p.MainWindowProperties.MinimizeTo);
            prefs.Add(Preference.EnableUserStats, p => p.MainWindowProperties.EnableStats);
            prefs.Add(Preference.UserStatsType, p => p.MainWindowProperties.StatsType);

            prefs.Add(Preference.BenchmarksFormLocation, p => p.BenchmarksWindow.Location);
            prefs.Add(Preference.BenchmarksFormSize, p => p.BenchmarksWindow.Size);
            prefs.Add(Preference.GraphColors, p => p.BenchmarksGraphing.GraphColors);

            prefs.Add(Preference.MessagesFormLocation, p => p.MessagesWindow.Location);
            prefs.Add(Preference.MessagesFormSize, p => p.MessagesWindow.Size);

            prefs.Add(Preference.ClientRetrievalTask, p => p.ClientRetrievalTask);
            prefs.AddReadOnly(Preference.ClientRetrievalTaskEnabled, p => p.ClientRetrievalTask.Enabled);
            prefs.AddReadOnly(Preference.ClientRetrievalTaskInterval, p => p.ClientRetrievalTask.Interval);
            prefs.AddReadOnly(Preference.ClientRetrievalTaskType, p => p.ClientRetrievalTask.ProcessingMode);

            prefs.Add(Preference.DuplicateProjectCheck, p => p.ApplicationSettings.DuplicateProjectCheck);

            prefs.Add(Preference.WebGenerationTask, p => p.WebGenerationTask);
            prefs.AddReadOnly(Preference.WebGenerationTaskEnabled, p => p.WebGenerationTask.Enabled);
            prefs.AddReadOnly(Preference.WebGenerationTaskInterval, p => p.WebGenerationTask.Interval);
            prefs.AddReadOnly(Preference.WebGenerationTaskAfterClientRetrieval, p => p.WebGenerationTask.AfterClientRetrieval);
            prefs.Add(Preference.WebDeploymentType, p => p.WebDeployment.DeploymentType);
            prefs.Add(Preference.WebDeploymentRoot, p => p.WebDeployment.DeploymentRoot);
            prefs.Add(Preference.WebGenServer, p => p.WebDeployment.FtpServer.Address);
            prefs.Add(Preference.WebGenPort, p => p.WebDeployment.FtpServer.Port);
            prefs.Add(Preference.WebGenUsername, p => p.WebDeployment.FtpServer.Username);
            prefs.AddEncrypted(Preference.WebGenPassword, p => p.WebDeployment.FtpServer.Password);
            prefs.Add(Preference.WebGenCopyFAHlog, p => p.WebDeployment.CopyLog);
            prefs.Add(Preference.WebGenFtpMode, p => p.WebDeployment.FtpMode);
            prefs.Add(Preference.WebGenCopyHtml, p => p.WebDeployment.CopyHtml);
            prefs.Add(Preference.WebGenCopyXml, p => p.WebDeployment.CopyXml);
            prefs.Add(Preference.WebGenLimitLogSize, p => p.WebDeployment.LogSizeLimitEnabled);
            prefs.Add(Preference.WebGenLimitLogSizeLength, p => p.WebDeployment.LogSizeLimitedTo);
            prefs.Add(Preference.CssFile, p => p.WebRendering.StyleSheet);
            prefs.Add(Preference.WebOverview, p => p.WebRendering.OverviewTransform);
            prefs.Add(Preference.WebSummary, p => p.WebRendering.SummaryTransform);
            prefs.Add(Preference.WebSlot, p => p.WebRendering.SlotTransform);

            prefs.Add(Preference.RunMinimized, p => p.Startup.RunMinimized);
            prefs.Add(Preference.StartupCheckForUpdate, p => p.Startup.CheckForUpdate);
            prefs.Add(Preference.UseDefaultConfigFile, p => p.Startup.DefaultConfigFileEnabled);
            prefs.Add(Preference.DefaultConfigFile, p => p.Startup.DefaultConfigFilePath);

            prefs.Add(Preference.OfflineLast, p => p.MainWindowGridProperties.OfflineClientsLast);
            prefs.Add(Preference.ColorLogFile, p => p.LogWindowProperties.ApplyColor);
            prefs.Add(Preference.AutoSaveConfig, p => p.ApplicationSettings.AutoSaveConfig);
            prefs.Add(Preference.PPDCalculation, p => p.ApplicationSettings.PpdCalculation);
            prefs.Add(Preference.DecimalPlaces, p => p.ApplicationSettings.DecimalPlaces);
            prefs.Add(Preference.BonusCalculation, p => p.ApplicationSettings.BonusCalculation);
            prefs.Add(Preference.FollowLog, p => p.LogWindowProperties.FollowLog);
            prefs.Add(Preference.DisplayEtaAsDate, p => p.MainWindowGridProperties.DisplayEtaAsDate);
            prefs.Add(Preference.LogFileViewer, p => p.ApplicationSettings.LogFileViewer);
            prefs.Add(Preference.FileExplorer, p => p.ApplicationSettings.FileExplorer);
            prefs.Add(Preference.MessageLevel, p => p.ApplicationSettings.MessageLevel);

            prefs.Add(Preference.EmailReportingEnabled, p => p.Email.Enabled);
            prefs.Add(Preference.EmailReportingServerSecure, p => p.Email.SecureConnection);
            prefs.Add(Preference.EmailReportingToAddress, p => p.Email.ToAddress);
            prefs.Add(Preference.EmailReportingFromAddress, p => p.Email.FromAddress);
            prefs.Add(Preference.EmailReportingServerAddress, p => p.Email.SmtpServer.Address);
            prefs.Add(Preference.EmailReportingServerPort, p => p.Email.SmtpServer.Port);
            prefs.Add(Preference.EmailReportingServerUsername, p => p.Email.SmtpServer.Username);
            prefs.AddEncrypted(Preference.EmailReportingServerPassword, p => p.Email.SmtpServer.Password);

            // p => p.Reporting.???

            prefs.Add(Preference.EocUserId, p => p.UserSettings.EocUserId);
            prefs.Add(Preference.StanfordId, p => p.UserSettings.StanfordId);
            prefs.Add(Preference.TeamId, p => p.UserSettings.TeamId);
            prefs.Add(Preference.ProjectDownloadUrl, p => p.ApplicationSettings.ProjectDownloadUrl);
            prefs.Add(Preference.UseProxy, p => p.WebProxy.Enabled);
            prefs.Add(Preference.ProxyServer, p => p.WebProxy.Server.Address);
            prefs.Add(Preference.ProxyPort, p => p.WebProxy.Server.Port);
            prefs.Add(Preference.UseProxyAuth, p => p.WebProxy.CredentialsEnabled);
            prefs.Add(Preference.ProxyUser, p => p.WebProxy.Server.Username);
            prefs.AddEncrypted(Preference.ProxyPass, p => p.WebProxy.Server.Password);

            prefs.Add(Preference.HistoryBonusCalculation, p => p.HistoryWindowProperties.BonusCalculation);
            prefs.Add(Preference.ShowEntriesValue, p => p.HistoryWindowProperties.MaximumResults);
            prefs.Add(Preference.HistorySortColumnName, p => p.HistoryWindowGrid.SortColumn);
            prefs.Add(Preference.HistorySortOrder, p => p.HistoryWindowGrid.SortOrder);
            prefs.Add(Preference.HistoryFormLocation, p => p.HistoryWindow.Location);
            prefs.Add(Preference.HistoryFormSize, p => p.HistoryWindow.Size);
            prefs.Add(Preference.HistoryFormColumns, p => p.HistoryWindowGrid.Columns);

            prefs.AddReadOnly(Preference.CacheFolder, p => p.ApplicationSettings.CacheFolder);

            return prefs;
        }

        public void Save()
        {
            Write(_prefs.Data);
        }

        private PreferenceData Read()
        {
            return OnRead();
        }

        protected virtual PreferenceData OnRead()
        {
            return null;
        }

        protected void Write(PreferenceData data)
        {
            OnWrite(data);
        }

        protected virtual void OnWrite(PreferenceData data)
        {
            // do nothing
        }

        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        public T Get<T>(Preference key)
        {
            var metadata = _prefs[key];
            if (typeof(T) == metadata.DataType)
            {
                if (metadata.Data == null && metadata.DataType == typeof(string))
                {
                    return (T)(object)String.Empty;
                }
                return ((IMetadata<T>)metadata).Data.Copy();
            }
            if (typeof(T).IsAssignableFrom(metadata.DataType))
            {
                return (T)metadata.Data.Copy();
            }
            if (typeof(T).IsEnum && metadata.DataType == typeof(string))
            {
                if (metadata.Data is null)
                {
                    return default(T);
                }

                try
                {
                    return (T)Enum.Parse(typeof(T), ((IMetadata<string>)metadata).Data);
                }
                catch (Exception)
                {
                    return default(T);
                }
            }
            if (typeof(T).IsEnum && metadata.DataType == typeof(int))
            {
                return (T)Enum.ToObject(typeof(T), ((IMetadata<int>)metadata).Data);
            }

            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
        }

        public void Set<T>(Preference key, T value)
        {
            var metadata = _prefs[key];
            if (metadata.DataType == typeof(T))
            {
                if (metadata.Data == null || !metadata.Data.Equals(value))
                {
                    ((IMetadata<T>)metadata).Data = value.Copy();
                    OnPreferenceChanged(key);
                }
            }
            else if (metadata.DataType.IsAssignableFrom(typeof(T)) ||
                     metadata.DataType.CanBeCreatedFrom(typeof(T)))
            {
                if (metadata.Data == null || !metadata.Data.Equals(value))
                {
                    metadata.Data = value.Copy(metadata.DataType);
                    OnPreferenceChanged(key);
                }
            }
            else if (metadata.DataType == typeof(int) && value != null && value.GetType().IsEnum)
            {
                var newValue = (int)Convert.ChangeType(value, typeof(int));

                var intMetadata = (IMetadata<int>)metadata;
                if (!intMetadata.Data.Equals(newValue))
                {
                    intMetadata.Data = newValue;
                    OnPreferenceChanged(key);
                }
            }
            else if (metadata.DataType == typeof(int))
            {
                var stringValue = value as string;
                var newValue = String.IsNullOrEmpty(stringValue) ? default(int) : Int32.Parse(stringValue);

                var intMetadata = (IMetadata<int>)metadata;
                if (!intMetadata.Data.Equals(newValue))
                {
                    // Issue 189 - Use default value if string is null or empty
                    intMetadata.Data = newValue;
                    OnPreferenceChanged(key);
                }
            }
            else if (metadata.DataType == typeof(string) && value.GetType().IsEnum)
            {
                string newValue = value.ToString();

                if (metadata.Data == null || !metadata.Data.Equals(value))
                {
                    metadata.Data = newValue;
                    OnPreferenceChanged(key);
                }
            }
            else
            {
                throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
                   "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
            }
        }

        public event EventHandler<PreferenceChangedEventArgs> PreferenceChanged;

        protected virtual void OnPreferenceChanged(Preference preference)
        {
            PreferenceChanged?.Invoke(this, new PreferenceChangedEventArgs(preference));
        }

        private class PreferenceDictionary
        {
            public PreferenceData Data { get; }

            private readonly Dictionary<Preference, IMetadata> _inner;

            public PreferenceDictionary(PreferenceData data)
            {
                Data = data;
                _inner = new Dictionary<Preference, IMetadata>();
            }

            public void Add<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression)
            {
                _inner.Add(key, new ExpressionMetadata<T>(Data, propertyExpression));
            }

            public void AddReadOnly<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression)
            {
                _inner.Add(key, new ExpressionMetadata<T>(Data, propertyExpression, true));
            }

            public void AddEncrypted(Preference key, Expression<Func<PreferenceData, string>> propertyExpression)
            {
                _inner.Add(key, new EncryptedExpressionMetadata(Data, propertyExpression));
            }

            public IMetadata this[Preference key] => _inner[key];
        }
    }
}
