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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instances;
using HFM.Instrumentation;

namespace HFM.Forms
{
   public enum HostAction
   {
      Add,
      Edit
   }

   public partial class frmHost : Classes.FormWrapper
   {
      #region Members
      /// <summary>
      /// Maximum Dialog Height
      /// </summary>
      private readonly int MaxHeight;

      /// <summary>
      /// Maximum Dialog Width
      /// </summary>
      private readonly int MaxWidth;
      
      /// <summary>
      /// Instance Collection
      /// </summary>
      private readonly InstanceCollection _ClientInstances;

      /// <summary>
      /// Client Instance being Edited
      /// </summary>
      private readonly ClientInstance _Instance;

      /// <summary>
      /// Dialog Action Mode
      /// </summary>
      private readonly HostAction _Action;
      
      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private NetworkOps net;
      #endregion
   
      #region Constructor
      public frmHost(InstanceCollection ClientInstances) 
         : this(ClientInstances, null, HostAction.Add)
      { }

      public frmHost(InstanceCollection ClientInstances, ClientInstance Instance)
         : this(ClientInstances, Instance, HostAction.Edit)
      { }

      private frmHost(InstanceCollection ClientInstances, ClientInstance Instance, HostAction Action)
      {
         _ClientInstances = ClientInstances;
         _Instance = Instance;
         _Action = Action;
      
         InitializeComponent();
         
         txtFTPUser.CompanionControls.Add(txtFTPPass);
         txtFTPPass.CompanionControls.Add(txtFTPUser);
         
         txtWebUser.CompanionControls.Add(txtWebPass);
         txtWebPass.CompanionControls.Add(txtWebUser);
         
         MaxHeight = Height;
         MaxWidth = Width;

         txtLogFileName.Text = Constants.LocalFAHLog;
         txtUnitFileName.Text = Constants.LocalUnitInfo;
         txtQueueFileName.Text = Constants.LocalQueue;
         
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
      
      #region Set Instance Data
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
               throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
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
               throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
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
         switch (Instance.FtpMode)
         {
            case FtpType.Passive:
               radioPassive.Checked = true;
               break;
            case FtpType.Active:
               radioActive.Checked = true;
               break;
            default:
               radioPassive.Checked = true;
               break;
         }
      }
      
      private void SetHTTPClientInfo(ClientInstance Instance)
      {
         txtWebURL.Text = Instance.Path;
         txtWebUser.Text = Instance.Username;
         txtWebPass.Text = Instance.Password;
      }
      #endregion

      #region Get Instance Data
      private ClientInstance GetInstanceData()
      {
         ClientInstance NewInstance = _ClientInstances.GetNewClientInstance();
         NewInstance.InstanceHostType = GetInstanceType();
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
               throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
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
         Instance.FtpMode = GetFtpTypeFromControls();
      }
      
      private FtpType GetFtpTypeFromControls()
      {
         FtpType ftpType = FtpType.Passive;
         if (radioActive.Checked) ftpType = FtpType.Active;
         return ftpType;
      }

      private void GetHTTPClientInfo(ClientInstance Instance)
      {
         Instance.Path = txtWebURL.Text;
         Instance.Username = txtWebUser.Text;
         Instance.Password = txtWebPass.Text;
      }
      #endregion

      #region Radio Button Management
      /// <summary>
      /// Enable the HTTP controls
      /// </summary>
      /// <param name="bState"></param>
      private void HTTPFieldsActive(Boolean bState)
      {
         if (bState)
         {
            Size = new Size(MaxWidth, MaxHeight - 50);
         }
         grpHTTP.Visible = bState;
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
            Size = new Size(MaxWidth, MaxHeight - 78);
         }
         grpLocal.Visible = bState;
         txtLocalPath.Enabled = bState;
      }

      /// <summary>
      /// Configure the form fields according to the selected radio button
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      private void radioButtonSet_CheckedChanged(object sender, EventArgs e)
      {
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
            txtLocalPath.ValidateControlText();
         }
      }
      #endregion

      #region Text Field Validators
      private void txtName_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         
         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (StringOps.ValidateInstanceName(e.ControlText) == false)
         {
            e.ValidationResult = false;
         }
      }

      private void txtClientMegahertz_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         int mhz;
         if (int.TryParse(e.ControlText, out mhz) == false)
         {
            e.ValidationResult = false;
         }
         else if (mhz < 1)
         {
            e.ValidationResult = false;
         }
      }

      private void txtFileName_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         
         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (StringOps.ValidateFileName(e.ControlText) == false)
         {
            e.ValidationResult = false;
         }
      }

      private void txtLocalPath_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ControlText = StripFahClientFileNames(e.ControlText);

         bool bPath = StringOps.ValidatePathInstancePath(e.ControlText);
         bool bPathWithSlash = StringOps.ValidatePathInstancePath(String.Concat(e.ControlText, Path.DirectorySeparatorChar));

         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (e.ControlText.Length > 2 && (bPath || bPathWithSlash) != true)
         {
            e.ValidationResult = false;
         }
         else
         {
            if (bPath == false && bPathWithSlash)
            {
               e.ControlText += Path.DirectorySeparatorChar;
            }
            e.ValidationResult = true;
         }
      }

      private void txtFTPServer_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
      
         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (StringOps.ValidateServerName(e.ControlText) == false)
         {
            e.ValidationResult = false;
         }
      }

      private void txtFTPPath_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         e.ControlText = StripFahClientFileNames(e.ControlText);
         
         if (e.ControlText.EndsWith("/") == false)
         {
            e.ControlText += "/";
         }

         if (e.ControlText != "/") // Root path, don't validate against Regex
         {
            if (e.ControlText.Length == 0)
            {
               e.ValidationResult = false;
            }
            else if (StringOps.ValidateFtpPath(e.ControlText) == false)
            {
               e.ValidationResult = false;
            }
         }
      }

      private void txtFtpCredentials_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;

         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtFTPUser.Text, txtFTPPass.Text, true);
         }
         catch (ArgumentException ex)
         {
            e.ErrorToolTipText = ex.Message;
            e.ValidationResult = false;
         }
      }

      private void txtWebURL_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;
         e.ControlText = StripFahClientFileNames(e.ControlText);

         if (e.ControlText.EndsWith("/") == false)
         {
            e.ControlText += "/";
         }

         if (e.ControlText.Length == 0)
         {
            e.ValidationResult = false;
         }
         else if (StringOps.ValidateHttpURL(e.ControlText) == false)
         {
            e.ValidationResult = false;
         }
      }

      private void txtHttpCredentials_CustomValidation(object sender, ValidatingControlCustomValidationEventArgs e)
      {
         e.ValidationResult = true;

         try
         {
            // This will violate FxCop rule (rule ID)
            StringOps.ValidateUsernamePasswordPair(txtWebUser.Text, txtWebPass.Text);
         }
         catch (ArgumentException ex)
         {
            e.ErrorToolTipText = ex.Message;
            e.ValidationResult = false;
         }
      }
      
      private string StripFahClientFileNames(string value)
      {
         if (value.ToUpperInvariant().EndsWith("FAHLOG.TXT"))
         {
            return value.Substring(0, txtLocalPath.Text.Length - 10);
         }
         if (value.ToUpperInvariant().EndsWith("UNITINFO.TXT"))
         {
            return value.Substring(0, txtLocalPath.Text.Length - 12);
         }
         if (value.ToUpperInvariant().EndsWith("QUEUE.DAT"))
         {
            return value.Substring(0, txtLocalPath.Text.Length - 9);
         }
         
         return value;
      }
      #endregion

      #region Button Event Handlers
      private void btnTestConnection_Click(object sender, EventArgs e)
      {
         if (net == null)
         {
            net = new NetworkOps();
         }

         try
         {
            SetWaitCursor();
            if (radioLocal.Checked)
            {
               CheckFileConnectionDelegate del = CheckFileConnection;
               del.BeginInvoke(txtLocalPath.Text, CheckFileConnectionCallback, del);
            }
            else if (radioFTP.Checked)
            {
               FtpCheckConnectionDelegate del = net.FtpCheckConnection;
               del.BeginInvoke(txtFTPServer.Text, txtFTPPath.Text, txtFTPUser.Text, txtFTPPass.Text, GetFtpTypeFromControls(), FtpCheckConnectionCallback, del);
            }
            else if (radioHTTP.Checked)
            {
               HttpCheckConnectionDelegate del = net.HttpCheckConnection;
               del.BeginInvoke(txtWebURL.Text, txtWebUser.Text, txtWebPass.Text, HttpCheckConnectionCallback, del);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            ShowConnectionFailedMessage(ex.Message);
         }
      }

      private delegate void CheckFileConnectionDelegate(string directory);

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
            CheckFileConnectionDelegate del = (CheckFileConnectionDelegate)result.AsyncState;
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
            FtpCheckConnectionDelegate del = (FtpCheckConnectionDelegate)result.AsyncState;
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

      private void HttpCheckConnectionCallback(IAsyncResult result)
      {
         try
         {
            HttpCheckConnectionDelegate del = (HttpCheckConnectionDelegate)result.AsyncState;
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
      
      private delegate void ShowConnectionFailedMessageDelegate(string message);

      private void ShowConnectionFailedMessage(string message)
      {
         if (InvokeRequired)
         {
            Invoke(new ShowConnectionFailedMessageDelegate(ShowConnectionFailedMessage), message);
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
         if (ValidateAcceptance())
         {
            UpdateClientData();
         
            DialogResult = DialogResult.OK;
            Close();
         }
      }
      
      private bool ValidateAcceptance()
      {
         txtName.ValidateControlText();
         txtLogFileName.ValidateControlText();
         txtUnitFileName.ValidateControlText();
         txtQueueFileName.ValidateControlText();
         if (radioLocal.Checked)
         {
            txtLocalPath.ValidateControlText();
         }
         else if (radioFTP.Checked)
         {
            txtFTPServer.ValidateControlText();
            txtFTPPath.ValidateControlText();
            txtFTPUser.ValidateControlText();
            txtFTPPass.ValidateControlText();
         }
         else if (radioHTTP.Checked)
         {
            txtWebURL.ValidateControlText();
            txtWebUser.ValidateControlText();
            txtWebPass.ValidateControlText();
         }

         // Check for error conditions
         if (txtName.ErrorState ||
             txtClientMegahertz.ErrorState ||
             txtLogFileName.ErrorState ||
             txtUnitFileName.ErrorState ||
             txtQueueFileName.ErrorState)
         {
            MessageBox.Show("There are validation errors.  Please correct the yellow highlighted fields.", "HFM.NET",
               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         if (txtLocalPath.ErrorState ||
             txtFTPServer.ErrorState ||
             txtFTPPath.ErrorState ||
             txtFTPUser.ErrorState ||
             txtFTPPass.ErrorState ||
             txtWebURL.ErrorState ||
             txtWebUser.ErrorState ||
             txtWebPass.ErrorState)
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
               throw new NotImplementedException(String.Format(CultureInfo.CurrentCulture,
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
         _Instance.FtpMode = EditInstance.FtpMode;

         _ClientInstances.Edit(PreviousName, PreviousPath, _Instance);
      }
      #endregion
   }
}
