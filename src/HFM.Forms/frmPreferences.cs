/*
 * HFM.NET - User Preferences Form
 * Copyright (C) 2006-2007 David Rawling
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmPreferences : FormWrapper
   // ReSharper restore InconsistentNaming
   {
      /// <summary>
      /// Tab Name Enumeration (maintain in same order as tab pages)
      /// </summary>
      private enum TabName
      {
         ScheduledTasks,
         StartupAndExternal,
         Options,
         Reporting,
         WebSettings,
         WebVisualStyles
      }
   
      #region Members

      private const string XsltExt = "xslt";
      private const string XsltFilter = "XML Transform (*.xslt;*.xsl)|*.xslt;*.xsl";
      private const string HfmExt = "hfm";
      private const string HfmFilter = "HFM Configuration Files|*.hfm";
      private const string ExeExt = "exe";
      private const string ExeFilter = "Program Files|*.exe";
      
      private readonly IPreferenceSet _prefs;
      private readonly bool _isRunningOnMono;
      
      private readonly List<IValidatingControl>[] _validatingControls;
      private readonly PropertyDescriptorCollection[] _propertyCollection;
      private readonly object[] _models;
      
      private readonly WebBrowser _cssSampleBrowser;

      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private NetworkOps _net;

      private readonly ScheduledTasksModel _scheduledTasksModel;
      private readonly StartupAndExternalModel _startupAndExternalModel;
      private readonly OptionsModel _optionsModel;
      private readonly ReportingModel _reportingModel;
      private readonly WebSettingsModel _webSettingsModel;
      private readonly WebVisualStylesModel _webVisualStylesModel;
      
      #endregion

      #region Constructor And Binding/Load Methods
      public frmPreferences(IPreferenceSet prefs)
      {
         _prefs = prefs;
         _isRunningOnMono = PlatformOps.IsRunningOnMono();
      
         InitializeComponent();

         udDecimalPlaces.Minimum = 0;
         udDecimalPlaces.Maximum = Default.MaxDecimalPlaces;

         _validatingControls = new List<IValidatingControl>[tabControl1.TabCount];
         _propertyCollection = new PropertyDescriptorCollection[tabControl1.TabCount];
         _models = new object[tabControl1.TabCount];
         if (_isRunningOnMono == false)
         {
            _cssSampleBrowser = new WebBrowser();

            pnl1CSSSample.Controls.Add(_cssSampleBrowser);

            _cssSampleBrowser.Dock = DockStyle.Fill;
            _cssSampleBrowser.Location = new Point(0, 0);
            _cssSampleBrowser.MinimumSize = new Size(20, 20);
            _cssSampleBrowser.Name = "_cssSampleBrowser";
            _cssSampleBrowser.Size = new Size(354, 208);
            _cssSampleBrowser.TabIndex = 0;
            _cssSampleBrowser.TabStop = false;
         }

         txtCollectMinutes.ErrorToolTipText = String.Format("Minutes must be a value from {0} to {1}.", Constants.MinMinutes, Constants.MaxMinutes);
         txtWebGenMinutes.ErrorToolTipText = String.Format("Minutes must be a value from {0} to {1}.", Constants.MinMinutes, Constants.MaxMinutes);

         _scheduledTasksModel = new ScheduledTasksModel(prefs);
         _startupAndExternalModel = new StartupAndExternalModel(prefs);
         _optionsModel = new OptionsModel(prefs);
         _reportingModel = new ReportingModel(prefs);
         _webSettingsModel = new WebSettingsModel(prefs);
         _webVisualStylesModel = new WebVisualStylesModel(prefs);
      }

      private void frmPreferences_Load(object sender, EventArgs e)
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
            _validatingControls[i] = FindValidatingControls(tabControl1.SelectedTab.Controls);
         }
         tabControl1.SelectTab(0);

         _scheduledTasksModel.PropertyChanged += ScheduledTasksPropertyChanged;
         _startupAndExternalModel.PropertyChanged += StartupAndExternalPropertyChanged;
         //_optionsModel.PropertyChanged += OptionsPropertyChanged;
         _reportingModel.PropertyChanged += ReportingPropertyChanged;
         _webSettingsModel.PropertyChanged += WebSettingsChanged;
         _webVisualStylesModel.PropertyChanged += WebVisualStylesPropertyChanged;
      }
      
      private static List<IValidatingControl> FindValidatingControls(Control.ControlCollection controls)
      {
         var validatingControls = new List<IValidatingControl>();

         foreach (Control control in controls)
         {
            var validatingControl = control as IValidatingControl;
            if (validatingControl != null)
            {
               validatingControls.Add(validatingControl);
            }

            validatingControls.AddRange(FindValidatingControls(control.Controls));
         }

         return validatingControls;
      }

      private void ScheduledTasksPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         SetPropertyErrorState((int)TabName.ScheduledTasks, e.PropertyName, true);
         if (_isRunningOnMono)
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
               txtCollectMinutes.Enabled = _scheduledTasksModel.SyncOnSchedule;
               break;
            case "GenerateWeb":
               radioSchedule.Enabled = _scheduledTasksModel.GenerateWeb;
               lbl2MinutesToGen.Enabled = _scheduledTasksModel.GenerateWeb;
               radioFullRefresh.Enabled = _scheduledTasksModel.GenerateWeb;
               txtWebSiteBase.Enabled = _scheduledTasksModel.GenerateWeb;
               chkHtml.Enabled = _scheduledTasksModel.GenerateWeb;
               chkXml.Enabled = _scheduledTasksModel.GenerateWeb;
               chkClientData.Enabled = _scheduledTasksModel.GenerateWeb;
               chkFAHlog.Enabled = _scheduledTasksModel.GenerateWeb;
               btnTestConnection.Enabled = _scheduledTasksModel.GenerateWeb;
               btnBrowseWebFolder.Enabled = _scheduledTasksModel.GenerateWeb;
               break;
            case "GenerateIntervalEnabled":
               txtWebGenMinutes.Enabled = _scheduledTasksModel.GenerateIntervalEnabled;
               break;
            case "FtpModeEnabled":
               pnlFtpMode.Enabled = _scheduledTasksModel.FtpModeEnabled;
               break;
            case "LimitLogSizeEnabled":
               chkLimitSize.Enabled = _scheduledTasksModel.LimitLogSizeEnabled;
               break;
            case "LimitLogSizeLengthEnabled":
               udLimitSize.Enabled = _scheduledTasksModel.LimitLogSizeLengthEnabled;
               break;
         }
      }

      private void HandleScheduledTasksPropertyChangedForMono(string propertyName)
      {
         switch (propertyName)
         {
            case "WebRoot":
               txtWebSiteBase.Text = _scheduledTasksModel.WebRoot;
               break;
         }
      }
      
      private void StartupAndExternalPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (_isRunningOnMono)
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
               txtDefaultConfigFile.Enabled = _startupAndExternalModel.UseDefaultConfigFile;
               btnBrowseConfigFile.Enabled = _startupAndExternalModel.UseDefaultConfigFile;
               break;
         }
      }

      private void HandleStartupAndExternalPropertyChangedForMono(string propertyName)
      {
         switch (propertyName)
         {
            case "DefaultConfigFile":
               txtDefaultConfigFile.Text = _startupAndExternalModel.DefaultConfigFile;
               break;
            case "LogFileViewer":
               txtLogFileViewer.Text = _startupAndExternalModel.LogFileViewer;
               break;
            case "FileExplorer":
               txtFileExplorer.Text = _startupAndExternalModel.FileExplorer;
               break;
         }
      }

      private void ReportingPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         SetPropertyErrorState((int)TabName.Reporting, e.PropertyName, true);
         if (_isRunningOnMono) HandleReportingPropertyEnabledForMono(e.PropertyName);
      }

      private void HandleReportingPropertyEnabledForMono(string propertyName)
      {
         switch (propertyName)
         {
            case "ReportingEnabled":
               chkEmailSecure.Enabled = _reportingModel.ReportingEnabled;
               btnTestEmail.Enabled = _reportingModel.ReportingEnabled;
               txtToEmailAddress.Enabled = _reportingModel.ReportingEnabled;
               txtFromEmailAddress.Enabled = _reportingModel.ReportingEnabled;
               txtSmtpServer.Enabled = _reportingModel.ReportingEnabled;
               txtSmtpServerPort.Enabled = _reportingModel.ReportingEnabled;
               txtSmtpUsername.Enabled = _reportingModel.ReportingEnabled;
               txtSmtpPassword.Enabled = _reportingModel.ReportingEnabled;
               grpReportSelections.Enabled = _reportingModel.ReportingEnabled;
               break;
         }
      }

      private void WebSettingsChanged(object sender, PropertyChangedEventArgs e)
      {
         SetPropertyErrorState((int)TabName.WebSettings, e.PropertyName, true);
         if (_isRunningOnMono) HandleWebSettingsPropertyEnabledForMono(e.PropertyName);
      }

      private void HandleWebSettingsPropertyEnabledForMono(string propertyName)
      {
         switch (propertyName)
         {
            case "UseProxy":
               txtProxyServer.Enabled = _webSettingsModel.UseProxy;
               txtProxyPort.Enabled = _webSettingsModel.UseProxy;
               chkUseProxyAuth.Enabled = _webSettingsModel.UseProxy;
               break;
            case "ProxyAuthEnabled":
               txtProxyUser.Enabled = _webSettingsModel.ProxyAuthEnabled;
               txtProxyPass.Enabled = _webSettingsModel.ProxyAuthEnabled;
               break;
         }
      }

      private void WebVisualStylesPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         if (_isRunningOnMono) HandleWebVisualStylesPropertyChangedForMono(e.PropertyName);
      }

      private void HandleWebVisualStylesPropertyChangedForMono(string propertyName)
      {
         switch (propertyName)
         {
            case "WebOverview":
               txtOverview.Text = _webVisualStylesModel.WebOverview;
               break;
            case "WebMobileOverview":
               txtMobileOverview.Text = _webVisualStylesModel.WebMobileOverview;
               break;
            case "WebSummary":
               txtSummary.Text = _webVisualStylesModel.WebSummary;
               break;
            case "WebMobileSummary":
               txtMobileSummary.Text = _webVisualStylesModel.WebMobileSummary;
               break;
            case "WebInstance":
               txtInstance.Text = _webVisualStylesModel.WebInstance;
               break;
         }
      }

      private void SetPropertyErrorState()
      {
         for (int index = 0; index < _propertyCollection.Length; index++)
         {
            foreach (PropertyDescriptor property in _propertyCollection[index])
            {
               SetPropertyErrorState(index, property.DisplayName, false);
            }
         }
      }

      private void SetPropertyErrorState(int index, string boundProperty, bool showToolTip)
      {
         var errorProperty = _propertyCollection[index].Find(boundProperty + "Error", false);
         if (errorProperty != null)
         {
            SetPropertyErrorState(index, boundProperty, errorProperty, showToolTip);
         }
      }

      private void SetPropertyErrorState(int index, string boundProperty, PropertyDescriptor errorProperty, bool showToolTip)
      {
         ICollection<IValidatingControl> validatingControls = FindBoundControls(index, boundProperty);
         var errorState = (bool)errorProperty.GetValue(_models[index]);
         foreach (var control in validatingControls)
         {
            control.ErrorState = errorState;
            if (showToolTip) control.ShowToolTip();
         }
      }

      private ReadOnlyCollection<IValidatingControl> FindBoundControls(int index, string propertyName)
      {
         return _validatingControls[index].FindAll(x => x.DataBindings["Text"].BindingMemberInfo.BindingField == propertyName).AsReadOnly();
      }

      private void LoadScheduledTasksTab()
      {
         _propertyCollection[(int)TabName.ScheduledTasks] = TypeDescriptor.GetProperties(_scheduledTasksModel);
         _models[(int)TabName.ScheduledTasks] = _scheduledTasksModel;
      
         #region Refresh Data
         chkSynchronous.DataBindings.Add("Checked", _scheduledTasksModel, "SyncOnLoad", false, DataSourceUpdateMode.OnPropertyChanged);
         chkDuplicateProject.DataBindings.Add("Checked", _scheduledTasksModel, "DuplicateProjectCheck", false, DataSourceUpdateMode.OnPropertyChanged);
         chkDuplicateUserID.DataBindings.Add("Checked", _scheduledTasksModel, "DuplicateUserIdCheck", false, DataSourceUpdateMode.OnPropertyChanged);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         
         // Add the CheckBox.Checked => TextBox.Enabled Binding
         txtCollectMinutes.DataBindings.Add("Enabled", _scheduledTasksModel, "SyncOnSchedule", false, DataSourceUpdateMode.OnPropertyChanged);
         // Bind the value to the TextBox
         txtCollectMinutes.DataBindings.Add("Text", _scheduledTasksModel, "SyncTimeMinutes", false, DataSourceUpdateMode.OnValidation);
         // Finally, add the CheckBox.Checked Binding
         chkScheduled.DataBindings.Add("Checked", _scheduledTasksModel, "SyncOnSchedule", false, DataSourceUpdateMode.OnPropertyChanged);

         chkAllowRunningAsync.DataBindings.Add("Checked", _scheduledTasksModel, "AllowRunningAsync", false, DataSourceUpdateMode.OnPropertyChanged);
         chkShowUserStats.DataBindings.Add("Checked", _scheduledTasksModel, "ShowXmlStats", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Web Generation
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox

         radioSchedule.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         lbl2MinutesToGen.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         // Bind the value to the TextBox
         txtWebGenMinutes.DataBindings.Add("Text", _scheduledTasksModel, "GenerateInterval", false, DataSourceUpdateMode.OnValidation);
         txtWebGenMinutes.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateIntervalEnabled", false, DataSourceUpdateMode.OnValidation);
         // Finally, add the RadioButton.Checked Binding
         radioFullRefresh.DataBindings.Add("Checked", _scheduledTasksModel, "WebGenAfterRefresh", false, DataSourceUpdateMode.OnPropertyChanged);
         radioFullRefresh.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);

         txtWebSiteBase.DataBindings.Add("Text", _scheduledTasksModel, "WebRoot", false, DataSourceUpdateMode.OnValidation);
         txtWebSiteBase.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         chkHtml.DataBindings.Add("Checked", _scheduledTasksModel, "CopyHtml", false, DataSourceUpdateMode.OnPropertyChanged);
         chkHtml.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         chkXml.DataBindings.Add("Checked", _scheduledTasksModel, "CopyXml", false, DataSourceUpdateMode.OnPropertyChanged);
         chkXml.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         chkClientData.DataBindings.Add("Checked", _scheduledTasksModel, "CopyClientData", false, DataSourceUpdateMode.OnPropertyChanged);
         chkClientData.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         chkFAHlog.DataBindings.Add("Checked", _scheduledTasksModel, "CopyFAHlog", false, DataSourceUpdateMode.OnPropertyChanged);
         chkFAHlog.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         pnlFtpMode.DataSource = _scheduledTasksModel;
         pnlFtpMode.ValueMember = "FtpMode";
         pnlFtpMode.DataBindings.Add("Enabled", _scheduledTasksModel, "FtpModeEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         chkLimitSize.DataBindings.Add("Checked", _scheduledTasksModel, "LimitLogSize", false, DataSourceUpdateMode.OnPropertyChanged);
         chkLimitSize.DataBindings.Add("Enabled", _scheduledTasksModel, "LimitLogSizeEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         udLimitSize.DataBindings.Add("Value", _scheduledTasksModel, "LimitLogSizeLength", false, DataSourceUpdateMode.OnPropertyChanged);
         udLimitSize.DataBindings.Add("Enabled", _scheduledTasksModel, "LimitLogSizeLengthEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         // Finally, add the CheckBox.Checked Binding
         btnTestConnection.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         btnBrowseWebFolder.DataBindings.Add("Enabled", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         chkWebSiteGenerator.DataBindings.Add("Checked", _scheduledTasksModel, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }
      
      private void LoadStartupTab()
      {
         _propertyCollection[(int)TabName.StartupAndExternal] = TypeDescriptor.GetProperties(_startupAndExternalModel);
         _models[(int)TabName.StartupAndExternal] = _startupAndExternalModel;
      
         #region Startup
         /*** Auto-Run Is Not DataBound ***/
         if (_isRunningOnMono == false)
         {
            chkAutoRun.Checked = RegistryOps.IsHfmAutoRunSet();
         }
         else
         {
            // No AutoRun under Mono
            chkAutoRun.Enabled = false;
         }
         
         chkRunMinimized.DataBindings.Add("Checked", _startupAndExternalModel, "RunMinimized", false, DataSourceUpdateMode.OnPropertyChanged);
         chkCheckForUpdate.DataBindings.Add("Checked", _startupAndExternalModel, "StartupCheckForUpdate", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Configuration File
         txtDefaultConfigFile.DataBindings.Add("Enabled", _startupAndExternalModel, "UseDefaultConfigFile", false, DataSourceUpdateMode.OnPropertyChanged);
         btnBrowseConfigFile.DataBindings.Add("Enabled", _startupAndExternalModel, "UseDefaultConfigFile", false, DataSourceUpdateMode.OnPropertyChanged);
         txtDefaultConfigFile.DataBindings.Add("Text", _startupAndExternalModel, "DefaultConfigFile", false, DataSourceUpdateMode.OnValidation);
         
         chkDefaultConfig.DataBindings.Add("Checked", _startupAndExternalModel, "UseDefaultConfigFile", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region External Programs
         txtLogFileViewer.DataBindings.Add("Text", _startupAndExternalModel, "LogFileViewer", false, DataSourceUpdateMode.OnValidation);
         txtFileExplorer.DataBindings.Add("Text", _startupAndExternalModel, "FileExplorer", false, DataSourceUpdateMode.OnValidation);
         #endregion
      }

      private void LoadOptionsTab()
      {
         _propertyCollection[(int)TabName.Options] = TypeDescriptor.GetProperties(_optionsModel);
         _models[(int)TabName.Options] = _optionsModel;
      
         #region Interactive Options
         chkOffline.DataBindings.Add("Checked", _optionsModel, "OfflineLast", false, DataSourceUpdateMode.OnPropertyChanged);
         chkColorLog.DataBindings.Add("Checked", _optionsModel, "ColorLogFile", false, DataSourceUpdateMode.OnPropertyChanged);
         chkAutoSave.DataBindings.Add("Checked", _optionsModel, "AutoSaveConfig", false, DataSourceUpdateMode.OnPropertyChanged);
         chkMaintainSelected.DataBindings.Add("Checked", _optionsModel, "MaintainSelectedClient", false, DataSourceUpdateMode.OnPropertyChanged);

         cboPpdCalc.DataSource = OptionsModel.PpdCalculationList;
         cboPpdCalc.DisplayMember = "DisplayMember";
         cboPpdCalc.ValueMember = "ValueMember";
         cboPpdCalc.DataBindings.Add("SelectedValue", _optionsModel, "PpdCalculation", false, DataSourceUpdateMode.OnPropertyChanged);
         udDecimalPlaces.DataBindings.Add("Value", _optionsModel, "DecimalPlaces", false, DataSourceUpdateMode.OnPropertyChanged);
         chkCalcBonus.DataBindings.Add("Checked", _optionsModel, "CalculateBonus", false, DataSourceUpdateMode.OnPropertyChanged);
         chkEtaAsDate.DataBindings.Add("Checked", _optionsModel, "EtaDate", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Debug Message Level
         cboMessageLevel.DataSource = OptionsModel.DebugList;
         cboMessageLevel.DisplayMember = "DisplayMember";
         cboMessageLevel.ValueMember = "ValueMember";
         cboMessageLevel.DataBindings.Add("SelectedValue", _optionsModel, "MessageLevel", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Form Docking Style
         cboShowStyle.DataSource = OptionsModel.DockingStyleList;
         cboShowStyle.DisplayMember = "DisplayMember";
         cboShowStyle.ValueMember = "ValueMember";
         cboShowStyle.DataBindings.Add("SelectedValue", _optionsModel, "FormShowStyle", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }
      
      private void LoadReportingTab()
      {
         _propertyCollection[(int)TabName.Reporting] = TypeDescriptor.GetProperties(_reportingModel);
         _models[(int)TabName.Reporting] = _reportingModel;
      
         #region Email Settings
         chkEmailSecure.DataBindings.Add("Checked", _reportingModel, "ServerSecure", false, DataSourceUpdateMode.OnPropertyChanged);
         chkEmailSecure.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

         btnTestEmail.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtToEmailAddress.DataBindings.Add("Text", _reportingModel, "ToAddress", false, DataSourceUpdateMode.OnValidation);
         txtToEmailAddress.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtFromEmailAddress.DataBindings.Add("Text", _reportingModel, "FromAddress", false, DataSourceUpdateMode.OnValidation);
         txtFromEmailAddress.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtSmtpServer.DataBindings.Add("Text", _reportingModel, "ServerAddress", false, DataSourceUpdateMode.OnValidation);
         txtSmtpServer.DataBindings.Add("ErrorToolTipText", _reportingModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtSmtpServer.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtSmtpServerPort.DataBindings.Add("Text", _reportingModel, "ServerPort", false, DataSourceUpdateMode.OnValidation);
         txtSmtpServerPort.DataBindings.Add("ErrorToolTipText", _reportingModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtSmtpServerPort.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtSmtpUsername.DataBindings.Add("Text", _reportingModel, "ServerUsername", false, DataSourceUpdateMode.OnValidation);
         txtSmtpUsername.DataBindings.Add("ErrorToolTipText", _reportingModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtSmtpUsername.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtSmtpPassword.DataBindings.Add("Text", _reportingModel, "ServerPassword", false, DataSourceUpdateMode.OnValidation);
         txtSmtpPassword.DataBindings.Add("ErrorToolTipText", _reportingModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtSmtpPassword.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

         chkEnableEmail.DataBindings.Add("Checked", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
         
         #region Report Selections
         grpReportSelections.DataBindings.Add("Enabled", _reportingModel, "ReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         chkClientEuePause.DataBindings.Add("Checked", _reportingModel, "ReportEuePause", false, DataSourceUpdateMode.OnPropertyChanged);
         chkClientHung.DataBindings.Add("Checked", _reportingModel, "ReportHung", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }

      private void LoadWebSettingsTab()
      {
         _propertyCollection[(int)TabName.WebSettings] = TypeDescriptor.GetProperties(_webSettingsModel);
         _models[(int)TabName.WebSettings] = _webSettingsModel;
      
         #region Web Statistics
         txtEOCUserID.DataBindings.Add("Text", _webSettingsModel, "EocUserId", false, DataSourceUpdateMode.OnValidation);
         txtStanfordUserID.DataBindings.Add("Text", _webSettingsModel, "StanfordId", false, DataSourceUpdateMode.OnValidation);
         txtStanfordTeamID.DataBindings.Add("Text", _webSettingsModel, "TeamId", false, DataSourceUpdateMode.OnValidation);
         #endregion
         
         #region Project Download URL
         txtProjectDownloadUrl.DataBindings.Add("Text", _webSettingsModel, "ProjectDownloadUrl", false, DataSourceUpdateMode.OnValidation);
         #endregion

         #region Web Proxy Settings
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyServer.DataBindings.Add("Text", _webSettingsModel, "ProxyServer", false, DataSourceUpdateMode.OnValidation);
         txtProxyServer.DataBindings.Add("ErrorToolTipText", _webSettingsModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtProxyServer.DataBindings.Add("Enabled", _webSettingsModel, "UseProxy", false, DataSourceUpdateMode.OnPropertyChanged);

         txtProxyPort.DataBindings.Add("Text", _webSettingsModel, "ProxyPort", false, DataSourceUpdateMode.OnValidation);
         txtProxyPort.DataBindings.Add("ErrorToolTipText", _webSettingsModel, "ServerPortPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtProxyPort.DataBindings.Add("Enabled", _webSettingsModel, "UseProxy", false, DataSourceUpdateMode.OnPropertyChanged);

         // Finally, add the CheckBox.Checked Binding
         chkUseProxy.DataBindings.Add("Checked", _webSettingsModel, "UseProxy", false, DataSourceUpdateMode.OnPropertyChanged);
         chkUseProxyAuth.DataBindings.Add("Enabled", _webSettingsModel, "UseProxy", false, DataSourceUpdateMode.OnPropertyChanged);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyUser.DataBindings.Add("Text", _webSettingsModel, "ProxyUser", false, DataSourceUpdateMode.OnValidation);
         txtProxyUser.DataBindings.Add("ErrorToolTipText", _webSettingsModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtProxyUser.DataBindings.Add("Enabled", _webSettingsModel, "ProxyAuthEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

         txtProxyPass.DataBindings.Add("Text", _webSettingsModel, "ProxyPass", false, DataSourceUpdateMode.OnValidation);
         txtProxyPass.DataBindings.Add("ErrorToolTipText", _webSettingsModel, "UsernamePasswordPairErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtProxyPass.DataBindings.Add("Enabled", _webSettingsModel, "ProxyAuthEnabled", false, DataSourceUpdateMode.OnPropertyChanged);

         // Finally, add the CheckBox.Checked Binding
         chkUseProxyAuth.DataBindings.Add("Checked", _webSettingsModel, "UseProxyAuth", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }
      
      private void LoadVisualStylesTab()
      {
         _propertyCollection[(int)TabName.WebVisualStyles] = TypeDescriptor.GetProperties(_webVisualStylesModel);
         _models[(int)TabName.WebVisualStyles] = _webVisualStylesModel;
      
         StyleList.DataSource = _webVisualStylesModel.CssFileList;
         StyleList.DisplayMember = "DisplayMember";
         StyleList.ValueMember = "ValueMember";
         StyleList.DataBindings.Add("SelectedValue", _webVisualStylesModel, "CssFile", false, DataSourceUpdateMode.OnPropertyChanged);
         
         txtOverview.DataBindings.Add("Text", _webVisualStylesModel, "WebOverview", false, DataSourceUpdateMode.OnPropertyChanged);
         txtMobileOverview.DataBindings.Add("Text", _webVisualStylesModel, "WebMobileOverview", false, DataSourceUpdateMode.OnPropertyChanged);
         txtSummary.DataBindings.Add("Text", _webVisualStylesModel, "WebSummary", false, DataSourceUpdateMode.OnPropertyChanged);
         txtMobileSummary.DataBindings.Add("Text", _webVisualStylesModel, "WebMobileSummary", false, DataSourceUpdateMode.OnPropertyChanged);
         txtInstance.DataBindings.Add("Text", _webVisualStylesModel, "WebInstance", false, DataSourceUpdateMode.OnPropertyChanged);
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
         if (_scheduledTasksModel.WebRoot.Length != 0)
         {
            locateWebFolder.SelectedPath = _scheduledTasksModel.WebRoot;
         }
         if (locateWebFolder.ShowDialog() == DialogResult.OK)
         {
            _scheduledTasksModel.WebRoot = locateWebFolder.SelectedPath;
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
         if (_reportingModel.Error)
         {
            MessageBox.Show(this, "Please correct error conditions before sending a Test Email.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
         else
         {
            try
            {
               NetworkOps.SendEmail(chkEmailSecure.Checked, txtFromEmailAddress.Text, txtToEmailAddress.Text, "HFM.NET - Test Email",
                  "HFM.NET - Test Email", txtSmtpServer.Text, int.Parse(txtSmtpServerPort.Text), txtSmtpUsername.Text, txtSmtpPassword.Text);
               MessageBox.Show(this, "Test Email sent successfully.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(TraceLevel.Warning, ex);
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
            Process.Start(String.Concat(Constants.EOCUserBaseUrl, txtEOCUserID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "EOC User Stats page"));
         }
      }

      private void linkStanford_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(Constants.StanfordBaseUrl, txtStanfordUserID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "Stanford User Stats page"));
         }
      }

      private void linkTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(Constants.EOCTeamBaseUrl, txtStanfordTeamID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
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
         if (_isRunningOnMono) return;
         
         string sStylesheet = Path.Combine(Path.Combine(_prefs.ApplicationPath, Constants.CssFolderName), _webVisualStylesModel.CssFile);
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

      private void btnTestConnection_Click(object sender, EventArgs e)
      {
         if (_net == null)
         {
            _net = new NetworkOps(_prefs);
         }

         try
         {
            SetWaitCursor();
            Match mMatchFtpWithUserPassUrl = StringOps.MatchFtpWithUserPassUrl(txtWebSiteBase.Text);
            if (mMatchFtpWithUserPassUrl.Success == false)
            {
               Action<string> del = CheckFileConnection;
               del.BeginInvoke(txtWebSiteBase.Text, CheckFileConnectionCallback, del);
            }
            else
            {
               string server = mMatchFtpWithUserPassUrl.Result("${domain}");
               string path = mMatchFtpWithUserPassUrl.Result("${file}");
               string username = mMatchFtpWithUserPassUrl.Result("${username}");
               string password = mMatchFtpWithUserPassUrl.Result("${password}");
               
               FtpCheckConnectionAction del = _net.FtpCheckConnection;
               del.BeginInvoke(server, path, username, password, _scheduledTasksModel.FtpMode, FtpCheckConnectionCallback, del);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            ShowConnectionFailedMessage(ex.Message);
         }
      }
      
      public void CheckFileConnection(string directory)
      {
         if (Directory.Exists(directory) == false)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture,
               "Folder Path '{0}' does not exist.", directory));
         }
      }

      private void CheckFileConnectionCallback(IAsyncResult result)
      {
         try
         {
            var del = (Action<string>)result.AsyncState;
            del.EndInvoke(result);
            ShowConnectionSucceededMessage();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            ShowConnectionFailedMessage(ex.Message);
         }
         finally
         {
            SetDefaultCursor();
         }
      }

      private void FtpCheckConnectionCallback(IAsyncResult result)
      {
         try
         {
            var del = (FtpCheckConnectionAction)result.AsyncState;
            del.EndInvoke(result);
            ShowConnectionSucceededMessage();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            ShowConnectionFailedMessage(ex.Message);
         }
         finally
         {
            SetDefaultCursor();
         }
      }

      private void ShowConnectionSucceededMessage()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(ShowConnectionSucceededMessage));
            return;
         }

         MessageBox.Show(this, "Test Connection Succeeded", PlatformOps.ApplicationNameAndVersion,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      private void ShowConnectionFailedMessage(string message)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<string>(ShowConnectionFailedMessage), message);
            return;
         }

         MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Test Connection Failed{0}{0}{1}",
            Environment.NewLine, message), PlatformOps.ApplicationNameAndVersion, MessageBoxButtons.OK,
               MessageBoxIcon.Error);
      }
      
      private void SetDefaultCursor()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(SetDefaultCursor));
            return;
         }
         
         Cursor = Cursors.Default;
      }
      
      private void SetWaitCursor()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(SetWaitCursor));
            return;
         }

         Cursor = Cursors.WaitCursor;
      }

      private void btnOK_Click(object sender, EventArgs e)
      {
         if (CheckForErrorConditions() == false)
         {
            GetAutoRun();
            _scheduledTasksModel.Update(_prefs);
            _startupAndExternalModel.Update(_prefs);
            _optionsModel.Update(_prefs);
            _reportingModel.Update(_prefs);
            _webSettingsModel.Update(_prefs);
            _webVisualStylesModel.Update(_prefs);
            _prefs.Save();

            DialogResult = DialogResult.OK;
            Close();
         }
      }
      
      private bool CheckForErrorConditions()
      {
         SetPropertyErrorState();
         if (_scheduledTasksModel.Error)
         {
            tabControl1.SelectedTab = tabSchdTasks;
            return true;
         }
         if (_reportingModel.Error)
         {
            tabControl1.SelectedTab = tabReporting;
            return true;
         }
         if (_webSettingsModel.Error)
         {
            tabControl1.SelectedTab = tabWeb;
            return true;
         }
         
         return false;
      }

      private void GetAutoRun()
      {
         if (_isRunningOnMono) return;
         
         try
         {
            if (chkAutoRun.Checked)
            {
               RegistryOps.SetHfmAutoRun(Application.ExecutablePath);
            }
            else
            {
               RegistryOps.SetHfmAutoRun(String.Empty);
            }
         }
         catch (InvalidOperationException ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(this, "Failed to save HFM.NET Auto Run Registry Value.  Please see the Messages Windows for detailed error information.",
               Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         _prefs.Discard();
      }

      #region Folder Browsing
      private void btnBrowseConfigFile_Click(object sender, EventArgs e)
      {
         string path = DoFolderBrowse(_startupAndExternalModel.DefaultConfigFile, HfmExt, HfmFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _startupAndExternalModel.DefaultConfigFile = path;
         }
      }

      private void btnBrowseLogViewer_Click(object sender, EventArgs e)
      {
         string path = DoFolderBrowse(_startupAndExternalModel.LogFileViewer, ExeExt, ExeFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _startupAndExternalModel.LogFileViewer = path;
         }
      }

      private void btnBrowseFileExplorer_Click(object sender, EventArgs e)
      {
         string path = DoFolderBrowse(_startupAndExternalModel.FileExplorer, ExeExt, ExeFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _startupAndExternalModel.FileExplorer = path;
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
         string path = DoXsltBrowse(_webVisualStylesModel.WebOverview, XsltExt, XsltFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _webVisualStylesModel.WebOverview = path;
         }
      }

      private void btnMobileOverviewBrowse_Click(object sender, EventArgs e)
      {
         string path = DoXsltBrowse(_webVisualStylesModel.WebMobileOverview, XsltExt, XsltFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _webVisualStylesModel.WebMobileOverview = path;
         }
      }

      private void btnSummaryBrowse_Click(object sender, EventArgs e)
      {
         string path = DoXsltBrowse(_webVisualStylesModel.WebSummary, XsltExt, XsltFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _webVisualStylesModel.WebSummary = path;
         }
      }

      private void btnMobileSummaryBrowse_Click(object sender, EventArgs e)
      {
         string path = DoXsltBrowse(_webVisualStylesModel.WebMobileSummary, XsltExt, XsltFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _webVisualStylesModel.WebMobileSummary = path;
         }
      }

      private void btnInstanceBrowse_Click(object sender, EventArgs e)
      {
         string path = DoXsltBrowse(_webVisualStylesModel.WebInstance, XsltExt, XsltFilter);
         if (String.IsNullOrEmpty(path) == false)
         {
            _webVisualStylesModel.WebInstance = path;
         }
      }

      private string DoXsltBrowse(string path, string extension, string filter)
      {
         if (String.IsNullOrEmpty(path) == false)
         {
            var fileInfo = new FileInfo(path);
            string xsltPath = Path.Combine(_prefs.ApplicationPath, Constants.XsltFolderName);
            
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
            if (Path.Combine(_prefs.ApplicationPath, Constants.XsltFolderName).Equals(Path.GetDirectoryName(openConfigDialog.FileName)))
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
      private void txtDigitsOnly_KeyPress(object sender, KeyPressEventArgs e)
      {
         Debug.WriteLine(String.Format("Keystroke: {0}", (int)e.KeyChar));
      
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
