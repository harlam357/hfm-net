using System;
using System.ComponentModel;
using System.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class FahClientSettingsDialog : FormBase, IWin32Dialog
    {
        private readonly FahClientSettingsPresenter _presenter;
        private readonly BindingSource _slotsGridBindingSource;

        public FahClientSettingsDialog(FahClientSettingsPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _slotsGridBindingSource = new BindingSource();

            InitializeComponent();
            EscapeKeyReturnsCancelDialogResult();
            SetupDataGridViewColumns(SlotsDataGridView);
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

        private void FahClientSettingsDialog_Shown(object sender, EventArgs e)
        {
            DummyTextBox.Visible = false;
        }

        private void FahClientSettingsDialog_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(FahClientSettingsModel.Slots))
            {
                RefreshSlotsGrid();
            }
        }

        private void LoadData(FahClientSettingsModel settings)
        {
            ClientNameTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Name), false, DataSourceUpdateMode.OnValidation);
            AddressTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Server), false, DataSourceUpdateMode.OnValidation);
            AddressPortTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Port), false, DataSourceUpdateMode.OnValidation);
            PasswordTextBox.DataBindings.Add(nameof(TextBox.Text), settings, nameof(FahClientSettingsModel.Password), false, DataSourceUpdateMode.OnValidation);
            ConnectButton.DataBindings.Add(nameof(Button.Enabled), settings, nameof(FahClientSettingsModel.ConnectEnabled), false, DataSourceUpdateMode.OnPropertyChanged);
            _slotsGridBindingSource.DataSource = settings.Slots;
            SlotsDataGridView.DataSource = _slotsGridBindingSource;

            _presenter.Model.PropertyChanged += ModelPropertyChanged;
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
