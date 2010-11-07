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

using HFM.Instances;
using HFM.Framework;
using HFM.Framework.DataTypes;

namespace HFM.Forms
{
   // ReSharper disable InconsistentNaming
   public partial class frmBenchmarks : Classes.FormWrapper
   // ReSharper restore InconsistentNaming
   {
      #region Members
      private readonly IPreferenceSet _prefs;
      private readonly IProteinCollection _proteinCollection;
      private readonly IProteinBenchmarkContainer _benchmarkContainer;
      private readonly List<Color> _graphColors;
      private readonly InstanceCollection _instanceCollection;
      private readonly int _initialProjectID; 
      
      private IBenchmarkClient _currentBenchmarkClient;
      #endregion

      #region Form Constructor / functionality
      public frmBenchmarks(IPreferenceSet prefs, IProteinCollection proteinCollection, IProteinBenchmarkContainer benchmarkContainer, 
                           InstanceCollection instanceCollection, int projectID)
      {
         _prefs = prefs;
         _proteinCollection = proteinCollection;
         _benchmarkContainer = benchmarkContainer;
         _graphColors = _prefs.GetPreference<List<Color>>(Preference.GraphColors);
         _instanceCollection = instanceCollection;
         _initialProjectID = projectID;
      
         InitializeComponent();
      }

      private void frmBenchmarks_Shown(object sender, EventArgs e)
      {
         UpdateClientsComboBinding();
         UpdateProjectListBoxBinding(_initialProjectID);
         lstColors.DataSource = _graphColors;
         
         // Issue 154 - make sure focus is on the projects list box
         listBox1.Select();
      }

      private void frmBenchmarks_FormClosing(object sender, FormClosingEventArgs e)
      {
         // Save state data
         _prefs.SetPreference(Preference.BenchmarksFormLocation, Location);
         _prefs.SetPreference(Preference.BenchmarksFormSize, Size);
         _prefs.Save();
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
         int projectID = (int)listBox1.SelectedItem;

         string[] lines = UpdateProteinInformation(projectID);

         UpdateBenchmarkText(lines);

         List<IInstanceProteinBenchmark> list = _benchmarkContainer.GetBenchmarks(_currentBenchmarkClient, projectID);
         list.Sort(delegate(IInstanceProteinBenchmark benchmark1, IInstanceProteinBenchmark benchmark2)
         {
            return benchmark1.OwningInstanceName.CompareTo(benchmark2.OwningInstanceName);
         });

         foreach (InstanceProteinBenchmark benchmark in list)
         {
            IUnitInfoLogic unit = null;
            bool valuesOk = false;

            var instance = _instanceCollection[benchmark.OwningInstanceName];
            if (instance != null && instance.Owns(benchmark))
            {
               unit = instance.CurrentUnitInfo;
               valuesOk = instance.ProductionValuesOk;
            }
            UpdateBenchmarkText(benchmark.ToMultiLineString(unit, _prefs.PpdFormatString, valuesOk));
         }

         CreateFrameTimeGraph(zgFrameTime, lines, list, _graphColors);
         CreatePpdGraph(zgPpd, lines, list, _graphColors, _prefs.GetPreference<int>(Preference.DecimalPlaces));
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
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
         ColorDialog dlg = new ColorDialog();
         if (dlg.ShowDialog(this).Equals(DialogResult.OK))
         {
            Color addColor = FindNearestKnown(dlg.Color);
            if (_graphColors.Contains(addColor))
            {
               MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "{0} is already a graph color.", addColor.Name), 
                  Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            MessageBox.Show(this, "No Color Selected.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
         }

         if (_graphColors.Count <= 3)
         {
            MessageBox.Show(this, "Must have at least three colors.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
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

      private void btnExit_Click(object sender, EventArgs e)
      {
         Close();
      } 
      #endregion

      #region Helper Routines
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

      /// <summary>
      /// Build The PPD GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="projectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="graphColors">Graph Colors List</param>
      /// <param name="decimalPlaces">PPD Decimal Places</param>
      private static void CreatePpdGraph(ZedGraphControl zg, string[] projectInfo, IEnumerable<IInstanceProteinBenchmark> benchmarks, 
                                         IList<Color> graphColors, int decimalPlaces)
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
            if (projectInfo.Length == 0)
            {
               return;
            }

            // Scale YAxis In Thousands?
            bool inThousands = false;

            // Create the bars for each benchmark
            int i = 0;
            foreach (InstanceProteinBenchmark benchmark in benchmarks)
            {
               if (benchmark.MinimumFrameTimePPD >= 1000 || benchmark.AverageFrameTimePPD >= 1000)
               {
                  inThousands = true;
               }

               double[] yPoints = new double[2];
               yPoints[0] = Math.Round(benchmark.MinimumFrameTimePPD, decimalPlaces);
               yPoints[1] = Math.Round(benchmark.AverageFrameTimePPD, decimalPlaces);

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, graphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            var sb = new StringBuilder();
            for (i = 0; i < projectInfo.Length - 2; i++)
            {
               sb.Append(projectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(projectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "PPD";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            var labels = new[] { "Min. Frame Time", "Avg. Frame Time" };
            myPane.XAxis.Scale.TextLabels = labels;
            // Set the XAxis to Text type
            myPane.XAxis.Type = AxisType.Text;

            // Don't show YAxis.Scale as 10^3         
            myPane.YAxis.Scale.MagAuto = false;
            // Set the YAxis Steps
            if (inThousands)
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
      /// <param name="projectInfo">Project Info Array</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      /// <param name="graphColors">Graph Colors List</param>
      private static void CreateFrameTimeGraph(ZedGraphControl zg, string[] projectInfo, IEnumerable<IInstanceProteinBenchmark> benchmarks,
                                               IList<Color> graphColors)
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
            if (projectInfo.Length == 0)
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

               CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints, graphColors);
               i++;
            }

            // Create the bar labels
            BarItem.CreateBarLabels(myPane, true, String.Empty, zg.Font.Name, zg.Font.Size, Color.Black, true, false, false);

            // Set the Titles
            myPane.Title.Text = "HFM.NET - Client Benchmarks";
            var sb = new StringBuilder();
            for (i = 0; i < projectInfo.Length - 2; i++)
            {
               sb.Append(projectInfo[i]);
               sb.Append(" / ");
            }
            sb.Append(projectInfo[i]);
            myPane.XAxis.Title.Text = sb.ToString();
            myPane.YAxis.Title.Text = "Frame Time (Seconds)";

            // Draw the X tics between the labels instead of at the labels
            myPane.XAxis.MajorTic.IsBetweenLabels = true;
            // Set the XAxis labels
            var labels = new[] { "Min. Frame Time", "Avg. Frame Time" };
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

      private static void CreateBar(int index, GraphPane myPane, string instanceName, double[] y, IList<Color> graphColors)
      {
         int colorIndex = index % graphColors.Count;
         Color barColor = graphColors[colorIndex];

         // Generate a bar with the Instance Name in the legend
         BarItem myBar = myPane.AddBar(instanceName, null, y, barColor);
         myBar.Bar.Fill = new Fill(barColor, Color.White, barColor);
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
