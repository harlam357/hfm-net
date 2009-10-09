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
         this.btnOK = new HFM.Classes.ButtonWrapper();
         this.btnCancel = new HFM.Classes.ButtonWrapper();
         this.txtLocalPath = new HFM.Classes.TextBoxWrapper();
         this.btnBrowseLocal = new HFM.Classes.ButtonWrapper();
         this.radioLocal = new HFM.Classes.RadioButtonWrapper();
         this.txtName = new HFM.Classes.TextBoxWrapper();
         this.lblInstanceName = new HFM.Classes.LabelWrapper();
         this.radioFTP = new HFM.Classes.RadioButtonWrapper();
         this.lblFTPServer = new HFM.Classes.LabelWrapper();
         this.txtFTPServer = new HFM.Classes.TextBoxWrapper();
         this.lblFTPPath = new HFM.Classes.LabelWrapper();
         this.txtFTPPath = new HFM.Classes.TextBoxWrapper();
         this.radioHTTP = new HFM.Classes.RadioButtonWrapper();
         this.txtWebURL = new HFM.Classes.TextBoxWrapper();
         this.lblWebURL = new HFM.Classes.LabelWrapper();
         this.txtFTPUser = new HFM.Classes.TextBoxWrapper();
         this.lblFTPUser = new HFM.Classes.LabelWrapper();
         this.txtFTPPass = new HFM.Classes.TextBoxWrapper();
         this.lblFTPPass = new HFM.Classes.LabelWrapper();
         this.txtWebUser = new HFM.Classes.TextBoxWrapper();
         this.txtWebPass = new HFM.Classes.TextBoxWrapper();
         this.lblWebUser = new HFM.Classes.LabelWrapper();
         this.lblWebPass = new HFM.Classes.LabelWrapper();
         this.grpLocal = new HFM.Classes.GroupBoxWrapper();
         this.lblLogFolder = new HFM.Classes.LabelWrapper();
         this.grpFTP = new HFM.Classes.GroupBoxWrapper();
         this.grpHTTP = new HFM.Classes.GroupBoxWrapper();
         this.toolTipCore = new System.Windows.Forms.ToolTip(this.components);
         this.chkClientVM = new HFM.Classes.CheckBoxWrapper();
         this.numOffset = new System.Windows.Forms.NumericUpDown();
         this.openLogFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.txtLogFileName = new HFM.Classes.TextBoxWrapper();
         this.lblLogFileName = new HFM.Classes.LabelWrapper();
         this.txtUnitFileName = new HFM.Classes.TextBoxWrapper();
         this.lblUnitFileName = new HFM.Classes.LabelWrapper();
         this.lblClientMegahertz = new HFM.Classes.LabelWrapper();
         this.txtClientMegahertz = new HFM.Classes.TextBoxWrapper();
         this.label1 = new HFM.Classes.LabelWrapper();
         this.lblQueueFileName = new HFM.Classes.LabelWrapper();
         this.txtQueueFileName = new HFM.Classes.TextBoxWrapper();
         this.grpLocal.SuspendLayout();
         this.grpFTP.SuspendLayout();
         this.grpHTTP.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(212, 325);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(81, 25);
         this.btnOK.TabIndex = 19;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.CausesValidation = false;
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(304, 325);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(81, 25);
         this.btnCancel.TabIndex = 20;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // txtLocalPath
         // 
         this.txtLocalPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.txtLocalPath.Location = new System.Drawing.Point(11, 35);
         this.txtLocalPath.Name = "txtLocalPath";
         this.txtLocalPath.ReadOnly = true;
         this.txtLocalPath.Size = new System.Drawing.Size(320, 20);
         this.txtLocalPath.TabIndex = 1;
         this.txtLocalPath.Validating += new System.ComponentModel.CancelEventHandler(this.txtLocalPath_Validating);
         // 
         // btnBrowseLocal
         // 
         this.btnBrowseLocal.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.btnBrowseLocal.CausesValidation = false;
         this.btnBrowseLocal.Enabled = false;
         this.btnBrowseLocal.Location = new System.Drawing.Point(337, 33);
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
         this.radioLocal.Location = new System.Drawing.Point(10, 146);
         this.radioLocal.Name = "radioLocal";
         this.radioLocal.Size = new System.Drawing.Size(131, 17);
         this.radioLocal.TabIndex = 10;
         this.radioLocal.Text = "Local or Network Path";
         this.radioLocal.UseVisualStyleBackColor = true;
         this.radioLocal.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
         // 
         // txtName
         // 
         this.txtName.Location = new System.Drawing.Point(146, 12);
         this.txtName.MaxLength = 100;
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(237, 20);
         this.txtName.TabIndex = 1;
         this.txtName.Validating += new System.ComponentModel.CancelEventHandler(this.txtName_Validating);
         // 
         // lblInstanceName
         // 
         this.lblInstanceName.AutoSize = true;
         this.lblInstanceName.Location = new System.Drawing.Point(6, 15);
         this.lblInstanceName.Name = "lblInstanceName";
         this.lblInstanceName.Size = new System.Drawing.Size(82, 13);
         this.lblInstanceName.TabIndex = 0;
         this.lblInstanceName.Text = "Instance Name:";
         // 
         // radioFTP
         // 
         this.radioFTP.AutoSize = true;
         this.radioFTP.CausesValidation = false;
         this.radioFTP.Location = new System.Drawing.Point(256, 146);
         this.radioFTP.Name = "radioFTP";
         this.radioFTP.Size = new System.Drawing.Size(79, 17);
         this.radioFTP.TabIndex = 12;
         this.radioFTP.Text = "FTP Server";
         this.radioFTP.UseVisualStyleBackColor = true;
         this.radioFTP.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
         // 
         // lblFTPServer
         // 
         this.lblFTPServer.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblFTPServer.AutoSize = true;
         this.lblFTPServer.Location = new System.Drawing.Point(11, 18);
         this.lblFTPServer.Name = "lblFTPServer";
         this.lblFTPServer.Size = new System.Drawing.Size(116, 13);
         this.lblFTPServer.TabIndex = 0;
         this.lblFTPServer.Text = "FTP Server Name / IP:";
         // 
         // txtFTPServer
         // 
         this.txtFTPServer.Location = new System.Drawing.Point(152, 15);
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
         this.lblFTPPath.Location = new System.Drawing.Point(11, 45);
         this.lblFTPPath.Name = "lblFTPPath";
         this.lblFTPPath.Size = new System.Drawing.Size(104, 13);
         this.lblFTPPath.TabIndex = 2;
         this.lblFTPPath.Text = "Log Path (Directory):";
         // 
         // txtFTPPath
         // 
         this.txtFTPPath.Location = new System.Drawing.Point(152, 42);
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
         this.radioHTTP.Location = new System.Drawing.Point(159, 146);
         this.radioHTTP.Name = "radioHTTP";
         this.radioHTTP.Size = new System.Drawing.Size(82, 17);
         this.radioHTTP.TabIndex = 11;
         this.radioHTTP.Text = "Web Server";
         this.radioHTTP.UseVisualStyleBackColor = true;
         this.radioHTTP.CheckedChanged += new System.EventHandler(this.radioButtonSet_CheckedChanged);
         // 
         // txtWebURL
         // 
         this.txtWebURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebURL.Location = new System.Drawing.Point(152, 15);
         this.txtWebURL.Name = "txtWebURL";
         this.txtWebURL.ReadOnly = true;
         this.txtWebURL.Size = new System.Drawing.Size(215, 20);
         this.txtWebURL.TabIndex = 1;
         this.txtWebURL.Validating += new System.ComponentModel.CancelEventHandler(this.txtWebURL_Validating);
         // 
         // lblWebURL
         // 
         this.lblWebURL.AutoSize = true;
         this.lblWebURL.Location = new System.Drawing.Point(11, 18);
         this.lblWebURL.Name = "lblWebURL";
         this.lblWebURL.Size = new System.Drawing.Size(110, 13);
         this.lblWebURL.TabIndex = 0;
         this.lblWebURL.Text = "URL to Log Directory:";
         // 
         // txtFTPUser
         // 
         this.txtFTPUser.Location = new System.Drawing.Point(152, 69);
         this.txtFTPUser.Name = "txtFTPUser";
         this.txtFTPUser.ReadOnly = true;
         this.txtFTPUser.Size = new System.Drawing.Size(215, 20);
         this.txtFTPUser.TabIndex = 5;
         // 
         // lblFTPUser
         // 
         this.lblFTPUser.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblFTPUser.AutoSize = true;
         this.lblFTPUser.Location = new System.Drawing.Point(11, 72);
         this.lblFTPUser.Name = "lblFTPUser";
         this.lblFTPUser.Size = new System.Drawing.Size(81, 13);
         this.lblFTPUser.TabIndex = 4;
         this.lblFTPUser.Text = "FTP Username:";
         // 
         // txtFTPPass
         // 
         this.txtFTPPass.Location = new System.Drawing.Point(152, 96);
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
         this.lblFTPPass.Location = new System.Drawing.Point(11, 99);
         this.lblFTPPass.Name = "lblFTPPass";
         this.lblFTPPass.Size = new System.Drawing.Size(79, 13);
         this.lblFTPPass.TabIndex = 6;
         this.lblFTPPass.Text = "FTP Password:";
         // 
         // txtWebUser
         // 
         this.txtWebUser.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebUser.Location = new System.Drawing.Point(152, 42);
         this.txtWebUser.Name = "txtWebUser";
         this.txtWebUser.ReadOnly = true;
         this.txtWebUser.Size = new System.Drawing.Size(215, 20);
         this.txtWebUser.TabIndex = 3;
         // 
         // txtWebPass
         // 
         this.txtWebPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebPass.Location = new System.Drawing.Point(152, 69);
         this.txtWebPass.Name = "txtWebPass";
         this.txtWebPass.PasswordChar = '#';
         this.txtWebPass.ReadOnly = true;
         this.txtWebPass.Size = new System.Drawing.Size(215, 20);
         this.txtWebPass.TabIndex = 5;
         this.txtWebPass.UseSystemPasswordChar = true;
         // 
         // lblWebUser
         // 
         this.lblWebUser.AutoSize = true;
         this.lblWebUser.Location = new System.Drawing.Point(11, 45);
         this.lblWebUser.Name = "lblWebUser";
         this.lblWebUser.Size = new System.Drawing.Size(118, 13);
         this.lblWebUser.TabIndex = 2;
         this.lblWebUser.Text = "Web Server Username:";
         // 
         // lblWebPass
         // 
         this.lblWebPass.AutoSize = true;
         this.lblWebPass.Location = new System.Drawing.Point(11, 72);
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
         this.grpLocal.Location = new System.Drawing.Point(7, 166);
         this.grpLocal.Name = "grpLocal";
         this.grpLocal.Size = new System.Drawing.Size(378, 69);
         this.grpLocal.TabIndex = 13;
         this.grpLocal.TabStop = false;
         // 
         // lblLogFolder
         // 
         this.lblLogFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.lblLogFolder.AutoSize = true;
         this.lblLogFolder.Location = new System.Drawing.Point(11, 16);
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
         this.grpFTP.Location = new System.Drawing.Point(7, 166);
         this.grpFTP.Name = "grpFTP";
         this.grpFTP.Size = new System.Drawing.Size(378, 126);
         this.grpFTP.TabIndex = 15;
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
         this.grpHTTP.Location = new System.Drawing.Point(7, 166);
         this.grpHTTP.Name = "grpHTTP";
         this.grpHTTP.Size = new System.Drawing.Size(378, 99);
         this.grpHTTP.TabIndex = 14;
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
         this.chkClientVM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkClientVM.AutoSize = true;
         this.chkClientVM.Location = new System.Drawing.Point(12, 301);
         this.chkClientVM.Name = "chkClientVM";
         this.chkClientVM.Size = new System.Drawing.Size(301, 17);
         this.chkClientVM.TabIndex = 16;
         this.chkClientVM.Text = "Client is on Virtual Machine (and reports UTC as local time)";
         this.toolTipCore.SetToolTip(this.chkClientVM, resources.GetString("chkClientVM.ToolTip"));
         this.chkClientVM.UseVisualStyleBackColor = true;
         // 
         // numOffset
         // 
         this.numOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.numOffset.Location = new System.Drawing.Point(8, 327);
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
         this.numOffset.TabIndex = 17;
         this.toolTipCore.SetToolTip(this.numOffset, resources.GetString("numOffset.ToolTip"));
         // 
         // openLogFolder
         // 
         this.openLogFolder.ShowNewFolderButton = false;
         // 
         // txtLogFileName
         // 
         this.txtLogFileName.Location = new System.Drawing.Point(146, 64);
         this.txtLogFileName.MaxLength = 100;
         this.txtLogFileName.Name = "txtLogFileName";
         this.txtLogFileName.Size = new System.Drawing.Size(237, 20);
         this.txtLogFileName.TabIndex = 5;
         this.txtLogFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtLogFileName_Validating);
         // 
         // lblLogFileName
         // 
         this.lblLogFileName.AutoSize = true;
         this.lblLogFileName.Location = new System.Drawing.Point(6, 67);
         this.lblLogFileName.Name = "lblLogFileName";
         this.lblLogFileName.Size = new System.Drawing.Size(116, 13);
         this.lblLogFileName.TabIndex = 4;
         this.lblLogFileName.Text = "Filename for FAHlog.txt";
         // 
         // txtUnitFileName
         // 
         this.txtUnitFileName.Location = new System.Drawing.Point(146, 90);
         this.txtUnitFileName.MaxLength = 100;
         this.txtUnitFileName.Name = "txtUnitFileName";
         this.txtUnitFileName.Size = new System.Drawing.Size(237, 20);
         this.txtUnitFileName.TabIndex = 7;
         this.txtUnitFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtUnitFileName_Validating);
         // 
         // lblUnitFileName
         // 
         this.lblUnitFileName.AutoSize = true;
         this.lblUnitFileName.Location = new System.Drawing.Point(6, 93);
         this.lblUnitFileName.Name = "lblUnitFileName";
         this.lblUnitFileName.Size = new System.Drawing.Size(115, 13);
         this.lblUnitFileName.TabIndex = 6;
         this.lblUnitFileName.Text = "Filename for unitinfo.txt";
         // 
         // lblClientMegahertz
         // 
         this.lblClientMegahertz.AutoSize = true;
         this.lblClientMegahertz.Location = new System.Drawing.Point(6, 41);
         this.lblClientMegahertz.Name = "lblClientMegahertz";
         this.lblClientMegahertz.Size = new System.Drawing.Size(111, 13);
         this.lblClientMegahertz.TabIndex = 2;
         this.lblClientMegahertz.Text = "Client Processor MHz:";
         // 
         // txtClientMegahertz
         // 
         this.txtClientMegahertz.Location = new System.Drawing.Point(146, 38);
         this.txtClientMegahertz.MaxLength = 9;
         this.txtClientMegahertz.Name = "txtClientMegahertz";
         this.txtClientMegahertz.Size = new System.Drawing.Size(237, 20);
         this.txtClientMegahertz.TabIndex = 3;
         this.txtClientMegahertz.Text = "1";
         this.txtClientMegahertz.Validating += new System.ComponentModel.CancelEventHandler(this.txtClientMegahertz_Validating);
         // 
         // label1
         // 
         this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(64, 330);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(136, 13);
         this.label1.TabIndex = 18;
         this.label1.Text = "Client Time Offset (Minutes)";
         // 
         // lblQueueFileName
         // 
         this.lblQueueFileName.AutoSize = true;
         this.lblQueueFileName.Location = new System.Drawing.Point(6, 119);
         this.lblQueueFileName.Name = "lblQueueFileName";
         this.lblQueueFileName.Size = new System.Drawing.Size(115, 13);
         this.lblQueueFileName.TabIndex = 8;
         this.lblQueueFileName.Text = "Filename for queue.dat";
         // 
         // txtQueueFileName
         // 
         this.txtQueueFileName.Location = new System.Drawing.Point(146, 116);
         this.txtQueueFileName.MaxLength = 100;
         this.txtQueueFileName.Name = "txtQueueFileName";
         this.txtQueueFileName.Size = new System.Drawing.Size(237, 20);
         this.txtQueueFileName.TabIndex = 9;
         this.txtQueueFileName.Validating += new System.ComponentModel.CancelEventHandler(this.txtQueueFileName_Validating);
         // 
         // frmHost
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 360);
         this.Controls.Add(this.txtQueueFileName);
         this.Controls.Add(this.lblQueueFileName);
         this.Controls.Add(this.grpLocal);
         this.Controls.Add(this.grpHTTP);
         this.Controls.Add(this.grpFTP);
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
         this.Controls.Add(this.lblUnitFileName);
         this.Controls.Add(this.txtLogFileName);
         this.Controls.Add(this.txtUnitFileName);
         this.Controls.Add(this.radioHTTP);
         this.Controls.Add(this.radioFTP);
         this.Controls.Add(this.radioLocal);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmHost";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Folding Instance Setup";
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

      private Classes.ButtonWrapper btnOK;
      private Classes.ButtonWrapper btnCancel;
      private Classes.ButtonWrapper btnBrowseLocal;
      private Classes.LabelWrapper lblInstanceName;
      private Classes.LabelWrapper lblFTPServer;
      private Classes.LabelWrapper lblFTPPath;
      private Classes.LabelWrapper lblWebURL;
      private Classes.LabelWrapper lblFTPUser;
      private Classes.LabelWrapper lblFTPPass;
      private Classes.LabelWrapper lblWebUser;
      private Classes.LabelWrapper lblWebPass;
      private Classes.TextBoxWrapper txtLocalPath;
      private Classes.TextBoxWrapper txtName;
      private Classes.TextBoxWrapper txtFTPServer;
      private Classes.TextBoxWrapper txtFTPPath;
      private Classes.TextBoxWrapper txtWebURL;
      private Classes.TextBoxWrapper txtFTPUser;
      private Classes.TextBoxWrapper txtFTPPass;
      private Classes.TextBoxWrapper txtWebUser;
      private Classes.TextBoxWrapper txtWebPass;
      private Classes.GroupBoxWrapper grpLocal;
      private Classes.LabelWrapper lblLogFolder;
      private Classes.GroupBoxWrapper grpFTP;
      private Classes.GroupBoxWrapper grpHTTP;
      private System.Windows.Forms.ToolTip toolTipCore;

      #endregion
      private System.Windows.Forms.FolderBrowserDialog openLogFolder;
      private Classes.TextBoxWrapper txtLogFileName;
      private Classes.LabelWrapper lblLogFileName;
      private Classes.TextBoxWrapper txtUnitFileName;
      private Classes.LabelWrapper lblUnitFileName;
      private Classes.CheckBoxWrapper chkClientVM;
      private Classes.LabelWrapper lblClientMegahertz;
      private Classes.TextBoxWrapper txtClientMegahertz;
      private Classes.LabelWrapper label1;
      private System.Windows.Forms.NumericUpDown numOffset;
      private Classes.RadioButtonWrapper radioLocal;
      private Classes.RadioButtonWrapper radioFTP;
      private Classes.RadioButtonWrapper radioHTTP;
      private Classes.LabelWrapper lblQueueFileName;
      private Classes.TextBoxWrapper txtQueueFileName;

   }
}
