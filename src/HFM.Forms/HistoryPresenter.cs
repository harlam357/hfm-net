
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

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
        public MessageBoxPresenter MessageBox { get; }
        public IWorkUnitRepository WorkUnitRepository { get; }
        public HistoryPresenterModel Model { get; }

        public event EventHandler PresenterClosed;

        public HistoryPresenter(ILogger logger,
                                IPreferenceSet preferences,
                                WorkUnitQueryDataContainer queryDataContainer,
                                IHistoryView historyView,
                                IViewFactory viewFactory,
                                MessageBoxPresenter messageBox,
                                IWorkUnitRepository workUnitRepository)
        {
            Logger = logger;
            Preferences = preferences;
            QueryDataContainer = queryDataContainer;
            HistoryView = historyView;
            ViewFactory = viewFactory;
            MessageBox = messageBox;
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

        public void ExportClick(FileDialogPresenter saveFile)
        {
            ExportClick(saveFile, new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowCsvFileSerializer() });
        }

        internal void ExportClick(FileDialogPresenter saveFile, IList<IFileSerializer<List<WorkUnitRow>>> serializers)
        {
            saveFile.Filter = serializers.GetFileTypeFilters();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var serializer = serializers[saveFile.FilterIndex - 1];
                    serializer.Serialize(saveFile.FileName, Model.Repository.Fetch(Model.SelectedWorkUnitQuery, Model.BonusCalculation).ToList());
                }
                catch (Exception ex)
                {
                    Logger.Error(ex.Message, ex);
                    MessageBox.ShowError(HistoryView, String.Format(CultureInfo.CurrentCulture,
                       "The history data export failed.{0}{0}{1}", Environment.NewLine, ex.Message), Core.Application.NameAndVersion);
                }
            }
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
                        MessageBox.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
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
                        MessageBox.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
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
            var result = MessageBox.AskYesNoQuestion(HistoryView, "Are you sure?", Core.Application.NameAndVersion);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Model.RemoveQuery(Model.SelectedWorkUnitQuery);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
                }
            }
        }

        public void DeleteWorkUnitClick()
        {
            var entry = Model.SelectedWorkUnitRow;
            if (entry == null)
            {
                MessageBox.ShowInformation(HistoryView, "No work unit selected.", Core.Application.NameAndVersion);
            }
            else
            {
                var result = MessageBox.AskYesNoQuestion(HistoryView, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
                if (result == DialogResult.Yes)
                {
                    Model.DeleteHistoryEntry(entry);
                }
            }
        }

        public void RefreshAllProjectDataClick()
        {
            RefreshProjectData(WorkUnitProteinUpdateScope.All);
        }

        public void RefreshUnknownProjectDataClick()
        {
            RefreshProjectData(WorkUnitProteinUpdateScope.Unknown);
        }

        public void RefreshDataByProjectClick()
        {
            RefreshProjectData(WorkUnitProteinUpdateScope.Project);
        }

        public void RefreshDataByIdClick()
        {
            RefreshProjectData(WorkUnitProteinUpdateScope.Id);
        }

        private void RefreshProjectData(WorkUnitProteinUpdateScope scope)
        {
            var result = MessageBox.AskYesNoQuestion(HistoryView, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
            if (result == DialogResult.No)
            {
                return;
            }

            long id = 0;
            if (scope == WorkUnitProteinUpdateScope.Project)
            {
                id = Model.SelectedWorkUnitRow.ProjectID;
            }
            else if (scope == WorkUnitProteinUpdateScope.Id)
            {
                id = Model.SelectedWorkUnitRow.ID;
            }

            var proteinDataUpdater = new ProteinDataUpdater(WorkUnitRepository);

            try
            {
                using (var dialog = new ProgressDialogAsync((progress, token) => proteinDataUpdater.Execute(progress, token, scope, id), true))
                {
                    dialog.Icon = Properties.Resources.hfm_48_48;
                    dialog.Text = Core.Application.NameAndVersion;
                    dialog.ShowDialog(HistoryView);
                    if (dialog.Exception != null)
                    {
                        Logger.Error(dialog.Exception.Message, dialog.Exception);
                        MessageBox.ShowError(HistoryView, dialog.Exception.Message, Core.Application.NameAndVersion);
                    }
                }

                Model.ResetBindings(true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(HistoryView, ex.Message, Core.Application.NameAndVersion);
            }
        }
    }
}
