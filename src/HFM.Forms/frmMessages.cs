/*
 * HFM.NET - Messages Form Class
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

using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using HFM.Core;
using HFM.Core.Logging;
using HFM.Forms.Controls;

namespace HFM.Forms
{
   public interface IMessagesView
   {
      void AddMessage(string message);

      void ScrollToEnd();

      void SetManualStartPosition();

      void SetLocation(int x, int y);

      void SetSize(int width, int height);

      void Show();

      void Close();

      bool Visible { get; set; }
   }

   public partial class frmMessages : FormWrapper, IMessagesView
   {
      private readonly IPreferenceSet _prefs;
      private volatile List<string> _lines = new List<string>(500);

      // concrete HFM.Core.Logger instance
      private readonly Logger _logger;

      #region Constructor

      public frmMessages(IPreferenceSet prefs, Logger logger)
      {
         _prefs = prefs;
         _logger = logger;
      
         InitializeComponent();

         _logger.TextMessage += (sender, e) => AddMessage(e.Message);
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
      
      public void SetManualStartPosition()
      {
         StartPosition = FormStartPosition.Manual;
      }
      
      public void SetLocation(int x, int y)
      {
         Location = new Point(x, y);
      }
      
      public void SetSize(int width, int height)
      {
         Size = new Size(width, height);
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

      private void txtMessages_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.F7) //Close on F7 - Issue 74
         {
            Close();
         }
      }

      protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
      {
         // Save state data
         if (WindowState == FormWindowState.Normal)
         {
            _prefs.SetPreference(Preference.MessagesFormLocation, Location);
            _prefs.SetPreference(Preference.MessagesFormSize, Size);
            _prefs.Save();
         }
      
         Hide();
         e.Cancel = true;
         base.OnClosing(e);
      } 

      #endregion
   }
}
