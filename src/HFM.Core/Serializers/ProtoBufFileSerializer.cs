/*
 * HFM.NET - ProtoBuf File Serializer
 * Copyright (C) 2009-2012 Ryan Harlamert (harlam357)
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
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA.
 */

using System.IO;

using HFM.Core.Plugins;

namespace HFM.Core.Serializers
{
   public class ProtoBufFileSerializer<T> : IFileSerializer<T> where T : class, new()
   {
      public virtual string FileExtension
      {
         get { return "dat"; }
      }

      public virtual string FileTypeFilter
      {
         get { return "HFM Data Files|*.dat"; }
      }

      public T Deserialize(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            return ProtoBuf.Serializer.Deserialize<T>(fileStream);
         }
      }

      public void Serialize(string fileName, T value)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
         {
            ProtoBuf.Serializer.Serialize(fileStream, value);
         }
      }
   }
}
