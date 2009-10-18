/*
 * HFM.NET - Benchmarks Form Class
 * Copyright (C) 2009 Ryan Harlamert (harlam357)
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

using HFM.Instances;
using HFM.Preferences;
using HFM.Proteins;
using HFM.Instrumentation;
using ZedGraph;

namespace HFM.Forms
{
   public partial class frmBenchmarks : Classes.FormWrapper
   {
      #region Members
      private readonly InstanceCollection _clientInstances;
      private readonly int _initialProjectID; 
      
      private BenchmarkClient _currentBenchmarkClient;
      #endregion

      #region Form Constructor / functionality
      public frmBenchmarks(InstanceCollection clientInstances, int projectID)
      {
         _clientInstances = clientInstances;
         _initialProjectID = projectID;
      
         InitializeComponent();
      }

      private void frmBenchmarks_Shown(object sender, EventArgs e)
      {
         UpdateClientsComboBinding();
         UpdateProjectListBoxBinding(_initialProjectID);
      }
      #endregion

      #region Event Handlers
      private void cboClients_SelectedIndexChanged(object sender, EventArgs e)
      {
         _currentBenchmarkClient = (BenchmarkClient)cboClients.SelectedValue;
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

         List<InstanceProteinBenchmark> list = ProteinBenchmarkCollection.Instance.GetBenchmarks(_currentBenchmarkClient, ProjectID);

         foreach (InstanceProteinBenchmark benchmark in list)
         {
            ClientInstance Instance;
            _clientInstances.Instances.TryGetValue(benchmark.OwningInstanceName, out Instance);
            if (Instance != null && Instance.Owns(benchmark) == false)
            {
               Instance = null;
            }
            UpdateBenchmarkText(benchmark.ToMultiLineString(Instance));
         }

         CreatePpdGraph(zg1, lines, list);
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
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to refresh {0} - Project {1} minimum frame time?", _currentBenchmarkClient.NameAndPath, listBox1.SelectedItem),
                              Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            ProteinBenchmarkCollection.Instance.RefreshMinimumFrameTime(_currentBenchmarkClient, (int)listBox1.SelectedItem);
            listBox1_SelectedIndexChanged(sender, e);
         }
      }

      private void mnuContextDeleteProject_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to delete {0} - Project {1}?", _currentBenchmarkClient.NameAndPath, listBox1.SelectedItem),
                              Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            ProteinBenchmarkCollection.Instance.Delete(_currentBenchmarkClient, (int)listBox1.SelectedItem);
            UpdateProjectListBoxBinding();
            if (ProteinBenchmarkCollection.Instance.ContainsClient(_currentBenchmarkClient) == false)
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
            MessageBox.Show(String.Format(Properties.Resources.ProcessStartError, "Project Description"));
         }
      }

      private void picDeleteClient_Click(object sender, EventArgs e)
      {
         if (MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture, "Are you sure you want to delete {0}?", _currentBenchmarkClient.NameAndPath),
                     Text, MessageBoxButtons.YesNo, MessageBoxIcon.Question).Equals(DialogResult.Yes))
         {
            int currentIndex = cboClients.SelectedIndex;
            ProteinBenchmarkCollection.Instance.Delete(_currentBenchmarkClient);
            UpdateClientsComboBinding(currentIndex);
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

         Protein protein;
         ProteinCollection.Instance.TryGetValue(ProjectID, out protein);

         if (protein != null)
         {
            txtProjectID.Text = protein.WorkUnitName;
            txtCredit.Text = protein.Credit.ToString();
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
         cboClients.DataSource = ProteinBenchmarkCollection.Instance.GetBenchmarkClients();
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
            listBox1.DataSource = ProteinBenchmarkCollection.Instance.GetBenchmarkProjects();
         }
         else
         {
            listBox1.DataSource = ProteinBenchmarkCollection.Instance.GetBenchmarkProjects(_currentBenchmarkClient);
         }
         
         int index = listBox1.Items.IndexOf(InitialProjectID);
         if (index > -1)
         {
            listBox1.SelectedIndex = index;
         }
      }

      /// <summary>
      /// Build The GraphPane
      /// </summary>
      /// <param name="zg">ZedGraph Control</param>
      /// <param name="benchmarks">Benchmarks Collection to Plot</param>
      private static void CreatePpdGraph(ZedGraphControl zg, string[] ProjectInfo, IEnumerable<InstanceProteinBenchmark> benchmarks)
      {
         // get a reference to the GraphPane
         GraphPane myPane = zg.GraphPane;
         
         // Clear the bars
         myPane.CurveList.Clear();
         // Clear the bar labels
         myPane.GraphObjList.Clear();

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
            yPoints[0] = Math.Round(benchmark.MinimumFrameTimePPD, PreferenceSet.Instance.DecimalPlaces);
            yPoints[1] = Math.Round(benchmark.AverageFrameTimePPD, PreferenceSet.Instance.DecimalPlaces);

            CreateBar(i, myPane, benchmark.OwningInstanceName, yPoints);
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
         string[] labels = new string[] { "Min. Frame Time", "Avg. Frame Time" };
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

         // Tell ZedGraph to refigure the
         // axes since the data have changed
         zg.AxisChange();
         // Refresh the control
         zg.Refresh();
      }
      
      private static void CreateBar(int index, GraphPane myPane, string InstanceName, double[] y)
      {
         Color barColor;
         switch (index % 6)
         {
            case 0:
               barColor = Color.Red;
               break;
            case 1:
               barColor = Color.Blue;
               break;
            case 2:
               barColor = Color.Green;
               break;
            case 3:
               barColor = Color.Maroon;
               break;
            case 4:
               barColor = Color.DarkSlateBlue;
               break;
            case 5:
               barColor = Color.MediumAquamarine;
               break;
            case 6:
               barColor = Color.DarkViolet;
               break;
            case 7:
               barColor = Color.DeepSkyBlue;
               break;
            case 8:
               barColor = Color.DarkTurquoise;
               break;
            default:
               throw new InvalidOperationException("Logic Error in Bar Color selection.");
         }

         // Generate a bar with the Instance Name in the legend
         BarItem myBar = myPane.AddBar(InstanceName, null, y, barColor);
         myBar.Bar.Fill = new Fill(barColor, Color.White, barColor);
      }

      #endregion
   }
}
