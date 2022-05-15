using System.Globalization;
using System.Linq.Expressions;

using HFM.Preferences.Data;
using HFM.Preferences.Internal;

namespace HFM.Preferences;

public abstract class PreferencesProvider : IPreferences
{
    public string ApplicationPath { get; }

    public string ApplicationDataFolderPath { get; }

    public string ApplicationVersion { get; }

    private PreferenceDictionary _preferences;

    protected PreferencesProvider(string applicationPath, string applicationDataFolderPath, string applicationVersion)
    {
        ApplicationPath = applicationPath;
        ApplicationDataFolderPath = applicationDataFolderPath;
        ApplicationVersion = applicationVersion;

        var data = new PreferenceData { ApplicationVersion = ApplicationVersion };
        _preferences = CreateDictionary(data);
    }

    public void Reset()
    {
        var data = new PreferenceData { ApplicationVersion = ApplicationVersion };
        Write(data);
        _preferences = CreateDictionary(data);
    }

    public void Load()
    {
        var data = Read() ?? new PreferenceData();
        Load(data);
    }

    public void Load(PreferenceData data)
    {
        Upgrade(data);
        _preferences = CreateDictionary(data);
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

    private IEnumerable<PreferenceUpgrade> EnumerateUpgrades() => OnEnumerateUpgrades();

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
        var d = new PreferenceDictionary(data);

        d.AddReadOnly(Preference.ApplicationPath, p => ApplicationPath);
        d.AddReadOnly(Preference.ApplicationDataFolderPath, p => ApplicationDataFolderPath);
        d.AddReadOnly(Preference.ApplicationVersion, p => p.ApplicationVersion);
        d.AddReadOnly(Preference.CacheDirectory, p => Path.Combine(ApplicationDataFolderPath, p.ApplicationSettings.CacheFolder));

        d.Add(Preference.FormLocation, p => p.MainWindow.Location);
        d.Add(Preference.FormSize, p => p.MainWindow.Size);
        d.Add(Preference.FormColumns, p => p.MainWindowGrid.Columns);
        d.Add(Preference.FormSortColumn, p => p.MainWindowGrid.SortColumn);
        d.Add(Preference.FormSortOrder, p => p.MainWindowGrid.SortOrder);
        d.Add(Preference.FormSplitterLocation, p => p.MainWindowState.SplitterLocation);
        d.Add(Preference.FormLogWindowHeight, p => p.MainWindowState.LogWindowHeight);
        d.Add(Preference.FormLogWindowVisible, p => p.MainWindowState.LogWindowVisible);
        d.Add(Preference.QueueWindowVisible, p => p.MainWindowState.QueueWindowVisible);
        d.Add(Preference.QueueSplitterLocation, p => p.MainWindowState.QueueSplitterLocation);
        d.Add(Preference.TimeFormatting, p => p.MainWindowGridProperties.TimeFormatting);
        d.Add(Preference.UnitTotals, p => p.MainWindowGridProperties.UnitTotals);
        d.Add(Preference.DisplayVersions, p => p.MainWindowGridProperties.DisplayVersions);

        d.Add(Preference.MinimizeTo, p => p.MainWindowProperties.MinimizeTo);
        d.Add(Preference.EnableUserStats, p => p.MainWindowProperties.EnableStats);
        d.Add(Preference.UserStatsType, p => p.MainWindowProperties.StatsType);

        d.Add(Preference.BenchmarksFormLocation, p => p.BenchmarksWindow.Location);
        d.Add(Preference.BenchmarksFormSize, p => p.BenchmarksWindow.Size);
        d.Add(Preference.GraphColors, p => p.BenchmarksGraphing.GraphColors);

        d.Add(Preference.MessagesFormLocation, p => p.MessagesWindow.Location);
        d.Add(Preference.MessagesFormSize, p => p.MessagesWindow.Size);

        d.Add(Preference.ClientRetrievalTask, p => p.ClientRetrievalTask);
        d.AddReadOnly(Preference.ClientRetrievalTaskEnabled, p => p.ClientRetrievalTask.Enabled);
        d.AddReadOnly(Preference.ClientRetrievalTaskInterval, p => p.ClientRetrievalTask.Interval);
        d.AddReadOnly(Preference.ClientRetrievalTaskType, p => p.ClientRetrievalTask.ProcessingMode);

        d.Add(Preference.DuplicateProjectCheck, p => p.ApplicationSettings.DuplicateProjectCheck);

        d.Add(Preference.WebGenerationTask, p => p.WebGenerationTask);
        d.AddReadOnly(Preference.WebGenerationTaskEnabled, p => p.WebGenerationTask.Enabled);
        d.AddReadOnly(Preference.WebGenerationTaskInterval, p => p.WebGenerationTask.Interval);
        d.AddReadOnly(Preference.WebGenerationTaskAfterClientRetrieval, p => p.WebGenerationTask.AfterClientRetrieval);
        d.Add(Preference.WebDeploymentType, p => p.WebDeployment.DeploymentType);
        d.Add(Preference.WebDeploymentRoot, p => p.WebDeployment.DeploymentRoot);
        d.Add(Preference.WebGenServer, p => p.WebDeployment.FtpServer.Address);
        d.Add(Preference.WebGenPort, p => p.WebDeployment.FtpServer.Port);
        d.Add(Preference.WebGenUsername, p => p.WebDeployment.FtpServer.Username);
        d.AddEncrypted(Preference.WebGenPassword, p => p.WebDeployment.FtpServer.Password);
        d.Add(Preference.WebGenCopyFAHlog, p => p.WebDeployment.CopyLog);
        d.Add(Preference.WebGenFtpMode, p => p.WebDeployment.FtpMode);
        d.Add(Preference.WebGenCopyHtml, p => p.WebDeployment.CopyHtml);
        d.Add(Preference.WebGenCopyXml, p => p.WebDeployment.CopyXml);
        d.Add(Preference.WebGenLimitLogSize, p => p.WebDeployment.LogSizeLimitEnabled);
        d.Add(Preference.WebGenLimitLogSizeLength, p => p.WebDeployment.LogSizeLimitedTo);
        d.Add(Preference.CssFile, p => p.WebRendering.StyleSheet);
        d.Add(Preference.WebOverview, p => p.WebRendering.OverviewTransform);
        d.Add(Preference.WebSummary, p => p.WebRendering.SummaryTransform);
        d.Add(Preference.WebSlot, p => p.WebRendering.SlotTransform);

        d.Add(Preference.RunMinimized, p => p.Startup.RunMinimized);
        d.Add(Preference.StartupCheckForUpdate, p => p.Startup.CheckForUpdate);
        d.Add(Preference.UseDefaultConfigFile, p => p.Startup.DefaultConfigFileEnabled);
        d.Add(Preference.DefaultConfigFile, p => p.Startup.DefaultConfigFilePath);

        d.Add(Preference.OfflineLast, p => p.MainWindowGridProperties.OfflineClientsLast);
        d.Add(Preference.ColorLogFile, p => p.LogWindowProperties.ApplyColor);
        d.Add(Preference.AutoSaveConfig, p => p.ApplicationSettings.AutoSaveConfig);
        d.Add(Preference.PPDCalculation, p => p.ApplicationSettings.PpdCalculation);
        d.Add(Preference.DecimalPlaces, p => p.ApplicationSettings.DecimalPlaces);
        d.Add(Preference.BonusCalculation, p => p.ApplicationSettings.BonusCalculation);
        d.Add(Preference.HideInactiveSlots, p => p.ApplicationSettings.HideInactiveSlots);
        d.Add(Preference.FollowLog, p => p.LogWindowProperties.FollowLog);
        d.Add(Preference.DisplayEtaAsDate, p => p.MainWindowGridProperties.DisplayEtaAsDate);
        d.Add(Preference.LogFileViewer, p => p.ApplicationSettings.LogFileViewer);
        d.Add(Preference.FileExplorer, p => p.ApplicationSettings.FileExplorer);
        d.Add(Preference.MessageLevel, p => p.ApplicationSettings.MessageLevel);

        d.Add(Preference.EmailReportingEnabled, p => p.Email.Enabled);
        d.Add(Preference.EmailReportingServerSecure, p => p.Email.SecureConnection);
        d.Add(Preference.EmailReportingToAddress, p => p.Email.ToAddress);
        d.Add(Preference.EmailReportingFromAddress, p => p.Email.FromAddress);
        d.Add(Preference.EmailReportingServerAddress, p => p.Email.SmtpServer.Address);
        d.Add(Preference.EmailReportingServerPort, p => p.Email.SmtpServer.Port);
        d.Add(Preference.EmailReportingServerUsername, p => p.Email.SmtpServer.Username);
        d.AddEncrypted(Preference.EmailReportingServerPassword, p => p.Email.SmtpServer.Password);

        // p => p.Reporting.???

        d.Add(Preference.EocUserId, p => p.UserSettings.EocUserId);
        d.Add(Preference.StanfordId, p => p.UserSettings.StanfordId);
        d.Add(Preference.TeamId, p => p.UserSettings.TeamId);
        d.Add(Preference.ProjectDownloadUrl, p => p.ApplicationSettings.ProjectDownloadUrl);
        d.Add(Preference.UseProxy, p => p.WebProxy.Enabled);
        d.Add(Preference.ProxyServer, p => p.WebProxy.Server.Address);
        d.Add(Preference.ProxyPort, p => p.WebProxy.Server.Port);
        d.Add(Preference.UseProxyAuth, p => p.WebProxy.CredentialsEnabled);
        d.Add(Preference.ProxyUser, p => p.WebProxy.Server.Username);
        d.AddEncrypted(Preference.ProxyPass, p => p.WebProxy.Server.Password);

        d.Add(Preference.HistoryBonusCalculation, p => p.HistoryWindowProperties.BonusCalculation);
        d.Add(Preference.ShowEntriesValue, p => p.HistoryWindowProperties.MaximumResults);
        d.Add(Preference.HistorySortColumnName, p => p.HistoryWindowGrid.SortColumn);
        d.Add(Preference.HistorySortOrder, p => p.HistoryWindowGrid.SortOrder);
        d.Add(Preference.HistoryFormLocation, p => p.HistoryWindow.Location);
        d.Add(Preference.HistoryFormSize, p => p.HistoryWindow.Size);
        d.Add(Preference.HistoryFormColumns, p => p.HistoryWindowGrid.Columns);

        d.AddReadOnly(Preference.CacheFolder, p => p.ApplicationSettings.CacheFolder);

        return d;
    }

    public void Save() => Write(_preferences.Data);

    private PreferenceData Read() => OnRead();

    protected virtual PreferenceData OnRead() => null;

    protected void Write(PreferenceData data) => OnWrite(data);

    protected virtual void OnWrite(PreferenceData data)
    {
        // do nothing
    }

    public T Get<T>(Preference key)
    {
        var metadata = _preferences[key];
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
        var metadata = _preferences[key];
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

    protected virtual void OnPreferenceChanged(Preference preference) =>
        PreferenceChanged?.Invoke(this, new PreferenceChangedEventArgs(preference));

    private class PreferenceDictionary
    {
        public PreferenceData Data { get; }

        private readonly Dictionary<Preference, IMetadata> _inner;

        public PreferenceDictionary(PreferenceData data)
        {
            Data = data;
            _inner = new Dictionary<Preference, IMetadata>();
        }

        public void Add<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression) =>
            _inner.Add(key, new ExpressionMetadata<T>(Data, propertyExpression));

        public void AddReadOnly<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression) =>
            _inner.Add(key, new ExpressionMetadata<T>(Data, propertyExpression, true));

        public void AddEncrypted(Preference key, Expression<Func<PreferenceData, string>> propertyExpression) =>
            _inner.Add(key, new EncryptedExpressionMetadata(Data, propertyExpression));

        public IMetadata this[Preference key] => _inner[key];
    }
}
