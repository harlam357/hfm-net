/*
 * HFM.NET - Client.Tool Main Form
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using HFM.Client.DataTypes;

namespace HFM.Client.Tool
{
   public partial class MainForm : Form
   {
      private readonly FahClient _fahClient;

      private int _totalBytesSent;
      private int _totalBytesReceived;
      private string _debugBufferFileName;

      public MainForm()
      {
         _fahClient = new FahClient();
         _fahClient.MessageUpdated += FahClientMessageUpdated;
         _fahClient.ConnectedChanged += FahClientConnectedChanged;
         _fahClient.DataLengthSent += FahClientDataLengthSent;
         _fahClient.DataLengthReceived += FahClientDataLengthReceived;
         _fahClient.StatusMessage += FahClientStatusMessage;

         InitializeComponent();

         base.Text = String.Format("HFM Client Tool v{0}", Core.Application.VersionWithRevision);
      }

      private void FahClientMessageUpdated(object sender, MessageUpdatedEventArgs e)
      {
         JsonMessage jsonMessage = _fahClient.GetJsonMessage(e.Key);
         AppendToMessageDisplayTextBox(String.Empty);
         AppendToMessageDisplayTextBox(jsonMessage.ToString());
         
         if (e.DataType == typeof(SlotCollection))
         {
            var slotCollection = _fahClient.GetMessage<SlotCollection>();
            foreach (var slot in slotCollection)
            {
               _fahClient.SendCommand("slot-options " + slot.Id + " client-type client-subtype cpu-usage machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-index gpu-usage");
               _fahClient.SendCommand("simulation-info " + slot.Id);
            }
         }
      }

      private void AppendToMessageDisplayTextBox(string text)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string>(AppendToMessageDisplayTextBox), text);
            return;
         }

         MessageDisplayTextBox.AppendText(SetEnvironmentNewLineCharacters(text));
      }

      private static string SetEnvironmentNewLineCharacters(string text)
      {
         text = text.Replace("\n", Environment.NewLine);
         return text.Replace("\\n", Environment.NewLine);
      }

      private void FahClientStatusMessage(object sender, StatusMessageEventArgs e)
      {
         UpdateStatusLabel(e.Status);
      }

      private void UpdateStatusLabel(string text)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<string>(UpdateStatusLabel), text);
            return;
         }

         StatusLabel.Text = text;
         StatusMessageListBox.Items.Add(text);
         StatusMessageListBox.SelectedIndex = StatusMessageListBox.Items.Count - 1;
      }

      private void LogMessagesCheckBoxCheckedChanged(object sender, EventArgs e)
      {
         if (LogMessagesCheckBox.Checked)
         {
            if (String.IsNullOrEmpty(_debugBufferFileName))
            {
               using (var dlg = new OpenFileDialog { CheckFileExists = false })
               {
                  if (dlg.ShowDialog(this).Equals(DialogResult.OK))
                  {
                     _debugBufferFileName = dlg.FileName;
                  }
                  else
                  {
                     LogMessagesCheckBox.Checked = false;
                     return;
                  }
               }
            }

            if (File.Exists(_debugBufferFileName))
            {
               string message = String.Format(CultureInfo.CurrentCulture, 
                  "Do you want to delete the existing {0} file?", _debugBufferFileName);
               if (MessageBox.Show(this, message, Text, MessageBoxButtons.YesNo).Equals(DialogResult.Yes))
               {
                  try
                  {
                     File.Delete(_debugBufferFileName);
                  }
                  catch (Exception)
                  {
                     MessageBox.Show(String.Format(CultureInfo.CurrentCulture,
                        "Could not delete {0}.  Make sure the file is not already in use.", _debugBufferFileName));
                     LogMessagesCheckBox.Checked = false;
                     return;
                  }
               }
            }

            _fahClient.DebugBufferFileName = _debugBufferFileName;
            _fahClient.DebugReceiveBuffer = true;
         }
         else
         {
            _fahClient.DebugReceiveBuffer = false;
         }
      }

      private void ConnectButtonClick(object sender, EventArgs e)
      {
         try
         {
            _fahClient.Connect(HostAddressTextBox.Text, Int32.Parse(PortTextBox.Text), PasswordTextBox.Text);
            _totalBytesSent = 0;
            UpdateDataSentValueLabel(_totalBytesSent);
            _totalBytesReceived = 0;
            UpdateDataReceivedValueLabel(_totalBytesReceived);
         }
         catch (InvalidOperationException ex)
         {
            MessageBox.Show(ex.Message);
         }
         catch (TimeoutException ex)
         {
            MessageBox.Show(ex.Message);
         }
      }

      private void CloseButtonClick(object sender, EventArgs e)
      {
         _fahClient.Close();
      }

      private void SendCommandButtonClick(object sender, EventArgs e)
      {
         if (!_fahClient.Connected)
         {
            MessageBox.Show("Not connected.");
            return;
         }

         string command = CommandTextBox.Text;
         if (command == "test-commands")
         {
            _fahClient.SendCommand("info");
            _fahClient.SendCommand("options -a");   
            _fahClient.SendCommand("queue-info");
            _fahClient.SendCommand("slot-info");
            _fahClient.SendCommand("log-updates restart");
         }
         else
         {
            _fahClient.SendCommand(CommandTextBox.Text);
         }
      }

      private void FahClientConnectedChanged(object sender, ConnectedChangedEventArgs e)
      {
         SetConnectionButtons(e.Connected);
      }

      private void SetConnectionButtons(bool connected)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<bool>(SetConnectionButtons), connected);
            return;
         }

         ConnectButton.Enabled = !connected;
         CloseButton.Enabled = connected;
      }

      private void ClearMessagesButtonClick(object sender, EventArgs e)
      {
         StatusLabel.Text = String.Empty;
         StatusMessageListBox.Items.Clear();
         MessageDisplayTextBox.Clear();
      }

      private void FahClientDataLengthSent(object sender, DataLengthEventArgs e)
      {
         unchecked
         {
            _totalBytesSent += e.DataLength;
         }
         UpdateDataSentValueLabel(_totalBytesSent);
      }

      private void UpdateDataSentValueLabel(int value)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<int>(UpdateDataSentValueLabel), value);
            return;
         }

         DataSentValueLabel.Text = String.Format(CultureInfo.CurrentCulture,
            "{0:0.0} KBs", value / 1024.0);
      }

      private void FahClientDataLengthReceived(object sender, DataLengthEventArgs e)
      {
         unchecked
         {
            _totalBytesReceived += e.DataLength;
         }
         UpdateDataReceivedValueLabel(_totalBytesReceived);
      }

      private void UpdateDataReceivedValueLabel(int value)
      {
         if (InvokeRequired)
         {
            BeginInvoke(new Action<int>(UpdateDataReceivedValueLabel), value);
            return;
         }

         DataReceivedValueLabel.Text = String.Format(CultureInfo.CurrentCulture,
            "{0:0.0} KBs", value / 1024.0);
      }

      #region TextBox KeyPress Event Handler (to enforce digits only)

      private void DigitsOnlyKeyPress(object sender, KeyPressEventArgs e)
      {
         //Debug.WriteLine(String.Format("Keystroke: {0}", (int)e.KeyChar));

         // only allow digits & special keystrokes
         if (char.IsDigit(e.KeyChar) == false &&
               e.KeyChar != 8 &&       // backspace 
               e.KeyChar != 26 &&      // Ctrl+Z
               e.KeyChar != 24 &&      // Ctrl+X
               e.KeyChar != 3 &&       // Ctrl+C
               e.KeyChar != 22 &&      // Ctrl+V
               e.KeyChar != 25)        // Ctrl+Y
         {
            e.Handled = true;
         }
      }

      #endregion
   }
}
