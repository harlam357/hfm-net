﻿
using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using HFM.Core.Services;
using HFM.Forms.Models;
using HFM.Forms.Controls;
using HFM.Preferences;

namespace HFM.Forms
{
    public partial class PreferencesDialog : FormWrapper, IWin32Dialog
    {
        /// <summary>
        /// Tab Name Enumeration (maintain in same order as tab pages)
        /// </summary>
        private enum TabName
        {
            //ScheduledTasks = 0,
            //StartupAndExternal = 1,
            //Options = 2,
            //Reporting = 3,
            //WebSettings = 4,
            WebVisualStyles = 5
        }

        #region Fields

        private readonly PreferencesPresenter _presenter;

        private readonly WebBrowser _cssSampleBrowser;

        #endregion

        private const int MaxDecimalPlaces = 5;

        #region Constructor And Binding/Load Methods

        public PreferencesDialog(PreferencesPresenter presenter)
        {
            _presenter = presenter;

            InitializeComponent();

            udDecimalPlaces.Minimum = 0;
            udDecimalPlaces.Maximum = MaxDecimalPlaces;

            if (!Core.Application.IsRunningOnMono)
            {
                _cssSampleBrowser = new WebBrowser();

                pnl1CSSSample.Controls.Add(_cssSampleBrowser);

                _cssSampleBrowser.Dock = DockStyle.Fill;
                _cssSampleBrowser.Location = new Point(0, 0);
                _cssSampleBrowser.MinimumSize = new Size(20, 20);
                _cssSampleBrowser.Name = nameof(_cssSampleBrowser);
                _cssSampleBrowser.Size = new Size(354, 208);
                _cssSampleBrowser.TabIndex = 0;
                _cssSampleBrowser.TabStop = false;
            }
        }

        private void PreferencesDialogLoad(object sender, EventArgs e)
        {
            LoadScheduledTasksTab();
            LoadStartupTab();
            LoadOptionsTab();
            LoadReportingTab();
            LoadWebSettingsTab();
            LoadVisualStylesTab();

            // Cycle through Tabs to create all controls and Bind data
            for (int i = 0; i < tabControl1.TabPages.Count; i++)
            {
                tabControl1.SelectTab(i);
                if (tabControl1.SelectedIndex == (int)TabName.WebVisualStyles)
                {
                    ShowCssPreview();
                }
            }
            tabControl1.SelectTab(0);

            _presenter.Model.ScheduledTasksModel.PropertyChanged += ScheduledTasksPropertyChanged;
            _presenter.Model.StartupAndExternalModel.PropertyChanged += StartupAndExternalPropertyChanged;
            //_presenter.Model.OptionsModel.PropertyChanged += OptionsPropertyChanged;
            _presenter.Model.ReportingModel.PropertyChanged += ReportingPropertyChanged;
            _presenter.Model.WebSettingsModel.PropertyChanged += WebSettingsChanged;
            _presenter.Model.WebVisualStylesModel.PropertyChanged += WebVisualStylesPropertyChanged;
        }

        // *** Always add bindings for CheckBox controls that control TextBox.Enabled after binding TextBox.Text ***

        private const string EnabledPropertyName = "Enabled";
        private const string ErrorToolTipTextPropertyName = "ErrorToolTipText";
        private const string ValuePropertyName = "Value";
        private const string SelectedValuePropertyName = "SelectedValue";
        private const string TextPropertyName = "Text";

        private void LoadScheduledTasksTab()
        {
            // Refresh Data
            txtCollectMinutes.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.SyncOnSchedule));
            txtCollectMinutes.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.SyncTimeMinutes));
            chkScheduled.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.SyncOnSchedule));

            chkSynchronous.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.SyncOnLoad));

            // Web Generation
            radioSchedule.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            lbl2MinutesToGen.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));

            txtWebGenMinutes.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateInterval));
            txtWebGenMinutes.DataBindings.Add(EnabledPropertyName, _presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateIntervalEnabled), false, DataSourceUpdateMode.OnValidation);

            radioFullRefresh.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebGenAfterRefresh));
            radioFullRefresh.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));

            WebGenTypePanel.DataSource = _presenter.Model.ScheduledTasksModel;
            WebGenTypePanel.ValueMember = nameof(ScheduledTasksModel.WebGenType);
            WebGenTypePanel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));

            WebSiteTargetPathTextBox.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebRoot));
            WebSiteTargetPathTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            WebSiteTargetPathLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));

            WebSiteServerTextBox.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebGenServer));
            WebSiteServerTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));
            WebSiteServerLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));

            WebSitePortTextBox.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebGenPort));
            WebSitePortTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));
            WebSitePortLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));

            WebSiteUsernameTextBox.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebGenUsername));
            WebSiteUsernameTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));
            WebSiteUsernameTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            WebSiteUsernameLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));

            WebSitePasswordTextBox.BindText(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.WebGenPassword));
            WebSitePasswordTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));
            WebSitePasswordTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            WebSitePasswordLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));

            chkHtml.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.CopyHtml));
            chkHtml.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            chkXml.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.CopyXml));
            chkXml.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            chkFAHlog.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.CopyFAHlog));
            chkFAHlog.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            FtpModePanel.DataSource = _presenter.Model.ScheduledTasksModel;
            FtpModePanel.ValueMember = nameof(ScheduledTasksModel.FtpMode);
            FtpModePanel.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.FtpModeEnabled));
            chkLimitSize.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.LimitLogSize));
            chkLimitSize.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.LimitLogSizeEnabled));
            udLimitSize.DataBindings.Add(ValuePropertyName, _presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.LimitLogSizeLength), false, DataSourceUpdateMode.OnPropertyChanged);
            udLimitSize.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.LimitLogSizeLengthEnabled));

            TestConnectionButton.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
            BrowseWebFolderButton.BindEnabled(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.BrowseLocalPathEnabled));
            chkWebSiteGenerator.BindChecked(_presenter.Model.ScheduledTasksModel, nameof(ScheduledTasksModel.GenerateWeb));
        }

        private void LoadStartupTab()
        {
            // Startup
            if (!Core.Application.IsRunningOnMono)
            {
                chkAutoRun.BindChecked(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.AutoRun));
            }
            else
            {
                // No AutoRun under Mono
                chkAutoRun.Enabled = false;
            }

            chkRunMinimized.BindChecked(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.RunMinimized));
            chkCheckForUpdate.BindChecked(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.StartupCheckForUpdate));

            // Configuration File
            txtDefaultConfigFile.BindEnabled(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.UseDefaultConfigFile));
            btnBrowseConfigFile.BindEnabled(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.UseDefaultConfigFile));
            txtDefaultConfigFile.BindText(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.DefaultConfigFile));

            chkDefaultConfig.BindChecked(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.UseDefaultConfigFile));

            // External Programs
            txtLogFileViewer.BindText(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.LogFileViewer));
            txtFileExplorer.BindText(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.FileExplorer));
        }

        private void LoadOptionsTab()
        {
            // Interactive Options
            chkOffline.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.OfflineLast));
            chkColorLog.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.ColorLogFile));
            chkAutoSave.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.AutoSaveConfig));
            DuplicateProjectCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.DuplicateProjectCheck));
            ShowUserStatsCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.ShowXmlStats));

            PpdCalculationComboBox.DataSource = OptionsModel.PpdCalculationList;
            PpdCalculationComboBox.DisplayMember = "DisplayMember";
            PpdCalculationComboBox.ValueMember = "ValueMember";
            PpdCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.PpdCalculation), false, DataSourceUpdateMode.OnPropertyChanged);
            BonusCalculationComboBox.DataSource = OptionsModel.BonusCalculationList;
            BonusCalculationComboBox.DisplayMember = "DisplayMember";
            BonusCalculationComboBox.ValueMember = "ValueMember";
            BonusCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.CalculateBonus), false, DataSourceUpdateMode.OnPropertyChanged);
            udDecimalPlaces.DataBindings.Add(ValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.DecimalPlaces), false, DataSourceUpdateMode.OnPropertyChanged);
            chkEtaAsDate.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.EtaDate));

            // Debug Message Level
            cboMessageLevel.DataSource = OptionsModel.DebugList;
            cboMessageLevel.DisplayMember = "DisplayMember";
            cboMessageLevel.ValueMember = "ValueMember";
            cboMessageLevel.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.MessageLevel), false, DataSourceUpdateMode.OnPropertyChanged);

            // Form Docking Style
            cboShowStyle.DataSource = OptionsModel.DockingStyleList;
            cboShowStyle.DisplayMember = "DisplayMember";
            cboShowStyle.ValueMember = "ValueMember";
            cboShowStyle.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.FormShowStyle), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void LoadReportingTab()
        {
            // Email Settings
            chkEmailSecure.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerSecure));
            chkEmailSecure.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            btnTestEmail.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtToEmailAddress.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ToAddress));
            txtToEmailAddress.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtFromEmailAddress.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.FromAddress));
            txtFromEmailAddress.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtSmtpServer.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerAddress));
            txtSmtpServer.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerAddressPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpServer.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtSmtpServerPort.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerPort));
            txtSmtpServerPort.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerAddressPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpServerPort.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtSmtpUsername.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerUsername));
            txtSmtpUsername.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerUsernamePasswordError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpUsername.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            txtSmtpPassword.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerPassword));
            txtSmtpPassword.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerUsernamePasswordError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpPassword.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            chkEnableEmail.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            // Report Selections
            grpReportSelections.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));
            //chkClientEuePause.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportEuePause));
            //chkClientHung.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportHung));
        }

        private void LoadWebSettingsTab()
        {
            // Web Statistics
            txtEOCUserID.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.EocUserId));
            txtStanfordUserID.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.StanfordId));
            txtStanfordTeamID.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.TeamId));

            // Project Download URL
            txtProjectDownloadUrl.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProjectDownloadUrl));

            // Web Proxy Settings
            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox
            txtProxyServer.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyServer));
            txtProxyServer.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyServer.BindEnabled(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.UseProxy));

            txtProxyPort.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyPort));
            txtProxyPort.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPort.BindEnabled(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.UseProxy));

            // Finally, add the CheckBox.Checked Binding
            chkUseProxy.BindChecked(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.UseProxy));
            chkUseProxyAuth.BindEnabled(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.UseProxy));

            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox
            txtProxyUser.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyUser));
            txtProxyUser.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyUserPassError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyUser.BindEnabled(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyAuthEnabled));

            txtProxyPass.BindText(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyPass));
            txtProxyPass.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyUserPassError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPass.BindEnabled(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.ProxyAuthEnabled));

            // Finally, add the CheckBox.Checked Binding
            chkUseProxyAuth.BindChecked(_presenter.Model.WebSettingsModel, nameof(WebSettingsModel.UseProxyAuth));
        }

        private void LoadVisualStylesTab()
        {
            if (Core.Application.IsRunningOnMono)
            {
                StyleList.Sorted = false;
            }
            StyleList.DataSource = _presenter.Model.WebVisualStylesModel.CssFileList;
            StyleList.DisplayMember = "DisplayMember";
            StyleList.ValueMember = "ValueMember";
            StyleList.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.CssFile), false, DataSourceUpdateMode.OnPropertyChanged);

            txtOverview.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.WebOverview), false, DataSourceUpdateMode.OnPropertyChanged);
            txtSummary.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.WebSummary), false, DataSourceUpdateMode.OnPropertyChanged);
            txtInstance.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.WebSlot), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTipPrefs.RemoveAll();

            if (tabControl1.SelectedIndex == (int)TabName.WebVisualStyles)
            {
                ShowCssPreview();
            }
        }

        #endregion

        // Scheduled Tasks Tab
        private void BrowseWebFolderButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFolderDialogPresenter.BrowseFolder())
            {
                _presenter.BrowseWebFolderClicked(dialog);
            }
        }

        // Reporting Tab
        private void txtFromEmailAddress_MouseHover(object sender, EventArgs e)
        {
            var fromAddressError = _presenter.Model.ReportingModel[nameof(ReportingModel.FromAddress)];
            if (!String.IsNullOrEmpty(fromAddressError))
            {
                return;
            }

            toolTipPrefs.RemoveAll();
            toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine),
               txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 55, 10000);
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            _presenter.TestEmailClicked(SendMailService.Default);
        }

        private void grpReportSelections_EnabledChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in grpReportSelections.Controls)
            {
                if (ctrl is CheckBox)
                {
                    ctrl.Enabled = grpReportSelections.Enabled;
                }
            }
        }

        // Web Tab
        private void linkEOC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestExtremeOverclockingUserClicked(LocalProcessService.Default);
        }

        private void linkStanford_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestFoldingAtHomeUserClicked(LocalProcessService.Default);
        }

        private void linkTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestExtremeOverclockingTeamClicked(LocalProcessService.Default);
        }

        // Visual Style Tab
        private void StyleList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCssPreview();
        }

        private void ShowCssPreview()
        {
            if (Core.Application.IsRunningOnMono) return;

            string sStylesheet = Path.Combine(_presenter.Model.Preferences.Get<string>(Preference.ApplicationPath), Core.Application.CssFolderName, _presenter.Model.WebVisualStylesModel.CssFile);
            var sb = new StringBuilder();

            sb.Append("<HTML><HEAD><TITLE>Test CSS File</TITLE>");
            sb.Append("<LINK REL=\"Stylesheet\" TYPE=\"text/css\" href=\"file://" + sStylesheet + "\" />");
            sb.Append("</HEAD><BODY>");

            sb.Append("<table class=\"Instance\">");
            sb.Append("<tr>");
            sb.Append("<td class=\"Heading\">Heading</td>");
            sb.Append("<td class=\"Blank\" width=\"100%\"></td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"LeftCol\">Left Col</td>");
            sb.Append("<td class=\"RightCol\">Right Column</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"AltLeftCol\">Left Col</td>");
            sb.Append("<td class=\"AltRightCol\">Right Column</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"LeftCol\">Left Col</td>");
            sb.Append("<td class=\"RightCol\">Right Column</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"AltLeftCol\">Left Col</td>");
            sb.Append("<td class=\"AltRightCol\">Right Column</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append($"<td class=\"Plain\" colspan=\"2\" align=\"center\">Last updated {DateTime.Now.ToLongDateString()} at {DateTime.Now.ToLongTimeString()}</td>");
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</BODY></HTML>");

            _cssSampleBrowser.DocumentText = sb.ToString();
        }

        // Button Click Event Handlers
        private async void TestConnectionButtonClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                await _presenter.TestWebGenerationConnection(FtpService.Default).ConfigureAwait(true);
                _presenter.ShowTestWebGenerationConnectionSucceededMessage();
            }
            catch (Exception ex)
            {
                _presenter.ShowTestWebGenerationConnectionFailedMessage(ex);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            SetSelectedTabWithError();
            _presenter.OKClicked();
        }

        private void SetSelectedTabWithError()
        {
            if (_presenter.Model.ScheduledTasksModel.HasError)
            {
                tabControl1.SelectedTab = tabSchdTasks;
            }
            else if (_presenter.Model.StartupAndExternalModel.HasError)
            {
                tabControl1.SelectedTab = tabStartup;
            }
            else if (_presenter.Model.OptionsModel.HasError)
            {
                tabControl1.SelectedTab = tabOptions;
            }
            else if (_presenter.Model.ReportingModel.HasError)
            {
                tabControl1.SelectedTab = tabReporting;
            }
            else if (_presenter.Model.WebSettingsModel.HasError)
            {
                tabControl1.SelectedTab = tabWeb;
            }
            else if (_presenter.Model.WebVisualStylesModel.HasError)
            {
                tabControl1.SelectedTab = tabVisStyles;
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        // Folder Browsing
        private void btnBrowseConfigFile_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForConfigurationFileClicked(dialog);
            }
        }

        private void btnBrowseLogViewer_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForLogViewerClicked(dialog);
            }
        }

        private void btnBrowseFileExplorer_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForFileExplorerClicked(dialog);
            }
        }

        private void btnOverviewBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForOverviewTransform(dialog);
            }
        }

        private void btnSummaryBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForSummaryTransform(dialog);
            }
        }

        private void btnInstanceBrowse_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForSlotTransform(dialog);
            }
        }

        #region TextBox KeyPress Event Handler (to enforce digits only)

        private void TextBoxDigitsOnlyKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine($"Keystroke: {(int)e.KeyChar}");

            // only allow digits special keystrokes - Issue 65
            if (Char.IsDigit(e.KeyChar) == false &&
                  e.KeyChar != 8 &&       // backspace
                  e.KeyChar != 26 &&      // Ctrl+Z
                  e.KeyChar != 24 &&      // Ctrl+X
                  e.KeyChar != 3 &&       // Ctrl+C
                  e.KeyChar != 22 &&      // Ctrl+V
                  e.KeyChar != 25)        // Ctrl+Y
            {
                e.Handled = true;
            }
        }

        #endregion
    }
}