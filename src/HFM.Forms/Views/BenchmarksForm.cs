
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using ZedGraph;

using HFM.Core.Client;
using HFM.Core.Logging;
using HFM.Core.WorkUnits;
using HFM.Forms.Controls;
using HFM.Preferences;
using HFM.Proteins;

namespace HFM.Forms
{
    public interface IBenchmarksView
    {
        event EventHandler Closed;

        /// <summary>
        /// Gets or sets the project ID to load when shown.
        /// </summary>
        int ProjectId { get; set; }

        Point Location { get; set; }

        Size Size { get; set; }

        void Show();
    }

    public enum GraphLayoutType
    {
        Single,
        ClientsPerGraph
    }

    public partial class BenchmarksForm : FormWrapper, IBenchmarksView
    {
        #region Properties

        /// <summary>
        /// Gets or sets the project ID to load when shown.
        /// </summary>
        public int ProjectId { get; set; }

        public GraphLayoutType GraphLayoutType { get; set; }

        private ILogger _logger;

        public ILogger Logger
        {
            get { return _logger ?? (_logger = NullLogger.Instance); }
            set { _logger = value; }
        }

        #endregion

        #region Fields

        private int _currentNumberOfGraphs = 1;

        private readonly IPreferenceSet _prefs;
        private readonly IProteinService _proteinService;
        private readonly IProteinBenchmarkService _benchmarkService;
        private readonly IList<Color> _graphColors;
        private readonly ClientConfiguration _clientConfiguration;
        private readonly MessageBoxPresenter _messageBox;
        private readonly IExternalProcessStarter _processStarter;
        private readonly ZedGraphManager _zedGraphManager;

        private SlotIdentifier _currentSlotIdentifier;

        #endregion

        #region Constructor

        public BenchmarksForm(IPreferenceSet prefs, IProteinService proteinService, IProteinBenchmarkService benchmarkService,
                              ClientConfiguration clientConfiguration, MessageBoxPresenter messageBox, IExternalProcessStarter processStarter)
        {
            _prefs = prefs;
            _proteinService = proteinService;
            _benchmarkService = benchmarkService;
            _graphColors = _prefs.Get<List<Color>>(Preference.GraphColors);
            _clientConfiguration = clientConfiguration;
            _messageBox = messageBox;
            _processStarter = processStarter;
            _zedGraphManager = new ZedGraphManager();

            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
        }

        #endregion

        // ReSharper disable InconsistentNaming

        private void frmBenchmarks_Shown(object sender, EventArgs e)
        {
            UpdateClientsComboBinding();
            UpdateProjectListBoxBinding(ProjectId);
            lstColors.DataSource = _graphColors;
            GraphLayoutType = _prefs.Get<GraphLayoutType>(Preference.BenchmarksGraphLayoutType);
            pnlClientLayout.DataSource = this;
            pnlClientLayout.ValueMember = "GraphLayoutType";

            // Issue 154 - make sure focus is on the projects list box
            listBox1.Select();
        }

        private void frmBenchmarks_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Save state data
            _prefs.Set(Preference.BenchmarksFormLocation, Location);
            _prefs.Set(Preference.BenchmarksFormSize, Size);
            _prefs.Set(Preference.BenchmarksGraphLayoutType, GraphLayoutType);
            _prefs.Save();
        }

        #region Event Handlers

        private void cboClients_SelectedIndexChanged(object sender, EventArgs e)
        {
            _currentSlotIdentifier = (SlotIdentifier)cboClients.SelectedValue;
            picDeleteClient.Visible = _currentSlotIdentifier != SlotIdentifier.AllSlots;

            UpdateProjectListBoxBinding();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedItem = listBox1.SelectedItem;
            if (selectedItem is null) return;

            txtBenchmarks.Text = String.Empty;
            int projectID = (int)selectedItem;
            Protein protein = _proteinService.Get(projectID);
            if (protein == null)
            {
                Logger.Warn($"Could not find Project {projectID}.");
            }

            var projectInfoLines = new List<string>();
            PopulateProteinInformation(projectID, protein, projectInfoLines);

            var list = _benchmarkService.GetBenchmarks(_currentSlotIdentifier, projectID)
                .OrderBy(x => x.SlotIdentifier.Name).ThenBy(x => x.Threads)
                .ToList();

            var benchmarkInfoLines = new List<string>(projectInfoLines);
            foreach (ProteinBenchmark benchmark in list)
            {
                WorkUnitModel workUnitModel = null;
                SlotStatus status = SlotStatus.Unknown;

                var slotModel = _clientConfiguration.Slots.FirstOrDefault(x =>
                    x.SlotIdentifier.Equals(benchmark.SlotIdentifier) &&
                    x.WorkUnitModel.BenchmarkIdentifier.Equals(benchmark.BenchmarkIdentifier));

                if (slotModel != null && slotModel.Status.IsRunning())
                {
                    workUnitModel = slotModel.WorkUnitModel;
                    status = slotModel.Status;
                }
                string numberFormat = NumberFormat.Get(_prefs.Get<int>(Preference.DecimalPlaces));
                PopulateBenchmarkInformation(protein, benchmark, workUnitModel, status, numberFormat, benchmarkInfoLines);
            }

            UpdateBenchmarkText(benchmarkInfoLines);

            tabControl1.SuspendLayout();

            int clientsPerGraph = _prefs.Get<int>(Preference.BenchmarksClientsPerGraph);
            SetupGraphTabs(list.Count, clientsPerGraph);

            int tabIndex = 1;
            if (GraphLayoutType.Equals(GraphLayoutType.ClientsPerGraph))
            {
                int lastDisplayed = 0;
                for (int i = 1; i < list.Count; i++)
                {
                    if (i % clientsPerGraph == 0)
                    {
                        var benchmarks = new ProteinBenchmark[clientsPerGraph];
                        list.CopyTo(lastDisplayed, benchmarks, 0, clientsPerGraph);
                        DrawGraphs(tabIndex, projectInfoLines, benchmarks, protein);
                        tabIndex++;
                        lastDisplayed = i;
                    }
                }

                if (lastDisplayed < list.Count)
                {
                    var benchmarks = new ProteinBenchmark[list.Count - lastDisplayed];
                    list.CopyTo(lastDisplayed, benchmarks, 0, list.Count - lastDisplayed);
                    DrawGraphs(tabIndex, projectInfoLines, benchmarks, protein);
                }
            }
            else
            {
                DrawGraphs(tabIndex, projectInfoLines, list, protein);
            }

            tabControl1.ResumeLayout(true);
        }

        private void SetupGraphTabs(int numberOfBenchmarks, int clientsPerGraph)
        {
            int graphs = 1;
            if (GraphLayoutType.Equals(GraphLayoutType.ClientsPerGraph))
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
            _zedGraphManager.CreateFrameTimeGraph(GetFrameTimeGraph(tabIndex), projectInfoLines, benchmarks, _graphColors, protein);
            _zedGraphManager.CreatePpdGraph(GetPpdGraph(tabIndex), projectInfoLines, benchmarks, _graphColors,
               _prefs.Get<int>(Preference.DecimalPlaces), protein, IsEnabled(_prefs.Get<BonusCalculation>(Preference.BonusCalculation)));
        }

        private ZedGraphControl GetFrameTimeGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphFrameTime" + index].Controls["zgFrameTime" + index];
        }

        private ZedGraphControl GetPpdGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphPPD" + index].Controls["zgPpd" + index];
        }

        private void PopulateBenchmarkInformation(Protein protein, ProteinBenchmark benchmark, WorkUnitModel workUnitModel, SlotStatus status, string numberFormat, ICollection<string> lines)
        {
            if (protein == null)
            {
                return;
            }

            var calculateBonus = _prefs.Get<BonusCalculation>(Preference.BonusCalculation);
            var calculateBonusEnabled = IsEnabled(calculateBonus);

            lines.Add(String.Empty);
            lines.Add($" Name: {benchmark.SlotIdentifier.Name}");
            lines.Add($" Path: {benchmark.SlotIdentifier.ClientIdentifier.ToServerPortString()}");
            if (benchmark.BenchmarkIdentifier.HasProcessor)
            {
                var slotType = SlotTypeConvert.FromCoreName(protein.Core);
                lines.Add($" Proc: {benchmark.BenchmarkIdentifier.ToProcessorAndThreadsString(slotType)}");
            }
            lines.Add($" Number of Frames Observed: {benchmark.FrameTimes.Count}");
            lines.Add(String.Empty);
            lines.Add(String.Format(" Min. Time / Frame : {0} - {1} PPD",
               benchmark.MinimumFrameTime, GetPPD(benchmark.MinimumFrameTime, protein, calculateBonusEnabled).ToString(numberFormat)));
            lines.Add(String.Format(" Avg. Time / Frame : {0} - {1} PPD",
               benchmark.AverageFrameTime, GetPPD(benchmark.AverageFrameTime, protein, calculateBonusEnabled).ToString(numberFormat)));

            if (workUnitModel != null)
            {
                lines.Add(String.Format(" Cur. Time / Frame : {0} - {1} PPD",
                   workUnitModel.GetFrameTime(PPDCalculation.LastFrame), workUnitModel.GetPPD(status, PPDCalculation.LastFrame, calculateBonus).ToString(numberFormat)));
                lines.Add(String.Format(" R3F. Time / Frame : {0} - {1} PPD",
                   workUnitModel.GetFrameTime(PPDCalculation.LastThreeFrames), workUnitModel.GetPPD(status, PPDCalculation.LastThreeFrames, calculateBonus).ToString(numberFormat)));
                lines.Add(String.Format(" All  Time / Frame : {0} - {1} PPD",
                   workUnitModel.GetFrameTime(PPDCalculation.AllFrames), workUnitModel.GetPPD(status, PPDCalculation.AllFrames, calculateBonus).ToString(numberFormat)));
                lines.Add(String.Format(" Eff. Time / Frame : {0} - {1} PPD",
                   workUnitModel.GetFrameTime(PPDCalculation.EffectiveRate), workUnitModel.GetPPD(status, PPDCalculation.EffectiveRate, calculateBonus).ToString(numberFormat)));
            }

            lines.Add(String.Empty);
        }

        private static bool IsEnabled(BonusCalculation type)
        {
            return type.Equals(BonusCalculation.DownloadTime) ||
                   type.Equals(BonusCalculation.FrameTime);
        }

        private static double GetPPD(TimeSpan frameTime, Protein protein, bool calculateUnitTimeByFrameTime)
        {
            if (calculateUnitTimeByFrameTime)
            {
                var unitTime = TimeSpan.FromSeconds(frameTime.TotalSeconds * protein.Frames);
                return protein.GetBonusPPD(frameTime, unitTime);
            }
            return protein.GetPPD(frameTime);
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
            if (_messageBox.AskYesNoQuestion(this, String.Format(CultureInfo.CurrentCulture,
                    "Are you sure you want to refresh {0} - Project {1} minimum frame time?",
                    _currentSlotIdentifier, listBox1.SelectedItem), 
                Text) == DialogResult.Yes)
            {
                _benchmarkService.UpdateMinimumFrameTime(_currentSlotIdentifier, (int) listBox1.SelectedItem);
                listBox1_SelectedIndexChanged(sender, e);
            }
        }

        private void mnuContextDeleteProject_Click(object sender, EventArgs e)
        {
            if (_messageBox.AskYesNoQuestion(this, String.Format(CultureInfo.CurrentCulture,
                    "Are you sure you want to delete {0} - Project {1}?",
                    _currentSlotIdentifier, listBox1.SelectedItem), 
                Text) == DialogResult.Yes)
            {
                _benchmarkService.RemoveAll(_currentSlotIdentifier, (int)listBox1.SelectedItem);
                UpdateProjectListBoxBinding();
                if (_benchmarkService.GetSlotIdentifiers().Contains(_currentSlotIdentifier) == false)
                {
                    UpdateClientsComboBinding();
                }
            }
        }

        private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (DescriptionLinkLabel.Tag == null)
            {
                return;
            }
            string message = _processStarter.ShowWebBrowser(DescriptionLinkLabel.Tag.ToString());
            if (message != null)
            {
                _messageBox.ShowError(this, message, Text);
            }
        }

        private void picDeleteClient_Click(object sender, EventArgs e)
        {
            if (_messageBox.AskYesNoQuestion(this, String.Format(CultureInfo.CurrentCulture,
                    "Are you sure you want to delete {0}?", _currentSlotIdentifier), Text) == DialogResult.Yes)
            {
                int currentIndex = cboClients.SelectedIndex;
                _benchmarkService.RemoveAll(_currentSlotIdentifier);
                UpdateClientsComboBinding(currentIndex);
            }
        }

        private void lstColors_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1) return;

            picColorPreview.BackColor = _graphColors[lstColors.SelectedIndex];
        }

        private void btnMoveColorUp_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _messageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (lstColors.SelectedIndex == 0) return;

            int index = lstColors.SelectedIndex;
            Color moveColor = _graphColors[index];
            _graphColors.RemoveAt(index);
            _graphColors.Insert(index - 1, moveColor);
            UpdateGraphColorsBinding();
            lstColors.SelectedIndex = index - 1;
        }

        private void btnMoveColorDown_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _messageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (lstColors.SelectedIndex == _graphColors.Count - 1) return;

            int index = lstColors.SelectedIndex;
            Color moveColor = _graphColors[index];
            _graphColors.RemoveAt(index);
            _graphColors.Insert(index + 1, moveColor);
            UpdateGraphColorsBinding();
            lstColors.SelectedIndex = index + 1;
        }

        private void btnAddColor_Click(object sender, EventArgs e)
        {
            var dlg = new ColorDialog();
            if (dlg.ShowDialog(this).Equals(DialogResult.OK))
            {
                Color addColor = FindNearestKnown(dlg.Color);
                if (_graphColors.Contains(addColor))
                {
                    _messageBox.ShowInformation(this, String.Format(CultureInfo.CurrentCulture,
                       "{0} is already a graph color.", addColor.Name), Text);
                    return;
                }

                _graphColors.Add(addColor);
                UpdateGraphColorsBinding();
                lstColors.SelectedIndex = _graphColors.Count - 1;
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
                _messageBox.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (_graphColors.Count <= 3)
            {
                _messageBox.ShowInformation(this, "Must have at least three colors.", Text);
                return;
            }

            int index = lstColors.SelectedIndex;
            _graphColors.RemoveAt(index);
            UpdateGraphColorsBinding();
            if (index == _graphColors.Count)
            {
                lstColors.SelectedIndex = _graphColors.Count - 1;
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
            _prefs.Set(Preference.BenchmarksClientsPerGraph, (int)udClientsPerGraph.Value);
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

        #region Update Routines

        private void UpdateBenchmarkText(IEnumerable<string> benchmarkLines)
        {
            txtBenchmarks.Lines = benchmarkLines.ToArray();
        }

        private void PopulateProteinInformation(int projectID, Protein protein, ICollection<string> lines)
        {
            if (protein != null)
            {
                ProjectIDTextBox.Text = protein.WorkUnitName;
                CreditTextBox.Text = protein.Credit.ToString();
                KFactorTextBox.Text = protein.KFactor.ToString();
                FramesTextBox.Text = protein.Frames.ToString();
                AtomsTextBox.Text = protein.NumberOfAtoms.ToString();
                CoreTextBox.Text = protein.Core;
                DescriptionLinkLabel.Text = "Click to view online";
                DescriptionLinkLabel.Tag = protein.Description;
                TimeoutTextBox.Text = protein.PreferredDays.ToString();
                ExpirationTextBox.Text = protein.MaximumDays.ToString();
                ContactTextBox.Text = protein.Contact;
                WorkServerTextBox.Text = protein.ServerIP;

                lines.Add(String.Format(" Project ID: {0}", protein.ProjectNumber));
                lines.Add(String.Format(" Core: {0}", protein.Core));
                lines.Add(String.Format(" Credit: {0}", protein.Credit));
                lines.Add(String.Format(" Frames: {0}", protein.Frames));
                lines.Add(String.Empty);
            }
            else
            {
                ProjectIDTextBox.Text = String.Empty;
                CreditTextBox.Text = String.Empty;
                KFactorTextBox.Text = String.Empty;
                FramesTextBox.Text = String.Empty;
                AtomsTextBox.Text = String.Empty;
                CoreTextBox.Text = String.Empty;
                DescriptionLinkLabel.Text = String.Empty;
                DescriptionLinkLabel.Tag = null;
                TimeoutTextBox.Text = String.Empty;
                ExpirationTextBox.Text = String.Empty;
                ContactTextBox.Text = String.Empty;
                WorkServerTextBox.Text = String.Empty;

                lines.Add(String.Format(" Project ID: {0} Not Found", projectID));
            }
        }

        #region Binding

        private void UpdateClientsComboBinding()
        {
            UpdateClientsComboBinding(-1);
        }

        private void UpdateClientsComboBinding(int index)
        {
            var slotIdentifiers = Enumerable.Repeat(SlotIdentifier.AllSlots, 1)
                .Concat(_benchmarkService.GetSlotIdentifiers().OrderBy(x => x.Name))
                .ToList();

            cboClients.DataBindings.Clear();
            cboClients.DataSource = slotIdentifiers;
            cboClients.DisplayMember = "Value";
            // TODO: Is this required for Mono compatibility?
            //cboClients.ValueMember = "Client";

            if (index > -1 && cboClients.Items.Count > 0)
            {
                if (index < cboClients.Items.Count)
                {
                    cboClients.SelectedIndex = index;
                }
                else if (index == cboClients.Items.Count)
                {
                    cboClients.SelectedIndex = index - 1;
                }
            }
        }

        private void UpdateProjectListBoxBinding()
        {
            UpdateProjectListBoxBinding(-1);
        }

        private void UpdateProjectListBoxBinding(int initialProjectID)
        {
            listBox1.DataBindings.Clear();
            listBox1.DataSource = _benchmarkService.GetBenchmarkProjects(_currentSlotIdentifier);

            int index = listBox1.Items.IndexOf(initialProjectID);
            if (index > -1)
            {
                listBox1.SelectedIndex = index;
            }
        }

        private void UpdateGraphColorsBinding()
        {
            var cm = (CurrencyManager)lstColors.BindingContext[lstColors.DataSource];
            cm.Refresh();
        }

        #endregion

        #endregion
    }
}
