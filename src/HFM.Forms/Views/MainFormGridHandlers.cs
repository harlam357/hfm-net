using System;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Forms.Controls;

namespace HFM.Forms.Views
{
    public partial class MainForm
    {
        private int _currentMouseOverRow = -1;
        private int _currentMouseOverColumn = -1;

        private void DataGridViewMouseMove(object sender, MouseEventArgs e)
        {
            var info = dataGridView1.HitTest(e.X, e.Y);

            // only draw again if the cell changes
            if (info.RowIndex == _currentMouseOverRow && info.ColumnIndex == _currentMouseOverColumn)
            {
                return;
            }

            _currentMouseOverRow = info.RowIndex;
            _currentMouseOverColumn = info.ColumnIndex;

            string tooltipText;
            var column = SlotsGridColumns.Find(GetColumnName(info.ColumnIndex));
            if (info.Type == DataGridViewHitTestType.ColumnHeader)
            {
                tooltipText = column?.MouseOverHeaderText;
            }
            else
            {
                tooltipText = column?.GetMouseOverText(GetRowSlotModel(info.RowIndex));
            }

            if (String.IsNullOrWhiteSpace(tooltipText))
            {
                toolTipGrid.Hide(dataGridView1);
            }
            else
            {
                toolTipGrid.Show(tooltipText, dataGridView1, e.X + 15, e.Y);
            }
        }

        private void DataGridViewCellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var column = SlotsGridColumns.Find(GetColumnName(e.ColumnIndex));
            column?.PaintCell(dataGridView1, e, GetRowSlotModel(e.RowIndex));
        }

        private void DataGridViewColumnDividerDoubleClick(object sender, DataGridViewColumnDividerDoubleClickEventArgs e)
        {
            AutoSizeColumn(e.ColumnIndex);
            e.Handled = true;
        }

        private void AutoSizeColumn(int columnIndex)
        {
            var column = SlotsGridColumns.Find(GetColumnName(columnIndex));
            if (column is null)
            {
                return;
            }

            int columnWidth = 0;

            using (var g = dataGridView1.CreateGraphics())
            {
                for (int rowIndex = 0; rowIndex < dataGridView1.Rows.Count; rowIndex++)
                {
                    var slotModel = GetRowSlotModel(rowIndex);
                    if (slotModel is null)
                    {
                        continue;
                    }

                    int width = column.GetAutoSizeWidth(dataGridView1, slotModel, rowIndex, columnIndex);
                    if (columnWidth < width)
                    {
                        columnWidth = width + (int)(10 * GetDpiScale(g));
                    }
                }
            }

            var gridColumn = dataGridView1.Columns[columnIndex];
            if (columnWidth >= gridColumn.MinimumWidth)
            {
                gridColumn.Width = columnWidth;
            }
        }

        private static double GetDpiScale(Graphics g)
        {
            return g.DpiX / 96;
        }

        private string GetColumnName(int index)
        {
            if (0 > index || index >= dataGridView1.Columns.Count) return null;

            // ReSharper disable once PossibleNullReferenceException
            return dataGridView1.Columns[index].Name;
        }

        private SlotModel GetRowSlotModel(int index)
        {
            if (0 > index || index >= dataGridView1.Rows.Count) return null;

            return (SlotModel)dataGridView1.Rows[index].DataBoundItem;
        }

        private static SlotsGridColumnCollection SlotsGridColumns { get; } = new SlotsGridColumnCollection();

        public static int NumberOfDisplayFields => SlotsGridColumns.Count + 1;

        public static void SetupDataGridViewColumns(DataGridView dgv)
        {
            SlotsGridColumns.Find("Status").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Progress").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Name").AddToDataGridView(dgv);
            SlotsGridColumns.Find("SlotType").AddToDataGridView(dgv);
            SlotsGridColumns.Find("TPF").AddToDataGridView(dgv);
            SlotsGridColumns.Find("PPD").AddToDataGridView(dgv);
            SlotsGridColumns.Find("ETA").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Core").AddToDataGridView(dgv);
            SlotsGridColumns.Find("CoreID").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Project").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Credit").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Completed").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Failed").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Username").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Assigned").AddToDataGridView(dgv);
            SlotsGridColumns.Find("Timeout").AddToDataGridView(dgv);
            dgv.Columns.Add(String.Empty, String.Empty);
        }
    }
}
