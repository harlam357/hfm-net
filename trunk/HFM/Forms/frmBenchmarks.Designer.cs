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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmBenchmarks));
         this.listBox1 = new System.Windows.Forms.ListBox();
         this.splitContainerBench = new HFM.Classes.SplitContainerWrapper();
         this.txtBenchmarks = new HFM.Classes.TextBoxWrapper();
         this.btnExit = new HFM.Classes.ButtonWrapper();
         this.splitContainerMain = new HFM.Classes.SplitContainerWrapper();
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
         this.splitContainerBench.Panel1.SuspendLayout();
         this.splitContainerBench.Panel2.SuspendLayout();
         this.splitContainerBench.SuspendLayout();
         this.splitContainerMain.Panel1.SuspendLayout();
         this.splitContainerMain.Panel2.SuspendLayout();
         this.splitContainerMain.SuspendLayout();
         this.grpProjectInfo.SuspendLayout();
         this.SuspendLayout();
         // 
         // listBox1
         // 
         this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.listBox1.FormattingEnabled = true;
         this.listBox1.Location = new System.Drawing.Point(0, 0);
         this.listBox1.Name = "listBox1";
         this.listBox1.Size = new System.Drawing.Size(65, 342);
         this.listBox1.TabIndex = 0;
         this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
         // 
         // splitContainerBench
         // 
         this.splitContainerBench.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerBench.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
         this.splitContainerBench.Location = new System.Drawing.Point(0, 0);
         this.splitContainerBench.Name = "splitContainerBench";
         // 
         // splitContainerBench.Panel1
         // 
         this.splitContainerBench.Panel1.Controls.Add(this.listBox1);
         // 
         // splitContainerBench.Panel2
         // 
         this.splitContainerBench.Panel2.Controls.Add(this.txtBenchmarks);
         this.splitContainerBench.Panel2.Controls.Add(this.btnExit);
         this.splitContainerBench.Size = new System.Drawing.Size(556, 353);
         this.splitContainerBench.SplitterDistance = 65;
         this.splitContainerBench.TabIndex = 2;
         // 
         // txtBenchmarks
         // 
         this.txtBenchmarks.BackColor = System.Drawing.Color.White;
         this.txtBenchmarks.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtBenchmarks.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.txtBenchmarks.Location = new System.Drawing.Point(0, 0);
         this.txtBenchmarks.Multiline = true;
         this.txtBenchmarks.Name = "txtBenchmarks";
         this.txtBenchmarks.ReadOnly = true;
         this.txtBenchmarks.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
         this.txtBenchmarks.Size = new System.Drawing.Size(487, 353);
         this.txtBenchmarks.TabIndex = 0;
         // 
         // btnExit
         // 
         this.btnExit.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnExit.Location = new System.Drawing.Point(385, 182);
         this.btnExit.Name = "btnExit";
         this.btnExit.Size = new System.Drawing.Size(56, 39);
         this.btnExit.TabIndex = 11;
         this.btnExit.Text = "Exit";
         this.btnExit.UseVisualStyleBackColor = true;
         this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
         // 
         // splitContainerMain
         // 
         this.splitContainerMain.Dock = System.Windows.Forms.DockStyle.Fill;
         this.splitContainerMain.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
         this.splitContainerMain.IsSplitterFixed = true;
         this.splitContainerMain.Location = new System.Drawing.Point(0, 0);
         this.splitContainerMain.Name = "splitContainerMain";
         this.splitContainerMain.Orientation = System.Windows.Forms.Orientation.Horizontal;
         // 
         // splitContainerMain.Panel1
         // 
         this.splitContainerMain.Panel1.Controls.Add(this.splitContainerBench);
         // 
         // splitContainerMain.Panel2
         // 
         this.splitContainerMain.Panel2.Controls.Add(this.grpProjectInfo);
         this.splitContainerMain.Size = new System.Drawing.Size(556, 492);
         this.splitContainerMain.SplitterDistance = 353;
         this.splitContainerMain.SplitterWidth = 2;
         this.splitContainerMain.TabIndex = 3;
         // 
         // grpProjectInfo
         // 
         this.grpProjectInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                     | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
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
         this.grpProjectInfo.Location = new System.Drawing.Point(10, 6);
         this.grpProjectInfo.Margin = new System.Windows.Forms.Padding(10);
         this.grpProjectInfo.Name = "grpProjectInfo";
         this.grpProjectInfo.Size = new System.Drawing.Size(536, 121);
         this.grpProjectInfo.TabIndex = 0;
         this.grpProjectInfo.TabStop = false;
         this.grpProjectInfo.Text = "Project Information";
         // 
         // txtServerIP
         // 
         this.txtServerIP.Location = new System.Drawing.Point(429, 89);
         this.txtServerIP.Name = "txtServerIP";
         this.txtServerIP.ReadOnly = true;
         this.txtServerIP.Size = new System.Drawing.Size(95, 20);
         this.txtServerIP.TabIndex = 19;
         this.txtServerIP.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtContact
         // 
         this.txtContact.Location = new System.Drawing.Point(429, 66);
         this.txtContact.Name = "txtContact";
         this.txtContact.ReadOnly = true;
         this.txtContact.Size = new System.Drawing.Size(95, 20);
         this.txtContact.TabIndex = 18;
         this.txtContact.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtMaximumDays
         // 
         this.txtMaximumDays.Location = new System.Drawing.Point(429, 41);
         this.txtMaximumDays.Name = "txtMaximumDays";
         this.txtMaximumDays.ReadOnly = true;
         this.txtMaximumDays.Size = new System.Drawing.Size(95, 20);
         this.txtMaximumDays.TabIndex = 17;
         this.txtMaximumDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtPreferredDays
         // 
         this.txtPreferredDays.Location = new System.Drawing.Point(429, 17);
         this.txtPreferredDays.Name = "txtPreferredDays";
         this.txtPreferredDays.ReadOnly = true;
         this.txtPreferredDays.Size = new System.Drawing.Size(95, 20);
         this.txtPreferredDays.TabIndex = 16;
         this.txtPreferredDays.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCore
         // 
         this.txtCore.Location = new System.Drawing.Point(55, 66);
         this.txtCore.Name = "txtCore";
         this.txtCore.ReadOnly = true;
         this.txtCore.Size = new System.Drawing.Size(122, 20);
         this.txtCore.TabIndex = 15;
         this.txtCore.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtAtoms
         // 
         this.txtAtoms.Location = new System.Drawing.Point(233, 66);
         this.txtAtoms.Name = "txtAtoms";
         this.txtAtoms.ReadOnly = true;
         this.txtAtoms.Size = new System.Drawing.Size(92, 20);
         this.txtAtoms.TabIndex = 14;
         this.txtAtoms.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtFrames
         // 
         this.txtFrames.Location = new System.Drawing.Point(233, 41);
         this.txtFrames.Name = "txtFrames";
         this.txtFrames.ReadOnly = true;
         this.txtFrames.Size = new System.Drawing.Size(92, 20);
         this.txtFrames.TabIndex = 13;
         this.txtFrames.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCredit
         // 
         this.txtCredit.Location = new System.Drawing.Point(55, 41);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(122, 20);
         this.txtCredit.TabIndex = 12;
         this.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtProjectID
         // 
         this.txtProjectID.Location = new System.Drawing.Point(75, 17);
         this.txtProjectID.Name = "txtProjectID";
         this.txtProjectID.ReadOnly = true;
         this.txtProjectID.Size = new System.Drawing.Size(250, 20);
         this.txtProjectID.TabIndex = 11;
         this.txtProjectID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // linkDescription
         // 
         this.linkDescription.AutoSize = true;
         this.linkDescription.Location = new System.Drawing.Point(76, 92);
         this.linkDescription.Name = "linkDescription";
         this.linkDescription.Size = new System.Drawing.Size(55, 13);
         this.linkDescription.TabIndex = 10;
         this.linkDescription.TabStop = true;
         this.linkDescription.Text = "linkLabel1";
         this.linkDescription.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkDescription_LinkClicked);
         // 
         // lblServerIP
         // 
         this.lblServerIP.AutoSize = true;
         this.lblServerIP.Location = new System.Drawing.Point(369, 92);
         this.lblServerIP.Name = "lblServerIP";
         this.lblServerIP.Size = new System.Drawing.Size(54, 13);
         this.lblServerIP.TabIndex = 9;
         this.lblServerIP.Text = "Server IP:";
         // 
         // lblContact
         // 
         this.lblContact.AutoSize = true;
         this.lblContact.Location = new System.Drawing.Point(376, 69);
         this.lblContact.Name = "lblContact";
         this.lblContact.Size = new System.Drawing.Size(47, 13);
         this.lblContact.TabIndex = 8;
         this.lblContact.Text = "Contact:";
         // 
         // lblCore
         // 
         this.lblCore.AutoSize = true;
         this.lblCore.Location = new System.Drawing.Point(12, 69);
         this.lblCore.Name = "lblCore";
         this.lblCore.Size = new System.Drawing.Size(32, 13);
         this.lblCore.TabIndex = 7;
         this.lblCore.Text = "Core:";
         // 
         // lblMaxDays
         // 
         this.lblMaxDays.AutoSize = true;
         this.lblMaxDays.Location = new System.Drawing.Point(342, 44);
         this.lblMaxDays.Name = "lblMaxDays";
         this.lblMaxDays.Size = new System.Drawing.Size(81, 13);
         this.lblMaxDays.TabIndex = 6;
         this.lblMaxDays.Text = "Maximum Days:";
         // 
         // lblPreferred
         // 
         this.lblPreferred.AutoSize = true;
         this.lblPreferred.Location = new System.Drawing.Point(343, 20);
         this.lblPreferred.Name = "lblPreferred";
         this.lblPreferred.Size = new System.Drawing.Size(80, 13);
         this.lblPreferred.TabIndex = 5;
         this.lblPreferred.Text = "Preferred Days:";
         // 
         // lblAtoms
         // 
         this.lblAtoms.AutoSize = true;
         this.lblAtoms.Location = new System.Drawing.Point(188, 69);
         this.lblAtoms.Name = "lblAtoms";
         this.lblAtoms.Size = new System.Drawing.Size(39, 13);
         this.lblAtoms.TabIndex = 4;
         this.lblAtoms.Text = "Atoms:";
         // 
         // lblFrames
         // 
         this.lblFrames.AutoSize = true;
         this.lblFrames.Location = new System.Drawing.Point(183, 44);
         this.lblFrames.Name = "lblFrames";
         this.lblFrames.Size = new System.Drawing.Size(44, 13);
         this.lblFrames.TabIndex = 3;
         this.lblFrames.Text = "Frames:";
         // 
         // lblCredit
         // 
         this.lblCredit.AutoSize = true;
         this.lblCredit.Location = new System.Drawing.Point(12, 44);
         this.lblCredit.Name = "lblCredit";
         this.lblCredit.Size = new System.Drawing.Size(37, 13);
         this.lblCredit.TabIndex = 2;
         this.lblCredit.Text = "Credit:";
         // 
         // lblDescription
         // 
         this.lblDescription.AutoSize = true;
         this.lblDescription.Location = new System.Drawing.Point(12, 92);
         this.lblDescription.Name = "lblDescription";
         this.lblDescription.Size = new System.Drawing.Size(63, 13);
         this.lblDescription.TabIndex = 1;
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
         // frmBenchmarks
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.CancelButton = this.btnExit;
         this.ClientSize = new System.Drawing.Size(556, 492);
         this.Controls.Add(this.splitContainerMain);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(564, 200);
         this.Name = "frmBenchmarks";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Benchmarks";
         this.Shown += new System.EventHandler(this.frmBenchmarks_Shown);
         this.splitContainerBench.Panel1.ResumeLayout(false);
         this.splitContainerBench.Panel2.ResumeLayout(false);
         this.splitContainerBench.Panel2.PerformLayout();
         this.splitContainerBench.ResumeLayout(false);
         this.splitContainerMain.Panel1.ResumeLayout(false);
         this.splitContainerMain.Panel2.ResumeLayout(false);
         this.splitContainerMain.ResumeLayout(false);
         this.grpProjectInfo.ResumeLayout(false);
         this.grpProjectInfo.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private System.Windows.Forms.ListBox listBox1;
      private HFM.Classes.SplitContainerWrapper splitContainerBench;
      private HFM.Classes.TextBoxWrapper txtBenchmarks;
      private HFM.Classes.SplitContainerWrapper splitContainerMain;
      private HFM.Classes.GroupBoxWrapper grpProjectInfo;
      private HFM.Classes.LabelWrapper lblProjectID;
      private HFM.Classes.LabelWrapper lblCredit;
      private HFM.Classes.LabelWrapper lblDescription;
      private HFM.Classes.LabelWrapper lblFrames;
      private HFM.Classes.LabelWrapper lblMaxDays;
      private HFM.Classes.LabelWrapper lblPreferred;
      private HFM.Classes.LabelWrapper lblAtoms;
      private HFM.Classes.LabelWrapper lblServerIP;
      private HFM.Classes.LabelWrapper lblContact;
      private HFM.Classes.LabelWrapper lblCore;
      private System.Windows.Forms.LinkLabel linkDescription;
      private ButtonWrapper btnExit;
      private TextBoxWrapper txtProjectID;
      private TextBoxWrapper txtAtoms;
      private TextBoxWrapper txtFrames;
      private TextBoxWrapper txtCredit;
      private TextBoxWrapper txtServerIP;
      private TextBoxWrapper txtContact;
      private TextBoxWrapper txtMaximumDays;
      private TextBoxWrapper txtPreferredDays;
      private TextBoxWrapper txtCore;

   }
}
