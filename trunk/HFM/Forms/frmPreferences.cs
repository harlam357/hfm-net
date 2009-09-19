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
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

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

      #region Form Constructor / functionality
      public frmPreferences()
      {
         InitializeComponent();

         if (PlatformOps.IsRunningOnMono())
         {
            chkAutoRun.Checked = false;
            grpAutoRun.Enabled = false;
         }
         else
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

         Prefs = PreferenceSet.Instance;
      }

      private void frmPreferences_Shown(object sender, EventArgs e)
      {
         LoadScheduledTasksTab();
         LoadDefaultsTab();
         LoadReportingTab();
         LoadWebTab();
         LoadVisualStyleTab();
      }

      private void LoadScheduledTasksTab()
      {
         chkSynchronous.Checked = Prefs.SyncOnLoad;
         chkScheduled.Checked = Prefs.SyncOnSchedule;
         if (PreferenceSet.ValidateMinutes(Prefs.SyncTimeMinutes))
         {
            txtCollectMinutes.Text = Prefs.SyncTimeMinutes.ToString();
         }
         else
         {
            txtCollectMinutes.Text = PreferenceSet.MinutesDefault.ToString();
         }
         chkOffline.Checked = Prefs.OfflineLast;
         chkShowUserStats.Checked = Prefs.ShowUserStats;
         chkDuplicateUserID.Checked = Prefs.DuplicateUserIDCheck;
         chkDuplicateProject.Checked = Prefs.DuplicateProjectCheck;
         chkColorLog.Checked = Prefs.ColorLogFile;
         cboPpdCalc.Items.Add(ePpdCalculation.LastFrame);
         cboPpdCalc.Items.Add(ePpdCalculation.LastThreeFrames);
         cboPpdCalc.Items.Add(ePpdCalculation.AllFrames);
         cboPpdCalc.Items.Add(ePpdCalculation.EffectiveRate);
         cboPpdCalc.Text = Prefs.PpdCalculation.ToString();

         if (PreferenceSet.ValidateMinutes(Prefs.GenerateInterval))
         {
            txtWebGenMinutes.Text = Prefs.GenerateInterval.ToString();
         }
         else
         {
            txtWebGenMinutes.Text = PreferenceSet.MinutesDefault.ToString();
         }
         txtWebSiteBase.Text = Prefs.WebRoot;
         chkWebSiteGenerator.Checked = Prefs.GenerateWeb;
         if (Prefs.WebGenAfterRefresh)
         {
            radioFullRefresh.Checked = true;
         }
         else
         {
            radioSchedule.Checked = true;
         }
         
         if (PlatformOps.IsRunningOnMono() == false)
         {
            chkAutoRun.Checked = RegistryOps.IsHfmAutoRunSet();
         }
      }

      private void LoadDefaultsTab()
      {
         chkDefaultConfig.Checked = Prefs.UseDefaultConfigFile;
         txtDefaultConfigFile.Text = Prefs.DefaultConfigFile;
         chkAutoSave.Checked = Prefs.AutoSaveConfig;
         txtLogFileViewer.Text = Prefs.LogFileViewer;
         txtFileExplorer.Text = Prefs.FileExplorer;
         cboMessageLevel.Items.Add(TraceLevel.Off.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Error.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Warning.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Info.ToString());
         cboMessageLevel.Items.Add(TraceLevel.Verbose.ToString());
         if (Prefs.MessageLevel >= 0 && Prefs.MessageLevel <= 4)
         {
            cboMessageLevel.SelectedIndex = Prefs.MessageLevel;
         }
         else
         {
            cboMessageLevel.SelectedIndex = (int)TraceLevel.Info;
         }
         udDecimalPlaces.Minimum = PreferenceSet.MinDecimalPlaces;
         udDecimalPlaces.Maximum = PreferenceSet.MaxDecimalPlaces;
         udDecimalPlaces.Value = Prefs.DecimalPlaces;
      }
      
      private void LoadReportingTab()
      {
         txtToEmailAddress.Text = Prefs.EmailReportingToAddress;
         txtFromEmailAddress.Text = Prefs.EmailReportingFromAddress;
         txtSmtpServer.Text = Prefs.EmailReportingServerAddress;
         txtSmtpUsername.Text = Prefs.EmailReportingServerUsername;
         txtSmtpPassword.Text = Prefs.EmailReportingServerPassword;
         
         chkEnableEmail.Checked = Prefs.EmailReportingEnabled;
         
         chkClientEuePause.Checked = Prefs.ReportEuePause;
      }

      private void LoadWebTab()
      {
         txtEOCUserID.Text = Prefs.EOCUserID.ToString();
         txtStanfordUserID.Text = Prefs.StanfordID;
         txtStanfordTeamID.Text = Prefs.TeamID.ToString();
         txtProjectDownloadUrl.Text = Prefs.ProjectDownloadUrl;

         txtProxyServer.Text = Prefs.ProxyServer;
         txtProxyPort.Text = Prefs.ProxyPort.ToString();
         chkUseProxy.Checked = Prefs.UseProxy;

         txtProxyUser.Text = Prefs.ProxyUser;
         txtProxyPass.Text = Prefs.ProxyPass;
         chkUseProxyAuth.Checked = Prefs.UseProxyAuth;
      }
      
      private void LoadVisualStyleTab()
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
      private void chkScheduled_CheckedChanged(object sender, EventArgs e)
      {
         if (chkScheduled.Checked)
         {
            txtCollectMinutes.ReadOnly = false;
         }
         else
         {
            txtCollectMinutes.ReadOnly = true;
         }
      }

      private void chkWebSiteGenerator_CheckedChanged(object sender, EventArgs e)
      {
         if (chkWebSiteGenerator.Checked)
         {
            foreach (Control ctrl in grpHTMLOutput.Controls)
            {
               ctrl.CausesValidation = true;
            }
         
            radioSchedule.Enabled = true;
            lbl2MinutesToGen.Enabled = true;
            radioSchedule_CheckedChanged(sender, e);
            radioFullRefresh.Enabled = true;
            
            txtWebSiteBase.Enabled = true;
            txtWebSiteBase.ReadOnly = false;
            txtWebSiteBase.BackColor = SystemColors.Window;
            txtWebSiteBase_Validating(null, null);
            
            btnBrowseWebFolder.Enabled = true;
         }
         else
         {
            foreach (Control ctrl in grpHTMLOutput.Controls)
            {
               ctrl.CausesValidation = false;
            }
         
            radioSchedule.Enabled = false;
            lbl2MinutesToGen.Enabled = false;
            radioSchedule_CheckedChanged(sender, e);
            radioFullRefresh.Enabled = false;
            
            txtWebSiteBase.Enabled = false;
            txtWebSiteBase.BackColor = SystemColors.Control;
            txtWebSiteBase.ReadOnly = true;
            
            btnBrowseWebFolder.Enabled = false;
         }
      }

      private void radioSchedule_CheckedChanged(object sender, EventArgs e)
      {
         if (radioSchedule.Checked && radioSchedule.Enabled)
         {
            txtWebGenMinutes.Enabled = true;
            txtWebGenMinutes.ReadOnly = false;
         }
         else
         {
            txtWebGenMinutes.Enabled = false;
            txtWebGenMinutes.ReadOnly = true;
         }
      }

      private void txtMinutes_Validating(object sender, CancelEventArgs e)
      {
         Control textBox = (Control)sender;

         int Minutes;
         if (int.TryParse(textBox.Text, out Minutes) == false)
         {
            SetMinutesError(textBox);
         }
         else if (PreferenceSet.ValidateMinutes(Minutes) == false)
         {
            SetMinutesError(textBox);
         }
         else
         {
            textBox.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(textBox.Parent);
         }
      }
      
      private void SetMinutesError(Control textBox)
      {
         textBox.BackColor = Color.Yellow;
         textBox.Focus();
         if (textBox.Visible)
         {
            toolTipPrefs.Show(String.Format("Minutes must be a value from {0} to {1}", PreferenceSet.MinMinutes, PreferenceSet.MaxMinutes),
                              textBox.Parent, textBox.Location.X + 5, textBox.Location.Y - 20, 5000);
         }
      }

      private void txtWebSiteBase_Validating(object sender, CancelEventArgs e)
      {
         bool bPath = StringOps.ValidatePathInstancePath(txtWebSiteBase.Text);
         bool bPathWithSlash = StringOps.ValidatePathInstancePath(String.Concat(txtWebSiteBase.Text, Path.DirectorySeparatorChar));
         bool bIsFtpUrl = StringOps.ValidateFtpWithUserPassUrl(txtWebSiteBase.Text);

         if (txtWebSiteBase.Text.Length == 0)
         {
            SetWebFolderError();
         }
         else if (txtWebSiteBase.Text.Length > 2 && (bPath || bPathWithSlash || bIsFtpUrl) != true)
         {
            SetWebFolderError();
         }
         else
         {
            if (bPath == false && bPathWithSlash)
            {
               txtWebSiteBase.Text += Path.DirectorySeparatorChar;
            }
            txtWebSiteBase.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtWebSiteBase);
         }
      }

      private void SetWebFolderError()
      {
         txtWebSiteBase.BackColor = Color.Yellow;
         txtWebSiteBase.Focus();
         toolTipPrefs.Show("HTML Output Folder must be a valid local path, network (UNC) path, or FTP URL",
            txtWebSiteBase.Parent, txtWebSiteBase.Location.X + 5, txtWebSiteBase.Location.Y - 20, 5000);
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
      #endregion
 
      #region Defaults Tab
      private void chkDefaultConfig_CheckedChanged(object sender, EventArgs e)
      {
         if (chkDefaultConfig.Checked)
         {
            txtDefaultConfigFile.Enabled = true;
            txtDefaultConfigFile.ReadOnly = false;
            btnBrowseConfigFile.Enabled = true;
         }
         else
         {
            txtDefaultConfigFile.Enabled = false;
            txtDefaultConfigFile.ReadOnly = true;
            btnBrowseConfigFile.Enabled = false;
         }
      } 
      #endregion
      
      #region Reporting Tab
      private void chkEnableEmail_CheckedChanged(object sender, EventArgs e)
      {
         if (chkEnableEmail.Checked)
         {
            EnableEmailReporting();
         }
         else
         {
            DisableEmailReporting();
         }
      }
      
      private void EnableEmailReporting()
      {
         foreach (Control ctrl in grpEmailSettings.Controls)
         {
            ctrl.CausesValidation = true;
         }

         txtToEmailAddress.Enabled = true;
         txtToEmailAddress.ReadOnly = false;
         txtToEmailAddress.BackColor = SystemColors.Window;
         txtToEmailAddress_Validating(null, null);

         txtFromEmailAddress.Enabled = true;
         txtFromEmailAddress.ReadOnly = false;
         txtToEmailAddress.BackColor = SystemColors.Window;
         txtFromEmailAddress_Validating(null, null);

         txtSmtpServer.Enabled = true;
         txtSmtpServer.ReadOnly = false;
         txtToEmailAddress.BackColor = SystemColors.Window;
         txtSmtpServer_Validating(null, null);

         txtSmtpUsername.Enabled = true;
         txtSmtpUsername.ReadOnly = false;
         txtSmtpUsername.BackColor = SystemColors.Window;

         txtSmtpPassword.Enabled = true;
         txtSmtpPassword.ReadOnly = false;
         txtSmtpPassword.BackColor = SystemColors.Window;

         DoSmtpCredentialValidation();

         btnTestEmail.Enabled = true;
         
         grpReportSelections.Enabled = true;
         foreach (Control ctrl in grpReportSelections.Controls)
         {
            if (ctrl is CheckBox)
            {
               ctrl.Enabled = true;
            }
         }
      }
      
      private void DisableEmailReporting()
      {
         foreach (Control ctrl in grpEmailSettings.Controls)
         {
            ctrl.CausesValidation = false;
         }

         txtToEmailAddress.Enabled = false;
         txtToEmailAddress.BackColor = SystemColors.Control;
         txtToEmailAddress.ReadOnly = true;

         txtFromEmailAddress.Enabled = false;
         txtFromEmailAddress.BackColor = SystemColors.Control;
         txtFromEmailAddress.ReadOnly = true;

         txtSmtpServer.Enabled = false;
         txtSmtpServer.BackColor = SystemColors.Control;
         txtSmtpServer.ReadOnly = true;

         txtSmtpUsername.Enabled = false;
         txtSmtpUsername.BackColor = SystemColors.Control;
         txtSmtpUsername.ReadOnly = true;

         txtSmtpPassword.Enabled = false;
         txtSmtpPassword.BackColor = SystemColors.Control;
         txtSmtpPassword.ReadOnly = true;

         btnTestEmail.Enabled = false;

         grpReportSelections.Enabled = false;
         foreach (Control ctrl in grpReportSelections.Controls)
         {
            if (ctrl is CheckBox)
            {
               ctrl.Enabled = false;
            }
         }
      }

      private void txtToEmailAddress_Validating(object sender, CancelEventArgs e)
      {
         bool bAddress = StringOps.ValidateEmailAddress(txtToEmailAddress.Text);

         if (txtToEmailAddress.Text.Length == 0)
         {
            SetToEmailAddressError();
         }
         else if (txtToEmailAddress.Text.Length > 0 && bAddress != true)
         {
            SetToEmailAddressError();
         }
         else
         {
            txtToEmailAddress.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtToEmailAddress);
         }
      }

      private void SetToEmailAddressError()
      {
         txtToEmailAddress.BackColor = Color.Yellow;
         txtToEmailAddress.Focus();
         toolTipPrefs.Show("Must be a valid e-mail address.",
            txtToEmailAddress.Parent, txtToEmailAddress.Location.X + 5, txtToEmailAddress.Location.Y - 20, 5000);
      }

      private void txtFromEmailAddress_Validating(object sender, CancelEventArgs e)
      {
         bool bAddress = StringOps.ValidateEmailAddress(txtFromEmailAddress.Text);

         if (txtFromEmailAddress.Text.Length == 0)
         {
            SetFromEmailAddressError();
         }
         else if (txtFromEmailAddress.Text.Length > 0 && bAddress != true)
         {
            SetFromEmailAddressError();
         }
         else
         {
            txtFromEmailAddress.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtFromEmailAddress);
         }
      }

      private void SetFromEmailAddressError()
      {
         txtFromEmailAddress.BackColor = Color.Yellow;
         txtFromEmailAddress.Focus();
         toolTipPrefs.Show("Must be a valid e-mail address.",
            txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 20, 5000);
      }

      private void txtFromEmailAddress_MouseHover(object sender, EventArgs e)
      {
         if (txtFromEmailAddress.BackColor.Equals(Color.Yellow)) return;

         toolTipPrefs.RemoveAll();
         toolTipPrefs.Show(String.Format("Depending on your SMTP server, this 'From Address' field may or may not be of consequence.{0}If you are required to enter credentials to send Email through the SMTP server, the server will{0}likely use the Email Address tied to those credentials as the sender or 'From Address'.{0}Regardless of this limitation, a valid Email Address must still be specified here.", Environment.NewLine), 
            txtFromEmailAddress.Parent, txtFromEmailAddress.Location.X + 5, txtFromEmailAddress.Location.Y - 55, 10000);
      }

      private void txtSmtpServer_Validating(object sender, CancelEventArgs e)
      {
         bool bServerName = StringOps.ValidateServerName(txtSmtpServer.Text);

         if (txtSmtpServer.Text.Length == 0)
         {
            SetSmtpServerError();
         }
         else if (txtSmtpServer.Text.Length > 0 && bServerName != true)
         {
            SetSmtpServerError();
         }
         else
         {
            txtSmtpServer.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtSmtpServer);
         }
      }

      private void SetSmtpServerError()
      {
         txtSmtpServer.BackColor = Color.Yellow;
         txtSmtpServer.Focus();
         toolTipPrefs.Show("Must be a valid server name.",
            txtToEmailAddress.Parent, txtToEmailAddress.Location.X + 5, txtToEmailAddress.Location.Y - 20, 5000);
      }

      private void txtSmtpUsername_Validating(object sender, CancelEventArgs e)
      {
         DoSmtpCredentialValidation();
      }

      private void txtSmtpPassword_Validating(object sender, CancelEventArgs e)
      {
         DoSmtpCredentialValidation();
      }
      
      private void DoSmtpCredentialValidation()
      {
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtSmtpUsername.Text, txtSmtpPassword.Text);

            txtSmtpUsername.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtSmtpUsername);
            txtSmtpPassword.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtSmtpPassword);
         }
         catch (ArgumentException ex)
         {
            SetSmtpUsernamePasswordError(ex.Message);
         }
      }

      private void SetSmtpUsernamePasswordError(string Message)
      {
         txtSmtpUsername.BackColor = Color.Yellow;
         txtSmtpPassword.BackColor = Color.Yellow;
         toolTipPrefs.Show(Message,
            txtSmtpUsername.Parent, txtSmtpUsername.Location.X + 5, txtSmtpUsername.Location.Y - 20, 5000);
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

      private void txtProjectDownloadUrl_Validating(object sender, CancelEventArgs e)
      {
         Control textBox = (Control)sender;

         if (StringOps.ValidateHttpURL(txtProjectDownloadUrl.Text) == false)
         {
            txtProjectDownloadUrl.BackColor = Color.Yellow;
            if (txtProjectDownloadUrl.Visible)
            {
               txtProjectDownloadUrl.Focus();
               toolTipPrefs.Show("URL must be a valid URL and the path to a valid Stanford Project Summary page",
                                 textBox.Parent, textBox.Location.X + 5, textBox.Location.Y - 20, 5000);
            }
         }
         else
         {
            txtProjectDownloadUrl.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(textBox.Parent);
         }
      }

      private void chkUseProxy_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxy.Checked)
         {
            EnableProxy();
            if (chkUseProxyAuth.Checked)
            {
               EnableProxyAuth();
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
         txtProxyServer.ReadOnly = false;
         txtProxyServer.BackColor = SystemColors.Window;
         txtProxyServer.CausesValidation = true;

         txtProxyPort.Enabled = true;
         txtProxyPort.ReadOnly = false;
         txtProxyPort.BackColor = SystemColors.Window;
         txtProxyPort.CausesValidation = true;

         DoProxyServerPortValidation();
         
         chkUseProxyAuth.Enabled = true;
      }

      private void DisableProxy()
      {
         txtProxyServer.Enabled = false;
         txtProxyServer.ReadOnly = true;
         txtProxyServer.BackColor = SystemColors.Control;
         txtProxyServer.CausesValidation = false;
            
         txtProxyPort.Enabled = false;
         txtProxyPort.ReadOnly = true;
         txtProxyPort.BackColor = SystemColors.Control;
         txtProxyPort.CausesValidation = false;
         
         chkUseProxyAuth.Enabled = false;
         DisableProxyAuth();
      }

      private void txtProxyServer_Validating(object sender, CancelEventArgs e)
      {
         DoProxyServerPortValidation();
      }

      private void txtProxyPort_Validating(object sender, CancelEventArgs e)
      {
         DoProxyServerPortValidation();
      }

      private void DoProxyServerPortValidation()
      {
         try
         {
            ValidateProxyServerPort(txtProxyServer.Text, txtProxyPort.Text);

            txtProxyServer.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtProxyServer);
            txtProxyPort.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtProxyPort);
         }
         catch (ArgumentException ex)
         {
            SetProxyServerPortError(ex.Message);
         }
      }
      
      private static void ValidateProxyServerPort(string ProxyServer, string ProxyPort)
      {
         if (String.IsNullOrEmpty(ProxyServer) && String.IsNullOrEmpty(ProxyPort))
         {
            throw new ArgumentException("Proxy Server and Port must be specified.");
         }
         if (String.IsNullOrEmpty(ProxyServer) == false && String.IsNullOrEmpty(ProxyPort))
         {
            throw new ArgumentException("Proxy Port must also be specified when specifying Proxy Server.");
         }
         else if (String.IsNullOrEmpty(ProxyServer) && String.IsNullOrEmpty(ProxyPort) == false)
         {
            throw new ArgumentException("Proxy Server must also be specified when specifying Proxy Port.");
         }
      }

      private void SetProxyServerPortError(string Message)
      {
         txtProxyServer.BackColor = Color.Yellow;
         txtProxyPort.BackColor = Color.Yellow;
         toolTipPrefs.Show(Message,
            txtProxyServer.Parent, txtProxyServer.Location.X + 5, txtProxyServer.Location.Y - 20, 5000);
      }

      private void chkUseProxyAuth_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxyAuth.Checked && chkUseProxyAuth.Enabled)
         {
            EnableProxyAuth();
         }
         else
         {
            DisableProxyAuth();
         }
      }

      private void EnableProxyAuth()
      {
         txtProxyUser.Enabled = true;
         txtProxyUser.ReadOnly = false;
         txtProxyUser.BackColor = SystemColors.Window;
         txtProxyUser.CausesValidation = true;
         
         txtProxyPass.Enabled = true;
         txtProxyPass.ReadOnly = false;
         txtProxyPass.BackColor = SystemColors.Window;
         txtProxyPass.CausesValidation = true;
         
         DoProxyCredentialValidation();
      }

      private void DisableProxyAuth()
      {
         txtProxyUser.Enabled = false;
         txtProxyUser.ReadOnly = true;
         txtProxyUser.BackColor = SystemColors.Control;
         txtProxyUser.CausesValidation = false;
         
         txtProxyPass.Enabled = false;
         txtProxyPass.ReadOnly = true;
         txtProxyPass.BackColor = SystemColors.Control;
         txtProxyPass.CausesValidation = false;
      }

      private void txtProxyUser_Validating(object sender, CancelEventArgs e)
      {
         DoProxyCredentialValidation();
      }

      private void txtProxyPass_Validating(object sender, CancelEventArgs e)
      {
         DoProxyCredentialValidation();
      }

      private void DoProxyCredentialValidation()
      {
         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtProxyUser.Text, txtProxyPass.Text);

            txtProxyUser.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtProxyUser);
            txtProxyPass.BackColor = SystemColors.Window;
            toolTipPrefs.Hide(txtProxyPass);
         }
         catch (ArgumentException ex)
         {
            SetProxyUsernamePasswordError(ex.Message);
         }
      }

      private void SetProxyUsernamePasswordError(string Message)
      {
         txtProxyUser.BackColor = Color.Yellow;
         txtProxyPass.BackColor = Color.Yellow;
         toolTipPrefs.Show(Message,
            txtProxyUser.Parent, txtProxyUser.Location.X + 5, txtProxyUser.Location.Y - 20, 5000);
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
            GetDataScheduledTasksTab();
            GetDataDefaultsTab();
            GetReportingTab();
            GetDataWebSettingsTab();
            GetDataVisualStylesTab();

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
         if (txtCollectMinutes.BackColor == Color.Yellow ||
             txtWebGenMinutes.BackColor == Color.Yellow ||
             txtWebSiteBase.BackColor == Color.Yellow)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForReportingTabErrors()
      {
         // Check for error conditions on Reporting Tab
         if (txtToEmailAddress.BackColor == Color.Yellow ||
             txtFromEmailAddress.BackColor == Color.Yellow ||
             txtSmtpServer.BackColor == Color.Yellow ||
             txtSmtpUsername.BackColor == Color.Yellow ||
             txtSmtpPassword.BackColor == Color.Yellow)
         {
            return true;
         }
         
         return false;
      }
      
      private bool CheckForWebSettingsTabErrors()
      {
         // Check for error conditions on Web Settings Tab
         if (txtProjectDownloadUrl.BackColor == Color.Yellow ||
             txtProxyServer.BackColor == Color.Yellow ||
             txtProxyPort.BackColor == Color.Yellow ||
             txtProxyUser.BackColor == Color.Yellow ||
             txtProxyPass.BackColor == Color.Yellow)
         {
            return true;
         }
         
         return false;
      }

      private void GetDataScheduledTasksTab()
      {
         Prefs.GenerateInterval = Int32.Parse(txtWebGenMinutes.Text);
         Prefs.GenerateWeb = chkWebSiteGenerator.Checked;
         if (radioFullRefresh.Checked)
         {
            Prefs.WebGenAfterRefresh = true;
         }
         else
         {
            Prefs.WebGenAfterRefresh = false;
         }
         Prefs.SyncOnLoad = chkSynchronous.Checked;
         Prefs.SyncOnSchedule = chkScheduled.Checked;
         Prefs.OfflineLast = chkOffline.Checked;
         Prefs.ShowUserStats = chkShowUserStats.Checked;
         Prefs.DuplicateUserIDCheck = chkDuplicateUserID.Checked;
         Prefs.DuplicateProjectCheck = chkDuplicateProject.Checked;
         Prefs.ColorLogFile = chkColorLog.Checked;
         Prefs.PpdCalculation = (ePpdCalculation)cboPpdCalc.SelectedItem;
         Prefs.SyncTimeMinutes = Int32.Parse(txtCollectMinutes.Text);
         Prefs.WebRoot = txtWebSiteBase.Text;
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
      }

      private void GetDataDefaultsTab()
      {
         Prefs.DefaultConfigFile = txtDefaultConfigFile.Text;
         if (String.IsNullOrEmpty(Prefs.DefaultConfigFile))
         {
            Prefs.UseDefaultConfigFile = false;
         }
         else
         {
            Prefs.UseDefaultConfigFile = chkDefaultConfig.Checked;
         }
         Prefs.AutoSaveConfig = chkAutoSave.Checked;
         Prefs.LogFileViewer = txtLogFileViewer.Text;
         Prefs.FileExplorer = txtFileExplorer.Text;
         Prefs.MessageLevel = cboMessageLevel.SelectedIndex;
         Prefs.DecimalPlaces = Convert.ToInt32(udDecimalPlaces.Value);
      }
      
      private void GetReportingTab()
      {
         Prefs.EmailReportingToAddress = txtToEmailAddress.Text;
         Prefs.EmailReportingFromAddress = txtFromEmailAddress.Text;
         Prefs.EmailReportingServerAddress = txtSmtpServer.Text;
         Prefs.EmailReportingServerUsername = txtSmtpUsername.Text;
         Prefs.EmailReportingServerPassword = txtSmtpPassword.Text;

         Prefs.EmailReportingEnabled = chkEnableEmail.Checked;
         
         Prefs.ReportEuePause = chkClientEuePause.Checked;
      }

      private void GetDataWebSettingsTab()
      {
         if (txtEOCUserID.Text.Length > 0)
         {
            Prefs.EOCUserID = Int32.Parse(txtEOCUserID.Text);
         }
         else
         {
            Prefs.EOCUserID = 0;
         }
         Prefs.StanfordID = txtStanfordUserID.Text;
         if (txtStanfordTeamID.Text.Length > 0)
         {
            Prefs.TeamID = Int32.Parse(txtStanfordTeamID.Text);
         }
         else
         {
            Prefs.TeamID = 0;
         }
         Prefs.ProjectDownloadUrl = txtProjectDownloadUrl.Text;
         Prefs.UseProxy = chkUseProxy.Checked;
         Prefs.UseProxyAuth = chkUseProxyAuth.Checked;
         Prefs.ProxyServer = txtProxyServer.Text;
         if (txtProxyPort.Text.Length > 0)
         {
            Prefs.ProxyPort = Int32.Parse(txtProxyPort.Text);
         }
         else
         {
            Prefs.ProxyPort = PreferenceSet.ProxyPortDefault;
         }
         Prefs.ProxyUser = txtProxyUser.Text;
         Prefs.ProxyPass = txtProxyPass.Text;
      }

      private void GetDataVisualStylesTab()
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
