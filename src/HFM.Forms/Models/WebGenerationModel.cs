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
    public class WebGenerationModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public WebGenerationModel(IPreferenceSet preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public override void Load()
        {
            var webGenerationTask = Preferences.Get<WebGenerationTask>(Preference.WebGenerationTask);
            Enabled = webGenerationTask.Enabled;
            Interval = webGenerationTask.Interval;
            AfterClientRetrieval = webGenerationTask.AfterClientRetrieval;

            WebDeploymentType = Preferences.Get<WebDeploymentType>(Preference.WebDeploymentType);
            Path = Preferences.Get<string>(Preference.WebDeploymentRoot);
            Server = Preferences.Get<string>(Preference.WebGenServer);
            Port = Preferences.Get<int>(Preference.WebGenPort);
            Username = Preferences.Get<string>(Preference.WebGenUsername);
            Password = Preferences.Get<string>(Preference.WebGenPassword);
            CopyHtml = Preferences.Get<bool>(Preference.WebGenCopyHtml);
            CopyXml = Preferences.Get<bool>(Preference.WebGenCopyXml);
            CopyLog = Preferences.Get<bool>(Preference.WebGenCopyFAHlog);
            FtpMode = Preferences.Get<FtpMode>(Preference.WebGenFtpMode);
            LimitLogSize = Preferences.Get<bool>(Preference.WebGenLimitLogSize);
            LimitLogSizeLength = Preferences.Get<int>(Preference.WebGenLimitLogSizeLength);
        }

        public override void Save()
        {
            var webGenerationTask = new WebGenerationTask
            {
                Enabled = Enabled,
                Interval = Interval,
                AfterClientRetrieval = AfterClientRetrieval
            };
            Preferences.Set(Preference.WebGenerationTask, webGenerationTask);

            Preferences.Set(Preference.WebDeploymentType, WebDeploymentType);
            if (WebDeploymentType == WebDeploymentType.Ftp)
            {
                Preferences.Set(Preference.WebDeploymentRoot, Internal.FileSystemPath.AddUnixTrailingSlash(Path));
            }
            else
            {
                Preferences.Set(Preference.WebDeploymentRoot, Internal.FileSystemPath.AddTrailingSlash(Path));
            }
            Preferences.Set(Preference.WebGenServer, Server);
            Preferences.Set(Preference.WebGenPort, Port);
            Preferences.Set(Preference.WebGenUsername, Username);
            Preferences.Set(Preference.WebGenPassword, Password);
            Preferences.Set(Preference.WebGenCopyHtml, CopyHtml);
            Preferences.Set(Preference.WebGenCopyXml, CopyXml);
            Preferences.Set(Preference.WebGenCopyFAHlog, CopyLog);
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
                    case nameof(Interval):
                        return ValidateInterval() ? null : IntervalError;
                    case nameof(Path):
                        return ValidatePath() ? null : PathError;
                    case nameof(Server):
                        return ValidateServer() ? null : ServerError;
                    case nameof(Port):
                        return ValidatePort() ? null : PortError;
                    case nameof(Username):
                    case nameof(Password):
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
                    nameof(Interval),
                    nameof(Path),
                    nameof(Server),
                    nameof(Port),
                    nameof(Username)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Web Generation

        private WebDeploymentType _webDeploymentType;

        public WebDeploymentType WebDeploymentType
        {
            get { return _webDeploymentType; }
            set
            {
                if (WebDeploymentType != value)
                {
                    _webDeploymentType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(FtpModeEnabled));
                    OnPropertyChanged(nameof(BrowsePathEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        private int _interval;

        public int Interval
        {
            get { return _interval; }
            set
            {
                if (Interval != value)
                {
                    _interval = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string IntervalError { get; } = String.Format("Minutes must be a value from {0} to {1}.", ClientScheduledTasks.MinInterval, ClientScheduledTasks.MaxInterval);

        public bool IntervalEnabled
        {
            get { return Enabled && AfterClientRetrieval == false; }
        }

        public bool ValidateInterval()
        {
            return !IntervalEnabled || ClientScheduledTasks.ValidateInterval(Interval);
        }

        private bool _afterClientRetrieval;

        public bool AfterClientRetrieval
        {
            get { return _afterClientRetrieval; }
            set
            {
                if (AfterClientRetrieval != value)
                {
                    _afterClientRetrieval = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IntervalEnabled));
                }
            }
        }

        private string _path;

        public string Path
        {
            get { return _path; }
            set
            {
                if (Path != value)
                {
                    _path = value == null ? String.Empty : Internal.FileSystemPath.AddTrailingSlash(value.Trim());
                    OnPropertyChanged();
                }
            }
        }

        private const string PathError = "Web Generation target path must be a valid local path, network (UNC) path,\r\nor Un" +
                                            "ix style path when the upload type is FTP server.";

        public bool ValidatePath()
        {
            if (Enabled == false) return true;

            switch (WebDeploymentType)
            {
                case WebDeploymentType.Path:
                    if (Path.Length < 2)
                    {
                        return false;
                    }
                    return FileSystemPath.Validate(Path);
                case WebDeploymentType.Ftp:
                    return FileSystemPath.ValidateUnix(Path);
                default:
                    return false;
            }
        }

        private string _server;

        public string Server
        {
            get { return _server; }
            set
            {
                if (Server != value)
                {
                    _server = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged();
                }
            }
        }

        private const string ServerError = "FTP server must be a valid host name or IP address.";

        private bool ValidateServer()
        {
            switch (WebDeploymentType)
            {
                case WebDeploymentType.Ftp:
                    return HostName.Validate(Server);
                default:
                    return true;
            }
        }

        private int _port;

        public int Port
        {
            get { return _port; }
            set
            {
                if (Port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        private static string PortError { get; } = $"Must be greater than zero and less than {UInt16.MaxValue}";

        private bool ValidatePort()
        {
            switch (WebDeploymentType)
            {
                case WebDeploymentType.Ftp:
                    return TcpPort.Validate(Port);
                default:
                    return true;
            }
        }

        private string _username;

        public string Username
        {
            get { return _username; }
            set
            {
                if (Username != value)
                {
                    _username = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged(nameof(Password));
                    OnPropertyChanged();
                }
            }
        }

        private string _password;

        public string Password
        {
            get { return _password; }
            set
            {
                if (Password != value)
                {
                    _password = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged(nameof(Username));
                    OnPropertyChanged();
                }
            }
        }

        public string CredentialsError { get; private set; }

        private bool ValidateCredentials()
        {
            switch (WebDeploymentType)
            {
                case WebDeploymentType.Ftp:
                    return ValidateFtpCredentials();
                default:
                    return true;
            }
        }

        private bool ValidateFtpCredentials()
        {
            var result = NetworkCredentialFactory.ValidateRequired(Username, Password, out var message);
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
        private bool _copyLog;

        public bool CopyLog
        // ReSharper restore InconsistentNaming
        {
            get { return _copyLog; }
            set
            {
                if (CopyLog != value)
                {
                    _copyLog = value;
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
            get { return Enabled && WebDeploymentType == WebDeploymentType.Ftp; }
        }

        public bool BrowsePathEnabled
        {
            get { return Enabled && WebDeploymentType == WebDeploymentType.Path; }
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
            get { return Enabled && WebDeploymentType == WebDeploymentType.Ftp && CopyLog; }
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

        private bool _enabled;

        public bool Enabled
        {
            get { return _enabled; }
            set
            {
                if (Enabled != value)
                {
                    _enabled = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IntervalEnabled));
                    OnPropertyChanged(nameof(FtpModeEnabled));
                    OnPropertyChanged(nameof(BrowsePathEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeEnabled));
                    OnPropertyChanged(nameof(LimitLogSizeLengthEnabled));
                }
            }
        }

        #endregion
    }
}
