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

using harlam357.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Serializers;
using HFM.Forms.Models;
using HFM.Preferences;

namespace HFM.Forms
{
    public class HistoryPresenter
    {
        public ILogger Logger { get; }
        public IPreferenceSet Preferences { get; }
        public WorkUnitQueryDataContainer QueryDataContainer { get; }
        public IHistoryView HistoryView { get; }
        public IViewFactory ViewFactory { get; }
        public IMessageBoxView MessageBoxView { get; }
        public IWorkUnitRepository WorkUnitRepository { get; }
        public HistoryPresenterModel Model { get; }

        public event EventHandler PresenterClosed;

        public HistoryPresenter(ILogger logger,
                                IPreferenceSet preferences,
                                WorkUnitQueryDataContainer queryDataContainer,
                                IHistoryView historyView,
                                IViewFactory viewFactory,
                                IMessageBoxView messageBoxView,
                                IWorkUnitRepository workUnitRepository)
        {
            Logger = logger;
            Preferences = preferences;
            QueryDataContainer = queryDataContainer;
            HistoryView = historyView;
            ViewFactory = viewFactory;
            MessageBoxView = messageBoxView;
            WorkUnitRepository = workUnitRepository;
            Model = new HistoryPresenterModel(workUnitRepository);
        }

        public void Initialize()
        {
            HistoryView.AttachPresenter(this);
            Model.Load(Preferences, QueryDataContainer);
            HistoryView.DataBindModel(Model);
        }

        public void Show()
        {
            HistoryView.Show();
            if (HistoryView.WindowState == FormWindowState.Minimized)
            {
                HistoryView.WindowState = FormWindowState.Normal;
            }
            else
            {
                HistoryView.BringToFront();
            }
        }

        public void Close()
        {
            PresenterClosed?.Invoke(this, EventArgs.Empty);
        }

        public void ViewClosing()
        {
            // Save location and size data
            // RestoreBounds remembers normal position if minimized or maximized
            if (HistoryView.WindowState == FormWindowState.Normal)
            {
                Model.FormLocation = HistoryView.Location;
                Model.FormSize = HistoryView.Size;
            }
            else
            {
                Model.FormLocation = HistoryView.RestoreBounds.Location;
                Model.FormSize = HistoryView.RestoreBounds.Size;
            }

            Model.FormColumns = HistoryView.GetColumnSettings();
            Model.Update(Preferences, QueryDataContainer);
        }

        internal void ExportClick()
        {
            ExportClick(new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowCsvFileSerializer() });
        }

        internal void ExportClick(IList<IFileSerializer<List<WorkUnitRow>>> serializers)
        {
            var saveFileDialogView = ViewFactory.GetSaveFileDialogView();
            saveFileDialogView.Filter = serializers.GetFileTypeFilters();
            if (saveFileDialogView.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var serializer = serializers[saveFileDialogView.FilterIndex - 1];
                    serializer.Serialize(saveFileDialogView.FileName, Model.Repository.Fetch(Model.SelectedWorkUnitQuery, Model.BonusCalculation).ToList());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    MessageBoxView.ShowError(HistoryView, String.Format(CultureInfo.CurrentCulture,
                       "The history data export failed.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
                }
            }
            ViewFactory.Release(saveFileDialogView);
        }

        public void FirstPageClicked()
        {
            Model.CurrentPage = 1;
        }

        public void PreviousPageClicked()
        {
            Model.CurrentPage -= 1;
        }

        public void NextPageClicked()
        {
            Model.CurrentPage += 1;
        }

        public void LastPageClicked()
        {
            Model.CurrentPage = Model.TotalPages;
        }

        public void NewQueryClick()
        {
            var queryView = ViewFactory.GetQueryDialog();
            var query = new WorkUnitQuery("* New Query *")
                .AddParameter(new WorkUnitQueryParameter());
            queryView.Query = query;

            bool showDialog = true;
            while (showDialog)
            {
                if (queryView.ShowDialog(HistoryView) == DialogResult.OK)
                {
                    try
                    {
                        Model.AddQuery(queryView.Query);
                        showDialog = false;
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBoxView.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
                    }
                }
                else
                {
                    showDialog = false;
                }
            }
            ViewFactory.Release(queryView);
        }

        public void EditQueryClick()
        {
            var queryView = ViewFactory.GetQueryDialog();
            queryView.Query = Model.SelectedWorkUnitQuery.DeepClone();

            bool showDialog = true;
            while (showDialog)
            {
                if (queryView.ShowDialog(HistoryView) == DialogResult.OK)
                {
                    try
                    {
                        Model.ReplaceQuery(queryView.Query);
                        showDialog = false;
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBoxView.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
                    }
                }
                else
                {
                    showDialog = false;
                }
            }
            ViewFactory.Release(queryView);
        }

        public void DeleteQueryClick()
        {
            var result = MessageBoxView.AskYesNoQuestion(HistoryView, "Are you sure?", Core.Application.NameAndVersion);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Model.RemoveQuery(Model.SelectedWorkUnitQuery);
                }
                catch (ArgumentException ex)
                {
                    MessageBoxView.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
                }
            }
        }

        public void DeleteWorkUnitClick()
        {
            var entry = Model.SelectedWorkUnitRow;
            if (entry == null)
            {
                MessageBoxView.ShowInformation(HistoryView, "No work unit selected.", Core.Application.NameAndVersion);
            }
            else
            {
                var result = MessageBoxView.AskYesNoQuestion(HistoryView, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
                if (result == DialogResult.Yes)
                {
                    Model.DeleteHistoryEntry(entry);
                }
            }
        }

        public async void RefreshAllProjectDataClick()
        {
            await RefreshProjectData(WorkUnitProteinUpdateScope.All);
        }

        public async void RefreshUnknownProjectDataClick()
        {
            await RefreshProjectData(WorkUnitProteinUpdateScope.Unknown);
        }

        public async void RefreshDataByProjectClick()
        {
            await RefreshProjectData(WorkUnitProteinUpdateScope.Project);
        }

        public async void RefreshDataByIdClick()
        {
            await RefreshProjectData(WorkUnitProteinUpdateScope.Id);
        }

        private async Task RefreshProjectData(WorkUnitProteinUpdateScope scope)
        {
            var result = MessageBoxView.AskYesNoQuestion(HistoryView, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
            if (result == DialogResult.No)
            {
                return;
            }

            long arg = 0;
            if (scope == WorkUnitProteinUpdateScope.Project)
            {
                arg = Model.SelectedWorkUnitRow.ProjectID;
            }
            else if (scope == WorkUnitProteinUpdateScope.Id)
            {
                arg = Model.SelectedWorkUnitRow.ID;
            }

            try
            {
                bool updated = await WorkUnitRepository.UpdateProteinDataAsync(scope, arg).ConfigureAwait(false);
                if (updated)
                {
                    Model.ResetBindings(true);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBoxView.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
            }
        }
    }
}
