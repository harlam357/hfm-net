
using System;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Framework;

namespace HFM.Forms
{
   public class MainPresenter
   {
      /// <summary>
      /// Holds the state of the window before it is hidden (minimise to tray behaviour)
      /// </summary>
      public FormWindowState OriginalState { get; private set; }

      /// <summary>
      /// Holds current Sort Column Name
      /// </summary>
      public string SortColumnName { get; private set; }

      /// <summary>
      /// Holds current Sort Column Order
      /// </summary>
      public SortOrder SortColumnOrder { get; private set; }
   
      private readonly IMainView _view;
      private readonly IPreferenceSet _prefs;
      private readonly IInstanceCollection _instanceCollection;
      private readonly IInstanceConfigurationManager _configurationManager;
      private readonly IUpdateLogic _updateLogic;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IOpenFileDialogView _openFileDialogView;
      private readonly ISaveFileDialogView _saveFileDialogView;

      public MainPresenter(IMainView view, IPreferenceSet prefs, IInstanceCollection instanceCollection,
                           IInstanceConfigurationManager configurationManager, IUpdateLogic updateLogic, 
                           IMessageBoxView messageBoxView, IOpenFileDialogView openFileDialogView,
                           ISaveFileDialogView saveFileDialogView)
      {
         _view = view;
         _prefs = prefs;
         _instanceCollection = instanceCollection;
         _configurationManager = configurationManager;
         _updateLogic = updateLogic;
         _messageBoxView = messageBoxView;
         _openFileDialogView = openFileDialogView;
         _saveFileDialogView = saveFileDialogView;

         SortColumnName = String.Empty;
         SortColumnOrder = SortOrder.None;
      }
      
      #region View Handlers
   
      public void ViewShown()
      {
         if (_prefs.GetPreference<bool>(Preference.RunMinimized))
         {
            _view.WindowState = FormWindowState.Minimized;
         }

         string fileName = String.Empty;

         if (_prefs.GetPreference<bool>(Preference.UseDefaultConfigFile))
         {
            fileName = _prefs.GetPreference<string>(Preference.DefaultConfigFile);
         }

         if (String.IsNullOrEmpty(fileName) == false)
         {
            LoadConfigFile(fileName);
         }

         // Add the Index Changed Handler here after everything is shown
         _view.DataGridView.ColumnDisplayIndexChanged += delegate { DataGridViewColumnDisplayIndexChanged(); };
         // Then run it once to ensure the last column is set to Fill
         DataGridViewColumnDisplayIndexChanged();
         // Add the Splitter Moved Handler here after everything is shown - Issue 8
         _view.SplitContainer.SplitterMoved += delegate { SplitContainerSplitterMoved(); };

         //_notifyIcon = new NotifyIcon(components);

         _view.NotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
         _view.NotifyIcon.ContextMenuStrip = _view.NotifyMenu;
         _view.NotifyIcon.Icon = _view.Icon;
         _view.NotifyIcon.Text = _view.Text;
         _view.NotifyIcon.DoubleClick += delegate { NotifyIconDoubleClick(); };
         SetViewShowStyle();

         if (_prefs.GetPreference<bool>(Preference.StartupCheckForUpdate))
         {
            _updateLogic.CheckForUpdate();
         }
      }

      public void ViewResize()
      {
         if (_view.WindowState != FormWindowState.Minimized)
         {
            OriginalState = _view.WindowState;
            // ReApply Sort when restoring from the sys tray - Issue 32
            if (_view.ShowInTaskbar == false)
            {
               _view.ApplySort();
            }
         }

         SetViewShowStyle();

         // When the log file window (panel) is collapsed, get the split location
         // changes based on the height of Panel1 - Issue 8
         if (_view.Visible && _view.SplitContainer.Panel2Collapsed)
         {
            _prefs.SetPreference(Preference.FormSplitLocation, _view.SplitContainer.Panel1.Height);
         }
      }

      public bool ViewClosing()
      {
         if (CanContinueDestructiveOp() == false)
         {
            return true;
         }

         SaveColumnSettings();
         SaveSortColumn();

         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (_view.WindowState == FormWindowState.Normal)
         {
            _prefs.SetPreference(Preference.FormLocation, _view.Location);
            _prefs.SetPreference(Preference.FormSize, _view.Size);
         }
         else
         {
            _prefs.SetPreference(Preference.FormLocation, _view.RestoreBounds.Location);
            _prefs.SetPreference(Preference.FormSize, _view.RestoreBounds.Size);
         }

         _prefs.SetPreference(Preference.FormLogVisible, _view.LogFileVisible);
         _prefs.SetPreference(Preference.QueueViewerVisible, _view.QueueVisible);

         CheckForAndFireUpdateProcess();

         return false;
      }
      
      public void SetViewShowStyle()
      {
         switch (_prefs.GetPreference<FormShowStyleType>(Preference.FormShowStyle))
         {
            case FormShowStyleType.SystemTray:
               if (_view.NotifyIcon != null) _view.NotifyIcon.Visible = true;
               _view.ShowInTaskbar = (_view.WindowState != FormWindowState.Minimized);
               break;
            case FormShowStyleType.TaskBar:
               if (_view.NotifyIcon != null) _view.NotifyIcon.Visible = false;
               _view.ShowInTaskbar = true;
               break;
            case FormShowStyleType.Both:
               if (_view.NotifyIcon != null) _view.NotifyIcon.Visible = true;
               _view.ShowInTaskbar = true;
               break;
         }
      }

      public void RestoreFormPreferences()
      {
         //TODO: Would like to do this here in lieu of in frmMain_Shown() event.
         // There is some drawing error that if Minimized here, the first time the
         // Form is restored from the system tray, the DataGridView is drawn with
         // a big black box on the right hand side.  Like it didn't get initialized
         // properly when the Form was created.
         //if (Prefs.RunMinimized)
         //{
         //   WindowState = FormWindowState.Minimized;
         //}

         // Restore state data
         var location = _prefs.GetPreference<Point>(Preference.FormLocation);
         var size = _prefs.GetPreference<Size>(Preference.FormSize);

         if (location.X != 0 && location.Y != 0)
         {
            // Set StartPosition to manual
            _view.StartPosition = FormStartPosition.Manual;
            _view.Location = location;
         }
         if (size.Width != 0 && size.Height != 0)
         {
            if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
            {
               size = new Size(size.Width, size.Height + _prefs.GetPreference<int>(Preference.FormLogWindowHeight));
            }
            _view.Size = size;
            _view.SplitContainer.SplitterDistance = _prefs.GetPreference<int>(Preference.FormSplitLocation);
         }

         if (_prefs.GetPreference<bool>(Preference.FormLogVisible) == false)
         {
            _view.ShowHideLogWindow(false);
         }

         if (_prefs.GetPreference<bool>(Preference.QueueViewerVisible) == false)
         {
            _view.ShowHideQueue(false);
         }

         //if (Prefs.FormSortColumn != String.Empty &&
         //    Prefs.FormSortOrder != SortOrder.None)
         //{
            SortColumnName = _prefs.GetPreference<string>(Preference.FormSortColumn);
            SortColumnOrder = _prefs.GetPreference<SortOrder>(Preference.FormSortOrder);
         //}

         try
         {
            // Restore the columns' state
            var cols = _prefs.GetPreference<StringCollection>(Preference.FormColumns);
            var colsArray = new string[cols.Count];

            cols.CopyTo(colsArray, 0);
            Array.Sort(colsArray);

            for (int i = 0; i < colsArray.Length; i++)
            {
               string[] a = colsArray[i].Split(',');
               int index = int.Parse(a[3]);
               _view.DataGridView.Columns[index].DisplayIndex = Int16.Parse(a[0]);
               if (_view.DataGridView.Columns[index].AutoSizeMode.Equals(DataGridViewAutoSizeColumnMode.Fill) == false)
               {
                  _view.DataGridView.Columns[index].Width = Int16.Parse(a[1]);
               }
               _view.DataGridView.Columns[index].Visible = bool.Parse(a[2]);
            }
         }
         catch (NullReferenceException)
         {
            // This happens when the FormColumns setting is empty
         }
      }

      private void CheckForAndFireUpdateProcess()
      {
         if (String.IsNullOrEmpty(_updateLogic.UpdateFilePath) == false)
         {
            HfmTrace.WriteToHfmConsole(String.Format(CultureInfo.CurrentCulture,
               "Firing update file '{0}'...", _updateLogic.UpdateFilePath));
            try
            {
               Process.Start(_updateLogic.UpdateFilePath);
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               string message = String.Format(CultureInfo.CurrentCulture,
                                              "Update process failed to start with the following error:{0}{0}{1}",
                                              Environment.NewLine, ex.Message);
               _messageBoxView.ShowError(_view, message, _view.Text);
            }
         }
      }
      
      #endregion
      
      #region DataGridView Handlers

      private void DataGridViewColumnDisplayIndexChanged()
      {
         if (_view.DataGridView.Columns.Count == DataGridViewWrapper.NumberOfDisplayFields)
         {
            foreach (DataGridViewColumn column in _view.DataGridView.Columns)
            {
               if (column.DisplayIndex < _view.DataGridView.Columns.Count - 1)
               {
                  column.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
               }
               else
               {
                  column.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
               }
            }

            SaveColumnSettings(); // Save Column Settings - Issue 73
            _prefs.Save();
         }
      }

      private void SaveColumnSettings()
      {
         // Save column state data
         // including order, column width and whether or not the column is visible
         var stringCollection = new StringCollection();
         int i = 0;

         foreach (DataGridViewColumn column in _view.DataGridView.Columns)
         {
            stringCollection.Add(String.Format(CultureInfo.InvariantCulture,
                                    "{0},{1},{2},{3}",
                                    column.DisplayIndex.ToString("D2"),
                                    column.Width,
                                    column.Visible,
                                    i++));
         }

         _prefs.SetPreference(Preference.FormColumns, stringCollection);
      }

      private void SaveSortColumn()
      {
         _prefs.SetPreference(Preference.FormSortColumn, SortColumnName);
         _prefs.SetPreference(Preference.FormSortOrder, SortColumnOrder);
      }

      private void SplitContainerSplitterMoved()
      {
         // When the log file window (Panel2) is visible, this event will fire.
         // Update the split location directly from the split panel control. - Issue 8
         _prefs.SetPreference(Preference.FormSplitLocation, _view.SplitContainer.SplitterDistance);
         _prefs.Save();
      }

      public void DataGridViewSorted()
      {
         SortColumnName = _view.DataGridView.SortedColumn.Name;
         SortColumnOrder = _view.DataGridView.SortOrder;

         SaveSortColumn(); // Save Column Sort Order - Issue 73
         _prefs.Save();

         _view.SelectCurrentRowKey();
      }
      
      #endregion

      #region System Tray Icon Methods

      private void NotifyIconDoubleClick()
      {
         if (_view.WindowState == FormWindowState.Minimized)
         {
            _view.WindowState = OriginalState;
         }
         else
         {
            OriginalState = _view.WindowState;
            _view.WindowState = FormWindowState.Minimized;
         }
      }

      public void NotifyIconRestoreClick()
      {
         if (_view.WindowState == FormWindowState.Minimized)
         {
            _view.WindowState = OriginalState;
         }
         else if (_view.WindowState == FormWindowState.Maximized)
         {
            _view.WindowState = FormWindowState.Normal;
         }
      }

      public void NotifyIconMinimizeClick()
      {
         if (_view.WindowState != FormWindowState.Minimized)
         {
            OriginalState = _view.WindowState;
            _view.WindowState = FormWindowState.Minimized;
         }
      }

      public void NotifyIconMaximizeClick()
      {
         if (_view.WindowState != FormWindowState.Maximized)
         {
            _view.WindowState = FormWindowState.Maximized;
            OriginalState = _view.WindowState;
         }
      }
      
      #endregion
      
      #region File Handling Methods

      public void FileNewClick()
      {
         if (_instanceCollection.RetrievalInProgress)
         {
            _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to create a new config file.", _view.Text);
            return;
         }

         if (CanContinueDestructiveOp())
         {
            _instanceCollection.Clear();
         }
      }

      public void FileOpenClick()
      {
         if (_instanceCollection.RetrievalInProgress)
         {
            _messageBoxView.ShowInformation(_view, "Retrieval in progress... please wait to open another config file.", _view.Text);
            return;
         }

         if (CanContinueDestructiveOp())
         {
            _openFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
            _openFileDialogView.Filter = _configurationManager.FileTypeFilters;
            _openFileDialogView.FileName = _configurationManager.ConfigFilename;
            _openFileDialogView.RestoreDirectory = true;
            if (_openFileDialogView.ShowDialog() == DialogResult.OK)
            {
               LoadConfigFile(_openFileDialogView.FileName, _openFileDialogView.FilterIndex);
            }
         }
      }

      /// <summary>
      /// Test current application status for changes; ask for confirmation if necessary.
      /// </summary>
      private bool CanContinueDestructiveOp()
      {
         if (_instanceCollection.ChangedAfterSave)
         {
            DialogResult result = _messageBoxView.AskYesNoCancelQuestion(_view, 
               String.Format("There are changes to the configuration that have not been saved.  Would you like to save these changes?{0}{0}Yes - Continue and save the changes / No - Continue and do not save the changes / Cancel - Do not continue", Environment.NewLine),
               _view.Text);
               
            switch (result)
            {
               case DialogResult.Yes:
                  FileSaveClick();
                  return !(_instanceCollection.ChangedAfterSave);
               case DialogResult.No:
                  return true;
               case DialogResult.Cancel:
                  return false;
            }
            return false;
         }

         return true;
      }

      public void FileSaveClick()
      {
         if (_configurationManager.HasConfigFilename == false)
         {
            FileSaveAsClick();
         }
         else
         {
            try
            {
               _instanceCollection.WriteConfigFile();
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  _view.Text);
            }
         }
      }

      public void FileSaveAsClick()
      {
         // No Config File and no Instances, stub out
         if (_configurationManager.HasConfigFilename == false && _instanceCollection.HasInstances == false) return;

         _saveFileDialogView.DefaultExt = _configurationManager.ConfigFileExtension;
         _saveFileDialogView.Filter = _configurationManager.FileTypeFilters;
         if (_saveFileDialogView.ShowDialog() == DialogResult.OK)
         {
            try
            {
               _instanceCollection.WriteConfigFile(_saveFileDialogView.FileName, _saveFileDialogView.FilterIndex); // Issue 75
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The client configuration has not been saved.{0}{0}{1}", Environment.NewLine, ex.Message),
                  _view.Text);
            }
         }
      }

      private void LoadConfigFile(string filePath)
      {
         LoadConfigFile(filePath, 1);
      }

      private void LoadConfigFile(string filePath, int filterIndex)
      {
         try
         {
            // Read the config file
            _instanceCollection.ReadConfigFile(filePath, filterIndex);

            if (_instanceCollection.HasInstances == false)
            {
               _messageBoxView.ShowError(_view, "No client configurations were loaded from the given config file.", _view.Text);
            }
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
               "No client configurations were loaded from the given config file.{0}{0}{1}", Environment.NewLine, ex.Message),
               _view.Text);
         }
      }
      
      #endregion
   }
}
