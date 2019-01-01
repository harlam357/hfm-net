/*
 * HFM.NET
 * Copyright (C) 2009-2017 Ryan Harlamert (harlam357)
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

using Castle.Core.Logging;

using HFM.Core.Logging;
using HFM.Forms.Controls;
using HFM.Preferences;

namespace HFM.Forms
{
   public interface IMessagesView
   {
      void ScrollToEnd();

      void SetManualStartPosition();

      void SetLocation(int x, int y);

      void SetSize(int width, int height);

      void Show();

      void Close();

      bool Visible { get; set; }
   }

   public partial class MessagesForm : FormWrapper, IMessagesView
   {
      private const int MaxLines = 512;

      private readonly IPreferenceSet _prefs;
      private readonly List<string> _lines = new List<string>(MaxLines);

      #region Constructor

      public MessagesForm(IPreferenceSet prefs)
      {
         _prefs = prefs;

         InitializeComponent();
      }

      #endregion

      #region Implementation

      public void AttachLogger(ILogger logger)
      {
         ((Logger)logger).TextMessage += (sender, e) => AddMessage(e.Messages);
      }

      private void AddMessage(ICollection<string> messages)
      {
         if ((_lines.Count + messages.Count) > MaxLines)
         {
            _lines.RemoveRange(0, MaxLines / 4);
         }
         _lines.AddRange(messages);

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
            txtMessages.BeginInvoke(new UpdateMessagesDelegate(UpdateMessages), new object[] { lines });
            return;
         }

         txtMessages.Lines = lines;
         ScrollToEnd();
      }

      private void txtMessages_KeyDown(object sender, KeyEventArgs e)
      {
         if (e.KeyCode == Keys.F7)
         {
            // Close on F7 - Issue 74
            Close();
         }
      }

      protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
      {
         // Save state data
         if (WindowState == FormWindowState.Normal)
         {
            _prefs.Set(Preference.MessagesFormLocation, Location);
            _prefs.Set(Preference.MessagesFormSize, Size);
            _prefs.Save();
         }

         Hide();
         e.Cancel = true;
         base.OnClosing(e);
      }

      #endregion
   }
}
