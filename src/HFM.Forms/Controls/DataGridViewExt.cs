using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

namespace HFM.Forms.Controls
{
    public interface IDataGridView
    {
        #region System.Windows.Forms.DataGridView Properties

        object DataSource { get; set; }

        DataGridViewRowCollection Rows { get; }

        DataGridViewColumnCollection Columns { get; }

        #endregion

        #region System.Windows.Forms.DataGridView Events

        event DataGridViewColumnEventHandler ColumnDisplayIndexChanged;

        #endregion

        #region System.Windows.Forms.DataGridView Methods

        DataGridView.HitTestInfo HitTest(int x, int y);

        void Invalidate();

        #endregion

        #region System.Windows.Forms.Control Methods

        System.Drawing.Point PointToScreen(System.Drawing.Point p);

        #endregion
    }

    [ExcludeFromCodeCoverage]
    public partial class DataGridViewExt : DataGridView, IDataGridView
    {
        public DataGridViewExt()
        {
            InitializeComponent();
        }
    }
}
