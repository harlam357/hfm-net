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
            this.cboQueueIndex = new System.Windows.Forms.ComboBox();
            this.txtMachineID = new System.Windows.Forms.TextBox();
            this.labelWrapper1 = new System.Windows.Forms.Label();
            this.lblMachineID = new System.Windows.Forms.Label();
            this.txtStatus = new System.Windows.Forms.TextBox();
            this.txtSmpCores = new System.Windows.Forms.TextBox();
            this.lblCredit = new System.Windows.Forms.Label();
            this.lblSmpCores = new System.Windows.Forms.Label();
            this.txtMemory = new System.Windows.Forms.TextBox();
            this.txtCredit = new System.Windows.Forms.TextBox();
            this.txtOsType = new System.Windows.Forms.TextBox();
            this.lblBeginDate = new System.Windows.Forms.Label();
            this.lblMemory = new System.Windows.Forms.Label();
            this.txtCpuType = new System.Windows.Forms.TextBox();
            this.txtBeginDate = new System.Windows.Forms.TextBox();
            this.lblOsType = new System.Windows.Forms.Label();
            this.lblCpuType = new System.Windows.Forms.Label();
            this.lblServer = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.labelWrapper2 = new System.Windows.Forms.Label();
            this.labelWrapper3 = new System.Windows.Forms.Label();
            this.labelWrapper4 = new System.Windows.Forms.Label();
            this.WaitingOnTextBox = new System.Windows.Forms.TextBox();
            this.AttemptsTextBox = new System.Windows.Forms.TextBox();
            this.NextAttemptTextBox = new System.Windows.Forms.TextBox();
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
            this.tableLayoutPanel1.Controls.Add(this.txtMachineID, 1, 13);
            this.tableLayoutPanel1.Controls.Add(this.labelWrapper1, 0, 2);
            this.tableLayoutPanel1.Controls.Add(this.lblMachineID, 0, 13);
            this.tableLayoutPanel1.Controls.Add(this.txtStatus, 1, 2);
            this.tableLayoutPanel1.Controls.Add(this.txtSmpCores, 1, 12);
            this.tableLayoutPanel1.Controls.Add(this.lblCredit, 0, 6);
            this.tableLayoutPanel1.Controls.Add(this.lblSmpCores, 0, 12);
            this.tableLayoutPanel1.Controls.Add(this.txtMemory, 1, 11);
            this.tableLayoutPanel1.Controls.Add(this.txtCredit, 1, 6);
            this.tableLayoutPanel1.Controls.Add(this.txtOsType, 1, 10);
            this.tableLayoutPanel1.Controls.Add(this.lblBeginDate, 0, 7);
            this.tableLayoutPanel1.Controls.Add(this.lblMemory, 0, 11);
            this.tableLayoutPanel1.Controls.Add(this.txtCpuType, 1, 9);
            this.tableLayoutPanel1.Controls.Add(this.txtBeginDate, 1, 7);
            this.tableLayoutPanel1.Controls.Add(this.lblOsType, 0, 10);
            this.tableLayoutPanel1.Controls.Add(this.lblCpuType, 0, 9);
            this.tableLayoutPanel1.Controls.Add(this.lblServer, 0, 8);
            this.tableLayoutPanel1.Controls.Add(this.txtServer, 1, 8);
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
            this.tableLayoutPanel1.RowCount = 15;
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
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 5F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(236, 330);
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
            this.txtMachineID.Location = new System.Drawing.Point(100, 296);
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
            this.lblMachineID.Location = new System.Drawing.Point(10, 298);
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
            // txtSmpCores
            // 
            this.txtSmpCores.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSmpCores.Location = new System.Drawing.Point(100, 273);
            this.txtSmpCores.Name = "txtSmpCores";
            this.txtSmpCores.ReadOnly = true;
            this.txtSmpCores.Size = new System.Drawing.Size(124, 20);
            this.txtSmpCores.TabIndex = 36;
            this.txtSmpCores.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // lblSmpCores
            // 
            this.lblSmpCores.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblSmpCores.AutoSize = true;
            this.lblSmpCores.Location = new System.Drawing.Point(10, 275);
            this.lblSmpCores.Name = "lblSmpCores";
            this.lblSmpCores.Size = new System.Drawing.Size(63, 13);
            this.lblSmpCores.TabIndex = 35;
            this.lblSmpCores.Text = "SMP Cores:";
            // 
            // txtMemory
            // 
            this.txtMemory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtMemory.Location = new System.Drawing.Point(100, 250);
            this.txtMemory.Name = "txtMemory";
            this.txtMemory.ReadOnly = true;
            this.txtMemory.Size = new System.Drawing.Size(124, 20);
            this.txtMemory.TabIndex = 32;
            this.txtMemory.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            this.txtOsType.Location = new System.Drawing.Point(100, 227);
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
            this.lblMemory.Location = new System.Drawing.Point(10, 252);
            this.lblMemory.Name = "lblMemory";
            this.lblMemory.Size = new System.Drawing.Size(47, 13);
            this.lblMemory.TabIndex = 31;
            this.lblMemory.Text = "Memory:";
            // 
            // txtCpuType
            // 
            this.txtCpuType.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtCpuType.Location = new System.Drawing.Point(100, 204);
            this.txtCpuType.Name = "txtCpuType";
            this.txtCpuType.ReadOnly = true;
            this.txtCpuType.Size = new System.Drawing.Size(124, 20);
            this.txtCpuType.TabIndex = 28;
            this.txtCpuType.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            // lblOsType
            // 
            this.lblOsType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblOsType.AutoSize = true;
            this.lblOsType.Location = new System.Drawing.Point(10, 229);
            this.lblOsType.Name = "lblOsType";
            this.lblOsType.Size = new System.Drawing.Size(25, 13);
            this.lblOsType.TabIndex = 29;
            this.lblOsType.Text = "OS:";
            // 
            // lblCpuType
            // 
            this.lblCpuType.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblCpuType.AutoSize = true;
            this.lblCpuType.Location = new System.Drawing.Point(10, 206);
            this.lblCpuType.Name = "lblCpuType";
            this.lblCpuType.Size = new System.Drawing.Size(59, 13);
            this.lblCpuType.TabIndex = 27;
            this.lblCpuType.Text = "CPU Type:";
            // 
            // lblServer
            // 
            this.lblServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblServer.AutoSize = true;
            this.lblServer.Location = new System.Drawing.Point(10, 183);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(41, 13);
            this.lblServer.TabIndex = 21;
            this.lblServer.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtServer.Location = new System.Drawing.Point(100, 181);
            this.txtServer.Name = "txtServer";
            this.txtServer.ReadOnly = true;
            this.txtServer.Size = new System.Drawing.Size(124, 20);
            this.txtServer.TabIndex = 22;
            this.txtServer.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
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
            this.Size = new System.Drawing.Size(236, 330);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.ComboBox cboQueueIndex;
        private System.Windows.Forms.Label lblBeginDate;
        private System.Windows.Forms.Label lblCpuType;
        private System.Windows.Forms.Label lblOsType;
        private System.Windows.Forms.Label lblSmpCores;
        private System.Windows.Forms.Label lblMemory;
        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.TextBox txtBeginDate;
        private System.Windows.Forms.TextBox txtCpuType;
        private System.Windows.Forms.TextBox txtOsType;
        private System.Windows.Forms.TextBox txtSmpCores;
        private System.Windows.Forms.TextBox txtMemory;
        private System.Windows.Forms.TextBox txtCredit;
        private System.Windows.Forms.Label lblCredit;
        private System.Windows.Forms.TextBox txtStatus;
        private System.Windows.Forms.Label labelWrapper1;
        private System.Windows.Forms.TextBox txtMachineID;
        private System.Windows.Forms.Label lblMachineID;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label labelWrapper2;
        private System.Windows.Forms.Label labelWrapper3;
        private System.Windows.Forms.Label labelWrapper4;
        private System.Windows.Forms.TextBox WaitingOnTextBox;
        private System.Windows.Forms.TextBox AttemptsTextBox;
        private System.Windows.Forms.TextBox NextAttemptTextBox;
    }
}
