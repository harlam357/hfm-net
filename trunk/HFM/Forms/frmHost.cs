/*
 * HFM.NET - Host Configuration Form
 * Copyright (C) 2006 David Rawling
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
using System.Text.RegularExpressions;
using System.Windows.Forms;

using Debug = HFM.Instrumentation.Debug;

namespace HFM.Forms
{
   public partial class frmHost : Form
   {
      #region Constructor
      /// <summary>
      /// Class constructor
      /// </summary>
      public frmHost()
      {
         InitializeComponent();
      } 
      #endregion

      #region Radio button management
      /// <summary>
      /// Enable the HTTP controls
      /// </summary>
      /// <param name="bState"></param>
      private void HTTPFieldsActive(Boolean bState)
      {
         txtWebURL.ReadOnly = !bState;
         txtWebUser.ReadOnly = !bState;
         txtWebPass.ReadOnly = !bState;
         txtWebURL.Enabled = bState;
         txtWebUser.Enabled = bState;
         txtWebPass.Enabled = bState;
      }

      /// <summary>
      /// Enable/disable the FTP controls
      /// </summary>
      /// <param name="bState"></param>
      private void FTPFieldsActive(Boolean bState)
      {
         txtFTPServer.ReadOnly = !bState;
         txtFTPPath.ReadOnly = !bState;
         txtFTPUser.ReadOnly = !bState;
         txtFTPPass.ReadOnly = !bState;
         txtFTPServer.Enabled = bState;
         txtFTPPath.Enabled = bState;
         txtFTPUser.Enabled = bState;
         txtFTPPass.Enabled = bState;
      }

      /// <summary>
      /// Enable/disable the local path controls
      /// </summary>
      /// <param name="bState"></param>
      private void PathFieldsActive(Boolean bState)
      {
         txtLocalPath.ReadOnly = !bState;
         txtLocalPath.Enabled = bState;
         btnBrowseLocal.Enabled = bState;
      }

      /// <summary>
      /// Clears validation colours from all text fields (on button change)
      /// </summary>
      private void ClearValidate()
      {
         txtName.BackColor = SystemColors.Window;
         txtClientMegahertz.BackColor = SystemColors.Window;
         txtLocalPath.BackColor = SystemColors.Window;
         txtFTPServer.BackColor = SystemColors.Window;
         txtFTPPath.BackColor = SystemColors.Window;
         txtFTPUser.BackColor = SystemColors.Window;
         txtFTPPass.BackColor = SystemColors.Window;
         txtWebURL.BackColor = SystemColors.Window;
         txtWebUser.BackColor = SystemColors.Window;
         txtWebPass.BackColor = SystemColors.Window;
      }

      /// <summary>
      /// Configure the form fields according to the selected radio button
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void radioButtonSet_CheckedChanged(object sender, EventArgs e)
      {
         ClearValidate();
         if (radioLocal.Checked)
         {
            PathFieldsActive(true);
            FTPFieldsActive(false);
            HTTPFieldsActive(false);
         }
         else if (radioFTP.Checked)
         {
            PathFieldsActive(false);
            FTPFieldsActive(true);
            HTTPFieldsActive(false);
         }
         else if (radioHTTP.Checked)
         {
            PathFieldsActive(false);
            FTPFieldsActive(false);
            HTTPFieldsActive(true);
         }
      }
      #endregion

      #region Local Path Browse functions
      /// <summary>
      /// Display the folder selection dialog. We want a path.
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void btnBrowseLocal_Click(object sender, EventArgs e)
      {
         if (txtLocalPath.Text.Length > 0)
         {
            openLogFolder.SelectedPath = txtLocalPath.Text;
         }
         
         openLogFolder.ShowDialog();
         
         if (openLogFolder.SelectedPath.Length > 0)
         {
            txtLocalPath.Text = openLogFolder.SelectedPath;
         }
      }
      #endregion

      #region Text field validators
      /// <summary>
      /// Validate the contents of the Instance name textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtName_Validating(object sender, CancelEventArgs e)
      {
         Regex rValidName = new Regex("^[a-zA-Z0-9\\-_\\+=\\$&^\\[\\]][a-zA-Z0-9 \\.\\-_\\+=\\$&^\\[\\]]+$", RegexOptions.Singleline);
         Match mValidName = rValidName.Match(txtName.Text);
         if (txtName.Text.Length > 0 && mValidName.Success == false)
         {
            //e.Cancel = true;
            //txtName.Focus();
            txtName.BackColor = Color.Yellow;
            txtName.Focus();
            ShowToolTip("Instance name can contain only\r\nletters, numbers and basic symbols\r\n(+=-_~$@^&.,[]), must not end with\r\ndot (.) and cannot start with space.", 
               txtName, 5000);
         }
         else
         {
            txtName.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtName);
         }
      }

      /// <summary>
      /// Validate the contents of the Client Megahertz textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtClientMegahertz_Validating(object sender, CancelEventArgs e)
      {
         int mhz;
         if (int.TryParse(txtClientMegahertz.Text, out mhz) == false)
         {
            txtClientMegahertz.BackColor = Color.Yellow;
            txtClientMegahertz.Focus();
            ShowToolTip("Client Processor Megahertz must be numeric.",
               txtClientMegahertz, 5000);
         }
         else if (mhz < 1)
         {
            txtClientMegahertz.BackColor = Color.Yellow;
            txtClientMegahertz.Focus();
            ShowToolTip("Client Processor Megahertz must be greater than zero.",
               txtClientMegahertz, 5000);
         }
         else
         {
            txtClientMegahertz.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtClientMegahertz);
         }
      }

      /// <summary>
      /// Validate the contents of the Local File Path textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtLocalPath_Validating(object sender, CancelEventArgs e)
      {
         if (radioLocal.Checked == false) return;
         
         if (txtLocalPath.Text.ToUpper().EndsWith("FAHLOG.TXT"))
         {
            txtLocalPath.Text = txtLocalPath.Text.Substring(0, txtLocalPath.Text.Length - 10);
         }
         if (txtLocalPath.Text.ToUpper().EndsWith("UNITINFO.TXT"))
         {
            txtLocalPath.Text = txtLocalPath.Text.Substring(0, txtLocalPath.Text.Length - 12);
         }

         Regex rValidPath = new Regex("^((?<DRIVE>[a-z]:)|(\\\\\\\\(?<SERVER>[0-9]*[a-z\\-][a-z0-9\\-]*)\\\\(?<VOLUME>[^\\.\\x01-\\x1F\\\\\"\"\\*\\?<>:|\\\\/][^\\x01-\\x1F\\\\\"\"\\*\\?|><:\\\\/]*)))?(?<FOLDERS>(?<FOLDER1>(\\.|(\\.\\.)|([^\\.\\x01-\\x1F\\\\\"\"\\*\\?|><:\\\\/][^\\x01-\\x1F\\\\\"\"\\*\\?<>:|\\\\/]*)))?(?<FOLDERm>[\\\\/](\\.|(\\.\\.)|([^\\.\\x01-\\x1F\\\\\"\"\\*\\?|><:\\\\/][^\\x01-\\x1F\\\\\"\"\\*\\?<>:|\\\\/]*)))*)?[\\\\/]?$", RegexOptions.Singleline | RegexOptions.IgnoreCase);
         Match mPath = rValidPath.Match(txtLocalPath.Text);
         Match mPath2 = rValidPath.Match(txtLocalPath.Text + "\\");

         if (txtLocalPath.Text.Length == 0)
         {
            //e.Cancel = true;
            //txtLocalPath.Focus();
            txtLocalPath.BackColor = Color.Yellow;
            txtLocalPath.Focus();
            ShowToolTip("Log Folder must be a valid local\r\nor network (UNC) path.", txtLocalPath, 5000);
         }
         else if (txtLocalPath.Text.Length > 3 && (mPath.Success || mPath2.Success) == false)
         {
            //e.Cancel = true;
            //txtLocalPath.Focus();
            txtLocalPath.BackColor = Color.Yellow;
            txtLocalPath.Focus();
            ShowToolTip("Log Folder must be a valid local\r\nor network (UNC) path.", txtLocalPath, 5000);
         }
         else
         {
            if (mPath2.Success)
            {
               txtLocalPath.Text += "\\";
            }
            txtLocalPath.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtLocalPath);
         }
      }

      /// <summary>
      /// Validate the contents of the FTP Server textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtFTPServer_Validating(object sender, CancelEventArgs e)
      {
         if (radioFTP.Checked == false) return;

         Regex rServer = new Regex("^([a-z0-9]([-a-z0-9]*[a-z0-9])?\\.)+((a[cdefgilmnoqrstuwxz]|aero|arpa)|(b[abdefghijmnorstvwyz]|biz)|(c[acdfghiklmnorsuvxyz]|cat|com|coop)|d[ejkmoz]|(e[ceghrstu]|edu)|f[ijkmor]|(g[abdefghilmnpqrstuwy]|gov)|h[kmnrtu]|(i[delmnoqrst]|info|int)|(j[emop]|jobs)|k[eghimnprwyz]|l[abcikrstuvy]|(m[acdghklmnopqrstuvwxyz]|mil|mobi|museum)|(n[acefgilopruz]|name|net)|(om|org)|(p[aefghklmnrstwy]|pro)|qa|r[eouw]|s[abcdeghijklmnortvyz]|(t[cdfghjklmnoprtvwz]|travel)|u[agkmsyz]|v[aceginu]|w[fs]|y[etu]|z[amw])$", RegexOptions.Singleline);
         Regex rIP = new Regex("(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)", RegexOptions.Singleline);
         Match mServer = rServer.Match(txtFTPServer.Text);
         Match mIP = rIP.Match(txtFTPServer.Text);

         if (txtFTPServer.Text.Length == 0)
         {
            //e.Cancel = true;
            //txtLocalPath.Focus();
            txtFTPServer.BackColor = Color.Yellow;
            txtFTPServer.Focus();
            ShowToolTip("FTP server must be a valid\r\nhost name or IP address.", txtFTPServer, 5000);
         }
         else if (txtFTPServer.Text.Length > 0 && (mServer.Success || mIP.Success) == false)
         {
            //e.Cancel = true;
            //txtFTPServer.Focus();
            txtFTPServer.BackColor = Color.Yellow;
            txtFTPServer.Focus();
            ShowToolTip("FTP server must be a valid\r\nhost name or IP address.", txtFTPServer, 5000);
         }
         else
         {
            txtFTPServer.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPServer);
         }
      }

      /// <summary>
      /// Validate the contents of the FTP Path textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtFTPPath_Validating(object sender, CancelEventArgs e)
      {
         if (radioFTP.Checked == false) return;

         if (txtFTPPath.Text.ToUpper().EndsWith("FAHLOG.TXT"))
         {
            txtFTPPath.Text = txtFTPPath.Text.Substring(0, txtFTPPath.Text.Length - 10);
         }
         if (txtFTPPath.Text.ToUpper().EndsWith("UNITINFO.TXT"))
         {
            txtFTPPath.Text = txtFTPPath.Text.Substring(0, txtFTPPath.Text.Length - 12);
         }
         if (txtFTPPath.Text.EndsWith("/") == false)
         {
            txtFTPPath.Text += "/";
         }

         Regex sPath = new Regex("^/.*/$", RegexOptions.Singleline);
         Match mPath = sPath.Match(txtFTPPath.Text);

         if (txtFTPPath.Text.Length > 0 && mPath.Success == false)
         {
            //e.Cancel = true;
            //txtFTPPath.Focus();
            txtFTPPath.BackColor = Color.Yellow;
            txtFTPPath.Focus();
            ShowToolTip("FTP path should be the full\r\npath to the folder that\r\ncontains the log and Unit Info\r\nfiles (including the trailing /).", txtFTPPath, 5000);
         }
         else
         {
            txtFTPPath.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPPath);
         }
      }

      /// <summary>
      /// Validate the contents of the Web URL textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtWebURL_Validating(object sender, CancelEventArgs e)
      {
         if (radioHTTP.Checked == false) return;

         if (txtWebURL.Text.ToUpper().EndsWith("FAHLOG.TXT"))
         {
            txtWebURL.Text = txtWebURL.Text.Substring(0, txtWebURL.Text.Length - 10);
         }
         if (txtWebURL.Text.ToUpper().EndsWith("UNITINFO.TXT"))
         {
            txtWebURL.Text = txtWebURL.Text.Substring(0, txtWebURL.Text.Length - 12);
         }

         Regex rURLDNS = new Regex("^http([s])?://^([a-z0-9]([-a-z0-9]*[a-z0-9])?\\.)+((a[cdefgilmnoqrstuwxz]|aero|arpa)|(b[abdefghijmnorstvwyz]|biz)|(c[acdfghiklmnorsuvxyz]|cat|com|coop)|d[ejkmoz]|(e[ceghrstu]|edu)|f[ijkmor]|(g[abdefghilmnpqrstuwy]|gov)|h[kmnrtu]|(i[delmnoqrst]|info|int)|(j[emop]|jobs)|k[eghimnprwyz]|l[abcikrstuvy]|(m[acdghklmnopqrstuvwxyz]|mil|mobi|museum)|(n[acefgilopruz]|name|net)|(om|org)|(p[aefghklmnrstwy]|pro)|qa|r[eouw]|s[abcdeghijklmnortvyz]|(t[cdfghjklmnoprtvwz]|travel)|u[agkmsyz]|v[aceginu]|w[fs]|y[etu]|z[amw])(/([A-Za-z0-9\\-\\.\\,\\[\\]\\{\\}\\!\\@\\#\\$\\%\\^\\&\\(\\)])*)*/$", RegexOptions.Singleline);
         Regex rURLIP = new Regex("^http([s])?://(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)(/([A-Za-z0-9\\-\\.\\,\\[\\]\\{\\}\\!\\@\\#\\$\\%\\^\\&\\(\\)])*)*/$", RegexOptions.Singleline);
         Match mURLDNS = rURLDNS.Match(txtWebURL.Text);
         Match mURLIP = rURLIP.Match(txtWebURL.Text);
         
         if (txtWebURL.Text.Length == 0)
         {
            //e.Cancel = true;
            //txtWebURL.Focus();
            txtWebURL.BackColor = Color.Yellow;
            txtWebURL.Focus();
            ShowToolTip("URL must be a valid URL and be\r\nthe path to the folder containing\r\nunitinfo.txt and fahlog.txt.", txtWebURL, 5000);
         }
         else if (txtWebURL.Text.Length > 0 && (mURLDNS.Success || mURLIP.Success) == false)
         {
            //e.Cancel = true;
            //txtWebURL.Focus();
            txtWebURL.BackColor = Color.Yellow;
            txtWebURL.Focus();
            ShowToolTip("URL must be a valid URL and be\r\nthe path to the folder containing\r\nunitinfo.txt and fahlog.txt.", txtWebURL, 5000);
         }
         else
         {
            txtWebURL.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebURL);
         }
      }

      /// <summary>
      /// Delegate method to ensure tooltip is displayed by UI thread
      /// </summary>
      delegate void showTooltipCallback(String sMessage, Control cTarget, Int32 Delay);

      /// <summary>
      /// Show the appropriate tooltip balloon/box
      /// </summary>
      /// <param name="sMessage">Tip to be displayed</param>
      /// <param name="cTarget">Control to point to with the tip</param>
      /// <param name="atX">Screen location for tip (X)</param>
      /// <param name="atY">Screen location for tip (Y)</param>
      /// <param name="Delay">Time to show tip (milliseconds)</param>
      private void ShowToolTip(String sMessage, Control cTarget, Int32 Delay)
      {
         try
         {
            if (cTarget.InvokeRequired)
            {
               showTooltipCallback tFunc = ShowToolTip;
               Invoke(tFunc, new object[] { sMessage, cTarget, Delay });
            }
            else
            {
               toolTipCore.Show(sMessage, cTarget, cTarget.Width, 0, Delay);
            }
         }
         catch (InvalidOperationException ex)
         {
            Debug.WriteToHfmConsole(TraceLevel.Warning, String.Format("{0} threw InvalidOp exception {1}.", Debug.FunctionName, ex.Message));
         }
      }
      #endregion

      #region Button Event Handlers
      private void btnOK_Click(object sender, EventArgs e)
      {
         // Check for error condition on Name
         if (txtName.BackColor == Color.Yellow) return;

         if (txtName.Text.Length == 0)
         {
            txtName.BackColor = Color.Yellow;
            txtName.Focus();
            ShowToolTip("Instance Name is required.", txtName, 5000);
            return;
         }

         if (txtClientMegahertz.BackColor == Color.Yellow) return;

         if (radioLocal.Checked)
         {
            // Check for error condition on Path
            if (txtLocalPath.BackColor == Color.Yellow) return;

            if (txtLocalPath.Text.Length == 0)
            {
               txtLocalPath.BackColor = Color.Yellow;
               txtLocalPath.Focus();
               return;
            }
         }

         if (radioFTP.Checked)
         {
            // Check for error condition on Server and Path
            if (txtFTPServer.BackColor == Color.Yellow) return;
            if (txtFTPPath.BackColor == Color.Yellow) return;

            if (txtFTPServer.Text.Length == 0)
            {
               txtFTPServer.BackColor = Color.Yellow;
               txtFTPServer.Focus();
               return;
            }

            if (txtFTPPath.Text.Length == 0)
            {
               txtFTPPath.BackColor = Color.Yellow;
               txtFTPPath.Focus();
               return;
            }

            // Validate that the FTP user and password are specified (both are required)
            txtFTPUser.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPUser);
            txtFTPPass.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPPass);

            if (txtFTPUser.Text.Length == 0)
            {
               //e.Cancel = true;
               //txtFTPUser.Focus();
               txtFTPUser.BackColor = Color.Yellow;
               txtFTPUser.Focus();
               ShowToolTip("Username must be specified.", txtFTPUser, 5000);
               return;
            }
            else if (txtFTPPass.Text.Length == 0)
            {
               //e.Cancel = true;
               //txtFTPPass.Focus();
               txtFTPPass.BackColor = Color.Yellow;
               txtFTPPass.Focus();
               ShowToolTip("Password must be specified.", txtFTPPass, 5000);
               return;
            }
         }
         else if (radioHTTP.Checked)
         {
            // Check for error condition on Name and Path
            if (txtWebURL.BackColor == Color.Yellow) return;

            if (txtWebURL.Text.Length == 0)
            {
               txtWebURL.BackColor = Color.Yellow;
               txtWebURL.Focus();
               return;
            }

            // Validate that the HTTP user and password are specified (both are required)
            txtWebUser.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebUser);
            txtWebPass.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebPass);

            if ((txtWebUser.Text.Length < 1) && (txtWebPass.Text.Length > 0))
            {
               //e.Cancel = true;
               //txtWebUser.Focus();
               txtWebUser.BackColor = Color.Yellow;
               txtWebUser.Focus();
               ShowToolTip("Username must be specified if password is set.", txtWebUser, 5000);
               return;
            }
            else if ((txtWebUser.Text.Length > 0) && (txtWebPass.Text.Length < 1))
            {
               //e.Cancel = true;
               txtWebPass.BackColor = Color.Yellow;
               txtWebPass.Focus();
               ShowToolTip("Username must be specified if password is set.", txtWebPass, 5000);
               return;
            }
         }

         DialogResult = DialogResult.OK;
         Close();
      } 
      #endregion
   }
}