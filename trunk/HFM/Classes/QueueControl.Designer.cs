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

namespace HFM.Classes
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
         this.txtPerformanceFraction = new HFM.Classes.TextBoxWrapper();
         this.lblPerformanceFraction = new HFM.Classes.LabelWrapper();
         this.txtStatus = new HFM.Classes.TextBoxWrapper();
         this.labelWrapper1 = new HFM.Classes.LabelWrapper();
         this.txtGpuMemory = new HFM.Classes.TextBoxWrapper();
         this.lblGpuMemory = new HFM.Classes.LabelWrapper();
         this.txtSpeedFactor = new HFM.Classes.TextBoxWrapper();
         this.lblSpeedFactor = new HFM.Classes.LabelWrapper();
         this.txtCredit = new HFM.Classes.TextBoxWrapper();
         this.lblCredit = new HFM.Classes.LabelWrapper();
         this.txtMemory = new HFM.Classes.TextBoxWrapper();
         this.txtMegaFlops = new HFM.Classes.TextBoxWrapper();
         this.txtCoresToUse = new HFM.Classes.TextBoxWrapper();
         this.txtSmpCores = new HFM.Classes.TextBoxWrapper();
         this.txtOsType = new HFM.Classes.TextBoxWrapper();
         this.txtCpuType = new HFM.Classes.TextBoxWrapper();
         this.txtEndDate = new HFM.Classes.TextBoxWrapper();
         this.txtBeginDate = new HFM.Classes.TextBoxWrapper();
         this.txtBenchmark = new HFM.Classes.TextBoxWrapper();
         this.txtUserID = new HFM.Classes.TextBoxWrapper();
         this.txtServer = new HFM.Classes.TextBoxWrapper();
         this.lblMemory = new HFM.Classes.LabelWrapper();
         this.lblMegaFlops = new HFM.Classes.LabelWrapper();
         this.lblCoresToUse = new HFM.Classes.LabelWrapper();
         this.lblSmpCores = new HFM.Classes.LabelWrapper();
         this.lblOsType = new HFM.Classes.LabelWrapper();
         this.lblCpuType = new HFM.Classes.LabelWrapper();
         this.lblEndDate = new HFM.Classes.LabelWrapper();
         this.lblBeginDate = new HFM.Classes.LabelWrapper();
         this.lblBenchmark = new HFM.Classes.LabelWrapper();
         this.lblUserID = new HFM.Classes.LabelWrapper();
         this.cboQueueIndex = new HFM.Classes.ComboBoxWrapper();
         this.lblServer = new HFM.Classes.LabelWrapper();
         this.txtAverageUploadRate = new HFM.Classes.TextBoxWrapper();
         this.txtAverageDownloadRate = new HFM.Classes.TextBoxWrapper();
         this.lblAverageUploadRate = new HFM.Classes.LabelWrapper();
         this.lblAverageDownloadRate = new HFM.Classes.LabelWrapper();
         this.txtMachineID = new HFM.Classes.TextBoxWrapper();
         this.lblMachineID = new HFM.Classes.LabelWrapper();
         this.SuspendLayout();
         // 
         // txtPerformanceFraction
         // 
         this.txtPerformanceFraction.Location = new System.Drawing.Point(113, 157);
         this.txtPerformanceFraction.Name = "txtPerformanceFraction";
         this.txtPerformanceFraction.ReadOnly = true;
         this.txtPerformanceFraction.Size = new System.Drawing.Size(132, 20);
         this.txtPerformanceFraction.TabIndex = 23;
         this.txtPerformanceFraction.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblPerformanceFraction
         // 
         this.lblPerformanceFraction.AutoSize = true;
         this.lblPerformanceFraction.Location = new System.Drawing.Point(7, 160);
         this.lblPerformanceFraction.Name = "lblPerformanceFraction";
         this.lblPerformanceFraction.Size = new System.Drawing.Size(73, 13);
         this.lblPerformanceFraction.TabIndex = 6;
         this.lblPerformanceFraction.Text = "Perf. Fraction:";
         // 
         // txtStatus
         // 
         this.txtStatus.Location = new System.Drawing.Point(113, 42);
         this.txtStatus.Name = "txtStatus";
         this.txtStatus.ReadOnly = true;
         this.txtStatus.Size = new System.Drawing.Size(132, 20);
         this.txtStatus.TabIndex = 18;
         this.txtStatus.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.AutoSize = true;
         this.labelWrapper1.Location = new System.Drawing.Point(7, 45);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(40, 13);
         this.labelWrapper1.TabIndex = 1;
         this.labelWrapper1.Text = "Status:";
         // 
         // txtGpuMemory
         // 
         this.txtGpuMemory.Location = new System.Drawing.Point(113, 341);
         this.txtGpuMemory.Name = "txtGpuMemory";
         this.txtGpuMemory.ReadOnly = true;
         this.txtGpuMemory.Size = new System.Drawing.Size(132, 20);
         this.txtGpuMemory.TabIndex = 31;
         this.txtGpuMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblGpuMemory
         // 
         this.lblGpuMemory.AutoSize = true;
         this.lblGpuMemory.Location = new System.Drawing.Point(7, 344);
         this.lblGpuMemory.Name = "lblGpuMemory";
         this.lblGpuMemory.Size = new System.Drawing.Size(73, 13);
         this.lblGpuMemory.TabIndex = 14;
         this.lblGpuMemory.Text = "GPU Memory:";
         // 
         // txtSpeedFactor
         // 
         this.txtSpeedFactor.Location = new System.Drawing.Point(113, 134);
         this.txtSpeedFactor.Name = "txtSpeedFactor";
         this.txtSpeedFactor.ReadOnly = true;
         this.txtSpeedFactor.Size = new System.Drawing.Size(132, 20);
         this.txtSpeedFactor.TabIndex = 22;
         this.txtSpeedFactor.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblSpeedFactor
         // 
         this.lblSpeedFactor.AutoSize = true;
         this.lblSpeedFactor.Location = new System.Drawing.Point(7, 137);
         this.lblSpeedFactor.Name = "lblSpeedFactor";
         this.lblSpeedFactor.Size = new System.Drawing.Size(74, 13);
         this.lblSpeedFactor.TabIndex = 5;
         this.lblSpeedFactor.Text = "Speed Factor:";
         // 
         // txtCredit
         // 
         this.txtCredit.Location = new System.Drawing.Point(113, 65);
         this.txtCredit.Name = "txtCredit";
         this.txtCredit.ReadOnly = true;
         this.txtCredit.Size = new System.Drawing.Size(132, 20);
         this.txtCredit.TabIndex = 19;
         this.txtCredit.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblCredit
         // 
         this.lblCredit.AutoSize = true;
         this.lblCredit.Location = new System.Drawing.Point(7, 68);
         this.lblCredit.Name = "lblCredit";
         this.lblCredit.Size = new System.Drawing.Size(37, 13);
         this.lblCredit.TabIndex = 2;
         this.lblCredit.Text = "Credit:";
         // 
         // txtMemory
         // 
         this.txtMemory.Location = new System.Drawing.Point(113, 318);
         this.txtMemory.Name = "txtMemory";
         this.txtMemory.ReadOnly = true;
         this.txtMemory.Size = new System.Drawing.Size(132, 20);
         this.txtMemory.TabIndex = 30;
         this.txtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtMegaFlops
         // 
         this.txtMegaFlops.Location = new System.Drawing.Point(113, 180);
         this.txtMegaFlops.Name = "txtMegaFlops";
         this.txtMegaFlops.ReadOnly = true;
         this.txtMegaFlops.Size = new System.Drawing.Size(132, 20);
         this.txtMegaFlops.TabIndex = 24;
         this.txtMegaFlops.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCoresToUse
         // 
         this.txtCoresToUse.Location = new System.Drawing.Point(113, 364);
         this.txtCoresToUse.Name = "txtCoresToUse";
         this.txtCoresToUse.ReadOnly = true;
         this.txtCoresToUse.Size = new System.Drawing.Size(132, 20);
         this.txtCoresToUse.TabIndex = 32;
         this.txtCoresToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtSmpCores
         // 
         this.txtSmpCores.Location = new System.Drawing.Point(113, 341);
         this.txtSmpCores.Name = "txtSmpCores";
         this.txtSmpCores.ReadOnly = true;
         this.txtSmpCores.Size = new System.Drawing.Size(132, 20);
         this.txtSmpCores.TabIndex = 26;
         this.txtSmpCores.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtOsType
         // 
         this.txtOsType.Location = new System.Drawing.Point(113, 295);
         this.txtOsType.Name = "txtOsType";
         this.txtOsType.ReadOnly = true;
         this.txtOsType.Size = new System.Drawing.Size(132, 20);
         this.txtOsType.TabIndex = 29;
         this.txtOsType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtCpuType
         // 
         this.txtCpuType.Location = new System.Drawing.Point(113, 272);
         this.txtCpuType.Name = "txtCpuType";
         this.txtCpuType.ReadOnly = true;
         this.txtCpuType.Size = new System.Drawing.Size(132, 20);
         this.txtCpuType.TabIndex = 28;
         this.txtCpuType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtEndDate
         // 
         this.txtEndDate.Location = new System.Drawing.Point(113, 111);
         this.txtEndDate.Name = "txtEndDate";
         this.txtEndDate.ReadOnly = true;
         this.txtEndDate.Size = new System.Drawing.Size(132, 20);
         this.txtEndDate.TabIndex = 21;
         this.txtEndDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtBeginDate
         // 
         this.txtBeginDate.Location = new System.Drawing.Point(113, 88);
         this.txtBeginDate.Name = "txtBeginDate";
         this.txtBeginDate.ReadOnly = true;
         this.txtBeginDate.Size = new System.Drawing.Size(132, 20);
         this.txtBeginDate.TabIndex = 20;
         this.txtBeginDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtBenchmark
         // 
         this.txtBenchmark.Location = new System.Drawing.Point(113, 341);
         this.txtBenchmark.Name = "txtBenchmark";
         this.txtBenchmark.ReadOnly = true;
         this.txtBenchmark.Size = new System.Drawing.Size(132, 20);
         this.txtBenchmark.TabIndex = 25;
         this.txtBenchmark.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtUserID
         // 
         this.txtUserID.Location = new System.Drawing.Point(113, 387);
         this.txtUserID.Name = "txtUserID";
         this.txtUserID.ReadOnly = true;
         this.txtUserID.Size = new System.Drawing.Size(132, 20);
         this.txtUserID.TabIndex = 33;
         this.txtUserID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtServer
         // 
         this.txtServer.Location = new System.Drawing.Point(113, 203);
         this.txtServer.Name = "txtServer";
         this.txtServer.ReadOnly = true;
         this.txtServer.Size = new System.Drawing.Size(132, 20);
         this.txtServer.TabIndex = 25;
         this.txtServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblMemory
         // 
         this.lblMemory.AutoSize = true;
         this.lblMemory.Location = new System.Drawing.Point(7, 321);
         this.lblMemory.Name = "lblMemory";
         this.lblMemory.Size = new System.Drawing.Size(47, 13);
         this.lblMemory.TabIndex = 13;
         this.lblMemory.Text = "Memory:";
         // 
         // lblMegaFlops
         // 
         this.lblMegaFlops.AutoSize = true;
         this.lblMegaFlops.Location = new System.Drawing.Point(7, 183);
         this.lblMegaFlops.Name = "lblMegaFlops";
         this.lblMegaFlops.Size = new System.Drawing.Size(62, 13);
         this.lblMegaFlops.TabIndex = 7;
         this.lblMegaFlops.Text = "MegaFlops:";
         // 
         // lblCoresToUse
         // 
         this.lblCoresToUse.AutoSize = true;
         this.lblCoresToUse.Location = new System.Drawing.Point(7, 367);
         this.lblCoresToUse.Name = "lblCoresToUse";
         this.lblCoresToUse.Size = new System.Drawing.Size(75, 13);
         this.lblCoresToUse.TabIndex = 15;
         this.lblCoresToUse.Text = "Cores To Use:";
         // 
         // lblSmpCores
         // 
         this.lblSmpCores.AutoSize = true;
         this.lblSmpCores.Location = new System.Drawing.Point(7, 344);
         this.lblSmpCores.Name = "lblSmpCores";
         this.lblSmpCores.Size = new System.Drawing.Size(63, 13);
         this.lblSmpCores.TabIndex = 12;
         this.lblSmpCores.Text = "SMP Cores:";
         // 
         // lblOsType
         // 
         this.lblOsType.AutoSize = true;
         this.lblOsType.Location = new System.Drawing.Point(7, 298);
         this.lblOsType.Name = "lblOsType";
         this.lblOsType.Size = new System.Drawing.Size(25, 13);
         this.lblOsType.TabIndex = 12;
         this.lblOsType.Text = "OS:";
         // 
         // lblCpuType
         // 
         this.lblCpuType.AutoSize = true;
         this.lblCpuType.Location = new System.Drawing.Point(7, 275);
         this.lblCpuType.Name = "lblCpuType";
         this.lblCpuType.Size = new System.Drawing.Size(59, 13);
         this.lblCpuType.TabIndex = 11;
         this.lblCpuType.Text = "CPU Type:";
         // 
         // lblEndDate
         // 
         this.lblEndDate.AutoSize = true;
         this.lblEndDate.Location = new System.Drawing.Point(7, 114);
         this.lblEndDate.Name = "lblEndDate";
         this.lblEndDate.Size = new System.Drawing.Size(55, 13);
         this.lblEndDate.TabIndex = 4;
         this.lblEndDate.Text = "End Date:";
         // 
         // lblBeginDate
         // 
         this.lblBeginDate.AutoSize = true;
         this.lblBeginDate.Location = new System.Drawing.Point(7, 91);
         this.lblBeginDate.Name = "lblBeginDate";
         this.lblBeginDate.Size = new System.Drawing.Size(63, 13);
         this.lblBeginDate.TabIndex = 3;
         this.lblBeginDate.Text = "Begin Date:";
         // 
         // lblBenchmark
         // 
         this.lblBenchmark.AutoSize = true;
         this.lblBenchmark.Location = new System.Drawing.Point(7, 344);
         this.lblBenchmark.Name = "lblBenchmark";
         this.lblBenchmark.Size = new System.Drawing.Size(64, 13);
         this.lblBenchmark.TabIndex = 11;
         this.lblBenchmark.Text = "Benchmark:";
         // 
         // lblUserID
         // 
         this.lblUserID.AutoSize = true;
         this.lblUserID.Location = new System.Drawing.Point(7, 390);
         this.lblUserID.Name = "lblUserID";
         this.lblUserID.Size = new System.Drawing.Size(46, 13);
         this.lblUserID.TabIndex = 16;
         this.lblUserID.Text = "User ID:";
         // 
         // cboQueueIndex
         // 
         this.cboQueueIndex.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.cboQueueIndex.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboQueueIndex.FormattingEnabled = true;
         this.cboQueueIndex.Location = new System.Drawing.Point(10, 10);
         this.cboQueueIndex.Name = "cboQueueIndex";
         this.cboQueueIndex.Size = new System.Drawing.Size(236, 21);
         this.cboQueueIndex.TabIndex = 0;
         this.cboQueueIndex.SelectedIndexChanged += new System.EventHandler(this.cboQueueIndex_SelectedIndexChanged);
         // 
         // lblServer
         // 
         this.lblServer.AutoSize = true;
         this.lblServer.Location = new System.Drawing.Point(7, 206);
         this.lblServer.Name = "lblServer";
         this.lblServer.Size = new System.Drawing.Size(41, 13);
         this.lblServer.TabIndex = 8;
         this.lblServer.Text = "Server:";
         // 
         // txtAverageUploadRate
         // 
         this.txtAverageUploadRate.Location = new System.Drawing.Point(113, 249);
         this.txtAverageUploadRate.Name = "txtAverageUploadRate";
         this.txtAverageUploadRate.ReadOnly = true;
         this.txtAverageUploadRate.Size = new System.Drawing.Size(132, 20);
         this.txtAverageUploadRate.TabIndex = 27;
         this.txtAverageUploadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // txtAverageDownloadRate
         // 
         this.txtAverageDownloadRate.Location = new System.Drawing.Point(113, 226);
         this.txtAverageDownloadRate.Name = "txtAverageDownloadRate";
         this.txtAverageDownloadRate.ReadOnly = true;
         this.txtAverageDownloadRate.Size = new System.Drawing.Size(132, 20);
         this.txtAverageDownloadRate.TabIndex = 26;
         this.txtAverageDownloadRate.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblAverageUploadRate
         // 
         this.lblAverageUploadRate.AutoSize = true;
         this.lblAverageUploadRate.Location = new System.Drawing.Point(7, 252);
         this.lblAverageUploadRate.Name = "lblAverageUploadRate";
         this.lblAverageUploadRate.Size = new System.Drawing.Size(69, 13);
         this.lblAverageUploadRate.TabIndex = 10;
         this.lblAverageUploadRate.Text = "Avg. Upload:";
         // 
         // lblAverageDownloadRate
         // 
         this.lblAverageDownloadRate.AutoSize = true;
         this.lblAverageDownloadRate.Location = new System.Drawing.Point(7, 229);
         this.lblAverageDownloadRate.Name = "lblAverageDownloadRate";
         this.lblAverageDownloadRate.Size = new System.Drawing.Size(83, 13);
         this.lblAverageDownloadRate.TabIndex = 9;
         this.lblAverageDownloadRate.Text = "Avg. Download:";
         // 
         // txtMachineID
         // 
         this.txtMachineID.Location = new System.Drawing.Point(113, 410);
         this.txtMachineID.Name = "txtMachineID";
         this.txtMachineID.ReadOnly = true;
         this.txtMachineID.Size = new System.Drawing.Size(132, 20);
         this.txtMachineID.TabIndex = 34;
         this.txtMachineID.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         // 
         // lblMachineID
         // 
         this.lblMachineID.AutoSize = true;
         this.lblMachineID.Location = new System.Drawing.Point(7, 413);
         this.lblMachineID.Name = "lblMachineID";
         this.lblMachineID.Size = new System.Drawing.Size(65, 13);
         this.lblMachineID.TabIndex = 17;
         this.lblMachineID.Text = "Machine ID:";
         // 
         // QueueControl
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.BackColor = System.Drawing.SystemColors.Window;
         this.Controls.Add(this.txtMachineID);
         this.Controls.Add(this.lblMachineID);
         this.Controls.Add(this.txtAverageUploadRate);
         this.Controls.Add(this.txtAverageDownloadRate);
         this.Controls.Add(this.lblAverageUploadRate);
         this.Controls.Add(this.lblAverageDownloadRate);
         this.Controls.Add(this.txtPerformanceFraction);
         this.Controls.Add(this.lblPerformanceFraction);
         this.Controls.Add(this.txtStatus);
         this.Controls.Add(this.labelWrapper1);
         this.Controls.Add(this.txtGpuMemory);
         this.Controls.Add(this.lblGpuMemory);
         this.Controls.Add(this.txtSpeedFactor);
         this.Controls.Add(this.lblSpeedFactor);
         this.Controls.Add(this.txtCredit);
         this.Controls.Add(this.lblCredit);
         this.Controls.Add(this.txtMemory);
         this.Controls.Add(this.txtMegaFlops);
         this.Controls.Add(this.txtCoresToUse);
         this.Controls.Add(this.txtSmpCores);
         this.Controls.Add(this.txtOsType);
         this.Controls.Add(this.txtCpuType);
         this.Controls.Add(this.txtEndDate);
         this.Controls.Add(this.txtBeginDate);
         this.Controls.Add(this.txtBenchmark);
         this.Controls.Add(this.txtUserID);
         this.Controls.Add(this.txtServer);
         this.Controls.Add(this.lblMemory);
         this.Controls.Add(this.lblMegaFlops);
         this.Controls.Add(this.lblCoresToUse);
         this.Controls.Add(this.lblSmpCores);
         this.Controls.Add(this.lblOsType);
         this.Controls.Add(this.lblCpuType);
         this.Controls.Add(this.lblEndDate);
         this.Controls.Add(this.lblBeginDate);
         this.Controls.Add(this.lblBenchmark);
         this.Controls.Add(this.lblUserID);
         this.Controls.Add(this.cboQueueIndex);
         this.Controls.Add(this.lblServer);
         this.Name = "QueueControl";
         this.Size = new System.Drawing.Size(256, 447);
         this.ResumeLayout(false);
         this.PerformLayout();

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
   }
}
