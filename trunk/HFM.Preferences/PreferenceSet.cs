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
using System.Windows.Forms;

using HFM.Preferences.Properties;
using Debug=HFM.Instrumentation.Debug;

namespace HFM.Preferences
{
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

   public class PreferenceSet
   {
      public const String EOCUserBaseURL = "http://folding.extremeoverclocking.com/user_summary.php?s=&u=";
      public const String EOCTeamBaseURL = "http://folding.extremeoverclocking.com/team_summary.php?s=&t=";
      public const String StanfordBaseURL = "http://fah-web.stanford.edu/cgi-bin/main.py?qtype=userpage&username=";

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

      private bool _OfflineLast;
      public bool OfflineLast
      {
         get { return _OfflineLast; }
         set { _OfflineLast = value; }
      }

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
      
      private int _MessageLevel;
      public int MessageLevel
      {
         get { return _MessageLevel; }
         set { _MessageLevel = value; }
      }
      
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

      public String AppPath
      {
         get
         {
            String s = System.Reflection.Assembly.GetEntryAssembly().Location;
            String[] sParts = s.Split('\\');
            sParts[sParts.GetUpperBound(0)] = "";
            s = String.Join("\\", sParts);
            s.Trim(new char[] { '\\' });

            return s;
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

      /// <summary>
      /// Private Constructor to enforce Singleton pattern; loads preferences
      /// </summary>
      private PreferenceSet()
      {
         Load();
      }

      /// <summary>
      /// Load the current set of preferences and adjust for sanity
      /// </summary>
      public void Load()
      {
         DateTime Start = Debug.ExecStart;

         UpgradeUserSettings();

         _CSSFile = Settings.Default.CSSFile;

         if (Int32.TryParse(Settings.Default.GenerateInterval, out _GenerateInterval) == false)
         {
            _GenerateInterval = 15;
         }

         _GenerateWeb = Settings.Default.GenerateWeb;
         _SyncOnLoad = Settings.Default.SyncOnLoad;
         _SyncOnSchedule = Settings.Default.SyncOnSchedule;

         if (Int32.TryParse(Settings.Default.SyncTimeMinutes, out _SyncTimeMinutes) == false)
         {
            _SyncTimeMinutes = 15;
         }

         _WebRoot = Settings.Default.WebRoot;
         _EOCUserID = Settings.Default.EOCUserID;
         _StanfordID = Settings.Default.StanfordID;
         _TeamID = Settings.Default.TeamID;

         _UseProxy = Settings.Default.UseProxy;
         _ProxyServer = Settings.Default.ProxyServer;
         _ProxyPort = Settings.Default.ProxyPort;
         _UseProxyAuth = Settings.Default.UseProxyAuth;
         _ProxyUser = Settings.Default.ProxyUser;
         _ProxyPass = Settings.Default.ProxyPass;

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
         
         switch (Settings.Default.PpdCalculation)
         {
            case "LastFrame":
               _PpdCalculation = ePpdCalculation.LastFrame;
               break;
            case "LastThreeFrames":
               _PpdCalculation = ePpdCalculation.LastThreeFrames;
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

         _AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
         _AppDataPath = Path.Combine(_AppDataPath, System.Reflection.Assembly.GetEntryAssembly().GetName().Name); ;
         if (Directory.Exists(_AppDataPath) == false)
         {
            Directory.CreateDirectory(_AppDataPath);
         }

         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
      }

      private static void UpgradeUserSettings()
      {
         System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
         string appVersionString = asm.GetName().Version.ToString();

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
         DateTime Start = Debug.ExecStart;

         try
         {
            Settings.Default.CSSFile = _CSSFile;
            Settings.Default.GenerateInterval = _GenerateInterval.ToString();
            Settings.Default.GenerateWeb = _GenerateWeb;
            Settings.Default.SyncOnLoad = _SyncOnLoad;
            Settings.Default.SyncOnSchedule = _SyncOnSchedule;
            Settings.Default.SyncTimeMinutes = _SyncTimeMinutes.ToString();
            Settings.Default.WebRoot = _WebRoot;
            Settings.Default.EOCUserID = _EOCUserID;
            Settings.Default.StanfordID = _StanfordID;
            Settings.Default.TeamID = _TeamID;

            // Proxy Settings
            Settings.Default.UseProxy = _UseProxy;
            Settings.Default.ProxyServer = _ProxyServer;
            Settings.Default.ProxyPort = _ProxyPort;
            Settings.Default.UseProxyAuth = _UseProxyAuth;
            Settings.Default.ProxyUser = _ProxyUser;
            Settings.Default.ProxyPass = _ProxyPass;

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
            Settings.Default.UseDefaultConfigFile = _UseDefaultConfigFile;
            Settings.Default.PpdCalculation = _PpdCalculation.ToString();
            Settings.Default.TimeStyle = _TimeStyle.ToString();
            Settings.Default.LogFileViewer = _LogFileViewer;
            Settings.Default.FileExplorer = _FileExplorer;
            Settings.Default.ProjectDownloadUrl = _ProjectDownloadUrl;
            Settings.Default.WebGenAfterRefresh = _WebGenAfterRefresh;
            Settings.Default.MessageLevel = _MessageLevel;

            Settings.Default.Save();
         }
         catch (Exception ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Error, String.Format("{0} threw exception {1}.", Debug.FunctionName, ex.Message));
         }
         Debug.WriteToHfmConsole(TraceLevel.Info, String.Format("{0} Execution Time: {1}", Debug.FunctionName, Debug.GetExecTime(Start)));
      }

      /// <summary>
      /// Save on destroy
      /// </summary>
      //~PreferenceSet()
      //{
      //   Save();
      //}
   }
}
