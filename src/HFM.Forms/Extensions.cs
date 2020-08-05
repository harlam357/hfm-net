
using System.ComponentModel;
using System.Windows.Forms;

namespace HFM.Forms
{
    internal static class Extensions
    {
        internal static string ToDirectionString(this ListSortDirection direction)
        {
            return direction.Equals(ListSortDirection.Descending) ? "DESC" : "ASC";
        }

        internal static void BindText(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Text", dataSource, dataMember, false, DataSourceUpdateMode.OnValidation);
        }

        internal static void BindEnabled(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Enabled", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindChecked(this CheckBox control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        internal static void BindChecked(this RadioButton control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
