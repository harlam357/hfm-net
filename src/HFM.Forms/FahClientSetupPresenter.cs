
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Client;
using HFM.Client.ObjectModel;
using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Forms.Models;

namespace HFM.Forms
{
    public interface IFahClientSetupPresenter
    {
        FahClientSettingsModel SettingsModel { get; set; }

        DialogResult ShowDialog(IWin32Window owner);
    }

    public sealed class FahClientSetupPresenter : IFahClientSetupPresenter
    {
        private FahClientSettingsModel _settingsModel;

        public FahClientSettingsModel SettingsModel
        {
            get { return _settingsModel; }
            set
            {
                // remove any event handlers currently attached
                if (_settingsModel != null) _settingsModel.PropertyChanged -= SettingsModelPropertyChanged;

                _settingsModel = value;
                _settingsView.DataBind(_settingsModel);
                _propertyCollection = TypeDescriptor.GetProperties(_settingsModel);
                _settingsModel.PropertyChanged += SettingsModelPropertyChanged;
            }
        }

        private PropertyDescriptorCollection _propertyCollection;

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        private readonly IFahClientSetupView _settingsView;
        private FahClientConnection _connection;
        private readonly IMessageBoxView _messageBoxView;
        private readonly List<IValidatingControl> _validatingControls;

        private SlotCollection _slotCollection;

        public FahClientSetupPresenter(IFahClientSetupView settingsView, IMessageBoxView messageBoxView)
        {
            _settingsView = settingsView;
            _settingsView.AttachPresenter(this);
            _messageBoxView = messageBoxView;
            _validatingControls = _settingsView.FindValidatingControls();
        }

        public DialogResult ShowDialog(IWin32Window owner)
        {
            if (!_settingsModel.Error)
            {
                try
                {
                    Connect();
                }
                catch (Exception ex)
                {
                    _logger.Error(ex.Message, ex);
                }
            }

            return _settingsView.ShowDialog(owner);
        }

        private void SettingsModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            SetPropertyErrorState(e.PropertyName, true);
            if (e.PropertyName.Equals("Slots"))
            {
                _settingsView.RefreshSlotsGrid();
            }
        }

        public void SetPropertyErrorState()
        {
            foreach (PropertyDescriptor property in _propertyCollection)
            {
                SetPropertyErrorState(property.DisplayName, false);
            }
        }

        private void SetPropertyErrorState(string boundProperty, bool showToolTip)
        {
            var errorProperty = _propertyCollection.Find(boundProperty + "Error", false);
            if (errorProperty != null)
            {
                SetPropertyErrorState(boundProperty, errorProperty, showToolTip);
            }
        }

        private void SetPropertyErrorState(string boundProperty, PropertyDescriptor errorProperty, bool showToolTip)
        {
            Debug.Assert(boundProperty != null);
            Debug.Assert(errorProperty != null);

            var validatingControls = FindBoundControls(boundProperty);
            // ReSharper disable PossibleNullReferenceException
            var errorState = (bool)errorProperty.GetValue(SettingsModel);
            // ReSharper restore PossibleNullReferenceException
            foreach (var control in validatingControls)
            {
                control.ErrorState = errorState;
                if (showToolTip) control.ShowToolTip();
            }
        }

        private IEnumerable<IValidatingControl> FindBoundControls(string propertyName)
        {
            return _validatingControls.FindAll(x =>
            {
                if (x.DataBindings["Text"] != null)
                {
                    // ReSharper disable PossibleNullReferenceException
                    return x.DataBindings["Text"].BindingMemberInfo.BindingField == propertyName;
                    // ReSharper restore PossibleNullReferenceException
                }
                return false;
            }).AsReadOnly();
        }

        public async void ConnectClicked()
        {
            try
            {
                Connect();
            }
            catch (Exception ex)
            {
                _messageBoxView.ShowError(_settingsView, ex.Message, Core.Application.NameAndVersion);
            }
        }

        private void Connect()
        {
            _connection?.Dispose();
            _connection = new FahClientConnection(_settingsModel.Server, _settingsModel.Port);
            _connection.Open();
            if (!String.IsNullOrWhiteSpace(_settingsModel.Password))
            {
                _connection.CreateCommand("auth " + _settingsModel.Password).Execute();
            }

            bool connected = _connection.Connected;
            _settingsView.SetConnectButtonEnabled(connected);
            if (connected)
            {
                _connection.CreateCommand("slot-info").Execute();
                var reader = _connection.CreateReader();
                if (reader.Read())
                {
                    _slotCollection = SlotCollection.Load(reader.Message.MessageText);
                    foreach (var slot in _slotCollection)
                    {
                        _connection.CreateCommand(String.Format(CultureInfo.InvariantCulture, FahClientMessages.DefaultSlotOptions, slot.ID)).Execute();
                        reader.Read();

                        var slotOptions = SlotOptions.Load(reader.Message.MessageText);
                        if (slotOptions[Options.MachineID] != null)
                        {
                            slot.SlotOptions = slotOptions;
                        }
                    }

                    _settingsModel.RefreshSlots(_slotCollection);
                }
            }
        }

        public void OkClicked()
        {
            if (ValidateAcceptance())
            {
                _settingsView.DialogResult = DialogResult.OK;
                _settingsView.Close();
            }
        }

        private bool ValidateAcceptance()
        {
            SetPropertyErrorState();
            // Check for error conditions
            if (SettingsModel.Error)
            {
                _messageBoxView.ShowError(_settingsView,
                   "There are validation errors.  Please correct the yellow highlighted fields.",
                      Core.Application.NameAndVersion);
                return false;
            }

            return true;
        }
    }
}
