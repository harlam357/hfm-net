/*
 * HFM.NET - Host Configuration Form
 * Copyright (C) 2006 David Rawling
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Helpers;
using HFM.Instrumentation;

namespace HFM.Forms
{
   public partial class frmHost : Classes.FormWrapper
   {
      #region Members
      /// <summary>
      /// Maximum Dialog Height
      /// </summary>
      private readonly int _maxHeight;

      /// <summary>
      /// Maximum Dialog Width
      /// </summary>
      private readonly int _maxWidth;

      /// <summary>
      /// Client Instance being Edited
      /// </summary>
      private readonly IClientInstanceSettings _settings;
      /// <summary>
      /// Client Instance being Edited
      /// </summary>
      public IClientInstanceSettings Settings
      {
         get { return _settings; }
      }
      
      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private NetworkOps _net;

      private List<IValidatingControl> _validatingControls;

      private PropertyDescriptorCollection _propertyCollection;
      #endregion
   
      #region Constructor
      public frmHost(IClientInstanceSettings settings)
      {
         _settings = settings;
      
         InitializeComponent();
         
         numOffset.Minimum = Constants.MinOffsetMinutes;
         numOffset.Maximum = Constants.MaxOffsetMinutes;
         
         _maxHeight = Height;
         _maxWidth = Width;
         
         DataBind(settings);
         FindValidatingControls();
         GetModelProperties();
         _settings.PropertyChanged += SettingsPropertyChanged;
      }
      #endregion

      private void frmHost_Shown(object sender, EventArgs e)
      {
         txtDummy.Visible = false;
      }
      
      private void DataBind(IClientInstanceSettings settings)
      {
         txtName.DataBindings.Add("Text", settings, "InstanceName", false, DataSourceUpdateMode.OnValidation);
         txtClientMegahertz.DataBindings.Add("Text", settings, "ClientProcessorMegahertz", false, DataSourceUpdateMode.OnValidation);
         txtLogFileName.DataBindings.Add("Text", settings, "RemoteFAHLogFilename", false, DataSourceUpdateMode.OnValidation);
         txtUnitFileName.DataBindings.Add("Text", settings, "RemoteUnitInfoFilename", false, DataSourceUpdateMode.OnValidation);
         txtQueueFileName.DataBindings.Add("Text", settings, "RemoteQueueFilename", false, DataSourceUpdateMode.OnValidation);
         pnlHostType.DataSource = settings;
         pnlHostType.ValueMember = "InstanceHostType";
         txtLocalPath.DataBindings.Add("Text", settings, "Path", false, DataSourceUpdateMode.OnValidation);
         txtWebURL.DataBindings.Add("Text", settings, "Path", false, DataSourceUpdateMode.OnValidation);
         txtWebUser.DataBindings.Add("Text", settings, "Username", false, DataSourceUpdateMode.OnValidation);
         txtWebUser.DataBindings.Add("ErrorToolTipText", settings, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtWebPass.DataBindings.Add("Text", settings, "Password", false, DataSourceUpdateMode.OnValidation);
         txtWebPass.DataBindings.Add("ErrorToolTipText", settings, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtFTPServer.DataBindings.Add("Text", settings, "Server", false, DataSourceUpdateMode.OnValidation);
         txtFTPPath.DataBindings.Add("Text", settings, "Path", false, DataSourceUpdateMode.OnValidation);
         txtFTPUser.DataBindings.Add("Text", settings, "Username", false, DataSourceUpdateMode.OnValidation);
         txtFTPUser.DataBindings.Add("ErrorToolTipText", settings, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         txtFTPPass.DataBindings.Add("Text", settings, "Password", false, DataSourceUpdateMode.OnValidation);
         txtFTPPass.DataBindings.Add("ErrorToolTipText", settings, "CredentialsErrorMessage", false, DataSourceUpdateMode.OnPropertyChanged);
         pnlFtpMode.DataSource = settings;
         pnlFtpMode.ValueMember = "FtpMode";
         chkClientVM.DataBindings.Add("Checked", settings, "ClientIsOnVirtualMachine", false, DataSourceUpdateMode.OnPropertyChanged);
         numOffset.DataBindings.Add("Value", settings, "ClientTimeOffset", false, DataSourceUpdateMode.OnPropertyChanged);
         txtDummy.DataBindings.Add("Text", settings, "Dummy", false, DataSourceUpdateMode.Never);
      }

      private void FindValidatingControls()
      {
         _validatingControls = FindValidatingControls(Controls);
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

      private void GetModelProperties()
      {
         _propertyCollection = TypeDescriptor.GetProperties(Settings);
      }

      private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         SetPropertyErrorState(e.PropertyName);
      }
      
      private void SetPropertyErrorState()
      {
         foreach (PropertyDescriptor property in _propertyCollection)
         {
            SetPropertyErrorState(property.DisplayName);
         }
      }
      
      private void SetPropertyErrorState(string boundProperty)
      {
         var errorProperty = _propertyCollection.Find(boundProperty + "Error", false);
         if (errorProperty != null)
         {
            SetPropertyErrorState(boundProperty, errorProperty);
         }
      }

      private void SetPropertyErrorState(string boundProperty, PropertyDescriptor errorProperty)
      {
         ICollection<IValidatingControl> validatingControls = FindBoundControls(boundProperty);
         var errorState = (bool)errorProperty.GetValue(Settings);
         foreach (var control in validatingControls)
         {
            control.ErrorState = errorState;
         }
      }

      private ReadOnlyCollection<IValidatingControl> FindBoundControls(string propertyName)
      {
         return _validatingControls.FindAll(x => x.DataBindings["Text"].BindingMemberInfo.BindingField == propertyName).AsReadOnly();
      }

      #region Radio Button Management
      /// <summary>
      /// Enable the HTTP controls
      /// </summary>
      private void HttpFieldsActive(bool state)
      {
         if (state)
         {
            Size = new Size(_maxWidth, _maxHeight - 50);
         }
         grpHTTP.Visible = state;
         txtWebURL.Enabled = state;
         txtWebUser.Enabled = state;
         txtWebPass.Enabled = state;
      }

      /// <summary>
      /// Enable/disable the FTP controls
      /// </summary>
      private void FtpFieldsActive(bool state)
      {
         if (state)
         {
            Size = new Size(_maxWidth, _maxHeight);
         }
         grpFTP.Visible = state;
         txtFTPServer.Enabled = state;
         txtFTPPath.Enabled = state;
         txtFTPUser.Enabled = state;
         txtFTPPass.Enabled = state;
      }

      /// <summary>
      /// Enable/disable the local path controls
      /// </summary>
      private void PathFieldsActive(bool state)
      {
         if (state)
         {
            Size = new Size(_maxWidth, _maxHeight - 78);
         }
         grpLocal.Visible = state;
         txtLocalPath.Enabled = state;
      }

      /// <summary>
      /// Configure the form fields according to the selected radio button
      /// </summary>
      private void radioButtonSet_CheckedChanged(object sender, EventArgs e)
      {
         if (radioLocal.Checked)
         {
            PathFieldsActive(true);
            FtpFieldsActive(false);
            HttpFieldsActive(false);
         }
         else if (radioFTP.Checked)
         {
            PathFieldsActive(false);
            FtpFieldsActive(true);
            HttpFieldsActive(false);
         }
         else if (radioHTTP.Checked)
         {
            PathFieldsActive(false);
            FtpFieldsActive(false);
            HttpFieldsActive(true);
         }
      }
      #endregion

      #region Local Path Browse functions
      /// <summary>
      /// Display the folder selection dialog. We want a path.
      /// </summary>
      private void btnBrowseLocal_Click(object sender, EventArgs e)
      {
         if (Settings.Path.Length > 0)
         {
            openLogFolder.SelectedPath = Settings.Path;
         }

         openLogFolder.ShowDialog();
         if (openLogFolder.SelectedPath.Length > 0)
         {
            Settings.Path = openLogFolder.SelectedPath;
         }
      }
      #endregion

      #region Button Event Handlers
      private void btnTestConnection_Click(object sender, EventArgs e)
      {
         if (_net == null)
         {
            _net = new NetworkOps();
         }

         try
         {
            SetWaitCursor();
            if (radioLocal.Checked)
            {
               CheckFileConnectionDelegate del = CheckFileConnection;
               del.BeginInvoke(Settings.Path, CheckFileConnectionCallback, del);
            }
            else if (radioFTP.Checked)
            {
               FtpCheckConnectionDelegate del = _net.FtpCheckConnection;
               del.BeginInvoke(Settings.Server, Settings.Path, Settings.Username, Settings.Password, Settings.FtpMode, FtpCheckConnectionCallback, del);
            }
            else if (radioHTTP.Checked)
            {
               HttpCheckConnectionDelegate del = _net.HttpCheckConnection;
               del.BeginInvoke(Settings.Path, Settings.Username, Settings.Password, HttpCheckConnectionCallback, del);
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
            var del = (CheckFileConnectionDelegate)result.AsyncState;
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
            var del = (FtpCheckConnectionDelegate)result.AsyncState;
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
            var del = (HttpCheckConnectionDelegate)result.AsyncState;
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
            DialogResult = DialogResult.OK;
            Close();
         }
      }
      
      private bool ValidateAcceptance()
      {
         SetPropertyErrorState();
         // Check for error conditions
         if (Settings.InstanceNameError ||
             Settings.ClientProcessorMegahertzError ||
             Settings.RemoteFAHLogFilenameError ||
             Settings.RemoteUnitInfoFilenameError ||
             Settings.RemoteQueueFilenameError ||
             Settings.PathEmpty)
         {
            MessageBox.Show("There are validation errors.  Please correct the yellow highlighted fields.", Constants.ApplicationName,
               MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            return false;
         }

         if (Settings.PathError ||
             Settings.ServerError ||
             Settings.CredentialsError)
         {
            if (MessageBox.Show("There are validation errors.  Do you wish to accept the input anyway?", Constants.ApplicationName,
                  MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
               return true;
            }

            return false;
         }
         
         return true;
      }
      #endregion
   }
}
