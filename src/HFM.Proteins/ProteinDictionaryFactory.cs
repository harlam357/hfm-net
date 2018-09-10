
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace HFM.Proteins
{
   public static class ProteinDictionaryFactory
   {
      public static ProteinDictionary Create(ICollection<Protein> values)
      {
         if (values == null) throw new ArgumentNullException("values");

         var dictionary = new ProteinDictionary();
         dictionary.LoadInformation = new List<ProteinLoadInfo>(values.Count);
         foreach (Protein protein in values.Where(x => x.IsValid))
         {
            dictionary.LoadInformation.Add(new ProteinLoadInfo(protein.ProjectNumber, ProteinLoadResult.Added));
            dictionary.Add(protein.ProjectNumber, protein);
         }
         return dictionary;
      }

      public static ProteinDictionary CreateFromExisting(IReadOnlyDictionary<int, Protein> existingDictionary, ICollection<Protein> values)
      {
         if (values == null) throw new ArgumentNullException("values");

         var dictionary = new ProteinDictionary();
         dictionary.LoadInformation = new List<ProteinLoadInfo>(values.Count);
         foreach (Protein protein in values.Where(x => x.IsValid))
         {
            if (existingDictionary.ContainsKey(protein.ProjectNumber))
            {
               var changes = GetChangedProperties(existingDictionary[protein.ProjectNumber], protein);
               dictionary.LoadInformation.Add(changes.Count == 0
                  ? new ProteinLoadInfo(protein.ProjectNumber, ProteinLoadResult.NoChange)
                  : new ProteinLoadInfo(protein.ProjectNumber, ProteinLoadResult.Changed, changes));
            }
            else
            {
               dictionary.LoadInformation.Add(new ProteinLoadInfo(protein.ProjectNumber, ProteinLoadResult.Added));
            }
            dictionary[protein.ProjectNumber] = protein;
         }

         return dictionary;
      }

      private static ICollection<ProteinPropertyChange> GetChangedProperties(Protein oldProtein, Protein newProtein)
      {
         var changes = new List<ProteinPropertyChange>();
         var properties = TypeDescriptor.GetProperties(oldProtein);
         foreach (PropertyDescriptor prop in properties)
         {
            object value1 = prop.GetValue(oldProtein);
            object value2 = prop.GetValue(newProtein);
            if (value1 == null || value2 == null)
            {
               continue;
            }
            if (!value1.Equals(value2))
            {
               changes.Add(new ProteinPropertyChange(prop.Name, value1.ToString(), value2.ToString()));
            }
         }
         return changes.AsReadOnly();
      }
   }
}
