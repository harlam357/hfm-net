/*
 * HFM.NET - Work Unit History Presenter
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using HFM.Core.DataTypes;
using HFM.Forms.Models;

namespace HFM.Forms
{
   public class HistoryPresenter
   {
      private readonly IPreferenceSet _prefs;
      private readonly IQueryParametersCollection _queryCollection;
      private readonly IHistoryView _view;
      private readonly IQueryView _queryView;
      private readonly IOpenFileDialogView _openFileView;
      private readonly ISaveFileDialogView _saveFileView;
      private readonly IMessageBoxView _messageBoxView;
      private readonly HistoryPresenterModel _model;
      
      public event EventHandler PresenterClosed;
      
      public HistoryPresenter(IPreferenceSet prefs, IQueryParametersCollection queryCollection, IHistoryView view, 
                              IQueryView queryView, IOpenFileDialogView openFileView, ISaveFileDialogView saveFileView, 
                              IMessageBoxView messageBoxView, HistoryPresenterModel model)
      {
         _prefs = prefs;
         _queryCollection = queryCollection;
         _view = view;
         _queryView = queryView;
         _openFileView = openFileView;
         _saveFileView = saveFileView;
         _messageBoxView = messageBoxView;
         _model = model;
      }
      
      public void Initialize()
      {
         _view.AttachPresenter(this);
         _model.Load(_prefs, _queryCollection);
         _view.DataBindModel(_model);
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
         _model.Update(_prefs, _queryCollection);
      }
      
      public void RefreshClicked()
      {
         _model.ResetBindings(true);
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
                  _model.AddQuery(_queryView.Query);
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
         _queryView.Query = _model.SelectedQuery.DeepClone();

         bool showDialog = true;
         while (showDialog)
         {
            if (_queryView.ShowDialog(_view).Equals(DialogResult.OK))
            {
               try
               {
                  _model.ReplaceQuery(_queryView.Query);
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
               _model.RemoveQuery(_model.SelectedQuery);
            }
            catch (ArgumentException ex)
            {
               _messageBoxView.ShowError(_view, ex.Message, Core.Application.NameAndVersion);
            }
         }
      }
      
      public void DeleteWorkUnitClick()
      {
         var entry = _model.SelectedHistoryEntry;
         if (entry == null)
         {
            _messageBoxView.ShowInformation(_view, "No work unit selected.", Core.Application.NameAndVersion);
         }
         else
         {
            var result = _messageBoxView.AskYesNoQuestion(_view, "Are you sure?", Core.Application.NameAndVersion);
            if (result.Equals(DialogResult.Yes))
            {
               _model.DeleteHistoryEntry(entry);
            }
         }
      }
      
      public static string[] GetQueryFieldColumnNames()
      {
         // Indexes Must Match QueryFieldName enum defined in Enumerations.cs
         var list = new List<string>();
         list.Add("ProjectID");
         list.Add("Run");
         list.Add("Clone");
         list.Add("Gen");
         list.Add("Name");
         list.Add("Path");
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
         list.Add("Slot Type");
         list.Add("PPD");
         list.Add("Credit");

         return list.ToArray();
      }
   }
}
