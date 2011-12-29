/*
 * HFM.NET - User Preferences Form
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

using System;

using harlam357.Windows.Forms;

using HFM.Forms.Controls;

namespace HFM.Forms
{
   partial class PreferencesDialog
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
         System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesDialog));
         this.pnl1CSSSample = new System.Windows.Forms.Panel();
         this.StyleList = new System.Windows.Forms.ListBox();
         this.locateWebFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabSchdTasks = new System.Windows.Forms.TabPage();
         this.grpUpdateData = new HFM.Forms.Controls.GroupBoxWrapper();
         this.chkAllowRunningAsync = new HFM.Forms.Controls.CheckBoxWrapper();
         this.txtCollectMinutes = new harlam357.Windows.Forms.ValidatingTextBox();
         this.toolTipPrefs = new System.Windows.Forms.ToolTip(this.components);
         this.lbl2SchedExplain = new HFM.Forms.Controls.LabelWrapper();
         this.chkScheduled = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkSynchronous = new HFM.Forms.Controls.CheckBoxWrapper();
         this.grpHTMLOutput = new HFM.Forms.Controls.GroupBoxWrapper();
         this.WebSitePortTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebSitePortLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebSitePasswordTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebSiteUsernameTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebSiteServerTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebGenTypePanel = new harlam357.Windows.Forms.RadioPanel();
         this.UploadTypeLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebGenTypeFtpRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.WebGenTypePathRadioButton = new HFM.Forms.Controls.RadioButtonWrapper();
         this.WebSitePasswordLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebSiteUsernameLabel = new HFM.Forms.Controls.LabelWrapper();
         this.WebSiteServerLabel = new HFM.Forms.Controls.LabelWrapper();
         this.FtpModePanel = new harlam357.Windows.Forms.RadioPanel();
         this.radioActive = new HFM.Forms.Controls.RadioButtonWrapper();
         this.radioPassive = new HFM.Forms.Controls.RadioButtonWrapper();
         this.lblFtpMode = new HFM.Forms.Controls.LabelWrapper();
         this.udLimitSize = new System.Windows.Forms.NumericUpDown();
         this.chkLimitSize = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkXml = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkHtml = new HFM.Forms.Controls.CheckBoxWrapper();
         this.TestConnectionButton = new HFM.Forms.Controls.ButtonWrapper();
         this.chkFAHlog = new HFM.Forms.Controls.CheckBoxWrapper();
         this.radioFullRefresh = new HFM.Forms.Controls.RadioButtonWrapper();
         this.radioSchedule = new HFM.Forms.Controls.RadioButtonWrapper();
         this.txtWebGenMinutes = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lbl2MinutesToGen = new HFM.Forms.Controls.LabelWrapper();
         this.btnBrowseWebFolder = new HFM.Forms.Controls.ButtonWrapper();
         this.WebSiteTargetPathTextBox = new harlam357.Windows.Forms.ValidatingTextBox();
         this.WebSiteTargetPathLabel = new HFM.Forms.Controls.LabelWrapper();
         this.chkWebSiteGenerator = new HFM.Forms.Controls.CheckBoxWrapper();
         this.tabStartup = new System.Windows.Forms.TabPage();
         this.grpFileExplorer = new HFM.Forms.Controls.GroupBoxWrapper();
         this.btnBrowseFileExplorer = new HFM.Forms.Controls.ButtonWrapper();
         this.label4 = new HFM.Forms.Controls.LabelWrapper();
         this.txtFileExplorer = new harlam357.Windows.Forms.ValidatingTextBox();
         this.grpLogFileViewer = new HFM.Forms.Controls.GroupBoxWrapper();
         this.btnBrowseLogViewer = new HFM.Forms.Controls.ButtonWrapper();
         this.label3 = new HFM.Forms.Controls.LabelWrapper();
         this.txtLogFileViewer = new harlam357.Windows.Forms.ValidatingTextBox();
         this.grpDefaultConfig = new HFM.Forms.Controls.GroupBoxWrapper();
         this.chkDefaultConfig = new HFM.Forms.Controls.CheckBoxWrapper();
         this.btnBrowseConfigFile = new HFM.Forms.Controls.ButtonWrapper();
         this.txtDefaultConfigFile = new harlam357.Windows.Forms.ValidatingTextBox();
         this.label1 = new HFM.Forms.Controls.LabelWrapper();
         this.grpStartup = new HFM.Forms.Controls.GroupBoxWrapper();
         this.chkCheckForUpdate = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkAutoRun = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkRunMinimized = new HFM.Forms.Controls.CheckBoxWrapper();
         this.tabOptions = new System.Windows.Forms.TabPage();
         this.grpShowStyle = new HFM.Forms.Controls.GroupBoxWrapper();
         this.cboShowStyle = new HFM.Forms.Controls.ComboBoxWrapper();
         this.labelWrapper2 = new HFM.Forms.Controls.LabelWrapper();
         this.grpInteractiveOptions = new HFM.Forms.Controls.GroupBoxWrapper();
         this.ShowUserStatsCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.DuplicateUserCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.DuplicateProjectCheckBox = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkEtaAsDate = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkCalcBonus = new HFM.Forms.Controls.CheckBoxWrapper();
         this.label2 = new HFM.Forms.Controls.LabelWrapper();
         this.cboPpdCalc = new HFM.Forms.Controls.ComboBoxWrapper();
         this.chkOffline = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkColorLog = new HFM.Forms.Controls.CheckBoxWrapper();
         this.udDecimalPlaces = new System.Windows.Forms.NumericUpDown();
         this.chkAutoSave = new HFM.Forms.Controls.CheckBoxWrapper();
         this.labelWrapper1 = new HFM.Forms.Controls.LabelWrapper();
         this.grpDebugMessageLevel = new HFM.Forms.Controls.GroupBoxWrapper();
         this.cboMessageLevel = new HFM.Forms.Controls.ComboBoxWrapper();
         this.label6 = new HFM.Forms.Controls.LabelWrapper();
         this.tabReporting = new System.Windows.Forms.TabPage();
         this.grpReportSelections = new HFM.Forms.Controls.GroupBoxWrapper();
         this.chkClientHung = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkClientEuePause = new HFM.Forms.Controls.CheckBoxWrapper();
         this.grpEmailSettings = new HFM.Forms.Controls.GroupBoxWrapper();
         this.txtSmtpServerPort = new harlam357.Windows.Forms.ValidatingTextBox();
         this.labelWrapper3 = new HFM.Forms.Controls.LabelWrapper();
         this.chkEmailSecure = new HFM.Forms.Controls.CheckBoxWrapper();
         this.btnTestEmail = new HFM.Forms.Controls.ButtonWrapper();
         this.txtSmtpPassword = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtSmtpUsername = new harlam357.Windows.Forms.ValidatingTextBox();
         this.labelWrapper4 = new HFM.Forms.Controls.LabelWrapper();
         this.labelWrapper5 = new HFM.Forms.Controls.LabelWrapper();
         this.lblFromEmailAddress = new HFM.Forms.Controls.LabelWrapper();
         this.txtFromEmailAddress = new harlam357.Windows.Forms.ValidatingTextBox();
         this.chkEnableEmail = new HFM.Forms.Controls.CheckBoxWrapper();
         this.lblSmtpServer = new HFM.Forms.Controls.LabelWrapper();
         this.lblToAddress = new HFM.Forms.Controls.LabelWrapper();
         this.txtSmtpServer = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtToEmailAddress = new harlam357.Windows.Forms.ValidatingTextBox();
         this.tabWeb = new System.Windows.Forms.TabPage();
         this.grpProjectDownload = new HFM.Forms.Controls.GroupBoxWrapper();
         this.txtProjectDownloadUrl = new harlam357.Windows.Forms.ValidatingTextBox();
         this.label5 = new HFM.Forms.Controls.LabelWrapper();
         this.grpWebStats = new HFM.Forms.Controls.GroupBoxWrapper();
         this.lbl3EOCUserID = new HFM.Forms.Controls.LabelWrapper();
         this.lbl3StanfordUserID = new HFM.Forms.Controls.LabelWrapper();
         this.linkTeam = new System.Windows.Forms.LinkLabel();
         this.txtEOCUserID = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtStanfordTeamID = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lbl3StanfordTeamID = new HFM.Forms.Controls.LabelWrapper();
         this.linkStanford = new System.Windows.Forms.LinkLabel();
         this.txtStanfordUserID = new harlam357.Windows.Forms.ValidatingTextBox();
         this.linkEOC = new System.Windows.Forms.LinkLabel();
         this.grpWebProxy = new HFM.Forms.Controls.GroupBoxWrapper();
         this.chkUseProxy = new HFM.Forms.Controls.CheckBoxWrapper();
         this.chkUseProxyAuth = new HFM.Forms.Controls.CheckBoxWrapper();
         this.txtProxyPass = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtProxyUser = new harlam357.Windows.Forms.ValidatingTextBox();
         this.txtProxyPort = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lbl3ProxyPass = new HFM.Forms.Controls.LabelWrapper();
         this.txtProxyServer = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lbl3ProxyUser = new HFM.Forms.Controls.LabelWrapper();
         this.lbl3Port = new HFM.Forms.Controls.LabelWrapper();
         this.lbl3Proxy = new HFM.Forms.Controls.LabelWrapper();
         this.tabVisStyles = new System.Windows.Forms.TabPage();
         this.btnMobileSummaryBrowse = new HFM.Forms.Controls.ButtonWrapper();
         this.txtMobileSummary = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblMobileSummary = new HFM.Forms.Controls.LabelWrapper();
         this.btnMobileOverviewBrowse = new HFM.Forms.Controls.ButtonWrapper();
         this.txtMobileOverview = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblMobileOverview = new HFM.Forms.Controls.LabelWrapper();
         this.btnInstanceBrowse = new HFM.Forms.Controls.ButtonWrapper();
         this.txtInstance = new harlam357.Windows.Forms.ValidatingTextBox();
         this.SlotXsltLabel = new HFM.Forms.Controls.LabelWrapper();
         this.btnSummaryBrowse = new HFM.Forms.Controls.ButtonWrapper();
         this.txtSummary = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblSummary = new HFM.Forms.Controls.LabelWrapper();
         this.btnOverviewBrowse = new HFM.Forms.Controls.ButtonWrapper();
         this.txtOverview = new harlam357.Windows.Forms.ValidatingTextBox();
         this.lblOverview = new HFM.Forms.Controls.LabelWrapper();
         this.lbl1Preview = new HFM.Forms.Controls.LabelWrapper();
         this.lbl1Style = new HFM.Forms.Controls.LabelWrapper();
         this.openConfigDialog = new System.Windows.Forms.OpenFileDialog();
         this.btnOK = new HFM.Forms.Controls.ButtonWrapper();
         this.btnCancel = new HFM.Forms.Controls.ButtonWrapper();
         this.tabControl1.SuspendLayout();
         this.tabSchdTasks.SuspendLayout();
         this.grpUpdateData.SuspendLayout();
         this.grpHTMLOutput.SuspendLayout();
         this.WebGenTypePanel.SuspendLayout();
         this.FtpModePanel.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udLimitSize)).BeginInit();
         this.tabStartup.SuspendLayout();
         this.grpFileExplorer.SuspendLayout();
         this.grpLogFileViewer.SuspendLayout();
         this.grpDefaultConfig.SuspendLayout();
         this.grpStartup.SuspendLayout();
         this.tabOptions.SuspendLayout();
         this.grpShowStyle.SuspendLayout();
         this.grpInteractiveOptions.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).BeginInit();
         this.grpDebugMessageLevel.SuspendLayout();
         this.tabReporting.SuspendLayout();
         this.grpReportSelections.SuspendLayout();
         this.grpEmailSettings.SuspendLayout();
         this.tabWeb.SuspendLayout();
         this.grpProjectDownload.SuspendLayout();
         this.grpWebStats.SuspendLayout();
         this.grpWebProxy.SuspendLayout();
         this.tabVisStyles.SuspendLayout();
         this.SuspendLayout();
         // 
         // pnl1CSSSample
         // 
         this.pnl1CSSSample.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
         this.pnl1CSSSample.Location = new System.Drawing.Point(132, 27);
         this.pnl1CSSSample.Name = "pnl1CSSSample";
         this.pnl1CSSSample.Size = new System.Drawing.Size(358, 134);
         this.pnl1CSSSample.TabIndex = 3;
         // 
         // StyleList
         // 
         this.StyleList.FormattingEnabled = true;
         this.StyleList.Location = new System.Drawing.Point(6, 27);
         this.StyleList.Name = "StyleList";
         this.StyleList.Size = new System.Drawing.Size(120, 134);
         this.StyleList.Sorted = true;
         this.StyleList.TabIndex = 2;
         this.StyleList.SelectedIndexChanged += new System.EventHandler(this.StyleList_SelectedIndexChanged);
         // 
         // tabControl1
         // 
         this.tabControl1.Controls.Add(this.tabSchdTasks);
         this.tabControl1.Controls.Add(this.tabStartup);
         this.tabControl1.Controls.Add(this.tabOptions);
         this.tabControl1.Controls.Add(this.tabReporting);
         this.tabControl1.Controls.Add(this.tabWeb);
         this.tabControl1.Controls.Add(this.tabVisStyles);
         this.tabControl1.HotTrack = true;
         this.tabControl1.Location = new System.Drawing.Point(13, 13);
         this.tabControl1.Multiline = true;
         this.tabControl1.Name = "tabControl1";
         this.tabControl1.SelectedIndex = 0;
         this.tabControl1.Size = new System.Drawing.Size(509, 329);
         this.tabControl1.TabIndex = 5;
         this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
         // 
         // tabSchdTasks
         // 
         this.tabSchdTasks.BackColor = System.Drawing.Color.Transparent;
         this.tabSchdTasks.Controls.Add(this.grpUpdateData);
         this.tabSchdTasks.Controls.Add(this.grpHTMLOutput);
         this.tabSchdTasks.Location = new System.Drawing.Point(4, 22);
         this.tabSchdTasks.Name = "tabSchdTasks";
         this.tabSchdTasks.Padding = new System.Windows.Forms.Padding(3);
         this.tabSchdTasks.Size = new System.Drawing.Size(501, 303);
         this.tabSchdTasks.TabIndex = 2;
         this.tabSchdTasks.Text = "Scheduled Tasks";
         this.tabSchdTasks.UseVisualStyleBackColor = true;
         // 
         // grpUpdateData
         // 
         this.grpUpdateData.Controls.Add(this.chkAllowRunningAsync);
         this.grpUpdateData.Controls.Add(this.txtCollectMinutes);
         this.grpUpdateData.Controls.Add(this.lbl2SchedExplain);
         this.grpUpdateData.Controls.Add(this.chkScheduled);
         this.grpUpdateData.Controls.Add(this.chkSynchronous);
         this.grpUpdateData.Location = new System.Drawing.Point(6, 9);
         this.grpUpdateData.Name = "grpUpdateData";
         this.grpUpdateData.Size = new System.Drawing.Size(489, 51);
         this.grpUpdateData.TabIndex = 0;
         this.grpUpdateData.TabStop = false;
         this.grpUpdateData.Text = "Refresh Client Data";
         // 
         // chkAllowRunningAsync
         // 
         this.chkAllowRunningAsync.AutoSize = true;
         this.chkAllowRunningAsync.Location = new System.Drawing.Point(331, 20);
         this.chkAllowRunningAsync.Name = "chkAllowRunningAsync";
         this.chkAllowRunningAsync.Size = new System.Drawing.Size(156, 17);
         this.chkAllowRunningAsync.TabIndex = 6;
         this.chkAllowRunningAsync.Text = "Allow Asynchronous Clocks";
         this.chkAllowRunningAsync.UseVisualStyleBackColor = true;
         // 
         // txtCollectMinutes
         // 
         this.txtCollectMinutes.BackColor = System.Drawing.SystemColors.Control;
         this.txtCollectMinutes.DoubleBuffered = true;
         this.txtCollectMinutes.Enabled = false;
         this.txtCollectMinutes.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtCollectMinutes.ErrorState = false;
         this.txtCollectMinutes.ErrorToolTip = this.toolTipPrefs;
         this.txtCollectMinutes.ErrorToolTipDuration = 5000;
         this.txtCollectMinutes.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtCollectMinutes.ErrorToolTipText = "";
         this.txtCollectMinutes.Location = new System.Drawing.Point(61, 18);
         this.txtCollectMinutes.MaxLength = 3;
         this.txtCollectMinutes.Name = "txtCollectMinutes";
         this.txtCollectMinutes.ReadOnly = true;
         this.txtCollectMinutes.Size = new System.Drawing.Size(39, 20);
         this.txtCollectMinutes.TabIndex = 4;
         this.txtCollectMinutes.Text = "15";
         this.txtCollectMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         this.txtCollectMinutes.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtCollectMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // lbl2SchedExplain
         // 
         this.lbl2SchedExplain.AutoSize = true;
         this.lbl2SchedExplain.Location = new System.Drawing.Point(103, 22);
         this.lbl2SchedExplain.Name = "lbl2SchedExplain";
         this.lbl2SchedExplain.Size = new System.Drawing.Size(44, 13);
         this.lbl2SchedExplain.TabIndex = 5;
         this.lbl2SchedExplain.Text = "Minutes";
         // 
         // chkScheduled
         // 
         this.chkScheduled.AutoSize = true;
         this.chkScheduled.Location = new System.Drawing.Point(10, 20);
         this.chkScheduled.Name = "chkScheduled";
         this.chkScheduled.Size = new System.Drawing.Size(53, 17);
         this.chkScheduled.TabIndex = 3;
         this.chkScheduled.Text = "Every";
         this.chkScheduled.UseVisualStyleBackColor = true;
         // 
         // chkSynchronous
         // 
         this.chkSynchronous.AutoSize = true;
         this.chkSynchronous.Location = new System.Drawing.Point(153, 20);
         this.chkSynchronous.Name = "chkSynchronous";
         this.chkSynchronous.Size = new System.Drawing.Size(176, 17);
         this.chkSynchronous.TabIndex = 0;
         this.chkSynchronous.Text = "In Series (synchronous retrieval)";
         this.chkSynchronous.UseVisualStyleBackColor = true;
         // 
         // grpHTMLOutput
         // 
         this.grpHTMLOutput.Controls.Add(this.WebSitePortTextBox);
         this.grpHTMLOutput.Controls.Add(this.WebSitePortLabel);
         this.grpHTMLOutput.Controls.Add(this.WebSitePasswordTextBox);
         this.grpHTMLOutput.Controls.Add(this.WebSiteUsernameTextBox);
         this.grpHTMLOutput.Controls.Add(this.WebSiteServerTextBox);
         this.grpHTMLOutput.Controls.Add(this.WebGenTypePanel);
         this.grpHTMLOutput.Controls.Add(this.WebSitePasswordLabel);
         this.grpHTMLOutput.Controls.Add(this.WebSiteUsernameLabel);
         this.grpHTMLOutput.Controls.Add(this.WebSiteServerLabel);
         this.grpHTMLOutput.Controls.Add(this.FtpModePanel);
         this.grpHTMLOutput.Controls.Add(this.udLimitSize);
         this.grpHTMLOutput.Controls.Add(this.chkLimitSize);
         this.grpHTMLOutput.Controls.Add(this.chkXml);
         this.grpHTMLOutput.Controls.Add(this.chkHtml);
         this.grpHTMLOutput.Controls.Add(this.TestConnectionButton);
         this.grpHTMLOutput.Controls.Add(this.chkFAHlog);
         this.grpHTMLOutput.Controls.Add(this.radioFullRefresh);
         this.grpHTMLOutput.Controls.Add(this.radioSchedule);
         this.grpHTMLOutput.Controls.Add(this.txtWebGenMinutes);
         this.grpHTMLOutput.Controls.Add(this.lbl2MinutesToGen);
         this.grpHTMLOutput.Controls.Add(this.btnBrowseWebFolder);
         this.grpHTMLOutput.Controls.Add(this.WebSiteTargetPathTextBox);
         this.grpHTMLOutput.Controls.Add(this.WebSiteTargetPathLabel);
         this.grpHTMLOutput.Controls.Add(this.chkWebSiteGenerator);
         this.grpHTMLOutput.Location = new System.Drawing.Point(6, 66);
         this.grpHTMLOutput.Name = "grpHTMLOutput";
         this.grpHTMLOutput.Size = new System.Drawing.Size(489, 227);
         this.grpHTMLOutput.TabIndex = 0;
         this.grpHTMLOutput.TabStop = false;
         this.grpHTMLOutput.Text = "Web Generation";
         // 
         // WebSitePortTextBox
         // 
         this.WebSitePortTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WebSitePortTextBox.DoubleBuffered = true;
         this.WebSitePortTextBox.Enabled = false;
         this.WebSitePortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebSitePortTextBox.ErrorState = false;
         this.WebSitePortTextBox.ErrorToolTip = null;
         this.WebSitePortTextBox.ErrorToolTipDuration = 5000;
         this.WebSitePortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebSitePortTextBox.ErrorToolTipText = "";
         this.WebSitePortTextBox.Location = new System.Drawing.Point(308, 105);
         this.WebSitePortTextBox.Name = "WebSitePortTextBox";
         this.WebSitePortTextBox.ReadOnly = true;
         this.WebSitePortTextBox.Size = new System.Drawing.Size(54, 20);
         this.WebSitePortTextBox.TabIndex = 13;
         this.WebSitePortTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebSitePortLabel
         // 
         this.WebSitePortLabel.AutoSize = true;
         this.WebSitePortLabel.Location = new System.Drawing.Point(273, 108);
         this.WebSitePortLabel.Name = "WebSitePortLabel";
         this.WebSitePortLabel.Size = new System.Drawing.Size(29, 13);
         this.WebSitePortLabel.TabIndex = 12;
         this.WebSitePortLabel.Text = "Port:";
         // 
         // WebSitePasswordTextBox
         // 
         this.WebSitePasswordTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WebSitePasswordTextBox.DoubleBuffered = true;
         this.WebSitePasswordTextBox.Enabled = false;
         this.WebSitePasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebSitePasswordTextBox.ErrorState = false;
         this.WebSitePasswordTextBox.ErrorToolTip = this.toolTipPrefs;
         this.WebSitePasswordTextBox.ErrorToolTipDuration = 5000;
         this.WebSitePasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebSitePasswordTextBox.ErrorToolTipText = "";
         this.WebSitePasswordTextBox.Location = new System.Drawing.Point(79, 159);
         this.WebSitePasswordTextBox.Name = "WebSitePasswordTextBox";
         this.WebSitePasswordTextBox.ReadOnly = true;
         this.WebSitePasswordTextBox.Size = new System.Drawing.Size(187, 20);
         this.WebSitePasswordTextBox.TabIndex = 17;
         this.WebSitePasswordTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebSiteUsernameTextBox
         // 
         this.WebSiteUsernameTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WebSiteUsernameTextBox.DoubleBuffered = true;
         this.WebSiteUsernameTextBox.Enabled = false;
         this.WebSiteUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebSiteUsernameTextBox.ErrorState = false;
         this.WebSiteUsernameTextBox.ErrorToolTip = this.toolTipPrefs;
         this.WebSiteUsernameTextBox.ErrorToolTipDuration = 5000;
         this.WebSiteUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebSiteUsernameTextBox.ErrorToolTipText = "";
         this.WebSiteUsernameTextBox.Location = new System.Drawing.Point(79, 132);
         this.WebSiteUsernameTextBox.Name = "WebSiteUsernameTextBox";
         this.WebSiteUsernameTextBox.ReadOnly = true;
         this.WebSiteUsernameTextBox.Size = new System.Drawing.Size(187, 20);
         this.WebSiteUsernameTextBox.TabIndex = 15;
         this.WebSiteUsernameTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebSiteServerTextBox
         // 
         this.WebSiteServerTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WebSiteServerTextBox.DoubleBuffered = true;
         this.WebSiteServerTextBox.Enabled = false;
         this.WebSiteServerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebSiteServerTextBox.ErrorState = false;
         this.WebSiteServerTextBox.ErrorToolTip = this.toolTipPrefs;
         this.WebSiteServerTextBox.ErrorToolTipDuration = 5000;
         this.WebSiteServerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.WebSiteServerTextBox.ErrorToolTipText = "FTP server must be a valid host name or IP address.";
         this.WebSiteServerTextBox.Location = new System.Drawing.Point(79, 105);
         this.WebSiteServerTextBox.Name = "WebSiteServerTextBox";
         this.WebSiteServerTextBox.ReadOnly = true;
         this.WebSiteServerTextBox.Size = new System.Drawing.Size(187, 20);
         this.WebSiteServerTextBox.TabIndex = 11;
         this.WebSiteServerTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebGenTypePanel
         // 
         this.WebGenTypePanel.Controls.Add(this.UploadTypeLabel);
         this.WebGenTypePanel.Controls.Add(this.WebGenTypeFtpRadioButton);
         this.WebGenTypePanel.Controls.Add(this.WebGenTypePathRadioButton);
         this.WebGenTypePanel.Location = new System.Drawing.Point(3, 46);
         this.WebGenTypePanel.Name = "WebGenTypePanel";
         this.WebGenTypePanel.Size = new System.Drawing.Size(256, 26);
         this.WebGenTypePanel.TabIndex = 6;
         this.WebGenTypePanel.ValueMember = null;
         // 
         // UploadTypeLabel
         // 
         this.UploadTypeLabel.AutoSize = true;
         this.UploadTypeLabel.Location = new System.Drawing.Point(4, 6);
         this.UploadTypeLabel.Name = "UploadTypeLabel";
         this.UploadTypeLabel.Size = new System.Drawing.Size(71, 13);
         this.UploadTypeLabel.TabIndex = 0;
         this.UploadTypeLabel.Text = "Upload Type:";
         // 
         // WebGenTypeFtpRadioButton
         // 
         this.WebGenTypeFtpRadioButton.AutoSize = true;
         this.WebGenTypeFtpRadioButton.Location = new System.Drawing.Point(170, 4);
         this.WebGenTypeFtpRadioButton.Name = "WebGenTypeFtpRadioButton";
         this.WebGenTypeFtpRadioButton.Size = new System.Drawing.Size(79, 17);
         this.WebGenTypeFtpRadioButton.TabIndex = 2;
         this.WebGenTypeFtpRadioButton.TabStop = true;
         this.WebGenTypeFtpRadioButton.Tag = "1";
         this.WebGenTypeFtpRadioButton.Text = "FTP Server";
         this.WebGenTypeFtpRadioButton.UseVisualStyleBackColor = true;
         // 
         // WebGenTypePathRadioButton
         // 
         this.WebGenTypePathRadioButton.AutoSize = true;
         this.WebGenTypePathRadioButton.Location = new System.Drawing.Point(84, 4);
         this.WebGenTypePathRadioButton.Name = "WebGenTypePathRadioButton";
         this.WebGenTypePathRadioButton.Size = new System.Drawing.Size(76, 17);
         this.WebGenTypePathRadioButton.TabIndex = 1;
         this.WebGenTypePathRadioButton.TabStop = true;
         this.WebGenTypePathRadioButton.Tag = "0";
         this.WebGenTypePathRadioButton.Text = "Local Path";
         this.WebGenTypePathRadioButton.UseVisualStyleBackColor = true;
         // 
         // WebSitePasswordLabel
         // 
         this.WebSitePasswordLabel.AutoSize = true;
         this.WebSitePasswordLabel.Location = new System.Drawing.Point(7, 162);
         this.WebSitePasswordLabel.Name = "WebSitePasswordLabel";
         this.WebSitePasswordLabel.Size = new System.Drawing.Size(56, 13);
         this.WebSitePasswordLabel.TabIndex = 16;
         this.WebSitePasswordLabel.Text = "Password:";
         // 
         // WebSiteUsernameLabel
         // 
         this.WebSiteUsernameLabel.AutoSize = true;
         this.WebSiteUsernameLabel.Location = new System.Drawing.Point(7, 135);
         this.WebSiteUsernameLabel.Name = "WebSiteUsernameLabel";
         this.WebSiteUsernameLabel.Size = new System.Drawing.Size(58, 13);
         this.WebSiteUsernameLabel.TabIndex = 14;
         this.WebSiteUsernameLabel.Text = "Username:";
         // 
         // WebSiteServerLabel
         // 
         this.WebSiteServerLabel.AutoSize = true;
         this.WebSiteServerLabel.Location = new System.Drawing.Point(7, 108);
         this.WebSiteServerLabel.Name = "WebSiteServerLabel";
         this.WebSiteServerLabel.Size = new System.Drawing.Size(41, 13);
         this.WebSiteServerLabel.TabIndex = 10;
         this.WebSiteServerLabel.Text = "Server:";
         // 
         // FtpModePanel
         // 
         this.FtpModePanel.Controls.Add(this.radioActive);
         this.FtpModePanel.Controls.Add(this.radioPassive);
         this.FtpModePanel.Controls.Add(this.lblFtpMode);
         this.FtpModePanel.Location = new System.Drawing.Point(4, 191);
         this.FtpModePanel.Name = "FtpModePanel";
         this.FtpModePanel.Size = new System.Drawing.Size(199, 26);
         this.FtpModePanel.TabIndex = 18;
         this.FtpModePanel.ValueMember = null;
         // 
         // radioActive
         // 
         this.radioActive.AutoSize = true;
         this.radioActive.Location = new System.Drawing.Point(138, 4);
         this.radioActive.Name = "radioActive";
         this.radioActive.Size = new System.Drawing.Size(55, 17);
         this.radioActive.TabIndex = 2;
         this.radioActive.Tag = "1";
         this.radioActive.Text = "Active";
         this.radioActive.UseVisualStyleBackColor = true;
         // 
         // radioPassive
         // 
         this.radioPassive.AutoSize = true;
         this.radioPassive.Checked = true;
         this.radioPassive.Location = new System.Drawing.Point(70, 4);
         this.radioPassive.Name = "radioPassive";
         this.radioPassive.Size = new System.Drawing.Size(62, 17);
         this.radioPassive.TabIndex = 1;
         this.radioPassive.TabStop = true;
         this.radioPassive.Tag = "0";
         this.radioPassive.Text = "Passive";
         this.radioPassive.UseVisualStyleBackColor = true;
         // 
         // lblFtpMode
         // 
         this.lblFtpMode.AutoSize = true;
         this.lblFtpMode.Location = new System.Drawing.Point(4, 6);
         this.lblFtpMode.Name = "lblFtpMode";
         this.lblFtpMode.Size = new System.Drawing.Size(60, 13);
         this.lblFtpMode.TabIndex = 0;
         this.lblFtpMode.Text = "FTP Mode:";
         // 
         // udLimitSize
         // 
         this.udLimitSize.Enabled = false;
         this.udLimitSize.Location = new System.Drawing.Point(419, 204);
         this.udLimitSize.Maximum = new decimal(new int[] {
            10240,
            0,
            0,
            0});
         this.udLimitSize.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
         this.udLimitSize.Name = "udLimitSize";
         this.udLimitSize.Size = new System.Drawing.Size(62, 20);
         this.udLimitSize.TabIndex = 23;
         this.udLimitSize.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
         // 
         // chkLimitSize
         // 
         this.chkLimitSize.AutoSize = true;
         this.chkLimitSize.Enabled = false;
         this.chkLimitSize.Location = new System.Drawing.Point(276, 205);
         this.chkLimitSize.Name = "chkLimitSize";
         this.chkLimitSize.Size = new System.Drawing.Size(139, 17);
         this.chkLimitSize.TabIndex = 22;
         this.chkLimitSize.Text = "Limit log file size to (KB):";
         this.chkLimitSize.UseVisualStyleBackColor = true;
         // 
         // chkXml
         // 
         this.chkXml.AutoSize = true;
         this.chkXml.Enabled = false;
         this.chkXml.Location = new System.Drawing.Point(276, 159);
         this.chkXml.Name = "chkXml";
         this.chkXml.Size = new System.Drawing.Size(121, 17);
         this.chkXml.TabIndex = 20;
         this.chkXml.Text = "Copy XML to Target";
         this.chkXml.UseVisualStyleBackColor = true;
         // 
         // chkHtml
         // 
         this.chkHtml.AutoSize = true;
         this.chkHtml.Enabled = false;
         this.chkHtml.Location = new System.Drawing.Point(276, 136);
         this.chkHtml.Name = "chkHtml";
         this.chkHtml.Size = new System.Drawing.Size(129, 17);
         this.chkHtml.TabIndex = 19;
         this.chkHtml.Text = "Copy HTML to Target";
         this.chkHtml.UseVisualStyleBackColor = true;
         // 
         // TestConnectionButton
         // 
         this.TestConnectionButton.Enabled = false;
         this.TestConnectionButton.Location = new System.Drawing.Point(383, 15);
         this.TestConnectionButton.Name = "TestConnectionButton";
         this.TestConnectionButton.Size = new System.Drawing.Size(100, 24);
         this.TestConnectionButton.TabIndex = 5;
         this.TestConnectionButton.Text = "Test Connection";
         this.TestConnectionButton.UseVisualStyleBackColor = true;
         this.TestConnectionButton.Click += new System.EventHandler(this.TestConnectionButtonClick);
         // 
         // chkFAHlog
         // 
         this.chkFAHlog.AutoSize = true;
         this.chkFAHlog.Enabled = false;
         this.chkFAHlog.Location = new System.Drawing.Point(276, 182);
         this.chkFAHlog.Name = "chkFAHlog";
         this.chkFAHlog.Size = new System.Drawing.Size(148, 17);
         this.chkFAHlog.TabIndex = 21;
         this.chkFAHlog.Text = "Copy FAHlog.txt to Target";
         this.chkFAHlog.UseVisualStyleBackColor = true;
         // 
         // radioFullRefresh
         // 
         this.radioFullRefresh.AutoSize = true;
         this.radioFullRefresh.Enabled = false;
         this.radioFullRefresh.Location = new System.Drawing.Point(272, 19);
         this.radioFullRefresh.Name = "radioFullRefresh";
         this.radioFullRefresh.Size = new System.Drawing.Size(106, 17);
         this.radioFullRefresh.TabIndex = 4;
         this.radioFullRefresh.TabStop = true;
         this.radioFullRefresh.Text = "After Full Refresh";
         this.radioFullRefresh.UseVisualStyleBackColor = true;
         // 
         // radioSchedule
         // 
         this.radioSchedule.AutoSize = true;
         this.radioSchedule.Checked = true;
         this.radioSchedule.Enabled = false;
         this.radioSchedule.Location = new System.Drawing.Point(129, 19);
         this.radioSchedule.Name = "radioSchedule";
         this.radioSchedule.Size = new System.Drawing.Size(52, 17);
         this.radioSchedule.TabIndex = 1;
         this.radioSchedule.TabStop = true;
         this.radioSchedule.Text = "Every";
         this.radioSchedule.UseVisualStyleBackColor = true;
         // 
         // txtWebGenMinutes
         // 
         this.txtWebGenMinutes.BackColor = System.Drawing.SystemColors.Control;
         this.txtWebGenMinutes.DoubleBuffered = true;
         this.txtWebGenMinutes.Enabled = false;
         this.txtWebGenMinutes.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtWebGenMinutes.ErrorState = false;
         this.txtWebGenMinutes.ErrorToolTip = this.toolTipPrefs;
         this.txtWebGenMinutes.ErrorToolTipDuration = 5000;
         this.txtWebGenMinutes.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtWebGenMinutes.ErrorToolTipText = "";
         this.txtWebGenMinutes.Location = new System.Drawing.Point(181, 18);
         this.txtWebGenMinutes.MaxLength = 3;
         this.txtWebGenMinutes.Name = "txtWebGenMinutes";
         this.txtWebGenMinutes.ReadOnly = true;
         this.txtWebGenMinutes.Size = new System.Drawing.Size(39, 20);
         this.txtWebGenMinutes.TabIndex = 2;
         this.txtWebGenMinutes.Text = "15";
         this.txtWebGenMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         this.txtWebGenMinutes.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtWebGenMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // lbl2MinutesToGen
         // 
         this.lbl2MinutesToGen.AutoSize = true;
         this.lbl2MinutesToGen.Enabled = false;
         this.lbl2MinutesToGen.Location = new System.Drawing.Point(222, 21);
         this.lbl2MinutesToGen.Name = "lbl2MinutesToGen";
         this.lbl2MinutesToGen.Size = new System.Drawing.Size(44, 13);
         this.lbl2MinutesToGen.TabIndex = 3;
         this.lbl2MinutesToGen.Text = "Minutes";
         // 
         // btnBrowseWebFolder
         // 
         this.btnBrowseWebFolder.Enabled = false;
         this.btnBrowseWebFolder.Location = new System.Drawing.Point(459, 76);
         this.btnBrowseWebFolder.Name = "btnBrowseWebFolder";
         this.btnBrowseWebFolder.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseWebFolder.TabIndex = 9;
         this.btnBrowseWebFolder.Text = "...";
         this.btnBrowseWebFolder.UseVisualStyleBackColor = true;
         this.btnBrowseWebFolder.Click += new System.EventHandler(this.btnBrowseWebFolder_Click);
         // 
         // WebSiteTargetPathTextBox
         // 
         this.WebSiteTargetPathTextBox.BackColor = System.Drawing.SystemColors.Control;
         this.WebSiteTargetPathTextBox.DoubleBuffered = true;
         this.WebSiteTargetPathTextBox.Enabled = false;
         this.WebSiteTargetPathTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
         this.WebSiteTargetPathTextBox.ErrorState = false;
         this.WebSiteTargetPathTextBox.ErrorToolTip = this.toolTipPrefs;
         this.WebSiteTargetPathTextBox.ErrorToolTipDuration = 5000;
         this.WebSiteTargetPathTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
         this.WebSiteTargetPathTextBox.ErrorToolTipText = "Web Generation target path must be a valid local path, network (UNC) path,\r\nor Un" +
    "ix style path when the upload type is FTP server.";
         this.WebSiteTargetPathTextBox.Location = new System.Drawing.Point(79, 78);
         this.WebSiteTargetPathTextBox.Name = "WebSiteTargetPathTextBox";
         this.WebSiteTargetPathTextBox.ReadOnly = true;
         this.WebSiteTargetPathTextBox.Size = new System.Drawing.Size(374, 20);
         this.WebSiteTargetPathTextBox.TabIndex = 8;
         this.WebSiteTargetPathTextBox.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // WebSiteTargetPathLabel
         // 
         this.WebSiteTargetPathLabel.AutoSize = true;
         this.WebSiteTargetPathLabel.Location = new System.Drawing.Point(7, 81);
         this.WebSiteTargetPathLabel.Name = "WebSiteTargetPathLabel";
         this.WebSiteTargetPathLabel.Size = new System.Drawing.Size(66, 13);
         this.WebSiteTargetPathLabel.TabIndex = 7;
         this.WebSiteTargetPathLabel.Text = "Target Path:";
         // 
         // chkWebSiteGenerator
         // 
         this.chkWebSiteGenerator.AutoSize = true;
         this.chkWebSiteGenerator.Location = new System.Drawing.Point(10, 20);
         this.chkWebSiteGenerator.Name = "chkWebSiteGenerator";
         this.chkWebSiteGenerator.Size = new System.Drawing.Size(113, 17);
         this.chkWebSiteGenerator.TabIndex = 0;
         this.chkWebSiteGenerator.Text = "Create a Web Site";
         this.chkWebSiteGenerator.UseVisualStyleBackColor = true;
         // 
         // tabStartup
         // 
         this.tabStartup.Controls.Add(this.grpFileExplorer);
         this.tabStartup.Controls.Add(this.grpLogFileViewer);
         this.tabStartup.Controls.Add(this.grpDefaultConfig);
         this.tabStartup.Controls.Add(this.grpStartup);
         this.tabStartup.Location = new System.Drawing.Point(4, 22);
         this.tabStartup.Name = "tabStartup";
         this.tabStartup.Padding = new System.Windows.Forms.Padding(3);
         this.tabStartup.Size = new System.Drawing.Size(501, 303);
         this.tabStartup.TabIndex = 6;
         this.tabStartup.Text = "Startup & External";
         this.tabStartup.UseVisualStyleBackColor = true;
         // 
         // grpFileExplorer
         // 
         this.grpFileExplorer.Controls.Add(this.btnBrowseFileExplorer);
         this.grpFileExplorer.Controls.Add(this.label4);
         this.grpFileExplorer.Controls.Add(this.txtFileExplorer);
         this.grpFileExplorer.Location = new System.Drawing.Point(6, 239);
         this.grpFileExplorer.Name = "grpFileExplorer";
         this.grpFileExplorer.Size = new System.Drawing.Size(489, 54);
         this.grpFileExplorer.TabIndex = 6;
         this.grpFileExplorer.TabStop = false;
         this.grpFileExplorer.Text = "External File Explorer";
         // 
         // btnBrowseFileExplorer
         // 
         this.btnBrowseFileExplorer.Location = new System.Drawing.Point(456, 19);
         this.btnBrowseFileExplorer.Name = "btnBrowseFileExplorer";
         this.btnBrowseFileExplorer.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseFileExplorer.TabIndex = 2;
         this.btnBrowseFileExplorer.Text = "...";
         this.btnBrowseFileExplorer.UseVisualStyleBackColor = true;
         this.btnBrowseFileExplorer.Click += new System.EventHandler(this.btnBrowseFileExplorer_Click);
         // 
         // label4
         // 
         this.label4.AutoSize = true;
         this.label4.Location = new System.Drawing.Point(8, 24);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(52, 13);
         this.label4.TabIndex = 0;
         this.label4.Text = "Filename:";
         // 
         // txtFileExplorer
         // 
         this.txtFileExplorer.BackColor = System.Drawing.SystemColors.Window;
         this.txtFileExplorer.DoubleBuffered = true;
         this.txtFileExplorer.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFileExplorer.ErrorState = false;
         this.txtFileExplorer.ErrorToolTip = null;
         this.txtFileExplorer.ErrorToolTipDuration = 5000;
         this.txtFileExplorer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFileExplorer.ErrorToolTipText = "";
         this.txtFileExplorer.Location = new System.Drawing.Point(66, 21);
         this.txtFileExplorer.Name = "txtFileExplorer";
         this.txtFileExplorer.Size = new System.Drawing.Size(384, 20);
         this.txtFileExplorer.TabIndex = 1;
         this.txtFileExplorer.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // grpLogFileViewer
         // 
         this.grpLogFileViewer.Controls.Add(this.btnBrowseLogViewer);
         this.grpLogFileViewer.Controls.Add(this.label3);
         this.grpLogFileViewer.Controls.Add(this.txtLogFileViewer);
         this.grpLogFileViewer.Location = new System.Drawing.Point(6, 181);
         this.grpLogFileViewer.Name = "grpLogFileViewer";
         this.grpLogFileViewer.Size = new System.Drawing.Size(489, 54);
         this.grpLogFileViewer.TabIndex = 5;
         this.grpLogFileViewer.TabStop = false;
         this.grpLogFileViewer.Text = "External Log File Viewer";
         // 
         // btnBrowseLogViewer
         // 
         this.btnBrowseLogViewer.Location = new System.Drawing.Point(456, 19);
         this.btnBrowseLogViewer.Name = "btnBrowseLogViewer";
         this.btnBrowseLogViewer.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseLogViewer.TabIndex = 2;
         this.btnBrowseLogViewer.Text = "...";
         this.btnBrowseLogViewer.UseVisualStyleBackColor = true;
         this.btnBrowseLogViewer.Click += new System.EventHandler(this.btnBrowseLogViewer_Click);
         // 
         // label3
         // 
         this.label3.AutoSize = true;
         this.label3.Location = new System.Drawing.Point(8, 24);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(52, 13);
         this.label3.TabIndex = 0;
         this.label3.Text = "Filename:";
         // 
         // txtLogFileViewer
         // 
         this.txtLogFileViewer.BackColor = System.Drawing.SystemColors.Window;
         this.txtLogFileViewer.DoubleBuffered = true;
         this.txtLogFileViewer.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtLogFileViewer.ErrorState = false;
         this.txtLogFileViewer.ErrorToolTip = null;
         this.txtLogFileViewer.ErrorToolTipDuration = 5000;
         this.txtLogFileViewer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtLogFileViewer.ErrorToolTipText = "";
         this.txtLogFileViewer.Location = new System.Drawing.Point(66, 21);
         this.txtLogFileViewer.Name = "txtLogFileViewer";
         this.txtLogFileViewer.Size = new System.Drawing.Size(384, 20);
         this.txtLogFileViewer.TabIndex = 1;
         this.txtLogFileViewer.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // grpDefaultConfig
         // 
         this.grpDefaultConfig.Controls.Add(this.chkDefaultConfig);
         this.grpDefaultConfig.Controls.Add(this.btnBrowseConfigFile);
         this.grpDefaultConfig.Controls.Add(this.txtDefaultConfigFile);
         this.grpDefaultConfig.Controls.Add(this.label1);
         this.grpDefaultConfig.Location = new System.Drawing.Point(6, 65);
         this.grpDefaultConfig.Name = "grpDefaultConfig";
         this.grpDefaultConfig.Size = new System.Drawing.Size(489, 86);
         this.grpDefaultConfig.TabIndex = 4;
         this.grpDefaultConfig.TabStop = false;
         this.grpDefaultConfig.Text = "Configuration File";
         // 
         // chkDefaultConfig
         // 
         this.chkDefaultConfig.AutoSize = true;
         this.chkDefaultConfig.Location = new System.Drawing.Point(10, 22);
         this.chkDefaultConfig.Name = "chkDefaultConfig";
         this.chkDefaultConfig.Size = new System.Drawing.Size(134, 17);
         this.chkDefaultConfig.TabIndex = 0;
         this.chkDefaultConfig.Text = "Load Configuration File";
         this.chkDefaultConfig.UseVisualStyleBackColor = true;
         // 
         // btnBrowseConfigFile
         // 
         this.btnBrowseConfigFile.Enabled = false;
         this.btnBrowseConfigFile.Location = new System.Drawing.Point(456, 47);
         this.btnBrowseConfigFile.Name = "btnBrowseConfigFile";
         this.btnBrowseConfigFile.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseConfigFile.TabIndex = 3;
         this.btnBrowseConfigFile.Text = "...";
         this.btnBrowseConfigFile.UseVisualStyleBackColor = true;
         this.btnBrowseConfigFile.Click += new System.EventHandler(this.btnBrowseConfigFile_Click);
         // 
         // txtDefaultConfigFile
         // 
         this.txtDefaultConfigFile.BackColor = System.Drawing.SystemColors.Control;
         this.txtDefaultConfigFile.DoubleBuffered = true;
         this.txtDefaultConfigFile.Enabled = false;
         this.txtDefaultConfigFile.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtDefaultConfigFile.ErrorState = false;
         this.txtDefaultConfigFile.ErrorToolTip = null;
         this.txtDefaultConfigFile.ErrorToolTipDuration = 5000;
         this.txtDefaultConfigFile.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtDefaultConfigFile.ErrorToolTipText = "";
         this.txtDefaultConfigFile.Location = new System.Drawing.Point(66, 49);
         this.txtDefaultConfigFile.Name = "txtDefaultConfigFile";
         this.txtDefaultConfigFile.ReadOnly = true;
         this.txtDefaultConfigFile.Size = new System.Drawing.Size(384, 20);
         this.txtDefaultConfigFile.TabIndex = 2;
         this.txtDefaultConfigFile.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(8, 52);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(52, 13);
         this.label1.TabIndex = 1;
         this.label1.Text = "Filename:";
         // 
         // grpStartup
         // 
         this.grpStartup.Controls.Add(this.chkCheckForUpdate);
         this.grpStartup.Controls.Add(this.chkAutoRun);
         this.grpStartup.Controls.Add(this.chkRunMinimized);
         this.grpStartup.Location = new System.Drawing.Point(6, 9);
         this.grpStartup.Name = "grpStartup";
         this.grpStartup.Size = new System.Drawing.Size(489, 50);
         this.grpStartup.TabIndex = 3;
         this.grpStartup.TabStop = false;
         this.grpStartup.Text = "Startup";
         // 
         // chkCheckForUpdate
         // 
         this.chkCheckForUpdate.AutoSize = true;
         this.chkCheckForUpdate.Location = new System.Drawing.Point(310, 20);
         this.chkCheckForUpdate.Name = "chkCheckForUpdate";
         this.chkCheckForUpdate.Size = new System.Drawing.Size(115, 17);
         this.chkCheckForUpdate.TabIndex = 2;
         this.chkCheckForUpdate.Text = "Check for Updates";
         this.chkCheckForUpdate.UseVisualStyleBackColor = true;
         // 
         // chkAutoRun
         // 
         this.chkAutoRun.AutoSize = true;
         this.chkAutoRun.Location = new System.Drawing.Point(10, 20);
         this.chkAutoRun.Name = "chkAutoRun";
         this.chkAutoRun.Size = new System.Drawing.Size(170, 17);
         this.chkAutoRun.TabIndex = 0;
         this.chkAutoRun.Text = "Auto Run on Windows Startup";
         this.chkAutoRun.UseVisualStyleBackColor = true;
         // 
         // chkRunMinimized
         // 
         this.chkRunMinimized.AutoSize = true;
         this.chkRunMinimized.Location = new System.Drawing.Point(196, 20);
         this.chkRunMinimized.Name = "chkRunMinimized";
         this.chkRunMinimized.Size = new System.Drawing.Size(95, 17);
         this.chkRunMinimized.TabIndex = 1;
         this.chkRunMinimized.Text = "Run Minimized";
         this.chkRunMinimized.UseVisualStyleBackColor = true;
         // 
         // tabOptions
         // 
         this.tabOptions.Controls.Add(this.grpShowStyle);
         this.tabOptions.Controls.Add(this.grpInteractiveOptions);
         this.tabOptions.Controls.Add(this.grpDebugMessageLevel);
         this.tabOptions.Location = new System.Drawing.Point(4, 22);
         this.tabOptions.Name = "tabOptions";
         this.tabOptions.Size = new System.Drawing.Size(501, 303);
         this.tabOptions.TabIndex = 4;
         this.tabOptions.Text = "Options";
         this.tabOptions.UseVisualStyleBackColor = true;
         // 
         // grpShowStyle
         // 
         this.grpShowStyle.Controls.Add(this.cboShowStyle);
         this.grpShowStyle.Controls.Add(this.labelWrapper2);
         this.grpShowStyle.Location = new System.Drawing.Point(254, 239);
         this.grpShowStyle.Name = "grpShowStyle";
         this.grpShowStyle.Size = new System.Drawing.Size(241, 54);
         this.grpShowStyle.TabIndex = 4;
         this.grpShowStyle.TabStop = false;
         this.grpShowStyle.Text = "Docking Style";
         // 
         // cboShowStyle
         // 
         this.cboShowStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboShowStyle.FormattingEnabled = true;
         this.cboShowStyle.Location = new System.Drawing.Point(136, 19);
         this.cboShowStyle.Name = "cboShowStyle";
         this.cboShowStyle.Size = new System.Drawing.Size(89, 21);
         this.cboShowStyle.TabIndex = 1;
         // 
         // labelWrapper2
         // 
         this.labelWrapper2.AutoSize = true;
         this.labelWrapper2.Location = new System.Drawing.Point(13, 24);
         this.labelWrapper2.Name = "labelWrapper2";
         this.labelWrapper2.Size = new System.Drawing.Size(117, 13);
         this.labelWrapper2.TabIndex = 0;
         this.labelWrapper2.Text = "Show HFM.NET in the:";
         // 
         // grpInteractiveOptions
         // 
         this.grpInteractiveOptions.Controls.Add(this.ShowUserStatsCheckBox);
         this.grpInteractiveOptions.Controls.Add(this.DuplicateUserCheckBox);
         this.grpInteractiveOptions.Controls.Add(this.DuplicateProjectCheckBox);
         this.grpInteractiveOptions.Controls.Add(this.chkEtaAsDate);
         this.grpInteractiveOptions.Controls.Add(this.chkCalcBonus);
         this.grpInteractiveOptions.Controls.Add(this.label2);
         this.grpInteractiveOptions.Controls.Add(this.cboPpdCalc);
         this.grpInteractiveOptions.Controls.Add(this.chkOffline);
         this.grpInteractiveOptions.Controls.Add(this.chkColorLog);
         this.grpInteractiveOptions.Controls.Add(this.udDecimalPlaces);
         this.grpInteractiveOptions.Controls.Add(this.chkAutoSave);
         this.grpInteractiveOptions.Controls.Add(this.labelWrapper1);
         this.grpInteractiveOptions.Location = new System.Drawing.Point(6, 9);
         this.grpInteractiveOptions.Name = "grpInteractiveOptions";
         this.grpInteractiveOptions.Size = new System.Drawing.Size(489, 151);
         this.grpInteractiveOptions.TabIndex = 0;
         this.grpInteractiveOptions.TabStop = false;
         this.grpInteractiveOptions.Text = "Interactive Options";
         // 
         // ShowUserStatsCheckBox
         // 
         this.ShowUserStatsCheckBox.AutoSize = true;
         this.ShowUserStatsCheckBox.Location = new System.Drawing.Point(256, 124);
         this.ShowUserStatsCheckBox.Name = "ShowUserStatsCheckBox";
         this.ShowUserStatsCheckBox.Size = new System.Drawing.Size(194, 17);
         this.ShowUserStatsCheckBox.TabIndex = 12;
         this.ShowUserStatsCheckBox.Text = "Retrieve and Show EOC User Stats";
         this.ShowUserStatsCheckBox.UseVisualStyleBackColor = true;
         // 
         // DuplicateUserCheckBox
         // 
         this.DuplicateUserCheckBox.AutoSize = true;
         this.DuplicateUserCheckBox.Location = new System.Drawing.Point(10, 124);
         this.DuplicateUserCheckBox.Name = "DuplicateUserCheckBox";
         this.DuplicateUserCheckBox.Size = new System.Drawing.Size(190, 17);
         this.DuplicateUserCheckBox.TabIndex = 11;
         this.DuplicateUserCheckBox.Text = "Duplicate User/Machine ID Check";
         this.DuplicateUserCheckBox.UseVisualStyleBackColor = true;
         // 
         // DuplicateProjectCheckBox
         // 
         this.DuplicateProjectCheckBox.AutoSize = true;
         this.DuplicateProjectCheckBox.Location = new System.Drawing.Point(10, 98);
         this.DuplicateProjectCheckBox.Name = "DuplicateProjectCheckBox";
         this.DuplicateProjectCheckBox.Size = new System.Drawing.Size(183, 17);
         this.DuplicateProjectCheckBox.TabIndex = 10;
         this.DuplicateProjectCheckBox.Text = "Duplicate Project (R/C/G) Check";
         this.DuplicateProjectCheckBox.UseVisualStyleBackColor = true;
         // 
         // chkEtaAsDate
         // 
         this.chkEtaAsDate.AutoSize = true;
         this.chkEtaAsDate.Location = new System.Drawing.Point(256, 98);
         this.chkEtaAsDate.Name = "chkEtaAsDate";
         this.chkEtaAsDate.Size = new System.Drawing.Size(202, 17);
         this.chkEtaAsDate.TabIndex = 9;
         this.chkEtaAsDate.Text = "Show ETA value as a Date and Time";
         this.chkEtaAsDate.UseVisualStyleBackColor = true;
         // 
         // chkCalcBonus
         // 
         this.chkCalcBonus.AutoSize = true;
         this.chkCalcBonus.Location = new System.Drawing.Point(256, 72);
         this.chkCalcBonus.Name = "chkCalcBonus";
         this.chkCalcBonus.Size = new System.Drawing.Size(179, 17);
         this.chkCalcBonus.TabIndex = 8;
         this.chkCalcBonus.Text = "Calculate Bonus Credit and PPD";
         this.chkCalcBonus.UseVisualStyleBackColor = true;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(235, 21);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(126, 13);
         this.label2.TabIndex = 4;
         this.label2.Text = "Calculate PPD based on:";
         // 
         // cboPpdCalc
         // 
         this.cboPpdCalc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboPpdCalc.FormattingEnabled = true;
         this.cboPpdCalc.Location = new System.Drawing.Point(367, 16);
         this.cboPpdCalc.Name = "cboPpdCalc";
         this.cboPpdCalc.Size = new System.Drawing.Size(113, 21);
         this.cboPpdCalc.TabIndex = 5;
         // 
         // chkOffline
         // 
         this.chkOffline.AutoSize = true;
         this.chkOffline.Location = new System.Drawing.Point(10, 20);
         this.chkOffline.Name = "chkOffline";
         this.chkOffline.Size = new System.Drawing.Size(132, 17);
         this.chkOffline.TabIndex = 0;
         this.chkOffline.Text = "List Offline Clients Last";
         this.chkOffline.UseVisualStyleBackColor = true;
         // 
         // chkColorLog
         // 
         this.chkColorLog.AutoSize = true;
         this.chkColorLog.Location = new System.Drawing.Point(10, 46);
         this.chkColorLog.Name = "chkColorLog";
         this.chkColorLog.Size = new System.Drawing.Size(148, 17);
         this.chkColorLog.TabIndex = 1;
         this.chkColorLog.Text = "Color the Log Viewer Text";
         this.chkColorLog.UseVisualStyleBackColor = true;
         // 
         // udDecimalPlaces
         // 
         this.udDecimalPlaces.Location = new System.Drawing.Point(367, 43);
         this.udDecimalPlaces.Name = "udDecimalPlaces";
         this.udDecimalPlaces.Size = new System.Drawing.Size(39, 20);
         this.udDecimalPlaces.TabIndex = 7;
         // 
         // chkAutoSave
         // 
         this.chkAutoSave.AutoSize = true;
         this.chkAutoSave.Location = new System.Drawing.Point(10, 72);
         this.chkAutoSave.Name = "chkAutoSave";
         this.chkAutoSave.Size = new System.Drawing.Size(216, 17);
         this.chkAutoSave.TabIndex = 2;
         this.chkAutoSave.Text = "Auto Save Configuration when Changed";
         this.chkAutoSave.UseVisualStyleBackColor = true;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.AutoSize = true;
         this.labelWrapper1.Location = new System.Drawing.Point(253, 47);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(108, 13);
         this.labelWrapper1.TabIndex = 6;
         this.labelWrapper1.Text = "PPD Decimal Places:";
         // 
         // grpDebugMessageLevel
         // 
         this.grpDebugMessageLevel.Controls.Add(this.cboMessageLevel);
         this.grpDebugMessageLevel.Controls.Add(this.label6);
         this.grpDebugMessageLevel.Location = new System.Drawing.Point(6, 239);
         this.grpDebugMessageLevel.Name = "grpDebugMessageLevel";
         this.grpDebugMessageLevel.Size = new System.Drawing.Size(241, 54);
         this.grpDebugMessageLevel.TabIndex = 3;
         this.grpDebugMessageLevel.TabStop = false;
         this.grpDebugMessageLevel.Text = "Messages";
         // 
         // cboMessageLevel
         // 
         this.cboMessageLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboMessageLevel.FormattingEnabled = true;
         this.cboMessageLevel.Location = new System.Drawing.Point(139, 21);
         this.cboMessageLevel.Name = "cboMessageLevel";
         this.cboMessageLevel.Size = new System.Drawing.Size(75, 21);
         this.cboMessageLevel.TabIndex = 1;
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(16, 24);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(117, 13);
         this.label6.TabIndex = 0;
         this.label6.Text = "Debug Message Level:";
         // 
         // tabReporting
         // 
         this.tabReporting.Controls.Add(this.grpReportSelections);
         this.tabReporting.Controls.Add(this.grpEmailSettings);
         this.tabReporting.Location = new System.Drawing.Point(4, 22);
         this.tabReporting.Name = "tabReporting";
         this.tabReporting.Size = new System.Drawing.Size(501, 303);
         this.tabReporting.TabIndex = 5;
         this.tabReporting.Text = "Reporting";
         this.tabReporting.UseVisualStyleBackColor = true;
         // 
         // grpReportSelections
         // 
         this.grpReportSelections.Controls.Add(this.chkClientHung);
         this.grpReportSelections.Controls.Add(this.chkClientEuePause);
         this.grpReportSelections.Enabled = false;
         this.grpReportSelections.Location = new System.Drawing.Point(6, 179);
         this.grpReportSelections.Name = "grpReportSelections";
         this.grpReportSelections.Size = new System.Drawing.Size(489, 114);
         this.grpReportSelections.TabIndex = 1;
         this.grpReportSelections.TabStop = false;
         this.grpReportSelections.Text = "Report Selections (Legacy Clients Only)";
         this.grpReportSelections.EnabledChanged += new System.EventHandler(this.grpReportSelections_EnabledChanged);
         // 
         // chkClientHung
         // 
         this.chkClientHung.AutoSize = true;
         this.chkClientHung.Enabled = false;
         this.chkClientHung.Location = new System.Drawing.Point(10, 43);
         this.chkClientHung.Name = "chkClientHung";
         this.chkClientHung.Size = new System.Drawing.Size(137, 17);
         this.chkClientHung.TabIndex = 1;
         this.chkClientHung.Text = "Client Hung Notification";
         this.chkClientHung.UseVisualStyleBackColor = true;
         // 
         // chkClientEuePause
         // 
         this.chkClientEuePause.AutoSize = true;
         this.chkClientEuePause.Enabled = false;
         this.chkClientEuePause.Location = new System.Drawing.Point(10, 20);
         this.chkClientEuePause.Name = "chkClientEuePause";
         this.chkClientEuePause.Size = new System.Drawing.Size(166, 17);
         this.chkClientEuePause.TabIndex = 0;
         this.chkClientEuePause.Text = "Client EUE Pause Notification";
         this.chkClientEuePause.UseVisualStyleBackColor = true;
         // 
         // grpEmailSettings
         // 
         this.grpEmailSettings.Controls.Add(this.txtSmtpServerPort);
         this.grpEmailSettings.Controls.Add(this.labelWrapper3);
         this.grpEmailSettings.Controls.Add(this.chkEmailSecure);
         this.grpEmailSettings.Controls.Add(this.btnTestEmail);
         this.grpEmailSettings.Controls.Add(this.txtSmtpPassword);
         this.grpEmailSettings.Controls.Add(this.txtSmtpUsername);
         this.grpEmailSettings.Controls.Add(this.labelWrapper4);
         this.grpEmailSettings.Controls.Add(this.labelWrapper5);
         this.grpEmailSettings.Controls.Add(this.lblFromEmailAddress);
         this.grpEmailSettings.Controls.Add(this.txtFromEmailAddress);
         this.grpEmailSettings.Controls.Add(this.chkEnableEmail);
         this.grpEmailSettings.Controls.Add(this.lblSmtpServer);
         this.grpEmailSettings.Controls.Add(this.lblToAddress);
         this.grpEmailSettings.Controls.Add(this.txtSmtpServer);
         this.grpEmailSettings.Controls.Add(this.txtToEmailAddress);
         this.grpEmailSettings.Location = new System.Drawing.Point(6, 9);
         this.grpEmailSettings.Name = "grpEmailSettings";
         this.grpEmailSettings.Size = new System.Drawing.Size(489, 164);
         this.grpEmailSettings.TabIndex = 0;
         this.grpEmailSettings.TabStop = false;
         this.grpEmailSettings.Text = "Email Settings";
         // 
         // txtSmtpServerPort
         // 
         this.txtSmtpServerPort.BackColor = System.Drawing.SystemColors.Control;
         this.txtSmtpServerPort.DoubleBuffered = true;
         this.txtSmtpServerPort.Enabled = false;
         this.txtSmtpServerPort.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtSmtpServerPort.ErrorState = false;
         this.txtSmtpServerPort.ErrorToolTip = this.toolTipPrefs;
         this.txtSmtpServerPort.ErrorToolTipDuration = 5000;
         this.txtSmtpServerPort.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtSmtpServerPort.ErrorToolTipText = "";
         this.txtSmtpServerPort.Location = new System.Drawing.Point(415, 103);
         this.txtSmtpServerPort.MaxLength = 200;
         this.txtSmtpServerPort.Name = "txtSmtpServerPort";
         this.txtSmtpServerPort.ReadOnly = true;
         this.txtSmtpServerPort.Size = new System.Drawing.Size(54, 20);
         this.txtSmtpServerPort.TabIndex = 9;
         this.txtSmtpServerPort.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtSmtpServerPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // labelWrapper3
         // 
         this.labelWrapper3.AutoSize = true;
         this.labelWrapper3.Location = new System.Drawing.Point(380, 106);
         this.labelWrapper3.Name = "labelWrapper3";
         this.labelWrapper3.Size = new System.Drawing.Size(29, 13);
         this.labelWrapper3.TabIndex = 8;
         this.labelWrapper3.Text = "Port:";
         // 
         // chkEmailSecure
         // 
         this.chkEmailSecure.AutoSize = true;
         this.chkEmailSecure.Enabled = false;
         this.chkEmailSecure.Location = new System.Drawing.Point(152, 20);
         this.chkEmailSecure.Name = "chkEmailSecure";
         this.chkEmailSecure.Size = new System.Drawing.Size(168, 17);
         this.chkEmailSecure.TabIndex = 1;
         this.chkEmailSecure.Text = "Use Secure Connection (SSL)";
         this.chkEmailSecure.UseVisualStyleBackColor = true;
         // 
         // btnTestEmail
         // 
         this.btnTestEmail.Enabled = false;
         this.btnTestEmail.Location = new System.Drawing.Point(350, 15);
         this.btnTestEmail.Name = "btnTestEmail";
         this.btnTestEmail.Size = new System.Drawing.Size(119, 26);
         this.btnTestEmail.TabIndex = 14;
         this.btnTestEmail.Text = "Send Test Email";
         this.btnTestEmail.UseVisualStyleBackColor = true;
         this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
         // 
         // txtSmtpPassword
         // 
         this.txtSmtpPassword.BackColor = System.Drawing.SystemColors.Control;
         this.txtSmtpPassword.DoubleBuffered = true;
         this.txtSmtpPassword.Enabled = false;
         this.txtSmtpPassword.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtSmtpPassword.ErrorState = false;
         this.txtSmtpPassword.ErrorToolTip = this.toolTipPrefs;
         this.txtSmtpPassword.ErrorToolTipDuration = 5000;
         this.txtSmtpPassword.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtSmtpPassword.ErrorToolTipText = "";
         this.txtSmtpPassword.Location = new System.Drawing.Point(314, 129);
         this.txtSmtpPassword.MaxLength = 100;
         this.txtSmtpPassword.Name = "txtSmtpPassword";
         this.txtSmtpPassword.ReadOnly = true;
         this.txtSmtpPassword.Size = new System.Drawing.Size(155, 20);
         this.txtSmtpPassword.TabIndex = 13;
         this.txtSmtpPassword.UseSystemPasswordChar = true;
         this.txtSmtpPassword.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // txtSmtpUsername
         // 
         this.txtSmtpUsername.BackColor = System.Drawing.SystemColors.Control;
         this.txtSmtpUsername.DoubleBuffered = true;
         this.txtSmtpUsername.Enabled = false;
         this.txtSmtpUsername.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtSmtpUsername.ErrorState = false;
         this.txtSmtpUsername.ErrorToolTip = this.toolTipPrefs;
         this.txtSmtpUsername.ErrorToolTipDuration = 5000;
         this.txtSmtpUsername.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtSmtpUsername.ErrorToolTipText = "";
         this.txtSmtpUsername.Location = new System.Drawing.Point(92, 129);
         this.txtSmtpUsername.MaxLength = 100;
         this.txtSmtpUsername.Name = "txtSmtpUsername";
         this.txtSmtpUsername.ReadOnly = true;
         this.txtSmtpUsername.Size = new System.Drawing.Size(155, 20);
         this.txtSmtpUsername.TabIndex = 11;
         this.txtSmtpUsername.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // labelWrapper4
         // 
         this.labelWrapper4.AutoSize = true;
         this.labelWrapper4.Location = new System.Drawing.Point(253, 132);
         this.labelWrapper4.Name = "labelWrapper4";
         this.labelWrapper4.Size = new System.Drawing.Size(56, 13);
         this.labelWrapper4.TabIndex = 12;
         this.labelWrapper4.Text = "Password:";
         // 
         // labelWrapper5
         // 
         this.labelWrapper5.AutoSize = true;
         this.labelWrapper5.Location = new System.Drawing.Point(27, 132);
         this.labelWrapper5.Name = "labelWrapper5";
         this.labelWrapper5.Size = new System.Drawing.Size(58, 13);
         this.labelWrapper5.TabIndex = 10;
         this.labelWrapper5.Text = "Username:";
         // 
         // lblFromEmailAddress
         // 
         this.lblFromEmailAddress.AutoSize = true;
         this.lblFromEmailAddress.Location = new System.Drawing.Point(11, 80);
         this.lblFromEmailAddress.Name = "lblFromEmailAddress";
         this.lblFromEmailAddress.Size = new System.Drawing.Size(74, 13);
         this.lblFromEmailAddress.TabIndex = 4;
         this.lblFromEmailAddress.Text = "From Address:";
         // 
         // txtFromEmailAddress
         // 
         this.txtFromEmailAddress.BackColor = System.Drawing.SystemColors.Control;
         this.txtFromEmailAddress.DoubleBuffered = true;
         this.txtFromEmailAddress.Enabled = false;
         this.txtFromEmailAddress.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtFromEmailAddress.ErrorState = false;
         this.txtFromEmailAddress.ErrorToolTip = this.toolTipPrefs;
         this.txtFromEmailAddress.ErrorToolTipDuration = 5000;
         this.txtFromEmailAddress.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtFromEmailAddress.ErrorToolTipText = "Must be a valid e-mail address.";
         this.txtFromEmailAddress.Location = new System.Drawing.Point(92, 77);
         this.txtFromEmailAddress.MaxLength = 200;
         this.txtFromEmailAddress.Name = "txtFromEmailAddress";
         this.txtFromEmailAddress.ReadOnly = true;
         this.txtFromEmailAddress.Size = new System.Drawing.Size(377, 20);
         this.txtFromEmailAddress.TabIndex = 5;
         this.txtFromEmailAddress.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtFromEmailAddress.MouseHover += new System.EventHandler(this.txtFromEmailAddress_MouseHover);
         // 
         // chkEnableEmail
         // 
         this.chkEnableEmail.AutoSize = true;
         this.chkEnableEmail.Location = new System.Drawing.Point(10, 20);
         this.chkEnableEmail.Name = "chkEnableEmail";
         this.chkEnableEmail.Size = new System.Drawing.Size(136, 17);
         this.chkEnableEmail.TabIndex = 0;
         this.chkEnableEmail.Text = "Enable Email Reporting";
         this.chkEnableEmail.UseVisualStyleBackColor = true;
         // 
         // lblSmtpServer
         // 
         this.lblSmtpServer.AutoSize = true;
         this.lblSmtpServer.Location = new System.Drawing.Point(11, 106);
         this.lblSmtpServer.Name = "lblSmtpServer";
         this.lblSmtpServer.Size = new System.Drawing.Size(74, 13);
         this.lblSmtpServer.TabIndex = 6;
         this.lblSmtpServer.Text = "SMTP Server:";
         // 
         // lblToAddress
         // 
         this.lblToAddress.AutoSize = true;
         this.lblToAddress.Location = new System.Drawing.Point(21, 54);
         this.lblToAddress.Name = "lblToAddress";
         this.lblToAddress.Size = new System.Drawing.Size(64, 13);
         this.lblToAddress.TabIndex = 2;
         this.lblToAddress.Text = "To Address:";
         // 
         // txtSmtpServer
         // 
         this.txtSmtpServer.BackColor = System.Drawing.SystemColors.Control;
         this.txtSmtpServer.DoubleBuffered = true;
         this.txtSmtpServer.Enabled = false;
         this.txtSmtpServer.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtSmtpServer.ErrorState = false;
         this.txtSmtpServer.ErrorToolTip = this.toolTipPrefs;
         this.txtSmtpServer.ErrorToolTipDuration = 5000;
         this.txtSmtpServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtSmtpServer.ErrorToolTipText = "Must be a valid server name.";
         this.txtSmtpServer.Location = new System.Drawing.Point(92, 103);
         this.txtSmtpServer.MaxLength = 200;
         this.txtSmtpServer.Name = "txtSmtpServer";
         this.txtSmtpServer.ReadOnly = true;
         this.txtSmtpServer.Size = new System.Drawing.Size(282, 20);
         this.txtSmtpServer.TabIndex = 7;
         this.txtSmtpServer.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // txtToEmailAddress
         // 
         this.txtToEmailAddress.BackColor = System.Drawing.SystemColors.Control;
         this.txtToEmailAddress.DoubleBuffered = true;
         this.txtToEmailAddress.Enabled = false;
         this.txtToEmailAddress.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtToEmailAddress.ErrorState = false;
         this.txtToEmailAddress.ErrorToolTip = this.toolTipPrefs;
         this.txtToEmailAddress.ErrorToolTipDuration = 5000;
         this.txtToEmailAddress.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtToEmailAddress.ErrorToolTipText = "Must be a valid e-mail address.";
         this.txtToEmailAddress.Location = new System.Drawing.Point(92, 51);
         this.txtToEmailAddress.MaxLength = 200;
         this.txtToEmailAddress.Name = "txtToEmailAddress";
         this.txtToEmailAddress.ReadOnly = true;
         this.txtToEmailAddress.Size = new System.Drawing.Size(377, 20);
         this.txtToEmailAddress.TabIndex = 3;
         this.txtToEmailAddress.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // tabWeb
         // 
         this.tabWeb.BackColor = System.Drawing.Color.Transparent;
         this.tabWeb.Controls.Add(this.grpProjectDownload);
         this.tabWeb.Controls.Add(this.grpWebStats);
         this.tabWeb.Controls.Add(this.grpWebProxy);
         this.tabWeb.Location = new System.Drawing.Point(4, 22);
         this.tabWeb.Name = "tabWeb";
         this.tabWeb.Padding = new System.Windows.Forms.Padding(3);
         this.tabWeb.Size = new System.Drawing.Size(501, 303);
         this.tabWeb.TabIndex = 1;
         this.tabWeb.Text = "Web Settings";
         this.tabWeb.UseVisualStyleBackColor = true;
         // 
         // grpProjectDownload
         // 
         this.grpProjectDownload.Controls.Add(this.txtProjectDownloadUrl);
         this.grpProjectDownload.Controls.Add(this.label5);
         this.grpProjectDownload.Location = new System.Drawing.Point(6, 117);
         this.grpProjectDownload.Name = "grpProjectDownload";
         this.grpProjectDownload.Size = new System.Drawing.Size(489, 53);
         this.grpProjectDownload.TabIndex = 1;
         this.grpProjectDownload.TabStop = false;
         this.grpProjectDownload.Text = "Project Download URL";
         // 
         // txtProjectDownloadUrl
         // 
         this.txtProjectDownloadUrl.BackColor = System.Drawing.SystemColors.Window;
         this.txtProjectDownloadUrl.DoubleBuffered = true;
         this.txtProjectDownloadUrl.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtProjectDownloadUrl.ErrorState = false;
         this.txtProjectDownloadUrl.ErrorToolTip = this.toolTipPrefs;
         this.txtProjectDownloadUrl.ErrorToolTipDuration = 5000;
         this.txtProjectDownloadUrl.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtProjectDownloadUrl.ErrorToolTipText = "URL must be a valid URL and the path to a valid Stanford Project Summary page.";
         this.txtProjectDownloadUrl.Location = new System.Drawing.Point(56, 19);
         this.txtProjectDownloadUrl.Name = "txtProjectDownloadUrl";
         this.txtProjectDownloadUrl.Size = new System.Drawing.Size(423, 20);
         this.txtProjectDownloadUrl.TabIndex = 1;
         this.txtProjectDownloadUrl.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // label5
         // 
         this.label5.AutoSize = true;
         this.label5.Location = new System.Drawing.Point(7, 22);
         this.label5.Name = "label5";
         this.label5.Size = new System.Drawing.Size(48, 13);
         this.label5.TabIndex = 0;
         this.label5.Text = "Address:";
         // 
         // grpWebStats
         // 
         this.grpWebStats.Controls.Add(this.lbl3EOCUserID);
         this.grpWebStats.Controls.Add(this.lbl3StanfordUserID);
         this.grpWebStats.Controls.Add(this.linkTeam);
         this.grpWebStats.Controls.Add(this.txtEOCUserID);
         this.grpWebStats.Controls.Add(this.txtStanfordTeamID);
         this.grpWebStats.Controls.Add(this.lbl3StanfordTeamID);
         this.grpWebStats.Controls.Add(this.linkStanford);
         this.grpWebStats.Controls.Add(this.txtStanfordUserID);
         this.grpWebStats.Controls.Add(this.linkEOC);
         this.grpWebStats.Location = new System.Drawing.Point(6, 9);
         this.grpWebStats.Name = "grpWebStats";
         this.grpWebStats.Size = new System.Drawing.Size(489, 102);
         this.grpWebStats.TabIndex = 0;
         this.grpWebStats.TabStop = false;
         this.grpWebStats.Text = "Web Statistics";
         // 
         // lbl3EOCUserID
         // 
         this.lbl3EOCUserID.AutoSize = true;
         this.lbl3EOCUserID.Location = new System.Drawing.Point(7, 20);
         this.lbl3EOCUserID.Name = "lbl3EOCUserID";
         this.lbl3EOCUserID.Size = new System.Drawing.Size(153, 13);
         this.lbl3EOCUserID.TabIndex = 0;
         this.lbl3EOCUserID.Text = "Extreme Overclocking User ID:";
         // 
         // lbl3StanfordUserID
         // 
         this.lbl3StanfordUserID.AutoSize = true;
         this.lbl3StanfordUserID.Location = new System.Drawing.Point(7, 46);
         this.lbl3StanfordUserID.Name = "lbl3StanfordUserID";
         this.lbl3StanfordUserID.Size = new System.Drawing.Size(89, 13);
         this.lbl3StanfordUserID.TabIndex = 1;
         this.lbl3StanfordUserID.Text = "Stanford User ID:";
         // 
         // linkTeam
         // 
         this.linkTeam.AutoSize = true;
         this.linkTeam.Location = new System.Drawing.Point(338, 72);
         this.linkTeam.Name = "linkTeam";
         this.linkTeam.Size = new System.Drawing.Size(72, 13);
         this.linkTeam.TabIndex = 8;
         this.linkTeam.TabStop = true;
         this.linkTeam.Text = "Test Team ID";
         this.linkTeam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTeam_LinkClicked);
         // 
         // txtEOCUserID
         // 
         this.txtEOCUserID.BackColor = System.Drawing.SystemColors.Window;
         this.txtEOCUserID.DoubleBuffered = true;
         this.txtEOCUserID.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtEOCUserID.ErrorState = false;
         this.txtEOCUserID.ErrorToolTip = null;
         this.txtEOCUserID.ErrorToolTipDuration = 5000;
         this.txtEOCUserID.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtEOCUserID.ErrorToolTipText = "";
         this.txtEOCUserID.Location = new System.Drawing.Point(194, 17);
         this.txtEOCUserID.MaxLength = 9;
         this.txtEOCUserID.Name = "txtEOCUserID";
         this.txtEOCUserID.Size = new System.Drawing.Size(138, 20);
         this.txtEOCUserID.TabIndex = 3;
         this.txtEOCUserID.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtEOCUserID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // txtStanfordTeamID
         // 
         this.txtStanfordTeamID.BackColor = System.Drawing.SystemColors.Window;
         this.txtStanfordTeamID.DoubleBuffered = true;
         this.txtStanfordTeamID.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtStanfordTeamID.ErrorState = false;
         this.txtStanfordTeamID.ErrorToolTip = null;
         this.txtStanfordTeamID.ErrorToolTipDuration = 5000;
         this.txtStanfordTeamID.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtStanfordTeamID.ErrorToolTipText = "";
         this.txtStanfordTeamID.Location = new System.Drawing.Point(194, 69);
         this.txtStanfordTeamID.MaxLength = 9;
         this.txtStanfordTeamID.Name = "txtStanfordTeamID";
         this.txtStanfordTeamID.Size = new System.Drawing.Size(138, 20);
         this.txtStanfordTeamID.TabIndex = 5;
         this.txtStanfordTeamID.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtStanfordTeamID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // lbl3StanfordTeamID
         // 
         this.lbl3StanfordTeamID.AutoSize = true;
         this.lbl3StanfordTeamID.Location = new System.Drawing.Point(7, 72);
         this.lbl3StanfordTeamID.Name = "lbl3StanfordTeamID";
         this.lbl3StanfordTeamID.Size = new System.Drawing.Size(94, 13);
         this.lbl3StanfordTeamID.TabIndex = 2;
         this.lbl3StanfordTeamID.Text = "Stanford Team ID:";
         // 
         // linkStanford
         // 
         this.linkStanford.AutoSize = true;
         this.linkStanford.Location = new System.Drawing.Point(338, 46);
         this.linkStanford.Name = "linkStanford";
         this.linkStanford.Size = new System.Drawing.Size(85, 13);
         this.linkStanford.TabIndex = 7;
         this.linkStanford.TabStop = true;
         this.linkStanford.Text = "Test Stanford ID";
         this.linkStanford.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkStanford_LinkClicked);
         // 
         // txtStanfordUserID
         // 
         this.txtStanfordUserID.BackColor = System.Drawing.SystemColors.Window;
         this.txtStanfordUserID.DoubleBuffered = true;
         this.txtStanfordUserID.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtStanfordUserID.ErrorState = false;
         this.txtStanfordUserID.ErrorToolTip = null;
         this.txtStanfordUserID.ErrorToolTipDuration = 5000;
         this.txtStanfordUserID.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtStanfordUserID.ErrorToolTipText = "";
         this.txtStanfordUserID.Location = new System.Drawing.Point(194, 43);
         this.txtStanfordUserID.Name = "txtStanfordUserID";
         this.txtStanfordUserID.Size = new System.Drawing.Size(138, 20);
         this.txtStanfordUserID.TabIndex = 4;
         this.txtStanfordUserID.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // linkEOC
         // 
         this.linkEOC.AutoSize = true;
         this.linkEOC.Location = new System.Drawing.Point(338, 20);
         this.linkEOC.Name = "linkEOC";
         this.linkEOC.Size = new System.Drawing.Size(67, 13);
         this.linkEOC.TabIndex = 6;
         this.linkEOC.TabStop = true;
         this.linkEOC.Text = "Test EOC ID";
         this.linkEOC.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkEOC_LinkClicked);
         // 
         // grpWebProxy
         // 
         this.grpWebProxy.Controls.Add(this.chkUseProxy);
         this.grpWebProxy.Controls.Add(this.chkUseProxyAuth);
         this.grpWebProxy.Controls.Add(this.txtProxyPass);
         this.grpWebProxy.Controls.Add(this.txtProxyUser);
         this.grpWebProxy.Controls.Add(this.txtProxyPort);
         this.grpWebProxy.Controls.Add(this.lbl3ProxyPass);
         this.grpWebProxy.Controls.Add(this.txtProxyServer);
         this.grpWebProxy.Controls.Add(this.lbl3ProxyUser);
         this.grpWebProxy.Controls.Add(this.lbl3Port);
         this.grpWebProxy.Controls.Add(this.lbl3Proxy);
         this.grpWebProxy.Location = new System.Drawing.Point(6, 176);
         this.grpWebProxy.Name = "grpWebProxy";
         this.grpWebProxy.Size = new System.Drawing.Size(489, 117);
         this.grpWebProxy.TabIndex = 2;
         this.grpWebProxy.TabStop = false;
         this.grpWebProxy.Text = "Web Proxy Settings";
         // 
         // chkUseProxy
         // 
         this.chkUseProxy.AutoSize = true;
         this.chkUseProxy.Location = new System.Drawing.Point(6, 17);
         this.chkUseProxy.Name = "chkUseProxy";
         this.chkUseProxy.Size = new System.Drawing.Size(117, 17);
         this.chkUseProxy.TabIndex = 0;
         this.chkUseProxy.Text = "Use a Proxy Server";
         this.chkUseProxy.UseVisualStyleBackColor = true;
         // 
         // chkUseProxyAuth
         // 
         this.chkUseProxyAuth.AutoSize = true;
         this.chkUseProxyAuth.Enabled = false;
         this.chkUseProxyAuth.Location = new System.Drawing.Point(25, 66);
         this.chkUseProxyAuth.Name = "chkUseProxyAuth";
         this.chkUseProxyAuth.Size = new System.Drawing.Size(205, 17);
         this.chkUseProxyAuth.TabIndex = 5;
         this.chkUseProxyAuth.Text = "Authenticate to the Web Proxy Server";
         this.chkUseProxyAuth.UseVisualStyleBackColor = true;
         // 
         // txtProxyPass
         // 
         this.txtProxyPass.BackColor = System.Drawing.SystemColors.Control;
         this.txtProxyPass.DoubleBuffered = true;
         this.txtProxyPass.Enabled = false;
         this.txtProxyPass.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtProxyPass.ErrorState = false;
         this.txtProxyPass.ErrorToolTip = this.toolTipPrefs;
         this.txtProxyPass.ErrorToolTipDuration = 5000;
         this.txtProxyPass.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtProxyPass.ErrorToolTipText = "";
         this.txtProxyPass.Location = new System.Drawing.Point(327, 89);
         this.txtProxyPass.Name = "txtProxyPass";
         this.txtProxyPass.ReadOnly = true;
         this.txtProxyPass.Size = new System.Drawing.Size(155, 20);
         this.txtProxyPass.TabIndex = 9;
         this.txtProxyPass.UseSystemPasswordChar = true;
         this.txtProxyPass.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // txtProxyUser
         // 
         this.txtProxyUser.BackColor = System.Drawing.SystemColors.Control;
         this.txtProxyUser.DoubleBuffered = true;
         this.txtProxyUser.Enabled = false;
         this.txtProxyUser.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtProxyUser.ErrorState = false;
         this.txtProxyUser.ErrorToolTip = this.toolTipPrefs;
         this.txtProxyUser.ErrorToolTipDuration = 5000;
         this.txtProxyUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtProxyUser.ErrorToolTipText = "";
         this.txtProxyUser.Location = new System.Drawing.Point(104, 89);
         this.txtProxyUser.Name = "txtProxyUser";
         this.txtProxyUser.ReadOnly = true;
         this.txtProxyUser.Size = new System.Drawing.Size(155, 20);
         this.txtProxyUser.TabIndex = 7;
         this.txtProxyUser.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // txtProxyPort
         // 
         this.txtProxyPort.BackColor = System.Drawing.SystemColors.Control;
         this.txtProxyPort.DoubleBuffered = true;
         this.txtProxyPort.Enabled = false;
         this.txtProxyPort.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtProxyPort.ErrorState = false;
         this.txtProxyPort.ErrorToolTip = this.toolTipPrefs;
         this.txtProxyPort.ErrorToolTipDuration = 5000;
         this.txtProxyPort.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtProxyPort.ErrorToolTipText = "";
         this.txtProxyPort.Location = new System.Drawing.Point(389, 40);
         this.txtProxyPort.MaxLength = 5;
         this.txtProxyPort.Name = "txtProxyPort";
         this.txtProxyPort.ReadOnly = true;
         this.txtProxyPort.Size = new System.Drawing.Size(94, 20);
         this.txtProxyPort.TabIndex = 4;
         this.txtProxyPort.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         this.txtProxyPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
         // 
         // lbl3ProxyPass
         // 
         this.lbl3ProxyPass.AutoSize = true;
         this.lbl3ProxyPass.Location = new System.Drawing.Point(265, 92);
         this.lbl3ProxyPass.Name = "lbl3ProxyPass";
         this.lbl3ProxyPass.Size = new System.Drawing.Size(56, 13);
         this.lbl3ProxyPass.TabIndex = 8;
         this.lbl3ProxyPass.Text = "Password:";
         // 
         // txtProxyServer
         // 
         this.txtProxyServer.BackColor = System.Drawing.SystemColors.Control;
         this.txtProxyServer.DoubleBuffered = true;
         this.txtProxyServer.Enabled = false;
         this.txtProxyServer.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtProxyServer.ErrorState = false;
         this.txtProxyServer.ErrorToolTip = this.toolTipPrefs;
         this.txtProxyServer.ErrorToolTipDuration = 5000;
         this.txtProxyServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtProxyServer.ErrorToolTipText = "";
         this.txtProxyServer.Location = new System.Drawing.Point(98, 40);
         this.txtProxyServer.Name = "txtProxyServer";
         this.txtProxyServer.ReadOnly = true;
         this.txtProxyServer.Size = new System.Drawing.Size(250, 20);
         this.txtProxyServer.TabIndex = 2;
         this.txtProxyServer.ValidationType = harlam357.Windows.Forms.ValidationType.None;
         // 
         // lbl3ProxyUser
         // 
         this.lbl3ProxyUser.AutoSize = true;
         this.lbl3ProxyUser.Location = new System.Drawing.Point(40, 92);
         this.lbl3ProxyUser.Name = "lbl3ProxyUser";
         this.lbl3ProxyUser.Size = new System.Drawing.Size(58, 13);
         this.lbl3ProxyUser.TabIndex = 6;
         this.lbl3ProxyUser.Text = "Username:";
         // 
         // lbl3Port
         // 
         this.lbl3Port.AutoSize = true;
         this.lbl3Port.Location = new System.Drawing.Point(354, 43);
         this.lbl3Port.Name = "lbl3Port";
         this.lbl3Port.Size = new System.Drawing.Size(29, 13);
         this.lbl3Port.TabIndex = 3;
         this.lbl3Port.Text = "Port:";
         // 
         // lbl3Proxy
         // 
         this.lbl3Proxy.AutoSize = true;
         this.lbl3Proxy.Location = new System.Drawing.Point(22, 43);
         this.lbl3Proxy.Name = "lbl3Proxy";
         this.lbl3Proxy.Size = new System.Drawing.Size(70, 13);
         this.lbl3Proxy.TabIndex = 1;
         this.lbl3Proxy.Text = "Proxy Server:";
         // 
         // tabVisStyles
         // 
         this.tabVisStyles.BackColor = System.Drawing.Color.Transparent;
         this.tabVisStyles.Controls.Add(this.btnMobileSummaryBrowse);
         this.tabVisStyles.Controls.Add(this.txtMobileSummary);
         this.tabVisStyles.Controls.Add(this.lblMobileSummary);
         this.tabVisStyles.Controls.Add(this.btnMobileOverviewBrowse);
         this.tabVisStyles.Controls.Add(this.txtMobileOverview);
         this.tabVisStyles.Controls.Add(this.lblMobileOverview);
         this.tabVisStyles.Controls.Add(this.btnInstanceBrowse);
         this.tabVisStyles.Controls.Add(this.txtInstance);
         this.tabVisStyles.Controls.Add(this.SlotXsltLabel);
         this.tabVisStyles.Controls.Add(this.btnSummaryBrowse);
         this.tabVisStyles.Controls.Add(this.txtSummary);
         this.tabVisStyles.Controls.Add(this.lblSummary);
         this.tabVisStyles.Controls.Add(this.btnOverviewBrowse);
         this.tabVisStyles.Controls.Add(this.txtOverview);
         this.tabVisStyles.Controls.Add(this.lblOverview);
         this.tabVisStyles.Controls.Add(this.pnl1CSSSample);
         this.tabVisStyles.Controls.Add(this.lbl1Preview);
         this.tabVisStyles.Controls.Add(this.StyleList);
         this.tabVisStyles.Controls.Add(this.lbl1Style);
         this.tabVisStyles.Location = new System.Drawing.Point(4, 22);
         this.tabVisStyles.Name = "tabVisStyles";
         this.tabVisStyles.Size = new System.Drawing.Size(501, 303);
         this.tabVisStyles.TabIndex = 3;
         this.tabVisStyles.Text = "Web Visual Styles";
         this.tabVisStyles.UseVisualStyleBackColor = true;
         // 
         // btnMobileSummaryBrowse
         // 
         this.btnMobileSummaryBrowse.Location = new System.Drawing.Point(466, 247);
         this.btnMobileSummaryBrowse.Name = "btnMobileSummaryBrowse";
         this.btnMobileSummaryBrowse.Size = new System.Drawing.Size(24, 23);
         this.btnMobileSummaryBrowse.TabIndex = 15;
         this.btnMobileSummaryBrowse.Text = "...";
         this.btnMobileSummaryBrowse.UseVisualStyleBackColor = true;
         this.btnMobileSummaryBrowse.Click += new System.EventHandler(this.btnMobileSummaryBrowse_Click);
         // 
         // txtMobileSummary
         // 
         this.txtMobileSummary.BackColor = System.Drawing.SystemColors.Control;
         this.txtMobileSummary.DoubleBuffered = true;
         this.txtMobileSummary.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtMobileSummary.ErrorState = false;
         this.txtMobileSummary.ErrorToolTip = this.toolTipPrefs;
         this.txtMobileSummary.ErrorToolTipDuration = 5000;
         this.txtMobileSummary.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtMobileSummary.ErrorToolTipText = "HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL.";
         this.txtMobileSummary.Location = new System.Drawing.Point(132, 249);
         this.txtMobileSummary.Name = "txtMobileSummary";
         this.txtMobileSummary.ReadOnly = true;
         this.txtMobileSummary.Size = new System.Drawing.Size(328, 20);
         this.txtMobileSummary.TabIndex = 14;
         this.txtMobileSummary.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         // 
         // lblMobileSummary
         // 
         this.lblMobileSummary.AutoSize = true;
         this.lblMobileSummary.Location = new System.Drawing.Point(9, 252);
         this.lblMobileSummary.Name = "lblMobileSummary";
         this.lblMobileSummary.Size = new System.Drawing.Size(117, 13);
         this.lblMobileSummary.TabIndex = 13;
         this.lblMobileSummary.Text = "Mobile Summary XSLT:";
         this.lblMobileSummary.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // btnMobileOverviewBrowse
         // 
         this.btnMobileOverviewBrowse.Location = new System.Drawing.Point(466, 195);
         this.btnMobileOverviewBrowse.Name = "btnMobileOverviewBrowse";
         this.btnMobileOverviewBrowse.Size = new System.Drawing.Size(24, 23);
         this.btnMobileOverviewBrowse.TabIndex = 9;
         this.btnMobileOverviewBrowse.Text = "...";
         this.btnMobileOverviewBrowse.UseVisualStyleBackColor = true;
         this.btnMobileOverviewBrowse.Click += new System.EventHandler(this.btnMobileOverviewBrowse_Click);
         // 
         // txtMobileOverview
         // 
         this.txtMobileOverview.BackColor = System.Drawing.SystemColors.Control;
         this.txtMobileOverview.DoubleBuffered = true;
         this.txtMobileOverview.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtMobileOverview.ErrorState = false;
         this.txtMobileOverview.ErrorToolTip = this.toolTipPrefs;
         this.txtMobileOverview.ErrorToolTipDuration = 5000;
         this.txtMobileOverview.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtMobileOverview.ErrorToolTipText = "HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL.";
         this.txtMobileOverview.Location = new System.Drawing.Point(132, 197);
         this.txtMobileOverview.Name = "txtMobileOverview";
         this.txtMobileOverview.ReadOnly = true;
         this.txtMobileOverview.Size = new System.Drawing.Size(328, 20);
         this.txtMobileOverview.TabIndex = 8;
         this.txtMobileOverview.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         // 
         // lblMobileOverview
         // 
         this.lblMobileOverview.AutoSize = true;
         this.lblMobileOverview.Location = new System.Drawing.Point(7, 200);
         this.lblMobileOverview.Name = "lblMobileOverview";
         this.lblMobileOverview.Size = new System.Drawing.Size(119, 13);
         this.lblMobileOverview.TabIndex = 7;
         this.lblMobileOverview.Text = "Mobile Overview XSLT:";
         this.lblMobileOverview.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // btnInstanceBrowse
         // 
         this.btnInstanceBrowse.Location = new System.Drawing.Point(466, 273);
         this.btnInstanceBrowse.Name = "btnInstanceBrowse";
         this.btnInstanceBrowse.Size = new System.Drawing.Size(24, 23);
         this.btnInstanceBrowse.TabIndex = 18;
         this.btnInstanceBrowse.Text = "...";
         this.btnInstanceBrowse.UseVisualStyleBackColor = true;
         this.btnInstanceBrowse.Click += new System.EventHandler(this.btnInstanceBrowse_Click);
         // 
         // txtInstance
         // 
         this.txtInstance.BackColor = System.Drawing.SystemColors.Control;
         this.txtInstance.DoubleBuffered = true;
         this.txtInstance.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtInstance.ErrorState = false;
         this.txtInstance.ErrorToolTip = this.toolTipPrefs;
         this.txtInstance.ErrorToolTipDuration = 5000;
         this.txtInstance.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtInstance.ErrorToolTipText = "HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL.";
         this.txtInstance.Location = new System.Drawing.Point(132, 275);
         this.txtInstance.Name = "txtInstance";
         this.txtInstance.ReadOnly = true;
         this.txtInstance.Size = new System.Drawing.Size(328, 20);
         this.txtInstance.TabIndex = 17;
         this.txtInstance.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         // 
         // SlotXsltLabel
         // 
         this.SlotXsltLabel.AutoSize = true;
         this.SlotXsltLabel.Location = new System.Drawing.Point(68, 278);
         this.SlotXsltLabel.Name = "SlotXsltLabel";
         this.SlotXsltLabel.Size = new System.Drawing.Size(58, 13);
         this.SlotXsltLabel.TabIndex = 16;
         this.SlotXsltLabel.Text = "Slot XSLT:";
         this.SlotXsltLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // btnSummaryBrowse
         // 
         this.btnSummaryBrowse.Location = new System.Drawing.Point(466, 221);
         this.btnSummaryBrowse.Name = "btnSummaryBrowse";
         this.btnSummaryBrowse.Size = new System.Drawing.Size(24, 23);
         this.btnSummaryBrowse.TabIndex = 12;
         this.btnSummaryBrowse.Text = "...";
         this.btnSummaryBrowse.UseVisualStyleBackColor = true;
         this.btnSummaryBrowse.Click += new System.EventHandler(this.btnSummaryBrowse_Click);
         // 
         // txtSummary
         // 
         this.txtSummary.BackColor = System.Drawing.SystemColors.Control;
         this.txtSummary.DoubleBuffered = true;
         this.txtSummary.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtSummary.ErrorState = false;
         this.txtSummary.ErrorToolTip = this.toolTipPrefs;
         this.txtSummary.ErrorToolTipDuration = 5000;
         this.txtSummary.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtSummary.ErrorToolTipText = "HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL.";
         this.txtSummary.Location = new System.Drawing.Point(132, 223);
         this.txtSummary.Name = "txtSummary";
         this.txtSummary.ReadOnly = true;
         this.txtSummary.Size = new System.Drawing.Size(328, 20);
         this.txtSummary.TabIndex = 11;
         this.txtSummary.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         // 
         // lblSummary
         // 
         this.lblSummary.AutoSize = true;
         this.lblSummary.Location = new System.Drawing.Point(43, 226);
         this.lblSummary.Name = "lblSummary";
         this.lblSummary.Size = new System.Drawing.Size(83, 13);
         this.lblSummary.TabIndex = 10;
         this.lblSummary.Text = "Summary XSLT:";
         this.lblSummary.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // btnOverviewBrowse
         // 
         this.btnOverviewBrowse.Location = new System.Drawing.Point(466, 169);
         this.btnOverviewBrowse.Name = "btnOverviewBrowse";
         this.btnOverviewBrowse.Size = new System.Drawing.Size(24, 23);
         this.btnOverviewBrowse.TabIndex = 6;
         this.btnOverviewBrowse.Text = "...";
         this.btnOverviewBrowse.UseVisualStyleBackColor = true;
         this.btnOverviewBrowse.Click += new System.EventHandler(this.btnOverviewBrowse_Click);
         // 
         // txtOverview
         // 
         this.txtOverview.BackColor = System.Drawing.SystemColors.Control;
         this.txtOverview.DoubleBuffered = true;
         this.txtOverview.ErrorBackColor = System.Drawing.Color.Yellow;
         this.txtOverview.ErrorState = false;
         this.txtOverview.ErrorToolTip = this.toolTipPrefs;
         this.txtOverview.ErrorToolTipDuration = 5000;
         this.txtOverview.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
         this.txtOverview.ErrorToolTipText = "HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL.";
         this.txtOverview.Location = new System.Drawing.Point(132, 171);
         this.txtOverview.Name = "txtOverview";
         this.txtOverview.ReadOnly = true;
         this.txtOverview.Size = new System.Drawing.Size(328, 20);
         this.txtOverview.TabIndex = 5;
         this.txtOverview.ValidationType = harlam357.Windows.Forms.ValidationType.Custom;
         // 
         // lblOverview
         // 
         this.lblOverview.AutoSize = true;
         this.lblOverview.Location = new System.Drawing.Point(41, 174);
         this.lblOverview.Name = "lblOverview";
         this.lblOverview.Size = new System.Drawing.Size(85, 13);
         this.lblOverview.TabIndex = 4;
         this.lblOverview.Text = "Overview XSLT:";
         this.lblOverview.TextAlign = System.Drawing.ContentAlignment.TopRight;
         // 
         // lbl1Preview
         // 
         this.lbl1Preview.Location = new System.Drawing.Point(129, 6);
         this.lbl1Preview.Name = "lbl1Preview";
         this.lbl1Preview.Size = new System.Drawing.Size(67, 23);
         this.lbl1Preview.TabIndex = 1;
         this.lbl1Preview.Text = "Preview";
         // 
         // lbl1Style
         // 
         this.lbl1Style.Location = new System.Drawing.Point(3, 6);
         this.lbl1Style.Name = "lbl1Style";
         this.lbl1Style.Size = new System.Drawing.Size(84, 23);
         this.lbl1Style.TabIndex = 0;
         this.lbl1Style.Text = "Style Sheet";
         // 
         // openConfigDialog
         // 
         this.openConfigDialog.DefaultExt = "hfm";
         this.openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
         this.openConfigDialog.RestoreDirectory = true;
         // 
         // btnOK
         // 
         this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnOK.Location = new System.Drawing.Point(366, 353);
         this.btnOK.Name = "btnOK";
         this.btnOK.Size = new System.Drawing.Size(75, 23);
         this.btnOK.TabIndex = 3;
         this.btnOK.Text = "OK";
         this.btnOK.UseVisualStyleBackColor = true;
         this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
         // 
         // btnCancel
         // 
         this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
         this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
         this.btnCancel.Location = new System.Drawing.Point(447, 353);
         this.btnCancel.Name = "btnCancel";
         this.btnCancel.Size = new System.Drawing.Size(75, 23);
         this.btnCancel.TabIndex = 4;
         this.btnCancel.Text = "Cancel";
         this.btnCancel.UseVisualStyleBackColor = true;
         this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
         // 
         // PreferencesDialog
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(532, 388);
         this.Controls.Add(this.tabControl1);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.btnCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "PreferencesDialog";
         this.ShowInTaskbar = false;
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Preferences";
         this.Load += new System.EventHandler(this.PreferencesDialogLoad);
         this.tabControl1.ResumeLayout(false);
         this.tabSchdTasks.ResumeLayout(false);
         this.grpUpdateData.ResumeLayout(false);
         this.grpUpdateData.PerformLayout();
         this.grpHTMLOutput.ResumeLayout(false);
         this.grpHTMLOutput.PerformLayout();
         this.WebGenTypePanel.ResumeLayout(false);
         this.WebGenTypePanel.PerformLayout();
         this.FtpModePanel.ResumeLayout(false);
         this.FtpModePanel.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udLimitSize)).EndInit();
         this.tabStartup.ResumeLayout(false);
         this.grpFileExplorer.ResumeLayout(false);
         this.grpFileExplorer.PerformLayout();
         this.grpLogFileViewer.ResumeLayout(false);
         this.grpLogFileViewer.PerformLayout();
         this.grpDefaultConfig.ResumeLayout(false);
         this.grpDefaultConfig.PerformLayout();
         this.grpStartup.ResumeLayout(false);
         this.grpStartup.PerformLayout();
         this.tabOptions.ResumeLayout(false);
         this.grpShowStyle.ResumeLayout(false);
         this.grpShowStyle.PerformLayout();
         this.grpInteractiveOptions.ResumeLayout(false);
         this.grpInteractiveOptions.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).EndInit();
         this.grpDebugMessageLevel.ResumeLayout(false);
         this.grpDebugMessageLevel.PerformLayout();
         this.tabReporting.ResumeLayout(false);
         this.grpReportSelections.ResumeLayout(false);
         this.grpReportSelections.PerformLayout();
         this.grpEmailSettings.ResumeLayout(false);
         this.grpEmailSettings.PerformLayout();
         this.tabWeb.ResumeLayout(false);
         this.grpProjectDownload.ResumeLayout(false);
         this.grpProjectDownload.PerformLayout();
         this.grpWebStats.ResumeLayout(false);
         this.grpWebStats.PerformLayout();
         this.grpWebProxy.ResumeLayout(false);
         this.grpWebProxy.PerformLayout();
         this.tabVisStyles.ResumeLayout(false);
         this.tabVisStyles.PerformLayout();
         this.ResumeLayout(false);

      }

      #endregion

      private GroupBoxWrapper grpUpdateData;
      private LabelWrapper lbl2SchedExplain;
      private ButtonWrapper btnCancel;
      private ButtonWrapper btnOK;
      private System.Windows.Forms.Panel pnl1CSSSample;
      private System.Windows.Forms.ListBox StyleList;
      private LabelWrapper lbl1Style;
      private LabelWrapper lbl1Preview;
      private GroupBoxWrapper grpHTMLOutput;
      private LabelWrapper WebSiteTargetPathLabel;
      private ButtonWrapper btnBrowseWebFolder;
      private LabelWrapper lbl2MinutesToGen;
      private System.Windows.Forms.FolderBrowserDialog locateWebFolder;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabWeb;
      private System.Windows.Forms.TabPage tabVisStyles;
      private System.Windows.Forms.TabPage tabSchdTasks;
      private ValidatingTextBox txtStanfordUserID;
      private ValidatingTextBox txtEOCUserID;
      private LabelWrapper lbl3StanfordUserID;
      private LabelWrapper lbl3EOCUserID;
      private System.Windows.Forms.LinkLabel linkEOC;
      private System.Windows.Forms.LinkLabel linkStanford;
      private System.Windows.Forms.LinkLabel linkTeam;
      private ValidatingTextBox txtStanfordTeamID;
      private LabelWrapper lbl3StanfordTeamID;
      private ValidatingTextBox txtCollectMinutes;
      private CheckBoxWrapper chkScheduled;
      private CheckBoxWrapper chkSynchronous;
      private ValidatingTextBox txtWebGenMinutes;
      private CheckBoxWrapper chkWebSiteGenerator;
      private ValidatingTextBox WebSiteTargetPathTextBox;
      private GroupBoxWrapper grpWebStats;
      private GroupBoxWrapper grpWebProxy;
      private ValidatingTextBox txtProxyServer;
      private LabelWrapper lbl3Proxy;
      private ValidatingTextBox txtProxyPass;
      private ValidatingTextBox txtProxyUser;
      private ValidatingTextBox txtProxyPort;
      private LabelWrapper lbl3ProxyPass;
      private LabelWrapper lbl3ProxyUser;
      private LabelWrapper lbl3Port;
      private CheckBoxWrapper chkUseProxyAuth;
      private CheckBoxWrapper chkUseProxy;
      private System.Windows.Forms.TabPage tabOptions;
      private System.Windows.Forms.OpenFileDialog openConfigDialog;
      private GroupBoxWrapper grpProjectDownload;
      private ValidatingTextBox txtProjectDownloadUrl;
      private LabelWrapper label5;
      private RadioButtonWrapper radioSchedule;
      private RadioButtonWrapper radioFullRefresh;
      private GroupBoxWrapper grpDebugMessageLevel;
      private ComboBoxWrapper cboMessageLevel;
      private LabelWrapper label6;
      private System.Windows.Forms.ToolTip toolTipPrefs;
      private LabelWrapper labelWrapper1;
      private System.Windows.Forms.NumericUpDown udDecimalPlaces;
      private System.Windows.Forms.TabPage tabReporting;
      private GroupBoxWrapper grpEmailSettings;
      private LabelWrapper lblSmtpServer;
      private LabelWrapper lblToAddress;
      private ValidatingTextBox txtSmtpServer;
      private ValidatingTextBox txtToEmailAddress;
      private CheckBoxWrapper chkEnableEmail;
      private LabelWrapper lblFromEmailAddress;
      private ValidatingTextBox txtFromEmailAddress;
      private ValidatingTextBox txtSmtpPassword;
      private ValidatingTextBox txtSmtpUsername;
      private LabelWrapper labelWrapper4;
      private LabelWrapper labelWrapper5;
      private ButtonWrapper btnTestEmail;
      private GroupBoxWrapper grpReportSelections;
      private CheckBoxWrapper chkClientEuePause;
      private System.Windows.Forms.TabPage tabStartup;
      private GroupBoxWrapper grpStartup;
      private CheckBoxWrapper chkRunMinimized;
      private CheckBoxWrapper chkAutoRun;
      private CheckBoxWrapper chkFAHlog;
      private GroupBoxWrapper grpDefaultConfig;
      private CheckBoxWrapper chkDefaultConfig;
      private ButtonWrapper btnBrowseConfigFile;
      private ValidatingTextBox txtDefaultConfigFile;
      private LabelWrapper label1;
      private GroupBoxWrapper grpInteractiveOptions;
      private CheckBoxWrapper chkAutoSave;
      private CheckBoxWrapper chkColorLog;
      private CheckBoxWrapper chkOffline;
      private LabelWrapper label2;
      private ComboBoxWrapper cboPpdCalc;
      private CheckBoxWrapper chkCalcBonus;
      private CheckBoxWrapper chkAllowRunningAsync;
      private RadioButtonWrapper radioActive;
      private RadioButtonWrapper radioPassive;
      private LabelWrapper lblFtpMode;
      private ButtonWrapper TestConnectionButton;
      private ButtonWrapper btnInstanceBrowse;
      private ValidatingTextBox txtInstance;
      private LabelWrapper SlotXsltLabel;
      private ButtonWrapper btnSummaryBrowse;
      private ValidatingTextBox txtSummary;
      private LabelWrapper lblSummary;
      private ButtonWrapper btnOverviewBrowse;
      private ValidatingTextBox txtOverview;
      private LabelWrapper lblOverview;
      private ButtonWrapper btnMobileOverviewBrowse;
      private ValidatingTextBox txtMobileOverview;
      private LabelWrapper lblMobileOverview;
      private ButtonWrapper btnMobileSummaryBrowse;
      private ValidatingTextBox txtMobileSummary;
      private LabelWrapper lblMobileSummary;
      private GroupBoxWrapper grpShowStyle;
      private ComboBoxWrapper cboShowStyle;
      private LabelWrapper labelWrapper2;
      private CheckBoxWrapper chkEmailSecure;
      private ValidatingTextBox txtSmtpServerPort;
      private LabelWrapper labelWrapper3;
      private CheckBoxWrapper chkCheckForUpdate;
      private GroupBoxWrapper grpFileExplorer;
      private ButtonWrapper btnBrowseFileExplorer;
      private LabelWrapper label4;
      private ValidatingTextBox txtFileExplorer;
      private GroupBoxWrapper grpLogFileViewer;
      private ButtonWrapper btnBrowseLogViewer;
      private LabelWrapper label3;
      private ValidatingTextBox txtLogFileViewer;
      private CheckBoxWrapper chkHtml;
      private CheckBoxWrapper chkXml;
      private System.Windows.Forms.NumericUpDown udLimitSize;
      private CheckBoxWrapper chkLimitSize;
      private RadioPanel FtpModePanel;
      private CheckBoxWrapper chkEtaAsDate;
      private CheckBoxWrapper chkClientHung;
      private LabelWrapper WebSiteServerLabel;
      private LabelWrapper WebSitePasswordLabel;
      private LabelWrapper WebSiteUsernameLabel;
      private CheckBoxWrapper DuplicateUserCheckBox;
      private CheckBoxWrapper DuplicateProjectCheckBox;
      private RadioPanel WebGenTypePanel;
      private ValidatingTextBox WebSitePasswordTextBox;
      private ValidatingTextBox WebSiteUsernameTextBox;
      private ValidatingTextBox WebSiteServerTextBox;
      private CheckBoxWrapper ShowUserStatsCheckBox;
      private LabelWrapper UploadTypeLabel;
      private RadioButtonWrapper WebGenTypeFtpRadioButton;
      private RadioButtonWrapper WebGenTypePathRadioButton;
      private ValidatingTextBox WebSitePortTextBox;
      private LabelWrapper WebSitePortLabel;
   }
}
