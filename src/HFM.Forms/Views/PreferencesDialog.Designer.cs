
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSchdTasks = new System.Windows.Forms.TabPage();
            this.WebGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.CopyLabel = new System.Windows.Forms.Label();
            this.WebSitePortTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.toolTipPrefs = new System.Windows.Forms.ToolTip(this.components);
            this.WebSitePortLabel = new System.Windows.Forms.Label();
            this.WebSitePasswordTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.WebSiteUsernameTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.WebSiteServerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.WebGenTypePanel = new HFM.Forms.Controls.RadioPanel();
            this.UploadTypeLabel = new System.Windows.Forms.Label();
            this.WebGenTypeFtpRadioButton = new System.Windows.Forms.RadioButton();
            this.WebGenTypePathRadioButton = new System.Windows.Forms.RadioButton();
            this.WebSitePasswordLabel = new System.Windows.Forms.Label();
            this.WebSiteUsernameLabel = new System.Windows.Forms.Label();
            this.WebSiteServerLabel = new System.Windows.Forms.Label();
            this.FtpModePanel = new HFM.Forms.Controls.RadioPanel();
            this.radioActive = new System.Windows.Forms.RadioButton();
            this.radioPassive = new System.Windows.Forms.RadioButton();
            this.lblFtpMode = new System.Windows.Forms.Label();
            this.udLimitSize = new System.Windows.Forms.NumericUpDown();
            this.chkLimitSize = new System.Windows.Forms.CheckBox();
            this.chkXml = new System.Windows.Forms.CheckBox();
            this.chkHtml = new System.Windows.Forms.CheckBox();
            this.TestConnectionButton = new System.Windows.Forms.Button();
            this.chkFAHlog = new System.Windows.Forms.CheckBox();
            this.radioFullRefresh = new System.Windows.Forms.RadioButton();
            this.radioSchedule = new System.Windows.Forms.RadioButton();
            this.txtWebGenMinutes = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl2MinutesToGen = new System.Windows.Forms.Label();
            this.BrowseWebFolderButton = new System.Windows.Forms.Button();
            this.WebSiteTargetPathTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.WebSiteTargetPathLabel = new System.Windows.Forms.Label();
            this.chkWebSiteGenerator = new System.Windows.Forms.CheckBox();
            this.OptionsTab = new System.Windows.Forms.TabPage();
            this.IdentityGroupBox = new System.Windows.Forms.GroupBox();
            this.EocUserStatsCheckBox = new System.Windows.Forms.CheckBox();
            this.EocUserIDLabel = new System.Windows.Forms.Label();
            this.FahUserIDLabel = new System.Windows.Forms.Label();
            this.TestFahTeamIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.EocUserIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.FahTeamIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.FahTeamIDLabel = new System.Windows.Forms.Label();
            this.TestFahUserIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.FahUserIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.TestEocUserIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.grpShowStyle = new System.Windows.Forms.GroupBox();
            this.cboShowStyle = new System.Windows.Forms.ComboBox();
            this.labelWrapper2 = new System.Windows.Forms.Label();
            this.LoggingGroupBox = new System.Windows.Forms.GroupBox();
            this.cboMessageLevel = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.ExternalProgramsGroupBox = new System.Windows.Forms.GroupBox();
            this.btnBrowseFileExplorer = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.FileExplorerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.btnBrowseLogViewer = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.LogFileViewerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.grpStartup = new System.Windows.Forms.GroupBox();
            this.chkCheckForUpdate = new System.Windows.Forms.CheckBox();
            this.chkAutoRun = new System.Windows.Forms.CheckBox();
            this.chkRunMinimized = new System.Windows.Forms.CheckBox();
            this.ClientsTab = new System.Windows.Forms.TabPage();
            this.RefreshClientDataGroupBox = new System.Windows.Forms.GroupBox();
            this.ClientRefreshIntervalTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl2SchedExplain = new System.Windows.Forms.Label();
            this.ClientRefreshIntervalCheckBox = new System.Windows.Forms.CheckBox();
            this.ClientSynchronousTextBox = new System.Windows.Forms.CheckBox();
            this.ConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.chkDefaultConfig = new System.Windows.Forms.CheckBox();
            this.btnBrowseConfigFile = new System.Windows.Forms.Button();
            this.txtDefaultConfigFile = new HFM.Forms.Controls.DataErrorTextBox();
            this.chkAutoSave = new System.Windows.Forms.CheckBox();
            this.grpInteractiveOptions = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.BonusCalculationComboBox = new System.Windows.Forms.ComboBox();
            this.labelWrapper6 = new System.Windows.Forms.Label();
            this.DuplicateProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.chkEtaAsDate = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.PpdCalculationComboBox = new System.Windows.Forms.ComboBox();
            this.chkOffline = new System.Windows.Forms.CheckBox();
            this.chkColorLog = new System.Windows.Forms.CheckBox();
            this.udDecimalPlaces = new System.Windows.Forms.NumericUpDown();
            this.labelWrapper1 = new System.Windows.Forms.Label();
            this.tabReporting = new System.Windows.Forms.TabPage();
            this.grpReportSelections = new System.Windows.Forms.GroupBox();
            this.grpEmailSettings = new System.Windows.Forms.GroupBox();
            this.txtSmtpServerPort = new HFM.Forms.Controls.DataErrorTextBox();
            this.labelWrapper3 = new System.Windows.Forms.Label();
            this.chkEmailSecure = new System.Windows.Forms.CheckBox();
            this.btnTestEmail = new System.Windows.Forms.Button();
            this.txtSmtpPassword = new HFM.Forms.Controls.DataErrorTextBox();
            this.txtSmtpUsername = new HFM.Forms.Controls.DataErrorTextBox();
            this.labelWrapper4 = new System.Windows.Forms.Label();
            this.labelWrapper5 = new System.Windows.Forms.Label();
            this.lblFromEmailAddress = new System.Windows.Forms.Label();
            this.txtFromEmailAddress = new HFM.Forms.Controls.DataErrorTextBox();
            this.chkEnableEmail = new System.Windows.Forms.CheckBox();
            this.lblSmtpServer = new System.Windows.Forms.Label();
            this.lblToAddress = new System.Windows.Forms.Label();
            this.txtSmtpServer = new HFM.Forms.Controls.DataErrorTextBox();
            this.txtToEmailAddress = new HFM.Forms.Controls.DataErrorTextBox();
            this.tabWeb = new System.Windows.Forms.TabPage();
            this.grpProjectDownload = new System.Windows.Forms.GroupBox();
            this.txtProjectDownloadUrl = new HFM.Forms.Controls.DataErrorTextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.grpWebProxy = new System.Windows.Forms.GroupBox();
            this.chkUseProxy = new System.Windows.Forms.CheckBox();
            this.chkUseProxyAuth = new System.Windows.Forms.CheckBox();
            this.txtProxyPass = new HFM.Forms.Controls.DataErrorTextBox();
            this.txtProxyUser = new HFM.Forms.Controls.DataErrorTextBox();
            this.txtProxyPort = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl3ProxyPass = new System.Windows.Forms.Label();
            this.txtProxyServer = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl3ProxyUser = new System.Windows.Forms.Label();
            this.lbl3Port = new System.Windows.Forms.Label();
            this.lbl3Proxy = new System.Windows.Forms.Label();
            this.tabVisStyles = new System.Windows.Forms.TabPage();
            this.btnInstanceBrowse = new System.Windows.Forms.Button();
            this.txtInstance = new HFM.Forms.Controls.DataErrorTextBox();
            this.SlotXsltLabel = new System.Windows.Forms.Label();
            this.btnSummaryBrowse = new System.Windows.Forms.Button();
            this.txtSummary = new HFM.Forms.Controls.DataErrorTextBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.btnOverviewBrowse = new System.Windows.Forms.Button();
            this.txtOverview = new HFM.Forms.Controls.DataErrorTextBox();
            this.lblOverview = new System.Windows.Forms.Label();
            this.lbl1Preview = new System.Windows.Forms.Label();
            this.lbl1Style = new System.Windows.Forms.Label();
            this.openConfigDialog = new System.Windows.Forms.OpenFileDialog();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabSchdTasks.SuspendLayout();
            this.WebGenerationGroupBox.SuspendLayout();
            this.WebGenTypePanel.SuspendLayout();
            this.FtpModePanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLimitSize)).BeginInit();
            this.OptionsTab.SuspendLayout();
            this.IdentityGroupBox.SuspendLayout();
            this.grpShowStyle.SuspendLayout();
            this.LoggingGroupBox.SuspendLayout();
            this.ExternalProgramsGroupBox.SuspendLayout();
            this.grpStartup.SuspendLayout();
            this.ClientsTab.SuspendLayout();
            this.RefreshClientDataGroupBox.SuspendLayout();
            this.ConfigurationGroupBox.SuspendLayout();
            this.grpInteractiveOptions.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).BeginInit();
            this.tabReporting.SuspendLayout();
            this.grpEmailSettings.SuspendLayout();
            this.tabWeb.SuspendLayout();
            this.grpProjectDownload.SuspendLayout();
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
            this.tabControl1.Controls.Add(this.OptionsTab);
            this.tabControl1.Controls.Add(this.ClientsTab);
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
            this.tabSchdTasks.Controls.Add(this.WebGenerationGroupBox);
            this.tabSchdTasks.Location = new System.Drawing.Point(4, 22);
            this.tabSchdTasks.Name = "tabSchdTasks";
            this.tabSchdTasks.Padding = new System.Windows.Forms.Padding(3);
            this.tabSchdTasks.Size = new System.Drawing.Size(501, 303);
            this.tabSchdTasks.TabIndex = 2;
            this.tabSchdTasks.Text = "Scheduled Tasks";
            this.tabSchdTasks.UseVisualStyleBackColor = true;
            // 
            // WebGenerationGroupBox
            // 
            this.WebGenerationGroupBox.Controls.Add(this.CopyLabel);
            this.WebGenerationGroupBox.Controls.Add(this.WebSitePortTextBox);
            this.WebGenerationGroupBox.Controls.Add(this.WebSitePortLabel);
            this.WebGenerationGroupBox.Controls.Add(this.WebSitePasswordTextBox);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteUsernameTextBox);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteServerTextBox);
            this.WebGenerationGroupBox.Controls.Add(this.WebGenTypePanel);
            this.WebGenerationGroupBox.Controls.Add(this.WebSitePasswordLabel);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteUsernameLabel);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteServerLabel);
            this.WebGenerationGroupBox.Controls.Add(this.FtpModePanel);
            this.WebGenerationGroupBox.Controls.Add(this.udLimitSize);
            this.WebGenerationGroupBox.Controls.Add(this.chkLimitSize);
            this.WebGenerationGroupBox.Controls.Add(this.chkXml);
            this.WebGenerationGroupBox.Controls.Add(this.chkHtml);
            this.WebGenerationGroupBox.Controls.Add(this.TestConnectionButton);
            this.WebGenerationGroupBox.Controls.Add(this.chkFAHlog);
            this.WebGenerationGroupBox.Controls.Add(this.radioFullRefresh);
            this.WebGenerationGroupBox.Controls.Add(this.radioSchedule);
            this.WebGenerationGroupBox.Controls.Add(this.txtWebGenMinutes);
            this.WebGenerationGroupBox.Controls.Add(this.lbl2MinutesToGen);
            this.WebGenerationGroupBox.Controls.Add(this.BrowseWebFolderButton);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteTargetPathTextBox);
            this.WebGenerationGroupBox.Controls.Add(this.WebSiteTargetPathLabel);
            this.WebGenerationGroupBox.Controls.Add(this.chkWebSiteGenerator);
            this.WebGenerationGroupBox.Location = new System.Drawing.Point(6, 6);
            this.WebGenerationGroupBox.Name = "WebGenerationGroupBox";
            this.WebGenerationGroupBox.Size = new System.Drawing.Size(489, 188);
            this.WebGenerationGroupBox.TabIndex = 0;
            this.WebGenerationGroupBox.TabStop = false;
            this.WebGenerationGroupBox.Text = "Web Generation";
            // 
            // CopyLabel
            // 
            this.CopyLabel.AutoSize = true;
            this.CopyLabel.Location = new System.Drawing.Point(7, 163);
            this.CopyLabel.Name = "CopyLabel";
            this.CopyLabel.Size = new System.Drawing.Size(34, 13);
            this.CopyLabel.TabIndex = 24;
            this.CopyLabel.Text = "Copy:";
            // 
            // WebSitePortTextBox
            // 
            this.WebSitePortTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.WebSitePortTextBox.DoubleBuffered = true;
            this.WebSitePortTextBox.Enabled = false;
            this.WebSitePortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.WebSitePortTextBox.ErrorToolTip = this.toolTipPrefs;
            this.WebSitePortTextBox.ErrorToolTipDuration = 5000;
            this.WebSitePortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.WebSitePortTextBox.ErrorToolTipText = "";
            this.WebSitePortTextBox.Location = new System.Drawing.Point(396, 104);
            this.WebSitePortTextBox.Name = "WebSitePortTextBox";
            this.WebSitePortTextBox.ReadOnly = true;
            this.WebSitePortTextBox.Size = new System.Drawing.Size(54, 20);
            this.WebSitePortTextBox.TabIndex = 13;
            // 
            // WebSitePortLabel
            // 
            this.WebSitePortLabel.AutoSize = true;
            this.WebSitePortLabel.Location = new System.Drawing.Point(364, 107);
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
            this.WebSitePasswordTextBox.ErrorToolTip = this.toolTipPrefs;
            this.WebSitePasswordTextBox.ErrorToolTipDuration = 5000;
            this.WebSitePasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.WebSitePasswordTextBox.ErrorToolTipText = "";
            this.WebSitePasswordTextBox.Location = new System.Drawing.Point(295, 132);
            this.WebSitePasswordTextBox.Name = "WebSitePasswordTextBox";
            this.WebSitePasswordTextBox.ReadOnly = true;
            this.WebSitePasswordTextBox.Size = new System.Drawing.Size(155, 20);
            this.WebSitePasswordTextBox.TabIndex = 17;
            this.WebSitePasswordTextBox.UseSystemPasswordChar = true;
            // 
            // WebSiteUsernameTextBox
            // 
            this.WebSiteUsernameTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.WebSiteUsernameTextBox.DoubleBuffered = true;
            this.WebSiteUsernameTextBox.Enabled = false;
            this.WebSiteUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.WebSiteUsernameTextBox.ErrorToolTip = this.toolTipPrefs;
            this.WebSiteUsernameTextBox.ErrorToolTipDuration = 5000;
            this.WebSiteUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.WebSiteUsernameTextBox.ErrorToolTipText = "";
            this.WebSiteUsernameTextBox.Location = new System.Drawing.Point(76, 132);
            this.WebSiteUsernameTextBox.Name = "WebSiteUsernameTextBox";
            this.WebSiteUsernameTextBox.ReadOnly = true;
            this.WebSiteUsernameTextBox.Size = new System.Drawing.Size(155, 20);
            this.WebSiteUsernameTextBox.TabIndex = 15;
            // 
            // WebSiteServerTextBox
            // 
            this.WebSiteServerTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.WebSiteServerTextBox.DoubleBuffered = true;
            this.WebSiteServerTextBox.Enabled = false;
            this.WebSiteServerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.WebSiteServerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.WebSiteServerTextBox.ErrorToolTipDuration = 5000;
            this.WebSiteServerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.WebSiteServerTextBox.ErrorToolTipText = "";
            this.WebSiteServerTextBox.Location = new System.Drawing.Point(76, 105);
            this.WebSiteServerTextBox.Name = "WebSiteServerTextBox";
            this.WebSiteServerTextBox.ReadOnly = true;
            this.WebSiteServerTextBox.Size = new System.Drawing.Size(282, 20);
            this.WebSiteServerTextBox.TabIndex = 11;
            // 
            // WebGenTypePanel
            // 
            this.WebGenTypePanel.Controls.Add(this.UploadTypeLabel);
            this.WebGenTypePanel.Controls.Add(this.WebGenTypeFtpRadioButton);
            this.WebGenTypePanel.Controls.Add(this.WebGenTypePathRadioButton);
            this.WebGenTypePanel.Location = new System.Drawing.Point(3, 46);
            this.WebGenTypePanel.Name = "WebGenTypePanel";
            this.WebGenTypePanel.Size = new System.Drawing.Size(178, 26);
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
            this.WebGenTypeFtpRadioButton.Location = new System.Drawing.Point(131, 4);
            this.WebGenTypeFtpRadioButton.Name = "WebGenTypeFtpRadioButton";
            this.WebGenTypeFtpRadioButton.Size = new System.Drawing.Size(45, 17);
            this.WebGenTypeFtpRadioButton.TabIndex = 2;
            this.WebGenTypeFtpRadioButton.TabStop = true;
            this.WebGenTypeFtpRadioButton.Tag = "1";
            this.WebGenTypeFtpRadioButton.Text = "FTP";
            this.WebGenTypeFtpRadioButton.UseVisualStyleBackColor = true;
            // 
            // WebGenTypePathRadioButton
            // 
            this.WebGenTypePathRadioButton.AutoSize = true;
            this.WebGenTypePathRadioButton.Location = new System.Drawing.Point(78, 4);
            this.WebGenTypePathRadioButton.Name = "WebGenTypePathRadioButton";
            this.WebGenTypePathRadioButton.Size = new System.Drawing.Size(47, 17);
            this.WebGenTypePathRadioButton.TabIndex = 1;
            this.WebGenTypePathRadioButton.TabStop = true;
            this.WebGenTypePathRadioButton.Tag = "0";
            this.WebGenTypePathRadioButton.Text = "Path";
            this.WebGenTypePathRadioButton.UseVisualStyleBackColor = true;
            // 
            // WebSitePasswordLabel
            // 
            this.WebSitePasswordLabel.AutoSize = true;
            this.WebSitePasswordLabel.Location = new System.Drawing.Point(237, 135);
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
            this.FtpModePanel.Location = new System.Drawing.Point(182, 46);
            this.FtpModePanel.Name = "FtpModePanel";
            this.FtpModePanel.Size = new System.Drawing.Size(211, 26);
            this.FtpModePanel.TabIndex = 18;
            this.FtpModePanel.ValueMember = null;
            // 
            // radioActive
            // 
            this.radioActive.AutoSize = true;
            this.radioActive.Location = new System.Drawing.Point(153, 4);
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
            this.radioPassive.Location = new System.Drawing.Point(85, 4);
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
            this.lblFtpMode.Size = new System.Drawing.Size(79, 13);
            this.lblFtpMode.TabIndex = 0;
            this.lblFtpMode.Text = "Transfer Mode:";
            // 
            // udLimitSize
            // 
            this.udLimitSize.Enabled = false;
            this.udLimitSize.Location = new System.Drawing.Point(356, 161);
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
            this.chkLimitSize.Location = new System.Drawing.Point(213, 162);
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
            this.chkXml.Location = new System.Drawing.Point(109, 162);
            this.chkXml.Name = "chkXml";
            this.chkXml.Size = new System.Drawing.Size(48, 17);
            this.chkXml.TabIndex = 20;
            this.chkXml.Text = "XML";
            this.chkXml.UseVisualStyleBackColor = true;
            // 
            // chkHtml
            // 
            this.chkHtml.AutoSize = true;
            this.chkHtml.Enabled = false;
            this.chkHtml.Location = new System.Drawing.Point(47, 162);
            this.chkHtml.Name = "chkHtml";
            this.chkHtml.Size = new System.Drawing.Size(56, 17);
            this.chkHtml.TabIndex = 19;
            this.chkHtml.Text = "HTML";
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
            this.chkFAHlog.Location = new System.Drawing.Point(163, 162);
            this.chkFAHlog.Name = "chkFAHlog";
            this.chkFAHlog.Size = new System.Drawing.Size(44, 17);
            this.chkFAHlog.TabIndex = 21;
            this.chkFAHlog.Text = "Log";
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
            // BrowseWebFolderButton
            // 
            this.BrowseWebFolderButton.Enabled = false;
            this.BrowseWebFolderButton.Location = new System.Drawing.Point(456, 76);
            this.BrowseWebFolderButton.Name = "BrowseWebFolderButton";
            this.BrowseWebFolderButton.Size = new System.Drawing.Size(24, 23);
            this.BrowseWebFolderButton.TabIndex = 9;
            this.BrowseWebFolderButton.Text = "...";
            this.BrowseWebFolderButton.UseVisualStyleBackColor = true;
            this.BrowseWebFolderButton.Click += new System.EventHandler(this.BrowseWebFolderButton_Click);
            // 
            // WebSiteTargetPathTextBox
            // 
            this.WebSiteTargetPathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.WebSiteTargetPathTextBox.DoubleBuffered = true;
            this.WebSiteTargetPathTextBox.Enabled = false;
            this.WebSiteTargetPathTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.WebSiteTargetPathTextBox.ErrorToolTip = this.toolTipPrefs;
            this.WebSiteTargetPathTextBox.ErrorToolTipDuration = 5000;
            this.WebSiteTargetPathTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
            this.WebSiteTargetPathTextBox.ErrorToolTipText = "";
            this.WebSiteTargetPathTextBox.Location = new System.Drawing.Point(76, 78);
            this.WebSiteTargetPathTextBox.Name = "WebSiteTargetPathTextBox";
            this.WebSiteTargetPathTextBox.ReadOnly = true;
            this.WebSiteTargetPathTextBox.Size = new System.Drawing.Size(374, 20);
            this.WebSiteTargetPathTextBox.TabIndex = 8;
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
            // OptionsTab
            // 
            this.OptionsTab.Controls.Add(this.IdentityGroupBox);
            this.OptionsTab.Controls.Add(this.grpShowStyle);
            this.OptionsTab.Controls.Add(this.LoggingGroupBox);
            this.OptionsTab.Controls.Add(this.ExternalProgramsGroupBox);
            this.OptionsTab.Controls.Add(this.grpStartup);
            this.OptionsTab.Location = new System.Drawing.Point(4, 22);
            this.OptionsTab.Name = "OptionsTab";
            this.OptionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.OptionsTab.Size = new System.Drawing.Size(501, 303);
            this.OptionsTab.TabIndex = 6;
            this.OptionsTab.Text = "Options";
            this.OptionsTab.UseVisualStyleBackColor = true;
            // 
            // IdentityGroupBox
            // 
            this.IdentityGroupBox.Controls.Add(this.EocUserStatsCheckBox);
            this.IdentityGroupBox.Controls.Add(this.EocUserIDLabel);
            this.IdentityGroupBox.Controls.Add(this.FahUserIDLabel);
            this.IdentityGroupBox.Controls.Add(this.TestFahTeamIDLinkLabel);
            this.IdentityGroupBox.Controls.Add(this.EocUserIDTextBox);
            this.IdentityGroupBox.Controls.Add(this.FahTeamIDTextBox);
            this.IdentityGroupBox.Controls.Add(this.FahTeamIDLabel);
            this.IdentityGroupBox.Controls.Add(this.TestFahUserIDLinkLabel);
            this.IdentityGroupBox.Controls.Add(this.FahUserIDTextBox);
            this.IdentityGroupBox.Controls.Add(this.TestEocUserIDLinkLabel);
            this.IdentityGroupBox.Location = new System.Drawing.Point(6, 65);
            this.IdentityGroupBox.Name = "IdentityGroupBox";
            this.IdentityGroupBox.Size = new System.Drawing.Size(489, 77);
            this.IdentityGroupBox.TabIndex = 8;
            this.IdentityGroupBox.TabStop = false;
            this.IdentityGroupBox.Text = "Identity";
            // 
            // EocUserStatsCheckBox
            // 
            this.EocUserStatsCheckBox.AutoSize = true;
            this.EocUserStatsCheckBox.Location = new System.Drawing.Point(250, 47);
            this.EocUserStatsCheckBox.Name = "EocUserStatsCheckBox";
            this.EocUserStatsCheckBox.Size = new System.Drawing.Size(194, 17);
            this.EocUserStatsCheckBox.TabIndex = 14;
            this.EocUserStatsCheckBox.Text = "Retrieve and Show EOC User Stats";
            this.EocUserStatsCheckBox.UseVisualStyleBackColor = true;
            // 
            // EocUserIDLabel
            // 
            this.EocUserIDLabel.AutoSize = true;
            this.EocUserIDLabel.Location = new System.Drawing.Point(247, 22);
            this.EocUserIDLabel.Name = "EocUserIDLabel";
            this.EocUserIDLabel.Size = new System.Drawing.Size(71, 13);
            this.EocUserIDLabel.TabIndex = 0;
            this.EocUserIDLabel.Text = "EOC User ID:";
            // 
            // FahUserIDLabel
            // 
            this.FahUserIDLabel.AutoSize = true;
            this.FahUserIDLabel.Location = new System.Drawing.Point(7, 22);
            this.FahUserIDLabel.Name = "FahUserIDLabel";
            this.FahUserIDLabel.Size = new System.Drawing.Size(70, 13);
            this.FahUserIDLabel.TabIndex = 1;
            this.FahUserIDLabel.Text = "FAH User ID:";
            // 
            // TestFahTeamIDLinkLabel
            // 
            this.TestFahTeamIDLinkLabel.AutoSize = true;
            this.TestFahTeamIDLinkLabel.Location = new System.Drawing.Point(211, 48);
            this.TestFahTeamIDLinkLabel.Name = "TestFahTeamIDLinkLabel";
            this.TestFahTeamIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.TestFahTeamIDLinkLabel.TabIndex = 8;
            this.TestFahTeamIDLinkLabel.TabStop = true;
            this.TestFahTeamIDLinkLabel.Text = "Test";
            this.TestFahTeamIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TestFahTeamIDLinkLabel_LinkClicked);
            // 
            // EocUserIDTextBox
            // 
            this.EocUserIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.EocUserIDTextBox.DoubleBuffered = true;
            this.EocUserIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.EocUserIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.EocUserIDTextBox.ErrorToolTipDuration = 5000;
            this.EocUserIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.EocUserIDTextBox.ErrorToolTipText = "";
            this.EocUserIDTextBox.Location = new System.Drawing.Point(325, 19);
            this.EocUserIDTextBox.MaxLength = 9;
            this.EocUserIDTextBox.Name = "EocUserIDTextBox";
            this.EocUserIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.EocUserIDTextBox.TabIndex = 3;
            // 
            // FahTeamIDTextBox
            // 
            this.FahTeamIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FahTeamIDTextBox.DoubleBuffered = true;
            this.FahTeamIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.FahTeamIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.FahTeamIDTextBox.ErrorToolTipDuration = 5000;
            this.FahTeamIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.FahTeamIDTextBox.ErrorToolTipText = "";
            this.FahTeamIDTextBox.Location = new System.Drawing.Point(85, 45);
            this.FahTeamIDTextBox.MaxLength = 9;
            this.FahTeamIDTextBox.Name = "FahTeamIDTextBox";
            this.FahTeamIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.FahTeamIDTextBox.TabIndex = 5;
            // 
            // FahTeamIDLabel
            // 
            this.FahTeamIDLabel.AutoSize = true;
            this.FahTeamIDLabel.Location = new System.Drawing.Point(7, 48);
            this.FahTeamIDLabel.Name = "FahTeamIDLabel";
            this.FahTeamIDLabel.Size = new System.Drawing.Size(75, 13);
            this.FahTeamIDLabel.TabIndex = 2;
            this.FahTeamIDLabel.Text = "FAH Team ID:";
            // 
            // TestFahUserIDLinkLabel
            // 
            this.TestFahUserIDLinkLabel.AutoSize = true;
            this.TestFahUserIDLinkLabel.Location = new System.Drawing.Point(211, 22);
            this.TestFahUserIDLinkLabel.Name = "TestFahUserIDLinkLabel";
            this.TestFahUserIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.TestFahUserIDLinkLabel.TabIndex = 7;
            this.TestFahUserIDLinkLabel.TabStop = true;
            this.TestFahUserIDLinkLabel.Text = "Test";
            this.TestFahUserIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TestFahUserIDLinkLabel_LinkClicked);
            // 
            // FahUserIDTextBox
            // 
            this.FahUserIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FahUserIDTextBox.DoubleBuffered = true;
            this.FahUserIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.FahUserIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.FahUserIDTextBox.ErrorToolTipDuration = 5000;
            this.FahUserIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.FahUserIDTextBox.ErrorToolTipText = "";
            this.FahUserIDTextBox.Location = new System.Drawing.Point(85, 19);
            this.FahUserIDTextBox.Name = "FahUserIDTextBox";
            this.FahUserIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.FahUserIDTextBox.TabIndex = 4;
            // 
            // TestEocUserIDLinkLabel
            // 
            this.TestEocUserIDLinkLabel.AutoSize = true;
            this.TestEocUserIDLinkLabel.Location = new System.Drawing.Point(451, 22);
            this.TestEocUserIDLinkLabel.Name = "TestEocUserIDLinkLabel";
            this.TestEocUserIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.TestEocUserIDLinkLabel.TabIndex = 6;
            this.TestEocUserIDLinkLabel.TabStop = true;
            this.TestEocUserIDLinkLabel.Text = "Test";
            this.TestEocUserIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.TestEocUserIDLinkLabel_LinkClicked);
            // 
            // grpShowStyle
            // 
            this.grpShowStyle.Controls.Add(this.cboShowStyle);
            this.grpShowStyle.Controls.Add(this.labelWrapper2);
            this.grpShowStyle.Location = new System.Drawing.Point(254, 238);
            this.grpShowStyle.Name = "grpShowStyle";
            this.grpShowStyle.Size = new System.Drawing.Size(241, 54);
            this.grpShowStyle.TabIndex = 7;
            this.grpShowStyle.TabStop = false;
            this.grpShowStyle.Text = "Docking Style";
            // 
            // cboShowStyle
            // 
            this.cboShowStyle.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboShowStyle.FormattingEnabled = true;
            this.cboShowStyle.Location = new System.Drawing.Point(132, 19);
            this.cboShowStyle.Name = "cboShowStyle";
            this.cboShowStyle.Size = new System.Drawing.Size(89, 21);
            this.cboShowStyle.TabIndex = 1;
            // 
            // labelWrapper2
            // 
            this.labelWrapper2.AutoSize = true;
            this.labelWrapper2.Location = new System.Drawing.Point(9, 24);
            this.labelWrapper2.Name = "labelWrapper2";
            this.labelWrapper2.Size = new System.Drawing.Size(117, 13);
            this.labelWrapper2.TabIndex = 0;
            this.labelWrapper2.Text = "Show HFM.NET in the:";
            // 
            // LoggingGroupBox
            // 
            this.LoggingGroupBox.Controls.Add(this.cboMessageLevel);
            this.LoggingGroupBox.Controls.Add(this.label6);
            this.LoggingGroupBox.Location = new System.Drawing.Point(6, 238);
            this.LoggingGroupBox.Name = "LoggingGroupBox";
            this.LoggingGroupBox.Size = new System.Drawing.Size(241, 54);
            this.LoggingGroupBox.TabIndex = 6;
            this.LoggingGroupBox.TabStop = false;
            this.LoggingGroupBox.Text = "Logging";
            // 
            // cboMessageLevel
            // 
            this.cboMessageLevel.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cboMessageLevel.FormattingEnabled = true;
            this.cboMessageLevel.Location = new System.Drawing.Point(92, 19);
            this.cboMessageLevel.Name = "cboMessageLevel";
            this.cboMessageLevel.Size = new System.Drawing.Size(75, 21);
            this.cboMessageLevel.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(77, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Logging Level:";
            // 
            // ExternalProgramsGroupBox
            // 
            this.ExternalProgramsGroupBox.Controls.Add(this.btnBrowseFileExplorer);
            this.ExternalProgramsGroupBox.Controls.Add(this.label4);
            this.ExternalProgramsGroupBox.Controls.Add(this.FileExplorerTextBox);
            this.ExternalProgramsGroupBox.Controls.Add(this.btnBrowseLogViewer);
            this.ExternalProgramsGroupBox.Controls.Add(this.label3);
            this.ExternalProgramsGroupBox.Controls.Add(this.LogFileViewerTextBox);
            this.ExternalProgramsGroupBox.Location = new System.Drawing.Point(6, 148);
            this.ExternalProgramsGroupBox.Name = "ExternalProgramsGroupBox";
            this.ExternalProgramsGroupBox.Size = new System.Drawing.Size(489, 84);
            this.ExternalProgramsGroupBox.TabIndex = 5;
            this.ExternalProgramsGroupBox.TabStop = false;
            this.ExternalProgramsGroupBox.Text = "External Programs";
            // 
            // btnBrowseFileExplorer
            // 
            this.btnBrowseFileExplorer.Location = new System.Drawing.Point(456, 49);
            this.btnBrowseFileExplorer.Name = "btnBrowseFileExplorer";
            this.btnBrowseFileExplorer.Size = new System.Drawing.Size(24, 23);
            this.btnBrowseFileExplorer.TabIndex = 5;
            this.btnBrowseFileExplorer.Text = "...";
            this.btnBrowseFileExplorer.UseVisualStyleBackColor = true;
            this.btnBrowseFileExplorer.Click += new System.EventHandler(this.btnBrowseFileExplorer_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(9, 55);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(67, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "File Explorer:";
            // 
            // FileExplorerTextBox
            // 
            this.FileExplorerTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.FileExplorerTextBox.DoubleBuffered = true;
            this.FileExplorerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.FileExplorerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.FileExplorerTextBox.ErrorToolTipDuration = 5000;
            this.FileExplorerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.FileExplorerTextBox.ErrorToolTipText = "";
            this.FileExplorerTextBox.Location = new System.Drawing.Point(96, 51);
            this.FileExplorerTextBox.Name = "FileExplorerTextBox";
            this.FileExplorerTextBox.Size = new System.Drawing.Size(354, 20);
            this.FileExplorerTextBox.TabIndex = 4;
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
            this.label3.Size = new System.Drawing.Size(82, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Log File Viewer:";
            // 
            // LogFileViewerTextBox
            // 
            this.LogFileViewerTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LogFileViewerTextBox.DoubleBuffered = true;
            this.LogFileViewerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.LogFileViewerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.LogFileViewerTextBox.ErrorToolTipDuration = 5000;
            this.LogFileViewerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.LogFileViewerTextBox.ErrorToolTipText = "";
            this.LogFileViewerTextBox.Location = new System.Drawing.Point(96, 21);
            this.LogFileViewerTextBox.Name = "LogFileViewerTextBox";
            this.LogFileViewerTextBox.Size = new System.Drawing.Size(354, 20);
            this.LogFileViewerTextBox.TabIndex = 1;
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
            // ClientsTab
            // 
            this.ClientsTab.Controls.Add(this.RefreshClientDataGroupBox);
            this.ClientsTab.Controls.Add(this.ConfigurationGroupBox);
            this.ClientsTab.Controls.Add(this.grpInteractiveOptions);
            this.ClientsTab.Location = new System.Drawing.Point(4, 22);
            this.ClientsTab.Name = "ClientsTab";
            this.ClientsTab.Size = new System.Drawing.Size(501, 303);
            this.ClientsTab.TabIndex = 4;
            this.ClientsTab.Text = "Clients";
            this.ClientsTab.UseVisualStyleBackColor = true;
            // 
            // RefreshClientDataGroupBox
            // 
            this.RefreshClientDataGroupBox.Controls.Add(this.ClientRefreshIntervalTextBox);
            this.RefreshClientDataGroupBox.Controls.Add(this.lbl2SchedExplain);
            this.RefreshClientDataGroupBox.Controls.Add(this.ClientRefreshIntervalCheckBox);
            this.RefreshClientDataGroupBox.Controls.Add(this.ClientSynchronousTextBox);
            this.RefreshClientDataGroupBox.Location = new System.Drawing.Point(6, 94);
            this.RefreshClientDataGroupBox.Name = "RefreshClientDataGroupBox";
            this.RefreshClientDataGroupBox.Size = new System.Drawing.Size(489, 54);
            this.RefreshClientDataGroupBox.TabIndex = 6;
            this.RefreshClientDataGroupBox.TabStop = false;
            this.RefreshClientDataGroupBox.Text = "Refresh Client Data";
            // 
            // txtCollectMinutes
            // 
            this.ClientRefreshIntervalTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.ClientRefreshIntervalTextBox.DoubleBuffered = true;
            this.ClientRefreshIntervalTextBox.Enabled = false;
            this.ClientRefreshIntervalTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.ClientRefreshIntervalTextBox.ErrorToolTip = this.toolTipPrefs;
            this.ClientRefreshIntervalTextBox.ErrorToolTipDuration = 5000;
            this.ClientRefreshIntervalTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.ClientRefreshIntervalTextBox.ErrorToolTipText = "";
            this.ClientRefreshIntervalTextBox.Location = new System.Drawing.Point(61, 20);
            this.ClientRefreshIntervalTextBox.MaxLength = 3;
            this.ClientRefreshIntervalTextBox.Name = "ClientRefreshIntervalTextBox";
            this.ClientRefreshIntervalTextBox.ReadOnly = true;
            this.ClientRefreshIntervalTextBox.Size = new System.Drawing.Size(39, 20);
            this.ClientRefreshIntervalTextBox.TabIndex = 4;
            this.ClientRefreshIntervalTextBox.Text = "15";
            this.ClientRefreshIntervalTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lbl2SchedExplain
            // 
            this.lbl2SchedExplain.AutoSize = true;
            this.lbl2SchedExplain.Location = new System.Drawing.Point(103, 24);
            this.lbl2SchedExplain.Name = "lbl2SchedExplain";
            this.lbl2SchedExplain.Size = new System.Drawing.Size(44, 13);
            this.lbl2SchedExplain.TabIndex = 5;
            this.lbl2SchedExplain.Text = "Minutes";
            // 
            // chkScheduled
            // 
            this.ClientRefreshIntervalCheckBox.AutoSize = true;
            this.ClientRefreshIntervalCheckBox.Location = new System.Drawing.Point(10, 22);
            this.ClientRefreshIntervalCheckBox.Name = "ClientRefreshIntervalCheckBox";
            this.ClientRefreshIntervalCheckBox.Size = new System.Drawing.Size(53, 17);
            this.ClientRefreshIntervalCheckBox.TabIndex = 3;
            this.ClientRefreshIntervalCheckBox.Text = "Every";
            this.ClientRefreshIntervalCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkSynchronous
            // 
            this.ClientSynchronousTextBox.AutoSize = true;
            this.ClientSynchronousTextBox.Location = new System.Drawing.Point(153, 22);
            this.ClientSynchronousTextBox.Name = "ClientSynchronousTextBox";
            this.ClientSynchronousTextBox.Size = new System.Drawing.Size(176, 17);
            this.ClientSynchronousTextBox.TabIndex = 0;
            this.ClientSynchronousTextBox.Text = "In Series (synchronous retrieval)";
            this.ClientSynchronousTextBox.UseVisualStyleBackColor = true;
            // 
            // ConfigurationGroupBox
            // 
            this.ConfigurationGroupBox.Controls.Add(this.chkDefaultConfig);
            this.ConfigurationGroupBox.Controls.Add(this.btnBrowseConfigFile);
            this.ConfigurationGroupBox.Controls.Add(this.txtDefaultConfigFile);
            this.ConfigurationGroupBox.Controls.Add(this.chkAutoSave);
            this.ConfigurationGroupBox.Location = new System.Drawing.Point(6, 9);
            this.ConfigurationGroupBox.Name = "ConfigurationGroupBox";
            this.ConfigurationGroupBox.Size = new System.Drawing.Size(489, 79);
            this.ConfigurationGroupBox.TabIndex = 5;
            this.ConfigurationGroupBox.TabStop = false;
            this.ConfigurationGroupBox.Text = "Configuration";
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
            this.txtDefaultConfigFile.ErrorToolTip = this.toolTipPrefs;
            this.txtDefaultConfigFile.ErrorToolTipDuration = 5000;
            this.txtDefaultConfigFile.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtDefaultConfigFile.ErrorToolTipText = "";
            this.txtDefaultConfigFile.Location = new System.Drawing.Point(10, 49);
            this.txtDefaultConfigFile.Name = "txtDefaultConfigFile";
            this.txtDefaultConfigFile.ReadOnly = true;
            this.txtDefaultConfigFile.Size = new System.Drawing.Size(440, 20);
            this.txtDefaultConfigFile.TabIndex = 2;
            // 
            // chkAutoSave
            // 
            this.chkAutoSave.AutoSize = true;
            this.chkAutoSave.Location = new System.Drawing.Point(153, 22);
            this.chkAutoSave.Name = "chkAutoSave";
            this.chkAutoSave.Size = new System.Drawing.Size(151, 17);
            this.chkAutoSave.TabIndex = 2;
            this.chkAutoSave.Text = "Auto Save when Changed";
            this.chkAutoSave.UseVisualStyleBackColor = true;
            // 
            // grpInteractiveOptions
            // 
            this.grpInteractiveOptions.Controls.Add(this.label1);
            this.grpInteractiveOptions.Controls.Add(this.BonusCalculationComboBox);
            this.grpInteractiveOptions.Controls.Add(this.labelWrapper6);
            this.grpInteractiveOptions.Controls.Add(this.DuplicateProjectCheckBox);
            this.grpInteractiveOptions.Controls.Add(this.chkEtaAsDate);
            this.grpInteractiveOptions.Controls.Add(this.label2);
            this.grpInteractiveOptions.Controls.Add(this.PpdCalculationComboBox);
            this.grpInteractiveOptions.Controls.Add(this.chkOffline);
            this.grpInteractiveOptions.Controls.Add(this.chkColorLog);
            this.grpInteractiveOptions.Controls.Add(this.udDecimalPlaces);
            this.grpInteractiveOptions.Controls.Add(this.labelWrapper1);
            this.grpInteractiveOptions.Location = new System.Drawing.Point(6, 154);
            this.grpInteractiveOptions.Name = "grpInteractiveOptions";
            this.grpInteractiveOptions.Size = new System.Drawing.Size(489, 130);
            this.grpInteractiveOptions.TabIndex = 0;
            this.grpInteractiveOptions.TabStop = false;
            this.grpInteractiveOptions.Text = "Display / Production Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(411, 72);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Decimals";
            // 
            // BonusCalculationComboBox
            // 
            this.BonusCalculationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.BonusCalculationComboBox.FormattingEnabled = true;
            this.BonusCalculationComboBox.Location = new System.Drawing.Point(367, 43);
            this.BonusCalculationComboBox.Name = "BonusCalculationComboBox";
            this.BonusCalculationComboBox.Size = new System.Drawing.Size(113, 21);
            this.BonusCalculationComboBox.TabIndex = 14;
            // 
            // labelWrapper6
            // 
            this.labelWrapper6.AutoSize = true;
            this.labelWrapper6.Location = new System.Drawing.Point(232, 47);
            this.labelWrapper6.Name = "labelWrapper6";
            this.labelWrapper6.Size = new System.Drawing.Size(131, 13);
            this.labelWrapper6.TabIndex = 13;
            this.labelWrapper6.Text = "Calculate Credit based on:";
            // 
            // DuplicateProjectCheckBox
            // 
            this.DuplicateProjectCheckBox.AutoSize = true;
            this.DuplicateProjectCheckBox.Location = new System.Drawing.Point(10, 73);
            this.DuplicateProjectCheckBox.Name = "DuplicateProjectCheckBox";
            this.DuplicateProjectCheckBox.Size = new System.Drawing.Size(183, 17);
            this.DuplicateProjectCheckBox.TabIndex = 10;
            this.DuplicateProjectCheckBox.Text = "Duplicate Project (R/C/G) Check";
            this.DuplicateProjectCheckBox.UseVisualStyleBackColor = true;
            // 
            // chkEtaAsDate
            // 
            this.chkEtaAsDate.AutoSize = true;
            this.chkEtaAsDate.Location = new System.Drawing.Point(10, 99);
            this.chkEtaAsDate.Name = "chkEtaAsDate";
            this.chkEtaAsDate.Size = new System.Drawing.Size(202, 17);
            this.chkEtaAsDate.TabIndex = 9;
            this.chkEtaAsDate.Text = "Show ETA value as a Date and Time";
            this.chkEtaAsDate.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 21);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(126, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Calculate PPD based on:";
            // 
            // PpdCalculationComboBox
            // 
            this.PpdCalculationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.PpdCalculationComboBox.FormattingEnabled = true;
            this.PpdCalculationComboBox.Location = new System.Drawing.Point(367, 16);
            this.PpdCalculationComboBox.Name = "PpdCalculationComboBox";
            this.PpdCalculationComboBox.Size = new System.Drawing.Size(113, 21);
            this.PpdCalculationComboBox.TabIndex = 5;
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
            this.chkColorLog.Location = new System.Drawing.Point(10, 47);
            this.chkColorLog.Name = "chkColorLog";
            this.chkColorLog.Size = new System.Drawing.Size(148, 17);
            this.chkColorLog.TabIndex = 1;
            this.chkColorLog.Text = "Color the Log Viewer Text";
            this.chkColorLog.UseVisualStyleBackColor = true;
            // 
            // udDecimalPlaces
            // 
            this.udDecimalPlaces.Location = new System.Drawing.Point(367, 69);
            this.udDecimalPlaces.Name = "udDecimalPlaces";
            this.udDecimalPlaces.Size = new System.Drawing.Size(39, 20);
            this.udDecimalPlaces.TabIndex = 7;
            // 
            // labelWrapper1
            // 
            this.labelWrapper1.AutoSize = true;
            this.labelWrapper1.Location = new System.Drawing.Point(297, 72);
            this.labelWrapper1.Name = "labelWrapper1";
            this.labelWrapper1.Size = new System.Drawing.Size(66, 13);
            this.labelWrapper1.TabIndex = 6;
            this.labelWrapper1.Text = "Calculate to:";
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
            this.grpReportSelections.Enabled = false;
            this.grpReportSelections.Location = new System.Drawing.Point(6, 179);
            this.grpReportSelections.Name = "grpReportSelections";
            this.grpReportSelections.Size = new System.Drawing.Size(489, 114);
            this.grpReportSelections.TabIndex = 1;
            this.grpReportSelections.TabStop = false;
            this.grpReportSelections.Text = "Report Selections";
            this.grpReportSelections.EnabledChanged += new System.EventHandler(this.grpReportSelections_EnabledChanged);
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
            // 
            // txtSmtpUsername
            // 
            this.txtSmtpUsername.BackColor = System.Drawing.SystemColors.Control;
            this.txtSmtpUsername.DoubleBuffered = true;
            this.txtSmtpUsername.Enabled = false;
            this.txtSmtpUsername.ErrorBackColor = System.Drawing.Color.Yellow;
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
            this.txtFromEmailAddress.ErrorToolTip = this.toolTipPrefs;
            this.txtFromEmailAddress.ErrorToolTipDuration = 5000;
            this.txtFromEmailAddress.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtFromEmailAddress.ErrorToolTipText = "";
            this.txtFromEmailAddress.Location = new System.Drawing.Point(92, 77);
            this.txtFromEmailAddress.MaxLength = 200;
            this.txtFromEmailAddress.Name = "txtFromEmailAddress";
            this.txtFromEmailAddress.ReadOnly = true;
            this.txtFromEmailAddress.Size = new System.Drawing.Size(377, 20);
            this.txtFromEmailAddress.TabIndex = 5;
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
            this.txtSmtpServer.ErrorToolTip = this.toolTipPrefs;
            this.txtSmtpServer.ErrorToolTipDuration = 5000;
            this.txtSmtpServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtSmtpServer.ErrorToolTipText = "";
            this.txtSmtpServer.Location = new System.Drawing.Point(92, 103);
            this.txtSmtpServer.MaxLength = 200;
            this.txtSmtpServer.Name = "txtSmtpServer";
            this.txtSmtpServer.ReadOnly = true;
            this.txtSmtpServer.Size = new System.Drawing.Size(282, 20);
            this.txtSmtpServer.TabIndex = 7;
            // 
            // txtToEmailAddress
            // 
            this.txtToEmailAddress.BackColor = System.Drawing.SystemColors.Control;
            this.txtToEmailAddress.DoubleBuffered = true;
            this.txtToEmailAddress.Enabled = false;
            this.txtToEmailAddress.ErrorBackColor = System.Drawing.Color.Yellow;
            this.txtToEmailAddress.ErrorToolTip = this.toolTipPrefs;
            this.txtToEmailAddress.ErrorToolTipDuration = 5000;
            this.txtToEmailAddress.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtToEmailAddress.ErrorToolTipText = "";
            this.txtToEmailAddress.Location = new System.Drawing.Point(92, 51);
            this.txtToEmailAddress.MaxLength = 200;
            this.txtToEmailAddress.Name = "txtToEmailAddress";
            this.txtToEmailAddress.ReadOnly = true;
            this.txtToEmailAddress.Size = new System.Drawing.Size(377, 20);
            this.txtToEmailAddress.TabIndex = 3;
            // 
            // tabWeb
            // 
            this.tabWeb.BackColor = System.Drawing.Color.Transparent;
            this.tabWeb.Controls.Add(this.grpProjectDownload);
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
            this.txtProjectDownloadUrl.ErrorToolTip = this.toolTipPrefs;
            this.txtProjectDownloadUrl.ErrorToolTipDuration = 5000;
            this.txtProjectDownloadUrl.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtProjectDownloadUrl.ErrorToolTipText = "";
            this.txtProjectDownloadUrl.Location = new System.Drawing.Point(56, 19);
            this.txtProjectDownloadUrl.Name = "txtProjectDownloadUrl";
            this.txtProjectDownloadUrl.Size = new System.Drawing.Size(423, 20);
            this.txtProjectDownloadUrl.TabIndex = 1;
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
            // 
            // txtProxyUser
            // 
            this.txtProxyUser.BackColor = System.Drawing.SystemColors.Control;
            this.txtProxyUser.DoubleBuffered = true;
            this.txtProxyUser.Enabled = false;
            this.txtProxyUser.ErrorBackColor = System.Drawing.Color.Yellow;
            this.txtProxyUser.ErrorToolTip = this.toolTipPrefs;
            this.txtProxyUser.ErrorToolTipDuration = 5000;
            this.txtProxyUser.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtProxyUser.ErrorToolTipText = "";
            this.txtProxyUser.Location = new System.Drawing.Point(104, 89);
            this.txtProxyUser.Name = "txtProxyUser";
            this.txtProxyUser.ReadOnly = true;
            this.txtProxyUser.Size = new System.Drawing.Size(155, 20);
            this.txtProxyUser.TabIndex = 7;
            // 
            // txtProxyPort
            // 
            this.txtProxyPort.BackColor = System.Drawing.SystemColors.Control;
            this.txtProxyPort.DoubleBuffered = true;
            this.txtProxyPort.Enabled = false;
            this.txtProxyPort.ErrorBackColor = System.Drawing.Color.Yellow;
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
            this.txtProxyServer.ErrorToolTip = this.toolTipPrefs;
            this.txtProxyServer.ErrorToolTipDuration = 5000;
            this.txtProxyServer.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtProxyServer.ErrorToolTipText = "";
            this.txtProxyServer.Location = new System.Drawing.Point(98, 40);
            this.txtProxyServer.Name = "txtProxyServer";
            this.txtProxyServer.ReadOnly = true;
            this.txtProxyServer.Size = new System.Drawing.Size(250, 20);
            this.txtProxyServer.TabIndex = 2;
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
            // btnInstanceBrowse
            // 
            this.btnInstanceBrowse.Location = new System.Drawing.Point(466, 221);
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
            this.txtInstance.ErrorToolTip = this.toolTipPrefs;
            this.txtInstance.ErrorToolTipDuration = 5000;
            this.txtInstance.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtInstance.ErrorToolTipText = "";
            this.txtInstance.Location = new System.Drawing.Point(132, 223);
            this.txtInstance.Name = "txtInstance";
            this.txtInstance.ReadOnly = true;
            this.txtInstance.Size = new System.Drawing.Size(328, 20);
            this.txtInstance.TabIndex = 17;
            // 
            // SlotXsltLabel
            // 
            this.SlotXsltLabel.AutoSize = true;
            this.SlotXsltLabel.Location = new System.Drawing.Point(68, 226);
            this.SlotXsltLabel.Name = "SlotXsltLabel";
            this.SlotXsltLabel.Size = new System.Drawing.Size(58, 13);
            this.SlotXsltLabel.TabIndex = 16;
            this.SlotXsltLabel.Text = "Slot XSLT:";
            this.SlotXsltLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // btnSummaryBrowse
            // 
            this.btnSummaryBrowse.Location = new System.Drawing.Point(466, 195);
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
            this.txtSummary.ErrorToolTip = this.toolTipPrefs;
            this.txtSummary.ErrorToolTipDuration = 5000;
            this.txtSummary.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtSummary.ErrorToolTipText = "";
            this.txtSummary.Location = new System.Drawing.Point(132, 197);
            this.txtSummary.Name = "txtSummary";
            this.txtSummary.ReadOnly = true;
            this.txtSummary.Size = new System.Drawing.Size(328, 20);
            this.txtSummary.TabIndex = 11;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(43, 200);
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
            this.txtOverview.ErrorToolTip = this.toolTipPrefs;
            this.txtOverview.ErrorToolTipDuration = 5000;
            this.txtOverview.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.txtOverview.ErrorToolTipText = "";
            this.txtOverview.Location = new System.Drawing.Point(132, 171);
            this.txtOverview.Name = "txtOverview";
            this.txtOverview.ReadOnly = true;
            this.txtOverview.Size = new System.Drawing.Size(328, 20);
            this.txtOverview.TabIndex = 5;
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
            this.btnOK.Location = new System.Drawing.Point(366, 352);
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
            this.btnCancel.Location = new System.Drawing.Point(447, 352);
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
            this.WebGenerationGroupBox.ResumeLayout(false);
            this.WebGenerationGroupBox.PerformLayout();
            this.WebGenTypePanel.ResumeLayout(false);
            this.WebGenTypePanel.PerformLayout();
            this.FtpModePanel.ResumeLayout(false);
            this.FtpModePanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udLimitSize)).EndInit();
            this.OptionsTab.ResumeLayout(false);
            this.IdentityGroupBox.ResumeLayout(false);
            this.IdentityGroupBox.PerformLayout();
            this.grpShowStyle.ResumeLayout(false);
            this.grpShowStyle.PerformLayout();
            this.LoggingGroupBox.ResumeLayout(false);
            this.LoggingGroupBox.PerformLayout();
            this.ExternalProgramsGroupBox.ResumeLayout(false);
            this.ExternalProgramsGroupBox.PerformLayout();
            this.grpStartup.ResumeLayout(false);
            this.grpStartup.PerformLayout();
            this.ClientsTab.ResumeLayout(false);
            this.RefreshClientDataGroupBox.ResumeLayout(false);
            this.RefreshClientDataGroupBox.PerformLayout();
            this.ConfigurationGroupBox.ResumeLayout(false);
            this.ConfigurationGroupBox.PerformLayout();
            this.grpInteractiveOptions.ResumeLayout(false);
            this.grpInteractiveOptions.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.udDecimalPlaces)).EndInit();
            this.tabReporting.ResumeLayout(false);
            this.grpEmailSettings.ResumeLayout(false);
            this.grpEmailSettings.PerformLayout();
            this.tabWeb.ResumeLayout(false);
            this.grpProjectDownload.ResumeLayout(false);
            this.grpProjectDownload.PerformLayout();
            this.grpWebProxy.ResumeLayout(false);
            this.grpWebProxy.PerformLayout();
            this.tabVisStyles.ResumeLayout(false);
            this.tabVisStyles.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Panel pnl1CSSSample;
        private System.Windows.Forms.ListBox StyleList;
        private System.Windows.Forms.Label lbl1Style;
        private System.Windows.Forms.Label lbl1Preview;
        private System.Windows.Forms.GroupBox WebGenerationGroupBox;
        private System.Windows.Forms.Label WebSiteTargetPathLabel;
        private System.Windows.Forms.Button BrowseWebFolderButton;
        private System.Windows.Forms.Label lbl2MinutesToGen;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabWeb;
        private System.Windows.Forms.TabPage tabVisStyles;
        private System.Windows.Forms.TabPage tabSchdTasks;
        private HFM.Forms.Controls.DataErrorTextBox txtWebGenMinutes;
        private System.Windows.Forms.CheckBox chkWebSiteGenerator;
        private HFM.Forms.Controls.DataErrorTextBox WebSiteTargetPathTextBox;
        private System.Windows.Forms.GroupBox grpWebProxy;
        private HFM.Forms.Controls.DataErrorTextBox txtProxyServer;
        private System.Windows.Forms.Label lbl3Proxy;
        private HFM.Forms.Controls.DataErrorTextBox txtProxyPass;
        private HFM.Forms.Controls.DataErrorTextBox txtProxyUser;
        private HFM.Forms.Controls.DataErrorTextBox txtProxyPort;
        private System.Windows.Forms.Label lbl3ProxyPass;
        private System.Windows.Forms.Label lbl3ProxyUser;
        private System.Windows.Forms.Label lbl3Port;
        private System.Windows.Forms.CheckBox chkUseProxyAuth;
        private System.Windows.Forms.CheckBox chkUseProxy;
        private System.Windows.Forms.TabPage ClientsTab;
        private System.Windows.Forms.OpenFileDialog openConfigDialog;
        private System.Windows.Forms.GroupBox grpProjectDownload;
        private HFM.Forms.Controls.DataErrorTextBox txtProjectDownloadUrl;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.RadioButton radioSchedule;
        private System.Windows.Forms.RadioButton radioFullRefresh;
        private System.Windows.Forms.ToolTip toolTipPrefs;
        private System.Windows.Forms.Label labelWrapper1;
        private System.Windows.Forms.NumericUpDown udDecimalPlaces;
        private System.Windows.Forms.TabPage tabReporting;
        private System.Windows.Forms.GroupBox grpEmailSettings;
        private System.Windows.Forms.Label lblSmtpServer;
        private System.Windows.Forms.Label lblToAddress;
        private HFM.Forms.Controls.DataErrorTextBox txtSmtpServer;
        private HFM.Forms.Controls.DataErrorTextBox txtToEmailAddress;
        private System.Windows.Forms.CheckBox chkEnableEmail;
        private System.Windows.Forms.Label lblFromEmailAddress;
        private HFM.Forms.Controls.DataErrorTextBox txtFromEmailAddress;
        private HFM.Forms.Controls.DataErrorTextBox txtSmtpPassword;
        private HFM.Forms.Controls.DataErrorTextBox txtSmtpUsername;
        private System.Windows.Forms.Label labelWrapper4;
        private System.Windows.Forms.Label labelWrapper5;
        private System.Windows.Forms.Button btnTestEmail;
        private System.Windows.Forms.GroupBox grpReportSelections;
        private System.Windows.Forms.TabPage OptionsTab;
        private System.Windows.Forms.GroupBox grpStartup;
        private System.Windows.Forms.CheckBox chkRunMinimized;
        private System.Windows.Forms.CheckBox chkAutoRun;
        private System.Windows.Forms.CheckBox chkFAHlog;
        private System.Windows.Forms.GroupBox grpInteractiveOptions;
        private System.Windows.Forms.CheckBox chkAutoSave;
        private System.Windows.Forms.CheckBox chkColorLog;
        private System.Windows.Forms.CheckBox chkOffline;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox PpdCalculationComboBox;
        private System.Windows.Forms.RadioButton radioActive;
        private System.Windows.Forms.RadioButton radioPassive;
        private System.Windows.Forms.Label lblFtpMode;
        private System.Windows.Forms.Button TestConnectionButton;
        private System.Windows.Forms.Button btnInstanceBrowse;
        private HFM.Forms.Controls.DataErrorTextBox txtInstance;
        private System.Windows.Forms.Label SlotXsltLabel;
        private System.Windows.Forms.Button btnSummaryBrowse;
        private HFM.Forms.Controls.DataErrorTextBox txtSummary;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button btnOverviewBrowse;
        private HFM.Forms.Controls.DataErrorTextBox txtOverview;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.CheckBox chkEmailSecure;
        private HFM.Forms.Controls.DataErrorTextBox txtSmtpServerPort;
        private System.Windows.Forms.Label labelWrapper3;
        private System.Windows.Forms.CheckBox chkCheckForUpdate;
        private System.Windows.Forms.GroupBox ExternalProgramsGroupBox;
        private System.Windows.Forms.Button btnBrowseLogViewer;
        private System.Windows.Forms.Label label3;
        private HFM.Forms.Controls.DataErrorTextBox LogFileViewerTextBox;
        private System.Windows.Forms.CheckBox chkHtml;
        private System.Windows.Forms.CheckBox chkXml;
        private System.Windows.Forms.NumericUpDown udLimitSize;
        private System.Windows.Forms.CheckBox chkLimitSize;
        private HFM.Forms.Controls.RadioPanel FtpModePanel;
        private System.Windows.Forms.CheckBox chkEtaAsDate;
        private System.Windows.Forms.Label WebSiteServerLabel;
        private System.Windows.Forms.Label WebSitePasswordLabel;
        private System.Windows.Forms.Label WebSiteUsernameLabel;
        private System.Windows.Forms.CheckBox DuplicateProjectCheckBox;
        private HFM.Forms.Controls.RadioPanel WebGenTypePanel;
        private HFM.Forms.Controls.DataErrorTextBox WebSitePasswordTextBox;
        private HFM.Forms.Controls.DataErrorTextBox WebSiteUsernameTextBox;
        private HFM.Forms.Controls.DataErrorTextBox WebSiteServerTextBox;
        private System.Windows.Forms.Label UploadTypeLabel;
        private System.Windows.Forms.RadioButton WebGenTypeFtpRadioButton;
        private System.Windows.Forms.RadioButton WebGenTypePathRadioButton;
        private HFM.Forms.Controls.DataErrorTextBox WebSitePortTextBox;
        private System.Windows.Forms.Label WebSitePortLabel;
        private System.Windows.Forms.Label labelWrapper6;
        private System.Windows.Forms.ComboBox BonusCalculationComboBox;
        private System.Windows.Forms.Label CopyLabel;
        private System.Windows.Forms.Button btnBrowseFileExplorer;
        private System.Windows.Forms.Label label4;
        private Controls.DataErrorTextBox FileExplorerTextBox;
        private System.Windows.Forms.GroupBox grpShowStyle;
        private System.Windows.Forms.ComboBox cboShowStyle;
        private System.Windows.Forms.Label labelWrapper2;
        private System.Windows.Forms.GroupBox LoggingGroupBox;
        private System.Windows.Forms.ComboBox cboMessageLevel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox ConfigurationGroupBox;
        private System.Windows.Forms.CheckBox chkDefaultConfig;
        private System.Windows.Forms.Button btnBrowseConfigFile;
        private Controls.DataErrorTextBox txtDefaultConfigFile;
        private System.Windows.Forms.GroupBox IdentityGroupBox;
        private System.Windows.Forms.Label EocUserIDLabel;
        private System.Windows.Forms.Label FahUserIDLabel;
        private System.Windows.Forms.LinkLabel TestFahTeamIDLinkLabel;
        private Controls.DataErrorTextBox EocUserIDTextBox;
        private Controls.DataErrorTextBox FahTeamIDTextBox;
        private System.Windows.Forms.Label FahTeamIDLabel;
        private System.Windows.Forms.LinkLabel TestFahUserIDLinkLabel;
        private Controls.DataErrorTextBox FahUserIDTextBox;
        private System.Windows.Forms.LinkLabel TestEocUserIDLinkLabel;
        private System.Windows.Forms.CheckBox EocUserStatsCheckBox;
        private System.Windows.Forms.GroupBox RefreshClientDataGroupBox;
        private Controls.DataErrorTextBox ClientRefreshIntervalTextBox;
        private System.Windows.Forms.Label lbl2SchedExplain;
        private System.Windows.Forms.CheckBox ClientRefreshIntervalCheckBox;
        private System.Windows.Forms.CheckBox ClientSynchronousTextBox;
        private System.Windows.Forms.Label label1;
    }
}
