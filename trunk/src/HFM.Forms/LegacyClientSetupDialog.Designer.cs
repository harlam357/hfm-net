/*
 * HFM.NET - Host Configuration Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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

using HFM.Forms.Controls;

namespace HFM.Forms
{
   partial class LegacyClientSetupDialog
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LegacyClientSetupDialog));
         this.DialogOkButton = new HFM.Forms.Controls.ButtonWrapper();
         this.DialogCancelButton = new HFM.Forms.Controls.ButtonWrapper();
         this.txtLocalPath = new harlam357.Windows.Forms.ValidatingTextBox();
         this.toolTipHost = new System.Windows.Forms.ToolTip(this.components);
         this.ClientNoUtcOffsetCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.ClientTimeOffsetUpDown = new System.Windows.Forms.NumericUpDown();
         this.btnBrowseLocal = new HFM.Forms.Controls.ButtonWrapper();
         this.radioLocal = new HFM.Forms.Controls.RadioButtonWrapper();
         this.ClientNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.ClientNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.radioFTP = new HFM.Forms.Controls.RadioButtonWrapper();
         this.FtpServerNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpServerNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpServerPathLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpServerPathTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.radioHTTP = new HFM.Forms.Controls.RadioButtonWrapper();
         this.txtWebURL = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblWebURL = new HFM.Forms.Controls.LabelWrapper();
         this.FtpUsernameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpUsernameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpPasswordTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpPasswordLabel = new HFM.Forms.Controls.LabelWrapper();
         this.txtWebUser = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtWebPass = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblWebUser = new HFM.Forms.Controls.LabelWrapper();
         this.lblWebPass = new HFM.Forms.Controls.LabelWrapper();
         this.grpLocal = new HFM.Forms.Controls.GroupBoxWrapper();
         this.lblLogFolder = new HFM.Forms.Controls.LabelWrapper();
         this.grpFTP = new HFM.Forms.Controls.GroupBoxWrapper();
         this.FtpServerPortTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpServerPortLabel = new HFM.Forms.Controls.LabelWrapper();
         this.pnlFtpMode = new harlam357.Windows.Forms.RadioPanel();
         this.radioActive = new HFM.Forms.Controls.RadioButtonWrapper();
         this.radioPassive = new HFM.Forms.Controls.RadioButtonWrapper();
         this.labelWrapper1 = new HFM.Forms.Controls.LabelWrapper();
         this.grpHTTP = new HFM.Forms.Controls.GroupBoxWrapper();
         this.openLogFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.txtLogFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblLogFileName = new HFM.Forms.Controls.LabelWrapper();
         this.txtUnitFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblUnitFileName = new HFM.Forms.Controls.LabelWrapper();
         this.lblClientMegahertz = new HFM.Forms.Controls.LabelWrapper();
         this.txtClientMegahertz = new harlam357.Windows.Forms.ValidatingTextBox();
         this.ClientTimeOffsetLabel = new HFM.Forms.Controls.LabelWrapper();
         this.lblQueueFileName = new HFM.Forms.Controls.LabelWrapper();
         this.txtQueueFileName = new harlam357.Windows.Forms.ValidatingTextBox();
         this.btnTestConnection = new HFM.Forms.Controls.ButtonWrapper();
         this.pnlHostType = new harlam357.Windows.Forms.RadioPanel();
         this.DummyTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         ((System.ComponentModel.ISupportInitialize)(this.ClientTimeOffsetUpDown)).BeginInit();
         this.grpLocal.SuspendLayout();
         this.grpFTP.SuspendLayout();
         this.pnlFtpMode.SuspendLayout();
         this.grpHTTP.SuspendLayout();
         this.pnlHostType.SuspendLayout();
         this.SuspendLayout();
         // 
         // DialogOkButton
         // 
         this.DialogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.DialogOkButton.Location = new System.Drawing.Point(212, 350);
         this.DialogOkButton.Name = "DialogOkButton";
         this.DialogOkButton.Size = new System.Drawing.Size(81, 25);
         this.DialogOkButton.TabIndex = 18;
         this.DialogOkButton.Text = "OK";
         this.DialogOkButton.UseVisualStyleBackColor = true;
         this.DialogOkButton.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // DialogCancelButton
         // 
         this.DialogCancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.DialogCancelButton.CausesValidation = false;
         this.DialogCancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.DialogCancelButton.Location = new System.Drawing.Point(304, 350);
         this.DialogCancelButton.Name = "DialogCancelButton";
         this.DialogCancelButton.Size = new System.Drawing.Size(81, 25);
         this.DialogCancelButton.TabIndex = 19;
         this.DialogCancelButton.Text = "Cancel";
         this.DialogCancelButton.UseVisualStyleBackColor = true;
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
         // ClientNoUtcOffsetCheckBox
         // 
         this.ClientNoUtcOffsetCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientNoUtcOffsetCheckBox.AutoSize = true;
         this.ClientNoUtcOffsetCheckBox.Location = new System.Drawing.Point(12, 326);
         this.ClientNoUtcOffsetCheckBox.Name = "ClientNoUtcOffsetCheckBox";
         this.ClientNoUtcOffsetCheckBox.Size = new System.Drawing.Size(144, 17);
         this.ClientNoUtcOffsetCheckBox.TabIndex = 15;
         this.ClientNoUtcOffsetCheckBox.Text = "Client has no UTC offset.";
         this.toolTipHost.SetToolTip(this.ClientNoUtcOffsetCheckBox, "This option only needs checked if the\r\nclient reports local time as UTC.  This\r\ni" +
        "s typically the case with VMs that cannot\r\nbe configured with a time zone offset" +
        ".");
         this.ClientNoUtcOffsetCheckBox.UseVisualStyleBackColor = true;
         // 
         // ClientTimeOffsetUpDown
         // 
         this.ClientTimeOffsetUpDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientTimeOffsetUpDown.Location = new System.Drawing.Point(8, 352);
         this.ClientTimeOffsetUpDown.Name = "ClientTimeOffsetUpDown";
         this.ClientTimeOffsetUpDown.Size = new System.Drawing.Size(54, 20);
         this.ClientTimeOffsetUpDown.TabIndex = 16;
         this.toolTipHost.SetToolTip(this.ClientTimeOffsetUpDown, resources.GetString("ClientTimeOffsetUpDown.ToolTip"));
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
         this.radioLocal.Tag = "1";
         this.radioLocal.Text = "Local Path";
         this.radioLocal.UseVisualStyleBackColor = true;
         // 
         // ClientNameTextBox
         // 
         this.ClientNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.ClientNameTextBox.DoubleBuffered = true;
         this.ClientNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.ClientNameTextBox.ErrorState = false;
         this.ClientNameTextBox.ErrorToolTip = this.toolTipHost;
         this.ClientNameTextBox.ErrorToolTipDuration = 5000;
         this.ClientNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, -20);
         this.ClientNameTextBox.ErrorToolTipText = "Instance name can contain only letters, numbers,\r\nand basic symbols (+=-_$&^.[])." +
    " It must be at\r\nleast three characters long and must not begin or\r\nend with a do" +
    "t (.) or a space.";
         this.ClientNameTextBox.Location = new System.Drawing.Point(146, 12);
         this.ClientNameTextBox.MaxLength = 100;
         this.ClientNameTextBox.Name = "ClientNameTextBox";
         this.ClientNameTextBox.Size = new System.Drawing.Size(237, 20);
         this.ClientNameTextBox.TabIndex = 1;
         this.ClientNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // ClientNameLabel
         // 
         this.ClientNameLabel.AutoSize = true;
         this.ClientNameLabel.Location = new System.Drawing.Point(6, 15);
         this.ClientNameLabel.Name = "ClientNameLabel";
         this.ClientNameLabel.Size = new System.Drawing.Size(67, 13);
         this.ClientNameLabel.TabIndex = 0;
         this.ClientNameLabel.Text = "Client Name:";
         // 
         // radioFTP
         // 
         this.radioFTP.AutoSize = true;
         this.radioFTP.CausesValidation = false;
         this.radioFTP.Location = new System.Drawing.Point(174, 6);
         this.radioFTP.Name = "radioFTP";
         this.radioFTP.Size = new System.Drawing.Size(79, 17);
         this.radioFTP.TabIndex = 12;
         this.radioFTP.Tag = "2";
         this.radioFTP.Text = "FTP Server";
         this.radioFTP.UseVisualStyleBackColor = true;
         // 
         // FtpServerNameLabel
         // 
         this.FtpServerNameLabel.AutoSize = true;
         this.FtpServerNameLabel.Location = new System.Drawing.Point(11, 18);
         this.FtpServerNameLabel.Name = "FtpServerNameLabel";
         this.FtpServerNameLabel.Size = new System.Drawing.Size(116, 13);
         this.FtpServerNameLabel.TabIndex = 0;
         this.FtpServerNameLabel.Text = "FTP Server Name / IP:";
         // 
         // FtpServerNameTextBox
         // 
         this.FtpServerNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FtpServerNameTextBox.DoubleBuffered = true;
         this.FtpServerNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.FtpServerNameTextBox.ErrorState = false;
         this.FtpServerNameTextBox.ErrorToolTip = this.toolTipHost;
         this.FtpServerNameTextBox.ErrorToolTipDuration = 5000;
         this.FtpServerNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.FtpServerNameTextBox.ErrorToolTipText = "FTP server must be a valid host name or IP address.";
         this.FtpServerNameTextBox.Location = new System.Drawing.Point(139, 15);
         this.FtpServerNameTextBox.Name = "FtpServerNameTextBox";
         this.FtpServerNameTextBox.Size = new System.Drawing.Size(228, 20);
         this.FtpServerNameTextBox.TabIndex = 1;
         this.FtpServerNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // FtpServerPathLabel
         // 
         this.FtpServerPathLabel.AutoSize = true;
         this.FtpServerPathLabel.Location = new System.Drawing.Point(11, 72);
         this.FtpServerPathLabel.Name = "FtpServerPathLabel";
         this.FtpServerPathLabel.Size = new System.Drawing.Size(104, 13);
         this.FtpServerPathLabel.TabIndex = 2;
         this.FtpServerPathLabel.Text = "Log Path (Directory):";
         // 
         // FtpServerPathTextBox
         // 
         this.FtpServerPathTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FtpServerPathTextBox.DoubleBuffered = true;
         this.FtpServerPathTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.FtpServerPathTextBox.ErrorState = false;
         this.FtpServerPathTextBox.ErrorToolTip = this.toolTipHost;
         this.FtpServerPathTextBox.ErrorToolTipDuration = 5000;
         this.FtpServerPathTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
         this.FtpServerPathTextBox.ErrorToolTipText = "FTP path must be the full path to the folder\r\ncontaining the log files.";
         this.FtpServerPathTextBox.Location = new System.Drawing.Point(139, 69);
         this.FtpServerPathTextBox.Name = "FtpServerPathTextBox";
         this.FtpServerPathTextBox.Size = new System.Drawing.Size(228, 20);
         this.FtpServerPathTextBox.TabIndex = 3;
         this.FtpServerPathTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // radioHTTP
         // 
         this.radioHTTP.AutoSize = true;
         this.radioHTTP.CausesValidation = false;
         this.radioHTTP.Location = new System.Drawing.Point(85, 6);
         this.radioHTTP.Name = "radioHTTP";
         this.radioHTTP.Size = new System.Drawing.Size(82, 17);
         this.radioHTTP.TabIndex = 11;
         this.radioHTTP.Tag = "3";
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
         this.txtWebURL.ErrorToolTipText = "URL must be the full path to the location containing the log files.";
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
         // FtpUsernameTextBox
         // 
         this.FtpUsernameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FtpUsernameTextBox.DoubleBuffered = true;
         this.FtpUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.FtpUsernameTextBox.ErrorState = false;
         this.FtpUsernameTextBox.ErrorToolTip = this.toolTipHost;
         this.FtpUsernameTextBox.ErrorToolTipDuration = 5000;
         this.FtpUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.FtpUsernameTextBox.ErrorToolTipText = "";
         this.FtpUsernameTextBox.Location = new System.Drawing.Point(75, 96);
         this.FtpUsernameTextBox.Name = "FtpUsernameTextBox";
         this.FtpUsernameTextBox.Size = new System.Drawing.Size(106, 20);
         this.FtpUsernameTextBox.TabIndex = 5;
         this.FtpUsernameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // FtpUsernameLabel
         // 
         this.FtpUsernameLabel.AutoSize = true;
         this.FtpUsernameLabel.Location = new System.Drawing.Point(11, 99);
         this.FtpUsernameLabel.Name = "FtpUsernameLabel";
         this.FtpUsernameLabel.Size = new System.Drawing.Size(58, 13);
         this.FtpUsernameLabel.TabIndex = 4;
         this.FtpUsernameLabel.Text = "Username:";
         // 
         // FtpPasswordTextBox
         // 
         this.FtpPasswordTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FtpPasswordTextBox.DoubleBuffered = true;
         this.FtpPasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.FtpPasswordTextBox.ErrorState = false;
         this.FtpPasswordTextBox.ErrorToolTip = this.toolTipHost;
         this.FtpPasswordTextBox.ErrorToolTipDuration = 5000;
         this.FtpPasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.FtpPasswordTextBox.ErrorToolTipText = "";
         this.FtpPasswordTextBox.Location = new System.Drawing.Point(249, 96);
         this.FtpPasswordTextBox.Name = "FtpPasswordTextBox";
         this.FtpPasswordTextBox.Size = new System.Drawing.Size(118, 20);
         this.FtpPasswordTextBox.TabIndex = 7;
         this.FtpPasswordTextBox.UseSystemPasswordChar = true;
         this.FtpPasswordTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // FtpPasswordLabel
         // 
         this.FtpPasswordLabel.AutoSize = true;
         this.FtpPasswordLabel.Location = new System.Drawing.Point(187, 99);
         this.FtpPasswordLabel.Name = "FtpPasswordLabel";
         this.FtpPasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.FtpPasswordLabel.TabIndex = 6;
         this.FtpPasswordLabel.Text = "Password:";
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
         this.grpLocal.TabIndex = 11;
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
         this.grpFTP.Controls.Add(this.FtpServerPortTextBox);
         this.grpFTP.Controls.Add(this.FtpServerPortLabel);
         this.grpFTP.Controls.Add(this.pnlFtpMode);
         this.grpFTP.Controls.Add(this.labelWrapper1);
         this.grpFTP.Controls.Add(this.FtpServerNameLabel);
         this.grpFTP.Controls.Add(this.FtpServerPathLabel);
         this.grpFTP.Controls.Add(this.FtpPasswordTextBox);
         this.grpFTP.Controls.Add(this.FtpUsernameLabel);
         this.grpFTP.Controls.Add(this.FtpServerNameTextBox);
         this.grpFTP.Controls.Add(this.FtpPasswordLabel);
         this.grpFTP.Controls.Add(this.FtpServerPathTextBox);
         this.grpFTP.Controls.Add(this.FtpUsernameTextBox);
         this.grpFTP.Location = new System.Drawing.Point(7, 166);
         this.grpFTP.Name = "grpFTP";
         this.grpFTP.Size = new System.Drawing.Size(378, 152);
         this.grpFTP.TabIndex = 13;
         this.grpFTP.TabStop = false;
         // 
         // FtpServerPortTextBox
         // 
         this.FtpServerPortTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.FtpServerPortTextBox.DoubleBuffered = true;
         this.FtpServerPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.FtpServerPortTextBox.ErrorState = false;
         this.FtpServerPortTextBox.ErrorToolTip = this.toolTipHost;
         this.FtpServerPortTextBox.ErrorToolTipDuration = 5000;
         this.FtpServerPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.FtpServerPortTextBox.ErrorToolTipText = "";
         this.FtpServerPortTextBox.Location = new System.Drawing.Point(139, 42);
         this.FtpServerPortTextBox.Name = "FtpServerPortTextBox";
         this.FtpServerPortTextBox.Size = new System.Drawing.Size(54, 20);
         this.FtpServerPortTextBox.TabIndex = 13;
         this.FtpServerPortTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.FtpServerPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // FtpServerPortLabel
         // 
         this.FtpServerPortLabel.AutoSize = true;
         this.FtpServerPortLabel.Location = new System.Drawing.Point(11, 45);
         this.FtpServerPortLabel.Name = "FtpServerPortLabel";
         this.FtpServerPortLabel.Size = new System.Drawing.Size(86, 13);
         this.FtpServerPortLabel.TabIndex = 12;
         this.FtpServerPortLabel.Text = "FTP Server Port:";
         // 
         // pnlFtpMode
         // 
         this.pnlFtpMode.Controls.Add(this.radioActive);
         this.pnlFtpMode.Controls.Add(this.radioPassive);
         this.pnlFtpMode.Location = new System.Drawing.Point(75, 118);
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
         this.grpHTTP.TabIndex = 12;
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
         this.txtClientMegahertz.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // ClientTimeOffsetLabel
         // 
         this.ClientTimeOffsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientTimeOffsetLabel.AutoSize = true;
         this.ClientTimeOffsetLabel.Location = new System.Drawing.Point(64, 355);
         this.ClientTimeOffsetLabel.Name = "ClientTimeOffsetLabel";
         this.ClientTimeOffsetLabel.Size = new System.Drawing.Size(136, 13);
         this.ClientTimeOffsetLabel.TabIndex = 17;
         this.ClientTimeOffsetLabel.Text = "Client Time Offset (Minutes)";
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
         this.btnTestConnection.TabIndex = 14;
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
         this.pnlHostType.TabIndex = 10;
         this.pnlHostType.ValueMember = null;
         // 
         // DummyTextBox
         // 
         this.DummyTextBox.Location = new System.Drawing.Point(331, 324);
         this.DummyTextBox.Name = "DummyTextBox";
         this.DummyTextBox.Size = new System.Drawing.Size(49, 20);
         this.DummyTextBox.TabIndex = 20;
         // 
         // LegacyClientSetupDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 385);
         this.Controls.Add(this.grpFTP);
         this.Controls.Add(this.grpLocal);
         this.Controls.Add(this.DummyTextBox);
         this.Controls.Add(this.pnlHostType);
         this.Controls.Add(this.btnTestConnection);
         this.Controls.Add(this.txtQueueFileName);
         this.Controls.Add(this.lblQueueFileName);
         this.Controls.Add(this.ClientTimeOffsetLabel);
         this.Controls.Add(this.ClientTimeOffsetUpDown);
         this.Controls.Add(this.txtClientMegahertz);
         this.Controls.Add(this.lblClientMegahertz);
         this.Controls.Add(this.ClientNoUtcOffsetCheckBox);
         this.Controls.Add(this.ClientNameLabel);
         this.Controls.Add(this.DialogCancelButton);
         this.Controls.Add(this.ClientNameTextBox);
         this.Controls.Add(this.lblLogFileName);
         this.Controls.Add(this.DialogOkButton);
         this.Controls.Add(this.lblUnitFileName);
         this.Controls.Add(this.txtLogFileName);
         this.Controls.Add(this.txtUnitFileName);
         this.Controls.Add(this.grpHTTP);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "LegacyClientSetupDialog";
         this.ShowInTaskbar = false;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Legacy Client Setup";
         this.Shown += new System.EventHandler(this.LegacyClientSetupDialogShown);
         ((System.ComponentModel.ISupportInitialize)(this.ClientTimeOffsetUpDown)).EndInit();
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

      private ButtonWrapper DialogOkButton;
      private ButtonWrapper DialogCancelButton;
      private ButtonWrapper btnBrowseLocal;
      private LabelWrapper ClientNameLabel;
      private LabelWrapper FtpServerNameLabel;
      private LabelWrapper FtpServerPathLabel;
      private LabelWrapper lblWebURL;
      private LabelWrapper FtpUsernameLabel;
      private LabelWrapper FtpPasswordLabel;
      private LabelWrapper lblWebUser;
      private LabelWrapper lblWebPass;
      private ValidatingTextBox txtLocalPath;
      private ValidatingTextBox ClientNameTextBox;
      private ValidatingTextBox FtpServerNameTextBox;
      private ValidatingTextBox FtpServerPathTextBox;
      private ValidatingTextBox txtWebURL;
      private ValidatingTextBox FtpUsernameTextBox;
      private ValidatingTextBox FtpPasswordTextBox;
      private ValidatingTextBox txtWebUser;
      private ValidatingTextBox txtWebPass;
      private GroupBoxWrapper grpLocal;
      private LabelWrapper lblLogFolder;
      private GroupBoxWrapper grpFTP;
      private GroupBoxWrapper grpHTTP;

      #endregion
      private System.Windows.Forms.FolderBrowserDialog openLogFolder;
      private ValidatingTextBox txtLogFileName;
      private LabelWrapper lblLogFileName;
      private ValidatingTextBox txtUnitFileName;
      private LabelWrapper lblUnitFileName;
      private CheckBoxWrapper ClientNoUtcOffsetCheckBox;
      private LabelWrapper lblClientMegahertz;
      private ValidatingTextBox txtClientMegahertz;
      private LabelWrapper ClientTimeOffsetLabel;
      private System.Windows.Forms.NumericUpDown ClientTimeOffsetUpDown;
      private RadioButtonWrapper radioLocal;
      private RadioButtonWrapper radioFTP;
      private RadioButtonWrapper radioHTTP;
      private LabelWrapper lblQueueFileName;
      private ValidatingTextBox txtQueueFileName;
      private RadioButtonWrapper radioPassive;
      private RadioButtonWrapper radioActive;
      private LabelWrapper labelWrapper1;
      private System.Windows.Forms.ToolTip toolTipHost;
      private ButtonWrapper btnTestConnection;
      private RadioPanel pnlHostType;
      private RadioPanel pnlFtpMode;
      private TextBoxWrapper DummyTextBox;
      private ValidatingTextBox FtpServerPortTextBox;
      private LabelWrapper FtpServerPortLabel;

   }
}
