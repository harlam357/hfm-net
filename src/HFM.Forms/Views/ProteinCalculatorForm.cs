using System;
using System.Linq;
using System.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Internal;

namespace HFM.Forms
{
    public interface IProteinCalculatorView
    {
        event EventHandler Closed;

        void Show(IWin32Window owner);
    }

    public partial class ProteinCalculatorForm : Form, IProteinCalculatorView
    {
        private readonly ProteinCalculatorModel _model;

        public ProteinCalculatorForm(ProteinCalculatorModel model)
        {
            _model = model;

            InitializeComponent();
            DataBind();
        }

        private void DataBind()
        {
            ProjectComboBox.DisplayMember = "DisplayMember";
            ProjectComboBox.ValueMember = "ValueMember";
            ProjectComboBox.DataSource = _model.Projects.Select(x => new ListItem { DisplayMember = x.ToString(), ValueMember = x }).ToList();
            ProjectComboBox.DataBindings.Add("SelectedValue", _model, "SelectedProject", false, DataSourceUpdateMode.OnPropertyChanged);

            TimePerFrameMinuteTextBox.BindText(_model, "TpfMinutes");
            TimePerFrameSecondTextBox.BindText(_model, "TpfSeconds");

            TotalTimeCheckBox.BindChecked(_model, "TotalWuTimeEnabled");
            TotalTimeMinuteTextBox.BindText(_model, "TotalWuTimeMinutes");
            TotalTimeMinuteTextBox.BindEnabled(_model, "TotalWuTimeEnabled");
            TotalTimeSecondTextBox.BindText(_model, "TotalWuTimeSeconds");
            TotalTimeSecondTextBox.BindEnabled(_model, "TotalWuTimeEnabled");

            CoreNameTextBox.BindText(_model, "CoreName");
            SlotTypeTextBox.BindText(_model, "SlotType");
            NumberOfAtomsTextBox.BindText(_model, "NumberOfAtoms");
            CompletionTimeTextBox.BindText(_model, "CompletionTime");

            PreferredDeadlineTextBox.BindText(_model, "PreferredDeadline");
            PreferredDeadlineTextBox.DataBindings.Add("ReadOnly", _model, "PreferredDeadlineIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
            PreferredDeadlineCheckBox.BindChecked(_model, "PreferredDeadlineChecked");

            FinalDeadlineTextBox.BindText(_model, "FinalDeadline");
            FinalDeadlineTextBox.DataBindings.Add("ReadOnly", _model, "FinalDeadlineIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
            FinalDeadlineCheckBox.BindChecked(_model, "FinalDeadlineChecked");

            KFactorTextBox.BindText(_model, "KFactor");
            KFactorTextBox.DataBindings.Add("ReadOnly", _model, "KFactorIsReadOnly", false, DataSourceUpdateMode.OnPropertyChanged);
            KFactorCheckBox.BindChecked(_model, "KFactorChecked");

            BonusMultiplierTextBox.BindText(_model, "BonusMultiplier");
            BaseCreditTextBox.BindText(_model, "BaseCredit");
            BonusCreditTextBox.BindText(_model, "BonusCredit");
            TotalCreditTextBox.BindText(_model, "TotalCredit");
            BasePpdTextBox.BindText(_model, "BasePpd");
            BonusPpdTextBox.BindText(_model, "BonusPpd");
            TotalPpdTextBox.BindText(_model, "TotalPpd");

            if (Core.Application.IsRunningOnMono)
            {
                _model.PropertyChanged += ModelPropertyChangedForMono;
            }
        }

        private void ModelPropertyChangedForMono(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            TimePerFrameMinuteTextBox.Text = _model.TpfMinutes.ToString();
            TimePerFrameSecondTextBox.Text = _model.TpfSeconds.ToString();

            TotalTimeCheckBox.Checked = _model.TotalWuTimeEnabled;
            TotalTimeMinuteTextBox.Text = _model.TotalWuTimeMinutes.ToString();
            TotalTimeMinuteTextBox.Enabled = _model.TotalWuTimeEnabled;
            TotalTimeSecondTextBox.Text = _model.TotalWuTimeSeconds.ToString();
            TotalTimeSecondTextBox.Enabled = _model.TotalWuTimeEnabled;

            CoreNameTextBox.Text = _model.CoreName;
            SlotTypeTextBox.Text = _model.SlotType;
            NumberOfAtomsTextBox.Text = _model.NumberOfAtoms.ToString();
            CompletionTimeTextBox.Text = _model.CompletionTime.ToString();

            PreferredDeadlineTextBox.Text = _model.PreferredDeadline.ToString();
            PreferredDeadlineTextBox.ReadOnly = _model.PreferredDeadlineIsReadOnly;
            PreferredDeadlineCheckBox.Checked = _model.PreferredDeadlineChecked;

            FinalDeadlineTextBox.Text = _model.FinalDeadline.ToString();
            FinalDeadlineTextBox.ReadOnly = _model.FinalDeadlineIsReadOnly;
            FinalDeadlineCheckBox.Checked = _model.FinalDeadlineChecked;

            KFactorTextBox.Text = _model.KFactor.ToString();
            KFactorTextBox.ReadOnly = _model.KFactorIsReadOnly;
            KFactorCheckBox.Checked = _model.KFactorChecked;

            BonusMultiplierTextBox.Text = _model.BonusMultiplier.ToString();
            BaseCreditTextBox.Text = _model.BaseCredit.ToString();
            BonusCreditTextBox.Text = _model.BonusCredit.ToString();
            TotalCreditTextBox.Text = _model.TotalCredit.ToString();
            BasePpdTextBox.Text = _model.BasePpd.ToString();
            BonusPpdTextBox.Text = _model.BonusPpd.ToString();
            TotalPpdTextBox.Text = _model.TotalPpd.ToString();
        }

        private void CalculateButtonClick(object sender, EventArgs e)
        {
            _model.Calculate();
        }
    }
}
