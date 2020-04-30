
using System.ComponentModel;
using System.Windows.Forms;

namespace HFM.Forms
{
    internal static class Extensions
    {
        public static string ToDirectionString(this ListSortDirection direction)
        {
            return direction.Equals(ListSortDirection.Descending) ? "DESC" : "ASC";
        }

        public static void BindText(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Text", dataSource, dataMember, false, DataSourceUpdateMode.OnValidation);
        }

        //public static void BindText(this Control control, object dataSource, string dataMember, DataSourceUpdateMode updateMode)
        //{
        //   control.DataBindings.Add("Text", dataSource, dataMember, false, updateMode);
        //}

        public static void BindEnabled(this Control control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Enabled", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        //public static void BindEnabled(this Control control, object dataSource, string dataMember, DataSourceUpdateMode updateMode)
        //{
        //   control.DataBindings.Add("Enabled", dataSource, dataMember, false, updateMode);
        //}

        public static void BindChecked(this CheckBox control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        public static void BindChecked(this RadioButton control, object dataSource, string dataMember)
        {
            control.DataBindings.Add("Checked", dataSource, dataMember, false, DataSourceUpdateMode.OnPropertyChanged);
        }
    }
}
