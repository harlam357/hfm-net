/*
 * HFM.NET - JSON Serializer
 * Copyright (C) 2009-2015 Ryan Harlamert (harlam357)
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
using System.IO;

using Newtonsoft.Json.Linq;

using HFM.Core.DataTypes;
using HFM.Core.Plugins;

namespace HFM.Proteins
{
   public class JsonSerializer : IFileSerializer<List<Protein>>
   {
      public string FileExtension
      {
         get { return String.Empty; }
      }

      public string FileTypeFilter
      {
         get { return String.Empty; }
      }

      public List<Protein> Deserialize(string fileName)
      {
         const double secondsToDays = 86400.0;

         var text = File.ReadAllText(fileName);

         var proteins = new List<Protein>();
         foreach (var token in JArray.Parse(text))
         {
            if (!token.HasValues)
            {
               continue;
            }

            var p = new Protein();
            p.ProjectNumber = (int)token["id"];
            p.ServerIP = (string)token["ws"];
            p.WorkUnitName = (string)token["title"];
            p.NumberOfAtoms = (int)token["atoms"];
            p.PreferredDays = ((int)token["timeout"]) / secondsToDays;
            p.MaximumDays = ((int)token["deadline"]) / secondsToDays;
            p.Credit = Convert.ToDouble(token["credit"].ToString());
            //p.Frames = 100;
            p.Core = (string)token["type"];
            p.Description = @"http://fah-web.stanford.edu/cgi-bin/fahproject.overusingIPswillbebanned?p=" + p.ProjectNumber;
            p.Contact = (string)token["contact"];
            p.KFactor = Convert.ToDouble(token["bonus"].ToString());
            proteins.Add(p);
         }

         return proteins;
      }

      public void Serialize(string fileName, List<Protein> value)
      {
         throw new NotSupportedException("JSON serialization is not supported.");
      }
   }
}
