/*
 * HFM.NET - Log Line Class
 * Copyright (C) 2009-2010 Ryan Harlamert (harlam357)
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
 
 using ProtoBuf;

namespace HFM.Framework.DataTypes
{
   public interface ILogLine
   {
      LogLineType LineType { get; set; }

      int LineIndex { get; }

      string LineRaw { get; }

      object LineData { get; }
   }
   
   [ProtoContract]
   public class LogLine : ILogLine
   {
      [ProtoMember(1)]
      public LogLineType LineType { get; set; }

      [ProtoMember(2)]
      public int LineIndex { get; set; }

      [ProtoMember(3)]
      public string LineRaw { get; set; }

      // cannot serialize object type with protobuf-net
      public object LineData { get; set; }

      public override string ToString()
      {
         return LineRaw;
      }
   }
}