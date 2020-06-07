
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Forms.Models;

namespace HFM.Forms
{
    public partial class FahClientSettingsDialog : FormWrapper, IWin32Dialog
    {
        private readonly FahClientSettingsPresenter _presenter;
        private readonly BindingSource _slotsGridBindingSource;
        private readonly List<IValidatingControl> _validatingControls;

        public FahClientSettingsDialog(FahClientSettingsPresenter presenter)
        {
            _presenter = presenter;
            _slotsGridBindingSource = new BindingSource();

            InitializeComponent();
            SetupDataGridViewColumns(SlotsDataGridView);

            _validatingControls = FindValidatingControls();
            
            DataBind(_presenter.Model);
            _presenter.Model.PropertyChanged += ModelPropertyChanged;
        }

        private static void SetupDataGridViewColumns(DataGridView dgv)
        {
            // ReSharper disable PossibleNullReferenceException
            dgv.Columns.Add("ID", "ID");
            dgv.Columns["ID"].DataPropertyName = "ID";
            dgv.Columns["ID"].Width = 48;
            dgv.Columns.Add("SlotType", "Slot Type");
            dgv.Columns["SlotType"].DataPropertyName = "SlotType";
            dgv.Columns["SlotType"].Width = 100;
            dgv.Columns.Add("ClientType", "Client Type");
            dgv.Columns["ClientType"].DataPropertyName = "ClientType";
            dgv.Columns["ClientType"].Width = 90;
            dgv.Columns.Add("MaxPacketSize", "Packet Size");
            dgv.Columns["MaxPacketSize"].DataPropertyName = "MaxPacketSize";
            dgv.Columns["MaxPacketSize"].Width = 90;
        }

        private void FahClientSettingsDialogShown(object sender, EventArgs e)
        {
            DummyTextBox.Visible = false;
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (String.IsNullOrEmpty(e.PropertyName))
            {
                SetPropertyErrorState();
            }
            else
            {
                SetPropertyErrorState(e.PropertyName, true);
                if (e.PropertyName == nameof(FahClientSettingsModel.Slots))
                {
                    RefreshSlotsGrid();
                }    
            }
        }

        private PropertyDescriptorCollection _propertyCollection;

        private PropertyDescriptorCollection PropertyCollection => 
            _propertyCollection ?? (_propertyCollection = TypeDescriptor.GetProperties(_presenter.Model));

        private void SetPropertyErrorState()
        {
            foreach (PropertyDescriptor property in PropertyCollection)
            {
                SetPropertyErrorState(property.DisplayName, false);
            }
        }

        private void SetPropertyErrorState(string boundProperty, bool showToolTip)
        {
            var errorProperty = PropertyCollection.Find(boundProperty + "Error", false);
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
            var errorState = (bool)errorProperty.GetValue(_presenter.Model);
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

        private void DataBind(FahClientSettingsModel settings)
        {
            ClientNameTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Name), false, DataSourceUpdateMode.OnValidation);
            AddressTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Server), false, DataSourceUpdateMode.OnValidation);
            AddressPortTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Port), false, DataSourceUpdateMode.OnValidation);
            PasswordTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Password), false, DataSourceUpdateMode.OnValidation);
            ConnectButton.DataBindings.Add(nameof(Button.Enabled), settings, nameof(FahClientSettingsModel.ConnectEnabled), false, DataSourceUpdateMode.OnPropertyChanged);
            _slotsGridBindingSource.DataSource = settings.Slots;
            SlotsDataGridView.DataSource = _slotsGridBindingSource;
        }

        private List<IValidatingControl> FindValidatingControls()
        {
            return FindValidatingControls(Controls);
        }

        private static List<IValidatingControl> FindValidatingControls(Control.ControlCollection controls)
        {
            var validatingControls = new List<IValidatingControl>();

            foreach (Control control in controls)
            {
                if (control is IValidatingControl validatingControl)
                {
                    validatingControls.Add(validatingControl);
                }

                validatingControls.AddRange(FindValidatingControls(control.Controls));
            }

            return validatingControls;
        }

        private void ConnectButtonClick(object sender, EventArgs e)
        {
            _presenter.ConnectClicked();
        }

        private void DialogOkButtonClick(object sender, EventArgs e)
        {
            _presenter.OKClicked();
        }

        private void RefreshSlotsGrid()
        {
            _slotsGridBindingSource.ResetBindings(false);
        }
    }
}
