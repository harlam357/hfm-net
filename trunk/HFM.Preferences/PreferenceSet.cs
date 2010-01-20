/*
 * HFM.NET - User Preferences Class
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Framework;
using HFM.Preferences.Properties;
using HFM.Instrumentation;

namespace HFM.Preferences
{
   public sealed class PreferenceSet : IPreferenceSet
   {
      #region Members
      private readonly Data IV = new Data("3k1vKL=Cz6!wZS`I");
      private readonly Data SymmetricKey = new Data("%`Bb9ega;$.GUDaf");

      private readonly Dictionary<Preference, IMetadata> _Preferences = new Dictionary<Preference, IMetadata>();
      #endregion

      #region Properties
      public string ApplicationPath
      {
         get
         {
            return Path.GetDirectoryName(Application.ExecutablePath);
         }
      }

      /// <summary>
      /// Log File Cache Directory
      /// </summary>
      public string CacheDirectory
      {
         get { return Path.Combine(GetPreference<string>(Preference.ApplicationDataFolderPath), 
                                   GetPreference<string>(Preference.CacheFolder)); }
      }
      
      /// <summary>
      /// Url to EOC User Xml File
      /// </summary>
      public string EocUserXml
      {
         get 
         { 
            return String.Concat(Constants.EOCUserXmlUrl, GetPreference<int>(Preference.EocUserID));
         }
      }

      /// <summary>
      /// Url to EOC User Page
      /// </summary>
      public Uri EocUserUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.EOCUserBaseUrl, GetPreference<int>(Preference.EocUserID)));
         }
      }

      /// <summary>
      /// Url to EOC Team Page
      /// </summary>
      public Uri EocTeamUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.EOCTeamBaseUrl, GetPreference<int>(Preference.TeamID)));
         }
      }

      /// <summary>
      /// Url to Stanford User Page
      /// </summary>
      public Uri StanfordUserUrl
      {
         get
         {
            return new Uri(String.Concat(Constants.StanfordBaseUrl, GetPreference<string>(Preference.StanfordID)));
         }
      }

      #endregion

      #region Constructor
      /// <summary>
      /// Construct a PreferenceSet Instance
      /// </summary>
      public PreferenceSet()
      {
         SetupDictionary();
         Load();
      } 
      #endregion

      #region Implementation
      /// <summary>
      /// Get a Preference of Type T
      /// </summary>
      /// <typeparam name="T">Preference Data Type</typeparam>
      /// <param name="key">Preference Key</param>
      public T GetPreference<T>(Preference key)
      {
         if (_Preferences[key].DataType == typeof(T))
         {
            return (T) _Preferences[key].Data;
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
      public void SetPreference<T>(Preference key, T value)
      {
         if (_Preferences[key].DataType == typeof(T))
         {
            ((Metadata<T>)_Preferences[key]).Data = value;
            return;
         }
         else if (_Preferences[key].DataType == typeof(int))
         {
            string stringValue = value as string;
            if (stringValue != null)
            {
               ((Metadata<int>)_Preferences[key]).Data = Int32.Parse(stringValue);
               return;
            }
         }

         throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
            "Preference '{0}' of Type '{1}' does not exist.", key, typeof(T)));
      }

      private void SetupDictionary()
      {
         DateTime Start = HfmTrace.ExecStart;
      
         _Preferences.Add(Preference.FormLocation, new Metadata<Point>());
         _Preferences.Add(Preference.FormSize, new Metadata<Size>());
         _Preferences.Add(Preference.FormColumns, new Metadata<StringCollection>());
         _Preferences.Add(Preference.FormSortColumn, new Metadata<string>());
         _Preferences.Add(Preference.FormSortOrder, new Metadata<SortOrder>());
         _Preferences.Add(Preference.FormSplitLocation, new Metadata<int>());
         _Preferences.Add(Preference.FormLogWindowHeight, new Metadata<int>());
         _Preferences.Add(Preference.FormLogVisible, new Metadata<bool>());
         _Preferences.Add(Preference.QueueViewerVisible, new Metadata<bool>());
         _Preferences.Add(Preference.TimeStyle, new Metadata<TimeStyleType>());

         _Preferences.Add(Preference.BenchmarksFormLocation, new Metadata<Point>());
         _Preferences.Add(Preference.BenchmarksFormSize, new Metadata<Size>());
         _Preferences.Add(Preference.GraphColors, new Metadata<List<Color>>());

         _Preferences.Add(Preference.MessagesFormLocation, new Metadata<Point>());
         _Preferences.Add(Preference.MessagesFormSize, new Metadata<Size>());

         _Preferences.Add(Preference.SyncOnLoad, new Metadata<bool>());
         _Preferences.Add(Preference.SyncOnSchedule, new Metadata<bool>());
         _Preferences.Add(Preference.SyncTimeMinutes, new Metadata<int>());
         _Preferences.Add(Preference.DuplicateUserIDCheck, new Metadata<bool>());
         _Preferences.Add(Preference.DuplicateProjectCheck, new Metadata<bool>());
         _Preferences.Add(Preference.AllowRunningAsync, new Metadata<bool>());
         _Preferences.Add(Preference.ShowUserStats, new Metadata<bool>());

         _Preferences.Add(Preference.GenerateWeb, new Metadata<bool>());
         _Preferences.Add(Preference.GenerateInterval, new Metadata<int>());
         _Preferences.Add(Preference.WebGenAfterRefresh, new Metadata<bool>());
         _Preferences.Add(Preference.WebRoot, new Metadata<string>());
         _Preferences.Add(Preference.WebGenCopyFAHlog, new Metadata<bool>());
         _Preferences.Add(Preference.CssFile, new Metadata<string>());

         _Preferences.Add(Preference.RunMinimized, new Metadata<bool>());
         _Preferences.Add(Preference.UseDefaultConfigFile, new Metadata<bool>());
         _Preferences.Add(Preference.DefaultConfigFile, new Metadata<string>());

         _Preferences.Add(Preference.OfflineLast, new Metadata<bool>());
         _Preferences.Add(Preference.ColorLogFile, new Metadata<bool>());
         _Preferences.Add(Preference.AutoSaveConfig, new Metadata<bool>());
         _Preferences.Add(Preference.PpdCalculation, new Metadata<PpdCalculationType>());
         _Preferences.Add(Preference.DecimalPlaces, new Metadata<int>());
         _Preferences.Add(Preference.CalculateBonus, new Metadata<bool>());
         _Preferences.Add(Preference.LogFileViewer, new Metadata<string>());
         _Preferences.Add(Preference.FileExplorer, new Metadata<string>());
         _Preferences.Add(Preference.MessageLevel, new Metadata<int>());

         _Preferences.Add(Preference.EmailReportingEnabled, new Metadata<bool>());
         _Preferences.Add(Preference.EmailReportingToAddress, new Metadata<string>());
         _Preferences.Add(Preference.EmailReportingFromAddress, new Metadata<string>());
         _Preferences.Add(Preference.EmailReportingServerAddress, new Metadata<string>());
         _Preferences.Add(Preference.EmailReportingServerUsername, new Metadata<string>());
         _Preferences.Add(Preference.EmailReportingServerPassword, new Metadata<string>());
         _Preferences.Add(Preference.ReportEuePause, new Metadata<bool>());

         _Preferences.Add(Preference.EocUserID, new Metadata<int>());
         _Preferences.Add(Preference.StanfordID, new Metadata<string>());
         _Preferences.Add(Preference.TeamID, new Metadata<int>());
         _Preferences.Add(Preference.ProjectDownloadUrl, new Metadata<string>());
         _Preferences.Add(Preference.UseProxy, new Metadata<bool>());
         _Preferences.Add(Preference.ProxyServer, new Metadata<string>());
         _Preferences.Add(Preference.ProxyPort, new Metadata<int>());
         _Preferences.Add(Preference.UseProxyAuth, new Metadata<bool>());
         _Preferences.Add(Preference.ProxyUser, new Metadata<string>());
         _Preferences.Add(Preference.ProxyPass, new Metadata<string>());

         _Preferences.Add(Preference.CacheFolder, new Metadata<string>(Settings.Default.CacheFolder));
         _Preferences.Add(Preference.ApplicationDataFolderPath, new Metadata<string>(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.ExeName)));

         Debug.WriteLine(String.Format("{0} Execution Time: {1}", HfmTrace.FunctionName, HfmTrace.GetExecTime(Start)));
      }

      /// <summary>
      /// Load the Preferences Set
      /// </summary>
      public void Load()
      {
         DateTime Start = HfmTrace.ExecStart;
         Symmetric SymmetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         UpgradeUserSettings();

         Point location = new Point();
         Size size = new Size();
         StringCollection columns = null;
         GetFormStateValues(ref location, ref size, ref columns);
         SetPreference(Preference.FormLocation, location);
         SetPreference(Preference.FormSize, size);
         SetPreference(Preference.FormColumns, columns);
         SetPreference(Preference.FormSortColumn, Settings.Default.FormSortColumn);
         SetPreference(Preference.FormSortOrder, GetFormSortOrder());
         SetPreference(Preference.FormSplitLocation, Settings.Default.FormSplitLocation);
         SetPreference(Preference.FormLogWindowHeight, Settings.Default.FormLogWindowHeight);
         SetPreference(Preference.FormLogVisible, Settings.Default.FormLogVisible);
         SetPreference(Preference.QueueViewerVisible, Settings.Default.QueueViewerVisible);
         SetPreference(Preference.TimeStyle, GetTimeStyle());

         location = new Point();
         size = new Size();
         GetBenchmarksFormStateValues(ref location, ref size);
         SetPreference(Preference.BenchmarksFormLocation, location);
         SetPreference(Preference.BenchmarksFormSize, size);
         SetPreference(Preference.GraphColors, GetGraphColorsList());

         location = new Point();
         size = new Size();
         GetMessagesFormStateValues(ref location, ref size);
         SetPreference(Preference.MessagesFormLocation, location);
         SetPreference(Preference.MessagesFormSize, size);

         SetPreference(Preference.SyncOnLoad, Settings.Default.SyncOnLoad);
         SetPreference(Preference.SyncOnSchedule, Settings.Default.SyncOnSchedule);
         SetPreference(Preference.SyncTimeMinutes, GetValidNumeric(Settings.Default.SyncTimeMinutes, Constants.MinutesDefault));
         SetPreference(Preference.DuplicateUserIDCheck, Settings.Default.DuplicateUserIDCheck);
         SetPreference(Preference.DuplicateProjectCheck, Settings.Default.DuplicateProjectCheck);
         SetPreference(Preference.AllowRunningAsync, Settings.Default.AllowRunningAsync);
         SetPreference(Preference.ShowUserStats, Settings.Default.ShowUserStats);
        
         SetPreference(Preference.GenerateWeb, Settings.Default.GenerateWeb);
         SetPreference(Preference.GenerateInterval, GetValidNumeric(Settings.Default.GenerateInterval, Constants.MinutesDefault));
         SetPreference(Preference.WebGenAfterRefresh, Settings.Default.WebGenAfterRefresh);
         SetPreference(Preference.WebRoot, DecryptWebRoot(Settings.Default.WebRoot, SymmetricProvider, IV, SymmetricKey));
         SetPreference(Preference.WebGenCopyFAHlog, Settings.Default.WebGenCopyFAHlog);
         SetPreference(Preference.CssFile, Settings.Default.CSSFile);

         SetPreference(Preference.RunMinimized, Settings.Default.RunMinimized);
         SetPreference(Preference.UseDefaultConfigFile, Settings.Default.UseDefaultConfigFile);
         SetPreference(Preference.DefaultConfigFile, Settings.Default.DefaultConfigFile);

         SetPreference(Preference.OfflineLast, Settings.Default.OfflineLast);
         SetPreference(Preference.ColorLogFile, Settings.Default.ColorLogFile);
         SetPreference(Preference.AutoSaveConfig, Settings.Default.AutoSaveConfig);
         SetPreference(Preference.PpdCalculation, GetPpdCalculation());
         SetPreference(Preference.DecimalPlaces, Settings.Default.DecimalPlaces);
         SetPreference(Preference.CalculateBonus, Settings.Default.CalculateBonus);
         SetPreference(Preference.LogFileViewer, Settings.Default.LogFileViewer);
         SetPreference(Preference.FileExplorer, Settings.Default.FileExplorer);
         SetPreference(Preference.MessageLevel, Settings.Default.MessageLevel);

         SetPreference(Preference.EmailReportingEnabled, Settings.Default.EmailReportingEnabled);
         SetPreference(Preference.EmailReportingToAddress, Settings.Default.EmailReportingToAddress);
         SetPreference(Preference.EmailReportingFromAddress, Settings.Default.EmailReportingFromAddress);
         SetPreference(Preference.EmailReportingServerAddress, Settings.Default.EmailReportingServerAddress);
         SetPreference(Preference.EmailReportingServerUsername, Settings.Default.EmailReportingServerUsername);
         SetPreference(Preference.EmailReportingServerPassword, DecryptEmailReportingServerPassword(Settings.Default.EmailReportingServerPassword, SymmetricProvider, IV, SymmetricKey));
         SetPreference(Preference.ReportEuePause, Settings.Default.ReportEuePause);
         
         SetPreference(Preference.EocUserID, Settings.Default.EOCUserID);
         SetPreference(Preference.StanfordID, Settings.Default.StanfordID);
         SetPreference(Preference.TeamID, Settings.Default.TeamID);
         SetPreference(Preference.ProjectDownloadUrl, Settings.Default.ProjectDownloadUrl);
         SetPreference(Preference.UseProxy, Settings.Default.UseProxy);
         SetPreference(Preference.ProxyServer, Settings.Default.ProxyServer);
         SetPreference(Preference.ProxyPort, Settings.Default.ProxyPort);
         SetPreference(Preference.UseProxyAuth, Settings.Default.UseProxyAuth);
         SetPreference(Preference.ProxyUser, Settings.Default.ProxyUser);
         SetPreference(Preference.ProxyPass, DecryptProxyPass(Settings.Default.ProxyPass, SymmetricProvider, IV, SymmetricKey));
         
         Debug.WriteLine(String.Format("{0} Execution Time: {1}", HfmTrace.FunctionName, HfmTrace.GetExecTime(Start)));
      }

      #region Load Support Methods
      private static void UpgradeUserSettings()
      {
         string appVersionString = PlatformOps.ApplicationVersionWithRevision;

         if (Settings.Default.ApplicationVersion != appVersionString)
         {
            Settings.Default.Upgrade();
            Settings.Default.ApplicationVersion = appVersionString;
            Settings.Default.Save();
         }
      }

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

      private static SortOrder GetFormSortOrder()
      {
         SortOrder order = SortOrder.None;
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
         List<Color> GraphColors = new List<Color>();
         foreach (string color in Settings.Default.GraphColors)
         {
            Color realColor = Color.FromName(color);
            if (realColor.IsEmpty == false)
            {
               GraphColors.Add(realColor);
            }
         }

         return GraphColors;
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

      private static string DecryptWebRoot(string encrypted, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string WebRoot = String.Empty;
         if (Settings.Default.WebRoot.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               WebRoot = SymmetricProvider.Decrypt(new Data(Utils.FromBase64(encrypted)), SymmetricKey).ToString();
            }
            catch (FormatException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "WebGen Root Folder is not Base64 encoded... loading clear value.", true);
               WebRoot = Settings.Default.WebRoot;
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt WebGen Root Folder... loading clear value.", true);
               WebRoot = Settings.Default.WebRoot;
            }
         }

         return WebRoot;
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

      private static string DecryptEmailReportingServerPassword(string encrypted, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string EmailReportingServerPassword = String.Empty;
         if (Settings.Default.EmailReportingServerPassword.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               EmailReportingServerPassword =
                  SymmetricProvider.Decrypt(new Data(Utils.FromBase64(encrypted)), SymmetricKey).ToString();
            }
            catch (FormatException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Stmp Server Password is not Base64 encoded... loading clear value.", true);
               EmailReportingServerPassword = Settings.Default.EmailReportingServerPassword;
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt Stmp Server Password... loading clear value.", true);
               EmailReportingServerPassword = Settings.Default.EmailReportingServerPassword;
            }
         }
         
         return EmailReportingServerPassword;
      }

      private static string DecryptProxyPass(string encrypted, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string ProxyPass = String.Empty;
         if (Settings.Default.ProxyPass.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               ProxyPass = SymmetricProvider.Decrypt(new Data(Utils.FromBase64(encrypted)), SymmetricKey).ToString();
            }
            catch (FormatException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Proxy Password is not Base64 encoded... loading clear value.", true);
               ProxyPass = Settings.Default.ProxyPass;
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt Proxy Password... loading clear value.", true);
               ProxyPass = Settings.Default.ProxyPass;
            }
         }

         return ProxyPass;
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
         DateTime Start = HfmTrace.ExecStart;

         Symmetric SymmetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);

         try
         {
            Settings.Default.FormLocation = GetPreference<Point>(Preference.FormLocation);
            Settings.Default.FormSize = GetPreference<Size>(Preference.FormSize);
            Settings.Default.FormColumns = GetPreference<StringCollection>(Preference.FormColumns);
            Settings.Default.FormSortColumn = GetPreference<string>(Preference.FormSortColumn);
            Settings.Default.FormSortOrder = GetPreference<SortOrder>(Preference.FormSortOrder);
            Settings.Default.FormSplitLocation = GetPreference<int>(Preference.FormSplitLocation);
            Settings.Default.FormLogWindowHeight = GetPreference<int>(Preference.FormLogWindowHeight);
            Settings.Default.FormLogVisible = GetPreference<bool>(Preference.FormLogVisible);
            Settings.Default.QueueViewerVisible = GetPreference<bool>(Preference.QueueViewerVisible);
            Settings.Default.TimeStyle = GetPreference<TimeStyleType>(Preference.TimeStyle).ToString();

            Settings.Default.BenchmarksFormLocation = GetPreference<Point>(Preference.BenchmarksFormLocation);
            Settings.Default.BenchmarksFormSize = GetPreference<Size>(Preference.BenchmarksFormSize);
            Settings.Default.GraphColors = GetGraphColorsStringCollection(GetPreference<List<Color>>(Preference.GraphColors));

            Settings.Default.MessagesFormLocation = GetPreference<Point>(Preference.MessagesFormLocation);
            Settings.Default.MessagesFormSize = GetPreference<Size>(Preference.MessagesFormSize);

            Settings.Default.SyncOnLoad = GetPreference<bool>(Preference.SyncOnLoad);
            bool RaiseTimerSettingsChanged = false;
            if (Settings.Default.SyncOnSchedule != GetPreference<bool>(Preference.SyncOnSchedule) ||
                Settings.Default.SyncTimeMinutes != GetPreference<int>(Preference.SyncTimeMinutes).ToString())
            {
               RaiseTimerSettingsChanged = true;
            }
            Settings.Default.SyncOnSchedule = GetPreference<bool>(Preference.SyncOnSchedule);
            Settings.Default.SyncTimeMinutes = GetPreference<int>(Preference.SyncTimeMinutes).ToString();
            bool RaiseDuplicateCheckChanged = false;
            if (Settings.Default.DuplicateUserIDCheck != GetPreference<bool>(Preference.DuplicateUserIDCheck) ||
                Settings.Default.DuplicateProjectCheck != GetPreference<bool>(Preference.DuplicateProjectCheck))
            {
               RaiseDuplicateCheckChanged = true;
            }
            Settings.Default.DuplicateUserIDCheck = GetPreference<bool>(Preference.DuplicateUserIDCheck);
            Settings.Default.DuplicateProjectCheck = GetPreference<bool>(Preference.DuplicateProjectCheck);
            Settings.Default.AllowRunningAsync = GetPreference<bool>(Preference.AllowRunningAsync);
            bool RaiseShowUserStatsChanged = false;
            if (Settings.Default.ShowUserStats != GetPreference<bool>(Preference.ShowUserStats))
            {
               RaiseShowUserStatsChanged = true;
            }
            Settings.Default.ShowUserStats = GetPreference<bool>(Preference.ShowUserStats);

            if (Settings.Default.GenerateWeb != GetPreference<bool>(Preference.GenerateWeb) ||
                Settings.Default.GenerateInterval != GetPreference<int>(Preference.GenerateInterval).ToString() ||
                Settings.Default.WebGenAfterRefresh != GetPreference<bool>(Preference.WebGenAfterRefresh))
            {
               RaiseTimerSettingsChanged = true;
            }
            Settings.Default.GenerateWeb = GetPreference<bool>(Preference.GenerateWeb);
            Settings.Default.GenerateInterval = GetPreference<int>(Preference.GenerateInterval).ToString();
            Settings.Default.WebGenAfterRefresh = GetPreference<bool>(Preference.WebGenAfterRefresh);
            Settings.Default.WebRoot = EncryptWebRoot(GetPreference<string>(Preference.WebRoot), SymmetricProvider, IV, SymmetricKey);
            Settings.Default.WebGenCopyFAHlog = GetPreference<bool>(Preference.WebGenCopyFAHlog);
            Settings.Default.CSSFile = GetPreference<string>(Preference.CssFile);

            Settings.Default.RunMinimized = GetPreference<bool>(Preference.RunMinimized);
            Settings.Default.UseDefaultConfigFile = GetPreference<bool>(Preference.UseDefaultConfigFile);
            Settings.Default.DefaultConfigFile = GetPreference<string>(Preference.DefaultConfigFile);
            // if config file name is nothing, automatically set default config to false
            if (Settings.Default.DefaultConfigFile.Length == 0)
            {
               SetPreference(Preference.UseDefaultConfigFile, false);
               Settings.Default.UseDefaultConfigFile = false;
            }

            bool RaiseOfflineLastChanged = false;
            if (Settings.Default.OfflineLast != GetPreference<bool>(Preference.OfflineLast))
            {
               RaiseOfflineLastChanged = true;
            }
            Settings.Default.OfflineLast = GetPreference<bool>(Preference.OfflineLast);
            bool RaiseColorLogFileChanged = false;
            if (Settings.Default.ColorLogFile != GetPreference<bool>(Preference.ColorLogFile))
            {
               RaiseColorLogFileChanged = true;
            }
            Settings.Default.ColorLogFile = GetPreference<bool>(Preference.ColorLogFile);
            Settings.Default.AutoSaveConfig = GetPreference<bool>(Preference.AutoSaveConfig);
            bool RaisePpdCalculationChanged = false;
            if (Settings.Default.PpdCalculation != GetPreference<PpdCalculationType>(Preference.PpdCalculation).ToString())
            {
               RaisePpdCalculationChanged = true;
            }
            Settings.Default.PpdCalculation = GetPreference<PpdCalculationType>(Preference.PpdCalculation).ToString();
            bool RaiseDecimalPlacesChanged = false;
            if (Settings.Default.DecimalPlaces != GetPreference<int>(Preference.DecimalPlaces))
            {
               RaiseDecimalPlacesChanged = true;
            }
            Settings.Default.DecimalPlaces = GetPreference<int>(Preference.DecimalPlaces);
            bool RaiseCalculateBonusChanged = false;
            if (Settings.Default.CalculateBonus != GetPreference<bool>(Preference.CalculateBonus))
            {
               RaiseCalculateBonusChanged = true;
            }
            Settings.Default.CalculateBonus = GetPreference<bool>(Preference.CalculateBonus);
            Settings.Default.LogFileViewer = GetPreference<string>(Preference.LogFileViewer);
            Settings.Default.FileExplorer = GetPreference<string>(Preference.FileExplorer);
            bool RaiseMessageLevelChanged = false;
            if (Settings.Default.MessageLevel != GetPreference<int>(Preference.MessageLevel))
            {
               RaiseMessageLevelChanged = true;
            }
            Settings.Default.MessageLevel = GetPreference<int>(Preference.MessageLevel);

            Settings.Default.EmailReportingEnabled = GetPreference<bool>(Preference.EmailReportingEnabled);
            Settings.Default.EmailReportingToAddress = GetPreference<string>(Preference.EmailReportingToAddress);
            Settings.Default.EmailReportingFromAddress = GetPreference<string>(Preference.EmailReportingFromAddress);
            Settings.Default.EmailReportingServerAddress = GetPreference<string>(Preference.EmailReportingServerAddress);
            Settings.Default.EmailReportingServerUsername = GetPreference<string>(Preference.EmailReportingServerUsername);
            Settings.Default.EmailReportingServerPassword = EncryptEmailReportingServerPassword(GetPreference<string>(Preference.EmailReportingServerPassword), SymmetricProvider, IV, SymmetricKey);
            Settings.Default.ReportEuePause = GetPreference<bool>(Preference.ReportEuePause);

            Settings.Default.EOCUserID = GetPreference<int>(Preference.EocUserID);
            Settings.Default.StanfordID = GetPreference<string>(Preference.StanfordID);
            Settings.Default.TeamID = GetPreference<int>(Preference.TeamID);
            Settings.Default.ProjectDownloadUrl = GetPreference<string>(Preference.ProjectDownloadUrl);
            Settings.Default.UseProxy = GetPreference<bool>(Preference.UseProxy);
            Settings.Default.ProxyServer = GetPreference<string>(Preference.ProxyServer);
            Settings.Default.ProxyPort = GetPreference<int>(Preference.ProxyPort);
            Settings.Default.UseProxyAuth = GetPreference<bool>(Preference.UseProxyAuth);
            Settings.Default.ProxyUser = GetPreference<string>(Preference.ProxyUser);
            Settings.Default.ProxyPass = EncryptProxyPass(GetPreference<string>(Preference.ProxyPass), SymmetricProvider, IV, SymmetricKey);
            
            if (RaiseTimerSettingsChanged) OnTimerSettingsChanged(EventArgs.Empty);
            if (RaiseDuplicateCheckChanged) OnDuplicateCheckChanged(EventArgs.Empty);
            if (RaiseShowUserStatsChanged) OnShowUserStatsChanged(EventArgs.Empty);
            if (RaiseOfflineLastChanged) OnOfflineLastChanged(EventArgs.Empty);
            if (RaiseColorLogFileChanged) OnColorLogFileChanged(EventArgs.Empty);
            if (RaisePpdCalculationChanged) OnPpdCalculationChanged(EventArgs.Empty);
            if (RaiseDecimalPlacesChanged) OnDecimalPlacesChanged(EventArgs.Empty);
            if (RaiseCalculateBonusChanged) OnCalculateBonusChanged(EventArgs.Empty);
            if (RaiseMessageLevelChanged) OnMessageLevelChanged(EventArgs.Empty);

            Settings.Default.Save();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         Debug.WriteLine(String.Format("{0} Execution Time: {1}", HfmTrace.FunctionName, HfmTrace.GetExecTime(Start)));
      }

      #region Save Support Methods
      private static StringCollection GetGraphColorsStringCollection(IEnumerable<Color> collection)
      {
         StringCollection col = new StringCollection();
         foreach (Color color in collection)
         {
            col.Add(color.Name);
         }
         return col;
      }

      private static string EncryptWebRoot(string clear, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string WebRoot = String.Empty;
         if (clear.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               WebRoot = SymmetricProvider.Encrypt(new Data(clear), SymmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt WebGen Root Folder... saving clear value.");
               WebRoot = clear;
            }
         }
         
         return WebRoot;
      }

      private static string EncryptEmailReportingServerPassword(string clear, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string EmailReportingServerPassword = String.Empty;
         if (clear.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               EmailReportingServerPassword = SymmetricProvider.Encrypt(new Data(clear), SymmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt Smtp Server Password... saving clear value.");
               EmailReportingServerPassword = clear;
            }
         }
         
         return EmailReportingServerPassword;
      }

      private static string EncryptProxyPass(string clear, Symmetric SymmetricProvider, Data IV, Data SymmetricKey)
      {
         string ProxyPass = String.Empty;
         if (clear.Length > 0)
         {
            try
            {
               SymmetricProvider.IntializationVector = IV;
               ProxyPass = SymmetricProvider.Encrypt(new Data(clear), SymmetricKey).ToBase64();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt Proxy Password... saving clear value.");
               ProxyPass = clear;
            }
         }

         return ProxyPass;
      }
      #endregion
      
      #endregion

      #region Event Wrappers
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
      /// Duplicate (Client ID or Project (R/C/G)) Check Settings Changed
      /// </summary>
      public event EventHandler DuplicateCheckChanged;
      private void OnDuplicateCheckChanged(EventArgs e)
      {
         if (DuplicateCheckChanged != null)
         {
            DuplicateCheckChanged(this, e);
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
         else if (ValidateMinutes(output) == false)
         {
            output = defaultValue;
         }
         
         return output;
      }
      
      public static bool ValidateMinutes(int Minutes)
      {
         if ((Minutes > Constants.MaxMinutes) || (Minutes < Constants.MinMinutes))
         {
            return false;
         }

         return true;
      } 
      
      //public static bool ValidateDecimalPlaces(int Places)
      //{
      //   if ((Places > MaxDecimalPlaces) || (Places < MinDecimalPlaces))
      //   {
      //      return false;
      //   }

      //   return true;
      //}
      #endregion
      
      #region Preference Formatting
      /// <summary>
      /// PPD String Formatter
      /// </summary>
      public string PpdFormatString
      {
         get
         {
            int DecimalPlaces = GetPreference<int>(Preference.DecimalPlaces);

            StringBuilder sbldr = new StringBuilder("###,###,##0");
            if (DecimalPlaces > 0)
            {
               sbldr.Append(".");
               for (int i = 0; i < DecimalPlaces; i++)
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
