using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;

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
            var columnChoices = new List<ListItem>();
            string[] names = WorkUnitQueryParameter.GetColumnNames();
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectID], WorkUnitRowColumn.ProjectID));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Name], WorkUnitRowColumn.Name));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Path], WorkUnitRowColumn.Path));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Username], WorkUnitRowColumn.Username));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Team], WorkUnitRowColumn.Team));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.SlotType], WorkUnitRowColumn.SlotType));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Core], WorkUnitRowColumn.Core));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.CoreVersion], WorkUnitRowColumn.CoreVersion));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.FrameTime], WorkUnitRowColumn.FrameTime));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.KFactor], WorkUnitRowColumn.KFactor));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.PPD], WorkUnitRowColumn.PPD));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Assigned], WorkUnitRowColumn.Assigned));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Finished], WorkUnitRowColumn.Finished));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Credit], WorkUnitRowColumn.Credit));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Frames], WorkUnitRowColumn.Frames));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.FramesCompleted], WorkUnitRowColumn.FramesCompleted));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Result], WorkUnitRowColumn.Result));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.Atoms], WorkUnitRowColumn.Atoms));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectRun], WorkUnitRowColumn.ProjectRun));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectClone], WorkUnitRowColumn.ProjectClone));
            columnChoices.Add(new ListItem(names[(int)WorkUnitRowColumn.ProjectGen], WorkUnitRowColumn.ProjectGen));
            return columnChoices;
        }

        private static List<ListItem> GetOperatorFieldChoices()
        {
            var columnChoices = new List<ListItem>();
            columnChoices.Add(new ListItem("Equal", WorkUnitQueryOperator.Equal));
            columnChoices.Add(new ListItem("Not Equal", WorkUnitQueryOperator.NotEqual));
            columnChoices.Add(new ListItem("Greater Than", WorkUnitQueryOperator.GreaterThan));
            columnChoices.Add(new ListItem("Greater Than Or Equal", WorkUnitQueryOperator.GreaterThanOrEqual));
            columnChoices.Add(new ListItem("Less Than", WorkUnitQueryOperator.LessThan));
            columnChoices.Add(new ListItem("Less Than Or Equal", WorkUnitQueryOperator.LessThanOrEqual));
            columnChoices.Add(new ListItem("Like", WorkUnitQueryOperator.Like));
            columnChoices.Add(new ListItem("Not Like", WorkUnitQueryOperator.NotLike));
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
