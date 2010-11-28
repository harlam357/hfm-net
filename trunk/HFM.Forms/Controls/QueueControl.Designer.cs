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
         this.cboQueueIndex = new ComboBoxWrapper();
         this.txtMachineID = new TextBoxWrapper();
         this.labelWrapper1 = new LabelWrapper();
         this.lblMachineID = new LabelWrapper();
         this.txtStatus = new TextBoxWrapper();
         this.txtUserID = new TextBoxWrapper();
         this.txtCoresToUse = new TextBoxWrapper();
         this.lblUserID = new LabelWrapper();
         this.txtGpuMemory = new TextBoxWrapper();
         this.txtSmpCores = new TextBoxWrapper();
         this.lblCoresToUse = new LabelWrapper();
         this.txtAverageUploadRate = new TextBoxWrapper();
         this.lblGpuMemory = new LabelWrapper();
         this.lblCredit = new LabelWrapper();
         this.lblBenchmark = new LabelWrapper();
         this.lblSmpCores = new LabelWrapper();
         this.txtMemory = new TextBoxWrapper();
         this.lblAverageUploadRate = new LabelWrapper();
         this.txtAverageDownloadRate = new TextBoxWrapper();
         this.txtCredit = new TextBoxWrapper();
         this.txtOsType = new TextBoxWrapper();
         this.lblBeginDate = new LabelWrapper();
         this.lblMemory = new LabelWrapper();
         this.txtCpuType = new TextBoxWrapper();
         this.lblAverageDownloadRate = new LabelWrapper();
         this.txtBeginDate = new TextBoxWrapper();
         this.txtPerformanceFraction = new TextBoxWrapper();
         this.lblEndDate = new LabelWrapper();
         this.lblOsType = new LabelWrapper();
         this.lblPerformanceFraction = new LabelWrapper();
         this.txtMegaFlops = new TextBoxWrapper();
         this.txtEndDate = new TextBoxWrapper();
         this.lblCpuType = new LabelWrapper();
         this.lblSpeedFactor = new LabelWrapper();
         this.txtSpeedFactor = new TextBoxWrapper();
         this.lblMegaFlops = new LabelWrapper();
         this.lblServer = new LabelWrapper();
         this.txtServer = new TextBoxWrapper();
         this.txtBenchmark = new TextBoxWrapper();
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
         this.tableLayoutPanel1.Controls.Add(this.txtMachineID, 1, 20);
         this.tableLayoutPanel1.Controls.Add(this.labelWrapper1, 0, 2);
         this.tableLayoutPanel1.Controls.Add(this.lblMachineID, 0, 20);
         this.tableLayoutPanel1.Controls.Add(this.txtStatus, 1, 2);
         this.tableLayoutPanel1.Controls.Add(this.txtUserID, 1, 19);
         this.tableLayoutPanel1.Controls.Add(this.txtCoresToUse, 1, 18);
         this.tableLayoutPanel1.Controls.Add(this.lblUserID, 0, 19);
         this.tableLayoutPanel1.Controls.Add(this.txtGpuMemory, 1, 15);
         this.tableLayoutPanel1.Controls.Add(this.txtSmpCores, 1, 17);
         this.tableLayoutPanel1.Controls.Add(this.lblCoresToUse, 0, 18);
         this.tableLayoutPanel1.Controls.Add(this.txtAverageUploadRate, 1, 11);
         this.tableLayoutPanel1.Controls.Add(this.lblGpuMemory, 0, 15);
         this.tableLayoutPanel1.Controls.Add(this.lblCredit, 0, 3);
         this.tableLayoutPanel1.Controls.Add(this.lblBenchmark, 0, 16);
         this.tableLayoutPanel1.Controls.Add(this.lblSmpCores, 0, 17);
         this.tableLayoutPanel1.Controls.Add(this.txtMemory, 1, 14);
         this.tableLayoutPanel1.Controls.Add(this.lblAverageUploadRate, 0, 11);
         this.tableLayoutPanel1.Controls.Add(this.txtAverageDownloadRate, 1, 10);
         this.tableLayoutPanel1.Controls.Add(this.txtCredit, 1, 3);
         this.tableLayoutPanel1.Controls.Add(this.txtOsType, 1, 13);
         this.tableLayoutPanel1.Controls.Add(this.lblBeginDate, 0, 4);
         this.tableLayoutPanel1.Controls.Add(this.lblMemory, 0, 14);
         this.tableLayoutPanel1.Controls.Add(this.txtCpuType, 1, 12);
         this.tableLayoutPanel1.Controls.Add(this.lblAverageDownloadRate, 0, 10);
         this.tableLayoutPanel1.Controls.Add(this.txtBeginDate, 1, 4);
         this.tableLayoutPanel1.Controls.Add(this.txtPerformanceFraction, 1, 7);
         this.tableLayoutPanel1.Controls.Add(this.lblEndDate, 0, 5);
         this.tableLayoutPanel1.Controls.Add(this.lblOsType, 0, 13);
         this.tableLayoutPanel1.Controls.Add(this.lblPerformanceFraction, 0, 7);
         this.tableLayoutPanel1.Controls.Add(this.txtMegaFlops, 1, 8);
         this.tableLayoutPanel1.Controls.Add(this.txtEndDate, 1, 5);
         this.tableLayoutPanel1.Controls.Add(this.lblCpuType, 0, 12);
         this.tableLayoutPanel1.Controls.Add(this.lblSpeedFactor, 0, 6);
         this.tableLayoutPanel1.Controls.Add(this.txtSpeedFactor, 1, 6);
         this.tableLayoutPanel1.Controls.Add(this.lblMegaFlops, 0, 8);
         this.tableLayoutPanel1.Controls.Add(this.lblServer, 0, 9);
         this.tableLayoutPanel1.Controls.Add(this.txtServer, 1, 9);
         this.tableLayoutPanel1.Controls.Add(this.txtBenchmark, 1, 16);
         this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
         this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
         this.tableLayoutPanel1.Name = "tableLayoutPanel1";
         this.tableLayoutPanel1.Padding = new System.Windows.Forms.Padding(7, 5, 9, 5);
         this.tableLayoutPanel1.RowCount = 22;
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
         this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
         this.tableLayoutPanel1.Size = new System.Drawing.Size(236, 486);
         this.tableLayoutPanel1.TabIndex = 35;
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
         this.txtMachineID.Location = new System.Drawing.Point(100, 457);
         this.txtMachineID.Name = "txtMachineID";
         this.txtMachineID.ReadOnly = true;
         this.txtMachineID.Size = new System.Drawing.Size(124, 20);
         this.txtMachineID.TabIndex = 34;
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
         this.lblMachineID.Location = new System.Drawing.Point(10, 459);
         this.lblMachineID.Name = "lblMachineID";
         this.lblMachineID.Size = new System.Drawing.Size(65, 13);
         this.lblMachineID.TabIndex = 17;
         this.lblMachineID.Text = "Machine ID:";
         // 
         // txtStatus
         // 
         this.txtStatus.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtStatus.Location = new System.Drawing.Point(100, 43);
         this.txtStatus.Name = "txtStatus";
         this.txtStatus.ReadOnly = true;
         this.txtStatus.Size = new System.Drawing.Size(124, 20);
         this.txtStatus.TabIndex = 18;
         this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtUserID
         // 
         this.txtUserID.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtUserID.Location = new System.Drawing.Point(100, 434);
         this.txtUserID.Name = "txtUserID";
         this.txtUserID.ReadOnly = true;
         this.txtUserID.Size = new System.Drawing.Size(124, 20);
         this.txtUserID.TabIndex = 33;
         this.txtUserID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCoresToUse
         // 
         this.txtCoresToUse.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCoresToUse.Location = new System.Drawing.Point(100, 411);
         this.txtCoresToUse.Name = "txtCoresToUse";
         this.txtCoresToUse.ReadOnly = true;
         this.txtCoresToUse.Size = new System.Drawing.Size(124, 20);
         this.txtCoresToUse.TabIndex = 32;
         this.txtCoresToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblUserID
         // 
         this.lblUserID.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblUserID.AutoSize = true;
         this.lblUserID.Location = new System.Drawing.Point(10, 436);
         this.lblUserID.Name = "lblUserID";
         this.lblUserID.Size = new System.Drawing.Size(46, 13);
         this.lblUserID.TabIndex = 16;
         this.lblUserID.Text = "User ID:";
         // 
         // txtGpuMemory
         // 
         this.txtGpuMemory.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtGpuMemory.Location = new System.Drawing.Point(100, 342);
         this.txtGpuMemory.Name = "txtGpuMemory";
         this.txtGpuMemory.ReadOnly = true;
         this.txtGpuMemory.Size = new System.Drawing.Size(124, 20);
         this.txtGpuMemory.TabIndex = 31;
         this.txtGpuMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtSmpCores
         // 
         this.txtSmpCores.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtSmpCores.Location = new System.Drawing.Point(100, 388);
         this.txtSmpCores.Name = "txtSmpCores";
         this.txtSmpCores.ReadOnly = true;
         this.txtSmpCores.Size = new System.Drawing.Size(124, 20);
         this.txtSmpCores.TabIndex = 26;
         this.txtSmpCores.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCoresToUse
         // 
         this.lblCoresToUse.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCoresToUse.AutoSize = true;
         this.lblCoresToUse.Location = new System.Drawing.Point(10, 413);
         this.lblCoresToUse.Name = "lblCoresToUse";
         this.lblCoresToUse.Size = new System.Drawing.Size(75, 13);
         this.lblCoresToUse.TabIndex = 15;
         this.lblCoresToUse.Text = "Cores To Use:";
         // 
         // txtAverageUploadRate
         // 
         this.txtAverageUploadRate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtAverageUploadRate.Location = new System.Drawing.Point(100, 250);
         this.txtAverageUploadRate.Name = "txtAverageUploadRate";
         this.txtAverageUploadRate.ReadOnly = true;
         this.txtAverageUploadRate.Size = new System.Drawing.Size(124, 20);
         this.txtAverageUploadRate.TabIndex = 27;
         this.txtAverageUploadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblGpuMemory
         // 
         this.lblGpuMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblGpuMemory.AutoSize = true;
         this.lblGpuMemory.Location = new System.Drawing.Point(10, 344);
         this.lblGpuMemory.Name = "lblGpuMemory";
         this.lblGpuMemory.Size = new System.Drawing.Size(73, 13);
         this.lblGpuMemory.TabIndex = 14;
         this.lblGpuMemory.Text = "GPU Memory:";
         // 
         // lblCredit
         // 
         this.lblCredit.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCredit.AutoSize = true;
         this.lblCredit.Location = new System.Drawing.Point(10, 68);
         this.lblCredit.Name = "lblCredit";
         this.lblCredit.Size = new System.Drawing.Size(37, 13);
         this.lblCredit.TabIndex = 2;
         this.lblCredit.Text = "Credit:";
         // 
         // lblBenchmark
         // 
         this.lblBenchmark.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblBenchmark.AutoSize = true;
         this.lblBenchmark.Location = new System.Drawing.Point(10, 367);
         this.lblBenchmark.Name = "lblBenchmark";
         this.lblBenchmark.Size = new System.Drawing.Size(64, 13);
         this.lblBenchmark.TabIndex = 11;
         this.lblBenchmark.Text = "Benchmark:";
         // 
         // lblSmpCores
         // 
         this.lblSmpCores.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblSmpCores.AutoSize = true;
         this.lblSmpCores.Location = new System.Drawing.Point(10, 390);
         this.lblSmpCores.Name = "lblSmpCores";
         this.lblSmpCores.Size = new System.Drawing.Size(63, 13);
         this.lblSmpCores.TabIndex = 12;
         this.lblSmpCores.Text = "SMP Cores:";
         // 
         // txtMemory
         // 
         this.txtMemory.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMemory.Location = new System.Drawing.Point(100, 319);
         this.txtMemory.Name = "txtMemory";
         this.txtMemory.ReadOnly = true;
         this.txtMemory.Size = new System.Drawing.Size(124, 20);
         this.txtMemory.TabIndex = 30;
         this.txtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblAverageUploadRate
         // 
         this.lblAverageUploadRate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblAverageUploadRate.AutoSize = true;
         this.lblAverageUploadRate.Location = new System.Drawing.Point(10, 252);
         this.lblAverageUploadRate.Name = "lblAverageUploadRate";
         this.lblAverageUploadRate.Size = new System.Drawing.Size(69, 13);
         this.lblAverageUploadRate.TabIndex = 10;
         this.lblAverageUploadRate.Text = "Avg. Upload:";
         // 
         // txtAverageDownloadRate
         // 
         this.txtAverageDownloadRate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtAverageDownloadRate.Location = new System.Drawing.Point(100, 227);
         this.txtAverageDownloadRate.Name = "txtAverageDownloadRate";
         this.txtAverageDownloadRate.ReadOnly = true;
         this.txtAverageDownloadRate.Size = new System.Drawing.Size(124, 20);
         this.txtAverageDownloadRate.TabIndex = 26;
         this.txtAverageDownloadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCredit
         // 
         this.txtCredit.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCredit.Location = new System.Drawing.Point(100, 66);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(124, 20);
         this.txtCredit.TabIndex = 19;
         this.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtOsType
         // 
         this.txtOsType.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtOsType.Location = new System.Drawing.Point(100, 296);
         this.txtOsType.Name = "txtOsType";
         this.txtOsType.ReadOnly = true;
         this.txtOsType.Size = new System.Drawing.Size(124, 20);
         this.txtOsType.TabIndex = 29;
         this.txtOsType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblBeginDate
         // 
         this.lblBeginDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblBeginDate.AutoSize = true;
         this.lblBeginDate.Location = new System.Drawing.Point(10, 91);
         this.lblBeginDate.Name = "lblBeginDate";
         this.lblBeginDate.Size = new System.Drawing.Size(63, 13);
         this.lblBeginDate.TabIndex = 3;
         this.lblBeginDate.Text = "Begin Date:";
         // 
         // lblMemory
         // 
         this.lblMemory.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblMemory.AutoSize = true;
         this.lblMemory.Location = new System.Drawing.Point(10, 321);
         this.lblMemory.Name = "lblMemory";
         this.lblMemory.Size = new System.Drawing.Size(47, 13);
         this.lblMemory.TabIndex = 13;
         this.lblMemory.Text = "Memory:";
         // 
         // txtCpuType
         // 
         this.txtCpuType.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtCpuType.Location = new System.Drawing.Point(100, 273);
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
         this.lblAverageDownloadRate.Location = new System.Drawing.Point(10, 229);
         this.lblAverageDownloadRate.Name = "lblAverageDownloadRate";
         this.lblAverageDownloadRate.Size = new System.Drawing.Size(83, 13);
         this.lblAverageDownloadRate.TabIndex = 9;
         this.lblAverageDownloadRate.Text = "Avg. Download:";
         // 
         // txtBeginDate
         // 
         this.txtBeginDate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtBeginDate.Location = new System.Drawing.Point(100, 89);
         this.txtBeginDate.Name = "txtBeginDate";
         this.txtBeginDate.ReadOnly = true;
         this.txtBeginDate.Size = new System.Drawing.Size(124, 20);
         this.txtBeginDate.TabIndex = 20;
         this.txtBeginDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtPerformanceFraction
         // 
         this.txtPerformanceFraction.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtPerformanceFraction.Location = new System.Drawing.Point(100, 158);
         this.txtPerformanceFraction.Name = "txtPerformanceFraction";
         this.txtPerformanceFraction.ReadOnly = true;
         this.txtPerformanceFraction.Size = new System.Drawing.Size(124, 20);
         this.txtPerformanceFraction.TabIndex = 23;
         this.txtPerformanceFraction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblEndDate
         // 
         this.lblEndDate.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblEndDate.AutoSize = true;
         this.lblEndDate.Location = new System.Drawing.Point(10, 114);
         this.lblEndDate.Name = "lblEndDate";
         this.lblEndDate.Size = new System.Drawing.Size(55, 13);
         this.lblEndDate.TabIndex = 4;
         this.lblEndDate.Text = "End Date:";
         // 
         // lblOsType
         // 
         this.lblOsType.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblOsType.AutoSize = true;
         this.lblOsType.Location = new System.Drawing.Point(10, 298);
         this.lblOsType.Name = "lblOsType";
         this.lblOsType.Size = new System.Drawing.Size(25, 13);
         this.lblOsType.TabIndex = 12;
         this.lblOsType.Text = "OS:";
         // 
         // lblPerformanceFraction
         // 
         this.lblPerformanceFraction.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblPerformanceFraction.AutoSize = true;
         this.lblPerformanceFraction.Location = new System.Drawing.Point(10, 160);
         this.lblPerformanceFraction.Name = "lblPerformanceFraction";
         this.lblPerformanceFraction.Size = new System.Drawing.Size(73, 13);
         this.lblPerformanceFraction.TabIndex = 6;
         this.lblPerformanceFraction.Text = "Perf. Fraction:";
         // 
         // txtMegaFlops
         // 
         this.txtMegaFlops.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtMegaFlops.Location = new System.Drawing.Point(100, 181);
         this.txtMegaFlops.Name = "txtMegaFlops";
         this.txtMegaFlops.ReadOnly = true;
         this.txtMegaFlops.Size = new System.Drawing.Size(124, 20);
         this.txtMegaFlops.TabIndex = 24;
         this.txtMegaFlops.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtEndDate
         // 
         this.txtEndDate.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtEndDate.Location = new System.Drawing.Point(100, 112);
         this.txtEndDate.Name = "txtEndDate";
         this.txtEndDate.ReadOnly = true;
         this.txtEndDate.Size = new System.Drawing.Size(124, 20);
         this.txtEndDate.TabIndex = 21;
         this.txtEndDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCpuType
         // 
         this.lblCpuType.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblCpuType.AutoSize = true;
         this.lblCpuType.Location = new System.Drawing.Point(10, 275);
         this.lblCpuType.Name = "lblCpuType";
         this.lblCpuType.Size = new System.Drawing.Size(59, 13);
         this.lblCpuType.TabIndex = 11;
         this.lblCpuType.Text = "CPU Type:";
         // 
         // lblSpeedFactor
         // 
         this.lblSpeedFactor.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblSpeedFactor.AutoSize = true;
         this.lblSpeedFactor.Location = new System.Drawing.Point(10, 137);
         this.lblSpeedFactor.Name = "lblSpeedFactor";
         this.lblSpeedFactor.Size = new System.Drawing.Size(74, 13);
         this.lblSpeedFactor.TabIndex = 5;
         this.lblSpeedFactor.Text = "Speed Factor:";
         // 
         // txtSpeedFactor
         // 
         this.txtSpeedFactor.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtSpeedFactor.Location = new System.Drawing.Point(100, 135);
         this.txtSpeedFactor.Name = "txtSpeedFactor";
         this.txtSpeedFactor.ReadOnly = true;
         this.txtSpeedFactor.Size = new System.Drawing.Size(124, 20);
         this.txtSpeedFactor.TabIndex = 22;
         this.txtSpeedFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblMegaFlops
         // 
         this.lblMegaFlops.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblMegaFlops.AutoSize = true;
         this.lblMegaFlops.Location = new System.Drawing.Point(10, 183);
         this.lblMegaFlops.Name = "lblMegaFlops";
         this.lblMegaFlops.Size = new System.Drawing.Size(62, 13);
         this.lblMegaFlops.TabIndex = 7;
         this.lblMegaFlops.Text = "MegaFlops:";
         // 
         // lblServer
         // 
         this.lblServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblServer.AutoSize = true;
         this.lblServer.Location = new System.Drawing.Point(10, 206);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(41, 13);
         this.lblServer.TabIndex = 8;
         this.lblServer.Text = "Server:";
         // 
         // txtServer
         // 
         this.txtServer.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtServer.Location = new System.Drawing.Point(100, 204);
         this.txtServer.Name = "txtServer";
         this.txtServer.ReadOnly = true;
         this.txtServer.Size = new System.Drawing.Size(124, 20);
         this.txtServer.TabIndex = 25;
         this.txtServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtBenchmark
         // 
         this.txtBenchmark.Dock = System.Windows.Forms.DockStyle.Fill;
         this.txtBenchmark.Location = new System.Drawing.Point(100, 365);
         this.txtBenchmark.Name = "txtBenchmark";
         this.txtBenchmark.ReadOnly = true;
         this.txtBenchmark.Size = new System.Drawing.Size(124, 20);
         this.txtBenchmark.TabIndex = 25;
         this.txtBenchmark.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // QueueControl
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.Controls.Add(this.tableLayoutPanel1);
         this.Name = "QueueControl";
         this.Size = new System.Drawing.Size(236, 486);
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
      private TextBoxWrapper txtGpuMemory;
      private LabelWrapper lblGpuMemory;
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
   }
}
