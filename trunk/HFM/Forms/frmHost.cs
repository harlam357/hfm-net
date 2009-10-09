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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using HFM.Helpers;
using HFM.Instances;

namespace HFM.Forms
{
   public enum HostAction
   {
      Add,
      Edit
   }

   public partial class frmHost : Classes.FormWrapper
   {
      private readonly int MaxHeight;
      private readonly int MaxWidth;
      
      private readonly InstanceCollection _ClientInstances;
      private readonly ClientInstance _Instance;
      private readonly HostAction _Action;
   
      #region Constructor
      public frmHost(InstanceCollection ClientInstances) 
         : this(ClientInstances, null, HostAction.Add)
      { }

      public frmHost(InstanceCollection ClientInstances, ClientInstance Instance)
         : this(ClientInstances, Instance, HostAction.Edit)
      { }

      /// <summary>
      /// Class constructor
      /// </summary>
      private frmHost(InstanceCollection ClientInstances, ClientInstance Instance, HostAction Action)
      {
         _ClientInstances = ClientInstances;
         _Instance = Instance;
         _Action = Action;
      
         InitializeComponent();
         
         MaxHeight = Height;
         MaxWidth = Width;

         txtLogFileName.Text = ClientInstance.LocalFAHLog;
         txtUnitFileName.Text = ClientInstance.LocalUnitInfo;
         txtQueueFileName.Text = ClientInstance.LocalQueue;
         
         if (Instance != null)
         {
            SetInstanceData(Instance);
         }
         else
         {
            SetInstanceType(InstanceType.PathInstance);
         }
      }
      #endregion
      
      private void SetInstanceData(ClientInstance Instance)
      {
         if (Instance == null) throw new ArgumentNullException("Instance", "Argument 'Instance' cannot be null.");
      
         SetInstanceType(Instance.InstanceHostType);
         SetBasicClientInfo(Instance);

         switch (Instance.InstanceHostType)
         {
            case InstanceType.PathInstance:
               SetPathClientInfo(Instance);
               break;
            case InstanceType.FTPInstance:
               SetFTPClientInfo(Instance);
               break;
            case InstanceType.HTTPInstance:
               SetHTTPClientInfo(Instance);
               break;
            default:
               throw new NotImplementedException(String.Format(CultureInfo.CurrentUICulture,
                  "Instance type '{0}' has not been implemented.", Instance.InstanceHostType));
         }
      }
      
      private void SetInstanceType(InstanceType type)
      {
         switch (type)
         {
            case InstanceType.PathInstance:
               radioLocal.Checked = true;
               break;
            case InstanceType.FTPInstance:
               radioFTP.Checked = true;
               break;
            case InstanceType.HTTPInstance:
               radioHTTP.Checked = true;
               break;
            default: 
               throw new NotImplementedException(String.Format(CultureInfo.CurrentUICulture,
                  "Instance type '{0}' has not been implemented.", type));
         }
      }
      
      private void SetBasicClientInfo(ClientInstance Instance)
      {
         txtName.Text = Instance.InstanceName;
         txtLogFileName.Text = Instance.RemoteFAHLogFilename;
         txtUnitFileName.Text = Instance.RemoteUnitInfoFilename;
         txtQueueFileName.Text = Instance.RemoteQueueFilename;
         txtClientMegahertz.Text = Instance.ClientProcessorMegahertz.ToString();
         chkClientVM.Checked = Instance.ClientIsOnVirtualMachine;
         numOffset.Value = Instance.ClientTimeOffset;
      }
      
      private void SetPathClientInfo(ClientInstance Instance)
      {
         txtLocalPath.Text = Instance.Path;
      }
      
      private void SetFTPClientInfo(ClientInstance Instance)
      {
         txtFTPPath.Text = Instance.Path;
         txtFTPServer.Text = Instance.Server;
         txtFTPUser.Text = Instance.Username;
         txtFTPPass.Text = Instance.Password;
      }
      
      private void SetHTTPClientInfo(ClientInstance Instance)
      {
         txtWebURL.Text = Instance.Path;
         txtWebUser.Text = Instance.Username;
         txtWebPass.Text = Instance.Password;
      }

      private ClientInstance GetInstanceData()
      {
         ClientInstance NewInstance = new ClientInstance(GetInstanceType());
         GetBasicClientInfo(NewInstance);

         switch (NewInstance.InstanceHostType)
         {
            case InstanceType.PathInstance:
               GetPathClientInfo(NewInstance);
               break;
            case InstanceType.FTPInstance:
               GetFTPClientInfo(NewInstance);
               break;
            case InstanceType.HTTPInstance:
               GetHTTPClientInfo(NewInstance);
               break;
            default:
               throw new NotImplementedException(String.Format(CultureInfo.CurrentUICulture,
                  "Instance type '{0}' has not been implemented.", NewInstance.InstanceHostType));
         }
         
         return NewInstance;
      }
      
      private InstanceType GetInstanceType()
      {
         if (radioLocal.Checked) return InstanceType.PathInstance;
         if (radioFTP.Checked) return InstanceType.FTPInstance;
         if (radioHTTP.Checked) return InstanceType.HTTPInstance;
      
         throw new InvalidOperationException("No Instance type could be determined.");
      }

      /// <summary>
      /// Gets basic client info and sets in the given ClientInstance
      /// </summary>
      /// <param name="Instance">ClientInstance</param>
      private void GetBasicClientInfo(ClientInstance Instance)
      {
         Instance.InstanceName = txtName.Text;
         Instance.RemoteFAHLogFilename = txtLogFileName.Text;
         Instance.RemoteUnitInfoFilename = txtUnitFileName.Text;
         Instance.RemoteQueueFilename = txtQueueFileName.Text;
         int mhz;
         if (int.TryParse(txtClientMegahertz.Text, out mhz))
         {
            Instance.ClientProcessorMegahertz = mhz;
         }
         else
         {
            Instance.ClientProcessorMegahertz = 1;
         }
         Instance.ClientIsOnVirtualMachine = chkClientVM.Checked;
         Instance.ClientTimeOffset = Convert.ToInt32(numOffset.Value);
      }

      private void GetPathClientInfo(ClientInstance Instance)
      {
         Instance.Path = txtLocalPath.Text;
      }

      private void GetFTPClientInfo(ClientInstance Instance)
      {
         Instance.Path = txtFTPPath.Text;
         Instance.Server = txtFTPServer.Text;
         Instance.Username = txtFTPUser.Text;
         Instance.Password = txtFTPPass.Text;
      }

      private void GetHTTPClientInfo(ClientInstance Instance)
      {
         Instance.Path = txtWebURL.Text;
         Instance.Username = txtWebUser.Text;
         Instance.Password = txtWebPass.Text;
      }

      #region Radio button management
      /// <summary>
      /// Enable the HTTP controls
      /// </summary>
      /// <param name="bState"></param>
      private void HTTPFieldsActive(Boolean bState)
      {
         if (bState)
         {
            Size = new Size(MaxWidth, MaxHeight - 27);
         }
         grpHTTP.Visible = bState;
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
         if (bState)
         {
            Size = new Size(MaxWidth, MaxHeight);
         }
         grpFTP.Visible = bState;
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
         if (bState)
         {
            Size = new Size(MaxWidth, MaxHeight - 57);
         }
         grpLocal.Visible = bState;
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
         txtLogFileName.BackColor = SystemColors.Window;
         txtUnitFileName.BackColor = SystemColors.Window;
         txtQueueFileName.BackColor = SystemColors.Window;
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
            if (txtLocalPath.Text.EndsWith(Path.DirectorySeparatorChar.ToString()) == false)
            {
               txtLocalPath.Text += Path.DirectorySeparatorChar;
            }
            txtLocalPath_Validating(sender, null);
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
         if (txtName.Text.Length > 0 && StringOps.ValidateInstanceName(txtName.Text) == false)
         {
            txtName.BackColor = Color.Yellow;
            txtName.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostNameToolTip, Environment.NewLine), txtName, 5000);
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
            toolTipCore.Show(Properties.Resources.HostProcessorMhzNumeric, txtClientMegahertz, 5000);
         }
         else if (mhz < 1)
         {
            txtClientMegahertz.BackColor = Color.Yellow;
            txtClientMegahertz.Focus();
            toolTipCore.Show(Properties.Resources.HostProcessorMhzGreaterThanZero, txtClientMegahertz, 5000);
         }
         else
         {
            txtClientMegahertz.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtClientMegahertz);
         }
      }

      /// <summary>
      /// Validate the contents of the FAHlog textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtLogFileName_Validating(object sender, CancelEventArgs e)
      {
         if (txtLogFileName.Text.Length > 0 && StringOps.ValidateFileName(txtLogFileName.Text) == false)
         {
            txtLogFileName.BackColor = Color.Yellow;
            txtLogFileName.Focus();
            toolTipCore.Show(Properties.Resources.HostFileNameInvalidChars, txtLogFileName, 5000);
         }
         else
         {
            txtLogFileName.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtLogFileName);
         }
      }

      /// <summary>
      /// Validate the contents of the unitinfo textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtUnitFileName_Validating(object sender, CancelEventArgs e)
      {
         if (txtUnitFileName.Text.Length > 0 && StringOps.ValidateFileName(txtUnitFileName.Text) == false)
         {
            txtUnitFileName.BackColor = Color.Yellow;
            txtUnitFileName.Focus();
            toolTipCore.Show(Properties.Resources.HostFileNameInvalidChars, txtUnitFileName, 5000);
         }
         else
         {
            txtUnitFileName.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtUnitFileName);
         }
      }

      /// <summary>
      /// Validate the contents of the queue textbox
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void txtQueueFileName_Validating(object sender, CancelEventArgs e)
      {
         if (txtQueueFileName.Text.Length > 0 && StringOps.ValidateFileName(txtQueueFileName.Text) == false)
         {
            txtQueueFileName.BackColor = Color.Yellow;
            txtQueueFileName.Focus();
            toolTipCore.Show(Properties.Resources.HostFileNameInvalidChars, txtQueueFileName, 5000);
         }
         else
         {
            txtQueueFileName.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtQueueFileName);
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
         if (txtLocalPath.Text.ToUpper().EndsWith("QUEUE.DAT"))
         {
            txtLocalPath.Text = txtLocalPath.Text.Substring(0, txtLocalPath.Text.Length - 9);
         }

         bool bPath = StringOps.ValidatePathInstancePath(txtLocalPath.Text);
         bool bPathWithSlash = StringOps.ValidatePathInstancePath(String.Concat(txtLocalPath.Text, Path.DirectorySeparatorChar));

         if (txtLocalPath.Text.Length == 0)
         {
            txtLocalPath.BackColor = Color.Yellow;
            //txtLocalPath.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostLocalPathInvalidTooltip, Environment.NewLine), txtLocalPath, 5000);
         }
         else if (txtLocalPath.Text.Length > 2 && (bPath || bPathWithSlash) != true)
         {
            txtLocalPath.BackColor = Color.Yellow;
            //txtLocalPath.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostLocalPathInvalidTooltip, Environment.NewLine), txtLocalPath, 5000);
         }
         else
         {
            if (bPath == false && bPathWithSlash)
            {
               txtLocalPath.Text += Path.DirectorySeparatorChar;
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

         if (txtFTPServer.Text.Length == 0)
         {
            txtFTPServer.BackColor = Color.Yellow;
            //txtFTPServer.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostFtpServerInvalidTooltip, Environment.NewLine), txtFTPServer, 5000);
         }
         else if (txtFTPServer.Text.Length > 0 && StringOps.ValidateServerName(txtFTPServer.Text) == false)
         {
            txtFTPServer.BackColor = Color.Yellow;
            //txtFTPServer.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostFtpServerInvalidTooltip, Environment.NewLine), txtFTPServer, 5000);
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
         if (txtFTPPath.Text.ToUpper().EndsWith("QUEUE.DAT"))
         {
            txtFTPPath.Text = txtFTPPath.Text.Substring(0, txtFTPPath.Text.Length - 9);
         }
         if (txtFTPPath.Text.EndsWith("/") == false)
         {
            txtFTPPath.Text += "/";
         }

         if (txtFTPPath.Text == "/") // Root path, don't validate against Regex
         {
            txtFTPPath.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPPath);
         }
         else if (txtFTPPath.Text.Length > 0 && StringOps.ValidateFtpPath(txtFTPPath.Text) == false)
         {
            txtFTPPath.BackColor = Color.Yellow;
            //txtFTPPath.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostFtpPathInvalidTooltip, Environment.NewLine), txtFTPPath, 5000);
         }
         else
         {
            txtFTPPath.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPPath);
         }
      }

      ///// <summary>
      ///// Validate the contents of the FTP User textbox
      ///// </summary>
      ///// <param name="sender"></param>
      ///// <param name="e"></param>
      //private void txtFTPUser_Validating(object sender, CancelEventArgs e)
      //{
      //   if (txtFTPUser.Text.Length > 0)
      //   {
      //      txtFTPUser.BackColor = SystemColors.Window;
      //      toolTipCore.Hide(txtFTPUser);
      //   }
      //}

      ///// <summary>
      ///// Validate the contents of the FTP Password textbox
      ///// </summary>
      ///// <param name="sender"></param>
      ///// <param name="e"></param>
      //private void txtFTPPass_Validating(object sender, CancelEventArgs e)
      //{
      //   if (txtFTPPass.Text.Length > 0)
      //   {
      //      txtFTPPass.BackColor = SystemColors.Window;
      //      toolTipCore.Hide(txtFTPPass);
      //   }
      //}

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
         if (txtWebURL.Text.ToUpper().EndsWith("QUEUE.DAT"))
         {
            txtWebURL.Text = txtWebURL.Text.Substring(0, txtWebURL.Text.Length - 9);
         }
         if (txtWebURL.Text.EndsWith("/") == false)
         {
            txtWebURL.Text += "/";
         }

         if (txtWebURL.Text.Length == 0)
         {
            txtWebURL.BackColor = Color.Yellow;
            //txtWebURL.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostHttpUrlInvalidTooltip, Environment.NewLine), txtWebURL, 5000);
         }
         else if (txtWebURL.Text.Length > 0 && StringOps.ValidateHttpURL(txtWebURL.Text) == false)
         {
            txtWebURL.BackColor = Color.Yellow;
            //txtWebURL.Focus();
            toolTipCore.Show(String.Format(Properties.Resources.HostHttpUrlInvalidTooltip, Environment.NewLine), txtWebURL, 5000);
         }
         else
         {
            txtWebURL.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebURL);
         }
      }
      #endregion

      #region Button Event Handlers
      private void btnOK_Click(object sender, EventArgs e)
      {
         if (ValidateAcceptance())
         {
            UpdateClientData();
         
            DialogResult = DialogResult.OK;
            Close();
         }
      }
      
      private bool ValidateAcceptance()
      {
         if (txtName.Text.Length == 0)
         {
            txtName.BackColor = Color.Yellow;
            txtName.Focus();
            toolTipCore.Show("Instance Name is required.", txtName, 5000);
            return false;
         }

         if (txtLogFileName.Text.Length == 0)
         {
            txtLogFileName.BackColor = Color.Yellow;
            txtLogFileName.Focus();
            toolTipCore.Show("FAHlog.txt File Name is required.", txtLogFileName, 5000);
            return false;
         }

         if (txtUnitFileName.Text.Length == 0)
         {
            txtUnitFileName.BackColor = Color.Yellow;
            txtUnitFileName.Focus();
            toolTipCore.Show("unitinfo.txt File Name is required.", txtUnitFileName, 5000);
            return false;
         }

         if (txtQueueFileName.Text.Length == 0)
         {
            txtQueueFileName.BackColor = Color.Yellow;
            txtQueueFileName.Focus();
            toolTipCore.Show("queue.dat File Name is required.", txtQueueFileName, 5000);
            return false;
         }

         if (radioLocal.Checked)
         {
            if (txtLocalPath.Text.Length == 0)
            {
               txtLocalPath.BackColor = Color.Yellow;
               txtLocalPath.Focus();
               return false;
            }
         }

         else if (radioFTP.Checked)
         {
            if (txtFTPServer.Text.Length == 0)
            {
               txtFTPServer.BackColor = Color.Yellow;
               txtFTPServer.Focus();
               return false;
            }

            if (txtFTPPath.Text.Length == 0)
            {
               txtFTPPath.BackColor = Color.Yellow;
               txtFTPPath.Focus();
               return false;
            }

            // Validate that the FTP user and password are specified (both are required)
            txtFTPUser.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPUser);
            txtFTPPass.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtFTPPass);

            if ((txtFTPUser.Text.Length < 1) && (txtFTPPass.Text.Length > 0))
            {
               txtFTPUser.BackColor = Color.Yellow;
               txtFTPUser.Focus();
               toolTipCore.Show("Username must be specified if password is set.", txtFTPUser, 5000);
               return false;
            }
            else if ((txtFTPUser.Text.Length > 0) && (txtFTPPass.Text.Length < 1))
            {
               txtFTPPass.BackColor = Color.Yellow;
               txtFTPPass.Focus();
               toolTipCore.Show("Password must be specified if username is set.", txtFTPPass, 5000);
               return false;
            }
         }
         else if (radioHTTP.Checked)
         {
            if (txtWebURL.Text.Length == 0)
            {
               txtWebURL.BackColor = Color.Yellow;
               txtWebURL.Focus();
               return false;
            }

            // Validate that the HTTP user and password are specified (both are required)
            txtWebUser.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebUser);
            txtWebPass.BackColor = SystemColors.Window;
            toolTipCore.Hide(txtWebPass);

            if ((txtWebUser.Text.Length < 1) && (txtWebPass.Text.Length > 0))
            {
               txtWebUser.BackColor = Color.Yellow;
               txtWebUser.Focus();
               toolTipCore.Show("Username must be specified if password is set.", txtWebUser, 5000);
               return false;
            }
            else if ((txtWebUser.Text.Length > 0) && (txtWebPass.Text.Length < 1))
            {
               txtWebPass.BackColor = Color.Yellow;
               txtWebPass.Focus();
               toolTipCore.Show("Password must be specified if username is set.", txtWebPass, 5000);
               return false;
            }
         }

         // Check for error conditions
         if (txtName.BackColor == Color.Yellow ||
             txtClientMegahertz.BackColor == Color.Yellow ||
             txtLogFileName.BackColor == Color.Yellow ||
             txtUnitFileName.BackColor == Color.Yellow ||
             txtQueueFileName.BackColor == Color.Yellow ||
             txtLocalPath.BackColor == Color.Yellow ||
             txtFTPServer.BackColor == Color.Yellow ||
             txtFTPPath.BackColor == Color.Yellow ||
             txtWebURL.BackColor == Color.Yellow)
         {
            if (MessageBox.Show("There are validation errors.  Do you wish to accept the input anyway?", "HFM.NET",
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == System.Windows.Forms.DialogResult.Yes)
            {
               return true;
            }
            else
            {
               return false;
            }
         }
         
         return true;
      }
      
      private void UpdateClientData()
      {
         switch (_Action)
         {
            case HostAction.Add:
               AddClient();
               break;
            case HostAction.Edit:
               EditClient();
               break;
            default:
               throw new NotImplementedException(String.Format(CultureInfo.CurrentUICulture,
                  "Host action '{0}' has not been implemented.", _Action));
         }
      }

      private void AddClient()
      {
         ClientInstance NewInstance = GetInstanceData();

         if (_ClientInstances.ContainsName(NewInstance.InstanceName))
         {
            MessageBox.Show(String.Format("Client Name '{0}' already exists.", NewInstance.InstanceName));
            return;
         }

         // Add the new Host Instance
         _ClientInstances.Add(NewInstance);
      }

      private void EditClient()
      {
         ClientInstance EditInstance = GetInstanceData();

         if (_Instance.InstanceName != EditInstance.InstanceName)
         {
            if (_ClientInstances.ContainsName(EditInstance.InstanceName))
            {
               MessageBox.Show(String.Format("Client Name '{0}' already exists.", EditInstance.InstanceName));
               return;
            }
         }

         string PreviousName = _Instance.InstanceName;
         string PreviousPath = _Instance.Path;
         
         _Instance.InstanceHostType = EditInstance.InstanceHostType;
         _Instance.InstanceName = EditInstance.InstanceName;
         _Instance.RemoteFAHLogFilename = EditInstance.RemoteFAHLogFilename;
         _Instance.RemoteUnitInfoFilename = EditInstance.RemoteUnitInfoFilename;
         _Instance.RemoteQueueFilename = EditInstance.RemoteQueueFilename;
         _Instance.ClientProcessorMegahertz = EditInstance.ClientProcessorMegahertz;
         _Instance.ClientIsOnVirtualMachine = EditInstance.ClientIsOnVirtualMachine;
         _Instance.ClientTimeOffset = EditInstance.ClientTimeOffset;
         
         _Instance.Path = EditInstance.Path;
         _Instance.Server = EditInstance.Server;
         _Instance.Username = EditInstance.Username;
         _Instance.Password = EditInstance.Password;

         _ClientInstances.Edit(PreviousName, PreviousPath, _Instance);
      }

      #endregion
   }
}
