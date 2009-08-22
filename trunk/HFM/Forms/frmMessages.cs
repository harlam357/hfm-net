/*
 * HFM.NET - Messages Form Class
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

using System.Collections.Generic;

namespace HFM.Forms
{
   public partial class frmMessages : Classes.FormWrapper
   {
      private volatile List<string> _lines = new List<string>(500);

      #region Constructor
      public frmMessages()
      {
         InitializeComponent();
      } 
      #endregion

      #region Implementation
      public void AddMessage(string message)
      {
         if (_lines.Count > 500)
         {
            _lines.RemoveRange(0, 100);
         }

         _lines.Add(message);

         UpdateMessages(_lines.ToArray());
      }

      public void ScrollToEnd()
      {
         txtMessages.SelectionStart = txtMessages.Text.Length;
         txtMessages.ScrollToCaret();
      }

      private delegate void UpdateMessagesDelegate(string[] lines);

      private void UpdateMessages(string[] lines)
      {
         if (InvokeRequired)
         {
            // BIG BUG FIX HERE!!! Using Invoke instead of BeginInvoke was casing 
            // deadlock when trying to call this delegate from multiple threads
            txtMessages.BeginInvoke(new UpdateMessagesDelegate(UpdateMessages), new object[] { lines });
            return;
         }
         
         txtMessages.Lines = lines;
         ScrollToEnd();
      }

      private void txtMessages_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
      {
         if (e.KeyCode == System.Windows.Forms.Keys.F7) //Close on F7 - Issue 74
         {
            Close();
         }
      }

      protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
      {
         Hide();
         e.Cancel = true;
         base.OnClosing(e);
      } 
      #endregion
   }
}
