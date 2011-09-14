/*
 * HFM.NET - Benchmarks Form Class
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

using ZedGraph;

using harlam357.Windows.Forms;

using HFM.Forms.Controls;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   public interface IBenchmarksView
   {
      /// <summary>
      /// ProjectID to Load when View is Shown
      /// </summary>
      int LoadProjectID { get; set; }

      void SetManualStartPosition();
   
      #region System.Windows.Forms.Form Exposure

      void Show();

      Point Location { get; set; }

      Size Size { get; set; }

      #endregion
   }

   // ReSharper disable InconsistentNaming
   public partial class frmBenchmarks : FormWrapper, IBenchmarksView
   // ReSharper restore InconsistentNaming
   {
      #region Properties
      
      /// <summary>
      /// ProjectID to Load when View is Shown
      /// </summary>
      public int LoadProjectID { get; set; }

      public GraphLayoutType GraphLayoutType { get; set; }
      
      #endregion
   
      #region Fields

      private int _currentNumberOfGraphs = 1;

      private readonly IPreferenceSet _prefs;
      private readonly IProteinCollection _proteinCollection;
      private readonly IProteinBenchmarkContainer _benchmarkContainer;
      private readonly IList<Color> _graphColors;
      private readonly IEnumerable<IDisplayInstance> _displayCollection;
      private readonly IMessageBoxView _messageBoxView;
      private readonly IExternalProcessStarter _processStarter;
      private readonly ZedGraphManager _zedGraphManager;
      
      private BenchmarkClient _currentBenchmarkClient;

      #endregion

      #region Constructor

      public frmBenchmarks(IPreferenceSet prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer, 
                           IDisplayInstanceCollection displayCollection, IMessageBoxView messageBoxView, IExternalProcessStarter processStarter)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _graphColors = _prefs.GetPreference<List<Color>>(Preference.GraphColors);
         _displayCollection = displayCollection;
         _messageBoxView = messageBoxView;
         _processStarter = processStarter;
         _zedGraphManager = new ZedGraphManager();

         InitializeComponent();
      }

      #endregion
      
      public void SetManualStartPosition()
      {
         StartPosition = FormStartPosition.Manual;
      }

      private void frmBenchmarks_Shown(object sender, EventArgs e)
      {
         UpdateClientsComboBinding();
         UpdateProjectListBoxBinding(LoadProjectID);
         lstColors.DataSource = _graphColors;
         GraphLayoutType = _prefs.GetPreference<GraphLayoutType>(Preference.BenchmarksGraphLayoutType);
         pnlClientLayout.DataSource = this;
         pnlClientLayout.ValueMember = "GraphLayoutType";
         
         // Issue 154 - make sure focus is on the projects list box
         listBox1.Select();
      }

      private void frmBenchmarks_FormClosing(object sender, FormClosingEventArgs e)
      {
         // Save state data
         _prefs.SetPreference(Preference.BenchmarksFormLocation, Location);
         _prefs.SetPreference(Preference.BenchmarksFormSize, Size);
         _prefs.SetPreference(Preference.BenchmarksGraphLayoutType, GraphLayoutType);
         _prefs.Save();
      }

      #region Event Handlers

      private void cboClients_SelectedIndexChanged(object sender, EventArgs e)
      {
         _currentBenchmarkClient = (BenchmarkClient)cboClients.SelectedValue;
         picDeleteClient.Visible = !_currentBenchmarkClient.AllClients;
         
         UpdateProjectListBoxBinding();
      }
      
      private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
         txtBenchmarks.Text = String.Empty;
         int projectID = (int)listBox1.SelectedItem;

         string[] lines = UpdateProteinInformation(projectID);

         UpdateBenchmarkText(lines);

         List<ProteinBenchmark> list = _benchmarkContainer.GetBenchmarks(_currentBenchmarkClient, projectID).ToList();
         list.Sort((benchmark1, benchmark2) => benchmark1.OwningInstanceName.CompareTo(benchmark2.OwningInstanceName));
         IProtein protein;
         _proteinCollection.TryGetValue(projectID, out protein);

         foreach (ProteinBenchmark benchmark in list)
         {
            IUnitInfoLogic unit = null;
            bool valuesOk = false;

            ProteinBenchmark benchmark1 = benchmark;
            var instance = _displayCollection.FirstOrDefault(x => x.Name.Equals(benchmark1.OwningInstanceName));
            if (instance != null && instance.Owns(benchmark))
            {
               unit = instance.CurrentUnitInfo;
               valuesOk = instance.ProductionValuesOk;
            }
            UpdateBenchmarkText(ToMultiLineString(benchmark, unit, _prefs.PpdFormatString, valuesOk));
         }

         tabControl1.SuspendLayout();

         int clientsPerGraph = _prefs.GetPreference<int>(Preference.BenchmarksClientsPerGraph);
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
                  DrawGraphs(tabIndex, lines, benchmarks, protein);
                  tabIndex++;
                  lastDisplayed = i;
               }
            }

            if (lastDisplayed < list.Count)
            {
               var benchmarks = new ProteinBenchmark[list.Count - lastDisplayed];
               list.CopyTo(lastDisplayed, benchmarks, 0, list.Count - lastDisplayed);
               DrawGraphs(tabIndex, lines, benchmarks, protein);
            }
         }
         else
         {
            DrawGraphs(tabIndex, lines, list, protein);
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

      private void DrawGraphs(int tabIndex, IList<string> lines, IEnumerable<ProteinBenchmark> benchmarks, IProtein protein)
      {
         _zedGraphManager.CreateFrameTimeGraph(GetFrameTimeGraph(tabIndex), lines, benchmarks, _graphColors);
         _zedGraphManager.CreatePpdGraph(GetPpdGraph(tabIndex), lines, benchmarks, _graphColors,
            _prefs.GetPreference<int>(Preference.DecimalPlaces), protein, _prefs.GetPreference<bool>(Preference.CalculateBonus));
      }

      private ZedGraphControl GetFrameTimeGraph(int index)
      {
         return (ZedGraphControl)tabControl1.TabPages["tabGraphFrameTime" + index].Controls["zgFrameTime" + index];
      }

      private ZedGraphControl GetPpdGraph(int index)
      {
         return (ZedGraphControl)tabControl1.TabPages["tabGraphPPD" + index].Controls["zgPpd" + index];
      }

      /// <summary>
      /// Return Multi-Line String (Array)
      /// </summary>
      /// <param name="benchmark"></param>
      /// <param name="unitInfo">Client Instance UnitInfo (null for unavailable)</param>
      /// <param name="ppdFormatString">PPD Format String</param>
      /// <param name="productionValuesOk">Client Instance Production Values Flag</param>
      private IEnumerable<string> ToMultiLineString(ProteinBenchmark benchmark, IUnitInfoLogic unitInfo, string ppdFormatString, bool productionValuesOk)
      {
         var output = new List<string>(12);

         IProtein protein;
         _proteinCollection.TryGetValue(benchmark.ProjectID, out protein);
         if (protein != null)
         {
            var calculateBonus = _prefs.GetPreference<bool>(Preference.CalculateBonus);

            output.Add(String.Empty);
            output.Add(String.Format(" Name: {0}", benchmark.OwningInstanceName));
            output.Add(String.Format(" Path: {0}", benchmark.OwningInstancePath));
            output.Add(String.Format(" Number of Frames Observed: {0}", benchmark.FrameTimes.Count));
            output.Add(String.Empty);
            output.Add(String.Format(" Min. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
               benchmark.MinimumFrameTime, protein.GetPPD(benchmark.MinimumFrameTime, calculateBonus)));
            output.Add(String.Format(" Avg. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
               benchmark.AverageFrameTime, protein.GetPPD(benchmark.AverageFrameTime, calculateBonus)));

            if (unitInfo != null && unitInfo.UnitInfoData.ProjectID.Equals(protein.ProjectNumber) && productionValuesOk)
            {
               output.Add(String.Format(" Cur. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                  unitInfo.TimePerLastSection, unitInfo.PPDPerLastSection));
               output.Add(String.Format(" R3F. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                  unitInfo.TimePerThreeSections, unitInfo.PPDPerThreeSections));
               output.Add(String.Format(" All  Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                  unitInfo.TimePerAllSections, unitInfo.PPDPerAllSections));
               output.Add(String.Format(" Eff. Time / Frame : {0} - {1:" + ppdFormatString + "} PPD",
                  unitInfo.TimePerUnitDownload, unitInfo.PPDPerUnitDownload));
            }

            output.Add(String.Empty);
         }
         else
         {
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} could not find Project ID '{1}'.", HfmTrace.FunctionName, benchmark.ProjectID));
         }

         return output.ToArray();
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
               _currentBenchmarkClient.NameAndPath, listBox1.SelectedItem),
                  Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            _benchmarkContainer.RefreshMinimumFrameTime(_currentBenchmarkClient, (int)listBox1.SelectedItem);
            listBox1_SelectedIndexChanged(sender, e);
         }
      }

      private void mnuContextDeleteProject_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, 
            "Are you sure you want to delete {0} - Project {1}?", 
               _currentBenchmarkClient.NameAndPath, listBox1.SelectedItem),
                  Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            _benchmarkContainer.Delete(_currentBenchmarkClient, (int)listBox1.SelectedItem);
            UpdateProjectListBoxBinding();
            if (_benchmarkContainer.ContainsClient(_currentBenchmarkClient) == false)
            {
               UpdateClientsComboBinding();
            }
         }
      }

      private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         HandleProcessStartResult(_processStarter.ShowWebBrowser(linkDescription.Text));
      }

      private void picDeleteClient_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to delete {0}?", _currentBenchmarkClient.NameAndPath),
                     Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            int currentIndex = cboClients.SelectedIndex;
            _benchmarkContainer.Delete(_currentBenchmarkClient);
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
         _prefs.SetPreference(Preference.BenchmarksClientsPerGraph, (int)udClientsPerGraph.Value);
      }

      private void SetClientsPerGraphUpDownEnabled()
      {
         udClientsPerGraph.Enabled = rdoClientsPerGraph.Checked;
      }

      private void btnExit_Click(object sender, EventArgs e)
      {
         Close();
      } 

      #endregion

      #region Update Routines

      private void UpdateBenchmarkText(IEnumerable<string> benchmarkLines)
      {
         var lines = new List<string>(txtBenchmarks.Lines);
         lines.AddRange(benchmarkLines);
         txtBenchmarks.Lines = lines.ToArray();
      }

      private string[] UpdateProteinInformation(int projectID)
      {
         var lines = new List<string>(5);

         IProtein protein;
         _proteinCollection.TryGetValue(projectID, out protein);

         if (protein != null)
         {
            txtProjectID.Text = protein.WorkUnitName;
            txtCredit.Text = protein.Credit.ToString();
            txtKFactor.Text = protein.KFactor.ToString();
            txtFrames.Text = protein.Frames.ToString();
            txtAtoms.Text = protein.NumAtoms.ToString();
            txtCore.Text = protein.Core;
            linkDescription.Text = protein.Description;
            txtPreferredDays.Text = protein.PreferredDays.ToString();
            txtMaximumDays.Text = protein.MaxDays.ToString();
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
            txtPreferredDays.Text = String.Empty;
            txtMaximumDays.Text = String.Empty;
            txtContact.Text = String.Empty;
            txtServerIP.Text = String.Empty;
         
            HfmTrace.WriteToHfmConsole(TraceLevel.Warning,
                                       String.Format("{0} could not find Project ID '{1}'.", HfmTrace.FunctionName, projectID));
         }

         return lines.ToArray();
      }

      #region Binding

      private void UpdateClientsComboBinding()
      {
         UpdateClientsComboBinding(-1);
      }

      private void UpdateClientsComboBinding(int index)
      {
         cboClients.DataBindings.Clear();
         cboClients.DataSource = _benchmarkContainer.GetBenchmarkClients();
         cboClients.DisplayMember = "NameAndPath";
         cboClients.ValueMember = "Client";
         
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
         if (_currentBenchmarkClient.AllClients)
         {
            listBox1.DataSource = _benchmarkContainer.GetBenchmarkProjects();
         }
         else
         {
            listBox1.DataSource = _benchmarkContainer.GetBenchmarkProjects(_currentBenchmarkClient);
         }
         
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

      private void HandleProcessStartResult(string message)
      {
         if (message != null)
         {
            _messageBoxView.ShowError(this, message, Text);
         }
      }
   }
}
