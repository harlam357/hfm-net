using System;
using System.ComponentModel;
using System.Linq;

using HFM.Core.Net;
using HFM.Core.Services;
using HFM.Preferences;

namespace HFM.Forms.Models
{
    public class ReportingModel : ViewModelBase, IDataErrorInfo
    {
        public IPreferenceSet Preferences { get; }

        public ReportingModel(IPreferenceSet preferences)
        {
            Preferences = preferences ?? new InMemoryPreferencesProvider();
        }

        public override void Load()
        {
            IsSecure = Preferences.Get<bool>(Preference.EmailReportingServerSecure);
            ToAddress = Preferences.Get<string>(Preference.EmailReportingToAddress);
            FromAddress = Preferences.Get<string>(Preference.EmailReportingFromAddress);
            Server = Preferences.Get<string>(Preference.EmailReportingServerAddress);
            Port = Preferences.Get<int>(Preference.EmailReportingServerPort);
            Username = Preferences.Get<string>(Preference.EmailReportingServerUsername);
            Password = Preferences.Get<string>(Preference.EmailReportingServerPassword);
            Enabled = Preferences.Get<bool>(Preference.EmailReportingEnabled);
            //ReportEuePause = Preferences.Get<bool>(Preference.ReportEuePause);
            //ReportHung = Preferences.Get<bool>(Preference.ReportHung);
        }

        public override void Save()
        {
            Preferences.Set(Preference.EmailReportingServerSecure, IsSecure);
            Preferences.Set(Preference.EmailReportingToAddress, ToAddress);
            Preferences.Set(Preference.EmailReportingFromAddress, FromAddress);
            Preferences.Set(Preference.EmailReportingServerAddress, Server);
            Preferences.Set(Preference.EmailReportingServerPort, Port);
            Preferences.Set(Preference.EmailReportingServerUsername, Username);
            Preferences.Set(Preference.EmailReportingServerPassword, Password);
            Preferences.Set(Preference.EmailReportingEnabled, Enabled);
            //Preferences.Set(Preference.ReportEuePause, ReportEuePause);
            //Preferences.Set(Preference.ReportHung, ReportHung);
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(ToAddress):
                        return ValidateToAddress() ? null : ToAddressError;
                    case nameof(FromAddress):
                        return ValidateFromAddress() ? null : FromAddressError;
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
                    nameof(ToAddress),
                    nameof(FromAddress),
                    nameof(Server),
                    nameof(Username)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Email Settings

        private bool _isSecure;

        public bool IsSecure
        {
            get { return _isSecure; }
            set
            {
                if (IsSecure != value)
                {
                    _isSecure = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _toAddress;

        public string ToAddress
        {
            get { return _toAddress; }
            set
            {
                if (ToAddress != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _toAddress = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private const string ToAddressError = "Must be a valid e-mail address.";

        public bool ValidateToAddress()
        {
            if (Enabled == false) return true;
            if (String.IsNullOrEmpty(ToAddress)) return false;

            return SendMailService.ValidateEmail(ToAddress);
        }

        private string _fromAddress;

        public string FromAddress
        {
            get { return _fromAddress; }
            set
            {
                if (FromAddress != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _fromAddress = newValue;
                    OnPropertyChanged();
                }
            }
        }

        private const string FromAddressError = "Must be a valid e-mail address.";

        public bool ValidateFromAddress()
        {
            if (Enabled == false) return true;
            if (String.IsNullOrEmpty(FromAddress)) return false;

            return SendMailService.ValidateEmail(FromAddress);
        }

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
            if (Enabled == false) return true;

            var result = NetworkCredentialFactory.ValidateOrEmpty(Username, Password, out var message);
            CredentialsError = result ? String.Empty : message;
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
                }
            }
        }

        #endregion

        //#region Report Selections
        //
        //private bool _reportEuePause;
        //
        //public bool ReportEuePause
        //{
        //   get { return _reportEuePause; }
        //   set
        //   {
        //      if (ReportEuePause != value)
        //      {
        //         _reportEuePause = value;
        //         OnPropertyChanged("ReportEuePause");
        //      }
        //   }
        //}
        //
        //private bool _reportHung;
        //
        //public bool ReportHung
        //{
        //   get { return _reportHung; }
        //   set
        //   {
        //      if (ReportHung != value)
        //      {
        //         _reportHung = value;
        //         OnPropertyChanged("ReportHung");
        //      }
        //   }
        //}
        //
        //#endregion
    }
}
