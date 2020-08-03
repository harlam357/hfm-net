
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
                case nameof(WebGenerationModel.GenerateWeb):
                    radioSchedule.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    lbl2MinutesToGen.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    radioFullRefresh.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    WebSiteTargetPathTextBox.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    chkHtml.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    chkXml.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    chkFAHlog.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    TestConnectionLinkLabel.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    WebGenTypePanel.Enabled = _presenter.Model.WebGenerationModel.GenerateWeb;
                    break;
                case nameof(WebGenerationModel.GenerateIntervalEnabled):
                    txtWebGenMinutes.Enabled = _presenter.Model.WebGenerationModel.GenerateIntervalEnabled;
                    break;
                case nameof(WebGenerationModel.FtpModeEnabled):
                    WebSiteServerTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSiteServerLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSitePortTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSitePortLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSiteUsernameTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSiteUsernameLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSitePasswordTextBox.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    WebSitePasswordLabel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    FtpModePanel.Enabled = _presenter.Model.WebGenerationModel.FtpModeEnabled;
                    break;
                case nameof(WebGenerationModel.BrowseLocalPathEnabled):
                    BrowseWebFolderButton.Enabled = _presenter.Model.WebGenerationModel.BrowseLocalPathEnabled;
                    break;
                case nameof(WebGenerationModel.LimitLogSizeEnabled):
                    chkLimitSize.Enabled = _presenter.Model.WebGenerationModel.LimitLogSizeEnabled;
                    break;
                case nameof(WebGenerationModel.LimitLogSizeLengthEnabled):
                    udLimitSize.Enabled = _presenter.Model.WebGenerationModel.LimitLogSizeLengthEnabled;
                    break;
            }
        }

        private void HandleWebGenerationPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(WebGenerationModel.WebRoot):
                    WebSiteTargetPathTextBox.Text = _presenter.Model.WebGenerationModel.WebRoot;
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
                    LogFileViewerTextBox.Text = _presenter.Model.OptionsModel.LogFileViewer;
                    break;
                case nameof(OptionsModel.FileExplorer):
                    FileExplorerTextBox.Text = _presenter.Model.OptionsModel.FileExplorer;
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
                case nameof(ClientsModel.UseDefaultConfigFile):
                    txtDefaultConfigFile.Enabled = _presenter.Model.ClientsModel.UseDefaultConfigFile;
                    btnBrowseConfigFile.Enabled = _presenter.Model.ClientsModel.UseDefaultConfigFile;
                    break;
                case nameof(ClientsModel.SyncOnSchedule):
                    ClientRefreshIntervalTextBox.Enabled = _presenter.Model.ClientsModel.SyncOnSchedule;
                    break;
            }
        }

        private void HandleClientsPropertyChangedForMono(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(ClientsModel.DefaultConfigFile):
                    txtDefaultConfigFile.Text = _presenter.Model.ClientsModel.DefaultConfigFile;
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
                case nameof(ReportingModel.ReportingEnabled):
                    chkEmailSecure.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
                    SendTestEmailLinkLabel.Enabled = _presenter.Model.ReportingModel.ReportingEnabled;
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
                case nameof(WebProxyModel.UseProxy):
                    txtProxyServer.Enabled = _presenter.Model.WebProxyModel.UseProxy;
                    txtProxyPort.Enabled = _presenter.Model.WebProxyModel.UseProxy;
                    chkUseProxyAuth.Enabled = _presenter.Model.WebProxyModel.UseProxy;
                    break;
                case nameof(WebProxyModel.ProxyAuthEnabled):
                    txtProxyUser.Enabled = _presenter.Model.WebProxyModel.ProxyAuthEnabled;
                    txtProxyPass.Enabled = _presenter.Model.WebProxyModel.ProxyAuthEnabled;
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
