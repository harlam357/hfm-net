/*
 * HFM.NET - JSON Serializer
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

namespace HFM.Proteins
{
   /// <summary>
   /// Represents a serializer capable of deserializing a collection of <see cref="Protein"/> objects from the Folding@Home project summary json.
   /// This class does not support serialization through the <see cref="IProteinCollectionSerializer"/> interface. The serialize methods will throw <see cref="NotSupportedException"/>.
   /// </summary>
   public class ProjectSummaryJsonDeserializer : IProteinCollectionSerializer
   {
      /// <summary>
      /// Deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
      /// </summary>
      public ICollection<Protein> Deserialize(Stream stream)
      {
         string json;
         using (var reader = new StreamReader(stream))
         {
            json = reader.ReadToEnd();
         }
         return DeserializeInternal(json);
      }

      /// <summary>
      /// Asynchronously deserializes a collection of <see cref="Protein"/> objects from a <see cref="Stream"/>.
      /// </summary>
      public async Task<ICollection<Protein>> DeserializeAsync(Stream stream)
      {
         string json;
         using (var reader = new StreamReader(stream))
         {
            json = await reader.ReadToEndAsync().ConfigureAwait(false);
         }
         return DeserializeInternal(json);
      }

      private static ICollection<Protein> DeserializeInternal(string json)
      {
         const double secondsToDays = 86400.0;

         var collection = new List<Protein>();
         if (json.Length > 0)
         {
            foreach (var token in JArray.Parse(json))
            {
               if (!token.HasValues)
               {
                  continue;
               }

               var p = new Protein();
               p.ProjectNumber = GetTokenValue<int>(token, "id");
               p.ServerIP = GetTokenValue<string>(token, "ws");
               p.WorkUnitName = GetTokenValue<string>(token, "title");
               p.NumberOfAtoms = GetTokenValue<int>(token, "atoms");
               p.PreferredDays = Math.Round(GetTokenValue<int>(token, "timeout") / secondsToDays, 3, MidpointRounding.AwayFromZero);
               p.MaximumDays = Math.Round(GetTokenValue<int>(token, "deadline") / secondsToDays, 3, MidpointRounding.AwayFromZero);
               p.Credit = GetTokenValue<double>(token, "credit");
               p.Frames = 100;
               p.Core = GetTokenValue<string>(token, "type");
               p.Description = @"https://apps.foldingathome.org/project.py?p=" + p.ProjectNumber;
               p.Contact = GetTokenValue<string>(token, "contact");
               p.KFactor = GetTokenValue<double>(token, "bonus");
               collection.Add(p);
            }
         }
         return collection;
      }

      private static T GetTokenValue<T>(JToken token, string path)
      {
         for (var selected = token.SelectToken(path); selected != null; )
         {
            return selected.Value<T>();
         }
         return default(T);
      }

      void IProteinCollectionSerializer.Serialize(Stream stream, ICollection<Protein> collection)
      {
         throw new NotSupportedException("Serialization is not supported.");
      }

      Task IProteinCollectionSerializer.SerializeAsync(Stream stream, ICollection<Protein> collection)
      {
         throw new NotSupportedException("Serialization is not supported.");
      }
   }
}
