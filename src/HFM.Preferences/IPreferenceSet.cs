
using System;
using System.Diagnostics.CodeAnalysis;

namespace HFM.Preferences
{
    // ReSharper disable InconsistentNaming

    public enum Preference
    {
        FormLocation,
        FormSize,
        FormColumns,
        FormSortColumn,
        FormSortOrder,
        FormSplitterLocation,
        FormLogWindowHeight,
        FormLogWindowVisible,
        QueueWindowVisible,
        TimeFormatting,
        UnitTotals,
        DisplayVersions,
        MinimizeTo,
        BenchmarksFormLocation,
        BenchmarksFormSize,
        GraphColors,
        BenchmarksGraphLayoutType,
        BenchmarksClientsPerGraph,
        MessagesFormLocation,
        MessagesFormSize,
        ClientRetrievalTaskType,
        ClientRetrievalTaskEnabled,
        ClientRetrievalTaskInterval,
        ClientRetrievalTask,
        DuplicateProjectCheck,
        EnableUserStats,
        UserStatsType,
        WebGenerationTask,
        WebGenerationTaskEnabled,
        WebGenerationTaskInterval,
        WebGenerationTaskAfterClientRetrieval,
        WebDeploymentType,
        WebDeploymentRoot,
        WebGenServer,
        WebGenPort,
        WebGenUsername,
        WebGenPassword,
        WebGenCopyFAHlog,
        WebGenFtpMode,
        WebGenCopyHtml,
        WebGenCopyXml,
        WebGenLimitLogSize,
        WebGenLimitLogSizeLength,
        CssFile,
        WebOverview,
        WebSummary,
        WebSlot,
        RunMinimized,
        StartupCheckForUpdate,
        UseDefaultConfigFile,
        DefaultConfigFile,
        OfflineLast,
        ColorLogFile,
        AutoSaveConfig,
        PPDCalculation,
        DecimalPlaces,
        BonusCalculation,
        FollowLog,
        DisplayEtaAsDate,
        LogFileViewer,
        FileExplorer,
        MessageLevel,
        EmailReportingEnabled,
        EmailReportingServerSecure,
        EmailReportingToAddress,
        EmailReportingFromAddress,
        EmailReportingServerAddress,
        EmailReportingServerPort,
        EmailReportingServerUsername,
        EmailReportingServerPassword,
        EocUserId,
        StanfordId,
        TeamId,
        ProjectDownloadUrl,
        UseProxy,
        ProxyServer,
        ProxyPort,
        UseProxyAuth,
        ProxyUser,
        ProxyPass,
        HistoryBonusCalculation,
        ShowEntriesValue,
        HistorySortColumnName,
        HistorySortOrder,
        HistoryFormLocation,
        HistoryFormSize,
        HistoryFormColumns,
        CacheFolder,
        ApplicationPath,
        ApplicationDataFolderPath,
        ApplicationVersion,
        CacheDirectory
    }

    // ReSharper restore InconsistentNaming

    public interface IPreferenceSet
    {
        /// <summary>
        /// Resets the preferences to default values.
        /// </summary>
        void Reset();

        /// <summary>
        /// Loads the preferences from the last saved values.
        /// </summary>
        void Load();

        /// <summary>
        /// Saves the preferences.
        /// </summary>
        void Save();

        /// <summary>
        /// Gets a preference value.
        /// </summary>
        /// <typeparam name="T">The type of the preference value.</typeparam>
        /// <param name="key">The preference key.</param>
        [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
        T Get<T>(Preference key);

        /// <summary>
        /// Sets a preference value.
        /// </summary>
        /// <typeparam name="T">The type of the preference value.</typeparam>
        /// <param name="key">The preference key.</param>
        /// <param name="value">The preference value.</param>
        void Set<T>(Preference key, T value);

        /// <summary>
        /// Raised when a preference value is changed.
        /// </summary>
        event EventHandler<PreferenceChangedEventArgs> PreferenceChanged;
    }

    public class PreferenceChangedEventArgs : EventArgs
    {
        public Preference Preference { get; }

        public PreferenceChangedEventArgs(Preference preference)
        {
            Preference = preference;
        }
    }
}
