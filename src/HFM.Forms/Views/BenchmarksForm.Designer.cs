
namespace HFM.Forms.Views
{
   partial class BenchmarksForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BenchmarksForm));
            this.grpProjectInfo = new System.Windows.Forms.GroupBox();
            this.KFactorTextBox = new System.Windows.Forms.TextBox();
            this.KFactorLabel = new System.Windows.Forms.Label();
            this.WorkServerTextBox = new System.Windows.Forms.TextBox();
            this.ContactTextBox = new System.Windows.Forms.TextBox();
            this.ExpirationTextBox = new System.Windows.Forms.TextBox();
            this.TimeoutTextBox = new System.Windows.Forms.TextBox();
            this.CoreTextBox = new System.Windows.Forms.TextBox();
            this.AtomsTextBox = new System.Windows.Forms.TextBox();
            this.FramesTextBox = new System.Windows.Forms.TextBox();
            this.CreditTextBox = new System.Windows.Forms.TextBox();
            this.ProjectIDTextBox = new System.Windows.Forms.TextBox();
            this.DescriptionLinkLabel = new System.Windows.Forms.LinkLabel();
            this.WorkServerLabel = new System.Windows.Forms.Label();
            this.ContactLabel = new System.Windows.Forms.Label();
            this.CoreLabel = new System.Windows.Forms.Label();
            this.ExpirationLabel = new System.Windows.Forms.Label();
            this.TimeoutLabel = new System.Windows.Forms.Label();
            this.AtomsLabel = new System.Windows.Forms.Label();
            this.FramesLabel = new System.Windows.Forms.Label();
            this.CreditLabel = new System.Windows.Forms.Label();
            this.DescriptionLabel = new System.Windows.Forms.Label();
            this.ProjectIDLabel = new System.Windows.Forms.Label();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.splitContainerBench = new System.Windows.Forms.SplitContainer();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabTextBenchmark = new System.Windows.Forms.TabPage();
            this.txtBenchmarks = new System.Windows.Forms.TextBox();
            this.tabGraphConfig = new System.Windows.Forms.TabPage();
            this.grpColors = new System.Windows.Forms.GroupBox();
            this.lstColors = new System.Windows.Forms.ListBox();
            this.picColorPreview = new System.Windows.Forms.PictureBox();
            this.btnMoveColorDown = new System.Windows.Forms.Button();
            this.btnAddColor = new System.Windows.Forms.Button();
            this.btnMoveColorUp = new System.Windows.Forms.Button();
            this.btnDeleteColor = new System.Windows.Forms.Button();
            this.tabGraphFrameTime1 = new System.Windows.Forms.TabPage();
            this.tabGraphPPD1 = new System.Windows.Forms.TabPage();
            this.btnExit = new System.Windows.Forms.Button();
            this.grpClients = new System.Windows.Forms.GroupBox();
            this.picDeleteClient = new System.Windows.Forms.PictureBox();
            this.cboClients = new System.Windows.Forms.ComboBox();
            this.listBox1ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.mnuContextRefreshMinimum = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuContextDeleteProject = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.grpProjectInfo.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBench)).BeginInit();
            this.splitContainerBench.Panel1.SuspendLayout();
            this.splitContainerBench.Panel2.SuspendLayout();
            this.splitContainerBench.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabTextBenchmark.SuspendLayout();
            this.tabGraphConfig.SuspendLayout();
            this.grpColors.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picColorPreview)).BeginInit();
            this.grpClients.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.picDeleteClient)).BeginInit();
            this.listBox1ContextMenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grpProjectInfo
            // 
            this.grpProjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpProjectInfo.BackColor = System.Drawing.SystemColors.Control;
            this.grpProjectInfo.Controls.Add(this.KFactorTextBox);
            this.grpProjectInfo.Controls.Add(this.KFactorLabel);
            this.grpProjectInfo.Controls.Add(this.WorkServerTextBox);
            this.grpProjectInfo.Controls.Add(this.ContactTextBox);
            this.grpProjectInfo.Controls.Add(this.ExpirationTextBox);
            this.grpProjectInfo.Controls.Add(this.TimeoutTextBox);
            this.grpProjectInfo.Controls.Add(this.CoreTextBox);
            this.grpProjectInfo.Controls.Add(this.AtomsTextBox);
            this.grpProjectInfo.Controls.Add(this.FramesTextBox);
            this.grpProjectInfo.Controls.Add(this.CreditTextBox);
            this.grpProjectInfo.Controls.Add(this.ProjectIDTextBox);
            this.grpProjectInfo.Controls.Add(this.DescriptionLinkLabel);
            this.grpProjectInfo.Controls.Add(this.WorkServerLabel);
            this.grpProjectInfo.Controls.Add(this.ContactLabel);
            this.grpProjectInfo.Controls.Add(this.CoreLabel);
            this.grpProjectInfo.Controls.Add(this.ExpirationLabel);
            this.grpProjectInfo.Controls.Add(this.TimeoutLabel);
            this.grpProjectInfo.Controls.Add(this.AtomsLabel);
            this.grpProjectInfo.Controls.Add(this.FramesLabel);
            this.grpProjectInfo.Controls.Add(this.CreditLabel);
            this.grpProjectInfo.Controls.Add(this.DescriptionLabel);
            this.grpProjectInfo.Controls.Add(this.ProjectIDLabel);
            this.grpProjectInfo.Location = new System.Drawing.Point(10, 465);
            this.grpProjectInfo.Margin = new System.Windows.Forms.Padding(10);
            this.grpProjectInfo.Name = "grpProjectInfo";
            this.grpProjectInfo.Size = new System.Drawing.Size(536, 120);
            this.grpProjectInfo.TabIndex = 2;
            this.grpProjectInfo.TabStop = false;
            this.grpProjectInfo.Text = "Project Information";
            // 
            // KFactorTextBox
            // 
            this.KFactorTextBox.Location = new System.Drawing.Point(179, 41);
            this.KFactorTextBox.Name = "KFactorTextBox";
            this.KFactorTextBox.ReadOnly = true;
            this.KFactorTextBox.Size = new System.Drawing.Size(48, 20);
            this.KFactorTextBox.TabIndex = 7;
            this.KFactorTextBox.TabStop = false;
            this.KFactorTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // KFactorLabel
            // 
            this.KFactorLabel.AutoSize = true;
            this.KFactorLabel.Location = new System.Drawing.Point(130, 44);
            this.KFactorLabel.Name = "KFactorLabel";
            this.KFactorLabel.Size = new System.Drawing.Size(47, 13);
            this.KFactorLabel.TabIndex = 6;
            this.KFactorLabel.Text = "KFactor:";
            // 
            // WorkServerTextBox
            // 
            this.WorkServerTextBox.Location = new System.Drawing.Point(429, 89);
            this.WorkServerTextBox.Name = "WorkServerTextBox";
            this.WorkServerTextBox.ReadOnly = true;
            this.WorkServerTextBox.Size = new System.Drawing.Size(95, 20);
            this.WorkServerTextBox.TabIndex = 20;
            this.WorkServerTextBox.TabStop = false;
            this.WorkServerTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ContactTextBox
            // 
            this.ContactTextBox.Location = new System.Drawing.Point(429, 66);
            this.ContactTextBox.Name = "ContactTextBox";
            this.ContactTextBox.ReadOnly = true;
            this.ContactTextBox.Size = new System.Drawing.Size(95, 20);
            this.ContactTextBox.TabIndex = 19;
            this.ContactTextBox.TabStop = false;
            this.ContactTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ExpirationTextBox
            // 
            this.ExpirationTextBox.Location = new System.Drawing.Point(429, 41);
            this.ExpirationTextBox.Name = "ExpirationTextBox";
            this.ExpirationTextBox.ReadOnly = true;
            this.ExpirationTextBox.Size = new System.Drawing.Size(95, 20);
            this.ExpirationTextBox.TabIndex = 18;
            this.ExpirationTextBox.TabStop = false;
            this.ExpirationTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // TimeoutTextBox
            // 
            this.TimeoutTextBox.Location = new System.Drawing.Point(429, 17);
            this.TimeoutTextBox.Name = "TimeoutTextBox";
            this.TimeoutTextBox.ReadOnly = true;
            this.TimeoutTextBox.Size = new System.Drawing.Size(95, 20);
            this.TimeoutTextBox.TabIndex = 17;
            this.TimeoutTextBox.TabStop = false;
            this.TimeoutTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CoreTextBox
            // 
            this.CoreTextBox.Location = new System.Drawing.Point(51, 66);
            this.CoreTextBox.Name = "CoreTextBox";
            this.CoreTextBox.ReadOnly = true;
            this.CoreTextBox.Size = new System.Drawing.Size(122, 20);
            this.CoreTextBox.TabIndex = 10;
            this.CoreTextBox.TabStop = false;
            this.CoreTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // AtomsTextBox
            // 
            this.AtomsTextBox.Location = new System.Drawing.Point(233, 66);
            this.AtomsTextBox.Name = "AtomsTextBox";
            this.AtomsTextBox.ReadOnly = true;
            this.AtomsTextBox.Size = new System.Drawing.Size(92, 20);
            this.AtomsTextBox.TabIndex = 12;
            this.AtomsTextBox.TabStop = false;
            this.AtomsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // FramesTextBox
            // 
            this.FramesTextBox.Location = new System.Drawing.Point(277, 41);
            this.FramesTextBox.Name = "FramesTextBox";
            this.FramesTextBox.ReadOnly = true;
            this.FramesTextBox.Size = new System.Drawing.Size(48, 20);
            this.FramesTextBox.TabIndex = 9;
            this.FramesTextBox.TabStop = false;
            this.FramesTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CreditTextBox
            // 
            this.CreditTextBox.Location = new System.Drawing.Point(51, 41);
            this.CreditTextBox.Name = "CreditTextBox";
            this.CreditTextBox.ReadOnly = true;
            this.CreditTextBox.Size = new System.Drawing.Size(76, 20);
            this.CreditTextBox.TabIndex = 5;
            this.CreditTextBox.TabStop = false;
            this.CreditTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // ProjectIDTextBox
            // 
            this.ProjectIDTextBox.Location = new System.Drawing.Point(75, 17);
            this.ProjectIDTextBox.Name = "ProjectIDTextBox";
            this.ProjectIDTextBox.ReadOnly = true;
            this.ProjectIDTextBox.Size = new System.Drawing.Size(250, 20);
            this.ProjectIDTextBox.TabIndex = 4;
            this.ProjectIDTextBox.TabStop = false;
            this.ProjectIDTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DescriptionLinkLabel
            // 
            this.DescriptionLinkLabel.Location = new System.Drawing.Point(76, 92);
            this.DescriptionLinkLabel.Name = "DescriptionLinkLabel";
            this.DescriptionLinkLabel.Size = new System.Drawing.Size(250, 13);
            this.DescriptionLinkLabel.TabIndex = 21;
            this.DescriptionLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDescription_LinkClicked);
            // 
            // WorkServerLabel
            // 
            this.WorkServerLabel.AutoSize = true;
            this.WorkServerLabel.Location = new System.Drawing.Point(353, 92);
            this.WorkServerLabel.Name = "WorkServerLabel";
            this.WorkServerLabel.Size = new System.Drawing.Size(70, 13);
            this.WorkServerLabel.TabIndex = 16;
            this.WorkServerLabel.Text = "Work Server:";
            // 
            // ContactLabel
            // 
            this.ContactLabel.AutoSize = true;
            this.ContactLabel.Location = new System.Drawing.Point(376, 69);
            this.ContactLabel.Name = "ContactLabel";
            this.ContactLabel.Size = new System.Drawing.Size(47, 13);
            this.ContactLabel.TabIndex = 15;
            this.ContactLabel.Text = "Contact:";
            // 
            // CoreLabel
            // 
            this.CoreLabel.AutoSize = true;
            this.CoreLabel.Location = new System.Drawing.Point(12, 69);
            this.CoreLabel.Name = "CoreLabel";
            this.CoreLabel.Size = new System.Drawing.Size(32, 13);
            this.CoreLabel.TabIndex = 2;
            this.CoreLabel.Text = "Core:";
            // 
            // ExpirationLabel
            // 
            this.ExpirationLabel.AutoSize = true;
            this.ExpirationLabel.Location = new System.Drawing.Point(340, 44);
            this.ExpirationLabel.Name = "ExpirationLabel";
            this.ExpirationLabel.Size = new System.Drawing.Size(83, 13);
            this.ExpirationLabel.TabIndex = 14;
            this.ExpirationLabel.Text = "Expiration Days:";
            // 
            // TimeoutLabel
            // 
            this.TimeoutLabel.AutoSize = true;
            this.TimeoutLabel.Location = new System.Drawing.Point(348, 20);
            this.TimeoutLabel.Name = "TimeoutLabel";
            this.TimeoutLabel.Size = new System.Drawing.Size(75, 13);
            this.TimeoutLabel.TabIndex = 13;
            this.TimeoutLabel.Text = "Timeout Days:";
            // 
            // AtomsLabel
            // 
            this.AtomsLabel.AutoSize = true;
            this.AtomsLabel.Location = new System.Drawing.Point(189, 69);
            this.AtomsLabel.Name = "AtomsLabel";
            this.AtomsLabel.Size = new System.Drawing.Size(39, 13);
            this.AtomsLabel.TabIndex = 11;
            this.AtomsLabel.Text = "Atoms:";
            // 
            // FramesLabel
            // 
            this.FramesLabel.AutoSize = true;
            this.FramesLabel.Location = new System.Drawing.Point(232, 44);
            this.FramesLabel.Name = "FramesLabel";
            this.FramesLabel.Size = new System.Drawing.Size(44, 13);
            this.FramesLabel.TabIndex = 8;
            this.FramesLabel.Text = "Frames:";
            // 
            // CreditLabel
            // 
            this.CreditLabel.AutoSize = true;
            this.CreditLabel.Location = new System.Drawing.Point(12, 44);
            this.CreditLabel.Name = "CreditLabel";
            this.CreditLabel.Size = new System.Drawing.Size(37, 13);
            this.CreditLabel.TabIndex = 1;
            this.CreditLabel.Text = "Credit:";
            // 
            // DescriptionLabel
            // 
            this.DescriptionLabel.AutoSize = true;
            this.DescriptionLabel.Location = new System.Drawing.Point(12, 92);
            this.DescriptionLabel.Name = "DescriptionLabel";
            this.DescriptionLabel.Size = new System.Drawing.Size(63, 13);
            this.DescriptionLabel.TabIndex = 3;
            this.DescriptionLabel.Text = "Description:";
            // 
            // ProjectIDLabel
            // 
            this.ProjectIDLabel.AutoSize = true;
            this.ProjectIDLabel.Location = new System.Drawing.Point(12, 20);
            this.ProjectIDLabel.Name = "ProjectIDLabel";
            this.ProjectIDLabel.Size = new System.Drawing.Size(57, 13);
            this.ProjectIDLabel.TabIndex = 0;
            this.ProjectIDLabel.Text = "Project ID:";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.BackColor = System.Drawing.SystemColors.Control;
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Controls.Add(this.grpProjectInfo, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.splitContainerBench, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.grpClients, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 60F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 140F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(556, 595);
            this.tableLayoutPanel1.TabIndex = 0;
            // 
            // splitContainerBench
            // 
            this.splitContainerBench.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainerBench.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.splitContainerBench.Location = new System.Drawing.Point(0, 60);
            this.splitContainerBench.Margin = new System.Windows.Forms.Padding(0);
            this.splitContainerBench.Name = "splitContainerBench";
            // 
            // splitContainerBench.Panel1
            // 
            this.splitContainerBench.Panel1.Controls.Add(this.listBox1);
            // 
            // splitContainerBench.Panel2
            // 
            this.splitContainerBench.Panel2.Controls.Add(this.tabControl1);
            this.splitContainerBench.Panel2.Controls.Add(this.btnExit);
            this.splitContainerBench.Size = new System.Drawing.Size(556, 395);
            this.splitContainerBench.SplitterDistance = 65;
            this.splitContainerBench.TabIndex = 1;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(65, 395);
            this.listBox1.TabIndex = 0;
            this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
            this.listBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseUp);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabTextBenchmark);
            this.tabControl1.Controls.Add(this.tabGraphConfig);
            this.tabControl1.Controls.Add(this.tabGraphFrameTime1);
            this.tabControl1.Controls.Add(this.tabGraphPPD1);
            this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl1.Location = new System.Drawing.Point(0, 0);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(487, 395);
            this.tabControl1.TabIndex = 0;
            // 
            // tabTextBenchmark
            // 
            this.tabTextBenchmark.Controls.Add(this.txtBenchmarks);
            this.tabTextBenchmark.Location = new System.Drawing.Point(4, 22);
            this.tabTextBenchmark.Name = "tabTextBenchmark";
            this.tabTextBenchmark.Padding = new System.Windows.Forms.Padding(3);
            this.tabTextBenchmark.Size = new System.Drawing.Size(479, 369);
            this.tabTextBenchmark.TabIndex = 0;
            this.tabTextBenchmark.Text = "Text";
            this.tabTextBenchmark.UseVisualStyleBackColor = true;
            // 
            // txtBenchmarks
            // 
            this.txtBenchmarks.BackColor = System.Drawing.Color.White;
            this.txtBenchmarks.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtBenchmarks.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtBenchmarks.Location = new System.Drawing.Point(3, 3);
            this.txtBenchmarks.Multiline = true;
            this.txtBenchmarks.Name = "txtBenchmarks";
            this.txtBenchmarks.ReadOnly = true;
            this.txtBenchmarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtBenchmarks.Size = new System.Drawing.Size(473, 363);
            this.txtBenchmarks.TabIndex = 0;
            // 
            // tabGraphConfig
            // 
            this.tabGraphConfig.Controls.Add(this.grpColors);
            this.tabGraphConfig.Location = new System.Drawing.Point(4, 22);
            this.tabGraphConfig.Name = "tabGraphConfig";
            this.tabGraphConfig.Size = new System.Drawing.Size(479, 369);
            this.tabGraphConfig.TabIndex = 3;
            this.tabGraphConfig.Text = "Graph Config";
            this.tabGraphConfig.UseVisualStyleBackColor = true;
            // 
            // grpColors
            // 
            this.grpColors.Controls.Add(this.lstColors);
            this.grpColors.Controls.Add(this.picColorPreview);
            this.grpColors.Controls.Add(this.btnMoveColorDown);
            this.grpColors.Controls.Add(this.btnAddColor);
            this.grpColors.Controls.Add(this.btnMoveColorUp);
            this.grpColors.Controls.Add(this.btnDeleteColor);
            this.grpColors.Location = new System.Drawing.Point(4, 6);
            this.grpColors.Name = "grpColors";
            this.grpColors.Size = new System.Drawing.Size(249, 355);
            this.grpColors.TabIndex = 6;
            this.grpColors.TabStop = false;
            this.grpColors.Text = "Color Configuration";
            // 
            // lstColors
            // 
            this.lstColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.lstColors.FormattingEnabled = true;
            this.lstColors.Location = new System.Drawing.Point(6, 19);
            this.lstColors.Name = "lstColors";
            this.lstColors.Size = new System.Drawing.Size(111, 329);
            this.lstColors.TabIndex = 0;
            this.lstColors.SelectedIndexChanged += new System.EventHandler(this.lstColors_SelectedIndexChanged);
            // 
            // picColorPreview
            // 
            this.picColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.picColorPreview.Location = new System.Drawing.Point(171, 19);
            this.picColorPreview.Name = "picColorPreview";
            this.picColorPreview.Size = new System.Drawing.Size(66, 66);
            this.picColorPreview.TabIndex = 5;
            this.picColorPreview.TabStop = false;
            // 
            // btnMoveColorDown
            // 
            this.btnMoveColorDown.Image = global::HFM.Forms.Properties.Resources.DownArrow;
            this.btnMoveColorDown.Location = new System.Drawing.Point(124, 55);
            this.btnMoveColorDown.Name = "btnMoveColorDown";
            this.btnMoveColorDown.Size = new System.Drawing.Size(30, 30);
            this.btnMoveColorDown.TabIndex = 2;
            this.toolTip1.SetToolTip(this.btnMoveColorDown, "Move Color Down");
            this.btnMoveColorDown.UseVisualStyleBackColor = true;
            this.btnMoveColorDown.Click += new System.EventHandler(this.btnMoveColorDown_Click);
            // 
            // btnAddColor
            // 
            this.btnAddColor.Image = global::HFM.Forms.Properties.Resources.Color;
            this.btnAddColor.Location = new System.Drawing.Point(124, 91);
            this.btnAddColor.Name = "btnAddColor";
            this.btnAddColor.Size = new System.Drawing.Size(30, 30);
            this.btnAddColor.TabIndex = 3;
            this.toolTip1.SetToolTip(this.btnAddColor, "Add Color");
            this.btnAddColor.UseVisualStyleBackColor = true;
            this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
            // 
            // btnMoveColorUp
            // 
            this.btnMoveColorUp.Image = global::HFM.Forms.Properties.Resources.UpArrow;
            this.btnMoveColorUp.Location = new System.Drawing.Point(124, 19);
            this.btnMoveColorUp.Name = "btnMoveColorUp";
            this.btnMoveColorUp.Size = new System.Drawing.Size(30, 30);
            this.btnMoveColorUp.TabIndex = 1;
            this.toolTip1.SetToolTip(this.btnMoveColorUp, "Move Color Up");
            this.btnMoveColorUp.UseVisualStyleBackColor = true;
            this.btnMoveColorUp.Click += new System.EventHandler(this.btnMoveColorUp_Click);
            // 
            // btnDeleteColor
            // 
            this.btnDeleteColor.Image = global::HFM.Forms.Properties.Resources.Delete;
            this.btnDeleteColor.Location = new System.Drawing.Point(124, 127);
            this.btnDeleteColor.Name = "btnDeleteColor";
            this.btnDeleteColor.Size = new System.Drawing.Size(30, 30);
            this.btnDeleteColor.TabIndex = 4;
            this.toolTip1.SetToolTip(this.btnDeleteColor, "Delete Color");
            this.btnDeleteColor.UseVisualStyleBackColor = true;
            this.btnDeleteColor.Click += new System.EventHandler(this.btnDeleteColor_Click);
            // 
            // tabGraphFrameTime1
            // 
            this.tabGraphFrameTime1.Location = new System.Drawing.Point(4, 22);
            this.tabGraphFrameTime1.Name = "tabGraphFrameTime1";
            this.tabGraphFrameTime1.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraphFrameTime1.Size = new System.Drawing.Size(479, 369);
            this.tabGraphFrameTime1.TabIndex = 2;
            this.tabGraphFrameTime1.Text = "Graph - Frame Time";
            this.tabGraphFrameTime1.UseVisualStyleBackColor = true;
            // 
            // tabGraphPPD1
            // 
            this.tabGraphPPD1.Location = new System.Drawing.Point(4, 22);
            this.tabGraphPPD1.Name = "tabGraphPPD1";
            this.tabGraphPPD1.Padding = new System.Windows.Forms.Padding(3);
            this.tabGraphPPD1.Size = new System.Drawing.Size(479, 369);
            this.tabGraphPPD1.TabIndex = 1;
            this.tabGraphPPD1.Text = "Graph - PPD";
            this.tabGraphPPD1.UseVisualStyleBackColor = true;
            // 
            // btnExit
            // 
            this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnExit.Location = new System.Drawing.Point(385, 182);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(56, 39);
            this.btnExit.TabIndex = 1;
            this.btnExit.TabStop = false;
            this.btnExit.Text = "Exit";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // grpClients
            // 
            this.grpClients.BackColor = System.Drawing.SystemColors.Control;
            this.grpClients.Controls.Add(this.picDeleteClient);
            this.grpClients.Controls.Add(this.cboClients);
            this.grpClients.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grpClients.Location = new System.Drawing.Point(3, 3);
            this.grpClients.Name = "grpClients";
            this.grpClients.Size = new System.Drawing.Size(550, 54);
            this.grpClients.TabIndex = 0;
            this.grpClients.TabStop = false;
            this.grpClients.Text = "Client Slots";
            // 
            // picDeleteClient
            // 
            this.picDeleteClient.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.picDeleteClient.Image = global::HFM.Forms.Properties.Resources.Quit;
            this.picDeleteClient.Location = new System.Drawing.Point(528, 21);
            this.picDeleteClient.Name = "picDeleteClient";
            this.picDeleteClient.Size = new System.Drawing.Size(16, 16);
            this.picDeleteClient.TabIndex = 2;
            this.picDeleteClient.TabStop = false;
            this.toolTip1.SetToolTip(this.picDeleteClient, "Delete Client Benchmarks");
            this.picDeleteClient.Click += new System.EventHandler(this.picDeleteClient_Click);
            // 
            // cboClients
            // 
            this.cboClients.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cboClients.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboClients.FormattingEnabled = true;
            this.cboClients.Location = new System.Drawing.Point(7, 19);
            this.cboClients.Name = "cboClients";
            this.cboClients.Size = new System.Drawing.Size(518, 21);
            this.cboClients.TabIndex = 0;
            // 
            // listBox1ContextMenuStrip
            // 
            this.listBox1ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContextRefreshMinimum,
            this.mnuContextDeleteProject});
            this.listBox1ContextMenuStrip.Name = "listBox1ContextMenuStrip";
            this.listBox1ContextMenuStrip.Size = new System.Drawing.Size(235, 48);
            // 
            // mnuContextRefreshMinimum
            // 
            this.mnuContextRefreshMinimum.Name = "mnuContextRefreshMinimum";
            this.mnuContextRefreshMinimum.Size = new System.Drawing.Size(234, 22);
            this.mnuContextRefreshMinimum.Text = "Refresh Minimum Frame Time";
            this.mnuContextRefreshMinimum.Click += new System.EventHandler(this.mnuContextRefreshMinimum_Click);
            // 
            // mnuContextDeleteProject
            // 
            this.mnuContextDeleteProject.Name = "mnuContextDeleteProject";
            this.mnuContextDeleteProject.Size = new System.Drawing.Size(234, 22);
            this.mnuContextDeleteProject.Text = "Delete This Project";
            this.mnuContextDeleteProject.Click += new System.EventHandler(this.mnuContextDeleteProject_Click);
            // 
            // BenchmarksForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnExit;
            this.ClientSize = new System.Drawing.Size(556, 595);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MinimumSize = new System.Drawing.Size(564, 300);
            this.Name = "BenchmarksForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Benchmarks Viewer";
            this.Load += new System.EventHandler(this.BenchmarksForm_Load);
            this.grpProjectInfo.ResumeLayout(false);
            this.grpProjectInfo.PerformLayout();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.splitContainerBench.Panel1.ResumeLayout(false);
            this.splitContainerBench.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainerBench)).EndInit();
            this.splitContainerBench.ResumeLayout(false);
            this.tabControl1.ResumeLayout(false);
            this.tabTextBenchmark.ResumeLayout(false);
            this.tabTextBenchmark.PerformLayout();
            this.tabGraphConfig.ResumeLayout(false);
            this.grpColors.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picColorPreview)).EndInit();
            this.grpClients.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.picDeleteClient)).EndInit();
            this.listBox1ContextMenuStrip.ResumeLayout(false);
            this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.GroupBox grpProjectInfo;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private System.Windows.Forms.TextBox WorkServerTextBox;
      private System.Windows.Forms.TextBox ContactTextBox;
      private System.Windows.Forms.TextBox ExpirationTextBox;
      private System.Windows.Forms.TextBox TimeoutTextBox;
      private System.Windows.Forms.TextBox CoreTextBox;
      private System.Windows.Forms.TextBox AtomsTextBox;
      private System.Windows.Forms.TextBox FramesTextBox;
      private System.Windows.Forms.TextBox CreditTextBox;
      private System.Windows.Forms.TextBox ProjectIDTextBox;
      private System.Windows.Forms.LinkLabel DescriptionLinkLabel;
      private System.Windows.Forms.Label WorkServerLabel;
      private System.Windows.Forms.Label ContactLabel;
      private System.Windows.Forms.Label CoreLabel;
      private System.Windows.Forms.Label ExpirationLabel;
      private System.Windows.Forms.Label TimeoutLabel;
      private System.Windows.Forms.Label AtomsLabel;
      private System.Windows.Forms.Label FramesLabel;
      private System.Windows.Forms.Label CreditLabel;
      private System.Windows.Forms.Label DescriptionLabel;
      private System.Windows.Forms.Label ProjectIDLabel;
      private System.Windows.Forms.SplitContainer splitContainerBench;
      private System.Windows.Forms.ListBox listBox1;
      private System.Windows.Forms.TextBox txtBenchmarks;
      private System.Windows.Forms.Button btnExit;
      private System.Windows.Forms.GroupBox grpClients;
      private System.Windows.Forms.ComboBox cboClients;
      private System.Windows.Forms.PictureBox picDeleteClient;
      private System.Windows.Forms.ContextMenuStrip listBox1ContextMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem mnuContextRefreshMinimum;
      private System.Windows.Forms.ToolStripMenuItem mnuContextDeleteProject;
      private System.Windows.Forms.ToolTip toolTip1;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabTextBenchmark;
      private System.Windows.Forms.TabPage tabGraphPPD1;
      private System.Windows.Forms.TabPage tabGraphFrameTime1;
      private System.Windows.Forms.TabPage tabGraphConfig;
      private System.Windows.Forms.Button btnMoveColorDown;
      private System.Windows.Forms.Button btnMoveColorUp;
      private System.Windows.Forms.Button btnDeleteColor;
      private System.Windows.Forms.Button btnAddColor;
      private System.Windows.Forms.ListBox lstColors;
      private System.Windows.Forms.PictureBox picColorPreview;
      private System.Windows.Forms.TextBox KFactorTextBox;
      private System.Windows.Forms.Label KFactorLabel;
      private System.Windows.Forms.GroupBox grpColors;


   }
}
