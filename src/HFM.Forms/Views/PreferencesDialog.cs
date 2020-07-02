
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using HFM.Core;
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

        private const string XsltExt = "xslt";
        private const string XsltFilter = "XML Transform (*.xslt;*.xsl)|*.xslt;*.xsl";
        private const string HfmExt = "hfmx";
        private const string HfmFilter = "HFM Configuration Files|*.hfmx";
        private const string ExeExt = "exe";
        private const string ExeFilter = "Program Files|*.exe";

        private readonly PreferencesPresenter _presenter;
        private readonly IFtpService _ftpService;

        private readonly WebBrowser _cssSampleBrowser;

        #endregion

        private const int MaxDecimalPlaces = 5;

        #region Constructor And Binding/Load Methods

        public PreferencesDialog(PreferencesPresenter presenter)
        {
            _presenter = presenter;
            _ftpService = new FtpService();

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

        private void ScheduledTasksPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleScheduledTasksPropertyEnabledForMono(e.PropertyName);
                HandleScheduledTasksPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleScheduledTasksPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "SyncOnSchedule":
                    txtCollectMinutes.Enabled = _presenter.Model.ScheduledTasksModel.SyncOnSchedule;
                    break;
                case "GenerateWeb":
                    radioSchedule.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    lbl2MinutesToGen.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    radioFullRefresh.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    WebSiteTargetPathTextBox.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    chkHtml.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    chkXml.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    chkFAHlog.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    TestConnectionButton.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    WebGenTypePanel.Enabled = _presenter.Model.ScheduledTasksModel.GenerateWeb;
                    break;
                case "GenerateIntervalEnabled":
                    txtWebGenMinutes.Enabled = _presenter.Model.ScheduledTasksModel.GenerateIntervalEnabled;
                    break;
                case "FtpModeEnabled":
                    WebSiteServerTextBox.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSiteServerLabel.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSitePortTextBox.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSitePortLabel.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSiteUsernameTextBox.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSiteUsernameLabel.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSitePasswordTextBox.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    WebSitePasswordLabel.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    FtpModePanel.Enabled = _presenter.Model.ScheduledTasksModel.FtpModeEnabled;
                    break;
                case "BrowseLocalPathEnabled":
                    btnBrowseWebFolder.Enabled = _presenter.Model.ScheduledTasksModel.BrowseLocalPathEnabled;
                    break;
                case "LimitLogSizeEnabled":
                    chkLimitSize.Enabled = _presenter.Model.ScheduledTasksModel.LimitLogSizeEnabled;
                    break;
                case "LimitLogSizeLengthEnabled":
                    udLimitSize.Enabled = _presenter.Model.ScheduledTasksModel.LimitLogSizeLengthEnabled;
                    break;
            }
        }

        private void HandleScheduledTasksPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "WebRoot":
                    WebSiteTargetPathTextBox.Text = _presenter.Model.ScheduledTasksModel.WebRoot;
                    break;
            }
        }

        private void StartupAndExternalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleStartupAndExternalPropertyEnabledForMono(e.PropertyName);
                HandleStartupAndExternalPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleStartupAndExternalPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "UseDefaultConfigFile":
                    txtDefaultConfigFile.Enabled = _presenter.Model.StartupAndExternalModel.UseDefaultConfigFile;
                    btnBrowseConfigFile.Enabled = _presenter.Model.StartupAndExternalModel.UseDefaultConfigFile;
                    break;
            }
        }

        private void HandleStartupAndExternalPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "DefaultConfigFile":
                    txtDefaultConfigFile.Text = _presenter.Model.StartupAndExternalModel.DefaultConfigFile;
                    break;
                case "LogFileViewer":
                    txtLogFileViewer.Text = _presenter.Model.StartupAndExternalModel.LogFileViewer;
                    break;
                case "FileExplorer":
                    txtFileExplorer.Text = _presenter.Model.StartupAndExternalModel.FileExplorer;
                    break;
            }
        }

        private void ReportingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleReportingPropertyEnabledForMono(e.PropertyName);
            }
        }

        private void HandleReportingPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "ReportingEnabled":
                    chkEmailSecure.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    btnTestEmail.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtToEmailAddress.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtFromEmailAddress.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtSmtpServer.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtSmtpServerPort.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtSmtpUsername.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    txtSmtpPassword.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    grpReportSelections.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    break;
            }
        }

        private void WebSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleWebSettingsPropertyEnabledForMono(e.PropertyName);
            }
        }

        private void HandleWebSettingsPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case "UseProxy":
                    txtProxyServer.Enabled = _presenter.Model.WebSettingsModel.UseProxy;
                    txtProxyPort.Enabled = _presenter.Model.WebSettingsModel.UseProxy;
                    chkUseProxyAuth.Enabled = _presenter.Model.WebSettingsModel.UseProxy;
                    break;
                case "ProxyAuthEnabled":
                    txtProxyUser.Enabled = _presenter.Model.WebSettingsModel.ProxyAuthEnabled;
                    txtProxyPass.Enabled = _presenter.Model.WebSettingsModel.ProxyAuthEnabled;
                    break;
            }
        }

        private void WebVisualStylesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleWebVisualStylesPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleWebVisualStylesPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WebVisualStylesModel.WebOverview):
                    txtOverview.Text = _presenter.Model.WebVisualStylesModel.WebOverview;
                    break;
                case nameof(WebVisualStylesModel.WebSummary):
                    txtSummary.Text = _presenter.Model.WebVisualStylesModel.WebSummary;
                    break;
                case nameof(WebVisualStylesModel.WebSlot):
                    txtInstance.Text = _presenter.Model.WebVisualStylesModel.WebSlot;
                    break;
            }
        }

        private void LoadScheduledTasksTab()
        {
            #region Refresh Data
            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox

            // Add the CheckBox.Checked => TextBox.Enabled Binding
            txtCollectMinutes.BindEnabled(_presenter.Model.ScheduledTasksModel, "SyncOnSchedule");
            // Bind the value to the TextBox
            txtCollectMinutes.BindText(_presenter.Model.ScheduledTasksModel, "SyncTimeMinutes");
            // Finally, add the CheckBox.Checked Binding
            chkScheduled.BindChecked(_presenter.Model.ScheduledTasksModel, "SyncOnSchedule");

            chkSynchronous.BindChecked(_presenter.Model.ScheduledTasksModel, "SyncOnLoad");
            #endregion

            #region Web Generation
            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox

            radioSchedule.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            lbl2MinutesToGen.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            // Bind the value to the TextBox
            txtWebGenMinutes.BindText(_presenter.Model.ScheduledTasksModel, "GenerateInterval");
            txtWebGenMinutes.DataBindings.Add("Enabled", _presenter.Model.ScheduledTasksModel, "GenerateIntervalEnabled", false, DataSourceUpdateMode.OnValidation);
            // Finally, add the RadioButton.Checked Binding
            radioFullRefresh.BindChecked(_presenter.Model.ScheduledTasksModel, "WebGenAfterRefresh");
            radioFullRefresh.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");

            WebGenTypePanel.DataSource = _presenter.Model.ScheduledTasksModel;
            WebGenTypePanel.ValueMember = "WebGenType";
            WebGenTypePanel.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");

            WebSiteTargetPathTextBox.BindText(_presenter.Model.ScheduledTasksModel, "WebRoot");
            WebSiteTargetPathTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            WebSiteTargetPathLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");

            WebSiteServerTextBox.BindText(_presenter.Model.ScheduledTasksModel, "WebGenServer");
            WebSiteServerTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");
            WebSiteServerLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");

            WebSitePortTextBox.BindText(_presenter.Model.ScheduledTasksModel, "WebGenPort");
            WebSitePortTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");
            WebSitePortLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");

            WebSiteUsernameTextBox.BindText(_presenter.Model.ScheduledTasksModel, "WebGenUsername");
            WebSiteUsernameTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");
            WebSiteUsernameTextBox.DataBindings.Add("ErrorToolTipText", _presenter.Model.ScheduledTasksModel, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            WebSiteUsernameLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");

            WebSitePasswordTextBox.BindText(_presenter.Model.ScheduledTasksModel, "WebGenPassword");
            WebSitePasswordTextBox.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");
            WebSitePasswordTextBox.DataBindings.Add("ErrorToolTipText", _presenter.Model.ScheduledTasksModel, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            WebSitePasswordLabel.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");

            chkHtml.BindChecked(_presenter.Model.ScheduledTasksModel, "CopyHtml");
            chkHtml.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            chkXml.BindChecked(_presenter.Model.ScheduledTasksModel, "CopyXml");
            chkXml.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            chkFAHlog.BindChecked(_presenter.Model.ScheduledTasksModel, "CopyFAHlog");
            chkFAHlog.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            FtpModePanel.DataSource = _presenter.Model.ScheduledTasksModel;
            FtpModePanel.ValueMember = "FtpMode";
            FtpModePanel.BindEnabled(_presenter.Model.ScheduledTasksModel, "FtpModeEnabled");
            chkLimitSize.BindChecked(_presenter.Model.ScheduledTasksModel, "LimitLogSize");
            chkLimitSize.BindEnabled(_presenter.Model.ScheduledTasksModel, "LimitLogSizeEnabled");
            udLimitSize.DataBindings.Add("Value", _presenter.Model.ScheduledTasksModel, "LimitLogSizeLength", false, DataSourceUpdateMode.OnPropertyChanged);
            udLimitSize.BindEnabled(_presenter.Model.ScheduledTasksModel, "LimitLogSizeLengthEnabled");

            // Finally, add the CheckBox.Checked Binding
            TestConnectionButton.BindEnabled(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            btnBrowseWebFolder.BindEnabled(_presenter.Model.ScheduledTasksModel, "BrowseLocalPathEnabled");
            chkWebSiteGenerator.BindChecked(_presenter.Model.ScheduledTasksModel, "GenerateWeb");
            #endregion
        }

        private void LoadStartupTab()
        {
            #region Startup
            /*** Auto-Run Is Not DataBound ***/
            if (!Core.Application.IsRunningOnMono)
            {
                chkAutoRun.BindChecked(_presenter.Model.StartupAndExternalModel, nameof(StartupAndExternalModel.AutoRun));
            }
            else
            {
                // No AutoRun under Mono
                chkAutoRun.Enabled = false;
            }

            chkRunMinimized.BindChecked(_presenter.Model.StartupAndExternalModel, "RunMinimized");
            chkCheckForUpdate.BindChecked(_presenter.Model.StartupAndExternalModel, "StartupCheckForUpdate");
            #endregion

            #region Configuration File
            txtDefaultConfigFile.BindEnabled(_presenter.Model.StartupAndExternalModel, "UseDefaultConfigFile");
            btnBrowseConfigFile.BindEnabled(_presenter.Model.StartupAndExternalModel, "UseDefaultConfigFile");
            txtDefaultConfigFile.BindText(_presenter.Model.StartupAndExternalModel, "DefaultConfigFile");

            chkDefaultConfig.BindChecked(_presenter.Model.StartupAndExternalModel, "UseDefaultConfigFile");
            #endregion

            #region External Programs
            txtLogFileViewer.BindText(_presenter.Model.StartupAndExternalModel, "LogFileViewer");
            txtFileExplorer.BindText(_presenter.Model.StartupAndExternalModel, "FileExplorer");
            #endregion
        }

        private void LoadOptionsTab()
        {
            #region Interactive Options
            chkOffline.BindChecked(_presenter.Model.OptionsModel, "OfflineLast");
            chkColorLog.BindChecked(_presenter.Model.OptionsModel, "ColorLogFile");
            chkAutoSave.BindChecked(_presenter.Model.OptionsModel, "AutoSaveConfig");
            DuplicateProjectCheckBox.BindChecked(_presenter.Model.OptionsModel, "DuplicateProjectCheck");
            ShowUserStatsCheckBox.BindChecked(_presenter.Model.OptionsModel, "ShowXmlStats");

            PpdCalculationComboBox.DataSource = OptionsModel.PpdCalculationList;
            PpdCalculationComboBox.DisplayMember = "DisplayMember";
            PpdCalculationComboBox.ValueMember = "ValueMember";
            PpdCalculationComboBox.DataBindings.Add("SelectedValue", _presenter.Model.OptionsModel, "PpdCalculation", false, DataSourceUpdateMode.OnPropertyChanged);
            BonusCalculationComboBox.DataSource = OptionsModel.BonusCalculationList;
            BonusCalculationComboBox.DisplayMember = "DisplayMember";
            BonusCalculationComboBox.ValueMember = "ValueMember";
            BonusCalculationComboBox.DataBindings.Add("SelectedValue", _presenter.Model.OptionsModel, "CalculateBonus", false, DataSourceUpdateMode.OnPropertyChanged);
            udDecimalPlaces.DataBindings.Add("Value", _presenter.Model.OptionsModel, "DecimalPlaces", false, DataSourceUpdateMode.OnPropertyChanged);
            chkEtaAsDate.BindChecked(_presenter.Model.OptionsModel, "EtaDate");
            #endregion

            #region Debug Message Level
            cboMessageLevel.DataSource = OptionsModel.DebugList;
            cboMessageLevel.DisplayMember = "DisplayMember";
            cboMessageLevel.ValueMember = "ValueMember";
            cboMessageLevel.DataBindings.Add("SelectedValue", _presenter.Model.OptionsModel, "MessageLevel", false, DataSourceUpdateMode.OnPropertyChanged);
            #endregion

            #region Form Docking Style
            cboShowStyle.DataSource = OptionsModel.DockingStyleList;
            cboShowStyle.DisplayMember = "DisplayMember";
            cboShowStyle.ValueMember = "ValueMember";
            cboShowStyle.DataBindings.Add("SelectedValue", _presenter.Model.OptionsModel, "FormShowStyle", false, DataSourceUpdateMode.OnPropertyChanged);
            #endregion
        }

        private void LoadReportingTab()
        {
            #region Email Settings
            chkEmailSecure.BindChecked(_presenter.Model.ReportingModel, "ServerSecure");
            chkEmailSecure.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            btnTestEmail.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtToEmailAddress.BindText(_presenter.Model.ReportingModel, "ToAddress");
            txtToEmailAddress.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtFromEmailAddress.BindText(_presenter.Model.ReportingModel, "FromAddress");
            txtFromEmailAddress.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtSmtpServer.BindText(_presenter.Model.ReportingModel, "ServerAddress");
            txtSmtpServer.DataBindings.Add("ErrorToolTipText", _presenter.Model.ReportingModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpServer.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtSmtpServerPort.BindText(_presenter.Model.ReportingModel, "ServerPort");
            txtSmtpServerPort.DataBindings.Add("ErrorToolTipText", _presenter.Model.ReportingModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpServerPort.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtSmtpUsername.BindText(_presenter.Model.ReportingModel, "ServerUsername");
            txtSmtpUsername.DataBindings.Add("ErrorToolTipText", _presenter.Model.ReportingModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpUsername.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            txtSmtpPassword.BindText(_presenter.Model.ReportingModel, "ServerPassword");
            txtSmtpPassword.DataBindings.Add("ErrorToolTipText", _presenter.Model.ReportingModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSmtpPassword.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");

            chkEnableEmail.BindChecked(_presenter.Model.ReportingModel, "ReportingEnabled");
            #endregion

            #region Report Selections
            grpReportSelections.BindEnabled(_presenter.Model.ReportingModel, "ReportingEnabled");
            //chkClientEuePause.BindChecked(_presenter.Model.ReportingModel, "ReportEuePause");
            //chkClientHung.BindChecked(_presenter.Model.ReportingModel, "ReportHung");
            #endregion
        }

        private void LoadWebSettingsTab()
        {
            #region Web Statistics
            txtEOCUserID.BindText(_presenter.Model.WebSettingsModel, "EocUserId");
            txtStanfordUserID.BindText(_presenter.Model.WebSettingsModel, "StanfordId");
            txtStanfordTeamID.BindText(_presenter.Model.WebSettingsModel, "TeamId");
            #endregion

            #region Project Download URL
            txtProjectDownloadUrl.BindText(_presenter.Model.WebSettingsModel, "ProjectDownloadUrl");
            #endregion

            #region Web Proxy Settings
            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox
            txtProxyServer.BindText(_presenter.Model.WebSettingsModel, "ProxyServer");
            txtProxyServer.DataBindings.Add("ErrorToolTipText", _presenter.Model.WebSettingsModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyServer.BindEnabled(_presenter.Model.WebSettingsModel, "UseProxy");

            txtProxyPort.BindText(_presenter.Model.WebSettingsModel, "ProxyPort");
            txtProxyPort.DataBindings.Add("ErrorToolTipText", _presenter.Model.WebSettingsModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPort.BindEnabled(_presenter.Model.WebSettingsModel, "UseProxy");

            // Finally, add the CheckBox.Checked Binding
            chkUseProxy.BindChecked(_presenter.Model.WebSettingsModel, "UseProxy");
            chkUseProxyAuth.BindEnabled(_presenter.Model.WebSettingsModel, "UseProxy");

            // Always Add Bindings for CheckBoxes that control input TextBoxes after
            // the data has been bound to the TextBox
            txtProxyUser.BindText(_presenter.Model.WebSettingsModel, "ProxyUser");
            txtProxyUser.DataBindings.Add("ErrorToolTipText", _presenter.Model.WebSettingsModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyUser.BindEnabled(_presenter.Model.WebSettingsModel, "ProxyAuthEnabled");

            txtProxyPass.BindText(_presenter.Model.WebSettingsModel, "ProxyPass");
            txtProxyPass.DataBindings.Add("ErrorToolTipText", _presenter.Model.WebSettingsModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
            txtProxyPass.BindEnabled(_presenter.Model.WebSettingsModel, "ProxyAuthEnabled");

            // Finally, add the CheckBox.Checked Binding
            chkUseProxyAuth.BindChecked(_presenter.Model.WebSettingsModel, "UseProxyAuth");
            #endregion
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
            StyleList.DataBindings.Add("SelectedValue", _presenter.Model.WebVisualStylesModel, "CssFile", false, DataSourceUpdateMode.OnPropertyChanged);

            txtOverview.DataBindings.Add("Text", _presenter.Model.WebVisualStylesModel, "WebOverview", false, DataSourceUpdateMode.OnPropertyChanged);
            txtSummary.DataBindings.Add("Text", _presenter.Model.WebVisualStylesModel, "WebSummary", false, DataSourceUpdateMode.OnPropertyChanged);
            txtInstance.DataBindings.Add("Text", _presenter.Model.WebVisualStylesModel, "WebSlot", false, DataSourceUpdateMode.OnPropertyChanged);
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

        #region Scheduled Tasks Tab

        private void btnBrowseWebFolder_Click(object sender, EventArgs e)
        {
            if (_presenter.Model.ScheduledTasksModel.WebRoot.Length != 0)
            {
                locateWebFolder.SelectedPath = _presenter.Model.ScheduledTasksModel.WebRoot;
            }
            if (locateWebFolder.ShowDialog() == DialogResult.OK)
            {
                _presenter.Model.ScheduledTasksModel.WebRoot = locateWebFolder.SelectedPath;
            }
        }

        #endregion

        #region Reporting Tab

        private void txtFromEmailAddress_MouseHover(object sender, EventArgs e)
        {
            if (txtFromEmailAddress.BackColor.Equals(Color.Yellow)) return;

            toolTipPrefs.RemoveAll();
            toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine),
               txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 55, 10000);
        }

        private void btnTestEmail_Click(object sender, EventArgs e)
        {
            if (_presenter.Model.ReportingModel.HasError)
            {
                MessageBox.Show(this, "Please correct error conditions before sending a Test Email.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                try
                {
                    var sendMailService = new SendMailService();
                    sendMailService.SendEmail(txtFromEmailAddress.Text, txtToEmailAddress.Text, "HFM.NET - Test Email",
                       "HFM.NET - Test Email", txtSmtpServer.Text, int.Parse(txtSmtpServerPort.Text), txtSmtpUsername.Text, txtSmtpPassword.Text, chkEmailSecure.Checked);
                    MessageBox.Show(this, "Test Email sent successfully.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                catch (Exception ex)
                {
                    _presenter.Logger.Warn(ex.Message, ex);
                    MessageBox.Show(this, String.Format("Test Email failed to send.  Please check your Email settings.{0}{0}Error: {1}", Environment.NewLine, ex.Message),
                       Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
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

        #endregion

        #region Web Tab

        private void linkEOC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(String.Concat(EocStatsService.UserBaseUrl, txtEOCUserID.Text));
            }
            catch (Exception ex)
            {
                _presenter.Logger.Error(ex.Message, ex);
                MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "EOC User Stats page"));
            }
        }

        private void linkStanford_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(String.Concat(FahUrl.UserBaseUrl, txtStanfordUserID.Text));
            }
            catch (Exception ex)
            {
                _presenter.Logger.Error(ex.Message, ex);
                MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "Stanford User Stats page"));
            }
        }

        private void linkTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(String.Concat(EocStatsService.TeamBaseUrl, txtStanfordTeamID.Text));
            }
            catch (Exception ex)
            {
                _presenter.Logger.Error(ex.Message, ex);
                MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "EOC Team Stats page"));
            }
        }

        #endregion

        #region Visual Style Tab

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
            sb.Append(String.Format("<td class=\"Plain\" colspan=\"2\" align=\"center\">Last updated {0} at {1}</td>", DateTime.Now.ToLongDateString(), DateTime.Now.ToLongTimeString()));
            sb.Append("</tr>");
            sb.Append("</table>");
            sb.Append("</BODY></HTML>");

            _cssSampleBrowser.DocumentText = sb.ToString();
        }

        #endregion

        #region Button Click Event Handlers

        private async void TestConnectionButtonClick(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;
            try
            {
                if (!_presenter.Model.ScheduledTasksModel.FtpModeEnabled)
                {
                    if (Directory.Exists(WebSiteTargetPathTextBox.Text))
                    {
                        ShowConnectionSucceededMessage();
                    }
                    else
                    {
                        ShowConnectionFailedMessage($"{WebSiteTargetPathTextBox.Text} does not exist.");
                    }
                }
                else
                {
                    string host = _presenter.Model.ScheduledTasksModel.WebGenServer;
                    int port = _presenter.Model.ScheduledTasksModel.WebGenPort;
                    string path = _presenter.Model.ScheduledTasksModel.WebRoot;
                    string username = _presenter.Model.ScheduledTasksModel.WebGenUsername;
                    string password = _presenter.Model.ScheduledTasksModel.WebGenPassword;

                    await Task.Run(() => _ftpService.CheckConnection(host, port, path, username, password, _presenter.Model.ScheduledTasksModel.FtpMode));
                    ShowConnectionSucceededMessage();
                }
            }
            catch (Exception ex)
            {
                _presenter.Logger.Error(ex.Message, ex);
                ShowConnectionFailedMessage(ex.Message);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void ShowConnectionSucceededMessage()
        {
            MessageBox.Show(this, "Test Connection Succeeded", Core.Application.NameAndVersion,
               MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void ShowConnectionFailedMessage(string message)
        {
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Test Connection Failed{0}{0}{1}",
               Environment.NewLine, message), Core.Application.NameAndVersion, MessageBoxButtons.OK,
                  MessageBoxIcon.Error);
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (CheckForErrorConditions() == false)
            {
                _presenter.OKClicked();
            }
        }

        private bool CheckForErrorConditions()
        {
            if (_presenter.Model.ScheduledTasksModel.HasError)
            {
                tabControl1.SelectedTab = tabSchdTasks;
                return true;
            }
            if (_presenter.Model.ReportingModel.HasError)
            {
                tabControl1.SelectedTab = tabReporting;
                return true;
            }
            if (_presenter.Model.WebSettingsModel.HasError)
            {
                tabControl1.SelectedTab = tabWeb;
                return true;
            }

            return false;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {

        }

        #region Folder Browsing

        private void btnBrowseConfigFile_Click(object sender, EventArgs e)
        {
            string path = DoFolderBrowse(_presenter.Model.StartupAndExternalModel.DefaultConfigFile, HfmExt, HfmFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.StartupAndExternalModel.DefaultConfigFile = path;
            }
        }

        private void btnBrowseLogViewer_Click(object sender, EventArgs e)
        {
            string path = DoFolderBrowse(_presenter.Model.StartupAndExternalModel.LogFileViewer, ExeExt, ExeFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.StartupAndExternalModel.LogFileViewer = path;
            }
        }

        private void btnBrowseFileExplorer_Click(object sender, EventArgs e)
        {
            string path = DoFolderBrowse(_presenter.Model.StartupAndExternalModel.FileExplorer, ExeExt, ExeFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.StartupAndExternalModel.FileExplorer = path;
            }
        }

        private string DoFolderBrowse(string path, string extension, string filter)
        {
            if (String.IsNullOrEmpty(path) == false)
            {
                var fileInfo = new FileInfo(path);
                if (fileInfo.Exists)
                {
                    openConfigDialog.InitialDirectory = fileInfo.DirectoryName;
                    openConfigDialog.FileName = fileInfo.Name;
                }
                else
                {
                    var dirInfo = new DirectoryInfo(path);
                    if (dirInfo.Exists)
                    {
                        openConfigDialog.InitialDirectory = dirInfo.FullName;
                        openConfigDialog.FileName = String.Empty;
                    }
                    else
                    {
                        openConfigDialog.InitialDirectory = String.Empty;
                        openConfigDialog.FileName = String.Empty;
                    }
                }
            }
            else
            {
                openConfigDialog.InitialDirectory = String.Empty;
                openConfigDialog.FileName = String.Empty;
            }

            openConfigDialog.DefaultExt = extension;
            openConfigDialog.Filter = filter;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
                return openConfigDialog.FileName;
            }

            return null;
        }

        private void btnOverviewBrowse_Click(object sender, EventArgs e)
        {
            string path = DoXsltBrowse(_presenter.Model.WebVisualStylesModel.WebOverview, XsltExt, XsltFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.WebVisualStylesModel.WebOverview = path;
            }
        }

        private void btnSummaryBrowse_Click(object sender, EventArgs e)
        {
            string path = DoXsltBrowse(_presenter.Model.WebVisualStylesModel.WebSummary, XsltExt, XsltFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.WebVisualStylesModel.WebSummary = path;
            }
        }

        private void btnInstanceBrowse_Click(object sender, EventArgs e)
        {
            string path = DoXsltBrowse(_presenter.Model.WebVisualStylesModel.WebSlot, XsltExt, XsltFilter);
            if (String.IsNullOrEmpty(path) == false)
            {
                _presenter.Model.WebVisualStylesModel.WebSlot = path;
            }
        }

        private string DoXsltBrowse(string path, string extension, string filter)
        {
            if (String.IsNullOrEmpty(path) == false)
            {
                var fileInfo = new FileInfo(path);
                string xsltPath = Path.Combine(_presenter.Model.Preferences.Get<string>(Preference.ApplicationPath), Core.Application.XsltFolderName);

                if (fileInfo.Exists)
                {
                    openConfigDialog.InitialDirectory = fileInfo.DirectoryName;
                    openConfigDialog.FileName = fileInfo.Name;
                }
                else if (File.Exists(Path.Combine(xsltPath, path)))
                {
                    openConfigDialog.InitialDirectory = xsltPath;
                    openConfigDialog.FileName = path;
                }
                else
                {
                    var dirInfo = new DirectoryInfo(path);
                    if (dirInfo.Exists)
                    {
                        openConfigDialog.InitialDirectory = dirInfo.FullName;
                        openConfigDialog.FileName = String.Empty;
                    }
                }
            }
            else
            {
                openConfigDialog.InitialDirectory = String.Empty;
                openConfigDialog.FileName = String.Empty;
            }

            openConfigDialog.DefaultExt = extension;
            openConfigDialog.Filter = filter;
            if (openConfigDialog.ShowDialog() == DialogResult.OK)
            {
                // Check to see if the path for the file returned is the \HFM\XSL path
                if (Path.Combine(_presenter.Model.Preferences.Get<string>(Preference.ApplicationPath), Core.Application.XsltFolderName).Equals(Path.GetDirectoryName(openConfigDialog.FileName)))
                {
                    // If so, return the file name only
                    return Path.GetFileName(openConfigDialog.FileName);
                }

                return openConfigDialog.FileName;
            }

            return null;
        }

        #endregion

        #endregion

        #region TextBox KeyPress Event Handler (to enforce digits only)

        private void TextBoxDigitsOnlyKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLine($"Keystroke: {(int)e.KeyChar}");

            // only allow digits special keystrokes - Issue 65
            if (char.IsDigit(e.KeyChar) == false &&
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
