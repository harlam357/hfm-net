/*
 * HFM.NET - User Preferences
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;

using harlam357.Security;
using harlam357.Security.Encryption;

using HFM.Preferences.Properties;
using HFM.Instrumentation;

namespace HFM.Preferences
{
   #region Enum
   public enum ePpdCalculation
   {
      LastFrame,
      LastThreeFrames,
      AllFrames,
      EffectiveRate
   }

   public enum eTimeStyle
   {
      Standard,
      Formatted
   }

   public enum ClientStatus
   {
      Unknown,
      Offline,
      Stopped,
      EuePause,
      Hung,
      Paused,
      RunningNoFrameTimes,
      Running
   }
   #endregion

   public class PreferenceSet
   {
      #region Public Const
      public const string ExeName = "HFM";

      public const string EOCUserXmlURL = "http://folding.extremeoverclocking.com/xml/user_summary.php?u=";
      public const string EOCUserBaseURL = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const string EOCTeamBaseURL = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const string StanfordBaseURL = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";

      public const Int32 MinDecimalPlaces = 0;
      public const Int32 MaxDecimalPlaces = 5;
      
      public const Int32 MinMinutes = 1;
      public const Int32 MaxMinutes = 180;
      
      public const Int32 MinutesDefault = 15;
      public const Int32 ProxyPortDefault = 8080;

      public const string UnassignedDescription = "Unassigned Description";

      private const string IV = "WN`f,cgvR{iDY^[=mS>0[0#BL;/7.td7aV=3p1O#:68n/$`]qyIF)e*@7qZ{RTUS";
      private const string SymmetricKey = "TJ5[7EB;L:Enmw#Y'5Q.P81v^o,06!Hf_4f&hNBC$9Rh*tK}pnNK7D&<r]Fq#%Hy";
      #endregion

      #region Public Properties and associated Private Variables
      private Boolean _SyncOnLoad;
      public Boolean SyncOnLoad
      {
         get { return _SyncOnLoad; }
         set { _SyncOnLoad = value; }
      }

      private Boolean _SyncOnSchedule;
      public Boolean SyncOnSchedule
      {
         get { return _SyncOnSchedule; }
         set { _SyncOnSchedule = value; }
      }

      private Int32 _SyncTimeMinutes;
      public Int32 SyncTimeMinutes
      {
         get { return _SyncTimeMinutes; }
         set { _SyncTimeMinutes = value; }
      }

      private Boolean _GenerateWeb;
      public Boolean GenerateWeb
      {
         get { return _GenerateWeb; }
         set { _GenerateWeb = value; }
      }

      private Int32 _GenerateInterval;
      public Int32 GenerateInterval
      {
         get { return _GenerateInterval; }
         set { _GenerateInterval = value; }
      }

      private String _WebRoot;
      public String WebRoot
      {
         get { return _WebRoot; }
         set { _WebRoot = value; }
      }

      private String _CSSFile;
      public String CSSFileName
      {
         get { return _CSSFile; }
         set { _CSSFile = value; }
      }

      private Int32 _EOCUserID;
      public Int32 EOCUserID
      {
         get { return _EOCUserID; }
         set { _EOCUserID = value; }
      }

      private String _StanfordID;
      public String StanfordID
      {
         get { return _StanfordID; }
         set { _StanfordID = value; }
      }

      private String _AppDataPath;
      public String AppDataPath
      {
         get { return _AppDataPath; }
      }

      private Int32 _TeamID;
      public Int32 TeamID
      {
         get { return _TeamID; }
         set { _TeamID = value; }
      }

      private Boolean _UseProxy;
      public Boolean UseProxy
      {
         get { return _UseProxy; }
         set { _UseProxy = value; }
      }

      private String _ProxyServer;
      public String ProxyServer
      {
         get { return _ProxyServer; }
         set { _ProxyServer = value; }
      }

      private Int32 _ProxyPort;
      public Int32 ProxyPort
      {
         get { return _ProxyPort; }
         set { _ProxyPort = value; }
      }

      private Boolean _UseProxyAuth;
      public Boolean UseProxyAuth
      {
         get { return _UseProxyAuth; }
         set { _UseProxyAuth = value; }
      }

      private String _ProxyUser;
      public String ProxyUser
      {
         get { return _ProxyUser; }
         set { _ProxyUser = value; }
      }

      private String _ProxyPass;
      public String ProxyPass
      {
         get { return _ProxyPass; }
         set { _ProxyPass = value; }
      }

      private string _CacheFolder;
      public string CacheFolder
      {
         get { return _CacheFolder; }
         set { _CacheFolder = value; }
      }

      private System.Drawing.Point _FormLocation;
      public System.Drawing.Point FormLocation
      {
         get { return _FormLocation; }
         set { _FormLocation = value; }
      }

      private System.Drawing.Size _FormSize;
      public System.Drawing.Size FormSize
      {
         get { return _FormSize; }
         set { _FormSize = value; }
      }

      private System.Collections.Specialized.StringCollection _FormColumns;
      public System.Collections.Specialized.StringCollection FormColumns
      {
         get { return _FormColumns; }
         set { _FormColumns = value; }
      }

      private bool _FormLogVisible;
      public bool FormLogVisible
      {
         get { return _FormLogVisible; }
         set { _FormLogVisible = value; }
      }

      private string _FormSortColumn;
      public string FormSortColumn
      {
         get { return _FormSortColumn; }
         set { _FormSortColumn = value; }
      }

      private SortOrder _FormSortOrder;
      public SortOrder FormSortOrder
      {
         get { return _FormSortOrder; }
         set { _FormSortOrder = value; }
      }

      #region OfflineLast
      public event EventHandler OfflineLastChanged;
      private bool _OfflineLast;
      public bool OfflineLast
      {
         get { return _OfflineLast; }
         set 
         {
            if (_OfflineLast != value)
            { 
               _OfflineLast = value; 
               OnOfflineLastChanged(EventArgs.Empty);
            }
         }
      }
      
      protected void OnOfflineLastChanged(EventArgs e)
      {
         if (OfflineLastChanged != null)
         {
            OfflineLastChanged(this, e);
         }
      }
      #endregion

      private string _DefaultConfigFile;
      public string DefaultConfigFile
      {
         get { return _DefaultConfigFile; }
         set { _DefaultConfigFile = value; }
      }

      private bool _UseDefaultConfigFile;
      public bool UseDefaultConfigFile
      {
         get { return _UseDefaultConfigFile; }
         set { _UseDefaultConfigFile = value; }
      }
      
      private bool _AutoSaveConfig;
      public bool AutoSaveConfig
      {
         get { return _AutoSaveConfig; }
         set { _AutoSaveConfig = value; }
      }
      
      private ePpdCalculation _PpdCalculation;
      public ePpdCalculation PpdCalculation
      {
         get { return _PpdCalculation; }
         set { _PpdCalculation = value; }
      }
      
      private eTimeStyle _TimeStyle;
      public eTimeStyle TimeStyle
      {
         get { return _TimeStyle; }
         set { _TimeStyle = value; }
      }
      
      private string _LogFileViewer;
      public string LogFileViewer
      {
         get { return _LogFileViewer; }
         set { _LogFileViewer = value; }
      }

      private string _FileExplorer;
      public string FileExplorer
      {
         get { return _FileExplorer; }
         set { _FileExplorer = value; }
      }
      
      private string _ProjectDownloadUrl;
      public string ProjectDownloadUrl
      {
         get { return _ProjectDownloadUrl; }
         set { _ProjectDownloadUrl = value; }
      }
      
      private bool _WebGenAfterRefresh;
      public bool WebGenAfterRefresh
      {
         get { return _WebGenAfterRefresh; }
         set { _WebGenAfterRefresh = value; }
      }
      
      #region MessageLevel
      public event EventHandler MessageLevelChanged;
      private int _MessageLevel;
      public int MessageLevel
      {
         get { return _MessageLevel; }
         set 
         {
            if (_MessageLevel != value)
            {
               _MessageLevel = value; 
               OnMessageLevelChanged(EventArgs.Empty);
            }
         }
      }
      
      protected void OnMessageLevelChanged(EventArgs e)
      {
         if (MessageLevelChanged != null)
         {
            MessageLevelChanged(this, e);
         }
      }
      #endregion

      private int _FormSplitLocation;
      public int FormSplitLocation
      {
         get { return _FormSplitLocation; }
         set { _FormSplitLocation = value; }
      }
      
      private int _FormLogWindowHeight;
      public int FormLogWindowHeight
      {
         get { return _FormLogWindowHeight; }
         set { _FormLogWindowHeight = value; }
      }
      
      private int _DecimalPlaces;
      public int DecimalPlaces
      {
         get { return _DecimalPlaces; }
         set { _DecimalPlaces = value; }
      }
      
      #region ShowUserStats
      public event EventHandler ShowUserStatsChanged;
      private bool _ShowUserStats;
      public bool ShowUserStats
      {
         get { return _ShowUserStats; }
         set 
         { 
            if (_ShowUserStats != value)
            {
               _ShowUserStats = value;
               OnShowUserStatsChanged(EventArgs.Empty);
            }
         }
      }
      
      protected void OnShowUserStatsChanged(EventArgs e)
      {
         if (ShowUserStatsChanged != null)
         {
            ShowUserStatsChanged(this, e);
         }
      }
      #endregion
      
      #region Duplicate Checks
      public event EventHandler DuplicateCheckChanged;
      private bool _DuplicateUserIDCheck;
      public bool DuplicateUserIDCheck
      {
         get { return _DuplicateUserIDCheck; }
         set { _DuplicateUserIDCheck = value; }
      }

      private bool _DuplicateProjectCheck;
      public bool DuplicateProjectCheck
      {
         get { return _DuplicateProjectCheck; }
         set { _DuplicateProjectCheck = value; }
      }
      
      protected void OnDuplicateCheckChanged(EventArgs e)
      {
         if (DuplicateCheckChanged != null)
         {
            DuplicateCheckChanged(this, e);
         }
      }
      #endregion

      #region Color Log File
      public event EventHandler ColorLogFileChanged;
      private bool _ColorLogFile;
      public bool ColorLogFile
      {
         get { return _ColorLogFile; }
         set 
         { 
            if (_ColorLogFile != value)
            {
               _ColorLogFile = value;
               OnColorLogFileChanged(EventArgs.Empty);
            }
         }
      }

      protected void OnColorLogFileChanged(EventArgs e)
      {
         if (ColorLogFileChanged != null)
         {
            ColorLogFileChanged(this, e);
         }
      } 
      #endregion
      
      private bool _EmailReportingEnabled;
      public bool EmailReportingEnabled
      {
         get { return _EmailReportingEnabled; }
         set { _EmailReportingEnabled = value; }
      }
      
      private string _EmailReportingToAddress;
      public string EmailReportingToAddress
      {
         get { return _EmailReportingToAddress; }
         set { _EmailReportingToAddress = value; }
      }

      private string _EmailReportingFromAddress;
      public string EmailReportingFromAddress
      {
         get { return _EmailReportingFromAddress; }
         set { _EmailReportingFromAddress = value; }
      }

      private string _EmailReportingServerAddress;
      public string EmailReportingServerAddress
      {
         get { return _EmailReportingServerAddress; }
         set { _EmailReportingServerAddress = value; }
      }

      private string _EmailReportingServerUsername;
      public string EmailReportingServerUsername
      {
         get { return _EmailReportingServerUsername; }
         set { _EmailReportingServerUsername = value; }
      }

      private string _EmailReportingServerPassword;
      public string EmailReportingServerPassword
      {
         get { return _EmailReportingServerPassword; }
         set { _EmailReportingServerPassword = value; }
      }

      public static String AppPath
      {
         get
         {
            return Path.GetDirectoryName(Application.ExecutablePath);
         }
      }
      
      public string EOCUserXml
      {
         get 
         { 
            return String.Concat(EOCUserXmlURL, EOCUserID);
         }
      }

      public string EOCUserURL
      {
         get
         {
            return String.Concat(EOCUserBaseURL, EOCUserID);
         }
      }

      public string EOCTeamURL
      {
         get
         {
            return String.Concat(EOCTeamBaseURL, TeamID);
         }
      }

      public string StanfordUserURL
      {
         get
         {
            return String.Concat(StanfordBaseURL + StanfordID);
         }
      }

      #endregion

      #region Singleton Support

      private static PreferenceSet _Instance;
      private static readonly Object classLock = typeof(PreferenceSet);

      public static PreferenceSet Instance
      {
         get
         {
            lock (classLock)
            {
               if (_Instance == null)
               {
                  _Instance = new PreferenceSet();
               }
            }
            return _Instance;
         }
      }

      #endregion

      #region Constructor
      /// <summary>
      /// Private Constructor to enforce Singleton pattern; loads preferences
      /// </summary>
      private PreferenceSet()
      {
         Load();
      } 
      #endregion

      #region Implementation
      /// <summary>
      /// Load the current set of preferences and adjust for sanity
      /// </summary>
      public void Load()
      {
         DateTime Start = HfmTrace.ExecStart;

         UpgradeUserSettings();
         
         Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);
         SymetricProvider.IntializationVector = new Data(IV);
         SymetricProvider.Key = new Data(SymmetricKey);

         _CSSFile = Settings.Default.CSSFile;

         if (Int32.TryParse(Settings.Default.GenerateInterval, out _GenerateInterval) == false)
         {
            _GenerateInterval = MinutesDefault;
         }
         else if (ValidateMinutes(_GenerateInterval) == false)
         {
            _GenerateInterval = MinutesDefault;
         }

         _GenerateWeb = Settings.Default.GenerateWeb;
         _SyncOnLoad = Settings.Default.SyncOnLoad;
         _SyncOnSchedule = Settings.Default.SyncOnSchedule;

         if (Int32.TryParse(Settings.Default.SyncTimeMinutes, out _SyncTimeMinutes) == false)
         {
            _SyncTimeMinutes = MinutesDefault;
         }
         else if (ValidateMinutes(_SyncTimeMinutes) == false)
         {
            _SyncTimeMinutes = MinutesDefault;
         }

         _WebRoot = String.Empty;
         if (Settings.Default.WebRoot.Length > 0)
         {
            try
            {
               _WebRoot = SymetricProvider.Decrypt(new Data(Settings.Default.WebRoot)).ToString();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt WebGen Root Folder... loading clear value.", true);
               _WebRoot = Settings.Default.WebRoot;
            }
         }
         
         _EOCUserID = Settings.Default.EOCUserID;
         _StanfordID = Settings.Default.StanfordID;
         _TeamID = Settings.Default.TeamID;

         _UseProxy = Settings.Default.UseProxy;
         _ProxyServer = Settings.Default.ProxyServer;
         _ProxyPort = Settings.Default.ProxyPort;
         _UseProxyAuth = Settings.Default.UseProxyAuth;
         _ProxyUser = Settings.Default.ProxyUser;
         
         _ProxyPass = String.Empty;
         if (Settings.Default.ProxyPass.Length > 0)
         {
            try
            {
               _ProxyPass = SymetricProvider.Decrypt(new Data(Settings.Default.ProxyPass)).ToString();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt Proxy Password... loading clear value.", true);
               _ProxyPass = Settings.Default.ProxyPass;
            }
         }

         _CacheFolder = Settings.Default.CacheFolder;
         _FormLogVisible = Settings.Default.FormLogVisible;

         try
         {
            _FormLocation = Settings.Default.FormLocation;
            _FormSize = Settings.Default.FormSize;
            _FormColumns = Settings.Default.FormColumns;
         }
         catch (NullReferenceException)
         { }

         _FormSortColumn = Settings.Default.FormSortColumn;
         try
         {
            _FormSortOrder = Settings.Default.FormSortOrder;
         }
         catch (NullReferenceException)
         {
            _FormSortOrder = SortOrder.None;
         }
         _FormSplitLocation = Settings.Default.FormSplitLocation;
         _FormLogWindowHeight = Settings.Default.FormLogWindowHeight;

         _OfflineLast = Settings.Default.OfflineLast;
         _DefaultConfigFile = Settings.Default.DefaultConfigFile;
         _UseDefaultConfigFile = Settings.Default.UseDefaultConfigFile;
         _AutoSaveConfig = Settings.Default.AutoSaveConfig;

         switch (Settings.Default.PpdCalculation)
         {
            case "LastFrame":
               _PpdCalculation = ePpdCalculation.LastFrame;
               break;
            case "LastThreeFrames":
               _PpdCalculation = ePpdCalculation.LastThreeFrames;
               break;
            case "AllFrames":
               _PpdCalculation = ePpdCalculation.AllFrames;
               break;
            case "EffectiveRate":
               _PpdCalculation = ePpdCalculation.EffectiveRate;
               break;               
            default:
               _PpdCalculation = ePpdCalculation.LastThreeFrames;
               break;
         }

         switch (Settings.Default.TimeStyle)
         {
            case "Standard":
               _TimeStyle = eTimeStyle.Standard;
               break;
            case "Formatted":
               _TimeStyle = eTimeStyle.Formatted;
               break;
            default:
               _TimeStyle = eTimeStyle.Standard;
               break;
         }

         _LogFileViewer = Settings.Default.LogFileViewer;
         _FileExplorer = Settings.Default.FileExplorer;
         _ProjectDownloadUrl = Settings.Default.ProjectDownloadUrl;
         _WebGenAfterRefresh = Settings.Default.WebGenAfterRefresh;
         _MessageLevel = Settings.Default.MessageLevel;
         _DecimalPlaces = Settings.Default.DecimalPlaces;
         _ShowUserStats = Settings.Default.ShowUserStats;
         _DuplicateUserIDCheck = Settings.Default.DuplicateUserIDCheck;
         _DuplicateProjectCheck = Settings.Default.DuplicateProjectCheck;
         _ColorLogFile = Settings.Default.ColorLogFile;
         _EmailReportingEnabled = Settings.Default.EmailReportingEnabled;
         _EmailReportingToAddress = Settings.Default.EmailReportingToAddress;
         _EmailReportingFromAddress = Settings.Default.EmailReportingFromAddress;
         _EmailReportingServerAddress = Settings.Default.EmailReportingServerAddress;
         _EmailReportingServerUsername = Settings.Default.EmailReportingServerUsername;
         _EmailReportingServerPassword = String.Empty;
         if (Settings.Default.EmailReportingServerPassword.Length > 0)
         {
            try
            {
               _EmailReportingServerPassword =
                  SymetricProvider.Decrypt(new Data(Settings.Default.EmailReportingServerPassword)).ToString();
            }
            catch (CryptographicException)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Cannot decrypt Stmp Server Password... loading clear value.", true);
               _EmailReportingServerPassword = Settings.Default.EmailReportingServerPassword;
            }
         }
         
         _AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         _AppDataPath = Path.Combine(_AppDataPath, ExeName);
         if (Directory.Exists(_AppDataPath) == false)
         {
            Directory.CreateDirectory(_AppDataPath);
         }

         Debug.WriteLine(String.Format("{0} Execution Time: {1}", HfmTrace.FunctionName, HfmTrace.GetExecTime(Start)));
      }

      private static void UpgradeUserSettings()
      {
         // Only store Major.Minor.Build, Changes to Settings will
         // only be made when these numbers change, not Revision!!!
         string appVersionString = PlatformOps.ApplicationVersion;

         if (Settings.Default.ApplicationVersion != appVersionString)
         {
            Settings.Default.Upgrade();
            Settings.Default.ApplicationVersion = appVersionString;
            Settings.Default.Save();
         }
      }

      /// <summary>
      /// Revert to the previously saved settings
      /// </summary>
      public void Discard()
      {
         Load();
      }

      /// <summary>
      /// Save the current settings
      /// </summary>
      public void Save()
      {
         DateTime Start = HfmTrace.ExecStart;

         Symmetric SymetricProvider = new Symmetric(Symmetric.Provider.Rijndael, false);
         SymetricProvider.IntializationVector = new Data(IV);
         SymetricProvider.Key = new Data(SymmetricKey);

         try
         {
            Settings.Default.CSSFile = _CSSFile;
            Settings.Default.GenerateInterval = _GenerateInterval.ToString();
            Settings.Default.GenerateWeb = _GenerateWeb;
            Settings.Default.SyncOnLoad = _SyncOnLoad;
            Settings.Default.SyncOnSchedule = _SyncOnSchedule;
            Settings.Default.SyncTimeMinutes = _SyncTimeMinutes.ToString();
            Settings.Default.WebRoot = String.Empty;
            if (_WebRoot.Length > 0)
            {
               try
               {
                  Settings.Default.WebRoot = SymetricProvider.Encrypt(new Data(_WebRoot)).ToString();
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt WebGen Root Folder... saving clear value.");
                  Settings.Default.WebRoot = _WebRoot;
               }
            }
            
            Settings.Default.EOCUserID = _EOCUserID;
            Settings.Default.StanfordID = _StanfordID;
            Settings.Default.TeamID = _TeamID;

            // Proxy Settings
            Settings.Default.UseProxy = _UseProxy;
            Settings.Default.ProxyServer = _ProxyServer;
            Settings.Default.ProxyPort = _ProxyPort;
            Settings.Default.UseProxyAuth = _UseProxyAuth;
            Settings.Default.ProxyUser = _ProxyUser;
            Settings.Default.ProxyPass = String.Empty;
            if (_ProxyPass.Length > 0)
            {
               try
               {
                  Settings.Default.ProxyPass = SymetricProvider.Encrypt(new Data(_ProxyPass)).ToString();
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt Proxy Password... saving clear value.");
                  Settings.Default.ProxyPass = _ProxyPass;
               }
            }

            Settings.Default.CacheFolder = _CacheFolder;
            Settings.Default.FormLogVisible = _FormLogVisible;
            Settings.Default.FormLocation = _FormLocation;
            Settings.Default.FormSize = _FormSize;
            Settings.Default.FormColumns = _FormColumns;
            Settings.Default.FormSortColumn = _FormSortColumn;
            Settings.Default.FormSortOrder = _FormSortOrder;
            Settings.Default.FormSplitLocation = _FormSplitLocation;
            Settings.Default.FormLogWindowHeight = _FormLogWindowHeight;
            Settings.Default.OfflineLast = _OfflineLast;
            Settings.Default.DefaultConfigFile = _DefaultConfigFile;
            Settings.Default.AutoSaveConfig = _AutoSaveConfig;
            Settings.Default.UseDefaultConfigFile = _UseDefaultConfigFile;
            Settings.Default.PpdCalculation = _PpdCalculation.ToString();
            Settings.Default.TimeStyle = _TimeStyle.ToString();
            Settings.Default.LogFileViewer = _LogFileViewer;
            Settings.Default.FileExplorer = _FileExplorer;
            Settings.Default.ProjectDownloadUrl = _ProjectDownloadUrl;
            Settings.Default.WebGenAfterRefresh = _WebGenAfterRefresh;
            Settings.Default.MessageLevel = _MessageLevel;
            Settings.Default.DecimalPlaces = _DecimalPlaces;
            Settings.Default.ShowUserStats = _ShowUserStats;
            
            #region Duplicate Checks
            bool RaiseDuplicateCheckChanged = false;
            if (Settings.Default.DuplicateUserIDCheck != _DuplicateUserIDCheck ||
                Settings.Default.DuplicateProjectCheck != _DuplicateProjectCheck)
            {
               RaiseDuplicateCheckChanged = true;
            }

            Settings.Default.DuplicateUserIDCheck = _DuplicateUserIDCheck;
            Settings.Default.DuplicateProjectCheck = _DuplicateProjectCheck;
            
            if (RaiseDuplicateCheckChanged) OnDuplicateCheckChanged(EventArgs.Empty);
            #endregion

            Settings.Default.ColorLogFile = _ColorLogFile;
            Settings.Default.EmailReportingEnabled = _EmailReportingEnabled;
            Settings.Default.EmailReportingToAddress = _EmailReportingToAddress;
            Settings.Default.EmailReportingFromAddress = _EmailReportingFromAddress;
            Settings.Default.EmailReportingServerAddress = _EmailReportingServerAddress;
            Settings.Default.EmailReportingServerUsername = _EmailReportingServerUsername;
            Settings.Default.EmailReportingServerPassword = String.Empty;
            if (_EmailReportingServerPassword.Length > 0)
            {
               try
               {
                  Settings.Default.EmailReportingServerPassword =
                     SymetricProvider.Encrypt(new Data(_EmailReportingServerPassword)).ToString();
               }
               catch (CryptographicException)
               {
                  HfmTrace.WriteToHfmConsole(TraceLevel.Warning, "Failed to encrypt Smtp Server Password... saving clear value.");
                  Settings.Default.EmailReportingServerPassword = _EmailReportingServerPassword;
               }
            }

            Settings.Default.Save();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
         }

         Debug.WriteLine(String.Format("{0} Execution Time: {1}", HfmTrace.FunctionName, HfmTrace.GetExecTime(Start)));
      } 
      #endregion

      #region Preference Validation
      public static bool ValidateMinutes(int Minutes)
      {
         if ((Minutes > MaxMinutes) || (Minutes < MinMinutes))
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
      public static string GetPPDFormatString()
      {
         int DecimalPlaces = Instance.DecimalPlaces;
      
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
      #endregion
   }
}
