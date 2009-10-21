using HFM.Classes;

namespace HFM.Forms
{
   partial class frmBenchmarks
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBenchmarks));
         this.grpProjectInfo = new HFM.Classes.GroupBoxWrapper();
         this.txtServerIP = new HFM.Classes.TextBoxWrapper();
         this.txtContact = new HFM.Classes.TextBoxWrapper();
         this.txtMaximumDays = new HFM.Classes.TextBoxWrapper();
         this.txtPreferredDays = new HFM.Classes.TextBoxWrapper();
         this.txtCore = new HFM.Classes.TextBoxWrapper();
         this.txtAtoms = new HFM.Classes.TextBoxWrapper();
         this.txtFrames = new HFM.Classes.TextBoxWrapper();
         this.txtCredit = new HFM.Classes.TextBoxWrapper();
         this.txtProjectID = new HFM.Classes.TextBoxWrapper();
         this.linkDescription = new System.Windows.Forms.LinkLabel();
         this.lblServerIP = new HFM.Classes.LabelWrapper();
         this.lblContact = new HFM.Classes.LabelWrapper();
         this.lblCore = new HFM.Classes.LabelWrapper();
         this.lblMaxDays = new HFM.Classes.LabelWrapper();
         this.lblPreferred = new HFM.Classes.LabelWrapper();
         this.lblAtoms = new HFM.Classes.LabelWrapper();
         this.lblFrames = new HFM.Classes.LabelWrapper();
         this.lblCredit = new HFM.Classes.LabelWrapper();
         this.lblDescription = new HFM.Classes.LabelWrapper();
         this.lblProjectID = new HFM.Classes.LabelWrapper();
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.splitContainerBench = new HFM.Classes.SplitContainerWrapper();
         this.listBox1 = new System.Windows.Forms.ListBox();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabTextBenchmark = new System.Windows.Forms.TabPage();
         this.txtBenchmarks = new HFM.Classes.TextBoxWrapper();
         this.tabGraphFrameTime = new System.Windows.Forms.TabPage();
         this.zgFrameTime = new ZedGraph.ZedGraphControl();
         this.tabGraphPPD = new System.Windows.Forms.TabPage();
         this.zgPpd = new ZedGraph.ZedGraphControl();
         this.tabGraphColors = new System.Windows.Forms.TabPage();
         this.picColorPreview = new System.Windows.Forms.PictureBox();
         this.btnMoveColorDown = new HFM.Classes.ButtonWrapper();
         this.btnMoveColorUp = new HFM.Classes.ButtonWrapper();
         this.btnDeleteColor = new HFM.Classes.ButtonWrapper();
         this.btnAddColor = new HFM.Classes.ButtonWrapper();
         this.lstColors = new System.Windows.Forms.ListBox();
         this.btnExit = new HFM.Classes.ButtonWrapper();
         this.grpClients = new HFM.Classes.GroupBoxWrapper();
         this.picDeleteClient = new System.Windows.Forms.PictureBox();
         this.cboClients = new HFM.Classes.ComboBoxWrapper();
         this.listBox1ContextMenuStrip = new System.Windows.Forms.ContextMenuStrip(this.components);
         this.mnuContextRefreshMinimum = new System.Windows.Forms.ToolStripMenuItem();
         this.mnuContextDeleteProject = new System.Windows.Forms.ToolStripMenuItem();
         this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
         this.grpProjectInfo.SuspendLayout();
         this.tableLayoutPanel1.SuspendLayout();
         this.splitContainerBench.Panel1.SuspendLayout();
         this.splitContainerBench.Panel2.SuspendLayout();
         this.splitContainerBench.SuspendLayout();
         this.tabControl1.SuspendLayout();
         this.tabTextBenchmark.SuspendLayout();
         this.tabGraphFrameTime.SuspendLayout();
         this.tabGraphPPD.SuspendLayout();
         this.tabGraphColors.SuspendLayout();
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
         // txtServerIP
         // 
         this.txtServerIP.Location = new System.Drawing.Point(429, 89);
         this.txtServerIP.Name = "txtServerIP";
         this.txtServerIP.ReadOnly = true;
         this.txtServerIP.Size = new System.Drawing.Size(95, 20);
         this.txtServerIP.TabIndex = 18;
         this.txtServerIP.TabStop = false;
         this.txtServerIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtContact
         // 
         this.txtContact.Location = new System.Drawing.Point(429, 66);
         this.txtContact.Name = "txtContact";
         this.txtContact.ReadOnly = true;
         this.txtContact.Size = new System.Drawing.Size(95, 20);
         this.txtContact.TabIndex = 17;
         this.txtContact.TabStop = false;
         this.txtContact.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtMaximumDays
         // 
         this.txtMaximumDays.Location = new System.Drawing.Point(429, 41);
         this.txtMaximumDays.Name = "txtMaximumDays";
         this.txtMaximumDays.ReadOnly = true;
         this.txtMaximumDays.Size = new System.Drawing.Size(95, 20);
         this.txtMaximumDays.TabIndex = 16;
         this.txtMaximumDays.TabStop = false;
         this.txtMaximumDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtPreferredDays
         // 
         this.txtPreferredDays.Location = new System.Drawing.Point(429, 17);
         this.txtPreferredDays.Name = "txtPreferredDays";
         this.txtPreferredDays.ReadOnly = true;
         this.txtPreferredDays.Size = new System.Drawing.Size(95, 20);
         this.txtPreferredDays.TabIndex = 15;
         this.txtPreferredDays.TabStop = false;
         this.txtPreferredDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCore
         // 
         this.txtCore.Location = new System.Drawing.Point(55, 66);
         this.txtCore.Name = "txtCore";
         this.txtCore.ReadOnly = true;
         this.txtCore.Size = new System.Drawing.Size(122, 20);
         this.txtCore.TabIndex = 8;
         this.txtCore.TabStop = false;
         this.txtCore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtAtoms
         // 
         this.txtAtoms.Location = new System.Drawing.Point(233, 66);
         this.txtAtoms.Name = "txtAtoms";
         this.txtAtoms.ReadOnly = true;
         this.txtAtoms.Size = new System.Drawing.Size(92, 20);
         this.txtAtoms.TabIndex = 10;
         this.txtAtoms.TabStop = false;
         this.txtAtoms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtFrames
         // 
         this.txtFrames.Location = new System.Drawing.Point(233, 41);
         this.txtFrames.Name = "txtFrames";
         this.txtFrames.ReadOnly = true;
         this.txtFrames.Size = new System.Drawing.Size(92, 20);
         this.txtFrames.TabIndex = 7;
         this.txtFrames.TabStop = false;
         this.txtFrames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCredit
         // 
         this.txtCredit.Location = new System.Drawing.Point(55, 41);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(122, 20);
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
         this.linkDescription.TabIndex = 19;
         this.linkDescription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDescription_LinkClicked);
         // 
         // lblServerIP
         // 
         this.lblServerIP.AutoSize = true;
         this.lblServerIP.Location = new System.Drawing.Point(369, 92);
         this.lblServerIP.Name = "lblServerIP";
         this.lblServerIP.Size = new System.Drawing.Size(54, 13);
         this.lblServerIP.TabIndex = 14;
         this.lblServerIP.Text = "Server IP:";
         // 
         // lblContact
         // 
         this.lblContact.AutoSize = true;
         this.lblContact.Location = new System.Drawing.Point(376, 69);
         this.lblContact.Name = "lblContact";
         this.lblContact.Size = new System.Drawing.Size(47, 13);
         this.lblContact.TabIndex = 13;
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
         this.lblMaxDays.TabIndex = 12;
         this.lblMaxDays.Text = "Maximum Days:";
         // 
         // lblPreferred
         // 
         this.lblPreferred.AutoSize = true;
         this.lblPreferred.Location = new System.Drawing.Point(343, 20);
         this.lblPreferred.Name = "lblPreferred";
         this.lblPreferred.Size = new System.Drawing.Size(80, 13);
         this.lblPreferred.TabIndex = 11;
         this.lblPreferred.Text = "Preferred Days:";
         // 
         // lblAtoms
         // 
         this.lblAtoms.AutoSize = true;
         this.lblAtoms.Location = new System.Drawing.Point(188, 69);
         this.lblAtoms.Name = "lblAtoms";
         this.lblAtoms.Size = new System.Drawing.Size(39, 13);
         this.lblAtoms.TabIndex = 9;
         this.lblAtoms.Text = "Atoms:";
         // 
         // lblFrames
         // 
         this.lblFrames.AutoSize = true;
         this.lblFrames.Location = new System.Drawing.Point(183, 44);
         this.lblFrames.Name = "lblFrames";
         this.lblFrames.Size = new System.Drawing.Size(44, 13);
         this.lblFrames.TabIndex = 6;
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
         this.listBox1.Size = new System.Drawing.Size(65, 394);
         this.listBox1.TabIndex = 0;
         this.listBox1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseUp);
         this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
         this.listBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.listBox1_MouseDown);
         // 
         // tabControl1
         // 
         this.tabControl1.Controls.Add(this.tabTextBenchmark);
         this.tabControl1.Controls.Add(this.tabGraphFrameTime);
         this.tabControl1.Controls.Add(this.tabGraphPPD);
         this.tabControl1.Controls.Add(this.tabGraphColors);
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
         // tabGraphFrameTime
         // 
         this.tabGraphFrameTime.Controls.Add(this.zgFrameTime);
         this.tabGraphFrameTime.Location = new System.Drawing.Point(4, 22);
         this.tabGraphFrameTime.Name = "tabGraphFrameTime";
         this.tabGraphFrameTime.Padding = new System.Windows.Forms.Padding(3);
         this.tabGraphFrameTime.Size = new System.Drawing.Size(479, 369);
         this.tabGraphFrameTime.TabIndex = 2;
         this.tabGraphFrameTime.Text = "Graph - Frame Time";
         this.tabGraphFrameTime.UseVisualStyleBackColor = true;
         // 
         // zgFrameTime
         // 
         this.zgFrameTime.Dock = System.Windows.Forms.DockStyle.Fill;
         this.zgFrameTime.Location = new System.Drawing.Point(3, 3);
         this.zgFrameTime.Name = "zgFrameTime";
         this.zgFrameTime.ScrollGrace = 0;
         this.zgFrameTime.ScrollMaxX = 0;
         this.zgFrameTime.ScrollMaxY = 0;
         this.zgFrameTime.ScrollMaxY2 = 0;
         this.zgFrameTime.ScrollMinX = 0;
         this.zgFrameTime.ScrollMinY = 0;
         this.zgFrameTime.ScrollMinY2 = 0;
         this.zgFrameTime.Size = new System.Drawing.Size(473, 363);
         this.zgFrameTime.TabIndex = 0;
         // 
         // tabGraphPPD
         // 
         this.tabGraphPPD.Controls.Add(this.zgPpd);
         this.tabGraphPPD.Location = new System.Drawing.Point(4, 22);
         this.tabGraphPPD.Name = "tabGraphPPD";
         this.tabGraphPPD.Padding = new System.Windows.Forms.Padding(3);
         this.tabGraphPPD.Size = new System.Drawing.Size(479, 369);
         this.tabGraphPPD.TabIndex = 1;
         this.tabGraphPPD.Text = "Graph - PPD";
         this.tabGraphPPD.UseVisualStyleBackColor = true;
         // 
         // zgPpd
         // 
         this.zgPpd.Dock = System.Windows.Forms.DockStyle.Fill;
         this.zgPpd.Location = new System.Drawing.Point(3, 3);
         this.zgPpd.Name = "zgPpd";
         this.zgPpd.ScrollGrace = 0;
         this.zgPpd.ScrollMaxX = 0;
         this.zgPpd.ScrollMaxY = 0;
         this.zgPpd.ScrollMaxY2 = 0;
         this.zgPpd.ScrollMinX = 0;
         this.zgPpd.ScrollMinY = 0;
         this.zgPpd.ScrollMinY2 = 0;
         this.zgPpd.Size = new System.Drawing.Size(473, 363);
         this.zgPpd.TabIndex = 0;
         // 
         // tabGraphColors
         // 
         this.tabGraphColors.Controls.Add(this.picColorPreview);
         this.tabGraphColors.Controls.Add(this.btnMoveColorDown);
         this.tabGraphColors.Controls.Add(this.btnMoveColorUp);
         this.tabGraphColors.Controls.Add(this.btnDeleteColor);
         this.tabGraphColors.Controls.Add(this.btnAddColor);
         this.tabGraphColors.Controls.Add(this.lstColors);
         this.tabGraphColors.Location = new System.Drawing.Point(4, 22);
         this.tabGraphColors.Name = "tabGraphColors";
         this.tabGraphColors.Size = new System.Drawing.Size(479, 369);
         this.tabGraphColors.TabIndex = 3;
         this.tabGraphColors.Text = "Graph Colors";
         this.tabGraphColors.UseVisualStyleBackColor = true;
         // 
         // picColorPreview
         // 
         this.picColorPreview.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.picColorPreview.Location = new System.Drawing.Point(170, 12);
         this.picColorPreview.Name = "picColorPreview";
         this.picColorPreview.Size = new System.Drawing.Size(66, 66);
         this.picColorPreview.TabIndex = 5;
         this.picColorPreview.TabStop = false;
         // 
         // btnMoveColorDown
         // 
         this.btnMoveColorDown.Image = global::HFM.Properties.Resources.DownArrow;
         this.btnMoveColorDown.Location = new System.Drawing.Point(123, 48);
         this.btnMoveColorDown.Name = "btnMoveColorDown";
         this.btnMoveColorDown.Size = new System.Drawing.Size(30, 30);
         this.btnMoveColorDown.TabIndex = 2;
         this.toolTip1.SetToolTip(this.btnMoveColorDown, "Move Color Down");
         this.btnMoveColorDown.UseVisualStyleBackColor = true;
         this.btnMoveColorDown.Click += new System.EventHandler(this.btnMoveColorDown_Click);
         // 
         // btnMoveColorUp
         // 
         this.btnMoveColorUp.Image = global::HFM.Properties.Resources.UpArrow;
         this.btnMoveColorUp.Location = new System.Drawing.Point(123, 12);
         this.btnMoveColorUp.Name = "btnMoveColorUp";
         this.btnMoveColorUp.Size = new System.Drawing.Size(30, 30);
         this.btnMoveColorUp.TabIndex = 1;
         this.toolTip1.SetToolTip(this.btnMoveColorUp, "Move Color Up");
         this.btnMoveColorUp.UseVisualStyleBackColor = true;
         this.btnMoveColorUp.Click += new System.EventHandler(this.btnMoveColorUp_Click);
         // 
         // btnDeleteColor
         // 
         this.btnDeleteColor.Image = global::HFM.Properties.Resources.Delete;
         this.btnDeleteColor.Location = new System.Drawing.Point(123, 120);
         this.btnDeleteColor.Name = "btnDeleteColor";
         this.btnDeleteColor.Size = new System.Drawing.Size(30, 30);
         this.btnDeleteColor.TabIndex = 4;
         this.toolTip1.SetToolTip(this.btnDeleteColor, "Delete Color");
         this.btnDeleteColor.UseVisualStyleBackColor = true;
         this.btnDeleteColor.Click += new System.EventHandler(this.btnDeleteColor_Click);
         // 
         // btnAddColor
         // 
         this.btnAddColor.Image = global::HFM.Properties.Resources.Color;
         this.btnAddColor.Location = new System.Drawing.Point(123, 84);
         this.btnAddColor.Name = "btnAddColor";
         this.btnAddColor.Size = new System.Drawing.Size(30, 30);
         this.btnAddColor.TabIndex = 3;
         this.toolTip1.SetToolTip(this.btnAddColor, "Add Color");
         this.btnAddColor.UseVisualStyleBackColor = true;
         this.btnAddColor.Click += new System.EventHandler(this.btnAddColor_Click);
         // 
         // lstColors
         // 
         this.lstColors.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)));
         this.lstColors.FormattingEnabled = true;
         this.lstColors.Location = new System.Drawing.Point(3, 6);
         this.lstColors.Name = "lstColors";
         this.lstColors.Size = new System.Drawing.Size(111, 355);
         this.lstColors.TabIndex = 0;
         this.lstColors.SelectedIndexChanged += new System.EventHandler(this.lstColors_SelectedIndexChanged);
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
         this.picDeleteClient.Image = global::HFM.Properties.Resources.Quit;
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
         // frmBenchmarks
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnExit;
         this.ClientSize = new System.Drawing.Size(556, 595);
         this.Controls.Add(this.tableLayoutPanel1);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(564, 300);
         this.Name = "frmBenchmarks";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Benchmarks";
         this.Shown += new System.EventHandler(this.frmBenchmarks_Shown);
         this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmBenchmarks_FormClosing);
         this.grpProjectInfo.ResumeLayout(false);
         this.grpProjectInfo.PerformLayout();
         this.tableLayoutPanel1.ResumeLayout(false);
         this.splitContainerBench.Panel1.ResumeLayout(false);
         this.splitContainerBench.Panel2.ResumeLayout(false);
         this.splitContainerBench.ResumeLayout(false);
         this.tabControl1.ResumeLayout(false);
         this.tabTextBenchmark.ResumeLayout(false);
         this.tabTextBenchmark.PerformLayout();
         this.tabGraphFrameTime.ResumeLayout(false);
         this.tabGraphPPD.ResumeLayout(false);
         this.tabGraphColors.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.picColorPreview)).EndInit();
         this.grpClients.ResumeLayout(false);
         ((System.ComponentModel.ISupportInitialize)(this.picDeleteClient)).EndInit();
         this.listBox1ContextMenuStrip.ResumeLayout(false);
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
      private System.Windows.Forms.TabPage tabGraphPPD;
      private ZedGraph.ZedGraphControl zgPpd;
      private System.Windows.Forms.TabPage tabGraphFrameTime;
      private ZedGraph.ZedGraphControl zgFrameTime;
      private System.Windows.Forms.TabPage tabGraphColors;
      private ButtonWrapper btnMoveColorDown;
      private ButtonWrapper btnMoveColorUp;
      private ButtonWrapper btnDeleteColor;
      private ButtonWrapper btnAddColor;
      private System.Windows.Forms.ListBox lstColors;
      private System.Windows.Forms.PictureBox picColorPreview;


   }
}
