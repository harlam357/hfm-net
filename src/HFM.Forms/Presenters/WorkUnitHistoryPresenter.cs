﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Data;
using HFM.Core.Logging;
using HFM.Core.Serializers;
using HFM.Core.WorkUnits;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Views;

using Microsoft.Extensions.DependencyInjection;

namespace HFM.Forms.Presenters
{
    public class WorkUnitHistoryPresenter : AsyncFormPresenter<WorkUnitHistoryModel>
    {
        public WorkUnitHistoryModel Model { get; }
        public ILogger Logger { get; }
        public IServiceScopeFactory ServiceScopeFactory { get; }
        public MessageBoxPresenter MessageBox { get; }
        public IProteinService ProteinService { get; }

        public WorkUnitHistoryPresenter(WorkUnitHistoryModel model, ILogger logger, IServiceScopeFactory serviceScopeFactory, MessageBoxPresenter messageBox, 
                                        IProteinService proteinService)
            : base(model)
        {
            Model = model;
            Logger = logger ?? NullLogger.Instance;
            ServiceScopeFactory = serviceScopeFactory ?? NullServiceScopeFactory.Instance;
            MessageBox = messageBox ?? NullMessageBoxPresenter.Instance;
            ProteinService = proteinService ?? NullProteinService.Instance;
        }

        public override async Task ShowAsync()
        {
            if (Form is null)
            {
                await Model.LoadAsync().ConfigureAwait(true);

                Form = OnCreateForm();
                Form.Closed += async (s, e) => await OnClosed(s, e).ConfigureAwait(true);
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

        public async void ExportClick(FileDialogPresenter saveFile)
        {
            await ExportClick(saveFile, new List<IFileSerializer<List<WorkUnitRow>>> { new WorkUnitRowCsvFileSerializer() });
        }

        internal async Task ExportClick(FileDialogPresenter saveFile, IList<IFileSerializer<List<WorkUnitRow>>> serializers)
        {
            saveFile.Filter = serializers.GetFileTypeFilters();
            if (saveFile.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    var serializer = serializers[saveFile.FilterIndex - 1];
                    var value = await Model.Repository.FetchAsync(Model.SelectedWorkUnitQuery, Model.BonusCalculation).ConfigureAwait(true);
                    serializer.Serialize(saveFile.FileName, value as List<WorkUnitRow> ?? value.ToList());
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

        public async Task DeleteWorkUnitClick()
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
                    await Model.DeleteHistoryEntry(entry).ConfigureAwait(true);
                }
            }
        }

        public void CopyPRCGToClipboardClicked()
        {
            if (Model.SelectedWorkUnitRow == null) return;

            string projectString = Model.SelectedWorkUnitRow.ToProjectString();

            // TODO: Replace ClipboardWrapper.SetText() with abstraction
            ClipboardWrapper.SetText(projectString);
        }
    }
}
