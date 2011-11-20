/*
 * HFM.NET - Work Unit History Presenter
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
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Core;
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public class HistoryPresenter
   {
      //private const string CsvFilter = "CSV Files|*.csv";
   
      private readonly IPreferenceSet _prefs;
      private readonly IUnitInfoDatabase _database;
      private readonly IQueryParametersCollection _queryCollection;
      private readonly IHistoryView _view;
      private readonly IQueryView _queryView;
      //private readonly IProgressDialogView _unitImportView;
      //private readonly ICompletedUnitsFileReader _completedUnitsReader;
      private readonly IOpenFileDialogView _openFileView;
      private readonly ISaveFileDialogView _saveFileView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IHistoryPresenterModel _model;
      
      private IList<HistoryEntry> _currentHistoryEntries;

      public event EventHandler PresenterClosed;
      
      public int NumberOfQueries
      {
         get { return _queryCollection.Count; }
      }

      public HistoryPresenter(IPreferenceSet prefs, IUnitInfoDatabase database, IQueryParametersCollection queryCollection, IHistoryView view,
                              IQueryView queryView, // IProgressDialogView unitImportView, ICompletedUnitsFileReader completedUnitsReader, 
                              IOpenFileDialogView openFileView, ISaveFileDialogView saveFileView, IMessageBoxView messageBoxView, 
                              IHistoryPresenterModel model)
      {
         _prefs = prefs;
         _database = database;
         _queryCollection = queryCollection;
         _view = view;
         _queryView = queryView;
         //_unitImportView = unitImportView;
         //_completedUnitsReader = completedUnitsReader;
         _openFileView = openFileView;
         _saveFileView = saveFileView;
         _messageBoxView = messageBoxView;
         _model = model;

         //_unitImportView.OwnerWindow = _view;
         //_unitImportView.ProcessRunner = _completedUnitsReader;
         
         _currentHistoryEntries = new List<HistoryEntry>();
      }
      
      public void Initialize()
      {
         _view.AttachPresenter(this);
         _model.LoadPreferences();
         _view.DataBindModel(_model);
         _queryCollection.Sort();
         _view.QueryComboRefreshList(_queryCollection);
      }

      public void Show()
      {
         _view.Show();
         if (_view.WindowState.Equals(FormWindowState.Minimized))
         {
            _view.WindowState = FormWindowState.Normal;            
         }
         else
         {
            _view.BringToFront();
         }
      }

      public void Close()
      {
         if (PresenterClosed != null)
         {
            PresenterClosed(this, EventArgs.Empty);
         }
      }
      
      public void OnViewClosing()
      {
         // Save location and size data
         // RestoreBounds remembers normal position if minimized or maximized
         if (_view.WindowState == FormWindowState.Normal)
         {
            _model.FormLocation = _view.Location;
            _model.FormSize = _view.Size;
         }
         else
         {
            _model.FormLocation = _view.RestoreBounds.Location;
            _model.FormSize = _view.RestoreBounds.Size;
         }

         _model.FormColumns = _view.GetColumnSettings();
         _model.SavePreferences();
      }
      
      public void RefreshClicked()
      {
         SelectQuery(_view.QueryComboSelectedIndex);
      }
      
      public void AddQuery(QueryParameters parameters)
      {
         CheckQueryParametersForAddOrReplace(parameters);

         if (_queryCollection.FirstOrDefault(x => x.Name == parameters.Name) != null)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }
         
         _queryCollection.Add(parameters);
         _queryCollection.Sort();
         _queryCollection.Write();
         _view.QueryComboRefreshList(_queryCollection);
      }
      
      public void ReplaceQuery(int index, QueryParameters parameters)
      {
         if (index == 0)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot replace the '{0}' query.", QueryParameters.SelectAll));
         }

         CheckQueryParametersForAddOrReplace(parameters);

         var existing = _queryCollection.FirstOrDefault(x => x.Name == parameters.Name);
         if (existing != null && existing.Name != _queryCollection[index].Name)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }
         
         _queryCollection.RemoveAt(index);
         _queryCollection.Add(parameters);
         _queryCollection.Sort();
         _queryCollection.Write();
         _view.QueryComboRefreshList(_queryCollection);
      }
      
      private static void CheckQueryParametersForAddOrReplace(QueryParameters parameters)
      {
         if (parameters.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Query name cannot be '{0}'.", QueryParameters.SelectAll));
         }

         if (parameters.Fields.Count == 0)
         {
            throw new ArgumentException("No query fields defined.");
         }

         for (int i = 0; i < parameters.Fields.Count; i++)
         {
            if (parameters.Fields[i].Value == null)
            {
               throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Field index {0} must have a query value.", (i + 1)));
            }
         }
      }

      public void RemoveQuery(QueryParameters parameters)
      {
         if (parameters.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot remove '{0}' query.", QueryParameters.SelectAll));
         }

         _queryCollection.Remove(parameters);
         _queryCollection.Write();
         _view.QueryComboRefreshList(_queryCollection);
      }

      public void SelectQuery(int index)
      {
         if (index < 0 || index >= _queryCollection.Count)
         {
            throw new ArgumentOutOfRangeException("index");
         }

         _view.EditButtonEnabled = index != 0;
         _view.DeleteButtonEnabled = index != 0;

         _currentHistoryEntries = _database.QueryUnitData(_queryCollection[index], _model.ProductionView);
         var showEntries = _currentHistoryEntries;
         if (_model.ShowFirstChecked)
         {
            showEntries = _currentHistoryEntries.Take(_model.ShowEntriesValue).ToList();
         }
         else if (_model.ShowLastChecked)
         {
            showEntries = _currentHistoryEntries.Reverse().Take(_model.ShowEntriesValue).ToList();
         }
         _view.DataGridSetDataSource(_currentHistoryEntries.Count, showEntries);
         _view.ApplySort(_model.SortColumnName, _model.SortOrder);
      }
      
      public void NewQueryClick()
      {
         var query = new QueryParameters { Name = "* New Query *" };
         query.Fields.Add(new QueryField());
         _queryView.Query = query;
         
         bool showDialog = true;
         while (showDialog)
         {
            if (_queryView.ShowDialog(_view).Equals(DialogResult.OK))
            {
               try
               {
                  AddQuery(_queryView.Query);
                  _view.QueryComboSelectedIndex = _queryCollection.Count - 1;
                  showDialog = false;
               }
               catch (ArgumentException ex)
               {
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
            else
            {
               showDialog = false;
            }
         }
      }
      
      public void EditQueryClick()
      {
         _queryView.Query = _view.QueryComboSelectedValue.DeepCopy();

         bool showDialog = true;
         while (showDialog)
         {
            if (_queryView.ShowDialog(_view).Equals(DialogResult.OK))
            {
               try
               {
                  ReplaceQuery(_view.QueryComboSelectedIndex, _queryView.Query);
                  showDialog = false;
               }
               catch (ArgumentException ex)
               {
                  _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
               }
            }
            else
            {
               showDialog = false;
            }
         }
      }

      public void DeleteQueryClick()
      {
         var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?", Core.Application.NameAndVersion);
         if (result.Equals(DialogResult.Yes))
         {
            try
            {
               RemoveQuery(_view.QueryComboSelectedValue);
            }
            catch (ArgumentException ex)
            {
               _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
            }
         }
      }
      
      public void DeleteWorkUnitClick()
      {
         var entry = _view.DataGridSelectedHistoryEntry;
         if (entry == null)
         {
            _messageBoxView.ShowInformation(_view, "No work unit selected.", Core.Application.NameAndVersion);
         }
         else
         {
            var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?", Core.Application.NameAndVersion);
            if (result.Equals(DialogResult.Yes))
            {
               if (_database.DeleteUnitInfo(entry.ID) != 0)
               {
                  RefreshClicked();
               }
            }
         }
      }
      
      public void SaveSortSettings(string sortColumnName, SortOrder sortOrder)
      {
         _model.SortColumnName = sortColumnName;
         _model.SortOrder = sortOrder;
      }

      #region ImportCompletedUnits

      //public void ImportCompletedUnitsClick()
      //{
      //   _openFileView.Filter = CsvFilter;
      //   _openFileView.FileName = Constants.CompletedUnitsCsvFileName;
      //   _openFileView.InitialDirectory = _prefs.ApplicationDataFolderPath;
      //   if (_openFileView.ShowDialog(_view).Equals(DialogResult.OK))
      //   {
      //      _completedUnitsReader.CompletedUnitsFilePath = _openFileView.FileName;
            
      //      _unitImportView.UpdateMessage(String.Empty);
      //      _unitImportView.UpdateProgress(0);
      //      _unitImportView.Process();
      //      if (_unitImportView.ProcessRunner.Exception != null)
      //      {
      //         Exception ex = _unitImportView.ProcessRunner.Exception;
      //         HfmTrace.WriteToHfmConsole(ex);
      //         _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture, 
      //            "Import Failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message), 
      //            Core.Application.NameAndVersion);
                  
      //         return;
      //      }
      //      ShowImportResultDialog(_completedUnitsReader.Result);
      //      _database.ImportCompletedUnits(_completedUnitsReader.Result.Entries);
      //      _view.QueryComboSelectedIndex = 0;
      //   }
      //}
      
      //public void ShowImportResultDialog(CompletedUnitsReadResult result)
      //{
      //   if (result.ErrorLines.Count != 0)
      //   {
      //      if (_messageBoxView.AskYesNoQuestion(_view, GetImportResultMessage(result),
      //                                           Core.Application.NameAndVersion).Equals(DialogResult.Yes))
      //      {
      //         _saveFileView.Filter = CsvFilter;
      //         _saveFileView.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
      //         if (_saveFileView.ShowDialog(_view).Equals(DialogResult.OK))
      //         {
      //            SaveErrorLines(_saveFileView.FileName, result.ErrorLines);
      //         }
      //      }
      //   }
      //   else
      //   {
      //      _messageBoxView.ShowInformation(_view, GetImportResultMessage(result), Core.Application.NameAndVersion);
      //   }
      //}
      
      //private void SaveErrorLines(string filePath, IEnumerable<string> lines)
      //{
      //   try
      //   {
      //      _completedUnitsReader.WriteCompletedUnitErrorLines(filePath, lines);
      //   }
      //   catch (Exception ex)
      //   {
      //      HfmTrace.WriteToHfmConsole(ex);
      //      _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
      //         "Save Failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message),
      //         Core.Application.NameAndVersion);
      //   }
      //}
      
      //private static string GetImportResultMessage(CompletedUnitsReadResult result)
      //{
      //   var sb = new StringBuilder();
      //   sb.AppendFormat("{0} imported {1} unit result(s) successfully.", Constants.ApplicationName, result.Entries.Count);
      //   if (result.Duplicates != 0)
      //   {
      //      sb.AppendLine();
      //      sb.AppendLine();
      //      sb.AppendFormat("{0} result(s) are duplicates and were not imported.", result.Duplicates);
      //   }
      //   if (result.ErrorLines.Count != 0)
      //   {
      //      sb.AppendLine();
      //      sb.AppendLine();
      //      sb.AppendFormat("{0} result(s) could not be imported.  You may save these lines incapable of being imported to another file.  There you may edit the lines to fix the problems with the csv format.  Would you like to save those lines now?", result.ErrorLines.Count);
      //   }

      //   return sb.ToString();
      //}

      #endregion

      public static string[] GetQueryFieldColumnNames()
      {
         // Indexes Must Match QueryFieldName enum defined in Enumerations.cs
         var list = new List<string>();
         list.Add("ProjectID");
         list.Add("Run");
         list.Add("Clone");
         list.Add("Gen");
         list.Add("Instance Name");
         list.Add("Instance Path");
         list.Add("Username");
         list.Add("Team");
         list.Add("Core Version");
         list.Add("Frames Completed");
         list.Add("Frame Time");
         list.Add("Unit Result");
         list.Add("Download Date (UTC)");
         list.Add("Completion Date (UTC)");
         list.Add("Work Unit Name");
         list.Add("KFactor");
         list.Add("Core Name");
         list.Add("Total Frames");
         list.Add("Atoms");
         list.Add("Client Type");
         list.Add("PPD");
         list.Add("Credit");

         return list.ToArray();
      }
   }
}
