/*
 * HFM.NET - Host Configuration Form
 * Copyright (C) 2006-2007 David Rawling
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA.
 */

namespace HFM.Forms
{
    partial class frmHost
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
           System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmHost));
           this.btnOK = new System.Windows.Forms.Button();
           this.btnCancel = new System.Windows.Forms.Button();
           this.txtLocalPath = new System.Windows.Forms.TextBox();
           this.btnBrowseLocal = new System.Windows.Forms.Button();
           this.radioLocal = new System.Windows.Forms.RadioButton();
           this.txtName = new System.Windows.Forms.TextBox();
           this.lblInstanceName = new System.Windows.Forms.Label();
           this.radioFTP = new System.Windows.Forms.RadioButton();
           this.lblFTPServer = new System.Windows.Forms.Label();
           this.txtFTPServer = new System.Windows.Forms.TextBox();
           this.lblFTPPath = new System.Windows.Forms.Label();
           this.txtFTPPath = new System.Windows.Forms.TextBox();
           this.radioHTTP = new System.Windows.Forms.RadioButton();
           this.txtWebURL = new System.Windows.Forms.TextBox();
           this.lblWebURL = new System.Windows.Forms.Label();
           this.txtFTPUser = new System.Windows.Forms.TextBox();
           this.lblFTPUser = new System.Windows.Forms.Label();
           this.txtFTPPass = new System.Windows.Forms.TextBox();
           this.lblFTPPass = new System.Windows.Forms.Label();
           this.txtWebUser = new System.Windows.Forms.TextBox();
           this.txtWebPass = new System.Windows.Forms.TextBox();
           this.lblWebUser = new System.Windows.Forms.Label();
           this.lblWebPass = new System.Windows.Forms.Label();
           this.grpLocal = new System.Windows.Forms.GroupBox();
           this.lblLogFolder = new System.Windows.Forms.Label();
           this.grpFTP = new System.Windows.Forms.GroupBox();
           this.grpHTTP = new System.Windows.Forms.GroupBox();
           this.toolTipCore = new System.Windows.Forms.ToolTip(this.components);
           this.chkClientVM = new System.Windows.Forms.CheckBox();
           this.numOffset = new System.Windows.Forms.NumericUpDown();
           this.openLogFolder = new System.Windows.Forms.FolderBrowserDialog();
           this.txtLogFileName = new System.Windows.Forms.TextBox();
           this.lblLogFileName = new System.Windows.Forms.Label();
           this.txtUnitFileName = new System.Windows.Forms.TextBox();
           this.lblUnitInfoName = new System.Windows.Forms.Label();
           this.lblClientMegahertz = new System.Windows.Forms.Label();
           this.txtClientMegahertz = new System.Windows.Forms.TextBox();
           this.label1 = new System.Windows.Forms.Label();
           this.grpLocal.SuspendLayout();
           this.grpFTP.SuspendLayout();
           this.grpHTTP.SuspendLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
           this.SuspendLayout();
           // 
           // btnOK
           // 
           this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.btnOK.Location = new System.Drawing.Point(214, 498);
           this.btnOK.Name = "btnOK";
           this.btnOK.Size = new System.Drawing.Size(81, 25);
           this.btnOK.TabIndex = 14;
           this.btnOK.Text = "OK";
           this.btnOK.UseVisualStyleBackColor = true;
           this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
           // 
           // btnCancel
           // 
           this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
           this.btnCancel.CausesValidation = false;
           this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
           this.btnCancel.Location = new System.Drawing.Point(306, 498);
           this.btnCancel.Name = "btnCancel";
           this.btnCancel.Size = new System.Drawing.Size(81, 25);
           this.btnCancel.TabIndex = 15;
           this.btnCancel.Text = "Cancel";
           this.btnCancel.UseVisualStyleBackColor = true;
           // 
           // txtLocalPath
           // 
           this.txtLocalPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.txtLocalPath.Location = new System.Drawing.Point(80, 19);
           this.txtLocalPath.Name = "txtLocalPath";
           this.txtLocalPath.ReadOnly = true;
           this.txtLocalPath.Size = new System.Drawing.Size(249, 20);
           this.txtLocalPath.TabIndex = 1;
           this.txtLocalPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLocalPath_Validating);
           // 
           // btnBrowseLocal
           // 
           this.btnBrowseLocal.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.btnBrowseLocal.CausesValidation = false;
           this.btnBrowseLocal.Enabled = false;
           this.btnBrowseLocal.Location = new System.Drawing.Point(335, 16);
           this.btnBrowseLocal.Name = "btnBrowseLocal";
           this.btnBrowseLocal.Size = new System.Drawing.Size(30, 24);
           this.btnBrowseLocal.TabIndex = 2;
           this.btnBrowseLocal.Text = "...";
           this.btnBrowseLocal.UseVisualStyleBackColor = true;
           this.btnBrowseLocal.Click += new System.EventHandler(this.btnBrowseLocal_Click);
           // 
           // radioLocal
           // 
           this.radioLocal.AutoSize = true;
           this.radioLocal.CausesValidation = false;
           this.radioLocal.Location = new System.Drawing.Point(9, 123);
           this.radioLocal.Name = "radioLocal";
           this.radioLocal.Size = new System.Drawing.Size(202, 17);
           this.radioLocal.TabIndex = 6;
           this.radioLocal.Text = "Use local file path or network file path";
           this.radioLocal.UseVisualStyleBackColor = true;
           this.radioLocal.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
           // 
           // txtName
           // 
           this.txtName.Location = new System.Drawing.Point(214, 12);
           this.txtName.MaxLength = 100;
           this.txtName.Name = "txtName";
           this.txtName.Size = new System.Drawing.Size(173, 20);
           this.txtName.TabIndex = 1;
           this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
           // 
           // lblInstanceName
           // 
           this.lblInstanceName.AutoSize = true;
           this.lblInstanceName.Location = new System.Drawing.Point(6, 15);
           this.lblInstanceName.Name = "lblInstanceName";
           this.lblInstanceName.Size = new System.Drawing.Size(138, 13);
           this.lblInstanceName.TabIndex = 0;
           this.lblInstanceName.Text = "Computer / Instance Name:";
           // 
           // radioFTP
           // 
           this.radioFTP.AutoSize = true;
           this.radioFTP.CausesValidation = false;
           this.radioFTP.Location = new System.Drawing.Point(9, 193);
           this.radioFTP.Name = "radioFTP";
           this.radioFTP.Size = new System.Drawing.Size(153, 17);
           this.radioFTP.TabIndex = 8;
           this.radioFTP.Text = "Download from FTP Server";
           this.radioFTP.UseVisualStyleBackColor = true;
           this.radioFTP.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
           // 
           // lblFTPServer
           // 
           this.lblFTPServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.lblFTPServer.AutoSize = true;
           this.lblFTPServer.Location = new System.Drawing.Point(5, 22);
           this.lblFTPServer.Name = "lblFTPServer";
           this.lblFTPServer.Size = new System.Drawing.Size(116, 13);
           this.lblFTPServer.TabIndex = 0;
           this.lblFTPServer.Text = "FTP Server Name / IP:";
           // 
           // txtFTPServer
           // 
           this.txtFTPServer.Location = new System.Drawing.Point(154, 19);
           this.txtFTPServer.Name = "txtFTPServer";
           this.txtFTPServer.ReadOnly = true;
           this.txtFTPServer.Size = new System.Drawing.Size(215, 20);
           this.txtFTPServer.TabIndex = 1;
           this.txtFTPServer.Validating += new System.ComponentModel.CancelEventHandler(this.txtFTPServer_Validating);
           // 
           // lblFTPPath
           // 
           this.lblFTPPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.lblFTPPath.AutoSize = true;
           this.lblFTPPath.Location = new System.Drawing.Point(5, 49);
           this.lblFTPPath.Name = "lblFTPPath";
           this.lblFTPPath.Size = new System.Drawing.Size(104, 13);
           this.lblFTPPath.TabIndex = 2;
           this.lblFTPPath.Text = "Log Path (Directory):";
           // 
           // txtFTPPath
           // 
           this.txtFTPPath.Location = new System.Drawing.Point(154, 46);
           this.txtFTPPath.Name = "txtFTPPath";
           this.txtFTPPath.ReadOnly = true;
           this.txtFTPPath.Size = new System.Drawing.Size(215, 20);
           this.txtFTPPath.TabIndex = 3;
           this.txtFTPPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtFTPPath_Validating);
           // 
           // radioHTTP
           // 
           this.radioHTTP.AutoSize = true;
           this.radioHTTP.CausesValidation = false;
           this.radioHTTP.Location = new System.Drawing.Point(9, 341);
           this.radioHTTP.Name = "radioHTTP";
           this.radioHTTP.Size = new System.Drawing.Size(156, 17);
           this.radioHTTP.TabIndex = 10;
           this.radioHTTP.Text = "Download from Web Server";
           this.radioHTTP.UseVisualStyleBackColor = true;
           this.radioHTTP.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
           // 
           // txtWebURL
           // 
           this.txtWebURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.txtWebURL.Location = new System.Drawing.Point(151, 19);
           this.txtWebURL.Name = "txtWebURL";
           this.txtWebURL.ReadOnly = true;
           this.txtWebURL.Size = new System.Drawing.Size(218, 20);
           this.txtWebURL.TabIndex = 1;
           this.txtWebURL.Validating += new System.ComponentModel.CancelEventHandler(this.txtWebURL_Validating);
           // 
           // lblWebURL
           // 
           this.lblWebURL.AutoSize = true;
           this.lblWebURL.Location = new System.Drawing.Point(5, 22);
           this.lblWebURL.Name = "lblWebURL";
           this.lblWebURL.Size = new System.Drawing.Size(110, 13);
           this.lblWebURL.TabIndex = 0;
           this.lblWebURL.Text = "URL to Log Directory:";
           // 
           // txtFTPUser
           // 
           this.txtFTPUser.Location = new System.Drawing.Point(154, 73);
           this.txtFTPUser.Name = "txtFTPUser";
           this.txtFTPUser.ReadOnly = true;
           this.txtFTPUser.Size = new System.Drawing.Size(215, 20);
           this.txtFTPUser.TabIndex = 5;
           // 
           // lblFTPUser
           // 
           this.lblFTPUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.lblFTPUser.AutoSize = true;
           this.lblFTPUser.Location = new System.Drawing.Point(5, 76);
           this.lblFTPUser.Name = "lblFTPUser";
           this.lblFTPUser.Size = new System.Drawing.Size(81, 13);
           this.lblFTPUser.TabIndex = 4;
           this.lblFTPUser.Text = "FTP Username:";
           // 
           // txtFTPPass
           // 
           this.txtFTPPass.Location = new System.Drawing.Point(154, 100);
           this.txtFTPPass.Name = "txtFTPPass";
           this.txtFTPPass.ReadOnly = true;
           this.txtFTPPass.Size = new System.Drawing.Size(215, 20);
           this.txtFTPPass.TabIndex = 7;
           this.txtFTPPass.UseSystemPasswordChar = true;
           // 
           // lblFTPPass
           // 
           this.lblFTPPass.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.lblFTPPass.AutoSize = true;
           this.lblFTPPass.Location = new System.Drawing.Point(5, 103);
           this.lblFTPPass.Name = "lblFTPPass";
           this.lblFTPPass.Size = new System.Drawing.Size(79, 13);
           this.lblFTPPass.TabIndex = 6;
           this.lblFTPPass.Text = "FTP Password:";
           // 
           // txtWebUser
           // 
           this.txtWebUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.txtWebUser.Location = new System.Drawing.Point(151, 45);
           this.txtWebUser.Name = "txtWebUser";
           this.txtWebUser.ReadOnly = true;
           this.txtWebUser.Size = new System.Drawing.Size(218, 20);
           this.txtWebUser.TabIndex = 3;
           // 
           // txtWebPass
           // 
           this.txtWebPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                       | System.Windows.Forms.AnchorStyles.Right)));
           this.txtWebPass.Location = new System.Drawing.Point(151, 71);
           this.txtWebPass.Name = "txtWebPass";
           this.txtWebPass.PasswordChar = '#';
           this.txtWebPass.ReadOnly = true;
           this.txtWebPass.Size = new System.Drawing.Size(218, 20);
           this.txtWebPass.TabIndex = 5;
           this.txtWebPass.UseSystemPasswordChar = true;
           // 
           // lblWebUser
           // 
           this.lblWebUser.AutoSize = true;
           this.lblWebUser.Location = new System.Drawing.Point(5, 48);
           this.lblWebUser.Name = "lblWebUser";
           this.lblWebUser.Size = new System.Drawing.Size(118, 13);
           this.lblWebUser.TabIndex = 2;
           this.lblWebUser.Text = "Web Server Username:";
           // 
           // lblWebPass
           // 
           this.lblWebPass.AutoSize = true;
           this.lblWebPass.Location = new System.Drawing.Point(5, 74);
           this.lblWebPass.Name = "lblWebPass";
           this.lblWebPass.Size = new System.Drawing.Size(116, 13);
           this.lblWebPass.TabIndex = 4;
           this.lblWebPass.Text = "Web Server Password:";
           // 
           // grpLocal
           // 
           this.grpLocal.Controls.Add(this.btnBrowseLocal);
           this.grpLocal.Controls.Add(this.txtLocalPath);
           this.grpLocal.Controls.Add(this.lblLogFolder);
           this.grpLocal.Location = new System.Drawing.Point(9, 138);
           this.grpLocal.Name = "grpLocal";
           this.grpLocal.Size = new System.Drawing.Size(378, 51);
           this.grpLocal.TabIndex = 7;
           this.grpLocal.TabStop = false;
           // 
           // lblLogFolder
           // 
           this.lblLogFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
           this.lblLogFolder.AutoSize = true;
           this.lblLogFolder.Location = new System.Drawing.Point(5, 22);
           this.lblLogFolder.Name = "lblLogFolder";
           this.lblLogFolder.Size = new System.Drawing.Size(60, 13);
           this.lblLogFolder.TabIndex = 0;
           this.lblLogFolder.Text = "Log Folder:";
           // 
           // grpFTP
           // 
           this.grpFTP.Controls.Add(this.lblFTPServer);
           this.grpFTP.Controls.Add(this.lblFTPPath);
           this.grpFTP.Controls.Add(this.txtFTPPass);
           this.grpFTP.Controls.Add(this.lblFTPUser);
           this.grpFTP.Controls.Add(this.txtFTPServer);
           this.grpFTP.Controls.Add(this.lblFTPPass);
           this.grpFTP.Controls.Add(this.txtFTPPath);
           this.grpFTP.Controls.Add(this.txtFTPUser);
           this.grpFTP.Location = new System.Drawing.Point(9, 208);
           this.grpFTP.Name = "grpFTP";
           this.grpFTP.Size = new System.Drawing.Size(378, 129);
           this.grpFTP.TabIndex = 9;
           this.grpFTP.TabStop = false;
           // 
           // grpHTTP
           // 
           this.grpHTTP.Controls.Add(this.txtWebPass);
           this.grpHTTP.Controls.Add(this.txtWebURL);
           this.grpHTTP.Controls.Add(this.txtWebUser);
           this.grpHTTP.Controls.Add(this.lblWebPass);
           this.grpHTTP.Controls.Add(this.lblWebUser);
           this.grpHTTP.Controls.Add(this.lblWebURL);
           this.grpHTTP.Location = new System.Drawing.Point(9, 356);
           this.grpHTTP.Name = "grpHTTP";
           this.grpHTTP.Size = new System.Drawing.Size(378, 110);
           this.grpHTTP.TabIndex = 11;
           this.grpHTTP.TabStop = false;
           // 
           // toolTipCore
           // 
           this.toolTipCore.AutoPopDelay = 8000;
           this.toolTipCore.InitialDelay = 500;
           this.toolTipCore.IsBalloon = true;
           this.toolTipCore.ReshowDelay = 100;
           this.toolTipCore.ToolTipTitle = "Quick Help";
           // 
           // chkClientVM
           // 
           this.chkClientVM.AutoSize = true;
           this.chkClientVM.Location = new System.Drawing.Point(9, 472);
           this.chkClientVM.Name = "chkClientVM";
           this.chkClientVM.Size = new System.Drawing.Size(301, 17);
           this.chkClientVM.TabIndex = 11;
           this.chkClientVM.Text = "Client is on Virtual Machine (and reports UTC as local time)";
           this.toolTipCore.SetToolTip(this.chkClientVM, resources.GetString("chkClientVM.ToolTip"));
           this.chkClientVM.UseVisualStyleBackColor = true;
           // 
           // numOffset
           // 
           this.numOffset.Location = new System.Drawing.Point(9, 499);
           this.numOffset.Maximum = new decimal(new int[] {
            720,
            0,
            0,
            0});
           this.numOffset.Minimum = new decimal(new int[] {
            720,
            0,
            0,
            -2147483648});
           this.numOffset.Name = "numOffset";
           this.numOffset.Size = new System.Drawing.Size(54, 20);
           this.numOffset.TabIndex = 12;
           this.toolTipCore.SetToolTip(this.numOffset, resources.GetString("numOffset.ToolTip"));
           // 
           // openLogFolder
           // 
           this.openLogFolder.ShowNewFolderButton = false;
           // 
           // txtLogFileName
           // 
           this.txtLogFileName.Location = new System.Drawing.Point(214, 64);
           this.txtLogFileName.MaxLength = 100;
           this.txtLogFileName.Name = "txtLogFileName";
           this.txtLogFileName.Size = new System.Drawing.Size(173, 20);
           this.txtLogFileName.TabIndex = 3;
           this.txtLogFileName.Text = "FAHLog.txt";
           this.txtLogFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
           // 
           // lblLogFileName
           // 
           this.lblLogFileName.AutoSize = true;
           this.lblLogFileName.Location = new System.Drawing.Point(6, 67);
           this.lblLogFileName.Name = "lblLogFileName";
           this.lblLogFileName.Size = new System.Drawing.Size(181, 13);
           this.lblLogFileName.TabIndex = 2;
           this.lblLogFileName.Text = "Filename for FAH Log (\"FAHLog.txt\")";
           // 
           // txtUnitFileName
           // 
           this.txtUnitFileName.Location = new System.Drawing.Point(214, 90);
           this.txtUnitFileName.MaxLength = 100;
           this.txtUnitFileName.Name = "txtUnitFileName";
           this.txtUnitFileName.Size = new System.Drawing.Size(173, 20);
           this.txtUnitFileName.TabIndex = 4;
           this.txtUnitFileName.Text = "UnitInfo.txt";
           this.txtUnitFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
           // 
           // lblUnitInfoName
           // 
           this.lblUnitInfoName.AutoSize = true;
           this.lblUnitInfoName.Location = new System.Drawing.Point(6, 93);
           this.lblUnitInfoName.Name = "lblUnitInfoName";
           this.lblUnitInfoName.Size = new System.Drawing.Size(177, 13);
           this.lblUnitInfoName.TabIndex = 4;
           this.lblUnitInfoName.Text = "Filename for Unit Info (\"UnitInfo.txt\")";
           // 
           // lblClientMegahertz
           // 
           this.lblClientMegahertz.AutoSize = true;
           this.lblClientMegahertz.Location = new System.Drawing.Point(6, 41);
           this.lblClientMegahertz.Name = "lblClientMegahertz";
           this.lblClientMegahertz.Size = new System.Drawing.Size(111, 13);
           this.lblClientMegahertz.TabIndex = 15;
           this.lblClientMegahertz.Text = "Client Processor MHz:";
           // 
           // txtClientMegahertz
           // 
           this.txtClientMegahertz.Location = new System.Drawing.Point(214, 38);
           this.txtClientMegahertz.MaxLength = 100;
           this.txtClientMegahertz.Name = "txtClientMegahertz";
           this.txtClientMegahertz.Size = new System.Drawing.Size(173, 20);
           this.txtClientMegahertz.TabIndex = 2;
           this.txtClientMegahertz.Text = "1";
           this.txtClientMegahertz.Validating += new System.ComponentModel.CancelEventHandler(this.txtClientMegahertz_Validating);
           // 
           // label1
           // 
           this.label1.AutoSize = true;
           this.label1.Location = new System.Drawing.Point(65, 502);
           this.label1.Name = "label1";
           this.label1.Size = new System.Drawing.Size(136, 13);
           this.label1.TabIndex = 13;
           this.label1.Text = "Client Time Offset (Minutes)";
           // 
           // frmHost
           // 
           this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
           this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
           this.ClientSize = new System.Drawing.Size(394, 533);
           this.Controls.Add(this.label1);
           this.Controls.Add(this.numOffset);
           this.Controls.Add(this.txtClientMegahertz);
           this.Controls.Add(this.lblClientMegahertz);
           this.Controls.Add(this.chkClientVM);
           this.Controls.Add(this.lblInstanceName);
           this.Controls.Add(this.btnCancel);
           this.Controls.Add(this.txtName);
           this.Controls.Add(this.lblLogFileName);
           this.Controls.Add(this.btnOK);
           this.Controls.Add(this.lblUnitInfoName);
           this.Controls.Add(this.txtLogFileName);
           this.Controls.Add(this.grpHTTP);
           this.Controls.Add(this.txtUnitFileName);
           this.Controls.Add(this.radioHTTP);
           this.Controls.Add(this.grpFTP);
           this.Controls.Add(this.radioFTP);
           this.Controls.Add(this.radioLocal);
           this.Controls.Add(this.grpLocal);
           this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
           this.MaximizeBox = false;
           this.MinimizeBox = false;
           this.Name = "frmHost";
           this.ShowIcon = false;
           this.ShowInTaskbar = false;
           this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
           this.Text = "Add / Modify Folding Instance";
           this.grpLocal.ResumeLayout(false);
           this.grpLocal.PerformLayout();
           this.grpFTP.ResumeLayout(false);
           this.grpFTP.PerformLayout();
           this.grpHTTP.ResumeLayout(false);
           this.grpHTTP.PerformLayout();
           ((System.ComponentModel.ISupportInitialize)(this.numOffset)).EndInit();
           this.ResumeLayout(false);
           this.PerformLayout();

        }

        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnBrowseLocal;
        private System.Windows.Forms.Label lblInstanceName;
        private System.Windows.Forms.Label lblFTPServer;
        private System.Windows.Forms.Label lblFTPPath;
        private System.Windows.Forms.Label lblWebURL;
        private System.Windows.Forms.Label lblFTPUser;
        private System.Windows.Forms.Label lblFTPPass;
        private System.Windows.Forms.Label lblWebUser;
        private System.Windows.Forms.Label lblWebPass;
       public System.Windows.Forms.TextBox txtLocalPath;
       public System.Windows.Forms.TextBox txtName;
        public System.Windows.Forms.TextBox txtFTPServer;
       public System.Windows.Forms.TextBox txtFTPPath;
        public System.Windows.Forms.TextBox txtWebURL;
        public System.Windows.Forms.TextBox txtFTPUser;
        public System.Windows.Forms.TextBox txtFTPPass;
        public System.Windows.Forms.TextBox txtWebUser;
        public System.Windows.Forms.TextBox txtWebPass;
        private System.Windows.Forms.GroupBox grpLocal;
        private System.Windows.Forms.Label lblLogFolder;
        private System.Windows.Forms.GroupBox grpFTP;
        private System.Windows.Forms.GroupBox grpHTTP;
        private System.Windows.Forms.ToolTip toolTipCore;

        #endregion
        private System.Windows.Forms.FolderBrowserDialog openLogFolder;
        public System.Windows.Forms.TextBox txtLogFileName;
        private System.Windows.Forms.Label lblLogFileName;
        public System.Windows.Forms.TextBox txtUnitFileName;
       private System.Windows.Forms.Label lblUnitInfoName;
       internal System.Windows.Forms.RadioButton radioLocal;
       internal System.Windows.Forms.RadioButton radioFTP;
       internal System.Windows.Forms.RadioButton radioHTTP;
       internal System.Windows.Forms.CheckBox chkClientVM;
       private System.Windows.Forms.Label lblClientMegahertz;
       public System.Windows.Forms.TextBox txtClientMegahertz;
       private System.Windows.Forms.Label label1;
       internal System.Windows.Forms.NumericUpDown numOffset;

    }
}