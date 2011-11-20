/*
 * HFM.NET - Host Configuration Form
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
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Forms.Controls;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public interface ILegacyClientSetupView : IWin32Window
   {
      /// <summary>
      /// Maximum Dialog Height
      /// </summary>
      int MaxHeight { get; }

      /// <summary>
      /// Maximum Dialog Width
      /// </summary>
      int MaxWidth { get; }

      void AttachPresenter(LegacyClientSetupPresenter presenter);
   
      void DataBind(LegacyClientSettingsModel settings);

      List<IValidatingControl> FindValidatingControls();

      string ClientMegahertzLabelText { get; set; }
      
      bool MergeFileNameVisible { get; set; }

      bool LogFileNamesVisible { set; }
      
      bool PathGroupVisible { set; }

      bool HttpGroupVisible { set; }

      bool FtpGroupVisible { set; }

      string LocalPathText { set; }
      
      bool ClientIsOnVirtualMachineVisible { set; }
      
      bool ClientTimeOffsetVisible { set; }

      void ShowConnectionSucceededMessage();

      void ShowConnectionFailedMessage(string message);

      void SetDefaultCursor();

      void SetWaitCursor();

      #region System.Windows.Forms.Form Exposure

      string Text { get; set; }

      DialogResult ShowDialog(IWin32Window owner);

      void Close();

      Size Size { get; set; }

      DialogResult DialogResult { get; set; }

      #endregion
   }

   public partial class LegacyClientSetupDialog : FormWrapper, ILegacyClientSetupView
   {
      #region Members
      
      /// <summary>
      /// Maximum Dialog Height
      /// </summary>
      public int MaxHeight { get; private set; }

      /// <summary>
      /// Maximum Dialog Width
      /// </summary>
      public int MaxWidth { get; private set; }

      private LegacyClientSetupPresenter _presenter;

      #endregion
   
      #region Constructor
      
      public LegacyClientSetupDialog()
      {
         InitializeComponent();
         
         numOffset.Minimum = Constants.MinOffsetMinutes;
         numOffset.Maximum = Constants.MaxOffsetMinutes;
         
         MaxHeight = Height;
         MaxWidth = Width;
      }
      
      #endregion
      
      public void AttachPresenter(LegacyClientSetupPresenter presenter)
      {
         _presenter = presenter;
      }

      private void LegacyClientSetupDialogShown(object sender, EventArgs e)
      {
         txtDummy.Visible = false;
      }
      
      public void DataBind(LegacyClientSettingsModel settings)
      {
         txtName.DataBindings.Add("Text", settings, "Name", false, DataSourceUpdateMode.OnValidation);
         //txtMergeFileName.DataBindings.Add("Text", settings, "ExternalFileName", false, DataSourceUpdateMode.OnValidation);
         txtClientMegahertz.DataBindings.Add("Text", settings, "ClientProcessorMegahertz", false, DataSourceUpdateMode.OnValidation);
         txtLogFileName.DataBindings.Add("Text", settings, "FahLogFileName", false, DataSourceUpdateMode.OnValidation);
         txtUnitFileName.DataBindings.Add("Text", settings, "UnitInfoFileName", false, DataSourceUpdateMode.OnValidation);
         txtQueueFileName.DataBindings.Add("Text", settings, "QueueFileName", false, DataSourceUpdateMode.OnValidation);
         pnlHostType.DataSource = settings;
         pnlHostType.ValueMember = "ClientLegacySubType";
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
         chkClientVM.DataBindings.Add("Checked", settings, "UtcOffsetIsZero", false, DataSourceUpdateMode.OnPropertyChanged);
         numOffset.DataBindings.Add("Value", settings, "ClientTimeOffset", false, DataSourceUpdateMode.OnPropertyChanged);
         txtDummy.DataBindings.Add("Text", settings, "Dummy", false, DataSourceUpdateMode.Never);
      }

      public List<IValidatingControl> FindValidatingControls()
      {
         return FindValidatingControls(Controls);
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

      public string ClientMegahertzLabelText
      {
         get { return lblClientMegahertz.Text; }
         set { lblClientMegahertz.Text = value; }
      }

      public bool MergeFileNameVisible
      {
         get { return txtMergeFileName.Visible; }
         set { txtMergeFileName.Visible = value; }
      }
      
      public bool LogFileNamesVisible
      {
         set 
         { 
            //lblClientMegahertz.Visible = value;
            //txtClientMegahertz.Visible = value;
            lblLogFileName.Visible = value;
            txtLogFileName.Visible = value;
            lblUnitFileName.Visible = value;
            txtUnitFileName.Visible = value;
            lblQueueFileName.Visible = value;
            txtQueueFileName.Visible = value;

            pnlHostType.Top = pnlHostType.Top - LegacyClientSetupPresenter.LogFileNamesVisibleOffset;
            btnTestConnection.Top = btnTestConnection.Top - LegacyClientSetupPresenter.LogFileNamesVisibleOffset;
            grpLocal.Top = grpLocal.Top - LegacyClientSetupPresenter.LogFileNamesVisibleOffset;
            grpHTTP.Top = grpHTTP.Top - LegacyClientSetupPresenter.LogFileNamesVisibleOffset;
            grpFTP.Top = grpFTP.Top - LegacyClientSetupPresenter.LogFileNamesVisibleOffset;
         }
      }

      public bool PathGroupVisible
      {
         //get { return grpLocal.Visible; }
         set
         {
            grpLocal.Visible = value;
            txtLocalPath.Enabled = value;
         }
      }

      public bool HttpGroupVisible
      {
         //get { return grpHTTP.Visible; }
         set
         {
            grpHTTP.Visible = value;
            txtWebURL.Enabled = value;
            txtWebUser.Enabled = value;
            txtWebPass.Enabled = value;
         }
      }

      public bool FtpGroupVisible
      {
         //get { return grpFTP.Visible; }
         set
         {
            grpFTP.Visible = value;
            txtFTPServer.Enabled = value;
            txtFTPPath.Enabled = value;
            txtFTPUser.Enabled = value;
            txtFTPPass.Enabled = value;
         }
      }
      
      public string LocalPathText
      {
         set { txtLocalPath.Text = value; }
      }
      
      public bool ClientIsOnVirtualMachineVisible
      {
         set { chkClientVM.Visible = value; }
      }
      
      public bool ClientTimeOffsetVisible
      {
         set
         {
            numOffset.Visible = value;
            lblOffset.Visible = value;
         }
      }

      #region Button Event Handlers

      private void btnBrowseLocal_Click(object sender, EventArgs e)
      {
         _presenter.LocalBrowseClicked();
      }
      
      private void btnTestConnection_Click(object sender, EventArgs e)
      {
         _presenter.TestConnectionClicked();
      }
      
      private void btnOK_Click(object sender, EventArgs e)
      {
         _presenter.OkClicked();
      }

      #endregion
      
      public void ShowConnectionSucceededMessage()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(ShowConnectionSucceededMessage));
            return;
         }

         MessageBox.Show(this, "Test Connection Succeeded", Core.Application.NameAndVersion,
            MessageBoxButtons.OK, MessageBoxIcon.Information);
      }

      public void ShowConnectionFailedMessage(string message)
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(() => ShowConnectionFailedMessage(message)));
            return;
         }

         MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Test Connection Failed{0}{0}{1}",
            Environment.NewLine, message), Core.Application.NameAndVersion, MessageBoxButtons.OK,
               MessageBoxIcon.Error);
      }

      public void SetDefaultCursor()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(SetDefaultCursor));
            return;
         }

         Cursor = Cursors.Default;
      }

      public void SetWaitCursor()
      {
         if (InvokeRequired)
         {
            Invoke(new MethodInvoker(SetWaitCursor));
            return;
         }

         Cursor = Cursors.WaitCursor;
      }
   }
}
