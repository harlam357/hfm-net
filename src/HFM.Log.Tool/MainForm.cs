
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

using HFM.Core.DataTypes;

namespace HFM.Log.Tool
{
   public partial class MainForm : Form
   {
      private IList<LogLine> _logLines = new List<LogLine>();
      private IList<ClientRun> _clientRuns;
   
      public MainForm()
      {
         InitializeComponent();

         base.Text = String.Format("HFM Log Tool v{0}", Core.Application.VersionWithRevision);
#if !DEV
         btnGenCode.Visible = false;
#endif
      }

      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         int index = -1;

         TreeNode node = e.Node;
         if (node.Level == 0)
         {
            index = _clientRuns[int.Parse(node.Name)].ClientStartIndex;
         }
         else if (node.Level == 1)
         {
            TreeNode parent = node.Parent;
            index = _clientRuns[int.Parse(parent.Name)].UnitIndexes[int.Parse(node.Name)].StartIndex;
         }

         if (index > -1)
         {
            richTextBox1.SelectionStart = 0;
            richTextBox1.ScrollToCaret();
            richTextBox1.ScrollToLine(index);
            SelectLogLine(index);
         }
      }

      private void richTextBox1_MouseDown(object sender, MouseEventArgs e)
      {
         int firstCharIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
         int index = richTextBox1.GetLineFromCharIndex(firstCharIndex);
         SelectLogLine(index);
      }

      private void richTextBox1_KeyUp(object sender, KeyEventArgs e)
      {
         int firstCharIndex = richTextBox1.GetFirstCharIndexOfCurrentLine();
         int index = richTextBox1.GetLineFromCharIndex(firstCharIndex);
         SelectLogLine(index);
      }

      private void SelectLogLine(int index)
      {
         if (index >= 0 && index < _logLines.Count)
         {
            var logLine = _logLines[index];

            txtLogLineIndex.Text = logLine.LineIndex.ToString();
            txtLogLineType.Text = logLine.LineType.ToString();
            txtLogLineData.Text = logLine.LineData != null ? logLine.LineData.ToString() : String.Empty;
         }
      }

      private void btnBrowse_Click(object sender, EventArgs e)
      {
         using (var dlg = new OpenFileDialog())
         {
            string path = GetDirectoryName(txtLogPath.Text);
            if (!String.IsNullOrEmpty(path))
            {
               dlg.InitialDirectory = txtLogPath.Text;
            }
            if (dlg.ShowDialog(this).Equals(DialogResult.OK))
            {
               txtLogPath.Text = dlg.FileName;
            }
         }
      }

      private static string GetDirectoryName(string path)
      {
         try
         {
            return Path.GetDirectoryName(path);
         }
         catch (Exception)
         {
            return null;
         }
      }

      private void btnParse_Click(object sender, EventArgs e)
      {
         if (txtLogPath.Text.Length == 0) return;

         treeView1.Nodes.Clear();

         if (File.Exists(txtLogPath.Text))
         {
            LogFileType logFileType = GetLogFileType();
            _logLines = LogReader.GetLogLines(txtLogPath.Text, logFileType);
            _clientRuns = LogReader.GetClientRuns(_logLines, logFileType);

            PopulateClientRunsInTree(_clientRuns);
            richTextBox1.SetLogLines(_logLines, String.Empty, true);
         }
         else
         {
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "File '{0}' does not exist.", txtLogPath.Text), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private LogFileType GetLogFileType()
      {
         return LegacyRadioButton.Checked ? LogFileType.Legacy : LogFileType.Version7;
      }

      private void PopulateClientRunsInTree(IList<ClientRun> clientRunList)
      {
         for (int i = 0; i < clientRunList.Count; i++)
         {
            treeView1.Nodes.Add(i.ToString(), "Run " + i);
            for (int j = 0; j < clientRunList[i].UnitIndexes.Count; j++)
            {
               treeView1.Nodes[i].Nodes.Add(j.ToString(), String.Format(CultureInfo.InvariantCulture,
                  "Queue ({0}) Line ({1}) Index", clientRunList[i].UnitIndexes[j].QueueIndex, clientRunList[i].UnitIndexes[j].StartIndex));
            }
         }
      }

      private void btnGenCode_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         for (int i = 0; i < _clientRuns.Count; i++)
         {
            sb.AppendLine("// Check Run " + i + " Positions");
            sb.AppendLine("var expectedRun = new ClientRun(" + _clientRuns[i].ClientStartIndex + ");");
            for (int j = 0; j < _clientRuns[i].UnitIndexes.Count; j++)
            {
               var index = _clientRuns[i].UnitIndexes[j];
               sb.AppendLine("expectedRun.UnitIndexes.Add(new UnitIndex(" + index.QueueIndex + "," + index.StartIndex + "," + index.EndIndex + "));");
            }
            sb.AppendLine("expectedRun.Arguments = \"" + _clientRuns[i].Arguments + "\";");
            sb.AppendLine("expectedRun.FoldingID = \"" + _clientRuns[i].FoldingID + "\";");
            sb.AppendLine("expectedRun.Team = " + _clientRuns[i].Team + ";");
            sb.AppendLine("expectedRun.UserID = \"" + _clientRuns[i].UserID + "\";");
            sb.AppendLine("expectedRun.MachineID = " + _clientRuns[i].MachineID + ";");
            sb.AppendLine("expectedRun.CompletedUnits = " + _clientRuns[i].CompletedUnits + ";");
            sb.AppendLine("expectedRun.FailedUnits = " + _clientRuns[i].FailedUnits + ";");
            sb.AppendLine("expectedRun.TotalCompletedUnits = " + _clientRuns[i].TotalCompletedUnits + ";");
            sb.AppendLine("expectedRun.Status = ClientStatus." + _clientRuns[i].Status + ";");
            sb.AppendLine();
            sb.AppendLine("DoClientRunCheck(expectedRun, clientRuns[" + i + "]);");
         }

         var form2 = new TextDialog();
         form2.SetText(sb.ToString());
         form2.Show();
      }
   }
}
