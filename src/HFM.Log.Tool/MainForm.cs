
#define V1

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using HFM.Core.DataTypes;

namespace HFM.Log.Tool
{
   public partial class MainForm : Form
   {
      private IList<LogLine> _logLines = new List<LogLine>();
#if V1
      private IList<ClientRun> _clientRuns;
#else
      private FahLog _fahLog;
#endif

      public MainForm()
      {
         InitializeComponent();

         base.Text = String.Format("HFM Log Tool v{0}", Core.Application.VersionWithRevision);
#if !DEV
         btnGenCode.Visible = false;
#endif
      }

#if V1
      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         int index = -1;

         TreeNode node = e.Node;
         if (node.Level == 0)
         {
            index = _clientRuns[int.Parse(node.Name)].ClientStartIndex;
         }
         else if (node.Level == 2)
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
#else
      private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
      {
         int index = -1;

         TreeNode node = e.Node;
         if (node.Level == 0)
         {
            index = _fahLog.ClientRuns.Reverse().ElementAt(Int32.Parse(node.Name)).ClientStartIndex;
         }
         else if (node.Level == 2)
         {
            TreeNode slotParent = node.Parent;
            TreeNode runParent = slotParent.Parent;
            index = (int)_fahLog.ClientRuns.Reverse().ElementAt(Int32.Parse(runParent.Name)).SlotRuns[Int32.Parse(slotParent.Name)].UnitRuns.Reverse().ElementAt(Int32.Parse(node.Name)).StartIndex;
         }

         if (index > -1)
         {
            richTextBox1.SelectionStart = 0;
            richTextBox1.ScrollToCaret();
            richTextBox1.ScrollToLine(index);
            SelectLogLine(index);
         }
      }
#endif

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
#if V1
            var sw = Stopwatch.StartNew();
            _logLines = LogReader.GetLogLines(txtLogPath.Text, logFileType).ToList();
            sw.Stop();
            Debug.WriteLine("GetLogLines ET: {0}", sw.Elapsed);
            sw = Stopwatch.StartNew();
            _clientRuns = LogReader.GetClientRuns(_logLines, logFileType);
            sw.Stop();
            Debug.WriteLine("GetClientRuns ET: {0}", sw.Elapsed);
            PopulateClientRunsInTree(_clientRuns);
#else
            var sw = Stopwatch.StartNew();
            _fahLog = LogReader2.GetFahLog(File.ReadLines(txtLogPath.Text), logFileType);
            sw.Stop();
            Debug.WriteLine("GetFahLog ET: {0}", sw.Elapsed);
            _logLines = RestoreOrderedLog(_fahLog);
            PopulateClientRunsInTree(_fahLog);
#endif
            richTextBox1.SetLogLines(_logLines, String.Empty, true);
         }
         else
         {
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "File '{0}' does not exist.", txtLogPath.Text), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private static IList<LogLine> RestoreOrderedLog(FahLog fahLog)
      {
         var lines1 = fahLog.ClientRuns.SelectMany(x => x.LogLines);
         var lines2 = fahLog.ClientRuns.SelectMany(x => x.SlotRuns).Select(x => x.Value).SelectMany(x => x.UnitRuns).SelectMany(x => x.LogLines);
         var logLines = lines1.Concat(lines2).ToList();
         logLines.Sort((x, y) => x.LineIndex.CompareTo(y.LineIndex));
         return logLines;
      }

      private LogFileType GetLogFileType()
      {
         return LegacyRadioButton.Checked ? LogFileType.Legacy : LogFileType.FahClient;
      }

#if V1
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
#else
      private void PopulateClientRunsInTree(FahLog fahLog)
      {
         int i = 0;
         foreach (var clientRun in fahLog.ClientRuns.Reverse())
         {
            treeView1.Nodes.Add(i.ToString(), "Run " + i);
            foreach (var slotRun in clientRun.SlotRuns.Values)
            {
               treeView1.Nodes[i].Nodes.Add(slotRun.FoldingSlot.ToString(), String.Format(CultureInfo.InvariantCulture,
                  "Slot {0}", slotRun.FoldingSlot));
               int j = 0;
               foreach (var unitRun in slotRun.UnitRuns.Reverse())
               {
                  treeView1.Nodes[i].Nodes[slotRun.FoldingSlot.ToString()].Nodes.Add(j.ToString(), String.Format(CultureInfo.InvariantCulture,
                     "Queue ({0}) Line ({1}) Index", unitRun.QueueIndex, unitRun.StartIndex));
                  j++;
               }
            }
            i++;
         }
      }
#endif

      private void btnGenCode_Click(object sender, EventArgs e)
      {
         //var sb = new StringBuilder();
         //for (int i = 0; i < _clientRuns.Count; i++)
         //{
         //   sb.AppendLine("// Check Run " + i + " Positions");
         //   sb.AppendLine("var expectedRun = new ClientRun(" + _clientRuns[i].ClientStartIndex + ");");
         //   for (int j = 0; j < _clientRuns[i].UnitIndexes.Count; j++)
         //   {
         //      var index = _clientRuns[i].UnitIndexes[j];
         //      sb.AppendLine("expectedRun.UnitIndexes.Add(new UnitIndex(" + index.QueueIndex + "," + index.StartIndex + "," + index.EndIndex + "));");
         //   }
         //   sb.AppendLine("expectedRun.ClientVersion = \"" + _clientRuns[i].ClientVersion + "\";");
         //   sb.AppendLine("expectedRun.Arguments = \"" + _clientRuns[i].Arguments + "\";");
         //   sb.AppendLine("expectedRun.FoldingID = \"" + _clientRuns[i].FoldingID + "\";");
         //   sb.AppendLine("expectedRun.Team = " + _clientRuns[i].Team + ";");
         //   sb.AppendLine("expectedRun.UserID = \"" + _clientRuns[i].UserID + "\";");
         //   sb.AppendLine("expectedRun.MachineID = " + _clientRuns[i].MachineID + ";");
         //   sb.AppendLine("expectedRun.CompletedUnits = " + _clientRuns[i].CompletedUnits + ";");
         //   sb.AppendLine("expectedRun.FailedUnits = " + _clientRuns[i].FailedUnits + ";");
         //   sb.AppendLine("expectedRun.TotalCompletedUnits = " + _clientRuns[i].TotalCompletedUnits + ";");
         //   sb.AppendLine("expectedRun.Status = SlotStatus." + _clientRuns[i].Status + ";");
         //   sb.AppendLine();
         //   sb.AppendLine("DoClientRunCheck(expectedRun, clientRuns[" + i + "]);");
         //}
         //
         //var form2 = new TextDialog();
         //form2.SetText(sb.ToString());
         //form2.Show();
      }
   }
}
