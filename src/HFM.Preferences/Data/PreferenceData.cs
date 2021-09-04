using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace HFM.Preferences.Data
{
    [DataContract(Namespace = "")]
    public class ApplicationSettings
    {
        public const string DefaultProjectDownloadUrl = "https://apps.foldingathome.org/psummary.json";

        public ApplicationSettings()
        {
            CacheFolder = "logcache";
            MessageLevel = 4; // Info
            //AutoSaveConfig = false;
            DecimalPlaces = 1;
            PpdCalculation = "LastThreeFrames";
            //BonusCalculation = null;
            LogFileViewer = "notepad.exe";
            FileExplorer = "explorer.exe";
            ProjectDownloadUrl = DefaultProjectDownloadUrl;
            DuplicateProjectCheck = true;
        }

        [DataMember]
        public string CacheFolder { get; set; }

        [DataMember]
        public int MessageLevel { get; set; }

        [DataMember]
        public bool AutoSaveConfig { get; set; }

        [DataMember]
        public int DecimalPlaces { get; set; }

        [DataMember]
        public string PpdCalculation { get; set; }

        [DataMember]
        public string BonusCalculation { get; set; }

        [DataMember]
        public string LogFileViewer { get; set; }

        [DataMember]
        public string FileExplorer { get; set; }

        [DataMember]
        public string ProjectDownloadUrl { get; set; }

        [DataMember]
        public bool DuplicateProjectCheck { get; set; }
    }

    [DataContract(Namespace = "")]
    public class WebDeployment
    {
        public WebDeployment()
        {
            //DeploymentType = null;
            //DeploymentRoot = null;
            FtpServer = new ConnectionProperties
            {
                //Server = null,
                Port = 21,
                //Username = null,
                //Password = null
            };
            //FtpMode = null;
            CopyHtml = true;
            //CopyXml = false;
            CopyLog = true;
            //LogSizeLimitEnabled = false;
            LogSizeLimitedTo = 1024;
        }

        [DataMember]
        public string DeploymentType { get; set; }

        [DataMember]
        public string DeploymentRoot { get; set; }

        [DataMember]
        public ConnectionProperties FtpServer { get; set; }

        [DataMember]
        public string FtpMode { get; set; }

        [DataMember]
        public bool CopyHtml { get; set; }

        [DataMember]
        public bool CopyXml { get; set; }

        [DataMember]
        public bool CopyLog { get; set; }

        [DataMember]
        public bool LogSizeLimitEnabled { get; set; }

        [DataMember]
        public int LogSizeLimitedTo { get; set; }
    }

    [DataContract(Namespace = "")]
    public class WebRendering
    {
        public WebRendering()
        {
            StyleSheet = "Blue.css";
            OverviewTransform = "WebOverview.xslt";
            SummaryTransform = "WebSummary.xslt";
            SlotTransform = "WebSlot.xslt";
        }

        [DataMember]
        public string StyleSheet { get; set; }

        [DataMember]
        public string OverviewTransform { get; set; }

        [DataMember]
        public string SummaryTransform { get; set; }

        [DataMember]
        public string SlotTransform { get; set; }
    }

    [DataContract(Namespace = "")]
    public class Startup
    {
        public Startup()
        {
            //RunMinimized = false;
            CheckForUpdate = true;
            //DefaultConfigFileEnabled = false;
            //DefaultConfigFilePath = null;
        }

        [DataMember]
        public bool RunMinimized { get; set; }

        [DataMember]
        public bool CheckForUpdate { get; set; }

        [DataMember]
        public bool DefaultConfigFileEnabled { get; set; }

        [DataMember]
        public string DefaultConfigFilePath { get; set; }
    }

    [DataContract(Namespace = "")]
    public class WebProxy
    {
        public WebProxy()
        {
            //Enabled = false;
            Server = new ConnectionProperties
            {
                //Address = null,
                Port = 8080,
                //Username = null,
                //Password = null
            };
            //CredentialsEnabled = false;
        }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public ConnectionProperties Server { get; set; }

        [DataMember]
        public bool CredentialsEnabled { get; set; }
    }

    [DataContract(Namespace = "")]
    public class Email
    {
        public Email()
        {
            //Enabled = false;
            //ToAddress = null;
            //FromAddress = null;
            SmtpServer = new ConnectionProperties
            {
                //Address = null,
                Port = 25,
                //Username = null,
                //Password = null
            };
            //SecureConnection = false;
        }

        [DataMember]
        public bool Enabled { get; set; }

        [DataMember]
        public string ToAddress { get; set; }

        [DataMember]
        public string FromAddress { get; set; }

        [DataMember]
        public ConnectionProperties SmtpServer { get; set; }

        [DataMember]
        public bool SecureConnection { get; set; }
    }

    [DataContract(Namespace = "")]
    public class Reporting
    {

    }

    [DataContract(Namespace = "")]
    public class UserSettings
    {
        public UserSettings()
        {
            //TeamId = 0;
            EocUserId = 811139;
            StanfordId = "anonymous";
        }

        [DataMember]
        public int TeamId { get; set; }

        [DataMember]
        public int EocUserId { get; set; }

        [DataMember]
        public string StanfordId { get; set; }
    }

    [DataContract(Namespace = "")]
    public class MainWindowState
    {
        public const int DefaultQueueSplitterLocation = 289;

        public MainWindowState()
        {
            //LogWindowVisible = false;
            LogWindowHeight = 360;
            QueueWindowVisible = true;
            QueueSplitterLocation = DefaultQueueSplitterLocation;
            SplitterLocation = 360;
        }

        [DataMember]
        public bool LogWindowVisible { get; set; }

        [DataMember]
        public int LogWindowHeight { get; set; }

        [DataMember]
        public bool QueueWindowVisible { get; set; }

        [DataMember]
        public int QueueSplitterLocation { get; set; }

        [DataMember]
        public int SplitterLocation { get; set; }
    }

    [DataContract(Namespace = "")]
    public class MainWindowProperties
    {
        public MainWindowProperties()
        {
            //MinimizeTo = null;
            EnableStats = true;
            //StatsType = null;
        }

        [DataMember]
        public string MinimizeTo { get; set; }

        [DataMember]
        public bool EnableStats { get; set; }

        [DataMember]
        public string StatsType { get; set; }
    }

    [DataContract(Namespace = "")]
    public class MainWindowGridProperties
    {
        public MainWindowGridProperties()
        {
            //TimeFormatting = null;
            OfflineClientsLast = true;
            //UnitTotals = null;
            //DisplayVersions = false;
            //DisplayEtaAsDate = false;
        }

        [DataMember]
        public string TimeFormatting { get; set; }

        [DataMember]
        public bool OfflineClientsLast { get; set; }

        [DataMember]
        public string UnitTotals { get; set; }

        [DataMember]
        public bool DisplayVersions { get; set; }

        [DataMember]
        public bool DisplayEtaAsDate { get; set; }
    }

    [DataContract(Namespace = "")]
    public class BenchmarksGraphing
    {
        public BenchmarksGraphing()
        {
            GraphColors = new List<Color>
            {
                Color.Red,
                Color.Green,
                Color.Blue,
                Color.Chocolate,
                Color.Teal,
                Color.MidnightBlue,
                Color.Maroon,
                Color.DarkOliveGreen,
                Color.Indigo
            };
        }

        [DataMember]
        public List<Color> GraphColors { get; set; }
    }

    [DataContract(Namespace = "")]
    public class HistoryWindowProperties
    {
        public HistoryWindowProperties()
        {
            //BonusCalculation = null;
            MaximumResults = 1000;
        }

        [DataMember]
        public string BonusCalculation { get; set; }

        [DataMember]
        public int MaximumResults { get; set; }
    }

    [DataContract(Namespace = "")]
    public class LogWindowProperties
    {
        public LogWindowProperties()
        {
            ApplyColor = true;
            FollowLog = true;
        }

        [DataMember]
        public bool ApplyColor { get; set; }

        [DataMember]
        public bool FollowLog { get; set; }
    }

    [DataContract(Namespace = "")]
    public class PreferenceData
    {
        public PreferenceData()
        {
            ApplicationSettings = new ApplicationSettings();
            ClientRetrievalTask = new ClientRetrievalTask();
            WebGenerationTask = new WebGenerationTask();
            WebDeployment = new WebDeployment();
            WebRendering = new WebRendering();
            Startup = new Startup();
            WebProxy = new WebProxy();
            Email = new Email();
            Reporting = new Reporting();
            UserSettings = new UserSettings();
            MainWindow = new WindowState();
            MainWindowState = new MainWindowState();
            MainWindowProperties = new MainWindowProperties();
            MainWindowGrid = new DataGridState();
            MainWindowGridProperties = new MainWindowGridProperties();
            BenchmarksWindow = new WindowState();
            BenchmarksGraphing = new BenchmarksGraphing();
            HistoryWindow = new WindowState();
            HistoryWindowGrid = new DataGridState();
            HistoryWindowProperties = new HistoryWindowProperties();
            MessagesWindow = new WindowState();
            LogWindowProperties = new LogWindowProperties();
        }

        [DataMember]
        public string ApplicationVersion { get; set; }

        [DataMember]
        public ApplicationSettings ApplicationSettings { get; set; }

        [DataMember]
        public ClientRetrievalTask ClientRetrievalTask { get; set; }

        [DataMember]
        public WebGenerationTask WebGenerationTask { get; set; }

        [DataMember]
        public WebDeployment WebDeployment { get; set; }

        [DataMember]
        public WebRendering WebRendering { get; set; }

        [DataMember]
        public Startup Startup { get; set; }

        [DataMember]
        public WebProxy WebProxy { get; set; }

        [DataMember]
        public Email Email { get; set; }

        [DataMember]
        public Reporting Reporting { get; set; }

        [DataMember]
        public UserSettings UserSettings { get; set; }

        [DataMember]
        public WindowState MainWindow { get; set; }

        [DataMember]
        public MainWindowState MainWindowState { get; set; }

        [DataMember]
        public MainWindowProperties MainWindowProperties { get; set; }

        [DataMember]
        public DataGridState MainWindowGrid { get; set; }

        [DataMember]
        public MainWindowGridProperties MainWindowGridProperties { get; set; }

        [DataMember]
        public WindowState BenchmarksWindow { get; set; }

        [DataMember]
        public BenchmarksGraphing BenchmarksGraphing { get; set; }

        [DataMember]
        public WindowState HistoryWindow { get; set; }

        [DataMember]
        public DataGridState HistoryWindowGrid { get; set; }

        [DataMember]
        public HistoryWindowProperties HistoryWindowProperties { get; set; }

        [DataMember]
        public WindowState MessagesWindow { get; set; }

        [DataMember]
        public LogWindowProperties LogWindowProperties { get; set; }
    }

    [DataContract(Namespace = "")]
    public class WindowState
    {
        [DataMember]
        public Point Location { get; set; }

        [DataMember]
        public Size Size { get; set; }
    }

    [DataContract(Namespace = "")]
    public class DataGridState
    {
        //public DataGridState()
        //{
        //   //Columns = null;
        //   //SortOrder = default(ListSortDirection);
        //   //SortColumn = null;
        //}

        [DataMember]
        public List<string> Columns { get; set; }

        [DataMember]
        public ListSortDirection SortOrder { get; set; }

        [DataMember]
        public string SortColumn { get; set; }
    }

    [DataContract(Namespace = "")]
    public class ConnectionProperties
    {
        [DataMember]
        public string Address { get; set; }

        [DataMember]
        public int Port { get; set; }

        [DataMember]
        public string Username { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}
