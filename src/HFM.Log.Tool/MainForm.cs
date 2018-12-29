
//#define DEV

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace HFM.Log.Tool
{
   public partial class MainForm : Form
   {
      private IList<LogLine> _logLines = new List<LogLine>();
      private FahLog _fahLog;

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

            txtLogLineIndex.Text = logLine.Index.ToString();
            txtLogLineType.Text = logLine.LineType.ToString();
            txtLogLineData.Text = logLine.Data != null ? logLine.Data.ToString() : String.Empty;
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

      private const string LegacyLogType = "Legacy";
      private const string FahClientLogType = "FahClient";

      private void btnParse_Click(object sender, EventArgs e)
      {
         if (txtLogPath.Text.Length == 0) return;

         treeView1.Nodes.Clear();

         if (File.Exists(txtLogPath.Text))
         {
#if DEBUG
            var sw = System.Diagnostics.Stopwatch.StartNew();
#endif
            string fahLogType = GetLogFileType();
            switch (fahLogType)
            {
               case LegacyLogType:
                  _fahLog = Legacy.LegacyLog.Read(txtLogPath.Text);
                  break;
               case FahClientLogType:
                  _fahLog = FahClient.FahClientLog.Read(txtLogPath.Text);
                  break;
            }
#if DEBUG
            sw.Stop();
            System.Diagnostics.Debug.WriteLine("FahLog.Read ET: {0}", sw.Elapsed);
#endif
            _logLines = _fahLog.ToList();
            PopulateClientRunsInTree(_fahLog);
            richTextBox1.SetLogLines(_logLines, String.Empty, true);
         }
         else
         {
            MessageBox.Show(this, String.Format(CultureInfo.CurrentCulture,
               "File '{0}' does not exist.", txtLogPath.Text), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
         }
      }

      private string GetLogFileType()
      {
         return LegacyRadioButton.Checked ? LegacyLogType : FahClientLogType;
      }

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

      private void btnGenCode_Click(object sender, EventArgs e)
      {
         var sb = new StringBuilder();
         int i = 0;
         int j = 0;
         int k = 0;
         foreach (var clientRun in _fahLog.ClientRuns.Reverse())
         {
            sb.AppendLine("// Setup ClientRun " + i);
            sb.AppendLine((i == 0 ? "var " : String.Empty) + "expectedRun = new ClientRun(null, " + clientRun.ClientStartIndex + ");");
            foreach (var slotRun in clientRun.SlotRuns.Values)
            {
               sb.AppendLine();
               sb.AppendLine("// Setup SlotRun " + slotRun.FoldingSlot);
               sb.AppendLine((j == 0 ? "var " : String.Empty) + "expectedSlotRun = new SlotRun(expectedRun, " + slotRun.FoldingSlot + ");");
               sb.AppendLine("expectedRun.SlotRuns.Add(" + slotRun.FoldingSlot + ", expectedSlotRun);");
               int unitCount = 0;
               foreach (var unitRun in slotRun.UnitRuns.Reverse())
               {
                  sb.AppendLine();
                  sb.AppendLine("// Setup SlotRun " + slotRun.FoldingSlot + " - UnitRun " + unitCount);
                  sb.AppendLine((k == 0 ? "var " : String.Empty) + "expectedUnitRun = new UnitRun(expectedSlotRun, " + unitRun.QueueIndex + "," + unitRun.StartIndex + "," + unitRun.EndIndex + ");");
                  if (unitRun.Data is Legacy.LegacyUnitRunData)
                  {
                     sb.AppendLine((k == 0 ? "var " : String.Empty) + "expectedUnitRunData = new LegacyUnitRunData();");
                  }
                  else if (unitRun.Data is FahClient.FahClientUnitRunData)
                  {
                     sb.AppendLine((k == 0 ? "var " : String.Empty) + "expectedUnitRunData = new FahClientUnitRunData();");
                  }
                  if (unitRun.Data.UnitStartTimeStamp.HasValue)
                  {
                     var st1 = unitRun.Data.UnitStartTimeStamp.Value;
                     sb.AppendLine("expectedUnitRunData.UnitStartTimeStamp = new TimeSpan(" + st1.Hours + ", " + st1.Minutes + ", " + st1.Seconds + ");");
                  }
                  if (unitRun.Data.CoreVersion == null)
                  {
                     sb.AppendLine("expectedUnitRunData.CoreVersion = null;");
                  }
                  else
                  {
                     sb.AppendLine("expectedUnitRunData.CoreVersion = \"" + unitRun.Data.CoreVersion + "\";");
                  }
                  sb.AppendLine("expectedUnitRunData.FramesObserved = " + unitRun.Data.FramesObserved + ";");
                  sb.AppendLine("expectedUnitRunData.ProjectID = " + unitRun.Data.ProjectID + ";");
                  sb.AppendLine("expectedUnitRunData.ProjectRun = " + unitRun.Data.ProjectRun + ";");
                  sb.AppendLine("expectedUnitRunData.ProjectClone = " + unitRun.Data.ProjectClone + ";");
                  sb.AppendLine("expectedUnitRunData.ProjectGen = " + unitRun.Data.ProjectGen + ";");
                  if (!String.IsNullOrEmpty(unitRun.Data.WorkUnitResult))
                  {
                     sb.AppendLine("expectedUnitRunData.WorkUnitResult = WorkUnitResult." + unitRun.Data.WorkUnitResult + ";");
                  }
                  else
                  {
                     sb.AppendLine("expectedUnitRunData.WorkUnitResult = WorkUnitResult.None;");
                  }
                  sb.AppendLine("expectedUnitRun.Data = expectedUnitRunData;");
                  sb.AppendLine("expectedSlotRun.UnitRuns.Push(expectedUnitRun);");
                  unitCount++;
                  k++;
               }
               sb.AppendLine();
               sb.AppendLine("// Setup SlotRunData " + slotRun.FoldingSlot);
               if (slotRun.Data is Legacy.LegacySlotRunData legacySlotRunData)
               {
                  sb.AppendLine((j == 0 ? "var " : String.Empty) + "expectedSlotRunData = new LegacySlotRunData();");
                  sb.AppendLine("expectedSlotRunData.CompletedUnits = " + legacySlotRunData.CompletedUnits + ";");
                  sb.AppendLine("expectedSlotRunData.FailedUnits = " + legacySlotRunData.FailedUnits + ";");
                  sb.AppendLine("expectedSlotRunData.TotalCompletedUnits = " + GetStringOrNull(legacySlotRunData.TotalCompletedUnits) + ";");
                  sb.AppendLine("expectedSlotRunData.Status = SlotStatus." + legacySlotRunData.Status + ";");
                  sb.AppendLine("expectedSlotRun.Data = expectedSlotRunData;");
               }
               else if (slotRun.Data is FahClient.FahClientSlotRunData fahClientSlotRunData)
               {
                  sb.AppendLine((j == 0 ? "var " : String.Empty) + "expectedSlotRunData = new FahClientSlotRunData();");
                  sb.AppendLine("expectedSlotRunData.CompletedUnits = " + fahClientSlotRunData.CompletedUnits + ";");
                  sb.AppendLine("expectedSlotRunData.FailedUnits = " + fahClientSlotRunData.FailedUnits + ";");
                  sb.AppendLine("expectedSlotRun.Data = expectedSlotRunData;");
               }
               j++;
            }
            sb.AppendLine();
            sb.AppendLine("// Setup ClientRunData " + i);
            if (clientRun.Data is Legacy.LegacyClientRunData legacyClientRunData)
            {
               sb.AppendLine((i == 0 ? "var " : String.Empty) + "expectedRunData = new LegacyClientRunData();");
               var st2 = clientRun.Data.StartTime;
               sb.AppendLine("expectedRunData.StartTime = new DateTime(" + st2.Year + ", " + st2.Month + ", " + st2.Day + ", " + st2.Hour + ", " + st2.Minute + ", " + st2.Second + ", DateTimeKind.Utc);");
               sb.AppendLine("expectedRunData.Arguments = " + GetStringOrNull(legacyClientRunData.Arguments) + ";");
               sb.AppendLine("expectedRunData.ClientVersion = " + GetStringOrNull(legacyClientRunData.ClientVersion) + ";");
               sb.AppendLine("expectedRunData.FoldingID = " + GetStringOrNull(legacyClientRunData.FoldingID) + ";");
               sb.AppendLine("expectedRunData.Team = " + legacyClientRunData.Team + ";");
               sb.AppendLine("expectedRunData.UserID = " + GetStringOrNull(legacyClientRunData.UserID) + ";");
               sb.AppendLine("expectedRunData.MachineID = " + legacyClientRunData.MachineID + ";");
               sb.AppendLine("expectedRun.Data = expectedRunData;");
            }
            else if (clientRun.Data is FahClient.FahClientClientRunData)
            {
               sb.AppendLine((i == 0 ? "var " : String.Empty) + "expectedRunData = new FahClientClientRunData();");
               var st2 = clientRun.Data.StartTime;
               sb.AppendLine("expectedRunData.StartTime = new DateTime(" + st2.Year + ", " + st2.Month + ", " + st2.Day + ", " + st2.Hour + ", " + st2.Minute + ", " + st2.Second + ", DateTimeKind.Utc);");
               sb.AppendLine("expectedRun.Data = expectedRunData;");
            }
            sb.AppendLine();
            sb.AppendLine("var actualRun = fahLog.ClientRuns.ElementAt(" + (_fahLog.ClientRuns.Count - 1 - i) + ");");
            sb.AppendLine("AssertClientRun.AreEqual(expectedRun, actualRun, true);");
            sb.AppendLine();
            sb.AppendLine(String.Format("Assert.AreEqual({0}, actualRun.Count(x => x.Data is LogLineDataParserError));", clientRun.Count(x => x.Data is LogLineDataParserError)));
            i++;
         }

         var form2 = new TextDialog();
         form2.SetText(sb.ToString());
         form2.Show();
      }

      private static string GetStringOrNull(object value)
      {
         return value == null ? "null" : "\"" + value + "\"";
      }
   }
}
