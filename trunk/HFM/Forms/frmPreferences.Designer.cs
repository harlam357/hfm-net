/*
 * HFM.NET - User Preferences Form
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
   partial class frmPreferences
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
         this.pnl1CSSSample = new System.Windows.Forms.Panel();
         this.StyleList = new System.Windows.Forms.ListBox();
         this.locateWebFolder = new System.Windows.Forms.FolderBrowserDialog();
         this.tabControl1 = new System.Windows.Forms.TabControl();
         this.tabSchdTasks = new System.Windows.Forms.TabPage();
         this.grpUpdateData = new HFM.Classes.GroupBoxWrapper();
         this.chkColorLog = new HFM.Classes.CheckBoxWrapper();
         this.chkDuplicateProject = new HFM.Classes.CheckBoxWrapper();
         this.chkDuplicateUserID = new HFM.Classes.CheckBoxWrapper();
         this.chkShowUserStats = new HFM.Classes.CheckBoxWrapper();
         this.label2 = new HFM.Classes.LabelWrapper();
         this.cboPpdCalc = new HFM.Classes.ComboBoxWrapper();
         this.chkOffline = new HFM.Classes.CheckBoxWrapper();
         this.lbl2Collect = new HFM.Classes.LabelWrapper();
         this.txtCollectMinutes = new HFM.Classes.TextBoxWrapper();
         this.lbl2SchedExplain = new HFM.Classes.LabelWrapper();
         this.chkScheduled = new HFM.Classes.CheckBoxWrapper();
         this.chkSynchronous = new HFM.Classes.CheckBoxWrapper();
         this.grpHTMLOutput = new HFM.Classes.GroupBoxWrapper();
         this.chkFAHlog = new HFM.Classes.CheckBoxWrapper();
         this.radioFullRefresh = new HFM.Classes.RadioButtonWrapper();
         this.radioSchedule = new HFM.Classes.RadioButtonWrapper();
         this.txtWebGenMinutes = new HFM.Classes.TextBoxWrapper();
         this.lbl2MinutesToGen = new HFM.Classes.LabelWrapper();
         this.btnBrowseWebFolder = new HFM.Classes.ButtonWrapper();
         this.txtWebSiteBase = new HFM.Classes.TextBoxWrapper();
         this.lbl2WebSiteDir = new HFM.Classes.LabelWrapper();
         this.chkWebSiteGenerator = new HFM.Classes.CheckBoxWrapper();
         this.tabStartup = new System.Windows.Forms.TabPage();
         this.grpStartup = new HFM.Classes.GroupBoxWrapper();
         this.checkBoxWrapper1 = new HFM.Classes.CheckBoxWrapper();
         this.checkBoxWrapper2 = new HFM.Classes.CheckBoxWrapper();
         this.tabDefaults = new System.Windows.Forms.TabPage();
         this.grpDecimalPlaces = new HFM.Classes.GroupBoxWrapper();
         this.udDecimalPlaces = new System.Windows.Forms.NumericUpDown();
         this.labelWrapper1 = new HFM.Classes.LabelWrapper();
         this.grpDebugMessageLevel = new HFM.Classes.GroupBoxWrapper();
         this.cboMessageLevel = new HFM.Classes.ComboBoxWrapper();
         this.label6 = new HFM.Classes.LabelWrapper();
         this.grpFileExplorer = new HFM.Classes.GroupBoxWrapper();
         this.btnBrowseFileExplorer = new HFM.Classes.ButtonWrapper();
         this.label4 = new HFM.Classes.LabelWrapper();
         this.txtFileExplorer = new HFM.Classes.TextBoxWrapper();
         this.grpLogFileViewer = new HFM.Classes.GroupBoxWrapper();
         this.btnBrowseLogViewer = new HFM.Classes.ButtonWrapper();
         this.label3 = new HFM.Classes.LabelWrapper();
         this.txtLogFileViewer = new HFM.Classes.TextBoxWrapper();
         this.grpDefaultConfig = new HFM.Classes.GroupBoxWrapper();
         this.chkAutoSave = new HFM.Classes.CheckBoxWrapper();
         this.chkDefaultConfig = new HFM.Classes.CheckBoxWrapper();
         this.btnBrowseConfigFile = new HFM.Classes.ButtonWrapper();
         this.txtDefaultConfigFile = new HFM.Classes.TextBoxWrapper();
         this.label1 = new HFM.Classes.LabelWrapper();
         this.tabReporting = new System.Windows.Forms.TabPage();
         this.grpReportSelections = new HFM.Classes.GroupBoxWrapper();
         this.chkClientEuePause = new HFM.Classes.CheckBoxWrapper();
         this.grpEmailSettings = new HFM.Classes.GroupBoxWrapper();
         this.btnTestEmail = new HFM.Classes.ButtonWrapper();
         this.txtSmtpPassword = new HFM.Classes.TextBoxWrapper();
         this.txtSmtpUsername = new HFM.Classes.TextBoxWrapper();
         this.labelWrapper4 = new HFM.Classes.LabelWrapper();
         this.labelWrapper5 = new HFM.Classes.LabelWrapper();
         this.lblFromEmailAddress = new HFM.Classes.LabelWrapper();
         this.txtFromEmailAddress = new HFM.Classes.TextBoxWrapper();
         this.chkEnableEmail = new HFM.Classes.CheckBoxWrapper();
         this.lblSmtpServer = new HFM.Classes.LabelWrapper();
         this.lblToAddress = new HFM.Classes.LabelWrapper();
         this.txtSmtpServer = new HFM.Classes.TextBoxWrapper();
         this.txtToEmailAddress = new HFM.Classes.TextBoxWrapper();
         this.tabWeb = new System.Windows.Forms.TabPage();
         this.grpProjectDownload = new HFM.Classes.GroupBoxWrapper();
         this.txtProjectDownloadUrl = new HFM.Classes.TextBoxWrapper();
         this.label5 = new HFM.Classes.LabelWrapper();
         this.grpWebStats = new HFM.Classes.GroupBoxWrapper();
         this.lbl3EOCUserID = new HFM.Classes.LabelWrapper();
         this.lbl3StanfordUserID = new HFM.Classes.LabelWrapper();
         this.linkTeam = new System.Windows.Forms.LinkLabel();
         this.txtEOCUserID = new HFM.Classes.TextBoxWrapper();
         this.txtStanfordTeamID = new HFM.Classes.TextBoxWrapper();
         this.lbl3StanfordTeamID = new HFM.Classes.LabelWrapper();
         this.linkStanford = new System.Windows.Forms.LinkLabel();
         this.txtStanfordUserID = new HFM.Classes.TextBoxWrapper();
         this.linkEOC = new System.Windows.Forms.LinkLabel();
         this.grpWebProxy = new HFM.Classes.GroupBoxWrapper();
         this.chkUseProxy = new HFM.Classes.CheckBoxWrapper();
         this.chkUseProxyAuth = new HFM.Classes.CheckBoxWrapper();
         this.txtProxyPass = new HFM.Classes.TextBoxWrapper();
         this.txtProxyUser = new HFM.Classes.TextBoxWrapper();
         this.txtProxyPort = new HFM.Classes.TextBoxWrapper();
         this.lbl3ProxyPass = new HFM.Classes.LabelWrapper();
         this.txtProxyServer = new HFM.Classes.TextBoxWrapper();
         this.lbl3ProxyUser = new HFM.Classes.LabelWrapper();
         this.lbl3Port = new HFM.Classes.LabelWrapper();
         this.lbl3Proxy = new HFM.Classes.LabelWrapper();
         this.tabVisStyles = new System.Windows.Forms.TabPage();
         this.lbl1Preview = new HFM.Classes.LabelWrapper();
         this.lbl1Style = new HFM.Classes.LabelWrapper();
         this.openConfigDialog = new System.Windows.Forms.OpenFileDialog();
         this.toolTipPrefs = new System.Windows.Forms.ToolTip(this.components);
         this.btnOK = new HFM.Classes.ButtonWrapper();
         this.btnCancel = new HFM.Classes.ButtonWrapper();
         this.chkRunMinimized = new HFM.Classes.CheckBoxWrapper();
         this.chkAutoRun = new HFM.Classes.CheckBoxWrapper();
         this.tabControl1.SuspendLayout();
         this.tabSchdTasks.SuspendLayout();
         this.grpUpdateData.SuspendLayout();
         this.grpHTMLOutput.SuspendLayout();
         this.tabStartup.SuspendLayout();
         this.grpStartup.SuspendLayout();
         this.tabDefaults.SuspendLayout();
         this.grpDecimalPlaces.SuspendLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).BeginInit();
         this.grpDebugMessageLevel.SuspendLayout();
         this.grpFileExplorer.SuspendLayout();
         this.grpLogFileViewer.SuspendLayout();
         this.grpDefaultConfig.SuspendLayout();
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
         this.pnl1CSSSample.Location = new System.Drawing.Point(132, 31);
         this.pnl1CSSSample.Name = "pnl1CSSSample";
         this.pnl1CSSSample.Size = new System.Drawing.Size(358, 212);
         this.pnl1CSSSample.TabIndex = 1;
         // 
         // StyleList
         // 
         this.StyleList.FormattingEnabled = true;
         this.StyleList.Location = new System.Drawing.Point(6, 31);
         this.StyleList.Name = "StyleList";
         this.StyleList.Size = new System.Drawing.Size(120, 212);
         this.StyleList.Sorted = true;
         this.StyleList.TabIndex = 1;
         this.StyleList.SelectedIndexChanged += new System.EventHandler(this.StyleList_SelectedIndexChanged);
         // 
         // tabControl1
         // 
         this.tabControl1.Controls.Add(this.tabSchdTasks);
         this.tabControl1.Controls.Add(this.tabStartup);
         this.tabControl1.Controls.Add(this.tabDefaults);
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
         this.tabSchdTasks.BackColor = System.Drawing.SystemColors.Control;
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
         this.grpUpdateData.Controls.Add(this.chkColorLog);
         this.grpUpdateData.Controls.Add(this.chkDuplicateProject);
         this.grpUpdateData.Controls.Add(this.chkDuplicateUserID);
         this.grpUpdateData.Controls.Add(this.chkShowUserStats);
         this.grpUpdateData.Controls.Add(this.label2);
         this.grpUpdateData.Controls.Add(this.cboPpdCalc);
         this.grpUpdateData.Controls.Add(this.chkOffline);
         this.grpUpdateData.Controls.Add(this.lbl2Collect);
         this.grpUpdateData.Controls.Add(this.txtCollectMinutes);
         this.grpUpdateData.Controls.Add(this.lbl2SchedExplain);
         this.grpUpdateData.Controls.Add(this.chkScheduled);
         this.grpUpdateData.Controls.Add(this.chkSynchronous);
         this.grpUpdateData.Location = new System.Drawing.Point(6, 9);
         this.grpUpdateData.Name = "grpUpdateData";
         this.grpUpdateData.Size = new System.Drawing.Size(489, 154);
         this.grpUpdateData.TabIndex = 0;
         this.grpUpdateData.TabStop = false;
         this.grpUpdateData.Text = "Update Data";
         // 
         // chkColorLog
         // 
         this.chkColorLog.AutoSize = true;
         this.chkColorLog.Location = new System.Drawing.Point(270, 118);
         this.chkColorLog.Name = "chkColorLog";
         this.chkColorLog.Size = new System.Drawing.Size(126, 17);
         this.chkColorLog.TabIndex = 11;
         this.chkColorLog.Text = "Color the FAHlog text";
         this.chkColorLog.UseVisualStyleBackColor = true;
         // 
         // chkDuplicateProject
         // 
         this.chkDuplicateProject.AutoSize = true;
         this.chkDuplicateProject.Location = new System.Drawing.Point(270, 92);
         this.chkDuplicateProject.Name = "chkDuplicateProject";
         this.chkDuplicateProject.Size = new System.Drawing.Size(182, 17);
         this.chkDuplicateProject.TabIndex = 8;
         this.chkDuplicateProject.Text = "Duplicate Project (R/C/G) check";
         this.chkDuplicateProject.UseVisualStyleBackColor = true;
         // 
         // chkDuplicateUserID
         // 
         this.chkDuplicateUserID.AutoSize = true;
         this.chkDuplicateUserID.Location = new System.Drawing.Point(10, 92);
         this.chkDuplicateUserID.Name = "chkDuplicateUserID";
         this.chkDuplicateUserID.Size = new System.Drawing.Size(189, 17);
         this.chkDuplicateUserID.TabIndex = 7;
         this.chkDuplicateUserID.Text = "Duplicate User/Machine ID check";
         this.chkDuplicateUserID.UseVisualStyleBackColor = true;
         // 
         // chkShowUserStats
         // 
         this.chkShowUserStats.AutoSize = true;
         this.chkShowUserStats.Location = new System.Drawing.Point(270, 66);
         this.chkShowUserStats.Name = "chkShowUserStats";
         this.chkShowUserStats.Size = new System.Drawing.Size(190, 17);
         this.chkShowUserStats.TabIndex = 6;
         this.chkShowUserStats.Text = "Retrieve and Show EOC user stats";
         this.chkShowUserStats.UseVisualStyleBackColor = true;
         // 
         // label2
         // 
         this.label2.AutoSize = true;
         this.label2.Location = new System.Drawing.Point(7, 125);
         this.label2.Name = "label2";
         this.label2.Size = new System.Drawing.Size(123, 13);
         this.label2.TabIndex = 9;
         this.label2.Text = "Calculate PPD based on";
         // 
         // cboPpdCalc
         // 
         this.cboPpdCalc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboPpdCalc.FormattingEnabled = true;
         this.cboPpdCalc.Location = new System.Drawing.Point(136, 122);
         this.cboPpdCalc.Name = "cboPpdCalc";
         this.cboPpdCalc.Size = new System.Drawing.Size(109, 21);
         this.cboPpdCalc.TabIndex = 10;
         // 
         // chkOffline
         // 
         this.chkOffline.AutoSize = true;
         this.chkOffline.Location = new System.Drawing.Point(10, 66);
         this.chkOffline.Name = "chkOffline";
         this.chkOffline.Size = new System.Drawing.Size(157, 17);
         this.chkOffline.TabIndex = 5;
         this.chkOffline.Text = "Always list offline clients last";
         this.chkOffline.UseVisualStyleBackColor = true;
         // 
         // lbl2Collect
         // 
         this.lbl2Collect.AutoSize = true;
         this.lbl2Collect.Location = new System.Drawing.Point(7, 19);
         this.lbl2Collect.Name = "lbl2Collect";
         this.lbl2Collect.Size = new System.Drawing.Size(94, 13);
         this.lbl2Collect.TabIndex = 0;
         this.lbl2Collect.Text = "Collect client data:";
         // 
         // txtCollectMinutes
         // 
         this.txtCollectMinutes.Location = new System.Drawing.Point(323, 38);
         this.txtCollectMinutes.MaxLength = 3;
         this.txtCollectMinutes.Name = "txtCollectMinutes";
         this.txtCollectMinutes.Size = new System.Drawing.Size(48, 20);
         this.txtCollectMinutes.TabIndex = 3;
         this.txtCollectMinutes.Text = "15";
         this.txtCollectMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         this.txtCollectMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDigitsOnly_KeyPress);
         this.txtCollectMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinutes_Validating);
         // 
         // lbl2SchedExplain
         // 
         this.lbl2SchedExplain.AutoSize = true;
         this.lbl2SchedExplain.Location = new System.Drawing.Point(375, 41);
         this.lbl2SchedExplain.Name = "lbl2SchedExplain";
         this.lbl2SchedExplain.Size = new System.Drawing.Size(44, 13);
         this.lbl2SchedExplain.TabIndex = 4;
         this.lbl2SchedExplain.Text = "Minutes";
         this.lbl2SchedExplain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
         // 
         // chkScheduled
         // 
         this.chkScheduled.AutoSize = true;
         this.chkScheduled.Checked = true;
         this.chkScheduled.CheckState = System.Windows.Forms.CheckState.Checked;
         this.chkScheduled.Location = new System.Drawing.Point(270, 40);
         this.chkScheduled.Name = "chkScheduled";
         this.chkScheduled.Size = new System.Drawing.Size(53, 17);
         this.chkScheduled.TabIndex = 2;
         this.chkScheduled.Text = "Every";
         this.chkScheduled.UseVisualStyleBackColor = true;
         this.chkScheduled.CheckedChanged += new System.EventHandler(this.chkScheduled_CheckedChanged);
         // 
         // chkSynchronous
         // 
         this.chkSynchronous.AutoSize = true;
         this.chkSynchronous.Location = new System.Drawing.Point(10, 40);
         this.chkSynchronous.Name = "chkSynchronous";
         this.chkSynchronous.Size = new System.Drawing.Size(174, 17);
         this.chkSynchronous.TabIndex = 1;
         this.chkSynchronous.Text = "In series (synchronous retrieval)";
         this.chkSynchronous.UseVisualStyleBackColor = true;
         // 
         // grpHTMLOutput
         // 
         this.grpHTMLOutput.Controls.Add(this.chkFAHlog);
         this.grpHTMLOutput.Controls.Add(this.radioFullRefresh);
         this.grpHTMLOutput.Controls.Add(this.radioSchedule);
         this.grpHTMLOutput.Controls.Add(this.txtWebGenMinutes);
         this.grpHTMLOutput.Controls.Add(this.lbl2MinutesToGen);
         this.grpHTMLOutput.Controls.Add(this.btnBrowseWebFolder);
         this.grpHTMLOutput.Controls.Add(this.txtWebSiteBase);
         this.grpHTMLOutput.Controls.Add(this.lbl2WebSiteDir);
         this.grpHTMLOutput.Controls.Add(this.chkWebSiteGenerator);
         this.grpHTMLOutput.Location = new System.Drawing.Point(6, 166);
         this.grpHTMLOutput.Name = "grpHTMLOutput";
         this.grpHTMLOutput.Size = new System.Drawing.Size(489, 131);
         this.grpHTMLOutput.TabIndex = 1;
         this.grpHTMLOutput.TabStop = false;
         this.grpHTMLOutput.Text = "HTML Output";
         // 
         // chkFAHlog
         // 
         this.chkFAHlog.AutoSize = true;
         this.chkFAHlog.Enabled = false;
         this.chkFAHlog.Location = new System.Drawing.Point(10, 76);
         this.chkFAHlog.Name = "chkFAHlog";
         this.chkFAHlog.Size = new System.Drawing.Size(123, 17);
         this.chkFAHlog.TabIndex = 8;
         this.chkFAHlog.Text = "Copy FAHlog.txt files";
         this.chkFAHlog.UseVisualStyleBackColor = true;
         // 
         // radioFullRefresh
         // 
         this.radioFullRefresh.AutoSize = true;
         this.radioFullRefresh.Enabled = false;
         this.radioFullRefresh.Location = new System.Drawing.Point(319, 19);
         this.radioFullRefresh.Name = "radioFullRefresh";
         this.radioFullRefresh.Size = new System.Drawing.Size(125, 17);
         this.radioFullRefresh.TabIndex = 4;
         this.radioFullRefresh.TabStop = true;
         this.radioFullRefresh.Text = "After each full refresh";
         this.radioFullRefresh.UseVisualStyleBackColor = true;
         // 
         // radioSchedule
         // 
         this.radioSchedule.AutoSize = true;
         this.radioSchedule.Enabled = false;
         this.radioSchedule.Location = new System.Drawing.Point(151, 19);
         this.radioSchedule.Name = "radioSchedule";
         this.radioSchedule.Size = new System.Drawing.Size(52, 17);
         this.radioSchedule.TabIndex = 1;
         this.radioSchedule.TabStop = true;
         this.radioSchedule.Text = "Every";
         this.radioSchedule.UseVisualStyleBackColor = true;
         this.radioSchedule.CheckedChanged += new System.EventHandler(this.radioSchedule_CheckedChanged);
         // 
         // txtWebGenMinutes
         // 
         this.txtWebGenMinutes.Enabled = false;
         this.txtWebGenMinutes.Location = new System.Drawing.Point(204, 18);
         this.txtWebGenMinutes.MaxLength = 3;
         this.txtWebGenMinutes.Name = "txtWebGenMinutes";
         this.txtWebGenMinutes.ReadOnly = true;
         this.txtWebGenMinutes.Size = new System.Drawing.Size(48, 20);
         this.txtWebGenMinutes.TabIndex = 2;
         this.txtWebGenMinutes.Text = "15";
         this.txtWebGenMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
         this.txtWebGenMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDigitsOnly_KeyPress);
         this.txtWebGenMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinutes_Validating);
         // 
         // lbl2MinutesToGen
         // 
         this.lbl2MinutesToGen.AutoSize = true;
         this.lbl2MinutesToGen.Enabled = false;
         this.lbl2MinutesToGen.Location = new System.Drawing.Point(256, 21);
         this.lbl2MinutesToGen.Name = "lbl2MinutesToGen";
         this.lbl2MinutesToGen.Size = new System.Drawing.Size(44, 13);
         this.lbl2MinutesToGen.TabIndex = 3;
         this.lbl2MinutesToGen.Text = "Minutes";
         // 
         // btnBrowseWebFolder
         // 
         this.btnBrowseWebFolder.Enabled = false;
         this.btnBrowseWebFolder.Location = new System.Drawing.Point(445, 43);
         this.btnBrowseWebFolder.Name = "btnBrowseWebFolder";
         this.btnBrowseWebFolder.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseWebFolder.TabIndex = 7;
         this.btnBrowseWebFolder.Text = "...";
         this.btnBrowseWebFolder.UseVisualStyleBackColor = true;
         this.btnBrowseWebFolder.Click += new System.EventHandler(this.btnBrowseWebFolder_Click);
         // 
         // txtWebSiteBase
         // 
         this.txtWebSiteBase.Enabled = false;
         this.txtWebSiteBase.Location = new System.Drawing.Point(112, 45);
         this.txtWebSiteBase.Name = "txtWebSiteBase";
         this.txtWebSiteBase.ReadOnly = true;
         this.txtWebSiteBase.Size = new System.Drawing.Size(327, 20);
         this.txtWebSiteBase.TabIndex = 6;
         this.txtWebSiteBase.Validating += new System.ComponentModel.CancelEventHandler(this.txtWebSiteBase_Validating);
         // 
         // lbl2WebSiteDir
         // 
         this.lbl2WebSiteDir.AutoSize = true;
         this.lbl2WebSiteDir.Location = new System.Drawing.Point(26, 48);
         this.lbl2WebSiteDir.Name = "lbl2WebSiteDir";
         this.lbl2WebSiteDir.Size = new System.Drawing.Size(80, 13);
         this.lbl2WebSiteDir.TabIndex = 5;
         this.lbl2WebSiteDir.Text = "In the directory:";
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
         this.chkWebSiteGenerator.CheckedChanged += new System.EventHandler(this.chkWebSiteGenerator_CheckedChanged);
         // 
         // tabStartup
         // 
         this.tabStartup.Controls.Add(this.grpStartup);
         this.tabStartup.Location = new System.Drawing.Point(4, 22);
         this.tabStartup.Name = "tabStartup";
         this.tabStartup.Padding = new System.Windows.Forms.Padding(3);
         this.tabStartup.Size = new System.Drawing.Size(501, 303);
         this.tabStartup.TabIndex = 6;
         this.tabStartup.Text = "Startup";
         this.tabStartup.UseVisualStyleBackColor = true;
         // 
         // grpStartup
         // 
         this.grpStartup.Controls.Add(this.checkBoxWrapper1);
         this.grpStartup.Controls.Add(this.checkBoxWrapper2);
         this.grpStartup.Location = new System.Drawing.Point(6, 9);
         this.grpStartup.Name = "grpStartup";
         this.grpStartup.Size = new System.Drawing.Size(489, 50);
         this.grpStartup.TabIndex = 3;
         this.grpStartup.TabStop = false;
         this.grpStartup.Text = "Startup";
         // 
         // checkBoxWrapper1
         // 
         this.checkBoxWrapper1.AutoSize = true;
         this.checkBoxWrapper1.Location = new System.Drawing.Point(196, 20);
         this.checkBoxWrapper1.Name = "checkBoxWrapper1";
         this.checkBoxWrapper1.Size = new System.Drawing.Size(95, 17);
         this.checkBoxWrapper1.TabIndex = 1;
         this.checkBoxWrapper1.Text = "Run Minimized";
         this.checkBoxWrapper1.UseVisualStyleBackColor = true;
         // 
         // checkBoxWrapper2
         // 
         this.checkBoxWrapper2.AutoSize = true;
         this.checkBoxWrapper2.Location = new System.Drawing.Point(10, 20);
         this.checkBoxWrapper2.Name = "checkBoxWrapper2";
         this.checkBoxWrapper2.Size = new System.Drawing.Size(170, 17);
         this.checkBoxWrapper2.TabIndex = 0;
         this.checkBoxWrapper2.Text = "Auto Run on Windows Startup";
         this.checkBoxWrapper2.UseVisualStyleBackColor = true;
         // 
         // tabDefaults
         // 
         this.tabDefaults.Controls.Add(this.grpDecimalPlaces);
         this.tabDefaults.Controls.Add(this.grpDebugMessageLevel);
         this.tabDefaults.Controls.Add(this.grpFileExplorer);
         this.tabDefaults.Controls.Add(this.grpLogFileViewer);
         this.tabDefaults.Controls.Add(this.grpDefaultConfig);
         this.tabDefaults.Location = new System.Drawing.Point(4, 22);
         this.tabDefaults.Name = "tabDefaults";
         this.tabDefaults.Size = new System.Drawing.Size(501, 303);
         this.tabDefaults.TabIndex = 4;
         this.tabDefaults.Text = "Defaults";
         this.tabDefaults.UseVisualStyleBackColor = true;
         // 
         // grpDecimalPlaces
         // 
         this.grpDecimalPlaces.Controls.Add(this.udDecimalPlaces);
         this.grpDecimalPlaces.Controls.Add(this.labelWrapper1);
         this.grpDecimalPlaces.Location = new System.Drawing.Point(254, 233);
         this.grpDecimalPlaces.Name = "grpDecimalPlaces";
         this.grpDecimalPlaces.Size = new System.Drawing.Size(241, 60);
         this.grpDecimalPlaces.TabIndex = 4;
         this.grpDecimalPlaces.TabStop = false;
         this.grpDecimalPlaces.Text = "Decimal Places";
         // 
         // udDecimalPlaces
         // 
         this.udDecimalPlaces.Location = new System.Drawing.Point(130, 25);
         this.udDecimalPlaces.Name = "udDecimalPlaces";
         this.udDecimalPlaces.Size = new System.Drawing.Size(39, 20);
         this.udDecimalPlaces.TabIndex = 1;
         // 
         // labelWrapper1
         // 
         this.labelWrapper1.AutoSize = true;
         this.labelWrapper1.Location = new System.Drawing.Point(16, 28);
         this.labelWrapper1.Name = "labelWrapper1";
         this.labelWrapper1.Size = new System.Drawing.Size(108, 13);
         this.labelWrapper1.TabIndex = 0;
         this.labelWrapper1.Text = "Show PPD Decimals:";
         // 
         // grpDebugMessageLevel
         // 
         this.grpDebugMessageLevel.Controls.Add(this.cboMessageLevel);
         this.grpDebugMessageLevel.Controls.Add(this.label6);
         this.grpDebugMessageLevel.Location = new System.Drawing.Point(6, 233);
         this.grpDebugMessageLevel.Name = "grpDebugMessageLevel";
         this.grpDebugMessageLevel.Size = new System.Drawing.Size(241, 60);
         this.grpDebugMessageLevel.TabIndex = 3;
         this.grpDebugMessageLevel.TabStop = false;
         this.grpDebugMessageLevel.Text = "Messages";
         // 
         // cboMessageLevel
         // 
         this.cboMessageLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
         this.cboMessageLevel.FormattingEnabled = true;
         this.cboMessageLevel.Location = new System.Drawing.Point(139, 25);
         this.cboMessageLevel.Name = "cboMessageLevel";
         this.cboMessageLevel.Size = new System.Drawing.Size(75, 21);
         this.cboMessageLevel.TabIndex = 2;
         // 
         // label6
         // 
         this.label6.AutoSize = true;
         this.label6.Location = new System.Drawing.Point(16, 28);
         this.label6.Name = "label6";
         this.label6.Size = new System.Drawing.Size(117, 13);
         this.label6.TabIndex = 1;
         this.label6.Text = "Debug Message Level:";
         // 
         // grpFileExplorer
         // 
         this.grpFileExplorer.Controls.Add(this.btnBrowseFileExplorer);
         this.grpFileExplorer.Controls.Add(this.label4);
         this.grpFileExplorer.Controls.Add(this.txtFileExplorer);
         this.grpFileExplorer.Location = new System.Drawing.Point(6, 167);
         this.grpFileExplorer.Name = "grpFileExplorer";
         this.grpFileExplorer.Size = new System.Drawing.Size(489, 60);
         this.grpFileExplorer.TabIndex = 2;
         this.grpFileExplorer.TabStop = false;
         this.grpFileExplorer.Text = "External File Explorer";
         // 
         // btnBrowseFileExplorer
         // 
         this.btnBrowseFileExplorer.Location = new System.Drawing.Point(421, 23);
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
         this.label4.Location = new System.Drawing.Point(30, 28);
         this.label4.Name = "label4";
         this.label4.Size = new System.Drawing.Size(52, 13);
         this.label4.TabIndex = 0;
         this.label4.Text = "Filename:";
         // 
         // txtFileExplorer
         // 
         this.txtFileExplorer.Location = new System.Drawing.Point(88, 25);
         this.txtFileExplorer.Name = "txtFileExplorer";
         this.txtFileExplorer.Size = new System.Drawing.Size(327, 20);
         this.txtFileExplorer.TabIndex = 1;
         // 
         // grpLogFileViewer
         // 
         this.grpLogFileViewer.Controls.Add(this.btnBrowseLogViewer);
         this.grpLogFileViewer.Controls.Add(this.label3);
         this.grpLogFileViewer.Controls.Add(this.txtLogFileViewer);
         this.grpLogFileViewer.Location = new System.Drawing.Point(6, 101);
         this.grpLogFileViewer.Name = "grpLogFileViewer";
         this.grpLogFileViewer.Size = new System.Drawing.Size(489, 60);
         this.grpLogFileViewer.TabIndex = 1;
         this.grpLogFileViewer.TabStop = false;
         this.grpLogFileViewer.Text = "External Log File Viewer";
         // 
         // btnBrowseLogViewer
         // 
         this.btnBrowseLogViewer.Location = new System.Drawing.Point(421, 23);
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
         this.label3.Location = new System.Drawing.Point(30, 28);
         this.label3.Name = "label3";
         this.label3.Size = new System.Drawing.Size(52, 13);
         this.label3.TabIndex = 0;
         this.label3.Text = "Filename:";
         // 
         // txtLogFileViewer
         // 
         this.txtLogFileViewer.Location = new System.Drawing.Point(88, 25);
         this.txtLogFileViewer.Name = "txtLogFileViewer";
         this.txtLogFileViewer.Size = new System.Drawing.Size(327, 20);
         this.txtLogFileViewer.TabIndex = 1;
         // 
         // grpDefaultConfig
         // 
         this.grpDefaultConfig.Controls.Add(this.chkAutoSave);
         this.grpDefaultConfig.Controls.Add(this.chkDefaultConfig);
         this.grpDefaultConfig.Controls.Add(this.btnBrowseConfigFile);
         this.grpDefaultConfig.Controls.Add(this.txtDefaultConfigFile);
         this.grpDefaultConfig.Controls.Add(this.label1);
         this.grpDefaultConfig.Location = new System.Drawing.Point(6, 9);
         this.grpDefaultConfig.Name = "grpDefaultConfig";
         this.grpDefaultConfig.Size = new System.Drawing.Size(489, 86);
         this.grpDefaultConfig.TabIndex = 0;
         this.grpDefaultConfig.TabStop = false;
         this.grpDefaultConfig.Text = "Configuration File";
         // 
         // chkAutoSave
         // 
         this.chkAutoSave.AutoSize = true;
         this.chkAutoSave.Location = new System.Drawing.Point(294, 22);
         this.chkAutoSave.Name = "chkAutoSave";
         this.chkAutoSave.Size = new System.Drawing.Size(141, 17);
         this.chkAutoSave.TabIndex = 4;
         this.chkAutoSave.Text = "Auto Save Configuration";
         this.chkAutoSave.UseVisualStyleBackColor = true;
         // 
         // chkDefaultConfig
         // 
         this.chkDefaultConfig.AutoSize = true;
         this.chkDefaultConfig.Location = new System.Drawing.Point(13, 22);
         this.chkDefaultConfig.Name = "chkDefaultConfig";
         this.chkDefaultConfig.Size = new System.Drawing.Size(166, 17);
         this.chkDefaultConfig.TabIndex = 0;
         this.chkDefaultConfig.Text = "Use Default Configuration File";
         this.chkDefaultConfig.UseVisualStyleBackColor = true;
         this.chkDefaultConfig.CheckedChanged += new System.EventHandler(this.chkDefaultConfig_CheckedChanged);
         // 
         // btnBrowseConfigFile
         // 
         this.btnBrowseConfigFile.Enabled = false;
         this.btnBrowseConfigFile.Location = new System.Drawing.Point(421, 47);
         this.btnBrowseConfigFile.Name = "btnBrowseConfigFile";
         this.btnBrowseConfigFile.Size = new System.Drawing.Size(24, 23);
         this.btnBrowseConfigFile.TabIndex = 3;
         this.btnBrowseConfigFile.Text = "...";
         this.btnBrowseConfigFile.UseVisualStyleBackColor = true;
         this.btnBrowseConfigFile.Click += new System.EventHandler(this.btnBrowseConfigFile_Click);
         // 
         // txtDefaultConfigFile
         // 
         this.txtDefaultConfigFile.Enabled = false;
         this.txtDefaultConfigFile.Location = new System.Drawing.Point(88, 49);
         this.txtDefaultConfigFile.Name = "txtDefaultConfigFile";
         this.txtDefaultConfigFile.ReadOnly = true;
         this.txtDefaultConfigFile.Size = new System.Drawing.Size(327, 20);
         this.txtDefaultConfigFile.TabIndex = 2;
         // 
         // label1
         // 
         this.label1.AutoSize = true;
         this.label1.Location = new System.Drawing.Point(30, 52);
         this.label1.Name = "label1";
         this.label1.Size = new System.Drawing.Size(52, 13);
         this.label1.TabIndex = 1;
         this.label1.Text = "Filename:";
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
         this.grpReportSelections.Controls.Add(this.chkClientEuePause);
         this.grpReportSelections.Enabled = false;
         this.grpReportSelections.Location = new System.Drawing.Point(6, 179);
         this.grpReportSelections.Name = "grpReportSelections";
         this.grpReportSelections.Size = new System.Drawing.Size(489, 115);
         this.grpReportSelections.TabIndex = 1;
         this.grpReportSelections.TabStop = false;
         this.grpReportSelections.Text = "Report Selections";
         // 
         // chkClientEuePause
         // 
         this.chkClientEuePause.AutoSize = true;
         this.chkClientEuePause.Enabled = false;
         this.chkClientEuePause.Location = new System.Drawing.Point(13, 22);
         this.chkClientEuePause.Name = "chkClientEuePause";
         this.chkClientEuePause.Size = new System.Drawing.Size(166, 17);
         this.chkClientEuePause.TabIndex = 0;
         this.chkClientEuePause.Text = "Client EUE Pause Notification";
         this.chkClientEuePause.UseVisualStyleBackColor = true;
         // 
         // grpEmailSettings
         // 
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
         // btnTestEmail
         // 
         this.btnTestEmail.Enabled = false;
         this.btnTestEmail.Location = new System.Drawing.Point(350, 15);
         this.btnTestEmail.Name = "btnTestEmail";
         this.btnTestEmail.Size = new System.Drawing.Size(119, 26);
         this.btnTestEmail.TabIndex = 11;
         this.btnTestEmail.Text = "Send Test Email";
         this.btnTestEmail.UseVisualStyleBackColor = true;
         this.btnTestEmail.Click += new System.EventHandler(this.btnTestEmail_Click);
         // 
         // txtSmtpPassword
         // 
         this.txtSmtpPassword.Enabled = false;
         this.txtSmtpPassword.Location = new System.Drawing.Point(314, 129);
         this.txtSmtpPassword.Name = "txtSmtpPassword";
         this.txtSmtpPassword.ReadOnly = true;
         this.txtSmtpPassword.Size = new System.Drawing.Size(155, 20);
         this.txtSmtpPassword.TabIndex = 10;
         this.txtSmtpPassword.UseSystemPasswordChar = true;
         this.txtSmtpPassword.Validating += new System.ComponentModel.CancelEventHandler(this.txtSmtpPassword_Validating);
         // 
         // txtSmtpUsername
         // 
         this.txtSmtpUsername.Enabled = false;
         this.txtSmtpUsername.Location = new System.Drawing.Point(92, 129);
         this.txtSmtpUsername.Name = "txtSmtpUsername";
         this.txtSmtpUsername.ReadOnly = true;
         this.txtSmtpUsername.Size = new System.Drawing.Size(155, 20);
         this.txtSmtpUsername.TabIndex = 8;
         this.txtSmtpUsername.Validating += new System.ComponentModel.CancelEventHandler(this.txtSmtpUsername_Validating);
         // 
         // labelWrapper4
         // 
         this.labelWrapper4.AutoSize = true;
         this.labelWrapper4.Location = new System.Drawing.Point(253, 132);
         this.labelWrapper4.Name = "labelWrapper4";
         this.labelWrapper4.Size = new System.Drawing.Size(56, 13);
         this.labelWrapper4.TabIndex = 9;
         this.labelWrapper4.Text = "Password:";
         // 
         // labelWrapper5
         // 
         this.labelWrapper5.AutoSize = true;
         this.labelWrapper5.Location = new System.Drawing.Point(27, 132);
         this.labelWrapper5.Name = "labelWrapper5";
         this.labelWrapper5.Size = new System.Drawing.Size(58, 13);
         this.labelWrapper5.TabIndex = 7;
         this.labelWrapper5.Text = "Username:";
         // 
         // lblFromEmailAddress
         // 
         this.lblFromEmailAddress.AutoSize = true;
         this.lblFromEmailAddress.Location = new System.Drawing.Point(11, 80);
         this.lblFromEmailAddress.Name = "lblFromEmailAddress";
         this.lblFromEmailAddress.Size = new System.Drawing.Size(74, 13);
         this.lblFromEmailAddress.TabIndex = 3;
         this.lblFromEmailAddress.Text = "From Address:";
         // 
         // txtFromEmailAddress
         // 
         this.txtFromEmailAddress.Enabled = false;
         this.txtFromEmailAddress.Location = new System.Drawing.Point(92, 77);
         this.txtFromEmailAddress.Name = "txtFromEmailAddress";
         this.txtFromEmailAddress.ReadOnly = true;
         this.txtFromEmailAddress.Size = new System.Drawing.Size(377, 20);
         this.txtFromEmailAddress.TabIndex = 4;
         this.txtFromEmailAddress.MouseHover += new System.EventHandler(this.txtFromEmailAddress_MouseHover);
         this.txtFromEmailAddress.Validating += new System.ComponentModel.CancelEventHandler(this.txtFromEmailAddress_Validating);
         // 
         // chkEnableEmail
         // 
         this.chkEnableEmail.AutoSize = true;
         this.chkEnableEmail.Location = new System.Drawing.Point(13, 22);
         this.chkEnableEmail.Name = "chkEnableEmail";
         this.chkEnableEmail.Size = new System.Drawing.Size(136, 17);
         this.chkEnableEmail.TabIndex = 0;
         this.chkEnableEmail.Text = "Enable Email Reporting";
         this.chkEnableEmail.UseVisualStyleBackColor = true;
         this.chkEnableEmail.CheckedChanged += new System.EventHandler(this.chkEnableEmail_CheckedChanged);
         // 
         // lblSmtpServer
         // 
         this.lblSmtpServer.AutoSize = true;
         this.lblSmtpServer.Location = new System.Drawing.Point(11, 106);
         this.lblSmtpServer.Name = "lblSmtpServer";
         this.lblSmtpServer.Size = new System.Drawing.Size(74, 13);
         this.lblSmtpServer.TabIndex = 5;
         this.lblSmtpServer.Text = "SMTP Server:";
         // 
         // lblToAddress
         // 
         this.lblToAddress.AutoSize = true;
         this.lblToAddress.Location = new System.Drawing.Point(21, 54);
         this.lblToAddress.Name = "lblToAddress";
         this.lblToAddress.Size = new System.Drawing.Size(64, 13);
         this.lblToAddress.TabIndex = 1;
         this.lblToAddress.Text = "To Address:";
         // 
         // txtSmtpServer
         // 
         this.txtSmtpServer.Enabled = false;
         this.txtSmtpServer.Location = new System.Drawing.Point(92, 103);
         this.txtSmtpServer.Name = "txtSmtpServer";
         this.txtSmtpServer.ReadOnly = true;
         this.txtSmtpServer.Size = new System.Drawing.Size(377, 20);
         this.txtSmtpServer.TabIndex = 6;
         this.txtSmtpServer.Validating += new System.ComponentModel.CancelEventHandler(this.txtSmtpServer_Validating);
         // 
         // txtToEmailAddress
         // 
         this.txtToEmailAddress.Enabled = false;
         this.txtToEmailAddress.Location = new System.Drawing.Point(92, 51);
         this.txtToEmailAddress.Name = "txtToEmailAddress";
         this.txtToEmailAddress.ReadOnly = true;
         this.txtToEmailAddress.Size = new System.Drawing.Size(377, 20);
         this.txtToEmailAddress.TabIndex = 2;
         this.txtToEmailAddress.Validating += new System.ComponentModel.CancelEventHandler(this.txtToEmailAddress_Validating);
         // 
         // tabWeb
         // 
         this.tabWeb.BackColor = System.Drawing.SystemColors.Control;
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
         this.txtProjectDownloadUrl.Location = new System.Drawing.Point(56, 19);
         this.txtProjectDownloadUrl.Name = "txtProjectDownloadUrl";
         this.txtProjectDownloadUrl.Size = new System.Drawing.Size(423, 20);
         this.txtProjectDownloadUrl.TabIndex = 1;
         this.txtProjectDownloadUrl.Validating += new System.ComponentModel.CancelEventHandler(this.txtProjectDownloadUrl_Validating);
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
         this.txtEOCUserID.Location = new System.Drawing.Point(194, 17);
         this.txtEOCUserID.MaxLength = 9;
         this.txtEOCUserID.Name = "txtEOCUserID";
         this.txtEOCUserID.Size = new System.Drawing.Size(138, 20);
         this.txtEOCUserID.TabIndex = 3;
         this.txtEOCUserID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDigitsOnly_KeyPress);
         // 
         // txtStanfordTeamID
         // 
         this.txtStanfordTeamID.Location = new System.Drawing.Point(194, 69);
         this.txtStanfordTeamID.MaxLength = 9;
         this.txtStanfordTeamID.Name = "txtStanfordTeamID";
         this.txtStanfordTeamID.Size = new System.Drawing.Size(138, 20);
         this.txtStanfordTeamID.TabIndex = 5;
         this.txtStanfordTeamID.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDigitsOnly_KeyPress);
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
         this.txtStanfordUserID.Location = new System.Drawing.Point(194, 43);
         this.txtStanfordUserID.Name = "txtStanfordUserID";
         this.txtStanfordUserID.Size = new System.Drawing.Size(138, 20);
         this.txtStanfordUserID.TabIndex = 4;
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
         this.grpWebProxy.Size = new System.Drawing.Size(489, 122);
         this.grpWebProxy.TabIndex = 2;
         this.grpWebProxy.TabStop = false;
         this.grpWebProxy.Text = "Web Proxy Settings";
         // 
         // chkUseProxy
         // 
         this.chkUseProxy.AutoSize = true;
         this.chkUseProxy.Location = new System.Drawing.Point(6, 19);
         this.chkUseProxy.Name = "chkUseProxy";
         this.chkUseProxy.Size = new System.Drawing.Size(117, 17);
         this.chkUseProxy.TabIndex = 0;
         this.chkUseProxy.Text = "Use a Proxy Server";
         this.chkUseProxy.UseVisualStyleBackColor = true;
         this.chkUseProxy.CheckedChanged += new System.EventHandler(this.chkUseProxy_CheckedChanged);
         // 
         // chkUseProxyAuth
         // 
         this.chkUseProxyAuth.AutoSize = true;
         this.chkUseProxyAuth.Enabled = false;
         this.chkUseProxyAuth.Location = new System.Drawing.Point(25, 68);
         this.chkUseProxyAuth.Name = "chkUseProxyAuth";
         this.chkUseProxyAuth.Size = new System.Drawing.Size(205, 17);
         this.chkUseProxyAuth.TabIndex = 5;
         this.chkUseProxyAuth.Text = "Authenticate to the Web Proxy Server";
         this.chkUseProxyAuth.UseVisualStyleBackColor = true;
         this.chkUseProxyAuth.CheckedChanged += new System.EventHandler(this.chkUseProxyAuth_CheckedChanged);
         // 
         // txtProxyPass
         // 
         this.txtProxyPass.Enabled = false;
         this.txtProxyPass.Location = new System.Drawing.Point(327, 91);
         this.txtProxyPass.Name = "txtProxyPass";
         this.txtProxyPass.ReadOnly = true;
         this.txtProxyPass.Size = new System.Drawing.Size(155, 20);
         this.txtProxyPass.TabIndex = 9;
         this.txtProxyPass.UseSystemPasswordChar = true;
         this.txtProxyPass.Validating += new System.ComponentModel.CancelEventHandler(this.txtProxyPass_Validating);
         // 
         // txtProxyUser
         // 
         this.txtProxyUser.Enabled = false;
         this.txtProxyUser.Location = new System.Drawing.Point(104, 91);
         this.txtProxyUser.Name = "txtProxyUser";
         this.txtProxyUser.ReadOnly = true;
         this.txtProxyUser.Size = new System.Drawing.Size(155, 20);
         this.txtProxyUser.TabIndex = 7;
         this.txtProxyUser.Validating += new System.ComponentModel.CancelEventHandler(this.txtProxyUser_Validating);
         // 
         // txtProxyPort
         // 
         this.txtProxyPort.Enabled = false;
         this.txtProxyPort.Location = new System.Drawing.Point(389, 42);
         this.txtProxyPort.MaxLength = 5;
         this.txtProxyPort.Name = "txtProxyPort";
         this.txtProxyPort.ReadOnly = true;
         this.txtProxyPort.Size = new System.Drawing.Size(94, 20);
         this.txtProxyPort.TabIndex = 4;
         this.txtProxyPort.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtDigitsOnly_KeyPress);
         this.txtProxyPort.Validating += new System.ComponentModel.CancelEventHandler(this.txtProxyPort_Validating);
         // 
         // lbl3ProxyPass
         // 
         this.lbl3ProxyPass.AutoSize = true;
         this.lbl3ProxyPass.Location = new System.Drawing.Point(265, 94);
         this.lbl3ProxyPass.Name = "lbl3ProxyPass";
         this.lbl3ProxyPass.Size = new System.Drawing.Size(56, 13);
         this.lbl3ProxyPass.TabIndex = 8;
         this.lbl3ProxyPass.Text = "Password:";
         // 
         // txtProxyServer
         // 
         this.txtProxyServer.Enabled = false;
         this.txtProxyServer.Location = new System.Drawing.Point(98, 42);
         this.txtProxyServer.Name = "txtProxyServer";
         this.txtProxyServer.ReadOnly = true;
         this.txtProxyServer.Size = new System.Drawing.Size(250, 20);
         this.txtProxyServer.TabIndex = 2;
         this.txtProxyServer.Validating += new System.ComponentModel.CancelEventHandler(this.txtProxyServer_Validating);
         // 
         // lbl3ProxyUser
         // 
         this.lbl3ProxyUser.AutoSize = true;
         this.lbl3ProxyUser.Location = new System.Drawing.Point(40, 94);
         this.lbl3ProxyUser.Name = "lbl3ProxyUser";
         this.lbl3ProxyUser.Size = new System.Drawing.Size(58, 13);
         this.lbl3ProxyUser.TabIndex = 6;
         this.lbl3ProxyUser.Text = "Username:";
         // 
         // lbl3Port
         // 
         this.lbl3Port.AutoSize = true;
         this.lbl3Port.Location = new System.Drawing.Point(354, 45);
         this.lbl3Port.Name = "lbl3Port";
         this.lbl3Port.Size = new System.Drawing.Size(29, 13);
         this.lbl3Port.TabIndex = 3;
         this.lbl3Port.Text = "Port:";
         // 
         // lbl3Proxy
         // 
         this.lbl3Proxy.AutoSize = true;
         this.lbl3Proxy.Location = new System.Drawing.Point(22, 45);
         this.lbl3Proxy.Name = "lbl3Proxy";
         this.lbl3Proxy.Size = new System.Drawing.Size(70, 13);
         this.lbl3Proxy.TabIndex = 1;
         this.lbl3Proxy.Text = "Proxy Server:";
         // 
         // tabVisStyles
         // 
         this.tabVisStyles.BackColor = System.Drawing.SystemColors.Control;
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
         // lbl1Preview
         // 
         this.lbl1Preview.Location = new System.Drawing.Point(129, 6);
         this.lbl1Preview.Name = "lbl1Preview";
         this.lbl1Preview.Size = new System.Drawing.Size(67, 23);
         this.lbl1Preview.TabIndex = 2;
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
         // frmPreferences
         // 
         this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
         this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
         this.ClientSize = new System.Drawing.Size(532, 388);
         this.Controls.Add(this.tabControl1);
         this.Controls.Add(this.btnOK);
         this.Controls.Add(this.btnCancel);
         this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
         this.MaximizeBox = false;
         this.MinimizeBox = false;
         this.Name = "frmPreferences";
         this.ShowIcon = false;
         this.ShowInTaskbar = false;
         this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
         this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
         this.Text = "Preferences";
         this.Shown += new System.EventHandler(this.frmPreferences_Shown);
         this.tabControl1.ResumeLayout(false);
         this.tabSchdTasks.ResumeLayout(false);
         this.grpUpdateData.ResumeLayout(false);
         this.grpUpdateData.PerformLayout();
         this.grpHTMLOutput.ResumeLayout(false);
         this.grpHTMLOutput.PerformLayout();
         this.tabStartup.ResumeLayout(false);
         this.grpStartup.ResumeLayout(false);
         this.grpStartup.PerformLayout();
         this.tabDefaults.ResumeLayout(false);
         this.grpDecimalPlaces.ResumeLayout(false);
         this.grpDecimalPlaces.PerformLayout();
         ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).EndInit();
         this.grpDebugMessageLevel.ResumeLayout(false);
         this.grpDebugMessageLevel.PerformLayout();
         this.grpFileExplorer.ResumeLayout(false);
         this.grpFileExplorer.PerformLayout();
         this.grpLogFileViewer.ResumeLayout(false);
         this.grpLogFileViewer.PerformLayout();
         this.grpDefaultConfig.ResumeLayout(false);
         this.grpDefaultConfig.PerformLayout();
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
         this.ResumeLayout(false);

      }

      #endregion

      private Classes.GroupBoxWrapper grpUpdateData;
      private Classes.LabelWrapper lbl2SchedExplain;
      private Classes.ButtonWrapper btnCancel;
      private Classes.ButtonWrapper btnOK;
      private System.Windows.Forms.Panel pnl1CSSSample;
      private System.Windows.Forms.ListBox StyleList;
      private Classes.LabelWrapper lbl1Style;
      private Classes.LabelWrapper lbl1Preview;
      private Classes.GroupBoxWrapper grpHTMLOutput;
      private Classes.LabelWrapper lbl2WebSiteDir;
      private Classes.ButtonWrapper btnBrowseWebFolder;
      private Classes.LabelWrapper lbl2MinutesToGen;
      private System.Windows.Forms.FolderBrowserDialog locateWebFolder;
      private System.Windows.Forms.TabControl tabControl1;
      private System.Windows.Forms.TabPage tabWeb;
      private System.Windows.Forms.TabPage tabVisStyles;
      private System.Windows.Forms.TabPage tabSchdTasks;
      private Classes.TextBoxWrapper txtStanfordUserID;
      private Classes.TextBoxWrapper txtEOCUserID;
      private Classes.LabelWrapper lbl3StanfordUserID;
      private Classes.LabelWrapper lbl3EOCUserID;
      private System.Windows.Forms.LinkLabel linkEOC;
      private System.Windows.Forms.LinkLabel linkStanford;
      private System.Windows.Forms.LinkLabel linkTeam;
      private Classes.TextBoxWrapper txtStanfordTeamID;
      private Classes.LabelWrapper lbl3StanfordTeamID;
      private Classes.TextBoxWrapper txtCollectMinutes;
      private Classes.CheckBoxWrapper chkScheduled;
      private Classes.CheckBoxWrapper chkSynchronous;
      private Classes.TextBoxWrapper txtWebGenMinutes;
      private Classes.CheckBoxWrapper chkWebSiteGenerator;
      private Classes.TextBoxWrapper txtWebSiteBase;
      private Classes.GroupBoxWrapper grpWebStats;
      private Classes.GroupBoxWrapper grpWebProxy;
      private Classes.TextBoxWrapper txtProxyServer;
      private Classes.LabelWrapper lbl3Proxy;
      private Classes.TextBoxWrapper txtProxyPass;
      private Classes.TextBoxWrapper txtProxyUser;
      private Classes.TextBoxWrapper txtProxyPort;
      private Classes.LabelWrapper lbl3ProxyPass;
      private Classes.LabelWrapper lbl3ProxyUser;
      private Classes.LabelWrapper lbl3Port;
      private Classes.CheckBoxWrapper chkUseProxyAuth;
      private Classes.CheckBoxWrapper chkUseProxy;
      private Classes.LabelWrapper lbl2Collect;
      private Classes.CheckBoxWrapper chkOffline;
      private System.Windows.Forms.TabPage tabDefaults;
      private Classes.GroupBoxWrapper grpDefaultConfig;
      private Classes.CheckBoxWrapper chkDefaultConfig;
      private Classes.ButtonWrapper btnBrowseConfigFile;
      private Classes.TextBoxWrapper txtDefaultConfigFile;
      private Classes.LabelWrapper label1;
      private System.Windows.Forms.OpenFileDialog openConfigDialog;
      private Classes.LabelWrapper label2;
      private Classes.ComboBoxWrapper cboPpdCalc;
      private Classes.GroupBoxWrapper grpFileExplorer;
      private Classes.LabelWrapper label4;
      private Classes.TextBoxWrapper txtFileExplorer;
      private Classes.GroupBoxWrapper grpLogFileViewer;
      private Classes.LabelWrapper label3;
      private Classes.TextBoxWrapper txtLogFileViewer;
      private Classes.ButtonWrapper btnBrowseFileExplorer;
      private Classes.ButtonWrapper btnBrowseLogViewer;
      private Classes.GroupBoxWrapper grpProjectDownload;
      private Classes.TextBoxWrapper txtProjectDownloadUrl;
      private Classes.LabelWrapper label5;
      private Classes.RadioButtonWrapper radioSchedule;
      private Classes.RadioButtonWrapper radioFullRefresh;
      private Classes.GroupBoxWrapper grpDebugMessageLevel;
      private Classes.ComboBoxWrapper cboMessageLevel;
      private Classes.LabelWrapper label6;
      private Classes.CheckBoxWrapper chkAutoSave;
      private System.Windows.Forms.ToolTip toolTipPrefs;
      private HFM.Classes.GroupBoxWrapper grpDecimalPlaces;
      private HFM.Classes.LabelWrapper labelWrapper1;
      private System.Windows.Forms.NumericUpDown udDecimalPlaces;
      private HFM.Classes.CheckBoxWrapper chkShowUserStats;
      private HFM.Classes.CheckBoxWrapper chkDuplicateProject;
      private HFM.Classes.CheckBoxWrapper chkDuplicateUserID;
      private HFM.Classes.CheckBoxWrapper chkColorLog;
      private System.Windows.Forms.TabPage tabReporting;
      private HFM.Classes.GroupBoxWrapper grpEmailSettings;
      private HFM.Classes.LabelWrapper lblSmtpServer;
      private HFM.Classes.LabelWrapper lblToAddress;
      private HFM.Classes.TextBoxWrapper txtSmtpServer;
      private HFM.Classes.TextBoxWrapper txtToEmailAddress;
      private HFM.Classes.CheckBoxWrapper chkEnableEmail;
      private HFM.Classes.LabelWrapper lblFromEmailAddress;
      private HFM.Classes.TextBoxWrapper txtFromEmailAddress;
      private HFM.Classes.TextBoxWrapper txtSmtpPassword;
      private HFM.Classes.TextBoxWrapper txtSmtpUsername;
      private HFM.Classes.LabelWrapper labelWrapper4;
      private HFM.Classes.LabelWrapper labelWrapper5;
      private HFM.Classes.ButtonWrapper btnTestEmail;
      private HFM.Classes.GroupBoxWrapper grpReportSelections;
      private HFM.Classes.CheckBoxWrapper chkClientEuePause;
      private System.Windows.Forms.TabPage tabStartup;
      private HFM.Classes.GroupBoxWrapper grpStartup;
      private HFM.Classes.CheckBoxWrapper checkBoxWrapper1;
      private HFM.Classes.CheckBoxWrapper checkBoxWrapper2;
      private HFM.Classes.CheckBoxWrapper chkRunMinimized;
      private HFM.Classes.CheckBoxWrapper chkAutoRun;
      private HFM.Classes.CheckBoxWrapper chkFAHlog;
   }
}