/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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
using System.Threading.Tasks;
using System.Windows.Forms;

using Castle.Core.Logging;

using harlam357.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Serializers;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
   public class HistoryPresenter
   {
      private readonly IPreferenceSet _prefs;
      private readonly WorkUnitHistoryQueryDataContainer _queryContainer;
      private readonly IHistoryView _view;
      private readonly IViewFactory _viewFactory;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IUnitInfoDatabase _database;
      private readonly HistoryPresenterModel _model;

      private ILogger _logger;

      public ILogger Logger
      {
         get { return _logger ?? (_logger = NullLogger.Instance); }
         set { _logger = value; }
      }

      public event EventHandler PresenterClosed;
      
      public HistoryPresenter(IPreferenceSet prefs, 
                              WorkUnitHistoryQueryDataContainer queryContainer, 
                              IHistoryView view, 
                              IViewFactory viewFactory, 
                              IMessageBoxView messageBoxView, 
                              IUnitInfoDatabase database,
                              HistoryPresenterModel model)
      {
         _prefs = prefs;
         _queryContainer = queryContainer;
         _view = view;
         _viewFactory = viewFactory;
         _messageBoxView = messageBoxView;
         _database = database;
         _model = model;
      }
      
      public void Initialize()
      {
         _view.AttachPresenter(this);
         _model.Load(_prefs, _queryContainer);
         _view.DataBindModel(_model);
      }

      public void Show()
      {
         _view.Show();
         if (_view.WindowState == FormWindowState.Minimized)
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
         var handler = PresenterClosed;
         if (handler != null)
         {
            handler(this, EventArgs.Empty);
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
         _model.Update(_prefs, _queryContainer);
      }

      internal IList<IFileSerializer<List<WorkUnitHistoryRow>>> ExportSerializers { get; set; }

      internal void ExportClick()
      {
         var serializers = ExportSerializers ?? new List <IFileSerializer<List<WorkUnitHistoryRow>>>
         {
            new WorkUnitHistoryRowCsvFileSerializer()
         };

         var saveFileDialogView = _viewFactory.GetSaveFileDialogView();
         saveFileDialogView.Filter = serializers.GetFileTypeFilters();
         if (saveFileDialogView.ShowDialog() == DialogResult.OK)
         {
            try
            {
               var serializer = serializers[saveFileDialogView.FilterIndex - 1];
               serializer.Serialize(saveFileDialogView.FileName, _model.FetchSelectedQuery().ToList());
            }
            catch (Exception ex)
            {
               Logger.ErrorFormat(ex, "{0}", ex.Message);
               _messageBoxView.ShowError(_view, String.Format(CultureInfo.CurrentCulture,
                  "The history data export failed.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
            }
         }
         _viewFactory.Release(saveFileDialogView);
      }

      public void FirstPageClicked()
      {
         _model.CurrentPage = 1;
      }

      public void PreviousPageClicked()
      {
         _model.CurrentPage -= 1;
      }

      public void NextPageClicked()
      {
         _model.CurrentPage += 1;
      }

      public void LastPageClicked()
      {
         _model.CurrentPage = _model.TotalPages;
      }
      
      public void NewQueryClick()
      {
         var queryView = _viewFactory.GetQueryDialog();
         var query = new WorkUnitHistoryQuery("* New Query *")
             .AddParameter(new WorkUnitHistoryQueryParameter());
         queryView.Query = query;
         
         bool showDialog = true;
         while (showDialog)
         {
            if (queryView.ShowDialog(_view) == DialogResult.OK)
            {
               try
               {
                  _model.AddQuery(queryView.Query);
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
         _viewFactory.Release(queryView);
      }
      
      public void EditQueryClick()
      {
         var queryView = _viewFactory.GetQueryDialog();
         queryView.Query = _model.SelectedWorkUnitHistoryQuery.DeepClone();

         bool showDialog = true;
         while (showDialog)
         {
            if (queryView.ShowDialog(_view) == DialogResult.OK)
            {
               try
               {
                  _model.ReplaceQuery(queryView.Query);
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
         _viewFactory.Release(queryView);
      }

      public void DeleteQueryClick()
      {
         var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?", Core.Application.NameAndVersion);
         if (result == DialogResult.Yes)
         {
            try
            {
               _model.RemoveQuery(_model.SelectedWorkUnitHistoryQuery);
            }
            catch (ArgumentException ex)
            {
               _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
            }
         }
      }
      
      public void DeleteWorkUnitClick()
      {
         var entry = _model.SelectedWorkUnitHistoryRow;
         if (entry == null)
         {
            _messageBoxView.ShowInformation(_view, "No work unit selected.", Core.Application.NameAndVersion);
         }
         else
         {
            var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
            if (result == DialogResult.Yes)
            {
               _model.DeleteHistoryEntry(entry);
            }
         }
      }

      public async void RefreshAllProjectDataClick()
      {
         await RefreshProjectData(WorkUnitHistoryProteinUpdateScope.All);
      }

      public async void RefreshUnknownProjectDataClick()
      {
         await RefreshProjectData(WorkUnitHistoryProteinUpdateScope.Unknown);
      }

      public async void RefreshDataByProjectClick()
      {
         await RefreshProjectData(WorkUnitHistoryProteinUpdateScope.Project);
      }

      public async void RefreshDataByIdClick()
      {
         await RefreshProjectData(WorkUnitHistoryProteinUpdateScope.Id);
      }

      private async Task RefreshProjectData(WorkUnitHistoryProteinUpdateScope scope)
      {
         var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
         if (result == DialogResult.No)
         {
            return;
         }

         long arg = 0;
         if (scope == WorkUnitHistoryProteinUpdateScope.Project)
         {
            arg = _model.SelectedWorkUnitHistoryRow.ProjectID;
         }
         else if (scope == WorkUnitHistoryProteinUpdateScope.Id)
         {
            arg = _model.SelectedWorkUnitHistoryRow.ID;
         }

         try
         {
            bool updated = await _database.UpdateProteinDataAsync(scope, arg).ConfigureAwait(false);
            if (updated)
            {
               _model.ResetBindings(true);
            }
         }
         catch (Exception ex)
         {
            Logger.Error(ex.Message, ex);
            _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
         }
      }
   }
}
