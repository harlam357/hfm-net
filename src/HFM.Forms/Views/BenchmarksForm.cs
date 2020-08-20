using System;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Services;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;

namespace HFM.Forms.Views
{
    public partial class BenchmarksForm : FormWrapper, IWin32Form
    {
        private readonly BenchmarksPresenter _presenter;

        public BenchmarksForm(BenchmarksPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));

            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
        }

        private void BenchmarksForm_Load(object sender, EventArgs e)
        {
            LoadData(_presenter.Model);
        }

        // ReSharper disable InconsistentNaming

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

            picDeleteClient.BindVisible(model, nameof(BenchmarksModel.SelectedSlotDeleteEnabled));

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

            listBox1.DataSource = model.SlotProjects;
            listBox1.DisplayMember = nameof(ListItem.DisplayMember);
            listBox1.ValueMember = nameof(ListItem.ValueMember);
            listBox1.DataBindings.Add(nameof(ListBox.SelectedValue), model, nameof(BenchmarksModel.SelectedSlotProject), true, DataSourceUpdateMode.OnPropertyChanged);

            // load any existing reports
            LoadBenchmarkText();
            LoadFrameTimeGraphControl();
            LoadProductionGraphControl();
            model.PropertyChanged += (s, e) => ModelPropertyChanged(e);

            lstColors.DataSource = model.GraphColors;
        }

        private void ModelPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BenchmarksModel.BenchmarkText):
                    LoadBenchmarkText();
                    break;
                case nameof(BenchmarksModel.FrameTimeGraphControl):
                    LoadFrameTimeGraphControl();
                    break;
                case nameof(BenchmarksModel.ProductionGraphControl):
                    LoadProductionGraphControl();
                    break;
            }
        }

        private void LoadBenchmarkText()
        {
            txtBenchmarks.Lines = _presenter.Model.BenchmarkText?.ToArray() ?? Array.Empty<string>();
        }

        private void LoadFrameTimeGraphControl()
        {
            foreach (Control c in tabGraphFrameTime1.Controls)
            {
                c.Dispose();
            }

            tabGraphFrameTime1.Controls.Clear();
            var control = _presenter.Model.FrameTimeGraphControl;
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                tabGraphFrameTime1.Controls.Add(control);
            }
        }

        private void LoadProductionGraphControl()
        {
            foreach (Control c in tabGraphPPD1.Controls)
            {
                c.Dispose();
            }

            tabGraphPPD1.Controls.Clear();
            var control = _presenter.Model.ProductionGraphControl;
            if (control != null)
            {
                control.Dock = DockStyle.Fill;
                tabGraphPPD1.Controls.Add(control);
            }
        }

        #region Event Handlers

        private void listBox1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                listBox1.SelectedIndex = listBox1.IndexFromPoint(e.X, e.Y);
            }
        }

        private void listBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (listBox1.SelectedIndex == -1) return;

            if (e.Button == MouseButtons.Right)
            {
                listBox1ContextMenuStrip.Show(listBox1.PointToScreen(new Point(e.X, e.Y)));
            }
        }

        private void mnuContextRefreshMinimum_Click(object sender, EventArgs e)
        {
            _presenter.RefreshMinimumFrameTimeClicked();
        }

        private void mnuContextDeleteProject_Click(object sender, EventArgs e)
        {
            _presenter.DeleteProjectClicked();
        }

        private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            _presenter.DescriptionLinkClicked(LocalProcessService.Default);
        }

        private void picDeleteClient_Click(object sender, EventArgs e)
        {
            _presenter.DeleteSlotClicked();
        }

        private void lstColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1) return;

            picColorPreview.BackColor = _presenter.Model.GraphColors[lstColors.SelectedIndex];
        }

        private void btnMoveColorUp_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _presenter.MessageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (lstColors.SelectedIndex == 0) return;

            int index = lstColors.SelectedIndex;
            Color moveColor = _presenter.Model.GraphColors[index];
            _presenter.Model.GraphColors.RemoveAt(index);
            _presenter.Model.GraphColors.Insert(index - 1, moveColor);
            UpdateGraphColorsBinding();
            lstColors.SelectedIndex = index - 1;
        }

        private void btnMoveColorDown_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _presenter.MessageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (lstColors.SelectedIndex == _presenter.Model.GraphColors.Count - 1) return;

            int index = lstColors.SelectedIndex;
            Color moveColor = _presenter.Model.GraphColors[index];
            _presenter.Model.GraphColors.RemoveAt(index);
            _presenter.Model.GraphColors.Insert(index + 1, moveColor);
            UpdateGraphColorsBinding();
            lstColors.SelectedIndex = index + 1;
        }

        private void btnAddColor_Click(object sender, EventArgs e)
        {
            using (var dlg = new ColorDialog())
            {
                if (dlg.ShowDialog(this).Equals(DialogResult.OK))
                {
                    Color addColor = FindNearestKnown(dlg.Color);
                    if (_presenter.Model.GraphColors.Contains(addColor))
                    {
                        _presenter.MessageBox.ShowInformation(this, String.Format(CultureInfo.CurrentCulture,
                            "{0} is already a graph color.", addColor.Name), Text);
                        return;
                    }

                    _presenter.Model.GraphColors.Add(addColor);
                    UpdateGraphColorsBinding();
                    lstColors.SelectedIndex = _presenter.Model.GraphColors.Count - 1;
                }
            }
        }

        private static Color FindNearestKnown(Color c)
        {
            var best = new ColorName { Name = null };

            foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
            {
                var known = Color.FromName(colorName);
                int dist = Math.Abs(c.R - known.R) + Math.Abs(c.G - known.G) + Math.Abs(c.B - known.B);

                if (best.Name == null || dist < best.Distance)
                {
                    best.Color = known;
                    best.Name = colorName;
                    best.Distance = dist;
                }
            }

            return best.Color;
        }

        private struct ColorName
        {
            public Color Color;
            public string Name;
            public int Distance;
        }

        private void btnDeleteColor_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _presenter.MessageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (_presenter.Model.GraphColors.Count <= 3)
            {
                _presenter.MessageBox.ShowInformation(this, "Must have at least three colors.", Text);
                return;
            }

            int index = lstColors.SelectedIndex;
            _presenter.Model.GraphColors.RemoveAt(index);
            UpdateGraphColorsBinding();
            if (index == _presenter.Model.GraphColors.Count)
            {
                lstColors.SelectedIndex = _presenter.Model.GraphColors.Count - 1;
            }
        }

        private void btnExit_Click(object sender, EventArgs e)
        {
            Close();
        }

        // ReSharper restore InconsistentNaming

        #endregion

        private void UpdateGraphColorsBinding()
        {
            var cm = (CurrencyManager)lstColors.BindingContext[lstColors.DataSource];
            cm.Refresh();
        }
    }
}
