/*
 * HFM.NET - Protein Dictionary
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
using System.ComponentModel;
using System.Linq;

namespace HFM.Proteins
{
   public class ProteinDictionary : Dictionary<int, Protein>
   {
      public IReadOnlyCollection<ProteinDictionaryChange> Changes { get; internal set; }

      public static ProteinDictionary Create(IEnumerable<Protein> proteins)
      {
         if (proteins == null) throw new ArgumentNullException("proteins");

         var dictionary = new ProteinDictionary();
         var dictionaryChanges = new List<ProteinDictionaryChange>();
         foreach (Protein protein in proteins.Where(x => x.IsValid))
         {
            dictionaryChanges.Add(new ProteinDictionaryChange(protein.ProjectNumber, ProteinDictionaryChangeResult.Added));
            dictionary.Add(protein.ProjectNumber, protein);
         }
         dictionary.Changes = dictionaryChanges;
         return dictionary;
      }

      public static ProteinDictionary CreateFromExisting(IReadOnlyDictionary<int, Protein> existingDictionary, IEnumerable<Protein> proteins)
      {
         if (existingDictionary == null) throw new ArgumentNullException("existingDictionary");
         if (proteins == null) throw new ArgumentNullException("proteins");

         var dictionary = new ProteinDictionary();
         var dictionaryChanges = new List<ProteinDictionaryChange>();
         foreach (Protein protein in proteins.Where(x => x.IsValid))
         {
            if (existingDictionary.ContainsKey(protein.ProjectNumber))
            {
               var propertyChanges = GetChangedProperties(existingDictionary[protein.ProjectNumber], protein);
               dictionaryChanges.Add(propertyChanges.Count == 0
                  ? new ProteinDictionaryChange(protein.ProjectNumber, ProteinDictionaryChangeResult.NoChange)
                  : new ProteinDictionaryChange(protein.ProjectNumber, ProteinDictionaryChangeResult.Changed, propertyChanges));
            }
            else
            {
               dictionaryChanges.Add(new ProteinDictionaryChange(protein.ProjectNumber, ProteinDictionaryChangeResult.Added));
            }
            dictionary[protein.ProjectNumber] = protein;
         }
         dictionary.Changes = dictionaryChanges;
         return dictionary;
      }

      private static List<ProteinPropertyChange> GetChangedProperties(Protein previous, Protein current)
      {
         var changes = new List<ProteinPropertyChange>();
         var properties = TypeDescriptor.GetProperties(previous);
         foreach (PropertyDescriptor prop in properties)
         {
            object value1 = prop.GetValue(previous);
            object value2 = prop.GetValue(current);
            if (value1 == null || value2 == null)
            {
               continue;
            }
            if (!value1.Equals(value2))
            {
               changes.Add(new ProteinPropertyChange(prop.Name, value1.ToString(), value2.ToString()));
            }
         }
         return changes;
      }
   }
}
