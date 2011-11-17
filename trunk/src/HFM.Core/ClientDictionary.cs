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
   public sealed class ClientDictionary : IDictionary<string, IClient>
   {
      private readonly Dictionary<string, IClient> _clientDictionary;

      public bool Dirty { get; private set; }

      public ClientDictionary()
      {
         _clientDictionary = new Dictionary<string, IClient>();
      }

      /// <summary>
      /// Method to load IClient objects from file.  This method will set the dirty flag to false.
      /// </summary>
      /// <param name="clients">Enumerable collection of IClient objects.</param>
      public void LoadClients(IEnumerable<IClient> clients)
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

      public event EventHandler DictionaryChanged;

      private void OnDictionaryChanged(EventArgs e)
      {
         if (DictionaryChanged != null)
         {
            DictionaryChanged(this, e);
         }
      }

      #region IDictionary<string,IClient> Implementation

      #region IDictionary<string,IClient> Members

      public void Add(string key, IClient value)
      {
         _clientDictionary.Add(key, value);

         Dirty = true;
         OnDictionaryChanged(EventArgs.Empty);
      }

      public bool ContainsKey(string key)
      {
         return _clientDictionary.ContainsKey(key);
      }

      public ICollection<string> Keys
      {
         get { return _clientDictionary.Keys; }
      }

      public bool Remove(string key)
      {
         bool result = _clientDictionary.Remove(key);
         if (result)
         {
            Dirty = true;
            OnDictionaryChanged(EventArgs.Empty);   
         }
         return result;
      }

      public bool TryGetValue(string key, out IClient value)
      {
         return _clientDictionary.TryGetValue(key, out value);
      }

      public ICollection<IClient> Values
      {
         get { return _clientDictionary.Values; }
      }

      public IClient this[string key]
      {
         get { return _clientDictionary[key]; }
         set { _clientDictionary[key] = value; }
      }

      #endregion

      #region ICollection<KeyValuePair<string,IClient>> Members

      void ICollection<KeyValuePair<string,IClient>>.Add(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      /// <summary>
      /// Removes all keys and values from the collection and set the dirty flag to false.
      /// </summary>
      public void Clear()
      {
         bool hasValues = Count != 0;
         _clientDictionary.Clear();

         Dirty = false;
         if (hasValues)
         {
            OnDictionaryChanged(EventArgs.Empty);   
         }
      }

      bool ICollection<KeyValuePair<string,IClient>>.Contains(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      void ICollection<KeyValuePair<string,IClient>>.CopyTo(KeyValuePair<string, IClient>[] array, int arrayIndex)
      {
         throw new NotImplementedException();
      }

      public int Count
      {
         get { return _clientDictionary.Count; }
      }

      bool ICollection<KeyValuePair<string,IClient>>.IsReadOnly
      {
         get { throw new NotImplementedException(); }
      }

      bool ICollection<KeyValuePair<string,IClient>>.Remove(KeyValuePair<string, IClient> item)
      {
         throw new NotImplementedException();
      }

      #endregion

      #region IEnumerable<KeyValuePair<string,IClient>> Members

      public IEnumerator<KeyValuePair<string, IClient>> GetEnumerator()
      {
         return _clientDictionary.GetEnumerator();
      }

      #endregion

      #region IEnumerable Members

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }

      #endregion

      #endregion
   }
}
