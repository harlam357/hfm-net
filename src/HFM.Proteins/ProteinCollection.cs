using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace HFM.Proteins
{
    /// <summary>
    /// Represents a collection of <see cref="Protein"/> values keyed by project number.
    /// </summary>
    public class ProteinCollection : KeyedCollection<int, Protein>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProteinCollection"/> class.
        /// </summary>
        public ProteinCollection()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ProteinCollection"/> class from valid proteins copied from the <paramref name="proteins"/> collection.
        /// </summary>
        public ProteinCollection(IEnumerable<Protein> proteins)
        {
            if (proteins == null) throw new ArgumentNullException(nameof(proteins));

            foreach (Protein protein in proteins.Where(Protein.IsValid))
            {
                Add(protein);
            }
        }

        protected override int GetKeyForItem(Protein item) => item.ProjectNumber;

        /// <summary>
        /// Determines whether the <see cref="ProteinCollection" /> contains a <see cref="Protein"/> with the specified <paramref name="projectNumber"/>.
        /// </summary>
        public bool ContainsKey(int projectNumber)
        {
            if (Dictionary is null) return false;
            return Dictionary.ContainsKey(projectNumber);
        }

        /// <summary>
        /// Gets the <see cref="Protein"/> associated with the specified <paramref name="projectNumber"/>.
        /// </summary>
        public bool TryGetValue(int projectNumber, out Protein protein)
        {
            if (Dictionary is null)
            {
                protein = null;
                return false;
            }
            return Dictionary.TryGetValue(projectNumber, out protein);
        }

        /// <summary>
        /// Updates the <see cref="ProteinCollection"/> from the collection of <see cref="Protein"/> objects.
        /// </summary>
        /// <param name="proteins">The collection of <see cref="Protein"/> objects used to apply changes to this collection.</param>
        public IReadOnlyList<ProteinChange> Update(IEnumerable<Protein> proteins)
        {
            if (proteins == null) throw new ArgumentNullException(nameof(proteins));

            var changes = new List<ProteinChange>();
            foreach (Protein protein in proteins.Where(Protein.IsValid))
            {
                if (Dictionary != null && Dictionary.ContainsKey(protein.ProjectNumber))
                {
                    var propertyChanges = GetChangedProperties(Dictionary[protein.ProjectNumber], protein);
                    if (propertyChanges.Count > 0)
                    {
                        changes.Add(ProteinChange.Property(protein.ProjectNumber, propertyChanges));
                        Dictionary[protein.ProjectNumber] = protein;
                    }
                    else
                    {
                        changes.Add(ProteinChange.None(protein.ProjectNumber));
                    }
                }
                else
                {
                    changes.Add(ProteinChange.Add(protein.ProjectNumber));
                    Add(protein);
                }

            }
            return changes;
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
