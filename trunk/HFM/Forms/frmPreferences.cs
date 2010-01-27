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

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instrumentation;
using HFM.Preferences;

namespace HFM.Forms
{
   public partial class frmPreferences : Classes.FormWrapper
   {
      #region Members
      private readonly IPreferenceSet _Prefs;
      private readonly WebBrowser wbCssSample;

      private const string CssExtension = ".css";
      private const string CssFolder = "CSS"; 
      #endregion

      #region Constructor And Binding/Load Methods
      public frmPreferences(IPreferenceSet Prefs)
      {
         _Prefs = Prefs;
      
         InitializeComponent();

         if (PlatformOps.IsRunningOnMono() == false)
         {
            wbCssSample = new WebBrowser();

            pnl1CSSSample.Controls.Add(wbCssSample);

            wbCssSample.Dock = DockStyle.Fill;
            wbCssSample.Location = new Point(0, 0);
            wbCssSample.MinimumSize = new Size(20, 20);
            wbCssSample.Name = "wbCssSample";
            wbCssSample.Size = new Size(354, 208);
            wbCssSample.TabIndex = 0;
            wbCssSample.TabStop = false;
         }

         txtCollectMinutes.ErrorToolTipText = String.Format("Minutes must be a value from {0} to {1}.", Constants.MinMinutes, Constants.MaxMinutes);
         txtWebGenMinutes.ErrorToolTipText = String.Format("Minutes must be a value from {0} to {1}.", Constants.MinMinutes, Constants.MaxMinutes);
      }

      private void frmPreferences_Load(object sender, EventArgs e)
      {
         // Cycle through Tabs to create all controls and Bind data
         for (int i = 0; i < tabControl1.TabPages.Count; i++)
         {
            tabControl1.SelectTab(i);
         }

         tabControl1.SelectTab(0);
      }

      private void frmPreferences_Shown(object sender, EventArgs e)
      {
         LoadScheduledTasksTab();
         LoadStartupTab();
         LoadOptionsTab();
         LoadReportingTab();
         LoadWebSettingsTab();
         LoadVisualStylesTab();
      }

      private void LoadScheduledTasksTab()
      {
         #region Refresh Data
         chkSynchronous.Checked = _Prefs.GetPreference<bool>(Preference.SyncOnLoad);
         chkDuplicateProject.Checked = _Prefs.GetPreference<bool>(Preference.DuplicateProjectCheck);
         chkDuplicateUserID.Checked = _Prefs.GetPreference<bool>(Preference.DuplicateUserIDCheck);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         if (PreferenceSet.ValidateMinutes(_Prefs.GetPreference<int>(Preference.SyncTimeMinutes)) == false)
         {
            _Prefs.SetPreference(Preference.SyncTimeMinutes, Constants.MinutesDefault);
         }
         // Add the CheckBox.Checked => TextBox.Enabled Binding
         txtCollectMinutes.DataBindings.Add("Enabled", chkScheduled, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         // Bind the value to the TextBox
         txtCollectMinutes.Text = _Prefs.GetPreference<int>(Preference.SyncTimeMinutes).ToString();
         // Finally, add the CheckBox.Checked Binding
         chkScheduled.Checked = _Prefs.GetPreference<bool>(Preference.SyncOnSchedule);

         chkAllowRunningAsync.Checked = _Prefs.GetPreference<bool>(Preference.AllowRunningAsync);
         chkShowUserStats.Checked = _Prefs.GetPreference<bool>(Preference.ShowUserStats);
         #endregion

         #region Web Generation
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         if (PreferenceSet.ValidateMinutes(_Prefs.GetPreference<int>(Preference.GenerateInterval)) == false)
         {
            _Prefs.SetPreference(Preference.GenerateInterval, Constants.MinutesDefault);
         }
         // Bind the value to the TextBox
         txtWebGenMinutes.Text = _Prefs.GetPreference<int>(Preference.GenerateInterval).ToString();
         // Finally, add the RadioButton.Checked Binding
         radioFullRefresh.Checked = _Prefs.GetPreference<bool>(Preference.WebGenAfterRefresh);

         txtWebSiteBase.Text = _Prefs.GetPreference<string>(Preference.WebRoot);
         chkFAHlog.Checked = _Prefs.GetPreference<bool>(Preference.WebGenCopyFAHlog);

         // Finally, add the CheckBox.Checked Binding
         chkWebSiteGenerator.Checked = _Prefs.GetPreference<bool>(Preference.GenerateWeb);
         #endregion
      }
      
      private void LoadStartupTab()
      {
         #region Startup
         /*** Auto-Run Is Not DataBound ***/
         if (PlatformOps.IsRunningOnMono() == false)
         {
            chkAutoRun.Checked = RegistryOps.IsHfmAutoRunSet();
         }
         else
         {
            // No AutoRun under Mono
            chkAutoRun.Enabled = false;
         }
         
         chkRunMinimized.Checked = _Prefs.GetPreference<bool>(Preference.RunMinimized);
         #endregion

         #region Configuration File
         txtDefaultConfigFile.DataBindings.Add("Enabled", chkDefaultConfig, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         btnBrowseConfigFile.DataBindings.Add("Enabled", chkDefaultConfig, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         txtDefaultConfigFile.Text = _Prefs.GetPreference<string>(Preference.DefaultConfigFile);
         
         chkDefaultConfig.Checked = _Prefs.GetPreference<bool>(Preference.UseDefaultConfigFile);
         #endregion
      }

      private void LoadOptionsTab()
      {
         #region Interactive Options
         chkOffline.Checked = _Prefs.GetPreference<bool>(Preference.OfflineLast);
         chkColorLog.Checked = _Prefs.GetPreference<bool>(Preference.ColorLogFile);
         chkAutoSave.Checked = _Prefs.GetPreference<bool>(Preference.AutoSaveConfig);

         /*** PPD Calculation Is Not DataBound ***/
         IList<PpdCalculationType> ppdList = new List<PpdCalculationType>();
         ppdList.Add(PpdCalculationType.LastFrame);
         ppdList.Add(PpdCalculationType.LastThreeFrames);
         ppdList.Add(PpdCalculationType.AllFrames);
         ppdList.Add(PpdCalculationType.EffectiveRate);
         cboPpdCalc.DataSource = ppdList;
         cboPpdCalc.SelectedItem = _Prefs.GetPreference<PpdCalculationType>(Preference.PpdCalculation);

         udDecimalPlaces.Minimum = Constants.MinDecimalPlaces;
         udDecimalPlaces.Maximum = Constants.MaxDecimalPlaces;
         udDecimalPlaces.Value = _Prefs.GetPreference<int>(Preference.DecimalPlaces);

         chkCalcBonus.Checked = _Prefs.GetPreference<bool>(Preference.CalculateBonus);
         #endregion

         #region External Programs
         txtLogFileViewer.Text = _Prefs.GetPreference<string>(Preference.LogFileViewer);
         txtFileExplorer.Text = _Prefs.GetPreference<string>(Preference.FileExplorer);
         #endregion

         #region Debug Message Level
         /*** Message Level Is Not DataBound ***/
         IList<TraceLevel> traceList = new List<TraceLevel>();
         traceList.Add(TraceLevel.Off);
         traceList.Add(TraceLevel.Error);
         traceList.Add(TraceLevel.Warning);
         traceList.Add(TraceLevel.Info);
         traceList.Add(TraceLevel.Verbose);
         cboMessageLevel.DataSource = traceList;
         if (_Prefs.GetPreference<int>(Preference.MessageLevel) >= 0 && _Prefs.GetPreference<int>(Preference.MessageLevel) <= 4)
         {
            cboMessageLevel.SelectedIndex = _Prefs.GetPreference<int>(Preference.MessageLevel);
         }
         else
         {
            cboMessageLevel.SelectedItem = TraceLevel.Info;
         } 
         #endregion
      }
      
      private void LoadReportingTab()
      {
         #region Email Settings
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtToEmailAddress.Text = _Prefs.GetPreference<string>(Preference.EmailReportingToAddress);
         txtFromEmailAddress.Text = _Prefs.GetPreference<string>(Preference.EmailReportingFromAddress);
         txtSmtpServer.Text = _Prefs.GetPreference<string>(Preference.EmailReportingServerAddress);
         txtSmtpUsername.Text = _Prefs.GetPreference<string>(Preference.EmailReportingServerUsername);
         txtSmtpUsername.CompanionControls.Add(txtSmtpPassword);
         txtSmtpPassword.Text = _Prefs.GetPreference<string>(Preference.EmailReportingServerPassword);
         txtSmtpPassword.CompanionControls.Add(txtSmtpUsername);

         // Finally, add the CheckBox.Checked Binding
         chkEnableEmail.Checked = _Prefs.GetPreference<bool>(Preference.EmailReportingEnabled);
         #endregion
         
         #region Report Selections
         chkClientEuePause.Checked = _Prefs.GetPreference<bool>(Preference.ReportEuePause);
         #endregion
      }

      private void LoadWebSettingsTab()
      {
         #region Web Statistics
         txtEOCUserID.Text = _Prefs.GetPreference<int>(Preference.EocUserID).ToString();
         txtStanfordUserID.Text = _Prefs.GetPreference<string>(Preference.StanfordID);
         txtStanfordTeamID.Text = _Prefs.GetPreference<int>(Preference.TeamID).ToString();
         #endregion
         
         #region Project Download URL
         txtProjectDownloadUrl.Text = _Prefs.GetPreference<string>(Preference.ProjectDownloadUrl);
         #endregion

         #region Web Proxy Settings
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyServer.Text = _Prefs.GetPreference<string>(Preference.ProxyServer);
         txtProxyServer.CompanionControls.Add(txtProxyPort);
         txtProxyPort.Text = _Prefs.GetPreference<int>(Preference.ProxyPort).ToString();
         txtProxyPort.CompanionControls.Add(txtProxyServer);
         // Finally, add the CheckBox.Checked Binding
         chkUseProxy.Checked = _Prefs.GetPreference<bool>(Preference.UseProxy);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyUser.Text = _Prefs.GetPreference<string>(Preference.ProxyUser);
         txtProxyUser.CompanionControls.Add(txtProxyPass);
         txtProxyPass.Text = _Prefs.GetPreference<string>(Preference.ProxyPass);
         txtProxyPass.CompanionControls.Add(txtProxyUser);
         // Finally, add the CheckBox.Checked Binding
         chkUseProxyAuth.Checked = _Prefs.GetPreference<bool>(Preference.UseProxyAuth);
         #endregion
      }
      
      private void LoadVisualStylesTab()
      {
         DirectoryInfo di = new DirectoryInfo(Path.Combine(_Prefs.ApplicationPath, CssFolder));
         
         if (di.Exists)
         {
            StyleList.Items.Clear();
            foreach (FileInfo fi in di.GetFiles())
            {
               if (fi.Name.EndsWith(CssExtension))
               {
                  StyleList.Items.Add(fi.Name.Replace(CssExtension, String.Empty));
               }
            }

            for (int i = 0; i < StyleList.Items.Count; i++)
            {
               object item = StyleList.Items[i];

               if (item.ToString().ToLower().Equals(_Prefs.GetPreference<string>(Preference.CssFile).ToLower().Replace(CssExtension, String.Empty)))
               {
                  StyleList.SelectedItem = item;
               }
            }
         }
      }

      private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
      {
         toolTipPrefs.RemoveAll();
      }
      #endregion

      #region Scheduled Tasks Tab
      private void chkWebSiteGenerator_CheckedChanged(object sender, EventArgs e)
      {
         SetWebGenerator(chkWebSiteGenerator.Checked);
      }
      
      private void SetWebGenerator(bool value)
      {
         foreach (Control ctrl in grpHTMLOutput.Controls)
         {
            ctrl.CausesValidation = value;
         }

         radioSchedule.Enabled = value;
         lbl2MinutesToGen.Enabled = value;
         radioSchedule_CheckedChanged(null, EventArgs.Empty);
         radioFullRefresh.Enabled = value;

         txtWebSiteBase.Enabled = value;
         btnBrowseWebFolder.Enabled = value;

         chkFAHlog.Enabled = value;
      }

      private void radioSchedule_CheckedChanged(object sender, EventArgs e)
      {
         if (radioSchedule.Checked && radioSchedule.Enabled)
         {
            txtWebGenMinutes.Enabled = true;
         }
         else
         {
            txtWebGenMinutes.Enabled = false;
         }
      }

      private void txtWebSiteBase_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         bool bPath = StringOps.ValidatePathInstancePath(txtWebSiteBase.Text);
         bool bPathWithSlash = StringOps.ValidatePathInstancePath(String.Concat(txtWebSiteBase.Text, Path.DirectorySeparatorChar));
         bool bIsFtpUrl = StringOps.ValidateFtpWithUserPassUrl(txtWebSiteBase.Text);

         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (e.ControlText.Length > 2 && (bPath || bPathWithSlash || bIsFtpUrl) != true)
         {
            e.ValidationResult = false;
         }

         // This PathWithSlash Code seems to be defunct by the current
         // Path Regex in use.  It could probably be removed.
         if (bPath == false && bPathWithSlash)
         {
            e.ControlText += Path.DirectorySeparatorChar;
         }
      }

      private void btnBrowseWebFolder_Click(object sender, EventArgs e)
      {
         if (txtWebSiteBase.Text.Length != 0) // FxCop: CA1820
         {
            locateWebFolder.SelectedPath = txtWebSiteBase.Text;
         }
         if (locateWebFolder.ShowDialog() == DialogResult.OK)
         {
            txtWebSiteBase.Text = locateWebFolder.SelectedPath;
         }
      }

      private void txtMinutes_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         int Minutes;
         if (Int32.TryParse(e.ControlText, out Minutes) == false)
         {
            e.ValidationResult = false;
         }
         else if (PreferenceSet.ValidateMinutes(Minutes) == false)
         {
            e.ValidationResult = false;
         }
      }
      #endregion
      
      #region Reporting Tab
      private void chkEnableEmail_CheckedChanged(object sender, EventArgs e)
      {
         SetEmailReporting(chkEnableEmail.Checked);
      }
      
      private void SetEmailReporting(bool value)
      {
         foreach (Control ctrl in grpEmailSettings.Controls)
         {
            ctrl.CausesValidation = value;
         }

         txtToEmailAddress.Enabled = value;
         txtFromEmailAddress.Enabled = value;
         txtSmtpServer.Enabled = value;
         txtSmtpUsername.Enabled = value;
         txtSmtpPassword.Enabled = value;

         btnTestEmail.Enabled = value;

         grpReportSelections.Enabled = value;
         foreach (Control ctrl in grpReportSelections.Controls)
         {
            if (ctrl is CheckBox)
            {
               ctrl.Enabled = value;
            }
         }
      }

      private void txtEmailAddress_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         bool bAddress = StringOps.ValidateEmailAddress(e.ControlText);

         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (e.ControlText.Length > 0 && bAddress != true)
         {
            e.ValidationResult = false;
         }
      }

      private void txtFromEmailAddress_MouseHover(object sender, EventArgs e)
      {
         if (txtFromEmailAddress.BackColor.Equals(Color.Yellow)) return;

         toolTipPrefs.RemoveAll();
         toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine), 
            txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 55, 10000);
      }

      private void txtSmtpServer_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         bool bServerName = StringOps.ValidateServerName(e.ControlText);

         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (e.ControlText.Length > 0 && bServerName != true)
         {
            e.ValidationResult = false;
         }
      }

      private void txtSmtpCredentials_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtSmtpUsername.Text, txtSmtpPassword.Text);
         }
         catch (ArgumentException ex)
         {
            e.ErrorToolTipText = ex.Message;
            e.ValidationResult = false;
         }
      }

      private void btnTestEmail_Click(object sender, EventArgs e)
      {
         if (CheckForReportingTabErrors())
         {
            MessageBox.Show(this, "Please correct error conditions before sending a Test Email.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
         }
         else
         {
            try
            {
               NetworkOps.SendEmail(txtFromEmailAddress.Text, txtToEmailAddress.Text, "HFM.NET - Test Email",
                                    "HFM.NET - Test Email", txtSmtpServer.Text, txtSmtpUsername.Text, txtSmtpPassword.Text);
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

      private void txtProjectDownloadUrl_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = StringOps.ValidateHttpURL(txtProjectDownloadUrl.Text);
      }

      private void chkUseProxy_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxy.Checked)
         {
            EnableProxy();
            if (chkUseProxyAuth.Checked)
            {
               SetProxyAuth(true);
            }
         }
         else
         {
            DisableProxy();
         }
      }

      private void EnableProxy()
      {
         txtProxyServer.Enabled = true;
         txtProxyPort.Enabled = true;
         
         chkUseProxyAuth.Enabled = true;
      }

      private void DisableProxy()
      {
         txtProxyServer.Enabled = false;
         txtProxyPort.Enabled = false;
         
         chkUseProxyAuth.Enabled = false;
         
         SetProxyAuth(false);
      }

      private void txtProxyServerPort_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateServerPortPair(txtProxyServer.Text, txtProxyPort.Text);
         }
         catch (ArgumentException ex)
         {
            e.ErrorToolTipText = ex.Message;
            e.ValidationResult = false;
         }
      }

      private void chkUseProxyAuth_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxyAuth.Checked && chkUseProxyAuth.Enabled)
         {
            SetProxyAuth(true);
         }
         else
         {
            SetProxyAuth(false);
         }
      }

      private void SetProxyAuth(bool value)
      {
         txtProxyUser.Enabled = value;
         txtProxyPass.Enabled = value;
      }

      private void txtProxyCredentials_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtProxyUser.Text, txtProxyPass.Text, true);
         }
         catch (ArgumentException ex)
         {
            e.ErrorToolTipText = ex.Message;
            e.ValidationResult = false;
         }
      }
      #endregion

      #region Visual Style Tab
      private void StyleList_SelectedIndexChanged(object sender, EventArgs e)
      {
         String sStylesheet = Path.Combine(Path.Combine(_Prefs.ApplicationPath, CssFolder), Path.ChangeExtension(StyleList.SelectedItem.ToString(), CssExtension));
         StringBuilder sb = new StringBuilder();

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

         if (PlatformOps.IsRunningOnMono() == false)
         {
            wbCssSample.DocumentText = sb.ToString();
         }
      }
      #endregion

      #region Button Click Event Handlers
      
      private void btnOK_Click(object sender, EventArgs e)
      {
         if (CheckForErrorConditions() == false)
         {
            GetScheduledTasksTab();
            GetStartupTab();
            GetOptionsTab();
            GetReportingTab();
            GetWebSettingsTab();
            GetVisualStylesTab();

            _Prefs.Save();

            DialogResult = System.Windows.Forms.DialogResult.OK;
            Close();
         }
      }
      
      private bool CheckForErrorConditions()
      {
         if (CheckForScheduledTasksTabErrors())
         {
            tabControl1.SelectedTab = tabSchdTasks;
            return true;
         }

         if (CheckForReportingTabErrors())
         {
            tabControl1.SelectedTab = tabReporting;
            return true;
         }

         if (CheckForWebSettingsTabErrors())
         {
            tabControl1.SelectedTab = tabWeb;
            return true;
         }
         
         return false;
      }
      
      private bool CheckForScheduledTasksTabErrors()
      {
         // Check for error conditions on Scheduled Tasks Tab
         if (txtCollectMinutes.ErrorState ||
             txtWebGenMinutes.ErrorState ||
             txtWebSiteBase.ErrorState)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForReportingTabErrors()
      {
         // Check for error conditions on Reporting Tab
         if (txtToEmailAddress.ErrorState ||
             txtFromEmailAddress.ErrorState ||
             txtSmtpServer.ErrorState ||
             txtSmtpUsername.ErrorState ||
             txtSmtpPassword.ErrorState)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForWebSettingsTabErrors()
      {
         // Check for error conditions on Web Settings Tab
         if (txtProjectDownloadUrl.ErrorState ||
             txtProxyServer.ErrorState ||
             txtProxyPort.ErrorState ||
             txtProxyUser.ErrorState ||
             txtProxyPass.ErrorState)
         {
            return true;
         }
         
         return false;
      }

      private void GetScheduledTasksTab()
      {
         #region Refresh Data
         _Prefs.SetPreference(Preference.SyncOnLoad, chkSynchronous.Checked);
         _Prefs.SetPreference(Preference.DuplicateProjectCheck, chkDuplicateProject.Checked);
         _Prefs.SetPreference(Preference.DuplicateUserIDCheck, chkDuplicateUserID.Checked);

         _Prefs.SetPreference(Preference.SyncTimeMinutes, txtCollectMinutes.Text);
         _Prefs.SetPreference(Preference.SyncOnSchedule, chkScheduled.Checked);

         _Prefs.SetPreference(Preference.AllowRunningAsync, chkAllowRunningAsync.Checked);
         _Prefs.SetPreference(Preference.ShowUserStats, chkShowUserStats.Checked);
         #endregion

         #region Web Generation
         _Prefs.SetPreference(Preference.GenerateInterval, txtWebGenMinutes.Text);
         _Prefs.SetPreference(Preference.WebGenAfterRefresh, radioFullRefresh.Checked);

         _Prefs.SetPreference(Preference.WebRoot, txtWebSiteBase.Text);
         _Prefs.SetPreference(Preference.WebGenCopyFAHlog, chkFAHlog.Checked);
         
         _Prefs.SetPreference(Preference.GenerateWeb, chkWebSiteGenerator.Checked);
         #endregion
      }

      private void GetStartupTab()
      {
         #region Startup
         if (PlatformOps.IsRunningOnMono() == false)
         {
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

         _Prefs.SetPreference(Preference.RunMinimized, chkRunMinimized.Checked);
         #endregion

         #region Configuration File
         _Prefs.SetPreference(Preference.DefaultConfigFile, txtDefaultConfigFile.Text);
         _Prefs.SetPreference(Preference.UseDefaultConfigFile, chkDefaultConfig.Checked);
         #endregion
      }

      private void GetOptionsTab()
      {
         #region Interactive Options
         _Prefs.SetPreference(Preference.OfflineLast, chkOffline.Checked);
         _Prefs.SetPreference(Preference.ColorLogFile, chkColorLog.Checked);
         _Prefs.SetPreference(Preference.AutoSaveConfig, chkAutoSave.Checked);

         _Prefs.SetPreference(Preference.PpdCalculation, (PpdCalculationType)cboPpdCalc.SelectedItem);
         _Prefs.SetPreference(Preference.DecimalPlaces, (int)udDecimalPlaces.Value);
         _Prefs.SetPreference(Preference.CalculateBonus, chkCalcBonus.Checked);
         #endregion

         #region External Programs
         _Prefs.SetPreference(Preference.LogFileViewer, txtLogFileViewer.Text);
         _Prefs.SetPreference(Preference.FileExplorer, txtFileExplorer.Text);
         #endregion

         #region Debug Message Level
         _Prefs.SetPreference(Preference.MessageLevel, cboMessageLevel.SelectedIndex);
         #endregion
      }

      private void GetReportingTab()
      {
         #region Email Settings
         _Prefs.SetPreference(Preference.EmailReportingToAddress, txtToEmailAddress.Text);
         _Prefs.SetPreference(Preference.EmailReportingFromAddress, txtFromEmailAddress.Text);
         _Prefs.SetPreference(Preference.EmailReportingServerAddress, txtSmtpServer.Text);
         _Prefs.SetPreference(Preference.EmailReportingServerUsername, txtSmtpUsername.Text);
         _Prefs.SetPreference(Preference.EmailReportingServerPassword, txtSmtpPassword.Text);

         _Prefs.SetPreference(Preference.EmailReportingEnabled, chkEnableEmail.Checked);
         #endregion

         #region Report Selections
         _Prefs.SetPreference(Preference.ReportEuePause, chkClientEuePause.Checked);
         #endregion
      }

      private void GetWebSettingsTab()
      {
         #region Web Statistics
         _Prefs.SetPreference(Preference.EocUserID, txtEOCUserID.Text);
         _Prefs.SetPreference(Preference.StanfordID, txtStanfordUserID.Text);
         _Prefs.SetPreference(Preference.TeamID, txtStanfordTeamID.Text);
         #endregion

         #region Project Download URL
         _Prefs.SetPreference(Preference.ProjectDownloadUrl, txtProjectDownloadUrl.Text);
         #endregion

         #region Web Proxy Settings
         _Prefs.SetPreference(Preference.ProxyServer, txtProxyServer.Text);
         _Prefs.SetPreference(Preference.ProxyPort, txtProxyPort.Text);
         _Prefs.SetPreference(Preference.UseProxy, chkUseProxy.Checked);
         _Prefs.SetPreference(Preference.ProxyUser, txtProxyUser.Text);
         _Prefs.SetPreference(Preference.ProxyPass, txtProxyPass.Text);
         _Prefs.SetPreference(Preference.UseProxyAuth, chkUseProxyAuth.Checked);
         #endregion
      }
      
      private void GetVisualStylesTab()
      {
         _Prefs.SetPreference(Preference.CssFile, String.Concat(StyleList.SelectedItem, CssExtension));
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         _Prefs.Discard();
      }

      #region Folder Browsing
      private void btnBrowseConfigFile_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtDefaultConfigFile, "hfm", "HFM Configuration Files|*.hfm");
      }

      private void btnBrowseLogViewer_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtLogFileViewer, "exe", "Program Files|*.exe");
      }

      private void btnBrowseFileExplorer_Click(object sender, EventArgs e)
      {
         DoFolderBrowse(txtFileExplorer, "exe", "Program Files|*.exe");
      }

      private void DoFolderBrowse(Control txt, string extension, string filter)
      {
         if (String.IsNullOrEmpty(txt.Text) == false)
         {
            FileInfo fileInfo = new FileInfo(txt.Text);
            if (fileInfo.Exists)
            {
               openConfigDialog.InitialDirectory = fileInfo.DirectoryName;
               openConfigDialog.FileName = fileInfo.Name;
            }
            else
            {
               DirectoryInfo dirInfo = new DirectoryInfo(txt.Text);
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
            txt.Text = openConfigDialog.FileName;
         }
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
