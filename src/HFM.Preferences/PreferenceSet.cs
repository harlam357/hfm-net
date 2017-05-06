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
using System.Linq.Expressions;
using System.Runtime.Serialization;
using System.Xml;

using HFM.Preferences.Data;

namespace HFM.Preferences
{
   public sealed class PreferenceSet : IPreferenceSet
   {
      #region Implementation

      public string ApplicationPath { get; private set; }

      public string ApplicationDataFolderPath { get; private set; }

      public string CacheDirectory
      {
         get { return Path.Combine(ApplicationDataFolderPath, Get<string>(Preference.CacheFolder)); }
      }

      private PreferenceDictionary _prefs;
      private readonly string _versionString;

      internal PreferenceSet()
      {
         var data = new PreferenceData();
         _prefs = CreateDictionary(data);
      }

      public PreferenceSet(string applicationPath, string applicationDataFolderPath, string versionString)
      {
         if (String.IsNullOrWhiteSpace(applicationPath)) throw new ArgumentException("Value cannot be null or whitespace.", "applicationPath");
         if (String.IsNullOrWhiteSpace(applicationDataFolderPath)) throw new ArgumentException("Value cannot be null or whitespace.", "applicationDataFolderPath");
         if (String.IsNullOrWhiteSpace(versionString)) throw new ArgumentException("Value cannot be null or whitespace.", "versionString");
         ApplicationPath = applicationPath;
         ApplicationDataFolderPath = applicationDataFolderPath;
         _versionString = versionString;

         var data = new PreferenceData();
         _prefs = CreateDictionary(data);
      }

      public void Reset()
      {
         EnsureApplicationDataFolderExists();
         InitializeInternal(new PreferenceData());
      }

      public void Initialize()
      {
         EnsureApplicationDataFolderExists();
         var data = Read() ?? Migrate() ?? new PreferenceData();
         InitializeInternal(data);
      }

      private void EnsureApplicationDataFolderExists()
      {
         if (!Directory.Exists(ApplicationDataFolderPath))
         {
            Directory.CreateDirectory(ApplicationDataFolderPath);
         }
      }

      private void InitializeInternal(PreferenceData data)
      {
         Upgrade(data);
         _prefs = CreateDictionary(data);
      }

      private PreferenceData Migrate()
      {
         try
         {
            var data = MigrateFromUserSettings.Execute();
            if (data != null)
            {
               Write(data);
            }
            return data;
         }
         catch (Exception)
         {
            return null;
         }
      }

      private void Upgrade(PreferenceData data)
      {
         if (data.ApplicationVersion != _versionString)
         {
            Version settingsVersion;
            if (Version.TryParse(data.ApplicationVersion, out settingsVersion))
            {
               ExecuteVersionUpgrades(settingsVersion, data);
            }
            data.ApplicationVersion = _versionString;
            Write(data);
         }
      }

      private static void ExecuteVersionUpgrades(Version settingsVersion, PreferenceData data)
      {
         foreach (var upgrade in CreateVersionUpgrades().Where(upgrade => settingsVersion < upgrade.Version))
         {
            upgrade.Action(data);
         }
      }

      private static IEnumerable<VersionUpgrade> CreateVersionUpgrades()
      {
         return new[]
         {
            new VersionUpgrade
            {
               Version = new Version(0, 9, 5),
               Action = data => data.ApplicationSettings.ProjectDownloadUrl = "http://assign.stanford.edu/api/project/summary"
            }
         };
      }

      private sealed class VersionUpgrade
      {
         public Version Version { get; set; }

         public Action<PreferenceData> Action { get; set; }
      }

      private static PreferenceDictionary CreateDictionary(PreferenceData data)
      {
         var prefs = new PreferenceDictionary(data);

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
         prefs.Add(Preference.BenchmarksGraphLayoutType, p => p.BenchmarksGraphing.GraphLayout);
         prefs.Add(Preference.BenchmarksClientsPerGraph, p => p.BenchmarksGraphing.ClientsPerGraph);

         prefs.Add(Preference.MessagesFormLocation, p => p.MessagesWindow.Location);
         prefs.Add(Preference.MessagesFormSize, p => p.MessagesWindow.Size);

         prefs.Add(Preference.ClientRetrievalTask, p => p.ClientRetrievalTask);
         prefs.AddReadOnly(Preference.ClientRetrievalTaskEnabled, p => p.ClientRetrievalTask.Enabled);
         prefs.AddReadOnly(Preference.ClientRetrievalTaskInterval, p => p.ClientRetrievalTask.Interval);
         prefs.AddReadOnly(Preference.ClientRetrievalTaskType, p => p.ClientRetrievalTask.ProcessingMode);

         prefs.Add(Preference.DuplicateUserIdCheck, p => p.ApplicationSettings.DuplicateUserIdCheck);
         prefs.Add(Preference.DuplicateProjectCheck, p => p.ApplicationSettings.DuplicateProjectCheck);
         prefs.Add(Preference.AllowRunningAsync, p => p.ApplicationSettings.AllowRunningAsync);

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
         prefs.Add(Preference.WebMobileOverview, p => p.WebRendering.MobileOverviewTransform);
         prefs.Add(Preference.WebSummary, p => p.WebRendering.SummaryTransform);
         prefs.Add(Preference.WebMobileSummary, p => p.WebRendering.MobileSummaryTransform);
         prefs.Add(Preference.WebSlot, p => p.WebRendering.SlotTransform);

         prefs.Add(Preference.RunMinimized, p => p.Startup.RunMinimized);
         prefs.Add(Preference.StartupCheckForUpdate, p => p.Startup.CheckForUpdate);
         prefs.Add(Preference.UseDefaultConfigFile, p => p.Startup.DefaultConfigFileEnabled);
         prefs.Add(Preference.DefaultConfigFile, p => p.Startup.DefaultConfigFilePath);

         prefs.Add(Preference.OfflineLast, p => p.MainWindowGridProperties.OfflineClientsLast);
         prefs.Add(Preference.ColorLogFile, p => p.LogWindowProperties.ApplyColor);
         prefs.Add(Preference.AutoSaveConfig, p => p.ApplicationSettings.AutoSaveConfig);
         prefs.Add(Preference.PpdCalculation, p => p.ApplicationSettings.PpdCalculation);
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
         prefs.Add(Preference.ReportEuePause, p => p.Reporting.EuePauseEnabled);
         prefs.Add(Preference.ReportHung, p => p.Reporting.ClientHungEnabled);

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
         string path = Path.Combine(ApplicationDataFolderPath, "config.xml");
         if (File.Exists(path))
         {
            using (var fileStream = new FileStream(path, FileMode.Open, FileAccess.Read))
            {
               var serializer = new DataContractSerializer(typeof(PreferenceData));
               var data = (PreferenceData)serializer.ReadObject(fileStream);
               // validate interval properties
               data.ClientRetrievalTask.Interval = Validation.GetValidInterval(data.ClientRetrievalTask.Interval);
               data.WebGenerationTask.Interval = Validation.GetValidInterval(data.WebGenerationTask.Interval);
               data.ApplicationSettings.MessageLevel = Validation.GetValidMessageLevel(data.ApplicationSettings.MessageLevel);
               return data;
            }
         }
         return null;
      }

      private void Write(PreferenceData data)
      {
         string path = Path.Combine(ApplicationDataFolderPath, "config.xml");
         using (var fileStream = new FileStream(path, FileMode.Create, FileAccess.Write))
         using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
         {
            var serializer = new DataContractSerializer(typeof(PreferenceData));
            serializer.WriteObject(xmlWriter, data);
         }
      }

      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      public T Get<T>(Preference key)
      {
         var metadata = _prefs[key];
         if (metadata.Data == null)
         {
            if (metadata.DataType == typeof(string))
            {
               return (T)(object)String.Empty;
            }
            return (T)metadata.Data;
         }
         if (metadata.DataType == typeof(T))
         {
            return ((IMetadata<T>)metadata).Data.Copy();
         }
         if (typeof(T).IsAssignableFrom(metadata.DataType))
         {
            return (T)metadata.Data.Copy();
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
         else
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
         }
      }

      public event EventHandler<PreferenceChangedEventArgs> PreferenceChanged;

      private void OnPreferenceChanged(Preference preference)
      {
         var handler = PreferenceChanged;
         if (handler != null)
         {
            handler(this, new PreferenceChangedEventArgs(preference));
         }
      }

      #endregion

      private sealed class PreferenceDictionary
      {
         public PreferenceData Data
         {
            get { return _data; }
         }

         private readonly PreferenceData _data;
         private readonly Dictionary<Preference, IMetadata> _inner;

         public PreferenceDictionary(PreferenceData data)
         {
            _data = data;
            _inner = new Dictionary<Preference, IMetadata>();
         }

         public void Add<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression)
         {
            _inner.Add(key, new ExpressionMetadata<T>(_data, propertyExpression));
         }

         public void AddReadOnly<T>(Preference key, Expression<Func<PreferenceData, T>> propertyExpression)
         {
            _inner.Add(key, new ExpressionMetadata<T>(_data, propertyExpression, true));
         }

         public void AddEncrypted(Preference key, Expression<Func<PreferenceData, string>> propertyExpression)
         {
            _inner.Add(key, new EncryptedExpressionMetadata(_data, propertyExpression));
         }

         public IMetadata this[Preference key]
         {
            get { return _inner[key]; }
         }
      }
   }
}
