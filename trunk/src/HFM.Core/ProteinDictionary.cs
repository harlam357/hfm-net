/*
 * HFM.NET - Core Protein Dictionary
 * Copyright (C) 2009-2011 Ryan Harlamert (harlam357)
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
using System.Linq;

using HFM.Core.DataTypes;

namespace HFM.Core
{
   public interface IProteinDictionary : IDictionary<int, Protein>
   {
      /// <summary>
      /// Load element values into the ProteinDictionary and return an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.
      /// </summary>
      /// <param name="values">The <paramref name="values"/> to load into the ProteinDictionary. <paramref name="values"/> cannot be null.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="values"/> is null.</exception>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.</returns>
      IEnumerable<Proteins.ProteinLoadInfo> Load(IEnumerable<Protein> values);

      #region DataContainer<T>

      void Read();

      List<Protein> Read(string filePath, Plugins.IFileSerializer<List<Protein>> serializer);

      void Write();

      void Write(string filePath, Plugins.IFileSerializer<List<Protein>> serializer);

      #endregion
   }

   public class ProteinDictionary : DataContainer<List<Protein>>, IProteinDictionary
   {
      private readonly Proteins.ProteinDictionary _dictionary;

      public ProteinDictionary(IPreferenceSet prefs)
      {
         _dictionary = new Proteins.ProteinDictionary();

         if (prefs != null && !String.IsNullOrEmpty(prefs.ApplicationDataFolderPath))
         {
            FileName = System.IO.Path.Combine(prefs.ApplicationDataFolderPath, Constants.ProjectInfoFileName);
         }
      }

      #region Properties

      public override Plugins.IFileSerializer<List<Protein>> DefaultSerializer
      {
         get { return new Proteins.TabSerializer(); }
      }

      #endregion

      /// <summary>
      /// Load element values into the ProteinDictionary and return an <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.
      /// </summary>
      /// <param name="values">The <paramref name="values"/> to load into the ProteinDictionary. <paramref name="values"/> cannot be null.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="values"/> is null.</exception>
      /// <returns>An <see cref="T:System.Collections.Generic.IEnumerable`1"/> containing ProteinLoadInfo which details how the ProteinDictionary was changed.</returns>
      public IEnumerable<Proteins.ProteinLoadInfo> Load(IEnumerable<Protein> values)
      {
         return _dictionary.Load(values);
      }

      #region DataContainer<T>

      public override void Read()
      {
         // read the List<Protein>
         base.Read();
         // add each protein to the Dictionary<int, Protein>
         foreach (var protein in Data)
         {
            Add(protein.ProjectNumber, protein);
         }
      }

      public override void Write()
      {
         Data = _dictionary.Values.ToList();

         base.Write();
      }

      public override void Write(string filePath, Plugins.IFileSerializer<List<Protein>> serializer)
      {
         Data = _dictionary.Values.ToList();

         base.Write(filePath, serializer);
      }

      #endregion

      #region IDictionary<int,Protein> Members

      /// <summary>
      /// Adds an element with the specified key and value into the ProteinDictionary.
      /// </summary>
      /// <param name="key">The key of the element to add.</param>
      /// <param name="value">The value of the element to add. The value cannot be null.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="value"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">An element with the same <paramref name="key"/> already exists in the ProteinDictionary.  The <paramref name="value"/> is not valid or the value's ProjectNumber does not match the <paramref name="key"/>.</exception>
      public void Add(int key, Protein value)
      {
         _dictionary.Add(key, value);
      }

      /// <summary>
      /// Determines whether the ProteinDictionary contains an element with the specified key.
      /// </summary>
      /// <returns>
      /// true if the ProteinDictionary contains an element with the key; otherwise, false.
      /// </returns>
      /// <param name="key">The key to locate in the ProteinDictionary.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
      public bool ContainsKey(int key)
      {
         return _dictionary.ContainsKey(key);
      }

      /// <summary>
      /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the ProteinDictionary.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the keys of the ProteinDictionary.
      /// </returns>
      public ICollection<int> Keys
      {
         get { return _dictionary.Keys; }
      }

      /// <summary>
      /// Removes the element with the specified key from the ProteinDictionary.
      /// </summary>
      /// <returns>
      /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the ProteinDictionary.
      /// </returns>
      /// <param name="key">The key of the element to remove.</param>
      public bool Remove(int key)
      {
         return _dictionary.Remove(key);
      }

      /// <summary>
      /// Gets the value associated with the specified key.
      /// </summary>
      /// <returns>
      /// true if the ProteinDictionary contains an element with the specified key; otherwise, false.
      /// </returns>
      /// <param name="key">The key whose value to get.</param>
      /// <param name="value">When this method returns, the value associated with the specified key, if the key is found; otherwise, the default value for the type of the <paramref name="value"/> parameter. This parameter is passed uninitialized.</param>
      public bool TryGetValue(int key, out Protein value)
      {
         return _dictionary.TryGetValue(key, out value);
      }

      /// <summary>
      /// Gets an <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the ProteinDictionary.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.Generic.ICollection`1"/> containing the values in the ProteinDictionary.
      /// </returns>
      public ICollection<Protein> Values
      {
         get { return _dictionary.Values; }
      }

      /// <summary>
      /// Gets or sets the element with the specified key.
      /// </summary>
      /// <returns>
      /// The element with the specified key.
      /// </returns>
      /// <param name="key">The key of the element to get or set.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="value"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">The <paramref name="value"/> is not valid or the value's ProjectNumber does not match the <paramref name="key"/>.</exception>
      /// <exception cref="T:System.Collections.Generic.KeyNotFoundException">The property is retrieved and <paramref name="key"/> is not found.</exception>
      public Protein this[int key]
      {
         get { return _dictionary[key]; }
         set { _dictionary[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<int,Protein>> Members

      /// <summary>
      /// Not Implemented.
      /// </summary>
      /// <exception cref="T:System.NotImplementedException">Not Implemented.</exception>
      void ICollection<KeyValuePair<int, Protein>>.Add(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Removes all items from the ProteinDictionary.
      /// </summary>
      public void Clear()
      {
         _dictionary.Clear();
      }

      /// <summary>
      /// Not Implemented.
      /// </summary>
      /// <exception cref="T:System.NotImplementedException">Not Implemented.</exception>
      bool ICollection<KeyValuePair<int, Protein>>.Contains(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Copies the elements of the ProteinDictionary to an <see cref="T:System.Array"/>, starting at a particular <see cref="T:System.Array"/> index.
      /// </summary>
      /// <param name="array">The one-dimensional <see cref="T:System.Array"/> that is the destination of the elements copied from ProteinDictionary. The <see cref="T:System.Array"/> must have zero-based indexing.</param>
      /// <param name="index">The zero-based index in <paramref name="array"/> at which copying begins.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="array"/> is null.</exception>
      /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than 0.</exception>
      /// <exception cref="T:System.ArgumentException"><paramref name="array"/> is multidimensional.
      ///                     -or-
      ///                     <paramref name="index"/> is equal to or greater than the length of <paramref name="array"/>.
      ///                     -or-
      ///                     The number of elements in the ProteinDictionary is greater than the available space from <paramref name="index"/> to the end of the destination <paramref name="array"/>.
      /// </exception>
      public void CopyTo(KeyValuePair<int, Protein>[] array, int index)
      {
         _dictionary.CopyTo(array, index);
      }

      /// <summary>
      /// Gets the number of elements contained in the ProteinDictionary.
      /// </summary>
      /// <returns>
      /// The number of elements contained in the ProteinDictionary.
      /// </returns>
      public int Count
      {
         get { return _dictionary.Count; }
      }

      /// <summary>
      /// Gets a value indicating whether the ProteinDictionary is read-only.
      /// </summary>
      /// <returns>
      /// true if the ProteinDictionary is read-only; otherwise, false.
      /// </returns>
      bool ICollection<KeyValuePair<int, Protein>>.IsReadOnly
      {
         get { return false; }
      }

      /// <summary>
      /// Not Implemented.
      /// </summary>
      /// <exception cref="T:System.NotImplementedException">Not Implemented.</exception>
      bool ICollection<KeyValuePair<int, Protein>>.Remove(KeyValuePair<int, Protein> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<int,Protein>> Members

      /// <summary>
      /// Returns an enumerator that iterates through the collection.
      /// </summary>
      /// <returns>
      /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>1</filterpriority>
      public IEnumerator<KeyValuePair<int, Protein>> GetEnumerator()
      {
         return _dictionary.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      /// <summary>
      /// Returns an enumerator that iterates through a collection.
      /// </summary>
      /// <returns>
      /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
      /// </returns>
      /// <filterpriority>2</filterpriority>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
