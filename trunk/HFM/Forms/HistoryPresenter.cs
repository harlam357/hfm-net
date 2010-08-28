
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;
using harlam357.Windows.Forms;
using HFM.Framework;
using HFM.Instances;
using HFM.Instrumentation;

namespace HFM.Forms
{
   public class HistoryPresenter
   {
      private const string CsvFilter = "CSV Files|*.csv";
   
      private readonly IPreferenceSet _prefs;
      private readonly IUnitInfoDatabase _database;
      private readonly IQueryParameterContainer _queryContainer;
      private readonly IHistoryView _view;
      private readonly IOpenFileDialogView _openFileView;
      private readonly ISaveFileDialogView _saveFileView;
      private readonly IMessageBoxView _messageBoxView;
      
      public int NumberOfQueries
      {
         get { return _queryContainer.QueryList.Count; }
      }
   
      public HistoryPresenter(IPreferenceSet prefs, IUnitInfoDatabase database, IQueryParameterContainer queryContainer, IHistoryView view,
                              IOpenFileDialogView openFileView, ISaveFileDialogView saveFileView, IMessageBoxView messageBoxView)
      {
         _prefs = prefs;
         _database = database;
         _queryContainer = queryContainer;
         _view = view;
         _openFileView = openFileView;
         _saveFileView = saveFileView;
         _messageBoxView = messageBoxView;
      }

      public void Initialize()
      {
         _database.DatabaseFilePath = Path.Combine(_prefs.GetPreference<string>(Preference.ApplicationDataFolderPath), UnitInfoDatabase.SqLiteFilename);
         _view.AttachPresenter(this);
         _view.QueryComboRefreshList(_queryContainer.QueryList);
      }

      public void Show()
      {
         _view.Show();
      }

      public void Close()
      {
         _view.Close();
      }
      
      public void AddQuery(QueryParameters parameters)
      {
         if (parameters.Name == QueryParameters.SelectAll)
         {
            throw new ArgumentException("Parameters cannot be Select All.");
         }

         _queryContainer.QueryList.Add(parameters);
         _queryContainer.Write();
         _view.QueryComboRefreshList(_queryContainer.QueryList);
      }
      
      public void RemoveQuery(string name)
      {
         if (name == QueryParameters.SelectAll)
         {
            throw new ArgumentException("Cannot remove Select All query.");
         }

         int index = _queryContainer.QueryList.FindIndex(x => x.Name == name);
         if (index >= 0)
         {
            _queryContainer.QueryList.RemoveAt(index);
            _queryContainer.Write();
            _view.QueryComboRefreshList(_queryContainer.QueryList);
         }
      }

      public void QueryComboIndexChanged(int index)
      {
         if (index < 0 || index >= _queryContainer.QueryList.Count)
         {
            throw new ArgumentOutOfRangeException("index");
         }

         var entries = _database.QueryUnitData(_queryContainer.QueryList[index]);
         _view.DataGridSetDataSource(entries);
      }

      public CompletedUnitsReadResult ImportCompletedUnitsClick()
      {
         _openFileView.Filter = CsvFilter;
         _openFileView.FileName = Constants.CompletedUnitsCsvFileName;
         _openFileView.InitialDirectory = _prefs.GetPreference<string>(Preference.ApplicationDataFolderPath);
         if (_openFileView.ShowDialog(_view).Equals(DialogResult.OK))
         {
            try
            {
               var result = _database.ReadCompletedUnits(_openFileView.FileName);
               _database.ImportCompletedUnits(result.Entries);
               return result;
            }
            catch (Exception ex)
            {
               HfmTrace.WriteToHfmConsole(ex);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture, 
                  "Import Failed with the following error:{0}{0}{1}", Environment.NewLine, ex.Message), 
                  PlatformOps.ApplicationNameAndVersion);
            }
         }

         return null;
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
