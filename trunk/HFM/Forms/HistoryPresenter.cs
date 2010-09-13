/*
 * HFM.NET - Work Unit History Presenter
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
using System.Globalization;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using harlam357.Windows.Forms;

using HFM.Framework;
using HFM.Instances;
using HFM.Instrumentation;
using HFM.Models;

namespace HFM.Forms
{
   public class HistoryPresenter
   {
      private const string CsvFilter = "CSV Files|*.csv";
   
      private readonly IPreferenceSet _prefs;
      private readonly IUnitInfoDatabase _database;
      private readonly IQueryParameterContainer _queryContainer;
      private readonly IHistoryView _view;
      private readonly IQueryView _queryView;
      private readonly IOpenFileDialogView _openFileView;
      private readonly ISaveFileDialogView _saveFileView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IHistoryPresenterModel _model;
      
      private IList<HistoryEntry> _currentHistoryEntries;

      public event EventHandler PresenterClosed;
      
      public int NumberOfQueries
      {
         get { return _queryContainer.QueryList.Count; }
      }
   
      public HistoryPresenter(IPreferenceSet prefs, IUnitInfoDatabase database, IQueryParameterContainer queryContainer, IHistoryView view,
                              IQueryView queryView, IOpenFileDialogView openFileView, ISaveFileDialogView saveFileView, IMessageBoxView messageBoxView,
                              IHistoryPresenterModel model)
      {
         _prefs = prefs;
         _database = database;
         _queryContainer = queryContainer;
         _view = view;
         _queryView = queryView;
         _openFileView = openFileView;
         _saveFileView = saveFileView;
         _messageBoxView = messageBoxView;
         _model = model;
         
         _currentHistoryEntries = new List<HistoryEntry>();
      }
      
      public void Initialize()
      {
         _view.AttachPresenter(this);
         _model.LoadPreferences();
         _view.DataBindModel(_model);
         _view.QueryComboRefreshList(_queryContainer.QueryList);
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
         _model.SavePreferences();
      }
      
      public void RefreshClicked()
      {
         SelectQuery(_view.QueryComboSelectedIndex);
      }
      
      public void AddQuery(QueryParameters parameters)
      {
         CheckQueryParametersForAddOrReplace(parameters);

         if (_queryContainer.QueryList.Find(x => x.Name == parameters.Name) != null)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }
         
         _queryContainer.QueryList.Add(parameters);
         _queryContainer.Write();
         _view.QueryComboRefreshList(_queryContainer.QueryList);
      }
      
      public void ReplaceQuery(int index, QueryParameters parameters)
      {
         if (index == 0)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture, "Cannot replace the '{0}' query.", QueryParameters.SelectAll));
         }

         CheckQueryParametersForAddOrReplace(parameters);

         var existing = _queryContainer.QueryList.Find(x => x.Name == parameters.Name);
         if (existing != null && existing.Name != _queryContainer.QueryList[index].Name)
         {
            throw new ArgumentException(String.Format(CultureInfo.CurrentCulture,
               "A query with name '{0}' already exists.", parameters.Name));
         }
         
         _queryContainer.QueryList.RemoveAt(index);
         _queryContainer.QueryList.Insert(index, parameters);
         _queryContainer.Write();
         _view.QueryComboRefreshList(_queryContainer.QueryList);
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

         _queryContainer.QueryList.Remove(parameters);
         _queryContainer.Write();
         _view.QueryComboRefreshList(_queryContainer.QueryList);
      }

      public void SelectQuery(int index)
      {
         if (index < 0 || index >= _queryContainer.QueryList.Count)
         {
            throw new ArgumentOutOfRangeException("index");
         }

         _view.EditButtonEnabled = index != 0;
         _view.DeleteButtonEnabled = index != 0;

         _currentHistoryEntries = _database.QueryUnitData(_queryContainer.QueryList[index], _model.ProductionView);
         var showEntries = _currentHistoryEntries;
         if (_model.ShowTopChecked)
         {
            showEntries = _currentHistoryEntries.Take(_model.ShowTopValue).ToList();
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
                  _view.QueryComboSelectedIndex = _queryContainer.QueryList.Count - 1;
                  showDialog = false;
               }
               catch (ArgumentException ex)
               {
                  _messageBoxView.ShowError(_view, ex.Message, PlatformOps.ApplicationNameAndVersion);
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
                  _messageBoxView.ShowError(_view, ex.Message, PlatformOps.ApplicationNameAndVersion);
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
         try
         {
            RemoveQuery(_view.QueryComboSelectedValue);
         }
         catch (ArgumentException ex)
         {
            _messageBoxView.ShowError(_view, ex.Message, PlatformOps.ApplicationNameAndVersion);
         }
      }
      
      public void SaveSortSettings(string sortColumnName, SortOrder sortOrder)
      {
         _model.SortColumnName = sortColumnName;
         _model.SortOrder = sortOrder;
      }

      public void ImportCompletedUnitsClick()
      {
         _openFileView.Filter = CsvFilter;
         _openFileView.FileName = Constants.CompletedUnitsCsvFileName;
         _openFileView.InitialDirectory = _prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);
         if (_openFileView.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               var result = _database.ReadCompletedUnits(_openFileView.FileName);
               ShowImportResultDialog(result);
               _database.ImportCompletedUnits(result.Entries);
               _view.QueryComboSelectedIndex = 0;
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture, 
                  "Import Failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message), 
                  PlatformOps.ApplicationNameAndVersion);
            }
         }
      }
      
      public void ShowImportResultDialog(CompletedUnitsReadResult result)
      {
         if (result.ErrorLines.Count != 0)
         {
            if (_messageBoxView.AskYesNoQuestion(_view, GetImportResultMessage(result),
                                                 PlatformOps.ApplicationNameAndVersion).Equals(DialogResult.Yes))
            {
               _saveFileView.Filter = CsvFilter;
               _saveFileView.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
               if (_saveFileView.ShowDialog(_view).Equals(DialogResult.OK))
               {
                  SaveErrorLines(_saveFileView.FileName, result.ErrorLines);
               }
            }
         }
         else
         {
            _messageBoxView.ShowInformation(_view, GetImportResultMessage(result), PlatformOps.ApplicationNameAndVersion);
         }
      }
      
      private void SaveErrorLines(string filePath, IEnumerable<string> lines)
      {
         try
         {
            _database.WriteCompletedUnitErrorLines(filePath, lines);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
               "Save Failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message),
               PlatformOps.ApplicationNameAndVersion);
         }
      }
      
      private static string GetImportResultMessage(CompletedUnitsReadResult result)
      {
         var sb = new StringBuilder();
         sb.AppendFormat("{0} imported {1} unit result(s) successfully.", Constants.ApplicationName, result.Entries.Count);
         if (result.Duplicates != 0)
         {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("{0} result(s) are duplicates and were not imported.", result.Duplicates);
         }
         if (result.ErrorLines.Count != 0)
         {
            sb.AppendLine();
            sb.AppendLine();
            sb.AppendFormat("{0} result(s) could not be imported.  You may save these lines incapable of being imported to another file.  There you may edit the lines to fix the problems with the csv format.  Would you like to save those lines now?", result.ErrorLines.Count);
         }

         return sb.ToString();
      }
   }
}
