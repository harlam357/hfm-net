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
using System.IO;
using System.Windows.Forms;

using HFM.Client.DataTypes;

namespace HFM.Client.Tool
{
   public partial class MainForm : Form
   {
      private readonly FahClient _fahClient;

      public MainForm()
      {
         _fahClient = new FahClient();
         _fahClient.MessageUpdated += FahClientMessageUpdated;
         _fahClient.StatusMessage += FahClientStatusMessage;

         InitializeComponent();
      }

      private void FahClientMessageUpdated(object sender, MessageUpdatedEventArgs e)
      {
         JsonMessage jsonMessage = _fahClient.GetJsonMessage(e.Key);
         AppendToMessageDisplayTextBox(String.Empty);
         AppendToMessageDisplayTextBox(jsonMessage.ToString());
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
      }

      private void ConnectButtonClick(object sender, EventArgs e)
      {
#if DEBUG
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
#endif
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

         _fahClient.SendCommand(CommandTextBox.Text);
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
