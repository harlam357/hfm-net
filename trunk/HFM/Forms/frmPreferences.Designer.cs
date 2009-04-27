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
           this.grpUpdateData = new System.Windows.Forms.GroupBox();
           this.label2 = new System.Windows.Forms.Label();
           this.cboPpdCalc = new System.Windows.Forms.ComboBox();
           this.chkOffline = new System.Windows.Forms.CheckBox();
           this.lbl2Collect = new System.Windows.Forms.Label();
           this.txtCollectMinutes = new System.Windows.Forms.TextBox();
           this.lbl2SchedExplain = new System.Windows.Forms.Label();
           this.chkScheduled = new System.Windows.Forms.CheckBox();
           this.chkSynchronous = new System.Windows.Forms.CheckBox();
           this.btnCancel = new System.Windows.Forms.Button();
           this.btnOK = new System.Windows.Forms.Button();
           this.lbl1Preview = new System.Windows.Forms.Label();
           this.lbl1Style = new System.Windows.Forms.Label();
           this.pnl1CSSSample = new System.Windows.Forms.Panel();
           this.wbCssSample = new System.Windows.Forms.WebBrowser();
           this.StyleList = new System.Windows.Forms.ListBox();
           this.grpHTMLOutput = new System.Windows.Forms.GroupBox();
           this.txtWebGenMinutes = new System.Windows.Forms.TextBox();
           this.lbl2MinutesToGen = new System.Windows.Forms.Label();
           this.btnBrowseWebFolder = new System.Windows.Forms.Button();
           this.txtWebSiteBase = new System.Windows.Forms.TextBox();
           this.lbl2WebSiteDir = new System.Windows.Forms.Label();
           this.chkWebSiteGenerator = new System.Windows.Forms.CheckBox();
           this.locateWebFolder = new System.Windows.Forms.FolderBrowserDialog();
           this.tabControl1 = new System.Windows.Forms.TabControl();
           this.tabSchdTasks = new System.Windows.Forms.TabPage();
           this.tabDefaults = new System.Windows.Forms.TabPage();
           this.grpFileExplorer = new System.Windows.Forms.GroupBox();
           this.btnBrowseFileExplorer = new System.Windows.Forms.Button();
           this.label4 = new System.Windows.Forms.Label();
           this.txtFileExplorer = new System.Windows.Forms.TextBox();
           this.grpLogFileViewer = new System.Windows.Forms.GroupBox();
           this.btnBrowseLogViewer = new System.Windows.Forms.Button();
           this.label3 = new System.Windows.Forms.Label();
           this.txtLogFileViewer = new System.Windows.Forms.TextBox();
           this.grpDefaultConfig = new System.Windows.Forms.GroupBox();
           this.chkDefaultConfig = new System.Windows.Forms.CheckBox();
           this.btnBrowseConfigFile = new System.Windows.Forms.Button();
           this.txtDefaultConfigFile = new System.Windows.Forms.TextBox();
           this.label1 = new System.Windows.Forms.Label();
           this.tabWeb = new System.Windows.Forms.TabPage();
           this.grpProjectDownload = new System.Windows.Forms.GroupBox();
           this.txtProjectDownloadUrl = new System.Windows.Forms.TextBox();
           this.label5 = new System.Windows.Forms.Label();
           this.grpWebStats = new System.Windows.Forms.GroupBox();
           this.lbl3EOCUserID = new System.Windows.Forms.Label();
           this.lbl3StanfordUserID = new System.Windows.Forms.Label();
           this.linkTeam = new System.Windows.Forms.LinkLabel();
           this.txtEOCUserID = new System.Windows.Forms.TextBox();
           this.txtStanfordTeamID = new System.Windows.Forms.TextBox();
           this.lbl3StanfordTeamID = new System.Windows.Forms.Label();
           this.linkStanford = new System.Windows.Forms.LinkLabel();
           this.txtStanfordUserID = new System.Windows.Forms.TextBox();
           this.linkEOC = new System.Windows.Forms.LinkLabel();
           this.grpWebProxy = new System.Windows.Forms.GroupBox();
           this.chkUseProxy = new System.Windows.Forms.CheckBox();
           this.chkUseProxyAuth = new System.Windows.Forms.CheckBox();
           this.txtProxyPass = new System.Windows.Forms.TextBox();
           this.txtProxyUser = new System.Windows.Forms.TextBox();
           this.txtProxyPort = new System.Windows.Forms.TextBox();
           this.lbl3ProxyPass = new System.Windows.Forms.Label();
           this.txtProxyServer = new System.Windows.Forms.TextBox();
           this.lbl3ProxyUser = new System.Windows.Forms.Label();
           this.lbl3Port = new System.Windows.Forms.Label();
           this.lbl3Proxy = new System.Windows.Forms.Label();
           this.tabVisStyles = new System.Windows.Forms.TabPage();
           this.openConfigDialog = new System.Windows.Forms.OpenFileDialog();
           this.radioSchedule = new System.Windows.Forms.RadioButton();
           this.radioFullRefresh = new System.Windows.Forms.RadioButton();
           this.grpUpdateData.SuspendLayout();
           this.pnl1CSSSample.SuspendLayout();
           this.grpHTMLOutput.SuspendLayout();
           this.tabControl1.SuspendLayout();
           this.tabSchdTasks.SuspendLayout();
           this.tabDefaults.SuspendLayout();
           this.grpFileExplorer.SuspendLayout();
           this.grpLogFileViewer.SuspendLayout();
           this.grpDefaultConfig.SuspendLayout();
           this.tabWeb.SuspendLayout();
           this.grpProjectDownload.SuspendLayout();
           this.grpWebStats.SuspendLayout();
           this.grpWebProxy.SuspendLayout();
           this.tabVisStyles.SuspendLayout();
           this.SuspendLayout();
           // 
           // grpUpdateData
           // 
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
           this.grpUpdateData.Size = new System.Drawing.Size(485, 101);
           this.grpUpdateData.TabIndex = 0;
           this.grpUpdateData.TabStop = false;
           this.grpUpdateData.Text = "Update Data";
           // 
           // label2
           // 
           this.label2.AutoSize = true;
           this.label2.Location = new System.Drawing.Point(217, 67);
           this.label2.Name = "label2";
           this.label2.Size = new System.Drawing.Size(123, 13);
           this.label2.TabIndex = 8;
           this.label2.Text = "Calculate PPD based on";
           // 
           // cboPpdCalc
           // 
           this.cboPpdCalc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
           this.cboPpdCalc.FormattingEnabled = true;
           this.cboPpdCalc.Location = new System.Drawing.Point(346, 64);
           this.cboPpdCalc.Name = "cboPpdCalc";
           this.cboPpdCalc.Size = new System.Drawing.Size(109, 21);
           this.cboPpdCalc.TabIndex = 7;
           // 
           // chkOffline
           // 
           this.chkOffline.AutoSize = true;
           this.chkOffline.Location = new System.Drawing.Point(10, 66);
           this.chkOffline.Name = "chkOffline";
           this.chkOffline.Size = new System.Drawing.Size(157, 17);
           this.chkOffline.TabIndex = 6;
           this.chkOffline.Text = "Always list offline clients last";
           this.chkOffline.UseVisualStyleBackColor = true;
           // 
           // lbl2Collect
           // 
           this.lbl2Collect.AutoSize = true;
           this.lbl2Collect.Location = new System.Drawing.Point(7, 19);
           this.lbl2Collect.Name = "lbl2Collect";
           this.lbl2Collect.Size = new System.Drawing.Size(94, 13);
           this.lbl2Collect.TabIndex = 5;
           this.lbl2Collect.Text = "Collect client data:";
           // 
           // txtCollectMinutes
           // 
           this.txtCollectMinutes.Location = new System.Drawing.Point(296, 38);
           this.txtCollectMinutes.MaxLength = 3;
           this.txtCollectMinutes.Name = "txtCollectMinutes";
           this.txtCollectMinutes.Size = new System.Drawing.Size(48, 20);
           this.txtCollectMinutes.TabIndex = 3;
           this.txtCollectMinutes.Text = "15";
           this.txtCollectMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
           this.txtCollectMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinutes_KeyPress);
           this.txtCollectMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinutes_Validating);
           // 
           // lbl2SchedExplain
           // 
           this.lbl2SchedExplain.Location = new System.Drawing.Point(343, 35);
           this.lbl2SchedExplain.Name = "lbl2SchedExplain";
           this.lbl2SchedExplain.Size = new System.Drawing.Size(136, 24);
           this.lbl2SchedExplain.TabIndex = 4;
           this.lbl2SchedExplain.Text = "minutes while running";
           this.lbl2SchedExplain.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
           // 
           // chkScheduled
           // 
           this.chkScheduled.Checked = true;
           this.chkScheduled.CheckState = System.Windows.Forms.CheckState.Checked;
           this.chkScheduled.Location = new System.Drawing.Point(246, 36);
           this.chkScheduled.Name = "chkScheduled";
           this.chkScheduled.Size = new System.Drawing.Size(53, 24);
           this.chkScheduled.TabIndex = 2;
           this.chkScheduled.Text = "Every";
           this.chkScheduled.UseVisualStyleBackColor = true;
           this.chkScheduled.CheckedChanged += new System.EventHandler(this.checkBox2_CheckedChanged);
           // 
           // chkSynchronous
           // 
           this.chkSynchronous.Location = new System.Drawing.Point(10, 36);
           this.chkSynchronous.Name = "chkSynchronous";
           this.chkSynchronous.Size = new System.Drawing.Size(221, 24);
           this.chkSynchronous.TabIndex = 1;
           this.chkSynchronous.Text = "In series (synchronous retrieval)";
           this.chkSynchronous.UseVisualStyleBackColor = true;
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
           // pnl1CSSSample
           // 
           this.pnl1CSSSample.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
           this.pnl1CSSSample.Controls.Add(this.wbCssSample);
           this.pnl1CSSSample.Location = new System.Drawing.Point(132, 31);
           this.pnl1CSSSample.Name = "pnl1CSSSample";
           this.pnl1CSSSample.Size = new System.Drawing.Size(358, 212);
           this.pnl1CSSSample.TabIndex = 1;
           // 
           // wbCssSample
           // 
           this.wbCssSample.Dock = System.Windows.Forms.DockStyle.Fill;
           this.wbCssSample.Location = new System.Drawing.Point(0, 0);
           this.wbCssSample.MinimumSize = new System.Drawing.Size(20, 20);
           this.wbCssSample.Name = "wbCssSample";
           this.wbCssSample.Size = new System.Drawing.Size(354, 208);
           this.wbCssSample.TabIndex = 0;
           this.wbCssSample.TabStop = false;
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
           // grpHTMLOutput
           // 
           this.grpHTMLOutput.Controls.Add(this.radioFullRefresh);
           this.grpHTMLOutput.Controls.Add(this.radioSchedule);
           this.grpHTMLOutput.Controls.Add(this.txtWebGenMinutes);
           this.grpHTMLOutput.Controls.Add(this.lbl2MinutesToGen);
           this.grpHTMLOutput.Controls.Add(this.btnBrowseWebFolder);
           this.grpHTMLOutput.Controls.Add(this.txtWebSiteBase);
           this.grpHTMLOutput.Controls.Add(this.lbl2WebSiteDir);
           this.grpHTMLOutput.Controls.Add(this.chkWebSiteGenerator);
           this.grpHTMLOutput.Location = new System.Drawing.Point(6, 116);
           this.grpHTMLOutput.Name = "grpHTMLOutput";
           this.grpHTMLOutput.Size = new System.Drawing.Size(485, 76);
           this.grpHTMLOutput.TabIndex = 1;
           this.grpHTMLOutput.TabStop = false;
           this.grpHTMLOutput.Text = "HTML Output";
           // 
           // txtWebGenMinutes
           // 
           this.txtWebGenMinutes.Enabled = false;
           this.txtWebGenMinutes.Location = new System.Drawing.Point(204, 18);
           this.txtWebGenMinutes.MaxLength = 3;
           this.txtWebGenMinutes.Name = "txtWebGenMinutes";
           this.txtWebGenMinutes.ReadOnly = true;
           this.txtWebGenMinutes.Size = new System.Drawing.Size(48, 20);
           this.txtWebGenMinutes.TabIndex = 1;
           this.txtWebGenMinutes.Text = "15";
           this.txtWebGenMinutes.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
           this.txtWebGenMinutes.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txtMinutes_KeyPress);
           this.txtWebGenMinutes.Validating += new System.ComponentModel.CancelEventHandler(this.txtMinutes_Validating);
           // 
           // lbl2MinutesToGen
           // 
           this.lbl2MinutesToGen.AutoSize = true;
           this.lbl2MinutesToGen.Location = new System.Drawing.Point(256, 21);
           this.lbl2MinutesToGen.Name = "lbl2MinutesToGen";
           this.lbl2MinutesToGen.Size = new System.Drawing.Size(44, 13);
           this.lbl2MinutesToGen.TabIndex = 2;
           this.lbl2MinutesToGen.Text = "Minutes";
           // 
           // btnBrowseWebFolder
           // 
           this.btnBrowseWebFolder.Enabled = false;
           this.btnBrowseWebFolder.Location = new System.Drawing.Point(445, 43);
           this.btnBrowseWebFolder.Name = "btnBrowseWebFolder";
           this.btnBrowseWebFolder.Size = new System.Drawing.Size(24, 23);
           this.btnBrowseWebFolder.TabIndex = 5;
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
           this.txtWebSiteBase.TabIndex = 4;
           // 
           // lbl2WebSiteDir
           // 
           this.lbl2WebSiteDir.AutoSize = true;
           this.lbl2WebSiteDir.Location = new System.Drawing.Point(26, 48);
           this.lbl2WebSiteDir.Name = "lbl2WebSiteDir";
           this.lbl2WebSiteDir.Size = new System.Drawing.Size(80, 13);
           this.lbl2WebSiteDir.TabIndex = 3;
           this.lbl2WebSiteDir.Text = "In the directory:";
           // 
           // chkWebSiteGenerator
           // 
           this.chkWebSiteGenerator.AutoSize = true;
           this.chkWebSiteGenerator.Location = new System.Drawing.Point(7, 20);
           this.chkWebSiteGenerator.Name = "chkWebSiteGenerator";
           this.chkWebSiteGenerator.Size = new System.Drawing.Size(113, 17);
           this.chkWebSiteGenerator.TabIndex = 0;
           this.chkWebSiteGenerator.Text = "Create a Web Site";
           this.chkWebSiteGenerator.UseVisualStyleBackColor = true;
           this.chkWebSiteGenerator.CheckedChanged += new System.EventHandler(this.chkWebSiteGenerator_CheckedChanged);
           // 
           // tabControl1
           // 
           this.tabControl1.Controls.Add(this.tabSchdTasks);
           this.tabControl1.Controls.Add(this.tabDefaults);
           this.tabControl1.Controls.Add(this.tabWeb);
           this.tabControl1.Controls.Add(this.tabVisStyles);
           this.tabControl1.HotTrack = true;
           this.tabControl1.Location = new System.Drawing.Point(13, 13);
           this.tabControl1.Multiline = true;
           this.tabControl1.Name = "tabControl1";
           this.tabControl1.SelectedIndex = 0;
           this.tabControl1.Size = new System.Drawing.Size(509, 329);
           this.tabControl1.TabIndex = 5;
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
           // tabDefaults
           // 
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
           // grpFileExplorer
           // 
           this.grpFileExplorer.Controls.Add(this.btnBrowseFileExplorer);
           this.grpFileExplorer.Controls.Add(this.label4);
           this.grpFileExplorer.Controls.Add(this.txtFileExplorer);
           this.grpFileExplorer.Location = new System.Drawing.Point(6, 167);
           this.grpFileExplorer.Name = "grpFileExplorer";
           this.grpFileExplorer.Size = new System.Drawing.Size(485, 60);
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
           this.grpLogFileViewer.Size = new System.Drawing.Size(485, 60);
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
           this.grpDefaultConfig.Controls.Add(this.chkDefaultConfig);
           this.grpDefaultConfig.Controls.Add(this.btnBrowseConfigFile);
           this.grpDefaultConfig.Controls.Add(this.txtDefaultConfigFile);
           this.grpDefaultConfig.Controls.Add(this.label1);
           this.grpDefaultConfig.Location = new System.Drawing.Point(6, 9);
           this.grpDefaultConfig.Name = "grpDefaultConfig";
           this.grpDefaultConfig.Size = new System.Drawing.Size(485, 86);
           this.grpDefaultConfig.TabIndex = 0;
           this.grpDefaultConfig.TabStop = false;
           this.grpDefaultConfig.Text = "Default Config File";
           // 
           // chkDefaultConfig
           // 
           this.chkDefaultConfig.AutoSize = true;
           this.chkDefaultConfig.Location = new System.Drawing.Point(13, 22);
           this.chkDefaultConfig.Name = "chkDefaultConfig";
           this.chkDefaultConfig.Size = new System.Drawing.Size(134, 17);
           this.chkDefaultConfig.TabIndex = 0;
           this.chkDefaultConfig.Text = "Use Default Config File";
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
           this.grpProjectDownload.TabIndex = 10;
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
           this.grpWebStats.TabIndex = 9;
           this.grpWebStats.TabStop = false;
           this.grpWebStats.Text = "Web Statistics";
           // 
           // lbl3EOCUserID
           // 
           this.lbl3EOCUserID.AutoSize = true;
           this.lbl3EOCUserID.Location = new System.Drawing.Point(2, 20);
           this.lbl3EOCUserID.Name = "lbl3EOCUserID";
           this.lbl3EOCUserID.Size = new System.Drawing.Size(153, 13);
           this.lbl3EOCUserID.TabIndex = 0;
           this.lbl3EOCUserID.Text = "Extreme Overclocking User ID:";
           // 
           // lbl3StanfordUserID
           // 
           this.lbl3StanfordUserID.AutoSize = true;
           this.lbl3StanfordUserID.Location = new System.Drawing.Point(2, 46);
           this.lbl3StanfordUserID.Name = "lbl3StanfordUserID";
           this.lbl3StanfordUserID.Size = new System.Drawing.Size(89, 13);
           this.lbl3StanfordUserID.TabIndex = 1;
           this.lbl3StanfordUserID.Text = "Stanford User ID:";
           // 
           // linkTeam
           // 
           this.linkTeam.Location = new System.Drawing.Point(338, 72);
           this.linkTeam.Name = "linkTeam";
           this.linkTeam.Size = new System.Drawing.Size(70, 19);
           this.linkTeam.TabIndex = 8;
           this.linkTeam.TabStop = true;
           this.linkTeam.Text = "Test Team ID";
           this.linkTeam.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkTeam_LinkClicked);
           // 
           // txtEOCUserID
           // 
           this.txtEOCUserID.Location = new System.Drawing.Point(194, 17);
           this.txtEOCUserID.Name = "txtEOCUserID";
           this.txtEOCUserID.Size = new System.Drawing.Size(138, 20);
           this.txtEOCUserID.TabIndex = 3;
           // 
           // txtStanfordTeamID
           // 
           this.txtStanfordTeamID.Location = new System.Drawing.Point(194, 69);
           this.txtStanfordTeamID.Name = "txtStanfordTeamID";
           this.txtStanfordTeamID.Size = new System.Drawing.Size(138, 20);
           this.txtStanfordTeamID.TabIndex = 5;
           // 
           // lbl3StanfordTeamID
           // 
           this.lbl3StanfordTeamID.AutoSize = true;
           this.lbl3StanfordTeamID.Location = new System.Drawing.Point(2, 72);
           this.lbl3StanfordTeamID.Name = "lbl3StanfordTeamID";
           this.lbl3StanfordTeamID.Size = new System.Drawing.Size(94, 13);
           this.lbl3StanfordTeamID.TabIndex = 2;
           this.lbl3StanfordTeamID.Text = "Stanford Team ID:";
           // 
           // linkStanford
           // 
           this.linkStanford.Location = new System.Drawing.Point(338, 46);
           this.linkStanford.Name = "linkStanford";
           this.linkStanford.Size = new System.Drawing.Size(83, 19);
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
           this.linkEOC.Location = new System.Drawing.Point(338, 20);
           this.linkEOC.Name = "linkEOC";
           this.linkEOC.Size = new System.Drawing.Size(64, 19);
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
           this.grpWebProxy.TabIndex = 9;
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
           // 
           // txtProxyUser
           // 
           this.txtProxyUser.Enabled = false;
           this.txtProxyUser.Location = new System.Drawing.Point(104, 91);
           this.txtProxyUser.Name = "txtProxyUser";
           this.txtProxyUser.ReadOnly = true;
           this.txtProxyUser.Size = new System.Drawing.Size(155, 20);
           this.txtProxyUser.TabIndex = 7;
           // 
           // txtProxyPort
           // 
           this.txtProxyPort.Enabled = false;
           this.txtProxyPort.Location = new System.Drawing.Point(389, 42);
           this.txtProxyPort.Name = "txtProxyPort";
           this.txtProxyPort.ReadOnly = true;
           this.txtProxyPort.Size = new System.Drawing.Size(94, 20);
           this.txtProxyPort.TabIndex = 4;
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
           // openConfigDialog
           // 
           this.openConfigDialog.DefaultExt = "hfm";
           this.openConfigDialog.Filter = "HFM Configuration Files|*.hfm";
           this.openConfigDialog.RestoreDirectory = true;
           // 
           // radioSchedule
           // 
           this.radioSchedule.AutoSize = true;
           this.radioSchedule.Location = new System.Drawing.Point(151, 19);
           this.radioSchedule.Name = "radioSchedule";
           this.radioSchedule.Size = new System.Drawing.Size(52, 17);
           this.radioSchedule.TabIndex = 6;
           this.radioSchedule.TabStop = true;
           this.radioSchedule.Text = "Every";
           this.radioSchedule.UseVisualStyleBackColor = true;
           this.radioSchedule.CheckedChanged += new System.EventHandler(this.radioSchedule_CheckedChanged);
           // 
           // radioFullRefresh
           // 
           this.radioFullRefresh.AutoSize = true;
           this.radioFullRefresh.Location = new System.Drawing.Point(319, 19);
           this.radioFullRefresh.Name = "radioFullRefresh";
           this.radioFullRefresh.Size = new System.Drawing.Size(125, 17);
           this.radioFullRefresh.TabIndex = 7;
           this.radioFullRefresh.TabStop = true;
           this.radioFullRefresh.Text = "After each full refresh";
           this.radioFullRefresh.UseVisualStyleBackColor = true;
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
           this.grpUpdateData.ResumeLayout(false);
           this.grpUpdateData.PerformLayout();
           this.pnl1CSSSample.ResumeLayout(false);
           this.grpHTMLOutput.ResumeLayout(false);
           this.grpHTMLOutput.PerformLayout();
           this.tabControl1.ResumeLayout(false);
           this.tabSchdTasks.ResumeLayout(false);
           this.tabDefaults.ResumeLayout(false);
           this.grpFileExplorer.ResumeLayout(false);
           this.grpFileExplorer.PerformLayout();
           this.grpLogFileViewer.ResumeLayout(false);
           this.grpLogFileViewer.PerformLayout();
           this.grpDefaultConfig.ResumeLayout(false);
           this.grpDefaultConfig.PerformLayout();
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

        private System.Windows.Forms.GroupBox grpUpdateData;
        private System.Windows.Forms.Label lbl2SchedExplain;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnl1CSSSample;
        private System.Windows.Forms.ListBox StyleList;
        private System.Windows.Forms.WebBrowser wbCssSample;
        private System.Windows.Forms.Label lbl1Style;
        private System.Windows.Forms.Label lbl1Preview;
        private System.Windows.Forms.GroupBox grpHTMLOutput;
        private System.Windows.Forms.Label lbl2WebSiteDir;
        private System.Windows.Forms.Button btnBrowseWebFolder;
        private System.Windows.Forms.Label lbl2MinutesToGen;
        private System.Windows.Forms.FolderBrowserDialog locateWebFolder;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabWeb;
        private System.Windows.Forms.TabPage tabVisStyles;
        private System.Windows.Forms.TabPage tabSchdTasks;
        private System.Windows.Forms.TextBox txtStanfordUserID;
        private System.Windows.Forms.TextBox txtEOCUserID;
        private System.Windows.Forms.Label lbl3StanfordUserID;
        private System.Windows.Forms.Label lbl3EOCUserID;
        private System.Windows.Forms.LinkLabel linkEOC;
        private System.Windows.Forms.LinkLabel linkStanford;
        private System.Windows.Forms.LinkLabel linkTeam;
        private System.Windows.Forms.TextBox txtStanfordTeamID;
        private System.Windows.Forms.Label lbl3StanfordTeamID;
        private System.Windows.Forms.TextBox txtCollectMinutes;
        private System.Windows.Forms.CheckBox chkScheduled;
        private System.Windows.Forms.CheckBox chkSynchronous;
        private System.Windows.Forms.TextBox txtWebGenMinutes;
        private System.Windows.Forms.CheckBox chkWebSiteGenerator;
        private System.Windows.Forms.TextBox txtWebSiteBase;
        private System.Windows.Forms.GroupBox grpWebStats;
        private System.Windows.Forms.GroupBox grpWebProxy;
        private System.Windows.Forms.TextBox txtProxyServer;
        private System.Windows.Forms.Label lbl3Proxy;
        private System.Windows.Forms.TextBox txtProxyPass;
        private System.Windows.Forms.TextBox txtProxyUser;
        private System.Windows.Forms.TextBox txtProxyPort;
        private System.Windows.Forms.Label lbl3ProxyPass;
        private System.Windows.Forms.Label lbl3ProxyUser;
        private System.Windows.Forms.Label lbl3Port;
        private System.Windows.Forms.CheckBox chkUseProxyAuth;
        private System.Windows.Forms.CheckBox chkUseProxy;
        private System.Windows.Forms.Label lbl2Collect;
       private System.Windows.Forms.CheckBox chkOffline;
       private System.Windows.Forms.TabPage tabDefaults;
       private System.Windows.Forms.GroupBox grpDefaultConfig;
       private System.Windows.Forms.CheckBox chkDefaultConfig;
       private System.Windows.Forms.Button btnBrowseConfigFile;
       private System.Windows.Forms.TextBox txtDefaultConfigFile;
       private System.Windows.Forms.Label label1;
       private System.Windows.Forms.OpenFileDialog openConfigDialog;
       private System.Windows.Forms.Label label2;
       private System.Windows.Forms.ComboBox cboPpdCalc;
       private System.Windows.Forms.GroupBox grpFileExplorer;
       private System.Windows.Forms.Label label4;
       private System.Windows.Forms.TextBox txtFileExplorer;
       private System.Windows.Forms.GroupBox grpLogFileViewer;
       private System.Windows.Forms.Label label3;
       private System.Windows.Forms.TextBox txtLogFileViewer;
       private System.Windows.Forms.Button btnBrowseFileExplorer;
       private System.Windows.Forms.Button btnBrowseLogViewer;
       private System.Windows.Forms.GroupBox grpProjectDownload;
       private System.Windows.Forms.TextBox txtProjectDownloadUrl;
       private System.Windows.Forms.Label label5;
       private System.Windows.Forms.RadioButton radioSchedule;
       private System.Windows.Forms.RadioButton radioFullRefresh;
    }
}