/*
 * HFM.NET - Xml File Serializer
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
using System.Runtime.Serialization;
using System.Xml;

using HFM.Core.Plugins;

namespace HFM.Core.Serializers
{
   public class XmlFileSerializer<T> : IFileSerializer<T> where T : class, new()
   {
      public string FileExtension
      {
         get { return "xml"; }
      }

      public string FileTypeFilter
      {
         get { return "Xml Files|*.xml"; }
      }

      public T Deserialize(string fileName)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read))
         {
            var serializer = new DataContractSerializer(typeof(T));
            return (T)serializer.ReadObject(fileStream);
         }
      }

      [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times")]
      public void Serialize(string fileName, T value)
      {
         using (var fileStream = new FileStream(fileName, FileMode.Create, FileAccess.Write))
         using (var xmlWriter = XmlWriter.Create(fileStream, new XmlWriterSettings { Indent = true }))
         {
            var serializer = new DataContractSerializer(typeof(T));
            serializer.WriteObject(xmlWriter, value);
         }
      }
   }
}
