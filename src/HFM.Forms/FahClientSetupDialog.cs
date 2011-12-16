/*
 * HFM.NET - FAH Client Setup Dialog
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
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Forms.Controls;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public interface IFahClientSetupView : IWin32Window, IDisposable
   {
      void AttachPresenter(FahClientSetupPresenter presenter);

      void DataBind(FahClientSettingsModel settings);

      List<IValidatingControl> FindValidatingControls();

      void RefreshSlotsGrid();

      void SetConnectButtonEnabled(bool enabled);

      void SetDefaultCursor();

      void SetWaitCursor();

      #region System.Windows.Forms.Form Exposure

      DialogResult ShowDialog(IWin32Window owner);

      void Close();

      DialogResult DialogResult { get; set; }

      #endregion
   }

   public partial class FahClientSetupDialog : FormWrapper, IFahClientSetupView
   {
      private FahClientSetupPresenter _presenter;
      private readonly BindingSource _slotsGridBindingSource;

      public FahClientSetupDialog()
      {
         _slotsGridBindingSource = new BindingSource();

         InitializeComponent();

         ClientTimeOffsetUpDown.Minimum = Constants.MinOffsetMinutes;
         ClientTimeOffsetUpDown.Maximum = Constants.MaxOffsetMinutes;
      }

      public void AttachPresenter(FahClientSetupPresenter presenter)
      {
         _presenter = presenter;
      }

      private void FahClientSetupDialogShown(object sender, EventArgs e)
      {
         DummyTextBox.Visible = false;
      }

      public void DataBind(FahClientSettingsModel settings)
      {
         ClientNameTextBox.DataBindings.Add("Text", settings, "Name", false, DataSourceUpdateMode.OnValidation);
         AddressTextBox.DataBindings.Add("Text", settings, "Server", false, DataSourceUpdateMode.OnValidation);
         AddressPortTextBox.DataBindings.Add("Text", settings, "Port", false, DataSourceUpdateMode.OnValidation);
         PasswordTextBox.DataBindings.Add("Text", settings, "Password", false, DataSourceUpdateMode.OnValidation);
         _slotsGridBindingSource.DataSource = settings.Slots;
         SlotsDataGridView.DataSource = _slotsGridBindingSource;
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

      private void ConnectButtonClick(object sender, EventArgs e)
      {
         _presenter.ConnectClicked();
      }

      private void DialogOkButtonClick(object sender, EventArgs e)
      {
         _presenter.OkClicked();   
      }

      public void RefreshSlotsGrid()
      {
         _slotsGridBindingSource.ResetBindings(false);
      }

      public void SetConnectButtonEnabled(bool enabled)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<bool>(SetConnectButtonEnabled), enabled);
            return;
         }

         ConnectButton.Enabled = enabled;
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
