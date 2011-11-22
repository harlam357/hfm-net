/*
 * HFM.NET - Client Dictionary Class
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

namespace HFM.Core
{
   public interface IClientDictionary : IDictionary<string, IClient>
   {
      event EventHandler DictionaryChanged;

      bool IsDirty { get; }

      /// <summary>
      /// Clears the dictionary and loads a collection of IClient objects.
      /// </summary>
      /// <remarks>This method will clear the dictionary, per implementaiton of the Clear() method, and raise the DictionaryChanged event if items were loaded.</remarks>
      /// <param name="clients"><see cref="T:System.Collections.Generic.IEnumerable`1"/> collection of IClient objects.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="clients"/> is null.</exception>
      void Load(IEnumerable<IClient> clients);

      #region IDictionary<string,IClient> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Adds an <see cref="T:HFM.Core.IClient"/> element with the provided key and value to the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event.</remarks>
      /// <param name="key">The string to use as the key of the element to add.</param>
      /// <param name="value">The <see cref="T:HFM.Core.IClient"/> object to use as the value of the element to add.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> or <paramref name="value"/> is null.</exception>
      /// <exception cref="T:System.ArgumentException">An element with the same key already exists in the <see cref="T:System.Collections.Generic.IDictionary`2"/>.</exception>
      new void Add(string key, IClient value);

      /// <summary>
      /// Removes the <see cref="T:HFM.Core.IClient"/> element with the specified key from the <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </summary>
      /// <returns>
      /// true if the element is successfully removed; otherwise, false.  This method also returns false if <paramref name="key"/> was not found in the original <see cref="T:System.Collections.Generic.IDictionary`2"/>.
      /// </returns>
      /// <remarks>Sets the IsDirty property to true and raises the DictionaryChanged event when successful.</remarks>
      /// <param name="key">The key of the element to remove.</param>
      /// <exception cref="T:System.ArgumentNullException"><paramref name="key"/> is null.</exception>
      new bool Remove(string key);
      
      #endregion

      #region ICollection<KeyValuePair<string,IClient>> Members

      // Override Default Interface Documentation

      /// <summary>
      /// Removes all items from the <see cref="T:System.Collections.Generic.ICollection`1"/>.
      /// </summary>
      /// <remarks>Sets the IsDirty property to false and raises the DictionaryChanged event if values existed before this call.</remarks>
      new void Clear();

      #endregion
   }

   public sealed class ClientDictionary : IClientDictionary
   {
      public event EventHandler DictionaryChanged;

      private void OnDictionaryChanged(EventArgs e)
      {
         if (DictionaryChanged != null)
         {
            DictionaryChanged(this, e);
         }
      }

      public bool IsDirty { get; private set; }

      private readonly Dictionary<string, IClient> _clientDictionary;

      public ClientDictionary()
      {
         _clientDictionary = new Dictionary<string, IClient>();
      }

      public void Load(IEnumerable<IClient> clients)
      {
         if (clients == null) throw new ArgumentNullException("clients");

         Clear();

         int added = 0;
         // add each instance to the collection
         foreach (var instance in clients)
         {
            if (instance != null)
            {
               _clientDictionary.Add(instance.Settings.Name, instance);
               added++;
            }
         }
         if (added != 0)
         {
            OnDictionaryChanged(EventArgs.Empty);
         }
      }

      #region IDictionary<string,IClient> Members

      public void Add(string key, IClient value)
      {
         if (key == null) throw new ArgumentNullException("key");
         if (value == null) throw new ArgumentNullException("value");

         _clientDictionary.Add(key, value);

         IsDirty = true;
         OnDictionaryChanged(EventArgs.Empty);
      }

      [CoverageExclude]
      public bool ContainsKey(string key)
      {
         return _clientDictionary.ContainsKey(key);
      }

      public ICollection<string> Keys
      {
         [CoverageExclude]
         get { return _clientDictionary.Keys; }
      }

      public bool Remove(string key)
      {
         bool result = _clientDictionary.Remove(key);
         if (result)
         {
            IsDirty = true;
            OnDictionaryChanged(EventArgs.Empty);   
         }
         return result;
      }

      [CoverageExclude]
      public bool TryGetValue(string key, out IClient value)
      {
         return _clientDictionary.TryGetValue(key, out value);
      }

      public ICollection<IClient> Values
      {
         [CoverageExclude]
         get { return _clientDictionary.Values; }
      }

      public IClient this[string key]
      {
         [CoverageExclude]
         get { return _clientDictionary[key]; }
         [CoverageExclude]
         set { _clientDictionary[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<string,IClient>> Members

      [CoverageExclude]
      void ICollection<KeyValuePair<string, IClient>>.Add(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      public void Clear()
      {
         bool hasValues = Count != 0;

         _clientDictionary.Clear();

         IsDirty = false;
         if (hasValues)
         {
            OnDictionaryChanged(EventArgs.Empty);   
         }
      }

      [CoverageExclude]
      bool ICollection<KeyValuePair<string, IClient>>.Contains(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      [CoverageExclude]
      void ICollection<KeyValuePair<string, IClient>>.CopyTo(KeyValuePair<string, IClient>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         [CoverageExclude]
         get { return _clientDictionary.Count; }
      }

      bool ICollection<KeyValuePair<string,IClient>>.IsReadOnly
      {
         [CoverageExclude]
         get { return false; }
      }

      [CoverageExclude]
      bool ICollection<KeyValuePair<string, IClient>>.Remove(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<string,IClient>> Members

      [CoverageExclude]
      public IEnumerator<KeyValuePair<string, IClient>> GetEnumerator()
      {
         return _clientDictionary.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      [CoverageExclude]
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion
   }
}
