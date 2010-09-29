
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;

namespace HFM.Forms
{
   public class InstanceSettingsPresenter
   {
      public const int LogFileNamesVisibleOffset = 70;
   
      #region Fields
      
      private IClientInstanceSettings _settings;
      /// <summary>
      /// Client Instance being Edited
      /// </summary>
      public IClientInstanceSettings Settings
      {
         get { return _settings; }
         set
         {
            // remove any event handlers currently attached
            if (_settings != null) _settings.PropertyChanged -= SettingsPropertyChanged;
            
            _settings = value;
            _settingsView.DataBind(_settings);
            _propertyCollection = TypeDescriptor.GetProperties(_settings);
            _settings.PropertyChanged += SettingsPropertyChanged;
            SetViewInstanceType();
            SetViewHostType();
         }
      }

      private PropertyDescriptorCollection _propertyCollection;

      private readonly IInstanceSettingsView _settingsView;

      /// <summary>
      /// Network Operations Interface
      /// </summary>
      private readonly INetworkOps _networkOps;

      private readonly IMessageBoxView _messageBoxView;

      private readonly IFolderBrowserView _folderBrowserView;

      private readonly List<IValidatingControl> _validatingControls;

      #endregion

      #region Constructor

      public InstanceSettingsPresenter(IInstanceSettingsView settingsView, INetworkOps networkOps, IMessageBoxView messageBoxView, IFolderBrowserView folderBrowserView)
      {
         _settingsView = settingsView;
         _settingsView.AttachPresenter(this);
         _networkOps = networkOps;
         _messageBoxView = messageBoxView;
         _folderBrowserView = folderBrowserView;
         _validatingControls = _settingsView.FindValidatingControls();
      }
      
      #endregion
      
      public DialogResult ShowDialog(IWin32Window owner)
      {
         return _settingsView.ShowDialog(owner);
      }

      private void SettingsPropertyChanged(object sender, PropertyChangedEventArgs e)
      {
         // Dummy because this is a radio button group
         if (e.PropertyName == "Dummy")
         {
            SetViewHostType();
         }
         SetPropertyErrorState(e.PropertyName, true);
      }
      
      private void SetViewInstanceType()
      {
         if (_settings.ExternalInstance)
         {
            _settingsView.ClientMegahertzLabelText = "Merge File Name:";
            _settingsView.MergeFileNameVisible = true;
            _settingsView.LogFileNamesVisible = false;
            _settingsView.ClientIsOnVirtualMachineVisible = false;
            _settingsView.ClientTimeOffsetVisible = false;
         }
      }
      
      private void SetViewHostType()
      {
         switch (_settings.InstanceHostType)
         {
            case InstanceType.PathInstance:
               if (_settings.ExternalInstance)
               {
                  
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight - 78 - LogFileNamesVisibleOffset);
               }
               else
               {
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight - 78);
               }
               _settingsView.PathGroupVisible = true;
               _settingsView.HttpGroupVisible = false;
               _settingsView.FtpGroupVisible = false;
               break;
            case InstanceType.HttpInstance:
               if (_settings.ExternalInstance)
               {
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight - 50 - LogFileNamesVisibleOffset);
               }
               else
               {
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight - 50);
               }
               _settingsView.PathGroupVisible = false;
               _settingsView.HttpGroupVisible = true;
               _settingsView.FtpGroupVisible = false;
               break;
            case InstanceType.FtpInstance:
               if (_settings.ExternalInstance)
               {
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight - LogFileNamesVisibleOffset);
               }
               else
               {
                  _settingsView.Size = new Size(_settingsView.MaxWidth, _settingsView.MaxHeight);
               }
               _settingsView.PathGroupVisible = false;
               _settingsView.HttpGroupVisible = false;
               _settingsView.FtpGroupVisible = true;
               break;
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
         var errorState = (bool)errorProperty.GetValue(Settings);
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
      
      public void LocalBrowseClicked()
      {
         if (Settings.Path.Length > 0)
         {
            _folderBrowserView.SelectedPath = Settings.Path;
         }

         _folderBrowserView.ShowDialog(_settingsView);
         if (_folderBrowserView.SelectedPath.Length > 0)
         {
            Settings.Path = _folderBrowserView.SelectedPath;
         }
      }

      public void TestConnectionClicked()
      {
         //try
         //{
            _settingsView.SetWaitCursor();
            if (_settings.InstanceHostType.Equals(InstanceType.PathInstance))
            {
               var action = new Action(() => CheckFileConnection(Settings.Path));
               action.BeginInvoke(CheckConnectionCallback, action);
            }
            else if (_settings.InstanceHostType.Equals(InstanceType.FtpInstance))
            {
               _networkOps.BeginFtpCheckConnection(Settings.Server, Settings.Path, Settings.Username, Settings.Password, Settings.FtpMode, CheckConnectionCallback);
            }
            else if (_settings.InstanceHostType.Equals(InstanceType.HttpInstance))
            {
               _networkOps.BeginHttpCheckConnection(Settings.Path, Settings.Username, Settings.Password, CheckConnectionCallback);
            }
         //}
         //catch (Exception ex)
         //{
         //   HfmTrace.WriteToHfmConsole(ex);
         //   _settingsView.ShowConnectionFailedMessage(ex.Message);
         //}
      }

      public void CheckFileConnection(string directory)
      {
         if (Directory.Exists(directory) == false)
         {
            throw new IOException(String.Format(CultureInfo.CurrentCulture,
               "Folder Path '{0}' does not exist.", directory));
         }
      }

      private void CheckConnectionCallback(IAsyncResult result)
      {
         try
         {
            var action = (Action)result.AsyncState;
            action.EndInvoke(result);
            _settingsView.ShowConnectionSucceededMessage();
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            _settingsView.ShowConnectionFailedMessage(ex.Message);
         }
         finally
         {
            _settingsView.SetDefaultCursor();
         }
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
         if (Settings.InstanceNameError ||
             Settings.ClientProcessorMegahertzError ||
             Settings.RemoteFAHLogFilenameError ||
             Settings.RemoteUnitInfoFilenameError ||
             Settings.RemoteQueueFilenameError ||
             Settings.ServerError ||
             Settings.CredentialsError ||
             Settings.PathEmpty)
         {
            _messageBoxView.ShowError(_settingsView, 
               "There are validation errors.  Please correct the yellow highlighted fields.", 
                  Constants.ApplicationName);
            return false;
         }

         if (Settings.PathError)
         {
            if (_messageBoxView.AskYesNoQuestion(_settingsView, 
               "There are validation errors.  Do you wish to accept the input anyway?", 
                  Constants.ApplicationName) == DialogResult.Yes)
            {
               return true;
            }

            return false;
         }

         return true;
      }
   }
}
