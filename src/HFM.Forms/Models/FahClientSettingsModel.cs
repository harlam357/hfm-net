
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Client;
using HFM.Core.Net;

namespace HFM.Forms.Models
{
    public class FahClientSettingsModel : ViewModelBase, IDataErrorInfo
    {
        private bool _connectEnabled = true;

        public bool ConnectEnabled
        {
            get => _connectEnabled && !HasError;
            set
            {
                if (_connectEnabled != value)
                {
                    _connectEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string this[string columnName]
        {
            get
            {
                switch (columnName)
                {
                    case nameof(Name):
                        return ClientSettings.ValidateName(Name) ? null : NameError;
                    case nameof(Server):
                        return HostName.Validate(Server) ? null : ServerError;
                    case nameof(Port):
                        return TcpPort.Validate(Port) ? null : PortError;
                    default:
                        return null;
                }
            }
        }

        public override string Error
        {
            get
            {
                var names = new[] { nameof(Name), nameof(Server), nameof(Port) };
                var errors = names.Select(x => this[x]).Where(x => x != null);
                return String.Join(Environment.NewLine, errors);
            }
        }

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

        private const string NameError = "Client name can contain only letters, numbers,\r\nand basic symbols (+=-_$&^.[]). I" +
                                         "t must be at\r\nleast three characters long and must not begin or\r\nend with a dot " +
                                         "(.) or a space.";

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

        private const string ServerError = "Must be a valid host name or IP address.";

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

        private static readonly string PortError = $"Must be greater than zero and less than {UInt16.MaxValue}";

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
    }

    public class FahClientSettingsSlotModel
    {
        public string ID { get; set; }

        public string SlotType { get; set; }

        public string ClientType { get; set; }

        public string MaxPacketSize { get; set; }
    }
}
