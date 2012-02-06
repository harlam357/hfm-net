/*
 * HFM.NET - User Preferences Class
 * Copyright (C) 2006-2007 David Rawling
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
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

using Castle.Core.Logging;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Core;
using HFM.Preferences.Properties;

namespace HFM.Preferences
{
   public sealed class PreferenceSet : IPreferenceSet
   {
      #region Fields

      private readonly Data _iv = new Data("3k1vKL=Cz6!wZS`I");
      private readonly Data _symmetricKey = new Data("%`Bb9ega;$.GUDaf");

      private readonly Dictionary<Preference, IMetadata> _prefs = new Dictionary<Preference, IMetadata>();

      #endregion

      #region Properties

      public string ApplicationPath
      {
         get { return System.Windows.Forms.Application.StartupPath; }
      }
      
      public string ApplicationDataFolderPath
      {
         get { return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ExeName); }
      }

      /// <summary>
      /// Log File Cache Directory
      /// </summary>
      public string CacheDirectory
      {
         get { return Path.Combine(ApplicationDataFolderPath, Get<string>(Preference.CacheFolder)); }
      }
      
      /// <summary>
      /// Url to EOC User Xml File
      /// </summary>
      public string EocUserXml
      {
         get 
         { 
            return String.Concat(Constants.EOCUserXmlUrl, Get<int>(Preference.EocUserId));
         }
      }

      /// <summary>
      /// Url to EOC User Page
      /// </summary>
      public Uri EocUserUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.EOCUserBaseUrl, Get<int>(Preference.EocUserId)));
         }
      }

      /// <summary>
      /// Url to EOC Team Page
      /// </summary>
      public Uri EocTeamUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.EOCTeamBaseUrl, Get<int>(Preference.TeamId)));
         }
      }

      /// <summary>
      /// Url to Stanford User Page
      /// </summary>
      public Uri StanfordUserUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.StanfordBaseUrl, Get<string>(Preference.StanfordId)));
         }
      }

      #endregion

      private ILogger _logger = NullLogger.Instance;

      public ILogger Logger
      {
         [CoverageExclude]
         get { return _logger; }
         [CoverageExclude]
         set { _logger = value; }
      }

      public PreferenceSet()
      {
         Upgrade();
         // 
         SetupDictionary();
         // Issue 176
         Load();
      }

      #region Implementation
      
      public void Reset()
      {
         _logger.Debug("Resetting user preferences...");
         Settings.Default.Reset();
      }

      private static void Upgrade()
      {
         string appVersionString = Application.VersionWithRevision;

         if (Settings.Default.ApplicationVersion != appVersionString)
         {
            Settings.Default.Upgrade();
            Settings.Default.ApplicationVersion = appVersionString;
            Settings.Default.Save();
         }
      }
      
      /// <summary>
      /// Get a Preference of Type T
      /// </summary>
      /// <typeparam name="T">Preference Data Type</typeparam>
      /// <param name="key">Preference Key</param>
      [SuppressMessage("Microsoft.Design", "CA1004:GenericMethodsShouldProvideTypeParameter")]
      public T Get<T>(Preference key)
      {
         if (_prefs[key].DataType == typeof(T))
         {
            return (T) _prefs[key].Data;
         }
         
         throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, 
            "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
      }

      /// <summary>
      /// Set a Preference of Type T
      /// </summary>
      /// <typeparam name="T">Preference Data Type</typeparam>
      /// <param name="key">Preference Key</param>
      /// <param name="value">Preference Value</param>
      public void Set<T>(Preference key, T value)
      {
         if (_prefs[key].DataType == typeof(T))
         {
            ((Metadata<T>)_prefs[key]).Data = value;
            return;
         }
         if (_prefs[key].DataType == typeof(int))
         {
            var stringValue = value as string;
            // Issue 189 - Use default value if string is null or empty
            ((Metadata<int>)_prefs[key]).Data = String.IsNullOrEmpty(stringValue) ? default(int) : Int32.Parse(stringValue);
            return;
         }

         throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
            "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
      }

      private void SetupDictionary()
      {
         //DateTime start = Instrumentation.ExecStart;
      
         _prefs.Add(Preference.FormLocation, new Metadata<Point>());
         _prefs.Add(Preference.FormSize, new Metadata<Size>());
         _prefs.Add(Preference.FormColumns, new Metadata<StringCollection>());
         _prefs.Add(Preference.FormSortColumn, new Metadata<string>());
         _prefs.Add(Preference.FormSortOrder, new Metadata<ListSortDirection>());
         _prefs.Add(Preference.FormSplitLocation, new Metadata<int>());
         _prefs.Add(Preference.FormLogWindowHeight, new Metadata<int>());
         _prefs.Add(Preference.FormLogVisible, new Metadata<bool>());
         _prefs.Add(Preference.QueueViewerVisible, new Metadata<bool>());
         _prefs.Add(Preference.TimeStyle, new Metadata<TimeStyleType>());
         _prefs.Add(Preference.CompletedCountDisplay, new Metadata<CompletedCountDisplayType>());
         _prefs.Add(Preference.ShowVersions, new Metadata<bool>());
         _prefs.Add(Preference.FormShowStyle, new Metadata<FormShowStyleType>());

         _prefs.Add(Preference.BenchmarksFormLocation, new Metadata<Point>());
         _prefs.Add(Preference.BenchmarksFormSize, new Metadata<Size>());
         _prefs.Add(Preference.GraphColors, new Metadata<List<Color>>());
         _prefs.Add(Preference.BenchmarksGraphLayoutType, new Metadata<GraphLayoutType>());
         _prefs.Add(Preference.BenchmarksClientsPerGraph, new Metadata<int>());

         _prefs.Add(Preference.MessagesFormLocation, new Metadata<Point>());
         _prefs.Add(Preference.MessagesFormSize, new Metadata<Size>());

         _prefs.Add(Preference.SyncOnLoad, new Metadata<bool>());
         _prefs.Add(Preference.SyncOnSchedule, new Metadata<bool>());
         _prefs.Add(Preference.SyncTimeMinutes, new Metadata<int>());
         _prefs.Add(Preference.DuplicateUserIdCheck, new Metadata<bool>());
         _prefs.Add(Preference.DuplicateProjectCheck, new Metadata<bool>());
         _prefs.Add(Preference.AllowRunningAsync, new Metadata<bool>());
         _prefs.Add(Preference.ShowXmlStats, new Metadata<bool>());
         _prefs.Add(Preference.ShowTeamStats, new Metadata<bool>());

         _prefs.Add(Preference.GenerateWeb, new Metadata<bool>());
         _prefs.Add(Preference.GenerateInterval, new Metadata<int>());
         _prefs.Add(Preference.WebGenAfterRefresh, new Metadata<bool>());
         _prefs.Add(Preference.WebGenType, new Metadata<WebGenType>());
         _prefs.Add(Preference.WebRoot, new Metadata<string>());
         _prefs.Add(Preference.WebGenServer, new Metadata<string>());
         _prefs.Add(Preference.WebGenPort, new Metadata<int>());
         _prefs.Add(Preference.WebGenUsername, new Metadata<string>());
         _prefs.Add(Preference.WebGenPassword, new Metadata<string>());
         _prefs.Add(Preference.WebGenCopyFAHlog, new Metadata<bool>());
         _prefs.Add(Preference.WebGenFtpMode, new Metadata<FtpType>());
         _prefs.Add(Preference.WebGenCopyHtml, new Metadata<bool>());
         _prefs.Add(Preference.WebGenCopyXml, new Metadata<bool>());
         _prefs.Add(Preference.WebGenLimitLogSize, new Metadata<bool>());
         _prefs.Add(Preference.WebGenLimitLogSizeLength, new Metadata<int>());
         _prefs.Add(Preference.CssFile, new Metadata<string>());
         _prefs.Add(Preference.WebOverview, new Metadata<string>());
         _prefs.Add(Preference.WebMobileOverview, new Metadata<string>());
         _prefs.Add(Preference.WebSummary, new Metadata<string>());
         _prefs.Add(Preference.WebMobileSummary, new Metadata<string>());
         _prefs.Add(Preference.WebSlot, new Metadata<string>());

         _prefs.Add(Preference.RunMinimized, new Metadata<bool>());
         _prefs.Add(Preference.StartupCheckForUpdate, new Metadata<bool>());
         _prefs.Add(Preference.UseDefaultConfigFile, new Metadata<bool>());
         _prefs.Add(Preference.DefaultConfigFile, new Metadata<string>());

         _prefs.Add(Preference.OfflineLast, new Metadata<bool>());
         _prefs.Add(Preference.ColorLogFile, new Metadata<bool>());
         _prefs.Add(Preference.AutoSaveConfig, new Metadata<bool>());
         _prefs.Add(Preference.PpdCalculation, new Metadata<PpdCalculationType>());
         _prefs.Add(Preference.DecimalPlaces, new Metadata<int>());
         _prefs.Add(Preference.CalculateBonus, new Metadata<BonusCalculationType>());
         _prefs.Add(Preference.FollowLogFile, new Metadata<bool>());
         _prefs.Add(Preference.EtaDate, new Metadata<bool>());
         _prefs.Add(Preference.LogFileViewer, new Metadata<string>());
         _prefs.Add(Preference.FileExplorer, new Metadata<string>());
         _prefs.Add(Preference.MessageLevel, new Metadata<int>());

         _prefs.Add(Preference.EmailReportingEnabled, new Metadata<bool>());
         _prefs.Add(Preference.EmailReportingServerSecure, new Metadata<bool>());
         _prefs.Add(Preference.EmailReportingToAddress, new Metadata<string>());
         _prefs.Add(Preference.EmailReportingFromAddress, new Metadata<string>());
         _prefs.Add(Preference.EmailReportingServerAddress, new Metadata<string>());
         _prefs.Add(Preference.EmailReportingServerPort, new Metadata<int>());
         _prefs.Add(Preference.EmailReportingServerUsername, new Metadata<string>());
         _prefs.Add(Preference.EmailReportingServerPassword, new Metadata<string>());
         _prefs.Add(Preference.ReportEuePause, new Metadata<bool>());
         _prefs.Add(Preference.ReportHung, new Metadata<bool>());

         _prefs.Add(Preference.EocUserId, new Metadata<int>());
         _prefs.Add(Preference.StanfordId, new Metadata<string>());
         _prefs.Add(Preference.TeamId, new Metadata<int>());
         _prefs.Add(Preference.ProjectDownloadUrl, new Metadata<string>());
         _prefs.Add(Preference.UseProxy, new Metadata<bool>());
         _prefs.Add(Preference.ProxyServer, new Metadata<string>());
         _prefs.Add(Preference.ProxyPort, new Metadata<int>());
         _prefs.Add(Preference.UseProxyAuth, new Metadata<bool>());
         _prefs.Add(Preference.ProxyUser, new Metadata<string>());
         _prefs.Add(Preference.ProxyPass, new Metadata<string>());

         _prefs.Add(Preference.HistoryProductionType, new Metadata<HistoryProductionView>());
         _prefs.Add(Preference.ShowFirstChecked, new Metadata<bool>());
         _prefs.Add(Preference.ShowLastChecked, new Metadata<bool>());
         _prefs.Add(Preference.ShowEntriesValue, new Metadata<int>());
         _prefs.Add(Preference.HistorySortColumnName, new Metadata<string>());
         _prefs.Add(Preference.HistorySortOrder, new Metadata<ListSortDirection>());
         _prefs.Add(Preference.HistoryFormLocation, new Metadata<Point>());
         _prefs.Add(Preference.HistoryFormSize, new Metadata<Size>());
         _prefs.Add(Preference.HistoryFormColumns, new Metadata<StringCollection>());

         _prefs.Add(Preference.CacheFolder, new Metadata<string>());

         //Debug.WriteLine(String.Format("{0} Execution Time: {1}", Instrumentation.FunctionName, Instrumentation.GetExecTime(start)));
      }

      /// <summary>
      /// Load the Preferences Set
      /// </summary>
      private void Load()
      {
         //DateTime start = Instrumentation.ExecStart;
         var symmetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         var location = new Point();
         var size = new Size();
         StringCollection columns = null;
         GetFormStateValues(ref location, ref size, ref columns);
         Set(Preference.FormLocation, location);
         Set(Preference.FormSize, size);
         Set(Preference.FormColumns, columns);
         Set(Preference.FormSortColumn, Settings.Default.FormSortColumn);
         Set(Preference.FormSortOrder, GetFormSortOrder());
         Set(Preference.FormSplitLocation, Settings.Default.FormSplitLocation);
         Set(Preference.FormLogWindowHeight, Settings.Default.FormLogWindowHeight);
         Set(Preference.FormLogVisible, Settings.Default.FormLogVisible);
         Set(Preference.QueueViewerVisible, Settings.Default.QueueViewerVisible);
         Set(Preference.TimeStyle, GetTimeStyle());
         Set(Preference.CompletedCountDisplay, GetCompletedCountDisplay());
         Set(Preference.ShowVersions, Settings.Default.ShowVersions);
         Set(Preference.FormShowStyle, GetFormShowStyle());

         location = new Point();
         size = new Size();
         GetBenchmarksFormStateValues(ref location, ref size);
         Set(Preference.BenchmarksFormLocation, location);
         Set(Preference.BenchmarksFormSize, size);
         Set(Preference.GraphColors, GetGraphColorsList());
         Set(Preference.BenchmarksGraphLayoutType, GetBenchmarksGraphLayoutType());
         Set(Preference.BenchmarksClientsPerGraph, Settings.Default.BenchmarksClientsPerGraph);

         location = new Point();
         size = new Size();
         GetMessagesFormStateValues(ref location, ref size);
         Set(Preference.MessagesFormLocation, location);
         Set(Preference.MessagesFormSize, size);

         Set(Preference.SyncOnLoad, Settings.Default.SyncOnLoad);
         Set(Preference.SyncOnSchedule, Settings.Default.SyncOnSchedule);
         Set(Preference.SyncTimeMinutes, GetValidNumeric(Settings.Default.SyncTimeMinutes, Constants.MinutesDefault));
         Set(Preference.DuplicateUserIdCheck, Settings.Default.DuplicateUserIDCheck);
         Set(Preference.DuplicateProjectCheck, Settings.Default.DuplicateProjectCheck);
         Set(Preference.AllowRunningAsync, Settings.Default.AllowRunningAsync);
         Set(Preference.ShowXmlStats, Settings.Default.ShowUserStats);
         Set(Preference.ShowTeamStats, Settings.Default.ShowTeamStats);
        
         Set(Preference.GenerateWeb, Settings.Default.GenerateWeb);
         Set(Preference.GenerateInterval, GetValidNumeric(Settings.Default.GenerateInterval, Constants.MinutesDefault));
         Set(Preference.WebGenAfterRefresh, Settings.Default.WebGenAfterRefresh);
         Set(Preference.WebGenType, GetWebGenType());
         Set(Preference.WebRoot, Settings.Default.WebRoot);
         Set(Preference.WebGenServer, Settings.Default.WebGenServer);
         Set(Preference.WebGenPort, Settings.Default.WebGenPort);
         Set(Preference.WebGenUsername, Settings.Default.WebGenUsername);
         Set(Preference.WebGenPassword, DecryptWebGenPassword(Settings.Default.WebGenPassword, symmetricProvider));
         Set(Preference.WebGenCopyFAHlog, Settings.Default.WebGenCopyFAHlog);
         Set(Preference.WebGenFtpMode, GetFtpType());
         Set(Preference.WebGenCopyHtml, Settings.Default.WebGenCopyHtml);
         Set(Preference.WebGenCopyXml, Settings.Default.WebGenCopyXml);
         Set(Preference.WebGenLimitLogSize, Settings.Default.WebGenLimitLogSize);
         Set(Preference.WebGenLimitLogSizeLength, Settings.Default.WebGenLimitLogSizeLength);
         Set(Preference.CssFile, Settings.Default.CSSFile);
         Set(Preference.WebOverview, Settings.Default.WebOverview);
         Set(Preference.WebMobileOverview, Settings.Default.WebMobileOverview);
         Set(Preference.WebSummary, Settings.Default.WebSummary);
         Set(Preference.WebMobileSummary, Settings.Default.WebMobileSummary);
         Set(Preference.WebSlot, Settings.Default.WebSlot);

         Set(Preference.RunMinimized, Settings.Default.RunMinimized);
         Set(Preference.StartupCheckForUpdate, Settings.Default.StartupCheckForUpdate);
         Set(Preference.UseDefaultConfigFile, Settings.Default.UseDefaultConfigFile);
         Set(Preference.DefaultConfigFile, Settings.Default.DefaultConfigFile);

         Set(Preference.OfflineLast, Settings.Default.OfflineLast);
         Set(Preference.ColorLogFile, Settings.Default.ColorLogFile);
         Set(Preference.AutoSaveConfig, Settings.Default.AutoSaveConfig);
         Set(Preference.PpdCalculation, GetPpdCalculation());
         Set(Preference.DecimalPlaces, Settings.Default.DecimalPlaces);
         Set(Preference.CalculateBonus, GetBonusCalculation());
         Set(Preference.FollowLogFile, Settings.Default.FollowLogFile);
         Set(Preference.EtaDate, Settings.Default.EtaDate);
         Set(Preference.LogFileViewer, Settings.Default.LogFileViewer);
         Set(Preference.FileExplorer, Settings.Default.FileExplorer);
         Set(Preference.MessageLevel, Settings.Default.MessageLevel);

         Set(Preference.EmailReportingEnabled, Settings.Default.EmailReportingEnabled);
         Set(Preference.EmailReportingServerSecure, Settings.Default.EmailReportingServerSecure);
         Set(Preference.EmailReportingToAddress, Settings.Default.EmailReportingToAddress);
         Set(Preference.EmailReportingFromAddress, Settings.Default.EmailReportingFromAddress);
         Set(Preference.EmailReportingServerAddress, Settings.Default.EmailReportingServerAddress);
         Set(Preference.EmailReportingServerPort, Settings.Default.EmailReportingServerPort);
         Set(Preference.EmailReportingServerUsername, Settings.Default.EmailReportingServerUsername);
         Set(Preference.EmailReportingServerPassword, DecryptEmailReportingServerPassword(Settings.Default.EmailReportingServerPassword, symmetricProvider));
         Set(Preference.ReportEuePause, Settings.Default.ReportEuePause);
         Set(Preference.ReportHung, Settings.Default.ReportHung);
         
         Set(Preference.EocUserId, Settings.Default.EOCUserID);
         Set(Preference.StanfordId, Settings.Default.StanfordID);
         Set(Preference.TeamId, Settings.Default.TeamID);
         Set(Preference.ProjectDownloadUrl, Settings.Default.ProjectDownloadUrl);
         Set(Preference.UseProxy, Settings.Default.UseProxy);
         Set(Preference.ProxyServer, Settings.Default.ProxyServer);
         Set(Preference.ProxyPort, Settings.Default.ProxyPort);
         Set(Preference.UseProxyAuth, Settings.Default.UseProxyAuth);
         Set(Preference.ProxyUser, Settings.Default.ProxyUser);
         Set(Preference.ProxyPass, DecryptProxyPass(Settings.Default.ProxyPass, symmetricProvider));

         Set(Preference.HistoryProductionType, (HistoryProductionView)Settings.Default.HistoryProductionView);
         Set(Preference.ShowFirstChecked, Settings.Default.ShowFirstChecked);
         Set(Preference.ShowLastChecked, Settings.Default.ShowLastChecked);
         Set(Preference.ShowEntriesValue, Settings.Default.ShowEntriesValue);
         Set(Preference.HistorySortColumnName, Settings.Default.HistorySortColumnName);
         Set(Preference.HistorySortOrder, Settings.Default.HistorySortOrder);
         location = new Point();
         size = new Size();
         columns = null;
         GetHistoryFormStateValues(ref location, ref size, ref columns);
         Set(Preference.HistoryFormLocation, location);
         Set(Preference.HistoryFormSize, size);
         Set(Preference.HistoryFormColumns, columns);

         Set(Preference.CacheFolder, Settings.Default.CacheFolder);

         //Debug.WriteLine(String.Format("{0} Execution Time: {1}", Instrumentation.FunctionName, Instrumentation.GetExecTime(start)));
      }

      #region Load Support Methods
      

      private static void GetFormStateValues(ref Point location, ref Size size, ref StringCollection columns)
      {
         try
         {
            location = Settings.Default.FormLocation;
            size = Settings.Default.FormSize;
            columns = Settings.Default.FormColumns;
         }
         catch (NullReferenceException)
         { }
      }

      private static ListSortDirection GetFormSortOrder()
      {
         ListSortDirection order = ListSortDirection.Ascending;
         try
         {
            order = Settings.Default.FormSortOrder;
         }
         catch (NullReferenceException)
         { }

         return order;
      }

      private static TimeStyleType GetTimeStyle()
      {
         switch (Settings.Default.TimeStyle)
         {
            case "Standard":
               return TimeStyleType.Standard;
            case "Formatted":
               return TimeStyleType.Formatted;
            default:
               return TimeStyleType.Standard;
         }
      }

      private static CompletedCountDisplayType GetCompletedCountDisplay()
      {
         switch (Settings.Default.CompletedCountDisplay)
         {
            case "ClientTotal":
               return CompletedCountDisplayType.ClientTotal;
            case "ClientRunTotal":
               return CompletedCountDisplayType.ClientRunTotal;
            default:
               return CompletedCountDisplayType.ClientRunTotal;
         }
      }

      private static FormShowStyleType GetFormShowStyle()
      {
         switch (Settings.Default.FormShowStyle)
         {
            case "SystemTray":
               return FormShowStyleType.SystemTray;
            case "TaskBar":
               return FormShowStyleType.TaskBar;
            case "Both":
               return FormShowStyleType.Both;
            default:
               return FormShowStyleType.SystemTray;
         }
      }

      private static void GetBenchmarksFormStateValues(ref Point location, ref Size size)
      {
         try
         {
            location = Settings.Default.BenchmarksFormLocation;
            size = Settings.Default.BenchmarksFormSize;
         }
         catch (NullReferenceException)
         { }
      }

      private static List<Color> GetGraphColorsList()
      {
         var graphColors = new List<Color>();
         foreach (string color in Settings.Default.GraphColors)
         {
            Color realColor = Color.FromName(color);
            if (realColor.IsEmpty == false)
            {
               graphColors.Add(realColor);
            }
         }

         return graphColors;
      }

      private static GraphLayoutType GetBenchmarksGraphLayoutType()
      {
         switch (Settings.Default.BenchmarksGraphLayoutType)
         {
            case "Single":
               return GraphLayoutType.Single;
            case "ClientsPerGraph":
               return GraphLayoutType.ClientsPerGraph;
            default:
               return GraphLayoutType.Single;
         }
      }

      private static WebGenType GetWebGenType()
      {
         switch (Settings.Default.WebGenType)
         {
            case "Path":
               return WebGenType.Path;
            case "Ftp":
               return WebGenType.Ftp;
            default:
               return WebGenType.Path;
         }
      }

      private static void GetMessagesFormStateValues(ref Point location, ref Size size)
      {
         try
         {
            location = Settings.Default.MessagesFormLocation;
            size = Settings.Default.MessagesFormSize;
         }
         catch (NullReferenceException)
         { }
      }

      private string DecryptWebGenPassword(string value, Symmetric symmetricProvider)
      {
         Debug.Assert(value != null);
         Debug.Assert(symmetricProvider != null);

         string webGenPassword = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               webGenPassword = symmetricProvider.Decrypt(new Data(Utils.FromBase64(value)), _symmetricKey).ToString();
            }
            catch (FormatException)
            {
               _logger.Warn("Cannot decrypt Web Generation Password... loading clear value.");
               webGenPassword = value;
            }
            catch (CryptographicException)
            {
               _logger.Warn("Cannot decrypt Web Generation Password... loading clear value.");
               webGenPassword = value;
            }
         }

         return webGenPassword;
      }
      
      private static FtpType GetFtpType()
      {
         switch (Settings.Default.WebGenFtpMode)
         {
            case "Passive":
               return FtpType.Passive;
            case "Active":
               return FtpType.Active;
            default:
               return FtpType.Passive;
         }
      }

      private static PpdCalculationType GetPpdCalculation()
      {
         switch (Settings.Default.PpdCalculation)
         {
            case "LastFrame":
               return PpdCalculationType.LastFrame;
            case "LastThreeFrames":
               return PpdCalculationType.LastThreeFrames;
            case "AllFrames":
               return PpdCalculationType.AllFrames;
            case "EffectiveRate":
               return PpdCalculationType.EffectiveRate;
            default:
               return PpdCalculationType.LastThreeFrames;
         }
      }

      private static BonusCalculationType GetBonusCalculation()
      {
         switch (Settings.Default.CalculateBonus)
         {
            case "DownloadTime":
               return BonusCalculationType.DownloadTime;
            case "FrameTime":
               return BonusCalculationType.FrameTime;
            case "None":
               return BonusCalculationType.None;
            default:
               return BonusCalculationType.DownloadTime;
         }
      }

      private string DecryptEmailReportingServerPassword(string value, Symmetric symmetricProvider)
      {
         Debug.Assert(value != null);
         Debug.Assert(symmetricProvider != null);

         string emailReportingServerPassword = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               emailReportingServerPassword = symmetricProvider.Decrypt(new Data(Utils.FromBase64(value)), _symmetricKey).ToString();
            }
            catch (FormatException)
            {
               _logger.Warn("Cannot decrypt Stmp Server Password... loading clear value.");
               emailReportingServerPassword = value;
            }
            catch (CryptographicException)
            {
               _logger.Warn("Cannot decrypt Stmp Server Password... loading clear value.");
               emailReportingServerPassword = value;
            }
         }
         
         return emailReportingServerPassword;
      }

      private string DecryptProxyPass(string value, Symmetric symmetricProvider)
      {
         Debug.Assert(value != null);
         Debug.Assert(symmetricProvider != null);

         string proxyPass = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               proxyPass = symmetricProvider.Decrypt(new Data(Utils.FromBase64(value)), _symmetricKey).ToString();
            }
            catch (FormatException)
            {
               _logger.Warn("Cannot decrypt Proxy Password... loading clear value.");
               proxyPass = value;
            }
            catch (CryptographicException)
            {
               _logger.Warn("Cannot decrypt Proxy Password... loading clear value.");
               proxyPass = value;
            }
         }

         return proxyPass;
      }

      private static void GetHistoryFormStateValues(ref Point location, ref Size size, ref StringCollection columns)
      {
         try
         {
            location = Settings.Default.HistoryFormLocation;
            size = Settings.Default.HistoryFormSize;
            columns = Settings.Default.HistoryFormColumns;
         }
         catch (NullReferenceException)
         { }
      }
      #endregion

      /// <summary>
      /// Revert to the previously saved settings
      /// </summary>
      public void Discard()
      {
         Load();
      }

      /// <summary>
      /// Save the Preferences Set
      /// </summary>
      public void Save()
      {
         //DateTime start = Instrumentation.ExecStart;
         var symmetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         bool raiseFormShowStyleChanged = false;
         bool raiseTimerSettingsChanged = false;
         bool raiseShowUserStatsChanged = false;
         bool raiseOfflineLastChanged = false;
         bool raiseColorLogFileChanged = false;
         bool raisePpdCalculationChanged = false;
         bool raiseDecimalPlacesChanged = false;
         bool raiseCalculateBonusChanged = false;
         bool raiseMessageLevelChanged = false;
         
         try
         {
            Settings.Default.FormLocation = Get<Point>(Preference.FormLocation);
            Settings.Default.FormSize = Get<Size>(Preference.FormSize);
            Settings.Default.FormColumns = Get<StringCollection>(Preference.FormColumns);
            Settings.Default.FormSortColumn = Get<string>(Preference.FormSortColumn);
            Settings.Default.FormSortOrder = Get<ListSortDirection>(Preference.FormSortOrder);
            Settings.Default.FormSplitLocation = Get<int>(Preference.FormSplitLocation);
            Settings.Default.FormLogWindowHeight = Get<int>(Preference.FormLogWindowHeight);
            Settings.Default.FormLogVisible = Get<bool>(Preference.FormLogVisible);
            Settings.Default.QueueViewerVisible = Get<bool>(Preference.QueueViewerVisible);
            Settings.Default.TimeStyle = Get<TimeStyleType>(Preference.TimeStyle).ToString();
            Settings.Default.CompletedCountDisplay = Get<CompletedCountDisplayType>(Preference.CompletedCountDisplay).ToString();
            Settings.Default.ShowVersions = Get<bool>(Preference.ShowVersions);
            if (Settings.Default.FormShowStyle != Get<FormShowStyleType>(Preference.FormShowStyle).ToString())
            {
               raiseFormShowStyleChanged = true;
            }
            Settings.Default.FormShowStyle = Get<FormShowStyleType>(Preference.FormShowStyle).ToString();

            Settings.Default.BenchmarksFormLocation = Get<Point>(Preference.BenchmarksFormLocation);
            Settings.Default.BenchmarksFormSize = Get<Size>(Preference.BenchmarksFormSize);
            Settings.Default.GraphColors = GetGraphColorsStringCollection(Get<List<Color>>(Preference.GraphColors));
            Settings.Default.BenchmarksGraphLayoutType = Get<GraphLayoutType>(Preference.BenchmarksGraphLayoutType).ToString();
            Settings.Default.BenchmarksClientsPerGraph = Get<int>(Preference.BenchmarksClientsPerGraph);

            Settings.Default.MessagesFormLocation = Get<Point>(Preference.MessagesFormLocation);
            Settings.Default.MessagesFormSize = Get<Size>(Preference.MessagesFormSize);

            Settings.Default.SyncOnLoad = Get<bool>(Preference.SyncOnLoad);
            if (Settings.Default.SyncOnSchedule != Get<bool>(Preference.SyncOnSchedule) ||
                Settings.Default.SyncTimeMinutes != Get<int>(Preference.SyncTimeMinutes).ToString())
            {
               raiseTimerSettingsChanged = true;
            }
            Settings.Default.SyncOnSchedule = Get<bool>(Preference.SyncOnSchedule);
            Settings.Default.SyncTimeMinutes = Get<int>(Preference.SyncTimeMinutes).ToString();
            Settings.Default.DuplicateUserIDCheck = Get<bool>(Preference.DuplicateUserIdCheck);
            Settings.Default.DuplicateProjectCheck = Get<bool>(Preference.DuplicateProjectCheck);
            Settings.Default.AllowRunningAsync = Get<bool>(Preference.AllowRunningAsync);
            if (Settings.Default.ShowUserStats != Get<bool>(Preference.ShowXmlStats))
            {
               raiseShowUserStatsChanged = true;
            }
            Settings.Default.ShowUserStats = Get<bool>(Preference.ShowXmlStats);
            Settings.Default.ShowTeamStats = Get<bool>(Preference.ShowTeamStats);

            if (Settings.Default.GenerateWeb != Get<bool>(Preference.GenerateWeb) ||
                Settings.Default.GenerateInterval != Get<int>(Preference.GenerateInterval).ToString() ||
                Settings.Default.WebGenAfterRefresh != Get<bool>(Preference.WebGenAfterRefresh))
            {
               raiseTimerSettingsChanged = true;
            }
            Settings.Default.GenerateWeb = Get<bool>(Preference.GenerateWeb);
            Settings.Default.GenerateInterval = Get<int>(Preference.GenerateInterval).ToString();
            Settings.Default.WebGenAfterRefresh = Get<bool>(Preference.WebGenAfterRefresh);
            Settings.Default.WebGenType = Get<WebGenType>(Preference.WebGenType).ToString();
            Settings.Default.WebRoot = Get<string>(Preference.WebRoot);
            Settings.Default.WebGenServer = Get<string>(Preference.WebGenServer);
            Settings.Default.WebGenPort = Get<int>(Preference.WebGenPort);
            Settings.Default.WebGenUsername = Get<string>(Preference.WebGenUsername);
            Settings.Default.WebGenPassword = EncryptWebGenPassword(Get<string>(Preference.WebGenPassword), symmetricProvider);
            Settings.Default.WebGenCopyFAHlog = Get<bool>(Preference.WebGenCopyFAHlog);
            Settings.Default.WebGenFtpMode = Get<FtpType>(Preference.WebGenFtpMode).ToString();
            Settings.Default.WebGenCopyHtml = Get<bool>(Preference.WebGenCopyHtml);
            Settings.Default.WebGenCopyXml = Get<bool>(Preference.WebGenCopyXml);
            Settings.Default.WebGenLimitLogSize = Get<bool>(Preference.WebGenLimitLogSize);
            Settings.Default.WebGenLimitLogSizeLength = Get<int>(Preference.WebGenLimitLogSizeLength);
            Settings.Default.CSSFile = Get<string>(Preference.CssFile);
            Settings.Default.WebOverview = Get<string>(Preference.WebOverview);
            Settings.Default.WebMobileOverview = Get<string>(Preference.WebMobileOverview);
            Settings.Default.WebSummary = Get<string>(Preference.WebSummary);
            Settings.Default.WebMobileSummary = Get<string>(Preference.WebMobileSummary);
            Settings.Default.WebSlot = Get<string>(Preference.WebSlot);

            Settings.Default.RunMinimized = Get<bool>(Preference.RunMinimized);
            Settings.Default.StartupCheckForUpdate = Get<bool>(Preference.StartupCheckForUpdate);
            Settings.Default.UseDefaultConfigFile = Get<bool>(Preference.UseDefaultConfigFile);
            Settings.Default.DefaultConfigFile = Get<string>(Preference.DefaultConfigFile);
            // if config file name is nothing, automatically set default config to false
            if (Settings.Default.DefaultConfigFile.Length == 0)
            {
               Set(Preference.UseDefaultConfigFile, false);
               Settings.Default.UseDefaultConfigFile = false;
            }

            if (Settings.Default.OfflineLast != Get<bool>(Preference.OfflineLast))
            {
               raiseOfflineLastChanged = true;
            }
            Settings.Default.OfflineLast = Get<bool>(Preference.OfflineLast);
            if (Settings.Default.ColorLogFile != Get<bool>(Preference.ColorLogFile))
            {
               raiseColorLogFileChanged = true;
            }
            Settings.Default.ColorLogFile = Get<bool>(Preference.ColorLogFile);
            Settings.Default.AutoSaveConfig = Get<bool>(Preference.AutoSaveConfig);
            if (Settings.Default.PpdCalculation != Get<PpdCalculationType>(Preference.PpdCalculation).ToString())
            {
               raisePpdCalculationChanged = true;
            }
            Settings.Default.PpdCalculation = Get<PpdCalculationType>(Preference.PpdCalculation).ToString();
            if (Settings.Default.DecimalPlaces != Get<int>(Preference.DecimalPlaces))
            {
               raiseDecimalPlacesChanged = true;
            }
            Settings.Default.DecimalPlaces = Get<int>(Preference.DecimalPlaces);
            if (Settings.Default.CalculateBonus != Get<BonusCalculationType>(Preference.CalculateBonus).ToString())
            {
               raiseCalculateBonusChanged = true;
            }
            Settings.Default.CalculateBonus = Get<BonusCalculationType>(Preference.CalculateBonus).ToString();
            Settings.Default.FollowLogFile = Get<bool>(Preference.FollowLogFile);
            Settings.Default.EtaDate = Get<bool>(Preference.EtaDate);
            Settings.Default.LogFileViewer = Get<string>(Preference.LogFileViewer);
            Settings.Default.FileExplorer = Get<string>(Preference.FileExplorer);
            if (Settings.Default.MessageLevel != Get<int>(Preference.MessageLevel))
            {
               raiseMessageLevelChanged = true;
            }
            Settings.Default.MessageLevel = Get<int>(Preference.MessageLevel);

            Settings.Default.EmailReportingEnabled = Get<bool>(Preference.EmailReportingEnabled);
            Settings.Default.EmailReportingServerSecure = Get<bool>(Preference.EmailReportingServerSecure);
            Settings.Default.EmailReportingToAddress = Get<string>(Preference.EmailReportingToAddress);
            Settings.Default.EmailReportingFromAddress = Get<string>(Preference.EmailReportingFromAddress);
            Settings.Default.EmailReportingServerAddress = Get<string>(Preference.EmailReportingServerAddress);
            Settings.Default.EmailReportingServerPort = Get<int>(Preference.EmailReportingServerPort);
            Settings.Default.EmailReportingServerUsername = Get<string>(Preference.EmailReportingServerUsername);
            Settings.Default.EmailReportingServerPassword = EncryptEmailReportingServerPassword(Get<string>(Preference.EmailReportingServerPassword), symmetricProvider);
            Settings.Default.ReportEuePause = Get<bool>(Preference.ReportEuePause);
            Settings.Default.ReportHung = Get<bool>(Preference.ReportHung);

            Settings.Default.EOCUserID = Get<int>(Preference.EocUserId);
            Settings.Default.StanfordID = Get<string>(Preference.StanfordId);
            Settings.Default.TeamID = Get<int>(Preference.TeamId);
            Settings.Default.ProjectDownloadUrl = Get<string>(Preference.ProjectDownloadUrl);
            Settings.Default.UseProxy = Get<bool>(Preference.UseProxy);
            Settings.Default.ProxyServer = Get<string>(Preference.ProxyServer);
            Settings.Default.ProxyPort = Get<int>(Preference.ProxyPort);
            Settings.Default.UseProxyAuth = Get<bool>(Preference.UseProxyAuth);
            Settings.Default.ProxyUser = Get<string>(Preference.ProxyUser);
            Settings.Default.ProxyPass = EncryptProxyPass(Get<string>(Preference.ProxyPass), symmetricProvider);

            Settings.Default.HistoryProductionView = (int)Get<HistoryProductionView>(Preference.HistoryProductionType);
            Settings.Default.ShowFirstChecked = Get<bool>(Preference.ShowFirstChecked);
            Settings.Default.ShowLastChecked = Get<bool>(Preference.ShowLastChecked);
            Settings.Default.ShowEntriesValue = Get<int>(Preference.ShowEntriesValue);
            Settings.Default.HistorySortColumnName = Get<string>(Preference.HistorySortColumnName);
            Settings.Default.HistorySortOrder = Get<ListSortDirection>(Preference.HistorySortOrder);
            Settings.Default.HistoryFormLocation = Get<Point>(Preference.HistoryFormLocation);
            Settings.Default.HistoryFormSize = Get<Size>(Preference.HistoryFormSize);
            Settings.Default.HistoryFormColumns = Get<StringCollection>(Preference.HistoryFormColumns);
            
            if (raiseFormShowStyleChanged) OnFormShowStyleSettingsChanged(EventArgs.Empty);
            if (raiseTimerSettingsChanged) OnTimerSettingsChanged(EventArgs.Empty);
            if (raiseShowUserStatsChanged) OnShowUserStatsChanged(EventArgs.Empty);
            if (raiseOfflineLastChanged) OnOfflineLastChanged(EventArgs.Empty);
            if (raiseColorLogFileChanged) OnColorLogFileChanged(EventArgs.Empty);
            if (raisePpdCalculationChanged) OnPpdCalculationChanged(EventArgs.Empty);
            if (raiseDecimalPlacesChanged) OnDecimalPlacesChanged(EventArgs.Empty);
            if (raiseCalculateBonusChanged) OnCalculateBonusChanged(EventArgs.Empty);
            if (raiseMessageLevelChanged) OnMessageLevelChanged(EventArgs.Empty);

            Settings.Default.Save();
         }
         catch (Exception ex)
         {
            _logger.ErrorFormat(ex, "{0}", ex.Message);
         }

         //Debug.WriteLine(String.Format("{0} Execution Time: {1}", Instrumentation.FunctionName, Instrumentation.GetExecTime(start)));
      }

      #region Save Support Methods

      private static StringCollection GetGraphColorsStringCollection(IEnumerable<Color> collection)
      {
         var col = new StringCollection();
         foreach (Color color in collection)
         {
            col.Add(color.Name);
         }
         return col;
      }

      private string EncryptWebGenPassword(string value, Symmetric symmetricProvider)
      {
         string webGenPassword = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               webGenPassword = symmetricProvider.Encrypt(new Data(value), _symmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               _logger.Warn("Failed to encrypt Web Generation Password... saving clear value.");
               webGenPassword = value;
            }
         }
         
         return webGenPassword;
      }

      private string EncryptEmailReportingServerPassword(string value, Symmetric symmetricProvider)
      {
         string emailReportingServerPassword = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               emailReportingServerPassword = symmetricProvider.Encrypt(new Data(value), _symmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               _logger.Warn("Failed to encrypt Smtp Server Password... saving clear value.");
               emailReportingServerPassword = value;
            }
         }
         
         return emailReportingServerPassword;
      }

      private string EncryptProxyPass(string value, Symmetric symmetricProvider)
      {
         string proxyPass = String.Empty;
         if (value.Length > 0)
         {
            try
            {
               symmetricProvider.IntializationVector = _iv;
               proxyPass = symmetricProvider.Encrypt(new Data(value), _symmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               _logger.Warn("Failed to encrypt Proxy Password... saving clear value.");
               proxyPass = value;
            }
         }

         return proxyPass;
      }
      
      #endregion
      
      #endregion

      #region Event Wrappers

      /// <summary>
      /// Form Show Style Settings Changed
      /// </summary>
      public event EventHandler FormShowStyleSettingsChanged;
      private void OnFormShowStyleSettingsChanged(EventArgs e)
      {
         if (FormShowStyleSettingsChanged != null)
         {
            FormShowStyleSettingsChanged(this, e);
         }
      }
      
      /// <summary>
      /// Background Timer (Refresh or Web) Settings Changed
      /// </summary>
      public event EventHandler TimerSettingsChanged;
      private void OnTimerSettingsChanged(EventArgs e)
      {
         if (TimerSettingsChanged != null)
         {
            TimerSettingsChanged(this, e);
         }
      }

      /// <summary>
      /// Offline Last Setting Changed
      /// </summary>
      public event EventHandler OfflineLastChanged;
      private void OnOfflineLastChanged(EventArgs e)
      {
         if (OfflineLastChanged != null)
         {
            OfflineLastChanged(this, e);
         }
      }

      /// <summary>
      /// PPD Calculation Type Changed
      /// </summary>
      public event EventHandler PpdCalculationChanged;
      private void OnPpdCalculationChanged(EventArgs e)
      {
         if (PpdCalculationChanged != null)
         {
            PpdCalculationChanged(this, e);
         }
      }

      /// <summary>
      /// Debug Message Level Changed
      /// </summary>
      public event EventHandler MessageLevelChanged;
      private void OnMessageLevelChanged(EventArgs e)
      {
         if (MessageLevelChanged != null)
         {
            MessageLevelChanged(this, e);
         }
      }

      /// <summary>
      /// PPD Decimal Places Setting Changed
      /// </summary>
      public event EventHandler DecimalPlacesChanged;
      private void OnDecimalPlacesChanged(EventArgs e)
      {
         if (DecimalPlacesChanged != null)
         {
            DecimalPlacesChanged(this, e);
         }
      }

      /// <summary>
      /// Show User Statistics Setting Changed
      /// </summary>
      public event EventHandler ShowUserStatsChanged;
      private void OnShowUserStatsChanged(EventArgs e)
      {
         if (ShowUserStatsChanged != null)
         {
            ShowUserStatsChanged(this, e);
         }
      }

      /// <summary>
      /// Color Log File Setting Changed
      /// </summary>
      public event EventHandler ColorLogFileChanged;
      private void OnColorLogFileChanged(EventArgs e)
      {
         if (ColorLogFileChanged != null)
         {
            ColorLogFileChanged(this, e);
         }
      } 

      /// <summary>
      /// Calculate Bonus Credit and PPD Setting Changed
      /// </summary>
      public event EventHandler CalculateBonusChanged;
      private void OnCalculateBonusChanged(EventArgs e)
      {
         if (CalculateBonusChanged != null)
         {
            CalculateBonusChanged(this, e);
         }
      } 

      #endregion

      #region Preference Validation

      private static int GetValidNumeric(string input, int defaultValue)
      {
         int output;
         if (Int32.TryParse(input, out output) == false)
         {
            output = defaultValue;
         }
         else if (Validate.Minutes(output) == false)
         {
            output = defaultValue;
         }
         
         return output;
      }

      #endregion
      
      #region Preference Formatting

      /// <summary>
      /// PPD String Formatter
      /// </summary>
      public string PpdFormatString
      {
         get
         {
            var decimalPlaces = Get<int>(Preference.DecimalPlaces);

            var sbldr = new StringBuilder("###,###,##0");
            if (decimalPlaces > 0)
            {
               sbldr.Append(".");
               for (int i = 0; i < decimalPlaces; i++)
               {
                  sbldr.Append("0");
               }
            }

            return sbldr.ToString();
         }
      }

      #endregion
   }
}
