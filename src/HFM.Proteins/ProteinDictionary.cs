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
   /// <summary>
   /// Represents a collection of <see cref="Protein"/> values keyed by project number.
   /// </summary>
   public class ProteinDictionary : Dictionary<int, Protein>
   {
      /// <summary>
      /// Initializes a new instance of the <see cref="ProteinDictionary"/> class.
      /// </summary>
      public ProteinDictionary()
      {
         
      }

      /// <summary>
      /// Initializes a new instance of the <see cref="ProteinDictionary"/> class that contains elements copied from the specified dictionary.
      /// </summary>
      public ProteinDictionary(IDictionary<int, Protein> dictionary)
         : base(dictionary)
      {
         
      }

      /// <summary>
      /// Gets a collection of proteins that changed.
      /// </summary>
      public IReadOnlyCollection<ProteinDictionaryChange> Changes { get; private set; }

      /// <summary>
      /// Creates a new <see cref="ProteinDictionary"/> from the collection of <see cref="Protein"/> objects.
      /// </summary>
      /// <param name="proteins">The collection of <see cref="Protein"/> objects that will populate the new dictionary.</param>
      public static ProteinDictionary Create(IEnumerable<Protein> proteins)
      {
         if (proteins == null) throw new ArgumentNullException(nameof(proteins));

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

      /// <summary>
      /// Creates a new <see cref="ProteinDictionary"/> using elements copied from the existing dictionary and changes applied from the collection of <see cref="Protein"/> objects.
      /// </summary>
      /// <param name="existingDictionary">The existing dictionary with elements to use as the basis for the new dictionary.</param>
      /// <param name="proteins">The collection of <see cref="Protein"/> objects used to apply changes to the new dictionary.</param>
      public static ProteinDictionary CreateFromExisting(IDictionary<int, Protein> existingDictionary, IEnumerable<Protein> proteins)
      {
         if (existingDictionary == null) throw new ArgumentNullException(nameof(existingDictionary));
         if (proteins == null) throw new ArgumentNullException(nameof(proteins));

         var dictionary = new ProteinDictionary(existingDictionary);
         var dictionaryChanges = new List<ProteinDictionaryChange>();
         foreach (Protein protein in proteins.Where(x => x.IsValid))
         {
            if (dictionary.ContainsKey(protein.ProjectNumber))
            {
               var propertyChanges = GetChangedProperties(dictionary[protein.ProjectNumber], protein);
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
