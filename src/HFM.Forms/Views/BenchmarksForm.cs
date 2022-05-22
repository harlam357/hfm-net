using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Services;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class BenchmarksForm : FormBase, IWin32Form
    {
        private readonly BenchmarksPresenter _presenter;

        public BenchmarksForm(BenchmarksPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
            EscapeKeyReturnsCancelDialogResult();
        }

        private void BenchmarksForm_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
        }

        private void LoadData(BenchmarksModel model)
        {
            var (location, size) = WindowPosition.Normalize(this, model.FormLocation, model.FormSize);

            Location = location;
            LocationChanged += (s, e) => model.FormLocation = WindowState == FormWindowState.Normal ? Location : RestoreBounds.Location;
            Size = size;
            SizeChanged += (s, e) => model.FormSize = WindowState == FormWindowState.Normal ? Size : RestoreBounds.Size;

            cboClients.BindSelectedValue(model, nameof(BenchmarksModel.SelectedSlotIdentifier));
            cboClients.DataSource = model.SlotIdentifiers;
            cboClients.DisplayMember = nameof(ListItem.DisplayMember);
            cboClients.ValueMember = nameof(ListItem.ValueMember);

            ProjectIDTextBox.BindText(model, nameof(BenchmarksModel.WorkUnitName));
            CreditTextBox.BindText(model, nameof(BenchmarksModel.Credit));
            KFactorTextBox.BindText(model, nameof(BenchmarksModel.KFactor));
            FramesTextBox.BindText(model, nameof(BenchmarksModel.Frames));
            AtomsTextBox.BindText(model, nameof(BenchmarksModel.NumberOfAtoms));
            CoreTextBox.BindText(model, nameof(BenchmarksModel.Core));
            DescriptionLinkLabel.Text = "Click to view online";
            DescriptionLinkLabel.DataBindings.Add(nameof(Tag), model, nameof(BenchmarksModel.DescriptionUrl), false, DataSourceUpdateMode.OnPropertyChanged);
            TimeoutTextBox.BindText(model, nameof(BenchmarksModel.PreferredDays));
            ExpirationTextBox.BindText(model, nameof(BenchmarksModel.MaximumDays));
            ContactTextBox.BindText(model, nameof(BenchmarksModel.Contact));
            WorkServerTextBox.BindText(model, nameof(BenchmarksModel.ServerIP));

            model.PropertyChanged += (s, e) => ModelPropertyChanged((BenchmarksModel)s, e);
            model.SelectedSlotProjectListItems = new ListBoxSelectedListItemCollection(projectsListBox);
            projectsListBox.DisplayMember = nameof(ListItem.DisplayMember);
            projectsListBox.ValueMember = nameof(ListItem.ValueMember);
            projectsListBox.DataSource = model.SlotProjects;
            model.SetDefaultSlotProject();

            lstColors.DisplayMember = nameof(ListItem.DisplayMember);
            lstColors.ValueMember = nameof(ListItem.ValueMember);
            lstColors.DataSource = model.GraphColors;
            lstColors.BindSelectedValue(model, nameof(BenchmarksModel.SelectedGraphColorItem));

            colorPreviewPictureBox.DataBindings.Add(nameof(PictureBox.BackColor), model, nameof(BenchmarksModel.SelectedGraphColor), false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private void ModelPropertyChanged(BenchmarksModel model, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BenchmarksModel.BenchmarkText):
                    LoadBenchmarkText(model);
                    break;
                case nameof(BenchmarksModel.FrameTimeGraphControl):
                    LoadFrameTimeGraphControl(model);
                    break;
                case nameof(BenchmarksModel.ProductionGraphControl):
                    LoadProductionGraphControl(model);
                    break;
                case nameof(BenchmarksModel.ProjectComparisonGraphControl):
                    LoadProjectComparisonGraphControl(model);
                    break;
            }
        }

        private void LoadBenchmarkText(BenchmarksModel model)
        {
            txtBenchmarks.Lines = model.BenchmarkText?.ToArray() ?? Array.Empty<string>();
        }

        private void LoadFrameTimeGraphControl(BenchmarksModel model)
        {
            LoadGraphControl(frameTimeGraphTab, model.FrameTimeGraphControl);
        }

        private void LoadProductionGraphControl(BenchmarksModel model)
        {
            LoadGraphControl(productionGraphTab, model.ProductionGraphControl);
        }

        private void LoadProjectComparisonGraphControl(BenchmarksModel model)
        {
            LoadGraphControl(projectComparisonGraphTab, model.ProjectComparisonGraphControl);
        }

        private static void LoadGraphControl(TabPage tab, Control graphControl)
        {
            foreach (Control c in tab.Controls)
            {
                c.Dispose();
            }

            tab.Controls.Clear();
            if (graphControl != null)
            {
                graphControl.Dock = DockStyle.Fill;
                tab.Controls.Add(graphControl);
            }
        }

        // Event Handlers

        private void projectsListBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                int index = projectsListBox.IndexFromPoint(e.X, e.Y);
                projectsListBox.SelectedItems.Clear();
                projectsListBox.SelectedIndex = index;
            }
        }

        private void projectsListBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (projectsListBox.SelectedIndex == -1) return;

            if (e.Button == MouseButtons.Right)
            {
                projectsListBoxContextMenuStrip.Show(projectsListBox.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.DescriptionLinkClicked(LocalProcessService.Default);
        }

        private void btnMoveColorUp_Click(object sender, EventArgs e)
        {
            _presenter.Model.MoveSelectedGraphColorUp();
        }

        private void btnMoveColorDown_Click(object sender, EventArgs e)
        {
            _presenter.Model.MoveSelectedGraphColorDown();
        }

        private void btnAddColor_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColorDialogPresenter())
            {
                _presenter.AddGraphColorClicked(dialog);
            }
        }

        private void btnDeleteColor_Click(object sender, EventArgs e)
        {
            _presenter.DeleteGraphColorClicked();
        }
    }
}
