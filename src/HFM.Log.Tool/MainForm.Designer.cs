namespace HFM.Log.Tool
{
   partial class MainForm
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
         this.txtLogPath = new System.Windows.Forms.TextBox();
         this.btnBrowse = new System.Windows.Forms.Button();
         this.btnParse = new System.Windows.Forms.Button();
         this.treeView1 = new System.Windows.Forms.TreeView();
         this.btnGenCode = new System.Windows.Forms.Button();
         this.splitContainer1 = new System.Windows.Forms.SplitContainer();
         this.lblLogLineIndex = new System.Windows.Forms.Label();
         this.lblLogLineType = new System.Windows.Forms.Label();
         this.lblLogLineData = new System.Windows.Forms.Label();
         this.txtLogLineIndex = new System.Windows.Forms.TextBox();
         this.txtLogLineType = new System.Windows.Forms.TextBox();
         this.txtLogLineData = new System.Windows.Forms.TextBox();
         this.panel1 = new System.Windows.Forms.Panel();
         this.LegacyRadioButton = new System.Windows.Forms.RadioButton();
         this.Version7RadioButton = new System.Windows.Forms.RadioButton();
         this.richTextBox1 = new HFM.Forms.Controls.RichTextBoxWrapper();
         this.splitContainer1.Panel1.SuspendLayout();
         this.splitContainer1.Panel2.SuspendLayout();
         this.splitContainer1.SuspendLayout();
         this.panel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // txtLogPath
         // 
         this.txtLogPath.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtLogPath.Location = new System.Drawing.Point(12, 477);
         this.txtLogPath.Name = "txtLogPath";
         this.txtLogPath.Size = new System.Drawing.Size(371, 20);
         this.txtLogPath.TabIndex = 0;
         // 
         // btnBrowse
         // 
         this.btnBrowse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnBrowse.Location = new System.Drawing.Point(389, 474);
         this.btnBrowse.Name = "btnBrowse";
         this.btnBrowse.Size = new System.Drawing.Size(24, 24);
         this.btnBrowse.TabIndex = 1;
         this.btnBrowse.Text = "...";
         this.btnBrowse.UseVisualStyleBackColor = true;
         this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
         // 
         // btnParse
         // 
         this.btnParse.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnParse.Location = new System.Drawing.Point(432, 474);
         this.btnParse.Name = "btnParse";
         this.btnParse.Size = new System.Drawing.Size(108, 24);
         this.btnParse.TabIndex = 2;
         this.btnParse.Text = "Parse Logs";
         this.btnParse.UseVisualStyleBackColor = true;
         this.btnParse.Click += new System.EventHandler(this.btnParse_Click);
         // 
         // treeView1
         // 
         this.treeView1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.treeView1.Location = new System.Drawing.Point(0, 0);
         this.treeView1.Name = "treeView1";
         this.treeView1.Size = new System.Drawing.Size(220, 419);
         this.treeView1.TabIndex = 4;
         this.treeView1.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterSelect);
         // 
         // btnGenCode
         // 
         this.btnGenCode.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.btnGenCode.Location = new System.Drawing.Point(546, 474);
         this.btnGenCode.Name = "btnGenCode";
         this.btnGenCode.Size = new System.Drawing.Size(118, 24);
         this.btnGenCode.TabIndex = 6;
         this.btnGenCode.Text = "Gen Unit Test Code";
         this.btnGenCode.UseVisualStyleBackColor = true;
         this.btnGenCode.Click += new System.EventHandler(this.btnGenCode_Click);
         // 
         // splitContainer1
         // 
         this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
         this.splitContainer1.Location = new System.Drawing.Point(12, 12);
         this.splitContainer1.Name = "splitContainer1";
         // 
         // splitContainer1.Panel1
         // 
         this.splitContainer1.Panel1.Controls.Add(this.treeView1);
         // 
         // splitContainer1.Panel2
         // 
         this.splitContainer1.Panel2.Controls.Add(this.richTextBox1);
         this.splitContainer1.Size = new System.Drawing.Size(862, 419);
         this.splitContainer1.SplitterDistance = 220;
         this.splitContainer1.TabIndex = 7;
         // 
         // lblLogLineIndex
         // 
         this.lblLogLineIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblLogLineIndex.AutoSize = true;
         this.lblLogLineIndex.Location = new System.Drawing.Point(12, 448);
         this.lblLogLineIndex.Name = "lblLogLineIndex";
         this.lblLogLineIndex.Size = new System.Drawing.Size(36, 13);
         this.lblLogLineIndex.TabIndex = 8;
         this.lblLogLineIndex.Text = "Index:";
         // 
         // lblLogLineType
         // 
         this.lblLogLineType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblLogLineType.AutoSize = true;
         this.lblLogLineType.Location = new System.Drawing.Point(131, 448);
         this.lblLogLineType.Name = "lblLogLineType";
         this.lblLogLineType.Size = new System.Drawing.Size(34, 13);
         this.lblLogLineType.TabIndex = 9;
         this.lblLogLineType.Text = "Type:";
         // 
         // lblLogLineData
         // 
         this.lblLogLineData.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblLogLineData.AutoSize = true;
         this.lblLogLineData.Location = new System.Drawing.Point(429, 448);
         this.lblLogLineData.Name = "lblLogLineData";
         this.lblLogLineData.Size = new System.Drawing.Size(33, 13);
         this.lblLogLineData.TabIndex = 10;
         this.lblLogLineData.Text = "Data:";
         // 
         // txtLogLineIndex
         // 
         this.txtLogLineIndex.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtLogLineIndex.Location = new System.Drawing.Point(54, 445);
         this.txtLogLineIndex.Name = "txtLogLineIndex";
         this.txtLogLineIndex.ReadOnly = true;
         this.txtLogLineIndex.Size = new System.Drawing.Size(58, 20);
         this.txtLogLineIndex.TabIndex = 11;
         // 
         // txtLogLineType
         // 
         this.txtLogLineType.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.txtLogLineType.Location = new System.Drawing.Point(171, 445);
         this.txtLogLineType.Name = "txtLogLineType";
         this.txtLogLineType.ReadOnly = true;
         this.txtLogLineType.Size = new System.Drawing.Size(242, 20);
         this.txtLogLineType.TabIndex = 12;
         // 
         // txtLogLineData
         // 
         this.txtLogLineData.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.txtLogLineData.Location = new System.Drawing.Point(468, 445);
         this.txtLogLineData.Name = "txtLogLineData";
         this.txtLogLineData.ReadOnly = true;
         this.txtLogLineData.Size = new System.Drawing.Size(406, 20);
         this.txtLogLineData.TabIndex = 13;
         // 
         // panel1
         // 
         this.panel1.Controls.Add(this.Version7RadioButton);
         this.panel1.Controls.Add(this.LegacyRadioButton);
         this.panel1.Location = new System.Drawing.Point(670, 474);
         this.panel1.Name = "panel1";
         this.panel1.Size = new System.Drawing.Size(174, 23);
         this.panel1.TabIndex = 14;
         // 
         // LegacyRadioButton
         // 
         this.LegacyRadioButton.AutoSize = true;
         this.LegacyRadioButton.Checked = true;
         this.LegacyRadioButton.Location = new System.Drawing.Point(3, 3);
         this.LegacyRadioButton.Name = "LegacyRadioButton";
         this.LegacyRadioButton.Size = new System.Drawing.Size(60, 17);
         this.LegacyRadioButton.TabIndex = 0;
         this.LegacyRadioButton.TabStop = true;
         this.LegacyRadioButton.Text = "Legacy";
         this.LegacyRadioButton.UseVisualStyleBackColor = true;
         // 
         // Version7RadioButton
         // 
         this.Version7RadioButton.AutoSize = true;
         this.Version7RadioButton.Location = new System.Drawing.Point(69, 3);
         this.Version7RadioButton.Name = "Version7RadioButton";
         this.Version7RadioButton.Size = new System.Drawing.Size(69, 17);
         this.Version7RadioButton.TabIndex = 1;
         this.Version7RadioButton.Text = "Version 7";
         this.Version7RadioButton.UseVisualStyleBackColor = true;
         // 
         // richTextBox1
         // 
         this.richTextBox1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.richTextBox1.Font = new System.Drawing.Font("Courier New", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
         this.richTextBox1.Location = new System.Drawing.Point(0, 0);
         this.richTextBox1.Name = "richTextBox1";
         this.richTextBox1.Size = new System.Drawing.Size(638, 419);
         this.richTextBox1.TabIndex = 3;
         this.richTextBox1.Text = "";
         this.richTextBox1.WordWrap = false;
         this.richTextBox1.KeyUp += new System.Windows.Forms.KeyEventHandler(this.richTextBox1_KeyUp);
         this.richTextBox1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.richTextBox1_MouseDown);
         // 
         // MainForm
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(886, 509);
         this.Controls.Add(this.panel1);
         this.Controls.Add(this.txtLogLineData);
         this.Controls.Add(this.txtLogLineType);
         this.Controls.Add(this.txtLogLineIndex);
         this.Controls.Add(this.lblLogLineData);
         this.Controls.Add(this.lblLogLineType);
         this.Controls.Add(this.lblLogLineIndex);
         this.Controls.Add(this.splitContainer1);
         this.Controls.Add(this.btnGenCode);
         this.Controls.Add(this.btnParse);
         this.Controls.Add(this.btnBrowse);
         this.Controls.Add(this.txtLogPath);
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MinimumSize = new System.Drawing.Size(902, 547);
         this.Name = "MainForm";
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
         this.Text = "HFM.NET Log Tool";
         this.splitContainer1.Panel1.ResumeLayout(false);
         this.splitContainer1.Panel2.ResumeLayout(false);
         this.splitContainer1.ResumeLayout(false);
         this.panel1.ResumeLayout(false);
         this.panel1.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      #endregion

      private System.Windows.Forms.TextBox txtLogPath;
      private System.Windows.Forms.Button btnBrowse;
      private System.Windows.Forms.Button btnParse;
      private HFM.Forms.Controls.RichTextBoxWrapper richTextBox1;
      private System.Windows.Forms.TreeView treeView1;
      private System.Windows.Forms.Button btnGenCode;
      private System.Windows.Forms.SplitContainer splitContainer1;
      private System.Windows.Forms.Label lblLogLineIndex;
      private System.Windows.Forms.Label lblLogLineType;
      private System.Windows.Forms.Label lblLogLineData;
      private System.Windows.Forms.TextBox txtLogLineIndex;
      private System.Windows.Forms.TextBox txtLogLineType;
      private System.Windows.Forms.TextBox txtLogLineData;
      private System.Windows.Forms.Panel panel1;
      private System.Windows.Forms.RadioButton Version7RadioButton;
      private System.Windows.Forms.RadioButton LegacyRadioButton;
   }
}

