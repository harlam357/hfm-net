
using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class WebSettingsModel : INotifyPropertyChanged
    {
        public IPreferenceSet Preferences { get; }

        public WebSettingsModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
            Load();
        }

        public void Load()
        {
            EocUserId = Preferences.Get<int>(Preference.EocUserId);
            StanfordId = Preferences.Get<string>(Preference.StanfordId);
            TeamId = Preferences.Get<int>(Preference.TeamId);
            ProjectDownloadUrl = Preferences.Get<string>(Preference.ProjectDownloadUrl);
            ProxyServer = Preferences.Get<string>(Preference.ProxyServer);
            ProxyPort = Preferences.Get<int>(Preference.ProxyPort);
            UseProxy = Preferences.Get<bool>(Preference.UseProxy);
            ProxyUser = Preferences.Get<string>(Preference.ProxyUser);
            ProxyPass = Preferences.Get<string>(Preference.ProxyPass);
            UseProxyAuth = Preferences.Get<bool>(Preference.UseProxyAuth);
        }

        public void Update()
        {
            Preferences.Set(Preference.EocUserId, EocUserId);
            Preferences.Set(Preference.StanfordId, StanfordId);
            Preferences.Set(Preference.TeamId, TeamId);
            Preferences.Set(Preference.ProjectDownloadUrl, ProjectDownloadUrl);
            Preferences.Set(Preference.ProxyServer, ProxyServer);
            Preferences.Set(Preference.ProxyPort, ProxyPort);
            Preferences.Set(Preference.UseProxy, UseProxy);
            Preferences.Set(Preference.ProxyUser, ProxyUser);
            Preferences.Set(Preference.ProxyPass, ProxyPass);
            Preferences.Set(Preference.UseProxyAuth, UseProxyAuth);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(EocUserId):
                        return ValidateEocUserId() ? null : EocUserIdError;
                    case nameof(StanfordId):
                        return ValidateStanfordId() ? null : StanfordIdError;
                    case nameof(TeamId):
                        return ValidateTeamId() ? null : TeamIdError;
                    case nameof(ProjectDownloadUrl):
                        return ValidateProjectDownloadUrl() ? null : ProjectDownloadUrlError;
                    case nameof(ProxyServer):
                    case nameof(ProxyPort):
                        return ValidateProxyServerPort() ? null : ProxyServerPortError;
                    case nameof(ProxyUser):
                    case nameof(ProxyPass):
                        return ValidateProxyUserPass() ? null : ProxyUserPassError;
                    default:
                        return null;
                }
            }
        }

        public string Error
        {
            get
            {
                var names = new[]
                {
                    nameof(EocUserId),
                    nameof(StanfordId),
                    nameof(TeamId),
                    nameof(ProjectDownloadUrl),
                    nameof(ProxyServer),
                    nameof(ProxyUser)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        public bool HasError => !String.IsNullOrWhiteSpace(Error);

        #region Web Statistics

        private int _eocUserId;

        public int EocUserId
        {
            get { return _eocUserId; }
            set
            {
                if (EocUserId != value)
                {
                    _eocUserId = value;
                    OnPropertyChanged();
                }
            }
        }

        private const string EocUserIdError = "Provide EOC user ID.";

        private bool ValidateEocUserId()
        {
            return EocUserId >= 0;
        }

        private string _stanfordId;

        public string StanfordId
        {
            get { return _stanfordId; }
            set
            {
                if (StanfordId != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _stanfordId = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private const string StanfordIdError = "Provide FAH user ID.";

        private bool ValidateStanfordId()
        {
            return StanfordId.Length != 0;
        }

        private int _teamId;

        public int TeamId
        {
            get { return _teamId; }
            set
            {
                if (TeamId != value)
                {
                    _teamId = value;
                    OnPropertyChanged();
                }
            }
        }

        private const string TeamIdError = "Provide FAH team number.";

        private bool ValidateTeamId()
        {
            return TeamId >= 0;
        }

        #endregion

        #region Project Download URL

        private string _projectDownloadUrl;

        public string ProjectDownloadUrl
        {
            get { return _projectDownloadUrl; }
            set
            {
                if (ProjectDownloadUrl != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _projectDownloadUrl = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private const string ProjectDownloadUrlError = "Provide project summary URL.";

        private bool ValidateProjectDownloadUrl()
        {
            return HttpUrl.Validate(ProjectDownloadUrl);
        }

        #endregion

        #region Web Proxy Settings

        private string _proxyServer;

        public string ProxyServer
        {
            get { return _proxyServer; }
            set
            {
                if (ProxyServer != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _proxyServer = newValue;
                    OnPropertyChanged(nameof(ProxyPort));
                    OnPropertyChanged();
                }
            }
        }

        private int _proxyPort;

        public int ProxyPort
        {
            get { return _proxyPort; }
            set
            {
                if (ProxyPort != value)
                {
                    _proxyPort = value;
                    OnPropertyChanged(nameof(ProxyServer));
                    OnPropertyChanged();
                }
            }
        }

        private string ProxyServerPortError { get; set; }

        private bool ValidateProxyServerPort()
        {
            if (UseProxy == false) return true;

            var result = HostName.ValidateNameAndPort(ProxyServer, ProxyPort, out var message);
            ProxyServerPortError = result ? String.Empty : message;
            return result;
        }

        private bool _useProxy;

        public bool UseProxy
        {
            get { return _useProxy; }
            set
            {
                if (UseProxy != value)
                {
                    _useProxy = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProxyAuthEnabled));
                }
            }
        }

        private string _proxyUser;

        public string ProxyUser
        {
            get { return _proxyUser; }
            set
            {
                if (ProxyUser != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _proxyUser = newValue;
                    OnPropertyChanged(nameof(ProxyPass));
                    OnPropertyChanged();
                }
            }
        }

        private string _proxyPass;

        public string ProxyPass
        {
            get { return _proxyPass; }
            set
            {
                if (ProxyPass != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _proxyPass = newValue;
                    OnPropertyChanged(nameof(ProxyUser));
                    OnPropertyChanged();
                }
            }
        }

        private string ProxyUserPassError { get; set; }
        
        private bool ValidateProxyUserPass()
        {
            if (ProxyAuthEnabled == false) return true;

            var result = NetworkCredentialFactory.ValidateRequired(ProxyUser, ProxyPass, out var message);
            ProxyUserPassError = result ? String.Empty : message;
            return result;
        }

        private bool _useProxyAuth;

        public bool UseProxyAuth
        {
            get { return _useProxyAuth; }
            set
            {
                if (UseProxyAuth != value)
                {
                    _useProxyAuth = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(ProxyAuthEnabled));
                }
            }
        }

        public bool ProxyAuthEnabled
        {
            get { return UseProxy && UseProxyAuth; }
        }

        #endregion

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
