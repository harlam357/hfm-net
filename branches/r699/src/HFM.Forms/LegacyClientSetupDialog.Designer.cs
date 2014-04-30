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
         this.LogFolderTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.toolTipHost = new System.Windows.Forms.ToolTip(this.components);
         this.ClientNoUtcOffsetCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.ClientTimeOffsetUpDown = new System.Windows.Forms.NumericUpDown();
         this.LogFolderBrowseButton = new HFM.Forms.Controls.ButtonWrapper();
         this.LegacyClientSubTypeLocalPathRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.ClientNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.ClientNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.LegacyClientSubTypeFtpRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.FtpServerNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpServerNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpServerPathLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpServerPathTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.LegacyClientSubTypeHttpRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.WebUrlTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebUrlLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpUsernameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpUsernameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpPasswordTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpPasswordLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebUsernameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebPasswordTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebUsernameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebPasswordLabel = new HFM.Forms.Controls.LabelWrapper();
         this.LocalPathGroupBox = new HFM.Forms.Controls.GroupBoxWrapper();
         this.LogFolderLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpGroupBox = new HFM.Forms.Controls.GroupBoxWrapper();
         this.FtpServerPortTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.FtpServerPortLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpModePanel = new harlam357.Windows.Forms.RadioPanel();
         this.FtpModeActiveRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.FtpModePassiveRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.FtpModeLabel = new HFM.Forms.Controls.LabelWrapper();
         this.HttpGroupBox = new HFM.Forms.Controls.GroupBoxWrapper();
         this.LogFolderBrowserDialog = new System.Windows.Forms.FolderBrowserDialog();
         this.LogFileNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.LogFileNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.UnitInfoFileNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.UnitInfoFileNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.ClientTimeOffsetLabel = new HFM.Forms.Controls.LabelWrapper();
         this.QueueFileNameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.QueueFileNameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.TestConnectionButton = new HFM.Forms.Controls.ButtonWrapper();
         this.LegacyClientSubTypePanel = new harlam357.Windows.Forms.RadioPanel();
         this.DummyTextBox = new HFM.Forms.Controls.TextBoxWrapper();
         ((System.ComponentModel.ISupportInitialize)(this.ClientTimeOffsetUpDown)).BeginInit();
         this.LocalPathGroupBox.SuspendLayout();
         this.FtpGroupBox.SuspendLayout();
         this.FtpModePanel.SuspendLayout();
         this.HttpGroupBox.SuspendLayout();
         this.LegacyClientSubTypePanel.SuspendLayout();
         this.SuspendLayout();
         // 
         // DialogOkButton
         // 
         this.DialogOkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.DialogOkButton.Location = new System.Drawing.Point(212, 326);
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
         this.DialogCancelButton.Location = new System.Drawing.Point(304, 326);
         this.DialogCancelButton.Name = "DialogCancelButton";
         this.DialogCancelButton.Size = new System.Drawing.Size(81, 25);
         this.DialogCancelButton.TabIndex = 19;
         this.DialogCancelButton.Text = "Cancel";
         this.DialogCancelButton.UseVisualStyleBackColor = true;
         // 
         // LogFolderTextBox
         // 
         this.LogFolderTextBox.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.LogFolderTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.LogFolderTextBox.DoubleBuffered = true;
         this.LogFolderTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.LogFolderTextBox.ErrorState = false;
         this.LogFolderTextBox.ErrorToolTip = this.toolTipHost;
         this.LogFolderTextBox.ErrorToolTipDuration = 5000;
         this.LogFolderTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.LogFolderTextBox.ErrorToolTipText = "Log Folder must be a valid local or network (UNC) path.";
         this.LogFolderTextBox.Location = new System.Drawing.Point(11, 35);
         this.LogFolderTextBox.Name = "LogFolderTextBox";
         this.LogFolderTextBox.Size = new System.Drawing.Size(320, 20);
         this.LogFolderTextBox.TabIndex = 1;
         this.LogFolderTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // ClientNoUtcOffsetCheckBox
         // 
         this.ClientNoUtcOffsetCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientNoUtcOffsetCheckBox.AutoSize = true;
         this.ClientNoUtcOffsetCheckBox.Location = new System.Drawing.Point(12, 302);
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
         this.ClientTimeOffsetUpDown.Location = new System.Drawing.Point(8, 328);
         this.ClientTimeOffsetUpDown.Name = "ClientTimeOffsetUpDown";
         this.ClientTimeOffsetUpDown.Size = new System.Drawing.Size(54, 20);
         this.ClientTimeOffsetUpDown.TabIndex = 16;
         this.toolTipHost.SetToolTip(this.ClientTimeOffsetUpDown, resources.GetString("ClientTimeOffsetUpDown.ToolTip"));
         // 
         // LogFolderBrowseButton
         // 
         this.LogFolderBrowseButton.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.LogFolderBrowseButton.Location = new System.Drawing.Point(337, 33);
         this.LogFolderBrowseButton.Name = "LogFolderBrowseButton";
         this.LogFolderBrowseButton.Size = new System.Drawing.Size(30, 24);
         this.LogFolderBrowseButton.TabIndex = 2;
         this.LogFolderBrowseButton.Text = "...";
         this.LogFolderBrowseButton.UseVisualStyleBackColor = true;
         this.LogFolderBrowseButton.Click += new System.EventHandler(this.btnBrowseLocal_Click);
         // 
         // LegacyClientSubTypeLocalPathRadioButton
         // 
         this.LegacyClientSubTypeLocalPathRadioButton.AutoSize = true;
         this.LegacyClientSubTypeLocalPathRadioButton.CausesValidation = false;
         this.LegacyClientSubTypeLocalPathRadioButton.Location = new System.Drawing.Point(3, 6);
         this.LegacyClientSubTypeLocalPathRadioButton.Name = "LegacyClientSubTypeLocalPathRadioButton";
         this.LegacyClientSubTypeLocalPathRadioButton.Size = new System.Drawing.Size(76, 17);
         this.LegacyClientSubTypeLocalPathRadioButton.TabIndex = 0;
         this.LegacyClientSubTypeLocalPathRadioButton.Tag = "1";
         this.LegacyClientSubTypeLocalPathRadioButton.Text = "Local Path";
         this.LegacyClientSubTypeLocalPathRadioButton.UseVisualStyleBackColor = true;
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
         // LegacyClientSubTypeFtpRadioButton
         // 
         this.LegacyClientSubTypeFtpRadioButton.AutoSize = true;
         this.LegacyClientSubTypeFtpRadioButton.CausesValidation = false;
         this.LegacyClientSubTypeFtpRadioButton.Location = new System.Drawing.Point(174, 6);
         this.LegacyClientSubTypeFtpRadioButton.Name = "LegacyClientSubTypeFtpRadioButton";
         this.LegacyClientSubTypeFtpRadioButton.Size = new System.Drawing.Size(79, 17);
         this.LegacyClientSubTypeFtpRadioButton.TabIndex = 2;
         this.LegacyClientSubTypeFtpRadioButton.Tag = "2";
         this.LegacyClientSubTypeFtpRadioButton.Text = "FTP Server";
         this.LegacyClientSubTypeFtpRadioButton.UseVisualStyleBackColor = true;
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
         this.FtpServerPathLabel.TabIndex = 4;
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
         this.FtpServerPathTextBox.TabIndex = 5;
         this.FtpServerPathTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // LegacyClientSubTypeHttpRadioButton
         // 
         this.LegacyClientSubTypeHttpRadioButton.AutoSize = true;
         this.LegacyClientSubTypeHttpRadioButton.CausesValidation = false;
         this.LegacyClientSubTypeHttpRadioButton.Location = new System.Drawing.Point(85, 6);
         this.LegacyClientSubTypeHttpRadioButton.Name = "LegacyClientSubTypeHttpRadioButton";
         this.LegacyClientSubTypeHttpRadioButton.Size = new System.Drawing.Size(82, 17);
         this.LegacyClientSubTypeHttpRadioButton.TabIndex = 1;
         this.LegacyClientSubTypeHttpRadioButton.Tag = "3";
         this.LegacyClientSubTypeHttpRadioButton.Text = "Web Server";
         this.LegacyClientSubTypeHttpRadioButton.UseVisualStyleBackColor = true;
         // 
         // WebUrlTextBox
         // 
         this.WebUrlTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.WebUrlTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.WebUrlTextBox.DoubleBuffered = true;
         this.WebUrlTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebUrlTextBox.ErrorState = false;
         this.WebUrlTextBox.ErrorToolTip = this.toolTipHost;
         this.WebUrlTextBox.ErrorToolTipDuration = 5000;
         this.WebUrlTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebUrlTextBox.ErrorToolTipText = "URL must be the full path to the location containing the log files.";
         this.WebUrlTextBox.Location = new System.Drawing.Point(152, 15);
         this.WebUrlTextBox.Name = "WebUrlTextBox";
         this.WebUrlTextBox.Size = new System.Drawing.Size(215, 20);
         this.WebUrlTextBox.TabIndex = 1;
         this.WebUrlTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebUrlLabel
         // 
         this.WebUrlLabel.AutoSize = true;
         this.WebUrlLabel.Location = new System.Drawing.Point(11, 18);
         this.WebUrlLabel.Name = "WebUrlLabel";
         this.WebUrlLabel.Size = new System.Drawing.Size(110, 13);
         this.WebUrlLabel.TabIndex = 0;
         this.WebUrlLabel.Text = "URL to Log Directory:";
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
         this.FtpUsernameTextBox.TabIndex = 7;
         this.FtpUsernameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // FtpUsernameLabel
         // 
         this.FtpUsernameLabel.AutoSize = true;
         this.FtpUsernameLabel.Location = new System.Drawing.Point(11, 99);
         this.FtpUsernameLabel.Name = "FtpUsernameLabel";
         this.FtpUsernameLabel.Size = new System.Drawing.Size(58, 13);
         this.FtpUsernameLabel.TabIndex = 6;
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
         this.FtpPasswordTextBox.TabIndex = 9;
         this.FtpPasswordTextBox.UseSystemPasswordChar = true;
         this.FtpPasswordTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // FtpPasswordLabel
         // 
         this.FtpPasswordLabel.AutoSize = true;
         this.FtpPasswordLabel.Location = new System.Drawing.Point(187, 99);
         this.FtpPasswordLabel.Name = "FtpPasswordLabel";
         this.FtpPasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.FtpPasswordLabel.TabIndex = 8;
         this.FtpPasswordLabel.Text = "Password:";
         // 
         // WebUsernameTextBox
         // 
         this.WebUsernameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.WebUsernameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.WebUsernameTextBox.DoubleBuffered = true;
         this.WebUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebUsernameTextBox.ErrorState = false;
         this.WebUsernameTextBox.ErrorToolTip = this.toolTipHost;
         this.WebUsernameTextBox.ErrorToolTipDuration = 5000;
         this.WebUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebUsernameTextBox.ErrorToolTipText = "";
         this.WebUsernameTextBox.Location = new System.Drawing.Point(152, 42);
         this.WebUsernameTextBox.Name = "WebUsernameTextBox";
         this.WebUsernameTextBox.Size = new System.Drawing.Size(215, 20);
         this.WebUsernameTextBox.TabIndex = 3;
         this.WebUsernameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebPasswordTextBox
         // 
         this.WebPasswordTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
         this.WebPasswordTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.WebPasswordTextBox.DoubleBuffered = true;
         this.WebPasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebPasswordTextBox.ErrorState = false;
         this.WebPasswordTextBox.ErrorToolTip = this.toolTipHost;
         this.WebPasswordTextBox.ErrorToolTipDuration = 5000;
         this.WebPasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebPasswordTextBox.ErrorToolTipText = "";
         this.WebPasswordTextBox.Location = new System.Drawing.Point(152, 69);
         this.WebPasswordTextBox.Name = "WebPasswordTextBox";
         this.WebPasswordTextBox.PasswordChar = '#';
         this.WebPasswordTextBox.Size = new System.Drawing.Size(215, 20);
         this.WebPasswordTextBox.TabIndex = 5;
         this.WebPasswordTextBox.UseSystemPasswordChar = true;
         this.WebPasswordTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebUsernameLabel
         // 
         this.WebUsernameLabel.AutoSize = true;
         this.WebUsernameLabel.Location = new System.Drawing.Point(11, 45);
         this.WebUsernameLabel.Name = "WebUsernameLabel";
         this.WebUsernameLabel.Size = new System.Drawing.Size(118, 13);
         this.WebUsernameLabel.TabIndex = 2;
         this.WebUsernameLabel.Text = "Web Server Username:";
         // 
         // WebPasswordLabel
         // 
         this.WebPasswordLabel.AutoSize = true;
         this.WebPasswordLabel.Location = new System.Drawing.Point(11, 72);
         this.WebPasswordLabel.Name = "WebPasswordLabel";
         this.WebPasswordLabel.Size = new System.Drawing.Size(116, 13);
         this.WebPasswordLabel.TabIndex = 4;
         this.WebPasswordLabel.Text = "Web Server Password:";
         // 
         // LocalPathGroupBox
         // 
         this.LocalPathGroupBox.Controls.Add(this.LogFolderBrowseButton);
         this.LocalPathGroupBox.Controls.Add(this.LogFolderTextBox);
         this.LocalPathGroupBox.Controls.Add(this.LogFolderLabel);
         this.LocalPathGroupBox.Location = new System.Drawing.Point(7, 140);
         this.LocalPathGroupBox.Name = "LocalPathGroupBox";
         this.LocalPathGroupBox.Size = new System.Drawing.Size(378, 69);
         this.LocalPathGroupBox.TabIndex = 11;
         this.LocalPathGroupBox.TabStop = false;
         // 
         // LogFolderLabel
         // 
         this.LogFolderLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
         this.LogFolderLabel.AutoSize = true;
         this.LogFolderLabel.Location = new System.Drawing.Point(11, 16);
         this.LogFolderLabel.Name = "LogFolderLabel";
         this.LogFolderLabel.Size = new System.Drawing.Size(60, 13);
         this.LogFolderLabel.TabIndex = 0;
         this.LogFolderLabel.Text = "Log Folder:";
         // 
         // FtpGroupBox
         // 
         this.FtpGroupBox.Controls.Add(this.FtpServerPortTextBox);
         this.FtpGroupBox.Controls.Add(this.FtpServerPortLabel);
         this.FtpGroupBox.Controls.Add(this.FtpModePanel);
         this.FtpGroupBox.Controls.Add(this.FtpModeLabel);
         this.FtpGroupBox.Controls.Add(this.FtpServerNameLabel);
         this.FtpGroupBox.Controls.Add(this.FtpServerPathLabel);
         this.FtpGroupBox.Controls.Add(this.FtpPasswordTextBox);
         this.FtpGroupBox.Controls.Add(this.FtpUsernameLabel);
         this.FtpGroupBox.Controls.Add(this.FtpServerNameTextBox);
         this.FtpGroupBox.Controls.Add(this.FtpPasswordLabel);
         this.FtpGroupBox.Controls.Add(this.FtpServerPathTextBox);
         this.FtpGroupBox.Controls.Add(this.FtpUsernameTextBox);
         this.FtpGroupBox.Location = new System.Drawing.Point(7, 140);
         this.FtpGroupBox.Name = "FtpGroupBox";
         this.FtpGroupBox.Size = new System.Drawing.Size(378, 152);
         this.FtpGroupBox.TabIndex = 13;
         this.FtpGroupBox.TabStop = false;
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
         this.FtpServerPortTextBox.TabIndex = 3;
         this.FtpServerPortTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.FtpServerPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // FtpServerPortLabel
         // 
         this.FtpServerPortLabel.AutoSize = true;
         this.FtpServerPortLabel.Location = new System.Drawing.Point(11, 45);
         this.FtpServerPortLabel.Name = "FtpServerPortLabel";
         this.FtpServerPortLabel.Size = new System.Drawing.Size(86, 13);
         this.FtpServerPortLabel.TabIndex = 2;
         this.FtpServerPortLabel.Text = "FTP Server Port:";
         // 
         // FtpModePanel
         // 
         this.FtpModePanel.Controls.Add(this.FtpModeActiveRadioButton);
         this.FtpModePanel.Controls.Add(this.FtpModePassiveRadioButton);
         this.FtpModePanel.Location = new System.Drawing.Point(75, 118);
         this.FtpModePanel.Name = "FtpModePanel";
         this.FtpModePanel.Size = new System.Drawing.Size(134, 30);
         this.FtpModePanel.TabIndex = 11;
         this.FtpModePanel.ValueMember = null;
         // 
         // FtpModeActiveRadioButton
         // 
         this.FtpModeActiveRadioButton.AutoSize = true;
         this.FtpModeActiveRadioButton.Location = new System.Drawing.Point(73, 7);
         this.FtpModeActiveRadioButton.Name = "FtpModeActiveRadioButton";
         this.FtpModeActiveRadioButton.Size = new System.Drawing.Size(55, 17);
         this.FtpModeActiveRadioButton.TabIndex = 1;
         this.FtpModeActiveRadioButton.Tag = "1";
         this.FtpModeActiveRadioButton.Text = "Active";
         this.FtpModeActiveRadioButton.UseVisualStyleBackColor = true;
         // 
         // FtpModePassiveRadioButton
         // 
         this.FtpModePassiveRadioButton.AutoSize = true;
         this.FtpModePassiveRadioButton.Checked = true;
         this.FtpModePassiveRadioButton.Location = new System.Drawing.Point(5, 7);
         this.FtpModePassiveRadioButton.Name = "FtpModePassiveRadioButton";
         this.FtpModePassiveRadioButton.Size = new System.Drawing.Size(62, 17);
         this.FtpModePassiveRadioButton.TabIndex = 0;
         this.FtpModePassiveRadioButton.TabStop = true;
         this.FtpModePassiveRadioButton.Tag = "0";
         this.FtpModePassiveRadioButton.Text = "Passive";
         this.FtpModePassiveRadioButton.UseVisualStyleBackColor = true;
         // 
         // FtpModeLabel
         // 
         this.FtpModeLabel.AutoSize = true;
         this.FtpModeLabel.Location = new System.Drawing.Point(11, 126);
         this.FtpModeLabel.Name = "FtpModeLabel";
         this.FtpModeLabel.Size = new System.Drawing.Size(60, 13);
         this.FtpModeLabel.TabIndex = 10;
         this.FtpModeLabel.Text = "FTP Mode:";
         // 
         // HttpGroupBox
         // 
         this.HttpGroupBox.Controls.Add(this.WebPasswordTextBox);
         this.HttpGroupBox.Controls.Add(this.WebUrlTextBox);
         this.HttpGroupBox.Controls.Add(this.WebUsernameTextBox);
         this.HttpGroupBox.Controls.Add(this.WebPasswordLabel);
         this.HttpGroupBox.Controls.Add(this.WebUsernameLabel);
         this.HttpGroupBox.Controls.Add(this.WebUrlLabel);
         this.HttpGroupBox.Location = new System.Drawing.Point(7, 140);
         this.HttpGroupBox.Name = "HttpGroupBox";
         this.HttpGroupBox.Size = new System.Drawing.Size(378, 99);
         this.HttpGroupBox.TabIndex = 12;
         this.HttpGroupBox.TabStop = false;
         // 
         // LogFolderBrowserDialog
         // 
         this.LogFolderBrowserDialog.ShowNewFolderButton = false;
         // 
         // LogFileNameTextBox
         // 
         this.LogFileNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.LogFileNameTextBox.DoubleBuffered = true;
         this.LogFileNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.LogFileNameTextBox.ErrorState = false;
         this.LogFileNameTextBox.ErrorToolTip = this.toolTipHost;
         this.LogFileNameTextBox.ErrorToolTipDuration = 5000;
         this.LogFileNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.LogFileNameTextBox.ErrorToolTipText = "File name contains invalid characters.";
         this.LogFileNameTextBox.Location = new System.Drawing.Point(146, 38);
         this.LogFileNameTextBox.MaxLength = 100;
         this.LogFileNameTextBox.Name = "LogFileNameTextBox";
         this.LogFileNameTextBox.Size = new System.Drawing.Size(237, 20);
         this.LogFileNameTextBox.TabIndex = 5;
         this.LogFileNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // LogFileNameLabel
         // 
         this.LogFileNameLabel.AutoSize = true;
         this.LogFileNameLabel.Location = new System.Drawing.Point(6, 41);
         this.LogFileNameLabel.Name = "LogFileNameLabel";
         this.LogFileNameLabel.Size = new System.Drawing.Size(116, 13);
         this.LogFileNameLabel.TabIndex = 4;
         this.LogFileNameLabel.Text = "Filename for FAHlog.txt";
         // 
         // UnitInfoFileNameTextBox
         // 
         this.UnitInfoFileNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.UnitInfoFileNameTextBox.DoubleBuffered = true;
         this.UnitInfoFileNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.UnitInfoFileNameTextBox.ErrorState = false;
         this.UnitInfoFileNameTextBox.ErrorToolTip = this.toolTipHost;
         this.UnitInfoFileNameTextBox.ErrorToolTipDuration = 5000;
         this.UnitInfoFileNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.UnitInfoFileNameTextBox.ErrorToolTipText = "File name contains invalid characters.";
         this.UnitInfoFileNameTextBox.Location = new System.Drawing.Point(146, 64);
         this.UnitInfoFileNameTextBox.MaxLength = 100;
         this.UnitInfoFileNameTextBox.Name = "UnitInfoFileNameTextBox";
         this.UnitInfoFileNameTextBox.Size = new System.Drawing.Size(237, 20);
         this.UnitInfoFileNameTextBox.TabIndex = 7;
         this.UnitInfoFileNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // UnitInfoFileNameLabel
         // 
         this.UnitInfoFileNameLabel.AutoSize = true;
         this.UnitInfoFileNameLabel.Location = new System.Drawing.Point(6, 67);
         this.UnitInfoFileNameLabel.Name = "UnitInfoFileNameLabel";
         this.UnitInfoFileNameLabel.Size = new System.Drawing.Size(115, 13);
         this.UnitInfoFileNameLabel.TabIndex = 6;
         this.UnitInfoFileNameLabel.Text = "Filename for unitinfo.txt";
         // 
         // ClientTimeOffsetLabel
         // 
         this.ClientTimeOffsetLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
         this.ClientTimeOffsetLabel.AutoSize = true;
         this.ClientTimeOffsetLabel.Location = new System.Drawing.Point(64, 331);
         this.ClientTimeOffsetLabel.Name = "ClientTimeOffsetLabel";
         this.ClientTimeOffsetLabel.Size = new System.Drawing.Size(136, 13);
         this.ClientTimeOffsetLabel.TabIndex = 17;
         this.ClientTimeOffsetLabel.Text = "Client Time Offset (Minutes)";
         // 
         // QueueFileNameLabel
         // 
         this.QueueFileNameLabel.AutoSize = true;
         this.QueueFileNameLabel.Location = new System.Drawing.Point(6, 93);
         this.QueueFileNameLabel.Name = "QueueFileNameLabel";
         this.QueueFileNameLabel.Size = new System.Drawing.Size(115, 13);
         this.QueueFileNameLabel.TabIndex = 8;
         this.QueueFileNameLabel.Text = "Filename for queue.dat";
         // 
         // QueueFileNameTextBox
         // 
         this.QueueFileNameTextBox.BackColor = System.Drawing.SystemColors.Window;
         this.QueueFileNameTextBox.DoubleBuffered = true;
         this.QueueFileNameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.QueueFileNameTextBox.ErrorState = false;
         this.QueueFileNameTextBox.ErrorToolTip = this.toolTipHost;
         this.QueueFileNameTextBox.ErrorToolTipDuration = 5000;
         this.QueueFileNameTextBox.ErrorToolTipPoint = new System.Drawing.Point(230, 0);
         this.QueueFileNameTextBox.ErrorToolTipText = "File name contains invalid characters.";
         this.QueueFileNameTextBox.Location = new System.Drawing.Point(146, 90);
         this.QueueFileNameTextBox.MaxLength = 100;
         this.QueueFileNameTextBox.Name = "QueueFileNameTextBox";
         this.QueueFileNameTextBox.Size = new System.Drawing.Size(237, 20);
         this.QueueFileNameTextBox.TabIndex = 9;
         this.QueueFileNameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // TestConnectionButton
         // 
         this.TestConnectionButton.Location = new System.Drawing.Point(271, 116);
         this.TestConnectionButton.Name = "TestConnectionButton";
         this.TestConnectionButton.Size = new System.Drawing.Size(114, 24);
         this.TestConnectionButton.TabIndex = 14;
         this.TestConnectionButton.Text = "Test Connection";
         this.TestConnectionButton.UseVisualStyleBackColor = true;
         this.TestConnectionButton.Click += new System.EventHandler(this.btnTestConnection_Click);
         // 
         // LegacyClientSubTypePanel
         // 
         this.LegacyClientSubTypePanel.Controls.Add(this.LegacyClientSubTypeLocalPathRadioButton);
         this.LegacyClientSubTypePanel.Controls.Add(this.LegacyClientSubTypeFtpRadioButton);
         this.LegacyClientSubTypePanel.Controls.Add(this.LegacyClientSubTypeHttpRadioButton);
         this.LegacyClientSubTypePanel.Location = new System.Drawing.Point(8, 113);
         this.LegacyClientSubTypePanel.Name = "LegacyClientSubTypePanel";
         this.LegacyClientSubTypePanel.Size = new System.Drawing.Size(257, 30);
         this.LegacyClientSubTypePanel.TabIndex = 10;
         this.LegacyClientSubTypePanel.ValueMember = null;
         // 
         // DummyTextBox
         // 
         this.DummyTextBox.Location = new System.Drawing.Point(331, 298);
         this.DummyTextBox.Name = "DummyTextBox";
         this.DummyTextBox.Size = new System.Drawing.Size(49, 20);
         this.DummyTextBox.TabIndex = 20;
         // 
         // LegacyClientSetupDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(392, 361);
         this.Controls.Add(this.FtpGroupBox);
         this.Controls.Add(this.LocalPathGroupBox);
         this.Controls.Add(this.DummyTextBox);
         this.Controls.Add(this.LegacyClientSubTypePanel);
         this.Controls.Add(this.TestConnectionButton);
         this.Controls.Add(this.QueueFileNameTextBox);
         this.Controls.Add(this.QueueFileNameLabel);
         this.Controls.Add(this.ClientTimeOffsetLabel);
         this.Controls.Add(this.ClientTimeOffsetUpDown);
         this.Controls.Add(this.ClientNoUtcOffsetCheckBox);
         this.Controls.Add(this.ClientNameLabel);
         this.Controls.Add(this.DialogCancelButton);
         this.Controls.Add(this.ClientNameTextBox);
         this.Controls.Add(this.LogFileNameLabel);
         this.Controls.Add(this.DialogOkButton);
         this.Controls.Add(this.UnitInfoFileNameLabel);
         this.Controls.Add(this.LogFileNameTextBox);
         this.Controls.Add(this.UnitInfoFileNameTextBox);
         this.Controls.Add(this.HttpGroupBox);
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
         this.LocalPathGroupBox.ResumeLayout(false);
         this.LocalPathGroupBox.PerformLayout();
         this.FtpGroupBox.ResumeLayout(false);
         this.FtpGroupBox.PerformLayout();
         this.FtpModePanel.ResumeLayout(false);
         this.FtpModePanel.PerformLayout();
         this.HttpGroupBox.ResumeLayout(false);
         this.HttpGroupBox.PerformLayout();
         this.LegacyClientSubTypePanel.ResumeLayout(false);
         this.LegacyClientSubTypePanel.PerformLayout();
         this.ResumeLayout(false);
         this.PerformLayout();

      }

      private ButtonWrapper DialogOkButton;
      private ButtonWrapper DialogCancelButton;
      private ButtonWrapper LogFolderBrowseButton;
      private LabelWrapper ClientNameLabel;
      private LabelWrapper FtpServerNameLabel;
      private LabelWrapper FtpServerPathLabel;
      private LabelWrapper WebUrlLabel;
      private LabelWrapper FtpUsernameLabel;
      private LabelWrapper FtpPasswordLabel;
      private LabelWrapper WebUsernameLabel;
      private LabelWrapper WebPasswordLabel;
      private ValidatingTextBox LogFolderTextBox;
      private ValidatingTextBox ClientNameTextBox;
      private ValidatingTextBox FtpServerNameTextBox;
      private ValidatingTextBox FtpServerPathTextBox;
      private ValidatingTextBox WebUrlTextBox;
      private ValidatingTextBox FtpUsernameTextBox;
      private ValidatingTextBox FtpPasswordTextBox;
      private ValidatingTextBox WebUsernameTextBox;
      private ValidatingTextBox WebPasswordTextBox;
      private GroupBoxWrapper LocalPathGroupBox;
      private LabelWrapper LogFolderLabel;
      private GroupBoxWrapper FtpGroupBox;
      private GroupBoxWrapper HttpGroupBox;

      #endregion
      private System.Windows.Forms.FolderBrowserDialog LogFolderBrowserDialog;
      private ValidatingTextBox LogFileNameTextBox;
      private LabelWrapper LogFileNameLabel;
      private ValidatingTextBox UnitInfoFileNameTextBox;
      private LabelWrapper UnitInfoFileNameLabel;
      private CheckBoxWrapper ClientNoUtcOffsetCheckBox;
      private LabelWrapper ClientTimeOffsetLabel;
      private System.Windows.Forms.NumericUpDown ClientTimeOffsetUpDown;
      private RadioButtonWrapper LegacyClientSubTypeLocalPathRadioButton;
      private RadioButtonWrapper LegacyClientSubTypeFtpRadioButton;
      private RadioButtonWrapper LegacyClientSubTypeHttpRadioButton;
      private LabelWrapper QueueFileNameLabel;
      private ValidatingTextBox QueueFileNameTextBox;
      private RadioButtonWrapper FtpModePassiveRadioButton;
      private RadioButtonWrapper FtpModeActiveRadioButton;
      private LabelWrapper FtpModeLabel;
      private System.Windows.Forms.ToolTip toolTipHost;
      private ButtonWrapper TestConnectionButton;
      private RadioPanel LegacyClientSubTypePanel;
      private RadioPanel FtpModePanel;
      private TextBoxWrapper DummyTextBox;
      private ValidatingTextBox FtpServerPortTextBox;
      private LabelWrapper FtpServerPortLabel;

   }
}
