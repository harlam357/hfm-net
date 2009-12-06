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
using System.IO;
using System.Text;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Helpers;
using HFM.Instrumentation;
using HFM.Preferences;

namespace HFM.Forms
{
   public partial class frmPreferences : Classes.FormWrapper
   {
      #region Members
      private readonly WebBrowser wbCssSample;
      private readonly PreferenceSet Prefs;

      private const string CssExtension = ".css";
      private const string CssFolder = "CSS"; 
      #endregion

      #region Constructor And Binding/Load Methods
      public frmPreferences()
      {
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
         
         txtCollectMinutes.ToolTipText = String.Format("Minutes must be a value from {0} to {1}.", PreferenceSet.MinMinutes, PreferenceSet.MaxMinutes);
         txtWebGenMinutes.ToolTipText = String.Format("Minutes must be a value from {0} to {1}.", PreferenceSet.MinMinutes, PreferenceSet.MaxMinutes);

         Prefs = PreferenceSet.Instance;
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
         chkSynchronous.DataBindings.Add("Checked", Prefs, "SyncOnLoad", false, DataSourceUpdateMode.OnPropertyChanged);
         chkDuplicateProject.DataBindings.Add("Checked", Prefs, "DuplicateProjectCheck", false, DataSourceUpdateMode.OnPropertyChanged);
         chkDuplicateUserID.DataBindings.Add("Checked", Prefs, "DuplicateUserIDCheck", false, DataSourceUpdateMode.OnPropertyChanged);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         if (PreferenceSet.ValidateMinutes(Prefs.SyncTimeMinutes) == false)
         {
            Prefs.SyncTimeMinutes = PreferenceSet.MinutesDefault;
         }
         // Add the CheckBox.Checked => TextBox.Enabled Binding
         txtCollectMinutes.DataBindings.Add("Enabled", chkScheduled, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         // Bind the value to the TextBox
         txtCollectMinutes.DataBindings.Add("Text", Prefs, "SyncTimeMinutes", false, DataSourceUpdateMode.OnValidation);
         // Finally, add the CheckBox.Checked Binding
         chkScheduled.DataBindings.Add("Checked", Prefs, "SyncOnSchedule", false, DataSourceUpdateMode.OnPropertyChanged);

         chkAllowRunningAsync.DataBindings.Add("Checked", Prefs, "AllowRunningAsync", false, DataSourceUpdateMode.OnPropertyChanged);
         chkShowUserStats.DataBindings.Add("Checked", Prefs, "ShowUserStats", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Web Generation
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         if (PreferenceSet.ValidateMinutes(Prefs.GenerateInterval) == false)
         {
            Prefs.GenerateInterval = PreferenceSet.MinutesDefault;
         }
         // Bind the value to the TextBox
         txtWebGenMinutes.DataBindings.Add("Text", Prefs, "GenerateInterval", false, DataSourceUpdateMode.OnValidation);
         // Finally, add the RadioButton.Checked Binding
         radioFullRefresh.DataBindings.Add("Checked", Prefs, "WebGenAfterRefresh", false, DataSourceUpdateMode.OnPropertyChanged);

         txtWebSiteBase.DataBindings.Add("Text", Prefs, "WebRoot", false, DataSourceUpdateMode.OnValidation);
         chkFAHlog.DataBindings.Add("Checked", Prefs, "WebGenCopyFAHlog", false, DataSourceUpdateMode.OnPropertyChanged);

         // Finally, add the CheckBox.Checked Binding
         chkWebSiteGenerator.DataBindings.Add("Checked", Prefs, "GenerateWeb", false, DataSourceUpdateMode.OnPropertyChanged);
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
         
         chkRunMinimized.DataBindings.Add("Checked", Prefs, "RunMinimized", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region Configuration File
         txtDefaultConfigFile.DataBindings.Add("Enabled", chkDefaultConfig, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         btnBrowseConfigFile.DataBindings.Add("Enabled", chkDefaultConfig, "Checked", false, DataSourceUpdateMode.OnPropertyChanged);
         txtDefaultConfigFile.DataBindings.Add("Text", Prefs, "DefaultConfigFile", false, DataSourceUpdateMode.OnPropertyChanged);
         
         chkDefaultConfig.DataBindings.Add("Checked", Prefs, "UseDefaultConfigFile", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }

      private void LoadOptionsTab()
      {
         #region Interactive Options
         chkOffline.DataBindings.Add("Checked", Prefs, "OfflineLast", false, DataSourceUpdateMode.OnPropertyChanged);
         chkColorLog.DataBindings.Add("Checked", Prefs, "ColorLogFile", false, DataSourceUpdateMode.OnPropertyChanged);
         chkAutoSave.DataBindings.Add("Checked", Prefs, "AutoSaveConfig", false, DataSourceUpdateMode.OnPropertyChanged);

         /*** PPD Calculation Is Not DataBound ***/
         IList<ePpdCalculation> ppdList = new List<ePpdCalculation>();
         ppdList.Add(ePpdCalculation.LastFrame);
         ppdList.Add(ePpdCalculation.LastThreeFrames);
         ppdList.Add(ePpdCalculation.AllFrames);
         ppdList.Add(ePpdCalculation.EffectiveRate);
         cboPpdCalc.DataSource = ppdList;
         cboPpdCalc.SelectedItem = Prefs.PpdCalculation;

         udDecimalPlaces.Minimum = PreferenceSet.MinDecimalPlaces;
         udDecimalPlaces.Maximum = PreferenceSet.MaxDecimalPlaces;
         udDecimalPlaces.DataBindings.Add("Value", Prefs, "DecimalPlaces", false, DataSourceUpdateMode.OnPropertyChanged);

         chkCalcBonus.DataBindings.Add("Checked", Prefs, "CalculateBonus", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion

         #region External Programs
         txtLogFileViewer.DataBindings.Add("Text", Prefs, "LogFileViewer", false, DataSourceUpdateMode.OnPropertyChanged);
         txtFileExplorer.DataBindings.Add("Text", Prefs, "FileExplorer", false, DataSourceUpdateMode.OnPropertyChanged);
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
         if (Prefs.MessageLevel >= 0 && Prefs.MessageLevel <= 4)
         {
            cboMessageLevel.SelectedIndex = Prefs.MessageLevel;
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
         txtToEmailAddress.DataBindings.Add("Text", Prefs, "EmailReportingToAddress", false, DataSourceUpdateMode.OnValidation);
         txtFromEmailAddress.DataBindings.Add("Text", Prefs, "EmailReportingFromAddress", false, DataSourceUpdateMode.OnValidation);
         txtSmtpServer.DataBindings.Add("Text", Prefs, "EmailReportingServerAddress", false, DataSourceUpdateMode.OnValidation);
         txtSmtpUsername.DataBindings.Add("Text", Prefs, "EmailReportingServerUsername", false, DataSourceUpdateMode.OnValidation);
         txtSmtpUsername.CompanionControls.Add(txtSmtpPassword);
         txtSmtpPassword.DataBindings.Add("Text", Prefs, "EmailReportingServerPassword", false, DataSourceUpdateMode.OnValidation);
         txtSmtpPassword.CompanionControls.Add(txtSmtpUsername);

         // Finally, add the CheckBox.Checked Binding
         chkEnableEmail.DataBindings.Add("Checked", Prefs, "EmailReportingEnabled", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
         
         #region Report Selections
         chkClientEuePause.DataBindings.Add("Checked", Prefs, "ReportEuePause", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }

      private void LoadWebSettingsTab()
      {
         #region Web Statistics
         txtEOCUserID.DataBindings.Add("Text", Prefs, "EOCUserID", false, DataSourceUpdateMode.OnPropertyChanged);
         txtStanfordUserID.DataBindings.Add("Text", Prefs, "StanfordID", false, DataSourceUpdateMode.OnPropertyChanged);
         txtStanfordTeamID.DataBindings.Add("Text", Prefs, "TeamID", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
         
         #region Project Download URL
         txtProjectDownloadUrl.DataBindings.Add("Text", Prefs, "ProjectDownloadUrl", false, DataSourceUpdateMode.OnValidation);
         #endregion

         #region Web Proxy Settings
         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyServer.DataBindings.Add("Text", Prefs, "ProxyServer", false, DataSourceUpdateMode.OnValidation);
         txtProxyServer.CompanionControls.Add(txtProxyPort);
         txtProxyPort.DataBindings.Add("Text", Prefs, "ProxyPort", false, DataSourceUpdateMode.OnValidation);
         txtProxyPort.CompanionControls.Add(txtProxyServer);
         // Finally, add the CheckBox.Checked Binding
         chkUseProxy.DataBindings.Add("Checked", Prefs, "UseProxy", false, DataSourceUpdateMode.OnPropertyChanged);

         // Always Add Bindings for CheckBoxes that control input TextBoxes after
         // the data has been bound to the TextBox
         txtProxyUser.DataBindings.Add("Text", Prefs, "ProxyUser", false, DataSourceUpdateMode.OnValidation);
         txtProxyUser.CompanionControls.Add(txtProxyPass);
         txtProxyPass.DataBindings.Add("Text", Prefs, "ProxyPass", false, DataSourceUpdateMode.OnValidation);
         txtProxyPass.CompanionControls.Add(txtProxyUser);
         // Finally, add the CheckBox.Checked Binding
         chkUseProxyAuth.DataBindings.Add("Checked", Prefs, "UseProxyAuth", false, DataSourceUpdateMode.OnPropertyChanged);
         #endregion
      }
      
      private void LoadVisualStylesTab()
      {
         DirectoryInfo di = new DirectoryInfo(Path.Combine(PreferenceSet.AppPath, CssFolder));
         
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

               if (item.ToString().ToLower().Equals(Prefs.CSSFileName.ToLower().Replace(CssExtension, String.Empty)))
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

      private void txtWebSiteBase_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
      
         bool bPath = StringOps.ValidatePathInstancePath(txtWebSiteBase.Text);
         bool bPathWithSlash = StringOps.ValidatePathInstancePath(String.Concat(txtWebSiteBase.Text, Path.DirectorySeparatorChar));
         bool bIsFtpUrl = StringOps.ValidateFtpWithUserPassUrl(txtWebSiteBase.Text);

         if (e.Text.Length == 0)
         {
            e.Result = false;
         }
         else if (e.Text.Length > 2 && (bPath || bPathWithSlash || bIsFtpUrl) != true)
         {
            e.Result = false;
         }

         // This PathWithSlash Code seems to be defunct by the current
         // Path Regex in use.  It could probably be removed.
         if (bPath == false && bPathWithSlash)
         {
            e.Text += Path.DirectorySeparatorChar;
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

      private void txtMinutes_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
      
         int Minutes;
         if (Int32.TryParse(e.Text, out Minutes) == false)
         {
            e.Result = false;
         }
         else if (PreferenceSet.ValidateMinutes(Minutes) == false)
         {
            e.Result = false;
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

      private void txtEmailAddress_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
         bool bAddress = StringOps.ValidateEmailAddress(e.Text);

         if (e.Text.Length == 0)
         {
            e.Result = false;
         }
         else if (e.Text.Length > 0 && bAddress != true)
         {
            e.Result = false;
         }
      }

      private void txtFromEmailAddress_MouseHover(object sender, EventArgs e)
      {
         if (txtFromEmailAddress.BackColor.Equals(Color.Yellow)) return;

         toolTipPrefs.RemoveAll();
         toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine), 
            txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 55, 10000);
      }

      private void txtSmtpServer_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
         bool bServerName = StringOps.ValidateServerName(e.Text);

         if (e.Text.Length == 0)
         {
            e.Result = false;
         }
         else if (e.Text.Length > 0 && bServerName != true)
         {
            e.Result = false;
         }
      }

      private void txtSmtpCredentials_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
         
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtSmtpUsername.Text, txtSmtpPassword.Text);
         }
         catch (ArgumentException ex)
         {
            e.ToolTipText = ex.Message;
            e.Result = false;
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
            Process.Start(String.Concat(PreferenceSet.EOCUserBaseUrl, txtEOCUserID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, "EOC User Stats page"));
         }
      }

      private void linkStanford_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.StanfordBaseUrl, txtStanfordUserID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, "Stanford User Stats page"));
         }
      }

      private void linkTeam_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.EOCTeamBaseUrl, txtStanfordTeamID.Text));
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, "EOC Team Stats page"));
         }
      }

      private void txtProjectDownloadUrl_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = StringOps.ValidateHttpURL(txtProjectDownloadUrl.Text);
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

      private void txtProxyServerPort_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
      
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateServerPortPair(txtProxyServer.Text, txtProxyPort.Text);
         }
         catch (ArgumentException ex)
         {
            e.ToolTipText = ex.Message;
            e.Result = false;
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

      private void txtProxyCredentials_CustomValidation(object sender, CustomValidationEventArgs e)
      {
         e.Result = true;
      
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtProxyUser.Text, txtProxyPass.Text, true);
         }
         catch (ArgumentException ex)
         {
            e.ToolTipText = ex.Message;
            e.Result = false;
         }
      }
      #endregion

      #region Visual Style Tab
      private void StyleList_SelectedIndexChanged(object sender, EventArgs e)
      {
         String sStylesheet = Path.Combine(Path.Combine(PreferenceSet.AppPath, CssFolder), Path.ChangeExtension(StyleList.SelectedItem.ToString(), CssExtension));
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
            GetStartupTab();
            GetOptionsTab();
            GetVisualStylesTab();

            Prefs.Save();

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
         if (txtCollectMinutes.Error ||
             txtWebGenMinutes.Error ||
             txtWebSiteBase.Error)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForReportingTabErrors()
      {
         // Check for error conditions on Reporting Tab
         if (txtToEmailAddress.Error ||
             txtFromEmailAddress.Error ||
             txtSmtpServer.Error ||
             txtSmtpUsername.Error ||
             txtSmtpPassword.Error)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForWebSettingsTabErrors()
      {
         // Check for error conditions on Web Settings Tab
         if (txtProjectDownloadUrl.Error ||
             txtProxyServer.Error ||
             txtProxyPort.Error ||
             txtProxyUser.Error ||
             txtProxyPass.Error)
         {
            return true;
         }
         
         return false;
      }

      private void GetStartupTab()
      {
         #region Auto-Run
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
         #endregion
      }

      private void GetOptionsTab()
      {
         #region PPD Calculation
         Prefs.PpdCalculation = (ePpdCalculation)cboPpdCalc.SelectedItem;
         #endregion

         #region Debug Message Level
         Prefs.MessageLevel = cboMessageLevel.SelectedIndex;
         #endregion
      }
      
      private void GetVisualStylesTab()
      {
         Prefs.CSSFileName = String.Concat(StyleList.SelectedItem, CssExtension);
      }

      private void btnCancel_Click(object sender, EventArgs e)
      {
         Prefs.Discard();
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
