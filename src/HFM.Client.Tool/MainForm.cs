/*
 * HFM.NET - Client.Tool Main Form
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
using System.Globalization;
using System.IO;
using System.Windows.Forms;

using HFM.Client.DataTypes;
using HFM.Framework;

namespace HFM.Client.Tool
{
   public partial class MainForm : Form
   {
      private readonly FahClient _fahClient;

      private int _totalBytesSent;
      private int _totalBytesReceived;

      public MainForm()
      {
         _fahClient = new FahClient();
         _fahClient.MessageUpdated += FahClientMessageUpdated;
         _fahClient.ConnectedChanged += FahClientConnectedChanged;
         _fahClient.DataLengthSent += FahClientDataLengthSent;
         _fahClient.DataLengthReceived += FahClientDataLengthReceived;
         _fahClient.StatusMessage += FahClientStatusMessage;
         _fahClient.DebugReceiveBuffer = true;

         InitializeComponent();

         base.Text = String.Format("HFM Client Tool v{0}", PlatformOps.ApplicationVersionWithRevision);
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
               _fahClient.SendCommand("slot-options " + slot.Id + " client-type client-subtype machine-id max-packet-size core-priority next-unit-percentage max-units checkpoint pause-on-start gpu-vendor-id gpu-device-id");
               _fahClient.SendCommand("simulation-info " + slot.Id);
            }
         }
      }

      private void AppendToMessageDisplayTextBox(string text)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<string>(AppendToMessageDisplayTextBox), text);
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
            Invoke(new Action<string>(UpdateStatusLabel), text);
            return;
         }

         StatusLabel.Text = text;
         StatusMessageListBox.Items.Add(text);
         StatusMessageListBox.SelectedIndex = StatusMessageListBox.Items.Count - 1;
      }

      private void ConnectButtonClick(object sender, EventArgs e)
      {
         if (File.Exists("buffer.txt"))
         {
            try
            {
               File.Delete("buffer.txt");
            }
            // ReSharper disable EmptyGeneralCatchClause
            catch (Exception)
            { }
            // ReSharper restore EmptyGeneralCatchClause
         }

         try
         {
            _fahClient.Connect(HostAddressTextBox.Text, Int32.Parse(PortTextBox.Text), PasswordTextBox.Text);
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
            Invoke(new Action<bool>(SetConnectionButtons), connected);
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
         UpdateDataSentValueLabel(String.Format(CultureInfo.CurrentCulture, 
            "{0:0.0} KBs", _totalBytesSent / 1024.0));
      }

      private void UpdateDataSentValueLabel(string text)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<string>(UpdateDataSentValueLabel), text);
            return;
         }

         DataSentValueLabel.Text = text;
      }

      private void FahClientDataLengthReceived(object sender, DataLengthEventArgs e)
      {
         unchecked
         {
            _totalBytesReceived += e.DataLength;
         }
         UpdateDataReceivedValueLabel(String.Format(CultureInfo.CurrentCulture,
            "{0:0.0} KBs", _totalBytesReceived / 1024.0));
      }

      private void UpdateDataReceivedValueLabel(string text)
      {
         if (InvokeRequired)
         {
            Invoke(new Action<string>(UpdateDataReceivedValueLabel), text);
            return;
         }

         DataReceivedValueLabel.Text = text;
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
