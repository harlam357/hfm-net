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
         this.chkClientVM = new HFM.Classes.CheckBoxWrapper();
         this.numOffset = new System.Windows.Forms.NumericUpDown();
         this.btnBrowseLocal = new HFM.Classes.ButtonWrapper();
         this.radioLocal = new HFM.Classes.RadioButtonWrapper();
         this.txtName = new harlam357.Windows.Forms.ValidatingTextBox();
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
         this.pnlFtpMode = new harlam357.Windows.Forms.RadioPanel();
         this.radioActive = new HFM.Classes.RadioButtonWrapper();
         this.radioPassive = new HFM.Classes.RadioButtonWrapper();
         this.labelWrapper1 = new HFM.Classes.LabelWrapper();
         this.grpHTTP = new HFM.Classes.GroupBoxWrapper();
         this.openLogFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.txtLogFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblLogFileName = new HFM.Classes.LabelWrapper();
         this.txtUnitFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblUnitFileName = new HFM.Classes.LabelWrapper();
         this.lblClientMegahertz = new HFM.Classes.LabelWrapper();
         this.txtClientMegahertz = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblOffset = new HFM.Classes.LabelWrapper();
         this.lblQueueFileName = new HFM.Classes.LabelWrapper();
         this.txtQueueFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.btnTestConnection = new HFM.Classes.ButtonWrapper();
         this.pnlHostType = new harlam357.Windows.Forms.RadioPanel();
         this.txtDummy = new HFM.Classes.TextBoxWrapper();
         this.txtMergeFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         ((System.ComponentModel.ISupportInitialize)(this.numOffset)).BeginInit();
         this.grpLocal.SuspendLayout();
         this.grpFTP.SuspendLayout();
         this.pnlFtpMode.SuspendLayout();
         this.grpHTTP.SuspendLayout();
         this.pnlHostType.SuspendLayout();
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
         this.txtLocalPath.ErrorState = false;
         this.txtLocalPath.ErrorToolTip = this.toolTipHost;
         this.txtLocalPath.ErrorToolTipDuration = 5000;
         this.txtLocalPath.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtLocalPath.ErrorToolTipText = "Log Folder must be a valid local or network (UNC) path.";
         this.txtLocalPath.Location = new System.Drawing.Point(11, 35);
         this.txtLocalPath.Name = "txtLocalPath";
         this.txtLocalPath.Size = new System.Drawing.Size(320, 20);
         this.txtLocalPath.TabIndex = 1;
         this.txtLocalPath.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.toolTipHost.SetToolTip(this.chkClientVM, resources.GetString("chkClientVM.ToolTip"));
         this.chkClientVM.UseVisualStyleBackColor = true;
         // 
         // numOffset
         // 
         this.numOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.numOffset.Location = new System.Drawing.Point(8, 352);
         this.numOffset.Name = "numOffset";
         this.numOffset.Size = new System.Drawing.Size(54, 20);
         this.numOffset.TabIndex = 17;
         this.toolTipHost.SetToolTip(this.numOffset, resources.GetString("numOffset.ToolTip"));
         // 
         // btnBrowseLocal
         // 
         this.btnBrowseLocal.Anchor = System.Windows.Forms.AnchorStyles.Left;
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
         this.radioLocal.Location = new System.Drawing.Point(3, 6);
         this.radioLocal.Name = "radioLocal";
         this.radioLocal.Size = new System.Drawing.Size(76, 17);
         this.radioLocal.TabIndex = 10;
         this.radioLocal.Tag = "0";
         this.radioLocal.Text = "Local Path";
         this.radioLocal.UseVisualStyleBackColor = true;
         // 
         // txtName
         // 
         this.txtName.BackColor = System.Drawing.SystemColors.Window;
         this.txtName.DoubleBuffered = true;
         this.txtName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtName.ErrorState = false;
         this.txtName.ErrorToolTip = this.toolTipHost;
         this.txtName.ErrorToolTipDuration = 5000;
         this.txtName.ErrorToolTipPoint = new System.Drawing.Point(230, -20);
         this.txtName.ErrorToolTipText = "Instance name can contain only letters, numbers,\r\nand basic symbols (+=-_$&^.[])." +
             " It must be at\r\nleast three characters long and must not begin or\r\nend with a do" +
             "t (.) or a space.";
         this.txtName.Location = new System.Drawing.Point(146, 12);
         this.txtName.MaxLength = 100;
         this.txtName.Name = "txtName";
         this.txtName.Size = new System.Drawing.Size(237, 20);
         this.txtName.TabIndex = 1;
         this.txtName.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.radioFTP.Location = new System.Drawing.Point(174, 6);
         this.radioFTP.Name = "radioFTP";
         this.radioFTP.Size = new System.Drawing.Size(79, 17);
         this.radioFTP.TabIndex = 12;
         this.radioFTP.Tag = "1";
         this.radioFTP.Text = "FTP Server";
         this.radioFTP.UseVisualStyleBackColor = true;
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
         this.txtFTPServer.ErrorState = false;
         this.txtFTPServer.ErrorToolTip = this.toolTipHost;
         this.txtFTPServer.ErrorToolTipDuration = 5000;
         this.txtFTPServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPServer.ErrorToolTipText = "FTP server must be a valid host name or IP address.";
         this.txtFTPServer.Location = new System.Drawing.Point(152, 15);
         this.txtFTPServer.Name = "txtFTPServer";
         this.txtFTPServer.Size = new System.Drawing.Size(215, 20);
         this.txtFTPServer.TabIndex = 1;
         this.txtFTPServer.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtFTPPath.ErrorState = false;
         this.txtFTPPath.ErrorToolTip = this.toolTipHost;
         this.txtFTPPath.ErrorToolTipDuration = 5000;
         this.txtFTPPath.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
         this.txtFTPPath.ErrorToolTipText = "FTP path must be the full path to the folder\r\ncontaining the log files (including" +
             " the trailing /).";
         this.txtFTPPath.Location = new System.Drawing.Point(152, 42);
         this.txtFTPPath.Name = "txtFTPPath";
         this.txtFTPPath.Size = new System.Drawing.Size(215, 20);
         this.txtFTPPath.TabIndex = 3;
         this.txtFTPPath.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // radioHTTP
         // 
         this.radioHTTP.AutoSize = true;
         this.radioHTTP.CausesValidation = false;
         this.radioHTTP.Location = new System.Drawing.Point(85, 6);
         this.radioHTTP.Name = "radioHTTP";
         this.radioHTTP.Size = new System.Drawing.Size(82, 17);
         this.radioHTTP.TabIndex = 11;
         this.radioHTTP.Tag = "2";
         this.radioHTTP.Text = "Web Server";
         this.radioHTTP.UseVisualStyleBackColor = true;
         // 
         // txtWebURL
         // 
         this.txtWebURL.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebURL.BackColor = System.Drawing.SystemColors.Window;
         this.txtWebURL.DoubleBuffered = true;
         this.txtWebURL.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebURL.ErrorState = false;
         this.txtWebURL.ErrorToolTip = this.toolTipHost;
         this.txtWebURL.ErrorToolTipDuration = 5000;
         this.txtWebURL.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebURL.ErrorToolTipText = "URL must be a the full path to the location containing the log files.";
         this.txtWebURL.Location = new System.Drawing.Point(152, 15);
         this.txtWebURL.Name = "txtWebURL";
         this.txtWebURL.Size = new System.Drawing.Size(215, 20);
         this.txtWebURL.TabIndex = 1;
         this.txtWebURL.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtFTPUser.ErrorState = false;
         this.txtFTPUser.ErrorToolTip = this.toolTipHost;
         this.txtFTPUser.ErrorToolTipDuration = 5000;
         this.txtFTPUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPUser.ErrorToolTipText = "";
         this.txtFTPUser.Location = new System.Drawing.Point(152, 69);
         this.txtFTPUser.Name = "txtFTPUser";
         this.txtFTPUser.Size = new System.Drawing.Size(215, 20);
         this.txtFTPUser.TabIndex = 5;
         this.txtFTPUser.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtFTPPass.ErrorState = false;
         this.txtFTPPass.ErrorToolTip = this.toolTipHost;
         this.txtFTPPass.ErrorToolTipDuration = 5000;
         this.txtFTPPass.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFTPPass.ErrorToolTipText = "";
         this.txtFTPPass.Location = new System.Drawing.Point(152, 96);
         this.txtFTPPass.Name = "txtFTPPass";
         this.txtFTPPass.Size = new System.Drawing.Size(215, 20);
         this.txtFTPPass.TabIndex = 7;
         this.txtFTPPass.UseSystemPasswordChar = true;
         this.txtFTPPass.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtWebUser.ErrorState = false;
         this.txtWebUser.ErrorToolTip = this.toolTipHost;
         this.txtWebUser.ErrorToolTipDuration = 5000;
         this.txtWebUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebUser.ErrorToolTipText = "";
         this.txtWebUser.Location = new System.Drawing.Point(152, 42);
         this.txtWebUser.Name = "txtWebUser";
         this.txtWebUser.Size = new System.Drawing.Size(215, 20);
         this.txtWebUser.TabIndex = 3;
         this.txtWebUser.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // txtWebPass
         // 
         this.txtWebPass.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                     | System.Windows.Forms.AnchorStyles.Right)));
         this.txtWebPass.BackColor = System.Drawing.SystemColors.Window;
         this.txtWebPass.DoubleBuffered = true;
         this.txtWebPass.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebPass.ErrorState = false;
         this.txtWebPass.ErrorToolTip = this.toolTipHost;
         this.txtWebPass.ErrorToolTipDuration = 5000;
         this.txtWebPass.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebPass.ErrorToolTipText = "";
         this.txtWebPass.Location = new System.Drawing.Point(152, 69);
         this.txtWebPass.Name = "txtWebPass";
         this.txtWebPass.PasswordChar = '#';
         this.txtWebPass.Size = new System.Drawing.Size(215, 20);
         this.txtWebPass.TabIndex = 5;
         this.txtWebPass.UseSystemPasswordChar = true;
         this.txtWebPass.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.grpFTP.Controls.Add(this.pnlFtpMode);
         this.grpFTP.Controls.Add(this.labelWrapper1);
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
         // pnlFtpMode
         // 
         this.pnlFtpMode.Controls.Add(this.radioActive);
         this.pnlFtpMode.Controls.Add(this.radioPassive);
         this.pnlFtpMode.Location = new System.Drawing.Point(152, 118);
         this.pnlFtpMode.Name = "pnlFtpMode";
         this.pnlFtpMode.Size = new System.Drawing.Size(134, 30);
         this.pnlFtpMode.TabIndex = 11;
         this.pnlFtpMode.ValueMember = null;
         // 
         // radioActive
         // 
         this.radioActive.AutoSize = true;
         this.radioActive.Location = new System.Drawing.Point(73, 7);
         this.radioActive.Name = "radioActive";
         this.radioActive.Size = new System.Drawing.Size(55, 17);
         this.radioActive.TabIndex = 9;
         this.radioActive.Tag = "1";
         this.radioActive.Text = "Active";
         this.radioActive.UseVisualStyleBackColor = true;
         // 
         // radioPassive
         // 
         this.radioPassive.AutoSize = true;
         this.radioPassive.Checked = true;
         this.radioPassive.Location = new System.Drawing.Point(5, 7);
         this.radioPassive.Name = "radioPassive";
         this.radioPassive.Size = new System.Drawing.Size(62, 17);
         this.radioPassive.TabIndex = 8;
         this.radioPassive.TabStop = true;
         this.radioPassive.Tag = "0";
         this.radioPassive.Text = "Passive";
         this.radioPassive.UseVisualStyleBackColor = true;
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
         this.txtLogFileName.BackColor = System.Drawing.SystemColors.Window;
         this.txtLogFileName.DoubleBuffered = true;
         this.txtLogFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtLogFileName.ErrorState = false;
         this.txtLogFileName.ErrorToolTip = this.toolTipHost;
         this.txtLogFileName.ErrorToolTipDuration = 5000;
         this.txtLogFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtLogFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtLogFileName.Location = new System.Drawing.Point(146, 64);
         this.txtLogFileName.MaxLength = 100;
         this.txtLogFileName.Name = "txtLogFileName";
         this.txtLogFileName.Size = new System.Drawing.Size(237, 20);
         this.txtLogFileName.TabIndex = 5;
         this.txtLogFileName.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtUnitFileName.BackColor = System.Drawing.SystemColors.Window;
         this.txtUnitFileName.DoubleBuffered = true;
         this.txtUnitFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtUnitFileName.ErrorState = false;
         this.txtUnitFileName.ErrorToolTip = this.toolTipHost;
         this.txtUnitFileName.ErrorToolTipDuration = 5000;
         this.txtUnitFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtUnitFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtUnitFileName.Location = new System.Drawing.Point(146, 90);
         this.txtUnitFileName.MaxLength = 100;
         this.txtUnitFileName.Name = "txtUnitFileName";
         this.txtUnitFileName.Size = new System.Drawing.Size(237, 20);
         this.txtUnitFileName.TabIndex = 7;
         this.txtUnitFileName.ValidationType = harlam357.Windows.Forms.ValidationType.None;
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
         this.txtClientMegahertz.BackColor = System.Drawing.SystemColors.Window;
         this.txtClientMegahertz.DoubleBuffered = true;
         this.txtClientMegahertz.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtClientMegahertz.ErrorState = false;
         this.txtClientMegahertz.ErrorToolTip = this.toolTipHost;
         this.txtClientMegahertz.ErrorToolTipDuration = 5000;
         this.txtClientMegahertz.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtClientMegahertz.ErrorToolTipText = "Client Processor Megahertz must be numeric and greater than zero.";
         this.txtClientMegahertz.Location = new System.Drawing.Point(146, 38);
         this.txtClientMegahertz.MaxLength = 9;
         this.txtClientMegahertz.Name = "txtClientMegahertz";
         this.txtClientMegahertz.Size = new System.Drawing.Size(237, 20);
         this.txtClientMegahertz.TabIndex = 3;
         this.txtClientMegahertz.Text = "1";
         this.txtClientMegahertz.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // lblOffset
         // 
         this.lblOffset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.lblOffset.AutoSize = true;
         this.lblOffset.Location = new System.Drawing.Point(64, 355);
         this.lblOffset.Name = "lblOffset";
         this.lblOffset.Size = new System.Drawing.Size(136, 13);
         this.lblOffset.TabIndex = 18;
         this.lblOffset.Text = "Client Time Offset (Minutes)";
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
         this.txtQueueFileName.BackColor = System.Drawing.SystemColors.Window;
         this.txtQueueFileName.DoubleBuffered = true;
         this.txtQueueFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtQueueFileName.ErrorState = false;
         this.txtQueueFileName.ErrorToolTip = this.toolTipHost;
         this.txtQueueFileName.ErrorToolTipDuration = 5000;
         this.txtQueueFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtQueueFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtQueueFileName.Location = new System.Drawing.Point(146, 116);
         this.txtQueueFileName.MaxLength = 100;
         this.txtQueueFileName.Name = "txtQueueFileName";
         this.txtQueueFileName.Size = new System.Drawing.Size(237, 20);
         this.txtQueueFileName.TabIndex = 9;
         this.txtQueueFileName.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // btnTestConnection
         // 
         this.btnTestConnection.Location = new System.Drawing.Point(271, 142);
         this.btnTestConnection.Name = "btnTestConnection";
         this.btnTestConnection.Size = new System.Drawing.Size(114, 24);
         this.btnTestConnection.TabIndex = 21;
         this.btnTestConnection.Text = "Test Connection";
         this.btnTestConnection.UseVisualStyleBackColor = true;
         this.btnTestConnection.Click += new System.EventHandler(this.btnTestConnection_Click);
         // 
         // pnlHostType
         // 
         this.pnlHostType.Controls.Add(this.radioLocal);
         this.pnlHostType.Controls.Add(this.radioFTP);
         this.pnlHostType.Controls.Add(this.radioHTTP);
         this.pnlHostType.Location = new System.Drawing.Point(8, 139);
         this.pnlHostType.Name = "pnlHostType";
         this.pnlHostType.Size = new System.Drawing.Size(257, 30);
         this.pnlHostType.TabIndex = 22;
         this.pnlHostType.ValueMember = null;
         // 
         // txtDummy
         // 
         this.txtDummy.Location = new System.Drawing.Point(331, 324);
         this.txtDummy.Name = "txtDummy";
         this.txtDummy.Size = new System.Drawing.Size(49, 20);
         this.txtDummy.TabIndex = 23;
         // 
         // txtMergeFileName
         // 
         this.txtMergeFileName.BackColor = System.Drawing.SystemColors.Window;
         this.txtMergeFileName.DoubleBuffered = true;
         this.txtMergeFileName.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtMergeFileName.ErrorState = false;
         this.txtMergeFileName.ErrorToolTip = this.toolTipHost;
         this.txtMergeFileName.ErrorToolTipDuration = 5000;
         this.txtMergeFileName.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.txtMergeFileName.ErrorToolTipText = "File name contains invalid characters.";
         this.txtMergeFileName.Location = new System.Drawing.Point(146, 38);
         this.txtMergeFileName.MaxLength = 100;
         this.txtMergeFileName.Name = "txtMergeFileName";
         this.txtMergeFileName.Size = new System.Drawing.Size(237, 20);
         this.txtMergeFileName.TabIndex = 3;
         this.txtMergeFileName.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtMergeFileName.Visible = false;
         // 
         // frmHost
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 385);
         this.Controls.Add(this.txtMergeFileName);
         this.Controls.Add(this.txtDummy);
         this.Controls.Add(this.grpLocal);
         this.Controls.Add(this.pnlHostType);
         this.Controls.Add(this.btnTestConnection);
         this.Controls.Add(this.txtQueueFileName);
         this.Controls.Add(this.lblQueueFileName);
         this.Controls.Add(this.lblOffset);
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
         this.Controls.Add(this.grpFTP);
         this.Controls.Add(this.grpHTTP);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmHost";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Folding Instance Setup";
         this.Shown += new System.EventHandler(this.frmHost_Shown);
         ((System.ComponentModel.ISupportInitialize)(this.numOffset)).EndInit();
         this.grpLocal.ResumeLayout(false);
         this.grpLocal.PerformLayout();
         this.grpFTP.ResumeLayout(false);
         this.grpFTP.PerformLayout();
         this.pnlFtpMode.ResumeLayout(false);
         this.pnlFtpMode.PerformLayout();
         this.grpHTTP.ResumeLayout(false);
         this.grpHTTP.PerformLayout();
         this.pnlHostType.ResumeLayout(false);
         this.pnlHostType.PerformLayout();
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
      private Classes.LabelWrapper lblOffset;
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
      private HFM.Classes.ButtonWrapper btnTestConnection;
      private RadioPanel pnlHostType;
      private RadioPanel pnlFtpMode;
      private HFM.Classes.TextBoxWrapper txtDummy;
      private ValidatingTextBox txtMergeFileName;

   }
}
