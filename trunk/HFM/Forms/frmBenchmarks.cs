/*
 * HFM.NET - Benchmarks Form Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
using System.Text;
using System.Windows.Forms;

using ZedGraph;

using HFM.Framework;
using HFM.Instances;
using HFM.Instrumentation;

namespace HFM.Forms
{
   public partial class frmBenchmarks : Classes.FormWrapper
   {
      #region Members
      private readonly IPreferenceSet _Prefs;
      private readonly IProteinBenchmarkContainer _Benchmark;
      private readonly List<Color> _GraphColors;
      private readonly InstanceCollection _clientInstances;
      private readonly int _initialProjectID; 
      
      private IBenchmarkClient _currentBenchmarkClient;
      #endregion

      #region Form Constructor / functionality
      public frmBenchmarks(IPreferenceSet Prefs, IProteinBenchmarkContainer Benchmark, InstanceCollection clientInstances, int projectID)
      {
         _Prefs = Prefs;
         _Benchmark = Benchmark;
         _GraphColors = _Prefs.GetPreference<List<Color>>(Preference.GraphColors);
         _clientInstances = clientInstances;
         _initialProjectID = projectID;
      
         InitializeComponent();
      }

      private void frmBenchmarks_Shown(object sender, EventArgs e)
      {
         UpdateClientsComboBinding();
         UpdateProjectListBoxBinding(_initialProjectID);
         lstColors.DataSource = _GraphColors;
         
         // Issue 154 - make sure focus is on the projects list box
         listBox1.Select();
      }

      private void frmBenchmarks_FormClosing(object sender, FormClosingEventArgs e)
      {
         // Save state data
         _Prefs.SetPreference(Preference.BenchmarksFormLocation, Location);
         _Prefs.SetPreference(Preference.BenchmarksFormSize, Size);
         _Prefs.Save();
      }
      #endregion

      #region Event Handlers
      private void cboClients_SelectedIndexChanged(object sender, EventArgs e)
      {
         _currentBenchmarkClient = (IBenchmarkClient)cboClients.SelectedValue;
         if (_currentBenchmarkClient.AllClients)
         {
            picDeleteClient.Visible = false;
         }
         else
         {
            picDeleteClient.Visible = true;
         }
         
         UpdateProjectListBoxBinding();
      }
      
      private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
      {
         txtBenchmarks.Text = String.Empty;
         int ProjectID = (int)listBox1.SelectedItem;

         string[] lines = UpdateProteinInformation(ProjectID);

         UpdateBenchmarkText(lines);

         List<IInstanceProteinBenchmark> list = _Benchmark.GetBenchmarks(_currentBenchmarkClient, ProjectID);
         list.Sort(delegate(IInstanceProteinBenchmark benchmark1, IInstanceProteinBenchmark benchmark2)
         {
            return benchmark1.OwningInstanceName.CompareTo(benchmark2.OwningInstanceName);
         });

         foreach (InstanceProteinBenchmark benchmark in list)
         {
            IUnitInfoLogic unit = null;
            bool valuesOk = false;
         
            ClientInstance Instance;
            _clientInstances.Instances.TryGetValue(benchmark.OwningInstanceName, out Instance);
            if (Instance != null && Instance.Owns(benchmark))
            {
               unit = Instance.CurrentUnitInfo;
               valuesOk = Instance.ProductionValuesOk;
            }
            UpdateBenchmarkText(benchmark.ToMultiLineString(unit, _Prefs.PpdFormatString, valuesOk));
         }

         CreateFrameTimeGraph(zgFrameTime, lines, list, _GraphColors);
         CreatePpdGraph(zgPpd, lines, list, _GraphColors, _Prefs.GetPreference<int>(Preference.DecimalPlaces));
      }

      private void listBox1_MouseDown(object sender, MouseEventArgs e)
      {
         if (e.Button == System.Windows.Forms.MouseButtons.Right)
         {
            listBox1.SelectedIndex = listBox1.IndexFromPoint(e.X, e.Y);
         }
      }

      private void listBox1_MouseUp(object sender, MouseEventArgs e)
      {
         if (listBox1.SelectedIndex == -1) return;

         if (e.Button == System.Windows.Forms.MouseButtons.Right)
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
            _Benchmark.RefreshMinimumFrameTime(_currentBenchmarkClient, (int)listBox1.SelectedItem);
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
            _Benchmark.Delete(_currentBenchmarkClient, (int)listBox1.SelectedItem);
            UpdateProjectListBoxBinding();
            if (_Benchmark.ContainsClient(_currentBenchmarkClient) == false)
            {
               UpdateClientsComboBinding();
            }
         }
      }

      private void linkDescription_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
      {
         try
         {
            Process.Start(linkDescription.Text);
         }
         catch (Exception ex)
         {
            HfmTrace.WriteToHfmConsole(ex);
            MessageBox.Show(String.Format(CultureInfo.CurrentCulture, Properties.Resources.ProcessStartError, "Project Description"));
         }
      }

      private void picDeleteClient_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to delete {0}?", _currentBenchmarkClient.NameAndPath),
                     Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            int currentIndex = cboClients.SelectedIndex;
            _Benchmark.Delete(_currentBenchmarkClient);
            UpdateClientsComboBinding(currentIndex);
         }
      }

      private void lstColors_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (lstColors.SelectedIndex == -1) return;
         
         picColorPreview.BackColor = _GraphColors[lstColors.SelectedIndex];
      }

      private void btnMoveColorUp_Click(object sender, EventArgs e)
      {
         if (lstColors.SelectedIndex == -1)
         {
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (lstColors.SelectedIndex == 0) return;

         int index = lstColors.SelectedIndex;
         Color moveColor = _GraphColors[index];
         _GraphColors.RemoveAt(index);
         _GraphColors.Insert(index - 1, moveColor);
         UpdateGraphColorsBinding();
         lstColors.SelectedIndex = index - 1;
      }

      private void btnMoveColorDown_Click(object sender, EventArgs e)
      {
         if (lstColors.SelectedIndex == -1)
         {
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (lstColors.SelectedIndex == _GraphColors.Count - 1) return;
         
         int index = lstColors.SelectedIndex;
         Color moveColor = _GraphColors[index];
         _GraphColors.RemoveAt(index);
         _GraphColors.Insert(index + 1, moveColor);
         UpdateGraphColorsBinding();
         lstColors.SelectedIndex = index + 1;
      }

      private void btnAddColor_Click(object sender, EventArgs e)
      {
         ColorDialog dlg = new ColorDialog();
         if (dlg.ShowDialog(this).Equals(DialogResult.OK))
         {
            Color addColor = FindNearestKnown(dlg.Color);
            if (_GraphColors.Contains(addColor))
            {
               MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "{0} is already a graph color.", addColor.Name), 
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
               return;
            }

            _GraphColors.Add(addColor);
            UpdateGraphColorsBinding();
            lstColors.SelectedIndex = _GraphColors.Count - 1;
         }
      }

      private void btnDeleteColor_Click(object sender, EventArgs e)
      {
         if (lstColors.SelectedIndex == -1)
         {
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (_GraphColors.Count <= 3)
         {
            MessageBox.Show(this, "Must have at least three colors.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         int index = lstColors.SelectedIndex;
         _GraphColors.RemoveAt(index);
         UpdateGraphColorsBinding();
         if (index == _GraphColors.Count)
         {
            lstColors.SelectedIndex = _GraphColors.Count - 1;
         }
      }

      private void btnExit_Click(object sender, EventArgs e)
      {
         Close();
      } 
      #endregion

      #region Helper Routines
      private void UpdateBenchmarkText(IEnumerable<string> benchmarkLines)
      {
         List<string> lines = new List<string>(txtBenchmarks.Lines);
         lines.AddRange(benchmarkLines);
         txtBenchmarks.Lines = lines.ToArray();
      }

      private string[] UpdateProteinInformation(int ProjectID)
      {
         List<string> lines = new List<string>(5);

         IProtein protein;
         InstanceProvider.GetInstance<IProteinCollection>().TryGetValue(ProjectID, out protein);

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
                                       String.Format("{0} could not find Project ID '{1}'.", HfmTrace.FunctionName, ProjectID));
         }

         return lines.ToArray();
      }

      private void UpdateClientsComboBinding()
      {
         UpdateClientsComboBinding(-1);
      }

      private void UpdateClientsComboBinding(int index)
      {
         cboClients.DataBindings.Clear();
         cboClients.DataSource = _Benchmark.GetBenchmarkClients();
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

      private void UpdateProjectListBoxBinding(int InitialProjectID)
      {
         listBox1.DataBindings.Clear();
         if (_currentBenchmarkClient.AllClients)
         {
            listBox1.DataSource = _Benchmark.GetBenchmarkProjects();
         }
         else
         {
            listBox1.DataSource = _Benchmark.GetBenchmarkProjects(_currentBenchmarkClient);
         }
         
         int index = listBox1.Items.IndexOf(InitialProjectID);
         if (index > -1)
         {
            listBox1.SelectedIndex = index;
         }
      }

      private void UpdateGraphColorsBinding()
      {
         CurrencyManager cm = (CurrencyManager)lstColors.BindingContext[lstColors.DataSource];
         cm.Refresh();
      }

      /// <summary>
      /// Build The PPD GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="ProjectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="GraphColors">Graph Colors List</param>
      /// <param name="DecimalPlaces">PPD Decimal Places</param>
      private static void CreatePpdGraph(ZedGraphControl zg, string[] ProjectInfo, IEnumerable<IInstanceProteinBenchmark> benchmarks, 
                                         IList<Color> GraphColors, int DecimalPlaces)
      {
         Debug.Assert(zg != null);
         
         try
         {
            // get a reference to the GraphPane
            GraphPane myPane = zg.GraphPane;

            // Clear the bars
            myPane.CurveList.Clear();
            // Clear the bar labels
            myPane.GraphObjList.Clear();
            // Clear the XAxis Project Information
            myPane.XAxis.Title.Text = String.Empty;

            // If no Project Information, get out
            if (ProjectInfo.Length == 0)
            {
               return;
            }

            // Scale YAxis In Thousands?
            bool InThousands = false;

            // Create the bars for each benchmark
            int i = 0;
            foreach (InstanceProteinBenchmark benchmark in benchmarks)
            {
               if (benchmark.MinimumFrameTimePPD >= 1000 || benchmark.AverageFrameTimePPD >= 1000)
               {
                  InThousands = true;
               }

               double[] yPoints = new double[2];
               yPoints[0] = Math.Round(benchmark.MinimumFrameTimePPD, DecimalPlaces);
               yPoints[1] = Math.Round(benchmark.AverageFrameTimePPD, DecimalPlaces);

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, GraphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < ProjectInfo.Length - 2; i++)
            {
               sb.Append(ProjectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(ProjectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "PPD";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            string[] labels = new string[] {"Min. Frame Time", "Avg. Frame Time"};
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Don't show YAxis.Scale as 10^3         
            myPane.YAxis.Scale.MagAuto = false;
            // Set the YAxis Steps
            if (InThousands)
            {
               myPane.YAxis.Scale.MajorStep = 1000;
               myPane.YAxis.Scale.MinorStep = 500;
            }
            else
            {
               myPane.YAxis.Scale.MajorStep = 100;
               myPane.YAxis.Scale.MinorStep = 10;
            }

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));
         }
         finally
         {
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zg.AxisChange();
            // Refresh the control
            zg.Refresh();
         }
      }
      
      /// <summary>
      /// Build The Frame Time GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="ProjectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="GraphColors">Graph Colors List</param>
      private static void CreateFrameTimeGraph(ZedGraphControl zg, string[] ProjectInfo, IEnumerable<IInstanceProteinBenchmark> benchmarks,
                                               IList<Color> GraphColors)
      {
         Debug.Assert(zg != null);

         try
         {
            // get a reference to the GraphPane
            GraphPane myPane = zg.GraphPane;

            // Clear the bars
            myPane.CurveList.Clear();
            // Clear the bar labels
            myPane.GraphObjList.Clear();
            // Clear the XAxis Project Information
            myPane.XAxis.Title.Text = String.Empty;

            // If no Project Information, get out
            if (ProjectInfo.Length == 0)
            {
               return;
            }

            // Create the bars for each benchmark
            int i = 0;
            foreach (InstanceProteinBenchmark benchmark in benchmarks)
            {
               double[] yPoints = new double[2];
               yPoints[0] = benchmark.MinimumFrameTime.TotalSeconds;
               yPoints[1] = benchmark.AverageFrameTime.TotalSeconds;

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, GraphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            StringBuilder sb = new StringBuilder();
            for (i = 0; i < ProjectInfo.Length - 2; i++)
            {
               sb.Append(ProjectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(ProjectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "Frame Time (Seconds)";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            string[] labels = new string[] {"Min. Frame Time", "Avg. Frame Time"};
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Fill the Axis and Pane backgrounds
            myPane.Chart.Fill = new Fill(Color.White, Color.FromArgb(255, 255, 166), 90F);
            myPane.Fill = new Fill(Color.FromArgb(250, 250, 255));
         }
         finally
         {
            // Tell ZedGraph to refigure the
            // axes since the data have changed
            zg.AxisChange();
            // Refresh the control
            zg.Refresh();
         }
      }

      private static void CreateBar(int index, GraphPane myPane, string InstanceName, double[] y, IList<Color> GraphColors)
      {
         int ColorIndex = index % GraphColors.Count;
         Color barColor = GraphColors[ColorIndex];

         // Generate a bar with the Instance Name in the legend
         BarItem myBar = myPane.AddBar(InstanceName, null, y, barColor);
         myBar.Bar.Fill = new Fill(barColor, Color.White, barColor);
      }

      private static Color FindNearestKnown(Color c)
      {
         ColorName best = new ColorName();
         best.Name = null;

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
      #endregion
   }

   /// <summary>
   /// Container for Color, Name, 
   /// </summary>
   internal struct ColorName
   {
      internal Color Color;
      internal string Name;
      internal int Distance;
   }
}
