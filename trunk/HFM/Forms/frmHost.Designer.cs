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

using harlam357.Windows.Forms;

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
         this.txtLocalPath = new harlam357.Windows.Forms.ValidatingTextBox();
         this.toolTipHost = new System.Windows.Forms.ToolTip(this.components);
         this.btnBrowseLocal = new HFM.Classes.ButtonWrapper();
         this.radioLocal = new HFM.Classes.RadioButtonWrapper();
         this.txtName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.chkClientVM = new HFM.Classes.CheckBoxWrapper();
         this.numOffset = new System.Windows.Forms.NumericUpDown();
         this.lblInstanceName = new HFM.Classes.LabelWrapper();
         this.radioFTP = new HFM.Classes.RadioButtonWrapper();
         this.lblFTPServer = new HFM.Classes.LabelWrapper();
         this.txtFTPServer = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblFTPPath = new HFM.Classes.LabelWrapper();
         this.txtFTPPath = new harlam357.Windows.Forms.ValidatingTextBox();
         this.radioHTTP = new HFM.Classes.RadioButtonWrapper();
         this.txtWebURL = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblWebURL = new HFM.Classes.LabelWrapper();
         this.txtFTPUser = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblFTPUser = new HFM.Classes.LabelWrapper();
         this.txtFTPPass = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblFTPPass = new HFM.Classes.LabelWrapper();
         this.txtWebUser = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtWebPass = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblWebUser = new HFM.Classes.LabelWrapper();
         this.lblWebPass = new HFM.Classes.LabelWrapper();
         this.grpLocal = new HFM.Classes.GroupBoxWrapper();
         this.lblLogFolder = new HFM.Classes.LabelWrapper();
         this.grpFTP = new HFM.Classes.GroupBoxWrapper();
         this.labelWrapper1 = new HFM.Classes.LabelWrapper();
         this.radioActive = new HFM.Classes.RadioButtonWrapper();
         this.radioPassive = new HFM.Classes.RadioButtonWrapper();
         this.grpHTTP = new HFM.Classes.GroupBoxWrapper();
         this.openLogFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.txtLogFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblLogFileName = new HFM.Classes.LabelWrapper();
         this.txtUnitFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblUnitFileName = new HFM.Classes.LabelWrapper();
         this.lblClientMegahertz = new HFM.Classes.LabelWrapper();
         this.txtClientMegahertz = new harlam357.Windows.Forms.ValidatingTextBox();
         this.label1 = new HFM.Classes.LabelWrapper();
         this.lblQueueFileName = new HFM.Classes.LabelWrapper();
         this.txtQueueFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
         this.grpLocal.SuspendLayout();
         this.grpFTP.SuspendLayout();
         this.grpHTTP.SuspendLayout();
         this.SuspendLayout();
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(212, 350);
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
         this.btnCancel.Location = new System.Drawing.Point(304, 350);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(81, 25);
         this.btnCancel.TabIndex = 20;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         // 
         // txtLocalPath
         // 
         this.txtLocalPath.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.txtLocalPath.BackColor = System.Drawing.SystemColors.Window;
         this.txtLocalPath.DoubleBuffered = true;
         this.txtLocalPath.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtLocalPath.Location = new System.Drawing.Point(11, 35);
         this.txtLocalPath.Name = "txtLocalPath";
         this.txtLocalPath.Size = new System.Drawing.Size(320, 20);
         this.txtLocalPath.TabIndex = 1;
         this.txtLocalPath.ErrorToolTip = this.toolTipHost;
         this.txtLocalPath.ErrorToolTipDuration = 5000;
         this.txtLocalPath.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtLocalPath.ErrorToolTipText = "Log Folder must be a valid local or network (UNC) path.";
         this.txtLocalPath.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtLocalPath.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtLocalPath_CustomValidation);
         // 
         // btnBrowseLocal
         // 
         this.btnBrowseLocal.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.btnBrowseLocal.CausesValidation = false;
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
         this.txtName.BackColor = System.Drawing.SystemColors.Window;
         this.txtName.DoubleBuffered = true;
         this.txtName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtName.Location = new System.Drawing.Point(146, 12);
         this.txtName.MaxLength = 100;
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(237, 20);
         this.txtName.TabIndex = 1;
         this.txtName.ErrorToolTip = this.toolTipHost;
         this.txtName.ErrorToolTipDuration = 5000;
         this.txtName.ErrorToolTipPoint = new System.Drawing.Point(230, -20);
         this.txtName.ErrorToolTipText = "Instance name can contain only letters, numbers,\r\nand basic symbols (+=-_$&^.[])." +
             " It must be at\r\nleast three characters long and must not begin or\r\nend with a do" +
             "t (.) or a space.";
         this.txtName.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtName.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtName_CustomValidation);
         // 
         // chkClientVM
         // 
         this.chkClientVM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.chkClientVM.AutoSize = true;
         this.chkClientVM.Location = new System.Drawing.Point(12, 326);
         this.chkClientVM.Name = "chkClientVM";
         this.chkClientVM.Size = new System.Drawing.Size(301, 17);
         this.chkClientVM.TabIndex = 16;
         this.chkClientVM.Text = "Client is on Virtual Machine (and reports UTC as local time)";
         this.chkClientVM.UseVisualStyleBackColor = true;
         // 
         // numOffset
         // 
         this.numOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.numOffset.Location = new System.Drawing.Point(8, 352);
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
         this.lblFTPServer.AutoSize = true;
         this.lblFTPServer.Location = new System.Drawing.Point(11, 18);
         this.lblFTPServer.Name = "lblFTPServer";
         this.lblFTPServer.Size = new System.Drawing.Size(116, 13);
         this.lblFTPServer.TabIndex = 0;
         this.lblFTPServer.Text = "FTP Server Name / IP:";
         // 
         // txtFTPServer
         // 
         this.txtFTPServer.BackColor = System.Drawing.SystemColors.Window;
         this.txtFTPServer.DoubleBuffered = true;
         this.txtFTPServer.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFTPServer.Location = new System.Drawing.Point(152, 15);
         this.txtFTPServer.Name = "txtFTPServer";
         this.txtFTPServer.Size = new System.Drawing.Size(215, 20);
         this.txtFTPServer.TabIndex = 1;
         this.txtFTPServer.ErrorToolTip = this.toolTipHost;
         this.txtFTPServer.ErrorToolTipDuration = 5000;
         this.txtFTPServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPServer.ErrorToolTipText = "FTP server must be a valid host name or IP address.";
         this.txtFTPServer.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtFTPServer.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFTPServer_CustomValidation);
         // 
         // lblFTPPath
         // 
         this.lblFTPPath.AutoSize = true;
         this.lblFTPPath.Location = new System.Drawing.Point(11, 45);
         this.lblFTPPath.Name = "lblFTPPath";
         this.lblFTPPath.Size = new System.Drawing.Size(104, 13);
         this.lblFTPPath.TabIndex = 2;
         this.lblFTPPath.Text = "Log Path (Directory):";
         // 
         // txtFTPPath
         // 
         this.txtFTPPath.BackColor = System.Drawing.SystemColors.Window;
         this.txtFTPPath.DoubleBuffered = true;
         this.txtFTPPath.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFTPPath.Location = new System.Drawing.Point(152, 42);
         this.txtFTPPath.Name = "txtFTPPath";
         this.txtFTPPath.Size = new System.Drawing.Size(215, 20);
         this.txtFTPPath.TabIndex = 3;
         this.txtFTPPath.ErrorToolTip = this.toolTipHost;
         this.txtFTPPath.ErrorToolTipDuration = 5000;
         this.txtFTPPath.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
         this.txtFTPPath.ErrorToolTipText = "FTP path must be the full path to the folder\r\ncontaining the log files (including" +
             " the trailing /).";
         this.txtFTPPath.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtFTPPath.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFTPPath_CustomValidation);
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
         this.txtWebURL.BackColor = System.Drawing.SystemColors.Window;
         this.txtWebURL.DoubleBuffered = true;
         this.txtWebURL.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebURL.Location = new System.Drawing.Point(152, 15);
         this.txtWebURL.Name = "txtWebURL";
         this.txtWebURL.Size = new System.Drawing.Size(215, 20);
         this.txtWebURL.TabIndex = 1;
         this.txtWebURL.ErrorToolTip = this.toolTipHost;
         this.txtWebURL.ErrorToolTipDuration = 5000;
         this.txtWebURL.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebURL.ErrorToolTipText = "URL must be a the full path to the location containing the log files.";
         this.txtWebURL.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtWebURL.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtWebURL_CustomValidation);
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
         this.txtFTPUser.BackColor = System.Drawing.SystemColors.Window;
         this.txtFTPUser.DoubleBuffered = true;
         this.txtFTPUser.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFTPUser.Location = new System.Drawing.Point(152, 69);
         this.txtFTPUser.Name = "txtFTPUser";
         this.txtFTPUser.Size = new System.Drawing.Size(215, 20);
         this.txtFTPUser.TabIndex = 5;
         this.txtFTPUser.ErrorToolTip = this.toolTipHost;
         this.txtFTPUser.ErrorToolTipDuration = 5000;
         this.txtFTPUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPUser.ErrorToolTipText = "";
         this.txtFTPUser.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtFTPUser.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFtpCredentials_CustomValidation);
         // 
         // lblFTPUser
         // 
         this.lblFTPUser.AutoSize = true;
         this.lblFTPUser.Location = new System.Drawing.Point(11, 72);
         this.lblFTPUser.Name = "lblFTPUser";
         this.lblFTPUser.Size = new System.Drawing.Size(81, 13);
         this.lblFTPUser.TabIndex = 4;
         this.lblFTPUser.Text = "FTP Username:";
         // 
         // txtFTPPass
         // 
         this.txtFTPPass.BackColor = System.Drawing.SystemColors.Window;
         this.txtFTPPass.DoubleBuffered = true;
         this.txtFTPPass.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFTPPass.Location = new System.Drawing.Point(152, 96);
         this.txtFTPPass.Name = "txtFTPPass";
         this.txtFTPPass.Size = new System.Drawing.Size(215, 20);
         this.txtFTPPass.TabIndex = 7;
         this.txtFTPPass.ErrorToolTip = this.toolTipHost;
         this.txtFTPPass.ErrorToolTipDuration = 5000;
         this.txtFTPPass.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPPass.ErrorToolTipText = "";
         this.txtFTPPass.UseSystemPasswordChar = true;
         this.txtFTPPass.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtFTPPass.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFtpCredentials_CustomValidation);
         // 
         // lblFTPPass
         // 
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
         this.txtWebUser.BackColor = System.Drawing.SystemColors.Window;
         this.txtWebUser.DoubleBuffered = true;
         this.txtWebUser.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebUser.Location = new System.Drawing.Point(152, 42);
         this.txtWebUser.Name = "txtWebUser";
         this.txtWebUser.Size = new System.Drawing.Size(215, 20);
         this.txtWebUser.TabIndex = 3;
         this.txtWebUser.ErrorToolTip = this.toolTipHost;
         this.txtWebUser.ErrorToolTipDuration = 5000;
         this.txtWebUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebUser.ErrorToolTipText = "";
         this.txtWebUser.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtWebUser.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtHttpCredentials_CustomValidation);
         // 
         // txtWebPass
         // 
         this.txtWebPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebPass.BackColor = System.Drawing.SystemColors.Window;
         this.txtWebPass.DoubleBuffered = true;
         this.txtWebPass.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebPass.Location = new System.Drawing.Point(152, 69);
         this.txtWebPass.Name = "txtWebPass";
         this.txtWebPass.PasswordChar = '#';
         this.txtWebPass.Size = new System.Drawing.Size(215, 20);
         this.txtWebPass.TabIndex = 5;
         this.txtWebPass.ErrorToolTip = this.toolTipHost;
         this.txtWebPass.ErrorToolTipDuration = 5000;
         this.txtWebPass.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebPass.ErrorToolTipText = "";
         this.txtWebPass.UseSystemPasswordChar = true;
         this.txtWebPass.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtWebPass.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtHttpCredentials_CustomValidation);
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
         this.grpFTP.Controls.Add(this.labelWrapper1);
         this.grpFTP.Controls.Add(this.radioActive);
         this.grpFTP.Controls.Add(this.radioPassive);
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
         this.grpFTP.Size = new System.Drawing.Size(378, 152);
         this.grpFTP.TabIndex = 15;
         this.grpFTP.TabStop = false;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.AutoSize = true;
         this.labelWrapper1.Location = new System.Drawing.Point(11, 126);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(60, 13);
         this.labelWrapper1.TabIndex = 10;
         this.labelWrapper1.Text = "FTP Mode:";
         // 
         // radioActive
         // 
         this.radioActive.AutoSize = true;
         this.radioActive.Location = new System.Drawing.Point(220, 124);
         this.radioActive.Name = "radioActive";
         this.radioActive.Size = new System.Drawing.Size(55, 17);
         this.radioActive.TabIndex = 9;
         this.radioActive.Text = "Active";
         this.radioActive.UseVisualStyleBackColor = true;
         // 
         // radioPassive
         // 
         this.radioPassive.AutoSize = true;
         this.radioPassive.Checked = true;
         this.radioPassive.Location = new System.Drawing.Point(152, 124);
         this.radioPassive.Name = "radioPassive";
         this.radioPassive.Size = new System.Drawing.Size(62, 17);
         this.radioPassive.TabIndex = 8;
         this.radioPassive.TabStop = true;
         this.radioPassive.Text = "Passive";
         this.radioPassive.UseVisualStyleBackColor = true;
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
         // openLogFolder
         // 
         this.openLogFolder.ShowNewFolderButton = false;
         // 
         // txtLogFileName
         // 
         this.txtLogFileName.DoubleBuffered = true;
         this.txtLogFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtLogFileName.Location = new System.Drawing.Point(146, 64);
         this.txtLogFileName.MaxLength = 100;
         this.txtLogFileName.Name = "txtLogFileName";
         this.txtLogFileName.Size = new System.Drawing.Size(237, 20);
         this.txtLogFileName.TabIndex = 5;
         this.txtLogFileName.ErrorToolTip = this.toolTipHost;
         this.txtLogFileName.ErrorToolTipDuration = 5000;
         this.txtLogFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtLogFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtLogFileName.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtLogFileName.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFileName_CustomValidation);
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
         this.txtUnitFileName.DoubleBuffered = true;
         this.txtUnitFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtUnitFileName.Location = new System.Drawing.Point(146, 90);
         this.txtUnitFileName.MaxLength = 100;
         this.txtUnitFileName.Name = "txtUnitFileName";
         this.txtUnitFileName.Size = new System.Drawing.Size(237, 20);
         this.txtUnitFileName.TabIndex = 7;
         this.txtUnitFileName.ErrorToolTip = this.toolTipHost;
         this.txtUnitFileName.ErrorToolTipDuration = 5000;
         this.txtUnitFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtUnitFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtUnitFileName.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtUnitFileName.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFileName_CustomValidation);
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
         this.txtClientMegahertz.DoubleBuffered = true;
         this.txtClientMegahertz.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtClientMegahertz.Location = new System.Drawing.Point(146, 38);
         this.txtClientMegahertz.MaxLength = 9;
         this.txtClientMegahertz.Name = "txtClientMegahertz";
         this.txtClientMegahertz.Size = new System.Drawing.Size(237, 20);
         this.txtClientMegahertz.TabIndex = 3;
         this.txtClientMegahertz.Text = "1";
         this.txtClientMegahertz.ErrorToolTip = this.toolTipHost;
         this.txtClientMegahertz.ErrorToolTipDuration = 5000;
         this.txtClientMegahertz.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtClientMegahertz.ErrorToolTipText = "Client Processor Megahertz must be numeric and greater than zero.";
         this.txtClientMegahertz.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtClientMegahertz.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtClientMegahertz_CustomValidation);
         // 
         // label1
         // 
         this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(64, 355);
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
         this.txtQueueFileName.DoubleBuffered = true;
         this.txtQueueFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtQueueFileName.Location = new System.Drawing.Point(146, 116);
         this.txtQueueFileName.MaxLength = 100;
         this.txtQueueFileName.Name = "txtQueueFileName";
         this.txtQueueFileName.Size = new System.Drawing.Size(237, 20);
         this.txtQueueFileName.TabIndex = 9;
         this.txtQueueFileName.ErrorToolTip = this.toolTipHost;
         this.txtQueueFileName.ErrorToolTipDuration = 5000;
         this.txtQueueFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtQueueFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtQueueFileName.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         this.txtQueueFileName.CustomValidation += new System.EventHandler<harlam357.Windows.Forms.ValidatingControlCustomValidationEventArgs>(this.txtFileName_CustomValidation);
         // 
         // frmHost
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 385);
         this.Controls.Add(this.txtQueueFileName);
         this.Controls.Add(this.lblQueueFileName);
         this.Controls.Add(this.grpLocal);
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
         this.Controls.Add(this.grpHTTP);
         this.Controls.Add(this.grpFTP);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmHost";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Folding Instance Setup";
         ((System.ComponentModel.ISupportInitialize)(this.numOffset)).EndInit();
         this.grpLocal.ResumeLayout(false);
         this.grpLocal.PerformLayout();
         this.grpFTP.ResumeLayout(false);
         this.grpFTP.PerformLayout();
         this.grpHTTP.ResumeLayout(false);
         this.grpHTTP.PerformLayout();
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
      private ValidatingTextBox txtLocalPath;
      private ValidatingTextBox txtName;
      private ValidatingTextBox txtFTPServer;
      private ValidatingTextBox txtFTPPath;
      private ValidatingTextBox txtWebURL;
      private ValidatingTextBox txtFTPUser;
      private ValidatingTextBox txtFTPPass;
      private ValidatingTextBox txtWebUser;
      private ValidatingTextBox txtWebPass;
      private Classes.GroupBoxWrapper grpLocal;
      private Classes.LabelWrapper lblLogFolder;
      private Classes.GroupBoxWrapper grpFTP;
      private Classes.GroupBoxWrapper grpHTTP;

      #endregion
      private System.Windows.Forms.FolderBrowserDialog openLogFolder;
      private ValidatingTextBox txtLogFileName;
      private Classes.LabelWrapper lblLogFileName;
      private ValidatingTextBox txtUnitFileName;
      private Classes.LabelWrapper lblUnitFileName;
      private Classes.CheckBoxWrapper chkClientVM;
      private Classes.LabelWrapper lblClientMegahertz;
      private ValidatingTextBox txtClientMegahertz;
      private Classes.LabelWrapper label1;
      private System.Windows.Forms.NumericUpDown numOffset;
      private Classes.RadioButtonWrapper radioLocal;
      private Classes.RadioButtonWrapper radioFTP;
      private Classes.RadioButtonWrapper radioHTTP;
      private Classes.LabelWrapper lblQueueFileName;
      private ValidatingTextBox txtQueueFileName;
      private Classes.RadioButtonWrapper radioPassive;
      private Classes.RadioButtonWrapper radioActive;
      private Classes.LabelWrapper labelWrapper1;
      private System.Windows.Forms.ToolTip toolTipHost;

   }
}
