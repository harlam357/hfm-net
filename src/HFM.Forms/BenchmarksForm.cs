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
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using Castle.Core.Logging;
using harlam357.Windows.Forms;
using ZedGraph;

using HFM.Core;
using HFM.Core.DataTypes;
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
        private readonly IClientConfiguration _clientConfiguration;
        private readonly IMessageBoxView _messageBoxView;
        private readonly IExternalProcessStarter _processStarter;
        private readonly ZedGraphManager _zedGraphManager;

        private ProteinBenchmarkSlotIdentifier _currentSlotIdentifier;

        #endregion

        #region Constructor

        public BenchmarksForm(IPreferenceSet prefs, IProteinService proteinService, IProteinBenchmarkService benchmarkService,
                              IClientConfiguration clientConfiguration, IMessageBoxView messageBoxView, IExternalProcessStarter processStarter)
        {
            _prefs = prefs;
            _proteinService = proteinService;
            _benchmarkService = benchmarkService;
            _graphColors = _prefs.Get<List<Color>>(Preference.GraphColors);
            _clientConfiguration = clientConfiguration;
            _messageBoxView = messageBoxView;
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
            _currentSlotIdentifier = (ProteinBenchmarkSlotIdentifier)cboClients.SelectedValue;
            picDeleteClient.Visible = !_currentSlotIdentifier.AllSlots;

            UpdateProjectListBoxBinding();
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            txtBenchmarks.Text = String.Empty;
            int projectId = (int)listBox1.SelectedItem;
            Protein protein = _proteinService.Get(projectId);
            if (protein == null)
            {
                Logger.WarnFormat("Could not find Project {0}.", projectId);
            }

            var projectInfoLines = new List<string>();
            PopulateProteinInformation(protein, projectInfoLines);

            List<ProteinBenchmark> list = _benchmarkService.GetBenchmarks(_currentSlotIdentifier, projectId).ToList();
            list.Sort((benchmark1, benchmark2) => benchmark1.OwningSlotName.CompareTo(benchmark2.OwningSlotName));

            var benchmarkInfoLines = new List<string>(projectInfoLines);
            foreach (ProteinBenchmark benchmark in list)
            {
                UnitInfoModel unitInfoModel = null;
                SlotStatus status = SlotStatus.Unknown;

                var slotModel = _clientConfiguration.Slots.FirstOrDefault(x =>
                   x.Name == benchmark.OwningSlotName &&
                   x.Settings.ClientPath == benchmark.OwningClientPath &&
                   x.UnitInfoModel.UnitInfoData.ProjectID == benchmark.ProjectID);
                if (slotModel != null && slotModel.ProductionValuesOk)
                {
                    unitInfoModel = slotModel.UnitInfoModel;
                    status = slotModel.Status;
                }
                PopulateBenchmarkInformation(protein, benchmark, unitInfoModel, status, _prefs.GetPpdFormatString(), benchmarkInfoLines);
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
            _zedGraphManager.CreateFrameTimeGraph(GetFrameTimeGraph(tabIndex), projectInfoLines, benchmarks, _graphColors);
            _zedGraphManager.CreatePpdGraph(GetPpdGraph(tabIndex), projectInfoLines, benchmarks, _graphColors,
               _prefs.Get<int>(Preference.DecimalPlaces), protein, IsEnabled(_prefs.Get<BonusCalculationType>(Preference.BonusCalculation)));
        }

        private ZedGraphControl GetFrameTimeGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphFrameTime" + index].Controls["zgFrameTime" + index];
        }

        private ZedGraphControl GetPpdGraph(int index)
        {
            return (ZedGraphControl)tabControl1.TabPages["tabGraphPPD" + index].Controls["zgPpd" + index];
        }

        private void PopulateBenchmarkInformation(Protein protein, ProteinBenchmark benchmark, UnitInfoModel unitInfoModel, SlotStatus status, string ppdFormatString, ICollection<string> lines)
        {
            if (protein == null)
            {
                return;
            }

            var calculateBonus = _prefs.Get<BonusCalculationType>(Preference.BonusCalculation);
            var calculateBonusEnabled = IsEnabled(calculateBonus);

            lines.Add(String.Empty);
            lines.Add(String.Format(" Name: {0}", benchmark.OwningSlotName));
            lines.Add(String.Format(" Path: {0}", benchmark.OwningClientPath));
            lines.Add(String.Format(" Number of Frames Observed: {0}", benchmark.FrameTimes.Count));
            lines.Add(String.Empty);
            lines.Add(String.Format(" Min. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
               benchmark.MinimumFrameTime, GetPPD(benchmark.MinimumFrameTime, protein, calculateBonusEnabled)));
            lines.Add(String.Format(" Avg. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
               benchmark.AverageFrameTime, GetPPD(benchmark.AverageFrameTime, protein, calculateBonusEnabled)));

            if (unitInfoModel != null)
            {
                lines.Add(String.Format(" Cur. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                   unitInfoModel.GetFrameTime(PpdCalculationType.LastFrame), unitInfoModel.GetPPD(status, PpdCalculationType.LastFrame, calculateBonus)));
                lines.Add(String.Format(" R3F. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                   unitInfoModel.GetFrameTime(PpdCalculationType.LastThreeFrames), unitInfoModel.GetPPD(status, PpdCalculationType.LastThreeFrames, calculateBonus)));
                lines.Add(String.Format(" All  Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                   unitInfoModel.GetFrameTime(PpdCalculationType.AllFrames), unitInfoModel.GetPPD(status, PpdCalculationType.AllFrames, calculateBonus)));
                lines.Add(String.Format(" Eff. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                   unitInfoModel.GetFrameTime(PpdCalculationType.EffectiveRate), unitInfoModel.GetPPD(status, PpdCalculationType.EffectiveRate, calculateBonus)));
            }

            lines.Add(String.Empty);
        }

        private static bool IsEnabled(BonusCalculationType type)
        {
            return type.Equals(BonusCalculationType.DownloadTime) ||
                   type.Equals(BonusCalculationType.FrameTime);
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
            if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "Are you sure you want to refresh {0} - Project {1} minimum frame time?",
                  _currentSlotIdentifier.Value, listBox1.SelectedItem),
                     Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
                _benchmarkService.UpdateMinimumFrameTime(_currentSlotIdentifier, (int)listBox1.SelectedItem);
                listBox1_SelectedIndexChanged(sender, e);
            }
        }

        private void mnuContextDeleteProject_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "Are you sure you want to delete {0} - Project {1}?",
                  _currentSlotIdentifier.Value, listBox1.SelectedItem),
                     Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
            {
                _benchmarkService.RemoveAll(_currentSlotIdentifier, (int)listBox1.SelectedItem);
                UpdateProjectListBoxBinding();
                if (_benchmarkService.SlotIdentifiers.Contains(_currentSlotIdentifier) == false)
                {
                    UpdateClientsComboBinding();
                }
            }
        }

        private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            if (linkDescription.Tag == null)
            {
                return;
            }
            string message = _processStarter.ShowWebBrowser(linkDescription.Tag.ToString());
            if (message != null)
            {
                _messageBoxView.ShowError(this, message, Text);
            }
        }

        private void picDeleteClient_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to delete {0}?", _currentSlotIdentifier.Value),
                        Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
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
                _messageBoxView.ShowInformation(this, "No Color Selected.", Text);
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
                _messageBoxView.ShowInformation(this, "No Color Selected.", Text);
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
                Color addColor = dlg.Color.FindNearestKnown();
                if (_graphColors.Contains(addColor))
                {
                    _messageBoxView.ShowInformation(this, String.Format(CultureInfo.CurrentCulture,
                       "{0} is already a graph color.", addColor.Name), Text);
                    return;
                }

                _graphColors.Add(addColor);
                UpdateGraphColorsBinding();
                lstColors.SelectedIndex = _graphColors.Count - 1;
            }
        }

        private void btnDeleteColor_Click(object sender, EventArgs e)
        {
            if (lstColors.SelectedIndex == -1)
            {
                _messageBoxView.ShowInformation(this, "No Color Selected.", Text);
                return;
            }

            if (_graphColors.Count <= 3)
            {
                _messageBoxView.ShowInformation(this, "Must have at least three colors.", Text);
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

        private void PopulateProteinInformation(Protein protein, ICollection<string> lines)
        {
            if (protein != null)
            {
                txtProjectID.Text = protein.WorkUnitName;
                txtCredit.Text = protein.Credit.ToString();
                txtKFactor.Text = protein.KFactor.ToString();
                txtFrames.Text = protein.Frames.ToString();
                txtAtoms.Text = protein.NumberOfAtoms.ToString();
                txtCore.Text = protein.Core;
                linkDescription.Text = "Click to view online";
                linkDescription.Tag = protein.Description;
                txtPreferredDays.Text = protein.PreferredDays.ToString();
                txtMaximumDays.Text = protein.MaximumDays.ToString();
                txtContact.Text = protein.Contact;
                txtServerIP.Text = protein.ServerIP;

                lines.Add(String.Format(" Project ID: {0}", protein.ProjectNumber));
                lines.Add(String.Format(" Core: {0}", protein.Core));
                lines.Add(String.Format(" Credit: {0}", protein.Credit));
                lines.Add(String.Format(" Frames: {0}", protein.Frames));
                lines.Add(String.Empty);
            }
            else
            {
                txtProjectID.Text = String.Empty;
                txtCredit.Text = String.Empty;
                txtKFactor.Text = String.Empty;
                txtFrames.Text = String.Empty;
                txtAtoms.Text = String.Empty;
                txtCore.Text = String.Empty;
                linkDescription.Text = String.Empty;
                linkDescription.Tag = null;
                txtPreferredDays.Text = String.Empty;
                txtMaximumDays.Text = String.Empty;
                txtContact.Text = String.Empty;
                txtServerIP.Text = String.Empty;
            }
        }

        #region Binding

        private void UpdateClientsComboBinding()
        {
            UpdateClientsComboBinding(-1);
        }

        private void UpdateClientsComboBinding(int index)
        {
            cboClients.DataBindings.Clear();
            cboClients.DataSource = _benchmarkService.SlotIdentifiers;
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
