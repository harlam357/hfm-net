
using System;
using System.ComponentModel;
using System.Linq;

using HFM.Core;
using HFM.Core.Client;
using HFM.Core.Net;
using HFM.Core.SlotXml;
using HFM.Preferences;
using HFM.Preferences.Data;

namespace HFM.Forms.Models
{
    public class ScheduledTasksModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public ScheduledTasksModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
        }

        public override void Load()
        {
            var webGenerationTask = Preferences.Get<WebGenerationTask>(Preference.WebGenerationTask);
            GenerateWeb = webGenerationTask.Enabled;
            GenerateInterval = webGenerationTask.Interval;
            WebGenAfterRefresh = webGenerationTask.AfterClientRetrieval;

            WebGenType = Preferences.Get<WebDeploymentType>(Preference.WebDeploymentType);
            WebRoot = Preferences.Get<string>(Preference.WebDeploymentRoot);
            WebGenServer = Preferences.Get<string>(Preference.WebGenServer);
            WebGenPort = Preferences.Get<int>(Preference.WebGenPort);
            WebGenUsername = Preferences.Get<string>(Preference.WebGenUsername);
            WebGenPassword = Preferences.Get<string>(Preference.WebGenPassword);
            CopyHtml = Preferences.Get<bool>(Preference.WebGenCopyHtml);
            CopyXml = Preferences.Get<bool>(Preference.WebGenCopyXml);
            CopyFAHlog = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            FtpMode = Preferences.Get<FtpMode>(Preference.WebGenFtpMode);
            LimitLogSize = Preferences.Get<bool>(Preference.WebGenLimitLogSize);
            LimitLogSizeLength = Preferences.Get<int>(Preference.WebGenLimitLogSizeLength);
        }

        public override void Save()
        {
            var webGenerationTask = new WebGenerationTask
            {
                Enabled = GenerateWeb,
                Interval = GenerateInterval,
                AfterClientRetrieval = WebGenAfterRefresh
            };
            Preferences.Set(Preference.WebGenerationTask, webGenerationTask);

            Preferences.Set(Preference.WebDeploymentType, WebGenType);
            if (WebGenType == WebDeploymentType.Ftp)
            {
                Preferences.Set(Preference.WebDeploymentRoot, Internal.FileSystemPath.AddUnixTrailingSlash(WebRoot));
            }
            else
            {
                Preferences.Set(Preference.WebDeploymentRoot, Internal.FileSystemPath.AddTrailingSlash(WebRoot));
            }
            Preferences.Set(Preference.WebGenServer, WebGenServer);
            Preferences.Set(Preference.WebGenPort, WebGenPort);
            Preferences.Set(Preference.WebGenUsername, WebGenUsername);
            Preferences.Set(Preference.WebGenPassword, WebGenPassword);
            Preferences.Set(Preference.WebGenCopyHtml, CopyHtml);
            Preferences.Set(Preference.WebGenCopyXml, CopyXml);
            Preferences.Set(Preference.WebGenCopyFAHlog, CopyFAHlog);
            Preferences.Set(Preference.WebGenFtpMode, FtpMode);
            Preferences.Set(Preference.WebGenLimitLogSize, LimitLogSize);
            Preferences.Set(Preference.WebGenLimitLogSizeLength, LimitLogSizeLength);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(GenerateInterval):
                        return ValidateGenerateInterval() ? null : GenerateIntervalError;
                    case nameof(WebRoot):
                        return ValidateWebRoot() ? null : WebRootError;
                    case nameof(WebGenServer):
                        return ValidateWebGenServer() ? null : WebGenServerError;
                    case nameof(WebGenPort):
                        return ValidateWebGenPort() ? null : WebGenPortError;
                    case nameof(WebGenUsername):
                    case nameof(WebGenPassword):
                        return ValidateCredentials() ? null : CredentialsError;
                    default:
                        return null;
                }
            }
        }

        public override string Error
        {
            get
            {
                var names = new[]
                {
                    nameof(GenerateInterval),
                    nameof(WebRoot),
                    nameof(WebGenServer),
                    nameof(WebGenPort),
                    nameof(WebGenUsername)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Web Generation

        private WebDeploymentType _webGenType;

        public WebDeploymentType WebGenType
        {
            get { return _webGenType; }
            set
            {
                if (WebGenType != value)
                {
                    _webGenType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FtpModeEnabled));
                    OnPropertyChanged(nameof(BrowseLocalPathEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        private int _generateInterval;

        public int GenerateInterval
        {
            get { return _generateInterval; }
            set
            {
                if (GenerateInterval != value)
                {
                    _generateInterval = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string GenerateIntervalError { get; } = String.Format("Minutes must be a value from {0} to {1}.", ClientScheduledTasks.MinInterval, ClientScheduledTasks.MaxInterval);

        public bool GenerateIntervalEnabled
        {
            get { return GenerateWeb && WebGenAfterRefresh == false; }
        }

        public bool ValidateGenerateInterval()
        {
            return !GenerateIntervalEnabled || ClientScheduledTasks.ValidateInterval(GenerateInterval);
        }

        private bool _webGenAfterRefresh;

        public bool WebGenAfterRefresh
        {
            get { return _webGenAfterRefresh; }
            set
            {
                if (WebGenAfterRefresh != value)
                {
                    _webGenAfterRefresh = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(GenerateIntervalEnabled));
                }
            }
        }

        private string _webRoot;

        public string WebRoot
        {
            get { return _webRoot; }
            set
            {
                if (WebRoot != value)
                {
                    _webRoot = value == null ? String.Empty : Internal.FileSystemPath.AddTrailingSlash(value.Trim());
                    OnPropertyChanged();
                }
            }
        }

        private const string WebRootError = "Web Generation target path must be a valid local path, network (UNC) path,\r\nor Un" +
                                            "ix style path when the upload type is FTP server.";

        public bool ValidateWebRoot()
        {
            if (GenerateWeb == false) return true;

            switch (WebGenType)
            {
                case WebDeploymentType.Path:
                    if (WebRoot.Length < 2)
                    {
                        return false;
                    }
                    return FileSystemPath.Validate(WebRoot);
                case WebDeploymentType.Ftp:
                    return FileSystemPath.ValidateUnix(WebRoot);
                default:
                    return false;
            }
        }

        private string _webGenServer;

        public string WebGenServer
        {
            get { return _webGenServer; }
            set
            {
                if (WebGenServer != value)
                {
                    _webGenServer = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged();
                }
            }
        }

        private const string WebGenServerError = "FTP server must be a valid host name or IP address.";

        private bool ValidateWebGenServer()
        {
            switch (WebGenType)
            {
                case WebDeploymentType.Ftp:
                    return HostName.Validate(WebGenServer);
                default:
                    return true;
            }
        }

        private int _webGenPort;

        public int WebGenPort
        {
            get { return _webGenPort; }
            set
            {
                if (WebGenPort != value)
                {
                    _webGenPort = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string WebGenPortError { get; } = $"Must be greater than zero and less than {UInt16.MaxValue}";

        private bool ValidateWebGenPort()
        {
            switch (WebGenType)
            {
                case WebDeploymentType.Ftp:
                    return TcpPort.Validate(WebGenPort);
                default:
                    return true;
            }
        }

        private string _webGenUsername;

        public string WebGenUsername
        {
            get { return _webGenUsername; }
            set
            {
                if (WebGenUsername != value)
                {
                    _webGenUsername = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged(nameof(WebGenPassword));
                    OnPropertyChanged();
                }
            }
        }

        private string _webGenPassword;

        public string WebGenPassword
        {
            get { return _webGenPassword; }
            set
            {
                if (WebGenPassword != value)
                {
                    _webGenPassword = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged(nameof(WebGenUsername));
                    OnPropertyChanged();
                }
            }
        }

        public string CredentialsError { get; private set; }

        private bool ValidateCredentials()
        {
            switch (WebGenType)
            {
                case WebDeploymentType.Ftp:
                    return ValidateFtpCredentials();
                default:
                    return true;
            }
        }

        private bool ValidateFtpCredentials()
        {
            var result = NetworkCredentialFactory.ValidateRequired(WebGenUsername, WebGenPassword, out var message);
            CredentialsError = result ? String.Empty : message;
            return result;
        }

        private bool _copyHtml;

        public bool CopyHtml
        {
            get { return _copyHtml; }
            set
            {
                if (CopyHtml != value)
                {
                    if (value == false && CopyXml == false)
                    {
                        return;
                    }
                    _copyHtml = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _copyXml;

        public bool CopyXml
        {
            get { return _copyXml; }
            set
            {
                if (CopyXml != value)
                {
                    if (value == false && CopyHtml == false)
                    {
                        CopyHtml = true;
                    }
                    _copyXml = value;
                    OnPropertyChanged();
                }
            }
        }

        // ReSharper disable InconsistentNaming
        private bool _copyFAHlog;

        public bool CopyFAHlog
        // ReSharper restore InconsistentNaming
        {
            get { return _copyFAHlog; }
            set
            {
                if (CopyFAHlog != value)
                {
                    _copyFAHlog = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        private FtpMode _ftpMode;

        public FtpMode FtpMode
        {
            get { return _ftpMode; }
            set
            {
                if (FtpMode != value)
                {
                    _ftpMode = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool FtpModeEnabled
        {
            get { return GenerateWeb && WebGenType == WebDeploymentType.Ftp; }
        }

        public bool BrowseLocalPathEnabled
        {
            get { return GenerateWeb && WebGenType == WebDeploymentType.Path; }
        }

        private bool _limitLogSize;

        public bool LimitLogSize
        {
            get { return _limitLogSize; }
            set
            {
                if (LimitLogSize != value)
                {
                    _limitLogSize = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        public bool LimitLogSizeEnabled
        {
            get { return GenerateWeb && WebGenType == WebDeploymentType.Ftp && CopyFAHlog; }
        }

        private int _limitLogSizeLength;

        public int LimitLogSizeLength
        {
            get { return _limitLogSizeLength; }
            set
            {
                if (LimitLogSizeLength != value)
                {
                    _limitLogSizeLength = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool LimitLogSizeLengthEnabled
        {
            get { return LimitLogSizeEnabled && LimitLogSize; }
        }

        private bool _generateWeb;

        public bool GenerateWeb
        {
            get { return _generateWeb; }
            set
            {
                if (GenerateWeb != value)
                {
                    _generateWeb = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(GenerateIntervalEnabled));
                    OnPropertyChanged(nameof(FtpModeEnabled));
                    OnPropertyChanged(nameof(BrowseLocalPathEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        #endregion
    }
}
