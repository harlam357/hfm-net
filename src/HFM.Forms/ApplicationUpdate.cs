/*
 * harlam357.Net - Application Update Class
 * Copyright (C) 2010 Ryan Harlamert (harlam357)
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1 of the License, or (at your option) any later version.
 *
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA
 */
 
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml.Serialization;

namespace HFM.Forms
{
   [Serializable]
   public class ApplicationUpdate
   {
      public string Version { get; set; }
      public DateTime UpdateDate { get; set; }
      public List<ApplicationUpdateFile> UpdateFiles { get; set; }
      
      public ApplicationUpdate()
      {
         Version = String.Empty;
         UpdateDate = DateTime.MinValue;
         UpdateFiles = new List<ApplicationUpdateFile>();
      }
   }

   [Serializable]
   public class ApplicationUpdateFile
   {
      // ReSharper disable InconsistentNaming
      public string Name { get; set; }
      public string Description { get; set; }
      public string HttpAddress { get; set; }
      public int Size { get; set; }
      public string MD5 { get; set; }
      public string SHA1 { get; set; }
      public int UpdateType { get; set; }
      // ReSharper restore InconsistentNaming

      public ApplicationUpdateFile()
      {
         HttpAddress = String.Empty;
         MD5 = String.Empty;
         SHA1 = String.Empty;
      }
   }
   
   public static class ApplicationUpdateSerializer
   {
      public static void SerializeToXml(ApplicationUpdate update, string filePath)
      {
         using (TextWriter stream = new StreamWriter(filePath, false, Encoding.UTF8))
         {
            var s = new XmlSerializer(typeof(ApplicationUpdate));
            s.Serialize(stream, update);
         }
      }

      public static ApplicationUpdate DeserializeFromXml(string filePath)
      {
         using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
         {
            var s = new XmlSerializer(typeof(ApplicationUpdate));
            return (ApplicationUpdate)s.Deserialize(stream);
         }
      }
   }
}