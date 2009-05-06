/*
 * HFM.NET - Instrumentation Class
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
using System.Diagnostics;

namespace HFM.Instrumentation
{
   public class TextWriterTraceListenerWithDateTime : TextWriterTraceListener
   {
      public TextWriterTraceListenerWithDateTime(string fileName) : base(fileName)
      {
      
      }

      public override void WriteLine(string message)
      {
         DateTime dateTime = DateTime.Now;
      
         string traceLine = String.Format("[{0}-{1}] {2}", dateTime.ToShortDateString(), dateTime.ToLongTimeString(), message);

         base.WriteLine(traceLine);
         OnTextMessage(new TextMessageEventArgs(traceLine));
      }

      public delegate void TextMessageEventHandler(TextMessageEventArgs e);

      public event TextMessageEventHandler TextMessage;

      private void OnTextMessage(TextMessageEventArgs e)
      {
         if (TextMessage != null)
         {
            TextMessage(e);
         }
      }
   }
}
