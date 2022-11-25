using System.Collections.ObjectModel;
using System.Globalization;

using HFM.Core.Client;
using HFM.Core.WorkUnits;
using HFM.Preferences;

namespace HFM.Forms.Controls;

public class SlotsGridColumnCollection : Collection<SlotsGridColumn>
{
    public IPreferences Preferences { get; }

    public SlotsGridColumnCollection(IPreferences preferences)
    {
        Preferences = preferences ?? throw new ArgumentNullException(nameof(preferences));
    }

    public void PopulateCollection()
    {
        Add(new SlotsGridStatusColumn());
        Add(new SlotsGridProgressColumn());
        Add(new SlotsGridDefaultColumn("Name", "Name", "Client name and slot ID", nameof(IClientData.Name)));
        Add(new SlotsGridDefaultColumn("SlotType", "Slot Type", "Client slot type (CPU:N or GPU) where N is the number of cpu threads", nameof(IClientData.SlotTypeString)));
        Add(new SlotsGridDefaultColumn("Processor", "Processor", "CPU or GPU processor name", nameof(IClientData.Processor)));
        Add(new SlotsGridTPFColumn());
        Add(new SlotsGridPPDColumn());
        Add(new SlotsGridETAColumn());
        Add(new SlotsGridDefaultColumn("Core", "Core", "Engine used to process the work unit", nameof(IClientData.Core)));
        Add(new SlotsGridProjectColumn());
        Add(new SlotsGridCreditColumn());
        Add(new SlotsGridInt32Column("Completed", "Completed", "Number of completed work units", nameof(IClientData.Completed)));
        Add(new SlotsGridInt32Column("Failed", "Failed", "Number of failed/incomplete work units", nameof(IClientData.Failed)));
        Add(new SlotsGridUsernameColumn());
        Add(new SlotsGridAssignedColumn());
        Add(new SlotsGridTimeoutColumn());
    }

    protected override void InsertItem(int index, SlotsGridColumn item)
    {
        base.InsertItem(index, item);
        if (item is not null)
        {
            item.Preferences = Preferences;
        }
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
    public string MouseOverHeaderText { get; }
    public string DataPropertyName { get; }
    public IPreferences Preferences { get; set; }

    protected SlotsGridColumn(string columnName, string headerText, string mouseOverHeaderText, string dataPropertyName)
    {
        ColumnName = columnName;
        HeaderText = headerText;
        MouseOverHeaderText = mouseOverHeaderText;
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

    public string GetMouseOverText(IClientData clientData)
    {
        if (clientData is null) return null;
        return OnGetMouseOverText(clientData);
    }

    protected virtual string OnGetMouseOverText(IClientData clientData)
    {
        return null;
    }

    public void PaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        if (e.Value is null) return;
        if (clientData is null) return;
        OnPaintCell(grid, e, clientData);
    }

    protected virtual void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(clientData) ? WarningColor : e.CellStyle.BackColor);
        FillCellBackColor(e, backColor);
        PaintGridLines(e, grid.GridColor);

        var textColor = GetTextColorOrSelectionTextColor(grid, e);
        DrawLeftJustifiedText(e, textColor, GetCellText(e, clientData));

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
            e.Graphics.DrawLine(gridLinePen, e.CellBounds.Left,
                e.CellBounds.Bottom - 1, e.CellBounds.Right,
                e.CellBounds.Bottom - 1);
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

    public string GetCellText(DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        return OnGetCellText(e.Value, clientData);
    }

    protected virtual string OnGetCellText(object value, IClientData clientData)
    {
        return value?.ToString();
    }

    public bool HasWarning(IClientData clientData)
    {
        return OnHasWarning(clientData);
    }

    protected virtual bool OnHasWarning(IClientData clientData)
    {
        return false;
    }

    protected static Color WarningColor => Color.Orange;

    public int GetAutoSizeWidth(DataGridView grid, IClientData clientData, int rowIndex, int columnIndex)
    {
        return OnGetAutoSizeWidth(grid, clientData, rowIndex, columnIndex);
    }

    protected virtual int OnGetAutoSizeWidth(DataGridView grid, IClientData clientData, int rowIndex, int columnIndex)
    {
        var value = grid.Rows[rowIndex].Cells[columnIndex].Value;
        var cellText = OnGetCellText(value, clientData);
        return TextRenderer.MeasureText(cellText, grid.DefaultCellStyle.Font).Width;
    }
}

public class SlotsGridDefaultColumn : SlotsGridColumn
{
    public SlotsGridDefaultColumn(string columnName, string headerText, string dataPropertyName)
        : base(columnName, headerText, null, dataPropertyName)
    {

    }

    public SlotsGridDefaultColumn(string columnName, string headerText, string mouseOverHeaderText, string dataPropertyName)
        : base(columnName, headerText, mouseOverHeaderText, dataPropertyName)
    {

    }
}

public class SlotsGridInt32Column : SlotsGridColumn
{
    public SlotsGridInt32Column(string columnName, string headerText, string mouseOverHeaderText, string dataPropertyName)
        : base(columnName, headerText, mouseOverHeaderText, dataPropertyName)
    {

    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.Status.IsOnline()) return String.Empty;
        return base.OnGetCellText(value, clientData);
    }
}

public class SlotsGridStatusColumn : SlotsGridColumn
{
    public SlotsGridStatusColumn() : base("Status", "Status", "Status of the client or slot", nameof(IClientData.Status))
    {

    }

    protected override string OnGetMouseOverText(IClientData clientData)
    {
        return clientData.Status.ToUserString();
    }

    protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
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

    protected override int OnGetAutoSizeWidth(DataGridView grid, IClientData clientData, int rowIndex, int columnIndex)
    {
        return 40;
    }
}

public class SlotsGridProgressColumn : SlotsGridColumn
{
    public SlotsGridProgressColumn() : base("Progress", "Progress", "Work unit progress", nameof(IClientData.PercentComplete))
    {

    }

    protected override DataGridViewColumn OnCreateDataGridViewColumn()
    {
        return new DataGridViewProgressColumn();
    }

    protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        e.Handled = false;
    }

    protected override int OnGetAutoSizeWidth(DataGridView grid, IClientData clientData, int rowIndex, int columnIndex)
    {
        return 50;
    }
}

public class SlotsGridTPFColumn : SlotsGridColumn
{
    public SlotsGridTPFColumn() : base("TPF", "TPF", "Time per frame (1% of progress)", nameof(IClientData.TPF))
    {

    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.Status.IsOnline())
        {
            return String.Empty;
        }

        var timeSpan = (TimeSpan)value;
        var timeFormatting = Preferences.Get<TimeFormatting>(Preference.TimeFormatting);
        if (timeFormatting == TimeFormatting.Format1)
        {
            string format = "{1:00}min {2:00}sec";
            if (timeSpan.Hours > 0)
            {
                format = "{0:00}hr {1:00}min {2:00}sec";
            }

            return String.Format(format, timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
        }

        return timeSpan.Hours > 0
            ? base.OnGetCellText(value, clientData)
            : timeSpan.ToString(@"mm\:ss");
    }
}

public class SlotsGridPPDColumn : SlotsGridColumn
{
    public SlotsGridPPDColumn() : base("PPD", "PPD", "Points per day", nameof(IClientData.PPD))
    {

    }

    protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(clientData) ? WarningColor : e.CellStyle.BackColor);
        FillCellBackColor(e, backColor);
        PaintGridLines(e, grid.GridColor);

        var textColor = GetTextColorOrSelectionTextColor(grid, e);
        DrawRightJustifiedText(e, textColor, GetCellText(e, clientData));

        e.Handled = true;
    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.Status.IsOnline())
        {
            return String.Empty;
        }

        var decimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
        string format = NumberFormat.Get(decimalPlaces);
        var number = (double)value;
        return number.ToString(format);
    }
}

public class SlotsGridETAColumn : SlotsGridColumn
{
    public SlotsGridETAColumn() : base("ETA", "ETA", "Estimated completion of the work unit", nameof(IClientData.ETA))
    {

    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.Status.IsOnline())
        {
            return String.Empty;
        }

        var timeFormatting = Preferences.Get<TimeFormatting>(Preference.TimeFormatting);
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

        var etaAsDate = Preferences.Get<bool>(Preference.DisplayEtaAsDate);
        if (etaAsDate)
        {
            return clientData.ETADate.ToString(CultureInfo.CurrentCulture);
        }

        return base.OnGetCellText(value, clientData);
    }
}

public class SlotsGridProjectColumn : SlotsGridColumn
{
    public SlotsGridProjectColumn() : base("Project", "Project (Run, Clone, Gen)", "Work unit identifier", nameof(IClientData.ProjectRunCloneGen))
    {

    }

    protected override string OnGetMouseOverText(IClientData clientData)
    {
        return HasWarning(clientData)
            ? "Client is working on the same work unit as another client."
            : base.OnGetMouseOverText(clientData);
    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.ProjectInfo.HasProject())
        {
            return String.Empty;
        }

        return base.OnGetCellText(value, clientData);
    }

    protected override bool OnHasWarning(IClientData clientData) =>
        clientData.Errors.GetValue<bool>(ClientProjectIsDuplicateValidationRule.Key) && Preferences.Get<bool>(Preference.DuplicateProjectCheck);
}

public class SlotsGridCreditColumn : SlotsGridColumn
{
    public SlotsGridCreditColumn() : base("Credit", "Credit", "Estimated points for this work unit", nameof(IClientData.Credit))
    {

    }

    protected override void OnPaintCell(DataGridView grid, DataGridViewCellPaintingEventArgs e, IClientData clientData)
    {
        var backColor = GetBackColorOrSelectionBackColor(grid, e, HasWarning(clientData) ? WarningColor : e.CellStyle.BackColor);
        FillCellBackColor(e, backColor);
        PaintGridLines(e, grid.GridColor);

        var textColor = GetTextColorOrSelectionTextColor(grid, e);
        DrawRightJustifiedText(e, textColor, GetCellText(e, clientData));

        e.Handled = true;
    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        if (!clientData.Status.IsOnline())
        {
            return String.Empty;
        }

        var decimalPlaces = Preferences.Get<int>(Preference.DecimalPlaces);
        string format = NumberFormat.Get(decimalPlaces);
        var number = (double)value;
        return number.ToString(format);
    }
}

public class SlotsGridUsernameColumn : SlotsGridColumn
{
    public SlotsGridUsernameColumn() : base("Username", "Username", "Username and team number", nameof(IClientData.Username))
    {

    }

    protected override string OnGetMouseOverText(IClientData clientData)
    {
        return HasWarning(clientData)
            ? $"Client's username does not match the username configured in {Core.Application.Name} preferences."
            : base.OnGetMouseOverText(clientData);
    }

    protected override bool OnHasWarning(IClientData clientData) =>
        !clientData.Errors.GetValue<bool>(ClientUsernameValidationRule.Key);
}

public class SlotsGridAssignedColumn : SlotsGridColumn
{
    public SlotsGridAssignedColumn() : base("Assigned", "Assigned", "When this work unit was assigned", nameof(IClientData.Assigned))
    {

    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        var dateTime = (DateTime)value;
        if (dateTime.IsMinValue())
        {
            return String.Empty;
        }

        var timeFormatting = Preferences.Get<TimeFormatting>(Preference.TimeFormatting);
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
    public SlotsGridTimeoutColumn() : base("Timeout", "Timeout", "When this work unit must be completed to achieve bonus points", nameof(IClientData.PreferredDeadline))
    {

    }

    protected override string OnGetCellText(object value, IClientData clientData)
    {
        var dateTime = (DateTime)value;
        if (dateTime.IsMinValue())
        {
            return String.Empty;
        }

        var timeFormatting = Preferences.Get<TimeFormatting>(Preference.TimeFormatting);
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
