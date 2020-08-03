
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
            //Clients = 0,
            //Options = 1,
            //WebGeneration = 2,
            WebVisualStyles = 3
            //Reporting = 4,
            //WebProxy = 5,
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

            clientsDecimalPlacesUpDown.Minimum = 0;
            clientsDecimalPlacesUpDown.Maximum = MaxDecimalPlaces;

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
            LoadClientsTab();
            LoadOptionsTab();
            LoadWebGenerationTab();
            LoadVisualStylesTab();
            LoadReportingTab();
            LoadWebProxyTab();

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

            _presenter.Model.ClientsModel.PropertyChanged += ClientsPropertyChanged;
            _presenter.Model.OptionsModel.PropertyChanged += OptionsPropertyChanged;
            _presenter.Model.WebGenerationModel.PropertyChanged += WebGenerationPropertyChanged;
            _presenter.Model.WebVisualStylesModel.PropertyChanged += WebVisualStylesPropertyChanged;
            _presenter.Model.ReportingModel.PropertyChanged += ReportingPropertyChanged;
            _presenter.Model.WebProxyModel.PropertyChanged += WebProxyPropertyChanged;
        }

        // *** Always add bindings for CheckBox controls that control TextBox.Enabled after binding TextBox.Text ***

        private const string EnabledPropertyName = "Enabled";
        private const string ErrorToolTipTextPropertyName = "ErrorToolTipText";
        private const string ValuePropertyName = "Value";
        private const string SelectedValuePropertyName = "SelectedValue";
        private const string TextPropertyName = "Text";

        private void LoadWebGenerationTab()
        {
            // Web Generation
            webGenerationOnScheduleRadioButton.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationIntervalLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));

            webGenerationIntervalTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Interval));
            webGenerationIntervalTextBox.DataBindings.Add(EnabledPropertyName, _presenter.Model.WebGenerationModel, nameof(WebGenerationModel.IntervalEnabled), false, DataSourceUpdateMode.OnValidation);

            webGenerationAfterClientRetrievalRadioButton.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.AfterClientRetrieval));
            webGenerationAfterClientRetrievalRadioButton.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));

            webDeploymentTypeRadioPanel.DataSource = _presenter.Model.WebGenerationModel;
            webDeploymentTypeRadioPanel.ValueMember = nameof(WebGenerationModel.WebDeploymentType);
            webDeploymentTypeRadioPanel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));

            webGenerationPathTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Path));
            webGenerationPathTextBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationPathLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));

            webGenerationServerTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Server));
            webGenerationServerTextBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));
            webGenerationServerLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));

            webGenerationPortTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Port));
            webGenerationPortTextBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));
            webGenerationPortLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));

            webGenerationUsernameTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Username));
            webGenerationUsernameTextBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));
            webGenerationUsernameTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebGenerationModel, nameof(WebGenerationModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            webGenerationUsernameLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));

            webGenerationPasswordTextBox.BindText(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Password));
            webGenerationPasswordTextBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));
            webGenerationPasswordTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebGenerationModel, nameof(WebGenerationModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            webGenerationPasswordLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));

            webGenerationCopyHtmlCheckBox.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.CopyHtml));
            webGenerationCopyHtmlCheckBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationCopyXmlCheckBox.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.CopyXml));
            webGenerationCopyXmlCheckBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationCopyLogCheckBox.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.CopyLog));
            webGenerationCopyLogCheckBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationFtpModeRadioPanel.DataSource = _presenter.Model.WebGenerationModel;
            webGenerationFtpModeRadioPanel.ValueMember = nameof(WebGenerationModel.FtpMode);
            webGenerationFtpModeRadioPanel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.FtpModeEnabled));
            webGenerationLimitLogSizeCheckBox.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.LimitLogSize));
            webGenerationLimitLogSizeCheckBox.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.LimitLogSizeEnabled));
            webGenerationLimitLogSizeLengthUpDown.DataBindings.Add(ValuePropertyName, _presenter.Model.WebGenerationModel, nameof(WebGenerationModel.LimitLogSizeLength), false, DataSourceUpdateMode.OnPropertyChanged);
            webGenerationLimitLogSizeLengthUpDown.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.LimitLogSizeLengthEnabled));

            webGenerationTestConnectionLinkLabel.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
            webGenerationBrowsePathButton.BindEnabled(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.BrowsePathEnabled));
            webGenerationEnabledCheckBox.BindChecked(_presenter.Model.WebGenerationModel, nameof(WebGenerationModel.Enabled));
        }

        private void LoadOptionsTab()
        {
            // Startup
            if (!Core.Application.IsRunningOnMono)
            {
                chkAutoRun.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.AutoRun));
            }
            else
            {
                // No AutoRun under Mono
                chkAutoRun.Enabled = false;
            }

            chkRunMinimized.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.RunMinimized));
            chkCheckForUpdate.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.StartupCheckForUpdate));

            // Identity
            EocUserIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.EocUserID));
            FahUserIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.FahUserID));
            FahTeamIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.TeamID));
            EocUserStatsCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.ShowXmlStats));

            // External Programs
            LogFileViewerTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.LogFileViewer));
            FileExplorerTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.FileExplorer));

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

        private void LoadClientsTab()
        {
            // Configuration
            clientsDefaultConfigFileTextBox.BindEnabled(_presenter.Model.ClientsModel, nameof(ClientsModel.DefaultConfigFileEnabled));
            clientsBrowseConfigFileButton.BindEnabled(_presenter.Model.ClientsModel, nameof(ClientsModel.DefaultConfigFileEnabled));
            clientsDefaultConfigFileTextBox.BindText(_presenter.Model.ClientsModel, nameof(ClientsModel.DefaultConfigFile));

            clientsDefaultConfigFileEnabledCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.DefaultConfigFileEnabled));
            clientsAutoSaveConfig.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.AutoSaveConfig));

            // Refresh Client Data
            clientsRetrievalIntervalTextBox.BindEnabled(_presenter.Model.ClientsModel, nameof(ClientsModel.RetrievalEnabled));
            clientsRetrievalIntervalTextBox.BindText(_presenter.Model.ClientsModel, nameof(ClientsModel.RetrievalInterval));
            clientsRetrievalEnabledCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.RetrievalEnabled));

            clientsRetrievalIsSerialCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.RetrievalIsSerial));

            // Display / Production Options
            clientsOfflineLastCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.OfflineLast));
            clientsColorLogFileCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.ColorLogFile));
            clientsDuplicateProjectCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.DuplicateProjectCheck));

            clientsPPDCalculationComboBox.DataSource = ClientsModel.PpdCalculationList;
            clientsPPDCalculationComboBox.DisplayMember = "DisplayMember";
            clientsPPDCalculationComboBox.ValueMember = "ValueMember";
            clientsPPDCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.PPDCalculation), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsBonusCalculationComboBox.DataSource = ClientsModel.BonusCalculationList;
            clientsBonusCalculationComboBox.DisplayMember = "DisplayMember";
            clientsBonusCalculationComboBox.ValueMember = "ValueMember";
            clientsBonusCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.BonusCalculation), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsDecimalPlacesUpDown.DataBindings.Add(ValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.DecimalPlaces), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsDisplayETADateCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.DisplayETADate));
        }

        private void LoadReportingTab()
        {
            // Email Settings
            chkEmailSecure.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ServerSecure));
            chkEmailSecure.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

            SendTestEmailLinkLabel.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportingEnabled));

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

        private void LoadWebProxyTab()
        {
            // Web Proxy Settings
            txtProxyServer.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyServer));
            txtProxyServer.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyServer.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.UseProxy));

            txtProxyPort.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyPort));
            txtProxyPort.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPort.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.UseProxy));

            // Finally, add the CheckBox.Checked Binding
            chkUseProxy.BindChecked(_presenter.Model.WebProxyModel, nameof(WebProxyModel.UseProxy));
            chkUseProxyAuth.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.UseProxy));

            txtProxyUser.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyUser));
            txtProxyUser.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyUserPassError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyUser.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyAuthEnabled));

            txtProxyPass.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyPass));
            txtProxyPass.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyUserPassError), false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPass.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.ProxyAuthEnabled));

            // Finally, add the CheckBox.Checked Binding
            chkUseProxyAuth.BindChecked(_presenter.Model.WebProxyModel, nameof(WebProxyModel.UseProxyAuth));
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
        private void webGenerationBrowsePathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFolderDialogPresenter.BrowseFolder())
            {
                _presenter.WebGenerationBrowsePathClicked(dialog);
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

        private void SendTestEmailLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        // Identity
        private void TestEocUserIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestExtremeOverclockingUserClicked(LocalProcessService.Default);
        }

        private void TestFahUserIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestFoldingAtHomeUserClicked(LocalProcessService.Default);
        }

        private void TestFahTeamIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
        private async void TestConnectionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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
            if (_presenter.Model.ClientsModel.HasError)
            {
                tabControl1.SelectedTab = clientsTab;
            }
            else if (_presenter.Model.OptionsModel.HasError)
            {
                tabControl1.SelectedTab = OptionsTab;
            }
            else if (_presenter.Model.WebGenerationModel.HasError)
            {
                tabControl1.SelectedTab = webGenerationTab;
            }
            else if (_presenter.Model.WebVisualStylesModel.HasError)
            {
                tabControl1.SelectedTab = webVisualStylesTab;
            }
            else if (_presenter.Model.ReportingModel.HasError)
            {
                tabControl1.SelectedTab = ReportingTab;
            }
            else if (_presenter.Model.WebProxyModel.HasError)
            {
                tabControl1.SelectedTab = ProxyTab;
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
