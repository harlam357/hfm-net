using System.ComponentModel;
using System.Diagnostics;

using HFM.Core.Data;
using HFM.Forms.Controls;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class WorkUnitQueryDialog : FormBase, IWin32Dialog
    {
        private readonly WorkUnitQueryPresenter _presenter;

        public WorkUnitQueryDialog(WorkUnitQueryPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            EscapeKeyButton(cancelButton);

            SetupDataGridViewColumns();
            dataGridView1.CellValueChanged += dataGridView1_CellValueChanged;
        }

        private void WorkUnitQueryDialog_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Query);
        }

        private BindingList<WorkUnitQueryParameter> _parametersList;

        private void LoadData(WorkUnitQuery query)
        {
            _parametersList = new BindingList<WorkUnitQueryParameter>(query.Parameters);
            BindNameTextBox(query);
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = _parametersList;
        }

        public void BindNameTextBox(WorkUnitQuery query)
        {
            txtName.DataBindings.Clear();
            txtName.DataBindings.Add("Text", query, "Name", false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void SetupDataGridViewColumns()
        {
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.DisplayedCellsExceptHeaders;

            var queryColumn = new DataGridViewComboBoxColumn();
            var columnChoices = GetQueryFieldChoices();
            queryColumn.Name = "Name";
            queryColumn.HeaderText = "Name";
            queryColumn.DataSource = columnChoices;
            queryColumn.DisplayMember = nameof(ListItem.DisplayMember);
            queryColumn.ValueMember = nameof(ListItem.ValueMember);
            queryColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Column);
            queryColumn.Width = 150;
            dataGridView1.Columns.Add(queryColumn);

            queryColumn = new DataGridViewComboBoxColumn();
            columnChoices = GetOperatorFieldChoices();
            queryColumn.Name = "Operator";
            queryColumn.HeaderText = "Operator";
            queryColumn.DataSource = columnChoices;
            queryColumn.DisplayMember = nameof(ListItem.DisplayMember);
            queryColumn.ValueMember = nameof(ListItem.ValueMember);
            queryColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Operator);
            queryColumn.Width = 175;
            dataGridView1.Columns.Add(queryColumn);

            var valueColumn = new DataGridViewQueryValueColumn();
            valueColumn.Name = "Value";
            valueColumn.HeaderText = "Value";
            valueColumn.DataPropertyName = nameof(WorkUnitQueryParameter.Value);
            valueColumn.DefaultCellStyle.DataSourceNullValue = null;
            //valueColumn.Width = 200;
            valueColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dataGridView1.Columns.Add(valueColumn);
        }

        private static List<ListItem> GetQueryFieldChoices()
        {
            string[] names = Models.WorkUnitHistoryModel.GetColumnNames();

            var columnChoices = new List<ListItem>
            {
                new(names[(int)WorkUnitRowColumn.ProjectID], WorkUnitRowColumn.ProjectID),
                new(names[(int)WorkUnitRowColumn.SlotName], WorkUnitRowColumn.SlotName),
                new(names[(int)WorkUnitRowColumn.ConnectionString], WorkUnitRowColumn.ConnectionString),
                new(names[(int)WorkUnitRowColumn.DonorName], WorkUnitRowColumn.DonorName),
                new(names[(int)WorkUnitRowColumn.DonorTeam], WorkUnitRowColumn.DonorTeam),
                new(names[(int)WorkUnitRowColumn.SlotType], WorkUnitRowColumn.SlotType),
                new(names[(int)WorkUnitRowColumn.Core], WorkUnitRowColumn.Core),
                new(names[(int)WorkUnitRowColumn.CoreVersion], WorkUnitRowColumn.CoreVersion),
                new(names[(int)WorkUnitRowColumn.FrameTime], WorkUnitRowColumn.FrameTime),
                new(names[(int)WorkUnitRowColumn.KFactor], WorkUnitRowColumn.KFactor),
                new(names[(int)WorkUnitRowColumn.PPD], WorkUnitRowColumn.PPD),
                new(names[(int)WorkUnitRowColumn.Assigned], WorkUnitRowColumn.Assigned),
                new(names[(int)WorkUnitRowColumn.Finished], WorkUnitRowColumn.Finished),
                new(names[(int)WorkUnitRowColumn.Credit], WorkUnitRowColumn.Credit),
                new(names[(int)WorkUnitRowColumn.Frames], WorkUnitRowColumn.Frames),
                new(names[(int)WorkUnitRowColumn.FramesCompleted], WorkUnitRowColumn.FramesCompleted),
                new(names[(int)WorkUnitRowColumn.Result], WorkUnitRowColumn.Result),
                new(names[(int)WorkUnitRowColumn.Atoms], WorkUnitRowColumn.Atoms),
                new(names[(int)WorkUnitRowColumn.ProjectRun], WorkUnitRowColumn.ProjectRun),
                new(names[(int)WorkUnitRowColumn.ProjectClone], WorkUnitRowColumn.ProjectClone),
                new(names[(int)WorkUnitRowColumn.ProjectGen], WorkUnitRowColumn.ProjectGen),
                new(names[(int)WorkUnitRowColumn.ClientVersion], WorkUnitRowColumn.ClientVersion),
                new(names[(int)WorkUnitRowColumn.OperatingSystem], WorkUnitRowColumn.OperatingSystem),
                new(names[(int)WorkUnitRowColumn.PlatformImplementation], WorkUnitRowColumn.PlatformImplementation),
                new(names[(int)WorkUnitRowColumn.PlatformProcessor], WorkUnitRowColumn.PlatformProcessor),
                new(names[(int)WorkUnitRowColumn.PlatformThreads], WorkUnitRowColumn.PlatformThreads),
                new(names[(int)WorkUnitRowColumn.DriverVersion], WorkUnitRowColumn.DriverVersion),
                new(names[(int)WorkUnitRowColumn.ComputeVersion], WorkUnitRowColumn.ComputeVersion),
                new(names[(int)WorkUnitRowColumn.CUDAVersion], WorkUnitRowColumn.CUDAVersion),
            };
            return columnChoices;
        }

        private static List<ListItem> GetOperatorFieldChoices()
        {
            var columnChoices = new List<ListItem>
            {
                new("Equal", WorkUnitQueryOperator.Equal),
                new("Not Equal", WorkUnitQueryOperator.NotEqual),
                new("Greater Than", WorkUnitQueryOperator.GreaterThan),
                new("Greater Than Or Equal", WorkUnitQueryOperator.GreaterThanOrEqual),
                new("Less Than", WorkUnitQueryOperator.LessThan),
                new("Less Than Or Equal", WorkUnitQueryOperator.LessThanOrEqual),
                new("Like", WorkUnitQueryOperator.Like),
                new("Not Like", WorkUnitQueryOperator.NotLike)
            };
            return columnChoices;
        }

        private void dataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            // column is query field
            if (e.ColumnIndex == 0)
            {
                // clear the value cell (this works for .NET and Mono)
                dataGridView1["Value", e.RowIndex].Value = null;
            }
        }

        private void addParameterButton_Click(object sender, EventArgs e)
        {
            _presenter.Query.Parameters.Add(new WorkUnitQueryParameter());
            RefreshDisplay();
        }

        private void removeParameterButton_Click(object sender, EventArgs e)
        {
            Debug.Assert(dataGridView1.SelectedCells.Count == 1);
            foreach (DataGridViewCell cell in dataGridView1.SelectedCells)
            {
                _presenter.Query.Parameters.RemoveAt(cell.OwningRow.Index);
            }
            RefreshDisplay();
        }

        private void RefreshDisplay()
        {
            _parametersList.ResetBindings();
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            _presenter.OKClicked();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _presenter.CancelClicked();
        }
    }
}
