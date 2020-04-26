
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Forms.Controls
{
    public class SlotsGridColumnCollection : ReadOnlyCollection<SlotsGridColumn>
    {
        public SlotsGridColumnCollection()
            : base(new SlotsGridColumn[]
            {
                new SlotsGridStatusColumn(),
                new SlotsGridProgressColumn(),
                new SlotsGridDefaultColumn("Name", "Name", nameof(SlotModel.Name)),
                new SlotsGridDefaultColumn("SlotType", "Slot Type", nameof(SlotModel.SlotType)),
                new SlotsGridTPFColumn(),
                new SlotsGridPPDColumn(),
                new SlotsGridETAColumn(),
                new SlotsGridDefaultColumn("Core", "Core", nameof(SlotModel.Core)),
                new SlotsGridDefaultColumn("CoreID", "Core ID", nameof(SlotModel.CoreID)),
                new SlotsGridProjectColumn(),
                new SlotsGridCreditColumn(), 
                new SlotsGridInt32Column("Completed", "Completed", nameof(SlotModel.Completed)),
                new SlotsGridInt32Column("Failed", "Failed", nameof(SlotModel.Failed)),
                new SlotsGridUsernameColumn(),
                new SlotsGridAssignedColumn(),
                new SlotsGridTimeoutColumn()
            })
        {

        }

        public SlotsGridColumn Find(string name)
        {
            return Items.FirstOrDefault(x => x.ColumnName == name);
        }
    }

    public abstract class SlotsGridColumn
    {
        public string ColumnName { get; }
        public string HeaderText { get; }
        public string DataPropertyName { get; }

        protected SlotsGridColumn(string columnName, string headerText, string dataPropertyName)
        {
            ColumnName = columnName;
            HeaderText = headerText;
            DataPropertyName = dataPropertyName;
        }

        public void AddToDataGridView(DataGridView grid)
        {
            int i;
            var column = OnCreateDataGridViewColumn();
            if (column is null)
            {
                i = grid.Columns.Add(ColumnName, HeaderText);
            }
            else
            {
                column.Name = ColumnName;
                column.HeaderText = HeaderText;
                i = grid.Columns.Add(column);
            }

            if (!String.IsNullOrWhiteSpace(DataPropertyName))
            {
                grid.Columns[i].DataPropertyName = DataPropertyName;
            }
        }

        protected virtual DataGridViewColumn OnCreateDataGridViewColumn()
        {
            return null;
        }

        public string GetMouseOverText(SlotModel slotModel)
        {
            if (slotModel is null) return null;
            return OnGetMouseOverText(slotModel);
        }

        protected virtual string OnGetMouseOverText(SlotModel slotModel)
        {
            return null;
        }

        public void PaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            if (e.Value is null) return;
            if (slotModel is null) return;
            OnPaintCell(grid, e, slotModel);
        }

        protected virtual void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(slotModel) ? WarningColor : e.CellStyle.BackColor);
            FillCellBackColor(e, backColor);
            PaintGridLines(e, grid.GridColor);

            var textColor = GetTextColorOrSelectionTextColor(grid, e);
            DrawLeftJustifiedText(e, textColor, GetCellText(e, slotModel));

            e.Handled = true;
        }

        protected static void FillCellBackColor(DataGridViewCellPaintingEventArgs e, Color backColor)
        {
            using (Brush backColorBrush = new SolidBrush(backColor))
            {
                e.Graphics.FillRectangle(backColorBrush, e.CellBounds);
            }
        }

        protected static void PaintGridLines(DataGridViewCellPaintingEventArgs e, Color gridColor)
        {
            using (var gridLinePen = new Pen(gridColor))
            {
                if (Core.Application.IsRunningOnMono)
                {
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                        e.CellBounds.Top, e.CellBounds.Right,
                        e.CellBounds.Top);

                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                        e.CellBounds.Bottom - 1, e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }
                else
                {
                    e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                        e.CellBounds.Bottom - 1, e.CellBounds.Right,
                        e.CellBounds.Bottom - 1);
                }
            }
        }

        protected static Color GetBackColorOrSelectionBackColor(DataGridView grid, DataGridViewCellPaintingEventArgs e, Color backColor)
        {
            return grid.Rows[e.RowIndex].Selected
                ? e.CellStyle.SelectionBackColor
                : backColor;
        }

        protected static Color GetTextColorOrSelectionTextColor(DataGridView grid, DataGridViewCellPaintingEventArgs e)
        {
            return grid.Rows[e.RowIndex].Selected
                ? e.CellStyle.SelectionForeColor
                : e.CellStyle.ForeColor;
        }

        private static int TextPadding { get; } = 4;

        protected static void DrawLeftJustifiedText(DataGridViewCellPaintingEventArgs e, Color color, string value)
        {
            var pt = new Point(e.CellBounds.X + TextPadding, e.CellBounds.Y + 2);
            TextRenderer.DrawText(e.Graphics, value, e.CellStyle.Font, pt, color);
        }

        protected static void DrawRightJustifiedText(DataGridViewCellPaintingEventArgs e, Color color, string value)
        {
            var textSize = TextRenderer.MeasureText(e.Graphics, value, e.CellStyle.Font);
            int difference = e.CellBounds.Width - textSize.Width;

            var pt = new Point(e.CellBounds.X + TextPadding, e.CellBounds.Y + 2);
            if (difference > 0)
            {
                pt = new Point(e.CellBounds.X + difference - TextPadding, e.CellBounds.Y + 2);
            }
            TextRenderer.DrawText(e.Graphics, value, e.CellStyle.Font, pt, color);
        }

        public string GetCellText(DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            return OnGetCellText(e.Value, slotModel);
        }

        protected virtual string OnGetCellText(object value, SlotModel slotModel)
        {
            return value?.ToString();
        }

        public bool HasWarning(SlotModel slotModel)
        {
            return OnHasWarning(slotModel);
        }

        protected virtual bool OnHasWarning(SlotModel slotModel)
        {
            return false;
        }

        protected static Color WarningColor => Color.Orange;

        public int GetAutoSizeWidth(DataGridView grid, SlotModel slotModel, int rowIndex, int columnIndex)
        {
            return OnGetAutoSizeWidth(grid, slotModel, rowIndex, columnIndex);
        }

        protected virtual int OnGetAutoSizeWidth(DataGridView grid, SlotModel slotModel, int rowIndex, int columnIndex)
        {
            var value = grid.Rows[rowIndex].Cells[columnIndex].Value;
            var cellText = OnGetCellText(value, slotModel);
            return TextRenderer.MeasureText(cellText, grid.DefaultCellStyle.Font).Width;
        }
    }

    public class SlotsGridDefaultColumn : SlotsGridColumn
    {
        public SlotsGridDefaultColumn(string columnName, string headerText, string dataPropertyName) : base(columnName, headerText, dataPropertyName)
        {

        }
    }

    public class SlotsGridInt32Column : SlotsGridColumn
    {
        public SlotsGridInt32Column(string columnName, string headerText, string dataPropertyName) : base(columnName, headerText, dataPropertyName)
        {

        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.Status.IsOnline()) return String.Empty;
            return base.OnGetCellText(value, slotModel);
        }
    }

    public class SlotsGridStatusColumn : SlotsGridColumn
    {
        public SlotsGridStatusColumn() : base("Status", "Status", nameof(SlotModel.Status))
        {

        }

        protected override string OnGetMouseOverText(SlotModel slotModel)
        {
            return slotModel.Status.ToString();
        }

        protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            FillCellBackColor(e, e.CellStyle.BackColor);
            PaintGridLines(e, grid.GridColor);

            var status = (SlotStatus)e.Value;
            using (var statusBrush = GetStatusBrush(status))
            using (var statusPen = new Pen(statusBrush))
            {
                // draw the inset box
                var statusRectangle = new Rectangle(e.CellBounds.X + 4, e.CellBounds.Y + 4,
                    e.CellBounds.Width - 10, e.CellBounds.Height - 10);

                e.Graphics.DrawRectangle(statusPen, statusRectangle);
                e.Graphics.FillRectangle(statusBrush, statusRectangle);
            }

            e.Handled = true;
        }

        private static SolidBrush GetStatusBrush(SlotStatus status)
        {
            return new SolidBrush(status.GetStatusColor());
        }

        protected override int OnGetAutoSizeWidth(DataGridView grid, SlotModel slotModel, int rowIndex, int columnIndex)
        {
            return 40;
        }
    }

    public class SlotsGridProgressColumn : SlotsGridColumn
    {
        public SlotsGridProgressColumn() : base("Progress", "Progress", nameof(SlotModel.Progress))
        {

        }

        protected override DataGridViewColumn OnCreateDataGridViewColumn()
        {
            return new DataGridViewProgressColumn();
        }

        protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            e.Handled = false;
        }

        protected override int OnGetAutoSizeWidth(DataGridView grid, SlotModel slotModel, int rowIndex, int columnIndex)
        {
            return 50;
        }
    }

    public class SlotsGridTPFColumn : SlotsGridColumn
    {
        public SlotsGridTPFColumn() : base("TPF", "TPF", nameof(SlotModel.TPF))
        {

        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.Status.IsOnline())
            {
                return String.Empty;
            }

            var timeFormatting = slotModel.Prefs.Get<TimeFormatting>(Preference.TimeFormatting);
            if (timeFormatting == TimeFormatting.Format1)
            {
                var timeSpan = (TimeSpan)value;
                string format = "{1:00}min {2:00}sec";
                if (timeSpan.Hours > 0)
                {
                    format = "{0:00}hr {1:00}min {2:00}sec";
                }

                return String.Format(format, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
            }

            return base.OnGetCellText(value, slotModel);
        }
    }

    public class SlotsGridPPDColumn : SlotsGridColumn
    {
        public SlotsGridPPDColumn() : base("PPD", "PPD", nameof(SlotModel.PPD))
        {

        }

        protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(slotModel) ? WarningColor : e.CellStyle.BackColor);
            FillCellBackColor(e, backColor);
            PaintGridLines(e, grid.GridColor);

            var textColor = GetTextColorOrSelectionTextColor(grid, e);
            DrawRightJustifiedText(e, textColor, GetCellText(e, slotModel));

            e.Handled = true;
        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.Status.IsOnline())
            {
                return String.Empty;
            }

            var decimalPlaces = slotModel.Prefs.Get<int>(Preference.DecimalPlaces);
            string format = NumberFormat.Get(decimalPlaces);
            var number = (double)value;
            return number.ToString(format);
        }
    }

    public class SlotsGridETAColumn : SlotsGridColumn
    {
        public SlotsGridETAColumn() : base("ETA", "ETA", nameof(SlotModel.ETA))
        {

        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.Status.IsOnline())
            {
                return String.Empty;
            }

            var timeFormatting = slotModel.Prefs.Get<TimeFormatting>(Preference.TimeFormatting);
            if (timeFormatting == TimeFormatting.Format1)
            {
                var timeSpan = (TimeSpan)value;
                string format = "{1:00}hr {2:00}min";
                if (timeSpan.Days > 0)
                {
                    format = "{0}d {1:00}hr {2:00}min";
                }

                return String.Format(format, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            }

            var etaAsDate = slotModel.Prefs.Get<bool>(Preference.DisplayEtaAsDate);
            if (etaAsDate)
            {
                return slotModel.ETADate.ToString(CultureInfo.CurrentCulture);
            }

            return base.OnGetCellText(value, slotModel);
        }
    }

    public class SlotsGridProjectColumn : SlotsGridColumn
    {
        public SlotsGridProjectColumn() : base("Project", "Project (Run, Clone, Gen)", nameof(SlotModel.ProjectRunCloneGen))
        {

        }

        protected override string OnGetMouseOverText(SlotModel slotModel)
        {
            return HasWarning(slotModel)
                ? "Client is working on the same work unit as another client."
                : base.OnGetMouseOverText(slotModel);
        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.WorkUnitModel.WorkUnit.HasProject())
            {
                return String.Empty;
            }

            return base.OnGetCellText(value, slotModel);
        }

        protected override bool OnHasWarning(SlotModel slotModel)
        {
            return slotModel.ProjectIsDuplicate && slotModel.Prefs.Get<bool>(Preference.DuplicateProjectCheck);
        }
    }

    public class SlotsGridCreditColumn : SlotsGridColumn
    {
        public SlotsGridCreditColumn() : base("Credit", "Credit", nameof(SlotModel.Credit))
        {

        }

        protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, SlotModel slotModel)
        {
            var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(slotModel) ? WarningColor : e.CellStyle.BackColor);
            FillCellBackColor(e, backColor);
            PaintGridLines(e, grid.GridColor);

            var textColor = GetTextColorOrSelectionTextColor(grid, e);
            DrawRightJustifiedText(e, textColor, GetCellText(e, slotModel));

            e.Handled = true;
        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            if (!slotModel.Status.IsOnline())
            {
                return String.Empty;
            }

            var decimalPlaces = slotModel.Prefs.Get<int>(Preference.DecimalPlaces);
            string format = NumberFormat.Get(decimalPlaces);
            var number = (double)value;
            return number.ToString(format);
        }
    }

    public class SlotsGridUsernameColumn : SlotsGridColumn
    {
        public SlotsGridUsernameColumn() : base("Username", "Username", nameof(SlotModel.Username))
        {

        }

        protected override string OnGetMouseOverText(SlotModel slotModel)
        {
            return HasWarning(slotModel)
                ? $"Client's username does not match the username configured in {Core.Application.ShortName} preferences."
                : base.OnGetMouseOverText(slotModel);
        }

        protected override bool OnHasWarning(SlotModel slotModel)
        {
            return !slotModel.UsernameOk;
        }
    }

    public class SlotsGridAssignedColumn : SlotsGridColumn
    {
        public SlotsGridAssignedColumn() : base("Assigned", "Assigned", nameof(SlotModel.Assigned))
        {

        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            var dateTime = (DateTime)value;
            if (dateTime.IsMinValue())
            {
                return String.Empty;
            }

            var timeFormatting = slotModel.Prefs.Get<TimeFormatting>(Preference.TimeFormatting);
            if (timeFormatting == TimeFormatting.Format1)
            {
                var timeSpan = DateTime.Now.Subtract(dateTime);
                string format = "{1:00}hr {2:00}min ago";
                if (timeSpan.Days > 0)
                {
                    format = "{0}d {1:00}hr {2:00}min ago";
                }

                return String.Format(format, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            }

            return dateTime.ToShortStringOrEmpty();
        }
    }

    public class SlotsGridTimeoutColumn : SlotsGridColumn
    {
        // NOTE: still using HFM calculated PreferredDeadline value
        public SlotsGridTimeoutColumn() : base("Timeout", "Timeout", nameof(SlotModel.PreferredDeadline))
        {

        }

        protected override string OnGetCellText(object value, SlotModel slotModel)
        {
            var dateTime = (DateTime)value;
            if (dateTime.IsMinValue())
            {
                return String.Empty;
            }

            var timeFormatting = slotModel.Prefs.Get<TimeFormatting>(Preference.TimeFormatting);
            if (timeFormatting == TimeFormatting.Format1)
            {
                var timeSpan = dateTime.Subtract(DateTime.Now);
                string format = "In {1:00}hr {2:00}min";
                if (timeSpan.Days > 0)
                {
                    format = "In {0}d {1:00}hr {2:00}min";
                }

                return String.Format(format, timeSpan.Days, timeSpan.Hours, timeSpan.Minutes);
            }

            return dateTime.ToShortStringOrEmpty();
        }
    }
}
