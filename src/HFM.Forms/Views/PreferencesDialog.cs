using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

using HFM.Core.Net;
using HFM.Core.Services;
using HFM.Core.SlotXml;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Preferences;

namespace HFM.Forms.Views
{
    public partial class PreferencesDialog : FormBase, IWin32Dialog
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

        private readonly PreferencesPresenter _presenter;
        private readonly WebBrowser _cssSampleBrowser;

        private const int MaxDecimalPlaces = 5;

        public PreferencesDialog(PreferencesPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            EscapeKeyButton(cancelButton);

            webDeploymentTypePathRadioButton.Tag = (int)WebDeploymentType.Path;
            webDeploymentTypeFtpRadioButton.Tag = (int)WebDeploymentType.Ftp;
            webDeploymentFtpPassiveRadioButton.Tag = (int)FtpMode.Passive;
            webDeploymentFtpActiveRadioButton.Tag = (int)FtpMode.Active;

            clientsDecimalPlacesUpDown.Minimum = 0;
            clientsDecimalPlacesUpDown.Maximum = MaxDecimalPlaces;

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

        private void PreferencesDialog_Load(object sender, EventArgs e)
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
            optionsAutoRunCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.AutoRun));

            optionsRunMinimizedCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.RunMinimized));
            optionsStartupCheckForUpdateCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.StartupCheckForUpdate));

            // Identity
            optionsEocUserIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.EocUserID));
            optionsFahUserIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.FahUserID));
            optionsFahTeamIDTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.TeamID));
            optionsEocUserStatsEnabledCheckBox.BindChecked(_presenter.Model.OptionsModel, nameof(OptionsModel.EocUserStatsEnabled));

            // External Programs
            optionsLogFileViewerTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.LogFileViewer));
            optionsFileExplorerTextBox.BindText(_presenter.Model.OptionsModel, nameof(OptionsModel.FileExplorer));

            // Debug Message Level
            optionsMessageLevelComboBox.DataSource = OptionsModel.DebugList;
            optionsMessageLevelComboBox.DisplayMember = "DisplayMember";
            optionsMessageLevelComboBox.ValueMember = "ValueMember";
            optionsMessageLevelComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.MessageLevel), false, DataSourceUpdateMode.OnPropertyChanged);

            // Form Docking Style
            optionsMinimizeToOptionComboBox.DataSource = OptionsModel.DockingStyleList;
            optionsMinimizeToOptionComboBox.DisplayMember = "DisplayMember";
            optionsMinimizeToOptionComboBox.ValueMember = "ValueMember";
            optionsMinimizeToOptionComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.OptionsModel, nameof(OptionsModel.MinimizeToOption), false, DataSourceUpdateMode.OnPropertyChanged);
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

            clientsPPDCalculationComboBox.DataSource = ClientsModel.PPDCalculationList;
            clientsPPDCalculationComboBox.DisplayMember = nameof(ListItem.DisplayMember);
            clientsPPDCalculationComboBox.ValueMember = nameof(ListItem.ValueMember);
            clientsPPDCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.PPDCalculation), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsBonusCalculationComboBox.DataSource = ClientsModel.BonusCalculationList;
            clientsBonusCalculationComboBox.DisplayMember = nameof(ListItem.DisplayMember);
            clientsBonusCalculationComboBox.ValueMember = nameof(ListItem.ValueMember);
            clientsBonusCalculationComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.BonusCalculation), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsDecimalPlacesUpDown.DataBindings.Add(ValuePropertyName, _presenter.Model.ClientsModel, nameof(ClientsModel.DecimalPlaces), false, DataSourceUpdateMode.OnPropertyChanged);
            clientsDisplayETADateCheckBox.BindChecked(_presenter.Model.ClientsModel, nameof(ClientsModel.DisplayETADate));
        }

        private void LoadReportingTab()
        {
            // Email Settings
            reportingIsSecureCheckBox.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.IsSecure));
            reportingIsSecureCheckBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingSendTestEmailLinkLabel.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingToAddressTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.ToAddress));
            reportingToAddressTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingFromAddressTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.FromAddress));
            reportingFromAddressTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingServerTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.Server));
            reportingServerTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            reportingServerTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingPortTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.Port));
            reportingPortTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.ServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            reportingPortTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingUsernameTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.Username));
            reportingUsernameTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            reportingUsernameTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingPasswordTextBox.BindText(_presenter.Model.ReportingModel, nameof(ReportingModel.Password));
            reportingPasswordTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.ReportingModel, nameof(ReportingModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            reportingPasswordTextBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            reportingEnabledCheckBox.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));

            // Report Selections
            reportingSelectionsGroupBox.BindEnabled(_presenter.Model.ReportingModel, nameof(ReportingModel.Enabled));
            //chkClientEuePause.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportEuePause));
            //chkClientHung.BindChecked(_presenter.Model.ReportingModel, nameof(ReportingModel.ReportHung));
        }

        private void LoadWebProxyTab()
        {
            // Web Proxy Settings
            webProxyServerTextBox.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Server));
            webProxyServerTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            webProxyServerTextBox.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Enabled));

            webProxyPortTextBox.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Port));
            webProxyPortTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.ServerPortError), false, DataSourceUpdateMode.OnPropertyChanged);
            webProxyPortTextBox.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Enabled));

            // Finally, add the CheckBox.Checked Binding
            webProxyEnabledCheckBox.BindChecked(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Enabled));
            webProxyCredentialsEnabledCheckBox.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Enabled));

            webProxyUsernameTextBox.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Username));
            webProxyUsernameTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            webProxyUsernameTextBox.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.AuthenticationEnabled));

            webProxyPasswordTextBox.BindText(_presenter.Model.WebProxyModel, nameof(WebProxyModel.Password));
            webProxyPasswordTextBox.DataBindings.Add(ErrorToolTipTextPropertyName, _presenter.Model.WebProxyModel, nameof(WebProxyModel.CredentialsError), false, DataSourceUpdateMode.OnPropertyChanged);
            webProxyPasswordTextBox.BindEnabled(_presenter.Model.WebProxyModel, nameof(WebProxyModel.AuthenticationEnabled));

            // Finally, add the CheckBox.Checked Binding
            webProxyCredentialsEnabledCheckBox.BindChecked(_presenter.Model.WebProxyModel, nameof(WebProxyModel.CredentialsEnabled));
        }

        private void LoadVisualStylesTab()
        {
            webVisualStylesCssFileListBox.DataSource = _presenter.Model.WebVisualStylesModel.CssFileList;
            webVisualStylesCssFileListBox.DisplayMember = nameof(ListItem.DisplayMember);
            webVisualStylesCssFileListBox.ValueMember = nameof(ListItem.ValueMember);
            webVisualStylesCssFileListBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.CssFile), false, DataSourceUpdateMode.OnPropertyChanged);

            webVisualStylesOverviewTextBox.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.OverviewXsltPath), false, DataSourceUpdateMode.OnPropertyChanged);
            webVisualStylesSummaryTextBox.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.SummaryXsltPath), false, DataSourceUpdateMode.OnPropertyChanged);
            webVisualStylesSlotTextBox.DataBindings.Add(TextPropertyName, _presenter.Model.WebVisualStylesModel, nameof(WebVisualStylesModel.SlotXsltPath), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            toolTipPrefs.RemoveAll();

            if (tabControl1.SelectedIndex == (int)TabName.WebVisualStyles)
            {
                ShowCssPreview();
            }
        }

        // Scheduled Tasks Tab
        private void webGenerationBrowsePathButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFolderDialogPresenter.BrowseFolder())
            {
                _presenter.BrowseForWebGenerationPath(dialog);
            }
        }

        // Reporting Tab
        private void reportingFromAddressTextBox_MouseHover(object sender, EventArgs e)
        {
            var fromAddressError = _presenter.Model.ReportingModel[nameof(ReportingModel.FromAddress)];
            if (!String.IsNullOrEmpty(fromAddressError))
            {
                return;
            }

            toolTipPrefs.RemoveAll();
            toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine),
               reportingFromAddressTextBox.Parent, reportingFromAddressTextBox.Location.X + 5, reportingFromAddressTextBox.Location.Y - 55, 10000);
        }

        private void reportingSendTestEmailLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestReportingEmail(SendMailService.Default);
        }

        private void reportingSelectionsGroupBox_EnabledChanged(object sender, EventArgs e)
        {
            foreach (Control ctrl in reportingSelectionsGroupBox.Controls)
            {
                if (ctrl is CheckBox)
                {
                    ctrl.Enabled = reportingSelectionsGroupBox.Enabled;
                }
            }
        }

        // Identity
        private void optionsTestEocUserIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestExtremeOverclockingUser(LocalProcessService.Default);
        }

        private void optionsTestFahUserIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestFoldingAtHomeUser(LocalProcessService.Default);
        }

        private void optionsTestFahTeamIDLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.TestExtremeOverclockingTeam(LocalProcessService.Default);
        }

        // Visual Style Tab
        private void webVisualStylesCssFileListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            ShowCssPreview();
        }

        private void ShowCssPreview()
        {
            string sStylesheet = Path.Combine(_presenter.Model.Preferences.Get<string>(Preference.ApplicationPath), Core.Application.CssFolderName, _presenter.Model.WebVisualStylesModel.CssFile);
            var sb = new StringBuilder();

            sb.Append("<HTML>");

            sb.Append("<HEAD>");
            sb.Append("<TITLE>Test CSS File</TITLE>");
            sb.Append("<LINK REL=\"stylesheet\" href=\"file://" + sStylesheet + "\" />");
            sb.Append("<style> body { font-family: \"Segoe UI\",Roboto,\"Helvetica Neue\",Arial }</style>");
            sb.Append("</HEAD>");

            sb.Append("<BODY>");
            sb.Append("<table>");
            sb.Append("<thead>");
            sb.Append("<tr>");
            sb.Append("<td class=\"table-heading\">Heading</td>");
            sb.Append("</tr>");
            sb.Append("</thead>");
            sb.Append("<tbody>");
            sb.Append("<tr>");
            sb.Append("<td class=\"table-column\">Column</td>");
            sb.Append("</tr>");
            sb.Append("<tr>");
            sb.Append("<td class=\"table-column-alt\">Column (Alt)</td>");
            sb.Append("</tr>");
            sb.Append("</tbody>");
            sb.Append("</table>");
            sb.Append("</BODY>");

            sb.Append("</HTML>");

            _cssSampleBrowser.DocumentText = sb.ToString();
        }

        // Button Click Event Handlers
        private async void webGenerationTestConnectionLinkLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
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

        private void okButton_Click(object sender, EventArgs e)
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
                tabControl1.SelectedTab = optionsTab;
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
                tabControl1.SelectedTab = reportingTab;
            }
            else if (_presenter.Model.WebProxyModel.HasError)
            {
                tabControl1.SelectedTab = webProxyTab;
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _presenter.CancelClicked();
        }

        // Folder Browsing
        private void clientsBrowseConfigFileButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForConfigurationFile(dialog);
            }
        }

        private void optionsBrowseLogFileViewerButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForLogFileViewer(dialog);
            }
        }

        private void optionsBrowseFileExplorerButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForFileExplorer(dialog);
            }
        }

        private void webVisualStylesOverviewBrowseXsltButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForOverviewTransform(dialog);
            }
        }

        private void webVisualStylesSummaryBrowseXsltButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForSummaryTransform(dialog);
            }
        }

        private void webVisualStylesSlotBrowseXsltButton_Click(object sender, EventArgs e)
        {
            using (var dialog = DefaultFileDialogPresenter.OpenFile())
            {
                _presenter.BrowseForSlotTransform(dialog);
            }
        }

        // TextBox KeyPress Event Handler (to enforce digits only)
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

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                components?.Dispose();
                _cssSampleBrowser?.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
