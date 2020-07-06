
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
            Preferences = preferences;
        }

        public override void Load()
        {
            ServerSecure = Preferences.Get<bool>(Preference.EmailReportingServerSecure);
            ToAddress = Preferences.Get<string>(Preference.EmailReportingToAddress);
            FromAddress = Preferences.Get<string>(Preference.EmailReportingFromAddress);
            ServerAddress = Preferences.Get<string>(Preference.EmailReportingServerAddress);
            ServerPort = Preferences.Get<int>(Preference.EmailReportingServerPort);
            ServerUsername = Preferences.Get<string>(Preference.EmailReportingServerUsername);
            ServerPassword = Preferences.Get<string>(Preference.EmailReportingServerPassword);
            ReportingEnabled = Preferences.Get<bool>(Preference.EmailReportingEnabled);
            //ReportEuePause = Preferences.Get<bool>(Preference.ReportEuePause);
            //ReportHung = Preferences.Get<bool>(Preference.ReportHung);
        }

        public override void Save()
        {
            Preferences.Set(Preference.EmailReportingServerSecure, ServerSecure);
            Preferences.Set(Preference.EmailReportingToAddress, ToAddress);
            Preferences.Set(Preference.EmailReportingFromAddress, FromAddress);
            Preferences.Set(Preference.EmailReportingServerAddress, ServerAddress);
            Preferences.Set(Preference.EmailReportingServerPort, ServerPort);
            Preferences.Set(Preference.EmailReportingServerUsername, ServerUsername);
            Preferences.Set(Preference.EmailReportingServerPassword, ServerPassword);
            Preferences.Set(Preference.EmailReportingEnabled, ReportingEnabled);
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
                    case nameof(ServerAddress):
                    case nameof(ServerPort):
                        return ValidateServerAddressPort() ? null : ServerAddressPortError;
                    case nameof(ServerUsername):
                    case nameof(ServerPassword):
                        return ValidateServerUsernamePassword() ? null : ServerUsernamePasswordError;
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
                    nameof(ServerAddress),
                    nameof(ServerUsername)
                };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

        #region Email Settings

        private bool _serverSecure;

        public bool ServerSecure
        {
            get { return _serverSecure; }
            set
            {
                if (ServerSecure != value)
                {
                    _serverSecure = value;
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
            if (ReportingEnabled == false) return true;
            if (ToAddress.Length == 0) return false;

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
            if (ReportingEnabled == false) return true;
            if (FromAddress.Length == 0) return false;

            return SendMailService.ValidateEmail(FromAddress);
        }

        private string _serverAddress;

        public string ServerAddress
        {
            get { return _serverAddress; }
            set
            {
                if (ServerAddress != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _serverAddress = newValue;
                    OnPropertyChanged(nameof(ServerPort));
                    OnPropertyChanged();
                }
            }
        }

        private int _serverPort;

        public int ServerPort
        {
            get { return _serverPort; }
            set
            {
                if (ServerPort != value)
                {
                    _serverPort = value;
                    OnPropertyChanged(nameof(ServerAddress));
                    OnPropertyChanged();
                }
            }
        }

        public string ServerAddressPortError { get; private set; }

        private bool ValidateServerAddressPort()
        {
            if (ReportingEnabled == false) return true;

            var result = HostName.ValidateNameAndPort(ServerAddress, ServerPort, out var message);
            ServerAddressPortError = result ? String.Empty : message;
            return result;
        }

        private string _serverUsername;

        public string ServerUsername
        {
            get { return _serverUsername; }
            set
            {
                if (ServerUsername != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _serverUsername = newValue;
                    OnPropertyChanged(nameof(ServerPassword));
                    OnPropertyChanged();
                }
            }
        }

        private string _serverPassword;

        public string ServerPassword
        {
            get { return _serverPassword; }
            set
            {
                if (ServerPassword != value)
                {
                    string newValue = value == null ? String.Empty : value.Trim();
                    _serverPassword = newValue;
                    OnPropertyChanged(nameof(ServerUsername));
                    OnPropertyChanged();
                }
            }
        }

        public string ServerUsernamePasswordError { get; private set; }

        private bool ValidateServerUsernamePassword()
        {
            if (ReportingEnabled == false) return true;

            var result = NetworkCredentialFactory.ValidateOrEmpty(ServerUsername, ServerPassword, out var message);
            ServerUsernamePasswordError = result ? String.Empty : message;
            return result;
        }

        private bool _reportingEnabled;

        public bool ReportingEnabled
        {
            get { return _reportingEnabled; }
            set
            {
                if (ReportingEnabled != value)
                {
                    _reportingEnabled = value;
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
