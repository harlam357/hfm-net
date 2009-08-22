/*
 * HFM.NET - RichTextBox Wrapper Class
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
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using HFM.Instances;

namespace HFM.Classes
{
   public partial class RichTextBoxWrapper : RichTextBox
   {
      IList<LogLine> _LogLines = null;
   
      public RichTextBoxWrapper()
      {
         InitializeComponent();
      }
      
      public void SetLogLines(IList<LogLine> lines)
      {
         _LogLines = lines;
      
         List<string> logLines = new List<string>();
         foreach (LogLine line in lines)
         {
            logLines.Add(line.LineRaw);
         }
         
         Lines = logLines.ToArray();
      }
      
      public void SetNoLogLines()
      {
         _LogLines = null;
      
         Text = "No Log Available";
         RemoveHighlight();
      }

      public void HighlightLines()
      {
         if (_LogLines == null) return;
         
         SuspendLayout();
         
         RemoveHighlight();
         
         ForeColor = Color.SlateGray;

         for (int i = 0; i < _LogLines.Count; i++)
         {
            LogLine line = _LogLines[i];
            if (line.LineType.Equals(LogLineType.WorkUnitFrame))
            {
               DoLineHighlight(i, Color.Green);
            }
            else if (line.LineType.Equals(LogLineType.ClientShutdown) ||
                     line.LineType.Equals(LogLineType.ClientEuePauseState) ||
                     line.LineType.Equals(LogLineType.WorkUnitCoreShutdown))
            {
               DoLineHighlight(i, Color.DarkRed);
            }
            else if (line.LineType.Equals(LogLineType.Unknown) == false)
            {
               DoLineHighlight(i, Color.Blue);
            }
         }
         
         ResumeLayout(false);
      }
      
      public void RemoveHighlight()
      {
         if (ForeColor.Equals(Color.Black) == false)
         {
            ForeColor = Color.Black;

            SelectAll();
            SelectionColor = ForeColor;
         }
      }

      private void DoLineHighlight(int lineIndex, Color color)
      {
         int firstChar = GetFirstCharIndexFromLine(lineIndex);
         int length = Lines[lineIndex].Length;
         Select(firstChar, length);

         SelectionColor = color;
         //SelectionBackColor = color;
      }

      #region Native Scroll Messages (don't call under Mono)
      [DllImport("user32.dll", CharSet = CharSet.Auto)]
      private static extern IntPtr SendMessage(
        IntPtr hWnd,
        uint Msg,
        IntPtr wParam,
        IntPtr lParam);

      private const int WM_VSCROLL = 277;
      private const int SB_LINEUP = 0;
      private const int SB_LINEDOWN = 1;
      private const int SB_TOP = 6;
      private const int SB_BOTTOM = 7;

      public void ScrollToBottom()
      {
         SelectionStart = TextLength;
      
         if (Helpers.PlatformOps.IsRunningOnMono())
         {
            ScrollToCaret();
         }
         else
         {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_BOTTOM), new IntPtr(0));
         }
      }

      public void ScrollToTop()
      {
         if (Helpers.PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }
         else
         {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_TOP), new IntPtr(0));
         }
      }

      public void ScrollLineDown()
      {
         if (Helpers.PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }
         else
         {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_LINEDOWN), new IntPtr(0));
         }
      }

      public void ScrollLineUp()
      {
         if (Helpers.PlatformOps.IsRunningOnMono())
         {
            throw new NotImplementedException("This function is not implemented when running under the Mono Runtime.");
         }
         else
         {
            SendMessage(Handle, WM_VSCROLL, new IntPtr(SB_LINEUP), new IntPtr(0));
         }
      }
      #endregion
   }
}
