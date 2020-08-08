using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Serializers;
using HFM.Forms.Models;
using HFM.Forms.Views;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Presenters
{
    public class WorkUnitHistoryPresenter : FormPresenter
    {
        public WorkUnitHistoryModel Model { get; }
        public ILogger Logger { get; }
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public MessageBoxPresenter MessageBox { get; }

        public WorkUnitHistoryPresenter(WorkUnitHistoryModel model, ILogger logger, IServiceScopeFactory serviceScopeFactory, MessageBoxPresenter messageBox)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            ServiceScopeFactory = serviceScopeFactory ?? NullServiceScopeFactory.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
        }

        public override void Show()
        {
            if (Form is null)
            {
                Model.Load();

                var form = OnCreateForm();
                form.Closed += OnClosed;
                Form = form;
            }

            Form.Show();
            if (Form.WindowState == FormWindowState.Minimized)
            {
                Form.WindowState = FormWindowState.Normal;
            }
            else
            {
                Form.BringToFront();
            }
        }

        protected override IWin32Form OnCreateForm()
        {
            return new WorkUnitHistoryForm(this);
        }

        protected override void OnClosed(object sender, EventArgs e)
        {
            Model.Save();
            base.OnClosed(sender, e);
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
                    MessageBox.ShowError(Form, String.Format(CultureInfo.CurrentCulture,
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

        public void NewQueryClick(WorkUnitQueryPresenter presenter)
        {
            presenter.Query.Name = WorkUnitQuery.NewQueryName;
            presenter.Query.Parameters.Add(new WorkUnitQueryParameter());

            bool showDialog = true;
            while (showDialog)
            {
                if (presenter.ShowDialog(Form) == DialogResult.OK)
                {
                    try
                    {
                        Model.AddQuery(presenter.Query);
                        showDialog = false;
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
                    }
                }
                else
                {
                    showDialog = false;
                }
            }
        }

        public void EditQueryClick(WorkUnitQueryPresenter presenter)
        {
            presenter.Query = Model.SelectedWorkUnitQuery.DeepClone();

            bool showDialog = true;
            while (showDialog)
            {
                if (presenter.ShowDialog(Form) == DialogResult.OK)
                {
                    try
                    {
                        Model.ReplaceQuery(presenter.Query);
                        showDialog = false;
                    }
                    catch (ArgumentException ex)
                    {
                        MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
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
            var result = MessageBox.AskYesNoQuestion(Form, "Are you sure?", Core.Application.NameAndVersion);
            if (result == DialogResult.Yes)
            {
                try
                {
                    Model.RemoveQuery(Model.SelectedWorkUnitQuery);
                }
                catch (ArgumentException ex)
                {
                    MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
                }
            }
        }

        public void DeleteWorkUnitClick()
        {
            var entry = Model.SelectedWorkUnitRow;
            if (entry == null)
            {
                MessageBox.ShowInformation(Form, "No work unit selected.", Core.Application.NameAndVersion);
            }
            else
            {
                var result = MessageBox.AskYesNoQuestion(Form, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
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
            var result = MessageBox.AskYesNoQuestion(Form, "Are you sure?  This operation cannot be undone.", Core.Application.NameAndVersion);
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

            var proteinDataUpdater = new ProteinDataUpdater(Model.Repository);

            try
            {
                using (var dialog = new ProgressDialog((progress, token) => proteinDataUpdater.Execute(progress, token, scope, id), true))
                {
                    dialog.Icon = Properties.Resources.hfm_48_48;
                    dialog.Text = Core.Application.NameAndVersion;
                    dialog.ShowDialog(Form);
                    if (dialog.Exception != null)
                    {
                        Logger.Error(dialog.Exception.Message, dialog.Exception);
                        MessageBox.ShowError(Form, dialog.Exception.Message, Core.Application.NameAndVersion);
                    }
                }

                Model.ResetBindings(true);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, ex);
                MessageBox.ShowError(Form, ex.Message, Core.Application.NameAndVersion);
            }
        }
    }
}
