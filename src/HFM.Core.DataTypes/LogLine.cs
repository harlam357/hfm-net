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
using System.Diagnostics;
using System.Runtime.Serialization;

namespace HFM.Core.DataTypes
{
   [DataContract(Namespace = "")]
   public class LogLine
   {
      [DataMember(Order = 1)]
      public LogLineType LineType { get; set; }

      [DataMember(Order = 2)]
      public int LineIndex { get; set; }

      [DataMember(Order = 3)]
      public string LineRaw { get; set; }

      private object _lineData;

      // cannot serialize object type with protobuf-net
      public object LineData
      {
         get
         {
            if (_lineData == null && _parser != null)
            {
               _lineData = _parser(this);
               if (_lineData == null)
               {
                  LineType = LogLineType.Error;
               }
            }
            return _lineData;
         }
         set { _lineData = value; }
      }

      public int FoldingSlot { get; set; }

      public int QueueIndex { get; set; }

      public override string ToString()
      {
         return LineRaw;
      }

      private Func<LogLine, object> _parser;

      public void SetParser(Func<LogLine, object> parser)
      {
         Debug.Assert(parser != null);
         _parser = parser;
      }
   }
}
