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

         Prefs = PreferenceSet.Instance;
      }

      private void frmPreferences_Shown(object sender, EventArgs e)
      {
         LoadScheduledTasksTab();
         LoadDefaultsTab();
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
         chkWebSiteGenerator.Checked = Prefs.GenerateWeb;
         if (Prefs.WebGenAfterRefresh)
         {
            radioFullRefresh.Checked = true;
         }
         else
         {
            radioSchedule.Checked = true;
         }
         if (PreferenceSet.ValidateMinutes(Prefs.GenerateInterval))
         {
            txtWebGenMinutes.Text = Prefs.GenerateInterval.ToString();
         }
         else
         {
            txtWebGenMinutes.Text = PreferenceSet.MinutesDefault.ToString();
         }
         txtWebSiteBase.Text = Prefs.WebRoot;
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

      private void LoadWebTab()
      {
         txtEOCUserID.Text = Prefs.EOCUserID.ToString();
         txtStanfordUserID.Text = Prefs.StanfordID;
         txtStanfordTeamID.Text = Prefs.TeamID.ToString();
         txtProjectDownloadUrl.Text = Prefs.ProjectDownloadUrl;
         chkUseProxy.Checked = Prefs.UseProxy;
         txtProxyServer.Text = Prefs.ProxyServer;
         txtProxyPort.Text = Prefs.ProxyPort.ToString();
         chkUseProxyAuth.Checked = Prefs.UseProxyAuth;
         txtProxyUser.Text = Prefs.ProxyUser;
         txtProxyPass.Text = Prefs.ProxyPass;
      }
      
      private void LoadVisualStyleTab()
      {
         DirectoryInfo di = new DirectoryInfo(Path.Combine(PreferenceSet.AppPath, CssFolder));
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
            radioSchedule.Enabled = true;
            radioSchedule_CheckedChanged(sender, e);
            lbl2MinutesToGen.Enabled = true;
            radioFullRefresh.Enabled = true;
            txtWebSiteBase.Enabled = true;
            txtWebSiteBase.ReadOnly = false;
            btnBrowseWebFolder.Enabled = true;
         }
         else
         {
            radioSchedule.Enabled = false;
            txtWebGenMinutes.Enabled = false;
            txtWebGenMinutes.ReadOnly = true;
            lbl2MinutesToGen.Enabled = false;
            radioFullRefresh.Enabled = false;
            txtWebSiteBase.Enabled = false;
            txtWebSiteBase.ReadOnly = true;
            btnBrowseWebFolder.Enabled = false;
         }
      }

      private void radioSchedule_CheckedChanged(object sender, EventArgs e)
      {
         if (radioSchedule.Checked)
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
         if (txtWebSiteBase.Text != String.Empty)
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

      #region Web Tab
      private void linkEOC_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(String.Concat(PreferenceSet.EOCUserBaseURL, txtEOCUserID.Text));
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
            Process.Start(String.Concat(PreferenceSet.StanfordBaseURL, txtStanfordUserID.Text));
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
            Process.Start(String.Concat(PreferenceSet.EOCTeamBaseURL, txtStanfordTeamID.Text));
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

      private void chkUseProxyAuth_CheckedChanged(object sender, EventArgs e)
      {
         if (chkUseProxyAuth.Checked)
         {
            EnableProxyAuth();
         }
         else
         {
            DisableProxyAuth();
         }
      }

      private void EnableProxy()
      {
         txtProxyServer.Enabled = true;
         txtProxyPort.Enabled = true;
         txtProxyServer.ReadOnly = false;
         txtProxyPort.ReadOnly = false;
         chkUseProxyAuth.Enabled = true;
      }

      private void DisableProxy()
      {
         txtProxyServer.Enabled = false;
         txtProxyPort.Enabled = false;
         txtProxyServer.ReadOnly = true;
         txtProxyPort.ReadOnly = true;
         chkUseProxyAuth.Enabled = false;
         DisableProxyAuth();
      }

      private void EnableProxyAuth()
      {
         txtProxyUser.Enabled = true;
         txtProxyUser.ReadOnly = false;
         txtProxyPass.Enabled = true;
         txtProxyPass.ReadOnly = false;
      }

      private void DisableProxyAuth()
      {
         txtProxyUser.Enabled = false;
         txtProxyUser.ReadOnly = true;
         txtProxyPass.Enabled = false;
         txtProxyPass.ReadOnly = true;
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
         // Check for error conditions
         if (txtCollectMinutes.BackColor == Color.Yellow) return;
         if (txtWebGenMinutes.BackColor == Color.Yellow) return;
         if (txtWebSiteBase.BackColor == Color.Yellow) return;
         if (txtProjectDownloadUrl.BackColor == Color.Yellow) return;

         GetDataScheduledTasksTab();
         GetDataDefaultsTab();
         GetDataWebSettingsTab();
         GetDataVisualStylesTab();

         Prefs.Save();

         DialogResult = System.Windows.Forms.DialogResult.OK;
         Close();
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
