/*
 * HFM.NET - Queue Control
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; version 2
 * of the License. See the included file GPLv2.TXT.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

namespace HFM.Forms.Controls
{
   partial class QueueControl
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

      #region Component Designer generated code

      /// <summary> 
      /// Required method for Designer support - do not modify 
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {
         this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
         this.cboQueueIndex = new HFM.Forms.Controls.ComboBoxWrapper();
         this.txtMachineID = new HFM.Forms.Controls.TextBoxWrapper();
         this.labelWrapper1 = new HFM.Forms.Controls.LabelWrapper();
         this.lblMachineID = new HFM.Forms.Controls.LabelWrapper();
         this.txtStatus = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtUserID = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtCoresToUse = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblUserID = new HFM.Forms.Controls.LabelWrapper();
         this.txtSmpCores = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblCoresToUse = new HFM.Forms.Controls.LabelWrapper();
         this.txtAverageUploadRate = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblCredit = new HFM.Forms.Controls.LabelWrapper();
         this.lblBenchmark = new HFM.Forms.Controls.LabelWrapper();
         this.lblSmpCores = new HFM.Forms.Controls.LabelWrapper();
         this.txtMemory = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblAverageUploadRate = new HFM.Forms.Controls.LabelWrapper();
         this.txtAverageDownloadRate = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtCredit = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtOsType = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblBeginDate = new HFM.Forms.Controls.LabelWrapper();
         this.lblMemory = new HFM.Forms.Controls.LabelWrapper();
         this.txtCpuType = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblAverageDownloadRate = new HFM.Forms.Controls.LabelWrapper();
         this.txtBeginDate = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtPerformanceFraction = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblEndDate = new HFM.Forms.Controls.LabelWrapper();
         this.lblOsType = new HFM.Forms.Controls.LabelWrapper();
         this.lblPerformanceFraction = new HFM.Forms.Controls.LabelWrapper();
         this.txtMegaFlops = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtEndDate = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblCpuType = new HFM.Forms.Controls.LabelWrapper();
         this.lblSpeedFactor = new HFM.Forms.Controls.LabelWrapper();
         this.txtSpeedFactor = new HFM.Forms.Controls.TextBoxWrapper();
         this.lblMegaFlops = new HFM.Forms.Controls.LabelWrapper();
         this.lblServer = new HFM.Forms.Controls.LabelWrapper();
         this.txtServer = new HFM.Forms.Controls.TextBoxWrapper();
         this.txtBenchmark = new HFM.Forms.Controls.TextBoxWrapper();
         this.labelWrapper2 = new HFM.Forms.Controls.LabelWrapper();
         this.labelWrapper3 = new HFM.Forms.Controls.LabelWrapper();
         this.labelWrapper4 = new HFM.Forms.Controls.LabelWrapper();
         this.WaitingOnTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.AttemptsTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.NextAttemptTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         this.tableLayoutPanel1.SuspendLayout();
         this.SuspendLayout();
         // 
         // tableLayoutPanel1
         // 
         this.tableLayoutPanel1.AutoScroll = true;
         this.tableLayoutPanel1.ColumnCount = 2;
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 90F));
         this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
         this.tableLayoutPanel1.Controls.Add(this.cboQueueIndex, 0, 0);
         this.tableLayoutPanel1.Controls.Add(this.txtMachineID, 1, 22);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper1, 0, 2);
         this.tableLayoutPanel1.Controls.Add(this.lblMachineID, 0, 22);
         this.tableLayoutPanel1.Controls.Add(this.txtStatus, 1, 2);
         this.tableLayoutPanel1.Controls.Add(this.txtUserID, 1, 21);
         this.tableLayoutPanel1.Controls.Add(this.txtCoresToUse, 1, 20);
         this.tableLayoutPanel1.Controls.Add(this.lblUserID, 0, 21);
         this.tableLayoutPanel1.Controls.Add(this.txtSmpCores, 1, 19);
         this.tableLayoutPanel1.Controls.Add(this.lblCoresToUse, 0, 20);
         this.tableLayoutPanel1.Controls.Add(this.txtAverageUploadRate, 1, 14);
         this.tableLayoutPanel1.Controls.Add(this.lblCredit, 0, 6);
         this.tableLayoutPanel1.Controls.Add(this.lblBenchmark, 0, 18);
         this.tableLayoutPanel1.Controls.Add(this.lblSmpCores, 0, 19);
         this.tableLayoutPanel1.Controls.Add(this.txtMemory, 1, 17);
         this.tableLayoutPanel1.Controls.Add(this.lblAverageUploadRate, 0, 14);
         this.tableLayoutPanel1.Controls.Add(this.txtAverageDownloadRate, 1, 13);
         this.tableLayoutPanel1.Controls.Add(this.txtCredit, 1, 6);
         this.tableLayoutPanel1.Controls.Add(this.txtOsType, 1, 16);
         this.tableLayoutPanel1.Controls.Add(this.lblBeginDate, 0, 7);
         this.tableLayoutPanel1.Controls.Add(this.lblMemory, 0, 17);
         this.tableLayoutPanel1.Controls.Add(this.txtCpuType, 1, 15);
         this.tableLayoutPanel1.Controls.Add(this.lblAverageDownloadRate, 0, 13);
         this.tableLayoutPanel1.Controls.Add(this.txtBeginDate, 1, 7);
         this.tableLayoutPanel1.Controls.Add(this.txtPerformanceFraction, 1, 10);
         this.tableLayoutPanel1.Controls.Add(this.lblEndDate, 0, 8);
         this.tableLayoutPanel1.Controls.Add(this.lblOsType, 0, 16);
         this.tableLayoutPanel1.Controls.Add(this.lblPerformanceFraction, 0, 10);
         this.tableLayoutPanel1.Controls.Add(this.txtMegaFlops, 1, 11);
         this.tableLayoutPanel1.Controls.Add(this.txtEndDate, 1, 8);
         this.tableLayoutPanel1.Controls.Add(this.lblCpuType, 0, 15);
         this.tableLayoutPanel1.Controls.Add(this.lblSpeedFactor, 0, 9);
         this.tableLayoutPanel1.Controls.Add(this.txtSpeedFactor, 1, 9);
         this.tableLayoutPanel1.Controls.Add(this.lblMegaFlops, 0, 11);
         this.tableLayoutPanel1.Controls.Add(this.lblServer, 0, 12);
         this.tableLayoutPanel1.Controls.Add(this.txtServer, 1, 12);
         this.tableLayoutPanel1.Controls.Add(this.txtBenchmark, 1, 18);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper2, 0, 3);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper3, 0, 4);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper4, 0, 5);
         this.tableLayoutPanel1.Controls.Add(this.WaitingOnTextBox, 1, 3);
         this.tableLayoutPanel1.Controls.Add(this.AttemptsTextBox, 1, 4);
         this.tableLayoutPanel1.Controls.Add(this.NextAttemptTextBox, 1, 5);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(7, 5, 9, 5);
         this.tableLayoutPanel1.RowCount = 24;
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 25F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 10F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 23F));
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(236, 530);
         this.tableLayoutPanel1.TabIndex = 0;
         // 
         // cboQueueIndex
         // 
         this.tableLayoutPanel1.SetColumnSpan(this.cboQueueIndex, 2);
         this.cboQueueIndex.Dock = System.Windows.Forms.DockStyle.Fill;
         this.cboQueueIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboQueueIndex.FormattingEnabled = true;
         this.cboQueueIndex.Location = new System.Drawing.Point(10, 8);
         this.cboQueueIndex.Name = "cboQueueIndex";
         this.cboQueueIndex.Size = new System.Drawing.Size(214, 21);
         this.cboQueueIndex.TabIndex = 0;
         this.cboQueueIndex.SelectedIndexChanged += new System.EventHandler(this.cboQueueIndex_SelectedIndexChanged);
         // 
         // txtMachineID
         // 
         this.txtMachineID.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMachineID.Location = new System.Drawing.Point(100, 503);
         this.txtMachineID.Name = "txtMachineID";
         this.txtMachineID.ReadOnly = true;
         this.txtMachineID.Size = new System.Drawing.Size(124, 20);
         this.txtMachineID.TabIndex = 42;
         this.txtMachineID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.labelWrapper1.AutoSize = true;
         this.labelWrapper1.Location = new System.Drawing.Point(10, 45);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(40, 13);
         this.labelWrapper1.TabIndex = 1;
         this.labelWrapper1.Text = "Status:";
         // 
         // lblMachineID
         // 
         this.lblMachineID.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblMachineID.AutoSize = true;
         this.lblMachineID.Location = new System.Drawing.Point(10, 505);
         this.lblMachineID.Name = "lblMachineID";
         this.lblMachineID.Size = new System.Drawing.Size(65, 13);
         this.lblMachineID.TabIndex = 41;
         this.lblMachineID.Text = "Machine ID:";
         // 
         // txtStatus
         // 
         this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtStatus.Location = new System.Drawing.Point(100, 43);
         this.txtStatus.Name = "txtStatus";
         this.txtStatus.ReadOnly = true;
         this.txtStatus.Size = new System.Drawing.Size(124, 20);
         this.txtStatus.TabIndex = 2;
         this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtUserID
         // 
         this.txtUserID.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtUserID.Location = new System.Drawing.Point(100, 480);
         this.txtUserID.Name = "txtUserID";
         this.txtUserID.ReadOnly = true;
         this.txtUserID.Size = new System.Drawing.Size(124, 20);
         this.txtUserID.TabIndex = 40;
         this.txtUserID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCoresToUse
         // 
         this.txtCoresToUse.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCoresToUse.Location = new System.Drawing.Point(100, 457);
         this.txtCoresToUse.Name = "txtCoresToUse";
         this.txtCoresToUse.ReadOnly = true;
         this.txtCoresToUse.Size = new System.Drawing.Size(124, 20);
         this.txtCoresToUse.TabIndex = 38;
         this.txtCoresToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblUserID
         // 
         this.lblUserID.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblUserID.AutoSize = true;
         this.lblUserID.Location = new System.Drawing.Point(10, 482);
         this.lblUserID.Name = "lblUserID";
         this.lblUserID.Size = new System.Drawing.Size(46, 13);
         this.lblUserID.TabIndex = 39;
         this.lblUserID.Text = "User ID:";
         // 
         // txtSmpCores
         // 
         this.txtSmpCores.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtSmpCores.Location = new System.Drawing.Point(100, 434);
         this.txtSmpCores.Name = "txtSmpCores";
         this.txtSmpCores.ReadOnly = true;
         this.txtSmpCores.Size = new System.Drawing.Size(124, 20);
         this.txtSmpCores.TabIndex = 36;
         this.txtSmpCores.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCoresToUse
         // 
         this.lblCoresToUse.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCoresToUse.AutoSize = true;
         this.lblCoresToUse.Location = new System.Drawing.Point(10, 459);
         this.lblCoresToUse.Name = "lblCoresToUse";
         this.lblCoresToUse.Size = new System.Drawing.Size(75, 13);
         this.lblCoresToUse.TabIndex = 37;
         this.lblCoresToUse.Text = "Cores To Use:";
         // 
         // txtAverageUploadRate
         // 
         this.txtAverageUploadRate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtAverageUploadRate.Location = new System.Drawing.Point(100, 319);
         this.txtAverageUploadRate.Name = "txtAverageUploadRate";
         this.txtAverageUploadRate.ReadOnly = true;
         this.txtAverageUploadRate.Size = new System.Drawing.Size(124, 20);
         this.txtAverageUploadRate.TabIndex = 26;
         this.txtAverageUploadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCredit
         // 
         this.lblCredit.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCredit.AutoSize = true;
         this.lblCredit.Location = new System.Drawing.Point(10, 137);
         this.lblCredit.Name = "lblCredit";
         this.lblCredit.Size = new System.Drawing.Size(37, 13);
         this.lblCredit.TabIndex = 9;
         this.lblCredit.Text = "Credit:";
         // 
         // lblBenchmark
         // 
         this.lblBenchmark.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblBenchmark.AutoSize = true;
         this.lblBenchmark.Location = new System.Drawing.Point(10, 413);
         this.lblBenchmark.Name = "lblBenchmark";
         this.lblBenchmark.Size = new System.Drawing.Size(64, 13);
         this.lblBenchmark.TabIndex = 33;
         this.lblBenchmark.Text = "Benchmark:";
         // 
         // lblSmpCores
         // 
         this.lblSmpCores.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblSmpCores.AutoSize = true;
         this.lblSmpCores.Location = new System.Drawing.Point(10, 436);
         this.lblSmpCores.Name = "lblSmpCores";
         this.lblSmpCores.Size = new System.Drawing.Size(63, 13);
         this.lblSmpCores.TabIndex = 35;
         this.lblSmpCores.Text = "SMP Cores:";
         // 
         // txtMemory
         // 
         this.txtMemory.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMemory.Location = new System.Drawing.Point(100, 388);
         this.txtMemory.Name = "txtMemory";
         this.txtMemory.ReadOnly = true;
         this.txtMemory.Size = new System.Drawing.Size(124, 20);
         this.txtMemory.TabIndex = 32;
         this.txtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblAverageUploadRate
         // 
         this.lblAverageUploadRate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblAverageUploadRate.AutoSize = true;
         this.lblAverageUploadRate.Location = new System.Drawing.Point(10, 321);
         this.lblAverageUploadRate.Name = "lblAverageUploadRate";
         this.lblAverageUploadRate.Size = new System.Drawing.Size(69, 13);
         this.lblAverageUploadRate.TabIndex = 25;
         this.lblAverageUploadRate.Text = "Avg. Upload:";
         // 
         // txtAverageDownloadRate
         // 
         this.txtAverageDownloadRate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtAverageDownloadRate.Location = new System.Drawing.Point(100, 296);
         this.txtAverageDownloadRate.Name = "txtAverageDownloadRate";
         this.txtAverageDownloadRate.ReadOnly = true;
         this.txtAverageDownloadRate.Size = new System.Drawing.Size(124, 20);
         this.txtAverageDownloadRate.TabIndex = 24;
         this.txtAverageDownloadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCredit
         // 
         this.txtCredit.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCredit.Location = new System.Drawing.Point(100, 135);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(124, 20);
         this.txtCredit.TabIndex = 10;
         this.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtOsType
         // 
         this.txtOsType.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtOsType.Location = new System.Drawing.Point(100, 365);
         this.txtOsType.Name = "txtOsType";
         this.txtOsType.ReadOnly = true;
         this.txtOsType.Size = new System.Drawing.Size(124, 20);
         this.txtOsType.TabIndex = 30;
         this.txtOsType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblBeginDate
         // 
         this.lblBeginDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblBeginDate.AutoSize = true;
         this.lblBeginDate.Location = new System.Drawing.Point(10, 160);
         this.lblBeginDate.Name = "lblBeginDate";
         this.lblBeginDate.Size = new System.Drawing.Size(63, 13);
         this.lblBeginDate.TabIndex = 11;
         this.lblBeginDate.Text = "Begin Date:";
         // 
         // lblMemory
         // 
         this.lblMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblMemory.AutoSize = true;
         this.lblMemory.Location = new System.Drawing.Point(10, 390);
         this.lblMemory.Name = "lblMemory";
         this.lblMemory.Size = new System.Drawing.Size(47, 13);
         this.lblMemory.TabIndex = 31;
         this.lblMemory.Text = "Memory:";
         // 
         // txtCpuType
         // 
         this.txtCpuType.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCpuType.Location = new System.Drawing.Point(100, 342);
         this.txtCpuType.Name = "txtCpuType";
         this.txtCpuType.ReadOnly = true;
         this.txtCpuType.Size = new System.Drawing.Size(124, 20);
         this.txtCpuType.TabIndex = 28;
         this.txtCpuType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblAverageDownloadRate
         // 
         this.lblAverageDownloadRate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblAverageDownloadRate.AutoSize = true;
         this.lblAverageDownloadRate.Location = new System.Drawing.Point(10, 298);
         this.lblAverageDownloadRate.Name = "lblAverageDownloadRate";
         this.lblAverageDownloadRate.Size = new System.Drawing.Size(83, 13);
         this.lblAverageDownloadRate.TabIndex = 23;
         this.lblAverageDownloadRate.Text = "Avg. Download:";
         // 
         // txtBeginDate
         // 
         this.txtBeginDate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtBeginDate.Location = new System.Drawing.Point(100, 158);
         this.txtBeginDate.Name = "txtBeginDate";
         this.txtBeginDate.ReadOnly = true;
         this.txtBeginDate.Size = new System.Drawing.Size(124, 20);
         this.txtBeginDate.TabIndex = 12;
         this.txtBeginDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtPerformanceFraction
         // 
         this.txtPerformanceFraction.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtPerformanceFraction.Location = new System.Drawing.Point(100, 227);
         this.txtPerformanceFraction.Name = "txtPerformanceFraction";
         this.txtPerformanceFraction.ReadOnly = true;
         this.txtPerformanceFraction.Size = new System.Drawing.Size(124, 20);
         this.txtPerformanceFraction.TabIndex = 18;
         this.txtPerformanceFraction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblEndDate
         // 
         this.lblEndDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblEndDate.AutoSize = true;
         this.lblEndDate.Location = new System.Drawing.Point(10, 183);
         this.lblEndDate.Name = "lblEndDate";
         this.lblEndDate.Size = new System.Drawing.Size(55, 13);
         this.lblEndDate.TabIndex = 13;
         this.lblEndDate.Text = "End Date:";
         // 
         // lblOsType
         // 
         this.lblOsType.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblOsType.AutoSize = true;
         this.lblOsType.Location = new System.Drawing.Point(10, 367);
         this.lblOsType.Name = "lblOsType";
         this.lblOsType.Size = new System.Drawing.Size(25, 13);
         this.lblOsType.TabIndex = 29;
         this.lblOsType.Text = "OS:";
         // 
         // lblPerformanceFraction
         // 
         this.lblPerformanceFraction.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblPerformanceFraction.AutoSize = true;
         this.lblPerformanceFraction.Location = new System.Drawing.Point(10, 229);
         this.lblPerformanceFraction.Name = "lblPerformanceFraction";
         this.lblPerformanceFraction.Size = new System.Drawing.Size(73, 13);
         this.lblPerformanceFraction.TabIndex = 17;
         this.lblPerformanceFraction.Text = "Perf. Fraction:";
         // 
         // txtMegaFlops
         // 
         this.txtMegaFlops.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMegaFlops.Location = new System.Drawing.Point(100, 250);
         this.txtMegaFlops.Name = "txtMegaFlops";
         this.txtMegaFlops.ReadOnly = true;
         this.txtMegaFlops.Size = new System.Drawing.Size(124, 20);
         this.txtMegaFlops.TabIndex = 20;
         this.txtMegaFlops.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtEndDate
         // 
         this.txtEndDate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtEndDate.Location = new System.Drawing.Point(100, 181);
         this.txtEndDate.Name = "txtEndDate";
         this.txtEndDate.ReadOnly = true;
         this.txtEndDate.Size = new System.Drawing.Size(124, 20);
         this.txtEndDate.TabIndex = 14;
         this.txtEndDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCpuType
         // 
         this.lblCpuType.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCpuType.AutoSize = true;
         this.lblCpuType.Location = new System.Drawing.Point(10, 344);
         this.lblCpuType.Name = "lblCpuType";
         this.lblCpuType.Size = new System.Drawing.Size(59, 13);
         this.lblCpuType.TabIndex = 27;
         this.lblCpuType.Text = "CPU Type:";
         // 
         // lblSpeedFactor
         // 
         this.lblSpeedFactor.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblSpeedFactor.AutoSize = true;
         this.lblSpeedFactor.Location = new System.Drawing.Point(10, 206);
         this.lblSpeedFactor.Name = "lblSpeedFactor";
         this.lblSpeedFactor.Size = new System.Drawing.Size(74, 13);
         this.lblSpeedFactor.TabIndex = 15;
         this.lblSpeedFactor.Text = "Speed Factor:";
         // 
         // txtSpeedFactor
         // 
         this.txtSpeedFactor.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtSpeedFactor.Location = new System.Drawing.Point(100, 204);
         this.txtSpeedFactor.Name = "txtSpeedFactor";
         this.txtSpeedFactor.ReadOnly = true;
         this.txtSpeedFactor.Size = new System.Drawing.Size(124, 20);
         this.txtSpeedFactor.TabIndex = 16;
         this.txtSpeedFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblMegaFlops
         // 
         this.lblMegaFlops.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblMegaFlops.AutoSize = true;
         this.lblMegaFlops.Location = new System.Drawing.Point(10, 252);
         this.lblMegaFlops.Name = "lblMegaFlops";
         this.lblMegaFlops.Size = new System.Drawing.Size(62, 13);
         this.lblMegaFlops.TabIndex = 19;
         this.lblMegaFlops.Text = "MegaFlops:";
         // 
         // lblServer
         // 
         this.lblServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblServer.AutoSize = true;
         this.lblServer.Location = new System.Drawing.Point(10, 275);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(41, 13);
         this.lblServer.TabIndex = 21;
         this.lblServer.Text = "Server:";
         // 
         // txtServer
         // 
         this.txtServer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtServer.Location = new System.Drawing.Point(100, 273);
         this.txtServer.Name = "txtServer";
         this.txtServer.ReadOnly = true;
         this.txtServer.Size = new System.Drawing.Size(124, 20);
         this.txtServer.TabIndex = 22;
         this.txtServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtBenchmark
         // 
         this.txtBenchmark.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtBenchmark.Location = new System.Drawing.Point(100, 411);
         this.txtBenchmark.Name = "txtBenchmark";
         this.txtBenchmark.ReadOnly = true;
         this.txtBenchmark.Size = new System.Drawing.Size(124, 20);
         this.txtBenchmark.TabIndex = 34;
         this.txtBenchmark.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // labelWrapper2
         // 
         this.labelWrapper2.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.labelWrapper2.AutoSize = true;
         this.labelWrapper2.Location = new System.Drawing.Point(10, 68);
         this.labelWrapper2.Name = "labelWrapper2";
         this.labelWrapper2.Size = new System.Drawing.Size(63, 13);
         this.labelWrapper2.TabIndex = 3;
         this.labelWrapper2.Text = "Waiting On:";
         // 
         // labelWrapper3
         // 
         this.labelWrapper3.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.labelWrapper3.AutoSize = true;
         this.labelWrapper3.Location = new System.Drawing.Point(10, 91);
         this.labelWrapper3.Name = "labelWrapper3";
         this.labelWrapper3.Size = new System.Drawing.Size(51, 13);
         this.labelWrapper3.TabIndex = 5;
         this.labelWrapper3.Text = "Attempts:";
         // 
         // labelWrapper4
         // 
         this.labelWrapper4.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.labelWrapper4.AutoSize = true;
         this.labelWrapper4.Location = new System.Drawing.Point(10, 114);
         this.labelWrapper4.Name = "labelWrapper4";
         this.labelWrapper4.Size = new System.Drawing.Size(68, 13);
         this.labelWrapper4.TabIndex = 7;
         this.labelWrapper4.Text = "Next Attempt";
         // 
         // WaitingOnTextBox
         // 
         this.WaitingOnTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WaitingOnTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.WaitingOnTextBox.Location = new System.Drawing.Point(100, 66);
         this.WaitingOnTextBox.Name = "WaitingOnTextBox";
         this.WaitingOnTextBox.ReadOnly = true;
         this.WaitingOnTextBox.Size = new System.Drawing.Size(124, 20);
         this.WaitingOnTextBox.TabIndex = 4;
         this.WaitingOnTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // AttemptsTextBox
         // 
         this.AttemptsTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.AttemptsTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.AttemptsTextBox.Location = new System.Drawing.Point(100, 89);
         this.AttemptsTextBox.Name = "AttemptsTextBox";
         this.AttemptsTextBox.ReadOnly = true;
         this.AttemptsTextBox.Size = new System.Drawing.Size(124, 20);
         this.AttemptsTextBox.TabIndex = 6;
         this.AttemptsTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // NextAttemptTextBox
         // 
         this.NextAttemptTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.NextAttemptTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
         this.NextAttemptTextBox.Location = new System.Drawing.Point(100, 112);
         this.NextAttemptTextBox.Name = "NextAttemptTextBox";
         this.NextAttemptTextBox.ReadOnly = true;
         this.NextAttemptTextBox.Size = new System.Drawing.Size(124, 20);
         this.NextAttemptTextBox.TabIndex = 8;
         this.NextAttemptTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // QueueControl
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.Controls.Add(this.tableLayoutPanel1);
         this.Name = "QueueControl";
         this.Size = new System.Drawing.Size(236, 530);
         this.tableLayoutPanel1.ResumeLayout(false);
         this.tableLayoutPanel1.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private LabelWrapper lblServer;
      private ComboBoxWrapper cboQueueIndex;
      private LabelWrapper lblUserID;
      private LabelWrapper lblBenchmark;
      private LabelWrapper lblBeginDate;
      private LabelWrapper lblEndDate;
      private LabelWrapper lblCpuType;
      private LabelWrapper lblOsType;
      private LabelWrapper lblSmpCores;
      private LabelWrapper lblCoresToUse;
      private LabelWrapper lblMegaFlops;
      private LabelWrapper lblMemory;
      private TextBoxWrapper txtServer;
      private TextBoxWrapper txtUserID;
      private TextBoxWrapper txtBenchmark;
      private TextBoxWrapper txtBeginDate;
      private TextBoxWrapper txtEndDate;
      private TextBoxWrapper txtCpuType;
      private TextBoxWrapper txtOsType;
      private TextBoxWrapper txtSmpCores;
      private TextBoxWrapper txtCoresToUse;
      private TextBoxWrapper txtMegaFlops;
      private TextBoxWrapper txtMemory;
      private TextBoxWrapper txtCredit;
      private LabelWrapper lblCredit;
      private TextBoxWrapper txtSpeedFactor;
      private LabelWrapper lblSpeedFactor;
      private TextBoxWrapper txtStatus;
      private LabelWrapper labelWrapper1;
      private TextBoxWrapper txtPerformanceFraction;
      private LabelWrapper lblPerformanceFraction;
      private TextBoxWrapper txtAverageUploadRate;
      private TextBoxWrapper txtAverageDownloadRate;
      private LabelWrapper lblAverageUploadRate;
      private LabelWrapper lblAverageDownloadRate;
      private TextBoxWrapper txtMachineID;
      private LabelWrapper lblMachineID;
      private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
      private LabelWrapper labelWrapper2;
      private LabelWrapper labelWrapper3;
      private LabelWrapper labelWrapper4;
      private TextBoxWrapper WaitingOnTextBox;
      private TextBoxWrapper AttemptsTextBox;
      private TextBoxWrapper NextAttemptTextBox;
   }
}
