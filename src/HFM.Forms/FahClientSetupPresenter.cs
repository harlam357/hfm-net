/*
 * HFM.NET - FAH Client Setup Presenter
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

using HFM.Client;
using HFM.Client.DataTypes;
using HFM.Core;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public interface IFahClientSetupPresenter
   {
      FahClientSettingsModel SettingsModel { get; set; }

      DialogResult ShowDialog(IWin32Window owner);
   }

   public sealed class FahClientSetupPresenter : IFahClientSetupPresenter
   {
      private FahClientSettingsModel _settingsModel;

      public FahClientSettingsModel SettingsModel
      {
         get { return _settingsModel; }
         set
         {
            // remove any event handlers currently attached
            if (_settingsModel != null) _settingsModel.PropertyChanged -= SettingsModelPropertyChanged;

            _settingsModel = value;
            _settingsView.DataBind(_settingsModel);
            _propertyCollection = TypeDescriptor.GetProperties(_settingsModel);
            _settingsModel.PropertyChanged += SettingsModelPropertyChanged;
         }
      }

      private PropertyDescriptorCollection _propertyCollection;

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      private readonly IFahClientSetupView _settingsView;
      private readonly IMessageConnection _messageConnection;
      private readonly IMessageBoxView _messageBoxView;
      private readonly List<IValidatingControl> _validatingControls;

      private SlotCollection _slotCollection;

      public FahClientSetupPresenter(IFahClientSetupView settingsView, IMessageConnection messageConnection, IMessageBoxView messageBoxView)
         : this(settingsView, messageConnection, messageBoxView, null)
      {
         
      }

      internal FahClientSetupPresenter(IFahClientSetupView settingsView, IMessageConnection messageConnection, IMessageBoxView messageBoxView, TaskScheduler taskScheduler)
      {
         _settingsView = settingsView;
         _settingsView.AttachPresenter(this);
         _messageConnection = messageConnection;
         _messageBoxView = messageBoxView;
         _validatingControls = _settingsView.FindValidatingControls();
         var fahClientEventTaskScheduler = taskScheduler ?? TaskScheduler.FromCurrentSynchronizationContext();

         // wire events - these may be raised on a thread other than the UI thread
         _messageConnection.ConnectedChanged += (s, e) => Task.Factory.StartNew(() => ConnectedChanged(e), CancellationToken.None, TaskCreationOptions.None, fahClientEventTaskScheduler);
         _messageConnection.MessageReceived += (s, e) => Task.Factory.StartNew(() => MessageReceived(e), CancellationToken.None, TaskCreationOptions.None, fahClientEventTaskScheduler);
      }

      private void ConnectedChanged(ConnectedChangedEventArgs e)
      {
         _settingsView.SetConnectButtonEnabled(!e.Connected);
         if (e.Connected)
         {
            _messageConnection.SendCommand("slot-info");
         }
      }

      private void MessageReceived(MessageReceivedEventArgs e)
      {
         if (e.DataType == typeof(SlotCollection))
         {
            _slotCollection = (SlotCollection)e.TypedMessage;
            foreach (var slot in _slotCollection)
            {
               _messageConnection.SendCommand(String.Format(CultureInfo.InvariantCulture, Constants.FahClientSlotOptions, slot.Id));
            }
            _settingsModel.RefreshSlots(_slotCollection);
         }
         else if (e.DataType == typeof(SlotOptions))
         {
            var options = (SlotOptions)e.TypedMessage;
            if (options.MachineId.HasValue)
            {
               var slot = _slotCollection.FirstOrDefault(x => x.Id == options.MachineId);
               if (slot != null)
               {
                  slot.SlotOptions = options;
                  _settingsModel.RefreshSlots(_slotCollection);
               }
            }
         }
      }

      public DialogResult ShowDialog(IWin32Window owner)
      {
         if (!_settingsModel.Error)
         {
            try
            {
               Connect();
            }
            catch (Exception ex)
            {
               _logger.ErrorFormat(ex, "{0}", ex.Message);
            }
         }

         return _settingsView.ShowDialog(owner);
      }

      private void SettingsModelPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         SetPropertyErrorState(e.PropertyName, true);
         if (e.PropertyName.Equals("Slots"))
         {
            _settingsView.RefreshSlotsGrid();
         }
      }

      public void SetPropertyErrorState()
      {
         foreach (PropertyDescriptor property in _propertyCollection)
         {
            SetPropertyErrorState(property.DisplayName, false);
         }
      }

      private void SetPropertyErrorState(string boundProperty, bool showToolTip)
      {
         var errorProperty = _propertyCollection.Find(boundProperty + "Error", false);
         if (errorProperty != null)
         {
            SetPropertyErrorState(boundProperty, errorProperty, showToolTip);
         }
      }

      private void SetPropertyErrorState(string boundProperty, PropertyDescriptor errorProperty, bool showToolTip)
      {
         Debug.Assert(boundProperty != null);
         Debug.Assert(errorProperty != null);

         var validatingControls = FindBoundControls(boundProperty);
         // ReSharper disable PossibleNullReferenceException
         var errorState = (bool)errorProperty.GetValue(SettingsModel);
         // ReSharper restore PossibleNullReferenceException
         foreach (var control in validatingControls)
         {
            control.ErrorState = errorState;
            if (showToolTip) control.ShowToolTip();
         }
      }

      private IEnumerable<IValidatingControl> FindBoundControls(string propertyName)
      {
         return _validatingControls.FindAll(x =>
         {
            if (x.DataBindings["Text"] != null)
            {
               // ReSharper disable PossibleNullReferenceException
               return x.DataBindings["Text"].BindingMemberInfo.BindingField == propertyName;
               // ReSharper restore PossibleNullReferenceException
            }
            return false;
         }).AsReadOnly();
      }
      
      public void ConnectClicked()
      {
         if (_messageConnection.Connected)
         {
            return;
         }

         try
         {
            Connect();
         }
         catch (Exception ex)
         {
            _messageBoxView.ShowError(_settingsView, ex.Message, Core.Application.NameAndVersion);
         }
      }

      private void Connect()
      {
         _messageConnection.Connect(_settingsModel.Server, _settingsModel.Port, _settingsModel.Password);
      }

      public void OkClicked()
      {
         if (ValidateAcceptance())
         {
            _settingsView.DialogResult = DialogResult.OK;
            _settingsView.Close();
         }
      }

      private bool ValidateAcceptance()
      {
         SetPropertyErrorState();
         // Check for error conditions
         if (SettingsModel.Error)
         {
            _messageBoxView.ShowError(_settingsView,
               "There are validation errors.  Please correct the yellow highlighted fields.",
                  Core.Application.NameAndVersion);
            return false;
         }

         return true;
      }
   }
}
