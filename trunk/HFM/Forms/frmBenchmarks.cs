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
using System.Windows.Forms;

using HFM.Instances;
using HFM.Proteins;
using HFM.Instrumentation;

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
            ProteinBenchmarkCollection.Instance.Delete(_currentBenchmarkClient);
            UpdateClientsComboBinding();
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
         cboClients.DataBindings.Clear();
         cboClients.DataSource = ProteinBenchmarkCollection.Instance.GetBenchmarkClients();
         cboClients.DisplayMember = "NameAndPath";
         cboClients.ValueMember = "Client";
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
      #endregion
   }
}
