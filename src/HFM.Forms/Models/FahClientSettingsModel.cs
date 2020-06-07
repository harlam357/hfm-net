
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Client;
using HFM.Core.Net;

namespace HFM.Forms.Models
{
    public class FahClientSettingsModel : INotifyPropertyChanged
    {
        private bool _connectEnabled = true;

        public bool ConnectEnabled
        {
            get => _connectEnabled && !Error;
            set
            {
                if (_connectEnabled != value)
                {
                    _connectEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool ValidateAcceptance()
        {
            OnPropertyChanged(String.Empty);
            return !Error;
        }

        public bool Error =>
            NameError ||
            ServerError ||
            PortError;

        private string _name = String.Empty;

        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged();
                }
            }
        }

        public bool NameError => !ClientSettings.ValidateName(Name);

        private string _server = String.Empty;

        public string Server
        {
            get => _server;
            set
            {
                if (_server != value)
                {
                    _server = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged();
                }
            }
        }

        public bool ServerError => !HostName.Validate(Server);

        private int _port = ClientSettings.DefaultPort;

        public int Port
        {
            get => _port;
            set
            {
                if (_port != value)
                {
                    _port = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool PortError => !TcpPort.Validate(Port);

        private string _password = String.Empty;

        public string Password
        {
            get => _password;
            set
            {
                if (_password != value)
                {
                    _password = value == null ? String.Empty : value.Trim();
                    OnPropertyChanged();
                }
            }
        }

        public Guid Guid { get; set; }

        public ICollection<FahClientSettingsSlotModel> Slots { get; } = new List<FahClientSettingsSlotModel>();

        public void RefreshSlots(SlotCollection slots)
        {
            Slots.Clear();
            foreach (var slot in slots)
            {
                Slots.Add(new FahClientSettingsSlotModel
                {
                    ID = String.Format(CultureInfo.InvariantCulture, "{0:00}", slot.ID),
                    SlotType = SlotTypeConvert.FromSlotOptions(slot.SlotOptions).ToString(),
                    ClientType = slot.SlotOptions[Options.ClientType],
                    MaxPacketSize = slot.SlotOptions[Options.MaxPacketSize]
                });
            }
            OnPropertyChanged(nameof(Slots));
        }

        public virtual FahClientConnection CreateConnection()
        {
            return new FahClientConnection(Server, Port);
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName]string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class FahClientSettingsSlotModel
    {
        public string ID { get; set; }

        public string SlotType { get; set; }

        public string ClientType { get; set; }

        public string MaxPacketSize { get; set; }
    }
}
