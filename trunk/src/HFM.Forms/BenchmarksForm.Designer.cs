using HFM.Forms.Controls;

namespace HFM.Forms
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
         this.grpProjectInfo = new HFM.Forms.Controls.GroupBoxWrapper();
         this.txtKFactor = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblKFactor = new HFM.Forms.Controls.LabelWrapper();
         this.txtServerIP = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtContact = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtMaximumDays = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtPreferredDays = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtCore = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtAtoms = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtFrames = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtCredit = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtProjectID = new HFM.Forms.Controls.TextBoxWrapper();
         this.linkDescription = new System.Windows.Forms.LinkLabel();
         this.lblServerIP = new HFM.Forms.Controls.LabelWrapper();
         this.lblContact = new HFM.Forms.Controls.LabelWrapper();
         this.lblCore = new HFM.Forms.Controls.LabelWrapper();
         this.lblMaxDays = new HFM.Forms.Controls.LabelWrapper();
         this.lblPreferred = new HFM.Forms.Controls.LabelWrapper();
         this.lblAtoms = new HFM.Forms.Controls.LabelWrapper();
         this.lblFrames = new HFM.Forms.Controls.LabelWrapper();
         this.lblCredit = new HFM.Forms.Controls.LabelWrapper();
         this.lblDescription = new HFM.Forms.Controls.LabelWrapper();
         this.lblProjectID = new HFM.Forms.Controls.LabelWrapper();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.splitContainerBench = new HFM.Forms.Controls.SplitContainerWrapper();
         this.listBox1 = new System.Windows.Forms.ListBox();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabTextBenchmark = new System.Windows.Forms.TabPage();
         this.txtBenchmarks = new HFM.Forms.Controls.TextBoxWrapper();
         this.tabGraphConfig = new System.Windows.Forms.TabPage();
         this.grpColors = new HFM.Forms.Controls.GroupBoxWrapper();
         this.lstColors = new System.Windows.Forms.ListBox();
         this.picColorPreview = new System.Windows.Forms.PictureBox();
         this.btnMoveColorDown = new HFM.Forms.Controls.ButtonWrapper();
         this.btnAddColor = new HFM.Forms.Controls.ButtonWrapper();
         this.btnMoveColorUp = new HFM.Forms.Controls.ButtonWrapper();
         this.btnDeleteColor = new HFM.Forms.Controls.ButtonWrapper();
         this.tabGraphFrameTime1 = new System.Windows.Forms.TabPage();
         this.zgFrameTime1 = new ZedGraph.ZedGraphControl();
         this.tabGraphPPD1 = new System.Windows.Forms.TabPage();
         this.zgPpd1 = new ZedGraph.ZedGraphControl();
         this.btnExit = new HFM.Forms.Controls.ButtonWrapper();
         this.grpClients = new HFM.Forms.Controls.GroupBoxWrapper();
         this.picDeleteClient = new System.Windows.Forms.PictureBox();
         this.cboClients = new HFM.Forms.Controls.ComboBoxWrapper();
         this.listBox1ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.mnuContextRefreshMinimum = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextDeleteProject = new System.Windows.Forms.ToolStripMenuItem();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.grpClientLayout = new HFM.Forms.Controls.GroupBoxWrapper();
         this.pnlClientLayout = new harlam357.Windows.Forms.RadioPanel();
         this.rdoSingleGraph = new HFM.Forms.Controls.RadioButtonWrapper();
         this.rdoClientsPerGraph = new HFM.Forms.Controls.RadioButtonWrapper();
         this.udClientsPerGraph = new System.Windows.Forms.NumericUpDown();
         this.grpProjectInfo.SuspendLayout();
         this.tableLayoutPanel1.SuspendLayout();
         this.splitContainerBench.Panel1.SuspendLayout();
         this.splitContainerBench.Panel2.SuspendLayout();
         this.splitContainerBench.SuspendLayout();
         this.tabControl1.SuspendLayout();
         this.tabTextBenchmark.SuspendLayout();
         this.tabGraphConfig.SuspendLayout();
         this.grpColors.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picColorPreview)).BeginInit();
         this.tabGraphFrameTime1.SuspendLayout();
         this.tabGraphPPD1.SuspendLayout();
         this.grpClients.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.picDeleteClient)).BeginInit();
         this.listBox1ContextMenuStrip.SuspendLayout();
         this.grpClientLayout.SuspendLayout();
         this.pnlClientLayout.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udClientsPerGraph)).BeginInit();
         this.SuspendLayout();
         // 
         // grpProjectInfo
         // 
         this.grpProjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.grpProjectInfo.BackColor = System.Drawing.SystemColors.Control;
         this.grpProjectInfo.Controls.Add(this.txtKFactor);
         this.grpProjectInfo.Controls.Add(this.lblKFactor);
         this.grpProjectInfo.Controls.Add(this.txtServerIP);
         this.grpProjectInfo.Controls.Add(this.txtContact);
         this.grpProjectInfo.Controls.Add(this.txtMaximumDays);
         this.grpProjectInfo.Controls.Add(this.txtPreferredDays);
         this.grpProjectInfo.Controls.Add(this.txtCore);
         this.grpProjectInfo.Controls.Add(this.txtAtoms);
         this.grpProjectInfo.Controls.Add(this.txtFrames);
         this.grpProjectInfo.Controls.Add(this.txtCredit);
         this.grpProjectInfo.Controls.Add(this.txtProjectID);
         this.grpProjectInfo.Controls.Add(this.linkDescription);
         this.grpProjectInfo.Controls.Add(this.lblServerIP);
         this.grpProjectInfo.Controls.Add(this.lblContact);
         this.grpProjectInfo.Controls.Add(this.lblCore);
         this.grpProjectInfo.Controls.Add(this.lblMaxDays);
         this.grpProjectInfo.Controls.Add(this.lblPreferred);
         this.grpProjectInfo.Controls.Add(this.lblAtoms);
         this.grpProjectInfo.Controls.Add(this.lblFrames);
         this.grpProjectInfo.Controls.Add(this.lblCredit);
         this.grpProjectInfo.Controls.Add(this.lblDescription);
         this.grpProjectInfo.Controls.Add(this.lblProjectID);
         this.grpProjectInfo.Location = new System.Drawing.Point(10, 465);
         this.grpProjectInfo.Margin = new System.Windows.Forms.Padding(10);
         this.grpProjectInfo.Name = "grpProjectInfo";
         this.grpProjectInfo.Size = new System.Drawing.Size(536, 120);
         this.grpProjectInfo.TabIndex = 2;
         this.grpProjectInfo.TabStop = false;
         this.grpProjectInfo.Text = "Project Information";
         // 
         // txtKFactor
         // 
         this.txtKFactor.Location = new System.Drawing.Point(179, 41);
         this.txtKFactor.Name = "txtKFactor";
         this.txtKFactor.ReadOnly = true;
         this.txtKFactor.Size = new System.Drawing.Size(48, 20);
         this.txtKFactor.TabIndex = 7;
         this.txtKFactor.TabStop = false;
         this.txtKFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblKFactor
         // 
         this.lblKFactor.AutoSize = true;
         this.lblKFactor.Location = new System.Drawing.Point(130, 44);
         this.lblKFactor.Name = "lblKFactor";
         this.lblKFactor.Size = new System.Drawing.Size(47, 13);
         this.lblKFactor.TabIndex = 6;
         this.lblKFactor.Text = "KFactor:";
         // 
         // txtServerIP
         // 
         this.txtServerIP.Location = new System.Drawing.Point(429, 89);
         this.txtServerIP.Name = "txtServerIP";
         this.txtServerIP.ReadOnly = true;
         this.txtServerIP.Size = new System.Drawing.Size(95, 20);
         this.txtServerIP.TabIndex = 20;
         this.txtServerIP.TabStop = false;
         this.txtServerIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtContact
         // 
         this.txtContact.Location = new System.Drawing.Point(429, 66);
         this.txtContact.Name = "txtContact";
         this.txtContact.ReadOnly = true;
         this.txtContact.Size = new System.Drawing.Size(95, 20);
         this.txtContact.TabIndex = 19;
         this.txtContact.TabStop = false;
         this.txtContact.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtMaximumDays
         // 
         this.txtMaximumDays.Location = new System.Drawing.Point(429, 41);
         this.txtMaximumDays.Name = "txtMaximumDays";
         this.txtMaximumDays.ReadOnly = true;
         this.txtMaximumDays.Size = new System.Drawing.Size(95, 20);
         this.txtMaximumDays.TabIndex = 18;
         this.txtMaximumDays.TabStop = false;
         this.txtMaximumDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtPreferredDays
         // 
         this.txtPreferredDays.Location = new System.Drawing.Point(429, 17);
         this.txtPreferredDays.Name = "txtPreferredDays";
         this.txtPreferredDays.ReadOnly = true;
         this.txtPreferredDays.Size = new System.Drawing.Size(95, 20);
         this.txtPreferredDays.TabIndex = 17;
         this.txtPreferredDays.TabStop = false;
         this.txtPreferredDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCore
         // 
         this.txtCore.Location = new System.Drawing.Point(51, 66);
         this.txtCore.Name = "txtCore";
         this.txtCore.ReadOnly = true;
         this.txtCore.Size = new System.Drawing.Size(122, 20);
         this.txtCore.TabIndex = 10;
         this.txtCore.TabStop = false;
         this.txtCore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtAtoms
         // 
         this.txtAtoms.Location = new System.Drawing.Point(233, 66);
         this.txtAtoms.Name = "txtAtoms";
         this.txtAtoms.ReadOnly = true;
         this.txtAtoms.Size = new System.Drawing.Size(92, 20);
         this.txtAtoms.TabIndex = 12;
         this.txtAtoms.TabStop = false;
         this.txtAtoms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtFrames
         // 
         this.txtFrames.Location = new System.Drawing.Point(277, 41);
         this.txtFrames.Name = "txtFrames";
         this.txtFrames.ReadOnly = true;
         this.txtFrames.Size = new System.Drawing.Size(48, 20);
         this.txtFrames.TabIndex = 9;
         this.txtFrames.TabStop = false;
         this.txtFrames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCredit
         // 
         this.txtCredit.Location = new System.Drawing.Point(51, 41);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(76, 20);
         this.txtCredit.TabIndex = 5;
         this.txtCredit.TabStop = false;
         this.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtProjectID
         // 
         this.txtProjectID.Location = new System.Drawing.Point(75, 17);
         this.txtProjectID.Name = "txtProjectID";
         this.txtProjectID.ReadOnly = true;
         this.txtProjectID.Size = new System.Drawing.Size(250, 20);
         this.txtProjectID.TabIndex = 4;
         this.txtProjectID.TabStop = false;
         this.txtProjectID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // linkDescription
         // 
         this.linkDescription.AutoSize = true;
         this.linkDescription.Location = new System.Drawing.Point(76, 92);
         this.linkDescription.Name = "linkDescription";
         this.linkDescription.Size = new System.Drawing.Size(0, 13);
         this.linkDescription.TabIndex = 21;
         this.linkDescription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDescription_LinkClicked);
         // 
         // lblServerIP
         // 
         this.lblServerIP.AutoSize = true;
         this.lblServerIP.Location = new System.Drawing.Point(369, 92);
         this.lblServerIP.Name = "lblServerIP";
         this.lblServerIP.Size = new System.Drawing.Size(54, 13);
         this.lblServerIP.TabIndex = 16;
         this.lblServerIP.Text = "Server IP:";
         // 
         // lblContact
         // 
         this.lblContact.AutoSize = true;
         this.lblContact.Location = new System.Drawing.Point(376, 69);
         this.lblContact.Name = "lblContact";
         this.lblContact.Size = new System.Drawing.Size(47, 13);
         this.lblContact.TabIndex = 15;
         this.lblContact.Text = "Contact:";
         // 
         // lblCore
         // 
         this.lblCore.AutoSize = true;
         this.lblCore.Location = new System.Drawing.Point(12, 69);
         this.lblCore.Name = "lblCore";
         this.lblCore.Size = new System.Drawing.Size(32, 13);
         this.lblCore.TabIndex = 2;
         this.lblCore.Text = "Core:";
         // 
         // lblMaxDays
         // 
         this.lblMaxDays.AutoSize = true;
         this.lblMaxDays.Location = new System.Drawing.Point(342, 44);
         this.lblMaxDays.Name = "lblMaxDays";
         this.lblMaxDays.Size = new System.Drawing.Size(81, 13);
         this.lblMaxDays.TabIndex = 14;
         this.lblMaxDays.Text = "Maximum Days:";
         // 
         // lblPreferred
         // 
         this.lblPreferred.AutoSize = true;
         this.lblPreferred.Location = new System.Drawing.Point(343, 20);
         this.lblPreferred.Name = "lblPreferred";
         this.lblPreferred.Size = new System.Drawing.Size(80, 13);
         this.lblPreferred.TabIndex = 13;
         this.lblPreferred.Text = "Preferred Days:";
         // 
         // lblAtoms
         // 
         this.lblAtoms.AutoSize = true;
         this.lblAtoms.Location = new System.Drawing.Point(189, 69);
         this.lblAtoms.Name = "lblAtoms";
         this.lblAtoms.Size = new System.Drawing.Size(39, 13);
         this.lblAtoms.TabIndex = 11;
         this.lblAtoms.Text = "Atoms:";
         // 
         // lblFrames
         // 
         this.lblFrames.AutoSize = true;
         this.lblFrames.Location = new System.Drawing.Point(232, 44);
         this.lblFrames.Name = "lblFrames";
         this.lblFrames.Size = new System.Drawing.Size(44, 13);
         this.lblFrames.TabIndex = 8;
         this.lblFrames.Text = "Frames:";
         // 
         // lblCredit
         // 
         this.lblCredit.AutoSize = true;
         this.lblCredit.Location = new System.Drawing.Point(12, 44);
         this.lblCredit.Name = "lblCredit";
         this.lblCredit.Size = new System.Drawing.Size(37, 13);
         this.lblCredit.TabIndex = 1;
         this.lblCredit.Text = "Credit:";
         // 
         // lblDescription
         // 
         this.lblDescription.AutoSize = true;
         this.lblDescription.Location = new System.Drawing.Point(12, 92);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.TabIndex = 3;
         this.lblDescription.Text = "Description:";
         // 
         // lblProjectID
         // 
         this.lblProjectID.AutoSize = true;
         this.lblProjectID.Location = new System.Drawing.Point(12, 20);
         this.lblProjectID.Name = "lblProjectID";
         this.lblProjectID.Size = new System.Drawing.Size(57, 13);
         this.lblProjectID.TabIndex = 0;
         this.lblProjectID.Text = "Project ID:";
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
         this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
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
         this.tabGraphConfig.Controls.Add(this.grpClientLayout);
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
         this.tabGraphFrameTime1.Controls.Add(this.zgFrameTime1);
         this.tabGraphFrameTime1.Location = new System.Drawing.Point(4, 22);
         this.tabGraphFrameTime1.Name = "tabGraphFrameTime1";
         this.tabGraphFrameTime1.Padding = new System.Windows.Forms.Padding(3);
         this.tabGraphFrameTime1.Size = new System.Drawing.Size(479, 369);
         this.tabGraphFrameTime1.TabIndex = 2;
         this.tabGraphFrameTime1.Text = "Graph - Frame Time (1)";
         this.tabGraphFrameTime1.UseVisualStyleBackColor = true;
         // 
         // zgFrameTime1
         // 
         this.zgFrameTime1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.zgFrameTime1.Location = new System.Drawing.Point(3, 3);
         this.zgFrameTime1.Name = "zgFrameTime1";
         this.zgFrameTime1.ScrollGrace = 0D;
         this.zgFrameTime1.ScrollMaxX = 0D;
         this.zgFrameTime1.ScrollMaxY = 0D;
         this.zgFrameTime1.ScrollMaxY2 = 0D;
         this.zgFrameTime1.ScrollMinX = 0D;
         this.zgFrameTime1.ScrollMinY = 0D;
         this.zgFrameTime1.ScrollMinY2 = 0D;
         this.zgFrameTime1.Size = new System.Drawing.Size(473, 363);
         this.zgFrameTime1.TabIndex = 0;
         // 
         // tabGraphPPD1
         // 
         this.tabGraphPPD1.Controls.Add(this.zgPpd1);
         this.tabGraphPPD1.Location = new System.Drawing.Point(4, 22);
         this.tabGraphPPD1.Name = "tabGraphPPD1";
         this.tabGraphPPD1.Padding = new System.Windows.Forms.Padding(3);
         this.tabGraphPPD1.Size = new System.Drawing.Size(479, 369);
         this.tabGraphPPD1.TabIndex = 1;
         this.tabGraphPPD1.Text = "Graph - PPD (1)";
         this.tabGraphPPD1.UseVisualStyleBackColor = true;
         // 
         // zgPpd1
         // 
         this.zgPpd1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.zgPpd1.Location = new System.Drawing.Point(3, 3);
         this.zgPpd1.Name = "zgPpd1";
         this.zgPpd1.ScrollGrace = 0D;
         this.zgPpd1.ScrollMaxX = 0D;
         this.zgPpd1.ScrollMaxY = 0D;
         this.zgPpd1.ScrollMaxY2 = 0D;
         this.zgPpd1.ScrollMinX = 0D;
         this.zgPpd1.ScrollMinY = 0D;
         this.zgPpd1.ScrollMinY2 = 0D;
         this.zgPpd1.Size = new System.Drawing.Size(473, 363);
         this.zgPpd1.TabIndex = 0;
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
         this.grpClients.Text = "Benchmark Clients";
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
         this.cboClients.SelectedIndexChanged += new System.EventHandler(this.cboClients_SelectedIndexChanged);
         // 
         // listBox1ContextMenuStrip
         // 
         this.listBox1ContextMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuContextRefreshMinimum,
            this.mnuContextDeleteProject});
         this.listBox1ContextMenuStrip.Name = "listBox1ContextMenuStrip";
         this.listBox1ContextMenuStrip.Size = new System.Drawing.Size(225, 48);
         // 
         // mnuContextRefreshMinimum
         // 
         this.mnuContextRefreshMinimum.Name = "mnuContextRefreshMinimum";
         this.mnuContextRefreshMinimum.Size = new System.Drawing.Size(224, 22);
         this.mnuContextRefreshMinimum.Text = "Refresh Minimum Frame Time";
         this.mnuContextRefreshMinimum.Click += new System.EventHandler(this.mnuContextRefreshMinimum_Click);
         // 
         // mnuContextDeleteProject
         // 
         this.mnuContextDeleteProject.Name = "mnuContextDeleteProject";
         this.mnuContextDeleteProject.Size = new System.Drawing.Size(224, 22);
         this.mnuContextDeleteProject.Text = "Delete This Project";
         this.mnuContextDeleteProject.Click += new System.EventHandler(this.mnuContextDeleteProject_Click);
         // 
         // grpClientLayout
         // 
         this.grpClientLayout.Controls.Add(this.pnlClientLayout);
         this.grpClientLayout.Location = new System.Drawing.Point(259, 6);
         this.grpClientLayout.Name = "grpClientLayout";
         this.grpClientLayout.Size = new System.Drawing.Size(215, 355);
         this.grpClientLayout.TabIndex = 7;
         this.grpClientLayout.TabStop = false;
         this.grpClientLayout.Text = "Client Layout";
         // 
         // pnlClientLayout
         // 
         this.pnlClientLayout.Controls.Add(this.udClientsPerGraph);
         this.pnlClientLayout.Controls.Add(this.rdoClientsPerGraph);
         this.pnlClientLayout.Controls.Add(this.rdoSingleGraph);
         this.pnlClientLayout.Location = new System.Drawing.Point(6, 19);
         this.pnlClientLayout.Name = "pnlClientLayout";
         this.pnlClientLayout.Size = new System.Drawing.Size(203, 91);
         this.pnlClientLayout.TabIndex = 0;
         this.pnlClientLayout.ValueMember = null;
         // 
         // rdoSingleGraph
         // 
         this.rdoSingleGraph.AutoSize = true;
         this.rdoSingleGraph.Location = new System.Drawing.Point(11, 8);
         this.rdoSingleGraph.Name = "rdoSingleGraph";
         this.rdoSingleGraph.Size = new System.Drawing.Size(86, 17);
         this.rdoSingleGraph.TabIndex = 0;
         this.rdoSingleGraph.TabStop = true;
         this.rdoSingleGraph.Tag = "0";
         this.rdoSingleGraph.Text = "Single Graph";
         this.rdoSingleGraph.UseVisualStyleBackColor = true;
         this.rdoSingleGraph.CheckedChanged += new System.EventHandler(this.rdoSingleGraph_CheckedChanged);
         // 
         // rdoClientsPerGraph
         // 
         this.rdoClientsPerGraph.AutoSize = true;
         this.rdoClientsPerGraph.Location = new System.Drawing.Point(11, 36);
         this.rdoClientsPerGraph.Name = "rdoClientsPerGraph";
         this.rdoClientsPerGraph.Size = new System.Drawing.Size(158, 17);
         this.rdoClientsPerGraph.TabIndex = 1;
         this.rdoClientsPerGraph.TabStop = true;
         this.rdoClientsPerGraph.Tag = "1";
         this.rdoClientsPerGraph.Text = "Number of Clients per Graph";
         this.rdoClientsPerGraph.UseVisualStyleBackColor = true;
         this.rdoClientsPerGraph.CheckedChanged += new System.EventHandler(this.rdoClientsPerGraph_CheckedChanged);
         // 
         // udClientsPerGraph
         // 
         this.udClientsPerGraph.Location = new System.Drawing.Point(11, 62);
         this.udClientsPerGraph.Maximum = new decimal(new int[] {
            50,
            0,
            0,
            0});
         this.udClientsPerGraph.Minimum = new decimal(new int[] {
            5,
            0,
            0,
            0});
         this.udClientsPerGraph.Name = "udClientsPerGraph";
         this.udClientsPerGraph.Size = new System.Drawing.Size(50, 20);
         this.udClientsPerGraph.TabIndex = 2;
         this.udClientsPerGraph.Value = new decimal(new int[] {
            5,
            0,
            0,
            0});
         this.udClientsPerGraph.ValueChanged += new System.EventHandler(this.udClientsPerGraph_ValueChanged);
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
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBenchmarks_FormClosing);
         this.Shown += new System.EventHandler(this.frmBenchmarks_Shown);
         this.grpProjectInfo.ResumeLayout(false);
         this.grpProjectInfo.PerformLayout();
         this.tableLayoutPanel1.ResumeLayout(false);
         this.splitContainerBench.Panel1.ResumeLayout(false);
         this.splitContainerBench.Panel2.ResumeLayout(false);
         this.splitContainerBench.ResumeLayout(false);
         this.tabControl1.ResumeLayout(false);
         this.tabTextBenchmark.ResumeLayout(false);
         this.tabTextBenchmark.PerformLayout();
         this.tabGraphConfig.ResumeLayout(false);
         this.grpColors.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.picColorPreview)).EndInit();
         this.tabGraphFrameTime1.ResumeLayout(false);
         this.tabGraphPPD1.ResumeLayout(false);
         this.grpClients.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.picDeleteClient)).EndInit();
         this.listBox1ContextMenuStrip.ResumeLayout(false);
         this.grpClientLayout.ResumeLayout(false);
         this.pnlClientLayout.ResumeLayout(false);
         this.pnlClientLayout.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udClientsPerGraph)).EndInit();
         this.ResumeLayout(false);

      }

      #endregion

      private GroupBoxWrapper grpProjectInfo;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private TextBoxWrapper txtServerIP;
      private TextBoxWrapper txtContact;
      private TextBoxWrapper txtMaximumDays;
      private TextBoxWrapper txtPreferredDays;
      private TextBoxWrapper txtCore;
      private TextBoxWrapper txtAtoms;
      private TextBoxWrapper txtFrames;
      private TextBoxWrapper txtCredit;
      private TextBoxWrapper txtProjectID;
      private System.Windows.Forms.LinkLabel linkDescription;
      private LabelWrapper lblServerIP;
      private LabelWrapper lblContact;
      private LabelWrapper lblCore;
      private LabelWrapper lblMaxDays;
      private LabelWrapper lblPreferred;
      private LabelWrapper lblAtoms;
      private LabelWrapper lblFrames;
      private LabelWrapper lblCredit;
      private LabelWrapper lblDescription;
      private LabelWrapper lblProjectID;
      private SplitContainerWrapper splitContainerBench;
      private System.Windows.Forms.ListBox listBox1;
      private TextBoxWrapper txtBenchmarks;
      private ButtonWrapper btnExit;
      private GroupBoxWrapper grpClients;
      private ComboBoxWrapper cboClients;
      private System.Windows.Forms.PictureBox picDeleteClient;
      private System.Windows.Forms.ContextMenuStrip listBox1ContextMenuStrip;
      private System.Windows.Forms.ToolStripMenuItem mnuContextRefreshMinimum;
      private System.Windows.Forms.ToolStripMenuItem mnuContextDeleteProject;
      private System.Windows.Forms.ToolTip toolTip1;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabTextBenchmark;
      private System.Windows.Forms.TabPage tabGraphPPD1;
      private ZedGraph.ZedGraphControl zgPpd1;
      private System.Windows.Forms.TabPage tabGraphFrameTime1;
      private ZedGraph.ZedGraphControl zgFrameTime1;
      private System.Windows.Forms.TabPage tabGraphConfig;
      private ButtonWrapper btnMoveColorDown;
      private ButtonWrapper btnMoveColorUp;
      private ButtonWrapper btnDeleteColor;
      private ButtonWrapper btnAddColor;
      private System.Windows.Forms.ListBox lstColors;
      private System.Windows.Forms.PictureBox picColorPreview;
      private TextBoxWrapper txtKFactor;
      private LabelWrapper lblKFactor;
      private GroupBoxWrapper grpColors;
      private GroupBoxWrapper grpClientLayout;
      private harlam357.Windows.Forms.RadioPanel pnlClientLayout;
      private RadioButtonWrapper rdoClientsPerGraph;
      private RadioButtonWrapper rdoSingleGraph;
      private System.Windows.Forms.NumericUpDown udClientsPerGraph;


   }
}
