/*
 * HFM.NET - Log Line Class
 * Copyright (C) 2009-2016 Ryan Harlamert (harlam357)
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

namespace HFM.Log
{
   public class LogLine
   {
      public LogLineType LineType { get; set; }

      public int Index { get; set; }

      public string Raw { get; set; }

      public LogLineDataParser DataParser { get; set; }

      private object _data;

      public object Data
      {
         get
         {
            if (_data == null && DataParser != null)
            {
               _data = DataParser(this);
               if (_data == null)
               {
                  LineType = LogLineType.Error;
               }
            }
            return _data;
         }
         set { _data = value; }
      }

      public int FoldingSlot { get; set; }

      public int QueueIndex { get; set; }

      public TimeSpan? TimeStamp { get; set; }

      public override string ToString()
      {
         return Raw;
      }
   }
}
