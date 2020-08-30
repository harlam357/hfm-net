using System;
using System.ComponentModel;
using System.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class ApplicationUpdateDialog : FormBase, IWin32Dialog
    {
        private readonly ApplicationUpdatePresenter _presenter;

        public ApplicationUpdateDialog(ApplicationUpdatePresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            EscapeKeyButton(cancelButton);
        }

        private void ApplicationUpdateDialog_Load(object sender, EventArgs e)
        {
            // some how leaving the ControlBox enabled in
            // the Designer allows the Form to scale when
            // run under a higher DPI setting, just turn
            // it off once the Form is loaded
            ControlBox = false;
            LoadData(_presenter.Model);
        }

        private const string SelectedValuePropertyName = "SelectedValue";

        private void LoadData(ApplicationUpdateModel model)
        {
            captionLabel.Text = $"A new version of {Core.Application.Name} is available for download.";
            thisVersionLabel.Text = $"This version: {Core.Application.FullVersion}";
            newVersionLabel.Text = $"New version: {model.Update.Version}";

            model.PropertyChanged += ModelPropertyChanged;

            updateFilesComboBox.DataSource = model.UpdateFilesList;
            updateFilesComboBox.DisplayMember = nameof(ListItem.DisplayMember);
            updateFilesComboBox.ValueMember = nameof(ListItem.ValueMember);
            updateFilesComboBox.DataBindings.Add(SelectedValuePropertyName, model, nameof(ApplicationUpdateModel.SelectedUpdateFile), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private async void downloadButton_Click(object sender, EventArgs e)
        {
            using (var saveFile = DefaultFileDialogPresenter.SaveFile())
            {
                await _presenter.DownloadClick(saveFile).ConfigureAwait(true);
            }
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            _presenter.CancelClick();
        }

        private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed) return;

            var model = _presenter.Model;
            switch (e.PropertyName)
            {
                case nameof(ApplicationUpdateModel.DownloadInProgress):
                    if (model.DownloadInProgress)
                    {
                        SetViewControlsForDownload(model);
                    }
                    else
                    {
                        SetViewControlsForUpdateSelection();
                    }
                    break;
            }
        }

        private void SetViewControlsForDownload(ApplicationUpdateModel model)
        {
            downloadProgressLabel.Text = $"Downloading {model.SelectedUpdateFile.Name}...";
            downloadButton.Enabled = false;
            updateFilesComboBox.Visible = false;
            downloadProgressBar.Visible = true;
        }

        private void SetViewControlsForUpdateSelection()
        {
            downloadProgressLabel.Text = "Please select an update to download.";
            downloadButton.Enabled = true;
            updateFilesComboBox.Visible = true;
            downloadProgressBar.Visible = false;
        }
    }
}
