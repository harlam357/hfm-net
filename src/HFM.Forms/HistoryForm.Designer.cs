using HFM.Forms.Controls;

namespace HFM.Forms
{
   partial class HistoryForm
   {
      /// <summary>
      /// Required designer variable.
      /// </summary>
      private System.ComponentModel.IContainer components = null;

      /// <summary>
      /// Clean up any resources being used.
      /// </summary>
      /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
      protected override void Dispose(bool disposing)
      {
         if (disposing && (components != null))
         {
            components.Dispose();
         }
         base.Dispose(disposing);
      }

      #region Windows Form Designer generated code

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.components = new System.ComponentModel.Container();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
         System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(HistoryForm));
         this.menuStrip1 = new System.Windows.Forms.MenuStrip();
         this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileExport = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuFileSep1 = new System.Windows.Forms.ToolStripSeparator();
         this.mnuFileExit = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuView = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuViewAutoSizeGrid = new System.Windows.Forms.ToolStripMenuItem();
         this.ToolsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.RefreshProjectDataMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.RefreshAllMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.RefreshUnknownMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.RefreshProjectMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.RefreshEntryMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dataGridMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.dataGridDeleteWorkUnitMenuItem = new System.Windows.Forms.ToolStripMenuItem();
         this.dataGridView1 = new HFM.Forms.Controls.HistoryGridViewWrapper();
         this.ResultsLabel = new System.Windows.Forms.Label();
         this.ResultNumberUpDownControl = new System.Windows.Forms.NumericUpDown();
         this.ResultsTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.PpdCalculationLabel = new System.Windows.Forms.Label();
         this.rdoPanelProduction = new harlam357.Windows.Forms.RadioPanel();
         this.PpdCalculationStandardRadioButton = new System.Windows.Forms.RadioButton();
         this.PpdCalculationBonusFrameTimeRadioButton = new System.Windows.Forms.RadioButton();
         this.PpdCalculationBonusDownloadTimeRadioButton = new System.Windows.Forms.RadioButton();
         this.DataViewGroupBox = new System.Windows.Forms.GroupBox();
         this.DataViewNewButton = new System.Windows.Forms.Button();
         this.DataViewComboBox = new System.Windows.Forms.ComboBox();
         this.DataViewDeleteButton = new System.Windows.Forms.Button();
         this.DataViewEditButton = new System.Windows.Forms.Button();
         this.ResultsGroupBox = new System.Windows.Forms.GroupBox();
         this.LastPageButton = new System.Windows.Forms.Button();
         this.FirstPageButton = new System.Windows.Forms.Button();
         this.PageNumberLabel = new System.Windows.Forms.Label();
         this.PageNumberTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.NextPageButton = new System.Windows.Forms.Button();
         this.PreviousPageButton = new System.Windows.Forms.Button();
         this.PageSizeLabel = new System.Windows.Forms.Label();
         this.splitContainerWrapper1 = new System.Windows.Forms.SplitContainer();
         this.menuStrip1.SuspendLayout();
         this.dataGridMenuStrip.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
         ((System.ComponentModel.ISupportInitialize)(this.ResultNumberUpDownControl)).BeginInit();
         this.rdoPanelProduction.SuspendLayout();
         this.DataViewGroupBox.SuspendLayout();
         this.ResultsGroupBox.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerWrapper1)).BeginInit();
         this.splitContainerWrapper1.Panel1.SuspendLayout();
         this.splitContainerWrapper1.Panel2.SuspendLayout();
         this.splitContainerWrapper1.SuspendLayout();
         this.SuspendLayout();
         // 
         // menuStrip1
         // 
         this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.mnuView,
            this.ToolsMenuItem});
         this.menuStrip1.Location = new System.Drawing.Point(0, 0);
         this.menuStrip1.Name = "menuStrip1";
         this.menuStrip1.Size = new System.Drawing.Size(903, 24);
         this.menuStrip1.TabIndex = 1;
         this.menuStrip1.Text = "menuStrip1";
         // 
         // mnuFile
         // 
         this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFileExport,
            this.mnuFileSep1,
            this.mnuFileExit});
         this.mnuFile.Name = "mnuFile";
         this.mnuFile.Size = new System.Drawing.Size(37, 20);
         this.mnuFile.Text = "&File";
         // 
         // mnuFileExport
         // 
         this.mnuFileExport.Name = "mnuFileExport";
         this.mnuFileExport.Size = new System.Drawing.Size(152, 22);
         this.mnuFileExport.Text = "&Export";
         this.mnuFileExport.Click += new System.EventHandler(this.mnuFileExport_Click);
         // 
         // mnuFileSep1
         // 
         this.mnuFileSep1.Name = "mnuFileSep1";
         this.mnuFileSep1.Size = new System.Drawing.Size(149, 6);
         // 
         // mnuFileExit
         // 
         this.mnuFileExit.Name = "mnuFileExit";
         this.mnuFileExit.Size = new System.Drawing.Size(152, 22);
         this.mnuFileExit.Text = "E&xit";
         this.mnuFileExit.Click += new System.EventHandler(this.mnuFileExit_Click);
         // 
         // mnuView
         // 
         this.mnuView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuViewAutoSizeGrid});
         this.mnuView.Name = "mnuView";
         this.mnuView.Size = new System.Drawing.Size(44, 20);
         this.mnuView.Text = "&View";
         // 
         // mnuViewAutoSizeGrid
         // 
         this.mnuViewAutoSizeGrid.Name = "mnuViewAutoSizeGrid";
         this.mnuViewAutoSizeGrid.Size = new System.Drawing.Size(199, 22);
         this.mnuViewAutoSizeGrid.Text = "Auto Size &Grid Columns";
         this.mnuViewAutoSizeGrid.Click += new System.EventHandler(this.mnuViewAutoSizeGrid_Click);
         // 
         // ToolsMenuItem
         // 
         this.ToolsMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RefreshProjectDataMenuItem});
         this.ToolsMenuItem.Name = "ToolsMenuItem";
         this.ToolsMenuItem.Size = new System.Drawing.Size(48, 20);
         this.ToolsMenuItem.Text = "&Tools";
         // 
         // RefreshProjectDataMenuItem
         // 
         this.RefreshProjectDataMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RefreshAllMenuItem,
            this.RefreshUnknownMenuItem,
            this.RefreshProjectMenuItem,
            this.RefreshEntryMenuItem});
         this.RefreshProjectDataMenuItem.Name = "RefreshProjectDataMenuItem";
         this.RefreshProjectDataMenuItem.Size = new System.Drawing.Size(180, 22);
         this.RefreshProjectDataMenuItem.Text = "Refresh &Project Data";
         // 
         // RefreshAllMenuItem
         // 
         this.RefreshAllMenuItem.Name = "RefreshAllMenuItem";
         this.RefreshAllMenuItem.Size = new System.Drawing.Size(158, 22);
         this.RefreshAllMenuItem.Text = "All";
         this.RefreshAllMenuItem.Click += new System.EventHandler(this.RefreshAllMenuItem_Click);
         // 
         // RefreshUnknownMenuItem
         // 
         this.RefreshUnknownMenuItem.Name = "RefreshUnknownMenuItem";
         this.RefreshUnknownMenuItem.Size = new System.Drawing.Size(158, 22);
         this.RefreshUnknownMenuItem.Text = "Unknown";
         this.RefreshUnknownMenuItem.Click += new System.EventHandler(this.RefreshUnknownMenuItem_Click);
         // 
         // RefreshProjectMenuItem
         // 
         this.RefreshProjectMenuItem.Name = "RefreshProjectMenuItem";
         this.RefreshProjectMenuItem.Size = new System.Drawing.Size(158, 22);
         this.RefreshProjectMenuItem.Text = "Project Number";
         this.RefreshProjectMenuItem.Click += new System.EventHandler(this.RefreshProjectMenuItem_Click);
         // 
         // RefreshEntryMenuItem
         // 
         this.RefreshEntryMenuItem.Name = "RefreshEntryMenuItem";
         this.RefreshEntryMenuItem.Size = new System.Drawing.Size(158, 22);
         this.RefreshEntryMenuItem.Text = "Single Entry";
         this.RefreshEntryMenuItem.Click += new System.EventHandler(this.RefreshEntryMenuItem_Click);
         // 
         // dataGridMenuStrip
         // 
         this.dataGridMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dataGridDeleteWorkUnitMenuItem});
         this.dataGridMenuStrip.Name = "dataGridMenuStrip";
         this.dataGridMenuStrip.Size = new System.Drawing.Size(164, 26);
         // 
         // dataGridDeleteWorkUnitMenuItem
         // 
         this.dataGridDeleteWorkUnitMenuItem.Name = "dataGridDeleteWorkUnitMenuItem";
         this.dataGridDeleteWorkUnitMenuItem.Size = new System.Drawing.Size(163, 22);
         this.dataGridDeleteWorkUnitMenuItem.Text = "Delete Work Unit";
         this.dataGridDeleteWorkUnitMenuItem.Click += new System.EventHandler(this.dataGridDeleteWorkUnitMenuItem_Click);
         // 
         // dataGridView1
         // 
         this.dataGridView1.AllowUserToAddRows = false;
         this.dataGridView1.AllowUserToDeleteRows = false;
         this.dataGridView1.AllowUserToOrderColumns = true;
         this.dataGridView1.AllowUserToResizeRows = false;
         dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
         this.dataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
         this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
         dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
         dataGridViewCellStyle2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
         dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
         dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
         this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
         this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
         dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
         dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
         dataGridViewCellStyle3.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
         dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
         dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
         this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
         this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.dataGridView1.Location = new System.Drawing.Point(0, 0);
         this.dataGridView1.MultiSelect = false;
         this.dataGridView1.Name = "dataGridView1";
         this.dataGridView1.ReadOnly = true;
         dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
         dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
         dataGridViewCellStyle4.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
         dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
         dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
         dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
         this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
         this.dataGridView1.RowTemplate.Height = 18;
         this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
         this.dataGridView1.ShowCellToolTips = false;
         this.dataGridView1.Size = new System.Drawing.Size(903, 198);
         this.dataGridView1.TabIndex = 0;
         this.dataGridView1.CellPainting += new System.Windows.Forms.DataGridViewCellPaintingEventHandler(this.dataGridView1_CellPainting);
         this.dataGridView1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.dataGridView1_MouseDown);
         // 
         // ResultsLabel
         // 
         this.ResultsLabel.AutoSize = true;
         this.ResultsLabel.Location = new System.Drawing.Point(9, 57);
         this.ResultsLabel.Name = "ResultsLabel";
         this.ResultsLabel.Size = new System.Drawing.Size(48, 13);
         this.ResultsLabel.TabIndex = 3;
         this.ResultsLabel.Text = "Results: ";
         // 
         // ResultNumberUpDownControl
         // 
         this.ResultNumberUpDownControl.Location = new System.Drawing.Point(194, 54);
         this.ResultNumberUpDownControl.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         this.ResultNumberUpDownControl.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.ResultNumberUpDownControl.Name = "ResultNumberUpDownControl";
         this.ResultNumberUpDownControl.Size = new System.Drawing.Size(46, 20);
         this.ResultNumberUpDownControl.TabIndex = 6;
         this.ResultNumberUpDownControl.Value = new decimal(new int[] {
            1000,
            0,
            0,
            0});
         // 
         // ResultsTextBox
         // 
         this.ResultsTextBox.Location = new System.Drawing.Point(59, 54);
         this.ResultsTextBox.Name = "ResultsTextBox";
         this.ResultsTextBox.ReadOnly = true;
         this.ResultsTextBox.Size = new System.Drawing.Size(65, 20);
         this.ResultsTextBox.TabIndex = 4;
         // 
         // PpdCalculationLabel
         // 
         this.PpdCalculationLabel.AutoSize = true;
         this.PpdCalculationLabel.Location = new System.Drawing.Point(9, 28);
         this.PpdCalculationLabel.Name = "PpdCalculationLabel";
         this.PpdCalculationLabel.Size = new System.Drawing.Size(119, 13);
         this.PpdCalculationLabel.TabIndex = 0;
         this.PpdCalculationLabel.Text = "PPD/Credit Calculation:";
         // 
         // rdoPanelProduction
         // 
         this.rdoPanelProduction.Controls.Add(this.PpdCalculationStandardRadioButton);
         this.rdoPanelProduction.Controls.Add(this.PpdCalculationBonusFrameTimeRadioButton);
         this.rdoPanelProduction.Controls.Add(this.PpdCalculationBonusDownloadTimeRadioButton);
         this.rdoPanelProduction.Location = new System.Drawing.Point(130, 24);
         this.rdoPanelProduction.Name = "rdoPanelProduction";
         this.rdoPanelProduction.Size = new System.Drawing.Size(342, 24);
         this.rdoPanelProduction.TabIndex = 1;
         this.rdoPanelProduction.ValueMember = null;
         // 
         // PpdCalculationStandardRadioButton
         // 
         this.PpdCalculationStandardRadioButton.AutoSize = true;
         this.PpdCalculationStandardRadioButton.Location = new System.Drawing.Point(272, 3);
         this.PpdCalculationStandardRadioButton.Name = "PpdCalculationStandardRadioButton";
         this.PpdCalculationStandardRadioButton.Size = new System.Drawing.Size(68, 17);
         this.PpdCalculationStandardRadioButton.TabIndex = 2;
         this.PpdCalculationStandardRadioButton.TabStop = true;
         this.PpdCalculationStandardRadioButton.Tag = "2";
         this.PpdCalculationStandardRadioButton.Text = "Standard";
         this.PpdCalculationStandardRadioButton.UseVisualStyleBackColor = true;
         // 
         // PpdCalculationBonusFrameTimeRadioButton
         // 
         this.PpdCalculationBonusFrameTimeRadioButton.AutoSize = true;
         this.PpdCalculationBonusFrameTimeRadioButton.Location = new System.Drawing.Point(147, 3);
         this.PpdCalculationBonusFrameTimeRadioButton.Name = "PpdCalculationBonusFrameTimeRadioButton";
         this.PpdCalculationBonusFrameTimeRadioButton.Size = new System.Drawing.Size(119, 17);
         this.PpdCalculationBonusFrameTimeRadioButton.TabIndex = 1;
         this.PpdCalculationBonusFrameTimeRadioButton.TabStop = true;
         this.PpdCalculationBonusFrameTimeRadioButton.Tag = "1";
         this.PpdCalculationBonusFrameTimeRadioButton.Text = "Bonus (Frame Time)";
         this.PpdCalculationBonusFrameTimeRadioButton.UseVisualStyleBackColor = true;
         // 
         // PpdCalculationBonusDownloadTimeRadioButton
         // 
         this.PpdCalculationBonusDownloadTimeRadioButton.AutoSize = true;
         this.PpdCalculationBonusDownloadTimeRadioButton.Location = new System.Drawing.Point(3, 3);
         this.PpdCalculationBonusDownloadTimeRadioButton.Name = "PpdCalculationBonusDownloadTimeRadioButton";
         this.PpdCalculationBonusDownloadTimeRadioButton.Size = new System.Drawing.Size(138, 17);
         this.PpdCalculationBonusDownloadTimeRadioButton.TabIndex = 0;
         this.PpdCalculationBonusDownloadTimeRadioButton.TabStop = true;
         this.PpdCalculationBonusDownloadTimeRadioButton.Tag = "0";
         this.PpdCalculationBonusDownloadTimeRadioButton.Text = "Bonus (Download Time)";
         this.PpdCalculationBonusDownloadTimeRadioButton.UseVisualStyleBackColor = true;
         // 
         // DataViewGroupBox
         // 
         this.DataViewGroupBox.Controls.Add(this.DataViewNewButton);
         this.DataViewGroupBox.Controls.Add(this.DataViewComboBox);
         this.DataViewGroupBox.Controls.Add(this.DataViewDeleteButton);
         this.DataViewGroupBox.Controls.Add(this.DataViewEditButton);
         this.DataViewGroupBox.Location = new System.Drawing.Point(3, 3);
         this.DataViewGroupBox.Name = "DataViewGroupBox";
         this.DataViewGroupBox.Size = new System.Drawing.Size(331, 84);
         this.DataViewGroupBox.TabIndex = 0;
         this.DataViewGroupBox.TabStop = false;
         this.DataViewGroupBox.Text = "Data View";
         // 
         // DataViewNewButton
         // 
         this.DataViewNewButton.Location = new System.Drawing.Point(19, 23);
         this.DataViewNewButton.Name = "DataViewNewButton";
         this.DataViewNewButton.Size = new System.Drawing.Size(95, 23);
         this.DataViewNewButton.TabIndex = 1;
         this.DataViewNewButton.Text = "New";
         this.DataViewNewButton.UseVisualStyleBackColor = true;
         this.DataViewNewButton.Click += new System.EventHandler(this.btnNew_Click);
         // 
         // DataViewComboBox
         // 
         this.DataViewComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.DataViewComboBox.FormattingEnabled = true;
         this.DataViewComboBox.Location = new System.Drawing.Point(19, 52);
         this.DataViewComboBox.Name = "DataViewComboBox";
         this.DataViewComboBox.Size = new System.Drawing.Size(297, 21);
         this.DataViewComboBox.TabIndex = 0;
         // 
         // DataViewDeleteButton
         // 
         this.DataViewDeleteButton.Location = new System.Drawing.Point(221, 23);
         this.DataViewDeleteButton.Name = "DataViewDeleteButton";
         this.DataViewDeleteButton.Size = new System.Drawing.Size(95, 23);
         this.DataViewDeleteButton.TabIndex = 3;
         this.DataViewDeleteButton.Text = "Delete";
         this.DataViewDeleteButton.UseVisualStyleBackColor = true;
         this.DataViewDeleteButton.Click += new System.EventHandler(this.btnDelete_Click);
         // 
         // DataViewEditButton
         // 
         this.DataViewEditButton.Location = new System.Drawing.Point(120, 23);
         this.DataViewEditButton.Name = "DataViewEditButton";
         this.DataViewEditButton.Size = new System.Drawing.Size(95, 23);
         this.DataViewEditButton.TabIndex = 2;
         this.DataViewEditButton.Text = "Edit";
         this.DataViewEditButton.UseVisualStyleBackColor = true;
         this.DataViewEditButton.Click += new System.EventHandler(this.btnEdit_Click);
         // 
         // ResultsGroupBox
         // 
         this.ResultsGroupBox.Controls.Add(this.LastPageButton);
         this.ResultsGroupBox.Controls.Add(this.FirstPageButton);
         this.ResultsGroupBox.Controls.Add(this.PageNumberLabel);
         this.ResultsGroupBox.Controls.Add(this.PageNumberTextBox);
         this.ResultsGroupBox.Controls.Add(this.NextPageButton);
         this.ResultsGroupBox.Controls.Add(this.PreviousPageButton);
         this.ResultsGroupBox.Controls.Add(this.PageSizeLabel);
         this.ResultsGroupBox.Controls.Add(this.rdoPanelProduction);
         this.ResultsGroupBox.Controls.Add(this.PpdCalculationLabel);
         this.ResultsGroupBox.Controls.Add(this.ResultsTextBox);
         this.ResultsGroupBox.Controls.Add(this.ResultNumberUpDownControl);
         this.ResultsGroupBox.Controls.Add(this.ResultsLabel);
         this.ResultsGroupBox.Location = new System.Drawing.Point(340, 3);
         this.ResultsGroupBox.Name = "ResultsGroupBox";
         this.ResultsGroupBox.Size = new System.Drawing.Size(500, 84);
         this.ResultsGroupBox.TabIndex = 7;
         this.ResultsGroupBox.TabStop = false;
         this.ResultsGroupBox.Text = "Results";
         // 
         // LastPageButton
         // 
         this.LastPageButton.Image = global::HFM.Forms.Properties.Resources.DataContainer_MoveLastHS;
         this.LastPageButton.Location = new System.Drawing.Point(460, 51);
         this.LastPageButton.Name = "LastPageButton";
         this.LastPageButton.Size = new System.Drawing.Size(26, 26);
         this.LastPageButton.TabIndex = 12;
         this.LastPageButton.UseVisualStyleBackColor = true;
         this.LastPageButton.Click += new System.EventHandler(this.LastPageButton_Click);
         // 
         // FirstPageButton
         // 
         this.FirstPageButton.Image = global::HFM.Forms.Properties.Resources.DataContainer_MoveFirstHS;
         this.FirstPageButton.Location = new System.Drawing.Point(328, 51);
         this.FirstPageButton.Name = "FirstPageButton";
         this.FirstPageButton.Size = new System.Drawing.Size(26, 26);
         this.FirstPageButton.TabIndex = 8;
         this.FirstPageButton.UseVisualStyleBackColor = true;
         this.FirstPageButton.Click += new System.EventHandler(this.FirstPageButton_Click);
         // 
         // PageNumberLabel
         // 
         this.PageNumberLabel.AutoSize = true;
         this.PageNumberLabel.Location = new System.Drawing.Point(252, 57);
         this.PageNumberLabel.Name = "PageNumberLabel";
         this.PageNumberLabel.Size = new System.Drawing.Size(75, 13);
         this.PageNumberLabel.TabIndex = 7;
         this.PageNumberLabel.Text = "Page Number:";
         // 
         // PageNumberTextBox
         // 
         this.PageNumberTextBox.Location = new System.Drawing.Point(392, 54);
         this.PageNumberTextBox.Name = "PageNumberTextBox";
         this.PageNumberTextBox.Size = new System.Drawing.Size(30, 20);
         this.PageNumberTextBox.TabIndex = 10;
         // 
         // NextPageButton
         // 
         this.NextPageButton.Image = global::HFM.Forms.Properties.Resources.DataContainer_MoveNextHS;
         this.NextPageButton.Location = new System.Drawing.Point(428, 51);
         this.NextPageButton.Name = "NextPageButton";
         this.NextPageButton.Size = new System.Drawing.Size(26, 26);
         this.NextPageButton.TabIndex = 11;
         this.NextPageButton.UseVisualStyleBackColor = true;
         this.NextPageButton.Click += new System.EventHandler(this.NextPageButton_Click);
         // 
         // PreviousPageButton
         // 
         this.PreviousPageButton.Image = global::HFM.Forms.Properties.Resources.DataContainer_MovePreviousHS;
         this.PreviousPageButton.Location = new System.Drawing.Point(360, 51);
         this.PreviousPageButton.Name = "PreviousPageButton";
         this.PreviousPageButton.Size = new System.Drawing.Size(26, 26);
         this.PreviousPageButton.TabIndex = 9;
         this.PreviousPageButton.UseVisualStyleBackColor = true;
         this.PreviousPageButton.Click += new System.EventHandler(this.PreviousPageButton_Click);
         // 
         // PageSizeLabel
         // 
         this.PageSizeLabel.AutoSize = true;
         this.PageSizeLabel.Location = new System.Drawing.Point(133, 57);
         this.PageSizeLabel.Name = "PageSizeLabel";
         this.PageSizeLabel.Size = new System.Drawing.Size(58, 13);
         this.PageSizeLabel.TabIndex = 5;
         this.PageSizeLabel.Text = "Page Size:";
         // 
         // splitContainerWrapper1
         // 
         this.splitContainerWrapper1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerWrapper1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
         this.splitContainerWrapper1.IsSplitterFixed = true;
         this.splitContainerWrapper1.Location = new System.Drawing.Point(0, 24);
         this.splitContainerWrapper1.Name = "splitContainerWrapper1";
         this.splitContainerWrapper1.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainerWrapper1.Panel1
         // 
         this.splitContainerWrapper1.Panel1.Controls.Add(this.DataViewGroupBox);
         this.splitContainerWrapper1.Panel1.Controls.Add(this.ResultsGroupBox);
         // 
         // splitContainerWrapper1.Panel2
         // 
         this.splitContainerWrapper1.Panel2.Controls.Add(this.dataGridView1);
         this.splitContainerWrapper1.Size = new System.Drawing.Size(903, 292);
         this.splitContainerWrapper1.SplitterDistance = 90;
         this.splitContainerWrapper1.TabIndex = 8;
         // 
         // HistoryForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(903, 316);
         this.Controls.Add(this.splitContainerWrapper1);
         this.Controls.Add(this.menuStrip1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(919, 350);
         this.Name = "HistoryForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
         this.Text = "Work Unit History";
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmHistory_FormClosing);
         this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmHistory_FormClosed);
         this.menuStrip1.ResumeLayout(false);
         this.menuStrip1.PerformLayout();
         this.dataGridMenuStrip.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
         ((System.ComponentModel.ISupportInitialize)(this.ResultNumberUpDownControl)).EndInit();
         this.rdoPanelProduction.ResumeLayout(false);
         this.rdoPanelProduction.PerformLayout();
         this.DataViewGroupBox.ResumeLayout(false);
         this.ResultsGroupBox.ResumeLayout(false);
         this.ResultsGroupBox.PerformLayout();
         this.splitContainerWrapper1.Panel1.ResumeLayout(false);
         this.splitContainerWrapper1.Panel2.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.splitContainerWrapper1)).EndInit();
         this.splitContainerWrapper1.ResumeLayout(false);
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.MenuStrip menuStrip1;
      private System.Windows.Forms.ToolStripMenuItem mnuFile;
      private System.Windows.Forms.ToolStripMenuItem mnuView;
      private System.Windows.Forms.ToolStripMenuItem mnuViewAutoSizeGrid;
      private System.Windows.Forms.ContextMenuStrip dataGridMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem dataGridDeleteWorkUnitMenuItem;
      private HistoryGridViewWrapper dataGridView1;
      private System.Windows.Forms.Label ResultsLabel;
      private System.Windows.Forms.NumericUpDown ResultNumberUpDownControl;
      private TextBoxWrapper ResultsTextBox;
      private System.Windows.Forms.Label PpdCalculationLabel;
      private harlam357.Windows.Forms.RadioPanel rdoPanelProduction;
      private System.Windows.Forms.RadioButton PpdCalculationStandardRadioButton;
      private System.Windows.Forms.RadioButton PpdCalculationBonusFrameTimeRadioButton;
      private System.Windows.Forms.RadioButton PpdCalculationBonusDownloadTimeRadioButton;
      private System.Windows.Forms.GroupBox DataViewGroupBox;
      private System.Windows.Forms.Button DataViewNewButton;
      private System.Windows.Forms.ComboBox DataViewComboBox;
      private System.Windows.Forms.Button DataViewDeleteButton;
      private System.Windows.Forms.Button DataViewEditButton;
      private System.Windows.Forms.GroupBox ResultsGroupBox;
      private System.Windows.Forms.SplitContainer splitContainerWrapper1;
      private System.Windows.Forms.ToolStripSeparator mnuFileSep1;
      private System.Windows.Forms.ToolStripMenuItem mnuFileExit;
      private System.Windows.Forms.ToolStripMenuItem ToolsMenuItem;
      private System.Windows.Forms.ToolStripMenuItem RefreshProjectDataMenuItem;
      private System.Windows.Forms.ToolStripMenuItem RefreshAllMenuItem;
      private System.Windows.Forms.ToolStripMenuItem RefreshUnknownMenuItem;
      private System.Windows.Forms.ToolStripMenuItem RefreshProjectMenuItem;
      private System.Windows.Forms.ToolStripMenuItem RefreshEntryMenuItem;
      private System.Windows.Forms.Label PageSizeLabel;
      private System.Windows.Forms.Label PageNumberLabel;
      private TextBoxWrapper PageNumberTextBox;
      private System.Windows.Forms.Button NextPageButton;
      private System.Windows.Forms.Button PreviousPageButton;
      private System.Windows.Forms.Button FirstPageButton;
      private System.Windows.Forms.Button LastPageButton;
      private System.Windows.Forms.ToolStripMenuItem mnuFileExport;
   }
}
