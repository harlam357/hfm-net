
using System.ComponentModel;

using HFM.Forms.Models;

namespace HFM.Forms
{
    public partial class PreferencesDialog
    {
        private void WebGenerationPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleWebGenerationPropertyEnabledForMono(e.PropertyName);
                HandleWebGenerationPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleWebGenerationPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WebGenerationModel.Enabled):
                    webGenerationOnScheduleRadioButton.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationIntervalLabel.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationAfterClientRetrievalRadioButton.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationPathTextBox.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationCopyHtmlCheckBox.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationCopyXmlCheckBox.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationCopyLogCheckBox.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webGenerationTestConnectionLinkLabel.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    webDeploymentTypeRadioPanel.Enabled = _presenter.Model.WebGenerationModel.Enabled;
                    break;
                case nameof(WebGenerationModel.IntervalEnabled):
                    webGenerationIntervalTextBox.Enabled = _presenter.Model.WebGenerationModel.IntervalEnabled;
                    break;
                case nameof(WebGenerationModel.FtpModeEnabled):
                    webGenerationServerTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationServerLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationPortTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationPortLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationUsernameTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationUsernameLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationPasswordTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationPasswordLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    webGenerationFtpModeRadioPanel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    break;
                case nameof(WebGenerationModel.BrowsePathEnabled):
                    webGenerationBrowsePathButton.Enabled = _presenter.Model.WebGenerationModel.BrowsePathEnabled;
                    break;
                case nameof(WebGenerationModel.LimitLogSizeEnabled):
                    webGenerationLimitLogSizeCheckBox.Enabled = _presenter.Model.WebGenerationModel.LimitLogSizeEnabled;
                    break;
                case nameof(WebGenerationModel.LimitLogSizeLengthEnabled):
                    webGenerationLimitLogSizeLengthUpDown.Enabled = _presenter.Model.WebGenerationModel.LimitLogSizeLengthEnabled;
                    break;
            }
        }

        private void HandleWebGenerationPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WebGenerationModel.Path):
                    webGenerationPathTextBox.Text = _presenter.Model.WebGenerationModel.Path;
                    break;
            }
        }

        private void OptionsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleOptionsPropertyEnabledForMono(e.PropertyName);
                HandleOptionsPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleOptionsPropertyEnabledForMono(string propertyName)
        {

        }

        private void HandleOptionsPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(OptionsModel.LogFileViewer):
                    optionsLogFileViewerTextBox.Text = _presenter.Model.OptionsModel.LogFileViewer;
                    break;
                case nameof(OptionsModel.FileExplorer):
                    optionsFileExplorerTextBox.Text = _presenter.Model.OptionsModel.FileExplorer;
                    break;
            }
        }

        private void ClientsPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleClientsPropertyEnabledForMono(e.PropertyName);
                HandleClientsPropertyChangedForMono(e.PropertyName);
            }
        }

        private void HandleClientsPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(ClientsModel.DefaultConfigFileEnabled):
                    clientsDefaultConfigFileTextBox.Enabled = _presenter.Model.ClientsModel.DefaultConfigFileEnabled;
                    clientsBrowseConfigFileButton.Enabled = _presenter.Model.ClientsModel.DefaultConfigFileEnabled;
                    break;
                case nameof(ClientsModel.RetrievalEnabled):
                    clientsRetrievalIntervalTextBox.Enabled = _presenter.Model.ClientsModel.RetrievalEnabled;
                    break;
            }
        }

        private void HandleClientsPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(ClientsModel.DefaultConfigFile):
                    clientsDefaultConfigFileTextBox.Text = _presenter.Model.ClientsModel.DefaultConfigFile;
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
                case nameof(ReportingModel.Enabled):
                    reportingIsSecureCheckBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingSendTestEmailLinkLabel.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingToAddressTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingFromAddressTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingServerTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingPortTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingUsernameTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingPasswordTextBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    reportingSelectionsGroupBox.Enabled = _presenter.Model.ReportingModel.Enabled;
                    break;
            }
        }

        private void WebProxyPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (Core.Application.IsRunningOnMono && Enabled)
            {
                HandleWebProxyPropertyEnabledForMono(e.PropertyName);
            }
        }

        private void HandleWebProxyPropertyEnabledForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WebProxyModel.Enabled):
                    webProxyServerTextBox.Enabled = _presenter.Model.WebProxyModel.Enabled;
                    webProxyPortTextBox.Enabled = _presenter.Model.WebProxyModel.Enabled;
                    webProxyCredentialsEnabledCheckBox.Enabled = _presenter.Model.WebProxyModel.Enabled;
                    break;
                case nameof(WebProxyModel.AuthenticationEnabled):
                    webProxyUsernameTextBox.Enabled = _presenter.Model.WebProxyModel.AuthenticationEnabled;
                    webProxyPasswordTextBox.Enabled = _presenter.Model.WebProxyModel.AuthenticationEnabled;
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
    }
}
