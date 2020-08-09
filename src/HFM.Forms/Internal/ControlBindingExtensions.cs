using System.Windows.Forms;

namespace HFM.Forms.Internal
{
    internal static class ControlBindingExtensions
    {
        internal static void BindText(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(Control.Text), dataSource, dataMember, false, DataSourceUpdateMode.OnValidation);
        }

        internal static void BindEnabled(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(Control.Enabled), dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindVisible(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(Control.Visible), dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindChecked(this CheckBox control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(CheckBox.Checked), dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindChecked(this RadioButton control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(RadioButton.Checked), dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindSelectedValue(this ComboBox control, object dataSource, string dataMember)
        {
            control.DataBindings.Add(nameof(ComboBox.SelectedValue), dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
