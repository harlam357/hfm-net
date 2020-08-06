using System;
using System.ComponentModel;
using System.Windows.Forms;

using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class ApplicationUpdateDialog : Form, IWin32Dialog
    {
        private readonly ApplicationUpdatePresenter _presenter;

        public ApplicationUpdateDialog(ApplicationUpdatePresenter presenter)
        {
            _presenter = presenter;
            _presenter.Model.PropertyChanged += ModelOnPropertyChanged;

            InitializeComponent();
        }

        private void ApplicationUpdateDialog_Load(object sender, EventArgs e)
        {
            // some how leaving the ControlBox enabled in
            // the Designer allows the Form to scale when
            // run under a higher DPI setting, just turn
            // it off once the Form is loaded
            ControlBox = false;
            LoadData();
        }

        private const string SelectedValuePropertyName = "SelectedValue";

        private void LoadData()
        {
            captionLabel.Text = $"A new version of {Core.Application.Name} is available for download.";
            thisVersionLabel.Text = $"This version: {Core.Application.FullVersion}";
            newVersionLabel.Text = $"New version: {_presenter.Model.Update.Version}";

            updateFilesComboBox.DataSource = _presenter.Model.UpdateFilesList;
            updateFilesComboBox.DisplayMember = nameof(ListItem.DisplayMember);
            updateFilesComboBox.ValueMember = nameof(ListItem.ValueMember);
            updateFilesComboBox.DataBindings.Add(SelectedValuePropertyName, _presenter.Model, nameof(ApplicationUpdateModel.SelectedUpdateFile), false, DataSourceUpdateMode.OnPropertyChanged);
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

        private void ModelOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (IsDisposed) return;

            switch (e.PropertyName)
            {
                case nameof(ApplicationUpdateModel.DownloadInProgress):
                    if (_presenter.Model.DownloadInProgress)
                    {
                        SetViewControlsForDownload();
                    }
                    else
                    {
                        SetViewControlsForUpdateSelection();
                    }
                    break;
            }
        }

        private void SetViewControlsForDownload()
        {
            downloadProgressLabel.Text = $"Downloading {_presenter.Model.SelectedUpdateFile.Name}...";
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
