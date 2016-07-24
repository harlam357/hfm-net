
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class ApplicationSettings
   {
      public ApplicationSettings()
      {
         CacheFolder = "logcache";
         MessageLevel = 4; // Info
         //AutoSaveConfig = false;
         DecimalPlaces = 1;
         PpdCalculation = PpdCalculationType.LastThreeFrames;
         BonusCalculation = BonusCalculationType.DownloadTime;
         LogFileViewer = "notepad.exe";
         FileExplorer = "explorer.exe";
         ProjectDownloadUrl = "http://assign.stanford.edu/api/project/summary";
         AllowRunningAsync = true;
         DuplicateUserIdCheck = true;
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
      public PpdCalculationType PpdCalculation { get; set; }

      [DataMember]
      public BonusCalculationType BonusCalculation { get; set; }

      [DataMember]
      public string LogFileViewer { get; set; }

      [DataMember]
      public string FileExplorer { get; set; }

      [DataMember]
      public string ProjectDownloadUrl { get; set; }

      [DataMember]
      public bool AllowRunningAsync { get; set; }

      [DataMember]
      public bool DuplicateUserIdCheck { get; set; }

      [DataMember]
      public bool DuplicateProjectCheck { get; set; }
   }

   [DataContract(Namespace = "")]
   public class ClientRetrievalTask : IEquatable<ClientRetrievalTask>
   {
      public ClientRetrievalTask()
      {
         Enabled = true;
         Interval = 15;
         //ProcessingMode = default(ProcessingMode);
      }

      public ClientRetrievalTask(ClientRetrievalTask other)
      {
         Enabled = other.Enabled;
         Interval = other.Interval;
         ProcessingMode = other.ProcessingMode;
      }

      [DataMember]
      public bool Enabled { get; set; }

      [DataMember]
      public int Interval { get; set; }

      [DataMember]
      public ProcessingMode ProcessingMode { get; set; }

      public bool Equals(ClientRetrievalTask other)
      {
         if (ReferenceEquals(null, other))
         {
            return false;
         }
         if (ReferenceEquals(this, other))
         {
            return true;
         }
         return Enabled.Equals(other.Enabled) && Interval == other.Interval && ProcessingMode == other.ProcessingMode;
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj))
         {
            return false;
         }
         if (ReferenceEquals(this, obj))
         {
            return true;
         }
         if (obj.GetType() != GetType())
         {
            return false;
         }
         return Equals((ClientRetrievalTask)obj);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int hashCode = Enabled.GetHashCode();
            hashCode = (hashCode * 397) ^ Interval;
            hashCode = (hashCode * 397) ^ (int)ProcessingMode;
            return hashCode;
         }
      }
   }

   [DataContract(Namespace = "")]
   public class WebGenerationTask : IEquatable<WebGenerationTask>
   {
      public WebGenerationTask()
      {
         //Enabled = false;
         Interval = 15;
         //AfterClientRetrieval = false;
      }

      public WebGenerationTask(WebGenerationTask other)
      {
         Enabled = other.Enabled;
         Interval = other.Interval;
         AfterClientRetrieval = other.AfterClientRetrieval;
      }

      [DataMember]
      public bool Enabled { get; set; }

      [DataMember]
      public int Interval { get; set; }

      [DataMember]
      public bool AfterClientRetrieval { get; set; }

      public bool Equals(WebGenerationTask other)
      {
         if (ReferenceEquals(null, other))
         {
            return false;
         }
         if (ReferenceEquals(this, other))
         {
            return true;
         }
         return Enabled.Equals(other.Enabled) && Interval == other.Interval && AfterClientRetrieval.Equals(other.AfterClientRetrieval);
      }

      public override bool Equals(object obj)
      {
         if (ReferenceEquals(null, obj))
         {
            return false;
         }
         if (ReferenceEquals(this, obj))
         {
            return true;
         }
         if (obj.GetType() != GetType())
         {
            return false;
         }
         return Equals((WebGenerationTask)obj);
      }

      public override int GetHashCode()
      {
         unchecked
         {
            int hashCode = Enabled.GetHashCode();
            hashCode = (hashCode * 397) ^ Interval;
            hashCode = (hashCode * 397) ^ AfterClientRetrieval.GetHashCode();
            return hashCode;
         }
      }
   }

   [DataContract(Namespace = "")]
   public class WebDeployment
   {
      public WebDeployment()
      {
         //DeploymentType = default(WebDeploymentType);
         //DeploymentRoot = null;
         FtpServer = new ConnectionProperties
         {
            //Server = null,
            Port = 21,
            //Username = null,
            //Password = null
         };
         //FtpMode = default(FtpType);
         CopyHtml = true;
         //CopyXml = false;
         CopyLog = true;
         //LogSizeLimitEnabled = false;
         LogSizeLimitedTo = 1024;
      }

      [DataMember]
      public WebDeploymentType DeploymentType { get; set; }

      [DataMember]
      public string DeploymentRoot { get; set; }

      [DataMember]
      public ConnectionProperties FtpServer { get; set; }

      [DataMember]
      public FtpMode FtpMode { get; set; }

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
         MobileOverviewTransform = "WebMobileOverview.xslt";
         SummaryTransform = "WebSummary.xslt";
         MobileSummaryTransform = "WebMobileSummary.xslt";
         SlotTransform = "WebSlot.xslt";
      }

      [DataMember]
      public string StyleSheet { get; set; }

      [DataMember]
      public string OverviewTransform { get; set; }

      [DataMember]
      public string MobileOverviewTransform { get; set; }

      [DataMember]
      public string SummaryTransform { get; set; }

      [DataMember]
      public string MobileSummaryTransform { get; set; }

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
      //public Reporting()
      //{
      //   //EuePauseEnabled = false;
      //   //ClientHungEnabled = false;
      //}

      [DataMember]
      public bool EuePauseEnabled { get; set; }

      [DataMember]
      public bool ClientHungEnabled { get; set; }
   }

   [DataContract(Namespace = "")]
   public class UserSettings
   {
      public UserSettings()
      {
         // TODO: Remove these defaults
         TeamId = 32;
         EocUserId = 136552;
         StanfordId = "harlam357";
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
      public MainWindowState()
      {
         //LogWindowVisible = false;
         LogWindowHeight = 360;
         QueueWindowVisible = true;
         SplitterLocation = 360;
      }

      [DataMember]
      public bool LogWindowVisible { get; set; }

      [DataMember]
      public int LogWindowHeight { get; set; }

      [DataMember]
      public bool QueueWindowVisible { get; set; }

      [DataMember]
      public int SplitterLocation { get; set; }
   }

   [DataContract(Namespace = "")]
   public class MainWindowProperties
   {
      public MainWindowProperties()
      {
         MinimizeTo = MinimizeToOption.SystemTray;
         EnableStats = true;
         //StatsType = default(StatsType);
      }

      [DataMember]
      public MinimizeToOption MinimizeTo { get; set; }

      [DataMember]
      public bool EnableStats { get; set; }

      [DataMember]
      public StatsType StatsType { get; set; }
   }

   [DataContract(Namespace = "")]
   public class MainWindowGridProperties
   {
      public MainWindowGridProperties()
      {
         //TimeFormatting = default(TimeFormatting);
         OfflineClientsLast = true;
         //CompletedCountDisplay = default(CompletedCountDisplayType);
         //DisplayVersions = false;
         //DisplayEtaAsDate = false;
      }

      [DataMember]
      public TimeFormatting TimeFormatting { get; set; }

      [DataMember]
      public bool OfflineClientsLast { get; set; }

      [DataMember]
      public UnitTotalsType UnitTotals { get; set; }

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
         //GraphLayout = default(GraphLayoutType);
         ClientsPerGraph = 5;
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
      public GraphLayoutType GraphLayout { get; set; }

      [DataMember]
      public int ClientsPerGraph { get; set; }

      [DataMember]
      public List<Color> GraphColors { get; set; }
   }

   [DataContract(Namespace = "")]
   public class HistoryWindowProperties
   {
      public HistoryWindowProperties()
      {
         //BonusCalculation = default(BonusCalculationType);
         MaximumResults = 1000;
      }

      [DataMember]
      public BonusCalculationType BonusCalculation { get; set; }

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
         LogWindowProperties =  new LogWindowProperties();
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
