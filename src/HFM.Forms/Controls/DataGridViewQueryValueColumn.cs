using System;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    internal class DataGridViewQueryValueColumn : DataGridViewColumn
    {
        public DataGridViewQueryValueColumn()
           : base(new DataGridViewQueryValueTextBoxCell())
        {

        }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                // Ensure that the cell used for the template is a DataGridViewQueryValueTextBoxCell.
                if (value != null && !value.GetType().IsAssignableFrom(typeof(DataGridViewQueryValueTextBoxCell)))
                {
                    throw new InvalidCastException("Must be a ValueCell");
                }
                base.CellTemplate = value;
            }
        }
    }
}
