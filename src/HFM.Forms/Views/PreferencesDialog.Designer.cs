
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
            this.webVisualStylesCssFileListBox = new System.Windows.Forms.ListBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.clientsTab = new System.Windows.Forms.TabPage();
            this.clientsRefreshClientDataGroupBox = new System.Windows.Forms.GroupBox();
            this.clientsRetrievalIntervalTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.toolTipPrefs = new System.Windows.Forms.ToolTip(this.components);
            this.lbl2SchedExplain = new System.Windows.Forms.Label();
            this.clientsRetrievalEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsRetrievalIsSerialCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsConfigurationGroupBox = new System.Windows.Forms.GroupBox();
            this.clientsDefaultConfigFileEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsBrowseConfigFileButton = new System.Windows.Forms.Button();
            this.clientsDefaultConfigFileTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.clientsAutoSaveConfig = new System.Windows.Forms.CheckBox();
            this.optionsDisplayProductionOptionsGroupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.clientsBonusCalculationComboBox = new System.Windows.Forms.ComboBox();
            this.labelWrapper6 = new System.Windows.Forms.Label();
            this.clientsDuplicateProjectCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsDisplayETADateCheckBox = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.clientsPPDCalculationComboBox = new System.Windows.Forms.ComboBox();
            this.clientsOfflineLastCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsColorLogFileCheckBox = new System.Windows.Forms.CheckBox();
            this.clientsDecimalPlacesUpDown = new System.Windows.Forms.NumericUpDown();
            this.labelWrapper1 = new System.Windows.Forms.Label();
            this.optionsTab = new System.Windows.Forms.TabPage();
            this.optionsIdentityGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsEocUserStatsEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsEocUserIDLabel = new System.Windows.Forms.Label();
            this.optionsFahUserIDLabel = new System.Windows.Forms.Label();
            this.optionsTestFahTeamIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.optionsEocUserIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.optionsFahTeamIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.optionsFahTeamIDLabel = new System.Windows.Forms.Label();
            this.optionsTestFahUserIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.optionsFahUserIDTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.optionsTestEocUserIDLinkLabel = new System.Windows.Forms.LinkLabel();
            this.optionsMinimizeToOptionGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsMinimizeToOptionComboBox = new System.Windows.Forms.ComboBox();
            this.labelWrapper2 = new System.Windows.Forms.Label();
            this.optionsMessageLevelGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsMessageLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.optionsExternalProgramsGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsBrowseFileExplorerButton = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.optionsFileExplorerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.optionsBrowseLogFileViewerButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.optionsLogFileViewerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.optionsStartupGroupBox = new System.Windows.Forms.GroupBox();
            this.optionsStartupCheckForUpdateCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsAutoRunCheckBox = new System.Windows.Forms.CheckBox();
            this.optionsRunMinimizedCheckBox = new System.Windows.Forms.CheckBox();
            this.webGenerationTab = new System.Windows.Forms.TabPage();
            this.webGenerationGroupBox = new System.Windows.Forms.GroupBox();
            this.webGenerationTestConnectionLinkLabel = new System.Windows.Forms.LinkLabel();
            this.webGenerationCopyLabel = new System.Windows.Forms.Label();
            this.webGenerationPortTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webGenerationPortLabel = new System.Windows.Forms.Label();
            this.webGenerationPasswordTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webGenerationUsernameTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webGenerationServerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webDeploymentTypeRadioPanel = new HFM.Forms.Controls.RadioPanel();
            this.webDeploymentTypeLabel = new System.Windows.Forms.Label();
            this.webDeploymentTypeFtpRadioButton = new System.Windows.Forms.RadioButton();
            this.webDeploymentTypePathRadioButton = new System.Windows.Forms.RadioButton();
            this.webGenerationPasswordLabel = new System.Windows.Forms.Label();
            this.webGenerationUsernameLabel = new System.Windows.Forms.Label();
            this.webGenerationServerLabel = new System.Windows.Forms.Label();
            this.webGenerationFtpModeRadioPanel = new HFM.Forms.Controls.RadioPanel();
            this.webDeploymentFtpActiveRadioButton = new System.Windows.Forms.RadioButton();
            this.webDeploymentFtpPassiveRadioButton = new System.Windows.Forms.RadioButton();
            this.lblFtpMode = new System.Windows.Forms.Label();
            this.webGenerationLimitLogSizeLengthUpDown = new System.Windows.Forms.NumericUpDown();
            this.webGenerationLimitLogSizeCheckBox = new System.Windows.Forms.CheckBox();
            this.webGenerationCopyXmlCheckBox = new System.Windows.Forms.CheckBox();
            this.webGenerationCopyHtmlCheckBox = new System.Windows.Forms.CheckBox();
            this.webGenerationCopyLogCheckBox = new System.Windows.Forms.CheckBox();
            this.webGenerationAfterClientRetrievalRadioButton = new System.Windows.Forms.RadioButton();
            this.webGenerationOnScheduleRadioButton = new System.Windows.Forms.RadioButton();
            this.webGenerationIntervalTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webGenerationIntervalLabel = new System.Windows.Forms.Label();
            this.webGenerationBrowsePathButton = new System.Windows.Forms.Button();
            this.webGenerationPathTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webGenerationPathLabel = new System.Windows.Forms.Label();
            this.webGenerationEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.webVisualStylesTab = new System.Windows.Forms.TabPage();
            this.webVisualStylesSlotBrowseXsltButton = new System.Windows.Forms.Button();
            this.webVisualStylesSlotTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webVisualStylesSlotLabel = new System.Windows.Forms.Label();
            this.webVisualStylesSummaryBrowseXsltButton = new System.Windows.Forms.Button();
            this.webVisualStylesSummaryTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.lblSummary = new System.Windows.Forms.Label();
            this.webVisualStylesOverviewBrowseXsltButton = new System.Windows.Forms.Button();
            this.webVisualStylesOverviewTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.lblOverview = new System.Windows.Forms.Label();
            this.lbl1Preview = new System.Windows.Forms.Label();
            this.lbl1Style = new System.Windows.Forms.Label();
            this.reportingTab = new System.Windows.Forms.TabPage();
            this.reportingSelectionsGroupBox = new System.Windows.Forms.GroupBox();
            this.reportingSettingsGroupBox = new System.Windows.Forms.GroupBox();
            this.reportingSendTestEmailLinkLabel = new System.Windows.Forms.LinkLabel();
            this.reportingPortTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.labelWrapper3 = new System.Windows.Forms.Label();
            this.reportingIsSecureCheckBox = new System.Windows.Forms.CheckBox();
            this.reportingPasswordTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.reportingUsernameTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.labelWrapper4 = new System.Windows.Forms.Label();
            this.labelWrapper5 = new System.Windows.Forms.Label();
            this.lblFromEmailAddress = new System.Windows.Forms.Label();
            this.reportingFromAddressTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.reportingEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.lblSmtpServer = new System.Windows.Forms.Label();
            this.lblToAddress = new System.Windows.Forms.Label();
            this.reportingServerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.reportingToAddressTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webProxyTab = new System.Windows.Forms.TabPage();
            this.webProxyGroupBox = new System.Windows.Forms.GroupBox();
            this.webProxyEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.webProxyCredentialsEnabledCheckBox = new System.Windows.Forms.CheckBox();
            this.webProxyPasswordTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webProxyUsernameTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.webProxyPortTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl3ProxyPass = new System.Windows.Forms.Label();
            this.webProxyServerTextBox = new HFM.Forms.Controls.DataErrorTextBox();
            this.lbl3ProxyUser = new System.Windows.Forms.Label();
            this.lbl3Port = new System.Windows.Forms.Label();
            this.lbl3Proxy = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.clientsTab.SuspendLayout();
            this.clientsRefreshClientDataGroupBox.SuspendLayout();
            this.clientsConfigurationGroupBox.SuspendLayout();
            this.optionsDisplayProductionOptionsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientsDecimalPlacesUpDown)).BeginInit();
            this.optionsTab.SuspendLayout();
            this.optionsIdentityGroupBox.SuspendLayout();
            this.optionsMinimizeToOptionGroupBox.SuspendLayout();
            this.optionsMessageLevelGroupBox.SuspendLayout();
            this.optionsExternalProgramsGroupBox.SuspendLayout();
            this.optionsStartupGroupBox.SuspendLayout();
            this.webGenerationTab.SuspendLayout();
            this.webGenerationGroupBox.SuspendLayout();
            this.webDeploymentTypeRadioPanel.SuspendLayout();
            this.webGenerationFtpModeRadioPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webGenerationLimitLogSizeLengthUpDown)).BeginInit();
            this.webVisualStylesTab.SuspendLayout();
            this.reportingTab.SuspendLayout();
            this.reportingSettingsGroupBox.SuspendLayout();
            this.webProxyTab.SuspendLayout();
            this.webProxyGroupBox.SuspendLayout();
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
            // webVisualStylesCssFileListBox
            // 
            this.webVisualStylesCssFileListBox.FormattingEnabled = true;
            this.webVisualStylesCssFileListBox.Location = new System.Drawing.Point(6, 27);
            this.webVisualStylesCssFileListBox.Name = "webVisualStylesCssFileListBox";
            this.webVisualStylesCssFileListBox.Size = new System.Drawing.Size(120, 134);
            this.webVisualStylesCssFileListBox.Sorted = true;
            this.webVisualStylesCssFileListBox.TabIndex = 2;
            this.webVisualStylesCssFileListBox.SelectedIndexChanged += new System.EventHandler(this.webVisualStylesCssFileListBox_SelectedIndexChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.clientsTab);
            this.tabControl1.Controls.Add(this.optionsTab);
            this.tabControl1.Controls.Add(this.webGenerationTab);
            this.tabControl1.Controls.Add(this.webVisualStylesTab);
            this.tabControl1.Controls.Add(this.reportingTab);
            this.tabControl1.Controls.Add(this.webProxyTab);
            this.tabControl1.HotTrack = true;
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Multiline = true;
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(509, 329);
            this.tabControl1.TabIndex = 5;
            this.tabControl1.SelectedIndexChanged += new System.EventHandler(this.tabControl1_SelectedIndexChanged);
            // 
            // clientsTab
            // 
            this.clientsTab.Controls.Add(this.clientsRefreshClientDataGroupBox);
            this.clientsTab.Controls.Add(this.clientsConfigurationGroupBox);
            this.clientsTab.Controls.Add(this.optionsDisplayProductionOptionsGroupBox);
            this.clientsTab.Location = new System.Drawing.Point(4, 22);
            this.clientsTab.Name = "clientsTab";
            this.clientsTab.Size = new System.Drawing.Size(501, 303);
            this.clientsTab.TabIndex = 4;
            this.clientsTab.Text = "Clients";
            this.clientsTab.UseVisualStyleBackColor = true;
            // 
            // clientsRefreshClientDataGroupBox
            // 
            this.clientsRefreshClientDataGroupBox.Controls.Add(this.clientsRetrievalIntervalTextBox);
            this.clientsRefreshClientDataGroupBox.Controls.Add(this.lbl2SchedExplain);
            this.clientsRefreshClientDataGroupBox.Controls.Add(this.clientsRetrievalEnabledCheckBox);
            this.clientsRefreshClientDataGroupBox.Controls.Add(this.clientsRetrievalIsSerialCheckBox);
            this.clientsRefreshClientDataGroupBox.Location = new System.Drawing.Point(6, 94);
            this.clientsRefreshClientDataGroupBox.Name = "clientsRefreshClientDataGroupBox";
            this.clientsRefreshClientDataGroupBox.Size = new System.Drawing.Size(489, 54);
            this.clientsRefreshClientDataGroupBox.TabIndex = 6;
            this.clientsRefreshClientDataGroupBox.TabStop = false;
            this.clientsRefreshClientDataGroupBox.Text = "Refresh Client Data";
            // 
            // clientsRetrievalIntervalTextBox
            // 
            this.clientsRetrievalIntervalTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.clientsRetrievalIntervalTextBox.DoubleBuffered = true;
            this.clientsRetrievalIntervalTextBox.Enabled = false;
            this.clientsRetrievalIntervalTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.clientsRetrievalIntervalTextBox.ErrorToolTip = this.toolTipPrefs;
            this.clientsRetrievalIntervalTextBox.ErrorToolTipDuration = 5000;
            this.clientsRetrievalIntervalTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.clientsRetrievalIntervalTextBox.ErrorToolTipText = "";
            this.clientsRetrievalIntervalTextBox.Location = new System.Drawing.Point(61, 20);
            this.clientsRetrievalIntervalTextBox.MaxLength = 3;
            this.clientsRetrievalIntervalTextBox.Name = "clientsRetrievalIntervalTextBox";
            this.clientsRetrievalIntervalTextBox.ReadOnly = true;
            this.clientsRetrievalIntervalTextBox.Size = new System.Drawing.Size(39, 20);
            this.clientsRetrievalIntervalTextBox.TabIndex = 4;
            this.clientsRetrievalIntervalTextBox.Text = "15";
            this.clientsRetrievalIntervalTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.clientsRetrievalIntervalTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
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
            // clientsRetrievalEnabledCheckBox
            // 
            this.clientsRetrievalEnabledCheckBox.AutoSize = true;
            this.clientsRetrievalEnabledCheckBox.Location = new System.Drawing.Point(10, 22);
            this.clientsRetrievalEnabledCheckBox.Name = "clientsRetrievalEnabledCheckBox";
            this.clientsRetrievalEnabledCheckBox.Size = new System.Drawing.Size(53, 17);
            this.clientsRetrievalEnabledCheckBox.TabIndex = 3;
            this.clientsRetrievalEnabledCheckBox.Text = "Every";
            this.clientsRetrievalEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsRetrievalIsSerialCheckBox
            // 
            this.clientsRetrievalIsSerialCheckBox.AutoSize = true;
            this.clientsRetrievalIsSerialCheckBox.Location = new System.Drawing.Point(153, 22);
            this.clientsRetrievalIsSerialCheckBox.Name = "clientsRetrievalIsSerialCheckBox";
            this.clientsRetrievalIsSerialCheckBox.Size = new System.Drawing.Size(176, 17);
            this.clientsRetrievalIsSerialCheckBox.TabIndex = 0;
            this.clientsRetrievalIsSerialCheckBox.Text = "In Series (synchronous retrieval)";
            this.clientsRetrievalIsSerialCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsConfigurationGroupBox
            // 
            this.clientsConfigurationGroupBox.Controls.Add(this.clientsDefaultConfigFileEnabledCheckBox);
            this.clientsConfigurationGroupBox.Controls.Add(this.clientsBrowseConfigFileButton);
            this.clientsConfigurationGroupBox.Controls.Add(this.clientsDefaultConfigFileTextBox);
            this.clientsConfigurationGroupBox.Controls.Add(this.clientsAutoSaveConfig);
            this.clientsConfigurationGroupBox.Location = new System.Drawing.Point(6, 9);
            this.clientsConfigurationGroupBox.Name = "clientsConfigurationGroupBox";
            this.clientsConfigurationGroupBox.Size = new System.Drawing.Size(489, 79);
            this.clientsConfigurationGroupBox.TabIndex = 5;
            this.clientsConfigurationGroupBox.TabStop = false;
            this.clientsConfigurationGroupBox.Text = "Configuration";
            // 
            // clientsDefaultConfigFileEnabledCheckBox
            // 
            this.clientsDefaultConfigFileEnabledCheckBox.AutoSize = true;
            this.clientsDefaultConfigFileEnabledCheckBox.Location = new System.Drawing.Point(10, 22);
            this.clientsDefaultConfigFileEnabledCheckBox.Name = "clientsDefaultConfigFileEnabledCheckBox";
            this.clientsDefaultConfigFileEnabledCheckBox.Size = new System.Drawing.Size(134, 17);
            this.clientsDefaultConfigFileEnabledCheckBox.TabIndex = 0;
            this.clientsDefaultConfigFileEnabledCheckBox.Text = "Load Configuration File";
            this.clientsDefaultConfigFileEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsBrowseConfigFileButton
            // 
            this.clientsBrowseConfigFileButton.Enabled = false;
            this.clientsBrowseConfigFileButton.Location = new System.Drawing.Point(456, 47);
            this.clientsBrowseConfigFileButton.Name = "clientsBrowseConfigFileButton";
            this.clientsBrowseConfigFileButton.Size = new System.Drawing.Size(24, 23);
            this.clientsBrowseConfigFileButton.TabIndex = 3;
            this.clientsBrowseConfigFileButton.Text = "...";
            this.clientsBrowseConfigFileButton.UseVisualStyleBackColor = true;
            this.clientsBrowseConfigFileButton.Click += new System.EventHandler(this.clientsBrowseConfigFileButton_Click);
            // 
            // clientsDefaultConfigFileTextBox
            // 
            this.clientsDefaultConfigFileTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.clientsDefaultConfigFileTextBox.DoubleBuffered = true;
            this.clientsDefaultConfigFileTextBox.Enabled = false;
            this.clientsDefaultConfigFileTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.clientsDefaultConfigFileTextBox.ErrorToolTip = this.toolTipPrefs;
            this.clientsDefaultConfigFileTextBox.ErrorToolTipDuration = 5000;
            this.clientsDefaultConfigFileTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.clientsDefaultConfigFileTextBox.ErrorToolTipText = "";
            this.clientsDefaultConfigFileTextBox.Location = new System.Drawing.Point(10, 49);
            this.clientsDefaultConfigFileTextBox.Name = "clientsDefaultConfigFileTextBox";
            this.clientsDefaultConfigFileTextBox.ReadOnly = true;
            this.clientsDefaultConfigFileTextBox.Size = new System.Drawing.Size(440, 20);
            this.clientsDefaultConfigFileTextBox.TabIndex = 2;
            // 
            // clientsAutoSaveConfig
            // 
            this.clientsAutoSaveConfig.AutoSize = true;
            this.clientsAutoSaveConfig.Location = new System.Drawing.Point(153, 22);
            this.clientsAutoSaveConfig.Name = "clientsAutoSaveConfig";
            this.clientsAutoSaveConfig.Size = new System.Drawing.Size(151, 17);
            this.clientsAutoSaveConfig.TabIndex = 2;
            this.clientsAutoSaveConfig.Text = "Auto Save when Changed";
            this.clientsAutoSaveConfig.UseVisualStyleBackColor = true;
            // 
            // optionsDisplayProductionOptionsGroupBox
            // 
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.label1);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsBonusCalculationComboBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.labelWrapper6);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsDuplicateProjectCheckBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsDisplayETADateCheckBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.label2);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsPPDCalculationComboBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsOfflineLastCheckBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsColorLogFileCheckBox);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.clientsDecimalPlacesUpDown);
            this.optionsDisplayProductionOptionsGroupBox.Controls.Add(this.labelWrapper1);
            this.optionsDisplayProductionOptionsGroupBox.Location = new System.Drawing.Point(6, 154);
            this.optionsDisplayProductionOptionsGroupBox.Name = "optionsDisplayProductionOptionsGroupBox";
            this.optionsDisplayProductionOptionsGroupBox.Size = new System.Drawing.Size(489, 130);
            this.optionsDisplayProductionOptionsGroupBox.TabIndex = 0;
            this.optionsDisplayProductionOptionsGroupBox.TabStop = false;
            this.optionsDisplayProductionOptionsGroupBox.Text = "Display / Production Options";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(411, 74);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 13);
            this.label1.TabIndex = 15;
            this.label1.Text = "Decimals";
            // 
            // clientsBonusCalculationComboBox
            // 
            this.clientsBonusCalculationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientsBonusCalculationComboBox.FormattingEnabled = true;
            this.clientsBonusCalculationComboBox.Location = new System.Drawing.Point(367, 45);
            this.clientsBonusCalculationComboBox.Name = "clientsBonusCalculationComboBox";
            this.clientsBonusCalculationComboBox.Size = new System.Drawing.Size(113, 21);
            this.clientsBonusCalculationComboBox.TabIndex = 14;
            // 
            // labelWrapper6
            // 
            this.labelWrapper6.AutoSize = true;
            this.labelWrapper6.Location = new System.Drawing.Point(232, 49);
            this.labelWrapper6.Name = "labelWrapper6";
            this.labelWrapper6.Size = new System.Drawing.Size(128, 13);
            this.labelWrapper6.TabIndex = 13;
            this.labelWrapper6.Text = "Calculate Credit based on";
            // 
            // clientsDuplicateProjectCheckBox
            // 
            this.clientsDuplicateProjectCheckBox.AutoSize = true;
            this.clientsDuplicateProjectCheckBox.Location = new System.Drawing.Point(10, 75);
            this.clientsDuplicateProjectCheckBox.Name = "clientsDuplicateProjectCheckBox";
            this.clientsDuplicateProjectCheckBox.Size = new System.Drawing.Size(183, 17);
            this.clientsDuplicateProjectCheckBox.TabIndex = 10;
            this.clientsDuplicateProjectCheckBox.Text = "Duplicate Project (R/C/G) Check";
            this.clientsDuplicateProjectCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsDisplayETADateCheckBox
            // 
            this.clientsDisplayETADateCheckBox.AutoSize = true;
            this.clientsDisplayETADateCheckBox.Location = new System.Drawing.Point(10, 101);
            this.clientsDisplayETADateCheckBox.Name = "clientsDisplayETADateCheckBox";
            this.clientsDisplayETADateCheckBox.Size = new System.Drawing.Size(202, 17);
            this.clientsDisplayETADateCheckBox.TabIndex = 9;
            this.clientsDisplayETADateCheckBox.Text = "Show ETA value as a Date and Time";
            this.clientsDisplayETADateCheckBox.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(123, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Calculate PPD based on";
            // 
            // clientsPPDCalculationComboBox
            // 
            this.clientsPPDCalculationComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.clientsPPDCalculationComboBox.FormattingEnabled = true;
            this.clientsPPDCalculationComboBox.Location = new System.Drawing.Point(367, 18);
            this.clientsPPDCalculationComboBox.Name = "clientsPPDCalculationComboBox";
            this.clientsPPDCalculationComboBox.Size = new System.Drawing.Size(113, 21);
            this.clientsPPDCalculationComboBox.TabIndex = 5;
            // 
            // clientsOfflineLastCheckBox
            // 
            this.clientsOfflineLastCheckBox.AutoSize = true;
            this.clientsOfflineLastCheckBox.Location = new System.Drawing.Point(10, 22);
            this.clientsOfflineLastCheckBox.Name = "clientsOfflineLastCheckBox";
            this.clientsOfflineLastCheckBox.Size = new System.Drawing.Size(132, 17);
            this.clientsOfflineLastCheckBox.TabIndex = 0;
            this.clientsOfflineLastCheckBox.Text = "List Offline Clients Last";
            this.clientsOfflineLastCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsColorLogFileCheckBox
            // 
            this.clientsColorLogFileCheckBox.AutoSize = true;
            this.clientsColorLogFileCheckBox.Location = new System.Drawing.Point(10, 49);
            this.clientsColorLogFileCheckBox.Name = "clientsColorLogFileCheckBox";
            this.clientsColorLogFileCheckBox.Size = new System.Drawing.Size(148, 17);
            this.clientsColorLogFileCheckBox.TabIndex = 1;
            this.clientsColorLogFileCheckBox.Text = "Color the Log Viewer Text";
            this.clientsColorLogFileCheckBox.UseVisualStyleBackColor = true;
            // 
            // clientsDecimalPlacesUpDown
            // 
            this.clientsDecimalPlacesUpDown.Location = new System.Drawing.Point(367, 71);
            this.clientsDecimalPlacesUpDown.Name = "clientsDecimalPlacesUpDown";
            this.clientsDecimalPlacesUpDown.Size = new System.Drawing.Size(39, 20);
            this.clientsDecimalPlacesUpDown.TabIndex = 7;
            // 
            // labelWrapper1
            // 
            this.labelWrapper1.AutoSize = true;
            this.labelWrapper1.Location = new System.Drawing.Point(297, 74);
            this.labelWrapper1.Name = "labelWrapper1";
            this.labelWrapper1.Size = new System.Drawing.Size(63, 13);
            this.labelWrapper1.TabIndex = 6;
            this.labelWrapper1.Text = "Calculate to";
            // 
            // optionsTab
            // 
            this.optionsTab.Controls.Add(this.optionsIdentityGroupBox);
            this.optionsTab.Controls.Add(this.optionsMinimizeToOptionGroupBox);
            this.optionsTab.Controls.Add(this.optionsMessageLevelGroupBox);
            this.optionsTab.Controls.Add(this.optionsExternalProgramsGroupBox);
            this.optionsTab.Controls.Add(this.optionsStartupGroupBox);
            this.optionsTab.Location = new System.Drawing.Point(4, 22);
            this.optionsTab.Name = "optionsTab";
            this.optionsTab.Padding = new System.Windows.Forms.Padding(3);
            this.optionsTab.Size = new System.Drawing.Size(501, 303);
            this.optionsTab.TabIndex = 6;
            this.optionsTab.Text = "Options";
            this.optionsTab.UseVisualStyleBackColor = true;
            // 
            // optionsIdentityGroupBox
            // 
            this.optionsIdentityGroupBox.Controls.Add(this.optionsEocUserStatsEnabledCheckBox);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsEocUserIDLabel);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsFahUserIDLabel);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsTestFahTeamIDLinkLabel);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsEocUserIDTextBox);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsFahTeamIDTextBox);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsFahTeamIDLabel);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsTestFahUserIDLinkLabel);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsFahUserIDTextBox);
            this.optionsIdentityGroupBox.Controls.Add(this.optionsTestEocUserIDLinkLabel);
            this.optionsIdentityGroupBox.Location = new System.Drawing.Point(6, 65);
            this.optionsIdentityGroupBox.Name = "optionsIdentityGroupBox";
            this.optionsIdentityGroupBox.Size = new System.Drawing.Size(489, 77);
            this.optionsIdentityGroupBox.TabIndex = 8;
            this.optionsIdentityGroupBox.TabStop = false;
            this.optionsIdentityGroupBox.Text = "Identity";
            // 
            // optionsEocUserStatsEnabledCheckBox
            // 
            this.optionsEocUserStatsEnabledCheckBox.AutoSize = true;
            this.optionsEocUserStatsEnabledCheckBox.Location = new System.Drawing.Point(250, 47);
            this.optionsEocUserStatsEnabledCheckBox.Name = "optionsEocUserStatsEnabledCheckBox";
            this.optionsEocUserStatsEnabledCheckBox.Size = new System.Drawing.Size(194, 17);
            this.optionsEocUserStatsEnabledCheckBox.TabIndex = 14;
            this.optionsEocUserStatsEnabledCheckBox.Text = "Retrieve and Show EOC User Stats";
            this.optionsEocUserStatsEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // optionsEocUserIDLabel
            // 
            this.optionsEocUserIDLabel.AutoSize = true;
            this.optionsEocUserIDLabel.Location = new System.Drawing.Point(247, 22);
            this.optionsEocUserIDLabel.Name = "optionsEocUserIDLabel";
            this.optionsEocUserIDLabel.Size = new System.Drawing.Size(68, 13);
            this.optionsEocUserIDLabel.TabIndex = 0;
            this.optionsEocUserIDLabel.Text = "EOC User ID";
            // 
            // optionsFahUserIDLabel
            // 
            this.optionsFahUserIDLabel.AutoSize = true;
            this.optionsFahUserIDLabel.Location = new System.Drawing.Point(12, 22);
            this.optionsFahUserIDLabel.Name = "optionsFahUserIDLabel";
            this.optionsFahUserIDLabel.Size = new System.Drawing.Size(67, 13);
            this.optionsFahUserIDLabel.TabIndex = 1;
            this.optionsFahUserIDLabel.Text = "FAH User ID";
            // 
            // optionsTestFahTeamIDLinkLabel
            // 
            this.optionsTestFahTeamIDLinkLabel.AutoSize = true;
            this.optionsTestFahTeamIDLinkLabel.Location = new System.Drawing.Point(211, 48);
            this.optionsTestFahTeamIDLinkLabel.Name = "optionsTestFahTeamIDLinkLabel";
            this.optionsTestFahTeamIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.optionsTestFahTeamIDLinkLabel.TabIndex = 8;
            this.optionsTestFahTeamIDLinkLabel.TabStop = true;
            this.optionsTestFahTeamIDLinkLabel.Text = "Test";
            this.optionsTestFahTeamIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.optionsTestFahTeamIDLinkLabel_LinkClicked);
            // 
            // optionsEocUserIDTextBox
            // 
            this.optionsEocUserIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.optionsEocUserIDTextBox.DoubleBuffered = true;
            this.optionsEocUserIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.optionsEocUserIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.optionsEocUserIDTextBox.ErrorToolTipDuration = 5000;
            this.optionsEocUserIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.optionsEocUserIDTextBox.ErrorToolTipText = "";
            this.optionsEocUserIDTextBox.Location = new System.Drawing.Point(321, 19);
            this.optionsEocUserIDTextBox.MaxLength = 9;
            this.optionsEocUserIDTextBox.Name = "optionsEocUserIDTextBox";
            this.optionsEocUserIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.optionsEocUserIDTextBox.TabIndex = 3;
            this.optionsEocUserIDTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // optionsFahTeamIDTextBox
            // 
            this.optionsFahTeamIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.optionsFahTeamIDTextBox.DoubleBuffered = true;
            this.optionsFahTeamIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.optionsFahTeamIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.optionsFahTeamIDTextBox.ErrorToolTipDuration = 5000;
            this.optionsFahTeamIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.optionsFahTeamIDTextBox.ErrorToolTipText = "";
            this.optionsFahTeamIDTextBox.Location = new System.Drawing.Point(85, 45);
            this.optionsFahTeamIDTextBox.MaxLength = 9;
            this.optionsFahTeamIDTextBox.Name = "optionsFahTeamIDTextBox";
            this.optionsFahTeamIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.optionsFahTeamIDTextBox.TabIndex = 5;
            this.optionsFahTeamIDTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // optionsFahTeamIDLabel
            // 
            this.optionsFahTeamIDLabel.AutoSize = true;
            this.optionsFahTeamIDLabel.Location = new System.Drawing.Point(7, 48);
            this.optionsFahTeamIDLabel.Name = "optionsFahTeamIDLabel";
            this.optionsFahTeamIDLabel.Size = new System.Drawing.Size(72, 13);
            this.optionsFahTeamIDLabel.TabIndex = 2;
            this.optionsFahTeamIDLabel.Text = "FAH Team ID";
            // 
            // optionsTestFahUserIDLinkLabel
            // 
            this.optionsTestFahUserIDLinkLabel.AutoSize = true;
            this.optionsTestFahUserIDLinkLabel.Location = new System.Drawing.Point(211, 22);
            this.optionsTestFahUserIDLinkLabel.Name = "optionsTestFahUserIDLinkLabel";
            this.optionsTestFahUserIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.optionsTestFahUserIDLinkLabel.TabIndex = 7;
            this.optionsTestFahUserIDLinkLabel.TabStop = true;
            this.optionsTestFahUserIDLinkLabel.Text = "Test";
            this.optionsTestFahUserIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.optionsTestFahUserIDLinkLabel_LinkClicked);
            // 
            // optionsFahUserIDTextBox
            // 
            this.optionsFahUserIDTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.optionsFahUserIDTextBox.DoubleBuffered = true;
            this.optionsFahUserIDTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.optionsFahUserIDTextBox.ErrorToolTip = this.toolTipPrefs;
            this.optionsFahUserIDTextBox.ErrorToolTipDuration = 5000;
            this.optionsFahUserIDTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.optionsFahUserIDTextBox.ErrorToolTipText = "";
            this.optionsFahUserIDTextBox.Location = new System.Drawing.Point(85, 19);
            this.optionsFahUserIDTextBox.Name = "optionsFahUserIDTextBox";
            this.optionsFahUserIDTextBox.Size = new System.Drawing.Size(120, 20);
            this.optionsFahUserIDTextBox.TabIndex = 4;
            // 
            // optionsTestEocUserIDLinkLabel
            // 
            this.optionsTestEocUserIDLinkLabel.AutoSize = true;
            this.optionsTestEocUserIDLinkLabel.Location = new System.Drawing.Point(447, 22);
            this.optionsTestEocUserIDLinkLabel.Name = "optionsTestEocUserIDLinkLabel";
            this.optionsTestEocUserIDLinkLabel.Size = new System.Drawing.Size(28, 13);
            this.optionsTestEocUserIDLinkLabel.TabIndex = 6;
            this.optionsTestEocUserIDLinkLabel.TabStop = true;
            this.optionsTestEocUserIDLinkLabel.Text = "Test";
            this.optionsTestEocUserIDLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.optionsTestEocUserIDLinkLabel_LinkClicked);
            // 
            // optionsMinimizeToOptionGroupBox
            // 
            this.optionsMinimizeToOptionGroupBox.Controls.Add(this.optionsMinimizeToOptionComboBox);
            this.optionsMinimizeToOptionGroupBox.Controls.Add(this.labelWrapper2);
            this.optionsMinimizeToOptionGroupBox.Location = new System.Drawing.Point(254, 238);
            this.optionsMinimizeToOptionGroupBox.Name = "optionsMinimizeToOptionGroupBox";
            this.optionsMinimizeToOptionGroupBox.Size = new System.Drawing.Size(241, 54);
            this.optionsMinimizeToOptionGroupBox.TabIndex = 7;
            this.optionsMinimizeToOptionGroupBox.TabStop = false;
            this.optionsMinimizeToOptionGroupBox.Text = "Docking Style";
            // 
            // optionsMinimizeToOptionComboBox
            // 
            this.optionsMinimizeToOptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionsMinimizeToOptionComboBox.FormattingEnabled = true;
            this.optionsMinimizeToOptionComboBox.Location = new System.Drawing.Point(132, 19);
            this.optionsMinimizeToOptionComboBox.Name = "optionsMinimizeToOptionComboBox";
            this.optionsMinimizeToOptionComboBox.Size = new System.Drawing.Size(89, 21);
            this.optionsMinimizeToOptionComboBox.TabIndex = 1;
            // 
            // labelWrapper2
            // 
            this.labelWrapper2.AutoSize = true;
            this.labelWrapper2.Location = new System.Drawing.Point(9, 24);
            this.labelWrapper2.Name = "labelWrapper2";
            this.labelWrapper2.Size = new System.Drawing.Size(114, 13);
            this.labelWrapper2.TabIndex = 0;
            this.labelWrapper2.Text = "Show HFM.NET in the";
            // 
            // optionsMessageLevelGroupBox
            // 
            this.optionsMessageLevelGroupBox.Controls.Add(this.optionsMessageLevelComboBox);
            this.optionsMessageLevelGroupBox.Controls.Add(this.label6);
            this.optionsMessageLevelGroupBox.Location = new System.Drawing.Point(6, 238);
            this.optionsMessageLevelGroupBox.Name = "optionsMessageLevelGroupBox";
            this.optionsMessageLevelGroupBox.Size = new System.Drawing.Size(241, 54);
            this.optionsMessageLevelGroupBox.TabIndex = 6;
            this.optionsMessageLevelGroupBox.TabStop = false;
            this.optionsMessageLevelGroupBox.Text = "Logging";
            // 
            // optionsMessageLevelComboBox
            // 
            this.optionsMessageLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.optionsMessageLevelComboBox.FormattingEnabled = true;
            this.optionsMessageLevelComboBox.Location = new System.Drawing.Point(92, 19);
            this.optionsMessageLevelComboBox.Name = "optionsMessageLevelComboBox";
            this.optionsMessageLevelComboBox.Size = new System.Drawing.Size(75, 21);
            this.optionsMessageLevelComboBox.TabIndex = 1;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 24);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 0;
            this.label6.Text = "Logging Level";
            // 
            // optionsExternalProgramsGroupBox
            // 
            this.optionsExternalProgramsGroupBox.Controls.Add(this.optionsBrowseFileExplorerButton);
            this.optionsExternalProgramsGroupBox.Controls.Add(this.label4);
            this.optionsExternalProgramsGroupBox.Controls.Add(this.optionsFileExplorerTextBox);
            this.optionsExternalProgramsGroupBox.Controls.Add(this.optionsBrowseLogFileViewerButton);
            this.optionsExternalProgramsGroupBox.Controls.Add(this.label3);
            this.optionsExternalProgramsGroupBox.Controls.Add(this.optionsLogFileViewerTextBox);
            this.optionsExternalProgramsGroupBox.Location = new System.Drawing.Point(6, 148);
            this.optionsExternalProgramsGroupBox.Name = "optionsExternalProgramsGroupBox";
            this.optionsExternalProgramsGroupBox.Size = new System.Drawing.Size(489, 84);
            this.optionsExternalProgramsGroupBox.TabIndex = 5;
            this.optionsExternalProgramsGroupBox.TabStop = false;
            this.optionsExternalProgramsGroupBox.Text = "External Programs";
            // 
            // optionsBrowseFileExplorerButton
            // 
            this.optionsBrowseFileExplorerButton.Location = new System.Drawing.Point(456, 49);
            this.optionsBrowseFileExplorerButton.Name = "optionsBrowseFileExplorerButton";
            this.optionsBrowseFileExplorerButton.Size = new System.Drawing.Size(24, 23);
            this.optionsBrowseFileExplorerButton.TabIndex = 5;
            this.optionsBrowseFileExplorerButton.Text = "...";
            this.optionsBrowseFileExplorerButton.UseVisualStyleBackColor = true;
            this.optionsBrowseFileExplorerButton.Click += new System.EventHandler(this.optionsBrowseFileExplorerButton_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 54);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(64, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "File Explorer";
            // 
            // optionsFileExplorerTextBox
            // 
            this.optionsFileExplorerTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.optionsFileExplorerTextBox.DoubleBuffered = true;
            this.optionsFileExplorerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.optionsFileExplorerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.optionsFileExplorerTextBox.ErrorToolTipDuration = 5000;
            this.optionsFileExplorerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.optionsFileExplorerTextBox.ErrorToolTipText = "";
            this.optionsFileExplorerTextBox.Location = new System.Drawing.Point(96, 51);
            this.optionsFileExplorerTextBox.Name = "optionsFileExplorerTextBox";
            this.optionsFileExplorerTextBox.Size = new System.Drawing.Size(354, 20);
            this.optionsFileExplorerTextBox.TabIndex = 4;
            // 
            // optionsBrowseLogFileViewerButton
            // 
            this.optionsBrowseLogFileViewerButton.Location = new System.Drawing.Point(456, 19);
            this.optionsBrowseLogFileViewerButton.Name = "optionsBrowseLogFileViewerButton";
            this.optionsBrowseLogFileViewerButton.Size = new System.Drawing.Size(24, 23);
            this.optionsBrowseLogFileViewerButton.TabIndex = 2;
            this.optionsBrowseLogFileViewerButton.Text = "...";
            this.optionsBrowseLogFileViewerButton.UseVisualStyleBackColor = true;
            this.optionsBrowseLogFileViewerButton.Click += new System.EventHandler(this.optionsBrowseLogFileViewerButton_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Log File Viewer";
            // 
            // optionsLogFileViewerTextBox
            // 
            this.optionsLogFileViewerTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.optionsLogFileViewerTextBox.DoubleBuffered = true;
            this.optionsLogFileViewerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.optionsLogFileViewerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.optionsLogFileViewerTextBox.ErrorToolTipDuration = 5000;
            this.optionsLogFileViewerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.optionsLogFileViewerTextBox.ErrorToolTipText = "";
            this.optionsLogFileViewerTextBox.Location = new System.Drawing.Point(96, 21);
            this.optionsLogFileViewerTextBox.Name = "optionsLogFileViewerTextBox";
            this.optionsLogFileViewerTextBox.Size = new System.Drawing.Size(354, 20);
            this.optionsLogFileViewerTextBox.TabIndex = 1;
            // 
            // optionsStartupGroupBox
            // 
            this.optionsStartupGroupBox.Controls.Add(this.optionsStartupCheckForUpdateCheckBox);
            this.optionsStartupGroupBox.Controls.Add(this.optionsAutoRunCheckBox);
            this.optionsStartupGroupBox.Controls.Add(this.optionsRunMinimizedCheckBox);
            this.optionsStartupGroupBox.Location = new System.Drawing.Point(6, 9);
            this.optionsStartupGroupBox.Name = "optionsStartupGroupBox";
            this.optionsStartupGroupBox.Size = new System.Drawing.Size(489, 50);
            this.optionsStartupGroupBox.TabIndex = 3;
            this.optionsStartupGroupBox.TabStop = false;
            this.optionsStartupGroupBox.Text = "Startup";
            // 
            // optionsStartupCheckForUpdateCheckBox
            // 
            this.optionsStartupCheckForUpdateCheckBox.AutoSize = true;
            this.optionsStartupCheckForUpdateCheckBox.Location = new System.Drawing.Point(310, 20);
            this.optionsStartupCheckForUpdateCheckBox.Name = "optionsStartupCheckForUpdateCheckBox";
            this.optionsStartupCheckForUpdateCheckBox.Size = new System.Drawing.Size(115, 17);
            this.optionsStartupCheckForUpdateCheckBox.TabIndex = 2;
            this.optionsStartupCheckForUpdateCheckBox.Text = "Check for Updates";
            this.optionsStartupCheckForUpdateCheckBox.UseVisualStyleBackColor = true;
            // 
            // optionsAutoRunCheckBox
            // 
            this.optionsAutoRunCheckBox.AutoSize = true;
            this.optionsAutoRunCheckBox.Location = new System.Drawing.Point(10, 20);
            this.optionsAutoRunCheckBox.Name = "optionsAutoRunCheckBox";
            this.optionsAutoRunCheckBox.Size = new System.Drawing.Size(170, 17);
            this.optionsAutoRunCheckBox.TabIndex = 0;
            this.optionsAutoRunCheckBox.Text = "Auto Run on Windows Startup";
            this.optionsAutoRunCheckBox.UseVisualStyleBackColor = true;
            // 
            // optionsRunMinimizedCheckBox
            // 
            this.optionsRunMinimizedCheckBox.AutoSize = true;
            this.optionsRunMinimizedCheckBox.Location = new System.Drawing.Point(196, 20);
            this.optionsRunMinimizedCheckBox.Name = "optionsRunMinimizedCheckBox";
            this.optionsRunMinimizedCheckBox.Size = new System.Drawing.Size(95, 17);
            this.optionsRunMinimizedCheckBox.TabIndex = 1;
            this.optionsRunMinimizedCheckBox.Text = "Run Minimized";
            this.optionsRunMinimizedCheckBox.UseVisualStyleBackColor = true;
            // 
            // webGenerationTab
            // 
            this.webGenerationTab.BackColor = System.Drawing.Color.Transparent;
            this.webGenerationTab.Controls.Add(this.webGenerationGroupBox);
            this.webGenerationTab.Location = new System.Drawing.Point(4, 22);
            this.webGenerationTab.Name = "webGenerationTab";
            this.webGenerationTab.Padding = new System.Windows.Forms.Padding(3);
            this.webGenerationTab.Size = new System.Drawing.Size(501, 303);
            this.webGenerationTab.TabIndex = 2;
            this.webGenerationTab.Text = "Web Generation";
            this.webGenerationTab.UseVisualStyleBackColor = true;
            // 
            // webGenerationGroupBox
            // 
            this.webGenerationGroupBox.Controls.Add(this.webGenerationTestConnectionLinkLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationCopyLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPortTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPortLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPasswordTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationUsernameTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationServerTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webDeploymentTypeRadioPanel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPasswordLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationUsernameLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationServerLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationFtpModeRadioPanel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationLimitLogSizeLengthUpDown);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationLimitLogSizeCheckBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationCopyXmlCheckBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationCopyHtmlCheckBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationCopyLogCheckBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationAfterClientRetrievalRadioButton);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationOnScheduleRadioButton);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationIntervalTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationIntervalLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationBrowsePathButton);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPathTextBox);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationPathLabel);
            this.webGenerationGroupBox.Controls.Add(this.webGenerationEnabledCheckBox);
            this.webGenerationGroupBox.Location = new System.Drawing.Point(6, 6);
            this.webGenerationGroupBox.Name = "webGenerationGroupBox";
            this.webGenerationGroupBox.Size = new System.Drawing.Size(489, 191);
            this.webGenerationGroupBox.TabIndex = 0;
            this.webGenerationGroupBox.TabStop = false;
            this.webGenerationGroupBox.Text = "Web Generation";
            // 
            // webGenerationTestConnectionLinkLabel
            // 
            this.webGenerationTestConnectionLinkLabel.AutoSize = true;
            this.webGenerationTestConnectionLinkLabel.Location = new System.Drawing.Point(395, 20);
            this.webGenerationTestConnectionLinkLabel.Name = "webGenerationTestConnectionLinkLabel";
            this.webGenerationTestConnectionLinkLabel.Size = new System.Drawing.Size(85, 13);
            this.webGenerationTestConnectionLinkLabel.TabIndex = 25;
            this.webGenerationTestConnectionLinkLabel.TabStop = true;
            this.webGenerationTestConnectionLinkLabel.Text = "Test Connection";
            this.webGenerationTestConnectionLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.webGenerationTestConnectionLinkLabel_LinkClicked);
            // 
            // webGenerationCopyLabel
            // 
            this.webGenerationCopyLabel.AutoSize = true;
            this.webGenerationCopyLabel.Location = new System.Drawing.Point(7, 163);
            this.webGenerationCopyLabel.Name = "webGenerationCopyLabel";
            this.webGenerationCopyLabel.Size = new System.Drawing.Size(31, 13);
            this.webGenerationCopyLabel.TabIndex = 24;
            this.webGenerationCopyLabel.Text = "Copy";
            // 
            // webGenerationPortTextBox
            // 
            this.webGenerationPortTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationPortTextBox.DoubleBuffered = true;
            this.webGenerationPortTextBox.Enabled = false;
            this.webGenerationPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationPortTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationPortTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webGenerationPortTextBox.ErrorToolTipText = "";
            this.webGenerationPortTextBox.Location = new System.Drawing.Point(396, 104);
            this.webGenerationPortTextBox.Name = "webGenerationPortTextBox";
            this.webGenerationPortTextBox.ReadOnly = true;
            this.webGenerationPortTextBox.Size = new System.Drawing.Size(54, 20);
            this.webGenerationPortTextBox.TabIndex = 13;
            this.webGenerationPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // webGenerationPortLabel
            // 
            this.webGenerationPortLabel.AutoSize = true;
            this.webGenerationPortLabel.Location = new System.Drawing.Point(364, 107);
            this.webGenerationPortLabel.Name = "webGenerationPortLabel";
            this.webGenerationPortLabel.Size = new System.Drawing.Size(26, 13);
            this.webGenerationPortLabel.TabIndex = 12;
            this.webGenerationPortLabel.Text = "Port";
            // 
            // webGenerationPasswordTextBox
            // 
            this.webGenerationPasswordTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationPasswordTextBox.DoubleBuffered = true;
            this.webGenerationPasswordTextBox.Enabled = false;
            this.webGenerationPasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationPasswordTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationPasswordTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationPasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webGenerationPasswordTextBox.ErrorToolTipText = "";
            this.webGenerationPasswordTextBox.Location = new System.Drawing.Point(295, 132);
            this.webGenerationPasswordTextBox.Name = "webGenerationPasswordTextBox";
            this.webGenerationPasswordTextBox.ReadOnly = true;
            this.webGenerationPasswordTextBox.Size = new System.Drawing.Size(155, 20);
            this.webGenerationPasswordTextBox.TabIndex = 17;
            this.webGenerationPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // webGenerationUsernameTextBox
            // 
            this.webGenerationUsernameTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationUsernameTextBox.DoubleBuffered = true;
            this.webGenerationUsernameTextBox.Enabled = false;
            this.webGenerationUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationUsernameTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationUsernameTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webGenerationUsernameTextBox.ErrorToolTipText = "";
            this.webGenerationUsernameTextBox.Location = new System.Drawing.Point(76, 132);
            this.webGenerationUsernameTextBox.Name = "webGenerationUsernameTextBox";
            this.webGenerationUsernameTextBox.ReadOnly = true;
            this.webGenerationUsernameTextBox.Size = new System.Drawing.Size(155, 20);
            this.webGenerationUsernameTextBox.TabIndex = 15;
            // 
            // webGenerationServerTextBox
            // 
            this.webGenerationServerTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationServerTextBox.DoubleBuffered = true;
            this.webGenerationServerTextBox.Enabled = false;
            this.webGenerationServerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationServerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationServerTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationServerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webGenerationServerTextBox.ErrorToolTipText = "";
            this.webGenerationServerTextBox.Location = new System.Drawing.Point(76, 105);
            this.webGenerationServerTextBox.Name = "webGenerationServerTextBox";
            this.webGenerationServerTextBox.ReadOnly = true;
            this.webGenerationServerTextBox.Size = new System.Drawing.Size(282, 20);
            this.webGenerationServerTextBox.TabIndex = 11;
            // 
            // webDeploymentTypeRadioPanel
            // 
            this.webDeploymentTypeRadioPanel.Controls.Add(this.webDeploymentTypeLabel);
            this.webDeploymentTypeRadioPanel.Controls.Add(this.webDeploymentTypeFtpRadioButton);
            this.webDeploymentTypeRadioPanel.Controls.Add(this.webDeploymentTypePathRadioButton);
            this.webDeploymentTypeRadioPanel.Location = new System.Drawing.Point(3, 46);
            this.webDeploymentTypeRadioPanel.Name = "webDeploymentTypeRadioPanel";
            this.webDeploymentTypeRadioPanel.Size = new System.Drawing.Size(178, 26);
            this.webDeploymentTypeRadioPanel.TabIndex = 6;
            this.webDeploymentTypeRadioPanel.ValueMember = null;
            // 
            // webDeploymentTypeLabel
            // 
            this.webDeploymentTypeLabel.AutoSize = true;
            this.webDeploymentTypeLabel.Location = new System.Drawing.Point(4, 6);
            this.webDeploymentTypeLabel.Name = "webDeploymentTypeLabel";
            this.webDeploymentTypeLabel.Size = new System.Drawing.Size(68, 13);
            this.webDeploymentTypeLabel.TabIndex = 0;
            this.webDeploymentTypeLabel.Text = "Upload Type";
            // 
            // webDeploymentTypeFtpRadioButton
            // 
            this.webDeploymentTypeFtpRadioButton.AutoSize = true;
            this.webDeploymentTypeFtpRadioButton.Location = new System.Drawing.Point(131, 4);
            this.webDeploymentTypeFtpRadioButton.Name = "webDeploymentTypeFtpRadioButton";
            this.webDeploymentTypeFtpRadioButton.Size = new System.Drawing.Size(45, 17);
            this.webDeploymentTypeFtpRadioButton.TabIndex = 2;
            this.webDeploymentTypeFtpRadioButton.TabStop = true;
            this.webDeploymentTypeFtpRadioButton.Tag = "1";
            this.webDeploymentTypeFtpRadioButton.Text = "FTP";
            this.webDeploymentTypeFtpRadioButton.UseVisualStyleBackColor = true;
            // 
            // webDeploymentTypePathRadioButton
            // 
            this.webDeploymentTypePathRadioButton.AutoSize = true;
            this.webDeploymentTypePathRadioButton.Location = new System.Drawing.Point(78, 4);
            this.webDeploymentTypePathRadioButton.Name = "webDeploymentTypePathRadioButton";
            this.webDeploymentTypePathRadioButton.Size = new System.Drawing.Size(47, 17);
            this.webDeploymentTypePathRadioButton.TabIndex = 1;
            this.webDeploymentTypePathRadioButton.TabStop = true;
            this.webDeploymentTypePathRadioButton.Tag = "0";
            this.webDeploymentTypePathRadioButton.Text = "Path";
            this.webDeploymentTypePathRadioButton.UseVisualStyleBackColor = true;
            // 
            // webGenerationPasswordLabel
            // 
            this.webGenerationPasswordLabel.AutoSize = true;
            this.webGenerationPasswordLabel.Location = new System.Drawing.Point(237, 135);
            this.webGenerationPasswordLabel.Name = "webGenerationPasswordLabel";
            this.webGenerationPasswordLabel.Size = new System.Drawing.Size(53, 13);
            this.webGenerationPasswordLabel.TabIndex = 16;
            this.webGenerationPasswordLabel.Text = "Password";
            // 
            // webGenerationUsernameLabel
            // 
            this.webGenerationUsernameLabel.AutoSize = true;
            this.webGenerationUsernameLabel.Location = new System.Drawing.Point(15, 135);
            this.webGenerationUsernameLabel.Name = "webGenerationUsernameLabel";
            this.webGenerationUsernameLabel.Size = new System.Drawing.Size(55, 13);
            this.webGenerationUsernameLabel.TabIndex = 14;
            this.webGenerationUsernameLabel.Text = "Username";
            // 
            // webGenerationServerLabel
            // 
            this.webGenerationServerLabel.AutoSize = true;
            this.webGenerationServerLabel.Location = new System.Drawing.Point(32, 107);
            this.webGenerationServerLabel.Name = "webGenerationServerLabel";
            this.webGenerationServerLabel.Size = new System.Drawing.Size(38, 13);
            this.webGenerationServerLabel.TabIndex = 10;
            this.webGenerationServerLabel.Text = "Server";
            // 
            // webGenerationFtpModeRadioPanel
            // 
            this.webGenerationFtpModeRadioPanel.Controls.Add(this.webDeploymentFtpActiveRadioButton);
            this.webGenerationFtpModeRadioPanel.Controls.Add(this.webDeploymentFtpPassiveRadioButton);
            this.webGenerationFtpModeRadioPanel.Controls.Add(this.lblFtpMode);
            this.webGenerationFtpModeRadioPanel.Location = new System.Drawing.Point(182, 46);
            this.webGenerationFtpModeRadioPanel.Name = "webGenerationFtpModeRadioPanel";
            this.webGenerationFtpModeRadioPanel.Size = new System.Drawing.Size(211, 26);
            this.webGenerationFtpModeRadioPanel.TabIndex = 18;
            this.webGenerationFtpModeRadioPanel.ValueMember = null;
            // 
            // webDeploymentFtpActiveRadioButton
            // 
            this.webDeploymentFtpActiveRadioButton.AutoSize = true;
            this.webDeploymentFtpActiveRadioButton.Location = new System.Drawing.Point(153, 4);
            this.webDeploymentFtpActiveRadioButton.Name = "webDeploymentFtpActiveRadioButton";
            this.webDeploymentFtpActiveRadioButton.Size = new System.Drawing.Size(55, 17);
            this.webDeploymentFtpActiveRadioButton.TabIndex = 2;
            this.webDeploymentFtpActiveRadioButton.Tag = "1";
            this.webDeploymentFtpActiveRadioButton.Text = "Active";
            this.webDeploymentFtpActiveRadioButton.UseVisualStyleBackColor = true;
            // 
            // webDeploymentFtpPassiveRadioButton
            // 
            this.webDeploymentFtpPassiveRadioButton.AutoSize = true;
            this.webDeploymentFtpPassiveRadioButton.Checked = true;
            this.webDeploymentFtpPassiveRadioButton.Location = new System.Drawing.Point(85, 4);
            this.webDeploymentFtpPassiveRadioButton.Name = "webDeploymentFtpPassiveRadioButton";
            this.webDeploymentFtpPassiveRadioButton.Size = new System.Drawing.Size(62, 17);
            this.webDeploymentFtpPassiveRadioButton.TabIndex = 1;
            this.webDeploymentFtpPassiveRadioButton.TabStop = true;
            this.webDeploymentFtpPassiveRadioButton.Tag = "0";
            this.webDeploymentFtpPassiveRadioButton.Text = "Passive";
            this.webDeploymentFtpPassiveRadioButton.UseVisualStyleBackColor = true;
            // 
            // lblFtpMode
            // 
            this.lblFtpMode.AutoSize = true;
            this.lblFtpMode.Location = new System.Drawing.Point(4, 6);
            this.lblFtpMode.Name = "lblFtpMode";
            this.lblFtpMode.Size = new System.Drawing.Size(76, 13);
            this.lblFtpMode.TabIndex = 0;
            this.lblFtpMode.Text = "Transfer Mode";
            // 
            // webGenerationLimitLogSizeLengthUpDown
            // 
            this.webGenerationLimitLogSizeLengthUpDown.Enabled = false;
            this.webGenerationLimitLogSizeLengthUpDown.Location = new System.Drawing.Point(356, 161);
            this.webGenerationLimitLogSizeLengthUpDown.Maximum = new decimal(new int[] {
            10240,
            0,
            0,
            0});
            this.webGenerationLimitLogSizeLengthUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.webGenerationLimitLogSizeLengthUpDown.Name = "webGenerationLimitLogSizeLengthUpDown";
            this.webGenerationLimitLogSizeLengthUpDown.Size = new System.Drawing.Size(62, 20);
            this.webGenerationLimitLogSizeLengthUpDown.TabIndex = 23;
            this.webGenerationLimitLogSizeLengthUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // webGenerationLimitLogSizeCheckBox
            // 
            this.webGenerationLimitLogSizeCheckBox.AutoSize = true;
            this.webGenerationLimitLogSizeCheckBox.Enabled = false;
            this.webGenerationLimitLogSizeCheckBox.Location = new System.Drawing.Point(213, 162);
            this.webGenerationLimitLogSizeCheckBox.Name = "webGenerationLimitLogSizeCheckBox";
            this.webGenerationLimitLogSizeCheckBox.Size = new System.Drawing.Size(139, 17);
            this.webGenerationLimitLogSizeCheckBox.TabIndex = 22;
            this.webGenerationLimitLogSizeCheckBox.Text = "Limit log file size to (KB):";
            this.webGenerationLimitLogSizeCheckBox.UseVisualStyleBackColor = true;
            // 
            // webGenerationCopyXmlCheckBox
            // 
            this.webGenerationCopyXmlCheckBox.AutoSize = true;
            this.webGenerationCopyXmlCheckBox.Enabled = false;
            this.webGenerationCopyXmlCheckBox.Location = new System.Drawing.Point(109, 162);
            this.webGenerationCopyXmlCheckBox.Name = "webGenerationCopyXmlCheckBox";
            this.webGenerationCopyXmlCheckBox.Size = new System.Drawing.Size(48, 17);
            this.webGenerationCopyXmlCheckBox.TabIndex = 20;
            this.webGenerationCopyXmlCheckBox.Text = "XML";
            this.webGenerationCopyXmlCheckBox.UseVisualStyleBackColor = true;
            // 
            // webGenerationCopyHtmlCheckBox
            // 
            this.webGenerationCopyHtmlCheckBox.AutoSize = true;
            this.webGenerationCopyHtmlCheckBox.Enabled = false;
            this.webGenerationCopyHtmlCheckBox.Location = new System.Drawing.Point(47, 162);
            this.webGenerationCopyHtmlCheckBox.Name = "webGenerationCopyHtmlCheckBox";
            this.webGenerationCopyHtmlCheckBox.Size = new System.Drawing.Size(56, 17);
            this.webGenerationCopyHtmlCheckBox.TabIndex = 19;
            this.webGenerationCopyHtmlCheckBox.Text = "HTML";
            this.webGenerationCopyHtmlCheckBox.UseVisualStyleBackColor = true;
            // 
            // webGenerationCopyLogCheckBox
            // 
            this.webGenerationCopyLogCheckBox.AutoSize = true;
            this.webGenerationCopyLogCheckBox.Enabled = false;
            this.webGenerationCopyLogCheckBox.Location = new System.Drawing.Point(163, 162);
            this.webGenerationCopyLogCheckBox.Name = "webGenerationCopyLogCheckBox";
            this.webGenerationCopyLogCheckBox.Size = new System.Drawing.Size(44, 17);
            this.webGenerationCopyLogCheckBox.TabIndex = 21;
            this.webGenerationCopyLogCheckBox.Text = "Log";
            this.webGenerationCopyLogCheckBox.UseVisualStyleBackColor = true;
            // 
            // webGenerationAfterClientRetrievalRadioButton
            // 
            this.webGenerationAfterClientRetrievalRadioButton.AutoSize = true;
            this.webGenerationAfterClientRetrievalRadioButton.Enabled = false;
            this.webGenerationAfterClientRetrievalRadioButton.Location = new System.Drawing.Point(272, 19);
            this.webGenerationAfterClientRetrievalRadioButton.Name = "webGenerationAfterClientRetrievalRadioButton";
            this.webGenerationAfterClientRetrievalRadioButton.Size = new System.Drawing.Size(106, 17);
            this.webGenerationAfterClientRetrievalRadioButton.TabIndex = 4;
            this.webGenerationAfterClientRetrievalRadioButton.TabStop = true;
            this.webGenerationAfterClientRetrievalRadioButton.Text = "After Full Refresh";
            this.webGenerationAfterClientRetrievalRadioButton.UseVisualStyleBackColor = true;
            // 
            // webGenerationOnScheduleRadioButton
            // 
            this.webGenerationOnScheduleRadioButton.AutoSize = true;
            this.webGenerationOnScheduleRadioButton.Checked = true;
            this.webGenerationOnScheduleRadioButton.Enabled = false;
            this.webGenerationOnScheduleRadioButton.Location = new System.Drawing.Point(129, 19);
            this.webGenerationOnScheduleRadioButton.Name = "webGenerationOnScheduleRadioButton";
            this.webGenerationOnScheduleRadioButton.Size = new System.Drawing.Size(52, 17);
            this.webGenerationOnScheduleRadioButton.TabIndex = 1;
            this.webGenerationOnScheduleRadioButton.TabStop = true;
            this.webGenerationOnScheduleRadioButton.Text = "Every";
            this.webGenerationOnScheduleRadioButton.UseVisualStyleBackColor = true;
            // 
            // webGenerationIntervalTextBox
            // 
            this.webGenerationIntervalTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationIntervalTextBox.DoubleBuffered = true;
            this.webGenerationIntervalTextBox.Enabled = false;
            this.webGenerationIntervalTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationIntervalTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationIntervalTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationIntervalTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webGenerationIntervalTextBox.ErrorToolTipText = "";
            this.webGenerationIntervalTextBox.Location = new System.Drawing.Point(181, 18);
            this.webGenerationIntervalTextBox.MaxLength = 3;
            this.webGenerationIntervalTextBox.Name = "webGenerationIntervalTextBox";
            this.webGenerationIntervalTextBox.ReadOnly = true;
            this.webGenerationIntervalTextBox.Size = new System.Drawing.Size(39, 20);
            this.webGenerationIntervalTextBox.TabIndex = 2;
            this.webGenerationIntervalTextBox.Text = "15";
            this.webGenerationIntervalTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.webGenerationIntervalTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // webGenerationIntervalLabel
            // 
            this.webGenerationIntervalLabel.AutoSize = true;
            this.webGenerationIntervalLabel.Enabled = false;
            this.webGenerationIntervalLabel.Location = new System.Drawing.Point(222, 21);
            this.webGenerationIntervalLabel.Name = "webGenerationIntervalLabel";
            this.webGenerationIntervalLabel.Size = new System.Drawing.Size(44, 13);
            this.webGenerationIntervalLabel.TabIndex = 3;
            this.webGenerationIntervalLabel.Text = "Minutes";
            // 
            // webGenerationBrowsePathButton
            // 
            this.webGenerationBrowsePathButton.Enabled = false;
            this.webGenerationBrowsePathButton.Location = new System.Drawing.Point(456, 76);
            this.webGenerationBrowsePathButton.Name = "webGenerationBrowsePathButton";
            this.webGenerationBrowsePathButton.Size = new System.Drawing.Size(24, 23);
            this.webGenerationBrowsePathButton.TabIndex = 9;
            this.webGenerationBrowsePathButton.Text = "...";
            this.webGenerationBrowsePathButton.UseVisualStyleBackColor = true;
            this.webGenerationBrowsePathButton.Click += new System.EventHandler(this.webGenerationBrowsePathButton_Click);
            // 
            // webGenerationPathTextBox
            // 
            this.webGenerationPathTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webGenerationPathTextBox.DoubleBuffered = true;
            this.webGenerationPathTextBox.Enabled = false;
            this.webGenerationPathTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webGenerationPathTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webGenerationPathTextBox.ErrorToolTipDuration = 5000;
            this.webGenerationPathTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -40);
            this.webGenerationPathTextBox.ErrorToolTipText = "";
            this.webGenerationPathTextBox.Location = new System.Drawing.Point(76, 78);
            this.webGenerationPathTextBox.Name = "webGenerationPathTextBox";
            this.webGenerationPathTextBox.ReadOnly = true;
            this.webGenerationPathTextBox.Size = new System.Drawing.Size(374, 20);
            this.webGenerationPathTextBox.TabIndex = 8;
            // 
            // webGenerationPathLabel
            // 
            this.webGenerationPathLabel.AutoSize = true;
            this.webGenerationPathLabel.Location = new System.Drawing.Point(7, 81);
            this.webGenerationPathLabel.Name = "webGenerationPathLabel";
            this.webGenerationPathLabel.Size = new System.Drawing.Size(63, 13);
            this.webGenerationPathLabel.TabIndex = 7;
            this.webGenerationPathLabel.Text = "Target Path";
            // 
            // webGenerationEnabledCheckBox
            // 
            this.webGenerationEnabledCheckBox.AutoSize = true;
            this.webGenerationEnabledCheckBox.Location = new System.Drawing.Point(10, 20);
            this.webGenerationEnabledCheckBox.Name = "webGenerationEnabledCheckBox";
            this.webGenerationEnabledCheckBox.Size = new System.Drawing.Size(113, 17);
            this.webGenerationEnabledCheckBox.TabIndex = 0;
            this.webGenerationEnabledCheckBox.Text = "Create a Web Site";
            this.webGenerationEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // webVisualStylesTab
            // 
            this.webVisualStylesTab.BackColor = System.Drawing.Color.Transparent;
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesSlotBrowseXsltButton);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesSlotTextBox);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesSlotLabel);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesSummaryBrowseXsltButton);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesSummaryTextBox);
            this.webVisualStylesTab.Controls.Add(this.lblSummary);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesOverviewBrowseXsltButton);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesOverviewTextBox);
            this.webVisualStylesTab.Controls.Add(this.lblOverview);
            this.webVisualStylesTab.Controls.Add(this.pnl1CSSSample);
            this.webVisualStylesTab.Controls.Add(this.lbl1Preview);
            this.webVisualStylesTab.Controls.Add(this.webVisualStylesCssFileListBox);
            this.webVisualStylesTab.Controls.Add(this.lbl1Style);
            this.webVisualStylesTab.Location = new System.Drawing.Point(4, 22);
            this.webVisualStylesTab.Name = "webVisualStylesTab";
            this.webVisualStylesTab.Size = new System.Drawing.Size(501, 303);
            this.webVisualStylesTab.TabIndex = 3;
            this.webVisualStylesTab.Text = "Web Visual Styles";
            this.webVisualStylesTab.UseVisualStyleBackColor = true;
            // 
            // webVisualStylesSlotBrowseXsltButton
            // 
            this.webVisualStylesSlotBrowseXsltButton.Location = new System.Drawing.Point(466, 221);
            this.webVisualStylesSlotBrowseXsltButton.Name = "webVisualStylesSlotBrowseXsltButton";
            this.webVisualStylesSlotBrowseXsltButton.Size = new System.Drawing.Size(24, 23);
            this.webVisualStylesSlotBrowseXsltButton.TabIndex = 18;
            this.webVisualStylesSlotBrowseXsltButton.Text = "...";
            this.webVisualStylesSlotBrowseXsltButton.UseVisualStyleBackColor = true;
            this.webVisualStylesSlotBrowseXsltButton.Click += new System.EventHandler(this.webVisualStylesSlotBrowseXsltButton_Click);
            // 
            // webVisualStylesSlotTextBox
            // 
            this.webVisualStylesSlotTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webVisualStylesSlotTextBox.DoubleBuffered = true;
            this.webVisualStylesSlotTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webVisualStylesSlotTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webVisualStylesSlotTextBox.ErrorToolTipDuration = 5000;
            this.webVisualStylesSlotTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webVisualStylesSlotTextBox.ErrorToolTipText = "";
            this.webVisualStylesSlotTextBox.Location = new System.Drawing.Point(132, 223);
            this.webVisualStylesSlotTextBox.Name = "webVisualStylesSlotTextBox";
            this.webVisualStylesSlotTextBox.ReadOnly = true;
            this.webVisualStylesSlotTextBox.Size = new System.Drawing.Size(328, 20);
            this.webVisualStylesSlotTextBox.TabIndex = 17;
            // 
            // webVisualStylesSlotLabel
            // 
            this.webVisualStylesSlotLabel.AutoSize = true;
            this.webVisualStylesSlotLabel.Location = new System.Drawing.Point(68, 226);
            this.webVisualStylesSlotLabel.Name = "webVisualStylesSlotLabel";
            this.webVisualStylesSlotLabel.Size = new System.Drawing.Size(55, 13);
            this.webVisualStylesSlotLabel.TabIndex = 16;
            this.webVisualStylesSlotLabel.Text = "Slot XSLT";
            this.webVisualStylesSlotLabel.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // webVisualStylesSummaryBrowseXsltButton
            // 
            this.webVisualStylesSummaryBrowseXsltButton.Location = new System.Drawing.Point(466, 195);
            this.webVisualStylesSummaryBrowseXsltButton.Name = "webVisualStylesSummaryBrowseXsltButton";
            this.webVisualStylesSummaryBrowseXsltButton.Size = new System.Drawing.Size(24, 23);
            this.webVisualStylesSummaryBrowseXsltButton.TabIndex = 12;
            this.webVisualStylesSummaryBrowseXsltButton.Text = "...";
            this.webVisualStylesSummaryBrowseXsltButton.UseVisualStyleBackColor = true;
            this.webVisualStylesSummaryBrowseXsltButton.Click += new System.EventHandler(this.webVisualStylesSummaryBrowseXsltButton_Click);
            // 
            // webVisualStylesSummaryTextBox
            // 
            this.webVisualStylesSummaryTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webVisualStylesSummaryTextBox.DoubleBuffered = true;
            this.webVisualStylesSummaryTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webVisualStylesSummaryTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webVisualStylesSummaryTextBox.ErrorToolTipDuration = 5000;
            this.webVisualStylesSummaryTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webVisualStylesSummaryTextBox.ErrorToolTipText = "";
            this.webVisualStylesSummaryTextBox.Location = new System.Drawing.Point(132, 197);
            this.webVisualStylesSummaryTextBox.Name = "webVisualStylesSummaryTextBox";
            this.webVisualStylesSummaryTextBox.ReadOnly = true;
            this.webVisualStylesSummaryTextBox.Size = new System.Drawing.Size(328, 20);
            this.webVisualStylesSummaryTextBox.TabIndex = 11;
            // 
            // lblSummary
            // 
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(43, 200);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(80, 13);
            this.lblSummary.TabIndex = 10;
            this.lblSummary.Text = "Summary XSLT";
            this.lblSummary.TextAlign = System.Drawing.ContentAlignment.TopRight;
            // 
            // webVisualStylesOverviewBrowseXsltButton
            // 
            this.webVisualStylesOverviewBrowseXsltButton.Location = new System.Drawing.Point(466, 169);
            this.webVisualStylesOverviewBrowseXsltButton.Name = "webVisualStylesOverviewBrowseXsltButton";
            this.webVisualStylesOverviewBrowseXsltButton.Size = new System.Drawing.Size(24, 23);
            this.webVisualStylesOverviewBrowseXsltButton.TabIndex = 6;
            this.webVisualStylesOverviewBrowseXsltButton.Text = "...";
            this.webVisualStylesOverviewBrowseXsltButton.UseVisualStyleBackColor = true;
            this.webVisualStylesOverviewBrowseXsltButton.Click += new System.EventHandler(this.webVisualStylesOverviewBrowseXsltButton_Click);
            // 
            // webVisualStylesOverviewTextBox
            // 
            this.webVisualStylesOverviewTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webVisualStylesOverviewTextBox.DoubleBuffered = true;
            this.webVisualStylesOverviewTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webVisualStylesOverviewTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webVisualStylesOverviewTextBox.ErrorToolTipDuration = 5000;
            this.webVisualStylesOverviewTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webVisualStylesOverviewTextBox.ErrorToolTipText = "";
            this.webVisualStylesOverviewTextBox.Location = new System.Drawing.Point(132, 171);
            this.webVisualStylesOverviewTextBox.Name = "webVisualStylesOverviewTextBox";
            this.webVisualStylesOverviewTextBox.ReadOnly = true;
            this.webVisualStylesOverviewTextBox.Size = new System.Drawing.Size(328, 20);
            this.webVisualStylesOverviewTextBox.TabIndex = 5;
            // 
            // lblOverview
            // 
            this.lblOverview.AutoSize = true;
            this.lblOverview.Location = new System.Drawing.Point(41, 174);
            this.lblOverview.Name = "lblOverview";
            this.lblOverview.Size = new System.Drawing.Size(82, 13);
            this.lblOverview.TabIndex = 4;
            this.lblOverview.Text = "Overview XSLT";
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
            // reportingTab
            // 
            this.reportingTab.Controls.Add(this.reportingSelectionsGroupBox);
            this.reportingTab.Controls.Add(this.reportingSettingsGroupBox);
            this.reportingTab.Location = new System.Drawing.Point(4, 22);
            this.reportingTab.Name = "reportingTab";
            this.reportingTab.Size = new System.Drawing.Size(501, 303);
            this.reportingTab.TabIndex = 5;
            this.reportingTab.Text = "Reporting";
            this.reportingTab.UseVisualStyleBackColor = true;
            // 
            // reportingSelectionsGroupBox
            // 
            this.reportingSelectionsGroupBox.Enabled = false;
            this.reportingSelectionsGroupBox.Location = new System.Drawing.Point(6, 179);
            this.reportingSelectionsGroupBox.Name = "reportingSelectionsGroupBox";
            this.reportingSelectionsGroupBox.Size = new System.Drawing.Size(489, 114);
            this.reportingSelectionsGroupBox.TabIndex = 1;
            this.reportingSelectionsGroupBox.TabStop = false;
            this.reportingSelectionsGroupBox.Text = "Report Selections";
            this.reportingSelectionsGroupBox.EnabledChanged += new System.EventHandler(this.reportingSelectionsGroupBox_EnabledChanged);
            // 
            // reportingSettingsGroupBox
            // 
            this.reportingSettingsGroupBox.Controls.Add(this.reportingSendTestEmailLinkLabel);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingPortTextBox);
            this.reportingSettingsGroupBox.Controls.Add(this.labelWrapper3);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingIsSecureCheckBox);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingPasswordTextBox);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingUsernameTextBox);
            this.reportingSettingsGroupBox.Controls.Add(this.labelWrapper4);
            this.reportingSettingsGroupBox.Controls.Add(this.labelWrapper5);
            this.reportingSettingsGroupBox.Controls.Add(this.lblFromEmailAddress);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingFromAddressTextBox);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingEnabledCheckBox);
            this.reportingSettingsGroupBox.Controls.Add(this.lblSmtpServer);
            this.reportingSettingsGroupBox.Controls.Add(this.lblToAddress);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingServerTextBox);
            this.reportingSettingsGroupBox.Controls.Add(this.reportingToAddressTextBox);
            this.reportingSettingsGroupBox.Location = new System.Drawing.Point(6, 9);
            this.reportingSettingsGroupBox.Name = "reportingSettingsGroupBox";
            this.reportingSettingsGroupBox.Size = new System.Drawing.Size(489, 164);
            this.reportingSettingsGroupBox.TabIndex = 0;
            this.reportingSettingsGroupBox.TabStop = false;
            this.reportingSettingsGroupBox.Text = "Email Settings";
            // 
            // reportingSendTestEmailLinkLabel
            // 
            this.reportingSendTestEmailLinkLabel.AutoSize = true;
            this.reportingSendTestEmailLinkLabel.Location = new System.Drawing.Point(384, 21);
            this.reportingSendTestEmailLinkLabel.Name = "reportingSendTestEmailLinkLabel";
            this.reportingSendTestEmailLinkLabel.Size = new System.Drawing.Size(84, 13);
            this.reportingSendTestEmailLinkLabel.TabIndex = 26;
            this.reportingSendTestEmailLinkLabel.TabStop = true;
            this.reportingSendTestEmailLinkLabel.Text = "Send Test Email";
            this.reportingSendTestEmailLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.reportingSendTestEmailLinkLabel_LinkClicked);
            // 
            // reportingPortTextBox
            // 
            this.reportingPortTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingPortTextBox.DoubleBuffered = true;
            this.reportingPortTextBox.Enabled = false;
            this.reportingPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingPortTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingPortTextBox.ErrorToolTipDuration = 5000;
            this.reportingPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingPortTextBox.ErrorToolTipText = "";
            this.reportingPortTextBox.Location = new System.Drawing.Point(415, 103);
            this.reportingPortTextBox.MaxLength = 200;
            this.reportingPortTextBox.Name = "reportingPortTextBox";
            this.reportingPortTextBox.ReadOnly = true;
            this.reportingPortTextBox.Size = new System.Drawing.Size(54, 20);
            this.reportingPortTextBox.TabIndex = 9;
            this.reportingPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // labelWrapper3
            // 
            this.labelWrapper3.AutoSize = true;
            this.labelWrapper3.Location = new System.Drawing.Point(380, 106);
            this.labelWrapper3.Name = "labelWrapper3";
            this.labelWrapper3.Size = new System.Drawing.Size(26, 13);
            this.labelWrapper3.TabIndex = 8;
            this.labelWrapper3.Text = "Port";
            // 
            // reportingIsSecureCheckBox
            // 
            this.reportingIsSecureCheckBox.AutoSize = true;
            this.reportingIsSecureCheckBox.Enabled = false;
            this.reportingIsSecureCheckBox.Location = new System.Drawing.Point(152, 20);
            this.reportingIsSecureCheckBox.Name = "reportingIsSecureCheckBox";
            this.reportingIsSecureCheckBox.Size = new System.Drawing.Size(168, 17);
            this.reportingIsSecureCheckBox.TabIndex = 1;
            this.reportingIsSecureCheckBox.Text = "Use Secure Connection (SSL)";
            this.reportingIsSecureCheckBox.UseVisualStyleBackColor = true;
            // 
            // reportingPasswordTextBox
            // 
            this.reportingPasswordTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingPasswordTextBox.DoubleBuffered = true;
            this.reportingPasswordTextBox.Enabled = false;
            this.reportingPasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingPasswordTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingPasswordTextBox.ErrorToolTipDuration = 5000;
            this.reportingPasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingPasswordTextBox.ErrorToolTipText = "";
            this.reportingPasswordTextBox.Location = new System.Drawing.Point(314, 129);
            this.reportingPasswordTextBox.MaxLength = 100;
            this.reportingPasswordTextBox.Name = "reportingPasswordTextBox";
            this.reportingPasswordTextBox.ReadOnly = true;
            this.reportingPasswordTextBox.Size = new System.Drawing.Size(155, 20);
            this.reportingPasswordTextBox.TabIndex = 13;
            this.reportingPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // reportingUsernameTextBox
            // 
            this.reportingUsernameTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingUsernameTextBox.DoubleBuffered = true;
            this.reportingUsernameTextBox.Enabled = false;
            this.reportingUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingUsernameTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingUsernameTextBox.ErrorToolTipDuration = 5000;
            this.reportingUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingUsernameTextBox.ErrorToolTipText = "";
            this.reportingUsernameTextBox.Location = new System.Drawing.Point(92, 129);
            this.reportingUsernameTextBox.MaxLength = 100;
            this.reportingUsernameTextBox.Name = "reportingUsernameTextBox";
            this.reportingUsernameTextBox.ReadOnly = true;
            this.reportingUsernameTextBox.Size = new System.Drawing.Size(155, 20);
            this.reportingUsernameTextBox.TabIndex = 11;
            // 
            // labelWrapper4
            // 
            this.labelWrapper4.AutoSize = true;
            this.labelWrapper4.Location = new System.Drawing.Point(253, 132);
            this.labelWrapper4.Name = "labelWrapper4";
            this.labelWrapper4.Size = new System.Drawing.Size(53, 13);
            this.labelWrapper4.TabIndex = 12;
            this.labelWrapper4.Text = "Password";
            // 
            // labelWrapper5
            // 
            this.labelWrapper5.AutoSize = true;
            this.labelWrapper5.Location = new System.Drawing.Point(27, 132);
            this.labelWrapper5.Name = "labelWrapper5";
            this.labelWrapper5.Size = new System.Drawing.Size(55, 13);
            this.labelWrapper5.TabIndex = 10;
            this.labelWrapper5.Text = "Username";
            // 
            // lblFromEmailAddress
            // 
            this.lblFromEmailAddress.AutoSize = true;
            this.lblFromEmailAddress.Location = new System.Drawing.Point(11, 80);
            this.lblFromEmailAddress.Name = "lblFromEmailAddress";
            this.lblFromEmailAddress.Size = new System.Drawing.Size(71, 13);
            this.lblFromEmailAddress.TabIndex = 4;
            this.lblFromEmailAddress.Text = "From Address";
            // 
            // reportingFromAddressTextBox
            // 
            this.reportingFromAddressTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingFromAddressTextBox.DoubleBuffered = true;
            this.reportingFromAddressTextBox.Enabled = false;
            this.reportingFromAddressTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingFromAddressTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingFromAddressTextBox.ErrorToolTipDuration = 5000;
            this.reportingFromAddressTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingFromAddressTextBox.ErrorToolTipText = "";
            this.reportingFromAddressTextBox.Location = new System.Drawing.Point(92, 77);
            this.reportingFromAddressTextBox.MaxLength = 200;
            this.reportingFromAddressTextBox.Name = "reportingFromAddressTextBox";
            this.reportingFromAddressTextBox.ReadOnly = true;
            this.reportingFromAddressTextBox.Size = new System.Drawing.Size(377, 20);
            this.reportingFromAddressTextBox.TabIndex = 5;
            this.reportingFromAddressTextBox.MouseHover += new System.EventHandler(this.reportingFromAddressTextBox_MouseHover);
            // 
            // reportingEnabledCheckBox
            // 
            this.reportingEnabledCheckBox.AutoSize = true;
            this.reportingEnabledCheckBox.Location = new System.Drawing.Point(10, 20);
            this.reportingEnabledCheckBox.Name = "reportingEnabledCheckBox";
            this.reportingEnabledCheckBox.Size = new System.Drawing.Size(136, 17);
            this.reportingEnabledCheckBox.TabIndex = 0;
            this.reportingEnabledCheckBox.Text = "Enable Email Reporting";
            this.reportingEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // lblSmtpServer
            // 
            this.lblSmtpServer.AutoSize = true;
            this.lblSmtpServer.Location = new System.Drawing.Point(11, 106);
            this.lblSmtpServer.Name = "lblSmtpServer";
            this.lblSmtpServer.Size = new System.Drawing.Size(71, 13);
            this.lblSmtpServer.TabIndex = 6;
            this.lblSmtpServer.Text = "SMTP Server";
            // 
            // lblToAddress
            // 
            this.lblToAddress.AutoSize = true;
            this.lblToAddress.Location = new System.Drawing.Point(21, 54);
            this.lblToAddress.Name = "lblToAddress";
            this.lblToAddress.Size = new System.Drawing.Size(61, 13);
            this.lblToAddress.TabIndex = 2;
            this.lblToAddress.Text = "To Address";
            // 
            // reportingServerTextBox
            // 
            this.reportingServerTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingServerTextBox.DoubleBuffered = true;
            this.reportingServerTextBox.Enabled = false;
            this.reportingServerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingServerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingServerTextBox.ErrorToolTipDuration = 5000;
            this.reportingServerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingServerTextBox.ErrorToolTipText = "";
            this.reportingServerTextBox.Location = new System.Drawing.Point(92, 103);
            this.reportingServerTextBox.MaxLength = 200;
            this.reportingServerTextBox.Name = "reportingServerTextBox";
            this.reportingServerTextBox.ReadOnly = true;
            this.reportingServerTextBox.Size = new System.Drawing.Size(282, 20);
            this.reportingServerTextBox.TabIndex = 7;
            // 
            // reportingToAddressTextBox
            // 
            this.reportingToAddressTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.reportingToAddressTextBox.DoubleBuffered = true;
            this.reportingToAddressTextBox.Enabled = false;
            this.reportingToAddressTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.reportingToAddressTextBox.ErrorToolTip = this.toolTipPrefs;
            this.reportingToAddressTextBox.ErrorToolTipDuration = 5000;
            this.reportingToAddressTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.reportingToAddressTextBox.ErrorToolTipText = "";
            this.reportingToAddressTextBox.Location = new System.Drawing.Point(92, 51);
            this.reportingToAddressTextBox.MaxLength = 200;
            this.reportingToAddressTextBox.Name = "reportingToAddressTextBox";
            this.reportingToAddressTextBox.ReadOnly = true;
            this.reportingToAddressTextBox.Size = new System.Drawing.Size(377, 20);
            this.reportingToAddressTextBox.TabIndex = 3;
            // 
            // webProxyTab
            // 
            this.webProxyTab.BackColor = System.Drawing.Color.Transparent;
            this.webProxyTab.Controls.Add(this.webProxyGroupBox);
            this.webProxyTab.Location = new System.Drawing.Point(4, 22);
            this.webProxyTab.Name = "webProxyTab";
            this.webProxyTab.Padding = new System.Windows.Forms.Padding(3);
            this.webProxyTab.Size = new System.Drawing.Size(501, 303);
            this.webProxyTab.TabIndex = 1;
            this.webProxyTab.Text = "Proxy";
            this.webProxyTab.UseVisualStyleBackColor = true;
            // 
            // webProxyGroupBox
            // 
            this.webProxyGroupBox.Controls.Add(this.webProxyEnabledCheckBox);
            this.webProxyGroupBox.Controls.Add(this.webProxyCredentialsEnabledCheckBox);
            this.webProxyGroupBox.Controls.Add(this.webProxyPasswordTextBox);
            this.webProxyGroupBox.Controls.Add(this.webProxyUsernameTextBox);
            this.webProxyGroupBox.Controls.Add(this.webProxyPortTextBox);
            this.webProxyGroupBox.Controls.Add(this.lbl3ProxyPass);
            this.webProxyGroupBox.Controls.Add(this.webProxyServerTextBox);
            this.webProxyGroupBox.Controls.Add(this.lbl3ProxyUser);
            this.webProxyGroupBox.Controls.Add(this.lbl3Port);
            this.webProxyGroupBox.Controls.Add(this.lbl3Proxy);
            this.webProxyGroupBox.Location = new System.Drawing.Point(6, 9);
            this.webProxyGroupBox.Name = "webProxyGroupBox";
            this.webProxyGroupBox.Size = new System.Drawing.Size(489, 121);
            this.webProxyGroupBox.TabIndex = 2;
            this.webProxyGroupBox.TabStop = false;
            this.webProxyGroupBox.Text = "Web Proxy Settings";
            // 
            // webProxyEnabledCheckBox
            // 
            this.webProxyEnabledCheckBox.AutoSize = true;
            this.webProxyEnabledCheckBox.Location = new System.Drawing.Point(6, 17);
            this.webProxyEnabledCheckBox.Name = "webProxyEnabledCheckBox";
            this.webProxyEnabledCheckBox.Size = new System.Drawing.Size(117, 17);
            this.webProxyEnabledCheckBox.TabIndex = 0;
            this.webProxyEnabledCheckBox.Text = "Use a Proxy Server";
            this.webProxyEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // webProxyCredentialsEnabledCheckBox
            // 
            this.webProxyCredentialsEnabledCheckBox.AutoSize = true;
            this.webProxyCredentialsEnabledCheckBox.Enabled = false;
            this.webProxyCredentialsEnabledCheckBox.Location = new System.Drawing.Point(6, 66);
            this.webProxyCredentialsEnabledCheckBox.Name = "webProxyCredentialsEnabledCheckBox";
            this.webProxyCredentialsEnabledCheckBox.Size = new System.Drawing.Size(205, 17);
            this.webProxyCredentialsEnabledCheckBox.TabIndex = 5;
            this.webProxyCredentialsEnabledCheckBox.Text = "Authenticate to the Web Proxy Server";
            this.webProxyCredentialsEnabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // webProxyPasswordTextBox
            // 
            this.webProxyPasswordTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webProxyPasswordTextBox.DoubleBuffered = true;
            this.webProxyPasswordTextBox.Enabled = false;
            this.webProxyPasswordTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webProxyPasswordTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webProxyPasswordTextBox.ErrorToolTipDuration = 5000;
            this.webProxyPasswordTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webProxyPasswordTextBox.ErrorToolTipText = "";
            this.webProxyPasswordTextBox.Location = new System.Drawing.Point(294, 89);
            this.webProxyPasswordTextBox.Name = "webProxyPasswordTextBox";
            this.webProxyPasswordTextBox.ReadOnly = true;
            this.webProxyPasswordTextBox.Size = new System.Drawing.Size(155, 20);
            this.webProxyPasswordTextBox.TabIndex = 9;
            this.webProxyPasswordTextBox.UseSystemPasswordChar = true;
            // 
            // webProxyUsernameTextBox
            // 
            this.webProxyUsernameTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webProxyUsernameTextBox.DoubleBuffered = true;
            this.webProxyUsernameTextBox.Enabled = false;
            this.webProxyUsernameTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webProxyUsernameTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webProxyUsernameTextBox.ErrorToolTipDuration = 5000;
            this.webProxyUsernameTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webProxyUsernameTextBox.ErrorToolTipText = "";
            this.webProxyUsernameTextBox.Location = new System.Drawing.Point(71, 89);
            this.webProxyUsernameTextBox.Name = "webProxyUsernameTextBox";
            this.webProxyUsernameTextBox.ReadOnly = true;
            this.webProxyUsernameTextBox.Size = new System.Drawing.Size(155, 20);
            this.webProxyUsernameTextBox.TabIndex = 7;
            // 
            // webProxyPortTextBox
            // 
            this.webProxyPortTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webProxyPortTextBox.DoubleBuffered = true;
            this.webProxyPortTextBox.Enabled = false;
            this.webProxyPortTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webProxyPortTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webProxyPortTextBox.ErrorToolTipDuration = 5000;
            this.webProxyPortTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webProxyPortTextBox.ErrorToolTipText = "";
            this.webProxyPortTextBox.Location = new System.Drawing.Point(395, 40);
            this.webProxyPortTextBox.MaxLength = 5;
            this.webProxyPortTextBox.Name = "webProxyPortTextBox";
            this.webProxyPortTextBox.ReadOnly = true;
            this.webProxyPortTextBox.Size = new System.Drawing.Size(54, 20);
            this.webProxyPortTextBox.TabIndex = 4;
            this.webProxyPortTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.TextBoxDigitsOnlyKeyPress);
            // 
            // lbl3ProxyPass
            // 
            this.lbl3ProxyPass.AutoSize = true;
            this.lbl3ProxyPass.Location = new System.Drawing.Point(232, 92);
            this.lbl3ProxyPass.Name = "lbl3ProxyPass";
            this.lbl3ProxyPass.Size = new System.Drawing.Size(53, 13);
            this.lbl3ProxyPass.TabIndex = 8;
            this.lbl3ProxyPass.Text = "Password";
            // 
            // webProxyServerTextBox
            // 
            this.webProxyServerTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.webProxyServerTextBox.DoubleBuffered = true;
            this.webProxyServerTextBox.Enabled = false;
            this.webProxyServerTextBox.ErrorBackColor = System.Drawing.Color.Yellow;
            this.webProxyServerTextBox.ErrorToolTip = this.toolTipPrefs;
            this.webProxyServerTextBox.ErrorToolTipDuration = 5000;
            this.webProxyServerTextBox.ErrorToolTipPoint = new System.Drawing.Point(10, -20);
            this.webProxyServerTextBox.ErrorToolTipText = "";
            this.webProxyServerTextBox.Location = new System.Drawing.Point(71, 40);
            this.webProxyServerTextBox.Name = "webProxyServerTextBox";
            this.webProxyServerTextBox.ReadOnly = true;
            this.webProxyServerTextBox.Size = new System.Drawing.Size(282, 20);
            this.webProxyServerTextBox.TabIndex = 2;
            // 
            // lbl3ProxyUser
            // 
            this.lbl3ProxyUser.AutoSize = true;
            this.lbl3ProxyUser.Location = new System.Drawing.Point(7, 92);
            this.lbl3ProxyUser.Name = "lbl3ProxyUser";
            this.lbl3ProxyUser.Size = new System.Drawing.Size(55, 13);
            this.lbl3ProxyUser.TabIndex = 6;
            this.lbl3ProxyUser.Text = "Username";
            // 
            // lbl3Port
            // 
            this.lbl3Port.AutoSize = true;
            this.lbl3Port.Location = new System.Drawing.Point(360, 43);
            this.lbl3Port.Name = "lbl3Port";
            this.lbl3Port.Size = new System.Drawing.Size(26, 13);
            this.lbl3Port.TabIndex = 3;
            this.lbl3Port.Text = "Port";
            // 
            // lbl3Proxy
            // 
            this.lbl3Proxy.AutoSize = true;
            this.lbl3Proxy.Location = new System.Drawing.Point(24, 43);
            this.lbl3Proxy.Name = "lbl3Proxy";
            this.lbl3Proxy.Size = new System.Drawing.Size(38, 13);
            this.lbl3Proxy.TabIndex = 1;
            this.lbl3Proxy.Text = "Server";
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(366, 352);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(75, 23);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(447, 352);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(75, 23);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // PreferencesDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(532, 388);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cancelButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "PreferencesDialog";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Preferences";
            this.Load += new System.EventHandler(this.PreferencesDialog_Load);
            this.tabControl1.ResumeLayout(false);
            this.clientsTab.ResumeLayout(false);
            this.clientsRefreshClientDataGroupBox.ResumeLayout(false);
            this.clientsRefreshClientDataGroupBox.PerformLayout();
            this.clientsConfigurationGroupBox.ResumeLayout(false);
            this.clientsConfigurationGroupBox.PerformLayout();
            this.optionsDisplayProductionOptionsGroupBox.ResumeLayout(false);
            this.optionsDisplayProductionOptionsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.clientsDecimalPlacesUpDown)).EndInit();
            this.optionsTab.ResumeLayout(false);
            this.optionsIdentityGroupBox.ResumeLayout(false);
            this.optionsIdentityGroupBox.PerformLayout();
            this.optionsMinimizeToOptionGroupBox.ResumeLayout(false);
            this.optionsMinimizeToOptionGroupBox.PerformLayout();
            this.optionsMessageLevelGroupBox.ResumeLayout(false);
            this.optionsMessageLevelGroupBox.PerformLayout();
            this.optionsExternalProgramsGroupBox.ResumeLayout(false);
            this.optionsExternalProgramsGroupBox.PerformLayout();
            this.optionsStartupGroupBox.ResumeLayout(false);
            this.optionsStartupGroupBox.PerformLayout();
            this.webGenerationTab.ResumeLayout(false);
            this.webGenerationGroupBox.ResumeLayout(false);
            this.webGenerationGroupBox.PerformLayout();
            this.webDeploymentTypeRadioPanel.ResumeLayout(false);
            this.webDeploymentTypeRadioPanel.PerformLayout();
            this.webGenerationFtpModeRadioPanel.ResumeLayout(false);
            this.webGenerationFtpModeRadioPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.webGenerationLimitLogSizeLengthUpDown)).EndInit();
            this.webVisualStylesTab.ResumeLayout(false);
            this.webVisualStylesTab.PerformLayout();
            this.reportingTab.ResumeLayout(false);
            this.reportingSettingsGroupBox.ResumeLayout(false);
            this.reportingSettingsGroupBox.PerformLayout();
            this.webProxyTab.ResumeLayout(false);
            this.webProxyGroupBox.ResumeLayout(false);
            this.webProxyGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Panel pnl1CSSSample;
        private System.Windows.Forms.ListBox webVisualStylesCssFileListBox;
        private System.Windows.Forms.Label lbl1Style;
        private System.Windows.Forms.Label lbl1Preview;
        private System.Windows.Forms.GroupBox webGenerationGroupBox;
        private System.Windows.Forms.Label webGenerationPathLabel;
        private System.Windows.Forms.Button webGenerationBrowsePathButton;
        private System.Windows.Forms.Label webGenerationIntervalLabel;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage webProxyTab;
        private System.Windows.Forms.TabPage webVisualStylesTab;
        private System.Windows.Forms.TabPage webGenerationTab;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationIntervalTextBox;
        private System.Windows.Forms.CheckBox webGenerationEnabledCheckBox;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationPathTextBox;
        private System.Windows.Forms.GroupBox webProxyGroupBox;
        private HFM.Forms.Controls.DataErrorTextBox webProxyServerTextBox;
        private System.Windows.Forms.Label lbl3Proxy;
        private HFM.Forms.Controls.DataErrorTextBox webProxyPasswordTextBox;
        private HFM.Forms.Controls.DataErrorTextBox webProxyUsernameTextBox;
        private HFM.Forms.Controls.DataErrorTextBox webProxyPortTextBox;
        private System.Windows.Forms.Label lbl3ProxyPass;
        private System.Windows.Forms.Label lbl3ProxyUser;
        private System.Windows.Forms.Label lbl3Port;
        private System.Windows.Forms.CheckBox webProxyCredentialsEnabledCheckBox;
        private System.Windows.Forms.CheckBox webProxyEnabledCheckBox;
        private System.Windows.Forms.TabPage clientsTab;
        private System.Windows.Forms.RadioButton webGenerationOnScheduleRadioButton;
        private System.Windows.Forms.RadioButton webGenerationAfterClientRetrievalRadioButton;
        private System.Windows.Forms.ToolTip toolTipPrefs;
        private System.Windows.Forms.Label labelWrapper1;
        private System.Windows.Forms.NumericUpDown clientsDecimalPlacesUpDown;
        private System.Windows.Forms.TabPage reportingTab;
        private System.Windows.Forms.GroupBox reportingSettingsGroupBox;
        private System.Windows.Forms.Label lblSmtpServer;
        private System.Windows.Forms.Label lblToAddress;
        private HFM.Forms.Controls.DataErrorTextBox reportingServerTextBox;
        private HFM.Forms.Controls.DataErrorTextBox reportingToAddressTextBox;
        private System.Windows.Forms.CheckBox reportingEnabledCheckBox;
        private System.Windows.Forms.Label lblFromEmailAddress;
        private HFM.Forms.Controls.DataErrorTextBox reportingFromAddressTextBox;
        private HFM.Forms.Controls.DataErrorTextBox reportingPasswordTextBox;
        private HFM.Forms.Controls.DataErrorTextBox reportingUsernameTextBox;
        private System.Windows.Forms.Label labelWrapper4;
        private System.Windows.Forms.Label labelWrapper5;
        private System.Windows.Forms.GroupBox reportingSelectionsGroupBox;
        private System.Windows.Forms.TabPage optionsTab;
        private System.Windows.Forms.GroupBox optionsStartupGroupBox;
        private System.Windows.Forms.CheckBox optionsRunMinimizedCheckBox;
        private System.Windows.Forms.CheckBox optionsAutoRunCheckBox;
        private System.Windows.Forms.CheckBox webGenerationCopyLogCheckBox;
        private System.Windows.Forms.GroupBox optionsDisplayProductionOptionsGroupBox;
        private System.Windows.Forms.CheckBox clientsAutoSaveConfig;
        private System.Windows.Forms.CheckBox clientsColorLogFileCheckBox;
        private System.Windows.Forms.CheckBox clientsOfflineLastCheckBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox clientsPPDCalculationComboBox;
        private System.Windows.Forms.RadioButton webDeploymentFtpActiveRadioButton;
        private System.Windows.Forms.RadioButton webDeploymentFtpPassiveRadioButton;
        private System.Windows.Forms.Label lblFtpMode;
        private System.Windows.Forms.Button webVisualStylesSlotBrowseXsltButton;
        private HFM.Forms.Controls.DataErrorTextBox webVisualStylesSlotTextBox;
        private System.Windows.Forms.Label webVisualStylesSlotLabel;
        private System.Windows.Forms.Button webVisualStylesSummaryBrowseXsltButton;
        private HFM.Forms.Controls.DataErrorTextBox webVisualStylesSummaryTextBox;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Button webVisualStylesOverviewBrowseXsltButton;
        private HFM.Forms.Controls.DataErrorTextBox webVisualStylesOverviewTextBox;
        private System.Windows.Forms.Label lblOverview;
        private System.Windows.Forms.CheckBox reportingIsSecureCheckBox;
        private HFM.Forms.Controls.DataErrorTextBox reportingPortTextBox;
        private System.Windows.Forms.Label labelWrapper3;
        private System.Windows.Forms.CheckBox optionsStartupCheckForUpdateCheckBox;
        private System.Windows.Forms.GroupBox optionsExternalProgramsGroupBox;
        private System.Windows.Forms.Button optionsBrowseLogFileViewerButton;
        private System.Windows.Forms.Label label3;
        private HFM.Forms.Controls.DataErrorTextBox optionsLogFileViewerTextBox;
        private System.Windows.Forms.CheckBox webGenerationCopyHtmlCheckBox;
        private System.Windows.Forms.CheckBox webGenerationCopyXmlCheckBox;
        private System.Windows.Forms.NumericUpDown webGenerationLimitLogSizeLengthUpDown;
        private System.Windows.Forms.CheckBox webGenerationLimitLogSizeCheckBox;
        private HFM.Forms.Controls.RadioPanel webGenerationFtpModeRadioPanel;
        private System.Windows.Forms.CheckBox clientsDisplayETADateCheckBox;
        private System.Windows.Forms.Label webGenerationServerLabel;
        private System.Windows.Forms.Label webGenerationPasswordLabel;
        private System.Windows.Forms.Label webGenerationUsernameLabel;
        private System.Windows.Forms.CheckBox clientsDuplicateProjectCheckBox;
        private HFM.Forms.Controls.RadioPanel webDeploymentTypeRadioPanel;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationPasswordTextBox;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationUsernameTextBox;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationServerTextBox;
        private System.Windows.Forms.Label webDeploymentTypeLabel;
        private System.Windows.Forms.RadioButton webDeploymentTypeFtpRadioButton;
        private System.Windows.Forms.RadioButton webDeploymentTypePathRadioButton;
        private HFM.Forms.Controls.DataErrorTextBox webGenerationPortTextBox;
        private System.Windows.Forms.Label webGenerationPortLabel;
        private System.Windows.Forms.Label labelWrapper6;
        private System.Windows.Forms.ComboBox clientsBonusCalculationComboBox;
        private System.Windows.Forms.Label webGenerationCopyLabel;
        private System.Windows.Forms.Button optionsBrowseFileExplorerButton;
        private System.Windows.Forms.Label label4;
        private Controls.DataErrorTextBox optionsFileExplorerTextBox;
        private System.Windows.Forms.GroupBox optionsMinimizeToOptionGroupBox;
        private System.Windows.Forms.ComboBox optionsMinimizeToOptionComboBox;
        private System.Windows.Forms.Label labelWrapper2;
        private System.Windows.Forms.GroupBox optionsMessageLevelGroupBox;
        private System.Windows.Forms.ComboBox optionsMessageLevelComboBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.GroupBox clientsConfigurationGroupBox;
        private System.Windows.Forms.CheckBox clientsDefaultConfigFileEnabledCheckBox;
        private System.Windows.Forms.Button clientsBrowseConfigFileButton;
        private Controls.DataErrorTextBox clientsDefaultConfigFileTextBox;
        private System.Windows.Forms.GroupBox optionsIdentityGroupBox;
        private System.Windows.Forms.Label optionsEocUserIDLabel;
        private System.Windows.Forms.Label optionsFahUserIDLabel;
        private System.Windows.Forms.LinkLabel optionsTestFahTeamIDLinkLabel;
        private Controls.DataErrorTextBox optionsEocUserIDTextBox;
        private Controls.DataErrorTextBox optionsFahTeamIDTextBox;
        private System.Windows.Forms.Label optionsFahTeamIDLabel;
        private System.Windows.Forms.LinkLabel optionsTestFahUserIDLinkLabel;
        private Controls.DataErrorTextBox optionsFahUserIDTextBox;
        private System.Windows.Forms.LinkLabel optionsTestEocUserIDLinkLabel;
        private System.Windows.Forms.CheckBox optionsEocUserStatsEnabledCheckBox;
        private System.Windows.Forms.GroupBox clientsRefreshClientDataGroupBox;
        private Controls.DataErrorTextBox clientsRetrievalIntervalTextBox;
        private System.Windows.Forms.Label lbl2SchedExplain;
        private System.Windows.Forms.CheckBox clientsRetrievalEnabledCheckBox;
        private System.Windows.Forms.CheckBox clientsRetrievalIsSerialCheckBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel webGenerationTestConnectionLinkLabel;
        private System.Windows.Forms.LinkLabel reportingSendTestEmailLinkLabel;
    }
}
