
using System;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Net;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class WebProxyModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public WebProxyModel(IPreferenceSet preferences)
        {
            Preferences = preferences;
        }

        public override void Load()
        {
            Server = Preferences.Get<string>(Preference.ProxyServer);
            Port = Preferences.Get<int>(Preference.ProxyPort);
            Enabled = Preferences.Get<bool>(Preference.UseProxy);
            Username = Preferences.Get<string>(Preference.ProxyUser);
            Password = Preferences.Get<string>(Preference.ProxyPass);
            CredentialsEnabled = Preferences.Get<bool>(Preference.UseProxyAuth);
        }

        public override void Save()
        {
            Preferences.Set(Preference.ProxyServer, Server);
            Preferences.Set(Preference.ProxyPort, Port);
            Preferences.Set(Preference.UseProxy, Enabled);
            Preferences.Set(Preference.ProxyUser, Username);
            Preferences.Set(Preference.ProxyPass, Password);
            Preferences.Set(Preference.UseProxyAuth, CredentialsEnabled);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Server):
                    case nameof(Port):
                        return ValidateServerPort() ? null : ServerPortError;
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
                    nameof(Server),
                    nameof(Username)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Web Proxy Settings

        private string _server;

        public string Server
        {
            get { return _server; }
            set
            {
                if (Server != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _server = newValue;
                    OnPropertyChanged(nameof(Port));
                    OnPropertyChanged();
                }
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
                    OnPropertyChanged(nameof(Server));
                    OnPropertyChanged();
                }
            }
        }

        public string ServerPortError { get; private set; }

        private bool ValidateServerPort()
        {
            if (Enabled == false) return true;

            var result = HostName.ValidateNameAndPort(Server, Port, out var message);
            ServerPortError = result ? String.Empty : message;
            return result;
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
                    OnPropertyChanged(nameof(AuthenticationEnabled));
                }
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
                    string newValue = value == null ? String.Empty : value.Trim();
                    _username = newValue;
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
                    string newValue = value == null ? String.Empty : value.Trim();
                    _password = newValue;
                    OnPropertyChanged(nameof(Username));
                    OnPropertyChanged();
                }
            }
        }

        public string CredentialsError { get; private set; }

        private bool ValidateCredentials()
        {
            if (AuthenticationEnabled == false) return true;

            var result = NetworkCredentialFactory.ValidateRequired(Username, Password, out var message);
            CredentialsError = result ? String.Empty : message;
            return result;
        }

        private bool _credentialsEnabled;

        public bool CredentialsEnabled
        {
            get { return _credentialsEnabled; }
            set
            {
                if (CredentialsEnabled != value)
                {
                    _credentialsEnabled = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(AuthenticationEnabled));
                }
            }
        }

        public bool AuthenticationEnabled
        {
            get { return Enabled && CredentialsEnabled; }
        }

        #endregion
    }
}
