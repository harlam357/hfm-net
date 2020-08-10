using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using HFM.Core.Services;
using HFM.Core.WorkUnits;
using HFM.Forms.Controls;
using HFM.Forms.Internal;
using HFM.Forms.Models;
using HFM.Forms.Presenters;
using HFM.Proteins;

using ZedGraph;

namespace HFM.Forms.Views
{
    public enum GraphLayoutType
    {
        Single,
        ClientsPerGraph
    }

    public partial class BenchmarksForm : FormWrapper, IWin32Form
    {
        private int _currentNumberOfGraphs = 1;

        private readonly BenchmarksPresenter _presenter;
        private readonly ZedGraphManager _zedGraphManager;

        public BenchmarksForm(BenchmarksPresenter presenter)
        {
            _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter));
            _zedGraphManager = new ZedGraphManager();

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

            // load any existing benchmark text
            txtBenchmarks.Lines = _presenter.Model.BenchmarkText.ToArray();
            model.PropertyChanged += (s, e) => ModelPropertyChanged(e);

            lstColors.DataSource = model.GraphColors;
            pnlClientLayout.DataSource = model;
            pnlClientLayout.ValueMember = nameof(BenchmarksModel.GraphLayoutType);

            // Issue 154 - make sure focus is on the projects list box
            //listBox1.Select();
        }

        private void ModelPropertyChanged(PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case nameof(BenchmarksModel.BenchmarkText):
                    txtBenchmarks.Lines = _presenter.Model.BenchmarkText.ToArray();
                    break;
            }
        }

        #region Event Handlers

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            //var selectedItem = listBox1.SelectedItem;
            //if (selectedItem is null) return;

            //txtBenchmarks.Text = String.Empty;
            //int projectID = (int)selectedItem;
            //Protein protein = _presenter.ProteinService.Get(projectID);

            //var projectInfoLines = new List<string>();
            //PopulateProteinInformation(projectID, protein, projectInfoLines);

            //var list = _presenter.Model.BenchmarkService.GetBenchmarks(_presenter.Model.SelectedSlotIdentifier.Value, projectID)
            //    .OrderBy(x => x.SlotIdentifier.Name).ThenBy(x => x.Threads)
            //    .ToList();

            //var benchmarkInfoLines = new List<string>(projectInfoLines);
            //foreach (ProteinBenchmark benchmark in list)
            //{
            //    WorkUnitModel workUnitModel = null;
            //    SlotStatus status = SlotStatus.Unknown;

            //    var slotModel = _presenter.ClientConfiguration.Slots.FirstOrDefault(x =>
            //        x.SlotIdentifier.Equals(benchmark.SlotIdentifier) &&
            //        x.WorkUnitModel.BenchmarkIdentifier.Equals(benchmark.BenchmarkIdentifier));

            //    if (slotModel != null && slotModel.Status.IsRunning())
            //    {
            //        workUnitModel = slotModel.WorkUnitModel;
            //        status = slotModel.Status;
            //    }
            //    string numberFormat = NumberFormat.Get(_presenter.Model.DecimalPlaces);
            //    PopulateBenchmarkInformation(protein, benchmark, workUnitModel, status, numberFormat, benchmarkInfoLines);
            //}

            //UpdateBenchmarkText(benchmarkInfoLines);

            //tabControl1.SuspendLayout();

            //int clientsPerGraph = _presenter.Model.ClientsPerGraph;
            //SetupGraphTabs(list.Count, clientsPerGraph);

            //int tabIndex = 1;
            //if (_presenter.Model.GraphLayoutType == GraphLayoutType.ClientsPerGraph)
            //{
            //    int lastDisplayed = 0;
            //    for (int i = 1; i < list.Count; i++)
            //    {
            //        if (i % clientsPerGraph == 0)
            //        {
            //            var benchmarks = new ProteinBenchmark[clientsPerGraph];
            //            list.CopyTo(lastDisplayed, benchmarks, 0, clientsPerGraph);
            //            DrawGraphs(tabIndex, projectInfoLines, benchmarks, protein);
            //            tabIndex++;
            //            lastDisplayed = i;
            //        }
            //    }

            //    if (lastDisplayed < list.Count)
            //    {
            //        var benchmarks = new ProteinBenchmark[list.Count - lastDisplayed];
            //        list.CopyTo(lastDisplayed, benchmarks, 0, list.Count - lastDisplayed);
            //        DrawGraphs(tabIndex, projectInfoLines, benchmarks, protein);
            //    }
            //}
            //else
            //{
            //    DrawGraphs(tabIndex, projectInfoLines, list, protein);
            //}

            //tabControl1.ResumeLayout(true);
        }

        private void SetupGraphTabs(int numberOfBenchmarks, int clientsPerGraph)
        {
            int graphs = 1;
            if (_presenter.Model.GraphLayoutType == GraphLayoutType.ClientsPerGraph)
            {
                graphs = (int)Math.Ceiling(numberOfBenchmarks / (double)clientsPerGraph);
                if (graphs == 0)
                {
                    graphs = 1;
                }
            }

            if (graphs > _currentNumberOfGraphs)
            {
                for (int i = _currentNumberOfGraphs + 1; i <= graphs; i++)
                {
                    tabControl1.TabPages.Add("tabGraphFrameTime" + i, "Graph - Frame Time (" + i + ")");
                    var zgFrameTime = new ZedGraphControl();
                    zgFrameTime.Name = "zgFrameTime" + i;
                    zgFrameTime.Dock = DockStyle.Fill;
                    tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(zgFrameTime);

                    tabControl1.TabPages.Add("tabGraphPPD" + i, "Graph - PPD (" + i + ")");
                    var zgPpd = new ZedGraphControl();
                    zgPpd.Name = "zgPpd" + i;
                    zgPpd.Dock = DockStyle.Fill;
                    tabControl1.TabPages[tabControl1.TabPages.Count - 1].Controls.Add(zgPpd);
                }
            }
            else if (graphs < _currentNumberOfGraphs)
            {
                for (int i = _currentNumberOfGraphs; i > graphs; i--)
                {
                    tabControl1.TabPages.RemoveByKey("tabGraphFrameTime" + i);
                    tabControl1.TabPages.RemoveByKey("tabGraphPPD" + i);
                }
            }

            _currentNumberOfGraphs = graphs;
        }

        private void DrawGraphs(int tabIndex, IList<string> projectInfoLines, ICollection<ProteinBenchmark> benchmarks, Protein protein)
        {
            _zedGraphManager.CreateFrameTimeGraph(GetFrameTimeGraph(tabIndex), projectInfoLines, benchmarks, _presenter.Model.GraphColors, protein);
            _zedGraphManager.CreatePpdGraph(GetPpdGraph(tabIndex), projectInfoLines, benchmarks, _presenter.Model.GraphColors,
               _presenter.Model.DecimalPlaces, protein, IsEnabled(_presenter.Model.BonusCalculation));
        }

        private ZedGraphControl GetFrameTimeGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphFrameTime" + index].Controls["zgFrameTime" + index];
        }

        private ZedGraphControl GetPpdGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphPPD" + index].Controls["zgPpd" + index];
        }

        private static bool IsEnabled(BonusCalculation type)
        {
            return type.Equals(BonusCalculation.DownloadTime) ||
                   type.Equals(BonusCalculation.FrameTime);
        }

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
            if (_presenter.MessageBox.AskYesNoQuestion(this, String.Format(CultureInfo.CurrentCulture,
                    "Are you sure you want to refresh {0} - Project {1} minimum frame time?",
                    _presenter.Model.SelectedSlotIdentifier, listBox1.SelectedItem),
                Text) == DialogResult.Yes)
            {
                _presenter.Model.BenchmarkService.UpdateMinimumFrameTime(_presenter.Model.SelectedSlotIdentifier.Value, (int)listBox1.SelectedItem);
                listBox1_SelectedIndexChanged(sender, e);
            }
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
            var dlg = new ColorDialog();
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

        private static Color FindNearestKnown(Color c)
        {
            var best = new ColorName { Name = null };

            foreach (string colorName in Enum.GetNames(typeof(KnownColor)))
            {
                Color known = Color.FromName(colorName);
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

        private void rdoSingleGraph_CheckedChanged(object sender, EventArgs e)
        {
            SetClientsPerGraphUpDownEnabled();
        }

        private void rdoClientsPerGraph_CheckedChanged(object sender, EventArgs e)
        {
            SetClientsPerGraphUpDownEnabled();
        }

        private void udClientsPerGraph_ValueChanged(object sender, EventArgs e)
        {
            _presenter.Model.ClientsPerGraph = (int)udClientsPerGraph.Value;
        }

        private void SetClientsPerGraphUpDownEnabled()
        {
            udClientsPerGraph.Enabled = rdoClientsPerGraph.Checked;
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
